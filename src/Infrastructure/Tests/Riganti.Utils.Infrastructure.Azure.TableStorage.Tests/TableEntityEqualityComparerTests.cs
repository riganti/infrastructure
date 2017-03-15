using System;
using Microsoft.WindowsAzure.Storage.Table;
using Riganti.Utils.Infrastructure.Azure.TableStorage.Tests.Entities;
using Xunit;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage.Tests
{
    public class TableEntityEqualityComparerTests
    {
        [Fact]
        public void TableEntities_WithSameKeys_AreEqual()
        {
            var entity1 = (ITableEntity) new Musician("John Lennon", "The Beatles", "thebeatles@riganti.cz");
            var entity2 = (ITableEntity) new Musician("Paul McCartney", "The Beatles", "thebeatles@riganti.cz");
            var equalityComparer = new TableEntityEqualityComparer<ITableEntity>();
            Assert.True(equalityComparer.Equals(entity1, entity2), "Equals failed.");
            Assert.Equal(equalityComparer.GetHashCode(entity1), equalityComparer.GetHashCode(entity2));
        }

    }
}
