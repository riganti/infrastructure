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
            // DateTime.UtcNow do not provide exactly the same value when it is called on next line. It can differ by about 16ms and more. 
            Assert.True(utcDateTimeNowProviderValue.Ticks - DateTime.UtcNow.Ticks < TimeSpan.FromMilliseconds(100).Ticks);
        }
    }
}