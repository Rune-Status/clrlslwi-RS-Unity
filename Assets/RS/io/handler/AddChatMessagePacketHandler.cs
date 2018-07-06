using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RS
{
    public class AddChatMessagePacketHandler : PacketHandler
    {
        public void Handle(int opcode, JagexBuffer buffer)
        {
            var message = buffer.ReadString(10);
            GameContext.Chat.Add(new ChatMessage(MessageType.Ambiguous, null, message));
        }
    }
}
