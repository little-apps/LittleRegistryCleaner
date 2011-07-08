namespace Little_Startup_Manager
{
    partial class NewRunItem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewRunItem));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxArgs = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxPath = new System.Windows.Forms.TextBox();
            this.buttonFileDlg = new System.Windows.Forms.Button();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxSection = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxArgs);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textBoxPath);
            this.groupBox1.Controls.Add(this.buttonFileDlg);
            this.groupBox1.Controls.Add(this.textBoxName);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // textBoxArgs
            // 
            resources.ApplyResources(this.textBoxArgs, "textBoxArgs");
            this.textBoxArgs.Name = "textBoxArgs";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // textBoxPath
            // 
            resources.ApplyResources(this.textBoxPath, "textBoxPath");
            this.textBoxPath.Name = "textBoxPath";
            this.textBoxPath.ReadOnly = true;
            // 
            // buttonFileDlg
            // 
            resources.ApplyResources(this.buttonFileDlg, "buttonFileDlg");
            this.buttonFileDlg.Name = "buttonFileDlg";
            this.buttonFileDlg.UseVisualStyleBackColor = true;
            this.buttonFileDlg.Click += new System.EventHandler(this.buttonFileDlg_Click);
            // 
            // textBoxName
            // 
            resources.ApplyResources(this.textBoxName, "textBoxName");
            this.textBoxName.Name = "textBoxName";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // comboBoxSection
            // 
            this.comboBoxSection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSection.FormattingEnabled = true;
            this.comboBoxSection.Items.AddRange(new object[] {
            resources.GetString("comboBoxSection.Items"),
            resources.GetString("comboBoxSection.Items1"),
            resources.GetString("comboBoxSection.Items2"),
            resources.GetString("comboBoxSection.Items3"),
            resources.GetString("comboBoxSection.Items4"),
            resources.GetString("comboBoxSection.Items5"),
            resources.GetString("comboBoxSection.Items6"),
            resources.GetString("comboBoxSection.Items7"),
            resources.GetString("comboBoxSection.Items8"),
            resources.GetString("comboBoxSection.Items9")});
            resources.ApplyResources(this.comboBoxSection, "comboBoxSection");
            this.comboBoxSection.Name = "comboBoxSection";
            // 
            // NewRunItem
            // 
            this.AcceptButton = this.buttonOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.comboBoxSection);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewRunItem";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonFileDlg;
        private System.Windows.Forms.TextBox textBoxPath;
        private System.Windows.Forms.TextBox textBoxArgs;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxSection;
    }
}