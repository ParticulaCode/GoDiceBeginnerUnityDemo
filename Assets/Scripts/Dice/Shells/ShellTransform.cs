using System.Collections.Generic;
using UnityEngine;

namespace GoDice.App.Modules.Dice.Shells
{
    internal class ShellTransform : IShellTransformer
    {
        private readonly Shell _shell;
        private readonly Dictionary<int, int> _transformations;

        public ShellTransform(Shell shell, Dictionary<int, int> transformations)
        {
            _shell = shell;
            _transformations = transformations;
        }

        public int AxesToValue(Vector3 axes) => _transformations[_shell.AxesToValue(axes)];
    }
}