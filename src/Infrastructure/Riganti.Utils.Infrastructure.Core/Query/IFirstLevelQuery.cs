using System.Linq;

namespace Riganti.Utils.Infrastructure.Core
{
    /// <summary>
    /// Represents a query that returns valid entities which the application should see.
    /// </summary>
    public interface IFirstLevelQuery<out TEntity>
    {

        IQueryable<TEntity> GetEntitySet();

    }
}
