using System;
using System.Threading;
using System.Threading.Tasks;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage
{
    public class TableStorageUnitOfWork : UnitOfWorkBase
    {
        private readonly bool hasOwnContext;

        public ITableStorageContext Context { get; }

        public TableStorageUnitOfWork(IUnitOfWorkProvider provider, Func<ITableStorageContext> contextFactory, StorageContextOptions options)
        {
            if (options == StorageContextOptions.ReuseParentContext)
            {
                var parentUow = provider.GetCurrent() as TableStorageUnitOfWork;
                if (parentUow != null)
                {
                    Context = parentUow.Context;
                    return;
                }
            }

            Context = contextFactory();
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
        public override async Task CommitAsync()
        {
            if (HasOwnContext())
            {
                await base.CommitAsync();
            }
        }

        protected override void CommitCore()
        {
            var task = Context.SaveChangesAsync(CancellationToken.None);
            Task.WaitAll(task);
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
        /// Tries to get the <see cref="ITableStorageContext"/> in the current scope.
        /// </summary>
        public static ITableStorageContext TryGetTableStorageContext(IUnitOfWorkProvider provider)
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
