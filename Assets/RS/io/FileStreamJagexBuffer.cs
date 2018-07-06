using System.IO;

namespace RS
{
    /// <summary>
    /// A jagex buffer implementation backed by a file stream.
    /// </summary>
    public class FileStreamJagexBuffer : JagexBuffer
    {
        /// <summary>
        /// The stream to read/write from.
        /// </summary>
        private FileStream raf;

        public FileStreamJagexBuffer(FileStream raf)
        {
            this.raf = raf;
        }

        /// <summary>
        /// Creates a stream from the file at the provided path.
        /// </summary>
        /// <param name="path">The path to the file to create a stream from.</param>
        public FileStreamJagexBuffer(string path)
            : this(new FileStream(path, FileMode.OpenOrCreate))
        {

        }

        override public void WriteByte(int i)
        {
            raf.WriteByte((byte)i);
        }

        override public int ReadByte()
        {
            return raf.ReadByte();
        }

        override public void Position(int pos)
        {
            raf.Seek(pos, SeekOrigin.Begin);
        }

        override public int Position()
        {
            return (int)raf.Position;
        }

        override public byte[] Array()
        {
            var b = new byte[(int)raf.Length];
            raf.Seek(0, SeekOrigin.Begin);
            raf.Read(b, 0, b.Length);
            return b;
        }

        override public int Capacity()
        {
            return (int)raf.Length;
        }

    }
}
