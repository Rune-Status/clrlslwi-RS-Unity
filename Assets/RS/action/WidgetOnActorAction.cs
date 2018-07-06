namespace RS
{
    /// <summary>
    /// A menu action that handles using a selected widget on an actor.
    /// </summary>
    public class WidgetOnActorAction : AbstractMenuAction
    {
        private int actorIndex;
        private int widgetId;

        public WidgetOnActorAction(int actorIndex, int widgetId, string caption, bool hasPriority)
        {
            this.actorIndex = actorIndex;
            this.widgetId = widgetId;
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
                var @out = new Packet(131);
                @out.WriteLEShortA(actorIndex);
                @out.WriteShortA(widgetId);
                GameContext.NetworkHandler.Write(@out);
            }

        }
    }
}
