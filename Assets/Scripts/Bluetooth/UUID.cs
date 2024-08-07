namespace GoDice.Bluetooth
{
    // ReSharper disable once InconsistentNaming
    public static class UUID
    {
        public static readonly string Service = Combine("0001");
        public static readonly string ReadCharacteristic = Combine("0003");
        public static readonly string WriteCharacteristic = Combine("0002");

        private static string Combine(string uuid) => "6E40" + uuid + "-B5A3-F393-E0A9-E50E24DCCA9E";
    }
}