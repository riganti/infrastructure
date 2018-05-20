using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Riganti.Utils.Infrastructure.Services.Mailing;
using SendGrid;
using SendGrid.Helpers.Mail;

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
            var web = new SendGridClient(apiKey);

            var msg = new SendGridMessage()
            {
                From = ConvertMailAddress(message.From),
                ReplyTo = message.ReplyTo.Select(ConvertMailAddress).FirstOrDefault(),
                Subject = message.Subject
            };

            msg.AddTos(message.To.Select(ConvertMailAddress).ToList());

            if (message.Cc.Count > 0)
            {
                msg.AddCcs(message.Cc.Select(ConvertMailAddress).ToList());
            }

            if (message.Bcc.Count > 0)
            {
                msg.AddBccs(message.Bcc.Select(ConvertMailAddress).ToList());
            }

            if (message.Attachments.Count > 0)
            {
                msg.AddAttachments(message.Attachments.Select(ConvertAttachment).ToList());
            }

            if (string.IsNullOrWhiteSpace(message.BodyHtml))
            {
                msg.PlainTextContent = message.BodyText;
            }
            else
            {
                msg.HtmlContent = message.BodyHtml;
            }

            ProcessTemplate(msg, message.Template);

            OnMessageSending(msg);
            return web.SendEmailAsync(msg);
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

        private void ProcessTemplate(SendGridMessage message, ITemplate template)
        {
            if (template != null)
            {
                message.SetTemplateId(template.TemplateId);
                foreach (var substitution in template.Substitution)
                {
                    message.AddSubstitution(substitution.Key, substitution.Value.ToString());
                }
            }
        }

        private Attachment ConvertAttachment(AttachmentDTO attachmentDTO)
        {
            return new Attachment()
            {
                Filename = attachmentDTO.Name,
                Content = Convert.ToBase64String(GetMemoryStream(attachmentDTO.Stream).ToArray()),
                Type = attachmentDTO.MimeType
            };
        }

        private EmailAddress ConvertMailAddress(MailAddressDTO address)
        {
            if (string.IsNullOrEmpty(address.DisplayName))
            {
                return new EmailAddress(address.Address);
            }
            else
            {
                return new EmailAddress(address.Address, address.DisplayName);
            }
        }
    }
}
