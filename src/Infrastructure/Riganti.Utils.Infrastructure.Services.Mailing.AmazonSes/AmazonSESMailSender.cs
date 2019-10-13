using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Riganti.Utils.Infrastructure.Services.Mailing;

namespace Riganti.Utils.Infrastructure.Services.Mailing.AmazonSes
{
    public class AmazonSESMailSender : IMailSender
    {
        private readonly string awsAccessKeyId;
        private readonly string awsSecretAccessKey;

        public AmazonSESMailSender(string awsAccessKeyId, string awsSecretAccessKey)
        {
            this.awsAccessKeyId = awsAccessKeyId;
            this.awsSecretAccessKey = awsSecretAccessKey;
        }

        public async Task SendAsync(MailMessageDTO message)
        {
            if (message.Attachments.Any())
            {
                throw new NotSupportedException("Amazon SES API doesn't support e-mail attachments!");
            }

            using (var client = new AmazonSimpleEmailServiceClient(awsAccessKeyId, awsSecretAccessKey))
            {
                var sendRequest = new SendEmailRequest
                {
                    Source = message.From.Address,
                    Destination = new Destination
                    {
                        ToAddresses = message.To.Select(t => t.Address).ToList(),
                        CcAddresses = message.Cc.Select(t => t.Address).ToList(),
                        BccAddresses = message.Bcc.Select(t => t.Address).ToList()
                    },
                    ReplyToAddresses = message.ReplyTo.Select(t => t.Address).ToList(),
                    Message = new Message
                    {
                        Subject = new Content(message.Subject),
                        Body = new Body
                        {
                            Text = new Content(message.BodyText),
                            Html = new Content(message.BodyHtml)
                        }
                    }
                };
                await client.SendEmailAsync(sendRequest);
            }
        }

        protected virtual void OnMessageSending(SendEmailRequest message)
        {
        }
        
    }
}
