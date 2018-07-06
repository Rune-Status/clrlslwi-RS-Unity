namespace RS
{
    /// <summary>
    /// A menu action that handles clicks on an actor/NPC.
    /// </summary>
    public class ActorAction : AbstractMenuAction
    {
        private int optionIndex;
        private int actorIndex;

        public ActorAction(int optionIndex, int actorIndex, string caption, bool hasPriority)
        {
            this.optionIndex = optionIndex;
            this.actorIndex = actorIndex;
            Caption = caption;
            HasPriority = hasPriority;
        }

        public override void Callback(ActionMenu menu)
        {
            var actor = GameContext.Actors[actorIndex];
            GameContext.WalkTo(2, 1, 1, GameContext.Self.PathX[0], GameContext.Self.PathY[0], actor.PathX[0], actor.PathY[0], 0, 0, 0, false);

            var pos = InputUtils.mousePosition;
            GameContext.Cross.Show(2, (int)pos.x, (int)pos.y);

            switch (optionIndex)
            {
                case 0:
                    {
                        var @out = new Packet(155);
                        @out.WriteLEShort(actorIndex);
                        GameContext.NetworkHandler.Write(@out);
                        break;
                    }

                case 1:
                    {
                        var @out = new Packet(72);
                        @out.WriteShortA(actorIndex);
                        GameContext.NetworkHandler.Write(@out);
                        break;
                    }
                case 2:
                    {
                        var @out = new Packet(17);
                        @out.WriteLEShortA(actorIndex);
                        GameContext.NetworkHandler.Write(@out);
                        break;
                    }
                case 3:
                    {
                        var @out = new Packet(21);
                        @out.WriteShort(actorIndex);
                        GameContext.NetworkHandler.Write(@out);
                        break;
                    }
                case 4:
                    {
                        var @out = new Packet(18);
                        @out.WriteLEShort(actorIndex);
                        GameContext.NetworkHandler.Write(@out);
                        break;
                    }
                case 5:
                    var desc = actor.Config;
                    GameContext.Chat.Add(new ChatMessage(MessageType.Ambiguous, "", "It's a " + desc.Name + "."));
                    break;
            }
        }
    }
}
