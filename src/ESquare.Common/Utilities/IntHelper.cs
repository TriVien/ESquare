namespace ESquare.Common.Utilities
{
    /// <summary>
    /// Helper for int type
    /// </summary>
    public static class IntHelper
    {
        /// <summary>
        /// Try to parse from string to int. If errors, return default value
        /// </summary>
        /// <param name="str">string to be parsed</param>
        /// <param name="defaultValue">default value</param>
        /// <returns>Return int value which parsed from string</returns>
        public static int Parse(string str, int defaultValue)
        {
            int result;
            if (!int.TryParse(str, out result))
            {
                result = defaultValue;
            }
            return result;
        }
    }
}