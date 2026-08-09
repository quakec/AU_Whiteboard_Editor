namespace AU_Whiteboard_Editor.Classes
{
    public class WbColor
    {
        public Color Color { get; set; }
        public byte WbValue { get; set; }
        public WbColor(Color color)
        {
            Color = color;
            WbValue = GetValue(color);
        }

        private byte GetValue(Color color)
        {
            if (color == Color.White)
                return 0x00;
            else if(color == Color.Black)
                return 0xFF;
            else if (color == Color.Red)
                return 0xFE;
            else if (color == Color.Green)
                return 0xFD;
            else if (color == Color.Blue)
                return 0xFC;
            else if (color == Color.Yellow)
                return 0xFB;
            else if (color == Color.Magenta)
                return 0xFA;
            else if (color == Color.Cyan)
                return 0xF9;

           throw new Exception("Unsupported whiteboard colour.");
        }
    }
}
