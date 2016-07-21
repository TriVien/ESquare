using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESquare.Infrastructure
{
    public class AppSettings
    {
        /// <summary>
        /// SendGrid account
        /// </summary>
        public static readonly string SendGridAccount = GetStringSetting("SendGridAccount", "Test");

        /// <summary>
        /// SendGrid password
        /// </summary>
        public static readonly string SendGridPassword = GetStringSetting("SendGridPassword", "Test");

        /// <summary>
        /// Resource Server (our web API) Id
        /// </summary>
        public static readonly string AudienceId = GetStringSetting("AudienceId", "Test");

        /// <summary>
        /// Resource Server password
        /// </summary>
        public static readonly string AudienceSecret = GetStringSetting("AudienceSecret", "Test");

        public static string GetStringSetting(string name, string defaultValue = "")
        {
            string s = ConfigurationManager.AppSettings[name];
            return string.IsNullOrWhiteSpace(s) ? defaultValue : s;
        }

        public static int GetIntSetting(string name, int defaultValue = 0)
        {
            string s = GetStringSetting(name);
            try
            { return int.Parse(s); }
            catch
            { return defaultValue; }
        }

        public static bool GetBooleanSetting(string name, bool defaultValue = false)
        {
            try
            {
                string s = GetStringSetting(name);
                return s.ToLower() == "true";
            }
            catch
            { return defaultValue; }
        }

        public static TimeSpan GetTimeSpanSetting(string name, TimeSpan defaultValue)
        {
            try
            {
                string s = GetStringSetting(name);
                return TimeSpan.Parse(s);
            }
            catch
            { return defaultValue; }
        }
    }
}
