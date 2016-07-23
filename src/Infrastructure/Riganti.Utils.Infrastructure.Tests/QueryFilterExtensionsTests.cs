using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Tests
{
    [TestClass]
    public class QueryFilterExtensionsTests
    {
        private IQueryable<CustomerTestData> customers;

        public QueryFilterExtensionsTests()
        {
            customers = new[]
            {
                new CustomerTestData()
                {
                    FirstName = "Humphrey",
                    LastName = "Appleby",
                    CategoryId = 2
                },
                new CustomerTestData()
                {
                    FirstName = "Jim",
                    LastName = "Hacker",
                    CategoryId = 1
                },
                new CustomerTestData()
                {
                    FirstName = "Bernard",
                    LastName = "Woolley",
                    CategoryId = 2
                }
            }.AsQueryable();
        }

        [TestMethod]
        public void StringFilterTest_Equality_EmptyFilter()
        {
            var filtered = customers.FilterOptionalString(c => c.FirstName, "", StringFilterMode.Equals).ToList();
            Assert.AreEqual(3, filtered.Count);
        }

        [TestMethod]
        public void StringFilterTest_Equality_NonEmptyFilter()
        {
            var filtered = customers.FilterOptionalString(c => c.FirstName, "Jim", StringFilterMode.Equals).ToList();
            Assert.AreEqual(1, filtered.Count);
            Assert.AreEqual("Jim", filtered[0].FirstName);
        }

        [TestMethod]
        public void StringFilterTest_StartsWith_NonEmptyFilter()
        {
            var filtered = customers.FilterOptionalString(c => c.FirstName, "Ji", StringFilterMode.StartsWith).ToList();
            Assert.AreEqual(1, filtered.Count);
            Assert.AreEqual("Jim", filtered[0].FirstName);
        }

        [TestMethod]
        public void StringFilterTest_Contains_NonEmptyFilter()
        {
            var filtered = customers.FilterOptionalString(c => c.FirstName + " " + c.LastName, "e", StringFilterMode.Contains).ToList();
            Assert.AreEqual(3, filtered.Count);
        }
        
        [TestMethod]
        public void OptionalFilterTest_Empty()
        {
            int? categoryId = null;
            var filtered = customers.FilterOptional(c => c.CategoryId, categoryId).ToList();
            Assert.AreEqual(3, filtered.Count);
        }

        [TestMethod]
        public void OptionalFilterTest_NonEmpty()
        {
            int? categoryId = 2;
            var filtered = customers.FilterOptional(c => c.CategoryId, categoryId).ToList();
            Assert.AreEqual(2, filtered.Count);
        }

        [TestMethod]
        public void RequiredFilterTest_NonEmpty()
        {
            var filtered = customers.FilterOptional(c => c.CategoryId, 1).ToList();
            Assert.AreEqual(1, filtered.Count);
        }
    }
}
