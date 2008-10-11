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
    public class COMObjects
    {
        private ScanDlg frmScanDlg;

        /// <summary>
        /// Scans ActiveX/COM Objects
        /// </summary>
        public COMObjects(ScanDlg frm)
        {
            // Allow ScanDlg to be accessed globally
            this.frmScanDlg = frm;

            // Scan all CLSID sub keys
            ScanCLSIDSubKey(Registry.ClassesRoot.OpenSubKey("CLSID"));
            ScanCLSIDSubKey(Registry.LocalMachine.OpenSubKey("SOFTWARE\\Classes\\CLSID"));
            ScanCLSIDSubKey(Registry.CurrentUser.OpenSubKey("SOFTWARE\\Classes\\CLSID"));

            ScanFileExts(Registry.ClassesRoot);
            ScanFileExts(Registry.LocalMachine.OpenSubKey("SOFTWARE\\Classes"));
            ScanFileExts(Registry.CurrentUser.OpenSubKey("SOFTWARE\\Classes"));

            ScanAppIds(Registry.ClassesRoot.OpenSubKey("AppID"));
            ScanAppIds(Registry.LocalMachine.OpenSubKey("SOFTWARE\\Classes\\AppID"));
            ScanAppIds(Registry.CurrentUser.OpenSubKey("SOFTWARE\\Classes\\AppID"));

            ScanBHOReferences();
        }

        /// <summary>
        /// Scans for the CLSID subkey
        /// <param name="regKey">Location of CLSID sub key</param>
        /// </summary>
        private void ScanCLSIDSubKey(RegistryKey regKey)
        {
            if (regKey == null)
                return;

            foreach (string strCLSID in regKey.GetSubKeyNames())
            {
                try
                {
                    RegistryKey regKey2 = regKey.OpenSubKey(strCLSID);

                    if (regKey2 == null)
                        continue;

                    frmScanDlg.UpdateScanSubKey(regKey2.ToString());

                    // Check for valid AppID
                    string strAppID = (string)regKey2.GetValue("AppID");

                    if (!string.IsNullOrEmpty(strAppID))
                        if (!AppIdExists(strAppID))
                            ScanDlg.StoreInvalidKey("Missing AppID reference", regKey2.ToString(), "AppID");

                    // See if DefaultIcon exists
                    RegistryKey regKeyIcon = regKey2.OpenSubKey("DefaultIcon");

                    if (regKeyIcon != null)
                    {
                        string strDefaultIcon = (string)regKeyIcon.GetValue("");

                        if (!string.IsNullOrEmpty(strDefaultIcon))
                            if (!IconExists(strDefaultIcon))
                                ScanDlg.StoreInvalidKey("Unable to find icon", regKeyIcon.ToString());
                    }

                    // Look for InprocServer files
                    RegistryKey regKeyInprocSrvr = regKey2.OpenSubKey("InprocServer");

                    if (regKeyInprocSrvr != null)
                    {
                        string strInprocServer = (string)regKeyInprocSrvr.GetValue("");

                        if (!string.IsNullOrEmpty(strInprocServer))
                            if (!InprocServerExists(strInprocServer))
                                ScanDlg.StoreInvalidKey("Unable to find InprocServer", regKeyInprocSrvr.ToString());
                    }

                    RegistryKey regKeyInprocSrvr32 = regKey2.OpenSubKey("InprocServer32");

                    if (regKeyInprocSrvr32 != null)
                    {
                        string strInprocServer32 = (string)regKeyInprocSrvr32.GetValue("");

                        if (!string.IsNullOrEmpty(strInprocServer32))
                            if (!InprocServerExists(strInprocServer32))
                                ScanDlg.StoreInvalidKey("Unable to find InprocServer32", regKeyInprocSrvr32.ToString());
                    }

                }
                catch (System.Security.SecurityException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }

            }

            regKey.Close();
            return;
        }

        /// <summary>
        /// Finds invalid ProgIDs referenced
        /// </summary>
        private void ScanFileExts(RegistryKey regKey)
        {
            if (regKey == null)
                return;

            foreach (string strFileExt in regKey.GetSubKeyNames())
            {
                try
                {
                    // Skip if its not a file extension
                    if (strFileExt[0] != '.')
                        continue;

                    RegistryKey regKey2 = regKey.OpenSubKey(strFileExt);

                    if (regKey2 == null)
                        continue;

                    // Update scan dialog
                    frmScanDlg.UpdateScanSubKey(regKey2.ToString());

                    // Find reference to ProgID
                    string strProgID = (string)regKey2.GetValue("");

                    if (!string.IsNullOrEmpty(strProgID))
                        if (!CheckProgIDReferences(strProgID))
                            ScanDlg.StoreInvalidKey("Missing ProgID reference", regKey2.ToString());
                }
                catch (System.Security.SecurityException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }

            regKey.Close();
        }

        /// <summary>
        /// Looks for invalid references to AppIDs
        /// </summary>
        private void ScanAppIds(RegistryKey regKey)
        {
            if (regKey == null)
                return;

            foreach (string strAppId in regKey.GetSubKeyNames())
            {
                try
                {
                    RegistryKey regKey2 = regKey.OpenSubKey(strAppId);

                    if (regKey2 == null)
                        continue;

                    // Update scan dialog
                    frmScanDlg.UpdateScanSubKey(regKey2.ToString());

                    // Check for AppId CLSID
                    string strCLSID = (string)regKey2.GetValue("AppID");

                    if (!string.IsNullOrEmpty(strCLSID))
                        // Check for reference to AppID
                        if (regKey.OpenSubKey(strCLSID) == null)
                            ScanDlg.StoreInvalidKey("Missing AppID reference", regKey2.ToString());
                }
                catch (System.Security.SecurityException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }

            regKey.Close();
        }

        /// <summary>
        /// Finds invalid browser help objects
        /// </summary>
        private void ScanBHOReferences()
        {
            try
            {
                RegistryKey regKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\explorer\\Browser Helper Objects");

                if (regKey == null)
                    return;

                foreach (string strBHOClsId in regKey.GetSubKeyNames())
                {
                    string strSubKey = regKey.ToString() + strBHOClsId;

                    // Update scan dialog
                    frmScanDlg.UpdateScanSubKey(strSubKey);

                    // See if reference exists, Otherwise, remove it...
                    if (!CheckCLSIDReferences(strBHOClsId))
                        ScanDlg.StoreInvalidKey("Missing CLSID reference", strSubKey);
                }

                regKey.Close();
            }
            catch (System.Security.SecurityException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Parses the InprocServer32 path and see if it exists
        /// </summary>
        /// <param name="strPath">The path of the file</param>
        /// <returns>True if the file exists</returns>
        private static bool InprocServerExists(string strPath)
        {
            string strBuffer = strPath.ToLower();

            // Removes qoutes
            if (strBuffer[0] == '"')
            {
                int iQouteLoc = 0, iQoutes = 1;
                for (int i = 0; (i < strBuffer.Length) && (iQoutes <= 2); i++)
                {
                    if (strBuffer[i] == '"')
                    {
                        strBuffer = strBuffer.Remove(i, 1);
                        iQouteLoc = i;
                        iQoutes++;
                    }
                }

                strBuffer = strBuffer.Substring(0, iQouteLoc);
            }

            // Expands environment variables
            strBuffer = Environment.ExpandEnvironmentVariables(strBuffer);

            // Check for file
            if (File.Exists(strBuffer))
                return true;

            if (StartUp.SearchFilePath(strBuffer))
                return true;

            return false;
        }

        private static bool CheckCLSIDReferences(string strGuid)
        {
            try
            {
                if (Registry.ClassesRoot.OpenSubKey("CLSID\\" + strGuid) != null)
                    return true;

                if (Registry.LocalMachine.OpenSubKey("Software\\Classes\\CLSID\\" + strGuid) != null)
                    return true;

                if (Registry.CurrentUser.OpenSubKey("Software\\Classes\\CLSID\\" + strGuid) != null)
                    return true;
            }
            catch (System.Security.SecurityException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return false;
        }

        private static bool CheckProgIDReferences(string strProgID)
        {
            try
            {
                if (Registry.ClassesRoot.OpenSubKey(strProgID) != null)
                    return true;

                if (Registry.LocalMachine.OpenSubKey("Software\\Classes\\" + strProgID) != null)
                    return true;

                if (Registry.CurrentUser.OpenSubKey("Software\\Classes\\" + strProgID) != null)
                    return true;
            }
            catch (System.Security.SecurityException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return false;
        }

        private static bool AppIdExists(string strAppId)
        {
            try
            {
                if (Registry.ClassesRoot.OpenSubKey("AppID\\" + strAppId) != null)
                    return true;

                if (Registry.LocalMachine.OpenSubKey("SOFTWARE\\Classes\\AppID\\" + strAppId) != null)
                    return true;

                if (Registry.CurrentUser.OpenSubKey("SOFTWARE\\Classes\\AppID\\" + strAppId) != null)
                    return true;
            }
            catch (System.Security.SecurityException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return false;
        }

        private static bool IconExists(string strDefaultIcon)
        {
            string strIconPath = "";

            // Remove quotes
            if (strDefaultIcon[0] == '"')
            {
                int iQouteLoc = 0, iQoutes = 1;
                for (int i = 0; (i < strDefaultIcon.Length) && (iQoutes <= 2); i++)
                {
                    if (strDefaultIcon[i] == '"')
                    {
                        strDefaultIcon = strDefaultIcon.Remove(i, 1);
                        iQouteLoc = i;
                        iQoutes++;
                    }
                }

                strDefaultIcon = strDefaultIcon.Substring(0, iQouteLoc);
            }

            // Get icon path
            if (strDefaultIcon.IndexOf(',') > 0)
                strIconPath = strDefaultIcon.Substring(0, strDefaultIcon.IndexOf(','));
            else
                strIconPath = strDefaultIcon;

            // Expand variables
            strIconPath = Environment.ExpandEnvironmentVariables(strIconPath);

            if (StartUp.SearchFilePath(strIconPath))
                return true;

            if (File.Exists(strIconPath))
                return true;

            return false;
        }
    }
}
