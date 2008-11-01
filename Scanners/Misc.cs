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
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

namespace Little_Registry_Cleaner
{
    public class Misc
    {
        [DllImport("kernel32.dll")]
        public static extern int SearchPath(string strPath, string strFileName, string strExtension, uint nBufferLength, StringBuilder strBuffer, string strFilePart);

        [DllImport("shell32.dll", EntryPoint = "FindExecutable")]
        public static extern long FindExecutableA(string lpFile, string lpDirectory, StringBuilder lpResult);

        /// <summary>
        /// Checks for the file in %PATH% variable
        /// </summary>
        /// <param name="strFilePath">The file path</param>
        /// <returns>True if it exists</returns>
        public static bool SearchFilePath(string strFilePath)
        {
            // Search for file in %path% variable
            StringBuilder strBuffer = new StringBuilder(260);

            if (SearchPath(null, strFilePath, null, 260, strBuffer, null) != 0)
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

            if (Misc.FileExists(strBuffer))
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
            string strFileName = strPath.Trim().ToLower();

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

            if (Misc.SearchFilePath(strFileName))
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


}
