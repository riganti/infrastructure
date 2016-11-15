using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.EntityFramework
{
    /// <summary>
    /// An implementation of unit of work in Entity ramework.
    /// </summary>
    public class EntityFrameworkUnitOfWork : UnitOfWorkBase
    {
        private readonly bool hasOwnContext;

        /// <summary>
        /// Gets the <see cref="DbContext"/>.
        /// </summary>
        public DbContext Context { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkUnitOfWork"/> class.
        /// </summary>
        public EntityFrameworkUnitOfWork(IUnitOfWorkProvider provider, Func<DbContext> dbContextFactory, DbContextOptions options)
        {
            if (options == DbContextOptions.ReuseParentContext)
            {
                var parentUow = provider.GetCurrent() as EntityFrameworkUnitOfWork;
                if (parentUow != null)
                {
                    this.Context = parentUow.Context;
                    return;
                }
            }

            this.Context = dbContextFactory();
            hasOwnContext = true;
        }


        /// <summary>
        /// Commits this instance when we have to.
        /// </summary>
        public override void Commit()
        {
            if (HasOwnContext())
            {
                base.Commit();
            }
        }
        
        /// <summary>
        /// Commits this instance when we have to.
        /// </summary>
        public override Task CommitAsync()
        {
            if (HasOwnContext())
            {
                return base.CommitAsync();
            }
            return Task.FromResult(true);
        }

        /// <summary>
        /// Commits the changes.
        /// </summary>
        protected override void CommitCore()
        {
            Context.SaveChanges();
        }

        protected override async Task CommitAsyncCore(CancellationToken cancellationToken)
        {
            await Context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Disposes the context.
        /// </summary>
        protected override void DisposeCore()
        {
            if (HasOwnContext())
            {
                Context.Dispose();
            }
        }


        private bool HasOwnContext()
        {
            return hasOwnContext;
        }

        /// <summary>
        /// Tries to get the <see cref="DbContext"/> in the current scope.
        /// </summary>
        public static DbContext TryGetDbContext(IUnitOfWorkProvider provider)
        {
            var uow = provider.GetCurrent() as EntityFrameworkUnitOfWork;
            if (uow == null)
            {
                throw new InvalidOperationException("The EntityFrameworkRepository must be used in a unit of work of type EntityFrameworkUnitOfWork!");
            }
            return uow.Context;
        }
    }
}