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

            string strVersion = "", strChangeLogURL = "", strDownloadURL = "";

            if (FindUpdate(ref strVersion, ref strChangeLogURL, ref strDownloadURL))
            {
                this.strDownloadURL = strDownloadURL;
                this.strChangeLogURL = strChangeLogURL;

                this.buttonDownload.Enabled = true;
                this.buttonChangelog.Enabled = true;
            }
            else
            {
                MessageBox.Show(this, "You have the current version", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information); 
            }
        }

        /// <summary>
        /// Connects to update server and sees if update is available
        /// </summary>
        /// <param name="strVersion">Current version</param>
        /// <param name="strChangeLogURL">Changelog URL</param>
        /// <param name="strDownloadURL">Download URL</param>
        /// <returns>True if a update is available</returns>
        public static bool FindUpdate(ref string strVersion, ref string strChangeLogURL, ref string strDownloadURL)
        {
            try
            {
                XmlReader xmlReader = XmlTextReader.Create(Properties.Settings.Default.strUpdateURL);

                while (xmlReader.Read())
                {
                    xmlReader.MoveToContent();

                    if (xmlReader.NodeType == XmlNodeType.Element)
                    {
                        if (xmlReader.Name.CompareTo("version") == 0)
                            strVersion = xmlReader.ReadString();
                        else if (xmlReader.Name.CompareTo("changelog") == 0)
                            strChangeLogURL = xmlReader.ReadString();
                        else if (xmlReader.Name.CompareTo("download") == 0)
                            strDownloadURL = xmlReader.ReadString();
                    }
                }

                Version verApp = new Version(Application.ProductVersion);
                Version verLatest = new Version(strVersion);

                if (verApp.Major < verLatest.Major || verApp.Minor < verLatest.Minor)
                    // Update found
                    return true;

                // We have the latest version
                return false;

            }
            catch (System.Net.WebException ex)
            {
                if (MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
                    return FindUpdate(ref strVersion, ref strChangeLogURL, ref strDownloadURL);
            }

            return false;
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
