namespace RS
{
    /// <summary>
    /// Provides utilities for XOR encrypting.
    /// </summary>
    public static class XOR
    {
        /// <summary>
        /// XORs a key with every byte in an array.
        /// </summary>
        /// <param name="arr">The array to apply the xor to.</param>
        /// <param name="key">The key to xor all of the bytes with.</param>
        public static void Perform(byte[] arr, int key)
        {
            sbyte bkey = (sbyte)key;
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] ^= (byte)bkey;
            }
        }
    }
}
