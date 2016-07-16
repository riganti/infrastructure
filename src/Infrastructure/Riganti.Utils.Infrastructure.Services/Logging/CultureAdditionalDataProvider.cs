using System.Collections.Generic;
using System.Threading;

namespace Riganti.Utils.Infrastructure.Services.Logging
{
    public class CultureAdditionalDataProvider : IAdditionalDataProvider
    {
        public void ExtractAdditinalData(IDictionary<string, string> additionalData)
        {
            additionalData["CurrentCulture"] = Thread.CurrentThread.CurrentCulture.DisplayName;
            additionalData["CurrentUICulture"] = Thread.CurrentThread.CurrentUICulture.DisplayName;
        }
    }
}