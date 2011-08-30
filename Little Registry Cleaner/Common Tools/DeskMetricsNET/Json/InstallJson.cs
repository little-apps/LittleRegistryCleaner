// **********************************************************************//
//                                                                       //
//     DeskMetrics NET - Json/InstallJson.cs                             //
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
	public class InstallJson: BaseJson
	{
		public string ID;
		public string Version;
		
		public InstallJson(string version)
			:base("ist",BaseJson.Session)
		{
			ID = System.Guid.NewGuid().ToString().Replace("-", "").ToUpper();
			Version = version;
            if (BaseJson.Session == null)
            {
                BaseJson.Session = System.Guid.NewGuid().ToString().Replace("-", "").ToUpper();
            }
		}
		
		public override Hashtable GetJsonHashTable ()
		{
			var json = base.GetJsonHashTable();
			json.Add("ID",ID);
			json.Add("aver",Version);
			return json;
		}
	}
}

