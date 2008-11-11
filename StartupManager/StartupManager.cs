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

            ImageList imageList = new ImageList();

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
                string strFile = regKey.GetValue(strItem) as string;

                if (!string.IsNullOrEmpty(strFile))
                {
                    string strRegKey = regKey.Name;
                    string strMainKey = strRegKey.Substring(0, strRegKey.IndexOf("\\"));
                    string strPath = strRegKey.Substring(strRegKey.LastIndexOf("\\") + 1);

                    string strShortKey = ShortRegKey(strMainKey, strPath);

                    ListViewItem listViewItem = new ListViewItem(new string[] { strItem, strShortKey, strFile });
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
                    FileInfo fileInfo = new FileInfo(strShortcut);

                    if (fileInfo.Name == "desktop.ini")
                        continue;

                    string strFilePath = Utils.ResolveShortcut(strShortcut);

                    ListViewItem listViewItem = new ListViewItem(new string[] { fileInfo.Name, strShortFolder, strFilePath });
                    listViewItem.Checked = true;
                    this.listView1.Items.Add(listViewItem);
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

            if (e.Node.Name == "NodeRegAll")
            {
                foreach (ListViewItem lvi in this.listView1.Items)
                {
                    if (!lvi.SubItems[1].Text.StartsWith(@"Registry\All Users"))
                        lvi.Remove();
                }
            }
            else if (e.Node.Name == "NodeRegCurrent")
            {
                foreach (ListViewItem lvi in this.listView1.Items)
                {
                    if (!lvi.SubItems[1].Text.StartsWith(@"Registry\Current User"))
                        lvi.Remove();
                }
            }
            else if (e.Node.Name == "NodeStartAll")
            {
                foreach (ListViewItem lvi in this.listView1.Items)
                {
                    if (!lvi.SubItems[1].Text.StartsWith(@"StartUp\All Users"))
                        lvi.Remove();
                }
            }
            else if (e.Node.Name == "NodeStartCurrent")
            {
                foreach (ListViewItem lvi in this.listView1.Items)
                {
                    if (!lvi.SubItems[1].Text.StartsWith(@"StartUp\Current User"))
                        lvi.Remove();
                }
            }
        }

        private void toolStripButtonAdd_Click(object sender, EventArgs e)
        {
            NewRunValue nrv = new NewRunValue();
            if (nrv.ShowDialog(this) == DialogResult.OK)
                this.LoadStartupFiles();
        }

        private void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure you want to remove this startup program?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {

            }
        }
    }
}
