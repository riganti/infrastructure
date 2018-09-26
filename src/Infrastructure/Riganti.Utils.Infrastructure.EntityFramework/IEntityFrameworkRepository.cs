using System.Data.Entity;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.EntityFramework
{
    /// <summary>
    /// An interface for repository in Entity Framework.
    /// </summary>
    // ReSharper disable once UnusedTypeParameter
    public interface IEntityFrameworkRepository<TEntity, in TKey, TDbContext> : IRepository<TEntity, TKey>
        where TEntity : IEntity<TKey>
        where TDbContext : DbContext
    {
    }
}