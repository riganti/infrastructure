using System;
using System.Globalization;

namespace Riganti.Utils.Infrastructure.Core
{
    /// <summary>
    /// Contains various helper methods.
    /// </summary>
    public static class RigantiNullableHelpers
    {

        /// <summary>
        /// Convers the string to int or returns null if the string is not valid.
        /// </summary>
        public static int? ToNullableInt(this string value)
        {
            int number;
            return int.TryParse(value, out number) ? number : (int?) null;
        }

        /// <summary>
        /// Convers the string to long or returns null if the string is not valid.
        /// </summary>
        public static long? ToNullableLong(this string value)
        {
            long number;
            return long.TryParse(value, out number) ? number : (long?)null;
        }

        /// <summary>
        /// Convers the string to float or returns null if the string is not valid.
        /// </summary>
        public static float? ToNullableFloat(this string value)
        {
            float number;
            return float.TryParse(value, out number) ? number : (float?)null;
        }

        /// <summary>
        /// Convers the string to double or returns null if the string is not valid.
        /// </summary>
        public static double? ToNullableDouble(this string value)
        {
            double number;
            return double.TryParse(value, out number) ? number : (double?)null;
        }

        /// <summary>
        /// Convers the string to decimal or returns null if the string is not valid.
        /// </summary>
        public static decimal? ToNullableDecimal(this string value)
        {
            decimal number;
            return decimal.TryParse(value, out number) ? number : (decimal?)null;
        }

        /// <summary>
        /// Convers the string to DateTime or returns null if the string is not valid.
        /// </summary>
        public static DateTime? ToNullableDateTime(this string value)
        {
            DateTime number;
            return DateTime.TryParse(value, out number) ? number : (DateTime?)null;
        }

        /// <summary>
        /// Convers the string to DateTime or returns null if the string is not valid.
        /// </summary>
        public static DateTime? ToNullableDateTime(this string value, string format, IFormatProvider formatProvider, DateTimeStyles styles = DateTimeStyles.None)
        {
            DateTime number;
            return DateTime.TryParseExact(value, format, formatProvider, styles, out number) ? number : (DateTime?)null;
        }

        /// <summary>
        /// Convers the string to DateTime or returns null if the string is not valid.
        /// </summary>
        public static DateTime? ToNullableDateTime(this string value, string[] formats, IFormatProvider formatProvider, DateTimeStyles styles = DateTimeStyles.None)
        {
            DateTime number;
            return DateTime.TryParseExact(value, formats, formatProvider, styles, out number) ? number : (DateTime?)null;
        }

    }
}
