namespace Riganti.Utils.Infrastructure.Azure.TableStorage
{
    public interface IStorageOptions
    {
        string StorageConnectionString { get; }

        StorageContextOptions ContextOptions { get; }
    }
}
