// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.TestUtilities;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Tests;

public class TrackBarTests
{
    public static readonly int s_dimension = (SystemInformation.HorizontalScrollBarHeight * 8) / 3;

    [WinFormsFact]
    public void TrackBar_Ctor_Default()
    {
        using SubTrackBar control = new();
        Assert.Null(control.AccessibleDefaultActionDescription);
        Assert.Null(control.AccessibleDescription);
        Assert.Null(control.AccessibleName);
        Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
        Assert.False(control.AllowDrop);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        Assert.True(control.AutoSize);
        Assert.Equal(Control.DefaultBackColor, control.BackColor);
        Assert.Null(control.BackgroundImage);
        Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
        Assert.Null(control.BindingContext);
        Assert.Equal(s_dimension, control.Bottom);
        Assert.Equal(new Rectangle(0, 0, 104, s_dimension), control.Bounds);
        Assert.False(control.CanEnableIme);
        Assert.False(control.CanFocus);
        Assert.True(control.CanRaiseEvents);
        Assert.True(control.CanSelect);
        Assert.False(control.Capture);
        Assert.True(control.CausesValidation);
        Assert.Equal(new Size(104, s_dimension), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, 104, s_dimension), control.ClientRectangle);
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
        Assert.Equal(new Size(104, s_dimension), control.DefaultSize);
        Assert.False(control.DesignMode);
        Assert.Equal(new Rectangle(0, 0, 104, s_dimension), control.DisplayRectangle);
        Assert.Equal(DockStyle.None, control.Dock);
        Assert.False(control.DoubleBuffered);
        Assert.True(control.Enabled);
        Assert.NotNull(control.Events);
        Assert.Same(control.Events, control.Events);
        Assert.False(control.Focused);
        Assert.Equal(Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.Equal(SystemColors.WindowText, control.ForeColor);
        Assert.False(control.HasChildren);
        Assert.Equal(s_dimension, control.Height);
        Assert.Equal(ImeMode.Disable, control.ImeMode);
        Assert.Equal(ImeMode.Disable, control.ImeModeBase);
        Assert.False(control.IsAccessible);
        Assert.False(control.IsMirrored);
        Assert.Equal(5, control.LargeChange);
        Assert.NotNull(control.LayoutEngine);
        Assert.Same(control.LayoutEngine, control.LayoutEngine);
        Assert.Equal(0, control.Left);
        Assert.Equal(Point.Empty, control.Location);
        Assert.Equal(new Padding(3), control.Margin);
        Assert.Equal(10, control.Maximum);
        Assert.Equal(Size.Empty, control.MaximumSize);
        Assert.Equal(0, control.Minimum);
        Assert.Equal(Size.Empty, control.MinimumSize);
        Assert.Equal(Orientation.Horizontal, control.Orientation);
        Assert.Equal(Padding.Empty, control.Padding);
        Assert.Null(control.Parent);
        Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
        Assert.Equal(new Size(104, s_dimension), control.PreferredSize);
        Assert.False(control.RecreatingHandle);
        Assert.Null(control.Region);
        Assert.False(control.ResizeRedraw);
        Assert.Equal(104, control.Right);
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.False(control.RightToLeftLayout);
        Assert.True(control.ShowFocusCues);
        Assert.True(control.ShowKeyboardCues);
        Assert.Null(control.Site);
        Assert.Equal(new Size(104, s_dimension), control.Size);
        Assert.Equal(1, control.SmallChange);
        Assert.Equal(0, control.TabIndex);
        Assert.True(control.TabStop);
        Assert.Empty(control.Text);
        Assert.Equal(1, control.TickFrequency);
        Assert.Equal(TickStyle.BottomRight, control.TickStyle);
        Assert.Equal(0, control.Top);
        Assert.Null(control.TopLevelControl);
        Assert.False(control.UseWaitCursor);
        Assert.Equal(0, control.Value);
        Assert.True(control.Visible);
        Assert.Equal(104, control.Width);

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TrackBar_CreateParams_GetDefault_ReturnsExpected()
    {
        using SubTrackBar control = new();
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Equal("msctls_trackbar32", createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0, createParams.ExStyle);
        Assert.Equal(s_dimension, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(0x56010001, createParams.Style);
        Assert.Equal(104, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(TickStyle.Both, 0x56010009)]
    [InlineData(TickStyle.BottomRight, 0x56010001)]
    [InlineData(TickStyle.None, 0x56010010)]
    [InlineData(TickStyle.TopLeft, 0x56010005)]
    public void TrackBar_CreateParams_GetTickStyle_ReturnsExpected(TickStyle tickStyle, int expectedStyle)
    {
        using SubTrackBar control = new()
        {
            TickStyle = tickStyle
        };

        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Equal("msctls_trackbar32", createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0, createParams.ExStyle);
        Assert.Equal(s_dimension, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(expectedStyle, createParams.Style);
        Assert.Equal(104, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(Orientation.Horizontal, 0x56010001)]
    [InlineData(Orientation.Vertical, 0x56010003)]
    public void TrackBar_CreateParams_GetOrientation_ReturnsExpected(Orientation orientation, int expectedStyle)
    {
        using SubTrackBar control = new()
        {
            Orientation = orientation
        };

        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Equal("msctls_trackbar32", createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0, createParams.ExStyle);
        Assert.Equal(s_dimension, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(expectedStyle, createParams.Style);
        Assert.Equal(104, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.Inherit, true, 0x0)]
    [InlineData(RightToLeft.No, true, 0x0)]
    [InlineData(RightToLeft.Yes, true, 0x500000)]
    [InlineData(RightToLeft.Inherit, false, 0x0)]
    [InlineData(RightToLeft.No, false, 0x0)]
    [InlineData(RightToLeft.Yes, false, 0x7000)]
    public void TrackBar_CreateParams_GetRightToLeft_ReturnsExpected(RightToLeft rightToLeft, bool rightToLeftLayout, int expectedExStyle)
    {
        using SubTrackBar control = new()
        {
            RightToLeft = rightToLeft,
            RightToLeftLayout = rightToLeftLayout
        };

        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Equal("msctls_trackbar32", createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(expectedExStyle, createParams.ExStyle);
        Assert.Equal(s_dimension, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(0x56010001, createParams.Style);
        Assert.Equal(104, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void TrackBar_AutoSize_Set_GetReturnsExpected(bool value)
    {
        using SubTrackBar control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.AutoSize = value;
        Assert.Equal(value, control.AutoSize);
        Assert.Equal(new Size(104, s_dimension), control.Size);
        Assert.False(control.GetStyle(ControlStyles.FixedWidth));
        Assert.False(control.GetStyle(ControlStyles.FixedHeight));
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AutoSize = value;
        Assert.Equal(value, control.AutoSize);
        Assert.Equal(new Size(104, s_dimension), control.Size);
        Assert.False(control.GetStyle(ControlStyles.FixedWidth));
        Assert.False(control.GetStyle(ControlStyles.FixedHeight));
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.AutoSize = !value;
        Assert.Equal(!value, control.AutoSize);
        Assert.Equal(new Size(104, s_dimension), control.Size);
        Assert.False(control.GetStyle(ControlStyles.FixedWidth));
        Assert.Equal(!value, control.GetStyle(ControlStyles.FixedHeight));
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void TrackBar_AutoSize_SetWithOrientation_GetReturnsExpected(bool value)
    {
        using SubTrackBar control = new()
        {
            Orientation = Orientation.Vertical
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.AutoSize = value;
        Assert.Equal(value, control.AutoSize);
        Assert.Equal(new Size(104, s_dimension), control.Size);
        Assert.Equal(value, control.GetStyle(ControlStyles.FixedWidth));
        Assert.False(control.GetStyle(ControlStyles.FixedHeight));
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AutoSize = value;
        Assert.Equal(value, control.AutoSize);
        Assert.Equal(new Size(104, s_dimension), control.Size);
        Assert.Equal(value, control.GetStyle(ControlStyles.FixedWidth));
        Assert.False(control.GetStyle(ControlStyles.FixedHeight));
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.AutoSize = !value;
        Assert.Equal(!value, control.AutoSize);
        Assert.Equal(new Size(104, s_dimension), control.Size);
        Assert.Equal(!value, control.GetStyle(ControlStyles.FixedWidth));
        Assert.False(control.GetStyle(ControlStyles.FixedHeight));
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void TrackBar_AutoSize_SetWithHandle_GetReturnsExpected(bool value)
    {
        using SubTrackBar control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.AutoSize = value;
        Assert.Equal(value, control.AutoSize);
        Assert.Equal(new Size(104, s_dimension), control.Size);
        Assert.False(control.GetStyle(ControlStyles.FixedWidth));
        Assert.False(control.GetStyle(ControlStyles.FixedHeight));
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.AutoSize = value;
        Assert.Equal(value, control.AutoSize);
        Assert.Equal(new Size(104, s_dimension), control.Size);
        Assert.False(control.GetStyle(ControlStyles.FixedWidth));
        Assert.False(control.GetStyle(ControlStyles.FixedHeight));
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.AutoSize = !value;
        Assert.Equal(!value, control.AutoSize);
        Assert.Equal(new Size(104, s_dimension), control.Size);
        Assert.False(control.GetStyle(ControlStyles.FixedWidth));
        Assert.Equal(!value, control.GetStyle(ControlStyles.FixedHeight));
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void TrackBar_AutoSize_SetWithOrientationWithHandle_GetReturnsExpected(bool value)
    {
        using SubTrackBar control = new()
        {
            Orientation = Orientation.Vertical
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.AutoSize = value;
        Assert.Equal(value, control.AutoSize);
        Assert.Equal(new Size(s_dimension, s_dimension), control.Size);
        Assert.Equal(value, control.GetStyle(ControlStyles.FixedWidth));
        Assert.False(control.GetStyle(ControlStyles.FixedHeight));
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.AutoSize = value;
        Assert.Equal(value, control.AutoSize);
        Assert.Equal(new Size(s_dimension, s_dimension), control.Size);
        Assert.Equal(value, control.GetStyle(ControlStyles.FixedWidth));
        Assert.False(control.GetStyle(ControlStyles.FixedHeight));
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.AutoSize = !value;
        Assert.Equal(!value, control.AutoSize);
        Assert.Equal(new Size(s_dimension, s_dimension), control.Size);
        Assert.Equal(!value, control.GetStyle(ControlStyles.FixedWidth));
        Assert.False(control.GetStyle(ControlStyles.FixedHeight));
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ToolStrip_AutoSize_SetWithHandler_CallsAutoSizeChanged()
    {
        using ToolStrip control = new()
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
    public void TrackBar_BackgroundImage_Set_GetReturnsExpected(Image value)
    {
        using TrackBar control = new()
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
    public void TrackBar_BackgroundImage_SetWithHandler_CallsBackgroundImageChanged()
    {
        using TrackBar control = new();
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
    public void TrackBar_BackgroundImageLayout_Set_GetReturnsExpected(ImageLayout value)
    {
        using SubTrackBar control = new()
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
    public void TrackBar_BackgroundImageLayout_SetWithHandler_CallsBackgroundImageLayoutChanged()
    {
        using TrackBar control = new();
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
    [BoolData]
    public void TrackBar_DoubleBuffered_Get_ReturnsExpected(bool value)
    {
        using SubTrackBar control = new();
        control.SetStyle(ControlStyles.OptimizedDoubleBuffer, value);
        Assert.Equal(value, control.DoubleBuffered);
    }

    [WinFormsTheory]
    [BoolData]
    public void TrackBar_DoubleBuffered_Set_GetReturnsExpected(bool value)
    {
        using SubTrackBar control = new()
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
    public void TrackBar_DoubleBuffered_SetWithHandle_GetReturnsExpected(bool value)
    {
        using SubTrackBar control = new();
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
    public void TrackBar_Font_Set_GetReturnsExpected(Font value)
    {
        using SubTrackBar control = new()
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
    public void TrackBar_Font_SetWithHandler_CallsFontChanged()
    {
        using TrackBar control = new();
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

    public static IEnumerable<object[]> ForeColor_Set_TestData()
    {
        yield return new object[] { Color.Red };
        yield return new object[] { Color.FromArgb(254, 1, 2, 3) };
        yield return new object[] { Color.White };
        yield return new object[] { Color.Black };
        yield return new object[] { Color.Empty };
    }

    [WinFormsTheory]
    [MemberData(nameof(ForeColor_Set_TestData))]
    public void TrackBar_ForeColor_Set_Nop(Color value)
    {
        using TrackBar control = new()
        {
            ForeColor = value
        };
        Assert.Equal(SystemColors.WindowText, control.ForeColor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ForeColor = value;
        Assert.Equal(SystemColors.WindowText, control.ForeColor);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ForeColor_Set_TestData))]
    public void TrackBar_ForeColor_SetWithHandle_Nop(Color value)
    {
        using TrackBar control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ForeColor = value;
        Assert.Equal(SystemColors.WindowText, control.ForeColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.ForeColor = value;
        Assert.Equal(SystemColors.WindowText, control.ForeColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TrackBar_ForeColor_SetWithHandler_DoesNotCallForeColorChanged()
    {
        using TrackBar control = new();
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
        Assert.Equal(SystemColors.WindowText, control.ForeColor);
        Assert.Equal(0, callCount);

        // Set same.
        control.ForeColor = Color.Red;
        Assert.Equal(SystemColors.WindowText, control.ForeColor);
        Assert.Equal(0, callCount);

        // Set different.
        control.ForeColor = Color.Empty;
        Assert.Equal(SystemColors.WindowText, control.ForeColor);
        Assert.Equal(0, callCount);

        // Remove handler.
        control.ForeColorChanged -= handler;
        control.ForeColor = Color.Red;
        Assert.Equal(SystemColors.WindowText, control.ForeColor);
        Assert.Equal(0, callCount);
    }

    [WinFormsFact]
    public void TrackBar_Handle_GetWithMaximum_Success()
    {
        using TrackBar control = new()
        {
            Maximum = 11
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(11, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETRANGEMAX));
    }

    [WinFormsFact]
    public void TrackBar_Handle_GetWithMinimum_Success()
    {
        using TrackBar control = new()
        {
            Minimum = 11
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(11, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETRANGEMIN));
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.Inherit, true, 5)]
    [InlineData(RightToLeft.No, true, 5)]
    [InlineData(RightToLeft.Yes, true, 5)]
    [InlineData(RightToLeft.Inherit, false, 5)]
    [InlineData(RightToLeft.No, false, 5)]
    [InlineData(RightToLeft.Yes, false, 5)]
    public void TrackBar_Handle_GetWithValue_Success(RightToLeft rightToLeft, bool rightToLeftLayout, int expected)
    {
        using TrackBar control = new()
        {
            Value = 5,
            RightToLeft = rightToLeft,
            RightToLeftLayout = rightToLeftLayout
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(expected, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETPOS));
    }

    [WinFormsFact]
    public void TrackBar_Handle_GetWithValueVertical_Success()
    {
        using TrackBar control = new()
        {
            Orientation = Orientation.Vertical,
            Value = 5
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(5, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETPOS));
    }

    [WinFormsFact]
    public void TrackBar_Handle_GetWithLargeChange_Success()
    {
        using TrackBar control = new()
        {
            LargeChange = 11
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(11, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETPAGESIZE));
    }

    [WinFormsFact]
    public void TrackBar_Handle_GetWithSmallChange_Success()
    {
        using TrackBar control = new()
        {
            SmallChange = 11
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(11, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETLINESIZE));
    }

    public static IEnumerable<object[]> Handle_GetSize_TestData()
    {
        yield return new object[] { true, Orientation.Horizontal, new Size(104, s_dimension) };
        yield return new object[] { true, Orientation.Vertical, new Size(s_dimension, s_dimension) };
        yield return new object[] { false, Orientation.Horizontal, new Size(104, s_dimension) };
        yield return new object[] { false, Orientation.Vertical, new Size(s_dimension, s_dimension) };
    }

    [WinFormsTheory]
    [MemberData(nameof(Handle_GetSize_TestData))]
    public void TrackBar_Handle_GetSize_Success(bool autoSize, Orientation orientation, Size expected)
    {
        using TrackBar control = new()
        {
            AutoSize = autoSize,
            Orientation = orientation
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(expected, control.Size);
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
    public void TrackBar_ImeMode_Set_GetReturnsExpected(ImeMode value, ImeMode expected)
    {
        using TrackBar control = new()
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
    public void TrackBar_ImeMode_SetWithHandle_GetReturnsExpected(ImeMode value, ImeMode expected)
    {
        using TrackBar control = new();
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
    public void TrackBar_ImeMode_SetWithHandler_CallsImeModeChanged()
    {
        using TrackBar control = new();
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
    public void TrackBar_ImeMode_SetInvalid_ThrowsInvalidEnumArgumentException(ImeMode value)
    {
        using TrackBar control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.ImeMode = value);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(11)]
    public void TrackBar_LargeChange_Set_GetReturnsExpected(int value)
    {
        using SubTrackBar control = new()
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

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(11)]
    public void TrackBar_LargeChange_SetWithHandle_GetReturnsExpected(int value)
    {
        using SubTrackBar control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.LargeChange = value;
        Assert.Equal(value, control.LargeChange);
        Assert.Equal(value, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETPAGESIZE));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.LargeChange = value;
        Assert.Equal(value, control.LargeChange);
        Assert.Equal(value, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETPAGESIZE));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TrackBar_LargeChange_SetNegative_ThrowsArgumentOutOfRangeException()
    {
        using SubTrackBar control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.LargeChange = -1);
        Assert.Equal(5, control.LargeChange);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(8)]
    [InlineData(10)]
    [InlineData(11)]
    public void TrackBar_Maximum_Set_GetReturnsExpected(int value)
    {
        using SubTrackBar control = new()
        {
            Maximum = value
        };
        Assert.Equal(value, control.Maximum);
        Assert.Equal(0, control.Minimum);
        Assert.Equal(0, control.Value);
        Assert.Equal(5, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Maximum = value;
        Assert.Equal(value, control.Maximum);
        Assert.Equal(0, control.Minimum);
        Assert.Equal(0, control.Value);
        Assert.Equal(5, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0, 1)]
    [InlineData(8, 1)]
    [InlineData(10, 0)]
    [InlineData(11, 1)]
    public void TrackBar_Maximum_SetWithHandle_GetReturnsExpected(int value, int expectedInvalidatedCallCount)
    {
        using SubTrackBar control = new();
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
        Assert.Equal(5, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETRANGEMIN));
        Assert.Equal(value, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETRANGEMAX));
        Assert.Equal(0, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETPOS));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Maximum = value;
        Assert.Equal(value, control.Maximum);
        Assert.Equal(0, control.Minimum);
        Assert.Equal(0, control.Value);
        Assert.Equal(5, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETRANGEMIN));
        Assert.Equal(value, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETRANGEMAX));
        Assert.Equal(0, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETPOS));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TrackBar_Maximum_SetLessThanValueAndMinimum_SetsValueAndMinimum()
    {
        using SubTrackBar control = new()
        {
            Value = 10,
            Minimum = 8,
            Maximum = 5
        };
        Assert.Equal(5, control.Maximum);
        Assert.Equal(5, control.Minimum);
        Assert.Equal(5, control.Value);
        Assert.Equal(5, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TrackBar_Maximum_SetLessThanValueAndMinimumWithHandle_SetsValueAndMinimum()
    {
        using SubTrackBar control = new()
        {
            Value = 10,
            Minimum = 8
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Maximum = 5;
        Assert.Equal(5, control.Maximum);
        Assert.Equal(5, control.Minimum);
        Assert.Equal(5, control.Value);
        Assert.Equal(5, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        Assert.Equal(5, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETRANGEMIN));
        Assert.Equal(5, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETRANGEMAX));
        Assert.Equal(5, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETPOS));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TrackBar_Maximum_SetNegative_SetsValueAndMinimum()
    {
        using SubTrackBar control = new()
        {
            Maximum = -1
        };
        Assert.Equal(-1, control.Maximum);
        Assert.Equal(-1, control.Minimum);
        Assert.Equal(-1, control.Value);
        Assert.Equal(5, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(5)]
    public void TrackBar_Minimum_Set_GetReturnsExpected(int value)
    {
        using SubTrackBar control = new()
        {
            Value = 5,
            Minimum = value
        };
        Assert.Equal(10, control.Maximum);
        Assert.Equal(value, control.Minimum);
        Assert.Equal(5, control.Value);
        Assert.Equal(5, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Minimum = value;
        Assert.Equal(10, control.Maximum);
        Assert.Equal(value, control.Minimum);
        Assert.Equal(5, control.Value);
        Assert.Equal(5, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1, 1)]
    [InlineData(0, 0)]
    [InlineData(5, 1)]
    public void TrackBar_Minimum_SetWithHandle_GetReturnsExpected(int value, int expectedInvalidatedCallCount)
    {
        using SubTrackBar control = new()
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
        Assert.Equal(10, control.Maximum);
        Assert.Equal(value, control.Minimum);
        Assert.Equal(5, control.Value);
        Assert.Equal(5, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        Assert.Equal(value, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETRANGEMIN));
        Assert.Equal(10, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETRANGEMAX));
        Assert.Equal(5, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETPOS));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Minimum = value;
        Assert.Equal(10, control.Maximum);
        Assert.Equal(value, control.Minimum);
        Assert.Equal(5, control.Value);
        Assert.Equal(5, control.LargeChange);
        Assert.Equal(value, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETRANGEMIN));
        Assert.Equal(10, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETRANGEMAX));
        Assert.Equal(5, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETPOS));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TrackBar_Minimum_SetGreaterThanValueAndMaximum_SetsValueAndMinimum()
    {
        using SubTrackBar control = new()
        {
            Value = 10,
            Maximum = 8,
            Minimum = 12
        };
        Assert.Equal(12, control.Maximum);
        Assert.Equal(12, control.Minimum);
        Assert.Equal(12, control.Value);
        Assert.Equal(5, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TrackBar_Minimum_SetGreaterThanValueAndMaximumWithHandle_SetsValueAndMinimum()
    {
        using SubTrackBar control = new()
        {
            Value = 10,
            Maximum = 8
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Minimum = 12;
        Assert.Equal(12, control.Maximum);
        Assert.Equal(12, control.Minimum);
        Assert.Equal(12, control.Value);
        Assert.Equal(5, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        Assert.Equal(12, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETRANGEMIN));
        Assert.Equal(12, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETRANGEMAX));
        Assert.Equal(12, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETPOS));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> Orientation_Set_TestData()
    {
        yield return new object[] { true, Orientation.Horizontal, new Size(104, s_dimension), false };
        yield return new object[] { true, Orientation.Vertical, new Size(104, s_dimension), true };
        yield return new object[] { false, Orientation.Horizontal, new Size(104, s_dimension), false };
        yield return new object[] { false, Orientation.Vertical, new Size(104, s_dimension), false };
    }

    [WinFormsTheory]
    [MemberData(nameof(Orientation_Set_TestData))]
    public void TrackBar_Orientation_Set_GetReturnsExpected(bool autoSize, Orientation value, Size expectedSize, bool expectedFixedWidth)
    {
        using SubTrackBar control = new()
        {
            AutoSize = autoSize,
            Orientation = value
        };
        Assert.Equal(value, control.Orientation);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedFixedWidth, control.GetStyle(ControlStyles.FixedWidth));
        Assert.False(control.GetStyle(ControlStyles.FixedHeight));
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Orientation = value;
        Assert.Equal(value, control.Orientation);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedFixedWidth, control.GetStyle(ControlStyles.FixedWidth));
        Assert.False(control.GetStyle(ControlStyles.FixedHeight));
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Orientation_SetWithCustomOldValue_TestData()
    {
        yield return new object[] { true, Orientation.Horizontal, new Size(s_dimension, s_dimension), false, true };
        yield return new object[] { true, Orientation.Vertical, new Size(104, s_dimension), true, false };
        yield return new object[] { false, Orientation.Horizontal, new Size(s_dimension, s_dimension), false, false };
        yield return new object[] { false, Orientation.Vertical, new Size(104, s_dimension), false, false };
    }

    [WinFormsTheory]
    [MemberData(nameof(Orientation_SetWithCustomOldValue_TestData))]
    public void TrackBar_Orientation_SetWithCustomOldValue_GetReturnsExpected(bool autoSize, Orientation value, Size expectedSize, bool expectedFixedWidth, bool expectedFixedHeight)
    {
        using SubTrackBar control = new()
        {
            AutoSize = autoSize,
            Orientation = Orientation.Vertical
        };

        control.Orientation = value;
        Assert.Equal(value, control.Orientation);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedFixedWidth, control.GetStyle(ControlStyles.FixedWidth));
        Assert.Equal(expectedFixedHeight, control.GetStyle(ControlStyles.FixedHeight));
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Orientation = value;
        Assert.Equal(value, control.Orientation);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedFixedWidth, control.GetStyle(ControlStyles.FixedWidth));
        Assert.Equal(expectedFixedHeight, control.GetStyle(ControlStyles.FixedHeight));
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Orientation_SetWithHandle_TestData()
    {
        yield return new object[] { true, Orientation.Horizontal, new Size(104, s_dimension), false, false, 0 };
        yield return new object[] { true, Orientation.Vertical, new Size(s_dimension, 104), true, false, 1 };
        yield return new object[] { false, Orientation.Horizontal, new Size(104, s_dimension), false, false, 0 };
        yield return new object[] { false, Orientation.Vertical, new Size(s_dimension, 104), false, false, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Orientation_SetWithHandle_TestData))]
    public void TrackBar_Orientation_SetWithHandle_GetReturnsExpected(bool autoSize, Orientation value, Size expectedSize, bool expectedFixedWidth, bool expectedFixedHeight, int expectedCreatedCallCount)
    {
        using SubTrackBar control = new()
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

        control.Orientation = value;
        Assert.Equal(value, control.Orientation);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedFixedWidth, control.GetStyle(ControlStyles.FixedWidth));
        Assert.Equal(expectedFixedHeight, control.GetStyle(ControlStyles.FixedHeight));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        control.Orientation = value;
        Assert.Equal(value, control.Orientation);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedFixedWidth, control.GetStyle(ControlStyles.FixedWidth));
        Assert.Equal(expectedFixedHeight, control.GetStyle(ControlStyles.FixedHeight));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);
    }

    public static IEnumerable<object[]> Orientation_SetWithCustomOldValueWithHandle_TestData()
    {
        yield return new object[] { true, Orientation.Horizontal, new Size(s_dimension, s_dimension), false, true, 1 };
        yield return new object[] { true, Orientation.Vertical, new Size(s_dimension, s_dimension), true, false, 0 };
        yield return new object[] { false, Orientation.Horizontal, new Size(s_dimension, s_dimension), false, false, 1 };
        yield return new object[] { false, Orientation.Vertical, new Size(s_dimension, s_dimension), false, false, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Orientation_SetWithCustomOldValueWithHandle_TestData))]
    public void TrackBar_Orientation_SetWithCustomOldValueWithHandle_GetReturnsExpected(bool autoSize, Orientation value, Size expectedSize, bool expectedFixedWidth, bool expectedFixedHeight, int expectedCreatedCallCount)
    {
        using SubTrackBar control = new()
        {
            AutoSize = autoSize,
            Orientation = Orientation.Vertical
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Orientation = value;
        Assert.Equal(value, control.Orientation);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedFixedWidth, control.GetStyle(ControlStyles.FixedWidth));
        Assert.Equal(expectedFixedHeight, control.GetStyle(ControlStyles.FixedHeight));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        control.Orientation = value;
        Assert.Equal(value, control.Orientation);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedFixedWidth, control.GetStyle(ControlStyles.FixedWidth));
        Assert.Equal(expectedFixedHeight, control.GetStyle(ControlStyles.FixedHeight));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<Orientation>]
    public void TrackBar_Orientation_SetInvalidValue_ThrowsInvalidEnumArgumentException(Orientation value)
    {
        using TrackBar control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.Orientation = value);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaddingNormalizedTheoryData))]
    public void TrackBar_Padding_Set_GetReturnsExpected(Padding value, Padding expected)
    {
        using TrackBar control = new()
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
    public void TrackBar_Padding_SetWithHandle_GetReturnsExpected(Padding value, Padding expected)
    {
        using TrackBar control = new();
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
    public void TrackBar_Padding_SetWithHandler_CallsPaddingChanged()
    {
        using TrackBar control = new();
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
    public void TrackBar_RightToLeftLayout_Set_GetReturnsExpected(RightToLeft rightToLeft, bool value, int expectedLayoutCallCount)
    {
        using SubTrackBar control = new()
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
    public void TrackBar_RightToLeftLayout_SetWithHandle_GetReturnsExpected(RightToLeft rightToLeft, bool value, int expectedLayoutCallCount, int expectedCreatedCallCount1, int expectedCreatedCallCount2)
    {
        using TrackBar control = new()
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
    public void TrackBar_RightToLeftLayout_SetWithHandler_CallsRightToLeftLayoutChanged()
    {
        using TrackBar control = new()
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
    public void TrackBar_RightToLeftLayout_SetWithHandlerInDisposing_DoesNotRightToLeftLayoutChanged()
    {
        using TrackBar control = new()
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

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(11)]
    public void TrackBar_SmallChange_Set_GetReturnsExpected(int value)
    {
        using SubTrackBar control = new()
        {
            SmallChange = value
        };
        Assert.Equal(value, control.SmallChange);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SmallChange = value;
        Assert.Equal(value, control.SmallChange);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(11)]
    public void TrackBar_SmallChange_SetWithHandle_GetReturnsExpected(int value)
    {
        using SubTrackBar control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SmallChange = value;
        Assert.Equal(value, control.SmallChange);
        Assert.Equal(value, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETLINESIZE));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.SmallChange = value;
        Assert.Equal(value, control.SmallChange);
        Assert.Equal(value, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETLINESIZE));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TrackBar_SmallChange_SetNegative_ThrowsArgumentOutOfRangeException()
    {
        using SubTrackBar control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.SmallChange = -1);
        Assert.Equal(1, control.SmallChange);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void TrackBar_Text_Set_GetReturnsExpected(string value, string expected)
    {
        using TrackBar control = new()
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
    public void TrackBar_Text_SetWithHandle_GetReturnsExpected(string value, string expected)
    {
        using TrackBar control = new();
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
    public void TrackBar_Text_SetWithHandler_CallsTextChanged()
    {
        using TrackBar control = new();
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
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(20)]
    [InlineData(int.MaxValue)]
    public void TrackBar_TickFrequency_Set_GetReturnsExpected(int value)
    {
        using TrackBar control = new()
        {
            TickFrequency = value
        };
        Assert.Equal(value, control.TickFrequency);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.TickFrequency = value;
        Assert.Equal(value, control.TickFrequency);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1, 1)]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    [InlineData(20, 1)]
    [InlineData(int.MaxValue, 1)]
    public void TrackBar_TickFrequency_SetWithHandle_GetReturnsExpected(int value, int expectedInvalidatedCallCount)
    {
        using TrackBar control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.TickFrequency = value;
        Assert.Equal(value, control.TickFrequency);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.TickFrequency = value;
        Assert.Equal(value, control.TickFrequency);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [EnumData<TickStyle>]
    public void TrackBar_TickStyle_Set_GetReturnsExpected(TickStyle value)
    {
        using TrackBar control = new()
        {
            TickStyle = value
        };
        Assert.Equal(value, control.TickStyle);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.TickStyle = value;
        Assert.Equal(value, control.TickStyle);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(TickStyle.Both, 1)]
    [InlineData(TickStyle.BottomRight, 0)]
    [InlineData(TickStyle.None, 1)]
    [InlineData(TickStyle.TopLeft, 1)]
    public void TrackBar_TickStyle_SetWithHandle_GetReturnsExpected(TickStyle value, int expectedCreatedCallCount)
    {
        using TrackBar control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.TickStyle = value;
        Assert.Equal(value, control.TickStyle);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        control.TickStyle = value;
        Assert.Equal(value, control.TickStyle);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<TickStyle>]
    public void TrackBar_TickStyle_SetInvalidValue_ThrowsInvalidEnumArgumentException(TickStyle value)
    {
        using TrackBar control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.TickStyle = value);
    }

    public static IEnumerable<object[]> Value_Set_TestData()
    {
        foreach (Orientation orientation in Enum.GetValues(typeof(Orientation)))
        {
            foreach (RightToLeft rightToLeft in Enum.GetValues(typeof(RightToLeft)))
            {
                foreach (bool rightToLeftLayout in new bool[] { true, false })
                {
                    yield return new object[] { orientation, rightToLeft, rightToLeftLayout, 0 };
                    yield return new object[] { orientation, rightToLeft, rightToLeftLayout, 1 };
                    yield return new object[] { orientation, rightToLeft, rightToLeftLayout, 5 };
                    yield return new object[] { orientation, rightToLeft, rightToLeftLayout, 9 };
                    yield return new object[] { orientation, rightToLeft, rightToLeftLayout, 10 };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Value_Set_TestData))]
    public void TrackBar_Value_Set_GetReturnsExpected(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int value)
    {
        using TrackBar control = new()
        {
            Orientation = orientation,
            RightToLeft = rightToLeft,
            RightToLeftLayout = rightToLeftLayout,
            Value = value
        };
        Assert.Equal(10, control.Maximum);
        Assert.Equal(0, control.Minimum);
        Assert.Equal(value, control.Value);
        Assert.Equal(5, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Value = value;
        Assert.Equal(10, control.Maximum);
        Assert.Equal(0, control.Minimum);
        Assert.Equal(value, control.Value);
        Assert.Equal(5, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(9)]
    [InlineData(10)]
    [InlineData(11)]
    public void TrackBar_Value_SetInitializing_GetReturnsExpected(int value)
    {
        using TrackBar control = new();
        control.BeginInit();

        control.Value = value;
        Assert.Equal(10, control.Maximum);
        Assert.Equal(0, control.Minimum);
        Assert.Equal(value, control.Value);
        Assert.Equal(5, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Value = value;
        Assert.Equal(10, control.Maximum);
        Assert.Equal(0, control.Minimum);
        Assert.Equal(value, control.Value);
        Assert.Equal(5, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Value_SetWithHandle_TestData()
    {
        yield return new object[] { Orientation.Horizontal, RightToLeft.Yes, true, 0, 0 };
        yield return new object[] { Orientation.Horizontal, RightToLeft.Yes, false, 0, 10 };
        yield return new object[] { Orientation.Horizontal, RightToLeft.No, true, 0, 0 };
        yield return new object[] { Orientation.Horizontal, RightToLeft.No, false, 0, 0 };
        yield return new object[] { Orientation.Horizontal, RightToLeft.Inherit, true, 0, 0 };
        yield return new object[] { Orientation.Horizontal, RightToLeft.Inherit, false, 0, 0 };
        yield return new object[] { Orientation.Vertical, RightToLeft.Yes, true, 0, 10 };
        yield return new object[] { Orientation.Vertical, RightToLeft.Yes, false, 0, 10 };
        yield return new object[] { Orientation.Vertical, RightToLeft.No, true, 0, 10 };
        yield return new object[] { Orientation.Vertical, RightToLeft.No, false, 0, 10 };
        yield return new object[] { Orientation.Vertical, RightToLeft.Inherit, true, 0, 10 };
        yield return new object[] { Orientation.Vertical, RightToLeft.Inherit, false, 0, 10 };

        yield return new object[] { Orientation.Horizontal, RightToLeft.Yes, true, 1, 1 };
        yield return new object[] { Orientation.Horizontal, RightToLeft.Yes, false, 1, 9 };
        yield return new object[] { Orientation.Horizontal, RightToLeft.No, true, 1, 1 };
        yield return new object[] { Orientation.Horizontal, RightToLeft.No, false, 1, 1 };
        yield return new object[] { Orientation.Horizontal, RightToLeft.Inherit, true, 1, 1 };
        yield return new object[] { Orientation.Horizontal, RightToLeft.Inherit, false, 1, 1 };
        yield return new object[] { Orientation.Vertical, RightToLeft.Yes, true, 1, 9 };
        yield return new object[] { Orientation.Vertical, RightToLeft.Yes, false, 1, 9 };
        yield return new object[] { Orientation.Vertical, RightToLeft.No, true, 1, 9 };
        yield return new object[] { Orientation.Vertical, RightToLeft.No, false, 1, 9 };
        yield return new object[] { Orientation.Vertical, RightToLeft.Inherit, true, 1, 9 };
        yield return new object[] { Orientation.Vertical, RightToLeft.Inherit, false, 1, 9 };

        yield return new object[] { Orientation.Horizontal, RightToLeft.Yes, true, 5, 5 };
        yield return new object[] { Orientation.Horizontal, RightToLeft.Yes, false, 5, 5 };
        yield return new object[] { Orientation.Horizontal, RightToLeft.No, true, 5, 5 };
        yield return new object[] { Orientation.Horizontal, RightToLeft.No, false, 5, 5 };
        yield return new object[] { Orientation.Horizontal, RightToLeft.Inherit, true, 5, 5 };
        yield return new object[] { Orientation.Horizontal, RightToLeft.Inherit, false, 5, 5 };
        yield return new object[] { Orientation.Vertical, RightToLeft.Yes, true, 5, 5 };
        yield return new object[] { Orientation.Vertical, RightToLeft.Yes, false, 5, 5 };
        yield return new object[] { Orientation.Vertical, RightToLeft.No, true, 5, 5 };
        yield return new object[] { Orientation.Vertical, RightToLeft.No, false, 5, 5 };
        yield return new object[] { Orientation.Vertical, RightToLeft.Inherit, true, 5, 5 };
        yield return new object[] { Orientation.Vertical, RightToLeft.Inherit, false, 5, 5 };

        yield return new object[] { Orientation.Horizontal, RightToLeft.Yes, true, 9, 9 };
        yield return new object[] { Orientation.Horizontal, RightToLeft.Yes, false, 9, 1 };
        yield return new object[] { Orientation.Horizontal, RightToLeft.No, true, 9, 9 };
        yield return new object[] { Orientation.Horizontal, RightToLeft.No, false, 9, 9 };
        yield return new object[] { Orientation.Horizontal, RightToLeft.Inherit, true, 9, 9 };
        yield return new object[] { Orientation.Horizontal, RightToLeft.Inherit, false, 9, 9 };
        yield return new object[] { Orientation.Vertical, RightToLeft.Yes, true, 9, 1 };
        yield return new object[] { Orientation.Vertical, RightToLeft.Yes, false, 9, 1 };
        yield return new object[] { Orientation.Vertical, RightToLeft.No, true, 9, 1 };
        yield return new object[] { Orientation.Vertical, RightToLeft.No, false, 9, 1 };
        yield return new object[] { Orientation.Vertical, RightToLeft.Inherit, true, 9, 1 };
        yield return new object[] { Orientation.Vertical, RightToLeft.Inherit, false, 9, 1 };

        yield return new object[] { Orientation.Horizontal, RightToLeft.Yes, true, 10, 10 };
        yield return new object[] { Orientation.Horizontal, RightToLeft.Yes, false, 10, 0 };
        yield return new object[] { Orientation.Horizontal, RightToLeft.No, true, 10, 10 };
        yield return new object[] { Orientation.Horizontal, RightToLeft.No, false, 10, 10 };
        yield return new object[] { Orientation.Horizontal, RightToLeft.Inherit, true, 10, 10 };
        yield return new object[] { Orientation.Horizontal, RightToLeft.Inherit, false, 10, 10 };
        yield return new object[] { Orientation.Vertical, RightToLeft.Yes, true, 10, 0 };
        yield return new object[] { Orientation.Vertical, RightToLeft.Yes, false, 10, 0 };
        yield return new object[] { Orientation.Vertical, RightToLeft.No, true, 10, 0 };
        yield return new object[] { Orientation.Vertical, RightToLeft.No, false, 10, 0 };
        yield return new object[] { Orientation.Vertical, RightToLeft.Inherit, true, 10, 0 };
        yield return new object[] { Orientation.Vertical, RightToLeft.Inherit, false, 10, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Value_SetWithHandle_TestData))]
    public void TrackBar_Value_SetWithHandle_GetReturnsExpected(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int value, int expectedPos)
    {
        using TrackBar control = new()
        {
            Orientation = orientation,
            RightToLeft = rightToLeft,
            RightToLeftLayout = rightToLeftLayout
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Value = value;
        Assert.Equal(10, control.Maximum);
        Assert.Equal(0, control.Minimum);
        Assert.Equal(value, control.Value);
        Assert.Equal(5, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        Assert.Equal(expectedPos, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETPOS));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Value = value;
        Assert.Equal(10, control.Maximum);
        Assert.Equal(0, control.Minimum);
        Assert.Equal(value, control.Value);
        Assert.Equal(5, control.LargeChange);
        Assert.Equal(1, control.SmallChange);
        Assert.Equal(expectedPos, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETPOS));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TrackBar_Value_SetWithHandler_CallsValueChanged()
    {
        using TrackBar control = new();
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
    [InlineData(11)]
    public void TrackBar_Value_SetOutOfRange_ThrowsArgumentOutOfRangeException(int value)
    {
        using TrackBar control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.Value = value);
        Assert.Equal(0, control.Value);
    }

    [WinFormsFact]
    public void TrackBar_BeginInit_InvokeMultipleTimes_Success()
    {
        using TrackBar control = new();
        control.BeginInit();

        // Call again.
        control.BeginInit();
    }

    [WinFormsFact]
    public void TrackBar_CreateHandle_Invoke_Success()
    {
        using SubTrackBar control = new();
        control.CreateHandle();
        Assert.True(control.Created);
        Assert.True(control.IsHandleCreated);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(9)]
    [InlineData(10)]
    public void TrackBar_EndInit_InvokeNotInitializing_Nop(int value)
    {
        using TrackBar control = new();
        control.Value = value;
        Assert.Equal(value, control.Value);

        control.EndInit();
        Assert.Equal(value, control.Value);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.EndInit();
        Assert.Equal(value, control.Value);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(5, 5)]
    [InlineData(9, 9)]
    [InlineData(10, 10)]
    [InlineData(11, 10)]
    public void TrackBar_EndInit_InvokeInitializing_Success(int value, int expectedValue)
    {
        using TrackBar control = new();
        control.BeginInit();
        control.Value = value;
        Assert.Equal(value, control.Value);

        control.EndInit();
        Assert.Equal(expectedValue, control.Value);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.EndInit();
        Assert.Equal(expectedValue, control.Value);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TrackBar_GetAutoSizeMode_Invoke_ReturnsExpected()
    {
        using SubTrackBar control = new();
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
    public void TrackBar_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
    {
        using SubTrackBar control = new();
        Assert.Equal(expected, control.GetStyle(flag));

        // Call again to test caching.
        Assert.Equal(expected, control.GetStyle(flag));
    }

    [WinFormsFact]
    public void TrackBar_GetTopLevel_Invoke_ReturnsExpected()
    {
        using SubTrackBar control = new();
        Assert.False(control.GetTopLevel());
    }

    [WinFormsTheory]
    [InlineData(Keys.Alt, false)]
    [InlineData(Keys.Alt | Keys.PageUp, false)]
    [InlineData(Keys.PageUp, true)]
    [InlineData(Keys.PageDown, true)]
    [InlineData(Keys.Home, true)]
    [InlineData(Keys.End, true)]
    [InlineData(Keys.A, false)]
    public void TrackBar_IsInputKey_Invoke_ReturnsExpected(Keys keyData, bool expected)
    {
        using SubTrackBar control = new();
        Assert.Equal(expected, control.IsInputKey(keyData));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void TrackBar_OnBackColorChanged_Invoke_CallsBackColorChanged(EventArgs eventArgs)
    {
        using SubTrackBar control = new();
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
    public void TrackBar_OnBackColorChanged_InvokeWithHandle_CallsBackColorChanged(EventArgs eventArgs)
    {
        using SubTrackBar control = new();
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
        control.BackColorChanged += handler;
        control.OnBackColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.BackColorChanged -= handler;
        control.OnBackColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(4, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void TrackBar_OnClick_Invoke_CallsClick(EventArgs eventArgs)
    {
        using SubTrackBar control = new();
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
    public void TrackBar_OnDoubleClick_Invoke_CallsDoubleClick(EventArgs eventArgs)
    {
        using SubTrackBar control = new();
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
    public void TrackBar_OnHandleCreated_Invoke_CallsHandleCreated(EventArgs eventArgs)
    {
        using SubTrackBar control = new();
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
    public void TrackBar_OnHandleCreated_InvokeWithHandle_CallsHandleCreated(EventArgs eventArgs)
    {
        using SubTrackBar control = new();
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
    public void TrackBar_OnMouseClick_Invoke_CallsMouseClick(MouseEventArgs eventArgs)
    {
        using SubTrackBar control = new();
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
    public void TrackBar_OnMouseDoubleClick_Invoke_CallsMouseDoubleClick(MouseEventArgs eventArgs)
    {
        using SubTrackBar control = new();
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
    public void TrackBar_OnMouseWheel_Invoke_CallsMouseWheel(MouseEventArgs eventArgs)
    {
        using SubTrackBar control = new();
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
    public void TrackBar_OnMouseWheel_InvokeHandledMouseEventArgs_SetsHandled(bool handled)
    {
        using SubTrackBar control = new();
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
    public void TrackBar_OnMouseWheel_NullE_ThrowsNullReferenceException()
    {
        using SubTrackBar control = new();
        Assert.Throws<NullReferenceException>(() => control.OnMouseWheel(null));
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
    public void TrackBar_OnRightToLeftLayoutChanged_Invoke_CallsRightToLeftLayoutChanged(RightToLeft rightToLeft, EventArgs eventArgs)
    {
        using SubTrackBar control = new()
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
    public void TrackBar_OnRightToLeftLayoutChanged_InvokeWithHandle_CallsRightToLeftLayoutChanged(RightToLeft rightToLeft, EventArgs eventArgs, int expectedCreatedCallCount)
    {
        using SubTrackBar control = new()
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
    public void TrackBar_OnRightToLeftLayoutChanged_InvokeInDisposing_DoesNotCallRightToLeftLayoutChanged()
    {
        using SubTrackBar control = new()
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

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void TrackBar_OnScroll_Invoke_CallsScroll(EventArgs eventArgs)
    {
        using SubTrackBar control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
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
    public void TrackBar_OnSystemColorsChanged_Invoke_CallsSystemColorsChanged(EventArgs eventArgs)
    {
        using SubTrackBar control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.SystemColorsChanged += handler;
        control.OnSystemColorsChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.SystemColorsChanged -= handler;
        control.OnSystemColorsChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void TrackBar_OnSystemColorsChanged_InvokeWithHandle_CallsSystemColorsChanged(EventArgs eventArgs)
    {
        using SubTrackBar control = new();
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
        control.SystemColorsChanged += handler;
        control.OnSystemColorsChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.SystemColorsChanged -= handler;
        control.OnSystemColorsChanged(eventArgs);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(4, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void TrackBar_OnValueChanged_Invoke_CallsValueChanged(EventArgs eventArgs)
    {
        using SubTrackBar control = new();
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

    public static IEnumerable<object[]> SetBoundsCore_TestData()
    {
        foreach (BoundsSpecified specified in Enum.GetValues(typeof(BoundsSpecified)))
        {
            foreach (Orientation orientation in Enum.GetValues(typeof(Orientation)))
            {
                yield return new object[] { false, orientation, 0, 0, 104, s_dimension, specified, 104, s_dimension, 0, 0 };
                yield return new object[] { false, orientation, 0, 0, 0, 0, specified, 0, 0, 0, 1 };
                yield return new object[] { false, orientation, -1, -2, -3, -4, specified, -3, -4, 1, 1 };
                yield return new object[] { false, orientation, 1, 0, 0, 0, specified, 0, 0, 1, 1 };
                yield return new object[] { false, orientation, 0, 2, 0, 0, specified, 0, 0, 1, 1 };
                yield return new object[] { false, orientation, 1, 2, 0, 0, specified, 0, 0, 1, 1 };
                yield return new object[] { false, orientation, 0, 0, 1, 0, specified, 1, 0, 0, 1 };
                yield return new object[] { false, orientation, 0, 0, 0, 2, specified, 0, 2, 0, 1 };
                yield return new object[] { false, orientation, 0, 0, 1, 2, specified, 1, 2, 0, 1 };
                yield return new object[] { false, orientation, 1, 2, 30, 40, specified, 30, 40, 1, 1 };
            }
        }

        foreach (BoundsSpecified specified in Enum.GetValues(typeof(BoundsSpecified)))
        {
            if ((specified & BoundsSpecified.Height) != 0)
            {
                continue;
            }

            yield return new object[] { true, Orientation.Horizontal, 0, 0, 104, s_dimension, specified, 104, s_dimension, 0, 0 };
            yield return new object[] { true, Orientation.Horizontal, 0, 0, 0, 0, specified, 0, 0, 0, 1 };
            yield return new object[] { true, Orientation.Horizontal, -1, -2, -3, -4, specified, -3, -4, 1, 1 };
            yield return new object[] { true, Orientation.Horizontal, 1, 0, 0, 0, specified, 0, 0, 1, 1 };
            yield return new object[] { true, Orientation.Horizontal, 0, 2, 0, 0, specified, 0, 0, 1, 1 };
            yield return new object[] { true, Orientation.Horizontal, 1, 2, 0, 0, specified, 0, 0, 1, 1 };
            yield return new object[] { true, Orientation.Horizontal, 0, 0, 1, 0, specified, 1, 0, 0, 1 };
            yield return new object[] { true, Orientation.Horizontal, 0, 0, 0, 2, specified, 0, 2, 0, 1 };
            yield return new object[] { true, Orientation.Horizontal, 0, 0, 1, 2, specified, 1, 2, 0, 1 };
            yield return new object[] { true, Orientation.Horizontal, 1, 2, 30, 40, specified, 30, 40, 1, 1 };
        }

        foreach (BoundsSpecified specified in new BoundsSpecified[] { BoundsSpecified.Height, BoundsSpecified.Size, BoundsSpecified.All })
        {
            yield return new object[] { true, Orientation.Horizontal, 0, 0, 104, s_dimension, specified, 104, s_dimension, 0, 0 };
            yield return new object[] { true, Orientation.Horizontal, 0, 0, 0, 0, specified, 0, s_dimension, 0, 1 };
            yield return new object[] { true, Orientation.Horizontal, -1, -2, -3, -4, specified, -3, s_dimension, 1, 1 };
            yield return new object[] { true, Orientation.Horizontal, 1, 0, 0, 0, specified, 0, s_dimension, 1, 1 };
            yield return new object[] { true, Orientation.Horizontal, 0, 2, 0, 0, specified, 0, s_dimension, 1, 1 };
            yield return new object[] { true, Orientation.Horizontal, 1, 2, 0, 0, specified, 0, s_dimension, 1, 1 };
            yield return new object[] { true, Orientation.Horizontal, 0, 0, 1, 0, specified, 1, s_dimension, 0, 1 };
            yield return new object[] { true, Orientation.Horizontal, 0, 0, 0, 2, specified, 0, s_dimension, 0, 1 };
            yield return new object[] { true, Orientation.Horizontal, 0, 0, 1, 2, specified, 1, s_dimension, 0, 1 };
            yield return new object[] { true, Orientation.Horizontal, 1, 2, 30, 40, specified, 30, s_dimension, 1, 1 };
        }

        foreach (BoundsSpecified specified in Enum.GetValues(typeof(BoundsSpecified)))
        {
            if ((specified & BoundsSpecified.Width) != 0)
            {
                continue;
            }

            yield return new object[] { true, Orientation.Vertical, 0, 0, 104, s_dimension, specified, 104, s_dimension, 0, 0 };
            yield return new object[] { true, Orientation.Vertical, 0, 0, 0, 0, specified, 0, 0, 0, 1 };
            yield return new object[] { true, Orientation.Vertical, -1, -2, -3, -4, specified, -3, -4, 1, 1 };
            yield return new object[] { true, Orientation.Vertical, 1, 0, 0, 0, specified, 0, 0, 1, 1 };
            yield return new object[] { true, Orientation.Vertical, 0, 2, 0, 0, specified, 0, 0, 1, 1 };
            yield return new object[] { true, Orientation.Vertical, 1, 2, 0, 0, specified, 0, 0, 1, 1 };
            yield return new object[] { true, Orientation.Vertical, 0, 0, 1, 0, specified, 1, 0, 0, 1 };
            yield return new object[] { true, Orientation.Vertical, 0, 0, 0, 2, specified, 0, 2, 0, 1 };
            yield return new object[] { true, Orientation.Vertical, 0, 0, 1, 2, specified, 1, 2, 0, 1 };
            yield return new object[] { true, Orientation.Vertical, 1, 2, 30, 40, specified, 30, 40, 1, 1 };
        }

        foreach (BoundsSpecified specified in new BoundsSpecified[] { BoundsSpecified.Width, BoundsSpecified.Size, BoundsSpecified.All })
        {
            yield return new object[] { true, Orientation.Vertical, 0, 0, 104, s_dimension, specified, s_dimension, s_dimension, 0, 1 };
            yield return new object[] { true, Orientation.Vertical, 0, 0, 0, 0, specified, s_dimension, 0, 0, 1 };
            yield return new object[] { true, Orientation.Vertical, -1, -2, -3, -4, specified, s_dimension, -4, 1, 1 };
            yield return new object[] { true, Orientation.Vertical, 1, 0, 0, 0, specified, s_dimension, 0, 1, 1 };
            yield return new object[] { true, Orientation.Vertical, 0, 2, 0, 0, specified, s_dimension, 0, 1, 1 };
            yield return new object[] { true, Orientation.Vertical, 1, 2, 0, 0, specified, s_dimension, 0, 1, 1 };
            yield return new object[] { true, Orientation.Vertical, 0, 0, 1, 0, specified, s_dimension, 0, 0, 1 };
            yield return new object[] { true, Orientation.Vertical, 0, 0, 0, 2, specified, s_dimension, 2, 0, 1 };
            yield return new object[] { true, Orientation.Vertical, 0, 0, 1, 2, specified, s_dimension, 2, 0, 1 };
            yield return new object[] { true, Orientation.Vertical, 1, 2, 30, 40, specified, s_dimension, 40, 1, 1 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(SetBoundsCore_TestData))]
    public void TrackBar_SetBoundsCore_Invoke_Success(bool autoSize, Orientation orientation, int x, int y, int width, int height, BoundsSpecified specified, int expectedWidth, int expectedHeight, int expectedLocationChangedCallCount, int expectedLayoutCallCount)
    {
        using SubTrackBar control = new()
        {
            AutoSize = autoSize,
            Orientation = orientation
        };
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
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
        Assert.Equal(x, control.Left);
        Assert.Equal(x + expectedWidth, control.Right);
        Assert.Equal(y, control.Top);
        Assert.Equal(y + expectedHeight, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(x, y, expectedWidth, expectedHeight), control.Bounds);
        Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.SetBoundsCore(x, y, width, height, specified);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
        Assert.Equal(x, control.Left);
        Assert.Equal(x + expectedWidth, control.Right);
        Assert.Equal(y, control.Top);
        Assert.Equal(y + expectedHeight, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(x, y, expectedWidth, expectedHeight), control.Bounds);
        Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> SetRange_TestData()
    {
        yield return new object[] { 0, 10, 0, 10, 5 };
        yield return new object[] { 0, 9, 0, 9, 5 };
        yield return new object[] { 1, 10, 1, 10, 5 };
        yield return new object[] { 10, 0, 10, 10, 10 };
        yield return new object[] { 1, 9, 1, 9, 5 };
        yield return new object[] { 5, 9, 5, 9, 5 };
        yield return new object[] { 6, 9, 6, 9, 6 };
        yield return new object[] { 1, 5, 1, 5, 5 };
        yield return new object[] { 1, 4, 1, 4, 4 };
    }

    [WinFormsTheory]
    [MemberData(nameof(SetRange_TestData))]
    public void TrackBar_SetRange_Invoke_Success(int minValue, int maxValue, int expectedMinimum, int expectedMaximum, int expectedValue)
    {
        using TrackBar control = new()
        {
            Value = 5
        };

        control.SetRange(minValue, maxValue);
        Assert.Equal(expectedMinimum, control.Minimum);
        Assert.Equal(expectedMaximum, control.Maximum);
        Assert.Equal(expectedValue, control.Value);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> SetRange_WithRange_TestData()
    {
        yield return new object[] { 0, 10, 0, 10, 5, 0 };
        yield return new object[] { 0, 9, 0, 9, 5, 1 };
        yield return new object[] { 1, 10, 1, 10, 5, 1 };
        yield return new object[] { 10, 0, 10, 10, 10, 1 };
        yield return new object[] { 1, 9, 1, 9, 5, 1 };
        yield return new object[] { 5, 9, 5, 9, 5, 1 };
        yield return new object[] { 6, 9, 6, 9, 6, 1 };
        yield return new object[] { 1, 5, 1, 5, 5, 1 };
        yield return new object[] { 1, 4, 1, 4, 4, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(SetRange_WithRange_TestData))]
    public void TrackBar_SetRange_InvokeWithHandle_Success(int minValue, int maxValue, int expectedMinimum, int expectedMaximum, int expectedValue, int expectedInvalidatedCallCount)
    {
        using TrackBar control = new()
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

        control.SetRange(minValue, maxValue);
        Assert.Equal(expectedMinimum, control.Minimum);
        Assert.Equal(expectedMaximum, control.Maximum);
        Assert.Equal(expectedValue, control.Value);
        Assert.Equal(expectedMinimum, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETRANGEMIN));
        Assert.Equal(expectedMaximum, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETRANGEMAX));
        Assert.Equal(expectedValue, (int)PInvokeCore.SendMessage(control, PInvoke.TBM_GETPOS));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TrackBar_ToString_Invoke_ReturnsExpected()
    {
        using TrackBar control = new();
        Assert.Equal("System.Windows.Forms.TrackBar, Minimum: 0, Maximum: 10, Value: 0", control.ToString());
    }

    [WinFormsFact]
    public void TrackBar_WndProc_InvokeMouseHoverWithHandle_Success()
    {
        using SubTrackBar control = new();
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

    public static IEnumerable<object[]> WndProc_Scroll_TestData()
    {
        yield return new object[] { MessageId.WM_REFLECT | PInvokeCore.WM_HSCROLL, IntPtr.Zero };
        yield return new object[] { MessageId.WM_REFLECT | PInvokeCore.WM_HSCROLL, PARAM.FromLowHigh(-1, int.MaxValue) };
        yield return new object[] { MessageId.WM_REFLECT | PInvokeCore.WM_HSCROLL, PARAM.FromLowHigh(0, int.MaxValue) };
        yield return new object[] { MessageId.WM_REFLECT | PInvokeCore.WM_HSCROLL, PARAM.FromLowHigh(1, int.MaxValue) };
        yield return new object[] { MessageId.WM_REFLECT | PInvokeCore.WM_HSCROLL, PARAM.FromLowHigh(2, int.MaxValue) };
        yield return new object[] { MessageId.WM_REFLECT | PInvokeCore.WM_HSCROLL, PARAM.FromLowHigh(3, int.MaxValue) };
        yield return new object[] { MessageId.WM_REFLECT | PInvokeCore.WM_HSCROLL, PARAM.FromLowHigh(4, int.MaxValue) };
        yield return new object[] { MessageId.WM_REFLECT | PInvokeCore.WM_HSCROLL, PARAM.FromLowHigh(5, int.MaxValue) };
        yield return new object[] { MessageId.WM_REFLECT | PInvokeCore.WM_HSCROLL, PARAM.FromLowHigh(6, int.MaxValue) };
        yield return new object[] { MessageId.WM_REFLECT | PInvokeCore.WM_HSCROLL, PARAM.FromLowHigh(7, int.MaxValue) };
        yield return new object[] { MessageId.WM_REFLECT | PInvokeCore.WM_HSCROLL, PARAM.FromLowHigh(8, int.MaxValue) };
        yield return new object[] { MessageId.WM_REFLECT | PInvokeCore.WM_HSCROLL, PARAM.FromLowHigh(9, int.MaxValue) };

        yield return new object[] { MessageId.WM_REFLECT | PInvokeCore.WM_VSCROLL, IntPtr.Zero };
        yield return new object[] { MessageId.WM_REFLECT | PInvokeCore.WM_VSCROLL, PARAM.FromLowHigh(-1, int.MaxValue) };
        yield return new object[] { MessageId.WM_REFLECT | PInvokeCore.WM_VSCROLL, PARAM.FromLowHigh(0, int.MaxValue) };
        yield return new object[] { MessageId.WM_REFLECT | PInvokeCore.WM_VSCROLL, PARAM.FromLowHigh(1, int.MaxValue) };
        yield return new object[] { MessageId.WM_REFLECT | PInvokeCore.WM_VSCROLL, PARAM.FromLowHigh(2, int.MaxValue) };
        yield return new object[] { MessageId.WM_REFLECT | PInvokeCore.WM_VSCROLL, PARAM.FromLowHigh(3, int.MaxValue) };
        yield return new object[] { MessageId.WM_REFLECT | PInvokeCore.WM_VSCROLL, PARAM.FromLowHigh(4, int.MaxValue) };
        yield return new object[] { MessageId.WM_REFLECT | PInvokeCore.WM_VSCROLL, PARAM.FromLowHigh(5, int.MaxValue) };
        yield return new object[] { MessageId.WM_REFLECT | PInvokeCore.WM_VSCROLL, PARAM.FromLowHigh(6, int.MaxValue) };
        yield return new object[] { MessageId.WM_REFLECT | PInvokeCore.WM_VSCROLL, PARAM.FromLowHigh(7, int.MaxValue) };
        yield return new object[] { MessageId.WM_REFLECT | PInvokeCore.WM_VSCROLL, PARAM.FromLowHigh(8, int.MaxValue) };
        yield return new object[] { MessageId.WM_REFLECT | PInvokeCore.WM_VSCROLL, PARAM.FromLowHigh(9, int.MaxValue) };
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_Scroll_TestData))]
    public void TrackBar_WndProc_InvokeScrollWithHandle_Success(int msg, IntPtr wParam)
    {
        using SubTrackBar control = new()
        {
            Value = 10
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int scrollCallCount = 0;
        control.Scroll += (sender, e) => scrollCallCount++;
        int valueChangedCallCount = 0;
        control.ValueChanged += (sender, e) => valueChangedCallCount++;
        Message m = new()
        {
            Msg = msg,
            WParam = wParam,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(250, m.Result);
        Assert.Equal(10, control.Value);
        Assert.Equal(0, scrollCallCount);
        Assert.Equal(0, valueChangedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TrackBar_AutoSizeChangedEvent_AddRemove_Success()
    {
        using TrackBar control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().BeSameAs(control);
            e.Should().BeSameAs(EventArgs.Empty);
            callCount++;
        };

        control.AutoSizeChanged += handler;
        control.AutoSize = !control.AutoSize;
        callCount.Should().Be(1);

        control.AutoSizeChanged -= handler;
        control.AutoSize = !control.AutoSize;
        callCount.Should().Be(1);
    }

    [WinFormsTheory]
    [InlineData(100, 200, 0, 0, 50, 100)]
    [InlineData(200, 300, 10, 20, 70, 140)]
    [InlineData(300, 400, -10, -20, 70, 140)]
    [InlineData(400, 500, 0, 0, 1, 1)]
    [InlineData(500, 600, 0, 0, 500, 600)]
    public void TrackBar_PaintEvent_AddRemove_Success(int bitmapWidth, int bitmapHeight, int rectX, int rectY, int rectWidth, int rectHeight)
    {
        using Bitmap bitmap = new(bitmapWidth, bitmapHeight);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Rectangle rectangle = new(rectX, rectY, rectWidth, rectHeight);

        using SubTrackBar trackBar = new();
        int callCount = 0;
        PaintEventHandler handler = (sender, e) =>
        {
            sender.Should().BeSameAs(trackBar);
            e.Graphics.Should().BeSameAs(graphics);
            e.ClipRectangle.Should().Be(rectangle);
            callCount++;
        };

        trackBar.Paint += handler;
        using (var eventArgs = new PaintEventArgs(graphics, rectangle))
        {
            trackBar.OnPaint(eventArgs);
        }

        callCount.Should().Be(1);

        callCount = 0;
        trackBar.Paint -= handler;
        using (var eventArgs = new PaintEventArgs(graphics, rectangle))
        {
            trackBar.OnPaint(eventArgs);
        }

        trackBar.Invalidate();
        callCount.Should().Be(0);
    }

    public class SubTrackBar : TrackBar
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

        public new void CreateHandle() => base.CreateHandle();

        public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

        public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

        public new bool GetTopLevel() => base.GetTopLevel();

        public new bool IsInputKey(Keys keyData) => base.IsInputKey(keyData);

        public new void OnBackColorChanged(EventArgs e) => base.OnBackColorChanged(e);

        public new void OnClick(EventArgs e) => base.OnClick(e);

        public new void OnDoubleClick(EventArgs e) => base.OnDoubleClick(e);

        public new void OnForeColorChanged(EventArgs e) => base.OnForeColorChanged(e);

        public new void OnHandleCreated(EventArgs e) => base.OnHandleCreated(e);

        public new void OnHandleDestroyed(EventArgs e) => base.OnHandleDestroyed(e);

        public new void OnMouseClick(MouseEventArgs e) => base.OnMouseClick(e);

        public new void OnMouseDoubleClick(MouseEventArgs e) => base.OnMouseDoubleClick(e);

        public new void OnMouseWheel(MouseEventArgs e) => base.OnMouseWheel(e);

        public new void OnPaint(PaintEventArgs e) => base.OnPaint(e);

        public new void OnRightToLeftLayoutChanged(EventArgs e) => base.OnRightToLeftLayoutChanged(e);

        public new void OnScroll(EventArgs e) => base.OnScroll(e);

        public new void OnSystemColorsChanged(EventArgs e) => base.OnSystemColorsChanged(e);

        public new void OnValueChanged(EventArgs e) => base.OnValueChanged(e);

        public new void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) => base.SetBoundsCore(x, y, width, height, specified);

        public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);

        public new void WndProc(ref Message m) => base.WndProc(ref m);
    }
}
