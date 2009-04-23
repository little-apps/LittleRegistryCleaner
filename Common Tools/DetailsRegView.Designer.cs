namespace Common_Tools
{
    partial class DetailsRegView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelDetails = new System.Windows.Forms.Label();
            this.labelData = new System.Windows.Forms.Label();
            this.labelProblem = new System.Windows.Forms.Label();
            this.labelValueName = new System.Windows.Forms.Label();
            this.labelHKEY = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.labelDetails, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelData, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelProblem, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelValueName, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelHKEY, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 11F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(330, 117);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // labelDetails
            // 
            this.labelDetails.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelDetails.Location = new System.Drawing.Point(3, 0);
            this.labelDetails.Name = "labelDetails";
            this.labelDetails.Size = new System.Drawing.Size(324, 20);
            this.labelDetails.TabIndex = 0;
            this.labelDetails.Text = "Details";
            this.labelDetails.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelData
            // 
            this.labelData.AutoSize = true;
            this.labelData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelData.Location = new System.Drawing.Point(3, 86);
            this.labelData.Name = "labelData";
            this.labelData.Size = new System.Drawing.Size(324, 31);
            this.labelData.TabIndex = 5;
            this.labelData.Text = "Data:";
            // 
            // labelProblem
            // 
            this.labelProblem.AutoSize = true;
            this.labelProblem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelProblem.Location = new System.Drawing.Point(3, 20);
            this.labelProblem.Name = "labelProblem";
            this.labelProblem.Size = new System.Drawing.Size(324, 22);
            this.labelProblem.TabIndex = 6;
            this.labelProblem.Text = "Problem:";
            // 
            // labelValueName
            // 
            this.labelValueName.AutoSize = true;
            this.labelValueName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelValueName.Location = new System.Drawing.Point(3, 66);
            this.labelValueName.Name = "labelValueName";
            this.labelValueName.Size = new System.Drawing.Size(324, 20);
            this.labelValueName.TabIndex = 7;
            this.labelValueName.Text = "Value Name:";
            // 
            // labelHKEY
            // 
            this.labelHKEY.AutoSize = true;
            this.labelHKEY.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelHKEY.Location = new System.Drawing.Point(3, 42);
            this.labelHKEY.Name = "labelHKEY";
            this.labelHKEY.Size = new System.Drawing.Size(324, 24);
            this.labelHKEY.TabIndex = 8;
            this.labelHKEY.Text = "Location:";
            // 
            // DetailsRegView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "DetailsRegView";
            this.Size = new System.Drawing.Size(330, 117);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelDetails;
        private System.Windows.Forms.Label labelData;
        private System.Windows.Forms.Label labelProblem;
        private System.Windows.Forms.Label labelValueName;
        private System.Windows.Forms.Label labelHKEY;



    }
}
