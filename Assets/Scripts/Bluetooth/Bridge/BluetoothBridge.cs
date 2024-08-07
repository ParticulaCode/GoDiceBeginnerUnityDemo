using System;
using FrostLib;
using GoDice.Bluetooth.Characteristics;
using UnityEngine;

namespace GoDice.Bluetooth.Bridge
{
    internal class BluetoothBridge
    {
        private Action<string> _onDisconnect;
        private Signal<string> _lastConnectionFailureSignal;

        private bool _scanIsRunning;

        public void Initialize(Action onSuccess, Action<string> onError,
            Action<string> onPeripheralDisconnected, Action<string> onLog)
        {
            _onDisconnect = onPeripheralDisconnected;
            BluetoothLEHardwareInterface.Initialize(true, false, onSuccess, onError);
        }

        public void ChangeDeviceBluetoothState(bool enable) =>
            BluetoothLEHardwareInterface.BluetoothEnable(enable);

        public void ConnectToPeripheral(string name, Action<string> connectAction,
            Action<string, string> serviceAction,
            Action<string, string, string> characteristicAction,
            Signal<string> onFailure)
        {
            _lastConnectionFailureSignal = onFailure;

            BluetoothLEHardwareInterface.ConnectToPeripheral(name, connectAction, serviceAction,
                characteristicAction, OnDisconnect);
        }

        private void OnDisconnect(string address)
        {
            _lastConnectionFailureSignal?.Dispatch(address);
            _onDisconnect?.Invoke(address);
        }

        public void DisconnectPeripheral(string address) =>
            BluetoothLEHardwareInterface.DisconnectPeripheral(address, null);

        public void ScanForPeripherals(Action<string, string> action,
            Action<string, string, int, byte[]> actionAdvertisingInfo, bool rssiOnly)
        {
            if (_scanIsRunning)
                return;

            try
            {
                _scanIsRunning = true;
                BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(null, action,
                    actionAdvertisingInfo, rssiOnly, false);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("android.bluetooth.le.BluetoothLeScanner.startScan"))
                {
                    Debug.Log(
                        $"Seems like bluetooth is disabled. Got an error from android: {e.Message}, \n{e.StackTrace}");
                    return;
                }

                Debug.LogException(e);
            }
        }

        public void StopScan()
        {
            //BLE plugin will throw a null ref error if Stop is called while scan is not running
            if (!_scanIsRunning)
                return;

            try
            {
                _scanIsRunning = false;
                BluetoothLEHardwareInterface.StopScan();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void SubscribeCharacteristicWithDeviceAddress(Characteristic ch,
            Action<string, string> notificationAction, Action<string, string, byte[]> action)
            => BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress(ch.Address,
                ch.Service, ch.Id, notificationAction, action);

        ///OnSuccess will return device address
        public void WriteCharacteristic(Characteristic ch, byte[] data, bool withResponse,
            Action<string> onSuccess)
            => BluetoothLEHardwareInterface.WriteCharacteristic(ch.Address, ch.Service, ch.Id, data,
                data.Length, withResponse, onSuccess);
    }
}