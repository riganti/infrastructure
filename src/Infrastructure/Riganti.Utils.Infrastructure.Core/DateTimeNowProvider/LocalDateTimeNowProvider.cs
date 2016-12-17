using System;

namespace Riganti.Utils.Infrastructure.Core
{
    public class LocalDateTimeNowProvider : IDateTimeNowProvider
    {
        public DateTime Now => DateTime.Now;
    }
}