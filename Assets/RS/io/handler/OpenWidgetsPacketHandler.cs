using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RS
{
    public class OpenWidgetsPacketHandler : PacketHandler
    {
        public void Handle(int opcode, JagexBuffer buffer)
        {
            var viewportWidget = buffer.ReadUShortA();
            var sidebarWidget = buffer.ReadUShort();
            GameContext.Chat.OverlayWidget = null;
            GameContext.ViewportWidget = new Widget(GameContext.Cache.GetWidgetConfig(viewportWidget));
            GameContext.TabArea.TabWidget = new Widget(GameContext.Cache.GetWidgetConfig(sidebarWidget));
        }
    }
}
