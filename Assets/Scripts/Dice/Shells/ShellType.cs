using System;

namespace GoDice.Dice.Shells
{
    [Flags]
    internal enum ShellType
    {
        None = 0,
        D4 = 1,
        D6 = 2,
        D8 = 4,
        D10X = 8,
        D10 = 16,
        D12 = 32,
        D20 = 64,
        Any = D4 | D6 | D8 | D10 | D10X | D12 | D20
    }
}