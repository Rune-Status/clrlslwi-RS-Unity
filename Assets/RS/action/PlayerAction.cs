using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RS
{
    public class PlayerAction : AbstractMenuAction
    {
        private int optionIndex;
        private int playerIndex;

        public PlayerAction(int optionIndex, int playerIndex, string caption, bool hasPriority)
        {
            this.optionIndex = optionIndex;
            this.playerIndex = playerIndex;
            base.Caption = caption;
            base.HasPriority = hasPriority;
        }

        public override void Callback(ActionMenu menu)
        {
            var player = GameContext.Players[playerIndex];
            GameContext.WalkTo(2, 1, 1, GameContext.Self.PathX[0], GameContext.Self.PathY[0], player.PathX[0], player.PathY[0], 0, 0, 0, false);
            switch (optionIndex)
            {
                case 0:
                    {
                        var @out = new Packet(128);
                        @out.WriteShort(playerIndex);
                        GameContext.NetworkHandler.Write(@out);
                        break;
                    }
                case 1:
                    {
                        var @out = new Packet(153);
                        @out.WriteLEShort(playerIndex);
                        GameContext.NetworkHandler.Write(@out);
                        break;
                    }
                case 2:
                    {
                        var @out = new Packet(73);
                        @out.WriteLEShort(playerIndex);
                        GameContext.NetworkHandler.Write(@out);
                        break;
                    }
                case 3:
                    {
                        var @out = new Packet(139);
                        @out.WriteLEShort(playerIndex);
                        GameContext.NetworkHandler.Write(@out);
                        break;
                    }
                case 4:
                    {
                        var @out = new Packet(39);
                        @out.WriteLEShort(playerIndex);
                        GameContext.NetworkHandler.Write(@out);
                        break;
                    }
            }
        }
    }
}
