using System;
using System.Text.RegularExpressions;

namespace ESquare.Common.Utilities
{
    /// <summary>
    /// Contains methods use for validations
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Check whether decimal digits is less than or equals to 2
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>True if decimal digits is less than or equals to 2</returns>
        public static bool IsValidCurrency(decimal? value)
        {
            if (!value.HasValue || !value.ToString().Contains("."))
            {
                return true;
            }

            string valueString = value.ToString();

            // Strip trailing zeros. These are sometimes added by ToString().
            while (valueString.EndsWith("0"))
            {
                valueString = valueString.Remove(valueString.Length - 1);
            }

            int decimalCount = valueString.Substring(valueString.IndexOf(".")).Length - 1;
            return decimalCount <= 2;
        }

        /// <summary>
        /// Test if the provided string is a valid date
        /// </summary>
        /// <param name="value">Date string to test</param>
        /// <returns>bool</returns>
        public static bool IsValidDate(string value)
        {
            DateTime date;
            return DateTime.TryParse(value, out date);
        }

        /// <summary>
        /// Test if the provided string is a valid website url
        /// </summary>
        /// <param name="urlString">Url string to test</param>
        /// <param name="requiredProtocol">Required protocol path in URL.</param>
        /// <returns></returns>
        public static bool IsValidWebsiteUrl(string urlString, bool requiredProtocol)
        {
            Regex regex = requiredProtocol
                ? new Regex(@"^(https?:\/\/)([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$")
                : new Regex(@"^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$");

            Match match = regex.Match(urlString);
            return match.Success;
        }
    }
}
