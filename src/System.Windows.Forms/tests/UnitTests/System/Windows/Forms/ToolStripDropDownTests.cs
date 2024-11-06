// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using Moq;
using System.Windows.Forms.TestUtilities;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Tests;

public class ToolStripDropDownTests
{
    [WinFormsFact]
    public void ToolStripDropDown_Ctor_Default()
    {
        using SubToolStripDropDown control = new();
        Assert.Null(control.AccessibleDefaultActionDescription);
        Assert.Null(control.AccessibleDescription);
        Assert.Null(control.AccessibleName);
        Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
        Assert.False(control.AllowDrop);
        Assert.False(control.AllowItemReorder);
        Assert.True(control.AllowMerge);
        Assert.False(control.AllowTransparency);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        Assert.True(control.AutoClose);
        Assert.False(control.AutoScroll);
        Assert.Equal(Size.Empty, control.AutoScrollMargin);
        Assert.Equal(Size.Empty, control.AutoScrollMinSize);
        Assert.Equal(Point.Empty, control.AutoScrollPosition);
        Assert.True(control.AutoSize);
        Assert.Equal(Control.DefaultBackColor, control.BackColor);
        Assert.Null(control.BackgroundImage);
        Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
        Assert.Null(control.BindingContext);
        Assert.Equal(25, control.Bottom);
        Assert.Equal(new Rectangle(0, 0, 100, 25), control.Bounds);
        Assert.True(control.CanEnableIme);
        Assert.False(control.CanFocus);
        Assert.False(control.CanOverflow);
        Assert.True(control.CanRaiseEvents);
        Assert.False(control.CanSelect);
        Assert.False(control.Capture);
        Assert.False(control.CausesValidation);
        Assert.Equal(new Rectangle(0, 0, 100, 25), control.ClientRectangle);
        Assert.Equal(new Size(100, 25), control.ClientSize);
        Assert.Null(control.Container);
        Assert.False(control.ContainsFocus);
        Assert.Null(control.ContextMenuStrip);
        Assert.Empty(control.Controls);
        Assert.Same(control.Controls, control.Controls);
        Assert.False(control.Created);
        Assert.Same(Cursors.Default, control.Cursor);
        Assert.Same(Cursors.Default, control.DefaultCursor);
        Assert.Equal(DockStyle.None, control.DefaultDock);
        Assert.Equal(ToolStripDropDownDirection.Right, control.DefaultDropDownDirection);
        Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
        Assert.Equal(new Padding(2, 2, 2, 2), control.DefaultGripMargin);
        Assert.Equal(Padding.Empty, control.DefaultMargin);
        Assert.Equal(Size.Empty, control.DefaultMaximumSize);
        Assert.Equal(Size.Empty, control.DefaultMinimumSize);
        Assert.Equal(new Padding(1, 2, 1, 2), control.DefaultPadding);
        Assert.Equal(new Size(100, 25), control.DefaultSize);
        Assert.True(control.DefaultShowItemToolTips);
        Assert.False(control.DesignMode);
        Assert.Empty(control.DisplayedItems);
        Assert.Same(control.DisplayedItems, control.DisplayedItems);
        Assert.Equal(new Rectangle(1, 2, 98, 21), control.DisplayRectangle);
        Assert.Equal(DockStyle.None, control.Dock);
        Assert.NotNull(control.DockPadding);
        Assert.Same(control.DockPadding, control.DockPadding);
        Assert.Equal(2, control.DockPadding.Top);
        Assert.Equal(2, control.DockPadding.Bottom);
        Assert.Equal(1, control.DockPadding.Left);
        Assert.Equal(1, control.DockPadding.Right);
        Assert.True(control.DoubleBuffered);
        Assert.Equal(SystemInformation.IsDropShadowEnabled, control.DropShadowEnabled);
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
        Assert.Equal(25, control.Height);
        Assert.NotNull(control.HorizontalScroll);
        Assert.Same(control.HorizontalScroll, control.HorizontalScroll);
        Assert.False(control.HScroll);
        Assert.Null(control.ImageList);
        Assert.Equal(new Size(16, 16), control.ImageScalingSize);
        Assert.Equal(ImeMode.NoControl, control.ImeMode);
        Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
        Assert.False(control.IsAccessible);
        Assert.False(control.IsAutoGenerated);
        Assert.False(control.IsCurrentlyDragging);
        Assert.True(control.IsDropDown);
        Assert.False(control.IsMirrored);
        Assert.Empty(control.Items);
        Assert.Same(control.Items, control.Items);
        Assert.NotNull(control.LayoutEngine);
        Assert.Same(control.LayoutEngine, control.LayoutEngine);
        Assert.IsType<FlowLayoutSettings>(control.LayoutSettings);
        Assert.Same(control.LayoutSettings, control.LayoutSettings);
        Assert.Equal(ToolStripLayoutStyle.Flow, control.LayoutStyle);
        Assert.Equal(0, control.Left);
        Assert.Equal(Point.Empty, control.Location);
        Assert.Equal(Padding.Empty, control.Margin);
        Assert.Equal(Size.Empty, control.MaximumSize);
        Assert.Equal(Screen.GetWorkingArea(new Rectangle(0, 0, 100, 25)).Size - new Size(2, 4), control.MaxItemSize);
        Assert.Equal(Size.Empty, control.MinimumSize);
        Assert.Equal(Orientation.Horizontal, control.Orientation);
        Assert.Equal(1, control.Opacity);
        Assert.NotNull(control.OverflowButton);
        Assert.Same(control.OverflowButton, control.OverflowButton);
        Assert.Same(control, control.OverflowButton.GetCurrentParent());
        Assert.Null(control.OwnerItem);
        Assert.Equal(new Padding(1, 2, 1, 2), control.Padding);
        Assert.Null(control.Parent);
        Assert.True(control.PreferredSize.Width > 0);
        Assert.True(control.PreferredSize.Height > 0);
        Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
        Assert.False(control.RecreatingHandle);
        Assert.Null(control.Region);
        Assert.NotNull(control.Renderer);
        Assert.Same(control.Renderer, control.Renderer);
        Assert.IsType<ToolStripProfessionalRenderer>(control.Renderer);
        Assert.Equal(ToolStripRenderMode.ManagerRenderMode, control.RenderMode);
        Assert.True(control.ResizeRedraw);
        Assert.Equal(SystemInformation.WorkingArea.X + 2, control.Right);
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.True(control.ShowFocusCues);
        Assert.True(control.ShowItemToolTips);
        Assert.True(control.ShowKeyboardCues);
        Assert.Null(control.Site);
        Assert.Equal(new Size(2, 4), control.Size);
        Assert.False(control.Stretch);
        Assert.Equal(0, control.TabIndex);
        Assert.False(control.TabStop);
        Assert.Empty(control.Text);
        Assert.Equal(ToolStripTextDirection.Horizontal, control.TextDirection);
        Assert.Equal(0, control.Top);
        Assert.True(control.TopMost);
        Assert.True(control.TopLevel);
        Assert.Same(control, control.TopLevelControl);
        Assert.False(control.UseWaitCursor);
        Assert.NotNull(control.VerticalScroll);
        Assert.Same(control.VerticalScroll, control.VerticalScroll);
        Assert.False(control.Visible);
        Assert.False(control.VScroll);
        Assert.Equal(2, control.Width);

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripDropDown_CreateParams_GetDefault_ReturnsExpected()
    {
        using SubToolStripDropDown control = new();
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Null(createParams.ClassName);
        Assert.Equal(SystemInformation.IsDropShadowEnabled ? 0x20808 : 0x808, createParams.ClassStyle);
        Assert.Equal(0x10000, createParams.ExStyle);
        Assert.Equal(25, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(-0x7E000000, createParams.Style);
        Assert.Equal(100, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> CreateParams_GetDropShadowEnabled_TestData()
    {
        yield return new object[] { true, SystemInformation.IsDropShadowEnabled ? 0x20808 : 0x808 };
        yield return new object[] { false, 0x808 };
    }

    [WinFormsTheory]
    [MemberData(nameof(CreateParams_GetDropShadowEnabled_TestData))]
    public void ToolStripDropDown_CreateParams_GetDropShadowEnabled_ReturnsExpected(bool dropShadowEnabled, int expectedClassStyle)
    {
        using SubToolStripDropDown control = new()
        {
            DropShadowEnabled = dropShadowEnabled
        };

        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Null(createParams.ClassName);
        Assert.Equal(expectedClassStyle, createParams.ClassStyle);
        Assert.Equal(0x10000, createParams.ExStyle);
        Assert.Equal(25, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(-0x7E000000, createParams.Style);
        Assert.Equal(100, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> CreateParams_GetTopLevelOpacity_TestData()
    {
        yield return new object[] { true, 1.0, SystemInformation.IsDropShadowEnabled ? 0x20808 : 0x808, -0x7E000000, 0x10000 };
        yield return new object[] { true, 0.5, SystemInformation.IsDropShadowEnabled ? 0x20008 : 0x8, -0x7E000000, 0x90000 };
        yield return new object[] { false, 1.0, SystemInformation.IsDropShadowEnabled ? 0x20008 : 0x8, 0x46000000, 0x10000 };
        yield return new object[] { false, 0.5, SystemInformation.IsDropShadowEnabled ? 0x20008 : 0x8, 0x46000000, 0x10000 };
    }

    [WinFormsTheory]
    [MemberData(nameof(CreateParams_GetTopLevelOpacity_TestData))]
    public void ToolStripDropDown_CreateParams_GetTopLevelOpacity_ReturnsExpected(bool topLevel, float opacity, int expectedClassStyle, int expectedStyle, int expectedExStyle)
    {
        using SubToolStripDropDown control = new()
        {
            TopLevel = topLevel,
            Opacity = opacity
        };

        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Null(createParams.ClassName);
        Assert.Equal(expectedClassStyle, createParams.ClassStyle);
        Assert.Equal(expectedExStyle, createParams.ExStyle);
        Assert.Equal(25, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(expectedStyle, createParams.Style);
        Assert.Equal(100, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripDropDown_AllowItemReorder_Set_GetReturnsExpected(bool value)
    {
        using ToolStripDropDown control = new()
        {
            AllowItemReorder = value
        };
        Assert.Equal(value, control.AllowItemReorder);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AllowItemReorder = value;
        Assert.Equal(value, control.AllowItemReorder);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.AllowItemReorder = value;
        Assert.Equal(value, control.AllowItemReorder);
        Assert.False(control.IsHandleCreated);
    }

    [Fact] // x-thread
    public void ToolStripDropDown_AllowItemReorder_SetWithHandleNonSTAThread_ThrowsInvalidOperationException()
    {
        using ToolStripDropDown control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Throws<InvalidOperationException>(() => control.AllowItemReorder = true);
        Assert.True(control.AllowItemReorder);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Can set to false.
        control.AllowItemReorder = false;
        Assert.False(control.AllowItemReorder);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripDropDown_AllowItemReorder_SetWithHandleSTA_GetReturnsExpected(bool value)
    {
        using ToolStripDropDown control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.AllowItemReorder = value;
        Assert.Equal(value, control.AllowItemReorder);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.AllowItemReorder = value;
        Assert.Equal(value, control.AllowItemReorder);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.AllowItemReorder = value;
        Assert.Equal(value, control.AllowItemReorder);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ToolStripDropDown_AllowItemReorder_SetAllowDrop_ThrowsArgumentException()
    {
        using ToolStripDropDown control = new()
        {
            AllowDrop = true
        };
        Assert.Throws<ArgumentException>(() => control.AllowItemReorder = true);
        Assert.False(control.AllowItemReorder);

        control.AllowItemReorder = false;
        Assert.False(control.AllowItemReorder);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripDropDown_AllowTransparency_Set_GetReturnsExpected(bool value)
    {
        using ToolStripDropDown control = new()
        {
            AllowTransparency = value
        };
        Assert.Equal(value, control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AllowTransparency = value;
        Assert.Equal(value, control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.AllowTransparency = !value;
        Assert.Equal(!value, control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> AllowTransparency_SetWithOpacity_TestData()
    {
        yield return new object[] { true, 0.5 };
        yield return new object[] { false, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(AllowTransparency_SetWithOpacity_TestData))]
    public void ToolStripDropDown_AllowTransparency_SetWithOpacity_GetReturnsExpected(bool value, float expectedOpacity)
    {
        using ToolStripDropDown control = new()
        {
            Opacity = 0.5,
            AllowTransparency = value
        };
        Assert.Equal(value, control.AllowTransparency);
        Assert.Equal(expectedOpacity, control.Opacity);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AllowTransparency = value;
        Assert.Equal(value, control.AllowTransparency);
        Assert.Equal(expectedOpacity, control.Opacity);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.AllowTransparency = !value;
        Assert.Equal(!value, control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripDropDown_AllowTransparency_SetNotTopLevel_GetReturnsExpected(bool value)
    {
        using ToolStripDropDown control = new()
        {
            TopLevel = false,
            AllowTransparency = value
        };
        Assert.Equal(value, control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AllowTransparency = value;
        Assert.Equal(value, control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.AllowTransparency = !value;
        Assert.Equal(!value, control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void ToolStripDropDown_AllowTransparency_SetWithHandle_GetReturnsExpected(bool value, int expectedStyleChangedCallCount)
    {
        using ToolStripDropDown control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.AllowTransparency = value;
        Assert.Equal(value, control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.AllowTransparency = value;
        Assert.Equal(value, control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.AllowTransparency = !value;
        Assert.Equal(!value, control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount + 1, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount + 1, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> AllowTransparency_SetWithOpacityWithHandle_TestData()
    {
        yield return new object[] { true, 0.5, 0 };
        yield return new object[] { false, 1, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(AllowTransparency_SetWithOpacityWithHandle_TestData))]
    public void ToolStripDropDown_AllowTransparency_SetWithOpacityWithHandle_GetReturnsExpected(bool value, float expectedOpacity, int expectedStyleChangedCallCount)
    {
        using ToolStripDropDown control = new()
        {
            Opacity = 0.5
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.AllowTransparency = value;
        Assert.Equal(value, control.AllowTransparency);
        Assert.Equal(expectedOpacity, control.Opacity);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.AllowTransparency = value;
        Assert.Equal(value, control.AllowTransparency);
        Assert.Equal(expectedOpacity, control.Opacity);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.AllowTransparency = !value;
        Assert.Equal(!value, control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount + 1, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount + 1, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void ToolStripDropDown_AllowTransparency_SetNotTopLevelWithHandle_GetReturnsExpected(bool value, int expectedStyleChangedCallCount)
    {
        using ToolStripDropDown control = new()
        {
            TopLevel = false
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.AllowTransparency = value;
        Assert.Equal(value, control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.AllowTransparency = value;
        Assert.Equal(value, control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.AllowTransparency = !value;
        Assert.Equal(!value, control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount + 1, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount + 1, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> Anchor_Set_TestData()
    {
        yield return new object[] { AnchorStyles.Top, AnchorStyles.Top, DockStyle.None };
        yield return new object[] { AnchorStyles.Top | AnchorStyles.Bottom, AnchorStyles.Top | AnchorStyles.Bottom, DockStyle.None };
        yield return new object[] { AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, DockStyle.None };
        yield return new object[] { AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right, DockStyle.None };
        yield return new object[] { AnchorStyles.Top | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Left, DockStyle.None };
        yield return new object[] { AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, DockStyle.None };
        yield return new object[] { AnchorStyles.Top | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Right, DockStyle.None };
        yield return new object[] { AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, DockStyle.None };

        yield return new object[] { AnchorStyles.Bottom, AnchorStyles.Bottom, DockStyle.None };
        yield return new object[] { AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Bottom | AnchorStyles.Left, DockStyle.None };
        yield return new object[] { AnchorStyles.Bottom | AnchorStyles.Right, AnchorStyles.Bottom | AnchorStyles.Right, DockStyle.None };
        yield return new object[] { AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, DockStyle.None };

        yield return new object[] { AnchorStyles.Left, AnchorStyles.Left, DockStyle.None };
        yield return new object[] { AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Left | AnchorStyles.Right, DockStyle.None };

        yield return new object[] { AnchorStyles.Right, AnchorStyles.Right, DockStyle.None };

        yield return new object[] { AnchorStyles.None, AnchorStyles.None, DockStyle.None };
        yield return new object[] { (AnchorStyles)(-1), AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, DockStyle.None };
        yield return new object[] { (AnchorStyles)int.MaxValue, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, DockStyle.None };
    }

    [WinFormsTheory]
    [MemberData(nameof(Anchor_Set_TestData))]
    public void ToolStripDropDown_Anchor_Set_GetReturnsExpected(AnchorStyles value, AnchorStyles expected, DockStyle expectedDock)
    {
        using ToolStripDropDown control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Anchor", e.AffectedProperty);
            layoutCallCount++;
        };

        control.Anchor = value;
        Assert.Equal(expected, control.Anchor);
        Assert.Equal(expectedDock, control.Dock);
        Assert.Equal(1, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Anchor = value;
        Assert.Equal(expected, control.Anchor);
        Assert.Equal(expectedDock, control.Dock);
        Assert.Equal(2, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripDropDown_AutoClose_Set_GetReturnsExpected(bool value)
    {
        using ToolStripDropDown control = new()
        {
            AutoClose = value
        };
        Assert.Equal(value, control.AutoClose);
        Assert.Equal(!value, control.IsHandleCreated);

        // Set same.
        control.AutoClose = value;
        Assert.Equal(value, control.AutoClose);
        Assert.Equal(!value, control.IsHandleCreated);

        // Set different.
        control.AutoClose = !value;
        Assert.Equal(!value, control.AutoClose);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripDropDown_AutoClose_SetNotTopMost_GetReturnsExpected(bool value)
    {
        using NotTopMostToolStripDropDown control = new()
        {
            AutoClose = value
        };
        Assert.Equal(value, control.AutoClose);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AutoClose = value;
        Assert.Equal(value, control.AutoClose);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.AutoClose = !value;
        Assert.Equal(!value, control.AutoClose);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 0)]
    [InlineData(false, 1)]
    public void ToolStripDropDown_AutoSize_Set_GetReturnsExpected(bool value, int expectedLayoutCallCount)
    {
        using ToolStripDropDown control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("AutoSize", e.AffectedProperty);
            layoutCallCount++;
        };

        control.AutoSize = value;
        Assert.Equal(value, control.AutoSize);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AutoSize = value;
        Assert.Equal(value, control.AutoSize);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.AutoSize = !value;
        Assert.Equal(!value, control.AutoSize);
        Assert.Equal(expectedLayoutCallCount + 1, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 0, 0)]
    [InlineData(false, 1, 2)]
    public void ToolStripDropDown_AutoSize_SetWithParent_GetReturnsExpected(bool value, int expectedLayoutCallCount, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using ToolStripDropDown control = new()
        {
            TopLevel = false,
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("AutoSize", e.AffectedProperty);
            layoutCallCount++;
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.AutoSize = value;
            Assert.Equal(value, control.AutoSize);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            control.AutoSize = value;
            Assert.Equal(value, control.AutoSize);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set different.
            control.AutoSize = !value;
            Assert.Equal(!value, control.AutoSize);
            Assert.Equal(expectedLayoutCallCount + 1, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void ToolStripDropDown_AutoSize_SetWithHandler_CallsAutoSizeChanged()
    {
        using ToolStripDropDown control = new()
        {
            AutoSize = true
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.AutoSizeChanged += handler;

        // Set different.
        control.AutoSize = false;
        Assert.False(control.AutoSize);
        Assert.Equal(1, callCount);

        // Set same.
        control.AutoSize = false;
        Assert.False(control.AutoSize);
        Assert.Equal(1, callCount);

        // Set different.
        control.AutoSize = true;
        Assert.True(control.AutoSize);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.AutoSizeChanged -= handler;
        control.AutoSize = false;
        Assert.False(control.AutoSize);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetImageTheoryData))]
    public void ToolStripDropDown_BackgroundImage_Set_GetReturnsExpected(Image value)
    {
        using ToolStripDropDown control = new()
        {
            BackgroundImage = value
        };
        Assert.Same(value, control.BackgroundImage);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BackgroundImage = value;
        Assert.Same(value, control.BackgroundImage);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripDropDown_BackgroundImage_SetWithHandler_CallsBackgroundImageChanged()
    {
        using ToolStripDropDown control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.BackgroundImageChanged += handler;

        // Set different.
        using Bitmap image1 = new(10, 10);
        control.BackgroundImage = image1;
        Assert.Same(image1, control.BackgroundImage);
        Assert.Equal(1, callCount);

        // Set same.
        control.BackgroundImage = image1;
        Assert.Same(image1, control.BackgroundImage);
        Assert.Equal(1, callCount);

        // Set different.
        using Bitmap image2 = new(10, 10);
        control.BackgroundImage = image2;
        Assert.Same(image2, control.BackgroundImage);
        Assert.Equal(2, callCount);

        // Set null.
        control.BackgroundImage = null;
        Assert.Null(control.BackgroundImage);
        Assert.Equal(3, callCount);

        // Remove handler.
        control.BackgroundImageChanged -= handler;
        control.BackgroundImage = image1;
        Assert.Same(image1, control.BackgroundImage);
        Assert.Equal(3, callCount);
    }

    [WinFormsTheory]
    [EnumData<ImageLayout>]
    public void ToolStripDropDown_BackgroundImageLayout_Set_GetReturnsExpected(ImageLayout value)
    {
        using SubToolStripDropDown control = new()
        {
            BackgroundImageLayout = value
        };
        Assert.Equal(value, control.BackgroundImageLayout);
        Assert.True(control.DoubleBuffered);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BackgroundImageLayout = value;
        Assert.Equal(value, control.BackgroundImageLayout);
        Assert.True(control.DoubleBuffered);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripDropDown_BackgroundImageLayout_SetWithHandler_CallsBackgroundImageLayoutChanged()
    {
        using ToolStripDropDown control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.BackgroundImageLayoutChanged += handler;

        // Set different.
        control.BackgroundImageLayout = ImageLayout.Center;
        Assert.Equal(ImageLayout.Center, control.BackgroundImageLayout);
        Assert.Equal(1, callCount);

        // Set same.
        control.BackgroundImageLayout = ImageLayout.Center;
        Assert.Equal(ImageLayout.Center, control.BackgroundImageLayout);
        Assert.Equal(1, callCount);

        // Set different.
        control.BackgroundImageLayout = ImageLayout.Stretch;
        Assert.Equal(ImageLayout.Stretch, control.BackgroundImageLayout);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.BackgroundImageLayoutChanged -= handler;
        control.BackgroundImageLayout = ImageLayout.Center;
        Assert.Equal(ImageLayout.Center, control.BackgroundImageLayout);
        Assert.Equal(2, callCount);
    }

    public static IEnumerable<object[]> BindingContext_Set_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new BindingContext() };
    }

    [WinFormsTheory]
    [MemberData(nameof(BindingContext_Set_TestData))]
    public void ToolStripDropDown_BindingContext_Set_GetReturnsExpected(BindingContext value)
    {
        using ToolStripDropDown control = new()
        {
            BindingContext = value
        };
        Assert.Same(value, control.BindingContext);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BindingContext = value;
        Assert.Same(value, control.BindingContext);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripDropDown_BindingContext_SetWithHandler_CallsBindingContextChanged()
    {
        using ToolStripDropDown control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.BindingContextChanged += handler;

        // Set different.
        BindingContext context1 = [];
        control.BindingContext = context1;
        Assert.Same(context1, control.BindingContext);
        Assert.Equal(1, callCount);

        // Set same.
        control.BindingContext = context1;
        Assert.Same(context1, control.BindingContext);
        Assert.Equal(1, callCount);

        // Set different.
        BindingContext context2 = [];
        control.BindingContext = context2;
        Assert.Same(context2, control.BindingContext);
        Assert.Equal(2, callCount);

        // Set null.
        control.BindingContext = null;
        Assert.Null(control.BindingContext);
        Assert.Equal(3, callCount);

        // Remove handler.
        control.BindingContextChanged -= handler;
        control.BindingContext = context1;
        Assert.Same(context1, control.BindingContext);
        Assert.Equal(3, callCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripDropDown_CanOverflow_Set_GetReturnsExpected(bool value)
    {
        using ToolStripDropDown control = new();
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

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void ToolStripDropDown_CanOverflow_SetWithHandle_GetReturnsExpected(bool value, int expectedLayoutCallCount)
    {
        using ToolStripDropDown control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Null(e.AffectedProperty);
            layoutCallCount++;
        };

        control.CanOverflow = value;
        Assert.Equal(value, control.CanOverflow);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedLayoutCallCount * 2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.CanOverflow = value;
        Assert.Equal(value, control.CanOverflow);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedLayoutCallCount * 2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.CanOverflow = !value;
        Assert.Equal(!value, control.CanOverflow);
        Assert.Equal(expectedLayoutCallCount + 1, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedLayoutCallCount + 2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> ContextMenuStrip_Set_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new ContextMenuStrip() };
    }

    [WinFormsTheory]
    [MemberData(nameof(ContextMenuStrip_Set_TestData))]
    public void ToolStripDropDown_ContextMenuStrip_Set_GetReturnsExpected(ContextMenuStrip value)
    {
        using SubToolStripDropDown control = new()
        {
            ContextMenuStrip = value
        };
        Assert.Same(value, control.ContextMenuStrip);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ContextMenuStrip = value;
        Assert.Same(value, control.ContextMenuStrip);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripDropDown_ContextMenuStrip_SetWithHandler_CallsContextMenuStripChanged()
    {
        using SubToolStripDropDown control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.ContextMenuStripChanged += handler;

        // Set different.
        using ContextMenuStrip menu1 = new();
        control.ContextMenuStrip = menu1;
        Assert.Same(menu1, control.ContextMenuStrip);
        Assert.Equal(1, callCount);

        // Set same.
        control.ContextMenuStrip = menu1;
        Assert.Same(menu1, control.ContextMenuStrip);
        Assert.Equal(1, callCount);

        // Set different.
        using ContextMenuStrip menu2 = new();
        control.ContextMenuStrip = menu2;
        Assert.Same(menu2, control.ContextMenuStrip);
        Assert.Equal(2, callCount);

        // Set null.
        control.ContextMenuStrip = null;
        Assert.Null(control.ContextMenuStrip);
        Assert.Equal(3, callCount);

        // Remove handler.
        control.ContextMenuStripChanged -= handler;
        control.ContextMenuStrip = menu1;
        Assert.Same(menu1, control.ContextMenuStrip);
        Assert.Equal(3, callCount);
    }

    public static IEnumerable<object[]> DefaultDropDownDirection_Get_TestData()
    {
        yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.None, RightToLeft.Yes, ToolStripDropDownDirection.Left };
        yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.None, RightToLeft.No, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.None, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Left, RightToLeft.Yes, ToolStripDropDownDirection.Left };
        yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Left, RightToLeft.No, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Left, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Right, RightToLeft.Yes, ToolStripDropDownDirection.Left };
        yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Right, RightToLeft.No, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Right, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Top, RightToLeft.Yes, ToolStripDropDownDirection.Left };
        yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Top, RightToLeft.No, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Top, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Bottom, RightToLeft.Yes, ToolStripDropDownDirection.Left };
        yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Bottom, RightToLeft.No, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Bottom, RightToLeft.Inherit, ToolStripDropDownDirection.Right };

        yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.None, RightToLeft.Yes, ToolStripDropDownDirection.Left };
        yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.None, RightToLeft.No, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.None, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Right, RightToLeft.Yes, ToolStripDropDownDirection.Left };
        yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Right, RightToLeft.No, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Right, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Left, RightToLeft.Yes, ToolStripDropDownDirection.Left };
        yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Left, RightToLeft.No, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Left, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Top, RightToLeft.Yes, ToolStripDropDownDirection.Left };
        yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Top, RightToLeft.No, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Top, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Bottom, RightToLeft.Yes, ToolStripDropDownDirection.Left };
        yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Bottom, RightToLeft.No, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Bottom, RightToLeft.Inherit, ToolStripDropDownDirection.Right };

        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.None, RightToLeft.Yes, ToolStripDropDownDirection.Left };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.None, RightToLeft.No, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.None, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Right, RightToLeft.Yes, ToolStripDropDownDirection.Left };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Right, RightToLeft.No, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Right, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, RightToLeft.Yes, ToolStripDropDownDirection.Left };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, RightToLeft.No, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, RightToLeft.Yes, ToolStripDropDownDirection.Left };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, RightToLeft.No, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Bottom, RightToLeft.Yes, ToolStripDropDownDirection.Left };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Bottom, RightToLeft.No, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Bottom, RightToLeft.Inherit, ToolStripDropDownDirection.Right };

        yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.None, RightToLeft.Yes, ToolStripDropDownDirection.Left };
        yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.None, RightToLeft.No, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.None, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Right, RightToLeft.Yes, ToolStripDropDownDirection.Left };
        yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Right, RightToLeft.No, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Right, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Left, RightToLeft.Yes, ToolStripDropDownDirection.Left };
        yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Left, RightToLeft.No, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Left, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Top, RightToLeft.Yes, ToolStripDropDownDirection.Left };
        yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Top, RightToLeft.No, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Top, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Bottom, RightToLeft.Yes, ToolStripDropDownDirection.Left };
        yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Bottom, RightToLeft.No, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Bottom, RightToLeft.Inherit, ToolStripDropDownDirection.Right };

        yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.None, RightToLeft.Yes, ToolStripDropDownDirection.Left };
        yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.None, RightToLeft.No, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.None, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Right, RightToLeft.Yes, ToolStripDropDownDirection.Left };
        yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Right, RightToLeft.No, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Right, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Left, RightToLeft.Yes, ToolStripDropDownDirection.Left };
        yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Left, RightToLeft.No, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Left, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Top, RightToLeft.Yes, ToolStripDropDownDirection.Left };
        yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Top, RightToLeft.No, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Top, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Bottom, RightToLeft.Yes, ToolStripDropDownDirection.Left };
        yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Bottom, RightToLeft.No, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Bottom, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
    }

    [WinFormsTheory]
    [MemberData(nameof(DefaultDropDownDirection_Get_TestData))]
    public void ToolStripDropDown_DefaultDropDownDirection_Get_ReturnsExpected(ToolStripLayoutStyle layoutStyle, DockStyle dock, RightToLeft rightToLeft, ToolStripDropDownDirection expected)
    {
        using ToolStripDropDown control = new()
        {
            LayoutStyle = layoutStyle,
            Dock = dock,
            RightToLeft = rightToLeft
        };
        Assert.Equal(expected, control.DefaultDropDownDirection);
    }

    [WinFormsTheory]
    [MemberData(nameof(DefaultDropDownDirection_Get_TestData))]
    public void ToolStripDropDown_DefaultDropDownDirection_GetDesignMode_ReturnsExpected(ToolStripLayoutStyle layoutStyle, DockStyle dock, RightToLeft rightToLeft, ToolStripDropDownDirection expected)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite
            .Setup(s => s.Name)
            .Returns("Name");
        using ToolStripDropDown control = new()
        {
            Site = mockSite.Object,
            LayoutStyle = layoutStyle,
            Dock = dock,
            RightToLeft = rightToLeft
        };
        Assert.Equal(expected, control.DefaultDropDownDirection);
    }

    public static IEnumerable<object[]> DefaultDropDownDirection_GetWithParent_TestData()
    {
        foreach (DockStyle parentDock in new object[] { DockStyle.None, DockStyle.Left, DockStyle.Right, DockStyle.Top, DockStyle.Bottom })
        {
            yield return new object[] { parentDock, ToolStripLayoutStyle.Flow, DockStyle.None, RightToLeft.Yes, ToolStripDropDownDirection.Left };
            yield return new object[] { parentDock, ToolStripLayoutStyle.Flow, DockStyle.None, RightToLeft.No, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.Flow, DockStyle.None, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.Flow, DockStyle.Left, RightToLeft.Yes, ToolStripDropDownDirection.Left };
            yield return new object[] { parentDock, ToolStripLayoutStyle.Flow, DockStyle.Left, RightToLeft.No, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.Flow, DockStyle.Left, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.Flow, DockStyle.Right, RightToLeft.Yes, ToolStripDropDownDirection.Left };
            yield return new object[] { parentDock, ToolStripLayoutStyle.Flow, DockStyle.Right, RightToLeft.No, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.Flow, DockStyle.Right, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.Flow, DockStyle.Top, RightToLeft.Yes, ToolStripDropDownDirection.Left };
            yield return new object[] { parentDock, ToolStripLayoutStyle.Flow, DockStyle.Top, RightToLeft.No, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.Flow, DockStyle.Top, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.Flow, DockStyle.Bottom, RightToLeft.Yes, ToolStripDropDownDirection.Left };
            yield return new object[] { parentDock, ToolStripLayoutStyle.Flow, DockStyle.Bottom, RightToLeft.No, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.Flow, DockStyle.Bottom, RightToLeft.Inherit, ToolStripDropDownDirection.Right };

            yield return new object[] { parentDock, ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.None, RightToLeft.Yes, ToolStripDropDownDirection.Left };
            yield return new object[] { parentDock, ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.None, RightToLeft.No, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.None, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Right, RightToLeft.Yes, ToolStripDropDownDirection.Left };
            yield return new object[] { parentDock, ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Right, RightToLeft.No, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Right, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Left, RightToLeft.Yes, ToolStripDropDownDirection.Left };
            yield return new object[] { parentDock, ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Left, RightToLeft.No, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Left, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Top, RightToLeft.Yes, ToolStripDropDownDirection.Left };
            yield return new object[] { parentDock, ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Top, RightToLeft.No, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Top, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Bottom, RightToLeft.Yes, ToolStripDropDownDirection.Left };
            yield return new object[] { parentDock, ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Bottom, RightToLeft.No, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Bottom, RightToLeft.Inherit, ToolStripDropDownDirection.Right };

            yield return new object[] { parentDock, ToolStripLayoutStyle.StackWithOverflow, DockStyle.None, RightToLeft.Yes, ToolStripDropDownDirection.Left };
            yield return new object[] { parentDock, ToolStripLayoutStyle.StackWithOverflow, DockStyle.None, RightToLeft.No, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.StackWithOverflow, DockStyle.None, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Right, RightToLeft.Yes, ToolStripDropDownDirection.Left };
            yield return new object[] { parentDock, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Right, RightToLeft.No, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Right, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, RightToLeft.Yes, ToolStripDropDownDirection.Left };
            yield return new object[] { parentDock, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, RightToLeft.No, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, RightToLeft.Yes, ToolStripDropDownDirection.Left };
            yield return new object[] { parentDock, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, RightToLeft.No, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Bottom, RightToLeft.Yes, ToolStripDropDownDirection.Left };
            yield return new object[] { parentDock, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Bottom, RightToLeft.No, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Bottom, RightToLeft.Inherit, ToolStripDropDownDirection.Right };

            yield return new object[] { parentDock, ToolStripLayoutStyle.Table, DockStyle.None, RightToLeft.Yes, ToolStripDropDownDirection.Left };
            yield return new object[] { parentDock, ToolStripLayoutStyle.Table, DockStyle.None, RightToLeft.No, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.Table, DockStyle.None, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.Table, DockStyle.Right, RightToLeft.Yes, ToolStripDropDownDirection.Left };
            yield return new object[] { parentDock, ToolStripLayoutStyle.Table, DockStyle.Right, RightToLeft.No, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.Table, DockStyle.Right, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.Table, DockStyle.Left, RightToLeft.Yes, ToolStripDropDownDirection.Left };
            yield return new object[] { parentDock, ToolStripLayoutStyle.Table, DockStyle.Left, RightToLeft.No, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.Table, DockStyle.Left, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.Table, DockStyle.Top, RightToLeft.Yes, ToolStripDropDownDirection.Left };
            yield return new object[] { parentDock, ToolStripLayoutStyle.Table, DockStyle.Top, RightToLeft.No, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.Table, DockStyle.Top, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.Table, DockStyle.Bottom, RightToLeft.Yes, ToolStripDropDownDirection.Left };
            yield return new object[] { parentDock, ToolStripLayoutStyle.Table, DockStyle.Bottom, RightToLeft.No, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.Table, DockStyle.Bottom, RightToLeft.Inherit, ToolStripDropDownDirection.Right };

            yield return new object[] { parentDock, ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.None, RightToLeft.Yes, ToolStripDropDownDirection.Left };
            yield return new object[] { parentDock, ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.None, RightToLeft.No, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.None, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Right, RightToLeft.Yes, ToolStripDropDownDirection.Left };
            yield return new object[] { parentDock, ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Right, RightToLeft.No, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Right, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Left, RightToLeft.Yes, ToolStripDropDownDirection.Left };
            yield return new object[] { parentDock, ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Left, RightToLeft.No, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Left, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Top, RightToLeft.Yes, ToolStripDropDownDirection.Left };
            yield return new object[] { parentDock, ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Top, RightToLeft.No, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Top, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Bottom, RightToLeft.Yes, ToolStripDropDownDirection.Left };
            yield return new object[] { parentDock, ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Bottom, RightToLeft.No, ToolStripDropDownDirection.Right };
            yield return new object[] { parentDock, ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Bottom, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DefaultDropDownDirection_GetWithParent_TestData))]
    public void ToolStripDropDown_DefaultDropDownDirection_GetWithParent_ReturnsExpected(DockStyle parentDock, ToolStripLayoutStyle layoutStyle, DockStyle dock, RightToLeft rightToLeft, ToolStripDropDownDirection expected)
    {
        using Control parent = new()
        {
            Dock = parentDock
        };
        using ToolStripDropDown control = new()
        {
            TopLevel = false,
            Parent = parent,
            LayoutStyle = layoutStyle,
            Dock = dock,
            RightToLeft = rightToLeft
        };
        Assert.Equal(expected, control.DefaultDropDownDirection);
    }

    [WinFormsTheory]
    [MemberData(nameof(DefaultDropDownDirection_GetWithParent_TestData))]
    public void ToolStripDropDown_DefaultDropDownDirection_GetDesignModeWithParent_ReturnsExpected(DockStyle parentDock, ToolStripLayoutStyle layoutStyle, DockStyle dock, RightToLeft rightToLeft, ToolStripDropDownDirection expected)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite
            .Setup(s => s.Name)
            .Returns("Name");
        using Control parent = new()
        {
            Dock = parentDock
        };
        using ToolStripDropDown control = new()
        {
            TopLevel = false,
            Parent = parent,
            Site = mockSite.Object,
            LayoutStyle = layoutStyle,
            Dock = dock,
            RightToLeft = rightToLeft
        };
        Assert.Equal(expected, control.DefaultDropDownDirection);
    }

    public static IEnumerable<object[]> DefaultDropDownDirection_Set_TestData()
    {
        yield return new object[] { ToolStripDropDownDirection.AboveLeft, ToolStripDropDownDirection.AboveLeft };
        yield return new object[] { ToolStripDropDownDirection.AboveRight, ToolStripDropDownDirection.AboveRight };
        yield return new object[] { ToolStripDropDownDirection.BelowLeft, ToolStripDropDownDirection.BelowLeft };
        yield return new object[] { ToolStripDropDownDirection.BelowRight, ToolStripDropDownDirection.BelowRight };
        yield return new object[] { ToolStripDropDownDirection.Default, ToolStripDropDownDirection.Right };
        yield return new object[] { ToolStripDropDownDirection.Left, ToolStripDropDownDirection.Left };
        yield return new object[] { ToolStripDropDownDirection.Right, ToolStripDropDownDirection.Right };
    }

    [WinFormsTheory]
    [MemberData(nameof(DefaultDropDownDirection_Set_TestData))]
    public void ToolStripDropDown_DefaultDropDownDirection_Set_GetReturnsExpected(ToolStripDropDownDirection value, ToolStripDropDownDirection expected)
    {
        using ToolStripDropDown control = new()
        {
            DefaultDropDownDirection = value
        };
        Assert.Equal(expected, control.DefaultDropDownDirection);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.DefaultDropDownDirection = value;
        Assert.Equal(expected, control.DefaultDropDownDirection);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(DefaultDropDownDirection_Set_TestData))]
    public void ToolStripDropDown_DefaultDropDownDirection_SetWithHandle_GetReturnsExpected(ToolStripDropDownDirection value, ToolStripDropDownDirection expected)
    {
        using ToolStripDropDown control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.DefaultDropDownDirection = value;
        Assert.Equal(expected, control.DefaultDropDownDirection);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.DefaultDropDownDirection = value;
        Assert.Equal(expected, control.DefaultDropDownDirection);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ToolStripDropDown_DefaultDropDownDirection_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStrip))[nameof(ToolStrip.DefaultDropDownDirection)];
        using ToolStripDropDown control = new();
        Assert.False(property.CanResetValue(control));

        control.DefaultDropDownDirection = ToolStripDropDownDirection.Right;
        Assert.Equal(ToolStripDropDownDirection.Right, control.DefaultDropDownDirection);
        Assert.False(property.CanResetValue(control));

        property.ResetValue(control);
        Assert.Equal(ToolStripDropDownDirection.Right, control.DefaultDropDownDirection);
        Assert.False(property.CanResetValue(control));
    }

    [WinFormsFact]
    public void ToolStripDropDown_DefaultDropDownDirection_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStrip))[nameof(ToolStrip.DefaultDropDownDirection)];
        using ToolStripDropDown control = new();
        Assert.False(property.ShouldSerializeValue(control));

        control.DefaultDropDownDirection = ToolStripDropDownDirection.Right;
        Assert.Equal(ToolStripDropDownDirection.Right, control.DefaultDropDownDirection);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.Equal(ToolStripDropDownDirection.Right, control.DefaultDropDownDirection);
        Assert.True(property.ShouldSerializeValue(control));

        control.DefaultDropDownDirection = ToolStripDropDownDirection.Default;
        Assert.Equal(ToolStripDropDownDirection.Right, control.DefaultDropDownDirection);
        Assert.False(property.ShouldSerializeValue(control));
    }

    [WinFormsTheory]
    [InvalidEnumData<ToolStripDropDownDirection>]
    public void ToolStripDropDown_DefaultDropDownDirection_SetInvalidValue_ThrowsInvalidEnumArgumentException(ToolStripDropDownDirection value)
    {
        using ToolStripDropDown control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.DefaultDropDownDirection = value);
    }

    [WinFormsFact]
    public void ToolStripDropDown_Font_Get_ReturnsSame()
    {
        using ToolStripDropDown control = new();
        Assert.NotSame(Control.DefaultFont, control.Font);
        Assert.Same(control.Font, control.Font);
    }

    [WinFormsFact]
    public void ToolStripDropDown_Font_GetWithParent_ReturnsExpected()
    {
        using Font font1 = new("Arial", 8.25f);
        using Font font2 = new("Arial", 8.5f);
        using Control parent = new()
        {
            Font = font1
        };
        using ToolStrip control = new()
        {
            Parent = parent
        };
        Assert.NotSame(font1, control.Font);
        Assert.Same(control.Font, control.Font);

        // Set custom.
        control.Font = font2;
        Assert.Same(font2, control.Font);
    }

    [WinFormsFact]
    public void ToolStripDropDown_Font_GetWithParentCantAccessProperties_ReturnsExpected()
    {
        using Font font1 = new("Arial", 8.25f);
        using Font font2 = new("Arial", 8.5f);
        using SubAxHost parent = new("00000000-0000-0000-0000-000000000000")
        {
            Font = font1
        };
        using ToolStrip control = new()
        {
            Parent = parent
        };
        Assert.NotSame(Control.DefaultFont, control.Font);
        Assert.Same(control.Font, control.Font);

        // Set custom.
        control.Font = font2;
        Assert.Same(font2, control.Font);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetFontTheoryData))]
    public void ToolStripDropDown_Font_Set_GetReturnsExpected(Font value)
    {
        using SubToolStripDropDown control = new()
        {
            Font = value
        };
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripDropDown_Font_SetWithHandler_CallsFontChanged()
    {
        using ToolStripDropDown control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.FontChanged += handler;

        // Set different.
        using Font font1 = new("Arial", 8.25f);
        control.Font = font1;
        Assert.Same(font1, control.Font);
        Assert.Equal(1, callCount);

        // Set same.
        control.Font = font1;
        Assert.Same(font1, control.Font);
        Assert.Equal(1, callCount);

        // Set different.
        using var font2 = SystemFonts.DialogFont;
        control.Font = font2;
        Assert.Same(font2, control.Font);
        Assert.Equal(2, callCount);

        // Set null.
        control.Font = null;
        Assert.Equal(Control.DefaultFont, control.Font);
        Assert.Equal(3, callCount);

        // Remove handler.
        control.FontChanged -= handler;
        control.Font = font1;
        Assert.Same(font1, control.Font);
        Assert.Equal(3, callCount);
    }

    [WinFormsFact]
    public void ToolStripDropDown_Font_SetWithItemsWithHandler_CallsFontChanged()
    {
        using SubToolStripItem item1 = new();
        using SubToolStripItem item2 = new();
        using ToolStripDropDown control = new();
        control.Items.Add(item1);
        control.Items.Add(item2);

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.FontChanged += handler;

        // Set different.
        using Font font1 = new("Arial", 8.25f);
        control.Font = font1;
        Assert.Same(font1, control.Font);
        Assert.Same(font1, item1.Font);
        Assert.Same(font1, item2.Font);
        Assert.Equal(1, callCount);

        // Set same.
        control.Font = font1;
        Assert.Same(font1, control.Font);
        Assert.Same(font1, item1.Font);
        Assert.Same(font1, item2.Font);
        Assert.Equal(1, callCount);

        // Set different.
        using var font2 = SystemFonts.DialogFont;
        control.Font = font2;
        Assert.Same(font2, control.Font);
        Assert.Same(font2, item1.Font);
        Assert.Same(font2, item2.Font);
        Assert.Equal(2, callCount);

        // Set null.
        control.Font = null;
        Assert.Equal(Control.DefaultFont, control.Font);
        Assert.Equal(Control.DefaultFont, item1.Font);
        Assert.Equal(Control.DefaultFont, item2.Font);
        Assert.Equal(3, callCount);

        // Remove handler.
        control.FontChanged -= handler;
        control.Font = font1;
        Assert.Same(font1, control.Font);
        Assert.Same(font1, item1.Font);
        Assert.Same(font1, item2.Font);
        Assert.Equal(3, callCount);
    }

    [WinFormsFact]
    public void ToolStripDropDown_Font_SetWithItemsWithFontWithHandler_CallsFontChanged()
    {
        using Font childFont1 = new("Arial", 1);
        using Font childFont2 = new("Arial", 1);
        using SubToolStripItem child1 = new()
        {
            Font = childFont1
        };
        using SubToolStripItem child2 = new()
        {
            Font = childFont2
        };
        using ToolStripDropDown control = new();
        control.Items.Add(child1);
        control.Items.Add(child2);

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.FontChanged += handler;

        // Set different.
        using Font font1 = new("Arial", 8.25f);
        control.Font = font1;
        Assert.Same(font1, control.Font);
        Assert.Same(childFont1, child1.Font);
        Assert.Same(childFont2, child2.Font);
        Assert.Equal(1, callCount);

        // Set same.
        control.Font = font1;
        Assert.Same(font1, control.Font);
        Assert.Same(childFont1, child1.Font);
        Assert.Same(childFont2, child2.Font);
        Assert.Equal(1, callCount);

        // Set different.
        using var font2 = SystemFonts.DialogFont;
        control.Font = font2;
        Assert.Same(font2, control.Font);
        Assert.Same(childFont1, child1.Font);
        Assert.Same(childFont2, child2.Font);
        Assert.Equal(2, callCount);

        // Set null.
        control.Font = null;
        Assert.Equal(Control.DefaultFont, control.Font);
        Assert.Same(childFont1, child1.Font);
        Assert.Same(childFont2, child2.Font);
        Assert.Equal(3, callCount);

        // Remove handler.
        control.FontChanged -= handler;
        control.Font = font1;
        Assert.Same(font1, control.Font);
        Assert.Same(childFont1, child1.Font);
        Assert.Same(childFont2, child2.Font);
        Assert.Equal(3, callCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetForeColorTheoryData))]
    public void ToolStripDropDown_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using ToolStripDropDown control = new()
        {
            ForeColor = value
        };
        Assert.Equal(expected, control.ForeColor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ForeColor = value;
        Assert.Equal(expected, control.ForeColor);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripDropDown_ForeColor_SetWithHandler_CallsForeColorChanged()
    {
        using ToolStripDropDown control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.ForeColorChanged += handler;

        // Set different.
        control.ForeColor = Color.Red;
        Assert.Equal(Color.Red, control.ForeColor);
        Assert.Equal(1, callCount);

        // Set same.
        control.ForeColor = Color.Red;
        Assert.Equal(Color.Red, control.ForeColor);
        Assert.Equal(1, callCount);

        // Set different.
        control.ForeColor = Color.Empty;
        Assert.Equal(Control.DefaultForeColor, control.ForeColor);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.ForeColorChanged -= handler;
        control.ForeColor = Color.Red;
        Assert.Equal(Color.Red, control.ForeColor);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaddingTheoryData))]
    public void ToolStripDropDown_GripMargin_Set_GetReturnsExpected(Padding value)
    {
        using ToolStripDropDown control = new();
        control.GripMargin = value;
        Assert.Equal(value, control.GripMargin);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.GripMargin = value;
        Assert.Equal(value, control.GripMargin);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(ToolStripGripStyle.Hidden, 0)]
    [InlineData(ToolStripGripStyle.Visible, 1)]
    public void ToolStripDropDown_GripStyle_Set_GetReturnsExpected(ToolStripGripStyle value, int expectedLayoutCallCount)
    {
        using ToolStripDropDown control = new();
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

        Assert.Empty(control.DisplayedItems);
    }

    [WinFormsTheory]
    [InvalidEnumData<ToolStripGripStyle>]
    public void ToolStripDropDown_GripStyle_SetInvalidValue_ThrowsInvalidEnumArgumentException(ToolStripGripStyle value)
    {
        using ToolStripDropDown control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.GripStyle = value);
    }

    public static IEnumerable<object[]> ImeMode_Set_TestData()
    {
        yield return new object[] { ImeMode.Inherit, ImeMode.NoControl };
        yield return new object[] { ImeMode.NoControl, ImeMode.NoControl };
        yield return new object[] { ImeMode.On, ImeMode.On };
        yield return new object[] { ImeMode.Off, ImeMode.Off };
        yield return new object[] { ImeMode.Disable, ImeMode.Disable };
        yield return new object[] { ImeMode.Hiragana, ImeMode.Hiragana };
        yield return new object[] { ImeMode.Katakana, ImeMode.Katakana };
        yield return new object[] { ImeMode.KatakanaHalf, ImeMode.KatakanaHalf };
        yield return new object[] { ImeMode.AlphaFull, ImeMode.AlphaFull };
        yield return new object[] { ImeMode.Alpha, ImeMode.Alpha };
        yield return new object[] { ImeMode.HangulFull, ImeMode.HangulFull };
        yield return new object[] { ImeMode.Hangul, ImeMode.Hangul };
        yield return new object[] { ImeMode.Close, ImeMode.Close };
        yield return new object[] { ImeMode.OnHalf, ImeMode.On };
    }

    [WinFormsTheory]
    [MemberData(nameof(ImeMode_Set_TestData))]
    public void ToolStripDropDown_ImeMode_Set_GetReturnsExpected(ImeMode value, ImeMode expected)
    {
        using ToolStripDropDown control = new()
        {
            ImeMode = value
        };
        Assert.Equal(expected, control.ImeMode);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ImeMode = value;
        Assert.Equal(expected, control.ImeMode);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ImeMode_Set_TestData))]
    public void ToolStripDropDown_ImeMode_SetWithHandle_GetReturnsExpected(ImeMode value, ImeMode expected)
    {
        using ToolStripDropDown control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ImeMode = value;
        Assert.Equal(expected, control.ImeMode);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.ImeMode = value;
        Assert.Equal(expected, control.ImeMode);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ToolStripDropDown_ImeMode_SetWithHandler_CallsImeModeChanged()
    {
        using ToolStripDropDown control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.ImeModeChanged += handler;

        // Set different.
        control.ImeMode = ImeMode.On;
        Assert.Equal(ImeMode.On, control.ImeMode);
        Assert.Equal(1, callCount);

        // Set same.
        control.ImeMode = ImeMode.On;
        Assert.Equal(ImeMode.On, control.ImeMode);
        Assert.Equal(1, callCount);

        // Set different.
        control.ImeMode = ImeMode.Off;
        Assert.Equal(ImeMode.Off, control.ImeMode);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.ImeModeChanged -= handler;
        control.ImeMode = ImeMode.Off;
        Assert.Equal(ImeMode.Off, control.ImeMode);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<ImeMode>]
    public void ToolStripDropDown_ImeMode_SetInvalid_ThrowsInvalidEnumArgumentException(ImeMode value)
    {
        using ToolStripDropDown control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.ImeMode = value);
    }

    public static IEnumerable<object[]> Location_Set_TestData()
    {
        yield return new object[] { new Point(0, 0), new Point(0, 0), 0 };
        yield return new object[] { new Point(-1, -2), new Point(0, 0), 0 };
        yield return new object[] { new Point(1, 0), new Point(1, 0), 1 };
        yield return new object[] { new Point(0, 2), new Point(0, 2), 1 };
        yield return new object[] { new Point(1, 2), new Point(1, 2), 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Location_Set_TestData))]
    public void ToolStripDropDown_Location_Set_GetReturnsExpected(Point value, Point expected, int expectedLocationChangedCallCount)
    {
        if (value != Point.Empty)
        {
            expected.X = Math.Max(expected.X, SystemInformation.WorkingArea.X);
            expected.Y = Math.Max(expected.Y, SystemInformation.WorkingArea.Y);

            if (expectedLocationChangedCallCount == 0 && SystemInformation.WorkingArea.Location != Point.Empty)
                expectedLocationChangedCallCount = 1;
        }

        using ToolStripDropDown control = new();
        int moveCallCount = 0;
        int locationChangedCallCount = 0;
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        control.Move += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(locationChangedCallCount, moveCallCount);
            moveCallCount++;
        };
        control.LocationChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(moveCallCount - 1, locationChangedCallCount);
            locationChangedCallCount++;
        };
        control.Layout += (sender, e) => layoutCallCount++;
        control.Resize += (sender, e) => resizeCallCount++;
        control.SizeChanged += (sender, e) => sizeChangedCallCount++;
        control.ClientSizeChanged += (sender, e) => clientSizeChangedCallCount++;

        control.Location = value;
        Assert.Equal(new Size(100, 25), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, 100, 25), control.ClientRectangle);
        Assert.Equal(new Rectangle(1, 2, 98, 21), control.DisplayRectangle);
        Assert.Equal(new Size(100, 25), control.Size);
        Assert.Equal(expected.X, control.Left);
        Assert.Equal(expected.X + 100, control.Right);
        Assert.Equal(expected, control.Location);
        Assert.Equal(expected.Y, control.Top);
        Assert.Equal(expected.Y + 25, control.Bottom);
        Assert.Equal(100, control.Width);
        Assert.Equal(25, control.Height);
        Assert.Equal(new Rectangle(expected.X, expected.Y, 100, 25), control.Bounds);
        Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(0, resizeCallCount);
        Assert.Equal(0, sizeChangedCallCount);
        Assert.Equal(0, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.Location = value;
        Assert.Equal(new Size(100, 25), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, 100, 25), control.ClientRectangle);
        Assert.Equal(new Rectangle(1, 2, 98, 21), control.DisplayRectangle);
        Assert.Equal(new Size(100, 25), control.Size);
        Assert.Equal(expected.X, control.Left);
        Assert.Equal(expected.X + 100, control.Right);
        Assert.Equal(expected, control.Location);
        Assert.Equal(expected.Y, control.Top);
        Assert.Equal(expected.Y + 25, control.Bottom);
        Assert.Equal(100, control.Width);
        Assert.Equal(25, control.Height);
        Assert.Equal(new Rectangle(expected.X, expected.Y, 100, 25), control.Bounds);
        Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(0, resizeCallCount);
        Assert.Equal(0, sizeChangedCallCount);
        Assert.Equal(0, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Opacity_Set_TestData()
    {
        foreach (bool allowTransparency in new bool[] { true, false })
        {
            yield return new object[] { allowTransparency, 1.1, 1.0, allowTransparency };
            yield return new object[] { allowTransparency, 1.0, 1.0, allowTransparency };
            yield return new object[] { allowTransparency, 0.5, 0.5, true };
            yield return new object[] { allowTransparency, 0, 0, true };
            yield return new object[] { allowTransparency, -0.1, 0, true };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Opacity_Set_TestData))]
    public void ToolStripDropDown_Opacity_Set_GetReturnsExpected(bool allowTransparency, double value, double expected, bool expectedAllowTransparency)
    {
        using ToolStripDropDown control = new()
        {
            AllowTransparency = allowTransparency,
            Opacity = value
        };
        Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
        Assert.Equal(expected, control.Opacity);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Opacity = value;
        Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
        Assert.Equal(expected, control.Opacity);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Opacity_Set_TestData))]
    public void ToolStripDropDown_Opacity_SetTopLevel_GetReturnsExpected(bool allowTransparency, double value, double expected, bool expectedAllowTransparency)
    {
        using ToolStripDropDown control = new()
        {
            TopLevel = false,
            AllowTransparency = allowTransparency,
            Opacity = value
        };
        Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
        Assert.Equal(expected, control.Opacity);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Opacity = value;
        Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
        Assert.Equal(expected, control.Opacity);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Opacity_SetNotTransparentWithHandle_TestData()
    {
        yield return new object[] { true, 1.1, 1.0, 1 };
        yield return new object[] { true, 1.0, 1.0, 1 };
        yield return new object[] { false, 1.1, 1.0, 0 };
        yield return new object[] { false, 1.0, 1.0, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Opacity_SetNotTransparentWithHandle_TestData))]
    public void ToolStripDropDown_Opacity_SetNotTransparentWithHandle_GetReturnsExpected(bool allowTransparency, double value, double expected, int expectedStyleChangedCallCount)
    {
        using ToolStripDropDown control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.AllowTransparency = allowTransparency;
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Opacity = value;
        Assert.Equal(allowTransparency, control.AllowTransparency);
        Assert.Equal(expected, control.Opacity);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Opacity = value;
        Assert.Equal(allowTransparency, control.AllowTransparency);
        Assert.Equal(expected, control.Opacity);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(0.5, 0.5, 0)]
    [InlineData(0, 0, 0)]
    [InlineData(-0.1, 0, 0)]
    public void ToolStripDropDown_Opacity_SetTransparentWithHandleSetAllowTransparencyBefore_GetReturnsExpected(float value, float expected, int expectedStyleChangedCallCount)
    {
        using ToolStripDropDown control = new()
        {
            AllowTransparency = true
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Opacity = value;
        Assert.True(control.AllowTransparency);
        Assert.Equal(expected, control.Opacity);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        control.Opacity = value;
        Assert.True(control.AllowTransparency);
        Assert.Equal(expected, control.Opacity);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> Opacity_SetTransparentWithHandle_TestData()
    {
        yield return new object[] { true, 0.5, 0.5, 0 };
        yield return new object[] { true, 0, 0, 0 };
        yield return new object[] { true, -0.1, -0, 0 };

        yield return new object[] { false, 0.5, 0.5, 2 };
        yield return new object[] { false, 0, 0, 2 };
        yield return new object[] { false, -0.1, -0, 2 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Opacity_SetTransparentWithHandle_TestData))]
    public void ToolStripDropDown_Opacity_SetTransparentWithHandleSetAllowTransparencyAfter_GetReturnsExpected(bool allowTransparency, float value, float expected, int expectedStyleChangedCallCount)
    {
        using ToolStripDropDown control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.AllowTransparency = allowTransparency;
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Opacity = value;
        Assert.True(control.AllowTransparency);
        Assert.Equal(expected, control.Opacity);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        control.Opacity = value;
        Assert.True(control.AllowTransparency);
        Assert.Equal(expected, control.Opacity);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> Opacity_SetTopLevelWithHandle_TestData()
    {
        yield return new object[] { true, 1.1, 1.0, true, 1 };
        yield return new object[] { true, 1.0, 1.0, true, 1 };
        yield return new object[] { true, 0.5, 0.5, true, 0 };
        yield return new object[] { true, 0, 0, true, 0 };
        yield return new object[] { true, -0.1, 0, true, 0 };

        yield return new object[] { false, 1.1, 1.0, false, 0 };
        yield return new object[] { false, 1.0, 1.0, false, 0 };
        yield return new object[] { false, 0.5, 0.5, true, 2 };
        yield return new object[] { false, 0, 0, true, 2 };
        yield return new object[] { false, -0.1, 0, true, 2 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Opacity_SetTopLevelWithHandle_TestData))]
    public void ToolStripDropDown_Opacity_SetTopLevelWithHandle_GetReturnsExpected(bool allowTransparency, double value, double expected, bool expectedAllowTransparency, int expectedStyleChangedCallCount)
    {
        using ToolStripDropDown control = new()
        {
            TopLevel = false,
            AllowTransparency = allowTransparency
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Opacity = value;
        Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
        Assert.Equal(expected, control.Opacity);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Opacity = value;
        Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
        Assert.Equal(expected, control.Opacity);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> Region_Set_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new Region() };
        yield return new object[] { new Region(new Rectangle(1, 2, 3, 4)) };
    }

    [WinFormsTheory]
    [MemberData(nameof(Region_Set_TestData))]
    public void ToolStripDropDown_Region_Set_GetReturnsExpected(Region value)
    {
        using ToolStripDropDown control = new()
        {
            Region = value
        };
        Assert.Same(value, control.Region);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Region = value;
        Assert.Same(value, control.Region);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Region_Set_TestData))]
    public void ToolStripDropDown_Region_SetWithHandle_GetReturnsExpected(Region value)
    {
        using ToolStripDropDown control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Region = value;
        Assert.Same(value, control.Region);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Region = value;
        Assert.Same(value, control.Region);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ToolStripDropDown_Region_SetWithHandler_CallsRegionChanged()
    {
        using ToolStripDropDown control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.RegionChanged += handler;

        // Set different.
        using Region region1 = new();
        control.Region = region1;
        Assert.Same(region1, control.Region);
        Assert.Equal(1, callCount);

        // Set same.
        control.Region = region1;
        Assert.Same(region1, control.Region);
        Assert.Equal(1, callCount);

        // Set different.
        using Region region2 = new();
        control.Region = region2;
        Assert.Same(region2, control.Region);
        Assert.Equal(2, callCount);

        // Set null.
        control.Region = null;
        Assert.False(control.RecreatingHandle);
        Assert.Null(control.Region);
        Assert.Equal(3, callCount);

        // Remove handler.
        control.RegionChanged -= handler;
        control.Region = region1;
        Assert.Same(region1, control.Region);
        Assert.Equal(3, callCount);
    }

    [WinFormsFact]
    public void ToolStripDropDown_RightToLeft_GetWithSourceToolStripDropDown_ReturnsExpected()
    {
        using Control sourceControl = new()
        {
            RightToLeft = RightToLeft.Yes
        };
        using ToolStripDropDown control = new();
        control.Show(sourceControl, Point.Empty);
        Assert.Equal(RightToLeft.Yes, control.RightToLeft);

        control.RightToLeft = RightToLeft.No;
        Assert.Equal(RightToLeft.No, control.RightToLeft);
    }

    [WinFormsFact]
    public void ToolStripDropDown_RightToLeft_GetWithOwnerItem_ReturnsExpected()
    {
        using SubToolStripItem ownerItem = new()
        {
            RightToLeft = RightToLeft.Yes
        };
        using ToolStripDropDown control = new()
        {
            OwnerItem = ownerItem
        };
        Assert.Equal(RightToLeft.Yes, control.RightToLeft);

        control.RightToLeft = RightToLeft.No;
        Assert.Equal(RightToLeft.No, control.RightToLeft);
    }

    [WinFormsFact]
    public void ToolStripDropDown_RightToLeft_GetWithSourceControlAndOwnerItem_ReturnsExpected()
    {
        using Control sourceControl = new()
        {
            RightToLeft = RightToLeft.Yes
        };
        using SubToolStripItem ownerItem = new()
        {
            RightToLeft = RightToLeft.No
        };
        using ToolStripDropDown control = new()
        {
            OwnerItem = ownerItem
        };
        control.Show(sourceControl, Point.Empty);
        Assert.Equal(RightToLeft.Yes, control.RightToLeft);
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.Yes, RightToLeft.Yes, 1)]
    [InlineData(RightToLeft.No, RightToLeft.No, 0)]
    [InlineData(RightToLeft.Inherit, RightToLeft.No, 0)]
    public void ToolStripDropDown_RightToLeft_Set_GetReturnsExpected(RightToLeft value, RightToLeft expected, int expectedLayoutCallCount)
    {
        using ToolStripDropDown control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("RightToLeft", e.AffectedProperty);
            layoutCallCount++;
        };

        control.RightToLeft = value;
        Assert.Equal(expected, control.RightToLeft);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.RightToLeft = value;
        Assert.Equal(expected, control.RightToLeft);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripDropDown_RightToLeft_SetWithHandler_CallsRightToLeftChanged()
    {
        using ToolStripDropDown control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.RightToLeftChanged += handler;

        // Set different.
        control.RightToLeft = RightToLeft.Yes;
        Assert.Equal(RightToLeft.Yes, control.RightToLeft);
        Assert.Equal(1, callCount);

        // Set same.
        control.RightToLeft = RightToLeft.Yes;
        Assert.Equal(RightToLeft.Yes, control.RightToLeft);
        Assert.Equal(1, callCount);

        // Set different.
        control.RightToLeft = RightToLeft.Inherit;
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.RightToLeftChanged -= handler;
        control.RightToLeft = RightToLeft.Yes;
        Assert.Equal(RightToLeft.Yes, control.RightToLeft);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<RightToLeft>]
    public void ToolStripDropDown_RightToLeft_SetInvalid_ThrowsInvalidEnumArgumentException(RightToLeft value)
    {
        using ToolStripDropDown control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.RightToLeft = value);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripDropDown_Stretch_Set_GetReturnsExpected(bool value)
    {
        using ToolStripDropDown control = new()
        {
            Stretch = value
        };
        Assert.Equal(value, control.Stretch);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Stretch = value;
        Assert.Equal(value, control.Stretch);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.Stretch = value;
        Assert.Equal(value, control.Stretch);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripDropDown_Stretch_SetWithHandle_GetReturnsExpected(bool value)
    {
        using ToolStripDropDown control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Stretch = value;
        Assert.Equal(value, control.Stretch);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Stretch = value;
        Assert.Equal(value, control.Stretch);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.Stretch = value;
        Assert.Equal(value, control.Stretch);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void ToolStripDropDown_TabIndex_Set_GetReturnsExpected(int value)
    {
        using ToolStripDropDown control = new()
        {
            TabIndex = value
        };
        Assert.Equal(value, control.TabIndex);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.TabIndex = value;
        Assert.Equal(value, control.TabIndex);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripDropDown_TabIndex_SetWithHandler_CallsTabIndexChanged()
    {
        using ToolStripDropDown control = new()
        {
            TabIndex = 0
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.TabIndexChanged += handler;

        // Set different.
        control.TabIndex = 1;
        Assert.Equal(1, control.TabIndex);
        Assert.Equal(1, callCount);

        // Set same.
        control.TabIndex = 1;
        Assert.Equal(1, control.TabIndex);
        Assert.Equal(1, callCount);

        // Set different.
        control.TabIndex = 2;
        Assert.Equal(2, control.TabIndex);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.TabIndexChanged -= handler;
        control.TabIndex = 1;
        Assert.Equal(1, control.TabIndex);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void ToolStripDropDown_TabIndex_SetNegative_CallsArgumentOutOfRangeException()
    {
        using ToolStripDropDown control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.TabIndex = -1);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripDropDown_TabStop_Set_GetReturnsExpected(bool value)
    {
        using ToolStripDropDown control = new()
        {
            TabStop = value
        };
        Assert.Equal(value, control.TabStop);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.TabStop = value;
        Assert.Equal(value, control.TabStop);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.TabStop = value;
        Assert.Equal(value, control.TabStop);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripDropDown_TabStop_SetWithHandle_GetReturnsExpected(bool value)
    {
        using ToolStripDropDown control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.TabStop = value;
        Assert.Equal(value, control.TabStop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.TabStop = value;
        Assert.Equal(value, control.TabStop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.TabStop = value;
        Assert.Equal(value, control.TabStop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ToolStripDropDown_TabStop_SetWithHandler_CallsTabStopChanged()
    {
        using ToolStripDropDown control = new()
        {
            TabStop = true
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.TabStopChanged += handler;

        // Set different.
        control.TabStop = false;
        Assert.False(control.TabStop);
        Assert.Equal(1, callCount);

        // Set same.
        control.TabStop = false;
        Assert.False(control.TabStop);
        Assert.Equal(1, callCount);

        // Set different.
        control.TabStop = true;
        Assert.True(control.TabStop);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.TabStopChanged -= handler;
        control.TabStop = false;
        Assert.False(control.TabStop);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ToolStripDropDown_Text_Set_GetReturnsExpected(string value, string expected)
    {
        using ToolStripDropDown control = new()
        {
            Text = value
        };
        Assert.Equal(expected, control.Text);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ToolStripDropDown_Text_SetWithHandle_GetReturnsExpected(string value, string expected)
    {
        using ToolStripDropDown control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ToolStripDropDown_Text_SetWithHandler_CallsTextChanged()
    {
        using ToolStripDropDown control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(EventArgs.Empty, e);
            callCount++;
        };
        control.TextChanged += handler;

        // Set different.
        control.Text = "text";
        Assert.Equal("text", control.Text);
        Assert.Equal(1, callCount);

        // Set same.
        control.Text = "text";
        Assert.Equal("text", control.Text);
        Assert.Equal(1, callCount);

        // Set different.
        control.Text = null;
        Assert.Empty(control.Text);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.TextChanged -= handler;
        control.Text = "text";
        Assert.Equal("text", control.Text);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [InlineData(ToolStripTextDirection.Inherit, ToolStripTextDirection.Horizontal, 1)]
    [InlineData(ToolStripTextDirection.Horizontal, ToolStripTextDirection.Horizontal, 1)]
    [InlineData(ToolStripTextDirection.Vertical90, ToolStripTextDirection.Vertical90, 1)]
    [InlineData(ToolStripTextDirection.Vertical270, ToolStripTextDirection.Vertical270, 1)]
    public void ToolStripDropDown_TextDirection_Set_GetReturnsExpected(ToolStripTextDirection value, ToolStripTextDirection expected, int expectedLayoutCallCount)
    {
        using ToolStripDropDown control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("TextDirection", e.AffectedProperty);
            layoutCallCount++;
        };

        control.TextDirection = value;
        Assert.Equal(expected, control.TextDirection);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.TextDirection = value;
        Assert.Equal(expected, control.TextDirection);
        Assert.Equal(expectedLayoutCallCount + 1, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(ToolStripTextDirection.Inherit, ToolStripTextDirection.Horizontal, 1)]
    [InlineData(ToolStripTextDirection.Horizontal, ToolStripTextDirection.Horizontal, 1)]
    [InlineData(ToolStripTextDirection.Vertical90, ToolStripTextDirection.Vertical90, 1)]
    [InlineData(ToolStripTextDirection.Vertical270, ToolStripTextDirection.Vertical270, 1)]
    public void ToolStripDropDown_TextDirection_SetWithHandle_GetReturnsExpected(ToolStripTextDirection value, ToolStripTextDirection expected, int expectedLayoutCallCount)
    {
        using ToolStripDropDown control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("TextDirection", e.AffectedProperty);
            layoutCallCount++;
        };

        control.TextDirection = value;
        Assert.Equal(expected, control.TextDirection);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedLayoutCallCount * 2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.TextDirection = value;
        Assert.Equal(expected, control.TextDirection);
        Assert.Equal(expectedLayoutCallCount + 1, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedLayoutCallCount * 2 + 1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(true, true, 0)]
    [InlineData(true, false, 1)]
    [InlineData(false, true, 0)]
    [InlineData(false, false, 1)]
    public void ToolStripDropDown_TopLevel_Set_GetReturnsExpected(bool visible, bool value, int expectedStyleChangedCallCount1)
    {
        using SubToolStripDropDown control = new()
        {
            Visible = visible
        };
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.TopLevel = value;
        Assert.Equal(value, control.TopLevel);
        Assert.Equal(value, control.GetTopLevel());
        Assert.False(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount1, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.TopLevel = value;
        Assert.Equal(value, control.TopLevel);
        Assert.Equal(value, control.GetTopLevel());
        Assert.False(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount1, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.TopLevel = !value;
        Assert.Equal(!value, control.TopLevel);
        Assert.Equal(!value, control.GetTopLevel());
        Assert.False(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount1 + 1, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(true, true, 0, 1, 0)]
    [InlineData(false, false, 1, 3, 1)]
    public void ToolStripDropDown_TopLevel_SetWithHandle_GetReturnsExpected(bool visible, bool value, int expectedStyleChangedCallCount1, int expectedStyleChangedCallCount2, int expectedCreatedCallCount)
    {
        using SubToolStripDropDown control = new()
        {
            Visible = visible
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.TopLevel = value;
        Assert.Equal(value, control.TopLevel);
        Assert.Equal(value, control.GetTopLevel());
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount1, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount1, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.TopLevel = value;
        Assert.Equal(value, control.TopLevel);
        Assert.Equal(value, control.GetTopLevel());
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount1, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount1, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.TopLevel = !value;
        Assert.Equal(!value, control.TopLevel);
        Assert.Equal(!value, control.GetTopLevel());
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount2, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount2, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);
    }

    [WinFormsFact]
    public void ToolStripDropDown_TopLevel_SetWithParent_ThrowsArgumentException()
    {
        using Control parent = new();
        using SubToolStripDropDown control = new()
        {
            TopLevel = false,
            Parent = parent
        };
        Assert.Throws<ArgumentException>("value", () => control.TopLevel = true);
        control.TopLevel = false;
        Assert.False(control.GetTopLevel());
    }

    [WinFormsFact]
    public void ToolStripDropDown_CreateAccessibilityInstance_Invoke_ReturnsExpected()
    {
        using SubToolStripDropDown control = new();
        ToolStripDropDown.ToolStripDropDownAccessibleObject instance = Assert.IsType<ToolStripDropDown.ToolStripDropDownAccessibleObject>(control.CreateAccessibilityInstance());
        Assert.NotNull(instance);
        Assert.Same(control, instance.Owner);
        Assert.Equal(AccessibleRole.MenuPopup, instance.Role);
        Assert.NotSame(control.CreateAccessibilityInstance(), instance);
        Assert.NotSame(control.AccessibilityObject, instance);
    }

    [WinFormsFact]
    public void ToolStripDropDown_CreateAccessibilityInstance_InvokeWithCustomRole_ReturnsExpected()
    {
        using SubToolStripDropDown control = new()
        {
            AccessibleRole = AccessibleRole.HelpBalloon
        };
        ToolStripDropDown.ToolStripDropDownAccessibleObject instance = Assert.IsType<ToolStripDropDown.ToolStripDropDownAccessibleObject>(control.CreateAccessibilityInstance());
        Assert.NotNull(instance);
        Assert.Same(control, instance.Owner);
        Assert.Equal(AccessibleRole.HelpBalloon, instance.Role);
        Assert.NotSame(control.CreateAccessibilityInstance(), instance);
        Assert.NotSame(control.AccessibilityObject, instance);
    }

    public static IEnumerable<object[]> CreateDefaultItem_Button_TestData()
    {
        EventHandler onClick = (sender, e) => { };

        yield return new object[] { null, null, null };
        yield return new object[] { string.Empty, new Bitmap(10, 10), onClick };
        yield return new object[] { "text", new Bitmap(10, 10), onClick };
    }

    [WinFormsTheory]
    [MemberData(nameof(CreateDefaultItem_Button_TestData))]
    public void ToolStripDropDown_CreateDefaultItem_InvokeButton_Success(string text, Image image, EventHandler onClick)
    {
        using SubToolStripDropDown control = new();
        ToolStripButton button = Assert.IsType<ToolStripButton>(control.CreateDefaultItem(text, image, onClick));
        Assert.Equal(text, button.Text);
        Assert.Same(image, button.Image);
    }

    public static IEnumerable<object[]> CreateDefaultItem_Separator_TestData()
    {
        EventHandler onClick = (sender, e) => { };

        yield return new object[] { null, null };
        yield return new object[] { new Bitmap(10, 10), onClick };
    }

    [WinFormsTheory]
    [MemberData(nameof(CreateDefaultItem_Separator_TestData))]
    public void ToolStripDropDown_CreateDefaultItem_InvokeSeparator_Success(Image image, EventHandler onClick)
    {
        using SubToolStripDropDown control = new();
        ToolStripSeparator separator = Assert.IsType<ToolStripSeparator>(control.CreateDefaultItem("-", image, onClick));
        Assert.Empty(separator.Text);
        Assert.Null(separator.Image);
    }

    [WinFormsTheory]
    [InlineData(null, 1)]
    [InlineData("", 1)]
    [InlineData("text", 1)]
    [InlineData("-", 0)]
    public void ToolStripDropDown_CreateDefaultItem_PerformClick_Success(string text, int expectedCallCount)
    {
        int callCount = 0;
        EventHandler onClick = (sender, e) => callCount++;
        using SubToolStripDropDown control = new();
        ToolStripItem button = Assert.IsAssignableFrom<ToolStripItem>(control.CreateDefaultItem(text, null, onClick));
        button.PerformClick();
        Assert.Equal(expectedCallCount, callCount);
    }

    [WinFormsFact]
    public void ToolStripDropDown_CreateLayoutSettings_InvokeFlow_ReturnsExpected()
    {
        using SubToolStripDropDown toolStrip = new();
        FlowLayoutSettings settings = Assert.IsType<FlowLayoutSettings>(toolStrip.CreateLayoutSettings(ToolStripLayoutStyle.Flow));
        Assert.Equal(FlowDirection.TopDown, settings.FlowDirection);
        Assert.NotNull(settings.LayoutEngine);
        Assert.False(settings.WrapContents);
        Assert.Same(settings.LayoutEngine, settings.LayoutEngine);
    }

    [WinFormsFact]
    public void ToolStripDropDown_CreateLayoutSettings_InvokeTable_ReturnsExpected()
    {
        SubToolStripDropDown toolStrip = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.CreateLayoutSettings(ToolStripLayoutStyle.Table));
        Assert.Equal(0, settings.ColumnCount);
        Assert.Empty(settings.ColumnStyles);
        Assert.Equal(TableLayoutPanelGrowStyle.AddRows, settings.GrowStyle);
        Assert.NotNull(settings.LayoutEngine);
        Assert.Same(settings.LayoutEngine, settings.LayoutEngine);
        Assert.Equal(0, settings.RowCount);
        Assert.Empty(settings.RowStyles);
    }

    [WinFormsTheory]
    [InvalidEnumData<ToolStripLayoutStyle>]
    [InlineData(ToolStripLayoutStyle.StackWithOverflow)]
    [InlineData(ToolStripLayoutStyle.HorizontalStackWithOverflow)]
    [InlineData(ToolStripLayoutStyle.VerticalStackWithOverflow)]
    public void ToolStripDropDown_CreateLayoutSettings_InvalidLayoutStyle_ReturnsNull(ToolStripLayoutStyle layoutStyle)
    {
        SubToolStripDropDown toolStrip = new();
        Assert.Null(toolStrip.CreateLayoutSettings(layoutStyle));
    }

    [WinFormsFact]
    public void ToolStripDropDown_Dispose_Invoke_Success()
    {
        using ToolStripDropDown control = new();
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.Items);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.Disposing);
            Assert.False(control.Visible);
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
            Assert.False(control.Visible);
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
            Assert.False(control.Visible);
            Assert.Equal(2, callCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void ToolStripDropDown_Dispose_InvokeWithSourceControl_Success()
    {
        using Control sourceControl = new()
        {
            RightToLeft = RightToLeft.Yes
        };
        using ToolStripDropDown control = new();
        control.Show(sourceControl, Point.Empty);
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.Items);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.Disposing);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.False(control.Visible);
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
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.False(control.Visible);
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
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.False(control.Visible);
            Assert.Equal(2, callCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void ToolStripDropDown_Dispose_InvokeNotVisible_Success()
    {
        using ToolStripDropDown control = new()
        {
            Visible = false
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
            Assert.False(control.Visible);
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
            Assert.False(control.Visible);
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
            Assert.False(control.Visible);
            Assert.Equal(2, callCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void ToolStripDropDown_Dispose_InvokeWithItems_Success()
    {
        using ToolStripDropDown control = new();
        using SubToolStripItem item1 = new();
        using SubToolStripItem item2 = new();
        control.Items.Add(item1);
        control.Items.Add(item2);
        int itemRemovedCallCount = 0;
        control.ItemRemoved += (sender, e) => itemRemovedCallCount++;
        int controlRemovedCallCount = 0;
        control.ControlRemoved += (sender, e) => controlRemovedCallCount++;

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.Items);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.Disposing);
            Assert.False(control.Visible);
            Assert.Equal(callCount > 0, control.IsDisposed);
            callCount++;
        }

        control.Disposed += handler;
        int item1CallCount = 0;
        item1.Disposed += (sender, e) => item1CallCount++;
        int item2CallCount = 0;
        item2.Disposed += (sender, e) => item2CallCount++;

        try
        {
            control.Dispose();
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.Items);
            Assert.Empty(control.DataBindings);
            Assert.Null(item1.Owner);
            Assert.Null(item2.Owner);
            Assert.Equal(0, controlRemovedCallCount);
            Assert.Equal(0, itemRemovedCallCount);
            Assert.True(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.False(control.Visible);
            Assert.True(item1.IsDisposed);
            Assert.True(item2.IsDisposed);
            Assert.Equal(1, callCount);
            Assert.Equal(1, item1CallCount);
            Assert.Equal(1, item2CallCount);
            Assert.False(control.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose();
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.Items);
            Assert.Empty(control.DataBindings);
            Assert.Null(item1.Owner);
            Assert.Null(item2.Owner);
            Assert.Equal(0, controlRemovedCallCount);
            Assert.Equal(0, itemRemovedCallCount);
            Assert.True(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.False(control.Visible);
            Assert.True(item1.IsDisposed);
            Assert.True(item2.IsDisposed);
            Assert.Equal(2, callCount);
            Assert.Equal(1, item1CallCount);
            Assert.Equal(1, item2CallCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void ToolStripDropDown_Dispose_InvokeDisposing_Success()
    {
        using SubToolStripDropDown control = new();
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.Items);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.Disposing);
            Assert.False(control.Visible);
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
            Assert.False(control.Visible);
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
            Assert.False(control.Visible);
            Assert.Equal(2, callCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void ToolStripDropDown_Dispose_InvokeNotDisposing_Success()
    {
        using SubToolStripDropDown control = new();
        int callCount = 0;
        void handler(object sender, EventArgs e) => callCount++;
        control.Disposed += handler;

        try
        {
            control.Dispose(false);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.Items);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose(false);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
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

    [WinFormsFact]
    public void ToolStripDropDown_Dispose_InvokeDisposingWithSourceControl_Success()
    {
        using Control sourceControl = new()
        {
            RightToLeft = RightToLeft.Yes
        };
        using SubToolStripDropDown control = new();
        control.Show(sourceControl, Point.Empty);
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.Items);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.Disposing);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.False(control.Visible);
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
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.False(control.Visible);
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
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.False(control.Visible);
            Assert.Equal(2, callCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void ToolStripDropDown_Dispose_InvokeNotDisposingWithSourceControl_Success()
    {
        using Control sourceControl = new()
        {
            RightToLeft = RightToLeft.Yes
        };
        using SubToolStripDropDown control = new();
        control.Show(sourceControl, Point.Empty);
        int callCount = 0;
        void handler(object sender, EventArgs e) => callCount++;
        control.Disposed += handler;

        try
        {
            control.Dispose(false);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.Items);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Equal(RightToLeft.Yes, control.RightToLeft);
            Assert.False(control.Visible);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose(false);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.Items);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Equal(RightToLeft.Yes, control.RightToLeft);
            Assert.False(control.Visible);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void ToolStripDropDown_Dispose_InvokeDisposingWithItems_Success()
    {
        using SubToolStripDropDown control = new();
        using SubToolStripItem item1 = new();
        using SubToolStripItem item2 = new();
        control.Items.Add(item1);
        control.Items.Add(item2);
        int itemRemovedCallCount = 0;
        control.ItemRemoved += (sender, e) => itemRemovedCallCount++;
        int controlRemovedCallCount = 0;
        control.ControlRemoved += (sender, e) => controlRemovedCallCount++;

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.Items);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.Disposing);
            Assert.False(control.Visible);
            Assert.Equal(callCount > 0, control.IsDisposed);
            callCount++;
        }

        control.Disposed += handler;
        int item1CallCount = 0;
        item1.Disposed += (sender, e) => item1CallCount++;
        int item2CallCount = 0;
        item2.Disposed += (sender, e) => item2CallCount++;

        try
        {
            control.Dispose(true);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.Items);
            Assert.Empty(control.DataBindings);
            Assert.Null(item1.Owner);
            Assert.Null(item2.Owner);
            Assert.Equal(0, controlRemovedCallCount);
            Assert.Equal(0, itemRemovedCallCount);
            Assert.True(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.False(control.Visible);
            Assert.True(item1.IsDisposed);
            Assert.True(item2.IsDisposed);
            Assert.Equal(1, callCount);
            Assert.Equal(1, item1CallCount);
            Assert.Equal(1, item2CallCount);
            Assert.False(control.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose(true);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.Items);
            Assert.Empty(control.DataBindings);
            Assert.Null(item1.Owner);
            Assert.Null(item2.Owner);
            Assert.Equal(0, controlRemovedCallCount);
            Assert.Equal(0, itemRemovedCallCount);
            Assert.True(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.False(control.Visible);
            Assert.True(item1.IsDisposed);
            Assert.True(item2.IsDisposed);
            Assert.False(control.Visible);
            Assert.Equal(2, callCount);
            Assert.Equal(1, item1CallCount);
            Assert.Equal(1, item2CallCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void ToolStripDropDown_Dispose_InvokeNotDisposingWithItems_Success()
    {
        using SubToolStripDropDown control = new();
        using SubToolStripItem item1 = new();
        using SubToolStripItem item2 = new();
        control.Items.Add(item1);
        control.Items.Add(item2);
        int itemRemovedCallCount = 0;
        control.ItemRemoved += (sender, e) => itemRemovedCallCount++;
        int controlRemovedCallCount = 0;
        control.ControlRemoved += (sender, e) => controlRemovedCallCount++;

        int callCount = 0;
        void handler(object sender, EventArgs e) => callCount++;
        control.Disposed += handler;
        int item1CallCount = 0;
        item1.Disposed += (sender, e) => item1CallCount++;
        int item2CallCount = 0;
        item2.Disposed += (sender, e) => item2CallCount++;

        try
        {
            control.Dispose(false);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Equal(new ToolStripItem[] { item1, item2 }, control.Items.Cast<ToolStripItem>());
            Assert.Empty(control.DataBindings);
            Assert.Same(control, item1.Owner);
            Assert.Same(control, item2.Owner);
            Assert.Equal(0, controlRemovedCallCount);
            Assert.Equal(0, itemRemovedCallCount);
            Assert.False(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.False(item1.IsDisposed);
            Assert.False(item2.IsDisposed);
            Assert.Equal(0, callCount);
            Assert.Equal(0, item1CallCount);
            Assert.Equal(0, item2CallCount);
            Assert.False(control.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose(false);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Equal(new ToolStripItem[] { item1, item2 }, control.Items.Cast<ToolStripItem>());
            Assert.Empty(control.DataBindings);
            Assert.Same(control, item1.Owner);
            Assert.Same(control, item2.Owner);
            Assert.Equal(0, controlRemovedCallCount);
            Assert.Equal(0, itemRemovedCallCount);
            Assert.False(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.False(item1.IsDisposed);
            Assert.False(item2.IsDisposed);
            Assert.Equal(0, callCount);
            Assert.Equal(0, item1CallCount);
            Assert.Equal(0, item2CallCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void ToolStripDropDown_GetAutoSizeMode_Invoke_ReturnsExpected()
    {
        using SubToolStripDropDown control = new();
        Assert.Equal(AutoSizeMode.GrowAndShrink, control.GetAutoSizeMode());
    }

    [WinFormsTheory]
    [InlineData(0, true)]
    [InlineData(SubToolStripDropDown.ScrollStateAutoScrolling, false)]
    [InlineData(SubToolStripDropDown.ScrollStateFullDrag, false)]
    [InlineData(SubToolStripDropDown.ScrollStateHScrollVisible, false)]
    [InlineData(SubToolStripDropDown.ScrollStateUserHasScrolled, false)]
    [InlineData(SubToolStripDropDown.ScrollStateVScrollVisible, false)]
    [InlineData(int.MaxValue, false)]
    [InlineData((-1), false)]
    public void ToolStripDropDown_GetScrollState_Invoke_ReturnsExpected(int bit, bool expected)
    {
        using SubToolStripDropDown control = new();
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
    public void ToolStripDropDown_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
    {
        using SubToolStripDropDown control = new();
        Assert.Equal(expected, control.GetStyle(flag));

        // Call again to test caching.
        Assert.Equal(expected, control.GetStyle(flag));
    }

    [WinFormsFact]
    public void ToolStripDropDown_GetTopLevel_Invoke_ReturnsExpected()
    {
        using SubToolStripDropDown control = new();
        Assert.True(control.GetTopLevel());
    }

    public static IEnumerable<object[]> UICuesEventArgs_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new UICuesEventArgs(UICues.None) };
        yield return new object[] { new UICuesEventArgs(UICues.Changed) };
    }

    [WinFormsTheory]
    [MemberData(nameof(UICuesEventArgs_TestData))]
    public void ToolStripDropDown_OnChangeUICues_Invoke_CallsChangeUICues(UICuesEventArgs eventArgs)
    {
        using SubToolStripDropDown control = new();
        int callCount = 0;
        UICuesEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.ChangeUICues += handler;
        control.OnChangeUICues(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.ChangeUICues -= handler;
        control.OnChangeUICues(eventArgs);
        Assert.Equal(1, callCount);
    }

    public static IEnumerable<object[]> OnClosed_TestData()
    {
        yield return new object[] { null, null };
        yield return new object[] { null, new ToolStripDropDownClosedEventArgs(ToolStripDropDownCloseReason.AppFocusChange) };
        yield return new object[] { new SubToolStripItem(), null };
        yield return new object[] { new SubToolStripItem(), new ToolStripDropDownClosedEventArgs(ToolStripDropDownCloseReason.AppFocusChange) };

        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        SubToolStripItem designModeOwnerItem = new()
        {
            Site = mockSite.Object
        };
        yield return new object[] { designModeOwnerItem, null };
        yield return new object[] { designModeOwnerItem, new ToolStripDropDownClosedEventArgs(ToolStripDropDownCloseReason.AppFocusChange) };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnClosed_TestData))]
    public void ToolStripDropDown_OnClosed_Invoke_CallsClosed(ToolStripItem ownerItem, ToolStripDropDownClosedEventArgs eventArgs)
    {
        using SubToolStripDropDown control = new()
        {
            OwnerItem = ownerItem
        };
        int callCount = 0;
        ToolStripDropDownClosedEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.Closed += handler;
        control.OnClosed(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.Closed -= handler;
        control.OnClosed(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnClosed_TestData))]
    public void ToolStripDropDown_OnClosed_InvokeWithHandle_CallsClosed(ToolStripItem ownerItem, ToolStripDropDownClosedEventArgs eventArgs)
    {
        using SubToolStripDropDown control = new()
        {
            OwnerItem = ownerItem
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int callCount = 0;
        ToolStripDropDownClosedEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.Closed += handler;
        control.OnClosed(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.Closed -= handler;
        control.OnClosed(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> OnClosing_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new ToolStripDropDownClosingEventArgs(ToolStripDropDownCloseReason.AppFocusChange) };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnClosing_TestData))]
    public void ToolStripDropDown_OnClosing_Invoke_CallsClosing(ToolStripDropDownClosingEventArgs eventArgs)
    {
        using SubToolStripDropDown control = new();
        int callCount = 0;
        void handler(object sender, ToolStripDropDownClosingEventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        }

        // Call with handler.
        control.Closing += handler;
        control.OnClosing(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.Closing -= handler;
        control.OnClosing(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripDropDown_OnEnter_Invoke_CallsEnter(EventArgs eventArgs)
    {
        using SubToolStripDropDown control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.Enter += handler;
        control.OnEnter(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.Enter -= handler;
        control.OnEnter(eventArgs);
        Assert.Equal(1, callCount);
    }

    public static IEnumerable<object[]> GiveFeedbackEventArgs_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new GiveFeedbackEventArgs(DragDropEffects.None, true) };
    }

    [WinFormsTheory]
    [MemberData(nameof(GiveFeedbackEventArgs_TestData))]
    public void ToolStripDropDown_OnGiveFeedback_Invoke_CallsGiveFeedback(GiveFeedbackEventArgs eventArgs)
    {
        using SubToolStripDropDown control = new();
        int callCount = 0;
        GiveFeedbackEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.GiveFeedback += handler;
        control.OnGiveFeedback(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.GiveFeedback -= handler;
        control.OnGiveFeedback(eventArgs);
        Assert.Equal(1, callCount);
    }

    public static IEnumerable<object[]> OnHandleCreated_TestData()
    {
        foreach (bool topLevel in new bool[] { true, false })
        {
            yield return new object[] { topLevel, 1, null };
            yield return new object[] { topLevel, 1, new EventArgs() };
            yield return new object[] { topLevel, 0.5, null };
            yield return new object[] { topLevel, 0.5, new EventArgs() };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnHandleCreated_TestData))]
    public void ToolStripDropDown_OnHandleCreated_Invoke_CallsHandleCreated(bool topLevel, float opacity, EventArgs eventArgs)
    {
        using SubToolStripDropDown control = new()
        {
            TopLevel = topLevel,
            Opacity = opacity
        };
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            styleChangedCallCount++;
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.HandleCreated += handler;
        control.OnHandleCreated(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(1, styleChangedCallCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.HandleCreated -= handler;
        control.OnHandleCreated(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(2, styleChangedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnHandleCreated_TestData))]
    public void ToolStripDropDown_OnHandleCreated_InvokeWithHandle_CallsHandleCreated(bool topLevel, float opacity, EventArgs eventArgs)
    {
        using SubToolStripDropDown control = new()
        {
            TopLevel = topLevel,
            Opacity = opacity
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            styleChangedCallCount++;
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.HandleCreated += handler;
        control.OnHandleCreated(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(1, styleChangedCallCount);
        Assert.True(control.IsHandleCreated);

        // Remove handler.
        control.HandleCreated -= handler;
        control.OnHandleCreated(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(2, styleChangedCallCount);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripDropDown_OnHandleDestroyed_Invoke_CallsHandleDestroyed(EventArgs eventArgs)
    {
        using SubToolStripDropDown control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.HandleDestroyed += handler;
        control.OnHandleDestroyed(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.HandleDestroyed -= handler;
        control.OnHandleDestroyed(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripDropDown_OnHandleDestroyed_InvokeWithHandle_CallsHandleDestroyed(EventArgs eventArgs)
    {
        using SubToolStripDropDown control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.HandleDestroyed += handler;
        control.OnHandleDestroyed(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);

        // Remove handler.
        control.HandleDestroyed -= handler;
        control.OnHandleDestroyed(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> HelpEventArgs_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new HelpEventArgs(new Point(1, 2)) };
    }

    [WinFormsTheory]
    [MemberData(nameof(HelpEventArgs_TestData))]
    public void ToolStripDropDown_OnHelpRequested_Invoke_CallsHelpRequested(HelpEventArgs eventArgs)
    {
        using SubToolStripDropDown control = new();
        int callCount = 0;
        HelpEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.HelpRequested += handler;
        control.OnHelpRequested(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.HelpRequested -= handler;
        control.OnHelpRequested(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetKeyEventArgsTheoryData))]
    public void ToolStripDropDown_OnKeyDown_Invoke_CallsKeyDown(KeyEventArgs eventArgs)
    {
        using SubToolStripDropDown control = new();
        int callCount = 0;
        KeyEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.KeyDown += handler;
        control.OnKeyDown(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.KeyDown -= handler;
        control.OnKeyDown(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetKeyPressEventArgsTheoryData))]
    public void ToolStripDropDown_OnKeyPress_Invoke_CallsKeyPress(KeyPressEventArgs eventArgs)
    {
        using SubToolStripDropDown control = new();
        int callCount = 0;
        KeyPressEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.KeyPress += handler;
        control.OnKeyPress(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.KeyPress -= handler;
        control.OnKeyPress(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetKeyEventArgsTheoryData))]
    public void ToolStripDropDown_OnKeyUp_Invoke_CallsKeyUp(KeyEventArgs eventArgs)
    {
        using SubToolStripDropDown control = new();
        int callCount = 0;
        KeyEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.KeyUp += handler;
        control.OnKeyUp(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.KeyUp -= handler;
        control.OnKeyUp(eventArgs);
        Assert.Equal(1, callCount);
    }

    public static IEnumerable<object[]> OnLayout_TestData()
    {
        yield return new object[] { true, null, 2 };
        yield return new object[] { true, new LayoutEventArgs(null, null), 2 };
        yield return new object[] { true, new LayoutEventArgs(new Control(), "affectedProperty"), 2 };

        yield return new object[] { false, null, 1 };
        yield return new object[] { false, new LayoutEventArgs(null, null), 1 };
        yield return new object[] { false, new LayoutEventArgs(new Control(), "affectedProperty"), 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnLayout_TestData))]
    public void ToolStripDropDown_OnLayout_Invoke_CallsLayout(bool autoSize, LayoutEventArgs eventArgs, int expectedLayoutCallCount)
    {
        using SubToolStripDropDown control = new()
        {
            AutoSize = autoSize
        };
        int layoutCompletedCallCount = 0;
        control.LayoutCompleted += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            layoutCompletedCallCount++;
        };
        int callCount = 0;
        LayoutEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            callCount++;
        };

        // Call with handler.
        control.Layout += handler;
        control.OnLayout(eventArgs);
        Assert.Equal(expectedLayoutCallCount, callCount);
        Assert.Equal(expectedLayoutCallCount, layoutCompletedCallCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.Layout -= handler;
        control.OnLayout(eventArgs);
        Assert.Equal(expectedLayoutCallCount, callCount);
        Assert.Equal(expectedLayoutCallCount + 1, layoutCompletedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> OnLayout_WithOverflowButton_TestData()
    {
        foreach (bool autoSize in new bool[] { true, false })
        {
            yield return new object[] { autoSize, null };
            yield return new object[] { autoSize, new LayoutEventArgs(null, null) };
            yield return new object[] { autoSize, new LayoutEventArgs(new Control(), "affectedProperty") };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnLayout_WithOverflowButton_TestData))]
    public void ToolStripDropDown_OnLayout_InvokeWithOverflowButton_CallsLayout(bool autoSize, LayoutEventArgs eventArgs)
    {
        using SubToolStripDropDown control = new()
        {
            AutoSize = autoSize
        };
        Assert.NotNull(control.OverflowButton);
        int layoutCompletedCallCount = 0;
        control.LayoutCompleted += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            layoutCompletedCallCount++;
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
        Assert.Equal(1, layoutCompletedCallCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.Layout -= handler;
        control.OnLayout(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(2, layoutCompletedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnLayout_WithOverflowButton_TestData))]
    public void ToolStripDropDown_OnLayout_InvokeWithOverflowButtonWithDropDown_CallsLayout(bool autoSize, LayoutEventArgs eventArgs)
    {
        using SubToolStripDropDown control = new()
        {
            AutoSize = autoSize
        };
        control.OverflowButton.DropDown = new ToolStripDropDown();
        int layoutCompletedCallCount = 0;
        control.LayoutCompleted += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            layoutCompletedCallCount++;
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
        Assert.Equal(1, layoutCompletedCallCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.Layout -= handler;
        control.OnLayout(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(2, layoutCompletedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnLayout_TestData))]
    public void ToolStripDropDown_OnLayout_InvokeWithItems_CallsLayout(bool autoSize, LayoutEventArgs eventArgs, int expectedLayoutCallCount)
    {
        using SubToolStripItem item1 = new();
        using SubToolStripItem item2 = new();
        using SubToolStripDropDown control = new()
        {
            AutoSize = autoSize
        };
        control.Items.Add(item1);
        control.Items.Add(item2);
        int itemLayoutCallCount1 = 0;
        item1.Layout += (sender, e) =>
        {
            Assert.Same(item1, sender);
            itemLayoutCallCount1++;
        };
        int itemLayoutCallCount2 = 0;
        item2.Layout += (sender, e) =>
        {
            Assert.Same(item2, sender);
            itemLayoutCallCount2++;
        };
        int layoutCompletedCallCount = 0;
        control.LayoutCompleted += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            layoutCompletedCallCount++;
        };
        int callCount = 0;
        LayoutEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            callCount++;
        };

        // Call with handler.
        control.Layout += handler;
        control.OnLayout(eventArgs);
        Assert.Equal(expectedLayoutCallCount, callCount);
        Assert.Equal(expectedLayoutCallCount, itemLayoutCallCount1);
        Assert.Equal(expectedLayoutCallCount, itemLayoutCallCount2);
        Assert.Equal(expectedLayoutCallCount, layoutCompletedCallCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.Layout -= handler;
        control.OnLayout(eventArgs);
        Assert.Equal(expectedLayoutCallCount, callCount);
        Assert.Equal(expectedLayoutCallCount + 1, itemLayoutCallCount1);
        Assert.Equal(expectedLayoutCallCount + 1, itemLayoutCallCount2);
        Assert.Equal(expectedLayoutCallCount + 1, layoutCompletedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> OnLayout_WithHandle_TestData()
    {
        yield return new object[] { true, null, 2, 3 };
        yield return new object[] { true, new LayoutEventArgs(null, null), 2, 3 };
        yield return new object[] { true, new LayoutEventArgs(new Control(), "affectedProperty"), 2, 3 };

        yield return new object[] { false, null, 1, 1 };
        yield return new object[] { false, new LayoutEventArgs(null, null), 1, 1 };
        yield return new object[] { false, new LayoutEventArgs(new Control(), "affectedProperty"), 1, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnLayout_WithHandle_TestData))]
    public void ToolStripDropDown_OnLayout_InvokeWithHandle_CallsLayout(bool autoSize, LayoutEventArgs eventArgs, int expectedLayoutCallCount, int expectedInvalidatedCallCount)
    {
        using SubToolStripDropDown control = new()
        {
            AutoSize = autoSize
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int layoutCompletedCallCount = 0;
        control.LayoutCompleted += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            layoutCompletedCallCount++;
        };
        int callCount = 0;
        LayoutEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            callCount++;
        };

        // Call with handler.
        control.Layout += handler;
        control.OnLayout(eventArgs);
        Assert.Equal(expectedLayoutCallCount, callCount);
        Assert.Equal(expectedLayoutCallCount, layoutCompletedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.Layout -= handler;
        control.OnLayout(eventArgs);
        Assert.Equal(expectedLayoutCallCount, callCount);
        Assert.Equal(expectedLayoutCallCount + 1, layoutCompletedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnLayout_WithOverflowButton_TestData))]
    public void ToolStripDropDown_OnLayout_InvokeWithOverflowButtonWithHandle_CallsLayout(bool autoSize, LayoutEventArgs eventArgs)
    {
        using SubToolStripDropDown control = new()
        {
            AutoSize = autoSize
        };
        Assert.NotNull(control.OverflowButton);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int layoutCompletedCallCount = 0;
        control.LayoutCompleted += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            layoutCompletedCallCount++;
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
        Assert.Equal(1, layoutCompletedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.Layout -= handler;
        control.OnLayout(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(2, layoutCompletedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnLayout_WithOverflowButton_TestData))]
    public void ToolStripDropDown_OnLayout_InvokeWithOverflowButtonWithDropDownWithHandle_CallsLayout(bool autoSize, LayoutEventArgs eventArgs)
    {
        using SubToolStripDropDown control = new()
        {
            AutoSize = autoSize
        };
        control.OverflowButton.DropDown = new ToolStripDropDown();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int layoutCompletedCallCount = 0;
        control.LayoutCompleted += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            layoutCompletedCallCount++;
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
        Assert.Equal(1, layoutCompletedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.Layout -= handler;
        control.OnLayout(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(2, layoutCompletedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> OnLayout_WithItemsWithHandle_TestData()
    {
        yield return new object[] { true, null, 2, 3 };
        yield return new object[] { true, new LayoutEventArgs(null, null), 2, 3 };
        yield return new object[] { true, new LayoutEventArgs(new Control(), "affectedProperty"), 2, 3 };

        yield return new object[] { false, null, 1, 1 };
        yield return new object[] { false, new LayoutEventArgs(null, null), 1, 1 };
        yield return new object[] { false, new LayoutEventArgs(new Control(), "affectedProperty"), 1, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnLayout_WithItemsWithHandle_TestData))]
    public void ToolStripDropDown_OnLayout_InvokeWithItemsWithHandle_CallsLayout(bool autoSize, LayoutEventArgs eventArgs, int expectedLayoutCallCount, int expectedInvalidatedCallCount)
    {
        using SubToolStripItem item1 = new();
        using SubToolStripItem item2 = new();
        using SubToolStripDropDown control = new()
        {
            AutoSize = autoSize
        };
        control.Items.Add(item1);
        control.Items.Add(item2);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int itemLayoutCallCount1 = 0;
        item1.Layout += (sender, e) =>
        {
            Assert.Same(item1, sender);
            itemLayoutCallCount1++;
        };
        int itemLayoutCallCount2 = 0;
        item2.Layout += (sender, e) =>
        {
            Assert.Same(item2, sender);
            itemLayoutCallCount2++;
        };
        int layoutCompletedCallCount = 0;
        control.LayoutCompleted += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            layoutCompletedCallCount++;
        };
        int callCount = 0;
        LayoutEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            callCount++;
        };

        // Call with handler.
        control.Layout += handler;
        control.OnLayout(eventArgs);
        Assert.Equal(expectedLayoutCallCount, callCount);
        Assert.Equal(expectedLayoutCallCount, itemLayoutCallCount1);
        Assert.Equal(expectedLayoutCallCount, itemLayoutCallCount2);
        Assert.Equal(expectedLayoutCallCount, layoutCompletedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.Layout -= handler;
        control.OnLayout(eventArgs);
        Assert.Equal(expectedLayoutCallCount, callCount);
        Assert.Equal(expectedLayoutCallCount + 1, itemLayoutCallCount1);
        Assert.Equal(expectedLayoutCallCount + 1, itemLayoutCallCount2);
        Assert.Equal(expectedLayoutCallCount + 1, layoutCompletedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripDropDown_OnLeave_Invoke_CallsLeave(EventArgs eventArgs)
    {
        using SubToolStripDropDown control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.Leave += handler;
        control.OnLeave(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.Leave -= handler;
        control.OnLeave(eventArgs);
        Assert.Equal(1, callCount);
    }

    public static IEnumerable<object[]> OnOpened_TestData()
    {
        yield return new object[] { null, null };
        yield return new object[] { null, new EventArgs() };
        yield return new object[] { new SubToolStripItem(), null };
        yield return new object[] { new SubToolStripItem(), new EventArgs() };

        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        SubToolStripItem designModeOwnerItem = new()
        {
            Site = mockSite.Object
        };
        yield return new object[] { designModeOwnerItem, null };
        yield return new object[] { designModeOwnerItem, new EventArgs() };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnOpened_TestData))]
    public void ToolStripDropDown_OnOpened_Invoke_CallsOpened(ToolStripItem ownerItem, EventArgs eventArgs)
    {
        using SubToolStripDropDown control = new()
        {
            OwnerItem = ownerItem
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.Opened += handler;
        control.OnOpened(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.Opened -= handler;
        control.OnOpened(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnOpened_TestData))]
    public void ToolStripDropDown_OnOpened_InvokeWithHandle_CallsOpened(ToolStripItem ownerItem, EventArgs eventArgs)
    {
        using SubToolStripDropDown control = new()
        {
            OwnerItem = ownerItem
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.Opened += handler;
        control.OnOpened(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.Opened -= handler;
        control.OnOpened(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> OnOpening_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new CancelEventArgs(false) };
        yield return new object[] { new CancelEventArgs(true) };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnOpening_TestData))]
    public void ToolStripDropDown_OnOpening_Invoke_CallsOpening(CancelEventArgs eventArgs)
    {
        using SubToolStripDropDown control = new();
        int callCount = 0;
        void handler(object sender, CancelEventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        }

        // Call with handler.
        control.Opening += handler;
        control.OnOpening(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.Opening -= handler;
        control.OnOpening(eventArgs);
        Assert.Equal(1, callCount);
    }

    public static IEnumerable<object[]> OnScroll_TestData()
    {
        foreach (ScrollEventType eventType in Enum.GetValues(typeof(ScrollEventType)))
        {
            foreach (ScrollOrientation orientation in Enum.GetValues(typeof(ScrollOrientation)))
            {
                yield return new object[] { new ScrollEventArgs(eventType, 1, 1, orientation) };
                yield return new object[] { new ScrollEventArgs(eventType, 1, 2, orientation) };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnScroll_TestData))]
    public void ToolStripDropDown_OnScroll_Invoke_CallsHandler(ScrollEventArgs eventArgs)
    {
        using SubToolStripDropDown control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int callCount = 0;
        ScrollEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.Scroll += handler;
        control.OnScroll(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.Scroll -= handler;
        control.OnScroll(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripDropDown_OnScroll_NullE_ThrowsNullReferenceException()
    {
        using SubToolStripDropDown control = new();
        Assert.Throws<NullReferenceException>(() => control.OnScroll(null));
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripDropDown_OnStyleChanged_Invoke_CallsStyleChanged(EventArgs eventArgs)
    {
        using SubToolStripDropDown control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.StyleChanged += handler;
        control.OnStyleChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.StyleChanged -= handler;
        control.OnStyleChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripDropDown_OnValidated_Invoke_CallsValidated(EventArgs eventArgs)
    {
        using SubToolStripDropDown control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.Validated += handler;
        control.OnValidated(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.Validated -= handler;
        control.OnValidated(eventArgs);
        Assert.Equal(1, callCount);
    }

    public static IEnumerable<object[]> OnValidating_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new CancelEventArgs() };
        yield return new object[] { new CancelEventArgs(true) };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnValidating_TestData))]
    public void ToolStripDropDown_OnValidating_Invoke_CallsValidating(CancelEventArgs eventArgs)
    {
        using SubToolStripDropDown control = new();
        int callCount = 0;
        CancelEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.Validating += handler;
        control.OnValidating(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.Validating -= handler;
        control.OnValidating(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void ToolStripDropDown_WndProc_InvokeMouseHoverWithHandle_Success()
    {
        using SubToolStripDropDown control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        control.MouseHover += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_MOUSEHOVER,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ToolStripDropDown_KeyboardArrowNavigation_Test()
    {
        ToolStripMenuItem toolStrip = new();
        ToolStripItemCollection items = toolStrip.DropDown.DisplayedItems;
        items.Add("First item");
        items.Add("Second item");
        items.Add("Third item");
        items.Add("Fourth item");
        items.Add("Fifth item");

        for (int i = 1; i < items.Count; i++) // it needs for correct work of ToolStrip.GetNextItemVertical method
        {
            items[i].SetBounds(0, items[i - 1].Bounds.Bottom + 1, items[i].Bounds.Width, items[i].Bounds.Height);
        }

        ToolStripItem expected = items[4];
        ToolStripItem actual = toolStrip.DropDown.GetNextItem(start: null, direction: ArrowDirection.Up);
        Assert.Equal(expected, actual);

        expected = items[4];
        actual = toolStrip.DropDown.GetNextItem(start: items[0], direction: ArrowDirection.Up);
        Assert.Equal(expected, actual);

        expected = items[0];
        actual = toolStrip.DropDown.GetNextItem(start: null, direction: ArrowDirection.Down);
        Assert.Equal(expected, actual);

        expected = items[1];
        actual = toolStrip.DropDown.GetNextItem(start: items[0], direction: ArrowDirection.Down);
        Assert.Equal(expected, actual);

        expected = items[0];
        actual = toolStrip.DropDown.GetNextItem(start: items[4], direction: ArrowDirection.Down);
        Assert.Equal(expected, actual);
    }

    [WinFormsFact]
    public void ToolStripDropDown_WorkingAreaConstrained_DefaultValueTest()
    {
        using ToolStripDropDown control = new();
        Assert.True(control.WorkingAreaConstrained);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripDropDown_WorkingAreaConstrained_AssignmentTest(bool value)
    {
        using ToolStripDropDown control = new();
        control.WorkingAreaConstrained = value;
        Assert.Equal(value, control.WorkingAreaConstrained);
    }

    [WinFormsFact]
    public void ToolStripDropDown_DockChanged_AddRemoveEvent_Invoke_Success()
    {
        using ToolStripDropDown control = new();

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(control);
            e.Should().Be(EventArgs.Empty);
            callCount++;
        };

        control.DockChanged += handler;
        control.Dock = DockStyle.Left;
        callCount.Should().Be(1);
        control.Dock.Should().Be(DockStyle.Left);

        control.Dock = DockStyle.Right;
        callCount.Should().Be(2);
        control.Dock.Should().Be(DockStyle.Right);

        control.DockChanged -= handler;
        control.Dock = DockStyle.Top;
        callCount.Should().Be(2);
        control.Dock.Should().Be(DockStyle.Top);
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData(ToolStripDropDownCloseReason.AppClicked)]
    [InlineData(ToolStripDropDownCloseReason.AppFocusChange)]
    [InlineData(ToolStripDropDownCloseReason.CloseCalled)]
    [InlineData(ToolStripDropDownCloseReason.ItemClicked)]
    [InlineData(ToolStripDropDownCloseReason.Keyboard)]
    public void ToolStripDropDown_Close_InvokeReason_Success(ToolStripDropDownCloseReason? reason)
    {
        using ToolStripDropDown control = new();
        control.Visible = true;

        if (reason.HasValue)
        {
            control.Close(reason.Value);
        }
        else
        {
            control.Close();
        }

        control.Visible.Should().BeFalse();
    }

    [WinFormsTheory]
    [InlineData(10, 20, ToolStripDropDownDirection.AboveLeft)]
    public void ToolStripDropDown_Show_ControlPointDirection(int x, int y, ToolStripDropDownDirection direction)
    {
        using Control control = new();
        using ToolStripDropDown toolStripDropDown = new();

        toolStripDropDown.Show(control, new Point(x, y), direction);

        toolStripDropDown.Location.Should().Be(new Point(x + 6, y + 27));
    }

    [WinFormsTheory]
    [InlineData(10, 20, ToolStripDropDownDirection.AboveLeft)]
    public void ToolStripDropDown_Show_PositionDirection(int x, int y, ToolStripDropDownDirection direction)
    {
        using ToolStripDropDown toolStripDropDown = new();

        toolStripDropDown.Show(new Point(x, y), direction);
        Point expectedLocation = new(x, y);
        var nonClientSize = SystemInformation.BorderSize;
        expectedLocation.Offset(nonClientSize.Width - 3, nonClientSize.Height - 1);

        if (direction is ToolStripDropDownDirection.AboveLeft or ToolStripDropDownDirection.AboveRight)
        {
            expectedLocation.Offset(0, -toolStripDropDown.Height);
        }

        toolStripDropDown.Location.Should().Be(expectedLocation);
    }

    [WinFormsTheory]
    [InlineData(10, 20, true)]
    [InlineData(10, 20, false)]
    public void ToolStripDropDown_Show_ControlLocation(int x, int y, bool usePoint)
    {
        using Control control = new();
        using ToolStripDropDown toolStripDropDown = new();

        if (usePoint)
        {
            toolStripDropDown.Show(control, new Point(x, y));
        }
        else
        {
            toolStripDropDown.Show(control, x, y);
        }

        toolStripDropDown.Location.Should().Be(new Point(x + 8, y + 31));
    }

    [WinFormsTheory]
    [InlineData(10, 20, true)]
    [InlineData(10, 20, false)]
    public void ToolStripDropDown_Show_ScreenLocation(int x, int y, bool usePoint)
    {
        using ToolStripDropDown toolStripDropDown = new();

        if (usePoint)
        {
            toolStripDropDown.Show(new Point(x, y));
        }
        else
        {
            toolStripDropDown.Show(x, y);
        }

        toolStripDropDown.Location.Should().Be(new Point(x, y));
    }

    private class SubAxHost : AxHost
    {
        public SubAxHost(string clsid) : base(clsid)
        {
        }
    }

    private class SubToolStripItem : ToolStripItem
    {
        public event LayoutEventHandler Layout;

        protected internal override void OnLayout(LayoutEventArgs e)
        {
            Layout?.Invoke(this, e);
            base.OnLayout(e);
        }
    }

    private class NotTopMostToolStripDropDown : ToolStripDropDown
    {
        protected override bool TopMost => false;
    }

    private class SubToolStripDropDown : ToolStripDropDown
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

        public new bool TopMost => base.TopMost;

        public new bool VScroll
        {
            get => base.VScroll;
            set => base.VScroll = value;
        }

        public new AccessibleObject CreateAccessibilityInstance() => base.CreateAccessibilityInstance();

        public new ControlCollection CreateControlsInstance() => base.CreateControlsInstance();

        public new ToolStripItem CreateDefaultItem(string text, Image image, EventHandler onClick) => base.CreateDefaultItem(text, image, onClick);

        public new void CreateHandle() => base.CreateHandle();

        public new LayoutSettings CreateLayoutSettings(ToolStripLayoutStyle layoutStyle) => base.CreateLayoutSettings(layoutStyle);

        public new void Dispose(bool disposing) => base.Dispose(disposing);

        public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

        public new bool GetScrollState(int bit) => base.GetScrollState(bit);

        public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

        public new bool GetTopLevel() => base.GetTopLevel();

        public new void OnChangeUICues(UICuesEventArgs e) => base.OnChangeUICues(e);

        public new void OnClosed(ToolStripDropDownClosedEventArgs e) => base.OnClosed(e);

        public new void OnClosing(ToolStripDropDownClosingEventArgs e) => base.OnClosing(e);

        public new void OnEnter(EventArgs e) => base.OnEnter(e);

        public new void OnGiveFeedback(GiveFeedbackEventArgs e) => base.OnGiveFeedback(e);

        public new void OnHandleCreated(EventArgs e) => base.OnHandleCreated(e);

        public new void OnHandleDestroyed(EventArgs e) => base.OnHandleDestroyed(e);

        public new void OnHelpRequested(HelpEventArgs e) => base.OnHelpRequested(e);

        public new void OnItemClicked(ToolStripItemClickedEventArgs e) => base.OnItemClicked(e);

        public new void OnKeyDown(KeyEventArgs e) => base.OnKeyDown(e);

        public new void OnKeyPress(KeyPressEventArgs e) => base.OnKeyPress(e);

        public new void OnKeyUp(KeyEventArgs e) => base.OnKeyUp(e);

        public new void OnLayout(LayoutEventArgs e) => base.OnLayout(e);

        public new void OnLeave(EventArgs e) => base.OnLeave(e);

        public new void OnMouseUp(MouseEventArgs mea) => base.OnMouseUp(mea);

        public new void OnOpened(EventArgs e) => base.OnOpened(e);

        public new void OnOpening(CancelEventArgs e) => base.OnOpening(e);

        public new void OnParentChanged(EventArgs e) => base.OnParentChanged(e);

        public new void OnScroll(ScrollEventArgs se) => base.OnScroll(se);

        public new void OnStyleChanged(EventArgs e) => base.OnStyleChanged(e);

        public new void OnValidated(EventArgs e) => base.OnValidated(e);

        public new void OnValidating(CancelEventArgs e) => base.OnValidating(e);

        public new void OnVisibleChanged(EventArgs e) => base.OnVisibleChanged(e);

        public new bool ProcessDialogChar(char charCode) => base.ProcessDialogChar(charCode);

        public new bool ProcessDialogKey(Keys keyData) => base.ProcessDialogKey(keyData);

        public new void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) => base.SetBoundsCore(x, y, width, height, specified);

        public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);

        public new void SetVisibleCore(bool visible) => base.SetVisibleCore(visible);

        public new void WndProc(ref Message m) => base.WndProc(ref m);
    }
}
