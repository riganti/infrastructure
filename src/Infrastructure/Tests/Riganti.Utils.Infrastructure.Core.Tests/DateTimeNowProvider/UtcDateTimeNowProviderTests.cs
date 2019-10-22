using System;
using Xunit;

namespace Riganti.Utils.Infrastructure.Core.Tests.DateTimeNowProvider
{
    public class UtcDateTimeNowProviderTests
    {
        readonly UtcDateTimeProvider utcDateTimeProviderSUT = new UtcDateTimeProvider();

        [Fact]
        public void ProvidesUtcTime_Test()
        {
            var utcDateTimeNowProviderValue = utcDateTimeProviderSUT.Now;

            Assert.True(AreDateTimeClose(DateTime.Now, utcDateTimeNowProviderValue));
        }

        public bool AreDateTimeClose(DateTime time, DateTime time2)
        {
            if (time.AddSeconds(-2) > time2 && time2 < time.AddSeconds(2))
                return true;
            return false;
        }
    }
}