using UnityEngine;

namespace RS
{
    /// <summary>
    /// Represents types of messages.
    /// </summary>
    public enum MessageType
    {
        Ambiguous,
        Player,
    }

    /// <summary>
    /// Represents states that the chat box can be in.
    /// </summary>
    public enum ChatState
    {
        None,
        Notify,
        AddFriend,
        AddIgnore,
        RemoveFriend,
        RemoveIgnore,
        SendPM,
        EnterAmount,
        EnterName,
    }

    /// <summary>
    /// Represents a chat message.
    /// </summary>
    public class ChatMessage
    {
        /// <summary>
        /// The type of this message.
        /// </summary>
        public MessageType Type;
        /// <summary>
        /// The sender of this message.
        /// </summary>
        public string Sender;
        /// <summary>
        /// The text in this message.
        /// </summary>
        public string Text;
        /// <summary>
        /// The index of the crown sprite to draw beside this message.
        /// </summary>
        public int CrownIndex = -1;
        /// <summary>
        /// If this message slot is currently in use.
        /// </summary>
        public bool Used = false;

        public ChatMessage(MessageType type, string sender, string text)
        {
            this.Type = type;
            this.Sender = sender;
            this.Text = text;
            this.Used = true;
        }

        /// <summary>
        /// Sets the data in this chat message.
        /// </summary>
        /// <param name="type">The new type.</param>
        /// <param name="sender">The new sender.</param>
        /// <param name="text">The new text.</param>
        /// <param name="crownIndex">The new crown index.</param>
        /// <param name="used">If the slot is used.</param>
        public void Copy(MessageType type, string sender, string text, int crownIndex, bool used)
        {
            this.Type = type;
            this.Sender = sender;
            this.Text = text;
            this.CrownIndex = crownIndex;
            this.Used = used;
        }

        /// <summary>
        /// Sets the data in this chat message.
        /// </summary>
        /// <param name="other">The message to copy the data from.</param>
        public void Copy(ChatMessage other)
        {
            Copy(other.Type, other.Sender, other.Text, other.CrownIndex, other.Used);
        }
    }

    /// <summary>
    /// A wrapper for the chat widget.
    /// </summary>
    public class Chat
    {
        /// <summary>
        /// The x coordinate of the chat box.
        /// </summary>
        public int X = 17;
        /// <summary>
        /// The y coordinate of the chat box.
        /// </summary>
        public int Y = 357;

        /// <summary>
        /// The inner width of the chat box.
        /// </summary>
        public const int InnerWidth = 506;

        /// <summary>
        /// The inner height of the chat box.
        /// </summary>
        public const int InnerHeight = 113;
        
        /// <summary>
        /// The height of the chat box.
        /// </summary>
        public const int Height = 175;

        /// <summary>
        /// The y coordinate of the first chat message.
        /// </summary>
        public const int MessageBeginY = InnerHeight - 17;

        /// <summary>
        /// The x coordinate of the scroll bar.
        /// </summary>
        public int ScrollBarX
        {
            get
            {
                return X + InnerWidth - 10;
            }
        }

        public const int SeparatorLength = InnerWidth;
        public const int MessageHeight = 14;
        public const int MinHeight = 114;

        /// <summary>
        /// A cache of history messages.
        /// </summary>
        private ChatMessage[] history = new ChatMessage[100];

        /// <summary>
        /// A cache of history message textures.
        /// </summary>
        private Texture2D[] messageTexs = new Texture2D[100];

        /// <summary>
        /// The maximum scroll height of the chat.
        /// </summary>
        public int ScrollHeight = 0;

        /// <summary>
        /// The current scroll amount of the chat.
        /// </summary>
        public int ScrollAmount = 0;

        /// <summary>
        /// The real number of messages being rendered.
        /// </summary>
        private int msgRenderCount = 0;

        /// <summary>
        /// The texture containing the scrollbar.
        /// </summary>
        private Texture2D scrollBarTex = null;

        /// <summary>
        /// The texture containing a line separator.
        /// </summary>
        private Texture2D lineTex = null;

        /// <summary>
        /// The name of the local player.
        /// </summary>
        private string name = "";

        /// <summary>
        /// The texture with the local player's name baked onto it.
        /// </summary>
        private Texture2D nameTex = null;

        /// <summary>
        /// The texture containing the baked input text.
        /// </summary>
        private Texture2D inputTex = null;

        /// <summary>
        /// The input text.
        /// </summary>
        private string input = "";
        
        /// <summary>
        /// The texture containing a bracket.
        /// </summary>
        private Texture2D bracketTex = null;

        /// <summary>
        /// The texture containing an asterik.
        /// </summary>
        private Texture2D asterikTex = null;

        /// <summary>
        /// The widget to draw under the chat.
        /// </summary>
        public Widget UnderlayWidget = null;

        /// <summary>
        /// The widget to draw over the chat.
        /// </summary>
        public Widget OverlayWidget = null;
        
        public Chat()
        {
            for (var i = 0; i < history.Length; i++)
            {
                history[i] = new ChatMessage(MessageType.Ambiguous, null, null);
                history[i].Used = false;
            }

            CreateLineTex();
        }

        /// <summary>
        /// Initializes the chat widget.
        /// </summary>
        public void Init()
        {
            // FIXME
            /*var allTab = new ChatTab(ResourceCache.ChatButton, ResourceCache.ChatButtonHovered, new Rect(5, 480, 56, 22), "All", new string[0], new uint[0]);
            chatTabAll = allTab;

            var gameTab = new ChatTab(ResourceCache.ChatButton, ResourceCache.ChatButtonHovered, new Rect(71, 480, 56, 22), "Game", new string[] { "On", "Off" }, new uint[] { 0xFF00FF00, 0xFFFF0000 });
            gameTab.Bind(0, () =>
            {
                return new SwitchTypeAction(gameTab, 0, "Turn on");
            });
            gameTab.Bind(1, () =>
            {
                return new SwitchTypeAction(gameTab, 1, "Turn off");
            });
            chatTabGame = gameTab;

            var publicTab = new ChatTab(ResourceCache.ChatButton, ResourceCache.ChatButtonHovered, new Rect(137, 480, 56, 22), "Public", new string[] { "On", "Hide", "Friends", "Off" }, new uint[] { 0xFF00FF00, 0xFF0000FF, 0xFF00FF00, 0xFFF0000 });
            publicTab.Bind(0, () =>
            {
                return new SwitchTypeAction(publicTab, 0, "Turn on");
            });
            publicTab.Bind(1, () =>
            {
                return new SwitchTypeAction(publicTab, 1, "Hide");
            });
            publicTab.Bind(2, () =>
            {
                return new SwitchTypeAction(publicTab, 2, "Friends only");
            });
            publicTab.Bind(3, () =>
            {
                return new SwitchTypeAction(publicTab, 3, "Turn off");
            });
            chatTabPublic = publicTab;

            var privateTab = new ChatTab(ResourceCache.ChatButton, ResourceCache.ChatButtonHovered, new Rect(203, 480, 56, 22), "Private", new string[] { "On", "Friends", "Off" }, new uint[] { 0xFF00FF00, 0xFF00FF00, 0xFFF0000 });
            privateTab.Bind(0, () =>
            {
                return new SwitchTypeAction(privateTab, 0, "Turn on");
            });
            privateTab.Bind(1, () =>
            {
                return new SwitchTypeAction(privateTab, 1, "Friends only");
            });
            privateTab.Bind(2, () =>
            {
                return new SwitchTypeAction(privateTab, 2, "Turn off");
            });
            chatTabPrivate = privateTab;

            var clanTab = new ChatTab(ResourceCache.ChatButton, ResourceCache.ChatButtonHovered, new Rect(269, 480, 56, 22), "Clan", new string[] { "On", "Friends", "Off" }, new uint[] { 0xFF00FF00, 0xFF00FF00, 0xFFF0000 });
            clanTab.Bind(0, () =>
            {
                return new SwitchTypeAction(clanTab, 0, "Turn on");
            });
            clanTab.Bind(1, () =>
            {
                return new SwitchTypeAction(clanTab, 1, "Friends only");
            });
            clanTab.Bind(2, () =>
            {
                return new SwitchTypeAction(clanTab, 2, "Turn off");
            });
            chatTabClan = clanTab;

            var tradeTab = new ChatTab(ResourceCache.ChatButton, ResourceCache.ChatButtonHovered, new Rect(335, 480, 56, 22), "Trade", new string[] { "On", "Friends", "Off" }, new uint[] { 0xFF00FF00, 0xFF00FF00, 0xFFF0000 });
            tradeTab.Bind(0, () =>
            {
                return new SwitchTypeAction(tradeTab, 0, "Turn on");
            });
            tradeTab.Bind(1, () =>
            {
                return new SwitchTypeAction(tradeTab, 1, "Friends only");
            });
            tradeTab.Bind(2, () =>
            {
                return new SwitchTypeAction(tradeTab, 2, "Turn off");
            });
            chatTabTrade = tradeTab;*/
        }

        /// <summary>
        /// Creates the texture of a separator line for the chat input.
        /// </summary>
        private void CreateLineTex()
        {
            lineTex = new Texture2D(SeparatorLength, 1, TextureFormat.RGB24, false, true);
            TextureRasterizer.DrawLineH(lineTex, 0, 0, SeparatorLength, 0xFF807660);
            lineTex.Apply();
        }

        /// <summary>
        /// Creates the texture of a bracket.
        /// </summary>
        private void CreateBracketTex()
        {
            bracketTex = GameContext.Cache.NormalFont.DrawString(":", 0xFFFFFFFF, true, false);
        }

        /// <summary>
        /// Creates the texture of an asterik.
        /// </summary>
        private void CreateAsterikTex()
        {
            asterikTex = GameContext.Cache.NormalFont.DrawString("*", 0xFFEEEEEE, true, false);
        }

        /// <summary>
        /// Creates the texture containing the name of teh local player.
        /// </summary>
        /// <param name="name">The name of teh local player.</param>
        public void CreateNameTex(string name)
        {
            this.name = StringUtils.Format(name);
            nameTex = GameContext.Cache.NormalFont.DrawString(this.name, 0xFFFFFFFF, true, true);
        }

        private void CreateInputTex()
        {
            if (input.Length == 0)
            {
                inputTex = null;
                return;
            }
            inputTex = GameContext.Cache.NormalFont.DrawString(input, 0xFFEEEEEE, true, true);
        }

        /// <summary>
        /// Processes the sending of a message.
        /// </summary>
        private void ProcessMessage()
        {
            if (input.StartsWith("::"))
            {
                var packet = new Packet(103);
                packet.WriteString(input.Substring(2), 10);
                GameContext.NetworkHandler.Write(packet);
            }
            else if (input.Length > 0)
            {
                var msg = new ChatMessage(MessageType.Player, name, input);
                msg.CrownIndex = GameContext.LocalRights;
                Add(msg);

                var self = GameContext.Self;
                if (self != null)
                {
                    self.SpokenMessage = input;
                    self.SpokenLife = 150;
                    self.SpokenTex = null;
                }

                var @out = new Packet(4);
                @out.WriteByte(0);
                var pos = @out.Position();
                @out.WriteByteS(0);
                @out.WriteByteS(0);
                var buf = new DefaultJagexBuffer(new byte[5000]);
                StringUtils.Pack(input, buf);
                @out.WriteBytesReversedA(buf.Array(), 0, buf.Position());
            }
        }

        /// <summary>
        /// Performs a frame update on the chat widget.
        /// </summary>
        public void Update()
        {
            var updated = false;
            KeyCode key;
            while ((key = InputManager.Instance.GetNextKey()) != KeyCode.None)
            {
                var text = InputUtils.ToText(key);
                if (InputManager.Instance.Pressed[(int)KeyCode.LeftShift] ||
                    InputManager.Instance.Pressed[(int)KeyCode.RightShift])
                {
                    text = StringUtils.ToUpper(text);
                }

                if (key == KeyCode.Backspace)
                {
                    if (input.Length > 0)
                    {
                        input = input.Substring(0, input.Length - 1);
                        updated = true;
                    }
                }

                if (key == KeyCode.Return)
                {
                    ProcessMessage();
                    input = "";
                    updated = true;
                }

                if (text.Length > 0)
                {
                    input += text;
                    updated = true;
                }
            }

            if (updated)
            {
                CreateInputTex();
            }
        }

        /// <summary>
        /// Renders all messages on the chat widget.
        /// </summary>
        private void RenderMessages()
        {
            GUI.BeginGroup(new Rect(X + 7, Y + 10, 503, 128));
            var counter = 0;
            for (var i = 0; i < history.Length; i++)
            {
                var msg = history[i];
                if (msg == null || !msg.Used) continue;

                var tex = messageTexs[i];
                var y = (MessageBeginY - (counter * MessageHeight)) + ScrollAmount;

                var x = 5;
                if (msg.CrownIndex != -1)
                {
                    var iconTex = ResourceCache.RankIcons[msg.CrownIndex];
                    GUI.DrawTexture(new Rect(x, y, iconTex.width, iconTex.height), iconTex);
                    x += iconTex.width;
                }

                if (msg.Type == MessageType.Ambiguous)
                {
                    if (y > -10 && y < 180)
                    {
                        GUI.DrawTexture(new Rect(x, y, tex.width, tex.height), tex);
                    }
                }
                else if (msg.Type == MessageType.Player)
                {
                    if (y > -10 && y < 180)
                    {
                        GUI.DrawTexture(new Rect(x, y, tex.width, tex.height), tex);
                    }
                }

                counter++;
            }

            GUI.EndGroup();

            if (scrollBarTex == null || msgRenderCount != counter)
            {
                msgRenderCount = counter;
                CreateScrollBarTex();
            }

            //var up = ResourceCache.ImageScrollUp;
            //var down = ResourceCache.ImageScrollDown;
            GUI.DrawTexture(new Rect(ScrollBarX, Y + 10, scrollBarTex.width, scrollBarTex.height), scrollBarTex);
            // FIXME
            //GUI.DrawTexture(new Rect(ScrollBarX, Y + 10, up.width, up.height), up);
            //GUI.DrawTexture(new Rect(ScrollBarX, Y + 10 + scrollBarTex.height, down.width, down.height), down);
            GUI.DrawTexture(new Rect(7, Y + 10 + InnerHeight, lineTex.width, lineTex.height), lineTex);

            var localX = 12;
            if (nameTex != null)
            {
                GUI.DrawTexture(new Rect(localX, Y + 10 + InnerHeight, nameTex.width, nameTex.height), nameTex);
                localX += nameTex.width;
            }

            if (bracketTex == null)
            {
                CreateBracketTex();
            }

            if (asterikTex == null)
            {
                CreateAsterikTex();
            }

            GUI.DrawTexture(new Rect(localX, Y + 10 + InnerHeight, bracketTex.width, bracketTex.height), bracketTex);
            localX += bracketTex.width;
            localX += 4;
            if (inputTex != null)
            {
                GUI.DrawTexture(new Rect(localX, Y + 10 + InnerHeight, inputTex.width, inputTex.height), inputTex);
                localX += inputTex.width;
            }


            GUI.DrawTexture(new Rect(localX, Y + 10 + InnerHeight, asterikTex.width, asterikTex.height), asterikTex);
        }

        /// <summary>
        /// Renders the chat widget.
        /// </summary>
        public void Render()
        {
            GUI.DrawTexture(new Rect(X, Y + 6, ResourceCache.ChatBack.width, ResourceCache.ChatBack.height), ResourceCache.ChatBack);
            
            if (UnderlayWidget != null)
            {
                UnderlayWidget.Draw(X + 7, Y + 10, 0);
            }
            else if (OverlayWidget != null)
            {
                OverlayWidget.Draw(X + 7, Y + 10, 0);
            }
            else
            {
                RenderMessages();
            }
        }

        /// <summary>
        /// Builds menu actions on all components of the chat.
        /// </summary>
        public void BuildMenu()
        {
            
        }

        /// <summary>
        /// Builds menu actions for all widgets in the chat area.
        /// </summary>
        public void BuildWidgetMenu()
        {
            var mpos = InputUtils.mousePosition;
            var chatOverlay = OverlayWidget;
            var chatUnderlay = UnderlayWidget;

            if (chatOverlay != null)
            {
                GameContext.BuildWidgetMenu(chatOverlay, X + 7, Y + 10, (int)mpos.x, (int)mpos.y, 0);
            }
            else if (chatUnderlay != null)
            {
                GameContext.BuildWidgetMenu(chatUnderlay, X + 7, Y + 10, (int)mpos.x, (int)mpos.y, 0);
            }
        }

        /// <summary>
        /// Resets all state stored in the chat.
        /// </summary>
        public void Reset()
        {
            foreach (var msg in history)
            {
                msg.Copy(MessageType.Player, null, null, -1, false);
            }
        }

        private void CreateScrollBarTex()
        {
            ScrollHeight = (msgRenderCount * 14) + 7;
            if (ScrollHeight < MinHeight)
            {
                ScrollHeight = MinHeight;
            }

            scrollBarTex = GameContext.CreateScrollbar(InnerHeight, ScrollHeight, ScrollHeight - ScrollAmount - InnerHeight);
        }

        private void CreateTexture(int index)
        {
            var msg = history[index];
            Texture2D tex = null;
            switch (msg.Type)
            {
                case MessageType.Ambiguous:
                    tex = GameContext.Cache.NormalFont.DrawString(msg.Text, 0xFFFFFFFF, true, true);
                    break;
                case MessageType.Player:
                    tex = GameContext.Cache.NormalFont.DrawString(msg.Sender + ": " + msg.Text, 0xFFFFFFFF, true, true);
                    break;
            }
            messageTexs[index] = tex;
        }

        /// <summary>
        /// Adds a new message to this chat.
        /// </summary>
        /// <param name="message">The message to add.</param>
        public void Add(ChatMessage message)
        {
            for (var i = history.Length - 1; i > 0; i--)
            {
                history[i].Copy(history[i - 1]);
                messageTexs[i] = messageTexs[i - 1];
            }
            history[0].Copy(message);
            history[0].Used = true;
            CreateTexture(0);
        }
    }
}
