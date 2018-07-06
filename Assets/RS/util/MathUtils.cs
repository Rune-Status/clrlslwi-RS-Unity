using System;

namespace RS
{
    /// <summary>
    /// Provides utilities relating to math.
    /// </summary>
    public static class MathUtils
    {
        /// <summary>
        /// The sin table.
        /// </summary>
        public static readonly int[] Sin;
        /// <summary>
        /// The cos table.
        /// </summary>
        public static readonly int[] Cos;

        static MathUtils()
        {
            Sin = new int[2048];
            Cos = new int[2048];
            for (var i = 0; i < 2048; i++)
            {
                Sin[i] = (int)(65536D * Math.Sin(i * 0.0030679615D));
                Cos[i] = (int)(65536D * Math.Cos(i * 0.0030679615D));
            }
        }
    }
}
