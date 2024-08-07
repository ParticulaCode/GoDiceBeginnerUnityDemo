using GoDice.App.Modules.Dice.Messaging;
using GoDice.App.Modules.Dice.Shells;
using GoDice.Bluetooth.Devices;
using GoDice.Dice.Led;
using UnityEngine;

namespace GoDice.Dice
{
    internal class Die
    {
        private readonly ColorType _color;
        private readonly Device _device;
        private readonly LedController _ledController;
        private readonly ShellController _shellController;
        private readonly Writer _writer;
        private readonly Reader _reader;
        private int _value;

        public Die(ColorType color, Device device, LedController ledController,
            ShellController shellController,
            Reader reader, Writer writer)
        {
            _color = color;

            _device = device;
            _ledController = ledController;
            _shellController = shellController;

            _reader = reader;
            _writer = writer;

            _device.OnDataReceivedSignal.Subscribe(OnDataReceived);
        }

        private void OnDataReceived(byte[] data)
        {
            switch (_reader.Read(data))
            {
                case Response.Undefined:
                    break;
                case Response.Battery:
                    UpdateBattery();
                    break;
                case Response.Roll:
                    StartRolling();
                    break;
                case Response.RollEnd:
                case Response.FakeStable:
                    EndRoll();
                    break;
                case Response.TiltStable:
                    EndRollAsTilt();
                    break;
                case Response.MoveStable:
                    BecomeStable();
                    break;
                case Response.ChargingStarted:
                    SetCharging(true);
                    break;
                case Response.ChargingStopped:
                    SetCharging(false);
                    break;
                default:
                    LogDie("Incoming data handling not implemented for demo.");
                    break;
            }
        }

        #region Rolls

        private void StartRolling() => LogDie("Roll started");

        private void EndRoll()
        {
            AssignValueByLatestAxes();
            LogDie($"Roll ended: {_value}");
        }

        private void EndRollAsTilt()
        {
            AssignValueByLatestAxes();
            LogDie($"Roll ended tilted: {_value}");
        }

        private void BecomeStable()
        {
            AssignValueByLatestAxes();
            LogDie($"Stable: {_value}");
        }

        private void AssignValueByLatestAxes() => SetValue(GetValueByLastAxes());

        private void SetValue(int newValue) => _value = newValue;

        private int GetValueByLastAxes() => _shellController.GetValue(_reader.Axes);

        public void SyncValue() => _writer.RequestLastStableValue();

        #endregion

        #region Battery

        public void RequestBatteryCharge() => _writer.RequestBatteryCharge();

        private void SetCharging(bool isCharging) => LogDie($"IsCharging: {isCharging}");

        private void UpdateBattery() => LogDie($"Battery updated: {_reader.Battery}");

        #endregion

        public void SendInitializationMessage() => _writer.SendInitializationMessage();

        public void DemoBlink() => _ledController.Blink(ToggleMode.Discover);

        private void LogDie(string message) =>
            Debug.Log($"[{_color}Die ({_device.Address})] {message}");
    }
}