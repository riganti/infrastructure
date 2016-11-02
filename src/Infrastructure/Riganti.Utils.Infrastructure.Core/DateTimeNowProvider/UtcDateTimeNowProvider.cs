using System;

namespace Riganti.Utils.Infrastructure.Core
{
    public class UtcDateTimeNowProvider : IDateTimeNowProvider
    {
        public DateTime Now=> DateTime.UtcNow;
    }
}