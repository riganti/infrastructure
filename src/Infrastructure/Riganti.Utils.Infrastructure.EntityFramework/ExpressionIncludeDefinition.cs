using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.EntityFramework
{

    public class ExpressionIncludeDefinition<T> : IIncludeDefinition<T>
    {
        public Expression<Func<T, object>> Expression { get; private set; }

        public ExpressionIncludeDefinition(Expression<Func<T, object>> expression)
        {
            Expression = expression;
        }

        IQueryable<T> IIncludeDefinition<T>.ApplyInclude(IQueryable<T> query)
        {
            return query.Include(Expression);
        }

    }
}
