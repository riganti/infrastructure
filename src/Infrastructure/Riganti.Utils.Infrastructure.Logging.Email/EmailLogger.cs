using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Riganti.Utils.Infrastructure.Core;
using Riganti.Utils.Infrastructure.Services.Mailing;

// ReSharper disable once CheckNamespace
namespace Riganti.Utils.Infrastructure.Logging
{
    public class EmailLogger : LoggerBase
    {
        private readonly string recipientAddress;
        private readonly MailerService mailerService;

        public EmailLogger(string recipientAddress, MailerService mailerService, IDateTimeProvider dateTimeProvider, IEnumerable<IAdditionalDataProvider> additionalDataProviders = null) : base(dateTimeProvider, additionalDataProviders)
        {
            this.recipientAddress = recipientAddress;
            this.mailerService = mailerService;
            MinimumSeverity = Severity.Error;
        }

        protected override void LogMessageCore(string message, IDictionary<string, string> additionalData, Severity severity)
        {
            var output = WebUtility.HtmlEncode(message).Replace("\r\n", "<br />").Replace("\r", "<br />").Replace("\n", "<br />");
            output = $"{DateTimeProvider.Now:yyyy-MM-dd HH:mm:ss}<br />" + output;

            Task.Run(async () => await mailerService.SendMailAsync(recipientAddress, "Error Report", output));
        }
    }
}