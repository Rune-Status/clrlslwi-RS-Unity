using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace RS
{
    public class WidgetScript
    {
        public const int MathTypeAdd = 0;
        public const int MathTypeDivide = 2;
        public const int MathTypeMultiply = 3;
        public const int MathTypeSubtract = 1;

        public const int OpcodeReturn = 0;
        public const int OpcodeGetCurLevel = 1;
        public const int OpcodeGetMaxLevel = 2;
        public const int OpcodeGetExperience = 3;
        public const int OpcodeGetItemCount = 4;
        public const int OpcodeGetSetting = 5;
        public const int OpcodeGetExperienceForLevel = 6;
        public const int OpcodeGetSettingTruncated = 7;
        public const int OpcodeGetSelfCombatLevel = 8;
        public const int OpcodeGetTotalLevel = 9;
        public const int OpcodeItemExists = 10;
        public const int OpcodeEnergyLeft = 11;
        public const int OpcodeWeightCarried = 12;
        public const int OpcodeBitExtractSetting = 13;
        public const int OpcodeGetVarbitSetting = 14;
        public const int OpcodeBufferSubtract = 15;
        public const int OpcodeBufferDivide = 16;
        public const int OpcodeBufferMultiply = 17;
        public const int OpcodeGetSelfGlobalX = 18;
        public const int OpcodeGetSelfGlobalY = 19;
        public const int OpcodePeekCode = 20;

        public const int CompareTypeGreaterOrEqual = 2;
        public const int CompareTypeLesserOrEqual = 3;
        public const int CompareTypeEqual = 4;

        public byte compareType = 0;
        public int compareValue = -2;
        public byte index;
        public int[] opcodes;

        public WidgetScript()
        {
            
        }

        public WidgetScript(JagexBuffer b, int index)
        {
            this.index = (byte) index;
            LoadFromBuffer(b);
        }

        private void LoadFromBuffer(JagexBuffer b)
        {
            opcodes = new int[b.ReadUShort()];
            for (var i = 0; i < opcodes.Length; i++)
            {
                opcodes[i] = b.ReadUShort();
            }
        }

        public int Execute()
        {
            int returnValue = 0;
            int mathType = 0;
            int isntructionPointer = 0;
            int[] instructions = opcodes;

            do
            {
                int opcode = instructions[isntructionPointer++];
                int register = 0;
                byte mathBufferOpcode = MathTypeAdd;

                switch (opcode)
                {
                    case OpcodeReturn:
                        return returnValue;

                    case OpcodeGetCurLevel:
                        register = GameContext.SkillCurrentLevels[instructions[isntructionPointer++]];
                        break;

                    case OpcodeGetMaxLevel:
                        register = GameContext.SkillMaxLevels[instructions[isntructionPointer++]];
                        break;

                    case OpcodeGetExperience:
                        register = GameContext.SkillExperiences[instructions[isntructionPointer++]];
                        break;

                    case OpcodeGetItemCount:
                        var w = GameContext.Cache.GetWidgetConfig(instructions[isntructionPointer++]);
                        if (w != null)
                        {
                            int itemIndex = instructions[isntructionPointer++];
                            if (itemIndex >= 0)
                            {
                                for (int j = 0; j < w.ItemIndices.Length; j++)
                                {
                                    if (w.ItemIndices[j] == itemIndex + 1)
                                    {
                                        register += w.ItemAmounts[j];
                                    }
                                }
                            }
                        }
                        break;

                    case OpcodeGetSetting:
                        register = GameContext.Settings[instructions[isntructionPointer++]];
                        break;

                    case OpcodeGetExperienceForLevel:
                        register = GameConstants.SkillXPTable[GameContext.SkillMaxLevels[instructions[isntructionPointer++]] - 1];
                        break;

                    case OpcodeGetSettingTruncated:
                        register = (GameContext.Settings[instructions[isntructionPointer++]] * 100) / 46875;
                        break;

                    case OpcodeGetSelfCombatLevel:
                        register = GameContext.Self.CombatLevel;
                        break;

                    case OpcodeGetTotalLevel:
                        for (int j = 0; j < GameConstants.SkillCount; j++)
                        {
                            if (GameConstants.SkillEnabled[j])
                            {
                                register += GameContext.SkillMaxLevels[j];
                            }
                        }
                        break;

                    case OpcodeItemExists:
                        var child = GameContext.Cache.GetWidgetConfig(instructions[isntructionPointer++]);
                        if (child != null)
                        {
                            int item_index = instructions[isntructionPointer++] + 1;
                            if (item_index >= 0)
                            {
                                for (int slot = 0; slot < child.ItemIndices.Length; slot++)
                                {
                                    if (child.ItemIndices[slot] == item_index)
                                    {
                                        register = 999999999;
                                        break;
                                    }
                                }
                            }
                        }
                        break;

                    case OpcodeEnergyLeft:
                        //register = Game.energy_left;
                        break;
                    case OpcodeWeightCarried:
                        //register = Game.weight_carried;
                        break;
                    case OpcodeBitExtractSetting:
                        int setting = GameContext.Settings[instructions[isntructionPointer++]];
                        int shift = instructions[isntructionPointer++];
                        register = (setting & 1 << shift) == 0 ? 0 : 1;
                        break;
                    case OpcodeGetVarbitSetting:
                        isntructionPointer++;
                        /*VarBit varbit = VarBit.instance[code[i++]];
                        int offset = varbit.offset;
                        int lsb = Game.LSB_BIT_MASK[varbit.shift - offset];
                        register = (Game.settings[varbit.setting] >> offset) & lsb;*/
                        break;
                    case OpcodeBufferSubtract:
                        mathBufferOpcode = MathTypeSubtract;
                        break;
                    case OpcodeBufferDivide:
                        mathBufferOpcode = MathTypeDivide;
                        break;
                    case OpcodeBufferMultiply:
                        mathBufferOpcode = MathTypeMultiply;
                        break;
                    case OpcodeGetSelfGlobalX:
                        register = GameContext.MapBaseX + (GameContext.Self.JSceneX >> 7);
                        break;
                    case OpcodeGetSelfGlobalY:
                        register = GameContext.MapBaseY + (GameContext.Self.JSceneY >> 7);
                        break;
                    case OpcodePeekCode:
                        register = instructions[isntructionPointer++];
                        break;
                }

                if (mathBufferOpcode == MathTypeAdd)
                {
                    switch (mathType)
                    {
                        case MathTypeAdd:
                            returnValue += register;
                            break;

                        case MathTypeSubtract:
                            returnValue -= register;
                            break;

                        case MathTypeDivide:
                            if (register != 0)
                            {
                                returnValue /= register;
                            }
                            break;

                        case MathTypeMultiply:
                            returnValue *= register;
                            break;
                    }
                    mathType = MathTypeAdd;
                }
                else
                {
                    // set type to buffer opcode, allowing us to
                    // do math operations on the next iteration, due to
                    // the math opcode defaulting back to MathTypeAdd
                    mathType = mathBufferOpcode;
                }
            } while (true);
        }
    }
}
