


namespace AU_Whiteboard_Editor
{
    partial class frmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            picImported = new PictureBox();
            tvwShips = new TreeView();
            btnExpandAll = new Button();
            btnCollapseAll = new Button();
            ofdLoadBitmap = new OpenFileDialog();
            btnOverwrite = new Button();
            btnRefresh = new Button();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            btnOpen = new Button();
            lblPalette = new Label();
            chkPaletteBlack = new CheckBox();
            chkPaletteRed = new CheckBox();
            chkPaletteGreen = new CheckBox();
            chkPaletteBlue = new CheckBox();
            chkPaletteYellow = new CheckBox();
            chkPaletteMagenta = new CheckBox();
            chkPaletteCyan = new CheckBox();
            lblVersion = new Label();
            txtSearch = new TextBox();
            btnClearSearch = new Label();
            lblBackFill = new Label();
            toolTip = new ToolTip(components);
            cboDitherType = new ComboBox();
            lblDitherType = new Label();
            lblResizeMode = new Label();
            cboResizeMode = new ComboBox();
            trkCropPosition = new TrackBar();
            chkPaletteWhite = new CheckBox();
            picWhiteboard = new PictureBox();
            chkRotate = new CheckBox();
            trkBrightness = new TrackBar();
            cboColorSpace = new ComboBox();
            lblColorSpace = new Label();
            lblBrightness = new Label();
            lblCropPosition = new Label();
            picOriginal = new PictureBox();
            cboBackfill = new ColorSelector();
            ((System.ComponentModel.ISupportInitialize)picImported).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trkCropPosition).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picWhiteboard).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trkBrightness).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picOriginal).BeginInit();
            SuspendLayout();
            // 
            // picImported
            // 
            picImported.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            picImported.BackColor = SystemColors.Window;
            picImported.BorderStyle = BorderStyle.FixedSingle;
            picImported.Location = new Point(483, 67);
            picImported.Name = "picImported";
            picImported.Size = new Size(576, 384);
            picImported.SizeMode = PictureBoxSizeMode.StretchImage;
            picImported.TabIndex = 0;
            picImported.TabStop = false;
            picImported.Click += picWhiteboard_Click;
            // 
            // tvwShips
            // 
            tvwShips.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            tvwShips.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tvwShips.ForeColor = Color.DarkGreen;
            tvwShips.FullRowSelect = true;
            tvwShips.HideSelection = false;
            tvwShips.LineColor = Color.FromArgb(227, 227, 227);
            tvwShips.Location = new Point(8, 71);
            tvwShips.Name = "tvwShips";
            tvwShips.Size = new Size(271, 762);
            tvwShips.TabIndex = 15;
            tvwShips.NodeMouseClick += tvwShips_NodeMouseClick;
            // 
            // btnExpandAll
            // 
            btnExpandAll.Location = new Point(73, 8);
            btnExpandAll.Name = "btnExpandAll";
            btnExpandAll.Size = new Size(74, 23);
            btnExpandAll.TabIndex = 1;
            btnExpandAll.Text = "Expand";
            btnExpandAll.UseVisualStyleBackColor = true;
            btnExpandAll.Click += btnExpandAll_Click;
            // 
            // btnCollapseAll
            // 
            btnCollapseAll.Location = new Point(153, 8);
            btnCollapseAll.Name = "btnCollapseAll";
            btnCollapseAll.Size = new Size(63, 23);
            btnCollapseAll.TabIndex = 2;
            btnCollapseAll.Text = "Collapse All";
            btnCollapseAll.UseVisualStyleBackColor = true;
            btnCollapseAll.Click += btnCollapseAll_Click;
            // 
            // ofdLoadBitmap
            // 
            ofdLoadBitmap.Filter = "Image Files|*.bmp;*.dib;*.gif;*.jpg;*.jpeg;*.jpe;*.jfif;*.png;*.tif;*.tiff;*.ico;*.icon;*.wmf;*.emf";
            // 
            // btnOverwrite
            // 
            btnOverwrite.Enabled = false;
            btnOverwrite.Image = (Image)resources.GetObject("btnOverwrite.Image");
            btnOverwrite.ImageAlign = ContentAlignment.TopCenter;
            btnOverwrite.Location = new Point(399, 200);
            btnOverwrite.Name = "btnOverwrite";
            btnOverwrite.Size = new Size(78, 60);
            btnOverwrite.TabIndex = 12;
            btnOverwrite.Text = "Overwrite";
            btnOverwrite.TextImageRelation = TextImageRelation.ImageAboveText;
            btnOverwrite.UseVisualStyleBackColor = true;
            btnOverwrite.Click += btnOverwrite_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(7, 8);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(60, 23);
            btnRefresh.TabIndex = 0;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // tabPage1
            // 
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(192, 72);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "tabPage1";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(192, 72);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "tabPage2";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnOpen
            // 
            btnOpen.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOpen.Location = new Point(285, 8);
            btnOpen.Name = "btnOpen";
            btnOpen.Size = new Size(83, 23);
            btnOpen.TabIndex = 3;
            btnOpen.Text = "Open Image";
            btnOpen.UseVisualStyleBackColor = true;
            btnOpen.Click += btnOpen_Click;
            // 
            // lblPalette
            // 
            lblPalette.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblPalette.AutoSize = true;
            lblPalette.Location = new Point(291, 39);
            lblPalette.Name = "lblPalette";
            lblPalette.Size = new Size(85, 15);
            lblPalette.TabIndex = 10;
            lblPalette.Text = "Colour Palette:";
            // 
            // chkPaletteBlack
            // 
            chkPaletteBlack.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkPaletteBlack.Appearance = Appearance.Button;
            chkPaletteBlack.BackColor = Color.Black;
            chkPaletteBlack.CheckAlign = ContentAlignment.MiddleCenter;
            chkPaletteBlack.Checked = true;
            chkPaletteBlack.CheckState = CheckState.Checked;
            chkPaletteBlack.Font = new Font("Webdings", 12F);
            chkPaletteBlack.ForeColor = Color.White;
            chkPaletteBlack.Location = new Point(402, 36);
            chkPaletteBlack.Name = "chkPaletteBlack";
            chkPaletteBlack.Size = new Size(24, 24);
            chkPaletteBlack.TabIndex = 4;
            chkPaletteBlack.Text = "a";
            chkPaletteBlack.TextAlign = ContentAlignment.TopCenter;
            chkPaletteBlack.UseCompatibleTextRendering = true;
            chkPaletteBlack.UseVisualStyleBackColor = false;
            chkPaletteBlack.CheckedChanged += chkPaletteBlack_CheckedChanged;
            // 
            // chkPaletteRed
            // 
            chkPaletteRed.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkPaletteRed.Appearance = Appearance.Button;
            chkPaletteRed.BackColor = Color.Red;
            chkPaletteRed.CheckAlign = ContentAlignment.MiddleCenter;
            chkPaletteRed.Checked = true;
            chkPaletteRed.CheckState = CheckState.Checked;
            chkPaletteRed.Font = new Font("Webdings", 12F);
            chkPaletteRed.ForeColor = Color.White;
            chkPaletteRed.Location = new Point(425, 36);
            chkPaletteRed.Name = "chkPaletteRed";
            chkPaletteRed.Size = new Size(24, 24);
            chkPaletteRed.TabIndex = 5;
            chkPaletteRed.Text = "a";
            chkPaletteRed.TextAlign = ContentAlignment.TopCenter;
            chkPaletteRed.UseCompatibleTextRendering = true;
            chkPaletteRed.UseVisualStyleBackColor = false;
            chkPaletteRed.CheckedChanged += chkPaletteRed_CheckedChanged;
            // 
            // chkPaletteGreen
            // 
            chkPaletteGreen.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkPaletteGreen.Appearance = Appearance.Button;
            chkPaletteGreen.BackColor = Color.Green;
            chkPaletteGreen.CheckAlign = ContentAlignment.MiddleCenter;
            chkPaletteGreen.Checked = true;
            chkPaletteGreen.CheckState = CheckState.Checked;
            chkPaletteGreen.Font = new Font("Webdings", 12F);
            chkPaletteGreen.ForeColor = Color.White;
            chkPaletteGreen.Location = new Point(448, 36);
            chkPaletteGreen.Name = "chkPaletteGreen";
            chkPaletteGreen.Size = new Size(24, 24);
            chkPaletteGreen.TabIndex = 6;
            chkPaletteGreen.Text = "a";
            chkPaletteGreen.TextAlign = ContentAlignment.TopCenter;
            chkPaletteGreen.UseCompatibleTextRendering = true;
            chkPaletteGreen.UseVisualStyleBackColor = false;
            chkPaletteGreen.CheckedChanged += chkPaletteGreen_CheckedChanged;
            // 
            // chkPaletteBlue
            // 
            chkPaletteBlue.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkPaletteBlue.Appearance = Appearance.Button;
            chkPaletteBlue.BackColor = Color.RoyalBlue;
            chkPaletteBlue.CheckAlign = ContentAlignment.MiddleCenter;
            chkPaletteBlue.Checked = true;
            chkPaletteBlue.CheckState = CheckState.Checked;
            chkPaletteBlue.Font = new Font("Webdings", 12F);
            chkPaletteBlue.ForeColor = Color.White;
            chkPaletteBlue.Location = new Point(471, 36);
            chkPaletteBlue.Name = "chkPaletteBlue";
            chkPaletteBlue.Size = new Size(24, 24);
            chkPaletteBlue.TabIndex = 7;
            chkPaletteBlue.Text = "a";
            chkPaletteBlue.TextAlign = ContentAlignment.TopCenter;
            chkPaletteBlue.UseCompatibleTextRendering = true;
            chkPaletteBlue.UseVisualStyleBackColor = false;
            chkPaletteBlue.CheckedChanged += chkPaletteBlue_CheckedChanged;
            // 
            // chkPaletteYellow
            // 
            chkPaletteYellow.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkPaletteYellow.Appearance = Appearance.Button;
            chkPaletteYellow.BackColor = Color.Yellow;
            chkPaletteYellow.CheckAlign = ContentAlignment.MiddleCenter;
            chkPaletteYellow.Checked = true;
            chkPaletteYellow.CheckState = CheckState.Checked;
            chkPaletteYellow.Font = new Font("Webdings", 12F);
            chkPaletteYellow.ForeColor = Color.Black;
            chkPaletteYellow.Location = new Point(494, 36);
            chkPaletteYellow.Name = "chkPaletteYellow";
            chkPaletteYellow.Size = new Size(24, 24);
            chkPaletteYellow.TabIndex = 8;
            chkPaletteYellow.Text = "a";
            chkPaletteYellow.TextAlign = ContentAlignment.TopCenter;
            chkPaletteYellow.UseCompatibleTextRendering = true;
            chkPaletteYellow.UseVisualStyleBackColor = false;
            chkPaletteYellow.CheckedChanged += chkPaletteYellow_CheckedChanged;
            // 
            // chkPaletteMagenta
            // 
            chkPaletteMagenta.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkPaletteMagenta.Appearance = Appearance.Button;
            chkPaletteMagenta.BackColor = Color.Fuchsia;
            chkPaletteMagenta.CheckAlign = ContentAlignment.MiddleCenter;
            chkPaletteMagenta.Checked = true;
            chkPaletteMagenta.CheckState = CheckState.Checked;
            chkPaletteMagenta.Font = new Font("Webdings", 12F);
            chkPaletteMagenta.ForeColor = Color.White;
            chkPaletteMagenta.Location = new Point(517, 36);
            chkPaletteMagenta.Name = "chkPaletteMagenta";
            chkPaletteMagenta.Size = new Size(24, 24);
            chkPaletteMagenta.TabIndex = 9;
            chkPaletteMagenta.Text = "a";
            chkPaletteMagenta.TextAlign = ContentAlignment.TopCenter;
            chkPaletteMagenta.UseCompatibleTextRendering = true;
            chkPaletteMagenta.UseVisualStyleBackColor = false;
            chkPaletteMagenta.CheckedChanged += chkPaletteMagenta_CheckedChanged;
            // 
            // chkPaletteCyan
            // 
            chkPaletteCyan.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkPaletteCyan.Appearance = Appearance.Button;
            chkPaletteCyan.BackColor = Color.Cyan;
            chkPaletteCyan.CheckAlign = ContentAlignment.MiddleCenter;
            chkPaletteCyan.Checked = true;
            chkPaletteCyan.CheckState = CheckState.Checked;
            chkPaletteCyan.Font = new Font("Webdings", 12F, FontStyle.Regular, GraphicsUnit.Point, 2);
            chkPaletteCyan.ForeColor = Color.Black;
            chkPaletteCyan.Location = new Point(540, 36);
            chkPaletteCyan.Name = "chkPaletteCyan";
            chkPaletteCyan.Size = new Size(24, 24);
            chkPaletteCyan.TabIndex = 10;
            chkPaletteCyan.Text = "a";
            chkPaletteCyan.TextAlign = ContentAlignment.TopCenter;
            chkPaletteCyan.UseCompatibleTextRendering = true;
            chkPaletteCyan.UseVisualStyleBackColor = false;
            chkPaletteCyan.CheckedChanged += chkPaletteCyan_CheckedChanged;
            // 
            // lblVersion
            // 
            lblVersion.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lblVersion.ForeColor = SystemColors.ControlDarkDark;
            lblVersion.Location = new Point(8, 835);
            lblVersion.Name = "lblVersion";
            lblVersion.Size = new Size(1051, 17);
            lblVersion.TabIndex = 18;
            lblVersion.Text = "[version]";
            lblVersion.TextAlign = ContentAlignment.BottomRight;
            // 
            // txtSearch
            // 
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtSearch.Location = new Point(7, 37);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderText = "Search...";
            txtSearch.Size = new Size(249, 27);
            txtSearch.TabIndex = 13;
            txtSearch.KeyPress += txtSearch_KeyPress;
            // 
            // btnClearSearch
            // 
            btnClearSearch.BackColor = SystemColors.Window;
            btnClearSearch.BorderStyle = BorderStyle.FixedSingle;
            btnClearSearch.Font = new Font("Webdings", 12F, FontStyle.Regular, GraphicsUnit.Point, 2);
            btnClearSearch.Location = new Point(254, 37);
            btnClearSearch.Name = "btnClearSearch";
            btnClearSearch.Size = new Size(25, 27);
            btnClearSearch.TabIndex = 14;
            btnClearSearch.Text = "r";
            btnClearSearch.TextAlign = ContentAlignment.MiddleCenter;
            btnClearSearch.Click += btnClearSearch_Click;
            // 
            // lblBackFill
            // 
            lblBackFill.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblBackFill.AutoSize = true;
            lblBackFill.Location = new Point(377, 12);
            lblBackFill.Name = "lblBackFill";
            lblBackFill.Size = new Size(53, 15);
            lblBackFill.TabIndex = 20;
            lblBackFill.Text = "Back Fill:";
            // 
            // cboDitherType
            // 
            cboDitherType.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cboDitherType.DropDownStyle = ComboBoxStyle.DropDownList;
            cboDitherType.Enabled = false;
            cboDitherType.FormattingEnabled = true;
            cboDitherType.Location = new Point(767, 36);
            cboDitherType.Name = "cboDitherType";
            cboDitherType.Size = new Size(148, 23);
            cboDitherType.TabIndex = 22;
            cboDitherType.SelectedIndexChanged += cboDitherType_SelectedIndexChanged;
            // 
            // lblDitherType
            // 
            lblDitherType.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblDitherType.AutoSize = true;
            lblDitherType.Location = new Point(706, 39);
            lblDitherType.Name = "lblDitherType";
            lblDitherType.Size = new Size(59, 15);
            lblDitherType.TabIndex = 23;
            lblDitherType.Text = "Dithering:";
            // 
            // lblResizeMode
            // 
            lblResizeMode.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblResizeMode.AutoSize = true;
            lblResizeMode.Location = new Point(539, 12);
            lblResizeMode.Name = "lblResizeMode";
            lblResizeMode.Size = new Size(76, 15);
            lblResizeMode.TabIndex = 25;
            lblResizeMode.Text = "Resize Mode:";
            // 
            // cboResizeMode
            // 
            cboResizeMode.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cboResizeMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cboResizeMode.Enabled = false;
            cboResizeMode.FormattingEnabled = true;
            cboResizeMode.Location = new Point(617, 8);
            cboResizeMode.Name = "cboResizeMode";
            cboResizeMode.Size = new Size(135, 23);
            cboResizeMode.TabIndex = 24;
            cboResizeMode.SelectedIndexChanged += cboResizeMode_SelectedIndexChanged;
            // 
            // trkCropPosition
            // 
            trkCropPosition.AutoSize = false;
            trkCropPosition.Enabled = false;
            trkCropPosition.Location = new Point(846, 8);
            trkCropPosition.Maximum = 100;
            trkCropPosition.Name = "trkCropPosition";
            trkCropPosition.Size = new Size(124, 24);
            trkCropPosition.TabIndex = 26;
            trkCropPosition.TickStyle = TickStyle.None;
            trkCropPosition.Scroll += trkCropPosition_Scroll;
            // 
            // chkPaletteWhite
            // 
            chkPaletteWhite.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkPaletteWhite.Appearance = Appearance.Button;
            chkPaletteWhite.BackColor = Color.White;
            chkPaletteWhite.CheckAlign = ContentAlignment.MiddleCenter;
            chkPaletteWhite.Checked = true;
            chkPaletteWhite.CheckState = CheckState.Checked;
            chkPaletteWhite.Font = new Font("Webdings", 12F);
            chkPaletteWhite.ForeColor = Color.Black;
            chkPaletteWhite.Location = new Point(379, 36);
            chkPaletteWhite.Name = "chkPaletteWhite";
            chkPaletteWhite.Size = new Size(24, 24);
            chkPaletteWhite.TabIndex = 27;
            chkPaletteWhite.Text = "a";
            chkPaletteWhite.TextAlign = ContentAlignment.TopCenter;
            chkPaletteWhite.UseCompatibleTextRendering = true;
            chkPaletteWhite.UseVisualStyleBackColor = false;
            chkPaletteWhite.CheckedChanged += chkPaletteWhite_CheckedChanged;
            // 
            // picWhiteboard
            // 
            picWhiteboard.BackColor = SystemColors.Window;
            picWhiteboard.BorderStyle = BorderStyle.FixedSingle;
            picWhiteboard.Location = new Point(285, 66);
            picWhiteboard.Name = "picWhiteboard";
            picWhiteboard.Size = new Size(192, 128);
            picWhiteboard.SizeMode = PictureBoxSizeMode.StretchImage;
            picWhiteboard.TabIndex = 28;
            picWhiteboard.TabStop = false;
            // 
            // chkRotate
            // 
            chkRotate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkRotate.AutoSize = true;
            chkRotate.Location = new Point(976, 11);
            chkRotate.Name = "chkRotate";
            chkRotate.Size = new Size(80, 19);
            chkRotate.TabIndex = 29;
            chkRotate.Text = "Rotate 90°";
            chkRotate.UseVisualStyleBackColor = true;
            chkRotate.CheckedChanged += chkRotate_CheckedChanged;
            // 
            // trkBrightness
            // 
            trkBrightness.AutoSize = false;
            trkBrightness.Location = new Point(637, 34);
            trkBrightness.Maximum = 50;
            trkBrightness.Name = "trkBrightness";
            trkBrightness.Size = new Size(64, 24);
            trkBrightness.TabIndex = 30;
            trkBrightness.TickFrequency = 25;
            trkBrightness.Value = 25;
            trkBrightness.Scroll += trkBrightness_Scroll;
            // 
            // cboColorSpace
            // 
            cboColorSpace.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cboColorSpace.DropDownStyle = ComboBoxStyle.DropDownList;
            cboColorSpace.Enabled = false;
            cboColorSpace.FormattingEnabled = true;
            cboColorSpace.Location = new Point(996, 36);
            cboColorSpace.Name = "cboColorSpace";
            cboColorSpace.Size = new Size(63, 23);
            cboColorSpace.TabIndex = 31;
            cboColorSpace.SelectedIndexChanged += cboColorSpace_SelectedIndexChanged;
            // 
            // lblColorSpace
            // 
            lblColorSpace.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblColorSpace.AutoSize = true;
            lblColorSpace.Enabled = false;
            lblColorSpace.Location = new Point(921, 39);
            lblColorSpace.Name = "lblColorSpace";
            lblColorSpace.Size = new Size(73, 15);
            lblColorSpace.TabIndex = 32;
            lblColorSpace.Text = "Color Space:";
            // 
            // lblBrightness
            // 
            lblBrightness.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblBrightness.AutoSize = true;
            lblBrightness.Location = new Point(575, 39);
            lblBrightness.Name = "lblBrightness";
            lblBrightness.Size = new Size(65, 15);
            lblBrightness.TabIndex = 33;
            lblBrightness.Text = "Brightness:";
            // 
            // lblCropPosition
            // 
            lblCropPosition.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblCropPosition.AutoSize = true;
            lblCropPosition.Location = new Point(758, 12);
            lblCropPosition.Name = "lblCropPosition";
            lblCropPosition.Size = new Size(82, 15);
            lblCropPosition.TabIndex = 34;
            lblCropPosition.Text = "Crop Position:";
            // 
            // picOriginal
            // 
            picOriginal.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            picOriginal.BackColor = SystemColors.Window;
            picOriginal.BorderStyle = BorderStyle.FixedSingle;
            picOriginal.Location = new Point(483, 450);
            picOriginal.Name = "picOriginal";
            picOriginal.Size = new Size(576, 384);
            picOriginal.SizeMode = PictureBoxSizeMode.StretchImage;
            picOriginal.TabIndex = 35;
            picOriginal.TabStop = false;
            // 
            // cboBackfill
            // 
            cboBackfill.DrawMode = DrawMode.OwnerDrawFixed;
            cboBackfill.DropDownStyle = ComboBoxStyle.DropDownList;
            cboBackfill.Enabled = false;
            cboBackfill.FormattingEnabled = true;
            cboBackfill.Location = new Point(432, 8);
            cboBackfill.Name = "cboBackfill";
            cboBackfill.Size = new Size(98, 24);
            cboBackfill.TabIndex = 36;
            cboBackfill.SelectedIndexChanged += cboBackfill_SelectedIndexChanged;
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1068, 857);
            Controls.Add(cboBackfill);
            Controls.Add(picOriginal);
            Controls.Add(lblCropPosition);
            Controls.Add(trkCropPosition);
            Controls.Add(lblBrightness);
            Controls.Add(cboColorSpace);
            Controls.Add(lblColorSpace);
            Controls.Add(trkBrightness);
            Controls.Add(chkRotate);
            Controls.Add(picWhiteboard);
            Controls.Add(chkPaletteWhite);
            Controls.Add(lblResizeMode);
            Controls.Add(cboResizeMode);
            Controls.Add(lblDitherType);
            Controls.Add(cboDitherType);
            Controls.Add(lblBackFill);
            Controls.Add(btnClearSearch);
            Controls.Add(txtSearch);
            Controls.Add(lblVersion);
            Controls.Add(chkPaletteCyan);
            Controls.Add(chkPaletteMagenta);
            Controls.Add(chkPaletteYellow);
            Controls.Add(chkPaletteBlue);
            Controls.Add(chkPaletteGreen);
            Controls.Add(chkPaletteRed);
            Controls.Add(chkPaletteBlack);
            Controls.Add(lblPalette);
            Controls.Add(btnOpen);
            Controls.Add(btnRefresh);
            Controls.Add(btnOverwrite);
            Controls.Add(btnCollapseAll);
            Controls.Add(btnExpandAll);
            Controls.Add(tvwShips);
            Controls.Add(picImported);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "frmMain";
            Text = "Approximately Up: Whiteboard Editor";
            Load += frmMain_Load;
            ((System.ComponentModel.ISupportInitialize)picImported).EndInit();
            ((System.ComponentModel.ISupportInitialize)trkCropPosition).EndInit();
            ((System.ComponentModel.ISupportInitialize)picWhiteboard).EndInit();
            ((System.ComponentModel.ISupportInitialize)trkBrightness).EndInit();
            ((System.ComponentModel.ISupportInitialize)picOriginal).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion

        private PictureBox picImported;
        private TreeView tvwShips;
        private Button btnExpandAll;
        private Button btnCollapseAll;
        private OpenFileDialog ofdLoadBitmap;
        private Button btnOverwrite;
        private Label lblInfo;
        private Button btnRefresh;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Button btnOpen;
        private Label lblPalette;
        private CheckBox chkPaletteBlack;
        private CheckBox chkPaletteRed;
        private CheckBox chkPaletteGreen;
        private CheckBox chkPaletteBlue;
        private CheckBox chkPaletteYellow;
        private CheckBox chkPaletteCyan;
        private CheckBox chkPaletteMagenta;
        private Label lblVersion;
        private TextBox txtSearch;
        private Label btnClearSearch;
        private Label lblBackFill;
        private ColorSelector cboBackfill;
        private ToolTip toolTip;
        private ComboBox cboDitherType;
        private Label lblDitherType;
        private Label lblResizeMode;
        private ComboBox cboResizeMode;
        private TrackBar trkCropPosition;
        private CheckBox chkPaletteWhite;
        private PictureBox picWhiteboard;
        private CheckBox chkRotate;
        private TrackBar trkBrightness;
        private ComboBox cboColorSpace;
        private Label lblColorSpace;
        private Label lblBrightness;
        private Label lblCropPosition;
        private PictureBox picOriginal;
    }
}
