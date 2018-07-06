
using UnityEngine;

namespace RS
{
    /// <summary>
    /// Represents the tab area on the gameframe.
    /// </summary>
    public class TabArea
    {
        /// <summary>
        /// All tabs to render within the gameframe.
        /// </summary>
        public Tab[] Tabs = new Tab[14];

        /// <summary>
        /// The index of the selected tab.
        /// </summary>
        public EncryptedInt SelectedTabIndex = 4;

        /// <summary>
        /// The widget being rendered.
        /// </summary>
        public Widget TabWidget = null;
        
        /// <summary>
        /// If the tabs are going to be stacked or not.
        /// </summary>
        public bool Stacked
        {
            get
            {
                return Camera.main.pixelWidth < 1000;
            }
        }

        /// <summary>
        /// Initializes the tab area.
        /// </summary>
        public void Init()
        {
            var ptr = 0;
            var redstone = ResourceCache.Redstone;
            var icons = ResourceCache.Icons;

            Tabs[ptr] = new Tab(ptr++, "Combat", redstone[1], icons[0], 0, 0, 8, 7, 33, 36);
            Tabs[ptr] = new Tab(ptr++, "Stats", redstone[1], icons[1], 0, 0, 3, 4, 33, 36);
            Tabs[ptr] = new Tab(ptr++, "Quest", redstone[1], icons[2], 0, 0, 4, 5, 33, 36);
            Tabs[ptr] = new Tab(ptr++, "Inventory", redstone[1], icons[3], 0, 0, 2, 2, 33, 36);
            Tabs[ptr] = new Tab(ptr++, "Equipment", redstone[1], icons[4], 0, 0, 2, 1, 33, 36);
            Tabs[ptr] = new Tab(ptr++, "Prayer", redstone[1], icons[5], 0, 0, 2, 2, 33, 36);
            Tabs[ptr] = new Tab(ptr++, "Magic", redstone[1], icons[6], 0, 0, 2, 5, 33, 36);

            Tabs[ptr] = new Tab(ptr++, "", redstone[1], icons[7], 0, 0, 4, 4, 33, 36);
            Tabs[ptr] = new Tab(ptr++, "Friends", redstone[1], icons[8], 0, 0, 4, 4, 33, 36);
            Tabs[ptr] = new Tab(ptr++, "Ignored", redstone[1], icons[9], 0, 0, 5, 4, 33, 36);
            Tabs[ptr] = new Tab(ptr++, "Log out", redstone[1], icons[10], 0, 0, 4, 3, 33, 36);
            Tabs[ptr] = new Tab(ptr++, "Settings", redstone[1], icons[11], 0, 0, 4, 4, 33, 36);
            Tabs[ptr] = new Tab(ptr++, "Emotes", redstone[1], icons[12], 0, 0, 5, 2, 33, 36);
            Tabs[ptr] = new Tab(ptr++, "", redstone[1], null, 0, 0, 0, 0, 33, 36);
        }

        /// <summary>
        /// Builds menu actions for the tab widget overlay.
        /// </summary>
        public void BuildWidgetMenu()
        {
            var mpos = InputUtils.mousePosition;
            var tabOverlay = TabWidget;
            if (tabOverlay != null)
            {
                GameContext.BuildWidgetMenu(tabOverlay, 553, 205, (int)mpos.x, (int)mpos.y, 0);
            }
            else
            {
                var sidebar = Tabs[SelectedTabIndex].Widget;
                if (sidebar != null)
                {
                    GameContext.BuildWidgetMenu(sidebar, 553, 205, (int)mpos.x, (int)mpos.y, 0);
                }
            }
        }

        /// <summary>
        /// Builds menu actions for the tab area.
        /// </summary>
        public void BuildTabMenu()
        {
            for (var i = 0; i < Tabs.Length; i++)
            {
                var tab = Tabs[i];
                if (tab != null)
                {
                    tab.BuildMenu();
                }
            }
        }

        /// <summary>
        /// Renders the tab area.
        /// </summary>
        public void Render()
        {
            Graphics.DrawTexture(new Rect(553.0f, 205.0f, ResourceCache.InvBack.width, ResourceCache.InvBack.height), ResourceCache.InvBack);

            var tabOverlay = TabWidget;
            if (tabOverlay == null)
            {
                foreach (var tab in Tabs)
                {
                    if (tab != null)
                    {
                        if (tab.Index == SelectedTabIndex)
                        {
                            tab.DrawRedstone();
                        }
                        tab.DrawIcon();
                    }
                }
            }
            else
            {
                foreach (var tab in Tabs)
                {
                    if (tab != null)
                    {
                        tab.DrawIcon();
                    }
                }
            }

            if (tabOverlay != null)
            {
                tabOverlay.Draw(553, 205, 0);
            }
            else
            {
                var widget = Tabs[SelectedTabIndex].Widget;
                if (widget != null)
                {
                    widget.Draw(553, 205, 0);
                }
            }
        }

        /// <summary>
        /// Performs a frame update to the tab area.
        /// </summary>
        public void Update()
        {
            var tabWidget = Tabs[SelectedTabIndex].Widget;
            if (tabWidget != null)
            {
                GameContext.UpdateWidget(tabWidget, 553, 205, 0);
            }
        }

        /// <summary>
        /// Invalidates a disabled message texture stored in the provided widget.
        /// </summary>
        /// <param name="widgetId">The id of the widget to invalidate the string in.</param>
        public void InvalidateWidgetString(int widgetId)
        {
            foreach (var tab in Tabs)
            {
                var widget = tab.Widget;
                if (widget != null)
                {
                    widget.InvalidateDisabledString(widgetId);
                }
            }
        }

        /// <summary>
        /// Invalidates an item texture of an item slot within a widget.
        /// </summary>
        /// <param name="widgetId">The id of the widget that the slot is within.</param>
        /// <param name="slot">The item slot to invalidate the texture of.</param>
        public void InvalidateItemTexture(int widgetId, int slot)
        {
            foreach (var tab in Tabs)
            {
                var widget = tab.Widget;
                if (widget != null)
                {
                    widget.InvalidateItemImage(widgetId, slot);
                }
            }
        }
    }
}
