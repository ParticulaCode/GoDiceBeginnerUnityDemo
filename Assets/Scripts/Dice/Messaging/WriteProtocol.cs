namespace GoDice.App.Modules.Dice.Messaging
{
    public static class WriteProtocol
    {
        private const byte Max = 0xFF;

        public const byte Initialization = 0x19;
        public const byte Battery = 0x03;
        public const byte LastStableValue = 0x09;
        public const byte Reset = 0x26;

        public static class Led
        {
            public const byte Toggle = 0x10;
            public const byte Constant = 0x08;
            public const byte Off = 0x14;
            public const byte Infinite = Max;
            public const byte BothLeds = 0x00;
            public const byte Led1 = 0x01;
            public const byte Led2 = 0x02;
        }

        public static class Color
        {
            public const byte Set = 0x27;
            public const byte Get = 0x17;
        }
    }
}