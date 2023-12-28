// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static System.ComponentModel.TypeConverter;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class TreeViewImageIndexConverterTests
{
    [Fact]
    public void TreeViewImageIndexConverter_IncludeNoneAsStandardValue_ReturnsFalse()
    {
        Assert.False(new TreeViewImageIndexConverter().TestAccessor().Dynamic.IncludeNoneAsStandardValue);
    }

    [Fact]
    public void TreeViewImageIndexConverter_ConvertTo_destinationType_null_ThrowsArgumentNullException()
    {
        TreeViewImageIndexConverter converter = new();

        Assert.Throws<ArgumentNullException>("destinationType", () => converter.ConvertTo(context: null, culture: null, new object(), destinationType: null));
    }

    public static IEnumerable<object[]> TreeViewImageIndexConverter_ConvertFrom_special_string_to_int_ReturnsExpected_TestData()
    {
        yield return new object[] { SR.toStringDefault, -1, };
        yield return new object[] { SR.toStringDefault, ImageList.Indexer.DefaultIndex };
        yield return new object[] { SR.toStringNone, -2 };
        yield return new object[] { SR.toStringNone, ImageList.Indexer.NoneIndex };
    }

    [WinFormsTheory]
    [MemberData(nameof(TreeViewImageIndexConverter_ConvertFrom_special_string_to_int_ReturnsExpected_TestData))]
    public void TreeViewImageIndexConverter_ConvertFrom_special_string_to_int_ReturnsExpected(object value, object expected)
    {
        TreeViewImageIndexConverter converter = new();

        object result = converter.ConvertFrom(context: null, culture: null, value);

        Assert.Equal(expected, result);
    }

    public static IEnumerable<object[]> TreeViewImageIndexConverter_ConvertTo_special_int_to_string_ReturnsExpected_TestData()
    {
        yield return new object[] { -1, SR.toStringDefault };
        yield return new object[] { ImageList.Indexer.DefaultIndex, SR.toStringDefault };
        yield return new object[] { -2, SR.toStringNone };
        yield return new object[] { ImageList.Indexer.NoneIndex, SR.toStringNone };
    }

    [WinFormsTheory]
    [MemberData(nameof(TreeViewImageIndexConverter_ConvertTo_special_int_to_string_ReturnsExpected_TestData))]
    public void TreeViewImageIndexConverter_ConvertTo_special_int_to_string_ReturnsExpected(object value, object expected)
    {
        TreeViewImageIndexConverter converter = new();

        object result = converter.ConvertTo(context: null, culture: null, value, destinationType: typeof(string));

        Assert.Equal(expected, result);
    }

    [Fact]
    public void TreeViewImageIndexConverter_GetStandardValues_Null_Context_ReturnsExpected()
    {
        TreeViewImageIndexConverter converter = new();

        StandardValuesCollection result = converter.GetStandardValues(context: null);

        Assert.Equal(2, result.Count);
        Assert.Equal(ImageList.Indexer.DefaultIndex, result[0]);
        Assert.Equal(ImageList.Indexer.NoneIndex, result[1]);
    }
}
