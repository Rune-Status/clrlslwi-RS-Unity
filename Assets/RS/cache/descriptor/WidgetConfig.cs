using UnityEngine;

namespace RS
{
    /// <summary>
    /// A config that describes a 2d widget.
    /// </summary>
    public class WidgetConfig
    {
        public int ActionType;
        public bool Centered;
        public int[] ChildX;
        public int[] ChildY;
        public int[] ChildIds;
        public int ColorHoverDisabled;
        public int ColorHoverEnabled;
        public bool Filled;
        public int FontIdx;
        public int Height;
        public bool Hidden;
        public int HoverIndex;
        public string ImageDisabled;
        public string ImageEnabled;
        public int Index;
        public string[] ItemActions;
        public int[] ItemAmounts;
        public int[] ItemIndices;
        public int ItemMarginX;
        public int ItemMarginY;
        public int[] ItemSlotX;
        public int[] ItemSlotY;
        public bool ItemsDraggable;
        public bool ItemsHaveActions;
        public bool ItemsSwappable;
        public bool ItemsUsable;
        public string MessageDisabled;
        public string MessageEnabled;
        public int ModelIndexDisabled;
        public int ModelIndexEnabled;
        public int ModelPitch;
        public int ModelTypeDisabled;
        public int ModelTypeEnabled;
        public int ModelYaw;
        public int ModelZoom;
        public int Opacity;
        public string Option;
        public int OptionAction;
        public string OptionPrefix;
        public string OptionSuffix;
        public int OptionType;
        public int Parent;
        public int RGBDisabled;
        public int RGBEnabled;
        public WidgetScript[] Script;
        public int ScrollAmount;
        public int ScrollHeight;
        public int AnimationIndexDisabled;
        public int AnimationIndexEnabled;
        public int AnimationCycle;
        public int AnimationFrame;
        public bool Shadow;
        public string[] SlotImage;
        public int Type;
        public bool Visible = true;
        public int Width;
        public int X;
        public int Y;
        
        private BitmapFont typeFont;

        public void LoadScript(JagexBuffer b)
        {
            int count = b.ReadUByte();

            byte[] compare_type = null;
            int[] compare_value = null;

            if (count > 0)
            {
                compare_type = new byte[count];
                compare_value = new int[count];

                for (int i = 0; i < count; i++)
                {
                    compare_type[i] = (byte) b.ReadByte();
                    compare_value[i] = b.ReadUShort();
                }
            }

            count = b.ReadUByte();

            if (count > 0)
            {
                Script = new WidgetScript[count];

                for (var i = 0; i < count; i++)
                {
                    var s = new WidgetScript(b, i);
                    if (compare_type != null && i < compare_type.Length)
                    {
                        s.compareType = compare_type[i];
                        s.compareValue = compare_value[i];
                    }

                    Script[i] = s;
                }
            }
        }

        public BitmapFont Font
        {
            get
            {
                if (typeFont == null)
                {
                    switch (FontIdx)
                    {
                        case 0:
                            return typeFont = GameContext.Cache.SmallFont;
                        case 1:
                            return typeFont = GameContext.Cache.NormalFont;
                        case 2:
                            return typeFont = GameContext.Cache.BoldFont;
                        case 3:
                            return typeFont = GameContext.Cache.FancyFont;
                    }
                }
                
                return typeFont;
            }
        }

        /// <summary>
        /// Swaps items from one widget slot to another.
        /// </summary>
        /// <param name="from">The from slot index.</param>
        /// <param name="to">The to slot index.</param>
        public void SwapSlots(int from, int to)
        {
            int tmp = ItemIndices[from];
            ItemIndices[from] = ItemIndices[to];
            ItemIndices[to] = tmp;

            tmp = ItemAmounts[from];
            ItemAmounts[from] = ItemAmounts[to];
            ItemAmounts[to] = tmp;
        }
    }

    public class WidgetConfigProvider : IndexedProvider<WidgetConfig>
    {
        private WidgetConfig[] configs;

        public WidgetConfigProvider(CacheArchive archive)
        {
            var b = new DefaultJagexBuffer(archive.GetFile("data"));
            configs = new WidgetConfig[b.ReadUShort()];

            int parent = -1;
            while (b.Position() < b.Capacity())
            {
                int index = b.ReadUShort();

                if (index == 65535)
                {
                    parent = b.ReadUShort();
                    index = b.ReadUShort();
                }

                var w = new WidgetConfig();
                
                w.Index = index;
                w.Parent = parent;
                w.Type = b.ReadByte();
                w.OptionType = b.ReadByte();
                w.ActionType = b.ReadUShort();
                w.Width = b.ReadUShort();
                w.Height = b.ReadUShort();
                w.Opacity = b.ReadUByte();
                w.HoverIndex = b.ReadUByte();

                if (w.HoverIndex != 0)
                {
                    w.HoverIndex = (w.HoverIndex - 1 << 8) + b.ReadUByte();
                }
                else
                {
                    w.HoverIndex = -1;
                }

                w.LoadScript(b);

                if (w.Type == 0)
                {
                    w.ScrollHeight = b.ReadUShort();
                    w.Hidden = b.ReadByte() == 1;

                    var count = b.ReadUShort();
                    w.ChildIds = new int[count];
                    w.ChildX = new int[count];
                    w.ChildY = new int[count];

                    for (var i = 0; i < count; i++)
                    {
                        w.ChildIds[i] = b.ReadUShort();
                        w.ChildX[i] = b.ReadUShort();
                        w.ChildY[i] = b.ReadUShort();
                    }
                }

                if (w.Type == 2)
                {
                    w.ItemIndices = new int[w.Width * w.Height];
                    w.ItemAmounts = new int[w.Width * w.Height];
                    w.ItemsDraggable = b.ReadByte() == 1;
                    w.ItemsHaveActions = b.ReadByte() == 1;
                    w.ItemsUsable = b.ReadByte() == 1;
                    w.ItemsSwappable = b.ReadByte() == 1;
                    w.ItemMarginX = (short)b.ReadUByte();
                    w.ItemMarginY = (short)b.ReadUByte();
                    w.ItemSlotX = new int[20];
                    w.ItemSlotY = new int[20];
                    w.SlotImage = new string[20];

                    for (int i = 0; i < 20; i++)
                    {
                        if (b.ReadUByte() == 1)
                        {
                            w.ItemSlotX[i] = (short)b.ReadUShort();
                            w.ItemSlotY[i] = (short)b.ReadUShort();
                            var str = b.ReadString(10);

                            w.SlotImage[i] = str;
                        }
                    }

                    w.ItemActions = new string[5];
                    for (int i = 0; i < 5; i++)
                    {
                        w.ItemActions[i] = b.ReadString(10);
                        if (w.ItemActions[i].Length == 0)
                        {
                            w.ItemActions[i] = null;
                        }
                    }

                }

                if (w.Type == 3)
                {
                    w.Filled = b.ReadByte() == 1;
                }

                if (w.Type == 4 || w.Type == 1)
                {
                    w.Centered = b.ReadByte() == 1;
                    w.FontIdx = b.ReadUByte();
                    w.Shadow = b.ReadByte() == 1;
                }

                if (w.Type == 4)
                {
                    w.MessageDisabled = b.ReadString(10);
                    w.MessageEnabled = b.ReadString(10);
                }

                if (w.Type == 1 || w.Type == 3 || w.Type == 4)
                {
                    w.RGBDisabled = b.ReadInt();
                }

                if (w.Type == 3 || w.Type == 4)
                {
                    w.RGBEnabled = b.ReadInt();
                    w.ColorHoverDisabled = b.ReadInt();
                    w.ColorHoverEnabled = b.ReadInt();
                }

                if (w.Type == 5)
                {
                    var str = b.ReadString(10);
                    if (str.Length > 0)
                    {
                        w.ImageDisabled = str;
                    }

                    str = b.ReadString(10);
                    if (str.Length > 0)
                    {
                        w.ImageEnabled = str;
                    }
                }

                if (w.Type == 6)
                {
                    int i = b.ReadUByte();
                    if (i != 0)
                    {
                        w.ModelTypeDisabled = 1;
                        w.ModelIndexDisabled = (i - 1 << 8) + b.ReadUByte();
                    }

                    i = b.ReadUByte();
                    if (i != 0)
                    {
                        w.ModelTypeEnabled = 1;
                        w.ModelIndexEnabled = (i - 1 << 8) + b.ReadUByte();
                    }

                    i = b.ReadUByte();
                    if (i != 0)
                    {
                        w.AnimationIndexDisabled = (i - 1 << 8) + b.ReadUByte();
                    }
                    else {
                        w.AnimationIndexDisabled = -1;
                    }

                    i = b.ReadUByte();
                    if (i != 0)
                    {
                        w.AnimationIndexEnabled = (i - 1 << 8) + b.ReadUByte();
                    }
                    else {
                        w.AnimationIndexEnabled = -1;
                    }

                    w.ModelZoom = b.ReadUShort();
                    w.ModelPitch = b.ReadUShort();
                    w.ModelYaw = b.ReadUShort();
                }

                if (w.Type == 7)
                {
                    w.ItemIndices = new int[w.Width * w.Height];
                    w.ItemAmounts = new int[w.Width * w.Height];
                    w.Centered = b.ReadUByte() == 1;
                    w.FontIdx = b.ReadUByte();
                    w.Shadow = b.ReadByte() == 1;
                    w.RGBDisabled = b.ReadInt();
                    w.ItemMarginX = b.ReadUShort();
                    w.ItemMarginY = b.ReadUShort();
                    w.ItemsHaveActions = b.ReadByte() == 1;
                    w.ItemActions = new string[5];

                    for (var i = 0; i < 5; i++)
                    {
                        w.ItemActions[i] = b.ReadString(10);
                        if (w.ItemActions[i].Length == 0)
                        {
                            w.ItemActions[i] = null;
                        }
                    }
                }

                if (w.OptionType == 2 || w.Type == 2)
                {
                    w.OptionPrefix = b.ReadString(10);
                    w.OptionSuffix = b.ReadString(10);
                    w.OptionAction = b.ReadUShort();
                }

                if (w.OptionType == 1 || w.OptionType == 4 || w.OptionType == 5 || w.OptionType == 6)
                {
                    w.Option = b.ReadString(10);
                    if (w.Option.Length == 0)
                    {
                        if (w.OptionType == 1)
                        {
                            w.Option = "Ok";
                        }
                        if (w.OptionType == 4)
                        {
                            w.Option = "Select";
                        }
                        if (w.OptionType == 5)
                        {
                            w.Option = "Select";
                        }
                        if (w.OptionType == 6)
                        {
                            w.Option = "Continue";
                        }
                    }
                }

                configs[index] = w;
            }
        }
        

        public WidgetConfig Provide(int index)
        {
            if (index < 0 || index >= configs.Length)
            {
                Debug.Log(index + " out of bounds");
                return null;
            }
            return configs[index];
        }
    }
}
