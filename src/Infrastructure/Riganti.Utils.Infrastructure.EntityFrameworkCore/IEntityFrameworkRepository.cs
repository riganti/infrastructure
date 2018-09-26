using Microsoft.EntityFrameworkCore;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.EntityFrameworkCore
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