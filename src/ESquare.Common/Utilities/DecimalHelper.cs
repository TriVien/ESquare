using System;

namespace ESquare.Common.Utilities
{
    /// <summary>
    /// Helper for decimal type
    /// </summary>
    public static class DecimalHelper
    {
        /// <summary>
        /// Try to parse from string to decimal. If errors, return default value
        /// </summary>
        /// <param name="str">string to be parsed</param>
        /// <param name="defaultValue">default value</param>
        /// <returns>Return decimal value which parsed from string</returns>
        public static decimal Parse(string str, decimal defaultValue)
        {
            decimal result;
            if (!decimal.TryParse(str, out result))
            {
                result = defaultValue;
            }
            return result;
        }

        /// <summary>
        /// Rounds a number to 2 decimal points.
        /// </summary>
        /// <param name="number">Number to round.</param>
        /// <returns></returns>
        public static decimal? Round(decimal? number)
        {
            decimal rounded = 0;

            if (number.HasValue)
            {
                decimal value = number.Value;

                // If the number is positive then do a simple round.
                if (value >= 0)
                {
                    rounded = Math.Round(value, 2);
                }
                else// Round away from zero.
                {
                    value = Math.Abs(value);
                    value = Math.Round(value, 2);
                    rounded = 0 - value;
                }
            }
            return rounded;
        }
    }
}