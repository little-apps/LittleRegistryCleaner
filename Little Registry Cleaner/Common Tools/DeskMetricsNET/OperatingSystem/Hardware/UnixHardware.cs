// **********************************************************************//
//                                                                       //
//     DeskMetrics NET - OperatingSystem/Hardware/UnixHardware.cs        //
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
using System.Windows.Forms;
namespace Common_Tools.DeskMetrics.OperatingSystem.Hardware
{
	public class UnixHardware:IHardware
	{
		public UnixHardware ()
		{
		}
	

		#region IHardware implementation
		public override  string ProcessorName {
			get {
				return GetProcessorName();
			}
			set {}
		}

		public override int ProcessorArchicteture {
			get {
				return GetArchitecture();
			}
			set {}
		}

		public override int ProcessorCores {
			get {
				return GetNumberOfCores();
			}
			set {}
		}

		public override double MemoryTotal {
			get {
				return GetTotalMemory();
			}
			set {}
		}

		public override double MemoryFree {
			get {
				return GetFreeMemory();
			}
			set {}
		}

		public override long DiskTotal {
			get {
				return GetTotalDisk();
			}
			set {}
		}

		public override long DiskFree {
			get {
				return GetFreeDisk();
			}
			set {}
		}

		public override string ScreenResolution {
			get {
				return GetScreenResolution();
			}
			set {}
		}

		public override string ProcessorBrand {
			get {
				return GetProcessorBrand();
			}
			set {}
		}

		public override double ProcessorFrequency {
			get {
				return GetProcessorFrequency();
			}
			set {}
		}
		#endregion
		
		string GetProcessorBrand()
		{
			try
			{
				string output = IOperatingSystem.GetCommandExecutionOutput("cat","/proc/cpuinfo");
				Regex regex = new Regex(@"(?:vendor_id\s+:\s*)(?<vendor>\w*)");
				MatchCollection matches = regex.Matches(output);
				return matches[0].Groups[1].Value;
			}
			catch {}
			return "none";
		}
		
		string GetProcessorName()
		{
			try
			{
				string output = IOperatingSystem.GetCommandExecutionOutput("cat","/proc/cpuinfo");
				Regex regex = new Regex(@"(?:model name\s+:\s*)(?<ModelName>[\w \(\)@\.]*)");
				MatchCollection matches = regex.Matches(output);
				return matches[0].Groups["ModelName"].Value;
			}
			catch {}
			return "none";
		}
		
		int GetProcessorFrequency()
		{
			try
			{
				string output = IOperatingSystem.GetCommandExecutionOutput("cat","/proc/cpuinfo");
				Regex regex = new Regex(@"(?:bogomips\s+:\s*)(?<bogomips>\w*)");
				MatchCollection matches = regex.Matches(output);
				int bogomips = int.Parse(matches[0].Groups[1].Value);
				return bogomips/GetNumberOfCores();
			}
			catch {}
			return 0;
		}
		
		int GetNumberOfCores()
		{
			try
			{
				string output = IOperatingSystem.GetCommandExecutionOutput("cat","/proc/cpuinfo");
				Regex regex = new Regex(@"(?:cpu cores\s+:\s*)(?<num>\w*)");
				MatchCollection matches = regex.Matches(output);
				return Int32.Parse(matches[0].Groups[1].Value);
			}
			catch {}
			return -1;
		}
		
		int GetArchitecture()
		{
			try
			{
				string output = IOperatingSystem.GetCommandExecutionOutput("cat","/proc/cpuinfo");
				Regex regex = new Regex(@"flags\s+\s:[\w\s]*");
				MatchCollection matches = regex.Matches(output);
				string flags = matches[0].Groups[0].Value;
				if (flags.Contains(" lm"))
					return 64;
			}
			catch {}
			return 32;
		}
		
		double GetTotalMemory()
		{
			try
			{
				string output = IOperatingSystem.GetCommandExecutionOutput("cat","/proc/meminfo");
				Regex regex = new Regex(@"(?:MemTotal:\s*)(?<memtotal>\d+)");
				MatchCollection matches = regex.Matches(output);
				return  double.Parse(matches[0].Groups[1].Value)*1024;
			}
			catch {}
			return -1;
		}
		
		double GetFreeMemory()
		{
			try
			{
				string output = IOperatingSystem.GetCommandExecutionOutput("cat","/proc/meminfo");
				Regex regex = new Regex(@"(?:MemFree:\s*)(?<memtotal>\d+)");
				MatchCollection matches = regex.Matches(output);
				return  double.Parse(matches[0].Groups[1].Value)*1024;
			}
			catch {}
			return -1;
		}
		
		long GetTotalDisk()
		{
			try
			{
				string output = IOperatingSystem.GetCommandExecutionOutput("df","-k");
				Regex regex = new Regex(@"^/[\w/]*\s*(?<total>\d+)\s*(?<used>\d+)\s*(?<available>\d+)");
				MatchCollection matches = regex.Matches(output);
				long total=0;
				foreach (Match match in matches)
					total += long.Parse(match.Groups["total"].Value);
				
				return  total*1024;
			}
			catch {}
			return -1;
		}
		
		long GetFreeDisk()
		{
			try
			{
				string output = IOperatingSystem.GetCommandExecutionOutput("df","-B 1k");
				Regex regex = new Regex(@"^/[\w/]*\s*(?<total>\d+)\s*(?<used>\d+)\s*(?<available>\d+)");
				MatchCollection matches = regex.Matches(output);
				long total=0;
				foreach (Match match in matches)
					total += long.Parse(match.Groups["available"].Value);
				
				return  total*1024;
			}
			catch {}
			return -1;
		}
		
		string GetScreenResolution()
        {
            try
            {
                int deskHeight = Screen.PrimaryScreen.Bounds.Height;
                int deskWidth = Screen.PrimaryScreen.Bounds.Width;
                return deskWidth + "x" + deskHeight;
            }
            catch{ }
			return "none";
        }    
	}
}

