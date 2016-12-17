using System;

namespace Riganti.Utils.Infrastructure.Core
{
    public class MockDateTimeNowProvider : IDateTimeNowProvider
    {
        private readonly DateTime desiredDate;

        public DateTime Now => desiredDate;

        public MockDateTimeNowProvider(DateTime desiredDate)
        {
            this.desiredDate = desiredDate;
        }
    }
}