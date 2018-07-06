using System;
using System.Collections.Generic;
using UnityEngine;

namespace RS
{
    /// <summary>
    /// Holds state related to the game.
    /// </summary>
    public class GameContext
    {
        private static Cache cache = new Cache();
        private static MaterialPool texPool;
        private static NetworkHandler networkHandler;

        public static float LoopCycle = 0;
        public static float AnimCycle = 0;
        public static float RenderCycle = 0;
        public static int LoadedRegionX;
        public static int LoadedRegionY;
        public static int RegionX;
        public static int RegionY;
        public static int MapBaseX;
        public static int MapBaseY;
        public static bool RestrictRegion;
        public static int LastMapBaseX;
        public static int LastMapBaseY;
        public static EncryptedInt Plane = 0;
        public static Player Self;
        public static EncryptedInt LocalRights = 0;
        public static int TargetRegionX = 0;
        public static int TargetRegionY = 0;
        public static float frameDelta = 0.0f;

        public static int LocalPlayerIndex = -1;
        public static int[] PlayerIndices = new int[2048];
        public static Player[] Players = new Player[2048];
        public static JagexBuffer[] PlayerBuffers = new JagexBuffer[2048];
        public static int PlayerCount = 0;
        public static PlayerOption[] PlayerOptions = new PlayerOption[10];

        public static int[] ActorIndices = new int[16384];
        public static Actor[] Actors = new Actor[16384];
        public static int ActorCount = 0;

        public static SocialStorageCap FriendsStorageType = SocialStorageCap.FreeToPlay;
        public static SocialStatus SocialStatus = SocialStatus.Loading;

        public static int[,,] HeightMap = new int[4, 104, 104];
        public static int[,,] RenderFlags = new int[4, 104, 104];
        public static Scene Scene = new Scene(HeightMap, RenderFlags, Cache);

        public static byte[][] ChunkLandscapePayload;
        public static byte[][] ChunkObjectPayload;
        public static int[] ChunkCoords;
        public static int[] MapUids;
        public static int[] LandscapeUids;

        public static int[] SkillExperiences = new int[GameConstants.SkillCount];
        public static int[] SkillCurrentLevels = new int[GameConstants.SkillCount];
        public static int[] SkillMaxLevels = new int[GameConstants.SkillCount];

        public static Widget ViewportWidget = null;

        public static Chat Chat = new Chat();
        public static TabArea TabArea = new TabArea();
        public static MinimapArea MinimapArea = new MinimapArea();

        public static int[] Settings = new int[2000];

        public static CollisionMap[] CollisionMaps;

        public static float CamAngle = 0;
        public static ActionMenu Menu = new ActionMenu();

        public static Cross Cross = new Cross();

        public static Texture2D MinimapImage;

        public static List<GroundItem>[,,] GroundItems = new List<GroundItem>[4, 104, 104];
        public static List<Projectile> Projectiles = new List<Projectile>();

        public static int MarkType;
        public static int MarkX;
        public static int MarkY;
        public static int MarkZ;

        public static int MapMarkerX;
        public static int MapMarkerY;

        public static int HoveredSlot;
        public static int HoveredSlotWidget;
        public static bool Dragging = false;
        public static int DragArea = 0;
        public static int DragCycle = 0;
        public static int DragSlot = 0;
        public static int DragStartX = 0;
        public static int DragStartY = 0;
        public static int DragWidget = 0;

        public static bool SelectedItem = false;
        public static int SelectedItemSlot;
        public static int SelectedItemWidget;
        public static int SelectedItemConfigIndex;
        public static string SelectedItemName;

        public static bool SelectedWidget = false;
        public static string SelectedTooltip;
        public static int SelectedMask;
        public static int SelectedWidgetIndex;

        public static string LastRenderedTooltipString = "";
        public static Texture2D TooltipTexture = null;

        public static bool WaitingForScene = false;
        public static bool ReceivedPlayerUpdate = false;
        public static int CursorId = -1;

        /// <summary>
        /// The cipher to use for encrypting packets.
        /// </summary>
        public static ISAACCipher InCipher;
        public static ISAACCipher OutCipher;

        static GameContext()
        {
            ResetState();

            Menu.PreFireListeners.Add(() =>
            {
                ResetSelectedItem();
                ResetSelectedWidget();
            });
        }

        /// <summary>
        /// The x coordinate of the viewport widget.
        /// </summary>
        public static int ViewportWidgetX
        {
            get
            {
                return (Camera.main.pixelWidth - MinimapArea.Width) / 2 - 150;
            }
        }

        /// <summary>
        /// The y coordinate of the viewport widget.
        /// </summary>
        public static int ViewportWidgetY
        {
            get
            {
                return (Camera.main.pixelHeight - Chat.Height) / 2 - 150;
            }
        }
        /// <summary>
        /// Updates the selected widget item.
        /// </summary>
        /// <param name="widget">The id of the widget containing the item.</param>
        /// <param name="slot">The slot of the selected item.</param>
        /// <param name="configId">The cache config id of the selected item.</param>

        public static void SetSelectedItem(int widget, int slot, int configId)
        {
            SelectedItem = true;
            SelectedItemSlot = slot;
            SelectedItemWidget = widget;
            SelectedItemConfigIndex = configId;
            SelectedItemName = Cache.GetItemConfig(configId).name;
            SelectedWidget = false;

            InvalidateItemTexture(widget, slot);
        }

        /// <summary>
        /// Resets the selected widget item.
        /// 
        /// This method frees any textures associated with the item.
        /// </summary>
        public static void ResetSelectedItem()
        {
            if (SelectedItem)
            {
                SelectedItem = false;
                InvalidateItemTexture(SelectedItemWidget, SelectedItemSlot);
            }
        }

        /// <summary>
        /// Updates the selected widget.
        /// </summary>
        /// <param name="widget">The id of the selected widget.</param>
        /// <param name="mask">The selection mask to use for deciding behavior.</param>
        /// <param name="tooltip">The tooltip to display while the widget is selected.</param>
        public static void SetSelectedWidget(int widget, int mask, string tooltip)
        {
            SelectedWidget = true;
            SelectedWidgetIndex = widget;
            SelectedMask = mask;
            SelectedTooltip = tooltip;
            SelectedItem = false;
        }

        /// <summary>
        /// Resets the selected widget.
        /// </summary>
        public static void ResetSelectedWidget()
        {
            if (SelectedWidget)
            {
                SelectedWidget = false;
            }
        }

        /// <summary>
        /// Performs a frame update on the scrollbar of a widget.
        /// </summary>
        /// <param name="w">The widget to update the scrollbar of.</param>
        /// <param name="x">The x coordinate of the scrollbar.</param>
        /// <param name="y">The y coordinate of the scrollbar.</param>
        /// <param name="mouseX">The x mouse coordinate.</param>
        /// <param name="mouseY">They mouse coordinate.</param>
        /// <param name="height">The height of the scroll area.</param>
        public static void UpdateWidgetScrollbar(Widget w, int x, int y, int mouseX, int mouseY, int height)
        {
            if (Input.GetMouseButton(0))
            {
                if (mouseX >= x && mouseX < x + 16 && mouseY >= y && mouseY < y + 16)
                {
                    w.ScrollAmount -= 4;
                }
                else if (mouseX >= x && mouseX < x + 16 && mouseY >= (y + height) - 16 && mouseY < y + height)
                {
                    w.ScrollAmount += 4;
                }
                else if (mouseX >= x - 32 && mouseX < x + 16 + 32 && mouseY >= y + 16 && mouseY < (y + height) - 16)
                {
                    var gripLength = ((height - 32) * height) / w.Config.ScrollHeight;
                    if (gripLength < 8)
                    {
                        gripLength = 8;
                    }

                    var scale = mouseY - y - 16 - gripLength / 2;
                    var divisor = height - 32 - gripLength;
                    w.ScrollAmount = ((w.Config.ScrollHeight - height) * scale) / divisor;
                }
            }
        }

        /// <summary>
        /// Performs a frame update on a widget.
        /// </summary>
        /// <param name="w">The widget being updated.</param>
        /// <param name="screenX">The x coordinate of the widget on the screen.</param>
        /// <param name="screenY">The y coordinate of the widget on the screen.</param>
        /// <param name="scrollAmount">The amount of scroll being applied to the widget.</param>
        public static void UpdateWidget(Widget w, int screenX, int screenY, int scrollAmount)
        {
            if (w.Config.Type != 0 || w.Config.ChildIds == null || w.Config.Hidden)
            {
                return;
            }

            if (!w.Config.Visible)
            {
                return;
            }

            var mp = InputUtils.mousePosition;
            var mouseX = (int)mp.x;
            var mouseY = (int)mp.y;

            for (int index = 0; index < w.Config.ChildIds.Length; index++)
            {
                var x = w.Config.ChildX[index] + screenX;
                var y = (w.Config.ChildY[index] + screenY) - w.Config.ScrollAmount;

                var child = w.GetChild(index);
                if (!child.Config.Visible)
                {
                    continue;
                }

                x += child.Config.X;
                y += child.Config.Y;

                var hover = false;
                if (mouseX >= x && mouseY >= y && mouseX < x + child.Config.Width && mouseY < y + child.Config.Height)
                {
                    hover = true;
                }

                if (child.Config.Type == 0)
                {
                    if (child.Config.ScrollHeight > child.Config.Height)
                    {
                        UpdateWidgetScrollbar(child, x + child.Config.Width, y, mouseX, mouseY, child.Config.Height);
                    }
                }
            }
        }

        /// <summary>
        /// Parses any queued landscape files, and loads them into the loaded scene.
        /// </summary>
        public static void LoadScene()
        {
            Scene.PlaneAtBuild = Plane;
            Scene.DestroySceneObjects();
            Scene.InitTiles();

            for (var i = 0; i < 4; i++)
            {
                CollisionMaps[i].SetDefaults();
            }

            int length = ChunkLandscapePayload.Length;
            for (int chunk = 0; chunk < length; chunk++)
            {
                int chunkX = (ChunkCoords[chunk] >> 8) * 64 - MapBaseX;
                int chunkY = (ChunkCoords[chunk] & 0xff) * 64 - MapBaseY;
                byte[] payload = ChunkLandscapePayload[chunk];
                if (payload != null && payload.Length > 0)
                {
                    Scene.LoadLandscapeSmallChunk(new DefaultJagexBuffer(payload), chunkX, chunkY, (LoadedRegionX - 6) * 8, (LoadedRegionY - 6) * 8);
                }
            }

            for (int chunk = 0; chunk < length; chunk++)
            {
                byte[] payload = ChunkObjectPayload[chunk];
                if (payload != null && payload.Length > 0)
                {
                    int regionX = (ChunkCoords[chunk] >> 8) * 64 - MapBaseX;
                    int regionY = (ChunkCoords[chunk] & 0xff) * 64 - MapBaseY;
                    Scene.LoadObjectsSmallChunk(CollisionMaps, new DefaultJagexBuffer(payload), regionX, regionY);
                }
            }

            Scene.CalcColors();
            Scene.Apply(CollisionMaps);
            Scene.CopyUnderlayAndOverlay();
            Scene.CreateObjects();

            MinimapImage = GenerateMinimap(Scene.PlaneAtBuild);
        }

        /// <summary>
        /// Calculates the menu tooltip string to draw at the corner of the client.
        /// </summary>
        /// <returns>The menu tooltip string to draw at the corner of the client.</returns>
        public static string GetTooltipString()
        {
            var count = Menu.ActionCount;

            if (count < 2 && !SelectedItem && !SelectedWidget)
            {
                return "";
            }

            string s;

            if (SelectedItem && count < 2)
            {
                s = "Use " + SelectedItemName + " with...";
            }
            else if (SelectedWidget && count < 2)
            {
                s = SelectedTooltip + "...";
            }
            else
            {
                s = Menu.GetLast().GetCaption();
            }

            if (count > 2)
            {
                s = s + "@whi@ / " + (count - 2) + " more options";
            }

            return s;
        }

        /// <summary>
        /// Updates the tooltip texture.
        /// </summary>
        public static void UpdateTooltip()
        {
            var s = GetTooltipString();
            if (!s.Equals(LastRenderedTooltipString))
            {
                if (s.Length == 0)
                {
                    TooltipTexture = null;
                }
                else
                {
                    TooltipTexture = Cache.BoldFont.DrawString(s, 0xFFFFFFFF, true, true);
                }
                LastRenderedTooltipString = s;
            }
        }

        /// <summary>
        /// Updates the models of the item pile at the provided coordinates.
        /// </summary>
        /// <param name="x">The x coordinate of the tile to update.</param>
        /// <param name="y">The y coordinate of the tile to update.</param>
        public static void UpdateItemPile(int x, int y)
        {
            var pile = GroundItems[Plane, x, y];
            if (pile == null)
            {
                Scene.RemoveItemPile(x, y, Plane);
                return;
            }
            
            var highestPriority = -99999999;
            GroundItem top = null;
            
            foreach (var i in pile)
            {
                var oc = Cache.GetItemConfig(i.Index);
                var a = oc.pilePriority;

                if (oc.stackable)
                {
                    a *= i.Amount + 1;
                }

                if (a > highestPriority)
                {
                    highestPriority = a;
                    top = i;
                }
            }

            GroundItem middle = null;
            GroundItem bottom = null;

            foreach (var item in pile)
            {
                if (item.Index != top.Index && bottom == null)
                {
                    bottom = item;
                }
                if (item.Index != top.Index && item.Index != bottom.Index && middle == null)
                {
                    middle = item;
                }
            }

            Model topModel = null;
            if (top != null)
            {
                topModel = Cache.GetItemConfig(top.Index).GetModel(top.Amount, false);
            }

            Model middleModel = null;
            if (middle != null)
            {
                middleModel = Cache.GetItemConfig(middle.Index).GetModel(middle.Amount, false);
            }


            Model bottomModel = null;
            if (bottom != null)
            {
                bottomModel = Cache.GetItemConfig(bottom.Index).GetModel(bottom.Amount, false);
            }

            var uid = x + (y << 7) + 0x60000000;
            Scene.AddItemPile(x, y, GetLandZ(x * 128 + 64, y * 128 + 64, Plane), Plane, topModel, middleModel, bottomModel, top, middle, bottom, uid);
        }
        
        /// <summary>
        /// Draws a minimap tile onto a raw array of pixels.
        /// </summary>
        /// <param name="pixels">The pixels to render onto.</param>
        /// <param name="x">The x coordinate of the tile to render.</param>
        /// <param name="y">The y coordinate of the tile to render.</param>
        /// <param name="plane">The plane of the tile to render.</param>
        /// <param name="wallRgb">The RGB color of walls.</param>
        /// <param name="doorRgb">The RGB color of doors.</param>
        public static void DrawMinimapTile(int[] pixels, int x, int y, int plane, int wallRgb, int doorRgb)
        {
            long uid = Scene.GetWallUniqueId(plane, x, y);

            if (uid != 0)
            {
                int arrangement = Scene.GetArrangement(plane, x, y, uid);
                int rotation = arrangement >> 6 & 3;
                int type = arrangement & 0x1f;
                int rgb = wallRgb;
                if (uid > 0)
                {
                    rgb = doorRgb;
                }

                int i = 24624 + x * 4 + (103 - y) * 512 * 4;
                var lc = Cache.GetObjectConfig((int)(uid >> 14 & 0xffff));
                if (lc.sceneImageIndex != -1)
                {
                    var b = ResourceCache.MapScene[lc.sceneImageIndex];
                    if (b != null)
                    {
                        int mapX = (lc.sizeX * 4 - b.width) / 2;
                        int mapY = (lc.sizeY * 4 - b.height) / 2;

                        TextureUtils.Draw(pixels, 512, 512, b, 48 + x * 4 + mapX, 48 + (104 - y - lc.sizeY) * 4 + mapY);
                    }
                }
                else
                {
                    if (type == 0 || type == 2)
                    {
                        if (rotation == 0)
                        {
                            pixels[i] = rgb;
                            pixels[i + 512] = rgb;
                            pixels[i + 1024] = rgb;
                            pixels[i + 1536] = rgb;
                        }
                        else if (rotation == 1)
                        {
                            pixels[i] = rgb;
                            pixels[i + 1] = rgb;
                            pixels[i + 2] = rgb;
                            pixels[i + 3] = rgb;
                        }
                        else if (rotation == 2)
                        {
                            pixels[i + 3] = rgb;
                            pixels[i + 3 + 512] = rgb;
                            pixels[i + 3 + 1024] = rgb;
                            pixels[i + 3 + 1536] = rgb;
                        }
                        else if (rotation == 3)
                        {
                            pixels[i + 1536] = rgb;
                            pixels[i + 1536 + 1] = rgb;
                            pixels[i + 1536 + 2] = rgb;
                            pixels[i + 1536 + 3] = rgb;
                        }
                    }

                    if (type == 2)
                    {
                        if (rotation == 3)
                        {
                            pixels[i] = rgb;
                            pixels[i + 512] = rgb;
                            pixels[i + 1024] = rgb;
                            pixels[i + 1536] = rgb;
                        }
                        else if (rotation == 0)
                        {
                            pixels[i] = rgb;
                            pixels[i + 1] = rgb;
                            pixels[i + 2] = rgb;
                            pixels[i + 3] = rgb;
                        }
                        else if (rotation == 1)
                        {
                            pixels[i + 3] = rgb;
                            pixels[i + 3 + 512] = rgb;
                            pixels[i + 3 + 1024] = rgb;
                            pixels[i + 3 + 1536] = rgb;
                        }
                        else if (rotation == 2)
                        {
                            pixels[i + 1536] = rgb;
                            pixels[i + 1536 + 1] = rgb;
                            pixels[i + 1536 + 2] = rgb;
                            pixels[i + 1536 + 3] = rgb;
                        }
                    }

                    if (type == 3)
                    {
                        if (rotation == 0)
                        {
                            pixels[i] = rgb;
                        }
                        else if (rotation == 1)
                        {
                            pixels[i + 3] = rgb;
                        }
                        else if (rotation == 2)
                        {
                            pixels[i + 3 + 1536] = rgb;
                        }
                        else if (rotation == 3)
                        {
                            pixels[i + 1536] = rgb;
                        }
                    }
                }
            }

            uid = Scene.GetInteractiveUniqueId(plane, x, y);
            if (uid != 0)
            {
                var arrangement = Scene.GetArrangement(plane, x, y, uid);
                var rotation = arrangement >> 6 & 3;
                var type = arrangement & 0x1f;
                var lc = Cache.GetObjectConfig((int)(uid >> 14 & 0xffff));
                if (lc.sceneImageIndex != -1)
                {
                    var b = ResourceCache.MapScene[lc.sceneImageIndex];
                    if (b != null)
                    {
                        int mapX = (lc.sizeX * 4 - b.width) / 2;
                        int mapY = (lc.sizeY * 4 - b.height) / 2;

                        TextureUtils.Draw(pixels, 512, 512, b, 48 + x * 4 + mapX, 48 + (104 - y - lc.sizeY) * 4 + mapY);
                    }
                }
                else if (type == 9)
                {
                    var color = 0xEEEEEE;
                    if (uid > 0)
                    {
                        color = 0xEE0000;
                    }

                    var i = 24624 + x * 4 + (103 - y) * 512 * 4;
                    if (rotation == 0 || rotation == 2)
                    {
                        pixels[i + 1536] = color;
                        pixels[i + 1024 + 1] = color;
                        pixels[i + 512 + 2] = color;
                        pixels[i + 3] = color;
                    }
                    else
                    {
                        pixels[i] = color;
                        pixels[i + 512 + 1] = color;
                        pixels[i + 1024 + 2] = color;
                        pixels[i + 1536 + 3] = color;
                    }
                }
            }

            uid = Scene.GetGroundDecorationUniqueId(plane, x, y);
            if (uid != 0)
            {
                var lc = Cache.GetObjectConfig((int)(uid >> 14 & 0xffff));
                if (lc.sceneImageIndex != -1)
                {
                    var b = ResourceCache.MapScene[lc.sceneImageIndex];
                    if (b != null)
                    {
                        int mapX = (lc.sizeX * 4 - b.width) / 2;
                        int mapY = (lc.sizeY * 4 - b.height) / 2;

                        TextureUtils.Draw(pixels, 512, 512, b, 48 + x * 4 + mapX, 48 + (104 - y - lc.sizeY) * 4 + mapY);
                    }
                }
            }
        }

        /// <summary>
        /// Generates the minimap texture.
        /// </summary>
        /// <param name="plane">The plane to generates the minimap on.</param>
        /// <returns>The generated minimap.</returns>
        public static Texture2D GenerateMinimap(int plane)
        {
            var pixels = new int[512 * 512];
            for (var y = 1; y < 103; y++)
            {
                var i = 24628 + (103 - y) * 512 * 4;
                for (var x = 1; x < 103; x++)
                {
                    if ((RenderFlags[plane, x, y] & 0x18) == 0)
                    {
                        Scene.DrawMinimapTile(pixels, i, 512, plane, x, y);
                    }

                    if (plane < 3 && (RenderFlags[plane + 1, x, y] & 8) != 0)
                    {
                        Scene.DrawMinimapTile(pixels, i, 512, plane + 1, x, y);
                    }

                    i += 4;
                }
            }

            var wallColor = ((238 + (int)(20D)) - 10 << 16) + ((238 + (int)(20D)) - 10 << 8) + ((238 + (int)(20D)) - 10);
            var doorColor = (238 + (int)(20D)) - 10 << 16;
            for (int y = 1; y < 103; y++)
            {
                for (int x = 1; x < 103; x++)
                {
                    if ((RenderFlags[plane, x, y] & 0x18) == 0)
                    {
                        DrawMinimapTile(pixels, x, y, plane, wallColor, doorColor);
                    }
                    if (plane < 3 && (RenderFlags[plane + 1, x, y] & 8) != 0)
                    {
                        DrawMinimapTile(pixels, x, y, plane + 1, wallColor, doorColor);
                    }
                }

            }

            var tex = new Texture2D(512, 512, TextureFormat.RGBA32, false, true);
            var colPixels = tex.GetPixels();
            for (var i = 0; i < pixels.Length; i++)
            {
                var pixel = pixels[i];
                var r = (byte)((pixel >> 16) & 0xFF);
                var g = (byte)((pixel >> 8) & 0xFF);
                var b = (byte)(pixel & 0xFF);
                colPixels[i] = new Color32(r, g, b, 255);
            }
            tex.SetPixels(colPixels);
            tex.Apply();
            tex = TextureUtils.FlipVertical(tex);
            return tex;
        }

        /// <summary>
        /// Creates a scrollbar texture matching the provided parameters.
        /// </summary>
        /// <param name="height">The height of the scroll area.</param>
        /// <param name="scrollHeight">The maximum scroll value.</param>
        /// <param name="scrollAmount">The current scroll amount.</param>
        /// <returns>The texture that displays the current scrollbar with the provided parameters.</returns>
        public static Texture2D CreateScrollbar(int height, int scrollHeight, int scrollAmount)
        {
            var gripLength = ((height - 32) * height) / scrollHeight;
            if (gripLength < 8)
            {
                gripLength = 8;
            }

            var offsetY = ((height - 32 - gripLength) * scrollAmount) / (scrollHeight - height);
            var scrollbar = new Texture2D(16, height - 16, TextureFormat.ARGB32, false, true);
            TextureRasterizer.FillRect(scrollbar, 0, 16, 16, height - 32, 0xFF23201B);
            TextureRasterizer.FillRect(scrollbar, 0, 16 + offsetY, 16, gripLength, 0xFF4D4233);
            TextureRasterizer.DrawLineV(scrollbar, 0, 16 + offsetY, gripLength, 0xFF766654);
            TextureRasterizer.DrawLineV(scrollbar, 1, 16 + offsetY, gripLength, 0xFF766654);
            TextureRasterizer.DrawLineH(scrollbar, 0, 16 + offsetY, 16, 0xFF766654);
            TextureRasterizer.DrawLineH(scrollbar, 0, 17 + offsetY, 16, 0xFF766654);
            TextureRasterizer.DrawLineV(scrollbar, 15, 16 + offsetY, gripLength, 0xFF332D25);
            TextureRasterizer.DrawLineV(scrollbar, 14, 17 + offsetY, gripLength - 1, 0xFF332D25);
            TextureRasterizer.DrawLineH(scrollbar, 0, 15 + offsetY + gripLength, 16, 0xFF332D25);
            TextureRasterizer.DrawLineH(scrollbar, 1, 14 + offsetY + gripLength, 15, 0xFF332D25);
            scrollbar.Apply();
            return scrollbar;
        }

        /// <summary>
        /// Closes all opened widgets.
        /// 
        /// This function queues a packet for sending to the server, notifying of a widgets close.
        /// </summary>
        public static void CloseWidgets()
        {
            NetworkHandler.Write(new Packet(130));
            TabArea.TabWidget = null;
            ViewportWidget = null;
            Chat.OverlayWidget = null;
        }

        /// <summary>
        /// Performs an interaction with the object at the provided coordinates.
        /// 
        /// If no object is found, false is returned.
        /// 
        /// If an object is found, an object interaction cross is shown.
        /// </summary>
        /// <param name="x">The x tile coordinate of the object.</param>
        /// <param name="y">The y tile coordinate of the object.</param>
        /// <param name="uid">The uid of the object.</param>
        /// <returns>If the object was interacted with successfully.</returns>
        public static bool InteractWithObject(int x, int y, long uid)
        {
            var index = (int)(uid >> 14 & 0x7fff);
            var locInfo = Scene.GetArrangement(Plane, x, y, uid);
            if (locInfo == -1)
            {
                return false;
            }

            var type = locInfo & 0x1f;
            var rotation = locInfo >> 6 & 3;
            if (type == 10 || type == 11 || type == 22)
            {
                var lc = Cache.GetObjectConfig(index);

                var sizeX = 0;
                var sizeY = 0;
                if (rotation == 0 || rotation == 2)
                {
                    sizeX = lc.sizeX;
                    sizeY = lc.sizeY;
                }
                else
                {
                    sizeX = lc.sizeY;
                    sizeY = lc.sizeX;
                }

                var faceFlags = lc.faceFlags;
                if (rotation != 0)
                {
                    faceFlags = (faceFlags << rotation & 0xf) + (faceFlags >> 4 - rotation);
                }

                WalkTo(2, sizeX, sizeY, Self.PathX[0], Self.PathY[0], x, y, 0, faceFlags, 0, false);
            }
            else
            {
                WalkTo(2, 0, 0, Self.PathX[0], Self.PathY[0], x, y, type + 1, 0, rotation, false);
            }

            var pos = InputUtils.mousePosition;
            Cross.Show(2, (int)pos.x, (int)pos.y);
            return true;
        }

        /// <summary>
        /// Resets all state back to default.
        /// </summary>
        public static void ResetState()
        {
            if (Self != null)
            {
                Self.Destroy();
                Self = null;
                Players[2047] = null;
            }

            if (networkHandler != null)
            {
                networkHandler.ResetState();
            }

            LoopCycle = 0;

            for (var i = 0; i < PlayerCount; i++)
            {
                var player = Players[PlayerIndices[i]];
                player.Destroy();
            }

            for (var i = 0; i < ActorCount; i++)
            {
                var actor = Actors[ActorIndices[i]];
                actor.Destroy();
            }

            PlayerIndices = new int[2048];
            Players = new Player[2048];
            PlayerCount = 0;
            PlayerOptions = new PlayerOption[10];

            ActorIndices = new int[16384];
            Actors = new Actor[16384];
            ActorCount = 0;

            SkillExperiences = new int[GameConstants.SkillCount];
            SkillCurrentLevels = new int[GameConstants.SkillCount];
            SkillMaxLevels = new int[GameConstants.SkillCount];

            for (int i = 0; i < TabArea.Tabs.Length; i++)
            {
                if (TabArea.Tabs[i] != null)
                {
                    TabArea.Tabs[i].WidgetId = -1;
                }
            }

            TabArea.SelectedTabIndex = 3;

            CollisionMaps = new CollisionMap[4];
            for (int plane = 0; plane < 4; plane++)
            {
                CollisionMaps[plane] = new CollisionMap(104, 104);
            }
        }

        /// <summary>
        /// Initializes game context state.
        /// </summary>
        public static void Init()
        {
            Model.Init(30000, Cache);
            Chat.Init();
            TabArea.Init();
        }

        /// <summary>
        /// Shows the cross at current mouse position.
        /// </summary>
        /// <param name="type">The type of cross to show.</param>
        public static void ShowCross(int type)
        {
            var mpos = InputUtils.mousePosition;
            Cross.Show(type, (int)mpos.x, (int)mpos.y);
        }

        /// <summary>
        /// Determines the height coordinate of a 3d RS coordinate.
        /// </summary>
        /// <param name="x">The 3D RS x coordinate</param>
        /// <param name="y">The 3D RS y coordinate</param>
        /// <param name="plane">The plane of the tile.</param>
        /// <returns>The height of the tile at the provided coordinate.</returns>
        public static int GetLandZ(int x, int y, int plane)
        {
            var localX = x >> 7;
            var localY = y >> 7;
            if (localX < 0 || localY < 0 || localX > 103 || localY > 103)
            {
                return 0;
            }

            int z = plane;

            if (z < 3 && (RenderFlags[1, localX, localY] & 2) == 2)
            {
                z++;
            }

            var tileX = x & 0x7f;
            var tileY = y & 0x7f;
            var ca = HeightMap[z, localX, localY] * (128 - tileX) + HeightMap[z, localX + 1, localY] * tileX >> 7;
            var cb = HeightMap[z, localX, localY + 1] * (128 - tileX) + HeightMap[z, localX + 1, localY + 1] * tileX >> 7;
            return ((ca * (128 - tileY) + (cb * tileY)) >> 7);
        }

        /// <summary>
        /// Instructs the client and server to move somewhere.
        /// </summary>
        /// <param name="moveType">The type of movement to perform.</param>
        /// <param name="sizeX">The x size of the destination.</param>
        /// <param name="sizeY">The y size of the destination.</param>
        /// <param name="startX">The start x coordinate of the movement.</param>
        /// <param name="startY">The start y coordinate of the movement.</param>
        /// <param name="destX">The end x coordinate of the movement.</param>
        /// <param name="destY">The end y coordinate of the movement.</param>
        /// <param name="faceType">The type of object to end the movement at, if we encounter it.</param>
        /// <param name="faceFlags">The flags to apply to objects for determing what to ignore.</param>
        /// <param name="rotation">The rotation to check for wall collisions at.</param>
        /// <param name="arbitrary">If arbitrary distance will be used.</param>
        /// <returns>If the path movement was successful.</returns>
        public static bool WalkTo(int moveType, int sizeX, int sizeY, int startX, int startY, int destX, int destY, int faceType, int faceFlags, int rotation, bool arbitrary)
        {
            int[,] pathWaypoint = new int[104, 104];
            int[,] pathDistance = new int[104, 104];
            int[] pathQueueX = new int[4000];
            int[] pathQueueY = new int[4000];

            byte mapSizeX = 104;
            byte mapSizeY = 104;

            for (int x1 = 0; x1 < mapSizeX; x1++)
            {
                for (int y1 = 0; y1 < mapSizeY; y1++)
                {
                    pathWaypoint[x1, y1] = 0;
                    pathDistance[x1, y1] = 0x5f5e0ff;
                }

            }

            int x = startX;
            int y = startY;
            pathWaypoint[startX, startY] = 99;
            pathDistance[startX, startY] = 0;

            int next = 0;
            int current = 0;
            pathQueueX[next] = startX;
            pathQueueY[next++] = startY;

            bool reached = false;
            int pathLength = pathQueueX.Length;
            int[,] ai = CollisionMaps[Plane].Flags;

            while (current != next)
            {
                x = pathQueueX[current];
                y = pathQueueY[current];
                current = (current + 1) % pathLength;

                if (x == destX && y == destY)
                {
                    reached = true;
                    break;
                }

                if (faceType != 0)
                {
                    if ((faceType < 5 || faceType == 10) && CollisionMaps[Plane].AtWall(x, y, destX, destY, faceType - 1, rotation))
                    {
                        reached = true;
                        break;
                    }
                    if (faceType < 10 && CollisionMaps[Plane].AtDecoration(x, y, destX, destY, faceType - 1, rotation))
                    {
                        reached = true;
                        break;
                    }
                }

                if (sizeX != 0 && sizeY != 0 && CollisionMaps[Plane].AtObject(x, y, destX, destY, sizeX, sizeY, faceFlags))
                {
                    reached = true;
                    break;
                }

                int distance = pathDistance[x, y] + 1;

                if (x > 0 && pathWaypoint[x - 1, y] == 0 && (ai[x - 1, y] & 0x1280108) == 0)
                {
                    pathQueueX[next] = x - 1;
                    pathQueueY[next] = y;
                    next = (next + 1) % pathLength;
                    pathWaypoint[x - 1, y] = 2;
                    pathDistance[x - 1, y] = distance;
                }

                if (x < mapSizeX - 1 && pathWaypoint[x + 1, y] == 0 && (ai[x + 1, y] & 0x1280180) == 0)
                {
                    pathQueueX[next] = x + 1;
                    pathQueueY[next] = y;
                    next = (next + 1) % pathLength;
                    pathWaypoint[x + 1, y] = 8;
                    pathDistance[x + 1, y] = distance;
                }

                if (y > 0 && pathWaypoint[x, y - 1] == 0 && (ai[x, y - 1] & 0x1280102) == 0)
                {
                    pathQueueX[next] = x;
                    pathQueueY[next] = y - 1;
                    next = (next + 1) % pathLength;
                    pathWaypoint[x, y - 1] = 1;
                    pathDistance[x, y - 1] = distance;
                }

                if (y < mapSizeY - 1 && pathWaypoint[x, y + 1] == 0 && (ai[x, y + 1] & 0x1280120) == 0)
                {
                    pathQueueX[next] = x;
                    pathQueueY[next] = y + 1;
                    next = (next + 1) % pathLength;
                    pathWaypoint[x, y + 1] = 4;
                    pathDistance[x, y + 1] = distance;
                }

                if (x > 0 && y > 0 && pathWaypoint[x - 1, y - 1] == 0 && (ai[x - 1, y - 1] & 0x128010e) == 0 && (ai[x - 1, y] & 0x1280108) == 0 && (ai[x, y - 1] & 0x1280102) == 0)
                {
                    pathQueueX[next] = x - 1;
                    pathQueueY[next] = y - 1;
                    next = (next + 1) % pathLength;
                    pathWaypoint[x - 1, y - 1] = 3;
                    pathDistance[x - 1, y - 1] = distance;
                }

                if (x < mapSizeX - 1 && y > 0 && pathWaypoint[x + 1, y - 1] == 0 && (ai[x + 1, y - 1] & 0x1280183) == 0 && (ai[x + 1, y] & 0x1280180) == 0 && (ai[x, y - 1] & 0x1280102) == 0)
                {
                    pathQueueX[next] = x + 1;
                    pathQueueY[next] = y - 1;
                    next = (next + 1) % pathLength;
                    pathWaypoint[x + 1, y - 1] = 9;
                    pathDistance[x + 1, y - 1] = distance;
                }

                if (x > 0 && y < mapSizeY - 1 && pathWaypoint[x - 1, y + 1] == 0 && (ai[x - 1, y + 1] & 0x1280138) == 0 && (ai[x - 1, y] & 0x1280108) == 0 && (ai[x, y + 1] & 0x1280120) == 0)
                {
                    pathQueueX[next] = x - 1;
                    pathQueueY[next] = y + 1;
                    next = (next + 1) % pathLength;
                    pathWaypoint[x - 1, y + 1] = 6;
                    pathDistance[x - 1, y + 1] = distance;
                }

                if (x < mapSizeX - 1 && y < mapSizeY - 1 && pathWaypoint[x + 1, y + 1] == 0 && (ai[x + 1, y + 1] & 0x12801e0) == 0 && (ai[x + 1, y] & 0x1280180) == 0 && (ai[x, y + 1] & 0x1280120) == 0)
                {
                    pathQueueX[next] = x + 1;
                    pathQueueY[next] = y + 1;
                    next = (next + 1) % pathLength;
                    pathWaypoint[x + 1, y + 1] = 12;
                    pathDistance[x + 1, y + 1] = distance;
                }
            }

            if (!reached)
            {
                if (arbitrary)
                {
                    int maxDistance = 100;
                    for (int dev = 1; dev < 2; dev++)
                    {
                        for (int moveX = destX - dev; moveX <= destX + dev; moveX++)
                        {
                            for (int moveY = destY - dev; moveY <= destY + dev; moveY++)
                            {
                                if (moveX >= 0 && moveY >= 0 && moveX < 104 && moveY < 104 && pathDistance[moveX, moveY] < maxDistance)
                                {
                                    maxDistance = pathDistance[moveX, moveY];
                                    x = moveX;
                                    y = moveY;
                                    reached = true;
                                }
                            }

                        }

                        if (reached)
                        {
                            break;
                        }
                    }

                }
                if (!reached)
                {
                    return false;
                }
            }

            current = 0;
            pathQueueX[current] = x;
            pathQueueY[current++] = y;
            int skipCheck;

            for (int waypoint = skipCheck = pathWaypoint[x, y]; x != startX || y != startY; waypoint = pathWaypoint[x, y])
            {
                if (waypoint != skipCheck)
                {
                    skipCheck = waypoint;
                    pathQueueX[current] = x;
                    pathQueueY[current++] = y;
                }
                if ((waypoint & 2) != 0)
                {
                    x++;
                }
                else if ((waypoint & 8) != 0)
                {
                    x--;
                }
                if ((waypoint & 1) != 0)
                {
                    y++;
                }
                else if ((waypoint & 4) != 0)
                {
                    y--;
                }
            }
            if (current > 0)
            {
                int index = current;
                if (index > 25)
                {
                    index = 25;
                }

                current--;

                MapMarkerX = pathQueueX[0];
                MapMarkerY = pathQueueY[0];

                int pathX = pathQueueX[current];
                int pathY = pathQueueY[current];

                var opcode = 164;
                if (moveType == 0)
                {
                    opcode = 164;
                }
                else if (moveType == 1)
                {
                    opcode = 248;
                }
                else if (moveType == 2)
                {
                    opcode = 98;
                }

                var @out = new Packet(opcode);
                @out.WriteByte(2 + ((index - 1) * 2) + 2 + 1);
                @out.WriteLEShortA(pathX + MapBaseX);

                for (int i = 1; i < index; i++)
                {
                    current--;
                    @out.WriteByte(pathQueueX[current] - pathX);
                    @out.WriteByte(pathQueueY[current] - pathY);
                }

                @out.WriteLEShort(pathY + MapBaseY);
                @out.WriteByteC(0);
                
                NetworkHandler.Write(@out);
                return true;
            }
            return moveType != 1;
        }

        /// <summary>
        /// Updates an entity's animation state.
        /// </summary>
        /// <param name="e">The entity to update.</param>
        public static void UpdateEntityAnimation(Entity e)
        {
            e.CanRotate = false;

            if (e.MoveSeqIndex != -1)
            {
                var s = Cache.GetSeq(e.MoveSeqIndex);
                if (s != null)
                {
                    e.MoveSeqCycle++;

                    if (e.MoveSeqFrame < s.FrameCount && e.MoveSeqCycle > s.GetFrameLength(e.MoveSeqFrame))
                    {
                        e.MoveSeqCycle = 1;
                        e.MoveSeqFrame++;
                        e.SeqNextIdleFrame++;
                    }

                    e.SeqNextIdleFrame = e.MoveSeqFrame + 1;
                    if (e.SeqNextIdleFrame >= s.FrameCount)
                    {
                        if (e.SeqNextIdleFrame >= s.FrameCount)
                        {
                            e.SeqNextIdleFrame = 0;
                        }
                    }

                    if (e.MoveSeqFrame < s.FrameCount && e.MoveSeqCycle > s.GetFrameLength(e.MoveSeqFrame))
                    {
                        e.MoveSeqCycle = 1;
                        e.MoveSeqFrame = 0;
                    }

                    if (e.MoveSeqFrame >= s.FrameCount)
                    {
                        e.MoveSeqCycle = 0;
                        e.MoveSeqFrame = 0;
                    }
                }
            }

            if (e.SpotAnimIndex != -1 && LoopCycle >= e.SpotAnimCycleEnd)
            {
                /*if (e.SpotAnimFrame < 0)
                {
                    e.SpotAnimFrame = 0;
                }

                Sequence s = null;
                if (e.SpotAnimIndex >= 0)
                {;
                    s = Cache.GetGraphicDescriptor(e.SpotAnimIndex).seq;
                }

                if (s != null)
                {
                    for (e.SpotAnimCycle++; e.SpotAnimFrame < s.FrameCount && e.SpotAnimCycle > s.GetFrameLength(e.SpotAnimFrame); e.SpotAnimFrame++)
                    {
                        e.SpotAnimCycle -= s.GetFrameLength(e.SpotAnimFrame);
                    }

                    if (e.SpotAnimFrame >= s.FrameCount && (e.SpotAnimFrame < 0 || e.SpotAnimFrame >= s.FrameCount))
                    {
                        e.SpotAnimIndex = -1;
                    }
                }*/
            }

            if (e.SeqIndex != -1 && e.SeqDelayCycle <= 1)
            {
                if (e.SeqIndex < 0)
                {
                    e.SeqIndex = -1;
                }
                else
                {
                    var s = Cache.GetSeq(e.SeqIndex);
                    if (s == null)
                    {
                        e.SeqIndex = -1;
                    }
                    else if (s.SpeedFlag == 1 && e.StillPathPosition > 0 && e.MoveCycleEnd <= LoopCycle && e.MoveCycleStart < LoopCycle)
                    {
                        e.SeqDelayCycle = 1;
                        return;
                    }
                }
            }

            if (e.SeqIndex != -1 && e.SeqDelayCycle == 0)
            {
                if (e.SeqIndex < 0)
                {
                    e.SeqIndex = -1;
                }
                else
                {
                    var s = Cache.GetSeq(e.SeqIndex);
                    if (s == null)
                    {
                        e.SeqIndex = -1;
                    }
                    else
                    {
                        for (e.SeqCycle++; e.SeqFrame < s.FrameCount && e.SeqCycle > s.GetFrameLength(e.SeqFrame); e.SeqFrame++)
                        {
                            e.SeqCycle -= s.GetFrameLength(e.SeqFrame);
                        }

                        if (e.SeqFrame >= s.FrameCount)
                        {
                            e.SeqFrame -= s.Padding;
                            e.SeqResetCycle++;

                            if (e.SeqResetCycle >= s.ResetCycle)
                            {
                                e.SeqIndex = -1;
                            }

                            if (e.SeqFrame < 0 || e.SeqFrame >= s.FrameCount)
                            {
                                e.SeqIndex = -1;
                            }
                        }
                        e.CanRotate = s.CanRotate;
                    }
                }
            }

            if (e.SeqDelayCycle > 0)
            {
                e.SeqDelayCycle--;
            }
        }

        /// <summary>
        /// Performs an update on an entity.
        /// </summary>
        /// <param name="e">The entity to update.</param>
        public static void UpdateEntity(Entity e)
        {
            e.SpokenLife -= 1;
            if (e.SpokenLife == 0)
            {
                e.SpokenMessage = null;
                e.SpokenTex = null;
            }

            if (e.JSceneX < 128 || e.JSceneY < 128 || e.JSceneX >= 13184 || e.JSceneY >= 13184)
            {
                e.SeqIndex = -1;
                e.SpotAnimIndex = -1;
                e.MoveCycleEnd = 0;
                e.MoveCycleStart = 0;
                e.SetSceneX(e.PathX[0] * 128 + e.TileSize * 64);
                e.SetSceneY(e.PathY[0] * 128 + e.TileSize * 64);
                e.ResetQueuedMovements();
            }

            if (e == Self && (e.JSceneX < 1536 || e.JSceneY < 1536 || e.JSceneX >= 11776 || e.JSceneY >= 11776))
            {
                e.SeqIndex = -1;
                e.SpotAnimIndex = -1;
                e.MoveCycleEnd = 0;
                e.MoveCycleStart = 0;
                e.SetSceneX(e.PathX[0] * 128 + e.TileSize * 64);
                e.SetSceneY(e.PathY[0] * 128 + e.TileSize * 64);
                e.ResetQueuedMovements();
            }

            if (e.MoveCycleEnd > LoopCycle)
            {
                UpdateEntityMovementVarsLate(e);
            }
            else if (e.MoveCycleStart >= LoopCycle)
            {
                UpdateEntityMovementVarsCurrent(e);
            }
            else
            {
                UpdateEntityMovement(e);
            }

            UpdateEntityRotation(e);
            UpdateEntityAnimation(e);
        }

        /// <summary>
        /// Updates an entity's movement variables for an entity that is way too late on movement.
        /// </summary>
        /// <param name="e">The entity to move.</param>
        public static void UpdateEntityMovementVarsLate(Entity e)
        {
            var deltaCycle = e.MoveCycleEnd - LoopCycle;
            var destX = e.MoveStartX * 128 + e.TileSize * 64;
            var destY = e.MoveStartY * 128 + e.TileSize * 64;

            e.JSceneX += (destX - e.JSceneX) / (int)deltaCycle;
            e.JSceneY += (destY - e.JSceneY) / (int)deltaCycle;
            e.ResyncWalkCycle = 0;

            if (e.MoveDirection == 0)
            {
                e.DestRotation = 1024;
            }
            else if (e.MoveDirection == 1)
            {
                e.DestRotation = 1536;
            }
            else if (e.MoveDirection == 2)
            {
                e.DestRotation = 0;
            }
            else if (e.MoveDirection == 3)
            {
                e.DestRotation = 512;
            }
        }

        /// <summary>
        /// Updates an entity's movement.
        /// </summary>
        /// <param name="e">The entity to update.</param>
        public static void UpdateEntityMovement(Entity e)
        {
            e.MoveSeqIndex = e.StandAnimation;
            if (e.PathPosition == 0)
            {
                e.ResyncWalkCycle = 0;
                return;
            }

            if (e.SeqIndex != -1 && e.SeqDelayCycle == 0)
            {
                var a = Cache.GetSeq(e.SeqIndex);
                if (e.StillPathPosition > 0 && a.SpeedFlag == 0)
                {
                    e.ResyncWalkCycle++;
                    return;
                }
                if (e.StillPathPosition <= 0 && a.WalkFlag == 0)
                {
                    e.ResyncWalkCycle++;
                    return;
                }
            }

            var sceneX = e.JSceneX;
            var sceneY = e.JSceneY;
            var destX = (e.PathX[e.PathPosition - 1] * 128 + e.TileSize * 64);
            var destY = (e.PathY[e.PathPosition - 1] * 128 + e.TileSize * 64);
            if (destX - sceneX > 256 || destX - sceneX < -256 || destY - sceneY > 256 || destY - sceneY < -256)
            {
                // we're too far away, just teleport :)
                e.JSceneX = destX;
                e.JSceneY = destY;
                return;
            }

            if (sceneX < destX)
            {
                if (sceneY < destY)
                {
                    e.DestRotation = 1280;
                }
                else if (sceneY > destY)
                {
                    e.DestRotation = 1792;
                }
                else
                {
                    e.DestRotation = 1536;
                }
            }
            else if (sceneX > destX)
            {
                if (sceneY < destY)
                {
                    e.DestRotation = 768;
                }
                else if (sceneY > destY)
                {
                    e.DestRotation = 256;
                }
                else
                {
                    e.DestRotation = 512;
                }
            }
            else if (sceneY < destY)
            {
                e.DestRotation = 1024;
            }
            else
            {
                e.DestRotation = 0;
            }

            int angleDiff = e.DestRotation - e.Rotation & 0x7ff;
            if (angleDiff > 1024)
            {
                angleDiff -= 2048;
            }

            int index = e.Turn180Animation;
            if (angleDiff >= -256 && angleDiff <= 256)
            {
                index = e.WalkAnimation;
            }
            else if (angleDiff >= 256 && angleDiff < 768)
            {
                index = e.TurnLeftAnimation;
            }
            else if (angleDiff >= -768 && angleDiff <= -256)
            {
                index = e.TurnRightAnimation;
            }

            if (index == -1)
            {
                index = e.WalkAnimation;
            }

            e.MoveSeqIndex = index;

            var speed = 4;
            if (e.Rotation != e.DestRotation && e.FaceEntity == -1 && e.RotateSpeed != 0)
            {
                speed = 2;
            }

            if (e.PathPosition > 2)
            {
                speed = 6;
            }

            if (e.PathPosition > 3)
            {
                speed = 8;
            }

            if (e.ResyncWalkCycle > 0 && e.PathPosition > 1)
            {
                speed = 8;
                e.ResyncWalkCycle--;
            }

            if (e.PathRun[e.PathPosition - 1])
            {
                speed <<= 1;
            }

            if (speed >= 8 && e.MoveSeqIndex == e.WalkAnimation && e.RunAnimation != -1)
            {
                e.MoveSeqIndex = e.RunAnimation;
            }

            if (sceneX < destX)
            {
                e.SetSceneX(e.JSceneX + speed);
                if (e.JSceneX > destX)
                {
                    e.SetSceneX(destX);
                }
            }
            else if (sceneX > destX)
            {
                e.SetSceneX(e.JSceneX - speed);
                if (e.JSceneX < destX)
                {
                    e.SetSceneX(destX);
                }
            }

            if (sceneY < destY)
            {
                e.SetSceneY(e.JSceneY + speed);
                if (e.JSceneY > destY)
                {
                    e.SetSceneY(destY);
                }
            }
            else if (sceneY > destY)
            {
                e.SetSceneY(e.JSceneY - speed);
                if (e.JSceneY < destY)
                {
                    e.SetSceneY(destY);
                }
            }

            if (e.JSceneX == destX && e.JSceneY == destY)
            {
                e.PathPosition--;
                if (e.StillPathPosition > 0)
                {
                    e.StillPathPosition--;
                }
            }
        }

        /// <summary>
        /// Updates an entity's movement variables for an entity that is up to date on their movement.
        /// </summary>
        /// <param name="e">The entity to move.</param>
        public static void UpdateEntityMovementVarsCurrent(Entity e)
        {
            if (e.MoveCycleStart == LoopCycle || e.SeqIndex == -1 || e.SeqDelayCycle != 0)
            {
                var startDelta = e.MoveCycleStart - e.MoveCycleEnd;
                var endDelta = (int)LoopCycle - e.MoveCycleEnd;
                var startX = e.MoveStartX * 128 + e.TileSize * 64;
                var startY = e.MoveStartY * 128 + e.TileSize * 64;
                var endX = e.MoveEndX * 128 + e.TileSize * 64;
                var endY = e.MoveEndY * 128 + e.TileSize * 64;
                e.JSceneX = (startX * (startDelta - endDelta) + endX * endDelta) / startDelta;
                e.JSceneY = (startY * (startDelta - endDelta) + endY * endDelta) / startDelta;
            }

            e.ResyncWalkCycle = 0;

            if (e.MoveDirection == 0)
            {
                e.DestRotation = 1024;
            }

            if (e.MoveDirection == 1)
            {
                e.DestRotation = 1536;
            }

            if (e.MoveDirection == 2)
            {
                e.DestRotation = 0;
            }

            if (e.MoveDirection == 3)
            {
                e.DestRotation = 512;
            }

            e.Rotation = e.DestRotation;
        }

        public static void UpdatePlayersRenderState()
        {
            for (int i = -1; i < PlayerCount; i++)
            {
                var index = -1;

                if (i == -1)
                    index = 2047;
                else
                    index = PlayerIndices[i];

                var p = Players[index];
                if (p != null)
                {
                    p.SetSceneZ(GetLandZ(p.JSceneX, p.JSceneY, Plane));
                    p.UnityObject.transform.rotation = Quaternion.Euler(0, p.Rotation / 5.688888888888889f, 0);
                    if (p.IsVisible)
                    {
                        p.ApplyAnimations();
                    }
                }
            }
        }

        public static void UpdateActorsRenderState()
        {
            for (int i = 0; i < ActorCount; i++)
            {
                var index = ActorIndices[i];

                var a = Actors[index];
                if (a != null)
                {
                    a.SetSceneZ(GetLandZ(a.JSceneX, a.JSceneY, Plane));
                    a.UnityObject.transform.rotation = Quaternion.Euler(0, a.Rotation / 5.688888888888889f, 0);
                    if (a.IsVisible)
                    {
                        a.ApplyAnimations();
                    }
                }
            }
        }

        public static void UpdateRenderStates()
        {
            UpdatePlayersRenderState();
            UpdateActorsRenderState();
            Scene.Update();
        }

        public static void HandlePlayers()
        {
            for (int i = -1; i < PlayerCount; i++)
            {
                var index = -1;

                if (i == -1)
                    index = 2047;
                else
                    index = PlayerIndices[i];

                var p = Players[index];
                if (p != null)
                {
                    UpdateEntity(p);
                }
            }
        }

        public static void HandleActors()
        {
            for (int i = 0; i < ActorCount; i++)
            {
                var index = ActorIndices[i];

                var a = Actors[index];
                if (a != null)
                {
                    UpdateEntity(a);
                }
            }
        }

        /// <summary>
        /// Performs an update on the provided entitiy's rotation.
        /// </summary>
        /// <param name="e">The entity to update the rotation of.</param>
        public static void UpdateEntityRotation(Entity e)
        {
            if (e.RotateSpeed == 0)
            {
                return;
            }

            if (e.FaceEntity != -1 && e.FaceEntity < 32768)
            {
                var a = Actors[e.FaceEntity];

                if (a != null)
                {
                    int dx = e.JSceneX - a.JSceneX;
                    int dy = e.JSceneY - a.JSceneY;

                    if (dx != 0 || dy != 0)
                    {
                        e.DestRotation = (int)(Math.Atan2(dx, dy) * 325.94900000000001D) & 0x7ff;
                    }
                }
            }

            if (e.FaceEntity >= 32768)
            {
                var playerIndex = e.FaceEntity - 32768;
                if (playerIndex == LocalPlayerIndex)
                {
                    playerIndex = 2047;
                }

                var p = Players[playerIndex];
                if (p != null)
                {
                    var dx = e.JSceneX - p.JSceneX;
                    var dy = e.JSceneY - p.JSceneY;
                    if (dx != 0 || dy != 0)
                    {
                        e.DestRotation = (int)(Math.Atan2(dx, dy) * 325.94900000000001D) & 0x7ff;
                    }
                }
            }

            if ((e.FaceX != 0 || e.FaceY != 0) && (e.PathPosition == 0 || e.ResyncWalkCycle > 0))
            {
                var dx = e.JSceneX - (e.FaceX - MapBaseX - MapBaseX) * 64;
                var dy = e.JSceneY - (e.FaceY - MapBaseY - MapBaseY) * 64;
                if (dx != 0 || dy != 0)
                {
                    e.DestRotation = (int)(Math.Atan2(dx, dy) * 325.94900000000001D) & 0x7ff;
                }

                e.FaceX = 0;
                e.FaceY = 0;
            }

            var da = e.DestRotation - e.Rotation & 0x7ff;
            if (da != 0)
            {
                if (da < e.RotateSpeed || da > 2048 - e.RotateSpeed)
                    e.Rotation = e.DestRotation;
                else if (da > 1024)
                    e.Rotation -= e.RotateSpeed;
                else
                    e.Rotation += e.RotateSpeed;

                e.Rotation &= 0x7ff;

                if (e.MoveSeqIndex == e.StandAnimation && e.Rotation != e.DestRotation)
                {
                    if (e.StandTurnAnimation != -1)
                    {
                        e.MoveSeqIndex = e.StandTurnAnimation;
                        return;
                    } else e.MoveSeqIndex = e.WalkAnimation;
                }
            }
        }

        /// <summary>
        /// Hides all entities that are stacked under other entities.
        /// </summary>
        public static void HideStackedEntities()
        {
            var hiddenEntities = new List<Entity>();
            for (int i = -1; i < PlayerCount; i++)
            {
                Entity cur = null;
                if (i == -1)
                    cur = Self;
                else
                    cur = Players[PlayerIndices[i]];

                if (hiddenEntities.Contains(cur)) continue;

                var curX = cur.JSceneX;
                var curY = cur.JSceneY;
                var curZ = cur.JSceneZ;

                for (int j = i + 1; j < PlayerCount; j++)
                {
                    var check = Players[PlayerIndices[j]];
                    var checkX = check.JSceneX;
                    var checkY = check.JSceneY;
                    var checkZ = check.JSceneZ;
                    if (curX == checkX && curY == checkY && curZ == checkZ)
                    {
                        check.UnityObject.SetActive(false);
                        hiddenEntities.Add(check);
                    }
                }

                for (int j = 0; j < ActorCount; j++)
                {
                    var check = Actors[ActorIndices[j]];
                    var checkX = check.JSceneX;
                    var checkY = check.JSceneY;
                    var checkZ = check.JSceneZ;
                    if (curX == checkX && curY == checkY && curZ == checkZ)
                    {
                        check.UnityObject.SetActive(false);
                        hiddenEntities.Add(check);
                    }
                }

                cur.UnityObject.SetActive(true);
            }

            for (int i = -1; i < ActorCount; i++)
            {
                Entity cur = null;
                if (i == -1)
                    cur = Self;
                else
                    cur = Actors[ActorIndices[i]];

                if (hiddenEntities.Contains(cur)) continue;

                var curX = cur.JSceneX;
                var curY = cur.JSceneY;
                var curZ = cur.JSceneZ;

                for (int j = i + 1; j < ActorCount; j++)
                {
                    var check = Actors[ActorIndices[j]];
                    var checkX = check.JSceneX;
                    var checkY = check.JSceneY;
                    var checkZ = check.JSceneZ;
                    if (curX == checkX && curY == checkY && curZ == checkZ)
                    {
                        check.UnityObject.SetActive(false);
                        hiddenEntities.Add(check);
                    }
                }

                for (int j = 0; j < PlayerCount; j++)
                {
                    var check = Players[PlayerIndices[j]];
                    var checkX = check.JSceneX;
                    var checkY = check.JSceneY;
                    var checkZ = check.JSceneZ;
                    if (curX == checkX && curY == checkY && curZ == checkZ)
                    {
                        check.UnityObject.SetActive(false);
                        hiddenEntities.Add(check);
                    }
                }

                cur.UnityObject.SetActive(true);
            }
        }

        /// <summary>
        /// Adds options to the menu for a specific widget.
        /// </summary>
        /// <param name="w"></param>
        /// <param name="screenX">The x screen coordinate of the widget.</param>
        /// <param name="screenY">The y screen coordinate of the widget.</param>
        /// <param name="mouseX">The x screen coordinate of the mouse.</param>
        /// <param name="mouseY">The yscreen coordinate of the mouse.</param>
        /// <param name="scrollAmount">The amount to scroll the widget by.</param>
        public static void BuildWidgetMenu(Widget w, int screenX, int screenY, int mouseX, int mouseY, int scrollAmount)
        {
            if (w.Config.Type != 0 || w.Config.ChildIds == null || w.Config.Hidden)
            {
                return;
            }

            if (!w.Config.Visible)
            {
                return;
            }

            if (mouseX < screenX || mouseY < screenY || mouseX > screenX + w.Config.Width || mouseY > screenY + w.Config.Height)
            {
                return;
            }

            for (int index = 0; index < w.Config.ChildIds.Length; index++)
            {
                var x = w.Config.ChildX[index] + screenX;
                var y = (w.Config.ChildY[index] + screenY) - scrollAmount;

                var child = w.GetChild(index);
                if (!child.Config.Visible)
                {
                    continue;
                }

                x += child.Config.X;
                y += child.Config.Y;

                var hovered = false;
                if (mouseX >= x && mouseY >= y && mouseX < x + child.Config.Width && mouseY < y + child.Config.Height)
                {
                    hovered = true;
                }

                if ((child.Config.HoverIndex >= 0 || child.Config.ColorHoverDisabled != 0) && hovered)
                {
                    if (child.Config.HoverIndex >= 0)
                    {
                        //tmp_hovered_widget = child.hover_index;
                    }
                    else
                    {
                        //tmp_hovered_widget = child.index;
                    }
                }

                if (child.Config.Type == 0)
                {
                    BuildWidgetMenu(child, x, y, mouseX, mouseY, child.Config.ScrollAmount);
                }
                else
                {
                    if (child.Config.OptionType == 1 && hovered)
                    {
                        var noOptions = false;

                        if (child.Config.ActionType != 0)
                        {
                            noOptions = false; // frenemy_option_valid(child, false);
                        }

                        if (!noOptions)
                        {
                            var option = new WidgetAction(child, child.Config.Option, false);
                            Menu.Add(option);
                        }
                    } else if (child.Config.OptionType == 2 && hovered)
                    {
                        var s = child.Config.OptionPrefix;
                        if (s.IndexOf(' ') != -1)
                        {
                            s = s.Substring(0, s.IndexOf(' '));
                        }

                        var option = new WidgetAction(child, s + " @gre@" + child.Config.OptionSuffix, false);
                        Menu.Add(option);
                    } else if (child.Config.OptionType == 3 && hovered)
                    {
                        var option = new WidgetAction(child, "Close", false);
                        Menu.Add(option);
                    } else if (child.Config.OptionType == 4 && hovered)
                    {
                        var option = new WidgetAction(child, child.Config.Option, false);
                        Menu.Add(option);
                    } else if (child.Config.OptionType == 5 && hovered)
                    {
                        var option = new WidgetAction(child, child.Config.Option, false);
                        Menu.Add(option);
                    } else if (child.Config.OptionType == 6 && hovered)
                    {
                        var option = new WidgetAction(child, child.Config.Option, false);
                        Menu.Add(option);
                    }

                    if (child.Config.Type == 2)
                    {
                        int slot = 0;

                        for (int column = 0; column < child.Config.Height; column++)
                        {
                            for (int row = 0; row < child.Config.Width; row++)
                            {
                                int slotX = x + row * (32 + child.Config.ItemMarginX);
                                int slotY = y + column * (32 + child.Config.ItemMarginY);

                                if (slot < 20)
                                {
                                    slotX += child.Config.ItemSlotX[slot];
                                    slotY += child.Config.ItemSlotY[slot];
                                }

                                if (mouseX >= slotX && mouseY >= slotY && mouseX < slotX + 32 && mouseY < slotY + 32)
                                {
                                    HoveredSlot = slot;
                                    HoveredSlotWidget = child.Config.Index;

                                    if (child.Config.ItemIndices[slot] > 0)
                                    {
                                        var oc = Cache.GetItemConfig(child.Config.ItemIndices[slot] - 1);

                                        if (SelectedItem && child.Config.ItemsHaveActions)
                                        {
                                            if (child.Config.Index != SelectedItemWidget || slot != SelectedItemSlot)
                                            {
                                                Menu.Add(new ItemOnItemAction(SelectedItemWidget, SelectedItemSlot, SelectedItemConfigIndex, child.Config.Index, slot, oc.index, "Use " + SelectedItemName + " with @lre@" + oc.name, false));
                                            }
                                        }
                                        else
                                        {
                                            if (child.Config.ItemsHaveActions)
                                            {
                                                for (int i = 4; i >= 3; i--)
                                                {
                                                    if (oc.action != null && oc.action[i] != null)
                                                    {
                                                        Menu.Add(new ItemAction(i, child.Config.Index, slot, oc.index, oc.action[i] + " @lre@" + oc.name, false));
                                                    }
                                                    else if (i == 4)
                                                    {
                                                        Menu.Add(new ItemAction(i, child.Config.Index, slot, oc.index, oc.action[i] + " @lre@" + oc.name, false));
                                                    }
                                                }
                                            }

                                            if (child.Config.ItemsUsable)
                                            {
                                                Menu.Add(new ItemAction(5, child.Config.Index, slot, oc.index, "Use @lre@" + oc.name, false));
                                            }

                                            if (child.Config.ItemsHaveActions && oc.action != null)
                                            {
                                                for (int i = 2; i >= 0; i--)
                                                {
                                                    if (oc.action[i] != null)
                                                    {
                                                        Menu.Add(new ItemAction(i, child.Config.Index, slot, oc.index, oc.action[i] + " @lre@" + oc.name, false));
                                                    }
                                                }
                                            }

                                            if (child.Config.ItemActions != null)
                                            {
                                                for (int i = 4; i >= 0; i--)
                                                {
                                                    if (child.Config.ItemActions[i] != null)
                                                    {
                                                        Menu.Add(new WidgetItemAction(i, child.Config.Index, slot, oc.index, child.Config.ItemActions[i] + " @lre@" + oc.name, false));
                                                    }
                                                }
                                            }

                                            Menu.Add(new ItemAction(6, child.Config.Index, slot, oc.index, "Examine @lre@" + oc.name, true));
                                        }
                                    }
                                }
                                slot++;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds options to the menu for a specific player.
        /// </summary>
        /// <param name="player">The player to add menu options for.</param>
        private static void BuildPlayerMenu(Player player)
        {
            for (var j = 0; j < PlayerOptions.Length; j++)
            {
                var opt = PlayerOptions[j];
                if (opt != null)
                {
                    Menu.Add(new PlayerAction(j, player.ServerIndex, opt.Text + " @whi@" + player.Name + StringUtils.GetLevelTag(player.CombatLevel), opt.Priority));
                }
            }
        }

        /// <summary>
        /// Adds options to the menu for a specific actor.
        /// </summary>
        /// <param name="actor">The actor to add menu options for.</param>
        private static void BuildActorMenu(Actor actor)
        {
            var name = actor.Config.Name;
            var attackPriority = false;

            if (actor.Config.CombatLevel != 0)
            {
                name = name + StringUtils.GetLevelTag(actor.Config.CombatLevel);

                if (actor.Config.CombatLevel > Self.CombatLevel)
                {
                    attackPriority = true;
                }
            }

            if (SelectedItem)
            {
                Menu.Add(new ItemOnActorAction(actor.ServerIndex, SelectedItemWidget, SelectedItemSlot, SelectedItemConfigIndex, "Use " + SelectedItemName + " with @yel@" + name, false));
            }
            else if (SelectedWidget)
            {
                if ((SelectedMask & 0x2) != 0)
                {
                    Menu.Add(new WidgetOnActorAction(actor.ServerIndex, SelectedWidgetIndex, SelectedTooltip + " @yel@" + name, false));
                }
            }
            else if (actor.Config.Action != null)
            {
                for (var i = 4; i >= 0; i--)
                {
                    var s = actor.Config.Action[i];
                    if (s != null && !s.Equals("null") && !s.Equals("attack"))
                    {
                        var action = new ActorAction(i, actor.ServerIndex, s + " @yel@" + name, false);
                        Menu.Add(action);
                    }
                }

                for (int i = 4; i >= 0; i--)
                {
                    var s = actor.Config.Action[i];
                    if (s != null && s.Equals("attack"))
                    {
                        var action = new ActorAction(i, actor.ServerIndex, s + " @yel@" + name, attackPriority);
                        Menu.Add(action);
                    }
                }

                Menu.Add(new ActorAction(5, actor.ServerIndex, "Examine @yel@" + name, true));
            }
        }

        /// <summary>
        /// Builds menu actions for anything in 3d scene.
        /// </summary>
        public static void BuildSceneMenu()
        {
            var hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 100.0f);
            RaycastHit? groundHit = null;
            var playerHits = new List<RaycastHit>();
            var actorHits = new List<RaycastHit>();
            var objectHits = new List<RaycastHit>();
            var wallHits = new List<RaycastHit>();
            var gdHits = new List<RaycastHit>();
            var wdHits = new List<RaycastHit>();
            var giHits = new List<RaycastHit>();
            var handledItemPiles = new bool[4, 104, 104];

            foreach (var hit in hits)
            {
                var collider = hit.collider;

                var isGround = collider.GetComponent<SceneComponent>() != null;
                if (isGround && !groundHit.HasValue)
                {
                    groundHit = hit;
                    continue;
                }

                var isPlayer = collider.GetComponent<PlayerComponent>() != null;
                if (isPlayer)
                {
                    playerHits.Add(hit);
                    continue;
                }

                var isActor = collider.GetComponent<ActorComponent>() != null;
                if (isActor)
                {
                    actorHits.Add(hit);
                    continue;
                }

                var isObject = collider.GetComponent<InteractiveComponent>() != null;
                if (isObject)
                {
                    objectHits.Add(hit);
                }

                var isWall = collider.GetComponent<WallComponent>() != null;
                if (isWall)
                {
                    wallHits.Add(hit);
                }

                var isGd = collider.GetComponent<GroundDecorationComponent>() != null;
                if (isGd)
                {
                    gdHits.Add(hit);
                }

                var isWd = collider.GetComponent<WallDecorationComponent>() != null;
                if (isWd)
                {
                    wdHits.Add(hit);
                }

                var isGi = collider.GetComponent<GroundItemComponent>() != null;
                if (isGi)
                {
                    giHits.Add(hit);
                }
            }

            if (hits.Length > 0)
            {
                if (groundHit != null)
                {
                    var point = groundHit.Value.point;
                    var x = GameConstants.UnscaleToTile(point.x);
                    var y = GameConstants.UnscaleToTile(point.z);
                    if (x >= 0 && y >= 0 && x < 104 && y < 104)
                    {
                        Menu.Add(new WalkHereMenuAction(x, y));
                    }
                }

                foreach (var hit in playerHits)
                {
                    var top = hit.collider.GetComponent<PlayerComponent>().Player;
                    for (var i = 0; i < PlayerCount; i++)
                    {
                        var cur = Players[PlayerIndices[i]];
                        if (cur.JSceneX == top.JSceneX && cur.JSceneY == top.JSceneY && cur != Self)
                        {
                            BuildPlayerMenu(cur);
                        }
                    }

                    for (var i = 0; i < ActorCount; i++)
                    {
                        var cur = Actors[ActorIndices[i]];
                        if (cur.JSceneX == top.JSceneX && cur.JSceneY == top.JSceneY)
                        {
                            BuildActorMenu(cur);
                        }
                    }
                }

                foreach (var hit in actorHits)
                {
                    var top = hit.collider.GetComponent<ActorComponent>().GameActor;
                    for (var i = 0; i < PlayerCount; i++)
                    {
                        var cur = Players[PlayerIndices[i]];
                        if (cur.JSceneX == top.JSceneX && cur.JSceneY == top.JSceneY && cur != Self)
                        {
                            BuildPlayerMenu(cur);
                        }
                    }

                    for (var i = 0; i < ActorCount; i++)
                    {
                        var cur = Actors[ActorIndices[i]];
                        if (cur.JSceneX == top.JSceneX && cur.JSceneY == top.JSceneY)
                        {
                            BuildActorMenu(cur);
                        }
                    }
                }

                foreach (var hit in objectHits)
                {
                    var obj = hit.collider.GetComponent<InteractiveComponent>().GameObject;
                    var x = (int)(obj.UniqueId & 0x7f);
                    var y = (int)(obj.UniqueId >> 7 & 0x7f);
                    var type = (int)(obj.UniqueId >> 61 & 3);

                    if (Scene.GetArrangement(Plane, x, y, obj.UniqueId) >= 0)
                    {
                        var index = (int)(obj.UniqueId >> 14 & 0xffff);
                        var desc = Cache.GetObjectConfig(index);
                        if (desc.childrenIds != null)
                        {
                            desc = desc.GetOverrideConfig();
                        }

                        if (desc == null)
                        {
                            continue;
                        }

                        if (desc.actions != null)
                        {
                            for (int i = 4; i >= 0; i--)
                            {
                                if (desc.actions[i] != null)
                                {
                                    Menu.Add(new ObjectAction(i, index, obj.UniqueId, x, y, desc.actions[i] + " @cya@" + desc.name, false));
                                }
                            }

                            Menu.Add(new ObjectAction(5, index, obj.UniqueId, x, y, "Examine @cya@" + desc.name, true));
                        }
                    }

                }

                foreach (var hit in wallHits)
                {
                    var obj = hit.collider.GetComponent<WallComponent>().GameObject;
                    var x = (int)(obj.UniqueId & 0x7f);
                    var y = (int)(obj.UniqueId >> 7 & 0x7f);
                    var type = (int)(obj.UniqueId >> 61 & 3);
                    if (Scene.GetArrangement(Plane, x, y, obj.UniqueId) >= 0)
                    {
                        var index = (int)(obj.UniqueId >> 14 & 0xffff);
                        var desc = Cache.GetObjectConfig(index);
                        if (desc.childrenIds != null)
                        {
                            desc = desc.GetOverrideConfig();
                        }

                        if (desc == null)
                        {
                            continue;
                        }

                        if (desc.actions != null)
                        {
                            for (int i = 4; i >= 0; i--)
                            {
                                if (desc.actions[i] != null)
                                {
                                    Menu.Add(new ObjectAction(i, index, obj.UniqueId, x, y, desc.actions[i] + " @cya@" + desc.name, false));
                                }
                            }
                            Menu.Add(new ObjectAction(5, index, obj.UniqueId, x, y, "Examine @cya@" + desc.name, true));
                        }
                    }
                }

                foreach (var hit in gdHits)
                {
                    var obj = hit.collider.GetComponent<GroundDecorationComponent>().GameObject;
                    var x = (int)(obj.UniqueId & 0x7f);
                    var y = (int)(obj.UniqueId >> 7 & 0x7f);
                    var type = (int)(obj.UniqueId >> 61 & 3);
                    if (Scene.GetArrangement(Plane, x, y, obj.UniqueId) >= 0)
                    {
                        var index = (int)(obj.UniqueId >> 14 & 0xffff);
                        var desc = Cache.GetObjectConfig(index);
                        if (desc.childrenIds != null)
                        {
                            desc = desc.GetOverrideConfig();
                        }

                        if (desc == null)
                        {
                            continue;
                        }

                        if (desc.actions != null)
                        {
                            for (int i = 4; i >= 0; i--)
                            {
                                if (desc.actions[i] != null)
                                {
                                    Menu.Add(new ObjectAction(i, index, obj.UniqueId, x, y, desc.actions[i] + " @cya@" + desc.name, false));
                                }
                            }
                            Menu.Add(new ObjectAction(5, index, obj.UniqueId, x, y, "Examine @cya@" + desc.name, true));
                        }
                    }
                }

                foreach (var hit in wdHits)
                {
                    var obj = hit.collider.GetComponent<WallDecorationComponent>().GameObject;
                    var x = (int)(obj.UniqueId & 0x7f);
                    var y = (int)(obj.UniqueId >> 7 & 0x7f);
                    var type = (int)(obj.UniqueId >> 61 & 3);
                    if (Scene.GetArrangement(Plane, x, y, obj.UniqueId) >= 0)
                    {
                        var index = (int)(obj.UniqueId >> 14 & 0xffff);
                        var desc = Cache.GetObjectConfig(index);
                        if (desc.childrenIds != null)
                        {
                            desc = desc.GetOverrideConfig();
                        }

                        if (desc == null)
                        {
                            continue;
                        }

                        if (desc.actions != null)
                        {
                            for (int i = 4; i >= 0; i--)
                            {
                                if (desc.actions[i] != null)
                                {
                                    Menu.Add(new ObjectAction(i, index, obj.UniqueId, x, y, desc.actions[i] + " @cya@" + desc.name, false));
                                }
                            }
                            Menu.Add(new ObjectAction(5, index, obj.UniqueId, x, y, "Examine @cya@" + desc.name, true));
                        }
                    }
                }

                foreach (var hit in giHits)
                {
                    var comp = hit.collider.GetComponent<GroundItemComponent>();
                    var items = comp.Items;
                    var x = items.UniqueId & 0x7f;
                    var y = items.UniqueId >> 7 & 0x7f;
                    if (!handledItemPiles[Plane, x, y])
                    {
                        handledItemPiles[Plane, x, y] = true;

                        var pile = GroundItems[Plane, x, y];
                        if (pile != null)
                        {
                            foreach (var cur in pile)
                            {
                                var desc = Cache.GetItemConfig(cur.Index);

                                for (int i = 4; i >= 0; i--)
                                {
                                    if (desc.groundAction != null && desc.groundAction[i] != null)
                                    {
                                        Menu.Add(new GroundItemAction(i, cur.Index, items.UniqueId, x, y, desc.groundAction[i] + " @lre@" + desc.name, false));
                                    }
                                    else if (i == 2)
                                    {
                                        Menu.Add(new GroundItemAction(i, cur.Index, items.UniqueId, x, y, "Take @lre@" + desc.name, false));
                                    }
                                }
                                Menu.Add(new GroundItemAction(5, cur.Index, items.UniqueId, x, y, "Examine @lre@" + desc.name, true));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Builds default menu actions (e.g. Cancel.)
        /// </summary>
        public static void BuildDefaultMenu()
        {
            var close = new DummyAction();
            close.Caption = "Cancel";
            close.HasPriority = true;
            Menu.Add(close);
        }

        /// <summary>
        /// Builds menu actions for all opened widgets.
        /// </summary>
        public static void BuildWidgetMenus()
        {
            var mpos = InputUtils.mousePosition;
            if (ViewportWidget != null)
            {
                BuildWidgetMenu(ViewportWidget, ViewportWidgetX, ViewportWidgetY, (int)mpos.x, (int)mpos.y, 0);
            }

            Chat.BuildWidgetMenu();
            TabArea.BuildWidgetMenu();
        }

        /// <summary>
        /// Builds menu actions for everything.
        /// </summary>
        public static void BuildMenu()
        {
            BuildDefaultMenu();

            if (ViewportWidget == null)
            {
                BuildSceneMenu();
            }

            BuildWidgetMenus();
            Chat.BuildMenu();
            TabArea.BuildTabMenu();
        }

        /// <summary>
        /// Performs a frame update on the menu.
        /// </summary>
        public static void MenuUpdate()
        {
            if (!Menu.Visible)
            {
                Menu.Reset();
                BuildMenu();
                Menu.Sort();
            }

            if (DragArea != 0)
            {
                return;
            }

            var mousePos = InputUtils.mousePosition;
            if (!Menu.Visible)
            {
                var ignore = false;

                if (Input.GetMouseButtonDown(0))
                {
                    var last = Menu.GetLast();

                    if (last is ItemAction || last is WidgetItemAction)
                    {
                        var slot = 0;
                        var index = 0;
                        if (last is ItemAction)
                        {
                            var conv = last as ItemAction;
                            slot = conv.WidgetSlot;
                            index = conv.WidgetId;
                        }
                        var widget = Cache.GetWidgetConfig(index);
                        if (widget != null && (widget.ItemsDraggable || widget.ItemsSwappable))
                        {
                            Dragging = false;
                            DragCycle = 0;
                            DragWidget = index;
                            DragSlot = slot;
                            DragStartX = (int)mousePos.x;
                            DragStartY = (int)mousePos.y;
                            DragArea = 1;
                            ignore = true;
                        }
                    }

                    if (!ignore)
                    {
                        Menu.ClickedLast();
                    }
                }

                if (!ignore && Input.GetMouseButtonDown(1))
                {
                    Menu.Show((int)Input.mousePosition.x, (int)InputUtils.mousePosition.y);
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Menu.Clicked();
                }
                else
                {
                    var menuX = Menu.X;
                    var menuY = Menu.Y;
                    var menuWidth = Menu.Width;
                    var menuHeight = Menu.Height;
                    if (mousePos.x < (menuX - 5) || mousePos.y < (menuY - 5) ||
                        mousePos.x > (menuX + menuWidth + 5) || mousePos.y > (menuY + menuHeight + 5))
                    {
                        Menu.Reset(true);
                    }
                }
            }
        }

        /// <summary>
        /// Destroys all projectiles within the scene.
        /// </summary>
        public static void DestroyProjectiles()
        {
            while (Projectiles.Count > 0)
            {
                var proj = Projectiles[0];
                proj.Destroy();
                Projectiles.Remove(proj);
            }
        }

        /// <summary>
        /// Performs a frame update on all projectiles within the scene.
        /// </summary>
        public static void ProjectilesUpdate()
        {
            var copy = new Projectile[Projectiles.Count];
            Projectiles.CopyTo(copy);

            foreach (var p in copy)
            {
                if (p.Plane != Plane || LoopCycle > p.CycleEnd)
                {
                    p.Destroy();
                    Projectiles.Remove(p);
                    continue;
                }

                if (LoopCycle >= p.CycleStart)
                {
                    if (p.TargetIndex > 0)
                    {
                        Actor a = Actors[p.TargetIndex - 1];
                        if (a != null && a.JSceneX >= 0 && a.JSceneX < 13312 && a.JSceneY >= 0 && a.JSceneY < 13312)
                        {
                            p.Update((int)LoopCycle, a.JSceneX, a.JSceneY, GetLandZ(a.JSceneX, a.JSceneY, p.Plane) - p.OffsetZ);
                        }
                    }

                    if (p.TargetIndex < 0)
                    {
                        int index = -p.TargetIndex - 1;
                        Player pl;

                        if (index == LocalPlayerIndex)
                        {
                            pl = Self;
                        }
                        else
                        {
                            pl = Players[index];
                        }

                        if (pl != null && pl.JSceneX >= 0 && pl.JSceneX < 13312 && pl.JSceneY >= 0 && pl.JSceneY < 13312)
                        {
                            p.Update((int)LoopCycle, pl.JSceneX, pl.JSceneY, GetLandZ(pl.JSceneX, pl.JSceneY, p.Plane) - p.OffsetZ);
                        }
                    }
                    p.Update((int)AnimCycle);
                    p.UpdateObject();
                }
            }
        }

        public static void InvalidateWidgetDisabledMessage(int widgetId)
        {
            if (ViewportWidget != null)
            {
                ViewportWidget.InvalidateDisabledString(widgetId);
            }

            var chatUnderlay = Chat.UnderlayWidget;
            if (chatUnderlay != null)
            {
                chatUnderlay.InvalidateDisabledString(widgetId);
            }

            var chatOverlay = Chat.OverlayWidget;
            if (chatOverlay != null)
            {
                chatOverlay.InvalidateDisabledString(widgetId);
            }
        }

        /// <summary>
        /// Invalidates the textures of an item within a widget item slot.
        /// </summary>
        /// <param name="widgetId">The id of the widget that the slot is within.</param>
        /// <param name="slot">The id of the slot containing the item texture to invalidate.</param>
        public static void InvalidateItemTexture(int widgetId, int slot)
        {
            if (ViewportWidget != null)
            {
                ViewportWidget.InvalidateItemImage(widgetId, slot);
            }

            TabArea.InvalidateItemTexture(widgetId, slot);

            var chatUnderlay = Chat.UnderlayWidget;
            if (chatUnderlay != null)
            {
                chatUnderlay.InvalidateItemImage(widgetId, slot);
            }

            var chatOverlay = Chat.OverlayWidget;
            if (chatOverlay != null)
            {
                chatOverlay.InvalidateItemImage(widgetId, slot);
            }
        }

        /// <summary>
        /// Handles a packet relative to the target coordinates.
        /// </summary>
        /// <param name="b">The buffer containing packet data.</param>
        /// <param name="opcode">The opcode of the packet being received.</param>
        public static void HandleTargetPacket(JagexBuffer b, int opcode)
        {
            if (opcode == 84)
            {
                int coord = b.ReadUByte();
                int x = TargetRegionX + (coord >> 4 & 7);
                int y = TargetRegionY + (coord & 7);
                int index = b.ReadUShort();
                int oldStackIndex = b.ReadUShort();
                int newStackIndex = b.ReadUShort();

                if (x >= 0 && y >= 0 && x < 104 && y < 104)
                {
                    var c = GroundItems[Plane, x, y];
                    if (c != null)
                    {
                        foreach (var item in c)
                        {
                            if (item.Index != (index & 0x7fff) || item.Amount != oldStackIndex)
                            {
                                continue;
                            }
                            item.Amount = newStackIndex;
                            break;
                        }

                        UpdateItemPile(x, y);
                    }
                }
                return;
            } else if (opcode == 156)
            {
                int coords = b.ReadUByteA();
                int x = TargetRegionX + (coords >> 4 & 7);
                int y = TargetRegionY + (coords & 7);
                int index = b.ReadUShort();
                if (x >= 0 && y >= 0 && x < 104 && y < 104)
                {
                    var pile = GroundItems[Plane, x, y];
                    if (pile != null)
                    {
                        for (int i = 0; i < pile.Count; i++)
                        {
                            var item = pile[i];
                            if (item.Index == (index & 0x7FFF))
                            {
                                pile.Remove(item);
                                break;
                            }
                        }

                        if (pile.Count == 0)
                        {
                            GroundItems[Plane, x, y] = null;
                        }
                        UpdateItemPile(x, y);
                    }
                }
                return;
            } else if (opcode == 44)
            {
                var itemIndex = b.ReadLEShortA();
                var stackIndex = b.ReadUShort();
                var offset = b.ReadUByte();
                var x = TargetRegionX + (offset >> 4 & 7);
                var y = TargetRegionY + (offset & 7);

                if (x >= 0 && y >= 0 && x < 104 && y < 104)
                {
                    var i = new GroundItem(itemIndex, stackIndex);

                    if (GroundItems[Plane, x, y] == null)
                    {
                        GroundItems[Plane, x, y] = new List<GroundItem>();
                    }

                    GroundItems[Plane, x, y].Add(i);
                    UpdateItemPile(x, y);
                }
                return;
            } else if (opcode == 117)
            {
                int offset = b.ReadUByte();
                int srcX = TargetRegionX + ((offset >> 4) & 7);
                int srcY = TargetRegionY + (offset & 7);
                int endX = srcX + b.ReadByte();
                int endY = srcY + b.ReadByte();
                int target = b.ReadShort();
                int effect = b.ReadUShort();
                int srcZ = b.ReadUByte() * 4;
                int endZ = b.ReadUByte() * 4;
                int delay = b.ReadUShort();
                int speed = b.ReadUShort();
                int slope = b.ReadUByte();
                int sourceSize = b.ReadUByte();

                if (srcX >= 0 && srcY >= 0 && srcX < 104 && srcY < 104 && endX >= 0 && endY >= 0 && endX < 104 && endY < 104 && effect != 65535)
                {
                    srcX = srcX * 128 + 64;
                    srcY = srcY * 128 + 64;
                    endX = endX * 128 + 64;
                    endY = endY * 128 + 64;

                    Projectile p = new Projectile(slope, endZ, delay + (int)LoopCycle, speed + (int)LoopCycle, sourceSize, Plane, GetLandZ(srcX, srcY, Plane) + srcZ, srcY, srcX, target, effect);
                    p.Update(delay + (int)LoopCycle, endX, endY, GetLandZ(endX, endY, Plane) + endZ);
                    Projectiles.Add(p);
                }
                return;
            }
        }

        /// <summary>
        /// The cache containing game assets.
        /// </summary>
        public static Cache Cache
        {
            get
            {
                return cache;
            }
            private set
            {
                if (cache != null)
                {
                    cache = value;
                }
            }
        }

        /// <summary>
        /// The pool containing all of our asset materials.
        /// </summary>
        public static MaterialPool MaterialPool
        {
            get { return texPool; }
            set { texPool = value; }
        }

        /// <summary>
        /// The network handler, handling IO connections to the game server.
        /// </summary>
        public static NetworkHandler NetworkHandler
        {
            get
            {
                if (networkHandler == null)
                {
                    GameObject obj = new GameObject("NetworkHandler");
                    networkHandler = obj.AddComponent<NetworkHandler>();
                }
                return networkHandler;
            }
        }
    }

}
