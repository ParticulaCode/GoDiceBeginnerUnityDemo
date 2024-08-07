namespace GoDice.Bluetooth.Operations
{
    public enum OperationType
    {
        Connection,
        LedToggle,
        LedConstant,
        LedOff,
        BatteryRequest,
        ColorRequest,
        Initialization,
        Reset,
        LastStableValueRequest
    }
}