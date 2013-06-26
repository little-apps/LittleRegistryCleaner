/*
    Little Registry Cleaner
    Copyright (C) 2008 Little Apps (http://www.little-apps.org/)

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
using System.Diagnostics;
using Little_Registry_Cleaner.Xml;

namespace Little_Registry_Cleaner
{
    public partial class Restore : Form
    {
        public Restore()
        {
            InitializeComponent();
        }

        private xmlReader xmlReader = new xmlReader();
        private xmlRegistry xmlReg = new xmlRegistry();

        private void Restore_Load(object sender, EventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(Properties.Settings.Default.strOptionsBackupDir);

            foreach (FileInfo fi in di.GetFiles()) {
                if (fi.Extension.CompareTo(".xml") == 0)
                {
                    ListViewItem lvi = new ListViewItem(new string[] { fi.Name, fi.CreationTime.ToString(), Utils.ConvertSizeToString((uint)fi.Length)});
                    this.listViewFiles.Items.Add(lvi);
                }
            }

            if (this.listViewFiles.Items.Count > 0)
                this.listViewFiles.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void buttonRestore_Click(object sender, EventArgs e)
        {
            long lSeqNum = 0;

            if (this.listViewFiles.SelectedIndices.Count > 0 && this.listViewFiles.Items.Count > 0)
            {
                if (MessageBox.Show(this, Properties.Resources.restoreAsk, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string strFile = this.listViewFiles.SelectedItems[0].Text;
                    string strFilePath = string.Format("{0}\\{1}", Properties.Settings.Default.strOptionsBackupDir, strFile);

                    SysRestore.StartRestore("Before Little Registry Cleaner Restore", out lSeqNum);

                    if (xmlReg.loadAsXml(xmlReader, strFilePath))
                    {
                        MessageBox.Show(this, Properties.Resources.restoreRestored, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        if (Properties.Settings.Default.bOptionsDelBackup)
                        {
                            File.Delete(strFilePath);
                            this.listViewFiles.SelectedItems[0].Remove();
                        }
                    }
                    else
                        MessageBox.Show(this, Properties.Resources.restoreError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    SysRestore.EndRestore(lSeqNum);
                }
            }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            Process.Start(Properties.Settings.Default.strOptionsBackupDir);
        }

        private void Restore_Resize(object sender, EventArgs e)
        {
            if (this.listViewFiles.Items.Count > 0)
                this.listViewFiles.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void listViewFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listViewFiles.SelectedItems.Count > 0)
            {
                // Enable restore button if something is selected
                this.buttonRestore.Enabled = true;
            }
            else
            {
                // Disable restore button if nothing selected
                this.buttonRestore.Enabled = false;
            }
        }
    }
}
