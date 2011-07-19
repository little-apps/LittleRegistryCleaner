/*
    Little Registry Cleaner
    Copyright (C) 2008-2011 Little Apps (http://www.little-apps.org/)

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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Common_Tools.TaskScheduler;

namespace Little_Registry_Cleaner
{
    public partial class Options : Form
    {
        private ExcludeList.ExcludeArray _arrayExclude = new ExcludeList.ExcludeArray();

        public ExcludeList.ExcludeArray ExcludeArray
        {
            get { return _arrayExclude; }
        }

        public Options()
        {
            InitializeComponent();
        }

        private void Options_Load(object sender, EventArgs e)
        {
            // Load settings
            this.listViewOptions.Items[0].Checked = Properties.Settings.Default.bOptionsLog;
            this.listViewOptions.Items[1].Checked = Properties.Settings.Default.bOptionsRescan;
            this.listViewOptions.Items[2].Checked = Properties.Settings.Default.bOptionsAutoUpdate;
            this.listViewOptions.Items[3].Checked = Properties.Settings.Default.bOptionsRestore;
            this.listViewOptions.Items[4].Checked = Properties.Settings.Default.bOptionsShowLog;
            this.listViewOptions.Items[5].Checked = Properties.Settings.Default.bOptionsDelBackup;
            this.listViewOptions.Items[6].Checked = Properties.Settings.Default.bOptionsRemMedia;
            this.listViewOptions.Items[7].Checked = Properties.Settings.Default.bOptionsAutoRepair;
            this.listViewOptions.Items[8].Checked = Properties.Settings.Default.bOptionsAutoExit; 

            // Load backup directorys
            this.textBoxBackupFolder.Text = Properties.Settings.Default.strOptionsBackupDir;
            this.textBoxLogFolder.Text = Properties.Settings.Default.strOptionsLogDir;

            // Load exclude list from settings
            if (Properties.Settings.Default.arrayExcludeList != null)
                this._arrayExclude = new ExcludeList.ExcludeArray(Properties.Settings.Default.arrayExcludeList);

            this.comboBoxDay.DataSource = Enum.GetValues(typeof(DaysOfTheWeek));
            this.comboBoxDay.SelectedItem = DaysOfTheWeek.Sunday;

            this.comboBoxDate.SelectedIndex = 0;

            PopulateExcludeList();

            GetJobInfo();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // Update settings
            Properties.Settings.Default.bOptionsLog = this.listViewOptions.Items[0].Checked;
            Properties.Settings.Default.bOptionsRescan = this.listViewOptions.Items[1].Checked;
            Properties.Settings.Default.bOptionsAutoUpdate = this.listViewOptions.Items[2].Checked;
            Properties.Settings.Default.bOptionsRestore = this.listViewOptions.Items[3].Checked;
            Properties.Settings.Default.bOptionsShowLog = this.listViewOptions.Items[4].Checked;
            Properties.Settings.Default.bOptionsDelBackup = this.listViewOptions.Items[5].Checked;
            Properties.Settings.Default.bOptionsRemMedia = this.listViewOptions.Items[6].Checked;
            Properties.Settings.Default.bOptionsAutoRepair = this.listViewOptions.Items[7].Checked;
            Properties.Settings.Default.bOptionsAutoExit = this.listViewOptions.Items[8].Checked;

            if (this.textBoxBackupFolder.Text != Properties.Settings.Default.strOptionsBackupDir)
                Properties.Settings.Default.strOptionsBackupDir = this.textBoxBackupFolder.Text;

            if (this.textBoxLogFolder.Text != Properties.Settings.Default.strOptionsLogDir)
                Properties.Settings.Default.strOptionsLogDir = this.textBoxLogFolder.Text;

            Properties.Settings.Default.arrayExcludeList = new ExcludeList.ExcludeArray(this._arrayExclude);

            SaveJobInfo();

            this.Close();
        }

        #region General

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDlg = new FolderBrowserDialog())
            {
                folderBrowserDlg.Description = Properties.Resources.optionsSelectBackupDir;
                folderBrowserDlg.SelectedPath = this.textBoxBackupFolder.Text;
                folderBrowserDlg.ShowNewFolderButton = true;

                if (folderBrowserDlg.ShowDialog(this) == DialogResult.OK)
                    this.textBoxBackupFolder.Text = folderBrowserDlg.SelectedPath;
            }
        }

        private void buttonBrowse2_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDlg = new FolderBrowserDialog())
            {
                folderBrowserDlg.Description = Properties.Resources.optionsSelectLogDir;
                folderBrowserDlg.SelectedPath = this.textBoxLogFolder.Text;
                folderBrowserDlg.ShowNewFolderButton = true;

                if (folderBrowserDlg.ShowDialog(this) == DialogResult.OK)
                    this.textBoxLogFolder.Text = folderBrowserDlg.SelectedPath;
            }
        }

        #endregion

        #region Exclude List
        private void addRegistryPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExcludeList.AddRegistryPath addRegistryPath = new ExcludeList.AddRegistryPath();
            if (addRegistryPath.ShowDialog(this) == DialogResult.OK)
            {
                ExcludeArray.Add(new ExcludeList.ExcludeItem(addRegistryPath.RegistryPath, null, null));
                PopulateExcludeList();
            }
        }

        private void addFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = Properties.Resources.optionsExcludeFile;
                openFileDialog.Filter = "All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    ExcludeArray.Add(new ExcludeList.ExcludeItem(null, null, openFileDialog.FileName));
                    PopulateExcludeList();
                }
            }
        }

        private void addFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = Properties.Resources.optionsExcludeDir;

                if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
                {
                    ExcludeArray.Add(new ExcludeList.ExcludeItem(null, folderBrowserDialog.SelectedPath, null));
                    PopulateExcludeList();
                }
            }
        }

        private void removeEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedIndices.Count > 0 && this.listView1.Items.Count > 0)
            {
                if (MessageBox.Show(this, Properties.Resources.optionsExcludeRemove, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    foreach (ExcludeList.ExcludeItem i in ExcludeArray)
                    {
                        if (i.ToString() == this.listView1.SelectedItems[0].Text)
                        {
                            ExcludeArray.Remove(i);
                            break;
                        }
                    }

                    PopulateExcludeList();
                }
            }
        }

        private void PopulateExcludeList()
        {
            this.listView1.Items.Clear();

            foreach (ExcludeList.ExcludeItem item in ExcludeArray)
                this.listView1.Items.Add(item.ToString());

            this.listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }
        #endregion

        #region Scheduler
        TaskService ts = new TaskService(); 

        /// <summary>
        /// Get scheduler info thru job ID
        /// </summary>
        private void GetJobInfo()
        {
            // Reset controls
            this.labelDay.Visible = false;
            this.labelDate.Visible = false;
            this.labelTime.Visible = false;
            this.dateTimePickerSched.Visible = false;
            this.comboBoxDate.Visible = false;
            this.comboBoxDay.Visible = false;

            using (Task t = ts.GetTask("Little Registry Cleaner"))
            {
                if (t == null)
                {
                    // Task not found
                    this.radioButtonNever.Checked = true;
                    return;
                }

                TaskDefinition td = t.Definition;

                if (td.Triggers.Count == 0)
                {
                    // Task is invalid
                    this.radioButtonNever.Checked = true;
                    return;
                }

                Trigger trigger = td.Triggers[0];
                if (trigger is DailyTrigger)
                {
                    this.radioButtonDaily.Checked = true;
                    UpdateScheduler(null, new EventArgs());

                    int hour = (trigger as DailyTrigger).StartBoundary.Hour;
                    int min = (trigger as DailyTrigger).StartBoundary.Minute;
                    this.dateTimePickerSched.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, hour, min, 0);
                }
                else if (trigger is WeeklyTrigger)
                {
                    this.radioButtonWeekly.Checked = true;
                    UpdateScheduler(null, new EventArgs());

                    int hour = (trigger as WeeklyTrigger).StartBoundary.Hour;
                    int min = (trigger as WeeklyTrigger).StartBoundary.Minute;
                    this.dateTimePickerSched.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, hour, min, 0);

                    DaysOfTheWeek dow = (trigger as WeeklyTrigger).DaysOfWeek;
                    this.comboBoxDay.SelectedItem = dow;
                }
                else if (trigger is MonthlyTrigger)
                {
                    this.radioButtonMonthly.Checked = true;
                    UpdateScheduler(null, new EventArgs());

                    this.comboBoxDate.SelectedItem = (trigger as MonthlyTrigger).DaysOfMonth[0].ToString();

                    int hour = (trigger as MonthlyTrigger).StartBoundary.Hour;
                    int min = (trigger as MonthlyTrigger).StartBoundary.Minute;
                    this.dateTimePickerSched.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, hour, min, 0);
                }
            }
        }

        private void SaveJobInfo()
        {
            if (ts.GetTask("Little Registry Cleaner") != null)
                ts.RootFolder.DeleteTask("Little Registry Cleaner");

            TaskDefinition td = ts.NewTask();

            td.RegistrationInfo.Date = DateTime.Now;
            td.RegistrationInfo.Description = "Runs a scan with Little Registry Cleaner";
            td.RegistrationInfo.Source = "Little Registry Cleaner";
            td.Principal.RunLevel = TaskRunLevel.Highest;

            if (this.radioButtonNever.Checked)
            {
                return;
            }
            else if (this.radioButtonDaily.Checked)
            {
                td.Triggers.Add(new DailyTrigger() { StartBoundary = this.dateTimePickerSched.Value });
            }
            else if (this.radioButtonWeekly.Checked)
            {
                DaysOfTheWeek dow = (DaysOfTheWeek)this.comboBoxDay.SelectedItem;

                td.Triggers.Add(new WeeklyTrigger(dow) { StartBoundary = this.dateTimePickerSched.Value });
            }
            else if (this.radioButtonMonthly.Checked)
            {
                int dom = Convert.ToInt32(this.comboBoxDate.SelectedItem);

                td.Triggers.Add(new MonthlyTrigger(dom) { StartBoundary = this.dateTimePickerSched.Value });
            }

            td.Actions.Add(new ExecAction(Application.ExecutablePath, "/scan"));

            ts.RootFolder.RegisterTaskDefinition("Little Registry Cleaner", td);
        }

        private void UpdateScheduler(object sender, EventArgs e)
        {
            // Reset controls
            this.labelDay.Visible = false;
            this.labelDate.Visible = false;
            this.labelTime.Visible = false;
            this.dateTimePickerSched.Visible = false;
            this.comboBoxDate.Visible = false;
            this.comboBoxDay.Visible = false;

            if (this.radioButtonNever.Checked)
            {
                this.groupBoxSchedule.Visible = false;
                this.groupBoxDesc.Visible = false;

                this.labelDescription.Text = "";
            }
            else if (this.radioButtonDaily.Checked)
            {
                this.groupBoxSchedule.Visible = true;
                this.groupBoxDesc.Visible = true;
                this.labelTime.Visible = true;
                this.dateTimePickerSched.Visible = true;

                this.labelDescription.Text = string.Format(Properties.Resources.optionsSchedDescD, this.dateTimePickerSched.Value.ToShortTimeString());
            }
            else if (this.radioButtonWeekly.Checked)
            {
                this.groupBoxSchedule.Visible = true;
                this.labelTime.Visible = true;
                this.dateTimePickerSched.Visible = true;
                this.labelDay.Visible = true;
                this.comboBoxDay.Visible = true;
                this.groupBoxDesc.Visible = true;

                this.labelDescription.Text = string.Format(Properties.Resources.optionsSchedDescW, this.comboBoxDay.SelectedItem, this.dateTimePickerSched.Value.ToShortTimeString());
            }
            else if (this.radioButtonMonthly.Checked)
            {
                this.groupBoxSchedule.Visible = true;
                this.labelTime.Visible = true;
                this.dateTimePickerSched.Visible = true;
                this.labelDate.Visible = true;
                this.comboBoxDate.Visible = true;
                this.groupBoxDesc.Visible = true;

                string numSuffix = Utils.GetNumberSuffix(Convert.ToInt32(this.comboBoxDate.SelectedItem));
                this.labelDescription.Text = string.Format(Properties.Resources.optionsSchedDescM, numSuffix, this.dateTimePickerSched.Value.ToShortTimeString());
            }
        }
        #endregion

        
    }
}
