using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RS
{
    /// <summary>
    /// Provides utilities for encrypting and decrypting with RC4.
    /// </summary>
    public static class RC4
    {
        /// <summary>
        /// Encrypts a string.
        /// </summary>
        /// <param name="key">The key to use for encryption.</param>
        /// <param name="data">The data to encrypt.</param>
        /// <returns>The encrypted data in a base64 string.</returns>
        public static string Encrypt(string key, string data)
        {
            var unicode = Encoding.Unicode;
            return Convert.ToBase64String(Encrypt(unicode.GetBytes(key), unicode.GetBytes(data)));
        }

        /// <summary>
        /// Decrypts a string.
        /// </summary>
        /// <param name="key">The key to use for decryption.</param>
        /// <param name="data">The data to decrypt.</param>
        /// <returns>The decrypted data.</returns>
        public static string Decrypt(string key, string data)
        {
            var unicode = Encoding.Unicode;
            return unicode.GetString(Encrypt(unicode.GetBytes(key), Convert.FromBase64String(data)));
        }

        /// <summary>
        /// Encrypts some binary data.
        /// </summary>
        /// <param name="key">The key to use for encryption.</param>
        /// <param name="data">The data to encrypt.</param>
        /// <returns>The encrypted data.</returns>
        public static byte[] Encrypt(byte[] key, byte[] data)
        {
            return EncryptOutput(key, data).ToArray();
        }

        /// <summary>
        /// Decrypts some binary data.
        /// </summary>
        /// <param name="key">The key to use for decryption.</param>
        /// <param name="data">The data to decrypt.</param>
        /// <returns>The decrypted data.</returns>
        public static byte[] Decrypt(byte[] key, byte[] data)
        {
            return EncryptOutput(key, data).ToArray();
        }

        /// <summary>
        /// Initializes an RC4 encryption pass.
        /// </summary>
        /// <param name="key">They key to initialize with.</param>
        /// <returns>The initialized data.</returns>
        private static byte[] EncryptInitalize(byte[] key)
        {
            var s = Enumerable.Range(0, 256).Select(i => (byte)i).ToArray();
            for (int i = 0, j = 0; i < 256; i++)
            {
                j = (j + key[i % key.Length] + s[i]) & 255;
                Swap(s, i, j);
            }

            return s;
        }

        private static IEnumerable<byte> EncryptOutput(byte[] key, IEnumerable<byte> data)
        {
            var s = EncryptInitalize(key);
            var i = 0;
            var j = 0;
            return data.Select((b) =>
            {
                i = (i + 1) & 255;
                j = (j + s[i]) & 255;

                Swap(s, i, j);
                return (byte)(b ^ s[(s[i] + s[j]) & 255]);
            });
        }

        /// <summary>
        /// Swaps 2 bytes.
        /// </summary>
        /// <param name="arr">The array to swap in.</param>
        /// <param name="a">The first index to swap.</param>
        /// <param name="b">The second index to swap.</param>
        private static void Swap(byte[] arr, int a, int b)
        {
            byte c = arr[a];

            arr[a] = arr[b];
            arr[b] = c;
        }
    }
}
