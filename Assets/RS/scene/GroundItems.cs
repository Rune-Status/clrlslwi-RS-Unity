using UnityEngine;

namespace RS
{
    /// <summary>
    /// Represents a pile of items on the ground.
    /// </summary>
    public class GroundItems
    {
        public Model TopModel;
        public Model MiddleModel;
        public Model BottomModel;
        public GroundItem TopItem;
        public GroundItem MiddleItem;
        public GroundItem BottomItem;
        public int UniqueId;
        public int SceneX;
        public int SceneY;
        public int SceneZ;
        public int OffZ;

        public GameObject TopObject;
        public GameObject MiddleObject;
        public GameObject BottomObject;

        public GroundItems(Model topModel, Model middleModel, Model bottomModel, GroundItem topItem, GroundItem midItem, GroundItem botItem, int uniqueId, int sceneX, int sceneY, int sceneZ, int offZ)
        {
            TopModel = topModel;
            MiddleModel = middleModel;
            BottomModel = bottomModel;
            TopItem = topItem;
            MiddleItem = midItem;
            BottomItem = botItem;
            UniqueId = uniqueId;
            SceneX = sceneX;
            SceneY = sceneY;
            SceneZ = sceneZ;
            OffZ = offZ;
        }

        public void Link(Scene scene)
        {
            if (TopObject != null)
            {
                scene.AddAsSceneObject(TopObject);
            }
            if (MiddleObject != null)
            {
                scene.AddAsSceneObject(MiddleObject);
            }
            if (BottomObject != null)
            {
                scene.AddAsSceneObject(BottomObject);
            }
        }

        public void Unlink(Scene scene)
        {
            if (TopObject != null)
            {
                scene.RemoveAsSceneObject(TopObject);
            }
            if (MiddleObject != null)
            {
                scene.RemoveAsSceneObject(MiddleObject);
            }
            if (BottomObject != null)
            {
                scene.RemoveAsSceneObject(BottomObject);
            }
        }

        private void AddCollider(GameObject unityObject)
        {
            var collider = unityObject.GetComponent<MeshCollider>();
            if (collider == null)
            {
                collider = unityObject.AddComponent<MeshCollider>();
            }
            collider.sharedMesh = unityObject.GetComponent<MeshFilter>().mesh;
            collider.enabled = true;
        }

        public void Create()
        {
            if (TopModel != null)
            {
                TopObject = new GameObject();
                TopModel.Backing = TopObject;
                TopModel.AddMeshToObject();
                TopObject.transform.position = new Vector3(GameConstants.RScale(SceneX), GameConstants.RScale(SceneY), GameConstants.RScale(SceneZ));

                var comp = TopObject.AddComponent<GroundItemComponent>();
                comp.Items = this;
                comp.Item = TopItem;
                AddCollider(TopObject);
            }

            if (MiddleModel != null)
            {
                MiddleObject = new GameObject();
                MiddleModel.Backing = MiddleObject;
                MiddleModel.AddMeshToObject();
                MiddleObject.transform.position = new Vector3(GameConstants.RScale(SceneX), GameConstants.RScale(SceneY), GameConstants.RScale(SceneZ));

                var comp = MiddleObject.AddComponent<GroundItemComponent>();
                comp.Items = this;
                comp.Item = MiddleItem;
                AddCollider(MiddleObject);
            }

            if (BottomModel != null)
            {
                BottomObject = new GameObject();
                BottomModel.Backing = BottomObject;
                BottomModel.AddMeshToObject();
                BottomObject.transform.position = new Vector3(GameConstants.RScale(SceneX), GameConstants.RScale(SceneY), GameConstants.RScale(SceneZ));

                var comp = BottomObject.AddComponent<GroundItemComponent>();
                comp.Items = this;
                comp.Item = BottomItem;
                AddCollider(BottomObject);
            }
        }

        public void Destroy()
        {
            if (TopObject != null)
            {
                GameObject.Destroy(TopObject);
                TopObject = null;
            }

            if (MiddleObject != null)
            {
                GameObject.Destroy(MiddleObject);
                MiddleObject = null;
            }

            if (BottomObject != null)
            {
                GameObject.Destroy(BottomObject);
                BottomObject = null;
            }
        }
    }
}
