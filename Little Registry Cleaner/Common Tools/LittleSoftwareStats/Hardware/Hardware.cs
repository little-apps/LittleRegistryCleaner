using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LittleSoftwareStats.Hardware
{
    internal abstract class Hardware
    {
        public abstract string CPUName { get; }
        public abstract int CPUArchitecture { get; }
        public abstract int CPUCores { get; }
        public abstract string CPUBrand { get; }
        public abstract double CPUFrequency { get; }

        public abstract double MemoryTotal { get; }
        public abstract double MemoryFree { get; }

        public abstract long DiskTotal { get; }
        public abstract long DiskFree { get; }

        public abstract string ScreenResolution { get; }
    }
}
