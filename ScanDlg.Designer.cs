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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelProblems = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelSection = new System.Windows.Forms.Label();
            this.buttonStop = new System.Windows.Forms.Button();
            this.textBoxSubKey = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(6, 32);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(304, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelProblems);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.labelSection);
            this.groupBox1.Controls.Add(this.buttonStop);
            this.groupBox1.Controls.Add(this.textBoxSubKey);
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Location = new System.Drawing.Point(12, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(379, 111);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // labelProblems
            // 
            this.labelProblems.Location = new System.Drawing.Point(338, 16);
            this.labelProblems.Name = "labelProblems";
            this.labelProblems.Size = new System.Drawing.Size(35, 13);
            this.labelProblems.TabIndex = 6;
            this.labelProblems.Text = "0";
            this.labelProblems.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(257, 16);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Problems Found: ";
            // 
            // labelSection
            // 
            this.labelSection.AutoSize = true;
            this.labelSection.Location = new System.Drawing.Point(6, 16);
            this.labelSection.Name = "labelSection";
            this.labelSection.Size = new System.Drawing.Size(89, 13);
            this.labelSection.TabIndex = 4;
            this.labelSection.Text = "Scanning section";
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(316, 33);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(57, 23);
            this.buttonStop.TabIndex = 3;
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
            this.textBoxSubKey.TabIndex = 2;
            // 
            // ScanDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(403, 129);
            this.Controls.Add(this.groupBox1);
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

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxSubKey;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Label labelSection;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelProblems;
    }
}