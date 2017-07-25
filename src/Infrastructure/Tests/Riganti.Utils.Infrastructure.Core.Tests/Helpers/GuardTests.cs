using System;
using Xunit;

namespace Riganti.Utils.Infrastructure.Core.Tests.Helpers
{
    public class GuardTests
    {
        [Theory]
        [InlineData("test")]
        [InlineData("  test  ")]
        [InlineData("_")]
        public void Guard_ArgumentNotNullOrEmpty_WithValidArgument(string input)
        {
            Guard.ArgumentNotNull(input, nameof(input));
            Guard.ArgumentNotNullOrWhiteSpace(input, nameof(input));
        }

        [Theory]
        [InlineData(null)]
        public void Guard_ArgumentNotNull(string input)
        {
            Assert.Throws<ArgumentNullException>(() => Guard.ArgumentNotNull(input, nameof(input)));
        }

        [Theory]
        [InlineData(null)]
        [InlineData(" ")]
        public void Guard_ArgumentNotNullOrWhiteSpace(string input)
        {
            Assert.Throws<ArgumentException>(() => Guard.ArgumentNotNullOrWhiteSpace(input, nameof(input)));
        }

        [Theory]
        [InlineData("QmFydCBTaW1wc29u")]
        [InlineData("QmFydCBTaW1wc29uIGlzIGEgbmF1Z2h0eSBndXk=")]
        public void Guard_IsBase64(string input)
        {
            Guard.ArgumentIsBase64String(input, nameof(input));
        }

        [Theory]
        [InlineData("Bart Simpson")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Guard_NotBase64_ThrowsArgumentException(string input)
        {
            Assert.Throws<ArgumentException>(() => Guard.ArgumentIsBase64String(input, nameof(input)));
        }
    }
}
