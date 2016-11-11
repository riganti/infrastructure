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
    public abstract class FilteredCrudFacadeBase<TEntity, TKey, TListDTO, TDetailDTO, TFilterDTO> : CrudFacadeBase<TEntity, TKey, TListDTO, TDetailDTO>
        where TEntity : IEntity<TKey> 
        where TDetailDTO : IEntity<TKey>
    {

        public new IFilteredQuery<TListDTO, TFilterDTO> Query => (IFilteredQuery<TListDTO, TFilterDTO>) base.Query;

        public FilteredCrudFacadeBase(IFilteredQuery<TListDTO, TFilterDTO> query, IRepository<TEntity, TKey> repository, IEntityDTOMapper<TEntity, TDetailDTO> mapper) 
            : base(query, repository, mapper)
        {
        }
        
        public IEnumerable<TListDTO> GetList(TFilterDTO filter)
        {
            Query.Filter = filter;
            return base.GetList();
        }
    }
}
