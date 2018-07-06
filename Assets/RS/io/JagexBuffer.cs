namespace RS
{
    /// <summary>
    /// Provides various ways of reading & writing data, some of which are Jagex/RS specific.
    /// </summary>
    public abstract class JagexBuffer
    {
        /// <summary>
        /// Begins bit access.
        /// </summary>
		public virtual void BeginBitAccess()
		{
			
		}
		
        /// <summary>
        /// Reads some bits from the buffer.
        /// </summary>
        /// <param name="amount">The amount of bits to read. Maximum 32 (obviously.)</param>
        /// <returns>The read number.</returns>
		public virtual int ReadBits(int amount)
		{
			return 0;
		}

        /// <summary>
        /// Ends bit access.
        /// 
        /// This typically results in the byte pointer being moved forward if not 0.
        /// </summary>
		public virtual void EndBitAccess()
		{
			
		}

        /// <summary>
        /// Retrieves the read/write position of bits in the current byte.
        /// </summary>
        /// <returns>The read/write position of bits in the current byte.</returns>
		public virtual int BitPosition()
		{
			return 0;
		}

        public abstract int ReadByte();

        /// <summary>
        /// Reads a boolean from the underlying data.
        /// </summary>
        /// <returns>The read boolean.</returns>
        public bool ReadBoolean()
        {
            return ReadByte() != 0;
        }

        /// <summary>
        /// Reads an unsigned byte from the underlying data.
        /// </summary>
        /// <returns>The read byte.</returns>
        public int ReadUByte()
        {
            return ReadByte() & 0xFF;
        }

        /// <summary>
        /// Reads a type A unsigned byte from the underlying data.
        /// </summary>
        /// <returns>The read byte.</returns>
		public int ReadUByteA() {
			return ReadByte() - 128 & 0xFF;
		}

        /// <summary>
        /// Reads a type S unsigned byte from the underlying data.
        /// </summary>
        /// <returns>The read byte.</returns>
		public int ReadUByteS() {
			return 128 - ReadByte() & 0xFF;
		}

        /// <summary>
        /// Reads a type C unsigned byte from the underlying data.
        /// </summary>
        /// <returns></returns>
		public int ReadUByteC() {
			return -ReadByte() & 0xFF;
		}

        /// <summary>
        /// Reads some bytes from the underlying data.
        /// </summary>
        /// <param name="dst">The array to store the read bytes in.</param>
        /// <param name="dstOff">The offset to store the bytes at.</param>
        /// <param name="len">The number of bytes to read.</param>
        public void ReadBytes(byte[] dst, int dstOff, int len)
        {
            for (int i = 0; i < len; i++)
            {
                dst[i + dstOff] = (byte)ReadByte();
            }
        }

        /// <summary>
        /// Reads some bytes in reverse order from the underlying data.
        /// </summary>
        /// <param name="dst">The array to store the read bytes in.</param>
        /// <param name="dstOff">The offset to store the bytes at.</param>
        /// <param name="len">The number of bytes to read.</param>
        public void ReadBytesReversed(byte[] dst, int off, int len)
        {
            for (int k = (off + len) - 1; k >= off; k--)
            {
                dst[k] = (byte)ReadByte();
            }
        }

        /// <summary>
        /// Reads a short from the underlying data.
        /// </summary>
        /// <returns>The read short.</returns>
        public int ReadShort()
        {
            int i = ReadUShort();
            if (i > 32767)
            {
                i -= 0x10000;
            }
            return i;
        }

        /// <summary>
        /// Reads a type 2 short from the underlying data.
        /// </summary>
        /// <returns>The read short.</returns>
        public int ReadShort2()
        {
            int val = 0;
            val += ReadByte() << 8;
            val += ReadUByte();
            if (val > 60000)
                val = -65535 + val;
            return val;
        }

        /// <summary>
        /// Reads an unsigned short from the underlying data.
        /// </summary>
        /// <returns>The read short.</returns>
        public int ReadUShort()
        {
            var val = 0;
            val += ReadUByte() << 8;
            val += ReadUByte();
            return val & 0xFFFF;
        }

        /// <summary>
        /// Reads a type A short from the underlying data.
        /// </summary>
        /// <returns>The read short.</returns>
        public int ReadShortA()
        {
            var val = ReadUShortA();
            if (val > 0x7FFF)
            {
                val -= 0x10000;
            }
            return val;
        }

        /// <summary>
        /// Reads a type A unsigned short from the underlying data.
        /// </summary>
        /// <returns>The read short.</returns>
		public int ReadUShortA()
		{
            var val = 0;
			val += ReadUByte() << 8;
			val += ReadByte() - 128 & 0xFF;
			return val;
		}

        /// <summary>
        /// Reads a little endian short from the underlying data.
        /// </summary>
        /// <returns>The read short.</returns>
        public int ReadLEShort()
        {
            var val = ReadLEUShort();
            if (val > 0x7FFF)
            {
                val -= 0x10000;
            }
            return val;
        }

        /// <summary>
        /// Reads a little endian unsigned short from the underlying data.
        /// </summary>
        /// <returns>The read short.</returns>
		public int ReadLEUShort()
        {
            var val = 0;
			val += ReadUByte();
			val += ReadUByte() << 8;
			return val;
		}

        /// <summary>
        /// Reads a type A little endian short from the underlying data.
        /// </summary>
        /// <returns>The read short.</returns>
        public int ReadLEShortA()
        {
            var val = ReadLEUShortA();
            if (val > 0x7FFF)
            {
                val -= 0x10000;
            }
            return val;
        }

        /// <summary>
        /// Reads a type A little endian unsigned short from the underlying data.
        /// </summary>
        /// <returns>The read short.</returns>
        public int ReadLEUShortA()
        {
            var val = 0;
			val += ReadByte() - 128 & 0xFF;
			val += ReadUByte() << 8;
			return val;
		}

        /// <summary>
        /// Reads a 3 byte integer from the underlying data.
        /// </summary>
        /// <returns>The read 3 byte integer.</returns>
        public int ReadTriByte()
        {
            int val = 0;
            val += ReadUByte() << 16;
            val += ReadUByte() << 8;
            val += ReadUByte();
            return val;
        }

        /// <summary>
        /// Reads a smart from the underlying data.
        /// </summary>
        /// <returns>The read smart.</returns>
        public int ReadSmart()
        {
            int val = ReadUByte();
            Position(Position() - 1);

            if (val < 128)
            {
                return ReadUByte() - 64;
            }
            else
            {
                return ReadUShort() - 49152;
            }
        }

        /// <summary>
        /// Reads an unsigned smart from the underlying data.
        /// </summary>
        /// <returns>The read smart.</returns>
        public int ReadUSmart()
        {
            int val = ReadUByte();
            Position(Position() - 1);

            if (val < 0x80)
            {
                return ReadUByte();
            }
            else
            {
                return ReadUShort() - 32768;
            }
        }

        /// <summary>
        /// Reads a type 2 unsigned smart from the underlying data.
        /// </summary>
        /// <returns>The read smart.</returns>
        public int ReadUSmart2()
        {
            int baseVal = 0;
            int lastVal = ReadUSmart();
            while (lastVal == 32767)
            {
                baseVal += 32767;
            }
            return baseVal + lastVal;
        }

        /// <summary>
        /// Reads an integer from the underlying data.
        /// </summary>
        /// <returns>The read integer.</returns>
        public int ReadInt()
        {
            int val = 0;
            val += ReadUShort() << 16;
            val += ReadUShort();
            return val;
        }

        /// <summary>
        /// Reads an IME integer from the underlying data.
        /// </summary>
        /// <returns>The read integer.</returns>
        public int ReadImeInt()
        {
            int val = 0;
            val += ReadUByte() << 16;
            val += ReadUByte() << 24;
            val += ReadUByte();
            val += ReadUByte() << 8;
            return val;
        }

        /// <summary>
        /// Reads a ME integer from the underlying data.
        /// </summary>
        /// <returns>The read integer.</returns>
        public int ReadMeInt()
        {
            int val = 0;
            val += ReadUByte() << 8;
            val += ReadUByte();
            val += ReadUByte() << 24;
            val += ReadUByte() << 16;
            return val;
        }

        /// <summary>
        /// Reads a long from the underlying data.
        /// </summary>
        /// <returns>The read long.</returns>
        public long ReadLong()
        {
            var l = ReadInt() & 0xFFFFFFFFL;
            var l1 = ReadInt() & 0xFFFFFFFFL;
            return (l << 32) + l1;
        }

        /// <summary>
        /// Reads a string from the underlying data.
        /// </summary>
        /// <param name="terminator">The terminator to stop reading at.</param>
        /// <returns>The read string.</returns>
        public string ReadString(int terminator)
        {
            string bldr = "";
            int read;
            while ((read = ReadUByte()) != terminator)
            {
                bldr += (char)read;
            }
            return bldr;
        }

        /// <summary>
        /// Writes a byte to the underlying data.
        /// </summary>
        /// <param name="i">The byte value to write.</param>
        public abstract void WriteByte(int i);

        /// <summary>
        /// Writes a type C byte to the underlying data.
        /// </summary>
        /// <param name="i">The byte value to write.</param>
        public void WriteByteC(int i)
        {
            WriteByte(-i);
        }

        /// <summary>
        /// Writes a type S byte to the underlying data.
        /// </summary>
        /// <param name="i">The byte value to write.</param>
        public void WriteByteS(int i)
        {
            WriteByte(128 - i);
        }

        /// <summary>
        /// Writes some bytes to the underlying data.
        /// </summary>
        /// <param name="src">The place to read the bytes from.</param>
        /// <param name="off">The offset to read from.</param>
        /// <param name="len">The amount of bytes to read.</param>
        public void WriteBytes(byte[] src, int off, int len)
        {
            for (int i = off; i < (off + len); i++)
            {
                WriteByte(src[i]);
            }

        }
        /// <summary>
        /// Writes some bytes to the underlying data in reverse order.
        /// </summary>
        /// <param name="src">The place to read the bytes from.</param>
        /// <param name="off">The offset to read from.</param>
        /// <param name="len">The amount of bytes to read.</param>
        public void WriteBytesReversedA(byte[] src, int off, int len)
        {
            for (int i = (off + len) - 1; i >= off; i--)
            {
                WriteByte(src[i] + 128);
            }
        }

        /// <summary>
        /// Writes the length to the underlying data.
        /// </summary>
        /// <param name="len">The length to write</param>
        public void WriteLength(int len)
        {
            var tmp = Position();
            Position(tmp - len - 1);
            WriteByte(len);
            Position(tmp);
        }

        /// <summary>
        /// Writes a short to the underlying data.
        /// </summary>
        /// <param name="i">The value to write.</param>
        public void WriteShort(int i)
        {
            WriteByte(i >> 8);
            WriteByte(i);
        }

        /// <summary>
        /// Writes a type A short to the underlying data.
        /// </summary>
        /// <param name="i">The value to write.</param>
        public void WriteShortA(int i)
        {
            WriteByte(i >> 8);
            WriteByte(i + 128);
        }

        /// <summary>
        /// Writes a little endian short to the underlying data.
        /// </summary>
        /// <param name="i">The value to write.</param>
        public void WriteLEShort(int i)
        {
            WriteByte(i);
            WriteByte(i >> 8);
        }

        /// <summary>
        /// Writes a type A little endian short to the underlying data.
        /// </summary>
        /// <param name="i">The value to write.</param>
        public void WriteLEShortA(int i)
        {
            WriteByte(i + 128);
            WriteByte(i >> 8);
        }

        /// <summary>
        /// Writes an integer to the underlying data.
        /// </summary>
        /// <param name="i">The value to write.</param>
        public void WriteInt(int i)
        {
            WriteShort(i >> 16);
            WriteShort(i);
        }

        /// <summary>
        /// Writes a long to the underlying data.
        /// </summary>
        /// <param name="i">The value to write.</param>
        public void WriteLong(long l)
        {
            WriteInt((int)(l >> 32));
            WriteInt((int)l);
        }

        /// <summary>
        /// Writes a string to the underlying data.
        /// </summary>
        /// <param name="s">The value to write.</param>
        /// <param name="terminator">The terminate byte to write at the end.</param>
        public void WriteString(string s, int terminator)
        {
            foreach (char c in s.ToCharArray())
            {
                WriteByte((int)c);
            }
            WriteByte(terminator);
        }

        /// <summary>
        /// Changes the rw position of this buffer.
        /// </summary>
        /// <param name="pos">The new position.</param>
        public abstract void Position(int pos);

        /// <summary>
        /// Retrieves the position of this buffer.
        /// </summary>
        /// <returns>The position of this buffer.</returns>
        public abstract int Position();

        /// <summary>
        /// Retrieves the underlying data from this buffer.
        /// </summary>
        /// <returns>The underlying data from this buffer.</returns>
        public abstract byte[] Array();

        /// <summary>
        /// Calculates the capcity of this buffer.
        /// </summary>
        /// <returns>The capcity of this buffer.</returns>
        public virtual int Capacity()
        {
            return Array().Length;
        }

        /// <summary>
        /// RSA encrypts this buffers contents up to the rw position.
        /// </summary>
        /// <param name="exp">The RSA exponent to encrypt with.</param>
        /// <param name="modulus">The RSA modulus to encrypt with.</param>
        public void RSA(string exp, string modulus)
        {
            int tmp = Position();
            Position(0);

            byte[] unencrypted = new byte[tmp];
            ReadBytes(unencrypted, 0, tmp);
			BigInteger val = new BigInteger(unencrypted);

			BigInteger encrypted = val.ModPow(BigInteger.Parse(modulus), BigInteger.Parse(exp));
			byte[] ba = encrypted.GetBytes();

			Position(0);
			WriteByte(ba.Length);
			WriteBytes(ba, 0, ba.Length);
        }
    }
}
