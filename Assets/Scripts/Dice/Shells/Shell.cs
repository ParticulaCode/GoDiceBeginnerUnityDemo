using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GoDice.App.Modules.Dice.Shells
{
    public class Shell : IShellTransformer
    {
        private readonly Dictionary<int, Vector3> _values;

        public Shell(Dictionary<int, Vector3> values) =>
            _values = values.ToDictionary(pair => pair.Key, pair => pair.Value.normalized);

        //Old firmware provide axes in [-64; 64] range 
        //New firmware provide axes in [-32; 32] range
        //We are using normalized vectors to handle both
        public int AxesToValue(Vector3 axes)
        {
            var value = 0;
            var distance = float.MaxValue;
            var normAxes = axes.normalized;
            foreach (var pair in _values)
            {
                var lDist = Vector3.Distance(normAxes, pair.Value);
                if (lDist >= distance)
                    continue;

                value = pair.Key;
                distance = lDist;
            }

            return value;
        }
    }
}