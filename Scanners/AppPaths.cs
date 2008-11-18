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
using Microsoft.Win32;

namespace Little_Registry_Cleaner.Scanners
{
    public class AppPaths
    {
        /// <summary>
        /// Verifies programs in App Paths
        /// </summary>
        public AppPaths()
        {
            try
            {
                RegistryKey regKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\App Paths");

                if (regKey == null)
                    return;

                foreach (string strSubKey in regKey.GetSubKeyNames())
                {
                    RegistryKey regKey2 = regKey.OpenSubKey(strSubKey);

                    if (regKey2 != null)
                    {
                        ScanDlg.UpdateScanSubKey(regKey2.ToString());

                        if (Convert.ToInt32(regKey2.GetValue("BlockOnTSNonInstallMode")) == 1)
                            continue;

                        string strAppPath = regKey2.GetValue("") as string;
                        string strAppDir = regKey2.GetValue("Path") as string;

                        if (string.IsNullOrEmpty(strAppPath))
                        {
                            ScanDlg.StoreInvalidKey("Invalid registry key", regKey2.ToString());
                            continue;
                        }

                        // Check if file exists
                        if (!Utils.FileExists(strAppPath) && !Utils.SearchPath(strAppPath, strAppDir) && !Utils.SearchPath(strSubKey, strAppDir))
                            ScanDlg.StoreInvalidKey("Invalid file or folder", regKey2.ToString());
                    }
                }

                regKey.Close();

            }
            catch (System.Security.SecurityException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}
