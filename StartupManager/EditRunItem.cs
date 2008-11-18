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
using Microsoft.Win32;

namespace Little_Registry_Cleaner.StartupManager
{
    public partial class EditRunItem : Form
    {
        private string strItem;
        private string strSection;

        public EditRunItem(string item, string section, string path, string args)
        {
            InitializeComponent();

            strItem = item;
            strSection = section;

            this.Text = "Edit - " + strItem;

            this.textBoxFile.Text = path;
            this.textBoxArgs.Text = args;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (!Utils.FileExists(this.textBoxFile.Text))
            {
                MessageBox.Show(this, "The file could not be found", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (strSection.StartsWith(@"Registry\All Users") || strSection.StartsWith(@"Registry\Current User"))
            {
                RegistryKey regKey = null;
                string strRegPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\" + strSection.Substring(strSection.LastIndexOf("\\") + 1);
                string strPath = "";

                if (!string.IsNullOrEmpty(this.textBoxFile.Text) && !string.IsNullOrEmpty(this.textBoxArgs.Text))
                    strPath = string.Format("\"{0}\" {1}", this.textBoxFile.Text, this.textBoxArgs.Text);
                else
                    strPath = string.Format("\"{0}\"", this.textBoxFile.Text);

                if (strSection.StartsWith(@"Registry\All Users"))
                {
                    regKey = Registry.LocalMachine.OpenSubKey(strRegPath, true);
                }
                else if (strSection.StartsWith(@"Registry\Current User"))
                {
                    regKey = Registry.CurrentUser.OpenSubKey(strRegPath, true);
                }

                regKey.SetValue(strItem, strPath);
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

                string strItemPath = string.Format("{0}\\{1}", strStartupFolder, strItem);

                File.Delete(strItemPath);

                Utils.CreateShortcut(strItemPath, string.Format("\"{0}\"", this.textBoxFile.Text), this.textBoxArgs.Text);
            }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.Multiselect = false;
            ofd.Filter = "All Files (*.*)|*.*";

            if (ofd.ShowDialog(this) == DialogResult.OK)
                this.textBoxFile.Text = ofd.FileName;
        }
    }
}
