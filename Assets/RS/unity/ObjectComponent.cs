
using UnityEngine;

namespace RS
{
    /// <summary>
    /// A script that contains an interactive object.
    /// </summary>
    public class InteractiveComponent : MonoBehaviour
    {
        public InteractiveObject GameObject;
    }

    /// <summary>
    /// A script that contains a wall object.
    /// </summary>
    public class WallComponent : MonoBehaviour
    {
        public WallObject GameObject;
    }
    
    /// <summary>
    /// A script that contains a ground decoration object.
    /// </summary>
    public class GroundDecorationComponent : MonoBehaviour
    {
        public GroundDecoration GameObject;
    }

    /// <summary>
    /// A script that contains a wall decoration object.
    /// </summary>
    public class WallDecorationComponent : MonoBehaviour
    {
        public WallDecoration GameObject;
    }
}
