using System;

using UnityEngine;

namespace RS
{
    /// <summary>
    /// Represents a projectile within the scene.
    /// </summary>
    public class Projectile
    {
        public double NextSlopeAmount;
        public int ArcSize;
        public int CycleEnd;
        public int CycleStart;
        public bool Mobile;
        public int OffsetZ;
        public int ParentSize;
        public int Pitch;
        public int Plane;
        public int Rotation;
        public double SceneX;
        public double SceneY;
        public double SceneZ;
        public float SeqCycle;
        public int SeqFrame;
        public double Speed;
        public double SpeedX;
        public double SpeedY;
        public double SpeedZ;
        public GraphicConfig GraphicDesc;
        public int StartX;
        public int StartY;
        public int StartZ;

        /// <summary>
        /// The index of the target entity.
        /// </summary>
        public int TargetIndex;

        /// <summary>
        /// The last applied animation frame.
        /// </summary>
        public int LastAppliedFrame = -1;

        /// <summary>
        /// The unity object backing this projectile.
        /// </summary>
        public GameObject UnityObject = new GameObject();

        /// <summary>
        /// If the model is dirty, and needs recalculating.
        /// </summary>
        public bool Dirty = true;
        
        /// <summary>
        /// The cached projectile model.
        /// </summary>
        public Model Model;

        /// <summary>
        /// A temporary model used during calculations.
        /// </summary>
        public Model TempModel = new Model();

        public Projectile(int arcSize, int endZ, int cycleStart, int cycleEnd, int parentSize, int plane, int startZ, int startY, int startX, int target, int graphicIndex)
        {
            Mobile = false;
            GraphicDesc = GameContext.Cache.GetGraphicConfig(graphicIndex);
            Plane = plane;
            StartX = startX;
            StartY = startY;
            StartZ = startZ;
            CycleStart = cycleStart;
            CycleEnd = cycleEnd;
            ArcSize = arcSize;
            ParentSize = parentSize;
            TargetIndex = target;
            OffsetZ = endZ;
            Mobile = false;
        }

        /// <summary>
        /// Builds this projectile's model based on current state.
        /// </summary>
        /// <returns>The built model.</returns>
        public Model BuildModel()
        {
            Model model = GraphicDesc.GetModel();
            if (model == null)
            {
                return null;
            }

            Model m = new Model(true, false, false, model);
            m.ApplyVertexWeights();

            return m;
        }

        /// <summary>
        /// Calculates the frame of the animation to play.
        /// </summary>
        /// <returns>The frame of the animation to play.</returns>
        public int GetFrame()
        {
            int frame = -1;
            if (GraphicDesc.Sequence != null)
            {
                frame = GraphicDesc.Sequence.FrameIndicesPrimary[SeqFrame];
            }
            return frame;
        }

        /// <summary>
        /// Applies animations to this projectile.
        /// </summary>
        public void ApplyAnimations()
        {
            var frame = GetFrame();
            var model = GetModel();
            if (model != null)
            {
                model.ApplyVertexWeights();
                TempModel.Replace(model, (frame == -1));

                if (frame != -1)
                {
                    TempModel.ApplySequenceFrame(frame);
                }
                ApplyPostAnimate(TempModel);
            }

            LastAppliedFrame = frame;
        }

        /// <summary>
        /// Adds a collider to this projectile so we can interact with it.
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
        /// Retrieves the model of this projectile.
        /// 
        /// The model is recalculated if dirty.
        /// </summary>
        /// <returns>The model of this projectile.</returns>
        public Model GetModel()
        {
            if (Dirty)
            {
                Dirty = false;
                TempModel.ClearAttachments();
                Model = BuildModel();
                Model.Backing = UnityObject;
                Model.AddMeshToObject();
                AddCollider();
                TempModel.Backing = UnityObject;
            }
            return Model;
        }

        public Model ApplyPostAnimate(Model model)
        {
            if (GraphicDesc.Scale != 128 || GraphicDesc.Height != 128)
            {
                model.Scale(GraphicDesc.Scale, GraphicDesc.Height, GraphicDesc.Scale);
            }

            model.SetPitch(Pitch);
            model.PushVertexData();
            return model;
        }

        /// <summary>
        /// Performs a frame update to this projectile.
        /// </summary>
        /// <param name="cycle">The current cycle.</param>
        public void Update(int cycle)
        {
            Mobile = true;
            SceneX += SpeedX * cycle;
            SceneY += SpeedY * cycle;
            SceneZ -= SpeedZ * cycle + 0.5D * NextSlopeAmount * cycle * cycle;
            SpeedZ -= NextSlopeAmount * cycle;
            Rotation = (int)(Math.Atan2(SpeedX, SpeedY) * (32595 / 100)) + 1024 & 0x7ff;
            Pitch = (int)(Math.Atan2(SpeedZ, Speed) * (32595 / 100)) & 0x7ff;

            if (GraphicDesc.Sequence != null)
            {
                for (SeqCycle += cycle; SeqCycle > GraphicDesc.Sequence.GetFrameLength(SeqFrame);)
                {
                    SeqCycle -= GraphicDesc.Sequence.GetFrameLength(SeqFrame) + 1;
                    SeqFrame++;
                    if (SeqFrame >= GraphicDesc.Sequence.FrameCount)
                    {
                        SeqFrame = 0;
                    }
                }
            }
        }

        public void Update(int cycle, int x, int y, int z)
        {
            if (!Mobile)
            {
                double dx = x - StartX;
                double dy = y - StartY;
                double distance = Math.Sqrt(dx * dx + dy * dy);
                SceneX = StartX + (dx * ParentSize) / distance;
                SceneY = StartY + (dy * ParentSize) / distance;
                SceneZ = StartZ;
            }

            double dt = (CycleEnd + 1) - cycle;
            SpeedX = (x - SceneX) / dt;
            SpeedY = (y - SceneY) / dt;
            Speed = Math.Sqrt(SpeedX * SpeedX + SpeedY * SpeedY);

            if (!Mobile)
            {
                SpeedZ = -Speed * Math.Tan(ArcSize * (Math.PI / 128D));
            }

            NextSlopeAmount = (2D * (z - SceneZ - SpeedZ * dt)) / (dt * dt);
        }

        public void UpdateObject()
        {
            UnityObject.name = "Projectile " + GraphicDesc.ModelIndex + " " + SceneX + " " + SceneY + " " + SceneZ;
            ApplyAnimations();
            UnityObject.transform.position = new Vector3(GameConstants.RScale((int)SceneX), GameConstants.RScale((int)SceneZ), GameConstants.RScale((int)SceneY));
            UnityObject.transform.rotation = Quaternion.Euler(0, Rotation / 5.688888888888889f, 0);
        }

        public void Destroy()
        {
            GameObject.Destroy(UnityObject);
        }
    }
}
