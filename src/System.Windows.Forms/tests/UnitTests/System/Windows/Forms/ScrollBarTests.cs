// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.TestUtilities;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Tests;

public class ScrollBarTests
{
    [WinFormsFact]
    public void ScrollBar_Ctor_Default()
    {
        using SubScrollBar control = new();
        Assert.Null(control.AccessibleDefaultActionDescription);
        Assert.Null(control.AccessibleDescription);
        Assert.Null(control.AccessibleName);
        Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
        Assert.False(control.AllowDrop);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        Assert.False(control.AutoSize);
        Assert.Equal(Control.DefaultBackColor, control.BackColor);
        Assert.Null(control.BackgroundImage);
        Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
        Assert.Null(control.BindingContext);
        Assert.Equal(0, control.Bottom);
        Assert.Equal(Rectangle.Empty, control.Bounds);
        Assert.False(control.CanEnableIme);
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
        Assert.Equal(ImeMode.Disable, control.DefaultImeMode);
        Assert.Equal(Padding.Empty, control.DefaultMargin);
        Assert.Equal(Size.Empty, control.DefaultMaximumSize);
        Assert.Equal(Size.Empty, control.DefaultMinimumSize);
        Assert.Equal(Padding.Empty, control.DefaultPadding);
        Assert.Equal(Size.Empty, control.DefaultSize);
        Assert.False(control.DesignMode);
        Assert.Equal(Rectangle.Empty, control.DisplayRectangle);
        Assert.Equal(DockStyle.None, control.Dock);
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
        Assert.Equal(ImeMode.Disable, control.ImeMode);
        Assert.Equal(ImeMode.Disable, control.ImeModeBase);
        Assert.False(control.IsAccessible);
        Assert.False(control.IsMirrored);
        Assert.Equal(10, control.LargeChange);
        Assert.NotNull(control.LayoutEngine);
        Assert.Same(control.LayoutEngine, control.LayoutEngine);
        Assert.Equal(0, control.Left);
        Assert.Equal(Point.Empty, control.Location);
        Assert.Equal(Padding.Empty, control.Margin);
        Assert.Equal(100, control.Maximum);
        Assert.Equal(Size.Empty, control.MaximumSize);
        Assert.Equal(0, control.Minimum);
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
        Assert.True(control.ScaleScrollBarForDpiChange);
        Assert.True(control.ShowFocusCues);
        Assert.True(control.ShowKeyboardCues);
        Assert.Null(control.Site);
        Assert.Equal(Size.Empty, control.Size);
        Assert.Equal(1, control.SmallChange);
        Assert.Equal(0, control.TabIndex);
        Assert.False(control.TabStop);
        Assert.Empty(control.Text);
        Assert.Equal(0, control.Top);
        Assert.Null(control.TopLevelControl);
        Assert.False(control.UseWaitCursor);
        Assert.Equal(0, control.Value);
        Assert.True(control.Visible);
        Assert.Equal(0, control.Width);

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ScrollBar_CreateParams_GetDefault_ReturnsExpected()
    {
        using SubScrollBar control = new();
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Equal("ScrollBar", createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0, createParams.ExStyle);
        Assert.Equal(0, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(0x56000000, createParams.Style);
        Assert.Equal(0, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ScrollBar_AutoSize_Set_GetReturnsExpected(bool value)
    {
        using SubScrollBar control = new();
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
    public void ScrollBar_AutoSize_SetWithHandler_CallsAutoSizeChanged()
    {
        using SubScrollBar control = new()
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
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetBackColorTheoryData))]
    public void ScrollBar_BackColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using SubScrollBar control = new()
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

    [WinFormsFact]
    public void ScrollBar_BackColor_SetWithHandler_CallsBackColorChanged()
    {
        using SubScrollBar control = new();
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
        Assert.Equal(1, callCount);

        // Set same.
        control.BackColor = Color.Red;
        Assert.Equal(Color.Red, control.BackColor);
        Assert.Equal(1, callCount);

        // Set different.
        control.BackColor = Color.Empty;
        Assert.Equal(Control.DefaultBackColor, control.BackColor);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.BackColorChanged -= handler;
        control.BackColor = Color.Red;
        Assert.Equal(Color.Red, control.BackColor);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetImageTheoryData))]
    public void ScrollBar_BackgroundImage_Set_GetReturnsExpected(Image value)
    {
        using SubScrollBar control = new()
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
    public void ScrollBar_BackgroundImage_SetWithHandler_CallsBackgroundImageChanged()
    {
        using SubScrollBar control = new();
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
    public void ScrollBar_BackgroundImageLayout_Set_GetReturnsExpected(ImageLayout value)
    {
        using SubScrollBar control = new()
        {
            BackgroundImageLayout = value
        };
        Assert.Equal(value, control.BackgroundImageLayout);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BackgroundImageLayout = value;
        Assert.Equal(value, control.BackgroundImageLayout);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ScrollBar_BackgroundImageLayout_SetWithHandler_CallsBackgroundImageLayoutChanged()
    {
        using SubScrollBar control = new();
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

    [WinFormsTheory]
    [InvalidEnumData<ImageLayout>]
    public void ScrollBar_BackgroundImageLayout_SetInvalid_ThrowsInvalidEnumArgumentException(ImageLayout value)
    {
        using SubScrollBar control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.BackgroundImageLayout = value);
    }

    [WinFormsTheory]
    [BoolData]
    public void ScrollBar_Enabled_Set_GetReturnsExpected(bool value)
    {
        using SubScrollBar control = new()
        {
            Enabled = value
        };
        Assert.Equal(value, control.Enabled);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Enabled = value;
        Assert.Equal(value, control.Enabled);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.Enabled = value;
        Assert.Equal(value, control.Enabled);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ScrollBar_Enabled_SetWithHandle_GetReturnsExpected(bool value)
    {
        using SubScrollBar control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Enabled = value;
        Assert.Equal(value, control.Enabled);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Enabled = value;
        Assert.Equal(value, control.Enabled);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.Enabled = value;
        Assert.Equal(value, control.Enabled);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public unsafe void ScrollBar_Enabled_GetScrollInfo_Updates()
    {
        using SubScrollBar control = new()
        {
            LargeChange = 15
        };

        // Enable.
        SCROLLINFO si = new()
        {
            cbSize = (uint)sizeof(SCROLLINFO),
            fMask = SCROLLINFO_MASK.SIF_ALL
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.Enabled = true;
        Assert.True(PInvoke.GetScrollInfo(control.HWND, SCROLLBAR_CONSTANTS.SB_CTL, ref si));
        Assert.Equal(15u, si.nPage);
    }

    [WinFormsFact]
    public void ScrollBar_Enabled_SetWithHandler_CallsEnabledChanged()
    {
        using SubScrollBar control = new()
        {
            Enabled = true
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.EnabledChanged += handler;

        // Set different.
        control.Enabled = false;
        Assert.False(control.Enabled);
        Assert.Equal(1, callCount);

        // Set same.
        control.Enabled = false;
        Assert.False(control.Enabled);
        Assert.Equal(1, callCount);

        // Set different.
        control.Enabled = true;
        Assert.True(control.Enabled);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.EnabledChanged -= handler;
        control.Enabled = false;
        Assert.False(control.Enabled);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetFontTheoryData))]
    public void ScrollBar_Font_Set_GetReturnsExpected(Font value)
    {
        using SubScrollBar control = new()
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
    public void ScrollBar_Font_SetWithHandler_CallsFontChanged()
    {
        using SubScrollBar control = new();
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

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetForeColorTheoryData))]
    public void ScrollBar_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using SubScrollBar control = new()
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
    public void ScrollBar_ForeColor_SetWithHandler_CallsForeColorChanged()
    {
        using SubScrollBar control = new();
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
    [InlineData(RightToLeft.Inherit)]
    [InlineData(RightToLeft.No)]
    [InlineData(RightToLeft.Yes)]
    public unsafe void ScrollBar_Handle_GetDefault_ReturnsExpected(RightToLeft rightToLeft)
    {
        using SubScrollBar control = new()
        {
            RightToLeft = rightToLeft,
            Minimum = 5,
            Value = 25,
            Maximum = 105,
            LargeChange = 15,
            SmallChange = 10,
        };

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        SCROLLINFO si = new()
        {
            cbSize = (uint)sizeof(SCROLLINFO),
            fMask = SCROLLINFO_MASK.SIF_ALL
        };
        Assert.True(PInvoke.GetScrollInfo((HWND)control.Handle, SCROLLBAR_CONSTANTS.SB_CTL, ref si));
        Assert.Equal(5, si.nMin);
        Assert.Equal(105, si.nMax);
        Assert.Equal(25, si.nPos);
        Assert.Equal(15u, si.nPage);
    }

    [WinFormsTheory]
    [EnumData<ImageLayout>]
    public void ScrollBar_ImeMode_Set_GetReturnsExpected(ImeMode value)
    {
        using SubScrollBar control = new()
        {
            ImeMode = value
        };
        Assert.Equal(value, control.ImeMode);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ImeMode = value;
        Assert.Equal(value, control.ImeMode);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ScrollBar_ImeMode_SetWithHandler_CallsImeModeChanged()
    {
        using SubScrollBar control = new();
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
        Assert.Equal(0, callCount);

        // Set same.
        control.ImeMode = ImeMode.On;
        Assert.Equal(ImeMode.On, control.ImeMode);
        Assert.Equal(0, callCount);

        // Set different.
        control.ImeMode = ImeMode.Off;
        Assert.Equal(ImeMode.Off, control.ImeMode);
        Assert.Equal(0, callCount);

        // Remove handler.
        control.ImeModeChanged -= handler;
        control.ImeMode = ImeMode.Off;
        Assert.Equal(ImeMode.Off, control.ImeMode);
        Assert.Equal(0, callCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<ImeMode>]
    public void ScrollBar_ImeMode_SetInvalid_ThrowsInvalidEnumArgumentException(ImeMode value)
    {
        using SubScrollBar control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.ImeMode = value);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(11)]
    public void ScrollBar_LargeChange_Set_GetReturnsExpected(int value)
    {
        using SubScrollBar control = new()
        {
            LargeChange = value
        };
        Assert.Equal(value, control.LargeChange);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.LargeChange = value;
        Assert.Equal(value, control.LargeChange);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ScrollBar_LargeChange_SetLarge_GetReturnsExpected()
    {
        using SubScrollBar control = new()
        {
            Minimum = 5,
            Maximum = 10,
            LargeChange = 7
        };
        Assert.Equal(6, control.LargeChange);
        Assert.False(control.IsHandleCreated);

        // Change maximum.
        control.Maximum = 15;
        Assert.Equal(7, control.LargeChange);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(11)]
    public unsafe void ScrollBar_LargeChange_SetWithHandle_GetReturnsExpected(int value)
    {
        using SubScrollBar control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.LargeChange = value;
        Assert.Equal(value, control.LargeChange);
        SCROLLINFO si = new()
        {
            cbSize = (uint)sizeof(SCROLLINFO),
            fMask = SCROLLINFO_MASK.SIF_ALL
        };
        Assert.True(PInvoke.GetScrollInfo((HWND)control.Handle, SCROLLBAR_CONSTANTS.SB_CTL, ref si));
        Assert.Equal((uint)value, si.nPage);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.LargeChange = value;
        Assert.Equal(value, control.LargeChange);
        si = new SCROLLINFO
        {
            cbSize = (uint)sizeof(SCROLLINFO),
            fMask = SCROLLINFO_MASK.SIF_ALL
        };
        Assert.True(PInvoke.GetScrollInfo((HWND)control.Handle, SCROLLBAR_CONSTANTS.SB_CTL, ref si));
        Assert.Equal((uint)value, si.nPage);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(11)]
    public unsafe void ScrollBar_LargeChange_SetWithHandleDisabled_GetReturnsExpected(int value)
    {
        using SubScrollBar control = new()
        {
            Enabled = false
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.LargeChange = value;
        Assert.Equal(value, control.LargeChange);
        SCROLLINFO si = new()
        {
            cbSize = (uint)sizeof(SCROLLINFO),
            fMask = SCROLLINFO_MASK.SIF_ALL
        };
        Assert.True(PInvoke.GetScrollInfo((HWND)control.Handle, SCROLLBAR_CONSTANTS.SB_CTL, ref si));
        Assert.Equal(0u, si.nPage);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.LargeChange = value;
        Assert.Equal(value, control.LargeChange);
        si = new SCROLLINFO
        {
            cbSize = (uint)sizeof(SCROLLINFO),
            fMask = SCROLLINFO_MASK.SIF_ALL
        };
        Assert.True(PInvoke.GetScrollInfo((HWND)control.Handle, SCROLLBAR_CONSTANTS.SB_CTL, ref si));
        Assert.Equal(0u, si.nPage);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ScrollBar_LargeChange_SetNegative_ThrowsArgumentOutOfRangeException()
    {
        using SubScrollBar control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.LargeChange = -1);
        Assert.Equal(10, control.LargeChange);
    }

    [WinFormsTheory]
    [InlineData(0, 1)]
    [InlineData(8, 9)]
    [InlineData(10, 10)]
    [InlineData(11, 10)]
    public void ScrollBar_Maximum_Set_GetReturnsExpected(int value, int expectedLargeChange)
    {
        using SubScrollBar control = new()
        {
            Maximum = value
        };
        Assert.Equal(value, control.Maximum);
        Assert.Equal(0, control.Minimum);
        Assert.Equal(0, control.Value);
        Assert.Equal(expectedLargeChange, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Maximum = value;
        Assert.Equal(value, control.Maximum);
        Assert.Equal(0, control.Minimum);
        Assert.Equal(0, control.Value);
        Assert.Equal(expectedLargeChange, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0, 1)]
    [InlineData(8, 9)]
    [InlineData(10, 10)]
    [InlineData(11, 10)]
    public unsafe void ScrollBar_Maximum_SetWithHandle_GetReturnsExpected(int value, int expectedLargeChange)
    {
        using SubScrollBar control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Maximum = value;
        Assert.Equal(value, control.Maximum);
        Assert.Equal(0, control.Minimum);
        Assert.Equal(0, control.Value);
        Assert.Equal(expectedLargeChange, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        SCROLLINFO si = new()
        {
            cbSize = (uint)sizeof(SCROLLINFO),
            fMask = SCROLLINFO_MASK.SIF_ALL
        };
        Assert.True(PInvoke.GetScrollInfo((HWND)control.Handle, SCROLLBAR_CONSTANTS.SB_CTL, ref si));
        Assert.Equal(value, si.nMax);
        Assert.Equal(0, si.nMin);
        Assert.Equal(0, si.nPos);
        Assert.Equal((uint)expectedLargeChange, si.nPage);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Maximum = value;
        Assert.Equal(value, control.Maximum);
        Assert.Equal(0, control.Minimum);
        Assert.Equal(0, control.Value);
        Assert.Equal(expectedLargeChange, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        si = new SCROLLINFO
        {
            cbSize = (uint)sizeof(SCROLLINFO),
            fMask = SCROLLINFO_MASK.SIF_ALL
        };
        Assert.True(PInvoke.GetScrollInfo((HWND)control.Handle, SCROLLBAR_CONSTANTS.SB_CTL, ref si));
        Assert.Equal(value, si.nMax);
        Assert.Equal(0, si.nMin);
        Assert.Equal(0, si.nPos);
        Assert.Equal((uint)expectedLargeChange, si.nPage);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(0, 1)]
    [InlineData(8, 9)]
    [InlineData(10, 10)]
    [InlineData(11, 10)]
    public unsafe void ScrollBar_Maximum_SetWithHandleDisabled_GetReturnsExpected(int value, int expectedLargeChange)
    {
        using SubScrollBar control = new()
        {
            Enabled = false
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Maximum = value;
        Assert.Equal(value, control.Maximum);
        Assert.Equal(0, control.Minimum);
        Assert.Equal(0, control.Value);
        Assert.Equal(expectedLargeChange, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        SCROLLINFO si = new()
        {
            cbSize = (uint)sizeof(SCROLLINFO),
            fMask = SCROLLINFO_MASK.SIF_ALL
        };
        Assert.True(PInvoke.GetScrollInfo((HWND)control.Handle, SCROLLBAR_CONSTANTS.SB_CTL, ref si));
        Assert.Equal(0, si.nMax);
        Assert.Equal(0, si.nMin);
        Assert.Equal(0, si.nPos);
        Assert.Equal(0u, si.nPage);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Maximum = value;
        Assert.Equal(value, control.Maximum);
        Assert.Equal(0, control.Minimum);
        Assert.Equal(0, control.Value);
        Assert.Equal(expectedLargeChange, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        Assert.Equal(0, createdCallCount);
        si = new SCROLLINFO
        {
            cbSize = (uint)sizeof(SCROLLINFO),
            fMask = SCROLLINFO_MASK.SIF_ALL
        };
        Assert.True(PInvoke.GetScrollInfo((HWND)control.Handle, SCROLLBAR_CONSTANTS.SB_CTL, ref si));
        Assert.Equal(0, si.nMax);
        Assert.Equal(0, si.nMin);
        Assert.Equal(0, si.nPos);
        Assert.Equal(0u, si.nPage);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ScrollBar_Maximum_SetLessThanValueAndMinimum_SetsValueAndMinimum()
    {
        using SubScrollBar control = new()
        {
            Value = 10,
            Minimum = 8,
            Maximum = 5
        };
        Assert.Equal(5, control.Maximum);
        Assert.Equal(5, control.Minimum);
        Assert.Equal(5, control.Value);
        Assert.Equal(1, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ScrollBar_Maximum_SetNegative_SetsValueAndMinimum()
    {
        using SubScrollBar control = new()
        {
            Maximum = -1
        };
        Assert.Equal(-1, control.Maximum);
        Assert.Equal(-1, control.Minimum);
        Assert.Equal(-1, control.Value);
        Assert.Equal(1, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(5)]
    public void ScrollBar_Minimum_Set_GetReturnsExpected(int value)
    {
        using SubScrollBar control = new()
        {
            Value = 5,
            Minimum = value
        };
        Assert.Equal(100, control.Maximum);
        Assert.Equal(value, control.Minimum);
        Assert.Equal(5, control.Value);
        Assert.Equal(10, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Minimum = value;
        Assert.Equal(100, control.Maximum);
        Assert.Equal(value, control.Minimum);
        Assert.Equal(5, control.Value);
        Assert.Equal(10, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(5)]
    public unsafe void ScrollBar_Minimum_SetWithHandle_GetReturnsExpected(int value)
    {
        using SubScrollBar control = new()
        {
            Value = 5
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Minimum = value;
        Assert.Equal(100, control.Maximum);
        Assert.Equal(value, control.Minimum);
        Assert.Equal(5, control.Value);
        Assert.Equal(10, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        SCROLLINFO si = new()
        {
            cbSize = (uint)sizeof(SCROLLINFO),
            fMask = SCROLLINFO_MASK.SIF_ALL
        };
        Assert.True(PInvoke.GetScrollInfo((HWND)control.Handle, SCROLLBAR_CONSTANTS.SB_CTL, ref si));
        Assert.Equal(100, si.nMax);
        Assert.Equal(value, si.nMin);
        Assert.Equal(5, si.nPos);
        Assert.Equal(10u, si.nPage);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Minimum = value;
        Assert.Equal(100, control.Maximum);
        Assert.Equal(value, control.Minimum);
        Assert.Equal(5, control.Value);
        Assert.Equal(10, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        si = new SCROLLINFO
        {
            cbSize = (uint)sizeof(SCROLLINFO),
            fMask = SCROLLINFO_MASK.SIF_ALL
        };
        Assert.True(PInvoke.GetScrollInfo((HWND)control.Handle, SCROLLBAR_CONSTANTS.SB_CTL, ref si));
        Assert.Equal(100, si.nMax);
        Assert.Equal(value, si.nMin);
        Assert.Equal(5, si.nPos);
        Assert.Equal(10u, si.nPage);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(5)]
    public unsafe void ScrollBar_Minimum_SetWithHandleDisabled_GetReturnsExpected(int value)
    {
        using SubScrollBar control = new()
        {
            Value = 5,
            Enabled = false
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Minimum = value;
        Assert.Equal(100, control.Maximum);
        Assert.Equal(value, control.Minimum);
        Assert.Equal(5, control.Value);
        Assert.Equal(10, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        SCROLLINFO si = new()
        {
            cbSize = (uint)sizeof(SCROLLINFO),
            fMask = SCROLLINFO_MASK.SIF_ALL
        };
        Assert.True(PInvoke.GetScrollInfo((HWND)control.Handle, SCROLLBAR_CONSTANTS.SB_CTL, ref si));
        Assert.Equal(0, si.nMax);
        Assert.Equal(0, si.nMin);
        Assert.Equal(0, si.nPos);
        Assert.Equal(0u, si.nPage);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Minimum = value;
        Assert.Equal(100, control.Maximum);
        Assert.Equal(value, control.Minimum);
        Assert.Equal(5, control.Value);
        Assert.Equal(10, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        si = new SCROLLINFO
        {
            cbSize = (uint)sizeof(SCROLLINFO),
            fMask = SCROLLINFO_MASK.SIF_ALL
        };
        Assert.True(PInvoke.GetScrollInfo((HWND)control.Handle, SCROLLBAR_CONSTANTS.SB_CTL, ref si));
        Assert.Equal(0, si.nMax);
        Assert.Equal(0, si.nMin);
        Assert.Equal(0, si.nPos);
        Assert.Equal(0u, si.nPage);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ScrollBar_Minimum_SetGreaterThanValueAndMaximum_SetsValueAndMinimum()
    {
        using SubScrollBar control = new()
        {
            Value = 10,
            Maximum = 8,
            Minimum = 12
        };
        Assert.Equal(12, control.Maximum);
        Assert.Equal(12, control.Minimum);
        Assert.Equal(12, control.Value);
        Assert.Equal(1, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetRightToLeftTheoryData))]
    public void ScrollBar_RightToLeft_Set_GetReturnsExpected(RightToLeft value, RightToLeft expected)
    {
        using SubScrollBar control = new()
        {
            RightToLeft = value
        };
        Assert.Equal(expected, control.RightToLeft);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.RightToLeft = value;
        Assert.Equal(expected, control.RightToLeft);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ScrollBar_RightToLeft_SetWithHandler_CallsRightToLeftChanged()
    {
        using SubScrollBar control = new();
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
    public void ScrollBar_RightToLeft_SetInvalid_ThrowsInvalidEnumArgumentException(RightToLeft value)
    {
        using SubScrollBar control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.RightToLeft = value);
    }

    [WinFormsTheory]
    [BoolData]
    public void ScrollBar_ScaleScrollBarForDpiChange_Set_GetReturnsExpected(bool value)
    {
        using SubScrollBar control = new()
        {
            ScaleScrollBarForDpiChange = value
        };
        Assert.Equal(value, control.ScaleScrollBarForDpiChange);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ScaleScrollBarForDpiChange = value;
        Assert.Equal(value, control.ScaleScrollBarForDpiChange);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.ScaleScrollBarForDpiChange = !value;
        Assert.Equal(!value, control.ScaleScrollBarForDpiChange);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(5, 5)]
    [InlineData(10, 10)]
    [InlineData(11, 10)]
    public void ScrollBar_SmallChange_Set_GetReturnsExpected(int value, int expected)
    {
        using SubScrollBar control = new()
        {
            SmallChange = value
        };
        Assert.Equal(expected, control.SmallChange);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SmallChange = value;
        Assert.Equal(expected, control.SmallChange);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ScrollBar_SmallChange_SetLarge_GetReturnsExpected()
    {
        using SubScrollBar control = new()
        {
            LargeChange = 10,
            SmallChange = 11
        };
        Assert.Equal(10, control.SmallChange);
        Assert.False(control.IsHandleCreated);

        // Change large change.
        control.LargeChange = 15;
        Assert.Equal(11, control.SmallChange);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(5, 5)]
    [InlineData(10, 10)]
    [InlineData(11, 10)]
    public void ScrollBar_SmallChange_SetWithHandle_GetReturnsExpected(int value, int expected)
    {
        using SubScrollBar control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SmallChange = value;
        Assert.Equal(expected, control.SmallChange);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.SmallChange = value;
        Assert.Equal(expected, control.SmallChange);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(5, 5)]
    [InlineData(10, 10)]
    [InlineData(11, 10)]
    public void ScrollBar_SmallChange_SetWithHandleDisabled_GetReturnsExpected(int value, int expected)
    {
        using SubScrollBar control = new()
        {
            Enabled = false
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SmallChange = value;
        Assert.Equal(expected, control.SmallChange);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.SmallChange = value;
        Assert.Equal(expected, control.SmallChange);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ScrollBar_SmallChange_SetNegative_ThrowsArgumentOutOfRangeException()
    {
        using SubScrollBar control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.SmallChange = -1);
        Assert.Equal(1, control.SmallChange);
    }

    [WinFormsTheory]
    [BoolData]
    public void ScrollBar_TabStop_Set_GetReturnsExpected(bool value)
    {
        using SubScrollBar control = new()
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
    public void ScrollBar_TabStop_SetWithHandle_GetReturnsExpected(bool value)
    {
        using SubScrollBar control = new();
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
    public void ScrollBar_TabStop_SetWithHandler_CallsTabStopChanged()
    {
        using SubScrollBar control = new()
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
    public void ScrollBar_Text_Set_GetReturnsExpected(string value, string expected)
    {
        using SubScrollBar control = new()
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
    public void ScrollBar_Text_SetWithHandle_GetReturnsExpected(string value, string expected)
    {
        using SubScrollBar control = new();
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
    public void ScrollBar_Text_SetWithHandler_CallsTextChanged()
    {
        using SubScrollBar control = new();
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
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(90)]
    [InlineData(91)]
    [InlineData(100)]
    public void ScrollBar_Value_Set_GetReturnsExpected(int value)
    {
        using SubScrollBar control = new()
        {
            Value = value
        };
        Assert.Equal(100, control.Maximum);
        Assert.Equal(0, control.Minimum);
        Assert.Equal(value, control.Value);
        Assert.Equal(10, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Value = value;
        Assert.Equal(100, control.Maximum);
        Assert.Equal(0, control.Minimum);
        Assert.Equal(value, control.Value);
        Assert.Equal(10, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0, 0)]
    [InlineData(5, 5)]
    [InlineData(90, 90)]
    [InlineData(91, 91)]
    [InlineData(100, 91)]
    public unsafe void ScrollBar_Value_SetWithHandle_GetReturnsExpected(int value, int expectedPos)
    {
        using SubScrollBar control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Value = value;
        Assert.Equal(100, control.Maximum);
        Assert.Equal(0, control.Minimum);
        Assert.Equal(value, control.Value);
        Assert.Equal(10, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        SCROLLINFO si = new()
        {
            cbSize = (uint)sizeof(SCROLLINFO),
            fMask = SCROLLINFO_MASK.SIF_ALL
        };
        Assert.True(PInvoke.GetScrollInfo((HWND)control.Handle, SCROLLBAR_CONSTANTS.SB_CTL, ref si));
        Assert.Equal(100, si.nMax);
        Assert.Equal(0, si.nMin);
        Assert.Equal(expectedPos, si.nPos);
        Assert.Equal(10u, si.nPage);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Value = value;
        Assert.Equal(100, control.Maximum);
        Assert.Equal(0, control.Minimum);
        Assert.Equal(value, control.Value);
        Assert.Equal(10, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        si = new SCROLLINFO
        {
            cbSize = (uint)sizeof(SCROLLINFO),
            fMask = SCROLLINFO_MASK.SIF_ALL
        };
        Assert.True(PInvoke.GetScrollInfo((HWND)control.Handle, SCROLLBAR_CONSTANTS.SB_CTL, ref si));
        Assert.Equal(100, si.nMax);
        Assert.Equal(0, si.nMin);
        Assert.Equal(expectedPos, si.nPos);
        Assert.Equal(10u, si.nPage);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(90)]
    [InlineData(91)]
    [InlineData(100)]
    public unsafe void ScrollBar_Value_SetWithHandleDisabled_GetReturnsExpected(int value)
    {
        using SubScrollBar control = new()
        {
            Enabled = false
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Value = value;
        Assert.Equal(100, control.Maximum);
        Assert.Equal(0, control.Minimum);
        Assert.Equal(value, control.Value);
        Assert.Equal(10, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        SCROLLINFO si = new()
        {
            cbSize = (uint)sizeof(SCROLLINFO),
            fMask = SCROLLINFO_MASK.SIF_ALL
        };
        Assert.True(PInvoke.GetScrollInfo((HWND)control.Handle, SCROLLBAR_CONSTANTS.SB_CTL, ref si));
        Assert.Equal(0, si.nMax);
        Assert.Equal(0, si.nMin);
        Assert.Equal(0, si.nPos);
        Assert.Equal(0u, si.nPage);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Value = value;
        Assert.Equal(100, control.Maximum);
        Assert.Equal(0, control.Minimum);
        Assert.Equal(value, control.Value);
        Assert.Equal(10, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        si = new SCROLLINFO
        {
            cbSize = (uint)sizeof(SCROLLINFO),
            fMask = SCROLLINFO_MASK.SIF_ALL
        };
        Assert.True(PInvoke.GetScrollInfo((HWND)control.Handle, SCROLLBAR_CONSTANTS.SB_CTL, ref si));
        Assert.Equal(0, si.nMax);
        Assert.Equal(0, si.nMin);
        Assert.Equal(0, si.nPos);
        Assert.Equal(0u, si.nPage);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ScrollBar_Value_SetWithHandler_CallsValueChanged()
    {
        using SubScrollBar control = new();
        int callCount = 0;
        EventHandler valueChangedHandler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.ValueChanged += valueChangedHandler;

        // Set different.
        control.Value = 1;
        Assert.Equal(1, control.Value);
        Assert.Equal(1, callCount);

        // Set same.
        control.Value = 1;
        Assert.Equal(1, control.Value);
        Assert.Equal(1, callCount);

        // Set different.
        control.Value = 2;
        Assert.Equal(2, control.Value);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.ValueChanged -= valueChangedHandler;
        control.Value = 1;
        Assert.Equal(1, control.Value);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(101)]
    public void ScrollBar_Value_SetOutOfRange_ThrowsArgumentOutOfRangeException(int value)
    {
        using SubScrollBar control = new();
        string paramName = "value";
        ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(paramName, () => control.Value = value);
        string expectedMessage = new ArgumentOutOfRangeException(paramName, string.Format(SR.InvalidBoundArgument, nameof(control.Value), value, $"'{nameof(control.Minimum)}'", $"'{nameof(control.Maximum)}'")).Message;
        Assert.Equal(expectedMessage, ex.Message);
        Assert.Equal(0, control.Value);
    }

    [WinFormsFact]
    public void ScrollBar_GetAutoSizeMode_Invoke_ReturnsExpected()
    {
        using SubScrollBar control = new();
        Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
    }

    public static IEnumerable<object[]> GetScaledBounds_TestData()
    {
        foreach (BoundsSpecified specified in Enum.GetValues(typeof(BoundsSpecified)))
        {
            yield return new object[] { Rectangle.Empty, new Size(0, 0), specified, Rectangle.Empty };
            yield return new object[] { Rectangle.Empty, new Size(1, 1), specified, Rectangle.Empty };
            yield return new object[] { Rectangle.Empty, new Size(2, 3), specified, Rectangle.Empty };
            yield return new object[] { Rectangle.Empty, new Size(-2, -3), specified, Rectangle.Empty };
        }

        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.All, new Rectangle(0, 0, 0, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.X, new Rectangle(0, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.Y, new Rectangle(1, 0, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.Width, new Rectangle(1, 2, 0, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.Height, new Rectangle(1, 2, 3, 4) };

        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.All, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.X, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.Y, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.Width, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.Height, new Rectangle(1, 2, 3, 4) };

        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.All, new Rectangle(2, 6, 6, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.X, new Rectangle(2, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.Y, new Rectangle(1, 6, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.Width, new Rectangle(1, 2, 6, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.Height, new Rectangle(1, 2, 3, 4) };

        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.All, new Rectangle(-2, -6, -6, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.X, new Rectangle(-2, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.Y, new Rectangle(1, -6, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.Width, new Rectangle(1, 2, -6, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.Height, new Rectangle(1, 2, 3, 4) };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetScaledBounds_TestData))]
    public void ScrollBar_GetScaledBounds_Invoke_ReturnsExpected(Rectangle bounds, SizeF factor, BoundsSpecified specified, Rectangle expected)
    {
        using SubScrollBar control = new();
        Assert.Equal(expected, control.GetScaledBounds(bounds, factor, specified));
        Assert.False(control.IsHandleCreated);

        // Call again.
        Assert.Equal(expected, control.GetScaledBounds(bounds, factor, specified));
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> GetScaledBounds_Vertical_TestData()
    {
        foreach (BoundsSpecified specified in Enum.GetValues(typeof(BoundsSpecified)))
        {
            yield return new object[] { Rectangle.Empty, new Size(0, 0), specified, Rectangle.Empty };
            yield return new object[] { Rectangle.Empty, new Size(1, 1), specified, Rectangle.Empty };
            yield return new object[] { Rectangle.Empty, new Size(2, 3), specified, Rectangle.Empty };
            yield return new object[] { Rectangle.Empty, new Size(-2, -3), specified, Rectangle.Empty };
        }

        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.All, new Rectangle(0, 0, 3, 0) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.X, new Rectangle(0, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.Y, new Rectangle(1, 0, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.Width, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.Height, new Rectangle(1, 2, 3, 0) };

        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.All, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.X, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.Y, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.Width, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.Height, new Rectangle(1, 2, 3, 4) };

        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.All, new Rectangle(2, 6, 3, 12) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.X, new Rectangle(2, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.Y, new Rectangle(1, 6, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.Width, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.Height, new Rectangle(1, 2, 3, 12) };

        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.All, new Rectangle(-2, -6, 3, -12) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.X, new Rectangle(-2, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.Y, new Rectangle(1, -6, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.Width, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.Height, new Rectangle(1, 2, 3, -12) };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetScaledBounds_Vertical_TestData))]
    public void ScrollBar_GetScaledBounds_InvokeVertical_ReturnsExpected(Rectangle bounds, SizeF factor, BoundsSpecified specified, Rectangle expected)
    {
        using VerticalScrollBar control = new();
        Assert.Equal(expected, control.GetScaledBounds(bounds, factor, specified));
        Assert.False(control.IsHandleCreated);

        // Call again.
        Assert.Equal(expected, control.GetScaledBounds(bounds, factor, specified));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(ControlStyles.ContainerControl, false)]
    [InlineData(ControlStyles.UserPaint, false)]
    [InlineData(ControlStyles.Opaque, false)]
    [InlineData(ControlStyles.ResizeRedraw, false)]
    [InlineData(ControlStyles.FixedWidth, false)]
    [InlineData(ControlStyles.FixedHeight, false)]
    [InlineData(ControlStyles.StandardClick, false)]
    [InlineData(ControlStyles.Selectable, true)]
    [InlineData(ControlStyles.UserMouse, false)]
    [InlineData(ControlStyles.SupportsTransparentBackColor, false)]
    [InlineData(ControlStyles.StandardDoubleClick, true)]
    [InlineData(ControlStyles.AllPaintingInWmPaint, true)]
    [InlineData(ControlStyles.CacheText, false)]
    [InlineData(ControlStyles.EnableNotifyMessage, false)]
    [InlineData(ControlStyles.DoubleBuffer, false)]
    [InlineData(ControlStyles.OptimizedDoubleBuffer, false)]
    [InlineData(ControlStyles.UseTextForAccessibility, false)]
    [InlineData((ControlStyles)0, true)]
    [InlineData((ControlStyles)int.MaxValue, false)]
    [InlineData((ControlStyles)(-1), false)]
    public void ScrollBar_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
    {
        using SubScrollBar control = new();
        Assert.Equal(expected, control.GetStyle(flag));

        // Call again to test caching.
        Assert.Equal(expected, control.GetStyle(flag));
    }

    [WinFormsFact]
    public void ScrollBar_GetTopLevel_Invoke_ReturnsExpected()
    {
        using SubScrollBar control = new();
        Assert.False(control.GetTopLevel());
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ScrollBar_OnClick_Invoke_CallsClick(EventArgs eventArgs)
    {
        using SubScrollBar control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.Click += handler;
        control.OnClick(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.Click -= handler;
        control.OnClick(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ScrollBar_OnDoubleClick_Invoke_CallsDoubleClick(EventArgs eventArgs)
    {
        using SubScrollBar control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.DoubleClick += handler;
        control.OnDoubleClick(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.DoubleClick -= handler;
        control.OnDoubleClick(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ScrollBar_OnEnabledChanged_Invoke_CallsEnabled(EventArgs eventArgs)
    {
        using SubScrollBar control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.EnabledChanged += handler;
        control.OnEnabledChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.EnabledChanged -= handler;
        control.OnEnabledChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ScrollBar_OnEnabledChanged_InvokeWithHandle_CallsEnabledChanged(EventArgs eventArgs)
    {
        using SubScrollBar control = new();
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
        control.EnabledChanged += handler;
        control.OnEnabledChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.EnabledChanged -= handler;
        control.OnEnabledChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ScrollBar_OnHandleCreated_Invoke_CallsHandleCreated(EventArgs eventArgs)
    {
        using SubScrollBar control = new();
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
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.HandleCreated -= handler;
        control.OnHandleCreated(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ScrollBar_OnHandleCreated_InvokeWithHandle_CallsHandleCreated(EventArgs eventArgs)
    {
        using SubScrollBar control = new();
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
        Assert.True(control.IsHandleCreated);

        // Remove handler.
        control.HandleCreated -= handler;
        control.OnHandleCreated(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetMouseEventArgsTheoryData))]
    public void ScrollBar_OnMouseClick_Invoke_CallsMouseClick(MouseEventArgs eventArgs)
    {
        using SubScrollBar control = new();
        int callCount = 0;
        MouseEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.MouseClick += handler;
        control.OnMouseClick(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.MouseClick -= handler;
        control.OnMouseClick(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetMouseEventArgsTheoryData))]
    public void ScrollBar_OnMouseDoubleClick_Invoke_CallsMouseDoubleClick(MouseEventArgs eventArgs)
    {
        using SubScrollBar control = new();
        int callCount = 0;
        MouseEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.MouseDoubleClick += handler;
        control.OnMouseDoubleClick(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.MouseDoubleClick -= handler;
        control.OnMouseDoubleClick(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetMouseEventArgsTheoryData))]
    public void ScrollBar_OnMouseDown_Invoke_CallsMouseDown(MouseEventArgs eventArgs)
    {
        using SubScrollBar control = new();
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

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetMouseEventArgsTheoryData))]
    public void ScrollBar_OnMouseMove_Invoke_CallsMouseMove(MouseEventArgs eventArgs)
    {
        using SubScrollBar control = new();
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
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetMouseEventArgsTheoryData))]
    public void ScrollBar_OnMouseUp_Invoke_CallsMouseUp(MouseEventArgs eventArgs)
    {
        using SubScrollBar control = new();
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

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetMouseEventArgsTheoryData))]
    public void ScrollBar_OnMouseWheel_Invoke_CallsMouseWheel(MouseEventArgs eventArgs)
    {
        using SubScrollBar control = new();
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

    [WinFormsFact]
    public void ScrollBar_OnMouseWheel_InvokeHandledMouseEventArgs_SetsHandled()
    {
        using SubScrollBar control = new();
        HandledMouseEventArgs eventArgs = new(MouseButtons.Left, 1, 2, 3, 4);
        int callCount = 0;
        MouseEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            Assert.True(eventArgs.Handled);
            callCount++;
        };
        control.MouseWheel += handler;

        control.OnMouseWheel(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(eventArgs.Handled);
    }

    public static IEnumerable<object[]> OnMouseWheel_TestData()
    {
        yield return new object[] { RightToLeft.No, 10, -119, new List<ScrollEventArgs>(), 10 };
        yield return new object[] { RightToLeft.No, 10, 0, new List<ScrollEventArgs>(), 10 };
        yield return new object[] { RightToLeft.No, 10, 119, new List<ScrollEventArgs>(), 10 };
        yield return new object[] { RightToLeft.Yes, 10, -119, new List<ScrollEventArgs>(), 10 };
        yield return new object[] { RightToLeft.Yes, 10, 0, new List<ScrollEventArgs>(), 10 };
        yield return new object[] { RightToLeft.Yes, 10, 119, new List<ScrollEventArgs>(), 10 };

        // Decrement.
        yield return new object[]
        {
            RightToLeft.No, 10, 120,
            new List<ScrollEventArgs>
            {
                new(ScrollEventType.SmallDecrement, 10, 9),
                new(ScrollEventType.EndScroll, 9, 9),
            }, 9
        };
        yield return new object[]
        {
            RightToLeft.No, 10, 121,
            new List<ScrollEventArgs>
            {
                new(ScrollEventType.SmallDecrement, 10, 9),
                new(ScrollEventType.EndScroll, 9, 9),
            }, 9
        };
        yield return new object[]
        {
            RightToLeft.No, 10, 240,
            new List<ScrollEventArgs>
            {
                new(ScrollEventType.SmallDecrement, 10, 9),
                new(ScrollEventType.SmallDecrement, 9, 8),
                new(ScrollEventType.EndScroll, 8, 8),
            }, 8
        };
        yield return new object[]
        {
            RightToLeft.No, 1, 120,
            new List<ScrollEventArgs>
            {
                new(ScrollEventType.SmallDecrement, 1, 0),
                new(ScrollEventType.EndScroll, 0, 0),
            }, 0
        };
        yield return new object[]
        {
            RightToLeft.No, 1, 240,
            new List<ScrollEventArgs>
            {
                new(ScrollEventType.SmallDecrement, 1, 0),
                new(ScrollEventType.SmallDecrement, 0, 0),
                new(ScrollEventType.EndScroll, 0, 0),
            }, 0
        };
        yield return new object[]
        {
            RightToLeft.No, 100, 240,
            new List<ScrollEventArgs>
            {
                new(ScrollEventType.SmallDecrement, 100, 99),
                new(ScrollEventType.SmallDecrement, 99, 98),
                new(ScrollEventType.EndScroll, 98, 98),
            }, 98
        };

        yield return new object[]
        {
            RightToLeft.Yes, 10, -120,
            new List<ScrollEventArgs>
            {
                new(ScrollEventType.SmallDecrement, 10, 9),
                new(ScrollEventType.EndScroll, 9, 9),
            }, 9
        };
        yield return new object[]
        {
            RightToLeft.Yes, 10, -121,
            new List<ScrollEventArgs>
            {
                new(ScrollEventType.SmallDecrement, 10, 9),
                new(ScrollEventType.EndScroll, 9, 9),
            }, 9
        };
        yield return new object[]
        {
            RightToLeft.Yes, 10, -240,
            new List<ScrollEventArgs>
            {
                new(ScrollEventType.SmallDecrement, 10, 9),
                new(ScrollEventType.SmallDecrement, 9, 8),
                new(ScrollEventType.EndScroll, 8, 8),
            }, 8
        };
        yield return new object[]
        {
            RightToLeft.Yes, 1, -120,
            new List<ScrollEventArgs>
            {
                new(ScrollEventType.SmallDecrement, 1, 0),
                new(ScrollEventType.EndScroll, 0, 0),
            }, 0
        };
        yield return new object[]
        {
            RightToLeft.Yes, 1, -240,
            new List<ScrollEventArgs>
            {
                new(ScrollEventType.SmallDecrement, 1, 0),
                new(ScrollEventType.SmallDecrement, 0, 0),
                new(ScrollEventType.EndScroll, 0, 0),
            }, 0
        };
        yield return new object[]
        {
            RightToLeft.Yes, 100, -240,
            new List<ScrollEventArgs>
            {
                new(ScrollEventType.SmallDecrement, 100, 99),
                new(ScrollEventType.SmallDecrement, 99, 98),
                new(ScrollEventType.EndScroll, 98, 98),
            }, 98
        };

        // Increment.
        yield return new object[]
        {
            RightToLeft.No, 10, -120,
            new List<ScrollEventArgs>
            {
                new(ScrollEventType.SmallIncrement, 10, 11),
                new(ScrollEventType.EndScroll, 11, 11),
            }, 11
        };
        yield return new object[]
        {
            RightToLeft.No, 10, -121,
            new List<ScrollEventArgs>
            {
                new(ScrollEventType.SmallIncrement, 10, 11),
                new(ScrollEventType.EndScroll, 11, 11),
            }, 11
        };
        yield return new object[]
        {
            RightToLeft.No, 10, -240,
            new List<ScrollEventArgs>
            {
                new(ScrollEventType.SmallIncrement, 10, 11),
                new(ScrollEventType.SmallIncrement, 11, 12),
                new(ScrollEventType.EndScroll, 12, 12),
            }, 12
        };
        yield return new object[]
        {
            RightToLeft.No, 90, -120,
            new List<ScrollEventArgs>
            {
                new(ScrollEventType.SmallIncrement, 90, 91),
                new(ScrollEventType.EndScroll, 91, 91),
            }, 91
        };
        yield return new object[]
        {
            RightToLeft.No, 99, -120,
            new List<ScrollEventArgs>
            {
                new(ScrollEventType.SmallIncrement, 99, 91),
                new(ScrollEventType.EndScroll, 91, 91),
            }, 91
        };
        yield return new object[]
        {
            RightToLeft.No, 99, -240,
            new List<ScrollEventArgs>
            {
                new(ScrollEventType.SmallIncrement, 99, 91),
                new(ScrollEventType.SmallIncrement, 91, 91),
                new(ScrollEventType.EndScroll, 91, 91),
            }, 91
        };

        yield return new object[]
        {
            RightToLeft.Yes, 10, 120,
            new List<ScrollEventArgs>
            {
                new(ScrollEventType.SmallIncrement, 10, 11),
                new(ScrollEventType.EndScroll, 11, 11),
            }, 11
        };
        yield return new object[]
        {
            RightToLeft.Yes, 10, 121,
            new List<ScrollEventArgs>
            {
                new(ScrollEventType.SmallIncrement, 10, 11),
                new(ScrollEventType.EndScroll, 11, 11),
            }, 11
        };
        yield return new object[]
        {
            RightToLeft.Yes, 10, 240,
            new List<ScrollEventArgs>
            {
                new(ScrollEventType.SmallIncrement, 10, 11),
                new(ScrollEventType.SmallIncrement, 11, 12),
                new(ScrollEventType.EndScroll, 12, 12),
            }, 12
        };
        yield return new object[]
        {
            RightToLeft.Yes, 90, 120,
            new List<ScrollEventArgs>
            {
                new(ScrollEventType.SmallIncrement, 90, 91),
                new(ScrollEventType.EndScroll, 91, 91),
            }, 91
        };
        yield return new object[]
        {
            RightToLeft.Yes, 99, 120,
            new List<ScrollEventArgs>
            {
                new(ScrollEventType.SmallIncrement, 99, 91),
                new(ScrollEventType.EndScroll, 91, 91),
            }, 91
        };
        yield return new object[]
        {
            RightToLeft.Yes, 99, 240,
            new List<ScrollEventArgs>
            {
                new(ScrollEventType.SmallIncrement, 99, 91),
                new(ScrollEventType.SmallIncrement, 91, 91),
                new(ScrollEventType.EndScroll, 91, 91),
            }, 91
        };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnMouseWheel_TestData))]
    public void ScrollBar_OnMouseWheel_InvokeWithScroll_CallsScroll(RightToLeft rightToLeft, int originalValue, int delta, IList<ScrollEventArgs> expected, int expectedValue)
    {
        using SubScrollBar control = new()
        {
            RightToLeft = rightToLeft,
            Value = originalValue
        };
        MouseEventArgs eventArgs = new(MouseButtons.Left, 0, 0, 0, delta);
        int callCount = 0;
        ScrollEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(expected[callCount].Type, e.Type);
            Assert.Equal(expected[callCount].NewValue, e.NewValue);
            Assert.Equal(expected[callCount].OldValue, e.OldValue);
            Assert.Equal(ScrollOrientation.HorizontalScroll, e.ScrollOrientation);
            callCount++;
        };
        control.Scroll += handler;

        control.OnMouseWheel(eventArgs);
        Assert.Equal(expectedValue, control.Value);
        Assert.Equal(expected.Count, callCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaintEventArgsTheoryData))]
    public void ScrollBar_OnPaint_Invoke_CallsPaint(PaintEventArgs eventArgs)
    {
        using SubScrollBar control = new();
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

        // Remove handler.
        control.Paint -= handler;
        control.OnPaint(eventArgs);
        Assert.Equal(1, callCount);
    }

    public static IEnumerable<object[]> OnScroll_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new ScrollEventArgs(ScrollEventType.SmallDecrement, 2) };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnScroll_TestData))]
    public void ScrollBar_OnScroll_Invoke_CallsScroll(ScrollEventArgs eventArgs)
    {
        using SubScrollBar control = new();
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

        // Remove handler.
        control.Scroll -= handler;
        control.OnScroll(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ScrollBar_OnValueChanged_Invoke_CallsValueChanged(EventArgs eventArgs)
    {
        using SubScrollBar control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.ValueChanged += handler;
        control.OnValueChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.ValueChanged -= handler;
        control.OnValueChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    public static IEnumerable<object[]> ScaleScrollBarForDpi_WithSize_TestData()
    {
        yield return new object[] { true, 100, 200, ScrollOrientation.HorizontalScroll, new Size(10, 20), new Size(20, 20) };
        yield return new object[] { true, 200, 100, ScrollOrientation.HorizontalScroll, new Size(10, 20), new Size(5, 20) };
        yield return new object[] { true, 100, 200, ScrollOrientation.VerticalScroll, new Size(10, 20), new Size(10, 40) };
        yield return new object[] { true, 200, 100, ScrollOrientation.VerticalScroll, new Size(10, 20), new Size(10, 10) };

        yield return new object[] { false, 100, 200, ScrollOrientation.HorizontalScroll, new Size(10, 20), new Size(10, 20) };
        yield return new object[] { false, 100, 100, ScrollOrientation.VerticalScroll, new Size(10, 20), new Size(10, 20) };
    }

    [WinFormsTheory]
    [MemberData(nameof(ScaleScrollBarForDpi_WithSize_TestData))]
    public void ScrollBar_ScaleScrollBarForDpi_InvokeWithSize_Nop(bool scaleScrollBarForDpiChange, int deviceDpiOld, int deviceDpiNew, ScrollOrientation orientation, Size controlSize, Size expected)
    {
        using SubScrollBar control = new()
        {
            ScaleScrollBarForDpiChange = scaleScrollBarForDpiChange,
            Size = controlSize
        };

        control.TestAccessor().Dynamic._scrollOrientation = orientation;

        SizeF factor = new(((float)deviceDpiNew) / deviceDpiOld, ((float)deviceDpiNew) / deviceDpiOld);
        control.ScaleControl(factor, factor);

        Assert.Equal(expected, control.Size);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ScrollBar_UpdateScrollInfo_NoHandle_Success(bool enabled)
    {
        using SubScrollBar control = new()
        {
            Enabled = enabled
        };
        control.UpdateScrollInfo();
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ScrollBar_UpdateScrollInfo_WithHandle_Success(bool enabled)
    {
        using SubScrollBar control = new()
        {
            Enabled = enabled
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.UpdateScrollInfo();
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void ScrollBar_UpdateScrollInfo_WithHandleRightToLeft_Success(bool enabled)
    {
        using SubScrollBar control = new()
        {
            Enabled = enabled,
            RightToLeft = RightToLeft.Yes
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.UpdateScrollInfo();
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ScrollBar_ToString_Invoke_ReturnsExpected()
    {
        using SubScrollBar control = new();
        Assert.Equal("System.Windows.Forms.Tests.ScrollBarTests+SubScrollBar, Minimum: 0, Maximum: 100, Value: 0", control.ToString());
    }

    public static IEnumerable<object[]> WndProc_EraseBkgnd_TestData()
    {
        foreach (bool userPaint in new bool[] { true, false })
        {
            foreach (bool allPaintingInWmPaint in new bool[] { true, false })
            {
                foreach (bool opaque in new bool[] { true, false })
                {
                    yield return new object[] { userPaint, allPaintingInWmPaint, opaque };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_EraseBkgnd_TestData))]
    public void Control_WndProc_InvokeEraseBkgnd_Nop(bool userPaint, bool allPaintingInWmPaint, bool opaque)
    {
        using SubScrollBar control = new();
        control.SetStyle(ControlStyles.UserPaint, userPaint);
        control.SetStyle(ControlStyles.AllPaintingInWmPaint, allPaintingInWmPaint);
        control.SetStyle(ControlStyles.Opaque, opaque);
        int paintCallCount = 0;
        control.Paint += (sender, e) => paintCallCount++;

        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_ERASEBKGND,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(250, m.Result);
        Assert.False(control.IsHandleCreated);
        Assert.Equal(0, paintCallCount);
    }

    public static IEnumerable<object[]> WndProc_Scroll_TestData()
    {
        foreach (MessageId msg in new MessageId[] { MessageId.WM_REFLECT | PInvokeCore.WM_HSCROLL, MessageId.WM_REFLECT | PInvokeCore.WM_VSCROLL })
        {
            yield return new object[] { msg, RightToLeft.No, 100, ScrollEventType.SmallIncrement, 91, ScrollEventType.SmallIncrement };
            yield return new object[] { msg, RightToLeft.No, 99, ScrollEventType.SmallIncrement, 91, ScrollEventType.SmallIncrement };
            yield return new object[] { msg, RightToLeft.No, 91, ScrollEventType.SmallIncrement, 91, ScrollEventType.SmallIncrement };
            yield return new object[] { msg, RightToLeft.No, 85, ScrollEventType.SmallIncrement, 86, ScrollEventType.SmallIncrement };
            yield return new object[] { msg, RightToLeft.No, 15, ScrollEventType.SmallIncrement, 16, ScrollEventType.SmallIncrement };
            yield return new object[] { msg, RightToLeft.No, 10, ScrollEventType.SmallIncrement, 11, ScrollEventType.SmallIncrement };
            yield return new object[] { msg, RightToLeft.No, 1, ScrollEventType.SmallIncrement, 2, ScrollEventType.SmallIncrement };
            yield return new object[] { msg, RightToLeft.No, 0, ScrollEventType.SmallIncrement, 1, ScrollEventType.SmallIncrement };
            yield return new object[] { msg, RightToLeft.Yes, 100, ScrollEventType.SmallDecrement, 91, ScrollEventType.SmallIncrement };
            yield return new object[] { msg, RightToLeft.Yes, 99, ScrollEventType.SmallDecrement, 91, ScrollEventType.SmallIncrement };
            yield return new object[] { msg, RightToLeft.Yes, 91, ScrollEventType.SmallDecrement, 91, ScrollEventType.SmallIncrement };
            yield return new object[] { msg, RightToLeft.Yes, 85, ScrollEventType.SmallDecrement, 86, ScrollEventType.SmallIncrement };
            yield return new object[] { msg, RightToLeft.Yes, 15, ScrollEventType.SmallDecrement, 16, ScrollEventType.SmallIncrement };
            yield return new object[] { msg, RightToLeft.Yes, 10, ScrollEventType.SmallDecrement, 11, ScrollEventType.SmallIncrement };
            yield return new object[] { msg, RightToLeft.Yes, 1, ScrollEventType.SmallDecrement, 2, ScrollEventType.SmallIncrement };
            yield return new object[] { msg, RightToLeft.Yes, 0, ScrollEventType.SmallDecrement, 1, ScrollEventType.SmallIncrement };

            yield return new object[] { msg, RightToLeft.No, 100, ScrollEventType.LargeIncrement, 91, ScrollEventType.LargeIncrement };
            yield return new object[] { msg, RightToLeft.No, 99, ScrollEventType.LargeIncrement, 91, ScrollEventType.LargeIncrement };
            yield return new object[] { msg, RightToLeft.No, 91, ScrollEventType.LargeIncrement, 91, ScrollEventType.LargeIncrement };
            yield return new object[] { msg, RightToLeft.No, 85, ScrollEventType.LargeIncrement, 91, ScrollEventType.LargeIncrement };
            yield return new object[] { msg, RightToLeft.No, 15, ScrollEventType.LargeIncrement, 25, ScrollEventType.LargeIncrement };
            yield return new object[] { msg, RightToLeft.No, 10, ScrollEventType.LargeIncrement, 20, ScrollEventType.LargeIncrement };
            yield return new object[] { msg, RightToLeft.No, 1, ScrollEventType.LargeIncrement, 11, ScrollEventType.LargeIncrement };
            yield return new object[] { msg, RightToLeft.No, 0, ScrollEventType.LargeIncrement, 10, ScrollEventType.LargeIncrement };
            yield return new object[] { msg, RightToLeft.Yes, 100, ScrollEventType.LargeDecrement, 91, ScrollEventType.LargeIncrement };
            yield return new object[] { msg, RightToLeft.Yes, 99, ScrollEventType.LargeDecrement, 91, ScrollEventType.LargeIncrement };
            yield return new object[] { msg, RightToLeft.Yes, 91, ScrollEventType.LargeDecrement, 91, ScrollEventType.LargeIncrement };
            yield return new object[] { msg, RightToLeft.Yes, 85, ScrollEventType.LargeDecrement, 91, ScrollEventType.LargeIncrement };
            yield return new object[] { msg, RightToLeft.Yes, 15, ScrollEventType.LargeDecrement, 25, ScrollEventType.LargeIncrement };
            yield return new object[] { msg, RightToLeft.Yes, 10, ScrollEventType.LargeDecrement, 20, ScrollEventType.LargeIncrement };
            yield return new object[] { msg, RightToLeft.Yes, 1, ScrollEventType.LargeDecrement, 11, ScrollEventType.LargeIncrement };
            yield return new object[] { msg, RightToLeft.Yes, 0, ScrollEventType.LargeDecrement, 10, ScrollEventType.LargeIncrement };

            yield return new object[] { msg, RightToLeft.No, 100, ScrollEventType.SmallDecrement, 99, ScrollEventType.SmallDecrement };
            yield return new object[] { msg, RightToLeft.No, 99, ScrollEventType.SmallDecrement, 98, ScrollEventType.SmallDecrement };
            yield return new object[] { msg, RightToLeft.No, 91, ScrollEventType.SmallDecrement, 90, ScrollEventType.SmallDecrement };
            yield return new object[] { msg, RightToLeft.No, 85, ScrollEventType.SmallDecrement, 84, ScrollEventType.SmallDecrement };
            yield return new object[] { msg, RightToLeft.No, 15, ScrollEventType.SmallDecrement, 14, ScrollEventType.SmallDecrement };
            yield return new object[] { msg, RightToLeft.No, 10, ScrollEventType.SmallDecrement, 9, ScrollEventType.SmallDecrement };
            yield return new object[] { msg, RightToLeft.No, 1, ScrollEventType.SmallDecrement, 0, ScrollEventType.SmallDecrement };
            yield return new object[] { msg, RightToLeft.No, 0, ScrollEventType.SmallDecrement, 0, ScrollEventType.SmallDecrement };
            yield return new object[] { msg, RightToLeft.Yes, 100, ScrollEventType.SmallIncrement, 99, ScrollEventType.SmallDecrement };
            yield return new object[] { msg, RightToLeft.Yes, 99, ScrollEventType.SmallIncrement, 98, ScrollEventType.SmallDecrement };
            yield return new object[] { msg, RightToLeft.Yes, 91, ScrollEventType.SmallIncrement, 90, ScrollEventType.SmallDecrement };
            yield return new object[] { msg, RightToLeft.Yes, 85, ScrollEventType.SmallIncrement, 84, ScrollEventType.SmallDecrement };
            yield return new object[] { msg, RightToLeft.Yes, 15, ScrollEventType.SmallIncrement, 14, ScrollEventType.SmallDecrement };
            yield return new object[] { msg, RightToLeft.Yes, 10, ScrollEventType.SmallIncrement, 9, ScrollEventType.SmallDecrement };
            yield return new object[] { msg, RightToLeft.Yes, 1, ScrollEventType.SmallIncrement, 0, ScrollEventType.SmallDecrement };
            yield return new object[] { msg, RightToLeft.Yes, 0, ScrollEventType.SmallIncrement, 0, ScrollEventType.SmallDecrement };

            yield return new object[] { msg, RightToLeft.No, 100, ScrollEventType.LargeDecrement, 90, ScrollEventType.LargeDecrement };
            yield return new object[] { msg, RightToLeft.No, 99, ScrollEventType.LargeDecrement, 89, ScrollEventType.LargeDecrement };
            yield return new object[] { msg, RightToLeft.No, 91, ScrollEventType.LargeDecrement, 81, ScrollEventType.LargeDecrement };
            yield return new object[] { msg, RightToLeft.No, 85, ScrollEventType.LargeDecrement, 75, ScrollEventType.LargeDecrement };
            yield return new object[] { msg, RightToLeft.No, 15, ScrollEventType.LargeDecrement, 5, ScrollEventType.LargeDecrement };
            yield return new object[] { msg, RightToLeft.No, 10, ScrollEventType.LargeDecrement, 0, ScrollEventType.LargeDecrement };
            yield return new object[] { msg, RightToLeft.No, 1, ScrollEventType.LargeDecrement, 0, ScrollEventType.LargeDecrement };
            yield return new object[] { msg, RightToLeft.No, 0, ScrollEventType.LargeDecrement, 0, ScrollEventType.LargeDecrement };
            yield return new object[] { msg, RightToLeft.Yes, 100, ScrollEventType.LargeIncrement, 90, ScrollEventType.LargeDecrement };
            yield return new object[] { msg, RightToLeft.Yes, 99, ScrollEventType.LargeIncrement, 89, ScrollEventType.LargeDecrement };
            yield return new object[] { msg, RightToLeft.Yes, 91, ScrollEventType.LargeIncrement, 81, ScrollEventType.LargeDecrement };
            yield return new object[] { msg, RightToLeft.Yes, 85, ScrollEventType.LargeIncrement, 75, ScrollEventType.LargeDecrement };
            yield return new object[] { msg, RightToLeft.Yes, 15, ScrollEventType.LargeIncrement, 5, ScrollEventType.LargeDecrement };
            yield return new object[] { msg, RightToLeft.Yes, 10, ScrollEventType.LargeIncrement, 0, ScrollEventType.LargeDecrement };
            yield return new object[] { msg, RightToLeft.Yes, 1, ScrollEventType.LargeIncrement, 0, ScrollEventType.LargeDecrement };
            yield return new object[] { msg, RightToLeft.Yes, 0, ScrollEventType.LargeIncrement, 0, ScrollEventType.LargeDecrement };

            yield return new object[] { msg, RightToLeft.No, 100, ScrollEventType.First, 0, ScrollEventType.First };
            yield return new object[] { msg, RightToLeft.No, 99, ScrollEventType.First, 0, ScrollEventType.First };
            yield return new object[] { msg, RightToLeft.No, 91, ScrollEventType.First, 0, ScrollEventType.First };
            yield return new object[] { msg, RightToLeft.No, 85, ScrollEventType.First, 0, ScrollEventType.First };
            yield return new object[] { msg, RightToLeft.No, 15, ScrollEventType.First, 0, ScrollEventType.First };
            yield return new object[] { msg, RightToLeft.No, 10, ScrollEventType.First, 0, ScrollEventType.First };
            yield return new object[] { msg, RightToLeft.No, 1, ScrollEventType.First, 0, ScrollEventType.First };
            yield return new object[] { msg, RightToLeft.No, 0, ScrollEventType.First, 0, ScrollEventType.First };
            yield return new object[] { msg, RightToLeft.Yes, 100, ScrollEventType.Last, 0, ScrollEventType.First };
            yield return new object[] { msg, RightToLeft.Yes, 99, ScrollEventType.Last, 0, ScrollEventType.First };
            yield return new object[] { msg, RightToLeft.Yes, 91, ScrollEventType.Last, 0, ScrollEventType.First };
            yield return new object[] { msg, RightToLeft.Yes, 85, ScrollEventType.Last, 0, ScrollEventType.First };
            yield return new object[] { msg, RightToLeft.Yes, 15, ScrollEventType.Last, 0, ScrollEventType.First };
            yield return new object[] { msg, RightToLeft.Yes, 10, ScrollEventType.Last, 0, ScrollEventType.First };
            yield return new object[] { msg, RightToLeft.Yes, 1, ScrollEventType.Last, 0, ScrollEventType.First };
            yield return new object[] { msg, RightToLeft.Yes, 0, ScrollEventType.Last, 0, ScrollEventType.First };

            yield return new object[] { msg, RightToLeft.No, 100, ScrollEventType.Last, 91, ScrollEventType.Last };
            yield return new object[] { msg, RightToLeft.No, 99, ScrollEventType.Last, 91, ScrollEventType.Last };
            yield return new object[] { msg, RightToLeft.No, 91, ScrollEventType.Last, 91, ScrollEventType.Last };
            yield return new object[] { msg, RightToLeft.No, 85, ScrollEventType.Last, 91, ScrollEventType.Last };
            yield return new object[] { msg, RightToLeft.No, 15, ScrollEventType.Last, 91, ScrollEventType.Last };
            yield return new object[] { msg, RightToLeft.No, 10, ScrollEventType.Last, 91, ScrollEventType.Last };
            yield return new object[] { msg, RightToLeft.No, 1, ScrollEventType.Last, 91, ScrollEventType.Last };
            yield return new object[] { msg, RightToLeft.No, 0, ScrollEventType.Last, 91, ScrollEventType.Last };
            yield return new object[] { msg, RightToLeft.Yes, 100, ScrollEventType.First, 91, ScrollEventType.Last };
            yield return new object[] { msg, RightToLeft.Yes, 99, ScrollEventType.First, 91, ScrollEventType.Last };
            yield return new object[] { msg, RightToLeft.Yes, 91, ScrollEventType.First, 91, ScrollEventType.Last };
            yield return new object[] { msg, RightToLeft.Yes, 85, ScrollEventType.First, 91, ScrollEventType.Last };
            yield return new object[] { msg, RightToLeft.Yes, 15, ScrollEventType.First, 91, ScrollEventType.Last };
            yield return new object[] { msg, RightToLeft.Yes, 10, ScrollEventType.First, 91, ScrollEventType.Last };
            yield return new object[] { msg, RightToLeft.Yes, 1, ScrollEventType.First, 91, ScrollEventType.Last };
            yield return new object[] { msg, RightToLeft.Yes, 0, ScrollEventType.First, 91, ScrollEventType.Last };

            yield return new object[] { msg, RightToLeft.No, 10, ScrollEventType.EndScroll, 10, ScrollEventType.EndScroll };
            yield return new object[] { msg, RightToLeft.Yes, 10, ScrollEventType.EndScroll, 10, ScrollEventType.EndScroll };
            yield return new object[] { msg, RightToLeft.No, 10, ScrollEventType.ThumbPosition, 10, ScrollEventType.ThumbPosition };
            yield return new object[] { msg, RightToLeft.Yes, 10, ScrollEventType.ThumbPosition, 10, ScrollEventType.ThumbPosition };
            yield return new object[] { msg, RightToLeft.No, 10, ScrollEventType.ThumbTrack, 10, ScrollEventType.ThumbTrack };
            yield return new object[] { msg, RightToLeft.Yes, 10, ScrollEventType.ThumbTrack, 10, ScrollEventType.ThumbTrack };
            yield return new object[] { msg, RightToLeft.No, 10, ScrollEventType.SmallDecrement - 1, 10, (ScrollEventType)ushort.MaxValue };
            yield return new object[] { msg, RightToLeft.Yes, 10, ScrollEventType.SmallDecrement - 1, 10, (ScrollEventType)ushort.MaxValue };
            yield return new object[] { msg, RightToLeft.No, 10, ScrollEventType.EndScroll + 1, 10, ScrollEventType.EndScroll + 1 };
            yield return new object[] { msg, RightToLeft.Yes, 10, ScrollEventType.EndScroll + 1, 10, ScrollEventType.EndScroll + 1 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_Scroll_TestData))]
    public void ScrollBar_WndProc_InvokeScroll_Success(int msg, RightToLeft rightToLeft, int originalValue, ScrollEventType eventType, int expectedValue, ScrollEventType expectedEventType)
    {
        using SubScrollBar control = new()
        {
            RightToLeft = rightToLeft,
            Value = originalValue
        };
        int callCount = 0;
        ScrollEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(expectedEventType, e.Type);
            Assert.Equal(expectedValue, e.NewValue);
            Assert.Equal(originalValue, e.OldValue);
            Assert.Equal(ScrollOrientation.HorizontalScroll, e.ScrollOrientation);
            callCount++;
        };
        control.Scroll += handler;

        Message message = new()
        {
            Msg = msg,
            WParam = (IntPtr)eventType
        };
        control.WndProc(ref message);
        Assert.Equal(1, callCount);
        Assert.Equal(expectedValue, control.Value);
    }

    [WinFormsFact]
    public void Control_WndProc_InvokeSizeWithoutHandle_Success()
    {
        using SubScrollBar control = new();

        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_SIZE,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(250, m.Result);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Control_WndProc_InvokeSizeWithHandle_Success()
    {
        using SubScrollBar control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_SIZE,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(250, m.Result);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ScrollBar_WndProc_InvokeMouseHoverWithHandle_Success()
    {
        using SubScrollBar control = new();
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

    [WinFormsTheory]
    [InlineData(RightToLeft.Yes, 0)]
    [InlineData(RightToLeft.Yes, 50)]
    [InlineData(RightToLeft.No, 50)]
    [InlineData(RightToLeft.No, 100)]
    public void HScrollBarFirstPageButtonAccessibleObject_Bounds_ReturnRectangle(RightToLeft rightToLeft, int value)
    {
        using HScrollBar hScrollBar = new();
        SetHScrollBar(hScrollBar, rightToLeft, minimum: 0, maximum: 100, value);

        Assert.True(HFirstPageButton(hScrollBar).Bounds.Width > 0);
        Assert.True(HFirstPageButton(hScrollBar).Bounds.Height > 0);
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.Yes, 100, 100)]
    [InlineData(RightToLeft.No, 0, 0)]
    [InlineData(RightToLeft.Yes, 0, 0)]
    [InlineData(RightToLeft.No, 100, 0)]
    public void HScrollBarFirstPageButtonAccessibleObject_Bounds_ReturnEmptyRectangle(RightToLeft rightToLeft, int maximum, int value)
    {
        using HScrollBar hScrollBar = new();
        SetHScrollBar(hScrollBar, rightToLeft, minimum: 0, maximum, value);

        Assert.True(HFirstPageButton(hScrollBar).Bounds.Width == 0 || HFirstPageButton(hScrollBar).Bounds.Height == 0);
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.No, 100, 0, AccessibleStates.Invisible)]
    [InlineData(RightToLeft.No, 100, 50, AccessibleStates.None)]
    [InlineData(RightToLeft.No, 100, 100, AccessibleStates.None)]
    [InlineData(RightToLeft.Yes, 100, 0, AccessibleStates.None)]
    [InlineData(RightToLeft.Yes, 100, 50, AccessibleStates.None)]
    [InlineData(RightToLeft.Yes, 100, 100, AccessibleStates.Invisible)]
    [InlineData(RightToLeft.No, 0, 0, AccessibleStates.Invisible)]
    [InlineData(RightToLeft.Yes, 0, 0, AccessibleStates.Invisible)]
    public void HScrollBarFirstPageButtonAccessibleObject_State_ReturnExpected(RightToLeft rightToLeft, int maximum, int value, AccessibleStates accessibleState)
    {
        using HScrollBar hScrollBar = new();
        SetHScrollBar(hScrollBar, rightToLeft, minimum: 0, maximum, value);

        Assert.Equal(accessibleState, HFirstPageButton(hScrollBar).State);
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.No, 0)]
    [InlineData(RightToLeft.No, 50)]
    [InlineData(RightToLeft.Yes, 100)]
    [InlineData(RightToLeft.Yes, 50)]
    public void HScrollBarLastPageButtonAccessibleObject_Bounds_ReturnRectangle(RightToLeft rightToLeft, int value)
    {
        using HScrollBar hScrollBar = new();
        SetHScrollBar(hScrollBar, rightToLeft, minimum: 0, maximum: 100, value);

        Assert.True(HLastPageButton(hScrollBar).Bounds.Width > 0);
        Assert.True(HLastPageButton(hScrollBar).Bounds.Height > 0);
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.No, 100, 100)]
    [InlineData(RightToLeft.Yes, 100, 0)]
    [InlineData(RightToLeft.No, 0, 0)]
    [InlineData(RightToLeft.Yes, 0, 0)]
    public void HScrollBarLastPageButtonAccessibleObject_Bounds_ReturnEmptyRectangle(RightToLeft rightToLeft, int maximum, int value)
    {
        using HScrollBar hScrollBar = new();
        SetHScrollBar(hScrollBar, rightToLeft, minimum: 0, maximum, value);

        Assert.True(HLastPageButton(hScrollBar).Bounds.Width == 0 || HLastPageButton(hScrollBar).Bounds.Height == 0);
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.No, 100, 0, AccessibleStates.None)]
    [InlineData(RightToLeft.No, 100, 50, AccessibleStates.None)]
    [InlineData(RightToLeft.No, 100, 100, AccessibleStates.Invisible)]
    [InlineData(RightToLeft.Yes, 100, 0, AccessibleStates.Invisible)]
    [InlineData(RightToLeft.Yes, 100, 50, AccessibleStates.None)]
    [InlineData(RightToLeft.Yes, 100, 100, AccessibleStates.None)]
    [InlineData(RightToLeft.No, 0, 0, AccessibleStates.Invisible)]
    [InlineData(RightToLeft.Yes, 0, 0, AccessibleStates.Invisible)]
    public void HScrollBarLastPageButtonAccessibleObject_State_ReturnExpected(RightToLeft rightToLeft, int maximum, int value, AccessibleStates accessibleState)
    {
        using HScrollBar hScrollBar = new();
        SetHScrollBar(hScrollBar, rightToLeft, minimum: 0, maximum, value);

        Assert.Equal(accessibleState, HLastPageButton(hScrollBar).State);
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.No, 100, 0)]
    [InlineData(RightToLeft.Yes, 100, 0)]
    [InlineData(RightToLeft.No, 0, 0)]
    [InlineData(RightToLeft.Yes, 0, 0)]
    public void VScrollBarFirstPageButtonAccessibleObject_Bounds_ReturnEmptyRectangle(RightToLeft rightToLeft, int maximum, int value)
    {
        using VScrollBar vScrollBar = new();
        SetVScrollBar(vScrollBar, rightToLeft, minimum: 0, maximum, value);

        Assert.True(VFirstPageButton(vScrollBar).Bounds.Width == 0 || VFirstPageButton(vScrollBar).Bounds.Height == 0);
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.No, 0)]
    [InlineData(RightToLeft.No, 50)]
    [InlineData(RightToLeft.Yes, 0)]
    [InlineData(RightToLeft.Yes, 50)]
    public void VScrollBarLastPageButtonAccessibleObject_Bounds_ReturnRectangle(RightToLeft rightToLeft, int value)
    {
        using VScrollBar vScrollBar = new();
        SetVScrollBar(vScrollBar, rightToLeft, minimum: 0, maximum: 100, value);

        Assert.True(VLastPageButton(vScrollBar).Bounds.Width > 0);
        Assert.True(VLastPageButton(vScrollBar).Bounds.Height > 0);
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.No, 100, 100)]
    [InlineData(RightToLeft.Yes, 100, 100)]
    [InlineData(RightToLeft.No, 0, 0)]
    [InlineData(RightToLeft.Yes, 0, 0)]
    public void VScrollBarLastPageButtonAccessibleObject_Bounds_ReturnEmptyRectangle(RightToLeft rightToLeft, int maximum, int value)
    {
        using VScrollBar vScrollBar = new();
        SetVScrollBar(vScrollBar, rightToLeft, minimum: 0, maximum, value);

        Assert.True(VLastPageButton(vScrollBar).Bounds.Width == 0 || VLastPageButton(vScrollBar).Bounds.Height == 0);
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.No, 100, 0, AccessibleStates.None)]
    [InlineData(RightToLeft.No, 100, 50, AccessibleStates.None)]
    [InlineData(RightToLeft.No, 100, 100, AccessibleStates.Invisible)]
    [InlineData(RightToLeft.Yes, 100, 0, AccessibleStates.None)]
    [InlineData(RightToLeft.Yes, 100, 50, AccessibleStates.None)]
    [InlineData(RightToLeft.Yes, 100, 100, AccessibleStates.Invisible)]
    [InlineData(RightToLeft.No, 0, 0, AccessibleStates.Invisible)]
    [InlineData(RightToLeft.Yes, 0, 0, AccessibleStates.Invisible)]
    public void VScrollBarLastPageButtonAccessibleObject_State_ReturnsExpected(RightToLeft rightToLeft, int maximum, int value, AccessibleStates accessibleState)
    {
        using VScrollBar vScrollBar = new();
        SetVScrollBar(vScrollBar, rightToLeft, minimum: 0, maximum, value);

        Assert.Equal(accessibleState, VLastPageButton(vScrollBar).State);
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.No, 50)]
    [InlineData(RightToLeft.Yes, 50)]
    [InlineData(RightToLeft.No, 100)]
    [InlineData(RightToLeft.Yes, 100)]
    public void VScrollBarFirstPageButtonAccessibleObject_Bounds_ReturnRectangle(RightToLeft rightToLeft, int value)
    {
        using VScrollBar vScrollBar = new();
        SetVScrollBar(vScrollBar, rightToLeft, minimum: 0, maximum: 100, value);

        Assert.True(VFirstPageButton(vScrollBar).Bounds.Width > 0);
        Assert.True(VFirstPageButton(vScrollBar).Bounds.Height > 0);
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.No, 100, 0, AccessibleStates.Invisible)]
    [InlineData(RightToLeft.No, 100, 50, AccessibleStates.None)]
    [InlineData(RightToLeft.No, 100, 100, AccessibleStates.None)]
    [InlineData(RightToLeft.Yes, 100, 0, AccessibleStates.Invisible)]
    [InlineData(RightToLeft.Yes, 100, 50, AccessibleStates.None)]
    [InlineData(RightToLeft.Yes, 100, 100, AccessibleStates.None)]
    [InlineData(RightToLeft.No, 0, 0, AccessibleStates.Invisible)]
    [InlineData(RightToLeft.Yes, 0, 0, AccessibleStates.Invisible)]
    public void VScrollBarFirstPageButtonAccessibleObject_State_ReturnsExpected(RightToLeft rightToLeft, int maximum, int value, AccessibleStates accessibleState)
    {
        using VScrollBar vScrollBar = new();
        SetVScrollBar(vScrollBar, rightToLeft, minimum: 0, maximum, value);

        Assert.Equal(accessibleState, VFirstPageButton(vScrollBar).State);
    }

    private void SetVScrollBar(VScrollBar vScrollBar, RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        Form form = new();
        form.Show();
        vScrollBar.RightToLeft = rightToLeft;
        vScrollBar.Minimum = minimum;
        vScrollBar.Maximum = maximum;
        vScrollBar.Value = value;
        vScrollBar.Size = new(50, 200);
        vScrollBar.Parent = form;
        Application.DoEvents();
    }

    private ScrollBar.ScrollBarFirstPageButtonAccessibleObject VFirstPageButton(VScrollBar vScrollBar)
        => ((ScrollBar.ScrollBarAccessibleObject)vScrollBar.AccessibilityObject).FirstPageButtonAccessibleObject;

    private ScrollBar.ScrollBarLastPageButtonAccessibleObject VLastPageButton(VScrollBar vScrollBar)
        => ((ScrollBar.ScrollBarAccessibleObject)vScrollBar.AccessibilityObject).LastPageButtonAccessibleObject;

    private void SetHScrollBar(HScrollBar hScrollBar, RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        Form form = new();
        form.Show();
        hScrollBar.RightToLeft = rightToLeft;
        hScrollBar.Minimum = minimum;
        hScrollBar.Maximum = maximum;
        hScrollBar.Value = value;
        hScrollBar.Size = new(200, 50);
        hScrollBar.Parent = form;
        Application.DoEvents();
    }

    private ScrollBar.ScrollBarFirstPageButtonAccessibleObject HFirstPageButton(HScrollBar hScrollBar)
        => ((ScrollBar.ScrollBarAccessibleObject)hScrollBar.AccessibilityObject).FirstPageButtonAccessibleObject;

    private ScrollBar.ScrollBarLastPageButtonAccessibleObject HLastPageButton(HScrollBar hScrollBar)
        => ((ScrollBar.ScrollBarAccessibleObject)hScrollBar.AccessibilityObject).LastPageButtonAccessibleObject;

    private class VerticalScrollBar : ScrollBar
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= (int)SCROLLBAR_CONSTANTS.SB_VERT;
                return cp;
            }
        }

        public new Rectangle GetScaledBounds(Rectangle bounds, SizeF factor, BoundsSpecified specified) => base.GetScaledBounds(bounds, factor, specified);
    }

    private class SubScrollBar : ScrollBar
    {
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

        public new bool ResizeRedraw
        {
            get => base.ResizeRedraw;
            set => base.ResizeRedraw = value;
        }

        public new bool ShowFocusCues => base.ShowFocusCues;

        public new bool ShowKeyboardCues => base.ShowKeyboardCues;

        public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

        public new Rectangle GetScaledBounds(Rectangle bounds, SizeF factor, BoundsSpecified specified) => base.GetScaledBounds(bounds, factor, specified);

        public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

        public new bool GetTopLevel() => base.GetTopLevel();

        public new void OnClick(EventArgs e) => base.OnClick(e);

        public new void OnDoubleClick(EventArgs e) => base.OnDoubleClick(e);

        public new void OnEnabledChanged(EventArgs e) => base.OnEnabledChanged(e);

        public new void OnHandleCreated(EventArgs e) => base.OnHandleCreated(e);

        public new void OnMouseClick(MouseEventArgs e) => base.OnMouseClick(e);

        public new void OnMouseDoubleClick(MouseEventArgs e) => base.OnMouseDoubleClick(e);

        public new void OnMouseDown(MouseEventArgs e) => base.OnMouseDown(e);

        public new void OnMouseMove(MouseEventArgs e) => base.OnMouseMove(e);

        public new void OnMouseUp(MouseEventArgs e) => base.OnMouseUp(e);

        public new void OnMouseWheel(MouseEventArgs e) => base.OnMouseWheel(e);

        public new void OnPaint(PaintEventArgs e) => base.OnPaint(e);

        public new void OnScroll(ScrollEventArgs se) => base.OnScroll(se);

        public new void OnValueChanged(EventArgs e) => base.OnValueChanged(e);

        public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);

        public new void UpdateScrollInfo() => base.UpdateScrollInfo();

        public new void WndProc(ref Message m) => base.WndProc(ref m);
    }
}
