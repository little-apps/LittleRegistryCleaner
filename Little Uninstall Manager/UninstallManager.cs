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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using Little_Registry_Cleaner;

namespace Little_Uninstall_Manager
{
    public partial class UninstallManager : Form
    {
        private int nSortColumn = -1;

        public UninstallManager()
        {
            InitializeComponent();
        }

        private void UninstallManager_Load(object sender, EventArgs e)
        {
            PopulateListView();

            this.textBoxSearch.Focus();
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            PopulateListView();
        }

        private void PopulateListView()
        {
            List<Common_Tools.ProgramInfo> listProgInfo = new List<Common_Tools.ProgramInfo>();

            // Clear listview
            this.listViewProgs.Items.Clear();

            // Turn textbox into regex pattern
            Regex regex = new Regex("", RegexOptions.IgnoreCase);

            if (this.textBoxSearch.ForeColor != SystemColors.GrayText)
            {
                StringBuilder result = new StringBuilder();
                foreach (string str in this.textBoxSearch.Text.Split(' '))
                {
                    result.Append(Regex.Escape(str));
                    result.Append(".*");
                }

                regex = new Regex(result.ToString(), RegexOptions.IgnoreCase);
            }

            // Get the program info list
            using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"))
            {
                if (regKey != null)
                {
                    foreach (string strSubKeyName in regKey.GetSubKeyNames())
                    {
                        using (RegistryKey subKey = regKey.OpenSubKey(strSubKeyName))
                        {
                            if (subKey != null)
                                listProgInfo.Add(new Common_Tools.ProgramInfo(subKey));
                        }
                    }
                }
            }

            // (x64 registry keys)
            if (Utils.Is64BitOS)
            {
                using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall"))
                {
                    if (regKey != null)
                    {
                        foreach (string strSubKeyName in regKey.GetSubKeyNames())
                        {
                            using (RegistryKey subKey = regKey.OpenSubKey(strSubKeyName))
                            {
                                if (subKey != null)
                                    listProgInfo.Add(new Common_Tools.ProgramInfo(subKey));
                            }
                        }
                    }
                }
            }
            

            // Populate list view
            foreach (Common_Tools.ProgramInfo progInfo in listProgInfo)
            {
                ListViewItem lvi = new ListViewItem();

                // Display Name
                if (!string.IsNullOrEmpty(progInfo.DisplayName))
                    lvi.Text = progInfo.DisplayName;
                else if (!string.IsNullOrEmpty(progInfo.QuietDisplayName))
                    lvi.Text = progInfo.QuietDisplayName;
                else
                    lvi.Text = progInfo.Key;

                // Publisher
                lvi.SubItems.Add(((!string.IsNullOrEmpty(progInfo.DisplayName)) ? (progInfo.Publisher) : ("")));

                // Estimated Size
                if (progInfo.InstallSize > 0)
                    lvi.SubItems.Add(Utils.ConvertSizeToString((uint)progInfo.InstallSize));
                else if (progInfo.EstimatedSize > 0)
                    lvi.SubItems.Add(Utils.ConvertSizeToString(progInfo.EstimatedSize * 1024));
                else
                    lvi.SubItems.Add("");

                if ((!string.IsNullOrEmpty(progInfo.DisplayName))
                    && (string.IsNullOrEmpty(progInfo.ParentKeyName))
                    && (!progInfo.SystemComponent))
                {
                    if (progInfo.Uninstallable)
                        lvi.ImageKey = "OK";
                    else
                        lvi.ImageKey = "ERROR";

                    // Add program info to tag
                    lvi.Tag = progInfo;

                    if (regex.IsMatch(lvi.Text))
                        this.listViewProgs.Items.Add(lvi);
                }
            }

            this.listViewProgs.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void textBoxSearch_Enter(object sender, EventArgs e)
        {
            if (this.textBoxSearch.ForeColor == SystemColors.GrayText)
            {
                this.textBoxSearch.ForeColor = SystemColors.WindowText;
                this.textBoxSearch.Text = "";
            }
        }

        private void textBoxSearch_Leave(object sender, EventArgs e)
        {
            if (this.textBoxSearch.Text.Trim() == "")
            {
                this.textBoxSearch.ForeColor = SystemColors.GrayText;
                this.textBoxSearch.Text = Properties.Resources.umSearchText;
            }
        }

        private void UninstallProgram()
        {
            if (this.listViewProgs.SelectedItems.Count > 0)
            {
                ListViewItem lvi = this.listViewProgs.SelectedItems[0];
                Common_Tools.ProgramInfo progInfo = lvi.Tag as Common_Tools.ProgramInfo;

                if (progInfo.InstallLocation == System.IO.Directory.GetCurrentDirectory())
                {
                    MessageBox.Show(this, Properties.Resources.umCannotUninstall, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show(this, Properties.Resources.umUninstall, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    progInfo.Uninstall();

                PopulateListView();
            }
        }

        private void RemoveProgramFromRegistry()
        {
            if (this.listViewProgs.SelectedItems.Count > 0)
            {
                ListViewItem lvi = this.listViewProgs.SelectedItems[0];
                Common_Tools.ProgramInfo progInfo = lvi.Tag as Common_Tools.ProgramInfo;

                if (MessageBox.Show(this, Properties.Resources.umForceUninstall, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    progInfo.RemoveFromRegistry();

                PopulateListView();
            }
        }

        private void buttonUninstall_Click(object sender, EventArgs e)
        {
            UninstallProgram();
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            RemoveProgramFromRegistry();
        }

        private void listViewProgs_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column != nSortColumn)
            {
                nSortColumn = e.Column;
                this.listViewProgs.Sorting = SortOrder.Ascending;
            }
            else
            {
                if (this.listViewProgs.Sorting == SortOrder.Ascending)
                    this.listViewProgs.Sorting = SortOrder.Descending;
                else
                    this.listViewProgs.Sorting = SortOrder.Ascending;
            }

            this.listViewProgs.ListViewItemSorter = new ListViewItemComparer(e.Column, this.listViewProgs.Sorting);
            this.listViewProgs.Sort();
        }

        private void listViewProgs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listViewProgs.SelectedItems.Count > 0)
            {
                ListViewItem lvi = this.listViewProgs.SelectedItems[0];
                Common_Tools.ProgramInfo progInfo = lvi.Tag as Common_Tools.ProgramInfo;

                this.buttonUninstall.Enabled = progInfo.Uninstallable;
            }
        }
    }

    #region ListViewItemComparer

    public class ListViewItemComparer : IComparer
    {
        private int col;
        private SortOrder order;
        public ListViewItemComparer()
        {
            col = 0;
            order = SortOrder.Ascending;
        }
        public ListViewItemComparer(int column, SortOrder order)
        {
            col = column;
            this.order = order;
        }
        public int Compare(object x, object y)
        {
            int returnVal = -1;
            returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text,
                                    ((ListViewItem)y).SubItems[col].Text);
            // Determine whether the sort order is descending.
            if (order == SortOrder.Descending)
                // Invert the value returned by String.Compare.
                returnVal *= -1;
            return returnVal;
        }
    }
    #endregion
}
