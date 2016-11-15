using System;
using Xunit;

namespace Riganti.Utils.Infrastructure.Core.Tests.Helpers
{
    public class RigantiCastHelpersTests
    {
        [Fact]
        public void AsCast_CorrectInput_ReturnsCastedObject()
        {
            var derivedInstance = new DerivedTestType();

            var baseInstance = derivedInstance.CastAs<TestTypeBase>();

            Assert.NotNull(baseInstance);
            Assert.True(baseInstance is TestTypeBase);
        }

        [Fact]
        public void AsCast_NonsenseInput_ReturnsNull()
        {
            var baseInstance = new TestTypeBase();

            var derivedInstance = baseInstance.CastAs<DerivedTestType>();

            Assert.Null(derivedInstance);
        }

        [Fact]
        public void CastTo_CorrectInput_ReturnsCastedObject()
        {
            var derivedInstance = new DerivedTestType();

            var baseInstance = derivedInstance.CastTo<TestTypeBase>();

            Assert.NotNull(baseInstance);
            Assert.True(baseInstance is TestTypeBase);
        }

        [Fact]
        public void CastTo_NonsenseInput_ReturnsNull()
        {
            var baseInstance = new TestTypeBase();

            Action sut = () => baseInstance.CastTo<DerivedTestType>();

            Assert.Throws<InvalidCastException>(sut);
        }
        private class TestTypeBase
        {
        }

        private class DerivedTestType : TestTypeBase
        {
        }
    }
}