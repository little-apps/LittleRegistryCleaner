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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Options));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listViewOptions = new System.Windows.Forms.ListView();
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
            this.addFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addRegistryPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.removeEntryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.groupBox1.Controls.Add(this.listViewOptions);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // listViewOptions
            // 
            this.listViewOptions.CheckBoxes = true;
            this.listViewOptions.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("listViewOptions.Items"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("listViewOptions.Items1"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("listViewOptions.Items2"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("listViewOptions.Items3"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("listViewOptions.Items4"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("listViewOptions.Items5"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("listViewOptions.Items6")))});
            resources.ApplyResources(this.listViewOptions, "listViewOptions");
            this.listViewOptions.Name = "listViewOptions";
            this.listViewOptions.UseCompatibleStateImageBehavior = false;
            this.listViewOptions.View = System.Windows.Forms.View.List;
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // textBoxBackupFolder
            // 
            resources.ApplyResources(this.textBoxBackupFolder, "textBoxBackupFolder");
            this.textBoxBackupFolder.Name = "textBoxBackupFolder";
            this.textBoxBackupFolder.ReadOnly = true;
            // 
            // buttonBrowse1
            // 
            resources.ApplyResources(this.buttonBrowse1, "buttonBrowse1");
            this.buttonBrowse1.Name = "buttonBrowse1";
            this.buttonBrowse1.UseVisualStyleBackColor = true;
            this.buttonBrowse1.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxBackupFolder);
            this.groupBox2.Controls.Add(this.buttonBrowse1);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.groupBox2);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.buttonBrowse2);
            this.groupBox3.Controls.Add(this.textBoxLogFolder);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // buttonBrowse2
            // 
            resources.ApplyResources(this.buttonBrowse2, "buttonBrowse2");
            this.buttonBrowse2.Name = "buttonBrowse2";
            this.buttonBrowse2.UseVisualStyleBackColor = true;
            this.buttonBrowse2.Click += new System.EventHandler(this.buttonBrowse2_Click);
            // 
            // textBoxLogFolder
            // 
            resources.ApplyResources(this.textBoxLogFolder, "textBoxLogFolder");
            this.textBoxLogFolder.Name = "textBoxLogFolder";
            this.textBoxLogFolder.ReadOnly = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.listView1);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listView1.ContextMenuStrip = this.contextMenuStrip1;
            resources.ApplyResources(this.listView1, "listView1");
            this.listView1.Name = "listView1";
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
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
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            // 
            // addFileToolStripMenuItem
            // 
            this.addFileToolStripMenuItem.Name = "addFileToolStripMenuItem";
            resources.ApplyResources(this.addFileToolStripMenuItem, "addFileToolStripMenuItem");
            this.addFileToolStripMenuItem.Click += new System.EventHandler(this.addFileToolStripMenuItem_Click);
            // 
            // addFolderToolStripMenuItem
            // 
            this.addFolderToolStripMenuItem.Name = "addFolderToolStripMenuItem";
            resources.ApplyResources(this.addFolderToolStripMenuItem, "addFolderToolStripMenuItem");
            this.addFolderToolStripMenuItem.Click += new System.EventHandler(this.addFolderToolStripMenuItem_Click);
            // 
            // addRegistryPathToolStripMenuItem
            // 
            this.addRegistryPathToolStripMenuItem.Name = "addRegistryPathToolStripMenuItem";
            resources.ApplyResources(this.addRegistryPathToolStripMenuItem, "addRegistryPathToolStripMenuItem");
            this.addRegistryPathToolStripMenuItem.Click += new System.EventHandler(this.addRegistryPathToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // removeEntryToolStripMenuItem
            // 
            this.removeEntryToolStripMenuItem.Name = "removeEntryToolStripMenuItem";
            resources.ApplyResources(this.removeEntryToolStripMenuItem, "removeEntryToolStripMenuItem");
            this.removeEntryToolStripMenuItem.Click += new System.EventHandler(this.removeEntryToolStripMenuItem_Click);
            // 
            // Options
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
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
            this.Load += new System.EventHandler(this.Options_Load);
            this.groupBox1.ResumeLayout(false);
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
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonBrowse1;
        private System.Windows.Forms.TextBox textBoxBackupFolder;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem removeEntryToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBoxLogFolder;
        private System.Windows.Forms.Button buttonBrowse2;
        private System.Windows.Forms.ToolStripMenuItem addFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addRegistryPathToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ListView listViewOptions;
    }
}