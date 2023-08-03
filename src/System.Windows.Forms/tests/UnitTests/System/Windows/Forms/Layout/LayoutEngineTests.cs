// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms.Layout.Tests;

public class LayoutEngineTests
{
    [WinFormsFact]
    public void LayoutEngine_InitLayout_ValidChild_Nop()
    {
        var engine = new SubLayoutEngine();
        engine.InitLayout(new ScrollableControl(), BoundsSpecified.All);
    }

    [WinFormsFact]
    public void LayoutEngine_InitLayout_InvalidChild_ThrowsNotSupportedException()
    {
        var engine = new SubLayoutEngine();
        Assert.Throws<NotSupportedException>(() => engine.InitLayout("child", BoundsSpecified.All));
    }

    [WinFormsFact]
    public void LayoutEngine_InitLayout_NullChild_ThrowsArgumentNullException()
    {
        var engine = new SubLayoutEngine();
        Assert.Throws<ArgumentNullException>("child", () => engine.InitLayout(null, BoundsSpecified.All));
    }

    [WinFormsFact]
    public void LayoutEngine_Layout_ValidContainer_ReturnsFalse()
    {
        var engine = new SubLayoutEngine();
        Assert.False(engine.Layout(new ScrollableControl(), new LayoutEventArgs(new Component(), "affectedProperty")));
    }

    [WinFormsFact]
    public void LayoutEngine_Layout_InvalidContainer_Nop()
    {
        var engine = new SubLayoutEngine();
        Assert.Throws<NotSupportedException>(() => engine.Layout("container", new LayoutEventArgs(new Component(), "affectedProperty")));
    }

    [WinFormsFact]
    public void LayoutEngine_Layout_NullContainer_ThrowsArgumentNullException()
    {
        var engine = new SubLayoutEngine();
        Assert.Throws<ArgumentNullException>("container", () => engine.Layout(null, new LayoutEventArgs(new Component(), "affectedProperty")));
    }

    private class SubLayoutEngine : LayoutEngine
    {
    }
}
