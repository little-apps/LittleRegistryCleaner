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

        private Thread threadMain;
        private Thread threadCurrent;

        private int SectionCount = 0;
        private int ItemsScanned = 0;

        private static ScanDlg self;

        public static BadRegKeyArray arrBadRegistryKeys = new BadRegKeyArray();

        private string strCurrentSection = "";

        /// <summary>
        /// Returns current section name
        /// </summary>
        public static string CurrentSection
        {
            get { return self.strCurrentSection; }
        }

        public ScanDlg(int nSectionCount)
        {
            InitializeComponent();

            // Set pointer to scandlg variable
            ScanDlg.self = this;

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

            this.progressBar.Position = 0;
            this.progressBar.PositionMin = 0;
            this.progressBar.PositionMax = this.SectionCount;

            // Begin scanning
            try
            {
                Main.Logger.WriteLine("Starting scan...");

                if (Main.bScanStartup)
                {
                    this.UpdateSection("Startup entries");

                    Main.Logger.WriteLine("Starting to scan startup entries");
                    this.threadCurrent = new Thread(new ThreadStart(StartUp.Scan));
                    this.threadCurrent.Start();
                    this.threadCurrent.Join();
                    Main.Logger.WriteLine("Scanning for startup entries finished");

                    this.progressBar.Position++;
                }

                if (Main.bScanSharedDLL)
                {
                    this.UpdateSection("Shared DLLs");

                    Main.Logger.WriteLine("Starting to scan shared dlls");
                    this.threadCurrent = new Thread(new ThreadStart(DLLs.Scan));
                    this.threadCurrent.Start();
                    this.threadCurrent.Join();
                    Main.Logger.WriteLine("Scanning for shared dlls finished");

                    this.progressBar.Position++;
                }

                if (Main.bScanFonts)
                {
                    this.UpdateSection("Windows Fonts");

                    Main.Logger.WriteLine("Starting to scan windows fonts");
                    this.threadCurrent = new Thread(new ThreadStart(Fonts.Scan));
                    this.threadCurrent.Start();
                    this.threadCurrent.Join();
                    Main.Logger.WriteLine("Scanning for windows fonts finished");

                    this.progressBar.Position++;
                }

                if (Main.bScanAppInfo)
                {
                    this.UpdateSection("Application info");

                    Main.Logger.WriteLine("Starting to scan application info");
                    this.threadCurrent = new Thread(new ThreadStart(AppInfo.Scan));
                    this.threadCurrent.Start();
                    this.threadCurrent.Join();
                    Main.Logger.WriteLine("Scanning for application info finished");

                    this.progressBar.Position++;
                }

                if (Main.bScanAppPaths)
                {
                    this.UpdateSection("Program Locations");

                    Main.Logger.WriteLine("Starting to scan program locations");
                    this.threadCurrent = new Thread(new ThreadStart(AppPaths.Scan));
                    this.threadCurrent.Start();
                    this.threadCurrent.Join();
                    Main.Logger.WriteLine("Scanning for program locations finished");

                    this.progressBar.Position++;
                }

                if (Main.bScanActivex)
                {
                    this.UpdateSection("ActiveX/COM objects");

                    Main.Logger.WriteLine("Starting to scan ActiveX/COM objects");
                    this.threadCurrent = new Thread(new ThreadStart(COMObjects.Scan));
                    this.threadCurrent.Start();
                    this.threadCurrent.Join();
                    Main.Logger.WriteLine("Scanning for ActiveX/COM objects finished");

                    this.progressBar.Position++;
                }

                if (Main.bScanDrivers)
                {
                    this.UpdateSection("Drivers");

                    Main.Logger.WriteLine("Starting to scan drivers");
                    this.threadCurrent = new Thread(new ThreadStart(Drivers.Scan));
                    this.threadCurrent.Start();
                    this.threadCurrent.Join();
                    Main.Logger.WriteLine("Scanning for drivers finished");

                    this.progressBar.Position++;
                }

                if (Main.bScanHelpFiles)
                {
                    this.UpdateSection("Help files");

                    Main.Logger.WriteLine("Starting to scan help files");
                    this.threadCurrent = new Thread(new ThreadStart(HelpFiles.Scan));
                    this.threadCurrent.Start();
                    this.threadCurrent.Join();
                    Main.Logger.WriteLine("Scanning for help files finished");

                    this.progressBar.Position++;
                }

                if (Main.bScanSounds)
                {
                    this.UpdateSection("Sound events");

                    Main.Logger.WriteLine("Starting to scan sound events");
                    this.threadCurrent = new Thread(new ThreadStart(Sounds.Scan));
                    this.threadCurrent.Start();
                    this.threadCurrent.Join();
                    Main.Logger.WriteLine("Scanning for sound events finished");

                    this.progressBar.Position++;
                }

                if (Main.bScanAppSettings)
                {
                    this.UpdateSection("Software settings");

                    Main.Logger.WriteLine("Starting to scan software settings");
                    this.threadCurrent = new Thread(new ThreadStart(AppSettings.Scan));
                    this.threadCurrent.Start();
                    this.threadCurrent.Join();
                    Main.Logger.WriteLine("Scanning for software settings finished");

                    this.progressBar.Position++;
                }

                if (Main.bScanHistoryList)
                {
                    this.UpdateSection("History List");

                    Main.Logger.WriteLine("Starting to scan history list");
                    this.threadCurrent = new Thread(new ThreadStart(HistoryList.Scan));
                    this.threadCurrent.Start();
                    this.threadCurrent.Join();
                    Main.Logger.WriteLine("Scanning for history list finished");

                    this.progressBar.Position++;
                }

                this.DialogResult = DialogResult.OK;
            }
            catch (ThreadAbortException)
            {
                // Scanning was aborted
                Main.Logger.WriteLine("User aborted scan... Exiting.");
                if (this.threadCurrent.IsAlive)
                    this.threadCurrent.Abort();

                this.DialogResult = DialogResult.Abort;
            }
            finally
            {
                // Finished Scanning
                Main.Logger.WriteLine("Total Items Scanned: " + this.ItemsScanned.ToString());
                Main.Logger.WriteLine("Finished Scanning!");

                this.Close();
            }

            return;
        }

        /// <summary>
        /// Stores an invalid registry key to array list
        /// </summary>
        /// <param name="Problem">Reason its invalid</param>
        /// <param name="Path">The path to registry key (including registry hive)</param>
        /// <returns>True if it was added</returns>
        public static bool StoreInvalidKey(string Problem, string Path)
        {
            return StoreInvalidKey(Problem, Path, "");
        }

        /// <summary>
        /// Stores an invalid registry key to array list
        /// </summary>
        /// <param name="Problem">Reason its invalid</param>
        /// <param name="Path">The path to registry key (including registry hive)</param>
        /// <param name="ValueName">Value name (leave blank if theres none)</param>
        /// <returns>True if it was added</returns>
        public static bool StoreInvalidKey(string Problem, string Path, string ValueName)
        {
            // See if key exists
            if (!Utils.RegKeyExists(Path))
                return false;  

            if (arrBadRegistryKeys.Add(ScanDlg.CurrentSection, Problem, Path, ValueName) > 0)
            {
                self.IncrementProblems();
                Main.Logger.WriteLine("Found invalid registry key. Key Name: \"" + ValueName + "\" Path: \"" + Path + "\" Reason: \"" + Problem + "\"");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks for registry subkey in ignore list
        /// </summary>
        /// <param name="Path">Registry subkey</param>
        /// <returns>true if it is on the ignore list, otherwise false</returns>
        private static bool IsOnIgnoreList(string Path)
        {
            if (Properties.Settings.Default.arrayOptionsExcludeList != null)
            {
                for (int i = 0; i < Properties.Settings.Default.arrayOptionsExcludeList.Count; i++)
                {
                    string[] arrayExcludePath = (string[])Properties.Settings.Default.arrayOptionsExcludeList[i];
                    string strExcludePath = string.Format("{0}\\{1}", arrayExcludePath[0], arrayExcludePath[1]);

                    if (string.Compare(strExcludePath, Path) == 0)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Updates the textbox with the current subkey being scanned
        /// </summary>
        public static void UpdateScanSubKey(string SubKey)
        {
            try
            {
                if (self.InvokeRequired)
                {
                    self.BeginInvoke(new UpdateScanSubKeyDelgate(UpdateScanSubKey), SubKey);
                    return;
                }

                string strSubKey = Utils.PrefixRegPath(SubKey);

                self.textBoxSubKey.Text = strSubKey;
                self.ItemsScanned++;
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Updates the dialog with the current section being scanned
        /// <param name="SectionName">Section Name thats being scanned</param>
        /// </summary>
        private void UpdateSection(string SectionName)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdateSectionDelegate(UpdateSection), SectionName);
                return;
            }

            this.strCurrentSection = SectionName;
            string strText = "Scanning: " + SectionName;

            this.progressBar.Text = strText;
        }

        

        /// <summary>
        /// Updates the number of problems
        /// </summary>
        private void IncrementProblems()
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
