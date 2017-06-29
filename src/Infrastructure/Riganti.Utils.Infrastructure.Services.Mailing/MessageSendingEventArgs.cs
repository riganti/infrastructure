namespace Riganti.Utils.Infrastructure.Services.Mailing
{
    public class MessageSendingEventArgs
    {
        public MailMessageDTO Message { get; set; }

        public bool Cancel { get; set; }

        public MessageSendingEventArgs(MailMessageDTO message)
        {
            Message = message;
        }
    }
}