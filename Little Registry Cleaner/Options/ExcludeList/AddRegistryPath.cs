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
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Little_Registry_Cleaner.ExcludeList
{
    public partial class AddRegistryPath : Form
    {
        private string _regPath;
        /// <summary>
        /// The registry path selected by the user
        /// </summary>
        public string RegistryPath
        {
            get { return _regPath; }
        }

        public AddRegistryPath()
        {
            InitializeComponent();

            this.comboBox1.Text = (string)this.comboBox1.Items[0];
        }

       
        private void buttonOk_Click(object sender, EventArgs e)
        {
            string strBaseKey = this.comboBox1.Text, strSubKey = this.textBox1.Text;

            if (string.IsNullOrEmpty(strBaseKey) || string.IsNullOrEmpty(strSubKey))
            {
                MessageBox.Show(this, Properties.Resources.optionsExcludeEmptyRegPath, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (!Utils.RegKeyExists(strBaseKey, strSubKey))
            {
                MessageBox.Show(this, Properties.Resources.optionsExcludeInvalidRegPath, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
                return;
            }

            this._regPath = string.Format(@"{0}\{1}", strBaseKey, strSubKey);

            this.Close();
        }
    }
}
