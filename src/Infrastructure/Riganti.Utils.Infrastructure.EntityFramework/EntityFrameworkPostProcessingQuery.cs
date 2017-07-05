using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.EntityFramework
{
    /// <summary>
    /// A base implementation of query object in Entity Framework with support of result post processing.
    /// </summary>
    public abstract class EntityFrameworkPostProcessingQuery<TQueryableResult, TResult> : EntityFrameworkPostProcessingQuery<TQueryableResult, TResult, DbContext>
    {
        protected EntityFrameworkPostProcessingQuery(IUnitOfWorkProvider unitOfWorkProvider)
            : base(unitOfWorkProvider)
        {
        }
    }

    /// <summary>
    /// A base implementation of query object in Entity Framework with support of result post processing.
    /// </summary>
    public abstract class EntityFrameworkPostProcessingQuery<TQueryableResult, TResult, TDbContext> : QueryBase<TQueryableResult, TResult>
        where TDbContext : DbContext
    {
        private readonly IUnitOfWorkProvider unitOfWorkProvider;

        /// <summary>
        /// Gets the <see cref="DbContext"/>.
        /// </summary>
        protected virtual TDbContext Context
        {
            get
            {
                var context = EntityFrameworkUnitOfWork.TryGetDbContext<TDbContext>(unitOfWorkProvider);
                if (context == null)
                {
                    throw new InvalidOperationException("The EntityFrameworkQuery must be used in a unit of work of type EntityFrameworkUnitOfWork!");
                }
                return context;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkPostProcessingQuery{TQueryableResult, TResult, TDbContext}"/> class.
        /// </summary>
        protected EntityFrameworkPostProcessingQuery(IEntityFrameworkUnitOfWorkProvider<TDbContext> unitOfWorkProvider)
            : this((IUnitOfWorkProvider)unitOfWorkProvider)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkPostProcessingQuery{TQueryableResult, TResult, TDbContext}"/> class.
        /// </summary>
        protected EntityFrameworkPostProcessingQuery(IUnitOfWorkProvider unitOfWorkProvider)
        {
            this.unitOfWorkProvider = unitOfWorkProvider;
        }

        protected override async Task<IList<TQueryableResult>> ExecuteQueryAsync(IQueryable<TQueryableResult> query, CancellationToken cancellationToken)
        {
            return await query.ToListAsync(cancellationToken);
        }

        public override async Task<int> GetTotalRowCountAsync(CancellationToken cancellationToken)
        {
            return await GetQueryable().CountAsync(cancellationToken);
        }
    }
}