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
using System.Collections;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Little_Registry_Cleaner.UninstallManager
{
    public class ProgramInfo : IComparable<ProgramInfo>
    {
        #region Slow Info Cache properties

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Size = 552)]
        internal struct SlowInfoCache
        {

            public uint cbSize; // size of the SlowInfoCache (552 bytes)
            public uint HasName; // unknown
            public Int64 InstallSize; // program size in bytes
            public System.Runtime.InteropServices.ComTypes.FILETIME LastUsed; // last time program was used
            public uint Frequency; // 0-2 = rarely; 3-9 = occassionaly; 10+ = frequently
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 262)]
            public string Name; //remaining 524 bytes (max path of 260 + null) in unicode
        }

        public bool SlowCache;
        public Int64 InstallSize;
        public uint Frequency;
        public DateTime LastUsed;
        public string FileName;
        public string SlowInfoCacheRegKey;
        #endregion

        #region Program Info
        public readonly string Key;
        public readonly string DisplayName;
        public readonly string UninstallString;
        public readonly string QuietDisplayName;
        public readonly string QuietUninstallString;
        public readonly string DisplayVersion;
        public readonly string Publisher;
        public readonly string URLInfoAbout;
        public readonly string URLUpdateInfo;
        public readonly string HelpLink;
        public readonly string HelpTelephone;
        public readonly string Contact;
        public readonly string Comments;
        public readonly string Readme;
        public readonly string DisplayIcon;
        public readonly string ParentKeyName;
        public readonly string InstallLocation;
        public readonly string InstallSource;

        public readonly int NoModify;
        public readonly int NoRepair;

        public readonly int EstimatedSize;
        public readonly bool SystemComponent;
        private readonly int _windowsInstaller;

        public bool WindowsInstaller
        {
            get
            {
                if (_windowsInstaller == 1)
                    return true;

                if (!string.IsNullOrEmpty(UninstallString))
                    if (UninstallString.Contains("msiexec"))
                        return true;

                if (!string.IsNullOrEmpty(QuietUninstallString))
                    if (QuietUninstallString.Contains("msiexec"))
                        return true;

                return false;
            }
        }

        public bool Uninstallable
        {
            get { return ((!string.IsNullOrEmpty(UninstallString)) || (!string.IsNullOrEmpty(QuietUninstallString))); }
        }
        #endregion

        public ProgramInfo(RegistryKey regKey)
        {
            Key = regKey.Name.Substring(regKey.Name.LastIndexOf('\\') + 1);

            try
            {
                DisplayName = regKey.GetValue("DisplayName") as string;
                QuietDisplayName = regKey.GetValue("QuietDisplayName") as string;
                UninstallString = regKey.GetValue("UninstallString") as string;
                QuietUninstallString = regKey.GetValue("QuietUninstallString") as string;
                Publisher = regKey.GetValue("Publisher") as string;
                DisplayVersion = regKey.GetValue("DisplayVersion") as string;
                HelpLink = regKey.GetValue("HelpLink") as string;
                URLInfoAbout = regKey.GetValue("URLInfoAbout") as string;
                HelpTelephone = regKey.GetValue("HelpTelephone") as string;
                Contact = regKey.GetValue("Contact") as string;
                Readme = regKey.GetValue("Readme") as string;
                Comments = regKey.GetValue("Comments") as string;
                DisplayIcon = regKey.GetValue("DisplayIcon") as string;
                ParentKeyName = regKey.GetValue("ParentKeyName") as string;
                InstallLocation = regKey.GetValue("InstallLocation") as string;
                InstallSource = regKey.GetValue("InstallSource") as string;

                NoModify = (Int32)regKey.GetValue("NoModify", 0);
                NoRepair = (Int32)regKey.GetValue("NoRepair", 0);

                SystemComponent = (((Int32)regKey.GetValue("SystemComponent", 0) == 1) ? (true) : (false));
                _windowsInstaller = (Int32)regKey.GetValue("WindowsInstaller", 0);
                EstimatedSize = (Int32)regKey.GetValue("EstimatedSize", 0);
            }
            catch (Exception)
            {
                SystemComponent = false;
                EstimatedSize = 0;
            }

            return;
        }

        /// <summary>
        /// Gets cached information
        /// </summary>
        private void GetARPCache()
        {
            RegistryKey regKey = null;

            try
            {
                if ((regKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Management\ARPCache\" + ParentKeyName)) == null)
                    if ((regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Management\ARPCache\" + ParentKeyName)) == null)
                        return;

                byte[] b = (byte[])regKey.GetValue("SlowInfoCache");

                GCHandle gcHandle = GCHandle.Alloc(b, GCHandleType.Pinned);
                IntPtr ptr = gcHandle.AddrOfPinnedObject();
                SlowInfoCache slowInfoCache = (SlowInfoCache)Marshal.PtrToStructure(ptr, typeof(SlowInfoCache));

                this.SlowCache = true;
                this.SlowInfoCacheRegKey = regKey.ToString();

                this.InstallSize = slowInfoCache.InstallSize;
                this.Frequency = slowInfoCache.Frequency;
                this.LastUsed = Utils.FileTime2DateTime(slowInfoCache.LastUsed);
                if (slowInfoCache.HasName == 1)
                    this.FileName = slowInfoCache.Name;

                if (gcHandle.IsAllocated)
                    gcHandle.Free();

                regKey.Close();
            }
            catch
            {
                SlowCache = false;
                InstallSize = 0;
                Frequency = 0;
                LastUsed = DateTime.MinValue;
                FileName = "";
            }

            return;
        }

        public bool Uninstall()
        {
            string cmdLine = "";

            if (!string.IsNullOrEmpty(UninstallString))
                cmdLine = this.UninstallString;
            else if (!string.IsNullOrEmpty(QuietUninstallString))
                cmdLine = this.QuietUninstallString;

            if (string.IsNullOrEmpty(cmdLine))
            {
                if (MessageBox.Show(Properties.Resources.piInvalidUninstallString, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    this.RemoveFromRegistry();

                return false;
            }

            try
            {
                Process proc = Process.Start(cmdLine);
                proc.WaitForExit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0} {1}", Properties.Resources.piErrorUninstalling, ex.Message), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }

            MessageBox.Show(Properties.Resources.piSuccessUninstall, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

            return true;
        }

        public bool RemoveFromRegistry()
        {
            string strKeyName = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" + Key;

            try
            {
                if (Registry.LocalMachine.OpenSubKey(strKeyName, true) != null)
                    Registry.LocalMachine.DeleteSubKeyTree(strKeyName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0}: {1}", Properties.Resources.piErrorRegKey, ex.Message), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }

            MessageBox.Show(Properties.Resources.piSuccessRegKey, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

            return true;
        }

        public override string ToString()
        {
            return DisplayName;
        }

        #region IComparable members
        public int CompareTo(ProgramInfo other)
        {
            return (DisplayName == null) ? 0 : DisplayName.CompareTo(other.DisplayName);
        }

        public bool Equals(ProgramInfo other)
        {
            return (other.Key == Key);
        }
        #endregion
    }
}
