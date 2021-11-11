namespace Kzrnm.Convert.Base32768
{
    internal static class MathUtil
    {
        /// <summary>
        /// Returns the smallest multiple of <paramref name="n"/> greater than or equal to <paramref name="value"/>.
        /// </summary>
        public static int CeilingNth(int value, int n)
        {
            var remainder = value % n;
            if (remainder == 0)
                return value;
            return value + n - remainder;
        }
        /// <summary>
        /// Returns the largest multiple of <paramref name="n"/> less than or equal to <paramref name="value"/>.
        /// </summary>
        public static int FloorNth(int value, int n)
        {
            var remainder = value % n;
            return value - remainder;
        }
    }
}
