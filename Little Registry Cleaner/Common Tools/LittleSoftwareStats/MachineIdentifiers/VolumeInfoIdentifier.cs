using System;
using System.Text;
using System.Runtime.InteropServices;

namespace LittleSoftwareStats.MachineIdentifiers
{
    public class VolumeInfoIdentifier : MachineIdentifierBase, IMachineIdentifier
    {
        [DllImport("kernel32.dll")]
        private static extern long GetVolumeInformation(string PathName, StringBuilder VolumeNameBuffer, UInt32 VolumeNameSize, ref UInt32 VolumeSerialNumber, ref UInt32 MaximumComponentLength, ref UInt32 FileSystemFlags, StringBuilder FileSystemNameBuffer, UInt32 FileSystemNameSize);

        protected override byte[] GetIdentifierHash()
        {
            string identifier = "NOTFOUND";

            if (Environment.OSVersion.Platform != PlatformID.MacOSX && Environment.OSVersion.Platform != PlatformID.Unix)
            {
                try
                {
                    uint serNum = 0;
                    uint maxCompLen = 0;
                    StringBuilder VolLabel = new StringBuilder(256);
                    UInt32 VolFlags = new UInt32();
                    StringBuilder FSName = new StringBuilder(256);
                    GetVolumeInformation(null, VolLabel, (UInt32)VolLabel.Capacity, ref serNum, ref maxCompLen, ref VolFlags, FSName, (UInt32)FSName.Capacity);
                    identifier = serNum.ToString();
                }
                catch { }
            }
            
            return base.ComputeHash(identifier);
        }
    }
}
