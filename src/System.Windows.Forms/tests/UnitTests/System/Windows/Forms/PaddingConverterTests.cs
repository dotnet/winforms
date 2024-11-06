// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using Moq;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class PaddingConverterTests
{
    [Theory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetConvertFromTheoryData))]
    [InlineData(typeof(Padding), false)]
    [InlineData(typeof(string), true)]
    public void PaddingConverter_CanConvertFrom_Invoke_ReturnsExpected(Type sourceType, bool expected)
    {
        PaddingConverter converter = new();
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
    [UseDefaultXunitCulture]
    [MemberData(nameof(ConvertFrom_TestData))]
    public void PaddingConverter_ConvertFrom_String_ReturnsExpected(string value, object expected)
    {
        PaddingConverter converter = new();
        Assert.Equal(expected, converter.ConvertFrom(value));
        Assert.Equal(expected, converter.ConvertFrom(null, null, value));
        Assert.Equal(expected, converter.ConvertFrom(null, CultureInfo.InvariantCulture, value));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(null)]
    public void PaddingConverter_ConvertFrom_InvalidValue_ThrowsNotSupportedException(object value)
    {
        PaddingConverter converter = new();
        Assert.Throws<NotSupportedException>(() => converter.ConvertFrom(value));
    }

    [Theory]
    [InlineData("1,2,3")]
    [InlineData("1,2,3,4,5")]
    public void PaddingConverter_ConvertFrom_InvalidString_ThrowsArgumentException(string value)
    {
        PaddingConverter converter = new();
        Assert.Throws<ArgumentException>("value", () => converter.ConvertFrom(value));
    }

    [Theory]
    [InlineData(typeof(string), true)]
    [InlineData(typeof(InstanceDescriptor), true)]
    [InlineData(typeof(TableLayoutPanelCellPosition), false)]
    [InlineData(typeof(int), false)]
    [InlineData(null, false)]
    public void PaddingConverter_CanConvertTo_Invoke_ReturnsExpected(Type destinationType, bool expected)
    {
        PaddingConverter converter = new();
        Assert.Equal(expected, converter.CanConvertTo(destinationType));
    }

    [Fact]
    [UseDefaultXunitCulture]
    public void PaddingConverter_ConvertTo_String_ReturnsExpected()
    {
        PaddingConverter converter = new();
        Assert.Equal("1, 2, 3, 4", converter.ConvertTo(new Padding(1, 2, 3, 4), typeof(string)));
        Assert.Equal("1, 2, 3, 4", converter.ConvertTo(null, null, new Padding(1, 2, 3, 4), typeof(string)));
        Assert.Equal("1, 2, 3, 4", converter.ConvertTo(null, CultureInfo.InvariantCulture, new Padding(1, 2, 3, 4), typeof(string)));
    }

    [Fact]
    public void PaddingConverter_ConvertTo_InstanceDescriptor_ReturnsExpected()
    {
        PaddingConverter converter = new();
        InstanceDescriptor descriptor = Assert.IsType<InstanceDescriptor>(converter.ConvertTo(new Padding(1, 2, 3, 4), typeof(InstanceDescriptor)));
        Assert.Equal(typeof(Padding).GetConstructor([typeof(int), typeof(int), typeof(int), typeof(int)]), descriptor.MemberInfo);
        Assert.Equal(new object[] { 1, 2, 3, 4 }, descriptor.Arguments);
    }

    [Fact]
    public void PaddingConverter_ConvertTo_InstanceDescriptorAll_ReturnsExpected()
    {
        PaddingConverter converter = new();
        InstanceDescriptor descriptor = Assert.IsType<InstanceDescriptor>(converter.ConvertTo(new Padding(1, 1, 1, 1), typeof(InstanceDescriptor)));
        Assert.Equal(typeof(Padding).GetConstructor([typeof(int)]), descriptor.MemberInfo);
        Assert.Equal(new object[] { 1 }, descriptor.Arguments);
    }

    [Fact]
    public void PaddingConverter_ConvertTo_NullDestinationType_ThrowsArgumentNullException()
    {
        PaddingConverter converter = new();
        Assert.Throws<ArgumentNullException>("destinationType", () => converter.ConvertTo(new object(), null));
    }

    [Fact]
    public void PaddingConverter_ConvertTo_ValueNotPadding_ThrowsNotSupportedException()
    {
        PaddingConverter converter = new();
        Assert.Throws<NotSupportedException>(() => converter.ConvertTo(1, typeof(InstanceDescriptor)));
    }

    [Theory]
    [InlineData(typeof(Padding))]
    [InlineData(typeof(int))]
    public void PaddingConverter_ConvertTo_InvalidDestinationType_ThrowsNotSupportedException(Type destinationType)
    {
        PaddingConverter converter = new();
        Assert.Throws<NotSupportedException>(() => converter.ConvertTo(default(Padding), destinationType));
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
        PaddingConverter converter = new();
        Mock<ITypeDescriptorContext> mockContext = new(MockBehavior.Strict);
        mockContext
            .Setup(c => c.Instance)
            .Returns(new ClassWithPadding { Padding = instance });
        mockContext
            .Setup(c => c.PropertyDescriptor)
            .Returns(TypeDescriptor.GetProperties(typeof(ClassWithPadding))[0]);
        Padding padding = Assert.IsType<Padding>(converter.CreateInstance(
            mockContext.Object, new Dictionary<string, object>
            {
                { nameof(Padding.All), all },
                { nameof(Padding.Left), 1 },
                { nameof(Padding.Top), 2 },
                { nameof(Padding.Right), 3 },
                { nameof(Padding.Bottom), 4 }
            }));
        Assert.Equal(result, padding);
    }

    [Fact]
    public void PaddingConverter_CreateInstance_ValidPropertyValuesNullContext_ReturnsExpected()
    {
        PaddingConverter converter = new();
        Padding expected = new(1, 2, 3, 4);
        Padding padding = Assert.IsType<Padding>(converter.CreateInstance(
            null, new Dictionary<string, object>
            {
                { nameof(Padding.All), expected.All },
                { nameof(Padding.Left), expected.Left },
                { nameof(Padding.Top), expected.Top },
                { nameof(Padding.Right), expected.Right },
                { nameof(Padding.Bottom), expected.Bottom }
            }));
        Assert.Equal(expected, padding);
    }

    [Fact]
    public void PaddingConverter_CreateInstance_NullPropertyValues_ThrowsArgumentNullException()
    {
        PaddingConverter converter = new();
        Assert.Throws<ArgumentNullException>("propertyValues", () => converter.CreateInstance(new Mock<ITypeDescriptorContext>(MockBehavior.Strict).Object, null));
    }

    public static IEnumerable<object[]> CreateInstance_InvalidPropertyValueType_TestData()
    {
        yield return new object[]
        {
            new Dictionary<string, object>
            {
                { nameof(Padding.All), new object() },
                { nameof(Padding.Left), 2 },
                { nameof(Padding.Top), 2 },
                { nameof(Padding.Right), 3 },
                { nameof(Padding.Bottom), 4 }
            }
        };
        yield return new object[]
        {
            new Dictionary<string, object>
            {
                { nameof(Padding.All), null },
                { nameof(Padding.Left), 2 },
                { nameof(Padding.Top), 2 },
                { nameof(Padding.Right), 3 },
                { nameof(Padding.Bottom), 4 }
            }
        };
        yield return new object[]
        {
            new Dictionary<string, object>
            {
                { nameof(Padding.Left), 2 },
                { nameof(Padding.Top), 2 },
                { nameof(Padding.Right), 3 },
                { nameof(Padding.Bottom), 4 }
            }
        };

        yield return new object[]
        {
            new Dictionary<string, object>
            {
                { nameof(Padding.All), 1 },
                { nameof(Padding.Left), new object() },
                { nameof(Padding.Top), 2 },
                { nameof(Padding.Right), 3 },
                { nameof(Padding.Bottom), 4 }
            }
        };
        yield return new object[]
        {
            new Dictionary<string, object>
            {
                { nameof(Padding.All), 1 },
                { nameof(Padding.Left), null },
                { nameof(Padding.Top), 2 },
                { nameof(Padding.Right), 3 },
                { nameof(Padding.Bottom), 4 }
            }
        };
        yield return new object[]
        {
            new Dictionary<string, object>
            {
                { nameof(Padding.All), 1 },
                { nameof(Padding.Top), 2 },
                { nameof(Padding.Right), 3 },
                { nameof(Padding.Bottom), 4 }
            }
        };

        yield return new object[]
        {
            new Dictionary<string, object>
            {
                { nameof(Padding.All), 1 },
                { nameof(Padding.Left), 2 },
                { nameof(Padding.Top), new object() },
                { nameof(Padding.Right), 3 },
                { nameof(Padding.Bottom), 4 }
            }
        };
        yield return new object[]
        {
            new Dictionary<string, object>
            {
                { nameof(Padding.All), 1 },
                { nameof(Padding.Left), 2 },
                { nameof(Padding.Top), null },
                { nameof(Padding.Right), 3 },
                { nameof(Padding.Bottom), 4 }
            }
        };
        yield return new object[]
        {
            new Dictionary<string, object>
            {
                { nameof(Padding.All), 1 },
                { nameof(Padding.Left), 2 },
                { nameof(Padding.Right), 3 },
                { nameof(Padding.Bottom), 4 }
            }
        };

        yield return new object[]
        {
            new Dictionary<string, object>
            {
                { nameof(Padding.All), 1 },
                { nameof(Padding.Left), 2 },
                { nameof(Padding.Top), 2 },
                { nameof(Padding.Right), new object() },
                { nameof(Padding.Bottom), 4 }
            }
        };
        yield return new object[]
        {
            new Dictionary<string, object>
            {
                { nameof(Padding.All), 1 },
                { nameof(Padding.Left), 2 },
                { nameof(Padding.Top), 2 },
                { nameof(Padding.Right), null },
                { nameof(Padding.Bottom), 4 }
            }
        };
        yield return new object[]
        {
            new Dictionary<string, object>
            {
                { nameof(Padding.All), 1 },
                { nameof(Padding.Left), 2 },
                { nameof(Padding.Top), 2 },
                { nameof(Padding.Bottom), 4 }
            }
        };

        yield return new object[]
        {
            new Dictionary<string, object>
            {
                { nameof(Padding.All), 1 },
                { nameof(Padding.Left), 2 },
                { nameof(Padding.Top), 2 },
                { nameof(Padding.Right), 3 },
                { nameof(Padding.Bottom), new object() }
            }
        };
        yield return new object[]
        {
            new Dictionary<string, object>
            {
                { nameof(Padding.All), 1 },
                { nameof(Padding.Left), 2 },
                { nameof(Padding.Top), 2 },
                { nameof(Padding.Right), 3 },
                { nameof(Padding.Bottom), null }
            }
        };
        yield return new object[]
        {
            new Dictionary<string, object>
            {
                { nameof(Padding.All), 1 },
                { nameof(Padding.Left), 2 },
                { nameof(Padding.Top), 2 },
                { nameof(Padding.Right), 3 }
            }
        };
    }

    [Theory]
    [MemberData(nameof(CreateInstance_InvalidPropertyValueType_TestData))]
    public void PaddingConverter_CreateInstance_InvalidPropertyValueType_ThrowsArgumentException(IDictionary propertyValues)
    {
        PaddingConverter converter = new();
        Mock<ITypeDescriptorContext> mockContext = new(MockBehavior.Strict);
        mockContext
            .Setup(c => c.Instance)
            .Returns(new ClassWithPadding { Padding = new(1) });
        mockContext
            .Setup(c => c.PropertyDescriptor)
            .Returns(TypeDescriptor.GetProperties(typeof(ClassWithPadding))[0]);

        Assert.Throws<ArgumentException>("propertyValues", () => converter.CreateInstance(mockContext.Object, propertyValues));
    }

    [Fact]
    public void PaddingConverter_CreateInstance_UnknownInstanceType_ReturnsExpected()
    {
        PaddingConverter converter = new();
        Mock<ITypeDescriptorContext> mockContext = new(MockBehavior.Strict);
        mockContext
            .Setup(c => c.Instance)
            .Returns("abc");
        mockContext
            .Setup(c => c.PropertyDescriptor)
            .Returns(TypeDescriptor.GetProperties(typeof(string))[0]);

        var propertyValues = new Dictionary<string, object>
        {
            { nameof(Padding.All), 1 },
            { nameof(Padding.Left), 2 },
            { nameof(Padding.Top), 2 },
            { nameof(Padding.Right), 3 },
            { nameof(Padding.Bottom), 4 },
        };

        Padding expected = new(2, 2, 3, 4);
        Padding padding = Assert.IsType<Padding>(converter.CreateInstance(mockContext.Object, propertyValues));
        Assert.Equal(expected, padding);
    }

    [Fact]
    public void PaddingConverter_GetCreateInstanceSupported_Invoke_ReturnsTrue()
    {
        PaddingConverter converter = new();
        Assert.True(converter.GetCreateInstanceSupported());
    }

    [Fact]
    public void PaddingConverter_GetProperties_Invoke_ReturnsExpected()
    {
        PaddingConverter converter = new();
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
        PaddingConverter converter = new();
        Assert.True(converter.GetPropertiesSupported());
    }

    private class ClassWithPadding
    {
        public Padding Padding { get; set; }
    }
}
