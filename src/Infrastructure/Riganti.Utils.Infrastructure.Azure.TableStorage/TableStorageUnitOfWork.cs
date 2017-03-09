using System;
using System.Threading;
using System.Threading.Tasks;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage
{
    public class TableStorageUnitOfWork : UnitOfWorkBase
    {
        private readonly bool hasOwnContext;

        public TableStorageContext Context { get; }

        public TableStorageUnitOfWork(IUnitOfWorkProvider provider, Func<TableStorageContext> contextFactory, TableStorageContextOptions options)
        {
            if (options == TableStorageContextOptions.ReuseParentContext)
            {
                var parentUow = provider.GetCurrent() as TableStorageUnitOfWork;
                if (parentUow != null)
                {
                    this.Context = parentUow.Context;
                    return;
                }
            }

            this.Context = contextFactory();
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

        protected override void CommitCore()
        {
            throw new NotSupportedException("This method isn't supported. Please use CommitAsyncCore instead.");
        }

        protected override Task CommitAsyncCore(CancellationToken cancellationToken)
        {
            return Context.SaveChangesAsync(cancellationToken);
        }

        protected override void DisposeCore()
        {
        }

        private bool HasOwnContext()
        {
            return hasOwnContext;
        }

        /// <summary>
        /// Tries to get the <see cref="TableStorageContext"/> in the current scope.
        /// </summary>
        public static TableStorageContext TryGetTableStorageContext(IUnitOfWorkProvider provider)
        {
            var uow = provider.GetCurrent() as TableStorageUnitOfWork;
            if (uow == null)
            {
                throw new InvalidOperationException("The TableStorageRepository must be used in a unit of work of type TableStorageUnitOfWork!");
            }
            return uow.Context;
        }
    }
}
