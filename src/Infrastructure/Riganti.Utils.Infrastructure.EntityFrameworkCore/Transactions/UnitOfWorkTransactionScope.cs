using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Riganti.Utils.Infrastructure.EntityFrameworkCore.Transactions
{
	public class UnitOfWorkTransactionScope : UnitOfWorkTransactionScope<DbContext>
	{
		public UnitOfWorkTransactionScope(IEntityFrameworkUnitOfWorkProvider<DbContext> unitOfWorkProvider) : base(unitOfWorkProvider)
		{
		}
	}

	public class UnitOfWorkTransactionScope<TDbContext> : IUnitOfWorkTransactionScope<TDbContext>
		where TDbContext : DbContext
	{
		private readonly IEntityFrameworkUnitOfWorkProvider<TDbContext> unitOfWorkProvider;

		public UnitOfWorkTransactionScope(IEntityFrameworkUnitOfWorkProvider<TDbContext> unitOfWorkProvider)
		{
			this.unitOfWorkProvider = unitOfWorkProvider;
		}

		public async Task<UnitOfWorkTransactionScopeResult> ExecuteAsync(Func<EntityFrameworkUnitOfWork<TDbContext>, Task> transactionBody)
		{
			using (var uow = (EntityFrameworkUnitOfWork<TDbContext>)unitOfWorkProvider.Create(DbContextOptions.AlwaysCreateOwnContext))
			using (var trans = uow.Context.Database.BeginTransaction())
			{
				UnitOfWorkTransactionScopeResult? result = null;

				try
				{
					uow.IsInTransaction = true;

					await transactionBody(uow);

					if (uow.CommitsCount > 0 && !uow.CommitPending)
					{
						trans.Commit();
						result = UnitOfWorkTransactionScopeResult.Commit;
					}
				}
				catch (Exception e)
				{
					if (e is RollbackRequestedException)
					{
						// someone called rollback, no worries here, continue to finally
					}
					else
					{
						// other than rollback exception occured, need to throw it up
						throw;
					}
				}
				finally
				{
					if (!result.HasValue)
					{
						trans.Rollback();
						result = UnitOfWorkTransactionScopeResult.Rollback;
					}
				}

				return result.Value;
			}
		}

		public UnitOfWorkTransactionScopeResult Execute(Action<EntityFrameworkUnitOfWork<TDbContext>> transactionBody)
		{
			// no issue with deadlock since it runs synchronously
			return ExecuteAsync(uow =>
			{
				transactionBody(uow);
				return Task.CompletedTask;
			}).GetAwaiter().GetResult();
		}
	}
}