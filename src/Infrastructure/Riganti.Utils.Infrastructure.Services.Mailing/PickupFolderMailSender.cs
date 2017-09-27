using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Riganti.Utils.Infrastructure.Services.Mailing
{
    public class PickupFolderMailSender : IMailSender
    {

        public string FolderName { get; }

        public PickupFolderMailSender(string folderName)
        {
            if (folderName == null) throw new ArgumentNullException(nameof(folderName));
            if (string.IsNullOrWhiteSpace(folderName)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(folderName));

            FolderName = folderName;
        }

        public async Task SendAsync(MailMessageDTO message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            // Convert to message
            var msg = message.ToMimeMessage();

            // Write to temp file to avoid pickup of incomplete message
            var tempFileName = Path.GetTempFileName();
            using (var sw = File.CreateText(tempFileName))
            {
                // Write envelope sender
                await sw.WriteLineAsync($"X-Sender: <{message.From.Address}>");

                // Write envelope receivers
                var receivers = message.To
                    .Union(message.Cc)
                    .Union(message.Bcc);
                foreach (var item in receivers.Select(x => x.Address))
                {
                    await sw.WriteLineAsync($"X-Receiver: <{item}>");
                }

                // Flush data and write rest of message
                await sw.FlushAsync();
                msg.WriteTo(sw.BaseStream);
            }

            var msgFileName = Path.Combine(FolderName, string.Join(".", DateTime.Now.ToString("yyyyMMdd-HHmmss"), Guid.NewGuid().ToString("N"), "eml"));

            // Create final directory if not exists
            if (!Directory.Exists(FolderName))
            {
                Directory.CreateDirectory(FolderName);
            }

            // Move the file to final destination
            File.Move(tempFileName, msgFileName);
        }
    }
}
