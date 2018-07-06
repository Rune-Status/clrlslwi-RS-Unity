namespace RS
{
    /// <summary>
    /// A menu action that handles using a selected item on an actor.
    /// </summary>
    public class ItemOnActorAction : AbstractMenuAction
    {
        private int actorIndex;
        private int widgetId;
        private int widgetSlot;
        private int itemIndex;

        public ItemOnActorAction(int actorIndex, int widgetId, int widgetSlot, int itemIndex, string caption, bool hasPriority)
        {
            this.actorIndex = actorIndex;
            this.widgetId = widgetId;
            this.widgetSlot = widgetSlot;
            this.itemIndex = itemIndex;
            Caption = caption;
            HasPriority = hasPriority;
        }

        public override void Callback(ActionMenu menu)
        {
            var actor = GameContext.Actors[actorIndex];
            if (actor != null)
            {
                GameContext.ShowCross(2);
                GameContext.WalkTo(2, 1, 1, GameContext.Self.PathX[0], GameContext.Self.PathY[0], actor.PathX[0], actor.PathY[0], 0, 0, 0, false);
                var @out = new Packet(57);
                @out.WriteShortA(itemIndex);
                @out.WriteShortA(actorIndex);
                @out.WriteLEShort(widgetSlot);
                @out.WriteShortA(widgetId);
                GameContext.NetworkHandler.Write(@out);
            }
            
        }
    }
}