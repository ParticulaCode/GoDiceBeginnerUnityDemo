using System.Text.RegularExpressions;
using FrostLib;
using GoDice.Bluetooth;
using GoDice.Bluetooth.Bridge;
using GoDice.Bluetooth.Devices;
using GoDice.Bluetooth.Operations;
using GoDice.Bluetooth.Scan;
using UnityEngine;

namespace GoDice
{
    internal class Bootstrapper : MonoBehaviour
    {
        private void Awake()
        {
            var provider = ServiceLocator.Instance;
            var routineRunner = RoutineRunner.Create();
            provider.Provide(routineRunner);
            provider.Provide(new DevicesHolder());

            var bluetoothBridge = new SetupBluetoothBridgeCommand().Execute();
            provider.Provide(bluetoothBridge);

            provider.Provide(new Scanner(new Regex("GoDice.*")));

            var operationsRunner = new OperationsRunner(bluetoothBridge, routineRunner, 10);
            provider.Provide(new ConnectionEstablisher(operationsRunner));
        }

        public void Scan()
        {
            new ResetCommand().Execute();
            new ScanForDevicesCommand().Execute();
        }
    }
}