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
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Little_Registry_Cleaner.Scanners
{
    public class Fonts
    {
        [DllImport("shell32.dll")]
        public static extern bool SHGetSpecialFolderPath(IntPtr hwndOwner, [Out] StringBuilder strPath, int nFolder, bool fCreate);

        const int CSIDL_FONTS = 0x0014;    // windows\fonts

        /// <summary>
        /// Finds invalid font references
        /// </summary>
        public Fonts(ScanDlg frm)
        {
            StringBuilder strPath = new StringBuilder(260);

            RegistryKey regKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion\\Fonts");

            if (regKey == null)
                return;

            if (!SHGetSpecialFolderPath(IntPtr.Zero, strPath, CSIDL_FONTS, false))
                return;

            frm.UpdateScanSubKey(regKey.ToString());

            foreach (string strFontName in regKey.GetValueNames())
            {
                string strValue = (string)regKey.GetValue(strFontName);

                // Skip if value is empty
                if (string.IsNullOrEmpty(strValue))
                    continue;

                // Check value by itself
                if (File.Exists(strValue))
                    continue;

                // Check for font in fonts folder
                string strFontPath = String.Format("{0}\\{1}", strPath.ToString(), strValue);

                if (!File.Exists(strFontPath))
                    frm.StoreInvalidKey("Invalid file or folder", regKey.ToString(), strFontName);
            }
        }
    }
}
