using System.Net.Mail;

namespace Riganti.Utils.Infrastructure.Services.Mailing
{
    public class SmtpMailSender : IMailSender
    {
        private readonly SmtpClient client;

        public SmtpMailSender()
        {
            client = new SmtpClient();
        }

        public SmtpMailSender(SmtpClient client)
        {
            this.client = client;
        }

        public void Send(MailMessageDTO message)
        {
            using (var smtpMessage = new MailMessage())
            {
                smtpMessage.From = ConvertAddress(message.From);
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
                smtpMessage.Body = message.Body;
                smtpMessage.IsBodyHtml = message.IsBodyHtml;
                
                foreach (var attachment in message.Attachments)
                {
                    smtpMessage.Attachments.Add(ConvertAttachment(attachment));
                }

                client.Send(smtpMessage);
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