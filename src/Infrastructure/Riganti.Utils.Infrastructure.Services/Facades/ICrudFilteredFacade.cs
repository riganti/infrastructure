using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Services.Facades
{
    public interface ICrudFilteredFacade<TListDTO, TDetailDTO, TFilterDTO, in TKey> : IFacade, ICrudFilteredListFacade<TListDTO, TFilterDTO>, ICrudDetailFacade<TDetailDTO, TKey>
        where TDetailDTO : IEntity<TKey>
    {
    }
}