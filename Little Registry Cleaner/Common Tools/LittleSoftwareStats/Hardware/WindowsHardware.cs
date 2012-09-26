using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Management;

namespace LittleSoftwareStats.Hardware
{
    internal class WindowsHardware : Hardware
    {
#region P/Invoke signatures
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;

            public MEMORYSTATUSEX()
            {
                this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
        }


        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);
#endregion
        readonly string _cpuBrand = "";
        public override string CPUBrand
        {
            get { return this._cpuBrand; }
        }

        readonly string _cpuName = "";
        public override string CPUName
        {
            get { return this._cpuName; }
        }

        readonly int _cpuArch;
        public override int CPUArchitecture
        {
            get { return this._cpuArch; }
        }
        
        public override int CPUCores
        {
            get { return Environment.ProcessorCount; }
        }

        public override double CPUFrequency
        {
            get { return Convert.ToDouble(Utils.GetRegistryValue(Registry.LocalMachine, @"HARDWARE\DESCRIPTION\System\CentralProcessor\0", "~MHz", 0)); }
        }

        readonly long _diskFree = 0;
        public override long DiskFree
        {
            get { return this._diskFree; }
        }

        readonly long _diskTotal = 0;
        public override long DiskTotal
        {
            get { return this._diskTotal; }
        }

        readonly double _memoryFree = 0;
        public override double MemoryFree
        {
            get { return this._memoryFree; }
        }

        readonly double _memoryTotal = 0;
        public override double MemoryTotal
        {
            get { return this._memoryTotal; }
        }

        public override string ScreenResolution
        {
            get { return Screen.PrimaryScreen.Bounds.Width + "x" + Screen.PrimaryScreen.Bounds.Height; }
        }

        public WindowsHardware()
        {
            // Get memory
            MEMORYSTATUSEX memStatus = new MEMORYSTATUSEX();
            if (GlobalMemoryStatusEx(memStatus))
            {
                // Convert from bytes -> megabytes
                this._memoryTotal = memStatus.ullTotalPhys / 1024 / 1024;
                this._memoryFree = memStatus.ullAvailPhys / 1024 / 1024;
            }

            // Get disk space
            foreach (DriveInfo di in DriveInfo.GetDrives())
            {
                if (di.IsReady && di.DriveType == DriveType.Fixed)
                {
                    this._diskFree += di.TotalFreeSpace / 1024 / 1024;
                    this._diskTotal += di.TotalSize / 1024 / 1024;
                }
            }

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Name, Manufacturer, Architecture FROM Win32_Processor");

            foreach (ManagementObject sysItem in searcher.Get())
            {
                try
                {
                    this._cpuName = sysItem["Name"].ToString();

                    if (!string.IsNullOrEmpty(this._cpuName))
                    {
                        this._cpuName = this._cpuName.Replace("(TM)", "");
                        this._cpuName = this._cpuName.Replace("(R)", "");
                        this._cpuName = this._cpuName.Replace(" ", "");
                    }
                }
                catch
                {
                    this._cpuName = "Unknown";
                }

                try
                {
                    this._cpuBrand = sysItem["Manufacturer"].ToString();
                }
                catch
                {
                    this._cpuBrand = "Unknown";
                }

                try
                {
                    int arch = Convert.ToInt32(sysItem["Architecture"].ToString());
                    if (arch == 6 || arch == 9)
                        this._cpuArch = 64;
                    else
                        this._cpuArch = 32;
                }
                catch
                {
                    this._cpuArch = 32;
                }
            }
        }
    } 
}
