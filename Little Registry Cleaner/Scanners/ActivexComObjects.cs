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
    public class ActivexComObjects : ScannerBase
    {
        public override string ScannerName
        {
            get { return Strings.ActivexComObjects; }
        }

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
                if (Utils.Is64BitOS)
                {
                    ScanCLSIDSubKey(Registry.ClassesRoot.OpenSubKey("Wow6432Node\\CLSID"));
                    ScanCLSIDSubKey(Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\Classes\\CLSID"));
                    ScanCLSIDSubKey(Registry.CurrentUser.OpenSubKey("SOFTWARE\\Wow6432Node\\Classes\\CLSID"));
                }

                // Scan file extensions + progids
                ScanClasses(Registry.ClassesRoot);
                ScanClasses(Registry.LocalMachine.OpenSubKey("SOFTWARE\\Classes"));
                ScanClasses(Registry.CurrentUser.OpenSubKey("SOFTWARE\\Classes"));
                if (Utils.Is64BitOS)
                {
                    ScanClasses(Registry.ClassesRoot.OpenSubKey("Wow6432Node"));
                    ScanClasses(Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\Classes"));
                    ScanClasses(Registry.CurrentUser.OpenSubKey("SOFTWARE\\Wow6432Node\\Classes"));
                }

                // Scan appids
                ScanAppIds(Registry.ClassesRoot.OpenSubKey("AppID"));
                ScanAppIds(Registry.LocalMachine.OpenSubKey("SOFTWARE\\Classes\\AppID"));
                ScanAppIds(Registry.CurrentUser.OpenSubKey("SOFTWARE\\Classes\\AppID"));
                if (Utils.Is64BitOS)
                {
                    ScanAppIds(Registry.ClassesRoot.OpenSubKey("Wow6432Node\\AppID"));
                    ScanAppIds(Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\AppID"));
                    ScanAppIds(Registry.CurrentUser.OpenSubKey("SOFTWARE\\Wow6432Node\\AppID"));
                }

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

            Main.Logger.WriteLine("Scanning " + regKey.Name + " for invalid CLSID's");

            foreach (string strCLSID in regKey.GetSubKeyNames())
            {
                RegistryKey rkCLSID = regKey.OpenSubKey(strCLSID);

                if (rkCLSID == null)
                    continue;

                ScanDlg.CurrentScannedObject = rkCLSID.ToString();

                // Check for valid AppID
                string strAppID = regKey.GetValue("AppID") as string;
                if (!string.IsNullOrEmpty(strAppID))
                    if (!appidExists(strAppID))
                        ScanDlg.StoreInvalidKey(Strings.MissingAppID, rkCLSID.ToString(), "AppID");

                // See if DefaultIcon exists
                using (RegistryKey regKeyDefaultIcon = rkCLSID.OpenSubKey("DefaultIcon"))
                {
                    if (regKeyDefaultIcon != null)
                    {
                        string iconPath = regKeyDefaultIcon.GetValue("") as string;

                        if (!string.IsNullOrEmpty(iconPath))
                            if (!Utils.IconExists(iconPath))
                                if (!ScanDlg.IsOnIgnoreList(iconPath))
                                    ScanDlg.StoreInvalidKey(Strings.InvalidFile, string.Format("{0}\\DefaultIcon", rkCLSID.ToString()));
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
                                ScanDlg.StoreInvalidKey(Strings.InvalidInprocServer, regKeyInprocSrvr.ToString());

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
                                ScanDlg.StoreInvalidKey(Strings.InvalidInprocServer32, regKeyInprocSrvr32.ToString());

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
            if (regKey == null)
                return;

            Main.Logger.WriteLine("Scanning " + regKey.Name + " for invalid AppID's");

            foreach (string strAppId in regKey.GetSubKeyNames())
            {
                using (RegistryKey rkAppId = regKey.OpenSubKey(strAppId))
                {
                    if (rkAppId != null)
                    {
                        // Update scan dialog
                        ScanDlg.CurrentScannedObject = rkAppId.ToString();

                        // Check for reference to AppID
                        string strCLSID = rkAppId.GetValue("AppID") as string;

                        if (!string.IsNullOrEmpty(strCLSID))
                            if (!appidExists(strCLSID))
                                ScanDlg.StoreInvalidKey(Strings.MissingAppID, rkAppId.ToString());
                    }
                }
            }

            regKey.Close();
        }

        /// <summary>
        /// Finds invalid File extensions + ProgIDs referenced
        /// </summary>
        private static void ScanClasses(RegistryKey regKey)
        {
            if (regKey == null)
                return;

            Main.Logger.WriteLine("Scanning " + regKey.Name + " for invalid Classes");

            foreach (string strSubKey in regKey.GetSubKeyNames())
            {
                // Update scan dialog
                ScanDlg.CurrentScannedObject = string.Format("{0}\\{1}", regKey.Name, strSubKey);

                // Skip any file (*)
                if (strSubKey == "*")
                    continue;

                if (strSubKey[0] == '.')
                {
                    // File Extension
                    using (RegistryKey rkFileExt = regKey.OpenSubKey(strSubKey))
                    {
                        if (rkFileExt != null)
                        {
                            // Find reference to ProgID
                            string strProgID = rkFileExt.GetValue("") as string;

                            if (!string.IsNullOrEmpty(strProgID))
                                if (!progIDExists(strProgID))
                                    ScanDlg.StoreInvalidKey(Strings.MissingProgID, rkFileExt.ToString());
                        }
                    }
                }
                else
                {
                    // ProgID or file class

                    // See if DefaultIcon exists
                    using (RegistryKey regKeyDefaultIcon = regKey.OpenSubKey(string.Format("{0}\\DefaultIcon", strSubKey)))
                    {
                        if (regKeyDefaultIcon != null)
                        {
                            string iconPath = regKeyDefaultIcon.GetValue("") as string;

                            if (!string.IsNullOrEmpty(iconPath))
                                if (!Utils.IconExists(iconPath))
                                    if (!ScanDlg.IsOnIgnoreList(iconPath))
                                        ScanDlg.StoreInvalidKey(Strings.InvalidFile, regKeyDefaultIcon.Name);
                        }
                    }

                    // Check referenced CLSID
                    using (RegistryKey rkCLSID = regKey.OpenSubKey(string.Format("{0}\\CLSID", strSubKey)))
                    {
                        if (rkCLSID != null)
                        {
                            string guid = rkCLSID.GetValue("") as string;

                            if (!string.IsNullOrEmpty(guid))
                                if (!clsidExists(guid))
                                    ScanDlg.StoreInvalidKey(Strings.MissingCLSID, string.Format("{0}\\{1}", regKey.Name, strSubKey));
                        }
                    }
                }

                // Check for unused progid/extension
                using (RegistryKey rk = regKey.OpenSubKey(strSubKey))
                {
                    if (rk != null)
                    {
                        if (rk.ValueCount <= 0 && rk.SubKeyCount <= 0)
                            ScanDlg.StoreInvalidKey(Strings.InvalidProgIDFileExt, rk.Name);
                    }
                }
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
                Main.Logger.WriteLine("Checking for invalid browser helper objects");

                if (regKey != null)
                {
                    RegistryKey rkBHO = null;

                    foreach (string strGuid in regKey.GetSubKeyNames())
                    {
                        if ((rkBHO = regKey.OpenSubKey(strGuid)) != null)
                        {
                            // Update scan dialog
                            ScanDlg.CurrentScannedObject = rkBHO.ToString();

                            if (!clsidExists(strGuid))
                                ScanDlg.StoreInvalidKey(Strings.MissingCLSID, rkBHO.ToString());
                        }
                    }
                }
            }

            // Check IE Toolbars
            using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Internet Explorer\\Toolbar"))
            {
                Main.Logger.WriteLine("Checking for invalid explorer toolbars");

                if (regKey != null)
                {
                    foreach (string strGuid in regKey.GetValueNames())
                    {
                        // Update scan dialog
                        ScanDlg.CurrentScannedObject = "CLSID: " + strGuid;

                        if (!IEToolbarIsValid(strGuid))
                            ScanDlg.StoreInvalidKey(Strings.InvalidToolbar, regKey.ToString(), strGuid);
                    }
                }
            }

            // Check IE Extensions
            using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Internet Explorer\\Extensions"))
            {
                RegistryKey rkExt = null;

                Main.Logger.WriteLine("Checking for invalid explorer extensions");

                if (regKey != null)
                {
                    foreach (string strGuid in regKey.GetSubKeyNames())
                    {
                        if ((rkExt = regKey.OpenSubKey(strGuid)) != null)
                        {
                            // Update scan dialog
                            ScanDlg.CurrentScannedObject = rkExt.ToString();

                            ValidateExplorerExt(rkExt);
                        }
                    }
                }
            }

            // Check Explorer File Exts
            using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts"))
            {
                RegistryKey rkFileExt = null;

                Main.Logger.WriteLine("Checking for invalid explorer file extensions");

                if (regKey != null)
                {
                    foreach (string strFileExt in regKey.GetSubKeyNames())
                    {
                        if ((rkFileExt = regKey.OpenSubKey(strFileExt)) == null || strFileExt[0] != '.')
                            continue;

                        // Update scan dialog
                        ScanDlg.CurrentScannedObject = rkFileExt.ToString();

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
                        if (progIDExists(strProgid))
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

                        if (appExists(strApp))
                            bAppExists = true;
                    }

                }
            }

            if (!bProgidExists && !bAppExists)
                ScanDlg.StoreInvalidKey(Strings.InvalidFileExt, regKey.ToString());

            return;
        }

        private static void ValidateExplorerExt(RegistryKey regKey)
        {
            try
            {
                // Sees if icon file exists
                string strHotIcon = regKey.GetValue("HotIcon") as string;
                if (!string.IsNullOrEmpty(strHotIcon))
                    if (!Utils.IconExists(strHotIcon))
                        ScanDlg.StoreInvalidKey(Strings.InvalidFile, regKey.ToString(), "HotIcon");

                string strIcon = regKey.GetValue("Icon") as string;
                if (!string.IsNullOrEmpty(strIcon))
                    if (!Utils.IconExists(strIcon))
                        ScanDlg.StoreInvalidKey(Strings.InvalidFile, regKey.ToString(), "Icon");

                // Lookup CLSID extension
                string strClsidExt = regKey.GetValue("ClsidExtension") as string;
                if (!string.IsNullOrEmpty(strClsidExt))
                    ScanDlg.StoreInvalidKey(Strings.MissingCLSID, regKey.ToString(), "ClsidExtension");

                // See if files exist
                string strExec = regKey.GetValue("Exec") as string;
                if (!string.IsNullOrEmpty(strExec))
                    if (!Utils.FileExists(strExec))
                        ScanDlg.StoreInvalidKey(Strings.InvalidFile, regKey.ToString(), "Exec");

                string strScript = regKey.GetValue("Script") as string;
                if (!string.IsNullOrEmpty(strScript))
                    if (!Utils.FileExists(strScript))
                        ScanDlg.StoreInvalidKey(Strings.InvalidFile, regKey.ToString(), "Script");
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
            if (regKey != null)
            {
                using (RegistryKey regKeyInprocSrvr = regKey.OpenSubKey("InprocServer"))
                {
                    if (regKeyInprocSrvr != null)
                    {
                        string strInprocServer = regKeyInprocSrvr.GetValue("") as string;

                        if (!string.IsNullOrEmpty(strInprocServer))
                            if (Utils.FileExists(strInprocServer))
                                return true;
                    }
                }

                using (RegistryKey regKeyInprocSrvr32 = regKey.OpenSubKey("InprocServer32"))
                {
                    if (regKeyInprocSrvr32 != null)
                    {
                        string strInprocServer32 = regKeyInprocSrvr32.GetValue("") as string;

                        if (!string.IsNullOrEmpty(strInprocServer32))
                            if (Utils.FileExists(strInprocServer32))
                                return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if IE toolbar GUID is valid
        /// </summary>
        private static bool IEToolbarIsValid(string strGuid)
        {
            bool bRet = false;

            if (!clsidExists(strGuid))
                bRet = false;

            if (InprocServerExists(Registry.ClassesRoot.OpenSubKey("CLSID\\" + strGuid)))
                bRet = true;

            if (InprocServerExists(Registry.LocalMachine.OpenSubKey("Software\\Classes\\CLSID\\" + strGuid)))
                bRet = true;

            if (InprocServerExists(Registry.CurrentUser.OpenSubKey("Software\\Classes\\CLSID\\" + strGuid)))
                bRet = true;

            if (Utils.Is64BitOS)
            {
                if (InprocServerExists(Registry.ClassesRoot.OpenSubKey("Wow6432Node\\CLSID\\" + strGuid)))
                    bRet = true;

                if (InprocServerExists(Registry.LocalMachine.OpenSubKey("Software\\Wow6432Node\\Classes\\CLSID\\" + strGuid)))
                    bRet = true;

                if (InprocServerExists(Registry.CurrentUser.OpenSubKey("Software\\Wow6432Node\\Classes\\CLSID\\" + strGuid)))
                    bRet = true;
            }

            return bRet;
        }

        /// <summary>
        /// Sees if application exists
        /// </summary>
        /// <param name="appName">Application Name</param>
        /// <returns>True if it exists</returns>
        private static bool appExists(string appName)
        {
            List<RegistryKey> listRegKeys = new List<RegistryKey>();

            listRegKeys.Add(Registry.ClassesRoot.OpenSubKey("Applications"));
            listRegKeys.Add(Registry.LocalMachine.OpenSubKey(@"Software\Classes\Applications"));
            listRegKeys.Add(Registry.CurrentUser.OpenSubKey(@"Software\Classes\Applications"));

            if (Utils.Is64BitOS)
            {
                listRegKeys.Add(Registry.ClassesRoot.OpenSubKey(@"Wow6432Node\Applications"));
                listRegKeys.Add(Registry.LocalMachine.OpenSubKey(@"Software\Wow6432Node\Classes\Applications"));
                listRegKeys.Add(Registry.CurrentUser.OpenSubKey(@"Software\Wow6432Node\Classes\Applications"));
            }

            try
            {
                foreach (RegistryKey rk in listRegKeys)
                {
                    if (rk == null)
                        continue;

                    using (RegistryKey subKey = rk.OpenSubKey(appName))
                    {
                        if (subKey != null)
                            if (!ScanDlg.IsOnIgnoreList(subKey.ToString()))
                                return true;
                    }
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        /// <summary>
        /// Sees if the specified CLSID exists
        /// </summary>
        /// <param name="clsid">The CLSID GUID</param>
        /// <returns>True if it exists</returns>
        private static bool clsidExists(string clsid)
        {
            List<RegistryKey> listRegKeys = new List<RegistryKey>();

            listRegKeys.Add(Registry.ClassesRoot.OpenSubKey("CLSID"));
            listRegKeys.Add(Registry.LocalMachine.OpenSubKey(@"Software\Classes\CLSID"));
            listRegKeys.Add(Registry.CurrentUser.OpenSubKey(@"Software\Classes\CLSID"));

            if (Utils.Is64BitOS)
            {
                listRegKeys.Add(Registry.ClassesRoot.OpenSubKey(@"Wow6432Node\CLSID"));
                listRegKeys.Add(Registry.LocalMachine.OpenSubKey(@"Software\Wow6432Node\Classes\CLSID"));
                listRegKeys.Add(Registry.CurrentUser.OpenSubKey(@"Software\Wow6432Node\Classes\CLSID"));
            }

            try
            {
                foreach (RegistryKey rk in listRegKeys)
                {
                    if (rk == null)
                        continue;

                    using (RegistryKey subKey = rk.OpenSubKey(clsid))
                    {
                        if (subKey != null)
                            if (!ScanDlg.IsOnIgnoreList(subKey.ToString()))
                                return true;
                    }
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        /// <summary>
        /// Checks if the ProgID exists in Classes subkey
        /// </summary>
        /// <param name="progID">The ProgID</param>
        /// <returns>True if it exists</returns>
        private static bool progIDExists(string progID)
        {
            List<RegistryKey> listRegKeys = new List<RegistryKey>();

            listRegKeys.Add(Registry.ClassesRoot);
            listRegKeys.Add(Registry.LocalMachine.OpenSubKey(@"Software\Classes"));
            listRegKeys.Add(Registry.CurrentUser.OpenSubKey(@"Software\Classes"));

            if (Utils.Is64BitOS)
            {
                listRegKeys.Add(Registry.ClassesRoot.OpenSubKey(@"Wow6432Node"));
                listRegKeys.Add(Registry.LocalMachine.OpenSubKey(@"Software\Wow6432Node\Classes"));
                listRegKeys.Add(Registry.CurrentUser.OpenSubKey(@"Software\Wow6432Node\Classes"));
            }

            try
            {
                foreach (RegistryKey rk in listRegKeys)
                {
                    if (rk == null)
                        continue;

                    using (RegistryKey subKey = rk.OpenSubKey(progID))
                    {
                        if (subKey != null)
                            if (!ScanDlg.IsOnIgnoreList(subKey.ToString()))
                                return true;
                    }
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        /// <summary>
        /// Checks if the AppID exists
        /// </summary>
        /// <param name="appID">The AppID or GUID</param>
        /// <returns>True if it exists</returns>
        private static bool appidExists(string appID)
        {
            List<RegistryKey> listRegKeys = new List<RegistryKey>();

            listRegKeys.Add(Registry.ClassesRoot.OpenSubKey(@"AppID"));
            listRegKeys.Add(Registry.LocalMachine.OpenSubKey(@"Software\Classes\AppID"));
            listRegKeys.Add(Registry.CurrentUser.OpenSubKey(@"Software\Classes\AppID"));

            if (Utils.Is64BitOS)
            {
                listRegKeys.Add(Registry.ClassesRoot.OpenSubKey(@"Wow6432Node\AppID"));
                listRegKeys.Add(Registry.LocalMachine.OpenSubKey(@"Software\Wow6432Node\Classes\AppID"));
                listRegKeys.Add(Registry.CurrentUser.OpenSubKey(@"Software\Wow6432Node\Classes\AppID"));
            }

            try
            {
                foreach (RegistryKey rk in listRegKeys)
                {
                    if (rk == null)
                        continue;

                    using (RegistryKey subKey = rk.OpenSubKey(appID))
                    {
                        if (subKey != null)
                            if (!ScanDlg.IsOnIgnoreList(subKey.ToString()))
                                return true;
                    }
                }
            }
            catch
            {
                return false;
            }

            return false;
        }


        #endregion
    }
}