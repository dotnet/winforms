// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class ListViewItemConverterTests
{
    public static TheoryData<Type, bool> CanConvertFromData =>
        CommonTestHelper.GetConvertFromTheoryData();

    [Theory]
    [MemberData(nameof(CanConvertFromData))]
    [InlineData(typeof(ListViewItem), false)]
    [InlineData(typeof(string), false)]
    public void ListViewItemConverter_CanConvertFrom_Invoke_ReturnsExpected(Type sourceType, bool expected)
    {
        ListViewItemConverter converter = new();
        Assert.Equal(expected, converter.CanConvertFrom(sourceType));
    }

    [Theory]
    [InlineData("value")]
    [InlineData(1)]
    [InlineData(null)]
    public void ListViewItemConverter_ConvertFrom_InvalidValue_ThrowsNotSupportedException(object value)
    {
        ListViewItemConverter converter = new();
        Assert.Throws<NotSupportedException>(() => converter.ConvertFrom(value));
    }

    [Theory]
    [InlineData(typeof(string), true)]
    [InlineData(typeof(InstanceDescriptor), true)]
    [InlineData(typeof(ListViewItem), false)]
    [InlineData(typeof(int), false)]
    [InlineData(null, false)]
    public void ListViewItemConverter_ConverterCanConvertTo_Invoke_ReturnsExpected(Type destinationType, bool expected)
    {
        ListViewItemConverter converter = new();
        Assert.Equal(expected, converter.CanConvertTo(destinationType));
    }

    public static IEnumerable<object[]> ConvertTo_InstanceDescriptor_TestData()
    {
        ListViewGroup group = new();
        var subItem1 = new ListViewItem.ListViewSubItem(null, "text1");
        var subItem2 = new ListViewItem.ListViewSubItem(null, "text2");
        var subItem3 = new ListViewItem.ListViewSubItem(null, "text3")
        {
            ForeColor = Color.Blue
        };
        var subItem4 = new ListViewItem.ListViewSubItem(null, "text4")
        {
            BackColor = Color.Blue
        };
        var subItem5 = new ListViewItem.ListViewSubItem(null, "text5")
        {
            Font = SystemFonts.MenuFont
        };

        // Simple.
        yield return new object[]
        {
            new ListViewItem(),
            new Type[] { typeof(string) },
            new object[] { string.Empty }
        };
        yield return new object[]
        {
            new ListViewItem(group),
            new Type[] { typeof(string) },
            new object[] { string.Empty }
        };
        yield return new object[]
        {
            new ListViewItem("text"),
            new Type[] { typeof(string) },
            new object[] { "text" }
        };
        yield return new object[]
        {
            new ListViewItem("text", group),
            new Type[] { typeof(string) },
            new object[] { "text" }
        };
        yield return new object[]
        {
            new ListViewItem("text", "imageKey"),
            new Type[] { typeof(string), typeof(string) },
            new object[] { "text", "imageKey" }
        };
        yield return new object[]
        {
            new ListViewItem("text", 1),
            new Type[] { typeof(string), typeof(int) },
            new object[] { "text", 1 }
        };
        yield return new object[]
        {
            new ListViewItem("text", "imageKey", group),
            new Type[] { typeof(string), typeof(string) },
            new object[] { "text", "imageKey" }
        };
        yield return new object[]
        {
            new ListViewItem("text", 1, group),
            new Type[] { typeof(string), typeof(int) },
            new object[] { "text", 1 }
        };

        // Item.
        yield return new object[]
        {
            new ListViewItem([subItem1], "imageKey"),
            new Type[] { typeof(string), typeof(string) },
            new object[] { "text1", "imageKey" }
        };
        yield return new object[]
        {
            new ListViewItem([subItem1], 1),
            new Type[] { typeof(string), typeof(int) },
            new object[] { "text1", 1 }
        };
        yield return new object[]
        {
            new ListViewItem([subItem1], "imageKey", group),
            new Type[] { typeof(string), typeof(string) },
            new object[] { "text1", "imageKey" }
        };
        yield return new object[]
        {
            new ListViewItem([subItem1], 1, group),
            new Type[] { typeof(string), typeof(int) },
            new object[] { "text1", 1 }
        };
        yield return new object[]
        {
            new ListViewItem([subItem1, subItem2], "imageKey"),
            new Type[] { typeof(string[]), typeof(string) },
            new object[] { new string[] { "text1", "text2" }, "imageKey" }
        };
        yield return new object[]
        {
            new ListViewItem([subItem1, subItem2], 1),
            new Type[] { typeof(string[]), typeof(int) },
            new object[] { new string[] { "text1", "text2" }, 1 }
        };
        yield return new object[]
        {
            new ListViewItem([subItem1, subItem2], "imageKey", group),
            new Type[] { typeof(string[]), typeof(string) },
            new object[] { new string[] { "text1", "text2" }, "imageKey" }
        };
        yield return new object[]
        {
            new ListViewItem([subItem1, subItem2], 1, group),
            new Type[] { typeof(string[]), typeof(int) },
            new object[] { new string[] { "text1", "text2" }, 1 }
        };
        yield return new object[]
        {
            new ListViewItem([subItem1, subItem2, subItem3, subItem4, subItem5], "imageKey"),
            new Type[] { typeof(ListViewItem.ListViewSubItem[]), typeof(string) },
            new object[] { new ListViewItem.ListViewSubItem[] { subItem1, subItem2, subItem3, subItem4, subItem5 }, "imageKey" }
        };
        yield return new object[]
        {
            new ListViewItem([subItem1, subItem2, subItem3, subItem4, subItem5], 1),
            new Type[] { typeof(ListViewItem.ListViewSubItem[]), typeof(int) },
            new object[] { new ListViewItem.ListViewSubItem[] { subItem1, subItem2, subItem3, subItem4, subItem5 }, 1 }
        };
        yield return new object[]
        {
            new ListViewItem([subItem1, subItem2, subItem3, subItem4, subItem5], "imageKey", group),
            new Type[] { typeof(ListViewItem.ListViewSubItem[]), typeof(string) },
            new object[] { new ListViewItem.ListViewSubItem[] { subItem1, subItem2, subItem3, subItem4, subItem5 }, "imageKey" }
        };
        yield return new object[]
        {
            new ListViewItem([subItem1, subItem2, subItem3, subItem4, subItem5], 1, group),
            new Type[] { typeof(ListViewItem.ListViewSubItem[]), typeof(int) },
            new object[] { new ListViewItem.ListViewSubItem[] { subItem1, subItem2, subItem3, subItem4, subItem5 }, 1 }
        };
        yield return new object[]
        {
            new ListViewItem([subItem3], 1),
            new Type[] { typeof(string[]), typeof(int), typeof(Color), typeof(Color), typeof(Font) },
            new object[] { new string[] { "text3" }, 1, Color.Blue, Color.Empty, null }
        };
        yield return new object[]
        {
            new ListViewItem([subItem4], 1),
            new Type[] { typeof(string[]), typeof(int), typeof(Color), typeof(Color), typeof(Font) },
            new object[] { new string[] { "text4" }, 1, Color.Empty, Color.Blue, null }
        };
        yield return new object[]
        {
            new ListViewItem([subItem5], 1),
            new Type[] { typeof(string[]), typeof(int), typeof(Color), typeof(Color), typeof(Font) },
            new object[] { new string[] { "text5" }, 1, Color.Empty, Color.Empty, SystemFonts.MenuFont }
        };
        yield return new object[]
        {
            new ListViewItem([subItem3], "imageKey"),
            new Type[] { typeof(string[]), typeof(string), typeof(Color), typeof(Color), typeof(Font) },
            new object[] { new string[] { "text3" }, "imageKey", Color.Blue, Color.Empty, null }
        };
        yield return new object[]
        {
            new ListViewItem([subItem4], "imageKey"),
            new Type[] { typeof(string[]), typeof(string), typeof(Color), typeof(Color), typeof(Font) },
            new object[] { new string[] { "text4" }, "imageKey", Color.Empty, Color.Blue, null }
        };
        yield return new object[]
        {
            new ListViewItem([subItem5], "imageKey"),
            new Type[] { typeof(string[]), typeof(string), typeof(Color), typeof(Color), typeof(Font) },
            new object[] { new string[] { "text5" }, "imageKey", Color.Empty, Color.Empty, SystemFonts.MenuFont }
        };

        // Custom style - text.
        yield return new object[]
        {
            new ListViewItem(["text"], "imageKey", Color.Red, Color.Blue, SystemFonts.MenuFont),
            new Type[] { typeof(string[]), typeof(string), typeof(Color), typeof(Color), typeof(Font) },
            new object[] { new string[] { "text" }, "imageKey", Color.Red, Color.Blue, SystemFonts.MenuFont }
        };
        yield return new object[]
        {
            new ListViewItem(["text"], 1, Color.Red, Color.Blue, SystemFonts.MenuFont),
            new Type[] { typeof(string[]), typeof(int), typeof(Color), typeof(Color), typeof(Font) },
            new object[] { new string[] { "text" }, 1, Color.Red, Color.Blue, SystemFonts.MenuFont }
        };
        yield return new object[]
        {
            new ListViewItem(["text"], "imageKey", Color.Red, Color.Blue, SystemFonts.MenuFont, group),
            new Type[] { typeof(string[]), typeof(string), typeof(Color), typeof(Color), typeof(Font) },
            new object[] { new string[] { "text" }, "imageKey", Color.Red, Color.Blue, SystemFonts.MenuFont }
        };
        yield return new object[]
        {
            new ListViewItem(["text"], 1, Color.Red, Color.Blue, SystemFonts.MenuFont, group),
            new Type[] { typeof(string[]), typeof(int), typeof(Color), typeof(Color), typeof(Font) },
            new object[] { new string[] { "text" }, 1, Color.Red, Color.Blue, SystemFonts.MenuFont }
        };
    }

    [Theory]
    [MemberData(nameof(ConvertTo_InstanceDescriptor_TestData))]
    public void ListViewItemConverter_ConvertTo_InstanceDescriptor_ReturnsExpected(ListViewItem value, Type[] parameterTypes, object[] arguments)
    {
        ListViewItemConverter converter = new();
        InstanceDescriptor descriptor = Assert.IsType<InstanceDescriptor>(converter.ConvertTo(value, typeof(InstanceDescriptor)));
        Assert.Equal(typeof(ListViewItem).GetConstructor(parameterTypes), descriptor.MemberInfo);
        Assert.Equal(arguments, descriptor.Arguments);
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData(1, "1")]
    public void ListViewItemConverter_ConvertTo_String_ReturnsExpected(object value, string expected)
    {
        ListViewItemConverter converter = new();
        Assert.Equal(expected, converter.ConvertTo(value, typeof(string)));
    }

    [Fact]
    public void ListViewItemConverter_ConvertTo_NullDestinationType_ThrowsArgumentNullException()
    {
        ListViewItemConverter converter = new();
        Assert.Throws<ArgumentNullException>("destinationType", () => converter.ConvertTo(new object(), null));
    }

    [Fact]
    public void ListViewItemConverter_ConvertTo_ValueNotThrowsNotSupportedException()
    {
        ListViewItemConverter converter = new();
        Assert.Throws<NotSupportedException>(() => converter.ConvertTo(1, typeof(InstanceDescriptor)));
    }

    [Theory]
    [InlineData(typeof(ListViewItem))]
    [InlineData(typeof(int))]
    public void ListViewItemConverter_ConvertTo_InvalidDestinationType_ThrowsNotSupportedException(Type destinationType)
    {
        ListViewItemConverter converter = new();
        Assert.Throws<NotSupportedException>(() => converter.ConvertTo(new ListViewItem(), destinationType));
    }

    [Fact]
    public void ListViewItemConverter_GetPropertiesSupported_Invoke_ReturnsTrue()
    {
        ListViewItemConverter converter = new();
        Assert.True(converter.GetPropertiesSupported(null));
    }

    [Fact]
    public void ListViewItemConverter_GetProperties_Invoke_ReturnsExpected()
    {
        ListViewItemConverter converter = new();
        ListViewItem item = new();
        Assert.Equal(TypeDescriptor.GetProperties(item, null).Count, converter.GetProperties(null, item, null).Count);
    }

    [Fact]
    public void ListViewItemConverter_GetStandardValues_Invoke_ReturnsNull()
    {
        ListViewItemConverter converter = new();
        Assert.Null(converter.GetStandardValues(null));
    }

    [Fact]
    public void ListViewItemConverter_GetStandardValuesExclusive_Invoke_ReturnsFalse()
    {
        ListViewItemConverter converter = new();
        Assert.False(converter.GetStandardValuesExclusive(null));
    }

    [Fact]
    public void ListViewItemConverter_GetStandardValuesSupported_Invoke_ReturnsFalse()
    {
        ListViewItemConverter converter = new();
        Assert.False(converter.GetStandardValuesSupported(null));
    }
}
