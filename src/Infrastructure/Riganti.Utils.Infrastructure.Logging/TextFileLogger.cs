using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Logging
{
    public class TextFileLogger : LoggerBase
    {
        private readonly string directory;
        private readonly string extension;
        private readonly Encoding fileEncoding = new UTF8Encoding(false);
        private readonly string fileNameWithoutExtension;
        private readonly object fileWriteLock = new object();

        public string RollingTimeFormat { get; set; } = "yyyy-MM-dd";

        public TextFileLogger(string filePath, IDateTimeProvider dateTimeProvider, IEnumerable<IAdditionalDataProvider> additionalDataProviders = null)
            : base(dateTimeProvider, additionalDataProviders)
        {
            directory = Path.GetDirectoryName(filePath);
            fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            extension = Path.GetExtension(filePath);

            if (!String.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        protected virtual string GetLogFileName()
        {
            var fileName = $"{fileNameWithoutExtension}_{DateTimeProvider.Now.ToString(RollingTimeFormat)}{extension}";
            return Path.Combine(directory, fileName);
        }

        protected override void LogMessageCore(string message, IDictionary<string, string> additionalData, Severity severity)
        {
            var sb = new StringBuilder();
            MessageFormatter.Format(message, additionalData, severity, sb);
            sb.AppendLine();

            // TODO: temporary workaround
            lock (fileWriteLock)
            {
                File.AppendAllText(GetLogFileName(), sb.ToString(), fileEncoding);
            }
        }
    }
}