using System;
using System.Data.Entity;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.EntityFramework
{
    /// <summary>
    /// An implementation of unit of work provider in Entity Framework.
    /// </summary>
    public class EntityFrameworkUnitOfWorkProvider : UnitOfWorkProviderBase
    {
        private readonly Func<DbContext> dbContextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkUnitOfWorkProvider"/> class.
        /// </summary>
        public EntityFrameworkUnitOfWorkProvider(IUnitOfWorkRegistry registry, Func<DbContext> dbContextFactory) : base(registry)
        {
            this.dbContextFactory = dbContextFactory;
        }

        /// <summary>
        /// Creates the unit of work with specified options.
        /// </summary>
        public IUnitOfWork Create(DbContextOptions options)
        {
            return CreateCore(options);
        }

        /// <summary>
        /// Creates the unit of work.
        /// </summary>
        protected sealed override IUnitOfWork CreateUnitOfWork(object parameter)
        {
            var options = (parameter as DbContextOptions?) ?? DbContextOptions.ReuseParentContext;
            return CreateUnitOfWork(dbContextFactory, options);
        }

        /// <summary>
        /// Creates the unit of work.
        /// </summary>
        protected virtual EntityFrameworkUnitOfWork CreateUnitOfWork(Func<DbContext> dbContextFactory, DbContextOptions options)
        {
            return new EntityFrameworkUnitOfWork(this, dbContextFactory, options);
        }
    }
}