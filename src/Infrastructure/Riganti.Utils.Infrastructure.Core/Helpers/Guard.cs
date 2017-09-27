using System;
using System.Text.RegularExpressions;

namespace Riganti.Utils.Infrastructure.Core
{
    public static class Guard
    {
        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> if the given argument is null.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value is null.</exception>
        /// <param name="argumentValue">The argument value to test.</param>
        /// <param name="argumentName">The name of the argument to test.</param>
        public static void ArgumentNotNull(object argumentValue, string argumentName)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        /// <summary>
        /// Throws an exception if the tested string argument is null or an empty string.
        /// </summary>
        /// <exception cref="ArgumentNullException">The string value is null.</exception>
        /// <exception cref="ArgumentException">The string is empty.</exception>
        /// <param name="argumentValue">The argument value to test.</param>
        /// <param name="argumentName">The name of the argument to test.</param>
        public static void ArgumentNotNullOrWhiteSpace(string argumentValue, string argumentName)
        {
            if (string.IsNullOrWhiteSpace(argumentValue))
            {
                throw new ArgumentException($"{argumentName} cannot be null, empty, or only whitespace.");
            }
        }

        /// <summary>
        /// Throws an exception if the tested string argument is not Base64 string.
        /// </summary>
        /// <exception cref="ArgumentNullException">The string value is null.</exception>
        /// <exception cref="ArgumentException">The string is not Base64 string.</exception>
        /// <param name="argumentValue">The argument value to test.</param>
        /// <param name="argumentName">The name of the argument to test.</param>
        public static void ArgumentIsBase64String(string argumentValue, string argumentName)
        {
            ArgumentNotNullOrWhiteSpace(argumentValue, nameof(argumentName));

            if (argumentValue.Length % 4 != 0 ||
                !Regex.IsMatch(argumentValue, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None))
            {
                throw new ArgumentException($"{argumentName} is not Base64 string.");
            }

        }
    }
}
