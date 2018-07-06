using System;
using System.Text;
using UnityEngine;

namespace RS
{
    /// <summary>
    /// Handles the packet containing player updates.
    /// </summary>
    public class PlayerUpdatePacketHandler : PacketHandler
    {
        private int entityUpdateCount = 0;
        private int entityCount = 0;
        private int[] entityIndices = new int[2048];
        private int[] entityUpdateIndices = new int[2048];

        private void UpdateLocalPlayerMovement(JagexBuffer b)
        {
            b.BeginBitAccess();
            if (b.ReadBits(1) == 0)
            {
                return;
            }

            int moveType = b.ReadBits(2);
            if (moveType == 0)
            {
                entityIndices[entityCount++] = 2047;
                return;
            }

            if (moveType == 1)
            {
                int direction = b.ReadBits(3);
                GameContext.Self.QueueMove(direction, false);

                if (b.ReadBits(1) == 1)
                {
                    entityIndices[entityCount++] = 2047;
                }
                return;
            }

            if (moveType == 2)
            {
                GameContext.Self.QueueMove(b.ReadBits(3), true);
                GameContext.Self.QueueMove(b.ReadBits(3), true);

                if (b.ReadBits(1) == 1)
                {
                    entityIndices[entityCount++] = 2047;
                }
                return;
            }

            if (moveType == 3)
            {
                GameContext.Plane = b.ReadBits(2);
                int discardMoveQueue = b.ReadBits(1);

                if (b.ReadBits(1) == 1)
                {
                    entityIndices[entityCount++] = 2047;
                }
                
                int y = b.ReadBits(7);
                int x = b.ReadBits(7);
                GameContext.Self.TeleportTo(x, y, discardMoveQueue == 1);
            }
        }

        private void UpdateRemotePlayerMovement(JagexBuffer b)
        {
            int playerCount = b.ReadBits(8);
            if (playerCount < GameContext.PlayerCount)
            {
                for (int k = playerCount; k < GameContext.PlayerCount; k++)
                {
                    entityUpdateIndices[entityUpdateCount++] = GameContext.PlayerIndices[k];
                }
            }

            if (playerCount > GameContext.PlayerCount)
            {
                Debug.Log("ERROR PLAYER UPDATING 0");
                return;
            }

            GameContext.PlayerCount = 0;

            for (int i = 0; i < playerCount; i++)
            {
                int player_index = GameContext.PlayerIndices[i];
                Player p = GameContext.Players[player_index];

                if (b.ReadBits(1) == 0)
                {
                    GameContext.PlayerIndices[GameContext.PlayerCount++] = player_index;
                    p.UpdateCycle = (int)GameContext.LoopCycle;
                }
                else
                {
                    int move_type = b.ReadBits(2);

                    switch (move_type)
                    {
                        case 0:
                            {
                                GameContext.PlayerIndices[GameContext.PlayerCount++] = player_index;
                                p.UpdateCycle = (int)GameContext.LoopCycle;
                                entityIndices[entityCount++] = player_index;
                                break;
                            }
                        case 1:
                            {
                                GameContext.PlayerIndices[GameContext.PlayerCount++] = player_index;
                                p.UpdateCycle = (int)GameContext.LoopCycle;
                                p.QueueMove(b.ReadBits(3), false);

                                if (b.ReadBits(1) == 1)
                                {
                                    entityIndices[entityCount++] = player_index;
                                }
                                break;
                            }
                        case 2:
                            {
                                GameContext.PlayerIndices[GameContext.PlayerCount++] = player_index;
                                p.UpdateCycle = (int)GameContext.LoopCycle;
                                p.QueueMove(b.ReadBits(3), true);
                                p.QueueMove(b.ReadBits(3), true);

                                if (b.ReadBits(1) == 1)
                                {
                                    entityIndices[entityCount++] = player_index;
                                }
                                break;
                            }
                        case 3:
                            {
                                entityUpdateIndices[entityUpdateCount++] = player_index;
                                break;
                            }
                    }
                }
            }
        }

        private void UpdateNewPlayers(JagexBuffer b)
        {
            while (b.BitPosition() + 10 < b.Capacity() * 8)
            {
                var playerIndex = b.ReadBits(11);
                if (playerIndex == 2047)
                {
                    break;
                }

                if (GameContext.Players[playerIndex] == null)
                {
                    GameContext.Players[playerIndex] = new Player();
                    JagexBuffer buf = GameContext.PlayerBuffers[playerIndex];
                    if (buf != null)
                    {
                        GameContext.Players[playerIndex].Update(buf);
                    }
                }

                GameContext.PlayerIndices[GameContext.PlayerCount++] = playerIndex;
                var p = GameContext.Players[playerIndex];
                p.ServerIndex = playerIndex;
                p.UpdateCycle = (int)GameContext.LoopCycle;

                if (b.ReadBits(1) == 1)
                {
                    entityIndices[entityCount++] = playerIndex;
                }

                var discardWalkQueue = b.ReadBits(1);
                var x = b.ReadBits(5);
                var y = b.ReadBits(5);

                if (x > 15)
                {
                    x -= 32;
                }

                if (y > 15)
                {
                    y -= 32;
                }

                p.TeleportTo(GameContext.Self.PathX[0] + y, GameContext.Self.PathY[0] + x, discardWalkQueue == 1);
            }
            b.EndBitAccess();
        }

        private void UpdatePlayerMask(int mask, int index, JagexBuffer b, Player p)
        {
            if ((mask & 0x400) != 0)
            {
                p.MoveStartX = b.ReadUByteS();
                p.MoveStartY = b.ReadUByteS();
                p.MoveEndX = b.ReadUByteS();
                p.MoveEndY = b.ReadUByteS();
                p.MoveCycleEnd = b.ReadLEUShortA() + (int)GameContext.LoopCycle;
                p.MoveCycleStart = b.ReadUShortA() + (int)GameContext.LoopCycle;
                p.MoveDirection = b.ReadUByteS();
                p.ResetQueuedMovements();
            }

            if ((mask & 0x100) != 0)
            {
                p.SpotAnimIndex = b.ReadLEUShort();
                var info = b.ReadInt();
                p.GraphicOffsetY = info >> 16;
                p.SpotAnimCycleEnd = (int)GameContext.LoopCycle + (info & 0xffff);
                p.SpotAnimFrame = 0;
                p.SpotAnimCycle = 0;

                if (p.SpotAnimCycleEnd > GameContext.LoopCycle)
                {
                    p.SpotAnimFrame = -1;
                }

                if (p.SpotAnimIndex == 65535)
                {
                    p.SpotAnimIndex = -1;
                }
            }

            if ((mask & 8) != 0)
            {
                var seqIndex = b.ReadLEUShort();
                var delay = b.ReadUByteC();
                if (seqIndex == 65535)
                {
                    seqIndex = -1;
                }

                var seq = GameContext.Cache.GetSeq(seqIndex);
                if (seq != null)
                {
                    if (seqIndex == p.SeqIndex && seqIndex != -1)
                    {
                        var type = seq.Type;
                        if (type == 1)
                        {
                            p.SeqFrame = 0;
                            p.SeqCycle = 0;
                            p.SeqDelayCycle = delay;
                            p.SeqResetCycle = 0;
                        }
                        else if (type == 2)
                        {
                            p.SeqResetCycle = 0;
                        }
                    }
                    else if (seqIndex == -1 || p.SeqIndex == -1 || (GameContext.Cache.GetSeq(p.SeqIndex) != null && seq.Priority >= GameContext.Cache.GetSeq(p.SeqIndex).Priority))
                    {
                        p.SeqIndex = seqIndex;
                        p.SeqFrame = 0;
                        p.SeqCycle = 0;
                        p.SeqDelayCycle = delay;
                        p.SeqResetCycle = 0;
                        p.StillPathPosition = p.PathPosition;
                    }
                }
            }

            if ((mask & 4) != 0)
            {
                p.SpokenMessage = b.ReadString(10);

                if (p.SpokenMessage.ToCharArray()[0] == '~')
                {
                    p.SpokenMessage = p.SpokenMessage.Substring(1);
                    GameContext.Chat.Add(new ChatMessage(MessageType.Player, p.Name, p.SpokenMessage));
                }
                else if (p == GameContext.Self)
                {
                    GameContext.Chat.Add(new ChatMessage(MessageType.Player, p.Name, p.SpokenMessage));
                }

                p.SpokenColor = 0;
                p.SpokenEffect = 0;
                p.SpokenLife = 150;
            }

            if ((mask & 0x80) != 0)
            {
                var settings = b.ReadShort();
                var rights = b.ReadByte();
                b.ReadByte();
                var length = b.ReadUByteC();
                var startOff = b.Position();
                if (p.Name != null)
                {
                    var buf = new DefaultJagexBuffer(new byte[5000]);
                    b.ReadBytesReversed(buf.Array(), 0, length);
                    p.SpokenMessage = StringUtils.GetFormatted(length, buf);
                    p.SpokenEffect = settings & 0xFF;
                    p.SpokenColor = settings >> 8;
                    p.SpokenLife = 150;

                    var sb = new StringBuilder();
                    sb.Append(p.Name);

                    var msg = new ChatMessage(MessageType.Player, sb.ToString(), p.SpokenMessage);
                    if (rights > 0)
                    {
                        msg.CrownIndex = rights;
                    }

                    GameContext.Chat.Add(msg);
                }

                b.Position(startOff + length);
            }

            if ((mask & 1) != 0)
            {
                p.FaceEntity = b.ReadLEUShort();

                if (p.FaceEntity == 65535)
                {
                    p.FaceEntity = -1;
                }
            }

            if ((mask & 0x10) != 0)
            {
                var payload = new byte[b.ReadUByteC()];
                b.ReadBytes(payload, 0, payload.Length);
                var pb = new DefaultJagexBuffer(payload);
                GameContext.PlayerBuffers[index] = pb;
                p.Update(pb);
            }

            if ((mask & 2) != 0)
            {
                p.FaceX = b.ReadLEUShortA();
                p.FaceY = b.ReadLEUShort();
            }

            if ((mask & 0x20) != 0)
            {
                var damage = b.ReadUShortA();
                var type = b.ReadUByte();
                var icon = b.ReadUByte();
                var soak = b.ReadUShortA();
                p.QueueHit(type, damage, (int)GameContext.LoopCycle);
                p.CurrentHealth = b.ReadUShortA();
                p.MaxHealth = b.ReadUShortA();
                p.EndCombatCycle = (int)GameContext.LoopCycle + 300;
            }

            if ((mask & 0x200) != 0)
            {
                var damage = b.ReadUShortA();
                var type = b.ReadUByte();
                var icon = b.ReadUByte();
                var soak = b.ReadUShortA();
                p.QueueHit(type, damage, (int)GameContext.LoopCycle);
                p.CurrentHealth = b.ReadUShortA();
                p.MaxHealth = b.ReadUShortA();
                p.EndCombatCycle = (int)GameContext.LoopCycle + 300;
            }
        }

        /// <summary>
        /// Performs player mask updates using data from the provided buffer.
        /// </summary>
        /// <param name="b">The buffer pointing to mask update data.</param>
        private void UpdatePlayerMasks(JagexBuffer b)
        {
            for (var i = 0; i < entityCount; i++)
            {
                var index = entityIndices[i];
                var p = GameContext.Players[index];

                var mask = b.ReadUByte();
                if ((mask & 0x40) != 0)
                {
                    mask += b.ReadUByte() << 8;
                }

                UpdatePlayerMask(mask, index, b, p);
            }
        }

        public void Handle(int opcode, JagexBuffer buf)
        {
            entityUpdateCount = 0;
            entityCount = 0;

            UpdateLocalPlayerMovement(buf);
            UpdateRemotePlayerMovement(buf);
            UpdateNewPlayers(buf);
            UpdatePlayerMasks(buf);

            for (var i = 0; i < entityUpdateCount; i++)
            {
                var playerIndex = entityUpdateIndices[i];
                if (GameContext.Players[playerIndex].UpdateCycle != GameContext.LoopCycle)
                {
                    GameContext.Players[playerIndex].Destroy();
                    GameContext.Players[playerIndex] = null;
                }
            }

            if (buf.Position() != buf.Array().Length)
            {
                Debug.Log("PLAYER UPDATING ERROR 69");
            }

            for (var i = 0; i < GameContext.PlayerCount; i++)
            {
                if (GameContext.Players[GameContext.PlayerIndices[i]] == null)
                {
                    Debug.Log("PLAYER UPDATING ERROR 777");
                }
            }
            
            GameContext.ReceivedPlayerUpdate = true;
        }
    }
}

