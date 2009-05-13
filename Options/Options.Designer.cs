namespace Little_Registry_Cleaner
{
    partial class Options
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxShowLog = new System.Windows.Forms.CheckBox();
            this.checkBoxRemDrives = new System.Windows.Forms.CheckBox();
            this.checkBoxAutoUpdate = new System.Windows.Forms.CheckBox();
            this.checkBoxRestore = new System.Windows.Forms.CheckBox();
            this.checkBoxDeleteBackup = new System.Windows.Forms.CheckBox();
            this.checkBoxRescan = new System.Windows.Forms.CheckBox();
            this.checkBoxLog = new System.Windows.Forms.CheckBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.textBoxBackupFolder = new System.Windows.Forms.TextBox();
            this.buttonBrowse1 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.buttonBrowse2 = new System.Windows.Forms.Button();
            this.textBoxLogFolder = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeEntryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addRegistryPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxShowLog);
            this.groupBox1.Controls.Add(this.checkBoxRemDrives);
            this.groupBox1.Controls.Add(this.checkBoxAutoUpdate);
            this.groupBox1.Controls.Add(this.checkBoxRestore);
            this.groupBox1.Controls.Add(this.checkBoxDeleteBackup);
            this.groupBox1.Controls.Add(this.checkBoxRescan);
            this.groupBox1.Controls.Add(this.checkBoxLog);
            this.groupBox1.Location = new System.Drawing.Point(6, 112);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(444, 110);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            // 
            // checkBoxShowLog
            // 
            this.checkBoxShowLog.AutoSize = true;
            this.checkBoxShowLog.Location = new System.Drawing.Point(197, 19);
            this.checkBoxShowLog.Name = "checkBoxShowLog";
            this.checkBoxShowLog.Size = new System.Drawing.Size(179, 17);
            this.checkBoxShowLog.TabIndex = 12;
            this.checkBoxShowLog.Text = "Show log after finished scanning";
            this.checkBoxShowLog.UseVisualStyleBackColor = true;
            // 
            // checkBoxRemDrives
            // 
            this.checkBoxRemDrives.AutoSize = true;
            this.checkBoxRemDrives.Location = new System.Drawing.Point(197, 65);
            this.checkBoxRemDrives.Name = "checkBoxRemDrives";
            this.checkBoxRemDrives.Size = new System.Drawing.Size(212, 17);
            this.checkBoxRemDrives.TabIndex = 11;
            this.checkBoxRemDrives.Text = "Ignore missing files on removable media";
            this.checkBoxRemDrives.UseVisualStyleBackColor = true;
            // 
            // checkBoxAutoUpdate
            // 
            this.checkBoxAutoUpdate.AutoSize = true;
            this.checkBoxAutoUpdate.Location = new System.Drawing.Point(10, 65);
            this.checkBoxAutoUpdate.Name = "checkBoxAutoUpdate";
            this.checkBoxAutoUpdate.Size = new System.Drawing.Size(177, 17);
            this.checkBoxAutoUpdate.TabIndex = 10;
            this.checkBoxAutoUpdate.Text = "Check for updates automatically";
            this.checkBoxAutoUpdate.UseVisualStyleBackColor = true;
            // 
            // checkBoxRestore
            // 
            this.checkBoxRestore.AutoSize = true;
            this.checkBoxRestore.Location = new System.Drawing.Point(10, 87);
            this.checkBoxRestore.Name = "checkBoxRestore";
            this.checkBoxRestore.Size = new System.Drawing.Size(222, 17);
            this.checkBoxRestore.TabIndex = 9;
            this.checkBoxRestore.Text = "Create system restore points automatically";
            this.checkBoxRestore.UseVisualStyleBackColor = true;
            // 
            // checkBoxDeleteBackup
            // 
            this.checkBoxDeleteBackup.AutoSize = true;
            this.checkBoxDeleteBackup.Location = new System.Drawing.Point(197, 42);
            this.checkBoxDeleteBackup.Name = "checkBoxDeleteBackup";
            this.checkBoxDeleteBackup.Size = new System.Drawing.Size(236, 17);
            this.checkBoxDeleteBackup.TabIndex = 8;
            this.checkBoxDeleteBackup.Text = "Delete backup file after restoring the registry ";
            this.checkBoxDeleteBackup.UseVisualStyleBackColor = true;
            // 
            // checkBoxRescan
            // 
            this.checkBoxRescan.AutoSize = true;
            this.checkBoxRescan.Location = new System.Drawing.Point(10, 42);
            this.checkBoxRescan.Name = "checkBoxRescan";
            this.checkBoxRescan.Size = new System.Drawing.Size(190, 17);
            this.checkBoxRescan.TabIndex = 7;
            this.checkBoxRescan.Text = "Re-Scan after fixing registry issues ";
            this.checkBoxRescan.UseVisualStyleBackColor = true;
            // 
            // checkBoxLog
            // 
            this.checkBoxLog.AutoSize = true;
            this.checkBoxLog.Location = new System.Drawing.Point(10, 19);
            this.checkBoxLog.Name = "checkBoxLog";
            this.checkBoxLog.Size = new System.Drawing.Size(156, 17);
            this.checkBoxLog.TabIndex = 0;
            this.checkBoxLog.Text = "Log registry scans and fixes";
            this.checkBoxLog.UseVisualStyleBackColor = true;
            this.checkBoxLog.CheckedChanged += new System.EventHandler(this.checkBoxLog_CheckedChanged);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(369, 272);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(45, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(420, 272);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(59, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // textBoxBackupFolder
            // 
            this.textBoxBackupFolder.Location = new System.Drawing.Point(6, 19);
            this.textBoxBackupFolder.MaxLength = 260;
            this.textBoxBackupFolder.Name = "textBoxBackupFolder";
            this.textBoxBackupFolder.ReadOnly = true;
            this.textBoxBackupFolder.Size = new System.Drawing.Size(361, 20);
            this.textBoxBackupFolder.TabIndex = 3;
            // 
            // buttonBrowse1
            // 
            this.buttonBrowse1.Location = new System.Drawing.Point(373, 16);
            this.buttonBrowse1.Name = "buttonBrowse1";
            this.buttonBrowse1.Size = new System.Drawing.Size(68, 23);
            this.buttonBrowse1.TabIndex = 4;
            this.buttonBrowse1.Text = "Browse...";
            this.buttonBrowse1.UseVisualStyleBackColor = true;
            this.buttonBrowse1.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxBackupFolder);
            this.groupBox2.Controls.Add(this.buttonBrowse1);
            this.groupBox2.Location = new System.Drawing.Point(6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(444, 47);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Select a folder to backup deleted registry entries ";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(464, 254);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(456, 228);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.buttonBrowse2);
            this.groupBox3.Controls.Add(this.textBoxLogFolder);
            this.groupBox3.Location = new System.Drawing.Point(6, 59);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(444, 47);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Select a folder to save the log files to";
            // 
            // buttonBrowse2
            // 
            this.buttonBrowse2.Location = new System.Drawing.Point(374, 16);
            this.buttonBrowse2.Name = "buttonBrowse2";
            this.buttonBrowse2.Size = new System.Drawing.Size(67, 23);
            this.buttonBrowse2.TabIndex = 1;
            this.buttonBrowse2.Text = "Browse...";
            this.buttonBrowse2.UseVisualStyleBackColor = true;
            this.buttonBrowse2.Click += new System.EventHandler(this.buttonBrowse2_Click);
            // 
            // textBoxLogFolder
            // 
            this.textBoxLogFolder.Location = new System.Drawing.Point(6, 19);
            this.textBoxLogFolder.MaxLength = 260;
            this.textBoxLogFolder.Name = "textBoxLogFolder";
            this.textBoxLogFolder.ReadOnly = true;
            this.textBoxLogFolder.Size = new System.Drawing.Size(361, 20);
            this.textBoxLogFolder.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.listView1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(456, 228);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Ignore List";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(164, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Right click to add/remove entries";
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listView1.ContextMenuStrip = this.contextMenuStrip1;
            this.listView1.Location = new System.Drawing.Point(6, 22);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(444, 200);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Files and folders to exclude";
            this.columnHeader1.Width = 437;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addFileToolStripMenuItem,
            this.addFolderToolStripMenuItem,
            this.addRegistryPathToolStripMenuItem,
            this.toolStripSeparator1,
            this.removeEntryToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(169, 120);
            // 
            // removeEntryToolStripMenuItem
            // 
            this.removeEntryToolStripMenuItem.Name = "removeEntryToolStripMenuItem";
            this.removeEntryToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.removeEntryToolStripMenuItem.Text = "Remove Entry";
            this.removeEntryToolStripMenuItem.Click += new System.EventHandler(this.removeEntryToolStripMenuItem_Click);
            // 
            // addFileToolStripMenuItem
            // 
            this.addFileToolStripMenuItem.Name = "addFileToolStripMenuItem";
            this.addFileToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.addFileToolStripMenuItem.Text = "Add File";
            this.addFileToolStripMenuItem.Click += new System.EventHandler(this.addFileToolStripMenuItem_Click);
            // 
            // addFolderToolStripMenuItem
            // 
            this.addFolderToolStripMenuItem.Name = "addFolderToolStripMenuItem";
            this.addFolderToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.addFolderToolStripMenuItem.Text = "Add Folder";
            this.addFolderToolStripMenuItem.Click += new System.EventHandler(this.addFolderToolStripMenuItem_Click);
            // 
            // addRegistryPathToolStripMenuItem
            // 
            this.addRegistryPathToolStripMenuItem.Name = "addRegistryPathToolStripMenuItem";
            this.addRegistryPathToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.addRegistryPathToolStripMenuItem.Text = "Add Registry Path";
            this.addRegistryPathToolStripMenuItem.Click += new System.EventHandler(this.addRegistryPathToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(165, 6);
            // 
            // Options
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(488, 304);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Options";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Little Registry Cleaner - Options";
            this.Load += new System.EventHandler(this.Options_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBoxLog;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonBrowse1;
        private System.Windows.Forms.TextBox textBoxBackupFolder;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBoxDeleteBackup;
        private System.Windows.Forms.CheckBox checkBoxRescan;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem removeEntryToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxRestore;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBoxLogFolder;
        private System.Windows.Forms.Button buttonBrowse2;
        private System.Windows.Forms.CheckBox checkBoxAutoUpdate;
        private System.Windows.Forms.CheckBox checkBoxRemDrives;
        private System.Windows.Forms.CheckBox checkBoxShowLog;
        private System.Windows.Forms.ToolStripMenuItem addFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addRegistryPathToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}