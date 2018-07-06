
using UnityEngine;

namespace RS
{
    /// <summary>
    /// Represents a tile within the scene.
    /// </summary>
    public class SceneTile
    {
        /// <summary>
        /// The x coordinate of this tile.
        /// </summary>
        public int X;
        /// <summary>
        /// The y coordinate of this tile.
        /// </summary>
        public int Y;
        /// <summary>
        /// The plane this tile is on.
        /// </summary>
        public int Plane;

        /// <summary>
        /// The flags of this tile.
        /// </summary>
        public int Flags;
        /// <summary>
        /// The bridge tile bound to this tile.
        /// </summary>
        public SceneTile Bridge;

        /// <summary>
        /// The underlay of this tile.
        /// </summary>
        public UnderlayTile Underlay;
        /// <summary>
        /// The overlay of this tile.
        /// </summary>
        public OverlayTile Overlay;

        /// <summary>
        /// The material of the underlay on this tile.
        /// </summary>
        public Material UnderlayMaterial;
        /// <summary>
        /// The matierlao f the overlay on this tile.
        /// </summary>
        public Material OverlayMaterial;

        /// <summary>
        /// The number of interactive objects on this tile.
        /// </summary>
        public int InteractiveCount;
        /// <summary>
        /// The interactive objects on this tile.
        /// </summary>
        public InteractiveObject[] Interactives = new InteractiveObject[5];

        /// <summary>
        /// The wall on this tile.
        /// </summary>
        public WallObject Wall;
        /// <summary>
        /// The wall decoration on this tile.
        /// </summary>
        public WallDecoration WallDeco;
        /// <summary>
        /// The ground decoration on this tile.
        /// </summary>
        public GroundDecoration GroundDeco;
        /// <summary>
        /// The ground items on this tile.
        /// </summary>
        public GroundItems GroundItems;
        
        public SceneTile(int x, int y, int plane)
        {
            X = x;
            Y = y;
            Plane = plane;
        }
    }
}
