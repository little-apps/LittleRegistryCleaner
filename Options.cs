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

namespace Little_Registry_Cleaner
{
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();
        }

        
        private void Options_Load(object sender, EventArgs e)
        {
            // Load settings
            this.checkBoxDeleteBackup.Checked = Properties.Settings.Default.bOptionsDelBackup;
            this.checkBoxRescan.Checked = Properties.Settings.Default.bOptionsRescan;
            this.checkBoxLog.Checked = Properties.Settings.Default.bOptionsLog;
            this.checkBoxRestore.Checked = Properties.Settings.Default.bOptionsRestore;
            this.checkBoxAutoUpdate.Checked = Properties.Settings.Default.bOptionsAutoUpdate;

            // Load backup directorys
            this.textBoxBackupFolder.Text = Properties.Settings.Default.strOptionsBackupDir;
            this.textBoxLogFolder.Text = Properties.Settings.Default.strOptionsLogDir;

            // Load exclude list from settings
            if (Properties.Settings.Default.arrayOptionsExcludeList == null)
                Properties.Settings.Default.arrayOptionsExcludeList = new System.Collections.ArrayList();

            for (int i = 0; i < Properties.Settings.Default.arrayOptionsExcludeList.Count; i++)
            {
                string[] strExclude = (string[])Little_Registry_Cleaner.Properties.Settings.Default.arrayOptionsExcludeList[i];

                string strSection = strExclude[0];
                string strSubkey = strExclude[1];

                this.listView1.Items.Add(new ListViewItem(new string[] { strSection, strSubkey }));
            }

            this.listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }


        private void buttonOK_Click(object sender, EventArgs e)
        {
            // Update settings
            Properties.Settings.Default.bOptionsRescan = this.checkBoxRescan.Checked;
            Properties.Settings.Default.bOptionsDelBackup = this.checkBoxDeleteBackup.Checked;
            Properties.Settings.Default.bOptionsLog = this.checkBoxLog.Checked;
            Properties.Settings.Default.bOptionsRestore = this.checkBoxRestore.Checked;
            Properties.Settings.Default.bOptionsAutoUpdate = this.checkBoxAutoUpdate.Checked;

            if (!string.IsNullOrEmpty(this.textBoxBackupFolder.Text))
                Properties.Settings.Default.strOptionsBackupDir = this.textBoxBackupFolder.Text;
            else
                Properties.Settings.Default.strOptionsBackupDir = string.Format("{0}\\Backups", Properties.Settings.Default.strProgramSettingsDir);

            if (!string.IsNullOrEmpty(this.textBoxLogFolder.Text))
                Properties.Settings.Default.strOptionsLogDir = this.textBoxLogFolder.Text;
            else
                Properties.Settings.Default.strOptionsLogDir = string.Format("{0}\\Logs", Properties.Settings.Default.strProgramSettingsDir);

            // Update exclude list in settings
            Properties.Settings.Default.arrayOptionsExcludeList.Clear();
            
            for (int i = 0; i < this.listView1.Items.Count ;i++) 
            {
                ListViewItem listViewItem = this.listView1.Items[i];

                string strSection = listViewItem.SubItems[0].Text;
                string strSubkey = listViewItem.SubItems[1].Text;

                Properties.Settings.Default.arrayOptionsExcludeList.Add(new string[] { strSection, strSubkey });
            }

            this.Close();
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog1.Description = "Select the folder where the backup files will be placed";
            this.folderBrowserDialog1.SelectedPath = this.textBoxBackupFolder.Text;
            this.folderBrowserDialog1.ShowNewFolderButton = true;

            if (this.folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
                this.textBoxBackupFolder.Text = this.folderBrowserDialog1.SelectedPath;
        }

        private void addEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewExcludeEntryDlg frmNewExcludeEntry = new NewExcludeEntryDlg();
            frmNewExcludeEntry.NewExcludeEntry += new NewExcludeEntryDlg.NewExcludeEntryHandler(frmNewExcludeEntry_NewExcludeEntry);
            frmNewExcludeEntry.ShowDialog(this);
        }

        void frmNewExcludeEntry_NewExcludeEntry(string strRootKey, string strPath)
        {
            this.listView1.Items.Add(new ListViewItem(new string[] { strRootKey, strPath }));
            this.listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void removeEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedIndices.Count > 0 && this.listView1.Items.Count > 0)
            {
                if (MessageBox.Show(this, "Are you sure?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    this.listView1.Items[this.listView1.SelectedIndices[0]].Remove();
            }
        }

        private void buttonBrowse2_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog1.Description = "Select the folder where the log files will be placed";
            this.folderBrowserDialog1.SelectedPath = this.textBoxLogFolder.Text;
            this.folderBrowserDialog1.ShowNewFolderButton = true;

            if (this.folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
                this.textBoxLogFolder.Text = this.folderBrowserDialog1.SelectedPath;
        }
    }
}
