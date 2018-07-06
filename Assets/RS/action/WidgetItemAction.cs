namespace RS
{
    /// <summary>
    /// A menu action that handles a click on a widget item.
    /// </summary>
    public class WidgetItemAction : AbstractMenuAction
    {
        private int optionIndex;
        private int widgetId;
        private int widgetSlot;
        private int itemId;

        public WidgetItemAction(int optionIndex, int widgetId, int widgetSlot, int itemId, string caption, bool hasPriority)
        {
            this.optionIndex = optionIndex;
            this.widgetId = widgetId;
            this.widgetSlot = widgetSlot;
            this.itemId = itemId;
            base.Caption = caption;
            base.HasPriority = hasPriority;
        }

        public override void Callback(ActionMenu menu)
        {
            switch (optionIndex)
            {
                case 0:
                    {
                        var @out = new Packet(145);
                        @out.WriteShortA(widgetId);
                        @out.WriteShortA(widgetSlot);
                        @out.WriteShortA(itemId);
                        GameContext.NetworkHandler.Write(@out);
                        break;
                    }
                case 1:
                    {
                        var @out = new Packet(117);
                        @out.WriteLEShortA(widgetId);
                        @out.WriteLEShortA(itemId);
                        @out.WriteLEShort(widgetSlot);
                        GameContext.NetworkHandler.Write(@out);
                        break;
                    }
                case 2:
                    {
                        var @out = new Packet(43);
                        @out.WriteLEShort(widgetId);
                        @out.WriteShortA(itemId);
                        @out.WriteShortA(widgetSlot);
                        GameContext.NetworkHandler.Write(@out);
                        break;
                    }
                case 3:
                    {
                        var @out = new Packet(129);
                        @out.WriteShortA(widgetSlot);
                        @out.WriteShort(widgetId);
                        @out.WriteShortA(itemId);
                        GameContext.NetworkHandler.Write(@out);
                        break;
                    }
                case 4:
                    {
                        var @out = new Packet(135);
                        @out.WriteLEShort(widgetSlot);
                        @out.WriteShortA(widgetId);
                        @out.WriteLEShort(itemId);
                        GameContext.NetworkHandler.Write(@out);
                        break;
                    }
            }
        }
    }
}
