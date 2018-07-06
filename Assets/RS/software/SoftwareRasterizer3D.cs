namespace RS
{
    /// <summary>
    /// Provides a port of the 3D RS software renderer
    /// </summary>
    public static class SoftwareRasterizer3D
    {
        public static int Cycle;

        public static int CenterX;
        public static int CenterY;
        public static bool CheckBounds;

        public static int[] LightDecay = new int[2048];
        public static int[] ShadowDecay = new int[512];

        public static int Opacity;
        public static bool Opaque;

        public static int[] Pixels;
        public static bool Texturize = true;

        public static void Prepare()
        {
            Pixels = new int[SoftwareRasterizer2D.Height];

            for (int i = 0; i < SoftwareRasterizer2D.Height; i++)
            {
                Pixels[i] = SoftwareRasterizer2D.Width * i;
            }

            CenterX = SoftwareRasterizer2D.Width / 2;
            CenterY = SoftwareRasterizer2D.Height / 2;

            for (int i = 1; i < 512; i++)
            {
                ShadowDecay[i] = 0x8000 / i;
            }

            for (int i = 1; i < 2048; i++)
            {
                LightDecay[i] = 0x10000 / i;
            }
        }

        public static void DrawScanLine(int[] dst, int off, int rgb, int x1, int x2)
        {
            if (CheckBounds)
            {
                if (x2 > SoftwareRasterizer2D.Bound)
                {
                    x2 = SoftwareRasterizer2D.Bound;
                }
                if (x1 < 0)
                {
                    x1 = 0;
                }
            }

            if (x1 >= x2)
            {
                return;
            }

            off += x1;
            int length = x2 - x1;

            if (Opacity == 0)
            {
                while (length-- > 0)
                {
                    dst[off++] = rgb;
                }
            }
            else
            {
                var opacity = 256 - Opacity;
                rgb = ((rgb & 0xff00ff) * opacity >> 8 & 0xff00ff) + ((rgb & 0xff00) * opacity >> 8 & 0xff00);

                while (length-- >= 0)
                {
                    dst[off] = rgb + ((dst[off] & 0xff00ff) * Opacity >> 8 & 0xff00ff) + ((dst[off] & 0xff00) * Opacity >> 8 & 0xff00);
                    off++;
                }
            }
        }

        public static void DrawFlatTriangle(int aX, int aY, int bX, int bY, int cX, int cY, int rgb)
        {
            var atb = 0;
            if (bY != aY)
            {
                atb = (bX - aX << 16) / (bY - aY);
            }

            var btc = 0;
            if (cY != bY)
            {
                btc = (cX - bX << 16) / (cY - bY);
            }

            var cta = 0;
            if (cY != aY)
            {
                cta = (aX - cX << 16) / (aY - cY);
            }
            
            if (aY <= bY && aY <= cY)
            {
                if (aY >= SoftwareRasterizer2D.RightY)
                {
                    return;
                }

                if (bY > SoftwareRasterizer2D.RightY)
                {
                    bY = SoftwareRasterizer2D.RightY;
                }

                if (cY > SoftwareRasterizer2D.RightY)
                {
                    cY = SoftwareRasterizer2D.RightY;
                }

                if (bY < cY)
                {
                    cX = aX <<= 16;

                    if (aY < 0)
                    {
                        cX -= cta * aY;
                        aX -= atb * aY;
                        aY = 0;
                    }

                    bX <<= 16;

                    if (bY < 0)
                    {
                        bX -= btc * bY;
                        bY = 0;
                    }

                    if (aY != bY && cta < atb || aY == bY && cta > btc)
                    {
                        cY -= bY;
                        bY -= aY;

                        for (aY = Pixels[aY]; --bY >= 0; aY += SoftwareRasterizer2D.Width)
                        {
                            DrawScanLine(SoftwareRasterizer2D.Pixels, aY, rgb, cX >> 16, aX >> 16);
                            cX += cta;
                            aX += atb;
                        }

                        while (--cY >= 0)
                        {
                            DrawScanLine(SoftwareRasterizer2D.Pixels, aY, rgb, cX >> 16, bX >> 16);

                            cX += cta;
                            bX += btc;
                            aY += SoftwareRasterizer2D.Width;
                        }
                        return;
                    }

                    cY -= bY;
                    bY -= aY;

                    for (aY = Pixels[aY]; --bY >= 0; aY += SoftwareRasterizer2D.Width)
                    {
                        DrawScanLine(SoftwareRasterizer2D.Pixels, aY, rgb, aX >> 16, cX >> 16);
                        cX += cta;
                        aX += atb;
                    }

                    while (--cY >= 0)
                    {
                        DrawScanLine(SoftwareRasterizer2D.Pixels, aY, rgb, bX >> 16, cX >> 16);
                        cX += cta;
                        bX += btc;
                        aY += SoftwareRasterizer2D.Width;
                    }
                    return;
                }

                bX = aX <<= 16;

                if (aY < 0)
                {
                    bX -= cta * aY;
                    aX -= atb * aY;
                    aY = 0;
                }

                cX <<= 16;

                if (cY < 0)
                {
                    cX -= btc * cY;
                    cY = 0;
                }

                if (aY != cY && cta < atb || aY == cY && btc > atb)
                {
                    bY -= cY;
                    cY -= aY;

                    for (aY = Pixels[aY]; --cY >= 0; aY += SoftwareRasterizer2D.Width)
                    {
                        DrawScanLine(SoftwareRasterizer2D.Pixels, aY, rgb, bX >> 16, aX >> 16);

                        bX += cta;
                        aX += atb;
                    }

                    while (--bY >= 0)
                    {
                        DrawScanLine(SoftwareRasterizer2D.Pixels, aY, rgb, cX >> 16, aX >> 16);

                        cX += btc;
                        aX += atb;
                        aY += SoftwareRasterizer2D.Width;
                    }
                    return;
                }

                bY -= cY;
                cY -= aY;

                for (aY = Pixels[aY]; --cY >= 0; aY += SoftwareRasterizer2D.Width)
                {
                    DrawScanLine(SoftwareRasterizer2D.Pixels, aY, rgb, aX >> 16, bX >> 16);
                    bX += cta;
                    aX += atb;
                }

                while (--bY >= 0)
                {
                    DrawScanLine(SoftwareRasterizer2D.Pixels, aY, rgb, aX >> 16, cX >> 16);
                    cX += btc;
                    aX += atb;
                    aY += SoftwareRasterizer2D.Width;
                }
                return;
            }
            
            if (bY <= cY)
            {
                if (bY >= SoftwareRasterizer2D.RightY)
                {
                    return;
                }

                if (cY > SoftwareRasterizer2D.RightY)
                {
                    cY = SoftwareRasterizer2D.RightY;
                }

                if (aY > SoftwareRasterizer2D.RightY)
                {
                    aY = SoftwareRasterizer2D.RightY;
                }

                if (cY < aY)
                {
                    aX = bX <<= 16;

                    if (bY < 0)
                    {
                        aX -= atb * bY;
                        bX -= btc * bY;
                        bY = 0;
                    }

                    cX <<= 16;

                    if (cY < 0)
                    {
                        cX -= cta * cY;
                        cY = 0;
                    }

                    if (bY != cY && atb < btc || bY == cY && atb > cta)
                    {
                        aY -= cY;
                        cY -= bY;

                        for (bY = Pixels[bY]; --cY >= 0; bY += SoftwareRasterizer2D.Width)
                        {
                            DrawScanLine(SoftwareRasterizer2D.Pixels, bY, rgb, aX >> 16, bX >> 16);
                            aX += atb;
                            bX += btc;
                        }

                        while (--aY >= 0)
                        {
                            DrawScanLine(SoftwareRasterizer2D.Pixels, bY, rgb, aX >> 16, cX >> 16);
                            aX += atb;
                            cX += cta;
                            bY += SoftwareRasterizer2D.Width;
                        }
                        return;
                    }

                    aY -= cY;
                    cY -= bY;

                    for (bY = Pixels[bY]; --cY >= 0; bY += SoftwareRasterizer2D.Width)
                    {
                        DrawScanLine(SoftwareRasterizer2D.Pixels, bY, rgb, bX >> 16, aX >> 16);
                        aX += atb;
                        bX += btc;
                    }

                    while (--aY >= 0)
                    {
                        DrawScanLine(SoftwareRasterizer2D.Pixels, bY, rgb, cX >> 16, aX >> 16);
                        aX += atb;
                        cX += cta;
                        bY += SoftwareRasterizer2D.Width;
                    }
                    return;
                }

                cX = bX <<= 16;

                if (bY < 0)
                {
                    cX -= atb * bY;
                    bX -= btc * bY;
                    bY = 0;
                }

                aX <<= 16;

                if (aY < 0)
                {
                    aX -= cta * aY;
                    aY = 0;
                }

                if (atb < btc)
                {
                    cY -= aY;
                    aY -= bY;
                    for (bY = Pixels[bY]; --aY >= 0; bY += SoftwareRasterizer2D.Width)
                    {
                        DrawScanLine(SoftwareRasterizer2D.Pixels, bY, rgb, cX >> 16, bX >> 16);
                        cX += atb;
                        bX += btc;
                    }

                    while (--cY >= 0)
                    {
                        DrawScanLine(SoftwareRasterizer2D.Pixels, bY, rgb, aX >> 16, bX >> 16);
                        aX += cta;
                        bX += btc;
                        bY += SoftwareRasterizer2D.Width;
                    }
                    return;
                }

                cY -= aY;
                aY -= bY;

                for (bY = Pixels[bY]; --aY >= 0; bY += SoftwareRasterizer2D.Width)
                {
                    DrawScanLine(SoftwareRasterizer2D.Pixels, bY, rgb, bX >> 16, cX >> 16);
                    cX += atb;
                    bX += btc;
                }

                while (--cY >= 0)
                {
                    DrawScanLine(SoftwareRasterizer2D.Pixels, bY, rgb, bX >> 16, aX >> 16);
                    aX += cta;
                    bX += btc;
                    bY += SoftwareRasterizer2D.Width;
                }
                return;
            }

            if (cY >= SoftwareRasterizer2D.RightY)
            {
                return;
            }

            if (aY > SoftwareRasterizer2D.RightY)
            {
                aY = SoftwareRasterizer2D.RightY;
            }

            if (bY > SoftwareRasterizer2D.RightY)
            {
                bY = SoftwareRasterizer2D.RightY;
            }

            if (aY < bY)
            {
                bX = cX <<= 16;

                if (cY < 0)
                {
                    bX -= btc * cY;
                    cX -= cta * cY;
                    cY = 0;
                }

                aX <<= 16;

                if (aY < 0)
                {
                    aX -= atb * aY;
                    aY = 0;
                }

                if (btc < cta)
                {
                    bY -= aY;
                    aY -= cY;

                    for (cY = Pixels[cY]; --aY >= 0; cY += SoftwareRasterizer2D.Width)
                    {
                        DrawScanLine(SoftwareRasterizer2D.Pixels, cY, rgb, bX >> 16, cX >> 16);
                        bX += btc;
                        cX += cta;
                    }

                    while (--bY >= 0)
                    {
                        DrawScanLine(SoftwareRasterizer2D.Pixels, cY, rgb, bX >> 16, aX >> 16);
                        bX += btc;
                        aX += atb;
                        cY += SoftwareRasterizer2D.Width;
                    }
                    return;
                }

                bY -= aY;
                aY -= cY;

                for (cY = Pixels[cY]; --aY >= 0; cY += SoftwareRasterizer2D.Width)
                {
                    DrawScanLine(SoftwareRasterizer2D.Pixels, cY, rgb, cX >> 16, bX >> 16);
                    bX += btc;
                    cX += cta;
                }

                while (--bY >= 0)
                {
                    DrawScanLine(SoftwareRasterizer2D.Pixels, cY, rgb, aX >> 16, bX >> 16);
                    bX += btc;
                    aX += atb;
                    cY += SoftwareRasterizer2D.Width;
                }
                return;
            }

            aX = cX <<= 16;

            if (cY < 0)
            {
                aX -= btc * cY;
                cX -= cta * cY;
                cY = 0;
            }

            bX <<= 16;

            if (bY < 0)
            {
                bX -= atb * bY;
                bY = 0;
            }

            if (btc < cta)
            {
                aY -= bY;
                bY -= cY;

                for (cY = Pixels[cY]; --bY >= 0; cY += SoftwareRasterizer2D.Width)
                {
                    DrawScanLine(SoftwareRasterizer2D.Pixels, cY, rgb, aX >> 16, cX >> 16);
                    aX += btc;
                    cX += cta;
                }

                while (--aY >= 0)
                {
                    DrawScanLine(SoftwareRasterizer2D.Pixels, cY, rgb, bX >> 16, cX >> 16);
                    bX += atb;
                    cX += cta;
                    cY += SoftwareRasterizer2D.Width;
                }
                return;
            }

            aY -= bY;
            bY -= cY;

            for (cY = Pixels[cY]; --bY >= 0; cY += SoftwareRasterizer2D.Width)
            {
                DrawScanLine(SoftwareRasterizer2D.Pixels, cY, rgb, cX >> 16, aX >> 16);
                aX += btc;
                cX += cta;
            }

            while (--aY >= 0)
            {
                DrawScanLine(SoftwareRasterizer2D.Pixels, cY, rgb, cX >> 16, bX >> 16);

                bX += atb;
                cX += cta;
                cY += SoftwareRasterizer2D.Width;
            }
        }

        public static void DrawGradientScanLine(int[] dest, int destOff, int color, int position, int length, int x2, int hsl, int x4)
        {
            if (Texturize)
            {
                int cs1;

                if (CheckBounds)
                {
                    if (x2 - length > 3)
                    {
                        cs1 = (x4 - hsl) / (x2 - length);
                    }
                    else {
                        cs1 = 0;
                    }

                    if (x2 > SoftwareRasterizer2D.Bound)
                    {
                        x2 = SoftwareRasterizer2D.Bound;
                    }

                    if (length < 0)
                    {
                        hsl -= length * cs1;
                        length = 0;
                    }

                    if (length >= x2)
                    {
                        return;
                    }

                    destOff += length;
                    position = x2 - length >> 2;
                    cs1 <<= 2;
                }
                else
                {
                    if (length >= x2)
                    {
                        return;
                    }

                    destOff += length;
                    position = x2 - length >> 2;

                    if (position > 0)
                    {
                        cs1 = (x4 - hsl) * ShadowDecay[position] >> 15;
                    }
                    else {
                        cs1 = 0;
                    }
                }

                if (Opacity == 0)
                {
                    while (--position >= 0)
                    {
                        color = ColorUtils.HSLToRGBMap[hsl >> 8];
                        hsl += cs1;
                        dest[destOff++] = color;
                        dest[destOff++] = color;
                        dest[destOff++] = color;
                        dest[destOff++] = color;
                    }
                    position = x2 - length & 3;
                    if (position > 0)
                    {
                        color = ColorUtils.HSLToRGBMap[hsl >> 8];
                        do
                        {
                            dest[destOff++] = color;
                        } while (--position > 0);
                        return;
                    }
                }
                else
                {
                    int opacity2 = Opacity;
                    int alpha2 = 256 - Opacity;
                    while (--position >= 0)
                    {
                        color = ColorUtils.HSLToRGBMap[hsl >> 8];
                        hsl += cs1;
                        color = ((color & 0xff00ff) * alpha2 >> 8 & 0xff00ff) + ((color & 0xff00) * alpha2 >> 8 & 0xff00);
                        dest[destOff] = color + ((dest[destOff] & 0xff00ff) * opacity2 >> 8 & 0xff00ff) + ((dest[destOff] & 0xff00) * opacity2 >> 8 & 0xff00);
                        destOff++;
                        dest[destOff] = color + ((dest[destOff] & 0xff00ff) * opacity2 >> 8 & 0xff00ff) + ((dest[destOff] & 0xff00) * opacity2 >> 8 & 0xff00);
                        destOff++;
                        dest[destOff] = color + ((dest[destOff] & 0xff00ff) * opacity2 >> 8 & 0xff00ff) + ((dest[destOff] & 0xff00) * opacity2 >> 8 & 0xff00);
                        destOff++;
                        dest[destOff] = color + ((dest[destOff] & 0xff00ff) * opacity2 >> 8 & 0xff00ff) + ((dest[destOff] & 0xff00) * opacity2 >> 8 & 0xff00);
                        destOff++;
                    }
                    position = x2 - length & 3;
                    if (position > 0)
                    {
                        color = ColorUtils.HSLToRGBMap[hsl >> 8];
                        color = ((color & 0xff00ff) * alpha2 >> 8 & 0xff00ff) + ((color & 0xff00) * alpha2 >> 8 & 0xff00);
                        do
                        {
                            dest[destOff] = color + ((dest[destOff] & 0xff00ff) * opacity2 >> 8 & 0xff00ff) + ((dest[destOff] & 0xff00) * opacity2 >> 8 & 0xff00);
                            destOff++;
                        } while (--position > 0);
                    }
                }
                return;
            }

            if (length >= x2)
            {
                return;
            }

            int color_step = (x4 - hsl) / (x2 - length);

            if (CheckBounds)
            {
                if (x2 > SoftwareRasterizer2D.Bound)
                {
                    x2 = SoftwareRasterizer2D.Bound;
                }
                if (length < 0)
                {
                    hsl -= length * color_step;
                    length = 0;
                }
                if (length >= x2)
                {
                    return;
                }
            }

            destOff += length;
            position = x2 - length;

            if (Opacity == 0)
            {
                do
                {
                    dest[destOff++] = ColorUtils.HSLToRGBMap[hsl >> 8];
                    hsl += color_step;
                } while (--position > 0);
                return;
            }

            int opacity = Opacity;
            int alpha = 256 - Opacity;

            do
            {
                color = ColorUtils.HSLToRGBMap[hsl >> 8];
                hsl += color_step;
                color = ((color & 0xff00ff) * alpha >> 8 & 0xff00ff) + ((color & 0xff00) * alpha >> 8 & 0xff00);
                dest[destOff] = color + ((dest[destOff] & 0xff00ff) * opacity >> 8 & 0xff00ff) + ((dest[destOff] & 0xff00) * opacity >> 8 & 0xff00);
                destOff++;
            } while (--position > 0);
        }

        public static void DrawShadedTriangle(int x1, int y1, int x2, int y2, int x3, int y3, int hsl1, int hsl2, int hsl3)
        {
            var s1 = 0;
            var cs1 = 0;
            if (y2 != y1)
            {
                s1 = (x2 - x1 << 16) / (y2 - y1);
                cs1 = (hsl2 - hsl1 << 15) / (y2 - y1);
            }

            var s2 = 0;
            var cs2 = 0;
            if (y3 != y2)
            {
                s2 = (x3 - x2 << 16) / (y3 - y2);
                cs2 = (hsl3 - hsl2 << 15) / (y3 - y2);
            }

            var s3 = 0;
            var cs3 = 0;
            if (y3 != y1)
            {
                s3 = (x1 - x3 << 16) / (y1 - y3);
                cs3 = (hsl1 - hsl3 << 15) / (y1 - y3);
            }

            if (y1 <= y2 && y1 <= y3)
            {
                if (y1 >= SoftwareRasterizer2D.RightY)
                {
                    return;
                }

                if (y2 > SoftwareRasterizer2D.RightY)
                {
                    y2 = SoftwareRasterizer2D.RightY;
                }

                if (y3 > SoftwareRasterizer2D.RightY)
                {
                    y3 = SoftwareRasterizer2D.RightY;
                }

                if (y2 < y3)
                {
                    x3 = x1 <<= 16;
                    hsl3 = hsl1 <<= 15;

                    if (y1 < 0)
                    {
                        x3 -= s3 * y1;
                        x1 -= s1 * y1;
                        hsl3 -= cs3 * y1;
                        hsl1 -= cs1 * y1;
                        y1 = 0;
                    }

                    x2 <<= 16;
                    hsl2 <<= 15;

                    if (y2 < 0)
                    {
                        x2 -= s2 * y2;
                        hsl2 -= cs2 * y2;
                        y2 = 0;
                    }

                    if (y1 != y2 && s3 < s1 || y1 == y2 && s3 > s2)
                    {
                        y3 -= y2;
                        y2 -= y1;

                        for (y1 = Pixels[y1]; --y2 >= 0; y1 += SoftwareRasterizer2D.Width)
                        {
                            DrawGradientScanLine(SoftwareRasterizer2D.Pixels, y1, 0, 0, x3 >> 16, x1 >> 16, hsl3 >> 7, hsl1 >> 7);
                            x3 += s3;
                            x1 += s1;
                            hsl3 += cs3;
                            hsl1 += cs1;
                        }

                        while (--y3 >= 0)
                        {
                            DrawGradientScanLine(SoftwareRasterizer2D.Pixels, y1, 0, 0, x3 >> 16, x2 >> 16, hsl3 >> 7, hsl2 >> 7);
                            x3 += s3;
                            x2 += s2;
                            hsl3 += cs3;
                            hsl2 += cs2;
                            y1 += SoftwareRasterizer2D.Width;
                        }
                        return;
                    }

                    y3 -= y2;
                    y2 -= y1;

                    for (y1 = Pixels[y1]; --y2 >= 0; y1 += SoftwareRasterizer2D.Width)
                    {
                        DrawGradientScanLine(SoftwareRasterizer2D.Pixels, y1, 0, 0, x1 >> 16, x3 >> 16, hsl1 >> 7, hsl3 >> 7);
                        x3 += s3;
                        x1 += s1;
                        hsl3 += cs3;
                        hsl1 += cs1;
                    }

                    while (--y3 >= 0)
                    {
                        DrawGradientScanLine(SoftwareRasterizer2D.Pixels, y1, 0, 0, x2 >> 16, x3 >> 16, hsl2 >> 7, hsl3 >> 7);
                        x3 += s3;
                        x2 += s2;
                        hsl3 += cs3;
                        hsl2 += cs2;
                        y1 += SoftwareRasterizer2D.Width;
                    }
                    return;
                }

                x2 = x1 <<= 16;
                hsl2 = hsl1 <<= 15;

                if (y1 < 0)
                {
                    x2 -= s3 * y1;
                    x1 -= s1 * y1;
                    hsl2 -= cs3 * y1;
                    hsl1 -= cs1 * y1;
                    y1 = 0;
                }

                x3 <<= 16;
                hsl3 <<= 15;

                if (y3 < 0)
                {
                    x3 -= s2 * y3;
                    hsl3 -= cs2 * y3;
                    y3 = 0;
                }

                if (y1 != y3 && s3 < s1 || y1 == y3 && s2 > s1)
                {
                    y2 -= y3;
                    y3 -= y1;

                    for (y1 = Pixels[y1]; --y3 >= 0; y1 += SoftwareRasterizer2D.Width)
                    {
                        DrawGradientScanLine(SoftwareRasterizer2D.Pixels, y1, 0, 0, x2 >> 16, x1 >> 16, hsl2 >> 7, hsl1 >> 7);
                        x2 += s3;
                        x1 += s1;
                        hsl2 += cs3;
                        hsl1 += cs1;
                    }

                    while (--y2 >= 0)
                    {
                        DrawGradientScanLine(SoftwareRasterizer2D.Pixels, y1, 0, 0, x3 >> 16, x1 >> 16, hsl3 >> 7, hsl1 >> 7);
                        x3 += s2;
                        x1 += s1;
                        hsl3 += cs2;
                        hsl1 += cs1;
                        y1 += SoftwareRasterizer2D.Width;
                    }
                    return;
                }

                y2 -= y3;
                y3 -= y1;

                for (y1 = Pixels[y1]; --y3 >= 0; y1 += SoftwareRasterizer2D.Width)
                {
                    DrawGradientScanLine(SoftwareRasterizer2D.Pixels, y1, 0, 0, x1 >> 16, x2 >> 16, hsl1 >> 7, hsl2 >> 7);
                    x2 += s3;
                    x1 += s1;
                    hsl2 += cs3;
                    hsl1 += cs1;
                }

                while (--y2 >= 0)
                {
                    DrawGradientScanLine(SoftwareRasterizer2D.Pixels, y1, 0, 0, x1 >> 16, x3 >> 16, hsl1 >> 7, hsl3 >> 7);
                    x3 += s2;
                    x1 += s1;
                    hsl3 += cs2;
                    hsl1 += cs1;
                    y1 += SoftwareRasterizer2D.Width;
                }
                return;
            }

            if (y2 <= y3)
            {
                if (y2 >= SoftwareRasterizer2D.RightY)
                {
                    return;
                }

                if (y3 > SoftwareRasterizer2D.RightY)
                {
                    y3 = SoftwareRasterizer2D.RightY;
                }

                if (y1 > SoftwareRasterizer2D.RightY)
                {
                    y1 = SoftwareRasterizer2D.RightY;
                }

                if (y3 < y1)
                {
                    x1 = x2 <<= 16;
                    hsl1 = hsl2 <<= 15;

                    if (y2 < 0)
                    {
                        x1 -= s1 * y2;
                        x2 -= s2 * y2;
                        hsl1 -= cs1 * y2;
                        hsl2 -= cs2 * y2;
                        y2 = 0;
                    }

                    x3 <<= 16;
                    hsl3 <<= 15;

                    if (y3 < 0)
                    {
                        x3 -= s3 * y3;
                        hsl3 -= cs3 * y3;
                        y3 = 0;
                    }

                    if (y2 != y3 && s1 < s2 || y2 == y3 && s1 > s3)
                    {
                        y1 -= y3;
                        y3 -= y2;

                        for (y2 = Pixels[y2]; --y3 >= 0; y2 += SoftwareRasterizer2D.Width)
                        {
                            DrawGradientScanLine(SoftwareRasterizer2D.Pixels, y2, 0, 0, x1 >> 16, x2 >> 16, hsl1 >> 7, hsl2 >> 7);
                            x1 += s1;
                            x2 += s2;
                            hsl1 += cs1;
                            hsl2 += cs2;
                        }

                        while (--y1 >= 0)
                        {
                            DrawGradientScanLine(SoftwareRasterizer2D.Pixels, y2, 0, 0, x1 >> 16, x3 >> 16, hsl1 >> 7, hsl3 >> 7);
                            x1 += s1;
                            x3 += s3;
                            hsl1 += cs1;
                            hsl3 += cs3;
                            y2 += SoftwareRasterizer2D.Width;
                        }

                        return;
                    }

                    y1 -= y3;
                    y3 -= y2;

                    for (y2 = Pixels[y2]; --y3 >= 0; y2 += SoftwareRasterizer2D.Width)
                    {
                        DrawGradientScanLine(SoftwareRasterizer2D.Pixels, y2, 0, 0, x2 >> 16, x1 >> 16, hsl2 >> 7, hsl1 >> 7);
                        x1 += s1;
                        x2 += s2;
                        hsl1 += cs1;
                        hsl2 += cs2;
                    }

                    while (--y1 >= 0)
                    {
                        DrawGradientScanLine(SoftwareRasterizer2D.Pixels, y2, 0, 0, x3 >> 16, x1 >> 16, hsl3 >> 7, hsl1 >> 7);
                        x1 += s1;
                        x3 += s3;
                        hsl1 += cs1;
                        hsl3 += cs3;
                        y2 += SoftwareRasterizer2D.Width;
                    }
                    return;
                }

                x3 = x2 <<= 16;
                hsl3 = hsl2 <<= 15;

                if (y2 < 0)
                {
                    x3 -= s1 * y2;
                    x2 -= s2 * y2;
                    hsl3 -= cs1 * y2;
                    hsl2 -= cs2 * y2;
                    y2 = 0;
                }

                x1 <<= 16;
                hsl1 <<= 15;

                if (y1 < 0)
                {
                    x1 -= s3 * y1;
                    hsl1 -= cs3 * y1;
                    y1 = 0;
                }

                if (s1 < s2)
                {
                    y3 -= y1;
                    y1 -= y2;

                    for (y2 = Pixels[y2]; --y1 >= 0; y2 += SoftwareRasterizer2D.Width)
                    {
                        DrawGradientScanLine(SoftwareRasterizer2D.Pixels, y2, 0, 0, x3 >> 16, x2 >> 16, hsl3 >> 7, hsl2 >> 7);
                        x3 += s1;
                        x2 += s2;
                        hsl3 += cs1;
                        hsl2 += cs2;
                    }

                    while (--y3 >= 0)
                    {
                        DrawGradientScanLine(SoftwareRasterizer2D.Pixels, y2, 0, 0, x1 >> 16, x2 >> 16, hsl1 >> 7, hsl2 >> 7);
                        x1 += s3;
                        x2 += s2;
                        hsl1 += cs3;
                        hsl2 += cs2;
                        y2 += SoftwareRasterizer2D.Width;
                    }
                    return;
                }

                y3 -= y1;
                y1 -= y2;

                for (y2 = Pixels[y2]; --y1 >= 0; y2 += SoftwareRasterizer2D.Width)
                {
                    DrawGradientScanLine(SoftwareRasterizer2D.Pixels, y2, 0, 0, x2 >> 16, x3 >> 16, hsl2 >> 7, hsl3 >> 7);
                    x3 += s1;
                    x2 += s2;
                    hsl3 += cs1;
                    hsl2 += cs2;
                }

                while (--y3 >= 0)
                {
                    DrawGradientScanLine(SoftwareRasterizer2D.Pixels, y2, 0, 0, x2 >> 16, x1 >> 16, hsl2 >> 7, hsl1 >> 7);
                    x1 += s3;
                    x2 += s2;
                    hsl1 += cs3;
                    hsl2 += cs2;
                    y2 += SoftwareRasterizer2D.Width;
                }
                return;
            }

            if (y3 >= SoftwareRasterizer2D.RightY)
            {
                return;
            }

            if (y1 > SoftwareRasterizer2D.RightY)
            {
                y1 = SoftwareRasterizer2D.RightY;
            }

            if (y2 > SoftwareRasterizer2D.RightY)
            {
                y2 = SoftwareRasterizer2D.RightY;
            }

            if (y1 < y2)
            {
                x2 = x3 <<= 16;
                hsl2 = hsl3 <<= 15;

                if (y3 < 0)
                {
                    x2 -= s2 * y3;
                    x3 -= s3 * y3;
                    hsl2 -= cs2 * y3;
                    hsl3 -= cs3 * y3;
                    y3 = 0;
                }

                x1 <<= 16;
                hsl1 <<= 15;

                if (y1 < 0)
                {
                    x1 -= s1 * y1;
                    hsl1 -= cs1 * y1;
                    y1 = 0;
                }

                if (s2 < s3)
                {
                    y2 -= y1;
                    y1 -= y3;

                    for (y3 = Pixels[y3]; --y1 >= 0; y3 += SoftwareRasterizer2D.Width)
                    {
                        DrawGradientScanLine(SoftwareRasterizer2D.Pixels, y3, 0, 0, x2 >> 16, x3 >> 16, hsl2 >> 7, hsl3 >> 7);
                        x2 += s2;
                        x3 += s3;
                        hsl2 += cs2;
                        hsl3 += cs3;
                    }

                    while (--y2 >= 0)
                    {
                        DrawGradientScanLine(SoftwareRasterizer2D.Pixels, y3, 0, 0, x2 >> 16, x1 >> 16, hsl2 >> 7, hsl1 >> 7);
                        x2 += s2;
                        x1 += s1;
                        hsl2 += cs2;
                        hsl1 += cs1;
                        y3 += SoftwareRasterizer2D.Width;
                    }
                    return;
                }

                y2 -= y1;
                y1 -= y3;

                for (y3 = Pixels[y3]; --y1 >= 0; y3 += SoftwareRasterizer2D.Width)
                {
                    DrawGradientScanLine(SoftwareRasterizer2D.Pixels, y3, 0, 0, x3 >> 16, x2 >> 16, hsl3 >> 7, hsl2 >> 7);
                    x2 += s2;
                    x3 += s3;
                    hsl2 += cs2;
                    hsl3 += cs3;
                }

                while (--y2 >= 0)
                {
                    DrawGradientScanLine(SoftwareRasterizer2D.Pixels, y3, 0, 0, x1 >> 16, x2 >> 16, hsl1 >> 7, hsl2 >> 7);
                    x2 += s2;
                    x1 += s1;
                    hsl2 += cs2;
                    hsl1 += cs1;
                    y3 += SoftwareRasterizer2D.Width;
                }
                return;
            }

            x1 = x3 <<= 16;
            hsl1 = hsl3 <<= 15;

            if (y3 < 0)
            {
                x1 -= s2 * y3;
                x3 -= s3 * y3;
                hsl1 -= cs2 * y3;
                hsl3 -= cs3 * y3;
                y3 = 0;
            }

            x2 <<= 16;
            hsl2 <<= 15;

            if (y2 < 0)
            {
                x2 -= s1 * y2;
                hsl2 -= cs1 * y2;
                y2 = 0;
            }

            if (s2 < s3)
            {
                y1 -= y2;
                y2 -= y3;

                for (y3 = Pixels[y3]; --y2 >= 0; y3 += SoftwareRasterizer2D.Width)
                {
                    DrawGradientScanLine(SoftwareRasterizer2D.Pixels, y3, 0, 0, x1 >> 16, x3 >> 16, hsl1 >> 7, hsl3 >> 7);
                    x1 += s2;
                    x3 += s3;
                    hsl1 += cs2;
                    hsl3 += cs3;
                }

                while (--y1 >= 0)
                {
                    DrawGradientScanLine(SoftwareRasterizer2D.Pixels, y3, 0, 0, x2 >> 16, x3 >> 16, hsl2 >> 7, hsl3 >> 7);
                    x2 += s1;
                    x3 += s3;
                    hsl2 += cs1;
                    hsl3 += cs3;
                    y3 += SoftwareRasterizer2D.Width;
                }
                return;
            }

            y1 -= y2;
            y2 -= y3;

            for (y3 = Pixels[y3]; --y2 >= 0; y3 += SoftwareRasterizer2D.Width)
            {
                DrawGradientScanLine(SoftwareRasterizer2D.Pixels, y3, 0, 0, x3 >> 16, x1 >> 16, hsl3 >> 7, hsl1 >> 7);
                x1 += s2;
                x3 += s3;
                hsl1 += cs2;
                hsl3 += cs3;
            }

            while (--y1 >= 0)
            {
                DrawGradientScanLine(SoftwareRasterizer2D.Pixels, y3, 0, 0, x3 >> 16, x2 >> 16, hsl3 >> 7, hsl2 >> 7);
                x2 += s1;
                x3 += s3;
                hsl2 += cs1;
                hsl3 += cs3;
                y3 += SoftwareRasterizer2D.Width;
            }
        }

        public static void DrawTextureScanLine(int[] ai, int[] ai1, int i, int j, int k, int l, int i1, int j1, int k1, int l1, int i2, int j2, int k2, int l2, int i3)
        {
            if (l >= i1)
            {
                return;
            }
            int j3;
            int k3;
            if (CheckBounds)
            {
                j3 = (k1 - j1) / (i1 - l);
                if (i1 > SoftwareRasterizer2D.Bound)
                {
                    i1 = SoftwareRasterizer2D.Bound;
                }
                if (l < 0)
                {
                    j1 -= l * j3;
                    l = 0;
                }
                if (l >= i1)
                {
                    return;
                }
                k3 = i1 - l >> 3;
                j3 <<= 12;
                j1 <<= 9;
            }
            else
            {
                if (i1 - l > 7)
                {
                    k3 = i1 - l >> 3;
                    j3 = (k1 - j1) * ShadowDecay[k3] >> 6;
                }
                else {
                    k3 = 0;
                    j3 = 0;
                }
                j1 <<= 9;
            }
            k += l;
            
            int j4 = 0;
            int l4 = 0;
            int l6 = l - CenterX;
            l1 += (k2 >> 3) * l6;
            i2 += (l2 >> 3) * l6;
            j2 += (i3 >> 3) * l6;
            int l5 = j2 >> 14;
            if (l5 != 0)
            {
                i = l1 / l5;
                j = i2 / l5;
                if (i < 0)
                {
                    i = 0;
                }
                else if (i > 16256)
                {
                    i = 16256;
                }
            }
            l1 += k2;
            i2 += l2;
            j2 += i3;
            l5 = j2 >> 14;
            if (l5 != 0)
            {
                j4 = l1 / l5;
                l4 = i2 / l5;
                if (j4 < 7)
                {
                    j4 = 7;
                }
                else if (j4 > 16256)
                {
                    j4 = 16256;
                }
            }
            int j7 = j4 - i >> 3;
            int l7 = l4 - j >> 3;
            i += j1 & 0x600000;
            int j8 = j1 >> 23;
            if (Opaque)
            {
                while (k3-- > 0)
                {
                    ai[k++] = (int)((uint)ai1[(j & 0x3f80) + (i >> 7)] >> j8);
                    i += j7;
                    j += l7;
                    ai[k++] = (int)((uint)ai1[(j & 0x3f80) + (i >> 7)] >> j8);
                    i += j7;
                    j += l7;
                    ai[k++] = (int)((uint)ai1[(j & 0x3f80) + (i >> 7)] >> j8);
                    i += j7;
                    j += l7;
                    ai[k++] = (int)((uint)ai1[(j & 0x3f80) + (i >> 7)] >> j8);
                    i += j7;
                    j += l7;
                    ai[k++] = (int)((uint)ai1[(j & 0x3f80) + (i >> 7)] >> j8);
                    i += j7;
                    j += l7;
                    ai[k++] = (int)((uint)ai1[(j & 0x3f80) + (i >> 7)] >> j8);
                    i += j7;
                    j += l7;
                    ai[k++] = (int)((uint)ai1[(j & 0x3f80) + (i >> 7)] >> j8);
                    i += j7;
                    j += l7;
                    ai[k++] = (int)((uint)ai1[(j & 0x3f80) + (i >> 7)] >> j8);
                    i = j4;
                    j = l4;
                    l1 += k2;
                    i2 += l2;
                    j2 += i3;
                    int i6 = j2 >> 14;
                    if (i6 != 0)
                    {
                        j4 = l1 / i6;
                        l4 = i2 / i6;
                        if (j4 < 7)
                        {
                            j4 = 7;
                        }
                        else if (j4 > 16256)
                        {
                            j4 = 16256;
                        }
                    }
                    j7 = j4 - i >> 3;
                    l7 = l4 - j >> 3;
                    j1 += j3;
                    i += j1 & 0x600000;
                    j8 = j1 >> 23;
                }
                for (k3 = i1 - l & 7; k3-- > 0;)
                {
                    ai[k++] = (int)((uint)ai1[(j & 0x3f80) + (i >> 7)] >> j8);
                    i += j7;
                    j += l7;
                }

                return;
            }
            while (k3-- > 0)
            {
                int i9;
                if ((i9 = (int)((uint)ai1[(j & 0x3f80) + (i >> 7)] >> j8)) != 0)
                {
                    ai[k] = i9;
                }
                k++;
                i += j7;
                j += l7;
                if ((i9 = (int)((uint)ai1[(j & 0x3f80) + (i >> 7)] >> j8)) != 0)
                {
                    ai[k] = i9;
                }
                k++;
                i += j7;
                j += l7;
                if ((i9 = (int)((uint)ai1[(j & 0x3f80) + (i >> 7)] >> j8)) != 0)
                {
                    ai[k] = i9;
                }
                k++;
                i += j7;
                j += l7;
                if ((i9 = (int)((uint)ai1[(j & 0x3f80) + (i >> 7)] >> j8)) != 0)
                {
                    ai[k] = i9;
                }
                k++;
                i += j7;
                j += l7;
                if ((i9 = (int)((uint)ai1[(j & 0x3f80) + (i >> 7)] >> j8)) != 0)
                {
                    ai[k] = i9;
                }
                k++;
                i += j7;
                j += l7;
                if ((i9 = (int)((uint)ai1[(j & 0x3f80) + (i >> 7)] >> j8)) != 0)
                {
                    ai[k] = i9;
                }
                k++;
                i += j7;
                j += l7;
                if ((i9 = (int)((uint)ai1[(j & 0x3f80) + (i >> 7)] >> j8)) != 0)
                {
                    ai[k] = i9;
                }
                k++;
                i += j7;
                j += l7;
                if ((i9 = (int)((uint)ai1[(j & 0x3f80) + (i >> 7)] >> j8)) != 0)
                {
                    ai[k] = i9;
                }
                k++;
                i = j4;
                j = l4;
                l1 += k2;
                i2 += l2;
                j2 += i3;
                int j6 = j2 >> 14;
                if (j6 != 0)
                {
                    j4 = l1 / j6;
                    l4 = i2 / j6;
                    if (j4 < 7)
                    {
                        j4 = 7;
                    }
                    else if (j4 > 16256)
                    {
                        j4 = 16256;
                    }
                }
                j7 = j4 - i >> 3;
                l7 = l4 - j >> 3;
                j1 += j3;
                i += j1 & 0x600000;
                j8 = j1 >> 23;
            }
            for (int l3 = i1 - l & 7; l3-- > 0;)
            {
                int j9;
                if ((j9 = (int)((uint)ai1[(j & 0x3f80) + (i >> 7)] >> j8)) != 0)
                {
                    ai[k] = j9;
                }
                k++;
                i += j7;
                j += l7;
            }

        }

        public static void DrawTexturedTriangle(int x1, int y1, int x2, int y2, int x3, int y3, int hsl1, int hsl2, int hsl3, int sx1, int sy1, int sz1, int sx2, int sy2, int sz2, int sx3, int sy3, int sz3, int texture)
        {
            int[] texels = null; // get_texture_pixels(texture);
            Opaque = true;
            sx2 = sx1 - sx2;
            sy2 = sy1 - sy2;
            sz2 = sz1 - sz2;
            sx3 -= sx1;
            sy3 -= sy1;
            sz3 -= sz1;
            int l4 = sx3 * sy1 - sy3 * sx1 << 14;
            int i5 = sy3 * sz1 - sz3 * sy1 << 8;
            int j5 = sz3 * sx1 - sx3 * sz1 << 5;
            int k5 = sx2 * sy1 - sy2 * sx1 << 14;
            int l5 = sy2 * sz1 - sz2 * sy1 << 8;
            int i6 = sz2 * sx1 - sx2 * sz1 << 5;
            int j6 = sy2 * sx3 - sx2 * sy3 << 14;
            int k6 = sz2 * sy3 - sy2 * sz3 << 8;
            int l6 = sx2 * sz3 - sz2 * sx3 << 5;
            int i7 = 0;
            int j7 = 0;
            if (y2 != y1)
            {
                i7 = (x2 - x1 << 16) / (y2 - y1);
                j7 = (hsl2 - hsl1 << 16) / (y2 - y1);
            }
            int k7 = 0;
            int l7 = 0;
            if (y3 != y2)
            {
                k7 = (x3 - x2 << 16) / (y3 - y2);
                l7 = (hsl3 - hsl2 << 16) / (y3 - y2);
            }
            int i8 = 0;
            int j8 = 0;
            if (y3 != y1)
            {
                i8 = (x1 - x3 << 16) / (y1 - y3);
                j8 = (hsl1 - hsl3 << 16) / (y1 - y3);
            }
            if (y1 <= y2 && y1 <= y3)
            {
                if (y1 >= SoftwareRasterizer2D.RightY)
                {
                    return;
                }
                if (y2 > SoftwareRasterizer2D.RightY)
                {
                    y2 = SoftwareRasterizer2D.RightY;
                }
                if (y3 > SoftwareRasterizer2D.RightY)
                {
                    y3 = SoftwareRasterizer2D.RightY;
                }
                if (y2 < y3)
                {
                    x3 = x1 <<= 16;
                    hsl3 = hsl1 <<= 16;
                    if (y1 < 0)
                    {
                        x3 -= i8 * y1;
                        x1 -= i7 * y1;
                        hsl3 -= j8 * y1;
                        hsl1 -= j7 * y1;
                        y1 = 0;
                    }
                    x2 <<= 16;
                    hsl2 <<= 16;
                    if (y2 < 0)
                    {
                        x2 -= k7 * y2;
                        hsl2 -= l7 * y2;
                        y2 = 0;
                    }
                    int k8 = y1 - CenterY;
                    l4 += j5 * k8;
                    k5 += i6 * k8;
                    j6 += l6 * k8;
                    if (y1 != y2 && i8 < i7 || y1 == y2 && i8 > k7)
                    {
                        y3 -= y2;
                        y2 -= y1;
                        y1 = Pixels[y1];
                        while (--y2 >= 0)
                        {
                            DrawTextureScanLine(SoftwareRasterizer2D.Pixels, texels, 0, 0, y1, x3 >> 16, x1 >> 16, hsl3 >> 8, hsl1 >> 8, l4, k5, j6, i5, l5, k6);
                            x3 += i8;
                            x1 += i7;
                            hsl3 += j8;
                            hsl1 += j7;
                            y1 += SoftwareRasterizer2D.Width;
                            l4 += j5;
                            k5 += i6;
                            j6 += l6;
                        }
                        while (--y3 >= 0)
                        {
                            DrawTextureScanLine(SoftwareRasterizer2D.Pixels, texels, 0, 0, y1, x3 >> 16, x2 >> 16, hsl3 >> 8, hsl2 >> 8, l4, k5, j6, i5, l5, k6);
                            x3 += i8;
                            x2 += k7;
                            hsl3 += j8;
                            hsl2 += l7;
                            y1 += SoftwareRasterizer2D.Width;
                            l4 += j5;
                            k5 += i6;
                            j6 += l6;
                        }
                        return;
                    }
                    y3 -= y2;
                    y2 -= y1;
                    y1 = Pixels[y1];
                    while (--y2 >= 0)
                    {
                        DrawTextureScanLine(SoftwareRasterizer2D.Pixels, texels, 0, 0, y1, x1 >> 16, x3 >> 16, hsl1 >> 8, hsl3 >> 8, l4, k5, j6, i5, l5, k6);
                        x3 += i8;
                        x1 += i7;
                        hsl3 += j8;
                        hsl1 += j7;
                        y1 += SoftwareRasterizer2D.Width;
                        l4 += j5;
                        k5 += i6;
                        j6 += l6;
                    }
                    while (--y3 >= 0)
                    {
                        DrawTextureScanLine(SoftwareRasterizer2D.Pixels, texels, 0, 0, y1, x2 >> 16, x3 >> 16, hsl2 >> 8, hsl3 >> 8, l4, k5, j6, i5, l5, k6);
                        x3 += i8;
                        x2 += k7;
                        hsl3 += j8;
                        hsl2 += l7;
                        y1 += SoftwareRasterizer2D.Width;
                        l4 += j5;
                        k5 += i6;
                        j6 += l6;
                    }
                    return;
                }
                x2 = x1 <<= 16;
                hsl2 = hsl1 <<= 16;
                if (y1 < 0)
                {
                    x2 -= i8 * y1;
                    x1 -= i7 * y1;
                    hsl2 -= j8 * y1;
                    hsl1 -= j7 * y1;
                    y1 = 0;
                }
                x3 <<= 16;
                hsl3 <<= 16;
                if (y3 < 0)
                {
                    x3 -= k7 * y3;
                    hsl3 -= l7 * y3;
                    y3 = 0;
                }
                int l8 = y1 - CenterY;
                l4 += j5 * l8;
                k5 += i6 * l8;
                j6 += l6 * l8;
                if (y1 != y3 && i8 < i7 || y1 == y3 && k7 > i7)
                {
                    y2 -= y3;
                    y3 -= y1;
                    y1 = Pixels[y1];
                    while (--y3 >= 0)
                    {
                        DrawTextureScanLine(SoftwareRasterizer2D.Pixels, texels, 0, 0, y1, x2 >> 16, x1 >> 16, hsl2 >> 8, hsl1 >> 8, l4, k5, j6, i5, l5, k6);
                        x2 += i8;
                        x1 += i7;
                        hsl2 += j8;
                        hsl1 += j7;
                        y1 += SoftwareRasterizer2D.Width;
                        l4 += j5;
                        k5 += i6;
                        j6 += l6;
                    }
                    while (--y2 >= 0)
                    {
                        DrawTextureScanLine(SoftwareRasterizer2D.Pixels, texels, 0, 0, y1, x3 >> 16, x1 >> 16, hsl3 >> 8, hsl1 >> 8, l4, k5, j6, i5, l5, k6);
                        x3 += k7;
                        x1 += i7;
                        hsl3 += l7;
                        hsl1 += j7;
                        y1 += SoftwareRasterizer2D.Width;
                        l4 += j5;
                        k5 += i6;
                        j6 += l6;
                    }
                    return;
                }
                y2 -= y3;
                y3 -= y1;
                y1 = Pixels[y1];
                while (--y3 >= 0)
                {
                    DrawTextureScanLine(SoftwareRasterizer2D.Pixels, texels, 0, 0, y1, x1 >> 16, x2 >> 16, hsl1 >> 8, hsl2 >> 8, l4, k5, j6, i5, l5, k6);
                    x2 += i8;
                    x1 += i7;
                    hsl2 += j8;
                    hsl1 += j7;
                    y1 += SoftwareRasterizer2D.Width;
                    l4 += j5;
                    k5 += i6;
                    j6 += l6;
                }
                while (--y2 >= 0)
                {
                    DrawTextureScanLine(SoftwareRasterizer2D.Pixels, texels, 0, 0, y1, x1 >> 16, x3 >> 16, hsl1 >> 8, hsl3 >> 8, l4, k5, j6, i5, l5, k6);
                    x3 += k7;
                    x1 += i7;
                    hsl3 += l7;
                    hsl1 += j7;
                    y1 += SoftwareRasterizer2D.Width;
                    l4 += j5;
                    k5 += i6;
                    j6 += l6;
                }
                return;
            }
            if (y2 <= y3)
            {
                if (y2 >= SoftwareRasterizer2D.RightY)
                {
                    return;
                }
                if (y3 > SoftwareRasterizer2D.RightY)
                {
                    y3 = SoftwareRasterizer2D.RightY;
                }
                if (y1 > SoftwareRasterizer2D.RightY)
                {
                    y1 = SoftwareRasterizer2D.RightY;
                }
                if (y3 < y1)
                {
                    x1 = x2 <<= 16;
                    hsl1 = hsl2 <<= 16;
                    if (y2 < 0)
                    {
                        x1 -= i7 * y2;
                        x2 -= k7 * y2;
                        hsl1 -= j7 * y2;
                        hsl2 -= l7 * y2;
                        y2 = 0;
                    }
                    x3 <<= 16;
                    hsl3 <<= 16;
                    if (y3 < 0)
                    {
                        x3 -= i8 * y3;
                        hsl3 -= j8 * y3;
                        y3 = 0;
                    }
                    int i9 = y2 - CenterY;
                    l4 += j5 * i9;
                    k5 += i6 * i9;
                    j6 += l6 * i9;
                    if (y2 != y3 && i7 < k7 || y2 == y3 && i7 > i8)
                    {
                        y1 -= y3;
                        y3 -= y2;
                        y2 = Pixels[y2];
                        while (--y3 >= 0)
                        {
                            DrawTextureScanLine(SoftwareRasterizer2D.Pixels, texels, 0, 0, y2, x1 >> 16, x2 >> 16, hsl1 >> 8, hsl2 >> 8, l4, k5, j6, i5, l5, k6);
                            x1 += i7;
                            x2 += k7;
                            hsl1 += j7;
                            hsl2 += l7;
                            y2 += SoftwareRasterizer2D.Width;
                            l4 += j5;
                            k5 += i6;
                            j6 += l6;
                        }
                        while (--y1 >= 0)
                        {
                            DrawTextureScanLine(SoftwareRasterizer2D.Pixels, texels, 0, 0, y2, x1 >> 16, x3 >> 16, hsl1 >> 8, hsl3 >> 8, l4, k5, j6, i5, l5, k6);
                            x1 += i7;
                            x3 += i8;
                            hsl1 += j7;
                            hsl3 += j8;
                            y2 += SoftwareRasterizer2D.Width;
                            l4 += j5;
                            k5 += i6;
                            j6 += l6;
                        }
                        return;
                    }
                    y1 -= y3;
                    y3 -= y2;
                    y2 = Pixels[y2];
                    while (--y3 >= 0)
                    {
                        DrawTextureScanLine(SoftwareRasterizer2D.Pixels, texels, 0, 0, y2, x2 >> 16, x1 >> 16, hsl2 >> 8, hsl1 >> 8, l4, k5, j6, i5, l5, k6);
                        x1 += i7;
                        x2 += k7;
                        hsl1 += j7;
                        hsl2 += l7;
                        y2 += SoftwareRasterizer2D.Width;
                        l4 += j5;
                        k5 += i6;
                        j6 += l6;
                    }
                    while (--y1 >= 0)
                    {
                        DrawTextureScanLine(SoftwareRasterizer2D.Pixels, texels, 0, 0, y2, x3 >> 16, x1 >> 16, hsl3 >> 8, hsl1 >> 8, l4, k5, j6, i5, l5, k6);
                        x1 += i7;
                        x3 += i8;
                        hsl1 += j7;
                        hsl3 += j8;
                        y2 += SoftwareRasterizer2D.Width;
                        l4 += j5;
                        k5 += i6;
                        j6 += l6;
                    }
                    return;
                }
                x3 = x2 <<= 16;
                hsl3 = hsl2 <<= 16;
                if (y2 < 0)
                {
                    x3 -= i7 * y2;
                    x2 -= k7 * y2;
                    hsl3 -= j7 * y2;
                    hsl2 -= l7 * y2;
                    y2 = 0;
                }
                x1 <<= 16;
                hsl1 <<= 16;
                if (y1 < 0)
                {
                    x1 -= i8 * y1;
                    hsl1 -= j8 * y1;
                    y1 = 0;
                }
                int j9 = y2 - CenterY;
                l4 += j5 * j9;
                k5 += i6 * j9;
                j6 += l6 * j9;
                if (i7 < k7)
                {
                    y3 -= y1;
                    y1 -= y2;
                    y2 = Pixels[y2];
                    while (--y1 >= 0)
                    {
                        DrawTextureScanLine(SoftwareRasterizer2D.Pixels, texels, 0, 0, y2, x3 >> 16, x2 >> 16, hsl3 >> 8, hsl2 >> 8, l4, k5, j6, i5, l5, k6);
                        x3 += i7;
                        x2 += k7;
                        hsl3 += j7;
                        hsl2 += l7;
                        y2 += SoftwareRasterizer2D.Width;
                        l4 += j5;
                        k5 += i6;
                        j6 += l6;
                    }
                    while (--y3 >= 0)
                    {
                        DrawTextureScanLine(SoftwareRasterizer2D.Pixels, texels, 0, 0, y2, x1 >> 16, x2 >> 16, hsl1 >> 8, hsl2 >> 8, l4, k5, j6, i5, l5, k6);
                        x1 += i8;
                        x2 += k7;
                        hsl1 += j8;
                        hsl2 += l7;
                        y2 += SoftwareRasterizer2D.Width;
                        l4 += j5;
                        k5 += i6;
                        j6 += l6;
                    }
                    return;
                }
                y3 -= y1;
                y1 -= y2;
                y2 = Pixels[y2];
                while (--y1 >= 0)
                {
                    DrawTextureScanLine(SoftwareRasterizer2D.Pixels, texels, 0, 0, y2, x2 >> 16, x3 >> 16, hsl2 >> 8, hsl3 >> 8, l4, k5, j6, i5, l5, k6);
                    x3 += i7;
                    x2 += k7;
                    hsl3 += j7;
                    hsl2 += l7;
                    y2 += SoftwareRasterizer2D.Width;
                    l4 += j5;
                    k5 += i6;
                    j6 += l6;
                }
                while (--y3 >= 0)
                {
                    DrawTextureScanLine(SoftwareRasterizer2D.Pixels, texels, 0, 0, y2, x2 >> 16, x1 >> 16, hsl2 >> 8, hsl1 >> 8, l4, k5, j6, i5, l5, k6);
                    x1 += i8;
                    x2 += k7;
                    hsl1 += j8;
                    hsl2 += l7;
                    y2 += SoftwareRasterizer2D.Width;
                    l4 += j5;
                    k5 += i6;
                    j6 += l6;
                }
                return;
            }
            if (y3 >= SoftwareRasterizer2D.RightY)
            {
                return;
            }
            if (y1 > SoftwareRasterizer2D.RightY)
            {
                y1 = SoftwareRasterizer2D.RightY;
            }
            if (y2 > SoftwareRasterizer2D.RightY)
            {
                y2 = SoftwareRasterizer2D.RightY;
            }
            if (y1 < y2)
            {
                x2 = x3 <<= 16;
                hsl2 = hsl3 <<= 16;
                if (y3 < 0)
                {
                    x2 -= k7 * y3;
                    x3 -= i8 * y3;
                    hsl2 -= l7 * y3;
                    hsl3 -= j8 * y3;
                    y3 = 0;
                }
                x1 <<= 16;
                hsl1 <<= 16;
                if (y1 < 0)
                {
                    x1 -= i7 * y1;
                    hsl1 -= j7 * y1;
                    y1 = 0;
                }
                int k9 = y3 - CenterY;
                l4 += j5 * k9;
                k5 += i6 * k9;
                j6 += l6 * k9;
                if (k7 < i8)
                {
                    y2 -= y1;
                    y1 -= y3;
                    y3 = Pixels[y3];
                    while (--y1 >= 0)
                    {
                        DrawTextureScanLine(SoftwareRasterizer2D.Pixels, texels, 0, 0, y3, x2 >> 16, x3 >> 16, hsl2 >> 8, hsl3 >> 8, l4, k5, j6, i5, l5, k6);
                        x2 += k7;
                        x3 += i8;
                        hsl2 += l7;
                        hsl3 += j8;
                        y3 += SoftwareRasterizer2D.Width;
                        l4 += j5;
                        k5 += i6;
                        j6 += l6;
                    }
                    while (--y2 >= 0)
                    {
                        DrawTextureScanLine(SoftwareRasterizer2D.Pixels, texels, 0, 0, y3, x2 >> 16, x1 >> 16, hsl2 >> 8, hsl1 >> 8, l4, k5, j6, i5, l5, k6);
                        x2 += k7;
                        x1 += i7;
                        hsl2 += l7;
                        hsl1 += j7;
                        y3 += SoftwareRasterizer2D.Width;
                        l4 += j5;
                        k5 += i6;
                        j6 += l6;
                    }
                    return;
                }
                y2 -= y1;
                y1 -= y3;
                y3 = Pixels[y3];
                while (--y1 >= 0)
                {
                    DrawTextureScanLine(SoftwareRasterizer2D.Pixels, texels, 0, 0, y3, x3 >> 16, x2 >> 16, hsl3 >> 8, hsl2 >> 8, l4, k5, j6, i5, l5, k6);
                    x2 += k7;
                    x3 += i8;
                    hsl2 += l7;
                    hsl3 += j8;
                    y3 += SoftwareRasterizer2D.Width;
                    l4 += j5;
                    k5 += i6;
                    j6 += l6;
                }
                while (--y2 >= 0)
                {
                    DrawTextureScanLine(SoftwareRasterizer2D.Pixels, texels, 0, 0, y3, x1 >> 16, x2 >> 16, hsl1 >> 8, hsl2 >> 8, l4, k5, j6, i5, l5, k6);
                    x2 += k7;
                    x1 += i7;
                    hsl2 += l7;
                    hsl1 += j7;
                    y3 += SoftwareRasterizer2D.Width;
                    l4 += j5;
                    k5 += i6;
                    j6 += l6;
                }
                return;
            }
            x1 = x3 <<= 16;
            hsl1 = hsl3 <<= 16;
            if (y3 < 0)
            {
                x1 -= k7 * y3;
                x3 -= i8 * y3;
                hsl1 -= l7 * y3;
                hsl3 -= j8 * y3;
                y3 = 0;
            }
            x2 <<= 16;
            hsl2 <<= 16;
            if (y2 < 0)
            {
                x2 -= i7 * y2;
                hsl2 -= j7 * y2;
                y2 = 0;
            }
            int l9 = y3 - CenterY;
            l4 += j5 * l9;
            k5 += i6 * l9;
            j6 += l6 * l9;
            if (k7 < i8)
            {
                y1 -= y2;
                y2 -= y3;
                y3 = Pixels[y3];
                while (--y2 >= 0)
                {
                    DrawTextureScanLine(SoftwareRasterizer2D.Pixels, texels, 0, 0, y3, x1 >> 16, x3 >> 16, hsl1 >> 8, hsl3 >> 8, l4, k5, j6, i5, l5, k6);
                    x1 += k7;
                    x3 += i8;
                    hsl1 += l7;
                    hsl3 += j8;
                    y3 += SoftwareRasterizer2D.Width;
                    l4 += j5;
                    k5 += i6;
                    j6 += l6;
                }
                while (--y1 >= 0)
                {
                    DrawTextureScanLine(SoftwareRasterizer2D.Pixels, texels, 0, 0, y3, x2 >> 16, x3 >> 16, hsl2 >> 8, hsl3 >> 8, l4, k5, j6, i5, l5, k6);
                    x2 += i7;
                    x3 += i8;
                    hsl2 += j7;
                    hsl3 += j8;
                    y3 += SoftwareRasterizer2D.Width;
                    l4 += j5;
                    k5 += i6;
                    j6 += l6;
                }
                return;
            }
            y1 -= y2;
            y2 -= y3;
            y3 = Pixels[y3];
            while (--y2 >= 0)
            {
                DrawTextureScanLine(SoftwareRasterizer2D.Pixels, texels, 0, 0, y3, x3 >> 16, x1 >> 16, hsl3 >> 8, hsl1 >> 8, l4, k5, j6, i5, l5, k6);
                x1 += k7;
                x3 += i8;
                hsl1 += l7;
                hsl3 += j8;
                y3 += SoftwareRasterizer2D.Width;
                l4 += j5;
                k5 += i6;
                j6 += l6;
            }
            while (--y1 >= 0)
            {
                DrawTextureScanLine(SoftwareRasterizer2D.Pixels, texels, 0, 0, y3, x3 >> 16, x2 >> 16, hsl3 >> 8, hsl2 >> 8, l4, k5, j6, i5, l5, k6);
                x2 += i7;
                x3 += i8;
                hsl2 += j7;
                hsl3 += j8;
                y3 += SoftwareRasterizer2D.Width;
                l4 += j5;
                k5 += i6;
                j6 += l6;
            }
        }
    }
}
