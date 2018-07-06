namespace RS
{
    /// <summary>
    /// Provides utilities for generating random data via perlin noise.
    /// </summary>
    public class PerlinNoise
    {
        private PerlinNoise() { }

        public static int GetLerpedCosine(int a, int b, int x, int frequency)
        {
            int f = (65536 - MathUtils.Cos[x * 1024 / frequency] >> 1);
            return (a * (65536 - f) >> 16) + (b * f >> 16);
        }

        public static int GetPerlinNoise(int x, int y)
        {
            int a = x + y * 57;
            a = a << 13 ^ a;
            int b = a * (a * a * 15731 + 789221) + 1376312589 & 0x7fffffff;
            return b >> 19 & 0xFF;
        }

        public static int GetNoise2D(int x, int y)
        {
            int a = (GetPerlinNoise(x - 1, y - 1) + GetPerlinNoise(x + 1, y - 1) + GetPerlinNoise(x - 1, y + 1) + GetPerlinNoise(x + 1, y + 1));
            int b = (GetPerlinNoise(x - 1, y) + GetPerlinNoise(x + 1, y) + GetPerlinNoise(x, y - 1) + GetPerlinNoise(x, y + 1));
            int c = GetPerlinNoise(x, y);
            return a / 16 + b / 8 + c / 4;
        }

        public static int GetNoise(int a, int b, int amplitude)
        {
            int x = a / amplitude;
            int x1 = a & amplitude - 1;
            int y = b / amplitude;
            int x2 = b & amplitude - 1;
            int a1 = GetNoise2D(x, y);
            int b1 = GetNoise2D(x + 1, y);
            int a2 = GetNoise2D(x, y + 1);
            int b2 = GetNoise2D(x + 1, y + 1);
            int a3 = GetLerpedCosine(a1, b1, x1, amplitude);
            int b3 = GetLerpedCosine(a2, b2, x1, amplitude);
            return GetLerpedCosine(a3, b3, x2, amplitude);
        }

        public static int GetNoiseHeight(int x, int y)
        {
            int height = (GetNoise(x + 45365, y + 91923, 4) - 128 + (GetNoise(x + 10294, y + 37821, 2) - 128 >> 1) + (GetNoise(x, y, 1) - 128 >> 2));
            height = (int)((double)height * 0.3) + 35;

            if (height < 10)
            {
                height = 10;
            }
            else if (height > 60)
            {
                height = 60;
            }

            return height;
        }
    }
}
