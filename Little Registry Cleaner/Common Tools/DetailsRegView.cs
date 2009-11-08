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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Common_Tools
{
    public partial class DetailsRegView : UserControl
    {

        public string Data
        {
            set { this.labelData1.Text = value; }
        }

        public string RegKey
        {
            set { this.labelHKEY1.Text = value; }
        }

        public string Problem
        {
            set { this.labelProblem1.Text = value; }
        }

        public string ValueName
        {
            set { this.labelValueName1.Text = value; }
        }

        public void ReloadControls()
        {
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(DetailsRegView));

            // Details
            this.labelDetails.Text = resources.GetString("labelDetails.Text");

            // Problem
            this.labelProblem.Text = resources.GetString("labelProblem.Text");

            // Location
            this.labelHKEY.Text = resources.GetString("labelHKEY.Text");

            // Value Name
            this.labelValueName.Text = resources.GetString("labelValueName.Text");

            // Data
            this.labelData.Text = resources.GetString("labelData.Text");
        }

        public DetailsRegView()
        {
            InitializeComponent();
        }
    }
}
