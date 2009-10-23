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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Little_Registry_Cleaner
{
    public partial class Options : Form
    {
        private ExcludeList.ExcludeArray _arrayExclude = new ExcludeList.ExcludeArray();

        public ExcludeList.ExcludeArray ExcludeArray
        {
            get { return _arrayExclude; }
        }

        public Options()
        {
            InitializeComponent();
        }

        private void Options_Load(object sender, EventArgs e)
        {
            // Load settings
            this.listViewOptions.Items[0].Checked = Properties.Settings.Default.bOptionsLog;
            this.listViewOptions.Items[1].Checked = Properties.Settings.Default.bOptionsRescan;
            this.listViewOptions.Items[2].Checked = Properties.Settings.Default.bOptionsAutoUpdate;
            this.listViewOptions.Items[3].Checked = Properties.Settings.Default.bOptionsRestore;
            this.listViewOptions.Items[4].Checked = Properties.Settings.Default.bOptionsShowLog;
            this.listViewOptions.Items[5].Checked = Properties.Settings.Default.bOptionsDelBackup;
            this.listViewOptions.Items[6].Checked = Properties.Settings.Default.bOptionsRemMedia; 

            // Load backup directorys
            this.textBoxBackupFolder.Text = Properties.Settings.Default.strOptionsBackupDir;
            this.textBoxLogFolder.Text = Properties.Settings.Default.strOptionsLogDir;

            // Load exclude list from settings
            if (Properties.Settings.Default.arrayExcludeList != null)
                this._arrayExclude = new ExcludeList.ExcludeArray(Properties.Settings.Default.arrayExcludeList);

            PopulateExcludeList();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // Update settings
            Properties.Settings.Default.bOptionsLog = this.listViewOptions.Items[0].Checked;
            Properties.Settings.Default.bOptionsRescan = this.listViewOptions.Items[1].Checked;
            Properties.Settings.Default.bOptionsAutoUpdate = this.listViewOptions.Items[2].Checked;
            Properties.Settings.Default.bOptionsRestore = this.listViewOptions.Items[3].Checked;
            Properties.Settings.Default.bOptionsShowLog = this.listViewOptions.Items[4].Checked;
            Properties.Settings.Default.bOptionsDelBackup = this.listViewOptions.Items[5].Checked;
            Properties.Settings.Default.bOptionsRemMedia = this.listViewOptions.Items[6].Checked;

            if (this.textBoxBackupFolder.Text != Properties.Settings.Default.strOptionsBackupDir)
                Properties.Settings.Default.strOptionsBackupDir = this.textBoxBackupFolder.Text;

            if (this.textBoxLogFolder.Text != Properties.Settings.Default.strOptionsLogDir)
                Properties.Settings.Default.strOptionsLogDir = this.textBoxLogFolder.Text;

            Properties.Settings.Default.arrayExcludeList = new ExcludeList.ExcludeArray(this._arrayExclude);

            this.Close();
        }

        #region General

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDlg = new FolderBrowserDialog())
            {
                folderBrowserDlg.Description = "Select the folder where the backup files will be placed";
                folderBrowserDlg.SelectedPath = this.textBoxBackupFolder.Text;
                folderBrowserDlg.ShowNewFolderButton = true;

                if (folderBrowserDlg.ShowDialog(this) == DialogResult.OK)
                    this.textBoxBackupFolder.Text = folderBrowserDlg.SelectedPath;
            }
        }

        private void buttonBrowse2_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDlg = new FolderBrowserDialog())
            {
                folderBrowserDlg.Description = "Select the folder where the log files will be placed";
                folderBrowserDlg.SelectedPath = this.textBoxLogFolder.Text;
                folderBrowserDlg.ShowNewFolderButton = true;

                if (folderBrowserDlg.ShowDialog(this) == DialogResult.OK)
                    this.textBoxLogFolder.Text = folderBrowserDlg.SelectedPath;
            }
        }

        #endregion

        #region Exclude List
        private void addRegistryPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExcludeList.AddRegistryPath addRegistryPath = new ExcludeList.AddRegistryPath();
            if (addRegistryPath.ShowDialog(this) == DialogResult.OK)
            {
                ExcludeArray.Add(new ExcludeList.ExcludeItem(addRegistryPath.RegistryPath, null, null));
                PopulateExcludeList();
            }
        }

        private void addFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    ExcludeArray.Add(new ExcludeList.ExcludeItem(null, null, openFileDialog.FileName));
                    PopulateExcludeList();
                }
            }
        }

        private void addFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Select the folder to exclude from the registry scan";

                if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
                {
                    ExcludeArray.Add(new ExcludeList.ExcludeItem(null, folderBrowserDialog.SelectedPath, null));
                    PopulateExcludeList();
                }
            }
        }

        private void removeEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedIndices.Count > 0 && this.listView1.Items.Count > 0)
            {
                if (MessageBox.Show(this, "Are you sure?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    foreach (ExcludeList.ExcludeItem i in ExcludeArray)
                    {
                        if (i.ToString() == this.listView1.SelectedItems[0].Text)
                        {
                            ExcludeArray.Remove(i);
                            break;
                        }
                    }

                    PopulateExcludeList();
                }
            }
        }

        private void PopulateExcludeList()
        {
            this.listView1.Items.Clear();

            foreach (ExcludeList.ExcludeItem item in ExcludeArray)
                this.listView1.Items.Add(item.ToString());

            this.listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }
        #endregion
    }
}
