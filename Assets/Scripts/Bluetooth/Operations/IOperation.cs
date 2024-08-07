using GoDice.Bluetooth.Bridge;

namespace GoDice.Bluetooth.Operations
{
    internal interface IOperation
    {
        OperationType Type { get; }
        bool IsDone { get; }

        void Perform(BluetoothBridge bluetoothBridge);
        void Abort();

        bool CanBePruned(string address);
    }
}