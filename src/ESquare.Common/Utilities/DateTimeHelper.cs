using System;

namespace ESquare.Common.Utilities
{
    /// <summary>
    /// Helper for DateTime type
    /// </summary>
    public static class DateTimeHelper
    {

        /// <summary>
        /// Clamps the given date time value to be within the interval given by lowerBound
        /// and upperBound.
        /// </summary>
        /// <param name="value">Value to clamp.</param>
        /// <param name="min">Minimum allowed value.</param>
        /// <param name="max">Maximum allowed value.</param>
        /// <returns></returns>
        public static DateTime? Clamp(DateTime? value, DateTime? min, DateTime? max)
        {
            if (value == null) {
                return null;
            }

            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }
    }
}