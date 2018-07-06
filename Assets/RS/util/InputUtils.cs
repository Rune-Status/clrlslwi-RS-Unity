
using UnityEngine;

namespace RS
{
    /// <summary>
    /// Provides utilities related to input.
    /// </summary>
    public static class InputUtils
    {
        /// <summary>
        /// Converts a keycode to text.
        /// </summary>
        /// <param name="kc">The keycode to convert.</param>
        /// <returns>The keycode converted to text.</returns>
        public static string ToText(KeyCode kc)
        {
            if (kc == KeyCode.Space)
            {
                return " ";
            }

            if (kc == KeyCode.Semicolon)
            {
                return ";";
            }

            if (kc == KeyCode.Comma)
            {
                return ",";
            }

            if (kc == KeyCode.Period)
            {
                return ".";
            }

            if (kc == KeyCode.Equals)
            {
                return "=";
            }

            var text = kc.ToString().ToLower();
            if (text.StartsWith("alpha"))
            {
                text = text.Substring("alpha".Length);
            }
            else if (text.Length > 1)
            {
                text = "";
            }

            return text;
        }

        /// <summary>
        /// The current position of the mouse.
        /// </summary>
        public static Vector2 mousePosition
        {
            get
            {
                var existing = Input.mousePosition;
                return new Vector2(existing.x, Screen.height - existing.y);
            }
        }

        /// <summary>
        /// Determines if the mouse is in the provided rectangle.
        /// </summary>
        /// <param name="rect">The rect to check.</param>
        /// <returns>If the mouse is in the provided rect.</returns>
        public static bool MouseWithin(Rect rect)
        {
            var mp = mousePosition;
            return mp.x >= rect.x && mp.y >= rect.y && mp.x <= (rect.x + rect.width) && mp.y <= (rect.y + rect.height);
        }
    }
}
