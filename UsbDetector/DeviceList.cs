using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace UsbDetector
{
    public class DeviceList
    {
        public static IEnumerable<string> GetDevices()
        {
            ManagementObjectSearcher searcher = new("SELECT * FROM Win32_PnPDevice");
            var results = searcher.Get();
            var output = new string[results.Count];
            int i = 0;

            foreach (ManagementObject obj in results)
            {
                output[i++] = (string)obj.Properties["SameElement"].Value;
            }

            return output;
        }

        public static string GetNewDevice(IEnumerable<string> previusDevices)
        {
            return GetNewDevice(previusDevices, DeviceList.GetDevices());
        }

        public static string GetNewDevice(IEnumerable<string> previusDevices, IEnumerable<string> currentDevices)
        {
            if(previusDevices is HashSet<string>)
            {
                return GetNewDevice((HashSet<string>)previusDevices, currentDevices);
            }

            HashSet<string> set = new HashSet<string>();
            foreach(string dev in previusDevices)
            {
                set.Add(dev);
            }

            return GetNewDevice(set, currentDevices);
        }
        public static string GetNewDevice(HashSet<string> previusDevices, IEnumerable<string> currentDevices)
        {
            foreach(string curDev in currentDevices)
            {
                if (!previusDevices.Contains(curDev))
                {
                    return curDev;
                }
            }
            return null;
        }
    }
}
