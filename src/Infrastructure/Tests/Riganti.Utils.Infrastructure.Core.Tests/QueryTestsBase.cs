using System.Linq;

namespace Riganti.Utils.Infrastructure.Core.Tests
{
    public class QueryTestsBase
    {
        protected readonly IQueryable<CustomerTestData> customers = new[]
        {
            new CustomerTestData()
            {
                FirstName = "Humphrey",
                LastName = "Appleby",
                CategoryId = 2,
                Truthful = false
            },
            new CustomerTestData()
            {
                FirstName = "Jim",
                LastName = "Hacker",
                CategoryId = 1,
                Truthful = false
            },
            new CustomerTestData()
            {
                FirstName = "Bernard",
                LastName = "Woolley",
                CategoryId = 2,
                Truthful = true
            }
        }.AsQueryable();
    }
}