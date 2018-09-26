using System;
using System.Collections.Generic;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Logging
{
    /// <summary>
    /// Logger that does nothing with logged message.
    /// </summary>
    public class EmptyLogger : ILogger
    {
        public void LogVerbose(string message)
        {
            // do nothing
        }
        
        public void LogInfo(string message)
        {
            // do nothing
        }

        public void LogWarning(string message)
        {
            // do nothing
        }
        
        public void LogError(string message)
        {
            // do nothing
        }
        
        public void LogCritical(string message)
        {
            // do nothing
        }

        public void LogException(Exception exception, IDictionary<string, string> additionalData = null, Severity severity = Severity.Error)
        {
            // do nothing
        }

        public void LogMessage(string message, IDictionary<string, string> additionalData = null, Severity severity = Severity.Info)
        {
            // do nothing
        }
    }
}
