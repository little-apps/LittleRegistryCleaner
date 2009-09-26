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
