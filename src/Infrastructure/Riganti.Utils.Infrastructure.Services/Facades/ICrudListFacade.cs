using System;
using System.Collections.Generic;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Services.Facades
{
    public interface ICrudListFacade<TListDTO>
    {
        IEnumerable<TListDTO> GetList(Action<IQuery<TListDTO>> queryConfiguration = null);
    }
}