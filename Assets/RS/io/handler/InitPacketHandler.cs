﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RS
{
    public class InitPacketHandler : PacketHandler
    {
        public void Handle(int opcode, JagexBuffer buffer)
        {
            GameContext.FriendsStorageType = (SocialStorageCap)buffer.ReadUByteA();
            GameContext.LocalPlayerIndex = buffer.ReadLEUShortA();
        }
    }
}
