using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Riganti.Utils.Infrastructure.Services.Mailing;
using Riganti.Utils.Infrastructure.Services.Smtp.Mailing;
using Xunit;

namespace Riganti.Utils.Infrastructure.Services.Smtp.Tests
{
    public class PickupFolderMailSenderTest
    {

        [Fact]
        public async Task SendPlainTextMail_Test()
        {
            var mx = new PickupFolderMailSender(CreateTempFolder("plain"));
            var msg = new MailMessageDTO
            {
                From = new MailAddressDTO("sender@example.com", "Example Sender"),
                Subject = "Žluťoučký kůň úpěl ďábelské ódy - subject",
                BodyText = "Žluťoučký kůň úpěl ďábelské ódy - text."
            };
            msg.To.Add(new MailAddressDTO("recipient@example.com", "Example Recipient"));
            await mx.SendAsync(msg);

            Assert.True(EmlFileExists(mx.FolderName));
        }

        [Fact]
        public async Task SendHtmlMail_Test()
        {
            var mx = new PickupFolderMailSender(CreateTempFolder("html"));
            var msg = new MailMessageDTO
            {
                From = new MailAddressDTO("sender@example.com", "Example Sender"),
                Subject = "Žluťoučký kůň úpěl ďábelské ódy - subject",
                BodyHtml = "<html><body><p>Žluťoučký kůň úpěl ďábelské ódy <b>v HTML</b>.</p></body></html>"
            };
            msg.To.Add(new MailAddressDTO("recipient@example.com", "Example Recipient"));
            await mx.SendAsync(msg);

            Assert.True(EmlFileExists(mx.FolderName));
        }

        [Fact]
        public async Task SendAlternateMail_Test()
        {
            var mx = new PickupFolderMailSender(CreateTempFolder("alternate"));
            var msg = new MailMessageDTO
            {
                From = new MailAddressDTO("sender@example.com", "Example Sender"),
                Subject = "Žluťoučký kůň úpěl ďábelské ódy - subject",
                BodyText = "Žluťoučký kůň úpěl ďábelské ódy - text.",
                BodyHtml = "<html><body><p>Žluťoučký kůň úpěl ďábelské ódy <b>v HTML</b>.</p></body></html>"
            };
            msg.To.Add(new MailAddressDTO("recipient@example.com", "Example Recipient"));
            await mx.SendAsync(msg);

            Assert.True(EmlFileExists(mx.FolderName));
        }

        [Fact]
        public async Task SendMailWithAttachment_Test()
        {
            var mx = new PickupFolderMailSender(CreateTempFolder("attachment"));
            var msg = new MailMessageDTO
            {
                From = new MailAddressDTO("sender@example.com", "Example Sender"),
                Subject = "Žluťoučký kůň úpěl ďábelské ódy - subject",
                BodyText = "Žluťoučký kůň úpěl ďábelské ódy - text.",
                BodyHtml = "<html><body><p>Žluťoučký kůň úpěl ďábelské ódy <b>v HTML</b>.</p></body></html>"
            };
            msg.To.Add(new MailAddressDTO("recipient@example.com", "Example Recipient"));

            using (var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("Test attachment file")))
            {
                msg.Attachments.Add(new AttachmentDTO { Name = "attachment.txt", MimeType = "text/plain", Stream = ms });
                await mx.SendAsync(msg);
            }

            Assert.True(EmlFileExists(mx.FolderName));
        }

        private static bool EmlFileExists(string folderName)
        {
            return Directory.EnumerateFiles(folderName, "*.eml").Count() == 1;
        }

        private static string CreateTempFolder(string suffix)
        {
            var folderName = Path.Combine(Path.GetTempPath(), "PickupFolderMailSenderTest", DateTime.Now.ToString("yyyyMMdd-HHmmss-fffffff") + "-" + suffix);
            Directory.CreateDirectory(folderName);
            return folderName;
        }

    }
}
