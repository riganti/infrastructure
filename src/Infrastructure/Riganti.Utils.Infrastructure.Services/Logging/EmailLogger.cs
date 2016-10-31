using System;
using System.Collections.Generic;
using System.Net;
using Riganti.Utils.Infrastructure.Core;
using Riganti.Utils.Infrastructure.Services.Mailing;

namespace Riganti.Utils.Infrastructure.Services.Logging
{
    public class EmailLogger : LoggerBase
    {
        private readonly string recipientAddress;
        private readonly MailerService mailerService;

        public EmailLogger(string recipientAddress, MailerService mailerService, IDateTimeNowProvider dateTimeNowProvider, IEnumerable<IAdditionalDataProvider> additionalDataProviders = null) : base(dateTimeNowProvider, additionalDataProviders)
        {
            this.recipientAddress = recipientAddress;
            this.mailerService = mailerService;
            MinimumSeverity = Severity.Error;
        }

        protected override void LogMessageCore(string message, IDictionary<string, string> additionalData, Severity severity)
        {
            var output = WebUtility.HtmlEncode(message).Replace("\r\n", "<br />").Replace("\r", "<br />").Replace("\n", "<br />");
            output = $"{dateTimeNowProvider.Now:yyyy-MM-dd HH:mm:ss}<br />" + output; 

            mailerService.SendMail(recipientAddress, "Error Report", output);
        }
    }
}