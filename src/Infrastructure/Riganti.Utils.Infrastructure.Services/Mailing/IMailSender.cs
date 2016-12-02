namespace Riganti.Utils.Infrastructure.Services.Mailing
{
    public interface IMailSender
    {

        void Send(MailMessageDTO message);

    }
}