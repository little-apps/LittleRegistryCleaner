/*
    Little Registry Cleaner
    Copyright (C) 2008-2009 Little Apps (http://www.littleapps.co.cc/)

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
    public static class StartUp
    {
        public static void Scan()
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
        private static void CheckAutoRun(RegistryKey regKey)
        {
            bool bRet = true;

            if (regKey == null)
                return;

            ScanDlg.UpdateScanSubKey(regKey.ToString());

            foreach (string strProgName in regKey.GetValueNames())
            {
                string strRunPath = regKey.GetValue(strProgName) as string;
                string strFilePath, strArgs;

                // Check if value is not empty
                if (string.IsNullOrEmpty(strRunPath))
                    bRet = false;
                // See if file exists
                else if (Utils.ExtractArguments(strRunPath, out strFilePath, out strArgs))
                {
                    if (!string.IsNullOrEmpty(strFilePath))
                    {
                        if (!Utils.FileExists(strFilePath))
                            bRet = false;
                    }

                    // See if run path exists without arguments extracted
                    if (Utils.FileExists(strRunPath))
                        bRet = true;
                }

                if (!bRet)
                    ScanDlg.StoreInvalidKey("Invalid file or folder", regKey.Name, strProgName);
            }

            regKey.Close();
            return;
        }
        
    }
}
