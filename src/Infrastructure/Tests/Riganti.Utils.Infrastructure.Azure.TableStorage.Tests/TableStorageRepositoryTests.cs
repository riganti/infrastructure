using System;
using System.Collections.Generic;
using System.Text;
using Riganti.Utils.Infrastructure.Azure.TableStorage.Tests.Entities;
using Riganti.Utils.Infrastructure.Core;
using Xunit;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage.Tests
{
    public class TableStorageRepositoryTests
    {
        [Fact]
        public void InstantiateNew_Should_SetPartitionKeyAndRowKey()
        {
            var repo = CreateEmployeeRepository();
            var employee = repo.InitializeNew("Doe", "john.doe@riganti.cz");
            Assert.Equal("Doe", employee.PartitionKey);
            Assert.Equal("john.doe@riganti.cz", employee.RowKey);
        }




        #region Helpers

        private ITableStorageRepository<Musician> CreateEmployeeRepository()
        {
            var provider = CreateProvider();
            return new TableStorageRepository<Musician>(provider, new LocalDateTimeProvider());
        }

        private IUnitOfWorkProvider CreateProvider()
        {
            var registry = new ThreadLocalUnitOfWorkRegistry(); 
            return new TableStorageUnitOfWorkProvider(registry, () => new TableStorageContext(new CreateOwnContextTableStorageOptions()));
        }
        #endregion
    }
}
