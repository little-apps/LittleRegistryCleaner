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
        public AppPaths(ScanDlg frm)
        {
            RegistryKey regKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\App Paths");

            if (regKey == null)
                return;

            foreach (string strSubKey in regKey.GetSubKeyNames())
            {
                try
                {
                    RegistryKey regKey2 = regKey.OpenSubKey(strSubKey);

                    if (regKey2 == null)
                        continue;

                    frm.UpdateScanSubKey(regKey2.ToString());

                    string strAppPath = (string)regKey2.GetValue("");

                    if (string.IsNullOrEmpty(strAppPath))
                        continue;

                    // Remove quotes
                    if (strAppPath[0] == '"')
                    {
                        int i, iQouteLoc = 0, iQoutes = 1;
                        for (i = 0; (i < strAppPath.Length) && (iQoutes <= 2); i++)
                        {
                            if (strAppPath[i] == '"')
                            {
                                strAppPath = strAppPath.Remove(i, 1);
                                iQouteLoc = i;
                                iQoutes++;
                            }
                        }

                        strAppPath = strAppPath.Substring(0, iQouteLoc);
                    }

                    // Check if file exists
                    if (!File.Exists(strAppPath))
                        frm.StoreInvalidKey("Invalid file or folder", regKey2.Name, "(default)");
                }
                catch (System.Security.SecurityException ex)
                { 
                    System.Diagnostics.Debug.WriteLine(ex.Message); 
                }
            }

            regKey.Close();
        }
    }
}
