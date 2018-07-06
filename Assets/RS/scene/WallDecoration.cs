namespace RS
{
    /// <summary>
    /// Represents a wall decoration within the scene.
    /// </summary>
    public class WallDecoration : SceneObject
    {
        /// <summary>
        /// The unique id of this object.
        /// </summary>
        public long UniqueId;
        /// <summary>
        /// The scene x coordinate of this object.
        /// </summary>
        public int SceneX;
        /// <summary>
        /// The scene y coordinate of this object.
        /// </summary>
        public int SceneY;
        /// <summary>
        /// The scene z coordinate of this object.
        /// </summary>
        public int SceneZ;
        /// <summary>
        /// The flags of this object.
        /// </summary>
        public int Flags;
        /// <summary>
        /// If this object is animated.
        /// </summary>
        public bool IsAnimatedObject = false;
        /// <summary>
        /// The arrangement specifies type and rotation.
        /// </summary>
        public int Arrangement;
        /// <summary>
        /// The rotation of this object.
        /// </summary>
        public int Rotation;

        public object Node;

        public WallDecoration(int arrangement, int flags, object node, int rotation, long uniqueId, int sceneX, int sceneY, int sceneZ)
        {
            Arrangement = arrangement;
            Flags = flags;
            Node = node;
            Rotation = rotation;
            UniqueId = uniqueId;
            SceneX = sceneX;
            SceneY = sceneY;
            SceneZ = sceneZ;
            IsAnimatedObject = (Node is AnimatedObject);
        }
    }
}
