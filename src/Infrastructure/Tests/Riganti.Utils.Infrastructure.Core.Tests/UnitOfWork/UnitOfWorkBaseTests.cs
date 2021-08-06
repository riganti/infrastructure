using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Xunit;

namespace Riganti.Utils.Infrastructure.Core.Tests.UnitOfWork
{
    public class UnitOfWorkBaseTests : UnitOfWorkTestsBase
    {
        [Fact]
        public void Commit_CallsCommitCoreOnce()
        {
            var unitOfWorkMock = new Mock<UnitOfWorkBase> { CallBase = true };

            unitOfWorkMock.Object.Commit();

            unitOfWorkMock.Protected().Verify("CommitCore", Times.Once());
        }

        [Fact]
        public void Commit_OneRegisteredAction_CallsRegisterAfterCommitActionOnce()
        {
            var unitOfWork = CreateUnitOfWorkStub();
            var afterCommitCallCount = 0;
            unitOfWork.RegisterAfterCommitAction(() => afterCommitCallCount++);

            unitOfWork.Commit();

            Assert.Equal(1, afterCommitCallCount);
        }


        [Fact]
        public void Commit_ThreeRegisteredAction_CallsAllRegisterAfterCommitActionOnce()
        {
            var unitOfWork = CreateUnitOfWorkStub();
            var afterCommitCallCount1 = 0;
            var afterCommitCallCount2 = 0;
            var afterCommitCallCount3 = 0;
            unitOfWork.RegisterAfterCommitAction(() => afterCommitCallCount1++);
            unitOfWork.RegisterAfterCommitAction(() => afterCommitCallCount2++);
            unitOfWork.RegisterAfterCommitAction(() => afterCommitCallCount3++);

            unitOfWork.Commit();

            Assert.Equal(1, afterCommitCallCount1);
            Assert.Equal(1, afterCommitCallCount2);
            Assert.Equal(1, afterCommitCallCount3);
        }

        [Fact]
        public async Task CommitAsync_CallsCommitCoreOnce()
        {
            var unitOfWorkMock = new Mock<UnitOfWorkBase> { CallBase = true };

            await unitOfWorkMock.Object.CommitAsync();

            unitOfWorkMock.Protected().Verify("CommitAsyncCore", Times.Once(), new CancellationToken());
        }

        [Fact]
        public async Task CommitAsync_OneRegisteredAction_CallsRegisterAfterCommitActionOnce()
        {
            var unitOfWork = CreateUnitOfWorkStub();
            var afterCommitCallCount = 0;
            unitOfWork.RegisterAfterCommitAction(() => afterCommitCallCount++);

            await unitOfWork.CommitAsync();

            Assert.Equal(1, afterCommitCallCount);
        }


        [Fact]
        public async Task CommitAsync_ThreeRegisteredAction_CallsAllRegisterAfterCommitActionOnce()
        {
            var unitOfWork = CreateUnitOfWorkStub();
            var afterCommitCallCount1 = 0;
            var afterCommitCallCount2 = 0;
            var afterCommitCallCount3 = 0;
            unitOfWork.RegisterAfterCommitAction(() => afterCommitCallCount1++);
            unitOfWork.RegisterAfterCommitAction(() => afterCommitCallCount2++);
            unitOfWork.RegisterAfterCommitAction(() => afterCommitCallCount3++);

            await unitOfWork.CommitAsync();

            Assert.Equal(1, afterCommitCallCount1);
            Assert.Equal(1, afterCommitCallCount2);
            Assert.Equal(1, afterCommitCallCount3);
        }

        [Fact]
        public void Disposable_HasIDisposableInterface()
        {
            var unitOfWork = CreateUnitOfWorkStub();

            var disposable = unitOfWork as IDisposable;

            Assert.NotNull(disposable);
        }

        [Fact]
        public void Disposable_Disposing_CallsRegisterDisposing()
        {
            int disposingCallCount1;
            using (var unitOfWork = CreateUnitOfWorkStub())
            {
                disposingCallCount1 = 0;

                unitOfWork.Disposing += (sender, args) =>
                {
                    disposingCallCount1++;
                    Assert.Equal(unitOfWork, sender);
                };
            }

            Assert.Equal(1, disposingCallCount1);
        }
    }
}