namespace Little_Registry_Cleaner.Optimizer
{
    partial class Optimizer
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
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelWarning = new System.Windows.Forms.Label();
            this.progressBarDefrag = new System.Windows.Forms.ProgressBar();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.labelAction = new System.Windows.Forms.Label();
            this.progressBarAnalyzed = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            this.buttonStart.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonStart.Enabled = false;
            this.buttonStart.Location = new System.Drawing.Point(167, 357);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(109, 23);
            this.buttonStart.TabIndex = 1;
            this.buttonStart.Text = "Start Optimization >";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(282, 357);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(50, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // labelWarning
            // 
            this.labelWarning.AutoSize = true;
            this.labelWarning.Location = new System.Drawing.Point(3, 328);
            this.labelWarning.Name = "labelWarning";
            this.labelWarning.Size = new System.Drawing.Size(323, 26);
            this.labelWarning.TabIndex = 10;
            this.labelWarning.Text = "WARNING: You must close all running programs before starting the\r\ndefragmentation" +
                " process";
            // 
            // progressBarDefrag
            // 
            this.progressBarDefrag.Location = new System.Drawing.Point(6, 58);
            this.progressBarDefrag.Name = "progressBarDefrag";
            this.progressBarDefrag.Size = new System.Drawing.Size(326, 23);
            this.progressBarDefrag.TabIndex = 9;
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listView1.Location = new System.Drawing.Point(6, 87);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(326, 238);
            this.listView1.TabIndex = 8;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Registry Hive";
            this.columnHeader1.Width = 132;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Current Size";
            this.columnHeader2.Width = 79;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Compacted Size";
            this.columnHeader3.Width = 95;
            // 
            // labelAction
            // 
            this.labelAction.AutoSize = true;
            this.labelAction.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAction.Location = new System.Drawing.Point(3, 9);
            this.labelAction.Name = "labelAction";
            this.labelAction.Size = new System.Drawing.Size(158, 16);
            this.labelAction.TabIndex = 7;
            this.labelAction.Text = "Analyzing the registry:";
            // 
            // progressBarAnalyzed
            // 
            this.progressBarAnalyzed.Location = new System.Drawing.Point(6, 29);
            this.progressBarAnalyzed.Name = "progressBarAnalyzed";
            this.progressBarAnalyzed.Size = new System.Drawing.Size(326, 23);
            this.progressBarAnalyzed.TabIndex = 6;
            // 
            // Optimizer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(344, 386);
            this.Controls.Add(this.labelWarning);
            this.Controls.Add(this.progressBarDefrag);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.labelAction);
            this.Controls.Add(this.progressBarAnalyzed);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonStart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Optimizer";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Little Registry Optimizer";
            this.Shown += new System.EventHandler(this.Optimizer1_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Optimizer1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelWarning;
        private System.Windows.Forms.ProgressBar progressBarDefrag;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Label labelAction;
        private System.Windows.Forms.ProgressBar progressBarAnalyzed;
    }
}