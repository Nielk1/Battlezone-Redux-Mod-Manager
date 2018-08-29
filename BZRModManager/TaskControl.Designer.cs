namespace BZRModManager
{
    partial class TaskControl
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
            this.lblText = new System.Windows.Forms.Label();
            this.pbProg = new System.Windows.Forms.ProgressBar();
            this.pnlTasks = new System.Windows.Forms.TableLayoutPanel();
            this.SuspendLayout();
            // 
            // lblText
            // 
            this.lblText.AutoSize = true;
            this.lblText.Location = new System.Drawing.Point(0, 0);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(35, 13);
            this.lblText.TabIndex = 0;
            this.lblText.Text = "label1";
            // 
            // pbProg
            // 
            this.pbProg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbProg.Location = new System.Drawing.Point(3, 16);
            this.pbProg.Name = "pbProg";
            this.pbProg.Size = new System.Drawing.Size(4994, 23);
            this.pbProg.TabIndex = 1;
            // 
            // pnlTasks
            // 
            this.pnlTasks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlTasks.ColumnCount = 1;
            this.pnlTasks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlTasks.Location = new System.Drawing.Point(0, 45);
            this.pnlTasks.Margin = new System.Windows.Forms.Padding(0);
            this.pnlTasks.Name = "pnlTasks";
            this.pnlTasks.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.pnlTasks.RowCount = 1;
            this.pnlTasks.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlTasks.Size = new System.Drawing.Size(5000, 0);
            this.pnlTasks.TabIndex = 2;
            // 
            // TaskControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.lblText);
            this.Controls.Add(this.pbProg);
            this.Controls.Add(this.pnlTasks);
            this.Name = "TaskControl";
            this.Size = new System.Drawing.Size(5000, 45);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.ProgressBar pbProg;
        private System.Windows.Forms.TableLayoutPanel pnlTasks;
    }
}
