namespace RS
{
    /// <summary>
    /// Represents an underlay tile within the scene.
    /// </summary>
    public class UnderlayTile
    {
        public int HslNorthEast;
        public int HslNorthWest;
        public int HslSouthEast;
        public int HslSouthWest;
        public bool IsFlat;
        public int UnderlayMinimapRgb;
        public int TextureIndex;

        public UnderlayTile(int hslSouthWest, int hslSouthEast, int hslNorthEast, int hslNorthWest, int textureIndex, int underlayMinimapRgb, bool isFlat)
        {
            HslSouthWest = hslSouthWest;
            HslNorthEast = hslNorthEast;
            HslNorthWest = hslNorthWest;
            HslSouthEast = hslSouthEast;
            TextureIndex = textureIndex;
            UnderlayMinimapRgb = underlayMinimapRgb;
            IsFlat = isFlat;
        }
    }
}
