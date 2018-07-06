namespace RS
{
    /// <summary>
    /// Represents an option bound to players.
    /// </summary>
    public class PlayerOption
    {
        /// <summary>
        /// The text of the option.
        /// </summary>
        public string Text;
        /// <summary>
        /// If this option has priority.
        /// </summary>
        public bool Priority;

        public PlayerOption(string text, bool priority)
        {
            Text = text;
            Priority = priority;
        }
    }
}
