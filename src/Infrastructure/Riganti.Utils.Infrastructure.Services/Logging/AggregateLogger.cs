using System;
using System.Collections.Generic;

namespace Riganti.Utils.Infrastructure.Services.Logging
{
    public class AggregateLogger : ILogger 
    {
        private readonly ILogger[] loggers;

        public AggregateLogger(params ILogger[] loggers)
        {
            this.loggers = loggers;
        }

        public void LogException(Exception exception, IDictionary<string, string> additionalData = null, Severity severity = Severity.Error)
        {
            foreach (var logger in loggers)
            {
                logger.LogException(exception, additionalData, severity);
            }
        }

        public void LogMessage(string message, IDictionary<string, string> additionalData = null, Severity severity = Severity.Info)
        {
            foreach (var logger in loggers)
            {
                logger.LogMessage(message, additionalData, severity);
            }
        }
    }
}
