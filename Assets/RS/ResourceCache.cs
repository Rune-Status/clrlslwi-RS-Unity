using System;

using UnityEngine;

namespace RS
{
    /// <summary>
    /// Provides cached game assets, such as textures and shaders.
    /// </summary>
    public static class ResourceCache
    {
        public static Shader TransparentDiffuseShader;
        public static Shader DiffuseShader;
        public static Shader VertexColoredShader;
        public static Shader CutoutTransparentDiffuseShader;
        public static Shader ItemClickedShader;
        public static Material ItemClickedMaterial;

        public static Texture2D[] MapFunction = new Texture2D[71];
        public static Texture2D[] MapScene = new Texture2D[80];
        public static Texture2D[] RankIcons = new Texture2D[2];
        public static Texture2D InvBack;
        public static Texture2D Redstone1;
        public static Texture2D Redstone2;
        public static Texture2D Redstone3;
        public static Texture2D ChatBack;
        public static Texture2D[] Redstone = new Texture2D[10];
        public static Texture2D[] Icons = new Texture2D[14];
        public static Texture2D Mapback;
        public static Texture2D[] Hitmarks = new Texture2D[5];
        public static Texture2D[] Crosses = new Texture2D[8];
        public static Texture2D Compass;
        public static Texture2D[] MapDots = new Texture2D[5];
        public static Texture2D[] MapMarkers = new Texture2D[2];
        public static Texture2D[] HeadIconsPK = new Texture2D[1];
        public static Texture2D[] HeadIconsPrayer = new Texture2D[9];
        public static Texture2D[] HeadIconsHint = new Texture2D[5];


        /// <summary>
        /// The root directory that the cache is stored at.
        /// </summary>
        public static string RootDirectory
        {
            get
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.WindowsEditor:
                    case RuntimePlatform.WindowsPlayer:
                    case RuntimePlatform.WindowsWebPlayer:
                        return Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\resolute\");
                }
                return "";
            }
        }

        /// <summary>
        /// The file that cache data is stored in.
        /// </summary>
        public static string CacheFile
        {
            get
            {
                return RootDirectory + "cache.res";
            }
        }

        /// <summary>
        /// Initializes the resource cache.
        /// </summary>
        public static void Init()
        {
            TransparentDiffuseShader = Shader.Find("Transparent/Diffuse");
            DiffuseShader = Shader.Find("Diffuse");
            VertexColoredShader = Shader.Find("Custom/Vertex Colored");
            CutoutTransparentDiffuseShader = TransparentDiffuseShader;//Shader.Find("Legacy Shaders/Transparent/Cutout/Diffuse");
            ItemClickedShader = Shader.Find("ItemClicked");
            ItemClickedMaterial = new Material(ItemClickedShader);

            InvBack = GameContext.Cache.GetImageAsTex("invback", 0);
            Redstone1 = GameContext.Cache.GetImageAsTex("redstone1", 0);
            Redstone2 = GameContext.Cache.GetImageAsTex("redstone2", 0);
            Redstone3 = GameContext.Cache.GetImageAsTex("redstone2", 0);
            ChatBack = GameContext.Cache.GetImageAsTex("chatback", 0);

            Redstone[0] = GameContext.Cache.GetImageAsTex("redstone1", 0);
            Redstone[1] = GameContext.Cache.GetImageAsTex("redstone2", 0);
            Redstone[2] = GameContext.Cache.GetImageAsTex("redstone2", 0);

            Redstone[3] = GameContext.Cache.GetImageAsTex("redstone1", 0);
            Redstone[3] = TextureUtils.FlipHorizontal(Redstone[3]);

            Redstone[4] = GameContext.Cache.GetImageAsTex("redstone2", 0);
            Redstone[4] = TextureUtils.FlipHorizontal(Redstone[4]);

            Redstone[5] = GameContext.Cache.GetImageAsTex("redstone1", 0);
            Redstone[5] = TextureUtils.FlipVertical(Redstone[5]);

            Redstone[6] = GameContext.Cache.GetImageAsTex("redstone2", 0);
            Redstone[6] = TextureUtils.FlipVertical(Redstone[6]);

            Redstone[7] = GameContext.Cache.GetImageAsTex("redstone2", 0);
            Redstone[7] = TextureUtils.FlipVertical(Redstone[7]);

            Redstone[8] = GameContext.Cache.GetImageAsTex("redstone1", 0);
            Redstone[8] = TextureUtils.FlipHorizontal(Redstone[8]);
            Redstone[8] = TextureUtils.FlipVertical(Redstone[8]);

            Redstone[9] = GameContext.Cache.GetImageAsTex("redstone2", 0);
            Redstone[9] = TextureUtils.FlipHorizontal(Redstone[9]);
            Redstone[9] = TextureUtils.FlipVertical(Redstone[9]);

            Mapback = GameContext.Cache.GetImageAsTex("mapback", 0);
            Compass = GameContext.Cache.GetImageAsTex("compass", 0);

            for (var i = 0; i < MapDots.Length; i++)
            {
                MapDots[i] = GameContext.Cache.GetImageAsTex("mapdots", i);
            }

            for (var i = 0; i < MapMarkers.Length; i++)
            {
                MapMarkers[i] = GameContext.Cache.GetImageAsTex("mapmarker", i);
            }

            for (var i = 0; i < HeadIconsHint.Length; i++)
            {
                HeadIconsHint[i] = GameContext.Cache.GetImageAsTex("headicons_hint", i);
            }

            for (var i = 0; i < HeadIconsPK.Length; i++)
            {
                HeadIconsPK[i] = GameContext.Cache.GetImageAsTex("headicons_pk", i);
            }

            for (var i = 0; i < HeadIconsPrayer.Length; i++)
            {
                HeadIconsPrayer[i] = GameContext.Cache.GetImageAsTex("headicons_prayer", i);
            }

            for (var j = 0; j < MapFunction.Length; j++)
            {
                try
                {
                    MapFunction[j] = GameContext.Cache.GetImageAsTex("mapfunction", j);
                }
                catch (Exception e) { }
            }

            for (var j = 0; j < MapScene.Length; j++)
            {
                try
                {
                    MapScene[j] = GameContext.Cache.GetImageAsTex("mapscene", j);
                }
                catch (Exception e) { }
            }

            for (var j = 0; j < RankIcons.Length; j++)
            {
                try
                {
                    RankIcons[j] = GameContext.Cache.GetImageAsTex("mod_icons", j);
                }
                catch (Exception e) { }
            }

            for (var i = 0; i < 5; i++)
            {
                try
                {
                    Hitmarks[i] = GameContext.Cache.GetImageAsTex("hitmarks", i);
                }
                catch (Exception e) { }
            }

            for (var i = 0; i < 8; i++)
            {
                try
                {
                    Crosses[i] = GameContext.Cache.GetImageAsTex("cross", i);
                }
                catch (Exception e) { }
            }

            for (int i = 0; i < 13; i++)
            {
                try
                {
                    Icons[i] = GameContext.Cache.GetImageAsTex("sideicons", i);
                }
                catch (Exception e) { }
            }

        }
    }
}
