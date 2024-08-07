using System.Collections.Generic;
using GoDice.App.Modules.Dice.Messaging;
using GoDice.Bluetooth.Operations;
using UnityEngine;

namespace GoDice.Dice.Led
{
    internal class LedController
    {
        private static readonly Dictionary<ToggleMode, byte[]> Messages =
            new()
            {
                { ToggleMode.Discover, MessageComposer.Discovery }
            };

        private static byte[] OpenMessages(OpenMode mode, Color color)
        {
            byte[] result = null;
            switch (mode)
            {
                case OpenMode.Dual:
                    result = MessageComposer.ConstantTwoLed(color);
                    break;
                case OpenMode.Single:
                    result = MessageComposer.ConstantOneLed(color);
                    break;
            }

            return result;
        }

        private readonly Writer _writer;

        public LedController(Writer writer) => _writer = writer;

        public void Blink(ToggleMode mode)
        {
            var message = new List<byte> { WriteProtocol.Led.Toggle };
            message.AddRange(Messages[mode]);

            _writer.Send(message.ToArray(), OperationType.LedToggle);
        }

        public void Blink(byte blinksAmount, Color color, float onDuration,
            float offDuration, bool isMixed)
        {
            var message = new List<byte> { WriteProtocol.Led.Toggle };
            message.AddRange(
                MessageComposer.ComposeToggle(
                    blinksAmount,
                    color,
                    onDuration,
                    offDuration,
                    isMixed));

            _writer.Send(message.ToArray(), OperationType.LedToggle);
        }

        public void CloseAllLeds() =>
            _writer.Send(new[] { WriteProtocol.Led.Off }, OperationType.LedOff);

        public void OpenLed(OpenMode mode, Color color) =>
            _writer.Send(OpenMessages(mode, color), OperationType.LedConstant);
    }
}