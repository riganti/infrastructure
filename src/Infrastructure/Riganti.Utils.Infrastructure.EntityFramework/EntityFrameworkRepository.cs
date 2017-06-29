﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.EntityFramework
{
    /// <summary>
    /// A base implementation of a repository in Entity Framework.
    /// </summary>
    public class EntityFrameworkRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>, new()
    {
        private readonly IUnitOfWorkProvider provider;
        private readonly IDateTimeProvider dateTimeProvider;

        /// <summary>
        /// Gets the <see cref="DbContext"/>.
        /// </summary>
        protected DbContext Context => EntityFrameworkUnitOfWork.TryGetDbContext(provider);


        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkRepository{TEntity, TKey}"/> class.
        /// </summary>
        public EntityFrameworkRepository(IUnitOfWorkProvider provider, IDateTimeProvider dateTimeProvider)
        {
            this.provider = provider;
            this.dateTimeProvider = dateTimeProvider;
        }

        /// <summary>
        /// Gets the entity with specified ID.
        /// </summary>
        public TEntity GetById(TKey id, params Expression<Func<TEntity, object>>[] includes)
        {
            return GetByIds(new[] { id }, includes).FirstOrDefault();
        }

        /// <summary>
        ///     Asynchronously gets the entity with specified ID.
        /// </summary>
        public Task<TEntity> GetByIdAsync(TKey id, params Expression<Func<TEntity, object>>[] includes)
        {
            return GetByIdAsync(default(CancellationToken), id, includes);
        }

        /// <summary>
        ///     Asynchronously gets the entity with specified ID.
        /// </summary>
        public async Task<TEntity> GetByIdAsync(CancellationToken cancellationToken, TKey id, params Expression<Func<TEntity, object>>[] includes)
        {
            var items = await GetByIdsAsync(cancellationToken, new[] {id}, includes);
            return items.FirstOrDefault();
        }

        /// <summary>
        /// Gets a list of entities with specified IDs.
        /// </summary>
        /// <remarks>
        /// This method is not suitable for large amounts of entities - the reasonable limit of number of IDs is 30.
        /// </remarks>
        public virtual IList<TEntity> GetByIds(IEnumerable<TKey> ids, params Expression<Func<TEntity, object>>[] includes)
        {
            return GetByIdsCore(ids, includes).ToList();
        }

        /// <summary>
        ///     Asynchronously gets a list of entities with specified IDs.
        /// </summary>
        /// <remarks>This method is not suitable for large amounts of entities - the reasonable limit of number of IDs is 30.</remarks>
        public Task<IList<TEntity>> GetByIdsAsync(IEnumerable<TKey> ids, params Expression<Func<TEntity, object>>[] includes)
        {
            return GetByIdsAsync(default(CancellationToken), ids, includes);
        }

        /// <summary>
        ///     Asynchronously gets a list of entities with specified IDs.
        /// </summary>
        /// <remarks>This method is not suitable for large amounts of entities - the reasonable limit of number of IDs is 30.</remarks>
        public async Task<IList<TEntity>> GetByIdsAsync(CancellationToken cancellationToken, IEnumerable<TKey> ids, params Expression<Func<TEntity, object>>[] includes)
        {
            return await GetByIdsCore(ids, includes).ToListAsync(cancellationToken);
        }


        /// <summary>
        /// Gets the entity with specified ID.
        /// </summary>
        public TEntity GetById(TKey id, IIncludeDefinition<TEntity>[] includes)
        {
            return GetByIds(new[] { id }, includes).FirstOrDefault();
        }

        /// <summary>
        ///     Asynchronously gets the entity with specified ID.
        /// </summary>
        public Task<TEntity> GetByIdAsync(TKey id, IIncludeDefinition<TEntity>[] includes)
        {
            return GetByIdAsync(default(CancellationToken), id, includes);
        }

        /// <summary>
        ///     Asynchronously gets the entity with specified ID.
        /// </summary>
        public async Task<TEntity> GetByIdAsync(CancellationToken cancellationToken, TKey id, IIncludeDefinition<TEntity>[] includes)
        {
            var items = await GetByIdsAsync(cancellationToken, new[] { id }, includes);
            return items.FirstOrDefault();
        }

        /// <summary>
        /// Gets a list of entities with specified IDs.
        /// </summary>
        /// <remarks>
        /// This method is not suitable for large amounts of entities - the reasonable limit of number of IDs is 30.
        /// </remarks>
        public virtual IList<TEntity> GetByIds(IEnumerable<TKey> ids, IIncludeDefinition<TEntity>[] includes)
        {
            return GetByIdsCore(ids, includes).ToList();
        }

        /// <summary>
        ///     Asynchronously gets a list of entities with specified IDs.
        /// </summary>
        /// <remarks>This method is not suitable for large amounts of entities - the reasonable limit of number of IDs is 30.</remarks>
        public Task<IList<TEntity>> GetByIdsAsync(IEnumerable<TKey> ids, IIncludeDefinition<TEntity>[] includes)
        {
            return GetByIdsAsync(default(CancellationToken), ids, includes);
        }

        /// <summary>
        ///     Asynchronously gets a list of entities with specified IDs.
        /// </summary>
        /// <remarks>This method is not suitable for large amounts of entities - the reasonable limit of number of IDs is 30.</remarks>
        public async Task<IList<TEntity>> GetByIdsAsync(CancellationToken cancellationToken, IEnumerable<TKey> ids, IIncludeDefinition<TEntity>[] includes)
        {
            return await GetByIdsCore(ids, includes).ToListAsync(cancellationToken);
        }

        private IQueryable<TEntity> GetByIdsCore(IEnumerable<TKey> ids, Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = Context.Set<TEntity>();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query.Where(i => ids.Contains(i.Id));
        }

        private IQueryable<TEntity> GetByIdsCore(IEnumerable<TKey> ids, IIncludeDefinition<TEntity>[] includes)
        {
            IQueryable<TEntity> query = Context.Set<TEntity>();
            foreach (var include in includes)
            {
                query = include.ApplyInclude(query);
            }
            return query.Where(i => ids.Contains(i.Id));
        }

        /// <summary>
        /// Initializes a new entity with appropriate default values.
        /// </summary>
        public virtual TEntity InitializeNew()
        {
            return new TEntity();
        }

        /// <summary>
        /// Inserts the specified entity into the table.
        /// </summary>
        public virtual void Insert(TEntity entity)
        {
            Context.Set<TEntity>().Add(entity);
        }

        /// <summary>
        /// Inserts the specified entity into the table.
        /// </summary>
        public void Insert(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities.ToList())
            {
                Insert(entity);
            }
        }

        /// <summary>
        /// Marks the specified entity as updated.
        /// </summary>
        public virtual void Update(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
        }

        /// <summary>
        /// Marks the specified entity as updated.
        /// </summary>
        public void Update(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities.ToList())
            {
                Update(entity);
            }
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        public virtual void Delete(TEntity entity)
        {
            if (EnableSoftDelete && entity is ISoftDeleteEntity)
            {
                SoftDeleteCore((ISoftDeleteEntity)entity);
            }
            else
            {
                DeleteCore(entity);
            }
        }

        protected virtual void DeleteCore(TEntity entity)
        {
            Context.Set<TEntity>().Remove(entity);
        }

        protected virtual void SoftDeleteCore(ISoftDeleteEntity entity)
        {
            entity.DeletedDate = GetSoftDeleteEntityDeletedDateValue();
        }

        /// <summary>
        /// Gets the value indicating that the soft delete is enabled.
        /// </summary>
        protected virtual bool EnableSoftDelete => true;

        /// <summary>
        /// Gets the value that is stored in the DeletedDate property of ISoftDeleteEntities.
        /// </summary>
        protected virtual DateTime? GetSoftDeleteEntityDeletedDateValue()
        {
            return dateTimeProvider.Now;
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        public void Delete(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities.ToList())
            {
                Delete(entity);
            }
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        public virtual void Delete(TKey id)
        {
            var fake = new TEntity() { Id = id };
            Context.Set<TEntity>().Attach(fake);
            Delete(fake);
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        public void Delete(IEnumerable<TKey> ids)
        {
            foreach (var id in ids)
            {
                Delete(id);
            }
        }
    }
}
