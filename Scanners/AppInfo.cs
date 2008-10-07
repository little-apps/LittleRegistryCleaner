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

/* HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\App Paths
 * HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Uninstall
 * HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts */

namespace Little_Registry_Cleaner.Scanners
{
    public class AppInfo
    {
        /// <summary>
        /// Verifies installed programs in add/remove list
        /// </summary>
        public AppInfo(ScanDlg frmScanDlg)
        {
            try
            {
                RegistryKey regKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall");

                if (regKey == null)
                    return;

                foreach (string strProgName in regKey.GetSubKeyNames())
                {

                    RegistryKey regKey2 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\" + strProgName);

                    frmScanDlg.UpdateScanSubKey(regKey2.ToString());

                    // Skip if installed by msi installer
                    int? nWinInstaller = (int?)regKey2.GetValue("WindowsInstaller");
                    if (nWinInstaller.HasValue)
                        if (nWinInstaller.Value == 1)
                            continue;

                    string strDisplayIcon = (string)regKey2.GetValue("DisplayIcon");

                    // Check display icon
                    if (!string.IsNullOrEmpty(strDisplayIcon))
                    {
                        // See if file already exists
                        if (!File.Exists(strDisplayIcon))
                        {
                            // Remove quotes
                            if (strDisplayIcon[0] == '"')
                            {
                                int i, iQouteLoc = 0, iQoutes = 1;
                                for (i = 0; (i < strDisplayIcon.Length) && (iQoutes <= 2); i++)
                                {
                                    if (strDisplayIcon[i] == '"')
                                    {
                                        strDisplayIcon = strDisplayIcon.Remove(i, 1);
                                        iQouteLoc = i;
                                        iQoutes++;
                                    }
                                }
                            }

                            // Parse display icon path
                            if (strDisplayIcon.LastIndexOf(',') > 0)
                            {
                                string strIconPath = strDisplayIcon.Substring(0, strDisplayIcon.LastIndexOf(','));

                                if (!File.Exists(strIconPath))
                                    frmScanDlg.StoreInvalidKey("Invalid file or folder", "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\" + strProgName, "DisplayIcon");
                            }
                            else if (!File.Exists(strDisplayIcon))
                                frmScanDlg.StoreInvalidKey("Invalid file or folder", "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\" + strProgName, "DisplayIcon");
                        }
                    }

                    string strInstallLocation = (string)regKey2.GetValue("InstallLocation");

                    if (!string.IsNullOrEmpty(strInstallLocation))
                    {
                        if (!Directory.Exists(strInstallLocation) && !File.Exists(strInstallLocation))
                        {
                            frmScanDlg.StoreInvalidSubKey("Invalid file or folder", "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\" + strProgName);
                            continue;
                        }
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
