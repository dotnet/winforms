// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms.DataBinding.TestUtilities;
using System.Windows.Forms.TestUtilities;
using Moq;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Tests;

public class ButtonBaseTests : AbstractButtonBaseTests
{
    [WinFormsFact]
    public void ButtonBase_Ctor_Default()
    {
        using SubButtonBase control = new();
        Assert.Null(control.AccessibleDefaultActionDescription);
        Assert.Null(control.AccessibleDescription);
        Assert.Null(control.AccessibleName);
        Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
        Assert.False(control.AllowDrop);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        Assert.False(control.AutoEllipsis);
        Assert.False(control.AutoSize);
        Assert.Equal(Control.DefaultBackColor, control.BackColor);
        Assert.Null(control.BackgroundImage);
        Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
        Assert.Null(control.BindingContext);
        Assert.Equal(23, control.Bottom);
        Assert.Equal(new Rectangle(0, 0, 75, 23), control.Bounds);
        Assert.False(control.CanEnableIme);
        Assert.False(control.CanFocus);
        Assert.True(control.CanRaiseEvents);
        Assert.True(control.CanSelect);
        Assert.False(control.Capture);
        Assert.True(control.CausesValidation);
        Assert.Equal(new Size(75, 23), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, 75, 23), control.ClientRectangle);
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
        Assert.Equal(new Size(75, 23), control.DefaultSize);
        Assert.False(control.DesignMode);
        Assert.Equal(new Rectangle(0, 0, 75, 23), control.DisplayRectangle);
        Assert.Equal(DockStyle.None, control.Dock);
        Assert.True(control.DoubleBuffered);
        Assert.True(control.Enabled);
        Assert.NotNull(control.Events);
        Assert.Same(control.Events, control.Events);
        Assert.NotNull(control.FlatAppearance);
        Assert.Same(control.FlatAppearance, control.FlatAppearance);
        Assert.Equal(FlatStyle.Standard, control.FlatStyle);
        Assert.False(control.Focused);
        Assert.Equal(Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.Equal(Control.DefaultForeColor, control.ForeColor);
        Assert.False(control.HasChildren);
        Assert.Equal(23, control.Height);
        Assert.Null(control.Image);
        Assert.Equal(ContentAlignment.MiddleCenter, control.ImageAlign);
        Assert.Equal(-1, control.ImageIndex);
        Assert.Empty(control.ImageKey);
        Assert.Null(control.ImageList);
        Assert.Equal(ImeMode.Disable, control.ImeMode);
        Assert.Equal(ImeMode.Disable, control.ImeModeBase);
        Assert.False(control.IsAccessible);
        Assert.False(control.IsDefault);
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
        Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
        // This causes an assertion which we want to keep internally.
        using (new NoAssertContext())
        {
            Assert.Throws<NullReferenceException>(() => control.PreferredSize);
        }

        Assert.False(control.RecreatingHandle);
        Assert.Null(control.Region);
        Assert.True(control.ResizeRedraw);
        Assert.Equal(75, control.Right);
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.True(control.ShowFocusCues);
        Assert.True(control.ShowKeyboardCues);
        Assert.Null(control.Site);
        Assert.Equal(new Size(75, 23), control.Size);
        Assert.Equal(0, control.TabIndex);
        Assert.True(control.TabStop);
        Assert.Empty(control.Text);
        Assert.Equal(ContentAlignment.MiddleCenter, control.TextAlign);
        Assert.Equal(TextImageRelation.Overlay, control.TextImageRelation);
        Assert.Equal(0, control.Top);
        Assert.Null(control.TopLevelControl);
        Assert.True(control.UseCompatibleTextRendering);
        Assert.True(control.UseMnemonic);
        Assert.True(control.UseVisualStyleBackColor);
        Assert.False(control.UseWaitCursor);
        Assert.True(control.Visible);
        Assert.Equal(75, control.Width);

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ButtonBase_CreateParams_GetDefault_ReturnsExpected()
    {
        using SubButtonBase control = new();
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Null(createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0, createParams.ExStyle);
        Assert.Equal(23, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(0x56010000, createParams.Style);
        Assert.Equal(75, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(FlatStyle.Flat, true, 0x56010000)]
    [InlineData(FlatStyle.Flat, false, 0x56010000)]
    [InlineData(FlatStyle.Popup, true, 0x56010000)]
    [InlineData(FlatStyle.Popup, false, 0x56010000)]
    [InlineData(FlatStyle.Standard, true, 0x56010000)]
    [InlineData(FlatStyle.Standard, false, 0x56010000)]
    [InlineData(FlatStyle.System, true, 0x56012F01)]
    [InlineData(FlatStyle.System, false, 0x56012F00)]
    public void ButtonBase_CreateParams_GetIsDefault_ReturnsExpected(FlatStyle flatStyle, bool isDefault, int expectedStyle)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle,
            IsDefault = isDefault
        };

        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Null(createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0, createParams.ExStyle);
        Assert.Equal(23, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(expectedStyle, createParams.Style);
        Assert.Equal(75, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> CreateParams_GetIsDefault_TestData()
    {
        foreach (FlatStyle flatStyle in new FlatStyle[] { FlatStyle.Flat, FlatStyle.Popup, FlatStyle.Standard })
        {
            foreach (ContentAlignment textAlign in Enum.GetValues(typeof(ContentAlignment)))
            {
                yield return new object[] { flatStyle, RightToLeft.Yes, textAlign, 0x56010000, 0x7000 };
                yield return new object[] { flatStyle, RightToLeft.No, textAlign, 0x56010000, 0 };
                yield return new object[] { flatStyle, RightToLeft.Inherit, textAlign, 0x56010000, 0 };
            }
        }

        yield return new object[] { FlatStyle.System, RightToLeft.Yes, ContentAlignment.BottomLeft, 0x56012A00, 0x6000 };
        yield return new object[] { FlatStyle.System, RightToLeft.Yes, ContentAlignment.BottomCenter, 0x56012B00, 0x6000 };
        yield return new object[] { FlatStyle.System, RightToLeft.Yes, ContentAlignment.BottomRight, 0x56012900, 0x6000 };
        yield return new object[] { FlatStyle.System, RightToLeft.Yes, ContentAlignment.MiddleLeft, 0x56012E00, 0x6000 };
        yield return new object[] { FlatStyle.System, RightToLeft.Yes, ContentAlignment.MiddleCenter, 0x56012F00, 0x6000 };
        yield return new object[] { FlatStyle.System, RightToLeft.Yes, ContentAlignment.MiddleRight, 0x56012D00, 0x6000 };
        yield return new object[] { FlatStyle.System, RightToLeft.Yes, ContentAlignment.TopLeft, 0x56012600, 0x6000 };
        yield return new object[] { FlatStyle.System, RightToLeft.Yes, ContentAlignment.TopCenter, 0x56012700, 0x6000 };
        yield return new object[] { FlatStyle.System, RightToLeft.Yes, ContentAlignment.TopRight, 0x56012500, 0x6000 };

        yield return new object[] { FlatStyle.System, RightToLeft.No, ContentAlignment.BottomLeft, 0x56012900, 0 };
        yield return new object[] { FlatStyle.System, RightToLeft.No, ContentAlignment.BottomCenter, 0x56012B00, 0 };
        yield return new object[] { FlatStyle.System, RightToLeft.No, ContentAlignment.BottomRight, 0x56012A00, 0 };
        yield return new object[] { FlatStyle.System, RightToLeft.No, ContentAlignment.MiddleLeft, 0x56012D00, 0 };
        yield return new object[] { FlatStyle.System, RightToLeft.No, ContentAlignment.MiddleCenter, 0x56012F00, 0 };
        yield return new object[] { FlatStyle.System, RightToLeft.No, ContentAlignment.MiddleRight, 0x56012E00, 0 };
        yield return new object[] { FlatStyle.System, RightToLeft.No, ContentAlignment.TopLeft, 0x56012500, 0 };
        yield return new object[] { FlatStyle.System, RightToLeft.No, ContentAlignment.TopCenter, 0x56012700, 0 };
        yield return new object[] { FlatStyle.System, RightToLeft.No, ContentAlignment.TopRight, 0x56012600, 0 };

        yield return new object[] { FlatStyle.System, RightToLeft.Inherit, ContentAlignment.BottomLeft, 0x56012900, 0 };
        yield return new object[] { FlatStyle.System, RightToLeft.Inherit, ContentAlignment.BottomCenter, 0x56012B00, 0 };
        yield return new object[] { FlatStyle.System, RightToLeft.Inherit, ContentAlignment.BottomRight, 0x56012A00, 0 };
        yield return new object[] { FlatStyle.System, RightToLeft.Inherit, ContentAlignment.MiddleLeft, 0x56012D00, 0 };
        yield return new object[] { FlatStyle.System, RightToLeft.Inherit, ContentAlignment.MiddleCenter, 0x56012F00, 0 };
        yield return new object[] { FlatStyle.System, RightToLeft.Inherit, ContentAlignment.MiddleRight, 0x56012E00, 0 };
        yield return new object[] { FlatStyle.System, RightToLeft.Inherit, ContentAlignment.TopLeft, 0x56012500, 0 };
        yield return new object[] { FlatStyle.System, RightToLeft.Inherit, ContentAlignment.TopCenter, 0x56012700, 0 };
        yield return new object[] { FlatStyle.System, RightToLeft.Inherit, ContentAlignment.TopRight, 0x56012600, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(CreateParams_GetIsDefault_TestData))]
    public void ButtonBase_CreateParams_GetTextAlign_ReturnsExpected(FlatStyle flatStyle, RightToLeft rightToLeft, ContentAlignment textAlign, int expectedStyle, int expectedExStyle)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle,
            RightToLeft = rightToLeft,
            TextAlign = textAlign
        };

        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Null(createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(expectedExStyle, createParams.ExStyle);
        Assert.Equal(23, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(expectedStyle, createParams.Style);
        Assert.Equal(75, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ButtonBase_AutoEllipsis_Set_GetReturnsExpected(bool value)
    {
        using SubButtonBase control = new()
        {
            AutoEllipsis = value
        };
        Assert.Equal(value, control.AutoEllipsis);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AutoEllipsis = value;
        Assert.Equal(value, control.AutoEllipsis);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.AutoEllipsis = !value;
        Assert.Equal(!value, control.AutoEllipsis);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void ButtonBase_AutoEllipsis_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount)
    {
        using SubButtonBase control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.AutoEllipsis = value;
        Assert.Equal(value, control.AutoEllipsis);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.AutoEllipsis = value;
        Assert.Equal(value, control.AutoEllipsis);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.AutoEllipsis = !value;
        Assert.Equal(!value, control.AutoEllipsis);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void ButtonBase_AutoSize_Set_GetReturnsExpected(bool value)
    {
        using SubButtonBase control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.AutoSize = value;
        Assert.Equal(value, control.AutoSize);
        Assert.False(control.AutoEllipsis);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AutoSize = value;
        Assert.Equal(value, control.AutoSize);
        Assert.False(control.AutoEllipsis);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.AutoSize = !value;
        Assert.Equal(!value, control.AutoSize);
        Assert.False(control.AutoEllipsis);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ButtonBase_AutoSize_SetAutoEllipsis_GetReturnsExpected(bool value)
    {
        using SubButtonBase control = new()
        {
            AutoEllipsis = true
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.AutoSize = value;
        Assert.Equal(value, control.AutoSize);
        Assert.Equal(!value, control.AutoEllipsis);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AutoSize = value;
        Assert.Equal(value, control.AutoSize);
        Assert.Equal(!value, control.AutoEllipsis);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.AutoSize = !value;
        Assert.Equal(!value, control.AutoSize);
        Assert.False(control.AutoEllipsis);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ButtonBase_AutoSize_SetWithHandle_GetReturnsExpected(bool value)
    {
        using SubButtonBase control = new();
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
        Assert.False(control.AutoEllipsis);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.AutoSize = value;
        Assert.Equal(value, control.AutoSize);
        Assert.False(control.AutoEllipsis);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.AutoSize = !value;
        Assert.Equal(!value, control.AutoSize);
        Assert.False(control.AutoEllipsis);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void ButtonBase_AutoSize_SetAutoEllipsisWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount)
    {
        using SubButtonBase control = new()
        {
            AutoEllipsis = true
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
        Assert.Equal(!value, control.AutoEllipsis);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.AutoSize = value;
        Assert.Equal(value, control.AutoSize);
        Assert.Equal(!value, control.AutoEllipsis);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.AutoSize = !value;
        Assert.Equal(!value, control.AutoSize);
        Assert.False(control.AutoEllipsis);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ButtonBase_AutoSize_SetWithHandler_CallsAutoSizeChanged()
    {
        using SubButtonBase control = new()
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
    public void ButtonBase_BackColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using SubButtonBase control = new()
        {
            BackColor = value
        };
        Assert.Equal(expected, control.BackColor);
        Assert.False(control.UseVisualStyleBackColor);

        // Set same.
        control.BackColor = value;
        Assert.Equal(expected, control.BackColor);
        Assert.False(control.UseVisualStyleBackColor);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetBackColorTheoryData))]
    public void ButtonBase_BackColor_SetWithUseVisualStyleBackColor_GetReturnsExpected(Color value, Color expected)
    {
        using SubButtonBase control = new()
        {
            UseVisualStyleBackColor = true,
            BackColor = value
        };
        Assert.Equal(expected, control.BackColor);
        Assert.False(control.UseVisualStyleBackColor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BackColor = value;
        Assert.Equal(expected, control.BackColor);
        Assert.False(control.UseVisualStyleBackColor);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetBackColorTheoryData))]
    public void ButtonBase_BackColor_SetDesignMode_GetReturnsExpected(Color value, Color expected)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(IDictionaryService)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(IExtenderListService)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(IComponentChangeService)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
            .Returns(null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Name)
            .Returns((string)null);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using SubButtonBase control = new()
        {
            Site = mockSite.Object,
            UseVisualStyleBackColor = false,
            BackColor = value
        };
        Assert.Equal(expected, control.BackColor);
        Assert.False(control.UseVisualStyleBackColor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BackColor = value;
        Assert.Equal(expected, control.BackColor);
        Assert.False(control.UseVisualStyleBackColor);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetBackColorTheoryData))]
    public void ButtonBase_BackColor_SetDesignModeWithUseVisualStyleBackColor_GetReturnsExpected(Color value, Color expected)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(IDictionaryService)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(IExtenderListService)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(IComponentChangeService)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
            .Returns(null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Name)
            .Returns((string)null);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using SubButtonBase control = new()
        {
            Site = mockSite.Object,
            UseVisualStyleBackColor = true,
            BackColor = value
        };
        Assert.Equal(expected, control.BackColor);
        Assert.Equal(value.IsEmpty, control.UseVisualStyleBackColor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BackColor = value;
        Assert.Equal(expected, control.BackColor);
        Assert.Equal(value.IsEmpty, control.UseVisualStyleBackColor);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetBackColorTheoryData))]
    public void ButtonBase_BackColor_SetDesignModeWithInvalidDescriptor_GetReturnsExpected(Color value, Color expected)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(IDictionaryService)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(IExtenderListService)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(IComponentChangeService)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
            .Returns(null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Name)
            .Returns((string)null);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using SubButtonBase control = new()
        {
            Site = mockSite.Object,
            UseVisualStyleBackColor = true
        };
        Mock<ICustomTypeDescriptor> mockCustomTypeDescriptor = new(MockBehavior.Strict);
        mockCustomTypeDescriptor
            .Setup(d => d.GetProperties())
            .Returns(PropertyDescriptorCollection.Empty);
        Mock<TypeDescriptionProvider> mockProvider = new(MockBehavior.Strict);
        mockProvider
            .Setup(p => p.GetCache(control))
            .CallBase();
        mockProvider
            .Setup(p => p.GetExtendedTypeDescriptor(control))
            .CallBase();
        mockProvider
            .Setup(p => p.GetTypeDescriptor(typeof(SubButtonBase), control))
            .Returns(mockCustomTypeDescriptor.Object);
        TypeDescriptor.AddProvider(mockProvider.Object, control);

        control.BackColor = value;
        Assert.Equal(expected, control.BackColor);
        Assert.True(control.UseVisualStyleBackColor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BackColor = value;
        Assert.Equal(expected, control.BackColor);
        Assert.True(control.UseVisualStyleBackColor);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> BackColor_SetWithHandle_TestData()
    {
        yield return new object[] { Color.Red, Color.Red, 2 };
        yield return new object[] { Color.Empty, Control.DefaultBackColor, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(BackColor_SetWithHandle_TestData))]
    public void ButtonBase_BackColor_SetWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
    {
        using SubButtonBase control = new();
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
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ButtonBase_BackColor_SetWithHandler_CallsBackColorChanged()
    {
        using SubButtonBase control = new();
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

    [WinFormsFact]
    public void ButtonBase_BasicCommandBinding()
    {
        const string CommandParameter = nameof(CommandParameter);

        using SubButtonBase button = new();

        // TestCommandExecutionAbility is controlling the execution context.
        CommandViewModel viewModel = new() { TestCommandExecutionAbility = true };

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(button, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };

        button.CommandChanged += handler;
        button.Command = viewModel.TestCommand;
        Assert.Equal(1, callCount);

        button.CommandParameterChanged += handler;
        button.CommandParameter = CommandParameter;
        Assert.Equal(2, callCount);

        Assert.Same(viewModel.TestCommand, button.Command);
        Assert.True(button.Enabled);

        // OnClick is invoking the command in the ViewModel.
        // The CommandParameter should make its way into the viewmodel's CommandExecuteResult property.
        button.OnClick(EventArgs.Empty);
        Assert.Same(CommandParameter, viewModel.CommandExecuteResult);

        // We're changing the execution context.
        // The ViewModel calls RaiseCanExecuteChanged, which the Button should handle.
        viewModel.TestCommandExecutionAbility = false;
        Assert.False(button.Enabled);

        // Remove handler.
        button.CommandChanged -= handler;
        button.Command = null;
        Assert.Equal(2, callCount);

        button.CommandParameterChanged -= handler;
        button.CommandParameter = null;
        Assert.Equal(2, callCount);
    }

    public static IEnumerable<object[]> Enabled_Set_TestData()
    {
        foreach (bool visible in new bool[] { true, false })
        {
            foreach (Image image in new Image[] { null, new Bitmap(10, 10) })
            {
                yield return new object[] { visible, image, true };
                yield return new object[] { visible, image, false };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Enabled_Set_TestData))]
    public void ButtonBase_Enabled_Set_GetReturnsExpected(bool visible, Image image, bool value)
    {
        using SubButtonBase control = new()
        {
            Visible = visible,
            Image = image,
            Enabled = value
        };
        Assert.Equal(value, control.Enabled);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Enabled = value;
        Assert.Equal(value, control.Enabled);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.Enabled = !value;
        Assert.Equal(!value, control.Enabled);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Enabled_Set_TestData))]
    public void ButtonBase_Enabled_SetDesignMode_GetReturnsExpected(bool visible, Image image, bool value)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Name)
            .Returns((string)null);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using SubButtonBase control = new()
        {
            Visible = visible,
            Image = image,
            Site = mockSite.Object,
            Enabled = value
        };
        Assert.Equal(value, control.Enabled);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Enabled = value;
        Assert.Equal(value, control.Enabled);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.Enabled = !value;
        Assert.Equal(!value, control.Enabled);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Enabled_SetWithHandle_TestData()
    {
        foreach (bool visible in new bool[] { true, false })
        {
            foreach (Image image in new Image[] { null, new Bitmap(10, 10) })
            {
                yield return new object[] { visible, image, true, 0, 2 };
                yield return new object[] { visible, image, false, 2, 3 };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Enabled_SetWithHandle_TestData))]
    public void ButtonBase_Enabled_SetWithHandle_GetReturnsExpected(bool visible, Image image, bool value, int expectedInvalidatedCallCount1, int expectedInvalidatedCallCount2)
    {
        using SubButtonBase control = new()
        {
            Visible = visible,
            Image = image
        };
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
        Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Enabled = value;
        Assert.Equal(value, control.Enabled);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.Enabled = !value;
        Assert.Equal(!value, control.Enabled);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(Enabled_SetWithHandle_TestData))]
    public void ButtonBase_Enabled_SetDesignModeWithHandle_GetReturnsExpected(bool visible, Image image, bool value, int expectedInvalidatedCallCount1, int expectedInvalidatedCallCount2)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Name)
            .Returns((string)null);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using SubButtonBase control = new()
        {
            Visible = visible,
            Image = image,
            Site = mockSite.Object
        };
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
        Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Enabled = value;
        Assert.Equal(value, control.Enabled);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.Enabled = !value;
        Assert.Equal(!value, control.Enabled);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ButtonBase_Enabled_SetWithHandler_CallsEnabledChanged()
    {
        using SubButtonBase control = new();
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
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void ButtonBase_FlatAppearance_Get_ReturnsExpected()
    {
        using SubButtonBase control = new();
        FlatButtonAppearance appearance = control.FlatAppearance;
        Assert.Equal(Color.Empty, appearance.BorderColor);
        Assert.Equal(1, appearance.BorderSize);
        Assert.Equal(Color.Empty, appearance.CheckedBackColor);
        Assert.Equal(Color.Empty, appearance.MouseDownBackColor);
        Assert.Equal(Color.Empty, appearance.MouseOverBackColor);
        Assert.Same(appearance, control.FlatAppearance);
    }

    public static IEnumerable<object[]> FlatStyle_Set_TestData()
    {
        foreach (bool autoSize in new bool[] { true, false })
        {
            foreach (FlatStyle value in Enum.GetValues(typeof(FlatStyle)))
            {
                yield return new object[] { autoSize, value };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(FlatStyle_Set_TestData))]
    public void ButtonBase_FlatStyle_Set_GetReturnsExpected(bool autoSize, FlatStyle value)
    {
        using SubButtonBase control = new()
        {
            AutoSize = autoSize
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.FlatStyle = value;
        Assert.Equal(value, control.FlatStyle);
        Assert.Equal(value != FlatStyle.System, control.GetStyle(ControlStyles.UserMouse));
        Assert.Equal(value != FlatStyle.System, control.GetStyle(ControlStyles.UserPaint));
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.FlatStyle = value;
        Assert.Equal(value, control.FlatStyle);
        Assert.Equal(value != FlatStyle.System, control.GetStyle(ControlStyles.UserMouse));
        Assert.Equal(value != FlatStyle.System, control.GetStyle(ControlStyles.UserPaint));
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> FlatStyle_SetWithCustomOldValue_TestData()
    {
        foreach (bool autoSize in new bool[] { true, false })
        {
            foreach (FlatStyle oldValue in Enum.GetValues(typeof(FlatStyle)))
            {
                foreach (FlatStyle value in Enum.GetValues(typeof(FlatStyle)))
                {
                    yield return new object[] { autoSize, oldValue, value };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(FlatStyle_SetWithCustomOldValue_TestData))]
    public void ButtonBase_FlatStyle_SetWithCustomOldValue_GetReturnsExpected(bool autoSize, FlatStyle oldValue, FlatStyle value)
    {
        using SubButtonBase control = new()
        {
            AutoSize = autoSize,
            FlatStyle = oldValue
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.FlatStyle = value;
        Assert.Equal(value, control.FlatStyle);
        Assert.Equal(value != FlatStyle.System, control.GetStyle(ControlStyles.UserMouse));
        Assert.Equal(value != FlatStyle.System, control.GetStyle(ControlStyles.UserPaint));
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.FlatStyle = value;
        Assert.Equal(value, control.FlatStyle);
        Assert.Equal(value != FlatStyle.System, control.GetStyle(ControlStyles.UserMouse));
        Assert.Equal(value != FlatStyle.System, control.GetStyle(ControlStyles.UserPaint));
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> FlatStyle_SetWithParent_TestData()
    {
        yield return new object[] { true, FlatStyle.Flat, 1 };
        yield return new object[] { true, FlatStyle.Popup, 1 };
        yield return new object[] { true, FlatStyle.Standard, 0 };
        yield return new object[] { true, FlatStyle.System, 1 };

        yield return new object[] { false, FlatStyle.Flat, 0 };
        yield return new object[] { false, FlatStyle.Popup, 0 };
        yield return new object[] { false, FlatStyle.Standard, 0 };
        yield return new object[] { false, FlatStyle.System, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(FlatStyle_SetWithParent_TestData))]
    public void ButtonBase_FlatStyle_SetWithParent_GetReturnsExpected(bool autoSize, FlatStyle value, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using SubButton control = new()
        {
            Parent = parent,
            AutoSize = autoSize
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("FlatStyle", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.FlatStyle = value;
            Assert.Equal(value, control.FlatStyle);
            Assert.Equal(value != FlatStyle.System, control.GetStyle(ControlStyles.UserMouse));
            Assert.Equal(value != FlatStyle.System, control.GetStyle(ControlStyles.UserPaint));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            control.FlatStyle = value;
            Assert.Equal(value, control.FlatStyle);
            Assert.Equal(value != FlatStyle.System, control.GetStyle(ControlStyles.UserMouse));
            Assert.Equal(value != FlatStyle.System, control.GetStyle(ControlStyles.UserPaint));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> FlatStyle_SetWithHandle_TestData()
    {
        foreach (bool autoSize in new bool[] { true, false })
        {
            yield return new object[] { autoSize, FlatStyle.Flat, 1, 0 };
            yield return new object[] { autoSize, FlatStyle.Popup, 1, 0 };
            yield return new object[] { autoSize, FlatStyle.Standard, 0, 0 };
            yield return new object[] { autoSize, FlatStyle.System, 1, 1 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(FlatStyle_SetWithHandle_TestData))]
    public void ButtonBase_FlatStyle_SetWithHandle_GetReturnsExpected(bool autoSize, FlatStyle value, int expectedInvalidatedCallCount, int expectedCreatedCallCount)
    {
        using SubButtonBase control = new()
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
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.FlatStyle = value;
        Assert.Equal(value, control.FlatStyle);
        Assert.Equal(value != FlatStyle.System, control.GetStyle(ControlStyles.UserMouse));
        Assert.Equal(value != FlatStyle.System, control.GetStyle(ControlStyles.UserPaint));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        control.FlatStyle = value;
        Assert.Equal(value, control.FlatStyle);
        Assert.Equal(value != FlatStyle.System, control.GetStyle(ControlStyles.UserMouse));
        Assert.Equal(value != FlatStyle.System, control.GetStyle(ControlStyles.UserPaint));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);
    }

    public static IEnumerable<object[]> FlatStyle_SetWithCustomOldValueWithHandle_TestData()
    {
        yield return new object[] { FlatStyle.Flat, FlatStyle.Flat, 0, 0 };
        yield return new object[] { FlatStyle.Flat, FlatStyle.Popup, 1, 0 };
        yield return new object[] { FlatStyle.Flat, FlatStyle.Standard, 1, 0 };
        yield return new object[] { FlatStyle.Flat, FlatStyle.System, 1, 1 };

        yield return new object[] { FlatStyle.Popup, FlatStyle.Flat, 1, 0 };
        yield return new object[] { FlatStyle.Popup, FlatStyle.Popup, 0, 0 };
        yield return new object[] { FlatStyle.Popup, FlatStyle.Standard, 1, 0 };
        yield return new object[] { FlatStyle.Popup, FlatStyle.System, 1, 1 };

        yield return new object[] { FlatStyle.Standard, FlatStyle.Flat, 1, 0 };
        yield return new object[] { FlatStyle.Standard, FlatStyle.Popup, 1, 0 };
        yield return new object[] { FlatStyle.Standard, FlatStyle.Standard, 0, 0 };
        yield return new object[] { FlatStyle.Standard, FlatStyle.System, 1, 1 };

        yield return new object[] { FlatStyle.System, FlatStyle.Flat, 1, 1 };
        yield return new object[] { FlatStyle.System, FlatStyle.Popup, 1, 1 };
        yield return new object[] { FlatStyle.System, FlatStyle.Standard, 1, 1 };
        yield return new object[] { FlatStyle.System, FlatStyle.System, 0, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(FlatStyle_SetWithCustomOldValueWithHandle_TestData))]
    public void ButtonBase_FlatStyle_SetWithCustomOldValueWithHandle_GetReturnsExpected(FlatStyle oldValue, FlatStyle value, int expectedInvalidatedCallCount, int expectedCreatedCallCount)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = oldValue
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.FlatStyle = value;
        Assert.Equal(value, control.FlatStyle);
        Assert.Equal(value != FlatStyle.System, control.GetStyle(ControlStyles.UserMouse));
        Assert.Equal(value != FlatStyle.System, control.GetStyle(ControlStyles.UserPaint));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        control.FlatStyle = value;
        Assert.Equal(value, control.FlatStyle);
        Assert.Equal(value != FlatStyle.System, control.GetStyle(ControlStyles.UserMouse));
        Assert.Equal(value != FlatStyle.System, control.GetStyle(ControlStyles.UserPaint));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);
    }

    public static IEnumerable<object[]> FlatStyle_SetWithParentWithHandle_TestData()
    {
        yield return new object[] { true, FlatStyle.Flat, 1, 1, 0 };
        yield return new object[] { true, FlatStyle.Popup, 1, 1, 0 };
        yield return new object[] { true, FlatStyle.Standard, 0, 0, 0 };
        yield return new object[] { true, FlatStyle.System, 1, 1, 1 };

        yield return new object[] { false, FlatStyle.Flat, 0, 1, 0 };
        yield return new object[] { false, FlatStyle.Popup, 0, 1, 0 };
        yield return new object[] { false, FlatStyle.Standard, 0, 0, 0 };
        yield return new object[] { false, FlatStyle.System, 0, 1, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(FlatStyle_SetWithParentWithHandle_TestData))]
    public void ButtonBase_FlatStyle_SetWithParentWithHandle_GetReturnsExpected(bool autoSize, FlatStyle value, int expectedParentLayoutCallCount, int expectedInvalidatedCallCount, int expectedCreatedCallCount)
    {
        using Control parent = new();
        using SubButton control = new()
        {
            Parent = parent,
            AutoSize = autoSize
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("FlatStyle", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        try
        {
            control.FlatStyle = value;
            Assert.Equal(value, control.FlatStyle);
            Assert.Equal(value != FlatStyle.System, control.GetStyle(ControlStyles.UserMouse));
            Assert.Equal(value != FlatStyle.System, control.GetStyle(ControlStyles.UserPaint));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);
            Assert.True(parent.IsHandleCreated);

            // Set same.
            control.FlatStyle = value;
            Assert.Equal(value, control.FlatStyle);
            Assert.Equal(value != FlatStyle.System, control.GetStyle(ControlStyles.UserMouse));
            Assert.Equal(value != FlatStyle.System, control.GetStyle(ControlStyles.UserPaint));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);
            Assert.True(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InvalidEnumData<FlatStyle>]
    public void ButtonBase_FlatStyle_SetInvalidValue_ThrowsInvalidEnumArgumentException(FlatStyle value)
    {
        using SubButtonBase control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.FlatStyle = value);
    }

    public static IEnumerable<object[]> Image_Set_TestData()
    {
        foreach (bool autoSize in new bool[] { true, false })
        {
            foreach (bool enabled in new bool[] { true, false })
            {
                foreach (bool visible in new bool[] { true, false })
                {
                    yield return new object[] { autoSize, enabled, visible, null };
                    yield return new object[] { autoSize, enabled, visible, new Bitmap(10, 10) };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Image_Set_TestData))]
    public void ButtonBase_Image_Set_GetReturnsExpected(bool autoSize, bool enabled, bool visible, Image value)
    {
        using SubButtonBase control = new()
        {
            AutoSize = autoSize,
            Enabled = enabled,
            Visible = visible
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.Image = value;
        Assert.Same(value, control.Image);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Image = value;
        Assert.Same(value, control.Image);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ButtonBase_Image_SetWithImageIndex_GetReturnsExpected()
    {
        using SubButtonBase control = new()
        {
            ImageIndex = 1
        };

        // Set same.
        control.Image = null;
        Assert.Empty(control.ImageKey);
        Assert.Equal(1, control.ImageIndex);
        Assert.Null(control.Image);
        Assert.False(control.IsHandleCreated);

        // Set different.
        using Bitmap value = new(10, 10);
        control.Image = value;
        Assert.Empty(control.ImageKey);
        Assert.Equal(-1, control.ImageIndex);
        Assert.Same(value, control.Image);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ButtonBase_Image_SetWithImageKey_GetReturnsExpected()
    {
        using SubButtonBase control = new()
        {
            ImageKey = "ImageKey"
        };

        // Set same.
        control.Image = null;
        Assert.Equal("ImageKey", control.ImageKey);
        Assert.Equal(ImageList.Indexer.DefaultIndex, control.ImageIndex);
        Assert.Null(control.Image);
        Assert.False(control.IsHandleCreated);

        // Set different.
        using Bitmap value = new(10, 10);
        control.Image = value;
        Assert.Equal(ImageList.Indexer.DefaultKey, control.ImageKey);
        Assert.Equal(ImageList.Indexer.DefaultIndex, control.ImageIndex);
        Assert.Same(value, control.Image);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ButtonBase_Image_SetWithImageList_GetReturnsExpected()
    {
        using ImageList imageList = new();
        using SubButtonBase control = new()
        {
            ImageList = imageList
        };

        // Set same.
        control.Image = null;
        Assert.Empty(control.ImageKey);
        Assert.Equal(-1, control.ImageIndex);
        Assert.Same(imageList, control.ImageList);
        Assert.Null(control.Image);
        Assert.False(control.IsHandleCreated);

        // Set different.
        using Bitmap value = new(10, 10);
        control.Image = value;
        Assert.Empty(control.ImageKey);
        Assert.Equal(-1, control.ImageIndex);
        Assert.Null(control.ImageList);
        Assert.Same(value, control.Image);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Image_Set_TestData))]
    public void ButtonBase_Image_SetDesignMode_GetReturnsExpected(bool autoSize, bool enabled, bool visible, Image value)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Name)
            .Returns((string)null);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using SubButtonBase control = new()
        {
            AutoSize = autoSize,
            Enabled = enabled,
            Visible = visible,
            Site = mockSite.Object
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.Image = value;
        Assert.Same(value, control.Image);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Image = value;
        Assert.Same(value, control.Image);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Image_SetWithParent_TestData()
    {
        foreach (bool enabled in new bool[] { true, false })
        {
            foreach (bool visible in new bool[] { true, false })
            {
                yield return new object[] { true, enabled, visible, null, 0 };
                yield return new object[] { true, enabled, visible, new Bitmap(10, 10), 1 };
                yield return new object[] { false, enabled, visible, null, 0 };
                yield return new object[] { false, enabled, visible, new Bitmap(10, 10), 0 };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Image_SetWithParent_TestData))]
    public void ButtonBase_Image_SetWithParent_GetReturnsExpected(bool autoSize, bool enabled, bool visible, Image value, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using Button control = new()
        {
            AutoSize = autoSize,
            Enabled = enabled,
            Visible = visible,
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Image", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.Image = value;
            Assert.Same(value, control.Image);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            control.Image = value;
            Assert.Same(value, control.Image);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> Image_SetWithHandle_TestData()
    {
        foreach (bool autoSize in new bool[] { true, false })
        {
            foreach (bool enabled in new bool[] { true, false })
            {
                foreach (bool visible in new bool[] { true, false })
                {
                    yield return new object[] { autoSize, enabled, visible, null, 0 };
                    yield return new object[] { autoSize, enabled, visible, new Bitmap(10, 10), 2 };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Image_SetWithHandle_TestData))]
    public void ButtonBase_Image_SetWithHandle_GetReturnsExpected(bool autoSize, bool enabled, bool visible, Image value, int expectedInvalidatedCallCount)
    {
        using SubButtonBase control = new()
        {
            AutoSize = autoSize,
            Enabled = enabled,
            Visible = visible
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

        control.Image = value;
        Assert.Same(value, control.Image);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Image = value;
        Assert.Same(value, control.Image);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(Image_SetWithHandle_TestData))]
    public void ButtonBase_Image_SetDesignModeWithHandle_GetReturnsExpected(bool autoSize, bool enabled, bool visible, Image value, int expectedInvalidatedCallCount)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Name)
            .Returns((string)null);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using SubButtonBase control = new()
        {
            AutoSize = autoSize,
            Enabled = enabled,
            Visible = visible,
            Site = mockSite.Object
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

        control.Image = value;
        Assert.Same(value, control.Image);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Image = value;
        Assert.Same(value, control.Image);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> Image_SetWithParentWithHandle_TestData()
    {
        foreach (bool enabled in new bool[] { true, false })
        {
            foreach (bool visible in new bool[] { true, false })
            {
                yield return new object[] { true, enabled, visible, null, 0, 0 };
                yield return new object[] { true, enabled, visible, new Bitmap(10, 10), 1, 2 };
                yield return new object[] { false, enabled, visible, null, 0, 0 };
                yield return new object[] { false, enabled, visible, new Bitmap(10, 10), 0, 2 };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Image_SetWithParentWithHandle_TestData))]
    public void ButtonBase_Image_SetWithParentWithHandle_GetReturnsExpected(bool autoSize, bool enabled, bool visible, Image value, int expectedParentLayoutCallCount, int expectedInvalidatedCallCount)
    {
        using Control parent = new();
        using Button control = new()
        {
            AutoSize = autoSize,
            Enabled = enabled,
            Visible = visible,
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Image", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.Image = value;
            Assert.Same(value, control.Image);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);

            // Set same.
            control.Image = value;
            Assert.Same(value, control.Image);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void ButtonBase_Image_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ButtonBase))[nameof(ButtonBase.Image)];
        using SubButtonBase control = new();
        Assert.False(property.CanResetValue(control));

        using Bitmap image = new(10, 10);
        control.Image = image;
        Assert.Same(image, control.Image);
        Assert.True(property.CanResetValue(control));
        Assert.False(control.IsHandleCreated);

        property.ResetValue(control);
        Assert.Null(control.Image);
        Assert.False(property.CanResetValue(control));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ButtonBase_Image_ResetValueWithHandle_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ButtonBase))[nameof(ButtonBase.Image)];
        using SubButtonBase control = new();
        Assert.False(property.CanResetValue(control));

        using Bitmap image = new(10, 10);
        control.Image = image;
        Assert.Same(image, control.Image);
        Assert.True(property.CanResetValue(control));

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        property.ResetValue(control);
        Assert.Null(control.Image);
        Assert.False(property.CanResetValue(control));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ButtonBase_Image_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ButtonBase))[nameof(ButtonBase.Image)];
        using SubButtonBase control = new();
        Assert.False(property.ShouldSerializeValue(control));

        using Bitmap image = new(10, 10);
        control.Image = image;
        Assert.Same(image, control.Image);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.Null(control.Image);
        Assert.False(property.ShouldSerializeValue(control));
    }

    public static IEnumerable<object[]> ImageAlign_Set_TestData()
    {
        foreach (bool autoSize in new bool[] { true, false })
        {
            foreach (ContentAlignment value in Enum.GetValues(typeof(ContentAlignment)))
            {
                yield return new object[] { autoSize, value };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ImageAlign_Set_TestData))]
    public void ButtonBase_ImageAlign_Set_GetReturnsExpected(bool autoSize, ContentAlignment value)
    {
        using SubButtonBase control = new()
        {
            AutoSize = autoSize
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.ImageAlign = value;
        Assert.Equal(value, control.ImageAlign);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ImageAlign = value;
        Assert.Equal(value, control.ImageAlign);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> ImageAlign_SetWithParent_TestData()
    {
        foreach (ContentAlignment value in Enum.GetValues(typeof(ContentAlignment)))
        {
            int expectedCallCount = value == ContentAlignment.MiddleCenter ? 0 : 1;
            yield return new object[] { true, value, expectedCallCount };
            yield return new object[] { false, value, 0 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ImageAlign_SetWithParent_TestData))]
    public void ButtonBase_ImageAlign_SetWithParent_GetReturnsExpected(bool autoSize, ContentAlignment value, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using Button control = new()
        {
            Parent = parent,
            AutoSize = autoSize
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("ImageAlign", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.ImageAlign = value;
            Assert.Equal(value, control.ImageAlign);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            control.ImageAlign = value;
            Assert.Equal(value, control.ImageAlign);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> ImageAlign_SetWithHandle_TestData()
    {
        foreach (bool autoSize in new bool[] { true, false })
        {
            foreach (ContentAlignment value in Enum.GetValues(typeof(ContentAlignment)))
            {
                int expectedCallCount = value == ContentAlignment.MiddleCenter ? 0 : 1;
                yield return new object[] { autoSize, value, expectedCallCount };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ImageAlign_SetWithHandle_TestData))]
    public void ButtonBase_ImageAlign_SetWithHandle_GetReturnsExpected(bool autoSize, ContentAlignment value, int expectedInvalidatedCallCount)
    {
        using SubButtonBase control = new()
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
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.ImageAlign = value;
        Assert.Equal(value, control.ImageAlign);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.ImageAlign = value;
        Assert.Equal(value, control.ImageAlign);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> ImageAlign_SetWithParentWithHandle_TestData()
    {
        foreach (ContentAlignment value in Enum.GetValues(typeof(ContentAlignment)))
        {
            int expectedCallCount = value == ContentAlignment.MiddleCenter ? 0 : 1;
            yield return new object[] { true, value, expectedCallCount, expectedCallCount };
            yield return new object[] { false, value, 0, expectedCallCount };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ImageAlign_SetWithParentWithHandle_TestData))]
    public void ButtonBase_ImageAlign_SetWithParentWithHandle_GetReturnsExpected(bool autoSize, ContentAlignment value, int expectedParentLayoutCallCount, int expectedInvalidatedCallCount)
    {
        using Control parent = new();
        using Button control = new()
        {
            Parent = parent,
            AutoSize = autoSize
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("ImageAlign", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.ImageAlign = value;
            Assert.Equal(value, control.ImageAlign);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);

            // Set same.
            control.ImageAlign = value;
            Assert.Equal(value, control.ImageAlign);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InvalidEnumData<ContentAlignment>]
    public void ButtonBase_ImageAlign_SetInvalid_ThrowsInvalidEnumArgumentException(ContentAlignment value)
    {
        using SubButtonBase control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.ImageAlign = value);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ButtonBase_ImageIndex_Set_GetReturnsExpected(int value)
    {
        using SubButtonBase control = new()
        {
            ImageIndex = value
        };
        Assert.Equal(value, control.ImageIndex);
        Assert.Empty(control.ImageKey);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ImageIndex = value;
        Assert.Equal(value, control.ImageIndex);
        Assert.Empty(control.ImageKey);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ButtonBase_ImageIndex_SetWithImage_GetReturnsExpected()
    {
        using Bitmap image = new(10, 10);
        using SubButtonBase control = new()
        {
            Image = image
        };

        // Set same.
        control.ImageIndex = -1;
        Assert.Empty(control.ImageKey);
        Assert.Equal(-1, control.ImageIndex);
        Assert.Same(image, control.Image);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.ImageIndex = 0;
        Assert.Empty(control.ImageKey);
        Assert.Equal(0, control.ImageIndex);
        Assert.Null(control.Image);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ButtonBase_ImageIndex_SetWithImageKey_GetReturnsExpected(int value)
    {
        using SubButtonBase control = new()
        {
            ImageKey = "ImageKey",
            ImageIndex = value
        };
        Assert.Equal(value, control.ImageIndex);
        Assert.Equal(ImageList.Indexer.DefaultKey, control.ImageKey);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ImageIndex = value;
        Assert.Equal(value, control.ImageIndex);
        Assert.Equal(ImageList.Indexer.DefaultKey, control.ImageKey);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ButtonBase_ImageIndex_SetWithEmptyList_GetReturnsExpected(int value)
    {
        using ImageList imageList = new();
        using SubButtonBase control = new()
        {
            ImageList = imageList
        };

        control.ImageIndex = value;
        Assert.Equal(-1, control.ImageIndex);
        Assert.Empty(control.ImageKey);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ImageIndex = value;
        Assert.Equal(-1, control.ImageIndex);
        Assert.Empty(control.ImageKey);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1, -1)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    public void ButtonBase_ImageIndex_SetWithNotEmptyList_GetReturnsExpected(int value, int expected)
    {
        using Bitmap image1 = new(10, 10);
        using Bitmap image2 = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add(image1);
        imageList.Images.Add(image2);
        using SubButtonBase control = new()
        {
            ImageList = imageList
        };

        control.ImageIndex = value;
        Assert.Equal(expected, control.ImageIndex);
        Assert.Empty(control.ImageKey);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ImageIndex = value;
        Assert.Equal(expected, control.ImageIndex);
        Assert.Empty(control.ImageKey);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1, 1, 2)]
    [InlineData(0, 1, 1)]
    [InlineData(1, 1, 1)]
    public void ButtonBase_ImageIndex_SetWithHandle_GetReturnsExpected(int value, int expectedInvalidatedCallCount1, int expectedInvalidatedCallCount2)
    {
        using SubButtonBase control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ImageIndex = value;
        Assert.Equal(value, control.ImageIndex);
        Assert.Empty(control.ImageKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.ImageIndex = value;
        Assert.Equal(value, control.ImageIndex);
        Assert.Empty(control.ImageKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(-2)]
    public void ButtonBase_ImageIndex_SetInvalid_ThrowsArgumentOutOfRangeException(int value)
    {
        using SubButtonBase control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.ImageIndex = value);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ButtonBase_ImageKey_Set_GetReturnsExpected(string value, string expected)
    {
        using SubButtonBase control = new()
        {
            ImageKey = value
        };
        Assert.Equal(expected, control.ImageKey);
        Assert.Equal(-1, control.ImageIndex);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ImageKey = value;
        Assert.Equal(expected, control.ImageKey);
        Assert.Equal(-1, control.ImageIndex);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ButtonBase_ImageKey_SetWithImage_GetReturnsExpected()
    {
        using Bitmap image = new(10, 10);
        using SubButtonBase control = new()
        {
            Image = image
        };

        control.ImageKey = ImageList.Indexer.DefaultKey;
        Assert.Equal(ImageList.Indexer.DefaultKey, control.ImageKey);
        Assert.Equal(ImageList.Indexer.DefaultIndex, control.ImageIndex);
        Assert.Null(control.Image);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("ImageKey", "ImageKey")]
    public void ButtonBase_ImageKey_SetWithImageIndex_GetReturnsExpected(string value, string expectedImageKey)
    {
        using SubButtonBase control = new()
        {
            ImageIndex = 0,
            ImageKey = value
        };
        Assert.Equal(expectedImageKey, control.ImageKey);
        Assert.Equal(ImageList.Indexer.DefaultIndex, control.ImageIndex);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ImageKey = value;
        Assert.Equal(expectedImageKey, control.ImageKey);
        Assert.Equal(ImageList.Indexer.DefaultIndex, control.ImageIndex);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ButtonBase_ImageKey_SetWithEmptyList_GetReturnsExpected(string value, string expected)
    {
        using ImageList imageList = new();
        using SubButtonBase control = new()
        {
            ImageList = imageList
        };

        control.ImageKey = value;
        Assert.Equal(expected, control.ImageKey);
        Assert.Equal(-1, control.ImageIndex);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ImageKey = value;
        Assert.Equal(expected, control.ImageKey);
        Assert.Equal(-1, control.ImageIndex);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("Image1", "Image1")]
    [InlineData("image1", "image1")]
    [InlineData("Image2", "Image2")]
    [InlineData("NoSuchImage", "NoSuchImage")]
    public void ButtonBase_ImageKey_SetWithNotEmptyList_GetReturnsExpected(string value, string expected)
    {
        using Bitmap image1 = new(10, 10);
        using Bitmap image2 = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add("Image1", image1);
        imageList.Images.Add("Image2", image2);
        using SubButtonBase control = new()
        {
            ImageList = imageList
        };

        control.ImageKey = value;
        Assert.Equal(expected, control.ImageKey);
        Assert.Equal(-1, control.ImageIndex);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ImageKey = value;
        Assert.Equal(expected, control.ImageKey);
        Assert.Equal(-1, control.ImageIndex);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(null, "", 1, 2)]
    [InlineData("", "", 1, 2)]
    [InlineData("ImageKey", "ImageKey", 1, 1)]
    public void ButtonBase_ImageKey_SetWithHandle_GetReturnsExpected(string value, string expected, int expectedInvalidatedCallCount1, int expectedInvalidatedCallCount2)
    {
        using SubButtonBase control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ImageKey = value;
        Assert.Equal(expected, control.ImageKey);
        Assert.Equal(-1, control.ImageIndex);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.ImageKey = value;
        Assert.Equal(expected, control.ImageKey);
        Assert.Equal(-1, control.ImageIndex);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> ImageList_Set_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new ImageList() };
    }

    [WinFormsTheory]
    [MemberData(nameof(ImageList_Set_TestData))]
    public void ButtonBase_ImageList_Set_GetReturnsExpected(ImageList value)
    {
        using SubButtonBase control = new()
        {
            ImageList = value
        };
        Assert.Same(value, control.ImageList);
        Assert.Null(control.Image);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ImageList = value;
        Assert.Same(value, control.ImageList);
        Assert.Null(control.Image);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ButtonBase_ImageList_SetWithImage_GetReturnsExpected()
    {
        using Bitmap image = new(10, 10);
        using SubButtonBase control = new()
        {
            Image = image
        };

        // Set same.
        control.ImageList = null;
        Assert.Null(control.ImageList);
        Assert.Same(image, control.Image);
        Assert.False(control.IsHandleCreated);

        // Set different.
        using ImageList imageList = new();
        control.ImageList = imageList;
        Assert.Same(imageList, control.ImageList);
        Assert.Null(control.Image);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ImageList_Set_TestData))]
    public void ButtonBase_ImageList_SetWithNonNullOldValue_GetReturnsExpected(ImageList value)
    {
        using ImageList oldValue = new();
        using SubButtonBase control = new()
        {
            ImageList = oldValue
        };

        control.ImageList = value;
        Assert.Same(value, control.ImageList);
        Assert.Null(control.Image);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ImageList = value;
        Assert.Same(value, control.ImageList);
        Assert.Null(control.Image);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> ImageList_SetWithHandle_TestData()
    {
        yield return new object[] { null, 0 };
        yield return new object[] { new ImageList(), 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(ImageList_SetWithHandle_TestData))]
    public void ButtonBase_ImageList_SetWithHandle_GetReturnsExpected(ImageList value, int expectedInvalidatedCallCount)
    {
        using SubButtonBase control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ImageList = value;
        Assert.Same(value, control.ImageList);
        Assert.Null(control.Image);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.ImageList = value;
        Assert.Same(value, control.ImageList);
        Assert.Null(control.Image);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> ImageList_SetWithNonNullOldValueWithHandle_TestData()
    {
        yield return new object[] { null, 1 };
        yield return new object[] { new ImageList(), 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(ImageList_SetWithNonNullOldValueWithHandle_TestData))]
    public void ButtonBase_ImageList_SetWithNonNullOldValueWithHandle_GetReturnsExpected(ImageList value, int expectedInvalidatedCallCount)
    {
        using ImageList oldValue = new();
        using SubButtonBase control = new()
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
        Assert.Null(control.Image);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.ImageList = value;
        Assert.Same(value, control.ImageList);
        Assert.Null(control.Image);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ButtonBase_ImageList_Set_DoesNotCreateImageHandle()
    {
        using SubButtonBase control = new();
        using ImageList imageList = new();
        control.ImageList = imageList;
        Assert.False(imageList.HandleCreated);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ButtonBase_ImageList_Dispose_DetachesFromButtonBase()
    {
        using ImageList imageList1 = new();
        using ImageList imageList2 = new();
        using SubButtonBase control = new()
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
    public void ButtonBase_ImageList_DisposeWithHandle_DetachesFromButtonBase()
    {
        using ImageList imageList1 = new();
        using ImageList imageList2 = new();
        using SubButtonBase control = new();
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
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        imageList1.Dispose();
        Assert.Null(control.ImageList);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Make sure we detached the setter.
        control.ImageList = imageList2;
        imageList1.Dispose();
        Assert.Same(imageList2, control.ImageList);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(3, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ButtonBase_ImageList_RecreateHandle_Nop()
    {
        using ImageList imageList1 = new();
        int recreateCallCount1 = 0;
        imageList1.RecreateHandle += (sender, e) => recreateCallCount1++;
        using ImageList imageList2 = new();
        using SubButtonBase control = new()
        {
            ImageList = imageList1
        };
        Assert.Same(imageList1, control.ImageList);
        Assert.Equal(0, recreateCallCount1);
        Assert.NotEqual(IntPtr.Zero, imageList1.Handle);

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
    public void ButtonBase_ImageList_RecreateHandleWithHandle_Success()
    {
        using ImageList imageList1 = new();
        int recreateCallCount1 = 0;
        imageList1.RecreateHandle += (sender, e) => recreateCallCount1++;
        using ImageList imageList2 = new();
        using SubButtonBase control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        Assert.NotEqual(IntPtr.Zero, imageList1.Handle);

        control.ImageList = imageList1;
        Assert.Same(imageList1, control.ImageList);
        Assert.Equal(0, recreateCallCount1);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        imageList1.ImageSize = new Size(1, 2);
        Assert.Equal(1, recreateCallCount1);
        Assert.Same(imageList1, control.ImageList);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Make sure we detached the setter.
        control.ImageList = imageList2;
        imageList1.ImageSize = new Size(2, 3);
        Assert.Equal(2, recreateCallCount1);
        Assert.Same(imageList2, control.ImageList);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(3, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [EnumData<ImageLayout>]
    public void ButtonBase_ImeMode_Set_GetReturnsExpected(ImeMode value)
    {
        using SubButtonBase control = new()
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
    public void ButtonBase_ImeMode_SetWithHandler_CallsImeModeChanged()
    {
        using SubButtonBase control = new();
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
    public void ButtonBase_ImeMode_SetInvalid_ThrowsInvalidEnumArgumentException(ImeMode value)
    {
        using SubButtonBase control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.ImeMode = value);
    }

    public static IEnumerable<object[]> IsDefault_Set_TestData()
    {
        foreach (FlatStyle flatStyle in Enum.GetValues(typeof(FlatStyle)))
        {
            yield return new object[] { flatStyle, true };
            yield return new object[] { flatStyle, false };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(IsDefault_Set_TestData))]
    public void ButtonBase_IsDefault_Set_GetReturnsExpected(FlatStyle flatStyle, bool value)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle,
            IsDefault = value
        };
        Assert.Equal(value, control.IsDefault);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.IsDefault = value;
        Assert.Equal(value, control.IsDefault);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.IsDefault = !value;
        Assert.Equal(!value, control.IsDefault);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> IsDefault_SetWithHandle_TestData()
    {
        yield return new object[] { FlatStyle.Flat, true, 1, 0, 2, 0 };
        yield return new object[] { FlatStyle.Popup, true, 1, 0, 2, 0 };
        yield return new object[] { FlatStyle.Standard, true, 1, 0, 2, 0 };
        yield return new object[] { FlatStyle.System, true, 1, 1, 2, 2 };

        yield return new object[] { FlatStyle.Flat, false, 0, 0, 1, 0 };
        yield return new object[] { FlatStyle.Popup, false, 0, 0, 1, 0 };
        yield return new object[] { FlatStyle.Standard, false, 0, 0, 1, 0 };
        yield return new object[] { FlatStyle.System, false, 0, 0, 1, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(IsDefault_SetWithHandle_TestData))]
    public void ButtonBase_IsDefault_SetWithHandle_GetReturnsExpected(FlatStyle flatStyle, bool value, int expectedInvalidatedCallCount1, int expectedStyleChangeCallCount1, int expectedInvalidatedCallCount2, int expectedStyleChangeCallCount2)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.IsDefault = value;
        Assert.Equal(value, control.IsDefault);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
        Assert.Equal(expectedStyleChangeCallCount1, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.IsDefault = value;
        Assert.Equal(value, control.IsDefault);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
        Assert.Equal(expectedStyleChangeCallCount1, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.IsDefault = !value;
        Assert.Equal(!value, control.IsDefault);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount2, invalidatedCallCount);
        Assert.Equal(expectedStyleChangeCallCount2, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> Parent_Set_TestData()
    {
        foreach (bool enabled in new bool[] { true, false })
        {
            foreach (bool visible in new bool[] { true, false })
            {
                foreach (Image image in new Image[] { null, new Bitmap(10, 10) })
                {
                    yield return new object[] { enabled, visible, image, null };
                    yield return new object[] { enabled, visible, image, new Control() };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Parent_Set_TestData))]
    public void ButtonBase_Parent_Set_GetReturnsExpected(bool enabled, bool visible, Image image, Control value)
    {
        using SubButtonBase control = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image,
            Parent = value
        };
        Assert.Same(value, control.Parent);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Parent = value;
        Assert.Same(value, control.Parent);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Parent_Set_TestData))]
    public void ButtonBase_Parent_SetDesignMode_GetReturnsExpected(bool enabled, bool visible, Image image, Control value)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Name)
            .Returns((string)null);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using SubButtonBase control = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image,
            Site = mockSite.Object,
            Parent = value
        };
        Assert.Same(value, control.Parent);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Parent = value;
        Assert.Same(value, control.Parent);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Parent_Set_TestData))]
    public void ButtonBase_Parent_SetWithNonNullOldParent_GetReturnsExpected(bool enabled, bool visible, Image image, Control value)
    {
        using Control oldParent = new();
        using SubButtonBase control = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image,
            Parent = oldParent
        };

        control.Parent = value;
        Assert.Same(value, control.Parent);
        Assert.Empty(oldParent.Controls);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Parent = value;
        Assert.Same(value, control.Parent);
        Assert.Empty(oldParent.Controls);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ButtonBase_Parent_SetNonNull_AddsToControls()
    {
        using Control parent = new();
        using SubButtonBase control = new()
        {
            Parent = parent
        };
        Assert.Same(parent, control.Parent);
        Assert.Same(control, Assert.Single(parent.Controls));
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Parent = parent;
        Assert.Same(parent, control.Parent);
        Assert.Same(control, Assert.Single(parent.Controls));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Parent_Set_TestData))]
    public void ButtonBase_Parent_SetWithHandle_GetReturnsExpected(bool enabled, bool visible, Image image, Control value)
    {
        using SubButtonBase control = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Parent = value;
        Assert.Same(value, control.Parent);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Parent = value;
        Assert.Same(value, control.Parent);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(Parent_Set_TestData))]
    public void ButtonBase_Parent_SetDesignModeWithHandle_GetReturnsExpected(bool enabled, bool visible, Image image, Control value)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Name)
            .Returns((string)null);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using SubButtonBase control = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image,
            Site = mockSite.Object
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Parent = value;
        Assert.Same(value, control.Parent);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Parent = value;
        Assert.Same(value, control.Parent);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ButtonBase_Parent_SetWithHandler_CallsParentChanged()
    {
        using Control parent = new();
        using SubButtonBase control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.ParentChanged += handler;

        // Set different.
        control.Parent = parent;
        Assert.Same(parent, control.Parent);
        Assert.Equal(1, callCount);

        // Set same.
        control.Parent = parent;
        Assert.Same(parent, control.Parent);
        Assert.Equal(1, callCount);

        // Set null.
        control.Parent = null;
        Assert.Null(control.Parent);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.ParentChanged -= handler;
        control.Parent = parent;
        Assert.Same(parent, control.Parent);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void ButtonBase_Parent_SetSame_ThrowsArgumentException()
    {
        using SubButtonBase control = new();
        Assert.Throws<ArgumentException>(() => control.Parent = control);
        Assert.Null(control.Parent);
    }

    public static IEnumerable<object[]> Text_Set_TestData()
    {
        foreach (bool autoSize in new bool[] { true, false })
        {
            yield return new object[] { autoSize, null, string.Empty };
            yield return new object[] { autoSize, string.Empty, string.Empty };
            yield return new object[] { autoSize, "text", "text" };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Text_Set_TestData))]
    public void ButtonBase_Text_Set_GetReturnsExpected(bool autoSize, string value, string expected)
    {
        using SubButtonBase control = new()
        {
            AutoSize = autoSize
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Text_SetWithParent_TestData()
    {
        yield return new object[] { true, null, string.Empty, 0 };
        yield return new object[] { true, string.Empty, string.Empty, 0 };
        yield return new object[] { true, "text", "text", 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Text_SetWithParent_TestData))]
    public void ButtonBase_Text_SetWithParent_GetReturnsExpected(bool autoSize, string value, string expected, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using Button control = new()
        {
            AutoSize = autoSize,
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Text", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.Equal(expectedParentLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.Equal(expectedParentLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> Text_SetWithHandle_TestData()
    {
        foreach (bool autoSize in new bool[] { true, false })
        {
            yield return new object[] { autoSize, null, string.Empty, 0 };
            yield return new object[] { autoSize, string.Empty, string.Empty, 0 };
            yield return new object[] { autoSize, "text", "text", 1 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Text_SetWithHandle_TestData))]
    public void ButtonBase_Text_SetWithHandle_GetReturnsExpected(bool autoSize, string value, string expected, int expectedInvalidatedCallCount)
    {
        using SubButtonBase control = new()
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

        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> Text_SetWithParentWithHandle_TestData()
    {
        yield return new object[] { true, null, string.Empty, 0 };
        yield return new object[] { true, string.Empty, string.Empty, 0 };
        yield return new object[] { true, "text", "text", 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Text_SetWithParentWithHandle_TestData))]
    public void ButtonBase_Text_SetWithParentWithHandle_GetReturnsExpected(bool autoSize, string value, string expected, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using Button control = new()
        {
            AutoSize = autoSize,
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Text", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.Equal(expectedParentLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedParentLayoutCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.Equal(expectedParentLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedParentLayoutCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void ButtonBase_Text_SetWithHandler_CallsTextChanged()
    {
        using SubButtonBase control = new();
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

    public static IEnumerable<object[]> TextAlign_Set_TestData()
    {
        foreach (bool autoSize in new bool[] { true, false })
        {
            foreach (FlatStyle flatStyle in Enum.GetValues(typeof(FlatStyle)))
            {
                foreach (ContentAlignment value in Enum.GetValues(typeof(ContentAlignment)))
                {
                    yield return new object[] { autoSize, flatStyle, value };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(TextAlign_Set_TestData))]
    public void ButtonBase_TextAlign_Set_GetReturnsExpected(bool autoSize, FlatStyle flatStyle, ContentAlignment value)
    {
        using SubButtonBase control = new()
        {
            AutoSize = autoSize,
            FlatStyle = flatStyle
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.TextAlign = value;
        Assert.Equal(value, control.TextAlign);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.TextAlign = value;
        Assert.Equal(value, control.TextAlign);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> TextAlign_SetWithParent_TestData()
    {
        foreach (FlatStyle flatStyle in Enum.GetValues(typeof(FlatStyle)))
        {
            foreach (ContentAlignment value in Enum.GetValues(typeof(ContentAlignment)))
            {
                int expectedCallCount = value == ContentAlignment.MiddleCenter ? 0 : 1;
                yield return new object[] { true, flatStyle, value, expectedCallCount };
                yield return new object[] { false, flatStyle, value, 0 };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(TextAlign_SetWithParent_TestData))]
    public void ButtonBase_TextAlign_SetWithParent_GetReturnsExpected(bool autoSize, FlatStyle flatStyle, ContentAlignment value, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using Button control = new()
        {
            AutoSize = autoSize,
            FlatStyle = flatStyle,
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("TextAlign", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.TextAlign = value;
            Assert.Equal(value, control.TextAlign);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            control.TextAlign = value;
            Assert.Equal(value, control.TextAlign);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> TextAlign_SetWithHandle_TestData()
    {
        foreach (bool autoSize in new bool[] { true, false })
        {
            foreach (ContentAlignment value in Enum.GetValues(typeof(ContentAlignment)))
            {
                int expectedCallCount = value == ContentAlignment.MiddleCenter ? 0 : 1;
                yield return new object[] { autoSize, FlatStyle.Flat, value, expectedCallCount, 0 };
                yield return new object[] { autoSize, FlatStyle.Popup, value, expectedCallCount, 0 };
                yield return new object[] { autoSize, FlatStyle.Standard, value, expectedCallCount, 0 };
                yield return new object[] { autoSize, FlatStyle.System, value, expectedCallCount, expectedCallCount };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(TextAlign_SetWithHandle_TestData))]
    public void ButtonBase_TextAlign_SetWithHandle_GetReturnsExpected(bool autoSize, FlatStyle flatStyle, ContentAlignment value, int expectedInvalidatedCallCount, int expectedStyleChangedCallCount)
    {
        using SubButtonBase control = new()
        {
            AutoSize = autoSize,
            FlatStyle = flatStyle
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

        control.TextAlign = value;
        Assert.Equal(value, control.TextAlign);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.TextAlign = value;
        Assert.Equal(value, control.TextAlign);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> TextAlign_SetWithParentWithHandle_TestData()
    {
        foreach (ContentAlignment value in Enum.GetValues(typeof(ContentAlignment)))
        {
            int expectedCallCount = value == ContentAlignment.MiddleCenter ? 0 : 1;
            yield return new object[] { true, FlatStyle.Flat, value, expectedCallCount, expectedCallCount, 0 };
            yield return new object[] { true, FlatStyle.Popup, value, expectedCallCount, expectedCallCount, 0 };
            yield return new object[] { true, FlatStyle.Standard, value, expectedCallCount, expectedCallCount, 0 };
            yield return new object[] { true, FlatStyle.System, value, expectedCallCount, expectedCallCount, expectedCallCount };

            yield return new object[] { false, FlatStyle.Flat, value, 0, expectedCallCount, 0 };
            yield return new object[] { false, FlatStyle.Popup, value, 0, expectedCallCount, 0 };
            yield return new object[] { false, FlatStyle.Standard, value, 0, expectedCallCount, 0 };
            yield return new object[] { false, FlatStyle.System, value, 0, expectedCallCount, expectedCallCount };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(TextAlign_SetWithParentWithHandle_TestData))]
    public void ButtonBase_TextAlign_SetWithParentWithHandle_GetReturnsExpected(bool autoSize, FlatStyle flatStyle, ContentAlignment value, int expectedParentLayoutCallCount, int expectedInvalidatedCallCount, int expectedStyleChangedCallCount)
    {
        using Control parent = new();
        using Button control = new()
        {
            AutoSize = autoSize,
            FlatStyle = flatStyle,
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("TextAlign", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.TextAlign = value;
            Assert.Equal(value, control.TextAlign);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);

            // Set same.
            control.TextAlign = value;
            Assert.Equal(value, control.TextAlign);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InvalidEnumData<ContentAlignment>]
    public void ButtonBase_TextAlign_SetInvalidValue_ThrowsInvalidEnumArgumentException(ContentAlignment value)
    {
        using SubButtonBase control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.TextAlign = value);
    }

    public static IEnumerable<object[]> TextImageRelation_Set_TestData()
    {
        foreach (bool autoSize in new bool[] { true, false })
        {
            foreach (TextImageRelation value in Enum.GetValues(typeof(TextImageRelation)))
            {
                yield return new object[] { autoSize, value };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(TextImageRelation_Set_TestData))]
    public void ButtonBase_TextImageRelation_Set_GetReturnsExpected(bool autoSize, TextImageRelation value)
    {
        using SubButtonBase control = new()
        {
            AutoSize = autoSize
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.TextImageRelation = value;
        Assert.Equal(value, control.TextImageRelation);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.TextImageRelation = value;
        Assert.Equal(value, control.TextImageRelation);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> TextImageRelation_SetWithParent_TestData()
    {
        yield return new object[] { true, TextImageRelation.ImageAboveText, 1 };
        yield return new object[] { true, TextImageRelation.ImageBeforeText, 1 };
        yield return new object[] { true, TextImageRelation.Overlay, 0 };
        yield return new object[] { true, TextImageRelation.TextAboveImage, 1 };
        yield return new object[] { true, TextImageRelation.ImageBeforeText, 1 };

        yield return new object[] { false, TextImageRelation.ImageAboveText, 0 };
        yield return new object[] { false, TextImageRelation.ImageBeforeText, 0 };
        yield return new object[] { false, TextImageRelation.Overlay, 0 };
        yield return new object[] { false, TextImageRelation.TextAboveImage, 0 };
        yield return new object[] { false, TextImageRelation.ImageBeforeText, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(TextImageRelation_SetWithParent_TestData))]
    public void ButtonBase_TextImageRelation_SetWithParent_GetReturnsExpected(bool autoSize, TextImageRelation value, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using Button control = new()
        {
            Parent = parent,
            AutoSize = autoSize
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("TextImageRelation", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.TextImageRelation = value;
            Assert.Equal(value, control.TextImageRelation);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            control.TextImageRelation = value;
            Assert.Equal(value, control.TextImageRelation);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> TextImageRelation_SetWithHandle_TestData()
    {
        foreach (bool autoSize in new bool[] { true, false })
        {
            yield return new object[] { autoSize, TextImageRelation.ImageAboveText, 1 };
            yield return new object[] { autoSize, TextImageRelation.ImageBeforeText, 1 };
            yield return new object[] { autoSize, TextImageRelation.Overlay, 0 };
            yield return new object[] { autoSize, TextImageRelation.TextAboveImage, 1 };
            yield return new object[] { autoSize, TextImageRelation.ImageBeforeText, 1 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(TextImageRelation_SetWithHandle_TestData))]
    public void ButtonBase_TextImageRelation_SetWithHandle_GetReturnsExpected(bool autoSize, TextImageRelation value, int expectedInvalidatedCallCount)
    {
        using SubButtonBase control = new()
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
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.TextImageRelation = value;
        Assert.Equal(value, control.TextImageRelation);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.TextImageRelation = value;
        Assert.Equal(value, control.TextImageRelation);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> TextImageRelation_SetWithParentWithHandle_TestData()
    {
        yield return new object[] { true, TextImageRelation.ImageAboveText, 1, 1 };
        yield return new object[] { true, TextImageRelation.ImageBeforeText, 1, 1 };
        yield return new object[] { true, TextImageRelation.Overlay, 0, 0 };
        yield return new object[] { true, TextImageRelation.TextAboveImage, 1, 1 };
        yield return new object[] { true, TextImageRelation.ImageBeforeText, 1, 1 };

        yield return new object[] { false, TextImageRelation.ImageAboveText, 0, 1 };
        yield return new object[] { false, TextImageRelation.ImageBeforeText, 0, 1 };
        yield return new object[] { false, TextImageRelation.Overlay, 0, 0 };
        yield return new object[] { false, TextImageRelation.TextAboveImage, 0, 1 };
        yield return new object[] { false, TextImageRelation.ImageBeforeText, 0, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(TextImageRelation_SetWithParentWithHandle_TestData))]
    public void ButtonBase_TextImageRelation_SetWithParentWithHandle_GetReturnsExpected(bool autoSize, TextImageRelation value, int expectedParentLayoutCallCount, int expectedInvalidatedCallCount)
    {
        using Control parent = new();
        using Button control = new()
        {
            Parent = parent,
            AutoSize = autoSize
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("TextImageRelation", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.TextImageRelation = value;
            Assert.Equal(value, control.TextImageRelation);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);

            // Set same.
            control.TextImageRelation = value;
            Assert.Equal(value, control.TextImageRelation);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InvalidEnumData<TextImageRelation>]
    [InlineData((TextImageRelation)3)]
    [InlineData((TextImageRelation)5)]
    [InlineData((TextImageRelation)6)]
    [InlineData((TextImageRelation)7)]
    public void ButtonBase_TextImageRelation_SetInvalid_ThrowsInvalidEnumArgumentException(TextImageRelation value)
    {
        using SubButtonBase control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.TextImageRelation = value);
    }

    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void ButtonBase_UseCompatibleTextRendering_Set_GetReturnsExpected(bool autoSize, bool value)
    {
        using SubButtonBase control = new()
        {
            AutoSize = autoSize,
            UseCompatibleTextRendering = value
        };
        Assert.Equal(value, control.UseCompatibleTextRendering);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.UseCompatibleTextRendering = value;
        Assert.Equal(value, control.UseCompatibleTextRendering);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.UseCompatibleTextRendering = !value;
        Assert.Equal(!value, control.UseCompatibleTextRendering);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, true, 0, 1)]
    [InlineData(true, false, 1, 2)]
    [InlineData(false, true, 0, 0)]
    [InlineData(false, false, 0, 0)]
    public void ButtonBase_UseCompatibleTextRendering_SetWithParent_GetReturnsExpected(bool autoSize, bool value, int expectedParentLayoutCallCount1, int expectedParentLayoutCallCount2)
    {
        using Control parent = new();
        using Button control = new()
        {
            AutoSize = autoSize,
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("UseCompatibleTextRendering", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.UseCompatibleTextRendering = value;
            Assert.Equal(value, control.UseCompatibleTextRendering);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount1, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.UseCompatibleTextRendering = value;
            Assert.Equal(value, control.UseCompatibleTextRendering);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount1, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.UseCompatibleTextRendering = !value;
            Assert.Equal(!value, control.UseCompatibleTextRendering);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount2, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(true, true, 0)]
    [InlineData(true, false, 1)]
    [InlineData(false, true, 0)]
    [InlineData(false, false, 1)]
    public void ButtonBase_UseCompatibleTextRendering_SetWithHandle_GetReturnsExpected(bool autoSize, bool value, int expectedInvalidatedCallCount)
    {
        using SubButtonBase control = new()
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

        control.UseCompatibleTextRendering = value;
        Assert.Equal(value, control.UseCompatibleTextRendering);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.UseCompatibleTextRendering = value;
        Assert.Equal(value, control.UseCompatibleTextRendering);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.UseCompatibleTextRendering = !value;
        Assert.Equal(!value, control.UseCompatibleTextRendering);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(true, true, 0, 0, 1, 1)]
    [InlineData(true, false, 1, 1, 2, 2)]
    [InlineData(false, true, 0, 0, 1, 0)]
    [InlineData(false, false, 1, 0, 2, 0)]
    public void ButtonBase_UseCompatibleTextRendering_SetWithParentWithHandle_GetReturnsExpected(bool autoSize, bool value, int expectedInvalidatedCallCount1, int expectedParentLayoutCallCount1, int expectedInvalidatedCallCount2, int expectedParentLayoutCallCount2)
    {
        using Control parent = new();
        using Button control = new()
        {
            AutoSize = autoSize,
            Parent = parent
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
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("UseCompatibleTextRendering", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.UseCompatibleTextRendering = value;
            Assert.Equal(value, control.UseCompatibleTextRendering);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount1, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.UseCompatibleTextRendering = value;
            Assert.Equal(value, control.UseCompatibleTextRendering);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount1, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.UseCompatibleTextRendering = !value;
            Assert.Equal(!value, control.UseCompatibleTextRendering);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount2, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> UseMnemonic_Set_TestData()
    {
        foreach (bool autoSize in new bool[] { true, false })
        {
            yield return new object[] { autoSize, true };
            yield return new object[] { autoSize, false };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(UseMnemonic_Set_TestData))]
    public void ButtonBase_UseMnemonic_Set_GetReturnsExpected(bool autoSize, bool value)
    {
        using SubButtonBase control = new()
        {
            AutoSize = autoSize
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.UseMnemonic = value;
        Assert.Equal(value, control.UseMnemonic);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.UseMnemonic = value;
        Assert.Equal(value, control.UseMnemonic);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.UseMnemonic = !value;
        Assert.Equal(!value, control.UseMnemonic);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> UseMnemonic_SetWithParent_TestData()
    {
        yield return new object[] { true, true, 0, 1 };
        yield return new object[] { true, false, 1, 2 };
        yield return new object[] { false, true, 0, 0 };
        yield return new object[] { false, false, 0, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(UseMnemonic_SetWithParent_TestData))]
    public void ButtonBase_UseMnemonic_SetWithParent_GetReturnsExpected(bool autoSize, bool value, int expectedParentLayoutCallCount1, int expectedParentLayoutCallCount2)
    {
        using Control parent = new();
        using Button control = new()
        {
            Parent = parent,
            AutoSize = autoSize
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Text", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.UseMnemonic = value;
            Assert.Equal(value, control.UseMnemonic);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount1, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            control.UseMnemonic = value;
            Assert.Equal(value, control.UseMnemonic);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount1, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set different.
            control.UseMnemonic = !value;
            Assert.Equal(!value, control.UseMnemonic);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount2, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> UseMnemonic_SetWithHandle_TestData()
    {
        foreach (bool autoSize in new bool[] { true, false })
        {
            yield return new object[] { autoSize, true, 0 };
            yield return new object[] { autoSize, false, 1 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(UseMnemonic_SetWithHandle_TestData))]
    public void ButtonBase_UseMnemonic_SetWithHandle_GetReturnsExpected(bool autoSize, bool value, int expectedInvalidatedCallCount)
    {
        using SubButtonBase control = new()
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
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.UseMnemonic = value;
        Assert.Equal(value, control.UseMnemonic);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.UseMnemonic = value;
        Assert.Equal(value, control.UseMnemonic);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.UseMnemonic = !value;
        Assert.Equal(!value, control.UseMnemonic);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> UseMnemonic_SetWithParentWithHandle_TestData()
    {
        yield return new object[] { true, true, 0, 1, 0, 1 };
        yield return new object[] { true, false, 1, 2, 1, 2 };
        yield return new object[] { false, true, 0, 0, 0, 1 };
        yield return new object[] { false, false, 0, 0, 1, 2 };
    }

    [WinFormsTheory]
    [MemberData(nameof(UseMnemonic_SetWithParentWithHandle_TestData))]
    public void ButtonBase_UseMnemonic_SetWithParentWithHandle_GetReturnsExpected(bool autoSize, bool value, int expectedParentLayoutCallCount1, int expectedParentLayoutCallCount2, int expectedInvalidatedCallCount1, int expectedInvalidatedCallCount2)
    {
        using Control parent = new();
        using Button control = new()
        {
            Parent = parent,
            AutoSize = autoSize
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Text", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.UseMnemonic = value;
            Assert.Equal(value, control.UseMnemonic);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount1, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);

            // Set same.
            control.UseMnemonic = value;
            Assert.Equal(value, control.UseMnemonic);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount1, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);

            // Set different.
            control.UseMnemonic = !value;
            Assert.Equal(!value, control.UseMnemonic);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount2, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [BoolData]
    public void ButtonBase_UseVisualStyleBackColor_Set_GetReturnsExpected(bool value)
    {
        using SubButtonBase control = new()
        {
            UseVisualStyleBackColor = value
        };
        Assert.Equal(value, control.UseVisualStyleBackColor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.UseVisualStyleBackColor = value;
        Assert.Equal(value, control.UseVisualStyleBackColor);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.UseVisualStyleBackColor = !value;
        Assert.Equal(!value, control.UseVisualStyleBackColor);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ButtonBase_UseVisualStyleBackColor_SetWithCustomOldValue_GetReturnsExpected(bool value)
    {
        using SubButtonBase control = new()
        {
            UseVisualStyleBackColor = true
        };

        control.UseVisualStyleBackColor = value;
        Assert.Equal(value, control.UseVisualStyleBackColor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.UseVisualStyleBackColor = value;
        Assert.Equal(value, control.UseVisualStyleBackColor);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.UseVisualStyleBackColor = !value;
        Assert.Equal(!value, control.UseVisualStyleBackColor);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 1)]
    public void ButtonBase_UseVisualStyleBackColor_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount)
    {
        using SubButtonBase control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.UseVisualStyleBackColor = value;
        Assert.Equal(value, control.UseVisualStyleBackColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.UseVisualStyleBackColor = value;
        Assert.Equal(value, control.UseVisualStyleBackColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.UseVisualStyleBackColor = !value;
        Assert.Equal(!value, control.UseVisualStyleBackColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(true, 0)]
    [InlineData(false, 1)]
    public void ButtonBase_UseVisualStyleBackColor_SetWithCustomOldValueWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount)
    {
        using SubButtonBase control = new()
        {
            UseVisualStyleBackColor = true
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.UseVisualStyleBackColor = value;
        Assert.Equal(value, control.UseVisualStyleBackColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.UseVisualStyleBackColor = value;
        Assert.Equal(value, control.UseVisualStyleBackColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.UseVisualStyleBackColor = !value;
        Assert.Equal(!value, control.UseVisualStyleBackColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ButtonBase_UseVisualStyleBackColor_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ButtonBase))[nameof(ButtonBase.UseVisualStyleBackColor)];
        using SubButtonBase control = new();
        Assert.False(property.CanResetValue(control));

        control.UseVisualStyleBackColor = false;
        Assert.False(control.UseVisualStyleBackColor);
        Assert.True(property.CanResetValue(control));
        Assert.False(control.IsHandleCreated);

        property.ResetValue(control);
        Assert.True(control.UseVisualStyleBackColor);
        Assert.False(property.CanResetValue(control));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ButtonBase_UseVisualStyleBackColor_ResetValueWithHandle_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ButtonBase))[nameof(ButtonBase.UseVisualStyleBackColor)];
        using SubButtonBase control = new();
        Assert.False(property.CanResetValue(control));

        control.UseVisualStyleBackColor = false;
        Assert.False(control.UseVisualStyleBackColor);
        Assert.True(property.CanResetValue(control));

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        property.ResetValue(control);
        Assert.True(control.UseVisualStyleBackColor);
        Assert.False(property.CanResetValue(control));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ButtonBase_UseVisualStyleBackColor_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ButtonBase))[nameof(ButtonBase.UseVisualStyleBackColor)];
        using SubButtonBase control = new();
        Assert.False(property.ShouldSerializeValue(control));

        control.UseVisualStyleBackColor = false;
        Assert.False(control.UseVisualStyleBackColor);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.True(control.UseVisualStyleBackColor);
        Assert.False(property.ShouldSerializeValue(control));
    }

    public static IEnumerable<object[]> Visible_Set_TestData()
    {
        foreach (bool enabled in new bool[] { true, false })
        {
            foreach (Image image in new Image[] { null, new Bitmap(10, 10) })
            {
                yield return new object[] { enabled, image, true };
                yield return new object[] { enabled, image, false };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Visible_Set_TestData))]
    public void ButtonBase_Visible_Set_GetReturnsExpected(bool enabled, Image image, bool value)
    {
        using SubButtonBase control = new()
        {
            Enabled = enabled,
            Image = image,
            Visible = value
        };
        Assert.Equal(value, control.Visible);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Visible = value;
        Assert.Equal(value, control.Visible);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.Visible = !value;
        Assert.Equal(!value, control.Visible);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Visible_Set_TestData))]
    public void ButtonBase_Visible_SetDesignMode_GetReturnsExpected(bool enabled, Image image, bool value)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Name)
            .Returns((string)null);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using SubButtonBase control = new()
        {
            Enabled = enabled,
            Image = image,
            Site = mockSite.Object,
            Visible = value
        };
        Assert.Equal(value, control.Visible);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Visible = value;
        Assert.Equal(value, control.Visible);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.Visible = !value;
        Assert.Equal(!value, control.Visible);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Visible_SetWithHandle_TestData()
    {
        foreach (bool enabled in new bool[] { true, false })
        {
            foreach (Image image in new Image[] { null, new Bitmap(10, 10) })
            {
                yield return new object[] { enabled, image, true };
                yield return new object[] { enabled, image, false };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Visible_SetWithHandle_TestData))]
    public void ButtonBase_Visible_SetWithHandle_GetReturnsExpected(bool enabled, Image image, bool value)
    {
        using SubButtonBase control = new()
        {
            Enabled = enabled,
            Image = image
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Visible = value;
        Assert.Equal(value, control.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Visible = value;
        Assert.Equal(value, control.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.Visible = !value;
        Assert.Equal(!value, control.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(Visible_SetWithHandle_TestData))]
    public void ButtonBase_Visible_SetDesignModeWithHandle_GetReturnsExpected(bool enabled, Image image, bool value)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Name)
            .Returns((string)null);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using SubButtonBase control = new()
        {
            Enabled = enabled,
            Image = image,
            Site = mockSite.Object
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Visible = value;
        Assert.Equal(value, control.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Visible = value;
        Assert.Equal(value, control.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.Visible = !value;
        Assert.Equal(!value, control.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ButtonBase_Visible_SetWithHandler_CallsVisibleChanged()
    {
        using SubButtonBase control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.VisibleChanged += handler;

        // Set different.
        control.Visible = false;
        Assert.False(control.Visible);
        Assert.Equal(1, callCount);

        // Set same.
        control.Visible = false;
        Assert.False(control.Visible);
        Assert.Equal(1, callCount);

        // Set different.
        control.Visible = true;
        Assert.True(control.Visible);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.VisibleChanged -= handler;
        control.Visible = false;
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void ButtonBase_Dispose_Invoke_Success()
    {
        using SubButtonBase control = new();
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.Disposing);
            Assert.Null(control.ImageList);
            Assert.Equal(callCount > 0, control.IsDisposed);
            callCount++;
        }

        control.Disposed += handler;

        try
        {
            control.Dispose();
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.Disposing);
            Assert.Null(control.ImageList);
            Assert.True(control.IsDisposed);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose();
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.Disposing);
            Assert.Null(control.ImageList);
            Assert.True(control.IsDisposed);
            Assert.Equal(2, callCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void ButtonBase_Dispose_InvokeWithImageList_Success()
    {
        using ImageList imageList = new();
        using SubButtonBase control = new()
        {
            ImageList = imageList
        };
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.Disposing);
            Assert.Same(imageList, control.ImageList);
            Assert.Equal(callCount > 0, control.IsDisposed);
            callCount++;
        }

        control.Disposed += handler;

        try
        {
            control.Dispose();
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.Disposing);
            Assert.Same(imageList, control.ImageList);
            Assert.True(control.IsDisposed);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose();
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.Disposing);
            Assert.Same(imageList, control.ImageList);
            Assert.True(control.IsDisposed);
            Assert.Equal(2, callCount);
            Assert.False(control.IsHandleCreated);

            // Dispose image list.
            imageList.Dispose();
            Assert.Same(imageList, control.ImageList);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void ButtonBase_Dispose_InvokeWithToolTip_Success()
    {
        using SubButtonBase control = new()
        {
            AutoEllipsis = true
        };
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.Disposing);
            Assert.Null(control.ImageList);
            Assert.Equal(callCount > 0, control.IsDisposed);
            callCount++;
        }

        control.Disposed += handler;

        try
        {
            control.Dispose();
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.Disposing);
            Assert.Null(control.ImageList);
            Assert.True(control.IsDisposed);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose();
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.Disposing);
            Assert.Null(control.ImageList);
            Assert.True(control.IsDisposed);
            Assert.Equal(2, callCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void ButtonBase_Dispose_InvokeDisposing_Success()
    {
        using SubButtonBase control = new();
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.Disposing);
            Assert.Null(control.ImageList);
            Assert.Equal(callCount > 0, control.IsDisposed);
            callCount++;
        }

        control.Disposed += handler;

        try
        {
            control.Dispose(true);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.Disposing);
            Assert.Null(control.ImageList);
            Assert.True(control.IsDisposed);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose(true);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.Disposing);
            Assert.Null(control.ImageList);
            Assert.True(control.IsDisposed);
            Assert.Equal(2, callCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void ButtonBase_Dispose_InvokeDisposingWithImageList_Success()
    {
        using ImageList imageList = new();
        using SubButtonBase control = new()
        {
            ImageList = imageList
        };
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.Disposing);
            Assert.Same(imageList, control.ImageList);
            Assert.Equal(callCount > 0, control.IsDisposed);
            callCount++;
        }

        control.Disposed += handler;

        try
        {
            control.Dispose(true);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.Disposing);
            Assert.Same(imageList, control.ImageList);
            Assert.True(control.IsDisposed);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose(true);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.Disposing);
            Assert.Same(imageList, control.ImageList);
            Assert.True(control.IsDisposed);
            Assert.Equal(2, callCount);
            Assert.False(control.IsHandleCreated);

            // Dispose image list.
            imageList.Dispose();
            Assert.Same(imageList, control.ImageList);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void ButtonBase_Dispose_InvokeDisposingWithToolTip_Success()
    {
        using SubButtonBase control = new()
        {
            AutoEllipsis = true
        };
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.Disposing);
            Assert.Null(control.ImageList);
            Assert.Equal(callCount > 0, control.IsDisposed);
            callCount++;
        }

        control.Disposed += handler;

        try
        {
            control.Dispose(true);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.Disposing);
            Assert.Null(control.ImageList);
            Assert.True(control.IsDisposed);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose(true);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.Disposing);
            Assert.Null(control.ImageList);
            Assert.True(control.IsDisposed);
            Assert.Equal(2, callCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void ButtonBase_Dispose_InvokeNotDisposing_Success()
    {
        using SubButtonBase control = new();
        int callCount = 0;
        void handler(object sender, EventArgs e) => callCount++;
        control.Disposed += handler;

        try
        {
            control.Dispose(false);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Null(control.ImageList);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose(false);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Null(control.ImageList);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void ButtonBase_Dispose_InvokeNotDisposingWithImageList_Success()
    {
        using ImageList imageList = new();
        using SubButtonBase control = new()
        {
            ImageList = imageList
        };
        int callCount = 0;
        void handler(object sender, EventArgs e) => callCount++;
        control.Disposed += handler;

        try
        {
            control.Dispose(false);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Same(imageList, control.ImageList);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose(false);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Same(imageList, control.ImageList);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);

            // Dispose image list.
            imageList.Dispose();
            Assert.Null(control.ImageList);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void ButtonBase_Dispose_InvokeNotDisposingWithToolTip_Success()
    {
        using SubButtonBase control = new()
        {
            AutoEllipsis = true
        };
        int callCount = 0;
        void handler(object sender, EventArgs e) => callCount++;
        control.Disposed += handler;

        try
        {
            control.Dispose(false);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Null(control.ImageList);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose(false);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Null(control.ImageList);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void ButtonBase_GetAutoSizeMode_Invoke_ReturnsExpected()
    {
        using SubButtonBase control = new();
        Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
    }

    public static IEnumerable<object[]> GetPreferredSize_TestData()
    {
        foreach (FlatStyle flatStyle in Enum.GetValues(typeof(FlatStyle)))
        {
            yield return new object[] { flatStyle, Size.Empty };
            yield return new object[] { flatStyle, new Size(-1, -2) };
            yield return new object[] { flatStyle, new Size(1, 2) };
            yield return new object[] { flatStyle, new Size(2, 1) };
            yield return new object[] { flatStyle, new Size(1, 1) };
            yield return new object[] { flatStyle, new Size(10, 20) };
            yield return new object[] { flatStyle, new Size(30, 40) };
            yield return new object[] { flatStyle, new Size(int.MaxValue, int.MaxValue) };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(GetPreferredSize_TestData))]
    public void ButtonBase_GetPreferredSize_Invoke_ThrowsNullReferenceException(FlatStyle flatStyle, Size proposedSize)
    {
        using (new NoAssertContext())
        {
            using SubButtonBase control = new()
            {
                FlatStyle = flatStyle
            };
            Assert.Throws<NullReferenceException>(() => control.GetPreferredSize(proposedSize));
            Assert.False(control.IsHandleCreated);

            // Call again.
            Assert.Throws<NullReferenceException>(() => control.GetPreferredSize(proposedSize));
            Assert.False(control.IsHandleCreated);
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(GetPreferredSize_TestData))]
    public void ButtonBase_GetPreferredSize_InvokeWithHandle_ThrowsNullReferenceException(FlatStyle flatStyle, Size proposedSize)
    {
        using (new NoAssertContext())
        {
            using SubButtonBase control = new()
            {
                FlatStyle = flatStyle
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Throws<NullReferenceException>(() => control.GetPreferredSize(proposedSize));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            Assert.Throws<NullReferenceException>(() => control.GetPreferredSize(proposedSize));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
    }

    [WinFormsTheory]
    [InlineData(ControlStyles.ContainerControl, false)]
    [InlineData(ControlStyles.UserPaint, true)]
    [InlineData(ControlStyles.Opaque, true)]
    [InlineData(ControlStyles.ResizeRedraw, true)]
    [InlineData(ControlStyles.FixedWidth, false)]
    [InlineData(ControlStyles.FixedHeight, false)]
    [InlineData(ControlStyles.StandardClick, true)]
    [InlineData(ControlStyles.Selectable, true)]
    [InlineData(ControlStyles.UserMouse, true)]
    [InlineData(ControlStyles.SupportsTransparentBackColor, true)]
    [InlineData(ControlStyles.StandardDoubleClick, true)]
    [InlineData(ControlStyles.AllPaintingInWmPaint, true)]
    [InlineData(ControlStyles.CacheText, true)]
    [InlineData(ControlStyles.EnableNotifyMessage, false)]
    [InlineData(ControlStyles.DoubleBuffer, false)]
    [InlineData(ControlStyles.OptimizedDoubleBuffer, true)]
    [InlineData(ControlStyles.UseTextForAccessibility, true)]
    [InlineData((ControlStyles)0, true)]
    [InlineData((ControlStyles)int.MaxValue, false)]
    [InlineData((ControlStyles)(-1), false)]
    public void ButtonBase_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
    {
        using SubButtonBase control = new();
        Assert.Equal(expected, control.GetStyle(flag));

        // Call again to test caching.
        Assert.Equal(expected, control.GetStyle(flag));
    }

    [WinFormsFact]
    public void ButtonBase_GetTopLevel_Invoke_ReturnsExpected()
    {
        using SubButtonBase control = new();
        Assert.False(control.GetTopLevel());
    }

    public static IEnumerable<object[]> OnEnabledChanged_TestData()
    {
        foreach (bool enabled in new bool[] { true, false })
        {
            foreach (bool visible in new bool[] { true, false })
            {
                foreach (Image image in new Image[] { null, new Bitmap(10, 10) })
                {
                    yield return new object[] { enabled, visible, image, null };
                    yield return new object[] { enabled, visible, image, new EventArgs() };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnEnabledChanged_TestData))]
    public void ButtonBase_OnEnabledChanged_Invoke_CallsEnabledChanged(bool enabled, bool visible, Image image, EventArgs eventArgs)
    {
        using SubButtonBase control = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image
        };
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
    [MemberData(nameof(OnEnabledChanged_TestData))]
    public void ButtonBase_OnEnabledChanged_InvokeDesignMode_CallsEnabledChanged(bool enabled, bool visible, Image image, EventArgs eventArgs)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Name)
            .Returns((string)null);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using SubButtonBase control = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image,
            Site = mockSite.Object
        };
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

    public static IEnumerable<object[]> OnEnabledChanged_WithHandle_TestData()
    {
        foreach (bool enabled in new bool[] { true, false })
        {
            foreach (bool visible in new bool[] { true, false })
            {
                foreach (Image image in new Image[] { null, new Bitmap(10, 10) })
                {
                    yield return new object[] { enabled, visible, image, null, enabled ? 1 : 2 };
                    yield return new object[] { enabled, visible, image, new EventArgs(), enabled ? 1 : 2 };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnEnabledChanged_WithHandle_TestData))]
    public void ButtonBase_OnEnabledChanged_InvokeWithHandle_CallsEnabledChanged(bool enabled, bool visible, Image image, EventArgs eventArgs, int expectedInvalidatedCallCount)
    {
        using SubButtonBase control = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image
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
        control.EnabledChanged += handler;
        control.OnEnabledChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.EnabledChanged -= handler;
        control.OnEnabledChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnEnabledChanged_WithHandle_TestData))]
    public void ButtonBase_OnEnabledChanged_InvokeDesignModeWithHandle_CallsEnabledChanged(bool enabled, bool visible, Image image, EventArgs eventArgs, int expectedInvalidatedCallCount)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Name)
            .Returns((string)null);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using SubButtonBase control = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image,
            Site = mockSite.Object
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
        control.EnabledChanged += handler;
        control.OnEnabledChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.EnabledChanged -= handler;
        control.OnEnabledChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ButtonBase_OnGotFocus_Invoke_CallsGotFocus(EventArgs eventArgs)
    {
        using SubButtonBase control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.GotFocus += handler;
        control.OnGotFocus(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.GotFocus -= handler;
        control.OnGotFocus(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ButtonBase_OnGotFocus_InvokeWithHandle_CallsGotFocus(EventArgs eventArgs)
    {
        using SubButtonBase control = new();
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
        control.GotFocus += handler;
        control.OnGotFocus(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.GotFocus -= handler;
        control.OnGotFocus(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ButtonBase_OnHandleCreated_Invoke_CallsHandleCreated(EventArgs eventArgs)
    {
        using SubButtonBase control = new();
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
    public void ButtonBase_OnHandleCreated_InvokeWithHandle_CallsHandleCreated(EventArgs eventArgs)
    {
        using SubButtonBase control = new();
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
    public void ButtonBase_OnHandleDestroyed_Invoke_CallsHandleDestroyed(EventArgs eventArgs)
    {
        using SubButtonBase control = new();
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
    public void ButtonBase_OnHandleDestroyed_InvokeWithHandle_CallsHandleDestroyed(EventArgs eventArgs)
    {
        using SubButtonBase control = new();
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

    public static IEnumerable<object[]> OnKeyDown_TestData()
    {
        foreach (FlatStyle flatStyle in Enum.GetValues(typeof(FlatStyle)))
        {
            bool expectedIsHandleCreated = flatStyle == FlatStyle.System;
            foreach (bool enabled in new bool[] { true, false })
            {
                yield return new object[] { flatStyle, enabled, new KeyEventArgs(Keys.Cancel), false, false };
                yield return new object[] { flatStyle, enabled, new KeyEventArgs(Keys.Enter), false, false };
                yield return new object[] { flatStyle, enabled, new KeyEventArgs(Keys.Space), true, expectedIsHandleCreated };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnKeyDown_TestData))]
    public void ButtonBase_OnKeyDown_Invoke_CallsKeyDown(FlatStyle flatStyle, bool enabled, KeyEventArgs eventArgs, bool expectedHandled, bool expectedIsHandleCreated)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle,
            Enabled = enabled
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
        control.OnKeyDown(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(expectedHandled, eventArgs.Handled);
        Assert.Equal(expectedIsHandleCreated, control.IsHandleCreated);

        // Remove handler.
        control.KeyDown -= handler;
        control.OnKeyDown(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(expectedHandled, eventArgs.Handled);
        Assert.Equal(expectedIsHandleCreated, control.IsHandleCreated);
    }

    public static IEnumerable<object[]> OnKeyDown_MouseDown_TestData()
    {
        foreach (FlatStyle flatStyle in Enum.GetValues(typeof(FlatStyle)))
        {
            foreach (bool enabled in new bool[] { true, false })
            {
                yield return new object[] { flatStyle, enabled, new KeyEventArgs(Keys.Cancel), false };
                yield return new object[] { flatStyle, enabled, new KeyEventArgs(Keys.Enter), false };
                yield return new object[] { flatStyle, enabled, new KeyEventArgs(Keys.Space), true };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnKeyDown_MouseDown_TestData))]
    public void ButtonBase_OnKeyDown_InvokeMouseDown_CallsKeyDown(FlatStyle flatStyle, bool enabled, KeyEventArgs eventArgs, bool expectedHandled)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle,
            Enabled = enabled
        };
        control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
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
        Assert.Equal(expectedHandled, eventArgs.Handled);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.KeyDown -= handler;
        control.OnKeyDown(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(expectedHandled, eventArgs.Handled);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> OnKeyDown_WithHandle_TestData()
    {
        foreach (FlatStyle flatStyle in Enum.GetValues(typeof(FlatStyle)))
        {
            foreach (bool enabled in new bool[] { true, false })
            {
                yield return new object[] { flatStyle, enabled, new KeyEventArgs(Keys.Cancel), false, 0 };
                yield return new object[] { flatStyle, enabled, new KeyEventArgs(Keys.Enter), false, 0 };
                yield return new object[] { flatStyle, enabled, new KeyEventArgs(Keys.Space), true, 1 };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnKeyDown_WithHandle_TestData))]
    public void ButtonBase_OnKeyDown_InvokeWithHandle_CallsKeyDown(FlatStyle flatStyle, bool enabled, KeyEventArgs eventArgs, bool expectedHandled, int expectedInvalidatedCallCount)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle,
            Enabled = enabled
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
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
        Assert.Equal(expectedHandled, eventArgs.Handled);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.KeyDown -= handler;
        control.OnKeyDown(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(expectedHandled, eventArgs.Handled);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> OnKeyDown_MouseDownWithHandle_TestData()
    {
        foreach (FlatStyle flatStyle in Enum.GetValues(typeof(FlatStyle)))
        {
            foreach (bool enabled in new bool[] { true, false })
            {
                yield return new object[] { flatStyle, enabled, new KeyEventArgs(Keys.Cancel), false };
                yield return new object[] { flatStyle, enabled, new KeyEventArgs(Keys.Enter), false };
                yield return new object[] { flatStyle, enabled, new KeyEventArgs(Keys.Space), true };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnKeyDown_MouseDownWithHandle_TestData))]
    public void ButtonBase_OnKeyDown_InvokeMouseDownWithHandle_CallsKeyDown(FlatStyle flatStyle, bool enabled, KeyEventArgs eventArgs, bool expectedHandled)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle,
            Enabled = enabled
        };
        control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
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
        Assert.Equal(expectedHandled, eventArgs.Handled);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.KeyDown -= handler;
        control.OnKeyDown(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(expectedHandled, eventArgs.Handled);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(FlatStyle.Flat, Keys.Cancel, 0)]
    [InlineData(FlatStyle.Flat, Keys.Enter, 0)]
    [InlineData(FlatStyle.Flat, Keys.Space, 0)]
    [InlineData(FlatStyle.Popup, Keys.Cancel, 0)]
    [InlineData(FlatStyle.Popup, Keys.Enter, 0)]
    [InlineData(FlatStyle.Popup, Keys.Space, 0)]
    [InlineData(FlatStyle.Standard, Keys.Cancel, 0)]
    [InlineData(FlatStyle.Standard, Keys.Enter, 0)]
    [InlineData(FlatStyle.Standard, Keys.Space, 0)]
    [InlineData(FlatStyle.System, Keys.Cancel, 0)]
    [InlineData(FlatStyle.System, Keys.Enter, 0)]
    [InlineData(FlatStyle.System, Keys.Space, 0)]
    public void ButtonBase_OnKeyDown_GetState_ReturnsExpected(FlatStyle flatStyle, Keys key, int expected)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.OnKeyDown(new KeyEventArgs(key));
        Assert.Equal(expected, (int)PInvokeCore.SendMessage(control, PInvoke.BM_GETSTATE));
    }

    [WinFormsTheory]
    [InlineData(FlatStyle.Flat, Keys.Cancel, 0)]
    [InlineData(FlatStyle.Flat, Keys.Enter, 0)]
    [InlineData(FlatStyle.Flat, Keys.Space, 0)]
    [InlineData(FlatStyle.Popup, Keys.Cancel, 0)]
    [InlineData(FlatStyle.Popup, Keys.Enter, 0)]
    [InlineData(FlatStyle.Popup, Keys.Space, 0)]
    [InlineData(FlatStyle.Standard, Keys.Cancel, 0)]
    [InlineData(FlatStyle.Standard, Keys.Enter, 0)]
    [InlineData(FlatStyle.Standard, Keys.Space, 0)]
    [InlineData(FlatStyle.System, Keys.Cancel, 0)]
    [InlineData(FlatStyle.System, Keys.Enter, 0)]
    [InlineData(FlatStyle.System, Keys.Space, 0)]
    public void ButtonBase_OnKeyDown_MouseDownGetState_ReturnsExpected(FlatStyle flatStyle, Keys key, int expected)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        control.OnKeyDown(new KeyEventArgs(key));
        Assert.Equal(expected, (int)PInvokeCore.SendMessage(control, PInvoke.BM_GETSTATE));
    }

    [WinFormsFact]
    public void ButtonBase_OnKeyDown_NullE_ThrowsNullReferenceException()
    {
        using SubButtonBase control = new();
        Assert.Throws<NullReferenceException>(() => control.OnKeyDown(null));
    }

    public static IEnumerable<object[]> OnKeyUp_TestData()
    {
        foreach (FlatStyle flatStyle in Enum.GetValues(typeof(FlatStyle)))
        {
            foreach (bool enabled in new bool[] { true, false })
            {
                yield return new object[] { flatStyle, enabled, new KeyEventArgs(Keys.None) };
                yield return new object[] { flatStyle, enabled, new KeyEventArgs(Keys.Cancel) };
                yield return new object[] { flatStyle, enabled, new KeyEventArgs(Keys.Enter) };
                yield return new object[] { flatStyle, enabled, new KeyEventArgs(Keys.Space) };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnKeyUp_TestData))]
    public void ButtonBase_OnKeyUp_Invoke_CallsKeyUp(FlatStyle flatStyle, bool enabled, KeyEventArgs eventArgs)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle,
            Enabled = enabled
        };
        int clickCallCount = 0;
        control.Click += (sender, e) => clickCallCount++;
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
        Assert.Equal(0, clickCallCount);
        Assert.False(eventArgs?.Handled ?? false);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.KeyUp -= handler;
        control.OnKeyUp(eventArgs);
        Assert.Equal(0, clickCallCount);
        Assert.False(eventArgs?.Handled ?? false);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> OnKeyUp_MouseDown_TestData()
    {
        foreach (FlatStyle flatStyle in Enum.GetValues(typeof(FlatStyle)))
        {
            foreach (bool enabled in new bool[] { true, false })
            {
                bool expectedIsHandleCreated = flatStyle == FlatStyle.System;
                yield return new object[] { flatStyle, enabled, new KeyEventArgs(Keys.Cancel), 0, expectedIsHandleCreated };
                yield return new object[] { flatStyle, enabled, new KeyEventArgs(Keys.Enter), 1, expectedIsHandleCreated };
                yield return new object[] { flatStyle, enabled, new KeyEventArgs(Keys.Space), 1, expectedIsHandleCreated };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnKeyUp_MouseDown_TestData))]
    public void ButtonBase_OnKeyUp_InvokeMouseDown_CallsKeyUp(FlatStyle flatStyle, bool enabled, KeyEventArgs eventArgs, int expectedClickCallCount, bool expectedIsHandleCreated)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle,
            Enabled = enabled
        };
        control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        int clickCallCount = 0;
        control.Click += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            clickCallCount++;
        };
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
        Assert.Equal(expectedClickCallCount, clickCallCount);
        Assert.True(eventArgs.Handled);
        Assert.Equal(expectedIsHandleCreated, control.IsHandleCreated);

        // Remove handler.
        control.KeyUp -= handler;
        control.OnKeyUp(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(expectedClickCallCount, clickCallCount);
        Assert.True(eventArgs.Handled);
        Assert.Equal(expectedIsHandleCreated, control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnKeyUp_TestData))]
    public void ButtonBase_OnKeyUp_InvokeWithHandle_CallsKeyUp(FlatStyle flatStyle, bool enabled, KeyEventArgs eventArgs)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle,
            Enabled = enabled
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int clickCallCount = 0;
        control.Click += (sender, e) => clickCallCount++;
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
        Assert.Equal(0, clickCallCount);
        Assert.False(eventArgs?.Handled ?? false);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.KeyUp -= handler;
        control.OnKeyUp(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, clickCallCount);
        Assert.False(eventArgs?.Handled ?? false);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> OnKeyUp_MouseDownWithHandle_TestData()
    {
        foreach (FlatStyle flatStyle in Enum.GetValues(typeof(FlatStyle)))
        {
            foreach (bool enabled in new bool[] { true, false })
            {
                int expectedCallCount = flatStyle == FlatStyle.System ? 0 : 1;
                yield return new object[] { flatStyle, enabled, new KeyEventArgs(Keys.Cancel), 0, expectedCallCount };
                yield return new object[] { flatStyle, enabled, new KeyEventArgs(Keys.Enter), 1, expectedCallCount };
                yield return new object[] { flatStyle, enabled, new KeyEventArgs(Keys.Space), 1, expectedCallCount };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnKeyUp_MouseDownWithHandle_TestData))]
    public void ButtonBase_OnKeyUp_InvokeMouseDownWithHandle_CallsKeyUp(FlatStyle flatStyle, bool enabled, KeyEventArgs eventArgs, int expectedClickCallCount, int expectedInvalidatedCallCount)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle,
            Enabled = enabled
        };
        control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int clickCallCount = 0;
        control.Click += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            clickCallCount++;
        };
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
        Assert.Equal(expectedClickCallCount, clickCallCount);
        Assert.True(eventArgs.Handled);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.KeyUp -= handler;
        control.OnKeyUp(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(expectedClickCallCount, clickCallCount);
        Assert.True(eventArgs.Handled);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(FlatStyle.Flat, Keys.Cancel, 0)]
    [InlineData(FlatStyle.Flat, Keys.Enter, 0)]
    [InlineData(FlatStyle.Flat, Keys.Space, 0)]
    [InlineData(FlatStyle.Popup, Keys.Cancel, 0)]
    [InlineData(FlatStyle.Popup, Keys.Enter, 0)]
    [InlineData(FlatStyle.Popup, Keys.Space, 0)]
    [InlineData(FlatStyle.Standard, Keys.Cancel, 0)]
    [InlineData(FlatStyle.Standard, Keys.Enter, 0)]
    [InlineData(FlatStyle.Standard, Keys.Space, 0)]
    [InlineData(FlatStyle.System, Keys.Cancel, 0)]
    [InlineData(FlatStyle.System, Keys.Enter, 0)]
    [InlineData(FlatStyle.System, Keys.Space, 0)]
    public void ButtonBase_OnKeyUp_GetState_ReturnsExpected(FlatStyle flatStyle, Keys key, int expected)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.OnKeyUp(new KeyEventArgs(key));
        Assert.Equal(expected, (int)PInvokeCore.SendMessage(control, PInvoke.BM_GETSTATE));
    }

    [WinFormsTheory]
    [InlineData(FlatStyle.Flat, Keys.Cancel, 0)]
    [InlineData(FlatStyle.Flat, Keys.Enter, 0)]
    [InlineData(FlatStyle.Flat, Keys.Space, 0)]
    [InlineData(FlatStyle.Popup, Keys.Cancel, 0)]
    [InlineData(FlatStyle.Popup, Keys.Enter, 0)]
    [InlineData(FlatStyle.Popup, Keys.Space, 0)]
    [InlineData(FlatStyle.Standard, Keys.Cancel, 0)]
    [InlineData(FlatStyle.Standard, Keys.Enter, 0)]
    [InlineData(FlatStyle.Standard, Keys.Space, 0)]
    [InlineData(FlatStyle.System, Keys.Cancel, 0)]
    [InlineData(FlatStyle.System, Keys.Enter, 0)]
    [InlineData(FlatStyle.System, Keys.Space, 0)]
    public void ButtonBase_OnKeyUp_MouseDownGetState_ReturnsExpected(FlatStyle flatStyle, Keys key, int expected)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        control.OnKeyUp(new KeyEventArgs(key));
        Assert.Equal(expected, (int)PInvokeCore.SendMessage(control, PInvoke.BM_GETSTATE));
    }

    [WinFormsFact]
    public void ButtonBase_OnKeyUp_NullE_ThrowsNullReferenceException()
    {
        using SubButtonBase control = new();
        control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        Assert.Throws<NullReferenceException>(() => control.OnKeyUp(null));
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ButtonBase_OnLostFocus_Invoke_CallsLostFocus(EventArgs eventArgs)
    {
        using SubButtonBase control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.LostFocus += handler;
        control.OnLostFocus(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.Capture);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.LostFocus -= handler;
        control.OnLostFocus(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.Capture);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ButtonBase_OnLostFocus_InvokeWithHandle_CallsLostFocus(EventArgs eventArgs)
    {
        using SubButtonBase control = new();
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
        control.LostFocus += handler;
        control.OnLostFocus(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.Capture);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.LostFocus -= handler;
        control.OnLostFocus(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.Capture);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> OnMouseDown_TestData()
    {
        foreach (FlatStyle flatStyle in Enum.GetValues(typeof(FlatStyle)))
        {
            foreach (bool enabled in new bool[] { true, false })
            {
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0) };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0) };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 0, 0, 0, 0) };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0) };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 1, 0, 0, 0) };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0) };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 2, 0, 0, 0) };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Left, 3, 0, 0, 0) };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 3, 0, 0, 0) };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnMouseDown_TestData))]
    public void ButtonBase_OnMouseDown_Invoke_CallsMouseDown(FlatStyle flatStyle, bool enabled, MouseEventArgs eventArgs)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle,
            Enabled = enabled
        };
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
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.MouseDown -= handler;
        control.OnMouseDown(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> OnMouseDown_WithHandle_TestData()
    {
        foreach (FlatStyle flatStyle in Enum.GetValues(typeof(FlatStyle)))
        {
            foreach (bool enabled in new bool[] { true, false })
            {
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0), 0 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0), 1 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 0, 0, 0, 0), 0 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0), 1 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 1, 0, 0, 0), 0 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0), 1 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 2, 0, 0, 0), 0 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Left, 3, 0, 0, 0), 1 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 3, 0, 0, 0), 0 };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnMouseDown_WithHandle_TestData))]
    public void ButtonBase_OnMouseDown_InvokeWithHandle_CallsMouseDown(FlatStyle flatStyle, bool enabled, MouseEventArgs eventArgs, int expectedInvalidatedCallCount)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle,
            Enabled = enabled
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
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
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.MouseDown -= handler;
        control.OnMouseDown(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ButtonBase_OnMouseDown_NullE_ThrowsNullReferenceException()
    {
        using SubButtonBase control = new();
        Assert.Throws<NullReferenceException>(() => control.OnMouseDown(null));
    }

    public static IEnumerable<object[]> OnMouseEnter_TestData()
    {
        foreach (FlatStyle flatStyle in Enum.GetValues(typeof(FlatStyle)))
        {
            foreach (bool enabled in new bool[] { true, false })
            {
                foreach (bool autoEllipsis in new bool[] { true, false })
                {
                    foreach (string text in new string[] { null, string.Empty, "text" })
                    {
                        yield return new object[] { flatStyle, enabled, autoEllipsis, text, null };
                        yield return new object[] { flatStyle, enabled, autoEllipsis, text, new EventArgs() };
                    }
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnMouseEnter_TestData))]
    public void ButtonBase_OnMouseEnter_Invoke_CallsMouseEnter(FlatStyle flatStyle, bool enabled, bool autoEllipsis, string text, EventArgs eventArgs)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle,
            Enabled = enabled,
            AutoEllipsis = autoEllipsis,
            Text = text
        };
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
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.MouseEnter -= handler;
        control.OnMouseEnter(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnMouseEnter_TestData))]
    public void ButtonBase_OnMouseEnter_InvokeDesignMode_CallsMouseEnter(FlatStyle flatStyle, bool enabled, bool autoEllipsis, string text, EventArgs eventArgs)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Name)
            .Returns((string)null);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle,
            Enabled = enabled,
            AutoEllipsis = autoEllipsis,
            Text = text,
            Site = mockSite.Object
        };
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
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.MouseEnter -= handler;
        control.OnMouseEnter(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnMouseEnter_TestData))]
    public void ButtonBase_OnMouseEnter_InvokeWithHandle_CallsMouseEnter(FlatStyle flatStyle, bool enabled, bool autoEllipsis, string text, EventArgs eventArgs)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle,
            Enabled = enabled,
            AutoEllipsis = autoEllipsis,
            Text = text
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
        control.MouseEnter += handler;
        control.OnMouseEnter(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.MouseEnter -= handler;
        control.OnMouseEnter(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnMouseEnter_TestData))]
    public void ButtonBase_OnMouseEnter_InvokeDesignModeWithHandle_CallsMouseEnter(FlatStyle flatStyle, bool enabled, bool autoEllipsis, string text, EventArgs eventArgs)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Name)
            .Returns((string)null);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle,
            Enabled = enabled,
            AutoEllipsis = autoEllipsis,
            Text = text,
            Site = mockSite.Object
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
        control.MouseEnter += handler;
        control.OnMouseEnter(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.MouseEnter -= handler;
        control.OnMouseEnter(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> OnMouseLeave_TestData()
    {
        foreach (FlatStyle flatStyle in Enum.GetValues(typeof(FlatStyle)))
        {
            foreach (bool enabled in new bool[] { true, false })
            {
                foreach (bool autoEllipsis in new bool[] { true, false })
                {
                    foreach (string text in new string[] { null, string.Empty, "text" })
                    {
                        yield return new object[] { flatStyle, enabled, autoEllipsis, text, null };
                        yield return new object[] { flatStyle, enabled, autoEllipsis, text, new EventArgs() };
                    }
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnMouseLeave_TestData))]
    public void ButtonBase_OnMouseLeave_Invoke_CallsMouseLeave(FlatStyle flatStyle, bool enabled, bool autoEllipsis, string text, EventArgs eventArgs)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle,
            Enabled = enabled,
            AutoEllipsis = autoEllipsis,
            Text = text
        };
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
        Assert.Equal(autoEllipsis, control.IsHandleCreated);

        // Remove handler.
        control.MouseLeave -= handler;
        control.OnMouseLeave(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(autoEllipsis, control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnMouseLeave_TestData))]
    public void ButtonBase_OnMouseLeave_InvokeWithHandle_CallsMouseLeave(FlatStyle flatStyle, bool enabled, bool autoEllipsis, string text, EventArgs eventArgs)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle,
            Enabled = enabled,
            AutoEllipsis = autoEllipsis,
            Text = text
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
        control.MouseLeave += handler;
        control.OnMouseLeave(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.MouseLeave -= handler;
        control.OnMouseLeave(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> OnMouseMove_TestData()
    {
        foreach (FlatStyle flatStyle in Enum.GetValues(typeof(FlatStyle)))
        {
            foreach (bool enabled in new bool[] { true, false })
            {
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0) };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0) };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 0, 0, 0, 0) };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0) };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 1, 0, 0, 0) };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0) };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 2, 0, 0, 0) };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Left, 3, 0, 0, 0) };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 3, 0, 0, 0) };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 3, -1, 0, 0) };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 3, 0, -1, 0) };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnMouseMove_TestData))]
    public void ButtonBase_OnMouseMove_Invoke_CallsMouseMove(FlatStyle flatStyle, bool enabled, MouseEventArgs eventArgs)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle,
            Enabled = enabled
        };
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
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.MouseMove -= handler;
        control.OnMouseMove(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnMouseMove_TestData))]
    public void ButtonBase_OnMouseMove_InvokeMousePressed_CallsMouseMove(FlatStyle flatStyle, bool enabled, MouseEventArgs eventArgs)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle,
            Enabled = enabled
        };
        control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
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
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.MouseMove -= handler;
        control.OnMouseMove(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnMouseMove_TestData))]
    public void ButtonBase_OnMouseMove_InvokeMousePressedLeave_CallsMouseMove(FlatStyle flatStyle, bool enabled, MouseEventArgs eventArgs)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle,
            Enabled = enabled
        };
        control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        control.OnLostFocus(new EventArgs());
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
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.MouseMove -= handler;
        control.OnMouseMove(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> OnMouseMove_WithHandle_TestData()
    {
        foreach (FlatStyle flatStyle in Enum.GetValues(typeof(FlatStyle)))
        {
            foreach (bool enabled in new bool[] { true, false })
            {
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0), 0 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0), 0 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 0, 0, 0, 0), 0 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0), 0 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 1, 0, 0, 0), 0 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0), 0 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 2, 0, 0, 0), 0 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Left, 3, 0, 0, 0), 0 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 3, 0, 0, 0), 0 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 3, -1, 0, 0), 0 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 3, 0, -1, 0), 0 };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnMouseMove_WithHandle_TestData))]
    public void ButtonBase_OnMouseMove_InvokeWithHandle_CallsMouseMove(FlatStyle flatStyle, bool enabled, MouseEventArgs eventArgs, int expectedInvalidatedCallCount)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle,
            Enabled = enabled
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
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
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.MouseMove -= handler;
        control.OnMouseMove(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> OnMouseMove_MousePressedWithHandle_TestData()
    {
        foreach (FlatStyle flatStyle in Enum.GetValues(typeof(FlatStyle)))
        {
            foreach (bool enabled in new bool[] { true, false })
            {
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0), 0 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0), 0 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 0, 0, 0, 0), 0 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0), 0 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 1, 0, 0, 0), 0 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0), 0 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 2, 0, 0, 0), 0 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Left, 3, 0, 0, 0), 0 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 3, 0, 0, 0), 0 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 3, -1, 0, 0), 1 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 3, 0, -1, 0), 1 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 3, -1, -1, 0), 1 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 3, int.MaxValue, int.MaxValue, 0), 1 };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnMouseMove_MousePressedWithHandle_TestData))]
    public void ButtonBase_OnMouseMove_InvokeMousePressedWithHandle_CallsMouseMove(FlatStyle flatStyle, bool enabled, MouseEventArgs eventArgs, int expectedInvalidatedCallCount)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle,
            Enabled = enabled
        };
        control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
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
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.MouseMove -= handler;
        control.OnMouseMove(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> OnMouseMove_MousePressedLeaveWithHandle_TestData()
    {
        foreach (FlatStyle flatStyle in Enum.GetValues(typeof(FlatStyle)))
        {
            foreach (bool enabled in new bool[] { true, false })
            {
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0), 0 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0), 1 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 0, 0, 0, 0), 1 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0), 1 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 1, 0, 0, 0), 1 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0), 1 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 2, 0, 0, 0), 1 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Left, 3, 0, 0, 0), 1 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 3, 0, 0, 0), 1 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 3, -1, 0, 0), 0 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 3, 0, -1, 0), 0 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 3, -1, -1, 0), 0 };
                yield return new object[] { flatStyle, enabled, new MouseEventArgs(MouseButtons.Right, 3, int.MaxValue, int.MaxValue, 0), 0 };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnMouseMove_MousePressedLeaveWithHandle_TestData))]
    public void ButtonBase_OnMouseMove_InvokeMousePressedLeaveWithHandle_CallsMouseMove(FlatStyle flatStyle, bool enabled, MouseEventArgs eventArgs, int expectedInvalidatedCallCount)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle,
            Enabled = enabled
        };
        control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        control.OnLostFocus(new EventArgs());
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
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
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.MouseMove -= handler;
        control.OnMouseMove(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ButtonBase_OnMouseMove_NullE_ThrowsNullReferenceException()
    {
        using SubButtonBase control = new();
        Assert.Throws<NullReferenceException>(() => control.OnMouseMove(null));
    }

    public static IEnumerable<object[]> OnMouseUp_TestData()
    {
        foreach (FlatStyle flatStyle in Enum.GetValues(typeof(FlatStyle)))
        {
            yield return new object[] { flatStyle, null };
            yield return new object[] { flatStyle, new MouseEventArgs(MouseButtons.None, 1, 2, 3, 4) };
            yield return new object[] { flatStyle, new MouseEventArgs(MouseButtons.Left, 1, 2, 3, 4) };
            yield return new object[] { flatStyle, new MouseEventArgs(MouseButtons.Middle, 1, 2, 3, 4) };
            yield return new object[] { flatStyle, new MouseEventArgs(MouseButtons.Right, 1, 2, 3, 4) };
            yield return new object[] { flatStyle, new HandledMouseEventArgs(MouseButtons.None, 1, 2, 3, 4) };
            yield return new object[] { flatStyle, new HandledMouseEventArgs(MouseButtons.Left, 1, 2, 3, 4) };
            yield return new object[] { flatStyle, new HandledMouseEventArgs(MouseButtons.Middle, 1, 2, 3, 4) };
            yield return new object[] { flatStyle, new HandledMouseEventArgs(MouseButtons.Right, 1, 2, 3, 4) };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnMouseUp_TestData))]
    public void ButtonBase_OnMouseUp_Invoke_CallsMouseUp(FlatStyle flatStyle, MouseEventArgs eventArgs)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle
        };
        int callCount = 0;
        MouseEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };
        int clickCallCount = 0;
        control.Click += (sender, e) => clickCallCount++;
        int mouseClickCallCount = 0;
        control.MouseClick += (sender, e) => mouseClickCallCount++;

        // Call with handler.
        control.MouseUp += handler;
        control.OnMouseUp(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, clickCallCount);
        Assert.Equal(0, mouseClickCallCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.MouseUp -= handler;
        control.OnMouseUp(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, clickCallCount);
        Assert.Equal(0, mouseClickCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnMouseUp_TestData))]
    public void ButtonBase_OnMouseUp_InvokeWithHandle_CallsMouseUp(FlatStyle flatStyle, MouseEventArgs eventArgs)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int callCount = 0;
        MouseEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };
        int clickCallCount = 0;
        control.Click += (sender, e) => clickCallCount++;
        int mouseClickCallCount = 0;
        control.MouseClick += (sender, e) => mouseClickCallCount++;

        // Call with handler.
        control.MouseUp += handler;
        control.OnMouseUp(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, clickCallCount);
        Assert.Equal(0, mouseClickCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.MouseUp -= handler;
        control.OnMouseUp(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, clickCallCount);
        Assert.Equal(0, mouseClickCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> OnParentChanged_TestData()
    {
        foreach (bool enabled in new bool[] { true, false })
        {
            foreach (bool visible in new bool[] { true, false })
            {
                foreach (Image image in new Image[] { null, new Bitmap(10, 10) })
                {
                    yield return new object[] { enabled, visible, image, null };
                    yield return new object[] { enabled, visible, image, new EventArgs() };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnParentChanged_TestData))]
    public void ButtonBase_OnParentChanged_Invoke_CallsParentChanged(bool enabled, bool visible, Image image, EventArgs eventArgs)
    {
        using SubButtonBase control = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.ParentChanged += handler;
        control.OnParentChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.ParentChanged -= handler;
        control.OnParentChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnParentChanged_TestData))]
    public void ButtonBase_OnParentChanged_InvokeDesignMode_CallsParentChanged(bool enabled, bool visible, Image image, EventArgs eventArgs)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Name)
            .Returns((string)null);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using SubButtonBase control = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image,
            Site = mockSite.Object
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.ParentChanged += handler;
        control.OnParentChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.ParentChanged -= handler;
        control.OnParentChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> OnParentChanged_WithHandle_TestData()
    {
        foreach (bool enabled in new bool[] { true, false })
        {
            foreach (bool visible in new bool[] { true, false })
            {
                foreach (Image image in new Image[] { null, new Bitmap(10, 10) })
                {
                    yield return new object[] { enabled, visible, image, null };
                    yield return new object[] { enabled, visible, image, new EventArgs() };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnParentChanged_WithHandle_TestData))]
    public void ButtonBase_OnParentChanged_InvokeWithHandle_CallsParentChanged(bool enabled, bool visible, Image image, EventArgs eventArgs)
    {
        using SubButtonBase control = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image
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
        control.ParentChanged += handler;
        control.OnParentChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.ParentChanged -= handler;
        control.OnParentChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnParentChanged_WithHandle_TestData))]
    public void ButtonBase_OnParentChanged_InvokeDesignModeWithHandle_CallsParentChanged(bool enabled, bool visible, Image image, EventArgs eventArgs)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Name)
            .Returns((string)null);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using SubButtonBase control = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image,
            Site = mockSite.Object
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
        control.ParentChanged += handler;
        control.OnParentChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.ParentChanged -= handler;
        control.OnParentChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> OnTextChanged_TestData()
    {
        foreach (bool autoSize in new bool[] { true, false })
        {
            yield return new object[] { autoSize, null };
            yield return new object[] { autoSize, new EventArgs() };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnTextChanged_TestData))]
    public void ButtonBase_OnTextChanged_Invoke_CallsTextChanged(bool autoSize, EventArgs eventArgs)
    {
        using SubButtonBase control = new()
        {
            AutoSize = autoSize
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.TextChanged += handler;
        control.OnTextChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.TextChanged -= handler;
        control.OnTextChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> OnTextChanged_WithParent_TestData()
    {
        yield return new object[] { true, null, 1 };
        yield return new object[] { true, new EventArgs(), 1 };
        yield return new object[] { false, null, 0 };
        yield return new object[] { false, new EventArgs(), 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnTextChanged_WithParent_TestData))]
    public void ButtonBase_OnTextChanged_InvokeWithParent_CallsTextChanged(bool autoSize, EventArgs eventArgs, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using SubButton control = new()
        {
            AutoSize = autoSize,
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Text", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.TextChanged += handler;
            control.OnTextChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Remove handler.
            control.TextChanged -= handler;
            control.OnTextChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(expectedParentLayoutCallCount * 2, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnTextChanged_TestData))]
    public void ButtonBase_OnTextChanged_InvokeWithHandle_CallsTextChanged(bool autoSize, EventArgs eventArgs)
    {
        using SubButtonBase control = new()
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
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.TextChanged += handler;
        control.OnTextChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.TextChanged -= handler;
        control.OnTextChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnTextChanged_WithParent_TestData))]
    public void ButtonBase_OnTextChanged_InvokeWithParentWithHandle_CallsTextChanged(bool autoSize, EventArgs eventArgs, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using SubButton control = new()
        {
            AutoSize = autoSize,
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Text", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.TextChanged += handler;
            control.OnTextChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);

            // Remove handler.
            control.TextChanged -= handler;
            control.OnTextChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(expectedParentLayoutCallCount * 2, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> OnVisibleChanged_TestData()
    {
        foreach (bool enabled in new bool[] { true, false })
        {
            foreach (bool visible in new bool[] { true, false })
            {
                foreach (Image image in new Image[] { null, new Bitmap(10, 10) })
                {
                    yield return new object[] { enabled, visible, image, null };
                    yield return new object[] { enabled, visible, image, new EventArgs() };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnVisibleChanged_TestData))]
    public void ButtonBase_OnVisibleChanged_Invoke_CallsVisibleChanged(bool enabled, bool visible, Image image, EventArgs eventArgs)
    {
        using SubButtonBase control = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.VisibleChanged += handler;
        control.OnVisibleChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.VisibleChanged -= handler;
        control.OnVisibleChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnVisibleChanged_TestData))]
    public void ButtonBase_OnVisibleChanged_InvokeDesignMode_CallsVisibleChanged(bool enabled, bool visible, Image image, EventArgs eventArgs)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Name)
            .Returns((string)null);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using SubButtonBase control = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image,
            Site = mockSite.Object
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.VisibleChanged += handler;
        control.OnVisibleChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.VisibleChanged -= handler;
        control.OnVisibleChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> OnVisibleChanged_WithHandle_TestData()
    {
        foreach (bool enabled in new bool[] { true, false })
        {
            foreach (bool visible in new bool[] { true, false })
            {
                foreach (Image image in new Image[] { null, new Bitmap(10, 10) })
                {
                    yield return new object[] { enabled, visible, image, null };
                    yield return new object[] { enabled, visible, image, new EventArgs() };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnVisibleChanged_WithHandle_TestData))]
    public void ButtonBase_OnVisibleChanged_InvokeWithHandle_CallsVisibleChanged(bool enabled, bool visible, Image image, EventArgs eventArgs)
    {
        using SubButtonBase control = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image
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
        control.VisibleChanged += handler;
        control.OnVisibleChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.VisibleChanged -= handler;
        control.OnVisibleChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnVisibleChanged_WithHandle_TestData))]
    public void ButtonBase_OnVisibleChanged_InvokeDesignModeWithHandle_CallsVisibleChanged(bool enabled, bool visible, Image image, EventArgs eventArgs)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Name)
            .Returns((string)null);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using SubButtonBase control = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image,
            Site = mockSite.Object
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
        control.VisibleChanged += handler;
        control.OnVisibleChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.VisibleChanged -= handler;
        control.OnVisibleChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ButtonBase_ResetFlagsandPaint_Invoke_Success()
    {
        using SubButtonBase control = new();
        control.ResetFlagsandPaint();
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.ResetFlagsandPaint();
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ButtonBase_ResetFlagsandPaint_InvokeWithHandle_Success()
    {
        using SubButtonBase control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ResetFlagsandPaint();
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.ResetFlagsandPaint();
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [EnumData<FlatStyle>]
    public void ButtonBase_WndProc_InvokeCancelModeWithoutHandle_Success(FlatStyle flatStyle)
    {
        using (new NoAssertContext())
        {
            using SubButtonBase control = new()
            {
                FlatStyle = flatStyle
            };
            int callCount = 0;
            control.LostFocus += (sender, e) => callCount++;
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_CANCELMODE,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);
        }
    }

    [WinFormsTheory]
    [EnumData<FlatStyle>]
    public void ButtonBase_WndProc_InvokeCancelModeMousePressedWithoutHandle_Success(FlatStyle flatStyle)
    {
        using (new NoAssertContext())
        {
            using SubButtonBase control = new()
            {
                FlatStyle = flatStyle
            };
            control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
            int callCount = 0;
            control.LostFocus += (sender, e) => callCount++;
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_CANCELMODE,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);
        }
    }

    [WinFormsTheory]
    [EnumData<FlatStyle>]
    public void ButtonBase_WndProc_InvokeCancelModeMousePressedLostFocusWithoutHandle_Success(FlatStyle flatStyle)
    {
        using (new NoAssertContext())
        {
            using SubButtonBase control = new()
            {
                FlatStyle = flatStyle
            };
            control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
            control.OnLostFocus(new EventArgs());
            int callCount = 0;
            control.LostFocus += (sender, e) => callCount++;
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_CANCELMODE,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);
        }
    }

    [WinFormsTheory]
    [EnumData<FlatStyle>]
    public void ButtonBase_WndProc_InvokeCancelModeMousePressedInButtonUpWithoutHandle_Success(FlatStyle flatStyle)
    {
        using (new NoAssertContext())
        {
            using SubButtonBase control = new()
            {
                FlatStyle = flatStyle
            };
            control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));

            Message buttonM = new()
            {
                Msg = (int)PInvokeCore.WM_LBUTTONUP
            };
            int mouseUpCallCount = 0;
            control.MouseUp += (sender, e) =>
            {
                int callCount = 0;
                control.LostFocus += (sender, e) => callCount++;
                Message m = new()
                {
                    Msg = (int)PInvokeCore.WM_CANCELMODE,
                    Result = 250
                };
                control.WndProc(ref m);
                Assert.Equal(IntPtr.Zero, m.Result);
                Assert.Equal(0, callCount);
                Assert.True(control.IsHandleCreated);

                // Call again.
                control.WndProc(ref m);
                Assert.Equal(IntPtr.Zero, m.Result);
                Assert.Equal(0, callCount);
                Assert.True(control.IsHandleCreated);

                mouseUpCallCount++;
            };
            control.WndProc(ref buttonM);
            Assert.Equal(1, mouseUpCallCount);
        }
    }

    [WinFormsTheory]
    [EnumData<FlatStyle>]
    public void ButtonBase_WndProc_InvokeCancelModeWithHandle_Success(FlatStyle flatStyle)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle
        };
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
            Msg = (int)PInvokeCore.WM_CANCELMODE,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(0, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(0, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(FlatStyle.Flat, 1)]
    [InlineData(FlatStyle.Popup, 1)]
    [InlineData(FlatStyle.Standard, 1)]
    [InlineData(FlatStyle.System, 0)]
    public void ButtonBase_WndProc_InvokeCancelModeMousePressedWithHandle_Success(FlatStyle flatStyle, int expectedInvalidatedCallCount)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle
        };
        control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
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
            Msg = (int)PInvokeCore.WM_CANCELMODE,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(0, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(0, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [EnumData<FlatStyle>]
    public void ButtonBase_WndProc_InvokeCancelModeMousePressedLostFocusWithHandle_Success(FlatStyle flatStyle)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle
        };
        control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        control.OnLostFocus(new EventArgs());
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
            Msg = (int)PInvokeCore.WM_CANCELMODE,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(0, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(0, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [EnumData<FlatStyle>]
    public void ButtonBase_WndProc_InvokeCancelModeMousePressedInButtonUpWithHandle_Success(FlatStyle flatStyle)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle
        };
        control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Message buttonM = new()
        {
            Msg = (int)PInvokeCore.WM_LBUTTONUP
        };
        int mouseUpCallCount = 0;
        control.MouseUp += (sender, e) =>
        {
            int callCount = 0;
            control.LostFocus += (sender, e) => callCount++;
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_CANCELMODE,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(0, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(0, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            mouseUpCallCount++;
        };
        control.WndProc(ref buttonM);
        Assert.Equal(1, mouseUpCallCount);
    }

    [WinFormsTheory]
    [EnumData<FlatStyle>]
    public void ButtonBase_WndProc_InvokeCaptureChangedWithoutHandle_Success(FlatStyle flatStyle)
    {
        using (new NoAssertContext())
        {
            using SubButtonBase control = new()
            {
                FlatStyle = flatStyle
            };
            int callCount = 0;
            control.MouseCaptureChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_CAPTURECHANGED,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(2, callCount);
            Assert.False(control.IsHandleCreated);
        }
    }

    [WinFormsTheory]
    [EnumData<FlatStyle>]
    public void ButtonBase_WndProc_InvokeCaptureChangedMousePressedWithoutHandle_Success(FlatStyle flatStyle)
    {
        using (new NoAssertContext())
        {
            using SubButtonBase control = new()
            {
                FlatStyle = flatStyle
            };
            control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
            int callCount = 0;
            control.MouseCaptureChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_CAPTURECHANGED,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(2, callCount);
            Assert.False(control.IsHandleCreated);
        }
    }

    [WinFormsTheory]
    [EnumData<FlatStyle>]
    public void ButtonBase_WndProc_InvokeCaptureChangedMousePressedLostFocusWithoutHandle_Success(FlatStyle flatStyle)
    {
        using (new NoAssertContext())
        {
            using SubButtonBase control = new()
            {
                FlatStyle = flatStyle
            };
            control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
            control.OnLostFocus(new EventArgs());
            int callCount = 0;
            control.MouseCaptureChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_CAPTURECHANGED,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(2, callCount);
            Assert.False(control.IsHandleCreated);
        }
    }

    [WinFormsTheory]
    [EnumData<FlatStyle>]
    public void ButtonBase_WndProc_InvokeCaptureChangedMousePressedLostFocusInButtonUpWithoutHandle_Success(FlatStyle flatStyle)
    {
        using (new NoAssertContext())
        {
            using SubButtonBase control = new()
            {
                FlatStyle = flatStyle
            };
            control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));

            Message buttonM = new()
            {
                Msg = (int)PInvokeCore.WM_LBUTTONUP
            };
            int mouseUpCallCount = 0;
            control.MouseUp += (sender, e) =>
            {
                int callCount = 0;
                control.MouseCaptureChanged += (sender, e) =>
                {
                    Assert.Same(control, sender);
                    Assert.Same(EventArgs.Empty, e);
                    callCount++;
                };
                Message m = new()
                {
                    Msg = (int)PInvokeCore.WM_CAPTURECHANGED,
                    Result = 250
                };
                control.WndProc(ref m);
                Assert.Equal(IntPtr.Zero, m.Result);
                Assert.Equal(1, callCount);
                Assert.True(control.IsHandleCreated);

                // Call again.
                control.WndProc(ref m);
                Assert.Equal(IntPtr.Zero, m.Result);
                Assert.Equal(2, callCount);
                Assert.True(control.IsHandleCreated);

                mouseUpCallCount++;
            };
            control.WndProc(ref buttonM);
            Assert.Equal(1, mouseUpCallCount);
        }
    }

    [WinFormsTheory]
    [EnumData<FlatStyle>]
    public void ButtonBase_WndProc_InvokeCaptureChangedWithHandle_Success(FlatStyle flatStyle)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        control.MouseCaptureChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_CAPTURECHANGED,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(2, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(FlatStyle.Flat, 1)]
    [InlineData(FlatStyle.Popup, 1)]
    [InlineData(FlatStyle.Standard, 1)]
    [InlineData(FlatStyle.System, 0)]
    public void ButtonBase_WndProc_InvokeCaptureChangedMousePressedWithHandle_Success(FlatStyle flatStyle, int expectedInvalidatedCallCount)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle
        };
        control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        control.MouseCaptureChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_CAPTURECHANGED,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(2, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [EnumData<FlatStyle>]
    public void ButtonBase_WndProc_InvokeCaptureChangedMousePressedLostFocusWithHandle_Success(FlatStyle flatStyle)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle
        };
        control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        control.OnLostFocus(new EventArgs());
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        control.MouseCaptureChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_CAPTURECHANGED,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(2, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [EnumData<FlatStyle>]
    public void ButtonBase_WndProc_InvokeCaptureChangedMousePressedInButtonUpWithHandle_Success(FlatStyle flatStyle)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle
        };
        control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Message buttonM = new()
        {
            Msg = (int)PInvokeCore.WM_LBUTTONUP
        };
        int mouseUpCallCount = 0;
        control.MouseUp += (sender, e) =>
        {
            int callCount = 0;
            control.MouseCaptureChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_CAPTURECHANGED,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(2, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            mouseUpCallCount++;
        };
        control.WndProc(ref buttonM);
        Assert.Equal(1, mouseUpCallCount);
    }

    [WinFormsTheory]
    [EnumData<FlatStyle>]
    public void ButtonBase_WndProc_InvokeClick_Success(FlatStyle flatStyle)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle
        };

        int callCount = 0;
        control.Click += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };

        Message m = new()
        {
            Msg = (int)PInvoke.BM_CLICK,
            Result = 250
        };

        control.WndProc(ref m);
        Assert.Equal(250, m.Result);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [EnumData<FlatStyle>]
    public void ButtonBase_WndProc_InvokeClickButtonButtonBase_Success(FlatStyle flatStyle)
    {
        using ButtonControl control = new()
        {
            FlatStyle = flatStyle
        };

        int callCount = 0;
        control.Click += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };

        int performCallCount = 0;
        control.PerformClickAction = () => performCallCount++;

        Message m = new()
        {
            Msg = (int)PInvoke.BM_CLICK,
            Result = 250
        };

        control.WndProc(ref m);
        Assert.Equal(250, m.Result);
        Assert.Equal(0, callCount);
        Assert.Equal(1, performCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [EnumData<FlatStyle>]
    public void ButtonBase_WndProc_InvokeClickWithHandle_Success(FlatStyle flatStyle)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle
        };

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        control.Click += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };

        Message m = new()
        {
            Msg = (int)PInvoke.BM_CLICK,
            Result = 250
        };

        control.WndProc(ref m);
        Assert.Equal(250, m.Result);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [EnumData<FlatStyle>]
    public void ButtonBase_WndProc_InvokeClickButtonControlWithHandle_Success(FlatStyle flatStyle)
    {
        using ButtonControl control = new()
        {
            FlatStyle = flatStyle
        };

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        control.Click += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };

        int performCallCount = 0;
        control.PerformClickAction = () => performCallCount++;
        Message m = new()
        {
            Msg = (int)PInvoke.BM_CLICK,
            Result = 250
        };

        control.WndProc(ref m);
        Assert.Equal(250, m.Result);
        Assert.Equal(0, callCount);
        Assert.Equal(1, performCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [EnumData<FlatStyle>]
    public void ButtonBase_WndProc_InvokeKillFocusWithoutHandle_Success(FlatStyle flatStyle)
    {
        using (new NoAssertContext())
        {
            using SubButtonBase control = new()
            {
                FlatStyle = flatStyle
            };
            int callCount = 0;
            control.LostFocus += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_KILLFOCUS,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(2, callCount);
            Assert.False(control.IsHandleCreated);
        }
    }

    [WinFormsTheory]
    [EnumData<FlatStyle>]
    public void ButtonBase_WndProc_InvokeKillFocusMousePressedWithoutHandle_Success(FlatStyle flatStyle)
    {
        using (new NoAssertContext())
        {
            using SubButtonBase control = new()
            {
                FlatStyle = flatStyle
            };
            control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
            int callCount = 0;
            control.LostFocus += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_KILLFOCUS,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(2, callCount);
            Assert.False(control.IsHandleCreated);
        }
    }

    [WinFormsTheory]
    [EnumData<FlatStyle>]
    public void ButtonBase_WndProc_InvokeKillFocusMousePressedLostFocusWithoutHandle_Success(FlatStyle flatStyle)
    {
        using (new NoAssertContext())
        {
            using SubButtonBase control = new()
            {
                FlatStyle = flatStyle
            };
            control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
            control.OnLostFocus(new EventArgs());
            int callCount = 0;
            control.LostFocus += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_KILLFOCUS,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(2, callCount);
            Assert.False(control.IsHandleCreated);
        }
    }

    [WinFormsTheory]
    [EnumData<FlatStyle>]
    public void ButtonBase_WndProc_InvokeKillFocusMousePressedLostFocusInButtonUpWithoutHandle_Success(FlatStyle flatStyle)
    {
        using (new NoAssertContext())
        {
            using SubButtonBase control = new()
            {
                FlatStyle = flatStyle
            };
            control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));

            Message buttonM = new()
            {
                Msg = (int)PInvokeCore.WM_LBUTTONUP
            };
            int mouseUpCallCount = 0;
            control.MouseUp += (sender, e) =>
            {
                int callCount = 0;
                control.LostFocus += (sender, e) =>
                {
                    Assert.Same(control, sender);
                    Assert.Same(EventArgs.Empty, e);
                    callCount++;
                };
                Message m = new()
                {
                    Msg = (int)PInvokeCore.WM_KILLFOCUS,
                    Result = 250
                };
                control.WndProc(ref m);
                Assert.Equal(IntPtr.Zero, m.Result);
                Assert.Equal(1, callCount);
                Assert.True(control.IsHandleCreated);

                // Call again.
                control.WndProc(ref m);
                Assert.Equal(IntPtr.Zero, m.Result);
                Assert.Equal(2, callCount);
                Assert.True(control.IsHandleCreated);

                mouseUpCallCount++;
            };
            control.WndProc(ref buttonM);
            Assert.Equal(1, mouseUpCallCount);
        }
    }

    [WinFormsTheory]
    [EnumData<FlatStyle>]
    public void ButtonBase_WndProc_InvokeKillFocusWithHandle_Success(FlatStyle flatStyle)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        control.LostFocus += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_KILLFOCUS,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(2, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(FlatStyle.Flat, 2)]
    [InlineData(FlatStyle.Popup, 2)]
    [InlineData(FlatStyle.Standard, 2)]
    [InlineData(FlatStyle.System, 1)]
    public void ButtonBase_WndProc_InvokeKillFocusMousePressedWithHandle_Success(FlatStyle flatStyle, int expectedInvalidatedCallCount)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle
        };
        control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        control.LostFocus += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_KILLFOCUS,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(2, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [EnumData<FlatStyle>]
    public void ButtonBase_WndProc_InvokeKillFocusMousePressedLostFocusWithHandle_Success(FlatStyle flatStyle)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle
        };
        control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        control.OnLostFocus(new EventArgs());
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        control.LostFocus += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_KILLFOCUS,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(2, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [EnumData<FlatStyle>]
    public void ButtonBase_WndProc_InvokeKillFocusMousePressedInButtonUpWithHandle_Success(FlatStyle flatStyle)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle
        };
        control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Message buttonM = new()
        {
            Msg = (int)PInvokeCore.WM_LBUTTONUP
        };
        int mouseUpCallCount = 0;
        control.MouseUp += (sender, e) =>
        {
            int callCount = 0;
            control.LostFocus += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_KILLFOCUS,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(2, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            mouseUpCallCount++;
        };
        control.WndProc(ref buttonM);
        Assert.Equal(1, mouseUpCallCount);
    }

    [WinFormsTheory]
    [EnumData<FlatStyle>]
    public void ButtonBase_WndProc_InvokeMouseHoverWithHandle_Success(FlatStyle flatStyle)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle
        };
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

    public static IEnumerable<object[]> WndProc_MouseUp_TestData()
    {
        yield return new object[] { true, (int)PInvokeCore.WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero, (IntPtr)250, MouseButtons.Left, 1, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_LBUTTONUP, PARAM.FromLowHigh(1, 2), IntPtr.Zero, (IntPtr)250, MouseButtons.Left, 1, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_LBUTTONUP, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, (IntPtr)250, MouseButtons.Left, 1, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, MouseButtons.Left, 1, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_LBUTTONUP, PARAM.FromLowHigh(1, 2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Left, 1, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_LBUTTONUP, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Left, 1, -1, -2 };

        yield return new object[] { true, (int)PInvokeCore.WM_MBUTTONUP, IntPtr.Zero, IntPtr.Zero, (IntPtr)250, MouseButtons.Middle, 1, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_MBUTTONUP, PARAM.FromLowHigh(1, 2), IntPtr.Zero, (IntPtr)250, MouseButtons.Middle, 1, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_MBUTTONUP, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, (IntPtr)250, MouseButtons.Middle, 1, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_MBUTTONUP, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, MouseButtons.Middle, 1, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_MBUTTONUP, PARAM.FromLowHigh(1, 2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Middle, 1, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_MBUTTONUP, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Middle, 1, -1, -2 };

        yield return new object[] { true, (int)PInvokeCore.WM_RBUTTONUP, IntPtr.Zero, IntPtr.Zero, (IntPtr)250, MouseButtons.Right, 1, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_RBUTTONUP, PARAM.FromLowHigh(1, 2), IntPtr.Zero, (IntPtr)250, MouseButtons.Right, 1, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_RBUTTONUP, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, (IntPtr)250, MouseButtons.Right, 1, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_RBUTTONUP, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, MouseButtons.Right, 1, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_RBUTTONUP, PARAM.FromLowHigh(1, 2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Right, 1, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_RBUTTONUP, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Right, 1, -1, -2 };

        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONUP, IntPtr.Zero, IntPtr.Zero, (IntPtr)250, MouseButtons.None, 1, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONUP, PARAM.FromLowHigh(1, 2), IntPtr.Zero, (IntPtr)250, MouseButtons.None, 1, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONUP, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, (IntPtr)250, MouseButtons.None, 1, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONUP, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, MouseButtons.None, 1, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONUP, PARAM.FromLowHigh(1, 2), IntPtr.Zero, IntPtr.Zero, MouseButtons.None, 1, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONUP, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, IntPtr.Zero, MouseButtons.None, 1, -1, -2 };

        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONUP, IntPtr.Zero, PARAM.FromLowHigh(2, 1), (IntPtr)250, MouseButtons.XButton1, 1, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONUP, PARAM.FromLowHigh(1, 2), PARAM.FromLowHigh(2, 1), (IntPtr)250, MouseButtons.XButton1, 1, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONUP, PARAM.FromLowHigh(-1, -2), PARAM.FromLowHigh(2, 1), (IntPtr)250, MouseButtons.XButton1, 1, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONUP, IntPtr.Zero, PARAM.FromLowHigh(2, 1), IntPtr.Zero, MouseButtons.XButton1, 1, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONUP, PARAM.FromLowHigh(1, 2), PARAM.FromLowHigh(2, 1), IntPtr.Zero, MouseButtons.XButton1, 1, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONUP, PARAM.FromLowHigh(-1, -2), PARAM.FromLowHigh(2, 1), IntPtr.Zero, MouseButtons.XButton1, 1, -1, -2 };

        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONUP, IntPtr.Zero, PARAM.FromLowHigh(1, 2), (IntPtr)250, MouseButtons.XButton2, 1, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONUP, PARAM.FromLowHigh(1, 2), PARAM.FromLowHigh(1, 2), (IntPtr)250, MouseButtons.XButton2, 1, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONUP, PARAM.FromLowHigh(-1, -2), PARAM.FromLowHigh(1, 2), (IntPtr)250, MouseButtons.XButton2, 1, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONUP, IntPtr.Zero, PARAM.FromLowHigh(1, 2), IntPtr.Zero, MouseButtons.XButton2, 1, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONUP, PARAM.FromLowHigh(1, 2), PARAM.FromLowHigh(1, 2), IntPtr.Zero, MouseButtons.XButton2, 1, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONUP, PARAM.FromLowHigh(-1, -2), PARAM.FromLowHigh(1, 2), IntPtr.Zero, MouseButtons.XButton2, 1, -1, -2 };
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_MouseUp_TestData))]
    public void ButtonBase_WndProc_InvokeMouseUpWithoutHandle_Success(bool userMouse, int msg, IntPtr lParam, IntPtr wParam, IntPtr expectedResult, MouseButtons expectedButton, int expectedClicks, int expectedX, int expectedY)
    {
        using (new NoAssertContext())
        {
            using SubButtonBase control = new();
            control.SetStyle(ControlStyles.UserMouse, userMouse);
            int callCount = 0;
            control.MouseUp += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(expectedButton, e.Button);
                Assert.Equal(expectedClicks, e.Clicks);
                Assert.Equal(expectedX, e.X);
                Assert.Equal(expectedY, e.Y);
                Assert.Equal(0, e.Delta);
                callCount++;
            };
            Message m = new()
            {
                Msg = msg,
                LParam = lParam,
                WParam = wParam,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(expectedResult, m.Result);
            Assert.Equal(1, callCount);
            Assert.False(control.Capture);
            Assert.False(control.Focused);
            Assert.True(control.IsHandleCreated);
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_MouseUp_TestData))]
    public void ButtonBase_WndProc_InvokeMouseUpWithoutHandleNotSelectable_Success(bool userMouse, int msg, IntPtr lParam, IntPtr wParam, IntPtr expectedResult, MouseButtons expectedButton, int expectedClicks, int expectedX, int expectedY)
    {
        using (new NoAssertContext())
        {
            using SubButtonBase control = new();
            control.SetStyle(ControlStyles.UserMouse, userMouse);
            control.SetStyle(ControlStyles.Selectable, false);
            int callCount = 0;
            control.MouseUp += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(expectedButton, e.Button);
                Assert.Equal(expectedClicks, e.Clicks);
                Assert.Equal(expectedX, e.X);
                Assert.Equal(expectedY, e.Y);
                Assert.Equal(0, e.Delta);
                callCount++;
            };
            Message m = new()
            {
                Msg = msg,
                LParam = lParam,
                WParam = wParam,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(expectedResult, m.Result);
            Assert.Equal(1, callCount);
            Assert.False(control.Capture);
            Assert.False(control.Focused);
            Assert.True(control.IsHandleCreated);
        }
    }

    [WinFormsTheory]
    [InlineData((int)PInvokeCore.WM_LBUTTONUP)]
    [InlineData((int)PInvokeCore.WM_MBUTTONUP)]
    [InlineData((int)PInvokeCore.WM_RBUTTONUP)]
    [InlineData((int)PInvokeCore.WM_XBUTTONUP)]
    public void ButtonBase_WndProc_InvokeMouseUpWithoutHandleNotEnabled_CallsMouseUp(int msg)
    {
        using (new NoAssertContext())
        {
            using SubButtonBase control = new()
            {
                Enabled = false
            };
            int callCount = 0;
            control.MouseUp += (sender, e) => callCount++;
            Message m = new()
            {
                Msg = msg,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(250, m.Result);
            Assert.Equal(1, callCount);
            Assert.False(control.Capture);
            Assert.False(control.Focused);
            Assert.True(control.IsHandleCreated);
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_MouseUp_TestData))]
    public void ButtonBase_WndProc_InvokeMouseUpWithHandle_Success(bool userMouse, int msg, IntPtr lParam, IntPtr wParam, IntPtr expectedResult, MouseButtons expectedButton, int expectedClicks, int expectedX, int expectedY)
    {
        using SubButtonBase control = new();
        control.SetStyle(ControlStyles.UserMouse, userMouse);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        control.MouseUp += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(expectedButton, e.Button);
            Assert.Equal(expectedClicks, e.Clicks);
            Assert.Equal(expectedX, e.X);
            Assert.Equal(expectedY, e.Y);
            Assert.Equal(0, e.Delta);
            callCount++;
        };
        Message m = new()
        {
            Msg = msg,
            LParam = lParam,
            WParam = wParam,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(expectedResult, m.Result);
        Assert.Equal(1, callCount);
        Assert.False(control.Capture);
        Assert.False(control.Focused);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_MouseUp_TestData))]
    public void ButtonBase_WndProc_InvokeMouseUpWithHandleNotSelectable_DoesNotCallMouseUp(bool userMouse, int msg, IntPtr lParam, IntPtr wParam, IntPtr expectedResult, MouseButtons expectedButton, int expectedClicks, int expectedX, int expectedY)
    {
        using SubButtonBase control = new();
        control.SetStyle(ControlStyles.UserMouse, userMouse);
        control.SetStyle(ControlStyles.Selectable, false);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        control.MouseUp += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(expectedButton, e.Button);
            Assert.Equal(expectedClicks, e.Clicks);
            Assert.Equal(expectedX, e.X);
            Assert.Equal(expectedY, e.Y);
            Assert.Equal(0, e.Delta);
            callCount++;
        };
        Message m = new()
        {
            Msg = msg,
            LParam = lParam,
            WParam = wParam,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(expectedResult, m.Result);
        Assert.Equal(1, callCount);
        Assert.False(control.Capture);
        Assert.False(control.Focused);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData((int)PInvokeCore.WM_LBUTTONUP)]
    [InlineData((int)PInvokeCore.WM_MBUTTONUP)]
    [InlineData((int)PInvokeCore.WM_RBUTTONUP)]
    [InlineData((int)PInvokeCore.WM_XBUTTONUP)]
    public void ButtonBase_WndProc_InvokeMouseUpWithHandleNotEnabled_CallsMouseUp(int msg)
    {
        using SubButtonBase control = new()
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

        int callCount = 0;
        control.MouseUp += (sender, e) => callCount++;
        Message m = new()
        {
            Msg = msg,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(250, m.Result);
        Assert.Equal(1, callCount);
        Assert.False(control.Capture);
        Assert.False(control.Focused);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> WndProc_ReflectCommandWithoutHandle_TestData()
    {
        yield return new object[] { FlatStyle.Flat, IntPtr.Zero, IntPtr.Zero, 0 };
        yield return new object[] { FlatStyle.Flat, PARAM.FromLowHigh(0, (int)PInvoke.BN_CLICKED), IntPtr.Zero, 0 };
        yield return new object[] { FlatStyle.Flat, PARAM.FromLowHigh(123, (int)PInvoke.BN_CLICKED), IntPtr.Zero, 0 };
        yield return new object[] { FlatStyle.Flat, PARAM.FromLowHigh(123, 456), IntPtr.Zero, 0 };

        yield return new object[] { FlatStyle.Popup, IntPtr.Zero, IntPtr.Zero, 0 };
        yield return new object[] { FlatStyle.Popup, PARAM.FromLowHigh(0, (int)PInvoke.BN_CLICKED), IntPtr.Zero, 0 };
        yield return new object[] { FlatStyle.Popup, PARAM.FromLowHigh(123, (int)PInvoke.BN_CLICKED), IntPtr.Zero, 0 };
        yield return new object[] { FlatStyle.Popup, PARAM.FromLowHigh(123, 456), IntPtr.Zero, 0 };

        yield return new object[] { FlatStyle.Standard, IntPtr.Zero, IntPtr.Zero, 0 };
        yield return new object[] { FlatStyle.Standard, PARAM.FromLowHigh(0, (int)PInvoke.BN_CLICKED), IntPtr.Zero, 0 };
        yield return new object[] { FlatStyle.Standard, PARAM.FromLowHigh(123, (int)PInvoke.BN_CLICKED), IntPtr.Zero, 0 };
        yield return new object[] { FlatStyle.Standard, PARAM.FromLowHigh(123, 456), IntPtr.Zero, 0 };

        yield return new object[] { FlatStyle.System, IntPtr.Zero, (IntPtr)250, 1 };
        yield return new object[] { FlatStyle.System, PARAM.FromLowHigh(0, (int)PInvoke.BN_CLICKED), (IntPtr)250, 1 };
        yield return new object[] { FlatStyle.System, PARAM.FromLowHigh(123, (int)PInvoke.BN_CLICKED), (IntPtr)250, 1 };
        yield return new object[] { FlatStyle.System, PARAM.FromLowHigh(123, 456), (IntPtr)250, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_ReflectCommandWithoutHandle_TestData))]
    public void ButtonBase_WndProc_InvokeReflectCommandWithoutHandle_Success(FlatStyle flatStyle, IntPtr wParam, IntPtr expectedResult, int expectedCallCount)
    {
        using (new NoAssertContext())
        {
            using SubButtonBase control = new()
            {
                FlatStyle = flatStyle
            };
            int callCount = 0;
            control.Click += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };

            Message m = new()
            {
                Msg = (int)(MessageId.WM_REFLECT_COMMAND),
                WParam = wParam,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(expectedResult, m.Result);
            Assert.Equal(expectedCallCount, callCount);
            Assert.False(control.IsHandleCreated);
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_ReflectCommandWithoutHandle_TestData))]
    public void ButtonBase_WndProc_InvokeReflectCommandWithHandle_Success(FlatStyle flatStyle, IntPtr wParam, IntPtr expectedResult, int expectedCallCount)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int callCount = 0;
        control.Click += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };

        Message m = new()
        {
            Msg = (int)(MessageId.WM_REFLECT_COMMAND),
            WParam = wParam,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(expectedResult, m.Result);
        Assert.Equal(expectedCallCount, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> WndProc_SetState_TestData()
    {
        yield return new object[] { FlatStyle.Flat, (IntPtr)250 };
        yield return new object[] { FlatStyle.Popup, (IntPtr)250 };
        yield return new object[] { FlatStyle.Standard, (IntPtr)250 };
        yield return new object[] { FlatStyle.System, IntPtr.Zero };
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_SetState_TestData))]
    public void ButtonBase_WndProc_InvokeSetStateWithoutHandle_Success(FlatStyle flatStyle, IntPtr expectedResult)
    {
        using (new NoAssertContext())
        {
            using SubButtonBase control = new()
            {
                FlatStyle = flatStyle
            };

            Message m = new()
            {
                Msg = (int)PInvoke.BM_SETSTATE,
                Result = 250
            };

            control.WndProc(ref m);
            Assert.Equal(expectedResult, m.Result);
            Assert.False(control.IsHandleCreated);
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_SetState_TestData))]
    public void ButtonBase_WndProc_InvokeSetStateWithHandle_Success(FlatStyle flatStyle, IntPtr expectedResult)
    {
        using SubButtonBase control = new()
        {
            FlatStyle = flatStyle
        };

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Message m = new()
        {
            Msg = (int)PInvoke.BM_SETSTATE,
            Result = 250
        };

        control.WndProc(ref m);
        Assert.Equal(expectedResult, m.Result);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    private class SubButton : Button
    {
        public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

        public new void OnTextChanged(EventArgs e) => base.OnTextChanged(e);
    }

    private class ButtonControl : ButtonBase, IButtonControl
    {
        public DialogResult DialogResult { get; set; }

        public new AccessibleObject CreateAccessibilityInstance() => base.CreateAccessibilityInstance();

        public void NotifyDefault(bool value) => throw new NotImplementedException();

        public Action PerformClickAction { get; set; }

        public void PerformClick() => PerformClickAction();

        public new void WndProc(ref Message m) => base.WndProc(ref m);
    }

    private class SubButtonBase : ButtonBase
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

        public new bool IsDefault
        {
            get => base.IsDefault;
            set => base.IsDefault = value;
        }

        public new bool ResizeRedraw
        {
            get => base.ResizeRedraw;
            set => base.ResizeRedraw = value;
        }

        public new bool ShowFocusCues => base.ShowFocusCues;

        public new bool ShowKeyboardCues => base.ShowKeyboardCues;

        public new AccessibleObject CreateAccessibilityInstance() => base.CreateAccessibilityInstance();

        public new void Dispose(bool disposing) => base.Dispose(disposing);

        public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

        public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

        public new bool GetTopLevel() => base.GetTopLevel();

        public new void OnClick(EventArgs e) => base.OnClick(e);

        public new void OnEnabledChanged(EventArgs e) => base.OnEnabledChanged(e);

        public new void OnGotFocus(EventArgs e) => base.OnGotFocus(e);

        public new void OnHandleCreated(EventArgs e) => base.OnHandleCreated(e);

        public new void OnHandleDestroyed(EventArgs e) => base.OnHandleDestroyed(e);

        public new void OnKeyDown(KeyEventArgs kevent) => base.OnKeyDown(kevent);

        public new void OnKeyUp(KeyEventArgs kevent) => base.OnKeyUp(kevent);

        public new void OnLostFocus(EventArgs e) => base.OnLostFocus(e);

        public new void OnMouseDown(MouseEventArgs e) => base.OnMouseDown(e);

        public new void OnMouseEnter(EventArgs eventargs) => base.OnMouseEnter(eventargs);

        public new void OnMouseLeave(EventArgs eventargs) => base.OnMouseLeave(eventargs);

        public new void OnMouseMove(MouseEventArgs e) => base.OnMouseMove(e);

        public new void OnMouseUp(MouseEventArgs e) => base.OnMouseUp(e);

        public new void OnPaint(PaintEventArgs pevent) => base.OnPaint(pevent);

        public new void OnParentChanged(EventArgs e) => base.OnParentChanged(e);

        public new void OnTextChanged(EventArgs e) => base.OnTextChanged(e);

        public new void OnVisibleChanged(EventArgs e) => base.OnVisibleChanged(e);

        public new void ResetFlagsandPaint() => base.ResetFlagsandPaint();

        public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);

        public new void WndProc(ref Message m) => base.WndProc(ref m);
    }

    protected override ButtonBase CreateButton() => new Button();
}
