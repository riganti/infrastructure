using System;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage
{
    /// <summary>
    /// An implementation of unit of work provider in Azure Table Storage.
    /// </summary>
    public class TableStorageUnitOfWorkProvider : UnitOfWorkProviderBase
    {
        private readonly Func<TableStorageContext> contextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableStorageUnitOfWorkProvider"/> class.
        /// </summary>
        public TableStorageUnitOfWorkProvider(IUnitOfWorkRegistry registry, Func<TableStorageContext> contextFactory) : base(registry)
        {
            this.contextFactory = contextFactory;
        }

        /// <summary>
        /// Creates the unit of work with specified options.
        /// </summary>
        public IUnitOfWork Create(TableStorageContextOptions options)
        {
            return CreateCore(options);
        }

        /// <summary>
        /// Creates the unit of work.
        /// </summary>
        protected sealed override IUnitOfWork CreateUnitOfWork(object parameter)
        {
            var options = (parameter as TableStorageContextOptions?) ?? TableStorageContextOptions.ReuseParentContext;
            return CreateUnitOfWork(contextFactory, options);
        }

        /// <summary>
        /// Creates the unit of work.
        /// </summary>
        protected virtual TableStorageUnitOfWork CreateUnitOfWork(Func<TableStorageContext> context, TableStorageContextOptions options)
        {
            return new TableStorageUnitOfWork(this, context, options);
        }
    }
}
