using DotVVM.Framework.Controls;
using Riganti.Utils.Infrastructure.Core;
using Riganti.Utils.Infrastructure.Services.Facades;

// ReSharper disable once CheckNamespace
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
                facade.Query.Skip = dataSet.PageIndex * dataSet.PageSize;
                facade.Query.Take = dataSet.PageSize;
                facade.Query.SortCriteria.Clear();

                if (!string.IsNullOrEmpty(dataSet.SortExpression))
                {
                    facade.Query.AddSortCriteria(dataSet.SortExpression, dataSet.SortDescending ? SortDirection.Descending : SortDirection.Ascending);
                }

                dataSet.TotalItemsCount = facade.Query.GetTotalRowCount();
                dataSet.Items = facade.Query.Execute();
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
                facade.Query.Filter = filter;
                facade.Query.Skip = dataSet.PageIndex * dataSet.PageSize;
                facade.Query.Take = dataSet.PageSize;
                facade.Query.SortCriteria.Clear();

                if (!string.IsNullOrEmpty(dataSet.SortExpression))
                {
                    facade.Query.AddSortCriteria(dataSet.SortExpression, dataSet.SortDescending ? SortDirection.Descending : SortDirection.Ascending);
                }

                dataSet.TotalItemsCount = facade.Query.GetTotalRowCount();
                dataSet.Items = facade.Query.Execute();
            }
        }

    }
}
