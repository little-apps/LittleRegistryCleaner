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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Little_Registry_Cleaner.Xml;
using Microsoft.Win32;

namespace Little_Registry_Cleaner
{
    public partial class Main : Form
    {
        // Sections to scan, these must be set to true
        public static bool bScanActivex = true;
        public static bool bScanStartup = true;
        public static bool bScanFonts = true;
        public static bool bScanAppInfo = true;
        public static bool bScanDrivers = true;
        public static bool bScanHelpFiles = true;
        public static bool bScanSounds = true;
        public static bool bScanAppPaths = true;
        public static bool bScanAppSettings = true;
        public static bool bScanSharedDLL = true;
        public static bool bScanHistoryList = true;

        public Main()
        {
            InitializeComponent();
        } 

        /// <summary>
        /// Begins scanning the registry
        /// </summary>
        private void ScanRegistry()
        {
            int nSectionCount = 0;

            // Clear old results
            this.listResults.Items.Clear();
            ScanDlg.arrBadRegistryKeys.Clear();

            // Get number of sections to scan
            for (int i = 0; i < this.treeView1.Nodes[0].Nodes.Count; i++)
                if (this.treeView1.Nodes[0].Nodes[i].Checked)
                    nSectionCount++;

            if (nSectionCount == 0)
            {
                MessageBox.Show(this, "Please select a section to scan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Open Scan dialog
            ScanDlg frmScanBox = new ScanDlg(nSectionCount);
            frmScanBox.ShowDialog(this);

            // See if there are any bad registry keys
            if (ScanDlg.arrBadRegistryKeys.Count > 0)
            {
                foreach (ScanDlg.BadRegistryKey p in ScanDlg.arrBadRegistryKeys)
                {
                    ListViewItem listViewItem = new ListViewItem();

                    listViewItem.Checked = true;

                    listViewItem.Text = p.strProblem;
                    listViewItem.SubItems.Add(p.strRegPath);
                    listViewItem.SubItems.Add(p.strValueName);

                    this.listResults.Items.Add(listViewItem);
                }

                this.listResults.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

                // Notify user using notify icon
                this.notifyIcon1.ShowBalloonTip(5000, Application.ProductName, string.Format("Found {0} Problems", ScanDlg.arrBadRegistryKeys.Count), ToolTipIcon.Info);

                // Enable menu items
                this.fixToolStripMenuItem.Enabled = true;
                this.toolStripButtonFix.Enabled = true;
            }
        }
        
        /// <summary>
        /// If problems were found, removes them from registry
        /// </summary>
        private void FixProblems()
        {
            xmlRegistry xmlReg = new xmlRegistry();
            long lSeqNum = 0;

            if (this.listResults.Items.Count > 0)
            {
                if (MessageBox.Show(this, "Would you like to fix all selected problems?", "Little Registry Cleaner", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // Create Restore Point
                    SysRestore.StartRestore("Before Little Registry Cleaner Registry Fix", out lSeqNum);

                    // Generate filename to backup registry
                    string strBackupFile = string.Format("{0}\\{1:yyyy}_{1:MM}_{1:dd}_{1:HH}{1:mm}{1:ss}.xml", Properties.Settings.Default.strOptionsBackupDir, DateTime.Now);

                    ArrayList arrBadRegKeys = new ArrayList();

                    foreach (ListViewItem listViewItem in this.listResults.CheckedItems)
                    {
                        ScanDlg.BadRegistryKey obj = new ScanDlg.BadRegistryKey();
                        obj.strProblem = listViewItem.SubItems[0].Text;
                        obj.strRegPath = listViewItem.SubItems[1].Text;
                        obj.strValueName = listViewItem.SubItems[2].Text;
                        arrBadRegKeys.Add(obj);
                    }

                    // Generate a restore file and delete keys & values
                    xmlReg.deleteAsXml(arrBadRegKeys, strBackupFile);

                    // Disable menu items
                    this.fixToolStripMenuItem.Enabled = false;
                    this.toolStripButtonFix.Enabled = false;

                    SysRestore.EndRestore(lSeqNum);

                    MessageBox.Show(this, "Removed problems from registry", "Little Registry Cleaner", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Clear old results
                    this.listResults.Items.Clear();
                    ScanDlg.arrBadRegistryKeys.Clear();

                    // Scan again
                    if (Properties.Settings.Default.bOptionsRescan)
                        ScanRegistry();
                }
            }
        }

        private void Main_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                this.Hide();

            if (this.listResults.Items.Count > 0)
                this.listResults.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                this.Hide();
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void Main_Shown(object sender, EventArgs e)
        {
            // Expand all sections
            this.treeView1.Nodes[0].ExpandAll();

            // See if we have the current version
            if (Properties.Settings.Default.bOptionsAutoUpdate)
            {
                string strVersion = "", strChangeLogURL = "", strDownloadURL = "";
                if (UpdateDlg.FindUpdate(ref strVersion, ref strChangeLogURL, ref strDownloadURL))
                    if (MessageBox.Show(this, "A newer version is available. Would you like to download it?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        LaunchURI(strDownloadURL);
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // If My Computer is changed, set all to true/false
            if (e.Node.Name.CompareTo("Node0") == 0)
            {
                for (int i = 0; i < e.Node.Nodes.Count; i++)
                    e.Node.Nodes[i].Checked = e.Node.Checked;

                Main.bScanActivex = e.Node.Checked;
                Main.bScanStartup = e.Node.Checked;
                Main.bScanFonts = e.Node.Checked;
                Main.bScanAppInfo = e.Node.Checked;
                Main.bScanDrivers = e.Node.Checked;
                Main.bScanHelpFiles = e.Node.Checked;
                Main.bScanSounds = e.Node.Checked;
                Main.bScanAppPaths = e.Node.Checked;
                Main.bScanAppSettings = e.Node.Checked;
                Main.bScanSharedDLL = e.Node.Checked;
                Main.bScanHistoryList = e.Node.Checked;
            }
            else
            {
                // If single node is changed, set my computer to true/false
                this.treeView1.Nodes[0].Checked = false;

                for (int i = 0; i < this.treeView1.Nodes[0].Nodes.Count ;i++)
                    if (this.treeView1.Nodes[0].Nodes[i].Checked)
                    {
                        this.treeView1.Nodes[0].Checked = true;
                        break;
                    }
            }

            if (e.Node.Name.CompareTo("NodeActiveX") == 0)
                Main.bScanActivex = e.Node.Checked;
            else if (e.Node.Name.CompareTo("NodeStartup") == 0)
                Main.bScanStartup = e.Node.Checked;
            else if (e.Node.Name.CompareTo("NodeFonts") == 0)
                Main.bScanFonts = e.Node.Checked;
            else if (e.Node.Name.CompareTo("NodeAppInfo") == 0)
                Main.bScanAppInfo = e.Node.Checked;
            else if (e.Node.Name.CompareTo("NodeDrivers") == 0)
                Main.bScanDrivers = e.Node.Checked;
            else if (e.Node.Name.CompareTo("NodeHelp") == 0)
                Main.bScanHelpFiles = e.Node.Checked;
            else if (e.Node.Name.CompareTo("NodeSounds") == 0)
                Main.bScanSounds = e.Node.Checked;
            else if (e.Node.Name.CompareTo("NodeAppPaths") == 0)
                Main.bScanAppPaths = e.Node.Checked;
            else if (e.Node.Name.CompareTo("NodeAppSettings") == 0)
                Main.bScanAppSettings = e.Node.Checked;
            else if (e.Node.Name.CompareTo("NodeSharedDlls") == 0)
                Main.bScanSharedDLL = e.Node.Checked;
            else if (e.Node.Name.CompareTo("NodeHistoryList") == 0)
                Main.bScanHistoryList = e.Node.Checked;
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }

            if (MessageBox.Show(this, "Are you sure you want to exit?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                e.Cancel = true;
        }

        

        

        /// <summary>
        /// Checks for default program then launches URI
        /// </summary>
        /// <param name="uri">The URI to launch</param>
        /// <returns>False if the scheme is not set</returns>
        public static bool LaunchURI(string strUri)
        {
            if (!string.IsNullOrEmpty(strUri))
            {
                Uri uri = new Uri(strUri);

                if (Uri.CheckSchemeName(uri.Scheme))
                {
                    try
                    {
                        if (Process.Start(uri.ToString()) != null)
                            return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }

            return false;
        }

        #region "Menu Events"
        #region "Global Menu Events"
        private void StartOptimizer(object sender, EventArgs e)
        {
            Optimizer.Optimizer dlgOptimizer = new Little_Registry_Cleaner.Optimizer.Optimizer();
            dlgOptimizer.ShowDialog();
        }

        private void LaunchHelpSite(object sender, EventArgs e)
        {
            LaunchURI("http://answers.launchpad.net/lilregcleaner");
        }

        private void OpenOptions(object sender, EventArgs e)
        {
            Options dlgOptions = new Options();
            dlgOptions.ShowDialog(this);
        }

        private void ScanRegistry(object sender, EventArgs e)
        {
            ScanRegistry();
        }

        private void FixRegistry(object sender, EventArgs e)
        {
            FixProblems();
        }

        private void RestoreRegistry(object sender, EventArgs e)
        {
            Restore RestoreDlg = new Restore();
            RestoreDlg.ShowDialog(this);
        }

        private void ViewInRegEdit(object sender, EventArgs e)
        {
            if (this.listResults.SelectedIndices.Count > 0 && this.listResults.Items.Count > 0)
            {
                Utils.RegEditGo(this.listResults.SelectedItems[0].SubItems[1].Text);
            }
        }

        private void SelectAllListResults(object sender, EventArgs e)
        {
            for (int i = 0; i < this.listResults.Items.Count; i++)
                this.listResults.Items[i].Checked = true;
        }

        private void SelectNoneListResults(object sender, EventArgs e)
        {
            for (int i = 0; i < this.listResults.Items.Count; i++)
                this.listResults.Items[i].Checked = false;
        }

        private void ExcludeSelectedListResults(object sender, EventArgs e)
        {
            if (this.listResults.SelectedIndices.Count > 0 && this.listResults.Items.Count > 0)
            {
                for (int i = 0; i < this.listResults.SelectedItems.Count; i++)
                {
                    string strPath = this.listResults.SelectedItems[i].SubItems[1].Text;

                    string strBaseKey = strPath.Substring(0, strPath.IndexOf('\\'));
                    string strSubKey = strPath.Substring(strPath.IndexOf('\\') + 1);

                    if (Properties.Settings.Default.arrayOptionsExcludeList == null)
                        Properties.Settings.Default.arrayOptionsExcludeList = new ArrayList();

                    Properties.Settings.Default.arrayOptionsExcludeList.Add(new string[] { strBaseKey, strSubKey });
                }

                MessageBox.Show(this, "Added selected subkeys to the exclude list", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        #region "Main Menu Strip"

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void visitWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LaunchURI("http://sourceforge.net/projects/littlecleaner/");
        }

        private void viewChangeLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists("ChangeLog.txt"))
                Process.Start("ChangeLog.txt");
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateDlg frmCheckForUpdate = new UpdateDlg();
            frmCheckForUpdate.ShowDialog(this);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About frmAbout = new About();
            frmAbout.ShowDialog(this);
        }

        #endregion

        #region "Notify Icon Menu"
        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                About dlgAbout = new About();
                dlgAbout.ShowDialog(this);
            }
        }

        private void hideShowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                this.Hide();
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        private void startupManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartupManager.StartupManager dlgStartupManager = new StartupManager.StartupManager();
            dlgStartupManager.ShowDialog(this);
        }
        #endregion

        private void uninstallManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UninstallManager.UninstallManager dlgUninstallManager = new Little_Registry_Cleaner.UninstallManager.UninstallManager();
            dlgUninstallManager.ShowDialog(this);
        }


    }
}
