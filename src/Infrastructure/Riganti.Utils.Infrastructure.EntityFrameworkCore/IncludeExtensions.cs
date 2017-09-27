using System;
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

    }
}