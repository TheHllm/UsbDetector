using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsbDetector
{
    class DeviceInsertionWatchdog
    {
        private readonly DeviceWatchdog watchdog;
        private IEnumerable<string> devices;
        private readonly Action<string> callback;
        public DeviceInsertionWatchdog(Action<string> callback)
        {
            this.watchdog = new DeviceWatchdog(this.DeviceCallback);
            this.devices = DeviceList.GetDevices();
            this.callback = callback;
        }

        private void DeviceCallback()
        {
            string newDev = DeviceList.GetNewDevice(this.devices);
            if (newDev != null)
            {
                this.devices = DeviceList.GetDevices();
                this.callback(newDev);
            }
        }
    }
}
