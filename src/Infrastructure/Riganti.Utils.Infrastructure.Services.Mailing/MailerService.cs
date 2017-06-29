using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Riganti.Utils.Infrastructure.Services.Mailing
{
    /// <summary>
    /// A common gateway for outgoing e-mails. It supports global e-mail templates, formatting the subject or test environment settings for redirecting all messages to a test mailbox.
    /// </summary>
    public class MailerService
    {
        private readonly IMailSender sender;

        /// <summary>
        /// Gets or sets the From address.
        /// </summary>
        public MailAddressDTO From { get; set; }

        /// <summary>
        /// Gets or sets the format string with the {0} placeholder. If this property is set, the message subject will be placed in this placeholder.
        /// This property is used e.g. to add a common prefix to all e-mails sent from the application.
        /// </summary>
        public string SubjectFormatString { get; set; }

        /// <summary>
        /// Gets or sets the format string with the {0} placeholder. If this property is set, the message body text will be placed in this placeholder.
        /// </summary>
        public string BodyTextFormatString { get; set; }

        /// <summary>
        /// Gets or sets the format string with the {0} placeholder. If this property is set, the message body HTML be placed in this placeholder.
        /// </summary>
        public string BodyHtmlFormatString { get; set; }

        /// <summary>
        /// Gets or sets the e-mail address (or comma-separated addresses) where all e-mails will be sent, instead of the real message recipients.
        /// This property is used in the test environments to redirect all e-mails to one common test mailbox.
        /// </summary>
        public ICollection<MailAddressDTO> OverrideToAddresses { get; set; }
        


        public MailerService(IMailSender sender)
        {
            this.sender = sender;
        }


        /// <summary>
        /// Sends an e-mail message to a specified recipient.
        /// </summary>
        public Task SendMailAsync(string to, string subject, string body, 
            IEnumerable<string> ccAddresses = null, 
            IEnumerable<string> bccAddresses = null,
            IEnumerable<string> replyToAddresses = null,
            IEnumerable<AttachmentDTO> attachments = null  
        )
        {
            return SendMailAsync(new [] { to }, subject, body, ccAddresses, bccAddresses, replyToAddresses, attachments);
        }

        /// <summary>
        /// Sends an e-mail message to a specified recipients.
        /// </summary>
        public Task SendMailAsync(string[] to, string subject, string body,
            IEnumerable<string> ccAddresses = null,
            IEnumerable<string> bccAddresses = null,
            IEnumerable<string> replyToAddresses = null,
            IEnumerable<AttachmentDTO> attachments = null
        )
        {
            var message = new MailMessageDTO();
            foreach (var recipient in to)
            {
                message.To.Add(new MailAddressDTO(recipient));
            }
            message.Subject = subject;
            message.BodyHtml = body;

            if (ccAddresses != null)
            {
                foreach (var address in ccAddresses)
                {
                    message.Cc.Add(new MailAddressDTO(address));
                }
            }
            if (bccAddresses != null)
            {
                foreach (var address in bccAddresses)
                {
                    message.Bcc.Add(new MailAddressDTO(address));
                }
            }
            if (replyToAddresses != null)
            {
                foreach (var address in replyToAddresses)
                {
                    message.ReplyTo.Add(new MailAddressDTO(address));
                }
            }
            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    message.Attachments.Add(attachment);
                }
            }

            return SendMailAsync(message);
        }

        /// <summary>
        /// Sends a specified e-mail message.
        /// </summary>
        public async Task SendMailAsync(MailMessageDTO message)
        {
            if (From != null)
            {
                message.From = From;
            }
            if (!string.IsNullOrEmpty(SubjectFormatString))
            {
                message.Subject = string.Format(SubjectFormatString, message.Subject);
            }
            if (!string.IsNullOrEmpty(BodyTextFormatString) && !string.IsNullOrEmpty(message.BodyText))
            {
                message.BodyText = string.Format(BodyTextFormatString, message.BodyText);
            }
            if (!string.IsNullOrEmpty(BodyHtmlFormatString) && !string.IsNullOrEmpty(message.BodyHtml))
            {
                message.BodyHtml = string.Format(BodyHtmlFormatString, message.BodyHtml);
            }

            if (OverrideToAddresses != null)
            {
                message.To.Clear();
                message.Cc.Clear();
                message.Bcc.Clear();

                foreach (var address in OverrideToAddresses)
                {
                    message.To.Add(address);
                }
            }

            var sendingArgs = new MessageSendingEventArgs(message);
            OnMessageSending(sendingArgs);

            if (!sendingArgs.Cancel)
            {
                await sender.SendAsync(sendingArgs.Message);
            }
        }

        protected virtual void OnMessageSending(MessageSendingEventArgs args)
        {
        }
        
    }
}
