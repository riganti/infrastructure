using System;
using System.Collections.Generic;
using System.Linq;

namespace Riganti.Utils.Infrastructure.EntityFrameworkCore.Transactions
{
	public enum UnitOfWorkTransactionScopeResult
	{
		Commit,
		Rollback
	}
}