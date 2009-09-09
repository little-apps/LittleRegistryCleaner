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
using System.Collections;
using System.Windows.Forms;
using System.Text;
using System.Drawing;
using Microsoft.Win32;

namespace Little_Registry_Cleaner
{
    public class BadRegistryKey : Common_Tools.TreeViewAdv.Tree.Node
    {
        #region Properties
        private CheckState _bChecked = CheckState.Checked;
        private Image _icon;
        private string _strProblem = "";
        private string _strValueName = "";
        private string _strSectionName = "";
        private string _strData = "";
        public readonly string baseRegKey = "";
        public readonly string subRegKey = "";

        public CheckState Checked
        {
            get { return _bChecked; }
            set { SetIsChecked(value, true, true); }
        }

        public new bool IsLeaf
        {
            get { return string.IsNullOrEmpty(this._strSectionName); }
        }

        public Image Img
        {
            set { _icon = value; }
            get { return _icon; }
        }

        /// <summary>
        /// Get the problem
        /// </summary>
        public string Problem
        {
            get { return _strProblem; }
        }

        /// <summary>
        /// Gets the value name
        /// </summary>
        public string ValueName
        {
            get { return _strValueName; }
        }

        /// <summary>
        /// Gets/Sets the section name
        /// </summary>
        public string SectionName
        {
            get { return _strSectionName; }
            set { _strSectionName = value; }
        }

        /// <summary>
        /// Gets the data in the bad registry key
        /// </summary>
        public string Data
        {
            get { return _strData; }
        }

        private void SetIsChecked(CheckState value, bool updateChildren, bool updateParent)
        {
            this._bChecked = value;

            if (updateChildren && this._bChecked != CheckState.Indeterminate)
            {
                foreach (BadRegistryKey c in this.Nodes)
                    c.SetIsChecked(this._bChecked, true, false);
            }

            if (updateParent && this.IsLeaf)
            {
                (this.Parent as BadRegistryKey).VerifyCheckState();
            }
        }

        private void VerifyCheckState()
        {
            CheckState state = CheckState.Indeterminate;

            for (int i = 0; i < this.Nodes.Count; i++)
            {
                BadRegistryKey brk = this.Nodes[i] as BadRegistryKey;
                CheckState current = brk.Checked;

                if (i == 0)
                    state = current;
                else if (state != current)
                {
                    state = CheckState.Indeterminate;
                    break;
                }
            }

            this.SetIsChecked(state, false, true);
        }

        /// <summary>
        /// Gets the registry path
        /// </summary>
        public string RegKeyPath
        {
            get
            {
                if (!string.IsNullOrEmpty(baseRegKey) && !string.IsNullOrEmpty(subRegKey))
                    return string.Format("{0}\\{1}", baseRegKey, subRegKey);
                else if (!string.IsNullOrEmpty(baseRegKey))
                    return baseRegKey;

                return "";
            }
        }
        #endregion

        /// <summary>
        /// Constructor for new bad registry key
        /// </summary>
        /// <param name="problem">Reason registry key is invalid</param>
        /// <param name="regPath">Path to registry key (including registry hive)</param>
        /// <param name="valueName">Value Name (can be null)</param> 
        public BadRegistryKey(string problem, string baseKey, string subKey, string valueName)
        {
            _bChecked = CheckState.Checked;
            _strProblem = problem;
            baseRegKey = baseKey;
            subRegKey = subKey;

            if (!string.IsNullOrEmpty(valueName))
            {
                _strValueName = valueName;

                // Open registry key
                RegistryKey regKey = Utils.RegOpenKey(baseKey, subKey);

                // Convert value to string
                if (regKey != null)
                    this._strData = Utils.RegConvertXValueToString(regKey, valueName);
            }
        }

        /// <summary>
        /// Constructor for root node
        /// </summary>
        /// <param name="SectionName">Section Name</param>
        public BadRegistryKey()
        {
            _bChecked = CheckState.Checked;
            _strSectionName = "";
            _strProblem = "";
            _strValueName = "";
        }

        public override string ToString()
        {
            return string.Copy(RegKeyPath);
        }
    }

    public class BadRegKeyArray : CollectionBase
    {
        private int _problems = 0, _itemsscanned = 0, _sectioncount = 0;

        public int Problems
        {
            get { return _problems; }
            set { _problems = value; }
        }

        public int ItemsScanned
        {
            get { return _itemsscanned; }
            set { _itemsscanned = value; }
        }

        public int SectionCount
        {
            get { return _sectioncount; }
            set { _sectioncount = value; }
        }

        /// <summary>
        /// Returns number of problems
        /// </summary>
        public new int Count
        {
            get { return _problems; }
        }

        public BadRegistryKey this[int index]
        {
            get { return (BadRegistryKey)this.InnerList[index]; }
            set { this.InnerList[index] = value; }
        }

        public int Add(BadRegistryKey BadRegKey)
        {
            if (BadRegKey == null)
                throw new ArgumentNullException("BadRegKey");

            return (this.InnerList.Add(BadRegKey));
        }

        public int IndexOf(BadRegistryKey BadRegKey)
        {
            return (this.InnerList.IndexOf(BadRegKey));
        }

        public void Insert(int index, BadRegistryKey BadRegKey)
        {
            if (BadRegKey == null)
                throw new ArgumentNullException("BadRegKey");

            this.InnerList.Insert(index, BadRegKey);
        }

        public void Remove(BadRegistryKey BadRegKey)
        {
            if (BadRegKey == null)
                throw new ArgumentNullException("BadRegKey");

            this.InnerList.Remove(BadRegKey);
        }

        public bool Contains(BadRegistryKey BadRegKey)
        {
            return (this.InnerList.Contains(BadRegKey));
        }

        public void Clear(int SectionCount)
        {
            this._sectioncount = SectionCount;
            this._itemsscanned = 0;
            this._problems = 0;

            // Clears all lists
            base.Clear();
        }
    }

    public class BadRegKeySorter : IComparer
    {
        /// <summary>
        /// Specifies the column to be sorted
        /// </summary>
        private int ColumnToSort = 0;
        /// <summary>
        /// Specifies the order in which to sort (i.e. 'Ascending').
        /// </summary>
        private SortOrder OrderOfSort = SortOrder.Ascending;
        /// <summary>
        /// Case insensitive comparer object
        /// </summary>
        private CaseInsensitiveComparer ObjectCompare = new CaseInsensitiveComparer();

        public BadRegKeySorter()
        {
        }

        /// <summary>
        /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
        /// </summary>
        /// <param name="x">First object to be compared</param>
        /// <param name="y">Second object to be compared</param>
        /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
        public int Compare(object x, object y)
        {
            // Cast the objects to be compared to ListViewItem objects
            ListViewItem listviewX = (ListViewItem)x;
            ListViewItem listviewY = (ListViewItem)y;

            // See if columns exist, otherwise return 0
            if (listviewX.SubItems.Count <= ColumnToSort || listviewY.SubItems.Count <= ColumnToSort)
                return 0;

            // Compare the two items
            int compareResult = ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Text, listviewY.SubItems[ColumnToSort].Text);

            // Calculate correct return value based on object comparison
            if (OrderOfSort == SortOrder.Ascending)
            {
                // Ascending sort is selected, return normal result of compare operation
                return compareResult;
            }
            else if (OrderOfSort == SortOrder.Descending)
            {
                // Descending sort is selected, return negative result of compare operation
                return (-compareResult);
            }
            else
            {
                // Return '0' to indicate they are equal
                return 0;
            }
        }

        /// <summary>
        /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
        /// </summary>
        public int SortColumn
        {
            set
            {
                ColumnToSort = value;
            }
            get
            {
                return ColumnToSort;
            }
        }

        /// <summary>
        /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
        /// </summary>
        public SortOrder Order
        {
            set
            {
                OrderOfSort = value;
            }
            get
            {
                return OrderOfSort;
            }
        }

    }
}
