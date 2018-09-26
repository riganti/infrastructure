using System;
using System.Collections.Generic;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Logging
{
    /// <summary>
    /// Logger that aggregates other loggers.
    /// </summary>
    public class AggregateLogger : ILogger 
    {
        private readonly ILogger[] loggers;

        public AggregateLogger(params ILogger[] loggers)
        {
            this.loggers = loggers;
        }

        /// <summary>
        /// Log verbose message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void LogVerbose(string message)
        {
            foreach (var logger in loggers)
            {
                logger.LogVerbose(message);
            }
        }

        /// <summary>
        /// Log info message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void LogInfo(string message)
        {
            foreach (var logger in loggers)
            {
                logger.LogInfo(message);
            }
        }

        /// <summary>
        /// Log warning message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void LogWarning(string message)
        {
            foreach (var logger in loggers)
            {
                logger.LogWarning(message);
            }
        }

        /// <summary>
        /// Log error message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void LogError(string message)
        {
            foreach (var logger in loggers)
            {
                logger.LogError(message);
            }
        }

        /// <summary>
        /// Log critical message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void LogCritical(string message)
        {
            foreach (var logger in loggers)
            {
                logger.LogCritical(message);
            }
        }

        /// <summary>
        /// Log the exception with additional data.
        /// </summary>
        public void LogException(Exception exception, IDictionary<string, string> additionalData = null, Severity severity = Severity.Error)
        {
            foreach (var logger in loggers)
            {
                logger.LogException(exception, additionalData, severity);
            }
        }

        /// <summary>
        /// Log generic message with additional data.
        /// </summary>
        public void LogMessage(string message, IDictionary<string, string> additionalData = null, Severity severity = Severity.Info)
        {
            foreach (var logger in loggers)
            {
                logger.LogMessage(message, additionalData, severity);
            }
        }
    }
}
