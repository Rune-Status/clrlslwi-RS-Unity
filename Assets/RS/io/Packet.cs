namespace RS
{
    /// <summary>
    /// Represents an IO packet.
    /// </summary>
    public class Packet : DefaultJagexBuffer
    {
        public int Opcode;

        public Packet(int opcode, byte[] b) : base(b)
        {
            Opcode = opcode;
        }

        public Packet(int opcode) : this(opcode, new byte[0])
        {

        }

        public void WriteOpcode(int opcode)
        {
            base.WriteByte(opcode + GameContext.OutCipher.NextInt());
        }
    }
}
