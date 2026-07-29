using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace AU_Whiteboard_Editor.Classes
{
    public static class Whiteboard
    {
        public const int Width = 384;
        public const int Height = 256;
        public const int BytesPerBoard = Width * Height;

        public static Bitmap ReadBitmap(
            string bpexPath,
            int boardIndex,
            out int boardCount)
        {
            if (boardIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(boardIndex));

            byte[] bpex = File.ReadAllBytes(bpexPath);

            if (bpex.Length == 0 || bpex.Length % BytesPerBoard != 0)
            {
                throw new InvalidDataException(
                    "The BPEX file size is not a multiple of 98,304 bytes.");
            }

            boardCount = bpex.Length / BytesPerBoard;

            if (boardIndex >= boardCount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(boardIndex),
                    "The file contains " + boardCount + " whiteboard(s).");
            }

            Bitmap bitmap = new Bitmap(
                Width,
                Height,
                PixelFormat.Format24bppRgb);

            Rectangle rectangle = new Rectangle(
                0,
                0,
                Width,
                Height);

            BitmapData bitmapData = bitmap.LockBits(
                rectangle,
                ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb);

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
                        byte ink = bpex[sourceRow + x];

                        // BPEX:
                        // 0x00 = blank
                        // 0xFF = black
                        //
                        // Normal bitmap:
                        // 0xFF = white
                        // 0x00 = black
                        byte colour = (byte)(255 - ink);

                        int pixelOffset = destinationRow + x * 3;

                        pixels[pixelOffset] = colour;       // Blue
                        pixels[pixelOffset + 1] = colour;   // Green
                        pixels[pixelOffset + 2] = colour;   // Red
                    }
                }

                System.Runtime.InteropServices.Marshal.Copy(
                    pixels,
                    0,
                    bitmapData.Scan0,
                    pixels.Length);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            return bitmap;
        }

        public static void WriteBitmap(string bpexPath, int boardIndex, Bitmap source, byte threshold = 128, bool createBackup = false)
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

            using Bitmap image = ResizeBitmap(source);

            int boardOffset = boardIndex * BytesPerBoard;

            for (int y = 0; y < Height; y++)
            {
                int storedY = Height - 1 - y;
                int rowOffset = boardOffset + storedY * Width;

                for (int x = 0; x < Width; x++)
                {
                    Color colour = image.GetPixel(x, y);
                    int luminance = (colour.R * 299 + colour.G * 587 + colour.B * 114) / 1000;
                    bool isInk = colour.A >= 128 && luminance < threshold;

                    bpex[rowOffset + x] = isInk ? (byte)255 : (byte)0;
                }
            }

            if (createBackup)
                File.Copy(bpexPath, bpexPath + ".bak", true);

            File.WriteAllBytes(bpexPath, bpex);
        }

        public static void WriteBitmap(
            string bpexPath,
            int boardIndex,
            string bitmapPath,
            byte threshold = 128,
            bool createBackup = false)
        {
            if (boardIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(boardIndex));

            byte[] bpex = File.ReadAllBytes(bpexPath);

            if (bpex.Length == 0 || bpex.Length % BytesPerBoard != 0)
            {
                throw new InvalidDataException(
                    "The BPEX file size is not a multiple of 98,304 bytes.");
            }

            int boardCount = bpex.Length / BytesPerBoard;

            if (boardIndex >= boardCount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(boardIndex),
                    "The file contains " + boardCount + " whiteboard(s).");
            }

            using (Bitmap source = new Bitmap(bitmapPath))
            using (Bitmap image = ResizeBitmap(source))
            {
                int boardOffset = boardIndex * BytesPerBoard;

                for (int y = 0; y < Height; y++)
                {
                    int storedY = Height - 1 - y;
                    int rowOffset = boardOffset + storedY * Width;

                    for (int x = 0; x < Width; x++)
                    {
                        Color colour = image.GetPixel(x, y);

                        int luminance =
                            colour.R * 299 +
                            colour.G * 587 +
                            colour.B * 114;

                        luminance /= 1000;

                        bool isInk =
                            colour.A >= 128 &&
                            luminance < threshold;

                        bpex[rowOffset + x] = isInk
                            ? (byte)255
                            : (byte)0;
                    }
                }
            }

            if (createBackup)
                File.Copy(bpexPath, bpexPath + ".bak", true);

            File.WriteAllBytes(bpexPath, bpex);
        }

        private static Bitmap ResizeBitmap(Bitmap source)
        {
            Bitmap result = new Bitmap(
                Width,
                Height,
                PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(result))
            {
                graphics.Clear(Color.White);
                graphics.CompositingMode = CompositingMode.SourceOver;
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                graphics.PixelOffsetMode = PixelOffsetMode.Half;

                graphics.DrawImage(
                    source,
                    new Rectangle(0, 0, Width, Height),
                    new Rectangle(0, 0, source.Width, source.Height),
                    GraphicsUnit.Pixel);
            }

            return result;
        }
    }
}

