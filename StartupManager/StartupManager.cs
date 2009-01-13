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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Common_Tools.TreeView;

namespace Little_Registry_Cleaner.StartupManager
{
    public partial class StartupManager : Form
    {
        public StartupManager()
        {
            InitializeComponent();
        }

        private void StartupManager_Load(object sender, EventArgs e)
        {
            LoadStartupFiles();
        }

        private void LoadRegistryAutoRun(RegistryKey regKey)
        {
            if (regKey == null)
                return;

            if (regKey.ValueCount <= 0)
                return;

            TreeListNode tlnRoot = new TreeListNode();
            tlnRoot.Text = regKey.Name;

            if (regKey.Name.StartsWith("HKEY_LOCAL_MACHINE"))
                tlnRoot.ImageIndex = 0;
            else
                tlnRoot.ImageIndex = 1;

            foreach (string strItem in regKey.GetValueNames())
            {
                string strFilePath = regKey.GetValue(strItem) as string;

                if (!string.IsNullOrEmpty(strFilePath))
                {
                    // Get file arguments
                    string strFile = "", strArgs = "";
                    if (!Utils.FileExists(strFilePath))
                    {
                        Utils.ExtractArguments(strFilePath, out strFile, out strArgs);

                        if (!Utils.FileExists(strFile))
                        {
                            strFile = strArgs = "";
                            Utils.ExtractFileLocation(strFilePath, out strFile, out strArgs);
                        }
                    }
                    else
                        strFile = string.Copy(strFilePath);

                    TreeListNode tln = new TreeListNode();
                    tln.Text = strItem;
                    tln.SubItems.Add(strFile);
                    tln.SubItems.Add(strArgs);

                    Icon ico = Utils.ExtractIcon(strFile);
                    if (ico != null)
                    {
                        this.imageList1.Images.Add(strFile, ico);
                        tln.ImageIndex = this.imageList1.Images.IndexOfKey(strFile);
                    } else
                        tln.ImageIndex = this.imageList1.Images.IndexOfKey("default");

                    tlnRoot.Nodes.Add(tln);
                }
            }

            this.treeListView.Nodes.Add(tlnRoot);
        }

        private void AddStartupFolder(string strFolder)
        {
            try
            {
                if (string.IsNullOrEmpty(strFolder) || !Directory.Exists(strFolder))
                    return;

                TreeListNode tlnRoot = new TreeListNode();
                tlnRoot.Text = strFolder;
                tlnRoot.ImageIndex = 2;

                foreach (string strShortcut in Directory.GetFiles(strFolder))
                {
                    string strShortcutName = Path.GetFileName(strShortcut);
                    string strFilePath, strFileArgs;

                    if (Path.GetExtension(strShortcut) == ".lnk")
                    {
                        Utils.ResolveShortcut(strShortcut, out strFilePath, out strFileArgs);

                        TreeListNode tln = new TreeListNode();
                        tln.Text = strShortcutName;
                        tln.SubItems.Add(strFilePath);
                        tln.SubItems.Add(strFileArgs);

                        Icon ico = Utils.ExtractIcon(strFilePath);
                        if (ico != null)
                        {
                            this.imageList1.Images.Add(strShortcutName, ico);
                            tln.ImageIndex = this.imageList1.Images.IndexOfKey(strShortcutName);
                        }
                        else
                            tln.ImageIndex = this.imageList1.Images.IndexOfKey("default");

                        tlnRoot.Nodes.Add(tln);
                    }
                }

                if (tlnRoot.Nodes.Count <= 0)
                    return;

                this.treeListView.Nodes.Add(tlnRoot);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

        }

        /// <summary>
        /// Loads files that load on startup
        /// </summary>
        private void LoadStartupFiles()
        {
            this.treeListView.Nodes.Clear();

            if (!this.imageList1.Images.ContainsKey("default"))
                this.imageList1.Images.Add("default", SystemIcons.Application);

            try
            {
                // all user keys
                LoadRegistryAutoRun(Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer\\Run"));
                LoadRegistryAutoRun(Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunServicesOnce"));
                LoadRegistryAutoRun(Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunServices"));
                LoadRegistryAutoRun(Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnceEx"));
                LoadRegistryAutoRun(Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce\\Setup"));
                LoadRegistryAutoRun(Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce"));
                LoadRegistryAutoRun(Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunEx"));
                LoadRegistryAutoRun(Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run"));

                // current user keys
                LoadRegistryAutoRun(Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer\\Run"));
                LoadRegistryAutoRun(Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunServicesOnce"));
                LoadRegistryAutoRun(Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunServices"));
                LoadRegistryAutoRun(Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnceEx"));
                LoadRegistryAutoRun(Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce\\Setup"));
                LoadRegistryAutoRun(Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce"));
                LoadRegistryAutoRun(Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunEx"));
                LoadRegistryAutoRun(Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run"));
            }
            catch (System.Security.SecurityException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            AddStartupFolder(Utils.GetSpecialFolderPath(Utils.CSIDL_STARTUP));
            AddStartupFolder(Utils.GetSpecialFolderPath(Utils.CSIDL_COMMON_STARTUP));

            this.treeListView.ExpandAll();
            this.treeListView.AutoResizeColumns();
        }

        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            LoadStartupFiles();
        }

        private void toolStripButtonAdd_Click(object sender, EventArgs e)
        {
            NewRunItem nrv = new NewRunItem();
            if (nrv.ShowDialog(this) == DialogResult.OK)
                this.LoadStartupFiles();
        }

        private void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            if (this.treeListView.SelectedNodes.Count > 0)
            {
                if (MessageBox.Show(this, "Are you sure you want to remove this startup program?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    TreeListNode tln = this.treeListView.SelectedNodes[0];

                    if (tln.SubItems.Count > 0)
                    {
                        string strItem = tln.Text;
                        string strFilePath = tln.SubItems[0].Text;
                        string strArgs = tln.SubItems[1].Text;

                        if (tln.Parent.Text.StartsWith("HKEY"))
                        {
                            string strRegPath = tln.Parent.Text;
                            string strMainKey = strRegPath.Substring(0, strRegPath.IndexOf('\\'));
                            string strSubKey = strRegPath.Substring(strRegPath.IndexOf('\\') + 1);

                            RegistryKey rk = Utils.RegOpenKey(strMainKey, strSubKey, true);

                            if (rk != null)
                            {
                                rk.DeleteValue(strItem);
                                rk.Close();
                            }
                        }
                        else if (Directory.Exists(tln.Parent.Text))
                        {
                            string strPath = Path.Combine(tln.Parent.Text, strItem);

                            if (File.Exists(strPath))
                                File.Delete(strPath);
                        }
                    }

                }
            }

            LoadStartupFiles();
        }

        private void toolStripButtonEdit_Click(object sender, EventArgs e)
        {
            if (this.treeListView.SelectedNodes.Count > 0)
            {
                string strItem = this.treeListView.SelectedNodes[0].Text;
                string strSection = this.treeListView.SelectedNodes[0].Parent.Text;
                string strFilepath = this.treeListView.SelectedNodes[0].SubItems[0].Text;
                string strFileArgs = this.treeListView.SelectedNodes[0].SubItems[1].Text;

                EditRunItem dlgEditRunValue = new EditRunItem(strItem, strSection, strFilepath, strFileArgs);
                if (dlgEditRunValue.ShowDialog(this) == DialogResult.OK)
                    LoadStartupFiles();
            }
        }

        private void toolStripButtonView_Click(object sender, EventArgs e)
        {
            if (this.treeListView.SelectedNodes.Count > 0)
            {
                string strItem = this.treeListView.SelectedNodes[0].Text;

                string strPath = this.treeListView.SelectedNodes[0].Parent.Text;

                if (strPath.StartsWith("HKEY"))
                {
                    RegEditGo.GoTo(strPath, strItem);
                }
                else
                {
                    System.Diagnostics.Process.Start("explorer.exe", strPath);
                }
            }
        }

        private void toolStripButtonRun_Click(object sender, EventArgs e)
        {
            if (this.treeListView.SelectedNodes.Count > 0)
            {
                if (MessageBox.Show(this, "Running this program on vista will give it this programs priviledges\r\nAre you sure you want to continue?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string strFilepath = this.treeListView.SelectedNodes[0].SubItems[0].Text;
                    string strFileArgs = this.treeListView.SelectedNodes[0].SubItems[1].Text;

                    System.Diagnostics.Process.Start(strFilepath, strFileArgs);

                    MessageBox.Show(this, "Attempted to run: " + strFilepath, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
