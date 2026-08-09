using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace AU_Whiteboard_Editor.Classes
{
    public static class Whiteboard
    {
        public enum ResizeMode
        {
            [Description("Stretch")]
            Stretch,
            [Description("Keep Aspect Ratio")]
            KeepAspectRatio,
            [Description("Crop")]
            Crop,
        }

        public enum DitherType
        {
            [Description("None")]
            None,

            [Description("Floyd–Steinberg")]
            Floyd_Steinberg,

            [Description("Floyd–Steinberg (Serpentine Scanning)")]
            Floyd_Steinberg_Serpentine_Scanning,

            [Description("Stucki")]
            Stucki,

            [Description("Jarvis–Judice–Ninke")]
            Jarvis_Judice_Ninke,

            [Description("Atkinson")]
            Atkinson,

            [Description("Sierra")]
            Sierra,

            [Description("Sierra Lite")]
            Sierra_Lite,

            [Description("Bayer 4×4")]
            Bayer_4x4,

            [Description("Bayer 8×8")]
            Bayer_8x8,
        }

        public enum ColorSpace
        {
            [Description("RGB")]
            Rgb,
            [Description("OKLab")]
            OkLab,
        }

        public const int Width = 384;
        public const int Height = 256;
        public const int DoubleWidth = Width /* * 2 */;
        public const int DoubleHeight = Height /* * 2 */;
        public const int BytesPerBoard = Width * Height;

        public static readonly byte[] PaletteValues =
        {
            0x00,
            0xFF,
            0xFE,
            0xFD,
            0xFC,
            0xFB,
            0xFA,
            0xF9,
        };

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

            //using Bitmap image = ResizeBitmap(source);
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
                    //bpex[rowOffset + x] = PaletteValues[paletteIndex];
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

        public static Bitmap CropBitmap(Bitmap source, Color backFill, ResizeMode resizeMode, double cropShift = 50)
        {
            cropShift = Math.Clamp(cropShift, 0, 100);

            int resultWidth = source.Width;
            int resultHeight = source.Height;

            Bitmap result = new Bitmap(resultWidth, resultHeight, PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(result))
            {
                graphics.Clear(backFill);
                graphics.CompositingMode = CompositingMode.SourceOver;
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                graphics.PixelOffsetMode = PixelOffsetMode.Half;

                Rectangle destination;

                switch (resizeMode)
                {
                    case ResizeMode.Stretch:
                        destination = new Rectangle(0, 0, resultWidth, resultHeight);
                        break;

                    case ResizeMode.KeepAspectRatio:
                        {
                            double targetAspect = (double)DoubleWidth / DoubleHeight;
                            double sourceAspect = (double)source.Width / source.Height;

                            int targetWidth;
                            int targetHeight;

                            if (sourceAspect > targetAspect)
                            {
                                targetHeight = source.Height;
                                targetWidth = (int)Math.Round(targetHeight * targetAspect);
                            }
                            else
                            {
                                targetWidth = source.Width;
                                targetHeight = (int)Math.Round(targetWidth / targetAspect);
                            }

                            int x = (resultWidth - targetWidth) / 2;
                            int y = (resultHeight - targetHeight) / 2;

                            double scale = Math.Min((double)targetWidth / source.Width, (double)targetHeight / source.Height);

                            int scaledWidth = (int)Math.Round(source.Width * scale);
                            int scaledHeight = (int)Math.Round(source.Height * scale);

                            x += (targetWidth - scaledWidth) / 2;
                            y += (targetHeight - scaledHeight) / 2;

                            destination = new Rectangle(x, y, scaledWidth, scaledHeight);
                            break;
                        }

                    case ResizeMode.Crop:
                        {
                            double targetAspect = (double)DoubleWidth / DoubleHeight;
                            double sourceAspect = (double)source.Width / source.Height;

                            int cropWidth;
                            int cropHeight;

                            if (sourceAspect > targetAspect)
                            {
                                cropHeight = source.Height;
                                cropWidth = (int)Math.Round(cropHeight * targetAspect);
                            }
                            else
                            {
                                cropWidth = source.Width;
                                cropHeight = (int)Math.Round(cropWidth / targetAspect);
                            }

                            double shift = cropShift / 100.0;

                            int sourceX = source.Width > cropWidth ? (int)Math.Round((source.Width - cropWidth) * shift) : 0;
                            int sourceY = source.Height > cropHeight ? (int)Math.Round((source.Height - cropHeight) * shift) : 0;

                            Rectangle sourceRectangle = new Rectangle(sourceX, sourceY, cropWidth, cropHeight);

                            graphics.DrawImage(source, new Rectangle(0, 0, resultWidth, resultHeight), sourceRectangle, GraphicsUnit.Pixel);

                            return result;
                        }

                    default:
                        throw new ArgumentOutOfRangeException(nameof(resizeMode));
                }

                graphics.DrawImage(source, destination, new Rectangle(0, 0, source.Width, source.Height), GraphicsUnit.Pixel);
            }

            return result;
        }

        public static Bitmap ResizeBitmap(Bitmap source, Color backFill, ResizeMode resizeMode, double cropShift = 50)
        {
            Bitmap result = new Bitmap(DoubleWidth, DoubleHeight, PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(result))
            {
                graphics.Clear(backFill);
                graphics.CompositingMode = CompositingMode.SourceOver;
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                graphics.PixelOffsetMode = PixelOffsetMode.Half;

                Rectangle destination;

                switch (resizeMode)
                {
                    case ResizeMode.Stretch:
                        destination = new Rectangle(0, 0, DoubleWidth, DoubleHeight);
                        break;

                    case ResizeMode.KeepAspectRatio:
                        {
                            double scale = Math.Min((double)DoubleWidth / source.Width, (double)DoubleHeight / source.Height);

                            int scaledWidth = (int)Math.Round(source.Width * scale);
                            int scaledHeight = (int)Math.Round(source.Height * scale);

                            int x = (DoubleWidth - scaledWidth) / 2;
                            int y = (DoubleHeight - scaledHeight) / 2;

                            destination = new Rectangle(x, y, scaledWidth, scaledHeight);
                            break;
                        }

                    case ResizeMode.Crop:
                        {
                            cropShift = Math.Clamp(cropShift, 0, 100);

                            double scale = Math.Max((double)DoubleWidth / source.Width, (double)DoubleHeight / source.Height);

                            int scaledWidth = (int)Math.Round(source.Width * scale);
                            int scaledHeight = (int)Math.Round(source.Height * scale);

                            double shift = cropShift / 100.0;

                            int x = scaledWidth > DoubleWidth ? -(int)Math.Round((scaledWidth - DoubleWidth) * shift) : 0;
                            int y = scaledHeight > DoubleHeight ? -(int)Math.Round((scaledHeight - DoubleHeight) * shift) : 0;

                            destination = new Rectangle(x, y, scaledWidth, scaledHeight);
                            break;
                        }

                    default:
                        throw new ArgumentOutOfRangeException(nameof(resizeMode));
                }

                graphics.DrawImage(source, destination, new Rectangle(0, 0, source.Width, source.Height), GraphicsUnit.Pixel);
            }

            return result;
        }

        public static Bitmap AdjustBrightness(Bitmap source, int brightness)
        {
            brightness = Math.Clamp(brightness, -50, 50);
            float offset = brightness / 100f;

            ColorMatrix matrix = new ColorMatrix(new float[][]
            {
                new float[] { 1, 0, 0, 0, 0 },
                new float[] { 0, 1, 0, 0, 0 },
                new float[] { 0, 0, 1, 0, 0 },
                new float[] { 0, 0, 0, 1, 0 },
                new float[] { offset, offset, offset, 0, 1 }
            });

            Bitmap result = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);

            using Graphics graphics = Graphics.FromImage(result);
            using ImageAttributes attributes = new ImageAttributes();

            attributes.SetColorMatrix(matrix);
            graphics.DrawImage(source, new Rectangle(0, 0, source.Width, source.Height), 0, 0, source.Width, source.Height, GraphicsUnit.Pixel, attributes);

            return result;
        }

        public static int FindNearestColourIndexOkLab(Color source, WbColor[] palette)
        {
            (double l1, double a1, double b1) = RgbToOklab(source);

            int nearestIndex = 0;
            double nearestDistance = double.MaxValue;

            for (int i = 0; i < palette.Length; i++)
            {
                Color colour = palette[i].Color;
                (double l2, double a2, double b2) = RgbToOklab(colour);

                double dl = l1 - l2;
                double da = a1 - a2;
                double db = b1 - b2;

                double distance = dl * dl + da * da + db * db;

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestIndex = i;
                }
            }

            return nearestIndex;
        }

        private static int FindNearestColourIndexRgb(Color source, WbColor[] palette)
        {
            if (source.A < 128)
                return 0;

            int nearestIndex = 0;
            double nearestDistance = double.MaxValue;

            for (int i = 0; i < palette.Length; i++)
            {
                Color colour = palette[i].Color;

                int dr = source.R - colour.R;
                int dg = source.G - colour.G;
                int db = source.B - colour.B;

                double distance = dr * dr * 0.299 + dg * dg * 0.587 + db * db * 0.114;

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestIndex = i;
                }
            }

            return nearestIndex;
        }

        private static (double L, double A, double B) RgbToOklab(Color colour)
        {
            double r = colour.R / 255.0;
            double g = colour.G / 255.0;
            double b = colour.B / 255.0;

            r = r <= 0.04045 ? r / 12.92 : Math.Pow((r + 0.055) / 1.055, 2.4);
            g = g <= 0.04045 ? g / 12.92 : Math.Pow((g + 0.055) / 1.055, 2.4);
            b = b <= 0.04045 ? b / 12.92 : Math.Pow((b + 0.055) / 1.055, 2.4);

            double l = 0.4122214708 * r + 0.5363325363 * g + 0.0514459929 * b;
            double m = 0.2119034982 * r + 0.6806995451 * g + 0.1073969566 * b;
            double s = 0.0883024619 * r + 0.2817188376 * g + 0.6299787005 * b;

            double l_ = Math.Cbrt(l);
            double m_ = Math.Cbrt(m);
            double s_ = Math.Cbrt(s);

            double L = 0.2104542553 * l_ + 0.7936177850 * m_ - 0.0040720468 * s_;
            double A = 1.9779984951 * l_ - 2.4285922050 * m_ + 0.4505937099 * s_;
            double B = 0.0259040371 * l_ + 0.7827717662 * m_ - 0.8086757660 * s_;

            return (L, A, B);
        }

        private static Color OklabToRgb(double L, double A, double B)
        {
            double l_ = L + 0.3963377774 * A + 0.2158037573 * B;
            double m_ = L - 0.1055613458 * A - 0.0638541728 * B;
            double s_ = L - 0.0894841775 * A - 1.2914855480 * B;

            double l = l_ * l_ * l_;
            double m = m_ * m_ * m_;
            double s = s_ * s_ * s_;

            double r = +4.0767416621 * l - 3.3077115913 * m + 0.2309699292 * s;
            double g = -1.2684380046 * l + 2.6097574011 * m - 0.3413193965 * s;
            double b = -0.0041960863 * l - 0.7034186147 * m + 1.7076147010 * s;

            r = r <= 0.0031308 ? 12.92 * r : 1.055 * Math.Pow(r, 1.0 / 2.4) - 0.055;
            g = g <= 0.0031308 ? 12.92 * g : 1.055 * Math.Pow(g, 1.0 / 2.4) - 0.055;
            b = b <= 0.0031308 ? 12.92 * b : 1.055 * Math.Pow(b, 1.0 / 2.4) - 0.055;

            int R = (int)Math.Round(Math.Clamp(r, 0.0, 1.0) * 255.0);
            int G = (int)Math.Round(Math.Clamp(g, 0.0, 1.0) * 255.0);
            int B2 = (int)Math.Round(Math.Clamp(b, 0.0, 1.0) * 255.0);

            return Color.FromArgb(255, R, G, B2);
        }

        public static Bitmap ApplyPalette(Bitmap source, WbColor[] palette, ColorSpace colorSpace)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (palette == null || palette.Length == 0)
                throw new ArgumentException("Palette cannot be null or empty.", nameof(palette));

            Bitmap input = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(input))
                graphics.DrawImageUnscaled(source, 0, 0);

            Bitmap result = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);

            Rectangle rect = new Rectangle(0, 0, source.Width, source.Height);

            BitmapData inputData = input.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData resultData = result.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            try
            {
                int inputStride = inputData.Stride;
                int resultStride = resultData.Stride;

                unsafe
                {
                    byte* inputBase = (byte*)inputData.Scan0;
                    byte* resultBase = (byte*)resultData.Scan0;

                    for (int y = 0; y < source.Height; y++)
                    {
                        byte* inputRow = inputBase + y * inputStride;
                        byte* resultRow = resultBase + y * resultStride;

                        for (int x = 0; x < source.Width; x++)
                        {
                            int offset = x * 4;

                            byte b = inputRow[offset];
                            byte g = inputRow[offset + 1];
                            byte r = inputRow[offset + 2];
                            byte a = inputRow[offset + 3];

                            Color sourceColour = Color.FromArgb(a, r, g, b);
                            int paletteIndex = 0;
                            switch (colorSpace)
                            {
                                case ColorSpace.Rgb:
                                    paletteIndex = FindNearestColourIndexRgb(sourceColour, palette);
                                    break;
                                case ColorSpace.OkLab:
                                    paletteIndex = FindNearestColourIndexOkLab(sourceColour, palette);
                                    break;
                                default:
                                    break;
                            }
                            Color paletteColour = palette[paletteIndex].Color;

                            resultRow[offset] = paletteColour.B;
                            resultRow[offset + 1] = paletteColour.G;
                            resultRow[offset + 2] = paletteColour.R;
                            resultRow[offset + 3] = 255;
                        }
                    }
                }
            }
            finally
            {
                input.UnlockBits(inputData);
                result.UnlockBits(resultData);
                input.Dispose();
            }

            return result;
        }

        public static unsafe Bitmap ApplyPaletteDithered(Bitmap source, WbColor[] palette, DitherType ditherType, ColorSpace colorSpace)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (palette == null || palette.Length == 0)
                throw new ArgumentException("Palette cannot be null or empty.", nameof(palette));

            if (source.PixelFormat != PixelFormat.Format32bppArgb)
                throw new ArgumentException("Source bitmap must be Format32bppArgb.", nameof(source));

            int width = source.Width;
            int height = source.Height;

            Bitmap result = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            double[,] error1 = new double[width, height];
            double[,] error2 = new double[width, height];
            double[,] error3 = new double[width, height];

            int[,] bayer4x4 =
            {
        {  0,  8,  2, 10 },
        { 12,  4, 14,  6 },
        {  3, 11,  1,  9 },
        { 15,  7, 13,  5 }
    };

            int[,] bayer8x8 =
            {
        {  0, 32,  8, 40,  2, 34, 10, 42 },
        { 48, 16, 56, 24, 50, 18, 58, 26 },
        { 12, 44,  4, 36, 14, 46,  6, 38 },
        { 60, 28, 52, 20, 62, 30, 54, 22 },
        {  3, 35, 11, 43,  1, 33,  9, 41 },
        { 51, 19, 59, 27, 49, 17, 57, 25 },
        { 15, 47,  7, 39, 13, 45,  5, 37 },
        { 63, 31, 55, 23, 61, 29, 53, 21 }
    };

            Rectangle rectangle = new Rectangle(0, 0, width, height);

            BitmapData sourceData = source.LockBits(rectangle, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData resultData = result.LockBits(rectangle, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            try
            {
                byte* sourceBase = (byte*)sourceData.Scan0;
                byte* resultBase = (byte*)resultData.Scan0;

                for (int y = 0; y < height; y++)
                {
                    bool serpentine = ditherType == DitherType.Floyd_Steinberg_Serpentine_Scanning;
                    int direction = serpentine && (y & 1) != 0 ? -1 : 1;

                    int startX = direction == 1 ? 0 : width - 1;
                    int endX = direction == 1 ? width : -1;

                    byte* sourceRow = sourceBase + y * sourceData.Stride;
                    byte* resultRow = resultBase + y * resultData.Stride;

                    for (int x = startX; x != endX; x += direction)
                    {
                        int offset = x * 4;

                        byte sourceB = sourceRow[offset];
                        byte sourceG = sourceRow[offset + 1];
                        byte sourceR = sourceRow[offset + 2];
                        byte sourceA = sourceRow[offset + 3];

                        if (sourceA < 128)
                        {
                            Color transparentColour = palette[0].Color;

                            resultRow[offset] = transparentColour.B;
                            resultRow[offset + 1] = transparentColour.G;
                            resultRow[offset + 2] = transparentColour.R;
                            resultRow[offset + 3] = 255;

                            continue;
                        }

                        Color sourceColour = Color.FromArgb(sourceA, sourceR, sourceG, sourceB);

                        Color adjustedColour;
                        double value1;
                        double value2;
                        double value3;

                        switch (colorSpace)
                        {
                            case ColorSpace.Rgb:
                                value1 = sourceR + error1[x, y];
                                value2 = sourceG + error2[x, y];
                                value3 = sourceB + error3[x, y];

                                if (ditherType == DitherType.Bayer_4x4)
                                {
                                    double threshold = ((bayer4x4[y % 4, x % 4] + 0.5) / 16.0 - 0.5) * 64.0;

                                    value1 += threshold;
                                    value2 += threshold;
                                    value3 += threshold;
                                }
                                else if (ditherType == DitherType.Bayer_8x8)
                                {
                                    double threshold = ((bayer8x8[y % 8, x % 8] + 0.5) / 64.0 - 0.5) * 64.0;

                                    value1 += threshold;
                                    value2 += threshold;
                                    value3 += threshold;
                                }

                                adjustedColour = Color.FromArgb(255, Clamp(value1), Clamp(value2), Clamp(value3));
                                break;

                            case ColorSpace.OkLab:
                                (double l, double a, double b) = RgbToOklab(sourceColour);

                                value1 = l + error1[x, y];
                                value2 = a + error2[x, y];
                                value3 = b + error3[x, y];

                                adjustedColour = OklabToRgb(value1, value2, value3);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(colorSpace));
                        }

                        int paletteIndex;

                        switch (colorSpace)
                        {
                            case ColorSpace.Rgb:
                                paletteIndex = FindNearestColourIndexRgb(adjustedColour, palette);
                                break;

                            case ColorSpace.OkLab:
                                paletteIndex = FindNearestColourIndexOkLab(adjustedColour, palette);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(colorSpace));
                        }

                        Color paletteColour = palette[paletteIndex].Color;

                        resultRow[offset] = paletteColour.B;
                        resultRow[offset + 1] = paletteColour.G;
                        resultRow[offset + 2] = paletteColour.R;
                        resultRow[offset + 3] = 255;

                        double errorValue1;
                        double errorValue2;
                        double errorValue3;

                        switch (colorSpace)
                        {
                            case ColorSpace.Rgb:
                                errorValue1 = adjustedColour.R - paletteColour.R;
                                errorValue2 = adjustedColour.G - paletteColour.G;
                                errorValue3 = adjustedColour.B - paletteColour.B;
                                break;

                            case ColorSpace.OkLab:
                                (double adjustedL, double adjustedA, double adjustedB) = RgbToOklab(adjustedColour);
                                (double paletteL, double paletteA, double paletteB) = RgbToOklab(paletteColour);

                                errorValue1 = adjustedL - paletteL;
                                errorValue2 = adjustedA - paletteA;
                                errorValue3 = adjustedB - paletteB;
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(colorSpace));
                        }

                        switch (ditherType)
                        {
                            case DitherType.Floyd_Steinberg:
                            case DitherType.Floyd_Steinberg_Serpentine_Scanning:
                                AddDitherError(error1, error2, error3, x + direction, y, width, height, errorValue1, errorValue2, errorValue3, 7.0 / 16.0);
                                AddDitherError(error1, error2, error3, x - direction, y + 1, width, height, errorValue1, errorValue2, errorValue3, 3.0 / 16.0);
                                AddDitherError(error1, error2, error3, x, y + 1, width, height, errorValue1, errorValue2, errorValue3, 5.0 / 16.0);
                                AddDitherError(error1, error2, error3, x + direction, y + 1, width, height, errorValue1, errorValue2, errorValue3, 1.0 / 16.0);
                                break;

                            case DitherType.Stucki:
                                AddDitherError(error1, error2, error3, x + 1, y, width, height, errorValue1, errorValue2, errorValue3, 8.0 / 42.0);
                                AddDitherError(error1, error2, error3, x + 2, y, width, height, errorValue1, errorValue2, errorValue3, 4.0 / 42.0);

                                AddDitherError(error1, error2, error3, x - 2, y + 1, width, height, errorValue1, errorValue2, errorValue3, 2.0 / 42.0);
                                AddDitherError(error1, error2, error3, x - 1, y + 1, width, height, errorValue1, errorValue2, errorValue3, 4.0 / 42.0);
                                AddDitherError(error1, error2, error3, x, y + 1, width, height, errorValue1, errorValue2, errorValue3, 8.0 / 42.0);
                                AddDitherError(error1, error2, error3, x + 1, y + 1, width, height, errorValue1, errorValue2, errorValue3, 4.0 / 42.0);
                                AddDitherError(error1, error2, error3, x + 2, y + 1, width, height, errorValue1, errorValue2, errorValue3, 2.0 / 42.0);

                                AddDitherError(error1, error2, error3, x - 2, y + 2, width, height, errorValue1, errorValue2, errorValue3, 1.0 / 42.0);
                                AddDitherError(error1, error2, error3, x - 1, y + 2, width, height, errorValue1, errorValue2, errorValue3, 2.0 / 42.0);
                                AddDitherError(error1, error2, error3, x, y + 2, width, height, errorValue1, errorValue2, errorValue3, 4.0 / 42.0);
                                AddDitherError(error1, error2, error3, x + 1, y + 2, width, height, errorValue1, errorValue2, errorValue3, 2.0 / 42.0);
                                AddDitherError(error1, error2, error3, x + 2, y + 2, width, height, errorValue1, errorValue2, errorValue3, 1.0 / 42.0);
                                break;

                            case DitherType.Jarvis_Judice_Ninke:
                                AddDitherError(error1, error2, error3, x + 1, y, width, height, errorValue1, errorValue2, errorValue3, 7.0 / 48.0);
                                AddDitherError(error1, error2, error3, x + 2, y, width, height, errorValue1, errorValue2, errorValue3, 5.0 / 48.0);

                                AddDitherError(error1, error2, error3, x - 2, y + 1, width, height, errorValue1, errorValue2, errorValue3, 3.0 / 48.0);
                                AddDitherError(error1, error2, error3, x - 1, y + 1, width, height, errorValue1, errorValue2, errorValue3, 5.0 / 48.0);
                                AddDitherError(error1, error2, error3, x, y + 1, width, height, errorValue1, errorValue2, errorValue3, 7.0 / 48.0);
                                AddDitherError(error1, error2, error3, x + 1, y + 1, width, height, errorValue1, errorValue2, errorValue3, 5.0 / 48.0);
                                AddDitherError(error1, error2, error3, x + 2, y + 1, width, height, errorValue1, errorValue2, errorValue3, 3.0 / 48.0);

                                AddDitherError(error1, error2, error3, x - 2, y + 2, width, height, errorValue1, errorValue2, errorValue3, 1.0 / 48.0);
                                AddDitherError(error1, error2, error3, x - 1, y + 2, width, height, errorValue1, errorValue2, errorValue3, 3.0 / 48.0);
                                AddDitherError(error1, error2, error3, x, y + 2, width, height, errorValue1, errorValue2, errorValue3, 5.0 / 48.0);
                                AddDitherError(error1, error2, error3, x + 1, y + 2, width, height, errorValue1, errorValue2, errorValue3, 3.0 / 48.0);
                                AddDitherError(error1, error2, error3, x + 2, y + 2, width, height, errorValue1, errorValue2, errorValue3, 1.0 / 48.0);
                                break;

                            case DitherType.Atkinson:
                                AddDitherError(error1, error2, error3, x + 1, y, width, height, errorValue1, errorValue2, errorValue3, 1.0 / 8.0);
                                AddDitherError(error1, error2, error3, x + 2, y, width, height, errorValue1, errorValue2, errorValue3, 1.0 / 8.0);
                                AddDitherError(error1, error2, error3, x - 1, y + 1, width, height, errorValue1, errorValue2, errorValue3, 1.0 / 8.0);
                                AddDitherError(error1, error2, error3, x, y + 1, width, height, errorValue1, errorValue2, errorValue3, 1.0 / 8.0);
                                AddDitherError(error1, error2, error3, x + 1, y + 1, width, height, errorValue1, errorValue2, errorValue3, 1.0 / 8.0);
                                AddDitherError(error1, error2, error3, x, y + 2, width, height, errorValue1, errorValue2, errorValue3, 1.0 / 8.0);
                                break;

                            case DitherType.Sierra:
                                AddDitherError(error1, error2, error3, x + 1, y, width, height, errorValue1, errorValue2, errorValue3, 5.0 / 32.0);
                                AddDitherError(error1, error2, error3, x + 2, y, width, height, errorValue1, errorValue2, errorValue3, 3.0 / 32.0);

                                AddDitherError(error1, error2, error3, x - 2, y + 1, width, height, errorValue1, errorValue2, errorValue3, 2.0 / 32.0);
                                AddDitherError(error1, error2, error3, x - 1, y + 1, width, height, errorValue1, errorValue2, errorValue3, 4.0 / 32.0);
                                AddDitherError(error1, error2, error3, x, y + 1, width, height, errorValue1, errorValue2, errorValue3, 5.0 / 32.0);
                                AddDitherError(error1, error2, error3, x + 1, y + 1, width, height, errorValue1, errorValue2, errorValue3, 4.0 / 32.0);
                                AddDitherError(error1, error2, error3, x + 2, y + 1, width, height, errorValue1, errorValue2, errorValue3, 2.0 / 32.0);

                                AddDitherError(error1, error2, error3, x - 1, y + 2, width, height, errorValue1, errorValue2, errorValue3, 2.0 / 32.0);
                                AddDitherError(error1, error2, error3, x, y + 2, width, height, errorValue1, errorValue2, errorValue3, 3.0 / 32.0);
                                AddDitherError(error1, error2, error3, x + 1, y + 2, width, height, errorValue1, errorValue2, errorValue3, 2.0 / 32.0);
                                break;

                            case DitherType.Sierra_Lite:
                                AddDitherError(error1, error2, error3, x + 1, y, width, height, errorValue1, errorValue2, errorValue3, 2.0 / 4.0);
                                AddDitherError(error1, error2, error3, x - 1, y + 1, width, height, errorValue1, errorValue2, errorValue3, 1.0 / 4.0);
                                AddDitherError(error1, error2, error3, x, y + 1, width, height, errorValue1, errorValue2, errorValue3, 1.0 / 4.0);
                                break;

                            case DitherType.Bayer_4x4:
                            case DitherType.Bayer_8x8:
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(ditherType));
                        }
                    }
                }
            }
            finally
            {
                source.UnlockBits(sourceData);
                result.UnlockBits(resultData);
            }

            return result;
        }

        /*
        public static Bitmap ApplyPaletteDithered(Bitmap source, WbColor[] palette, DitherType ditherType, ColorSpace colorSpace)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (palette == null || palette.Length == 0)
                throw new ArgumentException("Palette cannot be null or empty.", nameof(palette));

            int width = source.Width;
            int height = source.Height;

            Bitmap result = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            double[,] error1 = new double[width, height];
            double[,] error2 = new double[width, height];
            double[,] error3 = new double[width, height];

            int[,] bayer4x4 =
            {
                {  0,  8,  2, 10 },
                { 12,  4, 14,  6 },
                {  3, 11,  1,  9 },
                { 15,  7, 13,  5 }
            };

            int[,] bayer8x8 =
            {
                {  0, 32,  8, 40,  2, 34, 10, 42 },
                { 48, 16, 56, 24, 50, 18, 58, 26 },
                { 12, 44,  4, 36, 14, 46,  6, 38 },
                { 60, 28, 52, 20, 62, 30, 54, 22 },
                {  3, 35, 11, 43,  1, 33,  9, 41 },
                { 51, 19, 59, 27, 49, 17, 57, 25 },
                { 15, 47,  7, 39, 13, 45,  5, 37 },
                { 63, 31, 55, 23, 61, 29, 53, 21 }
            };

            for (int y = 0; y < height; y++)
            {
                bool serpentine = ditherType == DitherType.Floyd_Steinberg_Serpentine_Scanning;
                int direction = serpentine && (y & 1) != 0 ? -1 : 1;

                int startX = direction == 1 ? 0 : width - 1;
                int endX = direction == 1 ? width : -1;

                for (int x = startX; x != endX; x += direction)
                {
                    Color sourceColour = source.GetPixel(x, y);

                    if (sourceColour.A < 128)
                    {
                        result.SetPixel(x, y, palette[0].Color);
                        continue;
                    }

                    Color adjustedColour;
                    double value1;
                    double value2;
                    double value3;

                    switch (colorSpace)
                    {
                        case ColorSpace.Rgb:
                            value1 = sourceColour.R + error1[x, y];
                            value2 = sourceColour.G + error2[x, y];
                            value3 = sourceColour.B + error3[x, y];

                            if (ditherType == DitherType.Bayer_4x4)
                            {
                                double threshold = ((bayer4x4[y % 4, x % 4] + 0.5) / 16.0 - 0.5) * 64.0;

                                value1 += threshold;
                                value2 += threshold;
                                value3 += threshold;
                            }
                            else if (ditherType == DitherType.Bayer_8x8)
                            {
                                double threshold = ((bayer8x8[y % 8, x % 8] + 0.5) / 64.0 - 0.5) * 64.0;

                                value1 += threshold;
                                value2 += threshold;
                                value3 += threshold;
                            }

                            adjustedColour = Color.FromArgb(255, Clamp(value1), Clamp(value2), Clamp(value3));
                            break;

                        case ColorSpace.OkLab:
                            (double l, double a, double b) = RgbToOklab(sourceColour);

                            value1 = l + error1[x, y];
                            value2 = a + error2[x, y];
                            value3 = b + error3[x, y];

                            adjustedColour = OklabToRgb(value1, value2, value3);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(colorSpace));
                    }

                    int paletteIndex;

                    switch (colorSpace)
                    {
                        case ColorSpace.Rgb:
                            paletteIndex = FindNearestColourIndexRgb(adjustedColour, palette);
                            break;

                        case ColorSpace.OkLab:
                            paletteIndex = FindNearestColourIndexOkLab(adjustedColour, palette);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(colorSpace));
                    }

                    Color paletteColour = palette[paletteIndex].Color;

                    result.SetPixel(x, y, paletteColour);

                    double errorValue1;
                    double errorValue2;
                    double errorValue3;

                    switch (colorSpace)
                    {
                        case ColorSpace.Rgb:
                            errorValue1 = adjustedColour.R - paletteColour.R;
                            errorValue2 = adjustedColour.G - paletteColour.G;
                            errorValue3 = adjustedColour.B - paletteColour.B;
                            break;

                        case ColorSpace.OkLab:
                            (double adjustedL, double adjustedA, double adjustedB) = RgbToOklab(adjustedColour);
                            (double paletteL, double paletteA, double paletteB) = RgbToOklab(paletteColour);

                            errorValue1 = adjustedL - paletteL;
                            errorValue2 = adjustedA - paletteA;
                            errorValue3 = adjustedB - paletteB;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(colorSpace));
                    }

                    switch (ditherType)
                    {
                        case DitherType.Floyd_Steinberg:
                        case DitherType.Floyd_Steinberg_Serpentine_Scanning:
                            AddDitherError(error1, error2, error3, x + direction, y, width, height, errorValue1, errorValue2, errorValue3, 7.0 / 16.0);
                            AddDitherError(error1, error2, error3, x - direction, y + 1, width, height, errorValue1, errorValue2, errorValue3, 3.0 / 16.0);
                            AddDitherError(error1, error2, error3, x, y + 1, width, height, errorValue1, errorValue2, errorValue3, 5.0 / 16.0);
                            AddDitherError(error1, error2, error3, x + direction, y + 1, width, height, errorValue1, errorValue2, errorValue3, 1.0 / 16.0);
                            break;

                        case DitherType.Stucki:
                            AddDitherError(error1, error2, error3, x + 1, y, width, height, errorValue1, errorValue2, errorValue3, 8.0 / 42.0);
                            AddDitherError(error1, error2, error3, x + 2, y, width, height, errorValue1, errorValue2, errorValue3, 4.0 / 42.0);

                            AddDitherError(error1, error2, error3, x - 2, y + 1, width, height, errorValue1, errorValue2, errorValue3, 2.0 / 42.0);
                            AddDitherError(error1, error2, error3, x - 1, y + 1, width, height, errorValue1, errorValue2, errorValue3, 4.0 / 42.0);
                            AddDitherError(error1, error2, error3, x, y + 1, width, height, errorValue1, errorValue2, errorValue3, 8.0 / 42.0);
                            AddDitherError(error1, error2, error3, x + 1, y + 1, width, height, errorValue1, errorValue2, errorValue3, 4.0 / 42.0);
                            AddDitherError(error1, error2, error3, x + 2, y + 1, width, height, errorValue1, errorValue2, errorValue3, 2.0 / 42.0);

                            AddDitherError(error1, error2, error3, x - 2, y + 2, width, height, errorValue1, errorValue2, errorValue3, 1.0 / 42.0);
                            AddDitherError(error1, error2, error3, x - 1, y + 2, width, height, errorValue1, errorValue2, errorValue3, 2.0 / 42.0);
                            AddDitherError(error1, error2, error3, x, y + 2, width, height, errorValue1, errorValue2, errorValue3, 4.0 / 42.0);
                            AddDitherError(error1, error2, error3, x + 1, y + 2, width, height, errorValue1, errorValue2, errorValue3, 2.0 / 42.0);
                            AddDitherError(error1, error2, error3, x + 2, y + 2, width, height, errorValue1, errorValue2, errorValue3, 1.0 / 42.0);
                            break;

                        case DitherType.Jarvis_Judice_Ninke:
                            AddDitherError(error1, error2, error3, x + 1, y, width, height, errorValue1, errorValue2, errorValue3, 7.0 / 48.0);
                            AddDitherError(error1, error2, error3, x + 2, y, width, height, errorValue1, errorValue2, errorValue3, 5.0 / 48.0);

                            AddDitherError(error1, error2, error3, x - 2, y + 1, width, height, errorValue1, errorValue2, errorValue3, 3.0 / 48.0);
                            AddDitherError(error1, error2, error3, x - 1, y + 1, width, height, errorValue1, errorValue2, errorValue3, 5.0 / 48.0);
                            AddDitherError(error1, error2, error3, x, y + 1, width, height, errorValue1, errorValue2, errorValue3, 7.0 / 48.0);
                            AddDitherError(error1, error2, error3, x + 1, y + 1, width, height, errorValue1, errorValue2, errorValue3, 5.0 / 48.0);
                            AddDitherError(error1, error2, error3, x + 2, y + 1, width, height, errorValue1, errorValue2, errorValue3, 3.0 / 48.0);

                            AddDitherError(error1, error2, error3, x - 2, y + 2, width, height, errorValue1, errorValue2, errorValue3, 1.0 / 48.0);
                            AddDitherError(error1, error2, error3, x - 1, y + 2, width, height, errorValue1, errorValue2, errorValue3, 3.0 / 48.0);
                            AddDitherError(error1, error2, error3, x, y + 2, width, height, errorValue1, errorValue2, errorValue3, 5.0 / 48.0);
                            AddDitherError(error1, error2, error3, x + 1, y + 2, width, height, errorValue1, errorValue2, errorValue3, 3.0 / 48.0);
                            AddDitherError(error1, error2, error3, x + 2, y + 2, width, height, errorValue1, errorValue2, errorValue3, 1.0 / 48.0);
                            break;

                        case DitherType.Atkinson:
                            AddDitherError(error1, error2, error3, x + 1, y, width, height, errorValue1, errorValue2, errorValue3, 1.0 / 8.0);
                            AddDitherError(error1, error2, error3, x + 2, y, width, height, errorValue1, errorValue2, errorValue3, 1.0 / 8.0);
                            AddDitherError(error1, error2, error3, x - 1, y + 1, width, height, errorValue1, errorValue2, errorValue3, 1.0 / 8.0);
                            AddDitherError(error1, error2, error3, x, y + 1, width, height, errorValue1, errorValue2, errorValue3, 1.0 / 8.0);
                            AddDitherError(error1, error2, error3, x + 1, y + 1, width, height, errorValue1, errorValue2, errorValue3, 1.0 / 8.0);
                            AddDitherError(error1, error2, error3, x, y + 2, width, height, errorValue1, errorValue2, errorValue3, 1.0 / 8.0);
                            break;

                        case DitherType.Sierra:
                            AddDitherError(error1, error2, error3, x + 1, y, width, height, errorValue1, errorValue2, errorValue3, 5.0 / 32.0);
                            AddDitherError(error1, error2, error3, x + 2, y, width, height, errorValue1, errorValue2, errorValue3, 3.0 / 32.0);

                            AddDitherError(error1, error2, error3, x - 2, y + 1, width, height, errorValue1, errorValue2, errorValue3, 2.0 / 32.0);
                            AddDitherError(error1, error2, error3, x - 1, y + 1, width, height, errorValue1, errorValue2, errorValue3, 4.0 / 32.0);
                            AddDitherError(error1, error2, error3, x, y + 1, width, height, errorValue1, errorValue2, errorValue3, 5.0 / 32.0);
                            AddDitherError(error1, error2, error3, x + 1, y + 1, width, height, errorValue1, errorValue2, errorValue3, 4.0 / 32.0);
                            AddDitherError(error1, error2, error3, x + 2, y + 1, width, height, errorValue1, errorValue2, errorValue3, 2.0 / 32.0);

                            AddDitherError(error1, error2, error3, x - 1, y + 2, width, height, errorValue1, errorValue2, errorValue3, 2.0 / 32.0);
                            AddDitherError(error1, error2, error3, x, y + 2, width, height, errorValue1, errorValue2, errorValue3, 3.0 / 32.0);
                            AddDitherError(error1, error2, error3, x + 1, y + 2, width, height, errorValue1, errorValue2, errorValue3, 2.0 / 32.0);
                            break;

                        case DitherType.Sierra_Lite:
                            AddDitherError(error1, error2, error3, x + 1, y, width, height, errorValue1, errorValue2, errorValue3, 2.0 / 4.0);
                            AddDitherError(error1, error2, error3, x - 1, y + 1, width, height, errorValue1, errorValue2, errorValue3, 1.0 / 4.0);
                            AddDitherError(error1, error2, error3, x, y + 1, width, height, errorValue1, errorValue2, errorValue3, 1.0 / 4.0);
                            break;

                        case DitherType.Bayer_4x4:
                        case DitherType.Bayer_8x8:
                            // Ordered dithering doesn't diffuse error.
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(ditherType));
                    }
                }
            }

            return result;
        }
        */

        private static void AddDitherError(double[,] errorR, double[,] errorG, double[,] errorB, int x, int y, int width, int height, double r, double g, double b, double amount)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
                return;

            errorR[x, y] += r * amount;
            errorG[x, y] += g * amount;
            errorB[x, y] += b * amount;
        }

        private static int Clamp(double value)
        {
            return (int)Math.Clamp(Math.Round(value), 0, 255);
        }
    }
}
