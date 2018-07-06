using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RS
{
    public class ChangeSmallSettingPacketHandler : PacketHandler
    {
        public void Handle(int opcode, JagexBuffer buffer)
        {
            var index = buffer.ReadLEUShort();
            var value = buffer.ReadByte();

            var settings = GameContext.Settings;
            settings[index] = value;
        }
    }
}
