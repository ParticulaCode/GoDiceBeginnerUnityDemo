using System;
using FrostLib;
using GoDice.Bluetooth.Devices;
using GoDice.Bluetooth.Operations;
using UnityEngine;

namespace GoDice.Bluetooth
{
    internal class ConnectionEstablisher
    {
        private static ServiceLocator Servicer => ServiceLocator.Instance;
        private static DevicesHolder DevicesHolder => Servicer.Get<DevicesHolder>();

        private readonly OperationsRunner _operationsRunner;

        public ConnectionEstablisher(OperationsRunner operationsRunner) =>
            _operationsRunner = operationsRunner;

        public void Connect(Device device, Action<bool> cb)
        {
            if (device.State != DeviceState.Disconnected)
            {
                Debug.Log(
                    $"Attempt to establish connection with not Disconnected device: {device}. Aborted");
                return;
            }

            var operation = new ConnectionOperation(_operationsRunner, device,
                () => IsConnectionStillRequired(device.Address));

            operation.OnFinishedSignal.SubscribeOnce(isSuccess =>
                OnConnectionOperationFinished(isSuccess, device, cb));

            _operationsRunner.Schedule(operation);
        }

        private static bool IsConnectionStillRequired(string deviceAddress)
        {
            var device = DevicesHolder.Get(deviceAddress);
            return device == null || device.State == DeviceState.Disconnected;
        }

        private void OnConnectionOperationFinished(bool isSuccess, Device device, Action<bool> cb)
        {
            if (isSuccess)
            {
                if (!DevicesHolder.Exists(device.Address))
                    DevicesHolder.Add(device);

                device.OnStateChangedSignal.Subscribe(OnDeviceConnectionChanged);
            }

            cb?.Invoke(isSuccess);
        }

        private void OnDeviceConnectionChanged(Device device, DeviceState state)
        {
            if (state != DeviceState.Disconnected)
                return;

            _operationsRunner.PruneOperationsByAddress(device.Address);
        }
    }
}