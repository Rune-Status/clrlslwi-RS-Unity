using System;

using UnityEngine;

namespace RS
{
    /// <summary>
    /// Handles sun movement.
    /// </summary>
    public class DayAndNight : MonoBehaviour
    {
        public Light light;
        private float angle = 0;

        public void Update()
        {
            light.transform.position = new Vector3(30, 400, 0);
            light.transform.RotateAround(new Vector3(120, 60, 120), Vector3.forward, angle);
            light.transform.LookAt(new Vector3(120, 60, 120));

            var intensity = light.transform.position.y / 130;
            intensity = Math.Max(intensity, 0.15f);
            intensity = Math.Min(intensity, 1.0f);
            light.intensity = intensity;
        }
    }
}
