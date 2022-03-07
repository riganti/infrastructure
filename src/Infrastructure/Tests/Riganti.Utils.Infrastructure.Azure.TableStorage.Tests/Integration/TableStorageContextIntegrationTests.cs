using System;
using System.Threading.Tasks;
using Riganti.Utils.Infrastructure.Azure.TableStorage.Tests.Entities;
using Riganti.Utils.Infrastructure.Azure.TableStorage.Tests.StorageContext;
using Xunit;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage.Tests.Integration
{
    public class TableStorageContextIntegrationTests
    {
        private readonly TableStorageContext context;

        public TableStorageContextIntegrationTests()
        {
            var options = new CreateOwnContextTableStorageOptions();
            context = new TableStorageContext(options);
        }

        [Fact]
        public void CanInstantiateNewContext_WithOptions()
        {
            Assert.NotNull(context);
        }

        [Fact]
        public async Task CanCreateAndDeleteTableAsync()
        {
            var tableName = $"xy1a{DateTime.Now.Ticks}";
            var cloudTable = await context.GetOrCreateTableAsync(tableName);
            await context.DeleteTableAsync(cloudTable.Name);
        }

        [Fact]
        public async Task SaveChanges_Should_ReturnNumberOfProcessedRecords()
        {
            var registry = new AggregateTableEntityMapper(new RegistryTableEntityMapper(), new AttributeTableEntityMapper(), new TypeNameTableEntityMapper());
            await context.DeleteTableAsync("Musician");

            var musician1 = new Musician("John Doe", "Anonymous", "john.doe@riganti.cz");
            var musician2 = new Musician("Bill Coal", "Anonymous", "bill.coal@riganti.cz");

            await context.DeleteTableAsync(registry.GetTable<Musician>());

            context.RegisterNew(musician1);
            context.RegisterNew(musician2);

            var records = await context.SaveChangesAsync();
            Assert.Equal(2, records);

            await context.DeleteTableAsync(registry.GetTable<Musician>());
        }

        [Fact]
        public async Task GetAsync_ShouldRetrieveCreatedObject()
        {
            var ticks = DateTime.Now.Ticks;
            var musician = new Musician($"John Doe {ticks}", "Anonymous", $"john.doe{ticks}@riganti.cz");
            context.RegisterNew(musician);
            await context.SaveChangesAsync();

            var loaded = await context.GetAsync<Musician>(musician.PartitionKey, musician.RowKey);
            Assert.NotNull(loaded);
            Assert.NotEmpty(loaded.ETag);
        }

        [Fact]
        public async Task DeleteTable_WhenTableIsntFound_ShouldNotFail()
        {
            var tableName = $"xy1a{DateTime.Now.Ticks}";
            await context.DeleteTableAsync(tableName);
        }
    }
}
