using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Riganti.Utils.Infrastructure.Core
{
    public interface ISortableQuery
    {

        /// <summary>
        /// Adds a specified sort criteria to the query.
        /// </summary>
        void AddSortCriteria(string fieldName, SortDirection direction = SortDirection.Ascending);

        /// <summary>
        /// Resets the list of sort criteria.
        /// </summary>
        void ClearSortCriteria();

    }

    public interface ISortableQuery<TQueryableResult>
    {

        /// <summary>
        /// Gets a list of sort criteria applied on this query.
        /// </summary>
        IList<Func<IQueryable<TQueryableResult>, IOrderedQueryable<TQueryableResult>>> SortCriteria { get; }

        /// <summary>
        /// Adds a specified sort criteria to the query.
        /// </summary>
        void AddSortCriteria<TKey>(Expression<Func<TQueryableResult, TKey>> field, SortDirection direction = SortDirection.Ascending);

    }
}