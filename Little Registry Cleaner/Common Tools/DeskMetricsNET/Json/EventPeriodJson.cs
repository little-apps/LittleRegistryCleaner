// **********************************************************************//
//                                                                       //
//     DeskMetrics NET - Json/EventPeriodJson.cs                         //
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
using System.Collections;

namespace Common_Tools.DeskMetrics.Json
{
	public class EventPeriodJson : EventJson
    {
        protected int Time;
        protected bool Completed;

        public EventPeriodJson(string category, string name, int flow,int time,bool completed)
            : base(category, name, flow)
        {
            Time = time;
            Completed = completed;
			Type = EventType.EventPeriod;
        }

        public override Hashtable GetJsonHashTable()
        {
            var json = base.GetJsonHashTable();
            json.Add("tm", Time);
            json.Add("ec", Completed?"1":"0");
            return json;
        }
    }
}

