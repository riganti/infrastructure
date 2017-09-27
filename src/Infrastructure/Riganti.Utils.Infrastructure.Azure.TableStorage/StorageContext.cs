using System;
using Microsoft.WindowsAzure.Storage;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage
{
    public abstract class StorageContext
    {
        protected readonly IStorageOptions Options;

        protected StorageContext(IStorageOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            Options = options;
        }

        public virtual CloudStorageAccount StorageAccount => new Lazy<CloudStorageAccount>(
            () => CloudStorageAccount.Parse(Options.StorageConnectionString)).Value;

        // todo: finish abstraction for other storage types
    }
}