namespace RS
{
    /// <summary>
    /// Handles the packet specifying to update all items stored
    /// within a specific widget.
    /// </summary>
    public class SetWidgetItemsPacketHandler : PacketHandler
    {
        /// <summary>
        /// The number of items that specifies we need an int to fit
        /// the amount of items.
        /// </summary>
        private const int IntId = 255;

        public void Handle(int opcode, JagexBuffer buffer)
        {
            var index = buffer.ReadUShort();
            var size = buffer.ReadUShort();
            var desc = GameContext.Cache.GetWidgetConfig(index);
            if (desc == null)
            {
                return;
            }

            if (desc.ItemIndices == null)
            {
                return;
            }

            for (var i = 0; i < size; i++)
            {
                var count = buffer.ReadUByte();
                if (count == IntId)
                {
                    count = buffer.ReadImeInt();
                }
                desc.ItemIndices[i] = buffer.ReadLEUShortA();
                desc.ItemAmounts[i] = count;
                GameContext.InvalidateItemTexture(index, i);
            }
        }
    }
}
