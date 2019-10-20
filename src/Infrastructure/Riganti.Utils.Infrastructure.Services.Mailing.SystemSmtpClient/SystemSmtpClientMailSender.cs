using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Riganti.Utils.Infrastructure.Services.Mailing.SystemSmtpClient
{
    public class SystemSmtpClientMailSender : IMailSender
    {
        private SmtpClient SmtpClient { get; }


        public SystemSmtpClientMailSender(SmtpClient smtpClient)
        {
            SmtpClient = smtpClient;
        }

        public async Task SendAsync(MailMessageDTO message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var mailMessage = message.ToMailMessage();
            await SmtpClient.SendMailAsync(mailMessage);
        }
    }
}
