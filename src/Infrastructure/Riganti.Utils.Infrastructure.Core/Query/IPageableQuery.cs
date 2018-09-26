using System.Threading;
using System.Threading.Tasks;

namespace Riganti.Utils.Infrastructure.Core
{
    public interface IPageableQuery
    {
        /// <summary>
        /// Gets or sets a number of rows to be skipped. If this value is null, the paging will be applied.
        /// </summary>
        int? Skip { get; set; }

        /// <summary>
        /// Gets or sets the page size. If this value is null, the paging will not be applied.
        /// </summary>
        int? Take { get; set; }
        
        /// <summary>
        /// Gets the total row count without respect to paging.
        /// </summary>
        int GetTotalRowCount();

        /// <summary>
        /// Gets the total row count without respect to paging.
        /// </summary>
        Task<int> GetTotalRowCountAsync();

        /// <summary>
        /// Gets the total row count without respect to paging.
        /// </summary>
        Task<int> GetTotalRowCountAsync(CancellationToken cancellationToken);
    }
}