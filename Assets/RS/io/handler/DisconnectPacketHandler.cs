using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RS
{
    public class DisconnectPacketHandler : PacketHandler
    {
        public void Handle(int opcode, JagexBuffer buffer)
        {
            GameContext.NetworkHandler.ResetState();
        }
    }
}
