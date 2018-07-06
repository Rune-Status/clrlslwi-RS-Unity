using UnityEngine;

namespace RS
{
    /// <summary>
    /// Contains constants for the game frame area.
    /// </summary>
    public static class GameFrameArea
    {
        /// <summary>
        /// Defines the viewport area in fixed mode.
        /// </summary>
        public static readonly Rect Viewport = new Rect(4, 4, 512, 334);
        /// <summary>
        /// Defines the side area in fixed mode.
        /// </summary>
        public static readonly Rect Side = new Rect(523, 169, 243, 335);
    }
}
