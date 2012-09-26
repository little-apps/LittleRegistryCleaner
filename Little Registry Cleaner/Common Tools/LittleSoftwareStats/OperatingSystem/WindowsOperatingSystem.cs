using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace LittleSoftwareStats.OperatingSystem
{
    internal class WindowsOperatingSystem : OperatingSystem
    {
#region P/Invoke Signatures
        public const byte VER_NT_WORKSTATION = 1;

        public const ushort VER_SUITE_WH_SERVER = 32768;

        public const ushort PROCESSOR_ARCHITECTURE_INTEL = 0;
        public const ushort PROCESSOR_ARCHITECTURE_IA64 = 6;
        public const ushort PROCESSOR_ARCHITECTURE_AMD64 = 9;

        public const int SM_SERVERR2 = 89;

        [StructLayout(LayoutKind.Sequential)]
        public struct OSVERSIONINFOEX
        {
            public uint dwOSVersionInfoSize;
            public uint dwMajorVersion;
            public uint dwMinorVersion;
            public uint dwBuildNumber;
            public uint dwPlatformId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szCSDVersion;
            public ushort wServicePackMajor;
            public ushort wServicePackMinor;
            public ushort wSuiteMask;
            public byte wProductType;
            public byte wReserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_INFO
        {
            public uint wProcessorArchitecture;
            public uint wReserved;
            public uint dwPageSize;
            public uint lpMinimumApplicationAddress;
            public uint lpMaximumApplicationAddress;
            public uint dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public uint dwProcessorLevel;
            public uint dwProcessorRevision;
        }

        [DllImport("kernel32.dll")]
        internal static extern bool GetVersionEx(ref OSVERSIONINFOEX osVersionInfo);

        [DllImport("kernel32.dll")]
        internal static extern void GetSystemInfo(ref SYSTEM_INFO pSI);

        [DllImport("user32.dll")]
        internal static extern int GetSystemMetrics(int nIndex);
#endregion

        Hardware.Hardware _hardware;
        public override Hardware.Hardware Hardware
        {
            get
            {
                if (_hardware == null)
                    _hardware = new Hardware.WindowsHardware();
                return _hardware;
            }
        }
        
        readonly Version _frameworkVersion;
        public override Version FrameworkVersion
        {
            get { return this._frameworkVersion; }
        }

        readonly int _frameworkSP;
        public override int FrameworkSP
        {
            get { return this._frameworkSP; }
        }

        readonly Version _javaVersion;
        public override Version JavaVersion
        {
            get { return this._javaVersion; }
        }

        public override int Architecture
        {
            get
            {
                string arch = (string)Utils.GetRegistryValue(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment", "PROCESSOR_ARCHITECTURE");

                switch (arch.ToLower())
                {
                    case "x86":
                        return 32;
                    case "amd64":
                    case "ia64":
                        return 64;
                    default:
                        break;
                }

                // Just use IntPtr size
                // (note: will always return 32 bit if process is not 64 bit)
                return (IntPtr.Size == 8) ? (64) : (32);
            }
        }

        private string _version;
        public override string Version
        {
            get { return this._version; }
        }

        private int _servicePack;
        public override int ServicePack
        {
            get { return this._servicePack; }
        }

        public WindowsOperatingSystem()
        {
            // Get OS Info
            GetOSInfo();

            // Get .NET Framework version + SP
            this._frameworkVersion = new Version(); // 0.0
            this._frameworkSP = 0;

            try
            {
                RegistryKey regNet = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\NET Framework Setup\NDP");

                if (regNet != null)
                {
                    if (regNet.OpenSubKey("v4") != null)
                    {
                        this._frameworkVersion = new Version(4, 0);
                    }
                    else if (regNet.OpenSubKey("v3.5") != null)
                    {
                        this._frameworkVersion = new Version(3, 5);
                        this._frameworkSP = (int)regNet.GetValue("SP", 0);
                    }
                    else if (regNet.OpenSubKey("v3.0") != null)
                    {
                        this._frameworkVersion = new Version(3, 0);
                        this._frameworkSP = (int)regNet.GetValue("SP", 0);
                    }
                    else if (regNet.OpenSubKey("v2.0.50727") != null)
                    {
                        this._frameworkVersion = new Version(2, 0, 50727);
                        this._frameworkSP = (int)regNet.GetValue("SP", 0);
                    }
                    else if (regNet.OpenSubKey("v1.1.4322") != null)
                    {
                        this._frameworkVersion = new Version(1, 1, 4322);
                        this._frameworkSP = (int)regNet.GetValue("SP", 0);
                    }
                    else if (regNet.OpenSubKey("v1.0") != null)
                    {
                        this._frameworkVersion = new Version(1, 0);
                        this._frameworkSP = (int)regNet.GetValue("SP", 0);
                    }

                    regNet.Close();
                }
            }
            catch { }
            
            // Get Java version
            this._javaVersion = new Version();

            try
            {
                string javaVersion;

                if (this.Architecture == 32)
                    javaVersion = (string)Utils.GetRegistryValue(Registry.LocalMachine, @"Software\JavaSoft\Java Runtime Environment", "CurrentVersion", "");
                else
                    javaVersion = (string)Utils.GetRegistryValue(Registry.LocalMachine, @"Software\Wow6432Node\JavaSoft\Java Runtime Environment", "CurrentVersion", "");

                this._javaVersion = new Version(javaVersion);
            }
            catch { }
        }

        private void GetOSInfo()
        {
            OSVERSIONINFOEX osVersionInfo = new OSVERSIONINFOEX();
            osVersionInfo.dwOSVersionInfoSize = (uint)Marshal.SizeOf(typeof(OSVERSIONINFOEX));

            if (!GetVersionEx(ref osVersionInfo))
            {
                this._version = "Unknown";
                this._servicePack = 0;
                return;
            }

            string osName = "";

            SYSTEM_INFO systemInfo = new SYSTEM_INFO();
            GetSystemInfo(ref systemInfo);

            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32Windows:
                    {
                        switch (osVersionInfo.dwMajorVersion)
                        {
                            case 4:
                                {
                                    switch (osVersionInfo.dwMinorVersion)
                                    {
                                        case 0:
                                            if (osVersionInfo.szCSDVersion == "B" ||
                                                osVersionInfo.szCSDVersion == "C")
                                                osName += "Windows 95 R2";
                                            else
                                                osName += "Windows 95";
                                            break;
                                        case 10:
                                            if (osVersionInfo.szCSDVersion == "A")
                                                osName += "Windows 98 SE";
                                            else
                                                osName += "Windows 98";
                                            break;
                                        case 90:
                                            osName += "Windows ME";
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                    break;

                case PlatformID.Win32NT:
                    {
                        switch (osVersionInfo.dwMajorVersion)
                        {
                            case 3:
                                osName += "Windows NT 3.5.1";
                                break;

                            case 4:
                                osName += "Windows NT 4.0";
                                break;

                            case 5:
                                {
                                    switch (osVersionInfo.dwMinorVersion)
                                    {
                                        case 0:
                                            osName += "Windows 2000";
                                            break;
                                        case 1:
                                            osName += "Windows XP";
                                            break;
                                        case 2:
                                            {
                                                if (osVersionInfo.wSuiteMask == VER_SUITE_WH_SERVER)
                                                    osName += "Windows Home Server";
                                                else if (osVersionInfo.wProductType == VER_NT_WORKSTATION && systemInfo.wProcessorArchitecture == PROCESSOR_ARCHITECTURE_AMD64)
                                                    osName += "Windows XP";
                                                else
                                                    osName += GetSystemMetrics(SM_SERVERR2) == 0 ? "Windows Server 2003" : "Windows Server 2003 R2";
                                            }
                                            break;
                                    }

                                }
                                break;

                            case 6:
                                {
                                    switch (osVersionInfo.dwMinorVersion)
                                    {
                                        case 0:
                                            osName += osVersionInfo.wProductType == VER_NT_WORKSTATION ? "Windows Vista" : "Windows Server 2008";
                                            break;

                                        case 1:
                                            osName += osVersionInfo.wProductType == VER_NT_WORKSTATION ? "Windows 7" : "Windows Server 2008 R2";
                                            break;
                                        case 2:
                                            osName += osVersionInfo.wProductType == VER_NT_WORKSTATION ? "Windows 8" : "Windows Server 8";
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }

            this._version = osName;
            this._servicePack = osVersionInfo.wServicePackMajor;

            return;
        }
    } 
}
