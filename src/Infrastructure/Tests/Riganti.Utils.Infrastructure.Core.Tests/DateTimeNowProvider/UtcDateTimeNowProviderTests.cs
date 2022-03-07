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
            var diff = DateTime.UtcNow - utcDateTimeNowProviderValue;

            Assert.True(diff < TimeSpan.FromSeconds(1));
        }
    }
}