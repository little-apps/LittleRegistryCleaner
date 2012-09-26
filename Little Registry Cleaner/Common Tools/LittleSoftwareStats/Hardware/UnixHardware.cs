using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LittleSoftwareStats.Hardware
{
    internal class UnixHardware : Hardware
    {
        public UnixHardware()
        {
        }

        public override string CPUName
        {
            get
            {
                try
                {
                    string output = Utils.GetCommandExecutionOutput("cat", "/proc/cpuinfo");
                    Regex regex = new Regex(@"(?:model name\s+:\s*)(?<ModelName>[\w \(\)@\.]*)");
                    MatchCollection matches = regex.Matches(output);
                    return matches[0].Groups["ModelName"].Value;
                }
                catch { }

                return "Unknown";
            }
        }

        public override string CPUBrand
        {
            get
            {
                try
                {
                    string output = Utils.GetCommandExecutionOutput("cat", "/proc/cpuinfo");
                    Regex regex = new Regex(@"(?:vendor_id\s+:\s*)(?<vendor>\w*)");
                    MatchCollection matches = regex.Matches(output);
                    return matches[0].Groups[1].Value;
                }
                catch { }

                return "Unknown";
            }
        }

        public override double CPUFrequency
        {
            get
            {
                try
                {
                    string output = Utils.GetCommandExecutionOutput("cat", "/proc/cpuinfo");
                    Regex regex = new Regex(@"(?:bogomips\s+:\s*)(?<bogomips>\w*)");
                    MatchCollection matches = regex.Matches(output);
                    int bogomips = int.Parse(matches[0].Groups[1].Value);
                    return bogomips / CPUCores;
                }
                catch { }

                return 0;
            }
        }

        public override int CPUArchitecture
        {
            get
            {
                try
                {
                    string output = Utils.GetCommandExecutionOutput("cat", "/proc/cpuinfo");
                    Regex regex = new Regex(@"flags\s+\s:[\w\s]*");
                    MatchCollection matches = regex.Matches(output);
                    string flags = matches[0].Groups[0].Value;
                    if (flags.Contains(" lm"))
                        return 64;
                }
                catch { }

                return 32;
            }
        }

        public override int CPUCores
        {
            get
            {
                try
                {
                    string output = Utils.GetCommandExecutionOutput("cat", "/proc/cpuinfo");
                    Regex regex = new Regex(@"(?:cpu cores\s+:\s*)(?<num>\w*)");
                    MatchCollection matches = regex.Matches(output);
                    return Int32.Parse(matches[0].Groups[1].Value);
                }
                catch { }

                // There has to be at least 1 core, cause how would we be able reach this ???
                return 1;
            }
        }

        public override double MemoryTotal
        {
            get
            {
                try
                {
                    string output = Utils.GetCommandExecutionOutput("cat", "/proc/meminfo");
                    Regex regex = new Regex(@"(?:MemTotal:\s*)(?<memtotal>\d+)");
                    MatchCollection matches = regex.Matches(output);

                    // Convert from KB -> MB
                    return double.Parse(matches[0].Groups[1].Value) / 1024;
                }
                catch { }

                return 0;
            }
        }

        public override double MemoryFree
        {
            get
            {
                try
                {
                    string output = Utils.GetCommandExecutionOutput("cat", "/proc/meminfo");
                    Regex regex = new Regex(@"(?:MemFree:\s*)(?<memtotal>\d+)");
                    MatchCollection matches = regex.Matches(output);

                    // Convert from KB -> MB
                    return double.Parse(matches[0].Groups[1].Value) / 1024;
                }
                catch { }
                return 0;
            }
        }

        public override long DiskTotal
        {
            get
            {
                try
                {
                    string output = Utils.GetCommandExecutionOutput("df", "-k");
                    Regex regex = new Regex(@"^/[\w/]*\s*(?<total>\d+)\s*(?<used>\d+)\s*(?<available>\d+)");
                    MatchCollection matches = regex.Matches(output);

                    long total = 0;
                    foreach (Match match in matches)
                        total += long.Parse(match.Groups["total"].Value);

                    // Convert from KB -> MB
                    return total / 1024;
                }
                catch { }

                return -1;
            }
        }

        public override long DiskFree
        {
            get
            {
                try
                {
                    string output = Utils.GetCommandExecutionOutput("df", "-B 1k");
                    Regex regex = new Regex(@"^/[\w/]*\s*(?<total>\d+)\s*(?<used>\d+)\s*(?<available>\d+)");
                    MatchCollection matches = regex.Matches(output);

                    long total = 0;
                    foreach (Match match in matches)
                        total += long.Parse(match.Groups["available"].Value);

                    // Convert from KB -> MB
                    return total / 1024;
                }
                catch { }

                return 0;
            }
        }

        public override string ScreenResolution
        {
            get
            {
                try
                {
                    int deskHeight = Screen.PrimaryScreen.Bounds.Height;
                    int deskWidth = Screen.PrimaryScreen.Bounds.Width;
                    return deskWidth + "x" + deskHeight;
                }
                catch { }

                return "800x600";
            }
        } 
    }
}
