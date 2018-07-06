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
}
