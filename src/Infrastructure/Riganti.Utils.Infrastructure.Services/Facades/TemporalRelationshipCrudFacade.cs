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

        // TODO: Tom slibil dodat comment
        public abstract Func<TRelationshipEntity, TKey> ParentEntityKeySelector { get; }
        // TODO: Tom slibil dodat comment
        public abstract Func<TRelationshipDTO, TKey> ParentDTOKeySelector { get; }

        // TODO: Tom slibil dodat comment
        public abstract Func<TRelationshipEntity, TKey> ChildEntityKeySelector { get; }
        // TODO: Tom slibil dodat comment
        public abstract Func<TRelationshipDTO, TKey> ChildDTOKeySelector { get; }

        // TODO: Tom slibil dodat comment
        public abstract Expression<Func<TParentEntity, ICollection<TRelationshipEntity>>> ChildEntityCollectionSelector { get; }
        // TODO: uplne zrusit propertu a nahradit ji propertou ChildEntityCollectionSelector
        public abstract Expression<Func<TParentEntity, object>> ChildEntityCollectionExpression { get; }
        // TODO: Tom slibil dodat comment
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
                var includes = AdditionalParentIncludes.Concat(new [] { ChildEntityCollectionExpression }).ToArray();
                var parentEntity = ParentRepository.GetById(parentId, includes);
                ValidateReadPermissions(parentEntity);
                return entityMapper.MapToDTO(entity);
            }
        }

        /// <summary>
        /// Adds a new member to the relationship. All current members received by <see cref="ChildDTOKeySelector" /> will be invalidated.
        /// </summary>
        public TRelationshipDTO Save(TRelationshipDTO relationship)
        {
            using (var uow = UnitOfWorkProvider.Create())
            {
                // get the entity collection in parent
                var relationshipCollection = GetRelationshipCollection(relationship);
                var identifierPropertyValue = ChildDTOKeySelector(relationship);

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
        /// Removes a member from the relationship. All current members received by <see cref="ChildDTOKeySelector" /> will be invalidated.
        /// </summary>
        public void Delete(TKey id)
        {
            using (UnitOfWorkProvider.Create())
            {
                var entity = Repository.GetById(id);
                if (entity == null)
                {
                    // entity was not found, nothing to delete
                    return;
                }

                var relationshipDto = entityMapper.MapToDTO(entity);
                Delete(relationshipDto);
            }
        }

        /// <summary>
        /// Removes a member from the relationship. All current members received by <see cref="ChildDTOKeySelector" /> will be invalidated.
        /// </summary>
        public void Delete(TRelationshipDTO relationship)
        {
            using (var uow = UnitOfWorkProvider.Create())
            {
                // get the entity collection in parent
                var now = dateTimeProvider.Now;
                var relationshipCollection = GetRelationshipCollection(relationship);
                var identifierPropertyValue = ChildDTOKeySelector(relationship);

                // invalidate all current records
                InvalidateEntities(relationshipCollection, identifierPropertyValue, now);

                uow.Commit();
            }
        }

        private ICollection<TRelationshipEntity> GetRelationshipCollection(TRelationshipDTO relationship)
        {
            var parentId = ParentDTOKeySelector(relationship);
            var includes = AdditionalParentIncludes.Concat(new[] { ChildEntityCollectionExpression }).ToArray();
            var parentEntity = ParentRepository.GetById(parentId, includes);
            ValidateModifyPermissions(parentEntity);

            var relationshipCollection = ChildEntityCollectionSelector.Compile()(parentEntity);
            return relationshipCollection;
        }
        
        protected virtual void InvalidateEntities(ICollection<TRelationshipEntity> relationshipCollection, TKey identifierPropertyValue, DateTime now)
        {
            foreach (var entity in relationshipCollection)
            {
                if (Equals(ChildEntityKeySelector(entity), identifierPropertyValue))
                {
                    if (entity.ValidityEndDate > now)
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