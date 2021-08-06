using System;
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

        protected static UnitOfWorkProviderBase CreateUnitOfWorkProviderStub(IUnitOfWork newUnitOfWork, IUnitOfWorkRegistry unitOfWorkRegistry = null)
        {
            return CreateUnitOfWorkProviderStub(() => newUnitOfWork, unitOfWorkRegistry);
        }

        protected static UnitOfWorkProviderBase CreateUnitOfWorkProviderStub(Func<IUnitOfWork> newUnitOfWorkFactory = null, IUnitOfWorkRegistry unitOfWorkRegistry = null)
        {
            unitOfWorkRegistry = unitOfWorkRegistry ?? CreateUnitOfWorkRegistryStub();
            newUnitOfWorkFactory = newUnitOfWorkFactory ?? (() => new Mock<IUnitOfWork>().Object);
            return new UnitOfWorkProviderBaseStub(unitOfWorkRegistry, newUnitOfWorkFactory);
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
            private readonly Func<IUnitOfWork> newUnitOfWorkFactory;

            public UnitOfWorkProviderBaseStub(IUnitOfWorkRegistry registry, Func<IUnitOfWork> newUnitOfWorkFactory) : base(registry)
            {
                this.newUnitOfWorkFactory = newUnitOfWorkFactory;
            }

            protected override IUnitOfWork CreateUnitOfWork(object parameter)
            {
                return newUnitOfWorkFactory();
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