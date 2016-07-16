using System.Net.Mail;

namespace Riganti.Utils.Infrastructure.Services.Mailing
{
    public class MessageSendingEventArgs
    {
        public MailMessage Message { get; set; }

        public bool Cancel { get; set; }

        public MessageSendingEventArgs(MailMessage message)
        {
            Message = message;
        }
    }
}