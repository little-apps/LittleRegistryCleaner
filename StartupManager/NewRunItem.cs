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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Little_Registry_Cleaner.StartupManager
{
    public partial class NewRunItem : Form
    {
        public NewRunItem()
        {
            InitializeComponent();

            this.comboBoxSection.Text = this.comboBoxSection.Items[0].ToString();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            RegistryKey regKey;

            string strRegPath = this.comboBoxSection.Text.Substring(this.comboBoxSection.Text.LastIndexOf('\\') + 1);
            string strRunRegPath = "";

            if (strRegPath == "Run")
                strRunRegPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
            else if (strRegPath == "Run Once")
                strRunRegPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce";
            else if (strRegPath == "Run Services")
                strRunRegPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunServices";
            else if (strRegPath == "Run Services Once")
                strRunRegPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunServicesOnce";

            if (string.IsNullOrEmpty(this.textBoxName.Text))
            {
                MessageBox.Show(this, "Shortcut/Value name cannot be empty", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(this.textBoxPath.Text))
            {
                MessageBox.Show(this, "File path cannot be empty", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string strFullPath = "";
            if (!string.IsNullOrEmpty(this.textBoxArgs.Text))
                strFullPath = string.Format("\"{0}\" {1}", this.textBoxPath.Text, this.textBoxArgs.Text);
            else
                strFullPath = string.Format("\"{0}\"", this.textBoxPath.Text);

            if (this.comboBoxSection.Text.StartsWith(@"Registry\All Users")) 
            {
                regKey = Registry.LocalMachine.OpenSubKey(strRunRegPath, true);
                regKey.SetValue(this.textBoxName.Text, strFullPath, RegistryValueKind.String);
                regKey.Close();
            }
            else if (this.comboBoxSection.Text.StartsWith(@"Registry\Current User"))
            {
                regKey = Registry.CurrentUser.OpenSubKey(strRunRegPath, true);
                regKey.SetValue(this.textBoxName.Text, strFullPath, RegistryValueKind.String);
                regKey.Close();
            }
            else if (this.comboBoxSection.Text.StartsWith("StartUp"))
            {
                string strStartUpPath = "";

                if (this.comboBoxSection.Text == @"StartUp\All Users")
                    strStartUpPath = Utils.GetSpecialFolderPath(Utils.CSIDL_STARTUP);
                else if (this.comboBoxSection.Text == @"StartUp\Current User")
                    strStartUpPath = Utils.GetSpecialFolderPath(Utils.CSIDL_COMMON_STARTUP);

                string strShortcutName = this.textBoxName.Text;
                if (!strShortcutName.EndsWith(".lnk"))
                    strShortcutName += ".lnk";

                string strShortcutPath = System.IO.Path.Combine(strStartUpPath, strShortcutName);

                if (!Utils.CreateShortcut(strShortcutPath, this.textBoxPath.Text, this.textBoxArgs.Text))
                    this.DialogResult = DialogResult.Cancel;
            }

            this.Close();
        }

        private string GetStartupRegPath(string strRegPath)
        {
            if (strRegPath == "Run")
                return "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
            else if (strRegPath == "Run Once")
                return "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce";
            else if (strRegPath == "Run Services")
                return "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunServices";
            else if (strRegPath == "Run Services Once")
                return "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunServicesOnce";
            else
                return "";
        }

        private void buttonFileDlg_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Multiselect = false;
            ofd.CheckFileExists = true;
            ofd.Filter = "All files (*.*)|*.*";

            if (ofd.ShowDialog(this) == DialogResult.OK)
                this.textBoxPath.Text = ofd.FileName;
        }
    }
}
