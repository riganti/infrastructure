using System.Linq;

namespace Riganti.Utils.Infrastructure.Core.Tests.Query
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
                Truthful = false,
                Address = new Address()
                {
                    City = "Small City",
                    Street = "Short Street"
                }
            },
            new CustomerTestData()
            {
                FirstName = "Jim",
                LastName = "Hacker",
                CategoryId = 1,
                Truthful = false,
                Address = new Address()
                {
                    City = "New York",
                    Street = "Big Street"
                }
            },
            new CustomerTestData()
            {
                FirstName = "Bernard",
                LastName = "Woolley",
                CategoryId = 2,
                Truthful = true,
                Address = new Address()
                {
                    City = "Prague",
                    Street = "Big Street"
                }
            }
        }.AsQueryable();
    }
}