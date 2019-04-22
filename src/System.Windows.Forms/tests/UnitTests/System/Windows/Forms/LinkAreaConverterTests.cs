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
    public class LinkAreaConverterTests
    {
        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetConvertFromTheoryData))]
        [InlineData(typeof(LinkArea), false)]
        [InlineData(typeof(string), true)]
        public void LinkAreaConverter_CanConvertFrom_Invoke_ReturnsExpected(Type sourceType, bool expected)
        {
            var converter = new LinkArea.LinkAreaConverter();
            Assert.Equal(expected, converter.CanConvertFrom(sourceType));
        }

        public static IEnumerable<object[]> ConvertFrom_TestData()
        {
            yield return new object[] { "1,2", new LinkArea(1, 2) };
            yield return new object[] { "  1 , 1", new LinkArea(1, 1) };
            yield return new object[] { "   ", null };
            yield return new object[] { string.Empty, null };
        }

        [Theory]
        [MemberData(nameof(ConvertFrom_TestData))]
        public void LinkAreaConverter_ConvertFrom_String_ReturnsExpected(string value, object expected)
        {
            var converter = new LinkArea.LinkAreaConverter();
            Assert.Equal(expected, converter.ConvertFrom(value));
            Assert.Equal(expected, converter.ConvertFrom(null, null, value));
            Assert.Equal(expected, converter.ConvertFrom(null, CultureInfo.InvariantCulture, value));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(null)]
        public void LinkAreaConverter_ConvertFrom_InvalidValue_ThrowsNotSupportedException(object value)
        {
            var converter = new LinkArea.LinkAreaConverter();
            Assert.Throws<NotSupportedException>(() => converter.ConvertFrom(value));
        }

        [Theory]
        [InlineData("1")]
        [InlineData("1,2,3")]
        public void LinkAreaConverter_ConvertFrom_InvalidString_ThrowsArgumentException(string value)
        {
            var converter = new LinkArea.LinkAreaConverter();
            Assert.Throws<ArgumentException>(null, () => converter.ConvertFrom(value));
        }

        [Theory]
        [InlineData(typeof(string), true)]
        [InlineData(typeof(InstanceDescriptor), true)]
        [InlineData(typeof(LinkArea), false)]
        [InlineData(typeof(int), false)]
        [InlineData(null, false)]
        public void LinkAreaConverter_CanConvertTo_Invoke_ReturnsExpected(Type destinationType, bool expected)
        {
            var converter = new LinkArea.LinkAreaConverter();
            Assert.Equal(expected, converter.CanConvertTo(destinationType));
        }

        [Fact]
        public void LinkAreaConverter_ConvertTo_String_ReturnsExpected()
        {
            var converter = new LinkArea.LinkAreaConverter();
            Assert.Equal("1, 2", converter.ConvertTo(new LinkArea(1, 2), typeof(string)));
            Assert.Equal("1, 2", converter.ConvertTo(null, null, new LinkArea(1, 2), typeof(string)));
            Assert.Equal("1, 2", converter.ConvertTo(null, CultureInfo.InvariantCulture, new LinkArea(1, 2), typeof(string)));
        }

        [Fact]
        public void LinkAreaConverter_ConvertTo_InstanceDescriptor_ReturnsExpected()
        {
            var converter = new LinkArea.LinkAreaConverter();
            InstanceDescriptor descriptor = Assert.IsType<InstanceDescriptor>(converter.ConvertTo(new LinkArea(1, 2), typeof(InstanceDescriptor)));
            Assert.Equal(typeof(LinkArea).GetConstructor(new Type[] { typeof(int), typeof(int) }), descriptor.MemberInfo);
            Assert.Equal(new object[] { 1, 2 }, descriptor.Arguments);
        }

        [Fact]
        public void LinkAreaConverter_ConvertTo_NullDestinationType_ThrowsArgumentNullException()
        {
            var converter = new LinkArea.LinkAreaConverter();
            Assert.Throws<ArgumentNullException>("destinationType", () => converter.ConvertTo(new object(), null));
        }

        [Fact]
        public void LinkAreaConverter_ConvertTo_ValueNotLinkArea_ThrowsNotSupportedException()
        {
            var converter = new LinkArea.LinkAreaConverter();
            Assert.Throws<NotSupportedException>(() => converter.ConvertTo(1, typeof(InstanceDescriptor)));
        }

        [Theory]
        [InlineData(typeof(LinkArea))]
        [InlineData(typeof(int))]
        public void LinkAreaConverter_ConvertTo_InvalidDestinationType_ThrowsNotSupportedException(Type destinationType)
        {
            var converter = new LinkArea.LinkAreaConverter();
            Assert.Throws<NotSupportedException>(() => converter.ConvertTo(new LinkArea(), destinationType));
        }

        [Fact]
        public void LinkAreaConverter_CreateInstance_ValidPropertyValues_ReturnsExpected()
        {
            var converter = new LinkArea.LinkAreaConverter();
            LinkArea area = Assert.IsType<LinkArea>(converter.CreateInstance(
                null, new Dictionary<string, object>
                {
                    {nameof(LinkArea.Start), 1},
                    {nameof(LinkArea.Length), 2}
                })
            );
            Assert.Equal(new LinkArea(1, 2), area);
        }

        [Fact]
        public void LinkAreaConverter_CreateInstance_NullPropertyValues_ThrowsArgumentNullException()
        {
            var converter = new LinkArea.LinkAreaConverter();
            Assert.Throws<ArgumentNullException>("propertyValues", () => converter.CreateInstance(null, null));
        }

        public static IEnumerable<object[]> CreateInstance_InvalidPropertyValueType_TestData()
        {
            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    {nameof(LinkArea.Start), new object()},
                    {nameof(LinkArea.Length), 2},
                }
            };
            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    {nameof(LinkArea.Start), null},
                    {nameof(LinkArea.Length), 2},
                }
            };
            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    {nameof(LinkArea.Length), 2}
                }
            };

            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    {nameof(LinkArea.Start), 1},
                    {nameof(LinkArea.Length), new object()}
                }
            };
            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    {nameof(LinkArea.Start), 1},
                    {nameof(LinkArea.Length), null}
                }
            };
            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    {nameof(LinkArea.Start), 1}
                }
            };
        }

        [Theory]
        [MemberData(nameof(CreateInstance_InvalidPropertyValueType_TestData))]
        public void LinkAreaConverter_CreateInstance_InvalidPropertyValueType_ThrowsArgumentException(IDictionary propertyValues)
        {
            var converter = new LinkArea.LinkAreaConverter();
            Assert.Throws<ArgumentException>("propertyValues", () => converter.CreateInstance(null, propertyValues));
        }

        [Fact]
        public void LinkAreaConverter_GetCreateInstanceSupported_Invoke_ReturnsTrue()
        {
            var converter = new LinkArea.LinkAreaConverter();
            Assert.True(converter.GetCreateInstanceSupported());
        }

        [Fact]
        public void LinkAreaConverter_GetProperties_Invoke_ReturnsExpected()
        {
            var converter = new LinkArea.LinkAreaConverter();
            PropertyDescriptorCollection properties = converter.GetProperties(null);
            Assert.Equal(2, properties.Count);
            Assert.Equal(nameof(LinkArea.Start), properties[0].Name);
            Assert.Equal(nameof(LinkArea.Length), properties[1].Name);
        }

        [Fact]
        public void LinkAreaConverter_GetPropertiesSupported_Invoke_ReturnsTrue()
        {
            var converter = new LinkArea.LinkAreaConverter();
            Assert.True(converter.GetPropertiesSupported());
        }

        private class ClassWithLinkArea
        {
            public LinkArea LinkArea { get; set; }
        }
    }
}
