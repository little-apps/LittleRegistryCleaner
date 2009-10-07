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
using Microsoft.Win32;
using System.Windows.Forms;

namespace Little_Registry_Cleaner.Scanners
{
    public class ApplicationSettings : ScannerBase
    {
        public override string ScannerName
        {
            get { return "Application Settings"; }
        }

        public static void Scan()
        {
            try
            {
                ScanRegistryKey(Registry.LocalMachine.OpenSubKey("SOFTWARE"));
                ScanRegistryKey(Registry.CurrentUser.OpenSubKey("SOFTWARE"));

                if (Utils.Is64BitOS)
                {
                    ScanRegistryKey(Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node"));
                    ScanRegistryKey(Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Wow6432Node"));
                }
            }
            catch (System.Security.SecurityException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private static void ScanRegistryKey(RegistryKey baseRegKey)
        {
            if (baseRegKey == null)
                return;

            Main.Logger.WriteLine("Scanning " + baseRegKey.Name + " for empty registry keys");

            foreach (string strSubKey in baseRegKey.GetSubKeyNames())
            {
                // Skip needed keys, we dont want to mess the system up
                //if (strSubKey == "Microsoft" ||
                //    strSubKey == "Policies" ||
                //    strSubKey == "Classes" ||
                //    strSubKey == "Printers" ||
                //    strSubKey == "Wow6432Node")
                //    continue;

                if (IsEmptyRegistryKey(baseRegKey.OpenSubKey(strSubKey, true)))
                    ScanDlg.StoreInvalidKey("The registry key doesn't contain any data", baseRegKey.Name + "\\" + strSubKey);
            }

            baseRegKey.Close();
            return;
        }

        /// <summary>
        /// Recursively goes through the registry keys and finds how many values there are
        /// </summary>
        /// <param name="regKey">The base registry key</param>
        /// <returns>True if the registry key is emtpy</returns>
        private static bool IsEmptyRegistryKey(RegistryKey regKey)
        {
            if (regKey == null)
                return false;

            ScanDlg.UpdateScanningObject(regKey.ToString());

            int nValueCount = regKey.ValueCount;
            int nSubKeyCount = regKey.SubKeyCount;

            if (regKey.ValueCount == 0)
                if (regKey.GetValue("") != null)
                    nValueCount = 1;

            return (nValueCount == 0 && nSubKeyCount == 0);
        }
    }
}
