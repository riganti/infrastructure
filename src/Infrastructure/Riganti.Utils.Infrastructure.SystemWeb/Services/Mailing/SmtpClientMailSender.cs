    using System.Net.Mail;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Riganti.Utils.Infrastructure.Services.Mailing
{
    public class SmtpClientMailSender : IMailSender
    {
        private readonly SmtpClient client;

        public SmtpClientMailSender()
        {
            client = new SmtpClient();
        }

        public SmtpClientMailSender(SmtpClient client)
        {
            this.client = client;
        }

        public async Task SendAsync(MailMessageDTO message)
        {
            using (var smtpMessage = new MailMessage())
            {
                if (message.From != null)
                {
                    smtpMessage.From = ConvertAddress(message.From);
                }
                foreach (var to in message.To)
                {
                    smtpMessage.To.Add(ConvertAddress(to));
                }
                foreach (var cc in message.Cc)
                {
                    smtpMessage.CC.Add(ConvertAddress(cc));
                }
                foreach (var bcc in message.Bcc)
                {
                    smtpMessage.Bcc.Add(ConvertAddress(bcc));
                }
                foreach (var replyTo in message.ReplyTo)
                {
                    smtpMessage.ReplyToList.Add(ConvertAddress(replyTo));
                }
                smtpMessage.Subject = message.Subject;
                if (!string.IsNullOrEmpty(message.BodyHtml))
                {
                    smtpMessage.Body = message.BodyHtml;
                    smtpMessage.IsBodyHtml = true;
                }
                else
                {
                    smtpMessage.Body = message.BodyText;
                    smtpMessage.IsBodyHtml = false;
                }

                foreach (var attachment in message.Attachments)
                {
                    smtpMessage.Attachments.Add(ConvertAttachment(attachment));
                }

                await client.SendMailAsync(smtpMessage);
            }
        }

        private Attachment ConvertAttachment(AttachmentDTO attachment)
        {
            return new Attachment(attachment.Stream, attachment.Name, attachment.MimeType);
        }

        private MailAddress ConvertAddress(MailAddressDTO address)
        {
            if (string.IsNullOrEmpty(address.DisplayName))
            {
                return new MailAddress(address.Address);
            }
            else
            {
                return new MailAddress(address.Address, address.DisplayName);
            }
        }
    }
}