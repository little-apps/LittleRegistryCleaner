// **********************************************************************//
//                                                                       //
//     DeskMetrics NET - Settings.cs                                     //
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
using System.Collections;
using System.Linq;
using System.Text;

namespace Common_Tools.DeskMetrics
{
    internal static class Settings
    {
        public const string UserAgent = "DeskMetricsNET";
        public const string DefaultServer = "api.deskmetrics.com";
        public const string NullStr = "null";
        public const string ApiEndpoint = "/sendData";

        public const int DefaultPort = 443;
        public const int Timeout = 25000; // 20 seconds

        public static readonly Dictionary<string, string> ErrorCodes = new Dictionary<string, string> 
        { 
          {"-14","Application version not found"},
          {"-13","Use POST Request"},
          {"-12","UserID not found"},
          {"-11","AppID not found"},
          {"-10","Missing required JSON data"},
          {"-9","Invalid JSON file"},
          {"-8","Empty POST data"},
          {"-7",""},
          {"-6",""},
          {"-5",""},
          {"-4",""},
          {"-3",""},
          {"-2",""},
          {"-1","Generic Exception"},
          {"0","OK"},
          {"1","OK"},
          {"2","Could not open HTTP component (InternetOpen)"},
          {"3","Could not connect to server (InternetConnect)"},
          {"4","Could not modify HTTP options (InternetSetOption)"},
          {"5","Could not modify HTTP security parameters (InternetQueryOption)"},
          {"6","Could not send HTTP request to server (HttpSendRequest)"},
          {"7","Could not read server response (InternetReadFile)"},
          {"8","Could not detect internet connection (InternetGetConnectedState)"},
          {"9","Bandwidth exceeded"}
        };    
    }
}
