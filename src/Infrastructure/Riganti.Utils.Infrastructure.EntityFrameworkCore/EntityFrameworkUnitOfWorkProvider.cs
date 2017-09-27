using System;
using Microsoft.EntityFrameworkCore;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.EntityFrameworkCore
{
    /// <summary>
    /// An implementation of unit of work provider in Entity Framework.
    /// </summary>
    public class EntityFrameworkUnitOfWorkProvider : EntityFrameworkUnitOfWorkProvider<DbContext>
    {
        public EntityFrameworkUnitOfWorkProvider(IUnitOfWorkRegistry registry, Func<DbContext> dbContextFactory) : base(registry, dbContextFactory)
        {
        }
    }

    /// <summary>
    /// An implementation of unit of work provider in Entity Framework.
    /// </summary>
    public class EntityFrameworkUnitOfWorkProvider<TDbContext> : UnitOfWorkProviderBase, IEntityFrameworkUnitOfWorkProvider<TDbContext>
        where TDbContext : DbContext
    {
        private readonly Func<TDbContext> dbContextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkUnitOfWorkProvider"/> class.
        /// </summary>
        public EntityFrameworkUnitOfWorkProvider(IUnitOfWorkRegistry registry, Func<TDbContext> dbContextFactory) : base(registry)
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
        protected virtual EntityFrameworkUnitOfWork<TDbContext> CreateUnitOfWork(Func<TDbContext> contextFactory, DbContextOptions options)
        {
            return new EntityFrameworkUnitOfWork<TDbContext>(this, contextFactory, options);
        }
    }
}