using FrostLib;
using UnityEngine;

namespace GoDice.Bluetooth.Characteristics
{
    internal class Reader
    {
        public readonly Signal<byte[]> OnDataSignal = new();
        public readonly Signal<string> OnNotificationSignal = new();

        private readonly Characteristic _characteristic;

        public Reader(Characteristic characteristic) => _characteristic = characteristic;

        public void OnNotification(string address, string notification)
        {
            if (_characteristic.Address != address)
                return;

            OnNotificationSignal?.Dispatch(notification);
        }

        public void OnReceive(string address, string characteristic, byte[] data)
        {
            if (_characteristic.Address != address)
                return;

            if (data.Length == 0)
            {
                LogMessage("Receive ignored! Data is empty");
                return;
            }

            LogMessage($"Data received: {BytesPrinter.Print(data)}");
            OnDataSignal?.Dispatch(data);
        }

        private void LogMessage(string text) =>
            Debug.Log($"[Reader ({_characteristic.Address})] {text}");
    }
}