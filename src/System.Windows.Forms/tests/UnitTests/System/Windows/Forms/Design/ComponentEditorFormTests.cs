// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Design.Tests;

public class ComponentEditorFormTests
{
    [WinFormsFact]
    public void ComponentEditorComponentEditorForm_Ctor_Default()
    {
        using Component component = new();
        using SubComponentEditorForm control = new(component, Array.Empty<Type>());
        Assert.NotNull(control.AcceptButton);
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
        Assert.Equal(control.Height, control.Bottom);
        Assert.Equal(new Rectangle(0, 0, control.Width, control.Height), control.Bounds);
        Assert.NotNull(control.CancelButton);
        Assert.False(control.CanEnableIme);
        Assert.False(control.CanFocus);
        Assert.True(control.CanRaiseEvents);
        Assert.False(control.CanSelect);
        Assert.False(control.Capture);
        Assert.True(control.CausesValidation);
        Assert.Equal(new Rectangle(0, 0, control.ClientSize.Width, control.ClientSize.Height), control.ClientRectangle);
        Assert.Equal(new Size(control.ClientSize.Width, control.ClientSize.Height), control.ClientSize);
        Assert.Null(control.Container);
        Assert.False(control.ContainsFocus);
        Assert.Null(control.ContextMenuStrip);
        Assert.True(control.ControlBox);
        Assert.NotEmpty(control.Controls);
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
        Assert.Equal(new Rectangle(-SystemInformation.WorkingArea.X, -SystemInformation.WorkingArea.Y, control.Width, control.Height), control.DesktopBounds);
        Assert.Equal(new Point(-SystemInformation.WorkingArea.X, -SystemInformation.WorkingArea.Y), control.DesktopLocation);
        Assert.Equal(DialogResult.None, control.DialogResult);
        Assert.Equal(new Rectangle(0, 0, control.ClientSize.Width, control.ClientSize.Height), control.DisplayRectangle);
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
        Assert.Equal(FormBorderStyle.FixedDialog, control.FormBorderStyle);
        Assert.True(control.HasChildren);
        Assert.True(control.Height > 0);
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
        Assert.False(control.MaximizeBox);
        Assert.Equal(Size.Empty, control.MaximumSize);
        Assert.False(control.MinimizeBox);
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
        Assert.Equal(new Rectangle(-1, -1, control.Width, control.Height), control.RestoreBounds);
        Assert.Equal(control.Width, control.Right);
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.False(control.RightToLeftLayout);
        Assert.True(control.ShowFocusCues);
        Assert.True(control.ShowIcon);
        Assert.False(control.ShowInTaskbar);
        Assert.True(control.ShowKeyboardCues);
        Assert.False(control.ShowWithoutActivation);
        Assert.Null(control.Site);
        Assert.Equal(new Size(control.Width, control.Height), control.Size);
        Assert.Equal(SizeGripStyle.Auto, control.SizeGripStyle);
        Assert.Equal(FormStartPosition.CenterParent, control.StartPosition);
        Assert.Equal(0, control.TabIndex);
        Assert.True(control.TabStop);
        Assert.Equal("Properties", control.Text);
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
        Assert.True(control.Width > 0);
        Assert.Equal(FormWindowState.Normal, control.WindowState);

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComponentEditorComponentEditorForm_CreateParams_GetDefault_ReturnsExpected()
    {
        using Component component = new();
        using SubComponentEditorForm control = new(component, Array.Empty<Type>());
        CreateParams createParams = control.CreateParams;
        Assert.Equal("Properties", createParams.Caption);
        Assert.Null(createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0x10001, createParams.ExStyle);
        Assert.Equal(control.Height, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(0x2C80000, createParams.Style);
        Assert.Equal(control.Width, createParams.Width);
        Assert.Equal(int.MinValue, createParams.X);
        Assert.Equal(int.MinValue, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComponentEditorComponentEditorForm_GetAutoSizeMode_Invoke_ReturnsExpected()
    {
        using Component component = new();
        using SubComponentEditorForm control = new(component, Array.Empty<Type>());
        Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
    }

    [WinFormsTheory]
    [InlineData(0, true)]
    [InlineData(SubComponentEditorForm.ScrollStateAutoScrolling, false)]
    [InlineData(SubComponentEditorForm.ScrollStateFullDrag, false)]
    [InlineData(SubComponentEditorForm.ScrollStateHScrollVisible, false)]
    [InlineData(SubComponentEditorForm.ScrollStateUserHasScrolled, false)]
    [InlineData(SubComponentEditorForm.ScrollStateVScrollVisible, false)]
    [InlineData(int.MaxValue, false)]
    [InlineData((-1), false)]
    public void ComponentEditorComponentEditorForm_GetScrollState_Invoke_ReturnsExpected(int bit, bool expected)
    {
        using Component component = new();
        using SubComponentEditorForm control = new(component, Array.Empty<Type>());
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
    public void ComponentEditorComponentEditorForm_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
    {
        using Component component = new();
        using SubComponentEditorForm control = new(component, Array.Empty<Type>());
        Assert.Equal(expected, control.GetStyle(flag));

        // Call again to test caching.
        Assert.Equal(expected, control.GetStyle(flag));
    }

    [WinFormsFact]
    public void ComponentEditorComponentEditorForm_GetTopLevel_Invoke_ReturnsExpected()
    {
        using Component component = new();
        using SubComponentEditorForm control = new(component, Array.Empty<Type>());
        Assert.True(control.GetTopLevel());
    }

    /*
            [WinFormsTheory]
            [NewAndDefaultData<EventArgs>]
            public void ComponentEditorForm_OnHandleCreated_Invoke_CallsHandleCreated(EventArgs eventArgs)
            {
                using Component component = new();
                using SubComponentEditorForm control = new(component, new Type[0]);
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
    */

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ComponentEditorForm_OnHandleCreated_InvokeWithHandle_CallsHandleCreated(EventArgs eventArgs)
    {
        using Component component = new();
        using SubComponentEditorForm control = new(component, Array.Empty<Type>());
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
    public void ComponentEditorForm_OnHandleDestroyed_Invoke_CallsHandleDestroyed(EventArgs eventArgs)
    {
        using Component component = new();
        using SubComponentEditorForm control = new(component, Array.Empty<Type>());
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
    public void ComponentEditorForm_OnHandleDestroyed_InvokeWithHandle_CallsHandleDestroyed(EventArgs eventArgs)
    {
        using Component component = new();
        using SubComponentEditorForm control = new(component, Array.Empty<Type>());
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

    public class SubComponentEditorForm : ComponentEditorForm
    {
        public SubComponentEditorForm(object component, Type[] pageTypes) : base(component, pageTypes)
        {
        }

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
