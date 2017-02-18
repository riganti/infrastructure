using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.EntityFrameworkCore
{

    public class ExpressionIncludeDefinition<TEntity, TResult> : IChainableIncludeDefinition<TEntity, TResult>, IIncludeDefinition<TEntity>
        where TEntity : class where TResult : class
    {
        public Expression<Func<TEntity, TResult>> Expression { get; private set; }


        public ExpressionIncludeDefinition(Expression<Func<TEntity, TResult>> expression)
        {
            Expression = expression;
        }

        IQueryable<TEntity> IIncludeDefinition<TEntity>.ApplyInclude(IQueryable<TEntity> query)
        {
            return query.Include(Expression);
        }
    }
}
