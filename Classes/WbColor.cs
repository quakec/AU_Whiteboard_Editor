namespace AU_Whiteboard_Editor.Classes
{
    public class WbColor
    {
        public Color Color { get; set; }
        public byte WbValue { get; set; }
        public double Bias { get; set; }

        public WbColor(Color color, double bias)
        {
            Color = color;
            WbValue = GetValue(color);
            Bias = MapPaletteBias((1 - bias) / 100);
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

        private static double MapPaletteBias(double bias)
        {
            bias = Math.Clamp(bias, 0.0, 1.0);

            if (bias <= 0.5)
                return 0.05 + (bias / 0.5) * 0.95;

            return 1.0 + ((bias - 0.5) / 0.5) * 19.0;
        }
    }
}
