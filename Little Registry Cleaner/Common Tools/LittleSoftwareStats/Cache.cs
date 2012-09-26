using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LittleSoftwareStats
{
    public class Cache
    {
        private string FileName
        {
            get { return string.Format(@"{0}\{1}.{2}", Path.GetTempPath(), Config.AppId, Config.ApiFormat); }
        }

        public Cache()
        {
        }

        internal string GetCacheData()
        {
            string fileContents = "";

            if (File.Exists(FileName)) {
                fileContents = Utils.DecodeFrom64(File.ReadAllText(FileName));
            }

            return fileContents;
        }

        internal string GetPostData(Events events)
        {
            string cachedData = GetCacheData();
            string data = "";
            string output = "";

            if (Config.ApiFormat == "json")
                data = Utils.SerializeAsJSON(events);
            else
                data = Utils.SerializeAsXML(events);

            if (string.IsNullOrEmpty(cachedData))
            {
                if (Config.ApiFormat == "json")
                    output = "[" + data + "]";
                else
                    output = "<?xml version=\"1.0\"?><data>" + data + "</data>";
            }
            else
            {
                if (Config.ApiFormat == "json")
                    output += "[" + data;
                else
                    output += "<?xml version=\"1.0\"?><data>" + data;

                foreach (string line in cachedData.Split(Environment.NewLine.ToCharArray()))
                {
                    if (!string.IsNullOrEmpty(line.Trim()))
                    {
                        if (Config.ApiFormat == "json")
                            output += "," + line;
                        else
                            output += line;
                    }
                }

                if (Config.ApiFormat == "json")
                    output += "]";
                else
                    output += "</data>";
            }

            return output.Replace("&", "%26");
        }

        internal void SaveCacheToFile(Events events)
        {
            string data = "";

            if (Config.ApiFormat == "json")
                data = Utils.SerializeAsJSON(events);
            else
                data = Utils.SerializeAsXML(events);

            data += "\n" + GetCacheData();

            Delete();

            File.WriteAllText(FileName, Utils.EncodeTo64(data));
            File.SetAttributes(FileName, FileAttributes.Hidden);
        }

        internal void Delete()
        {
            if (File.Exists(FileName))
                File.Delete(FileName);
        }
    }
}
