using System;

using UnityEngine;

namespace RS
{
    public class Player : Entity
    {
        public int Gender;
        public int PrayerIcon;
        public int SkullIcon;
        public int[] EquipmentIndices = new int[12];
        public int[] colors = new int[5];
        public string Name;
        public int CombatLevel;
        public bool Visible;
        public int SkillLevel;
        public long ModelUid;
        public long UniqueId;
        public bool Dirty = true;
        public Model OurModel;
        public Model TempModel = new Model();
        public int LastAppliedFrame1 = -1;
        public int LastAppliedFrame2 = -1;
        public int LastAppliedInterpolateFrame = -1;
        public int ServerIndex = 0;

        public Player()
        {
            var comp = UnityObject.AddComponent<PlayerComponent>();
            comp.GameEntity = this;
            comp.Player = this;
        }

        private Model BuildModel()
        {
            long modelUid = this.ModelUid;
            int shieldOverride = -1;
            int weaponOverride = -1;

            Model model = null;
            if (model == null)
            {
                var equipModels = new Model[12];
                var count = 0;

                for (var i = 0; i < 12; i++)
                {
                    int index = EquipmentIndices[i];
                    if (weaponOverride >= 0 && i == 3)
                    {
                        index = weaponOverride;
                    }

                    if (shieldOverride >= 0 && i == 5)
                    {
                        index = shieldOverride;
                    }
                    
                    if (index >= 256 && index < 512)
                    {
                        Model idModel = GameContext.Cache.GetPlayerAppearanceConfig(index - 256).GetModel();
                        if (idModel != null)
                        {
                            equipModels[count++] = idModel;
                        }
                    }

                    if (index >= 512)
                    {
                        var desc = GameContext.Cache.GetItemConfig(index - 512);
                        if (desc != null)
                        {
                            Model equipModel = desc.GetWornMesh(Gender);
                            if (equipModel != null)
                            {
                                equipModels[count++] = equipModel;
                            }
                        }
                    }
                }

                model = new Model(count, equipModels);
                for (int i = 0; i < 5; i++)
                {
                    if (colors[i] != 0)
                    {
                        //model.set_color(CharacterDesign.DESIGN_COLOR[i][0], CharacterDesign.DESIGN_COLOR[i][colors[i]]);
                        //if (i == 1) {
                        //	model.set_color(CharacterDesign.TORSO_COLORS[0], CharacterDesign.TORSO_COLORS[colors[i]]);
                        //}
                    }
                }

                model.ApplyVertexWeights();
                UniqueId = modelUid;
            }

            return model;
        }

        private void AddCollider()
        {
            var collider = UnityObject.GetComponent<MeshCollider>();
            if (collider == null)
            {
                collider = UnityObject.AddComponent<MeshCollider>();
            }
            collider.sharedMesh = UnityObject.GetComponent<MeshFilter>().mesh;
            collider.enabled = true;
        }

        public Model GetModel()
        {
            if (Dirty)
            {
                Dirty = false;
                TempModel.ClearAttachments();
                OurModel = BuildModel();
                OurModel.Backing = UnityObject;
                OurModel.AddMeshToObject();
                TempModel.Backing = UnityObject;
                AddCollider();
            }
            return OurModel;
        }

        public void ApplyAnimations()
        {
            var vertices = new int[0];
            var frame1 = -1;
            var frame2 = -1;
            var interpolateFrame = -1;
            var cycle1 = 0;
            var cycle2 = 0;

            if (SeqIndex >= 0 && SeqDelayCycle == 0)
            {
                Animation a = GameContext.Cache.GetSeq(SeqIndex);
                if (a != null)
                {
                    frame1 = a.FrameIndicesPrimary[SeqFrame];
                }

                cycle1 = a.FrameLengths[SeqFrame];
                cycle2 = SeqCycle;
                if (MoveSeqIndex >= 0 && MoveSeqIndex != StandAnimation)
                {
                    Animation seq = GameContext.Cache.GetSeq(MoveSeqIndex);
                    if (seq != null)
                    {
                        frame2 = seq.FrameIndicesPrimary[MoveSeqFrame];
                        vertices = a.Vertices;
                    }
                }
            }
            else if (MoveSeqIndex >= 0)
            {
                Animation seq = GameContext.Cache.GetSeq(MoveSeqIndex);
                if (seq != null)
                {
                    frame1 = seq.FrameIndicesPrimary[MoveSeqFrame];
                    interpolateFrame = seq.FrameIndicesPrimary[SeqNextIdleFrame];
                    cycle1 = seq.FrameLengths[MoveSeqFrame];
                    cycle2 = MoveSeqCycle;
                }
            }

            if (LastAppliedFrame1 == frame1 &&
                LastAppliedFrame2 == frame2 &&
                LastAppliedInterpolateFrame == interpolateFrame)
            {
                return;
            }

            TempModel.Replace(GetModel(), (frame1 == -1) & (frame2 == -1));

            if (frame1 != -1 && frame2 != -1)
            {
                TempModel.ApplySequenceFrames(vertices, frame1, frame2);
            }
            else if (frame1 != -1 && interpolateFrame != -1)
            {
                Debug.Log("Frame: " + frame1);
                TempModel.ApplyAnimFrames(frame1, interpolateFrame, cycle1, cycle2);
            }
            else if (frame1 != -1)
            {
                TempModel.ApplySequenceFrame(frame1, true);
            }

            LastAppliedFrame1 = frame1;
            LastAppliedFrame2 = frame2;
            LastAppliedInterpolateFrame = interpolateFrame;
        }

        public void Update(JagexBuffer b)
        {
            b.Position(0);

            Gender = b.ReadByte();
            PrayerIcon = b.ReadUByte();
            b.ReadByte();
            SkullIcon = b.ReadShort() == 0 ? -1 : 0;

            for (int i = 0; i < 12; i++)
            {
                int lsb = b.ReadUByte();

                if (lsb == 0)
                {
                    EquipmentIndices[i] = 0;
                    continue;
                }

                EquipmentIndices[i] = (lsb << 8) + b.ReadUByte();

                if (i == 0 && this.EquipmentIndices[0] == 65535)
                {
                    b.ReadUShort();
                    break;
                }
            }

            for (int i = 0; i < 5; i++)
            {
                colors[i] = b.ReadUByte();
            }

            StandAnimation = 625;//.ReadUShort();
            if (StandAnimation == 65535)
            {
                StandAnimation = -1;
            }

            StandTurnAnimation = b.ReadUShort();
            if (StandTurnAnimation == 65535)
            {
                StandTurnAnimation = -1;
            }

            WalkAnimation = b.ReadUShort();
            if (WalkAnimation == 65535)
            {
                WalkAnimation = -1;
            }

            Turn180Animation = b.ReadUShort();
            if (Turn180Animation == 65535)
            {
                Turn180Animation = -1;
            }

            TurnRightAnimation = b.ReadUShort();
            if (TurnRightAnimation == 65535)
            {
                TurnRightAnimation = -1;
            }

            TurnLeftAnimation = b.ReadUShort();
            if (TurnLeftAnimation == 65535)
            {
                TurnLeftAnimation = -1;
            }

            RunAnimation = b.ReadUShort();
            if (RunAnimation == 65535)
            {
                RunAnimation = -1;
            }

            this.Name = StringUtils.Format(StringUtils.LongToString(b.ReadLong()));
            this.CombatLevel = (short)b.ReadUByte();
            b.ReadUShort();
            b.ReadUShort();

            this.Visible = true;
            this.ModelUid = 0L;

            for (int i = 0; i < 12; i++)
            {
                this.ModelUid <<= 4;
                if (EquipmentIndices[i] >= 256)
                {
                    this.ModelUid += EquipmentIndices[i] - 256;
                }
            }

            if (EquipmentIndices[0] >= 256)
            {
                this.ModelUid += EquipmentIndices[0] - 256 >> 4;
            }

            if (EquipmentIndices[1] >= 256)
            {
                this.ModelUid += EquipmentIndices[1] - 256 >> 8;
            }

            for (int i = 0; i < 5; i++)
            {
                this.ModelUid <<= 3;
                this.ModelUid += colors[i];
            }

            this.ModelUid <<= 1;
            this.ModelUid += Gender;

            UnityObject.name = "Player " + Name;
            Dirty = true;
            UpdateObjectScenePos();
        }

        public bool IsVisible
        {
            get
            {
                var comp = UnityObject.GetComponent<Renderer>();
                if (comp == null)
                {
                    return true;
                }
                return comp.isVisible;
            }
        }
    }
}

