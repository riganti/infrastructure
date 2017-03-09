using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage
{
    public interface ITableStorageRepository<TEntity> where TEntity : TableEntity, new()
    {
        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        void Delete(string table, TEntity entity);

        /// <summary>
        /// Deletes the specified entities.
        /// </summary>
        void Delete(string table, IEnumerable<TEntity> entities);

        /// <summary>
        /// Deletes an entity with the specified ID.
        /// </summary>
        void Delete(string table, Tuple<string, string> keys);

        /// <summary>
        /// Asynchronously gets the entity with the specified partition and row keys.
        /// </summary>
        Task<TEntity> GetByKeyAsync(string table, Tuple<string, string> keys, CancellationToken cancellationToken);

        /// <summary>
        /// Gets all entities from the specified table.
        /// </summary>
        Task<IEnumerable<TEntity>> GetAllAsync(string table, string partitionKey);

        /// <summary>
        /// Initializes a new entity with appropriate default values.
        /// </summary>
        TEntity InitializeNew(Tuple<string, string> keys);

        /// <summary>
        /// Initializes a new query for the specified <typeparamref name="TEntity"/>.
        /// </summary>
        TableQuery<TEntity> InitializeNewQuery();

        /// <summary>
        /// Inserts the specified entity into the table.
        /// </summary>
        void Insert(string table, TEntity entity);

        /// <summary>
        /// Inserts the specified entities into the table.
        /// </summary>
        void Insert(string table, IEnumerable<TEntity> entities);

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        void Update(string table, TEntity entity);

        /// <summary>
        /// Updates the specified entities in a batch.
        /// </summary>
        void Update(string table, IEnumerable<TEntity> entities);

        /// <summary>
        /// Finds entities by executing the query.
        /// </summary>
        Task<IEnumerable<TEntity>> FindAsync(string table, TableQuery<TEntity> query);
    }
}