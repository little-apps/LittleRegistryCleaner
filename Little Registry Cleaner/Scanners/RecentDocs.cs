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
using System.Text.RegularExpressions;

namespace Little_Registry_Cleaner.Scanners
{
    public class RecentDocs : ScannerBase
    {
        public override string ScannerName
        {
            get { return Strings.RecentDocs; }
        }

        public static void Scan()
        {
            ScanMUICache();
            ScanExplorerDocs();
        }

        /// <summary>
        /// Checks MUI Cache for invalid file references (XP Only)
        /// </summary>
        private static void ScanMUICache()
        {
            try
            {
                using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\ShellNoRoam\MUICache"))
                {
                    if (regKey == null)
                        return;

                    foreach (string valueName in regKey.GetValueNames())
                    {
                        if (valueName.StartsWith("@") || valueName == "LangID")
                            continue;

                        ScanDlg.CurrentScannedObject = valueName;

                        if (!Utils.FileExists(valueName))
                            ScanDlg.StoreInvalidKey(Strings.InvalidFile, regKey.Name, valueName);
                    }
                }
            }
            catch (System.Security.SecurityException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Recurses through the recent documents registry keys for invalid files
        /// </summary>
        private static void ScanExplorerDocs()
        {
            try 
            {
                using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\RecentDocs"))
                {
                    if (regKey == null)
                        return;

                    Main.Logger.WriteLine("Cleaning invalid references in " + regKey.Name);

                    EnumMRUList(regKey);

                    foreach (string strSubKey in regKey.GetSubKeyNames())
                    {
                        RegistryKey subKey = regKey.OpenSubKey(strSubKey);

                        if (subKey == null)
                            continue;

                        EnumMRUList(subKey);
                    }
                }
            }
            catch (System.Security.SecurityException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private static void EnumMRUList(RegistryKey regKey)
        {
            foreach (string strValueName in regKey.GetValueNames())
            {
                string filePath, fileArgs;

                // Ignore MRUListEx and others
                if (!Regex.IsMatch(strValueName, "[0-9]"))
                    continue;

                string fileName = ExtractUnicodeStringFromBinary(regKey.GetValue(strValueName));
                string shortcutPath = string.Format("{0}\\{1}.lnk", Environment.GetFolderPath(Environment.SpecialFolder.Recent), fileName);

                ScanDlg.CurrentScannedObject = shortcutPath;

                // See if file exists in Recent Docs folder
                if (!string.IsNullOrEmpty(fileName))
                {
                    ScanDlg.StoreInvalidKey(Strings.InvalidRegKey, regKey.ToString(), strValueName);
                    continue;
                }

                if (!Utils.FileExists(shortcutPath) || !Utils.ResolveShortcut(shortcutPath, out filePath, out fileArgs))
                {
                    ScanDlg.StoreInvalidKey(Strings.InvalidFile, regKey.ToString(), strValueName);
                    continue;
                }
            }
        }

        /// <summary>
        /// Converts registry value to filename
        /// </summary>
        /// <param name="keyObj">Value from registry key</param>
        private static string ExtractUnicodeStringFromBinary(object keyObj)
        {
            string Value = keyObj.ToString();    //get object value 
            string strType = keyObj.GetType().Name;  //get object type

            if (strType == "Byte[]")
            {
                Value = "";
                byte[] Bytes = (byte[])keyObj;
                //this seems crude but cannot find a way to 'cast' a Unicode string to byte[]
                //even in case where we know the beginning format is Unicode
                //so do it the hard way

                char[] chars = Encoding.Unicode.GetChars(Bytes);
                foreach (char bt in chars)
                {
                    if (bt != 0)
                    {
                        Value = Value + bt; //construct string one at a time
                    }
                    else
                    {
                        break;  //apparently found 0,0 (end of string)
                    }
                }
            }
            return Value;
        }
    }
}
