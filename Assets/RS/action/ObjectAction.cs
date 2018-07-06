namespace RS
{
    /// <summary>
    /// A menu action that handles a click on an object.
    /// </summary>
    public class ObjectAction : AbstractMenuAction
    {
        private int optionIndex;
        private int objectIndex;
        private long objectUniqueId;
        private int objectX;
        private int objectY;

        public ObjectAction(int optionIndex, int objectIndex, long objectUniqueId, int objectX, int objectY, string caption, bool hasPriority)
        {
            this.optionIndex = optionIndex;
            this.objectIndex = objectIndex;
            this.objectUniqueId = objectUniqueId;
            this.objectX = objectX;
            this.objectY = objectY;
            Caption = caption;
            HasPriority = hasPriority;
        }

        public override void Callback(ActionMenu menu)
        {
            switch (optionIndex)
            {
                case 0:
                    {
                        GameContext.InteractWithObject(objectX, objectY, objectUniqueId);
                        var @out = new Packet(132);
                        @out.WriteLEShortA(objectX + GameContext.MapBaseX);
                        @out.WriteShort(objectIndex);
                        @out.WriteShortA(objectY + GameContext.MapBaseY);
                        GameContext.NetworkHandler.Write(@out);
                        break;
                    }

                case 1:
                    {
                        GameContext.InteractWithObject(objectX, objectY, objectUniqueId);
                        var @out = new Packet(252);
                        @out.WriteLEShortA(objectIndex);
                        @out.WriteLEShort(objectY + GameContext.MapBaseY);
                        @out.WriteShortA(objectX + GameContext.MapBaseX);
                        GameContext.NetworkHandler.Write(@out);
                        break;
                    }

                case 2:
                    {
                        GameContext.InteractWithObject(objectX, objectY, objectUniqueId);
                        var @out = new Packet(70);
                        @out.WriteLEShort(objectX + GameContext.MapBaseX);
                        @out.WriteShort(objectY + GameContext.MapBaseY);
                        @out.WriteLEShortA(objectIndex);
                        GameContext.NetworkHandler.Write(@out);
                        break;
                    }

                case 3:
                    {
                        GameContext.InteractWithObject(objectX, objectY, objectUniqueId);
                        var @out = new Packet(234);
                        @out.WriteLEShortA(objectX + GameContext.MapBaseX);
                        @out.WriteShortA(objectIndex);
                        @out.WriteLEShortA(objectY + GameContext.MapBaseY);
                        GameContext.NetworkHandler.Write(@out);
                        break;
                    }

                case 4:
                    {
                        GameContext.InteractWithObject(objectX, objectY, objectUniqueId);
                        var @out = new Packet(228);
                        @out.WriteShortA(objectIndex);
                        @out.WriteShortA(objectY + GameContext.MapBaseY);
                        @out.WriteShort(objectX + GameContext.MapBaseX);
                        GameContext.NetworkHandler.Write(@out);
                        break;
                    }
            }
        }
    }
}
