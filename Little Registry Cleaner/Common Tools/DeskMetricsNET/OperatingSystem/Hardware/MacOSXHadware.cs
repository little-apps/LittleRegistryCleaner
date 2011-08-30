// **********************************************************************//
//                                                                       //
//     DeskMetrics NET - OperatingSystem/Hardware/MacOSXHardware.cs      //
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
namespace Common_Tools.DeskMetrics.OperatingSystem.Hardware
{
	public class MacOSXHadware:UnixHardware
	{
		public MacOSXHadware ()
		{
		}
	

		#region IHardware implementation
		public override string ProcessorName {
			get {
				return GetProcessorName();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public override int ProcessorArchicteture {
			get {
				return GetProcessorArchitecture();
			}
			set {}
		}

		public override  int ProcessorCores {
			get {
				return GetProcessorCores();
			}
			set {}
		}

		public override  string ProcessorBrand {
			get {
				return "GenuineIntel";
			}
			set {}
		}

		public override double ProcessorFrequency {
			get {
				return GetProcessorFrequency();
			}
			set {}
		}
		
		public override double MemoryTotal
		{
			get
			{
				return GetTotalMemory();
			}
			set{}
		}
		
		#endregion
		string _system_profiler;
		string _sysctl;
		
		internal string Sysctl {
			get {
				if (string.IsNullOrEmpty(_sysctl))
					_sysctl = IOperatingSystem.GetCommandExecutionOutput("sysctl","-a hw");
				return this._sysctl;
			}
			set {
				_sysctl = value;
			}
		}

		internal string SystemProfiler {
			get {
				if (string.IsNullOrEmpty(_system_profiler))
					_system_profiler = IOperatingSystem.GetCommandExecutionOutput("system_profiler","");
				return this._system_profiler;
			}
			set {
				_system_profiler = value;
			}
		}

		string GetProcessorName()
		{
			try
			{
				Regex regex = new Regex(@"Processor Name\s*:\\s*(?<processor>[\w\s\d\.]+)");
				MatchCollection matches = regex.Matches(SystemProfiler);
				return  matches[0].Groups["processor"].Value;
			}catch{}
			
			return "Generic";
		}
		
		double GetTotalMemory()
		{
			Regex regex = new Regex(@"hw\.memsize\s*(:|=)\s*(?<memory>\d+)");
			MatchCollection matches = regex.Matches(Sysctl);
			return  double.Parse(matches[0].Groups["memory"].Value);
		}
		
		int GetProcessorCores()
		{
			Regex regex = new Regex(@"hw\.availcpu\s*(:|=)\s*(?<cpus>\d+)");
			MatchCollection matches = regex.Matches(Sysctl);
			return  int.Parse(matches[0].Groups["cpus"].Value);
		}
		
		int GetProcessorArchitecture()
		{
			Regex regex = new Regex(@"hw\.cpu64bit_capable\s*(:|=)\s*(?<capable>\d+)");
			MatchCollection matches = regex.Matches(Sysctl);
			if (matches[0].Groups["cpus"].Value=="1")
				return 64;
			return 32;
		}
		
		double GetProcessorFrequency()
		{
			Regex regex = new Regex(@"hw\.cpufrequency\s*(:|=)\s*(?<cpu_frequency>\d+)");
			MatchCollection matches = regex.Matches(Sysctl);
			return  double.Parse(matches[0].Groups["cpu_frequency"].Value);
		}
	}
}