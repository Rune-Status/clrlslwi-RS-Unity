using System;

namespace RS
{
    /// <summary>
    /// A default implementation of a jagex buffer.
    /// 
    /// Uses a byte array as a backing data source.
    /// </summary>
    public class DefaultJagexBuffer : JagexBuffer
    {
        /// <summary>
        /// The maximum value that can be stored in each bit column.
        /// </summary>
        public static int[] BIT_MASK;

        static DefaultJagexBuffer()
        {
            BIT_MASK = new int[32];
            for (int i = 0; i < BIT_MASK.Length; i++)
            {
                BIT_MASK[i] = (1 << i) - 1;
            }
        }

        private byte[] buffer;
        private int normalPosition;
        private int bitPosition;

        public DefaultJagexBuffer(byte[] buffer)
        {
            this.buffer = buffer;
        }

        public DefaultJagexBuffer(int size)
            : this(new byte[size])
        {

        }

        public DefaultJagexBuffer()
            : this(new byte[0])
        {

        }

        public override void BeginBitAccess()
        {
            this.bitPosition = this.normalPosition * 8;
        }

        public override int ReadBits(int amount)
        {
            var pos = this.bitPosition >> 3;
            var l = 8 - (this.bitPosition & 7);
            var value = 0;
            this.bitPosition += amount;
            for (; amount > l; l = 8)
            {
                value += (this.buffer[pos++] & BIT_MASK[l]) << amount - l;
                amount -= l;
            }

            if (amount == l)
            {
                value += this.buffer[pos] & BIT_MASK[l];
            }
            else {
                value += this.buffer[pos] >> l - amount & BIT_MASK[amount];
            }
            return value;
        }

        public override void EndBitAccess()
        {
            this.normalPosition = (this.bitPosition + 7) / 8;
        }

        public override int BitPosition()
        {
            return this.bitPosition;
        }

        private void EnsureCapacity()
        {
            if (normalPosition >= buffer.Length)
            {
                var newLen = buffer.Length == 0 ? 1 : buffer.Length * 2;
                var tmp = new byte[newLen];
                Buffer.BlockCopy(buffer, 0, tmp, 0, buffer.Length);
                buffer = tmp;
            }
        }

        override public void WriteByte(int i)
        {
            EnsureCapacity();
            buffer[normalPosition++] = (byte)i;
        }

        override public int ReadByte()
        {
            return ((sbyte)buffer[normalPosition++]);
        }

        override public void Position(int pos)
        {
            this.normalPosition = pos;
        }

        override public int Position()
        {
            return this.normalPosition;
        }

        public void Cap()
        {
            var capped = new byte[normalPosition];
            Buffer.BlockCopy(buffer, 0, capped, 0, capped.Length);
            buffer = capped;
        }

        override public byte[] Array()
        {
            return buffer;
        }
    }
}
