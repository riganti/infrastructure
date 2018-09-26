using Moq;
using Xunit;

namespace Riganti.Utils.Infrastructure.Core.Tests.UnitOfWork.Provider
{
    public class UnitOfWorkProviderBaseTests : UnitOfWorkTestsBase
    {
        [Fact]
        public void GetCurrent_ReturnsRegistryGetCurrent()
        {
            var unitOfWorkRegistryMock = new Mock<IUnitOfWorkRegistry>();
            var unitOfWork = new Mock<IUnitOfWork>().Object;
            unitOfWorkRegistryMock.Setup(unitOfWorkRegistry => unitOfWorkRegistry.GetCurrent(0)).Returns(unitOfWork);
            var unitOfWorkProviderSUT = CreateUnitOfWorkProviderStub(unitOfWorkRegistry: unitOfWorkRegistryMock.Object);

            var unitOfWorkProviderCurrentUnitOfWork = unitOfWorkProviderSUT.GetCurrent();

            Assert.Same(unitOfWork, unitOfWorkProviderCurrentUnitOfWork);
            unitOfWorkRegistryMock.Verify(unitOfWorkRegistry => unitOfWorkRegistry.GetCurrent(0), Times.Once);
        }

        [Fact]
        public void Create_CallsUnitOfWorkRegistryRegisterUnitOfWork()
        {
            var unitOfWorkRegistryMock = new Mock<IUnitOfWorkRegistry>();
            var newUnitOfWork = new Mock<IUnitOfWork>().Object;
            var unitOfWorkProviderSUT = CreateUnitOfWorkProviderStub(newUnitOfWork, unitOfWorkRegistryMock.Object);

            Assert.Same(newUnitOfWork, unitOfWorkProviderSUT.Create());
            unitOfWorkRegistryMock.Verify(unitOfWorkRegistry => unitOfWorkRegistry.RegisterUnitOfWork(newUnitOfWork), Times.Once);
        }

        [Fact]
        public void Create_CallsUnitOfWorkRegistryUnregisterUnitOfWorkWhenIsDisposed()
        {
            var unitOfWorkRegistryMock = new Mock<IUnitOfWorkRegistry>();
            var newUnitOfWork = CreateUnitOfWorkStub();
            var unitOfWorkProviderSUT = CreateUnitOfWorkProviderStub(newUnitOfWork, unitOfWorkRegistryMock.Object);

            var unitOfWork = unitOfWorkProviderSUT.Create();

            Assert.Same(newUnitOfWork, unitOfWork);
            unitOfWorkRegistryMock.Verify(unitOfWorkRegistry => unitOfWorkRegistry.UnregisterUnitOfWork(newUnitOfWork), Times.Never);

            unitOfWork.Dispose();

            unitOfWorkRegistryMock.Verify(unitOfWorkRegistry => unitOfWorkRegistry.UnregisterUnitOfWork(newUnitOfWork), Times.Once);
        }
    }
}