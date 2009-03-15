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
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Win32;

namespace Little_Registry_Cleaner
{
    static class Utils
    {
        #region Signatures imported from http://pinvoke.net

        [DllImport("kernel32.dll")] public static extern int SearchPath(string strPath, string strFileName, string strExtension, uint nBufferLength, StringBuilder strBuffer, string strFilePart);
        [DllImport("kernel32.dll")] public static extern DriveType GetDriveType([MarshalAs(UnmanagedType.LPStr)] string lpRootPathName);

        // Used for SHGetSpecialFolderPath
        public const int CSIDL_STARTUP = 0x0007; // All Users\Startup
        public const int CSIDL_COMMON_STARTUP = 0x0018; // Common Users\Startup

        [DllImport("shell32.dll")] public static extern bool SHGetSpecialFolderPath(IntPtr hwndOwner, [Out] StringBuilder lpszPath, int nFolder, bool fCreate);
        [DllImport("shell32.dll", EntryPoint = "FindExecutable")] public static extern long FindExecutableA(string lpFile, string lpDirectory, StringBuilder lpResult);
        [DllImport("shell32.dll", EntryPoint = "ExtractIconEx")] public static extern int ExtractIconExA(string lpszFile, int nIconIndex, ref IntPtr phiconLarge, ref IntPtr phiconSmall, int nIcons);
         
        [DllImport("Shlwapi.dll", SetLastError = true, CharSet = CharSet.Auto)] public static extern string PathGetArgs(string path);
        [DllImport("Shlwapi.dll", SetLastError = true, CharSet = CharSet.Auto)] public static extern void PathRemoveArgs([In, Out] StringBuilder path);
        [DllImport("Shlwapi.dll", SetLastError = true, CharSet = CharSet.Auto)] public static extern int PathParseIconLocation([In, Out] StringBuilder path);
        [DllImport("Shlwapi.dll", SetLastError = true, CharSet = CharSet.Auto)] public static extern void PathUnquoteSpaces([In, Out] StringBuilder path);
        [DllImport("Shlwapi.dll", SetLastError = true, CharSet = CharSet.Auto)] public static extern bool PathFileExists(string path);
        [DllImport("Shlwapi.dll", SetLastError = true, CharSet = CharSet.Auto)] public static extern bool PathStripToRoot([In, Out] StringBuilder path);
        [DllImport("Shlwapi.dll", SetLastError = true, CharSet = CharSet.Auto)] public static extern bool PathRemoveFileSpec([In, Out] StringBuilder path);

        [DllImport("user32.dll")] public static extern int DestroyIcon(IntPtr hIcon);
        

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SlowInfoCache
        {
            public uint cbSize;
            public uint HasName;
            public Int64 InstallSize;
            public FILETIME LastUsed;
            public uint Frequency;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 262)]
            public string Name;
        }
        #endregion
        #region "Interop (CreateProcess)"
        struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public IntPtr dwProcessId;
            public IntPtr dwThreadId;
        }

        struct STARTUPINFO
        {
            public int cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public int dwX;
            public int dwY;
            public int dwXSize;
            public int dwYSize;
            public int dwXCountChars;
            public int dwYCountChars;
            public int dwFillAttribute;
            public int dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public byte lpReserved2;
            public int hStdInput;
            public int hStdOutput;
            public int hStdError;
        }

        [DllImport("kernel32.dll")]
        static extern bool CreateProcess(string lpApplicationName,
          string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes,
          bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment,
          string lpCurrentDirectory, [In] ref STARTUPINFO lpStartupInfo,
          out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CloseHandle(IntPtr hObject);
        #endregion
        #region "Interop (IShellLink and IPersistFile)"
        [Flags()]
        enum SLGP_FLAGS
        {
            /// <summary>Retrieves the standard short (8.3 format) file name</summary>
            SLGP_SHORTPATH = 0x1,
            /// <summary>Retrieves the Universal Naming Convention (UNC) path name of the file</summary>
            SLGP_UNCPRIORITY = 0x2,
            /// <summary>Retrieves the raw path name. A raw path is something that might not exist and may include environment variables that need to be expanded</summary>
            SLGP_RAWPATH = 0x4
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        struct WIN32_FIND_DATAW
        {
            public uint dwFileAttributes;
            public long ftCreationTime;
            public long ftLastAccessTime;
            public long ftLastWriteTime;
            public uint nFileSizeHigh;
            public uint nFileSizeLow;
            public uint dwReserved0;
            public uint dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string cAlternateFileName;
        }

        [Flags()]

        enum SLR_FLAGS
        {
            /// <summary>
            /// Do not display a dialog box if the link cannot be resolved. When SLR_NO_UI is set,
            /// the high-order word of fFlags can be set to a time-out value that specifies the
            /// maximum amount of time to be spent resolving the link. The function returns if the
            /// link cannot be resolved within the time-out duration. If the high-order word is set
            /// to zero, the time-out duration will be set to the default value of 3,000 milliseconds
            /// (3 seconds). To specify a value, set the high word of fFlags to the desired time-out
            /// duration, in milliseconds.
            /// </summary>
            SLR_NO_UI = 0x1,
            /// <summary>Obsolete and no longer used</summary>
            SLR_ANY_MATCH = 0x2,
            /// <summary>If the link object has changed, update its path and list of identifiers.
            /// If SLR_UPDATE is set, you do not need to call IPersistFile::IsDirty to determine
            /// whether or not the link object has changed.</summary>
            SLR_UPDATE = 0x4,
            /// <summary>Do not update the link information</summary>
            SLR_NOUPDATE = 0x8,
            /// <summary>Do not execute the search heuristics</summary>
            SLR_NOSEARCH = 0x10,
            /// <summary>Do not use distributed link tracking</summary>
            SLR_NOTRACK = 0x20,
            /// <summary>Disable distributed link tracking. By default, distributed link tracking tracks
            /// removable media across multiple devices based on the volume name. It also uses the
            /// Universal Naming Convention (UNC) path to track remote file systems whose drive letter
            /// has changed. Setting SLR_NOLINKINFO disables both types of tracking.</summary>
            SLR_NOLINKINFO = 0x40,
            /// <summary>Call the Microsoft Windows Installer</summary>
            SLR_INVOKE_MSI = 0x80
        }


        /// <summary>The IShellLink interface allows Shell links to be created, modified, and resolved</summary>
        [ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("000214F9-0000-0000-C000-000000000046")]
        interface IShellLinkW
        {
            /// <summary>Retrieves the path and file name of a Shell link object</summary>
            void GetPath([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out WIN32_FIND_DATAW pfd, SLGP_FLAGS fFlags);
            /// <summary>Retrieves the list of item identifiers for a Shell link object</summary>
            void GetIDList(out IntPtr ppidl);
            /// <summary>Sets the pointer to an item identifier list (PIDL) for a Shell link object.</summary>
            void SetIDList(IntPtr pidl);
            /// <summary>Retrieves the description string for a Shell link object</summary>
            void GetDescription([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);
            /// <summary>Sets the description for a Shell link object. The description can be any application-defined string</summary>
            void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
            /// <summary>Retrieves the name of the working directory for a Shell link object</summary>
            void GetWorkingDirectory([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);
            /// <summary>Sets the name of the working directory for a Shell link object</summary>
            void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
            /// <summary>Retrieves the command-line arguments associated with a Shell link object</summary>
            void GetArguments([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
            /// <summary>Sets the command-line arguments for a Shell link object</summary>
            void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
            /// <summary>Retrieves the hot key for a Shell link object</summary>
            void GetHotkey(out short pwHotkey);
            /// <summary>Sets a hot key for a Shell link object</summary>
            void SetHotkey(short wHotkey);
            /// <summary>Retrieves the show command for a Shell link object</summary>
            void GetShowCmd(out int piShowCmd);
            /// <summary>Sets the show command for a Shell link object. The show command sets the initial show state of the window.</summary>
            void SetShowCmd(int iShowCmd);
            /// <summary>Retrieves the location (path and index) of the icon for a Shell link object</summary>
            void GetIconLocation([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath,
                int cchIconPath, out int piIcon);
            /// <summary>Sets the location (path and index) of the icon for a Shell link object</summary>
            void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
            /// <summary>Sets the relative path to the Shell link object</summary>
            void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
            /// <summary>Attempts to find the target of a Shell link, even if it has been moved or renamed</summary>
            void Resolve(IntPtr hwnd, SLR_FLAGS fFlags);
            /// <summary>Sets the path and file name of a Shell link object</summary>
            void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);

        }

        [ComImport, Guid("0000010c-0000-0000-c000-000000000046"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IPersist
        {
            [PreserveSig]
            void GetClassID(out Guid pClassID);
        }


        [ComImport, Guid("0000010b-0000-0000-C000-000000000046"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IPersistFile : IPersist
        {
            new void GetClassID(out Guid pClassID);
            [PreserveSig]
            int IsDirty();

            [PreserveSig]
            void Load([In, MarshalAs(UnmanagedType.LPWStr)]
            string pszFileName, uint dwMode);

            [PreserveSig]
            void Save([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName,
                [In, MarshalAs(UnmanagedType.Bool)] bool fRemember);

            [PreserveSig]
            void SaveCompleted([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName);

            [PreserveSig]
            void GetCurFile([In, MarshalAs(UnmanagedType.LPWStr)] string ppszFileName);
        }

        const uint STGM_READ = 0;
        const int MAX_PATH = 260;

        // CLSID_ShellLink from ShlGuid.h 
        [
            ComImport(),
            Guid("00021401-0000-0000-C000-000000000046")
        ]
        public class ShellLink
        {
        }

        #endregion
        
        /// <summary>
        /// Parses the registry key path and sees if exists
        /// </summary>
        /// <param name="InPath">The registry path (including hive)</param>
        /// <returns>True if it exists</returns>
        public static bool RegKeyExists(string InPath)
        {
            string strPath = InPath;

            if (strPath.Length == 0) return false;

            string strMainKeyname = strPath;

            int nSlash = strPath.IndexOf("\\");
            if (nSlash > -1)
            {
                strMainKeyname = strPath.Substring(0, nSlash);
                strPath = strPath.Substring(nSlash + 1);
            }
            else
                strPath = "";

            return RegKeyExists(strMainKeyname, strPath);
        }

        public static bool RegKeyExists(string MainKey, string SubKey)
        {
            bool bKeyExists = false;
            RegistryKey reg = RegOpenKey(MainKey, SubKey);

            if (reg != null)
            {
                bKeyExists = true;
                reg.Close();
            }

            return bKeyExists;
        }

        public static RegistryKey RegOpenKey(string MainKey, string SubKey)
        {
            RegistryKey reg = null;

            try
            {
                if (MainKey.ToUpper().CompareTo("HKEY_CLASSES_ROOT") == 0)
                {
                    reg = Registry.ClassesRoot.OpenSubKey(SubKey, true);
                }
                else if (MainKey.ToUpper().CompareTo("HKEY_CURRENT_USER") == 0)
                {
                    reg = Registry.CurrentUser.OpenSubKey(SubKey, true);
                }
                else if (MainKey.ToUpper().CompareTo("HKEY_LOCAL_MACHINE") == 0)
                {
                    reg = Registry.LocalMachine.OpenSubKey(SubKey, true);
                }
                else if (MainKey.ToUpper().CompareTo("HKEY_USERS") == 0)
                {
                    reg = Registry.Users.OpenSubKey(SubKey, true);
                }
                else if (MainKey.ToUpper().CompareTo("HKEY_CURRENT_CONFIG") == 0)
                {
                    reg = Registry.CurrentConfig.OpenSubKey(SubKey, true);
                }
                else
                    return null; // break here
            }
            catch (Exception)
            {
                return null;
            }

            return reg;
        }

        /// <summary>
        /// Starts the process using CreateProcess() API
        /// </summary>
        /// <param name="CmdLine">Command line to execute</param>
        /// <returns>Process ID</returns>
        public static IntPtr CreateProcess(string CmdLine)
        {
            PROCESS_INFORMATION pi;
            STARTUPINFO si = new STARTUPINFO();
            si.wShowWindow = 1;
            if (CreateProcess(null, CmdLine, IntPtr.Zero, IntPtr.Zero, false, 0, IntPtr.Zero, null, ref si, out pi))
                return pi.dwProcessId;
            else
                return IntPtr.Zero;
        }

        /// <summary>
        /// Uses PathGetArgs and PathRemoveArgs API to extract file arguments
        /// </summary>
        /// <param name="CmdLine">Command Line</param>
        /// <param name="FilePath">file path</param>
        /// <param name="Args">arguments</param>
        /// <returns>true or false if an exception was thrown</returns>
        public static bool ExtractArguments(string CmdLine, out string FilePath, out string Args)
        {
            try
            {
                Args = string.Copy(PathGetArgs(CmdLine));

                StringBuilder strCmdLine = new StringBuilder(CmdLine.ToLower().Trim());
                PathRemoveArgs(strCmdLine);
                FilePath = string.Copy(strCmdLine.ToString());

                // Try combining chars
                if (!FileExists(FilePath))
                    return ExtractFileLocation(CmdLine, out FilePath, out Args);
            }
            catch
            {
                return ExtractFileLocation(CmdLine, out FilePath, out Args);
            }

            return true;
        }

        /// <summary>
        /// Parses the file location w/o windows API
        /// </summary>
        /// <param name="Path">Path</param>
        /// <param name="Location">Returns Location</param>
        /// <returns>Returns true if file was located</returns>
        private static bool ExtractFileLocation(string Path, out string Location, out string Arguments)
        {
            string strFilePath = string.Copy(Path.ToLower().Trim());
            bool bRet = false;
            Location = Arguments = "";

            // Remove Quotes
            strFilePath = UnqouteSpaces(strFilePath);

            // Expand variables
            strFilePath = Environment.ExpandEnvironmentVariables(strFilePath);

            // Try to see file exists by combining parts
            StringBuilder strFileFullPath = new StringBuilder(260);
            int nPos = 0;
            foreach (char ch in strFilePath.ToCharArray())
            {
                strFileFullPath = strFileFullPath.Append(ch);
                nPos++;

                // See if part exists
               if (File.Exists(strFileFullPath.ToString()))
               {
                   Location = string.Copy(strFileFullPath.ToString());
                   bRet = true;
                   break;
               }
            }

            if (bRet && nPos > 0)
                Arguments = strFilePath.Remove(0, nPos).Trim();

            return bRet;
        }

        /// <summary>
        /// Resolves path to .lnk shortcut
        /// </summary>
        /// <param name="shortcut">The path to the shortcut</param>
        /// <param name="filepath">Returns the file path</param>
        /// <param name="arguments">Returns the shortcuts arguments</param>
        public static void ResolveShortcut(string shortcut, out string filepath, out string arguments)
        {
            ShellLink link = new ShellLink();
            ((IPersistFile)link).Load(shortcut, STGM_READ);
            // TODO: if I can get hold of the hwnd call resolve first. This handles moved and renamed files.  
            // ((IShellLinkW)link).Resolve(hwnd, 0) 
            StringBuilder path = new StringBuilder(MAX_PATH);
            WIN32_FIND_DATAW data = new WIN32_FIND_DATAW();
            ((IShellLinkW)link).GetPath(path, path.Capacity, out data, 0);

            StringBuilder args = new StringBuilder(MAX_PATH);
            ((IShellLinkW)link).GetArguments(args, args.Capacity);

            filepath = path.ToString();
            arguments = args.ToString();

            return;
        }

        /// <summary>
        /// Creates .lnk shortcut to filename
        /// </summary>
        /// <param name="filename">.lnk shortcut</param>
        /// <param name="path">path for filename</param>
        /// <param name="arguments">arguments for shortcut (can be null)</param>
        /// <returns>True if shortcut was created</returns>
        public static bool CreateShortcut(string filename, string path, string arguments)
        {
            ShellLink link = new ShellLink();
            ((IShellLinkW)link).SetPath(path);
            if (!string.IsNullOrEmpty(arguments))
                ((IShellLinkW)link).SetArguments(arguments);
            ((IPersistFile)link).Save(filename, false);

            return (File.Exists(filename));
        }

        /// <summary>
        /// Converts FILETIME structure to DateTime structure
        /// </summary>
        /// <param name="ft">FILETIME structure</param>
        /// <returns>DateTime structure</returns>
        public static DateTime FileTime2DateTime(FILETIME ft)
        {
            DateTime dt = DateTime.MaxValue;
            long hFT2 = (((long)ft.dwHighDateTime) << 32) + ft.dwLowDateTime;

            try
            {
                dt = DateTime.FromFileTimeUtc(hFT2);
            }
            catch (ArgumentOutOfRangeException)
            {
                dt = DateTime.MaxValue;
            }

            return dt;
        }

        /// <summary>
        /// Converts the size in bytes to a formatted string
        /// </summary>
        /// <param name="Length">Size in bytes</param>
        /// <returns>Formatted String</returns>
        public static string ConvertSizeToString(long Length)
        {
            if (Length < 0)
                return "";

            float nSize;
            string strSizeFmt, strUnit = "";

            if (Length < 1000)             // 1KB
            {
                nSize = Length;
                strUnit = " B";
            }
            else if (Length < 1000000)     // 1MB
            {
                nSize = Length / (float)0x400;
                strUnit = " KB";
            }
            else if (Length < 1000000000)   // 1GB
            {
                nSize = Length / (float)0x100000;
                strUnit = " MB";
            }
            else
            {
                nSize = Length / (float)0x40000000;
                strUnit = " GB";
            }

            if (nSize == (int)nSize)
                strSizeFmt = nSize.ToString("0");
            else if (nSize < 10)
                strSizeFmt = nSize.ToString("0.00");
            else if (nSize < 100)
                strSizeFmt = nSize.ToString("0.0");
            else
                strSizeFmt = nSize.ToString("0");

            return strSizeFmt + strUnit;
        }

        /// <summary>
        /// Calculates size of directory
        /// </summary>
        /// <param name="directory">DirectoryInfo class</param>
        /// <param name="includeSubdirectories">Includes sub directories if true</param>
        /// <returns>Size of directory in bytes</returns>
        public static long CalculateDirectorySize(DirectoryInfo directory, bool includeSubdirectories)
        {
            long totalSize = 0;

            // Examine all contained files.
            FileInfo[] files = directory.GetFiles();
            foreach (FileInfo file in files)
            {
                totalSize += file.Length;
            }

            // Examine all contained directories.
            if (includeSubdirectories)
            {
                DirectoryInfo[] dirs = directory.GetDirectories();
                foreach (DirectoryInfo dir in dirs)
                {
                    totalSize += CalculateDirectorySize(dir, true);
                }
            }

            return totalSize;
        }


        /// <summary>
        /// Returns special folder path specified by CSIDL
        /// </summary>
        /// <param name="CSIDL">CSIDL</param>
        /// <returns>Special folder path</returns>
        public static string GetSpecialFolderPath(int CSIDL)
        {
            StringBuilder path = new StringBuilder(260);

            if (Utils.SHGetSpecialFolderPath(IntPtr.Zero, path, CSIDL, false))
                return string.Copy(path.ToString());

            return "";
        }

        /// <summary>
        /// Checks for the file using the specified path and/or %PATH% variable
        /// </summary>
        /// <param name="FileName">The name of the file for which to search</param>
        /// <param name="Path">The path to be searched for the file (searches %path% variable if null)</param>
        /// <returns>The path containing the file found or null</returns>
        public static string SearchPath(string FileName, string Path)
        {
            StringBuilder strBuffer = new StringBuilder(260);

            if (SearchPath(((!string.IsNullOrEmpty(Path)) ? (Path) : (null)), FileName, null, 260, strBuffer, null) != 0)
                return string.Copy(strBuffer.ToString());

            return "";
        }

        /// <summary>
        /// Removes quotes from the path
        /// </summary>
        /// <param name="Path">Path w/ quotes</param>
        /// <returns>Path w/o quotes</returns>
        private static string UnqouteSpaces(string Path)
        {
            StringBuilder sb = new StringBuilder(Path);

            PathUnquoteSpaces(sb);

            return string.Copy(sb.ToString());
        }

        /// <summary>
        /// Gets the icon path and sees if it exists
        /// </summary>
        /// <param name="IconPath">The icon path</param>
        /// <returns>True if it exists</returns>
        public static bool IconExists(string IconPath)
        {
            string strFileName = string.Copy(IconPath.Trim().ToLower());

            // Remove quotes
            strFileName = UnqouteSpaces(strFileName);

            // Get icon path
            int nSlash = strFileName.IndexOf(',');
            if (nSlash > -1)
            {
                strFileName = strFileName.Substring(0, nSlash);

                return Utils.FileExists(strFileName);
            }
            else
            {
                StringBuilder sb = new StringBuilder(strFileName);
                if (PathParseIconLocation(sb) >= 0)
                    if (sb.Length > 0)
                        return Utils.FileExists(sb.ToString());
            }

            return false;
        }

        /// <summary>
        /// Extracts the large or small icon
        /// </summary>
        /// <param name="Path">Path to icon</param>
        /// <returns>Large or small icon or null</returns>
        public static Icon ExtractIcon(string Path)
        {
            IntPtr largeIcon = IntPtr.Zero;
            IntPtr smallIcon = IntPtr.Zero;

            string strPath = UnqouteSpaces(Path);

            ExtractIconExA(strPath, 0, ref largeIcon, ref smallIcon, 1);

            //Transform the bits into the icon image
            Icon returnIcon = null;
            if (smallIcon != IntPtr.Zero)
                returnIcon = (Icon)Icon.FromHandle(smallIcon).Clone();
            else if (largeIcon != IntPtr.Zero)
                returnIcon = (Icon)Icon.FromHandle(largeIcon).Clone();

            //clean up
            DestroyIcon(smallIcon);
            DestroyIcon(largeIcon);

            return returnIcon;
        }

        enum VDTReturn
        {
            ValidDrive = 0,
            InvalidDrive = 1,
            SkipCheck = 3
        }

        /// <summary>
        /// Sees if path has valid type
        /// </summary>
        /// <param name="path">Path containing drive</param>
        /// <returns>ValidDriveTypeReturn enum</returns>
        private static VDTReturn ValidDriveType(string path)
        {
            StringBuilder sb = new StringBuilder(path);
            if (PathStripToRoot(sb)) 
            {
                DriveType dt = GetDriveType(sb.ToString());

                if (Properties.Settings.Default.bOptionsRemMedia)
                {
                    // Just return true if its on a removable media
                    if (dt == DriveType.Removable ||
                        dt == DriveType.Network ||
                        dt == DriveType.CDRom)
                        return VDTReturn.SkipCheck;
                }

                // Return false for unkown and no root dir
                if (dt == DriveType.NoRootDirectory ||
                    dt == DriveType.Unknown)
                    return VDTReturn.InvalidDrive;
            }

            return VDTReturn.ValidDrive;
        }

        /// <summary>
        /// Sees if the file exists
        /// </summary>
        /// <param name="FilePath">The filename (including path)</param>
        /// <returns>True if it exists or false</returns>
        public static bool FileExists(string FilePath)
        {
            if (!string.IsNullOrEmpty(FilePath))
            {
                string strFileName = string.Copy(FilePath);

                // Remove quotes
                strFileName = UnqouteSpaces(strFileName);

                // Remove environment variables
                strFileName = Environment.ExpandEnvironmentVariables(strFileName);

                // Check for illegal characters
                if (FindAnyIllegalChars(strFileName))
                    return false;

                // Check Drive Type
                VDTReturn ret = ValidDriveType(strFileName);
                if (ret == VDTReturn.InvalidDrive)
                    return false;
                else if (ret == VDTReturn.SkipCheck)
                    return true;

                // Now see if file exists
                if (File.Exists(strFileName))
                    return true;

                if (PathFileExists(strFileName))
                    return true;

                if (SearchPath(strFileName, "") != "")
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Sees if the directory exists
        /// </summary>
        /// <param name="DirPath">The directory</param>
        /// <returns>True if it exists</returns>
        public static bool DirExists(string DirPath)
        {
            string strDirectory = string.Copy(DirPath.Trim().ToLower());

            // Remove quotes
            strDirectory = UnqouteSpaces(strDirectory);

            // Expand enviroment variables
            strDirectory = Environment.ExpandEnvironmentVariables(strDirectory);

            // Check drive type
            VDTReturn ret = ValidDriveType(strDirectory);
            if (ret == VDTReturn.InvalidDrive)
                return false;
            else if (ret == VDTReturn.SkipCheck)
                return true;

            // Check for illegal chars
            if (FindAnyIllegalChars(strDirectory))
                return false;

            // Remove filename.ext and trailing backslash from path
            StringBuilder sb = new StringBuilder(strDirectory);
            if (PathRemoveFileSpec(sb))
                if (Directory.Exists(sb.ToString()))
                    return true;

            if (Directory.Exists(strDirectory))
                return true;

            return false;
        }

        /// <summary>
        /// Parses the path and checks for any illegal characters
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>Returns true if it contains illegal characters</returns>
        private static bool FindAnyIllegalChars(string path)
        {
            // Get directory portion of the path.
            string dirName = path;
            string fullFileName = "";
            int pos = 0;
            if ((pos = path.LastIndexOf(Path.DirectorySeparatorChar)) >= 0)
            {
                dirName = path.Substring(0, pos);

                // Get filename portion of the path.
                if (pos >= 0 && (pos + 1) < path.Length)
                    fullFileName = path.Substring(pos + 1);
            }

            // Find any characters in the directory that are illegal.
            if (dirName.IndexOfAny(Path.GetInvalidPathChars()) != -1) // Found invalid character in directory
                return true;

            // Find any characters in the filename that are illegal.
            if (!string.IsNullOrEmpty(fullFileName))
                if (fullFileName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1) // Found invalid character in filename
                    return true;

            return false;
        }


        /// <summary>
        /// Uses the FindExecutable API to search for the file that opens the specified document
        /// </summary>
        /// <param name="strFilename">The document to search for</param>
        /// <returns>The file that opens the document</returns>
        public static string FindExecutable(string strFilename)
        {
            StringBuilder strResultBuffer = new StringBuilder(1024);

            long nResult = FindExecutableA(strFilename, string.Empty, strResultBuffer);

            if (nResult >= 32)
            {
                return strResultBuffer.ToString();
            }

            return string.Format("Error: ({0})", nResult);
        }

        /// <summary>
        /// Shortens the registry hive path
        /// </summary>
        /// <param name="SubKey">Path containing registry hive (EX: HKEY_CURRENT_USER/...) </param>
        /// <returns>Shortened registry path  (EX: HKCU/...) </returns>
        public static string PrefixRegPath(string SubKey)
        {
            string strSubKey = string.Copy(SubKey);

            if (strSubKey.ToUpper().StartsWith("HKEY_CLASSES_ROOT"))
            {
                strSubKey = strSubKey.Replace("HKEY_CLASSES_ROOT", "HKCR");
            }
            else if (strSubKey.ToUpper().StartsWith("HKEY_CURRENT_USER"))
            {
                strSubKey = strSubKey.Replace("HKEY_CURRENT_USER", "HKCU");
            }
            else if (strSubKey.ToUpper().StartsWith("HKEY_LOCAL_MACHINE"))
            {
                strSubKey = strSubKey.Replace("HKEY_LOCAL_MACHINE", "HKLM");
            }
            else if (strSubKey.ToUpper().StartsWith("HKEY_USERS"))
            {
                strSubKey = strSubKey.Replace("HKEY_USERS", "HKU");
            }
            else if (strSubKey.ToUpper().StartsWith("HKEY_CURRENT_CONFIG"))
            {
                strSubKey = strSubKey.Replace("HKEY_CURRENT_CONFIG", "HKCC");
            }

            return strSubKey;
        }

        /// <summary>
        /// Checks for default program then launches URI
        /// </summary>
        /// <param name="WebAddress">The address to launch</param>
        /// <returns>False if the scheme is not set</returns>
        public static bool LaunchURI(string WebAddress)
        {
            RegistryKey rk = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command");
            string strURL = string.Copy(WebAddress);
            string strBrowserPath, strBrowserArgs;

            if (rk == null || string.IsNullOrEmpty(strURL))
                return false;

            string strBrowserCmd = (string)rk.GetValue("");

            if (!Utils.ExtractArguments(strBrowserCmd, out strBrowserPath, out strBrowserArgs))
                return false;

            strBrowserArgs = strBrowserArgs.Replace("%1", strURL);

            try
            {
                Process.Start(strBrowserPath, strBrowserArgs);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
