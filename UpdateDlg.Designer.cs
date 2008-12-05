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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateDlg));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelInfo = new System.Windows.Forms.Label();
            this.buttonDownload = new System.Windows.Forms.Button();
            this.buttonChangelog = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(16, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(133, 128);
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // labelInfo
            // 
            this.labelInfo.Location = new System.Drawing.Point(13, 143);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(136, 13);
            this.labelInfo.TabIndex = 12;
            this.labelInfo.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // buttonDownload
            // 
            this.buttonDownload.Location = new System.Drawing.Point(16, 159);
            this.buttonDownload.Name = "buttonDownload";
            this.buttonDownload.Size = new System.Drawing.Size(63, 23);
            this.buttonDownload.TabIndex = 13;
            this.buttonDownload.Text = "Download";
            this.buttonDownload.UseVisualStyleBackColor = true;
            this.buttonDownload.Click += new System.EventHandler(this.buttonDownload_Click);
            // 
            // buttonChangelog
            // 
            this.buttonChangelog.Location = new System.Drawing.Point(81, 159);
            this.buttonChangelog.Name = "buttonChangelog";
            this.buttonChangelog.Size = new System.Drawing.Size(68, 23);
            this.buttonChangelog.TabIndex = 14;
            this.buttonChangelog.Text = "Changelog";
            this.buttonChangelog.UseVisualStyleBackColor = true;
            this.buttonChangelog.Click += new System.EventHandler(this.buttonChangelog_Click);
            // 
            // UpdateDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(160, 189);
            this.Controls.Add(this.buttonChangelog);
            this.Controls.Add(this.buttonDownload);
            this.Controls.Add(this.labelInfo);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdateDlg";
            this.ShowInTaskbar = false;
            this.Text = "Check for update";
            this.Load += new System.EventHandler(this.UpdateDlg_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.Button buttonDownload;
        private System.Windows.Forms.Button buttonChangelog;
    }
}