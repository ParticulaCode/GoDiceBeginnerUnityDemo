using GoDice.Dice.Shells;
using UnityEngine;

namespace GoDice.App.Modules.Dice.Shells
{
    internal class ShellController
    {
        private IShellTransformer _transformer;

        public ShellController(ShellType shellType) => SetShell(shellType);

        public int GetValue(Vector3 axis) => _transformer.AxesToValue(axis);

        public void SetShell(ShellType type) => _transformer = ShellHolder.GetShell(type);
    }
}