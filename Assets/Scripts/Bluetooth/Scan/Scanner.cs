using System.Collections;
using System.Text.RegularExpressions;
using FrostLib;
using GoDice.Bluetooth.Bridge;
using GoDice.Bluetooth.Devices;
using UnityEngine;

namespace GoDice.Bluetooth.Scan
{
    internal class Scanner
    {
        public Signal OnScanFinished { get; } = new();
        public Signal<Device> OnDeviceFoundSignal { get; } = new();

        private static ServiceLocator Servicer => ServiceLocator.Instance;
        private static BluetoothBridge BluetoothBridge => Servicer.Get<BluetoothBridge>();
        private static DevicesHolder Devices => Servicer.Get<DevicesHolder>();

        private const int ScanDuration = 10;

        private readonly Regex _deviceFilter;

        public Scanner(Regex deviceFilter) => _deviceFilter = deviceFilter;

        public virtual IEnumerator Scan()
        {
            StartScan();

            yield return new WaitForSecondsRealtime(ScanDuration);

            StopScan();
        }

        private void StartScan()
        {
            Debug.Log("Scan started");

            BluetoothBridge.ScanForPeripherals(null, OnReceiveDeviceAddressWithAdvertising, true);
        }

        private void StopScan()
        {
            Debug.Log("Scan ended");
            BluetoothBridge.StopScan();
            OnScanFinished.Dispatch();
        }

        private void OnReceiveDeviceAddressWithAdvertising(string address, string name, int rssi,
            byte[] _)
        {
            if (!IsGoDiceDevice(name))
                return;

            OnDeviceFound(address, name, rssi);
        }

        private bool IsGoDiceDevice(string name) => _deviceFilter.IsMatch(name);

        private void OnDeviceFound(string address, string name, int rssi)
        {
            var device = Devices.Get(address);
            if (device is { State: DeviceState.Disconnected })
            {
                // Reconnected
                OnDeviceFoundSignal.Dispatch(device);
                return;
            }

            OnDeviceFoundSignal.Dispatch(new Device(address, name, rssi));
        }
    }
}