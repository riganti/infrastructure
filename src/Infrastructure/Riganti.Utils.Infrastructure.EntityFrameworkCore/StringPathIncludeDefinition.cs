using System.Linq;
using Microsoft.EntityFrameworkCore;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.EntityFrameworkCore
{
    public class StringPathIncludeDefinition<T> : IIncludeDefinition<T> where T : class
    {
        public string Expression { get; private set; }

        public StringPathIncludeDefinition(string expression)
        {
            Expression = expression;
        }

        IQueryable<T> IIncludeDefinition<T>.ApplyInclude(IQueryable<T> query)
        {
            return query.Include(Expression);
        }
    }
}