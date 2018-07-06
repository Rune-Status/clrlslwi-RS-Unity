using System;
using UnityEngine;

namespace RS
{
    /// <summary>
    /// Provides content related to general gameplay, and logic.
    /// </summary>
    public sealed class GameConstants
    {
        /// <summary>
        /// The key to use for encrypting fields.
        /// </summary>
        public const int IntEncryptKey = 0x7FEBA783;
        /// <summary>
        /// The scale to apply to all points when rendering.
        /// </summary>
        public const float RenderScale = 0.02f;
        /// <summary>
        /// The size of a tile in 3d RS coordinates.
        /// </summary>
        public const int IntSceneTileSize = 128;

        /// <summary>
        /// The size of all packets.
        /// 
        /// These are required, since only certain packet types have a size in their
        /// header to size on bandwidth.
        /// </summary>
        public static readonly int[] PacketSizes = {
            0, 0, 0, 1, 6, 0, 0, 0, 4, 0,       // 0
		    0, 2, -1, 1, 1, -1, 1, 0, 0, 0,     // 10
		    0, 0, 0, 0, 1, 0, 0, -1, 1, 1,      // 20
		    0, 0, 0, 0, -2, 4, 3, 0, 2, 0,      // 30
		    0, 0, 0, 0, 5, 8, 0, 6, 0, 0,       // 40
		    9, 0, 0, -2, 0, 0, 0, 0, 0, 0,      // 50
		    -2, 1, 0, 0, 2, -2, 0, 0, 0, 0,     // 60
		    6, 3, 2, 4, 2, 4, 0, 0, 0, 4,       // 70
		    0, -2, 0, 0, 7, 2, 1, 6, 6, 0,      // 80
		    0, 0, 0, 0, 0, 0, 0, 2, 0, 1,       // 90
		    2, 2, 0, 1, -1, 4, 1, 0, 1, 0,      // 100
		    1, 1, 1, 1, 2, 1, 0, 15, 0, 0,      // 110
		    0, 4, 4, -1, 9, 0, -2, 1, 0, 0,     // 120
		    -1, 0, 0, 0, 9, 0, 0, 0, 0, 0,      // 130
		    3, 10, 2, 0, 0, 0, 0, 14, 0, 0,     // 140
		    0, 4, 5, 3, 0, 0, 3, 0, 0, 0,       // 150
		    4, 5, 0, 0, 2, 0, 6, 0, 0, 0,       // 160
		    0, 3, -2, -2, 5, 5, 10, 6, 5, -2,   // 170
		    0, 0, 0, 0, 0, 2, 0, -1, 0, 0,      // 180
		    0, 0, 0, 0, 0, 2, -1, 0, -1, 0,     // 190
		    4, 0, 0, 0, 0, 0, 3, 0, 2, 0,       // 200
		    0, 0, 0, 0, -1, 7, 0, -2, 2, 0,     // 210
		    0, 1, -2, -2, 0, 0, 0, 0, 0, 0,     // 220
		    8, 0, 0, 0, 0, 0, 0, 0, 0, 0,       // 230
		    2, -2, 0, 0, -1, 0, 6, 0, 4, 3,     // 240
		    -1, 0, 0, -1, 6, 0, 0               // 250 
        };

        /// <summary>
        /// The RSA exponent to use for login packet encryption.
        /// </summary>
        public static string LoginRsaExp = "141038977654242498796653256463581947707085475448374831324884224283104317501838296020488428503639086635001378639378416098546218003298341019473053164624088381038791532123008519201622098961063764779454144079550558844578144888226959180389428577531353862575582264379889305154355721898818709924743716570464556076517";
        /// <summary>
        /// The RSA mod to use for login packet encryption.
        /// </summary>
        public static string LoginRsaMod = "65537";
        /// <summary>
        /// The server to client key to use for encrypting packets.
        /// </summary>
        public static string ServerToClientRc4Key = "knQWDQWN23r@R#(K*@#FJ@#";
        /// <summary>
        /// The client to server key to use for encrypting packets.
        /// </summary>
        public static string ClientToServerRc4Key = "a]];'.23r@#R@#fs-ipDFSFSD";

        /// <summary>
        /// The number of skills.
        /// </summary>
        public const int SkillCount = 30;

        /// <summary>
        /// The state of each skill.
        /// </summary>
        public static readonly bool[] SkillEnabled = { true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, true, false, false, false, false, false, false, false, false, false };

        /// <summary>
        /// The name of each skill.
        /// </summary>
        public static readonly string[] SkillName = { "attack", "defence", "strength", "hitpoints", "ranged", "prayer", "magic", "cooking", "woodcutting", "fletching", "fishing", "firemaking", "crafting", "smithing", "mining", "herblore", "agility", "thieving", "slayer", "farming", "runecraft", "-unused-", "-unused-", "-unused-", "-unused-", "-unused-", "-unused-", "-unused-", "-unused-", "-unused-" };

        /// <summary>
        /// The amount of XP to reach a certain level in any stat.
        /// </summary>
        public static readonly int[] SkillXPTable = new int[99];

        /// <summary>
        /// Deltas for assisting in movement updates in the x coordinate direction.
        /// </summary>
        public static sbyte[] DirectionDeltaX = { -1, 0, 1, -1, 1, -1, 0, 1 };

        /// <summary>
        /// Deltas for assisting in movement updates in the y coordinate direction.
        /// </summary>
        public static sbyte[] DirectionDeltaY = { 1, 1, 1, 0, 0, -1, -1, -1 };

        /// <summary>
        /// Mappings that specify how to generate overlay tiles.
        /// </summary>
        public static int[][] OverlayClippingFlags;

        /// <summary>
        /// Mappings that specify how to rotate overlay tiles.
        /// </summary>
        public static byte[][] OverlapClippingPath;

        static GameConstants()
        {
            int k = 0;
            for (int level = 0; level < 99; level++)
            {
                int realLevel = level + 1;
                int j = (int)(realLevel + 300D * Math.Pow(2D, realLevel / 7D));
                k += j;
                SkillXPTable[level] = k / 4;
            }

            OverlayClippingFlags = new int[13][];
            OverlayClippingFlags[0] = new int[] { 1, 3, 5, 7 };
            OverlayClippingFlags[1] = new int[] { 1, 3, 5, 7 };
            OverlayClippingFlags[2] = new int[] { 1, 3, 5, 7 };
            OverlayClippingFlags[3] = new int[] { 1, 3, 5, 7, 6 };
            OverlayClippingFlags[4] = new int[] { 1, 3, 5, 7, 6 };
            OverlayClippingFlags[5] = new int[] { 1, 3, 5, 7, 6 };
            OverlayClippingFlags[6] = new int[] { 1, 3, 5, 7, 6 };
            OverlayClippingFlags[7] = new int[] { 1, 3, 5, 7, 2, 6 };
            OverlayClippingFlags[8] = new int[] { 1, 3, 5, 7, 2, 8 };
            OverlayClippingFlags[9] = new int[] { 1, 3, 5, 7, 2, 8 };
            OverlayClippingFlags[10] = new int[] { 1, 3, 5, 7, 11, 12 };
            OverlayClippingFlags[11] = new int[] { 1, 3, 5, 7, 11, 12 };
            OverlayClippingFlags[12] = new int[] { 1, 3, 5, 7, 13, 14 };

            OverlapClippingPath = new byte[13][];
            OverlapClippingPath[0] = new byte[] {
                0, 1, 2, 3,
                0, 0, 1, 3
            };
            OverlapClippingPath[1] = new byte[]{
                1, 1, 2, 3,
                1, 0, 1, 3
            };
            OverlapClippingPath[2] = new byte[]{
                0, 1, 2, 3,
                1, 0, 1, 3
            };
            OverlapClippingPath[3] = new byte[]{
                0, 0, 1, 2,
                0, 0, 2, 4,
                1, 0, 4, 3
            };
            OverlapClippingPath[4] = new byte[]{
                0, 0, 1, 4,
                0, 0, 4, 3,
                1, 1, 2, 4
            };
            OverlapClippingPath[5] = new byte[]{
                0, 0, 4, 3,
                1, 0, 1, 2,
                1, 0, 2, 4
            };
            OverlapClippingPath[6] = new byte[]{
                0, 1, 2, 4,
                1, 0, 1, 4,
                1, 0, 4, 3
            };
            OverlapClippingPath[7] = new byte[]{
                0, 4, 1, 2,
                0, 4, 2, 5,
                1, 0, 4, 5,
                1, 0, 5, 3
            };
            OverlapClippingPath[8] = new byte[]{
                0, 4, 1, 2,
                0, 4, 2, 3,
                0, 4, 3, 5,
                1, 0, 4, 5
            };
            OverlapClippingPath[9] = new byte[]{
                0, 0, 4, 5,
                1, 4, 1, 2,
                1, 4, 2, 3,
                1, 4, 3, 5
            };
            OverlapClippingPath[10] = new byte[]{
                0, 0, 1, 5,
                0, 1, 4, 5,
                0, 1, 2, 4,
                1, 0, 5, 3,
                1, 5, 4, 3,
                1, 4, 2, 3
            };
            OverlapClippingPath[11] = new byte[]{
                1, 0, 1, 5,
                1, 1, 4, 5,
                1, 1, 2, 4,
                0, 0, 5, 3,
                0, 5, 4, 3,
                0, 4, 2, 3
            };
            OverlapClippingPath[12] = new byte[]{
                1, 0, 5, 4,
                1, 0, 1, 5,
                0, 0, 4, 3,
                0, 4, 5, 3,
                0, 5, 2, 3,
                0, 1, 2, 5
            };
        }

        /// <summary>
        /// Mask an integer.
        /// 
        /// Used for hiding values in memory.
        /// </summary>
        /// <param name="i">The value to mask.</param>
        /// <returns>The masked values.</returns>
        public static int MaskInt(int i)
        {
            return i ^ IntEncryptKey;
        }

        /// <summary>
        /// Scales an integer point to the renderer scale value.
        /// </summary>
        /// <param name="i">The point to scale.</param>
        /// <returns>The scaled point.</returns>
        public static float RScale(int i)
        {
            return i * RenderScale;
        }

        /// <summary>
        /// Scales a float point to the renderer scale value.
        /// </summary>
        /// <param name="f">The point to scale.</param>
        /// <returns>The scaled point.</returns>
        public static float RScale(float f)
        {
            return f * RenderScale;
        }

        /// <summary>
        /// Unscales a float point from the renderer scale value.
        /// </summary>
        /// <param name="f">The point to unscale.</param>
        /// <returns>The unscaled point.</returns>
        public static int Unscale(float f)
        {
            return (int)(f / RenderScale);
        }

        /// <summary>
        /// Unscales a point from a 3D coordinate, and converts it to a RS tile coordinate.
        /// </summary>
        /// <param name="f">The coordinate to unscale, and convert.</param>
        /// <returns>The tile coordinate.</returns>
        public static int UnscaleToTile(float f)
        {
            return Unscale(f) / IntSceneTileSize;
        }
    }

}
