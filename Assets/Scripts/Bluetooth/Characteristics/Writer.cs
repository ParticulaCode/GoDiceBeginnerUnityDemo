using GoDice.Bluetooth.Operations;

namespace GoDice.Bluetooth.Characteristics
{
    internal class Writer
    {
        private readonly OperationsRunner _operationsRunner;
        private readonly Characteristic _characteristic;

        public Writer(Characteristic characteristic, OperationsRunner operationsRunner)
        {
            _characteristic = characteristic;
            _operationsRunner = operationsRunner;
        }

        public void Send(byte[] data, OperationType type) =>
            _operationsRunner.Schedule(new WriteOperation(_characteristic, data, type));
    }
}