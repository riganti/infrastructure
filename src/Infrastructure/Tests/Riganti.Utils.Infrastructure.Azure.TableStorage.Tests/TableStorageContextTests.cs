using System;
using Xunit;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage.Tests
{
    public class TableStorageContextTests
    {
        [Fact]
        public void CanInstantiateNewContext()
        {
            var context = CreateContext();
            Assert.NotNull(context);
        }

        [Fact]
        public void CanCreateANewTable()
        {
            var context = CreateContext();
            context.CreateTableIfNotExistsAsync("unit_test");

        }

        [Fact]
        public void TableStorageContext_ShouldCacheObjects()
        {
            throw new NotImplementedException();
        }




        #region Helpers
        private static TableStorageContext CreateContext()
        {
            var options = new CreateOwnContextTableStorageOptions();
            var context = new TableStorageContext(options);
            return context;
        }
        #endregion
    }
}
