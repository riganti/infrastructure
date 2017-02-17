using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeKit;
using Riganti.Utils.Infrastructure.Services.Mailing;

namespace Riganti.Utils.Infrastructure.Services.Smtp {
    internal static class Extensions {

        public static MimeMessage ToMimeMessage(this MailMessageDTO m) {
            var msg = new MimeMessage();

            // Add standard header fields
            msg.From.Add(m.From.ToMailboxAddress());
            msg.To.AddRange(m.To.ToMailboxAddress());
            msg.Cc.AddRange(m.Cc.ToMailboxAddress());
            msg.Bcc.AddRange(m.Bcc.ToMailboxAddress());
            msg.Sender = m.Sender.ToMailboxAddress();
            msg.ReplyTo.AddRange(m.ReplyTo.ToMailboxAddress());
            msg.Subject = m.Subject;

            // Add custom header fields
            foreach (var item in m.CustomHeaders) {
                msg.Headers.Add(item.Key, item.Value);
            }

            // Construct body
            var bb = new BodyBuilder {
                TextBody = m.BodyText,
                HtmlBody = m.BodyHtml
            };

            // Add attachments
            foreach (var item in m.Attachments) {
                ContentType ct;
                var r = ContentType.TryParse(item.MimeType, out ct);
                if (!r) ct = new ContentType("application", "octet-stream");
                bb.Attachments.Add(item.Name, item.Stream, ct);
            }

            msg.Body = bb.ToMessageBody();

            return msg;
        }

        internal static IEnumerable<MailboxAddress> ToMailboxAddress(this IEnumerable<MailAddressDTO> a) {
            return a.Select(x => x.ToMailboxAddress());
        }

        internal static MailboxAddress ToMailboxAddress(this MailAddressDTO a) {
            if (a == null) return null;
            return new MailboxAddress(a.DisplayName, a.Address);
        }

    }
}
