using System;
using Moq;
using Xunit;

namespace Riganti.Utils.Infrastructure.Core.Tests.UnitOfWork.Registry
{
    public class UnitOfWorkRegistryBaseTests : UnitOfWorkTestsBase
    {
        [Fact]
        public void GetCurrent_EmptyStack_ReturnsNull()
        {
            var unitOfWorkRegistrySUT = CreateUnitOfWorkRegistry();

            var unitOfWork = unitOfWorkRegistrySUT.GetCurrent();

            Assert.Null(unitOfWork);
        }

        [Fact]
        public void GetCurrent_StackWithOneUOW_ReturnsUOW()
        {
            var unitOfWorkRegistrySUT = CreateUnitOfWorkRegistry();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkRegistrySUT.RegisterUnitOfWork(unitOfWorkMock.Object);
            var unitOfWork = unitOfWorkRegistrySUT.GetCurrent();

            Assert.NotNull(unitOfWork);
            Assert.Same(unitOfWorkMock.Object, unitOfWork);
        }

        [Fact]
        public void UnregisterUnitOfWork_EmptyStack_ThrowsInvalidOperationException()
        {
            var unitOfWorkRegistrySUT = CreateUnitOfWorkRegistry();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            Action sut = () => unitOfWorkRegistrySUT.UnregisterUnitOfWork(unitOfWorkMock.Object);

            var invalidOperationException = Assert.Throws<InvalidOperationException>(sut);
            Assert.Equal("Some of the unit of works was not disposed correctly!", invalidOperationException.Message);
        }

        [Fact]
        public void UnregisterUnitOfWork_UnregisterFirstUOWFromStack_ThrowsInvalidOperationException()
        {
            var unitOfWorkRegistrySUT = CreateUnitOfWorkRegistry();
            var firstUnitOfWorkMock = new Mock<IUnitOfWork>();
            var secondUnitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkRegistrySUT.RegisterUnitOfWork(firstUnitOfWorkMock.Object);
            unitOfWorkRegistrySUT.RegisterUnitOfWork(secondUnitOfWorkMock.Object);

            Action sut = () => unitOfWorkRegistrySUT.UnregisterUnitOfWork(firstUnitOfWorkMock.Object);

            var invalidOperationException = Assert.Throws<InvalidOperationException>(sut);
            Assert.Equal("Some of the unit of works was not disposed correctly!", invalidOperationException.Message);
            Assert.Null(invalidOperationException.InnerException);
        }

        [Fact]
        public void UnregisterUnitOfWork_UnregisterLastUOWFromStack_ThrowsInvalidOperationException()
        {
            var unitOfWorkRegistrySUT = CreateUnitOfWorkRegistry();
            var firstUnitOfWorkMock = new Mock<IUnitOfWork>();
            var secondUnitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkRegistrySUT.RegisterUnitOfWork(firstUnitOfWorkMock.Object);
            unitOfWorkRegistrySUT.RegisterUnitOfWork(secondUnitOfWorkMock.Object);

            unitOfWorkRegistrySUT.UnregisterUnitOfWork(secondUnitOfWorkMock.Object);

            Assert.Same(firstUnitOfWorkMock.Object, unitOfWorkRegistrySUT.GetCurrent());
        }
    }
}