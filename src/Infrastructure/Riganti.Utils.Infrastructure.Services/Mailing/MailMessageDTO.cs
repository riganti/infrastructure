using System.Collections.Generic;

namespace Riganti.Utils.Infrastructure.Services.Mailing
{
    public class MailMessageDTO
    {

        public MailAddressDTO From { get; set; }

        public ICollection<MailAddressDTO> To { get; } = new List<MailAddressDTO>();

        public ICollection<MailAddressDTO> Cc { get; } = new List<MailAddressDTO>();

        public ICollection<MailAddressDTO> Bcc { get; } = new List<MailAddressDTO>();

        public ICollection<MailAddressDTO> ReplyTo { get; } = new List<MailAddressDTO>();

        public string Subject { get; set; }

        public string Body { get; set; }

        public bool IsBodyHtml { get; set; }

        public ICollection<AttachmentDTO> Attachments { get; } = new List<AttachmentDTO>();

    }
}