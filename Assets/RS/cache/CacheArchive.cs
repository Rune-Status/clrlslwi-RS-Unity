using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.GZip;

namespace RS
{
    public class CacheArchive
    {
        public static int DescriptorSize = 0xA;

        private JagexBuffer buffer;
        private bool extractedAsWhole = false;

        private int[] fileHashes;
        private int[] unpackedSizes;
        private int[] packedSizes;
        private int[] positions;

        private byte[] ReconstructHeader(JagexBuffer buffer)
        {
            var existing = buffer.Array();
            if (existing[0] == 'B' && existing[1] == 'Z' && existing[2] == 'h' && existing[3] == '1')
            {
                return existing;
            }

            var compressed = new byte[buffer.Capacity() + 4];
            buffer.ReadBytes(compressed, 4, buffer.Capacity());
            compressed[0] = (byte)'B';
            compressed[1] = (byte)'Z';
            compressed[2] = (byte)'h';
            compressed[3] = (byte)'1';
            return compressed;
        }

        private byte[] ReconstructHeader(byte[] buffer)
        {
            return ReconstructHeader(new DefaultJagexBuffer(buffer));
        }

        public CacheArchive(JagexBuffer buffer)
        {
            var decompressedSize = buffer.ReadTriByte();
            var compressedSize = buffer.ReadTriByte();
            
            if (decompressedSize != compressedSize)
            {
                byte[] tmp = new byte[buffer.Capacity() - 6];
                buffer.ReadBytes(tmp, 0, buffer.Capacity() - 6);

                byte[] compressed = ReconstructHeader(new DefaultJagexBuffer(tmp));

                MemoryStream outs = new MemoryStream();
                BZip2.Decompress(new MemoryStream(compressed), outs, true);
                buffer = new DefaultJagexBuffer(outs.ToArray());
                extractedAsWhole = true;
            }

            var size = buffer.ReadUShort();
            InitializeFiles(size);
            var position = buffer.Position() + (size * DescriptorSize);
            for (var i = 0; i < size; i++)
            {
                fileHashes[i] = buffer.ReadInt();
                unpackedSizes[i] = buffer.ReadTriByte();
                packedSizes[i] = buffer.ReadTriByte();
                positions[i] = position;
                position += packedSizes[i];
            }

            this.buffer = buffer;
        }

        public CacheArchive(byte[] b)
            : this(new DefaultJagexBuffer(b))
        {

        }

        private void InitializeFiles(int size)
        {
            this.fileHashes = new int[size];
            this.unpackedSizes = new int[size];
            this.packedSizes = new int[size];
            this.positions = new int[size];
        }

        public byte[] GetFile(String name)
        {
            return GetFile(StringUtils.HashString(name));
        }

        public byte[] GetFile(int hash)
        {
            for (var i = 0; i < fileHashes.Length; i++)
            {
                if (fileHashes[i] == hash)
                {
                    if (!extractedAsWhole)
                    {
                        var compressed = new byte[packedSizes[i]];
                        Buffer.BlockCopy(buffer.Array(), positions[i], compressed, 0, compressed.Length);
                        compressed = ReconstructHeader(compressed);

                        var outs = new MemoryStream();
                        BZip2.Decompress(new MemoryStream(compressed), outs, true);
                        return outs.ToArray();
                    }

                    var decompressed = new byte[unpackedSizes[i]];
                    Buffer.BlockCopy(buffer.Array(), positions[i], decompressed, 0, decompressed.Length);
                    return decompressed;
                }
            }

            throw new FileNotFoundException();
        }
    }
}
