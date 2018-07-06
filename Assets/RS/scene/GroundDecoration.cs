using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RS
{
    public class GroundDecoration : SceneObject
    {
        public int Arrangement;
        public Model Root;
        public int SceneX;
        public int SceneY;
        public int SceneZ;
        public long UniqueId;

        public GroundDecoration(int arrangement, Model root, int sceneX, int sceneY, int sceneZ, long uniqueId)
        {
            this.Arrangement = arrangement;
            this.Root = root;
            this.SceneX = sceneX;
            this.SceneY = sceneY;
            this.SceneZ = sceneZ;
            this.UniqueId = uniqueId;
        }

    }
}
