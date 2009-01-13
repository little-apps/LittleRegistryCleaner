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
using System.Collections;
using System.Linq;
using System.Text;

namespace Little_Registry_Cleaner
{
    public class BadRegistryKey : System.Windows.Forms.ListViewItem
    {
        /// <summary>
        /// Get/Sets the problem
        /// </summary>
        private string _strProblem = "";
        public string Problem
        {
            get { return _strProblem; }
            set { _strProblem = value; }
        }

        /// <summary>
        /// Gets/Sets the value name
        /// </summary>
        private string _strValueName = "";
        public string ValueName
        {
            get { return _strValueName; }
            set { _strValueName = value; }
        }

        /// <summary>
        /// Gets/Sets the section name
        /// </summary>
        private string _strSectionName = "";
        public string SectionName
        {
            get { return _strSectionName; }
            set { _strSectionName = value; }
        }

        /// <summary>
        /// Gets/Sets the registry path
        /// </summary>
        public string strMainKey = "";
        public string strSubKey = "";
        public string RegKeyPath
        {
            get
            {
                if (!string.IsNullOrEmpty(strMainKey) && !string.IsNullOrEmpty(strSubKey))
                    return string.Format("{0}\\{1}", strMainKey, strSubKey);
                else if (!string.IsNullOrEmpty(strMainKey))
                    return strMainKey;

                return "";
            }
            set
            {
                string strPath = value;

                if (strPath.Length == 0)
                    return;

                int nSlash = strPath.IndexOf("\\");
                if (nSlash > -1)
                {
                    strMainKey = strPath.Substring(0, nSlash);
                    strSubKey = strPath.Substring(nSlash + 1);
                }
                else
                {
                    strMainKey = strPath;
                    strSubKey = "";
                }
            }
        }

        /// <summary>
        /// Constructor for new bad registry key
        /// </summary>
        /// <param name="Problem">Reason registry key is invalid</param>
        /// <param name="RegistryKey">Path to registry key (including registry hive)</param>
        /// <param name="ValueName">Value Name (can be null)</param>
        public BadRegistryKey(string SectionName, string Problem, string RegistryKey, string ValueName)
        {
            if (!Utils.RegKeyExists(RegistryKey))
                throw new ArgumentException("Registry Key path doesnt exist", "RegistryKey");

            _strSectionName = SectionName;
            _strProblem = Problem;
            RegKeyPath = RegistryKey;
            _strValueName = ValueName;

            // Add listviewitem information
            base.Checked = true;
            base.Text = Problem;
            base.SubItems.Add(RegistryKey);
            if (!string.IsNullOrEmpty(ValueName))
                base.SubItems.Add(ValueName);
        }

        public override string ToString()
        {
            return string.Copy(RegKeyPath);
        }
    }

    public class BadRegKeyArray : CollectionBase
    {
        public BadRegistryKey this[int index]
        {
            get { return (BadRegistryKey)this.InnerList[index]; }
            set { this.InnerList[index] = value; }
        }

        public int Add(string Problem, string Path, string ValueName)
        {
            return (this.Add(ScanDlg.CurrentSection, Problem, Path, ValueName));
        }

        public int Add(string SectionName, string Problem, string Path, string ValueName)
        {
            BadRegistryKey p = new BadRegistryKey(SectionName, Problem, Path, ValueName);

            return (this.InnerList.Add((BadRegistryKey)p));
        }

        public int Add(BadRegistryKey BadRegKey)
        {
            return (this.InnerList.Add(BadRegKey));
        }

        public int IndexOf(BadRegistryKey BadRegKey)
        {
            return (this.InnerList.IndexOf(BadRegKey));
        }

        public void Insert(int index, BadRegistryKey BadRegKey)
        {
            this.InnerList.Insert(index, BadRegKey);
        }

        public void Remove(BadRegistryKey BadRegKey)
        {
            this.InnerList.Remove(BadRegKey);
        }

        public bool Contains(BadRegistryKey BadRegKey)
        {
            return (this.InnerList.Contains(BadRegKey));
        }

        protected override void OnValidate(object value)
        {
            if (value.GetType() != typeof(BadRegistryKey))
                throw new ArgumentException("value must be of type BadRegistryKey", "value");
        }
    }
}
