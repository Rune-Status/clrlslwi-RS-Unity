using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RS
{
    public class OpenChatOverlayWidgetPacketHandler : PacketHandler
    {
        public void Handle(int opcode, JagexBuffer buffer)
        {
            var index = buffer.ReadLEShort();
            GameContext.ViewportWidget = null;
            GameContext.TabArea.TabWidget = null;

            if (index == -1)
            {
                GameContext.Chat.OverlayWidget = null;
            }
            else
            {
                var desc = GameContext.Cache.GetWidgetConfig(index);
                GameContext.Chat.OverlayWidget = new Widget(desc);
            }
        }
    }
}
