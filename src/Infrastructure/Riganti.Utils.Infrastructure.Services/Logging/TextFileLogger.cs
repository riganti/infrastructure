using System.Collections.Generic;
using System.IO;
using System.Text;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Services.Logging
{
    public class TextFileLogger : LoggerBase
    {
        private readonly string directory;


        public TextFileLogger(string directory, IDateTimeProvider dateTimeProvider, IEnumerable<IAdditionalDataProvider> additionalDataProviders = null) : base(dateTimeProvider, additionalDataProviders)
        {
            this.directory = directory;

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        protected virtual string GetLogFileName()
        {
            return Path.Combine(directory, dateTimeProvider.Now.ToString("yyyy-MM-dd") + ".txt");
        }

        protected override void LogMessageCore(string message, IDictionary<string, string> additionalData, Severity severity)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{dateTimeProvider.Now:yyyy-MM-dd HH:mm:ss}\t{message}");
            if (additionalData.Count > 0)
            {
                sb.AppendLine("Additional data:");
                foreach (var data in additionalData)
                {
                    sb.AppendLine($"{data.Key,20}{data.Value}");
                }
            }
            sb.AppendLine();
            
            File.AppendAllText(GetLogFileName(), sb.ToString(), Encoding.UTF8);
        }
    }
}
