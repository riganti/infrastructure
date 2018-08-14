using Riganti.Utils.Infrastructure.Core;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
    public class EntityFrameworkUnitOfWork<TDbContext> : UnitOfWorkBase, ICheckChildCommitUnitOfWork
        where TDbContext : DbContext
    {
        private readonly bool hasOwnContext;

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
            Parent = unitOfWorkProvider.GetCurrent();

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
        /// Gets the <see cref="DbContext" />.
        /// </summary>
        public TDbContext Context { get; }

        /// <inheritdoc cref="ICheckChildCommitUnitOfWork.Parent"/>
        public IUnitOfWork Parent { get; }

        /// <inheritdoc cref="ICheckChildCommitUnitOfWork.CommitRequested"/>
        public bool CommitRequested { get; private set; }

        /// <summary>
        /// Commits this instance when we have to. Skip and request from parent, if we don't own the context.
        /// </summary>
        public override void Commit()
        {
            if (HasOwnContext())
            {
                CommitRequested = false;
                base.Commit();
            }
            else
            {
                TryRequestParentCommit();
            }
        }

        /// <summary>
        /// Commits this instance when we have to. Skip and request from parent, if we don't own the context.
        /// </summary>
        public override Task CommitAsync()
        {
            if (HasOwnContext())
            {
                CommitRequested = false;
                return base.CommitAsync();
            }
            else
            {
                TryRequestParentCommit();
            }

            return Task.FromResult(true);
        }

        /// <inheritdoc cref="ICheckChildCommitUnitOfWork.RequestCommit"/>
        public void RequestCommit()
        {
            CommitRequested = true;
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

                if (CommitRequested)
                {
                    throw new InvalidOperationException("Some of the unit of works requested commit! Ensure that commit is called on root unit of work as well.");
                }
            }
            else if (CommitRequested)
            {
                TryRequestParentCommit();
            }
        }

        private void TryRequestParentCommit()
        {
            if (Parent is ICheckChildCommitUnitOfWork uow)
            {
                uow.RequestCommit();
            }
        }

        private bool HasOwnContext()
        {
            return hasOwnContext;
        }
    }
}