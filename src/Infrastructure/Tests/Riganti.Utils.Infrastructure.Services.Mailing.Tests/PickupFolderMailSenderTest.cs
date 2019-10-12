using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Xunit;

namespace Riganti.Utils.Infrastructure.Services.Mailing.Tests
{
    public class PickupFolderMailSenderTest
    {

        [Fact]
        public async Task SendPlainTextMail_Test()
        {
            var tempFolder = CreateTempFolder("plain");
            var mx = new SmtpClientMailSender(CreateSmtpClient(tempFolder));
            var msg = new MailMessageDTO
            {
                From = new MailAddressDTO("sender@example.com", "Example Sender"),
                Subject = "Žluťoučký kůň úpěl ďábelské ódy - subject",
                BodyText = "Žluťoučký kůň úpěl ďábelské ódy - text."
            };
            msg.To.Add(new MailAddressDTO("recipient@example.com", "Example Recipient"));
            await mx.SendAsync(msg);

            Assert.True(EmlFileExists(tempFolder));
        }

        [Fact]
        public async Task SendHtmlMail_Test()
        {
            var tempFolder = CreateTempFolder("html");
            var mx = new SmtpClientMailSender(CreateSmtpClient(tempFolder));
            var msg = new MailMessageDTO
            {
                From = new MailAddressDTO("sender@example.com", "Example Sender"),
                Subject = "Žluťoučký kůň úpěl ďábelské ódy - subject",
                BodyHtml = "<html><body><p>Žluťoučký kůň úpěl ďábelské ódy <b>v HTML</b>.</p></body></html>"
            };
            msg.To.Add(new MailAddressDTO("recipient@example.com", "Example Recipient"));
            await mx.SendAsync(msg);

            Assert.True(EmlFileExists(tempFolder));
        }

        [Fact]
        public async Task SendAlternateMail_Test()
        {
            var tempFolder = CreateTempFolder("alternate");
            var mx = new SmtpClientMailSender(CreateSmtpClient(tempFolder));
            var msg = new MailMessageDTO
            {
                From = new MailAddressDTO("sender@example.com", "Example Sender"),
                Subject = "Žluťoučký kůň úpěl ďábelské ódy - subject",
                BodyText = "Žluťoučký kůň úpěl ďábelské ódy - text.",
                BodyHtml = "<html><body><p>Žluťoučký kůň úpěl ďábelské ódy <b>v HTML</b>.</p></body></html>"
            };
            msg.To.Add(new MailAddressDTO("recipient@example.com", "Example Recipient"));
            await mx.SendAsync(msg);

            Assert.True(EmlFileExists(tempFolder));
        }

        [Fact]
        public async Task SendMailWithAttachment_Test()
        {
            var tempFolder = CreateTempFolder("attachment");
            var mx = new SmtpClientMailSender(CreateSmtpClient(tempFolder));
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

            Assert.True(EmlFileExists(tempFolder));
        }
        
        private static bool EmlFileExists(string folderName)
        {
            return Directory.EnumerateFiles(folderName, "*.eml").Count() == 1;
        }

        private static string CreateTempFolder(string suffix)
        {
            var folderName = GetTempFolderName(suffix);
            Directory.CreateDirectory(folderName);
            return folderName;
        }

        private static string GetTempFolderName(string suffix)
        {
            var folderName = Path.Combine(Path.GetTempPath(), "PickupFolderMailSenderTest", DateTime.Now.ToString("yyyyMMdd-HHmmss-fffffff") + "-" + suffix);
            return folderName;
        }

        private static SmtpClient CreateSmtpClient(string pickupDirectory)
        {
            return new SmtpClient()
            {
                DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                PickupDirectoryLocation = pickupDirectory
            };
        }

    }
}
