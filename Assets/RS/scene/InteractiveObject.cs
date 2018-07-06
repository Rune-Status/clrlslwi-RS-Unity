namespace RS
{
    /// <summary>
    /// Represents an interactive object within the scene.
    /// </summary>
    public class InteractiveObject : SceneObject
    {
        /// <summary>
        /// The unique id of this object.
        /// </summary>
        public long UniqueId;
        public object Node;

        /// <summary>
        /// The x scene coordinate of the object.
        /// </summary>
        public int SceneX;
        /// <summary>
        /// The y scene coordinate of the object.
        /// </summary>
        public int SceneY;
        /// <summary>
        /// The z scene coordinate of the object.
        /// </summary>
        public int SceneZ;

        public byte Arrangement;
        public int Rotation;

        public int StartTileX;
        public int StartTileY;

        public int EndTileX;
        public int EndtileY;

        /// <summary>
        /// The plane of the object.
        /// </summary>
        public int Plane;

        /// <summary>
        /// If the object is animated.
        /// </summary>
        public bool IsAnimatedObject = false;

        public InteractiveObject(byte arrangement, object node, int plane, int rotation, int sceneX, int sceneY, int sceneZ, int endTileX, int endTileY, long uid, int startTileX, int startTileY)
        {
            Arrangement = arrangement;
            Node = node;
            Plane = plane;
            Rotation = rotation;
            SceneX = sceneX;
            SceneY = sceneY;
            SceneZ = sceneZ;
            EndTileX = endTileX;
            EndtileY = endTileY;
            UniqueId = uid;
            StartTileX = startTileX;
            StartTileY = startTileY;
            IsAnimatedObject = (Node is AnimatedObject);
        }
    }
}
