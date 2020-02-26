using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Riganti.Utils.Infrastructure.EntityFrameworkCore.Transactions
{
	public interface IUnitOfWorkTransactionScope<TDbContext> where TDbContext : DbContext
	{
		Task<UnitOfWorkTransactionScopeResult> ExecuteAsync(Func<EntityFrameworkUnitOfWork<TDbContext>, Task> transactionBody);
		UnitOfWorkTransactionScopeResult Execute(Action<EntityFrameworkUnitOfWork<TDbContext>> transactionBody);
	}
}