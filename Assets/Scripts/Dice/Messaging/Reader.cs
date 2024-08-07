using System.Collections.Generic;
using System.Linq;
using GoDice.Dice;
using UnityEngine;
using static System.Text.Encoding;

namespace GoDice.App.Modules.Dice.Messaging
{
    internal class Reader
    {
        public float Battery { get; private set; }
        public Vector3Int Axes { get; private set; }
        public ColorType Color { get; private set; }

        private const float BatteryScale = 100f;
        private static readonly int ColorsCount =
            typeof(ColorType).GetEnumValues().Cast<ColorType>().Count();

        private static readonly Dictionary<string, Response> StableVariants = new()
        {
            { ReadProtocol.Stable, Response.RollEnd },
            { ReadProtocol.FakeStable, Response.FakeStable },
            { ReadProtocol.TiltStable, Response.TiltStable },
            { ReadProtocol.MoveStable, Response.MoveStable }
        };

        public Response Read(byte[] data)
        {
            var str = UTF8.GetString(data);

            switch (str)
            {
                case ReadProtocol.Roll:
                    return Response.Roll;
            }

            if (str.StartsWith(ReadProtocol.Battery))
            {
                Battery = data[3] / BatteryScale;
                return Response.Battery;
            }

            if (str.StartsWith(ReadProtocol.Charging))
            {
                var valuePos = ReadProtocol.Charging.Length;
                return data[valuePos] == 0 ? Response.ChargingStopped : Response.ChargingStarted;
            }

            foreach (var variant in StableVariants.Where(variant => str.StartsWith(variant.Key)))
            {
                ToAxes(data, variant.Key.Length);
                return variant.Value;
            }

            if (str.StartsWith(ReadProtocol.Color))
            {
                var colorNum = data[3] + 1;
                Color = colorNum < ColorsCount ? (ColorType) colorNum : ColorType.None;
                return Response.Color;
            }

            Debug.LogError($"Ignored message in demo: {string.Join(", ", data)}");

            return Response.Undefined;
        }

        private void ToAxes(IReadOnlyList<byte> data, int pos) => Axes =
            new Vector3Int((sbyte) data[pos], (sbyte) data[pos + 1], (sbyte) data[pos + 2]);
    }
}