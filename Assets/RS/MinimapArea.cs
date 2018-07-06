
using UnityEngine;

namespace RS
{
    /// <summary>
    /// Represents the minimap area on the gameframe.
    /// </summary>
    public class MinimapArea
    {
        public float AngleOffset = 90.0f;
        public int X;
        public int Y;

        /// <summary>
        /// The width of the minimap.
        /// </summary>
        public int Width
        {
            get
            {
                return 170;
            }
        }

        /// <summary>
        /// The height of the minimap.
        /// </summary>
        public int Height
        {
            get
            {
                return 160;
            }
        }

        /// <summary>
        /// The inner x of the minimap.
        /// </summary>
        private int InnerX
        {
            get
            {
                return X + 5;
            }
        }

        /// <summary>
        /// The inner y of the minimap.
        /// </summary>
        private int InnerY
        {
            get
            {
                return Y + 5;
            }
        }

        public void Draw()
        {
            var width = 152.0d;
            var height = 152.0d;
            var centerX = width / 2.0d;
            var centerY = height / 2.0f;
            var halfWidth = centerX;
            var halfHeight = centerY;

            var nwidth = width / GameContext.MinimapImage.width;
            var nheight = height / GameContext.MinimapImage.width;
            var ncenterX = nwidth / 2.0d;
            var ncenterY = nheight / 2.0d;
            var nhalfWidth = ncenterX;
            var nhalfHeight = ncenterY;

            var old = GUI.matrix;
            GUIUtility.RotateAroundPivot(-GameContext.CamAngle + AngleOffset, new Vector2((float)(InnerX + width / 2), (float)(InnerY + height / 2)));

            var xPixelsPerSu = GameContext.MinimapImage.width / (128.0d * 104.0d);
            var yPixelsPerSu = GameContext.MinimapImage.height / (128.0d * 104.0d);
            var xPixelsPerTile = xPixelsPerSu * 128.0d;
            var yPixelsPerTile = yPixelsPerSu * 128.0d;

            var pixelStartX = (xPixelsPerSu * (GameContext.Self.JSceneX)) - halfWidth;
            var pixelEndX = (xPixelsPerSu * (GameContext.Self.JSceneX)) + halfWidth;

            var pixelStartY = (yPixelsPerSu * (GameContext.Self.JSceneY)) - halfHeight;
            var pixelEndY = (yPixelsPerSu * (GameContext.Self.JSceneY)) + halfHeight;

            var pixelWidth = pixelEndX - pixelStartX;
            var pixelHeight = pixelEndY - pixelStartY;

            var xNsPerSu = 1.0d / (128.0d * 104.0d);
            var yNsPerSu = 1.0d / (128.0d * 104.0d);
            var xNsPerTile = xNsPerSu * 128.0d;
            var yNsPerTile = yNsPerSu * 128.0d;

            var normalStartX = (xNsPerSu * (GameContext.Self.JSceneX)) - nhalfWidth;
            var normalEndX = (xNsPerSu * (GameContext.Self.JSceneX)) + nhalfWidth;

            var normalStartY = (yNsPerSu * (GameContext.Self.JSceneY)) - nhalfHeight;
            var normalEndY = (yNsPerSu * (GameContext.Self.JSceneY)) + nhalfHeight;

            var normalWidth = normalEndX - normalStartX;
            var normalHeight = normalEndY - normalStartY;

            //ResourceCache.MinimapMaskMaterial.SetVector("_Start", new Vector4((float)normalStartX, (float)normalStartY, 0, 0));
            //Graphics.DrawTexture(new Rect(InnerX, InnerY, (float)width, (float)height), GameContext.MinimapImage, new Rect((float)normalStartX, (float)normalStartY, (float)normalWidth, (float)normalHeight), 0, 0, 0, 0, ResourceCache.MinimapMaskMaterial);

            {
                var local = GameContext.Self;

                var ppx = ((local.JSceneX) * xPixelsPerSu);
                var ppy = ((local.JSceneY) * yPixelsPerSu);

                var rx = (ppx - pixelStartX);
                var ry = (ppy - pixelStartY);

                var gx = X + rx;
                var gy = Y + ry;

                var dot = ResourceCache.MapDots[(int)MapDot.White];
                Graphics.DrawTexture(new Rect((float)gx, (float)gy, dot.width, dot.height), dot);

            }
            for (var i = 0; i < GameContext.PlayerCount; i++)
            {
                var player = GameContext.Players[GameContext.PlayerIndices[i]];
                var tx = (player.JSceneX + 64) * xPixelsPerSu - pixelStartX + 300;
                var ty = (player.JSceneY - 64) * yPixelsPerSu - pixelStartY + 300;

                // var pos = SceneToMinimapPos(player.SceneX + 64, player.SceneY - 64);
                var dot = ResourceCache.MapDots[(int)MapDot.White];
                //Graphics.DrawTexture(new Rect(tx, ty, dot.width, dot.height), dot);
            }

            for (var i = 0; i < GameContext.ActorCount; i++)
            {
                var actor = GameContext.Actors[GameContext.ActorIndices[i]];
                if (actor.Config.ShowOnMiniMap)
                {
                    //var pos = SceneToMinimapPos(actor.SceneX + 64, actor.SceneY - 64);
                    //var dot = GameContext.mapDots[(int)MapDot.Yellow];
                    //GUI.DrawTexture(new Rect(pos.x, pos.y, dot.width, dot.height), dot);
                }
            }

            /*for (var x = 0; x < 104; x++)
            {
                for (var y = 0; y < 104; y++)
                {
                    var p = GameContext.GroundItems[GameContext.Plane, x, y];
                    if (p != null)
                    {
                        var pos = SceneToMinimapPos(x * 128 + 128, y * 128);
                        var dot = GameContext.mapDots[(int)MapDot.Red];
                        //GUI.DrawTexture(new Rect(pos.x, pos.y, dot.width, dot.height), dot);
                    }
                }
            }*/

            {
                //var pos = SceneToMinimapPos(GameContext.Self.SceneX, GameContext.Self.SceneY);
                //var dot = GameContext.mapDots[(int)MapDot.White];
                //GUI.DrawTexture(new Rect(pos.x + dot.width / 2, pos.y + dot.height / 2, dot.width, dot.height), dot);
            }

            if (GameContext.MapMarkerX != 0)
            {
                //var pos = SceneToMinimapPos(GameContext.MapMarkerX << 7 + 128, GameContext.MapMarkerY << 7 - 128);
                //var marker = GameContext.mapMarkers[0];
                //GUI.DrawTexture(new Rect(pos.x, pos.y - marker.height, marker.width, marker.height), marker);
            }

            GUI.matrix = old;

            //Graphics.DrawTexture(new Rect((float)X, (float)Y, ResourceCache.MinimapBorder.width, ResourceCache.MinimapBorder.height), ResourceCache.MinimapBorder);
        }
    }
}
