using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Common_Tools
{
    public partial class DetailsRegKey : Form
    {

        public DetailsRegKey(string Problem, string RegKey, string ValueName, string Data)
        {
            InitializeComponent();

            this.labelProblem1.Text = Problem;
            this.labelHKEY1.Text = RegKey;
            this.labelValueName1.Text = ValueName;
            this.textBox1.Text = Data;

            this.AutoResizeWindow();
        }

        private void AutoResizeWindow()
        {
            using (Graphics g = Graphics.FromHwnd(this.Handle))
            {
                SizeF sf = SizeF.Empty;
                int w = 0;

                sf = g.MeasureString(string.Format("{0} {1}", this.labelProblem.Text,this.labelProblem1.Text), this.Font);
                if (!sf.IsEmpty)
                    w = (int)Math.Ceiling(sf.Width);

                sf = g.MeasureString(string.Format("{0} {1}", this.labelHKEY.Text, this.labelHKEY1.Text), this.Font);
                if (!sf.IsEmpty)
                    w = Math.Max(w, (int)Math.Ceiling(sf.Width));

                sf = g.MeasureString(string.Format("{0} {1}", this.labelValueName.Text, this.labelValueName1.Text), this.Font);
                if (!sf.IsEmpty)
                    w = Math.Max(w, (int)Math.Ceiling(sf.Width));

                //sf = g.MeasureString(this.labelData1.Text, this.Font);
                //if (!sf.IsEmpty)
                //    w = Math.Max(w, (int)Math.Ceiling(sf.Width));

                this.Width = w + 5;
            }
        }
    }
}
