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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.Management; 
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;

namespace Little_Registry_Cleaner
{

    public partial class ErrorDlg : Form
    {
        [DllImport("kernel32.dll")]
        static extern uint GetTickCount();

        private string strErrorLog;
        private bool bExitOnClose = false;

        public ErrorDlg(Exception e)
        {
            InitializeComponent();

            GenerateDialogReport(e);
        }

        public ErrorDlg(Exception e, string strTitle)
        {
            InitializeComponent();

            // Set dialog title to exception descrition
            this.Text = strTitle;

            GenerateDialogReport(e);
        }

        public ErrorDlg(Exception e, bool bExitApplication)
        {
            InitializeComponent();

            this.bExitOnClose = bExitApplication;

            GenerateDialogReport(e);
        }

        /// <summary>
        /// Fills in dialog with system info and exception info
        /// </summary>
        /// <param name="e"></param>
        private void GenerateDialogReport(Exception e)
        {
            // Set picturebox to error
            this.pictureBox1.Image = SystemIcons.Error.ToBitmap();

            // Gets program uptime
            TimeSpan timeSpanProcTime = Process.GetCurrentProcess().TotalProcessorTime;

            // Used to get disk space
            DriveInfo driveInfo = new DriveInfo(Directory.GetDirectoryRoot(Application.ExecutablePath));

            // Get the CPU Name
            string strCPU = "";
            RegistryKey regKey = Registry.LocalMachine.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0");
            if (regKey != null)
            {
                strCPU = (string)regKey.GetValue("ProcessorNameString");
                regKey.Close();
            }

            this.listView1.Items.Add(new ListViewItem(new string[] { "Current Date/Time", DateTime.Now.ToString() }));
            this.listView1.Items.Add(new ListViewItem(new string[] { "Exec. Date/Time", Process.GetCurrentProcess().StartTime.ToString() }));
            this.listView1.Items.Add(new ListViewItem(new string[] { "Build Date", Properties.Settings.Default.strBuildTime }));
            this.listView1.Items.Add(new ListViewItem(new string[] { "Machine Name", Environment.MachineName }));
            this.listView1.Items.Add(new ListViewItem(new string[] { "Username", Environment.UserName }));
            this.listView1.Items.Add(new ListViewItem(new string[] { "OS", Environment.OSVersion.VersionString }));
            this.listView1.Items.Add(new ListViewItem(new string[] { "Language", Application.CurrentInputLanguage.LayoutName }));
            this.listView1.Items.Add(new ListViewItem(new string[] { "System Uptime", string.Format("{0} Days {1} Hours {2} Mins {3} Secs", Math.Round((decimal)GetTickCount() / 86400000), Math.Round((decimal)GetTickCount() / 3600000 % 24), Math.Round((decimal)GetTickCount() / 120000 % 60), Math.Round((decimal)GetTickCount() / 1000 % 60)) }));
            this.listView1.Items.Add(new ListViewItem(new string[] { "Program Uptime", string.Format("{0} hours {1} mins {2} secs", timeSpanProcTime.TotalHours.ToString("0"), timeSpanProcTime.TotalMinutes.ToString("0"), timeSpanProcTime.TotalSeconds.ToString("0")) }));
            this.listView1.Items.Add(new ListViewItem(new string[] { "CPU", strCPU }));
            this.listView1.Items.Add(new ListViewItem(new string[] { "Disk Space", string.Format("{0}GB / {1}GB", ((driveInfo.TotalSize - driveInfo.TotalFreeSpace) / 1024f / 1024f / 1024f).ToString("0.00"), (driveInfo.TotalSize / 1024f / 1024f / 1024f).ToString("0.00")) }));
            this.listView1.Items.Add(new ListViewItem(new string[] { "PID", Process.GetCurrentProcess().Id.ToString() }));
            this.listView1.Items.Add(new ListViewItem(new string[] { "Thread Count", Process.GetCurrentProcess().Threads.Count.ToString() }));
            this.listView1.Items.Add(new ListViewItem(new string[] { "Thread Id", System.Threading.Thread.CurrentThread.ManagedThreadId.ToString() }));
            this.listView1.Items.Add(new ListViewItem(new string[] { "Executable", Application.ExecutablePath }));
            this.listView1.Items.Add(new ListViewItem(new string[] { "Process Name", Process.GetCurrentProcess().ProcessName }));
            this.listView1.Items.Add(new ListViewItem(new string[] { "Version", Application.ProductVersion }));
            this.listView1.Items.Add(new ListViewItem(new string[] { "CLR Version", Environment.Version.ToString() }));

            Exception ex = e;
            for (int i = 0; ex != null; ex = ex.InnerException, i++)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    this.listView2.Items.Add(new ListViewItem(new string[] { "Message #" + i.ToString(), ex.Message }));
                if (!string.IsNullOrEmpty(ex.Source))
                    this.listView2.Items.Add(new ListViewItem(new string[] { "Source #" + i.ToString(), ex.Source }));
                if (!string.IsNullOrEmpty(ex.HelpLink))
                    this.listView2.Items.Add(new ListViewItem(new string[] { "Help Link #" + i.ToString(), ex.HelpLink }));
                if (ex.TargetSite != null)
                    this.listView2.Items.Add(new ListViewItem(new string[] { "Target Site #" + i.ToString(), ex.TargetSite.ToString() }));
                if (ex.Data != null)
                {
                    foreach (DictionaryEntry de in ex.Data)
                    {
                        this.listView2.Items.Add(new ListViewItem(new string[] { "Dictionary Entry #" + i.ToString(), string.Format("Key: {0} Value: {1}", de.Key, de.Value) }));
                    }
                }
            }

            this.listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            this.listView2.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            this.textBoxStackTrace.Text = e.StackTrace;

            this.strErrorLog = string.Format("{0}\\{1:yyyy}_{1:MM}_{1:dd}_{1:HH}{1:mm}{1:ss}.txt", Properties.Settings.Default.strErrorDir, DateTime.Now);
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.textBoxEmail.Text))
            {
                if (!Regex.IsMatch(this.textBoxEmail.Text, @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" + @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" + @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
                {
                    MessageBox.Show(this, "Invalid email address", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.tabControl1.SelectedTab = this.tabPage4;
                    return;
                }
            }

            if (CreateErrorLog())
            {
                bool bReportSent = SendBugReport();

                if (!bReportSent)
                    MessageBox.Show(this, "The bug report could not be sent", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show(this, "Sent bug report successfully", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(this, "The error report could not be created", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            return;
        }

        /// <summary>
        /// Uses PHP to upload bug report and email it
        /// </summary>
        /// <returns>True if it was sent</returns>
        private bool SendBugReport() 
        {
            string fileFormName = "uploadedfile";
            string contenttype = "application/octet-stream";
            string uploadfile = this.strErrorLog;

            string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(Properties.Settings.Default.strBugReportAddr);
            webrequest.CookieContainer = new CookieContainer();
            webrequest.ContentType = "multipart/form-data; boundary=" + boundary;
            webrequest.Method = "POST";


            // Build up the post message header

            StringBuilder sb = new StringBuilder();
            sb.Append("--");
            sb.Append(boundary);
            sb.Append("\r\n");
            sb.Append("Content-Disposition: form-data; name=\"");
            sb.Append(fileFormName);
            sb.Append("\"; filename=\"");
            sb.Append(Path.GetFileName(uploadfile));
            sb.Append("\"");
            sb.Append("\r\n");
            sb.Append("Content-Type: ");
            sb.Append(contenttype);
            sb.Append("\r\n");
            sb.Append("\r\n");

            string postHeader = sb.ToString();
            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(postHeader);

            // Build the trailing boundary string as a byte array
            // ensuring the boundary appears on a line by itself

            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            FileStream fileStream = new FileStream(uploadfile, FileMode.Open, FileAccess.Read);
            long length = postHeaderBytes.Length + fileStream.Length + boundaryBytes.Length;
            webrequest.ContentLength = length;

            try
            {
                Stream requestStream = webrequest.GetRequestStream();

                // Write out our post header
                requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);

                // Write out the file contents
                byte[] buffer = new Byte[checked((uint)Math.Min(4096,
                                         (int)fileStream.Length))];
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                    requestStream.Write(buffer, 0, bytesRead);

                requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
            }
            catch (WebException e)
            {
                if (MessageBox.Show(this, e.Message, "Error", MessageBoxButtons.RetryCancel) == DialogResult.Retry)
                    return SendBugReport();
                else
                    return false;
            }

            // Write out the trailing boundary
            HttpWebResponse response = (HttpWebResponse)webrequest.GetResponse();

            return (response.StatusCode == HttpStatusCode.OK);
        }

        /// <summary>
        /// Creates error log file
        /// </summary>
        /// <returns>True on success</returns>
        private bool CreateErrorLog()
        {
            int i;
            StreamWriter streamErrorLog = null;

            try
            {
                streamErrorLog = File.CreateText(strErrorLog);

                for (i = 0; i < this.listView1.Items.Count; i++)
                {
                    string strDesc = this.listView1.Items[i].SubItems[0].Text;
                    string strValue = this.listView1.Items[i].SubItems[1].Text;

                    streamErrorLog.WriteLine(string.Format("{0}: {1}", strDesc, strValue));
                }

                streamErrorLog.WriteLine();

                for (i = 0; i < this.listView2.Items.Count; i++)
                {
                    string strDesc = this.listView2.Items[i].SubItems[0].Text;
                    string strValue = this.listView2.Items[i].SubItems[1].Text;

                    streamErrorLog.WriteLine(string.Format("{0}: {1}", strDesc, strValue));
                }

                streamErrorLog.WriteLine("Stack Trace:");
                streamErrorLog.WriteLine(this.textBoxStackTrace.Text);

                streamErrorLog.WriteLine();

                if (!string.IsNullOrEmpty(this.textBoxName.Text))
                    streamErrorLog.WriteLine("Name: " + this.textBoxName.Text);
                if (!string.IsNullOrEmpty(this.textBoxEmail.Text))
                    streamErrorLog.WriteLine("Email: " + this.textBoxEmail.Text);
                if (!string.IsNullOrEmpty(this.textBoxInfo.Text))
                {
                    streamErrorLog.WriteLine("Additional Info: ");
                    streamErrorLog.WriteLine(this.textBoxInfo.Text);
                }

                streamErrorLog.Close();

            }
            catch (IOException )
            {
                return false;
            }

            return true;
        }

        private void ErrorDlg_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.bExitOnClose)
            {
                Application.Exit();
                Process.GetCurrentProcess().Kill();
            }
        }
    }
}
