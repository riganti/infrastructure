using System.Threading.Tasks;

namespace Riganti.Utils.Infrastructure.Services.Mailing
{
    public interface IMailSender
    {

        Task SendAsync(MailMessageDTO message);

    }
}