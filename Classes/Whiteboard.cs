using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace AU_Whiteboard_Editor.Classes
{
    public static class Whiteboard
    {
        public const int Width = 384;
        public const int Height = 256;
        public const int BytesPerBoard = Width * Height;

        public static Bitmap ReadWhiteboardImage(string bpexPath, int boardIndex, out int boardCount, WbColor[] palette)
        {
            if (boardIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(boardIndex));

            byte[] bpex = File.ReadAllBytes(bpexPath);

            if (bpex.Length == 0 || bpex.Length % BytesPerBoard != 0)
                throw new InvalidDataException("The BPEX file size is not a multiple of 98,304 bytes.");

            boardCount = bpex.Length / BytesPerBoard;

            if (boardIndex >= boardCount)
                throw new ArgumentOutOfRangeException(nameof(boardIndex), "The file contains " + boardCount + " whiteboard(s).");

            Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
            Rectangle rectangle = new Rectangle(0, 0, Width, Height);
            BitmapData bitmapData = bitmap.LockBits(rectangle, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            try
            {
                int boardOffset = boardIndex * BytesPerBoard;
                int stride = bitmapData.Stride;
                byte[] pixels = new byte[stride * Height];

                for (int y = 0; y < Height; y++)
                {
                    int storedY = Height - 1 - y;
                    int sourceRow = boardOffset + storedY * Width;
                    int destinationRow = y * stride;

                    for (int x = 0; x < Width; x++)
                    {
                        byte value = bpex[sourceRow + x];
                        Color colour = GetColour(value, palette);

                        int pixelOffset = destinationRow + x * 3;

                        pixels[pixelOffset] = colour.B;
                        pixels[pixelOffset + 1] = colour.G;
                        pixels[pixelOffset + 2] = colour.R;
                    }
                }

                Marshal.Copy(pixels, 0, bitmapData.Scan0, pixels.Length);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            return bitmap;
        }

        public static void WriteWhiteboardImage(string bpexPath, int boardIndex, Bitmap source, WbColor[] palette, bool createBackup = false)
        {
            if (boardIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(boardIndex));

            if (source == null)
                throw new ArgumentNullException(nameof(source));

            byte[] bpex = File.ReadAllBytes(bpexPath);

            if (bpex.Length == 0 || bpex.Length % BytesPerBoard != 0)
                throw new InvalidDataException("The BPEX file size is not a multiple of 98,304 bytes.");

            int boardCount = bpex.Length / BytesPerBoard;

            if (boardIndex >= boardCount)
                throw new ArgumentOutOfRangeException(nameof(boardIndex), "The file contains " + boardCount + " whiteboard(s).");

            using Bitmap image = new Bitmap(source);

            int boardOffset = boardIndex * BytesPerBoard;

            for (int y = 0; y < Height; y++)
            {
                int storedY = Height - 1 - y;
                int rowOffset = boardOffset + storedY * Width;

                for (int x = 0; x < Width; x++)
                {
                    Color colour = image.GetPixel(x, y);
                    int paletteIndex = FindExactColourIndex(colour, palette);
                    bpex[rowOffset + x] = palette[paletteIndex].WbValue;
                }
            }

            if (createBackup)
                File.Copy(bpexPath, bpexPath + ".bak", true);

            File.WriteAllBytes(bpexPath, bpex);
        }

        public static int FindExactColourIndex(Color colour, WbColor[] palette)
        {
            for (int i = 0; i < palette.Length; i++)
            {
                if (palette[i].Color.ToArgb() == colour.ToArgb())
                    return i;
            }

            return -1;
        }

        private static Color GetColour(byte value, WbColor[] palette)
        {
            WbColor color = palette.FirstOrDefault(o => o.WbValue == value);
            if (color == null)
                return Color.White;
            return color.Color;
        }
    }
}
