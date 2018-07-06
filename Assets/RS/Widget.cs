using System;
using System.Collections.Generic;

using UnityEngine;

namespace RS
{
    /// <summary>
    /// A text line holds information about some text on a widget.
    /// 
    /// The information stored includes a cached texture with the text of line 
    /// draw on the texture. This is for performance.
    /// </summary>
    class TextLine
    {
        /// <summary>
        /// The texture containing the text.
        /// </summary>
        public Texture2D Texture;
        /// <summary>
        /// The text on this line.
        /// </summary>
        public string Text;
        /// <summary>
        /// The x coordinate of the text.
        /// </summary>
        public int X;
        /// <summary>
        /// The y coordinate of the text.
        /// </summary>
        public int Y;

        public TextLine(Texture2D tex, string text, int x, int y)
        {
            this.Texture = tex;
            this.Text = text;
            this.X = x;
            this.Y = y;
        }
    }

    /// <summary>
    /// Represents a 2D widget on the game screen.
    /// </summary>
    public class Widget
    {
        /// <summary>
        /// The config that specifies how to render the widget.
        /// </summary>
        public WidgetConfig Config;
        /// <summary>
        /// The texture to render when the widget is disabled.
        /// </summary>
        public Texture2D ImgDisabled;
        /// <summary>
        /// The texture to render when the widget is enabled.
        /// </summary>
        public Texture2D ImgEnabled;
        /// <summary>
        /// The textures to render in each slot of the widget.
        /// </summary>
        private Texture2D[] slotImages = new Texture2D[20];
        /// <summary>
        /// Textures containing the images for each item slot.
        /// </summary>
        private Texture2D[] slotItemImages;
        /// <summary>
        /// Textures containing the amounts for each item slot.
        /// </summary>
        private Texture2D[] amountImages;

        /// <summary>
        /// The children of this widget.
        /// </summary>
        private Widget[] children;

        /// <summary>
        /// Text lines stored in this widget.
        /// 
        /// We cache these for performance.
        /// </summary>
        private List<TextLine> lines = new List<TextLine>();

        /// <summary>
        /// The texture of the scrollbar being drawn on this widget.
        /// </summary>
        private Texture2D scrollbar;

        /// <summary>
        /// If this widget is being hovered over.
        /// </summary>
        private bool hovered = false;

        public Widget(WidgetConfig desc)
        {
            this.Config = desc;
            if (desc.ChildIds != null)
            {
                this.children = new Widget[desc.ChildIds.Length];
            }
            if (Config.ItemIndices != null)
            {
                this.slotItemImages = new Texture2D[Config.ItemIndices.Length];
                this.amountImages = new Texture2D[Config.ItemIndices.Length];
            }
            UpdateTextLines(false);
        }

        public void DrawScrollbar(int x, int y, int height, int scrollHeight, int scrollAmount)
        {
            if (scrollbar == null)
            {
                scrollbar = GameContext.CreateScrollbar(height, scrollHeight, scrollAmount);
            }

            // FIXME
            /*var up = ResourceCache.ImageScrollUp;
            var down = ResourceCache.ImageScrollDown;
            GUI.DrawTexture(new Rect(x, y, scrollbar.width, scrollbar.height), scrollbar);
            GUI.DrawTexture(new Rect(x, y, up.width, up.height), up);
            GUI.DrawTexture(new Rect(x, y + scrollbar.height, down.width, down.height), down);*/
        }

        private bool LineUpdateRequired()
        {
            string text = null;
            if (IsEnabled())
            {
                text = Config.MessageEnabled;
            }
            else
            {
                text = Config.MessageDisabled;
            }

            if (text == null)
            {
                return false;
            }

            var segments = text.Split(new string[] { "\\n" }, StringSplitOptions.None);
            if (segments.Length != lines.Count)
            {
                return true;
            }

            for (int i = 0; i < lines.Count; i++)
            {
                var segment = segments[i];
                if (segment.IndexOf('%') != -1)
                {
                    for (int j = 1; j < 5; j++)
                    {
                        do
                        {
                            int k = segment.IndexOf("%" + j);
                            if (k == -1)
                            {
                                break;
                            }

                            int value = -2;

                            if (Config.Script != null && j - 1 < Config.Script.Length)
                            {
                                WidgetScript scr = Config.Script[j - 1];

                                if (scr != null)
                                {
                                    value = scr.Execute();
                                }
                            }

                            segment = segment.Substring(0, k) + (value > 999999999 ? '*' : value) + segment.Substring(k + 2);
                        } while (true);
                    }
                }
                if (!segment.Equals(lines[i].Text))
                {
                    return true;
                }
            }
            return false;
        }

        private void UpdateTextLines(bool hovered)
        {
            lines.Clear();
            string text = null;
            if (IsEnabled())
            {
                text = Config.MessageEnabled;
            }
            else
            {
                text = Config.MessageDisabled;
            }

            if (text == null)
            {
                return;
            }

            var dy = 0;
            var segments = text.Split(new string[] { "\\n" }, StringSplitOptions.None);
            foreach (var seg in segments)
            {
                var segment = seg;
                if (segment.IndexOf('%') != -1)
                {
                    for (int j = 1; j < 5; j++)
                    {
                        int k = segment.IndexOf("%" + j);
                        while (k != -1)
                        {
                            int value = -2;
                            if (Config.Script != null && j - 1 < Config.Script.Length)
                            {
                                var scr = Config.Script[j - 1];
                                if (scr != null)
                                {
                                    value = scr.Execute();
                                }
                            }

                            segment = segment.Substring(0, k) + (value > 999999999 ? '*' : value) + segment.Substring(k + 2);
                            k = segment.IndexOf("%" + j);
                        }
                    }
                }

                if (segment.Length > 0)
                {
                    uint rgb = 0;
                    if (IsEnabled())
                    {
                        rgb = ColorUtils.ForceAlpha(Config.RGBEnabled);
                        if (hovered && Config.ColorHoverEnabled != 0)
                        {
                            rgb = ColorUtils.ForceAlpha(Config.ColorHoverEnabled);
                        }
                    }
                    else
                    {
                        rgb = ColorUtils.ForceAlpha(Config.RGBDisabled);
                        if (hovered && Config.ColorHoverDisabled != 0)
                        {
                            rgb = ColorUtils.ForceAlpha(Config.ColorHoverDisabled);
                        }
                    }

                    var tex = Config.Font.DrawString(segment, rgb, Config.Shadow, false);
                    var dx = 0;
                    if (Config.Centered)
                    {
                        dx += (Config.Width / 2) - (tex.width / 2);
                    }

                    lines.Add(new TextLine(tex, segment, dx, dy));
                }

                dy += Config.Font.CharHeight;
            }
        }

        private Texture2D GetImageDisabled()
        {
            if (ImgDisabled == null)
            {
                var s = Config.ImageDisabled;
                if (s != null && s.Length > 0)
                {
                    var split = s.Split(',');
                    var key = split[0];
                    var index = int.Parse(split[1]);
                    ImgDisabled = GameContext.Cache.GetImageAsTex(key, index);
                    TextureUtils.Replace(ImgDisabled, new Color(0, 0, 0), new Color(0, 0, 0, 0));
                    ImgDisabled.Apply();

                }
            }
            return ImgDisabled;
        }

        private Texture2D GetImageEnabled()
        {
            if (ImgEnabled == null)
            {
                var s = Config.ImageEnabled;
                if (s != null && s.Length > 0)
                {
                    var split = s.Split(',');
                    var key = split[0];
                    var index = int.Parse(split[1]);
                    ImgEnabled = GameContext.Cache.GetImageAsTex(key, index);
                    TextureUtils.Replace(ImgEnabled, new Color(0, 0, 0), new Color(0, 0, 0, 0));
                    ImgEnabled.Apply();

                }
            }
            return ImgEnabled;
        }

        private Texture2D GetSlotImage(int slot)
        {
            if (slot >= 20)
            {
                return null;
            }

            if (slotImages[slot] == null)
            {
                var texInfo = Config.SlotImage[slot];
                if (texInfo == null || texInfo.Length == 0)
                {
                    return null;
                }

                var split = texInfo.Split(',');
                var key = split[0];
                var index = int.Parse(split[1]);
                var tex = GameContext.Cache.GetImageAsTex(key, index);
                TextureUtils.Replace(tex, new Color(0, 0, 0), new Color(0, 0, 0, 0));
                tex.Apply();
                slotImages[slot] = tex;
            }

            return slotImages[slot];
        }

        private Texture2D GetSlotItemImage(int slot)
        {
            if (slotItemImages[slot] != null)
            {
                return slotItemImages[slot];
            }

            var outlineRgb = 0;
            if (GameContext.SelectedItem && GameContext.SelectedItemSlot == slot && GameContext.SelectedItemWidget == Config.Index)
            {
                outlineRgb = 0xFFFFFF;
            }

            slotItemImages[slot] = GameContext.Cache.GetItemConfig(Config.ItemIndices[slot] - 1).GetTexture(Config.ItemAmounts[slot], outlineRgb);
            return slotItemImages[slot];
        }

        public void InvalidateDisabledString(int widgetId)
        {
            if (widgetId == Config.Index)
            {
                ImgDisabled = null;
                return;
            }

            if (children != null)
            {
                foreach (var child in children)
                {
                    if (child != null)
                    {
                        child.InvalidateDisabledString(widgetId);
                    }
                }
            }
        }

        public void InvalidateItemImage(int widgetId, int slot)
        {
            if (widgetId == Config.Index)
            {
                slotItemImages[slot] = null;
                amountImages[slot] = null;
                return;
            }

            if (children != null)
            {
                foreach (var child in children)
                {
                    if (child != null)
                    {
                        child.InvalidateItemImage(widgetId, slot);
                    }
                }
            }
        }

        private Texture2D GetSlotAmountImage(int slot)
        {
            if (amountImages[slot] != null)
            {
                return amountImages[slot];
            }

            var s1 = "";
            var color = 0xFFFFFF00;

            var count = Config.ItemAmounts[slot];
            if (count >= 10000000)
            {
                s1 = "" + (count / 1000000) + 'M';
                color = 0xFF00FF00;
            }
            else if (count >= 100000)
            {
                s1 = "" + (count / 1000) + 'K';
                color = 0xFFFFFFFF;
            }
            else
            {
                s1 = "" + (count);
            }

            if (s1 == null)
            {
                s1 = "-2";
            }

            amountImages[slot] = GameContext.Cache.SmallFont.DrawString(s1, color, true, false);
            return amountImages[slot];
        }

        public bool IsEnabled()
        {
            if (Config.Script == null || Config.Script.Length == 0)
            {
                return false;
            }

            foreach (var s in Config.Script)
            {
                var cur = s.Execute();
                var @base = s.compareValue;

                switch (s.compareType)
                {
                    case WidgetScript.CompareTypeGreaterOrEqual:
                        if (cur >= @base)
                        {
                            return false;
                        }
                        break;

                    case WidgetScript.CompareTypeLesserOrEqual:
                        if (cur <= @base)
                        {
                            return false;
                        }
                        break;

                    case WidgetScript.CompareTypeEqual:
                        if (cur == @base)
                        {
                            return false;
                        }
                        break;

                    default:
                        if (cur != @base)
                        {
                            return false;
                        }
                        break;
                }
            }
            return true;
        }

        public void Draw(int x, int y, Rect rect)
        {
            var tmp = hovered;
            hovered = InputUtils.MouseWithin(new Rect(x, y, Config.Width, Config.Height));
            var hoverUpdated = (tmp != hovered);
            var mpos = InputUtils.mousePosition;

            switch (Config.Type)
            {
                case 2:
                    {
                        var slot = 0;
                        for (var slotY = 0; slotY < Config.Height; slotY++)
                        {
                            for (var slotX = 0; slotX < Config.Width; slotX++)
                            {
                                int drawX = x + slotX * (32 + Config.ItemMarginX);
                                int drawY = y + slotY * (32 + Config.ItemMarginY);
                                var dragDx = 0;
                                var dragDy = 0;

                                if (slot < 20)
                                {
                                    drawX += Config.ItemSlotX[slot];
                                    drawY += Config.ItemSlotY[slot];
                                }

                                var back = GetSlotImage(slot);
                                if (Config.ItemIndices[slot] > 0)
                                {
                                    var tex = GetSlotItemImage(slot);
                                    if (tex != null)
                                    {
                                        if (GameContext.DragArea != 0 && GameContext.DragSlot == slot && GameContext.DragWidget == Config.Index)
                                        {
                                            dragDx = (int)mpos.x - GameContext.DragStartX;
                                            dragDy = (int)mpos.y - GameContext.DragStartY;

                                            if (dragDx < 5 && dragDx > -5)
                                            {
                                                dragDx = 0;
                                            }

                                            if (dragDy < 5 && dragDy > -5)
                                            {
                                                dragDy = 0;
                                            }

                                            if (GameContext.DragCycle < 5)
                                            {
                                                dragDx = 0;
                                                dragDy = 0;
                                            }

                                            ResourceCache.ItemClickedMaterial.SetTexture("_MainTex", tex);
                                            Graphics.DrawTexture(new Rect(drawX + dragDx, drawY + dragDy, tex.width, tex.height), tex, ResourceCache.ItemClickedMaterial);
                                        }
                                        else
                                        {
                                            GUI.DrawTexture(new Rect(drawX, drawY, tex.width, tex.height), tex);
                                        }
                                    }

                                    if (Config.ItemAmounts[slot] != 1)
                                    {
                                        var stex = GetSlotAmountImage(slot);
                                        if (stex != null)
                                        {
                                            GUI.DrawTexture(new Rect(drawX + dragDx, drawY + dragDy, stex.width, stex.height), stex);
                                        }
                                    }
                                }
                                else if (back != null)
                                {
                                    GUI.DrawTexture(new Rect(drawX, drawY, back.width, back.height), back);
                                }

                                slot++;
                            }
                        }
                        break;
                    }

                case 4:
                    {
                        var bounds = new Rect(x, y, Config.Width, Config.Height);
                        if (LineUpdateRequired() || hoverUpdated)
                        {
                            UpdateTextLines(hovered);
                        }

                        foreach (var line in lines)
                        {
                            GUI.BeginGroup(rect);
                            GUI.DrawTexture(new Rect(x + line.X - rect.x, y + line.Y - rect.y, line.Texture.width, line.Texture.height), line.Texture);
                            GUI.EndGroup();
                        }
                        break;
                    }
                case 5:
                    {
                        Texture2D tex = null;
                        if (IsEnabled())
                        {
                            tex = GetImageEnabled();
                        }
                        else
                        {
                            tex = GetImageDisabled();
                        }

                        if (tex != null)
                        {
                            GUI.BeginGroup(rect);
                            GUI.DrawTexture(new Rect(x - rect.x, y - rect.y, tex.width, tex.height), tex);
                            GUI.EndGroup();
                        }
                        break;
                    }
            }
        }

        public Widget GetChild(int index)
        {
            var child = children[index];
            if (child == null)
            {
                child = new Widget(GameContext.Cache.GetWidgetConfig(Config.ChildIds[index]));
                children[index] = child;
            }
            return child;
        }

        public void Draw(int x, int y, int scrollAmount)
        {
            if (!Config.Visible)
            {
                return;
            }

            if (Config.ChildIds == null)
            {
                return;
            }

            if (Config.Hidden)
            {
                return;
            }

            var bounds = new Rect(x, y, Config.Width, Config.Height);
            for (var i = 0; i < Config.ChildIds.Length; i++)
            {
                var childX = Config.ChildX[i] + x;
                var childY = Config.ChildY[i] + y - scrollAmount;
                var child = GetChild(i);

                if (!child.Config.Visible)
                {
                    continue;
                }

                if (child.Config.Type == 0)
                {
                    child.Draw(childX, childY, child.Config.ScrollAmount);

                    if (child.Config.ScrollHeight > child.Config.Height)
                    {
                        child.DrawScrollbar(childX + child.Config.Width, childY, child.Config.Height, child.Config.ScrollHeight, child.Config.ScrollAmount);
                    }
                }
                else
                {
                    child.Draw(childX, childY, bounds);
                }
            }
        }

        public int ScrollAmount
        {
            get
            {
                return Config.ScrollAmount;
            }
            set
            {
                Config.ScrollAmount = value;

                if (Config.ScrollAmount < 0)
                {
                    Config.ScrollAmount = 0;
                }

                int max = Config.ScrollHeight - Config.Height;
                if (Config.ScrollAmount > max)
                {
                    Config.ScrollAmount = max;
                }

                scrollbar = null;
            }
        }

        public void ScrollWheel(int amount)
        {
            ScrollAmount += (int)((Config.ScrollHeight / 30.25D) * amount);
        }
    }
}
