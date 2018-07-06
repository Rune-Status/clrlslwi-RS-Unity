using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace RS
{
    public class SetPlayerOptionPacketHandler : PacketHandler
    {
        public void Handle(int opcode, JagexBuffer buffer)
        {
            var index = buffer.ReadUByteC();
            var priority = buffer.ReadUByteA() == 0;
            var option = buffer.ReadString(10);

            if (option.ToLower().Equals("null"))
            {
                GameContext.PlayerOptions[index - 1] = null;
            }
            else
            {
                GameContext.PlayerOptions[index - 1] = new PlayerOption(option, priority);
            }
            
        }
    }
}
