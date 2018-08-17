namespace BZRModManager
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpBZ98R = new System.Windows.Forms.TabPage();
            this.lbModsBZ98R = new System.Windows.Forms.ListBox();
            this.btnDownloadBZ98R = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtDownloadBZ98R = new System.Windows.Forms.TextBox();
            this.tpBZCC = new System.Windows.Forms.TabPage();
            this.lbModsBZCC = new System.Windows.Forms.ListBox();
            this.btnDownloadBZCC = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDownloadBZCC = new System.Windows.Forms.TextBox();
            this.tpSettings = new System.Windows.Forms.TabPage();
            this.tpLog = new System.Windows.Forms.TabPage();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.tpLogSteamCMD = new System.Windows.Forms.TabPage();
            this.txtLogSteamCMD = new System.Windows.Forms.RichTextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslSteamCMD = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslSteamCMDCommand = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tmrModUpdate = new System.Windows.Forms.Timer(this.components);
            this.tpLogSteamCMDFull = new System.Windows.Forms.TabPage();
            this.txtLogSteamCMDFull = new System.Windows.Forms.RichTextBox();
            this.tabControl1.SuspendLayout();
            this.tpBZ98R.SuspendLayout();
            this.tpBZCC.SuspendLayout();
            this.tpLog.SuspendLayout();
            this.tpLogSteamCMD.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tpLogSteamCMDFull.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tpBZ98R);
            this.tabControl1.Controls.Add(this.tpBZCC);
            this.tabControl1.Controls.Add(this.tpSettings);
            this.tabControl1.Controls.Add(this.tpLog);
            this.tabControl1.Controls.Add(this.tpLogSteamCMD);
            this.tabControl1.Controls.Add(this.tpLogSteamCMDFull);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(594, 346);
            this.tabControl1.TabIndex = 0;
            // 
            // tpBZ98R
            // 
            this.tpBZ98R.Controls.Add(this.lbModsBZ98R);
            this.tpBZ98R.Controls.Add(this.btnDownloadBZ98R);
            this.tpBZ98R.Controls.Add(this.label1);
            this.tpBZ98R.Controls.Add(this.txtDownloadBZ98R);
            this.tpBZ98R.Location = new System.Drawing.Point(4, 22);
            this.tpBZ98R.Name = "tpBZ98R";
            this.tpBZ98R.Padding = new System.Windows.Forms.Padding(3);
            this.tpBZ98R.Size = new System.Drawing.Size(586, 320);
            this.tpBZ98R.TabIndex = 0;
            this.tpBZ98R.Text = "BZ98R";
            this.tpBZ98R.UseVisualStyleBackColor = true;
            // 
            // lbModsBZ98R
            // 
            this.lbModsBZ98R.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbModsBZ98R.FormattingEnabled = true;
            this.lbModsBZ98R.Location = new System.Drawing.Point(9, 35);
            this.lbModsBZ98R.Name = "lbModsBZ98R";
            this.lbModsBZ98R.Size = new System.Drawing.Size(571, 277);
            this.lbModsBZ98R.TabIndex = 3;
            // 
            // btnDownloadBZ98R
            // 
            this.btnDownloadBZ98R.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDownloadBZ98R.Location = new System.Drawing.Point(505, 6);
            this.btnDownloadBZ98R.Name = "btnDownloadBZ98R";
            this.btnDownloadBZ98R.Size = new System.Drawing.Size(75, 23);
            this.btnDownloadBZ98R.TabIndex = 2;
            this.btnDownloadBZ98R.Text = "Download";
            this.btnDownloadBZ98R.UseVisualStyleBackColor = true;
            this.btnDownloadBZ98R.Click += new System.EventHandler(this.btnDownloadBZ98R_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Workshop Item URL:";
            // 
            // txtDownloadBZ98R
            // 
            this.txtDownloadBZ98R.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDownloadBZ98R.Location = new System.Drawing.Point(119, 9);
            this.txtDownloadBZ98R.Name = "txtDownloadBZ98R";
            this.txtDownloadBZ98R.Size = new System.Drawing.Size(380, 20);
            this.txtDownloadBZ98R.TabIndex = 0;
            // 
            // tpBZCC
            // 
            this.tpBZCC.Controls.Add(this.lbModsBZCC);
            this.tpBZCC.Controls.Add(this.btnDownloadBZCC);
            this.tpBZCC.Controls.Add(this.label2);
            this.tpBZCC.Controls.Add(this.txtDownloadBZCC);
            this.tpBZCC.Location = new System.Drawing.Point(4, 22);
            this.tpBZCC.Name = "tpBZCC";
            this.tpBZCC.Padding = new System.Windows.Forms.Padding(3);
            this.tpBZCC.Size = new System.Drawing.Size(586, 320);
            this.tpBZCC.TabIndex = 1;
            this.tpBZCC.Text = "BZCC";
            this.tpBZCC.UseVisualStyleBackColor = true;
            // 
            // lbModsBZCC
            // 
            this.lbModsBZCC.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbModsBZCC.FormattingEnabled = true;
            this.lbModsBZCC.Location = new System.Drawing.Point(9, 35);
            this.lbModsBZCC.Name = "lbModsBZCC";
            this.lbModsBZCC.Size = new System.Drawing.Size(571, 277);
            this.lbModsBZCC.TabIndex = 6;
            // 
            // btnDownloadBZCC
            // 
            this.btnDownloadBZCC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDownloadBZCC.Location = new System.Drawing.Point(505, 6);
            this.btnDownloadBZCC.Name = "btnDownloadBZCC";
            this.btnDownloadBZCC.Size = new System.Drawing.Size(75, 23);
            this.btnDownloadBZCC.TabIndex = 5;
            this.btnDownloadBZCC.Text = "Download";
            this.btnDownloadBZCC.UseVisualStyleBackColor = true;
            this.btnDownloadBZCC.Click += new System.EventHandler(this.btnDownloadBZCC_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Workshop Item URL:";
            // 
            // txtDownloadBZCC
            // 
            this.txtDownloadBZCC.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDownloadBZCC.Location = new System.Drawing.Point(119, 9);
            this.txtDownloadBZCC.Name = "txtDownloadBZCC";
            this.txtDownloadBZCC.Size = new System.Drawing.Size(380, 20);
            this.txtDownloadBZCC.TabIndex = 3;
            // 
            // tpSettings
            // 
            this.tpSettings.Location = new System.Drawing.Point(4, 22);
            this.tpSettings.Name = "tpSettings";
            this.tpSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tpSettings.Size = new System.Drawing.Size(586, 319);
            this.tpSettings.TabIndex = 2;
            this.tpSettings.Text = "Settings";
            this.tpSettings.UseVisualStyleBackColor = true;
            // 
            // tpLog
            // 
            this.tpLog.Controls.Add(this.txtLog);
            this.tpLog.Location = new System.Drawing.Point(4, 22);
            this.tpLog.Name = "tpLog";
            this.tpLog.Padding = new System.Windows.Forms.Padding(3);
            this.tpLog.Size = new System.Drawing.Size(586, 319);
            this.tpLog.TabIndex = 3;
            this.tpLog.Text = "Log";
            this.tpLog.UseVisualStyleBackColor = true;
            // 
            // txtLog
            // 
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Location = new System.Drawing.Point(3, 3);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(580, 313);
            this.txtLog.TabIndex = 0;
            // 
            // tpLogSteamCMD
            // 
            this.tpLogSteamCMD.Controls.Add(this.txtLogSteamCMD);
            this.tpLogSteamCMD.Location = new System.Drawing.Point(4, 22);
            this.tpLogSteamCMD.Name = "tpLogSteamCMD";
            this.tpLogSteamCMD.Padding = new System.Windows.Forms.Padding(3);
            this.tpLogSteamCMD.Size = new System.Drawing.Size(586, 319);
            this.tpLogSteamCMD.TabIndex = 4;
            this.tpLogSteamCMD.Text = "SteamCMD";
            this.tpLogSteamCMD.UseVisualStyleBackColor = true;
            // 
            // txtLogSteamCMD
            // 
            this.txtLogSteamCMD.BackColor = System.Drawing.Color.Black;
            this.txtLogSteamCMD.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLogSteamCMD.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLogSteamCMD.ForeColor = System.Drawing.Color.White;
            this.txtLogSteamCMD.Location = new System.Drawing.Point(3, 3);
            this.txtLogSteamCMD.Name = "txtLogSteamCMD";
            this.txtLogSteamCMD.ReadOnly = true;
            this.txtLogSteamCMD.Size = new System.Drawing.Size(580, 313);
            this.txtLogSteamCMD.TabIndex = 1;
            this.txtLogSteamCMD.Text = "";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.tsslSteamCMD,
            this.tsslSteamCMDCommand,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 361);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(618, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.AutoSize = false;
            this.toolStripStatusLabel1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(67, 17);
            this.toolStripStatusLabel1.Text = "SteamCMD";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tsslSteamCMD
            // 
            this.tsslSteamCMD.Name = "tsslSteamCMD";
            this.tsslSteamCMD.Size = new System.Drawing.Size(24, 17);
            this.tsslSteamCMD.Text = "Off";
            this.tsslSteamCMD.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tsslSteamCMDCommand
            // 
            this.tsslSteamCMDCommand.Enabled = false;
            this.tsslSteamCMDCommand.Name = "tsslSteamCMDCommand";
            this.tsslSteamCMDCommand.Size = new System.Drawing.Size(34, 17);
            this.tsslSteamCMDCommand.Text = "none";
            this.tsslSteamCMDCommand.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(478, 17);
            this.toolStripStatusLabel2.Spring = true;
            this.toolStripStatusLabel2.Text = "-";
            // 
            // tmrModUpdate
            // 
            this.tmrModUpdate.Enabled = true;
            this.tmrModUpdate.Interval = 300000;
            this.tmrModUpdate.Tick += new System.EventHandler(this.tmrModUpdate_Tick);
            // 
            // tpLogSteamCMDFull
            // 
            this.tpLogSteamCMDFull.Controls.Add(this.txtLogSteamCMDFull);
            this.tpLogSteamCMDFull.Location = new System.Drawing.Point(4, 22);
            this.tpLogSteamCMDFull.Name = "tpLogSteamCMDFull";
            this.tpLogSteamCMDFull.Padding = new System.Windows.Forms.Padding(3);
            this.tpLogSteamCMDFull.Size = new System.Drawing.Size(586, 319);
            this.tpLogSteamCMDFull.TabIndex = 5;
            this.tpLogSteamCMDFull.Text = "SteamCMD Raw";
            this.tpLogSteamCMDFull.UseVisualStyleBackColor = true;
            // 
            // txtLogSteamCMDFull
            // 
            this.txtLogSteamCMDFull.BackColor = System.Drawing.Color.Black;
            this.txtLogSteamCMDFull.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLogSteamCMDFull.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLogSteamCMDFull.ForeColor = System.Drawing.Color.White;
            this.txtLogSteamCMDFull.Location = new System.Drawing.Point(3, 3);
            this.txtLogSteamCMDFull.Name = "txtLogSteamCMDFull";
            this.txtLogSteamCMDFull.ReadOnly = true;
            this.txtLogSteamCMDFull.Size = new System.Drawing.Size(580, 313);
            this.txtLogSteamCMDFull.TabIndex = 2;
            this.txtLogSteamCMDFull.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(618, 383);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Battlezone Redux Mod Manager";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tpBZ98R.ResumeLayout(false);
            this.tpBZ98R.PerformLayout();
            this.tpBZCC.ResumeLayout(false);
            this.tpBZCC.PerformLayout();
            this.tpLog.ResumeLayout(false);
            this.tpLog.PerformLayout();
            this.tpLogSteamCMD.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tpLogSteamCMDFull.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tpBZ98R;
        private System.Windows.Forms.TabPage tpBZCC;
        private System.Windows.Forms.TabPage tpSettings;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Button btnDownloadBZ98R;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtDownloadBZ98R;
        private System.Windows.Forms.Button btnDownloadBZCC;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDownloadBZCC;
        private System.Windows.Forms.ListBox lbModsBZ98R;
        private System.Windows.Forms.ListBox lbModsBZCC;
        private System.Windows.Forms.Timer tmrModUpdate;
        private System.Windows.Forms.ToolStripStatusLabel tsslSteamCMD;
        private System.Windows.Forms.ToolStripStatusLabel tsslSteamCMDCommand;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.TabPage tpLog;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.TabPage tpLogSteamCMD;
        private System.Windows.Forms.RichTextBox txtLogSteamCMD;
        private System.Windows.Forms.TabPage tpLogSteamCMDFull;
        private System.Windows.Forms.RichTextBox txtLogSteamCMDFull;
    }
}

