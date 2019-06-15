namespace BZRModManager
{
    partial class MainForm
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
            this.btnUpdateBZ98R = new System.Windows.Forms.Button();
            this.btnRefreshBZ98R = new System.Windows.Forms.Button();
            this.lvModsBZ98R = new BZRModManager.LinqListView();
            this.btnDownloadBZ98R = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtDownloadBZ98R = new System.Windows.Forms.TextBox();
            this.tpBZCC = new System.Windows.Forms.TabPage();
            this.btnDependenciesBZ98R = new System.Windows.Forms.Button();
            this.btnUpdateBZCC = new System.Windows.Forms.Button();
            this.btnRefreshBZCC = new System.Windows.Forms.Button();
            this.lvModsBZCC = new BZRModManager.LinqListView();
            this.btnDownloadBZCC = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDownloadBZCC = new System.Windows.Forms.TextBox();
            this.tpSettings = new System.Windows.Forms.TabPage();
            this.cbFallbackSteamCmdWindowHandling = new System.Windows.Forms.CheckBox();
            this.btnGOGBZCCASMAbout = new System.Windows.Forms.Button();
            this.btnGOGBZCCASM = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txtBZCCMyDocs = new System.Windows.Forms.TextBox();
            this.btnBZCCMyDocsApply = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtBZ98RGog = new System.Windows.Forms.TextBox();
            this.txtBZ98RGogApply = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtBZCCSteam = new System.Windows.Forms.TextBox();
            this.btnBZCCSteamApply = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtBZ98RSteam = new System.Windows.Forms.TextBox();
            this.btnBZ98RSteamApply = new System.Windows.Forms.Button();
            this.tpTasks = new System.Windows.Forms.TabPage();
            this.pnlTasks = new System.Windows.Forms.TableLayoutPanel();
            this.tpLog = new System.Windows.Forms.TabPage();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.tpLogSteamCmd = new System.Windows.Forms.TabPage();
            this.txtLogSteamCmd = new System.Windows.Forms.RichTextBox();
            this.tpLogSteamCmdFull = new System.Windows.Forms.TabPage();
            this.txtLogSteamCmdFull = new System.Windows.Forms.RichTextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslSteamCmd = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslSteamCmdCommand = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.ofdGOGBZCCASM = new System.Windows.Forms.OpenFileDialog();
            this.tabControl1.SuspendLayout();
            this.tpBZ98R.SuspendLayout();
            this.tpBZCC.SuspendLayout();
            this.tpSettings.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tpTasks.SuspendLayout();
            this.tpLog.SuspendLayout();
            this.tpLogSteamCmd.SuspendLayout();
            this.tpLogSteamCmdFull.SuspendLayout();
            this.statusStrip1.SuspendLayout();
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
            this.tabControl1.Controls.Add(this.tpTasks);
            this.tabControl1.Controls.Add(this.tpLog);
            this.tabControl1.Controls.Add(this.tpLogSteamCmd);
            this.tabControl1.Controls.Add(this.tpLogSteamCmdFull);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(742, 346);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tpBZ98R
            // 
            this.tpBZ98R.Controls.Add(this.btnUpdateBZ98R);
            this.tpBZ98R.Controls.Add(this.btnRefreshBZ98R);
            this.tpBZ98R.Controls.Add(this.lvModsBZ98R);
            this.tpBZ98R.Controls.Add(this.btnDownloadBZ98R);
            this.tpBZ98R.Controls.Add(this.label1);
            this.tpBZ98R.Controls.Add(this.txtDownloadBZ98R);
            this.tpBZ98R.Location = new System.Drawing.Point(4, 22);
            this.tpBZ98R.Name = "tpBZ98R";
            this.tpBZ98R.Padding = new System.Windows.Forms.Padding(3);
            this.tpBZ98R.Size = new System.Drawing.Size(734, 320);
            this.tpBZ98R.TabIndex = 0;
            this.tpBZ98R.Text = "BZ98R";
            this.tpBZ98R.UseVisualStyleBackColor = true;
            // 
            // btnUpdateBZ98R
            // 
            this.btnUpdateBZ98R.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdateBZ98R.Location = new System.Drawing.Point(676, 7);
            this.btnUpdateBZ98R.Name = "btnUpdateBZ98R";
            this.btnUpdateBZ98R.Size = new System.Drawing.Size(23, 23);
            this.btnUpdateBZ98R.TabIndex = 7;
            this.btnUpdateBZ98R.Text = "U";
            this.toolTip1.SetToolTip(this.btnUpdateBZ98R, "Update Mods");
            this.btnUpdateBZ98R.UseVisualStyleBackColor = true;
            this.btnUpdateBZ98R.Click += new System.EventHandler(this.btnUpdateBZ98R_Click);
            // 
            // btnRefreshBZ98R
            // 
            this.btnRefreshBZ98R.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshBZ98R.Location = new System.Drawing.Point(705, 7);
            this.btnRefreshBZ98R.Name = "btnRefreshBZ98R";
            this.btnRefreshBZ98R.Size = new System.Drawing.Size(23, 23);
            this.btnRefreshBZ98R.TabIndex = 6;
            this.btnRefreshBZ98R.Text = "R";
            this.toolTip1.SetToolTip(this.btnRefreshBZ98R, "Refresh List");
            this.btnRefreshBZ98R.UseVisualStyleBackColor = true;
            this.btnRefreshBZ98R.Click += new System.EventHandler(this.btnRefreshBZ98R_Click);
            // 
            // lvModsBZ98R
            // 
            this.lvModsBZ98R.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvModsBZ98R.FullRowSelect = true;
            this.lvModsBZ98R.GridLines = true;
            this.lvModsBZ98R.Location = new System.Drawing.Point(6, 35);
            this.lvModsBZ98R.MultiSelect = false;
            this.lvModsBZ98R.Name = "lvModsBZ98R";
            this.lvModsBZ98R.Size = new System.Drawing.Size(722, 279);
            this.lvModsBZ98R.TabIndex = 5;
            this.lvModsBZ98R.UseCompatibleStateImageBehavior = false;
            this.lvModsBZ98R.View = System.Windows.Forms.View.Details;
            this.lvModsBZ98R.VirtualMode = true;
            // 
            // btnDownloadBZ98R
            // 
            this.btnDownloadBZ98R.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDownloadBZ98R.Location = new System.Drawing.Point(595, 7);
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
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Mod URL:";
            // 
            // txtDownloadBZ98R
            // 
            this.txtDownloadBZ98R.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDownloadBZ98R.Location = new System.Drawing.Point(68, 9);
            this.txtDownloadBZ98R.Name = "txtDownloadBZ98R";
            this.txtDownloadBZ98R.Size = new System.Drawing.Size(521, 20);
            this.txtDownloadBZ98R.TabIndex = 0;
            // 
            // tpBZCC
            // 
            this.tpBZCC.Controls.Add(this.btnDependenciesBZ98R);
            this.tpBZCC.Controls.Add(this.btnUpdateBZCC);
            this.tpBZCC.Controls.Add(this.btnRefreshBZCC);
            this.tpBZCC.Controls.Add(this.lvModsBZCC);
            this.tpBZCC.Controls.Add(this.btnDownloadBZCC);
            this.tpBZCC.Controls.Add(this.label2);
            this.tpBZCC.Controls.Add(this.txtDownloadBZCC);
            this.tpBZCC.Location = new System.Drawing.Point(4, 22);
            this.tpBZCC.Name = "tpBZCC";
            this.tpBZCC.Padding = new System.Windows.Forms.Padding(3);
            this.tpBZCC.Size = new System.Drawing.Size(734, 320);
            this.tpBZCC.TabIndex = 1;
            this.tpBZCC.Text = "BZCC";
            this.tpBZCC.UseVisualStyleBackColor = true;
            // 
            // btnDependenciesBZ98R
            // 
            this.btnDependenciesBZ98R.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDependenciesBZ98R.Location = new System.Drawing.Point(647, 7);
            this.btnDependenciesBZ98R.Name = "btnDependenciesBZ98R";
            this.btnDependenciesBZ98R.Size = new System.Drawing.Size(23, 23);
            this.btnDependenciesBZ98R.TabIndex = 10;
            this.btnDependenciesBZ98R.Text = "D";
            this.toolTip1.SetToolTip(this.btnDependenciesBZ98R, "Download Dependencies");
            this.btnDependenciesBZ98R.UseVisualStyleBackColor = true;
            this.btnDependenciesBZ98R.Click += new System.EventHandler(this.btnDependenciesBZ98R_Click);
            // 
            // btnUpdateBZCC
            // 
            this.btnUpdateBZCC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdateBZCC.Location = new System.Drawing.Point(676, 7);
            this.btnUpdateBZCC.Name = "btnUpdateBZCC";
            this.btnUpdateBZCC.Size = new System.Drawing.Size(23, 23);
            this.btnUpdateBZCC.TabIndex = 9;
            this.btnUpdateBZCC.Text = "U";
            this.toolTip1.SetToolTip(this.btnUpdateBZCC, "Update Mods");
            this.btnUpdateBZCC.UseVisualStyleBackColor = true;
            this.btnUpdateBZCC.Click += new System.EventHandler(this.btnUpdateBZCC_Click);
            // 
            // btnRefreshBZCC
            // 
            this.btnRefreshBZCC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshBZCC.Location = new System.Drawing.Point(705, 7);
            this.btnRefreshBZCC.Name = "btnRefreshBZCC";
            this.btnRefreshBZCC.Size = new System.Drawing.Size(23, 23);
            this.btnRefreshBZCC.TabIndex = 8;
            this.btnRefreshBZCC.Text = "R";
            this.toolTip1.SetToolTip(this.btnRefreshBZCC, "Refresh List");
            this.btnRefreshBZCC.UseVisualStyleBackColor = true;
            this.btnRefreshBZCC.Click += new System.EventHandler(this.btnRefreshBZCC_Click);
            // 
            // lvModsBZCC
            // 
            this.lvModsBZCC.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvModsBZCC.FullRowSelect = true;
            this.lvModsBZCC.GridLines = true;
            this.lvModsBZCC.Location = new System.Drawing.Point(6, 35);
            this.lvModsBZCC.MultiSelect = false;
            this.lvModsBZCC.Name = "lvModsBZCC";
            this.lvModsBZCC.Size = new System.Drawing.Size(722, 279);
            this.lvModsBZCC.TabIndex = 6;
            this.lvModsBZCC.UseCompatibleStateImageBehavior = false;
            this.lvModsBZCC.View = System.Windows.Forms.View.Details;
            this.lvModsBZCC.VirtualMode = true;
            // 
            // btnDownloadBZCC
            // 
            this.btnDownloadBZCC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDownloadBZCC.Location = new System.Drawing.Point(566, 7);
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
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Mod URL:";
            // 
            // txtDownloadBZCC
            // 
            this.txtDownloadBZCC.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDownloadBZCC.Location = new System.Drawing.Point(68, 9);
            this.txtDownloadBZCC.Name = "txtDownloadBZCC";
            this.txtDownloadBZCC.Size = new System.Drawing.Size(492, 20);
            this.txtDownloadBZCC.TabIndex = 3;
            // 
            // tpSettings
            // 
            this.tpSettings.Controls.Add(this.cbFallbackSteamCmdWindowHandling);
            this.tpSettings.Controls.Add(this.btnGOGBZCCASMAbout);
            this.tpSettings.Controls.Add(this.btnGOGBZCCASM);
            this.tpSettings.Controls.Add(this.groupBox4);
            this.tpSettings.Controls.Add(this.groupBox3);
            this.tpSettings.Controls.Add(this.groupBox2);
            this.tpSettings.Controls.Add(this.groupBox1);
            this.tpSettings.Location = new System.Drawing.Point(4, 22);
            this.tpSettings.Name = "tpSettings";
            this.tpSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tpSettings.Size = new System.Drawing.Size(734, 320);
            this.tpSettings.TabIndex = 2;
            this.tpSettings.Text = "Settings";
            this.tpSettings.UseVisualStyleBackColor = true;
            // 
            // cbFallbackSteamCmdWindowHandling
            // 
            this.cbFallbackSteamCmdWindowHandling.AutoSize = true;
            this.cbFallbackSteamCmdWindowHandling.Location = new System.Drawing.Point(6, 259);
            this.cbFallbackSteamCmdWindowHandling.Name = "cbFallbackSteamCmdWindowHandling";
            this.cbFallbackSteamCmdWindowHandling.Size = new System.Drawing.Size(207, 17);
            this.cbFallbackSteamCmdWindowHandling.TabIndex = 18;
            this.cbFallbackSteamCmdWindowHandling.Text = "Fallback SteamCmd Window Handling";
            this.cbFallbackSteamCmdWindowHandling.UseVisualStyleBackColor = true;
            this.cbFallbackSteamCmdWindowHandling.Visible = false;
            this.cbFallbackSteamCmdWindowHandling.CheckedChanged += new System.EventHandler(this.cbFallbackSteamCmdWindowHandling_CheckedChanged);
            // 
            // btnGOGBZCCASMAbout
            // 
            this.btnGOGBZCCASMAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGOGBZCCASMAbout.Location = new System.Drawing.Point(663, 230);
            this.btnGOGBZCCASMAbout.Name = "btnGOGBZCCASMAbout";
            this.btnGOGBZCCASMAbout.Size = new System.Drawing.Size(65, 23);
            this.btnGOGBZCCASMAbout.TabIndex = 17;
            this.btnGOGBZCCASMAbout.Text = "About";
            this.btnGOGBZCCASMAbout.UseVisualStyleBackColor = true;
            this.btnGOGBZCCASMAbout.Click += new System.EventHandler(this.btnGOGBZCCASMAbout_Click);
            // 
            // btnGOGBZCCASM
            // 
            this.btnGOGBZCCASM.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGOGBZCCASM.Location = new System.Drawing.Point(6, 230);
            this.btnGOGBZCCASM.Name = "btnGOGBZCCASM";
            this.btnGOGBZCCASM.Size = new System.Drawing.Size(651, 23);
            this.btnGOGBZCCASM.TabIndex = 16;
            this.btnGOGBZCCASM.Text = "Apply GOG BZCC ASM Patch";
            this.btnGOGBZCCASM.UseVisualStyleBackColor = true;
            this.btnGOGBZCCASM.Click += new System.EventHandler(this.btnGOGBZCCASM_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.txtBZCCMyDocs);
            this.groupBox4.Controls.Add(this.btnBZCCMyDocsApply);
            this.groupBox4.Location = new System.Drawing.Point(6, 174);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(723, 50);
            this.groupBox4.TabIndex = 15;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "BZCC Folder in My Docs/My Games";
            // 
            // txtBZCCMyDocs
            // 
            this.txtBZCCMyDocs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBZCCMyDocs.Location = new System.Drawing.Point(6, 21);
            this.txtBZCCMyDocs.Name = "txtBZCCMyDocs";
            this.txtBZCCMyDocs.Size = new System.Drawing.Size(649, 20);
            this.txtBZCCMyDocs.TabIndex = 6;
            // 
            // btnBZCCMyDocsApply
            // 
            this.btnBZCCMyDocsApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBZCCMyDocsApply.Location = new System.Drawing.Point(661, 19);
            this.btnBZCCMyDocsApply.Name = "btnBZCCMyDocsApply";
            this.btnBZCCMyDocsApply.Size = new System.Drawing.Size(57, 23);
            this.btnBZCCMyDocsApply.TabIndex = 8;
            this.btnBZCCMyDocsApply.Text = "Apply";
            this.btnBZCCMyDocsApply.UseVisualStyleBackColor = true;
            this.btnBZCCMyDocsApply.Click += new System.EventHandler(this.btnBZCCMyDocsApply_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.txtBZ98RGog);
            this.groupBox3.Controls.Add(this.txtBZ98RGogApply);
            this.groupBox3.Location = new System.Drawing.Point(6, 118);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(723, 50);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "GOG install of BZ98R";
            // 
            // txtBZ98RGog
            // 
            this.txtBZ98RGog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBZ98RGog.Location = new System.Drawing.Point(6, 21);
            this.txtBZ98RGog.Name = "txtBZ98RGog";
            this.txtBZ98RGog.Size = new System.Drawing.Size(649, 20);
            this.txtBZ98RGog.TabIndex = 6;
            // 
            // txtBZ98RGogApply
            // 
            this.txtBZ98RGogApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBZ98RGogApply.Location = new System.Drawing.Point(661, 19);
            this.txtBZ98RGogApply.Name = "txtBZ98RGogApply";
            this.txtBZ98RGogApply.Size = new System.Drawing.Size(57, 23);
            this.txtBZ98RGogApply.TabIndex = 8;
            this.txtBZ98RGogApply.Text = "Apply";
            this.txtBZ98RGogApply.UseVisualStyleBackColor = true;
            this.txtBZ98RGogApply.Click += new System.EventHandler(this.txtBZ98RGogApply_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.txtBZCCSteam);
            this.groupBox2.Controls.Add(this.btnBZCCSteamApply);
            this.groupBox2.Location = new System.Drawing.Point(6, 62);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(723, 50);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "steamapps folder that contains BZCC";
            // 
            // txtBZCCSteam
            // 
            this.txtBZCCSteam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBZCCSteam.Location = new System.Drawing.Point(6, 21);
            this.txtBZCCSteam.Name = "txtBZCCSteam";
            this.txtBZCCSteam.Size = new System.Drawing.Size(649, 20);
            this.txtBZCCSteam.TabIndex = 6;
            // 
            // btnBZCCSteamApply
            // 
            this.btnBZCCSteamApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBZCCSteamApply.Location = new System.Drawing.Point(661, 19);
            this.btnBZCCSteamApply.Name = "btnBZCCSteamApply";
            this.btnBZCCSteamApply.Size = new System.Drawing.Size(57, 23);
            this.btnBZCCSteamApply.TabIndex = 8;
            this.btnBZCCSteamApply.Text = "Apply";
            this.btnBZCCSteamApply.UseVisualStyleBackColor = true;
            this.btnBZCCSteamApply.Click += new System.EventHandler(this.btnBZCCSteamApply_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.txtBZ98RSteam);
            this.groupBox1.Controls.Add(this.btnBZ98RSteamApply);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(723, 50);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "steamapps folder that contains BZ98R";
            // 
            // txtBZ98RSteam
            // 
            this.txtBZ98RSteam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBZ98RSteam.Location = new System.Drawing.Point(6, 21);
            this.txtBZ98RSteam.Name = "txtBZ98RSteam";
            this.txtBZ98RSteam.Size = new System.Drawing.Size(649, 20);
            this.txtBZ98RSteam.TabIndex = 6;
            // 
            // btnBZ98RSteamApply
            // 
            this.btnBZ98RSteamApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBZ98RSteamApply.Location = new System.Drawing.Point(661, 19);
            this.btnBZ98RSteamApply.Name = "btnBZ98RSteamApply";
            this.btnBZ98RSteamApply.Size = new System.Drawing.Size(57, 23);
            this.btnBZ98RSteamApply.TabIndex = 8;
            this.btnBZ98RSteamApply.Text = "Apply";
            this.btnBZ98RSteamApply.UseVisualStyleBackColor = true;
            this.btnBZ98RSteamApply.Click += new System.EventHandler(this.btnBZ98RSteamApply_Click);
            // 
            // tpTasks
            // 
            this.tpTasks.Controls.Add(this.pnlTasks);
            this.tpTasks.Location = new System.Drawing.Point(4, 22);
            this.tpTasks.Name = "tpTasks";
            this.tpTasks.Padding = new System.Windows.Forms.Padding(3);
            this.tpTasks.Size = new System.Drawing.Size(734, 320);
            this.tpTasks.TabIndex = 6;
            this.tpTasks.Text = "Tasks";
            this.tpTasks.UseVisualStyleBackColor = true;
            // 
            // pnlTasks
            // 
            this.pnlTasks.AutoSize = true;
            this.pnlTasks.ColumnCount = 1;
            this.pnlTasks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlTasks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTasks.Location = new System.Drawing.Point(3, 3);
            this.pnlTasks.Name = "pnlTasks";
            this.pnlTasks.RowCount = 1;
            this.pnlTasks.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlTasks.Size = new System.Drawing.Size(728, 314);
            this.pnlTasks.TabIndex = 0;
            this.pnlTasks.Resize += new System.EventHandler(this.pnlTasks_Resize);
            // 
            // tpLog
            // 
            this.tpLog.Controls.Add(this.txtLog);
            this.tpLog.Location = new System.Drawing.Point(4, 22);
            this.tpLog.Name = "tpLog";
            this.tpLog.Padding = new System.Windows.Forms.Padding(3);
            this.tpLog.Size = new System.Drawing.Size(734, 320);
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
            this.txtLog.Size = new System.Drawing.Size(728, 314);
            this.txtLog.TabIndex = 0;
            // 
            // tpLogSteamCmd
            // 
            this.tpLogSteamCmd.Controls.Add(this.txtLogSteamCmd);
            this.tpLogSteamCmd.Location = new System.Drawing.Point(4, 22);
            this.tpLogSteamCmd.Name = "tpLogSteamCmd";
            this.tpLogSteamCmd.Padding = new System.Windows.Forms.Padding(3);
            this.tpLogSteamCmd.Size = new System.Drawing.Size(734, 320);
            this.tpLogSteamCmd.TabIndex = 4;
            this.tpLogSteamCmd.Text = "SteamCmd";
            this.tpLogSteamCmd.UseVisualStyleBackColor = true;
            // 
            // txtLogSteamCmd
            // 
            this.txtLogSteamCmd.BackColor = System.Drawing.Color.Black;
            this.txtLogSteamCmd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLogSteamCmd.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLogSteamCmd.ForeColor = System.Drawing.Color.White;
            this.txtLogSteamCmd.Location = new System.Drawing.Point(3, 3);
            this.txtLogSteamCmd.Name = "txtLogSteamCmd";
            this.txtLogSteamCmd.ReadOnly = true;
            this.txtLogSteamCmd.Size = new System.Drawing.Size(728, 314);
            this.txtLogSteamCmd.TabIndex = 1;
            this.txtLogSteamCmd.Text = "";
            // 
            // tpLogSteamCmdFull
            // 
            this.tpLogSteamCmdFull.Controls.Add(this.txtLogSteamCmdFull);
            this.tpLogSteamCmdFull.Location = new System.Drawing.Point(4, 22);
            this.tpLogSteamCmdFull.Name = "tpLogSteamCmdFull";
            this.tpLogSteamCmdFull.Padding = new System.Windows.Forms.Padding(3);
            this.tpLogSteamCmdFull.Size = new System.Drawing.Size(734, 320);
            this.tpLogSteamCmdFull.TabIndex = 5;
            this.tpLogSteamCmdFull.Text = "SteamCmd Raw";
            this.tpLogSteamCmdFull.UseVisualStyleBackColor = true;
            // 
            // txtLogSteamCmdFull
            // 
            this.txtLogSteamCmdFull.BackColor = System.Drawing.Color.Black;
            this.txtLogSteamCmdFull.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLogSteamCmdFull.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLogSteamCmdFull.ForeColor = System.Drawing.Color.White;
            this.txtLogSteamCmdFull.Location = new System.Drawing.Point(3, 3);
            this.txtLogSteamCmdFull.Name = "txtLogSteamCmdFull";
            this.txtLogSteamCmdFull.ReadOnly = true;
            this.txtLogSteamCmdFull.Size = new System.Drawing.Size(728, 314);
            this.txtLogSteamCmdFull.TabIndex = 2;
            this.txtLogSteamCmdFull.Text = "";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.tsslSteamCmd,
            this.tsslSteamCmdCommand,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 361);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(766, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.AutoSize = false;
            this.toolStripStatusLabel1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(67, 17);
            this.toolStripStatusLabel1.Text = "SteamCmd";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tsslSteamCmd
            // 
            this.tsslSteamCmd.Name = "tsslSteamCmd";
            this.tsslSteamCmd.Size = new System.Drawing.Size(24, 17);
            this.tsslSteamCmd.Text = "Off";
            this.tsslSteamCmd.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tsslSteamCmdCommand
            // 
            this.tsslSteamCmdCommand.Enabled = false;
            this.tsslSteamCmdCommand.Name = "tsslSteamCmdCommand";
            this.tsslSteamCmdCommand.Size = new System.Drawing.Size(34, 17);
            this.tsslSteamCmdCommand.Text = "none";
            this.tsslSteamCmdCommand.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(626, 17);
            this.toolStripStatusLabel2.Spring = true;
            this.toolStripStatusLabel2.Text = "-";
            // 
            // ofdGOGBZCCASM
            // 
            this.ofdGOGBZCCASM.FileName = "battlezone2.exe";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(766, 383);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tabControl1);
            this.Name = "MainForm";
            this.Text = "Battlezone Redux Mod Manager";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tpBZ98R.ResumeLayout(false);
            this.tpBZ98R.PerformLayout();
            this.tpBZCC.ResumeLayout(false);
            this.tpBZCC.PerformLayout();
            this.tpSettings.ResumeLayout(false);
            this.tpSettings.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tpTasks.ResumeLayout(false);
            this.tpTasks.PerformLayout();
            this.tpLog.ResumeLayout(false);
            this.tpLog.PerformLayout();
            this.tpLogSteamCmd.ResumeLayout(false);
            this.tpLogSteamCmdFull.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
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
        private System.Windows.Forms.ToolStripStatusLabel tsslSteamCmd;
        private System.Windows.Forms.ToolStripStatusLabel tsslSteamCmdCommand;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.TabPage tpLog;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.TabPage tpLogSteamCmd;
        private System.Windows.Forms.RichTextBox txtLogSteamCmd;
        private System.Windows.Forms.TabPage tpLogSteamCmdFull;
        private System.Windows.Forms.RichTextBox txtLogSteamCmdFull;
        private LinqListView lvModsBZ98R;
        private System.Windows.Forms.Button btnRefreshBZ98R;
        private System.Windows.Forms.Button btnUpdateBZ98R;
        private LinqListView lvModsBZCC;
        private System.Windows.Forms.Button btnUpdateBZCC;
        private System.Windows.Forms.Button btnRefreshBZCC;
        private System.Windows.Forms.Button btnDependenciesBZ98R;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txtBZCCMyDocs;
        private System.Windows.Forms.Button btnBZCCMyDocsApply;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtBZ98RGog;
        private System.Windows.Forms.Button txtBZ98RGogApply;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtBZCCSteam;
        private System.Windows.Forms.Button btnBZCCSteamApply;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtBZ98RSteam;
        private System.Windows.Forms.Button btnBZ98RSteamApply;
        private System.Windows.Forms.Button btnGOGBZCCASM;
        private System.Windows.Forms.OpenFileDialog ofdGOGBZCCASM;
        private System.Windows.Forms.Button btnGOGBZCCASMAbout;
        private System.Windows.Forms.TabPage tpTasks;
        private System.Windows.Forms.TableLayoutPanel pnlTasks;
        private System.Windows.Forms.CheckBox cbFallbackSteamCmdWindowHandling;
    }
}

