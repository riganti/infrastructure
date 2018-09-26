using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Services.Facades
{
    public class FacadeBase : IFacade
    {

        public IUnitOfWorkProvider UnitOfWorkProvider { get; set; }

    }
}