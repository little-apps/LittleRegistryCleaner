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
using Microsoft.Win32;

namespace Little_Registry_Cleaner.Scanners
{
    public class HelpFiles : ScannerBase
    {
        public override string ScannerName
        {
            get { return "Windows Help Files"; }
        }

        public static void Scan()
        {
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

        /// <summary>
        /// Scans for invalid windows help files
        /// </summary>
        private static void CheckHelpFiles(RegistryKey regKey)
        {
            if (regKey == null)
                return;

            Main.Logger.WriteLine("Checking for missing help files in " + regKey.Name);

            foreach (string strHelpFile in regKey.GetValueNames())
            {
                string strHelpPath = regKey.GetValue(strHelpFile) as string;

                ScanDlg.UpdateScanningObject(strHelpPath);

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
        private static bool HelpFileExists(string strValueName, string strValue)
        {
            if (Utils.FileExists(strValue))
                return true;

            if (Utils.FileExists(strValueName))
                return true;

            if (Utils.FileExists(Path.Combine(strValue, strValueName)))
                return true;

            return false;
        }
    }
}
