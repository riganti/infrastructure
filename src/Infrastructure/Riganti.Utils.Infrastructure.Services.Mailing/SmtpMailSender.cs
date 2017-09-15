using System;
using System.Net.Security;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace Riganti.Utils.Infrastructure.Services.Mailing
{
    public class SmtpMailSender : IMailSender
    {

        public string HostName { get; }

        public int Port { get; }

        public string UserName { get; }

        public string Password { get; }

        public bool AllowSsl { get; }

        public RemoteCertificateValidationCallback ServerCertificateValidationCallback { get; }

        public SmtpMailSender(string hostName, int port, string userName, string password, bool allowSsl, RemoteCertificateValidationCallback sslCallback)
        {
            this.HostName = hostName;
            this.Port = port;
            this.UserName = userName;
            this.Password = password;
            this.AllowSsl = allowSsl;
            this.ServerCertificateValidationCallback = sslCallback;
        }

        public async Task SendAsync(MailMessageDTO message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            // Get MIME message
            var msg = message.ToMimeMessage();

            // Send message
            using (var mx = new SmtpClient())
            {
                await mx.ConnectAsync(this.HostName, this.Port, AllowSsl ? SecureSocketOptions.Auto : SecureSocketOptions.None);
                if (AllowSsl) mx.ServerCertificateValidationCallback = this.ServerCertificateValidationCallback;
                if (!string.IsNullOrEmpty(this.UserName) && !string.IsNullOrEmpty(this.Password)) await mx.AuthenticateAsync(this.UserName, this.Password);
                await mx.SendAsync(msg);
                await mx.DisconnectAsync(true);
            }
        }
    }
}
