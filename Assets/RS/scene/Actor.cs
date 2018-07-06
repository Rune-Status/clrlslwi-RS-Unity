
using UnityEngine;

namespace RS
{
    /// <summary>
    /// Represents an actor within the scene.
    /// </summary>
    public class Actor : Entity
    {
        /// <summary>
        /// The config describing this actor.
        /// </summary>
        private ActorConfig config;

        /// <summary>
        /// If this actor's model is dirty.
        /// </summary>
        public bool ModelDirty = true;

        /// <summary>
        /// The model of this actor.
        /// </summary>
        public Model Model;

        /// <summary>
        /// A temporary model we can do dirty things to.
        /// </summary>
        public Model TempModel = new Model();

        /// <summary>
        /// The graphic model of this actor.
        /// </summary>
        public Model GraphicModel;

        /// <summary>
        /// A temporary graphics model we can do dirty things to.
        /// </summary>
        public Model TempGraphicModel = new Model();

        /// <summary>
        /// If this actor's graphic model is dirty.
        /// </summary>
        public bool DirtyGraphic = true;

        public int LastAppliedFrame1 = -1;
        public int LastAppliedFrame2 = -1;
        public int LastAppliedInterpolateFrame = -1;

        /// <summary>
        /// The index of this actor on the game server.
        /// </summary>
        public int ServerIndex = 0;

        public Actor()
        {
            var comp = UnityObject.AddComponent<ActorComponent>();
            comp.GameEntity = this;
            comp.GameActor = this;
        }

        /// <summary>
        /// The config describing this actor.
        /// </summary>
        public ActorConfig Config
        {
            set
            {
                if (UnityObject != null)
                {
                    UnityObject.name = "NPC " + value;
                }

                config = value;
                ModelDirty = true;
            }
            get
            {
                return config;
            }
        }

        /// <summary>
        /// Builds this actor's model based on state information.
        /// </summary>
        /// <returns>This actor's built model.</returns>
        public Model BuildModel()
        {
            var frame1 = -1;
            if (SeqIndex >= 0 && SeqDelayCycle == 0)
            {
                frame1 = GameContext.Cache.GetSeq(SeqIndex).FrameIndicesPrimary[SeqFrame];
                var frame2 = -1;
                if (MoveSeqIndex >= 0 && MoveSeqIndex != StandAnimation)
                {
                    frame2 = GameContext.Cache.GetSeq(MoveSeqIndex).FrameIndicesPrimary[MoveSeqFrame];
                }

                return config.GetModel(GameContext.Cache.GetSeq(MoveSeqIndex).Vertices, frame1, frame2);
            }

            if (MoveSeqIndex >= 0)
            {
                frame1 = GameContext.Cache.GetSeq(MoveSeqIndex).FrameIndicesPrimary[MoveSeqFrame];
            }

            return config.GetModel(null, frame1, -1);
        }

        /// <summary>
        /// Adds a collider to this actor so that the user can interact with it.
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
        /// Applies animations to this actor's model.
        /// </summary>
        public void ApplyAnimations()
        {
            int[] vertices = new int[0];
            var frame1 = -1;
            var frame2 = -1;
            var interpolateFrame = -1;
            var cycle1 = 0;
            var cycle2 = 0;
            if (SeqIndex >= 0 && SeqDelayCycle == 0)
            {
                var a = GameContext.Cache.GetSeq(SeqIndex);
                if (a != null)
                {
                    frame1 = a.FrameIndicesPrimary[SeqFrame];
                }

                cycle1 = a.FrameLengths[SeqFrame];
                cycle2 = SeqCycle;
                if (MoveSeqIndex >= 0 && MoveSeqIndex != StandAnimation)
                {
                    var seq = GameContext.Cache.GetSeq(MoveSeqIndex);
                    if (seq != null)
                    {
                        frame2 = seq.FrameIndicesPrimary[MoveSeqFrame];
                        vertices = a.Vertices;
                    }
                }
            }
            else if (MoveSeqIndex >= 0)
            {
                var seq = GameContext.Cache.GetSeq(MoveSeqIndex);
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

            var model = GetModel();
            if (model == null) return;

            TempModel.Replace(model, (frame1 == -1) & (interpolateFrame == -1));
            if (frame1 != -1 && frame2 != -1)
            {
                TempModel.ApplySequenceFrames(vertices, frame1, frame2);
            }
            else if (frame1 != -1 && interpolateFrame != -1)
            {
                TempModel.ApplyAnimFrames(frame1, interpolateFrame, cycle1, cycle2);
            }
            else if (frame1 != -1)
            {
                TempModel.ApplySequenceFrame(frame1);
            }

            LastAppliedFrame1 = frame1;
            LastAppliedFrame2 = frame2;
            LastAppliedInterpolateFrame = interpolateFrame;

            //var graphicModel = GetGraphicModel();
            //if (graphicModel == null) return;

            //var combined = new Model(2, new Model[] { model, graphicModel });
        }

        /// <summary>
        /// Retrieves this actor's model.
        /// 
        /// This method calculates the model if it's dirty.
        /// </summary>
        /// <returns>This actor's model.</returns>
        public Model GetModel()
        {
            if (ModelDirty)
            {
                ModelDirty = false;
                Model = BuildModel();
                Model.Backing = UnityObject;
                Model.AddMeshToObject();
                TempModel.Backing = UnityObject;
                AddCollider();
            }

            return Model;
        }

        /// <summary>
        /// Retrieves this actor's graphic model.
        /// 
        /// This method calculates the graphic model if it's dirty.
        /// </summary>
        /// <returns>This actor's graphic model.</returns>
        public Model GetGraphicModel()
        {
            if (DirtyGraphic)
            {
                DirtyGraphic = false;
                GraphicModel = BuildModel();
                GraphicModel.Backing = UnityObject;
                GraphicModel.AddMeshToObject();
                TempGraphicModel.Backing = UnityObject;
            }

            return GraphicModel;
        }
    }
}
