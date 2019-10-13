using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace Riganti.Utils.Infrastructure.Services.Mailing
{
    public static class Extensions
    {

        public static MailMessage ToMailMessage(this MailMessageDTO message)
        {
            var mailMessage = new MailMessage()
            {
                Sender = message.Sender.ToMailAddress(),
                Subject = message.Subject
            };

            // set body
            if (message.BodyHtml != null)
            {
                mailMessage.Body = message.BodyHtml;
                mailMessage.IsBodyHtml = true;

                if (message.BodyText != null)
                {
                    var plainTextView = AlternateView.CreateAlternateViewFromString(message.BodyText, null, "text/plain");
                    mailMessage.AlternateViews.Add(plainTextView);
                }
            }
            else
            {
                mailMessage.Body = message.BodyText;
                mailMessage.IsBodyHtml = false;
            }

            // set contacts
            if (message.From != null)
            {
                mailMessage.From = message.From.ToMailAddress();
            }
            foreach (var address in message.To)
            {
                mailMessage.To.Add(address.ToMailAddress());
            }
            foreach (var address in message.Bcc)
            {
                mailMessage.Bcc.Add(address.ToMailAddress());
            }
            foreach (var attachment in message.Attachments)
            {
                mailMessage.Attachments.Add(attachment.ToAttachment());
            }
            foreach (var address in message.Cc)
            {
                mailMessage.CC.Add(address.ToMailAddress());
            }
            foreach (var address in message.ReplyTo)
            {
                mailMessage.ReplyToList.Add(address.ToMailAddress());
            }
            foreach (var header in message.CustomHeaders)
            {
                mailMessage.Headers.Add(header.Key, header.Value);
            }

            return mailMessage;
        }

        public static MailAddress ToMailAddress(this MailAddressDTO dto)
        {
            if (dto == null)
                return (MailAddress)null;
            return new MailAddress(dto.Address, dto.DisplayName);
        }

        public static Attachment ToAttachment(this AttachmentDTO dto)
        {
            if (dto == null)
                return (Attachment)null;
            return new Attachment(dto.Stream, dto.Name, dto.MimeType);
        }

    }
}
