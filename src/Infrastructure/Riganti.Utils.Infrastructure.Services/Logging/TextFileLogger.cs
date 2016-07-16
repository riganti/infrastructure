using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riganti.Utils.Infrastructure.Services.Logging
{
    public class TextFileLogger : LoggerBase
    {
        private readonly string directory;

        public TextFileLogger(string directory, IEnumerable<IAdditionalDataProvider> additionalDataProviders = null) : base(additionalDataProviders)
        {
            this.directory = directory;

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        protected virtual string GetLogFileName()
        {
            return Path.Combine(directory, DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
        }

        protected override void LogMessageCore(string message, IDictionary<string, string> additionalData, Severity severity)
        {
            var output = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\t{message}\r\n\r\n";
            File.AppendAllText(GetLogFileName(), output, Encoding.UTF8);
        }
    }
}
