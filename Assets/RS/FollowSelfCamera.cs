using System;

using UnityEngine;

namespace RS
{
    /// <summary>
    /// A script that forces the camera to follow the local player.
    /// </summary>
	public class FollowSelfCamera : MonoBehaviour
	{
        private const float moveHeightSpeed = 20f;
        private const float moveAngleSpeed = 170f;

        private float height = 0;
        private float camDistance = 15;

		public void Update()
		{
            var self = GameContext.Self;
			if (self == null) return;

            var obj = self.UnityObject;
			if (obj == null) return;

            var scroll = Input.GetAxis("Mouse ScrollWheel");
            camDistance += -scroll * 2;
            camDistance = Math.Min(camDistance, 25);
            camDistance = Math.Max(camDistance, 5);
            
			if (Input.GetKey(KeyCode.UpArrow))
				height = Math.Min(camDistance - 1, height + (moveHeightSpeed * Time.deltaTime));
			if (Input.GetKey(KeyCode.DownArrow))
				height = Math.Max(0, height - (moveHeightSpeed * Time.deltaTime));

            var targ = obj.transform.position;
            var x = targ.x + camDistance - (height / 2);
            var y = targ.y + height;
            var z = targ.z;
			transform.position = new Vector3(x, y, z);

			if (Input.GetKey(KeyCode.LeftArrow))
				GameContext.CamAngle += (moveAngleSpeed * Time.deltaTime);
			if (Input.GetKey(KeyCode.RightArrow))
                GameContext.CamAngle -= (moveAngleSpeed * Time.deltaTime);

			transform.RotateAround(targ, Vector3.up, GameContext.CamAngle);
			transform.LookAt(targ);
        }
	}
}

