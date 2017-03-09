namespace Riganti.Utils.Infrastructure.Azure.TableStorage
{
    public interface ITableStorageOptions
    {
        string StorageConnectionString { get; }

        TableStorageContextOptions ContextOptions { get; }
    }
}
