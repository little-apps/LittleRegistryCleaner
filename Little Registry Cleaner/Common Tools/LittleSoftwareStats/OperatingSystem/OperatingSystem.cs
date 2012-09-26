using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LittleSoftwareStats.OperatingSystem
{
    internal abstract class OperatingSystem
    {
        abstract public Version FrameworkVersion { get; }
        abstract public int FrameworkSP { get; }
        abstract public Version JavaVersion { get; }

        abstract public int Architecture { get; }
        abstract public string Version { get; }
        abstract public int ServicePack { get; }

        abstract public Hardware.Hardware Hardware { get; }

        public int Lcid
        {
            get
            {
                try
                {
                    return Thread.CurrentThread.CurrentCulture.LCID;
                }
                catch
                {
                    // Just return 1033 (English - USA)
                    return 1033;
                }
            }
        }

        public static OperatingSystem GetOperatingSystemInfo()
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
                return new UnixOperatingSystem();
            else if (Environment.OSVersion.Platform == PlatformID.MacOSX)
                return new MacOSXOperatingSystem();
            else
                return new WindowsOperatingSystem();
        }
    }
}
