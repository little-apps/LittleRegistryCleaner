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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;

namespace Little_Registry_Cleaner
{
    public partial class CrashReporter : Form
    {
        private MemoryStream memoryStream = new MemoryStream();
        private Exception exception;

        public CrashReporter(Exception e)
        {
            InitializeComponent();

            this.exception = e;

            GenerateDialogReport();
        }

        /// <summary>
        /// Fills in text box with exception info
        /// </summary>
        /// <param name="e"></param>
        private void GenerateDialogReport()
        {
            StringBuilder sb = new StringBuilder();

            // Set picturebox to error
            this.pictureBoxErr.Image = SystemIcons.Error.ToBitmap();

            Process proc = Process.GetCurrentProcess();

            // dates and time
            sb.AppendLine(string.Format("Current Date/Time: {0}", DateTime.Now.ToString()));
            sb.AppendLine(string.Format("Exec. Date/Time: {0}", proc.StartTime.ToString()));
            sb.AppendLine(string.Format("Build Date: {0}", Properties.Settings.Default.strBuildTime));
            // os info
            sb.AppendLine(string.Format("OS: {0}", Environment.OSVersion.VersionString));
            sb.AppendLine(string.Format("Language: {0}", Application.CurrentCulture.ToString()));
            // uptime stats
            sb.AppendLine(string.Format("System Uptime: {0} Days {1} Hours {2} Mins {3} Secs", Math.Round((decimal)Environment.TickCount / 86400000), Math.Round((decimal)Environment.TickCount / 3600000 % 24), Math.Round((decimal)Environment.TickCount / 120000 % 60), Math.Round((decimal)Environment.TickCount / 1000 % 60)));
            sb.AppendLine(string.Format("Program Uptime: {0}", proc.TotalProcessorTime.ToString()));
            // process id
            sb.AppendLine(string.Format("PID: {0}", proc.Id));
            // exe name
            sb.AppendLine(string.Format("Executable: {0}", Application.ExecutablePath));
            sb.AppendLine(string.Format("Process Name: {0}", proc.ToString()));
            sb.AppendLine(string.Format("Main Module Name: {0}", proc.MainModule.ModuleName));
            // exe stats
            sb.AppendLine(string.Format("Module Count: {0}", proc.Modules.Count));
            sb.AppendLine(string.Format("Thread Count: {0}", proc.Threads.Count));
            sb.AppendLine(string.Format("Thread ID: {0}", Thread.CurrentThread.ManagedThreadId));
            sb.AppendLine(string.Format("Is Admin: {0}", Permissions.IsUserAdministrator));
            sb.AppendLine(string.Format("Is Debugged: {0}", Debugger.IsAttached));
            // versions
            sb.AppendLine(string.Format("Version: {0}", Application.ProductVersion));
            sb.AppendLine(string.Format("CLR Version: {0}", Environment.Version.ToString()));


            Exception ex = this.exception;
            for (int i = 0; ex != null; ex = ex.InnerException, i++)
            {
                sb.AppendLine();
                sb.AppendLine(string.Format("Type #{0} {1}", i, ex.GetType().ToString()));
                
                foreach (System.Reflection.PropertyInfo propInfo in ex.GetType().GetProperties())
                {
                    string fieldName = string.Format("{0} #{1}", propInfo.Name, i);
                    string fieldValue = string.Format("{0}", propInfo.GetValue(ex, null));

                    // Ignore stack trace + data
                    if (propInfo.Name == "StackTrace" 
                        || propInfo.Name == "Data"
                        || string.IsNullOrEmpty(propInfo.Name)
                        || string.IsNullOrEmpty(fieldValue))
                        continue;

                    sb.AppendLine(string.Format("{0}: {1}", fieldName, fieldValue));
                }

                if (ex.Data != null)
                    foreach (DictionaryEntry de in ex.Data)
                        sb.AppendLine(string.Format("Dictionary Entry #{0}: Key: {1} Value: {2}", i, de.Key, de.Value));
            }

            sb.AppendLine();
            sb.AppendLine("StackTrace:");
            sb.AppendLine(this.exception.StackTrace);

            this.richTextBox1.Text = sb.ToString();

            byte[] b = Encoding.ASCII.GetBytes(sb.ToString());
            memoryStream.Write(b, 0, b.Length);
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (Main.Watcher != null)
            {
                Main.Watcher.Exception(this.exception);
                MessageBox.Show(this, Properties.Resources.crashReporterSuccess, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(this, Properties.Resources.crashReporterFail, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Close();
        }

        private void ErrorDlg_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.checkBoxRestart.Checked)
            {
                Application.Restart();
                Process.GetCurrentProcess().Kill();
            }
        }

        private void buttonDontSend_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
