using System.Collections.Generic;
using System.Linq;

namespace GoDice.Bluetooth.Devices
{
    internal class DevicesHolder
    {
        private readonly List<Device> _devices = new();

        public void Add(Device device)
        {
            if (_devices.Contains(device))
                return;

            _devices.Add(device);
        }

        public bool Exists(string address) => _devices.Any(d => d.Address == address);

        public Device Get(string address) => _devices.FirstOrDefault(d => d.Address == address);

        public IEnumerable<Device> GetDevices() => _devices;

        public void Clear() => _devices.Clear();
    }
}