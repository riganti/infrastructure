using System.Linq;

namespace Riganti.Utils.Infrastructure.Core
{
    public interface IIncludeDefinition<T>
    {

        IQueryable<T> ApplyInclude(IQueryable<T> query);

    }
}