// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.TestUtilities;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Tests;

public class UpDownBaseTests
{
    [WinFormsFact]
    public void UpDownBase_Ctor_Default()
    {
        using SubUpDownBase control = new();
        Assert.Null(control.ActiveControl);
        Assert.False(control.AllowDrop);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        Assert.False(control.AutoScroll);
        Assert.Equal(SizeF.Empty, control.AutoScaleDimensions);
        Assert.Equal(new SizeF(1, 1), control.AutoScaleFactor);
        Assert.Equal(Size.Empty, control.AutoScrollMargin);
        Assert.Equal(AutoScaleMode.Inherit, control.AutoScaleMode);
        Assert.Equal(Size.Empty, control.AutoScrollMinSize);
        Assert.Equal(Point.Empty, control.AutoScrollPosition);
        Assert.False(control.AutoSize);
        Assert.Equal(SystemColors.Window, control.BackColor);
        Assert.Null(control.BackgroundImage);
        Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
        Assert.NotNull(control.BindingContext);
        Assert.Same(control.BindingContext, control.BindingContext);
        Assert.Equal(BorderStyle.Fixed3D, control.BorderStyle);
        Assert.Equal(control.PreferredHeight, control.Bottom);
        Assert.Equal(new Rectangle(0, 0, 120, control.PreferredHeight), control.Bounds);
        Assert.False(control.CanEnableIme);
        Assert.False(control.CanFocus);
        Assert.True(control.CanRaiseEvents);
        Assert.True(control.CausesValidation);
        Assert.False(control.ChangingText);
        if (Application.UseVisualStyles)
        {
            Assert.Equal(new Rectangle(0, 0, 120, Control.DefaultFont.Height + 7), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 120, Control.DefaultFont.Height + 7), control.DisplayRectangle);
            Assert.Equal(new Size(120, Control.DefaultFont.Height + 7), control.ClientSize);
            Assert.Equal(new Size(122, control.PreferredHeight), control.PreferredSize);
        }
        else
        {
            Assert.Equal(new Rectangle(0, 0, 116, Control.DefaultFont.Height + 3), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 116, Control.DefaultFont.Height + 3), control.DisplayRectangle);
            Assert.Equal(new Size(116, Control.DefaultFont.Height + 3), control.ClientSize);
            Assert.Equal(new Size(123, control.PreferredHeight), control.PreferredSize);
        }

        Assert.Null(control.Container);
        Assert.False(control.ContainsFocus);
        Assert.Null(control.ContextMenuStrip);
        Assert.NotEmpty(control.Controls);
        Assert.Same(control.Controls, control.Controls);
        Assert.False(control.Created);
        Assert.Equal(SizeF.Empty, control.CurrentAutoScaleDimensions);
        Assert.Equal(Cursors.Default, control.Cursor);
        Assert.Equal(Cursors.Default, control.DefaultCursor);
        Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
        Assert.Equal(new Padding(3), control.DefaultMargin);
        Assert.Equal(Size.Empty, control.DefaultMaximumSize);
        Assert.Equal(Size.Empty, control.DefaultMinimumSize);
        Assert.Equal(Padding.Empty, control.DefaultPadding);
        Assert.Equal(new Size(120, control.PreferredHeight), control.DefaultSize);
        Assert.False(control.DesignMode);
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
        Assert.Equal(SystemColors.WindowText, control.ForeColor);
        Assert.True(control.HasChildren);
        Assert.Equal(control.PreferredHeight, control.Height);
        Assert.NotNull(control.HorizontalScroll);
        Assert.Same(control.HorizontalScroll, control.HorizontalScroll);
        Assert.False(control.HScroll);
        Assert.Equal(ImeMode.NoControl, control.ImeMode);
        Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
        Assert.True(control.InterceptArrowKeys);
        Assert.False(control.InvokeRequired);
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
        Assert.Equal(Control.DefaultFont.Height + 7, control.PreferredHeight);
        Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
        Assert.False(control.ReadOnly);
        Assert.False(control.RecreatingHandle);
        Assert.Null(control.Region);
        Assert.True(control.ResizeRedraw);
        Assert.Equal(120, control.Right);
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.True(control.ShowFocusCues);
        Assert.True(control.ShowKeyboardCues);
        Assert.Null(control.Site);
        Assert.Equal(new Size(120, control.PreferredHeight), control.Size);
        Assert.Equal(0, control.TabIndex);
        Assert.True(control.TabStop);
        Assert.Empty(control.Text);
        Assert.Equal(HorizontalAlignment.Left, control.TextAlign);
        Assert.Equal(0, control.Top);
        Assert.Null(control.TopLevelControl);
        Assert.Equal(LeftRightAlignment.Right, control.UpDownAlign);
        Assert.False(control.UserEdit);
        Assert.False(control.UseWaitCursor);
        Assert.True(control.Visible);
        Assert.NotNull(control.VerticalScroll);
        Assert.Same(control.VerticalScroll, control.VerticalScroll);
        Assert.False(control.VScroll);
        Assert.Equal(120, control.Width);

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void UpDownBase_CreateParams_GetDefault_ReturnsExpected()
    {
        using SubUpDownBase control = new();
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Null(createParams.ClassName);

        Assert.Equal(WNDCLASS_STYLES.CS_DBLCLKS, (WNDCLASS_STYLES)createParams.ClassStyle);
        Assert.Equal(WINDOW_STYLE.WS_MAXIMIZEBOX | WINDOW_STYLE.WS_CLIPCHILDREN | WINDOW_STYLE.WS_CLIPSIBLINGS |
            WINDOW_STYLE.WS_VISIBLE | WINDOW_STYLE.WS_CHILD, (WINDOW_STYLE)createParams.Style);

        if (Application.UseVisualStyles)
        {
            Assert.Equal(WINDOW_EX_STYLE.WS_EX_CONTROLPARENT, (WINDOW_EX_STYLE)createParams.ExStyle);
        }
        else
        {
            Assert.Equal(WINDOW_EX_STYLE.WS_EX_CLIENTEDGE | WINDOW_EX_STYLE.WS_EX_CONTROLPARENT, (WINDOW_EX_STYLE)createParams.ExStyle);
        }

        Assert.Equal(control.PreferredHeight, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(120, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(BorderStyle.None, 0x56010000, 0x10000)]
    [InlineData(BorderStyle.Fixed3D, 0x56010000, 0x10200)]
    [InlineData(BorderStyle.FixedSingle, 0x56810000, 0x10000)]
    public void UpDownSubUpDownBase_CreateParams_GetBorderStyleNoVisualStyles_ReturnsExpected(BorderStyle borderStyle, int expectedStyle, int expectedExStyle)
    {
        if (Application.RenderWithVisualStyles)
        {
            return;
        }

        using SubUpDownBase control = new()
        {
            BorderStyle = borderStyle
        };
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Null(createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(expectedExStyle, createParams.ExStyle);
        Assert.Equal(control.Height, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(expectedStyle, createParams.Style);
        Assert.Equal(120, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
    }

    [WinFormsTheory]
    [BoolData]
    public void UpDownBase_AutoScroll_Set_GetReturnsExpected(bool value)
    {
        using SubUpDownBase control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.AutoScroll = value;
        Assert.False(control.AutoScroll);
        Assert.False(control.GetScrollState(SubUpDownBase.ScrollStateAutoScrolling));
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AutoScroll = value;
        Assert.False(control.AutoScroll);
        Assert.False(control.GetScrollState(SubUpDownBase.ScrollStateAutoScrolling));
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.AutoScroll = !value;
        Assert.False(control.AutoScroll);
        Assert.False(control.GetScrollState(SubUpDownBase.ScrollStateAutoScrolling));
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void UpDownBase_AutoScroll_SetWithHandle_GetReturnsExpected(bool value)
    {
        using SubUpDownBase control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.AutoScroll = value;
        Assert.False(control.AutoScroll);
        Assert.False(control.GetScrollState(SubUpDownBase.ScrollStateAutoScrolling));
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.AutoScroll = value;
        Assert.False(control.AutoScroll);
        Assert.False(control.GetScrollState(SubUpDownBase.ScrollStateAutoScrolling));
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.AutoScroll = !value;
        Assert.False(control.AutoScroll);
        Assert.False(control.GetScrollState(SubUpDownBase.ScrollStateAutoScrolling));
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> AutoScrollMargin_Set_TestData()
    {
        yield return new object[] { true, new Size(0, 0) };
        yield return new object[] { true, new Size(1, 0) };
        yield return new object[] { true, new Size(0, 1) };
        yield return new object[] { true, new Size(1, 2) };
        yield return new object[] { false, new Size(0, 0) };
        yield return new object[] { false, new Size(1, 0) };
        yield return new object[] { false, new Size(0, 1) };
        yield return new object[] { false, new Size(1, 2) };
    }

    [WinFormsTheory]
    [MemberData(nameof(AutoScrollMargin_Set_TestData))]
    public void UpDownBase_AutoScrollMargin_Set_GetReturnsExpected(bool autoScroll, Size value)
    {
        using SubUpDownBase control = new()
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
        Assert.False(control.AutoScroll);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AutoScrollMargin = value;
        Assert.Equal(value, control.AutoScrollMargin);
        Assert.False(control.AutoScroll);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(AutoScrollMargin_Set_TestData))]
    public void UpDownBase_AutoScrollMargin_SetWithHandle_GetReturnsExpected(bool autoScroll, Size value)
    {
        using SubUpDownBase control = new()
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
        Assert.False(control.AutoScroll);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.AutoScrollMargin = value;
        Assert.Equal(value, control.AutoScrollMargin);
        Assert.False(control.AutoScroll);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> AutoScrollMinSize_TestData()
    {
        foreach (bool autoScroll in new bool[] { true, false })
        {
            yield return new object[] { autoScroll, new Size(-1, -2), 1 };
            yield return new object[] { autoScroll, new Size(0, 0), 0 };
            yield return new object[] { autoScroll, new Size(1, 0), 1 };
            yield return new object[] { autoScroll, new Size(0, 1), 1 };
            yield return new object[] { autoScroll, new Size(1, 2), 1 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(AutoScrollMinSize_TestData))]
    public void UpDownBase_AutoScrollMinSize_Set_GetReturnsExpected(bool autoScroll, Size value, int expectedLayoutCallCount)
    {
        using SubUpDownBase control = new()
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
        Assert.False(control.AutoScroll);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AutoScrollMinSize = value;
        Assert.Equal(value, control.AutoScrollMinSize);
        Assert.False(control.AutoScroll);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(AutoScrollMinSize_TestData))]
    public void UpDownBase_AutoScrollMinSize_SetWithHandle_GetReturnsExpected(bool autoScroll, Size value, int expectedLayoutCallCount)
    {
        using SubUpDownBase control = new()
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
        Assert.False(control.AutoScroll);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.AutoScrollMinSize = value;
        Assert.Equal(value, control.AutoScrollMinSize);
        Assert.False(control.AutoScroll);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void UpDownBase_AutoSize_Set_GetReturnsExpected(bool value)
    {
        using SubUpDownBase control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.AutoSize = value;
        Assert.Equal(value, control.AutoSize);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AutoSize = value;
        Assert.Equal(value, control.AutoSize);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.AutoSize = !value;
        Assert.Equal(!value, control.AutoSize);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void UpDownBase_AutoSize_SetWithHandler_CallsAutoSizeChanged()
    {
        using SubUpDownBase control = new()
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

    public static IEnumerable<object[]> BackColor_Set_TestData()
    {
        yield return new object[] { Color.Red, Color.Red };
        yield return new object[] { Color.Empty, SystemColors.Window };
    }

    [WinFormsTheory]
    [MemberData(nameof(BackColor_Set_TestData))]
    public void UpDownBase_BackColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using SubUpDownBase control = new()
        {
            BackColor = value
        };
        Assert.Equal(expected, control.BackColor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BackColor = value;
        Assert.Equal(expected, control.BackColor);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> BackColor_SetWithHandle_TestData()
    {
        yield return new object[] { Color.Red, Color.Red, 1 };
        yield return new object[] { Color.Empty, SystemColors.Window, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(BackColor_SetWithHandle_TestData))]
    public void UpDownBase_BackColor_SetWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
    {
        using SubUpDownBase control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.BackColor = value;
        Assert.Equal(expected, control.BackColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.BackColor = value;
        Assert.Equal(expected, control.BackColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void UpDownBase_BackColor_SetWithHandler_CallsBackColorChanged()
    {
        using SubUpDownBase control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.BackColorChanged += handler;

        // Set different.
        control.BackColor = Color.Red;
        Assert.Equal(Color.Red, control.BackColor);
        Assert.Equal(0, callCount);

        // Set same.
        control.BackColor = Color.Red;
        Assert.Equal(Color.Red, control.BackColor);
        Assert.Equal(0, callCount);

        // Set different.
        control.BackColor = Color.Empty;
        Assert.Equal(SystemColors.Window, control.BackColor);
        Assert.Equal(0, callCount);

        // Remove handler.
        control.BackColorChanged -= handler;
        control.BackColor = Color.Red;
        Assert.Equal(Color.Red, control.BackColor);
        Assert.Equal(0, callCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetImageTheoryData))]
    public void UpDownBase_BackgroundImage_Set_GetReturnsExpected(Image value)
    {
        using SubUpDownBase control = new()
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
    public void UpDownBase_BackgroundImage_SetWithHandler_CallsBackgroundImageChanged()
    {
        using SubUpDownBase control = new();
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        }

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
    public void UpDownBase_BackgroundImageLayout_Set_GetReturnsExpected(ImageLayout value)
    {
        using SubUpDownBase control = new()
        {
            BackgroundImageLayout = value
        };
        Assert.Equal(value, control.BackgroundImageLayout);
        Assert.False(control.DoubleBuffered);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BackgroundImageLayout = value;
        Assert.Equal(value, control.BackgroundImageLayout);
        Assert.False(control.DoubleBuffered);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void UpDownBase_BackgroundImageLayout_SetWithHandler_CallsBackgroundImageLayoutChanged()
    {
        using SubUpDownBase control = new();
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        }

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

    public static IEnumerable<object[]> BorderStyle_Set_TestData()
    {
        if (Application.UseVisualStyles)
        {
            yield return new object[] { BorderStyle.Fixed3D, new Size(122, Control.DefaultFont.Height + SystemInformation.BorderSize.Height * 4 + 3) };
            yield return new object[] { BorderStyle.FixedSingle, new Size(122, Control.DefaultFont.Height + SystemInformation.BorderSize.Height * 4 + 3) };
            yield return new object[] { BorderStyle.None, new Size(122, Control.DefaultFont.Height + 3) };
        }
        else
        {
            yield return new object[] { BorderStyle.Fixed3D, new Size(123, Control.DefaultFont.Height + SystemInformation.BorderSize.Height * 4 + 3) };
            yield return new object[] { BorderStyle.FixedSingle, new Size(121, Control.DefaultFont.Height + SystemInformation.BorderSize.Height * 4 + 3) };
            yield return new object[] { BorderStyle.None, new Size(119, Control.DefaultFont.Height + 3) };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(BorderStyle_Set_TestData))]
    public void UpDownBase_BorderStyle_Set_GetReturnsExpected(BorderStyle value, Size expectedPreferredSize)
    {
        using SubUpDownBase control = new()
        {
            BorderStyle = value
        };
        Assert.Equal(value, control.BorderStyle);
        Assert.Equal(expectedPreferredSize.Height, control.PreferredHeight);
        Assert.Equal(expectedPreferredSize, control.PreferredSize);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BorderStyle = value;
        Assert.Equal(value, control.BorderStyle);
        Assert.Equal(expectedPreferredSize.Height, control.PreferredHeight);
        Assert.Equal(expectedPreferredSize, control.PreferredSize);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> BorderStyle_SetWithHandle_TestData()
    {
        if (Application.UseVisualStyles)
        {
            yield return new object[] { BorderStyle.Fixed3D, 0, 0, new Size(122, Control.DefaultFont.Height + SystemInformation.BorderSize.Height * 4 + 3) };
            yield return new object[] { BorderStyle.FixedSingle, 0, 1, new Size(122, Control.DefaultFont.Height + SystemInformation.BorderSize.Height * 4 + 3) };
            yield return new object[] { BorderStyle.None, 1, 1, new Size(123, Control.DefaultFont.Height + 3) };
        }
        else
        {
            yield return new object[] { BorderStyle.Fixed3D, 0, 0, new Size(123, Control.DefaultFont.Height + SystemInformation.BorderSize.Height * 4 + 3) };
            yield return new object[] { BorderStyle.FixedSingle, 1, 1, new Size(123, Control.DefaultFont.Height + SystemInformation.BorderSize.Height * 4 + 3) };
            yield return new object[] { BorderStyle.None, 2, 1, new Size(123, Control.DefaultFont.Height + 3) };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(BorderStyle_SetWithHandle_TestData))]
    public void UpDownBase_BorderStyle_SetWithHandle_GetReturnsExpected(BorderStyle value, int expectedInvalidatedCallCount, int expectedCreatedCallCount, Size expectedPreferredSize)
    {
        using SubUpDownBase control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.BorderStyle = value;
        Assert.Equal(value, control.BorderStyle);
        Assert.Equal(expectedPreferredSize.Height, control.PreferredHeight);
        Assert.Equal(expectedPreferredSize, control.PreferredSize);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        control.BorderStyle = value;
        Assert.Equal(value, control.BorderStyle);
        Assert.Equal(expectedPreferredSize.Height, control.PreferredHeight);
        Assert.Equal(expectedPreferredSize, control.PreferredSize);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<BorderStyle>]
    public void UpDownBase_BorderStyle_SetInvalid_ThrowsInvalidEnumArgumentException(BorderStyle value)
    {
        using SubUpDownBase control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.BorderStyle = value);
    }

    [WinFormsTheory]
    [BoolData]
    public void UpDownBase_ChangingText_Set_GetReturnsExpected(bool value)
    {
        using SubUpDownBase control = new()
        {
            ChangingText = value
        };
        Assert.Equal(value, control.ChangingText);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ChangingText = value;
        Assert.Equal(value, control.ChangingText);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.ChangingText = !value;
        Assert.Equal(!value, control.ChangingText);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void UpDownBase_ChangingText_SetWithHandle_GetReturnsExpected(bool value)
    {
        using SubUpDownBase control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ChangingText = value;
        Assert.Equal(value, control.ChangingText);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.ChangingText = value;
        Assert.Equal(value, control.ChangingText);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.ChangingText = !value;
        Assert.Equal(!value, control.ChangingText);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
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
    public void UpDownBase_ContextMenuStrip_Set_GetReturnsExpected(ContextMenuStrip value)
    {
        using SubUpDownBase control = new()
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
    public void UpDownBase_ContextMenuStrip_SetWithHandler_CallsContextMenuStripChanged()
    {
        using SubUpDownBase control = new();
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

    public static IEnumerable<object[]> ForeColor_Set_TestData()
    {
        yield return new object[] { Color.Empty, SystemColors.WindowText };
        yield return new object[] { Color.FromArgb(254, 1, 2, 3), Color.FromArgb(254, 1, 2, 3) };
        yield return new object[] { Color.White, Color.White };
        yield return new object[] { Color.Black, Color.Black };
        yield return new object[] { Color.Red, Color.Red };
    }

    [WinFormsTheory]
    [MemberData(nameof(ForeColor_Set_TestData))]
    public void UpDownBase_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using SubUpDownBase control = new()
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

    [WinFormsTheory]
    [MemberData(nameof(ForeColor_Set_TestData))]
    public void UpDownBase_ForeColor_SetWithHandle_GetReturnsExpected(Color value, Color expected)
    {
        using SubUpDownBase control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ForeColor = value;
        Assert.Equal(expected, control.ForeColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.ForeColor = value;
        Assert.Equal(expected, control.ForeColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void UpDownBase_ForeColor_SetWithHandler_CallsForeColorChanged()
    {
        using SubUpDownBase control = new();
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
        Assert.Equal(0, callCount);

        // Set same.
        control.ForeColor = Color.Red;
        Assert.Equal(Color.Red, control.ForeColor);
        Assert.Equal(0, callCount);

        // Set different.
        control.ForeColor = Color.Empty;
        Assert.Equal(SystemColors.WindowText, control.ForeColor);
        Assert.Equal(0, callCount);

        // Remove handler.
        control.ForeColorChanged -= handler;
        control.ForeColor = Color.Red;
        Assert.Equal(Color.Red, control.ForeColor);
        Assert.Equal(0, callCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void UpDownBase_InterceptArrowKeys_Set_GetReturnsExpected(bool value)
    {
        using SubUpDownBase control = new()
        {
            InterceptArrowKeys = value
        };
        Assert.Equal(value, control.InterceptArrowKeys);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.InterceptArrowKeys = value;
        Assert.Equal(value, control.InterceptArrowKeys);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.InterceptArrowKeys = !value;
        Assert.Equal(!value, control.InterceptArrowKeys);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void UpDownBase_InterceptArrowKeys_SetWithHandle_GetReturnsExpected(bool value)
    {
        using SubUpDownBase control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.InterceptArrowKeys = value;
        Assert.Equal(value, control.InterceptArrowKeys);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.InterceptArrowKeys = value;
        Assert.Equal(value, control.InterceptArrowKeys);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.InterceptArrowKeys = !value;
        Assert.Equal(!value, control.InterceptArrowKeys);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> MaximumSize_Set_TestData()
    {
        yield return new object[] { Size.Empty, 120, 0 };
        yield return new object[] { new Size(-1, -2), 0, 1 };
        yield return new object[] { new Size(0, 1), 120, 0 };
        yield return new object[] { new Size(0, 10), 120, 0 };
        yield return new object[] { new Size(1, 0), 1, 1 };
        yield return new object[] { new Size(10, 0), 10, 1 };
        yield return new object[] { new Size(1, 2), 1, 1 };
        yield return new object[] { new Size(3, 4), 3, 1 };
        yield return new object[] { new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), 120, 0 };
        yield return new object[] { new Size(ushort.MaxValue, ushort.MaxValue), 120, 0 };
        yield return new object[] { new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), 120, 0 };
        yield return new object[] { new Size(int.MaxValue, int.MaxValue), 120, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(MaximumSize_Set_TestData))]
    public void UpDownBase_MaximumSize_Set_GetReturnsExpected(Size value, int expectedWidth, int expectedLayoutCallCount)
    {
        using SubUpDownBase control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            layoutCallCount++;
        };

        control.MaximumSize = value;
        Assert.Equal(new Size(value.Width, 0), control.MaximumSize);
        Assert.Equal(new Size(expectedWidth, control.PreferredHeight), control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.MaximumSize = value;
        Assert.Equal(new Size(value.Width, 0), control.MaximumSize);
        Assert.Equal(new Size(expectedWidth, control.PreferredHeight), control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> MinimumSize_Set_TestData()
    {
        yield return new object[] { Size.Empty, 120, 0 };
        yield return new object[] { new Size(0, 1), 120, 0 };
        yield return new object[] { new Size(0, 10), 120, 0 };
        yield return new object[] { new Size(1, 0), 120, 0 };
        yield return new object[] { new Size(10, 0), 120, 0 };
        yield return new object[] { new Size(-1, -2), 120, 0 };
        yield return new object[] { new Size(1, 2), 120, 0 };
        yield return new object[] { new Size(3, 4), 120, 0 };
        yield return new object[] { new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), ushort.MaxValue - 1, 1 };
        yield return new object[] { new Size(ushort.MaxValue, ushort.MaxValue), ushort.MaxValue, 1 };
        yield return new object[] { new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), ushort.MaxValue + 1, 1 };
        yield return new object[] { new Size(int.MaxValue, int.MaxValue), int.MaxValue, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(MinimumSize_Set_TestData))]
    public void UpDownBase_MinimumSize_Set_GetReturnsExpected(Size value, int expectedWidth, int expectedLayoutCallCount)
    {
        using SubUpDownBase control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            layoutCallCount++;
        };

        control.MinimumSize = value;
        Assert.Equal(new Size(value.Width, 0), control.MinimumSize);
        Assert.Equal(new Size(expectedWidth, control.PreferredHeight), control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.MinimumSize = value;
        Assert.Equal(new Size(value.Width, 0), control.MinimumSize);
        Assert.Equal(new Size(expectedWidth, control.PreferredHeight), control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void UpDownBase_ReadOnly_Set_GetReturnsExpected(bool value)
    {
        using SubUpDownBase control = new()
        {
            ReadOnly = value
        };
        Assert.Equal(value, control.ReadOnly);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ReadOnly = value;
        Assert.Equal(value, control.ReadOnly);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.ReadOnly = !value;
        Assert.Equal(!value, control.ReadOnly);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void UpDownBase_ReadOnly_SetWithHandle_GetReturnsExpected(bool value)
    {
        using SubUpDownBase control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ReadOnly = value;
        Assert.Equal(value, control.ReadOnly);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.ReadOnly = value;
        Assert.Equal(value, control.ReadOnly);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.ReadOnly = !value;
        Assert.Equal(!value, control.ReadOnly);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> Text_Set_TestData()
    {
        foreach (bool changingText in new bool[] { true, false })
        {
            yield return new object[] { changingText, true, null, string.Empty, 1, 0, true };
            yield return new object[] { changingText, false, null, string.Empty, 0, 0, false };
            yield return new object[] { changingText, true, string.Empty, string.Empty, 1, 0, true };
            yield return new object[] { changingText, false, string.Empty, string.Empty, 0, 0, false };
        }

        yield return new object[] { true, true, "text", "text", 1, 1, true };
        yield return new object[] { false, true, "text", "text", 1, 1, true };
        yield return new object[] { true, false, "text", "text", 0, 1, false };
        yield return new object[] { false, false, "text", "text", 1, 1, true };
    }

    [WinFormsTheory]
    [MemberData(nameof(Text_Set_TestData))]
    public void UpDownBase_Text_Set_GetReturnsExpected(bool changingText, bool userEdit, string value, string expected, int expectedValidateEditTextCallCount, int expectedOnChangedCallCount, bool expectedUserEdit)
    {
        using (new NoAssertContext())
        {
            int validateEditTextCallCount = 0;
            int onChangedCallCount = 0;
            using CustomValidateUpDownBase control = new()
            {
                ChangingText = changingText,
                UserEdit = userEdit,
                ValidateEditTextAction = () => validateEditTextCallCount++
            };
            control.OnChangedAction = (source, e) =>
            {
                Assert.Same(control, ((Control)source).Parent);
                Assert.Same(EventArgs.Empty, e);
                onChangedCallCount++;
            };

            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.False(control.ChangingText);
            Assert.Equal(expectedUserEdit, control.UserEdit);
            Assert.Equal(expectedValidateEditTextCallCount, validateEditTextCallCount);
            Assert.Equal(expectedOnChangedCallCount, onChangedCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.False(control.ChangingText);
            Assert.Equal(expectedUserEdit, control.UserEdit);
            Assert.Equal(expectedValidateEditTextCallCount * 2, validateEditTextCallCount);
            Assert.Equal(expectedOnChangedCallCount, onChangedCallCount);
            Assert.False(control.IsHandleCreated);
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Text_Set_TestData))]
    public void UpDownBase_Text_SetWithHandle_GetReturnsExpected(bool changingText, bool userEdit, string value, string expected, int expectedValidateEditTextCallCount, int expectedOnChangedCallCount, bool expectedUserEdit)
    {
        using (new NoAssertContext())
        {
            int validateEditTextCallCount = 0;
            int onChangedCallCount = 0;
            using CustomValidateUpDownBase control = new()
            {
                ChangingText = changingText,
                UserEdit = userEdit,
                ValidateEditTextAction = () => validateEditTextCallCount++
            };
            control.OnChangedAction = (source, e) =>
            {
                Assert.Same(control, ((Control)source).Parent);
                Assert.Same(EventArgs.Empty, e);
                onChangedCallCount++;
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.False(control.ChangingText);
            Assert.Equal(expectedUserEdit, control.UserEdit);
            Assert.Equal(expectedValidateEditTextCallCount, validateEditTextCallCount);
            Assert.Equal(expectedOnChangedCallCount, onChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.False(control.ChangingText);
            Assert.Equal(expectedUserEdit, control.UserEdit);
            Assert.Equal(expectedValidateEditTextCallCount * 2, validateEditTextCallCount);
            Assert.Equal(expectedOnChangedCallCount, onChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
    }

    [WinFormsFact]
    public void UpDownBase_Text_SetWithHandler_CallsTextChanged()
    {
        using SubUpDownBase control = new();
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
    [EnumData<HorizontalAlignment>]
    public void UpDownBase_TextAlign_Set_GetReturnsExpected(HorizontalAlignment value)
    {
        using SubUpDownBase control = new()
        {
            TextAlign = value
        };
        Assert.Equal(value, control.TextAlign);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.TextAlign = value;
        Assert.Equal(value, control.TextAlign);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [EnumData<HorizontalAlignment>]
    public void UpDownBase_TextAlign_SetWithHandle_GetReturnsExpected(HorizontalAlignment value)
    {
        using SubUpDownBase control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.TextAlign = value;
        Assert.Equal(value, control.TextAlign);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.TextAlign = value;
        Assert.Equal(value, control.TextAlign);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<HorizontalAlignment>]
    public void UpDownBase_TextAlign_SetInvalidValue_ThrowsInvalidEnumArgumentException(HorizontalAlignment value)
    {
        SubUpDownBase control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.TextAlign = value);
    }

    public static IEnumerable<object[]> UpDownAlign_Set_TestData()
    {
        foreach (BorderStyle borderStyle in Enum.GetValues(typeof(BorderStyle)))
        {
            foreach (RightToLeft rightToLeft in Enum.GetValues(typeof(RightToLeft)))
            {
                yield return new object[] { borderStyle, rightToLeft, LeftRightAlignment.Left };
                yield return new object[] { borderStyle, rightToLeft, LeftRightAlignment.Right };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(UpDownAlign_Set_TestData))]
    public void UpDownBase_UpDownAlign_Set_GetReturnsExpected(BorderStyle borderStyle, RightToLeft rightToLeft, LeftRightAlignment value)
    {
        using SubUpDownBase control = new()
        {
            BorderStyle = borderStyle,
            RightToLeft = rightToLeft,
            UpDownAlign = value
        };
        Assert.Equal(value, control.UpDownAlign);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.UpDownAlign = value;
        Assert.Equal(value, control.UpDownAlign);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> UpDownAlign_SetWithHandle_TestData()
    {
        foreach (BorderStyle borderStyle in Enum.GetValues(typeof(BorderStyle)))
        {
            foreach (RightToLeft rightToLeft in Enum.GetValues(typeof(RightToLeft)))
            {
                yield return new object[] { borderStyle, rightToLeft, LeftRightAlignment.Left, 1 };
                yield return new object[] { borderStyle, rightToLeft, LeftRightAlignment.Right, 0 };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(UpDownAlign_SetWithHandle_TestData))]
    public void UpDownBase_UpDownAlign_SetWithHandle_GetReturnsExpected(BorderStyle borderStyle, RightToLeft rightToLeft, LeftRightAlignment value, int expectedInvalidatedCallCount)
    {
        using SubUpDownBase control = new()
        {
            BorderStyle = borderStyle,
            RightToLeft = rightToLeft
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.UpDownAlign = value;
        Assert.Equal(value, control.UpDownAlign);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.UpDownAlign = value;
        Assert.Equal(value, control.UpDownAlign);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<LeftRightAlignment>]
    public void UpDownBase_UpDownAlign_SetInvalidValue_ThrowsInvalidEnumArgumentException(LeftRightAlignment value)
    {
        SubUpDownBase control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.UpDownAlign = value);
    }

    [WinFormsTheory]
    [BoolData]
    public void UpDownBase_UserEdit_Set_GetReturnsExpected(bool value)
    {
        using SubUpDownBase control = new()
        {
            UserEdit = value
        };
        Assert.Equal(value, control.UserEdit);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.UserEdit = value;
        Assert.Equal(value, control.UserEdit);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.UserEdit = !value;
        Assert.Equal(!value, control.UserEdit);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void UpDownBase_UserEdit_SetWithHandle_GetReturnsExpected(bool value)
    {
        using SubUpDownBase control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.UserEdit = value;
        Assert.Equal(value, control.UserEdit);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.UserEdit = value;
        Assert.Equal(value, control.UserEdit);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.UserEdit = !value;
        Assert.Equal(!value, control.UserEdit);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void UpDownBase_GetAutoSizeMode_Invoke_ReturnsExpected()
    {
        using SubUpDownBase control = new();
        Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
    }

    public static IEnumerable<object[]> GetPreferredSize_TestData()
    {
        yield return new object[] { Size.Empty };
        yield return new object[] { new Size(-1, -2) };
        yield return new object[] { new Size(10, 20) };
        yield return new object[] { new Size(30, 40) };
        yield return new object[] { new Size(int.MaxValue, int.MaxValue) };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetPreferredSize_TestData))]
    public void UpDownBase_GetPreferredSize_Invoke_ReturnsExpected(Size proposedSize)
    {
        using SubUpDownBase control = new();

        int expectedWidth = Application.UseVisualStyles ? 122 : 123;
        Size preferredSize = control.GetPreferredSize(proposedSize);
        Assert.Equal(new Size(expectedWidth, control.PreferredHeight), preferredSize);

        // Call again.
        preferredSize = control.GetPreferredSize(proposedSize);
        Assert.Equal(new Size(expectedWidth, control.PreferredHeight), preferredSize);

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(GetPreferredSize_TestData))]
    public void UpDownBase_GetPreferredSize_InvokeWithBounds_ReturnsExpected(Size proposedSize)
    {
        using SubUpDownBase control = new()
        {
            Bounds = new Rectangle(1, 2, 30, 40)
        };

        int expectedWidth = Application.UseVisualStyles ? 32 : 33;

        Assert.Equal(new Size(expectedWidth, control.PreferredHeight), control.GetPreferredSize(proposedSize));
        Assert.False(control.IsHandleCreated);

        // Call again.
        Assert.Equal(new Size(expectedWidth, control.PreferredHeight), control.GetPreferredSize(proposedSize));
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> GetPreferredSize_WithConstrainedSize_TestData()
    {
        int expectedWidth = Application.UseVisualStyles ? 122 : 123;

        yield return new object[] { Size.Empty, Size.Empty, new Size(30, 40), expectedWidth };
        yield return new object[] { new Size(10, 20), Size.Empty, new Size(30, 40), expectedWidth };
        yield return new object[] { new Size(30, 40), Size.Empty, new Size(30, 40), expectedWidth };
        yield return new object[] { new Size(31, 40), Size.Empty, new Size(30, 40), expectedWidth };
        yield return new object[] { new Size(30, 41), Size.Empty, new Size(30, 40), expectedWidth };
        yield return new object[] { new Size(40, 50), Size.Empty, new Size(30, 40), expectedWidth };

        yield return new object[] { Size.Empty, new Size(20, 10), new Size(30, 40), 20 };
        yield return new object[] { Size.Empty, new Size(30, 40), new Size(30, 40), 30 };
        yield return new object[] { Size.Empty, new Size(31, 40), new Size(30, 40), 31 };
        yield return new object[] { Size.Empty, new Size(30, 41), new Size(30, 40), 30 };
        yield return new object[] { Size.Empty, new Size(40, 50), new Size(30, 40), 40 };
        yield return new object[] { new Size(10, 20), new Size(40, 50), new Size(30, 40), 40 };
        yield return new object[] { new Size(10, 20), new Size(40, 50), new Size(int.MaxValue, int.MaxValue), 40 };
        yield return new object[] { new Size(10, 20), new Size(20, 30), new Size(30, 40), 20 };
        yield return new object[] { new Size(10, 20), new Size(20, 30), new Size(30, 40), 20 };
        yield return new object[] { new Size(30, 40), new Size(20, 30), new Size(30, 40), 30 };
        yield return new object[] { new Size(30, 40), new Size(40, 50), new Size(30, 40), 40 };
        yield return new object[] { new Size(40, 50), new Size(20, 30), new Size(30, 40), 40 };
        yield return new object[] { new Size(40, 50), new Size(40, 50), new Size(30, 40), 40 };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetPreferredSize_WithConstrainedSize_TestData))]
    public void UpDownBase_GetPreferredSize_InvokeWithConstrainedSize_ReturnsExpected(Size minimumSize, Size maximumSize, Size proposedSize, int expectedWidth)
    {
        using SubUpDownBase control = new()
        {
            MinimumSize = minimumSize,
            MaximumSize = maximumSize,
        };
        Assert.Equal(new Size(expectedWidth, control.PreferredHeight), control.GetPreferredSize(proposedSize));
        Assert.False(control.IsHandleCreated);

        // Call again.
        Assert.Equal(new Size(expectedWidth, control.PreferredHeight), control.GetPreferredSize(proposedSize));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0, true)]
    [InlineData(SubUpDownBase.ScrollStateAutoScrolling, false)]
    [InlineData(SubUpDownBase.ScrollStateFullDrag, false)]
    [InlineData(SubUpDownBase.ScrollStateHScrollVisible, false)]
    [InlineData(SubUpDownBase.ScrollStateUserHasScrolled, false)]
    [InlineData(SubUpDownBase.ScrollStateVScrollVisible, false)]
    [InlineData(int.MaxValue, false)]
    [InlineData((-1), false)]
    public void UpDownBase_GetScrollState_Invoke_ReturnsExpected(int bit, bool expected)
    {
        using SubUpDownBase control = new();
        Assert.Equal(expected, control.GetScrollState(bit));
    }

    [WinFormsTheory]
    [InlineData(ControlStyles.ContainerControl, true)]
    [InlineData(ControlStyles.UserPaint, true)]
    [InlineData(ControlStyles.Opaque, true)]
    [InlineData(ControlStyles.ResizeRedraw, true)]
    [InlineData(ControlStyles.FixedWidth, false)]
    [InlineData(ControlStyles.FixedHeight, true)]
    [InlineData(ControlStyles.StandardClick, false)]
    [InlineData(ControlStyles.Selectable, true)]
    [InlineData(ControlStyles.UserMouse, false)]
    [InlineData(ControlStyles.SupportsTransparentBackColor, false)]
    [InlineData(ControlStyles.StandardDoubleClick, true)]
    [InlineData(ControlStyles.AllPaintingInWmPaint, false)]
    [InlineData(ControlStyles.CacheText, false)]
    [InlineData(ControlStyles.EnableNotifyMessage, false)]
    [InlineData(ControlStyles.DoubleBuffer, false)]
    [InlineData(ControlStyles.OptimizedDoubleBuffer, false)]
    [InlineData(ControlStyles.UseTextForAccessibility, false)]
    [InlineData((ControlStyles)0, true)]
    [InlineData((ControlStyles)int.MaxValue, false)]
    [InlineData((ControlStyles)(-1), false)]
    public void UpDownBase_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
    {
        using SubUpDownBase control = new();
        Assert.Equal(expected, control.GetStyle(flag));

        // Call again to test caching.
        Assert.Equal(expected, control.GetStyle(flag));
    }

    [WinFormsFact]
    public void UpDownBase_GetTopLevel_Invoke_ReturnsExpected()
    {
        using SubUpDownBase control = new();
        Assert.False(control.GetTopLevel());
    }

    public static IEnumerable<object[]> OnChanged_TestData()
    {
        yield return new object[] { null, null };
        yield return new object[] { new(), new EventArgs() };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnChanged_TestData))]
    public void UpDownBase_OnChanged_Invoke_Nop(object source, EventArgs e)
    {
        using SubUpDownBase control = new();
        control.OnChanged(source, e);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.OnChanged(source, e);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnChanged_TestData))]
    public void UpDownBase_OnChanged_InvokeWithHandle_Nop(object source, EventArgs e)
    {
        using SubUpDownBase control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.OnChanged(source, e);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.OnChanged(source, e);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> OnFontChanged_TestData()
    {
        foreach (BorderStyle borderStyle in Enum.GetValues(typeof(BorderStyle)))
        {
            foreach (RightToLeft rightToLeft in Enum.GetValues(typeof(RightToLeft)))
            {
                foreach (LeftRightAlignment upDownAlign in Enum.GetValues(typeof(LeftRightAlignment)))
                {
                    yield return new object[] { borderStyle, rightToLeft, upDownAlign, null };
                    yield return new object[] { borderStyle, rightToLeft, upDownAlign, new EventArgs() };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnFontChanged_TestData))]
    public void UpDownBase_OnFontChanged_Invoke_CallsFontChanged(BorderStyle borderStyle, RightToLeft rightToLeft, LeftRightAlignment upDownAlign, EventArgs eventArgs)
    {
        using SubUpDownBase control = new()
        {
            BorderStyle = borderStyle,
            RightToLeft = rightToLeft,
            UpDownAlign = upDownAlign
        };
        int preferredHeight = control.PreferredHeight;

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.FontChanged += handler;
        control.OnFontChanged(eventArgs);
        Assert.Equal(preferredHeight, control.Height);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.FontChanged -= handler;
        control.OnFontChanged(eventArgs);
        Assert.Equal(preferredHeight, control.Height);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> OnHandleCreated_TestData()
    {
        foreach (BorderStyle borderStyle in Enum.GetValues(typeof(BorderStyle)))
        {
            foreach (RightToLeft rightToLeft in Enum.GetValues(typeof(RightToLeft)))
            {
                foreach (LeftRightAlignment upDownAlign in Enum.GetValues(typeof(LeftRightAlignment)))
                {
                    yield return new object[] { borderStyle, rightToLeft, upDownAlign, null };
                    yield return new object[] { borderStyle, rightToLeft, upDownAlign, new EventArgs() };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnHandleCreated_TestData))]
    public void UpDownBase_OnHandleCreated_Invoke_CallsHandleCreated(BorderStyle borderStyle, RightToLeft rightToLeft, LeftRightAlignment upDownAlign, EventArgs eventArgs)
    {
        using SubUpDownBase control = new()
        {
            BorderStyle = borderStyle,
            RightToLeft = rightToLeft,
            UpDownAlign = upDownAlign
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
        Assert.False(control.Created);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.HandleCreated -= handler;
        control.OnHandleCreated(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.Created);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnHandleCreated_TestData))]
    public void UpDownBase_OnHandleCreated_InvokeWithHandle_CallsHandleCreated(BorderStyle borderStyle, RightToLeft rightToLeft, LeftRightAlignment upDownAlign, EventArgs eventArgs)
    {
        using SubUpDownBase control = new()
        {
            BorderStyle = borderStyle,
            RightToLeft = rightToLeft,
            UpDownAlign = upDownAlign
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);

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
        Assert.True(control.Created);
        Assert.True(control.IsHandleCreated);

        // Remove handler.
        control.HandleCreated -= handler;
        control.OnHandleCreated(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.Created);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void UpDownBase_OnHandleDestroyed_Invoke_CallsHandleDestroyed(EventArgs eventArgs)
    {
        using SubUpDownBase control = new();
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
        Assert.False(control.Created);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.HandleDestroyed -= handler;
        control.OnHandleDestroyed(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.Created);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void UpDownBase_OnHandleDestroyed_InvokeWithHandle_CallsHandleDestroyed(EventArgs eventArgs)
    {
        using SubUpDownBase control = new();
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
        Assert.True(control.Created);
        Assert.True(control.IsHandleCreated);

        // Remove handler.
        control.HandleDestroyed -= handler;
        control.OnHandleDestroyed(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.Created);
        Assert.True(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> OnLayout_TestData()
    {
        foreach (BorderStyle borderStyle in Enum.GetValues(typeof(BorderStyle)))
        {
            foreach (RightToLeft rightToLeft in Enum.GetValues(typeof(RightToLeft)))
            {
                foreach (LeftRightAlignment upDownAlign in Enum.GetValues(typeof(LeftRightAlignment)))
                {
                    yield return new object[] { borderStyle, rightToLeft, upDownAlign, null };
                    yield return new object[] { borderStyle, rightToLeft, upDownAlign, new LayoutEventArgs(null, null) };
                    yield return new object[] { borderStyle, rightToLeft, upDownAlign, new LayoutEventArgs(new Control(), "affectedProperty") };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnLayout_TestData))]
    public void UpDownBase_OnLayout_Invoke_CallsLayout(BorderStyle borderStyle, RightToLeft rightToLeft, LeftRightAlignment upDownAlign, LayoutEventArgs eventArgs)
    {
        using SubUpDownBase control = new()
        {
            BorderStyle = borderStyle,
            RightToLeft = rightToLeft,
            UpDownAlign = upDownAlign
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
        Assert.NotEqual(0, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.Layout -= handler;
        control.OnLayout(eventArgs);
        Assert.NotEqual(0, callCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> MouseEventArgs_TestData()
    {
        yield return new object[] { new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0) };
        yield return new object[] { new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0) };
        yield return new object[] { new MouseEventArgs(MouseButtons.Right, 0, 0, 0, 0) };
        yield return new object[] { new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0) };
        yield return new object[] { new MouseEventArgs(MouseButtons.Right, 1, 0, 0, 0) };
        yield return new object[] { new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0) };
        yield return new object[] { new MouseEventArgs(MouseButtons.Right, 2, 0, 0, 0) };
        yield return new object[] { new MouseEventArgs(MouseButtons.Left, 3, 0, 0, 0) };
        yield return new object[] { new MouseEventArgs(MouseButtons.Right, 3, 0, 0, 0) };
    }

    [WinFormsTheory]
    [MemberData(nameof(MouseEventArgs_TestData))]
    public void UpDownBase_OnMouseDown_Invoke_CallsMouseDown(MouseEventArgs eventArgs)
    {
        using SubUpDownBase control = new();
        int callCount = 0;
        MouseEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.MouseDown += handler;
        control.OnMouseDown(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.MouseDown -= handler;
        control.OnMouseDown(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void UpDownBase_OnMouseDown_NullE_ThrowsNullReferenceException()
    {
        using SubUpDownBase control = new();
        Assert.Throws<NullReferenceException>(() => control.OnMouseDown(null));
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void UpDownBase_OnMouseEnter_Invoke_CallsMouseEnter(EventArgs eventArgs)
    {
        using SubUpDownBase control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.MouseEnter += handler;
        control.OnMouseEnter(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.MouseEnter -= handler;
        control.OnMouseEnter(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void UpDownSubUpDownBase_OnMouseHover_Invoke_CallsMouseHover(EventArgs eventArgs)
    {
        using SubUpDownBase control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.MouseHover += handler;
        control.OnMouseHover(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.MouseHover -= handler;
        control.OnMouseHover(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void UpDownBase_OnMouseLeave_Invoke_CallsMouseLeave(EventArgs eventArgs)
    {
        using SubUpDownBase control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.MouseLeave += handler;
        control.OnMouseLeave(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.MouseLeave -= handler;
        control.OnMouseLeave(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetMouseEventArgsTheoryData))]
    public void UpDownBase_OnMouseMove_Invoke_CallsMouseMove(MouseEventArgs eventArgs)
    {
        using SubUpDownBase control = new();
        int callCount = 0;
        MouseEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.MouseMove += handler;
        control.OnMouseMove(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.MouseMove -= handler;
        control.OnMouseMove(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(MouseEventArgs_TestData))]
    public void UpUpBase_OnMouseUp_Invoke_CallsMouseUp(MouseEventArgs eventArgs)
    {
        using SubUpDownBase control = new();
        int callCount = 0;
        MouseEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.MouseUp += handler;
        control.OnMouseUp(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.MouseUp -= handler;
        control.OnMouseUp(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void UpDownBase_OnMouseUp_NullE_ThrowsNullReferenceException()
    {
        using SubUpDownBase control = new();
        Assert.Throws<NullReferenceException>(() => control.OnMouseUp(null));
    }

    [WinFormsTheory]
    [MemberData(nameof(MouseEventArgs_TestData))]
    public void UpDownBase_OnMouseWheel_Invoke_CallsMouseWheel(MouseEventArgs eventArgs)
    {
        using SubUpDownBase control = new();
        int callCount = 0;
        MouseEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.MouseWheel += handler;
        control.OnMouseWheel(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.MouseWheel -= handler;
        control.OnMouseWheel(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void UpDownBase_OnMouseWheel_InvokeHandledMouseEventArgs_SetsHandled(bool handled)
    {
        using SubUpDownBase control = new();
        HandledMouseEventArgs eventArgs = new(MouseButtons.Left, 1, 2, 3, 4, handled);
        int callCount = 0;
        MouseEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            Assert.Equal(handled, eventArgs.Handled);
            callCount++;
        };
        control.MouseWheel += handler;

        control.OnMouseWheel(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(eventArgs.Handled);
    }

    [WinFormsFact]
    public void UpDownBase_OnMouseWheel_NullE_ThrowsNullReferenceException()
    {
        using SubUpDownBase control = new();
        Assert.Throws<NullReferenceException>(() => control.OnMouseWheel(null));
    }

    public static IEnumerable<object[]> OnPaint_TestData()
    {
        foreach (bool enabled in new bool[] { true, false })
        {
            foreach (BorderStyle borderStyle in Enum.GetValues(typeof(BorderStyle)))
            {
                foreach (Color backColor in new Color[] { Color.Red, Color.Empty })
                {
                    yield return new object[] { new Size(100, 200), enabled, borderStyle, backColor };
                    yield return new object[] { new Size(10, 10), enabled, borderStyle, backColor };
                    yield return new object[] { new Size(9, 10), enabled, borderStyle, backColor };
                    yield return new object[] { new Size(10, 9), enabled, borderStyle, backColor };
                    yield return new object[] { new Size(9, 9), enabled, borderStyle, backColor };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnPaint_TestData))]
    public void UpDownBase_OnPaint_Invoke_CallsPaint(Size size, bool enabled, BorderStyle borderStyle, Color backColor)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using PaintEventArgs eventArgs = new(graphics, Rectangle.Empty);

        using SubUpDownBase control = new()
        {
            Size = size,
            Enabled = enabled,
            BorderStyle = borderStyle,
            BackColor = backColor
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
        control.OnPaint(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.Paint -= handler;
        control.OnPaint(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnPaint_TestData))]
    public void UpDownBase_OnPaint_InvokeWithHandle_CallsPaint(Size size, bool enabled, BorderStyle borderStyle, Color backColor)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using PaintEventArgs eventArgs = new(graphics, Rectangle.Empty);

        using SubUpDownBase control = new()
        {
            Size = size,
            Enabled = enabled,
            BorderStyle = borderStyle,
            BackColor = backColor
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        PaintEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.Paint += handler;
        control.OnPaint(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.Paint -= handler;
        control.OnPaint(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void UpDownBase_OnPaint_VisualStyles_on_NullE_ThrowsNullReferenceException()
    {
        if (!Application.RenderWithVisualStyles)
        {
            return;
        }

        using SubUpDownBase control = new();
        Assert.Throws<NullReferenceException>(() => control.OnPaint(null));
    }

    [WinFormsFact]
    public void UpDownBase_OnPaint_VisualStyles_off_NullE_ThrowsArgumentNullException()
    {
        if (Application.RenderWithVisualStyles)
        {
            return;
        }

        using SubUpDownBase control = new();
        Assert.Throws<ArgumentNullException>(() => control.OnPaint(null));
    }

    public static IEnumerable<object[]> OnTextBoxKeyDown_TestData()
    {
        foreach (object source in new object[] { null, new() })
        {
            foreach (bool userEdit in new bool[] { true, false })
            {
                yield return new object[] { true, userEdit, source, new KeyEventArgs(Keys.A), 0, 0, 0, false };
                yield return new object[] { false, userEdit, source, new KeyEventArgs(Keys.A), 0, 0, 0, false };
                yield return new object[] { true, userEdit, source, new KeyEventArgs(Keys.Up), 1, 0, 0, true };
                yield return new object[] { false, userEdit, source, new KeyEventArgs(Keys.Up), 0, 0, 0, false };
                yield return new object[] { true, userEdit, source, new KeyEventArgs(Keys.Down), 0, 1, 0, true };
                yield return new object[] { false, userEdit, source, new KeyEventArgs(Keys.Down), 0, 0, 0, false };
                yield return new object[] { true, userEdit, source, new KeyEventArgs(Keys.Left), 0, 0, 0, false };
                yield return new object[] { false, userEdit, source, new KeyEventArgs(Keys.Left), 0, 0, 0, false };
                yield return new object[] { true, userEdit, source, new KeyEventArgs(Keys.Right), 0, 0, 0, false };
                yield return new object[] { false, userEdit, source, new KeyEventArgs(Keys.Right), 0, 0, 0, false };
            }

            foreach (bool interceptArrowKeys in new bool[] { true, false })
            {
                yield return new object[] { interceptArrowKeys, true, source, new KeyEventArgs(Keys.Return), 0, 0, 1, false };
                yield return new object[] { interceptArrowKeys, false, source, new KeyEventArgs(Keys.Return), 0, 0, 0, false };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnTextBoxKeyDown_TestData))]
    public void UpDownBase_OnTextBoxKeyDown_Invoke_CallsKeyDown(bool interceptArrowKeys, bool userEdit, object source, KeyEventArgs eventArgs, int expectedUpButtonCallCount, int expectedDownButtonCallCount, int expectedValidateEditTextCallCount, bool expectedHandled)
    {
        int upButtonCallCount = 0;
        int downButtonCallCount = 0;
        int validateEditTextCallCount = 0;
        using CustomValidateUpDownBase control = new()
        {
            InterceptArrowKeys = interceptArrowKeys,
            UserEdit = userEdit,
            UpButtonAction = () => upButtonCallCount++,
            DownButtonAction = () => downButtonCallCount++,
            ValidateEditTextAction = () => validateEditTextCallCount++
        };
        int callCount = 0;
        KeyEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.KeyDown += handler;
        control.OnTextBoxKeyDown(source, eventArgs);
        Assert.Equal(expectedUpButtonCallCount, upButtonCallCount);
        Assert.Equal(expectedDownButtonCallCount, downButtonCallCount);
        Assert.Equal(expectedValidateEditTextCallCount, validateEditTextCallCount);
        Assert.Equal(expectedHandled, eventArgs.Handled);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.KeyDown -= handler;
        control.OnTextBoxKeyDown(source, eventArgs);
        Assert.Equal(expectedUpButtonCallCount * 2, upButtonCallCount);
        Assert.Equal(expectedDownButtonCallCount * 2, downButtonCallCount);
        Assert.Equal(expectedValidateEditTextCallCount * 2, validateEditTextCallCount);
        Assert.Equal(expectedHandled, eventArgs.Handled);
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void UpDownBase_OnTextBoxKeyDown_NullE_ThrowsNullReferenceException()
    {
        using SubUpDownBase control = new();
        Assert.Throws<NullReferenceException>(() => control.OnTextBoxKeyDown(new object(), null));
    }

    public static IEnumerable<object[]> OnTextBoxKeyPress_TestData()
    {
        yield return new object[] { null, null };
        yield return new object[] { null, null };
        yield return new object[] { null, new KeyPressEventArgs('a') };
        yield return new object[] { null, new KeyPressEventArgs('a') };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnTextBoxKeyPress_TestData))]
    public void UpPressBase_OnTextBoxKeyPress_Invoke_CallsKeyPress(object source, KeyPressEventArgs eventArgs)
    {
        using SubUpDownBase control = new();
        int callCount = 0;
        KeyPressEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.KeyPress += handler;
        control.OnTextBoxKeyPress(source, eventArgs);
        Assert.False(eventArgs?.Handled ?? false);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.KeyPress -= handler;
        control.OnTextBoxKeyPress(source, eventArgs);
        Assert.False(eventArgs?.Handled ?? false);
        Assert.Equal(1, callCount);
    }

    public static IEnumerable<object[]> OnTextBoxLostFocus_TestData()
    {
        yield return new object[] { true, null, null, 1 };
        yield return new object[] { true, null, null, 1 };
        yield return new object[] { true, null, new EventArgs(), 1 };
        yield return new object[] { true, null, new EventArgs(), 1 };
        yield return new object[] { false, null, null, 0 };
        yield return new object[] { false, null, null, 0 };
        yield return new object[] { false, null, new EventArgs(), 0 };
        yield return new object[] { false, null, new EventArgs(), 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnTextBoxLostFocus_TestData))]
    public void UpPressBase_OnTextBoxLostFocus_Invoke_CallsLostFocus(bool userEdit, object source, EventArgs eventArgs, int expectedValidateEditTextCallCount)
    {
        int validateEditTextCallCount = 0;
        using CustomValidateUpDownBase control = new()
        {
            UserEdit = userEdit,
            ValidateEditTextAction = () => validateEditTextCallCount++
        };
        int callCount = 0;
        EventHandler handler = (sender, e) => callCount++;

        // Call with handler.
        control.LostFocus += handler;
        control.OnTextBoxLostFocus(source, eventArgs);
        Assert.Equal(expectedValidateEditTextCallCount, validateEditTextCallCount);
        Assert.Equal(0, callCount);

        // Remove handler.
        control.LostFocus -= handler;
        control.OnTextBoxLostFocus(source, eventArgs);
        Assert.Equal(expectedValidateEditTextCallCount * 2, validateEditTextCallCount);
        Assert.Equal(0, callCount);
    }

    public static IEnumerable<object[]> OnTextBoxResize_TestData()
    {
        foreach (BorderStyle borderStyle in Enum.GetValues(typeof(BorderStyle)))
        {
            foreach (RightToLeft rightToLeft in Enum.GetValues(typeof(RightToLeft)))
            {
                foreach (LeftRightAlignment upDownAlign in Enum.GetValues(typeof(LeftRightAlignment)))
                {
                    yield return new object[] { borderStyle, rightToLeft, upDownAlign, null, null };
                    yield return new object[] { borderStyle, rightToLeft, upDownAlign, new(), null };
                    yield return new object[] { borderStyle, rightToLeft, upDownAlign, null, new EventArgs() };
                    yield return new object[] { borderStyle, rightToLeft, upDownAlign, new(), new EventArgs() };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnTextBoxResize_TestData))]
    public void UpDownBase_OnTextBoxResize_Invoke_CallsTextBoxResize(BorderStyle borderStyle, RightToLeft rightToLeft, LeftRightAlignment upDownAlign, object source, EventArgs eventArgs)
    {
        using SubUpDownBase control = new()
        {
            BorderStyle = borderStyle,
            RightToLeft = rightToLeft,
            UpDownAlign = upDownAlign
        };
        int preferredHeight = control.PreferredHeight;

        // Call with handler.
        control.OnTextBoxResize(source, eventArgs);
        Assert.Equal(preferredHeight, control.Height);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.OnTextBoxResize(source, eventArgs);
        Assert.Equal(preferredHeight, control.Height);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> OnTextBoxTextChanged_TestData()
    {
        foreach (bool userEdit in new bool[] { true, false })
        {
            yield return new object[] { true, userEdit, null, null, userEdit };
            yield return new object[] { true, userEdit, null, null, userEdit };
            yield return new object[] { true, userEdit, null, new EventArgs(), userEdit };
            yield return new object[] { true, userEdit, null, new EventArgs(), userEdit };
        }

        foreach (bool userEdit in new bool[] { true, false })
        {
            yield return new object[] { false, userEdit, null, null, true };
            yield return new object[] { false, userEdit, null, null, true };
            yield return new object[] { false, userEdit, null, new EventArgs(), true };
            yield return new object[] { false, userEdit, null, new EventArgs(), true };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnTextBoxTextChanged_TestData))]
    public void UpPressBase_OnTextBoxTextChanged_Invoke_CallsTextChanged(bool changingText, bool userEdit, object source, EventArgs eventArgs, bool expectedUserEdit)
    {
        using (new NoAssertContext())
        {
            using CustomValidateUpDownBase control = new()
            {
                ChangingText = changingText,
                UserEdit = userEdit
            };
            int onChangedCallCount = 0;
            control.OnChangedAction = (sender, e) =>
            {
                Assert.Same(source, sender);
                Assert.Same(EventArgs.Empty, e);
                onChangedCallCount++;
            };
            int callCount = 0;
            EventHandler handler = (sender, e) => callCount++;

            // Call with handler.
            control.TextChanged += handler;
            control.OnTextBoxTextChanged(source, eventArgs);
            Assert.False(control.ChangingText);
            Assert.Equal(expectedUserEdit, control.UserEdit);
            Assert.Equal(1, callCount);
            Assert.Equal(1, onChangedCallCount);

            // Remove handler.
            control.TextChanged -= handler;
            control.OnTextBoxTextChanged(source, eventArgs);
            Assert.False(control.ChangingText);
            Assert.True(control.UserEdit);
            Assert.Equal(1, callCount);
            Assert.Equal(2, onChangedCallCount);
        }
    }

    [WinFormsTheory]
    [InlineData(1, 2)]
    [InlineData(0, 0)]
    [InlineData(-1, -2)]
    public void UpDownBase_RescaleConstantsForDpi_Invoke_Nop(int deviceDpiOld, int deviceDpiNew)
    {
        using SubUpDownBase control = new();
        control.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData("", 0, 0)]
    [InlineData("", 0, 1)]
    [InlineData("tex", 0, 4)]
    [InlineData("text", 1, 2)]
    public void UpDownBase_Select_Invoke_Success(string text, int start, int length)
    {
        using SubUpDownBase control = new()
        {
            Text = text
        };
        control.Select(start, length);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.Select(start, length);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData("", 0, 0)]
    [InlineData("", 0, 1)]
    [InlineData("tex", 0, 4)]
    [InlineData("text", 1, 2)]
    public void UpDownBase_Select_InvokeWithHandle_Success(string text, int start, int length)
    {
        using SubUpDownBase control = new()
        {
            Text = text
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Select(start, length);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.Select(start, length);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> SetBoundsCore_TestData()
    {
        foreach (BoundsSpecified specified in Enum.GetValues(typeof(BoundsSpecified)))
        {
            yield return new object[] { 0, 0, 120, 0, specified, 0, 0 };
            yield return new object[] { -1, -2, -3, -4, specified, 1, 1 };
            yield return new object[] { 1, 0, 120, 0, specified, 1, 0 };
            yield return new object[] { 0, 2, 120, 0, specified, 1, 0 };
            yield return new object[] { 1, 2, 120, 0, specified, 1, 0 };
            yield return new object[] { 0, 0, 1, 0, specified, 0, 1 };
            yield return new object[] { 0, 0, 120, 2, specified, 0, 0 };
            yield return new object[] { 0, 0, 1, 2, specified, 0, 1 };
            yield return new object[] { 1, 2, 30, 40, specified, 1, 1 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(SetBoundsCore_TestData))]
    public void UpDownBase_SetBoundsCore_Invoke_Success(int x, int y, int width, int height, BoundsSpecified specified, int expectedLocationChangedCallCount, int expectedLayoutCallCount)
    {
        using SubUpDownBase control = new();
        int preferredHeight = control.PreferredHeight;
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
            Assert.Equal(layoutCallCount, moveCallCount);
            Assert.Equal(resizeCallCount, moveCallCount);
            Assert.Equal(sizeChangedCallCount, moveCallCount);
            Assert.Equal(clientSizeChangedCallCount, moveCallCount);
            moveCallCount++;
        };
        control.LocationChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(moveCallCount - 1, locationChangedCallCount);
            Assert.Equal(layoutCallCount, locationChangedCallCount);
            Assert.Equal(resizeCallCount, locationChangedCallCount);
            Assert.Equal(sizeChangedCallCount, locationChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, locationChangedCallCount);
            locationChangedCallCount++;
        };
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };

        control.SetBoundsCore(x, y, width, height, specified);

        if (Application.UseVisualStyles)
        {
            Assert.Equal(new Size(width, Control.DefaultFont.Height + 7), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, width, Control.DefaultFont.Height + 7), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, width, Control.DefaultFont.Height + 7), control.DisplayRectangle);
        }
        else
        {
            Assert.Equal(new Size(width - 4, Control.DefaultFont.Height + 3), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, width - 4, Control.DefaultFont.Height + 3), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, width - 4, Control.DefaultFont.Height + 3), control.DisplayRectangle);
        }

        Assert.Equal(new Size(width, preferredHeight), control.Size);
        Assert.Equal(x, control.Left);
        Assert.Equal(x + width, control.Right);
        Assert.Equal(y, control.Top);
        Assert.Equal(y + preferredHeight, control.Bottom);
        Assert.Equal(width, control.Width);
        Assert.Equal(preferredHeight, control.Height);
        Assert.Equal(new Rectangle(x, y, width, preferredHeight), control.Bounds);
        Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.SetBoundsCore(x, y, width, height, specified);

        if (Application.UseVisualStyles)
        {
            Assert.Equal(new Size(width, Control.DefaultFont.Height + 7), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, width, Control.DefaultFont.Height + 7), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, width, Control.DefaultFont.Height + 7), control.DisplayRectangle);
        }
        else
        {
            Assert.Equal(new Size(width - 4, Control.DefaultFont.Height + 3), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, width - 4, Control.DefaultFont.Height + 3), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, width - 4, Control.DefaultFont.Height + 3), control.DisplayRectangle);
        }

        Assert.Equal(new Size(width, preferredHeight), control.Size);
        Assert.Equal(x, control.Left);
        Assert.Equal(x + width, control.Right);
        Assert.Equal(y, control.Top);
        Assert.Equal(y + preferredHeight, control.Bottom);
        Assert.Equal(width, control.Width);
        Assert.Equal(preferredHeight, control.Height);
        Assert.Equal(new Rectangle(x, y, width, preferredHeight), control.Bounds);
        Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void UpDownBase_ValidateEditText_Invoke_Nop()
    {
        using SubUpDownBase control = new();
        control.ValidateEditText();
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.ValidateEditText();
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void UpDownBase_ValidateEditText_InvokeWithHandle_Nop()
    {
        using SubUpDownBase control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ValidateEditText();
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.ValidateEditText();
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void UpDownBase_WndProc_InvokeKillFocusWithoutHandle_Success()
    {
        using (new NoAssertContext())
        {
            using SubUpDownBase control = new();
            int callCount = 0;
            control.LostFocus += (sender, e) => callCount++;
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_KILLFOCUS,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);
        }
    }

    [WinFormsFact]
    public void UpDownBase_WndProc_InvokeKillFocusWithHandle_Success()
    {
        using SubUpDownBase control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        control.LostFocus += (sender, e) => callCount++;
        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_KILLFOCUS,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(0, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void UpDownBase_WndProc_InvokeMouseHoverWithoutHandle_Success()
    {
        using (new NoAssertContext())
        {
            using SubUpDownBase control = new();
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
            Assert.False(control.IsHandleCreated);
        }
    }

    [WinFormsFact]
    public void UpDownBase_WndProc_InvokeMouseHoverWithHandle_Success()
    {
        using SubUpDownBase control = new();
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
    public void UpDownBase_WndProc_InvokeSetFocusWithoutHandle_Success()
    {
        using SubUpDownBase control = new();
        int callCount = 0;
        control.GotFocus += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_SETFOCUS,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(250, m.Result);
        Assert.Equal(0, callCount);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void UpDownBase_WndProc_InvokeSetFocusWithHandle_Success()
    {
        using SubUpDownBase control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        control.GotFocus += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_SETFOCUS,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(250, m.Result);
        Assert.Equal(0, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void UpDownBase_Invokes_SetToolTip_IfExternalToolTipIsSet()
    {
        using UpDownBase upDownBase = new SubUpDownBase();
        using ToolTip toolTip = new();
        upDownBase.CreateControl();

        string actualEditToolTipText = toolTip.GetToolTip(upDownBase._upDownEdit);
        string actualButtonsToolTipText = toolTip.GetToolTip(upDownBase._upDownButtons);

        actualEditToolTipText.Should().BeEmpty();
        actualButtonsToolTipText.Should().BeEmpty();
        toolTip.Handle.Should().NotBe(IntPtr.Zero); // A workaround to create the toolTip native window Handle

        string text = "Some test text";
        toolTip.SetToolTip(upDownBase, text); // Invokes UpDownBase's SetToolTip inside
        actualEditToolTipText = toolTip.GetToolTip(upDownBase._upDownEdit);
        actualButtonsToolTipText = toolTip.GetToolTip(upDownBase._upDownButtons);

        actualEditToolTipText.Should().Be(text);
        actualButtonsToolTipText.Should().Be(text);

        toolTip.SetToolTip(upDownBase, null); // Invokes UpDownBase's SetToolTip inside
        actualEditToolTipText = toolTip.GetToolTip(upDownBase._upDownEdit);
        actualButtonsToolTipText = toolTip.GetToolTip(upDownBase._upDownButtons);

        actualEditToolTipText.Should().BeEmpty();
        actualButtonsToolTipText.Should().BeEmpty();
    }

    private class CustomValidateUpDownBase : UpDownBase
    {
        public new bool ChangingText
        {
            get => base.ChangingText;
            set => base.ChangingText = value;
        }

        public Action DownButtonAction { get; set; }

        public override void DownButton() => DownButtonAction();

        public Action<object, EventArgs> OnChangedAction { get; set; }

        protected override void OnChanged(object source, EventArgs e) => OnChangedAction(source, e);

        public new void OnTextBoxKeyDown(object source, KeyEventArgs e) => base.OnTextBoxKeyDown(source, e);

        public new void OnTextBoxLostFocus(object source, EventArgs e) => base.OnTextBoxLostFocus(source, e);

        public new void OnTextBoxTextChanged(object source, EventArgs e) => base.OnTextBoxTextChanged(source, e);

        public Action UpButtonAction { get; set; }

        public override void UpButton() => UpButtonAction();

        public Action UpdateEditTextAction { get; set; }

        protected override void UpdateEditText() => UpdateEditTextAction();

        public new bool UserEdit
        {
            get => base.UserEdit;
            set => base.UserEdit = value;
        }

        public Action ValidateEditTextAction { get; set; }

        protected override void ValidateEditText() => ValidateEditTextAction();
    }

    public class SubUpDownBase : UpDownBase
    {
        public new const int ScrollStateAutoScrolling = ScrollableControl.ScrollStateAutoScrolling;

        public new const int ScrollStateHScrollVisible = ScrollableControl.ScrollStateHScrollVisible;

        public new const int ScrollStateVScrollVisible = ScrollableControl.ScrollStateVScrollVisible;

        public new const int ScrollStateUserHasScrolled = ScrollableControl.ScrollStateUserHasScrolled;

        public new const int ScrollStateFullDrag = ScrollableControl.ScrollStateFullDrag;

        public new SizeF AutoScaleFactor => base.AutoScaleFactor;

        public new bool CanEnableIme => base.CanEnableIme;

        public new bool CanRaiseEvents => base.CanRaiseEvents;

        public new bool ChangingText
        {
            get => base.ChangingText;
            set => base.ChangingText = value;
        }

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

        public new bool HScroll
        {
            get => base.HScroll;
            set => base.HScroll = value;
        }

        public new ImeMode ImeModeBase
        {
            get => base.ImeModeBase;
            set => base.ImeModeBase = value;
        }

        public new bool ResizeRedraw
        {
            get => base.ResizeRedraw;
            set => base.ResizeRedraw = value;
        }

        public new bool ShowFocusCues => base.ShowFocusCues;

        public new bool ShowKeyboardCues => base.ShowKeyboardCues;

        public new bool UserEdit
        {
            get => base.UserEdit;
            set => base.UserEdit = value;
        }

        public new bool VScroll
        {
            get => base.VScroll;
            set => base.VScroll = value;
        }

        public override void DownButton()
        {
            throw new NotImplementedException();
        }

        public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

        public new bool GetScrollState(int bit) => base.GetScrollState(bit);

        public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

        public new bool GetTopLevel() => base.GetTopLevel();

        public new void OnChanged(object source, EventArgs e) => base.OnChanged(source, e);

        public new void OnFontChanged(EventArgs e) => base.OnFontChanged(e);

        public new void OnHandleCreated(EventArgs e) => base.OnHandleCreated(e);

        public new void OnHandleDestroyed(EventArgs e) => base.OnHandleDestroyed(e);

        public new void OnLayout(LayoutEventArgs e) => base.OnLayout(e);

        public new void OnMouseDown(MouseEventArgs e) => base.OnMouseDown(e);

        public new void OnMouseEnter(EventArgs e) => base.OnMouseEnter(e);

        public new void OnMouseHover(EventArgs e) => base.OnMouseHover(e);

        public new void OnMouseLeave(EventArgs e) => base.OnMouseLeave(e);

        public new void OnMouseMove(MouseEventArgs e) => base.OnMouseMove(e);

        public new void OnMouseUp(MouseEventArgs mevent) => base.OnMouseUp(mevent);

        public new void OnMouseWheel(MouseEventArgs e) => base.OnMouseWheel(e);

        public new void OnPaint(PaintEventArgs e) => base.OnPaint(e);

        public new void OnTextBoxKeyDown(object source, KeyEventArgs e) => base.OnTextBoxKeyDown(source, e);

        public new void OnTextBoxKeyPress(object source, KeyPressEventArgs e) => base.OnTextBoxKeyPress(source, e);

        public new void OnTextBoxLostFocus(object source, EventArgs e) => base.OnTextBoxLostFocus(source, e);

        public new void OnTextBoxResize(object source, EventArgs e) => base.OnTextBoxResize(source, e);

        public new void OnTextBoxTextChanged(object source, EventArgs e) => base.OnTextBoxTextChanged(source, e);

        public new void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew) => base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);

        public new void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) => base.SetBoundsCore(x, y, width, height, specified);

        public override void UpButton()
        {
            throw new NotImplementedException();
        }

        protected override void UpdateEditText()
        {
            throw new NotImplementedException();
        }

        public new void ValidateEditText() => base.ValidateEditText();

        public new void WndProc(ref Message m) => base.WndProc(ref m);
    }
}
