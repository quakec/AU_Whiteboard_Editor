using AU_Whiteboard_Editor.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AU_Whiteboard_Editor.Classes.Whiteboard;

namespace AU_Whiteboard_Editor.Helpers
{
    public static class ImageProcessing
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
            [Description("RGB (Luma Weighted)")]
            Rgb,

            [Description("RGB (Euclidean)")]
            RgbEuclidean,

            [Description("Linear RGB")]
            LinearRgb,

            [Description("CIE XYZ")]
            CieXyz,

            [Description("CIE Lab")]
            CieLab,

            [Description("OKLab")]
            OkLab,

            [Description("OKLCh (Hue Aware)")]
            OkLch,

            [Description("YCbCr")]
            YCbCr,

            [Description("AU Palette (Hue Priority)")]
            AuPalette,
        }

        /// <summary>
        /// Used for preview only.
        /// </summary>
        public static Bitmap CropBitmap(Bitmap source, Color backFill, ResizeMode resizeMode, double cropShift = 50)
        {
            int previewWidth = (int)(Width * 1.5);
            int previewHeight = (int)(Height * 1.5);

            cropShift = Math.Clamp(cropShift, 0, 100);

            Bitmap result = new Bitmap(previewWidth, previewHeight, PixelFormat.Format32bppArgb);

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
                        destination = new Rectangle(0, 0, previewWidth, previewHeight);
                        break;

                    case ResizeMode.KeepAspectRatio:
                        {
                            double scale = Math.Min((double)previewWidth / source.Width, (double)previewHeight / source.Height);

                            int scaledWidth = (int)Math.Round(source.Width * scale);
                            int scaledHeight = (int)Math.Round(source.Height * scale);

                            int x = (previewWidth - scaledWidth) / 2;
                            int y = (previewHeight - scaledHeight) / 2;

                            destination = new Rectangle(x, y, scaledWidth, scaledHeight);
                            break;
                        }

                    case ResizeMode.Crop:
                        {
                            double scale = Math.Max((double)previewWidth / source.Width, (double)previewHeight / source.Height);

                            int scaledWidth = (int)Math.Round(source.Width * scale);
                            int scaledHeight = (int)Math.Round(source.Height * scale);

                            double shift = cropShift / 100.0;

                            int x = scaledWidth > previewWidth ? -(int)Math.Round((scaledWidth - previewWidth) * shift) : 0;
                            int y = scaledHeight > previewHeight ? -(int)Math.Round((scaledHeight - previewHeight) * shift) : 0;

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

        public static Bitmap ResizeBitmap(Bitmap source, Color backFill, ResizeMode resizeMode, double cropShift = 50)
        {
            Bitmap result = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);

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
                        destination = new Rectangle(0, 0, Width, Height);
                        break;

                    case ResizeMode.KeepAspectRatio:
                        {
                            double scale = Math.Min((double)Width / source.Width, (double)Height / source.Height);

                            int scaledWidth = (int)Math.Round(source.Width * scale);
                            int scaledHeight = (int)Math.Round(source.Height * scale);

                            int x = (Width - scaledWidth) / 2;
                            int y = (Height - scaledHeight) / 2;

                            destination = new Rectangle(x, y, scaledWidth, scaledHeight);
                            break;
                        }

                    case ResizeMode.Crop:
                        {
                            cropShift = Math.Clamp(cropShift, 0, 100);

                            double scale = Math.Max((double)Width / source.Width, (double)Height / source.Height);

                            int scaledWidth = (int)Math.Round(source.Width * scale);
                            int scaledHeight = (int)Math.Round(source.Height * scale);

                            double shift = cropShift / 100.0;

                            int x = scaledWidth > Width ? -(int)Math.Round((scaledWidth - Width) * shift) : 0;
                            int y = scaledHeight > Height ? -(int)Math.Round((scaledHeight - Height) * shift) : 0;

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

        private readonly struct ColorValue
        {
            public readonly double V1;
            public readonly double V2;
            public readonly double V3;

            public ColorValue(double v1, double v2, double v3) 
            {
                V1 = v1;
                V2 = v2;
                V3 = v3;
            }

            public static ColorValue operator +(ColorValue left, ColorValue right)
            {
                return new ColorValue(left.V1 + right.V1, left.V2 + right.V2, left.V3 + right.V3);
            }

            public static ColorValue operator -(ColorValue left, ColorValue right)
            {
                return new ColorValue(left.V1 - right.V1, left.V2 - right.V2, left.V3 - right.V3);
            }

            public static ColorValue operator *(ColorValue value, double amount)
            {
                return new ColorValue(value.V1 * amount, value.V2 * amount, value.V3 * amount);
            }
        }

        private readonly struct PaletteValue
        {
            public readonly ColorValue Color;
            public readonly double Multiplier;

            public PaletteValue(ColorValue colour, double multiplier)
            {
                Color = colour;
                Multiplier = multiplier;
            }
        }

        private readonly struct DitherPoint
        {
            public readonly int X;
            public readonly int Y;
            public readonly double Weight;

            public DitherPoint(int x, int y, double weight)
            {
                X = x;
                Y = y;
                Weight = weight;
            }
        }

        private static readonly DitherPoint[] NoDitherKernel = Array.Empty<DitherPoint>();

        private static readonly DitherPoint[] FloydSteinbergKernel =
        {
            new DitherPoint(1, 0, 7.0 / 16.0),
            new DitherPoint(-1, 1, 3.0 / 16.0),
            new DitherPoint(0, 1, 5.0 / 16.0),
            new DitherPoint(1, 1, 1.0 / 16.0),
        };

        private static readonly DitherPoint[] StuckiKernel =
        {
            new DitherPoint(1, 0, 8.0 / 42.0),
            new DitherPoint(2, 0, 4.0 / 42.0),

            new DitherPoint(-2, 1, 2.0 / 42.0),
            new DitherPoint(-1, 1, 4.0 / 42.0),
            new DitherPoint(0, 1, 8.0 / 42.0),
            new DitherPoint(1, 1, 4.0 / 42.0),
            new DitherPoint(2, 1, 2.0 / 42.0),

            new DitherPoint(-2, 2, 1.0 / 42.0),
            new DitherPoint(-1, 2, 2.0 / 42.0),
            new DitherPoint(0, 2, 4.0 / 42.0),
            new DitherPoint(1, 2, 2.0 / 42.0),
            new DitherPoint(2, 2, 1.0 / 42.0),
        };

        private static readonly DitherPoint[] JarvisJudiceNinkeKernel =
        {
            new DitherPoint(1, 0, 7.0 / 48.0),
            new DitherPoint(2, 0, 5.0 / 48.0),

            new DitherPoint(-2, 1, 3.0 / 48.0),
            new DitherPoint(-1, 1, 5.0 / 48.0),
            new DitherPoint(0, 1, 7.0 / 48.0),
            new DitherPoint(1, 1, 5.0 / 48.0),
            new DitherPoint(2, 1, 3.0 / 48.0),

            new DitherPoint(-2, 2, 1.0 / 48.0),
            new DitherPoint(-1, 2, 3.0 / 48.0),
            new DitherPoint(0, 2, 5.0 / 48.0),
            new DitherPoint(1, 2, 3.0 / 48.0),
            new DitherPoint(2, 2, 1.0 / 48.0),
        };

        private static readonly DitherPoint[] AtkinsonKernel =
        {
            new DitherPoint(1, 0, 1.0 / 8.0),
            new DitherPoint(2, 0, 1.0 / 8.0),
            new DitherPoint(-1, 1, 1.0 / 8.0),
            new DitherPoint(0, 1, 1.0 / 8.0),
            new DitherPoint(1, 1, 1.0 / 8.0),
            new DitherPoint(0, 2, 1.0 / 8.0),
        };

        private static readonly DitherPoint[] SierraKernel =
        {
            new DitherPoint(1, 0, 5.0 / 32.0),
            new DitherPoint(2, 0, 3.0 / 32.0),

            new DitherPoint(-2, 1, 2.0 / 32.0),
            new DitherPoint(-1, 1, 4.0 / 32.0),
            new DitherPoint(0, 1, 5.0 / 32.0),
            new DitherPoint(1, 1, 4.0 / 32.0),
            new DitherPoint(2, 1, 2.0 / 32.0),

            new DitherPoint(-1, 2, 2.0 / 32.0),
            new DitherPoint(0, 2, 3.0 / 32.0),
            new DitherPoint(1, 2, 2.0 / 32.0),
        };

        private static readonly DitherPoint[] SierraLiteKernel =
        {
            new DitherPoint(1, 0, 2.0 / 4.0),
            new DitherPoint(-1, 1, 1.0 / 4.0),
            new DitherPoint(0, 1, 1.0 / 4.0),
        };

        private static readonly int[,] Bayer4x4 =
        {
            {  0,  8,  2, 10 },
            { 12,  4, 14,  6 },
            {  3, 11,  1,  9 },
            { 15,  7, 13,  5 }
        };

        private static readonly int[,] Bayer8x8 =
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

        private static ColorValue ToColorSpace(Color Color, ColorSpace colorSpace)
        {
            switch (colorSpace)
            {
                case ColorSpace.Rgb:
                case ColorSpace.RgbEuclidean:
                    return new ColorValue(Color.R, Color.G, Color.B);

                case ColorSpace.LinearRgb:
                    return new ColorValue(SrgbToLinear(Color.R / 255.0), SrgbToLinear(Color.G / 255.0), SrgbToLinear(Color.B / 255.0));

                case ColorSpace.CieXyz:
                    return RgbToXyz(Color);

                case ColorSpace.CieLab:
                    return XyzToLab(RgbToXyz(Color));

                case ColorSpace.OkLab:
                case ColorSpace.OkLch:
                case ColorSpace.AuPalette:
                    return RgbToOklab(Color);

                case ColorSpace.YCbCr:
                    return RgbToYCbCr(Color);

                default:
                    throw new ArgumentOutOfRangeException(nameof(colorSpace));
            }
        }

        private static Color FromColorSpace(ColorValue value, ColorSpace colorSpace)
        {
            switch (colorSpace)
            {
                case ColorSpace.Rgb:
                case ColorSpace.RgbEuclidean:
                    return Color.FromArgb(255, Clamp(value.V1), Clamp(value.V2), Clamp(value.V3));

                case ColorSpace.LinearRgb:
                    return Color.FromArgb(255, Clamp255(LinearToSrgb(value.V1)), Clamp255(LinearToSrgb(value.V2)), Clamp255(LinearToSrgb(value.V3)));

                case ColorSpace.CieXyz:
                    return XyzToRgb(value);

                case ColorSpace.CieLab:
                    return XyzToRgb(LabToXyz(value));

                case ColorSpace.OkLab:
                case ColorSpace.OkLch:
                case ColorSpace.AuPalette:
                    return OklabToRgb(value);

                case ColorSpace.YCbCr:
                    return YCbCrToRgb(value);

                default:
                    throw new ArgumentOutOfRangeException(nameof(colorSpace));
            }
        }

        private static double ColorDistance(ColorValue source, ColorValue target, ColorSpace colorSpace)
        {
            double d1 = source.V1 - target.V1;
            double d2 = source.V2 - target.V2;
            double d3 = source.V3 - target.V3;

            switch (colorSpace)
            {
                case ColorSpace.Rgb:
                    return d1 * d1 * 0.299 + d2 * d2 * 0.587 + d3 * d3 * 0.114;

                case ColorSpace.OkLch:
                    return OkLchDistance(source, target, 1.0, 0.45, 2.25);

                case ColorSpace.AuPalette:
                    return AuPaletteDistance(source, target);

                case ColorSpace.RgbEuclidean:
                case ColorSpace.LinearRgb:
                case ColorSpace.CieXyz:
                case ColorSpace.CieLab:
                case ColorSpace.OkLab:
                case ColorSpace.YCbCr:
                    return d1 * d1 + d2 * d2 + d3 * d3;

                default:
                    throw new ArgumentOutOfRangeException(nameof(colorSpace));
            }
        }

        private static double OkLchDistance(ColorValue source, ColorValue target, double lightnessWeight, double chromaWeight, double hueWeight)
        {
            (double sourceL, double sourceC, double sourceH) = OklabToOklch(source);
            (double targetL, double targetC, double targetH) = OklabToOklch(target);

            double dL = sourceL - targetL;
            double dC = sourceC - targetC;
            double dH = HueDistance(sourceH, targetH);

            double hueStrength = Math.Min(sourceC, targetC);

            return dL * dL * lightnessWeight + dC * dC * chromaWeight + dH * dH * hueStrength * hueWeight;
        }

        private static double AuPaletteDistance(ColorValue source, ColorValue target)
        {
            (double sourceL, double sourceC, double sourceH) = OklabToOklch(source);
            (double targetL, double targetC, double targetH) = OklabToOklch(target);

            double dL = sourceL - targetL;
            double dC = sourceC - targetC;
            double dH = HueDistance(sourceH, targetH);

            double hueStrength = Math.Min(sourceC, targetC);
            double distance = dL * dL * 0.70 + dC * dC * 0.30 + dH * dH * hueStrength * 3.75;

            double sourceHueDegrees = RadiansToDegrees(sourceH);
            double targetHueDegrees = RadiansToDegrees(targetH);

            bool sourceIsWarm = HueWithin(sourceHueDegrees, 25.0, 115.0) && sourceC > 0.035;
            bool targetIsGreen = HueWithin(targetHueDegrees, 115.0, 175.0) && targetC > 0.05;

            if (sourceIsWarm && targetIsGreen)
                distance += 0.12 + sourceC * 0.40;

            bool sourceIsOrangeYellow = HueWithin(sourceHueDegrees, 45.0, 115.0) && sourceC > 0.035;
            bool targetIsYellow = HueWithin(targetHueDegrees, 75.0, 125.0) && targetC > 0.05;

            if (sourceIsOrangeYellow && targetIsYellow)
                distance *= 0.82;

            return distance;
        }

        private static (double L, double C, double H) OklabToOklch(ColorValue value)
        {
            double chroma = Math.Sqrt(value.V2 * value.V2 + value.V3 * value.V3);
            double hue = Math.Atan2(value.V3, value.V2);

            if (hue < 0)
                hue += Math.PI * 2.0;

            return (value.V1, chroma, hue);
        }

        private static double HueDistance(double hue1, double hue2)
        {
            double difference = Math.Abs(hue1 - hue2);

            if (difference > Math.PI)
                difference = Math.PI * 2.0 - difference;

            return difference;
        }

        private static double RadiansToDegrees(double radians)
        {
            return radians * 180.0 / Math.PI;
        }

        private static bool HueWithin(double hue, double start, double end)
        {
            if (start <= end)
                return hue >= start && hue <= end;

            return hue >= start || hue <= end;
        }

        private static PaletteValue[] BuildPaletteValues(WbColor[] palette, ColorSpace colorSpace)
        {
            PaletteValue[] values = new PaletteValue[palette.Length];

            for (int i = 0; i < palette.Length; i++)
            {
                double multiplier = palette[i].Bias;

                values[i] = new PaletteValue(ToColorSpace(palette[i].Color, colorSpace), multiplier);
            }

            return values;
        }

        private static int FindNearestColorIndex(ColorValue source, PaletteValue[] paletteValues, ColorSpace colorSpace)
        {
            int nearestIndex = 0;
            double nearestDistance = double.MaxValue;

            for (int i = 0; i < paletteValues.Length; i++)
            {
                double distance = ColorDistance(source, paletteValues[i].Color, colorSpace);
                distance *= paletteValues[i].Multiplier;

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestIndex = i;
                }
            }

            return nearestIndex;
        }

        private static ColorValue RgbToOklab(Color Color)
        {
            double r = SrgbToLinear(Color.R / 255.0);
            double g = SrgbToLinear(Color.G / 255.0);
            double b = SrgbToLinear(Color.B / 255.0);

            double l = 0.4122214708 * r + 0.5363325363 * g + 0.0514459929 * b;
            double m = 0.2119034982 * r + 0.6806995451 * g + 0.1073969566 * b;
            double s = 0.0883024619 * r + 0.2817188376 * g + 0.6299787005 * b;

            double l_ = Math.Cbrt(l);
            double m_ = Math.Cbrt(m);
            double s_ = Math.Cbrt(s);

            return new ColorValue(
                0.2104542553 * l_ + 0.7936177850 * m_ - 0.0040720468 * s_,
                1.9779984951 * l_ - 2.4285922050 * m_ + 0.4505937099 * s_,
                0.0259040371 * l_ + 0.7827717662 * m_ - 0.8086757660 * s_);
        }

        private static Color OklabToRgb(ColorValue value)
        {
            double l_ = value.V1 + 0.3963377774 * value.V2 + 0.2158037573 * value.V3;
            double m_ = value.V1 - 0.1055613458 * value.V2 - 0.0638541728 * value.V3;
            double s_ = value.V1 - 0.0894841775 * value.V2 - 1.2914855480 * value.V3;

            double l = l_ * l_ * l_;
            double m = m_ * m_ * m_;
            double s = s_ * s_ * s_;

            double r = +4.0767416621 * l - 3.3077115913 * m + 0.2309699292 * s;
            double g = -1.2684380046 * l + 2.6097574011 * m - 0.3413193965 * s;
            double b = -0.0041960863 * l - 0.7034186147 * m + 1.7076147010 * s;

            return Color.FromArgb(255, Clamp255(LinearToSrgb(r)), Clamp255(LinearToSrgb(g)), Clamp255(LinearToSrgb(b)));
        }

        private static ColorValue RgbToXyz(Color Color)
        {
            double r = SrgbToLinear(Color.R / 255.0);
            double g = SrgbToLinear(Color.G / 255.0);
            double b = SrgbToLinear(Color.B / 255.0);

            return new ColorValue(
                0.4124564 * r + 0.3575761 * g + 0.1804375 * b,
                0.2126729 * r + 0.7151522 * g + 0.0721750 * b,
                0.0193339 * r + 0.1191920 * g + 0.9503041 * b);
        }

        private static Color XyzToRgb(ColorValue xyz)
        {
            double r = 3.2404542 * xyz.V1 - 1.5371385 * xyz.V2 - 0.4985314 * xyz.V3;
            double g = -0.9692660 * xyz.V1 + 1.8760108 * xyz.V2 + 0.0415560 * xyz.V3;
            double b = 0.0556434 * xyz.V1 - 0.2040259 * xyz.V2 + 1.0572252 * xyz.V3;

            return Color.FromArgb(255, Clamp255(LinearToSrgb(r)), Clamp255(LinearToSrgb(g)), Clamp255(LinearToSrgb(b)));
        }

        private static ColorValue XyzToLab(ColorValue xyz)
        {
            const double xn = 0.95047;
            const double yn = 1.00000;
            const double zn = 1.08883;

            double x = LabF(xyz.V1 / xn);
            double y = LabF(xyz.V2 / yn);
            double z = LabF(xyz.V3 / zn);

            return new ColorValue(
                116.0 * y - 16.0,
                500.0 * (x - y),
                200.0 * (y - z));
        }

        private static ColorValue LabToXyz(ColorValue lab)
        {
            const double xn = 0.95047;
            const double yn = 1.00000;
            const double zn = 1.08883;

            double fy = (lab.V1 + 16.0) / 116.0;
            double fx = fy + lab.V2 / 500.0;
            double fz = fy - lab.V3 / 200.0;

            return new ColorValue(
                xn * LabFInverse(fx),
                yn * LabFInverse(fy),
                zn * LabFInverse(fz));
        }

        private static ColorValue RgbToYCbCr(Color Color)
        {
            double r = Color.R / 255.0;
            double g = Color.G / 255.0;
            double b = Color.B / 255.0;

            double y = 0.299 * r + 0.587 * g + 0.114 * b;
            double cb = (b - y) / 1.772;
            double cr = (r - y) / 1.402;

            return new ColorValue(y, cb, cr);
        }

        private static Color YCbCrToRgb(ColorValue value)
        {
            double r = value.V1 + 1.402 * value.V3;
            double b = value.V1 + 1.772 * value.V2;
            double g = (value.V1 - 0.299 * r - 0.114 * b) / 0.587;

            return Color.FromArgb(255, Clamp255(r), Clamp255(g), Clamp255(b));
        }

        private static double SrgbToLinear(double value)
        {
            return value <= 0.04045 ? value / 12.92 : Math.Pow((value + 0.055) / 1.055, 2.4);
        }

        private static double LinearToSrgb(double value)
        {
            return value <= 0.0031308 ? 12.92 * value : 1.055 * Math.Pow(Math.Max(value, 0.0), 1.0 / 2.4) - 0.055;
        }

        private static double LabF(double value)
        {
            const double delta = 6.0 / 29.0;
            double delta3 = delta * delta * delta;

            return value > delta3 ? Math.Cbrt(value) : value / (3.0 * delta * delta) + 4.0 / 29.0;
        }

        private static double LabFInverse(double value)
        {
            const double delta = 6.0 / 29.0;

            return value > delta ? value * value * value : 3.0 * delta * delta * (value - 4.0 / 29.0);
        }

        private static DitherPoint[] GetDitherKernel(DitherType ditherType)
        {
            switch (ditherType)
            {
                case DitherType.Floyd_Steinberg:
                case DitherType.Floyd_Steinberg_Serpentine_Scanning:
                    return FloydSteinbergKernel;

                case DitherType.Stucki:
                    return StuckiKernel;

                case DitherType.Jarvis_Judice_Ninke:
                    return JarvisJudiceNinkeKernel;

                case DitherType.Atkinson:
                    return AtkinsonKernel;

                case DitherType.Sierra:
                    return SierraKernel;

                case DitherType.Sierra_Lite:
                    return SierraLiteKernel;

                case DitherType.None:
                case DitherType.Bayer_4x4:
                case DitherType.Bayer_8x8:
                    return NoDitherKernel;

                default:
                    throw new ArgumentOutOfRangeException(nameof(ditherType));
            }
        }

        private static Color ApplyOrderedDither(Color source, int x, int y, DitherType ditherType)
        {
            double threshold;

            switch (ditherType)
            {
                case DitherType.Bayer_4x4:
                    threshold = ((Bayer4x4[y % 4, x % 4] + 0.5) / 16.0 - 0.5) * 64.0;
                    break;

                case DitherType.Bayer_8x8:
                    threshold = ((Bayer8x8[y % 8, x % 8] + 0.5) / 64.0 - 0.5) * 64.0;
                    break;

                default:
                    return source;
            }

            return Color.FromArgb(source.A, Clamp(source.R + threshold), Clamp(source.G + threshold), Clamp(source.B + threshold));
        }

        private static void DiffuseError(ColorValue[,] errors, int x, int y, int width, int height, int direction, ColorValue error, DitherType ditherType)
        {
            DitherPoint[] kernel = GetDitherKernel(ditherType);

            for (int i = 0; i < kernel.Length; i++)
            {
                DitherPoint point = kernel[i];

                int targetX = x + point.X * direction;
                int targetY = y + point.Y;

                if (targetX < 0 || targetX >= width || targetY < 0 || targetY >= height)
                    continue;

                errors[targetX, targetY] = errors[targetX, targetY] + error * point.Weight;
            }
        }

        private static Bitmap CreateArgbCopy(Bitmap source)
        {
            Bitmap result = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(result))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.DrawImageUnscaled(source, 0, 0);
            }

            return result;
        }

        public static unsafe Bitmap ApplyPalette(Bitmap source, WbColor[] palette, ColorSpace colorSpace)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (palette == null || palette.Length == 0)
                return CreateArgbCopy(source);

            bool ownsInput = source.PixelFormat != PixelFormat.Format32bppArgb;
            Bitmap input = ownsInput ? CreateArgbCopy(source) : source;

            Bitmap result = new Bitmap(input.Width, input.Height, PixelFormat.Format32bppArgb);
            PaletteValue[] paletteValues = BuildPaletteValues(palette, colorSpace);

            Rectangle rectangle = new Rectangle(0, 0, input.Width, input.Height);
            BitmapData inputData = input.LockBits(rectangle, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData resultData = result.LockBits(rectangle, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            try
            {
                byte* inputBase = (byte*)inputData.Scan0;
                byte* resultBase = (byte*)resultData.Scan0;

                for (int y = 0; y < input.Height; y++)
                {
                    byte* inputRow = inputBase + y * inputData.Stride;
                    byte* resultRow = resultBase + y * resultData.Stride;

                    for (int x = 0; x < input.Width; x++)
                    {
                        int offset = x * 4;

                        byte b = inputRow[offset];
                        byte g = inputRow[offset + 1];
                        byte r = inputRow[offset + 2];
                        byte a = inputRow[offset + 3];

                        Color sourceColor = Color.FromArgb(a, r, g, b);

                        if (a < 128)
                            sourceColor = Color.White;

                        ColorValue sourceValue = ToColorSpace(sourceColor, colorSpace);
                        int paletteIndex = FindNearestColorIndex(sourceValue, paletteValues, colorSpace);
                        Color paletteColor = palette[paletteIndex].Color;

                        resultRow[offset] = paletteColor.B;
                        resultRow[offset + 1] = paletteColor.G;
                        resultRow[offset + 2] = paletteColor.R;
                        resultRow[offset + 3] = 255;
                    }
                }
            }
            finally
            {
                input.UnlockBits(inputData);
                result.UnlockBits(resultData);

                if (ownsInput)
                    input.Dispose();
            }

            return result;
        }

        public static unsafe Bitmap ApplyPaletteDithered(Bitmap source, WbColor[] palette, DitherType ditherType, ColorSpace colorSpace)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (palette == null || palette.Length == 0)
                return CreateArgbCopy(source);

            bool ownsInput = source.PixelFormat != PixelFormat.Format32bppArgb;
            Bitmap input = ownsInput ? CreateArgbCopy(source) : source;

            int width = input.Width;
            int height = input.Height;

            Bitmap result = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            ColorValue[,] errors = new ColorValue[width, height];
            PaletteValue[] paletteValues = BuildPaletteValues(palette, colorSpace);

            Rectangle rectangle = new Rectangle(0, 0, width, height);
            BitmapData sourceData = input.LockBits(rectangle, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
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

                        Color sourceColor = Color.FromArgb(sourceA, sourceR, sourceG, sourceB);

                        if (sourceA < 128)
                            sourceColor = Color.White;

                        sourceColor = ApplyOrderedDither(sourceColor, x, y, ditherType);

                        ColorValue adjustedValue = ToColorSpace(sourceColor, colorSpace) + errors[x, y];

                        // round-trip through RGB so out-of-gamut error values are clipped consistently.
                        Color adjustedColor = FromColorSpace(adjustedValue, colorSpace);
                        adjustedValue = ToColorSpace(adjustedColor, colorSpace);

                        int paletteIndex = FindNearestColorIndex(adjustedValue, paletteValues, colorSpace);
                        Color paletteColor = palette[paletteIndex].Color;

                        resultRow[offset] = paletteColor.B;
                        resultRow[offset + 1] = paletteColor.G;
                        resultRow[offset + 2] = paletteColor.R;
                        resultRow[offset + 3] = 255;

                        ColorValue error = adjustedValue - paletteValues[paletteIndex].Color;

                        DiffuseError(errors, x, y, width, height, direction, error, ditherType);
                    }
                }
            }
            finally
            {
                input.UnlockBits(sourceData);
                result.UnlockBits(resultData);

                if (ownsInput)
                    input.Dispose();
            }

            return result;
        }

        private static int Clamp(double value)
        {
            return (int)Math.Clamp(Math.Round(value), 0, 255);
        }

        private static int Clamp255(double value)
        {
            return (int)Math.Clamp(Math.Round(value * 255.0), 0, 255);
        }
    }
}