using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Services.Facades
{
    public interface ICrudFacade<TListDTO, TDetailDTO, in TKey> : IFacade, ICrudListFacade<TListDTO>, ICrudDetailFacade<TDetailDTO, TKey> 
        where TDetailDTO : IEntity<TKey>
    {
    }
}