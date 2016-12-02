using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.EntityFramework
{
    /// <summary>
    /// A base implementation of query object in Entity Framework.
    /// </summary>
    public abstract class EntityFrameworkQuery<TResult> : QueryBase<TResult>
    {
        private readonly IUnitOfWorkProvider provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkQuery{TResult}"/> class.
        /// </summary>
        protected EntityFrameworkQuery(IUnitOfWorkProvider provider)
        {
            this.provider = provider;
        }

        /// <summary>
        /// Gets the <see cref="DbContext"/>.
        /// </summary>
        protected virtual DbContext Context => EntityFrameworkUnitOfWork.TryGetDbContext(provider);

        protected override async Task<IList<TResult>> ExecuteQueryAsync(IQueryable<TResult> query, CancellationToken cancellationToken)
        {
            return await query.ToListAsync(cancellationToken);
        }

        public override async Task<int> GetTotalRowCountAsync(CancellationToken cancellationToken)
        {
            return await GetQueryable().CountAsync(cancellationToken);
        }
    }
}
