using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace RS
{
    public class ActorUpdatePacketHandler : PacketHandler
    {
        private int entityUpdateCount = 0;
        private int entityCount = 0;
        private int[] entityIndices = new int[2048];
        private int[] entityUpdateIndices = new int[2048];

        public void UpdateActorMasks(JagexBuffer b)
        {
            for (int i = 0; i < entityCount; i++)
            {
                Actor a = GameContext.Actors[entityIndices[i]];
                int mask = b.ReadUByte();

                if ((mask & 0x10) != 0)
                {
                    int SeqIndex = b.ReadLEUShort();

                    if (SeqIndex == 65535)
                    {
                        SeqIndex = -1;
                    }

                    int delay = b.ReadUByte();

                    if (SeqIndex == a.SeqIndex && SeqIndex != -1)
                    {
                        int type = GameContext.Cache.GetSeq(SeqIndex).Type;
                        if (type == 1)
                        {
                            a.SeqFrame = 0;
                            a.SeqCycle = 0;
                            a.SeqDelayCycle = delay;
                            a.SeqResetCycle = 0;
                        }
                        if (type == 2)
                        {
                            a.SeqResetCycle = 0;
                        }
                    }
                    else if (SeqIndex == -1 || a.SeqIndex == -1 || GameContext.Cache.GetSeq(SeqIndex).Priority >= GameContext.Cache.GetSeq(a.SeqIndex).Priority)
                    {
                        a.SeqIndex = SeqIndex;
                        a.SeqFrame = 0;
                        a.SeqCycle = 0;
                        a.SeqDelayCycle = delay;
                        a.SeqResetCycle = 0;
                        a.StillPathPosition = a.PathPosition;
                    }
                }

                if ((mask & 8) != 0)
                {
                    var damage = b.ReadUShortA();
                    var type = b.ReadUByteC();
                    var icon = b.ReadUByte();
                    a.QueueHit(type, damage, (int)GameContext.LoopCycle);
                    a.EndCombatCycle = (int)GameContext.LoopCycle + 300;
                    a.CurrentHealth = b.ReadUShortA();
                    a.MaxHealth = b.ReadUShortA();
                }

                if ((mask & 0x80) != 0)
                {
                    a.SpotAnimIndex = b.ReadUShort();

                    int info = b.ReadInt();
                    a.GraphicOffsetY = info >> 16;
                    a.SpotAnimCycleEnd = (int)GameContext.LoopCycle + (info & 0xFFFF);

                    a.SpotAnimFrame = 0;
                    a.SpotAnimCycle = 0;

                    if (a.SpotAnimCycleEnd > GameContext.LoopCycle)
                    {
                        a.SpotAnimFrame = -1;
                    }

                    if (a.SpotAnimIndex == 65535)
                    {
                        a.SpotAnimIndex = -1;
                    }
                }

                if ((mask & 0x20) != 0)
                {
                    a.FaceEntity = b.ReadUShort();

                    if (a.FaceEntity == 65535)
                    {
                        a.FaceEntity = -1;
                    }
                }

                if ((mask & 1) != 0)
                {
                    a.SpokenMessage = b.ReadString(10);
                    a.SpokenLife = 100;
                }

                if ((mask & 0x40) != 0)
                {
                    var damage = b.ReadUShortA();
                    var type = b.ReadUByteS();
                    var icon = b.ReadUByte();
                    a.QueueHit(type, damage, (int)GameContext.LoopCycle);
                    a.EndCombatCycle = (int)GameContext.LoopCycle + 300;
                    a.CurrentHealth = b.ReadUShortA();
                    a.MaxHealth = b.ReadUShortA();
                }

                if ((mask & 2) != 0)
                {
                    a.Config = GameContext.Cache.GetActorConfig(b.ReadLEUShortA());
                    a.TileSize = a.Config.HasOptions;
                    a.RotateSpeed = a.Config.TurnSpeed;
                    a.WalkAnimation = a.Config.MoveAnim;
                    a.Turn180Animation = a.Config.Turn180Anim;
                    a.TurnRightAnimation = a.Config.TurnRightAnim;
                    a.TurnLeftAnimation = a.Config.TurnLeftAnim;
                    a.StandAnimation = a.Config.StandAnim;
                }

                if ((mask & 4) != 0)
                {
                    a.FaceX = b.ReadLEUShort();
                    a.FaceY = b.ReadLEUShort();
                }
            }
        }

        public void UpdateActorMovement(JagexBuffer b)
        {
            b.BeginBitAccess();
            int actorCount = b.ReadBits(8);

            if (actorCount < GameContext.ActorCount)
            {
                for (int l = actorCount; l < GameContext.ActorCount; l++)
                {
                    entityUpdateIndices[entityUpdateCount++] = GameContext.ActorIndices[l];
                }
            }

            if (actorCount > GameContext.ActorCount)
            {
                Debug.Log("Actor count is too high");
                return;
            }

            GameContext.ActorCount = 0;

            for (int i = 0; i < actorCount; i++)
            {
                int actorIndex = GameContext.ActorIndices[i];
                Actor a = GameContext.Actors[actorIndex];
                int movement_update = b.ReadBits(1);

                if (movement_update == 0)
                {
                    GameContext.ActorIndices[GameContext.ActorCount++] = actorIndex;
                    a.UpdateCycle = (int)GameContext.LoopCycle;
                }
                else
                {
                    int moveType = b.ReadBits(2);

                    switch (moveType)
                    {
                        case 0:
                            {
                                GameContext.ActorIndices[GameContext.ActorCount++] = actorIndex;
                                a.UpdateCycle = (int)GameContext.LoopCycle;
                                entityIndices[entityCount++] = actorIndex;
                                break;
                            }
                        case 1:
                            {
                                GameContext.ActorIndices[GameContext.ActorCount++] = actorIndex;
                                a.UpdateCycle = (int)GameContext.LoopCycle;
                                a.QueueMove(b.ReadBits(3), false);

                                if (b.ReadBits(1) == 1)
                                {
                                    entityIndices[entityCount++] = actorIndex;
                                }
                                break;
                            }
                        case 2:
                            {
                                GameContext.ActorIndices[GameContext.ActorCount++] = actorIndex;
                                a.UpdateCycle = (int)GameContext.LoopCycle;
                                a.QueueMove(b.ReadBits(3), true);
                                a.QueueMove(b.ReadBits(3), true);

                                if (b.ReadBits(1) == 1)
                                {
                                    entityIndices[entityCount++] = actorIndex;
                                }
                                break;
                            }
                        case 3:
                            {
                                entityUpdateIndices[entityUpdateCount++] = actorIndex;
                                break;
                            }
                    }
                }
            }
        }

        public void HandleNewActors(JagexBuffer b)
        {
            while (b.BitPosition() + 21 < b.Capacity() * 8)
            {
                int actorIndex = b.ReadBits(14);
                if (actorIndex == 16383)
                {
                    break;
                }

                if (GameContext.Actors[actorIndex] == null)
                {
                    GameContext.Actors[actorIndex] = new Actor();
                }

                Actor a = GameContext.Actors[actorIndex];
                GameContext.ActorIndices[GameContext.ActorCount++] = actorIndex;
                a.ServerIndex = actorIndex;
                a.UpdateCycle = (int)GameContext.LoopCycle;

                int y = b.ReadBits(5);
                if (y > 15)
                {
                    y -= 32;
                }

                int x = b.ReadBits(5);
                if (x > 15)
                {
                    x -= 32;
                }

                int discardWalkQueue = b.ReadBits(1);

                a.Config = GameContext.Cache.GetActorConfig(b.ReadBits(18));
                if (b.ReadBits(1) == 1)
                {
                    entityIndices[entityCount++] = actorIndex;
                }

                a.TileSize = a.Config.HasOptions;
                a.RotateSpeed = a.Config.TurnSpeed;
                a.WalkAnimation = a.Config.MoveAnim;
                a.Turn180Animation = a.Config.Turn180Anim;
                a.TurnRightAnimation = a.Config.TurnRightAnim;
                a.TurnLeftAnimation = a.Config.TurnLeftAnim;
                a.StandAnimation = a.Config.StandAnim;
                a.TeleportTo(GameContext.Self.PathX[0] + x, GameContext.Self.PathY[0] + y, discardWalkQueue == 1);
            }

            b.EndBitAccess();
        }

        public void Handle(int opcode, JagexBuffer buf)
        {
            entityUpdateCount = 0;
            entityCount = 0;
            UpdateActorMovement(buf);
            HandleNewActors(buf);
            UpdateActorMasks(buf);

            for (int i = 0; i < entityUpdateCount; i++)
            {
                int index = entityUpdateIndices[i];

                if (GameContext.Actors[index].UpdateCycle != GameContext.LoopCycle)
                {
                    GameContext.Actors[index].Destroy();
                    GameContext.Actors[index].Config = null;
                    GameContext.Actors[index] = null;
                }
            }

            if (buf.Position() != buf.Capacity())
            {
                Debug.Log("ERROR ACTOR UPDATING 1");
            }

            for (int i = 0; i < GameContext.ActorCount; i++)
            {
                if (GameContext.Actors[GameContext.ActorIndices[i]] == null)
                {
                    Debug.Log("ERROR ACTOR UPDATING 2");
                }
            }
        }

    }
}
