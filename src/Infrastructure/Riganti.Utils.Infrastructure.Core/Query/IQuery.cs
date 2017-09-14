using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Riganti.Utils.Infrastructure.Core
{
    /// <summary>
    /// A generic interface for query objects with support for paging and sorting.
    /// </summary>
    public interface IQuery<TResult> : IPageableQuery, ISortableQuery
    {

        /// <summary>
        /// Executes the query and returns the results.
        /// </summary>
        IList<TResult> Execute();

        /// <summary>
        /// Asynchronously executes the query and returns the results.
        /// </summary>
        Task<IList<TResult>> ExecuteAsync();

        /// <summary>
        /// Asynchronously executes the query and returns the results.
        /// </summary>
        /// <param name="cancellationToken"></param>
        Task<IList<TResult>> ExecuteAsync(CancellationToken cancellationToken);

    }

    /// <summary>
    /// A generic interface for query objects with support for paging and sorting.
    /// </summary>
    public interface IQuery<TQueryableResult, TResult> : IQuery<TResult>, ISortableQuery<TQueryableResult>
    {
    }
}