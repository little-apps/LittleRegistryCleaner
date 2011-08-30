// **********************************************************************//
//                                                                       //
//     DeskMetrics NET - OperatingSystem/Hardware/IHardware.cs            //
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common_Tools.DeskMetrics.OperatingSystem.Hardware
{
    public abstract class IHardware
    {
        public abstract string ProcessorName { get; set; }
        public abstract int ProcessorArchicteture { get; set; }
        public abstract int ProcessorCores { get; set; }
        public abstract double MemoryTotal { get; set; }
        public abstract double MemoryFree { get; set; }
        public abstract long DiskTotal { get; set; }
        public abstract long DiskFree { get; set; }
        public abstract string ScreenResolution { get; set; }
        public abstract string ProcessorBrand { get; set; }
        public abstract double ProcessorFrequency { get; set; }

    }
}
