using System;
using System.Globalization;
using Xunit;

namespace Riganti.Utils.Infrastructure.Core.Tests
{
    public class RigantiHelpersTests
    {
        [Fact]
        public void ToNullableInt_CorrectInput_ReturnsCorrectValue()
        {
            int expectedValue = int.MaxValue;
            var expectedValueString = expectedValue.ToString();

            var convertedValue = expectedValueString.ToNullableInt();

            Assert.True(convertedValue.HasValue);
            Assert.Equal(expectedValue, convertedValue.Value);
        }

        [Fact]
        public void ToNullableInt_NonsenseInput_ReturnsNull()
        {
            var convertedValue = "as I was informed, that should return null".ToNullableInt();

            Assert.False(convertedValue.HasValue);
        }

        [Fact]
        public void ToNullableLong_CorrectInput_ReturnsCorrectValue()
        {
            long expectedValue = long.MaxValue;
            var expectedValueString = expectedValue.ToString();

            var convertedValue = expectedValueString.ToNullableLong();

            Assert.True(convertedValue.HasValue);
            Assert.Equal(expectedValue, convertedValue.Value);
        }

        [Fact]
        public void ToNullableLong_NonsenseInput_ReturnsNull()
        {
            var convertedValue = "as I was informed, that should return null".ToNullableLong();

            Assert.False(convertedValue.HasValue);
        }

        [Fact]
        public void ToNullableFloat_CorrectInput_ReturnsCorrectValue()
        {
            float expectedValue = float.MaxValue;
            var expectedValueString = expectedValue.ToString("r");

            var convertedValue = expectedValueString.ToNullableFloat();

            Assert.True(convertedValue.HasValue);
            Assert.Equal(expectedValue, convertedValue.Value);
        }

        [Fact]
        public void ToNullableFloat_NonsenseInput_ReturnsNull()
        {
            var convertedValue = "as I was informed, that should return null".ToNullableFloat();

            Assert.False(convertedValue.HasValue);
        }

        [Fact]
        public void ToNullableDouble_CorrectInput_ReturnsCorrectValue()
        {
            double expectedValue = double.MaxValue;
            string expectedValueString = expectedValue.ToString("r");

            double number;
            double? convertedValue = double.TryParse(expectedValueString, out number) ? number : (double?) null;

            Assert.True(convertedValue.HasValue);
        }

        [Fact]
        public void ToNullableDouble_NonsenseInput_ReturnsNull()
        {
            var convertedValue = "as I was informed, that should return null".ToNullableDouble();

            Assert.False(convertedValue.HasValue);
        }

        [Fact]
        public void ToNullableDecimal_CorrectInput_ReturnsCorrectValue()
        {
            decimal expectedValue = decimal.MaxValue;
            var expectedValueString = expectedValue.ToString();

            var convertedValue = expectedValueString.ToNullableDecimal();

            Assert.True(convertedValue.HasValue);
            Assert.Equal(expectedValue, convertedValue.Value);
        }

        [Fact]
        public void ToNullableDecimal_NonsenseInput_ReturnsNull()
        {
            var convertedValue = "as I was informed, that should return null".ToNullableDecimal();

            Assert.False(convertedValue.HasValue);
        }

        [Fact]
        public void ToNullableDateTime_CorrectInput_ReturnsCorrectValue()
        {
            var expectedDateTime = new DateTime(2015, 11, 22);
            var expectedDateTimeString = expectedDateTime.ToLongDateString();

            var convertedDateTime = expectedDateTimeString.ToNullableDateTime();

            Assert.True(convertedDateTime.HasValue);
            Assert.Equal(expectedDateTime, convertedDateTime.Value);
        }

        [Fact]
        public void ToNullableDateTime_NonsenseInput_ReturnsNull()
        {
            var convertedDateTime = "as I was informed, that should return null".ToNullableDateTime();

            Assert.False(convertedDateTime.HasValue);
        }

        [Fact]
        public void ToNullableDateTime_CorrectInputStringFormat_ReturnsCorrectValue()
        {
            var expectedDateTime = new DateTime(2015, 11, 22, 22, 30, 00);
            var dateTimeStringformat = "dd.MM yyyy HH:mm";
            var expectedDateTimeString = expectedDateTime.ToString(dateTimeStringformat);

            var convertedDateTime = expectedDateTimeString.ToNullableDateTime(dateTimeStringformat,
                CultureInfo.InvariantCulture);

            Assert.True(convertedDateTime.HasValue);
            Assert.Equal(expectedDateTime, convertedDateTime.Value);
        }

        [Fact]
        public void ToNullableDateTime_NonsenseInputStringFormat_ReturnsNull()
        {
            var dateTimeStringformat = "dd.MM yyyy HH:mm";

            var convertedDateTime = "as I was informed, that should return null".ToNullableDateTime(
                dateTimeStringformat, CultureInfo.InvariantCulture);

            Assert.False(convertedDateTime.HasValue);
        }

        [Fact]
        public void ToNullableDateTime_CorrectInputStringFormats_ReturnsCorrectValue()
        {
            var expectedDateTime = new DateTime(2015, 11, 22, 22, 30, 00);
            var dateTimeStringformat = "dd.MM yyyy HH:mm";
            string[] dateTimeStringformats = {"MM/dd/yyyy hh:mm", "M/dd/yyyy hh:mm", dateTimeStringformat};
            var expectedDateTimeString = expectedDateTime.ToString(dateTimeStringformat);

            var convertedDateTime = expectedDateTimeString.ToNullableDateTime(dateTimeStringformats,
                CultureInfo.InvariantCulture);

            Assert.True(convertedDateTime.HasValue);
            Assert.Equal(expectedDateTime, convertedDateTime.Value);
        }

        [Fact]
        public void ToNullableDateTime_NonsenseInputStringFormats_ReturnsNull()
        {
            string[] dateTimeStringformats = {"MM/dd/yyyy hh:mm", "M/dd/yyyy hh:mm", "dd.MM yyyy HH:mm"};

            var convertedDateTime =
                "as I was informed, that should return null".ToNullableDateTime(dateTimeStringformats,
                    CultureInfo.InvariantCulture);

            Assert.False(convertedDateTime.HasValue);
        }
    }
}