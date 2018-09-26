using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Services.Facades
{
    public interface IFacade
    {
        IUnitOfWorkProvider UnitOfWorkProvider { get; }
    }
}