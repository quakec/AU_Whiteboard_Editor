


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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            picWhiteboard = new PictureBox();
            tvwShips = new TreeView();
            btnExpandAll = new Button();
            btnCollapseAll = new Button();
            ofdLoadBitmap = new OpenFileDialog();
            btnOverwrite = new Button();
            lblInfo = new Label();
            btnRefresh = new Button();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            ((System.ComponentModel.ISupportInitialize)picWhiteboard).BeginInit();
            SuspendLayout();
            // 
            // picWhiteboard
            // 
            picWhiteboard.BackColor = SystemColors.Window;
            picWhiteboard.BorderStyle = BorderStyle.FixedSingle;
            picWhiteboard.Location = new Point(223, 37);
            picWhiteboard.Name = "picWhiteboard";
            picWhiteboard.Size = new Size(768, 512);
            picWhiteboard.SizeMode = PictureBoxSizeMode.StretchImage;
            picWhiteboard.TabIndex = 0;
            picWhiteboard.TabStop = false;
            picWhiteboard.Click += picWhiteboard_Click;
            // 
            // tvwShips
            // 
            tvwShips.FullRowSelect = true;
            tvwShips.HideSelection = false;
            tvwShips.Location = new Point(8, 37);
            tvwShips.Name = "tvwShips";
            tvwShips.Size = new Size(209, 512);
            tvwShips.TabIndex = 3;
            tvwShips.NodeMouseClick += tvwShips_NodeMouseClick;
            // 
            // btnExpandAll
            // 
            btnExpandAll.Location = new Point(74, 12);
            btnExpandAll.Name = "btnExpandAll";
            btnExpandAll.Size = new Size(74, 23);
            btnExpandAll.TabIndex = 0;
            btnExpandAll.Text = "Expand All";
            btnExpandAll.UseVisualStyleBackColor = true;
            btnExpandAll.Click += btnExpandAll_Click;
            // 
            // btnCollapseAll
            // 
            btnCollapseAll.Location = new Point(154, 12);
            btnCollapseAll.Name = "btnCollapseAll";
            btnCollapseAll.Size = new Size(63, 23);
            btnCollapseAll.TabIndex = 1;
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
            btnOverwrite.Location = new Point(908, 8);
            btnOverwrite.Name = "btnOverwrite";
            btnOverwrite.Size = new Size(83, 23);
            btnOverwrite.TabIndex = 2;
            btnOverwrite.Text = "Overwrite";
            btnOverwrite.UseVisualStyleBackColor = true;
            btnOverwrite.Click += btnOverwrite_Click;
            // 
            // lblInfo
            // 
            lblInfo.AutoSize = true;
            lblInfo.Location = new Point(223, 16);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(240, 15);
            lblInfo.TabIndex = 6;
            lblInfo.Text = "Click the image to load your own image file.";
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(8, 12);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(60, 23);
            btnRefresh.TabIndex = 7;
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
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 558);
            Controls.Add(btnRefresh);
            Controls.Add(lblInfo);
            Controls.Add(btnOverwrite);
            Controls.Add(btnCollapseAll);
            Controls.Add(btnExpandAll);
            Controls.Add(tvwShips);
            Controls.Add(picWhiteboard);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "frmMain";
            Text = "Approximately Up: Whiteboard Editor";
            Load += frmMain_Load;
            ((System.ComponentModel.ISupportInitialize)picWhiteboard).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion

        private PictureBox picWhiteboard;
        private TreeView tvwShips;
        private Button btnExpandAll;
        private Button btnCollapseAll;
        private OpenFileDialog ofdLoadBitmap;
        private Button btnOverwrite;
        private Label lblInfo;
        private Button btnRefresh;
        private TabPage tabPage1;
        private TabPage tabPage2;
    }
}
