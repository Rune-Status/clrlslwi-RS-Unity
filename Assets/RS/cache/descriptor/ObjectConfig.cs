using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LitJson;

using UnityEngine;

namespace RS
{
    /// <summary>
    /// A config that describes a map object.
    /// </summary>
    public class ObjectConfig
    {
        private static Dictionary<long, Model> cachedModels = new Dictionary<long, Model>();
        private static Dictionary<long, Model> staticCachedModels = new Dictionary<long, Model>();
        private static Model[] tmpModel = new Model[4];

        public string[] actions;
        public bool fitsToTerrain;
        public bool blocksProjectiles;
        public int brightness;
        public bool castsShadow;
        public string desc;
        public int faceFlags;
        public bool flatShading;
        public bool ghost;
        public bool hasCollisions;
        public int icon;
        public int index;
        public bool isDecoration;
        public bool isSolid;
        public bool isStatic;
        public int[] modelIndices;
        public int[] modelTypes;
        public string name;
        public int[] newColor;
        public int offsetX;
        public int offsetY;
        public int offsetZ;
        public int[] oldColor;
        public int[] childrenIds;
        public int raisesItemPiles;
        public bool rotateCcw;
        public int scaleX;
        public int scaleY;
        public int scaleZ;
        public int sceneImageIndex;
        public int seqIndex;
        public int sessionSettingId;
        public int sizeX;
        public int sizeY;
        public int specular;
        public int varBitId;
        public int wallWidth;

        public ObjectConfig(JagexBuffer b)
        {
            SetDefaults();
            ParseFrom(b);
        }

        private void ParseFrom(JagexBuffer b)
        {
            int opcode = b.ReadUByte();
            while (opcode != 0)
            {
                ParseOpcode(opcode, b);
                opcode = b.ReadUByte();
            }
        }

        private void ParseOpcode(int opcode, JagexBuffer b)
        {
            if (opcode == 1)
            {
                int count = b.ReadUByte();
                if (count > 0)
                {
                    if (modelIndices == null)
                    {
                        modelIndices = new int[count];
                        modelTypes = new int[count];
                        for (int j = 0; j < count; j++)
                        {
                            modelIndices[j] = b.ReadUShort();
                            modelTypes[j] = b.ReadByte();
                        }
                    }
                    else {
                        b.Position(b.Position() + count * 3);
                    }
                }
            }
            else if (opcode == 2)
            {
                name = b.ReadString(10);
            }
            else if (opcode == 3)
            {
                desc = b.ReadString(10);
            }
            else if (opcode == 5)
            {
                int count = b.ReadUByte();
                if (count > 0)
                {
                    if (modelIndices == null)
                    {
                        modelTypes = null;
                        modelIndices = new int[count];
                        for (int j = 0; j < count; j++)
                        {
                            modelIndices[j] = b.ReadUShort();
                        }
                    }
                    else {
                        b.Position(b.Position() + count * 2);
                    }
                }
            }
            else if (opcode == 14)
            {
                sizeX = (byte)b.ReadUByte();
            }
            else if (opcode == 15)
            {
                sizeY = (byte)b.ReadUByte();
            }
            else if (opcode == 17)
            {
                hasCollisions = false;
            }
            else if (opcode == 18)
            {
                blocksProjectiles = false;
            }
            else if (opcode == 19)
            {
                if (b.ReadUByte() == 1)
                {
                    isStatic = true;
                }
            }
            else if (opcode == 21)
            {
                fitsToTerrain = true;
            }
            else if (opcode == 22)
            {
                flatShading = true;
            }
            else if (opcode == 23)
            {
                isSolid = true;
            }
            else if (opcode == 24)
            {
                seqIndex = b.ReadUShort();
            }
            else if (opcode == 28)
            {
                wallWidth = b.ReadByte();
            }
            else if (opcode == 29)
            {
                brightness = b.ReadByte();
            }
            else if (opcode == 39)
            {
                specular = b.ReadByte();
            }
            else if (opcode >= 30 && opcode < 39)
            {
                if (actions == null)
                {
                    actions = new string[5];
                }

                actions[opcode - 30] = b.ReadString(10);

                if (actions[opcode - 30].Equals("hidden", StringComparison.CurrentCultureIgnoreCase))
                {
                    actions[opcode - 30] = null;
                }
            }
            else if (opcode == 40)
            {
                int j = b.ReadUByte();
                oldColor = new int[j];
                newColor = new int[j];

                for (int k = 0; k < j; k++)
                {
                    oldColor[k] = b.ReadUShort();
                    newColor[k] = b.ReadUShort();
                }
            }
            else if (opcode == 60)
            {
                icon = b.ReadUShort();
            }
            else if (opcode == 62)
            {
                rotateCcw = true;
            }
            else if (opcode == 64)
            {
                castsShadow = false;
            }
            else if (opcode == 65)
            {
                scaleX = b.ReadUShort();
            }
            else if (opcode == 66)
            {
                scaleY = b.ReadUShort();
            }
            else if (opcode == 67)
            {
                scaleZ = b.ReadUShort();
            }
            else if (opcode == 68)
            {
                sceneImageIndex = b.ReadUShort();
            }
            else if (opcode == 69)
            {
                faceFlags = b.ReadUByte();
            }
            else if (opcode == 70)
            {
                offsetX = b.ReadUShort();
            }
            else if (opcode == 71)
            {
                offsetY = b.ReadUShort();
            }
            else if (opcode == 72)
            {
                offsetZ = b.ReadUShort();
            }
            else if (opcode == 73)
            {
                isDecoration = true;
            }
            else if (opcode == 74)
            {
                ghost = true;
            }
            else if (opcode == 75)
            {
                raisesItemPiles = b.ReadUByte();
            }
            else if (opcode == 77)
            {
                varBitId = b.ReadUShort();
                sessionSettingId = b.ReadUShort();
                childrenIds = new int[b.ReadUByte() + 1];

                for (int j = 0; j <= childrenIds.Length - 1; j++)
                {
                    childrenIds[j] = b.ReadUShort();
                }
            }
        }

        public void SetDefaults()
        {
            modelIndices = null;
            modelTypes = null;
            name = null;
            desc = null;
            oldColor = null;
            newColor = null;
            sizeX = 1;
            sizeY = 1;
            hasCollisions = true;
            blocksProjectiles = true;
            isStatic = false;
            fitsToTerrain = false;
            flatShading = false;
            isSolid = false;
            seqIndex = -1;
            wallWidth = 16;
            brightness = 0;
            specular = 0;
            actions = null;
            icon = -1;
            sceneImageIndex = -1;
            rotateCcw = false;
            castsShadow = true;
            scaleX = 128;
            scaleY = 128;
            scaleZ = 128;
            faceFlags = 0;
            offsetX = 0;
            offsetY = 0;
            offsetZ = 0;
            isDecoration = false;
            ghost = false;
            raisesItemPiles = -1;
            varBitId = -1;
            sessionSettingId = -1;
            childrenIds = null;
        }

        public ObjectConfig GetOverrideConfig()
        {
            int i = -1;

            if (varBitId != -1)
            {
                /*VarBit varbit = VarBit.instance[varBitId];
                int j = varbit.setting;
                int k = varbit.offset;
                int l = varbit.shift;
                int i1 = Game.LSB_BIT_MASK[l - k];
                i = Game.settings[j] >> k & i1;*/
                i = 0;
            }
            else if (sessionSettingId != -1)
            {
                i = GameContext.Settings[sessionSettingId];
            }

            if (i < 0 || i >= childrenIds.Length || childrenIds[i] == -1)
            {
                return null;
            }

            return GameContext.Cache.GetObjectConfig(childrenIds[i]);
        }

        public Model GetAssembledModel(int type, int rotation, bool postAnimate = true)
        {
            Model m = null;
            long uid;

            if (modelTypes == null)
            {
                if (type != 10)
                {
                    return null;
                }

                uid = (index << 6) + rotation;
                if (cachedModels.TryGetValue(uid, out m))
                {
                    return m;
                }

                if (modelIndices == null)
                {
                    return null;
                }

                bool rotate = rotateCcw ^ (rotation > 3);
                int count = modelIndices.Length;

                for (int j = 0; j < modelIndices.Length; j++)
                {
                    int xindex = modelIndices[j];

                    if (rotate)
                    {
                        xindex += 0x10000;
                    }

                    m = null;
                    staticCachedModels.TryGetValue(xindex, out m);
                    if (m == null)
                    {
                        m = new Model(xindex & 0xffff);

                        if (m == null)
                        {
                            return null;
                        }

                        if (rotate)
                        {
                            m.RotateCCW();
                        }

                        staticCachedModels.Add(xindex, m);
                    }

                    if (count > 1)
                    {
                        tmpModel[j] = m;
                    }
                }

                if (count > 1)
                {
                    m = new Model(count, tmpModel);
                }
            }
            else
            {
                var modelListIndex = -1;
                for (int i = 0; i < modelTypes.Length; i++)
                {
                    if (modelTypes[i] != type)
                    {
                        continue;
                    }
                    modelListIndex = i;
                    break;
                }

                if (modelListIndex == -1)
                {
                    return null;
                }

                uid = (index << 6) + (modelListIndex << 3) + rotation;
                Model cachedMesh;
                if (cachedModels.TryGetValue(uid, out cachedMesh))
                {
                    return cachedMesh;
                }

                var modelIndex = modelIndices[modelListIndex];
                bool rotate = rotateCcw ^ (rotation > 3);

                if (rotate)
                {
                    modelIndex += 0x10000;
                }

                m = null;
                staticCachedModels.TryGetValue(modelIndex, out m);

                if (m == null)
                {
                    m = new Model(modelIndex & 0xffff);

                    if (m == null)
                    {
                        return null;
                    }

                    if (rotate)
                    {
                        m.RotateCCW();
                    }

                    staticCachedModels.Add(modelIndex, m);
                }
            }

            var rescale = scaleX != 128 || scaleY != 128 || scaleZ != 128;
            var translate = offsetX != 0 || offsetY != 0 || offsetZ != 0;
            var model = new Model(oldColor == null, true, rotation == 0 && !rescale && !translate, m);
            if (postAnimate)
            {
                while (rotation-- > 0)
                {
                    model.RotateCW();
                }

                if (oldColor != null)
                {
                    model.SetColors(oldColor, newColor);
                }

                if (rescale)
                {
                    model.Scale(scaleX, scaleY, scaleZ);
                }

                if (translate)
                {
                    model.Translate(offsetX, offsetY, offsetZ);
                }
            }

           // model.ApplyLighting(64 + brightness, 768 + specular * 5, -50, -10, -50, !flatShading);

            cachedModels.Add(uid, model);
            return model;
        }

        public Model ApplyPostAnimate(Model model, int rotation)
        {
            var rescale = scaleX != 128 || scaleY != 128 || scaleZ != 128;
            var translate = offsetX != 0 || offsetY != 0 || offsetZ != 0;
            while (rotation-- > 0)
            {
                model.RotateCW();
            }

            if (oldColor != null)
            {
                model.SetColors(oldColor, newColor);
            }

            if (rescale)
            {
                model.Scale(scaleX, scaleY, scaleZ);
            }

            if (translate)
            {
                model.Translate(offsetX, offsetY, offsetZ);
            }
            model.PushVertexData();
            return model;
        }

        /// <summary>
        /// Retrieves a model that reflects an object described by this config.
        /// </summary>
        /// <param name="type">The type of object to create.</param>
        /// <param name="rotation">The rotation of the object to create.</param>
        /// <param name="southWestHeight">The south west height of the object.</param>
        /// <param name="southEastHeight">The south east height of the object.</param>
        /// <param name="northEastHeight">The north east height of the object.</param>
        /// <param name="northWestHeight">The north west height of the object.</param>
        /// <returns>The model of the object.</returns>
        public Model GetModel(int type, int rotation, int southWestHeight, int southEastHeight, int northEastHeight, int northWestHeight, bool postAnimate = true)
        {
            var m = GetAssembledModel(type, rotation, postAnimate);
            if (m == null)
            {
                return null;
            }

            if (fitsToTerrain || flatShading)
            {
                m = new Model(fitsToTerrain, flatShading, m);
            }

            if (fitsToTerrain)
            {
                var averageHeight = (southWestHeight + southEastHeight + northEastHeight + northWestHeight) / 4;
                for (var i = 0; i < m.VertexCount; i++)
                {
                    var x = m.VertexX[i];
                    var z = m.VertexZ[i];
                    var southHeight = southWestHeight + ((southEastHeight - southWestHeight) * (x + 64)) / 128;
                    var northHeight = northWestHeight + ((northEastHeight - northWestHeight) * (x + 64)) / 128;
                    var height = southHeight + ((northHeight - southHeight) * (z + 64)) / 128;
                    m.VertexY[i] -= (height - averageHeight);
                }
            }

            return m;
        }
    }

    public class ObjectConfigProvider : IndexedProvider<ObjectConfig>
    {
        private List<ObjectConfig> tmpCache = new List<ObjectConfig>();
        private int[] pointer;
        private JagexBuffer dataStream;

        public ObjectConfigProvider(CacheArchive archive)
        {
            dataStream = new DefaultJagexBuffer(archive.GetFile("loc.dat"));
            var idxStream = new DefaultJagexBuffer(archive.GetFile("loc.idx"));
            pointer = new int[idxStream.ReadUShort()];

            var position = 2;
            for (var i = 0; i < pointer.Length; i++)
            {
                pointer[i] = position;
                position += idxStream.ReadUShort();
            }
        }

        /// <summary>
        /// Removes all excess elements from the cache.
        /// </summary>
        public void RemoveExcess()
        {
            while (tmpCache.Count > 15)
            {
                tmpCache.RemoveAt(0);
            }
        }

        public ObjectConfig Provide(int index)
        {
            foreach (var cached in tmpCache)
            {
                if (cached.index == index)
                {
                    return cached;
                }
            }

            if (index < 0 || index >= pointer.Length)
            {
                throw new Exception("Object with index " + index + " does not exist!");
            }

            dataStream.Position(pointer[index]);
            var config = new ObjectConfig(dataStream);
            config.index = index;
            tmpCache.Add(config);

            RemoveExcess();
            return config;
        }
    }
}
