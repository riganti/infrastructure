using System;
using Microsoft.WindowsAzure.Storage.Table;
using Riganti.Utils.Infrastructure.Azure.TableStorage.Tests.Entities;
using Xunit;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage.Tests
{
    public class TableEntityEqualityComparerTests
    {
        [Fact]
        public void TwoTuples_HavingSameValue_AreEqual()
        {
            var entity1 = new Tuple<string, string>("The Beatles", "John");
            var entity2 = new Tuple<string, string>("The Beatles", "John");
            var equalityComparer = new TableEntityEqualityComparer();
            Assert.True(equalityComparer.Equals(entity1, entity2), "Equals failed.");
            Assert.Equal(equalityComparer.GetHashCode(entity1), equalityComparer.GetHashCode(entity2));
        }

        [Fact]
        public void TwoTuples_HavingSameValueWithDifferentCasing_AreNotEqual()
        {
            var entity1 = new Tuple<string, string>("The Beatles", "John");
            var entity2 = new Tuple<string, string>("the beatles", "john");
            var equalityComparer = new TableEntityEqualityComparer();
            Assert.False(equalityComparer.Equals(entity1, entity2), "Equals failed.");
            Assert.NotEqual(equalityComparer.GetHashCode(entity1), equalityComparer.GetHashCode(entity2));
        }

        [Fact]
        public void TableEntities_WithSameKeys_AreEqual()
        {
            var entity1 = (ITableEntity) new Musician("John Lennon", "The Beatles", "thebeatles@riganti.cz");
            var entity2 = (ITableEntity) new Musician("Paul McCartney", "The Beatles", "thebeatles@riganti.cz");
            var equalityComparer = new TableEntityEqualityComparer();
            Assert.True(equalityComparer.Equals(entity1, entity2), "Equals failed.");
            Assert.Equal(equalityComparer.GetHashCode(entity1), equalityComparer.GetHashCode(entity2));
        }

    }
}
