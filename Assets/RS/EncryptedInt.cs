using System;

namespace RS
{
    /// <summary>
    /// An integer that is encrypted in memory.
    /// </summary>
    public class EncryptedInt
    {
        public static readonly EncryptedInt Zero = new EncryptedInt(0, true);

        private int value;
        private bool readOnly;

        /// <summary>
        /// The real value stored within the encrypted block.
        /// </summary>
        public int Value
        {
            get
            {
                return GameConstants.MaskInt(value);
            }
            set
            {
                if (readOnly)
                {
                    throw new InvalidOperationException("Int is read only");
                }
                this.value = GameConstants.MaskInt(value);
            }
        }

        public EncryptedInt(int val, bool readOnly = false)
        {
            this.Value = val;
            this.readOnly = readOnly;
        }

        public static implicit operator EncryptedInt(int value)
        {
            return new EncryptedInt(value, false);
        }

        public static implicit operator int(EncryptedInt x)
        {
            return x.Value;
        }

        public static bool operator >(EncryptedInt x, int v)
        {
            return x.Value > v;
        }

        public static bool operator <(EncryptedInt x, int v)
        {
            return x.Value < v;
        }

        public static bool operator >=(EncryptedInt x, int v)
        {
            return x.Value >= v;
        }

        public static bool operator <=(EncryptedInt x, int v)
        {
            return x.Value <= v;
        }

        public static bool operator >(EncryptedInt x, EncryptedInt v)
        {
            return x.Value > v.Value;
        }

        public static bool operator <(EncryptedInt x, EncryptedInt v)
        {
            return x.Value < v.Value;
        }

        public static bool operator >=(EncryptedInt x, EncryptedInt v)
        {
            return x.Value >= v.Value;
        }

        public static bool operator <=(EncryptedInt x, EncryptedInt v)
        {
            return x.Value <= v.Value;
        }

        public static bool operator ==(EncryptedInt x, EncryptedInt y)
        {
            return x.Value == y.Value;
        }

        public static bool operator !=(EncryptedInt x, EncryptedInt y)
        {
            return x.Value != y.Value;
        }

        public static EncryptedInt operator +(EncryptedInt c1, EncryptedInt c2)
        {
            return new EncryptedInt(c1.Value + c2.Value, false);
        }

        public static EncryptedInt operator -(EncryptedInt c1, EncryptedInt c2)
        {
            return new EncryptedInt(c1.Value - c2.Value, false);
        }

        public static EncryptedInt operator *(EncryptedInt c1, EncryptedInt c2)
        {
            return new EncryptedInt(c1.Value * c2.Value, false);
        }

        public static EncryptedInt operator /(EncryptedInt c1, EncryptedInt c2)
        {
            return new EncryptedInt(c1.Value / c2.Value, false);
        }

        public static EncryptedInt operator |(EncryptedInt x, EncryptedInt y)
        {
            return new EncryptedInt(x.Value | y.Value);
        }

        public static EncryptedInt operator ^(EncryptedInt x, EncryptedInt y)
        {
            return new EncryptedInt(x.Value ^ y.Value);
        }

        public static EncryptedInt operator &(EncryptedInt x, EncryptedInt y)
        {
            return new EncryptedInt(x.Value & y.Value);
        }
    }
}
