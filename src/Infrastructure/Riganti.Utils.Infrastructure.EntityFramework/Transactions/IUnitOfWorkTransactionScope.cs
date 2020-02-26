using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Riganti.Utils.Infrastructure.EntityFramework.Transactions
{
	public interface IUnitOfWorkTransactionScope<TDbContext> where TDbContext : DbContext
	{
		Task<UnitOfWorkTransactionScopeResult> ExecuteAsync(Func<EntityFrameworkUnitOfWork<TDbContext>, Task> transactionBody);
		UnitOfWorkTransactionScopeResult Execute(Action<EntityFrameworkUnitOfWork<TDbContext>> transactionBody);
	}
}