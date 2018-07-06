using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RS
{
    public class OpenChatUnderlayWidgetPacketHandler : PacketHandler
    {
        public void Handle(int opcode, JagexBuffer buffer)
        {
            var index = buffer.ReadShortA();
            if (index == -1)
            {
                GameContext.Chat.UnderlayWidget = null;
            }
            else
            {
                var desc = GameContext.Cache.GetWidgetConfig(index);
                GameContext.Chat.UnderlayWidget = new Widget(desc);
            }
        }
    }
}
