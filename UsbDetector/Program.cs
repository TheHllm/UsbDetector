using KeePassXC_API;
using Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace UsbDetector
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            DeviceInsertionWatchdog diw = new(Callback);
            Thread.Sleep(int.MaxValue);
        }

        public static void Callback(string dev)
        {
            Console.WriteLine("Locking...");
            Task t = Bitlocker.LockAllDrives();
            KeepassXCApi api = new();
            api.LockDatabase().Wait();
            t.Wait();
        }

    }
}
