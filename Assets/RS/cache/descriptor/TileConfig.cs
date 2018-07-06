using System;

namespace RS
{
    /// <summary>
    /// A config that describes how to render a tile.
    /// </summary>
    public class TileConfig
    {
        public int Hsl;
        public int Rgb;
        public int Hue;
        public int HueDivisor;
        public int Hue2;
        public int Lightness;
        public string Name;
        public int Saturation;
        public bool ShowUnderlay;
        public int TextureIndex;

        public TileConfig(JagexBuffer b)
        {
            SetDefaults();
            ParseFrom(b);
        }

        private void ParseFrom(JagexBuffer b)
        {
            int opcode = b.ReadUByte();
            while (opcode != 0)
            {
                ParseOpcode(opcode, b);
                opcode = b.ReadUByte();
            }
        }

        private void ParseOpcode(int opcode, JagexBuffer b)
        {
            if (opcode == 1)
            {
                SetColor(Rgb = b.ReadTriByte());
            }
            else if (opcode == 2)
            {
                TextureIndex = b.ReadByte();
            }
            else if (opcode == 3)
            {
                
            }
            else if (opcode == 5)
            {
                ShowUnderlay = false;
            }
            else if (opcode == 6)
            {
                Name = b.ReadString(10);
            }
            else if (opcode == 7)
            {
                var hue2 = Hue2;
                var saturation = Saturation;
                var lightness = Lightness;
                var hue = Hue;
                SetColor(b.ReadTriByte());
                Hue2 = hue2;
                Saturation = saturation;
                Lightness = lightness;
                Hue = hue;
                HueDivisor = hue;
            }
            else
            {
                throw new Exception("Error unrecognised config code: " + opcode);
            }
        }

        private void SetDefaults()
        {
            TextureIndex = -1;
            ShowUnderlay = true;
        }

        public void SetColor(int rgb)
        {
            var red = (rgb >> 16 & 0xff) / 256D;
            var green = (rgb >> 8 & 0xff) / 256D;
            var blue = (rgb & 0xff) / 256D;
            var lowest = red;

            if (green < lowest)
            {
                lowest = green;
            }

            if (blue < lowest)
            {
                lowest = blue;
            }

            var d4 = red;
            if (green > d4)
            {
                d4 = green;
            }

            if (blue > d4)
            {
                d4 = blue;
            }

            var d5 = 0.0D;
            var d6 = 0.0D;
            var d7 = (lowest + d4) / 2D;
            if (lowest != d4)
            {
                if (d7 < 0.5D)
                {
                    d6 = (d4 - lowest) / (d4 + lowest);
                }
                if (d7 >= 0.5D)
                {
                    d6 = (d4 - lowest) / (2D - d4 - lowest);
                }
                if (red == d4)
                {
                    d5 = (green - blue) / (d4 - lowest);
                }
                else if (green == d4)
                {
                    d5 = 2D + (blue - red) / (d4 - lowest);
                }
                else if (blue == d4)
                {
                    d5 = 4D + (red - green) / (d4 - lowest);
                }
            }

            d5 /= 6D;
            Hue2 = (int)(d5 * 256D);
            Saturation = (int)(d6 * 256D);
            Lightness = (int)(d7 * 256D);

            if (Saturation < 0)
            {
                Saturation = 0;
            }
            else if (Saturation > 255)
            {
                Saturation = 255;
            }

            if (Lightness < 0)
            {
                Lightness = 0;
            }
            else if (Lightness > 255)
            {
                Lightness = 255;
            }

            if (d7 > 0.5D)
            {
                HueDivisor = (int)((1.0D - d7) * d6 * 512D);
            }
            else
            {
                HueDivisor = (int)(d7 * d6 * 512D);
            }

            if (HueDivisor < 1)
            {
                HueDivisor = 1;
            }

            Hue = (int)(d5 * HueDivisor);

            var rand = new System.Random();
            var hue = (Hue2 + (int)(rand.NextDouble() * 16D)) - 8;
            if (hue < 0)
            {
                hue = 0;
            }
            else if (hue > 255)
            {
                hue = 255;
            }

            var saturation = (Saturation + (int)(rand.NextDouble() * 48D)) - 24;
            if (saturation < 0)
            {
                saturation = 0;
            }
            else if (saturation > 255)
            {
                saturation = 255;
            }

            var lightness = (Lightness + (int)(rand.NextDouble() * 48D)) - 24;
            if (lightness < 0)
            {
                lightness = 0;
            }
            else if (lightness > 255)
            {
                lightness = 255;
            }

            Hsl = ColorUtils.TrimHSL(hue, saturation, lightness);
        }

    }

    public class UnderlayFloorDescriptorProvider : IndexedProvider<TileConfig>
    {

        public int count;
        public TileConfig[] cache;

        public UnderlayFloorDescriptorProvider(CacheArchive archive)
        {
            JagexBuffer s = new DefaultJagexBuffer(archive.GetFile("flo.dat"));

            count = s.ReadUShort();
            cache = new TileConfig[count];
            for (int i = 0; i < count; i++)
            {
                cache[i] = new TileConfig(s);
            }
        }

        public TileConfig Provide(int index)
        {
            return cache[index];
        }
    }
    
}
