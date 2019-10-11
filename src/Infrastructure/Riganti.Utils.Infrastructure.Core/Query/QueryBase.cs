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
        [Obsolete]
        public IList<Func<IQueryable<TQueryableResult>, IOrderedQueryable<TQueryableResult>>> SortCriteria 
            { get => sortCriteriaHandler.OldSortCriteria; } 

        private SortCriteriaHandler sortCriteriaHandler = new SortCriteriaHandler();
        
        
        /// <summary>
        ///     Adds a specified sort criteria to the query.
        /// </summary>
        public void AddSortCriteria(string fieldName, SortDirection direction = SortDirection.Ascending)
        {
            var propertyNames = fieldName.Split('.');
            
            Type lastSearchedPropertyType;
            var expressionParameter = Expression.Parameter(typeof(TQueryableResult), "i");
            
            var resultExpressions = CreateExpression(propertyNames.First(),
                                                                  expressionParameter,
                                                                  out lastSearchedPropertyType);
            
            var objectBeingSearchedForProperty = lastSearchedPropertyType.GetTypeInfo();
            
            foreach (var propertyName in propertyNames.Skip(1))
            {
                resultExpressions = AddPropertyToExpression(propertyName,
                    objectBeingSearchedForProperty,
                    out lastSearchedPropertyType,
                    resultExpressions);
                
                objectBeingSearchedForProperty = lastSearchedPropertyType.GetTypeInfo();
            }
             
            var resultLambda = Expression.Lambda(resultExpressions, expressionParameter);

            InvokeAddSortCriteriaCore(lastSearchedPropertyType, resultLambda, direction);
        }

        private MemberExpression CreateExpression(string propertyName, Expression expressionParameter, out Type addedPropertyType)
        {
            var property = typeof(TQueryableResult).GetTypeInfo().GetProperty(propertyName);
            addedPropertyType = property.PropertyType;
            return Expression.Property(expressionParameter, property);
        }

        private MemberExpression AddPropertyToExpression(string propertyName,
            TypeInfo objectBeingSearchedForProperty,
            out Type addedPropertyType,
            MemberExpression expression)
        {
            var property = objectBeingSearchedForProperty.GetProperty(propertyName);
            expression = Expression.Property(expression, property);
            addedPropertyType = property.PropertyType;
            return expression;
        }
        
        private void InvokeAddSortCriteriaCore(Type tKey, LambdaExpression expression, SortDirection direction)
        {
            typeof(SortCriteriaHandler).GetTypeInfo().GetMethod(nameof(sortCriteriaHandler.AddCriterion),
                    BindingFlags.Instance | BindingFlags.Public).MakeGenericMethod(tKey)
                .Invoke(sortCriteriaHandler, new object[] { expression, direction });
        }


        public void ClearSortCriteria()
        {
            sortCriteriaHandler.Clear();
        }

        /// <summary>
        ///     Adds a specified sort criteria to the query.
        /// </summary>
        public void AddSortCriteria<TKey>(Expression<Func<TQueryableResult, TKey>> field, SortDirection direction = SortDirection.Ascending)
        {
            sortCriteriaHandler.AddCriterion(field,direction);
        }

        /// <summary>
        ///     Executes the query and returns the results.
        /// </summary>
        public virtual IList<TResult> Execute()
        {
            var query = PreProcessQuery();
            var queryResults = query.ToList();
            var results = PostProcessResults(queryResults);
            return results;
        }

        /// <summary>
        ///     Asynchronously executes the query and returns the results.
        /// </summary>
        public virtual async Task<IList<TResult>> ExecuteAsync()
        {
            return await ExecuteAsync(default(CancellationToken));
        }

        /// <summary>
        ///     Asynchronously executes the query and returns the results.
        /// </summary>
        public virtual async Task<IList<TResult>> ExecuteAsync(CancellationToken cancellationToken)
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

        protected abstract Task<IList<TQueryableResult>> ExecuteQueryAsync(IQueryable<TQueryableResult> query,
            CancellationToken cancellationToken);

        private IQueryable<TQueryableResult> PreProcessQuery()
        {
            var query = GetQueryable();
            query = sortCriteriaHandler.ApplyCriteria(query);
            return ApplySkipAndTake(query);
        }
        
        private IQueryable<TQueryableResult> ApplySkipAndTake(IQueryable<TQueryableResult> query)
        {
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

        
        
        private class SortCriteriaHandler
        {
            public IList<Func<IQueryable<TQueryableResult>, IOrderedQueryable<TQueryableResult>>> OldSortCriteria { get; }
                  = new List<Func<IQueryable<TQueryableResult>, IOrderedQueryable<TQueryableResult>>>();
            
            private Func<IQueryable<TQueryableResult>, IOrderedQueryable<TQueryableResult>> firstSortCriterion;

            private IList<Func<IOrderedQueryable<TQueryableResult>, IOrderedQueryable<TQueryableResult>>>
                OrderedCriteria = new List<Func<IOrderedQueryable<TQueryableResult>, IOrderedQueryable<TQueryableResult>>>();

            
            public void AddCriterion<TKey>(Expression<Func<TQueryableResult, TKey>> sortExpression, SortDirection direction)
            {
                if (direction == SortDirection.Ascending)
                {
                    OldSortCriteria.Add(x => x.OrderBy(sortExpression));
                    OrderedCriteria.Add(x=>x.ThenBy(sortExpression));
                    if (firstSortCriterion == null)
                    {
                        firstSortCriterion = x => x.OrderBy(sortExpression);
                    }
                }
                else
                {
                    OldSortCriteria.Add(x => x.OrderByDescending(sortExpression));
                    OrderedCriteria.Add(x=>x.ThenByDescending(sortExpression));
                    if (firstSortCriterion == null)
                    {
                        firstSortCriterion = x => x.OrderByDescending(sortExpression);
                    }
                }
            }

            public IQueryable<TQueryableResult> ApplyCriteria(IQueryable<TQueryableResult> query)
            {
                //If lengths don't match user manipulated with oldCriteria and it is necessary to use old algorithm
                if (OrderedCriteria.Count != OldSortCriteria.Count)
                {
                    query = ApplyOldCriteria(query);
                }
                else
                {
                    query = ApplyNewCriteria(query);
                }
                return query;
            }
            
            public void Clear()
            {
                OldSortCriteria.Clear();
                OrderedCriteria.Clear();
                firstSortCriterion = null;
            }
            
            private IQueryable<TQueryableResult> ApplyNewCriteria(IQueryable<TQueryableResult> query)
            {
                if (firstSortCriterion == null)
                {
                    return query;
                }
                
                var orderedQuery = firstSortCriterion(query);
                
                //Skip first because we already applied this criterion using FirstSortCriteriaUnOrdered
                foreach (var criteria in OrderedCriteria.Skip(1))
                {
                    orderedQuery = criteria(orderedQuery);
                }

                return orderedQuery;
            }
            private IQueryable<TQueryableResult> ApplyOldCriteria(IQueryable<TQueryableResult> query)
            {
                for (var i = OldSortCriteria.Count - 1; i >= 0; i--)
                    query = OldSortCriteria[i](query);
                return query;
            }
        }
    }
}