using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace RS
{
    public class WalkHereMenuAction : MenuAction
    {
        private int tileX;
        private int tileY;

        public WalkHereMenuAction(int tileX, int tileY)
        {
            this.tileX = tileX;
            this.tileY = tileY;
        }

        public string GetCaption()
        {
            return "Walk here";
        }

        public void Callback(ActionMenu menu)
        {
            GameContext.WalkTo(0, 0, 0, GameContext.Self.PathX[0], GameContext.Self.PathY[0], tileX, tileY, 0, 0, 0, true);
            var pos = InputUtils.mousePosition;
            GameContext.Cross.Show(1, (int)pos.x, (int)pos.y);
        }

        public bool CheckHasPriority()
        {
            return false;
        }

        public int GetCursorId()
        {
            return -1;
        }
    }
}
