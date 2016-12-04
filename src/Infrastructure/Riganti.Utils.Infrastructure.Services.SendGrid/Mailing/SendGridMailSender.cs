using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Riganti.Utils.Infrastructure.Services.Mailing;
using SendGrid;

namespace Riganti.Utils.Infrastructure.Services.SendGrid.Mailing
{
    public class SendGridMailSender : IMailSender
    {
        private readonly string apiKey;

        public SendGridMailSender(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public Task SendAsync(MailMessageDTO message)
        {
            var web = new Web(apiKey);

            var msg = new SendGridMessage()
            {
                From = ConvertMailAddress(message.From),
                To = message.To.Select(ConvertMailAddress).ToArray(),
                Cc = message.To.Select(ConvertMailAddress).ToArray(),
                Bcc = message.To.Select(ConvertMailAddress).ToArray(),
                ReplyTo = message.To.Select(ConvertMailAddress).ToArray(),
                Subject = message.Subject,
                Html = message.BodyHtml,
                Text = message.BodyText,
                StreamedAttachments = message.Attachments.ToDictionary(a => a.Name, a => GetMemoryStream(a.Stream))
            };

            OnMessageSending(msg);
            return web.DeliverAsync(msg);
        }

        protected virtual void OnMessageSending(SendGridMessage message)
        {
        }

        private MemoryStream GetMemoryStream(Stream stream)
        {
            var ms = new MemoryStream();
            stream.CopyTo(ms);
            ms.Position = 0;
            return ms;
        }

        private MailAddress ConvertMailAddress(MailAddressDTO address)
        {
            if (string.IsNullOrEmpty(address.DisplayName))
            {
                return new MailAddress(address.Address);
            }
            else
            {
                return new MailAddress(address.Address);
            }
        }
    }
}
