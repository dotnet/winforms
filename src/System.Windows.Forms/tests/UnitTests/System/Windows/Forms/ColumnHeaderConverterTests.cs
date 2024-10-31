// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class ColumnHeaderConverterTests
{
    public static TheoryData<Type, bool> CanConvertFromData =>
        CommonTestHelper.GetConvertFromTheoryData();

    [Theory]
    [MemberData(nameof(CanConvertFromData))]
    [InlineData(typeof(ColumnHeader), false)]
    public void ColumnHeaderConverter_CanConvertFrom_Invoke_ReturnsExpected(Type sourceType, bool expected)
    {
        ColumnHeaderConverter converter = new();
        Assert.Equal(expected, converter.CanConvertFrom(sourceType));
    }

    [Theory]
    [InlineData("value")]
    [InlineData(1)]
    [InlineData(null)]
    public void ColumnHeaderConverter_ConvertFrom_InvalidValue_ThrowsNotSupportedException(object value)
    {
        ColumnHeaderConverter converter = new();
        Assert.Throws<NotSupportedException>(() => converter.ConvertFrom(value));
    }

    [Theory]
    [InlineData(typeof(string), true)]
    [InlineData(typeof(InstanceDescriptor), true)]
    [InlineData(typeof(ColumnHeader), false)]
    [InlineData(typeof(int), false)]
    [InlineData(null, false)]
    public void ColumnHeaderConverter_CanConvertTo_Invoke_ReturnsExpected(Type destinationType, bool expected)
    {
        ColumnHeaderConverter converter = new();
        Assert.Equal(expected, converter.CanConvertTo(destinationType));
    }

    public static IEnumerable<object[]> ConvertTo_InstanceDescriptor_TestData()
    {
        yield return new object[]
        {
            new ColumnHeader(),
            Array.Empty<Type>(),
            Array.Empty<object>()
        };
        yield return new object[]
        {
            new ColumnHeader("imageKey"),
            new Type[] { typeof(string) },
            new object[] { "imageKey" }
        };
        yield return new object[]
        {
            new ColumnHeader(1),
            new Type[] { typeof(int) },
            new object[] { 1 }
        };
        yield return new object[]
        {
            new PrivateIntConstructor() { ImageIndex = 1 },
            Array.Empty<Type>(),
            Array.Empty<object>()
        };
        yield return new object[]
        {
            new PrivateStringConstructor() { ImageKey = "imageKey" },
            Array.Empty<Type>(),
            Array.Empty<object>()
        };
        yield return new object[]
        {
            new PrivateDefaultConstructor("imageKey"),
            new Type[] { typeof(string) },
            new object[] { "imageKey" }
        };
        yield return new object[]
        {
            new PrivateDefaultConstructor(1),
            new Type[] { typeof(int) },
            new object[] { 1 }
        };
    }

    [Theory]
    [MemberData(nameof(ConvertTo_InstanceDescriptor_TestData))]
    public void ColumnHeaderConverter_ConvertTo_InstanceDescriptor_ReturnsExpected(ColumnHeader value, Type[] parameterTypes, object[] arguments)
    {
        ColumnHeaderConverter converter = new();
        InstanceDescriptor descriptor = Assert.IsType<InstanceDescriptor>(converter.ConvertTo(value, typeof(InstanceDescriptor)));
        Assert.Equal(value.GetType().GetConstructor(parameterTypes), descriptor.MemberInfo);
        Assert.Equal(arguments, descriptor.Arguments);
    }

    [WinFormsFact]
    public void ColumnHeaderConverter_ConvertTo_NoPublicDefaultConstructor_ThrowsArgumentException()
    {
        using PrivateDefaultConstructor value = new(-1);
        ColumnHeaderConverter converter = new();
        Assert.Throws<ArgumentException>(() => converter.ConvertTo(value, typeof(InstanceDescriptor)));
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData(1, "1")]
    public void ColumnHeaderConverter_ConvertTo_String_ReturnsExpected(object value, string expected)
    {
        ColumnHeaderConverter converter = new();
        Assert.Equal(expected, converter.ConvertTo(value, typeof(string)));
    }

    [Fact]
    public void ColumnHeaderConverter_ConvertTo_NullDestinationType_ThrowsArgumentNullException()
    {
        ColumnHeaderConverter converter = new();
        Assert.Throws<ArgumentNullException>("destinationType", () => converter.ConvertTo(new object(), null));
    }

    [Fact]
    public void ColumnHeaderConverter_ConvertTo_ValueNotThrowsNotSupportedException()
    {
        ColumnHeaderConverter converter = new();
        Assert.Throws<NotSupportedException>(() => converter.ConvertTo(1, typeof(InstanceDescriptor)));
    }

    [Theory]
    [InlineData(typeof(ColumnHeader))]
    [InlineData(typeof(int))]
    public void ColumnHeaderConverter_ConvertTo_InvalidDestinationType_ThrowsNotSupportedException(Type destinationType)
    {
        ColumnHeaderConverter converter = new();
        Assert.Throws<NotSupportedException>(() => converter.ConvertTo(new ColumnHeader(), destinationType));
    }

    [Fact]
    public void ColumnHeaderConverter_GetPropertiesSupported_Invoke_ReturnsTrue()
    {
        ColumnHeaderConverter converter = new();
        Assert.True(converter.GetPropertiesSupported(null));
    }

    [WinFormsFact]
    public void ColumnHeaderConverter_GetProperties_Invoke_ReturnsExpected()
    {
        ColumnHeaderConverter converter = new();
        using ColumnHeader item = new();
        Assert.Equal(TypeDescriptor.GetProperties(item, null).Count, converter.GetProperties(null, item, null).Count);
    }

    [Fact]
    public void ColumnHeaderConverter_GetStandardValues_Invoke_ReturnsNull()
    {
        ColumnHeaderConverter converter = new();
        Assert.Null(converter.GetStandardValues(null));
    }

    [Fact]
    public void ColumnHeaderConverter_GetStandardValuesExclusive_Invoke_ReturnsFalse()
    {
        ColumnHeaderConverter converter = new();
        Assert.False(converter.GetStandardValuesExclusive(null));
    }

    [Fact]
    public void ColumnHeaderConverter_GetStandardValuesSupported_Invoke_ReturnsFalse()
    {
        ColumnHeaderConverter converter = new();
        Assert.False(converter.GetStandardValuesSupported(null));
    }

    private class PrivateDefaultConstructor : ColumnHeader
    {
        private PrivateDefaultConstructor() : base() { }

        public PrivateDefaultConstructor(string imageKey) : base(imageKey) { }

        public PrivateDefaultConstructor(int imageIndex) : base(imageIndex) { }
    }

#pragma warning disable IDE0051 // Remove unused private members
    private class PrivateStringConstructor : ColumnHeader
    {
        public PrivateStringConstructor() : base() { }

        private PrivateStringConstructor(string imageKey) : base(imageKey) { }

        public PrivateStringConstructor(int imageIndex) : base(imageIndex) { }
    }

    private class PrivateIntConstructor : ColumnHeader
    {
        public PrivateIntConstructor() : base() { }

        public PrivateIntConstructor(string imageKey) : base(imageKey) { }

        private PrivateIntConstructor(int imageIndex) : base(imageIndex) { }
    }
#pragma warning restore IDE0051
}
