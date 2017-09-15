using System.Collections.Generic;

namespace Riganti.Utils.Infrastructure.Logging
{
    public interface IAdditionalDataProvider
    {
        void ExtractAdditinalData(IDictionary<string, string> additionalData);
    }
}