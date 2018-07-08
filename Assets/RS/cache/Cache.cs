using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using LitJson;

using UnityEngine;

namespace RS
{
    public class Cache
    {
        public FileStreamJagexBuffer DataStream;
        public Dictionary<int, FileStreamJagexBuffer> IdxStreams = new Dictionary<int, FileStreamJagexBuffer>();
        
        public ActorConfig[] ActorDescriptorCache = new ActorConfig[0];
        public Dictionary<int, ItemConfig> ItemConfigCache = new Dictionary<int, ItemConfig>();
        public Dictionary<int, ObjectConfig> ObjectConfigCache = new Dictionary<int, ObjectConfig>();
        public Dictionary<int, WidgetConfig> WidgetConfigCache = new Dictionary<int, WidgetConfig>();
        public Dictionary<int, Bitmap> Textures = new Dictionary<int, Bitmap>();

        public BitmapFont SmallFont;
        public BitmapFont NormalFont;
        public BitmapFont BoldFont;
        public BitmapFont FancyFont;

        public ItemConfigProvider ItemConfigProvider;
        public ObjectConfigProvider ObjectConfigProvider;
        public UnderlayFloorDescriptorProvider UnderlayDescProvider;
        public IdentityKitProvider IdkProvider;
        public SequenceProvider SeqProvider;
        public SequenceFrameProvider SeqFrameProvider;
        public GraphicDescriptorProvider GraphicDescProvider;
        public WidgetConfigProvider WidgetConfigProvider;

        /// <summary>
        /// A list of names of crc files.
        /// </summary>
        public static readonly string[] CrcFiles = { "model_crc", "anim_crc", "midi_crc", "map_crc" };

        /// <summary>
        /// A list of names of version files.
        /// </summary>
        public static readonly string[] VersionFiles = { "model_version", "anim_version", "midi_version", "map_version" };

        /// <summary>
        /// The size of a file index.
        /// </summary>
        public const int IndexSize = 6;

        /// <summary>
        /// The size of a block header.
        /// </summary>
        public const int HeaderSize = 8;

        /// <summary>
        /// The size of the data within a block.
        /// </summary>
        public const int DataSize = 512;

        /// <summary>
        /// The total size of a data block.
        /// </summary>
        public const int BlockSize = HeaderSize + DataSize;

        /// <summary>
        /// The number of data blocks within the cache.
        /// </summary>
        public int DataBlockCount
        {
            get
            {
                return DataStream.Capacity() / DataSize;
            }
        }

        /// <summary>
        /// The load priority of each file.
        /// </summary>
        public byte[][] FilePriorities = new byte[4][];

        /// <summary>
        /// The version of each file.
        /// </summary>
        public int[][] FileVersions = new int[4][];

        /// <summary>
        /// The CRC of each file.
        /// </summary>
        public int[][] FileCrcs = new int[4][];

        /// <summary>
        /// The ids of each map region.
        /// </summary>
        public int[] MapIndices;

        /// <summary>
        /// The ids of the map files to load for each region.
        /// </summary>
        public int[] ObjectFileIds;
        
        /// <summary>
        /// The ids of the object files to load for each region.
        /// </summary>
        public int[] LandscapeFileIds;

        /// <summary>
        /// The members status of each region.
        /// </summary>
        public int[] MapMembers;

        /// <summary>
        /// The index of each model.
        /// </summary>
        public byte[] ModelIndex;

        /// <summary>
        /// The index of each animation.
        /// </summary>
        public int[] AnimIndex;

        /// <summary>
        /// The index of each midi.
        /// </summary>
        public int[] MidiIndex;

        public int FileCount(int idx)
        {
            return IdxStreams[idx].Capacity() / IndexSize;
        }

        public int GetMapId(int x, int y, int type)
        {
            var uid = (x << 8) + y;
            for (var i = 0; i < MapIndices.Length; i++)
            {
                if (MapIndices[i] == uid)
                {
                    if (type == 0)
                    {
                        return ObjectFileIds[i];
                    }
                    else
                    {
                        return LandscapeFileIds[i];
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Reads a file from the cache.
        /// </summary>
        /// <param name="idx">The index to read from.</param>
        /// <param name="file">The file to read from.</param>
        /// <returns></returns>
        public byte[] Read(int idx, int file)
        {
            lock (this) {
                var idxs = IdxStreams[idx];
                idxs.Position(file * IndexSize);
                var size = idxs.ReadTriByte();
                if (size <= 0)
                    return null;

                var block = idxs.ReadTriByte();
                if (block <= 0 || block > DataBlockCount)
                {
                    Debug.Log(idxs + " " + idxs.Capacity() + " " + idxs.Position() + " " + idx + " " + file + " " + size + " " + block);
                    return null;
                }

                var data = new byte[size];
                var part = 0;
                var read = 0;
                while (read < size)
                {
                    DataStream.Position(block * BlockSize);
                    var toRead = Math.Min(DataSize, size - read);
                    var fileIndex = DataStream.ReadUShort();
                    var filePart = DataStream.ReadUShort();
                    var next = DataStream.ReadTriByte();
                    var index = DataStream.ReadUByte();

                    if (fileIndex != file)
                    {
                        throw new Exception("Block file index does not match file " + fileIndex + "/" + file);
                    }

                    if (filePart != part)
                    {
                        throw new Exception("Block part does not match part " + filePart + "/" + part);
                    }

                    if (index != (idx + 1))
                    {
                        throw new Exception("Block index parent does not match index " + index + "/" + idx);
                    }

                    if (next < 0 || next > DataBlockCount)
                    {
                        throw new Exception("File is missing ending " + next + "/" + DataBlockCount);
                    }

                    DataStream.ReadBytes(data, read, toRead);

                    read += toRead;
                    block = next;
                    part += 1;
                }
                return data;
            }
        }

        public Bitmap GetImageBitmap(CacheArchive archive, string key, int index)
        {
            return new Bitmap(archive, key, index);
        }

        public Texture2D GetImageAsTex(CacheArchive archive, string key, int index)
        {
            var bitmap = GetImageBitmap(archive, key, index);
            return bitmap.ToUnityTexture();
        }

        public Bitmap GetImageBitmap(string key, int index)
        {
            return GetImageBitmap(GetArchive(4), key, index);
        }

        public Texture2D GetImageAsTex(string key, int index)
        {
            return GetImageAsTex(GetArchive(4), key, index);
        }

        public Bitmap GetTextureBitmap(int index)
        {
            if (!Textures.ContainsKey(index))
            {
                return null;
            }
            return Textures[index];
        }

        public byte[] ReadCompressed(int idx, int file)
        {
            var data = Read(idx, file);
            if (data == null) return null;

            var ins = new MemoryStream(data);
            var outs = new MemoryStream();
            var dataBuffer = new byte[4096];
            var gzipStream = new PatchedGZipInputStream(ins);
            while (true)
            {
                var size = gzipStream.Read(dataBuffer, 0, dataBuffer.Length);
                if (size == 0) break;

                outs.Write(dataBuffer, 0, size);
            }

            return outs.ToArray();
        }

        public CacheArchive GetArchive(int index)
        {
            var raw = Read(0, index);
            if (raw == null)
            {
                throw new Exception("Config archive doesn't exist!");
            }
            
            return new CacheArchive(raw);
        }

        private void SetupFileInfo(CacheArchive archive)
        {
            for (var i = 0; i < VersionFiles.Length; i++)
            {
                var b = new DefaultJagexBuffer(archive.GetFile(VersionFiles[i]));
                int count = b.Capacity() / 2;

                FileVersions[i] = new int[count];
                FilePriorities[i] = new byte[count];

                for (int j = 0; j < count; j++)
                {
                    FileVersions[i][j] = b.ReadUShort();
                }
            }
        }

        private void SetupFileCrcs(CacheArchive archive)
        {
            for (var i = 0; i < CrcFiles.Length; i++)
            {
                var b = new DefaultJagexBuffer(archive.GetFile(CrcFiles[i]));

                FileCrcs[i] = new int[b.Capacity() / 4];
                for (int j = 0; j < FileCrcs[i].Length; j++)
                {
                    FileCrcs[i][j] = b.ReadInt();
                }
            }
        }

        private void SetupModelIndices(CacheArchive archive)
        {
            byte[] data = archive.GetFile("model_index");
            int size = FileVersions[0].Length;

            ModelIndex = new byte[size];
            for (int i = 0; i < size; i++)
            {
                if (i < data.Length)
                {
                    ModelIndex[i] = data[i];
                }
                else
                {
                    ModelIndex[i] = 0x0;
                }
            }
        }

        private void SetupMapIndices(CacheArchive archive)
        {
            var mapIdxStream = new DefaultJagexBuffer(archive.GetFile("map_index"));
            var size = mapIdxStream.Capacity() / 7;
            Debug.Log("Map count: " + size);

            MapIndices = new int[size];
            ObjectFileIds = new int[size];
            LandscapeFileIds = new int[size];
            MapMembers = new int[size];

            for (int i = 0; i < size; i++)
            {
                MapIndices[i] = mapIdxStream.ReadUShort();
                ObjectFileIds[i] = mapIdxStream.ReadUShort();
                LandscapeFileIds[i] = mapIdxStream.ReadUShort();
                mapIdxStream.ReadByte();
            }

            Debug.Log(mapIdxStream.Position() + " " + mapIdxStream.Capacity());
        }

        private void SetupAnimIndices(CacheArchive archive)
        {
            var animIdxStream = new DefaultJagexBuffer(archive.GetFile("anim_index"));
            var size = animIdxStream.Capacity() / 2;

            AnimIndex = new int[size];
            for (int i = 0; i < size; i++)
            {
                AnimIndex[i] = animIdxStream.ReadUShort();
            }
        }

        private void SetupMidiIndices(CacheArchive archive)
        {
            var midiIdxStream = new DefaultJagexBuffer(archive.GetFile("midi_index"));
            var size = midiIdxStream.Capacity();

            MidiIndex = new int[size];
            for (int i = 0; i < size; i++)
            {
                MidiIndex[i] = midiIdxStream.ReadUByte();
            }
        }

        public void Setup()
        {
            var archive = GetArchive(5);
            SetupFileInfo(archive);
            SetupFileCrcs(archive);
            SetupModelIndices(archive);
            SetupMapIndices(archive);
            SetupAnimIndices(archive);
            SetupMidiIndices(archive);
        }

        public void ClearTempCaches()
        {
            ItemConfigCache.Clear();
        }

        public ObjectConfig GetObjectConfig(int index)
        {
            return ObjectConfigProvider.Provide(index);
        }

        public ActorConfig GetActorConfig(int index)
        {
            return null;
        }
        
        public TileConfig GetFloorConfig(int index)
        {
            return UnderlayDescProvider.Provide(index);
        }

        public PlayerAppearanceConfig GetPlayerAppearanceConfig(int index)
        {
            return IdkProvider.Provide(index);
        }

        public ItemConfig GetItemConfig(int index)
        {
            return ItemConfigProvider.Provide(index);
        }

        public Animation GetSeq(int index)
        {
            return SeqProvider.Provide(index);
        }

        public SequenceFrame GetSeqFrame(int index)
        {
            return SeqFrameProvider.Provide(index);
        }

        public WidgetConfig GetWidgetConfig(int index)
        {
            return WidgetConfigProvider.Provide(index);
        }

        public GraphicConfig GetGraphicConfig(int index)
        {
            return GraphicDescProvider.Provide(index);
        }
    }
}
