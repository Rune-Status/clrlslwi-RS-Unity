namespace RS
{
    /// <summary>
    /// Represents a wall object within the scene.
    /// </summary>
    public class WallObject : SceneObject
    {
        /// <summary>
        /// The rotation and type of this object.
        /// </summary>
        public byte RotationType;
        public Model Extension;
        public Model Root;
        public int RotationFlag;
        public int SceneX;
        public int SceneZ;
        public int SceneY;
        public long UniqueId;

        public WallObject(byte arrangement, Model extension, Model root, int rotationFlag, int sceneX, int sceneY, int sceneZ, long uniqueId)
        {
            RotationType = arrangement;
            Extension = extension;
            Root = root;
            RotationFlag = rotationFlag;
            SceneX = sceneX;
            SceneY = sceneY;
            SceneZ = sceneZ;
            UniqueId = uniqueId;
        }

    }
}
