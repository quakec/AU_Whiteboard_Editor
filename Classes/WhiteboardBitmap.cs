using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AU_Whiteboard_Editor.Classes
{
    public class WhiteboardBitmap
    {
        public string FilePath { get; set; }
        public Bitmap Bitmap { get; set; }
        public WhiteboardBitmap(string filePath, Bitmap bitmap)
        {
            FilePath = filePath;
            Bitmap = bitmap;
        }
    }
}
