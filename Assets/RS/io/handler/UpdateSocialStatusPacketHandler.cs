using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RS
{
    /// <summary>
    /// Handles the packet which changes the status of the social system.
    /// </summary>
    public class UpdateSocialStatusPacketHandler : PacketHandler
    {
        public void Handle(int opcode, JagexBuffer buffer)
        {
            GameContext.SocialStatus = (SocialStatus)buffer.ReadUByte();
        }
    }
}
