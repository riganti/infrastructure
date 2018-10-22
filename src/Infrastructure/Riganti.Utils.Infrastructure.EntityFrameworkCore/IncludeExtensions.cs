using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Riganti.Utils.Infrastructure.EntityFrameworkCore
{
    public static class IncludeExtensions
    {

        public static NextExpressionIncludeDefinition<TEntity, TResult, TNextResult> Then<TEntity, TResult, TNextResult>(
            this IChainableIncludeDefinition<TEntity, TResult> target, 
            Expression<Func<TResult, TNextResult>> expression
        )
            where TEntity : class where TResult : class where TNextResult : class
        {
            return new NextExpressionIncludeDefinition<TEntity, TResult, TNextResult>(target, expression);
        }

        public static NextExpressionIncludeCollectionDefinition<TEntity, IEnumerable<TResult>, TResult, TNextResult> Then<TEntity, TResult, TNextResult>(
            this ExpressionIncludeDefinition<TEntity, IEnumerable<TResult>> target,
            Expression<Func<TResult, TNextResult>> expression
        )
            where TEntity : class where TResult : class where TNextResult : class
        {
            return new NextExpressionIncludeCollectionDefinition<TEntity, IEnumerable<TResult>, TResult, TNextResult>(target, expression);
        }
    }
}