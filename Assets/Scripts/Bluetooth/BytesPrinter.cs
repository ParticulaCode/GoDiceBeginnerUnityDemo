using System.Collections.Generic;
using System.Text;

namespace GoDice.Bluetooth
{
    internal static class BytesPrinter
    {
        public static string Print(IReadOnlyList<byte> bytes)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < bytes.Count; ++i)
            {
                if (i > 0)
                    builder.Append(" ");
                if (bytes[i] < 10)
                {
                    builder.Append("0");
                    builder.Append(bytes[i]);
                }
                else
                {
                    builder.Append(bytes[i]);
                }
            }

            return builder.ToString();
        }
    }
}