using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Riganti.Utils.Infrastructure.Core
{
    /// <summary>
    /// A base implementation of the query object pattern.
    /// </summary>
    /// <typeparam name="TResult">The type of the result that the query returns.</typeparam>
    public abstract class QueryBase<TResult> : IQuery<TResult>
    {
        /// <summary>
        /// Gets or sets a number of rows to be skipped. If this value is null, the paging will be applied.
        /// </summary>
        public int? Skip { get; set; }

        /// <summary>
        /// Gets or sets the page size. If this value is null, the paging will not be applied.
        /// </summary>
        public int? Take { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryBase{TResult}"/> class.
        /// </summary>
        protected QueryBase()
        {
            SortCriteria = new List<Func<IQueryable<TResult>, IOrderedQueryable<TResult>>>();
        }


        /// <summary>
        /// Gets a list of sort criteria applied on this query.
        /// </summary>
        public IList<Func<IQueryable<TResult>, IOrderedQueryable<TResult>>> SortCriteria { get; private set; }

        /// <summary>
        /// Adds a specified sort criteria to the query.
        /// </summary>
        public void AddSortCriteria(string fieldName, SortDirection direction = SortDirection.Ascending)
        {
            // create the expression
            var prop = typeof(TResult).GetProperty(fieldName);
            var param = Expression.Parameter(typeof(TResult), "i");
            var expr = Expression.Lambda(Expression.Property(param, prop), param);

            // call the method
            typeof(QueryBase<TResult>).GetMethod(nameof(AddSortCriteriaCore), BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(prop.PropertyType)
                .Invoke(this, new object[] { expr, direction });
        }

        /// <summary>
        /// Adds a specified sort criteria to the query.
        /// </summary>
        public void AddSortCriteria<TKey>(Expression<Func<TResult, TKey>> field, SortDirection direction)
        {
            AddSortCriteriaCore(field, direction);
        }

        private void AddSortCriteriaCore<TKey>(Expression<Func<TResult, TKey>> sortExpression, SortDirection direction)
        {
            if (direction == SortDirection.Ascending)
            {
                SortCriteria.Add(x => x.OrderBy(sortExpression));
            }
            else
            {
                SortCriteria.Add(x => x.OrderByDescending(sortExpression));
            }
        }

        /// <summary>
        /// Executes the query and returns the results.
        /// </summary>
        public IList<TResult> Execute()
        {
            var query = GetQueryable();

            for (int i = SortCriteria.Count - 1; i >= 0; i--)
            {
                query = SortCriteria[i](query);
            }

            if (Skip != null)
            {
                query = query.Skip(Skip.Value);
            }
            if (Take != null)
            {
                query = query.Take(Take.Value);
            }

            var results = query.ToList();
            PostProcessResults(results);
            return results;
        }

        /// <summary>
        /// When overriden in derived class, it allows to modify the materialized results of the query before they are returned to the caller.
        /// </summary>
        protected virtual void PostProcessResults(IList<TResult> results)
        {
        }

        /// <summary>
        /// Gets the total row count without respect to paging.
        /// </summary>
        public int GetTotalRowCount()
        {
            return GetQueryable().Count();
        }

        protected abstract IQueryable<TResult> GetQueryable();
    }
}