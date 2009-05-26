namespace Little_Registry_Cleaner
{
    partial class CrashReporter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CrashReporter));
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBoxErr = new System.Windows.Forms.PictureBox();
            this.buttonSend = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonDontSend = new System.Windows.Forms.Button();
            this.panelDevider = new System.Windows.Forms.Panel();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.checkBoxRestart = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxErr)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.checkBoxRestart);
            this.panel1.Controls.Add(this.pictureBoxErr);
            this.panel1.Controls.Add(this.buttonSend);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.buttonDontSend);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(414, 91);
            this.panel1.TabIndex = 5;
            // 
            // pictureBoxErr
            // 
            this.pictureBoxErr.Location = new System.Drawing.Point(12, 15);
            this.pictureBoxErr.Name = "pictureBoxErr";
            this.pictureBoxErr.Size = new System.Drawing.Size(32, 32);
            this.pictureBoxErr.TabIndex = 6;
            this.pictureBoxErr.TabStop = false;
            // 
            // buttonSend
            // 
            this.buttonSend.Location = new System.Drawing.Point(212, 63);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(112, 23);
            this.buttonSend.TabIndex = 11;
            this.buttonSend.Text = "Send Error Report";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(50, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(362, 57);
            this.label1.TabIndex = 5;
            this.label1.Text = "An exception occured in a component of Little Registry Cleaner. We have created a" +
                "n error report that you can send us. We will treat this report as confidential a" +
                "nd anonymous.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonDontSend
            // 
            this.buttonDontSend.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonDontSend.Location = new System.Drawing.Point(330, 63);
            this.buttonDontSend.Name = "buttonDontSend";
            this.buttonDontSend.Size = new System.Drawing.Size(70, 23);
            this.buttonDontSend.TabIndex = 12;
            this.buttonDontSend.Text = "Don\'t Send";
            this.buttonDontSend.UseVisualStyleBackColor = true;
            this.buttonDontSend.Click += new System.EventHandler(this.buttonDontSend_Click);
            // 
            // panelDevider
            // 
            this.panelDevider.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelDevider.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelDevider.Location = new System.Drawing.Point(0, 91);
            this.panelDevider.Name = "panelDevider";
            this.panelDevider.Size = new System.Drawing.Size(414, 2);
            this.panelDevider.TabIndex = 8;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(0, 93);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(414, 340);
            this.richTextBox1.TabIndex = 9;
            this.richTextBox1.Text = "";
            this.richTextBox1.WordWrap = false;
            // 
            // checkBoxRestart
            // 
            this.checkBoxRestart.AutoSize = true;
            this.checkBoxRestart.Location = new System.Drawing.Point(12, 67);
            this.checkBoxRestart.Name = "checkBoxRestart";
            this.checkBoxRestart.Size = new System.Drawing.Size(102, 17);
            this.checkBoxRestart.TabIndex = 13;
            this.checkBoxRestart.Text = "Restart Program";
            this.checkBoxRestart.UseVisualStyleBackColor = true;
            // 
            // CrashReporter
            // 
            this.AcceptButton = this.buttonSend;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonDontSend;
            this.ClientSize = new System.Drawing.Size(414, 433);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.panelDevider);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CrashReporter";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Little Registry Cleaner - Crash Reporter";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ErrorDlg_FormClosed);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxErr)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelDevider;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.Button buttonDontSend;
        private System.Windows.Forms.PictureBox pictureBoxErr;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.CheckBox checkBoxRestart;


    }
}