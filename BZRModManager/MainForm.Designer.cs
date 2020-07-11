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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cbBZ98RTypeError = new System.Windows.Forms.CheckBox();
            this.cbBZ98RTypeCampaign = new System.Windows.Forms.CheckBox();
            this.cbBZ98RTypeInstantAction = new System.Windows.Forms.CheckBox();
            this.cbBZ98RTypeMultiplayer = new System.Windows.Forms.CheckBox();
            this.cbBZ98RTypeMod = new System.Windows.Forms.CheckBox();
            this.btnHardUpdateBZ98R = new System.Windows.Forms.Button();
            this.btnUpdateBZ98R = new System.Windows.Forms.Button();
            this.btnRefreshBZ98R = new System.Windows.Forms.Button();
            this.lvModsBZ98R = new BZRModManager.LinqListView();
            this.btnDownloadBZ98R = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtDownloadBZ98R = new System.Windows.Forms.TextBox();
            this.tpBZCC = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.cbBZCCTypeAsset = new System.Windows.Forms.CheckBox();
            this.cbBZCCTypeError = new System.Windows.Forms.CheckBox();
            this.cbBZCCTypeConfig = new System.Windows.Forms.CheckBox();
            this.cbBZCCTypeAddon = new System.Windows.Forms.CheckBox();
            this.btnHardUpdateBZCC = new System.Windows.Forms.Button();
            this.btnDependenciesBZ98R = new System.Windows.Forms.Button();
            this.btnUpdateBZCC = new System.Windows.Forms.Button();
            this.btnRefreshBZCC = new System.Windows.Forms.Button();
            this.lvModsBZCC = new BZRModManager.LinqListView();
            this.btnDownloadBZCC = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDownloadBZCC = new System.Windows.Forms.TextBox();
            this.tpSettings = new System.Windows.Forms.TabPage();
            this.cbFallbackSteamCmdWindowHandling = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnBZCCMyDocsFind = new System.Windows.Forms.Button();
            this.txtBZCCMyDocs = new System.Windows.Forms.TextBox();
            this.btnBZCCMyDocsApply = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnBZ98RGogFind = new System.Windows.Forms.Button();
            this.txtBZ98RGog = new System.Windows.Forms.TextBox();
            this.btnBZ98RGogApply = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnBZCCSteamFind = new System.Windows.Forms.Button();
            this.txtBZCCSteam = new System.Windows.Forms.TextBox();
            this.btnBZCCSteamApply = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnBZ98RSteamFind = new System.Windows.Forms.Button();
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
            this.toolStripStatusLabel5 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslActiveTasks = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.ofdGOGBZCCASM = new System.Windows.Forms.OpenFileDialog();
            this.btnFindModsBZ98R = new System.Windows.Forms.Button();
            this.btnFindModsBZCC = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tpBZ98R.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tpBZCC.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
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
            this.tabControl1.Size = new System.Drawing.Size(742, 407);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tpBZ98R
            // 
            this.tpBZ98R.Controls.Add(this.btnFindModsBZ98R);
            this.tpBZ98R.Controls.Add(this.tableLayoutPanel1);
            this.tpBZ98R.Controls.Add(this.btnHardUpdateBZ98R);
            this.tpBZ98R.Controls.Add(this.btnUpdateBZ98R);
            this.tpBZ98R.Controls.Add(this.btnRefreshBZ98R);
            this.tpBZ98R.Controls.Add(this.lvModsBZ98R);
            this.tpBZ98R.Controls.Add(this.btnDownloadBZ98R);
            this.tpBZ98R.Controls.Add(this.label1);
            this.tpBZ98R.Controls.Add(this.txtDownloadBZ98R);
            this.tpBZ98R.Location = new System.Drawing.Point(4, 22);
            this.tpBZ98R.Name = "tpBZ98R";
            this.tpBZ98R.Padding = new System.Windows.Forms.Padding(3);
            this.tpBZ98R.Size = new System.Drawing.Size(734, 381);
            this.tpBZ98R.TabIndex = 0;
            this.tpBZ98R.Text = "BZ98R";
            this.tpBZ98R.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.cbBZ98RTypeError, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.cbBZ98RTypeCampaign, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.cbBZ98RTypeInstantAction, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.cbBZ98RTypeMultiplayer, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.cbBZ98RTypeMod, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 35);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(434, 23);
            this.tableLayoutPanel1.TabIndex = 10;
            // 
            // cbBZ98RTypeError
            // 
            this.cbBZ98RTypeError.AutoSize = true;
            this.cbBZ98RTypeError.Checked = true;
            this.cbBZ98RTypeError.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbBZ98RTypeError.Location = new System.Drawing.Point(312, 3);
            this.cbBZ98RTypeError.Name = "cbBZ98RTypeError";
            this.cbBZ98RTypeError.Size = new System.Drawing.Size(47, 17);
            this.cbBZ98RTypeError.TabIndex = 9;
            this.cbBZ98RTypeError.Text = "error";
            this.cbBZ98RTypeError.UseVisualStyleBackColor = true;
            this.cbBZ98RTypeError.CheckedChanged += new System.EventHandler(this.cbBZ98RType_CheckedChanged);
            // 
            // cbBZ98RTypeCampaign
            // 
            this.cbBZ98RTypeCampaign.AutoSize = true;
            this.cbBZ98RTypeCampaign.Checked = true;
            this.cbBZ98RTypeCampaign.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbBZ98RTypeCampaign.Location = new System.Drawing.Point(234, 3);
            this.cbBZ98RTypeCampaign.Name = "cbBZ98RTypeCampaign";
            this.cbBZ98RTypeCampaign.Size = new System.Drawing.Size(72, 17);
            this.cbBZ98RTypeCampaign.TabIndex = 8;
            this.cbBZ98RTypeCampaign.Text = "campaign";
            this.cbBZ98RTypeCampaign.UseVisualStyleBackColor = true;
            this.cbBZ98RTypeCampaign.CheckedChanged += new System.EventHandler(this.cbBZ98RType_CheckedChanged);
            // 
            // cbBZ98RTypeInstantAction
            // 
            this.cbBZ98RTypeInstantAction.AutoSize = true;
            this.cbBZ98RTypeInstantAction.Checked = true;
            this.cbBZ98RTypeInstantAction.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbBZ98RTypeInstantAction.Location = new System.Drawing.Point(136, 3);
            this.cbBZ98RTypeInstantAction.Name = "cbBZ98RTypeInstantAction";
            this.cbBZ98RTypeInstantAction.Size = new System.Drawing.Size(92, 17);
            this.cbBZ98RTypeInstantAction.TabIndex = 7;
            this.cbBZ98RTypeInstantAction.Text = "instant_action";
            this.cbBZ98RTypeInstantAction.UseVisualStyleBackColor = true;
            this.cbBZ98RTypeInstantAction.CheckedChanged += new System.EventHandler(this.cbBZ98RType_CheckedChanged);
            // 
            // cbBZ98RTypeMultiplayer
            // 
            this.cbBZ98RTypeMultiplayer.AutoSize = true;
            this.cbBZ98RTypeMultiplayer.Checked = true;
            this.cbBZ98RTypeMultiplayer.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbBZ98RTypeMultiplayer.Location = new System.Drawing.Point(55, 3);
            this.cbBZ98RTypeMultiplayer.Name = "cbBZ98RTypeMultiplayer";
            this.cbBZ98RTypeMultiplayer.Size = new System.Drawing.Size(75, 17);
            this.cbBZ98RTypeMultiplayer.TabIndex = 6;
            this.cbBZ98RTypeMultiplayer.Text = "multiplayer";
            this.cbBZ98RTypeMultiplayer.UseVisualStyleBackColor = true;
            this.cbBZ98RTypeMultiplayer.CheckedChanged += new System.EventHandler(this.cbBZ98RType_CheckedChanged);
            // 
            // cbBZ98RTypeMod
            // 
            this.cbBZ98RTypeMod.AutoSize = true;
            this.cbBZ98RTypeMod.Checked = true;
            this.cbBZ98RTypeMod.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbBZ98RTypeMod.Location = new System.Drawing.Point(3, 3);
            this.cbBZ98RTypeMod.Name = "cbBZ98RTypeMod";
            this.cbBZ98RTypeMod.Size = new System.Drawing.Size(46, 17);
            this.cbBZ98RTypeMod.TabIndex = 5;
            this.cbBZ98RTypeMod.Text = "mod";
            this.cbBZ98RTypeMod.UseVisualStyleBackColor = true;
            this.cbBZ98RTypeMod.CheckedChanged += new System.EventHandler(this.cbBZ98RType_CheckedChanged);
            // 
            // btnHardUpdateBZ98R
            // 
            this.btnHardUpdateBZ98R.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHardUpdateBZ98R.Location = new System.Drawing.Point(446, 35);
            this.btnHardUpdateBZ98R.Name = "btnHardUpdateBZ98R";
            this.btnHardUpdateBZ98R.Size = new System.Drawing.Size(90, 23);
            this.btnHardUpdateBZ98R.TabIndex = 10;
            this.btnHardUpdateBZ98R.Text = "Hard Update";
            this.btnHardUpdateBZ98R.UseVisualStyleBackColor = true;
            this.btnHardUpdateBZ98R.Click += new System.EventHandler(this.btnHardUpdateBZ98R_Click);
            // 
            // btnUpdateBZ98R
            // 
            this.btnUpdateBZ98R.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdateBZ98R.Location = new System.Drawing.Point(542, 35);
            this.btnUpdateBZ98R.Name = "btnUpdateBZ98R";
            this.btnUpdateBZ98R.Size = new System.Drawing.Size(90, 23);
            this.btnUpdateBZ98R.TabIndex = 11;
            this.btnUpdateBZ98R.Text = "Update Mods";
            this.btnUpdateBZ98R.UseVisualStyleBackColor = true;
            this.btnUpdateBZ98R.Click += new System.EventHandler(this.btnUpdateBZ98R_Click);
            // 
            // btnRefreshBZ98R
            // 
            this.btnRefreshBZ98R.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshBZ98R.Location = new System.Drawing.Point(638, 35);
            this.btnRefreshBZ98R.Name = "btnRefreshBZ98R";
            this.btnRefreshBZ98R.Size = new System.Drawing.Size(90, 23);
            this.btnRefreshBZ98R.TabIndex = 12;
            this.btnRefreshBZ98R.Text = "Refresh List";
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
            this.lvModsBZ98R.HideSelection = false;
            this.lvModsBZ98R.Location = new System.Drawing.Point(6, 64);
            this.lvModsBZ98R.MultiSelect = false;
            this.lvModsBZ98R.Name = "lvModsBZ98R";
            this.lvModsBZ98R.Size = new System.Drawing.Size(722, 311);
            this.lvModsBZ98R.TabIndex = 13;
            this.lvModsBZ98R.TypeFilter = null;
            this.lvModsBZ98R.UseCompatibleStateImageBehavior = false;
            this.lvModsBZ98R.View = System.Windows.Forms.View.Details;
            this.lvModsBZ98R.VirtualMode = true;
            // 
            // btnDownloadBZ98R
            // 
            this.btnDownloadBZ98R.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDownloadBZ98R.Location = new System.Drawing.Point(542, 7);
            this.btnDownloadBZ98R.Name = "btnDownloadBZ98R";
            this.btnDownloadBZ98R.Size = new System.Drawing.Size(90, 23);
            this.btnDownloadBZ98R.TabIndex = 3;
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
            this.txtDownloadBZ98R.Size = new System.Drawing.Size(468, 20);
            this.txtDownloadBZ98R.TabIndex = 2;
            // 
            // tpBZCC
            // 
            this.tpBZCC.Controls.Add(this.btnFindModsBZCC);
            this.tpBZCC.Controls.Add(this.tableLayoutPanel2);
            this.tpBZCC.Controls.Add(this.btnHardUpdateBZCC);
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
            this.tpBZCC.Size = new System.Drawing.Size(734, 381);
            this.tpBZCC.TabIndex = 1;
            this.tpBZCC.Text = "BZCC";
            this.tpBZCC.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Controls.Add(this.cbBZCCTypeAsset, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.cbBZCCTypeError, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.cbBZCCTypeConfig, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.cbBZCCTypeAddon, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(6, 35);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(288, 23);
            this.tableLayoutPanel2.TabIndex = 12;
            // 
            // cbBZCCTypeAsset
            // 
            this.cbBZCCTypeAsset.AutoSize = true;
            this.cbBZCCTypeAsset.Checked = true;
            this.cbBZCCTypeAsset.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbBZCCTypeAsset.Location = new System.Drawing.Point(65, 3);
            this.cbBZCCTypeAsset.Name = "cbBZCCTypeAsset";
            this.cbBZCCTypeAsset.Size = new System.Drawing.Size(51, 17);
            this.cbBZCCTypeAsset.TabIndex = 6;
            this.cbBZCCTypeAsset.Text = "asset";
            this.cbBZCCTypeAsset.UseVisualStyleBackColor = true;
            this.cbBZCCTypeAsset.CheckStateChanged += new System.EventHandler(this.cbBZCCType_CheckedChanged);
            // 
            // cbBZCCTypeError
            // 
            this.cbBZCCTypeError.AutoSize = true;
            this.cbBZCCTypeError.Checked = true;
            this.cbBZCCTypeError.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbBZCCTypeError.Location = new System.Drawing.Point(183, 3);
            this.cbBZCCTypeError.Name = "cbBZCCTypeError";
            this.cbBZCCTypeError.Size = new System.Drawing.Size(47, 17);
            this.cbBZCCTypeError.TabIndex = 8;
            this.cbBZCCTypeError.Text = "error";
            this.cbBZCCTypeError.UseVisualStyleBackColor = true;
            this.cbBZCCTypeError.CheckStateChanged += new System.EventHandler(this.cbBZCCType_CheckedChanged);
            // 
            // cbBZCCTypeConfig
            // 
            this.cbBZCCTypeConfig.AutoSize = true;
            this.cbBZCCTypeConfig.Checked = true;
            this.cbBZCCTypeConfig.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbBZCCTypeConfig.Location = new System.Drawing.Point(122, 3);
            this.cbBZCCTypeConfig.Name = "cbBZCCTypeConfig";
            this.cbBZCCTypeConfig.Size = new System.Drawing.Size(55, 17);
            this.cbBZCCTypeConfig.TabIndex = 7;
            this.cbBZCCTypeConfig.Text = "config";
            this.cbBZCCTypeConfig.UseVisualStyleBackColor = true;
            this.cbBZCCTypeConfig.CheckStateChanged += new System.EventHandler(this.cbBZCCType_CheckedChanged);
            // 
            // cbBZCCTypeAddon
            // 
            this.cbBZCCTypeAddon.AutoSize = true;
            this.cbBZCCTypeAddon.Checked = true;
            this.cbBZCCTypeAddon.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbBZCCTypeAddon.Location = new System.Drawing.Point(3, 3);
            this.cbBZCCTypeAddon.Name = "cbBZCCTypeAddon";
            this.cbBZCCTypeAddon.Size = new System.Drawing.Size(56, 17);
            this.cbBZCCTypeAddon.TabIndex = 5;
            this.cbBZCCTypeAddon.Text = "addon";
            this.cbBZCCTypeAddon.UseVisualStyleBackColor = true;
            this.cbBZCCTypeAddon.CheckStateChanged += new System.EventHandler(this.cbBZCCType_CheckedChanged);
            // 
            // btnHardUpdateBZCC
            // 
            this.btnHardUpdateBZCC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHardUpdateBZCC.Location = new System.Drawing.Point(446, 35);
            this.btnHardUpdateBZCC.Name = "btnHardUpdateBZCC";
            this.btnHardUpdateBZCC.Size = new System.Drawing.Size(90, 23);
            this.btnHardUpdateBZCC.TabIndex = 10;
            this.btnHardUpdateBZCC.Text = "Hard Update";
            this.btnHardUpdateBZCC.UseVisualStyleBackColor = true;
            this.btnHardUpdateBZCC.Click += new System.EventHandler(this.btnHardUpdateBZCC_Click);
            // 
            // btnDependenciesBZ98R
            // 
            this.btnDependenciesBZ98R.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDependenciesBZ98R.Location = new System.Drawing.Point(300, 35);
            this.btnDependenciesBZ98R.Name = "btnDependenciesBZ98R";
            this.btnDependenciesBZ98R.Size = new System.Drawing.Size(140, 23);
            this.btnDependenciesBZ98R.TabIndex = 9;
            this.btnDependenciesBZ98R.Text = "Download Dependencies";
            this.btnDependenciesBZ98R.UseVisualStyleBackColor = true;
            this.btnDependenciesBZ98R.Click += new System.EventHandler(this.btnDependenciesBZ98R_Click);
            // 
            // btnUpdateBZCC
            // 
            this.btnUpdateBZCC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdateBZCC.Location = new System.Drawing.Point(542, 35);
            this.btnUpdateBZCC.Name = "btnUpdateBZCC";
            this.btnUpdateBZCC.Size = new System.Drawing.Size(90, 23);
            this.btnUpdateBZCC.TabIndex = 11;
            this.btnUpdateBZCC.Text = "Update Mods";
            this.btnUpdateBZCC.UseVisualStyleBackColor = true;
            this.btnUpdateBZCC.Click += new System.EventHandler(this.btnUpdateBZCC_Click);
            // 
            // btnRefreshBZCC
            // 
            this.btnRefreshBZCC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshBZCC.Location = new System.Drawing.Point(638, 35);
            this.btnRefreshBZCC.Name = "btnRefreshBZCC";
            this.btnRefreshBZCC.Size = new System.Drawing.Size(90, 23);
            this.btnRefreshBZCC.TabIndex = 12;
            this.btnRefreshBZCC.Text = "Refresh List";
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
            this.lvModsBZCC.HideSelection = false;
            this.lvModsBZCC.Location = new System.Drawing.Point(6, 64);
            this.lvModsBZCC.MultiSelect = false;
            this.lvModsBZCC.Name = "lvModsBZCC";
            this.lvModsBZCC.Size = new System.Drawing.Size(722, 311);
            this.lvModsBZCC.TabIndex = 13;
            this.lvModsBZCC.TypeFilter = null;
            this.lvModsBZCC.UseCompatibleStateImageBehavior = false;
            this.lvModsBZCC.View = System.Windows.Forms.View.Details;
            this.lvModsBZCC.VirtualMode = true;
            // 
            // btnDownloadBZCC
            // 
            this.btnDownloadBZCC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDownloadBZCC.Location = new System.Drawing.Point(542, 7);
            this.btnDownloadBZCC.Name = "btnDownloadBZCC";
            this.btnDownloadBZCC.Size = new System.Drawing.Size(90, 23);
            this.btnDownloadBZCC.TabIndex = 3;
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
            this.label2.TabIndex = 1;
            this.label2.Text = "Mod URL:";
            // 
            // txtDownloadBZCC
            // 
            this.txtDownloadBZCC.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDownloadBZCC.Location = new System.Drawing.Point(68, 9);
            this.txtDownloadBZCC.Name = "txtDownloadBZCC";
            this.txtDownloadBZCC.Size = new System.Drawing.Size(468, 20);
            this.txtDownloadBZCC.TabIndex = 2;
            // 
            // tpSettings
            // 
            this.tpSettings.Controls.Add(this.cbFallbackSteamCmdWindowHandling);
            this.tpSettings.Controls.Add(this.groupBox4);
            this.tpSettings.Controls.Add(this.groupBox3);
            this.tpSettings.Controls.Add(this.groupBox2);
            this.tpSettings.Controls.Add(this.groupBox1);
            this.tpSettings.Location = new System.Drawing.Point(4, 22);
            this.tpSettings.Name = "tpSettings";
            this.tpSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tpSettings.Size = new System.Drawing.Size(734, 381);
            this.tpSettings.TabIndex = 2;
            this.tpSettings.Text = "Settings";
            this.tpSettings.UseVisualStyleBackColor = true;
            // 
            // cbFallbackSteamCmdWindowHandling
            // 
            this.cbFallbackSteamCmdWindowHandling.AutoSize = true;
            this.cbFallbackSteamCmdWindowHandling.Location = new System.Drawing.Point(6, 230);
            this.cbFallbackSteamCmdWindowHandling.Name = "cbFallbackSteamCmdWindowHandling";
            this.cbFallbackSteamCmdWindowHandling.Size = new System.Drawing.Size(207, 17);
            this.cbFallbackSteamCmdWindowHandling.TabIndex = 17;
            this.cbFallbackSteamCmdWindowHandling.Text = "Fallback SteamCmd Window Handling";
            this.cbFallbackSteamCmdWindowHandling.UseVisualStyleBackColor = true;
            this.cbFallbackSteamCmdWindowHandling.Visible = false;
            this.cbFallbackSteamCmdWindowHandling.CheckedChanged += new System.EventHandler(this.cbFallbackSteamCmdWindowHandling_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.btnBZCCMyDocsFind);
            this.groupBox4.Controls.Add(this.txtBZCCMyDocs);
            this.groupBox4.Controls.Add(this.btnBZCCMyDocsApply);
            this.groupBox4.Location = new System.Drawing.Point(6, 174);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(723, 50);
            this.groupBox4.TabIndex = 13;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "BZCC Folder in My Docs/My Games";
            // 
            // btnBZCCMyDocsFind
            // 
            this.btnBZCCMyDocsFind.Location = new System.Drawing.Point(6, 19);
            this.btnBZCCMyDocsFind.Name = "btnBZCCMyDocsFind";
            this.btnBZCCMyDocsFind.Size = new System.Drawing.Size(74, 23);
            this.btnBZCCMyDocsFind.TabIndex = 14;
            this.btnBZCCMyDocsFind.Text = "Quick Find";
            this.btnBZCCMyDocsFind.UseVisualStyleBackColor = true;
            this.btnBZCCMyDocsFind.Click += new System.EventHandler(this.btnBZCCMyDocsFind_Click);
            // 
            // txtBZCCMyDocs
            // 
            this.txtBZCCMyDocs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBZCCMyDocs.Location = new System.Drawing.Point(86, 21);
            this.txtBZCCMyDocs.Name = "txtBZCCMyDocs";
            this.txtBZCCMyDocs.Size = new System.Drawing.Size(569, 20);
            this.txtBZCCMyDocs.TabIndex = 15;
            // 
            // btnBZCCMyDocsApply
            // 
            this.btnBZCCMyDocsApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBZCCMyDocsApply.Location = new System.Drawing.Point(661, 19);
            this.btnBZCCMyDocsApply.Name = "btnBZCCMyDocsApply";
            this.btnBZCCMyDocsApply.Size = new System.Drawing.Size(57, 23);
            this.btnBZCCMyDocsApply.TabIndex = 16;
            this.btnBZCCMyDocsApply.Text = "Apply";
            this.btnBZCCMyDocsApply.UseVisualStyleBackColor = true;
            this.btnBZCCMyDocsApply.Click += new System.EventHandler(this.btnBZCCMyDocsApply_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.btnBZ98RGogFind);
            this.groupBox3.Controls.Add(this.txtBZ98RGog);
            this.groupBox3.Controls.Add(this.btnBZ98RGogApply);
            this.groupBox3.Location = new System.Drawing.Point(6, 118);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(723, 50);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "GOG install of BZ98R";
            // 
            // btnBZ98RGogFind
            // 
            this.btnBZ98RGogFind.Location = new System.Drawing.Point(6, 19);
            this.btnBZ98RGogFind.Name = "btnBZ98RGogFind";
            this.btnBZ98RGogFind.Size = new System.Drawing.Size(74, 23);
            this.btnBZ98RGogFind.TabIndex = 10;
            this.btnBZ98RGogFind.Text = "Quick Find";
            this.btnBZ98RGogFind.UseVisualStyleBackColor = true;
            this.btnBZ98RGogFind.Click += new System.EventHandler(this.btnBZ98RGogFind_Click);
            // 
            // txtBZ98RGog
            // 
            this.txtBZ98RGog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBZ98RGog.Location = new System.Drawing.Point(86, 21);
            this.txtBZ98RGog.Name = "txtBZ98RGog";
            this.txtBZ98RGog.Size = new System.Drawing.Size(569, 20);
            this.txtBZ98RGog.TabIndex = 11;
            // 
            // btnBZ98RGogApply
            // 
            this.btnBZ98RGogApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBZ98RGogApply.Location = new System.Drawing.Point(661, 19);
            this.btnBZ98RGogApply.Name = "btnBZ98RGogApply";
            this.btnBZ98RGogApply.Size = new System.Drawing.Size(57, 23);
            this.btnBZ98RGogApply.TabIndex = 12;
            this.btnBZ98RGogApply.Text = "Apply";
            this.btnBZ98RGogApply.UseVisualStyleBackColor = true;
            this.btnBZ98RGogApply.Click += new System.EventHandler(this.txtBZ98RGogApply_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btnBZCCSteamFind);
            this.groupBox2.Controls.Add(this.txtBZCCSteam);
            this.groupBox2.Controls.Add(this.btnBZCCSteamApply);
            this.groupBox2.Location = new System.Drawing.Point(6, 62);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(723, 50);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "steamapps folder that contains BZCC";
            // 
            // btnBZCCSteamFind
            // 
            this.btnBZCCSteamFind.Location = new System.Drawing.Point(6, 19);
            this.btnBZCCSteamFind.Name = "btnBZCCSteamFind";
            this.btnBZCCSteamFind.Size = new System.Drawing.Size(74, 23);
            this.btnBZCCSteamFind.TabIndex = 6;
            this.btnBZCCSteamFind.Text = "Quick Find";
            this.btnBZCCSteamFind.UseVisualStyleBackColor = true;
            this.btnBZCCSteamFind.Click += new System.EventHandler(this.btnBZCCSteamFind_Click);
            // 
            // txtBZCCSteam
            // 
            this.txtBZCCSteam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBZCCSteam.Location = new System.Drawing.Point(86, 21);
            this.txtBZCCSteam.Name = "txtBZCCSteam";
            this.txtBZCCSteam.Size = new System.Drawing.Size(569, 20);
            this.txtBZCCSteam.TabIndex = 7;
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
            this.groupBox1.Controls.Add(this.btnBZ98RSteamFind);
            this.groupBox1.Controls.Add(this.txtBZ98RSteam);
            this.groupBox1.Controls.Add(this.btnBZ98RSteamApply);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(723, 50);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "steamapps folder that contains BZ98R";
            // 
            // btnBZ98RSteamFind
            // 
            this.btnBZ98RSteamFind.Location = new System.Drawing.Point(6, 19);
            this.btnBZ98RSteamFind.Name = "btnBZ98RSteamFind";
            this.btnBZ98RSteamFind.Size = new System.Drawing.Size(74, 23);
            this.btnBZ98RSteamFind.TabIndex = 2;
            this.btnBZ98RSteamFind.Text = "Quick Find";
            this.btnBZ98RSteamFind.UseVisualStyleBackColor = true;
            this.btnBZ98RSteamFind.Click += new System.EventHandler(this.btnBZ98RSteamFind_Click);
            // 
            // txtBZ98RSteam
            // 
            this.txtBZ98RSteam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBZ98RSteam.Location = new System.Drawing.Point(86, 21);
            this.txtBZ98RSteam.Name = "txtBZ98RSteam";
            this.txtBZ98RSteam.Size = new System.Drawing.Size(569, 20);
            this.txtBZ98RSteam.TabIndex = 3;
            // 
            // btnBZ98RSteamApply
            // 
            this.btnBZ98RSteamApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBZ98RSteamApply.Location = new System.Drawing.Point(661, 19);
            this.btnBZ98RSteamApply.Name = "btnBZ98RSteamApply";
            this.btnBZ98RSteamApply.Size = new System.Drawing.Size(57, 23);
            this.btnBZ98RSteamApply.TabIndex = 4;
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
            this.tpTasks.Size = new System.Drawing.Size(734, 381);
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
            this.pnlTasks.Size = new System.Drawing.Size(728, 375);
            this.pnlTasks.TabIndex = 0;
            this.pnlTasks.Resize += new System.EventHandler(this.pnlTasks_Resize);
            // 
            // tpLog
            // 
            this.tpLog.Controls.Add(this.txtLog);
            this.tpLog.Location = new System.Drawing.Point(4, 22);
            this.tpLog.Name = "tpLog";
            this.tpLog.Padding = new System.Windows.Forms.Padding(3);
            this.tpLog.Size = new System.Drawing.Size(734, 381);
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
            this.txtLog.Size = new System.Drawing.Size(728, 375);
            this.txtLog.TabIndex = 0;
            // 
            // tpLogSteamCmd
            // 
            this.tpLogSteamCmd.Controls.Add(this.txtLogSteamCmd);
            this.tpLogSteamCmd.Location = new System.Drawing.Point(4, 22);
            this.tpLogSteamCmd.Name = "tpLogSteamCmd";
            this.tpLogSteamCmd.Padding = new System.Windows.Forms.Padding(3);
            this.tpLogSteamCmd.Size = new System.Drawing.Size(734, 381);
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
            this.txtLogSteamCmd.Size = new System.Drawing.Size(728, 375);
            this.txtLogSteamCmd.TabIndex = 1;
            this.txtLogSteamCmd.Text = "";
            // 
            // tpLogSteamCmdFull
            // 
            this.tpLogSteamCmdFull.Controls.Add(this.txtLogSteamCmdFull);
            this.tpLogSteamCmdFull.Location = new System.Drawing.Point(4, 22);
            this.tpLogSteamCmdFull.Name = "tpLogSteamCmdFull";
            this.tpLogSteamCmdFull.Padding = new System.Windows.Forms.Padding(3);
            this.tpLogSteamCmdFull.Size = new System.Drawing.Size(734, 381);
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
            this.txtLogSteamCmdFull.Size = new System.Drawing.Size(728, 375);
            this.txtLogSteamCmdFull.TabIndex = 2;
            this.txtLogSteamCmdFull.Text = "";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.tsslSteamCmd,
            this.toolStripStatusLabel5,
            this.toolStripStatusLabel3,
            this.tsslActiveTasks,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 422);
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
            // toolStripStatusLabel5
            // 
            this.toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            this.toolStripStatusLabel5.Size = new System.Drawing.Size(290, 17);
            this.toolStripStatusLabel5.Spring = true;
            this.toolStripStatusLabel5.Text = "-";
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.AutoSize = false;
            this.toolStripStatusLabel3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(67, 17);
            this.toolStripStatusLabel3.Text = "Busy Tasks";
            this.toolStripStatusLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tsslActiveTasks
            // 
            this.tsslActiveTasks.Name = "tsslActiveTasks";
            this.tsslActiveTasks.Size = new System.Drawing.Size(13, 17);
            this.tsslActiveTasks.Text = "0";
            this.tsslActiveTasks.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(290, 17);
            this.toolStripStatusLabel2.Spring = true;
            this.toolStripStatusLabel2.Text = "-";
            // 
            // ofdGOGBZCCASM
            // 
            this.ofdGOGBZCCASM.FileName = "battlezone2.exe";
            // 
            // btnFindModsBZ98R
            // 
            this.btnFindModsBZ98R.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFindModsBZ98R.Location = new System.Drawing.Point(638, 7);
            this.btnFindModsBZ98R.Name = "btnFindModsBZ98R";
            this.btnFindModsBZ98R.Size = new System.Drawing.Size(90, 23);
            this.btnFindModsBZ98R.TabIndex = 4;
            this.btnFindModsBZ98R.Text = "Find Mods";
            this.btnFindModsBZ98R.UseVisualStyleBackColor = true;
            // 
            // btnFindModsBZCC
            // 
            this.btnFindModsBZCC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFindModsBZCC.Location = new System.Drawing.Point(638, 7);
            this.btnFindModsBZCC.Name = "btnFindModsBZCC";
            this.btnFindModsBZCC.Size = new System.Drawing.Size(90, 23);
            this.btnFindModsBZCC.TabIndex = 4;
            this.btnFindModsBZCC.Text = "Find Mods";
            this.btnFindModsBZCC.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(766, 444);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tabControl1);
            this.Name = "MainForm";
            this.Text = "Battlezone Redux Mod Manager";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tpBZ98R.ResumeLayout(false);
            this.tpBZ98R.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tpBZCC.ResumeLayout(false);
            this.tpBZCC.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
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
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txtBZCCMyDocs;
        private System.Windows.Forms.Button btnBZCCMyDocsApply;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtBZ98RGog;
        private System.Windows.Forms.Button btnBZ98RGogApply;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtBZCCSteam;
        private System.Windows.Forms.Button btnBZCCSteamApply;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtBZ98RSteam;
        private System.Windows.Forms.Button btnBZ98RSteamApply;
        private System.Windows.Forms.OpenFileDialog ofdGOGBZCCASM;
        private System.Windows.Forms.TabPage tpTasks;
        private System.Windows.Forms.TableLayoutPanel pnlTasks;
        private System.Windows.Forms.CheckBox cbFallbackSteamCmdWindowHandling;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel5;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel tsslActiveTasks;
        private System.Windows.Forms.Button btnHardUpdateBZ98R;
        private System.Windows.Forms.CheckBox cbBZ98RTypeMod;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox cbBZ98RTypeMultiplayer;
        private System.Windows.Forms.CheckBox cbBZ98RTypeError;
        private System.Windows.Forms.CheckBox cbBZ98RTypeCampaign;
        private System.Windows.Forms.CheckBox cbBZ98RTypeInstantAction;
        private System.Windows.Forms.Button btnHardUpdateBZCC;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.CheckBox cbBZCCTypeError;
        private System.Windows.Forms.CheckBox cbBZCCTypeConfig;
        private System.Windows.Forms.CheckBox cbBZCCTypeAddon;
        private System.Windows.Forms.CheckBox cbBZCCTypeAsset;
        private System.Windows.Forms.Button btnBZ98RGogFind;
        private System.Windows.Forms.Button btnBZCCMyDocsFind;
        private System.Windows.Forms.Button btnBZCCSteamFind;
        private System.Windows.Forms.Button btnBZ98RSteamFind;
        private System.Windows.Forms.Button btnFindModsBZ98R;
        private System.Windows.Forms.Button btnFindModsBZCC;
    }
}

