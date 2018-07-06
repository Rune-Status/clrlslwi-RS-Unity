namespace RS
{
    /// <summary>
    /// Contains information about an item on the ground.
    /// </summary>
    public class GroundItem
    {
        /// <summary>
        /// The index of the cache config of the item.
        /// </summary>
        public int Index;
        /// <summary>
        /// The amount of items in the item stack.
        /// </summary>
        public int Amount;

        public GroundItem(int index, int amount)
        {
            Index = index;
            Amount = amount;
        }
    }
}
