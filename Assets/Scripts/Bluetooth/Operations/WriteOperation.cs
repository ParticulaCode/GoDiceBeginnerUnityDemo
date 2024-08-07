using System.Linq;
using GoDice.Bluetooth.Bridge;
using GoDice.Bluetooth.Characteristics;
using UnityEngine;

namespace GoDice.Bluetooth.Operations
{
    internal sealed class WriteOperation : IOperation
    {
        public OperationType Type { get; }
        public bool IsDone { get; private set; }

        private readonly Characteristic _characteristic;
        private readonly byte[] _data;

        public WriteOperation(Characteristic characteristic, byte[] data, OperationType type)
        {
            _characteristic = characteristic;
            _data = data;
            Type = type;
        }

        public void Perform(BluetoothBridge bluetoothBridge)
        {
            Debug.Log(
                $"[WriteOperation ({_characteristic.Address})] Sending data: {BytesPrinter.Print(_data.Select(sb => sb).ToArray())}");
            bluetoothBridge.WriteCharacteristic(_characteristic, _data, true, OnSendSuccess);
        }

        public void Abort()
        {
        }

        public bool CanBePruned(string address) => _characteristic.Address == address;

        private void OnSendSuccess(string service) => IsDone = true;

        #region Object overrides

        public override bool Equals(object obj)
        {
            if (!(obj is WriteOperation other))
                return false;

            return Equals(other);
        }

        private bool Equals(WriteOperation other) =>
            Type == other.Type
            && _characteristic.Equals(other._characteristic)
            && _data.SequenceEqual(other._data);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _characteristic.GetHashCode();
                hashCode = (hashCode * 397) ^ (_data != null ? _data.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int) Type;
                return hashCode;
            }
        }

        public override string ToString() => $"{Type} ({_characteristic.Address})";

        #endregion
    }
}