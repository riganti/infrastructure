using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Moq;
using Moq.Protected;
using Riganti.Utils.Infrastructure.Azure.TableStorage.Tests.Entities;
using Xunit;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage.Tests.StorageContext
{
    public class TableStorageContextTests
    {

        [Fact]
        public void CanCreateMockContext()
        {
            var mockContext = new StorageContextMockFactory().CreateStorageContextMock();
            var context = mockContext.Object;
            Assert.NotNull(context);
        }

        [Fact]
        public void RegisterNew_Should_AddRecordInEntityMap()
        {
            var mockContext = new StorageContextMockFactory().CreateStorageContextMock();
            var context = mockContext.Object;
            var musician = new Musician("John Doe", "Anonymous", "john.doe@riganti.cz");
            Assert.DoesNotContain(musician, context.Entities);
            context.RegisterNew(musician);
            Assert.Contains(musician, context.Entities);
        }

        [Fact]
        public void RegisterMultipleDifferentEntitiesWithSameKeys_Should_AddRecordsInEntityMap()
        {
            var mockContext = new StorageContextMockFactory().CreateStorageContextMock();
            var context = mockContext.Object;
            var musician = new Musician("John Doe", "Anonymous", "john.doe@riganti.cz");
            var bandTeacher = new BandTeacher("John Doe", "Anonymous", "john.doe@riganti.cz");
            Assert.DoesNotContain(musician, context.Entities);
            Assert.DoesNotContain(bandTeacher, context.Entities);
            context.RegisterNew(musician);
            context.RegisterNew(bandTeacher);
            Assert.Contains(musician, context.Entities);
            Assert.Contains(bandTeacher, context.Entities);
        }

        [Fact]
        public void RegisterNew_WhenObjectIsInCleanEntities_ShouldThrowException()
        {
            var mockContext = new StorageContextMockFactory().CreateStorageContextMock();
            var context = mockContext.Object;
            var musician = new Musician("John Doe", "Anonymous", "john.doe@riganti.cz");
            Assert.DoesNotContain(musician, context.Entities);
            context.RegisterClean(musician);
            Assert.Contains(musician, context.Entities);
            Assert.Throws<InvalidOperationException>(() => context.RegisterNew(musician));
        }

        [Fact]
        public void RegisterNew_WhenObjectIsInDirtyEntities_ShouldThrowException()
        {
            var mockContext = new StorageContextMockFactory().CreateStorageContextMock();
            var context = mockContext.Object;
            var musician = new Musician("John Doe", "Anonymous", "john.doe@riganti.cz");
            Assert.DoesNotContain(musician, context.Entities);
            context.RegisterDirty(musician);
            Assert.Contains(musician, context.Entities);
            Assert.Throws<InvalidOperationException>(() => context.RegisterNew(musician));
        }

        [Fact]
        public void RegisterNew_WhenObjectIsInRemovedEntities_ShouldThrowException()
        {
            var mockContext = new StorageContextMockFactory().CreateStorageContextMock();
            var context = mockContext.Object;
            var musician = new Musician("John Doe", "Anonymous", "john.doe@riganti.cz");
            Assert.DoesNotContain(musician, context.Entities);
            context.RegisterRemoved(musician);
            Assert.Contains(musician, context.Entities);
            Assert.Throws<InvalidOperationException>(() => context.RegisterNew(musician));
        }

        [Fact]
        public void RegisterClean_Should_AddRecordInEntityMap()
        {
            var mockContext = new StorageContextMockFactory().CreateStorageContextMock();
            var context = mockContext.Object;
            var musician = new Musician("John Doe", "Anonymous", "john.doe@riganti.cz");
            Assert.DoesNotContain(musician, context.Entities);
            context.RegisterClean(musician);
            Assert.Contains(musician, context.Entities);
        }

        [Fact]
        public void RegisterClean_WhenObjectIsInNewEntities_ShouldThrowException()
        {
            var mockContext = new StorageContextMockFactory().CreateStorageContextMock();
            var context = mockContext.Object;
            var musician = new Musician("John Doe", "Anonymous", "john.doe@riganti.cz");
            Assert.DoesNotContain(musician, context.Entities);
            context.RegisterNew(musician);
            Assert.Contains(musician, context.Entities);
            Assert.Throws<InvalidOperationException>(() => context.RegisterClean(musician));
        }

        [Fact]
        public void RegisterClean_WhenObjectIsInDirtyEntities_ShouldThrowException()
        {
            var mockContext = new StorageContextMockFactory().CreateStorageContextMock();
            var context = mockContext.Object;
            var musician = new Musician("John Doe", "Anonymous", "john.doe@riganti.cz");
            Assert.DoesNotContain(musician, context.Entities);
            context.RegisterDirty(musician);
            Assert.Contains(musician, context.Entities);
            Assert.Throws<InvalidOperationException>(() => context.RegisterClean(musician));
        }

        [Fact]
        public void RegisterClean_WhenObjectIsInRemovedEntities_ShouldThrowException()
        {
            var mockContext = new StorageContextMockFactory().CreateStorageContextMock();
            var context = mockContext.Object;
            var musician = new Musician("John Doe", "Anonymous", "john.doe@riganti.cz");
            Assert.DoesNotContain(musician, context.Entities);
            context.RegisterRemoved(musician);
            Assert.Contains(musician, context.Entities);
            Assert.Throws<InvalidOperationException>(() => context.RegisterClean(musician));
        }

        [Fact]
        public void RegisterDirty_Should_AddRecordInEntityMap()
        {
            var mockContext = new StorageContextMockFactory().CreateStorageContextMock();
            var context = mockContext.Object;
            var musician = new Musician("John Doe", "Anonymous", "john.doe@riganti.cz");
            Assert.DoesNotContain(musician, context.Entities);
            context.RegisterDirty(musician);
            Assert.Contains(musician, context.Entities);
        }

        [Fact]
        public void RegisterDirty_WhenObjectIsInRemovedEntities_ShouldThrowException()
        {
            var mockContext = new StorageContextMockFactory().CreateStorageContextMock();
            var context = mockContext.Object;
            var musician = new Musician("John Doe", "Anonymous", "john.doe@riganti.cz");
            Assert.DoesNotContain(musician, context.Entities);
            context.RegisterRemoved(musician);
            Assert.Contains(musician, context.Entities);
            Assert.Throws<InvalidOperationException>(() => context.RegisterDirty(musician));
        }

        [Fact]
        public void RegisterRemoved_Should_AddRecordInEntityMap()
        {
            var mockContext = new StorageContextMockFactory().CreateStorageContextMock();
            var context = mockContext.Object;
            var musician = new Musician("John Doe", "Anonymous", "john.doe@riganti.cz");
            Assert.DoesNotContain(musician, context.Entities);
            context.RegisterRemoved(musician);
            Assert.Contains(musician, context.Entities);
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldCallInsertNewEntitiesAsync()
        {
            var mockContext = new StorageContextMockFactory().CreateStorageContextMock();
            var context = mockContext.Object;
            await context.SaveChangesAsync();
            mockContext.Protected().Verify("InsertNewEntitiesAsync", Times.Once(),
                ItExpr.IsAny<CancellationToken>(),
                ItExpr.IsAny<TableRequestOptions>(),
                ItExpr.IsAny<OperationContext>());
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldCallUpdateDirtyEntitiesAsync()
        {
            var mockContext = new StorageContextMockFactory().CreateStorageContextMock();
            var context = mockContext.Object;
            await context.SaveChangesAsync();
            mockContext.Protected().Verify("UpdateDirtyEntitiesAsync", Times.Once(),
                ItExpr.IsAny<CancellationToken>(),
                ItExpr.IsAny<TableRequestOptions>(),
                ItExpr.IsAny<OperationContext>());
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldCallDeleteRemoedEntitiesAsync()
        {
            var mockContext = new StorageContextMockFactory().CreateStorageContextMock();
            var context = mockContext.Object;
            await context.SaveChangesAsync();
            mockContext.Protected().Verify("DeleteRemovedEntitiesAsync", Times.Once(),
                ItExpr.IsAny<CancellationToken>(),
                ItExpr.IsAny<TableRequestOptions>(),
                ItExpr.IsAny<OperationContext>());
        }

        [Fact]
        public async Task Get_Should_RetrieveEntityFromEntityMap()
        {
            var mockContext = new StorageContextMockFactory().CreateStorageContextMock();
            var context = mockContext.Object;
            var musician = new Musician("John Doe", "Anonymous", "john.doe@riganti.cz");
            context.RegisterNew(musician);
            var loadedMusician = await context.GetAsync<Musician>(musician.PartitionKey, musician.RowKey, CancellationToken.None);
            Assert.Equal(musician, loadedMusician, new TableEntityEqualityComparer<Musician>());
        }
    }
}
