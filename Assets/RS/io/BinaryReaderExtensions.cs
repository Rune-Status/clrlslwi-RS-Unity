using System.IO;
using System.Text;

namespace RS
{
    /// <summary>
    /// Extensions for binary readers.
    /// </summary>
    public static class BinaryReaderExtensions
    {
        /// <summary>
        /// Reads a string from the provided reader.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        /// <param name="del">The string delimiter to stop reading at.</param>
        /// <returns>The read string.</returns>
        public static string ReadString(this BinaryReader reader, int del)
        {
            var sb = new StringBuilder();
            var read = 0;
            while ((read = reader.Read()) != del)
            {
                sb.Append((char)read);
            }
            return sb.ToString();
        }
    }
}
