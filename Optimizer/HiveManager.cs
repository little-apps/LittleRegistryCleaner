/*
    Little Registry Cleaner
    Copyright (C) 2008 Little Apps (http://www.littleapps.co.cc/)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using Little_Registry_Cleaner.Xml;
using System.Threading;
using System.ComponentModel;

namespace Little_Registry_Cleaner.Optimizer
{
    public class Hive
    {
        [DllImport("advapi32.dll", EntryPoint = "RegOpenKey")] public static extern int RegOpenKeyA(uint hKey, string lpSubKey, ref int phkResult);
        [DllImport("advapi32.dll", EntryPoint = "RegReplaceKey", SetLastError=true)] public static extern int RegReplaceKeyA(int hKey, string lpSubKey, string lpNewFile, string lpOldFile);
        [DllImport("advapi32.dll", EntryPoint = "RegSaveKey")] public static extern int RegSaveKeyA(int hKey, string lpFile, int lpSecurityAttributes);
        [DllImport("advapi32.dll")] public static extern int RegCloseKey(int hKey);
        [DllImport("kernel32.dll")] public static extern bool GetVolumePathName(string lpszFileName,[Out] StringBuilder lpszVolumePathName, uint cchBufferLength);

        private const uint HKEY_LOCAL_MACHINE = 0x80000002;
        private const uint HKEY_USERS = 0x80000003;

        public readonly FileInfo fiHive;
        public FileInfo fiHiveTemp;
        public readonly string HiveName;
        public readonly string HivePath;
        public string HiveTempPath;
        private int hKey;

        public Hive(string strHiveName, string strHivePath)
        {
            this.HiveName = strHiveName;
            this.HivePath = strHivePath;

            string strMSDOSPath = ConvertDeviceToMSDOSName(this.HivePath);
            if (Utils.FileExists(strMSDOSPath))
                this.fiHive = new FileInfo(ConvertDeviceToMSDOSName(this.HivePath));
        }

        ~Hive()
        {
            if (this.hKey != 0)
                RegCloseKey(hKey);
        }

        /// <summary>
        /// Uses Windows RegSaveKeyA API to rewrite registry hive
        /// </summary>
        public void AnalyzeHive()
        {    
            int nRet = 0, hkey = 0;

            string strMainKey = this.HiveName.ToLower();
            string strSubKey = strMainKey.Substring(strMainKey.LastIndexOf('\\') + 1);

            // Open Handle to registry key
            if (strMainKey.Contains("\\registry\\machine"))
                nRet = RegOpenKeyA(HKEY_LOCAL_MACHINE, strSubKey, ref hkey);
            if (strMainKey.Contains("\\registry\\user"))
                nRet = RegOpenKeyA(HKEY_USERS, strSubKey, ref hkey);

            if (nRet != 0)
                return;

            // Get temporary path to new registry hive
            HiveTempPath = Path.ChangeExtension(this.fiHive.FullName, ".temp");

            if (File.Exists(HiveTempPath))
                File.Delete(HiveTempPath);

            Thread.BeginCriticalRegion();

            if (RegSaveKeyA(hkey, HiveTempPath, 0) == 0)
            {
                this.fiHiveTemp = new FileInfo(HiveTempPath);
                this.hKey = hkey;
            }

            Thread.EndCriticalRegion();
            return;
        }

        public void CompactHive() 
        {
            if (this.fiHiveTemp != null)
            {
                string strOldHivePath = Path.ChangeExtension(this.fiHive.FullName, ".bak");

                try { File.Delete(strOldHivePath); }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex.Message); }

                if (RegReplaceKeyA(this.hKey, null, this.fiHiveTemp.FullName, strOldHivePath) != 0)
                    throw new Exception("Error replacing old hive with compacted hive. Error Code: " + Marshal.GetLastWin32Error().ToString());

                this.fiHiveTemp = null;
            }
        }

        public static string ConvertDeviceToMSDOSName(string strDeviceName)
        {
            StringBuilder strMSDOSName = new StringBuilder(260);

            // Convert \Device\HarddiskVolumeX to X:\
            if (!GetVolumePathName(strDeviceName, strMSDOSName, (uint)strMSDOSName.Capacity))
                return string.Empty;

            // Remove \Device\HarddiskVolumeX from string
            string strMSDOSPath = Regex.Replace(strDeviceName, @"\\Device\\HarddiskVolume(\d)\\", strMSDOSName.ToString(), RegexOptions.IgnoreCase);

            return strMSDOSPath;
        }
    }

    public class HiveCollection : CollectionBase
    {
        public Hive this[int index]
        {
            get { return (Hive)InnerList[index]; }
            set { InnerList[index] = value; }
        }

        public int Add(Hive oHive)
        {
            return InnerList.Add(oHive);
        }

        public void Remove(Hive oHive)
        {
            InnerList.Remove(oHive);
        }
    }
}
