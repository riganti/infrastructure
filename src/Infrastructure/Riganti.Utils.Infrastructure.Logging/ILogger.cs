using System;
using System.Collections.Generic;

namespace Riganti.Utils.Infrastructure.Logging
{
    public interface ILogger
    {

        void LogException(Exception exception, IDictionary<string, string> additionalData = null, Severity severity = Severity.Error);

        void LogMessage(string message, IDictionary<string, string> additionalData = null, Severity severity = Severity.Info);

    }
}
