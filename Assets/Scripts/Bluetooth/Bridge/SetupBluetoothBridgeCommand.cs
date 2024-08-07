using FrostLib;
using GoDice.Bluetooth.Devices;
using UnityEngine;

namespace GoDice.Bluetooth.Bridge
{
    internal class SetupBluetoothBridgeCommand : ICommand<BluetoothBridge>
    {
        private static ServiceLocator Servicer => ServiceLocator.Instance;
        private static DevicesHolder DevicesHolder => Servicer.Get<DevicesHolder>();

        private BluetoothBridge _bridge;

        public BluetoothBridge Execute()
        {
            _bridge = new BluetoothBridge();
            _bridge.Initialize(OnSuccess, OnError, OnPeripheralDisconnected, null);

            return _bridge;
        }

        private static void OnSuccess() => Debug.Log("Bluetooth bridge initialized");

        private void OnError(string error)
        {
            Debug.Log($"==== Bt Error: {error} ====");

            if (error.Contains("Bluetooth LE Not Enabled"))
                _bridge.ChangeDeviceBluetoothState(true);
        }

        private static void OnPeripheralDisconnected(string address)
        {
            Debug.Log($"Device disconnected: {address}.");

            var device = DevicesHolder.Get(address);
            ProcessConnectionLossIfDeviceExists(device);
        }

        private static void ProcessConnectionLossIfDeviceExists(Device device)
        {
            if (device == null)
                return;

            device.State = DeviceState.Disconnected;
        }
    }
}