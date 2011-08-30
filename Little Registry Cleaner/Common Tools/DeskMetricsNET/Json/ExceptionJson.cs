// **********************************************************************//
//                                                                       //
//     DeskMetrics NET - Json/ExceptionJson.cs                           //
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
	public class ExceptionJson : BaseJson
    {
        protected Exception Exception;
        protected int Flow;
        public ExceptionJson(Exception e,int flow)
            : base(EventType.Exception, BaseJson.Session)
        {
            Exception = e;
            Flow = flow;
        }

        public override Hashtable GetJsonHashTable()
        {
            var json = base.GetJsonHashTable();
            json.Add("msg", Exception.Message.Trim().Replace("\r\n", "").Replace("  ", " ").Replace("\n", "").Replace(@"\n", "").Replace("\r", "").Replace("&", "").Replace("|", "").Replace(">", "").Replace("<", "").Replace("\t", "").Replace(@"\", @"/"));
            json.Add("stk", Exception.StackTrace.Trim().Replace("\r\n", "").Replace("  ", " ").Replace("\n", "").Replace(@"\n", "").Replace("\r", "").Replace("&", "").Replace("|", "").Replace(">", "").Replace("<", "").Replace("\t", "").Replace(@"\", @"/"));
            json.Add("src", Exception.Source.Trim().Replace("\r\n", "").Replace("  ", " ").Replace("\n", "").Replace(@"\n", "").Replace("\r", "").Replace("&", "").Replace("|", "").Replace(">", "").Replace("<", "").Replace("\t", "").Replace(@"\", @"/"));
            json.Add("tgs", Exception.TargetSite.ToString());
            json.Add("fl", Flow);
            return json;
        }
        

    }
}

