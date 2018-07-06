namespace RS
{
    /// <summary>
    /// A menu action that handles interaction with items.
    /// </summary>
    public class ItemAction : AbstractMenuAction
    {
        public int OptionIndex;
        public int WidgetId;
        public int WidgetSlot;
        public int ItemIndex;

        public ItemAction(int optionIndex, int widgetId, int widgetSlot, int itemIndex, string caption, bool hasPriority)
        {
            OptionIndex = optionIndex;
            WidgetId = widgetId;
            WidgetSlot = widgetSlot;
            ItemIndex = itemIndex;
            Caption = caption;
            HasPriority = hasPriority;
        }

        public override void Callback(ActionMenu menu)
        {
            switch (OptionIndex)
            {
                case 0:
                    {
                        var @out = new Packet(122);
                        @out.WriteLEShortA(WidgetId);
                        @out.WriteShortA(WidgetSlot);
                        @out.WriteLEShort(ItemIndex);
                        GameContext.NetworkHandler.Write(@out);
                        break;
                    }
                case 1:
                    {
                        var @out = new Packet(41);
                        @out.WriteShort(ItemIndex);
                        @out.WriteShortA(WidgetSlot);
                        @out.WriteShortA(WidgetId);
                        GameContext.NetworkHandler.Write(@out);
                        break;
                    }
                case 2:
                    {
                        var @out = new Packet(16);
                        @out.WriteShortA(ItemIndex);
                        @out.WriteLEShortA(WidgetSlot);
                        @out.WriteLEShortA(WidgetId);
                        GameContext.NetworkHandler.Write(@out);
                        break;
                    }
                case 3:
                    {
                        var @out = new Packet(75);
                        @out.WriteLEShortA(WidgetId);
                        @out.WriteLEShort(WidgetSlot);
                        @out.WriteShortA(ItemIndex);
                        GameContext.NetworkHandler.Write(@out);
                        break;
                    }
                case 4:
                    {
                        var @out = new Packet(87);
                        @out.WriteShortA(ItemIndex);
                        @out.WriteShort(WidgetId);
                        @out.WriteShortA(WidgetSlot);
                        GameContext.NetworkHandler.Write(@out);
                        break;
                    }
                case 5:
                    GameContext.SetSelectedItem(WidgetId, WidgetSlot, ItemIndex);
                    break;
                case 6:
                    var desc = GameContext.Cache.GetItemConfig(ItemIndex);
                    GameContext.Chat.Add(new ChatMessage(MessageType.Ambiguous, "", "It's a " + desc.name + "."));
                    break;
            }
        }
    }
}
