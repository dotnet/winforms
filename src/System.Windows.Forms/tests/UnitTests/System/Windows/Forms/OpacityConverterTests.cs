// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class OpacityConverterTests : IClassFixture<ThreadExceptionFixture>
    {
        public static TheoryData<Type, bool> CanConvertFromData =>
            CommonTestHelper.GetConvertFromTheoryData();

        [Theory]
        [MemberData(nameof(CanConvertFromData))]
        [InlineData(typeof(string), true)]
        public void OpacityConverter_CanConvertFrom_Invoke_ReturnsExpected(Type sourceType, bool expected)
        {
            var converter = new OpacityConverter();
            Assert.Equal(expected, converter.CanConvertFrom(sourceType));
        }

        public static IEnumerable<object[]> ConvertFrom_TestData()
        {
            yield return new object[] { "0%", 0.0 };
            yield return new object[] { "100%", 1.0 };
            yield return new object[] { "90%", 0.9 };
            yield return new object[] { "  90  %  ", 0.9 };
            yield return new object[] { "0.0", 0.0 };
            yield return new object[] { "0.9", 0.9 };
            yield return new object[] { "1.0", 1.0 };
            yield return new object[] { "1.2%", 0.012 };
            yield return new object[] { "1.1", 0.011 };
        }

        [Theory]
        [MemberData(nameof(ConvertFrom_TestData))]
        public void OpacityConverter_ConvertFrom_String_ReturnsExpected(string value, double expected)
        {
            var converter = new OpacityConverter();
            Assert.Equal(expected, Assert.IsType<double>(converter.ConvertFrom(value)), 5);
            Assert.Equal(expected, Assert.IsType<double>(converter.ConvertFrom(null, null, value)), 5);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(null)]
        public void OpacityConverter_ConvertFrom_InvalidValue_ThrowsNotSupportedException(object value)
        {
            var converter = new OpacityConverter();
            Assert.Throws<NotSupportedException>(() => converter.ConvertFrom(value));
        }

        [Theory]
        [InlineData("")]
        [InlineData("1%.2")]
        [InlineData("-1")]
        [InlineData("101")]
        [InlineData("-1%")]
        [InlineData("101%")]
        [InlineData("-0.1")]
        [InlineData("-0.1%")]
        [InlineData("invalid")]
        public void OpacityConverter_ConvertFrom_InvalidString_ThrowsFormatException(string value)
        {
            var converter = new OpacityConverter();
            Assert.Throws<FormatException>(() => converter.ConvertFrom(value));
        }

        [Theory]
        [InlineData(typeof(string), true)]
        [InlineData(typeof(InstanceDescriptor), false)]
        [InlineData(typeof(double), false)]
        [InlineData(typeof(int), false)]
        [InlineData(null, false)]
        public void OpacityConverter_CanConvertTo_Invoke_ReturnsExpected(Type destinationType, bool expected)
        {
            var converter = new OpacityConverter();
            Assert.Equal(expected, converter.CanConvertTo(destinationType));
        }

        [Theory]
        [InlineData(0.0, "0%")]
        [InlineData(0, "0")]
        public void OpacityConverter_ConvertTo_String_ReturnsExpected(object value, string expected)
        {
            var converter = new OpacityConverter();
            Assert.Equal(expected, converter.ConvertTo(value, typeof(string)));
            Assert.Equal(expected, converter.ConvertTo(null, null, value, typeof(string)));
        }

        [Fact]
        public void OpacityConverter_ConvertTo_NullDestinationType_ThrowsArgumentNullException()
        {
            var converter = new OpacityConverter();
            Assert.Throws<ArgumentNullException>("destinationType", () => converter.ConvertTo(new object(), null));
        }

        [Fact]
        public void OpacityConverter_ConvertTo_ValueNotDouble_ThrowsNotSupportedException()
        {
            var converter = new OpacityConverter();
            Assert.Throws<NotSupportedException>(() => converter.ConvertTo(1, typeof(InstanceDescriptor)));
        }

        [Theory]
        [InlineData(typeof(double))]
        [InlineData(typeof(InstanceDescriptor))]
        [InlineData(typeof(int))]
        public void OpacityConverter_ConvertTo_InvalidDestinationType_ThrowsNotSupportedException(Type destinationType)
        {
            var converter = new OpacityConverter();
            Assert.Throws<NotSupportedException>(() => converter.ConvertTo(1.1, destinationType));
        }

        [Fact]
        public void OpacityConverter_ConverterGetCreateInstanceSupported_Invoke_ReturnsFalse()
        {
            var converter = new OpacityConverter();
            Assert.False(converter.GetCreateInstanceSupported());
        }

        [Fact]
        public void OpacityConverter_ConverterGetPropertiesSupported_Invoke_ReturnsFalse()
        {
            var converter = new OpacityConverter();
            Assert.False(converter.GetPropertiesSupported());
        }
    }
}
