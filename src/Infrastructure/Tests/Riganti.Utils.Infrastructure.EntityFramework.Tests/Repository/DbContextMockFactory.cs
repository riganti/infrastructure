using System.Data.Entity;
using Moq;

namespace Riganti.Utils.Infrastructure.EntityFramework.Tests.Repository
{
    internal class DbContextMockFactory
    {
        internal Mock<TDbContext> CreateDbContextMock<TDbContext>() where TDbContext : DbContext
        {
            var dbContextMock = new Mock<TDbContext> {CallBase = true};

            return dbContextMock;
        }
    }
}