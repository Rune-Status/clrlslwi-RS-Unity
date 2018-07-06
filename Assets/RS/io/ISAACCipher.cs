namespace RS
{
    /// <summary>
    /// An ISAAC cipher implementation.
    /// </summary>
    public class ISAACCipher
    {
        private int count;
        private int[] results;
        private int[] memory;
        private int a;
        private int b;
        private int c;

        public ISAACCipher(int[] seed)
        {
            memory = new int[256];
            results = new int[256];
            for (int j = 0; j < seed.Length; j++)
            {
                results[j] = seed[j];
            }

            Init();
        }

        /// <summary>
        /// Generates the next int in the cipher.
        /// </summary>
        /// <returns>The next int in the cipher.</returns>
        public int NextInt()
        {
            if (count-- == 0)
            {
                GenerateNext();
                count = 255;
            }

            return results[count];
        }

        /// <summary>
        /// Generates a new round of values.
        /// </summary>
        public void GenerateNext()
        {
            b += ++c;
            for (int i = 0; i < 256; i++)
            {
                var x = memory[i];

                if ((i & 3) == 0)
                {
                    a ^= a << 13;
                }
                else if ((i & 3) == 1)
                {
                    a ^= (int)((uint)a >> 6);
                }
                else if ((i & 3) == 2)
                {
                    a ^= a << 2;
                }
                else if ((i & 3) == 3)
                {
                    a ^= (int)((uint)a >> 16);
                }

                a += memory[i + 128 & 0xff];
                int y;
                memory[i] = y = memory[(x & 0x3fc) >> 2] + a + b;
                results[i] = b = memory[(y >> 8 & 0x3fc) >> 2] + x;
            }

        }

        /// <summary>
        /// Initializes the ISAAC state.
        /// </summary>
        public void Init()
        {
            int b;
            int c;
            int d;
            int e;
            int f;
            int g;
            int h;
            int a = b = c = d = e = f = g = h = unchecked((int)0x9E3779B9);
            for (int i = 0; i < 4; i++)
            {
                a ^= b << 11;
                d += a;
                b += c;
                b ^= (int)((uint)c >> 2);
                e += b;
                c += d;
                c ^= d << 8;
                f += c;
                d += e;
                d ^= (int)((uint)e >> 16);
                g += d;
                e += f;
                e ^= f << 10;
                h += e;
                f += g;
                f ^= (int)((uint)g >> 4);
                a += f;
                g += h;
                g ^= h << 8;
                b += g;
                h += a;
                h ^= (int)((uint)a >> 9);
                c += h;
                a += b;
            }

            for (int j = 0; j < 256; j += 8)
            {
                a += results[j];
                b += results[j + 1];
                c += results[j + 2];
                d += results[j + 3];
                e += results[j + 4];
                f += results[j + 5];
                g += results[j + 6];
                h += results[j + 7];
                a ^= b << 11;
                d += a;
                b += c;
                b ^= (int)((uint)c >> 2);
                e += b;
                c += d;
                c ^= d << 8;
                f += c;
                d += e;
                d ^= (int)((uint)e >> 16);
                g += d;
                e += f;
                e ^= f << 10;
                h += e;
                f += g;
                f ^= (int)((uint)g >> 4);
                a += f;
                g += h;
                g ^= h << 8;
                b += g;
                h += a;
                h ^= (int)((uint)a >> 9);
                c += h;
                a += b;
                memory[j] = a;
                memory[j + 1] = b;
                memory[j + 2] = c;
                memory[j + 3] = d;
                memory[j + 4] = e;
                memory[j + 5] = f;
                memory[j + 6] = g;
                memory[j + 7] = h;
            }

            for (int k = 0; k < 256; k += 8)
            {
                a += memory[k];
                b += memory[k + 1];
                c += memory[k + 2];
                d += memory[k + 3];
                e += memory[k + 4];
                f += memory[k + 5];
                g += memory[k + 6];
                h += memory[k + 7];
                a ^= b << 11;
                d += a;
                b += c;
                b ^= (int)((uint)c >> 2);
                e += b;
                c += d;
                c ^= d << 8;
                f += c;
                d += e;
                d ^= (int)((uint)e >> 16);
                g += d;
                e += f;
                e ^= f << 10;
                h += e;
                f += g;
                f ^= (int)((uint)g >> 4);
                a += f;
                g += h;
                g ^= h << 8;
                b += g;
                h += a;
                h ^= (int)((uint)a >> 9);
                c += h;
                a += b;
                memory[k] = a;
                memory[k + 1] = b;
                memory[k + 2] = c;
                memory[k + 3] = d;
                memory[k + 4] = e;
                memory[k + 5] = f;
                memory[k + 6] = g;
                memory[k + 7] = h;
            }

            GenerateNext();
            count = 256;
        }
    }
}
