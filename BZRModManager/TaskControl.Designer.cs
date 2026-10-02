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
            lblText = new System.Windows.Forms.Label();
            pbProg = new System.Windows.Forms.ProgressBar();
            pnlTasks = new System.Windows.Forms.TableLayoutPanel();
            panel1 = new System.Windows.Forms.Panel();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // lblText
            // 
            lblText.AutoSize = true;
            lblText.Location = new System.Drawing.Point(0, 0);
            lblText.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblText.Name = "lblText";
            lblText.Size = new System.Drawing.Size(38, 15);
            lblText.TabIndex = 0;
            lblText.Text = "label1";
            // 
            // pbProg
            // 
            pbProg.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            pbProg.Location = new System.Drawing.Point(0, 0);
            pbProg.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pbProg.Name = "pbProg";
            pbProg.Size = new System.Drawing.Size(5824, 25);
            pbProg.TabIndex = 1;
            // 
            // pnlTasks
            // 
            pnlTasks.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            pnlTasks.ColumnCount = 1;
            pnlTasks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            pnlTasks.Location = new System.Drawing.Point(0, 52);
            pnlTasks.Margin = new System.Windows.Forms.Padding(0);
            pnlTasks.Name = "pnlTasks";
            pnlTasks.Padding = new System.Windows.Forms.Padding(12, 0, 0, 0);
            pnlTasks.RowCount = 1;
            pnlTasks.RowStyles.Add(new System.Windows.Forms.RowStyle());
            pnlTasks.Size = new System.Drawing.Size(5833, 0);
            pnlTasks.TabIndex = 2;
            // 
            // panel1
            // 
            panel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panel1.Controls.Add(pbProg);
            panel1.Location = new System.Drawing.Point(4, 18);
            panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(5826, 27);
            panel1.TabIndex = 1;
            // 
            // TaskControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            Controls.Add(lblText);
            Controls.Add(panel1);
            Controls.Add(pnlTasks);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "TaskControl";
            Size = new System.Drawing.Size(5833, 52);
            panel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.ProgressBar pbProg;
        private System.Windows.Forms.TableLayoutPanel pnlTasks;
        private System.Windows.Forms.Panel panel1;
    }
}
