namespace RS
{
    /// <summary>
    /// Represents a 'graphic'.
    /// 
    /// A graphic is an animated model in the scene that cannot be interacted with.
    /// It is typically animated, and sometimes moves (in the case of spells.)
    /// </summary>
    public class GraphicConfig
    {
        /// <summary>
        /// The index of the animation to apply to this graphic.
        /// </summary>
        public int SequenceIndex;
        /// <summary>
        /// The animation to apply to thi graphic.
        /// </summary>
        public Animation Sequence;
        public int Brightness;
        public int Height;
        public int ModelIndex;
        public int[] NewColors;
        public int[] OldColors;
        public int Rotation;
        public int Scale;
        public int Specular;
        public int UniqueId;

        public GraphicConfig(JagexBuffer s)
        {
            SetDefaults();

            var opcode = s.ReadUByte();
            while (opcode != 0)
            {
                if (opcode == 1)
                {
                    ModelIndex = s.ReadUShort();
                }
                else if (opcode == 2)
                {
                    SequenceIndex = s.ReadUShort();
                    Sequence = GameContext.Cache.GetSeq(SequenceIndex);
                }
                else if (opcode == 4)
                {
                    Scale = s.ReadUShort();
                }
                else if (opcode == 5)
                {
                    Height = s.ReadUShort();
                }
                else if (opcode == 6)
                {
                    Rotation = s.ReadUByte();
                }
                else if (opcode == 7)
                {
                    Brightness = s.ReadUByte();
                }
                else if (opcode == 8)
                {
                    Specular = s.ReadUByte();
                }

                opcode = s.ReadUByte();
            }

            int colorCumulative = 0;
            if (OldColors != null && OldColors.Length > 0)
            {
                foreach (var j in NewColors)
                {
                    colorCumulative += j;
                }
            }

            UniqueId = (ModelIndex << 16) | (SequenceIndex << 8);
            UniqueId += colorCumulative;
        }

        /// <summary>
        /// Sets this graphic to default values.
        /// </summary>
        public void SetDefaults()
        {
            SequenceIndex = -1;
            OldColors = new int[10];
            NewColors = new int[10];
            Scale = 128;
            Height = 128;
        }

        public Model GetModel()
        {
            var m = new Model(ModelIndex);
            if (m == null)
            {
                return null;
            }

            if (OldColors != null && OldColors[0] != 0)
            {
                m.SetColors(OldColors, NewColors);
            }
            
            return m;
        }
    }

    public class GraphicDescriptorProvider : IndexedProvider<GraphicConfig>
    {
        private GraphicConfig[] instance;
        private int count;

        public GraphicDescriptorProvider(CacheArchive a)
        {
            var buf = new DefaultJagexBuffer(a.GetFile("spotanim.dat"));
            count = buf.ReadUShort();
            instance = new GraphicConfig[count];

            for (int i = 0; i < count; i++)
            {
                instance[i] = new GraphicConfig(buf);
            }
        }

        public GraphicConfig Provide(int index)
        {
            return instance[index];
        }
    }
}
