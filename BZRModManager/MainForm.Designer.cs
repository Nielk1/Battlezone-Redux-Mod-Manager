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
            components = new System.ComponentModel.Container();
            tabControl1 = new System.Windows.Forms.TabControl();
            tpBZ98R = new System.Windows.Forms.TabPage();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            cbBZ98RTypeError = new System.Windows.Forms.CheckBox();
            cbBZ98RTypeCampaign = new System.Windows.Forms.CheckBox();
            cbBZ98RTypeInstantAction = new System.Windows.Forms.CheckBox();
            cbBZ98RTypeMultiplayer = new System.Windows.Forms.CheckBox();
            cbBZ98RTypeMod = new System.Windows.Forms.CheckBox();
            btnHardUpdateBZ98R = new System.Windows.Forms.Button();
            btnUpdateBZ98R = new System.Windows.Forms.Button();
            btnRefreshBZ98R = new System.Windows.Forms.Button();
            lvModsBZ98R = new LinqListViewMods();
            btnDownloadBZ98R = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            txtDownloadBZ98R = new System.Windows.Forms.TextBox();
            tpBZCC = new System.Windows.Forms.TabPage();
            tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            cbBZCCTypeAsset = new System.Windows.Forms.CheckBox();
            cbBZCCTypeError = new System.Windows.Forms.CheckBox();
            cbBZCCTypeConfig = new System.Windows.Forms.CheckBox();
            cbBZCCTypeAddon = new System.Windows.Forms.CheckBox();
            btnHardUpdateBZCC = new System.Windows.Forms.Button();
            btnDependenciesBZ98R = new System.Windows.Forms.Button();
            btnUpdateBZCC = new System.Windows.Forms.Button();
            btnRefreshBZCC = new System.Windows.Forms.Button();
            lvModsBZCC = new LinqListViewMods();
            btnDownloadBZCC = new System.Windows.Forms.Button();
            label2 = new System.Windows.Forms.Label();
            txtDownloadBZCC = new System.Windows.Forms.TextBox();
            tpFindMods = new System.Windows.Forms.TabPage();
            btnDownloadSelectedFoundMods = new System.Windows.Forms.Button();
            cbFindModsNewOnly = new System.Windows.Forms.CheckBox();
            rbFindModsTable = new System.Windows.Forms.RadioButton();
            btnFindMods = new System.Windows.Forms.Button();
            rbFindModsIcon = new System.Windows.Forms.RadioButton();
            tcFindMods = new System.Windows.Forms.TabControl();
            tpFindModsBZ98R = new System.Windows.Forms.TabPage();
            lvFindModsBZ98R = new LinqListViewFindMods();
            tpFindModsBZCC = new System.Windows.Forms.TabPage();
            lvFindModsBZCC = new LinqListViewFindMods();
            tabMultiplayer = new System.Windows.Forms.TabPage();
            lvPlayers = new LinqListViewPlayers();
            rbFindGamesTable = new System.Windows.Forms.RadioButton();
            btnGetModSteamCmd = new System.Windows.Forms.Button();
            btnMultiGetModSteam = new System.Windows.Forms.Button();
            rbFindGamesMap = new System.Windows.Forms.RadioButton();
            btnMultiJoinGOG = new System.Windows.Forms.Button();
            btnMultiJoinSteam = new System.Windows.Forms.Button();
            btnMultiRefresh = new System.Windows.Forms.Button();
            tcMultiplayer = new System.Windows.Forms.TabControl();
            tpMultiplayerBZ98R = new System.Windows.Forms.TabPage();
            lvMultiplayerBZ98R = new LinqListViewMultiplayer();
            tpMultiplayerBZCC = new System.Windows.Forms.TabPage();
            lvMultiplayerBZCC = new LinqListViewMultiplayer();
            tpAudit = new System.Windows.Forms.TabPage();
            txtAuditLog = new System.Windows.Forms.RichTextBox();
            btnRunAudit = new System.Windows.Forms.Button();
            tpSettings = new System.Windows.Forms.TabPage();
            groupBox6 = new System.Windows.Forms.GroupBox();
            btnGitFind = new System.Windows.Forms.Button();
            txtGit = new System.Windows.Forms.TextBox();
            btnGitApply = new System.Windows.Forms.Button();
            groupBox5 = new System.Windows.Forms.GroupBox();
            btnBZCCGogFind = new System.Windows.Forms.Button();
            txtBZCCGog = new System.Windows.Forms.TextBox();
            btnBZCCRGogApply = new System.Windows.Forms.Button();
            btnFixSteamCmd = new System.Windows.Forms.Button();
            cbFallbackSteamCmdWindowHandling = new System.Windows.Forms.CheckBox();
            groupBox4 = new System.Windows.Forms.GroupBox();
            btnBZCCMyDocsFind = new System.Windows.Forms.Button();
            txtBZCCMyDocs = new System.Windows.Forms.TextBox();
            btnBZCCMyDocsApply = new System.Windows.Forms.Button();
            groupBox3 = new System.Windows.Forms.GroupBox();
            btnBZ98RGogFind = new System.Windows.Forms.Button();
            txtBZ98RGog = new System.Windows.Forms.TextBox();
            btnBZ98RGogApply = new System.Windows.Forms.Button();
            groupBox2 = new System.Windows.Forms.GroupBox();
            btnBZCCSteamFind = new System.Windows.Forms.Button();
            txtBZCCSteam = new System.Windows.Forms.TextBox();
            btnBZCCSteamApply = new System.Windows.Forms.Button();
            groupBox1 = new System.Windows.Forms.GroupBox();
            btnBZ98RSteamFind = new System.Windows.Forms.Button();
            txtBZ98RSteam = new System.Windows.Forms.TextBox();
            btnBZ98RSteamApply = new System.Windows.Forms.Button();
            tpTasks = new System.Windows.Forms.TabPage();
            pnlTasks = new System.Windows.Forms.TableLayoutPanel();
            tpLog = new System.Windows.Forms.TabPage();
            txtLog = new System.Windows.Forms.TextBox();
            tpLogSteamCmd = new System.Windows.Forms.TabPage();
            txtLogSteamCmd = new System.Windows.Forms.RichTextBox();
            tpLogSteamCmdFull = new System.Windows.Forms.TabPage();
            txtLogSteamCmdFull = new System.Windows.Forms.RichTextBox();
            tpAbout = new System.Windows.Forms.TabPage();
            label3 = new System.Windows.Forms.Label();
            btnGithub = new System.Windows.Forms.Button();
            btnDiscord = new System.Windows.Forms.Button();
            btnSteamAward = new System.Windows.Forms.Button();
            logoPictureBox = new System.Windows.Forms.PictureBox();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            tsslSteamCmd = new System.Windows.Forms.ToolStripStatusLabel();
            toolStripStatusLabel5 = new System.Windows.Forms.ToolStripStatusLabel();
            toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            tsslActiveTasks = new System.Windows.Forms.ToolStripStatusLabel();
            toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            ofdGOGBZCCASM = new System.Windows.Forms.OpenFileDialog();
            tabControl1.SuspendLayout();
            tpBZ98R.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            tpBZCC.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tpFindMods.SuspendLayout();
            tcFindMods.SuspendLayout();
            tpFindModsBZ98R.SuspendLayout();
            tpFindModsBZCC.SuspendLayout();
            tabMultiplayer.SuspendLayout();
            tcMultiplayer.SuspendLayout();
            tpMultiplayerBZ98R.SuspendLayout();
            tpMultiplayerBZCC.SuspendLayout();
            tpAudit.SuspendLayout();
            tpSettings.SuspendLayout();
            groupBox6.SuspendLayout();
            groupBox5.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox1.SuspendLayout();
            tpTasks.SuspendLayout();
            tpLog.SuspendLayout();
            tpLogSteamCmd.SuspendLayout();
            tpLogSteamCmdFull.SuspendLayout();
            tpAbout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)logoPictureBox).BeginInit();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tabControl1.Controls.Add(tpBZ98R);
            tabControl1.Controls.Add(tpBZCC);
            tabControl1.Controls.Add(tpFindMods);
            tabControl1.Controls.Add(tabMultiplayer);
            tabControl1.Controls.Add(tpAudit);
            tabControl1.Controls.Add(tpSettings);
            tabControl1.Controls.Add(tpTasks);
            tabControl1.Controls.Add(tpLog);
            tabControl1.Controls.Add(tpLogSteamCmd);
            tabControl1.Controls.Add(tpLogSteamCmdFull);
            tabControl1.Controls.Add(tpAbout);
            tabControl1.Location = new System.Drawing.Point(14, 14);
            tabControl1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabControl1.Multiline = true;
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(866, 483);
            tabControl1.TabIndex = 0;
            tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
            // 
            // tpBZ98R
            // 
            tpBZ98R.Controls.Add(tableLayoutPanel1);
            tpBZ98R.Controls.Add(btnHardUpdateBZ98R);
            tpBZ98R.Controls.Add(btnUpdateBZ98R);
            tpBZ98R.Controls.Add(btnRefreshBZ98R);
            tpBZ98R.Controls.Add(lvModsBZ98R);
            tpBZ98R.Controls.Add(btnDownloadBZ98R);
            tpBZ98R.Controls.Add(label1);
            tpBZ98R.Controls.Add(txtDownloadBZ98R);
            tpBZ98R.Location = new System.Drawing.Point(4, 24);
            tpBZ98R.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tpBZ98R.Name = "tpBZ98R";
            tpBZ98R.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tpBZ98R.Size = new System.Drawing.Size(858, 455);
            tpBZ98R.TabIndex = 0;
            tpBZ98R.Text = "BZ98R";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tableLayoutPanel1.ColumnCount = 5;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel1.Controls.Add(cbBZ98RTypeError, 4, 0);
            tableLayoutPanel1.Controls.Add(cbBZ98RTypeCampaign, 3, 0);
            tableLayoutPanel1.Controls.Add(cbBZ98RTypeInstantAction, 2, 0);
            tableLayoutPanel1.Controls.Add(cbBZ98RTypeMultiplayer, 1, 0);
            tableLayoutPanel1.Controls.Add(cbBZ98RTypeMod, 0, 0);
            tableLayoutPanel1.Location = new System.Drawing.Point(7, 40);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new System.Drawing.Size(506, 27);
            tableLayoutPanel1.TabIndex = 10;
            // 
            // cbBZ98RTypeError
            // 
            cbBZ98RTypeError.AutoSize = true;
            cbBZ98RTypeError.Checked = true;
            cbBZ98RTypeError.CheckState = System.Windows.Forms.CheckState.Checked;
            cbBZ98RTypeError.Location = new System.Drawing.Point(352, 3);
            cbBZ98RTypeError.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbBZ98RTypeError.Name = "cbBZ98RTypeError";
            cbBZ98RTypeError.Size = new System.Drawing.Size(51, 19);
            cbBZ98RTypeError.TabIndex = 9;
            cbBZ98RTypeError.Text = "error";
            cbBZ98RTypeError.UseVisualStyleBackColor = true;
            cbBZ98RTypeError.CheckedChanged += cbBZ98RType_CheckedChanged;
            // 
            // cbBZ98RTypeCampaign
            // 
            cbBZ98RTypeCampaign.AutoSize = true;
            cbBZ98RTypeCampaign.Checked = true;
            cbBZ98RTypeCampaign.CheckState = System.Windows.Forms.CheckState.Checked;
            cbBZ98RTypeCampaign.Location = new System.Drawing.Point(265, 3);
            cbBZ98RTypeCampaign.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbBZ98RTypeCampaign.Name = "cbBZ98RTypeCampaign";
            cbBZ98RTypeCampaign.Size = new System.Drawing.Size(79, 19);
            cbBZ98RTypeCampaign.TabIndex = 8;
            cbBZ98RTypeCampaign.Text = "campaign";
            cbBZ98RTypeCampaign.UseVisualStyleBackColor = true;
            cbBZ98RTypeCampaign.CheckedChanged += cbBZ98RType_CheckedChanged;
            // 
            // cbBZ98RTypeInstantAction
            // 
            cbBZ98RTypeInstantAction.AutoSize = true;
            cbBZ98RTypeInstantAction.Checked = true;
            cbBZ98RTypeInstantAction.CheckState = System.Windows.Forms.CheckState.Checked;
            cbBZ98RTypeInstantAction.Location = new System.Drawing.Point(157, 3);
            cbBZ98RTypeInstantAction.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbBZ98RTypeInstantAction.Name = "cbBZ98RTypeInstantAction";
            cbBZ98RTypeInstantAction.Size = new System.Drawing.Size(100, 19);
            cbBZ98RTypeInstantAction.TabIndex = 7;
            cbBZ98RTypeInstantAction.Text = "instant_action";
            cbBZ98RTypeInstantAction.UseVisualStyleBackColor = true;
            cbBZ98RTypeInstantAction.CheckedChanged += cbBZ98RType_CheckedChanged;
            // 
            // cbBZ98RTypeMultiplayer
            // 
            cbBZ98RTypeMultiplayer.AutoSize = true;
            cbBZ98RTypeMultiplayer.Checked = true;
            cbBZ98RTypeMultiplayer.CheckState = System.Windows.Forms.CheckState.Checked;
            cbBZ98RTypeMultiplayer.Location = new System.Drawing.Point(63, 3);
            cbBZ98RTypeMultiplayer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbBZ98RTypeMultiplayer.Name = "cbBZ98RTypeMultiplayer";
            cbBZ98RTypeMultiplayer.Size = new System.Drawing.Size(86, 19);
            cbBZ98RTypeMultiplayer.TabIndex = 6;
            cbBZ98RTypeMultiplayer.Text = "multiplayer";
            cbBZ98RTypeMultiplayer.UseVisualStyleBackColor = true;
            cbBZ98RTypeMultiplayer.CheckedChanged += cbBZ98RType_CheckedChanged;
            // 
            // cbBZ98RTypeMod
            // 
            cbBZ98RTypeMod.AutoSize = true;
            cbBZ98RTypeMod.Checked = true;
            cbBZ98RTypeMod.CheckState = System.Windows.Forms.CheckState.Checked;
            cbBZ98RTypeMod.Location = new System.Drawing.Point(4, 3);
            cbBZ98RTypeMod.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbBZ98RTypeMod.Name = "cbBZ98RTypeMod";
            cbBZ98RTypeMod.Size = new System.Drawing.Size(51, 19);
            cbBZ98RTypeMod.TabIndex = 5;
            cbBZ98RTypeMod.Text = "mod";
            cbBZ98RTypeMod.UseVisualStyleBackColor = true;
            cbBZ98RTypeMod.CheckedChanged += cbBZ98RType_CheckedChanged;
            // 
            // btnHardUpdateBZ98R
            // 
            btnHardUpdateBZ98R.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnHardUpdateBZ98R.Location = new System.Drawing.Point(520, 40);
            btnHardUpdateBZ98R.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnHardUpdateBZ98R.Name = "btnHardUpdateBZ98R";
            btnHardUpdateBZ98R.Size = new System.Drawing.Size(105, 27);
            btnHardUpdateBZ98R.TabIndex = 10;
            btnHardUpdateBZ98R.Text = "Hard Update";
            btnHardUpdateBZ98R.UseVisualStyleBackColor = true;
            btnHardUpdateBZ98R.Click += btnHardUpdateBZ98R_Click;
            // 
            // btnUpdateBZ98R
            // 
            btnUpdateBZ98R.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnUpdateBZ98R.Location = new System.Drawing.Point(632, 40);
            btnUpdateBZ98R.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnUpdateBZ98R.Name = "btnUpdateBZ98R";
            btnUpdateBZ98R.Size = new System.Drawing.Size(105, 27);
            btnUpdateBZ98R.TabIndex = 11;
            btnUpdateBZ98R.Text = "Update Mods";
            btnUpdateBZ98R.UseVisualStyleBackColor = true;
            btnUpdateBZ98R.Click += btnUpdateBZ98R_Click;
            // 
            // btnRefreshBZ98R
            // 
            btnRefreshBZ98R.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnRefreshBZ98R.Location = new System.Drawing.Point(744, 40);
            btnRefreshBZ98R.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnRefreshBZ98R.Name = "btnRefreshBZ98R";
            btnRefreshBZ98R.Size = new System.Drawing.Size(105, 27);
            btnRefreshBZ98R.TabIndex = 12;
            btnRefreshBZ98R.Text = "Refresh List";
            btnRefreshBZ98R.UseVisualStyleBackColor = true;
            btnRefreshBZ98R.Click += btnRefreshBZ98R_Click;
            // 
            // lvModsBZ98R
            // 
            lvModsBZ98R.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lvModsBZ98R.FullRowSelect = true;
            lvModsBZ98R.GridLines = true;
            lvModsBZ98R.Location = new System.Drawing.Point(7, 74);
            lvModsBZ98R.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lvModsBZ98R.Name = "lvModsBZ98R";
            lvModsBZ98R.Size = new System.Drawing.Size(842, 358);
            lvModsBZ98R.TabIndex = 13;
            lvModsBZ98R.TypeFilter = null;
            lvModsBZ98R.UseCompatibleStateImageBehavior = false;
            lvModsBZ98R.View = System.Windows.Forms.View.Details;
            lvModsBZ98R.VirtualMode = true;
            // 
            // btnDownloadBZ98R
            // 
            btnDownloadBZ98R.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnDownloadBZ98R.Location = new System.Drawing.Point(744, 8);
            btnDownloadBZ98R.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnDownloadBZ98R.Name = "btnDownloadBZ98R";
            btnDownloadBZ98R.Size = new System.Drawing.Size(105, 27);
            btnDownloadBZ98R.TabIndex = 3;
            btnDownloadBZ98R.Text = "Download";
            btnDownloadBZ98R.UseVisualStyleBackColor = true;
            btnDownloadBZ98R.Click += btnDownloadBZ98R_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(7, 14);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(59, 15);
            label1.TabIndex = 1;
            label1.Text = "Mod URL:";
            // 
            // txtDownloadBZ98R
            // 
            txtDownloadBZ98R.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtDownloadBZ98R.Location = new System.Drawing.Point(79, 10);
            txtDownloadBZ98R.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtDownloadBZ98R.Name = "txtDownloadBZ98R";
            txtDownloadBZ98R.Size = new System.Drawing.Size(657, 23);
            txtDownloadBZ98R.TabIndex = 2;
            // 
            // tpBZCC
            // 
            tpBZCC.Controls.Add(tableLayoutPanel2);
            tpBZCC.Controls.Add(btnHardUpdateBZCC);
            tpBZCC.Controls.Add(btnDependenciesBZ98R);
            tpBZCC.Controls.Add(btnUpdateBZCC);
            tpBZCC.Controls.Add(btnRefreshBZCC);
            tpBZCC.Controls.Add(lvModsBZCC);
            tpBZCC.Controls.Add(btnDownloadBZCC);
            tpBZCC.Controls.Add(label2);
            tpBZCC.Controls.Add(txtDownloadBZCC);
            tpBZCC.Location = new System.Drawing.Point(4, 24);
            tpBZCC.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tpBZCC.Name = "tpBZCC";
            tpBZCC.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tpBZCC.Size = new System.Drawing.Size(858, 455);
            tpBZCC.TabIndex = 1;
            tpBZCC.Text = "BZCC";
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tableLayoutPanel2.ColumnCount = 4;
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            tableLayoutPanel2.Controls.Add(cbBZCCTypeAsset, 0, 0);
            tableLayoutPanel2.Controls.Add(cbBZCCTypeError, 2, 0);
            tableLayoutPanel2.Controls.Add(cbBZCCTypeConfig, 1, 0);
            tableLayoutPanel2.Controls.Add(cbBZCCTypeAddon, 0, 0);
            tableLayoutPanel2.Location = new System.Drawing.Point(7, 40);
            tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new System.Drawing.Size(336, 27);
            tableLayoutPanel2.TabIndex = 12;
            // 
            // cbBZCCTypeAsset
            // 
            cbBZCCTypeAsset.AutoSize = true;
            cbBZCCTypeAsset.Checked = true;
            cbBZCCTypeAsset.CheckState = System.Windows.Forms.CheckState.Checked;
            cbBZCCTypeAsset.Location = new System.Drawing.Point(72, 3);
            cbBZCCTypeAsset.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbBZCCTypeAsset.Name = "cbBZCCTypeAsset";
            cbBZCCTypeAsset.Size = new System.Drawing.Size(52, 19);
            cbBZCCTypeAsset.TabIndex = 6;
            cbBZCCTypeAsset.Text = "asset";
            cbBZCCTypeAsset.UseVisualStyleBackColor = true;
            cbBZCCTypeAsset.CheckStateChanged += cbBZCCType_CheckedChanged;
            // 
            // cbBZCCTypeError
            // 
            cbBZCCTypeError.AutoSize = true;
            cbBZCCTypeError.Checked = true;
            cbBZCCTypeError.CheckState = System.Windows.Forms.CheckState.Checked;
            cbBZCCTypeError.Location = new System.Drawing.Point(200, 3);
            cbBZCCTypeError.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbBZCCTypeError.Name = "cbBZCCTypeError";
            cbBZCCTypeError.Size = new System.Drawing.Size(51, 19);
            cbBZCCTypeError.TabIndex = 8;
            cbBZCCTypeError.Text = "error";
            cbBZCCTypeError.UseVisualStyleBackColor = true;
            cbBZCCTypeError.CheckStateChanged += cbBZCCType_CheckedChanged;
            // 
            // cbBZCCTypeConfig
            // 
            cbBZCCTypeConfig.AutoSize = true;
            cbBZCCTypeConfig.Checked = true;
            cbBZCCTypeConfig.CheckState = System.Windows.Forms.CheckState.Checked;
            cbBZCCTypeConfig.Location = new System.Drawing.Point(132, 3);
            cbBZCCTypeConfig.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbBZCCTypeConfig.Name = "cbBZCCTypeConfig";
            cbBZCCTypeConfig.Size = new System.Drawing.Size(60, 19);
            cbBZCCTypeConfig.TabIndex = 7;
            cbBZCCTypeConfig.Text = "config";
            cbBZCCTypeConfig.UseVisualStyleBackColor = true;
            cbBZCCTypeConfig.CheckStateChanged += cbBZCCType_CheckedChanged;
            // 
            // cbBZCCTypeAddon
            // 
            cbBZCCTypeAddon.AutoSize = true;
            cbBZCCTypeAddon.Checked = true;
            cbBZCCTypeAddon.CheckState = System.Windows.Forms.CheckState.Checked;
            cbBZCCTypeAddon.Location = new System.Drawing.Point(4, 3);
            cbBZCCTypeAddon.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbBZCCTypeAddon.Name = "cbBZCCTypeAddon";
            cbBZCCTypeAddon.Size = new System.Drawing.Size(60, 19);
            cbBZCCTypeAddon.TabIndex = 5;
            cbBZCCTypeAddon.Text = "addon";
            cbBZCCTypeAddon.UseVisualStyleBackColor = true;
            cbBZCCTypeAddon.CheckStateChanged += cbBZCCType_CheckedChanged;
            // 
            // btnHardUpdateBZCC
            // 
            btnHardUpdateBZCC.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnHardUpdateBZCC.Location = new System.Drawing.Point(520, 40);
            btnHardUpdateBZCC.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnHardUpdateBZCC.Name = "btnHardUpdateBZCC";
            btnHardUpdateBZCC.Size = new System.Drawing.Size(105, 27);
            btnHardUpdateBZCC.TabIndex = 10;
            btnHardUpdateBZCC.Text = "Hard Update";
            btnHardUpdateBZCC.UseVisualStyleBackColor = true;
            btnHardUpdateBZCC.Click += btnHardUpdateBZCC_Click;
            // 
            // btnDependenciesBZ98R
            // 
            btnDependenciesBZ98R.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnDependenciesBZ98R.Location = new System.Drawing.Point(350, 40);
            btnDependenciesBZ98R.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnDependenciesBZ98R.Name = "btnDependenciesBZ98R";
            btnDependenciesBZ98R.Size = new System.Drawing.Size(163, 27);
            btnDependenciesBZ98R.TabIndex = 9;
            btnDependenciesBZ98R.Text = "Download Dependencies";
            btnDependenciesBZ98R.UseVisualStyleBackColor = true;
            btnDependenciesBZ98R.Click += btnDependenciesBZ98R_Click;
            // 
            // btnUpdateBZCC
            // 
            btnUpdateBZCC.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnUpdateBZCC.Location = new System.Drawing.Point(632, 40);
            btnUpdateBZCC.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnUpdateBZCC.Name = "btnUpdateBZCC";
            btnUpdateBZCC.Size = new System.Drawing.Size(105, 27);
            btnUpdateBZCC.TabIndex = 11;
            btnUpdateBZCC.Text = "Update Mods";
            btnUpdateBZCC.UseVisualStyleBackColor = true;
            btnUpdateBZCC.Click += btnUpdateBZCC_Click;
            // 
            // btnRefreshBZCC
            // 
            btnRefreshBZCC.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnRefreshBZCC.Location = new System.Drawing.Point(744, 40);
            btnRefreshBZCC.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnRefreshBZCC.Name = "btnRefreshBZCC";
            btnRefreshBZCC.Size = new System.Drawing.Size(105, 27);
            btnRefreshBZCC.TabIndex = 12;
            btnRefreshBZCC.Text = "Refresh List";
            btnRefreshBZCC.UseVisualStyleBackColor = true;
            btnRefreshBZCC.Click += btnRefreshBZCC_Click;
            // 
            // lvModsBZCC
            // 
            lvModsBZCC.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lvModsBZCC.FullRowSelect = true;
            lvModsBZCC.GridLines = true;
            lvModsBZCC.Location = new System.Drawing.Point(7, 74);
            lvModsBZCC.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lvModsBZCC.Name = "lvModsBZCC";
            lvModsBZCC.Size = new System.Drawing.Size(842, 358);
            lvModsBZCC.TabIndex = 13;
            lvModsBZCC.TypeFilter = null;
            lvModsBZCC.UseCompatibleStateImageBehavior = false;
            lvModsBZCC.View = System.Windows.Forms.View.Details;
            lvModsBZCC.VirtualMode = true;
            // 
            // btnDownloadBZCC
            // 
            btnDownloadBZCC.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnDownloadBZCC.Location = new System.Drawing.Point(744, 8);
            btnDownloadBZCC.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnDownloadBZCC.Name = "btnDownloadBZCC";
            btnDownloadBZCC.Size = new System.Drawing.Size(105, 27);
            btnDownloadBZCC.TabIndex = 3;
            btnDownloadBZCC.Text = "Download";
            btnDownloadBZCC.UseVisualStyleBackColor = true;
            btnDownloadBZCC.Click += btnDownloadBZCC_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(7, 14);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(59, 15);
            label2.TabIndex = 1;
            label2.Text = "Mod URL:";
            // 
            // txtDownloadBZCC
            // 
            txtDownloadBZCC.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtDownloadBZCC.Location = new System.Drawing.Point(79, 10);
            txtDownloadBZCC.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtDownloadBZCC.Name = "txtDownloadBZCC";
            txtDownloadBZCC.Size = new System.Drawing.Size(657, 23);
            txtDownloadBZCC.TabIndex = 2;
            // 
            // tpFindMods
            // 
            tpFindMods.Controls.Add(btnDownloadSelectedFoundMods);
            tpFindMods.Controls.Add(cbFindModsNewOnly);
            tpFindMods.Controls.Add(rbFindModsTable);
            tpFindMods.Controls.Add(btnFindMods);
            tpFindMods.Controls.Add(rbFindModsIcon);
            tpFindMods.Controls.Add(tcFindMods);
            tpFindMods.Location = new System.Drawing.Point(4, 24);
            tpFindMods.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tpFindMods.Name = "tpFindMods";
            tpFindMods.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tpFindMods.Size = new System.Drawing.Size(858, 455);
            tpFindMods.TabIndex = 7;
            tpFindMods.Text = "Find Mods";
            // 
            // btnDownloadSelectedFoundMods
            // 
            btnDownloadSelectedFoundMods.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnDownloadSelectedFoundMods.Location = new System.Drawing.Point(578, 3);
            btnDownloadSelectedFoundMods.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnDownloadSelectedFoundMods.Name = "btnDownloadSelectedFoundMods";
            btnDownloadSelectedFoundMods.Size = new System.Drawing.Size(155, 27);
            btnDownloadSelectedFoundMods.TabIndex = 10;
            btnDownloadSelectedFoundMods.Text = "Download Selected";
            btnDownloadSelectedFoundMods.UseVisualStyleBackColor = true;
            btnDownloadSelectedFoundMods.Click += btnDownloadSelectedFoundMods_Click;
            // 
            // cbFindModsNewOnly
            // 
            cbFindModsNewOnly.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            cbFindModsNewOnly.AutoSize = true;
            cbFindModsNewOnly.Checked = true;
            cbFindModsNewOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            cbFindModsNewOnly.Location = new System.Drawing.Point(364, 8);
            cbFindModsNewOnly.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbFindModsNewOnly.Name = "cbFindModsNewOnly";
            cbFindModsNewOnly.Size = new System.Drawing.Size(78, 19);
            cbFindModsNewOnly.TabIndex = 9;
            cbFindModsNewOnly.Text = "New Only";
            cbFindModsNewOnly.UseVisualStyleBackColor = true;
            cbFindModsNewOnly.CheckedChanged += cbFindModsNewOnly_CheckedChanged;
            // 
            // rbFindModsTable
            // 
            rbFindModsTable.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            rbFindModsTable.AutoSize = true;
            rbFindModsTable.Location = new System.Drawing.Point(457, 7);
            rbFindModsTable.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            rbFindModsTable.Name = "rbFindModsTable";
            rbFindModsTable.Size = new System.Drawing.Size(53, 19);
            rbFindModsTable.TabIndex = 2;
            rbFindModsTable.Text = "Table";
            rbFindModsTable.UseVisualStyleBackColor = true;
            rbFindModsTable.CheckedChanged += rbFindMods_CheckedChanged;
            // 
            // btnFindMods
            // 
            btnFindMods.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnFindMods.Location = new System.Drawing.Point(740, 3);
            btnFindMods.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnFindMods.Name = "btnFindMods";
            btnFindMods.Size = new System.Drawing.Size(108, 27);
            btnFindMods.TabIndex = 4;
            btnFindMods.Text = "Find Mods";
            btnFindMods.UseVisualStyleBackColor = true;
            btnFindMods.Click += btnFindMods_Click;
            // 
            // rbFindModsIcon
            // 
            rbFindModsIcon.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            rbFindModsIcon.AutoSize = true;
            rbFindModsIcon.Checked = true;
            rbFindModsIcon.Location = new System.Drawing.Point(522, 7);
            rbFindModsIcon.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            rbFindModsIcon.Name = "rbFindModsIcon";
            rbFindModsIcon.Size = new System.Drawing.Size(48, 19);
            rbFindModsIcon.TabIndex = 3;
            rbFindModsIcon.TabStop = true;
            rbFindModsIcon.Text = "Icon";
            rbFindModsIcon.UseVisualStyleBackColor = true;
            rbFindModsIcon.CheckedChanged += rbFindMods_CheckedChanged;
            // 
            // tcFindMods
            // 
            tcFindMods.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tcFindMods.Controls.Add(tpFindModsBZ98R);
            tcFindMods.Controls.Add(tpFindModsBZCC);
            tcFindMods.Location = new System.Drawing.Point(7, 8);
            tcFindMods.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tcFindMods.Name = "tcFindMods";
            tcFindMods.SelectedIndex = 0;
            tcFindMods.Size = new System.Drawing.Size(842, 425);
            tcFindMods.TabIndex = 1;
            // 
            // tpFindModsBZ98R
            // 
            tpFindModsBZ98R.Controls.Add(lvFindModsBZ98R);
            tpFindModsBZ98R.Location = new System.Drawing.Point(4, 24);
            tpFindModsBZ98R.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tpFindModsBZ98R.Name = "tpFindModsBZ98R";
            tpFindModsBZ98R.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tpFindModsBZ98R.Size = new System.Drawing.Size(834, 397);
            tpFindModsBZ98R.TabIndex = 0;
            tpFindModsBZ98R.Text = "BZ98R";
            // 
            // lvFindModsBZ98R
            // 
            lvFindModsBZ98R.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lvFindModsBZ98R.FullRowSelect = true;
            lvFindModsBZ98R.GridLines = true;
            lvFindModsBZ98R.Location = new System.Drawing.Point(4, 3);
            lvFindModsBZ98R.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lvFindModsBZ98R.Name = "lvFindModsBZ98R";
            lvFindModsBZ98R.Size = new System.Drawing.Size(825, 385);
            lvFindModsBZ98R.TabIndex = 0;
            lvFindModsBZ98R.TypeFilter = null;
            lvFindModsBZ98R.UseCompatibleStateImageBehavior = false;
            lvFindModsBZ98R.VirtualMode = true;
            // 
            // tpFindModsBZCC
            // 
            tpFindModsBZCC.Controls.Add(lvFindModsBZCC);
            tpFindModsBZCC.Location = new System.Drawing.Point(4, 24);
            tpFindModsBZCC.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tpFindModsBZCC.Name = "tpFindModsBZCC";
            tpFindModsBZCC.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tpFindModsBZCC.Size = new System.Drawing.Size(834, 397);
            tpFindModsBZCC.TabIndex = 1;
            tpFindModsBZCC.Text = "BZCC";
            // 
            // lvFindModsBZCC
            // 
            lvFindModsBZCC.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lvFindModsBZCC.FullRowSelect = true;
            lvFindModsBZCC.GridLines = true;
            lvFindModsBZCC.Location = new System.Drawing.Point(4, 3);
            lvFindModsBZCC.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lvFindModsBZCC.Name = "lvFindModsBZCC";
            lvFindModsBZCC.Size = new System.Drawing.Size(825, 385);
            lvFindModsBZCC.TabIndex = 1;
            lvFindModsBZCC.TypeFilter = null;
            lvFindModsBZCC.UseCompatibleStateImageBehavior = false;
            lvFindModsBZCC.VirtualMode = true;
            // 
            // tabMultiplayer
            // 
            tabMultiplayer.Controls.Add(lvPlayers);
            tabMultiplayer.Controls.Add(rbFindGamesTable);
            tabMultiplayer.Controls.Add(btnGetModSteamCmd);
            tabMultiplayer.Controls.Add(btnMultiGetModSteam);
            tabMultiplayer.Controls.Add(rbFindGamesMap);
            tabMultiplayer.Controls.Add(btnMultiJoinGOG);
            tabMultiplayer.Controls.Add(btnMultiJoinSteam);
            tabMultiplayer.Controls.Add(btnMultiRefresh);
            tabMultiplayer.Controls.Add(tcMultiplayer);
            tabMultiplayer.Location = new System.Drawing.Point(4, 24);
            tabMultiplayer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabMultiplayer.Name = "tabMultiplayer";
            tabMultiplayer.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabMultiplayer.Size = new System.Drawing.Size(858, 455);
            tabMultiplayer.TabIndex = 8;
            tabMultiplayer.Text = "Multiplayer";
            // 
            // lvPlayers
            // 
            lvPlayers.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lvPlayers.Location = new System.Drawing.Point(7, 321);
            lvPlayers.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lvPlayers.MultiSelect = false;
            lvPlayers.Name = "lvPlayers";
            lvPlayers.Size = new System.Drawing.Size(840, 111);
            lvPlayers.TabIndex = 13;
            lvPlayers.UseCompatibleStateImageBehavior = false;
            lvPlayers.VirtualMode = true;
            lvPlayers.DoubleClick += lvPlayers_DoubleClick;
            // 
            // rbFindGamesTable
            // 
            rbFindGamesTable.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            rbFindGamesTable.AutoSize = true;
            rbFindGamesTable.Checked = true;
            rbFindGamesTable.Location = new System.Drawing.Point(176, 7);
            rbFindGamesTable.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            rbFindGamesTable.Name = "rbFindGamesTable";
            rbFindGamesTable.Size = new System.Drawing.Size(53, 19);
            rbFindGamesTable.TabIndex = 11;
            rbFindGamesTable.TabStop = true;
            rbFindGamesTable.Text = "Table";
            rbFindGamesTable.UseVisualStyleBackColor = true;
            rbFindGamesTable.CheckedChanged += rbFindGames_CheckedChanged;
            // 
            // btnGetModSteamCmd
            // 
            btnGetModSteamCmd.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnGetModSteamCmd.Enabled = false;
            btnGetModSteamCmd.Location = new System.Drawing.Point(296, 3);
            btnGetModSteamCmd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnGetModSteamCmd.Name = "btnGetModSteamCmd";
            btnGetModSteamCmd.Size = new System.Drawing.Size(153, 27);
            btnGetModSteamCmd.TabIndex = 10;
            btnGetModSteamCmd.Text = "Get Mods (SteamCmd)";
            btnGetModSteamCmd.UseVisualStyleBackColor = true;
            btnGetModSteamCmd.Click += btnGetModSteamCmd_Click;
            // 
            // btnMultiGetModSteam
            // 
            btnMultiGetModSteam.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnMultiGetModSteam.Enabled = false;
            btnMultiGetModSteam.Location = new System.Drawing.Point(456, 3);
            btnMultiGetModSteam.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnMultiGetModSteam.Name = "btnMultiGetModSteam";
            btnMultiGetModSteam.Size = new System.Drawing.Size(119, 27);
            btnMultiGetModSteam.TabIndex = 9;
            btnMultiGetModSteam.Text = "Get Mod (Steam)";
            btnMultiGetModSteam.UseVisualStyleBackColor = true;
            btnMultiGetModSteam.Click += btnMultiGetModSteam_Click;
            // 
            // rbFindGamesMap
            // 
            rbFindGamesMap.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            rbFindGamesMap.AutoSize = true;
            rbFindGamesMap.Location = new System.Drawing.Point(240, 7);
            rbFindGamesMap.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            rbFindGamesMap.Name = "rbFindGamesMap";
            rbFindGamesMap.Size = new System.Drawing.Size(49, 19);
            rbFindGamesMap.TabIndex = 12;
            rbFindGamesMap.Text = "Map";
            rbFindGamesMap.UseVisualStyleBackColor = true;
            rbFindGamesMap.CheckedChanged += rbFindGames_CheckedChanged;
            // 
            // btnMultiJoinGOG
            // 
            btnMultiJoinGOG.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnMultiJoinGOG.Enabled = false;
            btnMultiJoinGOG.Location = new System.Drawing.Point(582, 3);
            btnMultiJoinGOG.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnMultiJoinGOG.Name = "btnMultiJoinGOG";
            btnMultiJoinGOG.Size = new System.Drawing.Size(89, 27);
            btnMultiJoinGOG.TabIndex = 8;
            btnMultiJoinGOG.Text = "Join (GOG)";
            btnMultiJoinGOG.UseVisualStyleBackColor = true;
            btnMultiJoinGOG.Click += btnMultiJoinGOG_Click;
            // 
            // btnMultiJoinSteam
            // 
            btnMultiJoinSteam.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnMultiJoinSteam.Enabled = false;
            btnMultiJoinSteam.Location = new System.Drawing.Point(678, 3);
            btnMultiJoinSteam.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnMultiJoinSteam.Name = "btnMultiJoinSteam";
            btnMultiJoinSteam.Size = new System.Drawing.Size(89, 27);
            btnMultiJoinSteam.TabIndex = 7;
            btnMultiJoinSteam.Text = "Join (Steam)";
            btnMultiJoinSteam.UseVisualStyleBackColor = true;
            btnMultiJoinSteam.Click += btnMultiJoinSteam_Click;
            // 
            // btnMultiRefresh
            // 
            btnMultiRefresh.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnMultiRefresh.Location = new System.Drawing.Point(774, 3);
            btnMultiRefresh.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnMultiRefresh.Name = "btnMultiRefresh";
            btnMultiRefresh.Size = new System.Drawing.Size(75, 27);
            btnMultiRefresh.TabIndex = 6;
            btnMultiRefresh.Text = "Refresh";
            btnMultiRefresh.UseVisualStyleBackColor = true;
            btnMultiRefresh.Click += btnMultiRefresh_Click;
            // 
            // tcMultiplayer
            // 
            tcMultiplayer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tcMultiplayer.Controls.Add(tpMultiplayerBZ98R);
            tcMultiplayer.Controls.Add(tpMultiplayerBZCC);
            tcMultiplayer.Location = new System.Drawing.Point(7, 8);
            tcMultiplayer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tcMultiplayer.Name = "tcMultiplayer";
            tcMultiplayer.SelectedIndex = 0;
            tcMultiplayer.Size = new System.Drawing.Size(842, 306);
            tcMultiplayer.TabIndex = 5;
            tcMultiplayer.SelectedIndexChanged += tcMultiplayer_SelectedIndexChanged;
            // 
            // tpMultiplayerBZ98R
            // 
            tpMultiplayerBZ98R.Controls.Add(lvMultiplayerBZ98R);
            tpMultiplayerBZ98R.Location = new System.Drawing.Point(4, 24);
            tpMultiplayerBZ98R.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tpMultiplayerBZ98R.Name = "tpMultiplayerBZ98R";
            tpMultiplayerBZ98R.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tpMultiplayerBZ98R.Size = new System.Drawing.Size(834, 278);
            tpMultiplayerBZ98R.TabIndex = 0;
            tpMultiplayerBZ98R.Text = "BZ98R";
            // 
            // lvMultiplayerBZ98R
            // 
            lvMultiplayerBZ98R.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lvMultiplayerBZ98R.FullRowSelect = true;
            lvMultiplayerBZ98R.GridLines = true;
            lvMultiplayerBZ98R.Location = new System.Drawing.Point(4, 3);
            lvMultiplayerBZ98R.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lvMultiplayerBZ98R.MultiSelect = false;
            lvMultiplayerBZ98R.Name = "lvMultiplayerBZ98R";
            lvMultiplayerBZ98R.Size = new System.Drawing.Size(825, 266);
            lvMultiplayerBZ98R.TabIndex = 0;
            lvMultiplayerBZ98R.UseCompatibleStateImageBehavior = false;
            lvMultiplayerBZ98R.View = System.Windows.Forms.View.Details;
            lvMultiplayerBZ98R.VirtualMode = true;
            lvMultiplayerBZ98R.SelectedIndexChanged += lvMultiplayerBZ98R_SelectedIndexChanged;
            // 
            // tpMultiplayerBZCC
            // 
            tpMultiplayerBZCC.Controls.Add(lvMultiplayerBZCC);
            tpMultiplayerBZCC.Location = new System.Drawing.Point(4, 24);
            tpMultiplayerBZCC.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tpMultiplayerBZCC.Name = "tpMultiplayerBZCC";
            tpMultiplayerBZCC.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tpMultiplayerBZCC.Size = new System.Drawing.Size(834, 278);
            tpMultiplayerBZCC.TabIndex = 1;
            tpMultiplayerBZCC.Text = "BZCC";
            // 
            // lvMultiplayerBZCC
            // 
            lvMultiplayerBZCC.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lvMultiplayerBZCC.FullRowSelect = true;
            lvMultiplayerBZCC.GridLines = true;
            lvMultiplayerBZCC.Location = new System.Drawing.Point(4, 3);
            lvMultiplayerBZCC.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lvMultiplayerBZCC.MultiSelect = false;
            lvMultiplayerBZCC.Name = "lvMultiplayerBZCC";
            lvMultiplayerBZCC.Size = new System.Drawing.Size(825, 266);
            lvMultiplayerBZCC.TabIndex = 1;
            lvMultiplayerBZCC.UseCompatibleStateImageBehavior = false;
            lvMultiplayerBZCC.View = System.Windows.Forms.View.Details;
            lvMultiplayerBZCC.VirtualMode = true;
            lvMultiplayerBZCC.SelectedIndexChanged += lvMultiplayerBZCC_SelectedIndexChanged;
            // 
            // tpAudit
            // 
            tpAudit.Controls.Add(txtAuditLog);
            tpAudit.Controls.Add(btnRunAudit);
            tpAudit.Location = new System.Drawing.Point(4, 24);
            tpAudit.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tpAudit.Name = "tpAudit";
            tpAudit.Size = new System.Drawing.Size(858, 455);
            tpAudit.TabIndex = 10;
            tpAudit.Text = "Audit";
            // 
            // txtAuditLog
            // 
            txtAuditLog.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtAuditLog.Font = new System.Drawing.Font("Consolas", 8.25F);
            txtAuditLog.Location = new System.Drawing.Point(4, 37);
            txtAuditLog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtAuditLog.Name = "txtAuditLog";
            txtAuditLog.ReadOnly = true;
            txtAuditLog.Size = new System.Drawing.Size(849, 399);
            txtAuditLog.TabIndex = 2;
            txtAuditLog.Text = "";
            txtAuditLog.LinkClicked += txtAuditLog_LinkClicked;
            // 
            // btnRunAudit
            // 
            btnRunAudit.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            btnRunAudit.Location = new System.Drawing.Point(4, 3);
            btnRunAudit.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnRunAudit.Name = "btnRunAudit";
            btnRunAudit.Size = new System.Drawing.Size(849, 27);
            btnRunAudit.TabIndex = 0;
            btnRunAudit.Text = "Run Audit";
            btnRunAudit.UseVisualStyleBackColor = true;
            btnRunAudit.Click += btnRunAudit_Click;
            // 
            // tpSettings
            // 
            tpSettings.Controls.Add(groupBox6);
            tpSettings.Controls.Add(groupBox5);
            tpSettings.Controls.Add(btnFixSteamCmd);
            tpSettings.Controls.Add(cbFallbackSteamCmdWindowHandling);
            tpSettings.Controls.Add(groupBox4);
            tpSettings.Controls.Add(groupBox3);
            tpSettings.Controls.Add(groupBox2);
            tpSettings.Controls.Add(groupBox1);
            tpSettings.Location = new System.Drawing.Point(4, 24);
            tpSettings.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tpSettings.Name = "tpSettings";
            tpSettings.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tpSettings.Size = new System.Drawing.Size(858, 455);
            tpSettings.TabIndex = 2;
            tpSettings.Text = "Settings";
            // 
            // groupBox6
            // 
            groupBox6.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBox6.Controls.Add(btnGitFind);
            groupBox6.Controls.Add(txtGit);
            groupBox6.Controls.Add(btnGitApply);
            groupBox6.Location = new System.Drawing.Point(7, 330);
            groupBox6.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox6.Name = "groupBox6";
            groupBox6.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox6.Size = new System.Drawing.Size(844, 58);
            groupBox6.TabIndex = 20;
            groupBox6.TabStop = false;
            groupBox6.Text = "git.exe location";
            // 
            // btnGitFind
            // 
            btnGitFind.Location = new System.Drawing.Point(7, 22);
            btnGitFind.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnGitFind.Name = "btnGitFind";
            btnGitFind.Size = new System.Drawing.Size(86, 27);
            btnGitFind.TabIndex = 10;
            btnGitFind.Text = "Quick Find";
            btnGitFind.UseVisualStyleBackColor = true;
            btnGitFind.Click += btnGitFind_Click;
            // 
            // txtGit
            // 
            txtGit.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtGit.Location = new System.Drawing.Point(100, 24);
            txtGit.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtGit.Name = "txtGit";
            txtGit.Size = new System.Drawing.Size(663, 23);
            txtGit.TabIndex = 11;
            // 
            // btnGitApply
            // 
            btnGitApply.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnGitApply.Location = new System.Drawing.Point(771, 22);
            btnGitApply.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnGitApply.Name = "btnGitApply";
            btnGitApply.Size = new System.Drawing.Size(66, 27);
            btnGitApply.TabIndex = 12;
            btnGitApply.Text = "Apply";
            btnGitApply.UseVisualStyleBackColor = true;
            btnGitApply.Click += btnGitApply_Click;
            // 
            // groupBox5
            // 
            groupBox5.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBox5.Controls.Add(btnBZCCGogFind);
            groupBox5.Controls.Add(txtBZCCGog);
            groupBox5.Controls.Add(btnBZCCRGogApply);
            groupBox5.Location = new System.Drawing.Point(7, 265);
            groupBox5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox5.Name = "groupBox5";
            groupBox5.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox5.Size = new System.Drawing.Size(844, 58);
            groupBox5.TabIndex = 19;
            groupBox5.TabStop = false;
            groupBox5.Text = "GOG install of BZCC (only needed for multiplayer quicklaunch)";
            // 
            // btnBZCCGogFind
            // 
            btnBZCCGogFind.Location = new System.Drawing.Point(7, 22);
            btnBZCCGogFind.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnBZCCGogFind.Name = "btnBZCCGogFind";
            btnBZCCGogFind.Size = new System.Drawing.Size(86, 27);
            btnBZCCGogFind.TabIndex = 10;
            btnBZCCGogFind.Text = "Quick Find";
            btnBZCCGogFind.UseVisualStyleBackColor = true;
            btnBZCCGogFind.Click += btnBZCCGogFind_Click;
            // 
            // txtBZCCGog
            // 
            txtBZCCGog.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtBZCCGog.Location = new System.Drawing.Point(100, 24);
            txtBZCCGog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtBZCCGog.Name = "txtBZCCGog";
            txtBZCCGog.Size = new System.Drawing.Size(663, 23);
            txtBZCCGog.TabIndex = 11;
            // 
            // btnBZCCRGogApply
            // 
            btnBZCCRGogApply.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnBZCCRGogApply.Location = new System.Drawing.Point(771, 22);
            btnBZCCRGogApply.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnBZCCRGogApply.Name = "btnBZCCRGogApply";
            btnBZCCRGogApply.Size = new System.Drawing.Size(66, 27);
            btnBZCCRGogApply.TabIndex = 12;
            btnBZCCRGogApply.Text = "Apply";
            btnBZCCRGogApply.UseVisualStyleBackColor = true;
            btnBZCCRGogApply.Click += btnBZCCRGogApply_Click;
            // 
            // btnFixSteamCmd
            // 
            btnFixSteamCmd.Location = new System.Drawing.Point(7, 395);
            btnFixSteamCmd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnFixSteamCmd.Name = "btnFixSteamCmd";
            btnFixSteamCmd.Size = new System.Drawing.Size(209, 27);
            btnFixSteamCmd.TabIndex = 18;
            btnFixSteamCmd.Text = "Delete and Rebuild SteamCmd";
            btnFixSteamCmd.UseVisualStyleBackColor = true;
            btnFixSteamCmd.Click += btnFixSteamCmd_Click;
            // 
            // cbFallbackSteamCmdWindowHandling
            // 
            cbFallbackSteamCmdWindowHandling.AutoSize = true;
            cbFallbackSteamCmdWindowHandling.Location = new System.Drawing.Point(7, 428);
            cbFallbackSteamCmdWindowHandling.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbFallbackSteamCmdWindowHandling.Name = "cbFallbackSteamCmdWindowHandling";
            cbFallbackSteamCmdWindowHandling.Size = new System.Drawing.Size(230, 19);
            cbFallbackSteamCmdWindowHandling.TabIndex = 17;
            cbFallbackSteamCmdWindowHandling.Text = "Fallback SteamCmd Window Handling";
            cbFallbackSteamCmdWindowHandling.UseVisualStyleBackColor = true;
            cbFallbackSteamCmdWindowHandling.Visible = false;
            cbFallbackSteamCmdWindowHandling.CheckedChanged += cbFallbackSteamCmdWindowHandling_CheckedChanged;
            // 
            // groupBox4
            // 
            groupBox4.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBox4.Controls.Add(btnBZCCMyDocsFind);
            groupBox4.Controls.Add(txtBZCCMyDocs);
            groupBox4.Controls.Add(btnBZCCMyDocsApply);
            groupBox4.Location = new System.Drawing.Point(7, 201);
            groupBox4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox4.Name = "groupBox4";
            groupBox4.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox4.Size = new System.Drawing.Size(844, 58);
            groupBox4.TabIndex = 13;
            groupBox4.TabStop = false;
            groupBox4.Text = "BZCC Folder in My Docs/My Games";
            // 
            // btnBZCCMyDocsFind
            // 
            btnBZCCMyDocsFind.Location = new System.Drawing.Point(7, 22);
            btnBZCCMyDocsFind.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnBZCCMyDocsFind.Name = "btnBZCCMyDocsFind";
            btnBZCCMyDocsFind.Size = new System.Drawing.Size(86, 27);
            btnBZCCMyDocsFind.TabIndex = 14;
            btnBZCCMyDocsFind.Text = "Quick Find";
            btnBZCCMyDocsFind.UseVisualStyleBackColor = true;
            btnBZCCMyDocsFind.Click += btnBZCCMyDocsFind_Click;
            // 
            // txtBZCCMyDocs
            // 
            txtBZCCMyDocs.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtBZCCMyDocs.Location = new System.Drawing.Point(100, 24);
            txtBZCCMyDocs.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtBZCCMyDocs.Name = "txtBZCCMyDocs";
            txtBZCCMyDocs.Size = new System.Drawing.Size(663, 23);
            txtBZCCMyDocs.TabIndex = 15;
            // 
            // btnBZCCMyDocsApply
            // 
            btnBZCCMyDocsApply.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnBZCCMyDocsApply.Location = new System.Drawing.Point(771, 22);
            btnBZCCMyDocsApply.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnBZCCMyDocsApply.Name = "btnBZCCMyDocsApply";
            btnBZCCMyDocsApply.Size = new System.Drawing.Size(66, 27);
            btnBZCCMyDocsApply.TabIndex = 16;
            btnBZCCMyDocsApply.Text = "Apply";
            btnBZCCMyDocsApply.UseVisualStyleBackColor = true;
            btnBZCCMyDocsApply.Click += btnBZCCMyDocsApply_Click;
            // 
            // groupBox3
            // 
            groupBox3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBox3.Controls.Add(btnBZ98RGogFind);
            groupBox3.Controls.Add(txtBZ98RGog);
            groupBox3.Controls.Add(btnBZ98RGogApply);
            groupBox3.Location = new System.Drawing.Point(7, 136);
            groupBox3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox3.Size = new System.Drawing.Size(844, 58);
            groupBox3.TabIndex = 9;
            groupBox3.TabStop = false;
            groupBox3.Text = "GOG install of BZ98R";
            // 
            // btnBZ98RGogFind
            // 
            btnBZ98RGogFind.Location = new System.Drawing.Point(7, 22);
            btnBZ98RGogFind.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnBZ98RGogFind.Name = "btnBZ98RGogFind";
            btnBZ98RGogFind.Size = new System.Drawing.Size(86, 27);
            btnBZ98RGogFind.TabIndex = 10;
            btnBZ98RGogFind.Text = "Quick Find";
            btnBZ98RGogFind.UseVisualStyleBackColor = true;
            btnBZ98RGogFind.Click += btnBZ98RGogFind_Click;
            // 
            // txtBZ98RGog
            // 
            txtBZ98RGog.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtBZ98RGog.Location = new System.Drawing.Point(100, 24);
            txtBZ98RGog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtBZ98RGog.Name = "txtBZ98RGog";
            txtBZ98RGog.Size = new System.Drawing.Size(663, 23);
            txtBZ98RGog.TabIndex = 11;
            // 
            // btnBZ98RGogApply
            // 
            btnBZ98RGogApply.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnBZ98RGogApply.Location = new System.Drawing.Point(771, 22);
            btnBZ98RGogApply.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnBZ98RGogApply.Name = "btnBZ98RGogApply";
            btnBZ98RGogApply.Size = new System.Drawing.Size(66, 27);
            btnBZ98RGogApply.TabIndex = 12;
            btnBZ98RGogApply.Text = "Apply";
            btnBZ98RGogApply.UseVisualStyleBackColor = true;
            btnBZ98RGogApply.Click += txtBZ98RGogApply_Click;
            // 
            // groupBox2
            // 
            groupBox2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBox2.Controls.Add(btnBZCCSteamFind);
            groupBox2.Controls.Add(txtBZCCSteam);
            groupBox2.Controls.Add(btnBZCCSteamApply);
            groupBox2.Location = new System.Drawing.Point(7, 72);
            groupBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox2.Size = new System.Drawing.Size(844, 58);
            groupBox2.TabIndex = 5;
            groupBox2.TabStop = false;
            groupBox2.Text = "steamapps folder that contains BZCC";
            // 
            // btnBZCCSteamFind
            // 
            btnBZCCSteamFind.Location = new System.Drawing.Point(7, 22);
            btnBZCCSteamFind.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnBZCCSteamFind.Name = "btnBZCCSteamFind";
            btnBZCCSteamFind.Size = new System.Drawing.Size(86, 27);
            btnBZCCSteamFind.TabIndex = 6;
            btnBZCCSteamFind.Text = "Quick Find";
            btnBZCCSteamFind.UseVisualStyleBackColor = true;
            btnBZCCSteamFind.Click += btnBZCCSteamFind_Click;
            // 
            // txtBZCCSteam
            // 
            txtBZCCSteam.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtBZCCSteam.Location = new System.Drawing.Point(100, 24);
            txtBZCCSteam.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtBZCCSteam.Name = "txtBZCCSteam";
            txtBZCCSteam.Size = new System.Drawing.Size(663, 23);
            txtBZCCSteam.TabIndex = 7;
            // 
            // btnBZCCSteamApply
            // 
            btnBZCCSteamApply.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnBZCCSteamApply.Location = new System.Drawing.Point(771, 22);
            btnBZCCSteamApply.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnBZCCSteamApply.Name = "btnBZCCSteamApply";
            btnBZCCSteamApply.Size = new System.Drawing.Size(66, 27);
            btnBZCCSteamApply.TabIndex = 8;
            btnBZCCSteamApply.Text = "Apply";
            btnBZCCSteamApply.UseVisualStyleBackColor = true;
            btnBZCCSteamApply.Click += btnBZCCSteamApply_Click;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBox1.Controls.Add(btnBZ98RSteamFind);
            groupBox1.Controls.Add(txtBZ98RSteam);
            groupBox1.Controls.Add(btnBZ98RSteamApply);
            groupBox1.Location = new System.Drawing.Point(7, 7);
            groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Size = new System.Drawing.Size(844, 58);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "steamapps folder that contains BZ98R";
            // 
            // btnBZ98RSteamFind
            // 
            btnBZ98RSteamFind.Location = new System.Drawing.Point(7, 22);
            btnBZ98RSteamFind.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnBZ98RSteamFind.Name = "btnBZ98RSteamFind";
            btnBZ98RSteamFind.Size = new System.Drawing.Size(86, 27);
            btnBZ98RSteamFind.TabIndex = 2;
            btnBZ98RSteamFind.Text = "Quick Find";
            btnBZ98RSteamFind.UseVisualStyleBackColor = true;
            btnBZ98RSteamFind.Click += btnBZ98RSteamFind_Click;
            // 
            // txtBZ98RSteam
            // 
            txtBZ98RSteam.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtBZ98RSteam.Location = new System.Drawing.Point(100, 24);
            txtBZ98RSteam.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtBZ98RSteam.Name = "txtBZ98RSteam";
            txtBZ98RSteam.Size = new System.Drawing.Size(663, 23);
            txtBZ98RSteam.TabIndex = 3;
            // 
            // btnBZ98RSteamApply
            // 
            btnBZ98RSteamApply.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnBZ98RSteamApply.Location = new System.Drawing.Point(771, 22);
            btnBZ98RSteamApply.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnBZ98RSteamApply.Name = "btnBZ98RSteamApply";
            btnBZ98RSteamApply.Size = new System.Drawing.Size(66, 27);
            btnBZ98RSteamApply.TabIndex = 4;
            btnBZ98RSteamApply.Text = "Apply";
            btnBZ98RSteamApply.UseVisualStyleBackColor = true;
            btnBZ98RSteamApply.Click += btnBZ98RSteamApply_Click;
            // 
            // tpTasks
            // 
            tpTasks.Controls.Add(pnlTasks);
            tpTasks.Location = new System.Drawing.Point(4, 24);
            tpTasks.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tpTasks.Name = "tpTasks";
            tpTasks.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tpTasks.Size = new System.Drawing.Size(858, 455);
            tpTasks.TabIndex = 6;
            tpTasks.Text = "Tasks";
            // 
            // pnlTasks
            // 
            pnlTasks.AutoScroll = true;
            pnlTasks.AutoSize = true;
            pnlTasks.ColumnCount = 1;
            pnlTasks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            pnlTasks.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlTasks.Location = new System.Drawing.Point(4, 3);
            pnlTasks.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pnlTasks.Name = "pnlTasks";
            pnlTasks.RowCount = 1;
            pnlTasks.RowStyles.Add(new System.Windows.Forms.RowStyle());
            pnlTasks.Size = new System.Drawing.Size(850, 449);
            pnlTasks.TabIndex = 0;
            pnlTasks.Resize += pnlTasks_Resize;
            // 
            // tpLog
            // 
            tpLog.Controls.Add(txtLog);
            tpLog.Location = new System.Drawing.Point(4, 24);
            tpLog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tpLog.Name = "tpLog";
            tpLog.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tpLog.Size = new System.Drawing.Size(858, 455);
            tpLog.TabIndex = 3;
            tpLog.Text = "Log";
            // 
            // txtLog
            // 
            txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            txtLog.Location = new System.Drawing.Point(4, 3);
            txtLog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            txtLog.Size = new System.Drawing.Size(850, 449);
            txtLog.TabIndex = 0;
            // 
            // tpLogSteamCmd
            // 
            tpLogSteamCmd.Controls.Add(txtLogSteamCmd);
            tpLogSteamCmd.Location = new System.Drawing.Point(4, 24);
            tpLogSteamCmd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tpLogSteamCmd.Name = "tpLogSteamCmd";
            tpLogSteamCmd.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tpLogSteamCmd.Size = new System.Drawing.Size(858, 455);
            tpLogSteamCmd.TabIndex = 4;
            tpLogSteamCmd.Text = "SteamCmd";
            // 
            // txtLogSteamCmd
            // 
            txtLogSteamCmd.BackColor = System.Drawing.Color.Black;
            txtLogSteamCmd.Dock = System.Windows.Forms.DockStyle.Fill;
            txtLogSteamCmd.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            txtLogSteamCmd.ForeColor = System.Drawing.Color.White;
            txtLogSteamCmd.Location = new System.Drawing.Point(4, 3);
            txtLogSteamCmd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtLogSteamCmd.Name = "txtLogSteamCmd";
            txtLogSteamCmd.ReadOnly = true;
            txtLogSteamCmd.Size = new System.Drawing.Size(850, 449);
            txtLogSteamCmd.TabIndex = 1;
            txtLogSteamCmd.Text = "";
            // 
            // tpLogSteamCmdFull
            // 
            tpLogSteamCmdFull.Controls.Add(txtLogSteamCmdFull);
            tpLogSteamCmdFull.Location = new System.Drawing.Point(4, 24);
            tpLogSteamCmdFull.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tpLogSteamCmdFull.Name = "tpLogSteamCmdFull";
            tpLogSteamCmdFull.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tpLogSteamCmdFull.Size = new System.Drawing.Size(858, 455);
            tpLogSteamCmdFull.TabIndex = 5;
            tpLogSteamCmdFull.Text = "SteamCmd Raw";
            // 
            // txtLogSteamCmdFull
            // 
            txtLogSteamCmdFull.BackColor = System.Drawing.Color.Black;
            txtLogSteamCmdFull.Dock = System.Windows.Forms.DockStyle.Fill;
            txtLogSteamCmdFull.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            txtLogSteamCmdFull.ForeColor = System.Drawing.Color.White;
            txtLogSteamCmdFull.Location = new System.Drawing.Point(4, 3);
            txtLogSteamCmdFull.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtLogSteamCmdFull.Name = "txtLogSteamCmdFull";
            txtLogSteamCmdFull.ReadOnly = true;
            txtLogSteamCmdFull.Size = new System.Drawing.Size(850, 449);
            txtLogSteamCmdFull.TabIndex = 2;
            txtLogSteamCmdFull.Text = "";
            // 
            // tpAbout
            // 
            tpAbout.BackColor = System.Drawing.Color.FromArgb(37, 37, 37);
            tpAbout.Controls.Add(label3);
            tpAbout.Controls.Add(btnGithub);
            tpAbout.Controls.Add(btnDiscord);
            tpAbout.Controls.Add(btnSteamAward);
            tpAbout.Controls.Add(logoPictureBox);
            tpAbout.Location = new System.Drawing.Point(4, 24);
            tpAbout.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tpAbout.Name = "tpAbout";
            tpAbout.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tpAbout.Size = new System.Drawing.Size(858, 455);
            tpAbout.TabIndex = 9;
            tpAbout.Text = "About / Support";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label3.ForeColor = System.Drawing.Color.White;
            label3.Location = new System.Drawing.Point(7, 158);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(127, 34);
            label3.TabIndex = 41;
            label3.Text = "Created By\r\nJohn \"Nielk1\" Klein";
            label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnGithub
            // 
            btnGithub.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            btnGithub.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnGithub.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(54, 54, 54);
            btnGithub.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnGithub.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            btnGithub.ForeColor = System.Drawing.Color.White;
            btnGithub.Image = Properties.Resources.github_icon;
            btnGithub.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnGithub.Location = new System.Drawing.Point(163, 295);
            btnGithub.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnGithub.Name = "btnGithub";
            btnGithub.Size = new System.Drawing.Size(686, 137);
            btnGithub.TabIndex = 40;
            btnGithub.Text = "Source Code";
            btnGithub.Click += btnGithub_Click;
            // 
            // btnDiscord
            // 
            btnDiscord.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            btnDiscord.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnDiscord.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(54, 54, 54);
            btnDiscord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnDiscord.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            btnDiscord.ForeColor = System.Drawing.Color.FromArgb(140, 158, 255);
            btnDiscord.Image = Properties.Resources.discord_icon;
            btnDiscord.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnDiscord.Location = new System.Drawing.Point(163, 151);
            btnDiscord.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnDiscord.Name = "btnDiscord";
            btnDiscord.Size = new System.Drawing.Size(686, 137);
            btnDiscord.TabIndex = 39;
            btnDiscord.Text = "Community Discord";
            btnDiscord.Click += btnDiscord_Click;
            // 
            // btnSteamAward
            // 
            btnSteamAward.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            btnSteamAward.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnSteamAward.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(54, 54, 54);
            btnSteamAward.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnSteamAward.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            btnSteamAward.ForeColor = System.Drawing.Color.FromArgb(255, 200, 61);
            btnSteamAward.Image = Properties.Resources.award_icon;
            btnSteamAward.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnSteamAward.Location = new System.Drawing.Point(163, 7);
            btnSteamAward.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnSteamAward.Name = "btnSteamAward";
            btnSteamAward.Size = new System.Drawing.Size(686, 137);
            btnSteamAward.TabIndex = 38;
            btnSteamAward.Text = " Give a Steam Award";
            btnSteamAward.Click += btnSteamAward_Click;
            // 
            // logoPictureBox
            // 
            logoPictureBox.Image = Properties.Resources.nielk1_eyes_128;
            logoPictureBox.Location = new System.Drawing.Point(7, 7);
            logoPictureBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            logoPictureBox.Name = "logoPictureBox";
            logoPictureBox.Size = new System.Drawing.Size(149, 148);
            logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            logoPictureBox.TabIndex = 37;
            logoPictureBox.TabStop = false;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripStatusLabel1, tsslSteamCmd, toolStripStatusLabel5, toolStripStatusLabel3, tsslActiveTasks, toolStripStatusLabel2 });
            statusStrip1.Location = new System.Drawing.Point(0, 504);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            statusStrip1.Size = new System.Drawing.Size(894, 22);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.AutoSize = false;
            toolStripStatusLabel1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new System.Drawing.Size(67, 17);
            toolStripStatusLabel1.Text = "SteamCmd";
            toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tsslSteamCmd
            // 
            tsslSteamCmd.Name = "tsslSteamCmd";
            tsslSteamCmd.Size = new System.Drawing.Size(24, 17);
            tsslSteamCmd.Text = "Off";
            tsslSteamCmd.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel5
            // 
            toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            toolStripStatusLabel5.Size = new System.Drawing.Size(353, 17);
            toolStripStatusLabel5.Spring = true;
            toolStripStatusLabel5.Text = "-";
            // 
            // toolStripStatusLabel3
            // 
            toolStripStatusLabel3.AutoSize = false;
            toolStripStatusLabel3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            toolStripStatusLabel3.Size = new System.Drawing.Size(67, 17);
            toolStripStatusLabel3.Text = "Busy Tasks";
            toolStripStatusLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tsslActiveTasks
            // 
            tsslActiveTasks.Name = "tsslActiveTasks";
            tsslActiveTasks.Size = new System.Drawing.Size(13, 17);
            tsslActiveTasks.Text = "0";
            tsslActiveTasks.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel2
            // 
            toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            toolStripStatusLabel2.Size = new System.Drawing.Size(353, 17);
            toolStripStatusLabel2.Spring = true;
            toolStripStatusLabel2.Text = "-";
            // 
            // ofdGOGBZCCASM
            // 
            ofdGOGBZCCASM.FileName = "battlezone2.exe";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(894, 526);
            Controls.Add(statusStrip1);
            Controls.Add(tabControl1);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MinimumSize = new System.Drawing.Size(910, 551);
            Name = "MainForm";
            Text = "Battlezone Redux Mod Manager";
            Load += MainForm_Load;
            tabControl1.ResumeLayout(false);
            tpBZ98R.ResumeLayout(false);
            tpBZ98R.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tpBZCC.ResumeLayout(false);
            tpBZCC.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            tpFindMods.ResumeLayout(false);
            tpFindMods.PerformLayout();
            tcFindMods.ResumeLayout(false);
            tpFindModsBZ98R.ResumeLayout(false);
            tpFindModsBZCC.ResumeLayout(false);
            tabMultiplayer.ResumeLayout(false);
            tabMultiplayer.PerformLayout();
            tcMultiplayer.ResumeLayout(false);
            tpMultiplayerBZ98R.ResumeLayout(false);
            tpMultiplayerBZCC.ResumeLayout(false);
            tpAudit.ResumeLayout(false);
            tpSettings.ResumeLayout(false);
            tpSettings.PerformLayout();
            groupBox6.ResumeLayout(false);
            groupBox6.PerformLayout();
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            tpTasks.ResumeLayout(false);
            tpTasks.PerformLayout();
            tpLog.ResumeLayout(false);
            tpLog.PerformLayout();
            tpLogSteamCmd.ResumeLayout(false);
            tpLogSteamCmdFull.ResumeLayout(false);
            tpAbout.ResumeLayout(false);
            tpAbout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)logoPictureBox).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

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
        private LinqListViewMods lvModsBZ98R;
        private System.Windows.Forms.Button btnRefreshBZ98R;
        private System.Windows.Forms.Button btnUpdateBZ98R;
        private LinqListViewMods lvModsBZCC;
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
        private System.Windows.Forms.TabPage tpFindMods;
        private System.Windows.Forms.Button btnFindMods;
        private System.Windows.Forms.TabControl tcFindMods;
        private System.Windows.Forms.TabPage tpFindModsBZ98R;
        private System.Windows.Forms.TabPage tpFindModsBZCC;
        private LinqListViewFindMods lvFindModsBZ98R;
        private LinqListViewFindMods lvFindModsBZCC;
        private System.Windows.Forms.RadioButton rbFindModsIcon;
        private System.Windows.Forms.RadioButton rbFindModsTable;
        private System.Windows.Forms.CheckBox cbFindModsNewOnly;
        private System.Windows.Forms.Button btnDownloadSelectedFoundMods;
        private System.Windows.Forms.Button btnFixSteamCmd;
        private System.Windows.Forms.TabPage tabMultiplayer;
        private System.Windows.Forms.Button btnMultiRefresh;
        private System.Windows.Forms.TabControl tcMultiplayer;
        private System.Windows.Forms.TabPage tpMultiplayerBZ98R;
        private LinqListViewMultiplayer lvMultiplayerBZ98R;
        private System.Windows.Forms.TabPage tpMultiplayerBZCC;
        private LinqListViewMultiplayer lvMultiplayerBZCC;
        private System.Windows.Forms.Button btnMultiJoinSteam;
        private System.Windows.Forms.Button btnGetModSteamCmd;
        private System.Windows.Forms.Button btnMultiGetModSteam;
        private System.Windows.Forms.Button btnMultiJoinGOG;
        private System.Windows.Forms.RadioButton rbFindGamesTable;
        private System.Windows.Forms.RadioButton rbFindGamesMap;
        private LinqListViewPlayers lvPlayers;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btnBZCCGogFind;
        private System.Windows.Forms.TextBox txtBZCCGog;
        private System.Windows.Forms.Button btnBZCCRGogApply;
        private System.Windows.Forms.TabPage tpAbout;
        private System.Windows.Forms.Button btnGithub;
        private System.Windows.Forms.Button btnDiscord;
        private System.Windows.Forms.Button btnSteamAward;
        private System.Windows.Forms.PictureBox logoPictureBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabPage tpAudit;
        private System.Windows.Forms.Button btnRunAudit;
        private System.Windows.Forms.RichTextBox txtAuditLog;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button btnGitFind;
        private System.Windows.Forms.TextBox txtGit;
        private System.Windows.Forms.Button btnGitApply;
    }
}

