namespace Riganti.Utils.Infrastructure.Azure.TableStorage.Tests.StorageContext
{
    public class CreateOwnContextTableStorageOptions : IStorageOptions
    {
        public string StorageConnectionString => "UseDevelopmentStorage=true";
        public StorageContextOptions ContextOptions => StorageContextOptions.AlwaysCreateOwnContext;
    }

    public class ReuseParentContextTableStorageOptions : IStorageOptions
    {
        public string StorageConnectionString => "UseDevelopmentStorage=true";
        public StorageContextOptions ContextOptions => StorageContextOptions.ReuseParentContext;
    }
}
