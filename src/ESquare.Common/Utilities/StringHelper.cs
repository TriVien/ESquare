using System.Linq;

namespace ESquare.Common.Utilities
{
    /// <summary>
    /// Helper for string type
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Returns the first string in an array that is not empty or null. If not found, null is returned.
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
        public static string Coalesce(params string[] strings)
        {
            return strings.FirstOrDefault(s => !string.IsNullOrEmpty(s));
        }

        /// <summary>
        /// Encodes a string for use in SQL query
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static string EncodeStringForSql(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
            {
                return "''";
            }

            return "'" + sql.Replace("'", "''") + "'";
        }
    }
}