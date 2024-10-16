// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Layout;
using System.Windows.Forms.TestUtilities;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Tests;

public class GroupBoxTests
{
    [WinFormsFact]
    public void GroupBox_Ctor_Default()
    {
        using SubGroupBox control = new();
        Assert.Null(control.AccessibleDefaultActionDescription);
        Assert.Null(control.AccessibleDescription);
        Assert.Null(control.AccessibleName);
        Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
        Assert.False(control.AllowDrop);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        Assert.False(control.AutoSize);
        Assert.Equal(AutoSizeMode.GrowOnly, control.AutoSizeMode);
        Assert.Equal(Control.DefaultBackColor, control.BackColor);
        Assert.Null(control.BackgroundImage);
        Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
        Assert.Null(control.BindingContext);
        Assert.Equal(100, control.Bottom);
        Assert.Equal(new Rectangle(0, 0, 200, 100), control.Bounds);
        Assert.True(control.CanEnableIme);
        Assert.False(control.CanFocus);
        Assert.True(control.CanRaiseEvents);
        Assert.False(control.CanSelect);
        Assert.False(control.Capture);
        Assert.True(control.CausesValidation);
        Assert.Equal(new Size(200, 100), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, 200, 100), control.ClientRectangle);
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
        Assert.Equal(new Padding(3), control.DefaultPadding);
        Assert.Equal(new Size(200, 100), control.DefaultSize);
        Assert.False(control.DesignMode);
        Assert.Equal(new Rectangle(3, Control.DefaultFont.Height + 3, 194, 94 - Control.DefaultFont.Height), control.DisplayRectangle);
        Assert.Equal(DockStyle.None, control.Dock);
        Assert.False(control.DoubleBuffered);
        Assert.True(control.Enabled);
        Assert.NotNull(control.Events);
        Assert.Same(control.Events, control.Events);
        Assert.Equal(FlatStyle.Standard, control.FlatStyle);
        Assert.False(control.Focused);
        Assert.Equal(Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.Equal(Control.DefaultForeColor, control.ForeColor);
        Assert.False(control.HasChildren);
        Assert.Equal(100, control.Height);
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
        Assert.Equal(new Padding(3), control.Padding);
        Assert.Null(control.Parent);
        Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
        Assert.Equal(new Size(6, 6 + Control.DefaultFont.Height), control.PreferredSize);
        Assert.False(control.RecreatingHandle);
        Assert.Null(control.Region);
        Assert.True(control.ResizeRedraw);
        Assert.Equal(200, control.Right);
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.True(control.ShowFocusCues);
        Assert.True(control.ShowKeyboardCues);
        Assert.Null(control.Site);
        Assert.Equal(new Size(200, 100), control.Size);
        Assert.Equal(0, control.TabIndex);
        Assert.False(control.TabStop);
        Assert.Empty(control.Text);
        Assert.Equal(0, control.Top);
        Assert.Null(control.TopLevelControl);
        Assert.True(control.UseCompatibleTextRendering);
        Assert.False(control.UseWaitCursor);
        Assert.True(control.Visible);
        Assert.Equal(200, control.Width);

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void GroupBox_CreateParams_GetDefault_ReturnsExpected()
    {
        using SubGroupBox control = new();
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Null(createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0x10000, createParams.ExStyle);
        Assert.Equal(100, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(0x56000000, createParams.Style);
        Assert.Equal(200, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(FlatStyle.Flat, null, 0x56000000)]
    [InlineData(FlatStyle.Popup, null, 0x56000000)]
    [InlineData(FlatStyle.Standard, null, 0x56000000)]
    [InlineData(FlatStyle.System, "Button", 0x56000007)]
    public void GroupBox_CreateParams_GetFlatStyle_ReturnsExpected(FlatStyle flatStyle, string expectedClassName, int expectedStyle)
    {
        using SubGroupBox control = new()
        {
            FlatStyle = flatStyle
        };

        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Equal(expectedClassName, createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0x10000, createParams.ExStyle);
        Assert.Equal(100, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(expectedStyle, createParams.Style);
        Assert.Equal(200, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ProgressBar_AllowDrop_Set_GetReturnsExpected(bool value)
    {
        using GroupBox control = new()
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
    [BoolData]
    public void GroupBox_AutoSize_Set_GetReturnsExpected(bool value)
    {
        using GroupBox control = new();
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
    public void GroupBox_AutoSize_SetWithHandler_CallsAutoSizeChanged()
    {
        using GroupBox control = new()
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
    [EnumData<AutoSizeMode>]
    public void GroupBox_AutoSizeMode_Set_GetReturnsExpected(AutoSizeMode value)
    {
        using SubGroupBox control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.AutoSizeMode = value;
        Assert.Equal(value, control.AutoSizeMode);
        Assert.Equal(value, control.GetAutoSizeMode());
        Assert.False(control.IsHandleCreated);
        Assert.Equal(0, layoutCallCount);

        // Set same.
        control.AutoSizeMode = value;
        Assert.Equal(value, control.AutoSizeMode);
        Assert.Equal(value, control.GetAutoSizeMode());
        Assert.False(control.IsHandleCreated);
        Assert.Equal(0, layoutCallCount);
    }

    [WinFormsTheory]
    [InlineData(AutoSizeMode.GrowAndShrink, 1)]
    [InlineData(AutoSizeMode.GrowOnly, 0)]
    public void GroupBox_AutoSizeMode_SetWithParent_GetReturnsExpected(AutoSizeMode value, int expectedLayoutCallCount)
    {
        using Control parent = new();
        using SubGroupBox control = new()
        {
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("AutoSize", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.AutoSizeMode = value;
            Assert.Equal(value, control.AutoSizeMode);
            Assert.Equal(value, control.GetAutoSizeMode());
            Assert.False(control.IsHandleCreated);
            Assert.Equal(0, layoutCallCount);
            Assert.False(parent.IsHandleCreated);
            Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);

            // Set same.
            control.AutoSizeMode = value;
            Assert.Equal(value, control.AutoSizeMode);
            Assert.Equal(value, control.GetAutoSizeMode());
            Assert.False(control.IsHandleCreated);
            Assert.Equal(0, layoutCallCount);
            Assert.False(parent.IsHandleCreated);
            Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(AutoSizeMode.GrowAndShrink, 1)]
    [InlineData(AutoSizeMode.GrowOnly, 0)]
    public void GroupBox_AutoSizeMode_SetWithCustomLayoutEngineParent_GetReturnsExpected(AutoSizeMode value, int expectedLayoutCallCount)
    {
        using CustomLayoutEngineControl parent = new();
        using SubGroupBox control = new()
        {
            Parent = parent
        };
        parent.SetLayoutEngine(new SubLayoutEngine());
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("AutoSize", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;
        try
        {
            control.AutoSizeMode = value;
            Assert.Equal(value, control.AutoSizeMode);
            Assert.Equal(value, control.GetAutoSizeMode());
            Assert.False(control.IsHandleCreated);
            Assert.Equal(0, layoutCallCount);
            Assert.False(parent.IsHandleCreated);
            Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);

            // Set same.
            control.AutoSizeMode = value;
            Assert.Equal(value, control.AutoSizeMode);
            Assert.Equal(value, control.GetAutoSizeMode());
            Assert.False(control.IsHandleCreated);
            Assert.Equal(0, layoutCallCount);
            Assert.False(parent.IsHandleCreated);
            Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    private class CustomLayoutEngineControl : Control
    {
        private LayoutEngine _layoutEngine;

        public CustomLayoutEngineControl()
        {
            _layoutEngine = base.LayoutEngine;
        }

        public void SetLayoutEngine(LayoutEngine layoutEngine)
        {
            _layoutEngine = layoutEngine;
        }

        public override LayoutEngine LayoutEngine => _layoutEngine;
    }

    private class SubLayoutEngine : LayoutEngine
    {
    }

    [WinFormsTheory]
    [EnumData<AutoSizeMode>]
    public void GroupBox_AutoSizeMode_SetWithHandle_GetReturnsExpected(AutoSizeMode value)
    {
        using SubGroupBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.AutoSizeMode = value;
        Assert.Equal(value, control.AutoSizeMode);
        Assert.Equal(value, control.GetAutoSizeMode());
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.AutoSizeMode = value;
        Assert.Equal(value, control.AutoSizeMode);
        Assert.Equal(value, control.GetAutoSizeMode());
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(AutoSizeMode.GrowAndShrink, 1)]
    [InlineData(AutoSizeMode.GrowOnly, 0)]
    public void GroupBox_AutoSizeMode_SetWithParentWithHandle_GetReturnsExpected(AutoSizeMode value, int expectedLayoutCallCount)
    {
        using Control parent = new();
        using SubGroupBox control = new()
        {
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("AutoSize", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int parentInvalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

        try
        {
            control.AutoSizeMode = value;
            Assert.Equal(value, control.AutoSizeMode);
            Assert.Equal(value, control.GetAutoSizeMode());
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Set same.
            control.AutoSizeMode = value;
            Assert.Equal(value, control.AutoSizeMode);
            Assert.Equal(value, control.GetAutoSizeMode());
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(AutoSizeMode.GrowAndShrink, 1)]
    [InlineData(AutoSizeMode.GrowOnly, 0)]
    public void GroupBox_AutoSizeMode_SetWithCustomLayoutEngineParentWithHandle_GetReturnsExpected(AutoSizeMode value, int expectedLayoutCallCount)
    {
        using CustomLayoutEngineControl parent = new();
        using SubGroupBox control = new()
        {
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("AutoSize", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int parentInvalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

        try
        {
            control.AutoSizeMode = value;
            Assert.Equal(value, control.AutoSizeMode);
            Assert.Equal(value, control.GetAutoSizeMode());
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Set same.
            control.AutoSizeMode = value;
            Assert.Equal(value, control.AutoSizeMode);
            Assert.Equal(value, control.GetAutoSizeMode());
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InvalidEnumData<AutoSizeMode>]
    public void GroupBox_AutoSizeMode_SetInvalid_ThrowsInvalidEnumArgumentException(AutoSizeMode value)
    {
        using GroupBox control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.AutoSizeMode = value);
    }

    [WinFormsFact]
    public void GroupBox_DisplayRectangle_Get_ReturnsExpected()
    {
        using GroupBox control = new();
        Rectangle result = control.DisplayRectangle;
        Assert.Equal(new Rectangle(3, Control.DefaultFont.Height + 3, 194, 94 - Control.DefaultFont.Height), control.DisplayRectangle);
        Assert.Equal(result, control.DisplayRectangle);
    }

    [WinFormsFact]
    public void GroupBox_DisplayRectangle_GetWithCustomFontSetAfterCallingDisplayRectangle_ReturnsExpected()
    {
        using Font font = new("Arial", 8.25f);
        using GroupBox control = new();
        Assert.Equal(new Rectangle(3, Control.DefaultFont.Height + 3, 194, 94 - Control.DefaultFont.Height), control.DisplayRectangle);

        control.Font = font;
        Rectangle result = control.DisplayRectangle;
        Assert.Equal(new Rectangle(3, font.Height + 3, 194, 94 - font.Height), control.DisplayRectangle);
        Assert.Equal(result, control.DisplayRectangle);
    }

    [WinFormsFact]
    public void GroupBox_DisplayRectangle_GetWithCustomFontSetBeforeCallingDisplayRectangle_ReturnsExpected()
    {
        using Font font = new("Arial", 8.25f);
        using GroupBox control = new()
        {
            Font = font
        };
        Rectangle result = control.DisplayRectangle;
        Assert.Equal(new Rectangle(3, font.Height + 3, 194, 94 - font.Height), control.DisplayRectangle);
        Assert.Equal(result, control.DisplayRectangle);
    }

    [WinFormsFact]
    public void GroupBox_FontPropertyChange_UpdatesFontHeightAndCachedFont()
    {
        using GroupBox groupBox = new();
        var originalFont = groupBox.Font;
        using Font newFont = new(originalFont.FontFamily, originalFont.Size + 5, originalFont.Style);

        groupBox.Font = newFont;
        Rectangle result = groupBox.DisplayRectangle;
        var accessor = groupBox.TestAccessor();

        int updatedFontHeight = accessor.Dynamic._fontHeight;
        Font updatedCachedFont = accessor.Dynamic._cachedFont;

        updatedFontHeight.Should().Be(newFont.Height);
        updatedCachedFont.Should().BeEquivalentTo(newFont);
    }

    [WinFormsTheory]
    [InlineData(FlatStyle.Flat, true, true, true)]
    [InlineData(FlatStyle.Popup, true, true, true)]
    [InlineData(FlatStyle.Standard, false, true, false)]
    [InlineData(FlatStyle.System, true, false, false)]
    public void GroupBox_FlatStyle_Set_GetReturnsExpected(FlatStyle value, bool containerControl, bool ownerDraw, bool userMouse)
    {
        using SubGroupBox control = new();
        control.SetStyle(ControlStyles.ContainerControl, false);

        control.FlatStyle = value;
        Assert.Equal(value, control.FlatStyle);
        Assert.Equal(containerControl, control.GetStyle(ControlStyles.ContainerControl));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.SupportsTransparentBackColor));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.UserPaint));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.ResizeRedraw));
        Assert.Equal(userMouse, control.GetStyle(ControlStyles.UserMouse));
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.FlatStyle = value;
        Assert.Equal(value, control.FlatStyle);
        Assert.Equal(containerControl, control.GetStyle(ControlStyles.ContainerControl));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.SupportsTransparentBackColor));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.UserPaint));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.ResizeRedraw));
        Assert.Equal(userMouse, control.GetStyle(ControlStyles.UserMouse));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(FlatStyle.Flat, FlatStyle.Flat, false, true, true)]
    [InlineData(FlatStyle.Flat, FlatStyle.Popup, true, true, true)]
    [InlineData(FlatStyle.Flat, FlatStyle.Standard, true, true, true)]
    [InlineData(FlatStyle.Flat, FlatStyle.System, true, false, false)]
    [InlineData(FlatStyle.Popup, FlatStyle.Flat, true, true, true)]
    [InlineData(FlatStyle.Popup, FlatStyle.Popup, false, true, true)]
    [InlineData(FlatStyle.Popup, FlatStyle.Standard, true, true, true)]
    [InlineData(FlatStyle.Popup, FlatStyle.System, true, false, false)]
    [InlineData(FlatStyle.Standard, FlatStyle.Flat, true, true, true)]
    [InlineData(FlatStyle.Standard, FlatStyle.Popup, true, true, true)]
    [InlineData(FlatStyle.Standard, FlatStyle.Standard, false, true, false)]
    [InlineData(FlatStyle.Standard, FlatStyle.System, true, false, false)]
    [InlineData(FlatStyle.System, FlatStyle.Flat, true, true, true)]
    [InlineData(FlatStyle.System, FlatStyle.Popup, true, true, true)]
    [InlineData(FlatStyle.System, FlatStyle.Standard, true, true, true)]
    [InlineData(FlatStyle.System, FlatStyle.System, false, false, false)]
    public void GroupBox_FlatStyle_SetWithCustomOldValue_GetReturnsExpected(FlatStyle oldValue, FlatStyle value, bool containerControl, bool ownerDraw, bool userMouse)
    {
        using SubGroupBox control = new()
        {
            FlatStyle = oldValue
        };
        control.SetStyle(ControlStyles.ContainerControl, false);

        control.FlatStyle = value;
        Assert.Equal(value, control.FlatStyle);
        Assert.Equal(containerControl, control.GetStyle(ControlStyles.ContainerControl));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.SupportsTransparentBackColor));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.UserPaint));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.ResizeRedraw));
        Assert.Equal(userMouse, control.GetStyle(ControlStyles.UserMouse));
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.FlatStyle = value;
        Assert.Equal(containerControl, control.GetStyle(ControlStyles.ContainerControl));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.SupportsTransparentBackColor));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.UserPaint));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.ResizeRedraw));
        Assert.Equal(userMouse, control.GetStyle(ControlStyles.UserMouse));
        Assert.Equal(value, control.FlatStyle);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(FlatStyle.Flat, true, true, true, 1, 0)]
    [InlineData(FlatStyle.Popup, true, true, true, 1, 0)]
    [InlineData(FlatStyle.Standard, false, true, false, 0, 0)]
    [InlineData(FlatStyle.System, true, false, false, 0, 1)]
    public void GroupBox_FlatStyle_VisualStyles_off_SetWithHandle_GetReturnsExpected(FlatStyle value, bool containerControl, bool ownerDraw, bool userMouse, int expectedInvalidatedCallCount, int expectedCreatedCallCount)
    {
        if (Application.UseVisualStyles)
        {
            return;
        }

        using SubGroupBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        control.SetStyle(ControlStyles.ContainerControl, false);

        control.FlatStyle = value;
        Assert.Equal(value, control.FlatStyle);
        Assert.Equal(containerControl, control.GetStyle(ControlStyles.ContainerControl));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.SupportsTransparentBackColor));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.UserPaint));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.ResizeRedraw));
        Assert.Equal(userMouse, control.GetStyle(ControlStyles.UserMouse));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        control.FlatStyle = value;
        Assert.Equal(value, control.FlatStyle);
        Assert.Equal(containerControl, control.GetStyle(ControlStyles.ContainerControl));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.SupportsTransparentBackColor));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.UserPaint));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.ResizeRedraw));
        Assert.Equal(userMouse, control.GetStyle(ControlStyles.UserMouse));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(FlatStyle.Flat, true, true, true, 1, 0)]
    [InlineData(FlatStyle.Popup, true, true, true, 1, 0)]
    [InlineData(FlatStyle.Standard, false, true, false, 0, 0)]
    [InlineData(FlatStyle.System, true, false, false, 1, 1)]
    public void GroupBox_FlatStyle_VisualStyles_on_SetWithHandle_GetReturnsExpected(
        FlatStyle value,
        bool containerControl,
        bool ownerDraw,
        bool userMouse,
        int expectedInvalidatedCallCount,
        int expectedCreatedCallCount)
    {
        if (!Application.UseVisualStyles)
        {
            return;
        }

        using SubGroupBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        control.SetStyle(ControlStyles.ContainerControl, false);

        control.FlatStyle = value;
        Assert.Equal(value, control.FlatStyle);
        Assert.Equal(containerControl, control.GetStyle(ControlStyles.ContainerControl));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.SupportsTransparentBackColor));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.UserPaint));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.ResizeRedraw));
        Assert.Equal(userMouse, control.GetStyle(ControlStyles.UserMouse));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        control.FlatStyle = value;
        Assert.Equal(value, control.FlatStyle);
        Assert.Equal(containerControl, control.GetStyle(ControlStyles.ContainerControl));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.SupportsTransparentBackColor));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.UserPaint));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.ResizeRedraw));
        Assert.Equal(userMouse, control.GetStyle(ControlStyles.UserMouse));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(FlatStyle.Flat, FlatStyle.Flat, false, true, true, 0, 0)]
    [InlineData(FlatStyle.Flat, FlatStyle.Popup, true, true, true, 1, 0)]
    [InlineData(FlatStyle.Flat, FlatStyle.Standard, true, true, true, 1, 0)]
    [InlineData(FlatStyle.Flat, FlatStyle.System, true, false, false, 0, 1)]
    [InlineData(FlatStyle.Popup, FlatStyle.Flat, true, true, true, 1, 0)]
    [InlineData(FlatStyle.Popup, FlatStyle.Popup, false, true, true, 0, 0)]
    [InlineData(FlatStyle.Popup, FlatStyle.Standard, true, true, true, 1, 0)]
    [InlineData(FlatStyle.Popup, FlatStyle.System, true, false, false, 0, 1)]
    [InlineData(FlatStyle.Standard, FlatStyle.Flat, true, true, true, 1, 0)]
    [InlineData(FlatStyle.Standard, FlatStyle.Popup, true, true, true, 1, 0)]
    [InlineData(FlatStyle.Standard, FlatStyle.Standard, false, true, false, 0, 0)]
    [InlineData(FlatStyle.Standard, FlatStyle.System, true, false, false, 0, 1)]
    [InlineData(FlatStyle.System, FlatStyle.Flat, true, true, true, 0, 1)]
    [InlineData(FlatStyle.System, FlatStyle.Popup, true, true, true, 0, 1)]
    [InlineData(FlatStyle.System, FlatStyle.Standard, true, true, true, 0, 1)]
    [InlineData(FlatStyle.System, FlatStyle.System, false, false, false, 0, 0)]
    public void GroupBox_FlatStyle_VisualStyles_off_SetWithCustomOldValueWithHandle_GetReturnsExpected(FlatStyle oldValue, FlatStyle value, bool containerControl, bool ownerDraw, bool userMouse, int expectedInvalidatedCallCount, int expectedCreatedCallCount)
    {
        if (Application.UseVisualStyles)
        {
            return;
        }

        using SubGroupBox control = new()
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
        control.SetStyle(ControlStyles.ContainerControl, false);

        control.FlatStyle = value;
        Assert.Equal(value, control.FlatStyle);
        Assert.Equal(containerControl, control.GetStyle(ControlStyles.ContainerControl));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.SupportsTransparentBackColor));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.UserPaint));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.ResizeRedraw));
        Assert.Equal(userMouse, control.GetStyle(ControlStyles.UserMouse));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        control.FlatStyle = value;
        Assert.Equal(value, control.FlatStyle);
        Assert.Equal(containerControl, control.GetStyle(ControlStyles.ContainerControl));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.SupportsTransparentBackColor));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.UserPaint));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.ResizeRedraw));
        Assert.Equal(userMouse, control.GetStyle(ControlStyles.UserMouse));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(FlatStyle.Flat, FlatStyle.Flat, false, true, true, 0, 0)]
    [InlineData(FlatStyle.Flat, FlatStyle.Popup, true, true, true, 1, 0)]
    [InlineData(FlatStyle.Flat, FlatStyle.Standard, true, true, true, 1, 0)]
    [InlineData(FlatStyle.Flat, FlatStyle.System, true, false, false, 0, 1)]
    [InlineData(FlatStyle.Popup, FlatStyle.Flat, true, true, true, 1, 0)]
    [InlineData(FlatStyle.Popup, FlatStyle.Popup, false, true, true, 0, 0)]
    [InlineData(FlatStyle.Popup, FlatStyle.Standard, true, true, true, 1, 0)]
    [InlineData(FlatStyle.Popup, FlatStyle.System, true, false, false, 0, 1)]
    [InlineData(FlatStyle.Standard, FlatStyle.Flat, true, true, true, 1, 0)]
    [InlineData(FlatStyle.Standard, FlatStyle.Popup, true, true, true, 1, 0)]
    [InlineData(FlatStyle.Standard, FlatStyle.Standard, false, true, false, 0, 0)]
    [InlineData(FlatStyle.Standard, FlatStyle.System, true, false, false, 0, 1)]
    [InlineData(FlatStyle.System, FlatStyle.Flat, true, true, true, 0, 1)]
    [InlineData(FlatStyle.System, FlatStyle.Popup, true, true, true, 0, 1)]
    [InlineData(FlatStyle.System, FlatStyle.Standard, true, true, true, 0, 1)]
    [InlineData(FlatStyle.System, FlatStyle.System, false, false, false, 0, 0)]
    public void GroupBox_FlatStyle_VisualStyles_on_SetWithCustomOldValue_GetReturnsExpected(FlatStyle oldValue, FlatStyle value, bool containerControl, bool ownerDraw, bool userMouse, int expectedInvalidatedCallCount, int expectedCreatedCallCount)
    {
        if (!Application.UseVisualStyles)
        {
            return;
        }

        using SubGroupBox control = new()
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
        control.SetStyle(ControlStyles.ContainerControl, false);

        control.FlatStyle = value;
        Assert.Equal(value, control.FlatStyle);
        Assert.Equal(containerControl, control.GetStyle(ControlStyles.ContainerControl));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.SupportsTransparentBackColor));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.UserPaint));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.ResizeRedraw));
        Assert.Equal(userMouse, control.GetStyle(ControlStyles.UserMouse));
        Assert.True(control.IsHandleCreated);
        Assert.True(invalidatedCallCount >= expectedInvalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        control.FlatStyle = value;
        Assert.Equal(value, control.FlatStyle);
        Assert.Equal(containerControl, control.GetStyle(ControlStyles.ContainerControl));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.SupportsTransparentBackColor));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.UserPaint));
        Assert.Equal(ownerDraw, control.GetStyle(ControlStyles.ResizeRedraw));
        Assert.Equal(userMouse, control.GetStyle(ControlStyles.UserMouse));
        Assert.True(control.IsHandleCreated);
        Assert.True(invalidatedCallCount >= expectedInvalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);
    }

    [WinFormsFact]
    public void GroupBox_PreferredSize_GetWithChildrenSimple_ReturnsExpected()
    {
        using GroupBox control = new();
        using Control child = new()
        {
            Size = new Size(16, 20)
        };
        control.Controls.Add(child);
        Assert.Equal(new Size(22, 25), control.PreferredSize);
    }

    [WinFormsFact]
    public void GroupBox_PreferredSize_GetWithBorder_ReturnsExpected()
    {
        using BorderedGroupBox control = new()
        {
            Padding = new Padding(1, 2, 3, 4)
        };
        Assert.Equal(new Size(8, 9), control.PreferredSize);
    }

    private class BorderedGroupBox : GroupBox
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= (int)WINDOW_STYLE.WS_BORDER;
                cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_STATICEDGE;
                return cp;
            }
        }
    }

    [WinFormsTheory]
    [BoolData]
    public void GroupBox_TabStop_Set_GetReturnsExpected(bool value)
    {
        using GroupBox control = new()
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
    public void GroupBox_TabStop_SetWithHandle_GetReturnsExpected(bool value)
    {
        using GroupBox control = new();
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
    public void GroupBox_TabStop_SetWithHandler_CallsTabStopChanged()
    {
        using GroupBox control = new()
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
    public void GroupBox_Text_Set_GetReturnsExpected(string value, string expected)
    {
        using GroupBox control = new()
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
    public void GroupBox_Text_SetInvisible_GetReturnsExpected(string value, string expected)
    {
        using GroupBox control = new()
        {
            Visible = false,
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
    public void GroupBox_Text_SetWithHandle_GetReturnsExpected(string value, string expected)
    {
        using GroupBox control = new();
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
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void GroupBox_Text_SetInvisibleWithHandle_GetReturnsExpected(string value, string expected)
    {
        using GroupBox control = new()
        {
            Visible = false
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
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void GroupBox_Text_SetWithHandler_CallsTextChanged()
    {
        using GroupBox control = new();
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
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void GroupBox_UseCompatibleTextRendering_Set_GetReturnsExpected(bool autoSize, bool value)
    {
        using GroupBox control = new()
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
    public void GroupBox_UseCompatibleTextRendering_SetWithParent_GetReturnsExpected(bool autoSize, bool value, int expectedParentLayoutCallCount1, int expectedParentLayoutCallCount2)
    {
        using Control parent = new();
        using GroupBox control = new()
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
    public void GroupBox_UseCompatibleTextRendering_SetWithHandle_GetReturnsExpected(bool autoSize, bool value, int expectedInvalidatedCallCount)
    {
        using GroupBox control = new()
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
    [InlineData(true, true, 0, 0, 1)]
    [InlineData(true, false, 1, 1, 2)]
    [InlineData(false, true, 0, 0, 0)]
    [InlineData(false, false, 1, 0, 0)]
    public void GroupBox_UseCompatibleTextRendering_SetWithParentWithHandle_GetReturnsExpected(bool autoSize, bool value, int expectedInvalidatedCallCount, int expectedParentLayoutCallCount1, int expectedParentLayoutCallCount2)
    {
        using Control parent = new();
        using GroupBox control = new()
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
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.UseCompatibleTextRendering = value;
            Assert.Equal(value, control.UseCompatibleTextRendering);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount1, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.UseCompatibleTextRendering = !value;
            Assert.Equal(!value, control.UseCompatibleTextRendering);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount2, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void GroupBox_CreateAccessibilityInstance_Invoke_ReturnsExpected()
    {
        using SubGroupBox control = new();
        Control.ControlAccessibleObject instance = Assert.IsAssignableFrom<Control.ControlAccessibleObject>(control.CreateAccessibilityInstance());
        Assert.NotNull(instance);
        Assert.Same(control, instance.Owner);
        Assert.Equal(AccessibleRole.Grouping, instance.Role);
        Assert.NotSame(control.CreateAccessibilityInstance(), instance);
        Assert.NotSame(control.AccessibilityObject, instance);
    }

    [WinFormsFact]
    public void GroupBox_CreateAccessibilityInstance_InvokeWithCustomRole_ReturnsExpected()
    {
        using SubGroupBox control = new()
        {
            AccessibleRole = AccessibleRole.HelpBalloon
        };
        Control.ControlAccessibleObject instance = Assert.IsAssignableFrom<Control.ControlAccessibleObject>(control.CreateAccessibilityInstance());
        Assert.NotNull(instance);
        Assert.Same(control, instance.Owner);
        Assert.Equal(AccessibleRole.HelpBalloon, instance.Role);
        Assert.NotSame(control.CreateAccessibilityInstance(), instance);
        Assert.NotSame(control.AccessibilityObject, instance);
    }

    [WinFormsFact]
    public void GroupBox_GetAutoSizeMode_Invoke_ReturnsExpected()
    {
        using SubGroupBox control = new();
        Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
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
    [InlineData(ControlStyles.OptimizedDoubleBuffer, false)]
    [InlineData(ControlStyles.UseTextForAccessibility, true)]
    [InlineData((ControlStyles)0, true)]
    [InlineData((ControlStyles)int.MaxValue, false)]
    [InlineData((ControlStyles)(-1), false)]
    public void GroupBox_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
    {
        using SubGroupBox control = new();
        Assert.Equal(expected, control.GetStyle(flag));

        // Call again to test caching.
        Assert.Equal(expected, control.GetStyle(flag));
    }

    [WinFormsFact]
    public void GroupBox_GetTopLevel_Invoke_ReturnsExpected()
    {
        using SubGroupBox control = new();
        Assert.False(control.GetTopLevel());
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void GroupBox_OnClick_Invoke_CallsClick(EventArgs eventArgs)
    {
        using SubGroupBox control = new();
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
    public void GroupBox_OnDoubleClick_Invoke_CallsDoubleClick(EventArgs eventArgs)
    {
        using SubGroupBox control = new();
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
    public void GroupBox_OnFontChanged_Invoke_CallsFontChanged(EventArgs eventArgs)
    {
        using SubGroupBox control = new();
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
        Assert.Equal(Control.DefaultFont.Height, control.FontHeight);
        Assert.Equal(new Rectangle(3, Control.DefaultFont.Height + 3, 194, 94 - Control.DefaultFont.Height), control.DisplayRectangle);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.FontChanged -= handler;
        control.OnFontChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(Control.DefaultFont.Height, control.FontHeight);
        Assert.Equal(new Rectangle(3, Control.DefaultFont.Height + 3, 194, 94 - Control.DefaultFont.Height), control.DisplayRectangle);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> OnFontChanged_WithHandle_TestData()
    {
        foreach (bool userPaint in new bool[] { true, false })
        {
            yield return new object[] { userPaint, null };
            yield return new object[] { userPaint, new EventArgs() };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnFontChanged_WithHandle_TestData))]
    public void GroupBox_OnFontChanged_InvokeWithHandle_CallsFontChangedAndInvalidated(bool userPaint, EventArgs eventArgs)
    {
        using SubGroupBox control = new();
        control.SetStyle(ControlStyles.UserPaint, userPaint);
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
        control.FontChanged += handler;
        control.OnFontChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(Control.DefaultFont.Height, control.FontHeight);
        Assert.Equal(new Rectangle(3, Control.DefaultFont.Height + 3, 194, 94 - Control.DefaultFont.Height), control.DisplayRectangle);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.FontChanged -= handler;
        control.OnFontChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(Control.DefaultFont.Height, control.FontHeight);
        Assert.Equal(new Rectangle(3, Control.DefaultFont.Height + 3, 194, 94 - Control.DefaultFont.Height), control.DisplayRectangle);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(4, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetKeyEventArgsTheoryData))]
    public void GroupBox_OnKeyDown_Invoke_CallsKeyDown(KeyEventArgs eventArgs)
    {
        using SubGroupBox control = new();
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
    public void GroupBox_OnKeyPress_Invoke_CallsKeyPress(KeyPressEventArgs eventArgs)
    {
        using SubGroupBox control = new();
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
    public void GroupBox_OnKeyUp_Invoke_CallsKeyUp(KeyEventArgs eventArgs)
    {
        using SubGroupBox control = new();
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
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetMouseEventArgsTheoryData))]
    public void GroupBox_OnMouseClick_Invoke_CallsMouseClick(MouseEventArgs eventArgs)
    {
        using SubGroupBox control = new();
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
    public void GroupBox_OnMouseDoubleClick_Invoke_CallsMouseDoubleClick(MouseEventArgs eventArgs)
    {
        using SubGroupBox control = new();
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
    public void GroupBox_OnMouseDown_Invoke_CallsMouseDown(MouseEventArgs eventArgs)
    {
        using SubGroupBox control = new();
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
    [NewAndDefaultData<EventArgs>]
    public void GroupBox_OnMouseEnter_Invoke_CallsMouseEnter(EventArgs eventArgs)
    {
        using SubGroupBox control = new();
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
    public void GroupBox_OnMouseLeave_Invoke_CallsMouseLeave(EventArgs eventArgs)
    {
        using SubGroupBox control = new();
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
    public void GroupBox_OnMouseMove_Invoke_CallsMouseMove(MouseEventArgs eventArgs)
    {
        using SubGroupBox control = new();
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
    public void GroupBox_OnMouseUp_Invoke_CallsMouseUp(MouseEventArgs eventArgs)
    {
        using SubGroupBox control = new();
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

    public static IEnumerable<object[]> OnPaint_TestData()
    {
        foreach (bool enabled in new bool[] { true, false })
        {
            foreach (bool useCompatibleTextRendering in new bool[] { true, false })
            {
                foreach (RightToLeft rightToLeft in Enum.GetValues(typeof(RightToLeft)))
                {
                    foreach (Color foreColor in new Color[] { Color.Red, Color.Empty })
                    {
                        foreach (string text in new string[] { null, string.Empty, "text" })
                        {
                            yield return new object[] { new Size(100, 200), enabled, useCompatibleTextRendering, rightToLeft, foreColor, text };
                            yield return new object[] { new Size(10, 10), enabled, useCompatibleTextRendering, rightToLeft, foreColor, text };
                            yield return new object[] { new Size(9, 10), enabled, useCompatibleTextRendering, rightToLeft, foreColor, text };
                            yield return new object[] { new Size(10, 9), enabled, useCompatibleTextRendering, rightToLeft, foreColor, text };
                            yield return new object[] { new Size(9, 9), enabled, useCompatibleTextRendering, rightToLeft, foreColor, text };
                        }
                    }
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnPaint_TestData))]
    public void GroupBox_OnPaint_Invoke_CallsPaint(Size size, bool enabled, bool useCompatibleTextRendering, RightToLeft rightToLeft, Color foreColor, string text)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using PaintEventArgs eventArgs = new(graphics, Rectangle.Empty);

        using SubGroupBox control = new()
        {
            Size = size,
            Enabled = enabled,
            UseCompatibleTextRendering = useCompatibleTextRendering,
            RightToLeft = rightToLeft,
            ForeColor = foreColor,
            Text = text
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
    public void GroupBox_OnPaint_InvokeWithHandle_CallsPaint(Size size, bool enabled, bool useCompatibleTextRendering, RightToLeft rightToLeft, Color foreColor, string text)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using PaintEventArgs eventArgs = new(graphics, Rectangle.Empty);

        using SubGroupBox control = new()
        {
            Size = size,
            Enabled = enabled,
            UseCompatibleTextRendering = useCompatibleTextRendering,
            RightToLeft = rightToLeft,
            ForeColor = foreColor,
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
    public void GroupBox_OnPaint_VisualStyles_on_NullE_ThrowsArgumentNullException()
    {
        if (!Application.RenderWithVisualStyles)
        {
            return;
        }

        using SubGroupBox control = new();
        Assert.Throws<ArgumentNullException>(() => control.OnPaint(null));
    }

    [WinFormsFact]
    public void GroupBox_OnPaint_VisualStyles_off_NullE_ThrowsNullReferenceException()
    {
        if (Application.RenderWithVisualStyles)
        {
            return;
        }

        using SubGroupBox control = new();
        Assert.Throws<NullReferenceException>(() => control.OnPaint(null));
    }

    public static IEnumerable<object[]> ProcessMnemonic_TestData()
    {
        yield return new object[] { null, 'a', false };
        yield return new object[] { string.Empty, 'a', false };
        yield return new object[] { "a", 'a', false };
        yield return new object[] { "&a", 'a', true };
    }

    [WinFormsTheory]
    [MemberData(nameof(ProcessMnemonic_TestData))]
    public void GroupBox_ProcessMnemonic_Invoke_ReturnsExpected(string text, char charCode, bool expected)
    {
        using SubGroupBox control = new()
        {
            Text = text
        };
        Assert.Equal(expected, control.ProcessMnemonic(charCode));
    }

    [WinFormsTheory]
    [MemberData(nameof(ProcessMnemonic_TestData))]
    public void GroupBox_ProcessMnemonic_InvokeWithParent_ReturnsExpected(string text, char charCode, bool expected)
    {
        using Control parent = new();
        using SubGroupBox control = new()
        {
            Parent = parent,
            Text = text
        };
        Assert.Equal(expected, control.ProcessMnemonic(charCode));
    }

    public static IEnumerable<object[]> ProcessMnemonic_CantProcess_TestData()
    {
        yield return new object[] { null, 'a' };
        yield return new object[] { string.Empty, 'a' };
        yield return new object[] { "a", 'a' };
        yield return new object[] { "&a", 'a' };
    }

    [WinFormsTheory]
    [MemberData(nameof(ProcessMnemonic_CantProcess_TestData))]
    public void GroupBox_ProcessMnemonic_InvokeNotVisible_ReturnsFalse(string text, char charCode)
    {
        using SubGroupBox control = new()
        {
            Text = text,
            Visible = false
        };
        Assert.False(control.ProcessMnemonic(charCode));
    }

    [WinFormsTheory]
    [MemberData(nameof(ProcessMnemonic_CantProcess_TestData))]
    public void GroupBox_ProcessMnemonic_InvokeNotEnabled_ReturnsFalse(string text, char charCode)
    {
        using SubGroupBox control = new()
        {
            Text = text,
            Enabled = false
        };
        Assert.False(control.ProcessMnemonic(charCode));
    }

    [WinFormsTheory]
    [MemberData(nameof(ProcessMnemonic_CantProcess_TestData))]
    public void GroupBox_ProcessMnemonic_InvokeParentNotVisible_ReturnsFalse(string text, char charCode)
    {
        using Control parent = new()
        {
            Visible = false
        };
        using SubGroupBox control = new()
        {
            Parent = parent,
            Text = text
        };
        Assert.False(control.ProcessMnemonic(charCode));
    }

    [WinFormsTheory]
    [MemberData(nameof(ProcessMnemonic_CantProcess_TestData))]
    public void GroupBox_ProcessMnemonic_InvokeParentNotEnabled_ReturnsFalse(string text, char charCode)
    {
        using Control parent = new()
        {
            Enabled = false
        };
        using SubGroupBox control = new()
        {
            Parent = parent,
            Text = text
        };
        Assert.False(control.ProcessMnemonic(charCode));
    }

    [WinFormsFact]
    public void GroupBox_ToString_Invoke_ReturnsExpected()
    {
        using GroupBox control = new();
        Assert.Equal("System.Windows.Forms.GroupBox, Text: ", control.ToString());
    }

    [WinFormsFact]
    public void GroupBox_ToString_InvokeWithText_ReturnsExpected()
    {
        using GroupBox control = new()
        {
            Text = "CustomText"
        };
        Assert.Equal("System.Windows.Forms.GroupBox, Text: CustomText", control.ToString());
    }

    public static IEnumerable<object[]> WndProc_EraseBkgnd_TestData()
    {
        yield return new object[] { PInvokeCore.WM_ERASEBKGND };
        yield return new object[] { PInvokeCore.WM_PRINTCLIENT };
    }

    [WinFormsTheory]
    [InlineData((int)PInvokeCore.WM_ERASEBKGND)]
    [InlineData((int)PInvokeCore.WM_PRINTCLIENT)]
    public void GroupBox_WndProc_InvokeEraseBkgndNotOwnerDrawWithoutHandle_Success(int msg)
    {
        using SubGroupBox control = new()
        {
            FlatStyle = FlatStyle.System
        };
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        IntPtr hdc = graphics.GetHdc();
        try
        {
            Message m = new()
            {
                Msg = msg,
                WParam = hdc,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(1, m.Result);
            Assert.True(control.IsHandleCreated);
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [WinFormsTheory]
    [InlineData((int)PInvokeCore.WM_ERASEBKGND)]
    [InlineData((int)PInvokeCore.WM_PRINTCLIENT)]
    public void GroupBox_WndProc_InvokeEraseBkgndNotOwnerDrawWithHandle_Success(int msg)
    {
        using SubGroupBox control = new()
        {
            FlatStyle = FlatStyle.System
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        IntPtr hdc = graphics.GetHdc();
        try
        {
            Message m = new()
            {
                Msg = msg,
                WParam = hdc,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(1, m.Result);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [WinFormsTheory]
    [InlineData((int)PInvokeCore.WM_ERASEBKGND)]
    [InlineData((int)PInvokeCore.WM_PRINTCLIENT)]
    public void GroupBox_WndProc_InvokeEraseBkgndNotOwnerDrawZeroWParam_DoesNotThrow(int msg)
    {
        using SubGroupBox control = new()
        {
            FlatStyle = FlatStyle.System
        };
        Message m = new()
        {
            Msg = msg,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(250, m.Result);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(FlatStyle.Flat)]
    [InlineData(FlatStyle.Popup)]
    [InlineData(FlatStyle.Standard)]
    public void GroupBox_WndProc_InvokeEraseBkgndOwnerDrawWithHandleWithWParam_Success(FlatStyle flatStyle)
    {
        using SubGroupBox control = new()
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
        int paintCallCount = 0;
        control.Paint += (sender, e) => paintCallCount++;

        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        IntPtr hdc = graphics.GetHdc();
        try
        {
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_ERASEBKGND,
                WParam = hdc,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(1, m.Result);
            Assert.Equal(0, paintCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [WinFormsTheory]
    [InlineData(FlatStyle.Flat)]
    [InlineData(FlatStyle.Popup)]
    [InlineData(FlatStyle.Standard)]
    public void GroupBox_WndProc_InvokePrintClientOwnerDrawWithHandleWithWParam_Success(FlatStyle flatStyle)
    {
        using SubGroupBox control = new()
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
        int paintCallCount = 0;
        control.Paint += (sender, e) => paintCallCount++;

        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        IntPtr hdc = graphics.GetHdc();
        try
        {
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_PRINTCLIENT,
                WParam = hdc,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(250, m.Result);
            Assert.Equal(1, paintCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [WinFormsTheory]
    [EnumData<FlatStyle>]
    public void GroupBox_WndProc_InvokeMouseHoverWithHandle_Success(FlatStyle flatStyle)
    {
        using SubGroupBox control = new()
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

    public class SubGroupBox : GroupBox
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

        public new AccessibleObject CreateAccessibilityInstance() => base.CreateAccessibilityInstance();

        public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

        public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

        public new bool GetTopLevel() => base.GetTopLevel();

        public new void OnClick(EventArgs e) => base.OnClick(e);

        public new void OnDoubleClick(EventArgs e) => base.OnDoubleClick(e);

        public new void OnFontChanged(EventArgs e) => base.OnFontChanged(e);

        public new void OnKeyDown(KeyEventArgs e) => base.OnKeyDown(e);

        public new void OnKeyPress(KeyPressEventArgs e) => base.OnKeyPress(e);

        public new void OnKeyUp(KeyEventArgs e) => base.OnKeyUp(e);

        public new void OnMouseClick(MouseEventArgs e) => base.OnMouseClick(e);

        public new void OnMouseDoubleClick(MouseEventArgs e) => base.OnMouseDoubleClick(e);

        public new void OnMouseDown(MouseEventArgs e) => base.OnMouseDown(e);

        public new void OnMouseEnter(EventArgs e) => base.OnMouseEnter(e);

        public new void OnMouseLeave(EventArgs e) => base.OnMouseLeave(e);

        public new void OnMouseMove(MouseEventArgs e) => base.OnMouseMove(e);

        public new void OnMouseUp(MouseEventArgs e) => base.OnMouseUp(e);

        public new void OnPaint(PaintEventArgs e) => base.OnPaint(e);

        public new bool ProcessMnemonic(char charCode) => base.ProcessMnemonic(charCode);

        public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);

        public new Size SizeFromClientSize(Size clientSize) => base.SizeFromClientSize(clientSize);

        public new void WndProc(ref Message m) => base.WndProc(ref m);
    }
}
