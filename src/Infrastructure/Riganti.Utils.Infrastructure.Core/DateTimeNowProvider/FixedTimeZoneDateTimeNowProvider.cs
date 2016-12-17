using System;

namespace Riganti.Utils.Infrastructure.Core
{
    public class FixedTimeZoneDateTimeNowProvider : IDateTimeNowProvider
    {
        private TimeZoneInfo timezone;

        public FixedTimeZoneDateTimeNowProvider(string timezoneId)
        {
            this.timezone = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
        }

        public DateTime Now => TimeZoneInfo.ConvertTime(DateTime.UtcNow, timezone);

    }
}