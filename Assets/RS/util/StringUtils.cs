using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace RS
{
    public class StringUtils
    {
        public static char[] ValidNameCharacters = { '_', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        public static char[] ChatCharacters = { ' ', 'e', 't', 'a', 'o', 'i', 'h', 'n', 's', 'r', 'd', 'l', 'u', 'm', 'w', 'c', 'y', 'f', 'g', 'p', 'b', 'v', 'k', 'x', 'j', 'q', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ' ', '!', '^', '|', '<', '?', '.', ',', ':', ';', '(', ')', '-', '&', '*', '\\', '\'', '/', '@', '#', '+', '=', '\u0243', '$', '%', '"', '[', ']' };
        private static char[] formatBuffer = new char[100];

        public static void Pack(string s, JagexBuffer buffer)
        {
            if (s.Length > 80)
            {
                s = s.Substring(0, 80);
            }

            s = s.ToLower();

            int a = -1;
            for (int i = 0; i < s.Length; i++)
            {
                int b = 0;
                char c = s.ToCharArray()[i];
                for (int l = 0; l < ChatCharacters.Length; l++)
                {
                    if (c != ChatCharacters[l])
                    {
                        continue;
                    }
                    b = l;
                    break;
                }

                if (b > 12)
                {
                    b += 195;
                }

                if (a == -1)
                {
                    if (b < 13)
                    {
                        a = b;
                    }
                    else
                    {
                        buffer.WriteByte(b);
                    }
                }
                else if (b < 13)
                {
                    buffer.WriteByte((a << 4) + b);
                    a = -1;
                }
                else
                {
                    buffer.WriteByte((a << 4) + (b >> 4));
                    a = b & 0xF;
                }
            }

            if (a != -1)
            {
                buffer.WriteByte(a << 4);
            }
        }

        public static string GetFormatted(int length, JagexBuffer b)
        {
            int off = 0;
            int k = -1;
            for (int i = 0; i < length; i++)
            {
                int i1 = b.ReadUByte();
                int j1 = i1 >> 4 & 0xf;

                if (k == -1)
                {
                    if (j1 < 13)
                    {
                        formatBuffer[off++] = ChatCharacters[j1];
                    }
                    else {
                        k = j1;
                    }
                }
                else {
                    formatBuffer[off++] = ChatCharacters[((k << 4) + j1) - 195];
                    k = -1;
                }

                j1 = i1 & 0xF;

                if (k == -1)
                {
                    if (j1 < 13)
                    {
                        formatBuffer[off++] = ChatCharacters[j1];
                    }
                    else {
                        k = j1;
                    }
                }
                else {
                    formatBuffer[off++] = ChatCharacters[((k << 4) + j1) - 195];
                    k = -1;
                }
            }

            var capitalize = true;
            for (int i = 0; i < off; i++)
            {
                char c = formatBuffer[i];

                if (capitalize && c >= 'a' && c <= 'z')
                {
                    formatBuffer[i] += '\uFFE0';
                    capitalize = false;
                }

                if (c == '.' || c == '!' || c == '?')
                {
                    capitalize = true;
                }
            }

            return new string(formatBuffer, 0, off);
        }

        public static int HashString(string s)
        {
            s = s.ToUpper();

            var hash = 0;
            for (var j = 0; j < s.Length; j++)
            {
                hash = (hash * 61 + s.ToCharArray()[j]) - 32;
            }
            return hash;
        }

        public static string LongToString(long l)
        {
            if (l <= 0L || l >= 0x5b5b57f8a98a5dd1L)
            {
                return "";
            }

            if (l % 37L == 0L)
            {
                return "";
            }

            var len = 0;
            var characters = new char[12];
            while (l != 0L)
            {
                long l1 = l;
                l /= 37L;
                characters[11 - len++] = ValidNameCharacters[(int)(l1 - l * 37L)];
            }
            return new string(characters, 12 - len, len);
        }


        public static long StringToLong(string s)
        {
            var l = 0L;
            for (var i = 0; i < s.Length && i < 12; i++)
            {
                char c = s.ToCharArray()[i];

                l *= 37L;
                if (c >= 'A' && c <= 'Z')
                {
                    l += (1 + c) - 65;
                }
                else if (c >= 'a' && c <= 'z')
                {
                    l += (1 + c) - 97;
                }
                else if (c >= '0' && c <= '9')
                {
                    l += (27 + c) - 48;
                }
            }

            for (; l % 37L == 0L && l != 0L; l /= 37L) ;

            return l;
        }

        public static string Format(string s)
        {
            if (s.Length > 0)
            {
                var chars = s.ToCharArray();
                for (var i = 0; i < chars.Length; i++)
                {
                    if (chars[i] == '_')
                    {
                        chars[i] = ' ';
                        if (i + 1 < chars.Length && chars[i + 1] >= 'a' && chars[i + 1] <= 'z')
                        {
                            chars[i + 1] = (char)((chars[i + 1] + 65) - 97);
                        }
                    }
                }
                if (chars[0] >= 'a' && chars[0] <= 'z')
                {
                    chars[0] = (char)((chars[0] + 65) - 97);
                }
                return new string(chars);
            }
            else
            {
                return s;
            }
        }

        public static string GetLevelTag(int level)
        {
            var difference = GameContext.Self.CombatLevel - level;
            var s = new StringBuilder(" ");
            if (difference < -9)
            {
                s.Append("@red@");
            }
            else if (difference < -6)
            {
                s.Append("@or3@");
            }
            else if (difference < -3)
            {
                s.Append("@or2@");
            }
            else if (difference < 0)
            {
                s.Append("@or1@");
            }
            else if (difference > 9)
            {
                s.Append("@gre@");
            }
            else if (difference > 6)
            {
                s.Append("@gr3@");
            }
            else if (difference > 3)
            {
                s.Append("@gr2@");
            }
            else if (difference > 0)
            {
                s.Append("@gr1@");
            }
            else {
                s.Append("@yel@");
            }
            return s.Append("(level-").Append(level).Append(")").ToString();
        }

        public static string ToUpper(string s)
        {
            if (s.Equals(";"))
            {
                return ":";
            }
            if (s.Equals(","))
            {
                return "<";
            }
            if (s.Equals("."))
            {
                return ">";
            }
            if (s.Equals("7"))
            {
                return "&";
            }
            return s.ToUpper();
        }
    }
}
