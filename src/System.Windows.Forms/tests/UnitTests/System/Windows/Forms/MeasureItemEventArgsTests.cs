// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class MeasureItemEventArgsTests
{
    public static IEnumerable<object[]> Ctor_Graphics_Int_Int_TestData()
    {
        Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);

        yield return new object[] { null, -1, -1 };
        yield return new object[] { graphics, 0, 0 };
        yield return new object[] { graphics, 1, 2 };
    }

    [Theory]
    [MemberData(nameof(Ctor_Graphics_Int_Int_TestData))]
    public void Ctor_Graphics_Int_Int(Graphics graphics, int index, int itemHeight)
    {
        MeasureItemEventArgs e = new(graphics, index, itemHeight);
        Assert.Equal(graphics, e.Graphics);
        Assert.Equal(index, e.Index);
        Assert.Equal(itemHeight, e.ItemHeight);
        Assert.Equal(0, e.ItemWidth);
    }

    public static IEnumerable<object[]> Ctor_Graphics_Int_TestData()
    {
        Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);

        yield return new object[] { null, -1 };
        yield return new object[] { graphics, 0 };
        yield return new object[] { graphics, 1 };
    }

    [Theory]
    [MemberData(nameof(Ctor_Graphics_Int_TestData))]
    public void Ctor_Graphics_Int(Graphics graphics, int index)
    {
        MeasureItemEventArgs e = new(graphics, index);
        Assert.Equal(graphics, e.Graphics);
        Assert.Equal(index, e.Index);
        Assert.Equal(0, e.ItemHeight);
        Assert.Equal(0, e.ItemWidth);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ItemHeight_Set_GetReturnsExpected(int value)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        MeasureItemEventArgs e = new(graphics, 1)
        {
            ItemHeight = value
        };
        Assert.Equal(value, e.ItemHeight);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ItemWidth_Set_GetReturnsExpected(int value)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        MeasureItemEventArgs e = new(graphics, 1)
        {
            ItemWidth = value
        };
        Assert.Equal(value, e.ItemWidth);
    }
}
