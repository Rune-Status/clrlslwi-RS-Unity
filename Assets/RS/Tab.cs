using UnityEngine;

namespace RS
{
    /// <summary>
    /// Represents a tab in the gameframe.
    /// </summary>
    public class Tab
    {
        /// <summary>
        /// The index of this tab.
        /// </summary>
        private EncryptedInt index = new EncryptedInt(0);
        /// <summary>
        /// The name of this tab.
        /// </summary>
        public string Name;
        /// <summary>
        /// The x coordinate of this tab.
        /// </summary>
        public int X;
        /// <summary>
        /// The y coordinate of this tab.
        /// </summary>
        public int Y;
        /// <summary>
        /// The width of this tab.
        /// </summary>
        public int Width;
        /// <summary>
        /// The height of this tab.
        /// </summary>
        public int Height;
        /// <summary>
        /// The x offset to the redstone image.
        /// </summary>
        public int RedstoneOffX;
        /// <summary>
        /// The y offset to the redstone image.
        /// </summary>
        public int RedstoneOffY;
        /// <summary>
        /// The x offset to the icon image.
        /// </summary>
        public int IconOffX;
        /// <summary>
        /// The y offset to the icon image.
        /// </summary>
        public int IconOffY;
        /// <summary>
        /// The texture for the redstone to render for this tab.
        /// </summary>
        private Texture2D redstone;
        /// <summary>
        /// The texture for the icon to render for this tab.
        /// </summary>
        private Texture2D icon;
        /// <summary>
        /// The id of the widget being rendered within this tab.
        /// </summary>
        private EncryptedInt widgetId = new EncryptedInt(0);
        /// <summary>
        /// The widget being displayed within this tab.
        /// </summary>
        private Widget cachedWidget;

        public Tab(int index, string name, Texture2D redstone, Texture2D icon, int x, int y, int redstoneOffX, int redstoneOffY, int iconOffX, int iconOffY, int width, int height)
        {
            this.index.Value = index;
            Name = name;
            RedstoneOffX = redstoneOffX;
            RedstoneOffY = redstoneOffY;
            IconOffX = iconOffX;
            IconOffY = iconOffY;
            this.redstone = redstone;
            this.icon = icon;
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// The id of the widget being displayed within this tab.
        /// </summary>
        public int WidgetId
        {
            get
            {
                return widgetId;
            }
            set
            {
                if (widgetId != value)
                {
                    widgetId = value;
                    cachedWidget = null;
                }
            }
        }

        /// <summary>
        /// The index of this tab.
        /// </summary>
        public int Index
        {
            get
            {
                return index.Value;
            }
        }

        /// <summary>
        /// The widget being displayed over this tab.
        /// </summary>
        public Widget Widget
        {
            get
            {
                var binWidget = GameContext.Cache.GetWidgetConfig(widgetId.Value);
                if (binWidget == null)
                {
                    return null;
                }

                if (cachedWidget == null && widgetId > 0)
                {
                    cachedWidget = new Widget(binWidget);
                }
                return cachedWidget;
            }
        }

        /// <summary>
        /// Draws the icon of the tab.
        /// </summary>
        public void DrawIcon()
        {
            if (icon != null && widgetId.Value > 0)
            {
                Graphics.DrawTexture(new Rect(X + IconOffX, Y + IconOffY, icon.width, icon.height), icon);
            }
        }

        /// <summary>
        /// Draws the redstone of the tab.
        /// </summary>
        public void DrawRedstone()
        {
            if (redstone != null)
            {
                Graphics.DrawTexture(new Rect(X + RedstoneOffX, Y + RedstoneOffY, redstone.width, redstone.height), redstone);
            }
        }

        /// <summary>
        /// Builds menu actions for this tab.
        /// </summary>
        public void BuildMenu()
        {
            if (Widget != null)
            {
                if (InputUtils.MouseWithin(new Rect(X, Y, Width, Height)))
                {
                    GameContext.Menu.Add(new ChangeTabAction(index, Name, false));
                }
            }
        }

    }
}
