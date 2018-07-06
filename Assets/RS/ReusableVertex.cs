
using UnityEngine;

namespace RS
{
    /// <summary>
    /// A vertex which can be reused for many things.
    /// </summary>
    public class ReusableVertex
    {
        /// <summary>
        /// The position of the vertex.
        /// </summary>
        public Vector3 Pos;
        /// <summary>
        /// The UV mappings of the texture.
        /// </summary>
        public Vector2 UvPos;
        /// <summary>
        /// The HSL of the vertex.
        /// </summary>
        public int Hsl;

        public ReusableVertex(Vector3 pos, Vector2 uv, int hsl)
        {
            Pos = pos;
            UvPos = uv;
            Hsl = hsl;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + Pos.x.GetHashCode();
                hash = hash * 23 + Pos.y.GetHashCode();
                hash = hash * 23 + Pos.z.GetHashCode();
                hash = hash * 23 + UvPos.x.GetHashCode();
                hash = hash * 23 + UvPos.y.GetHashCode();
                hash = hash * 23 + Hsl;
                return hash;
            }
        }
    }
}
