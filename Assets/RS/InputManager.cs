using System;
using System.Collections;
using System.Linq;

using UnityEngine;

namespace RS
{
    /// <summary>
    /// Handles input tracking.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        private static InputManager instance;

        /// <summary>
        /// A singleton instance.
        /// </summary>
        public static InputManager Instance
        {
            get
            {
                if (!instance)
                {
                    Debug.LogError("No InputManager in scene");
                }
                return instance;
            }
        }

        /// <summary>
        /// The pressed state of each key.
        /// </summary>
        public bool[] Pressed = new bool[1024];

        /// <summary>
        /// A queue of newly pressed keys.
        /// </summary>
        private Queue keyQueue = new Queue();
        
        public /* override */ void Awake()
        {
            instance = this;
        }

        public /* override */ void Update()
        {
            foreach (var key in Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>())
            {
                if (Input.GetKeyDown(key))
                {
                    keyQueue.Enqueue(key);
                }

                if (Input.GetKey(key))
                {
                    Pressed[(int)key] = true;
                }
                else
                {
                    Pressed[(int)key] = false;
                }
            }
        }

        /// <summary>
        /// Resets all state.
        /// </summary>
        public void Reset()
        {
            keyQueue.Clear();
            Pressed = new bool[1024];
        }

        /// <summary>
        /// Retrieves the next pressed key in the queue.
        /// </summary>
        /// <returns>The next pressed key in the queue.</returns>
        public KeyCode GetNextKey()
        {
            if (keyQueue.Count > 0)
                return (KeyCode) keyQueue.Dequeue();
            else
                return KeyCode.None;
        }

        /// <summary>
        /// Determines if any newly pressed keys are queued.
        /// </summary>
        /// <returns>If any newly pressed keys are queued.</returns>
        public bool HasKeys()
        {
            return keyQueue.Count > 0;
        }
    }

}
