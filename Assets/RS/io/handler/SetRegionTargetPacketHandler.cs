namespace RS
{
    /// <summary>
    /// Handles the packet that changes the target region coordinates.
    /// 
    /// The target region coordinate affect a set of packets which base
    /// their coordinate on the target for reduced bandwidth usage.
    /// </summary>
    public class SetRegionTargetPacketHandler : PacketHandler
    {
        public void Handle(int opcode, JagexBuffer buffer)
        {
            GameContext.TargetRegionY = buffer.ReadUByteC();
            GameContext.TargetRegionX = buffer.ReadUByteC();
        }
    }
}
