// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class TreeViewImageKeyConverterTests
{
    private readonly TreeViewImageKeyConverter _converter = new();

    [WinFormsTheory]
    [InlineData(null, "(default)")]
    [InlineData("", "(default)")]
    [InlineData("non-empty", "non-empty")]
    public void TreeViewImageKeyConverter_ConvertTo_StringDestinationType_ReturnsExpected(string value, string expected)
    {
        object result = _converter.ConvertTo(context: null, culture: null, value: value, destinationType: typeof(string));
        result.Should().Be(expected);
    }
}
