using System;
using System.Collections.Generic;

using UnityEngine;

namespace RS
{
    /// <summary>
    /// A font backed by bitmap style data.
    /// </summary>
    public class BitmapFont
    {
        public const byte Center = (byte)0x1;
        public const byte Right = (byte)0x2;
        public const byte Shadow = (byte)0x4;
        public const byte AllowTags = (byte)0x8;
        public const byte ShadowCenter = Shadow | Center;

        public static BitmapFont SMALL, NORMAL, BOLD, FANCY;
        public int[] CharWidths;
        public int CharHeight;
        public int[][] Mask;
        public int[] MaskHeight;
        public int[] MaskWidth;
        public int[] OffsetX;
        public int[] OffsetY;
        public bool Strikethrough;

        private Dictionary<string, Texture2D> cachedImages = new Dictionary<string, Texture2D>();

        public BitmapFont(string name, CacheArchive archive)
        {
            Mask = new int[256][];
            MaskWidth = new int[256];
            MaskHeight = new int[256];
            OffsetX = new int[256];
            OffsetY = new int[256];
            CharWidths = new int[256];
            Strikethrough = false;

            var data = new DefaultJagexBuffer(archive.GetFile(name + ".dat"));
            var idx = new DefaultJagexBuffer(archive.GetFile("index.dat"));
            idx.Position(data.ReadUShort() + 4);

            var offset = idx.ReadUByte();
            if (offset > 0)
            {
                idx.Position(idx.Position() + (3 * (offset - 1)));
            }

            for (var i = 0; i < 256; i++)
            {
                OffsetX[i] = idx.ReadByte();
                OffsetY[i] = idx.ReadByte();
                int width = MaskWidth[i] = idx.ReadUShort();
                int height = MaskHeight[i] = idx.ReadUShort();
                int type = idx.ReadUByte();
                Mask[i] = new int[width * height];

                if (type == 0)
                {
                    for (var j = 0; j < Mask[i].Length; j++)
                    {
                        Mask[i][j] = data.ReadByte();
                    }
                }
                else if (type == 1)
                {
                    for (var x = 0; x < width; x++)
                    {
                        for (var y = 0; y < height; y++)
                        {
                            Mask[i][x + y * width] = data.ReadByte();
                        }
                    }
                }

                if (height > CharHeight && i < 128)
                {
                    CharHeight = height;
                }

                OffsetX[i] = 1;
                CharWidths[i] = (byte)(width + 2);

                var k2 = 0;
                for (var y = height / 7; y < height; y++)
                {
                    k2 += Mask[i][y * width];
                }

                if (k2 <= height / 7)
                {
                    CharWidths[i]--;
                    OffsetX[i] = 0;
                }

                k2 = 0;
                for (var y = height / 7; y < height; y++)
                {
                    k2 += Mask[i][(width - 1) + y * width];
                }

                if (k2 <= height / 7)
                {
                    CharWidths[i]--;
                }
            }

            if (name.Equals("q8_full"))
            {
                CharWidths[32] = CharWidths[73];
            }
            else
            {
                CharWidths[32] = CharWidths[105];
            }
        }

        public void Draw(Texture2D target, uint color, string s, int x, int y)
        {
            if (s == null)
            {
                return;
            }

            y -= CharHeight;

            var ca = s.ToCharArray();
            for (int i = 0; i < s.Length; i++)
            {
                var c = ca[i];
                if (c != ' ')
                {
                    var pixels = target.GetPixels();
                    DrawChar(target, pixels, Mask[c], x + OffsetX[c], y + OffsetY[c], MaskWidth[c], MaskHeight[c], color);
                    target.SetPixels(pixels);
                }
                x += CharWidths[c];
            }
        }

        public void DrawWavy(Texture2D target, string s, int x, int y, uint color, int wave)
        {
            if (s == null)
            {
                return;
            }
            x -= GetWidth(s) / 2;
            y -= CharHeight;

            var ca = s.ToCharArray();
            for (int i = 0; i < ca.Length; i++)
            {
                var c = ca[i];
                if (c != ' ')
                {
                    var pixels = target.GetPixels();
                    DrawChar(target, pixels, Mask[c], x + OffsetX[c], y + OffsetY[c] + (int)(Math.Sin((double)i / 2D + (double)wave / 5D) * 5D), MaskWidth[c], MaskHeight[c], color);
                    target.SetPixels(pixels);
                }
                x += CharWidths[c];
            }

        }

        public void Draw(Texture2D target, string s, int x, int y, uint color)
        {
            Draw(target, s, x, y, color, 0);
        }

        public void Draw(Texture2D target, string s, int x, int y, uint color, int flags)
        {
            if (s == null)
            {
                return;
            }

            DrawString(target, s, x, y, color, false, true);
        }

        public void DrawCenteredWavy(Texture2D target, int amplitude, string s, bool flag, int loop, int y, int x, uint color)
        {
            if (s == null)
            {
                return;
            }

            double offset = 7D - amplitude / 8D;
            if (offset < 0.0D)
            {
                offset = 0.0D;
            }

            x -= GetWidth(s) / 2;
            y -= CharHeight;

            var ca = s.ToCharArray();
            for (var i = 0; i < ca.Length; i++)
            {
                var c = ca[i];
                if (c != ' ')
                {
                    var pixels = target.GetPixels();
                    DrawChar(target, pixels, Mask[c], x + OffsetX[c], y + OffsetY[c] + (int)(Math.Sin((double)i / 1.5D + (double)loop) * offset), MaskWidth[c], MaskHeight[c], color);
                    target.SetPixels(pixels);
                }
                x += CharWidths[c];
            }
        }

        public void DrawCenteredWavy2(Texture2D target, int x, string s, int cycle, int y, uint l)
        {
            if (s == null)
            {
                return;
            }

            x -= GetWidth(s) / 2;
            y -= CharHeight;

            var ca = s.ToCharArray();
            for (var i = 0; i < ca.Length; i++)
            {
                var c = ca[i];
                if (c != ' ')
                {
                    var pixels = target.GetPixels();
                    DrawChar(target, pixels, Mask[c], x + OffsetX[c] + (int)(Math.Sin((double)i / 5D + (double)cycle / 5D) * 5D), y + OffsetY[c] + (int)(Math.Sin((double)i / 3D + (double)cycle / 5D) * 5D), MaskWidth[c], MaskHeight[c], l);
                    target.SetPixels(pixels);
                }
                x += CharWidths[c];
            }
        }

        public void DrawChar(Texture2D target, Color[] pixels, int[] mask, int x, int y, int width, int height, uint color)
        {
            var destOff = x + y * target.width;
            var destStep = target.width - width;
            var maskStep = 0;
            var maskOff = 0;
            if (y < 0)
            {
                var i = 0 - y;
                height -= i;
                y = 0;
                maskOff += i * width;
                destOff += i * Screen.width;
            }

            if (y + height >= Screen.height)
            {
                height -= ((y + height) - Screen.height) + 1;
            }

            if (x < 0)
            {
                var i = 0 - x;
                width -= i;
                x = 0;
                maskOff += i;
                destOff += i;
                maskStep += i;
                destStep += i;
            }

            if (x + width >= Screen.width)
            {
                var i = ((x + width) - Screen.width) + 1;
                width -= i;
                maskStep += i;
                destStep += i;
            }

            if (width <= 0 || height <= 0)
            {
                return;
            }

            TextureRasterizer.Draw(target, pixels, mask, maskOff, destOff, width, height, destStep, maskStep, color);
        }

        public void DrawChar(Texture2D target, int opacity, int x, int[] mask, int width, int y, int height, bool flag, uint color)
        {
            var destOff = x + y * target.width;
            var destStep = target.width - width;
            var maskStep = 0;
            var maskOff = 0;
            if (y < 0)
            {
                var yStep = 0 - y;
                height -= yStep;
                y = 0;
                maskOff += yStep * width;
                destOff += yStep * Screen.width;
            }

            if (y + height >= Screen.height)
            {
                height -= ((y + height) - Screen.height) + 1;
            }

            if (x < 0)
            {
                var xStep = 0 - x;
                width -= xStep;
                x = 0;
                maskOff += xStep;
                destOff += xStep;
                maskStep += xStep;
                destStep += xStep;
            }

            if (x + width >= Screen.width)
            {
                var step = ((x + width) - Screen.width) + 1;
                width -= step;
                maskStep += step;
                destStep += step;
            }

            if (width <= 0 || height <= 0)
            {
                return;
            }

            TextureRasterizer.Draw(target, mask, maskOff, destOff, width, height, destStep, maskStep, color, opacity);
        }

        public void DrawString(Texture2D target, bool shadow, int x, uint color, string s, int y, int opacity)
        {
            if (s == null)
            {
                return;
            }

            y -= CharHeight;
            var ca = s.ToCharArray();
            for (var i = 0; i < ca.Length; i++)
            {
                if (ca[i] == '@' && i + 4 < ca.Length && ca[i + 4] == '@')
                {
                    var tagColor = GetTagColor(s.Substring(i + 1, i + 4));
                    if (tagColor != 0)
                    {
                        color = tagColor;
                    }
                    i += 4;
                }
                else
                {
                    var c = ca[i];
                    if (c != ' ')
                    {
                        if (shadow)
                        {
                            DrawChar(target, opacity / 2, x + OffsetX[c] + 1, Mask[c], MaskWidth[c], y + OffsetY[c] + 1, MaskHeight[c], false, 0);
                        }
                        DrawChar(target, opacity, x + OffsetX[c], Mask[c], MaskWidth[c], target.height - (y + OffsetY[c]) - MaskHeight[c], MaskHeight[c], false, color);
                    }
                    x += CharWidths[c];
                }
            }

        }

        public void DrawString(Texture2D target, string s, int x, int y, uint color, bool shadow, bool allowTags)
        {
            Strikethrough = false;
            int startX = x;
            if (s == null)
            {
                return;
            }
            y -= CharHeight;

            var pixels = target.GetPixels();
            var ca = s.ToCharArray();
            for (var i = 0; i < ca.Length; i++)
            {
                if (ca[i] == '@' && i + 4 < ca.Length && ca[i + 4] == '@' && allowTags)
                {
                    var rgb = GetTagColor(s.Substring(i + 1, 3));
                    if (rgb != 111)
                    {
                        color = rgb;
                    }
                    i += 4;
                }
                else
                {
                    var extracted = false;
                    if (allowTags)
                    {
                        if (ca[i] == '<')
                        {
                            var cur = i;
                            var next = s.IndexOf('>', i);
                            if (next != -1)
                            {
                                var str = s.Substring(cur, next - cur + 1);
                                if (str.StartsWith("<col="))
                                {
                                    var kv = str.Split('=');
                                    var rgb = kv[1].Substring(0, kv[1].Length - 1);
                                    
                                    color = 0xFF000000 | uint.Parse(rgb, System.Globalization.NumberStyles.HexNumber);
                                }
                                else if (str.StartsWith("<img="))
                                {
                                    var inner = str.Substring(1, str.Length - 2);
                                    var segments = inner.Split('=');
                                    if (segments.Length == 2)
                                    {
                                        var @params = segments[1].Split('&');
                                        if (@params.Length >= 2)
                                        {
                                            var key = @params[0];
                                            var val = int.Parse(@params[1]);
                                            var tex = GetTex(key, val);

                                            var pos = "bottom";
                                            if (@params.Length >= 3)
                                            {
                                                pos = @params[2];
                                            }

                                            var texY = 0;
                                            switch (pos)
                                            {
                                                case "bottom":
                                                    texY = 0;
                                                    break;
                                                case "top":
                                                    texY = target.height - tex.height;
                                                    break;
                                                case "center":
                                                    texY = (target.height / 2) - (tex.height / 2);
                                                    break;
                                            }

                                            var yOff = 0;
                                            if (@params.Length >= 4)
                                            {
                                                yOff = int.Parse(@params[3]);
                                            }

                                            TextureUtils.Draw(pixels, target.width, target.height, tex, x, texY + yOff);
                                            x += tex.width;
                                        }
                                    }
                                }

                                i += str.Length - 1;
                                extracted = true;
                            }
                        }
                    }
                    
                    if (!extracted)
                    {
                        char c = ca[i];
                        if (c != ' ')
                        {
                            if (shadow)
                            {
                                DrawChar(target, pixels, Mask[c], x + OffsetX[c] + 1, target.height - (y + OffsetY[c]) - MaskHeight[c] - 1, MaskWidth[c], MaskHeight[c], 0xFF000000);
                            }
                            DrawChar(target, pixels, Mask[c], x + OffsetX[c], target.height - (y + OffsetY[c]) - MaskHeight[c], MaskWidth[c], MaskHeight[c], color);
                        }
                        x += CharWidths[c];
                    }
                }
            }
            if (Strikethrough)
            {
                //draw_line_h(startX, y + (int)((double)height * 0.7D), x - startX, 0x800000);
            }
            target.SetPixels(pixels);
        }

        public int MaxHeight
        {
            get
            {
                var max = 0;
                for (var i = 0; i < OffsetY.Length; i++)
                {
                    var height = OffsetY[i] + MaskHeight[i];
                    if (height > max)
                    {
                        max = height;
                    }
                }
                return max + 1;
            }
        }

        public Texture2D DrawString(string s, uint color, bool shadow, bool allowTags)
        {
            var tex = new Texture2D(GetWidth(s), MaxHeight, TextureFormat.ARGB32, false, true);
            for (var px = 0; px < tex.width; px++)
            {
                for (var py = 0; py < tex.height; py++)
                {
                    tex.SetPixel(px, py, new Color(0, 0, 0, 0));
                }
            }
            DrawString(tex, s, 0, CharHeight, color, shadow, allowTags);
            tex.Apply();
            return tex;
        }

        private Texture2D GetTex(string key, int val)
        {
            Texture2D tex;
            if (cachedImages.TryGetValue(key + "," + val, out tex))
            {
                return tex;
            }

            tex = GameContext.Cache.GetImageAsTex(key, val);
            cachedImages.Add(key + "," + val, tex);
            return tex;
        }

        public uint GetTagColor(string s)
        {
            if (s.Equals("str"))
            {
                Strikethrough = true;
                return 0;
            }
            else if (s.Equals("end"))
            {
                Strikethrough = false;
                return 0;
            }

            switch (s)
            {
                case "red":
                    return 0xFF000000 | 0xff0000;
                case "gre":
                    return 0xFF000000 | 65280;
                case "blu":
                    return 0xFF000000 | 0xFF;
                case "yel":
                    return 0xFF000000 | 0xffff00;
                case "cya":
                    return 0xFF000000 | 65535;
                case "mag":
                    return 0xFF000000 | 0xff00ff;
                case "whi":
                    return 0xFF000000 | 0xffffff;
                case "bla":
                    return 0xFF000000 | 0;
                case "lre":
                    return 0xFF000000 | 0xff9040;
                case "dre":
                    return 0xFF000000 | 0x800000;
                case "dbl":
                    return 0xFF000000 | 128;
                case "or1":
                    return 0xFF000000 | 0xffb000;
                case "or2":
                    return 0xFF000000 | 0xff7000;
                case "or3":
                    return 0xFF000000 | 0xff3000;
                case "gr1":
                    return 0xFF000000 | 0xc0ff00;
                case "gr2":
                    return 0xFF000000 | 0x80ff00;
                case "gr3":
                    return 0xFF000000 | 0x40ff00;
                default:
                    return 111;
            }
        }

        public int GetWidth(string s)
        {
            if (s == null)
            {
                return 0;
            }

            var width = 0;
            var ca = s.ToCharArray();
            for (var i = 0; i < ca.Length; i++)
            {
                if (ca[i] == '@' && i + 4 < ca.Length && ca[i + 4] == '@')
                {
                    i += 4;
                }
                else
                {
                    var extracted = false;
                    if (ca[i] == '<')
                    {
                        var cur = i;
                        var next = s.IndexOf('>', i);
                        if (next != -1)
                        {
                            var str = s.Substring(cur, next - cur + 1);
                            if (str.StartsWith("<img="))
                            {
                                var inner = str.Substring(1, str.Length - 2);
                                var segments = inner.Split('=');
                                if (segments.Length == 2)
                                {
                                    var @params = segments[1].Split('&');
                                    if (@params.Length >= 2)
                                    {
                                        var key = @params[0];
                                        var val = @params[1];
                                        var tex = GetTex(key, int.Parse(val));
                                        width += tex.width;
                                    }
                                }
                            }

                            i += str.Length - 1;
                            extracted = true;
                        }
                    }

                    if (!extracted)
                    {
                        width += CharWidths[ca[i]];
                    }
                }
            }
            return width;
        }
    }

}
