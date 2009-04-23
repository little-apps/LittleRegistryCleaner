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
            set { this.labelData.Text = "Data: " + value; }
        }

        public string RegKey
        {
            set { this.labelHKEY.Text = "Location: " + value; }
        }

        public string Problem
        {
            set { this.labelProblem.Text = "Problem: " + value; }
        }

        public string ValueName
        {
            set { this.labelValueName.Text = "Value Name: " + value; }
        }

        public DetailsRegView()
        {
            InitializeComponent();
        }
    }
}
