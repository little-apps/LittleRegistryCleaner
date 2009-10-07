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
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Little_Registry_Cleaner.Scanners
{
    public class ApplicationInfo : ScannerBase
    {
        public override string ScannerName
        {
            get { return "Application Info"; }
        }

        /// <summary>
        /// Verifies installed programs in add/remove list
        /// </summary>
        public static void Scan()
        {
            try
            {
                Main.Logger.WriteLine("Verifying programs in Add/Remove list");

                using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall"))
                {
                    if (regKey == null)
                        return;

                    foreach (string strProgName in regKey.GetSubKeyNames())
                    {
                        using (RegistryKey regKey2 = regKey.OpenSubKey(strProgName))
                        {
                            if (regKey2 != null)
                            {
                                ScanDlg.UpdateScanningObject(regKey2.ToString());

                                UninstallManager.ProgramInfo progInfo = new Little_Registry_Cleaner.UninstallManager.ProgramInfo(regKey2);

                                if (regKey2.ValueCount <= 0)
                                {
                                    ScanDlg.StoreInvalidKey("Invalid registry key", regKey2.ToString());
                                    continue;
                                }

                                if (progInfo.WindowsInstaller)
                                    continue;

                                if (string.IsNullOrEmpty(progInfo.DisplayName) && (!progInfo.Uninstallable))
                                {
                                    ScanDlg.StoreInvalidKey("Invalid registry key", regKey2.ToString());
                                    continue;
                                }

                                // Check display icon
                                if (!string.IsNullOrEmpty(progInfo.DisplayIcon))
                                    if (!Utils.IconExists(progInfo.DisplayIcon))
                                            ScanDlg.StoreInvalidKey("Invalid file or folder", regKey2.ToString(), "DisplayIcon");

                                // Check install location 
                                if (!string.IsNullOrEmpty(progInfo.InstallLocation))
                                    if ((!Utils.DirExists(progInfo.InstallLocation)) && (!Utils.FileExists(progInfo.InstallLocation)))
                                            ScanDlg.StoreInvalidKey("Invalid file or folder", regKey2.ToString(), "InstallLocation");

                                // Check install source 
                                if (!string.IsNullOrEmpty(progInfo.InstallSource))
                                    if ((!Utils.DirExists(progInfo.InstallSource)) && (!Utils.FileExists(progInfo.InstallSource)))
                                            ScanDlg.StoreInvalidKey("Invalid file or folder", regKey2.ToString(), "InstallSource");

                                // Check ARP Cache
                                if (progInfo.SlowCache)
                                {
                                    if (!string.IsNullOrEmpty(progInfo.FileName))
                                        if (!Utils.FileExists(progInfo.FileName))
                                            ScanDlg.StoreInvalidKey("Invalid registry key", progInfo.SlowInfoCacheRegKey);
                                }
                            }
                        }
                    }
                }

                Main.Logger.WriteLine("Verifying registry entries in Add/Remove Cache");

                checkARPCache(Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Management\ARPCache\"));
                checkARPCache(Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Management\ARPCache\"));
            }
            catch (System.Security.SecurityException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Do a cross-reference check on ARP Cache keys
        /// </summary>
        /// <param name="regKey"></param>
        private static void checkARPCache(RegistryKey regKey)
        {
            if (regKey == null)
                return;

            foreach (string subKey in regKey.GetSubKeyNames())
            {
                if (Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\" + subKey) == null)
                    ScanDlg.StoreInvalidKey("Obsolete registry key", string.Format("{0}/{1}", regKey.Name, subKey));
            }
        }
    }
}
