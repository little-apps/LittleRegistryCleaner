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
        public const int CSIDL_PROGRAMS = 0x0002;   // All Users\Start Menu\Programs
        public const int CSIDL_COMMON_PROGRAMS = 0x0017;   // Start Menu\Programs

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
        #endregion
        #region Interop (IShellLink and IPersistFile)
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
        /// Returns true if the OS is 64 bit
        /// </summary>
        public static bool Is64BitOS
        {
            get { return (IntPtr.Size == 8); }
        }

        #region Registry Functions
        /// <summary>
        /// Parses a registry key path and outputs the base and subkey to strings
        /// </summary>
        /// <param name="inPath">Registry key path</param>
        /// <param name="baseKey">Base Key (Hive name)</param>
        /// <param name="subKey">Sub Key Path</param>
        /// <returns>True if the path was parsed successfully</returns>
        public static bool ParseRegKeyPath(string inPath, out string baseKey, out string subKey)
        {
            baseKey = subKey = "";

            if (string.IsNullOrEmpty(inPath))
                return false;

            string strMainKeyname = inPath;

            try
            {
                int nSlash = strMainKeyname.IndexOf("\\");
                if (nSlash > -1)
                {
                    baseKey = strMainKeyname.Substring(0, nSlash);
                    subKey = strMainKeyname.Substring(nSlash + 1);
                }
                else if (strMainKeyname.ToUpper().StartsWith("HKEY"))
                    baseKey = strMainKeyname;
                else
                    return false;
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Parses the registry key path and sees if exists
        /// </summary>
        /// <param name="InPath">The registry path (including hive)</param>
        /// <returns>True if it exists</returns>
        public static bool RegKeyExists(string inPath)
        {
            string strBaseKey, strSubKey;

            if (!ParseRegKeyPath(inPath, out strBaseKey, out strSubKey))
                return false;

            return RegKeyExists(strBaseKey, strSubKey);
        }

        public static bool RegKeyExists(string mainKey, string subKey)
        {
            bool bKeyExists = false;
            RegistryKey reg = RegOpenKey(mainKey, subKey);

            if (reg != null)
            {
                bKeyExists = true;
                reg.Close();
            }

            return bKeyExists;
        }

        public static bool ValueNameExists(string mainKey, string subKey, string valueName)
        {
            bool bKeyExists = false;
            RegistryKey reg = RegOpenKey(mainKey, subKey);

            try
            {
                if (reg != null)
                {
                    if (reg.GetValue(valueName) != null)
                        bKeyExists = true;
                    reg.Close();
                }
            }
            catch
            {
                return false;
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
        /// Gets the value kind and converts it accordingly
        /// </summary>
        /// <returns>Registry value formatted to a string</returns>
        public static string RegConvertXValueToString(RegistryKey regKey, string valueName)
        {
            string strRet = "";

            if (regKey == null)
                return strRet;

            try
            {

                switch (regKey.GetValueKind(valueName))
                {
                    case RegistryValueKind.MultiString:
                        {
                            string strValue = "";
                            string[] strValues = (string[])regKey.GetValue(valueName);

                            for (int i = 0; i < strValues.Length; i++)
                            {
                                if (i != 0)
                                    strValue = string.Concat(strValue, ",");

                                strValue = string.Format("{0} {1}", strValue, strValues[i]);
                            }

                            strRet = string.Copy(strValue);

                            break;
                        }
                    case RegistryValueKind.Binary:
                        {
                            string strValue = "";

                            foreach (byte b in (byte[])regKey.GetValue(valueName))
                                strValue = string.Format("{0} {1:X2}", strValue, b);

                            strRet = string.Copy(strValue);

                            break;
                        }
                    case RegistryValueKind.DWord:
                    case RegistryValueKind.QWord:
                        {
                            strRet = string.Format("0x{0:X} ({0:D})", regKey.GetValue(valueName));
                            break;
                        }
                    default:
                        {
                            strRet = string.Format("{0}", regKey.GetValue(valueName));
                            break;
                        }

                }
            }
            catch
            {
                return "";
            }

            return strRet;
        }
        #endregion

        /// <summary>
        /// Uses PathGetArgs and PathRemoveArgs API to extract file arguments
        /// </summary>
        /// <param name="cmdLine">Command Line</param>
        /// <param name="filePath">file path</param>
        /// <param name="fileArgs">arguments</param>
        /// <exception cref="ArgumentNullException">Thrown when cmdLine is null or empty</exception>
        /// <returns>False if the path doesnt exist</returns>
        public static bool ExtractArguments(string cmdLine, out string filePath, out string fileArgs)
        {
            StringBuilder strCmdLine = new StringBuilder(cmdLine.ToLower().Trim());

            filePath = fileArgs = "";

            if (strCmdLine.Length <= 0)
                throw new ArgumentNullException("cmdLine");

            fileArgs = string.Copy(PathGetArgs(strCmdLine.ToString()));

            PathRemoveArgs(strCmdLine);

            filePath = string.Copy(strCmdLine.ToString());

            if (!string.IsNullOrEmpty(filePath))
                if (Utils.FileExists(filePath))
                    return true;

            return false;
        }

        /// <summary>
        /// Parses the file location w/o windows API
        /// </summary>
        /// <param name="cmdLine">Command Line</param>
        /// <param name="filePath">file path</param>
        /// <param name="fileArgs">arguments</param>
        /// <exception cref="ArgumentNullException">Thrown when cmdLine is null or empty</exception>
        /// <returns>Returns true if file was located</returns>
        public static bool ExtractArguments2(string cmdLine, out string filePath, out string fileArgs)
        {
            string strCmdLine = string.Copy(cmdLine.ToLower().Trim());
            bool bRet = false;

            filePath = fileArgs = "";

            if (string.IsNullOrEmpty(strCmdLine))
                throw new ArgumentNullException(cmdLine);

            // Remove Quotes
            strCmdLine = UnqouteSpaces(strCmdLine);

            // Expand variables
            strCmdLine = Environment.ExpandEnvironmentVariables(strCmdLine);

            // Try to see file exists by combining parts
            StringBuilder strFileFullPath = new StringBuilder(260);
            int nPos = 0;
            foreach (char ch in strCmdLine.ToCharArray())
            {
                strFileFullPath = strFileFullPath.Append(ch);
                nPos++;

                // See if part exists
               if (File.Exists(strFileFullPath.ToString()))
               {
                   filePath = string.Copy(strFileFullPath.ToString());
                   bRet = true;
                   break;
               }
            }

            if (bRet && nPos > 0)
                fileArgs = strCmdLine.Remove(0, nPos).Trim();

            return bRet;
        }

        /// <summary>
        /// Resolves path to .lnk shortcut
        /// </summary>
        /// <param name="shortcut">The path to the shortcut</param>
        /// <param name="filepath">Returns the file path</param>
        /// <param name="arguments">Returns the shortcuts arguments</param>
        /// <returns>Returns false if the filepath doesnt exist</returns>
        public static bool ResolveShortcut(string shortcut, out string filepath, out string arguments)
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

            if (!Utils.FileExists(filepath))
                return false;

            return true;
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
        public static DateTime FileTime2DateTime(System.Runtime.InteropServices.ComTypes.FILETIME ft)
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

        public static bool SearchPath(string fileName)
        {
            string retPath = "";

            return SearchPath(fileName, null, out retPath);
        }

        public static bool SearchPath(string fileName, string Path)
        {
            string retPath = "";

            return SearchPath(fileName, Path, out retPath);
        }

        /// <summary>
        /// Checks for the file using the specified path and/or %PATH% variable
        /// </summary>
        /// <param name="fileName">The name of the file for which to search</param>
        /// <param name="Path">The path to be searched for the file (searches %path% variable if null)</param>
        /// <param name="retPath">The path containing the file</param>
        /// <returns>True if it was found</returns>
        public static bool SearchPath(string fileName, string Path, out string retPath)
        {
            StringBuilder strBuffer = new StringBuilder(260);

            int ret = SearchPath(((!string.IsNullOrEmpty(Path)) ? (Path) : (null)), fileName, null, 260, strBuffer, null);

            if (ret != 0)
            {
                retPath = strBuffer.ToString();
                return true;
            }
            else
                retPath = "";

            return false;
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

            // Remove starting @
            if (strFileName.StartsWith("@"))
                strFileName = strFileName.Substring(1);

            // Return true if %1
            if (strFileName == "%1")
                return true;

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
                    if (!string.IsNullOrEmpty(sb.ToString()))
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
        /// <remarks>Always use this to check for files in the scanners!</remarks>
        /// <param name="filePath">The filename (including path)</param>
        /// <returns>
        /// True if it exists or if the path should be skipped. Otherwise, false if the file path is empty or doesnt exist
        /// </returns>
        public static bool FileExists(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            string strFileName = string.Copy(filePath.Trim().ToLower());

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

            // See if it is on exclude list
            if (ScanDlg.IsOnIgnoreList(strFileName))
                return true;

            // Now see if file exists
            if (File.Exists(strFileName))
                return true;

            if (PathFileExists(strFileName))
                return true;

            if (SearchPath(strFileName))
                return true;

            return false;
        }

        /// <summary>
        /// Sees if the directory exists
        /// </summary>
        /// <remarks>Always use this to check for directories in the scanners!</remarks>
        /// <param name="dirPath">The directory</param>
        /// <returns>True if it exists or if the path should be skipped. Otherwise, false if the file path is empty or doesnt exist</returns>
        public static bool DirExists(string dirPath)
        {
            if (string.IsNullOrEmpty(dirPath))
                return false;

            string strDirectory = string.Copy(dirPath.Trim().ToLower());

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

            // See if it is on the exclude list
            if (ScanDlg.IsOnIgnoreList(strDirectory))
                return true;

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
        public static void LaunchURI(string WebAddress)
        {
            Help.ShowHelp(Form.ActiveForm, string.Copy(WebAddress));
        }

        /// <summary>
        /// Converts the string representation to its equivalent GUID
        /// </summary>
        /// <param name="s">String containing the GUID to be converted</param>
        /// <param name="guid">If conversion is sucessful, this parameter is the GUID value of the string. Otherwise, it is empty.</param>
        /// <returns>True if the conversion succeeded</returns>
        public static bool TryParseGuid(string s, out Guid guid)
        {
            guid = Guid.Empty;

            try
            {
                if (string.IsNullOrEmpty(s))
                    return false;

                if (!Regex.IsMatch(s, @"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$"))
                    return false;

                guid = new Guid(s);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the string from the Resources.resx file
        /// </summary>
        /// <param name="name">name of the string</param>
        /// <returns>string</returns>
        public static string GetLocalizedResourceString(string name)
        {
            System.Resources.ResourceManager global = new System.Resources.ResourceManager("Little_Registry_Cleaner.Properties.Resources", System.Reflection.Assembly.GetExecutingAssembly());

            return global.GetString(name, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }
    }
}
