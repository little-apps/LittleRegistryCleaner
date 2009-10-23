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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Resources;
using System.Globalization;
using System.Threading;
using Little_Registry_Cleaner.Xml;
using Common_Tools.TreeViewAdv.Tree;
using Microsoft.Win32;

namespace Little_Registry_Cleaner
{
    public partial class Main : Form
    {

        #region Sections to scan
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
        #endregion

        private bool bDisplayExitMsgBox = true;

        private BadRegKeySorter listViewItemSorter = new BadRegKeySorter();

        private TreeModel treeModel = new TreeModel();

        private static Logger _logger;
        public static Logger Logger
        {
            get { return _logger; }
        }

        public Main()
        {
            InitializeComponent();
        } 

        /// <summary>
        /// Begins scanning the registry
        /// </summary>
        private void ScanRegistry()
        {
            // Clear old results
            this.treeModel.Nodes.Clear();

            // Get number of sections to scan
            int nSectionCount = 0;
            foreach (TreeNode tn in this.treeView1.TopNode.Nodes)
                if (tn.Checked)
                    nSectionCount++;

            if (nSectionCount == 0)
            {
                MessageBox.Show(this, "Please select a section to scan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Read start time of scan
            DateTime dtStart = DateTime.Now;

            // Create new logger instance + write header
            Main._logger = new Logger(Path.GetTempFileName());

            // Open Scan dialog
            ScanDlg frmScanBox = new ScanDlg(nSectionCount);
            DialogResult dlgResult = frmScanBox.ShowDialog(this);

            // Reset details control
            this.detailsRegView1.Problem = string.Empty;
            this.detailsRegView1.RegKey = string.Empty;
            this.detailsRegView1.ValueName = string.Empty;
            this.detailsRegView1.Data = string.Empty;

            // See if there are any bad registry keys
            if (ScanDlg.arrBadRegistryKeys.Count > 0)
            {
                // Load bad registry keys
                foreach (BadRegistryKey p in ScanDlg.arrBadRegistryKeys)
                    this.treeModel.Nodes.Add(p);

                // Collapse all and Resize columns 
                this.treeViewAdvResults.CollapseAll();
                this.treeViewAdvResults.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

                // Show notify box
                if (dlgResult == DialogResult.OK)
                    this.notifyIcon1.ShowBalloonTip(6000, Application.ProductName, "Finished scanning the registry", ToolTipIcon.Info);
                else
                    this.notifyIcon1.ShowBalloonTip(6000, Application.ProductName, "Aborted scanning the registry", ToolTipIcon.Info);
                
                // Copy to directory and display log file
                Main.Logger.DisplayLogFile((Properties.Settings.Default.bOptionsAutoRepair && dlgResult == DialogResult.OK));

                // Enable menu items
                this.fixToolStripMenuItem.Enabled = true;
                this.toolStripButtonFix.Enabled = true;

                // If power user option selected, Automatically fix problems
                if (Properties.Settings.Default.bOptionsAutoRepair && dlgResult == DialogResult.OK)
                    this.FixProblems();
            }
        }
        
        /// <summary>
        /// If problems were found, removes them from registry
        /// </summary>
        private void FixProblems()
        {
            xmlRegistry xmlReg = new xmlRegistry();
            long lSeqNum = 0;

            if (this.treeModel.Nodes.Count > 0)
            {
                if (!Properties.Settings.Default.bOptionsAutoRepair)
                {
                    if (MessageBox.Show(this, "Would you like to fix all selected problems?", "Little Registry Cleaner", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        return;
                }

                // Create Restore Point
                SysRestore.StartRestore("Before Little Registry Cleaner Registry Fix", out lSeqNum);

                // Generate filename to backup registry
                string strBackupFile = string.Format("{0}\\{1:yyyy}_{1:MM}_{1:dd}_{1:HH}{1:mm}{1:ss}.xml", Properties.Settings.Default.strProgramSettingsDir, DateTime.Now);

                BadRegKeyArray arrBadRegKeys = new BadRegKeyArray();
                foreach (BadRegistryKey badRegKeyRoot in this.treeModel.Nodes)
                {
                    foreach (BadRegistryKey badRegKey in badRegKeyRoot.Nodes)
                        if (badRegKey.Checked == CheckState.Checked)
                            arrBadRegKeys.Add(badRegKey);
                }

                // Generate a restore file and delete keys & values
                xmlReg.deleteAsXml(arrBadRegKeys, strBackupFile);

                // Disable menu items
                this.fixToolStripMenuItem.Enabled = false;
                this.toolStripButtonFix.Enabled = false;

                SysRestore.EndRestore(lSeqNum);

                // Display message box
                if (!Properties.Settings.Default.bOptionsAutoExit)
                    MessageBox.Show(this, "Removed problems from registry", "Little Registry Cleaner", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Clear old results
                this.treeModel.Nodes.Clear();

                // If power user option selected, Automatically exit program
                if (Properties.Settings.Default.bOptionsAutoExit)
                {
                    this.bDisplayExitMsgBox = false;
                    this.Close();
                    return;
                }

                // Scan again
                if (Properties.Settings.Default.bOptionsRescan)
                    ScanRegistry();
            }
        }

        private void Main_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                this.Hide();
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

            // Add tree model to treeviewadv
            this.treeViewAdvResults.Model = this.treeModel;

            // See if we have the current version
            if (Properties.Settings.Default.bOptionsAutoUpdate)
            {
                string strVersion = "", strChangeLogURL = "", strDownloadURL = "", strReleaseDate = "";
                if (UpdateDlg.FindUpdate(ref strVersion, ref strReleaseDate, ref strChangeLogURL, ref strDownloadURL, true))
                    if (MessageBox.Show(this, "A newer version is available. Would you like to download it?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        Utils.LaunchURI(strDownloadURL);
            }

            // Increase number to program starts
            Properties.Settings.Default.nProgramStarts += 1;

            // Check if we need to create restore point
            if (Properties.Settings.Default.nProgramStarts == 1)
            {
                if (MessageBox.Show(this, "This your first time running Little Registry Cleaner. \r\nWould you like to create a restore point?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SysRestore.FirstRunRestore("First time running Little Registry Cleaner");
                }
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

            if (bDisplayExitMsgBox)
            {
                if (MessageBox.Show(this, "Are you sure you want to exit?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    e.Cancel = true;
            }
        }

        private void listResults_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            //// Determine if clicked column is already the column that is being sorted.
            //if (e.Column == this.listViewItemSorter.SortColumn)
            //{
            //    // Reverse the current sort direction for this column.
            //    if (this.listViewItemSorter.Order == SortOrder.Ascending)
            //    {
            //        this.listViewItemSorter.Order = SortOrder.Descending;
            //    }
            //    else
            //    {
            //        this.listViewItemSorter.Order = SortOrder.Ascending;
            //    }
            //}
            //else
            //{
            //    // Set the column number that is to be sorted; default to ascending.
            //    this.listViewItemSorter.SortColumn = e.Column;
            //    this.listViewItemSorter.Order = SortOrder.Ascending;
            //}

            //// Perform the sort with these new sort options.
            //this.listResults.Sort();

        }

        #region "Menu Events"
        #region "Global Menu Events"

        private void LaunchHelpFile(object sender, EventArgs e)
        {
            if (!File.Exists("Little Registry Cleaner.chm"))
            {
                MessageBox.Show(this, "Unable to find Little Registry Cleaner.chm", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Help.ShowHelp(this, "Little Registry Cleaner.chm");
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
            if (this.treeViewAdvResults.SelectedNodes.Count > 0)
            {
                BadRegistryKey brk = this.treeViewAdvResults.SelectedNode.Tag as BadRegistryKey;
                string strSubKey = brk.RegKeyPath;
                string strValueName = brk.ValueName;

                RegEditGo.GoTo(strSubKey, strValueName);
            }
        }

        private void SelectAllListResults(object sender, EventArgs e)
        {
            foreach (BadRegistryKey brkRoot in this.treeModel.Nodes)
            {
                brkRoot.Checked = CheckState.Checked;
                foreach (BadRegistryKey brk in brkRoot.Nodes)
                    brk.Checked = CheckState.Checked;
            }

            this.treeViewAdvResults.Refresh();
        }

        private void SelectNoneListResults(object sender, EventArgs e)
        {
            foreach (BadRegistryKey brkRoot in this.treeModel.Nodes)
            {
                brkRoot.Checked = CheckState.Unchecked;
                foreach (BadRegistryKey brk in brkRoot.Nodes)
                    brk.Checked = CheckState.Unchecked;
            }

            this.treeViewAdvResults.Refresh();
        }

        private void ExcludeSelectedListResults(object sender, EventArgs e)
        {
            if (this.treeViewAdvResults.SelectedNodes.Count > 0)
            {
                for (int i = 0; i < this.treeViewAdvResults.SelectedNodes.Count; i++)
                {
                    BadRegistryKey brk = this.treeViewAdvResults.SelectedNodes[i].Tag as BadRegistryKey;

                    if (!string.IsNullOrEmpty(brk.RegKeyPath))
                        Properties.Settings.Default.arrayExcludeList.Add(new ExcludeList.ExcludeItem(brk.RegKeyPath, null, null));
                }

                MessageBox.Show(this, "Added selected registry keys to the exclude list", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            Utils.LaunchURI("http://sourceforge.net/projects/littlecleaner/");
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

        private void StartupManager(object sender, EventArgs e)
        {
            StartupManager.StartupManager dlgStartupManager = new StartupManager.StartupManager();
            dlgStartupManager.ShowDialog(this);
        }

        private void UninstallManager(object sender, EventArgs e)
        {
            UninstallManager.UninstallManager dlgUninstallManager = new Little_Registry_Cleaner.UninstallManager.UninstallManager();
            dlgUninstallManager.ShowDialog(this);
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

        
        #endregion

        #region "Tree View Adv Events"
        private void treeViewAdvResults_SelectionChanged(object sender, EventArgs e)
        {
            if (this.treeViewAdvResults.SelectedNode == null)
                return;

            BadRegistryKey brk = this.treeViewAdvResults.SelectedNode.Tag as BadRegistryKey;

            this.detailsRegView1.Problem = brk.Problem;
            this.detailsRegView1.RegKey = brk.RegKeyPath;
            this.detailsRegView1.ValueName = brk.ValueName;
            this.detailsRegView1.Data = brk.Data;
        }

        private void treeViewAdvResults_Expanded(object sender, TreeViewAdvEventArgs e)
        {
            this.treeViewAdvResults.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }
        #endregion

        private void ChangeLanguage(object sender, EventArgs e)
        {
            ToolStripMenuItem lang = sender as ToolStripMenuItem;
            CultureInfo ci = CultureInfo.CurrentUICulture;

            

            // Uncheck old language
            foreach (ToolStripMenuItem tsmi in this.languageToolStripMenuItem.DropDownItems)
            {
                if (tsmi.Checked)
                    tsmi.Checked = false;
            }

            ResourceManager rm = new ResourceManager("Main", this.GetType().Assembly);

            switch (lang.Text)
            {
                case "English":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
                    break;

                case "Spanish":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("es");
                    break;

                case "Arabic":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("ar");
                    break;

                case "German":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("de");
                    break;

                case "Greek":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("el");
                    break;

                case "French":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr");
                    break;

                case "Italian":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("it");
                    break;

                case "Japanese":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("ja");
                    break;

                case "Dutch":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("nl");
                    break;

                case "Portugese":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("pt");
                    break;

                case "Russian":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru");
                    break;

                case "Polish":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("pl");
                    break;

                case "Swedish":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("sv");
                    break;

                case "Thai":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("th");
                    break;

                case "Vietnamese":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("vi");
                    break;

                case "Chinese (Simplified)":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CHS");
                    break;
                case "Chinese (Traditional)":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CHT");
                    break;
            }

            this.ReloadControls();

            lang.Checked = true;
        }

        private void ReloadControls()
        {
            // Reload control strings
            ResourceManager resources = new ResourceManager(typeof(Main));
            this.Text = resources.GetString("$this.Text");
            this.aboutToolStripMenuItem.Text = resources.GetString("aboutToolStripMenuItem.Text");
            this.aboutToolStripMenuItem1.Text = resources.GetString("aboutToolStripMenuItem1.Text");
            this.checkForUpdatesToolStripMenuItem.Text = resources.GetString("checkForUpdatesToolStripMenuItem.Text");
            this.editToolStripMenuItem.Text = resources.GetString("editToolStripMenuItem.Text");
            this.excludeSelectedToolStripMenuItem.Text = resources.GetString("excludeSelectedToolStripMenuItem.Text");
            this.excludeSelectedToolStripMenuItem1.Text = resources.GetString("excludeSelectedToolStripMenuItem1.Text");
            this.exitToolStripMenuItem.Text = resources.GetString("exitToolStripMenuItem.Text");
            this.fileToolStripMenuItem.Text = resources.GetString("fileToolStripMenuItem.Text");
            this.fixToolStripMenuItem.Text = resources.GetString("fixToolStripMenuItem.Text");
            this.helpToolStripMenuItem.Text = resources.GetString("helpToolStripMenuItem.Text");
            this.helpToolStripMenuItem1.Text = resources.GetString("helpToolStripMenuItem1.Text");
            this.hideShowToolStripMenuItem.Text = resources.GetString("hideShowToolStripMenuItem.Text");
            this.languageToolStripMenuItem.Text = resources.GetString("languageToolStripMenuItem.Text");
            this.notifyIcon1.Text = resources.GetString("notifyIcon1.Text");
            this.restoreToolStripMenuItem.Text = resources.GetString("restoreToolStripMenuItem.Text");
            this.scanToolStripMenuItem.Text = resources.GetString("scanToolStripMenuItem.Text");
            this.selectAllToolStripMenuItem.Text = resources.GetString("selectAllToolStripMenuItem.Text");
            this.selectAllToolStripMenuItem1.Text = resources.GetString("selectAllToolStripMenuItem1.Text");
            this.selectNoneToolStripMenuItem.Text = resources.GetString("selectNoneToolStripMenuItem.Text");
            this.selectNoneToolStripMenuItem1.Text = resources.GetString("selectNoneToolStripMenuItem1.Text");
            this.startupManagerToolStripMenuItem.Text = resources.GetString("startupManagerToolStripMenuItem.Text");
            this.toolsToolStripMenuItem.Text = resources.GetString("toolsToolStripMenuItem.Text");
            this.toolStripButtonFix.Text = resources.GetString("toolStripButtonFix.Text");
            this.toolStripButtonHelp.Text = resources.GetString("toolStripButtonHelp.Text");
            this.toolStripButtonRestore.Text = resources.GetString("toolStripButtonRestore.Text");
            this.toolStripButtonScan.Text = resources.GetString("toolStripButtonScan.Text");
            this.toolStripButtonSettings.Text = resources.GetString("toolStripButtonSettings.Text");
            this.toolStripMenuItemOptions.Text = resources.GetString("toolStripMenuItemOptions.Text");
            this.treeColumn1.Header = resources.GetString("treeColumn1.Header");
            this.treeColumn2.Header = resources.GetString("treeColumn2.Header");
            this.treeColumn3.Header = resources.GetString("treeColumn3.Header");
            this.uninstallManagerToolStripMenuItem.Text = resources.GetString("uninstallManagerToolStripMenuItem.Text");
            this.viewInRegeditToolStripMenuItem.Text = resources.GetString("viewInRegeditToolStripMenuItem.Text");
            this.viewInRegeditToolStripMenuItem1.Text = resources.GetString("viewInRegeditToolStripMenuItem1.Text");
            this.visitWebsiteToolStripMenuItem.Text = resources.GetString("visitWebsiteToolStripMenuItem.Text");

            this.detailsRegView1.ReloadControls();

            this.treeView1.Nodes.Clear();
            this.treeView1.Nodes.Add(resources.GetObject("treeView1.Nodes") as TreeNode);
            this.treeView1.ExpandAll();
        }
    }
}
