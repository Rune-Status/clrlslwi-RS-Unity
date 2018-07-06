namespace RS
{
    /// <summary>
    /// Handles a decoded packet.
    /// </summary>
    public interface PacketHandler
    {
        /// <summary>
        /// Handles the provided packet.
        /// </summary>
        /// <param name="opcode">The opcode of the packet being handled.</param>
        /// <param name="buffer">The buffer containing the packet data.</param>
        void Handle(int opcode, JagexBuffer buffer);
    }
}

