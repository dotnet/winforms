// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class LinkConverterTests
{
    [Theory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetConvertFromTheoryData))]
    [InlineData(typeof(LinkLabel.Link), false)]
    [InlineData(typeof(string), true)]
    public void LinkConverter_CanConvertFrom_Invoke_ReturnsExpected(Type sourceType, bool expected)
    {
        LinkConverter converter = new();
        Assert.Equal(expected, converter.CanConvertFrom(sourceType));
    }

    public static IEnumerable<object[]> ConvertFrom_TestData()
    {
        yield return new object[] { "1,2", new LinkLabel.Link(1, 2) };
        yield return new object[] { "  1 , 1", new LinkLabel.Link(1, 1) };
        yield return new object[] { "   ", null };
        yield return new object[] { string.Empty, null };
    }

    [Theory]
    [MemberData(nameof(ConvertFrom_TestData))]
    public void LinkConverter_ConvertFrom_String_ReturnsExpected(string value, object expected)
    {
        LinkConverter converter = new();
        AssertEqualLink(expected, converter.ConvertFrom(value));
        AssertEqualLink(expected, converter.ConvertFrom(null, null, value));
        AssertEqualLink(expected, converter.ConvertFrom(null, CultureInfo.InvariantCulture, value));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(null)]
    public void LinkConverter_ConvertFrom_InvalidValue_ThrowsNotSupportedException(object value)
    {
        LinkConverter converter = new();
        Assert.Throws<NotSupportedException>(() => converter.ConvertFrom(value));
    }

    [Theory]
    [InlineData("1")]
    [InlineData("1,2,3")]
    public void LinkConverter_ConvertFrom_InvalidString_ThrowsArgumentException(string value)
    {
        LinkConverter converter = new();
        Assert.Throws<ArgumentException>(() => converter.ConvertFrom(value));
    }

    [Theory]
    [InlineData(typeof(string), true)]
    [InlineData(typeof(InstanceDescriptor), true)]
    [InlineData(typeof(LinkLabel.Link), false)]
    [InlineData(typeof(int), false)]
    [InlineData(null, false)]
    public void LinkConverter_CanConvertTo_Invoke_ReturnsExpected(Type destinationType, bool expected)
    {
        LinkConverter converter = new();
        Assert.Equal(expected, converter.CanConvertTo(destinationType));
    }

    [Fact]
    public void LinkConverter_ConvertTo_String_ReturnsExpected()
    {
        LinkConverter converter = new();
        Assert.Equal("1, 2", converter.ConvertTo(new LinkLabel.Link(1, 2), typeof(string)));
        Assert.Equal("1, 2", converter.ConvertTo(null, null, new LinkLabel.Link(1, 2), typeof(string)));
        Assert.Equal("1, 2", converter.ConvertTo(null, CultureInfo.InvariantCulture, new LinkLabel.Link(1, 2), typeof(string)));
    }

    [Fact]
    public void LinkConverter_ConvertTo_InstanceDescriptor_ReturnsExpected()
    {
        LinkConverter converter = new();
        InstanceDescriptor descriptor = Assert.IsType<InstanceDescriptor>(converter.ConvertTo(new LinkLabel.Link(1, 2), typeof(InstanceDescriptor)));
        Assert.Equal(typeof(LinkLabel.Link).GetConstructor([typeof(int), typeof(int)]), descriptor.MemberInfo);
        Assert.Equal(new object[] { 1, 2 }, descriptor.Arguments);
        Assert.True(descriptor.IsComplete);
    }

    [Fact]
    public void LinkConverter_ConvertTo_InstanceDescriptorWithLinkData_ReturnsExpected()
    {
        LinkConverter converter = new();
        InstanceDescriptor descriptor = Assert.IsType<InstanceDescriptor>(converter.ConvertTo(new LinkLabel.Link(1, 2, "linkData"), typeof(InstanceDescriptor)));
        Assert.Equal(typeof(LinkLabel.Link).GetConstructor([typeof(int), typeof(int), typeof(object)]), descriptor.MemberInfo);
        Assert.Equal(new object[] { 1, 2, "linkData" }, descriptor.Arguments);
        Assert.True(descriptor.IsComplete);
    }

    [Fact]
    public void LinkConverter_ConvertTo_NullDestinationType_ThrowsArgumentNullException()
    {
        LinkConverter converter = new();
        Assert.Throws<ArgumentNullException>("destinationType", () => converter.ConvertTo(new object(), null));
    }

    [Fact]
    public void LinkConverter_ConvertTo_ValueNotLink_ThrowsNotSupportedException()
    {
        LinkConverter converter = new();
        Assert.Throws<NotSupportedException>(() => converter.ConvertTo(1, typeof(InstanceDescriptor)));
    }

    [Theory]
    [InlineData(typeof(LinkLabel.Link))]
    [InlineData(typeof(int))]
    public void LinkConverter_ConvertTo_InvalidDestinationType_ThrowsNotSupportedException(Type destinationType)
    {
        LinkConverter converter = new();
        Assert.Throws<NotSupportedException>(() => converter.ConvertTo(new LinkLabel.Link(), destinationType));
    }

    [Fact]
    public void LinkConverter_GetCreateInstanceSupported_Invoke_ReturnsFalse()
    {
        LinkConverter converter = new();
        Assert.False(converter.GetCreateInstanceSupported());
    }

    [Fact]
    public void LinkConverter_GetPropertiesSupported_Invoke_ReturnsFalse()
    {
        LinkConverter converter = new();
        Assert.False(converter.GetPropertiesSupported());
    }

    private static void AssertEqualLink(object expected, object actual)
    {
        if (expected is LinkLabel.Link expectedLink && actual is LinkLabel.Link actualLink)
        {
            Assert.Equal(expectedLink.Description, actualLink.Description);
            Assert.Equal(expectedLink.Enabled, actualLink.Enabled);
            Assert.Equal(expectedLink.Length, actualLink.Length);
            Assert.Equal(expectedLink.LinkData, actualLink.LinkData);
            Assert.Equal(expectedLink.Name, actualLink.Name);
            Assert.Equal(expectedLink.Start, actualLink.Start);
            Assert.Equal(expectedLink.Tag, actualLink.Tag);
            Assert.Equal(expectedLink.Visited, actualLink.Visited);
        }
        else
        {
            Assert.Equal(expected, actual);
        }
    }
}
