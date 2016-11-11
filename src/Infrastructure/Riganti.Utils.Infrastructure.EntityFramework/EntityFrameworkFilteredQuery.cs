using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.EntityFramework
{
    /// <summary>
    /// A base implementation of query object in Entity Framework.
    /// </summary>
    public abstract class EntityFrameworkFilteredQuery<TResult, TFilter> : EntityFrameworkQuery<TResult>
    {
        
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkQuery{TResult}"/> class.
        /// </summary>
        protected EntityFrameworkFilteredQuery(IUnitOfWorkProvider provider) : base(provider)
        {
        }

    }
}