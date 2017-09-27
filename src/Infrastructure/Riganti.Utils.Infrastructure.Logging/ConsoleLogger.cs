using System;
using System.Collections.Generic;
using System.Text;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Logging
{
    /// <summary>
    /// Logger that writes colored messages to the console.
    /// </summary>
    public class ConsoleLogger : LoggerBase
    {
        public ConsoleLogger(IDateTimeProvider dateTimeProvider, IEnumerable<IAdditionalDataProvider> additionalDataProviders = null)
            : base(dateTimeProvider, additionalDataProviders)
        {
        }

        protected override void LogMessageCore(string message, IDictionary<string, string> additionalData, Severity severity)
        {
            var sb = new StringBuilder();
            MessageFormatter.Format(message, additionalData, severity, sb);
            Console.ForegroundColor = GetColor(severity);
            Console.WriteLine(sb.ToString());
        }

        private static ConsoleColor GetColor(Severity severity)
        {
            switch (severity)
            {
                case Severity.Verbose: return ConsoleColor.DarkGray;
                case Severity.Info: return ConsoleColor.Gray;
                case Severity.Warning: return ConsoleColor.Yellow;
                case Severity.Error: return ConsoleColor.Red;
                case Severity.Critical: return ConsoleColor.Red;
                default:
                    // TODO: rewrite to use inner logging
                    throw new ArgumentOutOfRangeException(nameof(severity), severity, null);
            }
        }
    }
}
