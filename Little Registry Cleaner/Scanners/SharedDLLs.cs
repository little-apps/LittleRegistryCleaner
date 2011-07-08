/*
    Little Registry Cleaner
    Copyright (C) 2008-2011 Little Apps (http://www.little-apps.org/)

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
    public class SharedDLLs : ScannerBase
    {
        public override string ScannerName
        {
            get { return Strings.SharedDLLs; }
        }

        /// <summary>
        /// Scan for missing links to DLLS
        /// </summary>
        public static void Scan()
        {
            try
            {
                RegistryKey regKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\SharedDLLs");

                if (regKey == null)
                    return;

                Main.Logger.WriteLine("Scanning for missing shared DLLs");

                // Validate Each DLL from the value names
                foreach (string strFilePath in regKey.GetValueNames())
                {
                    // Update scan dialog
                    ScanDlg.CurrentScannedObject = strFilePath;

                    if (!string.IsNullOrEmpty(strFilePath))
                        if (!Utils.FileExists(strFilePath))
                            ScanDlg.StoreInvalidKey(Strings.InvalidFile, regKey.Name, strFilePath);
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
