using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.EntityFrameworkCore
{
    public interface IChainableIncludeDefinition<TEntity, TResult>
        where TEntity : class where TResult : class
    {
    }
}