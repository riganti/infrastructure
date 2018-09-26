using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage
{
    /// <summary>
    /// Provides a global interface for table storage.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface ITableStorageRepository<TEntity> where TEntity : ITableEntity, new()
    {
        /// <summary>
        /// Gets the context associated with this repository.
        /// </summary>
        ITableStorageContext Context { get; }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        void Delete(TEntity entity);

        /// <summary>
        /// Deletes the specified entities.
        /// </summary>
        void Delete(IEnumerable<TEntity> entities);

        /// <summary>
        /// Deletes an entity with the specified ID.
        /// </summary>
        void DeleteAsync(string partitionKey, string rowKey, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously gets the entity with the specified partition and row keys.
        /// </summary>
        Task<TEntity> GetByKeyAsync(string partitionKey, string rowKey, CancellationToken cancellationToken);

        /// <summary>
        /// Gets all entities from the specified table.
        /// </summary>
        Task<IEnumerable<TEntity>> GetAllAsync(string partitionKey);

        /// <summary>
        /// Finds entities by executing the query.
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync(TableQuery<TEntity> query);

        /// <summary>
        /// Finds entities by executing the query.
        /// </summary>
        Task<TableQuerySegment<TEntity>> FindAsync(TableQuery<TEntity> query, TableContinuationToken continuationToken);

        /// <summary>
        /// Initializes a new entity with appropriate default values.
        /// </summary>
        TEntity InitializeNew(string partitionKey, string rowKey);

        /// <summary>
        /// Initializes a new query for the specified entity.
        /// </summary>
        TableQuery<TEntity> InitializeNewQuery();

        /// <summary>
        /// Inserts the specified entity into the table.
        /// </summary>
        void Insert(TEntity entity);

        /// <summary>
        /// Inserts the specified entities into the table.
        /// </summary>
        void Insert(IEnumerable<TEntity> entities);

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        void Update(TEntity entity);

        /// <summary>
        /// Updates the specified entities in a batch.
        /// </summary>
        void Update(IEnumerable<TEntity> entities);
    }
}