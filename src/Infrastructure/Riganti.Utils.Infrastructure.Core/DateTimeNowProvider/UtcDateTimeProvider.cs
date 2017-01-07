using System;

namespace Riganti.Utils.Infrastructure.Core
{
    public class UtcDateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.UtcNow;
    }
}