using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Little_Registry_Cleaner.Optimizer
{
    public partial class Optimizer : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ExitWindowsEx(uint uFlags, uint dwReason);

        // Shutdown reason codes
        const uint MajorOperatingSystem = 0x00020000;
        const uint MinorReconfig = 0x00000004;
        const uint FlagPlanned = 0x80000000;

        public static HiveCollection arrHives = new HiveCollection();
        delegate void AddToListViewDelegate(Hive oHive);
        private Thread tAnalyzeHives = null;

        public Optimizer()
        {
            InitializeComponent();
        }

        public static string GetSizeInMegaBytes(long Length)
        {
            double nMegaBytes = Length / 1024F / 1024F;

            return string.Format("{0} MB", nMegaBytes.ToString("0.00"));
        }

        private void Optimizer1_Shown(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "The program will now analyze your registry files. Continue?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                RegistryKey rkHives = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\hivelist");

                if (rkHives == null)
                    return;

                string[] strHives = rkHives.GetValueNames();

                foreach (string strValueName in strHives)
                {
                    string strHivePath = rkHives.GetValue(strValueName) as string;
                    if (!string.IsNullOrEmpty(strValueName) && !string.IsNullOrEmpty(strHivePath))
                    {
                        Hive oHive = new Hive(strValueName, strHivePath);
                        arrHives.Add(oHive);
                    }
                }

                rkHives.Close();

                this.progressBarAnalyzed.Step = 1;
                this.progressBarAnalyzed.Maximum = arrHives.Count;

                this.tAnalyzeHives = new Thread(new ThreadStart(AnalyzeHives));
                this.tAnalyzeHives.Start();
            }
            else
                this.Close();
        }

        private void AnalyzeHives()
        {
            if (arrHives.Count > 0)
            {
                foreach (Hive oHive in arrHives)
                {
                    oHive.AnalyzeHive();
                    AddToListView(oHive);
                }

                this.buttonStart.Enabled = true;
            }
        }

        private void AddToListView(Hive oHive)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new AddToListViewDelegate(AddToListView), new object[] { oHive });
                return;
            }

            this.listView1.Items.Add(new ListViewItem(new string[] { oHive.fiHive.Name, GetSizeInMegaBytes(oHive.fiHive.Length), GetSizeInMegaBytes(oHive.fiHiveTemp.Length) }));
            this.progressBarAnalyzed.PerformStep();
            this.labelAction.Text = string.Format("Analyzing the registry: {0}%", ((100 * this.progressBarAnalyzed.Value) / this.progressBarAnalyzed.Maximum));
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            
        }

        private void Optimizer1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (tAnalyzeHives != null)
                if (tAnalyzeHives.IsAlive)
                    tAnalyzeHives.Abort();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            long lSeqNum = 0;

            SysRestore.StartRestore("Before Little Registry Cleaner Optimization", out lSeqNum);

            this.progressBarDefrag.Step = 1;
            this.progressBarDefrag.Maximum = arrHives.Count;
            this.labelAction.Text = "Optimizing the registry: 0%";

            foreach (Hive oHive in arrHives)
            {
                oHive.CompactHive();
                this.progressBarDefrag.PerformStep();
                this.labelAction.Text = string.Format("Optimizing the registry: {0}%", ((100 * this.progressBarDefrag.Value) / this.progressBarDefrag.Maximum));
            }

            SysRestore.EndRestore(lSeqNum);

            if (MessageBox.Show(this, "You must restart your computer before the new setting will take effect. Do you want to restart your computer now?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                ExitWindowsEx(0x02, MajorOperatingSystem | MinorReconfig | FlagPlanned);

            this.Close();
        }
    }
}
