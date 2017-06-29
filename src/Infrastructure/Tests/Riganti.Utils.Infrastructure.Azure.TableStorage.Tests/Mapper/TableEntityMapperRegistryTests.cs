using Riganti.Utils.Infrastructure.Azure.TableStorage.Tests.Entities;
using Xunit;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage.Tests.Mapper
{
    public class TableEntityMapperRegistryTests
    {
        [Fact]
        public void CanMapEntityTypeToTypeNameTable()
        {
            var registry = new AggregateTableEntityMapper(new TypeNameTableEntityMapper());
            Assert.Equal("Musician", registry.GetTable(typeof(Musician)));
            Assert.Equal("Musician", registry.GetTable<Musician>());

            var musician = new Musician("John Doe", "Anonymous", "john.doe@riganti.cz");
            Assert.Equal("Musician", registry.GetTable(musician));
        }
        

        [Fact]
        public void CanAddMapperToRegistry()
        {
            var registry = new RegistryTableEntityMapper();
            registry.Map(typeof(Musician), "Singers");
            Assert.Equal("Singers", registry.GetTable(typeof(Musician)));
            Assert.Equal("Singers", registry.GetTable<Musician>());

            registry.Map<Band>("GuitarPlayers");
            Assert.Equal("GuitarPlayers", registry.GetTable<Band>());
        }
    }
}
