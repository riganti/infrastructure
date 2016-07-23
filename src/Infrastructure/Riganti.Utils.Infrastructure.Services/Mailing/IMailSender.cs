using System.Net.Mail;

namespace Riganti.Utils.Infrastructure.Services.Mailing
{
    public interface IMailSender
    {

        void Send(MailMessage message);

    }
}