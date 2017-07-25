using System;
using System.Configuration;
using System.Globalization;
using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace Riganti.Utils.Infrastructure.Core
{
    public class AppSettingsConfigurationBase
    {

        private static readonly CultureInfo culture = new CultureInfo("en-US");

        public static string GetString([CallerMemberName] string key = null)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return ConfigurationManager.AppSettings[key];
        }

        public static bool? GetBoolean([CallerMemberName] string key = null)
        {
            var value = GetString(key);
            if (!string.IsNullOrEmpty(value))
            {
                return bool.Parse(value);
            }
            return null;
        }

        public static Guid? GetGuid([CallerMemberName] string key = null)
        {
            var value = GetString(key);
            if (!string.IsNullOrEmpty(value))
            {
                return Guid.Parse(value);
            }
            return null;
        }

        public static byte? GetByte([CallerMemberName] string key = null)
        {
            var value = GetString(key);
            if (!string.IsNullOrEmpty(value))
            {
                return byte.Parse(value, culture);
            }
            return null;
        }

        public static short? GetShort([CallerMemberName] string key = null)
        {
            var value = GetString(key);
            if (!string.IsNullOrEmpty(value))
            {
                return short.Parse(value, culture);
            }
            return null;
        }

        public static int? GetInt([CallerMemberName] string key = null)
        {
            var value = GetString(key);
            if (!string.IsNullOrEmpty(value))
            {
                return int.Parse(value, culture);
            }
            return null;
        }

        public static long? GetLong([CallerMemberName] string key = null)
        {
            var value = GetString(key);
            if (!string.IsNullOrEmpty(value))
            {
                return long.Parse(value, culture);
            }
            return null;
        }

        public static double? GetDouble([CallerMemberName] string key = null)
        {
            var value = GetString(key);
            if (!string.IsNullOrEmpty(value))
            {
                return double.Parse(value, culture);
            }
            return null;
        }

        public static decimal? GetDecimal([CallerMemberName] string key = null)
        {
            var value = GetString(key);
            if (!string.IsNullOrEmpty(value))
            {
                return decimal.Parse(value, culture);
            }
            return null;
        }
        public static TimeSpan? GetTimeSpan([CallerMemberName] string key = null)
        {
            var value = GetString(key);
            if (!string.IsNullOrEmpty(value))
            {
                return TimeSpan.Parse(value, culture);
            }
            return null;
        }

        public static DateTime? GetDateTime([CallerMemberName] string key = null)
        {
            var value = GetString(key);
            if (!string.IsNullOrEmpty(value))
            {
                return DateTime.Parse(value, culture);
            }
            return null;
        }
    }
}
