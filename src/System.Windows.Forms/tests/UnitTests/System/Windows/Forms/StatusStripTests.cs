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
        control.AccessibleDefaultActionDescription.Should().BeNull();
        control.AccessibleDescription.Should().BeNull();
        control.AccessibleName.Should().BeNull();
        control.AccessibleRole.Should().Be(AccessibleRole.Default);
        control.AllowDrop.Should().BeFalse();
        control.AllowItemReorder.Should().BeFalse();
        control.AllowMerge.Should().BeTrue();
        control.Anchor.Should().Be(AnchorStyles.Top | AnchorStyles.Left);
        control.AutoScroll.Should().BeFalse();
        control.AutoScrollMargin.Should().Be(Size.Empty);
        control.AutoScrollMinSize.Should().Be(Size.Empty);
        control.AutoScrollPosition.Should().Be(Point.Empty);
        control.AutoSize.Should().BeTrue();
        control.BackColor.Should().Be(Control.DefaultBackColor);
        control.BackgroundImage.Should().BeNull();
        control.BackgroundImageLayout.Should().Be(ImageLayout.Tile);
        control.BindingContext.Should().BeNull();
        control.Bottom.Should().Be(22);
        control.Bounds.Should().Be(new Rectangle(0, 0, 200, 22));
        control.CanEnableIme.Should().BeTrue();
        control.CanFocus.Should().BeFalse();
        control.CanOverflow.Should().BeFalse();
        control.CanRaiseEvents.Should().BeTrue();
        control.CanSelect.Should().BeFalse();
        control.Capture.Should().BeFalse();
        control.CausesValidation.Should().BeFalse();
        control.ClientRectangle.Should().Be(new Rectangle(0, 0, 200, 22));
        control.ClientSize.Should().Be(new Size(200, 22));
        control.Container.Should().BeNull();
        control.ContainsFocus.Should().BeFalse();
        control.ContextMenuStrip.Should().BeNull();
        control.Controls.Count.Should().Be(0);
        control.Created.Should().BeFalse();
        control.Cursor.Should().Be(Cursors.Default);
        control.DefaultCursor.Should().Be(Cursors.Default);
        control.DefaultDock.Should().Be(DockStyle.Bottom);
        control.DefaultDropDownDirection.Should().Be(ToolStripDropDownDirection.AboveRight);
        control.DefaultImeMode.Should().Be(ImeMode.Inherit);
        control.DefaultGripMargin.Should().Be(new Padding(2, 2, 2, 2));
        control.DefaultMargin.Should().Be(Padding.Empty);
        control.DefaultMaximumSize.Should().Be(Size.Empty);
        control.DefaultMinimumSize.Should().Be(Size.Empty);
        control.DefaultPadding.Should().Be(new Padding(1, 0, 14, 0));
        control.DefaultSize.Should().Be(new Size(200, 22));
        control.DefaultShowItemToolTips.Should().BeFalse();
        control.DesignMode.Should().BeFalse();
        control.DisplayedItems.Count.Should().Be(0);
        control.DisplayRectangle.Should().Be(new Rectangle(1, 0, 185, 22));
        control.Dock.Should().Be(DockStyle.Bottom);
        control.DockPadding.Should().NotBeNull();
        control.DockPadding.Top.Should().Be(0);
        control.DockPadding.Bottom.Should().Be(0);
        control.DockPadding.Left.Should().Be(1);
        control.DockPadding.Right.Should().Be(14);
        control.DoubleBuffered.Should().BeTrue();
        control.Enabled.Should().BeTrue();
        control.Events.Should().NotBeNull();
        control.Focused.Should().BeFalse();
        control.Font.Should().Be(Control.DefaultFont);
        control.FontHeight.Should().Be(control.Font.Height);
        control.ForeColor.Should().Be(Control.DefaultForeColor);
        control.GripStyle.Should().Be(ToolStripGripStyle.Hidden);
        control.GripDisplayStyle.Should().Be(ToolStripGripDisplayStyle.Horizontal);
        control.GripMargin.Should().Be(new Padding(2, 2, 2, 2));
        control.GripRectangle.Should().Be(Rectangle.Empty);
        control.HasChildren.Should().BeFalse();
        control.Height.Should().Be(22);
        control.HorizontalScroll.Should().NotBeNull();
        control.HorizontalScroll.Should().Be(control.HorizontalScroll);
        control.HScroll.Should().BeFalse();
        control.ImageList.Should().BeNull();
        control.ImageScalingSize.Should().Be(new Size(16, 16));
        control.ImeMode.Should().Be(ImeMode.NoControl);
        control.ImeModeBase.Should().Be(ImeMode.NoControl);
        control.IsAccessible.Should().BeFalse();
        control.IsCurrentlyDragging.Should().BeFalse();
        control.IsDropDown.Should().BeFalse();
        control.IsMirrored.Should().BeFalse();
        control.Items.Count.Should().Be(0);
        control.LayoutEngine.Should().NotBeNull();
        control.LayoutEngine.Should().Be(control.LayoutEngine);
        control.LayoutSettings.Should().BeOfType<TableLayoutSettings>();
        control.LayoutStyle.Should().Be(ToolStripLayoutStyle.Table);
        control.Left.Should().Be(0);
        control.Location.Should().Be(Point.Empty);
        control.Margin.Should().Be(Padding.Empty);
        control.MaximumSize.Should().Be(Size.Empty);
        control.MaxItemSize.Should().Be(new Size(185, 22));
        control.MinimumSize.Should().Be(Size.Empty);
        control.Orientation.Should().Be(Orientation.Horizontal);
        control.OverflowButton.Should().NotBeNull();
        control.OverflowButton.Should().Be(control.OverflowButton);
        control.OverflowButton.GetCurrentParent().Should().BeSameAs(control);
        control.Padding.Should().Be(new Padding(1, 0, 14, 0));
        control.Parent.Should().BeNull();
        control.PreferredSize.Width.Should().BeGreaterThan(0);
        control.PreferredSize.Height.Should().BeGreaterThan(0);
        control.ProductName.Should().Be("Microsoft® .NET");
        control.RecreatingHandle.Should().BeFalse();
        control.Region.Should().BeNull();
        control.Renderer.Should().NotBeNull();
        control.Renderer.Should().BeSameAs(control.Renderer);

        control.Renderer.Should().Match<ToolStripRenderer>(r => r is ToolStripSystemRenderer || r is ToolStripProfessionalRenderer,
            "because the renderer should be one of the expected types");
        control.RenderMode.Should().BeOneOf(ToolStripRenderMode.System, ToolStripRenderMode.ManagerRenderMode);

        control.ResizeRedraw.Should().BeTrue();
        control.Right.Should().Be(200);
        control.RightToLeft.Should().Be(RightToLeft.No);
        control.ShowFocusCues.Should().BeTrue();
        control.ShowItemToolTips.Should().BeFalse();
        control.ShowKeyboardCues.Should().BeTrue();
        control.Site.Should().BeNull();
        control.Size.Should().Be(new Size(200, 22));
        control.SizingGrip.Should().BeTrue();
        control.SizeGripBounds.Should().Be(new Rectangle(188, 0, 12, 22));
        control.Stretch.Should().BeTrue();
        control.TabIndex.Should().Be(0);
        control.TabStop.Should().BeFalse();
        control.Text.Should().BeEmpty();
        control.TextDirection.Should().Be(ToolStripTextDirection.Horizontal);
        control.Top.Should().Be(0);
        control.TopLevelControl.Should().BeNull();
        control.UseWaitCursor.Should().BeFalse();
        control.VerticalScroll.Should().NotBeNull();
        control.VerticalScroll.Should().Be(control.VerticalScroll);
        control.Visible.Should().BeTrue();
        control.VScroll.Should().BeFalse();
        control.Width.Should().Be(200);

        control.IsHandleCreated.Should().BeFalse();
    }

    [WinFormsFact]
    public void StatusStrip_CreateParams_GetDefault_ReturnsExpected()
    {
        using SubStatusStrip control = new();
        CreateParams createParams = control.CreateParams;
        createParams.Caption.Should().BeNull();
        createParams.ClassName.Should().BeNull();
        createParams.ClassStyle.Should().Be(0x8);
        createParams.ExStyle.Should().Be(0x10000);
        createParams.Height.Should().Be(22);
        createParams.Parent.Should().Be(IntPtr.Zero);
        createParams.Param.Should().BeNull();
        createParams.Style.Should().Be(0x56000000);
        createParams.Width.Should().Be(200);
        createParams.X.Should().Be(0);
        createParams.Y.Should().Be(0);
        control.CreateParams.Should().BeSameAs(createParams);
        control.IsHandleCreated.Should().BeFalse();
    }

    [WinFormsFact]
    public void StatusStrip_Use_SystemRenderMode_As_Default()
    {
        using UseSystemRenderingModeAsDefaultScope scope = new(enable: true);
        using SubStatusStrip control = new();
        control.RenderMode.Should().Be(ToolStripRenderMode.System);
    }

    [WinFormsTheory]
    [BoolData]
    public void StatusStrip_CanOverflow_Set_GetReturnsExpected(bool value)
    {
        using StatusStrip control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.CanOverflow = value;
        control.CanOverflow.Should().Be(value);

        layoutCallCount.Should().Be(0);
        control.IsHandleCreated.Should().BeFalse();

        // Set same.
        control.CanOverflow = value;
        control.CanOverflow.Should().Be(value);
        layoutCallCount.Should().Be(0);
        control.IsHandleCreated.Should().BeFalse();

        // Set different.
        control.CanOverflow = !value;
        control.CanOverflow.Should().Be(!value);
        layoutCallCount.Should().Be(0);
        control.IsHandleCreated.Should().BeFalse();
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

        control.DefaultPadding.Should().Be(expected);
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
                sender.Should().BeSameAs(control);
                e.AffectedControl.Should().BeSameAs(control);
                layoutCallCount++;
            }
        };

        int layoutStyleChangedCallCount = 0;
        control.LayoutStyleChanged += (sender, e) =>
        {
            sender.Should().BeSameAs(control);
            e.Should().BeSameAs(EventArgs.Empty);
            layoutStyleChangedCallCount++;
        };

        control.Dock = value;
        control.Dock.Should().Be(value);
        control.Anchor.Should().Be(AnchorStyles.Top | AnchorStyles.Left);
        layoutCallCount.Should().Be(expectedLayoutCallCount);
        control.Orientation.Should().Be(expectedOrientation);
        layoutStyleChangedCallCount.Should().Be(expectedLayoutStyleChangedCallCount);
        control.IsHandleCreated.Should().BeFalse();

        // Set same.
        control.Dock = value;
        control.Dock.Should().Be(value);
        control.Anchor.Should().Be(AnchorStyles.Top | AnchorStyles.Left);
        layoutCallCount.Should().Be(expectedLayoutCallCount);
        control.Orientation.Should().Be(expectedOrientation);
        layoutStyleChangedCallCount.Should().Be(expectedLayoutStyleChangedCallCount);
        control.IsHandleCreated.Should().BeFalse();
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
            sender.Should().BeSameAs(control);
            e.AffectedControl.Should().BeSameAs(control);
            e.AffectedProperty.Should().Be("GripStyle");
            layoutCallCount++;
        };

        control.GripStyle = value;
        control.GripStyle.Should().Be(value);
        layoutCallCount.Should().Be(expectedLayoutCallCount);
        control.IsHandleCreated.Should().BeFalse();

        // Set same.
        control.GripStyle = value;
        control.GripStyle.Should().Be(value);
        layoutCallCount.Should().Be(expectedLayoutCallCount);
        control.IsHandleCreated.Should().BeFalse();
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
        control.LayoutStyle.Should().Be(expected);
        control.Orientation.Should().Be(expectedOrientation);
        control.GripDisplayStyle.Should().Be(expectedGripDisplayStyle);
        layoutCallCount.Should().Be(expectedLayoutCallCount);
        control.IsHandleCreated.Should().BeFalse();

        // Set same.
        control.LayoutStyle = value;
        control.LayoutStyle.Should().Be(expected);
        control.Orientation.Should().Be(expectedOrientation);
        control.GripDisplayStyle.Should().Be(expectedGripDisplayStyle);
        layoutCallCount.Should().Be(expectedLayoutCallCount);
        control.IsHandleCreated.Should().BeFalse();
    }

    public static IEnumerable<object[]> Padding_Set_TestData()
    {
        yield return new object[] { default(Padding), default(Padding), 1, 1 };
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
            sender.Should().BeSameAs(control);
            e.AffectedControl.Should().BeSameAs(control);
            e.AffectedProperty.Should().Be("Padding");
            layoutCallCount++;
        };

        control.Padding = value;
        control.Padding.Should().Be(expected);
        layoutCallCount.Should().Be(expectedLayoutCallCount1);
        control.IsHandleCreated.Should().BeFalse();

        // Set same.
        control.Padding = value;
        control.Padding.Should().Be(expected);
        layoutCallCount.Should().Be(expectedLayoutCallCount2);
        control.IsHandleCreated.Should().BeFalse();
    }

    [WinFormsFact]
    public void StatusStrip_Padding_SetWithHandler_CallsPaddingChanged()
    {
        using StatusStrip control = new();
        int callCount = 0;

        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(control);
            e.Should().Be(EventArgs.Empty);
            callCount++;
        };
        control.PaddingChanged += handler;

        // Set different.
        Padding padding1 = new(1);
        control.Padding = padding1;
        control.Padding.Should().Be(padding1);
        callCount.Should().Be(1);

        // Set same.
        control.Padding = padding1;
        control.Padding.Should().Be(padding1);
        callCount.Should().Be(1);

        // Set different.
        Padding padding2 = new(2);
        control.Padding = padding2;
        control.Padding.Should().Be(padding2);
        callCount.Should().Be(2);

        // Remove handler.
        control.PaddingChanged -= handler;
        control.Padding = padding1;
        control.Padding.Should().Be(padding1);
        callCount.Should().Be(2);
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
            sender.Should().BeSameAs(control);
            e.Should().BeSameAs(EventArgs.Empty);
            rendererChangedCallCount++;
        };

        // Set same.
        control.RenderMode = ToolStripRenderMode.System;
        control.RenderMode.Should().Be(ToolStripRenderMode.System);
        control.Renderer.Should().NotBeNull();
        control.Renderer.Should().BeSameAs(control.Renderer);
        control.Renderer.Should().BeOfType<ToolStripSystemRenderer>();
        rendererChangedCallCount.Should().Be(1);

        // Set different.
        control.RenderMode = value;
        control.RenderMode.Should().Be(value);
        control.Renderer.Should().BeOfType(expectedRendererType);
        rendererChangedCallCount.Should().Be(2);

        // Set same.
        control.RenderMode = value;
        control.RenderMode.Should().Be(value);
        control.Renderer.Should().BeOfType(expectedRendererType);
        rendererChangedCallCount.Should().Be(expectedSameRendererChangedCallCount);

        // Set System.
        control.RenderMode = ToolStripRenderMode.System;
        control.RenderMode.Should().Be(ToolStripRenderMode.System);
        control.Renderer.Should().NotBeNull();
        control.Renderer.Should().BeSameAs(control.Renderer);
        control.Renderer.Should().BeOfType<ToolStripSystemRenderer>();
        rendererChangedCallCount.Should().Be(expectedSameRendererChangedCallCount + 1);
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
            sender.Should().BeSameAs(control);
            e.Should().BeSameAs(EventArgs.Empty);
            rendererChangedCallCount++;
        };

        control.RenderMode = value;
        control.RenderMode.Should().Be(value);
        control.Renderer.Should().BeOfType(expectedRendererType);
        rendererChangedCallCount.Should().Be(1);

        // Set same.
        control.RenderMode = value;
        control.RenderMode.Should().Be(value);
        control.Renderer.Should().BeOfType(expectedRendererType);
        rendererChangedCallCount.Should().Be(expectedSameRendererChangedCallCount);

        // Set System.
        control.RenderMode = ToolStripRenderMode.System;
        control.RenderMode.Should().Be(ToolStripRenderMode.System);
        control.Renderer.Should().NotBeNull();
        control.Renderer.Should().BeSameAs(control.Renderer);
        control.Renderer.Should().BeOfType<ToolStripSystemRenderer>();
        rendererChangedCallCount.Should().Be(expectedSameRendererChangedCallCount + 1);
    }

    [WinFormsFact]
    public void StatusStrip_RenderMode_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(StatusStrip))[nameof(ToolStrip.RenderMode)];
        using StatusStrip control = new();
        property.CanResetValue(control).Should().BeFalse();

        control.RenderMode = ToolStripRenderMode.Professional;
        control.RenderMode.Should().Be(ToolStripRenderMode.Professional);
        property.CanResetValue(control).Should().BeTrue();

        control.RenderMode = ToolStripRenderMode.System;
        control.RenderMode.Should().Be(ToolStripRenderMode.System);
        property.CanResetValue(control).Should().BeTrue();

        control.Renderer = new SubToolStripRenderer();
        control.RenderMode.Should().Be(ToolStripRenderMode.Custom);
        property.CanResetValue(control).Should().BeFalse();

        control.RenderMode = ToolStripRenderMode.ManagerRenderMode;
        control.RenderMode.Should().Be(ToolStripRenderMode.ManagerRenderMode);
        property.CanResetValue(control).Should().BeFalse();

        property.ResetValue(control);
        control.RenderMode.Should().Be(ToolStripRenderMode.ManagerRenderMode);
        property.CanResetValue(control).Should().BeFalse();

        using UseSystemRenderingModeAsDefaultScope scope = new(enable: true);
        property.ResetValue(control);
        control.RenderMode.Should().Be(ToolStripRenderMode.System);
        property.CanResetValue(control).Should().BeFalse();
    }

    [WinFormsFact]
    public void StatusStrip_RenderMode_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(StatusStrip))[nameof(ToolStrip.RenderMode)];
        using StatusStrip control = new();

        property.ShouldSerializeValue(control).Should().BeFalse();

        control.RenderMode = ToolStripRenderMode.Professional;
        control.RenderMode.Should().Be(ToolStripRenderMode.Professional);
        property.ShouldSerializeValue(control).Should().BeTrue();

        control.RenderMode = ToolStripRenderMode.System;
        control.RenderMode.Should().Be(ToolStripRenderMode.System);
        property.ShouldSerializeValue(control).Should().BeTrue();

        control.Renderer = new SubToolStripRenderer();
        control.RenderMode.Should().Be(ToolStripRenderMode.Custom);
        property.ShouldSerializeValue(control).Should().BeFalse();

        control.RenderMode = ToolStripRenderMode.ManagerRenderMode;
        control.RenderMode.Should().Be(ToolStripRenderMode.ManagerRenderMode);
        property.ShouldSerializeValue(control).Should().BeFalse();

        property.ResetValue(control);
        control.RenderMode.Should().Be(ToolStripRenderMode.ManagerRenderMode);
        property.ShouldSerializeValue(control).Should().BeFalse();

        using UseSystemRenderingModeAsDefaultScope scope = new(enable: true);
        property.ResetValue(control);
        control.RenderMode.Should().Be(ToolStripRenderMode.System);
        property.ShouldSerializeValue(control).Should().BeFalse();
    }

    [WinFormsTheory]
    [InvalidEnumData<ToolStripRenderMode>]
    public void StatusStrip_RenderMode_SetInvalidValue_ThrowsInvalidEnumArgumentException(ToolStripRenderMode value)
    {
        using StatusStrip control = new();
        control.Invoking(c => c.RenderMode = value)
            .Should().Throw<InvalidEnumArgumentException>()
            .WithParameterName("value");
    }

    [WinFormsFact]
    public void StatusStrip_RenderMode_SetCustomThrowsInvalidEnumArgumentException()
    {
        using StatusStrip control = new();
        control.Invoking(c => c.RenderMode = ToolStripRenderMode.Custom)
           .Should().Throw<NotSupportedException>();
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

        control.SizeGripBounds.Should().Be(expected);
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

        control.SizeGripBounds.Should().Be(expected);
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

        control.SizeGripBounds.Should().Be(expected);
    }

    [WinFormsTheory]
    [BoolData]
    public void StatusStrip_SizingGrip_Set_GetReturnsExpected(bool value)
    {
        using StatusStrip control = new()
        {
            SizingGrip = value
        };

        control.SizingGrip.Should().Be(value);
        control.Controls.Count.Should().Be(0);
        control.IsHandleCreated.Should().BeFalse();

        // Set same.
        control.SizingGrip = value;
        control.SizingGrip.Should().Be(value);
        control.Controls.Count.Should().Be(0);
        control.IsHandleCreated.Should().BeFalse();

        // Set different.
        control.SizingGrip = !value;
        control.SizingGrip.Should().Be(!value);
        control.Controls.Count.Should().Be(0);
        control.IsHandleCreated.Should().BeFalse();
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

        control.SizingGrip.Should().Be(value);
        control.Controls.Count.Should().Be(expectedChildrenCallCount1);
        control.IsHandleCreated.Should().BeFalse();

        // Set same.
        control.SizingGrip = value;
        control.SizingGrip.Should().Be(value);
        control.Controls.Count.Should().Be(expectedChildrenCallCount1);
        control.IsHandleCreated.Should().BeFalse();

        // Set different.
        control.SizingGrip = !value;
        control.SizingGrip.Should().Be(!value);
        control.Controls.Count.Should().Be(expectedChildrenCallCount2);
        control.IsHandleCreated.Should().BeFalse();
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

        control.SizingGrip.Should().Be(value);
        control.Controls.Count.Should().Be(0);
        control.IsHandleCreated.Should().BeFalse();

        // Set same.
        control.SizingGrip = value;
        control.SizingGrip.Should().Be(value);
        control.Controls.Count.Should().Be(0);
        control.IsHandleCreated.Should().BeFalse();

        // Set different.
        control.SizingGrip = !value;
        control.SizingGrip.Should().Be(!value);
        control.Controls.Count.Should().Be(0);
        control.IsHandleCreated.Should().BeFalse();
    }

    [WinFormsTheory]
    [InlineData(true, 0)]
    [InlineData(false, 1)]
    public void StatusStrip_SizingGrip_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount)
    {
        using StatusStrip control = new();
        control.Handle.Should().NotBe(IntPtr.Zero);

        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SizingGrip = value;
        control.SizingGrip.Should().Be(value);
        control.Controls.Count.Should().Be(0);
        control.IsHandleCreated.Should().BeTrue();
        invalidatedCallCount.Should().Be(expectedInvalidatedCallCount);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);

        // Set same.
        control.SizingGrip = value;
        control.SizingGrip.Should().Be(value);
        control.Controls.Count.Should().Be(0);
        control.IsHandleCreated.Should().BeTrue();
        invalidatedCallCount.Should().Be(expectedInvalidatedCallCount);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);

        // Set different.
        control.SizingGrip = !value;
        control.SizingGrip.Should().Be(!value);
        control.Controls.Count.Should().Be(0);
        control.IsHandleCreated.Should().BeTrue();
        invalidatedCallCount.Should().Be(expectedInvalidatedCallCount + 1);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);
    }

    [WinFormsTheory]
    [BoolData]
    public void StatusStrip_ShowItemToolTips_Set_GetReturnsExpected(bool value)
    {
        using StatusStrip control = new()
        {
            ShowItemToolTips = value
        };

        control.ShowItemToolTips.Should().Be(value);
        control.IsHandleCreated.Should().BeFalse();

        // Set same.
        control.ShowItemToolTips = value;
        control.ShowItemToolTips.Should().Be(value);
        control.IsHandleCreated.Should().BeFalse();

        // Set different.
        control.ShowItemToolTips = !value;
        control.ShowItemToolTips.Should().Be(!value);
        control.OverflowButton.DropDown.ShowItemToolTips.Should().Be(!value);
        control.IsHandleCreated.Should().BeFalse();
    }

    [WinFormsFact]
    public void StatusStrip_CreateAccessibilityInstance_Invoke_ReturnsExpected()
    {
        using SubStatusStrip control = new();
        Control.ControlAccessibleObject instance = control.CreateAccessibilityInstance().Should().BeAssignableTo<Control.ControlAccessibleObject>().Subject;

        instance.Should().NotBeNull();
        control.CreateAccessibilityInstance().Should().NotBeSameAs(instance);
        control.AccessibilityObject.Should().NotBeSameAs(instance);
        instance.Owner.Should().BeSameAs(control);
        instance.Role.Should().Be(AccessibleRole.StatusBar);
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
        ToolStripStatusLabel button = control.CreateDefaultItem(text, image, onClick).Should().BeOfType<ToolStripStatusLabel>().Subject;

        button.Text.Should().Be(text);
        button.Image.Should().BeSameAs(image);
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
            control.Parent.Should().BeNull();
            control.Controls.Count.Should().Be(0);
            control.Items.Count.Should().Be(0);
            control.DataBindings.Count.Should().Be(0);
            control.IsHandleCreated.Should().BeFalse();
            control.Disposing.Should().BeTrue();
            control.IsDisposed.Should().Be(callCount > 0);
            callCount++;
        }

        control.Disposed += handler;

        try
        {
            control.Dispose();
            control.Parent.Should().BeNull();
            control.Controls.Count.Should().Be(0);
            control.Items.Count.Should().Be(0);
            control.DataBindings.Count.Should().Be(0);
            control.IsDisposed.Should().BeTrue();
            control.Disposing.Should().BeFalse();
            callCount.Should().Be(1);
            control.IsHandleCreated.Should().BeFalse();

            // Dispose multiple times.
            control.Dispose();
            control.Parent.Should().BeNull();
            control.Controls.Count.Should().Be(0);
            control.Items.Count.Should().Be(0);
            control.DataBindings.Count.Should().Be(0);
            control.IsDisposed.Should().BeTrue();
            control.Disposing.Should().BeFalse();
            callCount.Should().Be(2);
            control.IsHandleCreated.Should().BeFalse();
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
            control.Parent.Should().BeNull();
            control.Controls.Count.Should().Be(0);
            control.Items.Count.Should().Be(0);
            control.DataBindings.Count.Should().Be(0);
            control.IsHandleCreated.Should().BeFalse();
            control.Disposing.Should().BeTrue();
            control.IsDisposed.Should().Be(callCount > 0);
            callCount++;
        }

        control.Disposed += handler;

        try
        {
            control.Dispose(true);
            control.Parent.Should().BeNull();
            control.Controls.Count.Should().Be(0);
            control.Items.Count.Should().Be(0);
            control.DataBindings.Count.Should().Be(0);
            control.IsDisposed.Should().BeTrue();
            control.Disposing.Should().BeFalse();
            callCount.Should().Be(1);
            control.IsHandleCreated.Should().BeFalse();

            // Dispose multiple times.
            control.Dispose(true);
            control.Parent.Should().BeNull();
            control.Controls.Count.Should().Be(0);
            control.Items.Count.Should().Be(0);
            control.DataBindings.Count.Should().Be(0);
            control.IsDisposed.Should().BeTrue();
            control.Disposing.Should().BeFalse();
            callCount.Should().Be(2);
            control.IsHandleCreated.Should().BeFalse();
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
            control.Parent.Should().BeNull();
            control.Controls.Count.Should().Be(expectedChildrenCallCount1);
            control.Items.Count.Should().Be(0);
            control.DataBindings.Count.Should().Be(0);
            control.IsDisposed.Should().BeFalse();
            control.Disposing.Should().BeFalse();
            callCount.Should().Be(0);
            control.IsHandleCreated.Should().BeFalse();

            // Dispose multiple times.
            control.Dispose(false);
            control.Parent.Should().BeNull();
            control.Controls.Count.Should().Be(expectedChildrenCallCount1);
            control.Items.Count.Should().Be(0);
            control.DataBindings.Count.Should().Be(0);
            control.IsDisposed.Should().BeFalse();
            control.Disposing.Should().BeFalse();
            callCount.Should().Be(0);
            control.IsHandleCreated.Should().BeFalse();
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
        callCount.Should().Be(1);
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
        control.GetScrollState(bit).Should().Be(expected);
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
        control.GetStyle(flag).Should().Be(expected);

        // Call again to test caching.
        control.GetStyle(flag).Should().Be(expected);
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
            sender.Should().BeSameAs(control);
            e.Should().BeSameAs(eventArgs);
            callCount++;
        };

        // Call with handler.
        control.Layout += handler;
        control.OnLayout(eventArgs);
        callCount.Should().Be(1);

        // Remove handler.
        control.Layout -= handler;
        control.OnLayout(eventArgs);
        callCount.Should().Be(1);
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
        layoutCallCount.Should().Be(1);

        control.Controls.Count.Should().Be(1);
        Control grip = control.Controls[0];
        grip.Should().BeAssignableTo<Control>();
        grip.Bounds.Should().Be(control.SizeGripBounds);
        grip.BackColor.Should().Be(Color.Transparent);
    }

    [WinFormsFact]
    public void StatusStrip_OnLayout_NullE_ThrowsNullReferenceException()
    {
        using SubStatusStrip control = new();
        control.Invoking(c => c.OnLayout(null))
            .Should().Throw<NullReferenceException>();
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
            sender.Should().BeSameAs(control);
            e.Should().BeSameAs(eventArgs);
            callCount++;
        };

        // Call with handler.
        control.Paint += handler;
        control.OnPaintBackground(eventArgs);
        callCount.Should().Be(0);
        control.IsHandleCreated.Should().BeFalse();

        // Remove handler.
        control.Paint -= handler;
        control.OnPaintBackground(eventArgs);
        callCount.Should().Be(0);
        control.IsHandleCreated.Should().BeFalse();
    }

    [WinFormsFact]
    public void StatusStrip_OnPaintBackground_NullE_ThrowsArgumentNullException()
    {
        using SubStatusStrip control = new();
        control.Invoking(c => c.OnPaintBackground(null))
           .Should().Throw<ArgumentNullException>();
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
