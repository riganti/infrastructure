using System;
using System.Linq;
using System.Linq.Expressions;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.EntityFrameworkCore
{
    public class IncludeDefinitionStub<TEntity> : IIncludeDefinition<TEntity> where TEntity : class
    {
        IQueryable<TEntity> IIncludeDefinition<TEntity>.ApplyInclude(IQueryable<TEntity> query)
        {
            throw new NotSupportedException("Calling Includes.For<Entity>() without specifying the expression using Include(expression) is not permitted!");
        }


        /// <summary>
        /// Creates an Include clause for the specified expression.
        /// </summary>
        public ExpressionIncludeDefinition<TEntity, TResult> Include<TResult>(Expression<Func<TEntity, TResult>> expression) where TResult : class
        {
            return new ExpressionIncludeDefinition<TEntity, TResult>(expression);
        }

        /// <summary>
        /// Creates an Include clause for the specified expression.
        /// </summary>
        public StringPathIncludeDefinition<TEntity> Include(string expression)
        {
            return new StringPathIncludeDefinition<TEntity>(expression);
        }
    }
}