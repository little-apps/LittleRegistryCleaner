// **********************************************************************//
//                                                                       //
//     DeskMetrics NET - Json/CustomDataJson.cs                          //
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
    public class CustomDataJson : BaseJson
    {
        protected string Name;
        protected string Value;
        protected int Flow;

        public CustomDataJson(string name,string value, int flow)
            : base(EventType.CustomData, BaseJson.Session)
        {
            Name = name;
            Value = value;
            Flow = flow;
        }

        public override Hashtable GetJsonHashTable()
        {
            var json = base.GetJsonHashTable();
            json.Add("nm", Name);
            json.Add("vl", Value);
            json.Add("fl", Flow);
            return json;
        }

    }
}

