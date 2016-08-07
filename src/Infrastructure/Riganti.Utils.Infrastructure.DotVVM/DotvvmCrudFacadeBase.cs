using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Framework.Controls;
using Riganti.Utils.Infrastructure.Core;
using Riganti.Utils.Infrastructure.Services.Facades;

namespace Riganti.Utils.Infrastructure.DotVVM
{
    /// <summary>
    /// A base class for CRUD pages with DotVVM GridViewDataSet support.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the entity primary key.</typeparam>
    /// <typeparam name="TListDTO">The type of the DTO used in the list of records, e.g. in the GridView control.</typeparam>
    /// <typeparam name="TDetailDTO">The type of the DTO used in the detail form.</typeparam>
    public class DotvvmCrudFacadeBase<TEntity, TKey, TListDTO, TDetailDTO> : CrudFacadeBase<TEntity, TKey, TListDTO, TDetailDTO> where TEntity : IEntity<TKey> where TDetailDTO : IEntity<TKey>
    {

        public DotvvmCrudFacadeBase(IQuery<TListDTO> query, IRepository<TEntity, TKey> repository, IEntityDTOMapper<TEntity, TDetailDTO> mapper) : base(query, repository, mapper)
        {
        }

        /// <summary>
        /// Fills the data set using the query specified in the facade.
        /// </summary>
        public virtual void FillDataSet(GridViewDataSet<TListDTO> dataSet)
        {
            using (UnitOfWorkProvider.Create())
            {
                Query.Skip = dataSet.PageIndex * dataSet.PageSize;
                Query.Take = dataSet.PageSize;
                Query.SortCriteria.Clear();

                if (!string.IsNullOrEmpty(dataSet.SortExpression))
                {
                    Query.AddSortCriteria(dataSet.SortExpression, dataSet.SortDescending ? SortDirection.Descending : SortDirection.Ascending);
                }

                dataSet.TotalItemsCount = Query.GetTotalRowCount();
                dataSet.Items = Query.Execute();
            }    
        }
        

    }
}
