using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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
    public class CrudFacadeBase<TEntity, TKey, TListDTO, TDetailDTO> : FacadeBase where TEntity : IEntity<TKey> where TDetailDTO : IEntity<TKey>
    {
        /// <summary>
        /// Gets the query object used to populate the list or records.
        /// </summary>
        public IQuery<TListDTO> Query { get; private set; }

        /// <summary>
        /// Gets the repository used to perform database operations with the entity.
        /// </summary>
        public IRepository<TEntity, TKey> Repository { get; private set; }

        /// <summary>
        /// Gets the service that can map entities to DTOs and populate entities with changes made on DTOs.
        /// </summary>
        public IEntityDTOMapper<TEntity, TDetailDTO> Mapper { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="CrudFacadeBase{TEntity, TKey, TListDTO, TDetailDTO}"/> class.
        /// </summary>
        public CrudFacadeBase(IQuery<TListDTO> query, IRepository<TEntity, TKey> repository, IEntityDTOMapper<TEntity, TDetailDTO> mapper)
        {
            this.Query = query;
            this.Repository = repository;
            this.Mapper = mapper;
        }

        /// <summary>
        /// Gets the detail DTO for an entity with the specified ID.
        /// </summary>
        public virtual TDetailDTO GetDetail(TKey id)
        {
            using (UnitOfWorkProvider.Create())
            {
                var entity = Repository.GetById(id, EntityIncludes);
                var detail = Mapper.MapToDTO(entity);
                return detail;
            }
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
        public virtual void Save(TDetailDTO detail)
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
                }

                // populate the entity
                PopulateDetailToEntity(detail, entity);

                // save
                Save(entity, isNew, detail, uow);
            }
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
        /// Gets the list of the DTOs using the Query object.
        /// </summary>
        public virtual IEnumerable<TListDTO> GetList()
        {
            using (UnitOfWorkProvider.Create())
            {
                return Query.Execute();
            }
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
        protected virtual void Save(TEntity entity, bool isNew, TDetailDTO detail, IUnitOfWork uow)
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
        }

        /// <summary>
        /// Gets a list of navigation property expressions that should be included when the facade loads the entity.
        /// </summary>
        protected virtual Expression<Func<TEntity, object>>[] EntityIncludes => new Expression<Func<TEntity, object>>[] { };


    }
}
