using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Riganti.Utils.Infrastructure.Services.Mailing
{
    public class SmtpClientMailSender : IMailSender
    {
        private SmtpClient SmtpClient { get; }


        public SmtpClientMailSender(SmtpClient smtpClient)
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
