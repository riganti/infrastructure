using System.Threading;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Moq;
using Moq.Protected;
using Riganti.Utils.Infrastructure.Azure.TableStorage.Tests.Entities;
using Riganti.Utils.Infrastructure.Azure.TableStorage.Tests.StorageContext;
using Riganti.Utils.Infrastructure.Core;
using Xunit;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage.Tests.Repository
{
    public class TableStorageRepositoryTests
    {
        private readonly IUnitOfWorkRegistry uowRegistry;

        public TableStorageRepositoryTests()
        {
            uowRegistry = new ThreadLocalUnitOfWorkRegistry();
        }

        [Fact]
        public void InitializeNew_Should_SetPartitionKeyAndRowKey()
        {
            var mockContext = new StorageContextMockFactory().CreateStorageContextMock();
            var provider = new TableStorageUnitOfWorkProvider(uowRegistry, () => mockContext.Object);
            var repository = new TableStorageRepository<Musician>(provider, new LocalDateTimeProvider());
            var employee = repository.InitializeNew("Doe", "john.doe@riganti.cz");
            Assert.Equal("Doe", employee.PartitionKey);
            Assert.Equal("john.doe@riganti.cz", employee.RowKey);
        }

        [Fact]
        public void InitializeNewQuery_Should_CreateNewTableQuery()
        {
            var mockContext = new StorageContextMockFactory().CreateStorageContextMock();
            var provider = new TableStorageUnitOfWorkProvider(uowRegistry, () => mockContext.Object);
            var repository = new TableStorageRepository<Musician>(provider, new LocalDateTimeProvider());
            var query = repository.InitializeNewQuery();
            Assert.NotNull(query);
        }

        
    }
}
