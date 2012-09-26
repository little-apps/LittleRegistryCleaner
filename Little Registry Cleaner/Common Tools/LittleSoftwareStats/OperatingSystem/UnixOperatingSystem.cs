using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LittleSoftwareStats.OperatingSystem
{
    internal class UnixOperatingSystem : OperatingSystem
    {
        public UnixOperatingSystem()
        {
        }

        Hardware.Hardware _hardware;
        public override Hardware.Hardware Hardware
        {
            get
            {
                if (_hardware == null)
                    _hardware = new Hardware.UnixHardware();
                return _hardware;
            }
        }

        public override string Version
        {
            get { return Utils.GetCommandExecutionOutput("uname", "-rs"); }
        }

        public override int ServicePack
        {
            get { return 0; }
        }

        private Version _frameworkVersion;
        public override Version FrameworkVersion
        {
            get
            {
                if (this._frameworkVersion == null)
                {
                    try
                    {
                        Type type = Type.GetType("Mono.Runtime");
                        if (type != null)
                        {
                            MethodInfo invokeGetDisplayName = type.GetMethod("GetDisplayName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                            if (invokeGetDisplayName != null)
                            {
                                string displayName = invokeGetDisplayName.Invoke(null, null) as string;
                                this._frameworkVersion = new Version(displayName.Substring(0, displayName.IndexOf(" ")));
                            }
                        }
                    }
                    catch { }

                    if (this._frameworkVersion == null)
                    {
                        // Just use CLR version
                        this._frameworkVersion = new Version(Environment.Version.Major, Environment.Version.Minor);
                    }
                }

                return this._frameworkVersion;
            }
        }

        public override int FrameworkSP
        {
            get { return 0; }
        }

        private Version _javaVersion;
        public override Version JavaVersion
        {
            get
            {
                if (this._javaVersion == null)
                {
                    try
                    {
                        string[] j = Utils.GetCommandExecutionOutput("java", "-version 2>&1").Split('\n');
                        j = j[0].Split('"');
                        this._javaVersion = new Version(j[1]);
                    }
                    catch
                    {
                        this._javaVersion = new Version();
                    }
                }

                return this._javaVersion;
            }
        }

        public override int Architecture
        {
            get 
            {
                try
                {
                    string arch = Utils.GetCommandExecutionOutput("uname", "-m");
                    if (arch.Contains("64"))
                        return 64;
                }
                catch { }

                return 32;
            }
        }
    }
}
