// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using Moq;

namespace System.Windows.Forms.Tests;

public class FormTests
{
    [WinFormsFact]
    public void Form_Ctor_Default()
    {
        using SubForm control = new();
        Assert.Null(control.AcceptButton);
        Assert.Null(control.AccessibleDefaultActionDescription);
        Assert.Null(control.AccessibleDescription);
        Assert.Null(control.AccessibleName);
        Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
        Assert.Null(control.ActiveControl);
        Assert.Null(control.ActiveMdiChild);
        Assert.False(control.AllowDrop);
        Assert.False(control.AllowTransparency);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
#pragma warning disable 0618
        Assert.True(control.AutoScale);
#pragma warning restore 0618
        Assert.True(control.AutoScaleBaseSize.Width > 0);
        Assert.True(control.AutoScaleBaseSize.Height > 0);
        Assert.Equal(SizeF.Empty, control.AutoScaleDimensions);
        Assert.Equal(new SizeF(1, 1), control.AutoScaleFactor);
        Assert.Equal(AutoScaleMode.Inherit, control.AutoScaleMode);
        Assert.False(control.AutoScroll);
        Assert.Equal(Size.Empty, control.AutoScrollMargin);
        Assert.Equal(Size.Empty, control.AutoScrollMinSize);
        Assert.Equal(Point.Empty, control.AutoScrollPosition);
        Assert.False(control.AutoSize);
        Assert.Equal(AutoSizeMode.GrowOnly, control.AutoSizeMode);
        Assert.Equal(Control.DefaultBackColor, control.BackColor);
        Assert.Null(control.BackgroundImage);
        Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
        Assert.NotNull(control.BindingContext);
        Assert.Same(control.BindingContext, control.BindingContext);
        Assert.Equal(300, control.Bottom);
        Assert.Equal(new Rectangle(0, 0, 300, 300), control.Bounds);
        Assert.Null(control.CancelButton);
        Assert.False(control.CanEnableIme);
        Assert.False(control.CanFocus);
        Assert.True(control.CanRaiseEvents);
        Assert.False(control.CanSelect);
        Assert.False(control.Capture);
        Assert.True(control.CausesValidation);
        Assert.Equal(new Rectangle(0, 0, control.ClientSize.Width, control.ClientSize.Height), control.ClientRectangle);
        Assert.True(control.ClientSize.Width > 0);
        Assert.True(control.ClientSize.Height > 0);
        Assert.Null(control.Container);
        Assert.False(control.ContainsFocus);
        Assert.Null(control.ContextMenuStrip);
        Assert.True(control.ControlBox);
        Assert.Empty(control.Controls);
        Assert.Same(control.Controls, control.Controls);
        Assert.False(control.Created);
        Assert.Equal(SizeF.Empty, control.CurrentAutoScaleDimensions);
        Assert.Equal(AutoValidate.EnablePreventFocusChange, control.AutoValidate);
        Assert.Equal(Cursors.Default, control.Cursor);
        Assert.Equal(Cursors.Default, control.DefaultCursor);
        Assert.Equal(ImeMode.NoControl, control.DefaultImeMode);
        Assert.Equal(new Padding(3), control.DefaultMargin);
        Assert.Equal(Size.Empty, control.DefaultMaximumSize);
        Assert.Equal(Size.Empty, control.DefaultMinimumSize);
        Assert.Equal(Padding.Empty, control.DefaultPadding);
        Assert.Equal(new Size(300, 300), control.DefaultSize);
        Assert.False(control.DesignMode);
        Assert.Equal(new Rectangle(-SystemInformation.WorkingArea.X, -SystemInformation.WorkingArea.Y, 300, 300), control.DesktopBounds);
        Assert.Equal(new Point(-SystemInformation.WorkingArea.X, -SystemInformation.WorkingArea.Y), control.DesktopLocation);
        Assert.Equal(DialogResult.None, control.DialogResult);
        Assert.Equal(0, control.DisplayRectangle.X);
        Assert.Equal(0, control.DisplayRectangle.Y);
        Assert.True(control.DisplayRectangle.Width > 0);
        Assert.True(control.DisplayRectangle.Height > 0);
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
        Assert.Equal(FormBorderStyle.Sizable, control.FormBorderStyle);
        Assert.False(control.HasChildren);
        Assert.Equal(300, control.Height);
        Assert.False(control.HelpButton);
        Assert.NotNull(control.HorizontalScroll);
        Assert.Same(control.HorizontalScroll, control.HorizontalScroll);
        Assert.False(control.HScroll);
        Assert.NotNull(control.Icon);
        Assert.Equal(ImeMode.NoControl, control.ImeMode);
        Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
        Assert.False(control.IsAccessible);
        Assert.False(control.IsMdiChild);
        Assert.False(control.IsMdiContainer);
        Assert.False(control.IsMirrored);
        Assert.False(control.IsRestrictedWindow);
        Assert.False(control.KeyPreview);
        Assert.NotNull(control.LayoutEngine);
        Assert.Same(control.LayoutEngine, control.LayoutEngine);
        Assert.Equal(0, control.Left);
        Assert.Equal(Point.Empty, control.Location);
        Assert.Null(control.MainMenuStrip);
        Assert.Equal(new Padding(3), control.Margin);
        Assert.Empty(control.MdiChildren);
        Assert.Null(control.MdiParent);
        Assert.Equal(Rectangle.Empty, control.MaximizedBounds);
        Assert.True(control.MaximizeBox);
        Assert.Equal(Size.Empty, control.MaximumSize);
        Assert.True(control.MinimizeBox);
        Assert.Equal(Size.Empty, control.MinimumSize);
        Assert.False(control.Modal);
        Assert.Equal(1, control.Opacity);
        Assert.Empty(control.OwnedForms);
        Assert.Null(control.Owner);
        Assert.Equal(Padding.Empty, control.Padding);
        Assert.Null(control.Parent);
        Assert.True(control.PreferredSize.Width > 0);
        Assert.True(control.PreferredSize.Height > 0);
        Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
        Assert.False(control.RecreatingHandle);
        Assert.Null(control.Region);
        Assert.False(control.ResizeRedraw);
        Assert.Equal(new Rectangle(0, 0, 300, 300), control.RestoreBounds);
        Assert.Equal(300, control.Right);
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.False(control.RightToLeftLayout);
        Assert.True(control.ShowFocusCues);
        Assert.True(control.ShowIcon);
        Assert.True(control.ShowInTaskbar);
        Assert.True(control.ShowKeyboardCues);
        Assert.False(control.ShowWithoutActivation);
        Assert.Null(control.Site);
        Assert.Equal(new Size(300, 300), control.Size);
        Assert.Equal(SizeGripStyle.Auto, control.SizeGripStyle);
        Assert.Equal(FormStartPosition.WindowsDefaultLocation, control.StartPosition);
        Assert.Equal(0, control.TabIndex);
        Assert.True(control.TabStop);
        Assert.Empty(control.Text);
        Assert.Equal(0, control.Top);
        Assert.True(control.TopLevel);
        Assert.Same(control, control.TopLevelControl);
        Assert.False(control.TopMost);
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.False(control.UseWaitCursor);
        Assert.False(control.Visible);
        Assert.NotNull(control.VerticalScroll);
        Assert.Same(control.VerticalScroll, control.VerticalScroll);
        Assert.False(control.VScroll);
        Assert.Equal(300, control.Width);
        Assert.Equal(FormWindowState.Normal, control.WindowState);

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Form_CreateParams_GetDefault_ReturnsExpected()
    {
        using SubForm control = new();
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Null(createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0x50000, createParams.ExStyle);
        Assert.Equal(300, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(0x2CF0000, createParams.Style);
        Assert.Equal(300, createParams.Width);
        Assert.Equal(int.MinValue, createParams.X);
        Assert.Equal(int.MinValue, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public static void Form_Ctor_show_icon_by_default()
    {
        using Form form = new();
        Assert.True(form.Handle != IntPtr.Zero);

        HICON hSmallIcon = (HICON)PInvokeCore.SendMessage(form, PInvokeCore.WM_GETICON, (WPARAM)PInvoke.ICON_SMALL);
        Assert.False(hSmallIcon.IsNull);

        HICON hLargeIcon = (HICON)PInvokeCore.SendMessage(form, PInvokeCore.WM_GETICON, (WPARAM)PInvoke.ICON_BIG);
        Assert.False(hLargeIcon.IsNull);

        // normal form doesn't have WS_EX.DLGMODALFRAME set, and show icon
        WINDOW_EX_STYLE extendedStyle = (WINDOW_EX_STYLE)PInvokeCore.GetWindowLong(form, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
        Assert.False(extendedStyle.HasFlag(WINDOW_EX_STYLE.WS_EX_DLGMODALFRAME));
    }

    [WinFormsFact]
    public void Form_AcceptButtonGetSet()
    {
        using Form form = new();
        Mock<IButtonControl> mock = new(MockBehavior.Strict);
        mock.Setup(x => x.NotifyDefault(It.IsAny<bool>()));

        form.AcceptButton = mock.Object;

        Assert.Equal(mock.Object, form.AcceptButton);
    }

    [WinFormsTheory]
    [BoolData]
    public void Form_Active_Set_GetReturnsExpected(bool value)
    {
        using Form form = new()
        {
            Active = value
        };
        Assert.Equal(value, form.Active);

        // Set same.
        form.Active = value;
        Assert.Equal(value, form.Active);
    }

    // non deterministic, commenting out for now
    /*[Fact]
    public void Form_ActiveFormNotSetActive()
    {
        using Form form = new();

        form.Visible = true;
        form.Active = false;

        Assert.NotNull(Form.ActiveForm);
        Assert.Equal(form, Form.ActiveForm);
        Assert.False(Form.ActiveForm.Active);
    }*/

    [WinFormsFact]
    public void Form_ActiveMdiChildInternalGetSet()
    {
        using Form form = new();
        using Form child = new();

        form.ActiveMdiChildInternal = child;

        Assert.NotNull(form.ActiveMdiChildInternal);
        Assert.Equal(child, form.ActiveMdiChildInternal);
    }

    [WinFormsFact]
    public void Form_ActiveMdiChildGetSet()
    {
        using Form form = new();
        using Form child = new()
        {
            Visible = true,
            Enabled = true
        };

        form.ActiveMdiChildInternal = child;

        Assert.NotNull(form.ActiveMdiChild);
        Assert.Equal(child, form.ActiveMdiChild);
    }

    [WinFormsFact]
    public void Form_ActiveMdiChildGetSetChildNotVisible()
    {
        using Form form = new();
        using Form child = new()
        {
            Visible = false,
            Enabled = true
        };

        form.ActiveMdiChildInternal = child;

        Assert.Null(form.ActiveMdiChild);
    }

    [WinFormsFact]
    public void Form_ActiveMdiChildGetSetChildNotEnabled()
    {
        using Form form = new();
        using Form child = new()
        {
            Visible = true,
            Enabled = false
        };

        form.ActiveMdiChildInternal = child;

        Assert.Null(form.ActiveMdiChild);
    }

    [WinFormsTheory]
    [BoolData]
    public void Form_AllowTransparency_Set_GetReturnsExpected(bool value)
    {
        using Form control = new()
        {
            AllowTransparency = value
        };
        Assert.Equal(value, control.AllowTransparency);
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.Equal(1, control.Opacity);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AllowTransparency = value;
        Assert.Equal(value, control.AllowTransparency);
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.Equal(1, control.Opacity);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.AllowTransparency = !value;
        Assert.Equal(!value, control.AllowTransparency);
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.Equal(1, control.Opacity);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> AllowTransparency_SetWithOpacityAndTransparencyKey_TestData()
    {
        yield return new object[] { true, Color.Red, 0.5 };
        yield return new object[] { false, Color.Empty, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(AllowTransparency_SetWithOpacityAndTransparencyKey_TestData))]
    public void Form_AllowTransparency_SetWithOpacityAndTransparencyKey_GetReturnsExpected(bool value, Color expectedTransparencyKey, float expectedOpacity)
    {
        using Form control = new()
        {
            Opacity = 0.5,
            TransparencyKey = Color.Red,
            AllowTransparency = value
        };
        Assert.Equal(value, control.AllowTransparency);
        Assert.Equal(expectedTransparencyKey, control.TransparencyKey);
        Assert.Equal(expectedOpacity, control.Opacity);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AllowTransparency = value;
        Assert.Equal(value, control.AllowTransparency);
        Assert.Equal(expectedTransparencyKey, control.TransparencyKey);
        Assert.Equal(expectedOpacity, control.Opacity);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.AllowTransparency = !value;
        Assert.Equal(!value, control.AllowTransparency);
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.Equal(1, control.Opacity);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void Form_AllowTransparency_SetNotTopLevel_GetReturnsExpected(bool value)
    {
        using Form control = new()
        {
            TopLevel = false,
            AllowTransparency = value
        };
        Assert.Equal(value, control.AllowTransparency);
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.Equal(1, control.Opacity);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AllowTransparency = value;
        Assert.Equal(value, control.AllowTransparency);
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.Equal(1, control.Opacity);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.AllowTransparency = !value;
        Assert.Equal(!value, control.AllowTransparency);
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.Equal(1, control.Opacity);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void Form_AllowTransparency_SetWithHandle_GetReturnsExpected(bool value, int expectedStyleChangedCallCount)
    {
        using Form control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.AllowTransparency = value;
        Assert.Equal(value, control.AllowTransparency);
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.Equal(1, control.Opacity);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.AllowTransparency = value;
        Assert.Equal(value, control.AllowTransparency);
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.Equal(1, control.Opacity);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.AllowTransparency = !value;
        Assert.Equal(!value, control.AllowTransparency);
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.Equal(1, control.Opacity);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount + 1, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount + 1, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> AllowTransparency_SetWithOpacityAndTransparencyKeyWithHandle_TestData()
    {
        yield return new object[] { true, Color.Red, 0.5, 0 };
        yield return new object[] { false, Color.Empty, 1, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(AllowTransparency_SetWithOpacityAndTransparencyKeyWithHandle_TestData))]
    public void Form_AllowTransparency_SetWithOpacityAndTransparencyKeyWithHandle_GetReturnsExpected(bool value, Color expectedTransparencyKey, float expectedOpacity, int expectedStyleChangedCallCount)
    {
        using Form control = new()
        {
            Opacity = 0.5,
            TransparencyKey = Color.Red
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
        Assert.Equal(expectedTransparencyKey, control.TransparencyKey);
        Assert.Equal(expectedOpacity, control.Opacity);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.AllowTransparency = value;
        Assert.Equal(value, control.AllowTransparency);
        Assert.Equal(expectedTransparencyKey, control.TransparencyKey);
        Assert.Equal(expectedOpacity, control.Opacity);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.AllowTransparency = !value;
        Assert.Equal(!value, control.AllowTransparency);
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.Equal(1, control.Opacity);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount + 1, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount + 1, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void Form_AllowTransparency_SetNotTopLevelWithHandle_GetReturnsExpected(bool value, int expectedStyleChangedCallCount)
    {
        using Form control = new()
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
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.Equal(1, control.Opacity);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.AllowTransparency = value;
        Assert.Equal(value, control.AllowTransparency);
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.Equal(1, control.Opacity);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.AllowTransparency = !value;
        Assert.Equal(!value, control.AllowTransparency);
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.Equal(1, control.Opacity);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount + 1, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount + 1, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

#pragma warning disable 0618

    [WinFormsTheory]
    [BoolData]
    public void Form_AutoScale_Set_GetReturnsExpected(bool value)
    {
        using Form form = new()
        {
            AutoScale = value
        };
        Assert.Equal(value, form.AutoScale);

        // Set same.
        form.AutoScale = value;
        Assert.Equal(value, form.AutoScale);
    }

#pragma warning restore 0618

    [WinFormsTheory]
    [EnumData<AutoScaleMode>]
    public void Form_AutoScaleMode_Set_GetReturnsExpected(AutoScaleMode value)
    {
        using Form form = new()
        {
            AutoScaleMode = value
        };
        Assert.Equal(value, form.AutoScaleMode);

        // Set same.
        form.AutoScaleMode = value;
        Assert.Equal(value, form.AutoScaleMode);
    }

    [WinFormsTheory]
    [BoolData]
    public void Form_AutoScroll_Set_GetReturnsExpected(bool value)
    {
        using Form form = new()
        {
            AutoScroll = value
        };
        Assert.Equal(value, form.AutoScroll);

        // Set same.
        form.AutoScroll = value;
        Assert.Equal(value, form.AutoScroll);
    }

    [WinFormsTheory]
    [BoolData]
    public void Form_AutoSizeSet_GetReturnsExpected(bool value)
    {
        using Form form = new()
        {
            AutoSize = value
        };
        Assert.Equal(value, form.AutoSize);

        // Set same.
        form.AutoSize = value;
        Assert.Equal(value, form.AutoSize);
    }

    public static IEnumerable<object[]> Opacity_Set_TestData()
    {
        foreach (bool allowTransparency in new bool[] { true, false })
        {
            yield return new object[] { allowTransparency, 1.1, 1.0, allowTransparency, allowTransparency };
            yield return new object[] { allowTransparency, 1.0, 1.0, allowTransparency, allowTransparency };
            yield return new object[] { allowTransparency, 0.5, 0.5, true, false };
            yield return new object[] { allowTransparency, 0, 0, true, false };
            yield return new object[] { allowTransparency, -0.1, 0, true, false };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Opacity_Set_TestData))]
    public void Form_Opacity_Set_GetReturnsExpected(bool allowTransparency, double value, double expected, bool expectedAllowTransparency, bool expectedHandleCreated)
    {
        using Form control = new()
        {
            AllowTransparency = allowTransparency,
            Opacity = value
        };
        Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
        Assert.Equal(expected, control.Opacity);
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.Equal(expectedHandleCreated, control.IsHandleCreated);

        // Set same.
        control.Opacity = value;
        Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
        Assert.Equal(expected, control.Opacity);
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.Equal(expectedHandleCreated, control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Opacity_SetWithTransparencyKey_TestData()
    {
        foreach (bool allowTransparency in new bool[] { true, false })
        {
            yield return new object[] { allowTransparency, 1.1, 1.0 };
            yield return new object[] { allowTransparency, 1.0, 1.0 };
            yield return new object[] { allowTransparency, 0.5, 0.5 };
            yield return new object[] { allowTransparency, 0, 0 };
            yield return new object[] { allowTransparency, -0.1, 0 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Opacity_SetWithTransparencyKey_TestData))]
    public void Form_Opacity_SetWithTransparencyKey_GetReturnsExpected(bool allowTransparency, double value, double expected)
    {
        using Form control = new()
        {
            AllowTransparency = allowTransparency,
            TransparencyKey = Color.Red,
            Opacity = value
        };
        Assert.True(control.AllowTransparency);
        Assert.Equal(expected, control.Opacity);
        Assert.Equal(Color.Red, control.TransparencyKey);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Opacity = value;
        Assert.True(control.AllowTransparency);
        Assert.Equal(expected, control.Opacity);
        Assert.Equal(Color.Red, control.TransparencyKey);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Opacity_Set_TestData))]
    public void Form_Opacity_SetTopLevel_GetReturnsExpected(bool allowTransparency, double value, double expected, bool expectedAllowTransparency, bool expectedHandleCreated)
    {
        using Form control = new()
        {
            TopLevel = false,
            AllowTransparency = allowTransparency,
            Opacity = value
        };
        Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
        Assert.Equal(expected, control.Opacity);
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.Equal(expectedHandleCreated, control.IsHandleCreated);

        // Set same.
        control.Opacity = value;
        Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
        Assert.Equal(expected, control.Opacity);
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.Equal(expectedHandleCreated, control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Opacity_SetNotTransparentWithHandle_TestData()
    {
        foreach (bool allowTransparency in new bool[] { true, false })
        {
            yield return new object[] { allowTransparency, 1.1, 1.0 };
            yield return new object[] { allowTransparency, 1.0, 1.0 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Opacity_SetNotTransparentWithHandle_TestData))]
    public void Form_Opacity_SetNotTransparentWithHandle_GetReturnsExpected(bool allowTransparency, double value, double expected)
    {
        using Form control = new();
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
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Opacity = value;
        Assert.Equal(allowTransparency, control.AllowTransparency);
        Assert.Equal(expected, control.Opacity);
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(0.5, 0.5, 0)]
    [InlineData(0, 0, 0)]
    [InlineData(-0.1, 0, 0)]
    public void Form_Opacity_SetTransparentWithHandleSetAllowTransparencyBefore_GetReturnsExpected(float value, float expected, int expectedStyleChangedCallCount)
    {
        using Form control = new()
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
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        control.Opacity = value;
        Assert.True(control.AllowTransparency);
        Assert.Equal(expected, control.Opacity);
        Assert.Equal(Color.Empty, control.TransparencyKey);
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

        yield return new object[] { false, 0.5, 0.5, 1 };
        yield return new object[] { false, 0, 0, 1 };
        yield return new object[] { false, -0.1, -0, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Opacity_SetTransparentWithHandle_TestData))]
    public void Form_Opacity_SetTransparentWithHandleSetAllowTransparencyAfter_GetReturnsExpected(bool allowTransparency, float value, float expected, int expectedStyleChangedCallCount)
    {
        using Form control = new();
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
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        control.Opacity = value;
        Assert.True(control.AllowTransparency);
        Assert.Equal(expected, control.Opacity);
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> Opacity_SetWithTransparencyKeyWithHandle_TestData()
    {
        foreach (bool allowTransparency in new bool[] { true, false })
        {
            yield return new object[] { allowTransparency, 1.1, 1.0 };
            yield return new object[] { allowTransparency, 1.0, 1.0 };
            yield return new object[] { allowTransparency, 0.5, 0.5 };
            yield return new object[] { allowTransparency, 0, 0 };
            yield return new object[] { allowTransparency, -0.1, 0 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Opacity_SetWithTransparencyKeyWithHandle_TestData))]
    public void Form_Opacity_SetWithTransparencyKeyWithHandle_GetReturnsExpected(bool allowTransparency, double value, double expected)
    {
        using Form control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.AllowTransparency = allowTransparency;
        control.TransparencyKey = Color.Red;
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Opacity = value;
        Assert.True(control.AllowTransparency);
        Assert.Equal(expected, control.Opacity);
        Assert.Equal(Color.Red, control.TransparencyKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Opacity = value;
        Assert.True(control.AllowTransparency);
        Assert.Equal(expected, control.Opacity);
        Assert.Equal(Color.Red, control.TransparencyKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> Opacity_SetTopLevelWithHandle_TestData()
    {
        yield return new object[] { true, 1.1, 1.0, true, 0 };
        yield return new object[] { true, 1.0, 1.0, true, 0 };
        yield return new object[] { true, 0.5, 0.5, true, 0 };
        yield return new object[] { true, 0, 0, true, 0 };
        yield return new object[] { true, -0.1, 0, true, 0 };

        yield return new object[] { false, 1.1, 1.0, false, 0 };
        yield return new object[] { false, 1.0, 1.0, false, 0 };
        yield return new object[] { false, 0.5, 0.5, true, 1 };
        yield return new object[] { false, 0, 0, true, 1 };
        yield return new object[] { false, -0.1, 0, true, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Opacity_SetTopLevelWithHandle_TestData))]
    public void Form_Opacity_SetTopLevelWithHandle_GetReturnsExpected(bool allowTransparency, double value, double expected, bool expectedAllowTransparency, int expectedStyleChangedCallCount)
    {
        using Form control = new()
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
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Opacity = value;
        Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
        Assert.Equal(expected, control.Opacity);
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> Parent_Set_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new Control() };
        yield return new object[] { new Form() };
    }

    [WinFormsTheory]
    [MemberData(nameof(Parent_Set_TestData))]
    public void Form_Parent_Set_GetReturnsExpected(Control value)
    {
        using Form control = new()
        {
            TopLevel = false,
            Parent = value
        };
        Assert.Same(value, control.Parent);
        Assert.Null(control.MdiParent);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Parent = value;
        Assert.Same(value, control.Parent);
        Assert.Null(control.MdiParent);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Parent_Set_TestData))]
    public void Form_Parent_SetWithNonNullOldParent_GetReturnsExpected(Control value)
    {
        using Control oldParent = new();
        using Form control = new()
        {
            TopLevel = false,
            Parent = oldParent
        };

        control.Parent = value;
        Assert.Same(value, control.Parent);
        Assert.Null(control.MdiParent);
        Assert.Empty(oldParent.Controls);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Parent = value;
        Assert.Same(value, control.Parent);
        Assert.Null(control.MdiParent);
        Assert.Empty(oldParent.Controls);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Form_Restore_RestoresPosition_User()
    {
        if (!OsVersion.IsWindows11_OrGreater())
        {
            return;
        }

        using Form form = new();
        form.Show();

        form.Location = new Point(10, 11);
        form.Size = new Size(200, 210);

        PInvokeCore.SendMessage(form, PInvokeCore.WM_SYSCOMMAND, (WPARAM)PInvoke.SC_MAXIMIZE);

        form.Location = new Point(20, 21);
        form.Size = new Size(300, 310);

        Assert.Equal(FormWindowState.Maximized, form.WindowState);
        Assert.NotEqual(new Point(20, 21), form.Location);
        Assert.NotEqual(new Size(300, 310), form.Size);

        PInvokeCore.SendMessage(form, PInvokeCore.WM_SYSCOMMAND, (WPARAM)PInvoke.SC_RESTORE);

        Assert.Equal(new Point(20, 21), form.Location);
        Assert.Equal(new Size(300, 310), form.Size);
    }

    [WinFormsFact]
    public void Form_Restore_RestoresPosition_WindowState()
    {
        if (!OsVersion.IsWindows11_OrGreater())
        {
            return;
        }

        using Form form = new();
        form.Show();

        form.Location = new Point(10, 11);
        form.Size = new Size(200, 210);

        form.WindowState = FormWindowState.Maximized;

        form.Location = new Point(20, 21);
        form.Size = new Size(300, 310);

        Assert.Equal(FormWindowState.Maximized, form.WindowState);
        Assert.NotEqual(new Point(20, 21), form.Location);
        Assert.NotEqual(new Size(300, 310), form.Size);

        form.WindowState = FormWindowState.Normal;

        Assert.Equal(new Point(20, 21), form.Location);
        Assert.Equal(new Size(300, 310), form.Size);
    }

    [WinFormsTheory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public static void ShowIcon_renders_icon_correctly(bool showIcon, bool expectedIconNull)
    {
        using Form form = new();
        Assert.NotEqual(0, form.Handle);

        form.ShowIcon = showIcon;

        HICON hSmallIcon = (HICON)PInvokeCore.SendMessage(form, PInvokeCore.WM_GETICON, (WPARAM)PInvoke.ICON_SMALL);
        HICON hLargeIcon = (HICON)PInvokeCore.SendMessage(form, PInvokeCore.WM_GETICON, (WPARAM)PInvoke.ICON_BIG);
        Assert.Equal(expectedIconNull, hSmallIcon.IsNull);
        Assert.Equal(expectedIconNull, hLargeIcon.IsNull);
    }

    [WinFormsFact]
    public static void ShowIcon_false_should_render_no_icon()
    {
        using Form form = new();
        Assert.True(form.Handle != IntPtr.Zero);

        WINDOW_EX_STYLE extendedStyle = unchecked((WINDOW_EX_STYLE)(long)PInvokeCore.GetWindowLong(form, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE));
        Assert.False(extendedStyle.HasFlag(WINDOW_EX_STYLE.WS_EX_DLGMODALFRAME));

        form.ShowIcon = false;

        // hiding icon sets WS_EX.DLGMODALFRAME
        extendedStyle = unchecked((WINDOW_EX_STYLE)(long)PInvokeCore.GetWindowLong(form, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE));
        Assert.True(extendedStyle.HasFlag(WINDOW_EX_STYLE.WS_EX_DLGMODALFRAME));
    }

    public static IEnumerable<object[]> Parent_SetMdiChild_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new Control() };
    }

    [WinFormsTheory]
    [MemberData(nameof(Parent_SetMdiChild_TestData))]
    public void Form_Parent_SetMdiChild_GetReturnsExpected(Control value)
    {
        using Form oldParent = new()
        {
            IsMdiContainer = true
        };
        using Form control = new()
        {
            MdiParent = oldParent,
            Parent = value
        };

        control.Parent = value;
        Assert.Same(value, control.Parent);
        Assert.Null(control.MdiParent);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Parent = value;
        Assert.Same(value, control.Parent);
        Assert.Null(control.MdiParent);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Form_Parent_SetNonNull_AddsToControls()
    {
        using Control parent = new();
        using Form control = new()
        {
            TopLevel = false,
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
    public void Form_Parent_SetWithHandle_GetReturnsExpected(Control value)
    {
        using Form control = new()
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

        control.Parent = value;
        Assert.Same(value, control.Parent);
        Assert.Null(control.MdiParent);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Parent = value;
        Assert.Same(value, control.Parent);
        Assert.Null(control.MdiParent);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Form_Parent_SetWithHandler_CallsParentChanged()
    {
        using Control parent = new();
        using Form control = new()
        {
            TopLevel = false
        };
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
    public void Form_Parent_SetSame_ThrowsArgumentException()
    {
        using Form control = new()
        {
            TopLevel = false
        };
        Assert.Throws<ArgumentException>(() => control.Parent = control);
        Assert.Null(control.Parent);
    }

    [WinFormsFact]
    public void Form_Parent_SetTopLevel_ThrowsArgumentException()
    {
        using Form control = new();
        using Control parent = new();
        Assert.Throws<ArgumentException>(() => control.Parent = parent);
        Assert.Null(control.Parent);
    }

    [WinFormsFact]
    public void Form_Parent_SetFormWithMdiParent_ThrowsArgumentException()
    {
        using Form oldParent = new()
        {
            IsMdiContainer = true
        };
        using Form control = new()
        {
            MdiParent = oldParent
        };
        using Form parent = new();
        Assert.Throws<ArgumentException>("value", () => control.Parent = parent);
        Assert.NotNull(control.Parent);
        Assert.Same(oldParent, control.MdiParent);
    }

    [WinFormsFact]
    public void Form_Parent_ShowIconInMaximized()
    {
        using Form parent = new();
        using MenuStrip menuStrip = new();
        parent.Controls.Add(menuStrip);
        parent.IsMdiContainer = true;
        parent.MainMenuStrip = menuStrip;
        parent.Show();
        Assert.True(parent.Handle != IntPtr.Zero);
        using Form control = new();
        control.MdiParent = parent;
        control.Icon = Form.DefaultIcon;
        Assert.True(control.Handle != IntPtr.Zero);
        control.Show();

        control.ShowIcon = false;
        HICON hSmallIcon = (HICON)PInvokeCore.SendMessage(control, PInvokeCore.WM_GETICON, (WPARAM)PInvoke.ICON_SMALL);
        Assert.True(hSmallIcon.IsNull);
        HICON hLargeIcon = (HICON)PInvokeCore.SendMessage(control, PInvokeCore.WM_GETICON, (WPARAM)PInvoke.ICON_BIG);
        Assert.True(hLargeIcon.IsNull);

        control.WindowState = FormWindowState.Maximized;
        control.ShowIcon = false;
        hSmallIcon = (HICON)PInvokeCore.SendMessage(control, PInvokeCore.WM_GETICON, (WPARAM)PInvoke.ICON_SMALL);
        Assert.True(hSmallIcon.IsNull);
        hLargeIcon = (HICON)PInvokeCore.SendMessage(control, PInvokeCore.WM_GETICON, (WPARAM)PInvoke.ICON_BIG);
        Assert.True(hLargeIcon.IsNull);
        Assert.False(menuStrip.Items[0].Visible);

        control.ShowIcon = true;
        hSmallIcon = (HICON)PInvokeCore.SendMessage(control, PInvokeCore.WM_GETICON, (WPARAM)PInvoke.ICON_SMALL);
        Assert.False(hSmallIcon.IsNull);
        hLargeIcon = (HICON)PInvokeCore.SendMessage(control, PInvokeCore.WM_GETICON, (WPARAM)PInvoke.ICON_BIG);
        Assert.False(hLargeIcon.IsNull);
        Assert.True(menuStrip.Items[0].Visible);
    }

    public static IEnumerable<object[]> TransparencyKey_Set_TestData()
    {
        foreach (bool allowTransparency in new bool[] { true, false })
        {
            yield return new object[] { allowTransparency, Color.Empty, allowTransparency };
            yield return new object[] { allowTransparency, Color.Red, true };
            yield return new object[] { allowTransparency, Color.Transparent, true };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(TransparencyKey_Set_TestData))]
    public void Form_TransparencyKey_Set_GetReturnsExpected(bool allowTransparency, Color value, bool expectedAllowTransparency)
    {
        using Form control = new()
        {
            AllowTransparency = allowTransparency,
            TransparencyKey = value
        };
        Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.Equal(value, control.TransparencyKey);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.TransparencyKey = value;
        Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.Equal(value, control.TransparencyKey);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> TransparencyKey_SetWithCustomOldValue_TestData()
    {
        foreach (bool allowTransparency in new bool[] { true, false })
        {
            yield return new object[] { allowTransparency, Color.Empty };
            yield return new object[] { allowTransparency, Color.Red };
            yield return new object[] { allowTransparency, Color.Transparent };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(TransparencyKey_SetWithCustomOldValue_TestData))]
    public void Form_TransparencyKey_SetWithCustomOldValue_GetReturnsExpected(bool allowTransparency, Color value)
    {
        using Form control = new()
        {
            AllowTransparency = allowTransparency,
            TransparencyKey = Color.Yellow
        };

        control.TransparencyKey = value;
        Assert.True(control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.Equal(value, control.TransparencyKey);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.TransparencyKey = value;
        Assert.True(control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.Equal(value, control.TransparencyKey);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> TransparencyKey_SetWithOpacity_TestData()
    {
        foreach (bool allowTransparency in new bool[] { true, false })
        {
            yield return new object[] { allowTransparency, Color.Empty };
            yield return new object[] { allowTransparency, Color.Red };
            yield return new object[] { allowTransparency, Color.Transparent };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(TransparencyKey_SetWithOpacity_TestData))]
    public void Form_TransparencyKey_SetWithOpacity_GetReturnsExpected(bool allowTransparency, Color value)
    {
        using Form control = new()
        {
            AllowTransparency = allowTransparency,
            Opacity = 0.5,
            TransparencyKey = value
        };
        Assert.True(control.AllowTransparency);
        Assert.Equal(0.5, control.Opacity);
        Assert.Equal(value, control.TransparencyKey);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.TransparencyKey = value;
        Assert.True(control.AllowTransparency);
        Assert.Equal(0.5, control.Opacity);
        Assert.Equal(value, control.TransparencyKey);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> TransparencyKey_SetMdiContainer_TestData()
    {
        foreach (bool allowTransparency in new bool[] { true, false })
        {
            yield return new object[] { allowTransparency, Color.Empty };
            yield return new object[] { allowTransparency, Color.Red };
            yield return new object[] { allowTransparency, Color.Transparent };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(TransparencyKey_SetMdiContainer_TestData))]
    public void Form_TransparencyKey_SetMdiContainer_Nop(bool allowTransparency, Color value)
    {
        using Form control = new()
        {
            IsMdiContainer = true,
            AllowTransparency = allowTransparency,
            TransparencyKey = value
        };
        Assert.Equal(allowTransparency, control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.Equal(value, control.TransparencyKey);
        Assert.False(control.IsHandleCreated);

        // Set same.
        Assert.Equal(allowTransparency, control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.Equal(value, control.TransparencyKey);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(TransparencyKey_Set_TestData))]
    public void Form_TransparencyKey_SetNotTopLevel_GetReturnsExpected(bool allowTransparency, Color value, bool expectedAllowTransparency)
    {
        using Form control = new()
        {
            TopLevel = true,
            AllowTransparency = allowTransparency,
            TransparencyKey = value
        };
        Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.Equal(value, control.TransparencyKey);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.TransparencyKey = value;
        Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.Equal(value, control.TransparencyKey);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Form_TransparencyKey_SetNotTransparentWithHandleSetNotAllowTransparency_GetReturnsExpected()
    {
        using Form control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.TransparencyKey = Color.Empty;
        Assert.False(control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.TransparencyKey = Color.Empty;
        Assert.False(control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> TransparencyKey_SetWithHandleSetNotAllowTransparency_TestData()
    {
        yield return new object[] { Color.Red, true, 2 };
        yield return new object[] { Color.Transparent, true, 2 };
    }

    [WinFormsTheory]
    [MemberData(nameof(TransparencyKey_SetWithHandleSetNotAllowTransparency_TestData))]
    public void Form_TransparencyKey_SetTransparentWithHandleSetNotAllowTransparency_GetReturnsExpected(Color value, bool expectedAllowTransparency, int expectedStyleChangedCallCount)
    {
        using Form control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.TransparencyKey = value;
        Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.Equal(value, control.TransparencyKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.TransparencyKey = value;
        Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.Equal(value, control.TransparencyKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Form_TransparencyKey_SetNotTransparentWithHandleSetAllowTransparencyBefore_GetReturnsExpected()
    {
        using Form control = new()
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

        control.TransparencyKey = Color.Empty;
        Assert.True(control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(1, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.TransparencyKey = Color.Empty;
        Assert.True(control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(1, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> TransparencyKey_SetWithHandleSetAllowTransparency_TestData()
    {
        yield return new object[] { Color.Red };
        yield return new object[] { Color.Transparent };
    }

    [WinFormsTheory]
    [MemberData(nameof(TransparencyKey_SetWithHandleSetAllowTransparency_TestData))]
    public void Form_TransparencyKey_SetTransparentWithHandleSetAllowTransparencyBefore_GetReturnsExpected(Color value)
    {
        using Form control = new()
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

        control.TransparencyKey = value;
        Assert.True(control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.Equal(value, control.TransparencyKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.TransparencyKey = value;
        Assert.True(control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.Equal(value, control.TransparencyKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Form_TransparencyKey_SetNotTransparentWithHandleSetAllowTransparencyAfter_GetReturnsExpected()
    {
        using Form control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.AllowTransparency = true;
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.TransparencyKey = Color.Empty;
        Assert.True(control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(1, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.TransparencyKey = Color.Empty;
        Assert.True(control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(1, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(TransparencyKey_SetWithHandleSetAllowTransparency_TestData))]
    public void Form_TransparencyKey_SetTransparentWithHandleSetAllowTransparencyAfter_GetReturnsExpected(Color value)
    {
        using Form control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.AllowTransparency = true;
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.TransparencyKey = value;
        Assert.True(control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.Equal(value, control.TransparencyKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.TransparencyKey = value;
        Assert.True(control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.Equal(value, control.TransparencyKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Form_TransparencyKey_SetNotTransparentWithHandleSetOpacityBefore_GetReturnsExpected()
    {
        using Form control = new()
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

        control.TransparencyKey = Color.Empty;
        Assert.True(control.AllowTransparency);
        Assert.Equal(0.5, control.Opacity);
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.TransparencyKey = Color.Empty;
        Assert.True(control.AllowTransparency);
        Assert.Equal(0.5, control.Opacity);
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(TransparencyKey_SetWithHandleSetAllowTransparency_TestData))]
    public void Form_TransparencyKey_SetTransparentWithHandleSetOpacityBefore_GetReturnsExpected(Color value)
    {
        using Form control = new()
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

        control.TransparencyKey = value;
        Assert.True(control.AllowTransparency);
        Assert.Equal(0.5, control.Opacity);
        Assert.Equal(value, control.TransparencyKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.TransparencyKey = value;
        Assert.True(control.AllowTransparency);
        Assert.Equal(0.5, control.Opacity);
        Assert.Equal(value, control.TransparencyKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Form_TransparencyKey_SetNotTransparentWithHandleSetOpacityAfter_GetReturnsExpected()
    {
        using Form control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.Opacity = 0.5;
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.TransparencyKey = Color.Empty;
        Assert.True(control.AllowTransparency);
        Assert.Equal(0.5, control.Opacity);
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.TransparencyKey = Color.Empty;
        Assert.True(control.AllowTransparency);
        Assert.Equal(0.5, control.Opacity);
        Assert.Equal(Color.Empty, control.TransparencyKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(TransparencyKey_SetWithHandleSetAllowTransparency_TestData))]
    public void Form_TransparencyKey_SetTransparentWithHandleSetOpacityAfter_GetReturnsExpected(Color value)
    {
        using Form control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.Opacity = 0.5;
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.TransparencyKey = value;
        Assert.True(control.AllowTransparency);
        Assert.Equal(0.5, control.Opacity);
        Assert.Equal(value, control.TransparencyKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.TransparencyKey = value;
        Assert.True(control.AllowTransparency);
        Assert.Equal(0.5, control.Opacity);
        Assert.Equal(value, control.TransparencyKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> TransparencyKey_SetNotTopLevelWithHandle_TestData()
    {
        yield return new object[] { true, Color.Empty, true, 1 };
        yield return new object[] { true, Color.Red, true, 0 };
        yield return new object[] { true, Color.Transparent, true, 0 };

        yield return new object[] { false, Color.Empty, false, 0 };
        yield return new object[] { false, Color.Red, true, 2 };
        yield return new object[] { false, Color.Transparent, true, 2 };
    }

    [WinFormsTheory]
    [MemberData(nameof(TransparencyKey_SetNotTopLevelWithHandle_TestData))]
    public void Form_TransparencyKey_SetNotTopLevelWithHandle_GetReturnsExpected(bool allowTransparency, Color value, bool expectedAllowTransparency, int expectedStyleChangedCallCount)
    {
        using Form control = new()
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

        control.TransparencyKey = value;
        Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.Equal(value, control.TransparencyKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.TransparencyKey = value;
        Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
        Assert.Equal(1, control.Opacity);
        Assert.Equal(value, control.TransparencyKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Form_ShowInTaskbar_SetFalse_GetReturnsExpected()
    {
        // Regression test for https://github.com/dotnet/winforms/issues/6421

        using Form form = new()
        {
            ShowInTaskbar = true,
        };

        DialogResult expectedDialogResult = DialogResult.OK;

        form.Load += (object sender, EventArgs e) =>
        {
            IntPtr formHandle = form.Handle;
            form.ShowInTaskbar = false;

            Assert.True(form.IsHandleCreated);
            Assert.NotEqual(formHandle, form.Handle);
        };

        form.Shown += (object sender, EventArgs e) =>
        {
            form.DialogResult = expectedDialogResult;
        };

        Assert.Equal(expectedDialogResult, form.ShowDialog());
        Assert.Equal(expectedDialogResult, form.DialogResult);
    }

    public static IEnumerable<object[]> Visible_Set_TestData()
    {
        foreach (DialogResult dialogResult in Enum.GetValues(typeof(DialogResult)))
        {
            yield return new object[] { dialogResult, true };
            yield return new object[] { dialogResult, false };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Visible_Set_TestData))]
    public void Form_Visible_Set_GetReturnsExpected(DialogResult dialogResult, bool value)
    {
        using Form control = new()
        {
            DialogResult = dialogResult,
            Visible = value
        };
        Assert.Equal(value, control.Visible);
        Assert.Equal(value, control.IsHandleCreated);

        // Set same.
        control.Visible = value;
        Assert.Equal(value, control.Visible);
        Assert.Equal(value, control.IsHandleCreated);

        // Set different.
        control.Visible = !value;
        Assert.Equal(!value, control.Visible);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.Visible = !value;
        Assert.Equal(!value, control.Visible);
        Assert.True(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Visible_SetMdiChild_TestData()
    {
        foreach (DialogResult dialogResult in Enum.GetValues(typeof(DialogResult)))
        {
            yield return new object[] { dialogResult, true, true, true };
            yield return new object[] { dialogResult, true, false, false };
            yield return new object[] { dialogResult, false, true, false };
            yield return new object[] { dialogResult, false, false, false };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Visible_SetMdiChild_TestData))]
    public void Form_Visible_SetMdiChild_GetReturnsExpected(DialogResult dialogResult, bool parentVisible, bool value, bool expected)
    {
        using Form parent = new()
        {
            IsMdiContainer = true,
            Visible = parentVisible
        };
        using Form control = new()
        {
            MdiParent = parent,
            DialogResult = dialogResult,
            Visible = value
        };
        Assert.Equal(expected, control.Visible);
        Assert.Equal(expected, control.IsHandleCreated);

        // Set same.
        control.Visible = value;
        Assert.Equal(expected, control.Visible);
        Assert.Equal(expected, control.IsHandleCreated);

        // Set different.
        control.Visible = !value;
        Assert.Equal(!expected && parentVisible, control.Visible);
        Assert.Equal(!expected && parentVisible, control.IsHandleCreated);

        // Set same.
        control.Visible = !value;
        Assert.Equal(!expected && parentVisible, control.Visible);
        Assert.Equal(!expected && parentVisible, control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Visible_Set_TestData))]
    public void Form_Visible_SetWithHandle_GetReturnsExpected(DialogResult dialogResult, bool value)
    {
        using Form control = new()
        {
            DialogResult = dialogResult
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

        // Set same.
        control.Visible = !value;
        Assert.Equal(!value, control.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [EnumData<DialogResult>]
    public void Form_Visible_SetTrueMdiChildVisibleWithHandle_GetReturnsExpected(DialogResult dialogResult)
    {
        using Form parent = new()
        {
            IsMdiContainer = true,
            Visible = true
        };
        using Form control = new()
        {
            MdiParent = parent,
            DialogResult = dialogResult
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Visible = true;
        Assert.Equal(dialogResult is DialogResult.OK or DialogResult.None, control.Visible);
        Assert.Equal(dialogResult is DialogResult.OK or DialogResult.None, control.IsHandleCreated);

        // Set same.
        control.Visible = true;
        Assert.True(control.Visible);
        Assert.True(control.IsHandleCreated);

        // Set different.
        control.Visible = false;
        Assert.False(control.Visible);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Visible = false;
        Assert.False(control.Visible);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.Visible = true;
        Assert.Equal(dialogResult != DialogResult.OK, control.Visible);
        Assert.Equal(dialogResult != DialogResult.OK, control.IsHandleCreated);

        // Set same.
        control.Visible = true;
        Assert.True(control.Visible);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [EnumData<DialogResult>]
    public void Form_Visible_SetFalseMdiChildVisibleWithHandle_GetReturnsExpected(DialogResult dialogResult)
    {
        using Form parent = new()
        {
            IsMdiContainer = true,
            Visible = true
        };
        using Form control = new()
        {
            MdiParent = parent,
            DialogResult = dialogResult
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        // Set different.
        control.Visible = false;
        Assert.False(control.Visible);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Visible = false;
        Assert.False(control.Visible);
        Assert.False(control.IsHandleCreated);

        control.Visible = true;
        Assert.Equal(dialogResult == DialogResult.None, control.Visible);
        Assert.Equal(dialogResult == DialogResult.None, control.IsHandleCreated);

        // Set same.
        control.Visible = true;
        Assert.True(control.Visible);
        Assert.True(control.IsHandleCreated);

        // Set different.
        control.Visible = false;
        Assert.False(control.Visible);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Visible = false;
        Assert.False(control.Visible);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Visible_Set_TestData))]
    public void Form_Visible_SetMdiChildNotVisibleWithHandle_GetReturnsExpected(DialogResult dialogResult, bool value)
    {
        using Form parent = new()
        {
            IsMdiContainer = true
        };
        using Form control = new()
        {
            MdiParent = parent,
            DialogResult = dialogResult,
            Visible = value
        };
        Assert.False(control.Visible);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Visible = value;
        Assert.False(control.Visible);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.Visible = !value;
        Assert.False(control.Visible);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Visible = !value;
        Assert.False(control.Visible);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Visible_SetWithHandler_TestData()
    {
        foreach (DialogResult dialogResult in Enum.GetValues(typeof(DialogResult)))
        {
            yield return new object[] { dialogResult, true, 1 };
            yield return new object[] { dialogResult, false, 0 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Visible_SetWithHandler_TestData))]
    public void Form_Visible_SetTrueWithHandler_CallsVisibleChanged(DialogResult dialogResult, bool value, int expectedLoadCallCount)
    {
        using Form control = new()
        {
            DialogResult = dialogResult
        };
        int loadCallCount = 0;
        control.Load += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            loadCallCount++;
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.VisibleChanged += handler;

        // Set different.
        control.Visible = value;
        Assert.Equal(value, control.Visible);
        Assert.Equal(expectedLoadCallCount, callCount);
        Assert.Equal(expectedLoadCallCount, loadCallCount);

        // Set same.
        control.Visible = value;
        Assert.Equal(value, control.Visible);
        Assert.Equal(expectedLoadCallCount, callCount);
        Assert.Equal(expectedLoadCallCount, loadCallCount);

        // Set different.
        control.Visible = !value;
        Assert.Equal(!value, control.Visible);
        Assert.Equal(expectedLoadCallCount + 1, callCount);
        Assert.Equal(1, loadCallCount);

        // Set same.
        control.Visible = !value;
        Assert.Equal(!value, control.Visible);
        Assert.Equal(expectedLoadCallCount + 1, callCount);
        Assert.Equal(1, loadCallCount);

        // Remove handler.
        control.VisibleChanged -= handler;
        control.Visible = value;
        Assert.Equal(value, control.Visible);
        Assert.Equal(expectedLoadCallCount + 1, callCount);
        Assert.Equal(1, loadCallCount);
    }

    [WinFormsFact]
    public void Form_Visible_SetWithHandlerMdiChildVisible_CallsVisibleChanged()
    {
        using Form parent = new()
        {
            IsMdiContainer = true,
            Visible = true
        };
        using Form control = new()
        {
            MdiParent = parent
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.VisibleChanged += handler;

        // Set different.
        control.Visible = true;
        Assert.True(control.Visible);
        Assert.Equal(1, callCount);

        // Set same.
        control.Visible = true;
        Assert.True(control.Visible);
        Assert.Equal(1, callCount);

        // Set different.
        control.Visible = false;
        Assert.False(control.Visible);
        Assert.Equal(3, callCount);

        // Set same.
        control.Visible = false;
        Assert.False(control.Visible);
        Assert.Equal(3, callCount);

        // Set different.
        control.Visible = true;
        Assert.True(control.Visible);
        Assert.Equal(4, callCount);

        // Remove handler.
        control.VisibleChanged -= handler;
        control.Visible = false;
        Assert.False(control.Visible);
        Assert.Equal(4, callCount);
    }

    [WinFormsFact]
    public void Form_Visible_SetWithHandlerMdiChildNotVisible_CallsVisibleChanged()
    {
        using Form parent = new()
        {
            IsMdiContainer = true
        };
        using Form control = new()
        {
            MdiParent = parent
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.VisibleChanged += handler;

        // Set different.
        control.Visible = true;
        Assert.False(control.Visible);
        Assert.Equal(1, callCount);

        // Set same.
        control.Visible = true;
        Assert.False(control.Visible);
        Assert.Equal(2, callCount);

        // Set different.
        control.Visible = false;
        Assert.False(control.Visible);
        Assert.Equal(2, callCount);

        // Set same.
        control.Visible = false;
        Assert.False(control.Visible);
        Assert.Equal(2, callCount);

        // Set different.
        control.Visible = true;
        Assert.False(control.Visible);
        Assert.Equal(3, callCount);

        // Remove handler.
        control.VisibleChanged -= handler;
        control.Visible = false;
        Assert.False(control.Visible);
        Assert.Equal(3, callCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(Visible_SetWithHandler_TestData))]
    public void Form_Visible_SetTrueWithHandlerWithHandle_CallsVisibleChanged(DialogResult dialogResult, bool value, int expectedLoadCallCount)
    {
        using Form control = new()
        {
            DialogResult = dialogResult
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        int loadCallCount = 0;
        control.Load += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            loadCallCount++;
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.VisibleChanged += handler;

        // Set different.
        control.Visible = value;
        Assert.Equal(value, control.Visible);
        Assert.Equal(expectedLoadCallCount, callCount);
        Assert.Equal(expectedLoadCallCount, loadCallCount);

        // Set same.
        control.Visible = value;
        Assert.Equal(value, control.Visible);
        Assert.Equal(expectedLoadCallCount, callCount);
        Assert.Equal(expectedLoadCallCount, loadCallCount);

        // Set different.
        control.Visible = !value;
        Assert.Equal(!value, control.Visible);
        Assert.Equal(expectedLoadCallCount + 1, callCount);
        Assert.Equal(1, loadCallCount);

        // Set same.
        control.Visible = !value;
        Assert.Equal(!value, control.Visible);
        Assert.Equal(expectedLoadCallCount + 1, callCount);
        Assert.Equal(1, loadCallCount);

        // Remove handler.
        control.VisibleChanged -= handler;
        control.Visible = value;
        Assert.Equal(value, control.Visible);
        Assert.Equal(expectedLoadCallCount + 1, callCount);
        Assert.Equal(1, loadCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void Form_Visible_SetWithHandlerWithHandle_CallsVisibleChanged(bool initialVisible)
    {
        using Form control = new()
        {
            Visible = initialVisible
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
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.VisibleChanged += handler;

        // Set different.
        control.Visible = !initialVisible;
        Assert.Equal(!initialVisible, control.Visible);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Visible = !initialVisible;
        Assert.Equal(!initialVisible, control.Visible);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.Visible = initialVisible;
        Assert.Equal(initialVisible, control.Visible);
        Assert.Equal(2, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.VisibleChanged -= handler;
        control.Visible = !initialVisible;
        Assert.Equal(!initialVisible, control.Visible);
        Assert.Equal(2, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Form_GetAutoSizeMode_Invoke_ReturnsExpected()
    {
        using SubForm control = new();
        Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
    }

    [WinFormsTheory]
    [InlineData(0, true)]
    [InlineData(SubForm.ScrollStateAutoScrolling, false)]
    [InlineData(SubForm.ScrollStateFullDrag, false)]
    [InlineData(SubForm.ScrollStateHScrollVisible, false)]
    [InlineData(SubForm.ScrollStateUserHasScrolled, false)]
    [InlineData(SubForm.ScrollStateVScrollVisible, false)]
    [InlineData(int.MaxValue, false)]
    [InlineData((-1), false)]
    public void Form_GetScrollState_Invoke_ReturnsExpected(int bit, bool expected)
    {
        using SubForm control = new();
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
    public void Form_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
    {
        using SubForm control = new();
        Assert.Equal(expected, control.GetStyle(flag));

        // Call again to test caching.
        Assert.Equal(expected, control.GetStyle(flag));
    }

    [WinFormsFact]
    public void Form_GetTopLevel_Invoke_ReturnsExpected()
    {
        using SubForm control = new();
        Assert.True(control.GetTopLevel());
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void Form_OnHandleCreated_Invoke_CallsHandleCreated(EventArgs eventArgs)
    {
        using SubForm control = new();
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
    public void Form_OnHandleCreated_InvokeWithHandle_CallsHandleCreated(EventArgs eventArgs)
    {
        using SubForm control = new();
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
    public void Form_OnHandleDestroyed_Invoke_CallsHandleDestroyed(EventArgs eventArgs)
    {
        using SubForm control = new();
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
    public void Form_OnHandleDestroyed_InvokeWithHandle_CallsHandleDestroyed(EventArgs eventArgs)
    {
        using SubForm control = new();
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

    public static IEnumerable<object[]> FormShow_TestData()
    {
        // Test with control passed as owner.
        yield return new Action<Form, Control>[]
        {
            (Form form, Control control) => form.Show(owner: control)
        };
        yield return new Action<Form, Control>[]
        {
            (Form form, Control control) => form.ShowDialog(owner: control)
        };

        // Test with parent form passed as owner.
        yield return new Action<Form, Control>[]
        {
            (Form form, Control control) => form.Show(owner: control.FindForm())
        };
        yield return new Action<Form, Control>[]
        {
            (Form form, Control control) => form.ShowDialog(owner: control.FindForm())
        };
    }

    [WinFormsTheory]
    [MemberData(nameof(FormShow_TestData))]
    public void Form_Show_SetsOwnerToTopLevelForm_WhenShownWithOwner(Action<Form, Control> showAction)
    {
        // Regression test for https://github.com/dotnet/winforms/issues/8280

        using Form parent = new();
        using Control control = new();
        parent.Controls.Add(control);
        parent.Show();

        using Form child = new();
        Form owner = null;
        child.Load += (object sender, EventArgs e) =>
        {
            owner = ((Form)sender).Owner;
            child.Close();
        };

        showAction(child, control);

        Assert.Same(parent, owner);
        Assert.False(child.IsHandleCreated);
    }

    [WinFormsFact]
    public void Form_ParentThenSetShowInTaskbarToFalse()
    {
        // Regression test for https://github.com/dotnet/winforms/issues/8803
        using ParentedForm form = new();
        form.Show();
    }

    [WinFormsFact]
    public void Form_MdiContainer_WithChild_DoubleDispose()
    {
        // Regression test for https://github.com/dotnet/winforms/issues/8990
        using Form parent = new() { IsMdiContainer = true };
        using Form mdiChild = new() { MdiParent = parent };
        parent.Show();
        mdiChild.Show();
        parent.Dispose();
        parent.Dispose();
    }

    [WinFormsFact]
    public void Form_DoesNot_PreventShutDown()
    {
        using SubForm form = new();
        var message = Message.Create(HWND.Null, PInvokeCore.WM_QUERYENDSESSION, wparam: default, lparam: default);
        form.WndProc(ref message);
        Assert.True((BOOL)message.ResultInternal);
    }

    public partial class ParentedForm : Form
    {
        private ParentingForm _parentForm;

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            _parentForm = new ParentingForm(this);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _parentForm?.Dispose();
            }
        }

        public class ParentingForm : Form
        {
            public ParentingForm(Form targetForm)
            {
                targetForm.Owner = this;
                RecreateHandle();
            }
        }
    }

    public class SubForm : Form
    {
        public new const int ScrollStateAutoScrolling = ScrollableControl.ScrollStateAutoScrolling;

        public new const int ScrollStateHScrollVisible = ScrollableControl.ScrollStateHScrollVisible;

        public new const int ScrollStateVScrollVisible = ScrollableControl.ScrollStateVScrollVisible;

        public new const int ScrollStateUserHasScrolled = ScrollableControl.ScrollStateUserHasScrolled;

        public new const int ScrollStateFullDrag = ScrollableControl.ScrollStateFullDrag;

        public new SizeF AutoScaleFactor => base.AutoScaleFactor;

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

        public new Rectangle MaximizedBounds => base.MaximizedBounds;

        public new bool ResizeRedraw
        {
            get => base.ResizeRedraw;
            set => base.ResizeRedraw = value;
        }

        public new bool ShowFocusCues => base.ShowFocusCues;

        public new bool ShowKeyboardCues => base.ShowKeyboardCues;

        public new bool ShowWithoutActivation => base.ShowWithoutActivation;

        public new bool HScroll
        {
            get => base.HScroll;
            set => base.HScroll = value;
        }

        public new bool VScroll
        {
            get => base.VScroll;
            set => base.VScroll = value;
        }

        public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

        public new bool GetScrollState(int bit) => base.GetScrollState(bit);

        public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

        public new bool GetTopLevel() => base.GetTopLevel();

        public new void OnHandleCreated(EventArgs e) => base.OnHandleCreated(e);

        public new void OnHandleDestroyed(EventArgs e) => base.OnHandleDestroyed(e);

        public new void WndProc(ref Message m) => base.WndProc(ref m);
    }
}
