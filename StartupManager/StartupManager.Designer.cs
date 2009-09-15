namespace Little_Registry_Cleaner.StartupManager
{
    partial class StartupManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartupManager));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.treeViewAdv1 = new Common_Tools.TreeViewAdv.Tree.TreeViewAdv();
            this.treeColumn1 = new Common_Tools.TreeViewAdv.Tree.TreeColumn();
            this.treeColumn2 = new Common_Tools.TreeViewAdv.Tree.TreeColumn();
            this.treeColumn3 = new Common_Tools.TreeViewAdv.Tree.TreeColumn();
            this.nodeIcon1 = new Common_Tools.TreeViewAdv.Tree.NodeControls.NodeIcon();
            this.nodeTextBoxSection = new Common_Tools.TreeViewAdv.Tree.NodeControls.NodeTextBox();
            this.nodeTextBoxItem = new Common_Tools.TreeViewAdv.Tree.NodeControls.NodeTextBox();
            this.nodeTextBoxPath = new Common_Tools.TreeViewAdv.Tree.NodeControls.NodeTextBox();
            this.nodeTextBoxArgs = new Common_Tools.TreeViewAdv.Tree.NodeControls.NodeTextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonAdd = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonEdit = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonView = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRun = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonRefresh = new System.Windows.Forms.ToolStripButton();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.tableLayoutPanel1);
            resources.ApplyResources(this.toolStripContainer1.ContentPanel, "toolStripContainer1.ContentPanel");
            resources.ApplyResources(this.toolStripContainer1, "toolStripContainer1");
            this.toolStripContainer1.Name = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.treeViewAdv1, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // treeViewAdv1
            // 
            this.treeViewAdv1.BackColor = System.Drawing.SystemColors.Window;
            this.treeViewAdv1.Columns.Add(this.treeColumn1);
            this.treeViewAdv1.Columns.Add(this.treeColumn2);
            this.treeViewAdv1.Columns.Add(this.treeColumn3);
            this.treeViewAdv1.DefaultToolTipProvider = null;
            resources.ApplyResources(this.treeViewAdv1, "treeViewAdv1");
            this.treeViewAdv1.DragDropMarkColor = System.Drawing.Color.Black;
            this.treeViewAdv1.FullRowSelect = true;
            this.treeViewAdv1.GridLineStyle = Common_Tools.TreeViewAdv.Tree.GridLineStyle.Horizontal;
            this.treeViewAdv1.LineColor = System.Drawing.SystemColors.ControlDark;
            this.treeViewAdv1.Model = null;
            this.treeViewAdv1.Name = "treeViewAdv1";
            this.treeViewAdv1.NodeControls.Add(this.nodeIcon1);
            this.treeViewAdv1.NodeControls.Add(this.nodeTextBoxSection);
            this.treeViewAdv1.NodeControls.Add(this.nodeTextBoxItem);
            this.treeViewAdv1.NodeControls.Add(this.nodeTextBoxPath);
            this.treeViewAdv1.NodeControls.Add(this.nodeTextBoxArgs);
            this.treeViewAdv1.SelectedNode = null;
            this.treeViewAdv1.UseColumns = true;
            // 
            // treeColumn1
            // 
            resources.ApplyResources(this.treeColumn1, "treeColumn1");
            this.treeColumn1.SortOrder = System.Windows.Forms.SortOrder.None;
            this.treeColumn1.TooltipText = null;
            // 
            // treeColumn2
            // 
            resources.ApplyResources(this.treeColumn2, "treeColumn2");
            this.treeColumn2.SortOrder = System.Windows.Forms.SortOrder.None;
            this.treeColumn2.TooltipText = null;
            // 
            // treeColumn3
            // 
            resources.ApplyResources(this.treeColumn3, "treeColumn3");
            this.treeColumn3.SortOrder = System.Windows.Forms.SortOrder.None;
            this.treeColumn3.TooltipText = null;
            // 
            // nodeIcon1
            // 
            this.nodeIcon1.DataPropertyName = "Image";
            this.nodeIcon1.LeftMargin = 1;
            this.nodeIcon1.ParentColumn = this.treeColumn1;
            // 
            // nodeTextBoxSection
            // 
            this.nodeTextBoxSection.DataPropertyName = "Section";
            this.nodeTextBoxSection.EditEnabled = false;
            this.nodeTextBoxSection.IncrementalSearchEnabled = true;
            this.nodeTextBoxSection.LeftMargin = 3;
            this.nodeTextBoxSection.ParentColumn = this.treeColumn1;
            this.nodeTextBoxSection.UseCompatibleTextRendering = true;
            // 
            // nodeTextBoxItem
            // 
            this.nodeTextBoxItem.DataPropertyName = "Item";
            this.nodeTextBoxItem.EditEnabled = false;
            this.nodeTextBoxItem.IncrementalSearchEnabled = true;
            this.nodeTextBoxItem.LeftMargin = 3;
            this.nodeTextBoxItem.ParentColumn = this.treeColumn1;
            // 
            // nodeTextBoxPath
            // 
            this.nodeTextBoxPath.DataPropertyName = "Path";
            this.nodeTextBoxPath.EditEnabled = false;
            this.nodeTextBoxPath.IncrementalSearchEnabled = true;
            this.nodeTextBoxPath.LeftMargin = 3;
            this.nodeTextBoxPath.ParentColumn = this.treeColumn2;
            // 
            // nodeTextBoxArgs
            // 
            this.nodeTextBoxArgs.DataPropertyName = "Args";
            this.nodeTextBoxArgs.EditEnabled = false;
            this.nodeTextBoxArgs.IncrementalSearchEnabled = true;
            this.nodeTextBoxArgs.LeftMargin = 3;
            this.nodeTextBoxArgs.ParentColumn = this.treeColumn3;
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonAdd,
            this.toolStripButtonEdit,
            this.toolStripButtonDelete,
            this.toolStripButtonView,
            this.toolStripButtonRun,
            this.toolStripSeparator1,
            this.toolStripButtonRefresh});
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Stretch = true;
            // 
            // toolStripButtonAdd
            // 
            resources.ApplyResources(this.toolStripButtonAdd, "toolStripButtonAdd");
            this.toolStripButtonAdd.Name = "toolStripButtonAdd";
            this.toolStripButtonAdd.Click += new System.EventHandler(this.toolStripButtonAdd_Click);
            // 
            // toolStripButtonEdit
            // 
            resources.ApplyResources(this.toolStripButtonEdit, "toolStripButtonEdit");
            this.toolStripButtonEdit.Name = "toolStripButtonEdit";
            this.toolStripButtonEdit.Click += new System.EventHandler(this.toolStripButtonEdit_Click);
            // 
            // toolStripButtonDelete
            // 
            resources.ApplyResources(this.toolStripButtonDelete, "toolStripButtonDelete");
            this.toolStripButtonDelete.Name = "toolStripButtonDelete";
            this.toolStripButtonDelete.Click += new System.EventHandler(this.toolStripButtonDelete_Click);
            // 
            // toolStripButtonView
            // 
            resources.ApplyResources(this.toolStripButtonView, "toolStripButtonView");
            this.toolStripButtonView.Name = "toolStripButtonView";
            this.toolStripButtonView.Click += new System.EventHandler(this.toolStripButtonView_Click);
            // 
            // toolStripButtonRun
            // 
            resources.ApplyResources(this.toolStripButtonRun, "toolStripButtonRun");
            this.toolStripButtonRun.Name = "toolStripButtonRun";
            this.toolStripButtonRun.Click += new System.EventHandler(this.toolStripButtonRun_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // toolStripButtonRefresh
            // 
            resources.ApplyResources(this.toolStripButtonRefresh, "toolStripButtonRefresh");
            this.toolStripButtonRefresh.Name = "toolStripButtonRefresh";
            this.toolStripButtonRefresh.Click += new System.EventHandler(this.toolStripButtonRefresh_Click);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "User");
            this.imageList.Images.SetKeyName(1, "Users");
            // 
            // StartupManager
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStripContainer1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StartupManager";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.StartupManager_Load);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonDelete;
        private System.Windows.Forms.ToolStripButton toolStripButtonRefresh;
        private System.Windows.Forms.ToolStripButton toolStripButtonRun;
        private System.Windows.Forms.ToolStripButton toolStripButtonView;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ToolStripButton toolStripButtonAdd;
        private System.Windows.Forms.ToolStripButton toolStripButtonEdit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private Common_Tools.TreeViewAdv.Tree.TreeViewAdv treeViewAdv1;
        private Common_Tools.TreeViewAdv.Tree.TreeColumn treeColumn1;
        private Common_Tools.TreeViewAdv.Tree.TreeColumn treeColumn2;
        private Common_Tools.TreeViewAdv.Tree.TreeColumn treeColumn3;
        private Common_Tools.TreeViewAdv.Tree.NodeControls.NodeIcon nodeIcon1;
        private Common_Tools.TreeViewAdv.Tree.NodeControls.NodeTextBox nodeTextBoxSection;
        private Common_Tools.TreeViewAdv.Tree.NodeControls.NodeTextBox nodeTextBoxItem;
        private Common_Tools.TreeViewAdv.Tree.NodeControls.NodeTextBox nodeTextBoxPath;
        private Common_Tools.TreeViewAdv.Tree.NodeControls.NodeTextBox nodeTextBoxArgs;

    }
}