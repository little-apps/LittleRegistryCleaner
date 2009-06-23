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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.progressBar = new Common_Tools.XpProgressBar();
            this.labelProblems = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonStop = new System.Windows.Forms.Button();
            this.textBoxSubKey = new System.Windows.Forms.TextBox();
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
            this.groupBox1.Location = new System.Drawing.Point(12, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(379, 111);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // progressBar
            // 
            this.progressBar.ColorBackGround = System.Drawing.Color.White;
            this.progressBar.ColorBarBorder = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(240)))), ((int)(((byte)(170)))));
            this.progressBar.ColorBarCenter = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(150)))), ((int)(((byte)(10)))));
            this.progressBar.ColorText = System.Drawing.Color.Black;
            this.progressBar.Location = new System.Drawing.Point(6, 32);
            this.progressBar.Name = "progressBar";
            this.progressBar.Position = 0;
            this.progressBar.PositionMax = 100;
            this.progressBar.PositionMin = 0;
            this.progressBar.Size = new System.Drawing.Size(300, 24);
            this.progressBar.SteepDistance = ((byte)(0));
            this.progressBar.TabIndex = 2;
            // 
            // labelProblems
            // 
            this.labelProblems.Location = new System.Drawing.Point(91, 16);
            this.labelProblems.Name = "labelProblems";
            this.labelProblems.Size = new System.Drawing.Size(67, 13);
            this.labelProblems.TabIndex = 4;
            this.labelProblems.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Problems Found: ";
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(316, 33);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(57, 23);
            this.buttonStop.TabIndex = 0;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // textBoxSubKey
            // 
            this.textBoxSubKey.Location = new System.Drawing.Point(6, 62);
            this.textBoxSubKey.Multiline = true;
            this.textBoxSubKey.Name = "textBoxSubKey";
            this.textBoxSubKey.ReadOnly = true;
            this.textBoxSubKey.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.textBoxSubKey.Size = new System.Drawing.Size(367, 40);
            this.textBoxSubKey.TabIndex = 1;
            // 
            // ScanDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(403, 129);
            this.Controls.Add(this.groupBox1);
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ScanDlg";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Little Registry Cleaner - Scanning...";
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
    }
}