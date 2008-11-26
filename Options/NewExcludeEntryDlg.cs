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

namespace Little_Registry_Cleaner
{
    public partial class NewExcludeEntryDlg : Form
    {
        public delegate void NewExcludeEntryHandler(string strRootKey, string strPath);
        public event NewExcludeEntryHandler NewExcludeEntry;

        public NewExcludeEntryDlg()
        {
            InitializeComponent();

            this.comboBox1.Text = (string)this.comboBox1.Items[0];
        }

       
        private void buttonOk_Click(object sender, EventArgs e)
        {
            string strBaseKey = this.comboBox1.Text, strPath = this.textBox1.Text;

            if (string.IsNullOrEmpty(strBaseKey) || string.IsNullOrEmpty(strPath))
            {
                MessageBox.Show(this, "Registry path cannot be empty", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Xml.xmlRegistry.keyExists(strBaseKey, strPath))
            {
                MessageBox.Show(this, "Specified registry key doesn't exist", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            NewExcludeEntry(strBaseKey, strPath);
            this.Close();
        }
    }
}
