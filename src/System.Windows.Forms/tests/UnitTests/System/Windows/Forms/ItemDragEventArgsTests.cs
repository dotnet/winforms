// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class ItemDragEventArgsTests
{
    [Theory]
    [InlineData(MouseButtons.Left)]
    [InlineData((MouseButtons.None - 1))]
    public void Ctor_MouseButtons(MouseButtons button)
    {
        ItemDragEventArgs e = new(button);
        Assert.Equal(button, e.Button);
        Assert.Null(e.Item);
    }

    [Theory]
    [InlineData(MouseButtons.Left, 1)]
    [InlineData((MouseButtons.None - 1), null)]
    public void Ctor_MouseButtons_Object(MouseButtons button, object item)
    {
        ItemDragEventArgs e = new(button, item);
        Assert.Equal(button, e.Button);
        Assert.Equal(item, e.Item);
    }
}
