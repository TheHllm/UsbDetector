using KeePassXC_API;
using NeoSmart.AsyncLock;
using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CoreDetector
{
    class Program
    {
        static IEnumerable<string> drives = Bitlocker.GetBitlockerDrives();
        static KeepassXCApi API
        {
            get
            {
                if (_api == null)
                {
                    _api = new KeepassXCApi();
                }
                return _api;
            }
        }
        static KeepassXCApi _api = null;
        static AsyncLock apiLock = new();

        static async Task Main(string[] args)
        {
            TakeAction();
        }


        private static void TakeAction()
        {
            //if one is unlocked unlock all others
            bool oneDriveIsLocked = false;
            List<Task> tasks = new();
            foreach (string drive in drives)
            {
                if (DriveLocked(drive))
                {
                    oneDriveIsLocked = true;
                    //unlock this drive
                    tasks.Add(UnlockDrive(drive));
                }
            }


            if (oneDriveIsLocked)
            {
                foreach (Task task in tasks)
                {
                    task.Wait();
                }
                return; // if there is one locked drive, wich we unlocked we dont want to lock any drive.
            }


            // Otherwise: lock all drives
            Task t = Bitlocker.LockAllDrives();
            API.LockDatabase().Wait();
            t.Wait();
        }

        public static bool DriveLocked(string path)
        {
            try
            {
                Directory.EnumerateFiles(path);
                return false;
            }
            catch
            {
                return true;
            }
        }

        public static async Task UnlockDrive(string path)
        {
            KeepassXCApi api;
            using (IDisposable l = await apiLock.LockAsync())
            {
                api = API;
            }
            string pw = (await api.GetLogins("https://" + path))[0].Password;
            await Bitlocker.UnlockDriveWithPassword(path, pw);
        }
    }
}
