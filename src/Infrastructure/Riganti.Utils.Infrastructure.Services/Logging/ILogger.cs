using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riganti.Utils.Infrastructure.Services.Logging
{
    public interface ILogger
    {

        void LogException(Exception exception, IDictionary<string, string> additionalData = null, Severity severity = Severity.Error);

        void LogMessage(string message, IDictionary<string, string> additionalData = null, Severity severity = Severity.Info);

    }
}
