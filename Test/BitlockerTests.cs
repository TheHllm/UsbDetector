using NUnit.Framework;
using Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Test
{
    public class BitlockerTests
    {
        [Test]
        public void TestCanFindDrivesAsync()
        {
            IEnumerable<string> drives = Bitlocker.GetBitlockerDrives();
            Assert.IsNotEmpty(drives);
        }
    }
}