using System;
using System.Linq;
using Xunit;

namespace Riganti.Utils.Infrastructure.Core.Tests.Query
{
    public class QueryFilterExtensionsTests : QueryTestsBase
    {
        [Fact]
        public void StringFilterTest_Equality_EmptyFilter()
        {
            var filtered = customers.FilterOptionalString(c => c.FirstName, "", StringFilterMode.Equals).ToList();
            Assert.Equal(3, filtered.Count);
        }

        [Fact]
        public void StringFilterTest_Equality_NonEmptyFilter()
        {
            var jim = "Jim";
            var filtered = customers.FilterOptionalString(c => c.FirstName, jim, StringFilterMode.Equals).ToList();
            Assert.Equal(1, filtered.Count);
            Assert.Equal(jim, filtered[0].FirstName);
        }

        [Theory]
        [InlineData("J")]
        [InlineData("Ji")]
        [InlineData("Jim")]
        public void StringFilterTest_StartsWith_NonEmptyFilter(string valueToFilter)
        {
            var filtered = customers.FilterOptionalString(c => c.FirstName, valueToFilter, StringFilterMode.StartsWith).ToList();
            Assert.Equal(1, filtered.Count);
            Assert.Equal("Jim", filtered[0].FirstName);
        }

        [Fact]
        public void StringFilterTest_Contains_NonEmptyFilter()
        {
            var filtered = customers.FilterOptionalString(c => c.FirstName + " " + c.LastName, "e", StringFilterMode.Contains).ToList();
            Assert.Equal(3, filtered.Count);
        }

        [Fact]
        public void OptionalFilterTest_Empty()
        {
            int? categoryId = null;
            var filtered = customers.FilterOptional(c => c.CategoryId, categoryId).ToList();
            Assert.Equal(3, filtered.Count);
        }

        [Fact]
        public void OptionalFilterTest_NonEmpty()
        {
            int? categoryId = 2;
            var filtered = customers.FilterOptional(c => c.CategoryId, categoryId).ToList();
            Assert.Equal(2, filtered.Count);
        }

        [Fact]
        public void RequiredFilterTest_NonEmpty()
        {
            var filtered = customers.FilterOptional(c => c.CategoryId, 1).ToList();
            Assert.Equal(1, filtered.Count);
        }

        [Fact]
        public void OptionalFilterTestNullableParameter_Empty()
        {
            bool? truthful = null;
            var filtered = customers.FilterOptional(c => c.Truthful, truthful).ToList();
            Assert.Equal(3, filtered.Count);
        }

        [Fact]
        public void OptionalFilterTestNullableParameter_NonEmpty()
        {
            bool? truthful = true;
            var filtered = customers.FilterOptional(c => c.Truthful, truthful).ToList();
            Assert.Equal(1, filtered.Count);
        }

        [Fact]
        public void RequiredFilterTestNullableParameter_NonEmpty()
        {
            bool? truthful = false;
            var filtered = customers.FilterOptional(c => c.Truthful, truthful).ToList();
            Assert.Equal(2, filtered.Count);
        }

        [Fact]
        public void FilterOptionalString_SetNonExistsStringFilterMode_ThrowArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => customers.FilterOptionalString(c => c.FirstName, "Jim", (StringFilterMode)(-1)));
        }
    }
}
