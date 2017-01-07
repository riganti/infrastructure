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

            Assert.True(DateTime.Compare(utcDateTimeNowProviderValue, DateTime.UtcNow) == 0);
        }
    }
}