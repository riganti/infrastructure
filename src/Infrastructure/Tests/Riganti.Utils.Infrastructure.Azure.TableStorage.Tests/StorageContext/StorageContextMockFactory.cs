using Moq;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage.Tests.StorageContext
{
    internal class StorageContextMockFactory
    {
        internal Mock<TableStorageContext> CreateStorageContextMock(IStorageOptions options = null)
        {
            if (options == null)
                options = new CreateOwnContextTableStorageOptions();
            var storageContextMock = new Mock<TableStorageContext>(options, null) {CallBase = false};
            return storageContextMock;
        }
    }
}