// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms.TestUtilities;
using Microsoft.DotNet.RemoteExecutor;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Tests;

public class ContainerControlTests
{
    [WinFormsFact]
    public void ContainerControl_Ctor_Default()
    {
        using SubContainerControl control = new();
        Assert.Null(control.AccessibleDefaultActionDescription);
        Assert.Null(control.AccessibleDescription);
        Assert.Null(control.AccessibleName);
        Assert.Null(control.ActiveControl);
        Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
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
        Assert.Equal(AutoValidate.EnablePreventFocusChange, control.AutoValidate);
        Assert.Equal(Control.DefaultBackColor, control.BackColor);
        Assert.Null(control.BackgroundImage);
        Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
        Assert.NotNull(control.BindingContext);
        Assert.Same(control.BindingContext, control.BindingContext);
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
        Assert.Equal(SizeF.Empty, control.CurrentAutoScaleDimensions);
        Assert.Equal(Cursors.Default, control.Cursor);
        Assert.Equal(Cursors.Default, control.DefaultCursor);
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
    public void ContainerControl_CreateParams_GetDefault_ReturnsExpected()
    {
        using SubContainerControl control = new();
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

    [WinFormsFact]
    public void ContainerControl_ActiveContainerControl_Set_GetReturnsExpected()
    {
        using ContainerControl control = new();
        using Control child = new();
        Control grandchild = new();
        control.Controls.Add(child);
        child.Controls.Add(grandchild);

        control.ActiveControl = child;
        Assert.Same(child, control.ActiveControl);

        // Set same.
        control.ActiveControl = child;
        Assert.Same(child, control.ActiveControl);

        // Set grandchild.
        control.ActiveControl = grandchild;
        Assert.Same(grandchild, control.ActiveControl);

        // Set null.
        control.ActiveControl = null;
        Assert.Null(control.ActiveControl);
    }

    [WinFormsFact]
    public void ContainerControl_ActiveContainerControl_SetInvalid_ThrowsArgumentException()
    {
        using ContainerControl control = new();
        Assert.Throws<ArgumentException>("value", () => control.ActiveControl = control);
        Assert.Throws<ArgumentException>("value", () => control.ActiveControl = new Control());
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetSizeTheoryData), TestIncludeType.NoNegatives)]
    public void ContainerControl_AutoScaleDimensions_Set_GetReturnsExpected(Size value)
    {
        using ContainerControl control = new()
        {
            AutoScaleDimensions = value
        };
        Assert.Equal(value, control.AutoScaleDimensions);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AutoScaleDimensions = value;
        Assert.Equal(value, control.AutoScaleDimensions);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetSizeTheoryData), TestIncludeType.NoNegatives)]
    public void ContainerControl_AutoScaleDimensions_SetWithChildren_GetReturnsExpected(Size value)
    {
        using Control child = new();
        using ContainerControl control = new();
        control.Controls.Add(child);

        control.AutoScaleDimensions = value;
        Assert.Equal(value, control.AutoScaleDimensions);

        // Set same.
        control.AutoScaleDimensions = value;
        Assert.Equal(value, control.AutoScaleDimensions);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetSizeTheoryData), TestIncludeType.NoPositives)]
    public void ContainerControl_AutoScaleDimensions_SetInvalid_ThrowsArgumentOutOfRangeException(Size value)
    {
        using ContainerControl control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.AutoScaleDimensions = value);
    }

    [WinFormsTheory]
    [EnumData<AutoScaleMode>]
    public void ContainerControl_AutoScaleMode_Set_GetReturnsExpected(AutoScaleMode value)
    {
        using ContainerControl control = new()
        {
            AutoScaleMode = value
        };
        Assert.Equal(value, control.AutoScaleMode);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AutoScaleMode = value;
        Assert.Equal(value, control.AutoScaleMode);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> AutoScaleMode_SetWithCustomOldValue_TestData()
    {
        yield return new object[] { AutoScaleMode.None, SizeF.Empty };
        yield return new object[] { AutoScaleMode.Dpi, new SizeF(96, 96) };
        yield return new object[] { AutoScaleMode.Inherit, SizeF.Empty };
    }

    [WinFormsTheory]
    [MemberData(nameof(AutoScaleMode_SetWithCustomOldValue_TestData))]
    public void ContainerControl_AutoScaleMode_SetWithCustomOldValue_ResetsAutoScaleDimensions(AutoScaleMode value, SizeF expectedAutoScaleDimensions)
    {
        using ContainerControl control = new()
        {
            AutoScaleDimensions = new SizeF(1, 2),
            AutoScaleMode = AutoScaleMode.Font
        };

        control.AutoScaleMode = value;
        Assert.Equal(value, control.AutoScaleMode);
        Assert.Equal(expectedAutoScaleDimensions, control.AutoScaleDimensions);
        Assert.Equal(expectedAutoScaleDimensions, control.CurrentAutoScaleDimensions);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AutoScaleMode = value;
        Assert.Equal(value, control.AutoScaleMode);
        Assert.Equal(expectedAutoScaleDimensions, control.AutoScaleDimensions);
        Assert.Equal(expectedAutoScaleDimensions, control.CurrentAutoScaleDimensions);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InvalidEnumData<AutoScaleMode>]
    public void ContainerControl_AutoScaleMode_SetInvalid_ThrowsInvalidEnumArgumentException(AutoScaleMode value)
    {
        using ContainerControl control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.AutoScaleMode = value);
    }

    public static IEnumerable<object[]> AutoValidate_Set_TestData()
    {
        yield return new object[] { AutoValidate.Disable, AutoValidate.Disable };
        yield return new object[] { AutoValidate.EnablePreventFocusChange, AutoValidate.EnablePreventFocusChange };
        yield return new object[] { AutoValidate.EnableAllowFocusChange, AutoValidate.EnableAllowFocusChange };
        yield return new object[] { AutoValidate.Inherit, AutoValidate.EnablePreventFocusChange };
    }

    [WinFormsTheory]
    [MemberData(nameof(AutoValidate_Set_TestData))]
    public void UserControl_AutoValidate_Set_GetReturnsExpected(AutoValidate value, AutoValidate expected)
    {
        using ContainerControl control = new()
        {
            AutoValidate = value
        };
        Assert.Equal(expected, control.AutoValidate);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AutoValidate = value;
        Assert.Equal(expected, control.AutoValidate);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ContainerControl_AutoValidate_SetWithHandler_CallsAutoValidateChanged()
    {
        using ContainerControl control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.AutoValidateChanged += handler;

        // Set different.
        control.AutoValidate = AutoValidate.EnablePreventFocusChange;
        Assert.Equal(AutoValidate.EnablePreventFocusChange, control.AutoValidate);
        Assert.Equal(1, callCount);

        // Set same.
        control.AutoValidate = AutoValidate.EnablePreventFocusChange;
        Assert.Equal(AutoValidate.EnablePreventFocusChange, control.AutoValidate);
        Assert.Equal(1, callCount);

        // Set different.
        control.AutoValidate = AutoValidate.EnableAllowFocusChange;
        Assert.Equal(AutoValidate.EnableAllowFocusChange, control.AutoValidate);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.AutoValidateChanged -= handler;
        control.AutoValidate = AutoValidate.Disable;
        Assert.False(control.AutoSize);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<AutoValidate>]
    public void ContainerControl_AutoValidate_SetInvalidValue_ThrowsInvalidEnumArgumentException(AutoValidate value)
    {
        using ContainerControl control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.AutoValidate = value);
    }

    [WinFormsFact]
    public void ContainerControl_BindingContext_Set_GetReturnsExpected()
    {
        BindingContext value = [];
        using ContainerControl control = new()
        {
            BindingContext = value
        };
        Assert.Same(value, control.BindingContext);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BindingContext = value;
        Assert.Same(value, control.BindingContext);
        Assert.False(control.IsHandleCreated);

        // Set null.
        control.BindingContext = null;
        Assert.NotNull(control.BindingContext);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ContainerControl_BindingContext_SetWithHandler_CallsBindingContextChanged()
    {
        using ContainerControl control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.BindingContextChanged += handler;

        // Set different.
        BindingContext value1 = [];
        control.BindingContext = value1;
        Assert.Same(value1, control.BindingContext);
        Assert.Equal(1, callCount);

        // Set same.
        control.BindingContext = value1;
        Assert.Same(value1, control.BindingContext);
        Assert.Equal(1, callCount);

        // Set different.
        BindingContext value2 = [];
        control.BindingContext = value2;
        Assert.Equal(value2, control.BindingContext);
        Assert.Equal(2, callCount);

        // Remove handler.
        BindingContext value3 = [];
        control.BindingContextChanged -= handler;
        control.BindingContext = value3;
        Assert.Same(value3, control.BindingContext);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetFontTheoryData))]
    public void ContainerControl_Font_Set_GetReturnsExpected(Font value)
    {
        using SubContainerControl control = new()
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

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetFontTheoryData))]
    public void ContainerControl_Font_SetWithAutoScaleModeFont_GetReturnsExpected(Font value)
    {
        using SubContainerControl control = new()
        {
            AutoScaleMode = AutoScaleMode.Font
        };

        control.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.Equal(new Size(1, 1), control.AutoScaleFactor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.Equal(new Size(1, 1), control.AutoScaleFactor);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ContainerControl_Font_SetWithHandler_CallsFontChanged()
    {
        using ContainerControl control = new();
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
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void ContainerControl_Font_Set_ScalesInheritingContainers(bool nested, bool explicitFont)
    {
        using Font font = new(Control.DefaultFont.FontFamily, Control.DefaultFont.Size * 2);
        using ContainerControl parent = new()
        {
            AutoScaleMode = AutoScaleMode.Font
        };
        using ContainerControl container = new();
        if (explicitFont)
        {
            container.Font = Control.DefaultFont;
        }

        using ContainerControl child = new()
        {
            Bounds = new Rectangle(40, 50, 100, 80)
        };
        if (explicitFont && !nested)
        {
            child.Font = Control.DefaultFont;
        }

        using ContainerControl sibling = new()
        {
            Bounds = child.Bounds
        };
        parent.Controls.Add(container);
        (nested ? container.Controls : parent.Controls).Add(child);
        parent.Controls.Add(sibling);
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        Assert.NotEqual(IntPtr.Zero, container.Handle);
        Assert.NotEqual(IntPtr.Zero, child.Handle);
        Assert.NotEqual(IntPtr.Zero, sibling.Handle);
        parent.PerformAutoScale();
        SizeF dimensions = parent.CurrentAutoScaleDimensions;
        Rectangle bounds = child.Bounds;

        parent.Font = font;

        SizeF factor = ContainerControl.GetCurrentAutoScaleFactor(parent.CurrentAutoScaleDimensions, dimensions);
        Rectangle expected = new(
            (int)Math.Round(bounds.X * factor.Width),
            (int)Math.Round(bounds.Y * factor.Height),
            (int)Math.Round(bounds.Width * factor.Width),
            (int)Math.Round(bounds.Height * factor.Height));
        child.Bounds.Should().Be(expected);
        sibling.Bounds.Should().Be(expected);

        child.Font = Control.DefaultFont;
        child.Bounds.Should().Be(expected);
        child.ResetFont();
        child.Bounds.Should().Be(expected);
    }

    [Theory]
    [InlineData(AutoScaleMode.Font, false)]
    [InlineData(AutoScaleMode.Font, true)]
    [InlineData(AutoScaleMode.Dpi, false)]
    [InlineData(AutoScaleMode.Dpi, true)]
    public void ContainerControl_DpiChanged_ScalesNestedContainersOnce(AutoScaleMode autoScaleMode, bool explicitFont)
    {
        // DPI awareness must be set before any windows are created, without changing the test host's mode.
        using RemoteInvokeHandle handle = RemoteExecutor.Invoke((mode, fontSet) =>
        {
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2).Should().BeTrue();
            ScaleHelper.IsThreadPerMonitorV2Aware.Should().BeTrue();
            bool hasExplicitFont = bool.Parse(fontSet);
            using SubContainerControl parent = new()
            {
                AutoScaleMode = Enum.Parse<AutoScaleMode>(mode),
                Font = Control.DefaultFont
            };
            using SubContainerControl child = new()
            {
                Bounds = new Rectangle(40, 50, 100, 80)
            };
            if (hasExplicitFont)
            {
                child.Font = Control.DefaultFont;
            }

            using SubContainerControl grandchild = new()
            {
                Bounds = new Rectangle(20, 30, 40, 50)
            };
            parent.Controls.Add(child);
            child.Controls.Add(grandchild);
            Assert.NotEqual(IntPtr.Zero, parent.Handle);
            Assert.NotEqual(IntPtr.Zero, child.Handle);
            Assert.NotEqual(IntPtr.Zero, grandchild.Handle);
            parent.PerformAutoScale();
            SizeF dimensions = parent.CurrentAutoScaleDimensions;
            Rectangle childBounds = child.Bounds;
            Rectangle grandchildBounds = grandchild.Bounds;
            int childFontChanged = 0;
            int grandchildFontChanged = 0;
            child.FontChanged += (_, _) => childFontChanged++;
            grandchild.FontChanged += (_, _) => grandchildFontChanged++;
            int newDpi = parent.DeviceDpi * 2;

            Message grandchildMessage = Message.Create(grandchild.Handle, (int)PInvokeCore.WM_DPICHANGED_BEFOREPARENT, newDpi, 0);
            grandchild.WndProc(ref grandchildMessage);
            Message childMessage = Message.Create(child.Handle, (int)PInvokeCore.WM_DPICHANGED_BEFOREPARENT, newDpi, 0);
            child.WndProc(ref childMessage);

            child.IsDpiChangeScalingRequired.Should().Be(hasExplicitFont);
            child.Bounds.Should().Be(childBounds);
            grandchild.Bounds.Should().Be(grandchildBounds);
            childFontChanged.Should().Be(0);
            grandchildFontChanged.Should().Be(0);

            Message parentMessage = Message.Create(parent.Handle, (int)PInvokeCore.WM_DPICHANGED_BEFOREPARENT, newDpi, 0);
            parent.WndProc(ref parentMessage);

            SizeF factor = ContainerControl.GetCurrentAutoScaleFactor(parent.CurrentAutoScaleDimensions, dimensions);
            factor.Width.Should().BeGreaterThan(1);
            factor.Height.Should().BeGreaterThan(1);
            child.Bounds.Should().Be(new Rectangle(
                (int)Math.Round(childBounds.X * factor.Width),
                (int)Math.Round(childBounds.Y * factor.Height),
                (int)Math.Round(childBounds.Width * factor.Width),
                (int)Math.Round(childBounds.Height * factor.Height)));
            grandchild.Bounds.Should().Be(new Rectangle(
                (int)Math.Round(grandchildBounds.X * factor.Width),
                (int)Math.Round(grandchildBounds.Y * factor.Height),
                (int)Math.Round(grandchildBounds.Width * factor.Width),
                (int)Math.Round(grandchildBounds.Height * factor.Height)));
            childFontChanged.Should().Be(1);
            grandchildFontChanged.Should().Be(1);
            child.IsDpiChangeScalingRequired.Should().BeFalse();
            parent._currentAutoScaleFactor.Should().Be(new SizeF(1, 1));
            child._currentAutoScaleFactor.Should().Be(new SizeF(1, 1));
        }, autoScaleMode.ToString(), explicitFont.ToString());

        handle.ExitCode.Should().Be(RemoteExecutor.SuccessExitCode);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Form_DpiChanged_RoundTripPreservesAnchoredBounds(bool anchorLayoutV2)
    {
        using RemoteInvokeHandle handle = RemoteExecutor.Invoke((useV2) =>
        {
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2).Should().BeTrue();
            using AnchorLayoutV2Scope scope = new(bool.Parse(useV2));

            for (int anchors = 0; anchors < 16; anchors++)
            {
                using Form form = new()
                {
                    AutoScaleMode = AutoScaleMode.Dpi,
                    FormBorderStyle = FormBorderStyle.None,
                    ClientSize = new Size(250, 150)
                };
                using Panel panel = new()
                {
                    Anchor = (AnchorStyles)anchors,
                    Bounds = new Rectangle(20, 10, 150, 100)
                };
                using Panel nestedPanel = new()
                {
                    Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                    Bounds = new Rectangle(10, 10, 40, 30)
                };
                panel.Controls.Add(nestedPanel);
                form.Controls.Add(panel);
                Assert.NotEqual(IntPtr.Zero, form.Handle);
                Assert.NotEqual(IntPtr.Zero, panel.Handle);
                Assert.NotEqual(IntPtr.Zero, nestedPanel.Handle);
                form.PerformAutoScale();

                int initialDpi = form.DeviceDpi;
                Rectangle initialFormBounds = form.Bounds;
                Rectangle initialDisplayRectangle = form.DisplayRectangle;
                Rectangle initialPanelBounds = panel.Bounds;
                Rectangle initialNestedBounds = nestedPanel.Bounds;
                Rectangle scaledPanelBounds = new(
                    initialPanelBounds.X * 2,
                    initialPanelBounds.Y * 2,
                    initialPanelBounds.Width * 2,
                    initialPanelBounds.Height * 2);
                Rectangle scaledNestedBounds = new(
                    initialNestedBounds.X * 2,
                    initialNestedBounds.Y * 2,
                    initialNestedBounds.Width * 2,
                    initialNestedBounds.Height * 2);

                for (int i = 0; i < 2; i++)
                {
                    ChangeDpi(initialDpi * 2, initialFormBounds.Width * 2, initialFormBounds.Height * 2);
                    Assert.Equal(scaledPanelBounds, panel.Bounds);
                    Assert.Equal(scaledNestedBounds, nestedPanel.Bounds);
                    ChangeDpi(initialDpi, initialFormBounds.Width, initialFormBounds.Height);
                    Assert.Equal(initialPanelBounds, panel.Bounds);
                    Assert.Equal(initialNestedBounds, nestedPanel.Bounds);
                }

                const int extra = 20;
                ChangeDpi(initialDpi * 2, initialFormBounds.Width * 2 + extra, initialFormBounds.Height * 2 + extra);

                AnchorStyles anchor = panel.Anchor;
                bool left = (anchor & AnchorStyles.Left) != 0;
                bool right = (anchor & AnchorStyles.Right) != 0;
                bool top = (anchor & AnchorStyles.Top) != 0;
                bool bottom = (anchor & AnchorStyles.Bottom) != 0;
                Rectangle expected;
                if (bool.Parse(useV2) && anchor != (AnchorStyles.Top | AnchorStyles.Left))
                {
                    Rectangle displayRectangle = form.DisplayRectangle;
                    double widthFactor = (double)displayRectangle.Width / initialDisplayRectangle.Width;
                    double heightFactor = (double)displayRectangle.Height / initialDisplayRectangle.Height;
                    int leftDistance = (int)Math.Round(initialPanelBounds.Left * widthFactor);
                    int rightDistance = (int)Math.Round((initialDisplayRectangle.Width - initialPanelBounds.Right) * widthFactor);
                    int topDistance = (int)Math.Round(initialPanelBounds.Top * heightFactor);
                    int bottomDistance = (int)Math.Round((initialDisplayRectangle.Height - initialPanelBounds.Bottom) * heightFactor);
                    int width = left && right ? displayRectangle.Width - leftDistance - rightDistance : scaledPanelBounds.Width;
                    int height = top && bottom ? displayRectangle.Height - topDistance - bottomDistance : scaledPanelBounds.Height;
                    int x = left
                        ? leftDistance
                        : right
                            ? displayRectangle.Width - rightDistance - width
                            : leftDistance + (displayRectangle.Width - leftDistance - rightDistance - width) / 2;
                    int y = top
                        ? topDistance
                        : bottom
                            ? displayRectangle.Height - bottomDistance - height
                            : topDistance + (displayRectangle.Height - topDistance - bottomDistance - height) / 2;

                    expected = new Rectangle(
                        x,
                        y,
                        width,
                        height);
                }
                else
                {
                    expected = scaledPanelBounds;
                    expected.X += left ? 0 : right ? extra : extra / 2;
                    expected.Y += top ? 0 : bottom ? extra : extra / 2;
                    expected.Width += left && right ? extra : 0;
                    expected.Height += top && bottom ? extra : 0;
                }

                Assert.Equal(expected, panel.Bounds);
                ChangeDpi(initialDpi, initialFormBounds.Width, initialFormBounds.Height);
                Assert.Equal(initialPanelBounds, panel.Bounds);

                AnchorStyles finalAnchor = panel.Anchor;
                bool anchoredLeft = (finalAnchor & AnchorStyles.Left) != 0;
                bool anchoredRight = (finalAnchor & AnchorStyles.Right) != 0;
                bool anchoredTop = (finalAnchor & AnchorStyles.Top) != 0;
                bool anchoredBottom = (finalAnchor & AnchorStyles.Bottom) != 0;
                Rectangle resizedBounds = initialPanelBounds;
                resizedBounds.X += anchoredLeft ? 0 : anchoredRight ? 20 : 10;
                resizedBounds.Y += anchoredTop ? 0 : anchoredBottom ? 20 : 10;
                resizedBounds.Width += anchoredLeft && anchoredRight ? 20 : 0;
                resizedBounds.Height += anchoredTop && anchoredBottom ? 20 : 0;
                form.ClientSize += new Size(20, 20);
                if (bool.Parse(useV2))
                {
                    AssertBoundsWithinOnePixel(resizedBounds);
                }
                else
                {
                    Assert.Equal(resizedBounds, panel.Bounds);
                }

                void ChangeDpi(int dpi, int width, int height)
                {
                    WPARAM dpiParam = WPARAM.MAKEWPARAM(dpi, dpi);
                    PInvokeCore.SendMessage(nestedPanel, PInvokeCore.WM_DPICHANGED_BEFOREPARENT, dpiParam);
                    PInvokeCore.SendMessage(panel, PInvokeCore.WM_DPICHANGED_BEFOREPARENT, dpiParam);
                    RECT suggestedBounds = new(initialFormBounds.X, initialFormBounds.Y, initialFormBounds.X + width, initialFormBounds.Y + height);
                    PInvokeCore.SendMessage(form, PInvokeCore.WM_DPICHANGED, dpiParam, ref suggestedBounds);
                    PInvokeCore.SendMessage(panel, PInvokeCore.WM_DPICHANGED_AFTERPARENT);
                    PInvokeCore.SendMessage(nestedPanel, PInvokeCore.WM_DPICHANGED_AFTERPARENT);
                }

                void AssertBoundsWithinOnePixel(Rectangle expectedBounds)
                {
                    Rectangle actualBounds = panel.Bounds;
                    Assert.InRange(Math.Abs(actualBounds.X - expectedBounds.X), 0, 1);
                    Assert.InRange(Math.Abs(actualBounds.Y - expectedBounds.Y), 0, 1);
                    Assert.InRange(Math.Abs(actualBounds.Width - expectedBounds.Width), 0, 1);
                    Assert.InRange(Math.Abs(actualBounds.Height - expectedBounds.Height), 0, 1);
                }
            }
        }, anchorLayoutV2.ToString());

        handle.ExitCode.Should().Be(RemoteExecutor.SuccessExitCode);
    }

    [Theory]
    [InlineData(false, nameof(Control.Resize))]
    [InlineData(false, nameof(Control.SizeChanged))]
    [InlineData(false, nameof(Control.ClientSizeChanged))]
    [InlineData(false, nameof(Control.Move))]
    [InlineData(false, nameof(Control.LocationChanged))]
    [InlineData(true, nameof(Control.Resize))]
    [InlineData(true, nameof(Control.SizeChanged))]
    [InlineData(true, nameof(Control.ClientSizeChanged))]
    [InlineData(true, nameof(Control.Move))]
    [InlineData(true, nameof(Control.LocationChanged))]
    public void Form_DpiChanged_BoundsChangedHandlerKeepsChildBounds(bool anchorLayoutV2, string eventName)
    {
        using RemoteInvokeHandle handle = RemoteExecutor.Invoke((useV2, resizeEvent) =>
        {
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2).Should().BeTrue();
            using AnchorLayoutV2Scope scope = new(bool.Parse(useV2));
            using Form form = new()
            {
                AutoScaleMode = AutoScaleMode.Dpi,
                FormBorderStyle = FormBorderStyle.None,
                ClientSize = new Size(250, 150)
            };
            using Panel panel = new() { Bounds = form.ClientRectangle };
            form.Controls.Add(panel);
            Assert.NotEqual(IntPtr.Zero, form.Handle);
            Assert.NotEqual(IntPtr.Zero, panel.Handle);
            form.PerformAutoScale();

            int calls = 0;
            EventHandler handler = (_, _) =>
            {
                calls++;
                panel.Bounds = form.ClientRectangle;
            };

            switch (resizeEvent)
            {
                case nameof(Control.Resize):
                    form.Resize += handler;
                    break;
                case nameof(Control.SizeChanged):
                    form.SizeChanged += handler;
                    break;
                case nameof(Control.ClientSizeChanged):
                    form.ClientSizeChanged += handler;
                    break;
                case nameof(Control.Move):
                    form.Move += handler;
                    break;
                case nameof(Control.LocationChanged):
                    form.LocationChanged += handler;
                    break;
            }

            Rectangle bounds = form.Bounds;
            int initialDpi = form.DeviceDpi;
            ChangeDpi(initialDpi * 2, new Rectangle(bounds.X + 10, bounds.Y + 20, bounds.Width * 2, bounds.Height * 2));
            Assert.Equal(1, calls);
            Assert.Equal(form.ClientRectangle, panel.Bounds);
            ChangeDpi(initialDpi, bounds);
            Assert.Equal(2, calls);
            Assert.Equal(form.ClientRectangle, panel.Bounds);

            if (resizeEvent is nameof(Control.Move) or nameof(Control.LocationChanged))
            {
                form.Left += 10;
            }
            else
            {
                form.Width += 10;
            }

            Assert.Equal(3, calls);
            Assert.Equal(form.ClientRectangle, panel.Bounds);

            void ChangeDpi(int dpi, Rectangle newBounds)
            {
                WPARAM dpiParam = WPARAM.MAKEWPARAM(dpi, dpi);
                RECT suggested = new(newBounds);
                PInvokeCore.SendMessage(panel, PInvokeCore.WM_DPICHANGED_BEFOREPARENT, dpiParam);
                PInvokeCore.SendMessage(form, PInvokeCore.WM_DPICHANGED, dpiParam, ref suggested);
                PInvokeCore.SendMessage(panel, PInvokeCore.WM_DPICHANGED_AFTERPARENT);
            }
        }, anchorLayoutV2.ToString(), eventName);

        handle.ExitCode.Should().Be(RemoteExecutor.SuccessExitCode);
    }

    [Fact]
    public void Form_DpiChanged_BoundsEventsKeepOrderAndCount()
    {
        using RemoteInvokeHandle handle = RemoteExecutor.Invoke(() =>
        {
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2).Should().BeTrue();
            using Form form = new()
            {
                AutoScaleMode = AutoScaleMode.Dpi,
                FormBorderStyle = FormBorderStyle.None,
                ClientSize = new Size(250, 150)
            };
            Assert.NotEqual(IntPtr.Zero, form.Handle);
            form.PerformAutoScale();

            List<string> events = [];
            form.Move += (_, _) => events.Add(nameof(Control.Move));
            form.LocationChanged += (_, _) => events.Add(nameof(Control.LocationChanged));
            form.Resize += (_, _) => events.Add(nameof(Control.Resize));
            form.SizeChanged += (_, _) => events.Add(nameof(Control.SizeChanged));
            form.ClientSizeChanged += (_, _) => events.Add(nameof(Control.ClientSizeChanged));

            form.SetBounds(form.Left + 10, form.Top + 20, form.Width + 10, form.Height + 20);
            string[] ordinaryEvents = [.. events];
            Assert.Equal(
                [nameof(Control.Move), nameof(Control.LocationChanged), nameof(Control.Resize),
                    nameof(Control.SizeChanged), nameof(Control.ClientSizeChanged)],
                ordinaryEvents);
            events.Clear();

            int dpi = form.DeviceDpi * 2;
            WPARAM dpiParam = WPARAM.MAKEWPARAM(dpi, dpi);
            RECT suggested = new(form.Left + 10, form.Top + 20, form.Right + form.Width + 10, form.Bottom + form.Height + 20);
            PInvokeCore.SendMessage(form, PInvokeCore.WM_DPICHANGED, dpiParam, ref suggested);
            Assert.Equal(ordinaryEvents, events);

            events.Clear();
            form.FontChanged += (_, _) => form.Width += 10;
            dpi /= 2;
            dpiParam = WPARAM.MAKEWPARAM(dpi, dpi);
            suggested = new(form.Left, form.Top, form.Left + form.Width / 2, form.Top + form.Height / 2);
            PInvokeCore.SendMessage(form, PInvokeCore.WM_DPICHANGED, dpiParam, ref suggested);
            Assert.Equal(2, events.Count(eventName => eventName == nameof(Control.Resize)));
            Assert.Equal(2, events.Count(eventName => eventName == nameof(Control.SizeChanged)));
            Assert.Equal(2, events.Count(eventName => eventName == nameof(Control.ClientSizeChanged)));
        });

        handle.ExitCode.Should().Be(RemoteExecutor.SuccessExitCode);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Form_DpiChanged_ClientSizeSetInFontChanged_KeepsEventOrder(bool changeClientSize)
    {
        using RemoteInvokeHandle handle = RemoteExecutor.Invoke(changeSize =>
        {
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2).Should().BeTrue();
            using Form form = new()
            {
                AutoScaleMode = AutoScaleMode.Dpi,
                FormBorderStyle = FormBorderStyle.None,
                ClientSize = new Size(250, 150)
            };
            Assert.NotEqual(IntPtr.Zero, form.Handle);
            form.PerformAutoScale();

            List<string> events = [];
            form.Resize += (_, _) => events.Add(nameof(Control.Resize));
            form.SizeChanged += (_, _) => events.Add(nameof(Control.SizeChanged));
            form.ClientSizeChanged += (_, _) => events.Add(nameof(Control.ClientSizeChanged));

            Size increment = bool.Parse(changeSize) ? new Size(10, 10) : Size.Empty;
            form.ClientSize += increment;
            string[] ordinaryEvents = [.. events];
            Assert.Equal(
                bool.Parse(changeSize)
                    ? [nameof(Control.Resize), nameof(Control.SizeChanged), nameof(Control.ClientSizeChanged),
                        nameof(Control.ClientSizeChanged)]
                    : [nameof(Control.ClientSizeChanged)],
                ordinaryEvents);
            events.Clear();

            form.FontChanged += (_, _) => form.ClientSize += increment;
            int dpi = form.DeviceDpi * 2;
            WPARAM dpiParam = WPARAM.MAKEWPARAM(dpi, dpi);
            RECT suggested = new(form.Left, form.Top, form.Left + form.Width * 2, form.Top + form.Height * 2);
            PInvokeCore.SendMessage(form, PInvokeCore.WM_DPICHANGED, dpiParam, ref suggested);

            Assert.Equal(
                [nameof(Control.Resize), nameof(Control.SizeChanged), nameof(Control.ClientSizeChanged), .. ordinaryEvents],
                events);
        }, changeClientSize.ToString());

        handle.ExitCode.Should().Be(RemoteExecutor.SuccessExitCode);
    }

    [Theory]
    [InlineData(false, false, false, AnchorStyles.Top | AnchorStyles.Right)]
    [InlineData(false, false, false, AnchorStyles.None)]
    [InlineData(false, false, true, AnchorStyles.Top | AnchorStyles.Right)]
    [InlineData(false, true, false, AnchorStyles.Top | AnchorStyles.Right)]
    [InlineData(false, true, true, AnchorStyles.Top | AnchorStyles.Right)]
    [InlineData(true, false, false, AnchorStyles.Top | AnchorStyles.Right)]
    [InlineData(true, false, false, AnchorStyles.None)]
    [InlineData(true, false, true, AnchorStyles.Top | AnchorStyles.Right)]
    [InlineData(true, true, false, AnchorStyles.Top | AnchorStyles.Right)]
    [InlineData(true, true, true, AnchorStyles.Top | AnchorStyles.Right)]
    public void Form_DpiChanged_ScrollbarAnchorsRespectFixedDimension(
        bool vertical, bool stretch, bool anchorLayoutV2, AnchorStyles anchor)
    {
        using RemoteInvokeHandle handle = RemoteExecutor.Invoke((isVertical, stretchFixedDimension, useV2, initialAnchor) =>
        {
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2).Should().BeTrue();
            using AnchorLayoutV2Scope scope = new(bool.Parse(useV2));
            bool verticalScrollBar = bool.Parse(isVertical);
            bool stretchDimension = bool.Parse(stretchFixedDimension);
            using Form form = new()
            {
                AutoScaleMode = AutoScaleMode.Dpi,
                FormBorderStyle = FormBorderStyle.None,
                ClientSize = new Size(250, 150)
            };
            using ScrollBar scrollBar = verticalScrollBar ? new VScrollBar() : new HScrollBar();
            scrollBar.Bounds = verticalScrollBar
                ? new Rectangle(30, 40, 18, 80)
                : new Rectangle(30, 40, 120, 18);
            scrollBar.Anchor = Enum.Parse<AnchorStyles>(initialAnchor);
            if (stretchDimension)
            {
                scrollBar.Anchor |= verticalScrollBar
                    ? AnchorStyles.Left | AnchorStyles.Right
                    : AnchorStyles.Top | AnchorStyles.Bottom;
            }

            form.Controls.Add(scrollBar);
            Assert.NotEqual(IntPtr.Zero, form.Handle);
            Assert.NotEqual(IntPtr.Zero, scrollBar.Handle);
            form.PerformAutoScale();
            Rectangle initialBounds = scrollBar.Bounds;
            Rectangle initialFormBounds = form.Bounds;
            int initialDpi = form.DeviceDpi;

            ChangeDpi(initialDpi * 2, initialFormBounds.Width * 2, initialFormBounds.Height * 2);
            int initialFixedDimension = verticalScrollBar ? initialBounds.Width : initialBounds.Height;
            int actualFixedDimension = verticalScrollBar ? scrollBar.Width : scrollBar.Height;
            Assert.Equal(initialFixedDimension * (stretchDimension ? 2 : 1), actualFixedDimension);
            if (!stretchDimension && verticalScrollBar)
            {
                if (scrollBar.Anchor == AnchorStyles.None)
                {
                    int originalOffset = initialBounds.X + initialBounds.Width / 2 - initialFormBounds.Width / 2;
                    Assert.Equal(form.ClientSize.Width / 2 + originalOffset * 2, scrollBar.Left + scrollBar.Width / 2);
                }
                else
                {
                    Assert.Equal(form.ClientSize.Width - (initialFormBounds.Width - initialBounds.Right) * 2, scrollBar.Right);
                }
            }
            else if (!stretchDimension)
            {
                if (scrollBar.Anchor == AnchorStyles.None)
                {
                    int originalOffset = initialBounds.Y + initialBounds.Height / 2 - initialFormBounds.Height / 2;
                    Assert.Equal(form.ClientSize.Height / 2 + originalOffset * 2, scrollBar.Top + scrollBar.Height / 2);
                }
                else
                {
                    Assert.Equal(initialBounds.Top * 2, scrollBar.Top);
                }
            }

            ChangeDpi(initialDpi, initialFormBounds.Width, initialFormBounds.Height);
            Assert.Equal(initialBounds, scrollBar.Bounds);

            ChangeDpi(initialDpi * 3 / 2, initialFormBounds.Width * 3 / 2, initialFormBounds.Height * 3 / 2);
            actualFixedDimension = verticalScrollBar ? scrollBar.Width : scrollBar.Height;
            Assert.Equal(stretchDimension ? initialFixedDimension * 3 / 2 : initialFixedDimension, actualFixedDimension);
            ChangeDpi(initialDpi, initialFormBounds.Width, initialFormBounds.Height);
            Assert.InRange(Math.Abs(initialBounds.X - scrollBar.Left), 0, 1);
            Assert.InRange(Math.Abs(initialBounds.Y - scrollBar.Top), 0, 1);
            Assert.Equal(initialBounds.Size, scrollBar.Size);

            void ChangeDpi(int dpi, int width, int height)
            {
                WPARAM dpiParam = WPARAM.MAKEWPARAM(dpi, dpi);
                RECT suggested = new(initialFormBounds.X, initialFormBounds.Y, initialFormBounds.X + width, initialFormBounds.Y + height);
                PInvokeCore.SendMessage(scrollBar, PInvokeCore.WM_DPICHANGED_BEFOREPARENT, dpiParam);
                PInvokeCore.SendMessage(form, PInvokeCore.WM_DPICHANGED, dpiParam, ref suggested);
                PInvokeCore.SendMessage(scrollBar, PInvokeCore.WM_DPICHANGED_AFTERPARENT);
            }
        }, vertical.ToString(), stretch.ToString(), anchorLayoutV2.ToString(), anchor.ToString());

        handle.ExitCode.Should().Be(RemoteExecutor.SuccessExitCode);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Form_DpiChanged_AutoScaleNone_StillLaysOutAnchoredControls(bool anchorLayoutV2)
    {
        using RemoteInvokeHandle handle = RemoteExecutor.Invoke((useV2) =>
        {
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2).Should().BeTrue();
            using AnchorLayoutV2Scope scope = new(bool.Parse(useV2));
            using Form form = new()
            {
                AutoScaleMode = AutoScaleMode.None,
                FormBorderStyle = FormBorderStyle.None,
                ClientSize = new Size(250, 150)
            };
            using Panel panel = new()
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Bounds = new Rectangle(20, 10, 150, 100)
            };
            form.Controls.Add(panel);
            Assert.NotEqual(IntPtr.Zero, form.Handle);
            Assert.NotEqual(IntPtr.Zero, panel.Handle);
            int initialDpi = form.DeviceDpi;
            Rectangle initialBounds = form.Bounds;

            ChangeDpi(initialDpi * 2, initialBounds.Width * 2, initialBounds.Height * 2);
            Assert.Equal(new Rectangle(20, 10, 400, 250), panel.Bounds);
            ChangeDpi(initialDpi, initialBounds.Width, initialBounds.Height);
            Assert.Equal(new Rectangle(20, 10, 150, 100), panel.Bounds);

            void ChangeDpi(int dpi, int width, int height)
            {
                WPARAM dpiParam = WPARAM.MAKEWPARAM(dpi, dpi);
                PInvokeCore.SendMessage(panel, PInvokeCore.WM_DPICHANGED_BEFOREPARENT, dpiParam);
                RECT suggestedBounds = new(initialBounds.X, initialBounds.Y, initialBounds.X + width, initialBounds.Y + height);
                PInvokeCore.SendMessage(form, PInvokeCore.WM_DPICHANGED, dpiParam, ref suggestedBounds);
                PInvokeCore.SendMessage(panel, PInvokeCore.WM_DPICHANGED_AFTERPARENT);
            }
        }, anchorLayoutV2.ToString());

        handle.ExitCode.Should().Be(RemoteExecutor.SuccessExitCode);
    }

    [WinFormsTheory]
    [InlineData(AutoScaleMode.Font, false)]
    [InlineData(AutoScaleMode.Font, true)]
    [InlineData(AutoScaleMode.Dpi, false)]
    [InlineData(AutoScaleMode.Dpi, true)]
    public void ContainerControl_ResumeLayout_ScalesNestedContainersOnce(AutoScaleMode autoScaleMode, bool performLayout)
    {
        using SubContainerControl parent = new()
        {
            AutoScaleMode = autoScaleMode
        };
        using ContainerControl child = new()
        {
            Font = Control.DefaultFont,
            Bounds = new Rectangle(40, 50, 100, 80)
        };
        using ContainerControl grandchild = new()
        {
            Bounds = new Rectangle(20, 30, 40, 50)
        };
        parent.Controls.Add(child);
        child.Controls.Add(grandchild);
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        Assert.NotEqual(IntPtr.Zero, child.Handle);
        Assert.NotEqual(IntPtr.Zero, grandchild.Handle);
        parent.SuspendLayout();
        SizeF dimensions = parent.CurrentAutoScaleDimensions;
        parent.AutoScaleDimensions = new SizeF(dimensions.Width / 2, dimensions.Height / 2);

        parent.AutoScaleFactor.Should().Be(new SizeF(2, 2));
        parent._currentAutoScaleFactor.Should().Be(new SizeF(1, 1));
        child.Bounds.Should().Be(new Rectangle(40, 50, 100, 80));
        grandchild.Bounds.Should().Be(new Rectangle(20, 30, 40, 50));

        parent.ResumeLayout(performLayout);

        child.Bounds.Should().Be(new Rectangle(80, 100, 200, 160));
        grandchild.Bounds.Should().Be(new Rectangle(40, 60, 80, 100));
        parent.AutoScaleDimensions.Should().Be(dimensions);
        parent._currentAutoScaleFactor.Should().Be(new SizeF(1, 1));

        using Font childFont = new(Control.DefaultFont, FontStyle.Bold);
        child.Font = childFont;
        child.Bounds.Should().Be(new Rectangle(80, 100, 200, 160));
        grandchild.Bounds.Should().Be(new Rectangle(40, 60, 80, 100));
    }

    [WinFormsTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void ContainerControl_Font_Set_ScalesNonContainerChildren(bool explicitFont)
    {
        using Font font = new(Control.DefaultFont.FontFamily, Control.DefaultFont.Size * 2);
        using ContainerControl parent = new()
        {
            AutoScaleMode = AutoScaleMode.Font
        };
        using Label label = new();
        using TextBox textBox = new() { AutoSize = false, BorderStyle = BorderStyle.None };
        using Panel panel = new();
        Control[] controls = [label, textBox, panel];
        foreach (Control control in controls)
        {
            if (explicitFont)
            {
                control.Font = Control.DefaultFont;
            }

            control.Bounds = new Rectangle(40, 50, 100, 80);
            parent.Controls.Add(control);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
        }

        using ContainerControl nested = new()
        {
            Bounds = new Rectangle(40, 50, 100, 80)
        };
        panel.Controls.Add(nested);
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        Assert.NotEqual(IntPtr.Zero, nested.Handle);
        parent.PerformAutoScale();
        SizeF dimensions = parent.CurrentAutoScaleDimensions;
        Rectangle bounds = nested.Bounds;

        parent.Font = font;

        SizeF factor = ContainerControl.GetCurrentAutoScaleFactor(parent.CurrentAutoScaleDimensions, dimensions);
        Rectangle expected = new(
            (int)Math.Round(bounds.X * factor.Width),
            (int)Math.Round(bounds.Y * factor.Height),
            (int)Math.Round(bounds.Width * factor.Width),
            (int)Math.Round(bounds.Height * factor.Height));
        foreach (Control control in controls)
        {
            control.Bounds.Should().Be(expected);
            control.Font.Should().BeSameAs(explicitFont ? Control.DefaultFont : font);
        }

        nested.Bounds.Should().Be(expected);

        parent.ResetFont();

        foreach (Control control in controls)
        {
            control.Bounds.Should().Be(bounds);
        }

        nested.Bounds.Should().Be(bounds);
    }

    [WinFormsFact]
    public void ContainerControl_GetAutoSizeMode_Invoke_ReturnsExpected()
    {
        using SubContainerControl control = new();
        Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
    }

    [WinFormsTheory]
    [InlineData(0, true)]
    [InlineData(SubContainerControl.ScrollStateAutoScrolling, false)]
    [InlineData(SubContainerControl.ScrollStateFullDrag, false)]
    [InlineData(SubContainerControl.ScrollStateHScrollVisible, false)]
    [InlineData(SubContainerControl.ScrollStateUserHasScrolled, false)]
    [InlineData(SubContainerControl.ScrollStateVScrollVisible, false)]
    [InlineData(int.MaxValue, false)]
    [InlineData((-1), false)]
    public void ContainerControl_GetScrollState_Invoke_ReturnsExpected(int bit, bool expected)
    {
        using SubContainerControl control = new();
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
    public void ContainerControl_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
    {
        using SubContainerControl control = new();
        Assert.Equal(expected, control.GetStyle(flag));

        // Call again to test caching.
        Assert.Equal(expected, control.GetStyle(flag));
    }

    [WinFormsFact]
    public void ContainerControl_GetTopLevel_Invoke_ReturnsExpected()
    {
        using SubContainerControl control = new();
        Assert.False(control.GetTopLevel());
    }

    [WinFormsTheory]
    [EnumData<AutoScaleMode>]
    public void PerformAutoScale_InvokeWithoutChildren_Success(AutoScaleMode autoScaleMode)
    {
        using SubContainerControl control = new()
        {
            AutoScaleMode = autoScaleMode
        };
        control.PerformAutoScale();
    }

    [WinFormsTheory]
    [EnumData<AutoScaleMode>]
    public void PerformAutoScale_InvokeWithChildren_Success(AutoScaleMode autoScaleMode)
    {
        using Control child = new();
        using SubContainerControl control = new()
        {
            AutoScaleMode = autoScaleMode
        };
        control.Controls.Add(child);
        control.PerformAutoScale();
    }

    [WinFormsFact]
    public void ContainerControl_CreateContainerControl_Invoke_CallsBindingContextChanged()
    {
        using ContainerControl control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.BindingContextChanged += handler;

        // Set different.
        BindingContext value1 = [];
        control.BindingContext = value1;
        Assert.Same(value1, control.BindingContext);
        Assert.Equal(1, callCount);

        // Set same.
        control.BindingContext = value1;
        Assert.Same(value1, control.BindingContext);
        Assert.Equal(1, callCount);

        // Set different.
        BindingContext value2 = [];
        control.BindingContext = value2;
        Assert.Equal(value2, control.BindingContext);
        Assert.Equal(2, callCount);

        // Remove handler.
        BindingContext value3 = [];
        control.BindingContextChanged -= handler;
        control.BindingContext = value3;
        Assert.Same(value3, control.BindingContext);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void ContainerControl_Dispose_Invoke_ResetsActiveControl()
    {
        using ContainerControl control = new();
        using Control child = new();
        control.Controls.Add(child);
        control.ActiveControl = child;

        control.Dispose();
        Assert.Null(control.ActiveControl);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ContainerControl_OnAutoValidateChanged_Invoke_CallsAutoValidateChanged(EventArgs eventArgs)
    {
        using SubContainerControl control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.AutoValidateChanged += handler;
        control.OnAutoValidateChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.AutoValidateChanged -= handler;
        control.OnAutoValidateChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void UserControl_OnCreateControl_Invoke_Nop()
    {
        using SubContainerControl control = new();
        int bindingContextChangedCallCount = 0;
        control.BindingContextChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            bindingContextChangedCallCount++;
        };
        control.OnCreateControl();
        Assert.False(control.Created);
        Assert.False(control.IsHandleCreated);
        Assert.Equal(1, bindingContextChangedCallCount);

        control.OnCreateControl();
        Assert.False(control.Created);
        Assert.False(control.IsHandleCreated);
        Assert.Equal(2, bindingContextChangedCallCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ContainerControl_OnFontChanged_Invoke_CallsFontChanged(EventArgs eventArgs)
    {
        using SubContainerControl control = new();
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
        Assert.Equal(1, callCount);

        // Remove handler.
        control.FontChanged -= handler;
        control.OnFontChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ContainerControl_OnFontChanged_InvokeWithAutoScaleModeFont_CallsFontChanged(EventArgs eventArgs)
    {
        using SubContainerControl control = new()
        {
            AutoScaleMode = AutoScaleMode.Font
        };
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
        Assert.Equal(new Size(1, 1), control.AutoScaleFactor);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.FontChanged -= handler;
        control.OnFontChanged(eventArgs);
        Assert.Equal(new Size(1, 1), control.AutoScaleFactor);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetLayoutEventArgsTheoryData))]
    public void ContainerControl_OnLayout_Invoke_CallsLayout(LayoutEventArgs eventArgs)
    {
        using SubContainerControl control = new();
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

        // Remove handler.
        control.Layout -= handler;
        control.OnLayout(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ContainerControl_OnParentChanged_Invoke_CallsParentChanged(EventArgs eventArgs)
    {
        using SubContainerControl control = new();
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

        // Remove handler.
        control.ParentChanged -= handler;
        control.OnParentChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void ContainerControl_UpdateDefaultButton_Invoke_Nop()
    {
        using SubContainerControl control = new();
        control.UpdateDefaultButton();
        control.UpdateDefaultButton();
    }

    [WinFormsFact]
    public void ContainerControl_ValidateChildren_InvokeWithoutChildren_ReturnsTrue()
    {
        using ContainerControl control = new();
        Assert.True(control.ValidateChildren());
    }

    public static IEnumerable<object[]> ValidateChildren_TestData()
    {
        yield return new object[] { true, 0 };
        yield return new object[] { false, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(ValidateChildren_TestData))]
    public void ContainerControl_ValidateChildren_InvokeWithChildren_ReturnsExpected(bool cancel, int expectedCallCount)
    {
        using ContainerControl control = new();
        using Control child1 = new();
        using Control grandchild1 = new();
        child1.Controls.Add(grandchild1);
        using ContainerControl child2 = new();
        using Control grandchild2 = new();
        child2.Controls.Add(grandchild2);
        using TabControl child3 = new();
        using TabPage grandchild3 = new();
        child3.Controls.Add(grandchild3);
        using SubControl child4 = new();
        child4.SetStyle(ControlStyles.Selectable, false);
        using SubControl child5 = new()
        {
            Enabled = false
        };
        using SubControl child6 = new()
        {
            Visible = false
        };
        using SubControl child7 = new()
        {
            TabStop = false
        };
        using SubControl child8 = new()
        {
            CausesValidation = false
        };
        control.Controls.Add(child1);
        control.Controls.Add(child2);
        control.Controls.Add(child3);
        control.Controls.Add(child4);
        control.Controls.Add(child5);
        control.Controls.Add(child6);
        control.Controls.Add(child7);
        control.Controls.Add(child8);

        int validatingCallCount = 0;
        control.Validating += (sender, e) => validatingCallCount++;
        int validatedCallCount = 0;
        control.Validated += (sender, e) => validatedCallCount++;

        int child1ValidatingCallCount = 0;
        child1.Validating += (sender, e) =>
        {
            Assert.Same(child1, sender);
            Assert.False(e.Cancel);
            child1ValidatingCallCount++;
            e.Cancel = cancel;
        };
        int child1ValidatedCallCount = 0;
        child1.Validated += (sender, e) =>
        {
            Assert.Same(child1, sender);
            Assert.Same(EventArgs.Empty, e);
            child1ValidatedCallCount++;
        };
        int grandchild1ValidatingCallCount = 0;
        grandchild1.Validating += (sender, e) =>
        {
            Assert.Same(grandchild1, sender);
            Assert.False(e.Cancel);
            grandchild1ValidatingCallCount++;
        };
        int grandchild1ValidatedCallCount = 0;
        grandchild1.Validated += (sender, e) => grandchild1ValidatedCallCount++;
        int child2ValidatingCallCount = 0;
        child2.Validating += (sender, e) =>
        {
            Assert.Same(child2, sender);
            Assert.False(e.Cancel);
            child2ValidatingCallCount++;
        };
        int child2ValidatedCallCount = 0;
        child2.Validated += (sender, e) =>
        {
            Assert.Same(child2, sender);
            Assert.Same(EventArgs.Empty, e);
            child2ValidatedCallCount++;
        };
        int grandchild2ValidatingCallCount = 0;
        grandchild2.Validating += (sender, e) =>
        {
            Assert.Same(grandchild2, sender);
            Assert.False(e.Cancel);
            grandchild2ValidatingCallCount++;
        };
        int grandchild2ValidatedCallCount = 0;
        grandchild2.Validated += (sender, e) =>
        {
            Assert.Same(grandchild2, sender);
            Assert.Same(EventArgs.Empty, e);
            grandchild2ValidatedCallCount++;
        };
        int child3ValidatingCallCount = 0;
        child3.Validating += (sender, e) =>
        {
            Assert.Same(child3, sender);
            Assert.False(e.Cancel);
            child3ValidatingCallCount++;
        };
        int child3ValidatedCallCount = 0;
        child3.Validated += (sender, e) =>
        {
            Assert.Same(child3, sender);
            Assert.Same(EventArgs.Empty, e);
            child3ValidatedCallCount++;
        };
        int grandchild3ValidatingCallCount = 0;
        grandchild3.Validating += (sender, e) => grandchild3ValidatingCallCount++;
        int grandchild3ValidatedCallCount = 0;
        grandchild3.Validated += (sender, e) => grandchild3ValidatedCallCount++;
        int child4ValidatingCallCount = 0;
        child4.Validating += (sender, e) => child4ValidatingCallCount++;
        int child4ValidatedCallCount = 0;
        child4.Validated += (sender, e) => child4ValidatedCallCount++;
        int child5ValidatingCallCount = 0;
        child5.Validating += (sender, e) =>
        {
            Assert.Same(child5, sender);
            Assert.False(e.Cancel);
            child5ValidatingCallCount++;
        };
        int child5ValidatedCallCount = 0;
        child5.Validated += (sender, e) =>
        {
            Assert.Same(child5, sender);
            Assert.Same(EventArgs.Empty, e);
            child5ValidatedCallCount++;
        };
        int child6ValidatingCallCount = 0;
        child6.Validating += (sender, e) =>
        {
            Assert.Same(child6, sender);
            Assert.False(e.Cancel);
            child6ValidatingCallCount++;
        };
        int child6ValidatedCallCount = 0;
        child6.Validated += (sender, e) =>
        {
            Assert.Same(child6, sender);
            Assert.Same(EventArgs.Empty, e);
            child6ValidatedCallCount++;
        };
        int child7ValidatingCallCount = 0;
        child7.Validating += (sender, e) =>
        {
            Assert.Same(child7, sender);
            Assert.False(e.Cancel);
            child7ValidatingCallCount++;
        };
        int child7ValidatedCallCount = 0;
        child7.Validated += (sender, e) =>
        {
            Assert.Same(child7, sender);
            Assert.Same(EventArgs.Empty, e);
            child7ValidatedCallCount++;
        };
        int child8ValidatingCallCount = 0;
        child8.Validating += (sender, e) => child8ValidatingCallCount++;
        int child8ValidatedCallCount = 0;
        child8.Validated += (sender, e) => child8ValidatedCallCount++;

        Assert.Equal(!cancel, control.ValidateChildren());
        Assert.Equal(0, validatingCallCount);
        Assert.Equal(0, validatedCallCount);
        Assert.Equal(1, child1ValidatingCallCount);
        Assert.Equal(expectedCallCount, child1ValidatedCallCount);
        Assert.Equal(0, grandchild1ValidatingCallCount);
        Assert.Equal(0, grandchild1ValidatedCallCount);
        Assert.Equal(1, child2ValidatingCallCount);
        Assert.Equal(1, child2ValidatedCallCount);
        Assert.Equal(1, grandchild2ValidatingCallCount);
        Assert.Equal(1, grandchild2ValidatedCallCount);
        Assert.Equal(1, child3ValidatingCallCount);
        Assert.Equal(1, child3ValidatedCallCount);
        Assert.Equal(0, grandchild3ValidatingCallCount);
        Assert.Equal(0, grandchild3ValidatedCallCount);
        Assert.Equal(0, child4ValidatingCallCount);
        Assert.Equal(0, child4ValidatedCallCount);
        Assert.Equal(1, child5ValidatingCallCount);
        Assert.Equal(1, child5ValidatedCallCount);
        Assert.Equal(1, child6ValidatingCallCount);
        Assert.Equal(1, child6ValidatedCallCount);
        Assert.Equal(1, child7ValidatingCallCount);
        Assert.Equal(1, child7ValidatedCallCount);
        Assert.Equal(0, child8ValidatingCallCount);
        Assert.Equal(0, child8ValidatedCallCount);
    }

    [WinFormsTheory]
    [EnumData<ValidationConstraints>]
    public void ContainerControl_ValidateChildren_InvokeValidationConstraintsWithoutChildren_ReturnsTrue(ValidationConstraints validationConstraints)
    {
        using ContainerControl control = new();
        Assert.True(control.ValidateChildren(validationConstraints));
    }

    public static IEnumerable<object[]> ValidateChildren_ValidationConstraints_TestData()
    {
        yield return new object[] { ValidationConstraints.ImmediateChildren, true, 0, 0, 0, 1, 1, 1, 1 };
        yield return new object[] { ValidationConstraints.ImmediateChildren, false, 1, 0, 0, 1, 1, 1, 1 };

        yield return new object[] { ValidationConstraints.Selectable, true, 0, 1, 0, 0, 1, 1, 1 };
        yield return new object[] { ValidationConstraints.Selectable, false, 1, 1, 0, 0, 1, 1, 1 };

        yield return new object[] { ValidationConstraints.Enabled, true, 0, 1, 1, 1, 0, 1, 1 };
        yield return new object[] { ValidationConstraints.Enabled, false, 1, 1, 1, 1, 0, 1, 1 };

        yield return new object[] { ValidationConstraints.Visible, true, 0, 1, 0, 1, 1, 0, 1 };
        yield return new object[] { ValidationConstraints.Visible, false, 1, 1, 0, 1, 1, 0, 1 };

        yield return new object[] { ValidationConstraints.TabStop, true, 0, 1, 0, 1, 1, 1, 0 };
        yield return new object[] { ValidationConstraints.TabStop, false, 1, 1, 0, 1, 1, 1, 0 };

        yield return new object[] { ValidationConstraints.None, true, 0, 1, 1, 1, 1, 1, 1 };
        yield return new object[] { ValidationConstraints.None, false, 1, 1, 1, 1, 1, 1, 1 };

        yield return new object[] { ValidationConstraints.ImmediateChildren | ValidationConstraints.Selectable | ValidationConstraints.Enabled | ValidationConstraints.Visible | ValidationConstraints.TabStop, true, 0, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { ValidationConstraints.ImmediateChildren | ValidationConstraints.Selectable | ValidationConstraints.Enabled | ValidationConstraints.Visible | ValidationConstraints.TabStop, false, 1, 0, 0, 0, 0, 0, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(ValidateChildren_ValidationConstraints_TestData))]
    public void ContainerControl_ValidateChildren_InvokeValidationConstraintsWithChildren_ReturnsExpected(ValidationConstraints validationConstraints, bool cancel, int expectedChild1CallCount, int expectedGrandchild2CallCount, int expectedGrandchild3CallCount, int expectedChild4CallCount, int expectedChild5CallCount, int expectedChild6CallCount, int expectedChild7CallCount)
    {
        using ContainerControl control = new();
        using Control child1 = new();
        using Control grandchild1 = new();
        child1.Controls.Add(grandchild1);
        using ContainerControl child2 = new();
        using Control grandchild2 = new();
        child2.Controls.Add(grandchild2);
        using TabControl child3 = new();
        using TabPage grandchild3 = new();
        child3.Controls.Add(grandchild3);
        using SubControl child4 = new();
        child4.SetStyle(ControlStyles.Selectable, false);
        using SubControl child5 = new()
        {
            Enabled = false
        };
        using SubControl child6 = new()
        {
            Visible = false
        };
        using SubControl child7 = new()
        {
            TabStop = false
        };
        using SubControl child8 = new()
        {
            CausesValidation = false
        };
        control.Controls.Add(child1);
        control.Controls.Add(child2);
        control.Controls.Add(child3);
        control.Controls.Add(child4);
        control.Controls.Add(child5);
        control.Controls.Add(child6);
        control.Controls.Add(child7);
        control.Controls.Add(child8);

        int validatingCallCount = 0;
        control.Validating += (sender, e) => validatingCallCount++;
        int validatedCallCount = 0;
        control.Validated += (sender, e) => validatedCallCount++;

        int child1ValidatingCallCount = 0;
        child1.Validating += (sender, e) =>
        {
            Assert.Same(child1, sender);
            Assert.False(e.Cancel);
            child1ValidatingCallCount++;
            e.Cancel = cancel;
        };
        int child1ValidatedCallCount = 0;
        child1.Validated += (sender, e) =>
        {
            Assert.Same(child1, sender);
            Assert.Same(EventArgs.Empty, e);
            child1ValidatedCallCount++;
        };
        int grandchild1ValidatingCallCount = 0;
        grandchild1.Validating += (sender, e) =>
        {
            Assert.Same(grandchild1, sender);
            Assert.False(e.Cancel);
            grandchild1ValidatingCallCount++;
        };
        int grandchild1ValidatedCallCount = 0;
        grandchild1.Validated += (sender, e) => grandchild1ValidatedCallCount++;
        int child2ValidatingCallCount = 0;
        child2.Validating += (sender, e) =>
        {
            Assert.Same(child2, sender);
            Assert.False(e.Cancel);
            child2ValidatingCallCount++;
        };
        int child2ValidatedCallCount = 0;
        child2.Validated += (sender, e) =>
        {
            Assert.Same(child2, sender);
            Assert.Same(EventArgs.Empty, e);
            child2ValidatedCallCount++;
        };
        int grandchild2ValidatingCallCount = 0;
        grandchild2.Validating += (sender, e) =>
        {
            Assert.Same(grandchild2, sender);
            Assert.False(e.Cancel);
            grandchild2ValidatingCallCount++;
        };
        int grandchild2ValidatedCallCount = 0;
        grandchild2.Validated += (sender, e) =>
        {
            Assert.Same(grandchild2, sender);
            Assert.Same(EventArgs.Empty, e);
            grandchild2ValidatedCallCount++;
        };
        int child3ValidatingCallCount = 0;
        child3.Validating += (sender, e) =>
        {
            Assert.Same(child3, sender);
            Assert.False(e.Cancel);
            child3ValidatingCallCount++;
        };
        int child3ValidatedCallCount = 0;
        child3.Validated += (sender, e) =>
        {
            Assert.Same(child3, sender);
            Assert.Same(EventArgs.Empty, e);
            child3ValidatedCallCount++;
        };
        int grandchild3ValidatingCallCount = 0;
        grandchild3.Validating += (sender, e) => grandchild3ValidatingCallCount++;
        int grandchild3ValidatedCallCount = 0;
        grandchild3.Validated += (sender, e) => grandchild3ValidatedCallCount++;
        int child4ValidatingCallCount = 0;
        child4.Validating += (sender, e) => child4ValidatingCallCount++;
        int child4ValidatedCallCount = 0;
        child4.Validated += (sender, e) => child4ValidatedCallCount++;
        int child5ValidatingCallCount = 0;
        child5.Validating += (sender, e) =>
        {
            Assert.Same(child5, sender);
            Assert.False(e.Cancel);
            child5ValidatingCallCount++;
        };
        int child5ValidatedCallCount = 0;
        child5.Validated += (sender, e) =>
        {
            Assert.Same(child5, sender);
            Assert.Same(EventArgs.Empty, e);
            child5ValidatedCallCount++;
        };
        int child6ValidatingCallCount = 0;
        child6.Validating += (sender, e) =>
        {
            Assert.Same(child6, sender);
            Assert.False(e.Cancel);
            child6ValidatingCallCount++;
        };
        int child6ValidatedCallCount = 0;
        child6.Validated += (sender, e) =>
        {
            Assert.Same(child6, sender);
            Assert.Same(EventArgs.Empty, e);
            child6ValidatedCallCount++;
        };
        int child7ValidatingCallCount = 0;
        child7.Validating += (sender, e) =>
        {
            Assert.Same(child7, sender);
            Assert.False(e.Cancel);
            child7ValidatingCallCount++;
        };
        int child7ValidatedCallCount = 0;
        child7.Validated += (sender, e) =>
        {
            Assert.Same(child7, sender);
            Assert.Same(EventArgs.Empty, e);
            child7ValidatedCallCount++;
        };
        int child8ValidatingCallCount = 0;
        child8.Validating += (sender, e) => child8ValidatingCallCount++;
        int child8ValidatedCallCount = 0;
        child8.Validated += (sender, e) => child8ValidatedCallCount++;

        Assert.Equal(!cancel, control.ValidateChildren(validationConstraints));
        Assert.Equal(0, validatingCallCount);
        Assert.Equal(0, validatedCallCount);
        Assert.Equal(1, child1ValidatingCallCount);
        Assert.Equal(expectedChild1CallCount, child1ValidatedCallCount);
        Assert.Equal(0, grandchild1ValidatingCallCount);
        Assert.Equal(0, grandchild1ValidatedCallCount);
        Assert.Equal(1, child2ValidatingCallCount);
        Assert.Equal(1, child2ValidatedCallCount);
        Assert.Equal(expectedGrandchild2CallCount, grandchild2ValidatingCallCount);
        Assert.Equal(expectedGrandchild2CallCount, grandchild2ValidatedCallCount);
        Assert.Equal(1, child3ValidatingCallCount);
        Assert.Equal(1, child3ValidatedCallCount);
        Assert.Equal(expectedGrandchild3CallCount, grandchild3ValidatingCallCount);
        Assert.Equal(expectedGrandchild3CallCount, grandchild3ValidatedCallCount);
        Assert.Equal(expectedChild4CallCount, child4ValidatingCallCount);
        Assert.Equal(expectedChild4CallCount, child4ValidatedCallCount);
        Assert.Equal(expectedChild5CallCount, child5ValidatingCallCount);
        Assert.Equal(expectedChild5CallCount, child5ValidatedCallCount);
        Assert.Equal(expectedChild6CallCount, child6ValidatingCallCount);
        Assert.Equal(expectedChild6CallCount, child6ValidatedCallCount);
        Assert.Equal(expectedChild7CallCount, child7ValidatingCallCount);
        Assert.Equal(expectedChild7CallCount, child7ValidatedCallCount);
        Assert.Equal(0, child8ValidatingCallCount);
        Assert.Equal(0, child8ValidatedCallCount);
    }

    [WinFormsTheory]
    [InlineData((ValidationConstraints)(-1))]
    [InlineData((ValidationConstraints)(0x20))]
    public void ContainerControl_ValidateChildren_InvalidValidationConstraints_ThrowsInvalidEnumArgumentException(ValidationConstraints validationConstraints)
    {
        using ContainerControl control = new();
        Assert.Throws<InvalidEnumArgumentException>("validationConstraints", () => control.ValidateChildren(validationConstraints));
    }

    [WinFormsFact]
    public void ContainerControl_WndProc_InvokeMouseHoverWithHandle_Success()
    {
        using SubContainerControl control = new();
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
    public void ContainerControl_WndProc_InvokeSetFocusWithHandle_Success()
    {
        using Control child1 = new();
        using Control child2 = new();
        using SubContainerControl control = new();
        control.Controls.Add(child1);
        control.Controls.Add(child2);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_SETFOCUS,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Null(control.ActiveControl);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ContainerControl_WndProc_InvokeSetFocusWithActiveControlWithHandle_Success()
    {
        using Control child1 = new();
        using Control child2 = new();
        using SubContainerControl control = new();
        control.Controls.Add(child1);
        control.Controls.Add(child2);
        control.ActiveControl = child2;
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_SETFOCUS,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(250, m.Result);
        Assert.Same(child2, control.ActiveControl);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ContainerControl_OnMove_HidesToolTip_IfToolTipIsShown()
    {
        using Control control = new();
        using SubContainerControl container = new();
        container.Controls.Add(control);
        container.SetTopLevel();
        container.CreateControl();

        // Simulate that a keyboard toolTip is shown
        KeyboardToolTipStateMachine instance = KeyboardToolTipStateMachine.Instance;
        instance.TestAccessor.Dynamic._currentTool = control;
        instance.TestAccessor.Dynamic._currentState = (byte)2;

        container.MoveContainer();
        IKeyboardToolTip currentTool = instance.TestAccessor.Dynamic._currentTool;
        string currentState = instance.TestAccessor.Dynamic._currentState.ToString();

        Assert.Null(currentTool);
        Assert.Equal("Hidden", currentState);

        Assert.True(control.IsHandleCreated);
        Assert.True(container.IsHandleCreated);
    }

    [WinFormsFact]
    public void ContainerControl_OnResize_HidesToolTip_IfToolTipIsShown()
    {
        using Control control = new();
        using SubContainerControl container = new();
        container.Controls.Add(control);
        container.SetTopLevel();
        container.CreateControl();

        // Simulate that a keyboard toolTip is shown
        KeyboardToolTipStateMachine instance = KeyboardToolTipStateMachine.Instance;
        instance.TestAccessor.Dynamic._currentTool = control;
        instance.TestAccessor.Dynamic._currentState = (byte)2;

        container.ResizeContainer();
        IKeyboardToolTip currentTool = instance.TestAccessor.Dynamic._currentTool;
        string currentState = instance.TestAccessor.Dynamic._currentState.ToString();

        Assert.Null(currentTool);
        Assert.Equal("Hidden", currentState);

        Assert.True(control.IsHandleCreated);
        Assert.True(container.IsHandleCreated);
    }

    [WinFormsFact]
    public void ContainerControl_OnResize_DoNothing_IfInstanceIsNotCreated()
    {
        using Control control = new();
        using SubContainerControl container = new();
        container.Controls.Add(control);
        container.CreateControl();

        container.ResizeContainer();
        object instance = typeof(KeyboardToolTipStateMachine).GetField("s_instance", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);

        Assert.Null(instance);
        Assert.True(control.IsHandleCreated);
        Assert.True(container.IsHandleCreated);
    }

    [WinFormsFact]
    public void ContainerControl_OnMove_DoNothing_IfInstanceIsNotCreated()
    {
        using Control control = new();
        using SubContainerControl container = new();
        container.Controls.Add(control);
        container.CreateControl();

        container.MoveContainer();
        object instance = typeof(KeyboardToolTipStateMachine).GetField("s_instance", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);

        Assert.Null(instance);
        Assert.True(control.IsHandleCreated);
        Assert.True(container.IsHandleCreated);
    }

    private class SubControl : Control
    {
        public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);
    }

    public class SubContainerControl : ContainerControl
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

        public void MoveContainer() => base.OnMove(EventArgs.Empty);

        public void ResizeContainer() => base.OnResize(EventArgs.Empty);

        public new bool ResizeRedraw
        {
            get => base.ResizeRedraw;
            set => base.ResizeRedraw = value;
        }

        public new bool ShowFocusCues => base.ShowFocusCues;

        public new bool ShowKeyboardCues => base.ShowKeyboardCues;

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

        public new void OnAutoValidateChanged(EventArgs e) => base.OnAutoValidateChanged(e);

        public new void OnCreateControl() => base.OnCreateControl();

        public new void OnFontChanged(EventArgs e) => base.OnFontChanged(e);

        public new void OnLayout(LayoutEventArgs e) => base.OnLayout(e);

        public new void OnParentChanged(EventArgs e) => base.OnParentChanged(e);

        public void SetTopLevel() => SetTopLevel(true);

        public new void UpdateDefaultButton() => base.UpdateDefaultButton();

        public new void WndProc(ref Message m) => base.WndProc(ref m);
    }
}
