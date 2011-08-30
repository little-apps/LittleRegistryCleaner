// **********************************************************************//
//                                                                       //
//     DeskMetrics NET - OperatingSystem/UnixOperatingSystem.cs          //
//     Copyright (c) 2010-2011 DeskMetrics Limited                       //
//                                                                       //
//     http://deskmetrics.com                                            //
//     http://support.deskmetrics.com                                    //
//                                                                       //
//     support@deskmetrics.com                                           //
//                                                                       //
//     This code is provided under the DeskMetrics Modified BSD License  //
//     A copy of this license has been distributed in a file called      //
//     LICENSE with this source code.                                    //
//                                                                       //
// **********************************************************************//

using System;
namespace Common_Tools.DeskMetrics.OperatingSystem
{
	internal class UnixOperatingSystem:IOperatingSystem
	{
		public UnixOperatingSystem ()
		{
		}
	

		#region IOperatingSystem implementation
		Hardware.IHardware _hardware;
		public override Hardware.IHardware Hardware {
			get {
				if (_hardware == null)
					_hardware = new DeskMetrics.OperatingSystem.Hardware.UnixHardware();
				return _hardware;
			}
			set {}
		}
		
		string _frameworkVersion;
		public override string FrameworkVersion {
			get {
				if (_frameworkVersion == null)
					_frameworkVersion = GetFrameworkVersion();
				return _frameworkVersion;
			}
			set {}
		}

		int _architecture = 0;
		public override int Architecture {
			get {
				if (_architecture == 0)
					_architecture = GetArchitecture();
				return _architecture;
			}
			set {}
		}

		string _javaVersion;
		public override string JavaVersion {
			get {
				if (_javaVersion == null)
					_javaVersion = GetUnixJavaVersion();
				return _javaVersion;
			}
			set {}
		}

		
		public override string ServicePack {
			get {
				return "none";
			}
			set {}
		}
		#endregion
		#region implemented abstract members of DeskMetrics.IOperatingSystem
		string _version;
		public override string Version {
			get {
				if (_version == null)
					_version = GetOperatingSystemVersion();
				return _version;
			}
			set{}
		}
		
		
		public override string FrameworkServicePack {
			get {
				return "none";
			}
			set{}
		}
		
		#endregion
		
		string GetOperatingSystemVersion()
		{
			return GetCommandExecutionOutput("uname","-rs");
		}
		
		string GetFrameworkVersion()
		{
			try
			{
				string[] f = GetCommandExecutionOutput("mono","--version").Split('\n');
                return f[0];
			}
			catch
			{
				return "none";
			}
		}
		
		string GetUnixJavaVersion()
		{
			try
			{
				string[] j = GetCommandExecutionOutput("java","-version 2>&1").Split('\n');
				j = j[0].Split('"');
                return  j[1];
			}
			catch
			{
				return "none";
			}
		}
		
		int GetArchitecture()
		{
			try{
				string arch = GetCommandExecutionOutput("uname","-m");
				if (arch.Contains("64"))
					return 64;
			}
			catch{}
			
			return 32;
		}
	}
}

