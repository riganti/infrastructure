using System;
using System.Collections.Generic;

namespace Riganti.Utils.Infrastructure.Core
{
    public interface ILogger
    {
        /// <summary>
        /// Log verbose message.
        /// </summary>
        /// <param name="message">The message.</param>
        void LogVerbose(string message);

        /// <summary>
        /// Log info message.
        /// </summary>
        /// <param name="message">The message.</param>
        void LogInfo(string message);

        /// <summary>
        /// Log warning message.
        /// </summary>
        /// <param name="message">The message.</param>
        void LogWarning(string message);

        /// <summary>
        /// Log error message.
        /// </summary>
        /// <param name="message">The message.</param>
        void LogError(string message);

        /// <summary>
        /// Log critical message.
        /// </summary>
        /// <param name="message">The message.</param>
        void LogCritical(string message);

        /// <summary>
        /// Log the exception with additional data.
        /// </summary>
        void LogException(Exception exception, IDictionary<string, string> additionalData = null, Severity severity = Severity.Error);

        /// <summary>
        /// Log generic message with additional data.
        /// </summary>
        void LogMessage(string message, IDictionary<string, string> additionalData = null, Severity severity = Severity.Info);
    }
}
