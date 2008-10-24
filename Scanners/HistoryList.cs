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
using System.Text.RegularExpressions;

namespace Little_Registry_Cleaner.Scanners
{
    class HistoryList
    {
        public HistoryList()
        {
            try
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\RecentDocs");

                if (regKey == null)
                    return;

                FindInvalidLinks(regKey);

                foreach (string strSubKey in regKey.GetSubKeyNames())
                {
                    RegistryKey regKey2 = regKey.OpenSubKey(strSubKey);

                    if (regKey2 == null)
                        continue;

                    FindInvalidLinks(regKey2);
                }
            }
            catch (System.Security.SecurityException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

        }

        private void FindInvalidLinks(RegistryKey regKey)
        {
            if (regKey == null)
                return;

            ScanDlg.UpdateScanSubKey(regKey.ToString());
            
            string strRecentDocs = Environment.GetFolderPath(Environment.SpecialFolder.Recent);

            foreach (string strValueName in regKey.GetValueNames())
            {
                // Ignore MRUListEx and others
                if (!Regex.IsMatch(strValueName, "[0-9]"))
                    continue;

                object obj = regKey.GetValue(strValueName);
                string strFileName = ExtractUnicodeStringFromBinary(obj);

                // See if file exists in Recent Docs folder
                if (!Misc.FileExists(string.Format("{0}\\{1}.lnk", strRecentDocs, strFileName)))
                    ScanDlg.StoreInvalidKey("Invalid file or folder", regKey.ToString(), strValueName);
            }

            return;
        }

        private string ExtractUnicodeStringFromBinary(object keyObj)
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
