using System.Collections.Generic;
using System.Globalization;

namespace Riganti.Utils.Infrastructure.Logging
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