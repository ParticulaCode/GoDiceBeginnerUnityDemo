using System.Linq;
using FrostLib;
using GoDice.Bluetooth.Bridge;
using GoDice.Bluetooth.Devices;
using UnityEngine;

namespace GoDice
{
    internal class ResetCommand : ICommand
    {
        private static ServiceLocator Locator => ServiceLocator.Instance;
        private static DevicesHolder DevicesHolder => Locator.Get<DevicesHolder>();
        private static BluetoothBridge Bridge => Locator.Get<BluetoothBridge>();

        public void Execute()
        {
            var devices = DevicesHolder.GetDevices().ToArray();
            DevicesHolder.Clear();

            foreach (var device in devices)
            {
                device.Dispose();
                Bridge.DisconnectPeripheral(device.Address);
            }

            Object.FindAnyObjectByType<LogView>().Clear();
        }
    }
}