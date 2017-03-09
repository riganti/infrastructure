using System;
using Xunit;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage.Tests
{
    public class TableEntityEqualityComparerTests
    {
        [Fact]
        public void TwoTuples_HavingSameValue_ShouldBeEqual()
        {
            var entity1 = new Tuple<string, string>("Smith", "John");
            var entity2 = new Tuple<string, string>("Smith", "John");
            var equalityComparer = new TableEntityEqualityComparer();
            Assert.True(equalityComparer.Equals(entity1, entity2));
        }

        [Fact]
        public void TwoTuples_DifferentCasing_ShouldNotBeEqual()
        {
            var entity1 = new Tuple<string, string>("smith", "john");
            var entity2 = new Tuple<string, string>("SMITH", "JOHN");
            var equalityComparer = new TableEntityEqualityComparer();
            Assert.False(equalityComparer.Equals(entity1, entity2));
        }


    }
}
