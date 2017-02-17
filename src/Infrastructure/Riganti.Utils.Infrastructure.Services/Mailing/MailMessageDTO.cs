using System.Collections.Generic;
using System.Collections.Specialized;

namespace Riganti.Utils.Infrastructure.Services.Mailing {
    public class MailMessageDTO {

        public MailAddressDTO From { get; set; }

        public MailAddressDTO Sender { get; set; }

        public ICollection<MailAddressDTO> To { get; } = new List<MailAddressDTO>();

        public ICollection<MailAddressDTO> Cc { get; } = new List<MailAddressDTO>();

        public ICollection<MailAddressDTO> Bcc { get; } = new List<MailAddressDTO>();

        public ICollection<MailAddressDTO> ReplyTo { get; } = new List<MailAddressDTO>();

        public ICollection<KeyValuePair<string, string>> CustomHeaders { get; set; } = new List<KeyValuePair<string, string>>();

        public string Subject { get; set; }

        public string BodyText { get; set; }

        public string BodyHtml { get; set; }

        public ICollection<AttachmentDTO> Attachments { get; } = new List<AttachmentDTO>();
    }
}