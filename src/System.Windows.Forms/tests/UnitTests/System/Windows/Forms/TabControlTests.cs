// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.TestUtilities;
using Moq;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Tests;

public class TabControlTests
{
    [WinFormsFact]
    public void TabControl_Ctor_Default()
    {
        using SubTabControl control = new();
        Assert.Null(control.AccessibleDefaultActionDescription);
        Assert.Null(control.AccessibleDescription);
        Assert.Null(control.AccessibleName);
        Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
        Assert.Equal(TabAlignment.Top, control.Alignment);
        Assert.False(control.AllowDrop);
        Assert.Equal(TabAppearance.Normal, control.Appearance);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        Assert.False(control.AutoSize);
        Assert.Equal(Control.DefaultBackColor, control.BackColor);
        Assert.Null(control.BackgroundImage);
        Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
        Assert.Null(control.BindingContext);
        Assert.Equal(100, control.Bottom);
        Assert.Equal(new Rectangle(0, 0, 200, 100), control.Bounds);
        Assert.True(control.CanEnableIme);
        Assert.False(control.CanFocus);
        Assert.True(control.CanRaiseEvents);
        Assert.True(control.CanSelect);
        Assert.False(control.Capture);
        Assert.True(control.CausesValidation);
        Assert.Equal(new Rectangle(0, 0, 200, 100), control.ClientRectangle);
        Assert.Equal(new Size(200, 100), control.ClientSize);
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
        Assert.Equal(new Size(200, 100), control.DefaultSize);
        Assert.False(control.DesignMode);
        Assert.Equal(DockStyle.None, control.Dock);
        Assert.False(control.DoubleBuffered);
        Assert.Equal(TabDrawMode.Normal, control.DrawMode);
        Assert.True(control.Enabled);
        Assert.NotNull(control.Events);
        Assert.Same(control.Events, control.Events);
        Assert.False(control.Focused);
        Assert.Equal(Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.Equal(Control.DefaultForeColor, control.ForeColor);
        Assert.False(control.HasChildren);
        Assert.Equal(100, control.Height);
        Assert.False(control.HotTrack);
        Assert.Null(control.ImageList);
        Assert.Equal(ImeMode.NoControl, control.ImeMode);
        Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
        Assert.False(control.IsAccessible);
        Assert.False(control.IsMirrored);
        Assert.Equal(Size.Empty, control.ItemSize);
        Assert.NotNull(control.LayoutEngine);
        Assert.Same(control.LayoutEngine, control.LayoutEngine);
        Assert.Equal(0, control.Left);
        Assert.Equal(Point.Empty, control.Location);
        Assert.Equal(new Padding(3), control.Margin);
        Assert.Equal(Size.Empty, control.MaximumSize);
        Assert.Equal(Size.Empty, control.MinimumSize);
        Assert.False(control.Multiline);
        Assert.Equal(new Point(6, 3), control.Padding);
        Assert.Null(control.Parent);
        Assert.Equal(new Size(200, 100), control.PreferredSize);
        Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
        Assert.False(control.RecreatingHandle);
        Assert.Null(control.Region);
        Assert.False(control.ResizeRedraw);
        Assert.Equal(200, control.Right);
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.False(control.RightToLeftLayout);
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Null(control.SelectedTab);
        Assert.True(control.ShowFocusCues);
        Assert.True(control.ShowKeyboardCues);
        Assert.False(control.ShowToolTips);
        Assert.Null(control.Site);
        Assert.Equal(new Size(200, 100), control.Size);
        Assert.Equal(TabSizeMode.Normal, control.SizeMode);
        Assert.Equal(0, control.TabCount);
        Assert.Equal(0, control.TabIndex);
        Assert.Empty(control.TabPages);
        Assert.Same(control.TabPages, control.TabPages);
        Assert.True(control.TabStop);
        Assert.Empty(control.Text);
        Assert.Equal(0, control.Top);
        Assert.Null(control.TopLevelControl);
        Assert.False(control.UseWaitCursor);
        Assert.True(control.Visible);
        Assert.Equal(200, control.Width);

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabControl_CreateParams_GetDefault_ReturnsExpected()
    {
        using SubTabControl control = new();
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Equal("SysTabControl32", createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0, createParams.ExStyle);
        Assert.Equal(100, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(0x56010800, createParams.Style);
        Assert.Equal(200, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 0x56010A00)]
    [InlineData(false, 0x56010800)]
    public void TabControl_CreateParams_GetMultiline_ReturnsExpected(bool multiline, int expectedStyle)
    {
        using SubTabControl control = new()
        {
            Multiline = multiline
        };
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Equal("SysTabControl32", createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0, createParams.ExStyle);
        Assert.Equal(100, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(expectedStyle, createParams.Style);
        Assert.Equal(200, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
    }

    [WinFormsTheory]
    [InlineData(TabDrawMode.Normal, 0x56010800)]
    [InlineData(TabDrawMode.OwnerDrawFixed, 0x56012800)]
    public void TabControl_CreateParams_GetDrawMode_ReturnsExpected(TabDrawMode drawMode, int expectedStyle)
    {
        using SubTabControl control = new()
        {
            DrawMode = drawMode
        };
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Equal("SysTabControl32", createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0, createParams.ExStyle);
        Assert.Equal(100, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(expectedStyle, createParams.Style);
        Assert.Equal(200, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
    }

    [WinFormsTheory]
    [InlineData(true, true, 0x56010800)]
    [InlineData(false, true, 0x56010800)]
    [InlineData(true, false, 0x56014800)]
    [InlineData(false, false, 0x56010800)]
    public void TabControl_CreateParams_GetShowToolTips_ReturnsExpected(bool ShowToolTips, bool designMode, int expectedStyle)
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
        using SubTabControl control = new()
        {
            ShowToolTips = ShowToolTips,
            Site = mockSite.Object
        };
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Equal("SysTabControl32", createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0, createParams.ExStyle);
        Assert.Equal(100, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(expectedStyle, createParams.Style);
        Assert.Equal(200, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
    }

    [WinFormsTheory]
    [InlineData(TabAlignment.Bottom, 0x56010802)]
    [InlineData(TabAlignment.Left, 0x56010A80)]
    [InlineData(TabAlignment.Right, 0x56010A82)]
    [InlineData(TabAlignment.Top, 0x56010800)]
    public void TabControl_CreateParams_GetAlignment_ReturnsExpected(TabAlignment alignment, int expectedStyle)
    {
        using SubTabControl control = new()
        {
            Alignment = alignment
        };
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Equal("SysTabControl32", createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0, createParams.ExStyle);
        Assert.Equal(100, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(expectedStyle, createParams.Style);
        Assert.Equal(200, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
    }

    [WinFormsTheory]
    [InlineData(true, 0x56010840)]
    [InlineData(false, 0x56010800)]
    public void TabControl_CreateParams_GetHotTrack_ReturnsExpected(bool hotTrack, int expectedStyle)
    {
        using SubTabControl control = new()
        {
            HotTrack = hotTrack
        };
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Equal("SysTabControl32", createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0, createParams.ExStyle);
        Assert.Equal(100, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(expectedStyle, createParams.Style);
        Assert.Equal(200, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
    }

    [WinFormsTheory]
    [InlineData(TabAppearance.Normal, TabAlignment.Bottom, 0x56010802)]
    [InlineData(TabAppearance.Normal, TabAlignment.Left, 0x56010A80)]
    [InlineData(TabAppearance.Normal, TabAlignment.Right, 0x56010A82)]
    [InlineData(TabAppearance.Normal, TabAlignment.Top, 0x56010800)]
    [InlineData(TabAppearance.Buttons, TabAlignment.Bottom, 0x56010902)]
    [InlineData(TabAppearance.Buttons, TabAlignment.Left, 0x56010B80)]
    [InlineData(TabAppearance.Buttons, TabAlignment.Right, 0x56010B82)]
    [InlineData(TabAppearance.Buttons, TabAlignment.Top, 0x56010900)]
    [InlineData(TabAppearance.FlatButtons, TabAlignment.Bottom, 0x56010902)]
    [InlineData(TabAppearance.FlatButtons, TabAlignment.Left, 0x56010B80)]
    [InlineData(TabAppearance.FlatButtons, TabAlignment.Right, 0x56010B82)]
    [InlineData(TabAppearance.FlatButtons, TabAlignment.Top, 0x56010908)]
    public void TabControl_CreateParams_GetAppearance_ReturnsExpected(TabAppearance appearance, TabAlignment alignment, int expectedStyle)
    {
        using SubTabControl control = new()
        {
            Appearance = appearance,
            Alignment = alignment
        };
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Equal("SysTabControl32", createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0, createParams.ExStyle);
        Assert.Equal(100, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(expectedStyle, createParams.Style);
        Assert.Equal(200, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
    }

    [WinFormsTheory]
    [InlineData(TabSizeMode.FillToRight, 0x56010000)]
    [InlineData(TabSizeMode.Fixed, 0x56010400)]
    [InlineData(TabSizeMode.Normal, 0x56010800)]
    public void TabControl_CreateParams_GetSizeMode_ReturnsExpected(TabSizeMode sizeMode, int expectedStyle)
    {
        using SubTabControl control = new()
        {
            SizeMode = sizeMode
        };
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Equal("SysTabControl32", createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0, createParams.ExStyle);
        Assert.Equal(100, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(expectedStyle, createParams.Style);
        Assert.Equal(200, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.Inherit, true, 0x56010800, 0x0)]
    [InlineData(RightToLeft.No, true, 0x56010800, 0x0)]
    [InlineData(RightToLeft.Yes, true, 0x56010800, 0x500000)]
    [InlineData(RightToLeft.Inherit, false, 0x56010800, 0x0)]
    [InlineData(RightToLeft.No, false, 0x56010800, 0x0)]
    [InlineData(RightToLeft.Yes, false, 0x56010800, 0x7000)]
    public void TabControl_CreateParams_GetRightToLeft_ReturnsExpected(RightToLeft rightToLeft, bool rightToLeftLayout, int expectedStyle, int expectedExStyle)
    {
        using SubTabControl control = new()
        {
            RightToLeft = rightToLeft,
            RightToLeftLayout = rightToLeftLayout
        };
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Equal("SysTabControl32", createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(expectedExStyle, createParams.ExStyle);
        Assert.Equal(100, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(expectedStyle, createParams.Style);
        Assert.Equal(200, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
    }

    public static IEnumerable<object[]> Alignment_Set_TestData()
    {
        yield return new object[] { true, TabAlignment.Top, true };
        yield return new object[] { true, TabAlignment.Bottom, true };
        yield return new object[] { true, TabAlignment.Left, true };
        yield return new object[] { true, TabAlignment.Right, true };

        yield return new object[] { false, TabAlignment.Top, false };
        yield return new object[] { false, TabAlignment.Bottom, false };
        yield return new object[] { false, TabAlignment.Left, true };
        yield return new object[] { false, TabAlignment.Right, true };
    }

    [WinFormsTheory]
    [MemberData(nameof(Alignment_Set_TestData))]
    public void TabControl_Alignment_Set_GetReturnsExpected(bool multiline, TabAlignment value, bool expectedMultiline)
    {
        using TabControl control = new()
        {
            Multiline = multiline,
            Alignment = value
        };
        Assert.Equal(value, control.Alignment);
        Assert.Equal(expectedMultiline, control.Multiline);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Alignment = value;
        Assert.Equal(value, control.Alignment);
        Assert.Equal(expectedMultiline, control.Multiline);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Alignment_SetWithHandle_TestData()
    {
        yield return new object[] { true, TabAlignment.Top, true, 0 };
        yield return new object[] { true, TabAlignment.Bottom, true, 1 };
        yield return new object[] { true, TabAlignment.Left, true, 1 };
        yield return new object[] { true, TabAlignment.Right, true, 1 };

        yield return new object[] { false, TabAlignment.Top, false, 0 };
        yield return new object[] { false, TabAlignment.Bottom, false, 1 };
        yield return new object[] { false, TabAlignment.Left, true, 1 };
        yield return new object[] { false, TabAlignment.Right, true, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Alignment_SetWithHandle_TestData))]
    public void TabControl_Alignment_SetWithHandle_GetReturnsExpected(bool multiline, TabAlignment value, bool expectedMultiline, int expectedCreatedCallCount)
    {
        using TabControl control = new()
        {
            Multiline = multiline
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Alignment = value;
        Assert.Equal(value, control.Alignment);
        Assert.Equal(expectedMultiline, control.Multiline);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedCreatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        control.Alignment = value;
        Assert.Equal(value, control.Alignment);
        Assert.Equal(expectedMultiline, control.Multiline);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedCreatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<TabAlignment>]
    public void TabControl_Alignment_SetInvalidValue_ThrowsInvalidEnumArgumentException(TabAlignment value)
    {
        using TabControl control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.Alignment = value);
    }

    [WinFormsTheory]
    [InlineData(TabAlignment.Bottom)]
    [InlineData(TabAlignment.Left)]
    [InlineData(TabAlignment.Right)]
    public void TabControl_Appearance_GetFlatButtonsWithAlignment_ReturnsExpected(TabAlignment alignment)
    {
        using TabControl control = new()
        {
            Appearance = TabAppearance.FlatButtons,
            Alignment = alignment
        };
        Assert.Equal(TabAppearance.Buttons, control.Appearance);
        Assert.Equal(alignment, control.Alignment);
        Assert.False(control.IsHandleCreated);

        control.Alignment = TabAlignment.Top;
        Assert.Equal(TabAppearance.FlatButtons, control.Appearance);
        Assert.Equal(TabAlignment.Top, control.Alignment);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [EnumData<TabAppearance>]
    public void TabControl_Appearance_Set_GetReturnsExpected(TabAppearance value)
    {
        using TabControl control = new()
        {
            Appearance = value
        };
        Assert.Equal(value, control.Appearance);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Appearance = value;
        Assert.Equal(value, control.Appearance);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(TabAppearance.Normal, 0)]
    [InlineData(TabAppearance.Buttons, 1)]
    [InlineData(TabAppearance.FlatButtons, 1)]
    public void TabControl_Appearance_SetWithHandle_GetReturnsExpected(TabAppearance value, int expectedCreatedCallCount)
    {
        using TabControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Appearance = value;
        Assert.Equal(value, control.Appearance);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedCreatedCallCount, invalidatedCallCount);
        Assert.Equal(expectedCreatedCallCount, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        control.Appearance = value;
        Assert.Equal(value, control.Appearance);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedCreatedCallCount, invalidatedCallCount);
        Assert.Equal(expectedCreatedCallCount, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<TabAppearance>]
    public void TabControl_Appearance_SetInvalidValue_ThrowsInvalidEnumArgumentException(TabAppearance value)
    {
        using TabControl control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.Appearance = value);
    }

    public static IEnumerable<object[]> BackColor_Set_TestData()
    {
        yield return new object[] { Color.Red };
        yield return new object[] { Color.FromArgb(254, 1, 2, 3) };
        yield return new object[] { Color.Empty };
    }

    [WinFormsTheory]
    [MemberData(nameof(BackColor_Set_TestData))]
    public void TabControl_BackColor_Set_GetReturnsExpected(Color value)
    {
        using TabControl control = new()
        {
            BackColor = value
        };
        Assert.Equal(SystemColors.Control, control.BackColor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BackColor = value;
        Assert.Equal(SystemColors.Control, control.BackColor);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(BackColor_Set_TestData))]
    public void TabControl_BackColor_SetWithHandle_GetReturnsExpected(Color value)
    {
        using TabControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.BackColor = value;
        Assert.Equal(SystemColors.Control, control.BackColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.BackColor = value;
        Assert.Equal(SystemColors.Control, control.BackColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TabControl_BackColor_SetWithHandler_DoesNotCallBackColorChanged()
    {
        using TabControl control = new();
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        }

        control.BackColorChanged += handler;

        // Set different.
        control.BackColor = Color.Red;
        Assert.Equal(SystemColors.Control, control.BackColor);
        Assert.Equal(0, callCount);

        // Set same.
        control.BackColor = Color.Red;
        Assert.Equal(SystemColors.Control, control.BackColor);
        Assert.Equal(0, callCount);

        // Set different.
        control.BackColor = Color.Empty;
        Assert.Equal(SystemColors.Control, control.BackColor);
        Assert.Equal(0, callCount);

        // Remove handler.
        control.BackColorChanged -= handler;
        control.BackColor = Color.Red;
        Assert.Equal(SystemColors.Control, control.BackColor);
        Assert.Equal(0, callCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetImageTheoryData))]
    public void TabControl_BackgroundImage_Set_GetReturnsExpected(Image value)
    {
        using TabControl control = new()
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
    public void TabControl_BackgroundImage_SetWithHandler_CallsBackgroundImageChanged()
    {
        using TabControl control = new();
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
    public void TabControl_BackgroundImageLayout_Set_GetReturnsExpected(ImageLayout value)
    {
        using SubTabControl control = new()
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
    public void TabControl_BackgroundImageLayout_SetWithHandler_CallsBackgroundImageLayoutChanged()
    {
        using TabControl control = new();
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
    public void TabControl_BackgroundImageLayout_SetInvalid_ThrowsInvalidEnumArgumentException(ImageLayout value)
    {
        using TabControl control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.BackgroundImageLayout = value);
    }

    [WinFormsFact]
    public void TabControl_DisplayRectangle_Get_ReturnsExpectedAndCreatesHandle()
    {
        using TabControl control = new();
        Rectangle displayRectangle = control.DisplayRectangle;
        Assert.True(displayRectangle.X >= 0);
        Assert.True(displayRectangle.Y >= 0);
        Assert.Equal(200 - displayRectangle.X * 2, control.DisplayRectangle.Width);
        Assert.Equal(100 - displayRectangle.Y * 2, control.DisplayRectangle.Height);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(displayRectangle, control.DisplayRectangle);
    }

    [WinFormsFact]
    public void TabControl_DisplayRectangle_GetWithHandle_ReturnsExpected()
    {
        using TabControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Rectangle displayRectangle = control.DisplayRectangle;
        Assert.True(displayRectangle.X >= 0);
        Assert.True(displayRectangle.Y >= 0);
        Assert.Equal(200 - displayRectangle.X * 2, control.DisplayRectangle.Width);
        Assert.Equal(100 - displayRectangle.Y * 2, control.DisplayRectangle.Height);
        Assert.Equal(displayRectangle, control.DisplayRectangle);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TabControl_DisplayRectangle_GetDisposed_ReturnsExpected()
    {
        using TabControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.Dispose();

        Rectangle displayRectangle = control.DisplayRectangle;
        Assert.True(displayRectangle.X >= 0);
        Assert.True(displayRectangle.Y >= 0);
        Assert.Equal(200 - displayRectangle.X * 2, control.DisplayRectangle.Width);
        Assert.Equal(100 - displayRectangle.Y * 2, control.DisplayRectangle.Height);
        Assert.False(control.IsHandleCreated);
        Assert.Equal(displayRectangle, control.DisplayRectangle);
    }

    [WinFormsTheory]
    [BoolData]
    public void TabControl_DoubleBuffered_Get_ReturnsExpected(bool value)
    {
        using SubTabControl control = new();
        control.SetStyle(ControlStyles.OptimizedDoubleBuffer, value);
        Assert.Equal(value, control.DoubleBuffered);
    }

    [WinFormsTheory]
    [BoolData]
    public void TabControl_DoubleBuffered_Set_GetReturnsExpected(bool value)
    {
        using SubTabControl control = new()
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
    public void TabControl_DoubleBuffered_SetWithHandle_GetReturnsExpected(bool value)
    {
        using SubTabControl control = new();
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
    [EnumData<TabDrawMode>]
    public void TabControl_DrawMode_Set_GetReturnsExpected(TabDrawMode value)
    {
        using TabControl control = new()
        {
            DrawMode = value
        };
        Assert.Equal(value, control.DrawMode);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.DrawMode = value;
        Assert.Equal(value, control.DrawMode);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(TabDrawMode.Normal, 0)]
    [InlineData(TabDrawMode.OwnerDrawFixed, 1)]
    public void TabControl_DrawMode_SetWithHandle_GetReturnsExpected(TabDrawMode value, int expectedCreatedCallCount)
    {
        using TabControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.DrawMode = value;
        Assert.Equal(value, control.DrawMode);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedCreatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        control.DrawMode = value;
        Assert.Equal(value, control.DrawMode);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedCreatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<TabDrawMode>]
    public void TabControl_DrawMode_SetInvalidValue_ThrowsInvalidEnumArgumentException(TabDrawMode value)
    {
        using TabControl control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.DrawMode = value);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetForeColorTheoryData))]
    public void TabControl_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using TabControl control = new()
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
        yield return new object[] { Color.Empty, Control.DefaultForeColor, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(ForeColor_SetWithHandle_TestData))]
    public void TabControl_ForeColor_SetWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
    {
        using TabControl control = new();
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
    public void TabControl_ForeColor_SetWithHandler_CallsForeColorChanged()
    {
        using TabControl control = new();
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        }

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
    public void TabControl_Handle_GetNoImageList_Success()
    {
        using TabControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(control, PInvoke.TCM_GETIMAGELIST));
    }

    [WinFormsFact]
    public void TabControl_Handle_GetWithImageList_Success()
    {
        using ImageList imageList = new();
        using TabControl control = new()
        {
            ImageList = imageList
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(imageList.Handle, (nint)PInvokeCore.SendMessage(control, PInvoke.TCM_GETIMAGELIST));
    }

    [WinFormsFact]
    public void TabControl_Handle_GetItemsEmpty_Success()
    {
        using TabControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(control, PInvoke.TCM_GETITEMCOUNT));
    }

    [WinFormsTheory]
    [InlineData("Text", "Text")]
    [InlineData("&&Text", "&&Text")]
    [InlineData("&", "&&")]
    [InlineData("&Text", "&&Text")]
    public unsafe void TabControl_Handle_GetItems_Success(string text, string expectedText)
    {
        using TabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new()
        {
            Text = text,
            ImageIndex = 1
        };
        using NullTextTabPage page3 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        control.TabPages.Add(page3);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(control, PInvoke.TCM_GETITEMCOUNT));

        char* buffer = stackalloc char[256];
        TCITEMW item = default;
        item.cchTextMax = int.MaxValue;
        item.pszText = buffer;
        item.dwStateMask = (TAB_CONTROL_ITEM_STATE)uint.MaxValue;
        item.mask = (TCITEMHEADERA_MASK)uint.MaxValue;

        // Get item 0.
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.TCM_GETITEMW, 0, ref item));
        Assert.Equal(TAB_CONTROL_ITEM_STATE.TCIS_BUTTONPRESSED, item.dwState);
        Assert.Equal(IntPtr.Zero, (nint)item.lParam);
        Assert.Equal(int.MaxValue, item.cchTextMax);
        Assert.Empty(new string(item.pszText));
        Assert.Equal(-1, item.iImage);

        // Get item 1.
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.TCM_GETITEMW, 1, ref item));
        Assert.Equal((TAB_CONTROL_ITEM_STATE)0, item.dwState);
        Assert.Equal(IntPtr.Zero, (nint)item.lParam);
        Assert.Equal(int.MaxValue, item.cchTextMax);
        Assert.Equal(expectedText, new string(item.pszText));
        Assert.Equal(1, item.iImage);

        // Get item 2.
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.TCM_GETITEMW, 2, ref item));
        Assert.Equal((TAB_CONTROL_ITEM_STATE)0, item.dwState);
        Assert.Equal(IntPtr.Zero, (nint)item.lParam);
        Assert.Equal(int.MaxValue, item.cchTextMax);
        Assert.Empty(new string(item.pszText));
        Assert.Equal(-1, item.iImage);
    }

    [WinFormsTheory]
    [BoolData]
    public void TabControl_HotTrack_Set_GetReturnsExpected(bool value)
    {
        using SubTabControl control = new()
        {
            HotTrack = value
        };
        Assert.Equal(value, control.HotTrack);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.HotTrack = value;
        Assert.Equal(value, control.HotTrack);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.HotTrack = !value;
        Assert.Equal(!value, control.HotTrack);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void TabControl_HotTrack_SetWithHandle_GetReturnsExpected(bool value, int expectedCreatedCallCount)
    {
        using SubTabControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.HotTrack = value;
        Assert.Equal(value, control.HotTrack);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedCreatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        control.HotTrack = value;
        Assert.Equal(value, control.HotTrack);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedCreatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set different.
        control.HotTrack = !value;
        Assert.Equal(!value, control.HotTrack);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedCreatedCallCount + 1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount + 1, createdCallCount);
    }

    public static IEnumerable<object[]> ImageList_Set_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new ImageList() };
    }

    [WinFormsTheory]
    [MemberData(nameof(ImageList_Set_TestData))]
    public void TabControl_ImageList_Set_GetReturnsExpected(ImageList value)
    {
        using TabControl control = new()
        {
            ImageList = value
        };
        Assert.Same(value, control.ImageList);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ImageList = value;
        Assert.Same(value, control.ImageList);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ImageList_Set_TestData))]
    public void TabControl_ImageList_SetWithNonNullOldValue_GetReturnsExpected(ImageList value)
    {
        using ImageList oldValue = new();
        using TabControl control = new()
        {
            ImageList = oldValue
        };

        control.ImageList = value;
        Assert.Same(value, control.ImageList);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ImageList = value;
        Assert.Same(value, control.ImageList);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ImageList_Set_TestData))]
    public void TabControl_ImageList_SetWithHandle_GetReturnsExpected(ImageList value)
    {
        using TabControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ImageList = value;
        Assert.Same(value, control.ImageList);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.ImageList = value;
        Assert.Same(value, control.ImageList);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(ImageList_Set_TestData))]
    public void TabControl_ImageList_SetWithHandleWithNonNullOldValue_GetReturnsExpected(ImageList value)
    {
        using ImageList oldValue = new();
        using TabControl control = new()
        {
            ImageList = oldValue
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ImageList = value;
        Assert.Same(value, control.ImageList);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.ImageList = value;
        Assert.Same(value, control.ImageList);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TabControl_ImageList_SetWithTabPages_Success()
    {
        using TabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);

        using ImageList imageList = new();
        using Bitmap image1 = new(10, 10);
        using Bitmap image2 = new(10, 10);
        using Bitmap image3 = new(10, 10);
        imageList.Images.Add(image1);
        imageList.Images.Add(image2);
        imageList.Images.Add(image3);
        control.ImageList = imageList;
        Assert.Same(imageList, control.ImageList);
        Assert.False(control.IsHandleCreated);

        // Set null.
        control.ImageList = null;
        Assert.Null(control.ImageList);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabControl_ImageList_Set_CreatesImageHandle()
    {
        using TabControl control = new();
        using ImageList imageList = new();
        control.ImageList = imageList;
        Assert.True(imageList.HandleCreated);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabControl_ImageList_SetGetImageListWithHandle_Success()
    {
        using TabControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        // Set non-null.
        using ImageList imageList = new();
        control.ImageList = imageList;
        Assert.True(imageList.HandleCreated);
        Assert.Equal(imageList.Handle, (nint)PInvokeCore.SendMessage(control, PInvoke.TCM_GETIMAGELIST));

        // Set null.
        control.ImageList = null;
        Assert.Equal(0, (int)PInvokeCore.SendMessage(control, PInvoke.TCM_GETIMAGELIST));
    }

    [WinFormsFact]
    public void TabControl_ImageList_Dispose_DetachesFromTabControl()
    {
        using ImageList imageList1 = new();
        using ImageList imageList2 = new();
        using TabControl control = new()
        {
            ImageList = imageList1
        };
        Assert.Same(imageList1, control.ImageList);

        imageList1.Dispose();
        Assert.Null(control.ImageList);
        Assert.False(control.IsHandleCreated);

        // Make sure we detached the setter.
        control.ImageList = imageList2;
        imageList1.Dispose();
        Assert.Same(imageList2, control.ImageList);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabControl_ImageList_DisposeWithHandle_DetachesFromTabControl()
    {
        using ImageList imageList1 = new();
        using ImageList imageList2 = new();
        using TabControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ImageList = imageList1;
        Assert.Same(imageList1, control.ImageList);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        imageList1.Dispose();
        Assert.Null(control.ImageList);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Make sure we detached the setter.
        control.ImageList = imageList2;
        imageList1.Dispose();
        Assert.Same(imageList2, control.ImageList);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TabControl_ImageList_RecreateHandle_Nop()
    {
        using ImageList imageList1 = new();
        int recreateCallCount1 = 0;
        imageList1.RecreateHandle += (sender, e) => recreateCallCount1++;
        using ImageList imageList2 = new();
        using TabControl control = new()
        {
            ImageList = imageList1
        };
        Assert.Same(imageList1, control.ImageList);
        Assert.Equal(0, recreateCallCount1);

        imageList1.ImageSize = new Size(1, 2);
        Assert.Equal(1, recreateCallCount1);
        Assert.Same(imageList1, control.ImageList);
        Assert.False(control.IsHandleCreated);

        // Make sure we detached the setter.
        control.ImageList = imageList2;
        imageList1.ImageSize = new Size(2, 3);
        Assert.Equal(2, recreateCallCount1);
        Assert.Same(imageList2, control.ImageList);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabControl_ImageList_RecreateHandleWithHandle_Success()
    {
        using ImageList imageList1 = new();
        int recreateCallCount1 = 0;
        imageList1.RecreateHandle += (sender, e) => recreateCallCount1++;
        using ImageList imageList2 = new();
        using TabControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ImageList = imageList1;
        Assert.Same(imageList1, control.ImageList);
        Assert.Equal(0, recreateCallCount1);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        imageList1.ImageSize = new Size(1, 2);
        Assert.Equal(1, recreateCallCount1);
        Assert.Same(imageList1, control.ImageList);
        Assert.Equal(imageList1.Handle, (nint)PInvokeCore.SendMessage(control, PInvoke.TCM_GETIMAGELIST));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Make sure we detached the setter.
        control.ImageList = imageList2;
        imageList1.ImageSize = new Size(2, 3);
        Assert.Equal(2, recreateCallCount1);
        Assert.Same(imageList2, control.ImageList);
        Assert.Equal(imageList2.Handle, (nint)PInvokeCore.SendMessage(control, PInvoke.TCM_GETIMAGELIST));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TabControl_ItemSize_GetEmptyWithHandle_ReturnsExpected()
    {
        using TabControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Size size = control.ItemSize;
        Assert.Equal(0, size.Width);
        Assert.True(size.Height > 0);
        Assert.Equal(size, control.ItemSize);
    }

    [WinFormsFact]
    public void TabControl_ItemSize_GetNotEmptyWithHandle_ReturnsExpected()
    {
        using TabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        Size size = control.ItemSize;
        Assert.True(size.Width > 0);
        Assert.True(size.Height > 0);
        Assert.Equal(size, control.ItemSize);
    }

    public static IEnumerable<object[]> ItemSize_Set_TestData()
    {
        yield return new object[] { new Size(0, 0) };
        yield return new object[] { new Size(0, 16) };
        yield return new object[] { new Size(16, 0) };
        yield return new object[] { new Size(16, 16) };
    }

    [WinFormsTheory]
    [MemberData(nameof(ItemSize_Set_TestData))]
    public void TabControl_ItemSize_Set_GetReturnsExpected(Size value)
    {
        using TabControl control = new()
        {
            ItemSize = value
        };
        Assert.Equal(value, control.ItemSize);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ItemSize = value;
        Assert.Equal(value, control.ItemSize);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> ItemSize_SetWithHandle_TestData()
    {
        yield return new object[] { new Size(0, 16) };
        yield return new object[] { new Size(16, 0) };
        yield return new object[] { new Size(16, 16) };
    }

    [WinFormsTheory]
    [MemberData(nameof(ItemSize_SetWithHandle_TestData))]
    public void TabControl_ItemSize_SetWithHandle_GetReturnsExpected(Size value)
    {
        using TabControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ItemSize = value;
        Assert.Equal(value, control.ItemSize);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.ItemSize = value;
        Assert.Equal(value, control.ItemSize);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(4, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TabControl_SetEmptyWithHandle_GetReturnsExpected()
    {
        using TabControl control = new()
        {
            ItemSize = new Size(16, 16)
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ItemSize = Size.Empty;
        Size size = control.ItemSize;
        Assert.Equal(0, size.Width);
        Assert.True(size.Height > 0);
        Assert.Equal(size, control.ItemSize);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TabControl_ItemSize_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(TabControl))[nameof(TabControl.ItemSize)];
        using TabControl control = new();
        Assert.False(property.CanResetValue(control));

        control.ItemSize = new Size(1, 0);
        Assert.Equal(new Size(1, 0), control.ItemSize);
        Assert.True(property.CanResetValue(control));

        control.ItemSize = new Size(0, 1);
        Assert.Equal(new Size(0, 1), control.ItemSize);
        Assert.True(property.CanResetValue(control));

        control.ItemSize = new Size(1, 2);
        Assert.Equal(new Size(1, 2), control.ItemSize);
        Assert.True(property.CanResetValue(control));

        property.ResetValue(control);
        Assert.Equal(Size.Empty, control.ItemSize);
        Assert.False(property.CanResetValue(control));
    }

    [WinFormsFact]
    public void TabControl_ItemSize_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(TabControl))[nameof(TabControl.ItemSize)];
        using TabControl control = new();
        Assert.False(property.ShouldSerializeValue(control));

        control.ItemSize = new Size(1, 0);
        Assert.Equal(new Size(1, 0), control.ItemSize);
        Assert.True(property.ShouldSerializeValue(control));

        control.ItemSize = new Size(0, 1);
        Assert.Equal(new Size(0, 1), control.ItemSize);
        Assert.True(property.ShouldSerializeValue(control));

        control.ItemSize = new Size(1, 2);
        Assert.Equal(new Size(1, 2), control.ItemSize);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.Equal(Size.Empty, control.ItemSize);
        Assert.False(property.ShouldSerializeValue(control));
    }

    [WinFormsFact]
    public void TabControl_ItemSize_SetNegativeWidth_ThrowsArgumentOutOfRangeException()
    {
        using TabControl control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.ItemSize = new Size(-1, 1));
    }

    [WinFormsFact]
    public void TabControl_ItemSize_SetNegativeHeight_ThrowsArgumentOutOfRangeException()
    {
        using TabControl control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.ItemSize = new Size(1, -1));
    }

    public static IEnumerable<object[]> Multiline_Set_TestData()
    {
        yield return new object[] { TabAlignment.Bottom, true, TabAlignment.Bottom, TabAlignment.Bottom };
        yield return new object[] { TabAlignment.Left, true, TabAlignment.Left, TabAlignment.Top };
        yield return new object[] { TabAlignment.Right, true, TabAlignment.Right, TabAlignment.Top };
        yield return new object[] { TabAlignment.Top, true, TabAlignment.Top, TabAlignment.Top };
        yield return new object[] { TabAlignment.Bottom, false, TabAlignment.Bottom, TabAlignment.Bottom };
        yield return new object[] { TabAlignment.Left, false, TabAlignment.Top, TabAlignment.Top };
        yield return new object[] { TabAlignment.Right, false, TabAlignment.Top, TabAlignment.Top };
        yield return new object[] { TabAlignment.Top, false, TabAlignment.Top, TabAlignment.Top };
    }

    [WinFormsTheory]
    [MemberData(nameof(Multiline_Set_TestData))]
    public void TabControl_Multiline_Set_GetReturnsExpected(TabAlignment alignment, bool value, TabAlignment expectedAlignment1, TabAlignment expectedAlignment2)
    {
        using SubTabControl control = new()
        {
            Alignment = alignment,
            Multiline = value
        };
        Assert.Equal(value, control.Multiline);
        Assert.Equal(expectedAlignment1, control.Alignment);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Multiline = value;
        Assert.Equal(value, control.Multiline);
        Assert.Equal(expectedAlignment1, control.Alignment);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.Multiline = !value;
        Assert.Equal(!value, control.Multiline);
        Assert.Equal(expectedAlignment2, control.Alignment);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Multiline_SetWithHandle_TestData()
    {
        yield return new object[] { TabAlignment.Bottom, true, TabAlignment.Bottom, 1, TabAlignment.Bottom };
        yield return new object[] { TabAlignment.Left, true, TabAlignment.Left, 0, TabAlignment.Top };
        yield return new object[] { TabAlignment.Right, true, TabAlignment.Right, 0, TabAlignment.Top };
        yield return new object[] { TabAlignment.Top, true, TabAlignment.Top, 1, TabAlignment.Top };
        yield return new object[] { TabAlignment.Bottom, false, TabAlignment.Bottom, 0, TabAlignment.Bottom };
        yield return new object[] { TabAlignment.Left, false, TabAlignment.Top, 1, TabAlignment.Top };
        yield return new object[] { TabAlignment.Right, false, TabAlignment.Top, 1, TabAlignment.Top };
        yield return new object[] { TabAlignment.Top, false, TabAlignment.Top, 0, TabAlignment.Top };
    }

    [WinFormsTheory]
    [MemberData(nameof(Multiline_SetWithHandle_TestData))]
    public void TabControl_Multiline_SetWithHandle_GetReturnsExpected(TabAlignment alignment, bool value, TabAlignment expectedAlignment1, int expectedCreatedCallCount, TabAlignment expectedAlignment2)
    {
        using SubTabControl control = new()
        {
            Alignment = alignment
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Multiline = value;
        Assert.Equal(value, control.Multiline);
        Assert.Equal(expectedAlignment1, control.Alignment);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedCreatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        control.Multiline = value;
        Assert.Equal(value, control.Multiline);
        Assert.Equal(expectedAlignment1, control.Alignment);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedCreatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set different.
        control.Multiline = !value;
        Assert.Equal(expectedAlignment2, control.Alignment);
        Assert.Equal(!value, control.Multiline);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedCreatedCallCount + 1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount + 1, createdCallCount);
    }

    public static IEnumerable<object[]> Padding_Set_TestData()
    {
        yield return new object[] { new Point(0, 0) };
        yield return new object[] { new Point(0, 16) };
        yield return new object[] { new Point(16, 0) };
        yield return new object[] { new Point(16, 16) };
    }

    [WinFormsTheory]
    [MemberData(nameof(Padding_Set_TestData))]
    public void TabControl_Padding_Set_GetReturnsExpected(Point value)
    {
        using TabControl control = new()
        {
            Padding = value
        };
        Assert.Equal(value, control.Padding);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Padding = value;
        Assert.Equal(value, control.Padding);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Padding_Set_TestData))]
    public void TabControl_Padding_SetWithHandle_GetReturnsExpected(Point value)
    {
        using TabControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Padding = value;
        Assert.Equal(value, control.Padding);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(1, createdCallCount);

        // Set same.
        control.Padding = value;
        Assert.Equal(value, control.Padding);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(1, createdCallCount);
    }

    [WinFormsFact]
    public void TabControl_Padding_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(TabControl))[nameof(TabControl.Padding)];
        using TabControl control = new();
        Assert.False(property.CanResetValue(control));

        control.Padding = new Point(1, 0);
        Assert.Equal(new Point(1, 0), control.Padding);
        Assert.True(property.CanResetValue(control));

        control.Padding = new Point(0, 1);
        Assert.Equal(new Point(0, 1), control.Padding);
        Assert.True(property.CanResetValue(control));

        control.Padding = new Point(1, 2);
        Assert.Equal(new Point(1, 2), control.Padding);
        Assert.True(property.CanResetValue(control));

        property.ResetValue(control);
        Assert.Equal(new Point(6, 3), control.Padding);
        Assert.False(property.CanResetValue(control));
    }

    [WinFormsFact]
    public void TabControl_Padding_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(TabControl))[nameof(TabControl.Padding)];
        using TabControl control = new();
        Assert.False(property.ShouldSerializeValue(control));

        control.Padding = new Point(1, 0);
        Assert.Equal(new Point(1, 0), control.Padding);
        Assert.True(property.ShouldSerializeValue(control));

        control.Padding = new Point(0, 1);
        Assert.Equal(new Point(0, 1), control.Padding);
        Assert.True(property.ShouldSerializeValue(control));

        control.Padding = new Point(1, 2);
        Assert.Equal(new Point(1, 2), control.Padding);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.Equal(new Point(6, 3), control.Padding);
        Assert.False(property.ShouldSerializeValue(control));
    }

    [WinFormsFact]
    public void TabControl_Padding_SetNegativeX_ThrowsArgumentOutOfRangeException()
    {
        using TabControl control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.Padding = new Point(-1, 1));
    }

    [WinFormsFact]
    public void TabControl_Padding_SetNegativeY_ThrowsArgumentOutOfRangeException()
    {
        using TabControl control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.Padding = new Point(1, -1));
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.Yes, true, 1)]
    [InlineData(RightToLeft.Yes, false, 0)]
    [InlineData(RightToLeft.No, true, 1)]
    [InlineData(RightToLeft.No, false, 0)]
    [InlineData(RightToLeft.Inherit, true, 1)]
    [InlineData(RightToLeft.Inherit, false, 0)]
    public void TabControl_RightToLeftLayout_Set_GetReturnsExpected(RightToLeft rightToLeft, bool value, int expectedLayoutCallCount)
    {
        using TabControl control = new()
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
    public void TabControl_RightToLeftLayout_SetWithHandle_GetReturnsExpected(RightToLeft rightToLeft, bool value, int expectedLayoutCallCount, int expectedCreatedCallCount1, int expectedCreatedCallCount2)
    {
        using TabControl control = new()
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
        Assert.Equal(expectedCreatedCallCount1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount1, createdCallCount);

        // Set same.
        control.RightToLeftLayout = value;
        Assert.Equal(value, control.RightToLeftLayout);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedCreatedCallCount1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount1, createdCallCount);

        // Set different.
        control.RightToLeftLayout = !value;
        Assert.Equal(!value, control.RightToLeftLayout);
        Assert.Equal(expectedLayoutCallCount + 1, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedCreatedCallCount2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount2, createdCallCount);
    }

    [WinFormsFact]
    public void TabControl_RightToLeftLayout_SetWithHandler_CallsRightToLeftLayoutChanged()
    {
        using TabControl control = new()
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
    public void TabControl_RightToLeftLayout_SetWithHandlerInDisposing_DoesNotRightToLeftLayoutChanged()
    {
        using TabControl control = new()
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

    [WinFormsFact]
    public void TabControl_RowCount_Get_ReturnsExpectedAndCreatesHandle()
    {
        using TabControl control = new();
        Assert.Equal(0, control.RowCount);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabControl_RowCount_GetWithHandle_ReturnsExpected()
    {
        using TabControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(0, control.RowCount);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabControl_SelectedIndex_GetWithHandle_ReturnsExpected()
    {
        using TabControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(-1, control.SelectedIndex);
    }

    [WinFormsFact]
    public void TabControl_SelectedIndex_GetWithPagesWithHandle_ReturnsExpected()
    {
        using TabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(0, control.SelectedIndex);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void TabControl_SelectedIndex_Set_GetReturnsExpected(int value)
    {
        using TabControl control = new()
        {
            SelectedIndex = value
        };
        Assert.Equal(value, control.SelectedIndex);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SelectedIndex = value;
        Assert.Equal(value, control.SelectedIndex);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void TabControl_SelectedIndex_SetWithPages_GetReturnsExpected(int value)
    {
        using TabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);

        control.SelectedIndex = value;
        Assert.Equal(value, control.SelectedIndex);
        Assert.Equal(value < 0 || value >= control.TabPages.Count ? null : control.TabPages[value], control.SelectedTab);
        Assert.False(page1.Visible);
        Assert.False(page2.Visible);
        Assert.False(control.IsHandleCreated);
        Assert.False(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);

        // Set same.
        control.SelectedIndex = value;
        Assert.Equal(value, control.SelectedIndex);
        Assert.Equal(value < 0 || value >= control.TabPages.Count ? null : control.TabPages[value], control.SelectedTab);
        Assert.False(page1.Visible);
        Assert.False(page2.Visible);
        Assert.False(control.IsHandleCreated);
        Assert.False(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void TabControl_SelectedIndex_SetWithHandle_GetReturnsExpected(int value)
    {
        using TabControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SelectedIndex = value;
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Null(control.SelectedTab);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.SelectedIndex = value;
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Null(control.SelectedTab);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(-1, -1, new bool[] { false, false })]
    [InlineData(0, 0, new bool[] { true, false })]
    [InlineData(1, 1, new bool[] { false, true })]
    [InlineData(2, 0, new bool[] { true, false })]
    public void TabControl_SelectedIndex_SetWithPagesWithHandle_GetReturnsExpected(int value, int expected, bool[] expectedVisible)
    {
        using TabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SelectedIndex = value;
        Assert.Equal(expected, control.SelectedIndex);
        Assert.Equal(expected == -1 ? null : control.TabPages[expected], control.SelectedTab);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedVisible, control.TabPages.Cast<TabPage>().Select(p => p.Visible));
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.SelectedIndex = value;
        Assert.Equal(expected, control.SelectedIndex);
        Assert.Equal(expected == -1 ? null : control.TabPages[expected], control.SelectedTab);
        Assert.Equal(expectedVisible, control.TabPages.Cast<TabPage>().Select(p => p.Visible));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TabControl_SelectedIndex_SetWithHandler_CallsSelectedIndexChanged()
    {
        using TabControl control = new();
        int deselectingCallCount = 0;
        int deselectedCallCount = 0;
        int selectingCallCount = 0;
        int selectedCallCount = 0;
        int selectedIndexChangedCallCount = 0;
        void deselectingHandler(object sender, TabControlCancelEventArgs e) => deselectingCallCount++;
        void deselectedHandler(object sender, TabControlEventArgs e) => deselectedCallCount++;
        void selectingHandler(object sender, TabControlCancelEventArgs e) => selectingCallCount++;
        void selectedHandler(object sender, TabControlEventArgs e) => selectedCallCount++;
        void selectedIndexChangedHandler(object sender, EventArgs e) => selectedIndexChangedCallCount++;
        control.Deselecting += deselectingHandler;
        control.Deselected += deselectedHandler;
        control.Selecting += selectingHandler;
        control.Selected += selectedHandler;
        control.SelectedIndexChanged += selectedIndexChangedHandler;

        // Set different.
        control.SelectedIndex = 0;
        Assert.Equal(0, control.SelectedIndex);
        Assert.Equal(0, deselectingCallCount);
        Assert.Equal(0, deselectedCallCount);
        Assert.Equal(0, selectingCallCount);
        Assert.Equal(0, selectedCallCount);
        Assert.Equal(0, selectedIndexChangedCallCount);

        // Set same.
        control.SelectedIndex = 0;
        Assert.Equal(0, control.SelectedIndex);
        Assert.Equal(0, deselectingCallCount);
        Assert.Equal(0, deselectedCallCount);
        Assert.Equal(0, selectingCallCount);
        Assert.Equal(0, selectedCallCount);
        Assert.Equal(0, selectedIndexChangedCallCount);

        // Set different.
        control.SelectedIndex = 1;
        Assert.Equal(1, control.SelectedIndex);
        Assert.Equal(0, deselectingCallCount);
        Assert.Equal(0, deselectedCallCount);
        Assert.Equal(0, selectingCallCount);
        Assert.Equal(0, selectedCallCount);
        Assert.Equal(0, selectedIndexChangedCallCount);

        // Remove handler.
        control.Deselecting -= deselectingHandler;
        control.Deselected -= deselectedHandler;
        control.Selecting -= selectingHandler;
        control.Selected -= selectedHandler;
        control.SelectedIndexChanged -= selectedIndexChangedHandler;
        control.SelectedIndex = 0;
        Assert.Equal(0, control.SelectedIndex);
        Assert.Equal(0, deselectingCallCount);
        Assert.Equal(0, deselectedCallCount);
        Assert.Equal(0, selectingCallCount);
        Assert.Equal(0, selectedCallCount);
        Assert.Equal(0, selectedIndexChangedCallCount);
    }

    [WinFormsFact]
    public void TabControl_SelectedIndex_SetWithHandleWithHandler_CallsSelectedIndexChanged()
    {
        using TabControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int deselectingCallCount = 0;
        int deselectedCallCount = 0;
        int selectingCallCount = 0;
        int selectedCallCount = 0;
        int selectedIndexChangedCallCount = 0;
        void deselectingHandler(object sender, TabControlCancelEventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Equal(TabControlAction.Deselecting, e.Action);
            Assert.False(e.Cancel);
            Assert.Null(e.TabPage);
            Assert.Equal(control.SelectedIndex, e.TabPageIndex);
            Assert.Equal(deselectingCallCount, deselectedCallCount);
            Assert.Equal(deselectingCallCount, selectingCallCount);
            Assert.Equal(deselectingCallCount, selectedCallCount);
            Assert.Equal(deselectingCallCount, selectedIndexChangedCallCount);
            deselectingCallCount++;
        }

        void deselectedHandler(object sender, TabControlEventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Equal(TabControlAction.Deselected, e.Action);
            Assert.Null(e.TabPage);
            Assert.Equal(control.SelectedIndex, e.TabPageIndex);
            Assert.Equal(deselectedCallCount, selectingCallCount);
            Assert.Equal(deselectedCallCount, selectedCallCount);
            Assert.Equal(deselectedCallCount, selectedIndexChangedCallCount);
            deselectedCallCount++;
        }

        void selectingHandler(object sender, TabControlCancelEventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Equal(TabControlAction.Selecting, e.Action);
            Assert.False(e.Cancel);
            Assert.Null(e.TabPage);
            Assert.Equal(control.SelectedIndex, e.TabPageIndex);
            Assert.Equal(selectingCallCount, selectedCallCount);
            Assert.Equal(selectingCallCount, selectedIndexChangedCallCount);
            selectingCallCount++;
        }

        void selectedHandler(object sender, TabControlEventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Equal(TabControlAction.Selected, e.Action);
            Assert.Null(e.TabPage);
            Assert.Equal(control.SelectedIndex, e.TabPageIndex);
            Assert.Equal(selectedCallCount, selectedIndexChangedCallCount);
            selectedCallCount++;
        }

        void selectedIndexChangedHandler(object sender, EventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            selectedIndexChangedCallCount++;
        }

        control.Deselecting += deselectingHandler;
        control.Deselected += deselectedHandler;
        control.Selecting += selectingHandler;
        control.Selected += selectedHandler;
        control.SelectedIndexChanged += selectedIndexChangedHandler;

        // Set different.
        control.SelectedIndex = 0;
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Equal(1, deselectingCallCount);
        Assert.Equal(1, deselectedCallCount);
        Assert.Equal(1, selectingCallCount);
        Assert.Equal(1, selectedCallCount);
        Assert.Equal(1, selectedIndexChangedCallCount);

        // Set same.
        control.SelectedIndex = 0;
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Equal(2, deselectingCallCount);
        Assert.Equal(2, deselectedCallCount);
        Assert.Equal(2, selectingCallCount);
        Assert.Equal(2, selectedCallCount);
        Assert.Equal(2, selectedIndexChangedCallCount);

        // Set different.
        control.SelectedIndex = 1;
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Equal(3, deselectingCallCount);
        Assert.Equal(3, deselectedCallCount);
        Assert.Equal(3, selectingCallCount);
        Assert.Equal(3, selectedCallCount);
        Assert.Equal(3, selectedIndexChangedCallCount);

        // Remove handler.
        control.Deselecting -= deselectingHandler;
        control.Deselected -= deselectedHandler;
        control.Selecting -= selectingHandler;
        control.Selected -= selectedHandler;
        control.SelectedIndexChanged -= selectedIndexChangedHandler;
        control.SelectedIndex = 0;
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Equal(3, deselectingCallCount);
        Assert.Equal(3, deselectedCallCount);
        Assert.Equal(3, selectingCallCount);
        Assert.Equal(3, selectedCallCount);
        Assert.Equal(3, selectedIndexChangedCallCount);
    }

    [WinFormsFact]
    public void TabControl_SelectedIndex_SetWithHandleWithHandlerCancelDeselecting_DoesNotCallSelectedIndexChanged()
    {
        using TabControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int deselectingCallCount = 0;
        int deselectedCallCount = 0;
        int selectingCallCount = 0;
        int selectedCallCount = 0;
        int selectedIndexChangedCallCount = 0;
        void deselectingHandler(object sender, TabControlCancelEventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Equal(TabControlAction.Deselecting, e.Action);
            Assert.False(e.Cancel);
            Assert.Null(e.TabPage);
            Assert.Equal(control.SelectedIndex, e.TabPageIndex);
            Assert.Equal(0, deselectedCallCount);
            Assert.Equal(0, selectingCallCount);
            Assert.Equal(0, selectedCallCount);
            Assert.Equal(0, selectedIndexChangedCallCount);
            deselectingCallCount++;

            e.Cancel = true;
        }

        void deselectedHandler(object sender, TabControlEventArgs e) => deselectedCallCount++;
        void selectingHandler(object sender, TabControlCancelEventArgs e) => selectingCallCount++;
        void selectedHandler(object sender, TabControlEventArgs e) => selectedCallCount++;
        void selectedIndexChangedHandler(object sender, EventArgs e) => selectedIndexChangedCallCount++;
        control.Deselecting += deselectingHandler;
        control.Deselected += deselectedHandler;
        control.Selecting += selectingHandler;
        control.Selected += selectedHandler;
        control.SelectedIndexChanged += selectedIndexChangedHandler;

        // Set different.
        control.SelectedIndex = 0;
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Equal(1, deselectingCallCount);
        Assert.Equal(0, deselectedCallCount);
        Assert.Equal(0, selectingCallCount);
        Assert.Equal(0, selectedCallCount);
        Assert.Equal(0, selectedIndexChangedCallCount);

        // Set same.
        control.SelectedIndex = 0;
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Equal(2, deselectingCallCount);
        Assert.Equal(0, deselectedCallCount);
        Assert.Equal(0, selectingCallCount);
        Assert.Equal(0, selectedCallCount);
        Assert.Equal(0, selectedIndexChangedCallCount);

        // Set different.
        control.SelectedIndex = 1;
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Equal(3, deselectingCallCount);
        Assert.Equal(0, deselectedCallCount);
        Assert.Equal(0, selectingCallCount);
        Assert.Equal(0, selectedCallCount);
        Assert.Equal(0, selectedIndexChangedCallCount);

        // Remove handler.
        control.Deselecting -= deselectingHandler;
        control.Deselected -= deselectedHandler;
        control.Selecting -= selectingHandler;
        control.Selected -= selectedHandler;
        control.SelectedIndexChanged -= selectedIndexChangedHandler;
        control.SelectedIndex = 0;
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Equal(3, deselectingCallCount);
        Assert.Equal(0, deselectedCallCount);
        Assert.Equal(0, selectingCallCount);
        Assert.Equal(0, selectedCallCount);
        Assert.Equal(0, selectedIndexChangedCallCount);
    }

    [WinFormsFact]
    public void TabControl_SelectedIndex_SetWithHandleWithHandlerCancelSelecting_DoesNotCallSelectedIndexChanged()
    {
        using TabControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int deselectingCallCount = 0;
        int deselectedCallCount = 0;
        int selectingCallCount = 0;
        int selectedCallCount = 0;
        int selectedIndexChangedCallCount = 0;
        void deselectingHandler(object sender, TabControlCancelEventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Equal(TabControlAction.Deselecting, e.Action);
            Assert.False(e.Cancel);
            Assert.Null(e.TabPage);
            Assert.Equal(control.SelectedIndex, e.TabPageIndex);
            Assert.Equal(deselectingCallCount, deselectedCallCount);
            Assert.Equal(deselectingCallCount, selectingCallCount);
            Assert.Equal(0, selectedCallCount);
            Assert.Equal(0, selectedIndexChangedCallCount);
            deselectingCallCount++;
        }

        void deselectedHandler(object sender, TabControlEventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Equal(TabControlAction.Deselected, e.Action);
            Assert.Null(e.TabPage);
            Assert.Equal(control.SelectedIndex, e.TabPageIndex);
            Assert.Equal(deselectedCallCount, selectingCallCount);
            Assert.Equal(0, selectedCallCount);
            Assert.Equal(0, selectedIndexChangedCallCount);
            deselectedCallCount++;
        }

        void selectingHandler(object sender, TabControlCancelEventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Equal(TabControlAction.Selecting, e.Action);
            Assert.False(e.Cancel);
            Assert.Null(e.TabPage);
            Assert.Equal(control.SelectedIndex, e.TabPageIndex);
            Assert.Equal(0, selectedCallCount);
            Assert.Equal(0, selectedIndexChangedCallCount);
            selectingCallCount++;

            e.Cancel = true;
        }

        void selectedHandler(object sender, TabControlEventArgs e) => selectedCallCount++;
        void selectedIndexChangedHandler(object sender, EventArgs e) => selectedIndexChangedCallCount++;
        control.Deselecting += deselectingHandler;
        control.Deselected += deselectedHandler;
        control.Selecting += selectingHandler;
        control.Selected += selectedHandler;
        control.SelectedIndexChanged += selectedIndexChangedHandler;

        // Set different.
        control.SelectedIndex = 0;
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Equal(1, deselectingCallCount);
        Assert.Equal(1, deselectedCallCount);
        Assert.Equal(1, selectingCallCount);
        Assert.Equal(0, selectedCallCount);
        Assert.Equal(0, selectedIndexChangedCallCount);

        // Set same.
        control.SelectedIndex = 0;
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Equal(2, deselectingCallCount);
        Assert.Equal(2, deselectedCallCount);
        Assert.Equal(2, selectingCallCount);
        Assert.Equal(0, selectedCallCount);
        Assert.Equal(0, selectedIndexChangedCallCount);

        // Set different.
        control.SelectedIndex = 1;
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Equal(3, deselectingCallCount);
        Assert.Equal(3, deselectedCallCount);
        Assert.Equal(3, selectingCallCount);
        Assert.Equal(0, selectedCallCount);
        Assert.Equal(0, selectedIndexChangedCallCount);

        // Remove handler.
        control.Deselecting -= deselectingHandler;
        control.Deselected -= deselectedHandler;
        control.Selecting -= selectingHandler;
        control.Selected -= selectedHandler;
        control.SelectedIndexChanged -= selectedIndexChangedHandler;
        control.SelectedIndex = 0;
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Equal(3, deselectingCallCount);
        Assert.Equal(3, deselectedCallCount);
        Assert.Equal(3, selectingCallCount);
        Assert.Equal(0, selectedCallCount);
        Assert.Equal(0, selectedIndexChangedCallCount);
    }

    [WinFormsFact]
    public void TabControl_SelectedIndex_SetInvalid_ThrowsArgumentOutOfRangeException()
    {
        using TabControl control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.SelectedIndex = -2);
    }

    [WinFormsFact]
    public void TabControl_SelectedTab_GetWithHandle_ReturnsExpected()
    {
        using TabControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Null(control.SelectedTab);
    }

    [WinFormsFact]
    public void TabControl_SelectedTab_GetWithPagesWithHandle_ReturnsExpected()
    {
        using TabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Same(page1, control.SelectedTab);
        Assert.True(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void TabControl_SelectedTab_GetWithInvalidIndexNotGotPages_ReturnsNull(int value)
    {
        using TabControl control = new()
        {
            SelectedIndex = value
        };
        Assert.Null(control.SelectedTab);

        Assert.Empty(control.TabPages);
        Assert.Null(control.SelectedTab);
    }

    public static IEnumerable<object[]> SelectedTab_Set_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new TabPage() };
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectedTab_Set_TestData))]
    public void TabControl_SelectedTab_SetWithoutPages_GetReturnsExpected(TabPage value)
    {
        using TabControl control = new()
        {
            SelectedTab = value
        };
        Assert.Null(control.SelectedTab);
        Assert.Equal(-1, control.SelectedIndex);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SelectedTab = value;
        Assert.Null(control.SelectedTab);
        Assert.Equal(-1, control.SelectedIndex);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabControl_SelectedTab_SetWithPages_GetReturnsExpected()
    {
        using TabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        using TabPage page3 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);

        // Set valid.
        control.SelectedTab = page2;
        Assert.Same(page2, control.SelectedTab);
        Assert.Equal(1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.False(page2.Visible);
        Assert.False(control.IsHandleCreated);
        Assert.False(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);

        // Set same.
        control.SelectedTab = page2;
        Assert.Same(page2, control.SelectedTab);
        Assert.Equal(1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.False(page2.Visible);
        Assert.False(control.IsHandleCreated);
        Assert.False(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);

        // Set invalid.
        control.SelectedTab = page3;
        Assert.Null(control.SelectedTab);
        Assert.Equal(-1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.False(page2.Visible);
        Assert.False(control.IsHandleCreated);
        Assert.False(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);

        // Set different.
        control.SelectedTab = page1;
        Assert.Same(page1, control.SelectedTab);
        Assert.Equal(0, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.False(page2.Visible);
        Assert.False(control.IsHandleCreated);
        Assert.False(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);

        // Set invalid.
        control.SelectedTab = null;
        Assert.Null(control.SelectedTab);
        Assert.Equal(-1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.False(page2.Visible);
        Assert.False(control.IsHandleCreated);
        Assert.False(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectedTab_Set_TestData))]
    public void TabControl_SelectedTab_SetWithoutPagesWithHandle_GetReturnsExpected(TabPage value)
    {
        using TabControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SelectedTab = value;
        Assert.Null(control.SelectedTab);
        Assert.Equal(-1, control.SelectedIndex);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.SelectedTab = value;
        Assert.Null(control.SelectedTab);
        Assert.Equal(-1, control.SelectedIndex);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TabControl_SelectedTab_SetWithPagesWithHandle_GetReturnsExpected()
    {
        using TabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        using TabPage page3 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int pageInvalidatedCallCount1 = 0;
        page1.Invalidated += (sender, e) => pageInvalidatedCallCount1++;
        int pageStyleChangedCallCount1 = 0;
        page1.StyleChanged += (sender, e) => pageStyleChangedCallCount1++;
        int pageCreatedCallCount1 = 0;
        page1.HandleCreated += (sender, e) => pageCreatedCallCount1++;
        int pageInvalidatedCallCount2 = 0;
        page2.Invalidated += (sender, e) => pageInvalidatedCallCount2++;
        int pageStyleChangedCallCount2 = 0;
        page2.StyleChanged += (sender, e) => pageStyleChangedCallCount2++;
        int pageCreatedCallCount2 = 0;
        page2.HandleCreated += (sender, e) => pageCreatedCallCount2++;
        Assert.True(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);

        // Set valid.
        control.SelectedTab = page2;
        Assert.Same(page2, control.SelectedTab);
        Assert.Equal(1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.True(page2.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(page1.IsHandleCreated);
        Assert.Equal(0, pageInvalidatedCallCount1);
        Assert.Equal(0, pageStyleChangedCallCount1);
        Assert.Equal(0, pageCreatedCallCount1);
        Assert.True(page2.IsHandleCreated);
        Assert.Equal(0, pageInvalidatedCallCount2);
        Assert.Equal(0, pageStyleChangedCallCount2);
        Assert.Equal(1, pageCreatedCallCount2);

        // Set same.
        control.SelectedTab = page2;
        Assert.Same(page2, control.SelectedTab);
        Assert.Equal(1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.True(page2.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(page1.IsHandleCreated);
        Assert.Equal(0, pageInvalidatedCallCount1);
        Assert.Equal(0, pageStyleChangedCallCount1);
        Assert.Equal(0, pageCreatedCallCount1);
        Assert.True(page2.IsHandleCreated);
        Assert.Equal(0, pageInvalidatedCallCount2);
        Assert.Equal(0, pageStyleChangedCallCount2);
        Assert.Equal(1, pageCreatedCallCount2);

        // Set invalid.
        control.SelectedTab = page3;
        Assert.Null(control.SelectedTab);
        Assert.Equal(-1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.False(page2.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(page1.IsHandleCreated);
        Assert.Equal(0, pageInvalidatedCallCount1);
        Assert.Equal(0, pageStyleChangedCallCount1);
        Assert.Equal(0, pageCreatedCallCount1);
        Assert.True(page2.IsHandleCreated);
        Assert.Equal(0, pageInvalidatedCallCount2);
        Assert.Equal(0, pageStyleChangedCallCount2);
        Assert.Equal(1, pageCreatedCallCount2);

        // Set different.
        control.SelectedTab = page1;
        Assert.Same(page1, control.SelectedTab);
        Assert.Equal(0, control.SelectedIndex);
        Assert.True(page1.Visible);
        Assert.False(page2.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(page1.IsHandleCreated);
        Assert.Equal(1, pageInvalidatedCallCount1);
        Assert.Equal(0, pageStyleChangedCallCount1);
        Assert.Equal(0, pageCreatedCallCount1);
        Assert.True(page2.IsHandleCreated);
        Assert.Equal(0, pageInvalidatedCallCount2);
        Assert.Equal(0, pageStyleChangedCallCount2);
        Assert.Equal(1, pageCreatedCallCount2);

        // Set invalid.
        control.SelectedTab = null;
        Assert.Null(control.SelectedTab);
        Assert.Equal(-1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.False(page2.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(page1.IsHandleCreated);
        Assert.Equal(1, pageInvalidatedCallCount1);
        Assert.Equal(0, pageStyleChangedCallCount1);
        Assert.Equal(0, pageCreatedCallCount1);
        Assert.True(page2.IsHandleCreated);
        Assert.Equal(0, pageInvalidatedCallCount2);
        Assert.Equal(0, pageStyleChangedCallCount2);
        Assert.Equal(1, pageCreatedCallCount2);
    }

    [WinFormsTheory]
    [EnumData<TabSizeMode>]
    public void TabControl_SizeMode_Set_GetReturnsExpected(TabSizeMode value)
    {
        using TabControl control = new()
        {
            SizeMode = value
        };
        Assert.Equal(value, control.SizeMode);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SizeMode = value;
        Assert.Equal(value, control.SizeMode);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(TabSizeMode.Normal, 0)]
    [InlineData(TabSizeMode.FillToRight, 1)]
    [InlineData(TabSizeMode.Fixed, 1)]
    public void TabControl_SizeMode_SetWithHandle_GetReturnsExpected(TabSizeMode value, int expectedCreatedCallCount)
    {
        using TabControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SizeMode = value;
        Assert.Equal(value, control.SizeMode);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedCreatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        control.SizeMode = value;
        Assert.Equal(value, control.SizeMode);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedCreatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<TabSizeMode>]
    public void TabControl_SizeMode_SetInvalidValue_ThrowsInvalidEnumArgumentException(TabSizeMode value)
    {
        using TabControl control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.SizeMode = value);
    }

    [WinFormsTheory]
    [BoolData]
    public void TabControl_ShowToolTips_Set_GetReturnsExpected(bool value)
    {
        using SubTabControl control = new()
        {
            ShowToolTips = value
        };
        Assert.Equal(value, control.ShowToolTips);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ShowToolTips = value;
        Assert.Equal(value, control.ShowToolTips);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.ShowToolTips = !value;
        Assert.Equal(!value, control.ShowToolTips);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void TabControl_ShowToolTips_SetWithHandle_GetReturnsExpected(bool value, int expectedCreatedCallCount)
    {
        using SubTabControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ShowToolTips = value;
        Assert.Equal(value, control.ShowToolTips);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedCreatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        control.ShowToolTips = value;
        Assert.Equal(value, control.ShowToolTips);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedCreatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set different.
        control.ShowToolTips = !value;
        Assert.Equal(!value, control.ShowToolTips);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedCreatedCallCount + 1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount + 1, createdCallCount);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void TabControl_Text_Set_GetReturnsExpected(string value, string expected)
    {
        using TabControl control = new()
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
    public void TabControl_Text_SetWithHandle_GetReturnsExpected(string value, string expected)
    {
        using TabControl control = new();
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
    public void TabControl_Text_SetWithHandler_CallsTextChanged()
    {
        using TabControl control = new();
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Equal(EventArgs.Empty, e);
            callCount++;
        }

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

    [WinFormsFact]
    public void TabControl_CreateControlsInstance_Invoke_ReturnsExpected()
    {
        using SubTabControl control = new();
        Control.ControlCollection controls = Assert.IsType<TabControl.ControlCollection>(control.CreateControlsInstance());
        Assert.Empty(controls);
        Assert.Same(control, controls.Owner);
        Assert.False(controls.IsReadOnly);
        Assert.NotSame(controls, control.CreateControlsInstance());
    }

    [WinFormsFact]
    public void TabControl_CreateHandle_Invoke_Success()
    {
        using SubTabControl control = new();
        control.CreateHandle();
        Assert.False(control.Created);
        Assert.True(control.IsHandleCreated);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
    }

    [WinFormsFact]
    public void TabControl_DeselectTab_InvokeTabPageWithoutHandle_GetReturnsExpected()
    {
        using TabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);

        // Deselect first.
        control.DeselectTab(page1);
        Assert.Null(control.SelectedTab);
        Assert.Equal(-1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.False(page2.Visible);
        Assert.False(control.IsHandleCreated);
        Assert.False(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);

        // Select and deselect first.
        control.SelectTab(page1);
        control.DeselectTab(page1);
        Assert.Same(page2, control.SelectedTab);
        Assert.Equal(1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.False(page2.Visible);
        Assert.False(control.IsHandleCreated);
        Assert.False(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);

        // Deselect again.
        control.DeselectTab(page1);
        Assert.Same(page2, control.SelectedTab);
        Assert.Equal(1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.False(page2.Visible);
        Assert.False(control.IsHandleCreated);
        Assert.False(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);

        // Deselect last.
        control.DeselectTab(page2);
        Assert.Same(page1, control.SelectedTab);
        Assert.Equal(0, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.False(page2.Visible);
        Assert.False(control.IsHandleCreated);
        Assert.False(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabControl_DeselectTab_InvokeTabPageWithHandle_GetReturnsExpected()
    {
        using TabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int pageInvalidatedCallCount1 = 0;
        page1.Invalidated += (sender, e) => pageInvalidatedCallCount1++;
        int pageStyleChangedCallCount1 = 0;
        page1.StyleChanged += (sender, e) => pageStyleChangedCallCount1++;
        int pageCreatedCallCount1 = 0;
        page1.HandleCreated += (sender, e) => pageCreatedCallCount1++;
        int pageInvalidatedCallCount2 = 0;
        page2.Invalidated += (sender, e) => pageInvalidatedCallCount2++;
        int pageStyleChangedCallCount2 = 0;
        page2.StyleChanged += (sender, e) => pageStyleChangedCallCount2++;
        int pageCreatedCallCount2 = 0;
        page2.HandleCreated += (sender, e) => pageCreatedCallCount2++;
        Assert.True(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);

        // Deselect first.
        control.DeselectTab(page1);
        Assert.Same(page2, control.SelectedTab);
        Assert.Equal(1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.True(page2.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(page1.IsHandleCreated);
        Assert.Equal(0, pageInvalidatedCallCount1);
        Assert.Equal(0, pageStyleChangedCallCount1);
        Assert.Equal(0, pageCreatedCallCount1);
        Assert.True(page2.IsHandleCreated);
        Assert.Equal(0, pageInvalidatedCallCount2);
        Assert.Equal(0, pageStyleChangedCallCount2);
        Assert.Equal(1, pageCreatedCallCount2);

        // Select and deselect first.
        control.SelectTab(0);
        control.DeselectTab(page1);
        Assert.Same(page2, control.SelectedTab);
        Assert.Equal(1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.True(page2.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(page1.IsHandleCreated);
        Assert.Equal(1, pageInvalidatedCallCount1);
        Assert.Equal(0, pageStyleChangedCallCount1);
        Assert.Equal(0, pageCreatedCallCount1);
        Assert.True(page2.IsHandleCreated);
        Assert.Equal(1, pageInvalidatedCallCount2);
        Assert.Equal(0, pageStyleChangedCallCount2);
        Assert.Equal(1, pageCreatedCallCount2);

        // Deselect again.
        control.DeselectTab(page1);
        Assert.Same(page2, control.SelectedTab);
        Assert.Equal(1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.True(page2.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(page1.IsHandleCreated);
        Assert.Equal(1, pageInvalidatedCallCount1);
        Assert.Equal(0, pageStyleChangedCallCount1);
        Assert.Equal(0, pageCreatedCallCount1);
        Assert.True(page2.IsHandleCreated);
        Assert.Equal(1, pageInvalidatedCallCount2);
        Assert.Equal(0, pageStyleChangedCallCount2);
        Assert.Equal(1, pageCreatedCallCount2);

        // Deselect last.
        control.DeselectTab(page2);
        Assert.Same(page1, control.SelectedTab);
        Assert.Equal(0, control.SelectedIndex);
        Assert.True(page1.Visible);
        Assert.False(page2.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(page1.IsHandleCreated);
        Assert.Equal(2, pageInvalidatedCallCount1);
        Assert.Equal(0, pageStyleChangedCallCount1);
        Assert.Equal(0, pageCreatedCallCount1);
        Assert.True(page2.IsHandleCreated);
        Assert.Equal(1, pageInvalidatedCallCount2);
        Assert.Equal(0, pageStyleChangedCallCount2);
        Assert.Equal(1, pageCreatedCallCount2);
    }

    [WinFormsFact]
    public void TabControl_DeselectTab_InvalidTabPageNameWithoutPages_ThrowsArgumentOutOfRangeException()
    {
        using TabControl control = new();
        using TabPage page = new();
        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.DeselectTab(page));
    }

    [WinFormsFact]
    public void TabControl_DeselectTab_InvalidTabPageNameWithPages_ThrowsArgumentOutOfRangeException()
    {
        using TabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        using TabPage page3 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.DeselectTab(page3));
    }

    [WinFormsFact]
    public void TabControl_DeselectTab_InvokeStringWithoutHandle_GetReturnsExpected()
    {
        using TabControl control = new();
        using TabPage page1 = new()
        {
            Name = "Name1"
        };
        using TabPage page2 = new()
        {
            Name = "Name2"
        };
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);

        // Deselect first.
        control.DeselectTab("Name1");
        Assert.Null(control.SelectedTab);
        Assert.Equal(-1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.False(page2.Visible);
        Assert.False(control.IsHandleCreated);
        Assert.False(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);

        // Select and deselect first.
        control.SelectTab("Name1");
        control.DeselectTab("Name1");
        Assert.Same(page2, control.SelectedTab);
        Assert.Equal(1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.False(page2.Visible);
        Assert.False(control.IsHandleCreated);
        Assert.False(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);

        // Deselect again.
        control.DeselectTab("Name1");
        Assert.Same(page2, control.SelectedTab);
        Assert.Equal(1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.False(page2.Visible);
        Assert.False(control.IsHandleCreated);
        Assert.False(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);

        // Deselect last.
        control.DeselectTab("Name2");
        Assert.Same(page1, control.SelectedTab);
        Assert.Equal(0, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.False(page2.Visible);
        Assert.False(control.IsHandleCreated);
        Assert.False(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabControl_DeselectTab_InvokeStringWithHandle_GetReturnsExpected()
    {
        using TabControl control = new();
        using TabPage page1 = new()
        {
            Name = "Name1"
        };
        using TabPage page2 = new()
        {
            Name = "Name2"
        };
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int pageInvalidatedCallCount1 = 0;
        page1.Invalidated += (sender, e) => pageInvalidatedCallCount1++;
        int pageStyleChangedCallCount1 = 0;
        page1.StyleChanged += (sender, e) => pageStyleChangedCallCount1++;
        int pageCreatedCallCount1 = 0;
        page1.HandleCreated += (sender, e) => pageCreatedCallCount1++;
        int pageInvalidatedCallCount2 = 0;
        page2.Invalidated += (sender, e) => pageInvalidatedCallCount2++;
        int pageStyleChangedCallCount2 = 0;
        page2.StyleChanged += (sender, e) => pageStyleChangedCallCount2++;
        int pageCreatedCallCount2 = 0;
        page2.HandleCreated += (sender, e) => pageCreatedCallCount2++;
        Assert.True(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);

        // Deselect first.
        control.DeselectTab("Name1");
        Assert.Same(page2, control.SelectedTab);
        Assert.Equal(1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.True(page2.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(page1.IsHandleCreated);
        Assert.Equal(0, pageInvalidatedCallCount1);
        Assert.Equal(0, pageStyleChangedCallCount1);
        Assert.Equal(0, pageCreatedCallCount1);
        Assert.True(page2.IsHandleCreated);
        Assert.Equal(0, pageInvalidatedCallCount2);
        Assert.Equal(0, pageStyleChangedCallCount2);
        Assert.Equal(1, pageCreatedCallCount2);

        // Select and deselect first.
        control.SelectTab(0);
        control.DeselectTab("Name1");
        Assert.Same(page2, control.SelectedTab);
        Assert.Equal(1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.True(page2.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(page1.IsHandleCreated);
        Assert.Equal(1, pageInvalidatedCallCount1);
        Assert.Equal(0, pageStyleChangedCallCount1);
        Assert.Equal(0, pageCreatedCallCount1);
        Assert.True(page2.IsHandleCreated);
        Assert.Equal(1, pageInvalidatedCallCount2);
        Assert.Equal(0, pageStyleChangedCallCount2);
        Assert.Equal(1, pageCreatedCallCount2);

        // Deselect again.
        control.DeselectTab("Name1");
        Assert.Same(page2, control.SelectedTab);
        Assert.Equal(1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.True(page2.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(page1.IsHandleCreated);
        Assert.Equal(1, pageInvalidatedCallCount1);
        Assert.Equal(0, pageStyleChangedCallCount1);
        Assert.Equal(0, pageCreatedCallCount1);
        Assert.True(page2.IsHandleCreated);
        Assert.Equal(1, pageInvalidatedCallCount2);
        Assert.Equal(0, pageStyleChangedCallCount2);
        Assert.Equal(1, pageCreatedCallCount2);

        // Deselect last.
        control.DeselectTab("Name2");
        Assert.Same(page1, control.SelectedTab);
        Assert.Equal(0, control.SelectedIndex);
        Assert.True(page1.Visible);
        Assert.False(page2.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(page1.IsHandleCreated);
        Assert.Equal(2, pageInvalidatedCallCount1);
        Assert.Equal(0, pageStyleChangedCallCount1);
        Assert.Equal(0, pageCreatedCallCount1);
        Assert.True(page2.IsHandleCreated);
        Assert.Equal(1, pageInvalidatedCallCount2);
        Assert.Equal(0, pageStyleChangedCallCount2);
        Assert.Equal(1, pageCreatedCallCount2);
    }

    [WinFormsFact]
    public void TabControl_DeselectTab_NullTabPageName_ThrowsArgumentNullException()
    {
        using TabControl control = new();
        Assert.Throws<ArgumentNullException>("tabPageName", () => control.DeselectTab((string)null));
    }

    [WinFormsTheory]
    [InlineData("")]
    [InlineData("NoSuchName")]
    public void TabControl_DeselectTab_InvalidTabPageNameWithoutPages_ThrowsArgumentNullException(string tabPageName)
    {
        using TabControl control = new();
        Assert.Throws<ArgumentNullException>("tabPage", () => control.DeselectTab(tabPageName));
    }

    [WinFormsTheory]
    [InlineData("")]
    [InlineData("NoSuchName")]
    public void TabControl_DeselectTab_InvalidTabPageNameWithPages_ThrowsArgumentNullException(string tabPageName)
    {
        using TabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        Assert.Throws<ArgumentNullException>("tabPage", () => control.DeselectTab(tabPageName));
    }

    [WinFormsFact]
    public void TabControl_DeselectTab_InvokeIntWithoutHandle_GetReturnsExpected()
    {
        using TabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);

        // Deselect first.
        control.DeselectTab(0);
        Assert.Null(control.SelectedTab);
        Assert.Equal(-1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.False(page2.Visible);
        Assert.False(control.IsHandleCreated);
        Assert.False(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);

        // Select and deselect first.
        control.SelectTab(0);
        control.DeselectTab(0);
        Assert.Same(page2, control.SelectedTab);
        Assert.Equal(1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.False(page2.Visible);
        Assert.False(control.IsHandleCreated);
        Assert.False(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);

        // Deselect again.
        control.DeselectTab(0);
        Assert.Same(page2, control.SelectedTab);
        Assert.Equal(1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.False(page2.Visible);
        Assert.False(control.IsHandleCreated);
        Assert.False(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);

        // Deselect last.
        control.DeselectTab(1);
        Assert.Same(page1, control.SelectedTab);
        Assert.Equal(0, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.False(page2.Visible);
        Assert.False(control.IsHandleCreated);
        Assert.False(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabControl_DeselectTab_InvokeIntWithHandle_GetReturnsExpected()
    {
        using TabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int pageInvalidatedCallCount1 = 0;
        page1.Invalidated += (sender, e) => pageInvalidatedCallCount1++;
        int pageStyleChangedCallCount1 = 0;
        page1.StyleChanged += (sender, e) => pageStyleChangedCallCount1++;
        int pageCreatedCallCount1 = 0;
        page1.HandleCreated += (sender, e) => pageCreatedCallCount1++;
        int pageInvalidatedCallCount2 = 0;
        page2.Invalidated += (sender, e) => pageInvalidatedCallCount2++;
        int pageStyleChangedCallCount2 = 0;
        page2.StyleChanged += (sender, e) => pageStyleChangedCallCount2++;
        int pageCreatedCallCount2 = 0;
        page2.HandleCreated += (sender, e) => pageCreatedCallCount2++;
        Assert.True(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);

        // Deselect first.
        control.DeselectTab(0);
        Assert.Same(page2, control.SelectedTab);
        Assert.Equal(1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.True(page2.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(page1.IsHandleCreated);
        Assert.Equal(0, pageInvalidatedCallCount1);
        Assert.Equal(0, pageStyleChangedCallCount1);
        Assert.Equal(0, pageCreatedCallCount1);
        Assert.True(page2.IsHandleCreated);
        Assert.Equal(0, pageInvalidatedCallCount2);
        Assert.Equal(0, pageStyleChangedCallCount2);
        Assert.Equal(1, pageCreatedCallCount2);

        // Select and deselect first.
        control.SelectTab(0);
        control.DeselectTab(0);
        Assert.Same(page2, control.SelectedTab);
        Assert.Equal(1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.True(page2.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(page1.IsHandleCreated);
        Assert.Equal(1, pageInvalidatedCallCount1);
        Assert.Equal(0, pageStyleChangedCallCount1);
        Assert.Equal(0, pageCreatedCallCount1);
        Assert.True(page2.IsHandleCreated);
        Assert.Equal(1, pageInvalidatedCallCount2);
        Assert.Equal(0, pageStyleChangedCallCount2);
        Assert.Equal(1, pageCreatedCallCount2);

        // Deselect again.
        control.DeselectTab(0);
        Assert.Same(page2, control.SelectedTab);
        Assert.Equal(1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.True(page2.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(page1.IsHandleCreated);
        Assert.Equal(1, pageInvalidatedCallCount1);
        Assert.Equal(0, pageStyleChangedCallCount1);
        Assert.Equal(0, pageCreatedCallCount1);
        Assert.True(page2.IsHandleCreated);
        Assert.Equal(1, pageInvalidatedCallCount2);
        Assert.Equal(0, pageStyleChangedCallCount2);
        Assert.Equal(1, pageCreatedCallCount2);

        // Deselect last.
        control.DeselectTab(1);
        Assert.Same(page1, control.SelectedTab);
        Assert.Equal(0, control.SelectedIndex);
        Assert.True(page1.Visible);
        Assert.False(page2.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(page1.IsHandleCreated);
        Assert.Equal(2, pageInvalidatedCallCount1);
        Assert.Equal(0, pageStyleChangedCallCount1);
        Assert.Equal(0, pageCreatedCallCount1);
        Assert.True(page2.IsHandleCreated);
        Assert.Equal(1, pageInvalidatedCallCount2);
        Assert.Equal(0, pageStyleChangedCallCount2);
        Assert.Equal(1, pageCreatedCallCount2);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void TabControl_DeselectTab_InvalidIndexWithoutPages_ThrowsArgumentOutOfRangeException(int index)
    {
        using TabControl control = new();
        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.DeselectTab(index));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(2)]
    [InlineData(3)]
    public void TabControl_DeselectTab_InvalidIndexWithPages_ThrowsArgumentOutOfRangeException(int index)
    {
        using TabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.DeselectTab(index));
    }

    [WinFormsFact]
    public void TabControl_Dispose_InvokeWithImageList_DetachesImageList()
    {
        using TabControl control = new();
        using ImageList imageList = new();
        control.ImageList = imageList;
        control.Dispose();
        Assert.Same(imageList, control.ImageList);

        imageList.Dispose();
        Assert.Same(imageList, control.ImageList);
    }

    [WinFormsFact]
    public void TabControl_Dispose_InvokeDisposingWithImageList_DetachesImageList()
    {
        using SubTabControl control = new();
        using ImageList imageList = new();
        control.ImageList = imageList;
        control.Dispose(true);
        Assert.Same(imageList, control.ImageList);

        imageList.Dispose();
        Assert.Same(imageList, control.ImageList);
    }

    [WinFormsFact]
    public void TabControl_Dispose_InvokeNotDisposingWithImageList_DoesNotDetachImageList()
    {
        using SubTabControl control = new();
        using ImageList imageList = new();
        control.ImageList = imageList;
        control.Dispose(false);
        Assert.Same(imageList, control.ImageList);

        imageList.Dispose();
        Assert.Null(control.ImageList);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    public void TabControl_GetControl_Invoke_ReturnsExpected(int index)
    {
        using TabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        Assert.Same(control.TabPages[index], control.GetControl(index));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void TabControl_GetControl_InvokeWithoutPages_ThrowsArgumentOutOfRangeException(int index)
    {
        using SubTabControl control = new();
        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetControl(index));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(2)]
    [InlineData(3)]
    public void TabControl_GetControl_InvokeInvalidIndexWithPages_ThrowsArgumentOutOfRangeException(int index)
    {
        using TabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetControl(index));
    }

    [WinFormsFact]
    public void TabControl_GetTabRect_InvokeWithoutHandle_ReturnsExpectedAndCreatedHandle()
    {
        using TabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);

        Rectangle rect1 = control.GetTabRect(0);
        Assert.True(rect1.X >= 0);
        Assert.True(rect1.Y >= 0);
        Assert.True(rect1.Width > 0);
        Assert.True(rect1.Height > 0);
        Assert.Equal(rect1, control.GetTabRect(0));
        Assert.True(control.IsHandleCreated);

        Rectangle rect2 = control.GetTabRect(1);
        Assert.True(rect2.X >= rect1.X + rect1.Width);
        Assert.Equal(rect2.Y, rect1.Y);
        Assert.True(rect2.Width > 0);
        Assert.True(rect2.Height > 0);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabControl_GetTabRect_InvokeWithHandle_ReturnsExpected()
    {
        using TabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Rectangle rect1 = control.GetTabRect(0);
        Assert.True(rect1.X >= 0);
        Assert.True(rect1.Y >= 0);
        Assert.True(rect1.Width > 0);
        Assert.True(rect1.Height > 0);
        Assert.Equal(rect1, control.GetTabRect(0));
        Assert.True(control.IsHandleCreated);

        Rectangle rect2 = control.GetTabRect(1);
        Assert.True(rect2.X >= rect1.X + rect1.Width);
        Assert.Equal(rect2.Y, rect1.Y);
        Assert.True(rect2.Width > 0);
        Assert.True(rect2.Height > 0);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> GetTabRect_InvokeCustomGetItemRect_TestData()
    {
        yield return new object[] { default(RECT), Rectangle.Empty };
        yield return new object[] { new RECT(1, 2, 3, 4), new Rectangle(1, 2, 2, 2) };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetTabRect_InvokeCustomGetItemRect_TestData))]
    public void TabControl_GetTabRect_InvokeCustomGetItemRect_ReturnsExpected(object getItemRectResult, Rectangle expected)
    {
        using CustomGetItemRectTabControl control = new()
        {
            GetItemRectResult = (RECT)getItemRectResult
        };
        using TabPage page = new();
        control.TabPages.Add(page);
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        Assert.Equal(expected, control.GetTabRect(0));
    }

    private class CustomGetItemRectTabControl : TabControl
    {
        public RECT GetItemRectResult { get; set; }

        protected override unsafe void WndProc(ref Message m)
        {
            if (m.Msg == (int)PInvoke.TCM_GETITEMRECT)
            {
                RECT* pRect = (RECT*)m.LParam;
                *pRect = GetItemRectResult;
                m.Result = 1;
                return;
            }

            base.WndProc(ref m);
        }
    }

    [WinFormsFact]
    public void TabControl_GetTabRect_InvokeInvalidGetItemRect_ReturnsExpected()
    {
        using InvalidGetItemRectTabControl control = new();
        using TabPage page = new();
        control.TabPages.Add(page);
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.MakeInvalid = true;
        Assert.Equal(new Rectangle(1, 2, 2, 2), control.GetTabRect(0));
    }

    private class InvalidGetItemRectTabControl : TabControl
    {
        public bool MakeInvalid { get; set; }

        protected override unsafe void WndProc(ref Message m)
        {
            if (MakeInvalid && m.Msg == (int)PInvoke.TCM_GETITEMRECT)
            {
                RECT* pRect = (RECT*)m.LParam;
                *pRect = new RECT(1, 2, 3, 4);
                m.Result = IntPtr.Zero;
                return;
            }

            base.WndProc(ref m);
        }
    }

    [WinFormsFact]
    public void TabControl_GetTabRect_InvalidIndexEmpty_ThrowsArgumentOutOfRangeException()
    {
        using TabControl control = new();
        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetTabRect(-1));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetTabRect(0));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetTabRect(1));
    }

    [WinFormsFact]
    public void TabControl_GetTabRect_InvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException()
    {
        using TabControl control = new();
        using TabPage page1 = new();
        control.TabPages.Add(page1);

        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetTabRect(-1));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetTabRect(1));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetTabRect(2));
    }

    [WinFormsFact]
    public void TabControl_GetTabRect_InvalidIndexWithHandleEmpty_ThrowsArgumentOutOfRangeException()
    {
        using TabControl control = new();
        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetTabRect(-1));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetTabRect(0));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetTabRect(1));
    }

    [WinFormsFact]
    public void TabControl_GetTabRect_InvalidIndexWithHandleNotEmpty_ThrowsArgumentOutOfRangeException()
    {
        using TabControl control = new();
        using TabPage page1 = new();
        control.TabPages.Add(page1);
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetTabRect(-1));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetTabRect(1));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetTabRect(2));
    }

    [WinFormsFact]
    public void TabControl_GetAutoSizeMode_Invoke_ReturnsExpected()
    {
        using SubTabControl control = new();
        Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
    }

    [WinFormsFact]
    public void TabControl_GetItems_InvokeWithoutPages_ReturnsExpected()
    {
        using SubTabControl control = new();
        object[] result = Assert.IsType<TabPage[]>(control.GetItems());
        Assert.Empty(result);
        Assert.Same(result, control.GetItems());
    }

    [WinFormsFact]
    public void TabControl_GetItems_InvokeWithPages_ReturnsExpected()
    {
        using SubTabControl control = new();
        using TabPage page1 = new();
        using SubTabPage page2 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        object[] result = Assert.IsType<TabPage[]>(control.GetItems());
        Assert.Equal([page1, page2], result);
        Assert.NotSame(result, control.GetItems());
    }

    [WinFormsTheory]
    [InlineData(typeof(object))]
    [InlineData(typeof(TabPage))]
    public void TabControl_GetItems_InvokeTypeWithoutPages_ReturnsExpected(Type baseType)
    {
        using SubTabControl control = new();
        object[] result = control.GetItems(baseType);
        Assert.Empty(result);
        Assert.IsType(baseType.MakeArrayType(), result);
        Assert.NotSame(result, control.GetItems());
    }

    [WinFormsTheory]
    [InlineData(typeof(TabPage))]
    [InlineData(typeof(Control))]
    [InlineData(typeof(object))]
    public void TabControl_GetItems_InvokeTypeBaseTypeWithPages_ReturnsExpected(Type baseType)
    {
        using SubTabControl control = new();
        using TabPage page1 = new();
        using SubTabPage page2 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        object[] result = control.GetItems(baseType);
        Assert.IsType(baseType.MakeArrayType(), result);
        Assert.Equal([page1, page2], result);
        Assert.NotSame(result, control.GetItems(baseType));
    }

    [WinFormsFact]
    public void TabControl_GetItems_InvokeTypeSubTypeWithPages_ReturnsExpected()
    {
        using SubTabControl control = new();
        using SubTabPage page = new();
        control.TabPages.Add(page);
        object[] result = Assert.IsType<SubTabPage[]>(control.GetItems(typeof(SubTabPage)));
        Assert.Equal([page], result);
        Assert.NotSame(result, control.GetItems(typeof(SubTabPage)));
    }

    [WinFormsFact]
    public void TabControl_GetItems_InvokeNullBaseType_ThrowsArgumentNullException()
    {
        using SubTabControl control = new();
        Assert.Throws<ArgumentNullException>("elementType", () => control.GetItems(null));
    }

    [WinFormsTheory]
    [InlineData(typeof(int))]
    public void TabControl_GetItems_InvokeInvalidTypeWithoutPages_ThrowsInvalidCastException(Type baseType)
    {
        using SubTabControl control = new();
        Assert.Throws<InvalidCastException>(() => control.GetItems(baseType));
    }

    [WinFormsFact]
    public void TabControl_GetItems_InvokeInvalidTypeWithPages_ThrowsInvalidCastException()
    {
        using SubTabControl control = new();
        using TabPage page1 = new();
        using SubTabPage page2 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        Assert.Throws<InvalidCastException>(() => control.GetItems(typeof(int)));
    }

    [WinFormsFact]
    public void TabControl_GetItems_InvokeInvalidInheritedTypeWithPages_ThrowsArrayTypeMismatchException()
    {
        using SubTabControl control = new();
        using TabPage page1 = new();
        using SubTabPage page2 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        Assert.Throws<ArrayTypeMismatchException>(() => control.GetItems(typeof(SubTabPage)));
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
    [InlineData(ControlStyles.UseTextForAccessibility, true)]
    [InlineData((ControlStyles)0, true)]
    [InlineData((ControlStyles)int.MaxValue, false)]
    [InlineData((ControlStyles)(-1), false)]
    public void TabControl_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
    {
        using SubTabControl control = new();
        Assert.Equal(expected, control.GetStyle(flag));

        // Call again to test caching.
        Assert.Equal(expected, control.GetStyle(flag));
    }

    [WinFormsFact]
    public void TabControl_GetTopLevel_Invoke_ReturnsExpected()
    {
        using SubTabControl control = new();
        Assert.False(control.GetTopLevel());
    }

    [WinFormsFact]
    public void TabControl_GetToolTipText_Invoke_ReturnsExpected()
    {
        using SubTabControl control = new();
        using TabPage item = new()
        {
            ToolTipText = "text"
        };
        Assert.Equal("text", control.GetToolTipText(item));
    }

    [WinFormsFact]
    public void TabControl_GetToolTipText_NullItem_ThrowsArgumentNullException()
    {
        using SubTabControl control = new();
        Assert.Throws<ArgumentNullException>("item", () => control.GetToolTipText(null));
    }

    [WinFormsFact]
    public void TabControl_GetToolTipText_ItemNotTabPage_ThrowsArgumentException()
    {
        using SubTabControl control = new();
        Assert.Throws<ArgumentException>("item", () => control.GetToolTipText(new object()));
    }

    public static IEnumerable<object[]> IsInputKey_TestData()
    {
        yield return new object[] { Keys.Tab, false };
        yield return new object[] { Keys.Return, false };
        yield return new object[] { Keys.Escape, false };
        yield return new object[] { Keys.A, false };
        yield return new object[] { Keys.C, false };
        yield return new object[] { Keys.Insert, false };
        yield return new object[] { Keys.Space, false };
        yield return new object[] { Keys.Home, true };
        yield return new object[] { Keys.End, true };
        ;
        yield return new object[] { Keys.Back, false };
        yield return new object[] { Keys.Next, true };
        yield return new object[] { Keys.Prior, true };
        yield return new object[] { Keys.Delete, false };
        yield return new object[] { Keys.D0, false };
        yield return new object[] { Keys.NumPad0, false };
        yield return new object[] { Keys.F1, false };
        yield return new object[] { Keys.F2, false };
        yield return new object[] { Keys.F3, false };
        yield return new object[] { Keys.F4, false };
        yield return new object[] { Keys.RButton, false };
        yield return new object[] { Keys.PageUp, true };
        yield return new object[] { Keys.PageDown, true };
        yield return new object[] { Keys.None, false };

        yield return new object[] { Keys.Control | Keys.Tab, false };
        yield return new object[] { Keys.Control | Keys.Return, false };
        yield return new object[] { Keys.Control | Keys.Escape, false };
        yield return new object[] { Keys.Control | Keys.A, false };
        yield return new object[] { Keys.Control | Keys.C, false };
        yield return new object[] { Keys.Control | Keys.Insert, false };
        yield return new object[] { Keys.Control | Keys.Space, false };
        yield return new object[] { Keys.Control | Keys.Home, true };
        yield return new object[] { Keys.Control | Keys.End, true };
        ;
        yield return new object[] { Keys.Control | Keys.Back, false };
        yield return new object[] { Keys.Control | Keys.Next, true };
        yield return new object[] { Keys.Control | Keys.Prior, true };
        yield return new object[] { Keys.Control | Keys.Delete, false };
        yield return new object[] { Keys.Control | Keys.D0, false };
        yield return new object[] { Keys.Control | Keys.NumPad0, false };
        yield return new object[] { Keys.Control | Keys.F1, false };
        yield return new object[] { Keys.Control | Keys.F2, false };
        yield return new object[] { Keys.Control | Keys.F3, false };
        yield return new object[] { Keys.Control | Keys.F4, false };
        yield return new object[] { Keys.Control | Keys.RButton, false };
        yield return new object[] { Keys.Control | Keys.PageUp, true };
        yield return new object[] { Keys.Control | Keys.PageDown, true };
        yield return new object[] { Keys.Control | Keys.None, false };

        yield return new object[] { Keys.Alt | Keys.Tab, false };
        yield return new object[] { Keys.Alt | Keys.Up, false };
        yield return new object[] { Keys.Alt | Keys.Down, false };
        yield return new object[] { Keys.Alt | Keys.Left, false };
        yield return new object[] { Keys.Alt | Keys.Right, false };
        yield return new object[] { Keys.Alt | Keys.Return, false };
        yield return new object[] { Keys.Alt | Keys.Escape, false };
        yield return new object[] { Keys.Alt | Keys.A, false };
        yield return new object[] { Keys.Alt | Keys.C, false };
        yield return new object[] { Keys.Alt | Keys.Insert, false };
        yield return new object[] { Keys.Alt | Keys.Space, false };
        yield return new object[] { Keys.Alt | Keys.Home, false };
        yield return new object[] { Keys.Alt | Keys.End, false };
        ;
        yield return new object[] { Keys.Alt | Keys.Back, false };
        yield return new object[] { Keys.Alt | Keys.Next, false };
        yield return new object[] { Keys.Alt | Keys.Prior, false };
        yield return new object[] { Keys.Alt | Keys.Delete, false };
        yield return new object[] { Keys.Alt | Keys.D0, false };
        yield return new object[] { Keys.Alt | Keys.NumPad0, false };
        yield return new object[] { Keys.Alt | Keys.F1, false };
        yield return new object[] { Keys.Alt | Keys.F2, false };
        yield return new object[] { Keys.Alt | Keys.F3, false };
        yield return new object[] { Keys.Alt | Keys.F4, false };
        yield return new object[] { Keys.Alt | Keys.RButton, false };
        yield return new object[] { Keys.Alt | Keys.PageUp, false };
        yield return new object[] { Keys.Alt | Keys.PageDown, false };
        yield return new object[] { Keys.Alt | Keys.None, false };
    }

    [WinFormsTheory]
    [MemberData(nameof(IsInputKey_TestData))]
    [InlineData(Keys.Up, false)]
    [InlineData(Keys.Down, false)]
    [InlineData(Keys.Left, false)]
    [InlineData(Keys.Right, false)]
    [InlineData(Keys.Control | Keys.Up, false)]
    [InlineData(Keys.Control | Keys.Down, false)]
    [InlineData(Keys.Control | Keys.Left, false)]
    [InlineData(Keys.Control | Keys.Right, false)]
    public void TabControl_IsInputKey_InvokeWithoutHandle_ReturnsExpected(Keys keyData, bool expected)
    {
        using SubTabControl control = new();
        Assert.Equal(expected, control.IsInputKey(keyData));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(IsInputKey_TestData))]
    [InlineData(Keys.Up, true)]
    [InlineData(Keys.Down, true)]
    [InlineData(Keys.Left, true)]
    [InlineData(Keys.Right, true)]
    [InlineData(Keys.Control | Keys.Up, true)]
    [InlineData(Keys.Control | Keys.Down, true)]
    [InlineData(Keys.Control | Keys.Left, true)]
    [InlineData(Keys.Control | Keys.Right, true)]
    public void TabControl_IsInputKey_InvokeWithHandle_ReturnsExpected(Keys keyData, bool expected)
    {
        using SubTabControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(expected, control.IsInputKey(keyData));
        Assert.True(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> TabControlEventArgs_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new TabControlEventArgs(null, 0, TabControlAction.Deselecting) };
    }

    [WinFormsTheory]
    [MemberData(nameof(TabControlEventArgs_TestData))]
    public void TabControl_OnDeselected_Invoke_CallsDeselected(TabControlEventArgs eventArgs)
    {
        using SubTabControl control = new();
        int callCount = 0;
        void handler(object sender, TabControlEventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        }

        // Call with handler.
        control.Deselected += handler;
        control.OnDeselected(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.Deselected -= handler;
        control.OnDeselected(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(TabControlEventArgs_TestData))]
    public void TabControl_OnDeselected_InvokeWithSelectedTab_CallsDeselected(TabControlEventArgs eventArgs)
    {
        using SubTabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        control.SelectedIndex = 0;
        int callCount = 0;
        int leaveCallCount1 = 0;
        int leaveCallCount2 = 0;
        void handler(object sender, TabControlEventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            Assert.Equal(callCount, leaveCallCount1);
            callCount++;
        }

        void leaveHandler1(object sender, EventArgs e)
        {
            Assert.Same(page1, sender);
            Assert.Same(EventArgs.Empty, e);
            leaveCallCount1++;
        }

        void leaveHandler2(object sender, EventArgs e) => leaveCallCount2++;

        // Call with handler.
        control.Deselected += handler;
        page1.Leave += leaveHandler1;
        page2.Leave += leaveHandler2;
        control.OnDeselected(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(1, leaveCallCount1);
        Assert.Equal(0, leaveCallCount2);

        // Remove handler.
        control.Deselected -= handler;
        page1.Leave -= leaveHandler1;
        page2.Leave -= leaveHandler2;
        control.OnDeselected(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(1, leaveCallCount1);
        Assert.Equal(0, leaveCallCount2);
    }

    public static IEnumerable<object[]> TabControlCancelEventArgs_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new TabControlCancelEventArgs(null, 0, true, TabControlAction.Deselecting) };
    }

    [WinFormsTheory]
    [MemberData(nameof(TabControlCancelEventArgs_TestData))]
    public void TabControl_OnDeselecting_Invoke_CallsDeselecting(TabControlCancelEventArgs eventArgs)
    {
        using SubTabControl control = new();
        int callCount = 0;
        void handler(object sender, TabControlCancelEventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        }

        // Call with handler.
        control.Deselecting += handler;
        control.OnDeselecting(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.Deselecting -= handler;
        control.OnDeselecting(eventArgs);
        Assert.Equal(1, callCount);
    }

    public static IEnumerable<object[]> DrawItemEventArgs_TestData()
    {
        yield return new object[] { null };

        Bitmap bitmap = new(10, 10);
        Graphics graphics = Graphics.FromImage(bitmap);
        yield return new object[] { new DrawItemEventArgs(graphics, null, new Rectangle(1, 2, 3, 4), 0, DrawItemState.Checked) };
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawItemEventArgs_TestData))]
    public void TabControl_OnDrawItem_Invoke_CallsDrawItem(DrawItemEventArgs eventArgs)
    {
        using SubTabControl control = new();
        int callCount = 0;
        void handler(object sender, DrawItemEventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        }

        // Call with handler.
        control.DrawItem += handler;
        control.OnDrawItem(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.DrawItem -= handler;
        control.OnDrawItem(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void TabControl_OnEnter_Invoke_CallsEnter(EventArgs eventArgs)
    {
        using SubTabControl control = new();
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
    public void TabControl_OnEnter_InvokeWithSelectedTab_CallsEnter(EventArgs eventArgs)
    {
        using SubTabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        control.SelectedTab = page2;

        int callCount = 0;
        int childCallCount1 = 0;
        int childCallCount2 = 0;
        page1.Enter += (sender, e) => childCallCount1++;
        page2.Enter += (sender, e) =>
        {
            Assert.Same(page2, sender);
            Assert.Same(eventArgs, e);
            childCallCount2++;
        };
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);
            callCount++;
        };

        // Call with handler.
        control.Enter += handler;
        control.OnEnter(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(1, childCallCount2);

        // Remove handler.
        control.Enter -= handler;
        control.OnEnter(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(2, childCallCount2);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void TabControl_OnHandleCreated_Invoke_CallsHandleCreated(EventArgs eventArgs)
    {
        using SubTabControl control = new();
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
    public void TabControl_OnHandleCreated_InvokeWithHandle_CallsHandleCreated(EventArgs eventArgs)
    {
        using SubTabControl control = new();
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
    [NewAndDefaultData<EventArgs>]
    public void TabControl_OnHandleDestroyed_Invoke_CallsHandleDestroyed(EventArgs eventArgs)
    {
        using SubTabControl control = new();
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
    public void TabControl_OnHandleDestroyed_InvokeWithHandle_CallsHandleDestroyed(EventArgs eventArgs)
    {
        using SubTabControl control = new();
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

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void TabControl_OnLeave_Invoke_CallsLeave(EventArgs eventArgs)
    {
        using SubTabControl control = new();
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
    [NewAndDefaultData<EventArgs>]
    public void TabControl_OnLeave_InvokeWithSelectedTab_CallsLeave(EventArgs eventArgs)
    {
        using SubTabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        control.SelectedTab = page2;

        int callCount = 0;
        int childCallCount1 = 0;
        int childCallCount2 = 0;
        page1.Leave += (sender, e) => childCallCount1++;
        page2.Leave += (sender, e) =>
        {
            Assert.Same(page2, sender);
            Assert.Same(eventArgs, e);
            childCallCount2++;
        };
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(1, childCallCount2);
            callCount++;
        };

        // Call with handler.
        control.Leave += handler;
        control.OnLeave(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(1, childCallCount2);

        // Remove handler.
        control.Leave -= handler;
        control.OnLeave(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(2, childCallCount2);
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
    public void TabControl_OnRightToLeftLayoutChanged_Invoke_CallsRightToLeftLayoutChanged(RightToLeft rightToLeft, EventArgs eventArgs)
    {
        using SubTabControl control = new()
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
    public void TabControl_OnRightToLeftLayoutChanged_InvokeWithHandle_CallsRightToLeftLayoutChanged(RightToLeft rightToLeft, EventArgs eventArgs, int expectedCreatedCallCount)
    {
        using SubTabControl control = new()
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
        Assert.Equal(expectedCreatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Remove handler.
        control.RightToLeftLayoutChanged -= handler;
        control.OnRightToLeftLayoutChanged(eventArgs);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedCreatedCallCount * 2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount * 2, createdCallCount);
    }

    [WinFormsFact]
    public void TabControl_OnRightToLeftLayoutChanged_InvokeInDisposing_DoesNotCallRightToLeftLayoutChanged()
    {
        using SubTabControl control = new()
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
    [MemberData(nameof(TabControlEventArgs_TestData))]
    public void TabControl_OnSelected_InvokeWithSelectedTab_CallsSelected(TabControlEventArgs eventArgs)
    {
        using SubTabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        control.SelectedIndex = 0;
        int callCount = 0;
        int enterCallCount1 = 0;
        int enterCallCount2 = 0;
        void handler(object sender, TabControlEventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            Assert.Equal(callCount, enterCallCount1);
            callCount++;
        }

        void enterHandler1(object sender, EventArgs e)
        {
            Assert.Same(page1, sender);
            Assert.Same(EventArgs.Empty, e);
            enterCallCount1++;
        }

        void enterHandler2(object sender, EventArgs e) => enterCallCount2++;

        // Call with handler.
        control.Selected += handler;
        page1.Enter += enterHandler1;
        page2.Enter += enterHandler2;
        control.OnSelected(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(1, enterCallCount1);
        Assert.Equal(0, enterCallCount2);

        // Remove handler.
        control.Selected -= handler;
        page1.Enter -= enterHandler1;
        page2.Enter -= enterHandler2;
        control.OnSelected(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(1, enterCallCount1);
        Assert.Equal(0, enterCallCount2);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void TabControl_OnSelectedIndexChanged_Invoke_CallsSelectedIndexChanged(EventArgs eventArgs)
    {
        using SubTabControl control = new();
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        }

        // Call with handler.
        control.SelectedIndexChanged += handler;
        control.OnSelectedIndexChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.SelectedIndexChanged -= handler;
        control.OnSelectedIndexChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void TabControl_OnSelectedIndexChanged_InvokeWithPages_CallsSelectedIndexChanged(EventArgs eventArgs)
    {
        using SubTabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        }

        // Call with handler.
        control.SelectedIndexChanged += handler;
        control.OnSelectedIndexChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
        Assert.False(page1.Visible);
        Assert.False(page2.Visible);

        // Remove handler.
        control.SelectedIndexChanged -= handler;
        control.OnSelectedIndexChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
        Assert.False(page1.Visible);
        Assert.False(page2.Visible);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void TabControl_OnSelectedIndexChanged_InvokeWithHandle_CallsSelectedIndexChanged(EventArgs eventArgs)
    {
        using SubTabControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        }

        // Call with handler.
        control.SelectedIndexChanged += handler;
        control.OnSelectedIndexChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.OnSelectedIndexChanged(eventArgs);
        Assert.Equal(2, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.SelectedIndexChanged -= handler;
        control.OnSelectedIndexChanged(eventArgs);
        Assert.Equal(2, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void TabControl_OnSelectedIndexChanged_InvokeWithHandleWithPages_CallsSelectedIndexChanged(EventArgs eventArgs)
    {
        using SubTabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        }

        // Call with handler.
        control.SelectedIndexChanged += handler;
        control.OnSelectedIndexChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(page1.Visible);
        Assert.False(page2.Visible);

        // Remove handler.
        control.SelectedIndexChanged -= handler;
        control.OnSelectedIndexChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(page1.Visible);
        Assert.False(page2.Visible);
    }

    [WinFormsTheory]
    [MemberData(nameof(TabControlCancelEventArgs_TestData))]
    public void TabControl_OnSelecting_Invoke_CallsSelecting(TabControlCancelEventArgs eventArgs)
    {
        using SubTabControl control = new();
        int callCount = 0;
        void handler(object sender, TabControlCancelEventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        }

        // Call with handler.
        control.Selecting += handler;
        control.OnSelecting(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.Selecting -= handler;
        control.OnSelecting(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void TabControl_RecreateHandle_InvokeWithoutHandle_Nop()
    {
        using SubTabControl control = new();
        control.RecreateHandle();
        Assert.False(control.IsHandleCreated);

        // Invoke again.
        control.RecreateHandle();
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabControl_RecreateHandle_InvokeEmptyWithHandle_Success()
    {
        using SubTabControl control = new();
        IntPtr handle1 = control.Handle;
        Assert.NotEqual(IntPtr.Zero, handle1);
        Assert.Empty(control.Controls);
        Assert.Empty(control.TabPages);
        Assert.True(control.IsHandleCreated);

        control.RecreateHandle();
        IntPtr handle2 = control.Handle;
        Assert.NotEqual(IntPtr.Zero, handle2);
        Assert.NotEqual(handle1, handle2);
        Assert.Empty(control.Controls);
        Assert.Empty(control.TabPages);
        Assert.True(control.IsHandleCreated);

        // Invoke again.
        control.RecreateHandle();
        IntPtr handle3 = control.Handle;
        Assert.NotEqual(IntPtr.Zero, handle3);
        Assert.NotEqual(handle2, handle3);
        Assert.Empty(control.Controls);
        Assert.Empty(control.TabPages);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabControl_RecreateHandle_InvokeNotEmptyWithHandle_Success()
    {
        using TabPage page1 = new();
        using TabPage page2 = new();
        using SubTabControl control = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);

        IntPtr handle1 = control.Handle;
        Assert.NotEqual(IntPtr.Zero, handle1);
        Assert.Equal(new Control[] { page1, page2 }, control.Controls.Cast<Control>());
        Assert.Equal(new TabPage[] { page1, page2 }, control.TabPages.Cast<TabPage>());
        Assert.True(control.IsHandleCreated);

        control.RecreateHandle();
        IntPtr handle2 = control.Handle;
        Assert.NotEqual(IntPtr.Zero, handle2);
        Assert.NotEqual(handle1, handle2);
        Assert.Equal(new Control[] { page1, page2 }, control.Controls.Cast<Control>());
        Assert.Equal(new TabPage[] { page1, page2 }, control.TabPages.Cast<TabPage>());
        Assert.True(control.IsHandleCreated);

        // Invoke again.
        control.RecreateHandle();
        IntPtr handle3 = control.Handle;
        Assert.NotEqual(IntPtr.Zero, handle3);
        Assert.NotEqual(handle2, handle3);
        Assert.Equal(new Control[] { page1, page2 }, control.Controls.Cast<Control>());
        Assert.Equal(new TabPage[] { page1, page2 }, control.TabPages.Cast<TabPage>());
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData("Text", "Text")]
    [InlineData("&&Text", "&&Text")]
    [InlineData("&", "&&")]
    [InlineData("&Text", "&&Text")]
    public unsafe void TabControl_RecreateHandle_GetItemsWithHandle_Success(string text, string expectedText)
    {
        using SubTabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new()
        {
            Text = text,
            ImageIndex = 1
        };
        using NullTextTabPage page3 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        control.TabPages.Add(page3);

        control.RecreateHandle();
        Assert.Equal(3, (int)PInvokeCore.SendMessage(control, PInvoke.TCM_GETITEMCOUNT));

        char* buffer = stackalloc char[256];
        TCITEMW item = default;
        item.cchTextMax = int.MaxValue;
        item.pszText = buffer;
        item.dwStateMask = (TAB_CONTROL_ITEM_STATE)uint.MaxValue;
        item.mask = (TCITEMHEADERA_MASK)uint.MaxValue;

        // Get item 0.
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.TCM_GETITEMW, 0, ref item));
        Assert.Equal(TAB_CONTROL_ITEM_STATE.TCIS_BUTTONPRESSED, item.dwState);
        Assert.Equal(IntPtr.Zero, (nint)item.lParam);
        Assert.Equal(int.MaxValue, item.cchTextMax);
        Assert.Empty(new string(item.pszText));
        Assert.Equal(-1, item.iImage);

        // Get item 1.
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.TCM_GETITEMW, 1, ref item));
        Assert.Equal((TAB_CONTROL_ITEM_STATE)0, item.dwState);
        Assert.Equal(IntPtr.Zero, (nint)item.lParam);
        Assert.Equal(int.MaxValue, item.cchTextMax);
        Assert.Equal(expectedText, new string(item.pszText));
        Assert.Equal(1, item.iImage);

        // Get item 2.
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.TCM_GETITEMW, 2, ref item));
        Assert.Equal((TAB_CONTROL_ITEM_STATE)0, item.dwState);
        Assert.Equal(IntPtr.Zero, (nint)item.lParam);
        Assert.Equal(int.MaxValue, item.cchTextMax);
        Assert.Empty(new string(item.pszText));
        Assert.Equal(-1, item.iImage);
    }

    [WinFormsFact]
    public void TabControl_RemoveAll_InvokeEmpty_Success()
    {
        using SubTabControl control = new();
        int layoutCallCount = 0;
        void layoutHandler(object sender, LayoutEventArgs e) => layoutCallCount++;
        control.Layout += layoutHandler;
        int controlRemovedCallCount = 0;
        control.ControlRemoved += (sender, e) => controlRemovedCallCount++;

        try
        {
            control.RemoveAll();
            Assert.Empty(control.TabPages);
            Assert.Empty(control.Controls);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, controlRemovedCallCount);
            Assert.False(control.IsHandleCreated);

            // RemoveAll again.
            control.RemoveAll();
            Assert.Empty(control.TabPages);
            Assert.Empty(control.Controls);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, controlRemovedCallCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Layout -= layoutHandler;
        }
    }

    [WinFormsFact]
    public void TabControl_RemoveAll_InvokeNotEmpty_Success()
    {
        using SubTabControl control = new();
        using TabPage child1 = new();
        using TabPage child2 = new();
        using TabPage child3 = new();
        control.TabPages.Add(child1);
        control.TabPages.Add(child2);
        control.TabPages.Add(child3);
        int layoutCallCount = 0;
        void layoutHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(child3, e.AffectedControl);
            Assert.Equal("Parent", e.AffectedProperty);
            layoutCallCount++;
        }

        control.Layout += layoutHandler;
        int controlRemovedCallCount = 0;
        control.ControlRemoved += (sender, e) => controlRemovedCallCount++;

        try
        {
            control.RemoveAll();
            Assert.Empty(control.TabPages);
            Assert.Empty(control.Controls);
            Assert.Null(child1.Parent);
            Assert.Null(child2.Parent);
            Assert.Null(child3.Parent);
            Assert.Equal(1, layoutCallCount);
            Assert.Equal(3, controlRemovedCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child1.IsHandleCreated);
            Assert.False(child2.IsHandleCreated);
            Assert.False(child3.IsHandleCreated);

            // RemoveAll again.
            control.RemoveAll();
            Assert.Empty(control.TabPages);
            Assert.Empty(control.Controls);
            Assert.Null(child1.Parent);
            Assert.Null(child2.Parent);
            Assert.Null(child3.Parent);
            Assert.Equal(1, layoutCallCount);
            Assert.Equal(3, controlRemovedCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child1.IsHandleCreated);
            Assert.False(child2.IsHandleCreated);
            Assert.False(child3.IsHandleCreated);
        }
        finally
        {
            control.Layout -= layoutHandler;
        }
    }

    [WinFormsFact]
    public void TabControl_RemoveAll_InvokeEmptyWithHandle_Success()
    {
        using SubTabControl control = new();

        int controlRemovedCallCount = 0;
        control.ControlRemoved += (sender, e) => controlRemovedCallCount++;
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int parentInvalidatedCallCount = 0;
        control.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        control.HandleCreated += (sender, e) => parentCreatedCallCount++;
        int layoutCallCount = 0;
        void layoutHandler(object sender, LayoutEventArgs e) => layoutCallCount++;
        control.Layout += layoutHandler;

        try
        {
            control.RemoveAll();
            Assert.Empty(control.TabPages);
            Assert.Empty(control.Controls);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, controlRemovedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // RemoveAll again.
            control.RemoveAll();
            Assert.Empty(control.TabPages);
            Assert.Empty(control.Controls);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, controlRemovedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
        }
        finally
        {
            control.Layout -= layoutHandler;
        }
    }

    [WinFormsFact]
    public void TabControl_RemoveAll_InvokeNotEmptyWithHandle_Success()
    {
        using SubTabControl control = new();
        using TabPage child1 = new();
        using TabPage child2 = new();
        using TabPage child3 = new();
        control.TabPages.Add(child1);
        control.TabPages.Add(child2);
        control.TabPages.Add(child3);

        int controlRemovedCallCount = 0;
        control.ControlRemoved += (sender, e) => controlRemovedCallCount++;
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int parentInvalidatedCallCount = 0;
        control.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        control.HandleCreated += (sender, e) => parentCreatedCallCount++;
        int layoutCallCount = 0;
        void layoutHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(child3, e.AffectedControl);
            Assert.Equal("Parent", e.AffectedProperty);
            layoutCallCount++;
        }

        control.Layout += layoutHandler;

        try
        {
            control.RemoveAll();
            Assert.Empty(control.TabPages);
            Assert.Empty(control.Controls);
            Assert.Null(child1.Parent);
            Assert.Null(child2.Parent);
            Assert.Null(child3.Parent);
            Assert.Equal(1, layoutCallCount);
            Assert.Equal(3, controlRemovedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
            Assert.True(child1.IsHandleCreated);
            Assert.False(child2.IsHandleCreated);
            Assert.False(child3.IsHandleCreated);

            // RemoveAll again.
            control.RemoveAll();
            Assert.Empty(control.TabPages);
            Assert.Empty(control.Controls);
            Assert.Null(child1.Parent);
            Assert.Null(child2.Parent);
            Assert.Null(child3.Parent);
            Assert.Equal(1, layoutCallCount);
            Assert.Equal(3, controlRemovedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
            Assert.True(child1.IsHandleCreated);
            Assert.False(child2.IsHandleCreated);
            Assert.False(child3.IsHandleCreated);
        }
        finally
        {
            control.Layout -= layoutHandler;
        }
    }

    [WinFormsFact]
    public void TabControl_RemoveAll_GetItemsWithHandle_Success()
    {
        using SubTabControl control = new();
        using TabPage child1 = new();
        using TabPage child2 = new();
        using TabPage child3 = new();
        control.TabPages.Add(child1);
        control.TabPages.Add(child2);
        control.TabPages.Add(child3);

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.RemoveAll();
        Assert.Equal(0, (int)PInvokeCore.SendMessage(control, PInvoke.TCM_GETITEMCOUNT));
    }

    [WinFormsFact]
    public void TabControl_SelectTab_InvokeTabPageWithPages_GetReturnsExpected()
    {
        using TabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);

        // Set valid.
        control.SelectTab(page2);
        Assert.Same(page2, control.SelectedTab);
        Assert.Equal(1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.False(page2.Visible);
        Assert.False(control.IsHandleCreated);
        Assert.False(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);

        // Set same.
        control.SelectTab(page2);
        Assert.Same(page2, control.SelectedTab);
        Assert.Equal(1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.False(page2.Visible);
        Assert.False(control.IsHandleCreated);
        Assert.False(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);

        // Set different.
        control.SelectTab(page1);
        Assert.Same(page1, control.SelectedTab);
        Assert.Equal(0, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.False(page2.Visible);
        Assert.False(control.IsHandleCreated);
        Assert.False(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabControl_SelectTab_InvokeTabPageWithPagesWithHandle_GetReturnsExpected()
    {
        using TabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int pageInvalidatedCallCount1 = 0;
        page1.Invalidated += (sender, e) => pageInvalidatedCallCount1++;
        int pageStyleChangedCallCount1 = 0;
        page1.StyleChanged += (sender, e) => pageStyleChangedCallCount1++;
        int pageCreatedCallCount1 = 0;
        page1.HandleCreated += (sender, e) => pageCreatedCallCount1++;
        int pageInvalidatedCallCount2 = 0;
        page2.Invalidated += (sender, e) => pageInvalidatedCallCount2++;
        int pageStyleChangedCallCount2 = 0;
        page2.StyleChanged += (sender, e) => pageStyleChangedCallCount2++;
        int pageCreatedCallCount2 = 0;
        page2.HandleCreated += (sender, e) => pageCreatedCallCount2++;
        Assert.True(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);

        // Set valid.
        control.SelectTab(page2);
        Assert.Same(page2, control.SelectedTab);
        Assert.Equal(1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.True(page2.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(page1.IsHandleCreated);
        Assert.Equal(0, pageInvalidatedCallCount1);
        Assert.Equal(0, pageStyleChangedCallCount1);
        Assert.Equal(0, pageCreatedCallCount1);
        Assert.True(page2.IsHandleCreated);
        Assert.Equal(0, pageInvalidatedCallCount2);
        Assert.Equal(0, pageStyleChangedCallCount2);
        Assert.Equal(1, pageCreatedCallCount2);

        // Set same.
        control.SelectTab(page2);
        Assert.Same(page2, control.SelectedTab);
        Assert.Equal(1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.True(page2.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(page1.IsHandleCreated);
        Assert.Equal(0, pageInvalidatedCallCount1);
        Assert.Equal(0, pageStyleChangedCallCount1);
        Assert.Equal(0, pageCreatedCallCount1);
        Assert.True(page2.IsHandleCreated);
        Assert.Equal(0, pageInvalidatedCallCount2);
        Assert.Equal(0, pageStyleChangedCallCount2);
        Assert.Equal(1, pageCreatedCallCount2);

        // Set different.
        control.SelectTab(page1);
        Assert.Same(page1, control.SelectedTab);
        Assert.Equal(0, control.SelectedIndex);
        Assert.True(page1.Visible);
        Assert.False(page2.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(page1.IsHandleCreated);
        Assert.Equal(1, pageInvalidatedCallCount1);
        Assert.Equal(0, pageStyleChangedCallCount1);
        Assert.Equal(0, pageCreatedCallCount1);
        Assert.True(page2.IsHandleCreated);
        Assert.Equal(0, pageInvalidatedCallCount2);
        Assert.Equal(0, pageStyleChangedCallCount2);
        Assert.Equal(1, pageCreatedCallCount2);
    }

    [WinFormsFact]
    public void TabControl_SelectTab_NullTabPage_ThrowsArgumentNullException()
    {
        using TabControl control = new();
        Assert.Throws<ArgumentNullException>("tabPage", () => control.SelectTab((TabPage)null));
    }

    [WinFormsFact]
    public void TabControl_SelectTab_NoSuchTabPageWithoutPages_ThrowsArgumentOutOfRangeException()
    {
        using TabControl control = new();
        using TabPage page = new();
        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.SelectTab(page));
    }

    [WinFormsFact]
    public void TabControl_SelectTab_NoSuchTabPageWithPages_ThrowsArgumentOutOfRangeException()
    {
        using TabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        using TabPage page3 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.SelectTab(page3));
    }

    [WinFormsFact]
    public void TabControl_SelectTab_InvokeStringWithPages_GetReturnsExpected()
    {
        using TabControl control = new();
        using TabPage page1 = new()
        {
            Name = "Name1"
        };
        using TabPage page2 = new()
        {
            Name = "Name2"
        };
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);

        // Set valid.
        control.SelectTab("Name2");
        Assert.Same(page2, control.SelectedTab);
        Assert.Equal(1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.False(page2.Visible);
        Assert.False(control.IsHandleCreated);
        Assert.False(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);

        // Set same.
        control.SelectTab("Name2");
        Assert.Same(page2, control.SelectedTab);
        Assert.Equal(1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.False(page2.Visible);
        Assert.False(control.IsHandleCreated);
        Assert.False(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);

        // Set different.
        control.SelectTab("Name1");
        Assert.Same(page1, control.SelectedTab);
        Assert.Equal(0, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.False(page2.Visible);
        Assert.False(control.IsHandleCreated);
        Assert.False(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabControl_SelectTab_InvokeStringWithPagesWithHandle_GetReturnsExpected()
    {
        using TabControl control = new();
        using TabPage page1 = new()
        {
            Name = "Name1"
        };
        using TabPage page2 = new()
        {
            Name = "Name2"
        };
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int pageInvalidatedCallCount1 = 0;
        page1.Invalidated += (sender, e) => pageInvalidatedCallCount1++;
        int pageStyleChangedCallCount1 = 0;
        page1.StyleChanged += (sender, e) => pageStyleChangedCallCount1++;
        int pageCreatedCallCount1 = 0;
        page1.HandleCreated += (sender, e) => pageCreatedCallCount1++;
        int pageInvalidatedCallCount2 = 0;
        page2.Invalidated += (sender, e) => pageInvalidatedCallCount2++;
        int pageStyleChangedCallCount2 = 0;
        page2.StyleChanged += (sender, e) => pageStyleChangedCallCount2++;
        int pageCreatedCallCount2 = 0;
        page2.HandleCreated += (sender, e) => pageCreatedCallCount2++;
        Assert.True(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);

        // Set valid.
        control.SelectTab("Name2");
        Assert.Same(page2, control.SelectedTab);
        Assert.Equal(1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.True(page2.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(page1.IsHandleCreated);
        Assert.Equal(0, pageInvalidatedCallCount1);
        Assert.Equal(0, pageStyleChangedCallCount1);
        Assert.Equal(0, pageCreatedCallCount1);
        Assert.True(page2.IsHandleCreated);
        Assert.Equal(0, pageInvalidatedCallCount2);
        Assert.Equal(0, pageStyleChangedCallCount2);
        Assert.Equal(1, pageCreatedCallCount2);

        // Set same.
        control.SelectTab("Name2");
        Assert.Same(page2, control.SelectedTab);
        Assert.Equal(1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.True(page2.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(page1.IsHandleCreated);
        Assert.Equal(0, pageInvalidatedCallCount1);
        Assert.Equal(0, pageStyleChangedCallCount1);
        Assert.Equal(0, pageCreatedCallCount1);
        Assert.True(page2.IsHandleCreated);
        Assert.Equal(0, pageInvalidatedCallCount2);
        Assert.Equal(0, pageStyleChangedCallCount2);
        Assert.Equal(1, pageCreatedCallCount2);

        // Set different.
        control.SelectTab("Name1");
        Assert.Same(page1, control.SelectedTab);
        Assert.Equal(0, control.SelectedIndex);
        Assert.True(page1.Visible);
        Assert.False(page2.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(page1.IsHandleCreated);
        Assert.Equal(1, pageInvalidatedCallCount1);
        Assert.Equal(0, pageStyleChangedCallCount1);
        Assert.Equal(0, pageCreatedCallCount1);
        Assert.True(page2.IsHandleCreated);
        Assert.Equal(0, pageInvalidatedCallCount2);
        Assert.Equal(0, pageStyleChangedCallCount2);
        Assert.Equal(1, pageCreatedCallCount2);
    }

    [WinFormsFact]
    public void TabControl_SelectTab_NullTabPageName_ThrowsArgumentNullException()
    {
        using TabControl control = new();
        Assert.Throws<ArgumentNullException>("tabPageName", () => control.SelectTab((string)null));
    }

    [WinFormsTheory]
    [InlineData("")]
    [InlineData("NoSuchName")]
    public void TabControl_SelectTab_NoSuchTabPageNameWithoutPages_ThrowsArgumentOutOfRangeException(string tabPageName)
    {
        using TabControl control = new();
        Assert.Throws<ArgumentNullException>("tabPage", () => control.SelectTab(tabPageName));
    }

    [WinFormsTheory]
    [InlineData("")]
    [InlineData("NoSuchName")]
    public void TabControl_SelectTab_NoSuchTabPageNameWithPages_ThrowsArgumentOutOfRangeException(string tabPageName)
    {
        using TabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        using TabPage page3 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        Assert.Throws<ArgumentNullException>("tabPage", () => control.SelectTab(tabPageName));
    }

    [WinFormsFact]
    public void TabControl_SelectTab_InvokeIntWithPages_GetReturnsExpected()
    {
        using TabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);

        // Set valid.
        control.SelectTab(1);
        Assert.Same(page2, control.SelectedTab);
        Assert.Equal(1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.False(page2.Visible);
        Assert.False(control.IsHandleCreated);
        Assert.False(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);

        // Set same.
        control.SelectTab(1);
        Assert.Same(page2, control.SelectedTab);
        Assert.Equal(1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.False(page2.Visible);
        Assert.False(control.IsHandleCreated);
        Assert.False(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);

        // Set different.
        control.SelectTab(0);
        Assert.Same(page1, control.SelectedTab);
        Assert.Equal(0, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.False(page2.Visible);
        Assert.False(control.IsHandleCreated);
        Assert.False(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabControl_SelectTab_InvokeIntWithPagesWithHandle_GetReturnsExpected()
    {
        using TabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int pageInvalidatedCallCount1 = 0;
        page1.Invalidated += (sender, e) => pageInvalidatedCallCount1++;
        int pageStyleChangedCallCount1 = 0;
        page1.StyleChanged += (sender, e) => pageStyleChangedCallCount1++;
        int pageCreatedCallCount1 = 0;
        page1.HandleCreated += (sender, e) => pageCreatedCallCount1++;
        int pageInvalidatedCallCount2 = 0;
        page2.Invalidated += (sender, e) => pageInvalidatedCallCount2++;
        int pageStyleChangedCallCount2 = 0;
        page2.StyleChanged += (sender, e) => pageStyleChangedCallCount2++;
        int pageCreatedCallCount2 = 0;
        page2.HandleCreated += (sender, e) => pageCreatedCallCount2++;
        Assert.True(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);

        // Set valid.
        control.SelectTab(1);
        Assert.Same(page2, control.SelectedTab);
        Assert.Equal(1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.True(page2.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(page1.IsHandleCreated);
        Assert.Equal(0, pageInvalidatedCallCount1);
        Assert.Equal(0, pageStyleChangedCallCount1);
        Assert.Equal(0, pageCreatedCallCount1);
        Assert.True(page2.IsHandleCreated);
        Assert.Equal(0, pageInvalidatedCallCount2);
        Assert.Equal(0, pageStyleChangedCallCount2);
        Assert.Equal(1, pageCreatedCallCount2);

        // Set same.
        control.SelectTab(1);
        Assert.Same(page2, control.SelectedTab);
        Assert.Equal(1, control.SelectedIndex);
        Assert.False(page1.Visible);
        Assert.True(page2.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(page1.IsHandleCreated);
        Assert.Equal(0, pageInvalidatedCallCount1);
        Assert.Equal(0, pageStyleChangedCallCount1);
        Assert.Equal(0, pageCreatedCallCount1);
        Assert.True(page2.IsHandleCreated);
        Assert.Equal(0, pageInvalidatedCallCount2);
        Assert.Equal(0, pageStyleChangedCallCount2);
        Assert.Equal(1, pageCreatedCallCount2);

        // Set different.
        control.SelectTab(0);
        Assert.Same(page1, control.SelectedTab);
        Assert.Equal(0, control.SelectedIndex);
        Assert.True(page1.Visible);
        Assert.False(page2.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(page1.IsHandleCreated);
        Assert.Equal(1, pageInvalidatedCallCount1);
        Assert.Equal(0, pageStyleChangedCallCount1);
        Assert.Equal(0, pageCreatedCallCount1);
        Assert.True(page2.IsHandleCreated);
        Assert.Equal(0, pageInvalidatedCallCount2);
        Assert.Equal(0, pageStyleChangedCallCount2);
        Assert.Equal(1, pageCreatedCallCount2);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void TabControl_SelectTab_InvalidIndexWithoutPages_ThrowsArgumentOutOfRangeException(int index)
    {
        using TabControl control = new();
        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.SelectTab(index));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(2)]
    [InlineData(3)]
    public void TabControl_SelectTab_InvalidIndexWithPages_ThrowsArgumentOutOfRangeException(int index)
    {
        using TabControl control = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.SelectTab(index));
    }

    [WinFormsFact]
    public void TabControl_ToString_InvokeEmpty_ReturnsExpected()
    {
        using TabControl control = new();
        Assert.Equal("System.Windows.Forms.TabControl, TabPages.Count: 0", control.ToString());
    }

    [WinFormsFact]
    public void TabControl_ToString_InvokeNotEmpty_ReturnsExpected()
    {
        using TabControl control = new();
        using TabPage page1 = new("text1");
        using TabPage page2 = new("text2");
        control.TabPages.Add(page1);
        control.TabPages.Add(page2);
        Assert.Equal("System.Windows.Forms.TabControl, TabPages.Count: 2, TabPages[0]: TabPage: {text1}", control.ToString());
    }

    [WinFormsFact]
    public void TabControl_Invokes_SetToolTip_IfExternalToolTipIsSet()
    {
        using TabControl control = new() { ShowToolTips = true };
        using ToolTip toolTip = new();
        control.CreateControl();

        dynamic tabControl = control.TestAccessor().Dynamic;
        string actual = tabControl._controlTipText;

        Assert.Empty(actual);
        Assert.NotEqual(IntPtr.Zero, toolTip.Handle); // A workaround to create the toolTip native window Handle

        string text = "Some test text";
        toolTip.SetToolTip(control, text); // Invokes TabControl's SetToolTip inside
        actual = tabControl._controlTipText;

        Assert.Equal(text, actual);
    }

    [WinFormsFact]
    public void TabControl_WmSelChange_SelectedTabIsNull_DoesNotThrowException()
    {
        using Form form = new();
        using TabControl control = new();
        using TabPage page1 = new("text1");
        control.TabPages.Add(page1);
        _ = control.AccessibilityObject;

        form.Controls.Add(control);
        form.Show();
        control.SelectedIndex = 0;

        Action act = () => control.TestAccessor().Dynamic.WmSelChange();
        act.Should().NotThrow();

        control.TabPages.Clear();

        var exception = Record.Exception(() =>
        {
            Application.DoEvents();
            Thread.Sleep(100);
        });

        exception.Should().BeNull();
    }

    private class SubTabPage : TabPage
    {
    }

    private class NullTextTabPage : TabPage
    {
        public override string Text
        {
            get => null;
            set { }
        }
    }

    public class SubTabControl : TabControl
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

        public new Control.ControlCollection CreateControlsInstance() => base.CreateControlsInstance();

        public new void CreateHandle() => base.CreateHandle();

        public new void Dispose(bool disposing) => base.Dispose(disposing);

        public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

        public new object[] GetItems() => base.GetItems();

        public new object[] GetItems(Type baseType) => base.GetItems(baseType);

        public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

        public new bool GetTopLevel() => base.GetTopLevel();

        public new string GetToolTipText(object item) => base.GetToolTipText(item);

        public new bool IsInputKey(Keys keyData) => base.IsInputKey(keyData);

        public new void OnDeselected(TabControlEventArgs e) => base.OnDeselected(e);

        public new void OnDeselecting(TabControlCancelEventArgs e) => base.OnDeselecting(e);

        public new void OnDrawItem(DrawItemEventArgs e) => base.OnDrawItem(e);

        public new void OnEnter(EventArgs e) => base.OnEnter(e);

        public new void OnHandleCreated(EventArgs e) => base.OnHandleCreated(e);

        public new void OnHandleDestroyed(EventArgs e) => base.OnHandleDestroyed(e);

        public new void OnLeave(EventArgs e) => base.OnLeave(e);

        public new void OnRightToLeftLayoutChanged(EventArgs e) => base.OnRightToLeftLayoutChanged(e);

        public new void OnSelected(TabControlEventArgs e) => base.OnSelected(e);

        public new void OnSelectedIndexChanged(EventArgs e) => base.OnSelectedIndexChanged(e);

        public new void OnSelecting(TabControlCancelEventArgs e) => base.OnSelecting(e);

        public new void RecreateHandle() => base.RecreateHandle();

        public new void RemoveAll() => base.RemoveAll();

        public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);
    }
}
