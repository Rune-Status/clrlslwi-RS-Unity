namespace RS
{
    /// <summary>
    /// Handles the packet specifying to set the level/experience of a particular stat.
    /// </summary>
    public class SetStatPacketHandler : PacketHandler
    {
        public void Handle(int opcode, JagexBuffer buffer)
        {
            var index = buffer.ReadUByte();
            var exp = buffer.ReadMeInt();
            var level = buffer.ReadUShort();
            var maxLevel = buffer.ReadUShort();
            GameContext.SkillExperiences[index] = exp;
            GameContext.SkillCurrentLevels[index] = level;
            GameContext.SkillMaxLevels[index] = 1;
            for (int i = 0; i < 98; i++)
            {
                if (exp >= GameConstants.SkillXPTable[i])
                {
                    GameContext.SkillMaxLevels[index] = i + 2;
                }
            }
        }
    }
}
