using System.Data.Entity;
using System.Linq;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.EntityFramework
{
    public class StringPathIncludeDefinition<T> : IIncludeDefinition<T>
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