using Moq;

#if EFCORE
using Microsoft.EntityFrameworkCore;
#else
using System.Data.Entity;
#endif

#if EFCORE
namespace Riganti.Utils.Infrastructure.EntityFrameworkCore.Tests.Repository
#else
namespace Riganti.Utils.Infrastructure.EntityFramework.Tests.Repository
# endif
{
    internal class DbContextMockFactory
    {
        internal Mock<TDbContext> CreateDbContextMock<TDbContext>() where TDbContext : DbContext
        {
            var dbContextMock = new Mock<TDbContext> { CallBase = true };

            return dbContextMock;
        }
    }
}