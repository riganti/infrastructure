using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;

namespace Riganti.Utils.Infrastructure.Core.Tests
{
    public class UnitOfWorkTestsBase
    {
        
        protected static UnitOfWorkRegistryStub CreateUnitOfWorkRegistry()
        {
            return new UnitOfWorkRegistryStub();
        }

        protected class UnitOfWorkRegistryStub : UnitOfWorkRegistryBase
        {
            readonly Stack<IUnitOfWork> stack = new Stack<IUnitOfWork>();

            protected override Stack<IUnitOfWork> GetStack()
            {
                return stack;
            }
        }

        protected class UnitOfWorkProviderBaseStub : UnitOfWorkProviderBase
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

        protected class UnitOfWorkBaseStub : UnitOfWorkBase
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