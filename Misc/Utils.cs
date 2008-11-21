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
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        // Used for SHGetSpecialFolderPath
        public const int CSIDL_STARTUP = 0x0007; // All Users\Startup
        public const int CSIDL_COMMON_STARTUP = 0x0018; // Common Users\Startup

        [DllImport("shell32.dll")] public static extern bool SHGetSpecialFolderPath(IntPtr hwndOwner, [Out] StringBuilder lpszPath, int nFolder, bool fCreate);
        [DllImport("shell32.dll", EntryPoint = "FindExecutable")] public static extern long FindExecutableA(string lpFile, string lpDirectory, StringBuilder lpResult);
        [DllImport("shell32.dll")] public static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);
        
         
        [DllImport("Shlwapi.dll", SetLastError = true, CharSet = CharSet.Auto)] public static extern string PathGetArgs(string path);
        [DllImport("Shlwapi.dll", SetLastError = true, CharSet = CharSet.Auto)] public static extern void PathRemoveArgs([In, Out] StringBuilder path);
        [DllImport("Shlwapi.dll", SetLastError = true, CharSet = CharSet.Auto)] public static extern int PathParseIconLocation([In, Out] StringBuilder path);
        [DllImport("Shlwapi.dll", SetLastError = true, CharSet = CharSet.Auto)] public static extern bool PathFileExists(string path);

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
        /// Starts the process using CreateProcess() API
        /// </summary>
        /// <param name="strExePath">The path to the executable</param>
        /// <returns>Process ID</returns>
        public static IntPtr CreateProcess(string strExePath)
        {
            PROCESS_INFORMATION pi;
            STARTUPINFO si = new STARTUPINFO();
            si.wShowWindow = 1;
            if (CreateProcess(null, strExePath, IntPtr.Zero, IntPtr.Zero,
                false, 0, IntPtr.Zero, null, ref si, out pi))
                return pi.dwProcessId;
            else
                return IntPtr.Zero;
        }

        /// <summary>
        /// Uses PathGetArgs and PathRemoveArgs API to extract file arguments
        /// </summary>
        /// <param name="strPath">file path including arguments</param>
        /// <param name="strFile">file path</param>
        /// <param name="strArgs">arguments</param>
        /// <returns>true or false if an exception was thrown</returns>
        public static bool ExtractArguments(string strPath, out string strFile, out string strArgs)
        {
            try
            {
                StringBuilder path = new StringBuilder(strPath);
                strArgs = PathGetArgs(strPath);
                PathRemoveArgs(path);
                strFile = path.ToString();
            }
            catch (Exception)
            {
                strFile = strArgs = "";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Extracts icon from path and returns Icon
        /// </summary>
        /// <param name="path">The full path in form "filename,index"</param>
        /// <returns>Returns Icon</returns>
        public static Icon ParseIconPath(string path)
        {
            StringBuilder sb = new StringBuilder(path);

            int nIndex = PathParseIconLocation(sb);

            if (sb.Length > 0)
            {
                IntPtr hIcon = ExtractIcon(IntPtr.Zero, sb.ToString(), nIndex);

                Icon ico = (Icon)Icon.FromHandle(hIcon).Clone();

                return ico;
            }
            else
                return null;
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
        /// <param name="nCSIDL">CSIDL</param>
        /// <returns>Special folder path</returns>
        public static string GetSpecialFolderPath(int nCSIDL)
        {
            StringBuilder path = new StringBuilder(260);

            if (Utils.SHGetSpecialFolderPath(IntPtr.Zero, path, nCSIDL, false))
                return path.ToString();
            return "";
        }

        /// <summary>
        /// Checks for the file in %PATH% variable
        /// </summary>
        /// <param name="strFilePath">The file path</param>
        /// <returns>True if it exists</returns>
        public static bool SearchPath(string strFilePath)
        {
            // Search for file in %path% variable
            StringBuilder strBuffer = new StringBuilder(260);

            if (SearchPath(null, strFilePath, null, 260, strBuffer, null) != 0)
                return true;

            return false;
        }

        public static bool SearchPath(string strFile, string strPath)
        {
            StringBuilder strBuffer = new StringBuilder(260);

            if (SearchPath(strPath, strFile, null, 260, strBuffer, null) != 0)
                return true;

            return false;
        }

        /// <summary>
        /// Gets the icon path and sees if it exists
        /// </summary>
        /// <param name="strDefaultIcon">The icon path</param>
        /// <returns>True if it exists</returns>
        public static bool IconExists(string strIconPath)
        {
            string strBuffer = strIconPath.Trim().ToLower();

            // Remove quotes
            if (strBuffer[0] == '"')
            {
                int i, iQouteLoc = 0, iQoutes = 1;
                for (i = 0; (i < strBuffer.Length) && (iQoutes <= 2); i++)
                {
                    if (strBuffer[i] == '"')
                    {
                        strBuffer = strBuffer.Remove(i, 1);
                        iQouteLoc = i;
                        iQoutes++;
                    }
                }
            }

            // Get icon path
            int nSlash = strBuffer.IndexOf(',');
            if (nSlash > -1)
                strBuffer = strBuffer.Substring(0, nSlash);

            if (Utils.FileExists(strBuffer))
                return true;

            return false;
        }

        /// <summary>
        /// Sees if the file exists
        /// </summary>
        /// <param name="strFileName">The filename</param>
        /// <returns>True if it exists, false if it contains illegal characters</returns>
        public static bool FileExists(string strPath)
        {
            string strFileName = strPath;

            if (string.IsNullOrEmpty(strPath))
                return false;

            // Remove quotes
            if (strFileName[0] == '"')
            {
                int i, iQouteLoc = 0, iQoutes = 1;
                for (i = 0; (i < strFileName.Length) && (iQoutes <= 2); i++)
                {
                    if (strFileName[i] == '"')
                    {
                        strFileName = strFileName.Remove(i, 1);
                        iQouteLoc = i;
                        iQoutes++;
                    }
                }
            }

            strFileName = Environment.ExpandEnvironmentVariables(strFileName);

            if (FindAnyIllegalChars(strFileName))
                return false;

            if (Path.IsPathRooted(strFileName))
            {
                DriveInfo driveInfo = new DriveInfo(Path.GetPathRoot(strFileName));

                if (Properties.Settings.Default.bOptionsRemDrives)
                    // Just return true if its on a removable drive
                    if (driveInfo.DriveType == DriveType.Removable)
                        return true;
            }

            if (File.Exists(strFileName))
                return true;

            if (PathFileExists(strFileName))
                return true;

            if (Utils.SearchPath(strFileName))
                return true;

            return false;
        }

        /// <summary>
        /// Sees if the directory exists
        /// </summary>
        /// <param name="strPath">The directory</param>
        /// <returns>True if it exists</returns>
        public static bool DirExists(string strPath)
        {
            string strDirectory = strPath.Trim().ToLower();
            int pos;

            // Remove quotes
            if (strDirectory[0] == '"')
            {
                int i, iQouteLoc = 0, iQoutes = 1;
                for (i = 0; (i < strDirectory.Length) && (iQoutes <= 2); i++)
                {
                    if (strDirectory[i] == '"')
                    {
                        strDirectory = strDirectory.Remove(i, 1);
                        iQouteLoc = i;
                        iQoutes++;
                    }
                }
            }

            strDirectory = Environment.ExpandEnvironmentVariables(strDirectory);

            if (FindAnyIllegalChars(strDirectory))
                return false;

            // Remove filename.ext from strDirectory
            if ((pos = strDirectory.LastIndexOf(Path.DirectorySeparatorChar)) >= 0)
            {
                string strDirName = strDirectory.Substring(0, pos);

                if (Directory.Exists(strDirName))
                    return true;
            }

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
    }

    #region Logger
    public class Logger
    {
        /// <summary>
        /// Contains the path to the current log file
        /// </summary>
        public static string strLogFilePath = "";

        private bool bEnabled = false;

        public Logger(string strLogPath)
        {
            Logger.strLogFilePath = strLogPath;

            this.bEnabled = Properties.Settings.Default.bOptionsLog;

            if (this.bEnabled)
            {
                // Create log file
                try
                {
                    using (StreamWriter stream = File.CreateText(Logger.strLogFilePath))
                    {
                        if (stream != null)
                        {
                            stream.WriteLine("Little Registry Cleaner (" + DateTime.Now.ToString() + ")");
                            stream.WriteLine("Website: http://sourceforge.net/projects/littlecleaner");
                            stream.WriteLine("Version: " + Application.ProductVersion);
                            stream.WriteLine("----------------");
                            stream.WriteLine("{0}: Starting scan...", DateTime.Now.ToLongTimeString());
                            stream.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }

        

        /// <summary>
        /// Writes scanning info to log file with the date + time
        /// </summary>
        /// <param name="strLine">The string to write</param>
        public void WriteLine(string strLine)
        {
            if (this.bEnabled)
            {
                try
                {
                    using (StreamWriter stream = File.AppendText(Logger.strLogFilePath))
                        if (stream != null)
                            stream.WriteLine("{0}: {1}", DateTime.Now.ToLongTimeString(), strLine);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// Writes the specified string to the file
        /// </summary>
        /// <param name="strLogFilePath">The file to write to</param>
        /// <param name="strLine">The string to write (w/ date + time)</param>
        public static void WriteToFile(string strLogFilePath, string strLine)
        {
            if (Properties.Settings.Default.bOptionsLog)
            {
                try
                {
                    using (StreamWriter stream = File.AppendText(strLogFilePath))
                        if (stream != null)
                            stream.WriteLine("{0}: {1}", DateTime.Now.ToLongTimeString(), strLine);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }
    }
    #endregion
    #region ListViewItemComparer

    public class ListViewItemComparer : IComparer
    {
        private int col;
        private SortOrder order;
        public ListViewItemComparer()
        {
            col = 0;
            order = SortOrder.Ascending;
        }
        public ListViewItemComparer(int column, SortOrder order)
        {
            col = column;
            this.order = order;
        }
        public int Compare(object x, object y)
        {
            int returnVal = -1;
            returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text,
                                    ((ListViewItem)y).SubItems[col].Text);
            // Determine whether the sort order is descending.
            if (order == SortOrder.Descending)
                // Invert the value returned by String.Compare.
                returnVal *= -1;
            return returnVal;
        }
    }
    #endregion
}
