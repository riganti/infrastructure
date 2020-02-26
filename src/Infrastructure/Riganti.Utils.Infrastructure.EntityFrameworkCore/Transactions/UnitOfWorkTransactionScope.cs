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

		public async Task<TResult> ExecuteAsync<TResult>(Func<EntityFrameworkUnitOfWork<TDbContext>, Task<TResult>> transactionBody)
		{
			using (var uow = (EntityFrameworkUnitOfWork<TDbContext>)unitOfWorkProvider.Create(DbContextOptions.AlwaysCreateOwnContext))
			using (var trans = uow.Context.Database.BeginTransaction())
			{
				var committed = false;
				TResult result;

				try
				{
					uow.IsInTransaction = true;

					result = await transactionBody(uow);

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