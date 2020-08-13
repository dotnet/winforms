// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using WinForms.Common.Tests;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    using Point = System.Drawing.Point;
    using Size = System.Drawing.Size;

    public class MenuStripTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void MenuStrip_Ctor_Default()
        {
            using var control = new SubMenuStrip();
            Assert.Null(control.AccessibleDefaultActionDescription);
            Assert.Null(control.AccessibleDescription);
            Assert.Null(control.AccessibleName);
            Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
            Assert.False(control.AllowDrop);
            Assert.False(control.AllowItemReorder);
            Assert.True(control.AllowMerge);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.False(control.AutoScroll);
            Assert.Equal(Size.Empty, control.AutoScrollMargin);
            Assert.Equal(Size.Empty, control.AutoScrollMinSize);
            Assert.Equal(Point.Empty, control.AutoScrollPosition);
            Assert.True(control.AutoSize);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.Null(control.BindingContext);
            Assert.Equal(24, control.Bottom);
            Assert.Equal(new Rectangle(0, 0, 200, 24), control.Bounds);
            Assert.True(control.CanEnableIme);
            Assert.False(control.CanFocus);
            Assert.False(control.CanOverflow);
            Assert.True(control.CanRaiseEvents);
            Assert.False(control.CanSelect);
            Assert.False(control.Capture);
            Assert.False(control.CausesValidation);
            Assert.Equal(new Rectangle(0, 0, 200, 24), control.ClientRectangle);
            Assert.Equal(new Size(200, 24), control.ClientSize);
            Assert.Null(control.Container);
            Assert.False(control.ContainsFocus);
            Assert.Null(control.ContextMenuStrip);
            Assert.Empty(control.Controls);
            Assert.Same(control.Controls, control.Controls);
            Assert.False(control.Created);
            Assert.Same(Cursors.Default, control.Cursor);
            Assert.Same(Cursors.Default, control.DefaultCursor);
            Assert.Equal(DockStyle.Top, control.DefaultDock);
            Assert.Equal(ToolStripDropDownDirection.BelowRight, control.DefaultDropDownDirection);
            Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
            Assert.Equal(new Padding(2, 2, 0, 2), control.DefaultGripMargin);
            Assert.Equal(Padding.Empty, control.DefaultMargin);
            Assert.Equal(Size.Empty, control.DefaultMaximumSize);
            Assert.Equal(Size.Empty, control.DefaultMinimumSize);
            Assert.Equal(new Padding(6, 2, 0, 2), control.DefaultPadding);
            Assert.Equal(new Size(200, 24), control.DefaultSize);
            Assert.False(control.DefaultShowItemToolTips);
            Assert.False(control.DesignMode);
            Assert.Empty(control.DisplayedItems);
            Assert.Same(control.DisplayedItems, control.DisplayedItems);
            Assert.Equal(new Rectangle(6, 2, 194, 20), control.DisplayRectangle);
            Assert.Equal(DockStyle.Top, control.Dock);
            Assert.NotNull(control.DockPadding);
            Assert.Same(control.DockPadding, control.DockPadding);
            Assert.Equal(2, control.DockPadding.Top);
            Assert.Equal(2, control.DockPadding.Bottom);
            Assert.Equal(6, control.DockPadding.Left);
            Assert.Equal(0, control.DockPadding.Right);
            Assert.True(control.DoubleBuffered);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.False(control.Focused);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.Equal(ToolStripGripStyle.Hidden, control.GripStyle);
            Assert.Equal(ToolStripGripDisplayStyle.Vertical, control.GripDisplayStyle);
            Assert.Equal(new Padding(2, 2, 0, 2), control.GripMargin);
            Assert.Equal(Rectangle.Empty, control.GripRectangle);
            Assert.False(control.HasChildren);
            Assert.Equal(24, control.Height);
            Assert.NotNull(control.HorizontalScroll);
            Assert.Same(control.HorizontalScroll, control.HorizontalScroll);
            Assert.False(control.HScroll);
            Assert.Null(control.ImageList);
            Assert.Equal(new Size(16, 16), control.ImageScalingSize);
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.False(control.IsAccessible);
            Assert.False(control.IsCurrentlyDragging);
            Assert.False(control.IsDropDown);
            Assert.False(control.IsMirrored);
            Assert.Empty(control.Items);
            Assert.Same(control.Items, control.Items);
            Assert.NotNull(control.LayoutEngine);
            Assert.Same(control.LayoutEngine, control.LayoutEngine);
            Assert.Null(control.LayoutSettings);
            Assert.Equal(ToolStripLayoutStyle.HorizontalStackWithOverflow, control.LayoutStyle);
            Assert.Equal(0, control.Left);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Equal(Padding.Empty, control.Margin);
            Assert.Equal(Size.Empty, control.MaximumSize);
            Assert.Equal(new Size(194, 20), control.MaxItemSize);
            Assert.Null(control.MdiWindowListItem);
            Assert.Equal(Size.Empty, control.MinimumSize);
            Assert.Equal(Orientation.Horizontal, control.Orientation);
            Assert.NotNull(control.OverflowButton);
            Assert.Same(control.OverflowButton, control.OverflowButton);
            Assert.Same(control, control.OverflowButton.GetCurrentParent());
            Assert.Equal(new Padding(6, 2, 0, 2), control.Padding);
            Assert.Null(control.Parent);
            Assert.True(control.PreferredSize.Width > 0);
            Assert.True(control.PreferredSize.Height > 0);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.False(control.RecreatingHandle);
            Assert.Null(control.Region);
            Assert.NotNull(control.Renderer);
            Assert.Same(control.Renderer, control.Renderer);
            Assert.IsType<ToolStripProfessionalRenderer>(control.Renderer);
            Assert.Equal(ToolStripRenderMode.ManagerRenderMode, control.RenderMode);
            Assert.False(control.ResizeRedraw);
            Assert.Equal(200, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.True(control.ShowFocusCues);
            Assert.False(control.ShowItemToolTips);
            Assert.True(control.ShowKeyboardCues);
            Assert.Null(control.Site);
            Assert.Equal(new Size(200, 24), control.Size);
            Assert.True(control.Stretch);
            Assert.Equal(0, control.TabIndex);
            Assert.False(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(ToolStripTextDirection.Horizontal, control.TextDirection);
            Assert.Equal(0, control.Top);
            Assert.Null(control.TopLevelControl);
            Assert.False(control.UseWaitCursor);
            Assert.NotNull(control.VerticalScroll);
            Assert.Same(control.VerticalScroll, control.VerticalScroll);
            Assert.True(control.Visible);
            Assert.False(control.VScroll);
            Assert.Equal(200, control.Width);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void MenuStrip_CanOverflow_Set_GetReturnsExpected(bool value)
        {
            using var control = new MenuStrip();
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;

            control.CanOverflow = value;
            Assert.Equal(value, control.CanOverflow);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.CanOverflow = value;
            Assert.Equal(value, control.CanOverflow);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.CanOverflow = !value;
            Assert.Equal(!value, control.CanOverflow);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void MenuStrip_CanOverflow_SetWithHandle_GetReturnsExpected(bool value, int expectedLayoutCallCount)
        {
            using var control = new MenuStrip();
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
                Assert.Null(e.AffectedProperty);
                layoutCallCount++;
            };

            control.CanOverflow = value;
            Assert.Equal(value, control.CanOverflow);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedLayoutCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.CanOverflow = value;
            Assert.Equal(value, control.CanOverflow);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedLayoutCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.CanOverflow = !value;
            Assert.Equal(!value, control.CanOverflow);
            Assert.Equal(expectedLayoutCallCount + 1, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedLayoutCallCount + 1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void MenuStrip_DefaultPadding_GetVisibleGrip_ReturnsExpected()
        {
            using var control = new SubMenuStrip
            {
                GripStyle = ToolStripGripStyle.Visible
            };
            Assert.Equal(new Padding(3, 2, 0, 2), control.DefaultPadding);
        }

        [WinFormsTheory]
        [InlineData(ToolStripGripStyle.Hidden, 0)]
        [InlineData(ToolStripGripStyle.Visible, 1)]
        public void MenuStrip_GripStyle_Set_GetReturnsExpected(ToolStripGripStyle value, int expectedLayoutCallCount)
        {
            using var control = new MenuStrip();
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("GripStyle", e.AffectedProperty);
                layoutCallCount++;
            };

            control.GripStyle = value;
            Assert.Equal(value, control.GripStyle);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.GripStyle = value;
            Assert.Equal(value, control.GripStyle);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(ToolStripGripStyle.Hidden, 0)]
        [InlineData(ToolStripGripStyle.Visible, 1)]
        public void MenuStrip_GripStyle_SetWithHandle_GetReturnsExpected(ToolStripGripStyle value, int expectedLayoutCallCount)
        {
            using var control = new MenuStrip();
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
                Assert.Equal("GripStyle", e.AffectedProperty);
                layoutCallCount++;
            };

            control.GripStyle = value;
            Assert.Equal(value, control.GripStyle);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedLayoutCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.GripStyle = value;
            Assert.Equal(value, control.GripStyle);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedLayoutCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> MdiWindowListItem_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ToolStripMenuItem() };
        }

        [WinFormsTheory]
        [MemberData(nameof(MdiWindowListItem_Set_TestData))]
        public void MenuStrip_MdiWindowListItem_Set_GetReturnsExpected(ToolStripMenuItem value)
        {
            using var control = new MenuStrip
            {
                MdiWindowListItem = value
            };
            Assert.Same(value, control.MdiWindowListItem);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.MdiWindowListItem = value;
            Assert.Same(value, control.MdiWindowListItem);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void MenuStrip_ShowItemToolTips_Set_GetReturnsExpected(bool value)
        {
            using var control = new MenuStrip
            {
                ShowItemToolTips = value
            };
            Assert.Equal(value, control.ShowItemToolTips);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.ShowItemToolTips = value;
            Assert.Equal(value, control.ShowItemToolTips);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.ShowItemToolTips = !value;
            Assert.Equal(!value, control.ShowItemToolTips);
            Assert.Equal(!value, control.OverflowButton.DropDown.ShowItemToolTips);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void MenuStrip_Stretch_Set_GetReturnsExpected(bool value)
        {
            using var control = new ToolStrip
            {
                Stretch = value
            };
            Assert.Equal(value, control.Stretch);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Stretch = value;
            Assert.Equal(value, control.Stretch);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.Stretch = value;
            Assert.Equal(value, control.Stretch);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void MenuStrip_Stretch_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var control = new MenuStrip();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Stretch = value;
            Assert.Equal(value, control.Stretch);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Stretch = value;
            Assert.Equal(value, control.Stretch);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.Stretch = value;
            Assert.Equal(value, control.Stretch);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void MenuStrip_CreateAccessibilityInstance_Invoke_ReturnsExpected()
        {
            using var control = new SubMenuStrip();
            ToolStrip.ToolStripAccessibleObject instance = Assert.IsAssignableFrom<ToolStrip.ToolStripAccessibleObject>(control.CreateAccessibilityInstance());
            Assert.NotNull(instance);
            Assert.Same(control, instance.Owner);
            Assert.Equal(AccessibleRole.MenuBar, instance.Role);
            Assert.NotSame(control.CreateAccessibilityInstance(), instance);
            Assert.NotSame(control.AccessibilityObject, instance);
        }

        [WinFormsFact]
        public void MenuStrip_CreateAccessibilityInstance_InvokeWithCustomRole_ReturnsExpected()
        {
            using var control = new SubMenuStrip
            {
                AccessibleRole = AccessibleRole.HelpBalloon
            };
            ToolStrip.ToolStripAccessibleObject instance = Assert.IsAssignableFrom<ToolStrip.ToolStripAccessibleObject>(control.CreateAccessibilityInstance());
            Assert.NotNull(instance);
            Assert.Same(control, instance.Owner);
            Assert.Equal(AccessibleRole.HelpBalloon, instance.Role);
            Assert.NotSame(control.CreateAccessibilityInstance(), instance);
            Assert.NotSame(control.AccessibilityObject, instance);
        }

        public static IEnumerable<object[]> CreateDefaultItem_Button_TestData()
        {
            EventHandler onClick = (sender, e) => { };

            yield return new object[] { null, null, null };
            yield return new object[] { string.Empty, new Bitmap(10, 10), onClick };
            yield return new object[] { "text", new Bitmap(10, 10), onClick };
        }

        [WinFormsTheory]
        [MemberData(nameof(CreateDefaultItem_Button_TestData))]
        public void MenuStrip_CreateDefaultItem_InvokeButton_Success(string text, Image image, EventHandler onClick)
        {
            using var control = new SubMenuStrip();
            ToolStripMenuItem item = Assert.IsType<ToolStripMenuItem>(control.CreateDefaultItem(text, image, onClick));
            Assert.Equal(text, item.Text);
            Assert.Same(image, item.Image);
        }

        public static IEnumerable<object[]> CreateDefaultItem_Separator_TestData()
        {
            EventHandler onClick = (sender, e) => { };

            yield return new object[] { null, null };
            yield return new object[] { new Bitmap(10, 10), onClick };
        }

        [WinFormsTheory]
        [MemberData(nameof(CreateDefaultItem_Separator_TestData))]
        public void MenuStrip_CreateDefaultItem_InvokeSeparator_Success(Image image, EventHandler onClick)
        {
            using var control = new SubMenuStrip();
            ToolStripSeparator separator = Assert.IsType<ToolStripSeparator>(control.CreateDefaultItem("-", image, onClick));
            Assert.Empty(separator.Text);
            Assert.Null(separator.Image);
        }

        [WinFormsTheory]
        [InlineData(null, 1)]
        [InlineData("", 1)]
        [InlineData("text", 1)]
        [InlineData("-", 0)]
        public void MenuStrip_CreateDefaultItem_PerformClick_Success(string text, int expectedCallCount)
        {
            int callCount = 0;
            EventHandler onClick = (sender, e) => callCount++;
            using var control = new SubMenuStrip();
            ToolStripItem item = Assert.IsAssignableFrom<ToolStripItem>(control.CreateDefaultItem(text, null, onClick));
            item.PerformClick();
            Assert.Equal(expectedCallCount, callCount);
        }

        [WinFormsFact]
        public void MenuStrip_GetAutoSizeMode_Invoke_ReturnsExpected()
        {
            using var control = new SubMenuStrip();
            Assert.Equal(AutoSizeMode.GrowAndShrink, control.GetAutoSizeMode());
        }

        [WinFormsTheory]
        [InlineData(0, true)]
        [InlineData(SubMenuStrip.ScrollStateAutoScrolling, false)]
        [InlineData(SubMenuStrip.ScrollStateFullDrag, false)]
        [InlineData(SubMenuStrip.ScrollStateHScrollVisible, false)]
        [InlineData(SubMenuStrip.ScrollStateUserHasScrolled, false)]
        [InlineData(SubMenuStrip.ScrollStateVScrollVisible, false)]
        [InlineData(int.MaxValue, false)]
        [InlineData((-1), false)]
        public void MenuStrip_GetScrollState_Invoke_ReturnsExpected(int bit, bool expected)
        {
            using var control = new SubMenuStrip();
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
        [InlineData(ControlStyles.AllPaintingInWmPaint, true)]
        [InlineData(ControlStyles.CacheText, false)]
        [InlineData(ControlStyles.EnableNotifyMessage, false)]
        [InlineData(ControlStyles.DoubleBuffer, false)]
        [InlineData(ControlStyles.OptimizedDoubleBuffer, true)]
        [InlineData(ControlStyles.UseTextForAccessibility, true)]
        [InlineData((ControlStyles)0, true)]
        [InlineData((ControlStyles)int.MaxValue, false)]
        [InlineData((ControlStyles)(-1), false)]
        public void MenuStrip_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
        {
            using var control = new SubMenuStrip();
            Assert.Equal(expected, control.GetStyle(flag));

            // Call again to test caching.
            Assert.Equal(expected, control.GetStyle(flag));
        }

        [WinFormsFact]
        public void MenuStrip_GetTopLevel_Invoke_ReturnsExpected()
        {
            using var control = new SubMenuStrip();
            Assert.False(control.GetTopLevel());
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void MenuStrip_OnMenuActivate_Invoke_CallsMenuActivate(EventArgs eventArgs)
        {
            using var control = new SubMenuStrip();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.MenuActivate += handler;
            control.OnMenuActivate(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.MenuActivate -= handler;
            control.OnMenuActivate(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void MenuStrip_OnMenuActivate_InvokeWithHandle_CallsMenuActivate(EventArgs eventArgs)
        {
            using var control = new SubMenuStrip();
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
            control.MenuActivate += handler;
            control.OnMenuActivate(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.MenuActivate -= handler;
            control.OnMenuActivate(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void MenuStrip_OnMenuDeactivate_Invoke_CallsMenuDeactivate(EventArgs eventArgs)
        {
            using var control = new SubMenuStrip();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.MenuDeactivate += handler;
            control.OnMenuDeactivate(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.MenuDeactivate -= handler;
            control.OnMenuDeactivate(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void MenuStrip_OnMenuDeactivate_InvokeWithHandle_CallsMenuDeactivate(EventArgs eventArgs)
        {
            using var control = new SubMenuStrip();
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
            control.MenuDeactivate += handler;
            control.OnMenuDeactivate(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.MenuDeactivate -= handler;
            control.OnMenuDeactivate(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(Keys.A)]
        public void MenuStirp_ProcessCmdKey_InvokeWithoutParent_ReturnsFalse(Keys keyData)
        {
            using var control = new SubMenuStrip();
            var m = new Message();
            Assert.False(control.ProcessCmdKey(ref m, keyData));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(Keys.A)]
        [InlineData(Keys.Space)]
        [InlineData(Keys.Control)]
        [InlineData(Keys.Tab)]
        [InlineData(Keys.Control & Keys.Tab)]
        public void MenuStirp_ProcessCmdKey_InvokeWithParent_ReturnsFalse(Keys keyData)
        {
            using var parent = new Control();
            using var control = new SubMenuStrip
            {
                Parent = parent
            };
            var msg = new Message();
            Assert.False(control.ProcessCmdKey(ref msg, keyData));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(Keys.A, true)]
        [InlineData(Keys.A, false)]
        [InlineData(Keys.Space, true)]
        [InlineData(Keys.Space, false)]
        [InlineData(Keys.Control, true)]
        [InlineData(Keys.Control, false)]
        [InlineData(Keys.Tab, true)]
        [InlineData(Keys.Tab, false)]
        [InlineData(Keys.Control & Keys.Tab, true)]
        [InlineData(Keys.Control & Keys.Tab, false)]
        public void MenuStirp_ProcessCmdKey_InvokeWithCustomParent_ReturnsExpected(Keys keyData, bool result)
        {
            using var control = new SubMenuStrip();
            var msg = new Message
            {
                Msg = 1
            };
            int callCount = 0;
            bool action(Message actualMsg, Keys actualKeyData)
            {
                Assert.Equal(1, actualMsg.Msg);
                Assert.Equal(keyData, actualKeyData);
                callCount++;
                return result;
            }
            using var parent = new CustomProcessControl
            {
                ProcessCmdKeyAction = action
            };
            control.Parent = parent;

            Assert.Equal(result, control.ProcessCmdKey(ref msg, keyData));
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void MenuStrip_WndProc_InvokeMouseActivate_Success()
        {
            using var control = new SubMenuStrip();
            var m = new Message
            {
                Msg = (int)User32.WM.MOUSEACTIVATE,
                Result = (IntPtr)250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void MenuStrip_WndProc_InvokeMouseActivateWithHandle_Success()
        {
            using var control = new SubMenuStrip();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            var m = new Message
            {
                Msg = (int)User32.WM.MOUSEACTIVATE,
                Result = (IntPtr)250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void MenuStrip_WndProc_InvokeMouseHoverWithHandle_Success()
        {
            using var control = new SubMenuStrip();
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
            var m = new Message
            {
                Msg = (int)User32.WM.MOUSEHOVER,
                Result = (IntPtr)250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        private class CustomProcessControl : Control
        {
            public Func<Message, Keys, bool> ProcessCmdKeyAction { get; set; }

            protected override bool ProcessCmdKey(ref Message msg, Keys keyData) => ProcessCmdKeyAction(msg, keyData);

            public Func<char, bool> ProcessDialogCharAction { get; set; }

            protected override bool ProcessDialogChar(char charCode) => ProcessDialogCharAction(charCode);
        }

        private class SubMenuStrip : MenuStrip
        {
            public new const int ScrollStateAutoScrolling = MenuStrip.ScrollStateAutoScrolling;

            public new const int ScrollStateHScrollVisible = MenuStrip.ScrollStateHScrollVisible;

            public new const int ScrollStateVScrollVisible = MenuStrip.ScrollStateVScrollVisible;

            public new const int ScrollStateUserHasScrolled = MenuStrip.ScrollStateUserHasScrolled;

            public new const int ScrollStateFullDrag = MenuStrip.ScrollStateFullDrag;

            public SubMenuStrip() : base()
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
