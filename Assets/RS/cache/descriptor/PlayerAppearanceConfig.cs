using System;

using UnityEngine;

namespace RS
{
    public class PlayerAppearanceConfig
    {
        /// <summary>
        /// The body part this config applies to.
        /// </summary>
        public int Part;
        public short[] DialogModelIndex = { -1, -1, -1, -1, -1 };
        public short[] ModelIndex;
        public int[] NewColor;
        public int[] OldColor;
        public bool Unselectable;

        public PlayerAppearanceConfig()
        {
            SetDefaults();
        }

        /// <summary>
        /// Creates a player appearance config from binary data.
        /// </summary>
        /// <param name="b">The buffer containing the binary data.</param>
        public PlayerAppearanceConfig(JagexBuffer b)
        {
            SetDefaults();
            ParseFrom(b);
        }

        /// <summary>
        /// Updates this config to reflect the provided binary data.
        /// </summary>
        /// <param name="b">The binary data to parse.</param>
        private void ParseFrom(JagexBuffer b)
        {
            int opcode = b.ReadUByte();
            while (opcode != 0)
            {
                ParseOpcode(opcode, b);
                opcode = b.ReadUByte();
            }
        }

        /// <summary>
        /// Parses the next packet in the provided binary data.
        /// </summary>
        /// <param name="opcode">The opcode of the packet.</param>
        /// <param name="b">The binary data to decode.</param>
        private void ParseOpcode(int opcode, JagexBuffer b)
        {
            if (opcode == 1)
            {
                Part = b.ReadUByte();
            }
            else if (opcode == 2)
            {
                ModelIndex = new short[b.ReadUByte()];
                for (int k = 0; k < this.ModelIndex.Length; k++)
                {
                    ModelIndex[k] = (short)b.ReadUShort();
                }
            }
            else if (opcode == 3)
            {
                Unselectable = true;
            }
            else if (opcode >= 40 && opcode < 50)
            {
                OldColor[opcode - 40] = b.ReadUShort();
            }
            else if (opcode >= 50 && opcode < 60)
            {
                NewColor[opcode - 50] = b.ReadUShort();
            }
            else if (opcode >= 60 && opcode < 70)
            {
                DialogModelIndex[opcode - 60] = (short)b.ReadUShort();
            }
            else
            {
                Debug.Log("Error unrecognised config code: " + opcode);
            }
        }

        /// <summary>
        /// Sets this config to default value.
        /// </summary>
        public void SetDefaults()
        {
            Part = -1;
            OldColor = new int[6];
            NewColor = new int[6];
            Unselectable = false;
        }

        /// <summary>
        /// Retrieves a model for this appearance that should be shown on dialogues
        /// in place of certain body parts.
        /// </summary>
        /// <returns>The generated dialogue model.</returns>
        public Model GetDialogModel()
        {
            var models = new Model[5];

            var count = 0;
            foreach (var modelIdx in DialogModelIndex)
            {
                if (modelIdx != -1)
                {
                    models[count++] = new Model(modelIdx);
                }
            }

            var m = new Model(count, models);
            for (var i = 0; i < 6; i++)
            {
                if (OldColor[i] == 0)
                    break;
                else
                    m.SetColor(OldColor[i], NewColor[i]);
            }

            return m;
        }

        /// <summary>
        /// Retrieves the model for this appearance that should be shown
        /// in the place of certain body parts.
        /// </summary>
        /// <returns>The model to show.</returns>
        public Model GetModel()
        {
            if (ModelIndex == null)
            {
                return null;
            }

            var models = new Model[ModelIndex.Length];
            for (var i = 0; i < ModelIndex.Length; i++)
            {
                models[i] = new Model(ModelIndex[i]);
            }

            Model m;
            if (models.Length == 1)
                m = models[0];
            else
                m = new Model(models.Length, models);

            for (var i = 0; i < 6; i++)
            {
                if (OldColor[i] == 0)
                    break;
                else
                    m.SetColor(OldColor[i], NewColor[i]);
            }

            return m;
        }
        
        /// <summary>
        /// If this appearance has a valid dialog model.
        /// </summary>
        public bool IsDialogModelValid
        {
            get
            {
                bool valid = true;
                for (int i = 0; i < 5; i++)
                {

                }
                return valid;
            }
        }

        /// <summary>
        /// If this appearance has a valid model.
        /// </summary>
        public bool IsModelValid
        {
            get
            {
                return ModelIndex != null;
            }
        }
    }

    public class IdentityKitProvider : IndexedProvider<PlayerAppearanceConfig>
    {
        private int count;
        private PlayerAppearanceConfig[] instance;

        public IdentityKitProvider(CacheArchive a)
        {
            JagexBuffer s = new DefaultJagexBuffer(a.GetFile("idk.dat"));
            count = s.ReadUShort();
            instance = new PlayerAppearanceConfig[count];

            for (int i = 0; i < count; i++)
            {
                instance[i] = new PlayerAppearanceConfig(s);
            }
        }

        public PlayerAppearanceConfig Provide(int index)
        {
            return instance[index];
        }
    }
}

