namespace RS
{
    /// <summary>
    /// Provides a port of the RS model software renderer.
    /// </summary>
    public static class ModelSoftwareRasterizer
    {
        public const int TypeShadedTri = 0;
        public const int TypeFlatTri = 1;
        public const int TypeShadedTexTri = 2;
        public const int TypeFlatTexTri = 3;

        public static int AnInt1681;
        public static int AnInt1682;
        public static int AnInt1683;
        public static short[] ReplaceVertexX = new short[2000];
        public static short[] ReplaceVertexY = new short[2000];
        public static short[] ReplaceVertexZ = new short[2000];
        public static int[] AnIntArray1625 = new int[2000];
        public static int[] AnIntArray1671 = new int[1500];
        public static int[] AnIntArray1673 = new int[12];
        public static int[] AnIntArray1675 = new int[2000];
        public static int[] AnIntArray1676 = new int[2000];
        public static int[] AnIntArray1677 = new int[12];
        public static int[,] AnIntArrayArray1672 = new int[1500, 512];
        public static int[,] AnIntArrayArray1674 = new int[12, 2000];

        public static int[] TmpScreenX = new int[10];
        public static int[] TmpScreenY = new int[10];
        public static int[] TmpHsl = new int[10];

        public static int[] TmpTexturedX = new int[4096];
        public static int[] TmpTexturedY = new int[4096];
        public static int[] TmpTexturedZ = new int[4096];

        public static int[] TriangleX = new int[4096];
        public static int[] TriangleY = new int[4096];
        public static int[] TriangleDepth = new int[4096];
        public static bool[] TriangleProject = new bool[4096];
        public static bool[] TriangleCheckBounds = new bool[4096];

        public static void DrawTriangle2(Model m, int i)
        {
            var centerX = SoftwareRasterizer3D.CenterX;
            var centerY = SoftwareRasterizer3D.CenterY;

            var j = 0;
            var a = m.TriangleViewspaceA[i];
            var b = m.TriangleViewspaceB[i];
            var c = m.TriangleViewspaceC[i];
            var aZ = TmpTexturedZ[a];
            var bZ = TmpTexturedZ[b];
            var cZ = TmpTexturedZ[c];
            if (aZ >= 50)
            {
                TmpScreenX[j] = TriangleX[a];
                TmpScreenY[j] = TriangleY[a];
                TmpHsl[j++] = m.TriHsl1[i];
            }
            else
            {
                int x = TmpTexturedX[a];
                int y = TmpTexturedY[a];
                int hsl = m.TriHsl1[i];

                if (cZ >= 50)
                {
                    int decay = (50 - aZ) * SoftwareRasterizer3D.ShadowDecay[cZ - aZ];
                    TmpScreenX[j] = centerX + (x + ((TmpTexturedX[c] - x) * decay >> 16) << 9) / 50;
                    TmpScreenY[j] = centerY + (y + ((TmpTexturedY[c] - y) * decay >> 16) << 9) / 50;
                    TmpHsl[j++] = hsl + ((m.TriHsl3[i] - hsl) * decay >> 16);
                }

                if (bZ >= 50)
                {
                    int decay = (50 - aZ) * SoftwareRasterizer3D.ShadowDecay[bZ - aZ];
                    TmpScreenX[j] = centerX + (x + ((TmpTexturedX[b] - x) * decay >> 16) << 9) / 50;
                    TmpScreenY[j] = centerY + (y + ((TmpTexturedY[b] - y) * decay >> 16) << 9) / 50;
                    TmpHsl[j++] = hsl + ((m.TriHsl2[i] - hsl) * decay >> 16);
                }
            }

            if (bZ >= 50)
            {
                TmpScreenX[j] = TriangleX[b];
                TmpScreenY[j] = TriangleY[b];
                TmpHsl[j++] = m.TriHsl2[i];
            }
            else
            {
                int x = TmpTexturedX[b];
                int y = TmpTexturedY[b];
                int hsl = m.TriHsl2[i];

                if (aZ >= 50)
                {
                    int i6 = (50 - bZ) * SoftwareRasterizer3D.ShadowDecay[aZ - bZ];
                    TmpScreenX[j] = centerX + (x + ((TmpTexturedX[a] - x) * i6 >> 16) << 9) / 50;
                    TmpScreenY[j] = centerY + (y + ((TmpTexturedY[a] - y) * i6 >> 16) << 9) / 50;
                    TmpHsl[j++] = hsl + ((m.TriHsl1[i] - hsl) * i6 >> 16);
                }

                if (cZ >= 50)
                {
                    int j6 = (50 - bZ) * SoftwareRasterizer3D.ShadowDecay[cZ - bZ];
                    TmpScreenX[j] = centerX + (x + ((TmpTexturedX[c] - x) * j6 >> 16) << 9) / 50;
                    TmpScreenY[j] = centerY + (y + ((TmpTexturedY[c] - y) * j6 >> 16) << 9) / 50;
                    TmpHsl[j++] = hsl + ((m.TriHsl3[i] - hsl) * j6 >> 16);
                }
            }

            if (cZ >= 50)
            {
                TmpScreenX[j] = TriangleX[c];
                TmpScreenY[j] = TriangleY[c];
                TmpHsl[j++] = m.TriHsl3[i];
            }
            else
           {
                var x = TmpTexturedX[c];
                var y = TmpTexturedY[c];
                var hsl = m.TriHsl3[i];
                if (bZ >= 50)
                {
                    var k6 = (50 - cZ) * SoftwareRasterizer3D.ShadowDecay[bZ - cZ];
                    TmpScreenX[j] = centerX + (x + ((TmpTexturedX[b] - x) * k6 >> 16) << 9) / 50;
                    TmpScreenY[j] = centerY + (y + ((TmpTexturedY[b] - y) * k6 >> 16) << 9) / 50;
                    TmpHsl[j++] = hsl + ((m.TriHsl2[i] - hsl) * k6 >> 16);
                }
                if (aZ >= 50)
                {
                    var l6 = (50 - cZ) * SoftwareRasterizer3D.ShadowDecay[aZ - cZ];
                    TmpScreenX[j] = centerX + (x + ((TmpTexturedX[a] - x) * l6 >> 16) << 9) / 50;
                    TmpScreenY[j] = centerY + (y + ((TmpTexturedY[a] - y) * l6 >> 16) << 9) / 50;
                    TmpHsl[j++] = hsl + ((m.TriHsl2[i] - hsl) * l6 >> 16);
                }
            }

            var x0 = TmpScreenX[0];
            var x1 = TmpScreenX[1];
            var x2 = TmpScreenX[2];
            var y0 = TmpScreenY[0];
            var y1 = TmpScreenY[1];
            var y2 = TmpScreenY[2];
            if ((x0 - x1) * (y2 - y1) - (y0 - y1) * (x2 - x1) > 0)
            {
                SoftwareRasterizer3D.CheckBounds = false;
                if (j == 3)
                {
                    if (x0 < 0 || x1 < 0 || x2 < 0 || x0 > SoftwareRasterizer2D.Bound || x1 > SoftwareRasterizer2D.Bound || x2 > SoftwareRasterizer2D.Bound)
                    {
                        SoftwareRasterizer3D.CheckBounds = true;
                    }

                    var type = 0;
                    if (m.TriangleInfo != null)
                    {
                        type = m.TriangleInfo[i] & 3;
                    }

                    if (type == TypeShadedTri)
                    {
                        SoftwareRasterizer3D.DrawShadedTriangle(x0, y0, x1, y1, x2, y2, TmpHsl[0], TmpHsl[1], TmpHsl[2]);
                    }
                    else if (type == TypeFlatTri)
                    {
                        SoftwareRasterizer3D.DrawFlatTriangle(x0, y0, x1, y1, x2, y2, ColorUtils.HSLToRGBMap[m.TriHsl1[i]]);
                    }
                    else if (type == TypeShadedTexTri)
                    {
                        var k = m.TriangleInfo[i] >> 2;
                        var x = m.TextureMapX[k];
                        var y = m.TextureMapY[k];
                        var z = m.TextureMapZ[k];
                        SoftwareRasterizer3D.DrawShadedTriangle(x0, y0, x1, y1, x2, y2, TmpHsl[0], TmpHsl[1], TmpHsl[2]);
                    }
                    else if (type == TypeFlatTexTri)
                    {
                        var k = m.TriangleInfo[i] >> 2;
                        var x = m.TextureMapX[k];
                        var y = m.TextureMapY[k];
                        var z = m.TextureMapZ[k];
                        SoftwareRasterizer3D.DrawShadedTriangle(x0, y0, x1, y1, x2, y2, TmpHsl[0], TmpHsl[1], TmpHsl[2]);
                    }
                }
                else if (j == 4)
                {
                    if (x0 < 0 || x1 < 0 || x2 < 0 || x0 > SoftwareRasterizer2D.Bound || x1 > SoftwareRasterizer2D.Bound || x2 > SoftwareRasterizer2D.Bound || TmpScreenX[3] < 0 || TmpScreenX[3] > SoftwareRasterizer2D.Bound)
                    {
                        SoftwareRasterizer3D.CheckBounds = true;
                    }

                    var type = 0;
                    if (m.TriangleInfo != null)
                    {
                        type = m.TriangleInfo[i] & 3;
                    }

                    if (type == TypeShadedTri)
                    {
                        SoftwareRasterizer3D.DrawShadedTriangle(x0, y0, x1, y1, x2, y2, TmpHsl[0], TmpHsl[1], TmpHsl[2]);
                        SoftwareRasterizer3D.DrawShadedTriangle(x0, y0, x2, y2, TmpScreenX[3], TmpScreenY[3], TmpHsl[0], TmpHsl[2], TmpHsl[3]);
                    }
                    else if (type == TypeFlatTri)
                    {
                        var rgb = ColorUtils.HSLToRGBMap[m.TriHsl1[i]];
                        SoftwareRasterizer3D.DrawFlatTriangle(x0, y0, x1, y1, x2, y2, rgb);
                        SoftwareRasterizer3D.DrawFlatTriangle(x0, y0, x2, y2, TmpScreenX[3], TmpScreenY[3], rgb);
                    }
                    else if (type == TypeShadedTexTri)
                    {
                        var k = m.TriangleInfo[i] >> 2;
                        var x = m.TextureMapX[k];
                        var y = m.TextureMapY[k];
                        var z = m.TextureMapZ[k];
                        SoftwareRasterizer3D.DrawShadedTriangle(x0, y0, x1, y1, x2, y2, TmpHsl[0], TmpHsl[1], TmpHsl[2]);
                        SoftwareRasterizer3D.DrawShadedTriangle(x0, y0, x2, y2, TmpScreenX[3], TmpScreenY[3], TmpHsl[0], TmpHsl[2], TmpHsl[3]);
                    }
                    else if (type == TypeFlatTexTri)
                    {
                        var k = m.TriangleInfo[i] >> 2;
                        var x = m.TextureMapX[k];
                        var y = m.TextureMapY[k];
                        var z = m.TextureMapZ[k];
                        SoftwareRasterizer3D.DrawShadedTriangle(x0, y0, x1, y1, x2, y2, TmpHsl[0], TmpHsl[1], TmpHsl[2]);
                        SoftwareRasterizer3D.DrawShadedTriangle(x0, y0, x2, y2, TmpScreenX[3], TmpScreenY[3], TmpHsl[0], TmpHsl[2], TmpHsl[3]);
                    }
                }
            }
        }

        public static void DrawTriangle(Model m, int idx)
        {
            if (TriangleProject[idx])
            {
                DrawTriangle2(m, idx);
                return;
            }

            var firstTriVertex = m.TriangleViewspaceA[idx];
            var secondTriVertex = m.TriangleViewspaceB[idx];
            var thirdTriVertex = m.TriangleViewspaceC[idx];
            SoftwareRasterizer3D.CheckBounds = TriangleCheckBounds[idx];
            if (m.TriangleAlpha == null)
            {
                SoftwareRasterizer3D.Opacity = 0;
            }
            else
            {
                SoftwareRasterizer3D.Opacity = m.TriangleAlpha[idx];
            }

            var type = 0;
            if (m.TriangleInfo != null)
            {
                type = m.TriangleInfo[idx] & 3;
            }

            switch (type)
            {
                case TypeShadedTri:
                    {
                        SoftwareRasterizer3D.DrawShadedTriangle(TriangleX[firstTriVertex], TriangleY[firstTriVertex], TriangleX[secondTriVertex], TriangleY[secondTriVertex], TriangleX[thirdTriVertex], TriangleY[thirdTriVertex], m.TriHsl1[idx], m.TriHsl2[idx], m.TriHsl3[idx]);
                        return;
                    }
                case TypeFlatTri:
                    {
                        SoftwareRasterizer3D.DrawFlatTriangle(TriangleX[firstTriVertex], TriangleY[firstTriVertex], TriangleX[secondTriVertex], TriangleY[secondTriVertex], TriangleX[thirdTriVertex], TriangleY[thirdTriVertex], ColorUtils.HSLToRGBMap[m.TriHsl1[idx]]);
                        return;
                    }
                case TypeShadedTexTri:
                    {
                        var j = m.TriangleInfo[idx] >> 2;
                        var x = m.TextureMapX[j];
                        var y = m.TextureMapY[j];
                        var z = m.TextureMapZ[j];
                        SoftwareRasterizer3D.DrawShadedTriangle(TriangleX[firstTriVertex], TriangleY[firstTriVertex], TriangleX[secondTriVertex], TriangleY[secondTriVertex], TriangleX[thirdTriVertex], TriangleY[thirdTriVertex], m.TriHsl1[idx], m.TriHsl2[idx], m.TriHsl3[idx]);
                        return;
                    }
                case TypeFlatTexTri:
                    {
                        var j = m.TriangleInfo[idx] >> 2;
                        var x = m.TextureMapX[j];
                        var y = m.TextureMapY[j];
                        var z = m.TextureMapZ[j];
                        SoftwareRasterizer3D.DrawShadedTriangle(TriangleX[firstTriVertex], TriangleY[firstTriVertex], TriangleX[secondTriVertex], TriangleY[secondTriVertex], TriangleX[thirdTriVertex], TriangleY[thirdTriVertex], m.TriHsl1[idx], m.TriHsl2[idx], m.TriHsl3[idx]);
                        return;
                    }
            }
        }

        public static void Render(Model m, bool project, bool hoverable, int uid)
        {
            for (var j = 0; j < m.Unknown3; j++)
            {
                if (j >= AnIntArray1671.Length) j = 0;
                AnIntArray1671[j] = 0;
            }

            for (var i = 0; i < m.TriangleCount; i++)
            {
                if (m.TriangleInfo == null || m.TriangleInfo[i] != -1)
                {
                    var a = m.TriangleViewspaceA[i];
                    if (a < 0 || a >= m.TriangleCount) a = 0;

                    var b = m.TriangleViewspaceB[i];
                    if (b < 0 || b >= m.TriangleCount) b = 0;

                    var c = m.TriangleViewspaceC[i];
                    if (c < 0 || c >= m.TriangleCount) c = 0;

                    var x1 = TriangleX[a];
                    var x2 = TriangleX[b];
                    var x3 = TriangleX[c];
                    if (project && (x1 == -5000 || x2 == -5000 || x3 == -5000))
                    {
                        TriangleProject[i] = true;
                        var depth = (TriangleDepth[a] + TriangleDepth[b] + TriangleDepth[c]) / 3 + m.Unknown2;
                        AnIntArrayArray1672[depth, AnIntArray1671[depth]++] = i;
                    }
                    else
                    {
                        if ((x1 - x2) * (TriangleY[c] - TriangleY[b]) - (TriangleY[a] - TriangleY[b]) * (x3 - x2) > 0)
                        {
                            TriangleProject[i] = false;
                            if (x1 < 0 || x2 < 0 || x3 < 0 || x1 > SoftwareRasterizer2D.Bound || x2 > SoftwareRasterizer2D.Bound || x3 > SoftwareRasterizer2D.Bound)
                            {
                                TriangleCheckBounds[i] = true;
                            }
                            else
                            {
                                TriangleCheckBounds[i] = false;
                            }

                            var depth = (TriangleDepth[a] + TriangleDepth[b] + TriangleDepth[c]) / 3 + m.Unknown2;
                            AnIntArrayArray1672[depth, AnIntArray1671[depth]++] = i;
                        }
                    }
                }
            }

            if (m.TrianglePriority == null)
            {
                for (var e = m.Unknown3 - 1; e >= 0; e--)
                {
                    var l1 = AnIntArray1671[e];
                    if (l1 > 0)
                    {
                        for (var k = 0; k < l1; k++)
                        {
                            DrawTriangle(m, AnIntArrayArray1672[e, k]);
                        }
                    }
                }
                return;
            }

            for (var priority = 0; priority < 12; priority++)
            {
                AnIntArray1673[priority] = 0;
                AnIntArray1677[priority] = 0;
            }

            for (var xx = m.Unknown3 - 1; xx >= 0; xx--)
            {
                var k2 = AnIntArray1671[xx];
                if (k2 > 0)
                {
                    for (var j = 0; j < k2; j++)
                    {
                        var triangle = AnIntArrayArray1672[xx, j];
                        var priority = m.TrianglePriority[triangle];
                        var j6 = AnIntArray1673[priority]++;
                        AnIntArrayArray1674[priority, j6] = triangle;

                        if (priority < 10)
                        {
                            AnIntArray1677[priority] += xx;
                        }
                        else if (priority == 10)
                        {
                            AnIntArray1675[j6] = xx;
                        }
                        else
                        {
                            AnIntArray1676[j6] = xx;
                        }
                    }

                }
            }

            var l2 = 0;
            if (AnIntArray1673[1] > 0 || AnIntArray1673[2] > 0)
            {
                l2 = (AnIntArray1677[1] + AnIntArray1677[2]) / (AnIntArray1673[1] + AnIntArray1673[2]);
            }

            var k3 = 0;
            if (AnIntArray1673[3] > 0 || AnIntArray1673[4] > 0)
            {
                k3 = (AnIntArray1677[3] + AnIntArray1677[4]) / (AnIntArray1673[3] + AnIntArray1673[4]);
            }

            var j4 = 0;
            if (AnIntArray1673[6] > 0 || AnIntArray1673[8] > 0)
            {
                j4 = (AnIntArray1677[6] + AnIntArray1677[8]) / (AnIntArray1673[6] + AnIntArray1673[8]);
            }

            var iii = 0;
            var k6 = AnIntArray1673[10];
            var idx = 10;
            var ai3 = AnIntArray1675;
            if (iii == k6)
            {
                iii = 0;
                k6 = AnIntArray1673[11];
                idx = 11;
                ai3 = AnIntArray1676;
            }

            var i5 = -1000;
            if (iii < k6)
            {
                i5 = ai3[iii];
            }

            for (var l6 = 0; l6 < 10; l6++)
            {
                while (l6 == 0 && i5 > l2)
                {
                    DrawTriangle(m, AnIntArrayArray1674[idx, iii++]);
                    if (iii == k6 && idx != 11)
                    {
                        iii = 0;
                        k6 = AnIntArray1673[11];
                        idx = 11;
                        ai3 = AnIntArray1676;
                    }

                    if (iii < k6)
                    {
                        i5 = ai3[iii];
                    }
                    else
                    {
                        i5 = -1000;
                    }
                }

                while (l6 == 3 && i5 > k3)
                {
                    DrawTriangle(m, AnIntArrayArray1674[idx, iii++]);
                    if (iii == k6 && idx != 11)
                    {
                        iii = 0;
                        k6 = AnIntArray1673[11];
                        idx = 11;
                        ai3 = AnIntArray1676;
                    }
                    if (iii < k6)
                    {
                        i5 = ai3[iii];
                    }
                    else
                    {
                        i5 = -1000;
                    }
                }

                while (l6 == 5 && i5 > j4)
                {
                    DrawTriangle(m, AnIntArrayArray1674[idx, iii++]);
                    if (iii == k6 && idx != 11)
                    {
                        iii = 0;
                        k6 = AnIntArray1673[11];
                        idx = 11;
                        ai3 = AnIntArray1676;
                    }
                    if (iii < k6)
                    {
                        i5 = ai3[iii];
                    }
                    else
                    {
                        i5 = -1000;
                    }
                }
                int i7 = AnIntArray1673[l6];
                for (int j7 = 0; j7 < i7; j7++)
                {
                    DrawTriangle(m, AnIntArrayArray1674[l6, j7]);
                }
            }

            while (i5 != -1000)
            {
                DrawTriangle(m, AnIntArrayArray1674[idx, iii++]);
                if (iii == k6 && idx != 11)
                {
                    iii = 0;
                    idx = 11;
                    k6 = AnIntArray1673[11];
                    ai3 = AnIntArray1676;
                }
                if (iii < k6)
                {
                    i5 = ai3[iii];
                }
                else
                {
                    i5 = -1000;
                }
            }
        }

        public static void Render(Model m, int pitch, int yaw, int roll, int camPitch, int camera_x, int camY, int camZ)
        {
            var centerX = SoftwareRasterizer3D.CenterX;
            var centerY = SoftwareRasterizer3D.CenterY;
            var pitchSin = MathUtils.Sin[pitch];
            var pitchCos = MathUtils.Cos[pitch];
            var yawSin = MathUtils.Sin[yaw];
            var yawCos = MathUtils.Cos[yaw];
            var rollSin = MathUtils.Sin[roll];
            var rollCos = MathUtils.Cos[roll];
            var arcSin = MathUtils.Sin[camPitch];
            var arcCos = MathUtils.Cos[camPitch];
            var camDist = camY * arcSin + camZ * arcCos >> 16;

            for (var i = 0; i < m.VertexCount; i++)
            {
                var x = m.VertexX[i];
                var y = m.VertexY[i];
                var z = m.VertexZ[i];
                if (roll != 0)
                {
                    var x2 = y * rollSin + x * rollCos >> 16;
                    y = y * rollCos - x * rollSin >> 16;
                    x = x2;
                }

                if (pitch != 0)
                {
                    var y2x = y * pitchCos - z * pitchSin >> 16;
                    z = y * pitchSin + z * pitchCos >> 16;
                    y = y2x;
                }

                if (yaw != 0)
                {
                    var x2 = z * yawSin + x * yawCos >> 16;
                    z = z * yawCos - x * yawSin >> 16;
                    x = x2;
                }

                x += camera_x;
                y += camY;
                z += camZ;

                var y2 = y * arcCos - z * arcSin >> 16;
                z = y * arcSin + z * arcCos >> 16;
                y = y2;

                TriangleDepth[i] = z - camDist;
                TriangleX[i] = centerX + (x << 9) / z;
                TriangleY[i] = centerY + (y << 9) / z;
                if (m.TexturedTriangleCount > 0)
                {
                    TmpTexturedX[i] = x;
                    TmpTexturedY[i] = y;
                    TmpTexturedZ[i] = z;
                }
            }

            Render(m, false, false, 0);
        }
    }
}
