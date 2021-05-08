using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace UsbDetector
{
    class DeviceWatchdog
    {

        private readonly Action Callback;

        public DeviceWatchdog(Action callback)
        {
            WqlEventQuery insertQuery = new("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2");

            ManagementEventWatcher insertWatcher = new(insertQuery);
            insertWatcher.EventArrived += new(OnDeviceEvent);
            insertWatcher.Start();

            // we dont actually care about remove events...
            /*WqlEventQuery removeQuery = new("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 1");
            ManagementEventWatcher removeWatcher = new ManagementEventWatcher(removeQuery);
            removeWatcher.EventArrived += new(OnDeviceEvent);
            removeWatcher.Start();*/

            this.Callback = callback;
        }


        private void OnDeviceEvent(object sender, EventArrivedEventArgs e)
        {
            this.Callback();
        }
    }
}
