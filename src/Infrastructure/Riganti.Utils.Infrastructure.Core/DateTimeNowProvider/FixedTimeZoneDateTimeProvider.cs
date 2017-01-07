using System;

namespace Riganti.Utils.Infrastructure.Core
{
    public class FixedTimeZoneDateTimeProvider : IDateTimeProvider
    {
        private TimeZoneInfo timezone;

        public FixedTimeZoneDateTimeProvider(string timezoneId)
        {
            this.timezone = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
        }

        public DateTime Now => TimeZoneInfo.ConvertTime(DateTime.UtcNow, timezone);

    }
}