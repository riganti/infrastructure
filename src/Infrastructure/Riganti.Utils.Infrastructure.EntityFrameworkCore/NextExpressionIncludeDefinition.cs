using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.EntityFrameworkCore
{
    public class NextExpressionIncludeDefinition<TEntity, TResult, TNextResult> : IChainableIncludeDefinition<TEntity, TNextResult>, IIncludeDefinition<TEntity>
        where TEntity : class where TResult : class where TNextResult : class
    {
        private readonly IChainableIncludeDefinition<TEntity, TResult> baseInclude;

        public Expression<Func<TResult, TNextResult>> Expression { get; private set; }

        public NextExpressionIncludeDefinition(IChainableIncludeDefinition<TEntity, TResult> baseInclude, Expression<Func<TResult, TNextResult>> expression)
        {
            Expression = expression;
            this.baseInclude = baseInclude;
        }

        IQueryable<TEntity> IIncludeDefinition<TEntity>.ApplyInclude(IQueryable<TEntity> query)
        {
            var baseQuery = ((IIncludeDefinition<TEntity>)baseInclude).ApplyInclude(query);
            return ((IIncludableQueryable<TEntity, TResult>)baseQuery).ThenInclude(Expression);
        }
    }
}