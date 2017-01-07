using System;

namespace Riganti.Utils.Infrastructure.Core
{
    public class LocalDateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;
    }
}