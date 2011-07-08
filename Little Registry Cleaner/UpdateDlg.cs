/*
    Little Registry Cleaner
    Copyright (C) 2008-2011 Little Apps (http://www.little-apps.org/)

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

        public static DateTime LastUpdate
        {
            get {
                long o = Properties.Settings.Default.dtLastUpdate;
                if (o != 0)
                    return DateTime.FromBinary(o);
                else
                    return DateTime.MinValue;
            }
            set { Properties.Settings.Default.dtLastUpdate = value.ToBinary(); }
        }

        public UpdateDlg()
        {
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            string strVersion = "", strChangeLogURL = "", strDownloadURL = "", strReleaseDate = "";

            if (FindUpdate(ref strVersion, ref strReleaseDate, ref strChangeLogURL, ref strDownloadURL, false))
            {
                this.strDownloadURL = strDownloadURL;
                this.strChangeLogURL = strChangeLogURL;

                this.buttonDownload.Enabled = true;
                this.buttonChangelog.Enabled = true;

                this.labelInfo.Text = Properties.Resources.updateNewer;
            }
            else
            {
                this.buttonDownload.Enabled = false;
                this.buttonChangelog.Enabled = false;

                this.labelInfo.Text = Properties.Resources.updateLatest;
            }

            base.OnShown(e);
        }

        /// <summary>
        /// Connects to update server and sees if update is available
        /// </summary>
        /// <param name="strVersion">Current version available</param>
        /// <param name="strReleaseDate">Date version was released</param>
        /// <param name="strChangeLogURL">Changelog URL</param>
        /// <param name="strDownloadURL">Download URL</param>
        /// <param name="bCheckDate">Check last time since update</param>
        /// <returns>True if a update is available</returns>
        public static bool FindUpdate(ref string strVersion, ref string strReleaseDate, ref string strChangeLogURL, ref string strDownloadURL, bool bCheckDate)
        {
            bool bRet = false;
            DateTime dtReleaseDate;

            if (bCheckDate)
            {
                // Dont check cus this the first time the programs ran
                if (LastUpdate == DateTime.MinValue)
                {
                    LastUpdate = DateTime.Today;
                    return bRet;
                }

                // Only check if it has been >= 2 weeks since last check
                if (DateTime.Now.Subtract(LastUpdate).TotalDays < 14)
                    return bRet;
            }

            // Set last update date since were checking
            LastUpdate = DateTime.Today;

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
                        else if (xmlReader.Name.CompareTo("date") == 0)
                            strReleaseDate = xmlReader.ReadString();
                        else if (xmlReader.Name.CompareTo("changelog") == 0)
                            strChangeLogURL = xmlReader.ReadString();
                        else if (xmlReader.Name.CompareTo("download") == 0)
                            strDownloadURL = xmlReader.ReadString();
                    }
                }

                Version verApp = new Version(Application.ProductVersion);
                Version verLatest = new Version(strVersion);

                // Compare current version to latest
                if (verApp.CompareTo(verLatest) < 0)
                    bRet = true;

                if (DateTime.TryParseExact(strReleaseDate, @"MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out dtReleaseDate))
                {
                    DateTime dtBuildDate = new DateTime(2000, 1, 1).AddDays(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Build);

                    // See if date from xml is later than build date
                    if (DateTime.Compare(dtReleaseDate, dtBuildDate) > 0)
                        bRet = true;
                }

            }
            catch (System.Net.WebException ex)
            {
                if (MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
                    return FindUpdate(ref strVersion, ref strReleaseDate, ref strChangeLogURL, ref strDownloadURL, bCheckDate);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("The updater encountered an error ({0}). Please try again later...", ex.Message), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                bRet = false;
            }

            return bRet;
        }

        private void buttonDownload_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.strDownloadURL))
                Utils.LaunchURI(this.strDownloadURL);
        }

        private void buttonChangelog_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.strChangeLogURL))
                Utils.LaunchURI(this.strChangeLogURL);
        }
    }
}
