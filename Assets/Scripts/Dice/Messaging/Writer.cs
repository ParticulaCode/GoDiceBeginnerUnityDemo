using System.Collections.Generic;
using GoDice.Bluetooth.Devices;
using GoDice.Bluetooth.Operations;

namespace GoDice.App.Modules.Dice.Messaging
{
    internal class Writer
    {
        private static class Request
        {
            public static readonly byte[] Reset = { WriteProtocol.Reset };

            public static readonly byte[] Battery = { WriteProtocol.Battery };
            public static readonly byte[] Color = { WriteProtocol.Color.Get };
            public static readonly byte[] LastStableValue = { WriteProtocol.LastStableValue };
        }

        private readonly Device _device;

        public Writer(Device device) => _device = device;

        /// We don't initialize the die with values from this message
        /// But we still need to send it to get the 3 response messages about current die's state
        public void SendInitializationMessage()
        {
            var payloadStub = new byte[9];
            var message = new List<byte>(payloadStub.Length + 1)
            {
                WriteProtocol.Initialization
            };
            message.AddRange(payloadStub);

            Send(message.ToArray(), OperationType.Initialization);
        }

        public void RequestBatteryCharge() => Send(Request.Battery, OperationType.BatteryRequest);

        public void RequestLastStableValue() =>
            Send(Request.LastStableValue, OperationType.LastStableValueRequest);

        public void Reset() => Send(Request.Reset, OperationType.Reset);

        public void RequestColor() => Send(Request.Color, OperationType.ColorRequest);

        public void Send(byte[] data, OperationType type) => _device.SendData(data, type);
    }
}