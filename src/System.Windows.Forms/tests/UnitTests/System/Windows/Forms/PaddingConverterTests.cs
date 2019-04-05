﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class PaddingConverterTests
    {

        public static TheoryData<Type,bool> CanConvertFromData =>
            CommonTestHelper.GetConvertFromTheoryData();

        [Theory]
        [MemberData(nameof(CanConvertFromData))]
        [InlineData(typeof(Padding), false)]
        [InlineData(typeof(string), true)]
        public void PaddingConverter_CanConvertFrom_Invoke_ReturnsExpected(Type sourceType, bool expected)
        {
            var converter = new PaddingConverter();
            Assert.Equal(expected, converter.CanConvertFrom(sourceType));
        }

        public static IEnumerable<object[]> ConvertFrom_TestData()
        {
            yield return new object[] { "1,2,3,4", new Padding(1, 2, 3, 4) };
            yield return new object[] { "  1 , 1,  1, 1 ", new Padding(1) };
            yield return new object[] { "   ", null };
            yield return new object[] { string.Empty, null };
        }

        [Theory]
        [MemberData(nameof(ConvertFrom_TestData))]
        public void PaddingConverter_ConvertFrom_String_ReturnsExpected(string value, object expected)
        {
            var converter = new PaddingConverter();
            Assert.Equal(expected, converter.ConvertFrom(value));
            Assert.Equal(expected, converter.ConvertFrom(null, null, value));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(null)]
        public void PaddingConverter_ConvertFrom_InvalidValue_ThrowsNotSupportedException(object value)
        {
            var converter = new PaddingConverter();
            Assert.Throws<NotSupportedException>(() => converter.ConvertFrom(value));
        }

        [Theory]
        [InlineData("1,2,3")]
        [InlineData("1,2,3,4,5")]
        public void PaddingConverter_ConvertFrom_InvalidString_ThrowsArgumentException(string value)
        {
            var converter = new PaddingConverter();
            Assert.Throws<ArgumentException>(null, () => converter.ConvertFrom(value));
        }

        [Theory]
        [InlineData(typeof(string), true)]
        [InlineData(typeof(InstanceDescriptor), true)]
        [InlineData(typeof(TableLayoutPanelCellPosition), false)]
        [InlineData(typeof(int), false)]
        [InlineData(null, false)]
        public void PaddingConverter_CanConvertTo_Invoke_ReturnsExpected(Type destinationType, bool expected)
        {
            var converter = new PaddingConverter();
            Assert.Equal(expected, converter.CanConvertTo(destinationType));
        }

        [Fact]
        public void PaddingConverter_ConvertTo_String_ReturnsExpected()
        {
            var converter = new PaddingConverter();
            Assert.Equal("1, 2, 3, 4", converter.ConvertTo(new Padding(1, 2, 3, 4), typeof(string)));
            Assert.Equal("1, 2, 3, 4", converter.ConvertTo(null, null, new Padding(1, 2, 3, 4), typeof(string)));
        }

        [Fact]
        public void PaddingConverter_ConvertTo_InstanceDescriptor_ReturnsExpected()
        {
            var converter = new PaddingConverter();
            InstanceDescriptor descriptor = Assert.IsType<InstanceDescriptor>(converter.ConvertTo(new Padding(1, 2, 3, 4), typeof(InstanceDescriptor)));
            Assert.Equal(typeof(Padding).GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) }), descriptor.MemberInfo);
            Assert.Equal(new object[] { 1, 2, 3, 4 }, descriptor.Arguments);
        }

        [Fact]
        public void PaddingConverter_ConvertTo_InstanceDescriptorAll_ReturnsExpected()
        {
            var converter = new PaddingConverter();
            InstanceDescriptor descriptor = Assert.IsType<InstanceDescriptor>(converter.ConvertTo(new Padding(1, 1, 1, 1), typeof(InstanceDescriptor)));
            Assert.Equal(typeof(Padding).GetConstructor(new Type[] { typeof(int) }), descriptor.MemberInfo);
            Assert.Equal(new object[] { 1 }, descriptor.Arguments);
        }

        [Fact]
        public void PaddingConverter_ConvertTo_NullDestinationType_ThrowsArgumentNullException()
        {
            var converter = new PaddingConverter();
            Assert.Throws<ArgumentNullException>("destinationType", () => converter.ConvertTo(new object(), null));
        }

        [Fact]
        public void PaddingConverter_ConvertTo_ValueNotPadding_ThrowsNotSupportedException()
        {
            var converter = new PaddingConverter();
            Assert.Throws<NotSupportedException>(() => converter.ConvertTo(1, typeof(InstanceDescriptor)));
        }

        [Theory]
        [InlineData(typeof(Padding))]
        [InlineData(typeof(int))]
        public void PaddingConverter_ConvertTo_InvalidDestinationType_ThrowsNotSupportedException(Type destinationType)
        {
            var converter = new PaddingConverter();
            Assert.Throws<NotSupportedException>(() => converter.ConvertTo(new Padding(), destinationType));
        }

        public static IEnumerable<object[]> CreateInstance_TestData()
        {
            yield return new object[] { new Padding(1), 1, new Padding(1, 2, 3, 4) };
            yield return new object[] { new Padding(1), 2, new Padding(2) };
            yield return new object[] { new Padding(1, 2, 3, 4), -1, new Padding(1, 2, 3, 4) };
            yield return new object[] { new Padding(1, 2, 3, 4), 1, new Padding(1) };
        }

        [Theory]
        [MemberData(nameof(CreateInstance_TestData))]
        public void PaddingConverter_CreateInstance_ValidPropertyValuesAll_ReturnsExpected(Padding instance, int all, Padding result)
        {
            var converter = new PaddingConverter();
            var mockContext = new Mock<ITypeDescriptorContext>(MockBehavior.Strict);
            mockContext
                .Setup(c => c.Instance)
                .Returns(new ClassWithPadding { Padding = instance });
            mockContext
                .Setup(c => c.PropertyDescriptor)
                .Returns(TypeDescriptor.GetProperties(typeof(ClassWithPadding))[0]);
            Padding padding = Assert.IsType<Padding>(converter.CreateInstance(
                mockContext.Object, new Dictionary<string, object>
                {
                    {nameof(Padding.All), all},
                    {nameof(Padding.Left), 1},
                    {nameof(Padding.Top), 2},
                    {nameof(Padding.Right), 3},
                    {nameof(Padding.Bottom), 4}
                })
            );
            Assert.Equal(result, padding);
        }

        [Fact]
        public void PaddingConverter_CreateInstance_NullContext_ThrowsArgumentNullException()
        {
            var converter = new PaddingConverter();
            Assert.Throws<ArgumentNullException>("context", () => converter.CreateInstance(null, new Dictionary<string, object>()));
        }

        [Fact]
        public void PaddingConverter_CreateInstance_NullPropertyValues_ThrowsArgumentNullException()
        {
            var converter = new PaddingConverter();
            Assert.Throws<ArgumentNullException>("propertyValues", () => converter.CreateInstance(new Mock<ITypeDescriptorContext>(MockBehavior.Strict).Object, null));
        }

        public static IEnumerable<object[]> CreateInstance_InvalidPropertyValueType_TestData()
        {
            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    {nameof(Padding.All), new object()},
                    {nameof(Padding.Left), 2},
                    {nameof(Padding.Top), 2},
                    {nameof(Padding.Right), 3},
                    {nameof(Padding.Bottom), 4}
                }
            };
            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    {nameof(Padding.All), null},
                    {nameof(Padding.Left), 2},
                    {nameof(Padding.Top), 2},
                    {nameof(Padding.Right), 3},
                    {nameof(Padding.Bottom), 4}
                }
            };
            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    {nameof(Padding.Left), 2},
                    {nameof(Padding.Top), 2},
                    {nameof(Padding.Right), 3},
                    {nameof(Padding.Bottom), 4}
                }
            };

            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    {nameof(Padding.All), 1},
                    {nameof(Padding.Left), new object()},
                    {nameof(Padding.Top), 2},
                    {nameof(Padding.Right), 3},
                    {nameof(Padding.Bottom), 4}
                }
            };
            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    {nameof(Padding.All), 1},
                    {nameof(Padding.Left), null},
                    {nameof(Padding.Top), 2},
                    {nameof(Padding.Right), 3},
                    {nameof(Padding.Bottom), 4}
                }
            };
            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    {nameof(Padding.All), 1},
                    {nameof(Padding.Top), 2},
                    {nameof(Padding.Right), 3},
                    {nameof(Padding.Bottom), 4}
                }
            };

            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    {nameof(Padding.All), 1},
                    {nameof(Padding.Left), 2},
                    {nameof(Padding.Top), new object()},
                    {nameof(Padding.Right), 3},
                    {nameof(Padding.Bottom), 4}
                }
            };
            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    {nameof(Padding.All), 1},
                    {nameof(Padding.Left), 2},
                    {nameof(Padding.Top), null},
                    {nameof(Padding.Right), 3},
                    {nameof(Padding.Bottom), 4}
                }
            };
            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    {nameof(Padding.All), 1},
                    {nameof(Padding.Left), 2},
                    {nameof(Padding.Right), 3},
                    {nameof(Padding.Bottom), 4}
                }
            };

            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    {nameof(Padding.All), 1},
                    {nameof(Padding.Left), 2},
                    {nameof(Padding.Top), 2},
                    {nameof(Padding.Right), new object()},
                    {nameof(Padding.Bottom), 4}
                }
            };
            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    {nameof(Padding.All), 1},
                    {nameof(Padding.Left), 2},
                    {nameof(Padding.Top), 2},
                    {nameof(Padding.Right), null},
                    {nameof(Padding.Bottom), 4}
                }
            };
            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    {nameof(Padding.All), 1},
                    {nameof(Padding.Left), 2},
                    {nameof(Padding.Top), 2},
                    {nameof(Padding.Bottom), 4}
                }
            };
    
            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    {nameof(Padding.All), 1},
                    {nameof(Padding.Left), 2},
                    {nameof(Padding.Top), 2},
                    {nameof(Padding.Right), 3},
                    {nameof(Padding.Bottom), new object()}
                }
            };
            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    {nameof(Padding.All), 1},
                    {nameof(Padding.Left), 2},
                    {nameof(Padding.Top), 2},
                    {nameof(Padding.Right), 3},
                    {nameof(Padding.Bottom), null}
                }
            };
            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    {nameof(Padding.All), 1},
                    {nameof(Padding.Left), 2},
                    {nameof(Padding.Top), 2},
                    {nameof(Padding.Right), 3}
                }
            };
        }

        [Theory]
        [MemberData(nameof(CreateInstance_InvalidPropertyValueType_TestData))]
        public void PaddingConverter_CreateInstance_InvalidPropertyValueType_ThrowsArgumentException(IDictionary propertyValues)
        {
            var converter = new PaddingConverter();
            var mockContext = new Mock<ITypeDescriptorContext>(MockBehavior.Strict);
            mockContext
                .Setup(c => c.Instance)
                .Returns(new ClassWithPadding { Padding = new Padding(1) });
            mockContext
                .Setup(c => c.PropertyDescriptor)
                .Returns(TypeDescriptor.GetProperties(typeof(ClassWithPadding))[0]);

            Assert.Throws<ArgumentException>("propertyValues", () => converter.CreateInstance(mockContext.Object, propertyValues));
        }

        [Fact]
        public void PaddingConverter_CreateInstance_InvalidInstanceType_ThrowsInvalidCastException()
        {
            var converter = new PaddingConverter();
            var mockContext = new Mock<ITypeDescriptorContext>(MockBehavior.Strict);
            mockContext
                .Setup(c => c.Instance)
                .Returns("abc");
            mockContext
                .Setup(c => c.PropertyDescriptor)
                .Returns(TypeDescriptor.GetProperties(typeof(string))[0]);

            var propertyValues = new Dictionary<string, object>
            {
                {nameof(Padding.All), 1},
                {nameof(Padding.Left), 2},
                {nameof(Padding.Top), 2},
                {nameof(Padding.Right), 3},
                {nameof(Padding.Bottom), 4},
            };
            Assert.Throws<InvalidCastException>(() => converter.CreateInstance(mockContext.Object, propertyValues));
        }

        [Fact]
        public void PaddingConverter_GetCreateInstanceSupported_Invoke_ReturnsTrue()
        {
            var converter = new PaddingConverter();
            Assert.True(converter.GetCreateInstanceSupported());
        }

        [Fact]
        public void PaddingConverter_GetProperties_Invoke_ReturnsExpected()
        {
            var converter = new PaddingConverter();
            PropertyDescriptorCollection properties = converter.GetProperties(null);
            Assert.Equal(5, properties.Count);
            Assert.Equal(nameof(Padding.All), properties[0].Name);
            Assert.Equal(nameof(Padding.Left), properties[1].Name);
            Assert.Equal(nameof(Padding.Top), properties[2].Name);
            Assert.Equal(nameof(Padding.Right), properties[3].Name);
            Assert.Equal(nameof(Padding.Bottom), properties[4].Name);
        }

        [Fact]
        public void PaddingConverter_GetPropertiesSupported_Invoke_ReturnsTrue()
        {
            var converter = new PaddingConverter();
            Assert.True(converter.GetPropertiesSupported());
        }

        private class ClassWithPadding
        {
            public Padding Padding { get; set; }
        }
    }
}
