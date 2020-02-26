using System.Collections.Generic;

namespace Riganti.Utils.Infrastructure.Services.Mailing.SendGrid
{
    public class MailingTemplateDTO : IMailingTemplate
    {
        public string TemplateId { get; set; }
        public ICollection<KeyValuePair<string, object>> Substitution { get; set; } = new List<KeyValuePair<string, object>>();
    }
}
