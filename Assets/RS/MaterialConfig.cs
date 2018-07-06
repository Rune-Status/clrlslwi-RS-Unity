using System.Collections.Generic;

namespace RS
{
    /// <summary>
    /// Represents different rendering types of a material.
    /// </summary>
    public enum RenderType
    {
        /// <summary>
        /// The material is made of solid colors.
        /// </summary>
        Colored,
        /// <summary>
        /// The material is made out of a texture.
        /// </summary>
        Textured,
    }

    public class UnderlayMaterialConfig
    {
        public List<int> Triangles = new List<int>();
        public List<SceneTile> Tiles = new List<SceneTile>();
        public RenderType Type;
        public int Color;
        public int Opacity;
        public int Texture;
        public int ColNorth;
        public int ColEast;
        public int ColSouth;
        public int ColWest;

        public UnderlayMaterialConfig(RenderType type, int color, int opacity, int texture)
        {
            Type = type;
            Color = color;
            Opacity = opacity;
            Texture = texture;
        }

        public static bool operator ==(UnderlayMaterialConfig x, UnderlayMaterialConfig y)
        {
            if (ReferenceEquals(x, null) && !ReferenceEquals(y, null)) return false;
            if (ReferenceEquals(y, null) && !ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(x, null) && ReferenceEquals(y, null)) return true;

            return
                x.Type == y.Type &&
                x.Color == y.Color &&
                x.Opacity == y.Opacity &&
                x.Texture == y.Texture;
        }

        public static bool operator !=(UnderlayMaterialConfig x, UnderlayMaterialConfig y)
        {
            return !(x == y);
        }

        public override bool Equals(object o)
        {
            return o is UnderlayMaterialConfig && this == (UnderlayMaterialConfig)o;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + (int)Type;
            hash = (hash * 7) + Opacity;
            hash = (hash * 7) + Texture;
            return hash;
        }
    }
    
    public class OverlayMaterialConfig
    {
        public List<int> Triangles = new List<int>();
        public List<SceneTile> Tiles = new List<SceneTile>();
        public RenderType Type;
        public int Color;
        public int Opacity;
        public int Texture;

        public OverlayMaterialConfig(RenderType type, int color, int opacity, int texture)
        {
            Type = type;
            Color = color;
            Opacity = opacity;
            Texture = texture;
        }

        public static bool operator ==(OverlayMaterialConfig x, OverlayMaterialConfig y)
        {
            if (ReferenceEquals(x, null) && !ReferenceEquals(y, null)) return false;
            if (ReferenceEquals(y, null) && !ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(x, null) && ReferenceEquals(y, null)) return true;

            return
                x.Type == y.Type &&
                x.Color == y.Color &&
                x.Opacity == y.Opacity &&
                x.Texture == y.Texture;
        }

        public static bool operator !=(OverlayMaterialConfig x, OverlayMaterialConfig y)
        {
            return !(x == y);
        }

        public override bool Equals(object o)
        {
            return o is OverlayMaterialConfig && this == (OverlayMaterialConfig)o;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + (int)Type;
            if (Type == RenderType.Textured)
            {
                hash = (hash * 7) + Color;
            }
            hash = (hash * 7) + Opacity;
            hash = (hash * 7) + Texture;
            return hash;
        }
    }

    public class ModelMaterialConfig
    {
        public List<int> Triangles = new List<int>();
        public List<SceneTile> Tiles = new List<SceneTile>();
        public RenderType Type;
        public int Color;
        public int Opacity;
        public int Texture;
        public bool ShouldColor = true;

        public ModelMaterialConfig(RenderType type, int color, int opacity, int texture)
        {
            Type = type;
            Color = color;
            Opacity = opacity;
            Texture = texture;
        }

        public static bool operator ==(ModelMaterialConfig x, ModelMaterialConfig y)
        {
            if (ReferenceEquals(x, null) && !ReferenceEquals(y, null)) return false;
            if (ReferenceEquals(y, null) && !ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(x, null) && ReferenceEquals(y, null)) return true;

            return
                x.Type == y.Type &&
                x.Color == y.Color &&
                x.Opacity == y.Opacity &&
                x.Texture == y.Texture;
        }

        public static bool operator !=(ModelMaterialConfig x, ModelMaterialConfig y)
        {
            return !(x == y);
        }

        public override bool Equals(object o)
        {
            return o is ModelMaterialConfig && this == (ModelMaterialConfig)o;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + (int)Type;
            hash = (hash * 7) + Color;
            hash = (hash * 7) + Opacity;
            hash = (hash * 7) + Texture;
            hash = (hash * 7) + (ShouldColor ? 1 : 0);
            return hash;
        }
    }
}
