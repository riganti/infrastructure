using System.Collections.Generic;
using System.Data.Entity;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.EntityFramework
{
    /// <summary>
    /// A base implementation of query object in Entity Framework.
    /// </summary>
    public abstract class EntityFrameworkQuery<TResult> : EntityFrameworkPostProcessingQuery<TResult, TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkQuery{TResult}"/> class.
        /// </summary>
        protected EntityFrameworkQuery(IUnitOfWorkProvider unitOfWorkProvider)
            : base(unitOfWorkProvider)
        {
        }

        /// <summary>
        ///     When overriden in derived class, it allows to modify the materialized results of the query before they are returned
        ///     to the caller.
        /// </summary>
        protected override IList<TResult> PostProcessResults(IList<TResult> results)
        {
            return results;
        }
    }

    /// <summary>
    /// A base implementation of query object in Entity Framework.
    /// </summary>
    public abstract class EntityFrameworkQuery<TResult, TDbContext> : EntityFrameworkPostProcessingQuery<TResult, TResult, TDbContext>
        where TDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkQuery{TResult}"/> class.
        /// </summary>
        protected EntityFrameworkQuery(IUnitOfWorkProvider unitOfWorkProvider)
            : base(unitOfWorkProvider)
        {
        }

        /// <summary>
        ///     When overriden in derived class, it allows to modify the materialized results of the query before they are returned
        ///     to the caller.
        /// </summary>
        protected override IList<TResult> PostProcessResults(IList<TResult> results)
        {
            return results;
        }
    }
}