
using UnityEngine;

namespace RS
{
    /// <summary>
    /// Represents a menu action that changes the focused chat tab.
    /// </summary>
    public class SwitchChatTabAction : AbstractMenuAction
    {
        private ChatTab tab;

        public SwitchChatTabAction(ChatTab tab)
        {
            this.tab = tab;
            base.Caption = "Switch " + tab.Name;
        }

        public override void Callback(ActionMenu menu)
        {
            
        }
    }

    /// <summary>
    /// Represents a menu action that changes the display type of a tab.
    /// </summary>
    public class SwitchTypeAction : AbstractMenuAction
    {
        private ChatTab tab;
        private int index;

        public SwitchTypeAction(ChatTab tab, int index, string caption)
        {
            this.tab = tab;
            this.index = index;
            base.Caption = caption;
        }

        public override void Callback(ActionMenu menu)
        {
            tab.DisplayType = index;
        }
    }

    /// <summary>
    /// Represents a tab in the chat area.
    /// </summary>
    public class ChatTab
    {
        public delegate MenuAction CreateMenuAction();

        private Texture2D button;
        private Texture2D hovered;
        public Rect Bounds;

        public string Name;
        private Texture2D nameTexture;

        private string[] optionDisplays;
        private uint[] displayColors;
        private Texture2D[] displayTextures;
        private CreateMenuAction[] factories;

        public int DisplayType = 0;

        public ChatTab(Texture2D button, Texture2D hovered, Rect bounds, string name, string[] optionDisplays, uint[] displayColors)
        {
            this.button = button;
            this.hovered = hovered;
            this.Bounds = bounds;
            this.Name = name;
            this.optionDisplays = optionDisplays;
            this.displayColors = displayColors;
            this.factories = new CreateMenuAction[optionDisplays.Length];
            this.CreateTextures();
        }

        public int Height
        {
            get
            {
                return button.height;
            }
        }

        public void Bind(int index, CreateMenuAction action)
        {
            factories[index] = action;
        }

        private void CreateTextures()
        {
            nameTexture = GameContext.Cache.SmallFont.DrawString(Name, 0xFFFFFFFF, true, false);
            displayTextures = new Texture2D[optionDisplays.Length];
            for (var i = 0; i < optionDisplays.Length; i++)
            {
                displayTextures[i] = GameContext.Cache.SmallFont.DrawString(optionDisplays[i], displayColors[i], true, false);
            }
        }

        public void HandleMenu(ActionMenu menu)
        {
            if (InputUtils.MouseWithin(Bounds))
            {
                for (var i = 0; i < optionDisplays.Length; i++)
                {
                    var factory = factories[i];
                    if (factory != null)
                    {
                        menu.Add(factory());
                    }
                }
                menu.Add(new SwitchChatTabAction(this));
            }
        }

        public void Render()
        {
            var isHovered = false;
            if (!GameContext.Menu.Visible)
            {
                if (InputUtils.MouseWithin(Bounds))
                {
                    isHovered = true;
                }
            }

            var tex = button;
            if (isHovered)
            {
                tex = hovered;
            }

            GUI.DrawTexture(new Rect(Bounds.x, Bounds.y, tex.width, tex.height), tex);

            var hasDisplay = DisplayType < displayTextures.Length;
            if (hasDisplay)
            {
                var text = displayTextures[DisplayType];
                GUI.DrawTexture(new Rect(Bounds.x + (tex.width / 2) - (text.width / 2), Bounds.y + (tex.height / 2), text.width, text.height), text);
            }

            var y = Bounds.y;
            if (!hasDisplay)
            {
                y += (tex.height / 2) - (nameTexture.height / 2);
            }
            GUI.DrawTexture(new Rect(Bounds.x + (tex.width / 2) - (nameTexture.width / 2), y, nameTexture.width, nameTexture.height), nameTexture);
        }
    }
}
