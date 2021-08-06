using System.Data;
using System.Data.Entity;
using Riganti.Utils.Infrastructure.Core;
using Riganti.Utils.Infrastructure.EntityFramework.Transactions;

namespace Riganti.Utils.Infrastructure.EntityFramework
{
    /// <summary>
    /// An interface for unit of work provider which is responsible for creating and managing unit of work in Entity Framework.
    /// </summary>
    // ReSharper disable once UnusedTypeParameter
    public interface IEntityFrameworkUnitOfWorkProvider<TDbContext> : IUnitOfWorkProvider
        where TDbContext : DbContext
    {
        /// <summary>
        /// Creates the unit of work with specified options.
        /// </summary>
        IUnitOfWork Create(DbContextOptions options);

        /// <param name="isolationLevel">Isolation level of transactions.</param>
        /// <returns>Object with Execute and ExecuteAsync methods.</returns>
        IUnitOfWorkTransactionScope<TDbContext> CreateTransactionScope(IsolationLevel isolationLevel);

        /// <returns>Object with Execute and ExecuteAsync methods.</returns>
        IUnitOfWorkTransactionScope<TDbContext> CreateTransactionScope();
    }
}