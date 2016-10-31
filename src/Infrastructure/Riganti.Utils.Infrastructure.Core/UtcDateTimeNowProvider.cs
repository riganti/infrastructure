using System;

namespace Riganti.Utils.Infrastructure.Core
{
    class UtcDateTimeNowProvider : IDateTimeNowProvider
    {
        public DateTime Now=> DateTime.UtcNow;
    }
}