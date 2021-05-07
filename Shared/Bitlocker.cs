using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Shared
{
	public class Bitlocker
	{
		public static void LockDrive(string path)
		{
			var startInfo = new ProcessStartInfo();
			startInfo.WorkingDirectory = @"C:\Windows\System32";
			startInfo.FileName = @"C:\Windows\System32\cmd.exe";
			startInfo.Arguments = "/C manage-bde -lock -ForceDismount " + path;
			startInfo.Verb = "runasuser";
			Process.Start(startInfo);
		}

		public static async Task UnlockDriveWithPassword(string path, string password)
		{
			var startInfo = new ProcessStartInfo();
			startInfo.WorkingDirectory = @"C:\Windows\System32";
			startInfo.UseShellExecute = false;
			startInfo.FileName = Path.Combine(Environment.SystemDirectory, "manage-bde.exe");
			startInfo.Arguments = "-unlock " + path + " -recoverypassword " + password;
			startInfo.Verb = "runasuser";
			var proc = Process.Start(startInfo);

			await proc.WaitForExitAsync();
		}

		public static async Task<IEnumerable<string>> GetBitlockerDrivesAsync()
		{
			List<string> result = new();

			var startInfo = new ProcessStartInfo();
			startInfo.WorkingDirectory = @"C:\Windows\System32";
			startInfo.UseShellExecute = false;
			startInfo.FileName = Path.Combine(Environment.SystemDirectory, "manage-bde.exe");
			startInfo.Arguments = "-status";
			startInfo.Verb = "runasuser";
			startInfo.RedirectStandardOutput = true;
			var proc = Process.Start(startInfo);

			await proc.WaitForExitAsync();
			string output = await proc.StandardOutput.ReadToEndAsync();

			/* Example output:
             * 
                Volume C: []
                [OS Volume]

                Size:                 1234,5 GB
                BitLocker Version:    None
                Conversion Status:    Fully Decrypted
                Percentage Encrypted: 0,0%
                Encryption Method:    None
                Protection Status:    Protection Off
                Lock Status:          Unlocked
                Identification Field: None
                Key Protectors:       None Found
             */

			// parse the output
			string driveLetter = "";
			foreach (string line in output.Split('\n'))
			{
				if (line.StartsWith("Volume"))
				{
					driveLetter = line.Substring("Volume ".Length, 2);
				}
				else if (line == "    Key Protectors:\r")
				{
					result.Add(driveLetter);
				}
			}

			return result;
		}

		public static IEnumerable<string> GetBitlockerDrives()
		{
			return GetBitlockerDrivesAsync().Result;
		}

		public static async Task LockAllDrives()
		{
			foreach (string drive in await GetBitlockerDrivesAsync())
			{
				Bitlocker.LockDrive(drive);
			}
		}
	}
}
