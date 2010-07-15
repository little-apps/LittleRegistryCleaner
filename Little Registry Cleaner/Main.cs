/*
    Little Registry Cleaner
    Copyright (C) 2008-2010 Little Apps (http://www.littleapps.co.cc/)

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
                MessageBox.Show(this, Properties.Resources.mainSelectSections, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Read start time of scan
            DateTime dtStart = DateTime.Now;

            // Create new logger instance + write header
            Main._logger = new Logger(Path.GetTempFileName());

            // Open Scan dialog
            ScanDlg frmScanBox = new ScanDlg(nSectionCount);
            DialogResult dlgResult = frmScanBox.ShowDialog(this);

            // See if there are any bad registry keys
            if (ScanDlg.arrBadRegistryKeys.Count > 0)
            {
                // Load bad registry keys
                foreach (BadRegistryKey p in ScanDlg.arrBadRegistryKeys) 
                    this.treeModel.Nodes.Add(p);

                // Expand all and Resize columns 
                this.treeViewAdvResults.ExpandAll();
                this.treeViewAdvResults.AutoSizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

                // Show notify box and set status text
                ResourceManager rm = new ResourceManager(this.GetType());
                if (dlgResult == DialogResult.OK)
                {
                    this.notifyIcon1.ShowBalloonTip(6000, Application.ProductName, Properties.Resources.mainScanningFinished, ToolTipIcon.Info);
                    this.toolStripStatusLabel1.Text = Properties.Resources.mainScanningFinished;
                    this.toolStripStatusLabel1.Tag = "mainScanningFinished";
                }
                else
                {
                    this.notifyIcon1.ShowBalloonTip(6000, Application.ProductName, Properties.Resources.mainScanningAborted, ToolTipIcon.Info);
                    this.toolStripStatusLabel1.Text = Properties.Resources.mainScanningAborted;
                    this.toolStripStatusLabel1.Tag = "mainScanningAborted";
                }
                
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
                    if (MessageBox.Show(this, Properties.Resources.mainProblemsFix, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        return;
                }

                // Create Restore Point
                SysRestore.StartRestore("Before Little Registry Cleaner Registry Fix", out lSeqNum);

                // Generate filename to backup registry
                string strBackupFile = string.Format("{0}\\{1:yyyy}_{1:MM}_{1:dd}_{1:HH}{1:mm}{1:ss}.xml", Properties.Settings.Default.strOptionsBackupDir, DateTime.Now);

                BadRegKeyArray arrBadRegKeys = new BadRegKeyArray();
                foreach (BadRegistryKey badRegKeyRoot in this.treeModel.Nodes)
                {
                    foreach (BadRegistryKey badRegKey in badRegKeyRoot.Nodes)
                        if (badRegKey.Checked == CheckState.Checked)
                            arrBadRegKeys.Add(badRegKey);
                }

                // Generate a restore file and delete keys & values
                xmlReg.deleteAsXml(arrBadRegKeys, strBackupFile);

                SysRestore.EndRestore(lSeqNum);

                // Disable menu items
                this.fixToolStripMenuItem.Enabled = false;
                this.toolStripButtonFix.Enabled = false;

                // Clear status text
                ResourceManager rm = new ResourceManager(this.GetType());
                this.toolStripStatusLabel1.Text = rm.GetString("toolStripStatusLabel1.Text");
                this.toolStripStatusLabel1.Tag = "toolStripStatusLabel1.Text";

                // Display message box
                if (!Properties.Settings.Default.bOptionsAutoExit)
                    MessageBox.Show(this, Properties.Resources.mainProblemsRemoved, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

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
            this.treeViewAdvResults.Model = new SortedTreeModel(this.treeModel);

            // Set language to current culture
            this.SetCurrentLang();

            // See if we have the current version
            if (Properties.Settings.Default.bOptionsAutoUpdate)
            {
                string strVersion = "", strChangeLogURL = "", strDownloadURL = "", strReleaseDate = "";
                if (UpdateDlg.FindUpdate(ref strVersion, ref strReleaseDate, ref strChangeLogURL, ref strDownloadURL, true))
                    if (MessageBox.Show(this, Properties.Resources.mainUpdateAsk, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        Utils.LaunchURI(strDownloadURL);
            }

            // Increase number to program starts
            Properties.Settings.Default.nProgramStarts += 1;

            /***********************************************************************************************************/
            /* THE CODE BELOW IS LICENSED UNDER THE CREATIVE COMMONS ATTRIBUTION NON-COMMERCIAL NO DERIVATIVES LICENSE */
            if (Properties.Settings.Default.nProgramStarts == 1)
            {
                MessageBox.Show(this, Properties.Resources.mainFreeWareProgram, "Little Registry Cleaner", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            /* THE CODE ABOVE IS LICENSED UNDER THE CREATIVE COMMONS ATTRIBUTION NON-COMMERCIAL NO DERIVATIVES LICENSE */
            /***********************************************************************************************************/
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
                if (MessageBox.Show(this, Properties.Resources.mainAskExit, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    e.Cancel = true;
            }
        }

        #region "Menu Events"
        #region "Global Menu Events"
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

            switch (lang.Name)
            {
                case "englishToolStripMenuItem":
                    ci = new CultureInfo("en");
                    break;

                case "spanishToolStripMenuItem":
                    ci = new CultureInfo("es");
                    break;

                case "arabicToolStripMenuItem":
                    ci = new CultureInfo("ar");
                    break;

                case "germanToolStripMenuItem":
                    ci = new CultureInfo("de");
                    break;

                case "greekToolStripMenuItem":
                    ci = new CultureInfo("el");
                    break;

                case "frenchToolStripMenuItem":
                    ci = new CultureInfo("fr");
                    break;

                case "italianToolStripMenuItem":
                    ci = new CultureInfo("it");
                    break;

                case "japaneseToolStripMenuItem":
                    ci = new CultureInfo("ja");
                    break;

                case "dutchToolStripMenuItem":
                    ci = new CultureInfo("nl");
                    break;

                case "portugueseToolStripMenuItem":
                    ci = new CultureInfo("pt");
                    break;

                case "russianToolStripMenuItem":
                    ci = new CultureInfo("ru");
                    break;

                case "polishToolStripMenuItem":
                    ci = new CultureInfo("pl");
                    break;

                case "swedishToolStripMenuItem":
                    ci = new CultureInfo("sv-SE");
                    break;

                case "thaiToolStripMenuItem":
                    ci = new CultureInfo("th");
                    break;

                case "vietnameseToolStripMenuItem":
                    ci = new CultureInfo("vi");
                    break;

                case "chineseSimplifiedToolStripMenuItem":
                    ci = new CultureInfo("zh-CHS");
                    break;

                case "chineseTraditionalToolStripMenuItem":
                    ci = new CultureInfo("zh-CHT");
                    break;

                case "hungarianToolStripMenuItem":
                    ci = new CultureInfo("hu");
                    break;

                case "turkishToolStripMenuItem":
                    ci = new CultureInfo("tr");
                    break;

                case "lithuanianToolStripMenuItem":
                    ci = new CultureInfo("lt");
                    break;
                case "persianToolStripMenuItem":
                    ci = new CultureInfo("fa");
                    break;
            }

            if (ci.Name == "zh-CHS")
                Application.CurrentCulture = new CultureInfo(0x0804); // zh-CN Chinese (People's Republic of China)
            else if (ci.Name == "zh-CHT")
                Application.CurrentCulture = new CultureInfo(0x0404); // zh-TW Chinese (Taiwan)
            else
                Application.CurrentCulture = CultureInfo.CreateSpecificCulture(ci.Name);
            Thread.CurrentThread.CurrentUICulture = ci;
            Scanners.Strings.Culture = ci;
            Properties.Resources.Culture = ci;

            this.ReloadControls();

            lang.Checked = true;
        }

        private void LaunchHelpFile(object sender, EventArgs e)
        {
            if (!File.Exists("Little Registry Cleaner.chm"))
            {
                MessageBox.Show(this, Properties.Resources.mainInvalidHelpFile, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        private void treeViewAdvResults_NodeMouseDoubleClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            BadRegistryKey brk = e.Node.Tag as BadRegistryKey;

            Common_Tools.DetailsRegKey details = new Common_Tools.DetailsRegKey(brk.Problem, brk.RegKeyPath, brk.ValueName, brk.Data);
            details.ShowDialog(this);
        }

        private void treeViewAdvResults_Expanded(object sender, TreeViewAdvEventArgs e)
        {
            this.treeViewAdvResults.AutoSizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void treeViewAdvResults_ColumnClicked(object sender, TreeColumnEventArgs e)
        {
            TreeColumn col = e.Column;
            if (col.SortOrder == SortOrder.Ascending)
                col.SortOrder = SortOrder.Descending;
            else
                col.SortOrder = SortOrder.Ascending;

            (this.treeViewAdvResults.Model as SortedTreeModel).Comparer = new BadRegKeySorter(col.Header, col.SortOrder);

            this.treeViewAdvResults.ExpandAll();
        }

        private void treeViewAdvResults_SelectAll(object sender, EventArgs e)
        {
            foreach (BadRegistryKey brkRoot in this.treeModel.Nodes)
            {
                brkRoot.Checked = CheckState.Checked;
                foreach (BadRegistryKey brk in brkRoot.Nodes)
                    brk.Checked = CheckState.Checked;
            }

            this.treeViewAdvResults.Refresh();
        }

        private void treeViewAdvResults_SelectNone(object sender, EventArgs e)
        {
            foreach (BadRegistryKey brkRoot in this.treeModel.Nodes)
            {
                brkRoot.Checked = CheckState.Unchecked;
                foreach (BadRegistryKey brk in brkRoot.Nodes)
                    brk.Checked = CheckState.Unchecked;
            }

            this.treeViewAdvResults.Refresh();
        }

        private void treeViewAdvResults_InvertSelection(object sender, EventArgs e)
        {
            foreach (BadRegistryKey brkRoot in this.treeModel.Nodes)
            {
                foreach (BadRegistryKey brk in brkRoot.Nodes)
                    brk.Checked = ((brk.Checked == CheckState.Checked) ? (CheckState.Unchecked) : (CheckState.Checked));
            }

            this.treeViewAdvResults.Refresh();
        }

        private void treeViewAdvResults_ExcludeSelected(object sender, EventArgs e)
        {
            if (this.treeViewAdvResults.SelectedNodes.Count > 0)
            {
                for (int i = 0; i < this.treeViewAdvResults.SelectedNodes.Count; i++)
                {
                    BadRegistryKey brk = this.treeViewAdvResults.SelectedNodes[i].Tag as BadRegistryKey;

                    if (!string.IsNullOrEmpty(brk.RegKeyPath))
                        Properties.Settings.Default.arrayExcludeList.Add(new ExcludeList.ExcludeItem(brk.RegKeyPath, null, null));
                }

                MessageBox.Show(this, Properties.Resources.mainAddExcludeEntry, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        /// <summary>
        /// Sets LRC language to current culture
        /// </summary>
        private void SetCurrentLang()
        {
            ToolStripMenuItem lang = null;
            CultureInfo ci = CultureInfo.CurrentUICulture;

            // Uncheck old language
            foreach (ToolStripMenuItem tsmi in this.languageToolStripMenuItem.DropDownItems)
            {
                if (tsmi.Checked)
                    tsmi.Checked = false;
            }

            switch (ci.TwoLetterISOLanguageName)
            {
                case "en":
                    lang = (ToolStripMenuItem)this.languageToolStripMenuItem.DropDownItems["englishToolStripMenuItem"];
                    break;

                case "es":
                    lang = (ToolStripMenuItem)this.languageToolStripMenuItem.DropDownItems["spanishToolStripMenuItem"];
                    break;

                case "ar":
                    lang = (ToolStripMenuItem)this.languageToolStripMenuItem.DropDownItems["arabicToolStripMenuItem"];
                    break;

                case "de":
                    lang = (ToolStripMenuItem)this.languageToolStripMenuItem.DropDownItems["germanToolStripMenuItem"];
                    break;

                case "el":
                    lang = (ToolStripMenuItem)this.languageToolStripMenuItem.DropDownItems["greekToolStripMenuItem"];
                    break;

                case "fr":
                    lang = (ToolStripMenuItem)this.languageToolStripMenuItem.DropDownItems["frenchToolStripMenuItem"];
                    break;

                case "it":
                    lang = (ToolStripMenuItem)this.languageToolStripMenuItem.DropDownItems["italianToolStripMenuItem"];
                    break;

                case "ja":
                    lang = (ToolStripMenuItem)this.languageToolStripMenuItem.DropDownItems["japaneseToolStripMenuItem"];
                    break;

                case "nl":
                    lang = (ToolStripMenuItem)this.languageToolStripMenuItem.DropDownItems["dutchToolStripMenuItem"];
                    break;

                case "pt":
                    lang = (ToolStripMenuItem)this.languageToolStripMenuItem.DropDownItems["portugueseToolStripMenuItem"];
                    break;

                case "ru":
                    lang = (ToolStripMenuItem)this.languageToolStripMenuItem.DropDownItems["russianToolStripMenuItem"];
                    break;

                case "pl":
                    lang = (ToolStripMenuItem)this.languageToolStripMenuItem.DropDownItems["polishToolStripMenuItem"];
                    break;

                case "sv":
                    {
                        CultureInfo cultureInfo = new CultureInfo("sv-SE");
                        Thread.CurrentThread.CurrentUICulture = cultureInfo;
                        Scanners.Strings.Culture = cultureInfo;
                        Properties.Resources.Culture = cultureInfo;

                        lang = (ToolStripMenuItem)this.languageToolStripMenuItem.DropDownItems["swedishToolStripMenuItem"];
                    }
                    break;

                case "th":
                    lang = (ToolStripMenuItem)this.languageToolStripMenuItem.DropDownItems["thaiToolStripMenuItem"];
                    break;

                case "vi":
                    lang = (ToolStripMenuItem)this.languageToolStripMenuItem.DropDownItems["vietnameseToolStripMenuItem"];
                    break;

                case "hu":
                    lang = (ToolStripMenuItem)this.languageToolStripMenuItem.DropDownItems["hungarianToolStripMenuItem"];
                    break;

                case "tr":
                    lang = (ToolStripMenuItem)this.languageToolStripMenuItem.DropDownItems["turkishToolStripMenuItem"];
                    break;

                case "lt":
                    lang = (ToolStripMenuItem)this.languageToolStripMenuItem.DropDownItems["lithuaniaToolStripMenuItem"];
                    break;

                case "fa":
                    lang = (ToolStripMenuItem)this.languageToolStripMenuItem.DropDownItems["persianToolStripMenuItem"];
                    break;

                case "zh":
                    {
                        if (ci.EnglishName == "Chinese (Simplified)")
                            lang = (ToolStripMenuItem)this.languageToolStripMenuItem.DropDownItems["chineseSimplifiedToolStripMenuItem"];
                        else // Chinese (Traditional)
                            lang = (ToolStripMenuItem)this.languageToolStripMenuItem.DropDownItems["chineseTraditionalToolStripMenuItem"];
                    }
                    break;
                default:
                    lang = (ToolStripMenuItem)this.languageToolStripMenuItem.DropDownItems["englishToolStripMenuItem"];
                    break;
            }

            lang.Checked = true;

            this.ReloadControls();
        }

        /// <summary>
        /// Reloads controls from resource file
        /// </summary>
        private void ReloadControls()
        {
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

            if ((string)this.toolStripStatusLabel1.Tag == "mainScanningFinished")
                this.toolStripStatusLabel1.Text = Properties.Resources.mainScanningFinished;
            else if ((string)this.toolStripStatusLabel1.Tag == "mainScanningAborted")
                this.toolStripStatusLabel1.Text = Properties.Resources.mainScanningAborted;
            else
                this.toolStripStatusLabel1.Text = resources.GetString("toolStripStatusLabel1.Text");

            this.treeView1.Nodes.Clear();
            this.treeView1.Nodes.Add(resources.GetObject("treeView1.Nodes") as TreeNode);
            this.treeView1.ExpandAll();
        }
    }
}
