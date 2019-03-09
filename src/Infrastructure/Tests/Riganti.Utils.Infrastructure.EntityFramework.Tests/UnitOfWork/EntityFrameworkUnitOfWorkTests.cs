using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Riganti.Utils.Infrastructure.Core;
using Xunit;

#if EFCORE
using Microsoft.EntityFrameworkCore;
#else

using System.Data.Entity;

#endif

#if EFCORE
namespace Riganti.Utils.Infrastructure.EntityFrameworkCore.Tests.UnitOfWork
#else

namespace Riganti.Utils.Infrastructure.EntityFramework.Tests.UnitOfWork
#endif
{
    public class EntityFrameworkUnitOfWorkTests
    {
        [Fact]
        public void Commit_CallCommitCoreOnlyIfHasOwnDbContext()
        {
            Func<DbContext> dbContextFactory = () => new Mock<DbContext>().Object;

            var unitOfWorkRegistryStub = new ThreadLocalUnitOfWorkRegistry();

            var unitOfWorkProvider = new EntityFrameworkUnitOfWorkProvider(unitOfWorkRegistryStub, dbContextFactory);

            var unitOfWorkParentMock = new Mock<EntityFrameworkUnitOfWork>(unitOfWorkProvider, dbContextFactory, DbContextOptions.ReuseParentContext) { CallBase = true };
            using (var unitOfWorkParent = unitOfWorkParentMock.Object)
            {
                unitOfWorkRegistryStub.RegisterUnitOfWork(unitOfWorkParent);

                var unitOfWorkChildMock = new Mock<EntityFrameworkUnitOfWork>(unitOfWorkProvider, dbContextFactory, DbContextOptions.ReuseParentContext) { CallBase = true };
                using (var unitOfWorkChild = unitOfWorkChildMock.Object)
                {
                    unitOfWorkChild.Commit();
                }
                unitOfWorkChildMock.Protected().Verify("CommitCore", Times.Never());

                unitOfWorkParent.Commit();
            }
            unitOfWorkParentMock.Protected().Verify("CommitCore", Times.Once());
        }

        [Fact]
        public void Commit_CorrectChildRequestIgnoredBehavior()
        {
            Func<DbContext> dbContextFactory = () => new Mock<DbContext>().Object;

            var unitOfWorkRegistryStub = new ThreadLocalUnitOfWorkRegistry();

            var unitOfWorkProvider = new EntityFrameworkUnitOfWorkProvider(unitOfWorkRegistryStub, dbContextFactory);

            Assert.Throws<ChildCommitPendingException>(() =>
            {
                using (unitOfWorkProvider.Create(DbContextOptions.ReuseParentContext))
                {
                    using (var unitOfWorkChild = unitOfWorkProvider.Create(DbContextOptions.ReuseParentContext))
                    {
                        unitOfWorkChild.Commit();
                    }
                }
            });

            // test that unit of work provider keeps working after caught exception
            using (unitOfWorkProvider.Create(DbContextOptions.ReuseParentContext))
            {
                using (unitOfWorkProvider.Create(DbContextOptions.ReuseParentContext))
                {
                }
            }
        }

        [Fact]
        public void Commit_CorrectMultipleLayeredReuseParentBehavior()
        {
            Func<DbContext> dbContextFactory = () => new Mock<DbContext>().Object;

            var unitOfWorkRegistryStub = new ThreadLocalUnitOfWorkRegistry();

            var unitOfWorkProvider = new EntityFrameworkUnitOfWorkProvider(unitOfWorkRegistryStub, dbContextFactory);

            using (var unitOfWorkParent = unitOfWorkProvider.Create(DbContextOptions.ReuseParentContext))
            {
                // 1st level, context 1
                using (unitOfWorkProvider.Create(DbContextOptions.ReuseParentContext))
                {
                    // 2nd level, context 1
                    using (unitOfWorkProvider.Create(DbContextOptions.AlwaysCreateOwnContext))
                    {
                        // 3rd level, context 2
                        using (unitOfWorkProvider.Create(DbContextOptions.ReuseParentContext))
                        {
                            // 4th level, context 2
                            using (var unitOfWorkParent3 = unitOfWorkProvider.Create(DbContextOptions.AlwaysCreateOwnContext))
                            {
                                // 5th level, context 3
                                using (unitOfWorkProvider.Create(DbContextOptions.ReuseParentContext))
                                {
                                    // 6th level, context 3
                                    using (unitOfWorkProvider.Create(DbContextOptions.ReuseParentContext))
                                    {
                                    }
                                    using (var unitOfWorkChild3 = unitOfWorkProvider.Create(DbContextOptions.ReuseParentContext))
                                    {
                                        // 7th level, context 3 commit requested
                                        unitOfWorkChild3.Commit();
                                    }
                                }
                                // commit mandatory, context 3 commit pending
                                unitOfWorkParent3.Commit();
                            }
                        }
                    }
                }
                // commit optional, no reusing child commit pending
                unitOfWorkParent.Commit();
            }
        }

        [Fact]
        public void Commit_UOWHasNotParrent_CallCommitCore()
        {
            Func<DbContext> dbContextFactory = () => new Mock<DbContext>().Object;

            var unitOfWorkRegistryStub = new ThreadLocalUnitOfWorkRegistry();

            var unitOfWorkProvider = new EntityFrameworkUnitOfWorkProvider(unitOfWorkRegistryStub, dbContextFactory);

            var unitOfWorkParentMock = new Mock<EntityFrameworkUnitOfWork>(unitOfWorkProvider, dbContextFactory, DbContextOptions.ReuseParentContext) { CallBase = true };
            using (var unitOfWorkParent = unitOfWorkParentMock.Object)
            {
                unitOfWorkParent.Commit();
            }
            unitOfWorkParentMock.Protected().Verify("CommitCore", Times.Once());
        }

        [Fact]
        public void CommitAsync_UOWHasChild_CallCommitCore()
        {
            Func<DbContext> dbContextFactory = () => new Mock<DbContext>().Object;

            var unitOfWorkRegistryStub = new ThreadLocalUnitOfWorkRegistry();

            var unitOfWorkProvider = new EntityFrameworkUnitOfWorkProvider(unitOfWorkRegistryStub, dbContextFactory);

            var unitOfWorkParentMock = new Mock<EntityFrameworkUnitOfWork>(unitOfWorkProvider, dbContextFactory, DbContextOptions.ReuseParentContext) { CallBase = true };
            using (var unitOfWorkParent = unitOfWorkParentMock.Object)
            {
                unitOfWorkRegistryStub.RegisterUnitOfWork(unitOfWorkParent);

                using (var unitOfWorkChild = new EntityFrameworkUnitOfWork(unitOfWorkProvider, dbContextFactory, DbContextOptions.ReuseParentContext))
                {
                    unitOfWorkChild.Commit();
                }

                unitOfWorkParent.Commit();
            }
            unitOfWorkParentMock.Protected().Verify("CommitCore", Times.Once());
        }

        [Fact]
        public async Task CommitAsync_CallCommitCoreOnlyIfHasOwnDbContext()
        {
            Func<DbContext> dbContextFactory = () => new Mock<DbContext>().Object;

            var unitOfWorkRegistryStub = new ThreadLocalUnitOfWorkRegistry();

            var unitOfWorkProvider = new EntityFrameworkUnitOfWorkProvider(unitOfWorkRegistryStub, dbContextFactory);

            var unitOfWorkParentMock = new Mock<EntityFrameworkUnitOfWork>(unitOfWorkProvider, dbContextFactory, DbContextOptions.ReuseParentContext) { CallBase = true };
            using (var unitOfWorkParent = unitOfWorkParentMock.Object)
            {
                unitOfWorkRegistryStub.RegisterUnitOfWork(unitOfWorkParent);

                var unitOfWorkChildMock = new Mock<EntityFrameworkUnitOfWork>(unitOfWorkProvider, dbContextFactory, DbContextOptions.ReuseParentContext) { CallBase = true };
                using (var unitOfWorkChild = unitOfWorkChildMock.Object)
                {
                    await unitOfWorkChild.CommitAsync();
                }
                unitOfWorkChildMock.Protected().Verify("CommitAsyncCore", Times.Never(), new CancellationToken());

                await unitOfWorkParent.CommitAsync();
            }
            unitOfWorkParentMock.Protected().Verify("CommitAsyncCore", Times.Once(), new CancellationToken());
        }

        [Fact]
        public async Task CommitAsync_ThrowIfChildCommitRequestedNotFulfilledByRoot()
        {
            Func<DbContext> dbContextFactory = () => new Mock<DbContext>().Object;

            var unitOfWorkRegistryStub = new ThreadLocalUnitOfWorkRegistry();

            var unitOfWorkProvider = new EntityFrameworkUnitOfWorkProvider(unitOfWorkRegistryStub, dbContextFactory);

            await Assert.ThrowsAsync<ChildCommitPendingException>(async () =>
            {
                using (unitOfWorkProvider.Create(DbContextOptions.ReuseParentContext))
                {
                    using (var unitOfWorkChild = unitOfWorkProvider.Create(DbContextOptions.ReuseParentContext))
                    {
                        await unitOfWorkChild.CommitAsync();
                    }
                }
            });

            // test that unit of work provider keeps working after caught exception
            using (unitOfWorkProvider.Create(DbContextOptions.ReuseParentContext))
            {
                using (unitOfWorkProvider.Create(DbContextOptions.ReuseParentContext))
                {
                }
            }
        }

        [Fact]
        public async Task CommitAsync_CorrectMultipleLayeredReuseParentBehavior()
        {
            Func<DbContext> dbContextFactory = () => new Mock<DbContext>().Object;

            var unitOfWorkRegistryStub = new ThreadLocalUnitOfWorkRegistry();

            var unitOfWorkProvider = new EntityFrameworkUnitOfWorkProvider(unitOfWorkRegistryStub, dbContextFactory);

            using (var unitOfWorkParent = unitOfWorkProvider.Create(DbContextOptions.ReuseParentContext))
            {
                // 1st level, context 1
                using (unitOfWorkProvider.Create(DbContextOptions.ReuseParentContext))
                {
                    // 2nd level, context 1
                    using (unitOfWorkProvider.Create(DbContextOptions.AlwaysCreateOwnContext))
                    {
                        // 3rd level, context 2
                        using (unitOfWorkProvider.Create(DbContextOptions.ReuseParentContext))
                        {
                            // 4th level, context 2
                            using (var unitOfWorkParent3 = unitOfWorkProvider.Create(DbContextOptions.AlwaysCreateOwnContext))
                            {
                                // 5th level, context 3
                                using (unitOfWorkProvider.Create(DbContextOptions.ReuseParentContext))
                                {
                                    // 6th level, context 3
                                    using (unitOfWorkProvider.Create(DbContextOptions.ReuseParentContext))
                                    {
                                    }
                                    using (var unitOfWorkChild3 = unitOfWorkProvider.Create(DbContextOptions.ReuseParentContext))
                                    {
                                        // 7th level, context 3 commit requested
                                        await unitOfWorkChild3.CommitAsync();
                                    }
                                }
                                // commit mandatory, context 3 commit pending
                                await unitOfWorkParent3.CommitAsync();
                            }
                        }
                    }
                }
                // commit optional, no reusing child commit pending
                await unitOfWorkParent.CommitAsync();
            }
        }

        [Fact]
        public async Task CommitAsync_UOWHasNotParrent_CallCommitCore()
        {
            Func<DbContext> dbContextFactory = () => new Mock<DbContext>().Object;

            var unitOfWorkRegistryStub = new ThreadLocalUnitOfWorkRegistry();

            var unitOfWorkProvider = new EntityFrameworkUnitOfWorkProvider(unitOfWorkRegistryStub, dbContextFactory);

            var unitOfWorkParentMock = new Mock<EntityFrameworkUnitOfWork>(unitOfWorkProvider, dbContextFactory, DbContextOptions.ReuseParentContext) { CallBase = true };
            using (var unitOfWorkParent = unitOfWorkParentMock.Object)
            {
                await unitOfWorkParent.CommitAsync();
            }
            unitOfWorkParentMock.Protected().Verify("CommitAsyncCore", Times.Once(), new CancellationToken());
        }

        [Fact]
        public async Task Commit_UOWHasChild_CallCommitCore()
        {
            Func<DbContext> dbContextFactory = () => new Mock<DbContext>().Object;

            var unitOfWorkRegistryStub = new ThreadLocalUnitOfWorkRegistry();

            var unitOfWorkProvider = new EntityFrameworkUnitOfWorkProvider(unitOfWorkRegistryStub, dbContextFactory);

            var unitOfWorkParentMock = new Mock<EntityFrameworkUnitOfWork>(unitOfWorkProvider, dbContextFactory, DbContextOptions.ReuseParentContext) { CallBase = true };
            using (var unitOfWorkParent = unitOfWorkParentMock.Object)
            {
                unitOfWorkRegistryStub.RegisterUnitOfWork(unitOfWorkParent);

                using (var unitOfWorkChild = new EntityFrameworkUnitOfWork(unitOfWorkProvider, dbContextFactory, DbContextOptions.ReuseParentContext))
                {
                    await unitOfWorkChild.CommitAsync();
                }

                await unitOfWorkParent.CommitAsync();
            }
            unitOfWorkParentMock.Protected().Verify("CommitAsyncCore", Times.Once(), new CancellationToken());
        }

        [Fact]
        public void TryGetDbContext_UnitOfWorkRegistryHasUnitOfWork_ReturnCorrectDbContext()
        {
            var dbContext = new Mock<DbContext>().Object;
            Func<DbContext> dbContextFactory = () => dbContext;
            var unitOfWorkRegistryStub = new ThreadLocalUnitOfWorkRegistry();
            var unitOfWorkProvider = new EntityFrameworkUnitOfWorkProvider(unitOfWorkRegistryStub, dbContextFactory);
            var unitOfWork = new EntityFrameworkUnitOfWork(unitOfWorkProvider, dbContextFactory, DbContextOptions.ReuseParentContext);
            unitOfWorkRegistryStub.RegisterUnitOfWork(unitOfWork);

            var uowDbContext = EntityFrameworkUnitOfWork.TryGetDbContext(unitOfWorkProvider);

            Assert.NotNull(uowDbContext);
            Assert.Same(dbContext, uowDbContext);
        }

        [Fact]
        public void TryGetDbContext_UnitOfWorkRegistryHasNotUnitOfWork_ReturnsNull()
        {
            var dbContext = new Mock<DbContext>().Object;
            Func<DbContext> dbContextFactory = () => dbContext;
            var unitOfWorkRegistryStub = new ThreadLocalUnitOfWorkRegistry();
            var unitOfWorkProvider = new EntityFrameworkUnitOfWorkProvider(unitOfWorkRegistryStub, dbContextFactory);

            var value = EntityFrameworkUnitOfWork.TryGetDbContext(unitOfWorkProvider);
            Assert.Null(value);
        }

        [Fact]
        public async Task CommitAsyncWithToken_CommitInParentScopeContextTest()
        {
            var dbContext = new Mock<DbContext>();
            dbContext.Setup(context => context.SaveChangesAsync(new CancellationToken())).Throws(new SaveChangesException());
            Func<DbContext> dbContextFactory = () => dbContext.Object;


            var unitOfWorkRegistryStub = new ThreadLocalUnitOfWorkRegistry();

            var unitOfWorkProvider = new EntityFrameworkUnitOfWorkProvider(unitOfWorkRegistryStub, dbContextFactory);

            using(var uow = unitOfWorkProvider.Create())
            {
                using (var nested = unitOfWorkProvider.Create())
                {
                    await nested.CommitAsync(new CancellationToken());
                }

                try
                {
                    await uow.CommitAsync(new CancellationToken());
                }
                catch
                {
                    // ignored
                }
            }
        }

        [Fact]
        public async Task CommitAsync_CommitInParentScopeContextTest()
        {
            var dbContext = new Mock<DbContext>();
            dbContext.Setup(context => context.SaveChangesAsync(new CancellationToken())).Throws(new SaveChangesException());
            Func<DbContext> dbContextFactory = () => dbContext.Object;


            var unitOfWorkRegistryStub = new ThreadLocalUnitOfWorkRegistry();

            var unitOfWorkProvider = new EntityFrameworkUnitOfWorkProvider(unitOfWorkRegistryStub, dbContextFactory);

            using (var uow = unitOfWorkProvider.Create())
            {
                using (var nested = unitOfWorkProvider.Create())
                {
                    await nested.CommitAsync();
                }

                try
                {
                    await uow.CommitAsync();
                }
                catch
                {
                    // ignored
                }
            }
        }


        public class SaveChangesException : Exception
        {

        }
    }
}