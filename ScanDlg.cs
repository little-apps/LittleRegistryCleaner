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
        public delegate void ScanDelegate();
        public delegate void UpdateScanningObjectDelgate(string Object);
        public delegate void UpdateSectionDelegate(string strSection);

        Thread threadMain, threadScan;

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
            this.progressBar.Position = 0;
            this.progressBar.PositionMin = 0;
            this.progressBar.PositionMax = this.SectionCount;

            // Starts scanning registry on seperate thread
            this.threadMain = new Thread(new ThreadStart(StartScanning));
            this.threadMain.Start();
        }

        /// <summary>
        /// Begins scanning for errors in the registry
        /// </summary>
        private void StartScanning()
        {
            // Begin scanning
            try
            {
                Main.Logger.WriteLine("Starting scan...");

                if (Main.bScanStartup)
                    this.StartScanner(new StartUp());

                if (Main.bScanSharedDLL)
                    this.StartScanner(new DLLs());

                if (Main.bScanFonts)
                    this.StartScanner(new Fonts());

                if (Main.bScanAppInfo)
                    this.StartScanner(new AppInfo());

                if (Main.bScanAppPaths)
                    this.StartScanner(new AppPaths());

                if (Main.bScanActivex)
                    this.StartScanner(new COMObjects());

                if (Main.bScanDrivers)
                    this.StartScanner(new Drivers());

                if (Main.bScanHelpFiles)
                    this.StartScanner(new HelpFiles());

                if (Main.bScanSounds)
                    this.StartScanner(new Sounds());

                if (Main.bScanAppSettings)
                    this.StartScanner(new AppSettings());

                if (Main.bScanHistoryList)
                    this.StartScanner(new HistoryList());

                this.DialogResult = DialogResult.OK;
            }
            catch (ThreadAbortException)
            {
                // Scanning was aborted
                Main.Logger.WriteLine("User aborted scan... Exiting.");
                if (this.threadScan.IsAlive)
                    this.threadScan.Abort();

                this.DialogResult = DialogResult.Abort;
            }
            finally
            {
                // Finished Scanning
                Main.Logger.WriteLine("Total Items Scanned: " + this.ItemsScanned.ToString());
                Main.Logger.WriteLine("Finished Scanning!");
            }

            // Dialog will be closed automatically

            return;
        }

        /// <summary>
        /// Starts a scanner
        /// </summary>
        public void StartScanner(ScannerBase scannerName)
        {
            System.Reflection.MethodInfo mi = scannerName.GetType().GetMethod("Scan", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            ScanDelegate objScan = (ScanDelegate)Delegate.CreateDelegate(typeof(ScanDelegate), mi);

            this.UpdateSection(scannerName.ScannerName);
            Main.Logger.WriteLine("Starting to scan: " + scannerName.ScannerName);

            // Start scanning
            this.threadScan = new Thread(new ThreadStart(objScan));
            this.threadScan.Start();
            this.threadScan.Join();

            Main.Logger.WriteLine("Finished scanning: " + scannerName.ScannerName);
            this.progressBar.Position++;
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

                if (!string.IsNullOrEmpty(ValueName))
                    Main.Logger.WriteLine(string.Format("Bad Registry Value Found! Problem: \"{0}\" Path: \"{1}\" Value Name: \"{2}\"", Problem, Path, ValueName)); 
                else
                    Main.Logger.WriteLine(string.Format("Bad Registry Key Found! Problem: \"{0}\" Path: \"{1}\"", Problem, Path)); 

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
        /// Updates the textbox with the current object being scanned
        /// </summary>
        public static void UpdateScanningObject(string Object)
        {
            try
            {
                if (self.InvokeRequired)
                {
                    self.BeginInvoke(new UpdateScanningObjectDelgate(UpdateScanningObject), Object);
                    return;
                }

                string strSubKey = Utils.PrefixRegPath(Object);

                if (string.IsNullOrEmpty(Object))
                    return;

                self.textBoxSubKey.Text = Object;
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
                this.BeginInvoke(new UpdateSectionDelegate(UpdateSection), SectionName);
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
                this.BeginInvoke(new MethodInvoker(IncrementProblems));
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
