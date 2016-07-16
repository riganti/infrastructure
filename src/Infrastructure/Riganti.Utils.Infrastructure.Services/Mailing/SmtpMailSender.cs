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

        public void Send(MailMessage message)
        {
            client.Send(message);
        }
    }
}