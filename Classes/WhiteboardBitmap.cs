
namespace AU_Whiteboard_Editor.Classes
{
    public class WhiteboardBitmap
    {
        public string FilePath { get; set; }
        public int ImageIndex { get; set; }
        public Bitmap Bitmap { get; set; }
        public WhiteboardBitmap(string filePath, int imageIndex, Bitmap bitmap)
        {
            FilePath = filePath;
            ImageIndex = imageIndex;
            Bitmap = bitmap;
        }
    }
}
