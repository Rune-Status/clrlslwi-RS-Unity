namespace RS
{
    /// <summary>
    /// A menu action that handles clicks on items on the ground.
    /// </summary>
    public class GroundItemAction : AbstractMenuAction
    {
        private int optionIndex;
        private int itemIndex;
        private long itemUniqueId;
        private int itemX;
        private int itemY;

        public GroundItemAction(int optionIndex, int itemIndex, long itemUniqueId, int itemX, int itemY, string caption, bool hasPriority)
        {
            this.optionIndex = optionIndex;
            this.itemIndex = itemIndex;
            this.itemUniqueId = itemUniqueId;
            this.itemX = itemX;
            this.itemY = itemY;
            base.Caption = caption;
            base.HasPriority = hasPriority;
        }

        public override void Callback(ActionMenu menu)
        {
            var success = false;
            switch (optionIndex)
            {
                case 0:
                    {
                        success = GameContext.WalkTo(2, 0, 0, GameContext.Self.PathX[0], GameContext.Self.PathY[0], itemX, itemY, 0, 0, 0, false);
                        if (!success)
                        {
                            GameContext.WalkTo(2, 1, 1, GameContext.Self.PathX[0], GameContext.Self.PathY[0], itemX, itemY, 0, 0, 0, false);
                        }

                        var @out = new Packet(156);
                        @out.WriteShortA(itemX + GameContext.MapBaseX);
                        @out.WriteLEShort(itemY + GameContext.MapBaseY);
                        @out.WriteLEShortA(itemIndex);
                        GameContext.ShowCross(2);
                        GameContext.NetworkHandler.Write(@out);
                        break;
                    }

                case 1:
                    {
                        success = GameContext.WalkTo(2, 0, 0, GameContext.Self.PathX[0], GameContext.Self.PathY[0], itemX, itemY, 0, 0, 0, false);
                        if (!success)
                        {
                            GameContext.WalkTo(2, 1, 1, GameContext.Self.PathX[0], GameContext.Self.PathY[0], itemX, itemY, 0, 0, 0, false);
                        }

                        var @out = new Packet(156);
                        @out.WriteLEShort(itemY + GameContext.MapBaseY);
                        @out.WriteLEShort(itemIndex);
                        @out.WriteLEShort(itemX + GameContext.MapBaseX);
                        GameContext.ShowCross(2);
                        GameContext.NetworkHandler.Write(@out);
                        break;
                    }

                case 2:
                    {
                        success = GameContext.WalkTo(2, 0, 0, GameContext.Self.PathX[0], GameContext.Self.PathY[0], itemX, itemY, 0, 0, 0, false);
                        if (!success)
                        {
                            GameContext.WalkTo(2, 1, 1, GameContext.Self.PathX[0], GameContext.Self.PathY[0], itemX, itemY, 0, 0, 0, false);
                        }

                        var @out = new Packet(236);
                        @out.WriteLEShort(itemY + GameContext.MapBaseY);
                        @out.WriteShort(itemIndex);
                        @out.WriteLEShort(itemX + GameContext.MapBaseX);
                        GameContext.ShowCross(2);
                        GameContext.NetworkHandler.Write(@out);
                        break;
                    }

                case 3:
                    {
                        success = GameContext.WalkTo(2, 0, 0, GameContext.Self.PathX[0], GameContext.Self.PathY[0], itemX, itemY, 0, 0, 0, false);
                        if (!success)
                        {
                            GameContext.WalkTo(2, 1, 1, GameContext.Self.PathX[0], GameContext.Self.PathY[0], itemX, itemY, 0, 0, 0, false);
                        }

                        var @out = new Packet(253);
                        @out.WriteLEShort(itemX + GameContext.MapBaseX);
                        @out.WriteLEShortA(itemY + GameContext.MapBaseY);
                        @out.WriteShortA(itemIndex);
                        GameContext.ShowCross(2);
                        GameContext.NetworkHandler.Write(@out);
                        break;
                    }

                case 4:
                    {
                        success = GameContext.WalkTo(2, 0, 0, GameContext.Self.PathX[0], GameContext.Self.PathY[0], itemX, itemY, 0, 0, 0, false);
                        if (!success)
                        {
                            GameContext.WalkTo(2, 1, 1, GameContext.Self.PathX[0], GameContext.Self.PathY[0], itemX, itemY, 0, 0, 0, false);
                        }

                        var @out = new Packet(79);
                        @out.WriteLEShort(itemY + GameContext.MapBaseY);
                        @out.WriteShort(itemIndex);
                        @out.WriteShortA(itemX + GameContext.MapBaseX);
                        GameContext.ShowCross(2);
                        GameContext.NetworkHandler.Write(@out);
                        break;
                    }

                case 5:
                    var desc = GameContext.Cache.GetItemConfig(itemIndex);
                    GameContext.Chat.Add(new ChatMessage(MessageType.Ambiguous, "", "It's a " + desc.name + "."));
                    break;
            }
        }
    }
}
