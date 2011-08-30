// **********************************************************************//
//                                                                       //
//     DeskMetrics NET - Json/EventJson.cs                               //
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
	public class EventJson : BaseJson
    {
        protected string Category;
        protected string Name;
        protected int Flow;

        public EventJson(string category, string name, int flow)
            : base(EventType.Event, BaseJson.Session)
        {
            Category = category;
            Name = name;
            Flow = flow;
        }

        public override Hashtable GetJsonHashTable()
        {
            var json = base.GetJsonHashTable();
            json.Add("ca", Category);
            json.Add("nm", Name);
            json.Add("fl", Flow);

            return json;
        }
    }
}

