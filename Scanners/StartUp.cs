/*
    Little Registry Cleaner
    Copyright (C) 2008 Nick H.

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Little_Registry_Cleaner.Scanners
{
    public class StartUp
    {
        private string mFlags;
        private string mCmd;
        private string mPath;

        public StartUp()
        {
            try
            {
                // all user keys
                CheckAutoRun(Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer\\Run"));
                CheckAutoRun(Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunServicesOnce"));
                CheckAutoRun(Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunServices"));
                CheckAutoRun(Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnceEx"));
                CheckAutoRun(Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce\\Setup"));
                CheckAutoRun(Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce"));
                CheckAutoRun(Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunEx"));
                CheckAutoRun(Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run"));

                // current user keys
                CheckAutoRun(Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer\\Run"));
                CheckAutoRun(Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunServicesOnce"));
                CheckAutoRun(Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunServices"));
                CheckAutoRun(Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnceEx"));
                CheckAutoRun(Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce\\Setup"));
                CheckAutoRun(Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce"));
                CheckAutoRun(Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunEx"));
                CheckAutoRun(Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run"));
            }
            catch (System.Security.SecurityException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Checks for invalid files in startup registry key
        /// </summary>
        /// <param name="regKey">The registry key to scan</param>
        private void CheckAutoRun(RegistryKey regKey)
        {
            if (regKey == null)
                return;

            ScanDlg.UpdateScanSubKey(regKey.ToString());

            foreach (string strProgName in regKey.GetValueNames())
            {
                string strRunPath = regKey.GetValue(strProgName) as string;

                if (string.IsNullOrEmpty(strRunPath))
                {
                    ScanDlg.StoreInvalidKey("Invalid file or folder", regKey.Name, strProgName);
                    continue;
                }

                // See if file exists
                if (Misc.FileExists(strRunPath))
                    continue;

                // Extract path from file and remove parameters
                ExtractRunPath(strRunPath);

                if (!string.IsNullOrEmpty(this.mPath))
                    if (Misc.FileExists(this.mPath))
                        continue;

                ScanDlg.StoreInvalidKey("Invalid file or folder", regKey.Name, strProgName);
            }

            regKey.Close();
            return;
        }

        /// <summary>
        /// Extracts parameters,quotes,etc from value
        /// </summary>
        /// <param name="strValue">Value to get file from</param>
        /// <returns>True if file can be extracted</returns>
        private bool ExtractRunPath(string strValue)
        {
            string strBuffer = strValue.ToLower().Trim();

            int nPos = -1;

            // Reset global variables
            this.mPath = "";
            this.mFlags = "";
            this.mCmd = "";

            if (strBuffer.Contains("rundll32"))
            {
                this.mFlags = strBuffer.Substring(strBuffer.IndexOf(" "));
                this.mPath = this.mFlags.Substring(0, this.mFlags.IndexOf(',')).Trim();

                return true;
            }

            if (strBuffer.Contains("regsvr32"))
            {
                this.mFlags = strBuffer.Substring(strBuffer.IndexOf(" "));
                this.mPath = this.mFlags.Substring(0, this.mFlags.IndexOf(',')).Trim();

                return true;
            }

            if (strBuffer[0] == '"')
            {
                int i, iQouteLoc = 0, iQoutes = 1;
                for (i = 0; (i < strBuffer.Length) && (iQoutes <= 2); i++)
                {
                    if (strBuffer[i] == '"')
                    {
                        strBuffer = strBuffer.Remove(i, 1);
                        iQouteLoc = i;
                        iQoutes++;
                    }
                }

                this.mPath = strBuffer.Substring(0, iQouteLoc);
                this.mFlags = strBuffer.Substring(iQouteLoc, strBuffer.Length - this.mPath.Length).Trim();

                return true;
            }

            this.mCmd = strBuffer;
            
            if ((nPos = strBuffer.LastIndexOf('.')) > -1)
                strBuffer = strBuffer.Substring(nPos);
            else
            {
                // no extension found
                this.mPath = strBuffer;
                this.mFlags = "";
                return false;
            }

            Match matchRegEx = Regex.Match(strBuffer, @"[ \\/-](\w+)", RegexOptions.RightToLeft);
            if (matchRegEx.Success)
                strBuffer = strBuffer.Substring(matchRegEx.Index);
            else
            {
                if ((nPos = strBuffer.IndexOf(' ')) > -1)
                    strBuffer.Substring(nPos);
                else
                    strBuffer = "";
            }

            this.mFlags = strBuffer.Trim();
            strBuffer = this.mCmd;
            this.mPath = strBuffer.Remove(strBuffer.IndexOf(this.mFlags), this.mFlags.Length).Trim();

            return true;
        }
        
    }
}
