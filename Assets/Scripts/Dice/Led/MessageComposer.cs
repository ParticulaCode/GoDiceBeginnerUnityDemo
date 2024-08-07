using System;
using GoDice.App.Modules.Dice.Messaging;
using UnityEngine;

namespace GoDice.Dice.Led
{
    internal static class MessageComposer
    {
        //Firmware will multiple timing by 10 and operates in millisecond
        private const float SecondsScale = 1000f / 10f;

        //Seconds
        private const float BlinkDelay = 0.5f;

        public static readonly byte[] Discovery =
            ComposeToggle(
                3,
                Color.green,
                BlinkDelay,
                BlinkDelay,
                true);

        public static byte[] ConstantOneLed(Color color) =>
            ComposeActivation(Color.black, color);

        public static byte[] ConstantTwoLed(Color color) =>
            ComposeActivation(color, color);

        public static byte[] ComposeToggle(byte blinkNumber, Color color, float onDuration,
            float offDuration, bool mixed, byte ledsToActivate = WriteProtocol.Led.BothLeds)
        {
            var scaledColor = (Color32) color;
            return new[]
            {
                blinkNumber,
                AdaptTiming(onDuration),
                AdaptTiming(offDuration),
                scaledColor.r,
                scaledColor.g,
                scaledColor.b,
                Convert.ToByte(mixed),
                ledsToActivate
            };
        }

        private static byte AdaptTiming(float t) => (byte) Mathf.Floor(t * SecondsScale);

        /// RGB for Led1 and RGB for Led2
        private static byte[] ComposeActivation(Color32 color1, Color32 color2) =>
            new[]
            {
                WriteProtocol.Led.Constant,
                color1.r,
                color1.g,
                color1.b,
                color2.r,
                color2.g,
                color2.b
            };
    }
}