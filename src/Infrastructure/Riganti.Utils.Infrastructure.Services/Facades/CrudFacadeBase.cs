using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Services.Facades
{
    /// <summary>
    /// A base class for CRUD pages.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the entity primary key.</typeparam>
    /// <typeparam name="TListDTO">The type of the DTO used in the list of records, e.g. in the GridView control.</typeparam>
    /// <typeparam name="TDetailDTO">The type of the DTO used in the detail form.</typeparam>
    public abstract class CrudFacadeBase<TEntity, TKey, TListDTO, TDetailDTO> : FacadeBase, ICrudFacade<TListDTO, TDetailDTO, TKey> where TEntity : IEntity<TKey> where TDetailDTO : IEntity<TKey>
    {
        /// <summary>
        /// Gets the query object used to populate the list or records.
        /// </summary>
        public Func<IQuery<TListDTO>> QueryFactory { get; }

        /// <summary>
        /// Gets the repository used to perform database operations with the entity.
        /// </summary>
        public IRepository<TEntity, TKey> Repository { get; }

        /// <summary>
        /// Gets the service that can map entities to DTOs and populate entities with changes made on DTOs.
        /// </summary>
        public IEntityDTOMapper<TEntity, TDetailDTO> Mapper { get; }


        /// <summary>
        /// Initializes a new instance of the <see cref="CrudFacadeBase{TEntity, TKey, TListDTO, TDetailDTO}"/> class.
        /// </summary>
        protected CrudFacadeBase(Func<IQuery<TListDTO>> queryFactory, IRepository<TEntity, TKey> repository, IEntityDTOMapper<TEntity, TDetailDTO> mapper)
        {
            QueryFactory = queryFactory;
            Repository = repository;
            Mapper = mapper;
        }

        /// <summary>
        /// Gets the detail DTO for an entity with the specified ID.
        /// </summary>
        public virtual TDetailDTO GetDetail(TKey id)
        {
            using (UnitOfWorkProvider.Create())
            {
                var entity = Repository.GetById(id, EntityIncludes);
                ValidateReadPermissions(entity);
                var detail = Mapper.MapToDTO(entity);
                return detail;
            }
        }

        /// <summary>
        /// Gets the detail DTO for an entity with the specified ID.
        /// </summary>
        public virtual async Task<TDetailDTO> GetDetailAsync(CancellationToken cancellationToken, TKey id)
        {
            using (UnitOfWorkProvider.Create())
            {
                var entity = await Repository.GetByIdAsync(cancellationToken, id, EntityIncludes);
                await ValidateReadPermissionsAsync(cancellationToken, entity);
                var detail = Mapper.MapToDTO(entity);
                return detail;
            }
        }

        /// <summary>
        /// Gets the detail DTO for an entity with the specified ID.
        /// </summary>
        public virtual Task<TDetailDTO> GetDetailAsync(TKey id)
        {
            return GetDetailAsync(default(CancellationToken), id);
        }

        /// <summary>
        /// Gets a new detail DTO with default values.
        /// </summary>
        public virtual TDetailDTO InitializeNew()
        {
            using (UnitOfWorkProvider.Create())
            {
                var entity = Repository.InitializeNew();
                var detail = Mapper.MapToDTO(entity);
                return detail;
            }
        }

        /// <summary>
        /// Saves the changes on the specified DTO to the database.
        /// </summary>
        /// <returns>New instance of DTO with changes reflected during saving.</returns>
        public virtual TDetailDTO Save(TDetailDTO detail)
        {
            using (var uow = UnitOfWorkProvider.Create())
            {
                TEntity entity;
                var isNew = false;
                if (detail.Id.Equals(default(TKey)))
                {
                    // the record is new
                    entity = Repository.InitializeNew();
                    isNew = true;
                }
                else
                {
                    entity = Repository.GetById(detail.Id, EntityIncludes);
                    ValidateModifyPermissions(entity, ModificationStage.BeforeMap);
                }

                // populate the entity
                PopulateDetailToEntity(detail, entity);

                ValidateModifyPermissions(entity, ModificationStage.AfterMap);

                // save
                return Save(entity, isNew, detail, uow);
            }
        }

        /// <summary>
        /// Saves the changes on the specified DTO to the database.
        /// </summary>
        /// <returns>New instance of DTO with changes reflected during saving.</returns>
        public virtual async Task<TDetailDTO> SaveAsync(CancellationToken cancellationToken, TDetailDTO detail)
        {
            using (var uow = UnitOfWorkProvider.Create())
            {
                TEntity entity;
                var isNew = false;
                if (detail.Id.Equals(default(TKey)))
                {
                    // the record is new
                    entity = Repository.InitializeNew();
                    isNew = true;
                }
                else
                {
                    entity = await Repository.GetByIdAsync(cancellationToken, detail.Id, EntityIncludes);
                    await ValidateModifyPermissionsAsync(cancellationToken, entity, ModificationStage.BeforeMap);
                }

                // populate the entity
                PopulateDetailToEntity(detail, entity);

                await ValidateModifyPermissionsAsync(cancellationToken, entity, ModificationStage.AfterMap);

                // save
                return await SaveAsync(cancellationToken, entity, isNew, detail, uow);
            }
        }

        /// <summary>
        /// Saves the changes on the specified DTO to the database.
        /// </summary>
        /// <returns>New instance of DTO with changes reflected during saving.</returns>
        public virtual Task<TDetailDTO> SaveAsync(TDetailDTO detail)
        {
            return SaveAsync(default(CancellationToken), detail);
        }

        /// <summary>
        /// Deletes the entity with the specified ID.
        /// </summary>
        public virtual void Delete(TKey id)
        {
            using (var uow = UnitOfWorkProvider.Create())
            {
                Repository.Delete(id);
                uow.Commit();
            }
        }

        /// <summary>
        /// Deletes the entity with the specified ID.
        /// </summary>
        public virtual async Task DeleteAsync(CancellationToken cancellationToken, TKey id)
        {
            using (var uow = UnitOfWorkProvider.Create())
            {
                Repository.Delete(id);
                await uow.CommitAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Deletes the entity with the specified ID.
        /// </summary>
        public virtual Task DeleteAsync(TKey id)
        {
            return DeleteAsync(default(CancellationToken), id);
        }

        /// <summary>
        /// Gets the list of the DTOs using the Query object.
        /// </summary>
        public virtual IEnumerable<TListDTO> GetList(Action<IQuery<TListDTO>> queryConfiguration = null)
        {
            using (UnitOfWorkProvider.Create())
            {
                var query = QueryFactory();
                queryConfiguration?.Invoke(query);
                return query.Execute();
            }
        }

        /// <summary>
        /// Gets the list of the DTOs using the Query object.
        /// </summary>
        public virtual async Task<IEnumerable<TListDTO>> GetListAsync(CancellationToken cancellationToken, Action<IQuery<TListDTO>> queryConfiguration = null)
        {
            using (UnitOfWorkProvider.Create())
            {
                var query = QueryFactory();
                queryConfiguration?.Invoke(query);
                return await query.ExecuteAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Gets the list of the DTOs using the Query object.
        /// </summary>
        public virtual Task<IEnumerable<TListDTO>> GetListAsync(Action<IQuery<TListDTO>> queryConfiguration = null)
        {
            return GetListAsync(default(CancellationToken), queryConfiguration);
        }

        /// <summary>
        /// Transfers the changes on DTO made by the user to the corresponding database entity.
        /// </summary>
        protected virtual void PopulateDetailToEntity(TDetailDTO detail, TEntity entity)
        {
            Mapper.PopulateEntity(detail, entity);
        }

        /// <summary>
        /// Saves the changes made to the entity in the database, and if the entity was inserted, updates the DTO with its ID.
        /// </summary>
        /// <returns>New instance of DTO with changes reflected during saving.</returns>
        protected virtual TDetailDTO Save(TEntity entity, bool isNew, TDetailDTO detail, IUnitOfWork uow)
        {
            // insert or update
            if (isNew)
            {
                Repository.Insert(entity);
            }
            else
            {
                Repository.Update(entity);
            }

            // save
            uow.Commit();
            detail.Id = entity.Id;
            var savedDetail = Mapper.MapToDTO(entity);
            return savedDetail;
        }

        /// <summary>
        /// Saves the changes made to the entity in the database, and if the entity was inserted, updates the DTO with its ID.
        /// </summary>
        /// <returns>New instance of DTO with changes reflected during saving.</returns>
        protected virtual async Task<TDetailDTO> SaveAsync(CancellationToken cancellationToken, TEntity entity, bool isNew, TDetailDTO detail, IUnitOfWork uow)
        {
            // insert or update
            if (isNew)
            {
                Repository.Insert(entity);
            }
            else
            {
                Repository.Update(entity);
            }

            // save
            await uow.CommitAsync(cancellationToken);
            detail.Id = entity.Id;
            var savedDetail = Mapper.MapToDTO(entity);
            return savedDetail;
        }

        /// <summary>
        /// Gets a list of navigation property expressions that should be included when the facade loads the entity.
        /// </summary>
        protected virtual Expression<Func<TEntity, object>>[] EntityIncludes => new Expression<Func<TEntity, object>>[] { };       

        /// <summary>
        /// Validates that the entity detail can be displayed by the user. If the user does not have permissions, the method should throw an exception.
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void ValidateReadPermissions(TEntity entity)
        {
        }

        /// <summary>
        /// Validates that the entity detail can be displayed by the user. If the user does not have permissions, the method should throw an exception.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="entity"></param>
        protected virtual Task ValidateReadPermissionsAsync(CancellationToken cancellationToken, TEntity entity)
        {
            return cancellationToken.IsCancellationRequested ? Task.FromCanceled(cancellationToken) : Task.CompletedTask;
        }

        /// <summary>
        /// Validates that the entity can be modified by the current user. If the user does not have permissions, the method should throw an exception.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="stage">
        ///     The BeforeMap stage is called when an existing entity is loaded from the database and is about to be mapped. 
        ///     The AfterMap stage is called when the DTO was mapped to the entity and the entity is about to be saved.
        /// </param>
        protected virtual void ValidateModifyPermissions(TEntity entity, ModificationStage stage)
        {
        }

        /// <summary>
        /// Validates that the entity can be modified by the current user. If the user does not have permissions, the method should throw an exception.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="entity"></param>
        /// <param name="stage">
        ///     The BeforeMap stage is called when an existing entity is loaded from the database and is about to be mapped. 
        ///     The AfterMap stage is called when the DTO was mapped to the entity and the entity is about to be saved.
        /// </param>
        protected virtual Task ValidateModifyPermissionsAsync(CancellationToken cancellationToken, TEntity entity, ModificationStage stage)
        { 
            return cancellationToken.IsCancellationRequested ? Task.FromCanceled(cancellationToken) : Task.CompletedTask;
        }

    }
}