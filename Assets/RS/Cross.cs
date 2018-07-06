
using UnityEngine;

namespace RS
{
    /// <summary>
    /// Handles the rendering of the cross when the user clicks.
    /// </summary>
    public class Cross
    {
        public float CrossCycle;
        public int CrossType;
        public int CrossX;
        public int CrossY;

        /// <summary>
        /// Shows the cross on the screen.
        /// </summary>
        /// <param name="type">The type of cross being shown.</param>
        /// <param name="x">The x coordinate of the cross.</param>
        /// <param name="y">The y coordinate of the cross.</param>
        public void Show(int type, int x, int y)
        {
            CrossX = x;
            CrossY = y;
            CrossType = type;
            CrossCycle = 0;
        }

        /// <summary>
        /// The index of the cross animation to render on the screen.
        /// </summary>
        private int CrossIndex
        {
            get
            {
                return (int)(CrossCycle / 100);
            }
        }

        /// <summary>
        /// Renders the cross.
        /// </summary>
        public void Render()
        {
            if (CrossType != 0)
            {
                CrossCycle += (Time.deltaTime * 500);
                if (CrossCycle >= 399)
                {
                    CrossType = 0;
                }
            }

            if (CrossType == 1 || CrossType == 2)
            {
                var tex = ResourceCache.Crosses[CrossIndex];
                GUI.DrawTexture(new Rect(CrossX - 8, CrossY - 9, tex.width, tex.height), tex);
            }
        }
    }
}
