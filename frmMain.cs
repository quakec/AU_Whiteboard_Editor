using AU_Whiteboard_Editor.Classes;
using System.Drawing.Text;
using System.Windows.Forms;

namespace AU_Whiteboard_Editor
{
    public partial class frmMain : Form
    {
        private string AUBluePrintsPath = string.Empty;
        private string currentCustomImageFilePath = string.Empty;

        public frmMain()
        {
            InitializeComponent();
            AUBluePrintsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow")
                , "ApproximatelyGames\\ApproximatelyUp\\Blueprints");
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            ofdLoadBitmap.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            List<Ship> ships = EnumerateShips();
            InitializeTreeView(ships);
        }

        private List<Ship> EnumerateShips()
        {
            List<Ship> ships = new List<Ship>();

            List<string> metaFiles = Directory.EnumerateFiles(AUBluePrintsPath, "*.bpmeta").ToList();

            foreach (string metaFile in metaFiles)
            {
                Ship ship = Ship.Load(metaFile);
                if (ship != null)
                    ships.Add(ship);
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
            folders = folders.OrderBy(o => o != "root").ThenBy(o => o).ToList();

            foreach (string folder in folders)
            {
                tvwShips.Nodes.Add(folder, folder);
            }

            foreach (Ship ship in ships)
            {
                TreeNode folderNode = tvwShips.Nodes.Find(ship._folder, false)[0];

                TreeNode shipNode = folderNode.Nodes.Add(ship._name);

                int i = 0;
                foreach (WhiteboardBitmap whiteboardBitmap in ship._bitmaps)
                {
                    TreeNode imageNode = shipNode.Nodes.Add($"i{i}", i.ToString());
                    imageNode.Tag = whiteboardBitmap;
                    i++;
                }
            }

        }
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            tvwShips.Nodes.Clear();
            List<Ship> ships = EnumerateShips();
            InitializeTreeView(ships);
        }

        private void btnExpandAll_Click(object sender, EventArgs e)
        {
            tvwShips.ExpandAll();
        }

        private void btnCollapseAll_Click(object sender, EventArgs e)
        {
            tvwShips.CollapseAll();
        }

        private void tvwShips_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs args)
        {
            if (args.Node.Tag != null)
            {
                picWhiteboard.Image?.Dispose();
                picWhiteboard.Image = new Bitmap(((WhiteboardBitmap)args.Node.Tag).Bitmap);
                btnOverwrite.Enabled = false;
            }
        }

        private void btnOverwrite_Click(object sender, EventArgs e)
        {
            TreeNode node = tvwShips.SelectedNode;
            if (node.Tag != null)
            {
                WhiteboardBitmap whiteboardBitmap = (WhiteboardBitmap)node.Tag;
                int index = 0;
                if (int.TryParse(node.Text, out index))
                {
                    if (picWhiteboard.Image is Bitmap bitmap)
                    {
                        Whiteboard.WriteBitmap(whiteboardBitmap.FilePath, index, bitmap);
                        whiteboardBitmap.Bitmap = new Bitmap(bitmap);
                        btnOverwrite.Enabled = false;
                    }
                }
            }
        }

        private void picWhiteboard_Click(object sender, EventArgs e)
        {
            if (ofdLoadBitmap.ShowDialog() == DialogResult.OK)
            {
                using Stream stream = ofdLoadBitmap.OpenFile();
                using Image image = Image.FromStream(stream);

                picWhiteboard.Image?.Dispose();
                picWhiteboard.Image = new Bitmap(image);
                btnOverwrite.Enabled = true;
            }
        }
    }
}
