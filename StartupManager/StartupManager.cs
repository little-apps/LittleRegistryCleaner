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
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

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
            this.treeView1.Nodes[0].ExpandAll();

            LoadStartupFiles();
        }

        private void LoadRegistryAutoRun(RegistryKey regKey)
        {
            if (regKey == null)
                return;

            if (regKey.ValueCount < 1)
                return;

            foreach (string strItem in regKey.GetValueNames())
            {
                string strFilePath = regKey.GetValue(strItem) as string;

                if (!string.IsNullOrEmpty(strFilePath))
                {
                    // Convert HKLM\Software\Microsoft\... to Registry\All Users
                    string strRegKey = regKey.Name;
                    string strMainKey = strRegKey.Substring(0, strRegKey.IndexOf("\\"));
                    string strPath = strRegKey.Substring(strRegKey.LastIndexOf("\\") + 1);

                    string strShortKey = ShortRegKey(strMainKey, strPath);

                    // Get file arguments
                    string strFile, strArgs;
                    Utils.ExtractArguments(strFilePath, out strFile, out strArgs);

                    ListViewItem listViewItem = new ListViewItem(new string[] { strItem, strShortKey, strFile , strArgs});
                    listViewItem.Checked = true;
                    this.listView1.Items.Add(listViewItem);
                }
            }
        }

        private void AddStartupFolder(string strFolder, string strShortFolder)
        {
            try
            {
                if (string.IsNullOrEmpty(strFolder))
                    return;

                if (!Directory.Exists(strFolder))
                    return;

                foreach (string strShortcut in Directory.GetFiles(strFolder))
                {
                    string strShortcutName = Path.GetFileName(strShortcut);
                    string strFilePath, strFileArgs;

                    if (strShortcutName != "desktop.ini")
                    {
                        Utils.ResolveShortcut(strShortcut, out strFilePath, out strFileArgs);

                        ListViewItem listViewItem = new ListViewItem(new string[] { strShortcutName, strShortFolder, strFilePath, strFileArgs });
                        listViewItem.Checked = true;
                        this.listView1.Items.Add(listViewItem);
                    }
                }
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
            this.listView1.Items.Clear();

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

            AddStartupFolder(Utils.GetSpecialFolderPath(Utils.CSIDL_STARTUP), @"StartUp\All Users");
            AddStartupFolder(Utils.GetSpecialFolderPath(Utils.CSIDL_COMMON_STARTUP), @"StartUp\Current User");

            this.listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        

        private static string ShortRegKey(string strMainKey, string strPath)
        {
            string strShortName = "";

            if (strMainKey.ToUpper().CompareTo("HKEY_CURRENT_USER") == 0)
                strShortName = string.Format(@"Registry\Current User\{0}", strPath);
            else if (strMainKey.ToUpper().CompareTo("HKEY_LOCAL_MACHINE") == 0)
                strShortName = string.Format(@"Registry\All Users\{0}", strPath);
            else
                return strShortName; // break here

            return strShortName;
        }

        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            LoadStartupFiles();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Reload list
            LoadStartupFiles();


            if (e.Node.Name == "NodeReg")
            {
                RemoveItemsStartingWith(@"Registry\");
            }
            else if (e.Node.Name == "NodeStart")
            {
                RemoveItemsStartingWith(@"StartUp\");
            }
            else if (e.Node.Name == "NodeRegAll")
            {
                RemoveItemsStartingWith(@"Registry\All Users");
            }
            else if (e.Node.Name == "NodeRegCurrent")
            {
                RemoveItemsStartingWith(@"Registry\Current User");
            }
            else if (e.Node.Name == "NodeStartAll")
            {
                RemoveItemsStartingWith(@"StartUp\All Users");
            }
            else if (e.Node.Name == "NodeStartCurrent")
            {
                RemoveItemsStartingWith(@"StartUp\Current User");
            }
        }

        private void RemoveItemsStartingWith(string strText)
        {
            foreach (ListViewItem lvi in this.listView1.Items)
            {
                if (!lvi.SubItems[1].Text.StartsWith(strText))
                    lvi.Remove();
            }
        }

        private void toolStripButtonAdd_Click(object sender, EventArgs e)
        {
            NewRunItem nrv = new NewRunItem();
            if (nrv.ShowDialog(this) == DialogResult.OK)
                this.LoadStartupFiles();
        }

        private void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count > 0)
            {
                if (MessageBox.Show(this, "Are you sure you want to remove this startup program?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    foreach (ListViewItem lvi in this.listView1.SelectedItems)
                    {
                        string strItem = lvi.SubItems[0].Text;
                        string strSection = lvi.SubItems[1].Text;
                        string strFilepath = lvi.SubItems[2].Text;

                        if (strSection.StartsWith(@"Registry\"))
                        {
                            RegistryKey regKey = null;
                            string strRegPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\" + strSection.Substring(strSection.LastIndexOf("\\") + 1);

                            if (strSection.StartsWith(@"Registry\All Users"))
                            {
                                regKey = Registry.LocalMachine.OpenSubKey(strRegPath, true);
                            }
                            else if (strSection.StartsWith(@"Registry\Current User"))
                            {
                                regKey = Registry.CurrentUser.OpenSubKey(strRegPath, true);
                            }

                            regKey.DeleteValue(strItem, false);
                            regKey.Close();
                        }
                        else
                        {
                            string strStartupFolder = "";

                            if (strSection == @"StartUp\All Users")
                            {
                                strStartupFolder = Utils.GetSpecialFolderPath(Utils.CSIDL_STARTUP);
                            }
                            else if (strSection == @"StartUp\Current User")
                            {
                                strStartupFolder = Utils.GetSpecialFolderPath(Utils.CSIDL_COMMON_STARTUP);
                            }

                            File.Delete(string.Format("{0}\\{1}", strStartupFolder, strItem));
                        }
                    }

                    LoadStartupFiles();
                }
            }
        }

        private void toolStripButtonEdit_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count > 0)
            {
                string strItem = this.listView1.SelectedItems[0].SubItems[0].Text;
                string strSection = this.listView1.SelectedItems[0].SubItems[1].Text;
                string strFilepath = this.listView1.SelectedItems[0].SubItems[2].Text;
                string strFileArgs = this.listView1.SelectedItems[0].SubItems[3].Text;

                EditRunItem dlgEditRunValue = new EditRunItem(strItem, strSection, strFilepath, strFileArgs);
                if (dlgEditRunValue.ShowDialog(this) == DialogResult.OK)
                    LoadStartupFiles();
            }
        }

        private void toolStripButtonView_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count > 0)
            {
                string strItem = this.listView1.SelectedItems[0].SubItems[0].Text;
                string strSection = this.listView1.SelectedItems[0].SubItems[1].Text;

                if (strSection.StartsWith(@"Registry\"))
                {
                    RegistryKey regKey = null;
                    string strRegPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\" + strSection.Substring(strSection.LastIndexOf("\\") + 1);

                    if (strSection.StartsWith(@"Registry\All Users"))
                    {
                        regKey = Registry.LocalMachine.OpenSubKey(strRegPath, true);
                    }
                    else if (strSection.StartsWith(@"Registry\Current User"))
                    {
                        regKey = Registry.CurrentUser.OpenSubKey(strRegPath, true);
                    }

                    if (regKey != null)
                    {
                        Utils.RegEditGo(regKey.ToString());
                        regKey.Close();
                    }
                }
                else
                {
                    string strStartupFolder = "";

                    if (strSection == @"StartUp\All Users")
                    {
                        strStartupFolder = Utils.GetSpecialFolderPath(Utils.CSIDL_STARTUP);
                    }
                    else if (strSection == @"StartUp\Current User")
                    {
                        strStartupFolder = Utils.GetSpecialFolderPath(Utils.CSIDL_COMMON_STARTUP);
                    }

                    System.Diagnostics.Process.Start("explorer.exe", strStartupFolder);
                }
            }
        }

        private void StartupManager_Resize(object sender, EventArgs e)
        {
            if (this.listView1.Items.Count > 0)
                this.listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void toolStripButtonRun_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count > 0)
            {
                if (MessageBox.Show(this, "Running this program on vista will give it this programs priviledges\r\nAre you sure you want to continue?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string strFilepath = this.listView1.SelectedItems[0].SubItems[2].Text;
                    string strFileArgs = this.listView1.SelectedItems[0].SubItems[3].Text;

                    System.Diagnostics.Process.Start(strFilepath, strFileArgs);

                    MessageBox.Show(this, "Attempted to run: " + strFilepath, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
