using System;
using UnityEngine;

namespace RS
{
    public static class ColorUtils
    {
        /// <summary>
        /// A map from HSL -> RGB.
        /// </summary>
        public static readonly int[] HSLToRGBMap = new int[0x100000];

        static ColorUtils()
        {
             CreateHSLToRGBMap(0.8D);
        }

        /// <summary>
        /// Creates the HSL -> RGB map.
        /// </summary>
        /// <param name="brightness">The brightness of each color.</param>
        public static void CreateHSLToRGBMap(double brightness)
        {
            int position = 0;

            double twothird = 2D / 3D;
            double onethird = twothird / 2D;

            for (int y = 0; y < 512; y++)
            {
                double d1 = (double)(y / 8) / 64D + (1D / 128D);
                double d2 = (double)(y & 7) / 8D + (1D / 16D);

                for (int x = 0; x < 128; x++)
                {
                    double d3 = (double)x / 128D;
                    double d4 = d3;
                    double d5 = d3;
                    double d6 = d3;

                    if (d2 != 0.0D)
                    {
                        double d7;
                        if (d3 < 0.5D)
                        {
                            d7 = d3 * (1.0D + d2);
                        }
                        else
                        {
                            d7 = (d3 + d2) - d3 * d2;
                        }

                        double d8 = 2D * d3 - d7;
                        double d9 = d1 + onethird;

                        if (d9 > 1.0D)
                        {
                            d9--;
                        }

                        double d10 = d1;
                        double d11 = d1 - onethird;

                        if (d11 < 0.0D)
                        {
                            d11++;
                        }

                        if (6D * d9 < 1.0D)
                        {
                            d4 = d8 + (d7 - d8) * 6D * d9;
                        }
                        else if (2D * d9 < 1.0D)
                        {
                            d4 = d7;
                        }
                        else if (3D * d9 < 2D)
                        {
                            d4 = d8 + (d7 - d8) * (twothird - d9) * 6D;
                        }
                        else
                        {
                            d4 = d8;
                        }

                        if (6D * d10 < 1.0D)
                        {
                            d5 = d8 + (d7 - d8) * 6D * d10;
                        }
                        else if (2D * d10 < 1.0D)
                        {
                            d5 = d7;
                        }
                        else if (3D * d10 < 2D)
                        {
                            d5 = d8 + (d7 - d8) * (twothird - d10) * 6D;
                        }
                        else
                        {
                            d5 = d8;
                        }

                        if (6D * d11 < 1.0D)
                        {
                            d6 = d8 + (d7 - d8) * 6D * d11;
                        }
                        else if (2D * d11 < 1.0D)
                        {
                            d6 = d7;
                        }
                        else if (3D * d11 < 2D)
                        {
                            d6 = d8 + (d7 - d8) * (twothird - d11) * 6D;
                        }
                        else
                        {
                            d6 = d8;
                        }
                    }

                    int red = (int)(d4 * 256D);
                    int green = (int)(d5 * 256D);
                    int blue = (int)(d6 * 256D);
                    int rgb = (red << 16) + (green << 8) + blue;

                    if (rgb == 0)
                    {
                        rgb = 1;
                    }

                    HSLToRGBMap[position++] = rgb;
                }
            }
        }

        public static int TrimHSL(int hue, int saturation, int lightness)
        {
            if (lightness > 179)
            {
                saturation /= 2;
            }
            if (lightness > 192)
            {
                saturation /= 2;
            }
            if (lightness > 217)
            {
                saturation /= 2;
            }
            if (lightness > 243)
            {
                saturation /= 2;
            }
            return (hue / 4 << 10) + (saturation / 32 << 7) + lightness / 2;
        }

        /// <summary>
        /// Converts a RGB color code to a color object.
        /// </summary>
        /// <param name="rgb">The RGB color.</param>
        /// <param name="a">The alpha value.</param>
        /// <returns>The created color.</returns>
        public static Color RGBToColor(int rgb, int a)
        {
            var r = (byte)((rgb >> 16) & 0xFF);
            var g = (byte)((rgb >> 8) & 0xFF);
            var b = (byte)((rgb) & 0xFF);
            return new Color32(r, g, b, (byte)a);
        }

        /// <summary>
        /// Converts a RGB color code to a color object.
        /// </summary>
        /// <param name="rgb">The RGB color.</param>
        /// <returns>The created color.</returns>
        public static Color RGBToColor(int rgb)
        {
            return RGBToColor(rgb, (rgb >> 24) & 0xFF);
        }
        
        public static int TrimHSL(int hue, int saturation)
        {
            return (hue / 4 << 10) + (saturation / 32 << 7);
        }

        public static int SetHslLight(int hsl, int l)
        {
            if (hsl == -1)
            {
                return 12345678;
            }

            l = l * (hsl & 0x7f) / 128;

            if (l < 2)
            {
                l = 2;
            }
            else if (l > 126)
            {
                l = 126;
            }

            return (hsl & 0xff80) + l;
        }

        public static int SetHslLight2(int hsl, int brightness)
        {
            if (hsl == -2)
            {
                return 12345678;
            }

            if (hsl == -1)
            {
                if (brightness < 0)
                {
                    brightness = 0;
                }
                else if (brightness > 127)
                {
                    brightness = 127;
                }
                brightness = 127 - brightness;
                return brightness;
            }

            brightness = brightness * (hsl & 0x7f) / 128;

            if (brightness < 2)
            {
                brightness = 2;
            }
            else if (brightness > 126)
            {
                brightness = 126;
            }

            return (hsl & 0xff80) + brightness;
        }

        public static int SetHslLightness(int hsl, int lightness, int info)
        {
            if ((info & 2) == 2)
            {
                if (lightness < 0)
                {
                    lightness = 0;
                }
                else if (lightness > 127)
                {
                    lightness = 127;
                }

                lightness = 127 - lightness;
                return lightness;
            }

            lightness = lightness * (hsl & 0x7f) >> 7;

            if (lightness < 2)
            {
                lightness = 2;
            }
            else if (lightness > 126)
            {
                lightness = 126;
            }

            return (hsl & 0xff80) + lightness;
        }

        public static int GetRGB(int hsl, int brightness)
        {
            if (hsl == -2)
            {
                return 12345678;
            }

            if (hsl == -1)
            {
                if (brightness < 0)
                {
                    brightness = 0;
                }
                else if (brightness > 127)
                {
                    brightness = 127;
                }
                brightness = 127 - brightness;
                return brightness;
            }

            brightness = brightness * (hsl & 0x7f) / 128;

            if (brightness < 2)
            {
                brightness = 2;
            }
            else if (brightness > 126)
            {
                brightness = 126;
            }

            return (hsl & 0xff80) + brightness;
        }

        public static int LinearRGBBrightness(int rgb, double brightness)
        {
            double r = Math.Pow((rgb >> 16) / 256D, brightness);
            double g = Math.Pow((rgb >> 8 & 0xff) / 256D, brightness);
            double b = Math.Pow((rgb & 0xff) / 256D, brightness);
            return ((int)(r * 256D) << 16) + ((int)(g * 256D) << 8) + (int)(b * 256D);
        }

        public static float IntColorToFloat(int i)
        {
            float conv = i;
            return conv / 255.0f;
        }

        public static float IntColorToFloat(uint i)
        {
            float conv = i;
            return conv / 255.0f;
        }

        public static Color Mix(params Color[] aColors)
        {
            Color result = new Color(0, 0, 0, 0);
            foreach (Color c in aColors)
            {
                result += c;
            }
            result /= aColors.Length;
            return result;
        }

        public static uint ForceAlpha(int rgb)
        {
            return (uint)rgb | 0xFF000000;
        }

        public static Color32[] RotateMatrix(Color32[] matrix, int n)
        {
            var ret = new Color32[n * n];
            for (var i = 0; i < n; ++i)
            {
                for (var j = 0; j < n; ++j)
                {
                    ret[i * n + j] = matrix[(n - j - 1) * n + i];
                }
            }
            return ret;
        }
    }
}
