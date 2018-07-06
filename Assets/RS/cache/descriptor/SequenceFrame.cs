using UnityEngine;

namespace RS
{
    public class SequenceFrame
    {
        public int FrameCount;
        public int Length;
        public SkinList Skinlist;
        public int[] VertexX;
        public int[] VertexY;
        public int[] VertexZ;
        public int[] Vertices;
    }

    public class SequenceFrameProvider : IndexedProvider<SequenceFrame>
    {
        private SequenceFrame[] instance;

        public SequenceFrameProvider()
        {
            
        }

        /// <summary>
        /// Initializes this provider with a set amount of frame cache slots.
        /// </summary>
        /// <param name="animCount">The amount of animations to pre-cache.</param>
        /// <param name="frameCount">The amount of frames to allocate room for.</param>
        public void Init(int animCount, int frameCount)
        {
            instance = new SequenceFrame[frameCount + 1];

            for (var i = 0; i < animCount; i++)
            {
                Load(i, GameContext.Cache.ReadCompressed(2, i));
            }
        }

        public SequenceFrame Provide(int i)
        {
            if (i < 0 || i >= instance.Length)
            {
                return null;
            }

            return instance[i];
        }

        public void Load(int findex, byte[] payload)
        {
            var s = new DefaultJagexBuffer(payload);
            s.Position(payload.Length - 8);

            int flagPos = s.ReadUShort();
            int modPos = s.ReadUShort();
            int lenPos = s.ReadUShort();
            int skinPos = s.ReadUShort();

            int position = 0;
            var infoStream = new DefaultJagexBuffer(payload);
            infoStream.Position(position);

            position += flagPos + 2;
            var flagStream = new DefaultJagexBuffer(payload);
            flagStream.Position(position);

            position += modPos;
            var modifierStream = new DefaultJagexBuffer(payload);
            modifierStream.Position(position);

            position += lenPos;
            var lengthStream = new DefaultJagexBuffer(payload);
            lengthStream.Position(position);

            position += skinPos;
            var skinStream = new DefaultJagexBuffer(payload);
            skinStream.Position(position);

            var sl = new SkinList(skinStream);

            var count = infoStream.ReadUShort();
            var skins = new int[500];
            var vertX = new int[500];
            var vertY = new int[500];
            var vertZ = new int[500];

            for (var i = 0; i < count; i++)
            {
                var id = infoStream.ReadUShort();
                if (id >= instance.Length)
                    continue;

                var a = instance[id] = new SequenceFrame();
                a.Length = lengthStream.ReadUByte();
                a.Skinlist = sl;

                var frameCount = infoStream.ReadUByte();
                var lastIdx = -1;
                var frameIdx = 0;
                for (var index = 0; index < frameCount; index++)
                {
                    var vertex_mask = flagStream.ReadUByte();
                    if (vertex_mask > 0)
                    {
                        if (sl.Opcodes[index] != 0)
                        {
                            for (var opp = index - 1; opp > lastIdx; opp--)
                            {
                                if (sl.Opcodes[opp] == 0)
                                {
                                    skins[frameIdx] = opp;
                                    vertX[frameIdx] = 0;
                                    vertY[frameIdx] = 0;
                                    vertZ[frameIdx] = 0;
                                    frameIdx++;
                                    break;
                                }
                            }
                        }

                        skins[frameIdx] = index;
                        var vertex = 0;
                        if (sl.Opcodes[index] == 3)
                            vertex = 128;

                        if ((vertex_mask & 1) != 0)
                            vertX[frameIdx] = modifierStream.ReadSmart();
                        else
                            vertX[frameIdx] = vertex;

                        if ((vertex_mask & 2) != 0)
                            vertY[frameIdx] = modifierStream.ReadSmart();
                        else
                            vertY[frameIdx] = vertex;

                        if ((vertex_mask & 4) != 0)
                            vertZ[frameIdx] = modifierStream.ReadSmart();
                        else
                            vertZ[frameIdx] = vertex;

                        lastIdx = index;
                        frameIdx++;
                    }
                }

                a.FrameCount = frameIdx;
                a.Vertices = new int[frameIdx];
                a.VertexX = new int[frameIdx];
                a.VertexY = new int[frameIdx];
                a.VertexZ = new int[frameIdx];
                for (var j = 0; j < frameIdx; j++)
                {
                    a.Vertices[j] = skins[j];
                    a.VertexX[j] = vertX[j];
                    a.VertexY[j] = vertY[j];
                    a.VertexZ[j] = vertZ[j];
                }
            }
        }
    }

}

