using System.Data.Entity;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.EntityFramework
{
    /// <summary>
    /// Represents a query that returns valid entities which the application should see.
    /// </summary>
    // ReSharper disable once UnusedTypeParameter
    public interface IEntityFrameworkFirstLevelQuery<out TEntity, TDbContext> : IFirstLevelQuery<TEntity>
        where TDbContext : DbContext
    {
    }
}
