using System.Collections.Generic;
using FrostLib;
using GoDice.Bluetooth.Devices;
using GoDice.Dice.Commands;
using UnityEngine;

namespace GoDice.Bluetooth.Scan
{
    internal class ScanForDevicesCommand : ICommand
    {
        private static ServiceLocator Locator => ServiceLocator.Instance;
        private static RoutineRunner Runner => Locator.Get<RoutineRunner>();
        private static Scanner Scanner => Locator.Get<Scanner>();

        private static ConnectionEstablisher ConnectionEstablisher =>
            Locator.Get<ConnectionEstablisher>();

        private readonly List<Device> _foundDevices = new();

        public void Execute()
        {
            Scanner.OnDeviceFoundSignal.Subscribe(OnNewDeviceFound);
            Scanner.OnScanFinished.SubscribeOnce(() =>
                Scanner.OnDeviceFoundSignal.Unsubscribe(OnNewDeviceFound));

            Runner.StartRoutine(Scanner.Scan());
        }

        private void OnNewDeviceFound(Device device)
        {
            if (_foundDevices.Contains(device))
                return;

            Debug.Log($"Device found: {device}");

            _foundDevices.Add(device);
            ConnectionEstablisher.Connect(device, isSuccess => OnConnection(device, isSuccess));
        }

        private void OnConnection(Device device, bool isSuccess)
        {
            if (!isSuccess)
            {
                Debug.Log($"Failed to connect to {device}");
                return;
            }

            Debug.Log($"Connected to {device}");

            var die = new CreateDieCommand(device).Execute();
            die.DemoBlink();
            die.SendInitializationMessage();
        }
    }
}