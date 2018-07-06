using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RS
{
    public abstract class ModelAttachment
    {
        public int VertexIndex;

        public ModelAttachment(int vertexIndex)
        {
            this.VertexIndex = vertexIndex;
        }

        public virtual void Update(Model m)
        {

        }

        public virtual void CopyTo(Model m, GameObject go)
        {

        }

        public virtual void Destroy()
        {

        }
    }

    public class ParticleAttachment : ModelAttachment
    {
        private ParticleSystem component;
        public float StartSize = 0.5f;
        public ParticleSystemSimulationSpace SimulationSpace = ParticleSystemSimulationSpace.World;
        public Color StartColor = new Color(1, 0, 0);
        public float StartLifetime = 0.06f;
        public float GravityModifier = -6;
        public float StartSpeed = 1.0f;
        public float EmissionRate = 100.0f;

        public ParticleAttachment(int vertex) : base(vertex)
        {

        }

        public override void Update(Model m)
        {
            var pos = new Vector3(m.VertexX[VertexIndex] * GameConstants.RenderScale, -m.VertexY[VertexIndex] * GameConstants.RenderScale, m.VertexZ[VertexIndex] * GameConstants.RenderScale);
            component.transform.localPosition = pos;
        }

        public override void CopyTo(Model m, GameObject go)
        {
            var a = new ParticleAttachment(VertexIndex);
            a.component = Create(go);
            a.Update(m);
            m.Attachments.Add(a);
        }

        private ParticleSystem Create(GameObject go)
        {
            var ps = go.AddComponent<ParticleSystem>();
            ps.startSize = StartSize;
            ps.simulationSpace = ParticleSystemSimulationSpace.World;
            ps.startColor = StartColor;
            ps.startLifetime = StartLifetime;
            ps.gravityModifier = GravityModifier;
            ps.startSpeed = StartSpeed;

            var emission = ps.emission;
            emission.rate = new ParticleSystem.MinMaxCurve(EmissionRate);

            var shape = ps.shape;
            shape.enabled = false;
            return ps;
        }

        public override void Destroy()
        {
            GameObject.Destroy(component);
        }
    }

    public class LightAttachment : ModelAttachment
    {
        private Light component;


        public LightAttachment(int vertex) : base(vertex)
        {

        }

        public override void Update(Model m)
        {
            var pos = new Vector3(m.VertexX[VertexIndex] * GameConstants.RenderScale, -m.VertexY[VertexIndex] * GameConstants.RenderScale, m.VertexZ[VertexIndex] * GameConstants.RenderScale);
            component.transform.localPosition = pos;
        }

        public override void CopyTo(Model m, GameObject go)
        {
            var a = new LightAttachment(VertexIndex);
            a.component = Create(go);
            a.Update(m);
            m.Attachments.Add(a);
        }

        private Light Create(GameObject go)
        {
            var ps = go.AddComponent<Light>();
            ps.type = LightType.Point;
            ps.shadows = LightShadows.Soft;
            ps.shadowStrength = 0.6f;
            return ps;
        }

        public override void Destroy()
        {
            GameObject.Destroy(component);
        }
    }

    /// <summary>
    /// Holds meta-data about a model.
    /// </summary>
    public class ModelDescriptor
    {
        public byte[] buffer;
        public int alphaStreamPos;
        public int colorFlagStreamPos;
        public int priortyFlagStreamPos;
        public int alphaFlagStreamPos;
        public int tskinFlagStreamPos;
        public int triangleViewspacePos;
        public int triangleViewspaceTypePos;
        public int infoStreamPos;
        public int priortyStreamPos;
        public int textureMapStreamPos;
        public int texturedTriangleCount;
        public int typeFlagStreamPos;
        public int colorStreamPos;
        public int triangleCount;
        public int tskinStreamPos;
        public int vertexCount;
    }

    /// <summary>
    /// Represents a model.
    /// </summary>
    public class Model
    {
        private static byte[][] modelBufferCache;
        private static Cache cache;

        public static void Init(int cacheSize, Cache cache)
        {
            Model.modelBufferCache = new byte[90000][];
            Model.cache = cache;
        }

        public static int AnInt1681;
        public static int AnInt1682;
        public static int AnInt1683;
        public static int[] ReplaceVertexX = new int[2000];
        public static int[] ReplaceVertexY = new int[2000];
        public static int[] ReplaceVertexZ = new int[2000];
        public static int[] AnIntArray1625 = new int[2000];

        public int AnInt1641;
        public int[] TextureMapX;
        public int[] TextureMapY;
        public int[] TextureMapZ;
        public int TexturedTriangleCount;
        public int[] TriangleAlpha;
        public int[] TriangleColor;
        public int TriangleCount;
        public int[] TriangleInfo;
        public int[] TrianglePriority;
        public int[] TriangleTSkin;
        public int[] TriangleViewspaceA;
        public int[] TriangleViewspaceB;
        public int[] TriangleViewspaceC;
        public int VertexCount;
        public int[] VertexSkinTypes;
        public int[] VertexX;
        public int[] VertexY;
        public int[] VertexZ;
        public int[][] VertexWeights;
        public int[][] TriangleGroups;
        public int[] TriHsl1;
        public int[] TriHsl2;
        public int[] TriHsl3;
        public GameObject Backing;
        public PriorityVertex[] Normal;
        public PriorityVertex[] Vertices;
        private VNormal[] actualNormal;
        private VNormal[] actualNormal2;
        public int MaxHorizon;
        public int MaxX;
        public int MaxY;
        public int MaxZ;
        public int MinX;
        public int MinZ;
        public int Height;
        public int Unknown2;
        public int Unknown3;

        /// <summary>
        /// Normal X coordinate mappings.
        /// </summary>
        public int[] normalX;

        /// <summary>
        /// Normal Y coordinate mappings.
        /// </summary>
        public int[] normalY;

        /// <summary>
        /// Normal Z coordinate mappings.
        /// </summary>
        public int[] normalZ;

        /// <summary>
        /// A map of how many times each normal is referenced.
        /// </summary>
        public int[] normalUseCount;

        /// <summary>
        /// The total number of normals in this model.
        /// </summary>
        public int normalCount = 0;

        /// <summary>
        /// The scale of each triangle texture on the X coordinate.
        /// </summary>
        public int[] TextureScaleX;

        /// <summary>
        /// The scale of each triangle texture on the Y coordinate.
        /// </summary>
        public int[] TextureScaleY;

        /// <summary>
        /// The scale of each triangle texture on the Z coordinate.
        /// </summary>
        public int[] TextureScaleZ;
        sbyte[] TextureVertexPointers = null;
        sbyte[] TriangleTextureMapTypes = null;
        sbyte[] aByteArray2851 = null;
        sbyte[] aByteArray2870 = null;
        sbyte[] aByteArray2859 = null;
        sbyte[] TextureRotationY = null;
        sbyte[] aByteArray2888 = null;

        /// <summary>
        /// An array of texture ids for each triangle.
        /// </summary>
        public int[] TriangleTextures = null;

        /// <summary>
        /// The format of model to load.
        /// </summary>
        public int Format = 12;

        /// <summary>
        /// A cached mesh containing this model data.
        /// </summary>
        private Mesh lastMesh = null;

        /// <summary>
        /// A cached list of materials on this model.
        /// </summary>
        private Material[] lastMaterials;

        /// <summary>
        /// A list of attachments currently on this model.
        /// </summary>
        public List<ModelAttachment> Attachments = new List<ModelAttachment>();

        /// <summary>
        /// A list of attachments that have not been initialized yet.
        /// </summary>
        private List<ModelAttachment> preAttachments = new List<ModelAttachment>();

        /// <summary>
        /// A queue of attachments waiting to be added.
        /// </summary>
        private List<ModelAttachment> attachmentQueue = new List<ModelAttachment>();

        

        public Model()
        {

        }

        public void ConvertVertices()
        {

        }

        /// <summary>
        /// Inserts a normal into this model's normal data at the tail.
        /// 
        /// </summary>
        /// <param name="normalX">The normal X coordinate.</param>
        /// <param name="normalY">The normal Y coordinate.</param>
        /// <param name="normalZ">The normal Z coordinate.</param>
        /// <param name="refCount">The initial use count of the normal.</param>
        /// <returns>The index the normal was inserted at.</returns>
        public int InsertNormal(int normalX, int normalY, int normalZ, int refCount)
        {
            this.normalX[normalCount] = normalX;
            this.normalY[normalCount] = normalY;
            this.normalZ[normalCount] = normalZ;
            normalUseCount[normalCount] = refCount;
            return normalCount++;
        }

        /// <summary>
        /// Loads a model from v2 format.
        /// </summary>
        /// <param name="index">The index of the model being loaded.</param>
        public void LoadNewModel1(int index)
        {
            var data = modelBufferCache[index];
            var colorStream = new DefaultJagexBuffer(data);
            var typeStream = new DefaultJagexBuffer(data);
            var priorityStream = new DefaultJagexBuffer(data);
            var alphaStream = new DefaultJagexBuffer(data);
            var tskinStream = new DefaultJagexBuffer(data);
            var nc6 = new DefaultJagexBuffer(data);
            var nc7 = new DefaultJagexBuffer(data);
            colorStream.Position(data.Length - 23);
            VertexCount = colorStream.ReadUShort();
            TriangleCount = colorStream.ReadUShort();
            TexturedTriangleCount = colorStream.ReadUByte();

            var infoMask = colorStream.ReadUByte();
            var hasTriangleInfo = (infoMask & 1) == 1;
            var hasFormat = (infoMask & 8) == 8;
            if (hasFormat)
            {
                colorStream.Position(colorStream.Position() - 7);
                Format = colorStream.ReadUByte();
                colorStream.Position(colorStream.Position() + 6);
            }

            var priority = colorStream.ReadUByte();
            var hasAlpha = colorStream.ReadUByte();
            var hasSkins = colorStream.ReadUByte();
            var hasFaceTextures = colorStream.ReadUByte();
            var hasVertexSkins = colorStream.ReadUByte();
            var j3 = colorStream.ReadUShort();
            var k3 = colorStream.ReadUShort();
            var l3 = colorStream.ReadUShort();
            var i4 = colorStream.ReadUShort();
            var j4 = colorStream.ReadUShort();
            var simpleTextureFaceCount = 0;
            var complexTextureFaceCount = 0;
            var cubeTextureFaceCount = 0;

            TriangleColor = new int[TriangleCount];
            if (TexturedTriangleCount > 0)
            {
                TriangleTextureMapTypes = new sbyte[TexturedTriangleCount];
                colorStream.Position(0);
                for (int j5 = 0; j5 < TexturedTriangleCount; j5++)
                {
                    sbyte type = TriangleTextureMapTypes[j5] = (sbyte)colorStream.ReadByte();
                    if (type == 0)
                    {
                        simpleTextureFaceCount++;
                    }

                    if (type >= 1 && type <= 3)
                    {
                        complexTextureFaceCount++;
                    }

                    if (type == 2)
                    {
                        cubeTextureFaceCount++;
                    }
                }
            }

            var curPointer = TexturedTriangleCount;
            var l5 = curPointer;
            curPointer += VertexCount;
            int typeStreamPos = curPointer;
            if (hasTriangleInfo)
            {
                curPointer += TriangleCount;
            }

            var triangleViewspaceTypePos = curPointer;
            curPointer += TriangleCount;
            var priorityStreamPos = curPointer;
            if (priority == 255)
            {
                curPointer += TriangleCount;
            }

            var tskinStreamPos = curPointer;
            if (hasSkins == 1)
            {
                curPointer += TriangleCount;
            }

            var i7 = curPointer;
            if (hasVertexSkins == 1)
            {
                curPointer += VertexCount;
            }

            var alphaStreamPos = curPointer;
            if (hasAlpha == 1)
            {
                curPointer += TriangleCount;
            }

            var triangleViewspacePos = curPointer;
            curPointer += i4;
            var l7 = curPointer;
            if (hasFaceTextures == 1)
            {
                curPointer += TriangleCount * 2;
            }

            var i8 = curPointer;
            curPointer += j4;
            var colorStreamPos = curPointer;
            curPointer += TriangleCount * 2;
            var k8 = curPointer;
            curPointer += j3;
            var l8 = curPointer;
            curPointer += k3;
            var i9 = curPointer;
            curPointer += l3;
            var fuckmyass = curPointer;
            curPointer += simpleTextureFaceCount * 6;
            var k9 = curPointer;
            curPointer += complexTextureFaceCount * 6;
            var i_59_ = 6;

            if (Format != 14)
            {
                if (Format >= 15)
                {
                    i_59_ = 9;
                }

            }
            else
            {
                i_59_ = 7;
            }

            var l9 = curPointer;
            curPointer += i_59_ * complexTextureFaceCount;
            var i10 = curPointer;
            curPointer += complexTextureFaceCount;
            var j10 = curPointer;
            curPointer += complexTextureFaceCount;
            var k10 = curPointer;
            curPointer += complexTextureFaceCount + (cubeTextureFaceCount * 2);

            VertexX = new int[VertexCount];
            VertexY = new int[VertexCount];
            VertexZ = new int[VertexCount];

            TriangleViewspaceA = new int[TriangleCount];
            TriangleViewspaceB = new int[TriangleCount];
            TriangleViewspaceC = new int[TriangleCount];

            VertexSkinTypes = new int[VertexCount];

            TriangleInfo = new int[TriangleCount];
            TrianglePriority = new int[TriangleCount];
            TriangleAlpha = new int[TriangleCount];
            TriangleTSkin = new int[TriangleCount];

            if (hasVertexSkins == 1)
            {
                VertexSkinTypes = new int[VertexCount];
            }

            if (hasTriangleInfo)
            {
                TriangleInfo = new int[TriangleCount];
            }

            if (priority == 255)
            {
                TrianglePriority = new int[TriangleCount];
            }

            if (hasAlpha == 1)
            {
                TriangleAlpha = new int[TriangleCount];
            }

            if (hasSkins == 1)
            {
                TriangleTSkin = new int[TriangleCount];
            }

            if (hasFaceTextures == 1)
            {
                TriangleTextures = new int[TriangleCount];
            }

            if (hasFaceTextures == 1)
            {
                TextureVertexPointers = new sbyte[TriangleCount];
            }

            TriangleColor = new int[TriangleCount];
            if (TexturedTriangleCount > 0)
            {
                TextureMapX = new int[TexturedTriangleCount];
                TextureMapY = new int[TexturedTriangleCount];
                TextureMapZ = new int[TexturedTriangleCount];

                if (complexTextureFaceCount > 0)
                {
                    TextureScaleX = new int[complexTextureFaceCount];
                    TextureScaleY = new int[complexTextureFaceCount];
                    TextureScaleZ = new int[complexTextureFaceCount];
                    TextureRotationY = new sbyte[complexTextureFaceCount];
                    aByteArray2888 = new sbyte[complexTextureFaceCount];
                    aByteArray2870 = new sbyte[complexTextureFaceCount];
                }
                if (cubeTextureFaceCount > 0)
                {
                    aByteArray2859 = new sbyte[cubeTextureFaceCount];
                    aByteArray2851 = new sbyte[cubeTextureFaceCount];
                }
            }
            colorStream.Position(l5);
            typeStream.Position(k8);
            priorityStream.Position(l8);
            alphaStream.Position(i9);
            tskinStream.Position(i7);

            var lastX = 0;
            var lastY = 0;
            var lastZ = 0;
            for (var v = 0; v < VertexCount; v++)
            {
                var mask = colorStream.ReadUByte();

                var offX = 0;
                if ((mask & 1) != 0)
                {
                    offX = typeStream.ReadSmart();
                }

                var offY = 0;
                if ((mask & 2) != 0)
                {
                    offY = priorityStream.ReadSmart();
                }

                var offZ = 0;
                if ((mask & 4) != 0)
                {
                    offZ = alphaStream.ReadSmart();
                }

                VertexX[v] = lastX + offX;
                VertexY[v] = lastY + offY;
                VertexZ[v] = lastZ + offZ;
                lastX = VertexX[v];
                lastY = VertexY[v];
                lastZ = VertexZ[v];
                if (hasVertexSkins == 1)
                {
                    VertexSkinTypes[v] = tskinStream.ReadUByte();
                }
            }

            colorStream.Position(colorStreamPos);
            typeStream.Position(typeStreamPos);
            priorityStream.Position(priorityStreamPos);
            alphaStream.Position(alphaStreamPos);
            tskinStream.Position(tskinStreamPos);
            nc6.Position(l7);
            nc7.Position(i8);
            for (var i = 0; i < TriangleCount; i++)
            {
                TriangleColor[i] = colorStream.ReadUShort();
                if (hasTriangleInfo)
                {
                    TriangleInfo[i] = (sbyte)typeStream.ReadByte();
                }

                if (priority == 255)
                {
                    TrianglePriority[i] = (sbyte)priorityStream.ReadByte();
                }

                if (hasAlpha == 1)
                {
                    TriangleAlpha[i] = alphaStream.ReadUByte();
                }

                if (hasSkins == 1)
                {
                    TriangleTSkin[i] = tskinStream.ReadUByte();
                }

                if (hasFaceTextures == 1)
                {
                    TriangleTextures[i] = (short)(nc6.ReadUShort() - 1);
                }

                if (TextureVertexPointers != null)
                {
                    if (TriangleTextures[i] != -1)
                    {
                        TextureVertexPointers[i] = (sbyte)(nc7.ReadUByte() - 1);
                    }
                    else
                    {
                        TextureVertexPointers[i] = -1;
                    }
                }
            }

            colorStream.Position(triangleViewspacePos);
            typeStream.Position(triangleViewspaceTypePos);
            var x = 0;
            var y = 0;
            var z = 0;
            var last = 0;
            for (int i = 0; i < TriangleCount; i++)
            {
                var type = typeStream.ReadUByte();
                if (type == 1)
                {
                    x = colorStream.ReadSmart() + last;
                    last = x;

                    y = colorStream.ReadSmart() + last;
                    last = y;

                    z = colorStream.ReadSmart() + last;
                    last = z;

                    TriangleViewspaceA[i] = x;
                    TriangleViewspaceB[i] = y;
                    TriangleViewspaceC[i] = z;
                } else if (type == 2)
                {
                    y = z;
                    z = colorStream.ReadSmart() + last;
                    last = z;

                    TriangleViewspaceA[i] = x;
                    TriangleViewspaceB[i] = y;
                    TriangleViewspaceC[i] = z;
                } else if (type == 3)
                {
                    x = z;
                    z = colorStream.ReadSmart() + last;
                    last = z;

                    TriangleViewspaceA[i] = x;
                    TriangleViewspaceB[i] = y;
                    TriangleViewspaceC[i] = z;
                } else if (type == 4)
                {
                    int l14 = x;
                    x = y;
                    y = l14;
                    z = colorStream.ReadSmart() + last;
                    last = z;
                    TriangleViewspaceA[i] = x;
                    TriangleViewspaceB[i] = y;
                    TriangleViewspaceC[i] = z;
                }
            }

            colorStream.Position(fuckmyass);
            typeStream.Position(k9);
            priorityStream.Position(l9);
            alphaStream.Position(i10);
            tskinStream.Position(j10);
            nc6.Position(k10);
            for (var k14 = 0; k14 < TexturedTriangleCount; k14++)
            {
                var type = TriangleTextureMapTypes[k14] & 0xff;
                if (type == 0)
                {
                    TextureMapX[k14] = (short)colorStream.ReadUShort();
                    TextureMapY[k14] = (short)colorStream.ReadUShort();
                    TextureMapZ[k14] = (short)colorStream.ReadUShort();
                } else if (type == 1)
                {
                    TextureMapX[k14] = (short)typeStream.ReadUShort();
                    TextureMapY[k14] = (short)typeStream.ReadUShort();
                    TextureMapZ[k14] = (short)typeStream.ReadUShort();
                    if (Format < 15)
                    {
                        TextureScaleX[k14] = priorityStream.ReadUShort();
                        if (Format >= 14)
                        {
                            TextureScaleY[k14] = priorityStream.ReadTriByte();
                        }
                        else
                        {
                            TextureScaleY[k14] = priorityStream.ReadUShort();
                        }
                        TextureScaleZ[k14] = priorityStream.ReadUShort();
                    }
                    else
                    {
                        TextureScaleX[k14] = priorityStream.ReadTriByte();
                        TextureScaleY[k14] = priorityStream.ReadTriByte();
                        TextureScaleZ[k14] = priorityStream.ReadTriByte();
                    }

                    TextureRotationY[k14] = (sbyte)alphaStream.ReadByte();
                    aByteArray2888[k14] = (sbyte)tskinStream.ReadByte();
                    aByteArray2870[k14] = (sbyte)nc6.ReadByte();
                } else if (type == 2)
                {
                    TextureMapX[k14] = (short)typeStream.ReadUShort();
                    TextureMapY[k14] = (short)typeStream.ReadUShort();
                    TextureMapZ[k14] = (short)typeStream.ReadUShort();
                    if (Format >= 15)
                    {
                        TextureScaleX[k14] = priorityStream.ReadTriByte();
                        TextureScaleY[k14] = priorityStream.ReadTriByte();
                        TextureScaleZ[k14] = priorityStream.ReadTriByte();
                    }
                    else
                    {
                        TextureScaleX[k14] = priorityStream.ReadUShort();
                        if (Format < 14)
                        {
                            TextureScaleY[k14] = priorityStream.ReadUShort();
                        }
                        else
                        {
                            TextureScaleY[k14] = priorityStream.ReadTriByte();
                        }
                        TextureScaleZ[k14] = priorityStream.ReadUShort();
                    }
                    TextureRotationY[k14] = (sbyte)alphaStream.ReadByte();
                    aByteArray2888[k14] = (sbyte)tskinStream.ReadByte();
                    aByteArray2870[k14] = (sbyte)nc6.ReadByte();
                    aByteArray2859[k14] = (sbyte)nc6.ReadByte();
                    aByteArray2851[k14] = (sbyte)nc6.ReadByte();
                } else if (type == 3)
                {
                    TextureMapX[k14] = (short)typeStream.ReadUShort();
                    TextureMapY[k14] = (short)typeStream.ReadUShort();
                    TextureMapZ[k14] = (short)typeStream.ReadUShort();
                    if (Format < 15)
                    {
                        TextureScaleX[k14] = priorityStream.ReadUShort();
                        if (Format < 14)
                        {
                            TextureScaleY[k14] = priorityStream.ReadUShort();
                        }
                        else
                        {
                            TextureScaleY[k14] = priorityStream.ReadTriByte();
                        }
                        TextureScaleZ[k14] = priorityStream.ReadUShort();
                    }
                    else
                    {
                        TextureScaleX[k14] = priorityStream.ReadTriByte();
                        TextureScaleY[k14] = priorityStream.ReadTriByte();
                        TextureScaleZ[k14] = priorityStream.ReadTriByte();
                    }

                    TextureRotationY[k14] = (sbyte)alphaStream.ReadByte();
                    aByteArray2888[k14] = (sbyte)tskinStream.ReadByte();
                    aByteArray2870[k14] = (sbyte)nc6.ReadByte();
                }
            }

            if (Format > 13)
            {
                ScaleDivide(4);
            }

            FilterTriangles();
        }

        public void LoadOldModel(int index)
        {
            var hasValidTriangleInfo = false;
            var hasValidTriangleTexture = false;

            var data = modelBufferCache[index];
            var buffer = new DefaultJagexBuffer(data);
            buffer.Position(data.Length - 18);

            var vertexCount = buffer.ReadUShort();
            var triangleCount = buffer.ReadUShort();
            var texturedTriangleCount = buffer.ReadUByte();
            

            var hasTextures = buffer.ReadUByte();
            var priority = buffer.ReadUByte();
            var hasAlpha = buffer.ReadUByte();
            var hasSkins = buffer.ReadUByte();
            var hasVertexSkins = buffer.ReadUByte();
            var xDataLength = buffer.ReadUShort();
            var yDataLength = buffer.ReadUShort();
            var zDataLength = buffer.ReadUShort();
            var triDataLength = buffer.ReadUShort();

            var position = 0;
            var colorFlagStreamPos = position;

            position += vertexCount;
            var triangleViewspaceTypePos = position;

            position += triangleCount;
            var priortyStreamPos = position;

            if (priority == 255)
            {
                position += triangleCount;
            }
            else
            {
                priortyStreamPos = -priority - 1;
            }

            var tskinStreamPos = position;

            if (hasSkins == 1)
            {
                position += triangleCount;
            }
            else
            {
                tskinStreamPos = -1;
            }

            var infoStreamPos = position;

            if (hasTextures == 1)
            {
                position += triangleCount;
            }
            else
            {
                infoStreamPos = -1;
            }

            var tskinFlagStreamPos = position;

            if (hasVertexSkins == 1)
            {
                position += vertexCount;
            }
            else
            {
                tskinFlagStreamPos = -1;
            }

            var alphaStreamPos = position;

            if (hasAlpha == 1)
            {
                position += triangleCount;
            }
            else
            {
                alphaStreamPos = -1;
            }

            var triangleViewspacePos = position;

            position += triDataLength;
            var colorStreamPos = position;

            position += triangleCount * 2;
            var textureMapStreamPos = position;

            position += texturedTriangleCount * 6;
            var typeFlagStreamPos = position;

            position += xDataLength;
            var priortyFlagStreamPos = position;

            position += yDataLength;
            var alphaFlagStreamPos = position;

            VertexCount = vertexCount;
            TriangleCount = triangleCount;
            TexturedTriangleCount = texturedTriangleCount;
            if (TexturedTriangleCount > 0)
            {
                TriangleTextureMapTypes = new sbyte[TexturedTriangleCount];
            }

            VertexX = new int[VertexCount];
            VertexY = new int[VertexCount];
            VertexZ = new int[VertexCount];

            TriangleViewspaceA = new int[TriangleCount];
            TriangleViewspaceB = new int[TriangleCount];
            TriangleViewspaceC = new int[TriangleCount];

            TextureMapX = new int[TexturedTriangleCount];
            TextureMapY = new int[TexturedTriangleCount];
            TextureMapZ = new int[TexturedTriangleCount];

            if (tskinFlagStreamPos >= 0)
            {
                VertexSkinTypes = new int[VertexCount];
            }

            if (infoStreamPos >= 0)
            {
                TriangleInfo = new int[TriangleCount];
            }

            if (priortyStreamPos >= 0)
            {
                TrianglePriority = new int[TriangleCount];
            }
            else
            {
                AnInt1641 = -priortyStreamPos - 1;
            }

            if (alphaStreamPos >= 0)
            {
                TriangleAlpha = new int[TriangleCount];
            }

            if (tskinStreamPos >= 0)
            {
                TriangleTSkin = new int[TriangleCount];
            }

            TriangleColor = new int[TriangleCount];

            var colorStream = new DefaultJagexBuffer(data);
            colorStream.Position(colorFlagStreamPos);

            var typeStream = new DefaultJagexBuffer(data);
            typeStream.Position(typeFlagStreamPos);

            var priorityStream = new DefaultJagexBuffer(data);
            priorityStream.Position(priortyFlagStreamPos);

            var alphaStream = new DefaultJagexBuffer(data);
            alphaStream.Position(alphaFlagStreamPos);

            var tskinStream = new DefaultJagexBuffer(data);
            tskinStream.Position(tskinFlagStreamPos);

            var ifx = 0;
            var ify = 0;
            var ifz = 0;
            for (var v = 0; v < VertexCount; v++)
            {
                var flag = colorStream.ReadUByte();
                var offX = 0;
                var offY = 0;
                var offZ = 0;

                if ((flag & 1) != 0)
                {
                    offX = typeStream.ReadSmart();
                }

                if ((flag & 2) != 0)
                {
                    offY = priorityStream.ReadSmart();
                }

                if ((flag & 4) != 0)
                {
                    offZ = alphaStream.ReadSmart();
                }

                VertexX[v] = ifx + offX;
                VertexY[v] = ify + offY;
                VertexZ[v] = ifz + offZ;
                ifx = VertexX[v];
                ify = VertexY[v];
                ifz = VertexZ[v];

                if (VertexSkinTypes != null)
                {
                    VertexSkinTypes[v] = tskinStream.ReadUByte();
                }
            }

            colorStream.Position(colorStreamPos);
            typeStream.Position(infoStreamPos);
            priorityStream.Position(priortyStreamPos);
            alphaStream.Position(alphaStreamPos);
            tskinStream.Position(tskinStreamPos);

            for (var i = 0; i < TriangleCount; i++)
            {
                TriangleColor[i] = colorStream.ReadUShort();
                if (hasTextures == 1)
                {
                    var renderFlag = typeStream.ReadUByte();
                    if ((renderFlag & 1) == 1)
                    {
                        TriangleInfo[i] = 1;
                        hasValidTriangleInfo = true;
                    }
                    else
                    {
                        TriangleInfo[i] = 0;
                    }

                    if ((renderFlag & 2) == 1)
                    {
                        TextureVertexPointers[i] = (sbyte)(renderFlag >> 2);
                        TriangleTextures[i] = TriangleColor[i];
                        TriangleColor[i] = 127;
                        if (TriangleTextures[i] != -1)
                        {
                            hasValidTriangleTexture = true;
                        }
                    }
                    else
                    {
                        if (TextureVertexPointers != null)
                            TextureVertexPointers[i] = -1;
                        if (TriangleTextures != null)
                            TriangleTextures[i] = -1;
                    }
                }

                if (TrianglePriority != null)
                {
                    TrianglePriority[i] = priorityStream.ReadUByte();
                }

                if (TriangleAlpha != null)
                {
                    TriangleAlpha[i] = alphaStream.ReadUByte();
                }

                if (TriangleTSkin != null)
                {
                    TriangleTSkin[i] = tskinStream.ReadUByte();
                }
            }

            colorStream.Position(triangleViewspacePos);
            typeStream.Position(triangleViewspaceTypePos);

            var x = 0;
            var y = 0;
            var z = 0;
            var last = 0;

            for (var i = 0; i < TriangleCount; i++)
            {
                var type = typeStream.ReadUByte();
                if (type == 1)
                {
                    x = colorStream.ReadSmart() + last;
                    last = x;

                    y = colorStream.ReadSmart() + last;
                    last = y;

                    z = colorStream.ReadSmart() + last;
                    last = z;

                    TriangleViewspaceA[i] = x;
                    TriangleViewspaceB[i] = y;
                    TriangleViewspaceC[i] = z;
                } else if (type == 2)
                {
                    y = z;

                    z = colorStream.ReadSmart() + last;
                    last = z;

                    TriangleViewspaceA[i] = x;
                    TriangleViewspaceB[i] = y;
                    TriangleViewspaceC[i] = z;
                } else if (type == 3)
                {
                    x = z;

                    z = colorStream.ReadSmart() + last;
                    last = z;

                    TriangleViewspaceA[i] = x;
                    TriangleViewspaceB[i] = y;
                    TriangleViewspaceC[i] = z;
                } else if (type == 4)
                {
                    var tmp = x;
                    x = y;
                    y = tmp;

                    z = colorStream.ReadSmart() + last;
                    last = z;

                    TriangleViewspaceA[i] = x;
                    TriangleViewspaceB[i] = y;
                    TriangleViewspaceC[i] = z;
                }
            }

            colorStream.Position(textureMapStreamPos);
            for (var i = 0; i < TexturedTriangleCount; i++)
            {
                TriangleTextureMapTypes[i] = 0;
                TextureMapX[i] = colorStream.ReadUShort();
                TextureMapY[i] = colorStream.ReadUShort();
                TextureMapZ[i] = colorStream.ReadUShort();
            }

            if (TextureVertexPointers != null)
            {
                var hasValidTextureVertPtrs = false;
                for (int i = 0; i < triangleCount; i++)
                {
                    var tmi = TextureVertexPointers[i] & 0xff;
                    if (tmi != 255)
                    {
                        if ((TextureMapX[tmi] & 0xffff) == TriangleViewspaceA[i] 
                            && (TextureMapY[tmi] & 0xffff) == TriangleViewspaceB[i] 
                            && (TextureMapZ[tmi] & 0xffff) == TriangleViewspaceC[i])
                        {
                            TextureVertexPointers[i] = -1;
                        }
                        else
                        {
                            hasValidTextureVertPtrs = true;
                        }
                    }
                }
                if (!hasValidTextureVertPtrs)
                {
                    TextureVertexPointers = null;
                }
            }

            if (!hasValidTriangleTexture)
            {
                TriangleTextures = null;
            }
            if (!hasValidTriangleInfo)
            {
                TriangleInfo = null;
            }
        }

        public Model(int index)
        {
            if (modelBufferCache[index] == null)
            {
                modelBufferCache[index] = cache.ReadCompressed(1, index);
            }

            if (modelBufferCache[index] == null)
            {
                return;
            }

            LoadOldModel(index);
        }

        public void FilterTriangles()
        {
            for (int triangleId = 0; triangleId < TriangleCount; triangleId++)
            {
                int l = TriangleViewspaceA[triangleId];
                int k1 = TriangleViewspaceB[triangleId];
                int j2_ = TriangleViewspaceC[triangleId];
                var b = true;
                for (int triId = 0; triId < TriangleCount; triId++)
                {
                    if (triId == triangleId)
                    {
                        continue;
                    }

                    if (TriangleViewspaceA[triId] == l)
                    {
                        b = false;
                        break;
                    }

                    if (TriangleViewspaceB[triId] == k1)
                    {
                        b = false;
                        break;
                    }

                    if (TriangleViewspaceC[triId] == j2_)
                    {
                        b = false;
                        break;
                    }
                }

                if (b)
                {
                    if (TriangleInfo != null)
                    {
                        TriangleAlpha[triangleId] = 255;
                    }
                }
            }
        }

        public Model(bool copyYVertices, bool copyShading, Model m)
        {
            VertexCount = m.VertexCount;
            TriangleCount = m.TriangleCount;

            TexturedTriangleCount = m.TexturedTriangleCount;
            TriangleTextureMapTypes = m.TriangleTextureMapTypes;
            TextureScaleX = m.TextureScaleX;
            TextureScaleY = m.TextureScaleY;
            TextureScaleZ = m.TextureScaleZ;
            TriangleTextures = m.TriangleTextures;
            TextureMapX = m.TextureMapX;
            TextureMapY = m.TextureMapY;
            TextureMapZ = m.TextureMapZ;
            TextureRotationY = m.TextureRotationY;
            aByteArray2851 = m.aByteArray2851;
            aByteArray2859 = m.aByteArray2859;
            aByteArray2870 = m.aByteArray2870;
            aByteArray2888 = m.aByteArray2888;
            TextureVertexPointers = m.TextureVertexPointers;

            if (copyYVertices)
            {
                VertexY = new int[VertexCount];
                for (int j = 0; j < VertexCount; j++)
                {
                    VertexY[j] = m.VertexY[j];
                }
            }
            else
            {
                VertexY = m.VertexY;
            }

            if (copyShading)
            {
                TriangleInfo = new int[TriangleCount];

                if (m.TriangleInfo == null)
                {
                    for (int i = 0; i < TriangleCount; i++)
                    {
                        TriangleInfo[i] = 0;
                    }
                }
                else
                {
                    for (int i = 0; i < TriangleCount; i++)
                    {
                        TriangleInfo[i] = m.TriangleInfo[i];
                    }
                }
            }
            else
            {
                TriangleInfo = m.TriangleInfo;
            }

            VertexX = m.VertexX;
            VertexZ = m.VertexZ;
            TriangleColor = m.TriangleColor;
            TriangleAlpha = m.TriangleAlpha;
            TrianglePriority = m.TrianglePriority;
            AnInt1641 = m.AnInt1641;
            TriangleViewspaceA = m.TriangleViewspaceA;
            TriangleViewspaceB = m.TriangleViewspaceB;
            TriangleViewspaceC = m.TriangleViewspaceC;
        }

        public Model(bool copyColors, bool copyOpacity, bool copyVertices, Model m)
        {
            VertexCount = m.VertexCount;
            TriangleCount = m.TriangleCount;
            TriangleTextures = m.TriangleTextures;

            if (copyVertices)
            {
                VertexX = m.VertexX;
                VertexY = m.VertexY;
                VertexZ = m.VertexZ;
            }
            else
            {
                VertexX = new int[VertexCount];
                VertexY = new int[VertexCount];
                VertexZ = new int[VertexCount];
                for (int i = 0; i < VertexCount; i++)
                {
                    VertexX[i] = m.VertexX[i];
                    VertexY[i] = m.VertexY[i];
                    VertexZ[i] = m.VertexZ[i];
                }
            }

            if (copyColors)
            {
                TriangleColor = m.TriangleColor;
            }
            else
            {
                TriangleColor = new int[TriangleCount];

                for (int i = 0; i < TriangleCount; i++)
                {
                    TriangleColor[i] = m.TriangleColor[i];
                }
            }

            if (copyOpacity)
            {
                TriangleAlpha = m.TriangleAlpha;
            }
            else
            {
                TriangleAlpha = new int[TriangleCount];

                if (m.TriangleAlpha == null)
                {
                    for (int i = 0; i < TriangleCount; i++)
                    {
                        TriangleAlpha[i] = 0;
                    }
                }
                else
                {
                    for (int i = 0; i < TriangleCount; i++)
                    {
                        TriangleAlpha[i] = m.TriangleAlpha[i];
                    }
                }
            }

            VertexSkinTypes = m.VertexSkinTypes;
            TriangleTSkin = m.TriangleTSkin;
            TriangleInfo = m.TriangleInfo;
            TriangleViewspaceA = m.TriangleViewspaceA;
            TriangleViewspaceB = m.TriangleViewspaceB;
            TriangleViewspaceC = m.TriangleViewspaceC;
            TrianglePriority = m.TrianglePriority;
            AnInt1641 = m.AnInt1641;
            TextureMapX = m.TextureMapX;
            TextureMapY = m.TextureMapY;
            TextureMapZ = m.TextureMapZ;
            TextureRotationY = m.TextureRotationY;
            aByteArray2851 = m.aByteArray2851;
            aByteArray2859 = m.aByteArray2859;
            aByteArray2870 = m.aByteArray2870;
            aByteArray2888 = m.aByteArray2888;
            TexturedTriangleCount = m.TexturedTriangleCount;
            TriangleTextureMapTypes = m.TriangleTextureMapTypes;
            TextureScaleX = m.TextureScaleX;
            TextureScaleY = m.TextureScaleY;
            TextureScaleZ = m.TextureScaleZ;
            TextureVertexPointers = m.TextureVertexPointers;
        }


        public Model(int count, Model[] models)
        {
            bool hasInfo = false;
            bool hasPriorities = false;
            bool hasAlpha = false;
            bool hasTSkins = false;
            bool hasTexMappings = false;
            bool hasFaceTextures = false;
            VertexCount = 0;
            TriangleCount = 0;
            TexturedTriangleCount = 0;
            AnInt1641 = -1;

            for (int i = 0; i < count; i++)
            {
                Model m = models[i];
                if (m != null)
                {
                    foreach (var a in m.Attachments)
                    {
                        preAttachments.Add(a);
                    }
                    TexturedTriangleCount += m.TexturedTriangleCount;
                    VertexCount += m.VertexCount;
                    TriangleCount += m.TriangleCount;
                    TexturedTriangleCount += m.TexturedTriangleCount;
                    hasInfo |= m.TriangleInfo != null;
                    hasTexMappings |= m.TextureVertexPointers != null;
                    hasFaceTextures |= m.TriangleTextures != null;
                    if (m.TrianglePriority != null)
                    {
                        hasPriorities = true;
                    }
                    else
                    {
                        if (AnInt1641 == -1)
                        {
                            AnInt1641 = m.AnInt1641;
                        }
                        if (AnInt1641 != m.AnInt1641)
                        {
                            hasPriorities = true;
                        }
                    }

                    hasAlpha = m.TriangleAlpha != null;
                    hasTSkins = m.TriangleTSkin != null;
                }
            }

            VertexX = new int[VertexCount];
            VertexY = new int[VertexCount];
            VertexZ = new int[VertexCount];
            VertexSkinTypes = new int[VertexCount];

            TriangleViewspaceA = new int[TriangleCount];
            TriangleViewspaceB = new int[TriangleCount];
            TriangleViewspaceC = new int[TriangleCount];

            TextureMapX = new int[TexturedTriangleCount];
            TextureMapY = new int[TexturedTriangleCount];
            TextureMapZ = new int[TexturedTriangleCount];

            if (hasTexMappings)
            {
                TextureVertexPointers = new sbyte[TriangleCount];
            }

            if (hasFaceTextures)
            {
                TriangleTextures = new int[TriangleCount];
            }
            if (hasInfo)
            {
                TriangleInfo = new int[TriangleCount];
            }

            if (hasPriorities)
            {
                TrianglePriority = new int[TriangleCount];
            }

            if (hasAlpha)
            {
                TriangleAlpha = new int[TriangleCount];
            }

            if (hasTSkins)
            {
                TriangleTSkin = new int[TriangleCount];
            }

            if (TexturedTriangleCount > 0)
            {
                TriangleTextureMapTypes = new sbyte[TexturedTriangleCount];
                TextureMapX = new int[TexturedTriangleCount];
                TextureMapY = new int[TexturedTriangleCount];
                TextureMapZ = new int[TexturedTriangleCount];
                TextureScaleX = new int[TexturedTriangleCount];
                TextureScaleY = new int[TexturedTriangleCount];
                TextureScaleZ = new int[TexturedTriangleCount];
                TextureRotationY = new sbyte[TexturedTriangleCount];
                aByteArray2888 = new sbyte[TexturedTriangleCount];
                aByteArray2870 = new sbyte[TexturedTriangleCount];
                aByteArray2859 = new sbyte[TexturedTriangleCount];
                aByteArray2851 = new sbyte[TexturedTriangleCount];
            }

            TriangleColor = new int[TriangleCount];
            VertexCount = 0;
            TriangleCount = 0;
            TexturedTriangleCount = 0;

            int l = 0;
            for (int i = 0; i < count; i++)
            {
                Model m = models[i];
                if (m != null)
                {
                    for (int j = 0; j < m.TriangleCount; j++)
                    {
                        if (hasInfo && m.TriangleInfo != null)
                        {
                            TriangleInfo[TriangleCount] = m.TriangleInfo[j];
                        }

                        if (hasPriorities)
                        {
                            if (m.TrianglePriority == null)
                            {
                                TrianglePriority[TriangleCount] = m.AnInt1641;
                            }
                            else
                            {
                                TrianglePriority[TriangleCount] = m.TrianglePriority[j];
                            }
                        }

                        if (hasAlpha && m.TriangleAlpha != null)
                        {
                            TriangleAlpha[TriangleCount] = m.TriangleAlpha[j];
                        }

                        if (hasTSkins && m.TriangleTSkin != null)
                        {
                            TriangleTSkin[TriangleCount] = m.TriangleTSkin[j];
                        }

                        if (hasFaceTextures)
                        {
                            if (m.TriangleTextures != null)
                            {
                                TriangleTextures[TriangleCount] = m.TriangleTextures[j];
                            }
                            else
                            {
                                TriangleTextures[TriangleCount] = -1;
                            }
                        }

                        TriangleColor[TriangleCount] = m.TriangleColor[j];
                        TriangleViewspaceA[TriangleCount] = InsertVertexFrom(m, m.TriangleViewspaceA[j]);
                        TriangleViewspaceB[TriangleCount] = InsertVertexFrom(m, m.TriangleViewspaceB[j]);
                        TriangleViewspaceC[TriangleCount] = InsertVertexFrom(m, m.TriangleViewspaceC[j]);

                        TriangleCount++;
                    }
                }
            }

            int off = 0;
            for (int i = 0; count > i; i++)
            {
                short uid = (short)(1 << i);
                Model m = models[i];
                if (m != null)
                {
                    for (int tc = 0; m.TriangleCount > tc; tc++)
                    {
                        if (hasTexMappings)
                        {
                            TextureVertexPointers[off++] = (sbyte)((m.TextureVertexPointers != null && (m.TextureVertexPointers[tc] ^ 0xffffffff) != 0) ? (TexturedTriangleCount + m.TextureVertexPointers[tc]) : -1);
                        }
                    }

                    for (int j = 0; j < m.TexturedTriangleCount; j++)
                    {
                        sbyte type = (TriangleTextureMapTypes[TexturedTriangleCount] = m.TriangleTextureMapTypes[j]);
                        if (type == 0)
                        {
                            TextureMapX[TexturedTriangleCount] = InsertVertexFrom(m, m.TextureMapX[j]);
                            TextureMapY[TexturedTriangleCount] = InsertVertexFrom(m, m.TextureMapY[j]);
                            TextureMapZ[TexturedTriangleCount] = InsertVertexFrom(m, m.TextureMapZ[j]);
                        }
                        if (type >= 1 && type <= 3)
                        {
                            TextureMapX[TexturedTriangleCount] = m.TextureMapX[j];
                            TextureMapY[TexturedTriangleCount] = m.TextureMapY[j];
                            TextureMapZ[TexturedTriangleCount] = m.TextureMapZ[j];
                            TextureScaleX[TexturedTriangleCount] = m.TextureScaleX[j];
                            TextureScaleY[TexturedTriangleCount] = m.TextureScaleY[j];
                            TextureScaleZ[TexturedTriangleCount] = m.TextureScaleZ[j];
                            TextureRotationY[TexturedTriangleCount] = m.TextureRotationY[j];
                            aByteArray2888[TexturedTriangleCount] = m.aByteArray2888[j];
                            aByteArray2870[TexturedTriangleCount] = m.aByteArray2870[j];
                        }
                        if (type == 2)
                        {
                            aByteArray2859[TexturedTriangleCount] = m.aByteArray2859[j];
                            aByteArray2851[TexturedTriangleCount] = m.aByteArray2851[j];
                        }
                        TexturedTriangleCount++;
                    }
                }
            }
        }

        public void ClearAttachments()
        {
            foreach (var a in Attachments)
            {
                a.Destroy();
            }
            Attachments.Clear();
            attachmentQueue.Clear();
        }

        public int InsertVertexFrom(Model model, int vertex)
        {
            var x = model.VertexX[vertex];
            var y = model.VertexY[vertex];
            var z = model.VertexZ[vertex];

            for (var i = 0; i < VertexCount; i++)
            {
                if (x == VertexX[i] && y == VertexY[i] && z == VertexZ[i])
                {
                    return i;
                }
            }


            VertexX[VertexCount] = x;
            VertexY[VertexCount] = y;
            VertexZ[VertexCount] = z;

            for (int i = 0; i < preAttachments.Count; i++)
            {
                var a = preAttachments[i];
                if (a.VertexIndex == vertex)
                {
                    a.VertexIndex = VertexCount;
                    preAttachments.Remove(a);
                    attachmentQueue.Add(a);
                    i -= 1;
                }
            }

            if (model.VertexSkinTypes != null)
            {
                VertexSkinTypes[VertexCount] = model.VertexSkinTypes[vertex];
            }

            return VertexCount++; ;
        }

        public void SetPitch(int pitch)
        {
            int sin = MathUtils.Sin[pitch];
            int cos = MathUtils.Cos[pitch];
            for (int i = 0; i < VertexCount; i++)
            {
                int newY = VertexY[i] * cos - VertexZ[i] * sin >> 16;
                VertexZ[i] = (VertexY[i] * sin + VertexZ[i] * cos >> 16);
                VertexY[i] = newY;
            }
        }

        public void Replace(Model m, bool copyAlpha)
        {
            VertexCount = m.VertexCount;
            TriangleCount = m.TriangleCount;
            TexturedTriangleCount = m.TexturedTriangleCount;
            Attachments = m.Attachments;

            if (ReplaceVertexX.Length < VertexCount)
            {
                ReplaceVertexX = new int[VertexCount + 100];
                ReplaceVertexY = new int[VertexCount + 100];
                ReplaceVertexZ = new int[VertexCount + 100];
            }

            VertexX = ReplaceVertexX;
            VertexY = ReplaceVertexY;
            VertexZ = ReplaceVertexZ;

            for (int k = 0; k < VertexCount; k++)
            {
                VertexX[k] = m.VertexX[k];
                VertexY[k] = m.VertexY[k];
                VertexZ[k] = m.VertexZ[k];
            }

            if (copyAlpha)
            {
                TriangleAlpha = m.TriangleAlpha;
            }
            else
            {
                if (AnIntArray1625.Length < TriangleCount)
                {
                    AnIntArray1625 = new int[TriangleCount + 100];
                }
                TriangleAlpha = AnIntArray1625;

                if (m.TriangleAlpha == null)
                {
                    for (var l = 0; l < TriangleCount; l++)
                    {
                        TriangleAlpha[l] = 0;
                    }
                }
                else
                {
                    for (var i1 = 0; i1 < TriangleCount; i1++)
                    {
                        TriangleAlpha[i1] = m.TriangleAlpha[i1];
                    }
                }
            }

            TriangleInfo = m.TriangleInfo;
            TriangleColor = m.TriangleColor;
            TrianglePriority = m.TrianglePriority;
            AnInt1641 = m.AnInt1641;
            TriangleGroups = m.TriangleGroups;
            VertexWeights = m.VertexWeights;
            TriangleViewspaceA = m.TriangleViewspaceA;
            TriangleViewspaceB = m.TriangleViewspaceB;
            TriangleViewspaceC = m.TriangleViewspaceC;
            TextureMapX = m.TextureMapX;
            TextureMapY = m.TextureMapY;
            TextureMapZ = m.TextureMapZ;
        }

        public void RotateCCW()
        {
            for (var i = 0; i < VertexCount; i++)
            {
                VertexZ[i] = -VertexZ[i];
            }

            for (var i = 0; i < TriangleCount; i++)
            {
                int nz = TriangleViewspaceA[i];
                TriangleViewspaceA[i] = TriangleViewspaceC[i];
                TriangleViewspaceC[i] = nz;
            }
        }

        public void RotateCW()
        {
            for (var i = 0; i < VertexCount; i++)
            {
                int newZ = VertexX[i];
                VertexX[i] = VertexZ[i];
                VertexZ[i] = -newZ;
            }
        }

        public void Scale(int x, int y, int z)
        {
            for (var i = 0; i < VertexCount; i++)
            {
                VertexX[i] = ((VertexX[i] * x) / 128);
                VertexY[i] = ((VertexY[i] * y) / 128);
                VertexZ[i] = ((VertexZ[i] * z) / 128);
            }
        }

        public void ScaleDivide(int amount)
        {
            for (var i1 = 0; i1 < VertexCount; i1++)
            {
                VertexX[i1] = VertexX[i1] / amount;
                VertexY[i1] = VertexY[i1] / amount;
                VertexZ[i1] = VertexZ[i1] / amount;
            }
        }

        public void SetColor(int from, int to)
        {
            for (int i = 0; i < this.TriangleCount; i++)
            {
                if (this.TriangleColor[i] == from)
                {
                    this.TriangleColor[i] = to;
                }
            }
        }

        public void SetColors(int[] from, int[] to)
        {
            if (from.Length != to.Length)
            {
                return;
            }

            for (int i = 0; i < from.Length; i++)
            {
                this.SetColor(from[i], to[i]);
            }
        }

        public void Translate(int x, int y, int z)
        {
            for (var i = 0; i < VertexCount; i++)
            {
                VertexX[i] += x;
                VertexY[i] += y;
                VertexZ[i] += z;
            }
        }

        public void ApplyVertexWeights()
        {
            if (VertexSkinTypes != null)
            {
                var weightCounts = new int[256];
                var topLabel = 0;

                for (var i = 0; i < VertexCount; i++)
                {
                    var label = VertexSkinTypes[i];
                    weightCounts[label]++;

                    if (label > topLabel)
                    {
                        topLabel = label;
                    }
                }

                VertexWeights = new int[topLabel + 1][];

                for (var i = 0; i <= topLabel; i++)
                {
                    VertexWeights[i] = new int[weightCounts[i]];
                    weightCounts[i] = 0;
                }

                for (var i = 0; i < VertexCount; i++)
                {
                    int label = VertexSkinTypes[i];
                    VertexWeights[label][weightCounts[label]++] = i;
                }

                VertexSkinTypes = null;
                weightCounts = null;
            }

            if (TriangleTSkin != null)
            {
                var skinCounts = new int[256];
                var topSkin = 0;

                for (var i = 0; i < TriangleCount; i++)
                {
                    int skin = TriangleTSkin[i];
                    skinCounts[skin]++;

                    if (skin > topSkin)
                    {
                        topSkin = skin;
                    }
                }

                TriangleGroups = new int[topSkin + 1][];

                for (var i = 0; i <= topSkin; i++)
                {
                    TriangleGroups[i] = new int[skinCounts[i]];
                    skinCounts[i] = 0;
                }

                for (var i = 0; i < TriangleCount; i++)
                {
                    int group = TriangleTSkin[i];
                    TriangleGroups[group][skinCounts[group]++] = i;
                }

                TriangleTSkin = null;
                skinCounts = null;
            }
        }

        private void UpdateAttachments()
        {
            foreach (var a in Attachments)
            {
                a.Update(this);
            }
        }

        public void Transform(int opcode, int[] vertices, int x, int y, int z)
        {
            int vertexCount = vertices.Length;

            if (opcode == 0)
            {
                var j1 = 0;
                AnInt1681 = 0;
                AnInt1682 = 0;
                AnInt1683 = 0;

                for (var i = 0; i < vertexCount; i++)
                {
                    var j = vertices[i];
                    if (j < VertexWeights.Length)
                    {
                        var vWeights = VertexWeights[j];
                        for (var vWeight = 0; vWeight < vWeights.Length; vWeight++)
                        {
                            var weightVertex = vWeights[vWeight];
                            AnInt1681 += VertexX[weightVertex];
                            AnInt1682 += VertexY[weightVertex];
                            AnInt1683 += VertexZ[weightVertex];
                            j1++;
                        }
                    }
                }

                if (j1 > 0)
                {
                    AnInt1681 = AnInt1681 / j1 + x;
                    AnInt1682 = AnInt1682 / j1 + y;
                    AnInt1683 = AnInt1683 / j1 + z;
                    return;
                }
                else
                {
                    AnInt1681 = x;
                    AnInt1682 = y;
                    AnInt1683 = z;
                    return;
                }
            }

            if (opcode == 1)
            {
                for (var i = 0; i < vertexCount; i++)
                {
                    var j = vertices[i];
                    if (j < VertexWeights.Length)
                    {
                        var verticeIndices = VertexWeights[j];
                        for (var k = 0; k < verticeIndices.Length; k++)
                        {
                            var vIndex = verticeIndices[k];
                            VertexX[vIndex] += x;
                            VertexY[vIndex] += y;
                            VertexZ[vIndex] += z;
                        }
                    }
                }
                return;
            }

            if (opcode == 2)
            {
                for (var i = 0; i < vertexCount; i++)
                {
                    var j = vertices[i];
                    if (j < VertexWeights.Length)
                    {
                        var verticeIndices = VertexWeights[j];
                        for (var k = 0; k < verticeIndices.Length; k++)
                        {
                            var vIndex = verticeIndices[k];

                            VertexX[vIndex] -= AnInt1681;
                            VertexY[vIndex] -= AnInt1682;
                            VertexZ[vIndex] -= AnInt1683;

                            var pitch = (x & 0xff) * 8;
                            var yaw = (y & 0xff) * 8;
                            var roll = (z & 0xff) * 8;

                            if (roll != 0)
                            {
                                var sin = MathUtils.Sin[roll];
                                var cos = MathUtils.Cos[roll];
                                var newX = VertexY[vIndex] * sin + VertexX[vIndex] * cos >> 16;

                                VertexY[vIndex] = (VertexY[vIndex] * cos - VertexX[vIndex] * sin >> 16);
                                VertexX[vIndex] = newX;
                            }

                            if (pitch != 0)
                            {
                                var sin = MathUtils.Sin[pitch];
                                var cos = MathUtils.Cos[pitch];
                                var newY = VertexY[vIndex] * cos - VertexZ[vIndex] * sin >> 16;

                                VertexZ[vIndex] = (VertexY[vIndex] * sin + VertexZ[vIndex] * cos >> 16);
                                VertexY[vIndex] = newY;
                            }

                            if (yaw != 0)
                            {
                                var sin = MathUtils.Sin[yaw];
                                var cos = MathUtils.Cos[yaw];
                                var newZ = VertexZ[vIndex] * sin + VertexX[vIndex] * cos >> 16;

                                VertexZ[vIndex] = (VertexZ[vIndex] * cos - VertexX[vIndex] * sin >> 16);
                                VertexX[vIndex] = newZ;
                            }

                            VertexX[vIndex] += AnInt1681;
                            VertexY[vIndex] += AnInt1682;
                            VertexZ[vIndex] += AnInt1683;
                        }

                    }
                }
                return;
            }

            if (opcode == 3)
            {
                for (var i = 0; i < vertexCount; i++)
                {
                    var j = vertices[i];
                    if (j < VertexWeights.Length)
                    {
                        var vertexIndex = VertexWeights[j];
                        for (var k = 0; k < vertexIndex.Length; k++)
                        {
                            var v = vertexIndex[k];

                            VertexX[v] -= AnInt1681;
                            VertexY[v] -= AnInt1682;
                            VertexZ[v] -= AnInt1683;

                            VertexX[v] = ((VertexX[v] * x) / 128);
                            VertexY[v] = ((VertexY[v] * y) / 128);
                            VertexZ[v] = ((VertexZ[v] * z) / 128);

                            VertexX[v] += AnInt1681;
                            VertexY[v] += AnInt1682;
                            VertexZ[v] += AnInt1683;
                        }

                    }
                }
                return;
            }

            if (opcode == 5 && TriangleGroups != null && TriangleAlpha != null)
            {
                for (var i = 0; i < vertexCount; i++)
                {
                    var group = vertices[i];
                    if (group < TriangleGroups.Length)
                    {
                        var triIndices = TriangleGroups[group];
                        for (var k = 0; k < triIndices.Length; k++)
                        {
                            var triIndex = triIndices[k];

                            TriangleAlpha[triIndex] += x * 8;

                            if (TriangleAlpha[triIndex] < 0)
                            {
                                TriangleAlpha[triIndex] = 0;
                            }
                            else if (TriangleAlpha[triIndex] > 255)
                            {
                                TriangleAlpha[triIndex] = 255;
                            }
                        }
                    }
                }

            }
        }

        public void PushVertexData()
        {
            var vertices = new Vector3[TriangleCount * 3];
            for (var i = 0; i < TriangleCount; i++)
            {
                var idx1 = TriangleViewspaceA[i];
                vertices[i * 3] = new Vector3(VertexX[idx1] * GameConstants.RenderScale, -VertexY[idx1] * GameConstants.RenderScale, VertexZ[idx1] * GameConstants.RenderScale);

                var idx2 = TriangleViewspaceB[i];
                vertices[i * 3 + 1] = new Vector3(VertexX[idx2] * GameConstants.RenderScale, -VertexY[idx2] * GameConstants.RenderScale, VertexZ[idx2] * GameConstants.RenderScale);

                var idx3 = TriangleViewspaceC[i];
                vertices[i * 3 + 2] = new Vector3(VertexX[idx3] * GameConstants.RenderScale, -VertexY[idx3] * GameConstants.RenderScale, VertexZ[idx3] * GameConstants.RenderScale);
            }

            MeshFilter filter = Backing.GetComponent<MeshFilter>();
            Mesh mesh = filter.mesh;
            mesh.vertices = vertices;

            mesh.RecalculateBounds();
        }

        public void ApplyInterpolatedSequenceFrame(int frame, int nextFrame, int idk2, int idk)
        {

        }

        public void ApplySequenceFrame(int seqIndex, bool smooth = false)
        {
            if (VertexWeights == null)
            {
                return;
            }

            if (seqIndex == -1)
            {
                return;
            }

            SequenceFrame s = GameContext.Cache.GetSeqFrame(seqIndex);
            if (s == null)
            {
                return;
            }

            SkinList skin = s.Skinlist;
            AnInt1681 = 0;
            AnInt1682 = 0;
            AnInt1683 = 0;

            for (var frame = 0; frame < s.FrameCount; frame++)
            {
                var index = s.Vertices[frame];
                this.Transform(skin.Opcodes[index], skin.Vertices[index], s.VertexX[frame], s.VertexY[frame], s.VertexZ[frame]);
            }

            var vertices = new Vector3[TriangleCount * 3];
            for (var i = 0; i < TriangleCount; i++)
            {
                var idx1 = TriangleViewspaceA[i];
                vertices[i * 3] = new Vector3(VertexX[idx1] * GameConstants.RenderScale, -VertexY[idx1] * GameConstants.RenderScale, VertexZ[idx1] * GameConstants.RenderScale);

                var idx2 = TriangleViewspaceB[i];
                vertices[i * 3 + 1] = new Vector3(VertexX[idx2] * GameConstants.RenderScale, -VertexY[idx2] * GameConstants.RenderScale, VertexZ[idx2] * GameConstants.RenderScale);

                var idx3 = TriangleViewspaceC[i];
                vertices[i * 3 + 2] = new Vector3(VertexX[idx3] * GameConstants.RenderScale, -VertexY[idx3] * GameConstants.RenderScale, VertexZ[idx3] * GameConstants.RenderScale);
            }

            MeshFilter filter = Backing.GetComponent<MeshFilter>();
            Mesh mesh = filter.mesh;
            mesh.vertices = vertices;
            mesh.RecalculateBounds();

            actualNormal = null;
            actualNormal2 = null;
            CalculateNormals();
            PushNormals();
            UpdateAttachments();
            ;
        }

        /// <summary>
        /// Pushes all calculated normals to the unity model.
        /// </summary>
        private void PushNormals()
        {
            var normals = new List<Vector3>();

            foreach (var a in attachmentQueue)
            {
                var go = new GameObject();
                go.transform.parent = Backing.transform;
                go.transform.localPosition = new Vector3(0, 0, 0);
                a.CopyTo(this, go);
            }
            attachmentQueue.Clear();

            var scaleFactor = 3.0f / 768;
            var baseScale = 3.0f / (768 + 768 / 2);

            for (var i = 0; i < TriangleCount; i++)
            {
                var type = 0;
                if (TriangleInfo != null)
                {
                    type = TriangleInfo[i];
                }

                if (type == 0)
                {
                    var normalA = actualNormal[TriangleViewspaceA[i]];
                    if (normalA.ReuseCount == 0)
                    {
                        normals.Add(new Vector3(normalA.X * baseScale, normalA.Y * baseScale, normalA.Z * baseScale));
                    }
                    else
                    {
                        var scale = scaleFactor / normalA.ReuseCount;
                        normals.Add(new Vector3(normalA.X * scale, normalA.Y * scale, normalA.Z * scale));
                    }

                    var normalB = actualNormal[TriangleViewspaceB[i]];
                    if (normalB.ReuseCount == 0)
                    {
                        normals.Add(new Vector3(normalB.X * baseScale, normalB.Y * baseScale, normalB.Z * baseScale));
                    }
                    else
                    {
                        var scale = scaleFactor / normalB.ReuseCount;
                        normals.Add(new Vector3(normalB.X * scale, normalB.Y * scale, normalB.Z * scale));
                    }

                    var normalC = actualNormal[TriangleViewspaceC[i]];
                    if (normalC.ReuseCount == 0)
                    {
                        normals.Add(new Vector3(normalC.X * baseScale, normalC.Y * baseScale, normalC.Z * baseScale));
                    }
                    else
                    {
                        var scale = scaleFactor / normalC.ReuseCount;
                        normals.Add(new Vector3(normalC.X * scale, normalC.Y * scale, normalC.Z * scale));
                    }
                }
                else if (type == 1)
                {
                    var normalA = actualNormal2[i];
                    normals.Add(new Vector3(normalA.X * baseScale, normalA.Y * baseScale, normalA.Z * baseScale));
                    normals.Add(new Vector3(normalA.X * baseScale, normalA.Y * baseScale, normalA.Z * baseScale));
                    normals.Add(new Vector3(normalA.X * baseScale, normalA.Y * baseScale, normalA.Z * baseScale));
                }
                else
                {
                    normals.Add(new Vector3(1, 1, 1));
                    normals.Add(new Vector3(1, 1, 1));
                    normals.Add(new Vector3(1, 1, 1));
                }
            }

            MeshFilter filter = Backing.GetComponent<MeshFilter>();
            Mesh mesh = filter.mesh;
            mesh.normals = normals.ToArray();
        }

        /// <summary>
        /// Applies animations to this model.
        /// </summary>
        /// <param name="seqIndex">The index of the animation frame to apply.</param>
        /// <param name="nextSeqIndex">The index of the next animation frame to apply, for interpolation.</param>
        /// <param name="cycle1">The last cycle.</param>
        /// <param name="cycle2">The current cycle.</param>
        public void ApplyAnimFrames(int seqIndex, int nextSeqIndex, int cycle1, int cycle2)
        {
            if (cycle1 == 0)
            {
                if (cycle2 != 2)
                {
                    cycle1 = cycle2;
                } else
                {
                    cycle1 = 1;
                }
            }

            if (VertexWeights != null && seqIndex != -1)
            {
                
                var frame = GameContext.Cache.GetSeqFrame(seqIndex);
                if (frame != null)
                {
                    var transformList = frame.Skinlist;
                    AnInt1681 = 0;
                    AnInt1682 = 0;
                    AnInt1683 = 0;

                    SequenceFrame nextFrame = null;
                    if (nextSeqIndex != -1)
                    {
                        nextFrame = GameContext.Cache.GetSeqFrame(nextSeqIndex);
                        if (nextFrame.Skinlist != transformList)
                        {
                            nextFrame = null;
                        }
                    }

                    if (nextFrame == null)
                    {
                        for (var i = 0; i < frame.FrameCount; i++)
                        {
                            int v = frame.Vertices[i];
                            Transform(transformList.Opcodes[v], transformList.Vertices[v], frame.VertexX[i], frame.VertexY[i], frame.VertexZ[i]);
                        }
                    }
                    else
                    {
                        var curFrameId = 0;
                        var nextFrameId = 0;
                        for (var i = 0; i < transformList.Count; i++)
                        {
                            var curFrameValid = false;
                            if (curFrameId < frame.FrameCount && frame.Vertices[curFrameId] == i)
                            {
                                curFrameValid = true;
                            }

                            bool nextFrameValid = false;
                            if (nextFrameId < nextFrame.FrameCount && nextFrame.Vertices[nextFrameId] == i)
                            {
                                nextFrameValid = true;
                            }

                            if (curFrameValid || nextFrameValid)
                            {
                                int defaultModifier = 0;
                                int opcode = transformList.Opcodes[i];
                                if (opcode == 3)
                                {
                                    defaultModifier = 128;
                                }
                                int curAnimX;
                                int curAnimY;
                                int curAnimZ;
                                if (curFrameValid)
                                {
                                    curAnimX = frame.VertexX[curFrameId];
                                    curAnimY = frame.VertexY[curFrameId];
                                    curAnimZ = frame.VertexZ[curFrameId];
                                    curFrameId++;
                                }
                                else
                                {
                                    curAnimX = defaultModifier;
                                    curAnimY = defaultModifier;
                                    curAnimZ = defaultModifier;
                                }

                                int nextAnimX;
                                int nextAnimY;
                                int nextAnimZ;
                                if (nextFrameValid)
                                {
                                    nextAnimX = nextFrame.VertexX[nextFrameId];
                                    nextAnimY = nextFrame.VertexY[nextFrameId];
                                    nextAnimZ = nextFrame.VertexZ[nextFrameId];
                                    nextFrameId++;
                                }
                                else
                                {
                                    nextAnimX = defaultModifier;
                                    nextAnimY = defaultModifier;
                                    nextAnimZ = defaultModifier;
                                }

                                int interpolatedX;
                                int interpolatedY;
                                int interpolatedZ;
                                if (opcode == 2)
                                {
                                    int deltaX = nextAnimX - curAnimX & 0xff;
                                    int deltaY = nextAnimY - curAnimY & 0xff;
                                    int deltaZ = nextAnimZ - curAnimZ & 0xff;
                                    if (deltaX >= 128)
                                    {
                                        deltaX -= 256;
                                    }

                                    if (deltaY >= 128)
                                    {
                                        deltaY -= 256;
                                    }

                                    if (deltaZ >= 128)
                                    {
                                        deltaZ -= 256;
                                    }

                                    interpolatedX = curAnimX + deltaX * cycle2 / cycle1 & 0xff;
                                    interpolatedY = curAnimY + deltaY * cycle2 / cycle1 & 0xff;
                                    interpolatedZ = curAnimZ + deltaZ * cycle2 / cycle1 & 0xff;
                                }
                                else
                                {
                                    interpolatedX = curAnimX + (nextAnimX - curAnimX) * cycle2 / cycle1;
                                    interpolatedY = curAnimY + (nextAnimY - curAnimY) * cycle2 / cycle1;
                                    interpolatedZ = curAnimZ + (nextAnimZ - curAnimZ) * cycle2 / cycle1;
                                }

                                Transform(opcode, transformList.Vertices[i], interpolatedX, interpolatedY, interpolatedZ);
                            }
                        }
                    }
                }
            }

            var vertices = new Vector3[TriangleCount * 3];
            for (var i = 0; i < TriangleCount; i++)
            {
                var idx1 = TriangleViewspaceA[i];
                vertices[i * 3] = new Vector3(VertexX[idx1] * GameConstants.RenderScale, -VertexY[idx1] * GameConstants.RenderScale, VertexZ[idx1] * GameConstants.RenderScale);

                var idx2 = TriangleViewspaceB[i];
                vertices[i * 3 + 1] = new Vector3(VertexX[idx2] * GameConstants.RenderScale, -VertexY[idx2] * GameConstants.RenderScale, VertexZ[idx2] * GameConstants.RenderScale);

                var idx3 = TriangleViewspaceC[i];
                vertices[i * 3 + 2] = new Vector3(VertexX[idx3] * GameConstants.RenderScale, -VertexY[idx3] * GameConstants.RenderScale, VertexZ[idx3] * GameConstants.RenderScale);
            }

            MeshFilter filter = Backing.GetComponent<MeshFilter>();
            Mesh mesh = filter.mesh;
            mesh.vertices = vertices;
            mesh.RecalculateBounds();

            actualNormal = null;
            actualNormal2 = null;
            CalculateNormals();
            PushNormals();

            UpdateAttachments();
        }

        public void ApplySequenceFrames(int[] vertices, int frame1, int frame2, bool smooth = false)
        {
            if (frame1 == -1)
            {
                return;
            }

            if (vertices == null || frame2 == -1)
            {
                ApplySequenceFrame(frame1);
                return;
            }

            SequenceFrame af1 = GameContext.Cache.GetSeqFrame(frame1);
            if (af1 == null)
            {
                return;
            }

            SequenceFrame af2 = GameContext.Cache.GetSeqFrame(frame2);
            if (af2 == null)
            {
                ApplySequenceFrame(frame1);
                return;
            }

            SkinList slist = af1.Skinlist;

            AnInt1681 = 0;
            AnInt1682 = 0;
            AnInt1683 = 0;

            int position = 0;
            int vertex = vertices[position++];

            for (int frame = 0; frame < af1.FrameCount; frame++)
            {
                int v;
                for (v = af1.Vertices[frame]; v > vertex; vertex = vertices[position++]) ;
                if (v != vertex || slist.Opcodes[v] == 0)
                {
                    Transform(slist.Opcodes[v], slist.Vertices[v], af1.VertexX[frame], af1.VertexY[frame], af1.VertexZ[frame]);
                }
            }

            AnInt1681 = 0;
            AnInt1682 = 0;
            AnInt1683 = 0;

            position = 0;
            vertex = vertices[position++];

            for (var frame = 0; frame < af2.FrameCount; frame++)
            {
                int v;
                for (v = af2.Vertices[frame]; v > vertex; vertex = vertices[position++]);
                if (v == vertex || slist.Opcodes[v] == 0)
                {
                    Transform(slist.Opcodes[v], slist.Vertices[v], af2.VertexX[frame], af2.VertexY[frame], af2.VertexZ[frame]);
                }
            }

            actualNormal = null;
            actualNormal2 = null;
            CalculateNormals();
            PushNormals();

            UpdateAttachments();
        }

        public void UpdateMaxHorizonHeightCheck()
        {
            Height = 0;
            MaxHorizon = 0;
            MaxY = 0;

            for (var i = 0; i < VertexCount; i++)
            {
                var x = VertexX[i];
                var y = VertexY[i];
                var z = VertexZ[i];

                if (-y > Height)
                {
                    Height = -y;
                }

                if (y > MaxY)
                {
                    MaxY = y;
                }

                var horizon = x * x + z * z;
                if (horizon > MaxHorizon)
                {
                    MaxHorizon = horizon;
                }
            }

            MaxHorizon = (int)(Math.Sqrt(MaxHorizon) + 0.99D);
            Unknown2 = (int)(Math.Sqrt(MaxHorizon * MaxHorizon + Height * Height) + 0.99D);
            Unknown3 = Unknown2 + (int)(Math.Sqrt(MaxHorizon * MaxHorizon + MaxY * MaxY) + 0.99D);
        }

        public void UpdateMaxHorizonAllCheck()
        {
            Height = 0;
            MaxHorizon = 0;
            MaxY = 0;
            MinX = 999999;
            MaxX = -999999;
            MaxZ = -99999;
            MinZ = 99999;

            for (var i = 0; i < VertexCount; i++)
            {
                var x = VertexX[i];
                var y = VertexY[i];
                var z = VertexZ[i];

                if (x < MinX)
                {
                    MinX = x;
                }

                if (x > MaxX)
                {
                    MaxX = x;
                }

                if (z < MinZ)
                {
                    MinZ = z;
                }

                if (z > MaxZ)
                {
                    MaxZ = z;
                }

                if (-y > Height)
                {
                    Height = -y;
                }

                if (y > MaxY)
                {
                    MaxY = y;
                }

                var horizon = x * x + z * z;
                if (horizon > MaxHorizon)
                {
                    MaxHorizon = horizon;
                }
            }

            MaxHorizon = (int)Math.Sqrt(MaxHorizon);
            Unknown2 = (int)Math.Sqrt(MaxHorizon * MaxHorizon + Height * Height);
            Unknown3 = Unknown2 + (int)Math.Sqrt(MaxHorizon * MaxHorizon + MaxY * MaxY);
        }

        class VNormal
        {
            public int X;
            public int Y;
            public int Z;
            public int ReuseCount;
        }

        /// <summary>
        /// Calculates the normals for this module.
        /// </summary>
        public void CalculateNormals()
        {
            if (actualNormal == null)
            {
                actualNormal = new VNormal[this.VertexCount];
                for (var id = 0; id < this.VertexCount; id++)
                {
                    actualNormal[id] = new VNormal();
                }

                for (var id = 0; id < this.TriangleCount; id++)
                {
                    var tA = TriangleViewspaceA[id];
                    var tB = TriangleViewspaceB[id];
                    var tC = TriangleViewspaceC[id];
                    var v1x = VertexX[tB] - VertexX[tA];
                    var v1y = VertexY[tB] - VertexY[tA];
                    var v1z = VertexZ[tB] - VertexZ[tA];
                    var v2x = VertexX[tC] - VertexX[tA];
                    var v2y = VertexY[tC] - VertexY[tA];
                    var v2z = VertexZ[tC] - VertexZ[tA];
                    var normalX = v1y * v2z - v2y * v1z;
                    var normalY = v1z * v2x - v2z * v1x;
                    var normalZ = v1x * v2y - v2x * v1y;
                    for (; normalX > 8192 || normalY > 8192 || normalZ > 8192 || normalX < -8192 || normalY < -8192 || normalZ < -8192;)
                    {
                        normalZ >>= 1;
                        normalX >>= 1;
                        normalY >>= 1;
                    }

                    var normalLength = (int)Math.Sqrt(normalX * normalX + normalY * normalY + normalZ * normalZ);
                    if (normalLength <= 0)
                    {
                        normalLength = 1;
                    }

                    normalX = normalX * 256 / normalLength;
                    normalY = normalY * 256 / normalLength;
                    normalZ = normalZ * 256 / normalLength;

                    var type = 0;
                    if (TriangleInfo != null)
                    {
                        type = TriangleInfo[id];
                    }

                    if (type == 0)
                    {
                        var normal = this.actualNormal[tA];
                        normal.X += normalX;
                        normal.Y += normalY;
                        normal.Z += normalZ;
                        normal.ReuseCount++;
                        normal = this.actualNormal[tB];
                        normal.X += normalX;
                        normal.Y += normalY;
                        normal.Z += normalZ;
                        normal.ReuseCount++;
                        normal = this.actualNormal[tC];
                        normal.X += normalX;
                        normal.Y += normalY;
                        normal.Z += normalZ;
                        normal.ReuseCount++;
                    }
                    else if (type == 1)
                    {
                        if (actualNormal2 == null)
                            actualNormal2 = new VNormal[this.TriangleCount];

                        var normal = actualNormal2[id] = new VNormal();
                        normal.X = normalX;
                        normal.Y = normalY;
                        normal.Z = normalZ;
                    }
                }
            }
        }

        public void ApplyLighting(int brightness, int specular, int lightX, int lightY, int lightZ)
        {
            for (var i = 0; i < TriangleCount; i++)
            {
                var x = TriangleViewspaceA[i];
                var y = TriangleViewspaceB[i];
                var z = TriangleViewspaceC[i];

                if (TriangleInfo == null)
                {
                    var hsl = TriangleColor[i];
                    var v = Normal[x];

                    var lightness = brightness + (lightX * v.X + lightY * v.Y + lightZ * v.Z) / (specular * v.W);
                    TriHsl1[i] = ColorUtils.SetHslLightness(hsl, lightness, 0);

                    v = Normal[y];
                    lightness = brightness + (lightX * v.X + lightY * v.Y + lightZ * v.Z) / (specular * v.W);
                    TriHsl2[i] = ColorUtils.SetHslLightness(hsl, lightness, 0);

                    v = Normal[z];
                    lightness = brightness + (lightX * v.X + lightY * v.Y + lightZ * v.Z) / (specular * v.W);
                    TriHsl3[i] = ColorUtils.SetHslLightness(hsl, lightness, 0);
                }
                else if ((TriangleInfo[i] & 1) == 0)
                {
                    var hsl = TriangleColor[i];
                    var info = TriangleInfo[i];

                    var v = Normal[x];
                    var lightness = brightness + (lightX * v.X + lightY * v.Y + lightZ * v.Z) / (specular * v.W);
                    TriHsl1[i] = ColorUtils.SetHslLightness(hsl, lightness, info);

                    v = Normal[y];
                    lightness = brightness + (lightX * v.X + lightY * v.Y + lightZ * v.Z) / (specular * v.W);
                    TriHsl2[i] = ColorUtils.SetHslLightness(hsl, lightness, info);

                    v = Normal[z];
                    lightness = brightness + (lightX * v.X + lightY * v.Y + lightZ * v.Z) / (specular * v.W);
                    TriHsl3[i] = ColorUtils.SetHslLightness(hsl, lightness, info);
                }
            }

            Normal = null;
            Vertices = null;
            VertexSkinTypes = null;
            TriangleTSkin = null;

            if (TriangleInfo != null)
            {
                for (int i = 0; i < TriangleCount; i++)
                {
                    if ((TriangleInfo[i] & 2) == 2)
                    {
                        return;
                    }
                }
            }

            TriangleColor = null;
        }

        public void ApplyLighting(int lightBrightness, int specularFactor, int lightX, int ightY, int lightZ, bool smoothShading)
        {
            var lightLength = (int)Math.Sqrt(lightX * lightX + ightY * ightY + lightZ * lightZ);
            var specularDistribution = specularFactor * lightLength >> 8;

            if (TriHsl1 == null)
            {
                TriHsl1 = new int[TriangleCount];
                TriHsl2 = new int[TriangleCount];
                TriHsl3 = new int[TriangleCount];
            }

            if (Normal == null)
            {
                Normal = new PriorityVertex[VertexCount];
                for (int i = 0; i < VertexCount; i++)
                {
                    Normal[i] = new PriorityVertex();
                }
            }

            for (var i = 0; i < TriangleCount; i++)
            {
                var index1 = TriangleViewspaceA[i];
                var index2 = TriangleViewspaceB[i];
                var index3 = TriangleViewspaceC[i];

                var absX2 = VertexX[index2] - VertexX[index1];
                var absY2 = VertexY[index2] - VertexY[index1];
                var absZ2 = VertexZ[index2] - VertexZ[index1];

                var absX3 = VertexX[index3] - VertexX[index1];
                var absY3 = VertexY[index3] - VertexY[index1];
                var absZ3 = VertexZ[index3] - VertexZ[index1];

                var x = absY2 * absZ3 - absY3 * absZ2;
                var y = absZ2 * absX3 - absZ3 * absX2;
                int z;

                for (z = absX2 * absY3 - absX3 * absY2; x > 8192 || y > 8192 || z > 8192 || x < -8192 || y < -8192 || z < -8192; z >>= 1)
                {
                    x >>= 1;
                    y >>= 1;
                }

                var length = (int)Math.Sqrt(x * x + y * y + z * z);
                if (length <= 0)
                {
                    length = 1;
                }

                x = (x * 256) / length;
                y = (y * 256) / length;
                z = (z * 256) / length;

                if (TriangleInfo == null || (TriangleInfo[i] & 1) == 0)
                {
                    var v = Normal[index1];
                    v.X += x;
                    v.Y += y;
                    v.Z += z;
                    v.W++;
                    v = Normal[index2];
                    v.X += x;
                    v.Y += y;
                    v.Z += z;
                    v.W++;
                    v = Normal[index3];
                    v.X += x;
                    v.Y += y;
                    v.Z += z;
                    v.W++;
                }
                else
                {
                    TriHsl1[i] = ColorUtils.SetHslLightness(TriangleColor[i], lightBrightness + (lightX * x + ightY * y + lightZ * z) / (specularDistribution + specularDistribution / 2), TriangleInfo[i]);
                }
            }

            if (smoothShading)
            {
                ApplyLighting(lightBrightness, specularDistribution, lightX, ightY, lightZ);
            }
            else
            {
                Vertices = new PriorityVertex[VertexCount];

                for (int i = 0; i < VertexCount; i++)
                {
                    PriorityVertex v = Normal[i];
                    PriorityVertex n_v = Vertices[i] = new PriorityVertex();
                    n_v.X = v.X;
                    n_v.Y = v.Y;
                    n_v.Z = v.Z;
                    n_v.W = v.W;
                }

            }
            if (smoothShading)
            {
                UpdateMaxHorizonHeightCheck();
                return;
            }
            else
            {
                UpdateMaxHorizonAllCheck();
                return;
            }
        }

        /// <summary>
        /// Adds all attachments from the attachment queue to this model.
        /// </summary>
        private void ProcessAttachmentQueue()
        {
            foreach (var attachments in attachmentQueue)
            {
                var go = new GameObject();
                go.transform.parent = Backing.transform;
                go.transform.localPosition = new Vector3(0, 0, 0);
                attachments.CopyTo(this, go);
            }
            attachmentQueue.Clear();
        }

        public void AddMeshToObject(bool smooth = false, bool debug = false)
        {
            var filter = Backing.GetComponent<MeshFilter>();
            if (filter == null) filter = Backing.AddComponent<MeshFilter>();

            var meshRenderer = Backing.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
            {
                meshRenderer = Backing.AddComponent<MeshRenderer>();
            }

            if (lastMesh != null)
            {
                filter.mesh = lastMesh;
                meshRenderer.sharedMaterials = lastMaterials;
                return;
            }

            CalculateNormals();

            lastMesh = new Mesh();
            filter.mesh = lastMesh;

            var vertices = new Vector3[TriangleCount * 3];
            var vertexPtr = 0;
            var textureBatches = new List<ModelMaterialConfig>();
            var uvs = new List<Vector2>();
            var normals = new Vector3[TriangleCount * 3];
            var normalPtr = 0;

            

            var contrast = 768;
            var scaleFactor = 3.0f / contrast;
            var baseScale = 3.0f / (contrast + contrast / 2);

            for (var i = 0; i < TriangleCount; i++)
            {
                var type = 0;
                if (TriangleInfo != null)
                {
                    type = TriangleInfo[i];
                }

                if (type == 0)
                {
                    var normalA = actualNormal[TriangleViewspaceA[i]];
                    if (normalA.ReuseCount == 0)
                    {
                        normals[normalPtr++] = new Vector3(normalA.X * baseScale, normalA.Y * baseScale, normalA.Z * baseScale);
                    }
                    else
                    {
                        var scale = scaleFactor / normalA.ReuseCount;
                        normals[normalPtr++] = new Vector3(normalA.X * scale, normalA.Y * scale, normalA.Z * scale);
                    }

                    var normalB = actualNormal[TriangleViewspaceB[i]];
                    if (normalB.ReuseCount == 0)
                    {
                        normals[normalPtr++] = new Vector3(normalB.X * baseScale, normalB.Y * baseScale, normalB.Z * baseScale);
                    }
                    else
                    {
                        var scale = scaleFactor / normalB.ReuseCount;
                        normals[normalPtr++] = new Vector3(normalB.X * scale, normalB.Y * scale, normalB.Z * scale);
                    }

                    var normalC = actualNormal[TriangleViewspaceC[i]];
                    if (normalC.ReuseCount == 0)
                    {
                        normals[normalPtr++] = new Vector3(normalC.X * baseScale, normalC.Y * baseScale, normalC.Z * baseScale);
                    }
                    else
                    {
                        var scale = scaleFactor / normalC.ReuseCount;
                        normals[normalPtr++] = new Vector3(normalC.X * scale, normalC.Y * scale, normalC.Z * scale);
                    }
                }
                else if (type == 1)
                {
                    var normalA = actualNormal2[i];
                    normals[normalPtr++] = new Vector3(normalA.X * baseScale, normalA.Y * baseScale, normalA.Z * baseScale);
                    normals[normalPtr++] = new Vector3(normalA.X * baseScale, normalA.Y * baseScale, normalA.Z * baseScale);
                    normals[normalPtr++] = new Vector3(normalA.X * baseScale, normalA.Y * baseScale, normalA.Z * baseScale);
                }
                else
                {
                    normals[normalPtr++] = new Vector3(1, 1, 1);
                    normals[normalPtr++] = new Vector3(1, 1, 1);
                    normals[normalPtr++] = new Vector3(1, 1, 1);
                }
            }

            int[] is_734_ = null;
            int[] is_735_ = null;
            int[] is_736_ = null;
            float[][] fs = null;
            if (TextureVertexPointers != null)
            {
                int i_737_ = TexturedTriangleCount;
                int[] is_738_ = new int[i_737_];
                int[] is_739_ = new int[i_737_];
                int[] is_740_ = new int[i_737_];
                int[] is_741_ = new int[i_737_];
                int[] is_742_ = new int[i_737_];
                int[] is_743_ = new int[i_737_];
                for (int i_744_ = 0; i_744_ < i_737_; i_744_++)
                {
                    is_738_[i_744_] = 2147483647;
                    is_739_[i_744_] = -2147483647;
                    is_740_[i_744_] = 2147483647;
                    is_741_[i_744_] = -2147483647;
                    is_742_[i_744_] = 2147483647;
                    is_743_[i_744_] = -2147483647;
                }
                for (int i_745_ = 0; i_745_ < TriangleCount; i_745_++)
                {
                    int i_746_ = i_745_;
                    if (TextureVertexPointers[i_746_] != -1)
                    {
                        int i_747_ = TextureVertexPointers[i_746_] & 0xff;
                        for (int i_748_ = 0; i_748_ < 3; i_748_++)
                        {
                            int i_749_;
                            if (i_748_ == 0)
                            {
                                i_749_ = TriangleViewspaceA[i_746_];
                            }
                            else if (i_748_ == 1)
                            {
                                i_749_ = TriangleViewspaceB[i_746_];
                            }
                            else
                            {
                                i_749_ = TriangleViewspaceC[i_746_];
                            }
                            int i_750_ = VertexX[i_749_];
                            int i_751_ = VertexY[i_749_];
                            int i_752_ = VertexZ[i_749_];
                            if (i_750_ < is_738_[i_747_])
                            {
                                is_738_[i_747_] = i_750_;
                            }
                            if (i_750_ > is_739_[i_747_])
                            {
                                is_739_[i_747_] = i_750_;
                            }
                            if (i_751_ < is_740_[i_747_])
                            {
                                is_740_[i_747_] = i_751_;
                            }
                            if (i_751_ > is_741_[i_747_])
                            {
                                is_741_[i_747_] = i_751_;
                            }
                            if (i_752_ < is_742_[i_747_])
                            {
                                is_742_[i_747_] = i_752_;
                            }
                            if (i_752_ > is_743_[i_747_])
                            {
                                is_743_[i_747_] = i_752_;
                            }
                        }
                    }
                }
                is_734_ = new int[i_737_];
                is_735_ = new int[i_737_];
                is_736_ = new int[i_737_];
                fs = new float[i_737_][];
                for (int i_753_ = 0; i_753_ < i_737_; i_753_++)
                {
                    var i_754_ = TriangleTextureMapTypes[i_753_];
                    if (i_754_ > 0)
                    {
                        is_734_[i_753_] = (is_738_[i_753_] + is_739_[i_753_]) / 2;
                        is_735_[i_753_] = (is_740_[i_753_] + is_741_[i_753_]) / 2;
                        is_736_[i_753_] = (is_742_[i_753_] + is_743_[i_753_]) / 2;
                        float f;
                        float f_755_;
                        float f_756_;
                        if (i_754_ == 1)
                        {
                            var i_757_ = TextureScaleX[i_753_];
                            if (i_757_ == 0)
                            {
                                f = 1.0F;
                                f_756_ = 1.0F;
                            }
                            else if (i_757_ > 0)
                            {
                                f = 1.0F;
                                f_756_ = i_757_ / 1024.0F;
                            }
                            else
                            {
                                f_756_ = 1.0F;
                                f = -i_757_ / 1024.0F;
                            }
                            f_755_ = 64.0F / (TextureScaleY[i_753_] & 0xffff);
                        }
                        else if (i_754_ == 2)
                        {
                            f = 64.0F / (TextureScaleX[i_753_] & 0xffff);
                            f_755_ = 64.0F / (TextureScaleY[i_753_] & 0xffff);
                            f_756_ = 64.0F / (TextureScaleZ[i_753_] & 0xffff);
                        }
                        else
                        {
                            f = TextureScaleX[i_753_] / 1024.0F;
                            f_755_ = TextureScaleY[i_753_] / 1024.0F;
                            f_756_ = TextureScaleZ[i_753_] / 1024.0F;
                        }
                        fs[i_753_] = ObfuscatedShit.method2424(TextureMapX[i_753_], TextureMapY[i_753_], TextureMapZ[i_753_], TextureRotationY[i_753_] & 0xff, f, f_755_, f_756_);
                    }
                }
            }

            for (var i = 0; i < TriangleCount; i++)
            {
                var color = TriangleColor[i];
                var type = RenderType.Colored;
                var texture = -1;

                if (TriangleTextures == null || TriangleTextures[i] == -1)
                {
                    type = RenderType.Colored;
                }
                else
                {
                    type = RenderType.Textured;
                    texture = TriangleTextures[i];
                }

                var texMapPos = -1;
                if (type == RenderType.Textured)
                {
                    texMapPos = TextureVertexPointers[i];
                }

                var alpha = 255;
                if (TriangleAlpha != null && TriangleAlpha[i] > 0)
                {
                    alpha = 255 - TriangleAlpha[i];
                }

                var config = new ModelMaterialConfig(type, color, alpha, texture);
                config.ShouldColor = true;
                if (textureBatches.Contains(config))
                {
                    config = textureBatches.FirstOrDefault(f => f == config);
                }
                else
                {
                    textureBatches.Add(config);
                }

                var idx1 = TriangleViewspaceA[i];
                vertices[vertexPtr++] = new Vector3(VertexX[idx1] * GameConstants.RenderScale, -VertexY[idx1] * GameConstants.RenderScale, VertexZ[idx1] * GameConstants.RenderScale);
                config.Triangles.Add(i * 3 + 2);

                var idx2 = TriangleViewspaceB[i];
                vertices[vertexPtr++] = new Vector3(VertexX[idx2] * GameConstants.RenderScale, -VertexY[idx2] * GameConstants.RenderScale, VertexZ[idx2] * GameConstants.RenderScale);
                config.Triangles.Add(i * 3 + 1);

                var idx3 = TriangleViewspaceC[i];
                vertices[vertexPtr++] = new Vector3(VertexX[idx3] * GameConstants.RenderScale, -VertexY[idx3] * GameConstants.RenderScale, VertexZ[idx3] * GameConstants.RenderScale);
                config.Triangles.Add(i * 3);

                if (type == RenderType.Textured)
                {
                    if (texMapPos == -1)
                    {
                        uvs.Add(new Vector2(0, 1));
                        uvs.Add(new Vector2(1, 1));
                        uvs.Add(new Vector2(0, 0));
                    }
                    else
                    {
                        texMapPos &= 0xFF;
                        if (TriangleTextureMapTypes[texMapPos] == 0)
                        {
                            int tva = TriangleViewspaceA[i];
                            int tvb = TriangleViewspaceB[i];
                            int tvc = TriangleViewspaceC[i];
                            int tmx = TextureMapX[texMapPos];
                            int tmy = TextureMapY[texMapPos];
                            int tmz = TextureMapZ[texMapPos];

                            float f_779_ = VertexX[tmx];
                            float f_780_ = VertexY[tmx];
                            float f_781_ = VertexZ[tmx];

                            float f_782_ = VertexX[tmy] - f_779_;
                            float f_783_ = VertexY[tmy] - f_780_;
                            float f_784_ = VertexZ[tmy] - f_781_;

                            float f_785_ = VertexX[tmz] - f_779_;
                            float f_786_ = VertexY[tmz] - f_780_;
                            float f_787_ = VertexZ[tmz] - f_781_;

                            float f_788_ = VertexX[tva] - f_779_;
                            float f_789_ = VertexY[tva] - f_780_;
                            float f_790_ = VertexZ[tva] - f_781_;

                            float f_791_ = VertexX[tvb] - f_779_;
                            float f_792_ = VertexY[tvb] - f_780_;
                            float f_793_ = VertexZ[tvb] - f_781_;

                            float f_794_ = VertexX[tvc] - f_779_;
                            float f_795_ = VertexY[tvc] - f_780_;
                            float f_796_ = VertexZ[tvc] - f_781_;

                            float f_797_ = f_783_ * f_787_ - f_784_ * f_786_;
                            float f_798_ = f_784_ * f_785_ - f_782_ * f_787_;
                            float f_799_ = f_782_ * f_786_ - f_783_ * f_785_;
                            float f_800_ = f_786_ * f_799_ - f_787_ * f_798_;
                            float f_801_ = f_787_ * f_797_ - f_785_ * f_799_;
                            float f_802_ = f_785_ * f_798_ - f_786_ * f_797_;
                            float f_803_ = 1.0F / (f_800_ * f_782_ + f_801_ * f_783_ + f_802_ * f_784_);
                            var aU = (f_800_ * f_788_ + f_801_ * f_789_ + f_802_ * f_790_) * f_803_;
                            var bU = (f_800_ * f_791_ + f_801_ * f_792_ + f_802_ * f_793_) * f_803_;
                            var cU = (f_800_ * f_794_ + f_801_ * f_795_ + f_802_ * f_796_) * f_803_;
                            f_800_ = f_783_ * f_799_ - f_784_ * f_798_;
                            f_801_ = f_784_ * f_797_ - f_782_ * f_799_;
                            f_802_ = f_782_ * f_798_ - f_783_ * f_797_;
                            f_803_ = 1.0F / (f_800_ * f_785_ + f_801_ * f_786_ + f_802_ * f_787_);
                            var aV = (f_800_ * f_788_ + f_801_ * f_789_ + f_802_ * f_790_) * f_803_;
                            var bV = (f_800_ * f_791_ + f_801_ * f_792_ + f_802_ * f_793_) * f_803_;
                            var cV = (f_800_ * f_794_ + f_801_ * f_795_ + f_802_ * f_796_) * f_803_;

                            uvs.Add(new Vector2(aU >= 1 ? 1f : aU, aV >= 1 ? 1f : aV));
                            uvs.Add(new Vector2(bU >= 1 ? 1f : bU, bV >= 1 ? 1f : bV));
                            uvs.Add(new Vector2(cU >= 1 ? 1f : cU, cV >= 1 ? 1f : cV));
                        }
                        else
                        {
                            var i_807_ = is_734_[texMapPos];
                            var i_808_ = is_735_[texMapPos];
                            var i_809_ = is_736_[texMapPos];
                            var fs_810_ = fs[texMapPos];
                            var i_811_ = aByteArray2888[texMapPos];
                            var f_812_ = aByteArray2870[texMapPos] / 256.0F;

                            if (TriangleTextureMapTypes[texMapPos] == 2)
                            {
                                float f_815_ = aByteArray2859[texMapPos] / 256.0F;
                                float f_816_ = aByteArray2851[texMapPos] / 256.0F;
                                int i_817_ = VertexX[idx2] - VertexX[idx1];
                                int i_818_ = VertexY[idx2] - VertexY[idx1];
                                int i_819_ = VertexZ[idx2] - VertexZ[idx1];
                                int i_820_ = VertexX[idx3] - VertexX[idx1];
                                int i_821_ = VertexY[idx3] - VertexY[idx1];
                                int i_822_ = VertexZ[idx3] - VertexZ[idx1];
                                int i_823_ = i_818_ * i_822_ - i_821_ * i_819_;
                                int i_824_ = i_819_ * i_820_ - i_822_ * i_817_;
                                int i_825_ = i_817_ * i_821_ - i_820_ * i_818_;
                                float f_826_ = 64.0F / (TextureScaleX[texMapPos] & 0xffff);
                                float f_827_ = 64.0F / (TextureScaleY[texMapPos] & 0xffff);
                                float f_828_ = 64.0F / (TextureScaleZ[texMapPos] & 0xffff);
                                float f_829_ = (i_823_ * fs_810_[0] + i_824_ * fs_810_[1] + i_825_ * fs_810_[2]) / f_826_;
                                float f_830_ = (i_823_ * fs_810_[3] + i_824_ * fs_810_[4] + i_825_ * fs_810_[5]) / f_827_;
                                float f_831_ = (i_823_ * fs_810_[6] + i_824_ * fs_810_[7] + i_825_ * fs_810_[8]) / f_828_;
                                var i_771_ = ObfuscatedShit.method2437(f_829_, f_830_, f_831_);
                                ObfuscatedShit.method2416(VertexX[idx1], VertexY[idx1], VertexZ[idx1], i_807_, i_808_, i_809_, i_771_, fs_810_, i_811_, f_812_, f_815_, f_816_);
                                var aU = ObfuscatedShit.tmpUMapping1;
                                var aV = ObfuscatedShit.tmpVMapping1;
                                ObfuscatedShit.method2416(VertexX[idx2], VertexY[idx2], VertexZ[idx2], i_807_, i_808_, i_809_, i_771_, fs_810_, i_811_, f_812_, f_815_, f_816_);
                                var bU = ObfuscatedShit.tmpUMapping1;
                                var bV = ObfuscatedShit.tmpVMapping1;
                                ObfuscatedShit.method2416(VertexX[idx3], VertexY[idx3], VertexZ[idx3], i_807_, i_808_, i_809_, i_771_, fs_810_, i_811_, f_812_, f_815_, f_816_);
                                var cU = ObfuscatedShit.tmpUMapping1;
                                var cV = ObfuscatedShit.tmpVMapping1;

                                uvs.Add(new Vector2(aU >= 1 ? 1f : aU, aV >= 1 ? 1f : aV));
                                uvs.Add(new Vector2(bU >= 1 ? 1f : bU, bV >= 1 ? 1f : bV));
                                uvs.Add(new Vector2(cU >= 1 ? 1f : cU, cV >= 1 ? 1f : cV));


                            }
                            else if (TriangleTextureMapTypes[texMapPos] == 1)
                            {
                                float f_813_ = (TextureScaleZ[texMapPos] & 0xffff) / 1024.0F;
                                ObfuscatedShit.method2431(VertexX[idx1], VertexY[idx1], VertexZ[idx1], i_807_, i_808_, i_809_, fs_810_, f_813_, i_811_, f_812_);
                                var aU = ObfuscatedShit.aFloat3899;
                                var aV = ObfuscatedShit.aFloat3903;
                                ObfuscatedShit.method2431(VertexX[idx2], VertexY[idx2], VertexZ[idx2], i_807_, i_808_, i_809_, fs_810_, f_813_, i_811_, f_812_);
                                var bU = ObfuscatedShit.aFloat3899;
                                var bV = ObfuscatedShit.aFloat3903;
                                ObfuscatedShit.method2431(VertexX[idx3], VertexY[idx3], VertexZ[idx3], i_807_, i_808_, i_809_, fs_810_, f_813_, i_811_, f_812_);
                                var cU = ObfuscatedShit.aFloat3899;
                                var cV = ObfuscatedShit.aFloat3903;
                                float f_814_ = f_813_ / 2.0F;
                                if ((i_811_ & 0x1) == 0)
                                {
                                    if (bU - aU > f_814_)
                                    {
                                        bU -= f_813_;
                                    }
                                    else if (aU - bU > f_814_)
                                    {
                                        bU += f_813_;
                                    }
                                    if (cU - aU > f_814_)
                                    {
                                        cU -= f_813_;
                                    }
                                    else if (aU - cU > f_814_)
                                    {
                                        cU += f_813_;
                                    }
                                }
                                else
                                {
                                    if (bV - aV > f_814_)
                                    {
                                        bV -= f_813_;
                                    }
                                    else if (aV - bV > f_814_)
                                    {
                                        bV += f_813_;
                                    }
                                    if (cV - aV > f_814_)
                                    {
                                        cV -= f_813_;
                                    }
                                    else if (aV - cV > f_814_)
                                    {
                                        cV += f_813_;
                                    }
                                }

                                uvs.Add(new Vector2(aU >= 1 ? 1f : aU, aV >= 1 ? 1f : aV));
                                uvs.Add(new Vector2(bU >= 1 ? 1f : bU, bV >= 1 ? 1f : bV));
                                uvs.Add(new Vector2(cU >= 1 ? 1f : cU, cV >= 1 ? 1f : cV));
                            }
                            else if (TriangleTextureMapTypes[texMapPos] == 3)
                            {
                                ObfuscatedShit.method2434(VertexX[idx1], VertexY[idx1], VertexZ[idx1], i_807_, i_808_, i_809_, fs_810_, i_811_, f_812_);
                                var aU = ObfuscatedShit.aFloat3907;
                                var aV = ObfuscatedShit.aFloat3902;
                                ObfuscatedShit.method2434(VertexX[idx2], VertexY[idx2], VertexZ[idx2], i_807_, i_808_, i_809_, fs_810_, i_811_, f_812_);
                                var bU = ObfuscatedShit.aFloat3907;
                                var bV = ObfuscatedShit.aFloat3902;
                                ObfuscatedShit.method2434(VertexX[idx3], VertexY[idx3], VertexZ[idx3], i_807_, i_808_, i_809_, fs_810_, i_811_, f_812_);
                                var cU = ObfuscatedShit.aFloat3907;
                                var cV = ObfuscatedShit.aFloat3902;
                                if ((i_811_ & 0x1) == 0)
                                {
                                    if (bU - aU > 0.5F)
                                    {
                                        bU--;
                                    }
                                    else if (aU - bU > 0.5F)
                                    {
                                        bU++;
                                    }
                                    if (cU - aU > 0.5F)
                                    {
                                        cU--;
                                    }
                                    else if (aU - cU > 0.5F)
                                    {
                                        cU++;
                                    }
                                }
                                else
                                {
                                    if (bV - aV > 0.5F)
                                    {
                                        bV--;
                                    }
                                    else if (aV - bV > 0.5F)
                                    {
                                        bV++;
                                    }
                                    if (cV - aV > 0.5F)
                                    {
                                        cV--;
                                    }
                                    else if (aV - cV > 0.5F)
                                    {
                                        cV++;
                                    }
                                }

                                uvs.Add(new Vector2(aU >= 1 ? 1f : aU, aV >= 1 ? 1f : aV));
                                uvs.Add(new Vector2(bU >= 1 ? 1f : bU, bV >= 1 ? 1f : bV));
                                uvs.Add(new Vector2(cU >= 1 ? 1f : cU, cV >= 1 ? 1f : cV));
                            }
                            else
                            {
                                Debug.Log("unsupported type " + TriangleTextureMapTypes[texMapPos]);
                                uvs.Add(new Vector2(0, 0));
                                uvs.Add(new Vector2(0, 0));
                                uvs.Add(new Vector2(0, 0));
                            }
                        }
                    }
                }
                else
                {
                    uvs.Add(new Vector2(0, 0));
                    uvs.Add(new Vector2(0, 0));
                    uvs.Add(new Vector2(0, 0));
                }
            }

            lastMesh.subMeshCount = textureBatches.Count;
            lastMesh.vertices = vertices;
            lastMesh.normals = normals;
            lastMaterials = new Material[lastMesh.subMeshCount];

            for (int i = 0; i < textureBatches.Count; i++)
            {
                var seg = textureBatches[i];
                var material = new Material(ResourceCache.DiffuseShader);

                if (seg.Type == RenderType.Colored)
                {
                    var rgb = ColorUtils.HSLToRGBMap[seg.Color];
                    var alpha = seg.Opacity;
                    if (rgb == 0x0 || rgb == 0xFFFFFF || rgb == 0xFF00FF || rgb == 0xFFFCFC || rgb == 0xCC00CC)
                    {
                        alpha = 0;
                    }

                    if (alpha < 255)
                    {
                        material.shader = ResourceCache.CutoutTransparentDiffuseShader;
                    }
                    material.color = ColorUtils.RGBToColor(rgb, alpha);
                }
                else if (seg.Type == RenderType.Textured)
                {
                    var tex = GameContext.MaterialPool.GetTextureAsUnity(seg.Texture);
                    if (GameContext.MaterialPool.HasTrasparency(seg.Texture))
                    {
                        material.shader = ResourceCache.CutoutTransparentDiffuseShader;
                    }

                    var rgb = ColorUtils.HSLToRGBMap[seg.Color & 0xFFFF];
                    var alpha = seg.Opacity;
                    if (rgb == 0x0 || rgb == 0xFFFFFF || rgb == 0xFF00FF || rgb == 0xFFFCFC || rgb == 0xCC00CC)
                    {
                        alpha = 0;
                    }

                    if (alpha < 255)
                    {
                        material.shader = ResourceCache.CutoutTransparentDiffuseShader;
                    }

                    material.color = ColorUtils.RGBToColor(rgb, alpha);
                    material.mainTexture = tex;
                }

                lastMaterials[i] = material;
                lastMesh.SetTriangles(seg.Triangles.ToArray(), i);
            }

            meshRenderer.sharedMaterials = lastMaterials;
            lastMesh.uv = uvs.ToArray();
            lastMesh.RecalculateBounds();
            lastMesh.MarkDynamic();
        }
    }
}
