using System.Collections.Generic;

using UnityEngine;

namespace RS
{
    /// <summary>
    /// Represents an action on a menu.
    /// </summary>
    public interface MenuAction
    {
        /// <summary>
        /// Retrieves the caption to show on the action.
        /// </summary>
        /// <returns>The caption to show on the action.</returns>
        string GetCaption();

        /// <summary>
        /// Called when this action is clicked.
        /// </summary>
        /// <param name="menu">The menu that this action is within.</param>
        void Callback(ActionMenu menu);

        /// <summary>
        /// Determines if this action has priority over other actions.
        /// </summary>
        /// <returns>If this action has priority over other actions.</returns>
        bool CheckHasPriority();
    }

    /// <summary>
    /// A menu action with most of the methods filled out already.
    /// </summary>
    public abstract class AbstractMenuAction : MenuAction
    {
        /// <summary>
        /// The text to display on the menu.
        /// </summary>
        public string Caption = "N/A";

        /// <summary>
        /// If this option has priority over normal options on the menu.
        /// </summary>
        public bool HasPriority = false;

        public AbstractMenuAction()
        {

        }

        public AbstractMenuAction(string caption)
        {
            Caption = caption;
        }

        public abstract void Callback(ActionMenu menu);

        public string GetCaption()
        {
            return Caption;
        }

        public bool CheckHasPriority()
        {
            return HasPriority;
        }
    }

    /// <summary>
    /// A menu action used for testinng.
    /// </summary>
    public class TestMenuAction : AbstractMenuAction
    {
        public TestMenuAction(string caption) : base(caption)
        {

        }

        public override void Callback(ActionMenu menu)
        {

        }
    }

    /// <summary>
    /// An action menu, containing a lot of actions.
    /// </summary>
    public class ActionMenu
    {
        public delegate void PreFire();

        private List<MenuAction> actions = new List<MenuAction>();
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public bool Visible;
        private Texture2D menuTexture;
        private int lastHovered = -1;
        public List<PreFire> PreFireListeners = new List<PreFire>();
        
        /// <summary>
        /// Adds a new action to this menu.
        /// </summary>
        /// <param name="action">The action to add.</param>
        public void Add(MenuAction action)
        {
            actions.Add(action);
        }

        /// <summary>
        /// The number of actions in this menu.
        /// </summary>
        public int ActionCount
        {
            get
            {
                return actions.Count;
            }
        }

        /// <summary>
        /// Sorts all of the actions in this menu, based on if they have priority or not.
        /// </summary>
        public void Sort()
        {
            var unsorted = false;
            while (!unsorted)
            {
                unsorted = true;
                for (int i = 0; i < actions.Count - 1; i++)
                {
                    var cur = actions[i];
                    var next = actions[i + 1];
                    if (!cur.CheckHasPriority() && next.CheckHasPriority())
                    {
                        actions[i] = next;
                        actions[i + 1] = cur;
                        unsorted = false;
                    }
                }
            }
        }

        /// <summary>
        /// Renders this menu.
        /// </summary>
        public void Render()
        {
            if (Visible)
            {
                var tmp = CalculateHovered();
                if (lastHovered != tmp)
                {
                    lastHovered = tmp;
                    BuildMenuTexture();
                }

                GUI.DrawTexture(new Rect(X, Y, Width, Height), menuTexture);
            }
        }

        /// <summary>
        /// Calculates the bounds of this menu.
        /// </summary>
        private void CalculateBounds()
        {
            Visible = true;
            Width = GameContext.Cache.BoldFont.GetWidth("Choose Option");
            Height = 15 * actions.Count + 21;

            foreach (var action in actions)
            {
                var check = GameContext.Cache.BoldFont.GetWidth(action.GetCaption());
                if (check > Width)
                {
                    Width = check;
                }
            }

            Width += 8;

            var cam = Camera.main;
            if (X < 0) X = 0;
            if (Y < 0) Y = 0;
            if ((X + Width) > cam.pixelWidth) X = cam.pixelWidth - Width;
            if ((Y + Height) > cam.pixelHeight) Y = cam.pixelHeight - Height;
        }
        
        /// <summary>
        /// Calculates the menu option the end user is hovering over.
        /// </summary>
        /// <returns>The index of the option being hovered over, or -1 if none are being hovered.</returns>
        private int CalculateHovered()
        {
            var mousePos = InputUtils.mousePosition;
            var mouseX = mousePos.x - X;
            var mouseY = mousePos.y - Y;

            for (int i = 0; i < actions.Count; i++)
            {
                int optionY = 31 + (actions.Count - 1 - i) * 15;
                if (mouseX >= 0 && mouseY >= 0 && mouseX < Width && mouseY < Height)
                {
                    if (mouseY > optionY - 13 && mouseY < optionY + 3)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Builds the menu texture containing all options, hover, etc.
        /// </summary>
        private void BuildMenuTexture()
        {
            menuTexture = new Texture2D(Width, Height, TextureFormat.ARGB32, false, true);

            TextureRasterizer.FillRect(menuTexture, 0, 0, Width, Height, 0xFF5D5447);
            TextureRasterizer.FillRect(menuTexture, 1, 1, Width - 2, 16, 0xFF000000);
            TextureRasterizer.DrawRect(menuTexture, 1, 18, Width - 2, Height - 19, 0xFF000000);
            GameContext.Cache.BoldFont.DrawString(menuTexture, "Choose Option", 3, 14, 0xFF5D5447, true, true);

            for (int i = 0; i < actions.Count; i++)
            {
                var optionY = 31 + (actions.Count - 1 - i) * 15;
                var color = 0xFFFFFFFFu;
                if (i == lastHovered)
                {
                    color = 0xFFFFFF00;
                }

                GameContext.Cache.BoldFont.DrawString(menuTexture, actions[i].GetCaption(), 3, optionY, color, true, true);
            }

            menuTexture.Apply();
        }

        /// <summary>
        /// Shows this menu.
        /// </summary>
        /// <param name="x">The x coordinate to show at.</param>
        /// <param name="y">The y coordinate to show at.</param>
        /// <param name="force">IF the states will be updated even if the menu is already open.</param>
        public void Show(int x, int y, bool force = false)
        {
            if (Visible && !force)
            {
                return;
            }

            X = x;
            Y = y;
            CalculateBounds();
            BuildMenuTexture();
        }

        private void Execute(MenuAction action)
        {
            foreach (var cb in PreFireListeners)
            {
                cb();
            }
            action.Callback(this);
        }

        /// <summary>
        /// Called when this menu is clicked on.
        /// </summary>
        public void Clicked()
        {
            int idx = CalculateHovered();
            if (idx != -1)
            {
                Execute(actions[idx]);
            }
            Reset(true);
        }

        /// <summary>
        /// Called when the last menu option is clicked on.
        /// </summary>
        public void ClickedLast()
        {
            if (actions.Count > 0)
            {
                Execute(actions[actions.Count - 1]);
            }
        }

        /// <summary>
        /// Retrieves the last menu option in the list.
        /// </summary>
        /// <returns>The last menu option in the list.</returns>
        public MenuAction GetLast()
        {
            if (actions.Count == 0)
            {
                return null;
            }

            return actions[actions.Count - 1];
        }
        
        /// <summary>
        /// Resets all state stored in this menu.
        /// </summary>
        /// <param name="close">If the menu will be closed.</param>
        public void Reset(bool close = false)
        {
            actions.Clear();
            if (close)
            {
                menuTexture = null;
                X = 0;
                Y = 0;
                Width = 0;
                Height = 0;
                Visible = false;
            }
        }
    }
}
