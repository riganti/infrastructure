using Riganti.Utils.Infrastructure.Services.Mailing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Riganti.Utils.Infrastructure.Services.Mailing.SendGrid.Mailing
{
    public class TemplateDTO : ITemplate
    {
        public string TemplateId { get; set; }
        public ICollection<KeyValuePair<string, object>> Substitution { get; set; } = new List<KeyValuePair<string, object>>();
    }
}
