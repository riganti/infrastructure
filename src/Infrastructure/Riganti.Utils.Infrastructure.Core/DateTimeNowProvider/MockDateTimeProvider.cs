using System;

namespace Riganti.Utils.Infrastructure.Core
{
    public class MockDateTimeProvider : IDateTimeProvider
    {
        private readonly DateTime desiredDate;

        public DateTime Now => desiredDate;

        public MockDateTimeProvider(DateTime desiredDate)
        {
            this.desiredDate = desiredDate;
        }
    }
}