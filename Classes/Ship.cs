
using System.Text;
using Newtonsoft.Json;

namespace AU_Whiteboard_Editor.Classes
{
    public class Ship
    {
        // mappings
        public string _name { get; set; }
        public string _folder { get; set; }
        public string _version { get; set; }

        // meta data
        public string _guid { get; set; }
        public List<WhiteboardBitmap> _bitmaps { get; set; }
        

        public Ship()
        {
            _name = string.Empty; _folder = string.Empty; _version = string.Empty; _guid = string.Empty; _bitmaps = new List<WhiteboardBitmap>();
        }

        public static Ship Load(string filePath, WbColor[] palette)
        {
            Ship ship = new Ship();

            // deserialize
            string json = File.ReadAllText(filePath, Encoding.UTF8);
            if (json != null)
                ship = JsonConvert.DeserializeObject<Ship>(json);
            if (string.IsNullOrEmpty(ship._folder))
                ship._folder = "Root";

            // meta data
            ship._guid = Path.GetFileName(filePath).ToLower();

            string exPath = filePath.Replace(".bpmeta", ".bpex");
            if (!File.Exists(exPath))
                return null;

            int boardCount = 0;
            Bitmap bitmap = Whiteboard.ReadWhiteboardImage(exPath, 0, out boardCount, palette);
            WhiteboardBitmap whiteboardBitmap = new WhiteboardBitmap(exPath, 0, bitmap);
            ship._bitmaps.Add(whiteboardBitmap);
            if (boardCount > 1)
            {
                for (int i = 1; i < boardCount; i++)
                {
                    bitmap = Whiteboard.ReadWhiteboardImage(exPath, i, out boardCount, palette);
                    whiteboardBitmap = new WhiteboardBitmap(exPath, i, bitmap);
                    ship._bitmaps.Add(whiteboardBitmap);
                }
            }

            return ship;
        }
    }
}
