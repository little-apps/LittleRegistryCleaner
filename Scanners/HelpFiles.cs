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
    public class HelpFiles
    {
        private ScanDlg frmScanDlg;

        /// <summary>
        /// Scans for invalid windows help files
        /// </summary>
        public HelpFiles(ScanDlg frmScanDlg)
        {
            // Allow ScanDlg to be accessed globally
            this.frmScanDlg = frmScanDlg;

            try
            {
                CheckHelpFiles(Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\HTML Help"));
                CheckHelpFiles(Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\Help"));
            }
            catch (System.Security.SecurityException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private void CheckHelpFiles(RegistryKey regKey)
        {
            if (regKey == null)
                return;

            frmScanDlg.UpdateScanSubKey(regKey.ToString());

            foreach (string strHelpFile in regKey.GetValueNames())
            {
                string strHelpPath = (string)regKey.GetValue(strHelpFile);

                if (string.IsNullOrEmpty(strHelpPath))
                    continue;

                if (HelpFileExists(strHelpFile, strHelpPath))
                    continue;

                else
                    ScanDlg.StoreInvalidKey("Invalid file or folder", regKey.ToString(), strHelpFile);
            }

            return;
        }

        /// <summary>
        /// Sees if the help file exists
        /// </summary>
        /// <param name="strValueName">Should contain the filename</param>
        /// <param name="strValue">Should be the path to file</param>
        /// <returns>True if it exists</returns>
        private bool HelpFileExists(string strValueName, string strValue)
        {
            if (File.Exists(strValue))
                return true;

            if (File.Exists(strValueName))
                return true;

            if (File.Exists(Path.Combine(strValue, strValueName)))
                return true;

            if (StartUp.SearchFilePath(strValueName) || StartUp.SearchFilePath(strValue))
                return true;

            return false;
        }
    }
}
