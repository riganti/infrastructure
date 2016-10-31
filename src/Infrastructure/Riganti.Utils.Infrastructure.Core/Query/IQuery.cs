using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Riganti.Utils.Infrastructure.Core
{
    /// <summary>
    /// A generic interface for query objects with support for paging and sorting.
    /// </summary>
    public interface IQuery<TResult>
    {

        /// <summary>
        /// Gets or sets a number of rows to be skipped. If this value is null, the paging will be applied.
        /// </summary>
        int? Skip { get; set; }

        /// <summary>
        /// Gets or sets the page size. If this value is null, the paging will not be applied.
        /// </summary>
        int? Take { get; set; }

        /// <summary>
        /// Gets a list of sort criteria applied on this query.
        /// </summary>
        IList<Func<IQueryable<TResult>, IOrderedQueryable<TResult>>> SortCriteria { get; }

        /// <summary>
        /// Adds a specified sort criteria to the query.
        /// </summary>
        void AddSortCriteria<TKey>(Expression<Func<TResult, TKey>> field, SortDirection direction = SortDirection.Ascending);

        /// <summary>
        /// Adds a specified sort criteria to the query.
        /// </summary>
        void AddSortCriteria(string fieldName, SortDirection direction = SortDirection.Ascending);

        /// <summary>
        /// Executes the query and returns the results.
        /// </summary>
        IList<TResult> Execute();

        /// <summary>
        /// Asynchronously executes the query and returns the results.
        /// </summary>
        /// <param name="cancellationToken"></param>
        Task<IList<TResult>> ExecuteAsync();

        /// <summary>
        /// Asynchronously executes the query and returns the results.
        /// </summary>
        /// <param name="cancellationToken"></param>
        Task<IList<TResult>> ExecuteAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Gets the total row count without respect to paging.
        /// </summary>
        int GetTotalRowCount();

    }
}