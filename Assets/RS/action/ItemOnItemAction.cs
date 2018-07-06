using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RS
{
    public class ItemOnItemAction : AbstractMenuAction
    {
        private int usedWidgetId;
        private int usedWidgetSlot;
        private int usedItemIndex;

        private int onWidgetId;
        private int onWidgetSlot;
        private int onItemIndex;

        public ItemOnItemAction(int usedWidgetId, int usedWidgetSlot, int usedItemIndex, int onWidgetId, int onWidgetSlot, int onItemIndex, string caption, bool hasPriority)
        {
            this.usedWidgetId = usedWidgetId;
            this.usedWidgetSlot = usedWidgetSlot;
            this.usedItemIndex = usedItemIndex;
            this.onWidgetId = onWidgetId;
            this.onWidgetSlot = onWidgetSlot;
            this.onItemIndex = onItemIndex;
            base.Caption = caption;
            base.HasPriority = hasPriority;
        }

        public override void Callback(ActionMenu menu)
        {
            var @out = new Packet(53);
            @out.WriteShort(onWidgetSlot);
            @out.WriteShortA(usedWidgetSlot);
            @out.WriteLEShortA(onItemIndex);
            @out.WriteShort(usedWidgetId);
            @out.WriteLEShort(usedItemIndex);
            @out.WriteShort(onWidgetId);
            GameContext.NetworkHandler.Write(@out);
        }
    }
}
