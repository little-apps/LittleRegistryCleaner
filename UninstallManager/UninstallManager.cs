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

namespace Little_Registry_Cleaner.UninstallManager
{
    

    public partial class UninstallManager : Form
    {

        private static ProgramList arrProgList = new ProgramList();
        private int nSortColumn = -1;

        private RegistryKey Key
        {
            get
            {
                return Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            }
        }

        public UninstallManager()
        {
            InitializeComponent();
        }

        private void UninstallManager_Load(object sender, EventArgs e)
        {
            using (RegistryKey regKey = Key)
            {
                foreach (string strSubKeyName in regKey.GetSubKeyNames())
                {
                    if (strSubKeyName.Contains("KB") ||
                        strSubKeyName.Contains("Microsoft Security Patch"))
                        continue;

                    using (RegistryKey subKey = regKey.OpenSubKey(strSubKeyName))
                    {
                        ProgramInfo objProgInfo = new ProgramInfo(subKey);
                        
                        // Get cached information
                        using (RegistryKey rkARPCache = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Management\\ARPCache\\" + strSubKeyName))
                        {
                            if (rkARPCache != null)
                            {
                                byte[] b = (byte[])rkARPCache.GetValue("SlowInfoCache");

                                GCHandle gcHandle = GCHandle.Alloc(b, GCHandleType.Pinned);
                                IntPtr ptr = gcHandle.AddrOfPinnedObject();
                                Utils.SlowInfoCache objSlowInfoCache = (Utils.SlowInfoCache)Marshal.PtrToStructure(ptr, typeof(Utils.SlowInfoCache));

                                objProgInfo.InstallSize = objSlowInfoCache.InstallSize;
                                objProgInfo.Frequency = objSlowInfoCache.Frequency;
                                objProgInfo.LastUsed = Utils.FileTime2DateTime(objSlowInfoCache.LastUsed);
                                if (objSlowInfoCache.HasName == 1)
                                    objProgInfo.FileName = objSlowInfoCache.Name;

                                gcHandle.Free();
                                rkARPCache.Close();
                            }
                        }

                        arrProgList.Add(objProgInfo);

                        subKey.Close();
                    }
                }
            }

            PopulateListView();
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            if (this.textBoxSearch.ForeColor == SystemColors.GrayText) return;

            PopulateListView();
        }

        private void PopulateListView()
        {
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

            ProgramList tempProgList = new ProgramList();
            this.listViewProgs.Items.Clear();

            foreach (DictionaryEntry de in arrProgList)
            {

                ListViewItem lvi = new ListViewItem();
                ProgramInfo objProgInfo = (ProgramInfo)de.Key;

                // Display Name
                if (!string.IsNullOrEmpty(objProgInfo.DisplayName))
                    lvi.Text = objProgInfo.DisplayName;
                else if (!string.IsNullOrEmpty(objProgInfo.QuietDisplayName))
                    lvi.Text = objProgInfo.QuietDisplayName;
                else
                    lvi.Text = objProgInfo.Key;

                if (!regex.IsMatch(lvi.Text))
                    continue;

                // Publisher
                lvi.SubItems.Add(((!string.IsNullOrEmpty(objProgInfo.DisplayName)) ? (objProgInfo.Publisher) : ("")));

                // Estimated Size
                if (objProgInfo.InstallSize > 0)
                    lvi.SubItems.Add(Utils.ConvertSizeToString((uint)objProgInfo.InstallSize));
                else if (objProgInfo.EstimatedSize > 0)
                    lvi.SubItems.Add(Utils.ConvertSizeToString(objProgInfo.EstimatedSize * 1024));
                else
                    lvi.SubItems.Add("");

                if ((!string.IsNullOrEmpty(objProgInfo.DisplayName))
                    && (string.IsNullOrEmpty(objProgInfo.ParentKeyName))
                    && (!objProgInfo.SystemComponent))
                {
                    if (objProgInfo.Uninstallable)
                        lvi.ImageIndex = 0; // OK
                    else
                        lvi.ImageIndex = 1; // ERROR

                    this.listViewProgs.Items.Add(lvi);
                    tempProgList.Add(objProgInfo, lvi);
                }
            }

            arrProgList = tempProgList;

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
                this.textBoxSearch.Text = "Search by name";
            }

        }

        private void UninstallProgram()
        {
            if (this.listViewProgs.SelectedItems.Count > 0)
            {
                foreach (DictionaryEntry de in arrProgList)
                {
                    ProgramInfo objProgInfo = (ProgramInfo)de.Key;
                    ListViewItem lvi = (ListViewItem)de.Value;

                    if (lvi.Selected)
                    {
                        objProgInfo.Uninstall();
                        break;
                    }
                }

                PopulateListView();
            }
        }

        private void RemoveProgramFromRegistry()
        {
            if (this.listViewProgs.SelectedItems.Count > 0)
            {
                foreach (DictionaryEntry de in arrProgList)
                {
                    ProgramInfo objProgInfo = (ProgramInfo)de.Key;
                    ListViewItem lvi = (ListViewItem)de.Value;

                    if (lvi.Selected)
                    {
                        if (MessageBox.Show(this, "Are you sure you want to remove this program from the registry?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            objProgInfo.RemoveFromRegistry();
                        break;
                    }
                }

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
                foreach (DictionaryEntry de in arrProgList)
                {
                    ProgramInfo objProgInfo = (ProgramInfo)de.Key;
                    ListViewItem lvi = (ListViewItem)de.Value;

                    if (lvi.Selected)
                    {
                        this.buttonUninstall.Enabled = objProgInfo.Uninstallable;
                        break;
                    }
                }
            }
        }
    }

    
}
