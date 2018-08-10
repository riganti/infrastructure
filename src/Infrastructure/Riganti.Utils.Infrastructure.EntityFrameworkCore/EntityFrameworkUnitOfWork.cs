using Microsoft.EntityFrameworkCore;
using Riganti.Utils.Infrastructure.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Riganti.Utils.Infrastructure.EntityFrameworkCore
{
    /// <summary>
    /// An implementation of unit of work in Entity Framework.
    /// </summary>
    public class EntityFrameworkUnitOfWork : EntityFrameworkUnitOfWork<DbContext>
    {
        public EntityFrameworkUnitOfWork(IUnitOfWorkProvider unitOfWorkProvider, Func<DbContext> dbContextFactory, DbContextOptions options)
            : base(unitOfWorkProvider, dbContextFactory, options)
        {
        }

        /// <summary>
        /// Tries to get the <see cref="DbContext" /> in the current scope.
        /// </summary>
        public static DbContext TryGetDbContext(IUnitOfWorkProvider unitOfWorkProvider)
        {
            return TryGetDbContext<DbContext>(unitOfWorkProvider);
        }

        /// <summary>
        /// Tries to get the <see cref="DbContext" /> in the current scope.
        /// </summary>
        public static TDbContext TryGetDbContext<TDbContext>(IUnitOfWorkProvider unitOfWorkProvider)
            where TDbContext : DbContext
        {
            var index = 0;
            var uow = unitOfWorkProvider.GetCurrent(index);
            while (uow != null)
            {
                if (uow is EntityFrameworkUnitOfWork<TDbContext> efuow)
                {
                    return efuow.Context;
                }

                index++;
                uow = unitOfWorkProvider.GetCurrent(index);
            }

            return null;
        }
    }

    /// <summary>
    /// An implementation of unit of work in Entity ramework.
    /// </summary>
    public class EntityFrameworkUnitOfWork<TDbContext> : UnitOfWorkBase
        where TDbContext : DbContext
    {
        private readonly ICheckChildCommitUnitOfWorkProvider checkChildCommitProvider;
        private readonly bool hasOwnContext;

        /// <summary>
        /// Gets the <see cref="DbContext" />.
        /// </summary>
        public TDbContext Context { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkUnitOfWork{TDbContext}" /> class.
        /// </summary>
        public EntityFrameworkUnitOfWork(IEntityFrameworkUnitOfWorkProvider<TDbContext> unitOfWorkProvider, Func<TDbContext> dbContextFactory, DbContextOptions options)
            : this((IUnitOfWorkProvider)unitOfWorkProvider, dbContextFactory, options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkUnitOfWork{TDbContext}" /> class.
        /// </summary>
        protected EntityFrameworkUnitOfWork(IUnitOfWorkProvider unitOfWorkProvider, Func<TDbContext> dbContextFactory, DbContextOptions options)
        {
            this.checkChildCommitProvider = unitOfWorkProvider as ICheckChildCommitUnitOfWorkProvider;

            if (options == DbContextOptions.ReuseParentContext)
            {
                var parentContext = EntityFrameworkUnitOfWork.TryGetDbContext<TDbContext>(unitOfWorkProvider);
                if (parentContext != null)
                {
                    Context = parentContext;
                    return;
                }
            }

            Context = dbContextFactory();
            hasOwnContext = true;
        }

        /// <summary>
        /// Commits this instance when we have to. Report to provider, whether commit went through or
        /// got skipped.
        /// </summary>
        public override void Commit()
        {
            if (HasOwnContext())
            {
                checkChildCommitProvider?.CommitAttempt(true);
                base.Commit();
            }
            else
            {
                checkChildCommitProvider?.CommitAttempt(false);
            }
        }

        /// <summary>
        /// Commits this instance when we have to. Report to provider, whether commit went through or
        /// got skipped.
        /// </summary>
        public override Task CommitAsync()
        {
            if (HasOwnContext())
            {
                checkChildCommitProvider?.CommitAttempt(true);
                return base.CommitAsync();
            }

            checkChildCommitProvider?.CommitAttempt(false);
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
                if (checkChildCommitProvider?.CommitRequested == true)
                {
                    // Clear the commit requested flag on unit of work provider.
                    // Reason: just in case that user catches following exception and keeps using the same
                    // unit of work provider - origin of exception would be hard to find.
                    checkChildCommitProvider.CommitAttempt(true);

                    throw new InvalidOperationException("Some of the unit of works requested commit! Ensure that commit is called on root unit of work as well.");
                }

                Context.Dispose();
            }
        }

        private bool HasOwnContext()
        {
            return hasOwnContext;
        }
    }
}