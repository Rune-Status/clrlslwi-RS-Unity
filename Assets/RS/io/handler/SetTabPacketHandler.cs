namespace RS
{
    /// <summary>
    /// Handles the packet telling the client to change the selected
    /// tab on the gameframe.
    /// </summary>
    public class SetTabPacketHandler : PacketHandler
    {
        /// <summary>
        /// Represents an invalid widget id.
        /// </summary>
        private const int InvalidId = 65535;

        public void Handle(int opcode, JagexBuffer buffer)
        {
            var widgetId = buffer.ReadUShort();
            if (widgetId == InvalidId)
            {
                widgetId = 0;
            }

            int tabIndex = buffer.ReadUByteA();
            GameContext.TabArea.Tabs[tabIndex].WidgetId = widgetId;
        }
    }
}
