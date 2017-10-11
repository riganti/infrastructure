using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Services.Facades
{
    /// <summary>
    /// A CRUD facade for the M:N relationship entities with time-restricted validity.
    /// </summary>
    public abstract class TemporalRelationshipCrudFacade<TRelationshipEntity, TParentEntity, TKey, TRelationshipDTO, TFilterDTO> 
        : FacadeBase, ICrudFilteredFacade<TRelationshipDTO, TRelationshipDTO, TFilterDTO, TKey>
        where TRelationshipEntity : class, IEntity<TKey>, ITemporalEntity
        where TParentEntity : IEntity<TKey> 
        where TRelationshipDTO : IEntity<TKey>, new()
    {
        private readonly IEntityDTOMapper<TRelationshipEntity, TRelationshipDTO> entityMapper;
        private readonly IDateTimeProvider dateTimeProvider;

        public Func<IFilteredQuery<TRelationshipDTO, TFilterDTO>> QueryFactory { get; }
        public IRepository<TRelationshipEntity, TKey> Repository { get; }
        public IRepository<TParentEntity, TKey> ParentRepository { get; }

        /// <summary>
        /// Gets a function which selects a key of the parent entity.
        /// </summary>
        public abstract Func<TRelationshipEntity, TKey> ParentEntityKeySelector { get; }

        /// <summary>
        /// Gets a function which selects a key of the parent DTO.
        /// </summary>
        public abstract Func<TRelationshipDTO, TKey> ParentDTOKeySelector { get; }

        /// <summary>
        /// Gets a function which selects a key of the secondary entity.
        /// </summary>
        public abstract Func<TRelationshipEntity, TKey> SecondaryEntityKeySelector { get; }

        /// <summary>
        /// Gets a function which selects a key of the secondary DTO.
        /// </summary>
        public abstract Func<TRelationshipDTO, TKey> SecondaryDTOKeySelector { get; }

        /// <summary>
        /// Gets an expression which selects a collection of relationship entities from the parent entity.
        /// </summary>
        public abstract Expression<Func<TParentEntity, ICollection<TRelationshipEntity>>> RelationshipCollectionSelector { get; }
        
        /// <summary>
        /// Gets an array of expressions which defines additional navigation properties to be included when the parent entity is loaded.
        /// </summary>
        public virtual Expression<Func<TParentEntity, object>>[] AdditionalParentIncludes => new Expression<Func<TParentEntity, object>>[0];

        protected TemporalRelationshipCrudFacade(Func<IFilteredQuery<TRelationshipDTO, TFilterDTO>> queryFactory,
                                                 IEntityDTOMapper<TRelationshipEntity, TRelationshipDTO> entityMapper,
                                                 IRepository<TRelationshipEntity, TKey> repository,
                                                 IRepository<TParentEntity, TKey> parentRepository,
                                                 IDateTimeProvider dateTimeProvider,
                                                 IUnitOfWorkProvider unitOfWorkProvider)
        {
            this.entityMapper = entityMapper;
            this.dateTimeProvider = dateTimeProvider;

            QueryFactory = queryFactory;
            Repository = repository;
            ParentRepository = parentRepository;
            UnitOfWorkProvider = unitOfWorkProvider;
        }

        /// <summary>
        /// Gets a list of records in the relationship.
        /// </summary>
        public IEnumerable<TRelationshipDTO> GetList(TFilterDTO filter, Action<IFilteredQuery<TRelationshipDTO, TFilterDTO>> queryConfiguration = null)
        {
            using (UnitOfWorkProvider.Create())
            {
                var query = QueryFactory();
                query.Filter = filter;
                queryConfiguration?.Invoke(query);
                return query.Execute();
            }
        }

        public TRelationshipDTO InitializeNew()
        {
            return new TRelationshipDTO();
        }

        public TRelationshipDTO GetDetail(TKey id)
        {
            using (UnitOfWorkProvider.Create())
            {
                var entity = Repository.GetById(id);
                if (entity == null)
                {
                    return default(TRelationshipDTO);
                }

                var parentId = ParentEntityKeySelector(entity);
                var includes = AdditionalParentIncludes.Concat(new [] { ConvertToIncludesExpression(RelationshipCollectionSelector) }).ToArray();
                var parentEntity = ParentRepository.GetById(parentId, includes);
                ValidateReadPermissions(parentEntity);
                return entityMapper.MapToDTO(entity);
            }
        }

        private Expression<Func<TParentEntity, object>> ConvertToIncludesExpression(Expression<Func<TParentEntity, ICollection<TRelationshipEntity>>> relationshipCollectionSelector)
        {
            return Expression.Lambda<Func<TParentEntity, object>>(relationshipCollectionSelector.Body, relationshipCollectionSelector.Parameters);
        }

        /// <summary>
        /// Adds a new member to the relationship. All current members received by <see cref="SecondaryDTOKeySelector" /> will be invalidated.
        /// </summary>
        public TRelationshipDTO Save(TRelationshipDTO relationship)
        {
            using (var uow = UnitOfWorkProvider.Create())
            {
                // get the entity collection in parent
                var relationshipCollection = GetRelationshipCollection(relationship);
                var identifierPropertyValue = SecondaryDTOKeySelector(relationship);

                // invalidate all current records
                var now = dateTimeProvider.Now;
                InvalidateEntities(relationshipCollection, identifierPropertyValue, now);
                
                // insert a new entity
                var entity = Repository.InitializeNew();
                entityMapper.PopulateEntity(relationship, entity);
                entity.Id = default(TKey);
                BeginEntityValidityPeriod(entity, now);
                relationshipCollection.Add(entity);

                uow.Commit();

                relationship.Id = entity.Id;
                var savedrelationship = entityMapper.MapToDTO(entity);
                return savedrelationship;
            }
        }

        /// <summary>
        /// Removes a member from the relationship. All current members received by <see cref="SecondaryDTOKeySelector" /> will be invalidated.
        /// </summary>
        public void Delete(TKey id)
        {
            using (var uow = UnitOfWorkProvider.Create())
            {
                var entity = Repository.GetById(id);
                if (entity == null)
                {
                    // entity was not found, nothing to delete
                    return;
                }

                var relationshipDto = entityMapper.MapToDTO(entity);
                Delete(relationshipDto);

                uow.Commit();
            }
        }

        /// <summary>
        /// Removes a member from the relationship. All current members received by <see cref="SecondaryDTOKeySelector" /> will be invalidated.
        /// </summary>
        public void Delete(TRelationshipDTO relationship)
        {
            using (var uow = UnitOfWorkProvider.Create())
            {
                // get the entity collection in parent
                var now = dateTimeProvider.Now;
                var relationshipCollection = GetRelationshipCollection(relationship);
                var identifierPropertyValue = SecondaryDTOKeySelector(relationship);

                // invalidate all current records
                InvalidateEntities(relationshipCollection, identifierPropertyValue, now);

                uow.Commit();
            }
        }

        private ICollection<TRelationshipEntity> GetRelationshipCollection(TRelationshipDTO relationship)
        {
            var parentId = ParentDTOKeySelector(relationship);
            var includes = AdditionalParentIncludes.Concat(new[] { ConvertToIncludesExpression(RelationshipCollectionSelector) }).ToArray();
            var parentEntity = ParentRepository.GetById(parentId, includes);
            ValidateModifyPermissions(parentEntity);

            var relationshipCollection = RelationshipCollectionSelector.Compile()(parentEntity);
            return relationshipCollection;
        }
        
        protected virtual void InvalidateEntities(ICollection<TRelationshipEntity> relationshipCollection, TKey identifierPropertyValue, DateTime now)
        {
            foreach (var entity in relationshipCollection)
            {
                if (Equals(SecondaryEntityKeySelector(entity), identifierPropertyValue))
                {
                    if (entity.ValidityEndDate == null || entity.ValidityEndDate > now)
                    {
                        EndEntityValidityPeriod(entity, now);
                    }
                }
            }
        }

        protected virtual void BeginEntityValidityPeriod(TRelationshipEntity entity, DateTime now)
        {
            entity.ValidityBeginDate = now;
        }

        protected virtual void EndEntityValidityPeriod(TRelationshipEntity entity, DateTime now)
        {
            entity.ValidityEndDate = now;
        }

        protected virtual void ValidateReadPermissions(TParentEntity entity)
        {
            if (!HasReadPermissions(entity))
            {
                throw new UnauthorizedAccessException();
            }
        }

        protected virtual bool HasReadPermissions(TParentEntity entity) => true;

        protected virtual void ValidateModifyPermissions(TParentEntity entity)
        {
            if (!HasModifyPermissions(entity))
            {
                throw new UnauthorizedAccessException();
            }
        }

        protected virtual bool HasModifyPermissions(TParentEntity entity) => true;
    }
}