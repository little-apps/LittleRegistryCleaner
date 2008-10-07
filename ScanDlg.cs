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
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Management;
using Little_Registry_Cleaner.Scanners;
using Microsoft.Win32;
using Little_Registry_Cleaner.Xml;
using System.ServiceProcess;

namespace Little_Registry_Cleaner
{
    public partial class ScanDlg : Form
    {
        public delegate void UpdateListViewHandler(string strProblem, string strData, string strValue);
        public event UpdateListViewHandler UpdateListView;

        public delegate void StoreInvalidKeyDelegate(string strProblem, string strPath, string strValueName);
        public delegate void StoreInvalidSubKeyDelegate(string strProblem, string strPath);

        public delegate void UpdateScanSubKeyDelgate(string strSubKey);
        public delegate void UpdateSectionDelegate(string strSection);

        [DllImport("shell32.dll", EntryPoint = "FindExecutable")]
        public static extern long FindExecutableA(string lpFile, string lpDirectory, StringBuilder lpResult);

        private Logger loggerScan;
        private Thread threadMain;
        private Thread threadCurrent;

        private int SectionCount = 0;
        private int ItemsScanned = 0;

        public ScanDlg(int nSectionCount)
        {
            InitializeComponent();

            // Set the section count so it can be accessed later
            this.SectionCount = nSectionCount;
        }

        
        private void ScanDlg_Shown(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;

            // Starts scanning registry on seperate thread
            this.threadMain = new Thread(new ThreadStart(StartScanning));
            this.threadMain.Name = "Scan Thread Pool";
            this.threadMain.Start();
        }

        /// <summary>
        /// Begins scanning for errors in the registry
        /// </summary>
        private void StartScanning()
        {
            // Create log file
            this.loggerScan = new Logger();

            this.progressBar1.Step = 1;
            this.progressBar1.Maximum = this.SectionCount;

            // Create restore point (XP Only)
            if (Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor == 1)
            {
                if (Properties.Settings.Default.bOptionsRestore)
                {
                    this.loggerScan.WriteLine("Creating restore point...");
                    CreateRestorePoint();
                }
            }

            // Begin scanning
            try
            {
                if (Little_Registry_Cleaner.Properties.Settings.Default.bScanStartup)
                {
                    this.loggerScan.WriteLine("Checking for invalid startup entries");
                    this.UpdateSection("Startup entries");

                    this.threadCurrent = new Thread(new ThreadStart(delegate { new StartUp(this); }));
                    this.threadCurrent.Start();
                    this.threadCurrent.Join();

                    this.progressBar1.PerformStep();
                }

                if (Little_Registry_Cleaner.Properties.Settings.Default.bScanSharedDLL)
                {
                    this.loggerScan.WriteLine("Checking for invalid DLL entries");
                    this.UpdateSection("Shared DLLs");

                    this.threadCurrent = new Thread(new ThreadStart(delegate { new DLLs(this); }));
                    this.threadCurrent.Start();
                    this.threadCurrent.Join();

                    this.progressBar1.PerformStep();
                }

                if (Little_Registry_Cleaner.Properties.Settings.Default.bScanFonts)
                {
                    this.loggerScan.WriteLine("Checking for invalid font references");
                    this.UpdateSection("Windows Fonts");

                    this.threadCurrent = new Thread(new ThreadStart(delegate { new Fonts(this); }));
                    this.threadCurrent.Start();
                    this.threadCurrent.Join();

                    this.progressBar1.PerformStep();
                }

                if (Little_Registry_Cleaner.Properties.Settings.Default.bScanAppInfo)
                {
                    this.loggerScan.WriteLine("Checking for invalid application info");
                    this.UpdateSection("Application info");

                    this.threadCurrent = new Thread(new ThreadStart(delegate { new AppInfo(this); }));
                    this.threadCurrent.Start();
                    this.threadCurrent.Join();

                    this.progressBar1.PerformStep();
                }

                if (Little_Registry_Cleaner.Properties.Settings.Default.bScanAppPaths)
                {
                    this.loggerScan.WriteLine("Checking for invalid application paths");
                    this.UpdateSection("Program Locations");

                    this.threadCurrent = new Thread(new ThreadStart(delegate { new AppPaths(this); }));
                    this.threadCurrent.Start();
                    this.threadCurrent.Join();

                    this.progressBar1.PerformStep();
                }

                if (Little_Registry_Cleaner.Properties.Settings.Default.bScanActivex)
                {
                    this.loggerScan.WriteLine("Checking for invalid ActiveX/COM objects");
                    this.UpdateSection("ActiveX/COM objects");

                    this.threadCurrent = new Thread(new ThreadStart(delegate { new COMObjects(this); }));
                    this.threadCurrent.Start();
                    this.threadCurrent.Join();

                    this.progressBar1.PerformStep();
                }

                if (Little_Registry_Cleaner.Properties.Settings.Default.bScanDrivers)
                {
                    this.loggerScan.WriteLine("Checking for invalid driver entries");
                    this.UpdateSection("Drivers");

                    this.threadCurrent = new Thread(new ThreadStart(delegate { new Drivers(this); }));
                    this.threadCurrent.Start();
                    this.threadCurrent.Join();

                    this.progressBar1.PerformStep();
                }

                if (Little_Registry_Cleaner.Properties.Settings.Default.bScanHelpFiles)
                {
                    this.loggerScan.WriteLine("Checking for invalid help files");
                    this.UpdateSection("Help files");

                    this.threadCurrent = new Thread(new ThreadStart(delegate { new HelpFiles(this); }));
                    this.threadCurrent.Start();
                    this.threadCurrent.Join();

                    this.progressBar1.PerformStep();
                }

                if (Little_Registry_Cleaner.Properties.Settings.Default.bScanSounds)
                {
                    this.loggerScan.WriteLine("Checking for missing windows sounds");
                    this.UpdateSection("Sound events");

                    Thread threadSounds = new Thread(new ThreadStart(delegate { new Sounds(this); }));
                    threadSounds.Start();
                    threadSounds.Join();

                    this.progressBar1.PerformStep();
                }

                if (Little_Registry_Cleaner.Properties.Settings.Default.bScanAppSettings)
                {
                    this.loggerScan.WriteLine("Checking for missing software settings");
                    this.UpdateSection("Software settings");

                    this.threadCurrent = new Thread(new ThreadStart(delegate { new AppSettings(this); }));
                    this.threadCurrent.Start();
                    this.threadCurrent.Join();

                    this.progressBar1.PerformStep();
                }


                if (Little_Registry_Cleaner.Properties.Settings.Default.bScanHistoryList)
                {
                    this.loggerScan.WriteLine("Checking for missing recent documents links");
                    this.UpdateSection("History List");

                    this.threadCurrent = new Thread(new ThreadStart(delegate { new HistoryList(this); }));
                    this.threadCurrent.Start();
                    this.threadCurrent.Join();
                }

                this.progressBar1.PerformStep();

                this.DialogResult = DialogResult.OK;
            }
            catch (ThreadAbortException)
            {
                // Scanning was aborted
                this.loggerScan.WriteLine("User aborted scan... Exiting.");
                if (this.threadCurrent.IsAlive)
                    threadCurrent.Abort();
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

        private void StartScanner(Delegate obj) 
        {
            
        }

        /// <summary>
        /// Creates a restore point on the computer
        /// </summary>
        private void CreateRestorePoint()
        {
            bool bServiceFound = false;

            // See if System Restore is enabled

            foreach (ServiceController sc in ServiceController.GetServices())
            {
                if (sc.ServiceName.CompareTo("srservice") == 0)
                {
                    if (sc.Status != ServiceControllerStatus.Running)
                    {
                        this.loggerScan.WriteLine("System Restore Service isnt running, unable to create restore point.");
                        return;
                    }

                    bServiceFound = true;
                }
            }

            if (!bServiceFound)
            {
                this.loggerScan.WriteLine("System Restore Service wasnt found, unable to create restore point.");
                return;
            }
            
            ManagementScope oScope = new ManagementScope("\\\\localhost\\root\\default");
            ManagementPath oPath = new ManagementPath("SystemRestore");
            ObjectGetOptions oGetOp = new ObjectGetOptions();
            ManagementClass oProcess = new ManagementClass(oScope, oPath, oGetOp);

            ManagementBaseObject oInParams = oProcess.GetMethodParameters("CreateRestorePoint");
            oInParams["Description"] = "Little Registry Cleaner";
            oInParams["RestorePointType"] = 0;
            oInParams["EventType"] = 100;

            ManagementBaseObject oOutParams = oProcess.InvokeMethod("CreateRestorePoint", oInParams, null);
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

        /// <summary>
        /// Store an invalid registry key with information
        /// </summary>
        /// <param name="strProblem">The reason its invalid</param>
        /// <param name="strPath">The registry sub key</param>
        /// <param name="strValueName">The registry value name (cannot be blank)</param>
        /// <returns>True on success</returns>
        public void StoreInvalidKey(string strProblem, string strPath, string strValueName)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new StoreInvalidKeyDelegate(StoreInvalidKey), new string[] { strProblem, strPath, strValueName });
                    return;
                }
                catch (ObjectDisposedException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    return;
                }
            }

            // Check if it exists and isnt on ignore list
            if (xmlRegistry.keyExists(strPath) && !IsOnIgnoreList(strPath))
            {
                // Write to listview
                UpdateListView(strProblem, strPath, strValueName);

                // Write to log
                this.loggerScan.WriteLine("Found invalid registry key. Key Name: \"" + strValueName + "\" Path: \"" + strPath + "\" Reason: \"" + strProblem + "\"");
            }

            return;
        }

        /// <summary>
        /// Store an invalid registry sub key with information
        /// </summary>
        /// <param name="strProblem">The reason its invalid</param>
        /// <param name="strPath">The registry sub key</param>
        /// <returns>True on success</returns>
        public void StoreInvalidSubKey(string strProblem, string strPath)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new StoreInvalidSubKeyDelegate(StoreInvalidSubKey), new string[] { strProblem, strPath });
                }
                catch (ObjectDisposedException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }

                return;
            }

            // Check if it exists and isnt on ignore list
            if (xmlRegistry.keyExists(strPath) && !IsOnIgnoreList(strPath))
            {
                // Write to listview
                UpdateListView(strProblem, strPath, "");

                // Write to log
                this.loggerScan.WriteLine("Found invalid registry sub key: \"" + strPath + "\" Reason: \"" + strProblem + "\"");
            }

            return;
        }

        /// <summary>
        /// Checks for registry subkey in ignore list
        /// </summary>
        /// <param name="strPath">Registry subkey</param>
        /// <returns>true if it is on the ignore list, otherwise false</returns>
        private bool IsOnIgnoreList(string strPath)
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
        public void UpdateScanSubKey(string strSubKey)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new UpdateScanSubKeyDelgate(UpdateScanSubKey), strSubKey);
                    return;
                }

                this.textBoxSubKey.Text = strSubKey;
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

            this.labelSection.Text = "Scanning: " + strSectionName;
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
