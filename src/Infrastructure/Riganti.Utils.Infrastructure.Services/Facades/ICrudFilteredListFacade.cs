using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Services.Facades
{
    public interface ICrudFilteredListFacade<TListDTO, TFilterDTO>
    {
        Func<IFilteredQuery<TListDTO, TFilterDTO>> QueryFactory { get; }

        IEnumerable<TListDTO> GetList(TFilterDTO filter, Action<IFilteredQuery<TListDTO, TFilterDTO>> queryConfiguration = null);

        Task<IEnumerable<TListDTO>> GetListAsync(TFilterDTO filter, Action<IFilteredQuery<TListDTO, TFilterDTO>> queryConfiguration = null);
    }
}