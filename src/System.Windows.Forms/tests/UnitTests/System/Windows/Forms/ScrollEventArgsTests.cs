// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class ScrollEventArgsTests
{
    [Theory]
    [InlineData((ScrollEventType.EndScroll + 1), -2)]
    [InlineData(ScrollEventType.LargeIncrement, -1)]
    [InlineData(ScrollEventType.EndScroll, 0)]
    [InlineData(ScrollEventType.LargeIncrement, 1)]
    public void Ctor_ScrollEventType_Int(ScrollEventType type, int newValue)
    {
        ScrollEventArgs e = new(type, newValue);
        Assert.Equal(type, e.Type);
        Assert.Equal(newValue, e.NewValue);
        Assert.Equal(-1, e.OldValue);
        Assert.Equal(ScrollOrientation.HorizontalScroll, e.ScrollOrientation);
    }

    [Theory]
    [InlineData((ScrollEventType.EndScroll + 1), -2, -2)]
    [InlineData(ScrollEventType.LargeIncrement, -1, -1)]
    [InlineData(ScrollEventType.EndScroll, 0, 0)]
    [InlineData(ScrollEventType.LargeIncrement, 1, 2)]
    public void Ctor_ScrollEventType_Int_Int(ScrollEventType type, int oldValue, int newValue)
    {
        ScrollEventArgs e = new(type, oldValue, newValue);
        Assert.Equal(type, e.Type);
        Assert.Equal(oldValue, e.OldValue);
        Assert.Equal(newValue, e.NewValue);
        Assert.Equal(ScrollOrientation.HorizontalScroll, e.ScrollOrientation);
    }

    [Theory]
    [InlineData((ScrollEventType.EndScroll + 1), -2, (ScrollOrientation.HorizontalScroll - 1))]
    [InlineData(ScrollEventType.LargeIncrement, -1, ScrollOrientation.HorizontalScroll)]
    [InlineData(ScrollEventType.EndScroll, 0, ScrollOrientation.VerticalScroll)]
    [InlineData(ScrollEventType.LargeIncrement, 1, ScrollOrientation.VerticalScroll)]
    public void Ctor_ScrollEventType_Int_ScrollOrientation(ScrollEventType type, int newValue, ScrollOrientation scroll)
    {
        ScrollEventArgs e = new(type, newValue, scroll);
        Assert.Equal(type, e.Type);
        Assert.Equal(-1, e.OldValue);
        Assert.Equal(newValue, e.NewValue);
        Assert.Equal(scroll, e.ScrollOrientation);
    }

    [Theory]
    [InlineData((ScrollEventType.EndScroll + 1), -2, -2, (ScrollOrientation.HorizontalScroll - 1))]
    [InlineData(ScrollEventType.LargeIncrement, -1, -1, ScrollOrientation.HorizontalScroll)]
    [InlineData(ScrollEventType.EndScroll, 0, 0, ScrollOrientation.VerticalScroll)]
    [InlineData(ScrollEventType.LargeIncrement, 1, 2, ScrollOrientation.VerticalScroll)]
    public void Ctor_ScrollEventType_Int_Int_ScrollOrientation(ScrollEventType type, int oldValue, int newValue, ScrollOrientation scroll)
    {
        ScrollEventArgs e = new(type, oldValue, newValue, scroll);
        Assert.Equal(type, e.Type);
        Assert.Equal(oldValue, e.OldValue);
        Assert.Equal(newValue, e.NewValue);
        Assert.Equal(scroll, e.ScrollOrientation);
    }

    [Theory]
    [InlineData(-2)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void NewValue_Set_GetReturnsExpected(int value)
    {
        ScrollEventArgs e = new(ScrollEventType.LargeIncrement, 2)
        {
            NewValue = value
        };
        Assert.Equal(value, e.NewValue);
    }
}
