using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Services.Facades
{
    public interface ICrudListFacade<TListDTO>
    {
        Func<IQuery<TListDTO>> QueryFactory { get; }

        IEnumerable<TListDTO> GetList(Action<IQuery<TListDTO>> queryConfiguration = null);

        Task<IEnumerable<TListDTO>> GetListAsync(Action<IQuery<TListDTO>> queryConfiguration = null, CancellationToken cancellationToken = default);

    }
}