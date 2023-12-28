// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Layout.Tests;

public partial class FlowLayoutTests
{
    [WinFormsFact]
    public void LayoutEngine_InitLayout_ValidChild_Nop()
    {
        using FlowLayoutPanel control = new();
        LayoutEngine engine = control.LayoutEngine;
        engine.InitLayout(control, BoundsSpecified.All);
    }

    [WinFormsFact]
    public void LayoutEngine_InitLayout_InvalidChild_ThrowsNotSupportedException()
    {
        using FlowLayoutPanel control = new();
        LayoutEngine engine = control.LayoutEngine;
        Assert.Throws<NotSupportedException>(() => engine.InitLayout("child", BoundsSpecified.All));
    }

    [WinFormsFact]
    public void LayoutEngine_InitLayout_NullChild_ThrowsArgumentNullException()
    {
        using FlowLayoutPanel control = new();
        LayoutEngine engine = control.LayoutEngine;
        Assert.Throws<ArgumentNullException>("child", () => engine.InitLayout(null, BoundsSpecified.All));
    }

    [WinFormsFact]
    public void LayoutEngine_Layout_ValidContainerLeftToRight_Success()
    {
        using FlowLayoutPanel control = new();
        control.SuspendLayout();
        using Control child1 = new()
        {
            Margin = Padding.Empty,
            Size = new Size(10, 20)
        };
        using Control child2 = new()
        {
            Margin = new Padding(1, 2, 3, 4),
            Size = new Size(10, 20)
        };
        using Control largeChild = new()
        {
            Margin = Padding.Empty,
            Size = new Size(100, 200)
        };
        using Control emptyChild = new()
        {
            Size = Size.Empty
        };
        using Control emptyChildWithMargin = new()
        {
            Margin = new Padding(1, 2, 3, 4),
            Size = Size.Empty
        };
        using Control child3 = new()
        {
            Margin = Padding.Empty,
            Size = new Size(50, 100)
        };
        using Control child4 = new()
        {
            Margin = Padding.Empty,
            Size = new Size(10, 20)
        };
        control.Controls.Add(child1);
        control.Controls.Add(child2);
        control.Controls.Add(largeChild);
        control.Controls.Add(emptyChild);
        control.Controls.Add(emptyChildWithMargin);
        control.Controls.Add(child3);
        control.Controls.Add(child4);

        LayoutEngine engine = control.LayoutEngine;
        engine.Layout(control, null);
        Assert.Equal(new Rectangle(0, 0, 10, 20), child1.Bounds);
        Assert.Equal(new Rectangle(11, 2, 10, 20), child2.Bounds);
        Assert.Equal(new Rectangle(24, 0, 100, 200), largeChild.Bounds);
        Assert.Equal(new Rectangle(127, 3, 0, 0), emptyChild.Bounds);
        Assert.Equal(new Rectangle(131, 2, 0, 0), emptyChildWithMargin.Bounds);
        Assert.Equal(new Rectangle(134, 0, 50, 100), child3.Bounds);
        Assert.Equal(new Rectangle(184, 0, 10, 20), child4.Bounds);
    }

    [WinFormsFact]
    public void LayoutEngine_Layout_ValidContainerRightToLeft_Success()
    {
        using FlowLayoutPanel control = new()
        {
            FlowDirection = FlowDirection.RightToLeft
        };
        control.SuspendLayout();
        using Control child1 = new()
        {
            Margin = Padding.Empty,
            Size = new Size(10, 20)
        };
        using Control child2 = new()
        {
            Margin = new Padding(1, 2, 3, 4),
            Size = new Size(10, 20)
        };
        using Control largeChild = new()
        {
            Margin = Padding.Empty,
            Size = new Size(100, 200)
        };
        using Control emptyChild = new()
        {
            Size = Size.Empty
        };
        using Control emptyChildWithMargin = new()
        {
            Margin = new Padding(1, 2, 3, 4),
            Size = Size.Empty
        };
        using Control child3 = new()
        {
            Margin = Padding.Empty,
            Size = new Size(50, 100)
        };
        using Control child4 = new()
        {
            Margin = Padding.Empty,
            Size = new Size(10, 20)
        };
        control.Controls.Add(child1);
        control.Controls.Add(child2);
        control.Controls.Add(largeChild);
        control.Controls.Add(emptyChild);
        control.Controls.Add(emptyChildWithMargin);
        control.Controls.Add(child3);
        control.Controls.Add(child4);

        LayoutEngine engine = control.LayoutEngine;
        engine.Layout(control, null);
        Assert.Equal(new Rectangle(190, 0, 10, 20), child1.Bounds);
        Assert.Equal(new Rectangle(177, 2, 10, 20), child2.Bounds);
        Assert.Equal(new Rectangle(76, 0, 100, 200), largeChild.Bounds);
        Assert.Equal(new Rectangle(73, 3, 0, 0), emptyChild.Bounds);
        Assert.Equal(new Rectangle(67, 2, 0, 0), emptyChildWithMargin.Bounds);
        Assert.Equal(new Rectangle(16, 0, 50, 100), child3.Bounds);
        Assert.Equal(new Rectangle(6, 0, 10, 20), child4.Bounds);
    }

    [WinFormsFact]
    public void LayoutEngine_Layout_ValidContainerTopDown_Success()
    {
        using FlowLayoutPanel control = new()
        {
            FlowDirection = FlowDirection.TopDown
        };
        control.SuspendLayout();
        using Control child1 = new()
        {
            Margin = Padding.Empty,
            Size = new Size(10, 20)
        };
        using Control child2 = new()
        {
            Margin = new Padding(1, 2, 3, 4),
            Size = new Size(10, 20)
        };
        using Control largeChild = new()
        {
            Margin = Padding.Empty,
            Size = new Size(100, 200)
        };
        using Control emptyChild = new()
        {
            Size = Size.Empty
        };
        using Control emptyChildWithMargin = new()
        {
            Margin = new Padding(1, 2, 3, 4),
            Size = Size.Empty
        };
        using Control child3 = new()
        {
            Margin = Padding.Empty,
            Size = new Size(50, 100)
        };
        using Control child4 = new()
        {
            Margin = Padding.Empty,
            Size = new Size(10, 20)
        };
        control.Controls.Add(child1);
        control.Controls.Add(child2);
        control.Controls.Add(largeChild);
        control.Controls.Add(emptyChild);
        control.Controls.Add(emptyChildWithMargin);
        control.Controls.Add(child3);
        control.Controls.Add(child4);

        LayoutEngine engine = control.LayoutEngine;
        engine.Layout(control, null);
        Assert.Equal(new Rectangle(0, 0, 10, 20), child1.Bounds);
        Assert.Equal(new Rectangle(1, 22, 10, 20), child2.Bounds);
        Assert.Equal(new Rectangle(14, 0, 100, 200), largeChild.Bounds);
        Assert.Equal(new Rectangle(117, 3, 0, 0), emptyChild.Bounds);
        Assert.Equal(new Rectangle(115, 8, 0, 0), emptyChildWithMargin.Bounds);
        Assert.Equal(new Rectangle(120, 0, 50, 100), child3.Bounds);
        Assert.Equal(new Rectangle(170, 0, 10, 20), child4.Bounds);
    }

    [WinFormsFact]
    public void LayoutEngine_Layout_ValidContainerBottomUp_Success()
    {
        using FlowLayoutPanel control = new()
        {
            FlowDirection = FlowDirection.BottomUp
        };
        control.SuspendLayout();
        using Control child1 = new()
        {
            Margin = Padding.Empty,
            Size = new Size(10, 20)
        };
        using Control child2 = new()
        {
            Margin = new Padding(1, 2, 3, 4),
            Size = new Size(10, 20)
        };
        using Control largeChild = new()
        {
            Margin = Padding.Empty,
            Size = new Size(100, 200)
        };
        using Control emptyChild = new()
        {
            Size = Size.Empty
        };
        using Control emptyChildWithMargin = new()
        {
            Margin = new Padding(1, 2, 3, 4),
            Size = Size.Empty
        };
        using Control child3 = new()
        {
            Margin = Padding.Empty,
            Size = new Size(50, 100)
        };
        using Control child4 = new()
        {
            Margin = Padding.Empty,
            Size = new Size(10, 20)
        };
        control.Controls.Add(child1);
        control.Controls.Add(child2);
        control.Controls.Add(largeChild);
        control.Controls.Add(emptyChild);
        control.Controls.Add(emptyChildWithMargin);
        control.Controls.Add(child3);
        control.Controls.Add(child4);

        LayoutEngine engine = control.LayoutEngine;
        engine.Layout(control, null);
        Assert.Equal(new Rectangle(0, 80, 10, 20), child1.Bounds);
        Assert.Equal(new Rectangle(1, 56, 10, 20), child2.Bounds);
        Assert.Equal(new Rectangle(14, -100, 100, 200), largeChild.Bounds);
        Assert.Equal(new Rectangle(117, 97, 0, 0), emptyChild.Bounds);
        Assert.Equal(new Rectangle(115, 90, 0, 0), emptyChildWithMargin.Bounds);
        Assert.Equal(new Rectangle(120, 0, 50, 100), child3.Bounds);
        Assert.Equal(new Rectangle(170, 80, 10, 20), child4.Bounds);
    }

    [WinFormsFact]
    public void LayoutEngine_Layout_InvalidContainer_Nop()
    {
        using FlowLayoutPanel control = new();
        LayoutEngine engine = control.LayoutEngine;
        Assert.Throws<NotSupportedException>(() => engine.Layout("container", new LayoutEventArgs(control, "affectedProperty")));
    }

    [WinFormsFact]
    public void LayoutEngine_Layout_NullContainer_ThrowsArgumentNullException()
    {
        using FlowLayoutPanel control = new();
        LayoutEngine engine = control.LayoutEngine;
        Assert.Throws<ArgumentNullException>("container", () => engine.Layout(null, new LayoutEventArgs(control, "affectedProperty")));
    }
}
