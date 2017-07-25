using System.Data.Entity;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.EntityFramework
{
    /// <summary>
    /// An interface for unit of work provider which is responsible for creating and managing unit of work in Entity Framework.
    /// </summary>
    // ReSharper disable once UnusedTypeParameter
    public interface IEntityFrameworkUnitOfWorkProvider<TDbContext> : IUnitOfWorkProvider
        where TDbContext : DbContext
    {
    }
}