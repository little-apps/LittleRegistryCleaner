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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Little_Registry_Cleaner.UninstallManager
{
    public class ProgramList : DictionaryBase
    {
        public ListViewItem this[ProgramInfo key]
        {
            get
            {
                return (ListViewItem)Dictionary[key];
            }
            set
            {
                Dictionary[key] = value;
            }
        }

        public ICollection Keys
        {
            get
            {
                return (Dictionary.Keys);
            }
        }

        public ICollection Values
        {
            get
            {
                return (Dictionary.Values);
            }
        }

        public void Add(ProgramInfo key)
        {
            if (Dictionary.Contains(key))
                Dictionary.Remove(key);

            Dictionary.Add(key, null);
        }

        public void Add(ProgramInfo key, ListViewItem value) 
        {
            if (Dictionary.Contains(key))
                Dictionary.Remove(key);

            Dictionary.Add(key, value);
        }

        public bool Contains(ProgramInfo key)
        {
            return Dictionary.Contains(key);
        }

        public void Remove(ProgramInfo key)
        {
            Dictionary.Remove(key);
        }

        protected override void OnValidate(object key, object value)
        {
            if (key != null && key.GetType() != typeof(ProgramInfo))
                throw new ArgumentException("Key must be ProgramInfo type", "key");

            if (value != null && value.GetType() != typeof(ListViewItem))
                throw new ArgumentException("Value must be ListViewItem type", "value");

            base.OnValidate(key, value);
        }
    }
}
