namespace RS
{
    /// <summary>
    /// Provides a port of the 2D RS software renderer
    /// </summary>
    public static class SoftwareRasterizer2D
    {
        public static int Alpha;
        public static int Width;
        public static int Height;
        public static int CenterY;
        public static int CenterX;
        public static int Bound;
        public static int LeftX;
        public static int LeftY;
        public static int RightX;
        public static int RightY;
        public static int[] Pixels;

        public static void FillRect(int x, int y, int width, int height, int color)
        {
            if (x < LeftX)
            {
                width -= LeftX - x;
                x = LeftX;
            }

            if (y < LeftY)
            {
                height -= LeftY - y;
                y = LeftY;
            }

            if (x + width > RightX)
            {
                width = RightX - x;
            }

            if (y + height > RightY)
            {
                height = RightY - y;
            }

            var step = Width - width;
            var position = x + y * Width;
            for (var cx = -height; cx < 0; cx++)
            {
                for (var cy = -width; cy < 0; cy++)
                {
                    Pixels[position++] = color;
                }
                position += step;
            }
        }

        public static void Reset()
        {
            LeftX = 0;
            LeftY = 0;
            RightX = Width;
            RightY = Height;
            Bound = RightX - 1;
            CenterX = RightX / 2;
            CenterY = RightY / 2;
        }

        public static void Prepare(int width, int height, int[] pixels)
        {
            Pixels = pixels;
            Width = width;
            Height = height;
            SetBounds(0, 0, width, height);
        }

        public static void SetBounds(int x0, int y0, int x1, int y1)
        {
            if (x0 < 0)
            {
                x0 = 0;
            }

            if (y0 < 0)
            {
                y0 = 0;
            }

            if (x1 > Width)
            {
                x1 = Width;
            }

            if (y1 > Height)
            {
                y1 = Height;
            }

            LeftX = x0;
            LeftY = y0;
            RightX = x1;
            RightY = y1;
            Bound = RightX - 1;
            CenterX = RightX / 2;
            CenterY = RightY / 2;
        }
    }
}
