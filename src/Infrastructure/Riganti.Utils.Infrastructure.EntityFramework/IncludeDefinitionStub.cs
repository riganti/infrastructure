using System;
using System.Linq;
using System.Linq.Expressions;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.EntityFramework
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
        public ExpressionIncludeDefinition<TEntity> Include(Expression<Func<TEntity, object>> expression)
        {
            return new ExpressionIncludeDefinition<TEntity>(expression);
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