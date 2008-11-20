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
using System.Collections;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Little_Registry_Cleaner.UninstallManager
{
    public class ProgramInfo : IComparable<ProgramInfo>
    {

        #region "Program Info"
        public string Key;
        public string DisplayName;
        public string UninstallString;
        public string QuietDisplayName;
        public string QuietUninstallString;
        public string DisplayVersion;
        public string Publisher;
        public string URLInfoAbout;
        public string URLUpdateInfo;
        public string HelpLink;
        public string HelpTelephone;
        public string Contact;
        public string Comments;
        public string Readme;
        public string DisplayIcon;
        public string ParentKeyName;
        
        public uint EstimatedSize;
        public bool SystemComponent;

        public bool WindowsInstaller
        {
            get
            {
                return (UninstallString.Contains("msiexec.exe") || QuietUninstallString.Contains("msiexec.exe"));
            }
        }

        public bool Uninstallable
        {
            get
            {
                return ((!string.IsNullOrEmpty(UninstallString)) || (!string.IsNullOrEmpty(QuietUninstallString)));
            }
        }

        public bool SlowCache;
        public Int64 InstallSize;
        public uint Frequency;
        public DateTime LastUsed;
        public string FileName;
        #endregion

        public ProgramInfo(RegistryKey regKey)
        {
            Key = regKey.Name.Substring(regKey.Name.LastIndexOf('\\') + 1);

            DisplayName = regKey.GetValue("DisplayName") as string;
            QuietDisplayName = regKey.GetValue("QuietDisplayName") as string;
            UninstallString = regKey.GetValue("UninstallString") as string;
            QuietUninstallString = regKey.GetValue("QuietUninstallString") as string;
            Publisher = regKey.GetValue("Publisher") as string;
            DisplayVersion = regKey.GetValue("DisplayVersion") as string;
            EstimatedSize = Convert.ToUInt32(regKey.GetValue("EstimatedSize"));
            HelpLink = regKey.GetValue("HelpLink") as string;
            URLInfoAbout = regKey.GetValue("URLInfoAbout") as string;
            HelpTelephone = regKey.GetValue("HelpTelephone") as string;
            Contact = regKey.GetValue("Contact") as string;
            Readme = regKey.GetValue("Readme") as string;
            Comments = regKey.GetValue("Comments") as string;
            DisplayIcon = regKey.GetValue("DisplayIcon") as string;
            ParentKeyName = regKey.GetValue("ParentKeyName") as string;

            SystemComponent = (Convert.ToUInt32(regKey.GetValue("SystemComponent")) == 1);

            SlowCache = false;
            InstallSize = 0;
            Frequency = 0;
            LastUsed = DateTime.MinValue;
        }

        public void Uninstall()
        {
            if (!string.IsNullOrEmpty(UninstallString))
            {
                IntPtr pid = Utils.CreateProcess(UninstallString);
                if (pid != IntPtr.Zero)
                    Process.GetProcessById(pid.ToInt32()).WaitForExit();
            }
            else if (!string.IsNullOrEmpty(QuietUninstallString))
            {
                IntPtr pid = Utils.CreateProcess(QuietUninstallString);
                if (pid != IntPtr.Zero)
                    Process.GetProcessById(pid.ToInt32()).WaitForExit();
            }
        }

        public void RemoveFromRegistry()
        {
            string strKeyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" + Key;
            Registry.LocalMachine.DeleteSubKeyTree(strKeyName);
        }

        public override string ToString()
        {
            return DisplayName;
        }

        #region "IComparable members"
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
