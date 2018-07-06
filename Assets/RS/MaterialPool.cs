using UnityEngine;

namespace RS
{
    /// <summary>
    /// A pool of materials, for caching materials..
    /// </summary>
    public class MaterialPool
    {
        private Texture2D invalidTex;
        private Texture2D[] texCache = new Texture2D[1024];
        private Material[] texMatCache = new Material[1024];

        private bool[] hasTransparency = new bool[1024];
        private bool[] transparencySet = new bool[1024];

        /// <summary>
        /// Retrieves the invalid texture texture.
        /// </summary>
        /// <returns>The invalid texture texture.</returns>
        private Texture2D GetInvalidTex()
        {
            if (invalidTex == null)
            {
                invalidTex = new Texture2D(1, 1, TextureFormat.RGB24, false, true);
                invalidTex.SetPixel(1, 1, new Color(1, 1, 1));
            }
            return invalidTex;
        }

        /// <summary>
        /// Determines if a cached texture has transparency.
        /// </summary>
        /// <param name="index">The index of the texture to check.</param>
        /// <returns>If the texture at the provided index has transparency.</returns>
        public bool HasTrasparency(int index)
        {
            if (index < 0 || index >= texCache.Length)
            {
                return false;
            }

            if (texCache[index] == null || !transparencySet[index])
            {
                hasTransparency[index] = TextureUtils.HasTransparency(GetTextureAsUnity(index));
                transparencySet[index] = true;
            }
            return hasTransparency[index];
        }

        /// <summary>
        /// Retrieves a texture.
        /// </summary>
        /// <param name="index">The index of the texture to retrieve.</param>
        /// <param name="forceNew">If a new texture will always be created.</param>
        /// <returns>The created texture.</returns>
        public Texture2D GetTextureAsUnity(int index, bool forceNew = false)
        {
            if (index < 0 || index >= texCache.Length)
            {
                return GetInvalidTex();
            }

            if (texCache[index] != null)
            {
                var tmp = texCache[index];
                if (forceNew)
                {
                    var copy = new Texture2D(tmp.width, tmp.height, tmp.format, false);
                    copy.SetPixels(tmp.GetPixels());
                    copy.Apply();
                    tmp = copy;
                }
                return tmp;
            }

            var tex = GameContext.Cache.GetTextureBitmap(index).ToUnityTexture();
            tex.wrapMode = TextureWrapMode.Repeat;
            tex.filterMode = FilterMode.Trilinear;
            TextureUtils.Replace(tex, new Color32(0xCC, 0, 0xCC, 0xFF), new Color32(0, 0, 0, 0));
            TextureUtils.Replace(tex, new Color32(0xFF, 0, 0xFF, 0xFF), new Color32(0, 0, 0, 0));
            TextureUtils.Replace(tex, new Color32(0xFF, 0xFF, 0xFF, 0xFF), new Color32(0, 0, 0, 0));
            TextureUtils.Replace(tex, new Color32(0, 0, 0, 0xFF), new Color32(0, 0, 0, 0));
            return texCache[index] = tex;
        }
        
        /// <summary>
        /// Retrieves a cached material.
        /// </summary>
        /// <param name="index">The index of the material to retrieve.</param>
        /// <returns>The material cached at the provided index.</returns>
        public Material GetMaterial(int index)
        {
            Material material = null;
            if (texMatCache[index] == null)
            {
                material = Object.Instantiate(new UnityEngine.Material(Shader.Find("Diffuse")));
                material.shader = Shader.Find("Transparent/Diffuse");
                material.mainTexture = GameContext.MaterialPool.GetTextureAsUnity(index);
                texMatCache[index] = material;
            }
            return material;
        }
    }

}
