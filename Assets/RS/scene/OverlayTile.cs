using System.Collections.Generic;

using UnityEngine;

namespace RS
{
    /// <summary>
    /// Represents an overlay tile in the scene.
    /// </summary>
    public class OverlayTile
    {
        static OverlayTile()
        {
            
        }

        public int TextureIndex;
        public int UnderlayMinimapRgb;
        public int OverlayMinimapRgb;
        public bool IgnoreUv;
        public int Rotation;
        public int Shape;
        public int[] TriangleTextureIndex;
        public int[] TriangleX;
        public int[] TriangleY;
        public int[] TriangleZ;
        public int[] VertexColorA;
        public int[] VertexColorB;
        public int[] VertexColorC;
        public int[] VertexX;
        public int[] VertexY;
        public int[] VertexZ;
        public float[] TextureMapX;
        public float[] TextureMapY;

        public int SceneX;
        public int SceneZ;

        public int SwHeight;
        public int SeHeight;
        public int NwHeight;
        public int NeHeight;
        
        public OverlayTile(
            int localX, int localY, 
            int swHeight, int seHeight, int neHeight, int nwHeight, 
            int overlaySwHsl, int overlaySeHsl, int overlayNeHsl, int overlayNwHsl,
            int underlaySwHsl, int underlaySeHsl, int underlayNeHsl, int underlayNwHsl,
            int underlayMinimapRgb, int overlayMinimapRgb, 
            int textureIndex, int textureIndex2, int rotation, int shape)
        {
            SwHeight = swHeight;
            SeHeight = seHeight;
            NwHeight = nwHeight;
            NeHeight = neHeight;

            IgnoreUv = true;
            if (swHeight != seHeight || swHeight != neHeight || swHeight != nwHeight)
            {
                IgnoreUv = false;
            }

            Shape = shape;
            Rotation = rotation;
            UnderlayMinimapRgb = underlayMinimapRgb;
            OverlayMinimapRgb = overlayMinimapRgb;
            TextureIndex = textureIndex;

            var tileSceneSize = 128;
            var half = (byte)(tileSceneSize / 2);
            var quarter = (byte)(tileSceneSize / 4);
            var threequarters = (byte)((tileSceneSize * 3) / 4);
            var opcodes = GameConstants.OverlayClippingFlags[shape];
            var length = opcodes.Length;

            TriangleX = new int[length];
            TriangleY = new int[length];
            TriangleZ = new int[length];
            TextureMapX = new float[length];
            TextureMapY = new float[length];

            var underlayHsls = new int[length];
            var overlayHsls = new int[length];

            int sceneX = (localX * tileSceneSize);
            SceneX = sceneX;

            int sceneY = (localY * tileSceneSize);
            SceneZ = sceneY;

            for (int i = 0; i < length; i++)
            {
                int opcode = opcodes[i];

                if ((opcode & 1) == 0 && opcode <= 8)
                {
                    opcode = (opcode - rotation - rotation - 1 & 7) + 1;
                }

                if (opcode > 8 && opcode <= 12)
                {
                    opcode = (opcode - 9 - rotation & 3) + 9;
                }

                if (opcode > 12 && opcode <= 16)
                {
                    opcode = (opcode - 13 - rotation & 3) + 13;
                }

                int x;
                int z;
                int y;
                int underlayHsl;
                int overlayHsl;
                var texX = 0.0f;
                var texY = 0.0f;
                switch (opcode)
                {
                    case 1:
                        x = sceneX;
                        z = sceneY;
                        y = swHeight;
                        texX = 0.0f;
                        texY = 0.0f;
                        underlayHsl = underlaySwHsl;
                        overlayHsl = overlaySwHsl;
                        break;
                    case 2:
                        x = (sceneX + half);
                        z = sceneY;
                        y = (swHeight + seHeight >> 1);
                        texX = 0.5f;
                        texY = 0.0f;
                        underlayHsl = underlaySwHsl + underlaySeHsl >> 1;
                        overlayHsl = overlaySwHsl + overlaySeHsl >> 1;
                        break;
                    case 3:
                        x = (sceneX + tileSceneSize);
                        z = sceneY;
                        y = seHeight;
                        texX = 1.0f;
                        texY = 0.0f;
                        underlayHsl = underlaySeHsl;
                        overlayHsl = overlaySeHsl;
                        break;
                    case 4:
                        x = (sceneX + tileSceneSize);
                        z = (sceneY + half);
                        y = (seHeight + neHeight >> 1);
                        texX = 1.0f;
                        texY = 0.5f;
                        underlayHsl = underlaySeHsl + underlayNeHsl >> 1;
                        overlayHsl = overlaySeHsl + overlayNeHsl >> 1;
                        break;
                    case 5:
                        x = (sceneX + tileSceneSize);
                        z = (sceneY + tileSceneSize);
                        y = neHeight;
                        texX = 1.0f;
                        texY = 1.0f;
                        underlayHsl = underlayNeHsl;
                        overlayHsl = overlayNeHsl;
                        break;
                    case 6:
                        x = (sceneX + half);
                        z = (sceneY + tileSceneSize);
                        y = (neHeight + nwHeight >> 1);
                        texX = 0.5f;
                        texY = 1.0f;
                        underlayHsl = underlayNeHsl + underlayNwHsl >> 1;
                        overlayHsl = overlayNeHsl + overlayNwHsl >> 1;
                        break;
                    case 7:
                        x = sceneX;
                        z = (sceneY + tileSceneSize);
                        y = nwHeight;
                        texX = 0.0f;
                        texY = 1.0f;
                        underlayHsl = underlayNwHsl;
                        overlayHsl = overlayNwHsl;
                        break;
                    case 8:
                        x = sceneX;
                        z = (sceneY + half);
                        y = (nwHeight + swHeight >> 1);
                        texX = 0.0f;
                        texY = 0.5f;
                        underlayHsl = underlayNwHsl + underlaySwHsl >> 1;
                        overlayHsl = overlayNwHsl + overlaySwHsl >> 1;
                        break;
                    case 9:
                        x = (sceneX + half);
                        z = (sceneY + quarter);
                        y = (swHeight + seHeight >> 1);
                        texX = 0.5f;
                        texY = 0.25f;
                        underlayHsl = underlaySwHsl + underlaySeHsl >> 1;
                        overlayHsl = overlaySwHsl + overlaySeHsl >> 1;
                        break;
                    case 10:
                        x = (sceneX + threequarters);
                        z = (sceneY + half);
                        y = (seHeight + neHeight >> 1);
                        texX = 0.75f;
                        texY = 0.5f;
                        underlayHsl = underlaySeHsl + underlayNeHsl >> 1;
                        overlayHsl = overlaySeHsl + overlayNeHsl >> 1;
                        break;
                    case 11:
                        x = (sceneX + half);
                        z = (sceneY + threequarters);
                        y = (neHeight + nwHeight >> 1);
                        texX = 0.5f;
                        texY = 0.75f;
                        underlayHsl = underlayNeHsl + underlayNwHsl >> 1;
                        overlayHsl = overlayNeHsl + overlayNwHsl >> 1;
                        break;
                    case 12:
                        x = (sceneX + quarter);
                        z = (sceneY + half);
                        y = (nwHeight + swHeight >> 1);
                        texX = 0.25f;
                        texY = 0.5f;
                        underlayHsl = underlayNwHsl + underlaySwHsl >> 1;
                        overlayHsl = overlayNwHsl + overlaySwHsl >> 1;
                        break;
                    case 13:
                        x = (sceneX + quarter);
                        z = (sceneY + quarter);
                        y = swHeight;
                        texX = 0.25f;
                        texY = 0.25f;
                        underlayHsl = underlaySwHsl;
                        overlayHsl = overlaySwHsl;
                        break;
                    case 14:
                        x = (sceneX + threequarters);
                        z = (sceneY + quarter);
                        y = seHeight;
                        texX = 0.75f;
                        texY = 0.25f;
                        underlayHsl = underlaySeHsl;
                        overlayHsl = overlaySeHsl;
                        break;
                    case 15:
                        x = (sceneX + threequarters);
                        z = (sceneY + threequarters);
                        y = neHeight;
                        texX = 0.75f;
                        texY = 0.75f;
                        underlayHsl = underlayNeHsl;
                        overlayHsl = overlayNeHsl;
                        break;
                    default:
                        x = (sceneX + quarter);
                        z = (sceneY + threequarters);
                        y = nwHeight;
                        texX = 0.25f;
                        texY = 0.75f;
                        underlayHsl = underlayNwHsl;
                        overlayHsl = overlayNwHsl;
                        break;
                }

                TriangleX[i] = x;
                TriangleY[i] = y;
                TriangleZ[i] = z;
                TextureMapX[i] = texX;
                TextureMapY[i] = texY;
                underlayHsls[i] = underlayHsl;
                overlayHsls[i] = overlayHsl;
            }

            var path = GameConstants.OverlapClippingPath[shape];
            var vertexCount = path.Length / 4;
            VertexX = new int[vertexCount];
            VertexY = new int[vertexCount];
            VertexZ = new int[vertexCount];
            VertexColorA = new int[vertexCount];
            VertexColorB = new int[vertexCount];
            VertexColorC = new int[vertexCount];

            if (textureIndex != -1 || textureIndex2 != -1)
            {
                TriangleTextureIndex = new int[vertexCount];
            }

            var ix = 0;
            for (var j = 0; j < vertexCount; j++)
            {
                var type = path[ix];
                int x = path[ix + 1];
                int y = path[ix + 2];
                int z = path[ix + 3];
                ix += 4;

                if (x < 4)
                {
                    x = (x - rotation & 3);
                }

                if (y < 4)
                {
                    y = (y - rotation & 3);
                }

                if (z < 4)
                {
                    z = (z - rotation & 3);
                }

                VertexX[j] = x;
                VertexY[j] = y;
                VertexZ[j] = z;
                if (type == 0)
                {
                    VertexColorA[j] = underlayHsls[x];
                    VertexColorB[j] = underlayHsls[y];
                    VertexColorC[j] = underlayHsls[z];
                    if (TriangleTextureIndex != null)
                    {
                        TriangleTextureIndex[j] = -1;
                    }
                }
                else
                {
                    VertexColorA[j] = overlayHsls[x];
                    VertexColorB[j] = overlayHsls[y];
                    VertexColorC[j] = overlayHsls[z];
                    if (TriangleTextureIndex != null)
                    {
                        TriangleTextureIndex[j] = textureIndex;
                    }
                }
            }
        }
    }
}
