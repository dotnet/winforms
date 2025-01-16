// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.TestUtilities;
using Moq;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Tests;

public class ProgressBarTests
{
    [WinFormsFact]
    public void ProgressBar_Ctor_Default()
    {
        using SubProgressBar control = new();
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
        Assert.Equal(23, control.Bottom);
        Assert.Equal(new Rectangle(0, 0, 100, 23), control.Bounds);
        Assert.False(control.CanEnableIme);
        Assert.False(control.CanFocus);
        Assert.True(control.CanRaiseEvents);
        Assert.False(control.CanSelect);
        Assert.False(control.Capture);
        Assert.True(control.CausesValidation);
        Assert.Equal(new Rectangle(0, 0, 100, 23), control.ClientRectangle);
        Assert.Equal(new Size(100, 23), control.ClientSize);
        Assert.Null(control.Container);
        Assert.False(control.ContainsFocus);
        Assert.Null(control.ContextMenuStrip);
        Assert.Empty(control.Controls);
        Assert.Same(control.Controls, control.Controls);
        Assert.False(control.Created);
        Assert.Same(Cursors.Default, control.Cursor);
        Assert.Same(Cursors.Default, control.DefaultCursor);
        Assert.Equal(ImeMode.Disable, control.DefaultImeMode);
        Assert.Equal(new Padding(3), control.DefaultMargin);
        Assert.Equal(Size.Empty, control.DefaultMaximumSize);
        Assert.Equal(Size.Empty, control.DefaultMinimumSize);
        Assert.Equal(Padding.Empty, control.DefaultPadding);
        Assert.Equal(new Size(100, 23), control.DefaultSize);
        Assert.False(control.DesignMode);
        Assert.Equal(DockStyle.None, control.Dock);
        Assert.False(control.DoubleBuffered);
        Assert.True(control.Enabled);
        Assert.NotNull(control.Events);
        Assert.Same(control.Events, control.Events);
        Assert.False(control.Focused);
        Assert.Equal(Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.Equal(SystemColors.Highlight, control.ForeColor);
        Assert.False(control.HasChildren);
        Assert.Equal(23, control.Height);
        Assert.Equal(ImeMode.Disable, control.ImeMode);
        Assert.Equal(ImeMode.Disable, control.ImeModeBase);
        Assert.False(control.IsAccessible);
        Assert.False(control.IsMirrored);
        Assert.NotNull(control.LayoutEngine);
        Assert.Same(control.LayoutEngine, control.LayoutEngine);
        Assert.Equal(0, control.Left);
        Assert.Equal(Point.Empty, control.Location);
        Assert.Equal(100, control.MarqueeAnimationSpeed);
        Assert.Equal(100, control.Maximum);
        Assert.Equal(Size.Empty, control.MaximumSize);
        Assert.Equal(0, control.Minimum);
        Assert.Equal(Size.Empty, control.MinimumSize);
        Assert.Equal(Padding.Empty, control.Padding);
        Assert.Equal(new Size(100, 23), control.PreferredSize);
        Assert.False(control.RecreatingHandle);
        Assert.Null(control.Region);
        Assert.False(control.ResizeRedraw);
        Assert.Equal(100, control.Right);
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.False(control.RightToLeftLayout);
        Assert.True(control.ShowFocusCues);
        Assert.True(control.ShowKeyboardCues);
        Assert.Null(control.Site);
        Assert.Equal(new Size(100, 23), control.Size);
        Assert.Equal(10, control.Step);
        Assert.Equal(ProgressBarStyle.Blocks, control.Style);
        Assert.Equal(0, control.TabIndex);
        Assert.True(control.TabStop);
        Assert.Empty(control.Text);
        Assert.Equal(0, control.Top);
        Assert.Null(control.TopLevelControl);
        Assert.False(control.UseWaitCursor);
        Assert.Equal(0, control.Value);
        Assert.True(control.Visible);
        Assert.Equal(100, control.Width);

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ProgressBar_CreateParams_GetDefault_ReturnsExpected()
    {
        using SubProgressBar control = new();
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Equal("msctls_progress32", createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0, createParams.ExStyle);
        Assert.Equal(23, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(0x56010000, createParams.Style);
        Assert.Equal(100, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
    }

    [WinFormsTheory]
    [InlineData(ProgressBarStyle.Blocks, true, 0x56010000)]
    [InlineData(ProgressBarStyle.Continuous, true, 0x56010001)]
    [InlineData(ProgressBarStyle.Marquee, true, 0x56010000)]
    [InlineData(ProgressBarStyle.Blocks, false, 0x56010000)]
    [InlineData(ProgressBarStyle.Continuous, false, 0x56010001)]
    [InlineData(ProgressBarStyle.Marquee, false, 0x56010008)]
    public void ProgressBar_CreateParams_GetStyle_ReturnsExpected(ProgressBarStyle style, bool designMode, int expectedStyle)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(designMode);
        mockSite
            .Setup(s => s.Container)
            .Returns<IContainer>(null);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        using SubProgressBar control = new()
        {
            Style = style,
            Site = mockSite.Object
        };
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Equal("msctls_progress32", createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0, createParams.ExStyle);
        Assert.Equal(23, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(expectedStyle, createParams.Style);
        Assert.Equal(100, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.Inherit, true, 0x56010000, 0x0)]
    [InlineData(RightToLeft.No, true, 0x56010000, 0x0)]
    [InlineData(RightToLeft.Yes, true, 0x56010000, 0x400000)]
    [InlineData(RightToLeft.Inherit, false, 0x56010000, 0x0)]
    [InlineData(RightToLeft.No, false, 0x56010000, 0x0)]
    [InlineData(RightToLeft.Yes, false, 0x56010000, 0x7000)]
    public void ProgressBar_CreateParams_GetRightToLeft_ReturnsExpected(RightToLeft rightToLeft, bool rightToLeftLayout, int expectedStyle, int expectedExStyle)
    {
        using SubProgressBar control = new()
        {
            RightToLeft = rightToLeft,
            RightToLeftLayout = rightToLeftLayout
        };
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Equal("msctls_progress32", createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(expectedExStyle, createParams.ExStyle);
        Assert.Equal(23, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(expectedStyle, createParams.Style);
        Assert.Equal(100, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
    }

    [WinFormsTheory]
    [BoolData]
    public void ProgressBar_AllowDrop_Set_GetReturnsExpected(bool value)
    {
        using ProgressBar control = new()
        {
            AllowDrop = value
        };
        Assert.Equal(value, control.AllowDrop);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AllowDrop = value;
        Assert.Equal(value, control.AllowDrop);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.AllowDrop = !value;
        Assert.Equal(!value, control.AllowDrop);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetImageTheoryData))]
    public void ProgressBar_BackgroundImage_Set_GetReturnsExpected(Image value)
    {
        using ProgressBar control = new()
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
    public void ProgressBar_BackgroundImage_SetWithHandler_CallsBackgroundImageChanged()
    {
        using ProgressBar control = new();
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        }

        control.BackgroundImageChanged += handler;

        // Set different.
        Bitmap image1 = new(10, 10);
        control.BackgroundImage = image1;
        Assert.Same(image1, control.BackgroundImage);
        Assert.Equal(1, callCount);

        // Set same.
        control.BackgroundImage = image1;
        Assert.Same(image1, control.BackgroundImage);
        Assert.Equal(1, callCount);

        // Set different.
        Bitmap image2 = new(10, 10);
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
    public void ProgressBar_BackgroundImageLayout_Set_GetReturnsExpected(ImageLayout value)
    {
        using SubProgressBar control = new()
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
    public void ProgressBar_BackgroundImageLayout_SetWithHandler_CallsBackgroundImageLayoutChanged()
    {
        using ProgressBar control = new();
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

    [WinFormsTheory]
    [InvalidEnumData<ImageLayout>]
    public void ProgressBar_BackgroundImageLayout_SetInvalid_ThrowsInvalidEnumArgumentException(ImageLayout value)
    {
        using ProgressBar control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.BackgroundImageLayout = value);
    }

    [WinFormsTheory]
    [BoolData]
    public void ProgressBar_CausesValidation_Set_GetReturnsExpected(bool value)
    {
        using ProgressBar control = new()
        {
            CausesValidation = value
        };
        Assert.Equal(value, control.CausesValidation);
        Assert.False(control.IsHandleCreated);

        // Set same
        control.CausesValidation = value;
        Assert.Equal(value, control.CausesValidation);
        Assert.False(control.IsHandleCreated);

        // Set different
        control.CausesValidation = !value;
        Assert.Equal(!value, control.CausesValidation);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ProgressBar_CausesValidation_SetWithHandler_CallsCausesValidationChanged()
    {
        using ProgressBar control = new()
        {
            CausesValidation = true
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.CausesValidationChanged += handler;

        // Set different.
        control.CausesValidation = false;
        Assert.False(control.CausesValidation);
        Assert.Equal(1, callCount);

        // Set same.
        control.CausesValidation = false;
        Assert.False(control.CausesValidation);
        Assert.Equal(1, callCount);

        // Set different.
        control.CausesValidation = true;
        Assert.True(control.CausesValidation);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.CausesValidationChanged -= handler;
        control.CausesValidation = false;
        Assert.False(control.CausesValidation);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void ProgressBar_DoubleBuffered_Get_ReturnsExpected(bool value)
    {
        using SubProgressBar control = new();
        control.SetStyle(ControlStyles.OptimizedDoubleBuffer, value);
        Assert.Equal(value, control.DoubleBuffered);
    }

    [WinFormsTheory]
    [BoolData]
    public void ProgressBar_DoubleBuffered_Set_GetReturnsExpected(bool value)
    {
        using SubProgressBar control = new()
        {
            DoubleBuffered = value
        };
        Assert.Equal(value, control.DoubleBuffered);
        Assert.Equal(value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.DoubleBuffered = value;
        Assert.Equal(value, control.DoubleBuffered);
        Assert.Equal(value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.DoubleBuffered = !value;
        Assert.Equal(!value, control.DoubleBuffered);
        Assert.Equal(!value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ProgressBar_DoubleBuffered_SetWithHandle_GetReturnsExpected(bool value)
    {
        using SubProgressBar control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.DoubleBuffered = value;
        Assert.Equal(value, control.DoubleBuffered);
        Assert.Equal(value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.DoubleBuffered = value;
        Assert.Equal(value, control.DoubleBuffered);
        Assert.Equal(value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.DoubleBuffered = !value;
        Assert.Equal(!value, control.DoubleBuffered);
        Assert.Equal(!value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetFontTheoryData))]
    public void ProgressBar_Font_Set_GetReturnsExpected(Font value)
    {
        using SubProgressBar control = new()
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
    public void ProgressBar_Font_SetWithHandler_CallsFontChanged()
    {
        using ProgressBar control = new();
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
    public void ProgressBar_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using ProgressBar control = new()
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

    public static IEnumerable<object[]> ForeColor_SetWithHandle_TestData()
    {
        yield return new object[] { Color.Red, Color.Red, 1 };
        yield return new object[] { Color.FromArgb(254, 1, 2, 3), Color.FromArgb(254, 1, 2, 3), 1 };
        yield return new object[] { Color.Empty, Control.DefaultForeColor, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(ForeColor_SetWithHandle_TestData))]
    public void ProgressBar_ForeColor_SetWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
    {
        using ProgressBar control = new();
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
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.ForeColor = value;
        Assert.Equal(expected, control.ForeColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ProgressBar_ForeColor_SetWithHandler_CallsForeColorChanged()
    {
        using ProgressBar control = new();
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

    [WinFormsFact]
    public void ProgressBar_ForeColor_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.ForeColor)];
        using ProgressBar control = new();
        Assert.False(property.CanResetValue(control));

        control.ForeColor = Color.Red;
        Assert.Equal(Color.Red, control.ForeColor);
        Assert.True(property.CanResetValue(control));

        property.ResetValue(control);
        Assert.Equal(SystemColors.Highlight, control.ForeColor);
        Assert.False(property.CanResetValue(control));
    }

    [WinFormsFact]
    public void ProgressBar_ForeColor_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.ForeColor)];
        using ProgressBar control = new();
        Assert.False(property.ShouldSerializeValue(control));

        control.ForeColor = Color.Red;
        Assert.Equal(Color.Red, control.ForeColor);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.Equal(SystemColors.Highlight, control.ForeColor);
        Assert.False(property.ShouldSerializeValue(control));
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
    public void ProgressBar_ImeMode_Set_GetReturnsExpected(ImeMode value, ImeMode expected)
    {
        using ProgressBar control = new()
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
    public void ProgressBar_ImeMode_SetWithHandle_GetReturnsExpected(ImeMode value, ImeMode expected)
    {
        using ProgressBar control = new();
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
    public void ProgressBar_ImeMode_SetWithHandler_CallsImeModeChanged()
    {
        using ProgressBar control = new();
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
    public void ProgressBar_ImeMode_SetInvalid_ThrowsInvalidEnumArgumentException(ImeMode value)
    {
        using ProgressBar control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.ImeMode = value);
    }

    public static IEnumerable<object[]> MarqueeAnimationSpeed_Set_TestData()
    {
        foreach (ProgressBarStyle style in Enum.GetValues(typeof(ProgressBarStyle)))
        {
            yield return new object[] { style, 0 };
            yield return new object[] { style, 1 };
            yield return new object[] { style, 100 };
            yield return new object[] { style, int.MaxValue };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(MarqueeAnimationSpeed_Set_TestData))]
    public void ProgressBar_MarqueeAnimationSpeed_Set_GetReturnsExpected(ProgressBarStyle style, int value)
    {
        using ProgressBar control = new()
        {
            Style = style,
            MarqueeAnimationSpeed = value
        };
        Assert.Equal(value, control.MarqueeAnimationSpeed);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.MarqueeAnimationSpeed = value;
        Assert.Equal(value, control.MarqueeAnimationSpeed);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(MarqueeAnimationSpeed_Set_TestData))]
    public void ProgressBar_MarqueeAnimationSpeed_SetDesignMode_GetReturnsExpected(ProgressBarStyle style, int value)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Container)
            .Returns<IContainer>(null);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        using ProgressBar control = new()
        {
            Site = mockSite.Object,
            Style = style,
            MarqueeAnimationSpeed = value
        };
        Assert.Equal(value, control.MarqueeAnimationSpeed);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.MarqueeAnimationSpeed = value;
        Assert.Equal(value, control.MarqueeAnimationSpeed);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(MarqueeAnimationSpeed_Set_TestData))]
    public void ProgressBar_MarqueeAnimationSpeed_SetWithHandle_GetReturnsExpected(ProgressBarStyle style, int value)
    {
        using ProgressBar control = new()
        {
            Style = style
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.MarqueeAnimationSpeed = value;
        Assert.Equal(value, control.MarqueeAnimationSpeed);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.MarqueeAnimationSpeed = value;
        Assert.Equal(value, control.MarqueeAnimationSpeed);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(MarqueeAnimationSpeed_Set_TestData))]
    public void ProgressBar_MarqueeAnimationSpeed_SetWithHandleDesignMode_GetReturnsExpected(ProgressBarStyle style, int value)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Container)
            .Returns<IContainer>(null);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        using ProgressBar control = new()
        {
            Site = mockSite.Object,
            Style = style
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.MarqueeAnimationSpeed = value;
        Assert.Equal(value, control.MarqueeAnimationSpeed);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.MarqueeAnimationSpeed = value;
        Assert.Equal(value, control.MarqueeAnimationSpeed);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ProgressBar_MarqueeAnimationSpeed_SetNegative_ThrowsArgumentOutOfRangeException()
    {
        using ProgressBar control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.MarqueeAnimationSpeed = -1);
    }

    public static IEnumerable<object[]> Maximum_Set_TestData()
    {
        yield return new object[] { 0 };
        yield return new object[] { 1 };
        yield return new object[] { 100 };
        yield return new object[] { int.MaxValue };
    }

    [WinFormsTheory]
    [MemberData(nameof(Maximum_Set_TestData))]
    public void ProgressBar_Maximum_Set_GetReturnsExpected(int value)
    {
        using ProgressBar control = new()
        {
            Maximum = value
        };
        Assert.Equal(0, control.Minimum);
        Assert.Equal(value, control.Maximum);
        Assert.Equal(0, control.Value);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Maximum = value;
        Assert.Equal(0, control.Minimum);
        Assert.Equal(value, control.Maximum);
        Assert.Equal(0, control.Value);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(4)]
    [InlineData(0)]
    public void ProgressBar_Maximum_SetLessThanMinimum_SetsMinimumToMaximum(int value)
    {
        using ProgressBar control = new()
        {
            Minimum = 5,
            Maximum = value
        };
        Assert.Equal(value, control.Minimum);
        Assert.Equal(value, control.Maximum);
        Assert.Equal(value, control.Value);
    }

    [WinFormsTheory]
    [InlineData(4)]
    [InlineData(0)]
    public void ProgressBar_Maximum_SetLessThanValue_SetsMinimumToValue(int value)
    {
        using ProgressBar control = new()
        {
            Value = 5,
            Maximum = value
        };
        Assert.Equal(0, control.Minimum);
        Assert.Equal(value, control.Maximum);
        Assert.Equal(value, control.Value);
    }

    [WinFormsTheory]
    [MemberData(nameof(Maximum_Set_TestData))]
    public void ProgressBar_Maximum_SetWithHandle_GetReturnsExpected(int value)
    {
        using ProgressBar control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Maximum = value;
        Assert.Equal(0, control.Minimum);
        Assert.Equal(value, control.Maximum);
        Assert.Equal(0, control.Value);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Maximum = value;
        Assert.Equal(0, control.Minimum);
        Assert.Equal(value, control.Maximum);
        Assert.Equal(0, control.Value);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ProgressBar_Maximum_SetNegative_ThrowsArgumentOutOfRangeException()
    {
        using ProgressBar control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.Maximum = -1);
    }

    public static IEnumerable<object[]> Minimum_Set_TestData()
    {
        yield return new object[] { 0 };
        yield return new object[] { 5 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Minimum_Set_TestData))]
    public void ProgressBar_Minimum_Set_GetReturnsExpected(int value)
    {
        using ProgressBar control = new()
        {
            Value = 5,
            Minimum = value
        };
        Assert.Equal(value, control.Minimum);
        Assert.Equal(100, control.Maximum);
        Assert.Equal(5, control.Value);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Minimum = value;
        Assert.Equal(value, control.Minimum);
        Assert.Equal(100, control.Maximum);
        Assert.Equal(5, control.Value);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(101)]
    [InlineData(int.MaxValue)]
    public void ProgressBar_Minimum_SetGreaterThanMaximum_SetsMaximumToMinimum(int value)
    {
        using ProgressBar control = new()
        {
            Minimum = value
        };
        Assert.Equal(value, control.Minimum);
        Assert.Equal(value, control.Maximum);
        Assert.Equal(value, control.Value);
    }

    [WinFormsTheory]
    [InlineData(6)]
    public void ProgressBar_Minimum_SetGreaterThanValue_SetsMinimumToValue(int value)
    {
        using ProgressBar control = new()
        {
            Value = 5,
            Minimum = value
        };
        Assert.Equal(value, control.Minimum);
        Assert.Equal(100, control.Maximum);
        Assert.Equal(value, control.Value);
    }

    [WinFormsTheory]
    [MemberData(nameof(Minimum_Set_TestData))]
    public void ProgressBar_Minimum_SetWithHandle_GetReturnsExpected(int value)
    {
        using ProgressBar control = new()
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
        Assert.Equal(value, control.Minimum);
        Assert.Equal(100, control.Maximum);
        Assert.Equal(5, control.Value);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Minimum = value;
        Assert.Equal(value, control.Minimum);
        Assert.Equal(100, control.Maximum);
        Assert.Equal(5, control.Value);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ProgressBar_Minimum_SetNegative_ThrowsArgumentOutOfRangeException()
    {
        using ProgressBar control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.Minimum = -1);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaddingNormalizedTheoryData))]
    public void ProgressBar_Padding_Set_GetReturnsExpected(Padding value, Padding expected)
    {
        using ProgressBar control = new()
        {
            Padding = value
        };
        Assert.Equal(expected, control.Padding);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Padding = value;
        Assert.Equal(expected, control.Padding);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaddingNormalizedTheoryData))]
    public void ProgressBar_Padding_SetWithHandle_GetReturnsExpected(Padding value, Padding expected)
    {
        using ProgressBar control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Padding = value;
        Assert.Equal(expected, control.Padding);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Padding = value;
        Assert.Equal(expected, control.Padding);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ProgressBar_Padding_SetWithHandler_CallsPaddingChanged()
    {
        using ProgressBar control = new();
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
    [InlineData(RightToLeft.Yes, true, 1)]
    [InlineData(RightToLeft.Yes, false, 0)]
    [InlineData(RightToLeft.No, true, 1)]
    [InlineData(RightToLeft.No, false, 0)]
    [InlineData(RightToLeft.Inherit, true, 1)]
    [InlineData(RightToLeft.Inherit, false, 0)]
    public void ProgressBar_RightToLeftLayout_Set_GetReturnsExpected(RightToLeft rightToLeft, bool value, int expectedLayoutCallCount)
    {
        using ProgressBar control = new()
        {
            RightToLeft = rightToLeft
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("RightToLeftLayout", e.AffectedProperty);
            layoutCallCount++;
        };

        control.RightToLeftLayout = value;
        Assert.Equal(value, control.RightToLeftLayout);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.RightToLeftLayout = value;
        Assert.Equal(value, control.RightToLeftLayout);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.RightToLeftLayout = !value;
        Assert.Equal(!value, control.RightToLeftLayout);
        Assert.Equal(expectedLayoutCallCount + 1, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.Yes, true, 1, 1, 2)]
    [InlineData(RightToLeft.Yes, false, 0, 0, 1)]
    [InlineData(RightToLeft.No, true, 1, 0, 0)]
    [InlineData(RightToLeft.No, false, 0, 0, 0)]
    [InlineData(RightToLeft.Inherit, true, 1, 0, 0)]
    [InlineData(RightToLeft.Inherit, false, 0, 0, 0)]
    public void ProgressBar_RightToLeftLayout_SetWithHandle_GetReturnsExpected(RightToLeft rightToLeft, bool value, int expectedLayoutCallCount, int expectedCreatedCallCount1, int expectedCreatedCallCount2)
    {
        using ProgressBar control = new()
        {
            RightToLeft = rightToLeft
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
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("RightToLeftLayout", e.AffectedProperty);
            layoutCallCount++;
        };

        control.RightToLeftLayout = value;
        Assert.Equal(value, control.RightToLeftLayout);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount1, createdCallCount);

        // Set same.
        control.RightToLeftLayout = value;
        Assert.Equal(value, control.RightToLeftLayout);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount1, createdCallCount);

        // Set different.
        control.RightToLeftLayout = !value;
        Assert.Equal(!value, control.RightToLeftLayout);
        Assert.Equal(expectedLayoutCallCount + 1, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount2, createdCallCount);
    }

    [WinFormsFact]
    public void ProgressBar_RightToLeftLayout_SetWithHandler_CallsRightToLeftLayoutChanged()
    {
        using ProgressBar control = new()
        {
            RightToLeftLayout = true
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.RightToLeftLayoutChanged += handler;

        // Set different.
        control.RightToLeftLayout = false;
        Assert.False(control.RightToLeftLayout);
        Assert.Equal(1, callCount);

        // Set same.
        control.RightToLeftLayout = false;
        Assert.False(control.RightToLeftLayout);
        Assert.Equal(1, callCount);

        // Set different.
        control.RightToLeftLayout = true;
        Assert.True(control.RightToLeftLayout);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.RightToLeftLayoutChanged -= handler;
        control.RightToLeftLayout = false;
        Assert.False(control.RightToLeftLayout);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void ProgressBar_RightToLeftLayout_SetWithHandlerInDisposing_DoesNotRightToLeftLayoutChanged()
    {
        using ProgressBar control = new()
        {
            RightToLeft = RightToLeft.Yes
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        int callCount = 0;
        control.RightToLeftLayoutChanged += (sender, e) => callCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int disposedCallCount = 0;
        control.Disposed += (sender, e) =>
        {
            control.RightToLeftLayout = true;
            Assert.True(control.RightToLeftLayout);
            Assert.Equal(0, callCount);
            Assert.Equal(0, createdCallCount);
            disposedCallCount++;
        };

        control.Dispose();
        Assert.Equal(1, disposedCallCount);
    }

    public static IEnumerable<object[]> Step_Set_TestData()
    {
        yield return new object[] { int.MinValue };
        yield return new object[] { 0 };
        yield return new object[] { 5 };
        yield return new object[] { int.MaxValue };
    }

    [WinFormsTheory]
    [MemberData(nameof(Step_Set_TestData))]
    public void ProgressBar_Step_Set_GetReturnsExpected(int value)
    {
        using ProgressBar control = new()
        {
            Step = value
        };
        Assert.Equal(value, control.Step);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Step = value;
        Assert.Equal(value, control.Step);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Step_Set_TestData))]
    public void ProgressBar_Step_SetWithHandle_GetReturnsExpected(int value)
    {
        using ProgressBar control = new()
        {
            Step = 5
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Step = value;
        Assert.Equal(value, control.Step);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Step = value;
        Assert.Equal(value, control.Step);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [EnumData<ProgressBarStyle>]
    public void ProgressBar_Style_Set_GetReturnsExpected(ProgressBarStyle value)
    {
        using ProgressBar control = new()
        {
            Style = value
        };
        Assert.Equal(value, control.Style);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Style = value;
        Assert.Equal(value, control.Style);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [EnumData<ProgressBarStyle>]
    public void ProgressBar_Style_SetZeroMarqueeAnimationSpeed_GetReturnsExpected(ProgressBarStyle value)
    {
        using ProgressBar control = new()
        {
            MarqueeAnimationSpeed = 0,
            Style = value
        };
        Assert.Equal(value, control.Style);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Style = value;
        Assert.Equal(value, control.Style);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(ProgressBarStyle.Blocks, 0)]
    [InlineData(ProgressBarStyle.Continuous, 1)]
    [InlineData(ProgressBarStyle.Marquee, 1)]
    public void ProgressBar_Style_SetWithHandle_GetReturnsExpected(ProgressBarStyle value, int expectedCreatedCallCount)
    {
        using ProgressBar control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Style = value;
        Assert.Equal(value, control.Style);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        control.Style = value;
        Assert.Equal(value, control.Style);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(ProgressBarStyle.Blocks, 0)]
    [InlineData(ProgressBarStyle.Continuous, 1)]
    [InlineData(ProgressBarStyle.Marquee, 1)]
    public void ProgressBar_Style_SetWithHandleZeroMarqueeAnimationSpeed_GetReturnsExpected(ProgressBarStyle value, int expectedCreatedCallCount)
    {
        using ProgressBar control = new()
        {
            MarqueeAnimationSpeed = 0
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Style = value;
        Assert.Equal(value, control.Style);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        control.Style = value;
        Assert.Equal(value, control.Style);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<ProgressBarStyle>]
    public void ProgressBar_Style_SetInvalidValue_ThrowsInvalidEnumArgumentException(ProgressBarStyle value)
    {
        using ProgressBar control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.Style = value);
    }

    [WinFormsTheory]
    [BoolData]
    public void ProgressBar_TabStop_Set_GetReturnsExpected(bool value)
    {
        using ProgressBar control = new()
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
    public void ProgressBar_TabStop_SetWithHandle_GetReturnsExpected(bool value)
    {
        using ProgressBar control = new();
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
    public void ProgressBar_TabStop_SetWithHandler_CallsTabStopChanged()
    {
        using ProgressBar control = new()
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
    public void ProgressBar_Text_Set_GetReturnsExpected(string value, string expected)
    {
        using ProgressBar control = new()
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
    public void ProgressBar_Text_SetWithHandle_GetReturnsExpected(string value, string expected)
    {
        using ProgressBar control = new();
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
    public void ProgressBar_Text_SetWithHandler_CallsTextChanged()
    {
        using ProgressBar control = new();
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

    public static IEnumerable<object[]> Value_Set_TestData()
    {
        yield return new object[] { 0 };
        yield return new object[] { 5 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Value_Set_TestData))]
    public void ProgressBar_Value_Set_GetReturnsExpected(int value)
    {
        using ProgressBar control = new()
        {
            Value = value
        };
        Assert.Equal(0, control.Minimum);
        Assert.Equal(100, control.Maximum);
        Assert.Equal(value, control.Value);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Value = value;
        Assert.Equal(0, control.Minimum);
        Assert.Equal(100, control.Maximum);
        Assert.Equal(value, control.Value);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Value_Set_TestData))]
    public void ProgressBar_Value_SetWithHandle_GetReturnsExpected(int value)
    {
        using ProgressBar control = new()
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

        control.Value = value;
        Assert.Equal(0, control.Minimum);
        Assert.Equal(100, control.Maximum);
        Assert.Equal(value, control.Value);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Value = value;
        Assert.Equal(0, control.Minimum);
        Assert.Equal(100, control.Maximum);
        Assert.Equal(value, control.Value);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(101)]
    public void ProgressBar_Value_SetInvalid_ThrowsArgumentOutOfRangeException(int value)
    {
        using ProgressBar control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.Value = value);
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleRole.ProgressBar)]
    [InlineData(false, AccessibleRole.None)]
    public void ProgressBar_CreateAccessibilityInstance_Invoke_ReturnsExpected(bool createControl, AccessibleRole expectedAccessibleRole)
    {
        using SubProgressBar control = new();
        if (createControl)
        {
            control.CreateControl();
        }

        Assert.Equal(createControl, control.IsHandleCreated);
        Control.ControlAccessibleObject instance = Assert.IsAssignableFrom<Control.ControlAccessibleObject>(control.CreateAccessibilityInstance());
        Assert.Equal(createControl, control.IsHandleCreated);
        Assert.NotNull(instance);
        Assert.Same(control, instance.Owner);
        Assert.Equal(expectedAccessibleRole, instance.Role);
        Assert.NotSame(control.CreateAccessibilityInstance(), instance);
        Assert.NotSame(control.AccessibilityObject, instance);
    }

    [WinFormsFact]
    public void ProgressBar_CreateHandle_Invoke_Success()
    {
        using SubProgressBar control = new();
        control.CreateHandle();
        Assert.True(control.Created);
        Assert.True(control.IsHandleCreated);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
    }

    [WinFormsFact]
    public void ProgressBar_GetAutoSizeMode_Invoke_ReturnsExpected()
    {
        using SubProgressBar control = new();
        Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
    }

    [WinFormsTheory]
    [InlineData(ControlStyles.ContainerControl, false)]
    [InlineData(ControlStyles.UserPaint, false)]
    [InlineData(ControlStyles.Opaque, false)]
    [InlineData(ControlStyles.ResizeRedraw, false)]
    [InlineData(ControlStyles.FixedWidth, false)]
    [InlineData(ControlStyles.FixedHeight, false)]
    [InlineData(ControlStyles.StandardClick, true)]
    [InlineData(ControlStyles.Selectable, false)]
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
    public void ProgressBar_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
    {
        using SubProgressBar control = new();
        Assert.Equal(expected, control.GetStyle(flag));

        // Call again to test caching.
        Assert.Equal(expected, control.GetStyle(flag));
    }

    [WinFormsFact]
    public void ProgressBar_GetTopLevel_Invoke_ReturnsExpected()
    {
        using SubProgressBar control = new();
        Assert.False(control.GetTopLevel());
    }

    public static IEnumerable<object[]> Increment_TestData()
    {
        foreach (ProgressBarStyle style in new ProgressBarStyle[] { ProgressBarStyle.Blocks, ProgressBarStyle.Continuous })
        {
            yield return new object[] { style, 0, 1, 1 };
            yield return new object[] { style, 0, 100, 100 };
            yield return new object[] { style, 0, 101, 100 };
            yield return new object[] { style, 0, 0, 0 };
            yield return new object[] { style, 0, -1, 0 };
            yield return new object[] { style, 100, -1, 99 };
            yield return new object[] { style, 100, -100, 0 };
            yield return new object[] { style, 100, -101, 0 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Increment_TestData))]
    public void ProgressBar_Increment_Invoke_Success(ProgressBarStyle style, int originalValue, int value, int expectedValue)
    {
        using ProgressBar control = new()
        {
            Style = style,
            Value = originalValue
        };
        control.Increment(value);
        Assert.Equal(expectedValue, control.Value);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Increment_TestData))]
    public void ProgressBar_Increment_InvokeWithHandle_Success(ProgressBarStyle style, int originalValue, int value, int expectedValue)
    {
        using ProgressBar control = new()
        {
            Style = style,
            Value = originalValue
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Increment(value);
        Assert.Equal(expectedValue, control.Value);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ProgressBar_Increment_Marquee_ThrowsInvalidOperationException(int value)
    {
        using ProgressBar control = new()
        {
            Style = ProgressBarStyle.Marquee
        };
        Assert.Throws<InvalidOperationException>(() => control.Increment(value));
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ProgressBar_OnBackColorChanged_Invoke_CallsBackColorChanged(EventArgs eventArgs)
    {
        using SubProgressBar control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.BackColorChanged += handler;
        control.OnBackColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.BackColorChanged -= handler;
        control.OnBackColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ProgressBar_OnBackColorChanged_InvokeWithHandle_CallsBackColorChanged(EventArgs eventArgs)
    {
        using SubProgressBar control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };
        int invalidatedCallCount = 0;
        InvalidateEventHandler invalidatedHandler = (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        EventHandler styleChangedHandler = (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        EventHandler createdHandler = (sender, e) => createdCallCount++;

        // Call with handler.
        control.BackColorChanged += handler;
        control.Invalidated += invalidatedHandler;
        control.StyleChanged += styleChangedHandler;
        control.HandleCreated += createdHandler;
        control.OnBackColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(control.IsHandleCreated);

        // Remove handler.
        control.BackColorChanged -= handler;
        control.Invalidated -= invalidatedHandler;
        control.StyleChanged -= styleChangedHandler;
        control.HandleCreated -= createdHandler;
        control.OnBackColorChanged(eventArgs);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void MonthControl_OnDoubleClick_Invoke_CallsDoubleClick(EventArgs eventArgs)
    {
        using SubProgressBar control = new();
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
    public void ProgressBar_OnEnter_Invoke_CallsEnter(EventArgs eventArgs)
    {
        using SubProgressBar control = new();
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

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ProgressBar_OnForeColorChanged_Invoke_CallsForeColorChanged(EventArgs eventArgs)
    {
        using SubProgressBar control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.ForeColorChanged += handler;
        control.OnForeColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.ForeColorChanged -= handler;
        control.OnForeColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ProgressBar_OnForeColorChanged_InvokeWithHandle_CallsForeColorChanged(EventArgs eventArgs)
    {
        using SubProgressBar control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };
        int invalidatedCallCount = 0;
        InvalidateEventHandler invalidatedHandler = (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        EventHandler styleChangedHandler = (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        EventHandler createdHandler = (sender, e) => createdCallCount++;

        // Call with handler.
        control.ForeColorChanged += handler;
        control.Invalidated += invalidatedHandler;
        control.StyleChanged += styleChangedHandler;
        control.HandleCreated += createdHandler;
        control.OnForeColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(control.IsHandleCreated);

        // Remove handler.
        control.ForeColorChanged -= handler;
        control.Invalidated -= invalidatedHandler;
        control.StyleChanged -= styleChangedHandler;
        control.HandleCreated -= createdHandler;
        control.OnForeColorChanged(eventArgs);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> OnHandleCreated_TestData()
    {
        foreach (ProgressBarStyle style in Enum.GetValues(typeof(ProgressBarStyle)))
        {
            foreach (EventArgs testData in CommonTestHelper.GetEventArgsTheoryData())
            {
                yield return new object[] { style, testData };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnHandleCreated_TestData))]
    public void ProgressBar_OnHandleCreated_Invoke_CallsHandleCreated(ProgressBarStyle style, EventArgs eventArgs)
    {
        using SubProgressBar control = new()
        {
            Style = style
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
    public void ProgressBar_OnHandleCreated_InvokeWithHandle_CallsHandleCreated(ProgressBarStyle style, EventArgs eventArgs)
    {
        using SubProgressBar control = new()
        {
            Style = style
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.False(control.GetStyle(ControlStyles.UserPaint));

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
    public void ProgressBar_OnHandleDestroyed_Invoke_CallsHandleDestroyed(EventArgs eventArgs)
    {
        using SubProgressBar control = new();
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
    public void ProgressBar_OnHandleDestroyed_InvokeWithHandle_CallsHandleDestroyed(EventArgs eventArgs)
    {
        using SubProgressBar control = new();
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

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetKeyEventArgsTheoryData))]
    public void ProgressBar_OnKeyDown_Invoke_CallsKeyDown(KeyEventArgs eventArgs)
    {
        using SubProgressBar control = new();
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
    public void ProgressBar_OnKeyPress_Invoke_CallsKeyPress(KeyPressEventArgs eventArgs)
    {
        using SubProgressBar control = new();
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
    public void ProgressBar_OnKeyUp_Invoke_CallsKeyUp(KeyEventArgs eventArgs)
    {
        using SubProgressBar control = new();
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

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ProgressBar_OnLeave_Invoke_CallsLeave(EventArgs eventArgs)
    {
        using SubProgressBar control = new();
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

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetMouseEventArgsTheoryData))]
    public void ProgressBar_OnMouseDoubleClick_Invoke_CallsMouseDoubleClick(MouseEventArgs eventArgs)
    {
        using SubProgressBar control = new();
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
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaintEventArgsTheoryData))]
    public void ProgressBar_OnPaint_Invoke_CallsPaint(PaintEventArgs eventArgs)
    {
        using SubProgressBar control = new();
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

    public static IEnumerable<object[]> OnRightToLeftLayoutChanged_TestData()
    {
        yield return new object[] { RightToLeft.Yes, null };
        yield return new object[] { RightToLeft.Yes, new EventArgs() };
        yield return new object[] { RightToLeft.No, null };
        yield return new object[] { RightToLeft.No, new EventArgs() };
        yield return new object[] { RightToLeft.Inherit, null };
        yield return new object[] { RightToLeft.Inherit, new EventArgs() };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnRightToLeftLayoutChanged_TestData))]
    public void ProgressBar_OnRightToLeftLayoutChanged_Invoke_CallsRightToLeftLayoutChanged(RightToLeft rightToLeft, EventArgs eventArgs)
    {
        using SubProgressBar control = new()
        {
            RightToLeft = rightToLeft
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.RightToLeftLayoutChanged += handler;
        control.OnRightToLeftLayoutChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.RightToLeftLayoutChanged -= handler;
        control.OnRightToLeftLayoutChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> OnRightToLeftLayoutChanged_WithHandle_TestData()
    {
        yield return new object[] { RightToLeft.Yes, null, 1 };
        yield return new object[] { RightToLeft.Yes, new EventArgs(), 1 };
        yield return new object[] { RightToLeft.No, null, 0 };
        yield return new object[] { RightToLeft.No, new EventArgs(), 0 };
        yield return new object[] { RightToLeft.Inherit, null, 0 };
        yield return new object[] { RightToLeft.Inherit, new EventArgs(), 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnRightToLeftLayoutChanged_WithHandle_TestData))]
    public void ProgressBar_OnRightToLeftLayoutChanged_InvokeWithHandle_CallsRightToLeftLayoutChanged(RightToLeft rightToLeft, EventArgs eventArgs, int expectedCreatedCallCount)
    {
        using SubProgressBar control = new()
        {
            RightToLeft = rightToLeft
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
        control.RightToLeftLayoutChanged += handler;
        control.OnRightToLeftLayoutChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Remove handler.
        control.RightToLeftLayoutChanged -= handler;
        control.OnRightToLeftLayoutChanged(eventArgs);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount * 2, createdCallCount);
    }

    [WinFormsFact]
    public void ProgressBar_OnRightToLeftLayoutChanged_InvokeInDisposing_DoesNotCallRightToLeftLayoutChanged()
    {
        using SubProgressBar control = new()
        {
            RightToLeft = RightToLeft.Yes
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        int callCount = 0;
        control.RightToLeftLayoutChanged += (sender, e) => callCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int disposedCallCount = 0;
        control.Disposed += (sender, e) =>
        {
            control.OnRightToLeftLayoutChanged(EventArgs.Empty);
            Assert.Equal(0, callCount);
            Assert.Equal(0, createdCallCount);
            disposedCallCount++;
        };

        control.Dispose();
        Assert.Equal(1, disposedCallCount);
    }

    public static IEnumerable<object[]> PerformStep_TestData()
    {
        foreach (ProgressBarStyle style in new ProgressBarStyle[] { ProgressBarStyle.Blocks, ProgressBarStyle.Continuous })
        {
            yield return new object[] { style, 0, 1, 1 };
            yield return new object[] { style, 0, 100, 100 };
            yield return new object[] { style, 0, 101, 100 };
            yield return new object[] { style, 0, 0, 0 };
            yield return new object[] { style, 0, -1, 0 };
            yield return new object[] { style, 100, -1, 99 };
            yield return new object[] { style, 100, -100, 0 };
            yield return new object[] { style, 100, -101, 0 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(PerformStep_TestData))]
    public void ProgressBar_PerformStep_Invoke_Success(ProgressBarStyle style, int originalValue, int step, int expectedValue)
    {
        using ProgressBar control = new()
        {
            Style = style,
            Value = originalValue,
            Step = step
        };
        control.PerformStep();
        Assert.Equal(expectedValue, control.Value);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(PerformStep_TestData))]
    public void ProgressBar_PerformStep_InvokeWithHandle_Success(ProgressBarStyle style, int originalValue, int step, int expectedValue)
    {
        using ProgressBar control = new()
        {
            Style = style,
            Value = originalValue,
            Step = step
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.PerformStep();
        Assert.Equal(expectedValue, control.Value);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ProgressBar_PerformStep_Marquee_ThrowsInvalidOperationException(int step)
    {
        using ProgressBar control = new()
        {
            Style = ProgressBarStyle.Marquee,
            Step = step
        };
        Assert.Throws<InvalidOperationException>(control.PerformStep);
    }

    [WinFormsFact]
    public void ProgressBar_ResetForeColor_Invoke_Success()
    {
        using ProgressBar control = new();

        // Reset without value.
        control.ResetForeColor();
        Assert.Equal(SystemColors.Highlight, control.ForeColor);

        // Reset with value.
        control.ForeColor = Color.Black;
        control.ResetForeColor();
        Assert.Equal(SystemColors.Highlight, control.ForeColor);

        // Reset again.
        control.ResetForeColor();
        Assert.Equal(SystemColors.Highlight, control.ForeColor);
    }

    [WinFormsFact]
    public void ProgressBar_ToString_Invoke_ReturnsExpected()
    {
        using ProgressBar control = new();
        Assert.Equal("System.Windows.Forms.ProgressBar, Minimum: 0, Maximum: 100, Value: 0", control.ToString());
    }

    public class SubProgressBar : ProgressBar
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

        public new bool IsHandleCreated => base.IsHandleCreated;

        public new bool ResizeRedraw
        {
            get => base.ResizeRedraw;
            set => base.ResizeRedraw = value;
        }

        public new bool ShowFocusCues => base.ShowFocusCues;

        public new bool ShowKeyboardCues => base.ShowKeyboardCues;

        public new AccessibleObject CreateAccessibilityInstance() => base.CreateAccessibilityInstance();

        public new void CreateHandle() => base.CreateHandle();

        public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

        public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

        public new bool GetTopLevel() => base.GetTopLevel();

        public new void OnBackColorChanged(EventArgs e) => base.OnBackColorChanged(e);

        public new void OnDoubleClick(EventArgs e) => base.OnDoubleClick(e);

        public new void OnEnter(EventArgs e) => base.OnEnter(e);

        public new void OnForeColorChanged(EventArgs e) => base.OnForeColorChanged(e);

        public new void OnHandleCreated(EventArgs e) => base.OnHandleCreated(e);

        public new void OnHandleDestroyed(EventArgs e) => base.OnHandleDestroyed(e);

        public new void OnKeyDown(KeyEventArgs e) => base.OnKeyDown(e);

        public new void OnKeyPress(KeyPressEventArgs e) => base.OnKeyPress(e);

        public new void OnKeyUp(KeyEventArgs e) => base.OnKeyUp(e);

        public new void OnLeave(EventArgs e) => base.OnLeave(e);

        public new void OnMouseDoubleClick(MouseEventArgs e) => base.OnMouseDoubleClick(e);

        public new void OnPaint(PaintEventArgs e) => base.OnPaint(e);

        public new void OnRightToLeftLayoutChanged(EventArgs e) => base.OnRightToLeftLayoutChanged(e);

        public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);
    }
}
