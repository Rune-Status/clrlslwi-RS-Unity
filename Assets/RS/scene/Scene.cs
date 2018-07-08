using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;

namespace RS
{
    public class Scene
    {
        public static int PixelsPerTile = 128;
        
        public static int[] anIntArray463 = { 53, -53, -53, 53 };
        public static int[] anIntArray464 = { -53, -53, 53, 53 };
        public static int[] WallPaddingX = { -45, 45, 45, -45 };
        public static int[] WallPaddingZ = { 45, 45, -45, -45 };

        public static int[] WallRootDrawFlags = { 0x1, 0x2, 0x4, 0x8 };
        public static int[] WallExtDrawFlags = { 0x10, 0x20, 0x40, 0x80 };

        public static int[] RotXDelta = { 1, 0, -1, 0 };
        public static int[] RotYDelta = { 0, -1, 0, 1 };

        public int[,] MinimapOverlayMask = new int[,] {
            {
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0
            },
            {
                1, 1, 1, 1,
                1, 1, 1, 1,
                1, 1, 1, 1,
                1, 1, 1, 1
            },
            {
                1, 0, 0, 0,
                1, 1, 0, 0,
                1, 1, 1, 0,
                1, 1, 1, 1
            },
            {
                1, 1, 0, 0,
                1, 1, 0, 0,
                1, 0, 0, 0,
                1, 0, 0, 0
            },
            {
                0, 0, 1, 1,
                0, 0, 1, 1,
                0, 0, 0, 1,
                0, 0, 0, 1
            },
            {
                0, 1, 1, 1,
                0, 1, 1, 1,
                1, 1, 1, 1,
                1, 1, 1, 1
            },
            {
                1, 1, 1, 0,
                1, 1, 1, 0,
                1, 1, 1, 1,
                1, 1, 1, 1
            },
            {
                1, 1, 0, 0,
                1, 1, 0, 0,
                1, 1, 0, 0,
                1, 1, 0, 0
            },
            {
                0, 0, 0, 0,
                0, 0, 0, 0,
                1, 0, 0, 0,
                1, 1, 0, 0
            },
            {
                1, 1, 1, 1,
                1, 1, 1, 1,
                0, 1, 1, 1,
                0, 0, 1, 1
            },
            {
                1, 1, 1, 1,
                1, 1, 0, 0,
                1, 0, 0, 0,
                1, 0, 0, 0
            },
            {
                0, 0, 0, 0,
                0, 0, 1, 1,
                0, 1, 1, 1,
                0, 1, 1, 1
            },
            {
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 1, 1, 0,
                1, 1, 1, 1
            }
        };

        public int[,] MinimapOverlayMaskRotation = new int[,] {
            {
                0, 1, 2, 3,
                4, 5, 6, 7,
                8, 9, 10, 11,
                12, 13, 14, 15
            },
            {
                12, 8, 4, 0,
                13, 9, 5, 1,
                14, 10, 6, 2,
                15, 11, 7, 3
            },
            {
                15, 14, 13, 12,
                11, 10, 9, 8,
                7, 6, 5, 4,
                3, 2, 1, 0
            },
            {
                3, 7, 11, 15,
                2, 6, 10, 14,
                1, 5, 9, 13,
                0, 4, 8, 12
            }
        };

        public static string[] RegularTreePrefabs =
        {
            "Tree9_2",
            "Tree9_3",
            "Tree9_4",
            "Tree9_5",
        };

        public static int[] RegularTreeIds =
        {
            1276, 1277, 1278, 1279, 1280, 1281,
        };

        public static string[] MapleTreePrefabs =
        {
            "Tree_maple",
        };

        public static int[] MapleTreeIds =
        {
            1307,
        };

        public static GameObject CreateTree()
        {
            var rand = new System.Random();
            return (GameObject)GameObject.Instantiate(Resources.Load(RegularTreePrefabs[rand.Next(RegularTreePrefabs.Length)]));
        }

        public static GameObject CreateMaple()
        {
            return (GameObject)GameObject.Instantiate(Resources.Load(MapleTreePrefabs[0]));
        }

        private int[,,] heightMap;
        private int[,,] renderFlags;

        private byte[,,] overlayFloorIndex = new byte[4, 104, 104];
        private byte[,,] overlayRotation = new byte[4, 104, 104];
        private byte[,,] overlayShape = new byte[4, 104, 104];

        private byte[,,] underlayFloorIndex = new byte[4, 104, 104];

        private SceneTile[,,] tiles = new SceneTile[4, 104, 104];

        private List<GameObject> allObjects = new List<GameObject>();
        private List<GameObject> animatedObjects = new List<GameObject>();

        public int PlaneAtBuild = 0;

        private Texture2D textureMap;

        public Scene(int[,,] heightMap, int[,,] renderFlags, Cache cache)
        {
            this.heightMap = heightMap;
            this.renderFlags = renderFlags;
            InitTiles();
        }

        public void DrawMinimapTile(int[] pixels, int start, int width, int plane, int x, int y)
        {
            var tile = tiles[plane, x, y];
            if (tile == null) return;

            var ut = tile.Underlay;
            if (ut != null)
            {
                var rgb = ut.UnderlayMinimapRgb;
                if (rgb == 0)
                {
                    return;
                }

                for (var i = 0; i < 4; i++)
                {
                    pixels[start] = rgb;
                    pixels[start + 1] = rgb;
                    pixels[start + 2] = rgb;
                    pixels[start + 3] = rgb;
                    start += width;
                }
            }

            var ot = tile.Overlay;
            if (ot != null)
            {
                var rgb = ot.UnderlayMinimapRgb;
                var rgb2 = ot.OverlayMinimapRgb;

                var i = 0;
                if (rgb != 0)
                {
                    for (var j = 0; j < 4; j++)
                    {
                        pixels[start] = MinimapOverlayMask[ot.Shape, MinimapOverlayMaskRotation[ot.Rotation, i++]] != 0 ? rgb2 : rgb;
                        pixels[start + 1] = MinimapOverlayMask[ot.Shape, MinimapOverlayMaskRotation[ot.Rotation, i++]] != 0 ? rgb2 : rgb;
                        pixels[start + 2] = MinimapOverlayMask[ot.Shape, MinimapOverlayMaskRotation[ot.Rotation, i++]] != 0 ? rgb2 : rgb;
                        pixels[start + 3] = MinimapOverlayMask[ot.Shape, MinimapOverlayMaskRotation[ot.Rotation, i++]] != 0 ? rgb2 : rgb;
                        start += width;
                    }
                    return;
                }

                for (var j = 0; j < 4; j++)
                {
                    if (MinimapOverlayMask[ot.Shape, MinimapOverlayMaskRotation[ot.Rotation, i++]] != 0)
                    {
                        pixels[start] = rgb;
                    }

                    if (MinimapOverlayMask[ot.Shape, MinimapOverlayMaskRotation[ot.Rotation, i++]] != 0)
                    {
                        pixels[start + 1] = rgb;
                    }

                    if (MinimapOverlayMask[ot.Shape, MinimapOverlayMaskRotation[ot.Rotation, i++]] != 0)
                    {
                        pixels[start + 2] = rgb;
                    }

                    if (MinimapOverlayMask[ot.Shape, MinimapOverlayMaskRotation[ot.Rotation, i++]] != 0)
                    {
                        pixels[start + 3] = rgb;
                    }

                    start += width;
                }
            }
        }

        public int GetModifiedPlane(int x, int y, int plane)
        {
            if ((renderFlags[plane, x, y] & 0x8) != 0)
            {
                return 0;
            }
            if (plane > 0 && (renderFlags[1, x, y] & 0x2) != 0)
            {
                return plane - 1;
            }
            return plane;
        }

        public void DestroySceneObjects()
        {
            var tmp = new List<GameObject>();
            foreach (var obj in allObjects)
            {
                GameObject.Destroy(obj);
                tmp.Add(obj);
            }

            foreach (var obj in tmp)
            {
                RemoveAsSceneObject(obj);
            }
        }

        public void AddAsSceneObject(GameObject obj)
        {
            allObjects.Add(obj);

            var interactiveComp = obj.GetComponent<InteractiveComponent>();
            if (interactiveComp != null && interactiveComp.GameObject.IsAnimatedObject)
            {
                animatedObjects.Add(obj);
            }

            var wallDecoComp = obj.GetComponent<WallDecorationComponent>();
            if (wallDecoComp != null && wallDecoComp.GameObject.IsAnimatedObject)
            {
                animatedObjects.Add(obj);
            }
        }

        public void RemoveAsSceneObject(GameObject obj)
        {
            allObjects.Remove(obj);

            var interactiveComp = obj.GetComponent<InteractiveComponent>();
            if (interactiveComp != null && interactiveComp.GameObject.IsAnimatedObject)
            {
                animatedObjects.Remove(obj);
            }

            var wallDecoComp = obj.GetComponent<WallDecorationComponent>();
            if (wallDecoComp != null && wallDecoComp.GameObject.IsAnimatedObject)
            {
                animatedObjects.Remove(obj);
            }
        }

        public bool AddInteractiveObject(object o, int sceneX, int sceneY, int sceneZ, int plane, int startTileX, int startTileY, int tileWidth, int tileHeight, long uid, byte arrangement, int rotation)
        {
            for (int x = startTileX; x < startTileX + tileWidth; x++)
            {
                for (int y = startTileY; y < startTileY + tileHeight; y++)
                {
                    if (x < 0 || y < 0 || x >= 104 || y >= 104)
                    {
                        return false;
                    }

                    SceneTile t = tiles[plane, x, y];
                    if (t != null && t.InteractiveCount >= 5)
                    {
                        return false;
                    }
                }
            }

            int endTileX = (startTileX + tileWidth) - 1;
            int endTileY = (startTileY + tileHeight) - 1;
            InteractiveObject obj = new InteractiveObject(arrangement, o, plane, rotation, sceneX, sceneY, sceneZ, endTileX, endTileY, uid, startTileX, startTileY);

            for (int x = startTileX; x < startTileX + tileWidth; x++)
            {
                for (int y = startTileY; y < startTileY + tileHeight; y++)
                {
                    int flag = 0;

                    if (x > startTileX)
                    {
                        flag++;
                    }

                    if (x < (startTileX + tileWidth) - 1)
                    {
                        flag += 4;
                    }

                    if (y > startTileY)
                    {
                        flag += 8;
                    }

                    if (y < (startTileY + tileHeight) - 1)
                    {
                        flag += 2;
                    }

                    for (int z = plane; z >= 0; z--)
                    {
                        if (tiles[z, x, y] == null)
                        {
                            tiles[z, x, y] = new SceneTile(x, y, z);
                        }
                    }

                    SceneTile t = tiles[plane, x, y];
                    t.Interactives[t.InteractiveCount] = obj;
                    t.Flags |= flag;
                    t.InteractiveCount++;
                }
            }

            return true;
        }

        public bool AddInteractiveObject(object o, int tileX, int tileY, int plane, int tileSizeX, int tileSizeY, int sceneY, byte arrangement, int angle, long uid)
        {
            int sceneX = (tileX * 128 + 64 * tileSizeY);
            int sceneZ = (tileY * 128 + 64 * tileSizeX);
            return AddInteractiveObject(o, sceneX, sceneY, sceneZ, plane, tileX, tileY, tileSizeX, tileSizeY, uid, arrangement, angle);
        }

        public void InitTilesAtPos(int x, int y, int planeOff)
        {
            for (int z = planeOff; z >= 0; z--)
            {
                if (tiles[z, x, y] == null)
                {
                    tiles[z, x, y] = new SceneTile(z, x, y);
                }
            }
        }

        public void AddWallObject(Model root, Model extension, int tileX, int tileY, int sceneY, int plane, int rotationFlag, byte arrangement, long uniqueId)
        {
            var sceneX = (tileX * 128 + 64);
            var sceneZ = (tileY * 128 + 64);
            WallObject wl = new WallObject(arrangement, extension, root, rotationFlag, sceneX, sceneY, sceneZ, uniqueId);
            
            InitTilesAtPos(tileX, tileY, plane);
            tiles[plane, tileX, tileY].Wall = wl;
        }

        public void AddWallDecoration(object root, int tileX, int tileY, int plane, int offsetX, int offsetY, int sceneY, int rotation, int arrangement, int flags, long uniqueId)
        {
            var sceneX = (tileX * 128 + 64) + offsetX;
            var sceneZ = (tileY * 128 + 64) + offsetY;
            var wd = new WallDecoration(arrangement, flags, root, rotation, uniqueId, sceneX, sceneY, sceneZ);

            InitTilesAtPos(tileX, tileY, plane);
            tiles[plane, tileX, tileY].WallDeco = wd;
        }

        public void AddGroundDecoration(Model root, int tileX, int tileY, int plane, int sceneY, long uniqueId, int arrangement)
        {
            var sceneX = (tileX * 128 + 64);
            var sceneZ = (tileY * 128 + 64);
            var wd = new GroundDecoration(arrangement, root, sceneX, sceneY, sceneZ, uniqueId);

            InitTilesAtPos(tileX, tileY, plane);
            tiles[plane, tileX, tileY].GroundDeco = wd;
        }

        public long GetWallUniqueId(int plane, int x, int y)
        {
            var t = tiles[plane, x, y];
            if (t == null || t.Wall == null)
            {
                return 0;
            }

            return t.Wall.UniqueId;
        }

        public long GetGroundDecorationUniqueId(int plane, int x, int y)
        {
            var t = tiles[plane, x, y];
            if (t == null || t.GroundDeco == null)
            {
                return 0;
            }

            return t.GroundDeco.UniqueId;
        }

        public long GetInteractiveUniqueId(int plane, int x, int y)
        {
            var t = tiles[plane, x, y];
            if (t == null)
            {
                return 0;
            }

            for (int l = 0; l < t.InteractiveCount; l++)
            {
                var sl = t.Interactives[l];
                if (((sl.UniqueId >> 61) & 3) == 2 && sl.StartTileX == x && sl.StartTileY == y)
                {
                    return sl.UniqueId;
                }
            }
            return 0;
        }

        public int GetArrangement(int plane, int x, int y, long uid)
        {
            var t = tiles[plane, x, y];
            if (t == null)
            {
                return -1;
            }

            if (t.Wall != null && t.Wall.UniqueId == uid)
            {
                return t.Wall.RotationType & 0xff;
            }

            if (t.WallDeco != null && t.WallDeco.UniqueId == uid)
            {
                return t.WallDeco.Arrangement & 0xff;
            }

            if (t.GroundDeco != null && t.GroundDeco.UniqueId == uid)
            {
                return t.GroundDeco.Arrangement & 0xff;
            }

            for (int i = 0; i < t.InteractiveCount; i++)
            {
                if (t.Interactives[i].UniqueId == uid)
                {
                    return t.Interactives[i].Arrangement & 0xff;
                }
            }
            return -1;
        }

        public void AddObject(CollisionMap map, int objectIndex, int type, int x, int y, int plane, int rotation)
        {
            var valid = true; // false;
            if ((renderFlags[0, x, y] & 0x2) != 0)
            {
                valid = true;
            }
            else if ((renderFlags[plane, x, y] & 0x10) == 0 && GetModifiedPlane(x, y, plane) == PlaneAtBuild)
            {
                valid = true;
            }

            if (!valid)
            {
                return;
            }

            int southWestHeight = heightMap[plane, x, y];
            int southEastHeight = heightMap[plane, x + 1, y];
            int northEastHeight = heightMap[plane, x + 1, y + 1];
            int northWestHeight = heightMap[plane, x, y + 1];
            int averageHeight = (southWestHeight + southEastHeight + northEastHeight + northWestHeight) >> 2;

            ObjectConfig desc = GameContext.Cache.GetObjectConfig(objectIndex);
            if (desc == null)
            {
                return;
            }

            long uid = 0x4000000000000000 | (((long)objectIndex) << 14) | (((long)y) << 7) | (long)x;
            if (!desc.isStatic)
            {
                uid += long.MinValue;
            }

            byte arrangement = (byte)((rotation << 6) + type);

            if (type == 22)
            {
                Model m = desc.GetModel(22, rotation, southWestHeight, southEastHeight, northEastHeight, northWestHeight);
                AddGroundDecoration(m, x, y, plane, averageHeight, uid, arrangement);

                if (desc.hasCollisions && desc.isStatic && map != null)
                {
                    map.SetSolid(x, y);
                }
            }
            else if (type == 10 || type == 11)
            {
                int angle = 0;
                if (type == 11)
                {
                    angle += 256;
                }

                int sizeY;
                int sizeX;

                if (rotation == 1 || rotation == 3)
                {
                    sizeY = desc.sizeY;
                    sizeX = desc.sizeX;
                }
                else
                {
                    sizeY = desc.sizeX;
                    sizeX = desc.sizeY;
                }

                object o = null;
                if (desc.seqIndex == -1)
                {
                    o = desc.GetModel(10, rotation, southWestHeight, southEastHeight, northEastHeight, northWestHeight);
                }
                else
                {
                    o = new AnimatedObject(objectIndex, rotation, 10, southEastHeight, northEastHeight, southWestHeight, northWestHeight, desc.seqIndex, true);
                }


                AddInteractiveObject(o, x, y, plane, sizeX, sizeY, averageHeight, arrangement, angle, uid);

                if (desc.hasCollisions && map != null)
                {
                    map.AddObject(x, y, desc.sizeX, desc.sizeY, rotation, desc.blocksProjectiles);
                }
            }
            else if (type >= 12)
            {
                object o = null;
                if (desc.seqIndex == -1)
                {
                    o = desc.GetModel(type, rotation, southWestHeight, southEastHeight, northEastHeight, northWestHeight);
                }
                else
                {
                    o = new AnimatedObject(objectIndex, rotation, type, southEastHeight, northEastHeight, southWestHeight, northWestHeight, desc.seqIndex, true);
                }

                AddInteractiveObject(o, x, y, plane, 1, 1, averageHeight, arrangement, 0, uid);

                if (map != null)
                {
                    map.AddObject(x, y, desc.sizeX, desc.sizeY, rotation, desc.blocksProjectiles);
                }
            }
            else if (type == 0)
            {
                Model m = desc.GetModel(0, rotation, southWestHeight, southEastHeight, northEastHeight, northWestHeight);
                AddWallObject(m, null, x, y, averageHeight, plane, WallRootDrawFlags[rotation], arrangement, uid);

                if (desc.hasCollisions && map != null)
                {
                    map.AddWall(x, y, type, rotation, desc.blocksProjectiles);
                }
            }
            else if (type == 1)
            {
                Model m = desc.GetModel(1, rotation, southWestHeight, southEastHeight, northEastHeight, northWestHeight);
                AddWallObject(m, null, x, y, averageHeight, plane, WallExtDrawFlags[rotation], arrangement, uid);

                if (desc.hasCollisions && map != null)
                {
                    map.AddWall(x, y, type, rotation, desc.blocksProjectiles);
                }
            }
            else if (type == 2)
            {
                int nextRotation = rotation + 1 & 0x3;
                Model m = desc.GetModel(2, 4 + rotation, southWestHeight, southEastHeight, northEastHeight, northWestHeight);
                Model m2 = desc.GetModel(2, nextRotation, southWestHeight, southEastHeight, northEastHeight, northWestHeight);
                AddWallObject(m, m2, x, y, averageHeight, plane, WallRootDrawFlags[rotation], arrangement, uid);

                if (desc.hasCollisions && map != null)
                {
                    map.AddWall(x, y, type, rotation, desc.blocksProjectiles);
                }
            }
            else if (type == 3)
            {
                Model m = desc.GetModel(3, rotation, southWestHeight, southEastHeight, northEastHeight, northWestHeight);
                AddWallObject(m, null, x, y, averageHeight, plane, WallRootDrawFlags[rotation], arrangement, uid);

                if (desc.hasCollisions && map != null)
                {
                    map.AddWall(x, y, type, rotation, desc.blocksProjectiles);
                }
            }
            else if (type == 9)
            {
                object o = null;
                if (desc.seqIndex == -1)
                {
                    o = desc.GetModel(9, rotation, southWestHeight, southEastHeight, northEastHeight, northWestHeight);
                }
                else
                {
                    o = new AnimatedObject(objectIndex, rotation, 9, southEastHeight, northEastHeight, southWestHeight, northWestHeight, desc.seqIndex, true);
                }

                AddInteractiveObject(o, x, y, plane, 1, 1, averageHeight, arrangement, 0, uid);

                if (desc.hasCollisions && map != null)
                {
                    map.AddObject(x, y, desc.sizeX, desc.sizeY, rotation, desc.blocksProjectiles);
                }
            }
            else if (type == 4)
            {
                object o = null;
                if (desc.seqIndex == -1)
                {
                    o = desc.GetModel(4, 0, southWestHeight, southEastHeight, northEastHeight, northWestHeight);
                }
                else
                {
                    o = new AnimatedObject(objectIndex, 0, 4, southEastHeight, northEastHeight, southWestHeight, northWestHeight, desc.seqIndex, true);
                }
                
                AddWallDecoration(o, x, y, plane, 0, 0, averageHeight, rotation * 512, arrangement, 0, uid);
            }
            else if (type == 5)
            {
                int width = 16;

                object o = null;
                if (desc.seqIndex == -1)
                {
                    o = desc.GetModel(4, 0, southWestHeight, southEastHeight, northEastHeight, northWestHeight);
                }
                else
                {
                    o = new AnimatedObject(objectIndex, 0, 4, southEastHeight, northEastHeight, southWestHeight, northWestHeight, desc.seqIndex, true);
                }
                
                AddWallDecoration(o, x, y, plane, RotXDelta[rotation] * width, RotYDelta[rotation] * width, averageHeight, rotation * 512, arrangement, 0, uid);
            }
            else if (type == 6)
            {
                object o = null;
                if (desc.seqIndex == -1)
                {
                    o = desc.GetModel(4, 0, southWestHeight, southEastHeight, northEastHeight, northWestHeight);
                }
                else
                {
                    o = new AnimatedObject(objectIndex, 0, 4, southEastHeight, northEastHeight, southWestHeight, northWestHeight, desc.seqIndex, true);
                }
                
                AddWallDecoration(o, x, y, plane, 0, 0, averageHeight, rotation, arrangement, 0x100, uid);
            }
            else if (type == 7)
            {
                object o = null;
                if (desc.seqIndex == -1)
                {
                    o = desc.GetModel(4, 0, southWestHeight, southEastHeight, northEastHeight, northWestHeight);
                }
                else
                {
                    o = new AnimatedObject(objectIndex, 0, 4, southEastHeight, northEastHeight, southWestHeight, northWestHeight, desc.seqIndex, true);
                }
                
                AddWallDecoration(o, x, y, plane, 0, 0, averageHeight, rotation, arrangement, 0x200, uid);
            }
            else if (type == 8)
            {
                object o = null;
                if (desc.seqIndex == -1)
                {
                    o = desc.GetModel(4, 0, southWestHeight, southEastHeight, northEastHeight, northWestHeight);
                }
                else
                {
                    o = new AnimatedObject(objectIndex, 0, 4, southEastHeight, northEastHeight, southWestHeight, northWestHeight, desc.seqIndex, true);
                }

                AddWallDecoration(o, x, y, plane, 0, 0, averageHeight, rotation, arrangement, 0x300, uid);
            } else
            {
                Debug.Log("unknown type " + type);
            }
        }

        public void InitTiles()
        {
            overlayFloorIndex = new byte[4, 104, 104];
            overlayRotation = new byte[4, 104, 104];
            overlayShape = new byte[4, 104, 104];

            underlayFloorIndex = new byte[4, 104, 104];

            tiles = new SceneTile[4, 104, 104];
            for (int plane = 0; plane < 4; plane++)
            {
                for (int x = 0; x < 104; x++)
                {
                    for (int y = 0; y < 104; y++)
                    {
                        tiles[plane, x, y] = new SceneTile(x, y, plane);
                    }
                }
            }
        }

        public void AddTile(
            int plane, int x, int y, int shape, int rotation,
            int textureIndex, int textureIndex2,
            int southWestHeight, int southEastHeight, int northEastHeight, int northWestHeight,
            int underlaySwHsl, int underlaySeHsl, int underlayNeHsl, int underlayNwHsl,
            int overlaySwHsl, int overlaySeHsl, int overlayNeHsl, int overlayNwHsl,
            int underlayMinimapRgb, int overlayMinimapRgb)
        {
            if (shape == 0)
            {
                UnderlayTile t = new UnderlayTile(underlaySwHsl, underlaySeHsl, underlayNeHsl, underlayNwHsl, textureIndex2, underlayMinimapRgb, false);

                for (int z = plane; z >= 0; z--)
                {
                    if (tiles[z, x, y] == null)
                    {
                        tiles[z, x, y] = new SceneTile(x, y, z);
                    }
                }

                tiles[plane, x, y].Underlay = t;
            }
            else if (shape == 1)
            {
                UnderlayTile t = new UnderlayTile(overlaySwHsl, overlaySeHsl, overlayNeHsl, overlayNwHsl, textureIndex, overlayMinimapRgb, false);

                for (int z = plane; z >= 0; z--)
                {
                    if (tiles[z, x, y] == null)
                    {
                        tiles[z, x, y] = new SceneTile(x, y, z);
                    }
                }

                tiles[plane, x, y].Underlay = t;
            }
            else
            {
                OverlayTile t = new OverlayTile(
                    x, y,
                    southWestHeight, southEastHeight, northEastHeight, northWestHeight,
                    overlaySwHsl, overlaySeHsl, overlayNeHsl, overlayNwHsl,
                    underlaySwHsl, underlaySeHsl, underlayNeHsl, underlayNwHsl,
                    underlayMinimapRgb, overlayMinimapRgb, textureIndex, textureIndex2, rotation, shape);

                for (int z = plane; z >= 0; z--)
                {
                    if (tiles[z, x, y] == null)
                    {
                        tiles[z, x, y] = new SceneTile(x, y, z);
                    }
                }

                tiles[plane, x, y].Overlay = t;
            }
        }

        public void AddBridge(int x, int y)
        {
            SceneTile bridgeTile = tiles[0, x, y];

            for (int z = 0; z < 3; z++)
            {
                SceneTile t = tiles[z, x, y] = tiles[z + 1, x, y];
                if (t != null)
                {
                    for (int i = 0; i < t.InteractiveCount; i++)
                    {
                        InteractiveObject sl = t.Interactives[i];
                        if ((sl.UniqueId >> 61 & 3) == 2 && sl.StartTileX == x && sl.StartTileY == y)
                        {
                            sl.Plane--;
                        }
                    }

                }
            }

            if (tiles[0, x, y] == null)
            {
                tiles[0, x, y] = new SceneTile(x, y, 0);
            }

            tiles[0, x, y].Bridge = bridgeTile;
            tiles[3, x, y] = null;
        }

        private int[,,] underlayHsls = new int[4,104,104];

        public void CalcColors()
        {
            var hueRandomizer = 2;
            var lightnessRandomizer = 2;

            var blendedHue = new int[104];
            var blendedSaturation = new int[104];
            var blendedHueDivisor = new int[104];
            var blendedDirection = new int[104];
            var blendedBrightness = new int[104];
            var lightMap = new int[104, 104];

            for (int plane = 0; plane < 4; plane++)
            {
                int initialLightIntensity = 96;
                int specular_factor = 768;
                int lightX = -50;
                int lightZ = -10;
                int lightY = -50;
                int lightLength = (int)Math.Sqrt((double)(lightX * lightX + lightZ * lightZ + lightY * lightY));
                int specularDistribution = specular_factor * lightLength >> 8;

                for (int y = 1; y < 104 - 1; y++)
                {
                    for (int x = 1; x < 104 - 1; x++)
                    {
                        int xHeightDiff = (heightMap[plane, x + 1, y] - heightMap[plane, x - 1, y]);
                        int yHeightDiff = (heightMap[plane, x, y + 1] - heightMap[plane, x, y - 1]);
                        int normalLength = (int)Math.Sqrt((double)(xHeightDiff * xHeightDiff + 65536 + yHeightDiff * yHeightDiff));
                        int normalizedX = (xHeightDiff << 8) / normalLength;
                        int normalizedZ = 65536 / normalLength;
                        int normalizedY = (yHeightDiff << 8) / normalLength;
                        int intensity = initialLightIntensity + (lightX * normalizedX + lightZ * normalizedZ + lightY * normalizedY) / specularDistribution;
                        int weightedShadowIntensity = ((0 >> 2) + (0 >> 3) + (0 >> 2) + (0 >> 3) + (0 >> 1));
                        lightMap[x, y] = intensity - weightedShadowIntensity;
                    }
                }

                for (var y = 0; y < 104; y++)
                {
                    blendedHue[y] = 0;
                    blendedSaturation[y] = 0;
                    blendedHueDivisor[y] = 0;
                    blendedDirection[y] = 0;
                    blendedBrightness[y] = 0;
                }

                for (int x = -5; x < 109; x++)
                {
                    for (int y = 0; y < 104; y++)
                    {
                        int positiveOffsetX = x + 5;
                        if (positiveOffsetX >= 0 && positiveOffsetX < 104)
                        {
                            int index = underlayFloorIndex[plane, positiveOffsetX, y] & 0xFF;
                            if (index > 0)
                            {
                                var floor = GameContext.Cache.GetFloorConfig(index - 1);
                                blendedHue[y] += floor.Hue;
                                blendedSaturation[y] += floor.Saturation;
                                blendedBrightness[y] += floor.Lightness;
                                blendedHueDivisor[y] += floor.HueDivisor;
                                blendedDirection[y]++;
                            }
                        }

                        int negativeOffsetX = x - 5;
                        if (negativeOffsetX >= 0 && negativeOffsetX < 104)
                        {
                            int floorIndex = underlayFloorIndex[plane, negativeOffsetX, y] & 0xFF;
                            if (floorIndex > 0)
                            {
                                var f = GameContext.Cache.GetFloorConfig(floorIndex - 1);
                                blendedHue[y] -= f.Hue;
                                blendedSaturation[y] -= f.Saturation;
                                blendedBrightness[y] -= f.Lightness;
                                blendedHueDivisor[y] -= f.HueDivisor;
                                blendedDirection[y]--;
                            }
                        }
                    }

                    if (x >= 1 && x < 104 - 1)
                    {
                        int curBlendedHue = 0;
                        int curBlendedSaturation = 0;
                        int curBlendedBrightness = 0;
                        int curHueDivisor = 0;
                        int curDivisor = 0;

                        for (int y = -5; y < 104 + 5; y++)
                        {
                            int positive = y + 5;
                            if (positive >= 0 && positive < 104)
                            {
                                curBlendedHue += blendedHue[positive];
                                curBlendedSaturation += blendedSaturation[positive];
                                curBlendedBrightness += blendedBrightness[positive];
                                curHueDivisor += blendedHueDivisor[positive];
                                curDivisor += blendedDirection[positive];
                            }

                            int negative = y - 5;
                            if (negative >= 0 && negative < 104)
                            {
                                curBlendedHue -= blendedHue[negative];
                                curBlendedSaturation -= blendedSaturation[negative];
                                curBlendedBrightness -= blendedBrightness[negative];
                                curHueDivisor -= blendedHueDivisor[negative];
                                curDivisor -= blendedDirection[negative];
                            }

                            if (y >= 1 && y < 104 - 1)
                            {
                                int underlayId = underlayFloorIndex[plane, x, y] & 0xFF;
                                int overlayId = overlayFloorIndex[plane, x, y] & 0xFF;

                                if (underlayId > 0 || overlayId > 0)
                                {
                                    int southWestHeight = heightMap[plane, x, y];
                                    int southEastHeight = heightMap[plane, x + 1, y];
                                    int northEastHeight = heightMap[plane, x + 1, y + 1];
                                    int northWestHeight = heightMap[plane, x, y + 1];
                                    int lightSouthWest = lightMap[x, y];
                                    int lightSouthEast = lightMap[x + 1, y];
                                    int lightNorthEast = lightMap[x + 1, y + 1];
                                    int lightNorthWest = lightMap[x, y + 1];

                                    int underlayHsl = -1;
                                    int underlayMinimapHsl = -1;
                                    int underlayTex = -1;

                                    if (underlayId > 0)
                                    {
                                        int hue = curBlendedHue * 256 / curHueDivisor;
                                        int saturation = curBlendedSaturation / curDivisor;
                                        int brightness = curBlendedBrightness / curDivisor;
                                        underlayHsl = ColorUtils.TrimHSL(hue, saturation, brightness);
                                        hue = hue + hueRandomizer & 0xFF;
                                        brightness += lightnessRandomizer;

                                        if (brightness < 0)
                                        {
                                            brightness = 0;
                                        }
                                        else if (brightness > 255)
                                        {
                                            brightness = 255;
                                        }

                                        underlayMinimapHsl = ColorUtils.TrimHSL(hue, saturation, brightness);
                                    }

                                    underlayHsls[plane, x, y] = underlayHsl;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Apply(CollisionMap[] cm)
        {
            for (int plane = 0; plane < 4; plane++)
            {
                for (int x = 0; x < 104; x++)
                {
                    for (int y = 0; y < 104; y++)
                    {
                        if ((renderFlags[plane, x, y] & 0x1) == 1)
                        {
                            int tmp = plane;
                            if ((renderFlags[1, x, y] & 0x2) == 2)
                            {
                                tmp--;
                            }

                            if (tmp >= 0)
                            {
                                cm[tmp].SetSolid(x, y);
                            }
                        }
                    }
                }
            }

            var hueRandomizer = 2;
            var lightnessRandomizer = 2;

            var blendedHue = new int[104];
            var blendedSaturation = new int[104];
            var blendedHueDivisor = new int[104];
            var blendedDirection = new int[104];
            var blendedBrightness = new int[104];
            var lightMap = new int[104, 104];

            for (int plane = 0; plane < 4; plane++)
            {
                int initialLightIntensity = 96;
                int specular_factor = 768;
                int lightX = -50;
                int lightZ = -10;
                int lightY = -50;
                int lightLength = (int)Math.Sqrt((double)(lightX * lightX + lightZ * lightZ + lightY * lightY));
                int specularDistribution = specular_factor * lightLength >> 8;

                for (int y = 1; y < 104 - 1; y++)
                {
                    for (int x = 1; x < 104 - 1; x++)
                    {
                        int xHeightDiff = (heightMap[plane, x + 1, y] - heightMap[plane, x - 1, y]);
                        int yHeightDiff = (heightMap[plane, x, y + 1] - heightMap[plane, x, y - 1]);
                        int normalLength = (int)Math.Sqrt((double)(xHeightDiff * xHeightDiff + 65536 + yHeightDiff * yHeightDiff));
                        int normalizedX = (xHeightDiff << 8) / normalLength;
                        int normalizedZ = 65536 / normalLength;
                        int normalizedY = (yHeightDiff << 8) / normalLength;
                        int intensity = initialLightIntensity + (lightX * normalizedX + lightZ * normalizedZ + lightY * normalizedY) / specularDistribution;
                        int weightedShadowIntensity = ((0 >> 2) + (0 >> 3) + (0 >> 2) + (0 >> 3) + (0 >> 1));
                        lightMap[x, y] = intensity - weightedShadowIntensity;
                    }
                }

                for (var y = 0; y < 104; y++)
                {
                    blendedHue[y] = 0;
                    blendedSaturation[y] = 0;
                    blendedHueDivisor[y] = 0;
                    blendedDirection[y] = 0;
                    blendedBrightness[y] = 0;
                }

                for (int x = -5; x < 109; x++)
                {
                    for (int y = 0; y < 104; y++)
                    {
                        int positiveOffsetX = x + 5;
                        if (positiveOffsetX >= 0 && positiveOffsetX < 104)
                        {
                            int index = underlayFloorIndex[plane,positiveOffsetX,y] & 0xFF;
                            if (index > 0)
                            {
                                var floor = GameContext.Cache.GetFloorConfig(index - 1);
                                blendedHue[y] += floor.Hue;
                                blendedSaturation[y] += floor.Saturation;
                                blendedBrightness[y] += floor.Lightness;
                                blendedHueDivisor[y] += floor.HueDivisor;
                                blendedDirection[y]++;
                            }
                        }

                        int negativeOffsetX = x - 5;
                        if (negativeOffsetX >= 0 && negativeOffsetX < 104)
                        {
                            int flo_index = underlayFloorIndex[plane,negativeOffsetX,y] & 0xFF;
                            if (flo_index > 0)
                            {
                                var f = GameContext.Cache.GetFloorConfig(flo_index - 1);
                                blendedHue[y] -= f.Hue;
                                blendedSaturation[y] -= f.Saturation;
                                blendedBrightness[y] -= f.Lightness;
                                blendedHueDivisor[y] -= f.HueDivisor;
                                blendedDirection[y]--;
                            }
                        }
                    }

                    if (x >= 1 && x < 104 - 1)
                    {
                        int curBlendedHue = 0;
                        int curBlendedSaturation = 0;
                        int curBlendedBrightness = 0;
                        int curHueDivisor = 0;
                        int curDivisor = 0;

                        for (int y = -5; y < 104 + 5; y++)
                        {
                            int positive = y + 5;
                            if (positive >= 0 && positive < 104)
                            {
                                curBlendedHue += blendedHue[positive];
                                curBlendedSaturation += blendedSaturation[positive];
                                curBlendedBrightness += blendedBrightness[positive];
                                curHueDivisor += blendedHueDivisor[positive];
                                curDivisor += blendedDirection[positive];
                            }

                            int negative = y - 5;
                            if (negative >= 0 && negative < 104)
                            {
                                curBlendedHue -= blendedHue[negative];
                                curBlendedSaturation -= blendedSaturation[negative];
                                curBlendedBrightness -= blendedBrightness[negative];
                                curHueDivisor -= blendedHueDivisor[negative];
                                curDivisor -= blendedDirection[negative];
                            }

                            if (y >= 1 && y < 104 - 1)
                            {
                                int underlayId = underlayFloorIndex[plane, x, y] & 0xFF;
                                int overlayId = overlayFloorIndex[plane, x, y] & 0xFF;

                                if (underlayId > 0 || overlayId > 0)
                                {
                                    int southWestHeight = heightMap[plane, x, y];
                                    int southEastHeight = heightMap[plane, x + 1, y];
                                    int northEastHeight = heightMap[plane, x + 1, y + 1];
                                    int northWestHeight = heightMap[plane, x, y + 1];
                                    int lightSouthWest = lightMap[x, y];
                                    int lightSouthEast = lightMap[x + 1, y];
                                    int lightNorthEast = lightMap[x + 1, y + 1];
                                    int lightNorthWest = lightMap[x, y + 1];
                                    int hslSouthWest = underlayHsls[plane, x, y];
                                    int hslSouthEast = underlayHsls[plane, x + 1, y];
                                    int hslNorthEast = underlayHsls[plane, x + 1, y + 1];
                                    int hslNorthWest = underlayHsls[plane, x, y + 1];

                                    if (hslSouthEast == -1)
                                    {
                                        hslSouthEast = hslSouthWest;
                                    }

                                    if (hslNorthEast == -1)
                                    {
                                        hslNorthEast = hslSouthWest;
                                    }

                                    if (hslNorthWest == -1)
                                    {
                                        hslNorthWest = hslSouthWest;
                                    }

                                    int underlayHsl = -1;
                                    int underlayMinimapHsl = -1;
                                    int underlayTex = -1;

                                    if (underlayId > 0)
                                    {
                                        int hue = curBlendedHue * 256 / curHueDivisor;
                                        int saturation = curBlendedSaturation / curDivisor;
                                        int brightness = curBlendedBrightness / curDivisor;
                                        underlayHsl = ColorUtils.TrimHSL(hue, saturation, brightness);
                                        hue = hue + hueRandomizer & 0xFF;
                                        brightness += lightnessRandomizer;

                                        if (brightness < 0)
                                        {
                                            brightness = 0;
                                        }
                                        else if (brightness > 255)
                                        {
                                            brightness = 255;
                                        }

                                        underlayMinimapHsl = ColorUtils.TrimHSL(hue, saturation, brightness);
                                    }

                                    int underlayMinimapRgb = 0;
                                    if (underlayHsl != -1)
                                    {
                                        underlayMinimapRgb = ColorUtils.HSLToRGBMap[ColorUtils.SetHslLight(underlayMinimapHsl, 96)];
                                    }

                                    if (overlayId == 0)
                                    {
                                        var floor = GameContext.Cache.GetFloorConfig(underlayId - 1);
                                        AddTile(plane, x, y, 0, 0, underlayTex, underlayTex,
                                            southWestHeight, southEastHeight, northEastHeight, northWestHeight,
                                            ColorUtils.SetHslLight(underlayHsl, lightSouthWest), ColorUtils.SetHslLight(underlayHsl, lightSouthEast), ColorUtils.SetHslLight(underlayHsl, lightNorthEast), ColorUtils.SetHslLight(underlayHsl, lightNorthWest),
                                            0, 0, 0, 0,
                                            underlayMinimapRgb, 0);
                                    }
                                    else
                                    {
                                        int shape = overlayShape[plane, x, y] + 1;
                                        byte rotation = overlayRotation[plane, x, y];
                                        var f = GameContext.Cache.GetFloorConfig(overlayId - 1);
                                        int overlayTexture = f.TextureIndex;
                                        int overlayMinimapRgb = ColorUtils.HSLToRGBMap[ColorUtils.SetHslLight2(f.Hsl, 96)];
                                        int overlayHsl = ColorUtils.TrimHSL(f.Hue2, f.Saturation, f.Lightness);

                                        if (overlayTexture >= 0)
                                        {
                                            //overlayHsl = Canvas3D.get_average_texture_rgb(texture);
                                            //overlayTexture = -1;
                                        }
                                        if (f.Rgb == 0xFF00FF)
                                        {
                                            //overlayMinimapRgb = 0;
                                            overlayHsl = -2;
                                            overlayTexture = -1;
                                        }
                                        else
                                        {
                                            overlayHsl = ColorUtils.TrimHSL(f.Hue2, f.Saturation, f.Lightness);
                                            overlayMinimapRgb = ColorUtils.HSLToRGBMap[ColorUtils.SetHslLight2(f.Hsl, 96)];
                                        }

                                        AddTile(plane, x, y, shape, rotation, overlayTexture, underlayTex,
                                            southWestHeight, southEastHeight, northEastHeight, northWestHeight,
                                            ColorUtils.SetHslLight(underlayHsl, lightSouthWest), ColorUtils.SetHslLight(underlayHsl, lightSouthEast), ColorUtils.SetHslLight(underlayHsl, lightNorthEast), ColorUtils.SetHslLight(underlayHsl, lightNorthWest),
                                            ColorUtils.SetHslLight2(overlayHsl, lightSouthWest), ColorUtils.SetHslLight2(overlayHsl, lightSouthEast), ColorUtils.SetHslLight2(overlayHsl, lightNorthEast), ColorUtils.SetHslLight2(overlayHsl, lightNorthWest),
                                            underlayMinimapRgb, overlayMinimapRgb);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            for (int x = 0; x < 104; x++)
            {
                for (int y = 0; y < 104; y++)
                {
                    if ((renderFlags[1, x, y] & 0x2) == 2)
                    {
                        AddBridge(x, y);
                    }
                }
            }
        }

        public void CreateObjects()
        {
            for (int z = 0; z < 4; z++)
            {
                for (int x = 0; x < 104; x++)
                {
                    for (int y = 0; y < 104; y++)
                    {
                        SceneTile tile = tiles[z, x, y];
                        if (tile == null) continue;

                        SceneTile bridgedTile = tile.Bridge;
                        if (bridgedTile != null)
                        {
                            for (int i = 0; i < bridgedTile.InteractiveCount; i++)
                            {
                                var obj = bridgedTile.Interactives[i];
                                var sx = obj.SceneX;
                                var sy = obj.SceneY;
                                var sz = obj.SceneZ;
                                var o = obj.Node;
                                if (o is Model)
                                {
                                    var node = o as Model;
                                    var bridgeObj = new GameObject("Interactive (bridge) " + obj.UniqueId);
                                    AddAsSceneObject(bridgeObj);
                                    node.Backing = bridgeObj;
                                    node.AddMeshToObject();
                                    bridgeObj.transform.position = new Vector3(GameConstants.RScale(sx), GameConstants.RScale(sy), GameConstants.RScale(sz));
                                }
                                else if (o is AnimatedObject)
                                {
                                    var node = o as AnimatedObject;
                                    node.Init();
                                    node.UnityObject.transform.position = new Vector3(GameConstants.RScale(sx), GameConstants.RScale(sy), GameConstants.RScale(sz));
                                    node.ApplyAnimations();

                                    if (obj.UniqueId > 0)
                                    {
                                        var comp = node.UnityObject.AddComponent<InteractiveComponent>();
                                        comp.GameObject = obj;
                                    }

                                    AddAsSceneObject(node.UnityObject);
                                }
                            }

                            WallObject wall = bridgedTile.Wall;
                            if (wall != null)
                            {
                                var type = (wall.UniqueId >> 61) & 3;

                                float sx = wall.SceneX;
                                float sy = wall.SceneY;
                                float sz = wall.SceneZ;

                                if (wall.Root != null)
                                {
                                    GameObject rootObj = new GameObject("WallRoot (bridge) " + wall.UniqueId);
                                    wall.Root.Backing = rootObj;
                                    wall.Root.AddMeshToObject();
                                    rootObj.transform.position = new Vector3(GameConstants.RScale(sx), GameConstants.RScale(sy), GameConstants.RScale(sz));

                                    if (wall.UniqueId > 0)
                                    {
                                        var comp = rootObj.AddComponent<WallComponent>();
                                        comp.GameObject = wall;

                                        var collider = rootObj.AddComponent<MeshCollider>();
                                        collider.sharedMesh = rootObj.GetComponent<MeshFilter>().mesh;
                                        collider.enabled = true;
                                    }

                                    AddAsSceneObject(rootObj);
                                }

                                if (wall.Extension != null)
                                {
                                    GameObject extensionObj = new GameObject("WallExtension (bridge) " + wall.UniqueId);
                                    wall.Extension.Backing = extensionObj;
                                    wall.Extension.AddMeshToObject();
                                    extensionObj.transform.position = new Vector3(GameConstants.RScale(sx), GameConstants.RScale(sy), GameConstants.RScale(sz));

                                    if (wall.UniqueId > 0)
                                    {
                                        var comp = extensionObj.AddComponent<WallComponent>();
                                        comp.GameObject = wall;

                                        var collider = extensionObj.AddComponent<MeshCollider>();
                                        collider.sharedMesh = extensionObj.GetComponent<MeshFilter>().mesh;
                                        collider.enabled = true;
                                    }

                                    AddAsSceneObject(extensionObj);
                                }
                            }
                        }

                        bool overX = (tile.Flags & 1) == 1;
                        bool overY = (tile.Flags & 8) == 8;
                        if (!overX && !overY)
                        {
                            for (int i = 0; i < tile.InteractiveCount; i++)
                            {
                                InteractiveObject obj = tile.Interactives[i];

                                var o = obj.Node;
                                float sx = obj.SceneX;
                                float sy = obj.SceneY;
                                float sz = obj.SceneZ;

                                if (o is Model)
                                {
                                    var node = o as Model;
                                    GameObject interactiveObj = new GameObject("Interactive " + obj.UniqueId);

                                    var index = (int)(obj.UniqueId >> 14 & 0xffff);
                                    var desc = GameContext.Cache.GetObjectConfig(index);

                                    node.Backing = interactiveObj;
                                    node.AddMeshToObject(false, desc.name != null && desc.name.ToLower().Contains("tree"));
                                    interactiveObj.transform.position = new Vector3(GameConstants.RScale(sx), GameConstants.RScale(sy), GameConstants.RScale(sz));

                                    if (obj.UniqueId > 0)
                                    {
                                        var comp = interactiveObj.AddComponent<InteractiveComponent>();
                                        comp.GameObject = obj;

                                        var collider = interactiveObj.AddComponent<MeshCollider>();
                                        collider.sharedMesh = interactiveObj.GetComponent<MeshFilter>().mesh;
                                        collider.enabled = true;
                                    }

                                    AddAsSceneObject(interactiveObj);
                                }
                                else if (o is AnimatedObject)
                                {
                                    var node = o as AnimatedObject;
                                    node.Init();
                                    
                                    node.UnityObject.transform.position = new Vector3(GameConstants.RScale(sx), GameConstants.RScale(sy), GameConstants.RScale(sz));
                                    node.ApplyAnimations();

                                    if (obj.UniqueId > 0)
                                    {
                                        var comp = node.UnityObject.AddComponent<InteractiveComponent>();
                                        comp.GameObject = obj;
                                    }

                                    AddAsSceneObject(node.UnityObject);
                                }
                            }
                        }

                        if (tile.Wall != null)
                        {
                            WallObject wall = tile.Wall;
                            var type = (wall.UniqueId >> 61) & 3;

                            float sx = wall.SceneX;
                            float sy = wall.SceneY;
                            float sz = wall.SceneZ;

                            if (wall.Root != null)
                            {
                                GameObject rootObj = new GameObject("WallRoot " + wall.UniqueId);
                                
                                wall.Root.Backing = rootObj;
                                wall.Root.AddMeshToObject();
                                rootObj.transform.position = new Vector3(GameConstants.RScale(sx), GameConstants.RScale(sy), GameConstants.RScale(sz));

                                if (wall.UniqueId > 0)
                                {
                                    var comp = rootObj.AddComponent<WallComponent>();
                                    comp.GameObject = wall;

                                    var collider = rootObj.AddComponent<MeshCollider>();
                                    collider.sharedMesh = rootObj.GetComponent<MeshFilter>().mesh;
                                    collider.enabled = true;
                                }

                                AddAsSceneObject(rootObj);
                            }

                            if (wall.Extension != null)
                            {
                                GameObject extensionObj = new GameObject("WallExtension " + wall.UniqueId);
                                
                                wall.Extension.Backing = extensionObj;
                                wall.Extension.AddMeshToObject();
                                extensionObj.transform.position = new Vector3(GameConstants.RScale(sx), GameConstants.RScale(sy), GameConstants.RScale(sz));

                                if (wall.UniqueId > 0)
                                {
                                    var comp = extensionObj.AddComponent<WallComponent>();
                                    comp.GameObject = wall;

                                    var collider = extensionObj.AddComponent<MeshCollider>();
                                    collider.sharedMesh = extensionObj.GetComponent<MeshFilter>().mesh;
                                    collider.enabled = true;
                                }

                                AddAsSceneObject(extensionObj);
                            }
                        }

                        if (tile.WallDeco != null)
                        {
                            var deco = tile.WallDeco;
                            if (deco.Node != null)
                            {
                                var o = deco.Node;
                                var sx = deco.SceneX;
                                var sy = deco.SceneY;
                                var sz = deco.SceneZ;

                                if (o is Model)
                                {
                                    var node = o as Model;
                                    GameObject decoObj = new GameObject("WallDecoration " + deco.UniqueId);

                                    if ((deco.Flags & 0x200) != 0)
                                    {
                                        sx += WallPaddingX[deco.Rotation];
                                        sz += WallPaddingZ[deco.Rotation];
                                    }

                                    node.Backing = decoObj;
                                    node.AddMeshToObject();
                                    decoObj.transform.position = new Vector3(GameConstants.RScale(sx), GameConstants.RScale(sy), GameConstants.RScale(sz));
                                    decoObj.transform.rotation = Quaternion.Euler(0, deco.Rotation / 5.688888888888889f, 0);

                                    if ((deco.Flags & 0x200) != 0)
                                    {
                                        decoObj.transform.rotation = Quaternion.Euler(0, (((deco.Rotation * 512) + 1280) & 0x7FF) / 5.688888888888889f, 0);
                                    }
                                    else
                                    {
                                        decoObj.transform.rotation = Quaternion.Euler(0, deco.Rotation / 5.688888888888889f, 0);
                                    }

                                    if (deco.UniqueId > 0)
                                    {
                                        var comp = decoObj.AddComponent<WallDecorationComponent>();
                                        comp.GameObject = deco;

                                        var collider = decoObj.AddComponent<MeshCollider>();
                                        collider.sharedMesh = decoObj.GetComponent<MeshFilter>().mesh;
                                        collider.enabled = true;
                                    }

                                    AddAsSceneObject(decoObj);
                                }
                                else if (o is AnimatedObject)
                                {
                                    var node = o as AnimatedObject;
                                    node.Init();

                                    node.UnityObject.transform.position = new Vector3(GameConstants.RScale(sx), GameConstants.RScale(sy), GameConstants.RScale(sz));
                                    node.ApplyAnimations();

                                    if (deco.UniqueId > 0)
                                    {
                                        var comp = node.UnityObject.AddComponent<WallDecorationComponent>();
                                        comp.GameObject = deco;
                                    }

                                    AddAsSceneObject(node.UnityObject);
                                }
                            }
                        }

                        if (tile.GroundDeco != null)
                        {
                            var deco = tile.GroundDeco;
                            if (deco.Root != null/* && deco.UniqueId > 0*/)
                            {
                                GameObject decoObj = new GameObject("GroundDecoration " + deco.UniqueId);

                                float sx = deco.SceneX;
                                float sy = deco.SceneY;
                                float sz = deco.SceneZ;

                                deco.Root.Backing = decoObj;
                                deco.Root.AddMeshToObject();
                                decoObj.transform.position = new Vector3(GameConstants.RScale(sx), GameConstants.RScale(sy), GameConstants.RScale(sz));

                                if (deco.UniqueId > 0)
                                {
                                    var comp = decoObj.AddComponent<GroundDecorationComponent>();
                                    comp.GameObject = deco;

                                    var collider = decoObj.AddComponent<MeshCollider>();
                                    collider.sharedMesh = decoObj.GetComponent<MeshFilter>().mesh;
                                    collider.enabled = true;
                                }

                                AddAsSceneObject(decoObj);
                            }
                        }
                    }
                }
            }
        }

        private void CopyUnderlayTo(List<Color> colors, Dictionary<ReusableVertex, int> pointers, List<Vector3> vertices, List<Vector2> uvs, List<UnderlayMaterialConfig> segments, UnderlayTile underlay, int tileX, int tileY, int plane)
        {
            if (underlay.HslSouthEast == 12345678)
                return;

            RenderType type;
            var color = -1;// GameConstants.HslToRgbMap[underlay.HslSouthEast];
            var texture = -1;

            if (underlay.TextureIndex != -1)
            {
                type = RenderType.Textured;
                texture = underlay.TextureIndex;
                color = ColorUtils.HSLToRGBMap[underlay.HslSouthEast];
            }
            else
            {
                type = RenderType.Colored;
            }

            var config = new UnderlayMaterialConfig(type, color, 255, texture);
            if (segments.Contains(config))
            {
                config = segments.FirstOrDefault(f => f == config);
            }
            else
            {
                segments.Add(config);
            }

            config.Tiles.Add(tiles[plane, tileX, tileY]);

            var swyc = heightMap[plane, tileX, tileY] * GameConstants.RenderScale;
            var seyc = heightMap[plane, tileX + 1, tileY] * GameConstants.RenderScale;
            var neyc = heightMap[plane, tileX + 1, tileY + 1] * GameConstants.RenderScale;
            var nwyc = heightMap[plane, tileX, tileY + 1] * GameConstants.RenderScale;

            var nwxc = (tileX << 7) * GameConstants.RenderScale;
            var swxc = (tileX << 7) * GameConstants.RenderScale;
            var nexc = nwxc + (128 * GameConstants.RenderScale);
            var sexc = swxc + (128 * GameConstants.RenderScale);

            var sezc = (tileY << 7) * GameConstants.RenderScale;
            var swzc = (tileY << 7) * GameConstants.RenderScale;
            var nezc = sezc + (128 * GameConstants.RenderScale);
            var nwzc = swzc + (128 * GameConstants.RenderScale);

            var southEastVertex = new Vector3(sexc, seyc, sezc);
            var southEastUv = new Vector2(1, 0);
            var southEastMv = new ReusableVertex(southEastVertex, southEastUv, underlay.HslSouthEast);

            var southWestVertex = new Vector3(swxc, swyc, swzc);
            var southWestUv = new Vector2(0, 0);
            var southWestMv = new ReusableVertex(southWestVertex, southWestUv, underlay.HslSouthWest);

            var northWestVertex = new Vector3(nwxc, nwyc, nwzc);
            var northWestUv = new Vector2(0, 1);
            var northWestMv = new ReusableVertex(northWestVertex, northWestUv, underlay.HslNorthWest);

            var northEastVertex = new Vector3(nexc, neyc, nezc);
            var northEastUv = new Vector2(1, 1);
            var northEastMv = new ReusableVertex(northEastVertex, northEastUv, underlay.HslNorthEast);

            var southEastPointer = -1;
            if (!pointers.TryGetValue(southEastMv, out southEastPointer))
            {
                southEastPointer = -1;
            }

            var southWestPointer = -1;
            if (!pointers.TryGetValue(southWestMv, out southWestPointer))
            {
                southWestPointer = -1;
            }

            var northWestPointer = -1;
            if (!pointers.TryGetValue(northWestMv, out northWestPointer))
            {
                northWestPointer = -1;
            }

            var northEastPointer = -1;
            if (!pointers.TryGetValue(northEastMv, out northEastPointer))
            {
                northEastPointer = -1;
            }

            var ptr = vertices.Count;
            if (southEastPointer == -1)
            {
                vertices.Add(new Vector3(sexc, seyc, sezc));
                uvs.Add(new Vector2(1, 0));

                var rgb = ColorUtils.HSLToRGBMap[underlay.HslSouthEast];
                var r = (byte)((rgb >> 16) & 0xFF);
                var g = (byte)((rgb >> 8) & 0xFF);
                var b = (byte)((rgb) & 0xFF);

                colors.Add(new Color32(r, g, b, 255));
                pointers.Add(southEastMv, ptr);
                southEastPointer = ptr++;
            }

            if (southWestPointer == -1)
            {
                vertices.Add(new Vector3(swxc, swyc, swzc));
                uvs.Add(new Vector2(0, 0));
                var rgb = ColorUtils.HSLToRGBMap[underlay.HslSouthWest];
                var r = (byte)((rgb >> 16) & 0xFF);
                var g = (byte)((rgb >> 8) & 0xFF);
                var b = (byte)((rgb) & 0xFF);

                colors.Add(new Color32(r, g, b, 255));
                pointers.Add(southWestMv, ptr);
                southWestPointer = ptr++;
            }

            if (northWestPointer == -1)
            {
                vertices.Add(new Vector3(nwxc, nwyc, nwzc));
                uvs.Add(new Vector2(0, 1));
                var rgb = ColorUtils.HSLToRGBMap[underlay.HslNorthWest];
                var r = (byte)((rgb >> 16) & 0xFF);
                var g = (byte)((rgb >> 8) & 0xFF);
                var b = (byte)((rgb) & 0xFF);

                colors.Add(new Color32(r, g, b, 255));
                pointers.Add(northWestMv, ptr);
                northWestPointer = ptr++;
            }

            if (northEastPointer == -1)
            {
                vertices.Add(new Vector3(nexc, neyc, nezc));
                uvs.Add(new Vector2(1, 1));
                var rgb = ColorUtils.HSLToRGBMap[underlay.HslNorthEast];
                var r = (byte)((rgb >> 16) & 0xFF);
                var g = (byte)((rgb >> 8) & 0xFF);
                var b = (byte)((rgb) & 0xFF);

                colors.Add(new Color32(r, g, b, 255));
                pointers.Add(northEastMv, ptr);
                northEastPointer = ptr++;
            }

            config.Triangles.Add(southEastPointer);
            config.Triangles.Add(southWestPointer);
            config.Triangles.Add(northWestPointer);
            config.Triangles.Add(northEastPointer);
            config.Triangles.Add(southEastPointer);
            config.Triangles.Add(northWestPointer);
        }

        public void Update()
        {
            foreach (var obj in animatedObjects)
            {
                var interactiveComp = obj.GetComponent<InteractiveComponent>();
                if (interactiveComp != null)
                {
                    ((AnimatedObject)interactiveComp.GameObject.Node).Update();
                }

                var wdComp = obj.GetComponent<WallDecorationComponent>();
                if (wdComp != null)
                {
                    ((AnimatedObject)wdComp.GameObject.Node).Update();
                }
            }
        }

        public void CopyUnderlay(List<Color> colors, Dictionary<ReusableVertex, int> pointers, List<Vector3> vertices, List<Vector2> uvs, List<UnderlayMaterialConfig> segments, GameObject obj, Mesh mesh, int offX, int offY, int width, int height, int plane, int depth)
        {
            for (int z = plane; z < (plane + depth); z++)
            {
                for (int x = offX; x < (offX + width); x++)
                {
                    for (int y = offY; y < (offY + height); y++)
                    {
                        SceneTile tile = tiles[z, x, y];
                        UnderlayTile underlay = tile.Underlay;
                        SceneTile bridge = tile.Bridge;
                        if (bridge != null)
                        {
                            UnderlayTile bunderlay = bridge.Underlay;
                            if (bunderlay != null)
                            {
                                CopyUnderlayTo(colors, pointers, vertices, uvs, segments, bunderlay, x, y, 0);
                            }
                        }

                        if (underlay != null)
                        {
                            CopyUnderlayTo(colors, pointers, vertices, uvs, segments, underlay, x, y, tile.Plane);
                        }
                    }
                }
            }
        }
        
        private long HashPos(int x, int y, int z)
        {
            return ((long)z << 42) | ((long)x << 21) | ((long)y);
        }

        private int GetHashX(long l)
        {
            return (int)((l >> 21) & 0x1FFFFF);
        }

        private int GetHashY(long l)
        {
            return (int)(l & 0x1FFFFF);
        }

        private int GetHashZ(long l)
        {
            return (int)((l >> 42) & 0x1FFFFF);
        }
        
        private void CopyOverlayTo(List<Color> colors, Dictionary<ReusableVertex, int> pointers, List<Vector3> vertices, List<Vector2> uvs, List<OverlayMaterialConfig> segments, OverlayTile overlay, int tileX, int tileY, int plane)
        {
            for (var i = 0; i < overlay.VertexX.Length; i++)
            {
                RenderType type;
                int color = 0xFF0000;
                int texture = -1;

                if (overlay.TriangleTextureIndex != null && overlay.TriangleTextureIndex[i] != -1)
                {
                    type = RenderType.Textured;
                    texture = overlay.TriangleTextureIndex[i];
                    color = ColorUtils.HSLToRGBMap[overlay.VertexColorA[i]];
                }
                else if (overlay.UnderlayMinimapRgb >= 0)
                {
                    if (overlay.VertexColorA[i] == 12345678)
                    {
                        continue;
                    }
                    
                    type = RenderType.Colored;
                }
                else
                {
                    continue;
                }

                var config = new OverlayMaterialConfig(type, color, 255, texture);
                if (segments.Contains(config))
                {
                    config = segments.FirstOrDefault(f => f == config);
                }
                else
                {
                    segments.Add(config);
                }
                
                var x1c = overlay.TriangleX[overlay.VertexX[i]] * GameConstants.RenderScale;
                var y1c = overlay.TriangleY[overlay.VertexX[i]] * GameConstants.RenderScale;
                var z1c = overlay.TriangleZ[overlay.VertexX[i]] * GameConstants.RenderScale;

                var x2c = overlay.TriangleX[overlay.VertexY[i]] * GameConstants.RenderScale;
                var y2c = overlay.TriangleY[overlay.VertexY[i]] * GameConstants.RenderScale;
                var z2c = overlay.TriangleZ[overlay.VertexY[i]] * GameConstants.RenderScale;

                var x3c = overlay.TriangleX[overlay.VertexZ[i]] * GameConstants.RenderScale;
                var y3c = overlay.TriangleY[overlay.VertexZ[i]] * GameConstants.RenderScale;
                var z3c = overlay.TriangleZ[overlay.VertexZ[i]] * GameConstants.RenderScale;

                var x1Vertex = new Vector3(x1c, y1c, z1c);
                var x1Uv = new Vector2(overlay.TextureMapX[overlay.VertexX[i]], overlay.TextureMapY[overlay.VertexX[i]]);
                var x1Mv = new ReusableVertex(x1Vertex, x1Uv, overlay.VertexColorA[i]);

                var x2Vertex = new Vector3(x2c, y2c, z2c);
                var x2Uv = new Vector2(overlay.TextureMapX[overlay.VertexY[i]], overlay.TextureMapY[overlay.VertexY[i]]);
                var x2Mv = new ReusableVertex(x2Vertex, x2Uv, overlay.VertexColorB[i]);

                var x3Vertex = new Vector3(x3c, y3c, z3c);
                var x3Uv = new Vector2(overlay.TextureMapX[overlay.VertexZ[i]], overlay.TextureMapY[overlay.VertexZ[i]]);
                var x3Mv = new ReusableVertex(x3Vertex, x3Uv, overlay.VertexColorC[i]);

                var ptr1 = -1;
                if (!pointers.TryGetValue(x3Mv, out ptr1))
                {
                    ptr1 = -1;
                }

                var ptr2 = -1;
                if (!pointers.TryGetValue(x2Mv, out ptr2))
                {
                    ptr2 = -1;
                }

                var ptr3 = -1; 
                if (!pointers.TryGetValue(x1Mv, out ptr3))
                {
                    ptr3 = -1;
                }

                var ptr = vertices.Count;
                if (ptr1 == -1)
                {
                    vertices.Add(x3Vertex);
                    uvs.Add(x3Uv);
                    pointers.Add(x3Mv, ptr);
                    var rgb = ColorUtils.HSLToRGBMap[overlay.VertexColorC[i]];
                    var r = (byte)((rgb >> 16) & 0xFF);
                    var g = (byte)((rgb >> 8) & 0xFF);
                    var b = (byte)((rgb) & 0xFF);

                    colors.Add(new Color32(r, g, b, 255));

                    ptr1 = ptr++;
                }

                if (ptr2 == -1)
                {
                    vertices.Add(x2Vertex);
                    uvs.Add(x2Uv);
                    pointers.Add(x2Mv, ptr);
                    var rgb = ColorUtils.HSLToRGBMap[overlay.VertexColorB[i]];
                    var r = (byte)((rgb >> 16) & 0xFF);
                    var g = (byte)((rgb >> 8) & 0xFF);
                    var b = (byte)((rgb) & 0xFF);

                    colors.Add(new Color32(r, g, b, 255));

                    ptr2 = ptr++;
                }

                if (ptr3 == -1)
                {
                    vertices.Add(x1Vertex);
                    uvs.Add(x1Uv);
                    pointers.Add(x1Mv, ptr);
                    var rgb = ColorUtils.HSLToRGBMap[overlay.VertexColorA[i]];
                    var r = (byte)((rgb >> 16) & 0xFF);
                    var g = (byte)((rgb >> 8) & 0xFF);
                    var b = (byte)((rgb) & 0xFF);

                    colors.Add(new Color32(r, g, b, 255));
                    ptr3 = ptr++;
                }

                config.Triangles.Add(ptr1);
                config.Triangles.Add(ptr2);
                config.Triangles.Add(ptr3);
            }
        }

        public void CopyOverlay(List<Color> colors, Dictionary<ReusableVertex, int> pointers, List<Vector3> vertices, List<Vector2> uvs, List<OverlayMaterialConfig> segments, GameObject obj, Mesh mesh, int offX, int offY, int width, int height, int plane, int depth)
        {
            for (int z = plane; z < (plane + depth); z++)
            {
                for (int x = offX; x < (offX + width); x++)
                {
                    for (int y = offY; y < (offY + height); y++)
                    {
                        SceneTile tile = tiles[z, x, y];
                        SceneTile bridge = tile.Bridge;
                        if (bridge != null)
                        {
                            OverlayTile boverlay = bridge.Overlay;
                            if (boverlay != null && bridge.Underlay == null)
                            {
                                CopyOverlayTo(colors, pointers, vertices, uvs, segments, boverlay, x, y, 0);
                            }
                        }

                        OverlayTile overlay = tile.Overlay;
                        if (overlay != null)
                        {
                            CopyOverlayTo(colors, pointers, vertices, uvs, segments, overlay, x, y, z);
                        }
                    }
                }
            }
        }
        
        public List<GameObject> CopyUnderlayAndOverlay()
        {
            var chunkSize = 26;
            var chunkCount = 104 / chunkSize;
            var created = new List<GameObject>();

            for (int x = 0; x < chunkCount; x++)
            {
                for (int y = 0; y < chunkCount; y++)
                {
                    Mesh mesh = new Mesh();
                    GameObject obj = new GameObject("CombinedMesh " + x + " " + y);
                    created.Add(obj);

                    AddAsSceneObject(obj);

                    var meshRenderer = obj.AddComponent<MeshRenderer>();
                    meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    var filter = obj.AddComponent<MeshFilter>();
                    filter.mesh = mesh;

                    var pointers = new Dictionary<ReusableVertex, int>();
                    var vertices = new List<Vector3>();
                    var uvs = new List<Vector2>();
                    var colors = new List<Color>();

                    var overlaySegments = new List<OverlayMaterialConfig>();
                    CopyOverlay(colors, pointers, vertices, uvs, overlaySegments, obj, mesh, x * chunkSize, y * chunkSize, chunkSize, chunkSize, PlaneAtBuild, 3);

                    var underlaySegments = new List<UnderlayMaterialConfig>();
                    CopyUnderlay(colors, pointers, vertices, uvs, underlaySegments, obj, mesh, x * chunkSize, y * chunkSize, chunkSize, chunkSize, PlaneAtBuild, 3);

                    mesh.subMeshCount = overlaySegments.Count + underlaySegments.Count;
                    mesh.vertices = vertices.ToArray();
                    mesh.uv = uvs.ToArray();
                    mesh.colors = colors.ToArray();

                    var mats = new UnityEngine.Material[mesh.subMeshCount];
                    var matPointer = 0;

                    foreach (var seg in overlaySegments)
                    {
                        if (seg.Type == RenderType.Colored)
                        {
                            if (seg.Texture == 1 || seg.Texture == 24 || seg.Texture == 138 || seg.Texture == 174 || seg.Texture == 191 || seg.Texture == 336 || seg.Texture == 638 || seg.Texture == 669)
                            {
                                var mat = mats[matPointer] = GameObject.Instantiate((UnityEngine.Material)Resources.Load("Standard Assets/Environment/Water/Water4/Materials/Water4Advanced", typeof(UnityEngine.Material)));
                                mat.shader = Shader.Find("FX/Water4");
                                mat.SetVector("_GAmplitude", new Vector4(0, 0, 0, 0));
                            }
                            else
                            {
                                var rgb = seg.Color;
                                var r = (byte)((rgb >> 16) & 0xFF);
                                var g = (byte)((rgb >> 8) & 0xFF);
                                var b = (byte)(rgb & 0xFF);
                                var a = (byte)seg.Opacity;

                                var mat = mats[matPointer] = new Material(Shader.Find("Diffuse"));
                                if (a < 255)
                                {
                                    mat.shader = Shader.Find("Transparent/Diffuse");
                                }
                                else
                                {
                                    mat.shader = Shader.Find("Custom/Vertex Colored");
                                }

                                //mat.color = new Color32(r, g, b, a);
                            }
                        }
                        else if (seg.Type == RenderType.Textured)
                        {
                            if (seg.Texture == 1 || seg.Texture == 24 || seg.Texture == 138 || seg.Texture == 174 || seg.Texture == 191 || seg.Texture == 336 || seg.Texture == 638 || seg.Texture == 669)
                            {
                                var mat = mats[matPointer] = GameObject.Instantiate((UnityEngine.Material)Resources.Load("Standard Assets/Environment/Water/Water4/Materials/Water4Advanced", typeof(UnityEngine.Material)));
                                mat.SetVector("_GAmplitude", new Vector4(0, 0, 0, 0));
                                mat.shader = Shader.Find("FX/Water4");
                            }
                            else
                            {
                                var rgb = seg.Color;
                                var r = (byte)((rgb >> 16) & 0xFF);
                                var g = (byte)((rgb >> 8) & 0xFF);
                                var b = (byte)(rgb & 0xFF);
                                var a = (byte)seg.Opacity;

                                var mat = mats[matPointer] = new Material(Shader.Find("Diffuse"));
                                mat.mainTexture = GameContext.MaterialPool.GetTextureAsUnity(seg.Texture);
                                if (a < 255)
                                {
                                    mat.shader = Shader.Find("Transparent/Diffuse");
                                }
                                else
                                {
                                    mat.shader = Shader.Find("Diffuse");
                                }

                                mat.color = new Color32(r, g, b, a);
                            }
                        }

                        mesh.SetTriangles(seg.Triangles.ToArray(), matPointer);
                        matPointer++;
                    }

                    foreach (var seg in underlaySegments)
                    {
                        if (seg.Type == RenderType.Colored)
                        {
                            var rgb = seg.Color;
                            var r = (byte)((rgb >> 16) & 0xFF);
                            var g = (byte)((rgb >> 8) & 0xFF);
                            var b = (byte)(rgb & 0xFF);
                            var a = (byte)seg.Opacity;

                            if (rgb == 0xCC00CC || rgb == 0xFF00FF)
                            {
                                a = 0;
                            }

                            var mat = mats[matPointer] = new Material(Shader.Find("Diffuse"));
                            if (a < 255)
                            {
                                mat.shader = Shader.Find("Transparent/Diffuse");
                            }
                            else
                            {
                                mat.shader = Shader.Find("Custom/Vertex Colored");
                            }

                            //mat.color = new Color32(r, g, b, a);
                        }
                        else if (seg.Type == RenderType.Textured)
                        {
                            if (seg.Texture == 1 || seg.Texture == 24 || seg.Texture == 138 || seg.Texture == 174 || seg.Texture == 191 || seg.Texture == 336 || seg.Texture == 638 || seg.Texture == 669)
                            {
                                var mat = mats[matPointer] = GameObject.Instantiate((UnityEngine.Material)Resources.Load("Standard Assets/Environment/Water/Water4/Materials/Water4Advanced", typeof(UnityEngine.Material)));
                                mat.shader = Shader.Find("FX/Water4");
                                mat.SetVector("_GAmplitude", new Vector4(0, 0, 0, 0));
                            }
                            else
                            {
                                var mat = mats[matPointer] = new Material(Shader.Find("Diffuse"));
                                mat.shader = Shader.Find("Diffuse");
                                mat.mainTexture = GameContext.MaterialPool.GetTextureAsUnity(seg.Texture);
                                var rgb = seg.Color;
                                var r = (byte)((rgb >> 16) & 0xFF);
                                var g = (byte)((rgb >> 8) & 0xFF);
                                var b = (byte)(rgb & 0xFF);
                                var a = (byte)seg.Opacity;
                                mat.color = new Color32(r, g, b, a);
                            }
                        }

                        mesh.SetTriangles(seg.Triangles.ToArray(), matPointer);
                        matPointer++;
                    }

                    meshRenderer.sharedMaterials = mats;

                    var normals = new Vector3[vertices.Count];
                    for (var i = 0; i < normals.Length; i++)
                    {
                        normals[i] = new Vector3(1, 1, 1);
                    }
                    mesh.normals = normals;

                    var collider = obj.AddComponent<MeshCollider>();
                    collider.sharedMesh = mesh;
                    collider.enabled = true;

                    var groundMesh = obj.AddComponent<SceneComponent>();
                    groundMesh.Scene = this;

                   // mesh.Optimize();
                    mesh.RecalculateBounds();
                    mesh.RecalculateNormals();
                }
            }
            return created;
        }

        public void LoadLandscapeSmallChunk(JagexBuffer buffer, int positionX, int positionY, int loadChunkX, int loadChunkY)
        {
            for (int plane = 0; plane < 4; plane++)
            {
                for (int x = 0; x < 64; x++)
                {
                    for (int y = 0; y < 64; y++)
                    {
                        int tileX = positionX + x;
                        int tileY = positionY + y;

                        LoadLandscapeChunkTile(buffer, tileX, tileY, loadChunkX, loadChunkY, plane, 0);
                    }
                }
            }
        }

        public void LoadLandscapeChunkTile(JagexBuffer buf, int tileX, int tileY, int perlinOffsetX, int perlinOffsetY, int plane, int type)
        {
            if (tileX >= 0 && tileX < 104 && tileY >= 0 && tileY < 104)
            {
                renderFlags[plane, tileX, tileY] = 0;

                while (true)
                {
                    int opcode = buf.ReadUByte();
                    if (opcode == 0)
                    {
                        if (plane == 0)
                        {
                            heightMap[0, tileX, tileY] = 20000 + PerlinNoise.GetNoiseHeight(932731 + tileX + perlinOffsetX, 556238 + tileY + perlinOffsetY) * 8;
                        }
                        else
                        {
                            heightMap[plane, tileX, tileY] = heightMap[plane - 1, tileX, tileY] + 240;
                        }
                        break;
                    }

                    if (opcode == 1)
                    {
                        int height = buf.ReadUByte();

                        if (height == 1)
                        {
                            height = 0;
                        }

                        if (plane == 0)
                        {
                            heightMap[0, tileX, tileY] = 20000 + height * 8;
                        }
                        else
                        {
                            heightMap[plane, tileX, tileY] = heightMap[plane - 1, tileX, tileY] + height * 8;
                        }
                        break;
                    }

                    if (opcode <= 49)
                    {
                        overlayFloorIndex[plane, tileX, tileY] = (byte)buf.ReadByte();
                        overlayShape[plane, tileX, tileY] = (byte)((opcode - 2) / 4);
                        overlayRotation[plane, tileX, tileY] = (byte)(opcode - 2 + type & 0x3);
                    }
                    else if (opcode <= 81)
                    {
                        renderFlags[plane, tileX, tileY] = (byte)(opcode - 49);
                    }
                    else
                    {
                        underlayFloorIndex[plane, tileX, tileY] = (byte)(opcode - 81);
                    }
                }
            }
            else
            {
                while (true)
                {
                    int i = buf.ReadUByte();

                    if (i == 0)
                    {
                        break;
                    }

                    if (i == 1)
                    {
                        buf.ReadUByte();
                        break;
                    }

                    if (i <= 49)
                    {
                        buf.ReadUByte();
                    }
                }
            }
        }

        public void LoadObjectsSmallChunk(CollisionMap[] maps, JagexBuffer buf, int regionX, int regionY)
        {
            int currentIndex = -1;
            while (true)
            {
                int msb = buf.ReadUSmart();
                if (msb == 0)
                {
                    break;
                }

                currentIndex += msb;
                int uid = 0;

                while (true)
                {
                    int lsb = buf.ReadUSmart();
                    if (lsb == 0)
                    {
                        break;
                    }

                    uid += lsb - 1;
                    var objBaseY = uid & 0x3f;
                    var objBaseX = uid >> 6 & 0x3f;
                    var objPlane = uid >> 12;
                    var objArrangement = buf.ReadUByte();
                    var objType = objArrangement >> 2;
                    var objRotation = objArrangement & 0x3;
                    var objTileX = objBaseX + regionX;
                    var objTileY = objBaseY + regionY;
                    if (objTileX > 0 && objTileY > 0 && objTileX < 103 && objTileY < 103)
                    {
                        CollisionMap map = null;
                        var plane = objPlane;

                        if ((renderFlags[1, objTileX, objTileY] & 0x2) == 2)
                        {
                            plane--;
                        }

                        if (plane >= 0)
                        {
                            map = maps[plane];
                        }

                        AddObject(map, currentIndex, objType, objTileX, objTileY, objPlane, objRotation);
                    }
                }
            }
        }

        public void AddItemPile(int x, int y, int z, int plane, Model top, Model mid, Model bot, GroundItem topItem, GroundItem midItem, GroundItem botItem, int uniqueId)
        {
            var minZ = 0;
            var t = tiles[plane, x, y];
            if (t != null)
            {
                for (var i = 0; i < t.InteractiveCount; i++)
                {
                    var obj = t.Interactives[i];
                }
            }

            if (tiles[plane, x, y] == null)
            {
                tiles[plane, x, y] = new SceneTile(x, y, plane);
            }

            RemoveItemPile(x, y, plane);

            var items = new GroundItems(top, bot, mid, topItem, midItem, botItem, uniqueId, (x * 128) + 64, z, (y * 128) + 64, minZ);
            tiles[plane, x, y].GroundItems = items;
            items.Create();
            items.Link(this);
        }

        public void RemoveItemPile(int x, int y, int plane)
        {
            var t = tiles[plane, x, y];
            if (t != null)
            {
                var items = t.GroundItems;
                if (items != null)
                {
                    items.Unlink(this);
                    items.Destroy();
                    t.GroundItems = null;
                }
            }
        }
    }
}
