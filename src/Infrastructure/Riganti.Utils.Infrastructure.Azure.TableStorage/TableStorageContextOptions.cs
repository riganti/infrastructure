namespace Riganti.Utils.Infrastructure.Azure.TableStorage
{
    /// <summary>
    /// Options for Azure storage behavior in <see cref="StorageContext"/> instance.
    /// </summary>
    public enum StorageContextOptions
    {
        /// <summary>
        /// If we are already in another unit of work, its TableStorageContext will be reused and the changes will be committed after the outer unit of work commits.
        /// </summary>
        ReuseParentContext = 0,

        /// <summary>
        /// This unit of work is standalone, has its own TableStorageContext and doesn't depend on any other unit of work instances.
        /// </summary>
        AlwaysCreateOwnContext = 1
    }
}
