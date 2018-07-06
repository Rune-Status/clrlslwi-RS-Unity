using System.Collections.Generic;

using UnityEngine;

namespace RS
{
    /// <summary>
    /// Provides utilities relating to unity textures.
    /// </summary>
    public static class TextureUtils
    {
        /// <summary>
        /// Flips all pixels in the image horizontally.
        /// </summary>
        /// <param name="original">The texture to flip.</param>
        /// <returns>The flipped texture.</returns>
        public static Texture2D FlipHorizontal(Texture2D original)
        {
            Texture2D flipped = new Texture2D(original.width, original.height);

            int width = original.width;
            int height = original.height;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    flipped.SetPixel(width - x - 1, y, original.GetPixel(x, y));
                }
            }
            flipped.Apply();
            return flipped;
        }

        /// <summary>
        /// Flips all pixels in the image vertically.
        /// </summary>
        /// <param name="original">The texture to flip.</param>
        /// <returns>The flipped texture.</returns>
        public static Texture2D FlipVertical(Texture2D original)
        {
            Texture2D flipped = new Texture2D(original.width, original.height);

            int width = original.width;
            int height = original.height;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    flipped.SetPixel(x, height - y - 1, original.GetPixel(x, y));
                }
            }
            flipped.Apply();
            return flipped;
        }

        /// <summary>
        /// Replaces a color in the provided texture.
        /// </summary>
        /// <param name="tex">The texture to replace the colors in.</param>
        /// <param name="from">The color to replace.</param>
        /// <param name="to">The color to replace the from color with.</param>
        public static void Replace(Texture2D tex, Color from, Color to)
        {
            for (int x = 0; x < tex.width; x++)
            {
                for (int y = 0; y < tex.height; y++)
                {
                    var col = tex.GetPixel(x, y);
                    if (col.r == from.r && col.g == from.g && col.b == from.b)
                    {
                        tex.SetPixel(x, y, to);
                    }
                }
            }
            tex.Apply();
        }

        /// <summary>
        /// Creates a 1x1 texture with the provided color.
        /// </summary>
        /// <param name="r">The red value.</param>
        /// <param name="g">The green value.</param>
        /// <param name="b">The blue value.</param>
        /// <param name="a">The alpha value.</param>
        /// <returns>The texture to create.</returns>
        public static Texture2D CreateTextureOfColor(float r, float g, float b, float a)
        {
            Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            tex.SetPixel(0, 0, new Color(r, g, b, a));
            tex.Apply();
            return tex;
        }

        /// <summary>
        /// Creates a 1x1 texture with the provided color.
        /// </summary>
        /// <param name="r">The red value.</param>
        /// <param name="g">The green value.</param>
        /// <param name="b">The blue value.</param>
        /// <param name="a">The alpha value.</param>
        /// <returns>The texture to create.</returns>
        public static Texture2D CreateTextureOfColor(byte r, byte g, byte b, byte a)
        {
            Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            tex.SetPixel(0, 0, new Color32(r, g, b, a));
            tex.Apply();
            return tex;
        }

        /// <summary>
        /// Creates a 1x1 texture with the provided color.
        /// </summary>
        /// <param name="r">The red value.</param>
        /// <param name="g">The green value.</param>
        /// <param name="b">The blue value.</param>
        /// <param name="a">The alpha value.</param>
        /// <returns>The texture to create.</returns>
        public static Texture2D CreateTextureOfColor(int r, int g, int b, int a)
        {
            return CreateTextureOfColor((byte)r, (byte)g, (byte)b, (byte)a);
        }

        /// <summary>
        /// Creates a 1x1 texture with the provided color.
        /// </summary>
        /// <param name="rgb">The RGB values.</param>
        /// <param name="a">The alpha value.</param>
        /// <returns>The texture to create.</returns>
        public static Texture2D CreateTextureOfColor(int rgb, int a)
        {
            return CreateTextureOfColor((rgb >> 16) & 0xFF, (rgb >> 8) & 0xFF, rgb & 0xFF, a);
        }

        /// <summary>
        /// Rotates a texture by the provided angle.
        /// </summary>
        /// <param name="tex">The texture to rotate.</param>
        /// <param name="n">The angle to rotate by.</param>
        /// <returns>The rotated texture.</returns>
        public static Texture2D Rotate(Texture2D tex, int n)
        {
            Texture2D @new = new Texture2D(tex.width, tex.height, tex.format, false, true);
            @new.SetPixels32(ColorUtils.RotateMatrix(tex.GetPixels32(), n));
            @new.Apply();
            return @new;
        }

        /// <summary>
        /// Determines if the provided texture has any transparency.
        /// </summary>
        /// <param name="tex">The texture to check.</param>
        /// <returns>If the provided texture has transparency.</returns>
        public static bool HasTransparency(Texture2D tex)
        {
            var pixels = tex.GetPixels32();
            foreach (var col in pixels)
            {
                if (col.a < 255)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Draws a texture into the provided pixels.
        /// </summary>
        /// <param name="pixels">The pixels to draw into.</param>
        /// <param name="pixelsWidth">The pixel width.</param>
        /// <param name="pixelsHeight">The pixel height.</param>
        /// <param name="tex">The texture to draw.</param>
        /// <param name="x">The x coordinate to draw at.</param>
        /// <param name="y">The y coordinate to draw at.</param>
        public static void Draw(int[] pixels, int pixelsWidth, int pixelsHeight, Texture2D tex, int x, int y)
        {
            var from = tex.GetPixels32();
            var ptr = (y * pixelsWidth) + x;

            for (var cy = 0; cy < tex.height; cy++)
            {
                var off = ptr;
                for (var cx = 0; cx < tex.width; cx++)
                {
                    var col = from[cx + (cy * tex.width)];
                    var rgb = 0;
                    rgb |= (col.r) << 16;
                    rgb |= (col.g) << 8;
                    rgb |= (col.b);
                    if (rgb != 0)
                    {
                        pixels[off] = rgb;
                    }

                    off += 1;
                }
                ptr += pixelsWidth;
            }
        }

        /// <summary>
        /// Draws a texture into the provided pixels.
        /// </summary>
        /// <param name="pixels">The pixels to draw into.</param>
        /// <param name="pixelsWidth">The pixel width.</param>
        /// <param name="pixelsHeight">The pixel height.</param>
        /// <param name="tex">The texture to draw.</param>
        /// <param name="x">The x coordinate to draw at.</param>
        /// <param name="y">The y coordinate to draw at.</param>
        public static void Draw(Color[] pixels, int pixelsWidth, int pixelsHeight, Texture2D tex, int x, int y)
        {
            var from = tex.GetPixels32();
            var ptr = (y * pixelsWidth) + x;

            for (var cy = 0; cy < tex.height; cy++)
            {
                var off = ptr;
                for (var cx = 0; cx < tex.width; cx++)
                {
                    var col = from[cx + (cy * tex.width)];
                    var rgb = 0;
                    rgb |= (col.r) << 16;
                    rgb |= (col.g) << 8;
                    rgb |= (col.b);
                    if (rgb != 0)
                    {
                        pixels[off] = col;
                    }

                    off += 1;
                }
                ptr += pixelsWidth;
            }
        }

        /// <summary>
        /// Creates a palette of all of the colors in the provided texture.
        /// </summary>
        /// <param name="tex">The texture to generate a palette out of.</param>
        /// <returns>All colors in the provided texture.</returns>
        public static List<int> GetPalette(Texture2D tex)
        {
            var list = new List<int>();
            var pixels = tex.GetPixels32();
            for (var i = 0; i < pixels.Length; i++)
            {
                var pixel = pixels[i];
                var rgb = (pixel.r << 16) | (pixel.g << 8) | (pixel.b);
                if (!list.Contains(rgb))
                {
                    list.Add(rgb);
                }
            }
            return list;
        }

        /// <summary>
        /// Calculates the average RGB value of the provided texture.
        /// </summary>
        /// <param name="tex">The texture to calculate the average RGB of.</param>
        /// <returns>The average RGB value of the provided texture.</returns>
        public static int GetAverageRGB(Texture2D tex)
        {
            int red = 0;
            int green = 0;
            int blue = 0;
            var palette = GetPalette(tex);
            var count = palette.Count;

            for (int i = 0; i < count; i++)
            {
                var crgb = palette[i];
                red += crgb >> 16 & 0xff;
                green += crgb >> 8 & 0xff;
                blue += crgb & 0xff;
            }

            int rgb = ColorUtils.LinearRGBBrightness(((red / count) << 16) + ((green / count) << 8) + (blue / count), 1.3999999999999999D);

            if (rgb == 0)
            {
                rgb = 1;
            }
            
            return rgb;
        }

        /// <summary>
        /// Creates a copy of the provided texture.
        /// </summary>
        /// <param name="tex">The texture to copy.</param>
        /// <returns>The texture copy.</returns>
        public static Texture2D Copy(Texture2D tex)
        {
            var copy = new Texture2D(tex.width, tex.height, tex.format, false);
            copy.SetPixels(tex.GetPixels());
            copy.Apply();
            return copy;
        }
    }

}
