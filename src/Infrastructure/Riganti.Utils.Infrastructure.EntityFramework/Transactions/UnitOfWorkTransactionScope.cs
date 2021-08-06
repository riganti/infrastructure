using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;

namespace Riganti.Utils.Infrastructure.EntityFramework.Transactions
{
    public class UnitOfWorkTransactionScope : UnitOfWorkTransactionScope<DbContext>
    {
        public UnitOfWorkTransactionScope(IEntityFrameworkUnitOfWorkProvider<DbContext> unitOfWorkProvider, IsolationLevel isolationLevel) : base(unitOfWorkProvider, isolationLevel)
        {
        }
    }

    public class UnitOfWorkTransactionScope<TDbContext> : IUnitOfWorkTransactionScope<TDbContext>
        where TDbContext : DbContext
    {
        private readonly IEntityFrameworkUnitOfWorkProvider<TDbContext> unitOfWorkProvider;
        private readonly IsolationLevel isolationLevel;

        public UnitOfWorkTransactionScope(IEntityFrameworkUnitOfWorkProvider<TDbContext> unitOfWorkProvider)
            : this(unitOfWorkProvider, IsolationLevel.ReadCommitted)
        {
        }

        public UnitOfWorkTransactionScope(IEntityFrameworkUnitOfWorkProvider<TDbContext> unitOfWorkProvider, IsolationLevel isolationLevel)
        {
            this.unitOfWorkProvider = unitOfWorkProvider;
            this.isolationLevel = isolationLevel;
        }

        public async Task<TResult> ExecuteAsync<TResult>(Func<EntityFrameworkUnitOfWork<TDbContext>, Task<TResult>> transactionBody)
        {
            using (var uow = (EntityFrameworkUnitOfWork<TDbContext>)unitOfWorkProvider.Create(DbContextOptions.AlwaysCreateOwnContext))
            using (var trans = uow.Context.Database.BeginTransaction(isolationLevel))
            {
                var committed = false;
                TResult result;

                try
                {
                    uow.IsInTransaction = true;

                    result = await transactionBody(uow);

                    if (uow.RollbackRequested)
                    {
                        // user caught the exception, re-throw it here
                        throw new RollbackRequestedException();
                    }

                    if (uow.CommitsCount > 0 && !uow.CommitPending)
                    {
                        trans.Commit();
                        AfterCommit();
                        committed = true;
                    }
                }
                catch (Exception e)
                {
                    if (e is RollbackRequestedException)
                    {
                        // someone called rollback, set result to default and continue to finally
                        result = default;
                    }
                    else
                    {
                        // other than rollback exception occured, need to throw it up
                        throw;
                    }
                }
                finally
                {
                    if (!committed)
                    {
                        trans.Rollback();
                        AfterRollback();
                    }
                }

                return result;
            }
        }

        protected internal virtual void AfterRollback()
        {
        }

        protected internal virtual void AfterCommit()
        {
        }

        public Task ExecuteAsync(Func<EntityFrameworkUnitOfWork<TDbContext>, Task> transactionBody) =>
            ExecuteAsync<object>(async uow =>
            {
                await transactionBody(uow);
                return default;
            });

        public TResult Execute<TResult>(Func<EntityFrameworkUnitOfWork<TDbContext>, TResult> transactionBody) =>
            ExecuteAsync(uow => Task.FromResult(transactionBody(uow)))
                .GetAwaiter()
                .GetResult();

        public void Execute(Action<EntityFrameworkUnitOfWork<TDbContext>> transactionBody)
            => Execute<object>(uow =>
            {
                transactionBody(uow);
                return default;
            });
    }
}