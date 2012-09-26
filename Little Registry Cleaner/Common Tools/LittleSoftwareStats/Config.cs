using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LittleSoftwareStats
{
    public static class Config
    {
        public static bool Enabled { get; set; }

        internal static string ApiUrl
        {
            get
            {
                UriBuilder uri = new UriBuilder((Config.ApiSecure) ? ("https") : ("http"), Config.ApiHost, Config.ApiPort, Config.ApiPath);

                return uri.ToString();
            }
        }
        internal static string AppId { get; set; }
        internal static string AppVer { get; set; }

        internal const string ApiHost = "stats.little-apps.org";
        internal const int ApiPort = 80;
        internal const bool ApiSecure = false;
        internal const string ApiPath = "api."+ApiFormat;

        internal const string ApiFormat = "json";
        internal const string ApiUserAgent = "LittleSoftwareStatsNET";
        internal const int ApiTimeout = 25000;
    }
}
