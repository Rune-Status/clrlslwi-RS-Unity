namespace RS
{
    /// <summary>
    /// Handles the packet which changes the client target.
    /// </summary>
    public class TargetPacketHandler : PacketHandler
    {
        public void Handle(int opcode, JagexBuffer buffer)
        {
            GameContext.HandleTargetPacket(buffer, opcode);
        }
    }
}
