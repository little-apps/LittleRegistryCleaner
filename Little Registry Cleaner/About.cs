/*
    Little Registry Cleaner
    Copyright (C) 2008-2010 Little Apps (http://www.littleapps.co.cc/)

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
using System.Diagnostics;
using System.Reflection;

namespace Little_Registry_Cleaner
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();

            // Set version
            this.labelTitle.Text = this.labelTitle.Text + " v" + Application.ProductVersion;

            // Get build time
            //this.labelBuildTime.Text = Properties.Settings.Default.strBuildTime;
        }

        private void buttonWebsite_Click(object sender, EventArgs e)
        {
            Utils.LaunchURI("http://littlecleaner.sourceforge.net/");
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkLabelAuthor_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.LaunchURI("mailto:nick@littleapps.co.cc");

        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Utils.LaunchURI(e.LinkText);
        }
    }
}
