// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static System.ComponentModel.TypeConverter;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class ListViewItemStateImageIndexConverterTests
{
    [Fact]
    public void ListViewItemStateImageIndexConverter_IncludeNoneAsStandardValue_ReturnsFalse()
    {
        Assert.False(new ListViewItemStateImageIndexConverter().TestAccessor().Dynamic.IncludeNoneAsStandardValue);
    }

    [Fact]
    public void ListViewItemStateImageIndexConverter_GetStandardValues_Null_Context_ReturnsExpected()
    {
        ListViewItemStateImageIndexConverter converter = new();

        StandardValuesCollection result = converter.GetStandardValues(context: null);

        Assert.Empty(result);
    }
}
