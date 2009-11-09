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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Common_Tools.TreeViewAdv.Tree;

namespace Little_Registry_Cleaner.StartupManager
{
    public partial class StartupManager : Form
    {
        private TreeModel treeModel = new TreeModel();

        public StartupManager()
        {
            InitializeComponent();
        }

        private void StartupManager_Load(object sender, EventArgs e)
        {
            this.treeViewAdv1.Model = this.treeModel;
            LoadStartupFiles();
        }

        /// <summary>
        /// Loads files that load on startup
        /// </summary>
        private void LoadStartupFiles()
        {
            // Clear old list
            this.treeModel.Nodes.Clear();

            // Adds registry keys
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

            // Adds startup folders
            AddStartupFolder(Utils.GetSpecialFolderPath(Utils.CSIDL_STARTUP));
            AddStartupFolder(Utils.GetSpecialFolderPath(Utils.CSIDL_COMMON_STARTUP));

            // Expands treeview
            this.treeViewAdv1.ExpandAll();
            this.treeViewAdv1.AutoSizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        /// <summary>
        /// Loads registry sub key into tree view
        /// </summary>
        private void LoadRegistryAutoRun(RegistryKey regKey)
        {
            if (regKey == null)
                return;

            if (regKey.ValueCount <= 0)
                return;

            StartupManagerNode nodeRoot = new StartupManagerNode(regKey.Name);

            if (regKey.Name.Contains(Registry.CurrentUser.ToString()))
                nodeRoot.Image = this.imageList.Images["User"];
            else
                nodeRoot.Image = this.imageList.Images["Users"];

            foreach (string strItem in regKey.GetValueNames())
            {
                string strFilePath = regKey.GetValue(strItem) as string;

                if (!string.IsNullOrEmpty(strFilePath))
                {
                    // Get file arguments
                    string strFile = "", strArgs = "";

                    if (Utils.FileExists(strFilePath))
                        strFile = strFilePath;
                    else
                    {
                        if (!Utils.ExtractArguments(strFilePath, out strFile, out strArgs))
                            if (!Utils.ExtractArguments2(strFilePath, out strFile, out strArgs))
                                // If command line cannot be extracted, set file path to command line
                                strFile = strFilePath;
                    }

                    StartupManagerNode node = new StartupManagerNode();

                    node.Item = strItem;
                    node.Path = strFile;
                    node.Args = strArgs;

                    Icon ico = Utils.ExtractIcon(strFile);
                    if (ico != null)
                        node.Image = (Image)ico.ToBitmap().Clone();

                    nodeRoot.Nodes.Add(node);
                }
            }

            this.treeModel.Nodes.Add(nodeRoot);
        }

        /// <summary>
        /// Loads startup folder into tree view
        /// </summary>
        private void AddStartupFolder(string strFolder)
        {
            try
            {
                if (string.IsNullOrEmpty(strFolder) || !Directory.Exists(strFolder))
                    return;

                StartupManagerNode nodeRoot = new StartupManagerNode(strFolder);

                if (Utils.GetSpecialFolderPath(Utils.CSIDL_STARTUP) == strFolder)
                    nodeRoot.Image = this.imageList.Images["Users"];
                else
                    nodeRoot.Image = this.imageList.Images["User"];

                foreach (string strShortcut in Directory.GetFiles(strFolder))
                {
                    string strShortcutName = Path.GetFileName(strShortcut);
                    string strFilePath, strFileArgs;

                    if (Path.GetExtension(strShortcut) == ".lnk")
                    {
                        if (!Utils.ResolveShortcut(strShortcut, out strFilePath, out strFileArgs))
                            continue;

                        StartupManagerNode node = new StartupManagerNode();
                        node.Item = strShortcutName;
                        node.Path = strFilePath;
                        node.Args = strFileArgs;

                        Icon ico = Utils.ExtractIcon(strFilePath);
                        if (ico != null)
                            node.Image = (Image)ico.ToBitmap().Clone();

                        nodeRoot.Nodes.Add(node);
                    }
                }

                if (nodeRoot.Nodes.Count <= 0)
                    return;

                this.treeModel.Nodes.Add(nodeRoot);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

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
            if (this.treeViewAdv1.SelectedNodes.Count > 0)
            {
                if (MessageBox.Show(this, Properties.Resources.smRemove, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    StartupManagerNode node = this.treeViewAdv1.SelectedNode.Tag as StartupManagerNode;

                    bool bFailed = false;

                    if (!node.IsLeaf)
                        return;

                    string strSection = (node.Parent as StartupManagerNode).Section;

                    if (Directory.Exists(strSection))
                    {
                        // Startup folder
                        string strPath = Path.Combine(strSection, node.Item);

                        try
                        {
                            if (File.Exists(strPath))
                                File.Delete(strPath);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            bFailed = true;
                        }
                    }
                    else
                    {
                        // Registry key
                        string strMainKey = strSection.Substring(0, strSection.IndexOf('\\'));
                        string strSubKey = strSection.Substring(strSection.IndexOf('\\') + 1);
                        RegistryKey rk = Utils.RegOpenKey(strMainKey, strSubKey);

                        try
                        {
                            if (rk != null)
                                rk.DeleteValue(node.Item);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            bFailed = true;
                        }

                        rk.Close();
                    }

                    if (!bFailed)
                        MessageBox.Show(this, Properties.Resources.smRemoved, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }

            LoadStartupFiles();
        }

        private void toolStripButtonEdit_Click(object sender, EventArgs e)
        {
            if (this.treeViewAdv1.SelectedNodes.Count > 0)
            {
                StartupManagerNode node = this.treeViewAdv1.SelectedNode.Tag as StartupManagerNode;

                if (!node.IsLeaf)
                    return;

                string strSection = (node.Parent as StartupManagerNode).Section;

                EditRunItem frmEditRunItem = new EditRunItem(node.Item, strSection, node.Path, node.Args);
                if (frmEditRunItem.ShowDialog(this) == DialogResult.OK)
                    LoadStartupFiles();
            }
        }

        private void toolStripButtonView_Click(object sender, EventArgs e)
        {
            if (this.treeViewAdv1.SelectedNodes.Count > 0)
            {
                if (!(this.treeViewAdv1.SelectedNode.Tag as StartupManagerNode).IsLeaf)
                    return;

                string strItem = (this.treeViewAdv1.SelectedNode.Tag as StartupManagerNode).Item;
                string strPath = (this.treeViewAdv1.SelectedNode.Parent.Tag as StartupManagerNode).Section;

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
            if (this.treeViewAdv1.SelectedNodes.Count > 0)
            {
                if (!(this.treeViewAdv1.SelectedNode.Tag as StartupManagerNode).IsLeaf)
                    return;

                if (MessageBox.Show(this, Properties.Resources.smRun, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string strFilepath = (this.treeViewAdv1.SelectedNode.Tag as StartupManagerNode).Path;
                    string strFileArgs = (this.treeViewAdv1.SelectedNode.Tag as StartupManagerNode).Args;

                    // File path cannot be empty
                    if (string.IsNullOrEmpty(strFilepath))
                        return;

                    System.Diagnostics.Process.Start(strFilepath, strFileArgs);

                    MessageBox.Show(this, string.Format("{0}: {1}", Properties.Resources.smRun, strFilepath), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }

    #region "Startup Manager Node"
    public class StartupManagerNode : Node
    {
        private Image img = null;
        public new Image Image
        {
            get { return img; }
            set { img = value; }
        }

        private string strSection = "";
        public string Section
        {
            get { return strSection; }
            set { strSection = value; }
        }

        private string strItem = "";
        public string Item
        {
            get { return strItem; }
            set { strItem = value; }
        }

        private string strPath = "";
        public string Path
        {
            get { return strPath; }
            set { strPath = value; }
        }

        private string strArgs = "";
        public string Args
        {
            get { return strArgs; }
            set { strArgs = value; }
        }

        private string _path = "";
        public string ItemPath
        {
            get { return _path; }
            set { _path = value; }
        }

        public override bool IsLeaf
        {
            get
            {
                return (string.IsNullOrEmpty(strSection));
            }
        }

        public StartupManagerNode()
            : base()
        {
        }

        public StartupManagerNode(string SectionName)
            : base()
        {
            this.strSection = SectionName;
        }
    }
    #endregion
}
