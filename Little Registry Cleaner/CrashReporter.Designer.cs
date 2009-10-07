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
            this.checkBoxRestart = new System.Windows.Forms.CheckBox();
            this.pictureBoxErr = new System.Windows.Forms.PictureBox();
            this.buttonSend = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonDontSend = new System.Windows.Forms.Button();
            this.panelDevider = new System.Windows.Forms.Panel();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
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
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // checkBoxRestart
            // 
            resources.ApplyResources(this.checkBoxRestart, "checkBoxRestart");
            this.checkBoxRestart.Name = "checkBoxRestart";
            this.checkBoxRestart.UseVisualStyleBackColor = true;
            // 
            // pictureBoxErr
            // 
            resources.ApplyResources(this.pictureBoxErr, "pictureBoxErr");
            this.pictureBoxErr.Name = "pictureBoxErr";
            this.pictureBoxErr.TabStop = false;
            // 
            // buttonSend
            // 
            resources.ApplyResources(this.buttonSend, "buttonSend");
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // buttonDontSend
            // 
            this.buttonDontSend.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonDontSend, "buttonDontSend");
            this.buttonDontSend.Name = "buttonDontSend";
            this.buttonDontSend.UseVisualStyleBackColor = true;
            this.buttonDontSend.Click += new System.EventHandler(this.buttonDontSend_Click);
            // 
            // panelDevider
            // 
            this.panelDevider.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.panelDevider, "panelDevider");
            this.panelDevider.Name = "panelDevider";
            // 
            // richTextBox1
            // 
            resources.ApplyResources(this.richTextBox1, "richTextBox1");
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            // 
            // CrashReporter
            // 
            this.AcceptButton = this.buttonSend;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonDontSend;
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.panelDevider);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CrashReporter";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
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