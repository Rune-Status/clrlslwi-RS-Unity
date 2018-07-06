using System;
using System.IO;

using UnityEngine;
using System.Threading;

namespace RS
{
    /// <summary>
    /// Handles loading of the game cache asynchronously.
    /// </summary>
    public class AsyncCacheLoader
    {
        /// <summary>
        /// The cache being loaded.
        /// </summary>
        private Cache cache;

        /// <summary>
        /// The total progress (0-100) we've made in loading the cache.
        /// </summary>
        private int progress;

        /// <summary>
        /// If the cache has loaded.
        /// </summary>
        private bool done = false;

        /// <summary>
        /// The total progress (0-100) we've made in loading the cache.
        /// </summary>
        public int Progress
        {
            get
            {
                return progress;
            }
            set
            {
                Interlocked.Exchange(ref progress, value);
            }
        }

        /// <summary>
        /// If the cache is loaded.
        /// </summary>
        public bool Completed
        {
            get
            {
                return done;
            }
        }

        public AsyncCacheLoader(Cache cache)
        {
            this.cache = cache;
        }

        public void Run()
        {
            new Thread(() =>
            {
                try
                {
                    InitTables();
                } catch (Exception e)
                {
                    Debug.Log("Failed to load" + e);
                }
            }).Start();
        }

        /// <summary>
        /// Sets up the cache.
        /// </summary>
        private void SetupCache()
        {
            GameContext.Cache.DataStream = new FileStreamJagexBuffer(new FileStream(@"C:\Users\Cody\rs317_cache\main_file_cache.dat", FileMode.Open, FileAccess.Read));
            GameContext.Cache.IdxStreams.Add(0, new FileStreamJagexBuffer(new FileStream(@"C:\Users\Cody\rs317_cache\main_file_cache.idx0", FileMode.Open, FileAccess.Read)));
            GameContext.Cache.IdxStreams.Add(1, new FileStreamJagexBuffer(new FileStream(@"C:\Users\Cody\rs317_cache\main_file_cache.idx1", FileMode.Open, FileAccess.Read)));
            GameContext.Cache.IdxStreams.Add(2, new FileStreamJagexBuffer(new FileStream(@"C:\Users\Cody\rs317_cache\main_file_cache.idx2", FileMode.Open, FileAccess.Read)));
            GameContext.Cache.IdxStreams.Add(3, new FileStreamJagexBuffer(new FileStream(@"C:\Users\Cody\rs317_cache\main_file_cache.idx3", FileMode.Open, FileAccess.Read)));
            GameContext.Cache.IdxStreams.Add(4, new FileStreamJagexBuffer(new FileStream(@"C:\Users\Cody\rs317_cache\main_file_cache.idx4", FileMode.Open, FileAccess.Read)));
            GameContext.Cache.Setup();
        }

        /// <summary>
        /// Initializes all providers within the cache.
        /// </summary>
        private void InitProviders()
        {
            var configArchive = cache.GetArchive(2);
            cache.ItemConfigProvider = new ItemConfigProvider(configArchive);
            cache.ObjectConfigProvider = new ObjectConfigProvider(configArchive);
            cache.UnderlayDescProvider = new UnderlayFloorDescriptorProvider(configArchive);
            cache.IdkProvider = new IdentityKitProvider(configArchive);
            cache.SeqProvider = new SequenceProvider(configArchive);
            cache.SeqFrameProvider = new SequenceFrameProvider();
            cache.SeqFrameProvider.Init(cache.FileVersions[1].Length, cache.AnimIndex.Length);
            cache.GraphicDescProvider = new GraphicDescriptorProvider(configArchive);
        }

        /// <summary>
        /// Initializes all fonts within the cache.
        /// </summary>
        private void InitFonts()
        {
            var fontArchive = cache.GetArchive(1);
            cache.SmallFont = new BitmapFont("p11_full", fontArchive);
            cache.NormalFont = new BitmapFont("p12_full", fontArchive);
            cache.BoldFont = new BitmapFont("b12_full", fontArchive);
            cache.FancyFont = new BitmapFont("q8_full", fontArchive);
        }

        /// <summary>
        /// Initializes all textures within the cache.
        /// </summary>
        private void InitTextures()
        {
            var archive = cache.GetArchive(6);
            for (var i = 0; i < 50; i++)
            {
                var bitmap = new Bitmap(archive, "" + i, 0);
                bitmap.Crop();
                cache.Textures.Add(i, bitmap);
            }
        }

        /// <summary>
        /// Initializes everything within the cache.
        /// </summary>
        private void InitTables()
        {
            try {
                SetupCache();
                InitTextures();
                InitFonts();
                InitProviders();
            } finally
            {
                done = true;
            }
        }

    }
}
