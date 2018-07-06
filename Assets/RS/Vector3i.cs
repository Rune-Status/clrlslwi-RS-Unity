namespace RS
{
    /// <summary>
    /// Represents a vector3 backed by integers.
    /// </summary>
    public class Vector3i
    {
        /// <summary>
        /// The x coordinate of the vector.
        /// </summary>
        public int X;
        /// <summary>
        /// The y coordinate of the vector.
        /// </summary>
        public int Y;
        /// <summary>
        /// The z coordinate of the vector.
        /// </summary>
        public int Z;

        public Vector3i(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
