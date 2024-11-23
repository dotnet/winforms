// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms.TestUtilities;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Tests;

public class ScrollableControlTests
{
    [WinFormsFact]
    public void ScrollableControl_Ctor_Default()
    {
        using SubScrollableControl control = new();
        Assert.Null(control.AccessibleDefaultActionDescription);
        Assert.Null(control.AccessibleDescription);
        Assert.Null(control.AccessibleName);
        Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
        Assert.False(control.AllowDrop);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        Assert.False(control.AutoScroll);
        Assert.Equal(Size.Empty, control.AutoScrollMargin);
        Assert.Equal(Size.Empty, control.AutoScrollMinSize);
        Assert.Equal(Point.Empty, control.AutoScrollPosition);
        Assert.False(control.AutoSize);
        Assert.Equal(Control.DefaultBackColor, control.BackColor);
        Assert.Null(control.BackgroundImage);
        Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
        Assert.Null(control.BindingContext);
        Assert.Equal(0, control.Bottom);
        Assert.Equal(Rectangle.Empty, control.Bounds);
        Assert.True(control.CanEnableIme);
        Assert.False(control.CanFocus);
        Assert.True(control.CanRaiseEvents);
        Assert.True(control.CanSelect);
        Assert.False(control.Capture);
        Assert.True(control.CausesValidation);
        Assert.Equal(Rectangle.Empty, control.ClientRectangle);
        Assert.Equal(Size.Empty, control.ClientSize);
        Assert.Null(control.Container);
        Assert.False(control.ContainsFocus);
        Assert.Null(control.ContextMenuStrip);
        Assert.Empty(control.Controls);
        Assert.Same(control.Controls, control.Controls);
        Assert.False(control.Created);
        Assert.Same(Cursors.Default, control.Cursor);
        Assert.Same(Cursors.Default, control.DefaultCursor);
        Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
        Assert.Equal(new Padding(3), control.DefaultMargin);
        Assert.Equal(Size.Empty, control.DefaultMaximumSize);
        Assert.Equal(Size.Empty, control.DefaultMinimumSize);
        Assert.Equal(Padding.Empty, control.DefaultPadding);
        Assert.Equal(Size.Empty, control.DefaultSize);
        Assert.False(control.DesignMode);
        Assert.Equal(Rectangle.Empty, control.DisplayRectangle);
        Assert.Equal(DockStyle.None, control.Dock);
        Assert.NotNull(control.DockPadding);
        Assert.Same(control.DockPadding, control.DockPadding);
        Assert.Equal(0, control.DockPadding.Top);
        Assert.Equal(0, control.DockPadding.Bottom);
        Assert.Equal(0, control.DockPadding.Left);
        Assert.Equal(0, control.DockPadding.Right);
        Assert.False(control.DoubleBuffered);
        Assert.True(control.Enabled);
        Assert.NotNull(control.Events);
        Assert.Same(control.Events, control.Events);
        Assert.False(control.Focused);
        Assert.Equal(Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.Equal(Control.DefaultForeColor, control.ForeColor);
        Assert.False(control.HasChildren);
        Assert.Equal(0, control.Height);
        Assert.NotNull(control.HorizontalScroll);
        Assert.Same(control.HorizontalScroll, control.HorizontalScroll);
        Assert.False(control.HScroll);
        Assert.Equal(ImeMode.NoControl, control.ImeMode);
        Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
        Assert.False(control.IsAccessible);
        Assert.False(control.IsMirrored);
        Assert.NotNull(control.LayoutEngine);
        Assert.Same(control.LayoutEngine, control.LayoutEngine);
        Assert.Equal(0, control.Left);
        Assert.Equal(Point.Empty, control.Location);
        Assert.Equal(new Padding(3), control.Margin);
        Assert.Equal(Size.Empty, control.MaximumSize);
        Assert.Equal(Size.Empty, control.MinimumSize);
        Assert.Equal(Padding.Empty, control.Padding);
        Assert.Null(control.Parent);
        Assert.Equal(Size.Empty, control.PreferredSize);
        Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
        Assert.False(control.RecreatingHandle);
        Assert.Null(control.Region);
        Assert.False(control.ResizeRedraw);
        Assert.Equal(0, control.Right);
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.True(control.ShowFocusCues);
        Assert.True(control.ShowKeyboardCues);
        Assert.Null(control.Site);
        Assert.Equal(Size.Empty, control.Size);
        Assert.Equal(0, control.TabIndex);
        Assert.True(control.TabStop);
        Assert.Empty(control.Text);
        Assert.Equal(0, control.Top);
        Assert.Null(control.TopLevelControl);
        Assert.False(control.UseWaitCursor);
        Assert.True(control.Visible);
        Assert.NotNull(control.VerticalScroll);
        Assert.Same(control.VerticalScroll, control.VerticalScroll);
        Assert.False(control.VScroll);
        Assert.Equal(0, control.Width);

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ScrollableControl_CreateParams_GetDefault_ReturnsExpected()
    {
        using SubScrollableControl control = new();
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Null(createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0x10000, createParams.ExStyle);
        Assert.Equal(0, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(0x56010000, createParams.Style);
        Assert.Equal(0, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, true, 0x56110000)]
    [InlineData(true, false, 0x56110000)]
    [InlineData(false, true, 0x56110000)]
    [InlineData(false, false, 0x56010000)]
    public void ScrollableControl_CreateParams_GetHScroll_ReturnsExpected(bool hScroll, bool horizontalScrollVisible, int expectedStyle)
    {
        using SubScrollableControl control = new();
        control.HorizontalScroll.Visible = horizontalScrollVisible;
        control.HScroll = hScroll;

        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Null(createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0x10000, createParams.ExStyle);
        Assert.Equal(0, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(expectedStyle, createParams.Style);
        Assert.Equal(0, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, true, 0x56210000)]
    [InlineData(true, false, 0x56210000)]
    [InlineData(false, true, 0x56210000)]
    [InlineData(false, false, 0x56010000)]
    public void ScrollableControl_CreateParams_GetVScroll_ReturnsExpected(bool vScroll, bool verticalScrollVisible, int expectedStyle)
    {
        using SubScrollableControl control = new();
        control.VerticalScroll.Visible = verticalScrollVisible;
        control.VScroll = vScroll;

        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Null(createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0x10000, createParams.ExStyle);
        Assert.Equal(0, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(expectedStyle, createParams.Style);
        Assert.Equal(0, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> SetClientRectangle_TestData()
    {
        const int width = 70;
        const int height = 80;

        // create handle
        yield return new object[] { true, width, height, width - 20, height + 50, new Rectangle(0, 0, width - SystemInformation.VerticalScrollBarWidth, height) };
        yield return new object[] { true, width, height, width + 50, height - 20, new Rectangle(0, 0, width, height - SystemInformation.HorizontalScrollBarHeight) };
        yield return new object[] { true, width, height, width + 50, height + 50, new Rectangle(0, 0, width - SystemInformation.VerticalScrollBarWidth, height - SystemInformation.HorizontalScrollBarHeight) };
        yield return new object[] { true, width, height, width - 20, height - 20, new Rectangle(0, 0, width, height) };

        // no handle
        yield return new object[] { false, width, height, width + 50, height - 20, new Rectangle(0, 0, width, height) };
        yield return new object[] { false, width, height, width - 20, height + 50, new Rectangle(0, 0, width, height) };
        yield return new object[] { false, width, height, width + 50, height + 50, new Rectangle(0, 0, width, height) };
        yield return new object[] { false, width, height, width - 20, height - 20, new Rectangle(0, 0, width, height) };
    }

    [WinFormsTheory]
    [MemberData(nameof(SetClientRectangle_TestData))]
    public void ScrollableControl_ClientRectangle_should_reduce_if_scrollbars_shown(bool createHandle, int width, int height, int childWidth, int childHeight, Rectangle expected)
    {
        using SubScrollableControl control = new()
        {
            AutoScroll = true,
            ClientSize = new Size(width, height)
        };

        // if handle isn't created, scrollbars won't be rendered, which in turn affects the size of ClientRectangle
        if (createHandle)
        {
            Assert.NotEqual(IntPtr.Zero, control.Handle);
        }

        // add a child control
        Button child = new()
        {
            Width = childWidth,
            Height = childHeight
        };
        control.Controls.Add(child);

        Assert.Equal(expected, control.ClientRectangle);
    }

    [WinFormsTheory]
    [InlineData(true, 2, 5)]
    [InlineData(false, 1, 4)]
    public void ScrollableControl_AutoScroll_Set_GetReturnsExpected(bool value, int expectedLayoutCallCount1, int expectedLayoutCallCount2)
    {
        using SubScrollableControl control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("AutoScroll", e.AffectedProperty);
            layoutCallCount++;
        };

        control.AutoScroll = value;
        Assert.Equal(value, control.AutoScroll);
        Assert.Equal(value, control.GetScrollState(SubScrollableControl.ScrollStateAutoScrolling));
        Assert.Equal(expectedLayoutCallCount1, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AutoScroll = value;
        Assert.Equal(value, control.AutoScroll);
        Assert.Equal(value, control.GetScrollState(SubScrollableControl.ScrollStateAutoScrolling));
        Assert.Equal(expectedLayoutCallCount1 * 2, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.AutoScroll = !value;
        Assert.Equal(!value, control.AutoScroll);
        Assert.Equal(!value, control.GetScrollState(SubScrollableControl.ScrollStateAutoScrolling));
        Assert.Equal(expectedLayoutCallCount2, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 2, 5)]
    [InlineData(false, 1, 4)]
    public void ScrollableControl_AutoScroll_SetWithHandle_GetReturnsExpected(bool value, int expectedLayoutCallCount1, int expectedLayoutCallCount2)
    {
        using SubScrollableControl control = new();
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
            Assert.Equal("AutoScroll", e.AffectedProperty);
            layoutCallCount++;
        };

        control.AutoScroll = value;
        Assert.Equal(value, control.AutoScroll);
        Assert.Equal(value, control.GetScrollState(SubScrollableControl.ScrollStateAutoScrolling));
        Assert.Equal(expectedLayoutCallCount1, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.AutoScroll = value;
        Assert.Equal(value, control.AutoScroll);
        Assert.Equal(value, control.GetScrollState(SubScrollableControl.ScrollStateAutoScrolling));
        Assert.Equal(expectedLayoutCallCount1 * 2, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.AutoScroll = !value;
        Assert.Equal(!value, control.AutoScroll);
        Assert.Equal(!value, control.GetScrollState(SubScrollableControl.ScrollStateAutoScrolling));
        Assert.Equal(expectedLayoutCallCount2, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> AutoScrollMargin_Set_TestData()
    {
        yield return new object[] { true, new Size(0, 0), 0 };
        yield return new object[] { true, new Size(1, 0), 1 };
        yield return new object[] { true, new Size(0, 1), 1 };
        yield return new object[] { true, new Size(1, 2), 1 };
        yield return new object[] { false, new Size(0, 0), 0 };
        yield return new object[] { false, new Size(1, 0), 0 };
        yield return new object[] { false, new Size(0, 1), 0 };
        yield return new object[] { false, new Size(1, 2), 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(AutoScrollMargin_Set_TestData))]
    public void ScrollableControl_AutoScrollMargin_Set_GetReturnsExpected(bool autoScroll, Size value, int expectedLayoutCallCount)
    {
        using ScrollableControl control = new()
        {
            AutoScroll = autoScroll
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Null(e.AffectedControl);
            Assert.Null(e.AffectedProperty);
            layoutCallCount++;
        };

        control.AutoScrollMargin = value;
        Assert.Equal(value, control.AutoScrollMargin);
        Assert.Equal(autoScroll, control.AutoScroll);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AutoScrollMargin = value;
        Assert.Equal(value, control.AutoScrollMargin);
        Assert.Equal(autoScroll, control.AutoScroll);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(AutoScrollMargin_Set_TestData))]
    public void ScrollableControl_AutoScrollMargin_SetWithHandle_GetReturnsExpected(bool autoScroll, Size value, int expectedLayoutCallCount)
    {
        using ScrollableControl control = new()
        {
            AutoScroll = autoScroll
        };
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
            Assert.Null(e.AffectedControl);
            Assert.Null(e.AffectedProperty);
            layoutCallCount++;
        };

        control.AutoScrollMargin = value;
        Assert.Equal(value, control.AutoScrollMargin);
        Assert.Equal(autoScroll, control.AutoScroll);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.AutoScrollMargin = value;
        Assert.Equal(value, control.AutoScrollMargin);
        Assert.Equal(autoScroll, control.AutoScroll);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ScrollableControl_AutoScrollMargin_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ScrollableControl))[nameof(ScrollableControl.AutoScrollMargin)];
        using ScrollableControl control = new();
        Assert.False(property.CanResetValue(control));

        control.AutoScrollMargin = new Size(1, 0);
        Assert.Equal(new Size(1, 0), control.AutoScrollMargin);
        Assert.True(property.CanResetValue(control));

        control.AutoScrollMargin = new Size(0, 1);
        Assert.Equal(new Size(0, 1), control.AutoScrollMargin);
        Assert.True(property.CanResetValue(control));

        control.AutoScrollMargin = new Size(1, 2);
        Assert.Equal(new Size(1, 2), control.AutoScrollMargin);
        Assert.True(property.CanResetValue(control));

        property.ResetValue(control);
        Assert.Equal(Size.Empty, control.AutoScrollMargin);
        Assert.False(property.CanResetValue(control));
    }

    [WinFormsFact]
    public void ScrollableControl_AutoScrollMargin_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ScrollableControl))[nameof(ScrollableControl.AutoScrollMargin)];
        using ScrollableControl control = new();
        Assert.False(property.ShouldSerializeValue(control));

        control.AutoScrollMargin = new Size(1, 0);
        Assert.Equal(new Size(1, 0), control.AutoScrollMargin);
        Assert.True(property.ShouldSerializeValue(control));

        control.AutoScrollMargin = new Size(0, 1);
        Assert.Equal(new Size(0, 1), control.AutoScrollMargin);
        Assert.True(property.ShouldSerializeValue(control));

        control.AutoScrollMargin = new Size(1, 2);
        Assert.Equal(new Size(1, 2), control.AutoScrollMargin);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.Equal(Size.Empty, control.AutoScrollMargin);
        Assert.False(property.ShouldSerializeValue(control));
    }

    [WinFormsTheory]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    [InlineData(-1, -2)]
    public void ScrollableControl_AutoScrollMargin_SetInvalid_ThrowsArgumentOutOfRangeException(int x, int y)
    {
        using ScrollableControl control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.AutoScrollMargin = new Size(x, y));
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetPointTheoryData))]
    public void ScrollableControl_AutoScrollPosition_Set_GetReturnsExpected(Point value)
    {
        using ScrollableControl control = new()
        {
            AutoScrollPosition = value
        };
        Assert.Equal(Point.Empty, control.AutoScrollPosition);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AutoScrollPosition = value;
        Assert.Equal(Point.Empty, control.AutoScrollPosition);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetPointTheoryData))]
    public void ScrollableControl_AutoScrollPosition_SetWithHandle_GetReturnsExpected(Point value)
    {
        using ScrollableControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.AutoScrollPosition = value;
        Assert.Equal(Point.Empty, control.AutoScrollPosition);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.AutoScrollPosition = value;
        Assert.Equal(Point.Empty, control.AutoScrollPosition);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetPointTheoryData))]
    public void ScrollableControl_AutoScrollPosition_SetWithAutoScroll_GetReturnsExpected(Point value)
    {
        using ScrollableControl control = new()
        {
            AutoScroll = true,
            AutoScrollPosition = value
        };
        Assert.Equal(Point.Empty, control.AutoScrollPosition);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AutoScrollPosition = value;
        Assert.Equal(Point.Empty, control.AutoScrollPosition);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetPointTheoryData))]
    public void ScrollableControl_AutoScrollPosition_SetWithVisibleBars_GetReturnsExpected(Point value)
    {
        using ScrollableControl control = new();
        control.HorizontalScroll.Visible = true;
        control.VerticalScroll.Visible = true;

        control.AutoScrollPosition = value;
        Assert.Equal(Point.Empty, control.AutoScrollPosition);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AutoScrollPosition = value;
        Assert.Equal(Point.Empty, control.AutoScrollPosition);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> AutoScrollMinSize_TestData()
    {
        yield return new object[] { true, new Size(-1, -2), true, 3 };
        yield return new object[] { true, new Size(0, 0), true, 0 };
        yield return new object[] { true, new Size(1, 0), true, 4 };
        yield return new object[] { true, new Size(0, 1), true, 4 };
        yield return new object[] { true, new Size(1, 2), true, 4 };

        yield return new object[] { false, new Size(-1, -2), true, 3 };
        yield return new object[] { false, new Size(0, 0), false, 0 };
        yield return new object[] { false, new Size(1, 0), true, 4 };
        yield return new object[] { false, new Size(0, 1), true, 4 };
        yield return new object[] { false, new Size(1, 2), true, 4 };
    }

    [WinFormsTheory]
    [MemberData(nameof(AutoScrollMinSize_TestData))]
    public void ScrollableControl_AutoScrollMinSize_Set_GetReturnsExpected(bool autoScroll, Size value, bool expectedAutoScroll, int expectedLayoutCallCount)
    {
        using ScrollableControl control = new()
        {
            AutoScroll = autoScroll
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            layoutCallCount++;
        };

        control.AutoScrollMinSize = value;
        Assert.Equal(value, control.AutoScrollMinSize);
        Assert.Equal(expectedAutoScroll, control.AutoScroll);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AutoScrollMinSize = value;
        Assert.Equal(value, control.AutoScrollMinSize);
        Assert.Equal(expectedAutoScroll, control.AutoScroll);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> AutoScrollMinSize_WithHandle_TestData()
    {
        yield return new object[] { true, new Size(-1, -2), true, 3, 0 };
        yield return new object[] { true, new Size(0, 0), true, 0, 0 };
        yield return new object[] { true, new Size(1, 0), true, 4, 2 };
        yield return new object[] { true, new Size(0, 1), true, 4, 2 };
        yield return new object[] { true, new Size(1, 2), true, 4, 2 };

        yield return new object[] { false, new Size(-1, -2), true, 3, 0 };
        yield return new object[] { false, new Size(0, 0), false, 0, 0 };
        yield return new object[] { false, new Size(1, 0), true, 4, 2 };
        yield return new object[] { false, new Size(0, 1), true, 4, 2 };
        yield return new object[] { false, new Size(1, 2), true, 4, 2 };
    }

    [WinFormsTheory]
    [MemberData(nameof(AutoScrollMinSize_WithHandle_TestData))]
    public void ScrollableControl_AutoScrollMinSize_SetWithHandle_GetReturnsExpected(bool autoScroll, Size value, bool expectedAutoScroll, int expectedLayoutCallCount, int expectedStyleChangedCallCount)
    {
        using ScrollableControl control = new()
        {
            AutoScroll = autoScroll
        };
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
            layoutCallCount++;
        };

        control.AutoScrollMinSize = value;
        Assert.Equal(value, control.AutoScrollMinSize);
        Assert.Equal(expectedAutoScroll, control.AutoScroll);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.AutoScrollMinSize = value;
        Assert.Equal(value, control.AutoScrollMinSize);
        Assert.Equal(expectedAutoScroll, control.AutoScroll);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ScrollableControl_AutoScrollMinSize_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ScrollableControl))[nameof(ScrollableControl.AutoScrollMinSize)];
        using ScrollableControl control = new();
        Assert.False(property.CanResetValue(control));

        control.AutoScrollMinSize = new Size(1, 0);
        Assert.Equal(new Size(1, 0), control.AutoScrollMinSize);
        Assert.True(property.CanResetValue(control));

        control.AutoScrollMinSize = new Size(0, 1);
        Assert.Equal(new Size(0, 1), control.AutoScrollMinSize);
        Assert.True(property.CanResetValue(control));

        control.AutoScrollMinSize = new Size(1, 2);
        Assert.Equal(new Size(1, 2), control.AutoScrollMinSize);
        Assert.True(property.CanResetValue(control));

        property.ResetValue(control);
        Assert.Equal(Size.Empty, control.AutoScrollMinSize);
        Assert.False(property.CanResetValue(control));
    }

    [WinFormsFact]
    public void ScrollableControl_AutoScrollMinSize_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ScrollableControl))[nameof(ScrollableControl.AutoScrollMinSize)];
        using ScrollableControl control = new();
        Assert.False(property.ShouldSerializeValue(control));

        control.AutoScrollMinSize = new Size(1, 0);
        Assert.Equal(new Size(1, 0), control.AutoScrollMinSize);
        Assert.True(property.ShouldSerializeValue(control));

        control.AutoScrollMinSize = new Size(0, 1);
        Assert.Equal(new Size(0, 1), control.AutoScrollMinSize);
        Assert.True(property.ShouldSerializeValue(control));

        control.AutoScrollMinSize = new Size(1, 2);
        Assert.Equal(new Size(1, 2), control.AutoScrollMinSize);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.Equal(Size.Empty, control.AutoScrollMinSize);
        Assert.False(property.ShouldSerializeValue(control));
    }

    [WinFormsTheory]
    [InlineData(true, false, false)]
    [InlineData(false, false, false)]
    [InlineData(true, true, true)]
    [InlineData(false, true, true)]
    public void ScrollableControl_DisplayRectangle_GetWithClientRectangle_ReturnsExpected(bool autoScroll, bool hScroll, bool vScroll)
    {
        using SubScrollableControl control = new()
        {
            ClientSize = new Size(70, 80),
            Padding = new Padding(1, 2, 3, 4),
            AutoScroll = autoScroll,
            HScroll = hScroll,
            VScroll = vScroll
        };
        Assert.Equal(new Rectangle(1, 2, 66, 74), control.DisplayRectangle);

        // Get again.
        Assert.Equal(new Rectangle(1, 2, 66, 74), control.DisplayRectangle);
    }

    [WinFormsTheory]
    [BoolData]
    public void ScrollableControl_HScroll_Set_GetReturnsExpected(bool value)
    {
        using SubScrollableControl control = new()
        {
            HScroll = value
        };
        Assert.Equal(value, control.HScroll);
        Assert.Equal(value, control.GetScrollState(SubScrollableControl.ScrollStateHScrollVisible));
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.HScroll = value;
        Assert.Equal(value, control.HScroll);
        Assert.Equal(value, control.GetScrollState(SubScrollableControl.ScrollStateHScrollVisible));
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.HScroll = !value;
        Assert.Equal(!value, control.HScroll);
        Assert.Equal(!value, control.GetScrollState(SubScrollableControl.ScrollStateHScrollVisible));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ScrollableControl_HScroll_SetWithHandle_GetReturnsExpected(bool value)
    {
        using SubScrollableControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.HScroll = value;
        Assert.Equal(value, control.HScroll);
        Assert.Equal(value, control.GetScrollState(SubScrollableControl.ScrollStateHScrollVisible));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.HScroll = value;
        Assert.Equal(value, control.HScroll);
        Assert.Equal(value, control.GetScrollState(SubScrollableControl.ScrollStateHScrollVisible));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.HScroll = !value;
        Assert.Equal(!value, control.HScroll);
        Assert.Equal(!value, control.GetScrollState(SubScrollableControl.ScrollStateHScrollVisible));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(ControlTests.Padding_Set_TestData), MemberType = typeof(ControlTests))]
    public void ScrollableControl_Padding_Set_GetReturnsExpected(Padding value, Padding expected, int expectedLayoutCallCount1, int expectedLayoutCallCount2)
    {
        using ScrollableControl control = new();
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
    public void ScrollableControl_Padding_SetWithHandler_CallsPaddingChanged()
    {
        using ScrollableControl control = new();
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
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetRightToLeftTheoryData))]
    public void ScrollableControl_RightToLeft_Set_GetReturnsExpected(RightToLeft value, RightToLeft expected)
    {
        using ScrollableControl control = new()
        {
            RightToLeft = value
        };
        Assert.Equal(expected, control.RightToLeft);

        // Set same.
        control.RightToLeft = value;
        Assert.Equal(expected, control.RightToLeft);
    }

    [WinFormsTheory]
    [BoolData]
    public void ScrollableControl_Visible_Set_GetReturnsExpected(bool value)
    {
        using ScrollableControl control = new()
        {
            Visible = value
        };
        Assert.Equal(value, control.Visible);

        // Set same.
        control.Visible = value;
        Assert.Equal(value, control.Visible);

        // Set different.
        control.Visible = !value;
        Assert.Equal(!value, control.Visible);
    }

    [WinFormsTheory]
    [BoolData]
    public void ScrollableControl_VScroll_Set_GetReturnsExpected(bool value)
    {
        using SubScrollableControl control = new()
        {
            VScroll = value
        };
        Assert.Equal(value, control.VScroll);
        Assert.Equal(value, control.GetScrollState(SubScrollableControl.ScrollStateVScrollVisible));
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.VScroll = value;
        Assert.Equal(value, control.VScroll);
        Assert.Equal(value, control.GetScrollState(SubScrollableControl.ScrollStateVScrollVisible));
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.VScroll = !value;
        Assert.Equal(!value, control.VScroll);
        Assert.Equal(!value, control.GetScrollState(SubScrollableControl.ScrollStateVScrollVisible));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ScrollableControl_VScroll_SetWithHandle_GetReturnsExpected(bool value)
    {
        using SubScrollableControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.VScroll = value;
        Assert.Equal(value, control.VScroll);
        Assert.Equal(value, control.GetScrollState(SubScrollableControl.ScrollStateVScrollVisible));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.VScroll = value;
        Assert.Equal(value, control.VScroll);
        Assert.Equal(value, control.GetScrollState(SubScrollableControl.ScrollStateVScrollVisible));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.VScroll = !value;
        Assert.Equal(!value, control.VScroll);
        Assert.Equal(!value, control.GetScrollState(SubScrollableControl.ScrollStateVScrollVisible));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ScrollableControl_ScrollStateAutoScrolling_Get_ReturnsExpected()
    {
        Assert.Equal(0x0001, SubScrollableControl.ScrollStateAutoScrolling);
    }

    [WinFormsFact]
    public void ScrollableControl_ScrollStateHScrollVisible_Get_ReturnsExpected()
    {
        Assert.Equal(0x0002, SubScrollableControl.ScrollStateHScrollVisible);
    }

    [WinFormsFact]
    public void ScrollableControl_ScrollStateVScrollVisible_Get_ReturnsExpected()
    {
        Assert.Equal(0x0004, SubScrollableControl.ScrollStateVScrollVisible);
    }

    [WinFormsFact]
    public void ScrollableControl_ScrollStateUserHasScrolled_Get_ReturnsExpected()
    {
        Assert.Equal(0x0008, SubScrollableControl.ScrollStateUserHasScrolled);
    }

    [WinFormsFact]
    public void ScrollableControl_ScrollStateFullDrag_Get_ReturnsExpected()
    {
        Assert.Equal(0x0010, SubScrollableControl.ScrollStateFullDrag);
    }

    public static IEnumerable<object[]> AdjustFormScrollbars_TestData()
    {
        foreach (RightToLeft rightToLeft in Enum.GetValues(typeof(RightToLeft)))
        {
            yield return new object[] { rightToLeft, true, 0, true, 0, false, false, 0, false, 0 };
            yield return new object[] { rightToLeft, true, 0, true, 0, true, false, 0, false, 0 };
            yield return new object[] { rightToLeft, true, 0, true, 1, false, false, 0, false, 0 };
            yield return new object[] { rightToLeft, true, 0, true, 1, true, false, 0, false, 0 };
            yield return new object[] { rightToLeft, true, 1, true, 0, false, false, 0, false, 0 };
            yield return new object[] { rightToLeft, true, 1, true, 0, true, false, 0, false, 0 };
            yield return new object[] { rightToLeft, true, 1, true, 1, false, false, 0, false, 1 };
            yield return new object[] { rightToLeft, true, 1, true, 1, true, false, 0, false, 1 };

            yield return new object[] { rightToLeft, true, 0, false, 0, false, false, 0, false, 0 };
            yield return new object[] { rightToLeft, true, 0, false, 0, true, false, 0, false, 0 };
            yield return new object[] { rightToLeft, true, 0, false, 1, false, false, 0, false, 0 };
            yield return new object[] { rightToLeft, true, 0, false, 1, true, false, 0, false, 0 };
            yield return new object[] { rightToLeft, true, 1, false, 0, false, false, 0, false, 0 };
            yield return new object[] { rightToLeft, true, 1, false, 0, true, false, 0, false, 0 };
            yield return new object[] { rightToLeft, true, 1, false, 1, false, false, 0, false, 1 };
            yield return new object[] { rightToLeft, true, 1, false, 1, true, false, 0, false, 1 };

            yield return new object[] { rightToLeft, false, 0, true, 0, false, false, 0, false, 0 };
            yield return new object[] { rightToLeft, false, 0, true, 0, true, false, 0, false, 0 };
            yield return new object[] { rightToLeft, false, 0, true, 1, false, false, 0, false, 0 };
            yield return new object[] { rightToLeft, false, 0, true, 1, true, false, 0, false, 0 };
            yield return new object[] { rightToLeft, false, 1, true, 0, false, false, 0, false, 0 };
            yield return new object[] { rightToLeft, false, 1, true, 0, true, false, 0, false, 0 };
            yield return new object[] { rightToLeft, false, 1, true, 1, false, false, 0, false, 1 };
            yield return new object[] { rightToLeft, false, 1, true, 1, true, false, 0, false, 1 };

            yield return new object[] { rightToLeft, false, 0, false, 0, false, false, 0, false, 0 };
            yield return new object[] { rightToLeft, false, 0, false, 0, true, false, 0, false, 0 };
            yield return new object[] { rightToLeft, false, 0, false, 1, false, false, 0, false, 1 };
            yield return new object[] { rightToLeft, false, 0, false, 1, true, false, 0, false, 1 };
            yield return new object[] { rightToLeft, false, 1, false, 0, false, false, 1, false, 0 };
            yield return new object[] { rightToLeft, false, 1, false, 0, true, false, 1, false, 0 };
            yield return new object[] { rightToLeft, false, 1, false, 1, false, false, 1, false, 1 };
            yield return new object[] { rightToLeft, false, 1, false, 1, true, false, 1, false, 1 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(AdjustFormScrollbars_TestData))]
    public void ScrollableControl_AdjustFormScrollbars_Invoke_Success(RightToLeft rightToLeft, bool hScroll, int hValue, bool vScroll, int vValue, bool displayScrollbars, bool expectedHScroll, int expectedHValue, bool expectedVScroll, int expectedVValue)
    {
        using SubScrollableControl control = new()
        {
            RightToLeft = rightToLeft,
            HScroll = hScroll,
            VScroll = vScroll
        };
        control.HorizontalScroll.Value = hValue;
        control.VerticalScroll.Value = vValue;

        control.AdjustFormScrollbars(displayScrollbars);
        Assert.Equal(expectedHScroll, control.HScroll);
        Assert.Equal(expectedHValue, control.HorizontalScroll.Value);
        Assert.False(control.HorizontalScroll.Visible);
        Assert.Equal(expectedVScroll, control.VScroll);
        Assert.Equal(expectedVValue, control.VerticalScroll.Value);
        Assert.False(control.VerticalScroll.Visible);
        Assert.False(control.GetScrollState(SubScrollableControl.ScrollStateUserHasScrolled));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(AdjustFormScrollbars_TestData))]
    public void ScrollableControl_AdjustFormScrollbars_InvokeAutoScroll_Success(RightToLeft rightToLeft, bool hScroll, int hValue, bool vScroll, int vValue, bool displayScrollbars, bool expectedHScroll, int expectedHValue, bool expectedVScroll, int expectedVValue)
    {
        using SubScrollableControl control = new()
        {
            AutoScroll = true,
            RightToLeft = rightToLeft,
            HScroll = hScroll,
            VScroll = vScroll
        };
        control.HorizontalScroll.Value = hValue;
        control.VerticalScroll.Value = vValue;

        control.AdjustFormScrollbars(displayScrollbars);
        Assert.Equal(expectedHScroll, control.HScroll);
        Assert.Equal(expectedHValue, control.HorizontalScroll.Value);
        Assert.False(control.HorizontalScroll.Visible);
        Assert.Equal(expectedVScroll, control.VScroll);
        Assert.Equal(expectedVValue, control.VerticalScroll.Value);
        Assert.False(control.VerticalScroll.Visible);
        Assert.False(control.GetScrollState(SubScrollableControl.ScrollStateUserHasScrolled));
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> AdjustFormScrollbars_AutoScrollMinSize_TestData()
    {
        foreach (RightToLeft rightToLeft in Enum.GetValues(typeof(RightToLeft)))
        {
            yield return new object[] { rightToLeft, true, 0, true, 0, false, true, 0, true, 0 };
            yield return new object[] { rightToLeft, true, 0, true, 0, true, true, 0, true, 0 };
            yield return new object[] { rightToLeft, true, 0, true, 1, false, true, 0, true, 0 };
            yield return new object[] { rightToLeft, true, 0, true, 1, true, true, 0, true, 1 };
            yield return new object[] { rightToLeft, true, 1, true, 0, false, true, 0, true, 0 };
            yield return new object[] { rightToLeft, true, 1, true, 0, true, true, 1, true, 0 };
            yield return new object[] { rightToLeft, true, 1, true, 1, false, true, 0, true, 0 };
            yield return new object[] { rightToLeft, true, 1, true, 1, true, true, 1, true, 1 };

            yield return new object[] { rightToLeft, true, 0, false, 0, false, true, 0, true, 0 };
            yield return new object[] { rightToLeft, true, 0, false, 0, true, true, 0, true, 0 };
            yield return new object[] { rightToLeft, true, 0, false, 1, false, true, 0, true, 0 };
            yield return new object[] { rightToLeft, true, 0, false, 1, true, true, 0, true, 1 };
            yield return new object[] { rightToLeft, true, 1, false, 0, false, true, 0, true, 0 };
            yield return new object[] { rightToLeft, true, 1, false, 0, true, true, 1, true, 0 };
            yield return new object[] { rightToLeft, true, 1, false, 1, false, true, 0, true, 0 };
            yield return new object[] { rightToLeft, true, 1, false, 1, true, true, 1, true, 1 };

            yield return new object[] { rightToLeft, false, 0, true, 0, false, true, 0, true, 0 };
            yield return new object[] { rightToLeft, false, 0, true, 0, true, true, 0, true, 0 };
            yield return new object[] { rightToLeft, false, 0, true, 1, false, true, 0, true, 0 };
            yield return new object[] { rightToLeft, false, 0, true, 1, true, true, 0, true, 1 };
            yield return new object[] { rightToLeft, false, 1, true, 0, false, true, 0, true, 0 };
            yield return new object[] { rightToLeft, false, 1, true, 0, true, true, 1, true, 0 };
            yield return new object[] { rightToLeft, false, 1, true, 1, false, true, 0, true, 0 };
            yield return new object[] { rightToLeft, false, 1, true, 1, true, true, 1, true, 1 };

            yield return new object[] { rightToLeft, false, 0, false, 0, false, false, 0, false, 0 };
            yield return new object[] { rightToLeft, false, 0, false, 0, true, true, 0, true, 0 };
            yield return new object[] { rightToLeft, false, 0, false, 1, false, true, 0, true, 0 };
            yield return new object[] { rightToLeft, false, 0, false, 1, true, true, 0, true, 1 };
            yield return new object[] { rightToLeft, false, 1, false, 0, false, true, 0, true, 0 };
            yield return new object[] { rightToLeft, false, 1, false, 0, true, true, 1, true, 0 };
            yield return new object[] { rightToLeft, false, 1, false, 1, false, true, 0, true, 0 };
            yield return new object[] { rightToLeft, false, 1, false, 1, true, true, 1, true, 1 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(AdjustFormScrollbars_AutoScrollMinSize_TestData))]
    public void ScrollableControl_AdjustFormScrollbars_InvokeAutoScrollMinSize_Success(RightToLeft rightToLeft, bool hScroll, int hValue, bool vScroll, int vValue, bool displayScrollbars, bool expectedHScroll, int expectedHValue, bool expectedVScroll, int expectedVValue)
    {
        using SubScrollableControl control = new()
        {
            AutoScrollMinSize = new Size(10, 20),
            RightToLeft = rightToLeft,
            HScroll = hScroll,
            VScroll = vScroll
        };
        control.HorizontalScroll.Value = hValue;
        control.VerticalScroll.Value = vValue;

        control.AdjustFormScrollbars(displayScrollbars);
        Assert.Equal(expectedHScroll, control.HScroll);
        Assert.Equal(expectedHValue, control.HorizontalScroll.Value);
        Assert.True(control.HorizontalScroll.Visible);
        Assert.Equal(expectedVScroll, control.VScroll);
        Assert.Equal(expectedVValue, control.VerticalScroll.Value);
        Assert.True(control.VerticalScroll.Visible);
        Assert.False(control.GetScrollState(SubScrollableControl.ScrollStateUserHasScrolled));
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> AdjustFormScrollbars_WithHandle_TestData()
    {
        foreach (RightToLeft rightToLeft in Enum.GetValues(typeof(RightToLeft)))
        {
            yield return new object[] { rightToLeft, true, 0, true, 0, false, false, 0, false, 0, 1 };
            yield return new object[] { rightToLeft, true, 0, true, 0, true, false, 0, false, 0, 1 };
            yield return new object[] { rightToLeft, true, 0, true, 1, false, false, 0, false, 0, 0 };
            yield return new object[] { rightToLeft, true, 0, true, 1, true, false, 0, false, 0, 0 };
            yield return new object[] { rightToLeft, true, 1, true, 0, false, false, 0, false, 0, 0 };
            yield return new object[] { rightToLeft, true, 1, true, 0, true, false, 0, false, 0, 0 };
            yield return new object[] { rightToLeft, true, 1, true, 1, false, false, 0, false, 1, 0 };
            yield return new object[] { rightToLeft, true, 1, true, 1, true, false, 0, false, 1, 0 };

            yield return new object[] { rightToLeft, true, 0, false, 0, false, false, 0, false, 0, 1 };
            yield return new object[] { rightToLeft, true, 0, false, 0, true, false, 0, false, 0, 1 };
            yield return new object[] { rightToLeft, true, 0, false, 1, false, false, 0, false, 0, 0 };
            yield return new object[] { rightToLeft, true, 0, false, 1, true, false, 0, false, 0, 0 };
            yield return new object[] { rightToLeft, true, 1, false, 0, false, false, 0, false, 0, 0 };
            yield return new object[] { rightToLeft, true, 1, false, 0, true, false, 0, false, 0, 0 };
            yield return new object[] { rightToLeft, true, 1, false, 1, false, false, 0, false, 1, 0 };
            yield return new object[] { rightToLeft, true, 1, false, 1, true, false, 0, false, 1, 0 };

            yield return new object[] { rightToLeft, false, 0, true, 0, false, false, 0, false, 0, 1 };
            yield return new object[] { rightToLeft, false, 0, true, 0, true, false, 0, false, 0, 1 };
            yield return new object[] { rightToLeft, false, 0, true, 1, false, false, 0, false, 0, 0 };
            yield return new object[] { rightToLeft, false, 0, true, 1, true, false, 0, false, 0, 0 };
            yield return new object[] { rightToLeft, false, 1, true, 0, false, false, 0, false, 0, 0 };
            yield return new object[] { rightToLeft, false, 1, true, 0, true, false, 0, false, 0, 0 };
            yield return new object[] { rightToLeft, false, 1, true, 1, false, false, 0, false, 1, 0 };
            yield return new object[] { rightToLeft, false, 1, true, 1, true, false, 0, false, 1, 0 };

            yield return new object[] { rightToLeft, false, 0, false, 0, false, false, 0, false, 0, 0 };
            yield return new object[] { rightToLeft, false, 0, false, 0, true, false, 0, false, 0, 0 };
            yield return new object[] { rightToLeft, false, 0, false, 1, false, false, 0, false, 1, 0 };
            yield return new object[] { rightToLeft, false, 0, false, 1, true, false, 0, false, 1, 0 };
            yield return new object[] { rightToLeft, false, 1, false, 0, false, false, 1, false, 0, 0 };
            yield return new object[] { rightToLeft, false, 1, false, 0, true, false, 1, false, 0, 0 };
            yield return new object[] { rightToLeft, false, 1, false, 1, false, false, 1, false, 1, 0 };
            yield return new object[] { rightToLeft, false, 1, false, 1, true, false, 1, false, 1, 0 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(AdjustFormScrollbars_WithHandle_TestData))]
    public void ScrollableControl_AdjustFormScrollbars_InvokeWithHandle_Success(RightToLeft rightToLeft, bool hScroll, int hValue, bool vScroll, int vValue, bool displayScrollbars, bool expectedHScroll, int expectedHValue, bool expectedVScroll, int expectedVValue, int expectedInvalidatedCallCount)
    {
        using SubScrollableControl control = new()
        {
            RightToLeft = rightToLeft,
            HScroll = hScroll,
            VScroll = vScroll
        };
        control.HorizontalScroll.Value = hValue;
        control.VerticalScroll.Value = vValue;
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.AdjustFormScrollbars(displayScrollbars);
        Assert.Equal(expectedHScroll, control.HScroll);
        Assert.Equal(expectedHValue, control.HorizontalScroll.Value);
        Assert.False(control.HorizontalScroll.Visible);
        Assert.Equal(expectedVScroll, control.VScroll);
        Assert.Equal(expectedVValue, control.VerticalScroll.Value);
        Assert.False(control.VerticalScroll.Visible);
        Assert.False(control.GetScrollState(SubScrollableControl.ScrollStateUserHasScrolled));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(AdjustFormScrollbars_WithHandle_TestData))]
    public void ScrollableControl_AdjustFormScrollbars_InvokeWithHandleAutoScroll_Success(RightToLeft rightToLeft, bool hScroll, int hValue, bool vScroll, int vValue, bool displayScrollbars, bool expectedHScroll, int expectedHValue, bool expectedVScroll, int expectedVValue, int expectedInvalidatedCallCount)
    {
        using SubScrollableControl control = new()
        {
            AutoScroll = true,
            RightToLeft = rightToLeft,
            HScroll = hScroll,
            VScroll = vScroll
        };
        control.HorizontalScroll.Value = hValue;
        control.VerticalScroll.Value = vValue;
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.AdjustFormScrollbars(displayScrollbars);
        Assert.Equal(expectedHScroll, control.HScroll);
        Assert.Equal(expectedHValue, control.HorizontalScroll.Value);
        Assert.False(control.HorizontalScroll.Visible);
        Assert.Equal(expectedVScroll, control.VScroll);
        Assert.Equal(expectedVValue, control.VerticalScroll.Value);
        Assert.False(control.VerticalScroll.Visible);
        Assert.False(control.GetScrollState(SubScrollableControl.ScrollStateUserHasScrolled));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> AdjustFormScrollbars_WithHandleAutoScrollMinSize_TestData()
    {
        foreach (RightToLeft rightToLeft in Enum.GetValues(typeof(RightToLeft)))
        {
            yield return new object[] { rightToLeft, true, 0, true, 0, false, true, 0, true, 0, 2 };
            yield return new object[] { rightToLeft, true, 0, true, 0, true, true, 0, true, 0, 0 };
            yield return new object[] { rightToLeft, true, 0, true, 1, false, true, 0, true, 0, 2 };
            yield return new object[] { rightToLeft, true, 0, true, 1, true, true, 0, true, 1, 0 };
            yield return new object[] { rightToLeft, true, 1, true, 0, false, true, 0, true, 0, 2 };
            yield return new object[] { rightToLeft, true, 1, true, 0, true, true, 1, true, 0, 0 };
            yield return new object[] { rightToLeft, true, 1, true, 1, false, true, 0, true, 0, 2 };
            yield return new object[] { rightToLeft, true, 1, true, 1, true, true, 1, true, 1, 0 };

            yield return new object[] { rightToLeft, true, 0, false, 0, false, true, 0, true, 0, 2 };
            yield return new object[] { rightToLeft, true, 0, false, 0, true, true, 0, true, 0, 1 };
            yield return new object[] { rightToLeft, true, 0, false, 1, false, true, 0, true, 0, 2 };
            yield return new object[] { rightToLeft, true, 0, false, 1, true, true, 0, true, 1, 0 };
            yield return new object[] { rightToLeft, true, 1, false, 0, false, true, 0, true, 0, 2 };
            yield return new object[] { rightToLeft, true, 1, false, 0, true, true, 1, true, 0, 0 };
            yield return new object[] { rightToLeft, true, 1, false, 1, false, true, 0, true, 0, 2 };
            yield return new object[] { rightToLeft, true, 1, false, 1, true, true, 1, true, 1, 0 };

            yield return new object[] { rightToLeft, false, 0, true, 0, false, true, 0, true, 0, 2 };
            yield return new object[] { rightToLeft, false, 0, true, 0, true, true, 0, true, 0, 1 };
            yield return new object[] { rightToLeft, false, 0, true, 1, false, true, 0, true, 0, 2 };
            yield return new object[] { rightToLeft, false, 0, true, 1, true, true, 0, true, 1, 0 };
            yield return new object[] { rightToLeft, false, 1, true, 0, false, true, 0, true, 0, 2 };
            yield return new object[] { rightToLeft, false, 1, true, 0, true, true, 1, true, 0, 0 };
            yield return new object[] { rightToLeft, false, 1, true, 1, false, true, 0, true, 0, 2 };
            yield return new object[] { rightToLeft, false, 1, true, 1, true, true, 1, true, 1, 0 };

            yield return new object[] { rightToLeft, false, 0, false, 0, false, false, 0, false, 0, 0 };
            yield return new object[] { rightToLeft, false, 0, false, 0, true, true, 0, true, 0, 1 };
            yield return new object[] { rightToLeft, false, 0, false, 1, false, true, 0, true, 0, 2 };
            yield return new object[] { rightToLeft, false, 0, false, 1, true, true, 0, true, 1, 0 };
            yield return new object[] { rightToLeft, false, 1, false, 0, false, true, 0, true, 0, 2 };
            yield return new object[] { rightToLeft, false, 1, false, 0, true, true, 1, true, 0, 0 };
            yield return new object[] { rightToLeft, false, 1, false, 1, false, true, 0, true, 0, 2 };
            yield return new object[] { rightToLeft, false, 1, false, 1, true, true, 1, true, 1, 0 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(AdjustFormScrollbars_WithHandleAutoScrollMinSize_TestData))]
    public void ScrollableControl_AdjustFormScrollbars_InvokeWithHandleAutoScrollMinSize_Success(RightToLeft rightToLeft, bool hScroll, int hValue, bool vScroll, int vValue, bool displayScrollbars, bool expectedHScroll, int expectedHValue, bool expectedVScroll, int expectedVValue, int expectedInvalidatedCallCount)
    {
        using SubScrollableControl control = new()
        {
            AutoScrollMinSize = new Size(10, 20),
            RightToLeft = rightToLeft,
            HScroll = hScroll,
            VScroll = vScroll
        };
        control.HorizontalScroll.Value = hValue;
        control.VerticalScroll.Value = vValue;
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.AdjustFormScrollbars(displayScrollbars);
        Assert.Equal(expectedHScroll, control.HScroll);
        Assert.Equal(expectedHValue, control.HorizontalScroll.Value);
        Assert.True(control.HorizontalScroll.Visible);
        Assert.Equal(expectedVScroll, control.VScroll);
        Assert.Equal(expectedVValue, control.VerticalScroll.Value);
        Assert.True(control.VerticalScroll.Visible);
        Assert.False(control.GetScrollState(SubScrollableControl.ScrollStateUserHasScrolled));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ScrollableControl_GetAutoSizeMode_Invoke_ReturnsExpected()
    {
        using SubScrollableControl control = new();
        Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
    }

    [WinFormsTheory]
    [InlineData(0, true)]
    [InlineData(SubScrollableControl.ScrollStateAutoScrolling, false)]
    [InlineData(SubScrollableControl.ScrollStateFullDrag, false)]
    [InlineData(SubScrollableControl.ScrollStateHScrollVisible, false)]
    [InlineData(SubScrollableControl.ScrollStateUserHasScrolled, false)]
    [InlineData(SubScrollableControl.ScrollStateVScrollVisible, false)]
    [InlineData(int.MaxValue, false)]
    [InlineData((-1), false)]
    public void ScrollableControl_GetScrollState_Invoke_ReturnsExpected(int bit, bool expected)
    {
        using SubScrollableControl control = new();
        Assert.Equal(expected, control.GetScrollState(bit));
    }

    [WinFormsTheory]
    [InlineData(ControlStyles.ContainerControl, true)]
    [InlineData(ControlStyles.UserPaint, true)]
    [InlineData(ControlStyles.Opaque, false)]
    [InlineData(ControlStyles.ResizeRedraw, false)]
    [InlineData(ControlStyles.FixedWidth, false)]
    [InlineData(ControlStyles.FixedHeight, false)]
    [InlineData(ControlStyles.StandardClick, true)]
    [InlineData(ControlStyles.Selectable, true)]
    [InlineData(ControlStyles.UserMouse, false)]
    [InlineData(ControlStyles.SupportsTransparentBackColor, false)]
    [InlineData(ControlStyles.StandardDoubleClick, true)]
    [InlineData(ControlStyles.AllPaintingInWmPaint, false)]
    [InlineData(ControlStyles.CacheText, false)]
    [InlineData(ControlStyles.EnableNotifyMessage, false)]
    [InlineData(ControlStyles.DoubleBuffer, false)]
    [InlineData(ControlStyles.OptimizedDoubleBuffer, false)]
    [InlineData(ControlStyles.UseTextForAccessibility, true)]
    [InlineData((ControlStyles)0, true)]
    [InlineData((ControlStyles)int.MaxValue, false)]
    [InlineData((ControlStyles)(-1), false)]
    public void ScrollableControl_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
    {
        using SubScrollableControl control = new();
        Assert.Equal(expected, control.GetStyle(flag));

        // Call again to test caching.
        Assert.Equal(expected, control.GetStyle(flag));
    }

    [WinFormsFact]
    public void ScrollableControl_GetTopLevel_Invoke_ReturnsExpected()
    {
        using SubScrollableControl control = new();
        Assert.False(control.GetTopLevel());
    }

    public static IEnumerable<object[]> OnLayout_TestData()
    {
        // The control must be passed along as separate variable in order to be disposed properly.
        Control affectedControl;

        yield return new object[] { true, null, 1, null };
        yield return new object[] { true, new LayoutEventArgs(null, null), 1, null };
        yield return new object[] { true, new LayoutEventArgs(affectedControl = new Control(), "affectedProperty"), 2, affectedControl };

        yield return new object[] { false, null, 1, null };
        yield return new object[] { false, new LayoutEventArgs(null, null), 1, null };
        yield return new object[] { false, new LayoutEventArgs(affectedControl = new Control(), "affectedProperty"), 1, affectedControl };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnLayout_TestData))]
    public void ScrollableControl_OnLayout_Invoke_CallsLayout(bool autoScroll, LayoutEventArgs eventArgs, int expectedCallCount, Control affectedControl)
    {
        Assert.Same(eventArgs?.AffectedComponent, affectedControl);

        using SubScrollableControl control = new()
        {
            AutoScroll = autoScroll
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
        Assert.Equal(expectedCallCount, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.Layout -= handler;
        control.OnLayout(eventArgs);
        Assert.Equal(expectedCallCount, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ScrollableControl_OnPaddingChanged_Invoke_CallsPaddingChanged(EventArgs eventArgs)
    {
        using SubScrollableControl control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.PaddingChanged += handler;
        control.OnPaddingChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.PaddingChanged -= handler;
        control.OnPaddingChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> OnPaddingChanged_WithHandle_TestData()
    {
        foreach (bool resizeRedraw in new bool[] { true, false })
        {
            yield return new object[] { resizeRedraw, null };
            yield return new object[] { resizeRedraw, new EventArgs() };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnPaddingChanged_WithHandle_TestData))]
    public void ScrollableControl_OnPaddingChanged_InvokeWithHandle_CallsPaddingChanged(bool resizeRedraw, EventArgs eventArgs)
    {
        using SubScrollableControl control = new();
        control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        // Call with handler.
        control.PaddingChanged += handler;
        control.OnPaddingChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.PaddingChanged -= handler;
        control.OnPaddingChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> OnPaintBackground_TestData()
    {
        foreach (bool hScroll in new bool[] { true, false })
        {
            foreach (bool vScroll in new bool[] { true, false })
            {
                foreach (Image backgroundImage in new Image[] { null, new Bitmap(10, 10, PixelFormat.Format32bppRgb), new Bitmap(10, 10, PixelFormat.Format32bppArgb) })
                {
                    foreach (ImageLayout backgroundImageLayout in Enum.GetValues(typeof(ImageLayout)))
                    {
                        yield return new object[] { hScroll, vScroll, true, Color.Empty, backgroundImage, backgroundImageLayout };
                        yield return new object[] { hScroll, vScroll, true, Color.Red, backgroundImage, backgroundImageLayout };
                        yield return new object[] { hScroll, vScroll, true, Color.FromArgb(100, 50, 100, 150), backgroundImage, backgroundImageLayout };
                        yield return new object[] { hScroll, vScroll, true, Color.FromArgb(0, 50, 100, 150), backgroundImage, backgroundImageLayout };
                        yield return new object[] { hScroll, vScroll, false, Color.Empty, backgroundImage, backgroundImageLayout };
                        yield return new object[] { hScroll, vScroll, false, Color.Red, backgroundImage, backgroundImageLayout };
                    }
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnPaintBackground_TestData))]
    public void ScrollableControl_OnPaintBackground_Invoke_Success(bool hScroll, bool vScroll, bool supportsTransparentBackColor, Color backColor, Image backgroundImage, ImageLayout backgroundImageLayout)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using PaintEventArgs eventArgs = new(graphics, new Rectangle(1, 2, 3, 4));

        using SubScrollableControl control = new()
        {
            HScroll = hScroll,
            VScroll = vScroll
        };
        control.SetStyle(ControlStyles.SupportsTransparentBackColor, supportsTransparentBackColor);
        control.BackColor = backColor;
        control.BackgroundImage = backgroundImage;
        control.BackgroundImageLayout = backgroundImageLayout;
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

    // TODO: unify

    public static IEnumerable<object[]> OnPaintBackground_VisualStyles_off_WithParent_TestData()
    {
        Func<Control> controlFactory = () => new Control
        {
            Bounds = new Rectangle(1, 2, 30, 40)
        };
        Func<TabPage> tabPageFactory = () => new TabPage
        {
            Bounds = new Rectangle(1, 2, 30, 40)
        };
        foreach (Func<Control> parentFactory in new Func<Control>[] { controlFactory, tabPageFactory })
        {
            foreach (bool hScroll in new bool[] { true, false })
            {
                foreach (bool vScroll in new bool[] { true, false })
                {
                    foreach (Image backgroundImage in new Image[] { null, new Bitmap(10, 10, PixelFormat.Format32bppRgb) })
                    {
                        foreach (ImageLayout backgroundImageLayout in Enum.GetValues(typeof(ImageLayout)))
                        {
                            int expected = backgroundImage is not null && (backgroundImageLayout == ImageLayout.Zoom || backgroundImageLayout == ImageLayout.Stretch || backgroundImageLayout == ImageLayout.Center) && (hScroll || vScroll) ? 0 : 1;
                            yield return new object[] { parentFactory(), hScroll, vScroll, true, Color.Empty, backgroundImage, backgroundImageLayout, 0 };
                            yield return new object[] { parentFactory(), hScroll, vScroll, true, Color.Red, backgroundImage, backgroundImageLayout, 0 };
                            yield return new object[] { parentFactory(), hScroll, vScroll, true, Color.FromArgb(100, 50, 100, 150), backgroundImage, backgroundImageLayout, expected };
                            yield return new object[] { parentFactory(), hScroll, vScroll, true, Color.FromArgb(0, 50, 100, 150), backgroundImage, backgroundImageLayout, expected };
                            yield return new object[] { parentFactory(), hScroll, vScroll, false, Color.Empty, backgroundImage, backgroundImageLayout, 0 };
                            yield return new object[] { parentFactory(), hScroll, vScroll, false, Color.Red, backgroundImage, backgroundImageLayout, 0 };
                        }
                    }

                    yield return new object[] { parentFactory(), hScroll, vScroll, true, Color.Empty, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.None, 0 };
                    yield return new object[] { parentFactory(), hScroll, vScroll, true, Color.Red, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.None, 0 };
                    yield return new object[] { parentFactory(), hScroll, vScroll, true, Color.FromArgb(100, 50, 100, 150), new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.None, 1 };
                    yield return new object[] { parentFactory(), hScroll, vScroll, true, Color.FromArgb(0, 50, 100, 150), new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.None, 1 };
                    yield return new object[] { parentFactory(), hScroll, vScroll, false, Color.Empty, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.None, 0 };
                    yield return new object[] { parentFactory(), hScroll, vScroll, false, Color.Red, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.None, 0 };

                    yield return new object[] { parentFactory(), hScroll, vScroll, true, Color.Empty, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Tile, 1 };
                    yield return new object[] { parentFactory(), hScroll, vScroll, true, Color.Red, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Tile, 1 };
                    yield return new object[] { parentFactory(), hScroll, vScroll, true, Color.FromArgb(100, 50, 100, 150), new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Tile, 2 };
                    yield return new object[] { parentFactory(), hScroll, vScroll, true, Color.FromArgb(0, 50, 100, 150), new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Tile, 2 };
                    yield return new object[] { parentFactory(), hScroll, vScroll, false, Color.Empty, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Tile, 1 };
                    yield return new object[] { parentFactory(), hScroll, vScroll, false, Color.Red, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Tile, 1 };
                }
            }
        }
    }

    public static IEnumerable<object[]> OnPaintBackground_VisualStyles_on_WithParent_TestData()
    {
        foreach (Func<Control> parentFactory in new Func<Control>[] { CreateControl, CreateTabPage })
        {
            int expected1 = parentFactory == CreateTabPage ? 0 : 1;
            int expected2 = parentFactory == CreateTabPage ? 0 : 2;
            int expected3 = parentFactory == CreateTabPage ? 0 : 3;

            foreach (bool hScroll in new bool[] { true, false })
            {
                foreach (bool vScroll in new bool[] { true, false })
                {
                    foreach (Image backgroundImage in new Image[] { null, new Bitmap(10, 10, PixelFormat.Format32bppRgb) })
                    {
                        foreach (ImageLayout backgroundImageLayout in Enum.GetValues(typeof(ImageLayout)))
                        {
                            yield return new object[] { parentFactory(), hScroll, vScroll, true, Color.Empty, backgroundImage, backgroundImageLayout, 0 };
                            yield return new object[] { parentFactory(), hScroll, vScroll, true, Color.Red, backgroundImage, backgroundImageLayout, 0 };
                            yield return new object[] { parentFactory(), hScroll, vScroll, false, Color.Empty, backgroundImage, backgroundImageLayout, 0 };
                            yield return new object[] { parentFactory(), hScroll, vScroll, false, Color.Red, backgroundImage, backgroundImageLayout, 0 };

                            int expected = parentFactory == CreateTabPage
                                ? 0
                                : backgroundImage is not null && (backgroundImageLayout == ImageLayout.Zoom || backgroundImageLayout == ImageLayout.Stretch || backgroundImageLayout == ImageLayout.Center)
                                    && (hScroll || vScroll)
                                        ? 0
                                        : 1;
                            yield return new object[] { parentFactory(), hScroll, vScroll, true, Color.FromArgb(100, 50, 100, 150), backgroundImage, backgroundImageLayout, expected };
                            yield return new object[] { parentFactory(), hScroll, vScroll, true, Color.FromArgb(0, 50, 100, 150), backgroundImage, backgroundImageLayout, expected };
                        }
                    }

                    yield return new object[] { parentFactory(), hScroll, vScroll, true, Color.Empty, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.None, 0 };
                    yield return new object[] { parentFactory(), hScroll, vScroll, true, Color.Red, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.None, 0 };
                    yield return new object[] { parentFactory(), hScroll, vScroll, true, Color.FromArgb(100, 50, 100, 150), new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.None, expected1 };
                    yield return new object[] { parentFactory(), hScroll, vScroll, true, Color.FromArgb(0, 50, 100, 150), new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.None, expected1 };
                    yield return new object[] { parentFactory(), hScroll, vScroll, false, Color.Empty, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.None, 0 };
                    yield return new object[] { parentFactory(), hScroll, vScroll, false, Color.Red, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.None, 0 };

                    yield return new object[] { parentFactory(), hScroll, vScroll, true, Color.Empty, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Tile, expected1 };
                    yield return new object[] { parentFactory(), hScroll, vScroll, true, Color.Red, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Tile, expected1 };
                    yield return new object[] { parentFactory(), hScroll, vScroll, true, Color.FromArgb(100, 50, 100, 150), new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Tile, expected1 };
                    yield return new object[] { parentFactory(), hScroll, vScroll, true, Color.FromArgb(0, 50, 100, 150), new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Tile, expected1 };
                    yield return new object[] { parentFactory(), hScroll, vScroll, false, Color.Empty, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Tile, expected1 };
                    yield return new object[] { parentFactory(), hScroll, vScroll, false, Color.Red, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Tile, expected1 };
                }
            }
        }

        static Control CreateControl() => new()
        {
            Bounds = new Rectangle(1, 2, 30, 40)
        };
        static TabPage CreateTabPage() => new()
        {
            Bounds = new Rectangle(1, 2, 30, 40)
        };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnPaintBackground_VisualStyles_on_WithParent_TestData))]
    public void ScrollableControl_OnPaintBackground_InvokeWithParent_CallsPaint(Control parent, bool hScroll, bool vScroll, bool supportsTransparentBackColor, Color backColor, Image backgroundImage, ImageLayout backgroundImageLayout, int expectedPaintCallCount)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using PaintEventArgs eventArgs = new(graphics, new Rectangle(1, 2, 3, 4));

        using SubScrollableControl control = new()
        {
            Bounds = new Rectangle(1, 2, 10, 20),
            Parent = parent,
            HScroll = hScroll,
            VScroll = vScroll
        };
        control.SetStyle(ControlStyles.SupportsTransparentBackColor, supportsTransparentBackColor);
        control.BackColor = backColor;
        control.BackgroundImage = backgroundImage;
        control.BackgroundImageLayout = backgroundImageLayout;
        int callCount = 0;
        PaintEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };
        int parentCallCount = 0;
        PaintEventHandler parentHandler = (sender, e) =>
        {
            Assert.Same(parent, sender);
            Assert.NotSame(graphics, e.Graphics);
            Assert.Equal(new Rectangle(1, 2, 0, 0), e.ClipRectangle);
            parentCallCount++;
        };

        // Call with handler.
        control.Paint += handler;
        parent.Paint += parentHandler;
        control.OnPaintBackground(eventArgs);
        Assert.Equal(0, callCount);
        Assert.Equal(expectedPaintCallCount, parentCallCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.Paint -= handler;
        parent.Paint -= parentHandler;
        control.OnPaintBackground(eventArgs);
        Assert.Equal(0, callCount);
        Assert.Equal(expectedPaintCallCount, parentCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnPaintBackground_TestData))]
    public void ScrollableControl_OnPaintBackground_InvokeWithHandle_Success(bool hScroll, bool vScroll, bool supportsTransparentBackColor, Color backColor, Image backgroundImage, ImageLayout backgroundImageLayout)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using PaintEventArgs eventArgs = new(graphics, new Rectangle(1, 2, 3, 4));

        using SubScrollableControl control = new()
        {
            HScroll = hScroll,
            VScroll = vScroll
        };
        control.SetStyle(ControlStyles.SupportsTransparentBackColor, supportsTransparentBackColor);
        control.BackColor = backColor;
        control.BackgroundImage = backgroundImage;
        control.BackgroundImageLayout = backgroundImageLayout;
        int callCount = 0;
        PaintEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        // Call with handler.
        control.Paint += handler;
        control.OnPaintBackground(eventArgs);
        Assert.Equal(0, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.Paint -= handler;
        control.OnPaintBackground(eventArgs);
        Assert.Equal(0, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> OnPaintBackground_WithParentWithHandle_TestData()
    {
        foreach (bool hScroll in new bool[] { true, false })
        {
            foreach (bool vScroll in new bool[] { true, false })
            {
                foreach (Image backgroundImage in new Image[] { null, new Bitmap(10, 10, PixelFormat.Format32bppRgb) })
                {
                    foreach (ImageLayout backgroundImageLayout in Enum.GetValues(typeof(ImageLayout)))
                    {
                        int expected = backgroundImage is not null && (backgroundImageLayout == ImageLayout.Zoom || backgroundImageLayout == ImageLayout.Stretch || backgroundImageLayout == ImageLayout.Center) && (!hScroll && vScroll) ? 0 : 1;
                        yield return new object[] { hScroll, vScroll, true, Color.Empty, backgroundImage, backgroundImageLayout, 0 };
                        yield return new object[] { hScroll, vScroll, true, Color.Red, backgroundImage, backgroundImageLayout, 0 };
                        yield return new object[] { hScroll, vScroll, true, Color.FromArgb(100, 50, 100, 150), backgroundImage, backgroundImageLayout, expected };
                        yield return new object[] { hScroll, vScroll, true, Color.FromArgb(0, 50, 100, 150), backgroundImage, backgroundImageLayout, expected };
                        yield return new object[] { hScroll, vScroll, false, Color.Empty, backgroundImage, backgroundImageLayout, 0 };
                        yield return new object[] { hScroll, vScroll, false, Color.Red, backgroundImage, backgroundImageLayout, 0 };
                    }
                }

                yield return new object[] { hScroll, vScroll, true, Color.Empty, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.None, 0 };
                yield return new object[] { hScroll, vScroll, true, Color.Red, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.None, 0 };
                yield return new object[] { hScroll, vScroll, true, Color.FromArgb(100, 50, 100, 150), new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.None, 1 };
                yield return new object[] { hScroll, vScroll, true, Color.FromArgb(0, 50, 100, 150), new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.None, 1 };
                yield return new object[] { hScroll, vScroll, false, Color.Empty, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.None, 0 };
                yield return new object[] { hScroll, vScroll, false, Color.Red, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.None, 0 };

                yield return new object[] { hScroll, vScroll, true, Color.Empty, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Tile, 1 };
                yield return new object[] { hScroll, vScroll, true, Color.Red, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Tile, 1 };
                yield return new object[] { hScroll, vScroll, true, Color.FromArgb(100, 50, 100, 150), new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Tile, 1 };
                yield return new object[] { hScroll, vScroll, true, Color.FromArgb(0, 50, 100, 150), new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Tile, 1 };
                yield return new object[] { hScroll, vScroll, false, Color.Empty, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Tile, 1 };
                yield return new object[] { hScroll, vScroll, false, Color.Red, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Tile, 1 };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnPaintBackground_WithParentWithHandle_TestData))]
    public void ScrollableControl_OnPaintBackground_InvokeWithParentWithHandle_CallsPaint(bool hScroll, bool vScroll, bool supportsTransparentBackColor, Color backColor, Image backgroundImage, ImageLayout backgroundImageLayout, int expectedPaintCallCount)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using PaintEventArgs eventArgs = new(graphics, new Rectangle(1, 2, 3, 4));

        using Control parent = new()
        {
            Bounds = new Rectangle(1, 2, 30, 40)
        };
        using SubScrollableControl control = new()
        {
            Bounds = new Rectangle(1, 2, 10, 20),
            Parent = parent,
            HScroll = hScroll,
            VScroll = vScroll
        };
        control.SetStyle(ControlStyles.SupportsTransparentBackColor, supportsTransparentBackColor);
        control.BackColor = backColor;
        control.BackgroundImage = backgroundImage;
        control.BackgroundImageLayout = backgroundImageLayout;
        int callCount = 0;
        PaintEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };
        int parentCallCount = 0;
        PaintEventHandler parentHandler = (sender, e) =>
        {
            Assert.Same(parent, sender);
            Assert.NotSame(graphics, e.Graphics);
            Assert.Equal(new Rectangle(1, 2, 10, 20), e.ClipRectangle);
            parentCallCount++;
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        // Call with handler.
        control.Paint += handler;
        parent.Paint += parentHandler;
        control.OnPaintBackground(eventArgs);
        Assert.Equal(0, callCount);
        Assert.Equal(expectedPaintCallCount, parentCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.Paint -= handler;
        parent.Paint -= parentHandler;
        control.OnPaintBackground(eventArgs);
        Assert.Equal(0, callCount);
        Assert.Equal(expectedPaintCallCount, parentCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ScrollableControl_OnPaintBackground_NullEventArgs_ThrowsArgumentNullException()
    {
        using SubScrollableControl control = new();
        Assert.Throws<ArgumentNullException>(() => control.OnPaintBackground(null));
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ScrollableControl_OnRightToLeftChanged_Invoke_CallsRightToLeftChanged(EventArgs eventArgs)
    {
        using SubScrollableControl control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("RightToLeft", e.AffectedProperty);
            layoutCallCount++;
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.RightToLeftChanged += handler;
        control.OnRightToLeftChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(1, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.RightToLeftChanged -= handler;
        control.OnRightToLeftChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(2, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ScrollableControl_OnRightToLeftChanged_InvokeWithHandle_CallsRightToLeftChanged(EventArgs eventArgs)
    {
        using SubScrollableControl control = new();
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
            if (e.AffectedProperty == "RightToLeft")
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("RightToLeft", e.AffectedProperty);
                layoutCallCount++;
            }
        };

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.RightToLeftChanged += handler;
        control.OnRightToLeftChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(1, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(1, createdCallCount);

        // Remove handler.
        control.RightToLeftChanged -= handler;
        control.OnRightToLeftChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(2, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(2, createdCallCount);
    }

    public static IEnumerable<object[]> OnScroll_TestData()
    {
        yield return new object[] { null };

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
    public void ScrollableControl_OnScroll_Invoke_CallsScroll(ScrollEventArgs eventArgs)
    {
        using SubScrollableControl control = new();
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
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.Scroll -= handler;
        control.OnScroll(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnScroll_TestData))]
    public void ScrollableControl_OnScroll_InvokeWithHandle_CallsScroll(ScrollEventArgs eventArgs)
    {
        using SubScrollableControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
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
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.Scroll -= handler;
        control.OnScroll(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

#pragma warning disable 0618
    [WinFormsFact]
    public void ScrollableControl_Scale_InvokeWithoutPaddingWithDockPadding_Success()
    {
        using SubScrollableControl control = new();
        Assert.Equal(0, control.DockPadding.Left);
        Assert.Equal(0, control.DockPadding.Top);
        Assert.Equal(0, control.DockPadding.Right);
        Assert.Equal(0, control.DockPadding.Bottom);
        control.Scale(10, 20);

        Assert.Equal(0, control.DockPadding.Left);
        Assert.Equal(0, control.DockPadding.Top);
        Assert.Equal(0, control.DockPadding.Right);
        Assert.Equal(0, control.DockPadding.Bottom);
        Assert.Equal(Padding.Empty, control.Padding);
    }

    [WinFormsFact]
    public void ScrollableControl_Scale_InvokeWithoutPaddingWithoutDockPadding_Success()
    {
        using SubScrollableControl control = new();
        control.Scale(10, 20);
        Assert.Equal(0, control.DockPadding.Left);
        Assert.Equal(0, control.DockPadding.Top);
        Assert.Equal(0, control.DockPadding.Right);
        Assert.Equal(0, control.DockPadding.Bottom);
        Assert.Equal(Padding.Empty, control.Padding);
    }

    [WinFormsFact]
    public void ScrollableControl_Scale_InvokeWithPaddingWithDockPadding_Success()
    {
        using SubScrollableControl control = new()
        {
            Padding = new Padding(1, 2, 3, 4)
        };
        control.Scale(10, 20);
        Assert.Equal(1, control.DockPadding.Left);
        Assert.Equal(2, control.DockPadding.Top);
        Assert.Equal(3, control.DockPadding.Right);
        Assert.Equal(4, control.DockPadding.Bottom);
        Assert.Equal(new Padding(1, 2, 3, 4), control.Padding);
    }

    [WinFormsFact]
    public void ScrollableControl_Scale_InvokeWithPaddingWithoutDockPadding_Success()
    {
        using SubScrollableControl control = new()
        {
            Padding = new Padding(1, 2, 3, 4)
        };
        Assert.Equal(1, control.DockPadding.Left);
        Assert.Equal(2, control.DockPadding.Top);
        Assert.Equal(3, control.DockPadding.Right);
        Assert.Equal(4, control.DockPadding.Bottom);
        control.Scale(10, 20);
        Assert.Equal(1, control.DockPadding.Left);
        Assert.Equal(2, control.DockPadding.Top);
        Assert.Equal(3, control.DockPadding.Right);
        Assert.Equal(4, control.DockPadding.Bottom);
        Assert.Equal(new Padding(1, 2, 3, 4), control.Padding);
    }

    [WinFormsFact]
    public void ScrollableControl_ScaleCore_InvokeWithoutPaddingWithDockPadding_Success()
    {
        using SubScrollableControl control = new();
        Assert.Equal(0, control.DockPadding.Left);
        Assert.Equal(0, control.DockPadding.Top);
        Assert.Equal(0, control.DockPadding.Right);
        Assert.Equal(0, control.DockPadding.Bottom);
        control.ScaleCore(10, 20);

        Assert.Equal(0, control.DockPadding.Left);
        Assert.Equal(0, control.DockPadding.Top);
        Assert.Equal(0, control.DockPadding.Right);
        Assert.Equal(0, control.DockPadding.Bottom);
        Assert.Equal(Padding.Empty, control.Padding);
    }

    [WinFormsFact]
    public void ScrollableControl_ScaleCore_InvokeWithoutPaddingWithoutDockPadding_Success()
    {
        using SubScrollableControl control = new();
        control.ScaleCore(10, 20);
        Assert.Equal(0, control.DockPadding.Left);
        Assert.Equal(0, control.DockPadding.Top);
        Assert.Equal(0, control.DockPadding.Right);
        Assert.Equal(0, control.DockPadding.Bottom);
        Assert.Equal(Padding.Empty, control.Padding);
    }

    [WinFormsFact]
    public void ScrollableControl_ScaleCore_InvokeWithPaddingWithDockPadding_Success()
    {
        using SubScrollableControl control = new()
        {
            Padding = new Padding(1, 2, 3, 4)
        };
        control.ScaleCore(10, 20);
        Assert.Equal(1, control.DockPadding.Left);
        Assert.Equal(2, control.DockPadding.Top);
        Assert.Equal(3, control.DockPadding.Right);
        Assert.Equal(4, control.DockPadding.Bottom);
        Assert.Equal(new Padding(1, 2, 3, 4), control.Padding);
    }

    [WinFormsFact]
    public void ScrollableControl_ScaleCore_InvokeWithPaddingWithoutDockPadding_Success()
    {
        using SubScrollableControl control = new()
        {
            Padding = new Padding(1, 2, 3, 4)
        };
        Assert.Equal(1, control.DockPadding.Left);
        Assert.Equal(2, control.DockPadding.Top);
        Assert.Equal(3, control.DockPadding.Right);
        Assert.Equal(4, control.DockPadding.Bottom);
        control.ScaleCore(10, 20);
        Assert.Equal(1, control.DockPadding.Left);
        Assert.Equal(2, control.DockPadding.Top);
        Assert.Equal(3, control.DockPadding.Right);
        Assert.Equal(4, control.DockPadding.Bottom);
        Assert.Equal(new Padding(1, 2, 3, 4), control.Padding);
    }
#pragma warning restore 0618

    [WinFormsFact]
    public void ScrollableControl_ScaleControl_InvokeWithDockPadding_Success()
    {
        using ScrollableControl control = new()
        {
            Padding = new Padding(1, 2, 3, 4)
        };
        Assert.Equal(1, control.DockPadding.Left);
        Assert.Equal(2, control.DockPadding.Top);
        Assert.Equal(3, control.DockPadding.Right);
        Assert.Equal(4, control.DockPadding.Bottom);
        control.Scale(new SizeF(10, 20));

        Assert.Equal(10, control.DockPadding.Left);
        Assert.Equal(40, control.DockPadding.Top);
        Assert.Equal(30, control.DockPadding.Right);
        Assert.Equal(80, control.DockPadding.Bottom);
        Assert.Equal(new Padding(10, 40, 30, 80), control.Padding);
    }

    [WinFormsFact]
    public void ScrollableControl_ScaleControl_InvokeWithoutDockPadding_Success()
    {
        using ScrollableControl control = new();
        control.Scale(new SizeF(10, 20));
        Assert.Equal(0, control.DockPadding.Left);
        Assert.Equal(0, control.DockPadding.Top);
        Assert.Equal(0, control.DockPadding.Right);
        Assert.Equal(0, control.DockPadding.Bottom);
        Assert.Equal(Padding.Empty, control.Padding);
    }

    public static IEnumerable<object[]> SetAutoScrollMargin_TestData()
    {
        yield return new object[] { true, -1, -1, new Size(0, 0), 0 };
        yield return new object[] { true, 0, 0, new Size(0, 0), 0 };
        yield return new object[] { true, 0, 1, new Size(0, 1), 1 };
        yield return new object[] { true, 1, 0, new Size(1, 0), 1 };
        yield return new object[] { true, 1, 2, new Size(1, 2), 1 };
        yield return new object[] { false, -1, -1, new Size(0, 0), 0 };
        yield return new object[] { false, 0, 0, new Size(0, 0), 0 };
        yield return new object[] { false, 0, 1, new Size(0, 1), 0 };
        yield return new object[] { false, 1, 0, new Size(1, 0), 0 };
        yield return new object[] { false, 1, 2, new Size(1, 2), 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(SetAutoScrollMargin_TestData))]
    public void ScrollableControl_SetAutoScrollMargin_Invoke_Success(bool autoScroll, int width, int height, Size expectedAutoScrollMargin, int expectedLayoutCallCount)
    {
        using ScrollableControl control = new()
        {
            AutoScroll = autoScroll
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Null(e.AffectedControl);
            Assert.Null(e.AffectedProperty);
            layoutCallCount++;
        };

        control.SetAutoScrollMargin(width, height);
        Assert.Equal(expectedAutoScrollMargin, control.AutoScrollMargin);
        Assert.Equal(autoScroll, control.AutoScroll);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetAutoScrollMargin_TestData))]
    public void ScrollableControl_SetAutoScrollMargin_InvokeWithHandle_Success(bool autoScroll, int width, int height, Size expectedAutoScrollMargin, int expectedLayoutCallCount)
    {
        using ScrollableControl control = new()
        {
            AutoScroll = autoScroll
        };
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
            Assert.Null(e.AffectedControl);
            Assert.Null(e.AffectedProperty);
            layoutCallCount++;
        };

        control.SetAutoScrollMargin(width, height);
        Assert.Equal(expectedAutoScrollMargin, control.AutoScrollMargin);
        Assert.Equal(autoScroll, control.AutoScroll);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> SetDisplayRectLocation_TestData()
    {
        const int width = 70;
        const int height = 80;
        Size scrollableSize = new(100, 150);
        Size nonScrollableSize = new(width, height);

        yield return new object[] { true, width, height, 0, 0, new Point(0, 0), scrollableSize };
        yield return new object[] { false, width, height, 0, 0, new Point(0, 0), nonScrollableSize };

        yield return new object[] { true, width, height, -10, 0, new Point(-10, 0), scrollableSize };
        yield return new object[] { false, width, height, -10, 0, new Point(0, 0), nonScrollableSize };

        yield return new object[] { true, width, height, 0, -20, new Point(0, -20), scrollableSize };
        yield return new object[] { false, width, height, 0, -20, new Point(0, 0), nonScrollableSize };

        yield return new object[] { true, width, height, -10, -20, new Point(-10, -20), scrollableSize };
        yield return new object[] { false, width, height, -10, -20, new Point(0, 0), nonScrollableSize };

        // Overflow.
        yield return new object[] { true, width, height, -100, -20, new Point(-30, -20), scrollableSize };
        yield return new object[] { false, width, height, -100, -20, new Point(0, 0), nonScrollableSize };

        yield return new object[] { true, width, height, -10, -200, new Point(-10, -70), scrollableSize };
        yield return new object[] { false, width, height, -10, -200, new Point(0, 0), nonScrollableSize };

        // Underflow.
        yield return new object[] { true, width, height, 1, 20, new Point(0, 0), scrollableSize };
        yield return new object[] { false, width, height, 1, 20, new Point(0, 0), nonScrollableSize };

        yield return new object[] { true, width, height, 10, 2, new Point(0, 0), scrollableSize };
        yield return new object[] { false, width, height, 10, 2, new Point(0, 0), nonScrollableSize };
    }

    [WinFormsTheory]
    [MemberData(nameof(SetDisplayRectLocation_TestData))]
    public void ScrollableControl_SetDisplayRectLocation_Invoke_Success(bool autoScroll, int width, int height, int scrollX, int scrollY, Point expectedDisplayRectangleLocation, Size expectedDisplayRectangleSize)
    {
        using SubScrollableControl control = new()
        {
            AutoScroll = autoScroll,
            ClientSize = new Size(width, height)
        };

        // Without child.
        control.SetDisplayRectLocation(scrollX, scrollY);
        Assert.Equal(new Rectangle(0, 0, width, height), control.DisplayRectangle);
        Assert.Equal(Point.Empty, control.AutoScrollPosition);

        // With child.
        using LargeControl child = new();
        control.Controls.Add(child);
        Assert.Equal(child.ExpectedSize, child.Bounds);

        control.SetDisplayRectLocation(scrollX, scrollY);
        Assert.Equal(expectedDisplayRectangleSize, control.DisplayRectangle.Size);
        Assert.Equal(expectedDisplayRectangleLocation, control.DisplayRectangle.Location);
        Assert.Equal(expectedDisplayRectangleLocation, control.AutoScrollPosition);
        Assert.Equal(child.ExpectedSize, child.Bounds);
    }

    public static IEnumerable<object[]> SetDisplayRectLocation_WithHandle_TestData()
    {
        const int width = 70;
        const int height = 80;
        Size scrollableSize = new(100, 150);
        Size nonScrollableSize = new(width, height);

        yield return new object[] { true, width, height, 0, 0, new Point(0, 0), scrollableSize, 1 };
        yield return new object[] { false, width, height, 0, 0, new Point(0, 0), nonScrollableSize, 0 };

        yield return new object[] { true, width, height, -10, 0, new Point(-10, 0), scrollableSize, 1 };
        yield return new object[] { false, width, height, -10, 0, new Point(0, 0), nonScrollableSize, 0 };

        yield return new object[] { true, width, height, 0, -20, new Point(0, -20), scrollableSize, 1 };
        yield return new object[] { false, width, height, 0, -20, new Point(0, 0), nonScrollableSize, 0 };

        yield return new object[] { true, width, height, -10, -20, new Point(-10, -20), scrollableSize, 1 };
        yield return new object[] { false, width, height, -10, -20, new Point(0, 0), nonScrollableSize, 0 };

        // Overflow.
        yield return new object[] { true, width, height, -100, -20, new Point(-47, -20), scrollableSize, 1 };
        yield return new object[] { false, width, height, -100, -20, new Point(0, 0), nonScrollableSize, 0 };

        yield return new object[] { true, width, height, -10, -200, new Point(-10, -87), scrollableSize, 1 };
        yield return new object[] { false, width, height, -10, -200, new Point(0, 0), nonScrollableSize, 0 };

        // Underflow.
        yield return new object[] { true, width, height, 1, 20, new Point(0, 0), scrollableSize, 1 };
        yield return new object[] { false, width, height, 1, 20, new Point(0, 0), nonScrollableSize, 0 };

        yield return new object[] { true, width, height, 10, 2, new Point(0, 0), scrollableSize, 1 };
        yield return new object[] { false, width, height, 10, 2, new Point(0, 0), nonScrollableSize, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(SetDisplayRectLocation_WithHandle_TestData))]
    public void ScrollableControl_SetDisplayRectLocation_InvokeWithHandle_Success(bool autoScroll, int width, int height, int scrollX, int scrollY, Point expectedDisplayRectangleLocation, Size expectedDisplayRectangleSize, int expectedInvalidatedCallCount)
    {
        using SubScrollableControl control = new()
        {
            AutoScroll = autoScroll,
            ClientSize = new Size(width, height)
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        // Without child.
        control.SetDisplayRectLocation(scrollX, scrollY);
        Assert.Equal(new Rectangle(0, 0, width, height), control.DisplayRectangle);
        Assert.Equal(Point.Empty, control.AutoScrollPosition);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // With child.
        using LargeControl child = new();
        control.Controls.Add(child);
        Assert.Equal(child.ExpectedSize, child.Bounds);

        control.SetDisplayRectLocation(scrollX, scrollY);
        Assert.Equal(expectedDisplayRectangleSize, control.DisplayRectangle.Size);
        Assert.Equal(expectedDisplayRectangleLocation, control.DisplayRectangle.Location);
        Assert.Equal(expectedDisplayRectangleLocation, control.AutoScrollPosition);
        Assert.Equal(new Rectangle(expectedDisplayRectangleLocation, child.ExpectedSize.Size), child.Bounds);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> SetScrollState_TestData()
    {
        yield return new object[] { 1, true, true };
        yield return new object[] { 1, false, false };
        yield return new object[] { 0, true, true };
        yield return new object[] { 0, false, true };
    }

    [WinFormsTheory]
    [MemberData(nameof(SetScrollState_TestData))]
    public void ScrollableControl_SetScrollState_Invoke_GetScrollStateReturnsExpected(int bit, bool value, bool expected)
    {
        using SubScrollableControl control = new();
        control.SetScrollState(bit, value);
        Assert.Equal(expected, control.GetScrollState(bit));
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SetScrollState(bit, value);
        Assert.Equal(expected, control.GetScrollState(bit));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetScrollState_TestData))]
    public void ScrollableControl_GetScrollState_InvokeWithHandle_GetStyleReturnsExpected(int bit, bool value, bool expected)
    {
        using SubScrollableControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SetScrollState(bit, value);
        Assert.Equal(expected, control.GetScrollState(bit));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.SetScrollState(bit, value);
        Assert.Equal(expected, control.GetScrollState(bit));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> ScrollControlIntoView_TestData()
    {
        // Can't scroll - invalid child.
        yield return new object[] { true, true, true, new Size(70, 80), null, new Rectangle(0, 0, 70, 80) };
        yield return new object[] { false, true, true, new Size(70, 80), null, new Rectangle(0, 0, 70, 80) };

        yield return new object[] { true, true, true, new Size(70, 80), new Control(), new Rectangle(0, 0, 70, 80) };

        // Can't scroll - not AutoScroll.
        yield return new object[] { false, true, true, new Size(70, 80), new LargeControl(), new Rectangle(0, 0, 70, 80) };

        // Can't scroll - not HScroll or VScroll.
        yield return new object[] { true, false, false, new Size(70, 80), new LargeControl(), new Rectangle(0, 0, 100, 150) };

        // Can't scroll - empty.
        yield return new object[] { true, false, false, new Size(0, 80), new LargeControl(), new Rectangle(0, 0, 100, 150) };
        yield return new object[] { true, false, false, new Size(-1, 80), new LargeControl(), new Rectangle(0, 0, 100, 150) };
        yield return new object[] { true, false, false, new Size(70, 0), new LargeControl(), new Rectangle(0, 0, 100, 150) };
        yield return new object[] { true, false, false, new Size(70, -1), new LargeControl(), new Rectangle(0, 0, 100, 150) };

        // Can scroll.
        yield return new object[] { true, true, false, new Size(70, 80), new LargeControl(), new Rectangle(0, 0, 100, 150) };
        yield return new object[] { true, false, true, new Size(70, 80), new LargeControl(), new Rectangle(0, 0, 100, 150) };
        yield return new object[] { true, true, true, new Size(70, 80), new LargeControl(), new Rectangle(0, 0, 100, 150) };

        yield return new object[] { true, true, false, new Size(70, 80), new SmallControl(), new Rectangle(0, 0, 70, 80) };
        yield return new object[] { true, false, true, new Size(70, 80), new SmallControl(), new Rectangle(0, 0, 70, 80) };
        yield return new object[] { true, true, true, new Size(70, 80), new SmallControl(), new Rectangle(0, 0, 70, 80) };

        foreach (bool hScroll in new bool[] { true, false })
        {
            SmallControl childControl = new();
            LargeControl parentControl = new();
            parentControl.Controls.Add(childControl);
            yield return new object[] { true, true, true, new Size(70, 80), parentControl, new Rectangle(0, 0, 100, 150) };
            yield return new object[] { true, hScroll, true, new Size(70, 80), childControl, new Rectangle(0, 0, 100, 150) };
        }

        foreach (bool vScroll in new bool[] { true, false })
        {
            SmallControl childControl = new();
            LargeControl parentControl = new();
            parentControl.Controls.Add(childControl);
            yield return new object[] { true, true, true, new Size(70, 80), parentControl, new Rectangle(0, 0, 100, 150) };
            yield return new object[] { true, true, vScroll, new Size(70, 80), childControl, new Rectangle(0, 0, 100, 150) };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ScrollControlIntoView_TestData))]
    public void ScrollableControl_ScrollControlIntoView_Invoke_Success(bool autoScroll, bool hScroll, bool vScroll, Size clientSize, Control activeControl, Rectangle expectedDisplayRectangle)
    {
        using SubScrollableControl control = new()
        {
            AutoScroll = autoScroll,
            HScroll = hScroll,
            VScroll = vScroll,
            ClientSize = clientSize
        };
        if ((activeControl is LargeControl or SmallControl))
        {
            control.Controls.Add(activeControl.Parent ?? activeControl);
        }

        control.ScrollControlIntoView(activeControl);
        Assert.Equal(expectedDisplayRectangle, control.DisplayRectangle);

        control.Controls.Clear();
    }

    private class LargeControl : Control
    {
        protected override Size DefaultSize => new(100, 150);

        public Rectangle ExpectedSize => new(new Point(0, 0), DefaultSize);
    }

    private class SmallControl : Control
    {
        protected override Size DefaultSize => new(50, 60);
    }

    public class SubScrollableControl : ScrollableControl
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

        public new ImeMode DefaultImeMode => base.DefaultImeMode;

        public new Padding DefaultMargin => base.DefaultMargin;

        public new Size DefaultMaximumSize => base.DefaultMaximumSize;

        public new Size DefaultMinimumSize => base.DefaultMinimumSize;

        public new Padding DefaultPadding => base.DefaultPadding;

        public new Size DefaultSize => base.DefaultSize;

        public new bool DesignMode => base.DesignMode;

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

        public new void AdjustFormScrollbars(bool displayScrollbars) => base.AdjustFormScrollbars(displayScrollbars);

        public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

        public new bool GetScrollState(int bit) => base.GetScrollState(bit);

        public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

        public new bool GetTopLevel() => base.GetTopLevel();

        public new void OnLayout(LayoutEventArgs e) => base.OnLayout(e);

        public new void OnPaddingChanged(EventArgs e) => base.OnPaddingChanged(e);

        public new void OnPaintBackground(PaintEventArgs e) => base.OnPaintBackground(e);

        public new void OnRightToLeftChanged(EventArgs e) => base.OnRightToLeftChanged(e);

        public new void OnScroll(ScrollEventArgs se) => base.OnScroll(se);

        public new void ScaleCore(float dx, float dy) => base.ScaleCore(dx, dy);

        public new void SetDisplayRectLocation(int x, int y) => base.SetDisplayRectLocation(x, y);

        public new void SetScrollState(int bit, bool value) => base.SetScrollState(bit, value);

        public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);
    }
}
