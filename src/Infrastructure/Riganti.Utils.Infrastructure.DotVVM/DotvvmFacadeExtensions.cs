using System.Threading;
using System.Threading.Tasks;
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
        public static void FillDataSet<TListDTO>(this ICrudListFacade<TListDTO> facade,
            GridViewDataSet<TListDTO> dataSet,
            IUnitOfWorkProvider unitOfWorkProvider)
        {
            using (unitOfWorkProvider.Create())
            {
                var query = facade.QueryFactory();
                dataSet.LoadFromQuery(query);
            }
        }
        
        /// <summary>
        /// Fills the data set using the query specified in the facade.
        /// </summary>
        public static void FillDataSet<TListDTO, TFilterDTO>(this ICrudFilteredListFacade<TListDTO, TFilterDTO> facade,
            GridViewDataSet<TListDTO> dataSet,
            TFilterDTO filter,
            IUnitOfWorkProvider unitOfWorkProvider)
        {
            using (unitOfWorkProvider.Create())
            {
                var query = facade.QueryFactory();
                query.Filter = filter;
                dataSet.LoadFromQuery(query);
            }
        }
        
        /// <summary>
        /// Fills the data set using the query specified in the facade.
        /// </summary>
        public static void FillDataSet<TKey, TListDTO, TDetailDTO>(this ICrudFacade<TListDTO, TDetailDTO, TKey> facade,
            GridViewDataSet<TListDTO> dataSet)
            where TDetailDTO : IEntity<TKey>
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
        public static void FillDataSet<TKey, TListDTO, TDetailDTO, TFilterDTO>(
            this ICrudFilteredFacade<TListDTO, TDetailDTO, TFilterDTO, TKey> facade, GridViewDataSet<TListDTO> dataSet,
            TFilterDTO filter) where TDetailDTO : IEntity<TKey>
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
                query.AddSortCriteria(dataSet.SortingOptions.SortExpression,
                    dataSet.SortingOptions.SortDescending ? SortDirection.Descending : SortDirection.Ascending);
            }

            dataSet.PagingOptions.TotalItemsCount = query.GetTotalRowCount();
            dataSet.Items = query.Execute();
            dataSet.IsRefreshRequired = false;
        }

        /// <summary>
        /// Fills the data set using the query specified in the facade.
        /// </summary>
        public static async Task FillDataSetAsync<TKey, TListDTO, TDetailDTO>(this ICrudFacade<TListDTO, TDetailDTO, TKey> facade,
            GridViewDataSet<TListDTO> dataSet)
            where TDetailDTO : IEntity<TKey>
        {
            using (facade.UnitOfWorkProvider.Create())
            {
                var query = facade.QueryFactory();
                await dataSet.LoadFromQueryAsync(query);
            }
        }

        /// <summary>
        /// Fills the data set using the query specified in the facade.
        /// </summary>
        public static async Task FillDataSetAsync<TKey, TListDTO, TDetailDTO, TFilterDTO>(
            this ICrudFilteredFacade<TListDTO, TDetailDTO, TFilterDTO, TKey> facade, GridViewDataSet<TListDTO> dataSet,
            TFilterDTO filter) where TDetailDTO : IEntity<TKey>
        {
            using (facade.UnitOfWorkProvider.Create())
            {
                var query = facade.QueryFactory();
                query.Filter = filter;
                await dataSet.LoadFromQueryAsync(query);
            }
        }

        /// <summary>
        /// Fills the GridViewDataSet from the specified query object.
        /// </summary>
        public static async Task LoadFromQueryAsync<T>(this GridViewDataSet<T> dataSet, IQuery<T> query, CancellationToken cancellationToken = default)
        {
            query.Skip = dataSet.PagingOptions.PageIndex * dataSet.PagingOptions.PageSize;
            query.Take = dataSet.PagingOptions.PageSize;
            query.ClearSortCriteria();

            if (!string.IsNullOrEmpty(dataSet.SortingOptions.SortExpression))
            {
                query.AddSortCriteria(dataSet.SortingOptions.SortExpression,
                    dataSet.SortingOptions.SortDescending ? SortDirection.Descending : SortDirection.Ascending);
            }

            dataSet.PagingOptions.TotalItemsCount = await query.GetTotalRowCountAsync(cancellationToken);
            dataSet.Items = await query.ExecuteAsync(cancellationToken);
            dataSet.IsRefreshRequired = false;
        }
    }
}