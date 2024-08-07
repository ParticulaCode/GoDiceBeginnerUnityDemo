using System.Collections.Generic;
using System.Linq;
using FrostLib;

namespace GoDice.Dice.Commands
{
    internal class GetColorByDieNameCommand : ICommand<ColorType>
    {
        private readonly Dictionary<string, ColorType> _colorMap = new()
        {
            { "K", ColorType.Black },
            { "R", ColorType.Red },
            { "G", ColorType.Green },
            { "B", ColorType.Blue },
            { "Y", ColorType.Yellow },
            { "O", ColorType.Orange }
        };

        private readonly string _name;

        public GetColorByDieNameCommand(string name) => _name = name;

        public ColorType Execute()
        {
            var colorChar = _name.Last().ToString();
            return _colorMap.GetValueOrDefault(colorChar, ColorType.None);
        }
    }
}