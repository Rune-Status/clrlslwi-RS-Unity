using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

namespace RS
{
    /// <summary>
    /// A bitmap image.
    /// </summary>
    public class Bitmap
    {
        public int CropHeight;
        public int CropWidth;
        public int Width;
        public int Height;
        public int OffsetX;
        public int OffsetY;
        public int[] Palette;
        public sbyte[] Pixels;
        public bool HadTransparent = false;
        public Texture2D UnityTexture;

        public Bitmap(CacheArchive archive, String imageArchive)
            : this(archive, imageArchive, 0)
        {

        }

        public Bitmap(CacheArchive archive, string imageArchive, int fileIndex)
        {
            JagexBuffer data = new DefaultJagexBuffer(archive.GetFile(imageArchive + ".dat"));
            JagexBuffer idx = new DefaultJagexBuffer(archive.GetFile("index.dat"));

            idx.Position(data.ReadUShort());

            this.CropWidth = idx.ReadUShort();
            this.CropHeight = idx.ReadUShort();

            this.Palette = new int[idx.ReadUByte()];

            for (int i = 0; i < this.Palette.Length - 1; i++)
            {
                this.Palette[i + 1] = idx.ReadTriByte();
            }

            for (int l = 0; l < fileIndex; l++)
            {
                idx.Position(idx.Position() + 2);
                data.Position(data.Position() + idx.ReadUShort() * idx.ReadUShort());
                idx.Position(idx.Position() + 1);
            }

            this.OffsetX = idx.ReadUByte();
            this.OffsetY = idx.ReadUByte();
            this.Width = idx.ReadUShort();
            this.Height = idx.ReadUShort();
            int type = idx.ReadUByte();

            this.Pixels = new sbyte[this.Width * this.Height];

            if (type == 0)
            {
                for (int i = 0; i < this.Pixels.Length; i++)
                {
                    this.Pixels[i] = (sbyte)data.ReadByte();
                    if (this.Palette[this.Pixels[i]] == 0)
                    {
                        HadTransparent = true;
                    }
                }
            }
            else if (type == 1)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    for (int y = 0; y < this.Height; y++)
                    {
                        this.Pixels[x + (y * this.Width)] = (sbyte)data.ReadByte();
                        if (this.Palette[this.Pixels[x + (y * this.Width)]] == 0)
                        {
                            HadTransparent = true;
                        }
                    }
                }
            }
        }

        public void Crop()
        {
            if (this.Width == this.CropWidth && this.Height == this.CropHeight)
            {
                return;
            }

            sbyte[] pixels = new sbyte[this.CropWidth * this.CropHeight];

            int i = 0;
            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    pixels[x + this.OffsetX + (y + this.OffsetY) * this.CropWidth] = this.Pixels[i++];
                }
            }

            this.Pixels = pixels;
            this.Width = this.CropWidth;
            this.Height = this.CropHeight;
            this.OffsetX = 0;
            this.OffsetY = 0;
        }

        public Bitmap FlipHorizontal()
        {
            sbyte[] pixels = new sbyte[this.Width * this.Height];

            int i = 0;
            for (int y = 0; y < this.Height; y++)
            {
                for (int x = this.Width - 1; x >= 0; x--)
                {
                    pixels[i++] = this.Pixels[x + (y * this.Width)];
                }
            }

            this.Pixels = pixels;
            this.OffsetX = this.CropWidth - this.Width - this.OffsetX;
            return this;
        }

        public Bitmap FlipVertical()
        {
            sbyte[] pixels = new sbyte[this.Width * this.Height];

            int i = 0;
            for (int y = this.Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    pixels[i++] = this.Pixels[x + (y * this.Width)];
                }
            }

            this.Pixels = pixels;
            this.OffsetY = this.CropHeight - this.Height - this.OffsetY;
            return this;
        }

        public void Shrink()
        {
            this.CropWidth >>= 1;
            this.CropHeight >>= 1;

            sbyte[] pixels = new sbyte[this.CropWidth * this.CropHeight];
            int i = 0;
            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    pixels[(x + this.OffsetX >> 1) + (y + this.OffsetY >> 1) * this.CropWidth] = this.Pixels[i++];
                }
            }

            this.Pixels = pixels;
            this.Width = this.CropWidth;
            this.Height = this.CropHeight;
            this.OffsetX = 0;
            this.OffsetY = 0;
        }

        public void TranslateRGB(int red, int green, int blue)
        {
            for (int i = 0; i < this.Palette.Length; i++)
            {
                int r = (this.Palette[i] >> 16 & 0xff) + red;
                int g = (this.Palette[i] >> 8 & 0xff) + green;
                int b = (this.Palette[i] & 0xff) + blue;
                r = r > 255 ? 255 : r < 0 ? 0 : r;
                g = g > 255 ? 255 : g < 0 ? 0 : g;
                b = b > 255 ? 255 : b < 0 ? 0 : b;
                this.Palette[i] = (r << 16) + (g << 8) + b;
            }
        }

        public Texture2D ToUnityTexture()
        {
            Texture2D tex = new Texture2D(this.Width, this.Height, TextureFormat.ARGB32, false);
            UnityEngine.Color[] color = new UnityEngine.Color[this.Width * this.Height];
            for (int i = 0; i < color.Length; i++)
            {
                int rgb = this.Palette[this.Pixels[i]];
                int r = rgb >> 16 & 0xFF;
                int g = rgb >> 8 & 0xFF;
                int b = rgb & 0xFF;
                if (rgb <= 0)
                {
                    color[i] = new UnityEngine.Color(0, 0, 0, 0);
                }
                else
                {
                    color[i] = new UnityEngine.Color(ColorUtils.IntColorToFloat(r), ColorUtils.IntColorToFloat(g), ColorUtils.IntColorToFloat(b));
                }
            }

            tex.SetPixels(color);
            tex.Apply();
            tex.filterMode = FilterMode.Trilinear;
            return tex;
        }
    }
}
