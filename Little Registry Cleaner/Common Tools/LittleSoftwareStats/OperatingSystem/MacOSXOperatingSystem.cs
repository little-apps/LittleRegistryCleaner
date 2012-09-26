using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LittleSoftwareStats.OperatingSystem
{
    internal class MacOSXOperatingSystem : UnixOperatingSystem
    {
        public MacOSXOperatingSystem()
        {

        }

        public override int Architecture
        {
            get { return 64; }
        }

        public override string Version
        {
            get
            {
                Regex regex = new Regex(@"System Version:\s(?<version>[\w\s\d\.]*)\s");
                MatchCollection matches = regex.Matches(Utils.SystemProfilerCommandOutput);
                return matches[0].Groups["version"].Value;
            }
        }

        Hardware.Hardware _hardware;
        public override Hardware.Hardware Hardware
        {
            get
            {
                if (_hardware == null)
                    _hardware = new Hardware.MacOSXHardware();
                return _hardware;
            }
        }

    }
}
