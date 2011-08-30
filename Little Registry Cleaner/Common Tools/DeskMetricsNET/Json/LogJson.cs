// **********************************************************************//
//                                                                       //
//     DeskMetrics NET - Json/LogJson.cs                                 //
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
	public class LogJson : BaseJson
    {
        protected string Message;
        protected int Flow;
        public LogJson(string msg,int flow)
            : base(EventType.Log, BaseJson.Session)
        {
            Message = msg;
            Flow = flow;
        }

        public override Hashtable GetJsonHashTable()
        {
            var json = base.GetJsonHashTable();
            json.Add("ms", Message);
            json.Add("fl", Flow);
            return json;
        }
    }
}

