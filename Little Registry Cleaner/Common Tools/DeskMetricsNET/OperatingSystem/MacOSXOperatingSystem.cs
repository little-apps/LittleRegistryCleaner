// **********************************************************************//
//                                                                       //
//     DeskMetrics NET - OperatingSystem/MacOSXOperatingSystem.cs        //
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
using System.Text.RegularExpressions;
namespace Common_Tools.DeskMetrics.OperatingSystem
{
	internal class MacOSXOperatingSystem: UnixOperatingSystem
	{
		public MacOSXOperatingSystem ()
		{
		}
		
		#region IOperatingSystem implementation

		public override int Architecture {
			get {
				return 64;
			}
			set {
				
			}
		}
		
		public override string Version {
			get {
				return GetVersion();
			}
			set {
			}
		}
		
		Hardware.IHardware _hardware;
		public override Hardware.IHardware Hardware {
			get {
				if (_hardware == null)
					_hardware = new DeskMetrics.OperatingSystem.Hardware.MacOSXHadware();
				return _hardware;
			}
			set {}
		}
		
		#endregion
		
		string _system_profiler;
		string SystemProfilerCommandOutput
		{
			get{
				if (string.IsNullOrEmpty(_system_profiler))
					_system_profiler = GetCommandExecutionOutput("system_profiler","");
				return _system_profiler;
			}
		}
		
		double GetTotalMemory()
		{
			Regex regex = new Regex(@"^\s{6}Memory:\s*(?<memory>\d+)");
			MatchCollection matches = regex.Matches(SystemProfilerCommandOutput);
			return  double.Parse(matches[0].Groups["memory"].Value)*1024*1024*1024;
		}
		
		string GetVersion()
		{
			Regex regex = new Regex(@"System Version:\s(?<version>[\w\s\d\.]*)\s");
			MatchCollection matches = regex.Matches(SystemProfilerCommandOutput);
			return  matches[0].Groups["version"].Value;
		}
	}
}

