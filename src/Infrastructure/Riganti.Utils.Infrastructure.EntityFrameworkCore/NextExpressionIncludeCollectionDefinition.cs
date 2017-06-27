using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Riganti.Utils.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Riganti.Utils.Infrastructure.EntityFrameworkCore
{
    public class NextExpressionIncludeCollectionDefinition<TEntity, TResultList, TResult, TNextResult> : IChainableIncludeDefinition<TEntity, TNextResult>, IIncludeDefinition<TEntity>
      where TEntity : class
      where TResultList : class, IEnumerable<TResult>
      where TResult : class
      where TNextResult : class

    {
        private readonly IChainableIncludeDefinition<TEntity, TResultList> baseInclude;

        public Expression<Func<TResult, TNextResult>> Expression { get; private set; }

        public NextExpressionIncludeCollectionDefinition(IChainableIncludeDefinition<TEntity, TResultList> baseInclude, Expression<Func<TResult, TNextResult>> expression)
        {
            Expression = expression;
            this.baseInclude = baseInclude;
        }

        IQueryable<TEntity> IIncludeDefinition<TEntity>.ApplyInclude(IQueryable<TEntity> query)
        {
            var baseQuery = ((IIncludeDefinition<TEntity>)baseInclude).ApplyInclude(query);
            return ((IIncludableQueryable<TEntity, TResultList>)baseQuery).ThenInclude(Expression);
        }
    }
}
