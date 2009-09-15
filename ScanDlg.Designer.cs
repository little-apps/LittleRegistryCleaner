namespace Little_Registry_Cleaner
{
    partial class ScanDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScanDlg));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.progressBar = new Common_Tools.XpProgressBar();
            this.labelProblems = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonStop = new System.Windows.Forms.Button();
            this.textBoxSubKey = new System.Windows.Forms.TextBox();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.progressBar);
            this.groupBox1.Controls.Add(this.labelProblems);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.buttonStop);
            this.groupBox1.Controls.Add(this.textBoxSubKey);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // progressBar
            // 
            this.progressBar.ColorBackGround = System.Drawing.Color.White;
            this.progressBar.ColorBarBorder = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(240)))), ((int)(((byte)(170)))));
            this.progressBar.ColorBarCenter = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(150)))), ((int)(((byte)(10)))));
            this.progressBar.ColorText = System.Drawing.Color.Black;
            resources.ApplyResources(this.progressBar, "progressBar");
            this.progressBar.Name = "progressBar";
            this.progressBar.Position = 0;
            this.progressBar.PositionMax = 100;
            this.progressBar.PositionMin = 0;
            this.progressBar.SteepDistance = ((byte)(0));
            // 
            // labelProblems
            // 
            resources.ApplyResources(this.labelProblems, "labelProblems");
            this.labelProblems.Name = "labelProblems";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // buttonStop
            // 
            resources.ApplyResources(this.buttonStop, "buttonStop");
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // textBoxSubKey
            // 
            resources.ApplyResources(this.textBoxSubKey, "textBoxSubKey");
            this.textBoxSubKey.Name = "textBoxSubKey";
            this.textBoxSubKey.ReadOnly = true;
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "ActiveX/COM Objects");
            this.imageList.Images.SetKeyName(1, "Application Info");
            this.imageList.Images.SetKeyName(2, "System Drivers");
            this.imageList.Images.SetKeyName(3, "Windows Fonts");
            this.imageList.Images.SetKeyName(4, "Windows Help Files");
            this.imageList.Images.SetKeyName(5, "Recent Documents");
            this.imageList.Images.SetKeyName(6, "programlocations.ico");
            this.imageList.Images.SetKeyName(7, "Shared DLLs");
            this.imageList.Images.SetKeyName(8, "Application Settings");
            this.imageList.Images.SetKeyName(9, "Windows Sounds");
            this.imageList.Images.SetKeyName(10, "Startup Files");
            // 
            // ScanDlg
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ScanDlg";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Shown += new System.EventHandler(this.ScanDlg_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ScanDlg_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxSubKey;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelProblems;
        private Common_Tools.XpProgressBar progressBar;
        private System.Windows.Forms.ImageList imageList;
    }
}