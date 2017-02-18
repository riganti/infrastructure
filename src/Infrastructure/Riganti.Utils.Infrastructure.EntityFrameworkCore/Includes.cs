using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.EntityFrameworkCore
{
    public static class Includes
    {

        public static IncludeDefinitionStub<TEntity> For<TEntity>() where TEntity : class
        {
            return new IncludeDefinitionStub<TEntity>();
        }

    }
}