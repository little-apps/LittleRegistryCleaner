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
using System.Xml;

namespace Little_Registry_Cleaner
{
    public partial class UpdateDlg : Form
    {
        private string strDownloadURL;
        private string strChangeLogURL;

        public UpdateDlg()
        {
            InitializeComponent();
        }

        private void CheckForUpdate_Shown(object sender, EventArgs e)
        {
            this.labelCurrentVer.Text = Application.ProductVersion;

            FindUpdate();
        }

        private void FindUpdate()
        {
            try
            {
                XmlReader xmlReader = XmlTextReader.Create("http://littlecleaner.sourceforge.net/update.xml");

                string strVersion = "", strChangeLog = "", strDownloadURL = "";

                while (xmlReader.Read())
                {
                    xmlReader.MoveToContent();

                    if (xmlReader.NodeType == XmlNodeType.Element)
                    {
                        if (xmlReader.Name.CompareTo("version") == 0)
                            strVersion = xmlReader.ReadString();
                        else if (xmlReader.Name.CompareTo("changelog") == 0)
                            strChangeLog = xmlReader.ReadString();
                        else if (xmlReader.Name.CompareTo("download") == 0)
                            strDownloadURL = xmlReader.ReadString();
                    }
                }

                Version verApp = new Version(Application.ProductVersion);
                Version verLatest = new Version(strVersion);
                this.labelLatestVer.Text = verLatest.ToString();

                if (verApp.Major < verLatest.Major || verApp.Minor < verLatest.Minor)
                {
                    // Update found
                    this.buttonDownload.Enabled = true;
                    this.buttonChangelog.Enabled = true;

                    this.strDownloadURL = strDownloadURL;
                    this.strChangeLogURL = strChangeLog;
                }
                else
                    // We have the latest version
                    MessageBox.Show(this, "You have the current version", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (System.Net.WebException ex)
            {
                if (MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
                    FindUpdate();
            }
        }

        private void buttonDownload_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.strDownloadURL))
                System.Diagnostics.Process.Start(strDownloadURL);
        }

        private void buttonChangelog_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.strChangeLogURL))
                System.Diagnostics.Process.Start(this.strChangeLogURL);
        }
    }
}
