using System.Threading;
using System.Threading.Tasks;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Services.Facades
{
    public interface ICrudDetailFacade<TDetailDTO, in TKey>
        where TDetailDTO : IEntity<TKey>
    {
        TDetailDTO InitializeNew();

        TDetailDTO GetDetail(TKey id);

        Task<TDetailDTO> GetDetailAsync(TKey id, CancellationToken cancellationToken = default);

        TDetailDTO Save(TDetailDTO data);

        Task<TDetailDTO> SaveAsync(TDetailDTO detail, CancellationToken cancellationToken = default);

        void Delete(TKey id);

        Task DeleteAsync(TKey id, CancellationToken cancellationToken = default);
    }
}