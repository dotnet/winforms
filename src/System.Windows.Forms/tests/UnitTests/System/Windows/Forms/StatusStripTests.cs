// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Tests;

public partial class StatusStripTests
{
    [WinFormsFact]
    public void StatusStrip_Ctor_Default()
    {
        using SubStatusStrip control = new();
        Assert.Null(control.AccessibleDefaultActionDescription);
        Assert.Null(control.AccessibleDescription);
        Assert.Null(control.AccessibleName);
        Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
        Assert.False(control.AllowDrop);
        Assert.False(control.AllowItemReorder);
        Assert.True(control.AllowMerge);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        Assert.False(control.AutoScroll);
        Assert.Equal(Size.Empty, control.AutoScrollMargin);
        Assert.Equal(Size.Empty, control.AutoScrollMinSize);
        Assert.Equal(Point.Empty, control.AutoScrollPosition);
        Assert.True(control.AutoSize);
        Assert.Equal(Control.DefaultBackColor, control.BackColor);
        Assert.Null(control.BackgroundImage);
        Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
        Assert.Null(control.BindingContext);
        Assert.Equal(22, control.Bottom);
        Assert.Equal(new Rectangle(0, 0, 200, 22), control.Bounds);
        Assert.True(control.CanEnableIme);
        Assert.False(control.CanFocus);
        Assert.False(control.CanOverflow);
        Assert.True(control.CanRaiseEvents);
        Assert.False(control.CanSelect);
        Assert.False(control.Capture);
        Assert.False(control.CausesValidation);
        Assert.Equal(new Rectangle(0, 0, 200, 22), control.ClientRectangle);
        Assert.Equal(new Size(200, 22), control.ClientSize);
        Assert.Null(control.Container);
        Assert.False(control.ContainsFocus);
        Assert.Null(control.ContextMenuStrip);
        Assert.Empty(control.Controls);
        Assert.Same(control.Controls, control.Controls);
        Assert.False(control.Created);
        Assert.Same(Cursors.Default, control.Cursor);
        Assert.Same(Cursors.Default, control.DefaultCursor);
        Assert.Equal(DockStyle.Bottom, control.DefaultDock);
        Assert.Equal(ToolStripDropDownDirection.AboveRight, control.DefaultDropDownDirection);
        Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
        Assert.Equal(new Padding(2, 2, 2, 2), control.DefaultGripMargin);
        Assert.Equal(Padding.Empty, control.DefaultMargin);
        Assert.Equal(Size.Empty, control.DefaultMaximumSize);
        Assert.Equal(Size.Empty, control.DefaultMinimumSize);
        Assert.Equal(new Padding(1, 0, 14, 0), control.DefaultPadding);
        Assert.Equal(new Size(200, 22), control.DefaultSize);
        Assert.False(control.DefaultShowItemToolTips);
        Assert.False(control.DesignMode);
        Assert.Empty(control.DisplayedItems);
        Assert.Same(control.DisplayedItems, control.DisplayedItems);
        Assert.Equal(new Rectangle(1, 0, 185, 22), control.DisplayRectangle);
        Assert.Equal(DockStyle.Bottom, control.Dock);
        Assert.NotNull(control.DockPadding);
        Assert.Same(control.DockPadding, control.DockPadding);
        Assert.Equal(0, control.DockPadding.Top);
        Assert.Equal(0, control.DockPadding.Bottom);
        Assert.Equal(1, control.DockPadding.Left);
        Assert.Equal(14, control.DockPadding.Right);
        Assert.True(control.DoubleBuffered);
        Assert.True(control.Enabled);
        Assert.NotNull(control.Events);
        Assert.Same(control.Events, control.Events);
        Assert.False(control.Focused);
        Assert.Equal(Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.Equal(Control.DefaultForeColor, control.ForeColor);
        Assert.Equal(ToolStripGripStyle.Hidden, control.GripStyle);
        Assert.Equal(ToolStripGripDisplayStyle.Horizontal, control.GripDisplayStyle);
        Assert.Equal(new Padding(2, 2, 2, 2), control.GripMargin);
        Assert.Equal(Rectangle.Empty, control.GripRectangle);
        Assert.False(control.HasChildren);
        Assert.Equal(22, control.Height);
        Assert.NotNull(control.HorizontalScroll);
        Assert.Same(control.HorizontalScroll, control.HorizontalScroll);
        Assert.False(control.HScroll);
        Assert.Null(control.ImageList);
        Assert.Equal(new Size(16, 16), control.ImageScalingSize);
        Assert.Equal(ImeMode.NoControl, control.ImeMode);
        Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
        Assert.False(control.IsAccessible);
        Assert.False(control.IsCurrentlyDragging);
        Assert.False(control.IsDropDown);
        Assert.False(control.IsMirrored);
        Assert.Empty(control.Items);
        Assert.Same(control.Items, control.Items);
        Assert.NotNull(control.LayoutEngine);
        Assert.Same(control.LayoutEngine, control.LayoutEngine);
        Assert.IsType<TableLayoutSettings>(control.LayoutSettings);
        Assert.Equal(ToolStripLayoutStyle.Table, control.LayoutStyle);
        Assert.Equal(0, control.Left);
        Assert.Equal(Point.Empty, control.Location);
        Assert.Equal(Padding.Empty, control.Margin);
        Assert.Equal(Size.Empty, control.MaximumSize);
        Assert.Equal(new Size(185, 22), control.MaxItemSize);
        Assert.Equal(Size.Empty, control.MinimumSize);
        Assert.Equal(Orientation.Horizontal, control.Orientation);
        Assert.NotNull(control.OverflowButton);
        Assert.Same(control.OverflowButton, control.OverflowButton);
        Assert.Same(control, control.OverflowButton.GetCurrentParent());
        Assert.Equal(new Padding(1, 0, 14, 0), control.Padding);
        Assert.Null(control.Parent);
        Assert.True(control.PreferredSize.Width > 0);
        Assert.True(control.PreferredSize.Height > 0);
        Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
        Assert.False(control.RecreatingHandle);
        Assert.Null(control.Region);
        Assert.NotNull(control.Renderer);
        Assert.Same(control.Renderer, control.Renderer);

        // TODO: Assume control.Renderer can be either ToolStripSystemRenderer or ToolStripProfessionalRenderer for the Moment.
        Assert.True(control.Renderer is ToolStripSystemRenderer or ToolStripProfessionalRenderer, "Renderer is not one of the expected types.");
        Assert.True(control.RenderMode is ToolStripRenderMode.System or ToolStripRenderMode.ManagerRenderMode);

        Assert.True(control.ResizeRedraw);
        Assert.Equal(200, control.Right);
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.True(control.ShowFocusCues);
        Assert.False(control.ShowItemToolTips);
        Assert.True(control.ShowKeyboardCues);
        Assert.Null(control.Site);
        Assert.Equal(new Size(200, 22), control.Size);
        Assert.True(control.SizingGrip);
        Assert.Equal(new Rectangle(188, 0, 12, 22), control.SizeGripBounds);
        Assert.True(control.Stretch);
        Assert.Equal(0, control.TabIndex);
        Assert.False(control.TabStop);
        Assert.Empty(control.Text);
        Assert.Equal(ToolStripTextDirection.Horizontal, control.TextDirection);
        Assert.Equal(0, control.Top);
        Assert.Null(control.TopLevelControl);
        Assert.False(control.UseWaitCursor);
        Assert.NotNull(control.VerticalScroll);
        Assert.Same(control.VerticalScroll, control.VerticalScroll);
        Assert.True(control.Visible);
        Assert.False(control.VScroll);
        Assert.Equal(200, control.Width);

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void StatusStrip_CreateParams_GetDefault_ReturnsExpected()
    {
        using SubStatusStrip control = new();
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Null(createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0x10000, createParams.ExStyle);
        Assert.Equal(22, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(0x56000000, createParams.Style);
        Assert.Equal(200, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void StatusStrip_CanOverflow_Set_GetReturnsExpected(bool value)
    {
        using StatusStrip control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.CanOverflow = value;
        Assert.Equal(value, control.CanOverflow);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.CanOverflow = value;
        Assert.Equal(value, control.CanOverflow);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.CanOverflow = !value;
        Assert.Equal(!value, control.CanOverflow);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> DefaultPadding_Get_TestData()
    {
        yield return new object[] { ToolStripLayoutStyle.Flow, RightToLeft.Yes, new Padding(14, 0, 1, 0) };
        yield return new object[] { ToolStripLayoutStyle.Flow, RightToLeft.No, new Padding(1, 0, 14, 0) };
        yield return new object[] { ToolStripLayoutStyle.Flow, RightToLeft.Inherit, new Padding(1, 0, 14, 0) };

        yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, RightToLeft.Yes, new Padding(14, 0, 1, 0) };
        yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, RightToLeft.No, new Padding(1, 0, 14, 0) };
        yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, RightToLeft.Inherit, new Padding(1, 0, 14, 0) };

        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, RightToLeft.Yes, new Padding(14, 0, 1, 0) };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, RightToLeft.No, new Padding(1, 0, 14, 0) };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, RightToLeft.Inherit, new Padding(1, 0, 14, 0) };

        yield return new object[] { ToolStripLayoutStyle.Table, RightToLeft.Yes, new Padding(14, 0, 1, 0) };
        yield return new object[] { ToolStripLayoutStyle.Table, RightToLeft.No, new Padding(1, 0, 14, 0) };
        yield return new object[] { ToolStripLayoutStyle.Table, RightToLeft.Inherit, new Padding(1, 0, 14, 0) };

        yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, RightToLeft.Yes, new Padding(1, 3, 1, 22) };
        yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, RightToLeft.No, new Padding(1, 3, 1, 22) };
        yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, RightToLeft.Inherit, new Padding(1, 3, 1, 22) };
    }

    [WinFormsTheory]
    [MemberData(nameof(DefaultPadding_Get_TestData))]
    public void StatusStrip_DefaultPadding_Get_ReturnsExpected(ToolStripLayoutStyle layoutStyle, RightToLeft rightToLeft, Padding expected)
    {
        using SubStatusStrip control = new()
        {
            LayoutStyle = layoutStyle,
            RightToLeft = rightToLeft
        };
        Assert.Equal(expected, control.DefaultPadding);
    }

    public static IEnumerable<object[]> Dock_Set_TestData()
    {
        yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Bottom, 0, Orientation.Horizontal, 0 };
        yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Bottom, 0, Orientation.Horizontal, 0 };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Bottom, 0, Orientation.Horizontal, 0 };
        yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Bottom, 0, Orientation.Horizontal, 0 };
        yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Bottom, 0, Orientation.Vertical, 0 };

        yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Fill, 1, Orientation.Horizontal, 1 };
        yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Fill, 1, Orientation.Horizontal, 0 };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Fill, 1, Orientation.Horizontal, 1 };
        yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Fill, 1, Orientation.Horizontal, 1 };
        yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Fill, 1, Orientation.Vertical, 0 };

        yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Left, 1, Orientation.Vertical, 1 };
        yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Left, 1, Orientation.Horizontal, 0 };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, 1, Orientation.Vertical, 1 };
        yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Left, 1, Orientation.Vertical, 1 };
        yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Left, 1, Orientation.Vertical, 0 };

        yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.None, 1, Orientation.Horizontal, 1 };
        yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.None, 1, Orientation.Horizontal, 0 };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.None, 1, Orientation.Horizontal, 1 };
        yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.None, 1, Orientation.Horizontal, 1 };
        yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.None, 1, Orientation.Vertical, 0 };

        yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Right, 1, Orientation.Vertical, 1 };
        yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Right, 1, Orientation.Horizontal, 0 };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Right, 1, Orientation.Vertical, 1 };
        yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Right, 1, Orientation.Vertical, 1 };
        yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Right, 1, Orientation.Vertical, 0 };

        yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Top, 1, Orientation.Horizontal, 1 };
        yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Top, 1, Orientation.Horizontal, 0 };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, 1, Orientation.Horizontal, 1 };
        yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Top, 1, Orientation.Horizontal, 1 };
        yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Top, 1, Orientation.Vertical, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Dock_Set_TestData))]
    public void StatusStrip_Dock_Set_GetReturnsExpected(ToolStripLayoutStyle layoutStyle, DockStyle value, int expectedLayoutCallCount, Orientation expectedOrientation, int expectedLayoutStyleChangedCallCount)
    {
        using StatusStrip control = new()
        {
            LayoutStyle = layoutStyle
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            if (e.AffectedProperty == "Dock")
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                layoutCallCount++;
            }
        };
        int layoutStyleChangedCallCount = 0;
        control.LayoutStyleChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            layoutStyleChangedCallCount++;
        };

        control.Dock = value;
        Assert.Equal(value, control.Dock);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedOrientation, control.Orientation);
        Assert.Equal(expectedLayoutStyleChangedCallCount, layoutStyleChangedCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Dock = value;
        Assert.Equal(value, control.Dock);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedOrientation, control.Orientation);
        Assert.Equal(expectedLayoutStyleChangedCallCount, layoutStyleChangedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(ToolStripGripStyle.Hidden, 0)]
    [InlineData(ToolStripGripStyle.Visible, 1)]
    public void StatusStrip_GripStyle_Set_GetReturnsExpected(ToolStripGripStyle value, int expectedLayoutCallCount)
    {
        using StatusStrip control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("GripStyle", e.AffectedProperty);
            layoutCallCount++;
        };

        control.GripStyle = value;
        Assert.Equal(value, control.GripStyle);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.GripStyle = value;
        Assert.Equal(value, control.GripStyle);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> LayoutStyle_Set_TestData()
    {
        foreach (DockStyle dock in new DockStyle[] { DockStyle.None, DockStyle.Top, DockStyle.Bottom })
        {
            yield return new object[] { dock, ToolStripLayoutStyle.Flow, ToolStripLayoutStyle.Flow, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 0 };
            yield return new object[] { dock, ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 0 };
            yield return new object[] { dock, ToolStripLayoutStyle.StackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 1 };
            yield return new object[] { dock, ToolStripLayoutStyle.Table, ToolStripLayoutStyle.Table, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 0 };
            yield return new object[] { dock, ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, Orientation.Vertical, ToolStripGripDisplayStyle.Horizontal, 1 };
        }

        foreach (DockStyle dock in new DockStyle[] { DockStyle.Right, DockStyle.Left })
        {
            yield return new object[] { dock, ToolStripLayoutStyle.Flow, ToolStripLayoutStyle.Flow, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 1 };
            yield return new object[] { dock, ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 1 };
            yield return new object[] { dock, ToolStripLayoutStyle.StackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, Orientation.Vertical, ToolStripGripDisplayStyle.Horizontal, 1 };
            yield return new object[] { dock, ToolStripLayoutStyle.Table, ToolStripLayoutStyle.Table, Orientation.Vertical, ToolStripGripDisplayStyle.Horizontal, 0 };
            yield return new object[] { dock, ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, Orientation.Vertical, ToolStripGripDisplayStyle.Horizontal, 0 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(LayoutStyle_Set_TestData))]
    public void StatusStrip_LayoutStyle_Set_GetReturnsExpected(DockStyle dock, ToolStripLayoutStyle value, ToolStripLayoutStyle expected, Orientation expectedOrientation, ToolStripGripDisplayStyle expectedGripDisplayStyle, int expectedLayoutCallCount)
    {
        using StatusStrip control = new()
        {
            Dock = dock
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.LayoutStyle = value;
        Assert.Equal(expected, control.LayoutStyle);
        Assert.Equal(expectedOrientation, control.Orientation);
        Assert.Equal(expectedGripDisplayStyle, control.GripDisplayStyle);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.LayoutStyle = value;
        Assert.Equal(expected, control.LayoutStyle);
        Assert.Equal(expectedOrientation, control.Orientation);
        Assert.Equal(expectedGripDisplayStyle, control.GripDisplayStyle);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Padding_Set_TestData()
    {
        yield return new object[] { new Padding(), new Padding(), 1, 1 };
        yield return new object[] { new Padding(1, 0, 14, 0), new Padding(1, 0, 14, 0), 0, 0 };
        yield return new object[] { new Padding(1, 2, 3, 4), new Padding(1, 2, 3, 4), 1, 1 };
        yield return new object[] { new Padding(1), new Padding(1), 1, 1 };
        yield return new object[] { new Padding(-1, -2, -3, -4), Padding.Empty, 1, 2 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Padding_Set_TestData))]
    public void StatusStrip_Padding_Set_GetReturnsExpected(Padding value, Padding expected, int expectedLayoutCallCount1, int expectedLayoutCallCount2)
    {
        using StatusStrip control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Padding", e.AffectedProperty);
            layoutCallCount++;
        };

        control.Padding = value;
        Assert.Equal(expected, control.Padding);
        Assert.Equal(expectedLayoutCallCount1, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Padding = value;
        Assert.Equal(expected, control.Padding);
        Assert.Equal(expectedLayoutCallCount2, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void StatusStrip_Padding_SetWithHandler_CallsPaddingChanged()
    {
        using StatusStrip control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Equal(control, sender);
            Assert.Equal(EventArgs.Empty, e);
            callCount++;
        };
        control.PaddingChanged += handler;

        // Set different.
        Padding padding1 = new(1);
        control.Padding = padding1;
        Assert.Equal(padding1, control.Padding);
        Assert.Equal(1, callCount);

        // Set same.
        control.Padding = padding1;
        Assert.Equal(padding1, control.Padding);
        Assert.Equal(1, callCount);

        // Set different.
        Padding padding2 = new(2);
        control.Padding = padding2;
        Assert.Equal(padding2, control.Padding);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.PaddingChanged -= handler;
        control.Padding = padding1;
        Assert.Equal(padding1, control.Padding);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [InlineData(ToolStripRenderMode.Professional, typeof(ToolStripProfessionalRenderer), 3)]
    [InlineData(ToolStripRenderMode.ManagerRenderMode, typeof(ToolStripProfessionalRenderer), 2)]
    public void StatusStrip_RenderMode_Set_ReturnsExpected(ToolStripRenderMode value, Type expectedRendererType, int expectedSameRendererChangedCallCount)
    {
        using StatusStrip control = new();
        int rendererChangedCallCount = 0;
        control.RendererChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            rendererChangedCallCount++;
        };

        // Set same.
        control.RenderMode = ToolStripRenderMode.System;
        Assert.Equal(ToolStripRenderMode.System, control.RenderMode);
        Assert.NotNull(control.Renderer);
        Assert.Same(control.Renderer, control.Renderer);
        Assert.IsType<ToolStripSystemRenderer>(control.Renderer);
        Assert.Equal(1, rendererChangedCallCount);

        // Set different.
        control.RenderMode = value;
        Assert.Equal(value, control.RenderMode);
        Assert.IsType(expectedRendererType, control.Renderer);
        Assert.Equal(2, rendererChangedCallCount);

        // Set same.
        control.RenderMode = value;
        Assert.Equal(value, control.RenderMode);
        Assert.IsType(expectedRendererType, control.Renderer);
        Assert.Equal(expectedSameRendererChangedCallCount, rendererChangedCallCount);

        // Set System.
        control.RenderMode = ToolStripRenderMode.System;
        Assert.Equal(ToolStripRenderMode.System, control.RenderMode);
        Assert.NotNull(control.Renderer);
        Assert.Same(control.Renderer, control.Renderer);
        Assert.IsType<ToolStripSystemRenderer>(control.Renderer);
        Assert.Equal(expectedSameRendererChangedCallCount + 1, rendererChangedCallCount);
    }

    [WinFormsTheory]
    [InlineData(ToolStripRenderMode.Professional, typeof(ToolStripProfessionalRenderer), 2)]
    [InlineData(ToolStripRenderMode.ManagerRenderMode, typeof(ToolStripProfessionalRenderer), 1)]
    public void StatusStrip_RenderMode_SetWithCustomRenderer_ReturnsExpected(ToolStripRenderMode value, Type expectedRendererType, int expectedSameRendererChangedCallCount)
    {
        using StatusStrip control = new()
        {
            Renderer = new SubToolStripRenderer()
        };
        int rendererChangedCallCount = 0;
        control.RendererChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            rendererChangedCallCount++;
        };

        control.RenderMode = value;
        Assert.Equal(value, control.RenderMode);
        Assert.IsType(expectedRendererType, control.Renderer);
        Assert.Equal(1, rendererChangedCallCount);

        // Set same.
        control.RenderMode = value;
        Assert.Equal(value, control.RenderMode);
        Assert.IsType(expectedRendererType, control.Renderer);
        Assert.Equal(expectedSameRendererChangedCallCount, rendererChangedCallCount);

        // Set System.
        control.RenderMode = ToolStripRenderMode.System;
        Assert.Equal(ToolStripRenderMode.System, control.RenderMode);
        Assert.NotNull(control.Renderer);
        Assert.Same(control.Renderer, control.Renderer);
        Assert.IsType<ToolStripSystemRenderer>(control.Renderer);
        Assert.Equal(expectedSameRendererChangedCallCount + 1, rendererChangedCallCount);
    }

    [WinFormsFact]
    public void StatusStrip_RenderMode_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(StatusStrip))[nameof(ToolStrip.RenderMode)];
        using StatusStrip control = new();
        Assert.False(property.CanResetValue(control));

        control.RenderMode = ToolStripRenderMode.Professional;
        Assert.Equal(ToolStripRenderMode.Professional, control.RenderMode);
        Assert.True(property.CanResetValue(control));

        control.RenderMode = ToolStripRenderMode.System;
        Assert.Equal(ToolStripRenderMode.System, control.RenderMode);
        Assert.False(property.CanResetValue(control));

        control.Renderer = new SubToolStripRenderer();
        Assert.Equal(ToolStripRenderMode.Custom, control.RenderMode);
        Assert.False(property.CanResetValue(control));

        control.RenderMode = ToolStripRenderMode.ManagerRenderMode;
        Assert.Equal(ToolStripRenderMode.ManagerRenderMode, control.RenderMode);
        Assert.True(property.CanResetValue(control));

        property.ResetValue(control);
        Assert.Equal(ToolStripRenderMode.System, control.RenderMode);
        Assert.False(property.CanResetValue(control));
    }

    [WinFormsFact]
    public void StatusStrip_RenderMode_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(StatusStrip))[nameof(ToolStrip.RenderMode)];
        using StatusStrip control = new();
        Assert.False(property.ShouldSerializeValue(control));

        control.RenderMode = ToolStripRenderMode.Professional;
        Assert.Equal(ToolStripRenderMode.Professional, control.RenderMode);
        Assert.True(property.ShouldSerializeValue(control));

        control.RenderMode = ToolStripRenderMode.System;
        Assert.Equal(ToolStripRenderMode.System, control.RenderMode);
        Assert.False(property.ShouldSerializeValue(control));

        control.Renderer = new SubToolStripRenderer();
        Assert.Equal(ToolStripRenderMode.Custom, control.RenderMode);
        Assert.False(property.ShouldSerializeValue(control));

        control.RenderMode = ToolStripRenderMode.ManagerRenderMode;
        Assert.Equal(ToolStripRenderMode.ManagerRenderMode, control.RenderMode);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.Equal(ToolStripRenderMode.System, control.RenderMode);
        Assert.False(property.ShouldSerializeValue(control));
    }

    [WinFormsTheory]
    [InvalidEnumData<ToolStripRenderMode>]
    public void StatusStrip_RenderMode_SetInvalidValue_ThrowsInvalidEnumArgumentException(ToolStripRenderMode value)
    {
        using StatusStrip control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.RenderMode = value);
    }

    [WinFormsFact]
    public void StatusStrip_RenderMode_SetCustomThrowsInvalidEnumArgumentException()
    {
        using StatusStrip control = new();
        Assert.Throws<NotSupportedException>(() => control.RenderMode = ToolStripRenderMode.Custom);
    }

    public static IEnumerable<object[]> SizeGripBounds_Get_TestData()
    {
        foreach (ToolStripLayoutStyle layoutStyle in Enum.GetValues(typeof(ToolStripLayoutStyle)))
        {
            yield return new object[] { true, layoutStyle, RightToLeft.Yes, new Rectangle(0, 0, 12, 22) };
            yield return new object[] { true, layoutStyle, RightToLeft.No, new Rectangle(188, 0, 12, 22) };
            yield return new object[] { true, layoutStyle, RightToLeft.Inherit, new Rectangle(188, 0, 12, 22) };
            yield return new object[] { false, layoutStyle, RightToLeft.Yes, Rectangle.Empty };
            yield return new object[] { false, layoutStyle, RightToLeft.No, Rectangle.Empty };
            yield return new object[] { false, layoutStyle, RightToLeft.Inherit, Rectangle.Empty };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(SizeGripBounds_Get_TestData))]
    public void StatusStrip_SizeGripBounds_Get_ReturnsExpected(bool sizingGrip, ToolStripLayoutStyle layoutStyle, RightToLeft rightToLeft, Rectangle expected)
    {
        using SubStatusStrip control = new()
        {
            SizingGrip = sizingGrip,
            LayoutStyle = layoutStyle,
            RightToLeft = rightToLeft
        };
        Assert.Equal(expected, control.SizeGripBounds);
    }

    public static IEnumerable<object[]> SizeGripBounds_GetLargeSize_TestData()
    {
        foreach (ToolStripLayoutStyle layoutStyle in Enum.GetValues(typeof(ToolStripLayoutStyle)))
        {
            yield return new object[] { true, layoutStyle, RightToLeft.Yes, new Rectangle(0, 10, 12, 22) };
            yield return new object[] { true, layoutStyle, RightToLeft.No, new Rectangle(198, 10, 12, 22) };
            yield return new object[] { true, layoutStyle, RightToLeft.Inherit, new Rectangle(198, 10, 12, 22) };
            yield return new object[] { false, layoutStyle, RightToLeft.Yes, Rectangle.Empty };
            yield return new object[] { false, layoutStyle, RightToLeft.No, Rectangle.Empty };
            yield return new object[] { false, layoutStyle, RightToLeft.Inherit, Rectangle.Empty };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(SizeGripBounds_GetLargeSize_TestData))]
    public void StatusStrip_SizeGripBounds_GetLargeSize_ReturnsExpected(bool sizingGrip, ToolStripLayoutStyle layoutStyle, RightToLeft rightToLeft, Rectangle expected)
    {
        using SubStatusStrip control = new()
        {
            SizingGrip = sizingGrip,
            LayoutStyle = layoutStyle,
            RightToLeft = rightToLeft,
            Size = new Size(210, 32)
        };
        Assert.Equal(expected, control.SizeGripBounds);
    }

    public static IEnumerable<object[]> SizeGripBounds_GetSmallSize_TestData()
    {
        foreach (ToolStripLayoutStyle layoutStyle in Enum.GetValues(typeof(ToolStripLayoutStyle)))
        {
            yield return new object[] { true, layoutStyle, RightToLeft.Yes, new Rectangle(0, 0, 12, 12) };
            yield return new object[] { true, layoutStyle, RightToLeft.No, new Rectangle(178, 0, 12, 12) };
            yield return new object[] { true, layoutStyle, RightToLeft.Inherit, new Rectangle(178, 0, 12, 12) };
            yield return new object[] { false, layoutStyle, RightToLeft.Yes, Rectangle.Empty };
            yield return new object[] { false, layoutStyle, RightToLeft.No, Rectangle.Empty };
            yield return new object[] { false, layoutStyle, RightToLeft.Inherit, Rectangle.Empty };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(SizeGripBounds_GetSmallSize_TestData))]
    public void StatusStrip_SizeGripBounds_GetSmallSize_ReturnsExpected(bool sizingGrip, ToolStripLayoutStyle layoutStyle, RightToLeft rightToLeft, Rectangle expected)
    {
        using SubStatusStrip control = new()
        {
            SizingGrip = sizingGrip,
            LayoutStyle = layoutStyle,
            RightToLeft = rightToLeft,
            Size = new Size(190, 12)
        };
        Assert.Equal(expected, control.SizeGripBounds);
    }

    [WinFormsTheory]
    [BoolData]
    public void StatusStrip_SizingGrip_Set_GetReturnsExpected(bool value)
    {
        using StatusStrip control = new()
        {
            SizingGrip = value
        };
        Assert.Equal(value, control.SizingGrip);
        Assert.Empty(control.Controls);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SizingGrip = value;
        Assert.Equal(value, control.SizingGrip);
        Assert.Empty(control.Controls);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.SizingGrip = !value;
        Assert.Equal(!value, control.SizingGrip);
        Assert.Empty(control.Controls);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> SizingGrip_SetRightToLeft_TestData()
    {
        yield return new object[] { RightToLeft.Yes, true, 1, 0 };
        yield return new object[] { RightToLeft.No, true, 0, 0 };
        yield return new object[] { RightToLeft.Inherit, true, 0, 0 };

        yield return new object[] { RightToLeft.Yes, false, 0, 1 };
        yield return new object[] { RightToLeft.No, false, 0, 0 };
        yield return new object[] { RightToLeft.Inherit, false, 0, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(SizingGrip_SetRightToLeft_TestData))]
    public void StatusStrip_SizingGrip_SetRightToLeft_GetReturnsExpected(RightToLeft rightToLeft, bool value, int expectedChildrenCallCount1, int expectedChildrenCallCount2)
    {
        using StatusStrip control = new()
        {
            RightToLeft = rightToLeft,
            SizingGrip = value
        };
        Assert.Equal(value, control.SizingGrip);
        Assert.Equal(expectedChildrenCallCount1, control.Controls.Count);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SizingGrip = value;
        Assert.Equal(value, control.SizingGrip);
        Assert.Equal(expectedChildrenCallCount1, control.Controls.Count);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.SizingGrip = !value;
        Assert.Equal(!value, control.SizingGrip);
        Assert.Equal(expectedChildrenCallCount2, control.Controls.Count);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> StatusStrip_SizingGrip_SetRightToLeftNonReadOnlyControls_TestData()
    {
        foreach (RightToLeft rightToLeft in Enum.GetValues(typeof(RightToLeft)))
        {
            yield return new object[] { rightToLeft, true };
            yield return new object[] { rightToLeft, false };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(StatusStrip_SizingGrip_SetRightToLeftNonReadOnlyControls_TestData))]
    public void StatusStrip_SizingGrip_SetRightToLeftNonReadOnlyControls_GetReturnsExpected(RightToLeft rightToLeft, bool value)
    {
        using NonReadOnlyControlsStatusStrip control = new()
        {
            RightToLeft = rightToLeft,
            SizingGrip = value
        };
        Assert.Equal(value, control.SizingGrip);
        Assert.Empty(control.Controls);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SizingGrip = value;
        Assert.Equal(value, control.SizingGrip);
        Assert.Empty(control.Controls);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.SizingGrip = !value;
        Assert.Equal(!value, control.SizingGrip);
        Assert.Empty(control.Controls);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 0)]
    [InlineData(false, 1)]
    public void StatusStrip_SizingGrip_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount)
    {
        using StatusStrip control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SizingGrip = value;
        Assert.Equal(value, control.SizingGrip);
        Assert.Empty(control.Controls);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.SizingGrip = value;
        Assert.Equal(value, control.SizingGrip);
        Assert.Empty(control.Controls);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.SizingGrip = !value;
        Assert.Equal(!value, control.SizingGrip);
        Assert.Empty(control.Controls);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void StatusStrip_ShowItemToolTips_Set_GetReturnsExpected(bool value)
    {
        using StatusStrip control = new()
        {
            ShowItemToolTips = value
        };
        Assert.Equal(value, control.ShowItemToolTips);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ShowItemToolTips = value;
        Assert.Equal(value, control.ShowItemToolTips);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.ShowItemToolTips = !value;
        Assert.Equal(!value, control.ShowItemToolTips);
        Assert.Equal(!value, control.OverflowButton.DropDown.ShowItemToolTips);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void StatusStrip_CreateAccessibilityInstance_Invoke_ReturnsExpected()
    {
        using SubStatusStrip control = new();
        Control.ControlAccessibleObject instance = Assert.IsAssignableFrom<Control.ControlAccessibleObject>(control.CreateAccessibilityInstance());
        Assert.NotNull(instance);
        Assert.NotSame(control.CreateAccessibilityInstance(), instance);
        Assert.NotSame(control.AccessibilityObject, instance);
        Assert.Same(control, instance.Owner);
        Assert.Equal(AccessibleRole.StatusBar, instance.Role);
    }

    public static IEnumerable<object[]> CreateDefaultItem_Button_TestData()
    {
        EventHandler onClick = (sender, e) => { };

        yield return new object[] { null, null, null };
        yield return new object[] { string.Empty, new Bitmap(10, 10), onClick };
        yield return new object[] { "text", new Bitmap(10, 10), onClick };
        yield return new object[] { "-", new Bitmap(10, 10), onClick };
    }

    [WinFormsTheory]
    [MemberData(nameof(CreateDefaultItem_Button_TestData))]
    public void StatusStrip_CreateDefaultItem_Invoke_Success(string text, Image image, EventHandler onClick)
    {
        using SubStatusStrip control = new();
        ToolStripStatusLabel button = Assert.IsType<ToolStripStatusLabel>(control.CreateDefaultItem(text, image, onClick));
        Assert.Equal(text, button.Text);
        Assert.Same(image, button.Image);
    }

    public static IEnumerable<object[]> Dispose_TestData()
    {
        foreach (RightToLeft rightToLeft in Enum.GetValues(typeof(RightToLeft)))
        {
            yield return new object[] { rightToLeft, true };
            yield return new object[] { rightToLeft, false };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Dispose_TestData))]
    public void StatusStrip_Dispose_Invoke_Success(RightToLeft rightToLeft, bool sizingGrip)
    {
        using StatusStrip control = new()
        {
            RightToLeft = rightToLeft,
            SizingGrip = sizingGrip
        };
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.Items);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.Disposing);
            Assert.Equal(callCount > 0, control.IsDisposed);
            callCount++;
        }

        control.Disposed += handler;

        try
        {
            control.Dispose();
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.Items);
            Assert.Empty(control.DataBindings);
            Assert.True(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose();
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.Items);
            Assert.Empty(control.DataBindings);
            Assert.True(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Equal(2, callCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Dispose_TestData))]
    public void StatusStrip_Dispose_InvokeDisposing_Success(RightToLeft rightToLeft, bool sizingGrip)
    {
        using SubStatusStrip control = new()
        {
            RightToLeft = rightToLeft,
            SizingGrip = sizingGrip
        };
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.Items);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.Disposing);
            Assert.Equal(callCount > 0, control.IsDisposed);
            callCount++;
        }

        control.Disposed += handler;

        try
        {
            control.Dispose(true);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.Items);
            Assert.Empty(control.DataBindings);
            Assert.True(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose(true);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.Items);
            Assert.Empty(control.DataBindings);
            Assert.True(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Equal(2, callCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    public static IEnumerable<object[]> Dispose_NotDisposing_TestData()
    {
        yield return new object[] { RightToLeft.Yes, true, 1 };
        yield return new object[] { RightToLeft.No, true, 0 };
        yield return new object[] { RightToLeft.Inherit, true, 0 };

        yield return new object[] { RightToLeft.Yes, false, 0 };
        yield return new object[] { RightToLeft.No, false, 0 };
        yield return new object[] { RightToLeft.Inherit, false, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Dispose_NotDisposing_TestData))]
    public void StatusStrip_Dispose_InvokeNotDisposing_Success(RightToLeft rightToLeft, bool sizingGrip, int expectedChildrenCallCount1)
    {
        using SubStatusStrip control = new()
        {
            RightToLeft = rightToLeft,
            SizingGrip = sizingGrip
        };
        int callCount = 0;
        void handler(object sender, EventArgs e) => callCount++;
        control.Disposed += handler;

        try
        {
            control.Dispose(false);
            Assert.Null(control.Parent);
            Assert.Equal(expectedChildrenCallCount1, control.Controls.Count);
            Assert.Empty(control.Items);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose(false);
            Assert.Null(control.Parent);
            Assert.Equal(expectedChildrenCallCount1, control.Controls.Count);
            Assert.Empty(control.Items);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("text")]
    [InlineData("-")]
    public void StatusStrip_CreateDefaultItem_PerformClick_Success(string text)
    {
        int callCount = 0;
        EventHandler onClick = (sender, e) => callCount++;
        using SubStatusStrip control = new();
        ToolStripItem button = Assert.IsAssignableFrom<ToolStripItem>(control.CreateDefaultItem(text, null, onClick));
        button.PerformClick();
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void StatusStrip_GetAutoSizeMode_Invoke_ReturnsExpected()
    {
        using SubStatusStrip control = new();
        Assert.Equal(AutoSizeMode.GrowAndShrink, control.GetAutoSizeMode());
    }

    [WinFormsTheory]
    [InlineData(0, true)]
    [InlineData(SubStatusStrip.ScrollStateAutoScrolling, false)]
    [InlineData(SubStatusStrip.ScrollStateFullDrag, false)]
    [InlineData(SubStatusStrip.ScrollStateHScrollVisible, false)]
    [InlineData(SubStatusStrip.ScrollStateUserHasScrolled, false)]
    [InlineData(SubStatusStrip.ScrollStateVScrollVisible, false)]
    [InlineData(int.MaxValue, false)]
    [InlineData((-1), false)]
    public void StatusStrip_GetScrollState_Invoke_ReturnsExpected(int bit, bool expected)
    {
        using SubStatusStrip control = new();
        Assert.Equal(expected, control.GetScrollState(bit));
    }

    [WinFormsTheory]
    [InlineData(ControlStyles.ContainerControl, true)]
    [InlineData(ControlStyles.UserPaint, true)]
    [InlineData(ControlStyles.Opaque, false)]
    [InlineData(ControlStyles.ResizeRedraw, true)]
    [InlineData(ControlStyles.FixedWidth, false)]
    [InlineData(ControlStyles.FixedHeight, false)]
    [InlineData(ControlStyles.StandardClick, true)]
    [InlineData(ControlStyles.Selectable, false)]
    [InlineData(ControlStyles.UserMouse, false)]
    [InlineData(ControlStyles.SupportsTransparentBackColor, true)]
    [InlineData(ControlStyles.StandardDoubleClick, true)]
    [InlineData(ControlStyles.AllPaintingInWmPaint, true)]
    [InlineData(ControlStyles.CacheText, false)]
    [InlineData(ControlStyles.EnableNotifyMessage, false)]
    [InlineData(ControlStyles.DoubleBuffer, false)]
    [InlineData(ControlStyles.OptimizedDoubleBuffer, true)]
    [InlineData(ControlStyles.UseTextForAccessibility, true)]
    [InlineData((ControlStyles)0, true)]
    [InlineData((ControlStyles)int.MaxValue, false)]
    [InlineData((ControlStyles)(-1), false)]
    public void StatusStrip_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
    {
        using SubStatusStrip control = new();
        Assert.Equal(expected, control.GetStyle(flag));

        // Call again to test caching.
        Assert.Equal(expected, control.GetStyle(flag));
    }

    public static IEnumerable<object[]> OnLayout_TestData()
    {
        foreach (ToolStripLayoutStyle layoutStyle in Enum.GetValues(typeof(ToolStripLayoutStyle)))
        {
            foreach (DockStyle dock in Enum.GetValues(typeof(DockStyle)))
            {
                yield return new object[] { layoutStyle, dock, new LayoutEventArgs(null, null) };
                yield return new object[] { layoutStyle, dock, new LayoutEventArgs(new Control(), "AffectedProperty") };
                yield return new object[] { layoutStyle, dock, new LayoutEventArgs(new SubToolStripItem(), "AffectedProperty") };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnLayout_TestData))]
    public void Control_OnLayout_Invoke_CallsLayout(ToolStripLayoutStyle layoutStyle, DockStyle dock, LayoutEventArgs eventArgs)
    {
        using SubStatusStrip control = new()
        {
            LayoutStyle = layoutStyle,
            Dock = dock
        };
        int callCount = 0;
        LayoutEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.Layout += handler;
        control.OnLayout(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.Layout -= handler;
        control.OnLayout(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void Control_OnLayout_Invoke_CreatesSizingGrip()
    {
        LayoutEventArgs eventArgs = new(null, null);

        using SubStatusStrip control = new()
        {
            RightToLeft = RightToLeft.Yes
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            if (e == eventArgs)
            {
                layoutCallCount++;
            }
        };
        control.OnLayout(eventArgs);
        Assert.Equal(1, layoutCallCount);
        Control grip = Assert.IsAssignableFrom<Control>(Assert.Single(control.Controls));
        Assert.Equal(control.SizeGripBounds, grip.Bounds);
        Assert.Equal(Color.Transparent, grip.BackColor);
    }

    [WinFormsFact]
    public void StatusStrip_OnLayout_NullE_ThrowsNullReferenceException()
    {
        using SubStatusStrip control = new();
        Assert.Throws<NullReferenceException>(() => control.OnLayout(null));
    }

    public static IEnumerable<object[]> OnPaintBackground_TestData()
    {
        foreach (RightToLeft rightToLeft in Enum.GetValues(typeof(RightToLeft)))
        {
            yield return new object[] { rightToLeft, true };
            yield return new object[] { rightToLeft, false };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnPaintBackground_TestData))]
    public void StatusStrip_OnPaintBackground_Invoke_Success(RightToLeft rightToLeft, bool sizingGrip)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using PaintEventArgs eventArgs = new(graphics, new Rectangle(1, 2, 3, 4));

        using SubStatusStrip control = new()
        {
            RightToLeft = rightToLeft,
            SizingGrip = sizingGrip
        };
        int callCount = 0;
        PaintEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.Paint += handler;
        control.OnPaintBackground(eventArgs);
        Assert.Equal(0, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.Paint -= handler;
        control.OnPaintBackground(eventArgs);
        Assert.Equal(0, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void StatusStrip_OnPaintBackground_NullE_ThrowsArgumentNullException()
    {
        using SubStatusStrip control = new();
        Assert.Throws<ArgumentNullException>(() => control.OnPaintBackground(null));
    }

    private class SubToolStripRenderer : ToolStripRenderer
    {
    }

    private class SubToolStripItem : ToolStripItem
    {
    }

    private class NonReadOnlyControlsStatusStrip : StatusStrip
    {
        protected override ControlCollection CreateControlsInstance() => new(this);
    }

    private class SubStatusStrip : StatusStrip
    {
        public new const int ScrollStateAutoScrolling = ScrollableControl.ScrollStateAutoScrolling;

        public new const int ScrollStateHScrollVisible = ScrollableControl.ScrollStateHScrollVisible;

        public new const int ScrollStateVScrollVisible = ScrollableControl.ScrollStateVScrollVisible;

        public new const int ScrollStateUserHasScrolled = ScrollableControl.ScrollStateUserHasScrolled;

        public new const int ScrollStateFullDrag = ScrollableControl.ScrollStateFullDrag;

        public new bool CanEnableIme => base.CanEnableIme;

        public new bool CanRaiseEvents => base.CanRaiseEvents;

        public new CreateParams CreateParams => base.CreateParams;

        public new Cursor DefaultCursor => base.DefaultCursor;

        public new DockStyle DefaultDock => base.DefaultDock;

        public new Padding DefaultGripMargin => base.DefaultGripMargin;

        public new ImeMode DefaultImeMode => base.DefaultImeMode;

        public new Padding DefaultMargin => base.DefaultMargin;

        public new Size DefaultMaximumSize => base.DefaultMaximumSize;

        public new Size DefaultMinimumSize => base.DefaultMinimumSize;

        public new Padding DefaultPadding => base.DefaultPadding;

        public new Size DefaultSize => base.DefaultSize;

        public new bool DefaultShowItemToolTips => base.DefaultShowItemToolTips;

        public new bool DesignMode => base.DesignMode;

        public new ToolStripItemCollection DisplayedItems => base.DisplayedItems;

        public new bool DoubleBuffered
        {
            get => base.DoubleBuffered;
            set => base.DoubleBuffered = value;
        }

        public new EventHandlerList Events => base.Events;

        public new int FontHeight
        {
            get => base.FontHeight;
            set => base.FontHeight = value;
        }

        public new ImeMode ImeModeBase
        {
            get => base.ImeModeBase;
            set => base.ImeModeBase = value;
        }

        public new bool HScroll
        {
            get => base.HScroll;
            set => base.HScroll = value;
        }

        public new Size MaxItemSize => base.MaxItemSize;

        public new bool ResizeRedraw
        {
            get => base.ResizeRedraw;
            set => base.ResizeRedraw = value;
        }

        public new bool ShowFocusCues => base.ShowFocusCues;

        public new bool ShowKeyboardCues => base.ShowKeyboardCues;

        public new bool VScroll
        {
            get => base.VScroll;
            set => base.VScroll = value;
        }

        public new AccessibleObject CreateAccessibilityInstance() => base.CreateAccessibilityInstance();

        public new ToolStripItem CreateDefaultItem(string text, Image image, EventHandler onClick) => base.CreateDefaultItem(text, image, onClick);

        public new void Dispose(bool disposing) => base.Dispose(disposing);

        public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

        public new bool GetScrollState(int bit) => base.GetScrollState(bit);

        public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

        public new void OnLayout(LayoutEventArgs levent) => base.OnLayout(levent);

        public new void OnPaintBackground(PaintEventArgs e) => base.OnPaintBackground(e);

        public new void OnSpringTableLayoutCore() => base.OnSpringTableLayoutCore();

        public new void SetDisplayedItems() => base.SetDisplayedItems();

        public new void WndProc(ref Message m) => base.WndProc(ref m);
    }
}
