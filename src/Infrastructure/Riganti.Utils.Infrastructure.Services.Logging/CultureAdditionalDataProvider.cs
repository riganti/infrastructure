using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace Riganti.Utils.Infrastructure.Services.Logging
{
    public class CultureAdditionalDataProvider : IAdditionalDataProvider
    {
        public void ExtractAdditinalData(IDictionary<string, string> additionalData)
        {
            additionalData["CurrentCulture"] = CultureInfo.CurrentCulture.DisplayName;
            additionalData["CurrentUICulture"] = CultureInfo.CurrentUICulture.DisplayName;
        }
    }
}