using System;
using System.Collections.Generic;
using System.Net.Mail;

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
        public string FromAddress { get; set; }

        /// <summary>
        /// Gets or sets the format string with the {0} placeholder. If this property is set, the message subject will be placed in this placeholder.
        /// This property is used e.g. to add a common prefix to all e-mails sent from the application.
        /// </summary>
        public string SubjectFormatString { get; set; }

        /// <summary>
        /// Gets or sets the HTML with the {0} placeholder. If this property is set, the e-mail body will be placed in this placeholder.
        /// </summary>
        public string GlobalTemplateFormatString { get; set; }

        /// <summary>
        /// Gets or sets the e-mail address (or comma-separated addresses) where all e-mails will be sent, instead of the real message recipients.
        /// This property is used in the test environments to redirect all e-mails to one common test mailbox.
        /// </summary>
        public string OverrideToAddresses { get; set; }
        


        public MailerService(IMailSender sender)
        {
            this.sender = sender;
        }


        /// <summary>
        /// Sends an e-mail message to a specified recipient.
        /// </summary>
        public void SendMail(string to, string subject, string body, 
            IEnumerable<string> ccAddresses = null, 
            IEnumerable<string> bccAddresses = null,
            IEnumerable<string> replyToAddresses = null,
            IEnumerable<Attachment> attachments = null  
        )
        {
            SendMail(new [] { to }, subject, body, ccAddresses, bccAddresses, replyToAddresses, attachments);
        }

        /// <summary>
        /// Sends an e-mail message to a specified recipients.
        /// </summary>
        public void SendMail(string[] to, string subject, string body,
            IEnumerable<string> ccAddresses = null,
            IEnumerable<string> bccAddresses = null,
            IEnumerable<string> replyToAddresses = null,
            IEnumerable<Attachment> attachments = null
        )
        {
            using (var message = new MailMessage())
            {
                foreach (var recipient in to)
                {
                    message.To.Add(recipient);
                }
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                if (ccAddresses != null)
                {
                    foreach (var address in ccAddresses)
                    {
                        message.CC.Add(address);
                    }
                }
                if (bccAddresses != null)
                {
                    foreach (var address in bccAddresses)
                    {
                        message.Bcc.Add(address);
                    }
                }
                if (replyToAddresses != null)
                {
                    foreach (var address in replyToAddresses)
                    {
                        message.ReplyToList.Add(address);
                    }
                }
                if (attachments != null)
                {
                    foreach (var attachment in attachments)
                    {
                        message.Attachments.Add(attachment);
                    }
                }

                SendMail(message);
            }
        }

        /// <summary>
        /// Sends a specified e-mail message.
        /// </summary>
        public void SendMail(MailMessage message)
        {
            if (!string.IsNullOrEmpty(FromAddress))
            {
                message.From = new MailAddress(FromAddress);
            }
            if (!string.IsNullOrEmpty(SubjectFormatString))
            {
                message.Subject = string.Format(SubjectFormatString, message.Subject);
            }
            if (!string.IsNullOrEmpty(GlobalTemplateFormatString))
            {
                message.Body = string.Format(GlobalTemplateFormatString, message.Body);
            }

            if (!string.IsNullOrEmpty(OverrideToAddresses))
            {
                message.To.Clear();
                message.CC.Clear();
                message.Bcc.Clear();

                foreach (var address in OverrideToAddresses.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    message.To.Add(address);
                }
            }

            var sendingArgs = new MessageSendingEventArgs(message);
            OnMessageSending(sendingArgs);

            if (!sendingArgs.Cancel)
            {
                sender.Send(sendingArgs.Message);
            }
        }

        protected virtual void OnMessageSending(MessageSendingEventArgs args)
        {
        }
        
    }
}
