// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    using Point = System.Drawing.Point;
    using Size = System.Drawing.Size;

    public class MdiControlStripTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void MdiControlStrip_Ctor_Default()
        {
            using Form form = new();
            using var mdiControlStrip = new SubMdiControlStrip(form);
            Assert.Null(mdiControlStrip.AccessibleDefaultActionDescription);
            Assert.Null(mdiControlStrip.AccessibleDescription);
            Assert.Null(mdiControlStrip.AccessibleName);
            Assert.Equal(AccessibleRole.Default, mdiControlStrip.AccessibleRole);
            Assert.False(mdiControlStrip.AllowDrop);
            Assert.False(mdiControlStrip.AllowItemReorder);
            Assert.True(mdiControlStrip.AllowMerge);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, mdiControlStrip.Anchor);
            Assert.False(mdiControlStrip.AutoScroll);
            Assert.Equal(Size.Empty, mdiControlStrip.AutoScrollMargin);
            Assert.Equal(Size.Empty, mdiControlStrip.AutoScrollMinSize);
            Assert.Equal(Point.Empty, mdiControlStrip.AutoScrollPosition);
            Assert.True(mdiControlStrip.AutoSize);
            Assert.Equal(Control.DefaultBackColor, mdiControlStrip.BackColor);
            Assert.Null(mdiControlStrip.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, mdiControlStrip.BackgroundImageLayout);
            Assert.Null(mdiControlStrip.BindingContext);
            Assert.Equal(24, mdiControlStrip.Bottom);
            Assert.Equal(new Rectangle(0, 0, 200, 24), mdiControlStrip.Bounds);
            Assert.True(mdiControlStrip.CanEnableIme);
            Assert.False(mdiControlStrip.CanFocus);
            Assert.False(mdiControlStrip.CanOverflow);
            Assert.True(mdiControlStrip.CanRaiseEvents);
            Assert.False(mdiControlStrip.CanSelect);
            Assert.False(mdiControlStrip.Capture);
            Assert.False(mdiControlStrip.CausesValidation);
            Assert.Equal(new Rectangle(0, 0, 200, 24), mdiControlStrip.ClientRectangle);
            Assert.Equal(new Size(200, 24), mdiControlStrip.ClientSize);
            Assert.Null(mdiControlStrip.Container);
            Assert.False(mdiControlStrip.ContainsFocus);
            Assert.Null(mdiControlStrip.ContextMenuStrip);
            Assert.Empty(mdiControlStrip.Controls);
            Assert.Same(mdiControlStrip.Controls, mdiControlStrip.Controls);
            Assert.False(mdiControlStrip.Created);
            Assert.Same(Cursors.Default, mdiControlStrip.Cursor);
            Assert.Same(Cursors.Default, mdiControlStrip.DefaultCursor);
            Assert.Equal(DockStyle.Top, mdiControlStrip.DefaultDock);
            Assert.Equal(ToolStripDropDownDirection.BelowRight, mdiControlStrip.DefaultDropDownDirection);
            Assert.Equal(ImeMode.Inherit, mdiControlStrip.DefaultImeMode);
            Assert.Equal(new Padding(2, 2, 0, 2), mdiControlStrip.DefaultGripMargin);
            Assert.Equal(Padding.Empty, mdiControlStrip.DefaultMargin);
            Assert.Equal(Size.Empty, mdiControlStrip.DefaultMaximumSize);
            Assert.Equal(Size.Empty, mdiControlStrip.DefaultMinimumSize);
            Assert.Equal(new Padding(6, 2, 0, 2), mdiControlStrip.DefaultPadding);
            Assert.Equal(new Size(200, 24), mdiControlStrip.DefaultSize);
            Assert.False(mdiControlStrip.DefaultShowItemToolTips);
            Assert.False(mdiControlStrip.DesignMode);
            Assert.Equal(2, mdiControlStrip.DisplayedItems.Count);
            Assert.Same(mdiControlStrip.DisplayedItems, mdiControlStrip.DisplayedItems);
            Assert.Equal(new Rectangle(6, 2, 194, 20), mdiControlStrip.DisplayRectangle);
            Assert.Equal(DockStyle.Top, mdiControlStrip.Dock);
            Assert.NotNull(mdiControlStrip.DockPadding);
            Assert.Same(mdiControlStrip.DockPadding, mdiControlStrip.DockPadding);
            Assert.Equal(2, mdiControlStrip.DockPadding.Top);
            Assert.Equal(2, mdiControlStrip.DockPadding.Bottom);
            Assert.Equal(6, mdiControlStrip.DockPadding.Left);
            Assert.Equal(0, mdiControlStrip.DockPadding.Right);
            Assert.True(mdiControlStrip.DoubleBuffered);
            Assert.True(mdiControlStrip.Enabled);
            Assert.NotNull(mdiControlStrip.Events);
            Assert.Same(mdiControlStrip.Events, mdiControlStrip.Events);
            Assert.False(mdiControlStrip.Focused);
            Assert.Equal(Control.DefaultFont, mdiControlStrip.Font);
            Assert.Equal(mdiControlStrip.Font.Height, mdiControlStrip.FontHeight);
            Assert.Equal(Control.DefaultForeColor, mdiControlStrip.ForeColor);
            Assert.Equal(ToolStripGripStyle.Hidden, mdiControlStrip.GripStyle);
            Assert.Equal(ToolStripGripDisplayStyle.Vertical, mdiControlStrip.GripDisplayStyle);
            Assert.Equal(new Padding(2, 2, 0, 2), mdiControlStrip.GripMargin);
            Assert.Equal(Rectangle.Empty, mdiControlStrip.GripRectangle);
            Assert.False(mdiControlStrip.HasChildren);
            Assert.Equal(24, mdiControlStrip.Height);
            Assert.NotNull(mdiControlStrip.HorizontalScroll);
            Assert.Same(mdiControlStrip.HorizontalScroll, mdiControlStrip.HorizontalScroll);
            Assert.False(mdiControlStrip.HScroll);
            Assert.Null(mdiControlStrip.ImageList);
            Assert.Equal(new Size(16, 16), mdiControlStrip.ImageScalingSize);
            Assert.Equal(ImeMode.NoControl, mdiControlStrip.ImeMode);
            Assert.Equal(ImeMode.NoControl, mdiControlStrip.ImeModeBase);
            Assert.False(mdiControlStrip.IsAccessible);
            Assert.False(mdiControlStrip.IsCurrentlyDragging);
            Assert.False(mdiControlStrip.IsDropDown);
            Assert.False(mdiControlStrip.IsMirrored);
            Assert.Equal(4, mdiControlStrip.Items.Count);
            Assert.Same(mdiControlStrip.Items, mdiControlStrip.Items);
            Assert.NotNull(mdiControlStrip.LayoutEngine);
            Assert.Same(mdiControlStrip.LayoutEngine, mdiControlStrip.LayoutEngine);
            Assert.Null(mdiControlStrip.LayoutSettings);
            Assert.Equal(ToolStripLayoutStyle.HorizontalStackWithOverflow, mdiControlStrip.LayoutStyle);
            Assert.Equal(0, mdiControlStrip.Left);
            Assert.Equal(Point.Empty, mdiControlStrip.Location);
            Assert.Equal(Padding.Empty, mdiControlStrip.Margin);
            Assert.Equal(Size.Empty, mdiControlStrip.MaximumSize);
            Assert.Equal(new Size(194, 20), mdiControlStrip.MaxItemSize);
            Assert.Null(mdiControlStrip.MdiWindowListItem);
            Assert.Equal(Size.Empty, mdiControlStrip.MinimumSize);
            Assert.Equal(Orientation.Horizontal, mdiControlStrip.Orientation);
            Assert.NotNull(mdiControlStrip.OverflowButton);
            Assert.Same(mdiControlStrip.OverflowButton, mdiControlStrip.OverflowButton);
            Assert.Same(mdiControlStrip, mdiControlStrip.OverflowButton.GetCurrentParent());
            Assert.Equal(new Padding(6, 2, 0, 2), mdiControlStrip.Padding);
            Assert.Null(mdiControlStrip.Parent);
            Assert.True(mdiControlStrip.PreferredSize.Width > 0);
            Assert.True(mdiControlStrip.PreferredSize.Height > 0);
            Assert.Equal("Microsoft\u00AE .NET", mdiControlStrip.ProductName);
            Assert.False(mdiControlStrip.RecreatingHandle);
            Assert.Null(mdiControlStrip.Region);
            Assert.NotNull(mdiControlStrip.Renderer);
            Assert.Same(mdiControlStrip.Renderer, mdiControlStrip.Renderer);
            Assert.IsType<ToolStripProfessionalRenderer>(mdiControlStrip.Renderer);
            Assert.Equal(ToolStripRenderMode.ManagerRenderMode, mdiControlStrip.RenderMode);
            Assert.False(mdiControlStrip.ResizeRedraw);
            Assert.Equal(200, mdiControlStrip.Right);
            Assert.Equal(RightToLeft.No, mdiControlStrip.RightToLeft);
            Assert.True(mdiControlStrip.ShowFocusCues);
            Assert.False(mdiControlStrip.ShowItemToolTips);
            Assert.True(mdiControlStrip.ShowKeyboardCues);
            Assert.Null(mdiControlStrip.Site);
            Assert.Equal(new Size(200, 24), mdiControlStrip.Size);
            Assert.True(mdiControlStrip.Stretch);
            Assert.Equal(0, mdiControlStrip.TabIndex);
            Assert.False(mdiControlStrip.TabStop);
            Assert.Empty(mdiControlStrip.Text);
            Assert.Equal(ToolStripTextDirection.Horizontal, mdiControlStrip.TextDirection);
            Assert.Equal(0, mdiControlStrip.Top);
            Assert.Null(mdiControlStrip.TopLevelControl);
            Assert.False(mdiControlStrip.UseWaitCursor);
            Assert.NotNull(mdiControlStrip.VerticalScroll);
            Assert.Same(mdiControlStrip.VerticalScroll, mdiControlStrip.VerticalScroll);
            Assert.True(mdiControlStrip.Visible);
            Assert.False(mdiControlStrip.VScroll);
            Assert.Equal(200, mdiControlStrip.Width);

            Assert.False(mdiControlStrip.IsHandleCreated);
        }

        [WinFormsFact]
        public void MdiControlStrip_Ctor_VerifyMenuItemsInRightOrder()
        {
            using Form form = new();
            using var mdiControlStrip = new SubMdiControlStrip(form);

            Assert.Equal(4, mdiControlStrip.Items.Count);

            ToolStripMenuItem system = mdiControlStrip.TestAccessor().Dynamic._system;
            ToolStripMenuItem close = mdiControlStrip.TestAccessor().Dynamic._close;
            ToolStripMenuItem minimize = mdiControlStrip.TestAccessor().Dynamic._minimize;
            ToolStripMenuItem restore = mdiControlStrip.TestAccessor().Dynamic._restore;
            Assert.Equal(minimize, mdiControlStrip.Items[0]);
            Assert.Equal(restore, mdiControlStrip.Items[1]);
            Assert.Equal(close, mdiControlStrip.Items[2]);
            Assert.Equal(system, mdiControlStrip.Items[3]);
        }

        [WinFormsFact]
        public void MdiControlStrip_Ctor_VerifyMenuItemsHaveImages()
        {
            using Form form = new();
            using var mdiControlStrip = new SubMdiControlStrip(form);

            ToolStripMenuItem[] items = new ToolStripMenuItem[]
            {
                mdiControlStrip.TestAccessor().Dynamic._system,
                mdiControlStrip.TestAccessor().Dynamic._close,
                mdiControlStrip.TestAccessor().Dynamic._minimize,
                mdiControlStrip.TestAccessor().Dynamic._restore
            };

            foreach (ToolStripMenuItem item in items)
            {
                Assert.NotNull(item.Image);
            }
        }

        [WinFormsFact]
        public void MdiControlStrip_MaximizedChildWindow_RecreatesOnSizeChanged()
        {
            using var mdiParent = new Form() { IsMdiContainer = true, Text = "Parent" };
            using var mdiChild = new Form() { MdiParent = mdiParent, Text = "Child" };
            using var menuStrip = new MenuStrip();

            mdiParent.Controls.Add(menuStrip);
            mdiParent.MainMenuStrip = menuStrip;

            mdiParent.Show();
            mdiChild.Show();
            mdiChild.WindowState = FormWindowState.Maximized;

            MdiControlStrip originalMdiControlStrip = mdiParent.TestAccessor().Dynamic.MdiControlStrip;

            // Force size change with large icon
            IntPtr hicon = new Bitmap(256, 256).GetHicon();
            Icon largeIcon = (Icon)Icon.FromHandle(hicon).Clone();
            Interop.User32.DestroyIcon(hicon);
            mdiChild.Icon = largeIcon;

            MdiControlStrip currentMdiControlStrip = mdiParent.TestAccessor().Dynamic.MdiControlStrip;
            Assert.NotEqual(originalMdiControlStrip, currentMdiControlStrip);
        }

        private class SubMdiControlStrip : MdiControlStrip
        {
            public new const int ScrollStateAutoScrolling = MenuStrip.ScrollStateAutoScrolling;

            public new const int ScrollStateHScrollVisible = MenuStrip.ScrollStateHScrollVisible;

            public new const int ScrollStateVScrollVisible = MenuStrip.ScrollStateVScrollVisible;

            public new const int ScrollStateUserHasScrolled = MenuStrip.ScrollStateUserHasScrolled;

            public new const int ScrollStateFullDrag = MenuStrip.ScrollStateFullDrag;

            public SubMdiControlStrip(IWin32Window target) : base(target)
            {
            }

            public new bool CanEnableIme => base.CanEnableIme;

            public new bool CanRaiseEvents => base.CanRaiseEvents;

            public new CreateParams CreateParams => base.CreateParams;

            public new Cursor DefaultCursor => base.DefaultCursor;

            public new DockStyle DefaultDock => base.DefaultDock;

            public new Padding DefaultGripMargin => base.DefaultGripMargin;

            public new ImeMode DefaultImeMode => base.DefaultImeMode;

            public new Padding DefaultMargin => base.DefaultMargin;

            public new Size DefaultMaximumSize => base.DefaultMaximumSize;

            public new Size DefaultMinimumSize => base.DefaultMinimumSize;

            public new Padding DefaultPadding => base.DefaultPadding;

            public new Size DefaultSize => base.DefaultSize;

            public new bool DefaultShowItemToolTips => base.DefaultShowItemToolTips;

            public new bool DesignMode => base.DesignMode;

            public new ToolStripItemCollection DisplayedItems => base.DisplayedItems;

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

            public new Size MaxItemSize => base.MaxItemSize;

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

            public new AccessibleObject CreateAccessibilityInstance() => base.CreateAccessibilityInstance();

            public new ToolStripItem CreateDefaultItem(string text, Image image, EventHandler onClick) => base.CreateDefaultItem(text, image, onClick);

            public new LayoutSettings CreateLayoutSettings(ToolStripLayoutStyle layoutStyle) => base.CreateLayoutSettings(layoutStyle);

            public new void Dispose(bool disposing) => base.Dispose(disposing);

            public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

            public new bool GetScrollState(int bit) => base.GetScrollState(bit);

            public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

            public new bool GetTopLevel() => base.GetTopLevel();

            public new void OnMenuActivate(EventArgs e) => base.OnMenuActivate(e);

            public new void OnMenuDeactivate(EventArgs e) => base.OnMenuDeactivate(e);

            public new bool ProcessCmdKey(ref Message m, Keys keyData) => base.ProcessCmdKey(ref m, keyData);

            public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);

            public new void WndProc(ref Message m) => base.WndProc(ref m);
        }
    }
}
