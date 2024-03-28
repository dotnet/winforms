// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class GiveFeedbackEventArgsTests
{
    [Theory]
    [InlineData(DragDropEffects.None, true)]
    [InlineData((DragDropEffects.None - 1), false)]
    public void Ctor_DragDropEffects_Bool(DragDropEffects effect, bool useDefaultCursors)
    {
        GiveFeedbackEventArgs e = new(effect, useDefaultCursors);
        Assert.Equal(effect, e.Effect);
        Assert.Equal(useDefaultCursors, e.UseDefaultCursors);
    }

    [Theory]
    [MemberData(nameof(CursorOffset_TestData))]
    public void CursorOffset_Set_GetReturnsExpected(Point value)
    {
        GiveFeedbackEventArgs e = new(DragDropEffects.None, false, new Bitmap(1, 1), new Point(0, 0), false)
        {
            CursorOffset = value
        };
        Assert.Equal(value, e.CursorOffset);
    }

    public static IEnumerable<object[]> CursorOffset_TestData()
    {
        yield return new object[] { new Point(1, 1) };
        yield return new object[] { new Point(-1, -1) };
    }

    [Theory]
    [MemberData(nameof(DragImage_TestData))]
    public void DragImage_Set_GetReturnsExpected(Bitmap value)
    {
        GiveFeedbackEventArgs e = new(DragDropEffects.None, false, new Bitmap(2, 2), new Point(0, 0), false)
        {
            DragImage = value
        };
        Assert.Equal(value, e.DragImage);
    }

    public static IEnumerable<object[]> DragImage_TestData()
    {
        yield return new object[] { new Bitmap(1, 1) };
        yield return new object[] { null };
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void UseDefaultCursors_Set_GetReturnsExpected(bool value)
    {
        GiveFeedbackEventArgs e = new(DragDropEffects.None, false)
        {
            UseDefaultCursors = value
        };
        Assert.Equal(value, e.UseDefaultCursors);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void UseDefaultDragImage_Set_GetReturnsExpected(bool value)
    {
        GiveFeedbackEventArgs e = new(DragDropEffects.None, false, new Bitmap(1, 1), new Point(0, 0), false)
        {
            UseDefaultDragImage = value
        };
        Assert.Equal(value, e.UseDefaultDragImage);
    }
}
