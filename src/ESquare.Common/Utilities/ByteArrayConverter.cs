using System.Collections.Generic;

namespace ESquare.Common.Utilities
{
    /// <summary>
    /// Converts byte array to and from string
    /// </summary>
    /// <remarks>
    /// http://stackoverflow.com/questions/311165/how-do-you-convert-byte-array-to-hexadecimal-string-and-vice-versa/24343727#24343727
    /// </remarks>
    public static class ByteArrayConverter
    {
        private static readonly uint[] _stringIndex = CreateStringLookup();

        private static Dictionary<string, byte> _hexIndex = CreateHexLookup();

        private static uint[] CreateStringLookup()
        {
            var result = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                string s = i.ToString("X2");
                result[i] = ((uint)s[0]) + ((uint)s[1] << 16);
            }
            return result;
        }

        private static Dictionary<string, byte> CreateHexLookup()
        {
            Dictionary<string, byte> hexindex = new Dictionary<string, byte>();
            for (byte i = 0; i < 255; i++)
            {
                hexindex.Add(i.ToString("X2"), i);
            }
            // Adding missing value FF which was causing random errors.
            hexindex.Add("FF", 255);
            return hexindex;
        }

        /// <summary>
        /// Convert byte array into a string
        /// </summary>
        /// <param name="bytes">Array of bytes to convert</param>
        /// <returns>String of hex values. For example: <c>000102</c> for [0,1,2]</returns>
        public static string ToString(byte[] bytes)
        {
            if (bytes == null)
            { return string.Empty;  }

            char[] result = new char[bytes.Length * 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                var val = _stringIndex[bytes[i]];
                result[2 * i] = (char)val;
                result[2 * i + 1] = (char)(val >> 16);
            }
            return new string(result);
        }

        /// <summary>
        /// Converts a hex string back into a byte array
        /// </summary>
        /// <param name="str">String to convert</param>
        /// <returns>Bytes. For example: [0,1,2] from <c>000102</c></returns>
        /// ERROR: Missing FF in dictionary
        public static byte[] FromString(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            { return null; }

            List<byte> hexres = new List<byte>();
            for (int i = 0; i < str.Length; i += 2)
            {
                hexres.Add(_hexIndex[str.Substring(i, 2)]);
            }
            return hexres.ToArray();
        }

    }
}
