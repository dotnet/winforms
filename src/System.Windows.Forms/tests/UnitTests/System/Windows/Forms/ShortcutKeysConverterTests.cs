// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms.Tests;

public class ShortcutKeysConverterTests
{
    [Fact]
    public void GetStandardValues_ShouldReturnEmptyCollection()
    {
        ShortcutKeysConverter converter = new();
        var values = converter.GetStandardValues(null).Should().BeOfType<TypeConverter.StandardValuesCollection>().Subject;
        values.Count.Should().Be(0);
    }

    [Fact]
    public void GetStandardValuesSupported_ShouldReturnFalse()
    {
        ShortcutKeysConverter converter = new();
        converter.GetStandardValuesSupported(null).Should().BeFalse();
    }
}
