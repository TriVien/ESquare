using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ESquare.Common.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Converts string to enum value (opposite to Enum.ToString()).
        /// </summary>
        /// <typeparam name="T">Type of the enum to convert the string into.</typeparam>
        /// <param name="s">string to convert to enum value.</param>
        public static T ToEnum<T>(this string s) where T : struct
        {
            T newValue;
            return Enum.TryParse(s, out newValue) ? newValue : default(T);
        }

        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr =
                           Attribute.GetCustomAttribute(field,
                             typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }

            return null;
        }
    }
}
