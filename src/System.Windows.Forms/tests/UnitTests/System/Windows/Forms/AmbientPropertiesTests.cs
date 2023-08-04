﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.TestUtilities;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class AmbientPropertiesTests
{
    [Fact]
    public void Ctor_Default()
    {
        var property = new AmbientProperties();
        Assert.Equal(Color.Empty, property.BackColor);
        Assert.Null(property.Cursor);
        Assert.Null(property.Font);
        Assert.Equal(Color.Empty, property.ForeColor);
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetColorWithEmptyTheoryData))]
    public void BackColor_Set_GetReturnsExpected(Color value)
    {
        var property = new AmbientProperties
        {
            BackColor = value
        };
        Assert.Equal(value, property.BackColor);

        // Set same.
        property.BackColor = value;
        Assert.Equal(value, property.BackColor);
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetCursorTheoryData))]
    public void Cursor_Set_GetReturnsExpected(Cursor value)
    {
        var property = new AmbientProperties
        {
            Cursor = value
        };
        Assert.Equal(value, property.Cursor);

        // Set same.
        property.Cursor = value;
        Assert.Equal(value, property.Cursor);
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetFontTheoryData))]
    public void Font_Set_GetReturnsExpected(Font value)
    {
        var property = new AmbientProperties
        {
            Font = value
        };
        Assert.Equal(value, property.Font);

        // Set same.
        property.Font = value;
        Assert.Equal(value, property.Font);
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetColorTheoryData))]
    public void ForeColor_Set_GetReturnsExpected(Color value)
    {
        var property = new AmbientProperties
        {
            ForeColor = value
        };
        Assert.Equal(value, property.ForeColor);

        // Set same.
        property.ForeColor = value;
        Assert.Equal(value, property.ForeColor);
    }
}
