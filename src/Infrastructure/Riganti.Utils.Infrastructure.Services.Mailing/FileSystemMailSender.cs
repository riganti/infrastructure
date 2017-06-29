using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riganti.Utils.Infrastructure.Services.Mailing
{
    public class FileSystemMailSender : IMailSender
    {
        private readonly string directory;

        public FileSystemMailSender(string directory)
        {
            this.directory = directory;
        }

        public async Task SendAsync(MailMessageDTO message)
        {
            var sb = new StringBuilder();
            sb.AppendLine("From: " + message.From);
            sb.AppendLine("To: " + string.Join("; ", message.To));
            sb.AppendLine("CC: " + string.Join("; ", message.Cc));
            sb.AppendLine("BCC: " + string.Join("; ", message.Bcc));
            sb.AppendLine("Reply-To: " + string.Join("; ", message.ReplyTo));
            sb.AppendLine("Subject: " + message.Subject);
            sb.AppendLine();
            sb.AppendLine(message.BodyHtml);
            sb.AppendLine();
            sb.AppendLine(message.BodyText);

            var fileName = Path.Combine(directory, Guid.NewGuid() + ".txt");
            using (var fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    await sw.WriteAsync(sb.ToString());
                }
            }
        }
    }
}
