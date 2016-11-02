using System;
using Xunit;

namespace Riganti.Utils.Infrastructure.Core.Tests
{
    public class UtcDateTimeNowProviderTests
    {
        readonly UtcDateTimeNowProvider utcDateTimeNowProviderSUT = new UtcDateTimeNowProvider();

        [Fact]
        public void ProvidesUtcTime_Test()
        {
            var utcDateTimeNowProviderValue = utcDateTimeNowProviderSUT.Now;

            Assert.True(DateTime.Compare(utcDateTimeNowProviderValue, DateTime.UtcNow) == 0);
        }
    }
}