using UnityEngine;

namespace RS
{
    /// <summary>
    /// Represents various different map dots.
    /// </summary>
    public enum MapDot
    {
        /// <summary>
        /// A red dot.
        /// </summary>
        Red = 0,
        /// <summary>
        /// A yellow dot.
        /// </summary>
        Yellow = 1,
        /// <summary>
        /// A white dot.
        /// </summary>
        White = 2,
        /// <summary>
        /// A green dot.
        /// </summary>
        Green = 3,
        /// <summary>
        /// A blue dot.
        /// </summary>
        Blue = 4,
    }

    public class GameProcessor : MonoBehaviour
    {
        private float tickOverflowBucket = 0.0f;

        /// <summary>
        /// Applies all updates that can be applied incrementally.
        /// 
        /// This includes things such as updating models, animations, etc.
        /// </summary>
        public void UpdateIncremental()
        {
            if (GameContext.Plane != GameContext.Scene.PlaneAtBuild)
            {
                GameContext.LoadScene();
            }

            if (GameContext.WaitingForScene && GameContext.ReceivedPlayerUpdate)
            {
                Debug.Log("Received player!");
                GameContext.WaitingForScene = false;
                GameContext.ReceivedPlayerUpdate = false;
                GameContext.LoadScene();
            }

            GameContext.LoopCycle += 1;
            GameContext.HandlePlayers();
            GameContext.HandleActors();
            GameContext.HideStackedEntities();

            var vpWidget = GameContext.ViewportWidget;
            if (vpWidget != null)
            {
                GameContext.UpdateWidget(vpWidget, 4, 4, 0);
            }

            GameContext.Chat.Update();
            GameContext.AnimCycle += 1;
        }

        public void Update()
        {
            var deltaMs = Time.deltaTime * 1000f;
            tickOverflowBucket += deltaMs;
            var count = (int)tickOverflowBucket / 20;
            tickOverflowBucket -= count * 20;
            for (var i = 0; i < count; i++)
            {
                UpdateIncremental();
            }

            GameContext.MenuUpdate();

            if (GameContext.DragArea != 0)
            {
                GameContext.DragCycle++;

                var mpos = InputUtils.mousePosition;
                if (mpos.x > GameContext.DragStartX + 5 || mpos.x < GameContext.DragStartX - 5 ||
                    mpos.y > GameContext.DragStartY + 5 || mpos.y < GameContext.DragStartY - 5)
                {
                    GameContext.Dragging = true;
                }

                if (!Input.GetMouseButton(0))
                {
                    GameContext.DragArea = 0;

                    if (GameContext.Dragging && GameContext.DragCycle >= 5)
                    {
                        if (GameContext.HoveredSlotWidget == GameContext.DragWidget && GameContext.HoveredSlot != GameContext.DragSlot)
                        {
                            var w = GameContext.Cache.GetWidgetConfig(GameContext.DragWidget);
                            int type = 0;

                            /*if (anInt913 == 1 && w.actionType == 206)
                            {
                                type = 1;
                            }*/

                            if (w.ItemIndices[GameContext.HoveredSlot] <= 0)
                            {
                                type = 0;
                            }

                            if (w.ItemsSwappable)
                            {
                                int oldSlot = GameContext.DragSlot;
                                int newSlot = GameContext.HoveredSlot;
                                w.ItemIndices[newSlot] = w.ItemIndices[oldSlot];
                                w.ItemAmounts[newSlot] = w.ItemAmounts[oldSlot];
                                w.ItemIndices[oldSlot] = -1;
                                w.ItemAmounts[oldSlot] = 0;
                            }
                            else if (type == 1)
                            {
                                int oldSlot = GameContext.DragSlot;
                                for (int newSlot = GameContext.HoveredSlot; oldSlot != newSlot;)
                                {
                                    if (oldSlot > newSlot)
                                    {
                                        w.SwapSlots(oldSlot, oldSlot - 1);
                                        oldSlot--;
                                    }
                                    else if (oldSlot < newSlot)
                                    {
                                        w.SwapSlots(oldSlot, oldSlot + 1);
                                        oldSlot++;
                                    }
                                }
                            }
                            else
                            {
                                w.SwapSlots(GameContext.DragSlot, GameContext.HoveredSlot);
                            }

                            var @out = new Packet(214);
                            @out.WriteLEShortA(GameContext.DragWidget);
                            @out.WriteByteC(type);
                            @out.WriteLEShortA(GameContext.DragSlot);
                            @out.WriteLEShort(GameContext.HoveredSlot);
                            GameContext.NetworkHandler.Write(@out);
                        }

                        GameContext.HoveredSlotWidget = -1;
                    }
                    /*else if (mouse_button_setting == 1 && option_count > 2)
                    {
                        Menu.show();
                    }*/
                    else
                    {
                        GameContext.Menu.ClickedLast();
                        //handle_menu_option(option_count - 1);
                    }
                }
            }
        }

        /// <summary>
        /// Render head icons on the provided player.
        /// </summary>
        /// <param name="e">The entity to render the head icon on.</param>
        private void DrawHeadIcons(Player e)
        {
            var renderer = e.UnityObject.GetComponent<MeshRenderer>();
            var bounds = renderer.bounds;

            var pos = e.UnityObject.transform.position;
            var up = new Vector3(pos.x, pos.y + bounds.size.y + 1.5f, pos.z);

            if (e.PrayerIcon > 0 && e.PrayerIcon < ResourceCache.HeadIconsPrayer.Length)
            {
                var heading = up - Camera.main.transform.position;
                if (Vector3.Dot(Camera.main.transform.forward, heading) > 0)
                {
                    var screenPos = Camera.main.WorldToScreenPoint(up);
                    var @fixed = new Vector3(screenPos.x, Screen.height - screenPos.y);
                    var drawX = @fixed.x;
                    var drawY = @fixed.y;

                    var tex = ResourceCache.HeadIconsPrayer[e.PrayerIcon];
                    GUI.DrawTexture(new Rect(drawX - tex.width / 2, drawY, tex.width, tex.height), tex);
                }
            }
        }

        /// <summary>
        /// Draws all hits being applied to the provided entity.
        /// </summary>
        /// <param name="e">The entity to render the hits of.</param>
        private void DrawHits(Entity e)
        {
            var renderer = e.UnityObject.GetComponent<MeshRenderer>();
            var bounds = renderer.bounds;

            var pos = e.UnityObject.transform.position;
            var up = new Vector3(pos.x, pos.y + bounds.size.y / 2, pos.z);

            var heading = up - Camera.main.transform.position;
            if (Vector3.Dot(Camera.main.transform.forward, heading) > 0)
            {
                var screenPos = Camera.main.WorldToScreenPoint(up);
                var @fixed = new Vector3(screenPos.x, Screen.height - screenPos.y);

                for (var mark = 0; mark < 4; mark++)
                {
                    if (e.HitBeginCycles[mark] > GameContext.LoopCycle)
                    {
                        var drawX = @fixed.x;
                        var drawY = @fixed.y;

                        if (drawX > -1)
                        {
                            if (mark == 1)
                            {
                                drawY -= 20;
                            }
                            else if (mark == 2)
                            {
                                drawX -= 15;
                                drawY -= 10;
                            }
                            else if (mark == 3)
                            {
                                drawX += 15;
                                drawY -= 10;
                            }

                            var tex = ResourceCache.Hitmarks[e.HitType[mark]];
                            var hitmarkBounds = new Rect(drawX - 12, drawY - 12, tex.width, tex.height);
                            GUI.DrawTexture(hitmarkBounds, tex);

                            var text = GameContext.Cache.SmallFont.DrawString("" + e.HitDamages[mark], 0xFFFFFFFF, true, false);
                            var textX = hitmarkBounds.x + (hitmarkBounds.width / 2) - (text.width / 2);
                            GUI.DrawTexture(new Rect(textX, drawY - 5, text.width, text.height), text);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draws the health bar of the provided entity.
        /// </summary>
        /// <param name="e">The entity to render the health bar of.</param>
        private void DrawHealthBar(Entity e)
        {
            var renderer = e.UnityObject.GetComponent<MeshRenderer>();
            var bounds = renderer.bounds;

            var pos = e.UnityObject.transform.position;
            var up = new Vector3(pos.x, pos.y + bounds.size.y, pos.z);

            var heading = up - Camera.main.transform.position;
            if (Vector3.Dot(Camera.main.transform.forward, heading) > 0)
            {
                var screenPos = Camera.main.WorldToScreenPoint(up);
                var @fixed = new Vector3(screenPos.x, Screen.height - screenPos.y);
                var drawX = @fixed.x;
                var drawY = @fixed.y;

                LowLevelRendering.DrawQuad(new Rect(drawX - 15, drawY - 3, 30, 5), new Color(1.0f, 0.0f, 0.0f));
                LowLevelRendering.DrawQuad(new Rect(drawX - 15, drawY - 3, (int)(30 * (e.CurrentHealth / (double)e.MaxHealth)), 5), new Color(0.0f, 1.0f, 0.0f));
            }
        }

        /// <summary>
        /// Draws the message being spoken by the provided entity.
        /// </summary>
        /// <param name="e">The entity to render the spoken message of.</param>
        private void DrawSpokenMessage(Entity e)
        {
            var renderer = e.UnityObject.GetComponent<MeshRenderer>();
            var bounds = renderer.bounds;

            var pos = e.UnityObject.transform.position;
            var up = new Vector3(pos.x, pos.y + bounds.size.y + 0.6f, pos.z);

            var heading = up - Camera.main.transform.position;
            if (Vector3.Dot(Camera.main.transform.forward, heading) > 0)
            {
                var screenPos = Camera.main.WorldToScreenPoint(up);
                var @fixed = new Vector3(screenPos.x, Screen.height - screenPos.y);
                if (e.SpokenTex == null)
                {
                    e.SpokenTex = GameContext.Cache.BoldFont.DrawString(e.SpokenMessage, 0xFFFFFF00, true, true);
                }
                GUI.DrawTexture(new Rect(@fixed.x - e.SpokenTex.width / 2, @fixed.y, e.SpokenTex.width, e.SpokenTex.height), e.SpokenTex);
            }
        }

        /// <summary>
        /// Draws overlay elements on top of entities, etc.
        /// </summary>
        private void DrawOverlayBits()
        {
            for (var i = -1; i < GameContext.PlayerCount; i++)
            {
                var index = 2047;
                if (i != -1)
                    index = GameContext.PlayerIndices[i];

                var e = GameContext.Players[index];
                if (e.EndCombatCycle > GameContext.LoopCycle)
                {
                    DrawHits(e);
                }
            }

            for (var i = 0; i < GameContext.ActorCount; i++)
            {
                var index = GameContext.ActorIndices[i];
                var e = GameContext.Actors[index];
                if (e.EndCombatCycle > GameContext.LoopCycle)
                {
                    DrawHits(e);
                }
            }

            for (var i = -1; i < GameContext.PlayerCount; i++)
            {
                var index = 2047;
                if (i != -1)
                    index = GameContext.PlayerIndices[i];

                var e = GameContext.Players[index];
                if (e.EndCombatCycle > GameContext.LoopCycle)
                {
                    DrawHealthBar(e);
                }
            }

            for (var i = 0; i < GameContext.ActorCount; i++)
            {
                var index = GameContext.ActorIndices[i];
                var e = GameContext.Actors[index];
                if (e.EndCombatCycle > GameContext.LoopCycle)
                {
                    DrawHealthBar(e);
                }
            }

            for (var i = -1; i < GameContext.PlayerCount; i++)
            {
                var index = 2047;
                if (i != -1)
                    index = GameContext.PlayerIndices[i];

                var e = GameContext.Players[index];
                if (e.SpokenMessage != null)
                {
                    DrawSpokenMessage(e);
                }
            }

            for (var i = 0; i < GameContext.ActorCount; i++)
            {
                var index = GameContext.ActorIndices[i];
                var e = GameContext.Actors[index];
                if (e.SpokenMessage != null)
                {
                    DrawSpokenMessage(e);
                }
            }

            for (var i = -1; i < GameContext.PlayerCount; i++)
            {
                var index = 2047;
                if (i != -1)
                    index = GameContext.PlayerIndices[i];

                var e = GameContext.Players[index];
                DrawHeadIcons(e);
            }
        }

        /// <summary>
        /// Converts a 3D RS position to a minimap position.
        /// </summary>
        /// <param name="x">The x coordinate to convert.</param>
        /// <param name="y">The y coordinate to convert.</param>
        /// <returns>The converted coordinate.</returns>
        private Vector2 SceneToMinimapPos(int x, int y)
        {
            var centerX = GameContext.Self.JSceneX;
            var centerY = GameContext.Self.JSceneY;

            var offsetX = x - centerX;
            var offsetY = y - centerY;
            var offsetPx = offsetX >> 5;
            var offsetPy = offsetY >> 5;
            var rkoX = 146 / 2 + offsetPx;
            var rkoY = 151 / 2 - offsetPy;
            return new Vector2(rkoX, rkoY);
        }

        /// Overridden from unity
        public void OnGUI()
        {
            GameContext.UpdateRenderStates();

            GameContext.RenderCycle += 1;
            DrawOverlayBits();

            if (GameContext.MinimapImage != null)
            {
                GameContext.MinimapArea.Draw();
            }

            GameContext.Chat.Render();
            GameContext.TabArea.Render();
            GameContext.ProjectilesUpdate();
           

            if (GameContext.Self.JSceneX >> 7 == GameContext.MapMarkerX &&
                GameContext.Self.JSceneY >> 7 == GameContext.MapMarkerY)
            {
                GameContext.MapMarkerX = 0;
            }

            GameContext.Cross.Render();

            if (GameContext.ViewportWidget != null)
            {
                GameContext.ViewportWidget.Draw(GameContext.ViewportWidgetX, GameContext.ViewportWidgetY, 0);
            }

            if (!GameContext.Menu.Visible)
            {
                GameContext.UpdateTooltip();

                var tex = GameContext.TooltipTexture;
                if (tex != null)
                {
                    GUI.DrawTexture(new Rect(4, 4, tex.width, tex.height), tex);
                }
            }

            GameContext.Menu.Render();
            GameContext.AnimCycle = 0;
        }
    }
}

