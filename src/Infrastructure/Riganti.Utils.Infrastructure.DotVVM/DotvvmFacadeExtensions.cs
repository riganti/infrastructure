using DotVVM.Framework.Controls;
using Riganti.Utils.Infrastructure.Core;
using Riganti.Utils.Infrastructure.Services.Facades;

namespace Riganti.Utils.Infrastructure
{
    public static class DotvvmFacadeExtensions
    {
        
        /// <summary>
        /// Fills the data set using the query specified in the facade.
        /// </summary>
        public static void FillDataSet<TEntity, TKey, TListDTO, TDetailDTO>(this CrudFacadeBase<TEntity, TKey, TListDTO, TDetailDTO> facade, GridViewDataSet<TListDTO> dataSet) 
            where TEntity : IEntity<TKey> where TDetailDTO : IEntity<TKey>
        {
            using (facade.UnitOfWorkProvider.Create())
            {
                var query = facade.QueryFactory();
                dataSet.LoadFromQuery(query);
            }    
        }

        /// <summary>
        /// Fills the data set using the query specified in the facade.
        /// </summary>
        public static void FillDataSet<TEntity, TKey, TListDTO, TDetailDTO, TFilterDTO>(this FilteredCrudFacadeBase<TEntity, TKey, TListDTO, TDetailDTO, TFilterDTO> facade, GridViewDataSet<TListDTO> dataSet, TFilterDTO filter)
            where TEntity : IEntity<TKey> where TDetailDTO : IEntity<TKey>
        {
            using (facade.UnitOfWorkProvider.Create())
            {
                var query = facade.QueryFactory();
                query.Filter = filter;
                dataSet.LoadFromQuery(query);
            }
        }

        /// <summary>
        /// Fills the GridViewDataSet from the specified query object.
        /// </summary>
        public static void LoadFromQuery<T>(this GridViewDataSet<T> dataSet, IQuery<T> query)
        {
            query.Skip = dataSet.PagingOptions.PageIndex * dataSet.PagingOptions.PageSize;
            query.Take = dataSet.PagingOptions.PageSize;
            query.ClearSortCriteria();

            if (!string.IsNullOrEmpty(dataSet.SortingOptions.SortExpression))
            {
                query.AddSortCriteria(dataSet.SortingOptions.SortExpression, dataSet.SortingOptions.SortDescending ? SortDirection.Descending : SortDirection.Ascending);
            }

            dataSet.PagingOptions.TotalItemsCount = query.GetTotalRowCount();
            dataSet.Items = query.Execute();
        }

    }
}
