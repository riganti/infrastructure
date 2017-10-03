using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Services.Facades
{
    public interface ICrudDetailFacade<TDetailDTO, in TKey>
        where TDetailDTO : IEntity<TKey>
    {
        TDetailDTO InitializeNew();

        TDetailDTO GetDetail(TKey id);

        TDetailDTO Save(TDetailDTO data);

        void Delete(TKey id);
    }
}