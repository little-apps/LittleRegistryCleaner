/*
    Little Registry Cleaner
    Copyright (C) 2008 Little Apps (http://www.littleapps.co.cc/)

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
    public static class COMObjects
    {
        
        /// <summary>
        /// Scans ActiveX/COM Objects
        /// </summary>
        public static void Scan()
        {
            try
            {
                // Scan all CLSID sub keys
                ScanCLSIDSubKey(Registry.ClassesRoot.OpenSubKey("CLSID"));
                ScanCLSIDSubKey(Registry.LocalMachine.OpenSubKey("SOFTWARE\\Classes\\CLSID"));
                ScanCLSIDSubKey(Registry.CurrentUser.OpenSubKey("SOFTWARE\\Classes\\CLSID"));

                // Scan file extensions
                ScanFileExts(Registry.ClassesRoot);
                ScanFileExts(Registry.LocalMachine.OpenSubKey("SOFTWARE\\Classes"));
                ScanFileExts(Registry.CurrentUser.OpenSubKey("SOFTWARE\\Classes"));

                // Scan appids
                ScanAppIds(Registry.ClassesRoot.OpenSubKey("AppID"));
                ScanAppIds(Registry.LocalMachine.OpenSubKey("SOFTWARE\\Classes\\AppID"));
                ScanAppIds(Registry.CurrentUser.OpenSubKey("SOFTWARE\\Classes\\AppID"));

                // Scan explorer subkey
                ScanExplorer();
            }
            catch (System.Security.SecurityException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        #region Scan functions

        /// <summary>
        /// Scans for the CLSID subkey
        /// <param name="regKey">Location of CLSID Sub Key</param>
        /// </summary>
        private static void ScanCLSIDSubKey(RegistryKey regKey)
        {
            if (regKey == null)
                return;

            foreach (string strCLSID in regKey.GetSubKeyNames())
            {
                RegistryKey rkCLSID = regKey.OpenSubKey(strCLSID);

                if (rkCLSID == null)
                    continue;

                ScanDlg.UpdateScanSubKey(rkCLSID.ToString());

                // Check for valid AppID
                string strAppID = regKey.GetValue("AppID") as string;
                if (!string.IsNullOrEmpty(strAppID))
                    if (!AppIDExists(strAppID))
                        ScanDlg.StoreInvalidKey("Missing AppID reference", rkCLSID.ToString(), "AppID");

                // See if DefaultIcon exists
                using (RegistryKey regKeyIcon = rkCLSID.OpenSubKey("DefaultIcon"))
                {
                    if (regKeyIcon != null)
                    {
                        string strDefaultIcon = regKeyIcon.GetValue("") as string;

                        if (!string.IsNullOrEmpty(strDefaultIcon))
                            if (!Utils.IconExists(strDefaultIcon))
                                ScanDlg.StoreInvalidKey("Unable to find icon", regKeyIcon.ToString());

                        regKeyIcon.Close();
                    }
                }

                // Look for InprocServer files
                using (RegistryKey regKeyInprocSrvr = rkCLSID.OpenSubKey("InprocServer"))
                {
                    if (regKeyInprocSrvr != null)
                    {
                        string strInprocServer = regKeyInprocSrvr.GetValue("") as string;

                        if (!string.IsNullOrEmpty(strInprocServer))
                            if (!Utils.FileExists(strInprocServer))
                                ScanDlg.StoreInvalidKey("Unable to find InprocServer", regKeyInprocSrvr.ToString());

                        regKeyInprocSrvr.Close();
                    }
                }

                using (RegistryKey regKeyInprocSrvr32 = rkCLSID.OpenSubKey("InprocServer32"))
                {
                    if (regKeyInprocSrvr32 != null)
                    {
                        string strInprocServer32 = regKeyInprocSrvr32.GetValue("") as string;

                        if (!string.IsNullOrEmpty(strInprocServer32))
                            if (!Utils.FileExists(strInprocServer32))
                                ScanDlg.StoreInvalidKey("Unable to find InprocServer32", regKeyInprocSrvr32.ToString());

                        regKeyInprocSrvr32.Close();
                    }
                }

                rkCLSID.Close();
            }

            regKey.Close();
            return;
        }

        /// <summary>
        /// Looks for invalid references to AppIDs
        /// </summary>
        private static void ScanAppIds(RegistryKey regKey)
        {
            RegistryKey rkAppId = null;

            if (regKey == null)
                return;

            foreach (string strAppId in regKey.GetSubKeyNames())
            {
                if ((rkAppId = regKey.OpenSubKey(strAppId)) == null)
                    continue;

                // Update scan dialog
                ScanDlg.UpdateScanSubKey(rkAppId.ToString());

                // Check for reference to AppID
                string strCLSID = rkAppId.GetValue("AppID") as string;

                if (!string.IsNullOrEmpty(strCLSID))
                    if (!AppIDExists(strCLSID))
                        ScanDlg.StoreInvalidKey("Missing AppID reference", rkAppId.ToString());
            }

            regKey.Close();
        }

        /// <summary>
        /// Finds invalid ProgIDs referenced
        /// </summary>
        private static void ScanFileExts(RegistryKey regKey)
        {
            RegistryKey rkFileExt = null;

            if (regKey == null)
                return;

            foreach (string strFileExt in regKey.GetSubKeyNames())
            {
                // Skip if its not a file extension or subkey doesnt exist
                if ((rkFileExt = regKey.OpenSubKey(strFileExt)) == null || strFileExt[0] != '.')
                    continue;

                // Update scan dialog
                ScanDlg.UpdateScanSubKey(rkFileExt.ToString());

                // Find reference to ProgID
                string strProgID = rkFileExt.GetValue("") as string;

                if (!string.IsNullOrEmpty(strProgID))
                    if (!ProgIDExists(strProgID))
                        ScanDlg.StoreInvalidKey("Missing ProgID reference", rkFileExt.ToString());
            }

            regKey.Close();

            return;
        }

        /// <summary>
        /// Finds invalid windows explorer entries
        /// </summary>
        private static void ScanExplorer()
        {
            // Check Browser Help Objects
            using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\explorer\\Browser Helper Objects"))
            {
                RegistryKey rkBHO = null;

                if (regKey != null)
                {
                    foreach (string strGuid in regKey.GetSubKeyNames())
                    {
                        if ((rkBHO = regKey.OpenSubKey(strGuid)) != null)
                        {
                            // Update scan dialog
                            ScanDlg.UpdateScanSubKey(rkBHO.ToString());

                            if (!CLSIDExists(strGuid))
                                ScanDlg.StoreInvalidKey("Missing CLSID reference", rkBHO.ToString());
                        }
                    }
                }
            }

            // Check IE Toolbars
            using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Internet Explorer\\Toolbar"))
            {
                if (regKey != null)
                {
                    // Update scan dialog
                    ScanDlg.UpdateScanSubKey(regKey.ToString());

                    foreach (string strGuid in regKey.GetValueNames())
                    {
                        if (!IEToolbarIsValid(strGuid))
                            ScanDlg.StoreInvalidKey("Toolbar is not valid", regKey.ToString(), strGuid);
                    }
                }
            }

            // Check IE Extensions
            using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Internet Explorer\\Extensions"))
            {
                RegistryKey rkExt = null;

                if (regKey != null)
                {
                    foreach (string strGuid in regKey.GetSubKeyNames())
                    {
                        if ((rkExt = regKey.OpenSubKey(strGuid)) != null)
                        {
                            // Update scan dialog
                            ScanDlg.UpdateScanSubKey(rkExt.ToString());

                            ValidateExplorerExt(rkExt);
                        }
                    }
                }
            }

            // Check Explorer Exts
            using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts"))
            {
                RegistryKey rkFileExt = null;

                if (regKey != null)
                {
                    foreach (string strFileExt in regKey.GetSubKeyNames())
                    {
                        if ((rkFileExt = regKey.OpenSubKey(strFileExt)) == null || strFileExt[0] != '.')
                            continue;

                        // Update scan dialog
                        ScanDlg.UpdateScanSubKey(rkFileExt.ToString());

                        ValidateFileExt(rkFileExt);
                    }
                }
            }

            return;
        }

        #endregion

        #region Scan Sub-Functions

        private static void ValidateFileExt(RegistryKey regKey)
        {
            bool bProgidExists = false, bAppExists = false;

            // Skip if UserChoice subkey exists
            if (regKey.OpenSubKey("UserChoice") != null)
                return;

            // Parse and verify OpenWithProgId List
            using (RegistryKey rkProgids = regKey.OpenSubKey("OpenWithProgids"))
            {
                if (rkProgids != null) 
                {
                    foreach (string strProgid in rkProgids.GetValueNames())
                    {
                        if (ProgIDExists(strProgid))
                            bProgidExists = true;
                    }
                }
            }

            // Check if files in OpenWithList exist
            using (RegistryKey rkOpenList = regKey.OpenSubKey("OpenWithList"))
            {
                if (rkOpenList != null)
                {
                    foreach (string strValueName in rkOpenList.GetValueNames())
                    {
                        if (strValueName == "MRUList")
                            continue;

                        string strApp = rkOpenList.GetValue(strValueName) as string;

                        if (ApplicationExists(strApp))
                            bAppExists = true;
                    }
                    
                }
            }

            if (!bProgidExists && !bAppExists)
                ScanDlg.StoreInvalidKey("Unused File Extension", regKey.ToString());

            return;
        }

        private static void ValidateExplorerExt(RegistryKey regKey)
        {
            try
            {
                // Sees if icon file exists
                string strHotIcon = regKey.GetValue("HotIcon") as string;
                if (!string.IsNullOrEmpty(strHotIcon))
                {
                    if (!Utils.IconExists(strHotIcon))
                    {
                        ScanDlg.StoreInvalidKey("Missing hot icon file", regKey.ToString());
                        return;
                    }
                }

                string strIcon = regKey.GetValue("Icon") as string;
                if (!string.IsNullOrEmpty(strIcon))
                {
                    if (!Utils.IconExists(strIcon))
                    {
                        ScanDlg.StoreInvalidKey("Missing icon file", regKey.ToString());
                        return;
                    }
                }

                // Lookup CLSID extension
                string strClsidExt = regKey.GetValue("ClsidExtension") as string;
                if (!string.IsNullOrEmpty(strClsidExt))
                {
                    if (!CLSIDExists(strClsidExt))
                    {
                        ScanDlg.StoreInvalidKey("Missing CLSID Extension", regKey.ToString());
                        return;
                    }
                }

                // See if files exist
                string strExec = regKey.GetValue("Exec") as string;
                if (!string.IsNullOrEmpty(strExec))
                {
                    if (!Utils.FileExists(strExec))
                    {
                        ScanDlg.StoreInvalidKey("Missing executable", regKey.ToString());
                        return;
                    }
                }

                string strScript = regKey.GetValue("Script") as string;
                if (!string.IsNullOrEmpty(strScript))
                {
                    if (!Utils.FileExists(strScript))
                    {
                        ScanDlg.StoreInvalidKey("Missing script file", regKey.ToString());
                        return;
                    }
                }
            }
            catch (System.Security.SecurityException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Checks for inprocserver file
        /// </summary>
        /// <param name="regKey">The registry key contain Inprocserver subkey</param>
        /// <returns>False if Inprocserver is null or doesnt exist</returns>
        private static bool InprocServerExists(RegistryKey regKey)
        {
            bool bRet = false;

            try
            {
                if (regKey != null)
                {
                    using (RegistryKey regKeyInprocSrvr = regKey.OpenSubKey("InprocServer"))
                    {
                        if (regKeyInprocSrvr != null)
                        {
                            string strInprocServer = regKeyInprocSrvr.GetValue("") as string;

                            if (!string.IsNullOrEmpty(strInprocServer))
                                if (Utils.FileExists(strInprocServer))
                                    bRet = true;

                            regKeyInprocSrvr.Close();
                        }
                    }

                    using (RegistryKey regKeyInprocSrvr32 = regKey.OpenSubKey("InprocServer32"))
                    {
                        if (regKeyInprocSrvr32 != null)
                        {
                            string strInprocServer32 = regKeyInprocSrvr32.GetValue("") as string;

                            if (!string.IsNullOrEmpty(strInprocServer32))
                                if (Utils.FileExists(strInprocServer32))
                                    bRet = true;

                            regKeyInprocSrvr32.Close();
                        }
                    }
                }
            }
            catch (System.Security.SecurityException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return bRet;
        }

        /// <summary>
        /// Checks if IE toolbar GUID is valid
        /// </summary>
        private static bool IEToolbarIsValid(string strGuid)
        {
            bool bRet = false;

            if (!CLSIDExists(strGuid))
                bRet = false;

            if (InprocServerExists(Registry.ClassesRoot.OpenSubKey("CLSID\\" + strGuid)))
                bRet = true;

            if (InprocServerExists(Registry.LocalMachine.OpenSubKey("Software\\Classes\\CLSID\\" + strGuid)))
                bRet = true;

            if (InprocServerExists(Registry.CurrentUser.OpenSubKey("Software\\Classes\\CLSID\\" + strGuid)))
                bRet = true;

            return bRet;
        }

        /// <summary>
        /// Checks if application sub key exists
        /// </summary>
        /// <param name="strApp">Application name</param>
        /// <returns>True if app exists</returns>
        private static bool ApplicationExists(string strApp)
        {
            try
            {
                if (Registry.ClassesRoot.OpenSubKey("Applications\\" + strApp) != null)
                    return true;

                if (Registry.LocalMachine.OpenSubKey(@"Software\Classes\Applications\" + strApp) != null)
                    return true;

                if (Registry.CurrentUser.OpenSubKey(@"Software\Classes\Applications\" + strApp) != null)
                    return true;
            }
            catch (System.Security.SecurityException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return false;
        }

        /// <summary>
        /// Sees if the specified CLSID exists
        /// </summary>
        /// <param name="strGuid">The CLSID GUID</param>
        /// <returns>True if it exists</returns>
        private static bool CLSIDExists(string strGuid)
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

        /// <summary>
        /// Checks if the ProgID exists
        /// </summary>
        /// <param name="strProgID">The ProgID</param>
        /// <returns>True if it exists</returns>
        private static bool ProgIDExists(string strProgID)
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

        /// <summary>
        /// Checks if the AppID exists
        /// </summary>
        /// <param name="strAppId">The AppID or GUID</param>
        /// <returns>True if it exists</returns>
        private static bool AppIDExists(string strAppId)
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

        #endregion
    }
}