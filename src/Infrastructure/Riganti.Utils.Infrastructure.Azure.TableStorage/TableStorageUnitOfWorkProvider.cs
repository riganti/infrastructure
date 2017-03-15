using System;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage
{
    /// <summary>
    /// An implementation of unit of work provider in Azure Table Storage.
    /// </summary>
    public class TableStorageUnitOfWorkProvider : UnitOfWorkProviderBase
    {
        private readonly Func<ITableStorageContext> contextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableStorageUnitOfWorkProvider"/> class.
        /// </summary>
        public TableStorageUnitOfWorkProvider(IUnitOfWorkRegistry registry, Func<ITableStorageContext> contextFactory) : base(registry)
        {
            this.contextFactory = contextFactory;
        }

        /// <summary>
        /// Creates the unit of work with specified options.
        /// </summary>
        public IUnitOfWork Create(StorageContextOptions options)
        {
            return CreateCore(options);
        }

        /// <summary>
        /// Creates the unit of work.
        /// </summary>
        protected sealed override IUnitOfWork CreateUnitOfWork(object parameter)
        {
            var options = (parameter as StorageContextOptions?) ?? StorageContextOptions.ReuseParentContext;
            return CreateUnitOfWork(contextFactory, options);
        }

        /// <summary>
        /// Creates the unit of work.
        /// </summary>
        protected virtual TableStorageUnitOfWork CreateUnitOfWork(Func<ITableStorageContext> context, StorageContextOptions options)
        {
            return new TableStorageUnitOfWork(this, context, options);
        }
    }
}
