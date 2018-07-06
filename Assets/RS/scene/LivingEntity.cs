
using UnityEngine;

namespace RS
{
    /// <summary>
    /// Represents a living entity within the game scene.
    /// </summary>
    public class Entity
    {
        /// <summary>
        /// The last cycle this entity was updated on.
        /// 
        /// This is used to track which entities were not updated on cycles
        /// containing entity update packets (player, npcs.)
        /// </summary>
        public int UpdateCycle = 0;

        /// <summary>
        /// The move position within the movement queue.
        /// </summary>
        public int PathPosition = 0;

        /// <summary>
        /// The move x destination coordinates within the movement queue.
        /// </summary>
        public int[] PathX = new int[10];

        /// <summary>
        /// The move y destination coordinates within the movement queue.
        /// </summary>
        public int[] PathY = new int[10];

        /// <summary>
        /// The run state for each move within the movement queue.
        /// </summary>
        public bool[] PathRun = new bool[10];

        /// <summary>
        /// The current still path position.
        /// 
        /// This is used for stalling an entity in one place for a period of time (?)
        /// </summary>
        public int StillPathPosition;

        /// <summary>
        /// Calculates how much synchronizing is needed to make the entity land at the correct position
        /// in the movement queue.
        /// 
        /// It also makes the entity move extremely fast (see: item stalling.)
        /// </summary>
        public int ResyncWalkCycle = 0;

        /// <summary>
        /// This entity's size, in tiles.
        /// </summary>
        public int TileSize = 1;

        public int SeqCycle;
        public int SeqFrame;
        public int SeqNextIdleFrame;
        public int SeqIndex = -1;
        public int SeqResetCycle;
        public int SeqDelayCycle;
        public int MoveSeqCycle;
        public int MoveSeqFrame;
        public int MoveSeqIndex;
        public int MoveCycleEnd;
        public int MoveCycleStart;
        public int MoveDirection;
        public int MoveEndX;
        public int MoveEndY;
        public int MoveStartX;
        public int MoveStartY;
        public int SpotAnimIndex;
        public int GraphicOffsetY;
        
        public int SpotAnimCycleEnd;
        public int SpotAnimFrame;
        public int SpotAnimCycle;

        /// <summary>
        /// The color of the spoken text.
        /// </summary>
        public int SpokenColor;

        /// <summary>
        /// The effect of the spoken text.
        /// </summary>
        public int SpokenEffect;

        /// <summary>
        /// The life of the spoken text.
        /// </summary>
        public int SpokenLife;

        /// <summary>
        /// The spoken text message.
        /// </summary>
        public string SpokenMessage;

        /// <summary>
        /// The texture with the spoken message baked onto it.
        /// </summary>
        public Texture2D SpokenTex;
      
        /// <summary>
        /// The id of the entity that this entity is facing.
        /// </summary>
        public int FaceEntity = -1;

        /// <summary>
        /// The x tile coordinate this entity is facing.
        /// </summary>
        public int FaceX;

        /// <summary>
        /// The y tile coordinate this entity is facing.
        /// </summary>
        public int FaceY;

        /// <summary>
        /// The index of this entity's stand animation.
        /// </summary>
        public int StandAnimation;

        /// <summary>
        /// The index of this entity's turning while standing still animation.
        /// </summary>
        public int StandTurnAnimation;

        /// <summary>
        /// The index of this entity's turn 180 animation.
        /// </summary>
        public int Turn180Animation;

        /// <summary>
        /// The index of this entity's turn left animation.
        /// </summary>
        public int TurnLeftAnimation;

        /// <summary>
        /// The index of this entity's turn right animation.
        /// </summary>
        public int TurnRightAnimation;

        /// <summary>
        /// The index of this entity's walk animation.
        /// </summary>
        public int WalkAnimation;

        /// <summary>
        /// The index of this entity's run animation.
        /// </summary>
        public int RunAnimation;

        /// <summary>
        /// If this entity can rotate.
        /// </summary>
        public bool CanRotate = true;

        /// <summary>
        /// This entity's current rotation angle.
        /// </summary>
        public int Rotation;

        /// <summary>
        /// This entity's destination rotation angle.
        /// </summary>
        public int DestRotation;

        /// <summary>
        /// The speed at which this entity turns.
        /// </summary>
        public int RotateSpeed = 32;
        
        /// <summary>
        /// A list containing the cycles that each hit will render up to.
        /// </summary>
        public int[] HitBeginCycles = new int[4];

        /// <summary>
        /// A list containing the damage amounts of each hit queued on this entity.
        /// </summary>
        public int[] HitDamages = new int[4];

        /// <summary>
        /// A list containing the type of damage being applied for each hit queued on this entity.
        /// </summary>
        public int[] HitType = new int[4];

        /// <summary>
        /// The current health of this entity.
        /// </summary>
        public int CurrentHealth;

        /// <summary>
        /// The maximum health of this entity.
        /// </summary>
        public int MaxHealth;

        /// <summary>
        /// The last cycle that this entity's combat state will be shown on.
        /// 
        /// This is continuously pushed forward while an entity is in combat, by the server.
        /// </summary>
        public int EndCombatCycle;

        /// <summary>
        /// The jagex scene x coordinate of this entity.
        /// </summary>
        public int JSceneX;

        /// <summary>
        /// The jagex scene y coordinate of this entity.
        /// </summary>
        public int JSceneY;

        /// <summary>
        /// The jagex scene z coordinate of this entity.
        /// </summary>
        public int JSceneZ = (int)(12 / GameConstants.RenderScale);

        /// <summary>
        /// The unity object backing this entity for rendering.
        /// </summary>
        public GameObject UnityObject = new GameObject();

        /// <summary>
        /// Updates this entity's coordinate within the scene.
        /// 
        /// This method does not force an update on the unity object.
        /// </summary>
        /// <param name="x">The new x coordinate.</param>
        /// <param name="y">The new y coordinate.</param>
        /// <param name="z">The new z coordinate.</param>
        private void SetScenePosStale(int x, int y, int z)
        {
            JSceneX = x;
            JSceneY = y;
            JSceneZ = z;
        }

        /// <summary>
        /// Updates this entity's coordinate within the scene, and forces
        /// an update on the backing unity object.
        /// </summary>
        /// <param name="x">The new x coordinate.</param>
        /// <param name="y">The new y coordinate.</param>
        /// <param name="z">The new z coordinate.</param>
        public void SetScenePosFresh(int x, int y, int z)
        {
            SetScenePosStale(x, y, z);
            UpdateObjectScenePos();
        }

        /// <summary>
        /// Updates this entity's x coordinate within the scene.
        /// </summary>
        /// <param name="x">The new x coordinate.</param>
        public void SetSceneX(int x)
        {
            JSceneX = x;
            UpdateObjectScenePos();
        }

        /// <summary>
        /// Updates this entity's y coordinate within the scene.
        /// </summary>
        /// <param name="x">The new y coordinate.</param>
        public void SetSceneY(int y)
        {
            JSceneY = y;
            UpdateObjectScenePos();
        }

        /// <summary>
        /// Updates this entity's z coordinate within the scene.
        /// </summary>
        /// <param name="x">The new z coordinate.</param>
        public void SetSceneZ(int z)
        {
            JSceneZ = z;
            UpdateObjectScenePos();
        }

        /// <summary>
        /// Destroys any persistent state relating to this entity.
        /// </summary>
        public void Destroy()
        {
            if (UnityObject != null)
            {
                Object.Destroy(UnityObject);
                UnityObject = null;
            }
        }

        /// <summary>
        /// Updates this entity's backing unity object to reflect where it is.
        /// </summary>
        public void UpdateObjectScenePos()
        {
            if (UnityObject != null)
            {
                UnityObject.transform.position = new Vector3(JSceneX * GameConstants.RenderScale, JSceneZ * GameConstants.RenderScale, JSceneY * GameConstants.RenderScale);
            }
        }

        /// <summary>
        /// Teleports this entity to the provided coordinates.
        /// </summary>
        /// <param name="x">The x coordinate to teleport to.</param>
        /// <param name="y">The y coordinate to teleport to.</param>
        /// <param name="discardQueue">If the movement queue will be reset to defaults.</param>
        public void TeleportTo(int x, int y, bool discardQueue)
        {
            if (SeqIndex != -1)
            {
                var seq = GameContext.Cache.GetSeq(SeqIndex);
                if (seq != null && seq.WalkFlag == 1)
                {
                    SeqIndex = -1;
                }
            }

            if (!discardQueue)
            {
                int deltaX = x - PathX[0];
                int deltaY = y - PathY[0];

                if (deltaX >= -8 && deltaX <= 8 && deltaY >= -8 && deltaY <= 8)
                {
                    if (PathPosition < 9)
                    {
                        PathPosition++;
                    }

                    for (int i = PathPosition; i > 0; i--)
                    {
                        PathX[i] = PathX[i - 1];
                        PathY[i] = PathY[i - 1];
                        PathRun[i] = PathRun[i - 1];
                    }

                    PathX[0] = x;
                    PathY[0] = y;
                    PathRun[0] = false;
                    return;
                }
            }

            PathPosition = 0;
            StillPathPosition = 0;
            ResyncWalkCycle = 0;
            PathX[0] = x;
            PathY[0] = y;

            SetSceneX(PathX[0] * 128 + TileSize * 64);
            SetSceneY(PathY[0] * 128 + TileSize * 64);
        }

        /// <summary>
        /// Queues a movement for this entity.
        /// </summary>
        /// <param name="direction">The direciton to move in.</param>
        /// <param name="running">If the entity should run in the destination direction.</param>
        public void QueueMove(int direction, bool running)
        {
            var x = PathX[0];
            var y = PathY[0];

            x += GameConstants.DirectionDeltaX[direction];
            y += GameConstants.DirectionDeltaY[direction];

            if (SeqIndex != -1)
            {
                var seq = GameContext.Cache.GetSeq(SeqIndex);
                if (seq != null && seq.WalkFlag == 1)
                {
                    SeqIndex = -1;
                }
            }

            if (PathPosition < 9)
            {
                PathPosition++;
            }

            for (int l = PathPosition; l > 0; l--)
            {
                PathX[l] = PathX[l - 1];
                PathY[l] = PathY[l - 1];
                PathRun[l] = PathRun[l - 1];
            }

            PathX[0] = x;
            PathY[0] = y;
            PathRun[0] = running;
        }

        /// <summary>
        /// Resets all queued movements on this entity.
        /// </summary>
        public void ResetQueuedMovements()
        {
            PathPosition = 0;
            StillPathPosition = 0;
        }

        /// <summary>
        /// Queues a displayed hit on this entity.
        /// </summary>
        /// <param name="type">The type of hit to show.</param>
        /// <param name="damage">The amount of damage to show.</param>
        /// <param name="tick">The current tick.</param>
        public void QueueHit(int type, int damage, int tick)
        {
            for (var i = 0; i < 4; i++)
            {
                if (HitBeginCycles[i] <= tick)
                {
                    HitDamages[i] = damage;
                    HitType[i] = type;
                    HitBeginCycles[i] = tick + 70;
                    return;
                }
            }
        }

        /// <summary>
        /// If this entity is visible.
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
