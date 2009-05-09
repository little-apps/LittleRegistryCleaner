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
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.IO;

namespace Little_Registry_Cleaner
{
    public class Logger
    {
        private static string strLogFilePath = "";

        /// <summary>
        /// Contains the path to the current log file
        /// </summary>
        public static string LogFilePath
        {
            get { return strLogFilePath; }
        }

        public static bool IsEnabled
        {
            get { return Properties.Settings.Default.bOptionsLog; }
        }

        public Logger()
        {
            if (IsEnabled)
            {
                try
                {
                    // Generate log file path
                    Logger.strLogFilePath = Path.GetTempFileName();

                    // Create log directory if it doesnt exist
                    if (!Directory.Exists(Little_Registry_Cleaner.Properties.Settings.Default.strOptionsLogDir))
                        Directory.CreateDirectory(Little_Registry_Cleaner.Properties.Settings.Default.strOptionsLogDir);

                    // Remove file if it already exists
                    if (File.Exists(Logger.strLogFilePath))
                        File.Delete(Logger.strLogFilePath);

                    // Create log file + write header
                    StreamWriter stream = File.CreateText(Logger.strLogFilePath);

                    if (stream != null)
                    {
                        stream.WriteLine("Little Registry Cleaner (" + DateTime.Now.ToString() + ")");
                        stream.WriteLine("Website: http://sourceforge.net/projects/littlecleaner");
                        stream.WriteLine("Version: " + Application.ProductVersion);
                        stream.WriteLine("----------------");

                        stream.Close();
                        stream = null;
                    }
                }
                catch (Exception e)
                { 
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Writes scanning info to log file with the date + time
        /// </summary>
        /// <param name="Line">The string to write</param>
        public void WriteLine(string Line)
        {
            if (IsEnabled)
            {
                try
                {
                    StreamWriter sw = File.AppendText(Logger.strLogFilePath);

                    if (sw != null)
                    {
                        sw.WriteLine("{0}: {1}", DateTime.Now.ToLongTimeString(), Line);
                        sw.Close();
                        sw = null;
                    }
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
        /// <param name="FilePath">The file to write to</param>
        /// <param name="Line">The string to write (w/ date + time)</param>
        public static void WriteToFile(string FilePath, string Line)
        {
            if (IsEnabled)
            {
                try
                {
                    StreamWriter sw = File.AppendText(FilePath);

                    if (sw != null)
                    {
                        sw.WriteLine("{0}: {1}", DateTime.Now.ToLongTimeString(), Line);
                        sw.Close();
                        sw = null;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// Moves the temp file to the log directory and opens it with the default viewer
        /// </summary>
        public void DisplayLogFile()
        {
            if (IsEnabled)
            {
                string strNewFileName = string.Format("{0}\\{1:yyyy}_{1:MM}_{1:dd}_{1:HH}{1:mm}{1:ss}.txt", Little_Registry_Cleaner.Properties.Settings.Default.strOptionsLogDir, DateTime.Now);

                File.Copy(Logger.strLogFilePath, strNewFileName);

                if (Properties.Settings.Default.bOptionsShowLog)
                    System.Diagnostics.Process.Start(strNewFileName);
            }
        }
    }
}
