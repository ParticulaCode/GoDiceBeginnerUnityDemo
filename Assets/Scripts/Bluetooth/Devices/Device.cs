using System;
using FrostLib;
using GoDice.Bluetooth.Characteristics;
using GoDice.Bluetooth.Operations;
using UnityEngine;

namespace GoDice.Bluetooth.Devices
{
    internal class Device : IDisposable
    {
        public readonly string Address;
        public readonly string Name;
        public readonly int RSSI;

        public DeviceState State
        {
            get => _state;
            set
            {
                var prevState = _state;
                _state = value;

                if (_state == DeviceState.Disconnected)
                {
                    AttachReader(null);
                    AttachWriter(null);
                }

                if (prevState == _state)
                    return;

                Debug.Log($"[Device ({Address})] {prevState} --> {_state} ");
                OnStateChangedSignal.Dispatch(this, _state);
            }
        }

        private DeviceState _state = DeviceState.Disconnected;

        public readonly Signal<Device, DeviceState> OnStateChangedSignal = new();
        public readonly Signal<byte[]> OnDataReceivedSignal = new();

        private Reader _reader;
        private Writer _writer;

        public Device(string address, string name, int rssi)
        {
            Address = address;
            Name = GetDeviceNameWithoutVersion(name);
            RSSI = rssi;
        }

        private static string GetDeviceNameWithoutVersion(string name) =>
            name.Contains("_v") ? name.Remove(name.Length - 4, 4) : name;

        public void AttachReader(Reader reader)
        {
            if (_reader == reader)
                return;

            _reader?.OnDataSignal.Unsubscribe(OnDataReceived);
            _reader = reader;
            _reader?.OnDataSignal.Subscribe(OnDataReceived);
        }

        private void OnDataReceived(byte[] data) => OnDataReceivedSignal.Dispatch(data);

        public void AttachWriter(Writer writer) => _writer = writer;

        public void SendData(byte[] data, OperationType type) => _writer?.Send(data, type);

        public void Dispose()
        {
            State = DeviceState.Disconnected;

            _reader?.OnDataSignal.Unsubscribe(OnDataReceived);

            OnStateChangedSignal.Reset();
            OnDataReceivedSignal.Reset();
        }

        #region Object overrides

        public override string ToString() =>
            $"Device (Name = {Name}, Address = {Address}, RSSI = {RSSI})";

        public override bool Equals(object obj)
        {
            if (!(obj is Device other))
                return false;

            return Equals(other);
        }

        private bool Equals(Device other) => Address == other.Address;

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) _state;
                hashCode = (hashCode * 397) ^ (_reader != null ? _reader.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_writer != null ? _writer.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ RSSI;
                hashCode = (hashCode * 397)
                           ^ (OnStateChangedSignal != null ? OnStateChangedSignal.GetHashCode() : 0);
                hashCode = (hashCode * 397)
                           ^ (OnDataReceivedSignal != null ? OnDataReceivedSignal.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion
    }
}