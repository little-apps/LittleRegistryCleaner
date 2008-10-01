namespace Little_Registry_Cleaner
{
    partial class UpdateDlg
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonDownload = new System.Windows.Forms.Button();
            this.buttonChangelog = new System.Windows.Forms.Button();
            this.labelCurrentVer = new System.Windows.Forms.Label();
            this.labelLatestVer = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Current Version:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Newest Version:";
            // 
            // buttonDownload
            // 
            this.buttonDownload.Enabled = false;
            this.buttonDownload.Location = new System.Drawing.Point(57, 71);
            this.buttonDownload.Name = "buttonDownload";
            this.buttonDownload.Size = new System.Drawing.Size(63, 23);
            this.buttonDownload.TabIndex = 5;
            this.buttonDownload.Text = "Download";
            this.buttonDownload.UseVisualStyleBackColor = true;
            this.buttonDownload.Click += new System.EventHandler(this.buttonDownload_Click);
            // 
            // buttonChangelog
            // 
            this.buttonChangelog.Enabled = false;
            this.buttonChangelog.Location = new System.Drawing.Point(126, 71);
            this.buttonChangelog.Name = "buttonChangelog";
            this.buttonChangelog.Size = new System.Drawing.Size(92, 23);
            this.buttonChangelog.TabIndex = 6;
            this.buttonChangelog.Text = "View Changelog";
            this.buttonChangelog.UseVisualStyleBackColor = true;
            this.buttonChangelog.Click += new System.EventHandler(this.buttonChangelog_Click);
            // 
            // labelCurrentVer
            // 
            this.labelCurrentVer.AutoSize = true;
            this.labelCurrentVer.Location = new System.Drawing.Point(96, 29);
            this.labelCurrentVer.Name = "labelCurrentVer";
            this.labelCurrentVer.Size = new System.Drawing.Size(0, 13);
            this.labelCurrentVer.TabIndex = 7;
            // 
            // labelLatestVer
            // 
            this.labelLatestVer.AutoSize = true;
            this.labelLatestVer.Location = new System.Drawing.Point(96, 50);
            this.labelLatestVer.Name = "labelLatestVer";
            this.labelLatestVer.Size = new System.Drawing.Size(0, 13);
            this.labelLatestVer.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(202, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Checking if a newer version is available...";
            // 
            // UpdateDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(224, 104);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.labelLatestVer);
            this.Controls.Add(this.buttonDownload);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelCurrentVer);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonChangelog);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdateDlg";
            this.ShowInTaskbar = false;
            this.Text = "Check for update";
            this.Shown += new System.EventHandler(this.CheckForUpdate_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonDownload;
        private System.Windows.Forms.Button buttonChangelog;
        private System.Windows.Forms.Label labelCurrentVer;
        private System.Windows.Forms.Label labelLatestVer;
        private System.Windows.Forms.Label label3;
    }
}