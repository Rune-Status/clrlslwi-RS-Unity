using System;

using UnityEngine;

namespace RS
{
    /// <summary>
    /// Provides methos of drawing directly to a texture.
    /// </summary>
    public sealed class TextureRasterizer
    {
        public static void FillRect(Texture2D to, int x, int y, int width, int height, uint color)
        {
            var newA = (color >> 24) & 0xFF;
            var newR = (color >> 16) & 0xFF;
            var newG = (color >> 8) & 0xFF;
            var newB = (color) & 0xFF;
            var col = new Color32((byte)newR, (byte)newG, (byte)newB, (byte)newA);

            var pixels = to.GetPixels();
            for (int i = x; i < (x + width); i++)
            {
                for (int j = y; j < (y + height); j++)
                {
                    int pc = i + ((Math.Abs(j - to.height) - 1) * to.width);
                    pixels[pc] = col;
                }
            }
            to.SetPixels(pixels);
        }

        public static void DrawRect(Texture2D to, int x, int y, int width, int height, uint color)
        {
            DrawLineH(to, x, y, width, color);
            DrawLineH(to, x, (y + height) - 1, width, color);
            DrawLineV(to, x, y, height, color);
            DrawLineV(to, (x + width) - 1, y, height, color);
        }

        public static void DrawLineH(Texture2D to, int x, int y, int len, uint color)
        {
            var a = (byte)((color >> 24) & 0xFF);
            var r = (byte)((color >> 16) & 0xFF);
            var g = (byte)((color >> 8) & 0xFF);
            var b = (byte)(color & 0xFF);
            var col = new Color32(r, g, b, a);

            var pixels = to.GetPixels();
            for (int i = x; i < (x + len); i++)
            {
                int pc = i + ((Math.Abs(y - to.height) - 1) * to.width);
                pixels[pc] = col;
            }

            to.SetPixels(pixels);
        }

        public static void DrawLineV(Texture2D to, int x, int y, int len, uint color)
        {
            var newA = (color >> 24) & 0xFF;
            var newR = (color >> 16) & 0xFF;
            var newG = (color >> 8) & 0xFF;
            var newB = (color) & 0xFF;
            var col = new Color32((byte)newR, (byte)newG, (byte)newB, (byte)newA);

            var pixels = to.GetPixels();
            for (int i = y; i < (y + len); i++)
            {
                int pc = x + ((Math.Abs(i - to.height) - 1) * to.width);
                pixels[pc] = col;
            }

            to.SetPixels(pixels);
        }

        public static void Draw(Texture2D to, Color[] pixels, int[] mask, int maskOff, int destOff, int width, int height, int destStep, int maskStep, uint color)
        {
            var newA = (color >> 24) & 0xFF;
            var newR = (color >> 16) & 0xFF;
            var newG = (color >> 8) & 0xFF;
            var newB = (color) & 0xFF;
            var col = new Color32((byte)newR, (byte)newG, (byte)newB, (byte)newA);
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int maskX = maskOff % width;
                    int maskY = maskOff / width;
                    maskY = Math.Abs(maskY - height) - 1;
                    maskOff++;
                    if (mask[(maskY * width) + maskX] != 0)
                    {
                        pixels[destOff++] = col;
                    }
                    else
                    {
                        destOff++;
                    }
                }
                destOff += destStep;
                maskOff += maskStep;
            }
        }

        public static void Draw(Texture2D to, int[] mask, int maskOff, int destOff, int width, int height, int destStep, int maskStep, uint color, int opacity)
        {
            var newA = (color >> 24) & 0xFF;
            var newR = (color >> 16) & 0xFF;
            var newG = (color >> 8) & 0xFF;
            var newB = (color) & 0xFF;
            var col = new Color32((byte)newR, (byte)newG, (byte)newB, (byte)newA);

            var pixels = to.GetPixels();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int maskX = maskOff % width;
                    int maskY = maskOff / width;
                    maskY = Math.Abs(maskY - height) - 1;
                    maskOff++;
                    if (mask[(maskY * width) + maskX] != 0)
                    {
                        pixels[destOff++] = col;
                    }
                    else
                    {
                        destOff++;
                    }
                }
                destOff += destStep;
                maskOff += maskStep;
            }
            to.SetPixels(pixels);
        }
    }
}
