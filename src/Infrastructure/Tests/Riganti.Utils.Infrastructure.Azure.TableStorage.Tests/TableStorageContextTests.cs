using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Moq;
using Moq.Protected;
using Riganti.Utils.Infrastructure.Azure.TableStorage.Tests.Entities;
using Xunit;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage.Tests
{
    public class TableStorageContextTests
    {
        [Fact]
        public void Context_CanInstantiateNewContext_WithOptions()
        {
            var context = CreateContext();
            Assert.NotNull(context);
        }

        [Fact]
        public async Task GetOrCreateTableAsync_ShouldNot_ThrowException()
        {
            var context = CreateContext();
            await context.GetOrCreateTableAsync("xy1a");
        }

        [Fact]
        public void RegisterNew_Should_AddRecordInEntityMap()
        {
            var context = CreateContext();
            var musician = new Musician("John Doe", "Anonymous", "john.doe@riganti.cz");
            Assert.DoesNotContain(musician, context.Entities);
            context.RegisterNew(musician, "Musicians");
            Assert.Contains(musician, context.Entities);
        }

        [Fact]
        public void RegisterNew_WhenObjectIsInCleanEntities_ShouldThrowException()
        {
            var context = CreateContext();
            var musician = new Musician("John Doe", "Anonymous", "john.doe@riganti.cz");
            Assert.DoesNotContain(musician, context.Entities);
            context.RegisterClean(musician, "Musicians");
            Assert.Contains(musician, context.Entities);
            Assert.Throws<InvalidOperationException>(() => context.RegisterNew(musician, "Musicians"));
        }

        [Fact]
        public void RegisterNew_WhenObjectIsInDirtyEntities_ShouldThrowException()
        {
            var context = CreateContext();
            var musician = new Musician("John Doe", "Anonymous", "john.doe@riganti.cz");
            Assert.DoesNotContain(musician, context.Entities);
            context.RegisterDirty(musician, "Musicians");
            Assert.Contains(musician, context.Entities);
            Assert.Throws<InvalidOperationException>(() => context.RegisterNew(musician, "Musicians"));
        }

        [Fact]
        public void RegisterNew_WhenObjectIsInRemovedEntities_ShouldThrowException()
        {
            var context = CreateContext();
            var musician = new Musician("John Doe", "Anonymous", "john.doe@riganti.cz");
            Assert.DoesNotContain(musician, context.Entities);
            context.RegisterRemoved(musician, "Musicians");
            Assert.Contains(musician, context.Entities);
            Assert.Throws<InvalidOperationException>(() => context.RegisterNew(musician, "Musicians"));
        }

        [Fact]
        public void RegisterClean_Should_AddRecordInEntityMap()
        {
            var context = CreateContext();
            var musician = new Musician("John Doe", "Anonymous", "john.doe@riganti.cz");
            Assert.DoesNotContain(musician, context.Entities);
            context.RegisterClean(musician, "Musicians");
            Assert.Contains(musician, context.Entities);
        }

        [Fact]
        public void RegisterClean_WhenObjectIsInNewEntities_ShouldThrowException()
        {
            var context = CreateContext();
            var musician = new Musician("John Doe", "Anonymous", "john.doe@riganti.cz");
            Assert.DoesNotContain(musician, context.Entities);
            context.RegisterNew(musician, "Musicians");
            Assert.Contains(musician, context.Entities);
            Assert.Throws<InvalidOperationException>(() => context.RegisterClean(musician, "Musicians"));
        }

        [Fact]
        public void RegisterClean_WhenObjectIsInDirtyEntities_ShouldThrowException()
        {
            var context = CreateContext();
            var musician = new Musician("John Doe", "Anonymous", "john.doe@riganti.cz");
            Assert.DoesNotContain(musician, context.Entities);
            context.RegisterDirty(musician, "Musicians");
            Assert.Contains(musician, context.Entities);
            Assert.Throws<InvalidOperationException>(() => context.RegisterClean(musician, "Musicians"));
        }

        [Fact]
        public void RegisterClean_WhenObjectIsInRemovedEntities_ShouldThrowException()
        {
            var context = CreateContext();
            var musician = new Musician("John Doe", "Anonymous", "john.doe@riganti.cz");
            Assert.DoesNotContain(musician, context.Entities);
            context.RegisterRemoved(musician, "Musicians");
            Assert.Contains(musician, context.Entities);
            Assert.Throws<InvalidOperationException>(() => context.RegisterClean(musician, "Musicians"));
        }

        [Fact]
        public void RegisterDirty_Should_AddRecordInEntityMap()
        {
            var context = CreateContext();
            var musician = new Musician("John Doe", "Anonymous", "john.doe@riganti.cz");
            Assert.DoesNotContain(musician, context.Entities);
            context.RegisterDirty(musician, "Musicians");
            Assert.Contains(musician, context.Entities);
        }

        [Fact]
        public void RegisterDirty_WhenObjectIsInRemovedEntities_ShouldThrowException()
        {
            var context = CreateContext();
            var musician = new Musician("John Doe", "Anonymous", "john.doe@riganti.cz");
            Assert.DoesNotContain(musician, context.Entities);
            context.RegisterRemoved(musician, "Musicians");
            Assert.Contains(musician, context.Entities);
            Assert.Throws<InvalidOperationException>(() => context.RegisterDirty(musician, "Musicians"));
        }

        [Fact]
        public void RegisterRemoved_Should_AddRecordInEntityMap()
        {
            var context = CreateContext();
            var musician = new Musician("John Doe", "Anonymous", "john.doe@riganti.cz");
            Assert.DoesNotContain(musician, context.Entities);
            context.RegisterRemoved(musician, "Musicians");
            Assert.Contains(musician, context.Entities);
        }

        [Fact]
        public async Task SaveChanges_Should_ReturnNumberOfProcessedRecords()
        {
            var context = CreateContext();
            var musician1 = new Musician("John Doe", "Anonymous", "john.doe@riganti.cz");
            var musician2 = new Musician("Bill Coal", "Anonymous", "bill.coal@riganti.cz");

            context.RegisterNew(musician1, "Musicians");
            context.RegisterNew(musician2, "Musicians");
            var records = await context.SaveChangesAsync();
            Assert.Equal(2, records);
        }

        //[Fact]
        //public async Task SaveChangesAsync_Should_CallInsertUpdateAndDelete()
        //{
        //    var mock = CreateMockContext();
        //    // mock.Protected().Setup("Test", ItExpr.IsAny<CancellationToken>()).Verifiable();
        //    //mock.Protected().Setup("InsertNewEntitiesAsync", ItExpr.IsAny<CancellationToken>()).Verifiable("InsertNewEntitiesAsync wasn't called.");
        //    //mock.Protected().Setup("UpdateDirtyEntitiesAsync", ItExpr.IsAny<CancellationToken>()).Verifiable("UpdateDirtyEntitiesAsync wasn't called.");
        //    //mock.Protected().Setup("DeleteRemovedEntitiesAsync", ItExpr.IsAny<CancellationToken>()).Verifiable("DeleteRemovedEntitiesAsync wasn't called.");

        //    await mock.Object.SaveChangesAsync(CancellationToken.None);
        //    mock.Verify();
        //}

        


        #region Helpers

        private static Mock<TableStorageContext> CreateMockContext()
        {
            var options = new CreateOwnContextTableStorageOptions();
            var mock = new Mock<TableStorageContext>(options);
            return mock;
        }


        private static TableStorageContext CreateContext()
        {
            var options = new CreateOwnContextTableStorageOptions();
            var context = new TableStorageContext(options);
            return context;
        }
        #endregion
    }
}
