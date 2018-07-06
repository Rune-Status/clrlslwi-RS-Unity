using UnityEngine;

namespace RS
{
    /// <summary>
    /// Limits the FPS of the running application.
    /// </summary>
    public class FPSLimiter : MonoBehaviour
    {
        public void Awake()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 50;
        }
    }
}

