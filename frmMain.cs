using AU_Whiteboard_Editor.Classes;
using Newtonsoft.Json.Bson;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace AU_Whiteboard_Editor
{
    public partial class frmMain : Form
    {
        private string AUBluePrintsPath = string.Empty;
        private string currentCustomImageFilePath = string.Empty;

        private WbColor[] palette;

        private Bitmap originalLoadedImage; // untampered loaded image
        private Bitmap originalImage; // applied background, crop, rotate

        private Bitmap modifiedImage; // dithered image
        private WhiteboardBitmap currentWhiteboardBitmap;

        public frmMain()
        {
            // https://github.com/quakec/AU_Whiteboard_Editor/blob/master/AU_Whiteboard_Editor.exe
            InitializeComponent();
            AUBluePrintsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow")
                , "ApproximatelyGames\\ApproximatelyUp\\Blueprints");
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            ofdLoadBitmap.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            lblVersion.Text = "Version: " + FileVersionInfo.GetVersionInfo(Environment.ProcessPath!).FileVersion + " by QuakeC";
            AddBackFillEntries();
            AddResizeModeEntries();
            AddDitherTypeEntries();
            AddColorSpaceEntries();
            UpdatePaletteFromUserSelected();
            Refresh();
        }

        private void AddBackFillEntries()
        {
            cboBackfill.Items.Add(new DropDownItem("White"));
            cboBackfill.SelectedIndex = 0;
            cboBackfill.Items.Add(new DropDownItem("Black"));
            cboBackfill.Items.Add(new DropDownItem("Red"));
            cboBackfill.Items.Add(new DropDownItem("Green"));
            cboBackfill.Items.Add(new DropDownItem("Blue"));
            cboBackfill.Items.Add(new DropDownItem("Yellow"));
            cboBackfill.Items.Add(new DropDownItem("Magenta"));
            cboBackfill.Items.Add(new DropDownItem("Cyan"));
            cboBackfill.Enabled = true;
        }

        private void AddResizeModeEntries()
        {
            cboResizeMode.FormattingEnabled = true;
            cboResizeMode.DataSource = Enum.GetValues<Whiteboard.ResizeMode>();
            cboResizeMode.Format += (sender, e) =>
            {
                if (e.ListItem is Whiteboard.ResizeMode value)
                    e.Value = value.GetDescription();
            };
            cboResizeMode.SelectedIndex = 0;
            cboResizeMode.Enabled = true;
        }

        private void AddDitherTypeEntries()
        {
            cboDitherType.FormattingEnabled = true;
            cboDitherType.DataSource = Enum.GetValues<Whiteboard.DitherType>();
            cboDitherType.Format += (sender, e) =>
            {
                if (e.ListItem is Whiteboard.DitherType value)
                    e.Value = value.GetDescription();
            };
            cboDitherType.SelectedIndex = 0;
            cboDitherType.Enabled = true;
        }

        private void AddColorSpaceEntries()
        {
            cboColorSpace.FormattingEnabled = true;
            cboColorSpace.DataSource = Enum.GetValues<Whiteboard.ColorSpace>();
            cboColorSpace.Format += (sender, e) =>
            {
                if (e.ListItem is Whiteboard.ColorSpace value)
                    e.Value = value.GetDescription();
            };
            cboColorSpace.SelectedIndex = 0;
            cboColorSpace.Enabled = true;
        }

        private void DisposeNodeBitmaps(TreeNodeCollection parentNodes)
        {
            foreach (TreeNode node in parentNodes)
            {
                DisposeNodeBitmaps(node.Nodes);
                if (node.Tag is WhiteboardBitmap bitmap)
                    bitmap.Bitmap?.Dispose();
            }
        }

        private void Refresh(string query = "")
        {
            DisposeNodeBitmaps(tvwShips.Nodes);

            tvwShips.Nodes.Clear();
            List<Ship> ships = EnumerateShips(query);
            InitializeTreeView(ships);
            if (!string.IsNullOrEmpty(query))
                tvwShips.ExpandAll();
        }

        private List<Ship> EnumerateShips(string query = "")
        {
            List<Ship> ships = new List<Ship>();

            List<string> metaFiles = Directory.EnumerateFiles(AUBluePrintsPath, "*.bpmeta").ToList();

            foreach (string metaFile in metaFiles)
            {
                Ship ship = Ship.Load(metaFile, palette);
                if (ship != null)
                {
                    if (string.IsNullOrEmpty(query))
                        ships.Add(ship);
                    else if (ship._name.Contains(query, StringComparison.CurrentCultureIgnoreCase))
                        ships.Add(ship);
                }
            }

            return ships;
        }

        private void InitializeTreeView(List<Ship> ships)
        {
            // generate folders
            List<string> folders = new List<string>();
            foreach (Ship ship in ships)
            {
                if (!folders.Contains(ship._folder))
                    folders.Add(ship._folder);
            }
            folders = folders.OrderBy(o => o != "Root").ThenBy(o => o).ToList();

            foreach (string folder in folders)
            {
                TreeNode folderNode = tvwShips.Nodes.Add(folder, folder);
                folderNode.ForeColor = Color.DarkRed;
            }

            // insert ships into treeview
            foreach (Ship ship in ships)
            {
                TreeNode folderNode = tvwShips.Nodes.Find(ship._folder, false)[0];

                TreeNode shipNode = folderNode.Nodes.Add(ship._name);
                shipNode.ForeColor = Color.DarkBlue;

                int i = 0;
                foreach (WhiteboardBitmap whiteboardBitmap in ship._bitmaps)
                {
                    TreeNode imageNode = shipNode.Nodes.Add($"i{i}", $"Whiteboard #{i + 1}");
                    imageNode.ForeColor = Color.DarkGreen;
                    imageNode.Tag = whiteboardBitmap;
                    i++;
                }
            }

        }

        private void ApplyImageProcessing(Bitmap image)
        {
            if (image == null) return;
            DropDownItem selectedBackFill = new DropDownItem("White");

            Bitmap bitmap = new Bitmap(originalLoadedImage);

            // apply background
            if (cboBackfill.SelectedItem != null)
            {
                bitmap = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);

                selectedBackFill = (DropDownItem)cboBackfill.SelectedItem;

                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.Clear(selectedBackFill.Color);
                    graphics.DrawImage(image, 0, 0, image.Width, image.Height);
                }
            }

            // rotate
            if (chkRotate.Checked)
                bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);

            // apply back fill, resizemode
            Whiteboard.ResizeMode resizeMode = (Whiteboard.ResizeMode)cboResizeMode.SelectedItem;
            bitmap = Whiteboard.ResizeBitmap(bitmap, selectedBackFill.Color, resizeMode, trkCropPosition.Value);

            // brightness
            if (trkBrightness.Value != 25)
                bitmap = Whiteboard.AdjustBrightness(bitmap, trkBrightness.Value - 25);

            originalImage?.Dispose();
            originalImage = Whiteboard.CropBitmap(bitmap, selectedBackFill.Color, resizeMode, trkCropPosition.Value);
            ShowOriginalImage(originalImage);

            // apply dither
            Whiteboard.DitherType ditherType = (Whiteboard.DitherType)cboDitherType.SelectedItem;
            Whiteboard.ColorSpace colorSpace = (Whiteboard.ColorSpace)cboColorSpace.SelectedItem;
            if (ditherType == Whiteboard.DitherType.None)
            {
                bitmap = Whiteboard.ApplyPalette(bitmap, palette, colorSpace);
            }
            else
            {
                bitmap = Whiteboard.ApplyPaletteDithered(bitmap, palette, ditherType, colorSpace);
            }

            modifiedImage?.Dispose();
            modifiedImage = bitmap;
            ShowModifiedImage(modifiedImage);
        }

        private void ShowOriginalImage(Bitmap image)
        {
            picOriginal.Image?.Dispose();

            picOriginal.Image = new Bitmap(image);

            TestForWriteEnable();
        }

        private void ShowModifiedImage(Bitmap image)
        {
            picImported.Image?.Dispose();

            picImported.Image = new Bitmap(image);

            TestForWriteEnable();
        }

        private void ShowPreviewImage()
        {
            if (currentWhiteboardBitmap == null)
                return;
            picWhiteboard.Image?.Dispose();
            picWhiteboard.Image = new Bitmap(currentWhiteboardBitmap.Bitmap);
            TestForWriteEnable();
        }

        private void UpdatePaletteFromUserSelected()
        {
            List<WbColor> wbColors = new List<WbColor>();
            if (chkPaletteWhite.Checked) wbColors.Add(new WbColor(Color.White));
            if (chkPaletteBlack.Checked) wbColors.Add(new WbColor(Color.Black));
            if (chkPaletteRed.Checked) wbColors.Add(new WbColor(Color.Red));
            if (chkPaletteGreen.Checked) wbColors.Add(new WbColor(Color.Green));
            if (chkPaletteBlue.Checked) wbColors.Add(new WbColor(Color.Blue));
            if (chkPaletteYellow.Checked) wbColors.Add(new WbColor(Color.Yellow));
            if (chkPaletteMagenta.Checked) wbColors.Add(new WbColor(Color.Magenta));
            if (chkPaletteCyan.Checked) wbColors.Add(new WbColor(Color.Cyan));
            palette = wbColors.ToArray();
        }

        private void TestForWriteEnable()
        {
            btnOverwrite.Enabled = (currentWhiteboardBitmap != null && modifiedImage != null);
        }
        private bool suppressOptionChange = false;

        private void OptionChanged()
        {
            if (suppressOptionChange) return;

            UpdatePaletteFromUserSelected();
            ApplyImageProcessing(originalLoadedImage);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Refresh(txtSearch.Text);
        }

        private void btnExpandAll_Click(object sender, EventArgs e)
        {
            tvwShips.ExpandAll();
        }

        private void btnCollapseAll_Click(object sender, EventArgs e)
        {
            tvwShips.CollapseAll();
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                Refresh(txtSearch.Text);
                e.Handled = true;
            }
        }

        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
                return;
            txtSearch.Text = string.Empty;
            Refresh();
        }

        private void tvwShips_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs args)
        {
            if (args.Node.Tag != null)
            {
                currentWhiteboardBitmap = (WhiteboardBitmap)args.Node.Tag;
                ShowPreviewImage();
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (ofdLoadBitmap.ShowDialog() == DialogResult.OK)
            {
                using Stream stream = ofdLoadBitmap.OpenFile();
                using Image image = Image.FromStream(stream);

                originalLoadedImage?.Dispose();
                originalLoadedImage = new Bitmap(image);

                suppressOptionChange = true;
                chkPaletteWhite.Checked = true;
                chkPaletteBlack.Checked = true;
                chkPaletteRed.Checked = true;
                chkPaletteGreen.Checked = true;
                chkPaletteBlue.Checked = true;
                chkPaletteYellow.Checked = true;
                chkPaletteMagenta.Checked = true;
                chkPaletteCyan.Checked = true;
                suppressOptionChange = false;

                ApplyImageProcessing(originalLoadedImage);
            }
        }

        private void cboBackfill_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender == null)
                return;
            ColorSelector control = sender as ColorSelector;
            if (!control.Enabled)
                return;

            OptionChanged();
        }

        private void chkPaletteWhite_CheckedChanged(object sender, EventArgs e)
        {
            chkPaletteWhite.Text = chkPaletteWhite.Checked ? "a" : "";
            OptionChanged();
        }

        private void chkPaletteBlack_CheckedChanged(object sender, EventArgs e)
        {
            chkPaletteBlack.Text = chkPaletteBlack.Checked ? "a" : "";
            OptionChanged();
        }

        private void chkPaletteRed_CheckedChanged(object sender, EventArgs e)
        {
            chkPaletteRed.Text = chkPaletteRed.Checked ? "a" : "";
            OptionChanged();
        }

        private void chkPaletteGreen_CheckedChanged(object sender, EventArgs e)
        {
            chkPaletteGreen.Text = chkPaletteGreen.Checked ? "a" : "";
            OptionChanged();
        }

        private void chkPaletteBlue_CheckedChanged(object sender, EventArgs e)
        {
            chkPaletteBlue.Text = chkPaletteBlue.Checked ? "a" : "";
            OptionChanged();
        }

        private void chkPaletteYellow_CheckedChanged(object sender, EventArgs e)
        {
            chkPaletteYellow.Text = chkPaletteYellow.Checked ? "a" : "";
            OptionChanged();
        }

        private void chkPaletteMagenta_CheckedChanged(object sender, EventArgs e)
        {
            chkPaletteMagenta.Text = chkPaletteMagenta.Checked ? "a" : "";
            OptionChanged();
        }

        private void chkPaletteCyan_CheckedChanged(object sender, EventArgs e)
        {
            chkPaletteCyan.Text = chkPaletteCyan.Checked ? "a" : "";
            OptionChanged();
        }
        private void trkBrightness_Scroll(object sender, EventArgs e)
        {
            toolTip.SetToolTip(trkBrightness, trkBrightness.Value.ToString());
            OptionChanged();
        }

        private void cboResizeMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender == null)
                return;
            ComboBox control = sender as ComboBox;
            if (!control.Enabled)
                return;

            lblCropPosition.Enabled = ((Whiteboard.ResizeMode)cboResizeMode.SelectedItem) == Whiteboard.ResizeMode.Crop;
            trkCropPosition.Enabled = ((Whiteboard.ResizeMode)cboResizeMode.SelectedItem) == Whiteboard.ResizeMode.Crop;
            OptionChanged();
        }
        private void trkCropPosition_Scroll(object sender, EventArgs e)
        {
            toolTip.SetToolTip(trkCropPosition, trkCropPosition.Value.ToString());
            OptionChanged();
        }

        private void chkDither_CheckedChanged(object sender, EventArgs e)
        {
            OptionChanged();
        }
        private void cboDitherType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender == null)
                return;
            ComboBox control = sender as ComboBox;
            if (!control.Enabled)
                return;

            OptionChanged();
        }
        private void cboColorSpace_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender == null)
                return;
            ComboBox control = sender as ComboBox;
            if (!control.Enabled)
                return;

            OptionChanged();
        }

        private void chkRotate_CheckedChanged(object sender, EventArgs e)
        {
            OptionChanged();
        }

        private void btnOverwrite_Click(object sender, EventArgs e)
        {
            TreeNode node = tvwShips.SelectedNode;
            if (node.Tag != null)
            {
                WhiteboardBitmap whiteboardBitmap = (WhiteboardBitmap)node.Tag;
                whiteboardBitmap.Bitmap = new Bitmap(modifiedImage);
                ShowPreviewImage();
                Whiteboard.WriteWhiteboardImage(currentWhiteboardBitmap.FilePath, currentWhiteboardBitmap.ImageIndex, currentWhiteboardBitmap.Bitmap, palette);
                btnOverwrite.Enabled = false;
            }
        }

        private void picWhiteboard_Click(object sender, EventArgs e)
        {
            btnOpen_Click(sender, e);
        }
    }
}
