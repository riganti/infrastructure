using System;
using System.Collections.Generic;
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
    /// <typeparam name="TFilterDTO">The type of the DTO used for filtering the list.</typeparam>
    public abstract class FilteredCrudFacadeBase<TEntity, TKey, TListDTO, TDetailDTO, TFilterDTO> : CrudFacadeBase<TEntity, TKey, TListDTO, TDetailDTO>, ICrudFilteredFacade<TListDTO, TDetailDTO, TFilterDTO, TKey>
        where TEntity : IEntity<TKey> 
        where TDetailDTO : IEntity<TKey>
    {

        public new Func<IFilteredQuery<TListDTO, TFilterDTO>> QueryFactory => (Func<IFilteredQuery<TListDTO, TFilterDTO>>) base.QueryFactory;

        protected FilteredCrudFacadeBase(Func<IFilteredQuery<TListDTO, TFilterDTO>> queryFactory, IRepository<TEntity, TKey> repository, IEntityDTOMapper<TEntity, TDetailDTO> mapper) 
            : base(queryFactory, repository, mapper)
        {
        }

        /// <summary>
        /// Gets the list of the DTOs using the Query object and filter.
        /// </summary>
        public virtual IEnumerable<TListDTO> GetList(TFilterDTO filter, Action<IFilteredQuery<TListDTO, TFilterDTO>> queryConfiguration = null)
        {
            using (UnitOfWorkProvider.Create())
            {
                var query = QueryFactory();
                query.Filter = filter;
                queryConfiguration?.Invoke(query);
                return query.Execute();
            }
        }

    }
}
