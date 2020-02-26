namespace Riganti.Utils.Infrastructure.Core.Tests
{
    public class CustomerTestData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CategoryId { get; set; }
        public bool? Truthful { get; set; }
        public Address Address { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
    }
}