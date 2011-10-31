// **********************************************************************//
//                                                                       //
//     DeskMetrics NET - EventType.cs                                    //
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

namespace Common_Tools.DeskMetrics
{
    public class EventType
    {
        public const string StartApplication = "strApp";
        public const string StopApplication = "stApp";
        public const string Event = "ev";
        public const string CustomData = "ctD";
        public const string CustomDataR = "ctDR";
        public const string Log = "lg";
        public const string Exception = "exC";
        public const string EventValue = "evV";
        public const string EventPeriod = "evP";
    }
}
