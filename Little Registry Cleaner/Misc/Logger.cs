/*
    Little Registry Cleaner
    Copyright (C) 2008 Little Apps (http://www.little-apps.org/)

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
using System.Windows.Forms;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Little_Registry_Cleaner
{
    /// <summary>
    /// This log class is used for the scanner modules
    /// </summary>
    public class Logger : StreamWriter
    {
        private static string strLogFilePath = "";

        /// <summary>
        /// Contains the path to the current log file
        /// </summary>
        public static string LogFilePath
        {
            get { return strLogFilePath; }
        }

        /// <summary>
        /// Gets whether logging is enabled
        /// </summary>
        public static bool IsEnabled
        {
            get { return Properties.Settings.Default.bOptionsLog; }
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII;  }
        }

        public Logger(string fileName) : base(fileName)
        {
            if (IsEnabled)
            {
                try
                {
                    // Set log file path
                    Logger.strLogFilePath = fileName;

                    // Flush the buffers automatically
                    this.AutoFlush = true;

                    // Create log directory if it doesnt exist
                    if (!Directory.Exists(Little_Registry_Cleaner.Properties.Settings.Default.strOptionsLogDir))
                        Directory.CreateDirectory(Little_Registry_Cleaner.Properties.Settings.Default.strOptionsLogDir);

                    lock (this.BaseStream)
                    {
                        // Writes header to log file
                        this.WriteLine("Little Registry Cleaner " + Application.ProductVersion);
                        this.WriteLine("Website: http://www.little-apps.org/little-registry-cleaner/");
                        this.WriteLine(Environment.OSVersion.ToString());
                        this.WriteLine();
                    }
                }
                catch (Exception ex)
                { 
                    Debug.WriteLine(ex);
                }
            }
        }

        /// <summary>
        /// Writes the specified string to the file
        /// </summary>
        /// <param name="filePath">The file to write to</param>
        /// <param name="value">The string to write</param>
        public static void WriteToFile(string filePath, string value)
        {
            if (IsEnabled)
            {
                try
                {

                    StreamWriter streamWriter = File.AppendText(filePath);

                    lock (streamWriter)
                    {
                        streamWriter.Write(value);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        /// <summary>
        /// Moves the temp file to the log directory and opens it with the default viewer
        /// </summary>
        /// <param name="bCopyOnly">Only copies the log file and doesn't display it</param>
        /// <returns>True if the file is displayed</returns>
        public bool DisplayLogFile(bool bCopyOnly)
        {
            if (IsEnabled)
            {
                string strNewFileName = string.Format("{0}\\{1:yyyy}_{1:MM}_{1:dd}_{1:HH}{1:mm}{1:ss}.txt", Little_Registry_Cleaner.Properties.Settings.Default.strOptionsLogDir, DateTime.Now);

                try
                {
                    lock (this.BaseStream)
                    {
                        if (!File.Exists(strLogFilePath))
                            return false;

                        File.Copy(Logger.strLogFilePath, strNewFileName);

                        if (Properties.Settings.Default.bOptionsShowLog)
                        {
                            ProcessStartInfo startInfo = new ProcessStartInfo("NOTEPAD.EXE", strNewFileName);
                            startInfo.ErrorDialog = true;
                            Process.Start(startInfo);
                        }

                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the path to the filename
        /// </summary>
        /// <returns>Log path</returns>
        public override string ToString()
        {
            return string.Copy(Logger.strLogFilePath);
        }
    }
}
