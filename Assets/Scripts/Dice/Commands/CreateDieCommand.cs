using FrostLib;
using GoDice.App.Modules.Dice.Messaging;
using GoDice.App.Modules.Dice.Shells;
using GoDice.Bluetooth.Devices;
using GoDice.Dice.Led;
using GoDice.Dice.Shells;
using UnityEngine;

namespace GoDice.Dice.Commands
{
    internal class CreateDieCommand : ICommand<Die>
    {
        private readonly Device _device;

        public CreateDieCommand(Device device) => _device = device;

        public Die Execute()
        {
            var color = new GetColorByDieNameCommand(_device.Name).Execute();
            Debug.Log($"Creating {color} die for {_device}");

            var writer = new Writer(_device);
            var die = new Die(
                color,
                _device,
                new LedController(writer),
                new ShellController(ShellType.D6),
                new Reader(),
                writer);

            return die;
        }
    }
}