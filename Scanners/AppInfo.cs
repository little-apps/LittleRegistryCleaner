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
    public class AppInfo
    {
        /// <summary>
        /// Verifies installed programs in add/remove list
        /// </summary>
        public AppInfo()
        {
            try
            {
                RegistryKey regKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall");

                if (regKey == null)
                    return;

                foreach (string strProgName in regKey.GetSubKeyNames())
                {

                    RegistryKey regKey2 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\" + strProgName);

                    ScanDlg.UpdateScanSubKey(regKey2.ToString());

                    // Skip if installed by msi installer
                    int? nWinInstaller = (int?)regKey2.GetValue("WindowsInstaller");
                    if (nWinInstaller.HasValue)
                        if (nWinInstaller.Value == 1)
                            continue;

                    // Check display icon
                    string strDisplayIcon = (string)regKey2.GetValue("DisplayIcon");
                    {
                        if (!string.IsNullOrEmpty(strDisplayIcon))
                            if (!Misc.IconExists(strDisplayIcon))
                                ScanDlg.StoreInvalidKey("Invalid file or folder", regKey2.ToString(), "DisplayIcon");
                    }

                    // Check install location
                    string strInstallLocation = (string)regKey2.GetValue("InstallLocation");
                    {
                        if (!string.IsNullOrEmpty(strInstallLocation))
                            if ((!Misc.DirExists(strInstallLocation)) && (!Misc.FileExists(strInstallLocation)))
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
