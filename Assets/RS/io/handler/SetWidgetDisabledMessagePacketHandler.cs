namespace RS
{
    /// <summary>
    /// Handles the packet specifying to set a disabled message within a widget.
    /// </summary>
    public class SetWidgetDisabledMessagePacketHandler : PacketHandler
    {
        public void Handle(int opcode, JagexBuffer buffer)
        {
            var msg = buffer.ReadString(10);
            var index = buffer.ReadUShortA();

            var desc = GameContext.Cache.GetWidgetConfig(index);
            if (desc == null)
            {
                return;
            }

            desc.MessageDisabled = msg;
            GameContext.InvalidateWidgetDisabledMessage(index);
        }
    }
}

