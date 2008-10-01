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
    public class Drivers
    {
        /// <summary>
        /// Scans for invalid references to fonts
        /// </summary>
        public Drivers(ScanDlg frm)
        {
            RegistryKey regKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Drivers32");

            if (regKey == null)
                return;

            frm.UpdateScanSubKey(regKey.ToString());

            foreach (string strDriverName in regKey.GetValueNames())
            {
                string strValue = (string)regKey.GetValue(strDriverName);

                // Check if value is empty
                if (string.IsNullOrEmpty(strValue))
                    continue;

                // check path by itself
                if (File.Exists(strValue))
                    continue;

                // append path to %windir%\system32
                string strDriverPath = string.Format("{0}\\{1}", Environment.SystemDirectory, strValue);

                if (!File.Exists(strDriverPath))
                    frm.StoreInvalidKey("Invalid file or folder", regKey.Name, strDriverName);
            }

            regKey.Close();
        }
    }
}
