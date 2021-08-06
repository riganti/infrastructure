using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Riganti.Utils.Infrastructure.EntityFramework.Transactions
{
    public interface IUnitOfWorkTransactionScope<TDbContext> where TDbContext : DbContext
    {
        Task<TResult> ExecuteAsync<TResult>(Func<EntityFrameworkUnitOfWork<TDbContext>, Task<TResult>> transactionBody);
        Task ExecuteAsync(Func<EntityFrameworkUnitOfWork<TDbContext>, Task> transactionBody);

        TResult Execute<TResult>(Func<EntityFrameworkUnitOfWork<TDbContext>, TResult> transactionBody);
        void Execute(Action<EntityFrameworkUnitOfWork<TDbContext>> transactionBody);
    }
}