using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Services.Logging
{
    /// <summary>
    /// A base class for all logging mechanisms.
    /// </summary>
    public abstract class LoggerBase : ILogger
    {
        private readonly List<IAdditionalDataProvider> additionalDataProviders = new List<IAdditionalDataProvider>();

        protected readonly IDateTimeNowProvider dateTimeNowProvider;

        /// <summary>
        /// Gets or sets the minimum severity of messages that are not ignored.
        /// </summary>
        public Severity MinimumSeverity { get; set; } = Severity.Info;

        /// <summary>
        /// Gets or sets the mechanism that formats the exception messages.
        /// </summary>
        public IExceptionFormatter ExceptionFormatter { get; set; } = new DefaultExceptionFormatter();


        public LoggerBase(IDateTimeNowProvider dateTimeNowProvider, IEnumerable<IAdditionalDataProvider> additionalDataProviders = null)
        {
            this.dateTimeNowProvider = dateTimeNowProvider;
            if (additionalDataProviders != null)
            {
                this.additionalDataProviders.AddRange(additionalDataProviders);
            }
        }


        public void LogException(Exception exception, IDictionary<string, string> additionalData = null, Severity severity = Severity.Error)
        {
            try
            {
                if (severity < MinimumSeverity) return;
                ExtractAdditionalData(ref additionalData);

                LogExceptionCore(exception, additionalData, severity);
            }
            catch (Exception ex)
            {
                // the logging code should not throw any exceptions
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
            }
        }
        
        public void LogMessage(string message, IDictionary<string, string> additionalData = null, Severity severity = Severity.Info)
        {
            try
            {
                if (severity < MinimumSeverity) return;
                ExtractAdditionalData(ref additionalData);

                LogMessageCore(message, additionalData, severity);
            }
            catch (Exception ex)
            {
                // the logging code should not throw any exceptions
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
            }
        }

        protected virtual void LogExceptionCore(Exception exception, IDictionary<string, string> additionalData, Severity severity)
        {
            var message = ExceptionFormatter.FormatException(exception);
            LogMessageCore(message, additionalData, severity);
        }


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