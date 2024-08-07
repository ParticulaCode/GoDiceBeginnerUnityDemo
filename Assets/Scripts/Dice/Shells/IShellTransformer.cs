using UnityEngine;

namespace GoDice.App.Modules.Dice.Shells
{
    public interface IShellTransformer
    {
        int AxesToValue(Vector3 axes);
    }
}