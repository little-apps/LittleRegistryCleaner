/*
    Little Registry Cleaner
    Copyright (C) 2008 Little Apps (http://www.little-apps.org/)

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
    public class SystemDrivers : ScannerBase
    {
        public override string ScannerName
        {
            get { return Strings.SystemDrivers; }
        }

        /// <summary>
        /// Scans for invalid references to fonts
        /// </summary>
        public static void Scan()
        {
            try
            {
                using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Drivers32"))
                {
                    if (regKey == null)
                        return;

                    Main.Logger.WriteLine("Scanning for missing drivers");

                    foreach (string strDriverName in regKey.GetValueNames())
                    {
                        string strValue = regKey.GetValue(strDriverName) as string;

                        ScanDlg.CurrentScannedObject = strValue;

                        if (!string.IsNullOrEmpty(strValue))
                            if (!Utils.FileExists(strValue))
                                ScanDlg.StoreInvalidKey(Strings.InvalidFile, regKey.Name, strDriverName);
                    }
                }
            }
            catch (System.Security.SecurityException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}
