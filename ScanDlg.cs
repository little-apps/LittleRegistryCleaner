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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using Little_Registry_Cleaner.Scanners;
using Microsoft.Win32;
using Little_Registry_Cleaner.Xml;

namespace Little_Registry_Cleaner
{
    public partial class ScanDlg : Form
    {
        public delegate void UpdateScanSubKeyDelgate(string strSubKey);
        public delegate void UpdateSectionDelegate(string strSection);

        private Utils.Logger loggerScan;

        private Thread threadMain;
        private Thread threadCurrent;

        private int SectionCount = 0;
        private int ItemsScanned = 0;

        public static ScanDlg frmScanDlg;

        public static BadRegKeyArray arrBadRegistryKeys = new BadRegKeyArray();

        public ScanDlg(int nSectionCount)
        {
            InitializeComponent();

            // Set pointer to scandlg variable
            ScanDlg.frmScanDlg = this;

            // Set the section count so it can be accessed later
            this.SectionCount = nSectionCount;
        }

        
        private void ScanDlg_Shown(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;

            // Starts scanning registry on seperate thread
            ThreadPool.QueueUserWorkItem(new WaitCallback(StartScanning));
        }

        /// <summary>
        /// Begins scanning for errors in the registry
        /// </summary>
        private void StartScanning(object stateInfo)
        {
            // Sets threadMain to current thread
            this.threadMain = Thread.CurrentThread;

            // Create log file
            string strLogFile = string.Format("{0}\\{1:yyyy}_{1:MM}_{1:dd}_{1:HH}{1:mm}{1:ss}.txt", Little_Registry_Cleaner.Properties.Settings.Default.strOptionsLogDir, DateTime.Now);

            if (!Directory.Exists(Little_Registry_Cleaner.Properties.Settings.Default.strOptionsLogDir))
                Directory.CreateDirectory(Little_Registry_Cleaner.Properties.Settings.Default.strOptionsLogDir);

            this.loggerScan = new Utils.Logger(strLogFile);

            this.progressBar.Position = 0;
            this.progressBar.PositionMin = 0;
            this.progressBar.PositionMax = this.SectionCount;

            // Begin scanning
            try
            {
                if (Main.bScanStartup)
                {
                    this.UpdateSection("Startup entries");

                    this.threadCurrent = new Thread(new ThreadStart(delegate { new StartUp(); }));
                    this.threadCurrent.Start();
                    this.threadCurrent.Join();

                    this.progressBar.Position++;
                }

                if (Main.bScanSharedDLL)
                {
                    this.UpdateSection("Shared DLLs");

                    this.threadCurrent = new Thread(new ThreadStart(delegate { new DLLs(); }));
                    this.threadCurrent.Start();
                    this.threadCurrent.Join();

                    this.progressBar.Position++;
                }

                if (Main.bScanFonts)
                {
                    this.UpdateSection("Windows Fonts");

                    this.threadCurrent = new Thread(new ThreadStart(delegate { new Fonts(); }));
                    this.threadCurrent.Start();
                    this.threadCurrent.Join();

                    this.progressBar.Position++;
                }

                if (Main.bScanAppInfo)
                {
                    this.UpdateSection("Application info");

                    this.threadCurrent = new Thread(new ThreadStart(delegate { new AppInfo(); }));
                    this.threadCurrent.Start();
                    this.threadCurrent.Join();

                    this.progressBar.Position++;
                }

                if (Main.bScanAppPaths)
                {
                    this.UpdateSection("Program Locations");

                    this.threadCurrent = new Thread(new ThreadStart(delegate { new AppPaths(); }));
                    this.threadCurrent.Start();
                    this.threadCurrent.Join();

                    this.progressBar.Position++;
                }

                if (Main.bScanActivex)
                {
                    this.UpdateSection("ActiveX/COM objects");

                    this.threadCurrent = new Thread(new ThreadStart(delegate { new COMObjects(); }));
                    this.threadCurrent.Start();
                    this.threadCurrent.Join();

                    this.progressBar.Position++;
                }

                if (Main.bScanDrivers)
                {
                    this.UpdateSection("Drivers");

                    this.threadCurrent = new Thread(new ThreadStart(delegate { new Drivers(); }));
                    this.threadCurrent.Start();
                    this.threadCurrent.Join();

                    this.progressBar.Position++;
                }

                if (Main.bScanHelpFiles)
                {
                    this.UpdateSection("Help files");

                    this.threadCurrent = new Thread(new ThreadStart(delegate { new HelpFiles(); }));
                    this.threadCurrent.Start();
                    this.threadCurrent.Join();

                    this.progressBar.Position++;
                }

                if (Main.bScanSounds)
                {
                    this.UpdateSection("Sound events");

                    this.threadCurrent = new Thread(new ThreadStart(delegate { new Sounds(); }));
                    this.threadCurrent.Start();
                    this.threadCurrent.Join();

                    this.progressBar.Position++;
                }

                if (Main.bScanAppSettings)
                {
                    this.UpdateSection("Software settings");

                    this.threadCurrent = new Thread(new ThreadStart(delegate { new AppSettings(); }));
                    this.threadCurrent.Start();
                    this.threadCurrent.Join();

                    this.progressBar.Position++;
                }

                if (Main.bScanHistoryList)
                {
                    this.UpdateSection("History List");

                    this.threadCurrent = new Thread(new ThreadStart(delegate { new HistoryList(); }));
                    this.threadCurrent.Start();
                    this.threadCurrent.Join();

                    this.progressBar.Position++;
                }

                this.DialogResult = DialogResult.OK;
            }
            catch (ThreadAbortException)
            {
                // Scanning was aborted
                this.loggerScan.WriteLine("User aborted scan... Exiting.");
                if (this.threadCurrent.IsAlive)
                    this.threadCurrent.Abort();

                this.DialogResult = DialogResult.Abort;
            }
            finally
            {
                // Finished Scanning
                this.loggerScan.WriteLine("Total Items Scanned: " + this.ItemsScanned.ToString());
                this.loggerScan.WriteLine("Finished Scanning!");
                this.Close();
            }

            return;
        }

        /// <summary>
        /// Stores an invalid registry key to array list
        /// </summary>
        /// <param name="strProblem">Reason its invalid</param>
        /// <param name="strPath">The path to registry key (including registry hive)</param>
        /// <returns>True if it was added</returns>
        public static bool StoreInvalidKey(string Problem, string Path)
        {
            return StoreInvalidKey(Problem, Path, "");
        }

        /// <summary>
        /// Stores an invalid registry key to array list
        /// </summary>
        /// <param name="strProblem">Reason its invalid</param>
        /// <param name="strPath">The path to registry key (including registry hive)</param>
        /// <param name="strValueName">Value name (leave blank if theres none)</param>
        /// <returns>True if it was added</returns>
        public static bool StoreInvalidKey(string Problem, string Path, string ValueName)
        {
            // See if key exists
            if (!xmlRegistry.keyExists(Path))
                return false;  

            if (arrBadRegistryKeys.Add(Problem, Path, ValueName) > 0)
            {
                frmScanDlg.IncrementProblems();
                Utils.Logger.WriteToFile(Utils.Logger.strLogFilePath, "Found invalid registry key. Key Name: \"" + ValueName + "\" Path: \"" + Path + "\" Reason: \"" + Problem + "\"");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks for registry subkey in ignore list
        /// </summary>
        /// <param name="strPath">Registry subkey</param>
        /// <returns>true if it is on the ignore list, otherwise false</returns>
        private static bool IsOnIgnoreList(string strPath)
        {
            if (Properties.Settings.Default.arrayOptionsExcludeList != null)
            {
                for (int i = 0; i < Properties.Settings.Default.arrayOptionsExcludeList.Count; i++)
                {
                    string[] arrayExcludePath = (string[])Properties.Settings.Default.arrayOptionsExcludeList[i];
                    string strExcludePath = string.Format("{0}\\{1}", arrayExcludePath[0], arrayExcludePath[1]);

                    if (string.Compare(strExcludePath, strPath) == 0)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Updates the textbox with the current subkey being scanned
        /// </summary>
        public static void UpdateScanSubKey(string strSubKey)
        {
            try
            {
                if (ScanDlg.frmScanDlg.InvokeRequired)
                {
                    ScanDlg.frmScanDlg.Invoke(new UpdateScanSubKeyDelgate(UpdateScanSubKey), strSubKey);
                    return;
                }

                ScanDlg.frmScanDlg.textBoxSubKey.Text = strSubKey;
                ScanDlg.frmScanDlg.ItemsScanned++;
            }
            catch
            {

            }
        }

        /// <summary>
        /// Updates the dialog with the current section being scanned
        /// </summary>
        public void UpdateSection(string strSectionName)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdateSectionDelegate(UpdateSection), strSectionName);
                return;
            }

            string strText = "Scanning: " + strSectionName;

            this.progressBar.Text = strText;
            this.loggerScan.WriteLine(strText);
        }

        public void IncrementProblems()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(IncrementProblems));
                return;
            }

            this.labelProblems.Text = arrBadRegistryKeys.Count.ToString();
        }

        private void ScanDlg_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult != DialogResult.OK)
            {
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    if (MessageBox.Show(this, "Are you sure?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        e.Cancel = true;
                    else
                        this.threadMain.Abort();
                }
            }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Abort;
            this.Close();
        }
    }
}
