namespace RS
{
    /// <summary>
    /// Contains information about tile collision within the loaded map.
    /// </summary>
    public class CollisionMap
    {
        public int[,] Flags;
        public int Width;
        public int Height;

        public CollisionMap(int width, int height)
        {
            Width = width;
            Height = height;
            Flags = new int[Width, Height];
            SetDefaults();
        }

        public void Add(int x, int y, int flag)
        {
            Flags[x, y] |= flag;
        }

        public void AddObject(int locX, int locY, int sizeX, int sizeY, int rotation, bool blocksProjectiles)
        {
            var flag = 256;
            if (blocksProjectiles)
            {
                flag += 0x20000;
            }

            if (rotation == 1 || rotation == 3)
            {
                var tmp = sizeX;
                sizeX = sizeY;
                sizeY = tmp;
            }

            for (var x = locX; x < locX + sizeX; x++)
            {
                if (x >= 0 && x < Width)
                {
                    for (var y = locY; y < locY + sizeY; y++)
                    {
                        if (y >= 0 && y < Height)
                        {
                            Add(x, y, flag);
                        }
                    }
                }
            }
        }

        public void AddWall(int x, int y, int type, int rotation, bool blocksProjectiles)
        {
            if (type == 0)
            {
                if (rotation == 0)
                {
                    Add(x, y, 128);
                    Add(x - 1, y, 8);
                }
                if (rotation == 1)
                {
                    Add(x, y, 2);
                    Add(x, y + 1, 32);
                }
                if (rotation == 2)
                {
                    Add(x, y, 8);
                    Add(x + 1, y, 128);
                }
                if (rotation == 3)
                {
                    Add(x, y, 32);
                    Add(x, y - 1, 2);
                }
            }
            if (type == 1 || type == 3)
            {
                if (rotation == 0)
                {
                    Add(x, y, 1);
                    Add(x - 1, y + 1, 16);
                }
                if (rotation == 1)
                {
                    Add(x, y, 4);
                    Add(x + 1, y + 1, 64);
                }
                if (rotation == 2)
                {
                    Add(x, y, 16);
                    Add(x + 1, y - 1, 1);
                }
                if (rotation == 3)
                {
                    Add(x, y, 64);
                    Add(x - 1, y - 1, 4);
                }
            }
            if (type == 2)
            {
                if (rotation == 0)
                {
                    Add(x, y, 130);
                    Add(x - 1, y, 8);
                    Add(x, y + 1, 32);
                }
                if (rotation == 1)
                {
                    Add(x, y, 10);
                    Add(x, y + 1, 32);
                    Add(x + 1, y, 128);
                }
                if (rotation == 2)
                {
                    Add(x, y, 40);
                    Add(x + 1, y, 128);
                    Add(x, y - 1, 2);
                }
                if (rotation == 3)
                {
                    Add(x, y, 160);
                    Add(x, y - 1, 2);
                    Add(x - 1, y, 8);
                }
            }
            if (blocksProjectiles)
            {
                if (type == 0)
                {
                    if (rotation == 0)
                    {
                        Add(x, y, 0x10000);
                        Add(x - 1, y, 4096);
                    }
                    if (rotation == 1)
                    {
                        Add(x, y, 1024);
                        Add(x, y + 1, 16384);
                    }
                    if (rotation == 2)
                    {
                        Add(x, y, 4096);
                        Add(x + 1, y, 0x10000);
                    }
                    if (rotation == 3)
                    {
                        Add(x, y, 16384);
                        Add(x, y - 1, 1024);
                    }
                }
                if (type == 1 || type == 3)
                {
                    if (rotation == 0)
                    {
                        Add(x, y, 512);
                        Add(x - 1, y + 1, 8192);
                    }
                    if (rotation == 1)
                    {
                        Add(x, y, 2048);
                        Add(x + 1, y + 1, 32768);
                    }
                    if (rotation == 2)
                    {
                        Add(x, y, 8192);
                        Add(x + 1, y - 1, 512);
                    }
                    if (rotation == 3)
                    {
                        Add(x, y, 32768);
                        Add(x - 1, y - 1, 2048);
                    }
                }
                if (type == 2)
                {
                    if (rotation == 0)
                    {
                        Add(x, y, 0x10400);
                        Add(x - 1, y, 4096);
                        Add(x, y + 1, 16384);
                    }
                    if (rotation == 1)
                    {
                        Add(x, y, 5120);
                        Add(x, y + 1, 16384);
                        Add(x + 1, y, 0x10000);
                    }
                    if (rotation == 2)
                    {
                        Add(x, y, 20480);
                        Add(x + 1, y, 0x10000);
                        Add(x, y - 1, 1024);
                    }
                    if (rotation == 3)
                    {
                        Add(x, y, 0x14000);
                        Add(x, y - 1, 1024);
                        Add(x - 1, y, 4096);
                    }
                }
            }
        }

        public bool AtDecoration(int x, int y, int destX, int destY, int type, int rotation)
        {
            if (x == destX && y == destY)
            {
                return true;
            }
            if (type == 6 || type == 7)
            {
                if (type == 7)
                {
                    rotation = rotation + 2 & 3;
                }
                if (rotation == 0)
                {
                    if (x == destX + 1 && y == destY && (Flags[x, y] & 0x80) == 0)
                    {
                        return true;
                    }
                    if (x == destX && y == destY - 1 && (Flags[x, y] & 2) == 0)
                    {
                        return true;
                    }
                }
                else if (rotation == 1)
                {
                    if (x == destX - 1 && y == destY && (Flags[x, y] & 8) == 0)
                    {
                        return true;
                    }
                    if (x == destX && y == destY - 1 && (Flags[x, y] & 2) == 0)
                    {
                        return true;
                    }
                }
                else if (rotation == 2)
                {
                    if (x == destX - 1 && y == destY && (Flags[x, y] & 8) == 0)
                    {
                        return true;
                    }
                    if (x == destX && y == destY + 1 && (Flags[x, y] & 0x20) == 0)
                    {
                        return true;
                    }
                }
                else if (rotation == 3)
                {
                    if (x == destX + 1 && y == destY && (Flags[x, y] & 0x80) == 0)
                    {
                        return true;
                    }
                    if (x == destX && y == destY + 1 && (Flags[x, y] & 0x20) == 0)
                    {
                        return true;
                    }
                }
            }
            if (type == 8)
            {
                if (x == destX && y == destY + 1 && (Flags[x, y] & 0x20) == 0)
                {
                    return true;
                }
                if (x == destX && y == destY - 1 && (Flags[x, y] & 2) == 0)
                {
                    return true;
                }
                if (x == destX - 1 && y == destY && (Flags[x, y] & 8) == 0)
                {
                    return true;
                }
                if (x == destX + 1 && y == destY && (Flags[x, y] & 0x80) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool AtObject(int x, int y, int destX, int destY, int sizeX, int sizeY, int face)
        {
            int maxX = (destX + sizeX) - 1;
            int maxY = (destY + sizeY) - 1;

            if (x >= destX && x <= maxX && y >= destY && y <= maxY)
            {
                return true;
            }

            if (x == destX - 1 && y >= destY && y <= maxY && (Flags[x, y] & 0x8) == 0 && (face & 0x8) == 0)
            {
                return true;
            }

            if (x == maxX + 1 && y >= destY && y <= maxY && (Flags[x, y] & 0x80) == 0 && (face & 0x2) == 0)
            {
                return true;
            }

            if (y == destY - 1 && x >= destX && x <= maxX && (Flags[x, y] & 0x2) == 0 && (face & 0x4) == 0)
            {
                return true;
            }

            return y == maxY + 1 && x >= destX && x <= maxX && (Flags[x, y] & 0x20) == 0 && (face & 0x1) == 0;
        }

        public bool AtWall(int x, int y, int destX, int destY, int type, int rotation)
        {
            if (x == destX && y == destY)
            {
                return true;
            }
            if (type == 0)
            {
                if (rotation == 0)
                {
                    if (x == destX - 1 && y == destY)
                    {
                        return true;
                    }
                    if (x == destX && y == destY + 1 && (Flags[x, y] & 0x1280120) == 0)
                    {
                        return true;
                    }
                    if (x == destX && y == destY - 1 && (Flags[x, y] & 0x1280102) == 0)
                    {
                        return true;
                    }
                }
                else if (rotation == 1)
                {
                    if (x == destX && y == destY + 1)
                    {
                        return true;
                    }
                    if (x == destX - 1 && y == destY && (Flags[x, y] & 0x1280108) == 0)
                    {
                        return true;
                    }
                    if (x == destX + 1 && y == destY && (Flags[x, y] & 0x1280180) == 0)
                    {
                        return true;
                    }
                }
                else if (rotation == 2)
                {
                    if (x == destX + 1 && y == destY)
                    {
                        return true;
                    }
                    if (x == destX && y == destY + 1 && (Flags[x, y] & 0x1280120) == 0)
                    {
                        return true;
                    }
                    if (x == destX && y == destY - 1 && (Flags[x, y] & 0x1280102) == 0)
                    {
                        return true;
                    }
                }
                else if (rotation == 3)
                {
                    if (x == destX && y == destY - 1)
                    {
                        return true;
                    }
                    if (x == destX - 1 && y == destY && (Flags[x, y] & 0x1280108) == 0)
                    {
                        return true;
                    }
                    if (x == destX + 1 && y == destY && (Flags[x, y] & 0x1280180) == 0)
                    {
                        return true;
                    }
                }
            }
            if (type == 2)
            {
                if (rotation == 0)
                {
                    if (x == destX - 1 && y == destY)
                    {
                        return true;
                    }
                    if (x == destX && y == destY + 1)
                    {
                        return true;
                    }
                    if (x == destX + 1 && y == destY && (Flags[x, y] & 0x1280180) == 0)
                    {
                        return true;
                    }
                    if (x == destX && y == destY - 1 && (Flags[x, y] & 0x1280102) == 0)
                    {
                        return true;
                    }
                }
                else if (rotation == 1)
                {
                    if (x == destX - 1 && y == destY && (Flags[x, y] & 0x1280108) == 0)
                    {
                        return true;
                    }
                    if (x == destX && y == destY + 1)
                    {
                        return true;
                    }
                    if (x == destX + 1 && y == destY)
                    {
                        return true;
                    }
                    if (x == destX && y == destY - 1 && (Flags[x, y] & 0x1280102) == 0)
                    {
                        return true;
                    }
                }
                else if (rotation == 2)
                {
                    if (x == destX - 1 && y == destY && (Flags[x, y] & 0x1280108) == 0)
                    {
                        return true;
                    }
                    if (x == destX && y == destY + 1 && (Flags[x, y] & 0x1280120) == 0)
                    {
                        return true;
                    }
                    if (x == destX + 1 && y == destY)
                    {
                        return true;
                    }
                    if (x == destX && y == destY - 1)
                    {
                        return true;
                    }
                }
                else if (rotation == 3)
                {
                    if (x == destX - 1 && y == destY)
                    {
                        return true;
                    }
                    if (x == destX && y == destY + 1 && (Flags[x, y] & 0x1280120) == 0)
                    {
                        return true;
                    }
                    if (x == destX + 1 && y == destY && (Flags[x, y] & 0x1280180) == 0)
                    {
                        return true;
                    }
                    if (x == destX && y == destY - 1)
                    {
                        return true;
                    }
                }
            }
            if (type == 9)
            {
                if (x == destX && y == destY + 1 && (Flags[x, y] & 0x20) == 0)
                {
                    return true;
                }
                if (x == destX && y == destY - 1 && (Flags[x, y] & 2) == 0)
                {
                    return true;
                }
                if (x == destX - 1 && y == destY && (Flags[x, y] & 8) == 0)
                {
                    return true;
                }
                if (x == destX + 1 && y == destY && (Flags[x, y] & 0x80) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public void SetDefaults()
        {
            var borderX = Width - 1;
            var borderY = Height - 1;
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    if (x == 0 || y == 0 || x == borderX || y == borderY)
                    {
                        Flags[x, y] = 0xFFFFFF;
                    }
                    else
                    {
                        Flags[x, y] = 0x0000000;
                    }
                }
            }
        }

        public void method218(int x, int y)
        {
            Flags[x, y] &= 0xDFFFFF;
        }

        public void Remove(int x, int y, int flags)
        {
            Flags[x, y] &= 0xFFFFFF - flags;
        }

        public void RemoveLoc(int x, int y, int sizeX, int sizeY, int rotation, bool blocksProjectiles)
        {
            var flags = 256;
            if (blocksProjectiles)
            {
                flags += 0x20000;
            }

            if (rotation == 1 || rotation == 3)
            {
                int size_x2 = sizeX;
                sizeX = sizeY;
                sizeY = size_x2;
            }

            for (var i = x; i < x + sizeX; i++)
            {
                if (i >= 0 && i < Width)
                {
                    for (var j = y; j < y + sizeY; j++)
                    {
                        if (j >= 0 && j < Height)
                        {
                            Remove(i, j, flags);
                        }
                    }
                }
            }
        }

        public void RemoveWall(int x, int y, int type, int rotation, bool blocksProjectiles)
        {
            if (type == 0)
            {
                if (rotation == 0)
                {
                    Remove(x, y, 128);
                    Remove(x - 1, y, 8);
                }
                if (rotation == 1)
                {
                    Remove(x, y, 2);
                    Remove(x, y + 1, 32);
                }
                if (rotation == 2)
                {
                    Remove(x, y, 8);
                    Remove(x + 1, y, 128);
                }
                if (rotation == 3)
                {
                    Remove(x, y, 32);
                    Remove(x, y - 1, 2);
                }
            }
            if (type == 1 || type == 3)
            {
                if (rotation == 0)
                {
                    Remove(x, y, 1);
                    Remove(x - 1, y + 1, 16);
                }
                if (rotation == 1)
                {
                    Remove(x, y, 4);
                    Remove(x + 1, y + 1, 64);
                }
                if (rotation == 2)
                {
                    Remove(x, y, 16);
                    Remove(x + 1, y - 1, 1);
                }
                if (rotation == 3)
                {
                    Remove(x, y, 64);
                    Remove(x - 1, y - 1, 4);
                }
            }
            if (type == 2)
            {
                if (rotation == 0)
                {
                    Remove(x, y, 130);
                    Remove(x - 1, y, 8);
                    Remove(x, y + 1, 32);
                }
                if (rotation == 1)
                {
                    Remove(x, y, 10);
                    Remove(x, y + 1, 32);
                    Remove(x + 1, y, 128);
                }
                if (rotation == 2)
                {
                    Remove(x, y, 40);
                    Remove(x + 1, y, 128);
                    Remove(x, y - 1, 2);
                }
                if (rotation == 3)
                {
                    Remove(x, y, 160);
                    Remove(x, y - 1, 2);
                    Remove(x - 1, y, 8);
                }
            }
            if (blocksProjectiles)
            {
                if (type == 0)
                {
                    if (rotation == 0)
                    {
                        Remove(x, y, 0x10000);
                        Remove(x - 1, y, 4096);
                    }
                    if (rotation == 1)
                    {
                        Remove(x, y, 1024);
                        Remove(x, y + 1, 16384);
                    }
                    if (rotation == 2)
                    {
                        Remove(x, y, 4096);
                        Remove(x + 1, y, 0x10000);
                    }
                    if (rotation == 3)
                    {
                        Remove(x, y, 16384);
                        Remove(x, y - 1, 1024);
                    }
                }
                if (type == 1 || type == 3)
                {
                    if (rotation == 0)
                    {
                        Remove(x, y, 512);
                        Remove(x - 1, y + 1, 8192);
                    }
                    if (rotation == 1)
                    {
                        Remove(x, y, 2048);
                        Remove(x + 1, y + 1, 32768);
                    }
                    if (rotation == 2)
                    {
                        Remove(x, y, 8192);
                        Remove(x + 1, y - 1, 512);
                    }
                    if (rotation == 3)
                    {
                        Remove(x, y, 32768);
                        Remove(x - 1, y - 1, 2048);
                    }
                }
                if (type == 2)
                {
                    if (rotation == 0)
                    {
                        Remove(x, y, 0x10400);
                        Remove(x - 1, y, 4096);
                        Remove(x, y + 1, 16384);
                    }
                    if (rotation == 1)
                    {
                        Remove(x, y, 5120);
                        Remove(x, y + 1, 16384);
                        Remove(x + 1, y, 0x10000);
                    }
                    if (rotation == 2)
                    {
                        Remove(x, y, 20480);
                        Remove(x + 1, y, 0x10000);
                        Remove(x, y - 1, 1024);
                    }
                    if (rotation == 3)
                    {
                        Remove(x, y, 0x14000);
                        Remove(x, y - 1, 1024);
                        Remove(x - 1, y, 4096);
                    }
                }
            }
        }

        public void SetSolid(int x, int y)
        {
            Flags[x, y] |= 0x200000;
        }
    }
}
