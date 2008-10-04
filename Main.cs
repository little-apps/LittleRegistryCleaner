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
        public Main()
        {
            InitializeComponent();
        }
      
        private void scanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScanRegistry();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Options frmOptions = new Options();
            frmOptions.ShowDialog(this);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About frmAbout = new About();
            frmAbout.ShowDialog(this);
        }

        private void fixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FixProblems();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void restoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RestoreRegistry();
        }

        private void toolStripButtonScan_Click(object sender, EventArgs e)
        {
            ScanRegistry();
        }

        private void toolStripButtonFix_Click(object sender, EventArgs e)
        {
            FixProblems();
        }

        private void toolStripButtonSettings_Click(object sender, EventArgs e)
        {
            Options frmOptions = new Options();
            frmOptions.ShowDialog(this);
        }

        /// <summary>
        /// Begins scanning the registry
        /// </summary>
        private void ScanRegistry()
        {
            int nSectionCount = 0;

            // Clear old results
            this.listResults.Items.Clear();

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
            frmScanBox.UpdateListView += new ScanDlg.UpdateListViewHandler(frmScanBox_UpdateListView);
            if (frmScanBox.ShowDialog(this) == DialogResult.OK)
            {
                // Notify user using notify icon
                this.notifyIcon1.BalloonTipTitle = Application.ProductName;
                this.notifyIcon1.BalloonTipText = string.Format("Found {0} Problems", this.listResults.Items.Count);
                this.notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                this.notifyIcon1.ShowBalloonTip(5000);
            }

            // Enable menu items
            if (this.listResults.Items.Count > 0)
            {
                this.fixToolStripMenuItem.Enabled = true;
                this.toolStripButtonFix.Enabled = true;
            }
        }

        void frmScanBox_UpdateListView(string strProblem, string strData, string strValue)
        {
            // Add data to listview
            ListViewItem listViewItem = new ListViewItem();

            listViewItem.Text = strProblem;
            listViewItem.SubItems.Add(strData);

            if (!string.IsNullOrEmpty(strValue))
                listViewItem.SubItems.Add(strValue);

            listViewItem.Checked = true;

            this.listResults.Items.Add(listViewItem);

            // Resize columns to fit data
            this.listResults.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }
        
        /// <summary>
        /// If problems were found, removes them from registry
        /// </summary>
        private void FixProblems()
        {
            xmlRegistry xmlReg = new xmlRegistry();

            if (this.listResults.Items.Count > 0)
            {
                if (MessageBox.Show(this, "Would you like to fix all selected problems?", "Little Registry Cleaner", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // Generate filename to backup registry
                    string strBackupFile = string.Format("{0}\\{1:yyyy}_{1:MM}_{1:dd}_{1:HH}{1:mm}{1:ss}.xml", Properties.Settings.Default.strOptionsBackupDir, DateTime.Now);

                    // Generate a restore file and delete keys & values
                    xmlReg.deleteAsXml(this.listResults, strBackupFile);

                    // Disable menu items
                    this.fixToolStripMenuItem.Enabled = false;
                    this.toolStripButtonFix.Enabled = false;

                    MessageBox.Show(this, "Removed problems from registry", "Little Registry Cleaner", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Clear old results
                    this.listResults.Items.Clear();

                    // Scan again
                    if (Properties.Settings.Default.bOptionsRescan)
                        ScanRegistry();
                }
            }
        }

        /// <summary>
        /// Opens restore dialog
        /// </summary>
        private void RestoreRegistry()
        {
            Restore RestoreDlg = new Restore();
            RestoreDlg.ShowDialog(this);
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
        }

        private void toolStripButtonRestore_Click(object sender, EventArgs e)
        {
            RestoreRegistry();
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // If My Computer is changed, set all to true/false
            if (e.Node.Name.CompareTo("Node0") == 0)
            {
                for (int i = 0; i < e.Node.Nodes.Count; i++)
                    e.Node.Nodes[i].Checked = e.Node.Checked;

                Properties.Settings.Default.bScanActivex = e.Node.Checked;
                Properties.Settings.Default.bScanStartup = e.Node.Checked;
                Properties.Settings.Default.bScanFonts = e.Node.Checked;
                Properties.Settings.Default.bScanAppInfo = e.Node.Checked;
                Properties.Settings.Default.bScanDrivers = e.Node.Checked;
                Properties.Settings.Default.bScanHelpFiles = e.Node.Checked;
                Properties.Settings.Default.bScanSounds = e.Node.Checked;
                Properties.Settings.Default.bScanAppPaths = e.Node.Checked;
                Properties.Settings.Default.bScanAppSettings = e.Node.Checked;
                Properties.Settings.Default.bScanSharedDLL = e.Node.Checked;
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
                Properties.Settings.Default.bScanActivex = e.Node.Checked;
            else if (e.Node.Name.CompareTo("NodeStartup") == 0)
                Properties.Settings.Default.bScanStartup = e.Node.Checked;
            else if (e.Node.Name.CompareTo("NodeFonts") == 0)
                Properties.Settings.Default.bScanFonts = e.Node.Checked;
            else if (e.Node.Name.CompareTo("NodeAppInfo") == 0)
                Properties.Settings.Default.bScanAppInfo = e.Node.Checked;
            else if (e.Node.Name.CompareTo("NodeDrivers") == 0)
                Properties.Settings.Default.bScanDrivers = e.Node.Checked;
            else if (e.Node.Name.CompareTo("NodeHelp") == 0)
                Properties.Settings.Default.bScanHelpFiles = e.Node.Checked;
            else if (e.Node.Name.CompareTo("NodeSounds") == 0)
                Properties.Settings.Default.bScanSounds = e.Node.Checked;
            else if (e.Node.Name.CompareTo("NodeAppPaths") == 0)
                Properties.Settings.Default.bScanAppPaths = e.Node.Checked;
            else if (e.Node.Name.CompareTo("NodeAppSettings") == 0)
                Properties.Settings.Default.bScanAppSettings = e.Node.Checked;
            else if (e.Node.Name.CompareTo("NodeSharedDlls") == 0)
                Properties.Settings.Default.bScanSharedDLL = e.Node.Checked;
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.listResults.Items.Count; i++)
                this.listResults.Items[i].Checked = true;
        }

        private void selectNoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.listResults.Items.Count; i++)
                this.listResults.Items[i].Checked = false;
        }

        private void excludeSelectedToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void visitWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://sourceforge.net/projects/littlecleaner/");
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

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                About dlgAbout = new About();
                dlgAbout.ShowDialog(this);
            }
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (MessageBox.Show(this, "Are you sure?", "Little Registry Cleaner", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    e.Cancel = true;
            }
        }

        private void toolStripAbout_Click(object sender, EventArgs e)
        {
            About aboutDlg = new About();
            aboutDlg.ShowDialog(this);
        }

        private void viewInRegeditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listResults.SelectedIndices.Count > 0 && this.listResults.Items.Count > 0)
            {
                RegEditGo(this.listResults.SelectedItems[0].SubItems[1].Text);
            }
        }

        /// <summary>
        /// Goes to registry key in regedit
        /// </summary>
        /// <param name="strRegistryPath">Registry key</param>
        public static void RegEditGo(string strRegistryPath)
        {
            // Set key to be displayed when regedit starts
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Applets\\Regedit", true);
            if (regKey != null)
            {
                regKey.SetValue("LastKey", "Computer\\" + strRegistryPath);
                regKey.Close();
            }

            // See if regedit is running, restart it if it is
            foreach (Process p in Process.GetProcessesByName("regedit"))
            {
                p.WaitForInputIdle();
                p.Kill();
                p.Close();
            }

            Process.Start("regedit.exe");
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Process.Start("http://answers.launchpad.net/lilregcleaner");
        }
    }
}
