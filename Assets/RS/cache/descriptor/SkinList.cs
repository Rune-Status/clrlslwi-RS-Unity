namespace RS
{
    public class SkinList
	{
        /// <summary>
        /// The total number of transformations.
        /// </summary>
		public int Count;

        /// <summary>
        /// The opcodes that define how to apply modifications.
        /// </summary>
		public int[] Opcodes;

        /// <summary>
        /// The vertices being modified.
        /// </summary>
		public int[][] Vertices;
		
        /// <summary>
        /// Creates a skin list from binary data.
        /// </summary>
        /// <param name="buf">The buffer containing the binary data.</param>
		public SkinList(JagexBuffer buf) {
			Count = buf.ReadUByte();
			Opcodes = new int[Count];
			Vertices = new int[Count][];
			
			for (var i = 0; i < Count; i++)
            {
				Opcodes[i] = buf.ReadUByte();
			}

            for (var i = 0; i < Count; i++)
            {
                var size = buf.ReadUByte();
                Vertices[i] = new int[size];
                for (var j = 0; j < size; j++)
                {
                    Vertices[i][j] = buf.ReadUByte();
                }
            }
        }
	}
}

