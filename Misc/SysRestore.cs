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
using System.Runtime.InteropServices;

namespace Little_Registry_Cleaner
{
    public class SysRestore
    {
        [DllImport("srclient.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SRSetRestorePointW(ref RestorePointInfo pRestorePtSpec, out STATEMGRSTATUS pSMgrStatus);

        /// <summary>
        /// Contains information used by the SRSetRestorePoint function
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct RestorePointInfo
        {
            public int dwEventType; // The type of event
            public int dwRestorePtType; // The type of restore point
            public Int64 llSequenceNumber; // The sequence number of the restore point
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxDescW + 1)]
            public string szDescription; // The description to be displayed so the user can easily identify a restore point
        }

        /// <summary>
        /// Contains status information used by the SRSetRestorePoint function
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct STATEMGRSTATUS
        {
            public int nStatus; // The status code
            public Int64 llSequenceNumber; // The sequence number of the restore point
        }

        // Type of restorations
        public enum RestoreType
        {
            ApplicationInstall = 0, // Installing a new application
            ApplicationUninstall = 1, // An application has been uninstalled
            ModifySettings = 12, // An application has had features added or removed
            CancelledOperation = 13, // An application needs to delete the restore point it created
            Restore = 6, // System Restore
            Checkpoint = 7, // Checkpoint
            DeviceDriverInstall = 10, // Device driver has been installed
            FirstRun = 11, // Program used for 1st time 
            BackupRecovery = 14 // Restoring a backup
        } 

        // Constants
        public const Int16 BeginSystemChange = 100; // Start of operation 
        public const Int16 EndSystemChange = 101; // End of operation
        // Windows XP only - used to prevent the restore points intertwined
        public const Int16 BeginNestedSystemChange = 102;
        public const Int16 EndNestedSystemChange = 103;

        internal const Int16 DesktopSetting = 2; /* not implemented */
        internal const Int16 AccessibilitySetting = 3; /* not implemented */
        internal const Int16 OeSetting = 4; /* not implemented */
        internal const Int16 ApplicationRun = 5; /* not implemented */
        internal const Int16 WindowsShutdown = 8; /* not implemented */
        internal const Int16 WindowsBoot = 9; /* not implemented */
        internal const Int16 MaxDesc = 64;
        internal const Int16 MaxDescW = 256;

        /// <summary>
        /// Verifies that the OS can do system restores
        /// </summary>
        /// <returns>True if OS is either ME,XP,Vista</returns>
        public static bool SysRestoreAvailable()
        {
            int majorVersion = Environment.OSVersion.Version.Major;
            int minorVersion = Environment.OSVersion.Version.Minor;

            // See if it is enabled
            if (!Properties.Settings.Default.bOptionsRestore)
                return false;

            // Windows ME
            if (majorVersion == 4 && minorVersion == 90)
                return true;

            // Windows XP
            if (majorVersion == 5 && minorVersion == 1)
                return true;

            // Windows Vista
            if (majorVersion == 6 && minorVersion == 0)
                return true;

            // All others : Win 95, 98, 2000, Server
            return false;
        } 

        /// <summary>
        /// Starts system restore
        /// </summary>
        /// <param name="strDescription">The description of the restore</param>
        /// <param name="lSeqNum">Returns the sequence number</param>
        /// <returns>The status of call</returns>
        /// <seealso cref="Use EndRestore() or CancelRestore() to end the system restore"/>
        public static int StartRestore(string strDescription, out long lSeqNum)
        {
            RestorePointInfo rpInfo = new RestorePointInfo();
            STATEMGRSTATUS rpStatus = new STATEMGRSTATUS();

            if (!SysRestoreAvailable())
            {
                lSeqNum = 0;
                return 0;
            }

            // Prepare Restore Point
            rpInfo.dwEventType = BeginSystemChange;
            // By default we create a verification system
            rpInfo.dwRestorePtType = (int)RestoreType.Restore;
            rpInfo.llSequenceNumber = 0;
            rpInfo.szDescription = strDescription;

            SRSetRestorePointW(ref rpInfo, out rpStatus);
            
            lSeqNum = rpStatus.llSequenceNumber;

            return rpStatus.nStatus;
        }

        /// <summary>
        /// Ends system restore call
        /// </summary>
        /// <param name="lSeqNum">The restore sequence number</param>
        /// <returns>The status of call</returns>
        public static int EndRestore(long lSeqNum)
        {
            RestorePointInfo rpInfo = new RestorePointInfo();
            STATEMGRSTATUS rpStatus = new STATEMGRSTATUS();

            if (!SysRestoreAvailable())
                return 0;

            rpInfo.dwEventType = EndSystemChange;
            rpInfo.llSequenceNumber = lSeqNum;

            SRSetRestorePointW(ref rpInfo, out rpStatus);

            return rpStatus.nStatus;
        }

        /// <summary>
        /// Cancels restore call
        /// </summary>
        /// <param name="lSeqNum">The restore sequence number</param>
        /// <returns>The status of call</returns>
        public static int CancelRestore(long lSeqNum)
        {
            RestorePointInfo rpInfo = new RestorePointInfo();
            STATEMGRSTATUS rpStatus = new STATEMGRSTATUS();

            if (!SysRestoreAvailable())
                return 0;

            rpInfo.dwEventType = EndSystemChange;
            rpInfo.dwRestorePtType = (int)RestoreType.CancelledOperation;
            rpInfo.llSequenceNumber = lSeqNum;

            SRSetRestorePointW(ref rpInfo, out rpStatus);

            return rpStatus.nStatus;
        }
    }
}
