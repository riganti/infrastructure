using System;
using System.Collections.Generic;
using System.Diagnostics;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Logging
{
    /// <summary>
    /// A base class for all logging mechanisms.
    /// </summary>
    public abstract class LoggerBase : ILogger
    {
        private readonly List<IAdditionalDataProvider> additionalDataProviders = new List<IAdditionalDataProvider>();

        /// <summary>
        /// Gets the provider of current time.
        /// </summary>
        protected IDateTimeProvider DateTimeProvider { get; }

        /// <summary>
        /// Gets or sets the minimum severity of messages that are not ignored (default <see cref="Severity.Info"/>).
        /// </summary>
        public Severity MinimumSeverity { get; set; } = Severity.Info;

        /// <summary>
        /// Gets or sets the mechanism that formats the message
        /// (<see cref="DefaultMessageFormatter"/> by default).
        /// </summary>
        public IMessageFormatter MessageFormatter { get; set; }

        /// <summary>
        /// Gets or sets the mechanism that formats the exception
        /// (<see cref="DefaultExceptionFormatter"/> by default).
        /// </summary>
        public IExceptionFormatter ExceptionFormatter { get; set; } = new DefaultExceptionFormatter();

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerBase" /> class with defined
        /// <paramref name="dateTimeProvider"/> and <see cref="DefaultMessageFormatter"/>.
        /// </summary>
        protected LoggerBase(IDateTimeProvider dateTimeProvider, IEnumerable<IAdditionalDataProvider> additionalDataProviders = null)
        {
            DateTimeProvider = dateTimeProvider;
            MessageFormatter = new DefaultMessageFormatter(dateTimeProvider);
            if (additionalDataProviders != null)
            {
                this.additionalDataProviders.AddRange(additionalDataProviders);
            }
        }

        /// <summary>
        /// Log verbose message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void LogVerbose(string message)
        {
            LogMessage(message, severity: Severity.Verbose);
        }

        /// <summary>
        /// Log info message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void LogInfo(string message)
        {
            // ReSharper disable once RedundantArgumentDefaultValue
            LogMessage(message, severity: Severity.Info);
        }

        /// <summary>
        /// Log warning message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void LogWarning(string message)
        {
            LogMessage(message, severity: Severity.Warning);
        }

        /// <summary>
        /// Log error message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void LogError(string message)
        {
            LogMessage(message, severity: Severity.Error);
        }

        /// <summary>
        /// Log critical message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void LogCritical(string message)
        {
            LogMessage(message, severity: Severity.Critical);
        }

        /// <summary>
        /// Log the exception with additional data.
        /// </summary>
        public void LogException(Exception exception, IDictionary<string, string> additionalData = null, Severity severity = Severity.Error)
        {
            try
            {
                if (severity < MinimumSeverity) return;
                ExtractAdditionalData(ref additionalData);

                LogExceptionCore(exception, additionalData, severity);
            }
            catch (Exception)
            {
                // the logging code should not throw any exceptions
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
            }
        }

        /// <summary>
        /// Log generic message with additional data.
        /// </summary>
        public void LogMessage(string message, IDictionary<string, string> additionalData = null, Severity severity = Severity.Info)
        {
            try
            {
                if (severity < MinimumSeverity) return;
                ExtractAdditionalData(ref additionalData);

                LogMessageCore(message, additionalData, severity);
            }
            catch (Exception)
            {
                // the logging code should not throw any exceptions
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
            }
        }

        /// <summary>
        /// Internal implementation of how the exception is logged.
        /// </summary>
        protected virtual void LogExceptionCore(Exception exception, IDictionary<string, string> additionalData, Severity severity)
        {
            var message = ExceptionFormatter.FormatException(exception);
            LogMessageCore(message, additionalData, severity);
        }

        /// <summary>
        /// Internal implementation of how the message is logged.
        /// </summary>
        protected abstract void LogMessageCore(string message, IDictionary<string, string> additionalData, Severity severity);

        private void ExtractAdditionalData(ref IDictionary<string, string> additionalData)
        {
            if (additionalData == null)
            {
                additionalData = new Dictionary<string, string>();
            }

            foreach (var provider in additionalDataProviders)
            {
                provider.ExtractAdditinalData(additionalData);
            }
        }

    }
}