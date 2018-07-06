using UnityEngine;

namespace RS
{
    public class Animation
    {
        /// <summary>
        /// If the entity that this animation is being applied to can be rotated
        /// while the animation is being applied.
        /// </summary>
        public bool CanRotate;

        /// <summary>
        /// The number of frames in this animation.
        /// </summary>
        public int FrameCount;

        /// <summary>
        /// The length of each frame in this animation.
        /// </summary>
        public int[] FrameLengths;

        /// <summary>
        /// The index of each primary frame in this animation.
        /// </summary>
        public int[] FrameIndicesPrimary;

        /// <summary>
        /// The index of each secondary frame in this animation.
        /// </summary>
        public int[] FrameIndicesSecondary;

        /// <summary>
        /// The shield item that the player will have shadow equipped while
        /// this animation plays.
        /// 
        /// This field is specific to players.
        /// </summary>
        public int OverrideShieldIndex;

        /// <summary>
        /// The weapon item that the player will have shadow equipped while
        /// this animation plays.
        /// 
        /// This field is specific to players.
        /// </summary>
        public int OverrideWeaponIndex;

        /// <summary>
        /// The amount of padding to apply to his animation.
        /// </summary>
        public int Padding;

        /// <summary>
        /// The priority of this animation.
        /// 
        /// This is used for when animations are being stacked on top of one another. Highest priority wins.
        /// </summary>
        public int Priority;

        /// <summary>
        /// The cycle offset to stop this animation at.
        /// </summary>
        public int ResetCycle;

        /// <summary>
        /// The flag that determines how to handle speeding up/esyncing.
        /// </summary>
        public int SpeedFlag;

        /// <summary>
        /// The flag that determines how to handle stalling.
        /// </summary>
        public int WalkFlag;

        /// <summary>
        /// The type of animation.
        /// </summary>
        public int Type;

        /// <summary>
        /// The vertices that define what to manipulate during the animation.
        /// </summary>
        public int[] Vertices;
        
        /// <summary>
        /// Sets this animation state to default values.
        /// </summary>
        private void SetDefaults()
        {
            Padding = -1;
            CanRotate = false;
            Priority = 5;
            OverrideShieldIndex = -1;
            OverrideWeaponIndex = -1;
            ResetCycle = 99;
            SpeedFlag = -1;
            WalkFlag = -1;
            Type = 1;
        }

        private void ParseOpcode(int opcode, JagexBuffer b)
        {
            if (opcode == 1)
            {
                FrameCount = b.ReadUByte();
                FrameIndicesPrimary = new int[FrameCount];
                FrameIndicesSecondary = new int[FrameCount];
                FrameLengths = new int[FrameCount];

                for (var j = 0; j < FrameCount; j++)
                {
                    FrameIndicesPrimary[j] = b.ReadUShort();
                    FrameIndicesSecondary[j] = b.ReadUShort();
                    if (FrameIndicesSecondary[j] == 65535)
                    {
                        FrameIndicesSecondary[j] = -1;
                    }

                    FrameLengths[j] = b.ReadUShort();
                }
            }
            else if (opcode == 2)
            {
                Padding = b.ReadShort();
            }
            else if (opcode == 3)
            {
                int k = b.ReadUByte();
                Vertices = new int[k + 1];
                for (int j = 0; j < k; j++)
                {
                    Vertices[j] = b.ReadUByte();
                }
                Vertices[k] = 0x98967F;
            }
            else if (opcode == 4)
            {
                CanRotate = true;
            }
            else if (opcode == 5)
            {
                Priority = b.ReadUByte();
            }
            else if (opcode == 6)
            {
                OverrideShieldIndex = b.ReadShort();
            }
            else if (opcode == 7)
            {
                OverrideWeaponIndex = b.ReadShort();
            }
            else if (opcode == 8)
            {
                ResetCycle = b.ReadUByte();
            }
            else if (opcode == 9)
            {
                SpeedFlag = b.ReadUByte();
            }
            else if (opcode == 10)
            {
                WalkFlag = b.ReadUByte();
            }
            else if (opcode == 11)
            {
                Type = b.ReadUByte();
            }
            else
            {
                Debug.Log("Error unrecognised seq config code: " + opcode);
            }
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

        public Animation(JagexBuffer b)
        {
            SetDefaults();
            ParseFrom(b);

            if (FrameCount == 0)
            {
                FrameCount = 1;
                FrameIndicesPrimary = new int[1];
                FrameIndicesPrimary[0] = -1;
                FrameIndicesSecondary = new int[1];
                FrameIndicesSecondary[0] = -1;
                FrameLengths = new int[1];
                FrameLengths[0] = -1;
            }

            if (SpeedFlag == -1)
            {
                if (Vertices != null)
                {
                    SpeedFlag = 2;
                } else
                {
                    SpeedFlag = 0;
                }
            }

            if (WalkFlag == -1)
            {
                if (Vertices != null)
                {
                    WalkFlag = 2;
                } else
                {
                    WalkFlag = 0;
                }
            }
        }

        /// <summary>
        /// Retrieves the length of a frame in this animation.
        /// </summary>
        /// <param name="frame">The frame to retrieve the length of.</param>
        /// <returns>The length of the frame.</returns>
        public int GetFrameLength(int frame)
        {
            var length = FrameLengths[frame];

            if (length == 0)
            {
                SequenceFrame f = GameContext.Cache.GetSeqFrame(this.FrameIndicesPrimary[frame]);
                if (f != null)
                {
                    length = FrameLengths[frame] = f.Length;
                }
            }

            if (length == 0)
            {
                length = 1;
            }

            return length;
        }
    }

    public class SequenceProvider : IndexedProvider<Animation>
    {
        private Animation[] instance;
        private int count;

        public SequenceProvider(CacheArchive a)
        {
            var b = new DefaultJagexBuffer(a.GetFile("seq.dat"));

            count = b.ReadUShort();
            instance = new Animation[count];
            for (int i = 0; i < count; i++)
            {
                instance[i] = new Animation(b);
            }
        }

        public Animation Provide(int index)
        {
            return instance[index];
        }
    }

}

