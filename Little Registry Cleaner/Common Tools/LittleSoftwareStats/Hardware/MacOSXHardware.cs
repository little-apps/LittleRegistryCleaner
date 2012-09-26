using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LittleSoftwareStats.Hardware
{
    internal class MacOSXHardware : UnixHardware
    {
        public MacOSXHardware()
        {
        }

        public override string CPUName 
        {
            get { 
                try
                {
                    Regex regex = new Regex(@"Processor Name\s*:\\s*(?<processor>[\w\s\d\.]+)");
                    MatchCollection matches = regex.Matches(Utils.SystemProfilerCommandOutput);
                    return matches[0].Groups["processor"].Value;
                }
                catch { }

                return "Generic"; 
            }
        }

        public override int CPUArchitecture
        {
            get
            {
                Regex regex = new Regex(@"hw\.cpu64bit_capable\s*(:|=)\s*(?<capable>\d+)");
                MatchCollection matches = regex.Matches(Utils.SysctlCommandOutput);
                if (matches[0].Groups["cpus"].Value == "1")
                    return 64;
                return 32;
            }
        }

        public override int CPUCores
        {
            get 
            {
                Regex regex = new Regex(@"hw\.availcpu\s*(:|=)\s*(?<cpus>\d+)");
                MatchCollection matches = regex.Matches(Utils.SysctlCommandOutput);
                return int.Parse(matches[0].Groups["cpus"].Value);
            }
        }

        public override string CPUBrand 
        {
            get { return "GenuineIntel"; }
        }

        public override double CPUFrequency 
        {
            get
            {
                Regex regex = new Regex(@"hw\.cpufrequency\s*(:|=)\s*(?<cpu_frequency>\d+)");
                MatchCollection matches = regex.Matches(Utils.SysctlCommandOutput);

                // Convert from B -> MB
                return double.Parse(matches[0].Groups["cpu_frequency"].Value) / 1024 / 1024;
            }
        }

        public override double MemoryTotal
        {
            get 
            {
                Regex regex = new Regex(@"hw\.memsize\s*(:|=)\s*(?<memory>\d+)");
                MatchCollection matches = regex.Matches(Utils.SysctlCommandOutput);

                // Convert from B -> MB
                return double.Parse(matches[0].Groups["memory"].Value) / 1024 / 1024;
            }
        }
    }
}
