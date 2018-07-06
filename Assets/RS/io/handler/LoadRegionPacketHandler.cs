using System;
using System.Diagnostics;

using UnityEngine;

namespace RS
{
    public class LoadRegionPacketHandler : PacketHandler
    {
        public void Handle(int opcode, JagexBuffer buf)
        {
            GameContext.Cache.ClearTempCaches();
            GameContext.DestroyProjectiles();

            GameContext.RegionX = GameContext.LoadedRegionX;
            GameContext.RegionY = GameContext.LoadedRegionY;
            GameContext.RegionX = buf.ReadUShortA();
            GameContext.RegionY = buf.ReadUShort();
            GameContext.LoadedRegionX = GameContext.RegionX;
            GameContext.LoadedRegionY = GameContext.RegionY;

            UnityEngine.Debug.Log("RX/Y: " + GameContext.RegionX + "/" + GameContext.RegionY);

            GameContext.MapBaseX = (GameContext.LoadedRegionX - 6) * 8;
            GameContext.MapBaseY = (GameContext.LoadedRegionY - 6) * 8;
            GameContext.RestrictRegion = false;

            if ((GameContext.LoadedRegionX / 8 == 48 || GameContext.LoadedRegionX / 8 == 49) && GameContext.LoadedRegionY / 8 == 48)
            {
                GameContext.RestrictRegion = true;
            }

            if (GameContext.LoadedRegionX / 8 == 48 && GameContext.LoadedRegionY / 8 == 148)
            {
                GameContext.RestrictRegion = true;
            }

            int count = 0;

            for (int chunkX = (GameContext.LoadedRegionX - 6) / 8; chunkX <= (GameContext.LoadedRegionX + 6) / 8; chunkX++)
            {
                for (int chunkY = (GameContext.LoadedRegionY - 6) / 8; chunkY <= (GameContext.LoadedRegionY + 6) / 8; chunkY++)
                {
                    count++;
                }
            }

            GameContext.ChunkLandscapePayload = new byte[count][];
            GameContext.ChunkObjectPayload = new byte[count][];
            GameContext.ChunkCoords = new int[count];
            GameContext.MapUids = new int[count];
            GameContext.LandscapeUids = new int[count];
            count = 0;

            for (int chunkX = (GameContext.LoadedRegionX - 6) / 8; chunkX <= (GameContext.LoadedRegionX + 6) / 8; chunkX++)
            {
                for (int chunkY = (GameContext.LoadedRegionY - 6) / 8; chunkY <= (GameContext.LoadedRegionY + 6) / 8; chunkY++)
                {
                    GameContext.ChunkCoords[count] = (chunkX << 8) + chunkY;

                    if (GameContext.RestrictRegion && (chunkY == 49 || chunkY == 149 || chunkY == 147 || chunkX == 50 || chunkX == 49 && chunkY == 47))
                    {
                        GameContext.MapUids[count] = -1;
                        GameContext.LandscapeUids[count] = -1;
                        count++;
                    }
                    else
                    {
                        GameContext.MapUids[count] = GameContext.Cache.GetMapId(chunkX, chunkY, 0);
                        if (GameContext.MapUids[count] != -1)
                        {
                            GameContext.ChunkLandscapePayload[count] = GameContext.Cache.ReadCompressed(4, GameContext.MapUids[count]);
                        }
                       
                        GameContext.LandscapeUids[count] = GameContext.Cache.GetMapId(chunkX, chunkY, 1);
                        if (GameContext.LandscapeUids[count] != -1)
                        {
                            GameContext.ChunkObjectPayload[count] = GameContext.Cache.ReadCompressed(4, GameContext.LandscapeUids[count]);
                        }

                        //UnityEngine.Debug.Log(GameContext.MapUids[count] + " " + GameContext.LandscapeUids[count]);
                        count++;
                    }
                }
            }

            int baseDeltaX = GameContext.MapBaseX - GameContext.LastMapBaseX;
            int baseDeltaY = GameContext.MapBaseY - GameContext.LastMapBaseY;
            GameContext.LastMapBaseX = GameContext.MapBaseX;
            GameContext.LastMapBaseY = GameContext.MapBaseY;

            for (int i = 0; i < 2048; i++)
            {
                Player p = GameContext.Players[i];
                if (p != null)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        p.PathX[j] -= baseDeltaX;
                        p.PathY[j] -= baseDeltaY;
                    }
                    p.SetSceneX(p.JSceneX - (baseDeltaX * 128));
                    p.SetSceneY(p.JSceneY - (baseDeltaY * 128));
                }
            }

            sbyte x1 = 0;
            sbyte x2 = 104;
            sbyte dx = 1;

            if (baseDeltaX < 0)
            {
                x1 = 103;
                x2 = -1;
                dx = -1;
            }

            sbyte y1 = 0;
            sbyte y2 = 104;
            sbyte dy = 1;

            if (baseDeltaY < 0)
            {
                y1 = 103;
                y2 = -1;
                dy = -1;
            }

            if (GameContext.GroundItems != null)
            {
                for (int x = x1; x != x2; x += dx)
                {
                    for (int y = y1; y != y2; y += dy)
                    {
                        int oldX = x + baseDeltaX;
                        int oldY = y + baseDeltaY;
                        for (int plane = 0; plane < 4; plane++)
                        {
                            if (oldX >= 0 && oldY >= 0 && oldX < 104 && oldY < 104)
                            {
                                GameContext.GroundItems[plane, x, y] = GameContext.GroundItems[plane, oldX, oldY];
                            }
                            else
                            {
                                GameContext.GroundItems[plane, x, y] = null;
                            }
                        }
                    }
                }
            }

            if (GameContext.MapMarkerX != 0)
            {
                GameContext.MapMarkerX -= baseDeltaX;
                GameContext.MapMarkerY -= baseDeltaY;
            }

            UnityEngine.Debug.Log("Flagging for load");
            GameContext.WaitingForScene = true;
            GameContext.ReceivedPlayerUpdate = false;
        }
    }
}

