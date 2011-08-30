// **********************************************************************//
//                                                                       //
//     DeskMetrics NET - Json/BaseJson.cs                                //
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
	public abstract class BaseJson
    {
        protected string Type;

        private static string _session;
        protected static string Session
        {
            get
            {
                return _session;
            }
            set
            {
                //ensure that Session will be filled only once
                if (string.IsNullOrEmpty(_session) && !string.IsNullOrEmpty(value))
                    _session = value;
            }
        }

        protected int TimeStamp;
        protected Hashtable json;

        public BaseJson(string type,string session)
        {
            Session = session;
            Type = type;
            TimeStamp = Util.GetTimeStamp();
            json = new Hashtable();
        }

        public virtual Hashtable GetJsonHashTable()
        {
            json.Add("tp", Type);
            json.Add("ss", Session);
            json.Add("ts", TimeStamp);
            return json;
        }
    }
}

