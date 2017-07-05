using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Riganti.Utils.Infrastructure.Core;
using Xunit;

namespace Riganti.Utils.Infrastructure.EntityFramework.Tests.Repository
{
    public class EntityFrameworkUnitOfProviderTests
    {

        public class CustomDbContext1 : DbContext
        {
        }

        public class CustomDbContext2 : DbContext
        {
        }

        [Fact]
        public void NestedDbContexts()
        {
            var registry = new ThreadLocalUnitOfWorkRegistry();
            var uowp1 = new EntityFrameworkUnitOfWorkProvider<CustomDbContext1>(registry, () => new CustomDbContext1());
            var uowp2 = new EntityFrameworkUnitOfWorkProvider<CustomDbContext2>(registry, () => new CustomDbContext2());

            using (var uow1 = uowp1.Create())
            {
                using (var uow2 = uowp2.Create())
                {
                    var current = registry.GetCurrent(0);
                    Assert.Equal(uow2, current);

                    var parent = registry.GetCurrent(1);
                    Assert.Equal(uow1, parent);

                    var inner = EntityFrameworkUnitOfWork.TryGetDbContext<CustomDbContext2>(uowp2);
                    Assert.Equal(inner, ((EntityFrameworkUnitOfWork<CustomDbContext2>)uow2).Context);

                    var outer = EntityFrameworkUnitOfWork.TryGetDbContext<CustomDbContext1>(uowp1);
                    Assert.Equal(outer, ((EntityFrameworkUnitOfWork<CustomDbContext1>)uow1).Context);
                }
            }
        }

    }
}
