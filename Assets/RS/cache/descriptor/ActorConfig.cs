namespace RS
{
    /// <summary>
    /// A config that describes an actor (NPC) within the game scene.
    /// </summary>
    public class ActorConfig
    {
        public bool RenderPriority;
        public string[] Action;
        public int Brightness;
        public short CombatLevel;
        public string Description;
        public short[] DialogModelIndices;
        public byte HasOptions;
        public sbyte HeadIcon;
        public int Index;
        public bool Interactable;
        public short[] ModelIndices;
        public short MoveAnim;
        public string Name;
        public int[] NewColors;
        public int[] OldColors;
        public short scaleXZ;
        public short scaleY;
        public short SettingIndex;
        public short[] OverrideIndices;
        public bool ShowOnMiniMap;
        public short Specular;
        public short StandAnim;
        public short Turn180Anim;
        public short TurnLeftAnim;
        public short TurnRightAnim;
        public short TurnSpeed;
        public short VarbitIndex;

        public ActorConfig()
        {
            SetDefaults();
        }

        public void SetDefaults()
        {
            VarbitIndex = -1;
            SettingIndex = -1;
            CombatLevel = -1;
            MoveAnim = -1;
            HasOptions = 1;
            HeadIcon = -1;
            Index = -1;
            TurnSpeed = 32;
            Interactable = true;
            scaleXZ = 128;
            scaleY = 128;
            ShowOnMiniMap = true;
            RenderPriority = false;
        }

        public Model GetDialogModel()
        {
            if (DialogModelIndices == null)
                return null;

            var models = new Model[DialogModelIndices.Length];
            for (var i = 0; i < DialogModelIndices.Length; i++)
            {
                models[i] = new Model(DialogModelIndices[i]);
            }

            Model m;
            if (models.Length == 1)
                m = models[0];
            else
                m = new Model(models.Length, models);

            if (OldColors != null)
            {
                m.SetColors(OldColors, NewColors);
            }
            return m;
        }

        public Model GetModel(int[] vertices, int frame1, int frame2)
        {
            var models = new Model[ModelIndices.Length];
            for (int i = 0; i < ModelIndices.Length; i++)
            {
                models[i] = new Model(ModelIndices[i]);
            }

            Model model = null;
            if (models.Length == 1)
                model = models[0];
            else
                model = new Model(models.Length, models);

            if (OldColors != null)
            {
                model.SetColors(OldColors, NewColors);
            }

            model.ApplyVertexWeights();
            model.Scale(scaleXZ, scaleY, scaleXZ);
            return model;
        }
    }
    
}
