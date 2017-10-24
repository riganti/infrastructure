using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Riganti.Utils.Infrastructure.Core
{
    /// <summary>
    ///     A base implementation of the query object pattern.
    /// </summary>
    /// <typeparam name="TResult">The type of the result that the query returns.</typeparam>
    public abstract class QueryBase<TResult> : QueryBase<TResult, TResult>
    {
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
    ///     A base implementation of the query object pattern.
    /// </summary>
    /// <typeparam name="TResult">The type of the result that the query returns.</typeparam>
    /// <typeparam name="TQueryableResult">The type of the result of GetQueryable method. Thats for cases when you need compose TResult in PostProcessResults.</typeparam>
    public abstract class QueryBase<TQueryableResult, TResult> : IQuery<TQueryableResult, TResult>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="QueryBase{TResult}" /> class.
        /// </summary>
        protected QueryBase()
        {
            SortCriteria = new List<Func<IQueryable<TQueryableResult>, IOrderedQueryable<TQueryableResult>>>();
        }

        /// <summary>
        ///     Gets or sets a number of rows to be skipped. If this value is null, the paging will be applied.
        /// </summary>
        public int? Skip { get; set; }

        /// <summary>
        ///     Gets or sets the page size. If this value is null, the paging will not be applied.
        /// </summary>
        public int? Take { get; set; }


        /// <summary>
        ///     Gets a list of sort criteria applied on this query.
        /// </summary>
        public IList<Func<IQueryable<TQueryableResult>, IOrderedQueryable<TQueryableResult>>> SortCriteria { get; }

        /// <summary>
        ///     Adds a specified sort criteria to the query.
        /// </summary>
        public void AddSortCriteria(string fieldName, SortDirection direction = SortDirection.Ascending)
        {
            // create the expression
            var prop = typeof(TQueryableResult).GetTypeInfo().GetProperty(fieldName);
            var param = Expression.Parameter(typeof(TQueryableResult), "i");
            var expr = Expression.Lambda(Expression.Property(param, prop), param);

            // call the method
            typeof(QueryBase<TQueryableResult, TResult>).GetTypeInfo().GetMethod(nameof(AddSortCriteriaCore),
                    BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(prop.PropertyType)
                .Invoke(this, new object[] { expr, direction });
        }

        public void ClearSortCriteria()
        {
            SortCriteria.Clear();
        }

        /// <summary>
        ///     Adds a specified sort criteria to the query.
        /// </summary>
        public void AddSortCriteria<TKey>(Expression<Func<TQueryableResult, TKey>> field, SortDirection direction = SortDirection.Ascending)
        {
            AddSortCriteriaCore(field, direction);
        }

        /// <summary>
        ///     Executes the query and returns the results.
        /// </summary>
        public virtual IList<TResult> Execute()
        {
            return Execute(null);
        }

        /// <summary>
        ///     Executes the query with the filter and returns the results.
        /// </summary>
        public virtual IList<TResult> Execute(FilterDTOBase filter)
        {
            var query = PreProcessQuery(filter);
            var queryResults = query.ToList();
            var results = PostProcessResults(queryResults);
            return results;
        }

        /// <summary>
        ///     Asynchronously executes the query and returns the results.
        /// </summary>
        public virtual async Task<IList<TResult>> ExecuteAsync()
        {
            return await ExecuteAsync(null);
        }

        /// <summary>
        ///     Asynchronously executes the query and returns the results.
        /// </summary>
        public virtual Task<IList<TResult>> ExecuteAsync(CancellationToken cancellationToken)
        {
            return ExecuteAsync(null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query with the filter and returns the results.
        /// </summary>
        public async Task<IList<TResult>> ExecuteAsync(FilterDTOBase filter, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = PreProcessQuery();
            var queryResults = await ExecuteQueryAsync(query, cancellationToken);
            var results = PostProcessResults(queryResults);
            return results;
        }

        /// <summary>
        ///     Gets the total row count without respect to paging.
        /// </summary>
        public virtual int GetTotalRowCount()
        {
            return GetQueryable().Count();
        }

        /// <summary>
        ///     Gets the total row count without respect to paging.
        /// </summary>
        public virtual Task<int> GetTotalRowCountAsync()
        {
            return GetTotalRowCountAsync(CancellationToken.None);
        }

        /// <summary>
        ///     Gets the total row count without respect to paging.
        /// </summary>
        public abstract Task<int> GetTotalRowCountAsync(CancellationToken cancellationToken);

        private void AddSortCriteriaCore<TKey>(Expression<Func<TQueryableResult, TKey>> sortExpression, SortDirection direction)
        {
            if (direction == SortDirection.Ascending)
                SortCriteria.Add(x => x.OrderBy(sortExpression));
            else
                SortCriteria.Add(x => x.OrderByDescending(sortExpression));
        }

        protected abstract Task<IList<TQueryableResult>> ExecuteQueryAsync(IQueryable<TQueryableResult> query,
            CancellationToken cancellationToken);

        private IQueryable<TQueryableResult> PreProcessQuery(FilterDTOBase filter = null)
        {
            var query = GetQueryable();

            if (filter != null)
                query = query.Where(filter);

            for (var i = SortCriteria.Count - 1; i >= 0; i--)
                query = SortCriteria[i](query);

            if (Skip != null)
                query = query.Skip(Skip.Value);
            if (Take != null)
                query = query.Take(Take.Value);

            return query;
        }

        /// <summary>
        ///     When overriden in derived class, it allows to modify the materialized results of the query before they are returned
        ///     to the caller.
        /// </summary>
        protected abstract IList<TResult> PostProcessResults(IList<TQueryableResult> results);

        protected abstract IQueryable<TQueryableResult> GetQueryable();
    }
}