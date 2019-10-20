using System;
using System.Collections.Generic;
using System.Text;

namespace Riganti.Utils.Infrastructure.Services.Mailing
{
    public interface IMailingTemplate
    {
        string TemplateId { get; set; }
        ICollection<KeyValuePair<string, object>> Substitution { get; set; }
    }
}
