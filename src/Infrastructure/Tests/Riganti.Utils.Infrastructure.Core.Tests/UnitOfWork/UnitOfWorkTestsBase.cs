using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;

namespace Riganti.Utils.Infrastructure.Core.Tests.UnitOfWork
{
    public class UnitOfWorkTestsBase
    {
        
        protected static UnitOfWorkRegistryBase CreateUnitOfWorkRegistryStub()
        {
            return new UnitOfWorkRegistryStub();
        }
        protected static UnitOfWorkProviderBase CreateUnitOfWorkProviderStub(IUnitOfWorkRegistry unitOfWorkRegistry, IUnitOfWork newUnitOfWork = null)
        {
            return new UnitOfWorkProviderBaseStub(unitOfWorkRegistry, newUnitOfWork ?? new Mock<IUnitOfWork>().Object);
        }
        protected static UnitOfWorkBase CreateUnitOfWorkStub()
        {
            return new UnitOfWorkBaseStub();
        }

        private class UnitOfWorkRegistryStub : UnitOfWorkRegistryBase
        {
            public UnitOfWorkRegistryStub()
            {
                
            }

            readonly Stack<IUnitOfWork> stack = new Stack<IUnitOfWork>();

            protected override Stack<IUnitOfWork> GetStack()
            {
                return stack;
            }
        }

        private class UnitOfWorkProviderBaseStub : UnitOfWorkProviderBase
        {
            private readonly IUnitOfWork newUnitOfWork;

            public UnitOfWorkProviderBaseStub(IUnitOfWorkRegistry registry, IUnitOfWork newUnitOfWork = null) : base(registry)
            {
                this.newUnitOfWork = newUnitOfWork;
            }

            protected override IUnitOfWork CreateUnitOfWork(object parameter)
            {
                return newUnitOfWork ?? new Mock<IUnitOfWork>().Object;
            }
        }

        private class UnitOfWorkBaseStub : UnitOfWorkBase
        {
            protected override void CommitCore()
            {
            }

            protected override Task CommitAsyncCore(CancellationToken cancellationToken)
            {
                return Task.FromResult(true);
            }

            protected override void DisposeCore()
            {
            }
        }
    }
}