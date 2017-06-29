using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Riganti.Utils.Infrastructure.Core
{
    /// <summary>
    ///     A generic interface for repositories with batch operation support.
    /// </summary>
    public interface IRepository<TEntity, in TKey> where TEntity : IEntity<TKey>
    {
        /// <summary>
        ///     Deletes the specified entity.
        /// </summary>
        void Delete(TEntity entity);

        /// <summary>
        ///     Deletes the specified entities.
        /// </summary>
        void Delete(IEnumerable<TEntity> entities);

        /// <summary>
        ///     Deletes an entity with the specified ID.
        /// </summary>
        void Delete(TKey id);

        /// <summary>
        ///     Deletes entities with the specified IDs.
        /// </summary>
        void Delete(IEnumerable<TKey> ids);

        /// <summary>
        ///     Gets the entity with specified ID.
        /// </summary>
        TEntity GetById(TKey id, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        ///     Asynchronously gets the entity with specified ID.
        /// </summary>
        Task<TEntity> GetByIdAsync(TKey id, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        ///     Asynchronously gets the entity with specified ID.
        /// </summary>
        Task<TEntity> GetByIdAsync(CancellationToken cancellationToken, TKey id,
            params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        ///     Gets a list of entities with specified IDs.
        /// </summary>
        /// <remarks>This method is not suitable for large amounts of entities - the reasonable limit of number of IDs is 30.</remarks>
        IList<TEntity> GetByIds(IEnumerable<TKey> ids, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        ///     Asynchronously gets a list of entities with specified IDs.
        /// </summary>
        /// <remarks>This method is not suitable for large amounts of entities - the reasonable limit of number of IDs is 30.</remarks>
        Task<IList<TEntity>> GetByIdsAsync(IEnumerable<TKey> ids, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        ///     Asynchronously gets a list of entities with specified IDs.
        /// </summary>
        /// <remarks>This method is not suitable for large amounts of entities - the reasonable limit of number of IDs is 30.</remarks>
        Task<IList<TEntity>> GetByIdsAsync(CancellationToken cancellationToken, IEnumerable<TKey> ids,
            params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        ///     Gets the entity with specified ID.
        /// </summary>
        TEntity GetById(TKey id, IIncludeDefinition<TEntity>[] includes);

        /// <summary>
        ///     Asynchronously gets the entity with specified ID.
        /// </summary>
        Task<TEntity> GetByIdAsync(TKey id, IIncludeDefinition<TEntity>[] includes);

        /// <summary>
        ///     Asynchronously gets the entity with specified ID.
        /// </summary>
        Task<TEntity> GetByIdAsync(CancellationToken cancellationToken, TKey id, IIncludeDefinition<TEntity>[] includes);

        /// <summary>
        ///     Gets a list of entities with specified IDs.
        /// </summary>
        /// <remarks>This method is not suitable for large amounts of entities - the reasonable limit of number of IDs is 30.</remarks>
        IList<TEntity> GetByIds(IEnumerable<TKey> ids, IIncludeDefinition<TEntity>[] includes);

        /// <summary>
        ///     Asynchronously gets a list of entities with specified IDs.
        /// </summary>
        /// <remarks>This method is not suitable for large amounts of entities - the reasonable limit of number of IDs is 30.</remarks>
        Task<IList<TEntity>> GetByIdsAsync(IEnumerable<TKey> ids, IIncludeDefinition<TEntity>[] includes);

        /// <summary>
        ///     Asynchronously gets a list of entities with specified IDs.
        /// </summary>
        /// <remarks>This method is not suitable for large amounts of entities - the reasonable limit of number of IDs is 30.</remarks>
        Task<IList<TEntity>> GetByIdsAsync(CancellationToken cancellationToken, IEnumerable<TKey> ids, IIncludeDefinition<TEntity>[] includes);

        /// <summary>
        ///     Initializes a new entity with appropriate default values.
        /// </summary>
        TEntity InitializeNew();

        /// <summary>
        ///     Inserts the specified entity into the table.
        /// </summary>
        void Insert(TEntity entity);

        /// <summary>
        ///     Inserts the specified entities into the table.
        /// </summary>
        void Insert(IEnumerable<TEntity> entities);

        /// <summary>
        ///     Marks the specified entity as updated.
        /// </summary>
        void Update(TEntity entity);

        /// <summary>
        ///     Marks the specified entities as updated.
        /// </summary>
        void Update(IEnumerable<TEntity> entities);
    }
}