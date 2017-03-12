
using Xunit;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage.Tests
{
    public class TableEntityMapperRegistryTests
    {
        [Fact]
        public void CanMapEntityTypeToTable()
        {
            var registry = TableEntityMapperRegistry.Instance;
            var table = registry.GetTable(typeof(Entities.Musician));
            Assert.Equal("Musician", table);

            registry.Map(typeof(Entities.Musician), "Singers");
            table = registry.GetTable(typeof(Entities.Musician));
            Assert.Equal("Singers", table);
        }


    }
}
