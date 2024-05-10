// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Layout;
using Moq;
using System.Windows.Forms.TestUtilities;

namespace System.Windows.Forms.Tests;

public class PanelTests
{
    [WinFormsFact]
    public void Panel_Ctor_Default()
    {
        using SubPanel control = new();
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
        Assert.Equal(AutoSizeMode.GrowOnly, control.AutoSizeMode);
        Assert.Equal(Control.DefaultBackColor, control.BackColor);
        Assert.Null(control.BackgroundImage);
        Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
        Assert.Null(control.BindingContext);
        Assert.Equal(BorderStyle.None, control.BorderStyle);
        Assert.Equal(100, control.Bottom);
        Assert.Equal(new Rectangle(0, 0, 200, 100), control.Bounds);
        Assert.True(control.CanEnableIme);
        Assert.False(control.CanFocus);
        Assert.True(control.CanRaiseEvents);
        Assert.False(control.CanSelect);
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
        Assert.Equal(Cursors.Default, control.Cursor);
        Assert.Equal(Cursors.Default, control.DefaultCursor);
        Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
        Assert.Equal(new Padding(3), control.DefaultMargin);
        Assert.Equal(Size.Empty, control.DefaultMaximumSize);
        Assert.Equal(Size.Empty, control.DefaultMinimumSize);
        Assert.Equal(Padding.Empty, control.DefaultPadding);
        Assert.Equal(new Size(200, 100), control.DefaultSize);
        Assert.False(control.DesignMode);
        Assert.Equal(new Rectangle(0, 0, 200, 100), control.DisplayRectangle);
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
        Assert.Equal(100, control.Height);
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
        Assert.Equal(200, control.Right);
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.True(control.ShowFocusCues);
        Assert.True(control.ShowKeyboardCues);
        Assert.Equal(new Size(200, 100), control.Size);
        Assert.Equal(0, control.TabIndex);
        Assert.False(control.TabStop);
        Assert.Empty(control.Text);
        Assert.Equal(0, control.Top);
        Assert.Null(control.TopLevelControl);
        Assert.False(control.UseWaitCursor);
        Assert.True(control.Visible);
        Assert.NotNull(control.VerticalScroll);
        Assert.Same(control.VerticalScroll, control.VerticalScroll);
        Assert.False(control.VScroll);
        Assert.Equal(200, control.Width);

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Panel_CreateParams_GetDefault_ReturnsExpected()
    {
        using SubPanel control = new();
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
    [InlineData(BorderStyle.None, 0x56000000, 0x10000)]
    [InlineData(BorderStyle.Fixed3D, 0x56000000, 0x10200)]
    [InlineData(BorderStyle.FixedSingle, 0x56800000, 0x10000)]
    public void Panel_CreateParams_GetBorderStyle_ReturnsExpected(BorderStyle borderStyle, int expectedStyle, int expectedExStyle)
    {
        using SubPanel control = new()
        {
            BorderStyle = borderStyle
        };
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Null(createParams.ClassName);
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

    [WinFormsTheory]
    [BoolData]
    public void Panel_AutoSize_Set_GetReturnsExpected(bool value)
    {
        using Panel control = new();
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
    public void Panel_AutoSize_SetWithHandler_CallsAutoSizeChanged()
    {
        using Panel control = new()
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
    public void Panel_AutoSizeMode_Set_GetReturnsExpected(AutoSizeMode value)
    {
        using SubPanel control = new();
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
    public void Panel_AutoSizeMode_SetWithParent_GetReturnsExpected(AutoSizeMode value, int expectedLayoutCallCount)
    {
        using Control parent = new();
        using SubPanel control = new()
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
    public void Panel_AutoSizeMode_SetWithCustomLayoutEngineParent_GetReturnsExpected(AutoSizeMode value, int expectedLayoutCallCount)
    {
        using CustomLayoutEngineControl parent = new();
        using SubPanel control = new()
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
            _layoutEngine = new Control().LayoutEngine;
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
    public void Panel_AutoSizeMode_SetWithHandle_GetReturnsExpected(AutoSizeMode value)
    {
        using SubPanel control = new();
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
    public void Panel_AutoSizeMode_SetWithParentWithHandle_GetReturnsExpected(AutoSizeMode value, int expectedLayoutCallCount)
    {
        using Control parent = new();
        using SubPanel control = new()
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
    public void Panel_AutoSizeMode_SetWithCustomLayoutEngineParentWithHandle_GetReturnsExpected(AutoSizeMode value, int expectedLayoutCallCount)
    {
        using CustomLayoutEngineControl parent = new();
        using SubPanel control = new()
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
    public void Panel_AutoSizeMode_SetInvalid_ThrowsInvalidEnumArgumentException(AutoSizeMode value)
    {
        using Panel control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.AutoSizeMode = value);
    }

    [WinFormsTheory]
    [EnumData<BorderStyle>]
    public void Panel_BorderStyle_Set_GetReturnsExpected(BorderStyle value)
    {
        using Panel control = new()
        {
            BorderStyle = value
        };
        Assert.Equal(value, control.BorderStyle);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BorderStyle = value;
        Assert.Equal(value, control.BorderStyle);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(BorderStyle.Fixed3D, 1)]
    [InlineData(BorderStyle.FixedSingle, 1)]
    [InlineData(BorderStyle.None, 0)]
    public void Panel_BorderStyle_SetWithHandle_GetReturnsExpected(BorderStyle value, int expectedInvalidatedCallCount)
    {
        using Panel control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.BorderStyle = value;
        Assert.Equal(value, control.BorderStyle);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.BorderStyle = value;
        Assert.Equal(value, control.BorderStyle);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<BorderStyle>]
    public void Panel_BorderStyle_SetInvalid_ThrowsInvalidEnumArgumentException(BorderStyle value)
    {
        using Panel control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.BorderStyle = value);
    }

    [WinFormsFact]
    public void Panel_PreferredSize_GetWithChildrenSimple_ReturnsExpected()
    {
        using Panel control = new();
        using Control child = new()
        {
            Size = new Size(16, 20)
        };
        control.Controls.Add(child);
        Assert.Equal(new Size(19, 23), control.PreferredSize);
    }

    [WinFormsFact]
    public void Panel_PreferredSize_GetWithChildrenAdvanced_ReturnsExpected()
    {
        using BorderedPanel control = new()
        {
            Padding = new Padding(1, 2, 3, 4)
        };
        using Control child = new()
        {
            Size = new Size(16, 20)
        };
        control.Controls.Add(child);
        Assert.Equal(new Size(26, 31), control.PreferredSize);
    }

    private class BorderedPanel : Panel
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

        public new Size SizeFromClientSize(Size clientSize) => base.SizeFromClientSize(clientSize);
    }

    [WinFormsTheory]
    [BoolData]
    public void Panel_TabStop_Set_GetReturnsExpected(bool value)
    {
        using Panel control = new()
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
    public void Panel_TabStop_SetWithHandle_GetReturnsExpected(bool value)
    {
        using Panel control = new();
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
    public void Panel_TabStop_SetWithHandler_CallsTabStopChanged()
    {
        using Panel control = new()
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
    public void Panel_Text_Set_GetReturnsExpected(string value, string expected)
    {
        using Panel control = new()
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
    public void Panel_Text_SetWithHandle_GetReturnsExpected(string value, string expected)
    {
        using Panel control = new();
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
    public void Panel_Text_SetWithHandler_CallsTextChanged()
    {
        using Panel control = new();
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

    [WinFormsFact]
    public void Panel_GetAutoSizeMode_Invoke_ReturnsExpected()
    {
        using SubPanel control = new();
        Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
    }

    [WinFormsTheory]
    [InlineData(0, true)]
    [InlineData(SubPanel.ScrollStateAutoScrolling, false)]
    [InlineData(SubPanel.ScrollStateFullDrag, false)]
    [InlineData(SubPanel.ScrollStateHScrollVisible, false)]
    [InlineData(SubPanel.ScrollStateUserHasScrolled, false)]
    [InlineData(SubPanel.ScrollStateVScrollVisible, false)]
    [InlineData(int.MaxValue, false)]
    [InlineData((-1), false)]
    public void Panel_GetScrollState_Invoke_ReturnsExpected(int bit, bool expected)
    {
        using SubPanel control = new();
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
    [InlineData(ControlStyles.Selectable, false)]
    [InlineData(ControlStyles.UserMouse, false)]
    [InlineData(ControlStyles.SupportsTransparentBackColor, true)]
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
    public void Panel_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
    {
        using SubPanel control = new();
        Assert.Equal(expected, control.GetStyle(flag));

        // Call again to test caching.
        Assert.Equal(expected, control.GetStyle(flag));
    }

    [WinFormsFact]
    public void Panel_GetTopLevel_Invoke_ReturnsExpected()
    {
        using SubPanel control = new();
        Assert.False(control.GetTopLevel());
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetKeyEventArgsTheoryData))]
    public void Panel_OnKeyDown_Invoke_CallsKeyDown(KeyEventArgs eventArgs)
    {
        using SubPanel control = new();
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
    public void Panel_OnKeyPress_Invoke_CallsKeyPress(KeyPressEventArgs eventArgs)
    {
        using SubPanel control = new();
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
    public void Panel_OnKeyUp_Invoke_CallsKeyUp(KeyEventArgs eventArgs)
    {
        using SubPanel control = new();
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
    public void Panel_OnResize_Invoke_CallsResize(EventArgs eventArgs)
    {
        using SubPanel control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            layoutCallCount++;
        };

        // Call with handler.
        control.Resize += handler;
        control.OnResize(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(1, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.Resize -= handler;
        control.OnResize(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(2, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> OnResize_WithHandle_TestData()
    {
        yield return new object[] { true, null, 1 };
        yield return new object[] { true, new EventArgs(), 1 };
        yield return new object[] { false, null, 0 };
        yield return new object[] { false, new EventArgs(), 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnResize_WithHandle_TestData))]
    public void Panel_OnResize_InvokeWithHandle_CallsResize(bool resizeRedraw, EventArgs eventArgs, int expectedInvalidatedCallCount)
    {
        using SubPanel control = new();
        control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            layoutCallCount++;
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        // Call with handler.
        control.Resize += handler;
        control.OnResize(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(1, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.Resize -= handler;
        control.OnResize(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(2, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> OnResize_DesignMode_TestData()
    {
        yield return new object[] { true, BorderStyle.None, null, 2 };
        yield return new object[] { true, BorderStyle.None, new EventArgs(), 2 };
        yield return new object[] { true, BorderStyle.Fixed3D, null, 1 };
        yield return new object[] { true, BorderStyle.Fixed3D, new EventArgs(), 1 };
        yield return new object[] { true, BorderStyle.FixedSingle, null, 1 };
        yield return new object[] { true, BorderStyle.FixedSingle, new EventArgs(), 1 };

        yield return new object[] { false, BorderStyle.None, null, 1 };
        yield return new object[] { false, BorderStyle.None, new EventArgs(), 1 };
        yield return new object[] { false, BorderStyle.Fixed3D, null, 0 };
        yield return new object[] { false, BorderStyle.Fixed3D, new EventArgs(), 0 };
        yield return new object[] { false, BorderStyle.FixedSingle, null, 0 };
        yield return new object[] { false, BorderStyle.FixedSingle, new EventArgs(), 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnResize_DesignMode_TestData))]
    public void Panel_OnResize_InvokeWithDesignMode_CallsResize(bool resizeRedraw, BorderStyle borderStyle, EventArgs eventArgs, int expectedInvalidatedCallCount)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        using SubPanel control = new()
        {
            Site = mockSite.Object,
            BorderStyle = borderStyle
        };
        control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            layoutCallCount++;
        };
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        // Call with handler.
        control.Resize += handler;
        control.OnResize(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(1, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.Resize -= handler;
        control.OnResize(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(2, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Panel_ToString_Invoke_ReturnsExpected()
    {
        Panel panel = new() { BorderStyle = BorderStyle.Fixed3D };
        Assert.Equal("System.Windows.Forms.Panel, BorderStyle: System.Windows.Forms.BorderStyle.Fixed3D", panel.ToString());
    }

    private class SubPanel : Panel
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

        public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

        public new bool GetScrollState(int bit) => base.GetScrollState(bit);

        public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

        public new bool GetTopLevel() => base.GetTopLevel();

        public new void OnKeyDown(KeyEventArgs e) => base.OnKeyDown(e);

        public new void OnKeyPress(KeyPressEventArgs e) => base.OnKeyPress(e);

        public new void OnKeyUp(KeyEventArgs e) => base.OnKeyUp(e);

        public new void OnResize(EventArgs e) => base.OnResize(e);

        public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);
    }
}
