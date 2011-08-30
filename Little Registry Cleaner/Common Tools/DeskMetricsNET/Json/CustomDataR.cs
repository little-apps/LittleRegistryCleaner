// **********************************************************************//
//                                                                       //
//     DeskMetrics NET - Json/CustomDataR.cs                             //
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
	public class CustomDataRJson : CustomDataJson
    {
        protected string ID;
        protected string  AppVersion;
        public CustomDataRJson(string name, string value, int flow, string ID, string app_version):base(name,value,flow)
        {
            this.ID = ID;
            AppVersion = app_version;
        }

        public override Hashtable GetJsonHashTable()
        {
            var json = base.GetJsonHashTable();
            json.Add("aver", AppVersion);
            json.Add("ID", ID);
            return json;
        }
    }
}

