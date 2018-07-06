
using UnityEngine;

namespace RS
{
    /// <summary>
    /// Provides utilities for accessing low level game engine rendering.
    /// </summary>
    public class LowLevelRendering
    {
        protected static bool clippingEnabled;
        protected static Rect clippingBounds;
        protected static UnityEngine.Material lineMaterial;
        public static Texture2D Temporary;

        static LowLevelRendering()
        {
            Temporary = new Texture2D(1, 1, TextureFormat.RGBA32, false, true);
        }

        protected static bool ClipTest(float p, float q, ref float u1, ref float u2)
        {
            var r = 0.0f;
            var retval = true;
            if (p < 0.0)
            {
                r = q / p;
                if (r > u2)
                {
                    retval = false;
                }
                else if (r > u1)
                {
                    u1 = r;
                }
            }
            else if (p > 0.0)
            {
                r = q / p;
                if (r < u1)
                {
                    retval = false;
                }
                else if (r < u2)
                {
                    u2 = r;
                }
            }
            else if (q < 0.0)
            {
                retval = false;
            }

            return retval;
        }

        protected static bool SegmentRectIntersection(Rect bounds, ref Vector2 p1, ref Vector2 p2)
        {
            float u1 = 0.0f, u2 = 1.0f, dx = p2.x - p1.x, dy;
            if (ClipTest(-dx, p1.x - bounds.xMin, ref u1, ref u2))
            {
                if (ClipTest(dx, bounds.xMax - p1.x, ref u1, ref u2))
                {
                    dy = p2.y - p1.y;
                    if (ClipTest(-dy, p1.y - bounds.yMin, ref u1, ref u2))
                    {
                        if (ClipTest(dy, bounds.yMax - p1.y, ref u1, ref u2))
                        {
                            if (u2 < 1.0)
                            {
                                p2.x = p1.x + u2 * dx;
                                p2.y = p1.y + u2 * dy;
                            }
                            if (u1 > 0.0)
                            {
                                p1.x += u1 * dx;
                                p1.y += u1 * dy;
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static void BeginGroup(Rect position)
        {
            clippingEnabled = true;
            clippingBounds = new Rect(0, 0, position.width, position.height);
            GUI.BeginGroup(position);
        }

        public static void EndGroup()
        {
            GUI.EndGroup();
            clippingBounds = new Rect(0, 0, Screen.width, Screen.height);
            clippingEnabled = false;
        }

        public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color)
        {
            if (clippingEnabled)
                if (!SegmentRectIntersection(clippingBounds, ref pointA, ref pointB))
                    return;

            if (!lineMaterial)
            {
                /* Credit:  */
                lineMaterial = new UnityEngine.Material("Shader \"Lines/Colored Blended\" {" +
               "SubShader { Pass {" +
               "   BindChannels { Bind \"Color\",color }" +
               "   Blend SrcAlpha OneMinusSrcAlpha" +
               "   ZWrite Off Cull Off Fog { Mode Off }" +
               "} } }");
                lineMaterial.hideFlags = HideFlags.HideAndDontSave;
                lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
            }

            lineMaterial.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Color(color);
            GL.Vertex3(pointA.x, pointA.y, 0);
            GL.Vertex3(pointB.x, pointB.y, 0);
            GL.End();
        }

        public static void DrawQuad(Texture2D texture, Rect position, Color color)
        {
            texture.wrapMode = TextureWrapMode.Repeat;
            texture.SetPixel(0, 0, color);
            texture.Apply();
            GUI.DrawTexture(position, texture);
        }

        public static void DrawQuad(Rect position, Color color)
        {
            DrawQuad(Temporary, position, color);
        }
    }

}
