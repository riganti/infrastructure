using Microsoft.EntityFrameworkCore;
using Riganti.Utils.Infrastructure.Core;
using System;
using System.Threading;
using System.Threading.Tasks;
using Riganti.Utils.Infrastructure.EntityFrameworkCore.Transactions;

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
    /// An implementation of unit of work in Entity Framework.
    /// </summary>
    public class EntityFrameworkUnitOfWork<TDbContext> : UnitOfWorkBase, ICheckChildCommitUnitOfWork where TDbContext : DbContext
    {
        private readonly bool hasOwnContext;
        private bool? isInTransaction;

        /// <summary>
        /// Flag if UnitOfWork is in a tree that originates in UnitOfWorkTransactionScope.
        /// </summary>
	    public bool IsInTransaction
        {
            get => isInTransaction ?? (Parent as EntityFrameworkUnitOfWork<TDbContext>)?.IsInTransaction ?? false;
            internal set
            {
                if (isInTransaction.HasValue)
                {
                    throw new Exception("Cannot change IsInTransaction once set.");
                }
                isInTransaction = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="DbContext" />.
        /// </summary>
        public TDbContext Context { get; }

        /// <inheritdoc cref="ICheckChildCommitUnitOfWork.Parent" />
        public IUnitOfWork Parent { get; }

        /// <inheritdoc cref="ICheckChildCommitUnitOfWork.CommitPending" />
        public bool CommitPending { get; private set; }

        public bool RollbackRequested { get; private set; }

        /// <summary>
        /// Count of full CommitCore calls.
        /// Informs that there were changes being saved.
        /// Used to detect whether CommitTransaction is required.
        /// </summary>
        public int CommitsCount { get; private set; }

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
            CommitsCount = 0;

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
        /// Commits this instance when we have to. Skip and request from parent, if we don't own the context.
        /// </summary>
        public override void Commit()
        {
            if (CanCommit())
            {
                CommitPending = false;
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
        public override Task CommitAsync(CancellationToken cancellationToken)
        {
            if (CanCommit())
            {
                CommitPending = false;
                return base.CommitAsync(cancellationToken);
            }

            TryRequestParentCommit();

            return Task.CompletedTask;
        }

        public override async Task CommitAsync()
        {
            await CommitAsync(default(CancellationToken));
        }

        /// <inheritdoc cref="ICheckChildCommitUnitOfWork.RequestCommit" />
        public void RequestCommit()
        {
            CommitPending = true;
        }

        /// <summary>
        /// Throws <see cref="RollbackRequestedException"/> which is handled in the <see cref="UnitOfWorkTransactionScope{TDbContext}"/> Execute or ExecuteAsync call.
        /// </summary>
        /// <exception cref="RollbackRequestedException"></exception>
        /// <exception cref="Exception">In case of calling this method when IsInTransaction is false</exception>
        public void RollbackTransaction()
        {
            if (IsInTransaction)
            {
	            RollbackRequested = true;

	            if (!HasOwnContext() && Parent is EntityFrameworkUnitOfWork<TDbContext> parentUow)
	            {
		            parentUow.RollbackRequested = true;
	            }

                throw new RollbackRequestedException();
            }
            else
            {
                throw new Exception("UnitOfWork - There is no transaction to roll back!");
            }
        }

        private void IncrementCommitsCount()
        {
            CommitsCount++;

            if (!HasOwnContext() && Parent is EntityFrameworkUnitOfWork<TDbContext> parentUow)
            {
	            parentUow.IncrementCommitsCount();
            }
        }

        /// <summary>
        /// Commits the changes.
        /// </summary>
        protected override void CommitCore()
        {
            IncrementCommitsCount();
            Context.SaveChanges();
        }

        /// <summary>
        /// Commits the changes.
        /// </summary>
        protected override async Task CommitAsyncCore(CancellationToken cancellationToken)
        {
            IncrementCommitsCount();
            await Context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Disposes the context.
        /// </summary>
        protected override void DisposeCore()
        {
            if (HasOwnContext())
            {
                Context?.Dispose();

                if (CommitPending)
                {
                    throw new ChildCommitPendingException();
                }
            }
            else if (CommitPending)
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

        /// <summary>
        /// Checks whether immediate commit is allowed at the moment.
        /// </summary>
        private bool CanCommit() => HasOwnContext() || IsInTransaction;
    }
}