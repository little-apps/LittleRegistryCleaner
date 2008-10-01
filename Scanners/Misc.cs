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

namespace Little_Registry_Cleaner
{
    public class Logger
    {
        private string strLogFilePath = "";
        private bool bEnabled = false;

        public Logger()
        {
            string strLogPath = Little_Registry_Cleaner.Properties.Settings.Default.strOptionsLogDir;
            string strTextFile = string.Format("{0}\\{1:yyyy}_{1:MM}_{1:dd}_{1:HH}{1:mm}{1:ss}.txt", strLogPath, DateTime.Now);

            this.bEnabled = Properties.Settings.Default.bOptionsLog;

            if (this.bEnabled)
            {
                this.strLogFilePath = strTextFile;

                // Create directory if it doesnt exist
                if (!Directory.Exists(strLogPath))
                    Directory.CreateDirectory(strLogPath);

                // Create log file
                try
                {
                    using (StreamWriter stream = File.CreateText(this.strLogFilePath))
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
                    using (StreamWriter stream = File.AppendText(this.strLogFilePath))
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
