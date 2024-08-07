using System;
using FrostLib;
using GoDice.Bluetooth.Bridge;
using GoDice.Bluetooth.Characteristics;
using GoDice.Bluetooth.Devices;
using UnityEngine;

namespace GoDice.Bluetooth.Operations
{
    // From my extensive tests, a device will fail all write operations up 
    // until the point where a notification is received in the read characteristic.
    // The notification just contains the UUID of the read characteristic.
    // So that is why we have to wait for both characteristics and the notification 
    // from the read characteristic in order to deem the device readable and writable
    internal class ConnectionOperation : IOperation
    {
        public OperationType Type => OperationType.Connection;
        public bool IsDone { get; private set; }

        public readonly Signal<bool> OnFinishedSignal = new();

        private readonly OperationsRunner _operationsRunner;
        private readonly Action<bool> _onConnection;
        private readonly Device _device;
        private readonly Func<bool> _connectionStillRequiredCheck;
        private readonly Signal<string> _failureSignal = new();

        private BluetoothBridge _bluetoothBridge;
        private Reader _reader;
        private Writer _writer;
        private bool _gotNotification;

        public ConnectionOperation(OperationsRunner operationsRunner, Device device,
            Func<bool> connectionStillRequiredCheck)
        {
            _operationsRunner = operationsRunner;
            _device = device;
            _connectionStillRequiredCheck = connectionStillRequiredCheck;
        }

        public bool CanBePruned(string address) => false;

        public void Perform(BluetoothBridge bluetoothBridge)
        {
            if (!_connectionStillRequiredCheck.Invoke())
            {
                Debug.Log($"Connection to {_device} is not required anymore. Aborting.");
                Finish(false);
                return;
            }

            _bluetoothBridge = bluetoothBridge;
            _device.State = DeviceState.Connecting;
            _failureSignal.SubscribeOnce(OnFailure);

            _bluetoothBridge.ConnectToPeripheral(_device.Address, OnConnectionStarted, null,
                OnCharacteristic,
                _failureSignal);
        }

        private void OnFailure(string address)
        {
            if (address != _device.Address)
                return;

            Abort();
        }

        public void Abort()
        {
            Debug.Log($"Failed to establish connection with {_device}. Aborting.");

            Finish(false);
        }

        private void Finish(bool isSuccess)
        {
            IsDone = true;
            _device.State = isSuccess ? DeviceState.Connected : DeviceState.Disconnected;
            OnFinishedSignal.Dispatch(isSuccess);
            _failureSignal.Reset();
        }

        private void OnConnectionStarted(string address)
        {
            Debug.Log($"Connecting to {_device}");
            CheckState();
        }

        private void CheckState()
        {
            if (IsDone || _reader == null || _writer == null || !_gotNotification)
                return;

            Debug.Log($"Connected to {_device}");
            Finish(true);
        }

        private void OnCharacteristic(string address, string serviceUUID, string characteristicUUID)
        {
            if (serviceUUID.ToUpper() != UUID.Service || address != _device.Address)
                return;

            var upper = characteristicUUID.ToUpper();
            var ch = new Characteristic(address, serviceUUID, characteristicUUID);
            if (upper == UUID.ReadCharacteristic)
            {
                _reader = new Reader(ch);
                _reader.OnNotificationSignal.SubscribeOnce(OnNotification);
                _device.AttachReader(_reader);

                _bluetoothBridge.SubscribeCharacteristicWithDeviceAddress(ch, _reader.OnNotification,
                    _reader.OnReceive);
            }
            else if (upper == UUID.WriteCharacteristic)
            {
                _writer = new Writer(ch, _operationsRunner);
                _device.AttachWriter(_writer);
            }

            CheckState();
        }

        private void OnNotification(string notification)
        {
            _gotNotification = true;
            CheckState();
        }

        #region Object overrides

        public override bool Equals(object obj)
        {
            if (!(obj is ConnectionOperation other))
                return false;

            return Equals(other);
        }

        private bool Equals(ConnectionOperation other) => _device.Equals(other._device);

        public override int GetHashCode() => _device != null ? _device.GetHashCode() : 0;

        public override string ToString() => $"{_device.Address}, {Type}";

        #endregion
    }
}