using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace RS
{
    public class AnimatedObject : SceneObject
    {
        public Animation Seq;
        public int SeqCycle;
        public int Cycle;
        public int Index;
        public int[] OverrideIndex;
        public int Rotation;
        public int SettingIndex;
        public int Type;
        public int HeightNorthEast;
        public int HeightNorthWest;
        public int HeightSouthEast;
        public int HeightSouthWest;
        public int VarBitIndex;

        public GameObject UnityObject;

        public bool ModelDirty = true;
        public Model OurModel;
        public Model TempModel = new Model();

        public int LastAppliedFrame = -1;

        public AnimatedObject(int index, int rotation, int type, int heightSe, int heightNe, int heightSw, int heightNw, int seq, bool randomFrame)
        {
            Index = index;
            Type = type;
            Rotation = rotation;
            HeightSouthWest = heightSw;
            HeightSouthEast = heightSe;
            HeightNorthEast = heightNe;
            HeightNorthWest = heightNw;

            if (seq != -1)
            {
                Seq = GameContext.Cache.GetSeq(seq);
                SeqCycle = 0;
                Cycle = (int)GameContext.LoopCycle;
                if (randomFrame && Seq.Padding != -1)
                {
                    SeqCycle = Seq.FrameCount - 1;
                    if (SeqCycle >= 0)
                    {
                        Cycle -= Seq.GetFrameLength(SeqCycle);
                    }
                }
            }

            var config = GameContext.Cache.GetObjectConfig(Index);
            VarBitIndex = config.varBitId;
            SettingIndex = config.sessionSettingId;
            OverrideIndex = config.childrenIds;
        }

        public void Init()
        {
            UnityObject = new GameObject();
            UnityObject.name = "AnimatedObject " + Index + " " + Rotation + " " + Type;
        }

        /// <summary>
        /// Builds an object that reflects this animated object.
        /// </summary>
        /// <returns>The built model.</returns>
        private Model BuildModel()
        {
            var config = GameContext.Cache.GetObjectConfig(Index);
            Model model = null;
            if (config != null)
            {
                model = config.GetModel(Type, Rotation, HeightSouthWest, HeightSouthEast, HeightNorthEast, HeightNorthWest, false);
            }
            return model;
        }

        /// <summary>
        /// Adds a collider for this object, which allows for interaction.
        /// </summary>
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

        /// <summary>
        /// Reclaculates this model if it is dirty, and retrieves it.
        /// </summary>
        /// <returns>This object's model.</returns>
        public Model GetModel()
        {
            if (ModelDirty)
            {
                ModelDirty = false;
                TempModel.ClearAttachments();
                OurModel = BuildModel();
                if (OurModel != null)
                {
                    OurModel.Backing = UnityObject;
                    OurModel.AddMeshToObject();
                    AddCollider();
                }
                
                TempModel.Backing = UnityObject;
            }
            return OurModel;
        }

        /// <summary>
        /// Calculates the animation frame that this object is currently playing.
        /// </summary>
        /// <returns>The index of the animation frame that this object is playing.</returns>
        private int CalcAnimationFrame()
        {
            var frame = -1;
            if (Seq != null)
            {
                int deltaCycle = (int)GameContext.LoopCycle - Cycle;
                if (deltaCycle > 100 && Seq.Padding > 0)
                {
                    deltaCycle = 100;
                }

                while (deltaCycle > Seq.GetFrameLength(SeqCycle))
                {
                    deltaCycle -= Seq.GetFrameLength(SeqCycle);
                    SeqCycle++;
                    if (SeqCycle < Seq.FrameCount)
                    {
                        continue;
                    }

                    SeqCycle -= Seq.Padding;
                    if (SeqCycle >= 0 && SeqCycle < Seq.FrameCount)
                    {
                        continue;
                    }

                    Seq = null;
                    break;
                }

                Cycle = (int)GameContext.LoopCycle - deltaCycle;
                if (Seq != null)
                {
                    frame = Seq.FrameIndicesPrimary[SeqCycle];
                }
            }
            return frame;
        }

        /// <summary>
        /// Applies animations to this object's model.
        /// </summary>
        public void ApplyAnimations()
        {
            var frame = CalcAnimationFrame();
            if (LastAppliedFrame == frame)
                return;

            var model = GetModel();
            var unityVisible = UnityObject.GetComponent<Renderer>().isVisible;
            if (model != null && unityVisible)
            {
                model.ApplyVertexWeights();
                TempModel.Replace(model, (frame == -1));
                if (frame != -1)
                {
                    TempModel.ApplySequenceFrame(frame);
                }

                var config = GameContext.Cache.GetObjectConfig(Index);
                config.ApplyPostAnimate(TempModel, Rotation);
            }
            
            LastAppliedFrame = frame;
        }

        public override void Update()
        {
            ApplyAnimations();
        }

        /// <summary>
        /// If this object is visible.
        /// </summary>
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
