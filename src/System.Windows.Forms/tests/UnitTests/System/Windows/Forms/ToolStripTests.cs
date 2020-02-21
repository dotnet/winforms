// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Moq;
using WinForms.Common.Tests;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    using Point = System.Drawing.Point;
    using Size = System.Drawing.Size;

    public class ToolStripTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStrip_Ctor_Default()
        {
            using var control = new SubToolStrip();
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
            Assert.Equal(25, control.Bottom);
            Assert.Equal(new Rectangle(0, 0, 100, 25), control.Bounds);
            Assert.True(control.CanEnableIme);
            Assert.False(control.CanFocus);
            Assert.True(control.CanOverflow);
            Assert.True(control.CanRaiseEvents);
            Assert.False(control.CanSelect);
            Assert.False(control.Capture);
            Assert.False(control.CausesValidation);
            Assert.Equal(new Rectangle(0, 0, 100, 25), control.ClientRectangle);
            Assert.Equal(new Size(100, 25), control.ClientSize);
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
            Assert.Equal(new Padding(2, 2, 2, 2), control.DefaultGripMargin);
            Assert.Equal(Padding.Empty, control.DefaultMargin);
            Assert.Equal(Size.Empty, control.DefaultMaximumSize);
            Assert.Equal(Size.Empty, control.DefaultMinimumSize);
            Assert.Equal(new Padding(0, 0, 1, 0), control.DefaultPadding);
            Assert.Equal(new Size(100, 25), control.DefaultSize);
            Assert.True(control.DefaultShowItemToolTips);
            Assert.False(control.DesignMode);
            Assert.Single(control.DisplayedItems);
            Assert.Same(control.DisplayedItems, control.DisplayedItems);
            Assert.Equal(new Rectangle(7, 0, 92, 25), control.DisplayRectangle);
            Assert.Equal(DockStyle.Top, control.Dock);
            Assert.NotNull(control.DockPadding);
            Assert.Same(control.DockPadding, control.DockPadding);
            Assert.Equal(0, control.DockPadding.Top);
            Assert.Equal(0, control.DockPadding.Bottom);
            Assert.Equal(0, control.DockPadding.Left);
            Assert.Equal(1, control.DockPadding.Right);
            Assert.True(control.DoubleBuffered);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.False(control.Focused);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.Equal(ToolStripGripStyle.Visible, control.GripStyle);
            Assert.Equal(ToolStripGripDisplayStyle.Vertical, control.GripDisplayStyle);
            Assert.Equal(new Padding(2, 2, 2, 2), control.GripMargin);
            Assert.Equal(2, control.GripRectangle.X);
            Assert.Equal(0, control.GripRectangle.Y);
            Assert.True(control.GripRectangle.Width > 0);
            Assert.Equal(25, control.GripRectangle.Height);
            Assert.False(control.HasChildren);
            Assert.Equal(25, control.Height);
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
            Assert.Equal(new Size(92, 25), control.MaxItemSize);
            Assert.Equal(Size.Empty, control.MinimumSize);
            Assert.Equal(Orientation.Horizontal, control.Orientation);
            Assert.NotNull(control.OverflowButton);
            Assert.Same(control.OverflowButton, control.OverflowButton);
            Assert.Same(control, control.OverflowButton.GetCurrentParent());
            Assert.Equal(new Padding(0, 0, 1, 0), control.Padding);
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
            Assert.Equal(100, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.True(control.ShowFocusCues);
            Assert.True(control.ShowItemToolTips);
            Assert.True(control.ShowKeyboardCues);
            Assert.Null(control.Site);
            Assert.Equal(new Size(100, 25), control.Size);
            Assert.False(control.Stretch);
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
            Assert.Equal(100, control.Width);

            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Ctor_ToolStripItemArray_TestData()
        {
            yield return new object[] { new ToolStripItem[0] };
            yield return new object[] { new ToolStripItem[] { new SubToolStripItem() } };
        }

        [WinFormsTheory]
        [MemberData(nameof(Ctor_ToolStripItemArray_TestData))]
        public void ToolStrip_Ctor_ToolStripItemArray(ToolStripItem[] items)
        {
            using var control = new SubToolStrip(items);
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
            Assert.Equal(25, control.Bottom);
            Assert.Equal(new Rectangle(0, 0, 100, 25), control.Bounds);
            Assert.True(control.CanEnableIme);
            Assert.False(control.CanFocus);
            Assert.False(control.CanSelect);
            Assert.False(control.Capture);
            Assert.True(control.CanOverflow);
            Assert.True(control.CanRaiseEvents);
            Assert.False(control.CausesValidation);
            Assert.Equal(new Rectangle(0, 0, 100, 25), control.ClientRectangle);
            Assert.Equal(new Size(100, 25), control.ClientSize);
            Assert.False(control.Created);
            Assert.Null(control.Container);
            Assert.False(control.ContainsFocus);
            Assert.Empty(control.Controls);
            Assert.Same(control.Controls, control.Controls);
            Assert.Same(Cursors.Default, control.Cursor);
            Assert.Same(Cursors.Default, control.DefaultCursor);
            Assert.Equal(DockStyle.Top, control.DefaultDock);
            Assert.Equal(ToolStripDropDownDirection.BelowRight, control.DefaultDropDownDirection);
            Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
            Assert.Equal(new Padding(2, 2, 2, 2), control.DefaultGripMargin);
            Assert.Equal(Padding.Empty, control.DefaultMargin);
            Assert.Equal(Size.Empty, control.DefaultMaximumSize);
            Assert.Equal(Size.Empty, control.DefaultMinimumSize);
            Assert.Equal(new Padding(0, 0, 1, 0), control.DefaultPadding);
            Assert.Equal(new Size(100, 25), control.DefaultSize);
            Assert.True(control.DefaultShowItemToolTips);
            Assert.False(control.DesignMode);
            Assert.Equal(new Rectangle(7, 0, 92, 25), control.DisplayRectangle);
            Assert.Equal(DockStyle.Top, control.Dock);
            Assert.NotNull(control.DockPadding);
            Assert.Same(control.DockPadding, control.DockPadding);
            Assert.Equal(0, control.DockPadding.Top);
            Assert.Equal(0, control.DockPadding.Bottom);
            Assert.Equal(0, control.DockPadding.Left);
            Assert.Equal(1, control.DockPadding.Right);
            Assert.True(control.DoubleBuffered);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.False(control.Focused);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.Equal(ToolStripGripStyle.Visible, control.GripStyle);
            Assert.Equal(ToolStripGripDisplayStyle.Vertical, control.GripDisplayStyle);
            Assert.Equal(new Padding(2, 2, 2, 2), control.GripMargin);
            Assert.Equal(2, control.GripRectangle.X);
            Assert.Equal(0, control.GripRectangle.Y);
            Assert.True(control.GripRectangle.Width > 0);
            Assert.Equal(25, control.GripRectangle.Height);
            Assert.False(control.HasChildren);
            Assert.Equal(25, control.Height);
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
            Assert.NotSame(items, control.Items);
            Assert.Same(control.Items, control.Items);
            Assert.Equal(items, control.Items.Cast<ToolStripItem>());
            Assert.NotNull(control.LayoutEngine);
            Assert.Same(control.LayoutEngine, control.LayoutEngine);
            Assert.Null(control.LayoutSettings);
            Assert.Equal(ToolStripLayoutStyle.HorizontalStackWithOverflow, control.LayoutStyle);
            Assert.Equal(0, control.Left);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Equal(Padding.Empty, control.Margin);
            Assert.Equal(Size.Empty, control.MaximumSize);
            Assert.Equal(new Size(92, 25), control.MaxItemSize);
            Assert.Equal(Size.Empty, control.MinimumSize);
            Assert.Equal(Orientation.Horizontal, control.Orientation);
            Assert.NotNull(control.OverflowButton);
            Assert.Same(control.OverflowButton, control.OverflowButton);
            Assert.Same(control, control.OverflowButton.GetCurrentParent());
            Assert.Equal(new Padding(0, 0, 1, 0), control.Padding);
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
            Assert.Equal(100, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.True(control.ShowFocusCues);
            Assert.True(control.ShowItemToolTips);
            Assert.True(control.ShowKeyboardCues);
            Assert.Null(control.Site);
            Assert.Equal(new Size(100, 25), control.Size);
            Assert.False(control.Stretch);
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
            Assert.Equal(100, control.Width);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStrip_Ctor_NullItems_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("toolStripItems", () => new ToolStrip(null));
        }

        [WinFormsFact]
        public void ToolStrip_Ctor_NullValueInItems_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("value", () => new ToolStrip(new ToolStripItem[] { null }));
        }

        [WinFormsFact]
        public void ToolStrip_CreateParams_GetDefault_ReturnsExpected()
        {
            using var control = new SubToolStrip();
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Null(createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x10000, createParams.ExStyle);
            Assert.Equal(25, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x56000000, createParams.Style);
            Assert.Equal(100, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStrip_AllowDrop_Set_GetReturnsExpected(bool value)
        {
            using var control = new ToolStrip
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
            control.AllowDrop = value;
            Assert.Equal(value, control.AllowDrop);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void ToolStrip_AllowDrop_SetWithChildren_GetReturnsExpected(bool childAllowDrop, bool value)
        {
            using var control = new ToolStrip();
            using var item = new SubToolStripItem
            {
                AllowDrop = childAllowDrop
            };
            control.Items.Add(item);

            control.AllowDrop = value;
            Assert.Equal(value, control.AllowDrop);
            Assert.Equal(childAllowDrop, item.AllowDrop);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.AllowDrop = value;
            Assert.Equal(value, control.AllowDrop);
            Assert.Equal(childAllowDrop, item.AllowDrop);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.AllowDrop = value;
            Assert.Equal(value, control.AllowDrop);
            Assert.Equal(childAllowDrop, item.AllowDrop);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStrip_AllowDrop_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var control = new ToolStrip();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.AllowDrop = value;
            Assert.Equal(value, control.AllowDrop);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.AllowDrop = value;
            Assert.Equal(value, control.AllowDrop);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.AllowDrop = value;
            Assert.Equal(value, control.AllowDrop);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStrip_AllowDrop_SetWithHandleAlreadyRegistered_GetReturnsExpected(bool value)
        {
            using var control = new ToolStrip();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            var dropTarget = new CustomDropTarget();
            Assert.Equal(ApartmentState.STA, Application.OleRequired());
            Assert.Equal(HRESULT.S_OK, Ole32.RegisterDragDrop(control.Handle, dropTarget));

            try
            {
                control.AllowDrop = value;
                Assert.Equal(value, control.AllowDrop);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);

                // Set same.
                control.AllowDrop = value;
                Assert.Equal(value, control.AllowDrop);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);

                // Set different.
                control.AllowDrop = value;
                Assert.Equal(value, control.AllowDrop);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
            }
            finally
            {
                Ole32.RevokeDragDrop(control.Handle);
            }
        }

        private class CustomDropTarget : Ole32.IDropTarget
        {
            public HRESULT DragEnter([MarshalAs(UnmanagedType.Interface)] object pDataObj, uint grfKeyState, Point pt, ref uint pdwEffect)
            {
                throw new NotImplementedException();
            }

            public HRESULT DragOver(uint grfKeyState, Point pt, ref uint pdwEffect)
            {
                throw new NotImplementedException();
            }

            public HRESULT DragLeave()
            {
                throw new NotImplementedException();
            }

            public HRESULT Drop([MarshalAs(UnmanagedType.Interface)] object pDataObj, uint grfKeyState, Point pt, ref uint pdwEffect)
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public void ToolStrip_AllowDrop_SetWithHandleNonSTAThread_ThrowsInvalidOperationException()
        {
            using var control = new ToolStrip();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Throws<InvalidOperationException>(() => control.AllowDrop = true);
            Assert.False(control.AllowDrop);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Can set to false.
            control.AllowDrop = false;
            Assert.False(control.AllowDrop);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void ToolStrip_AllowDrop_SetWithChildrenWithHandle_GetReturnsExpected(bool childAllowDrop, bool value)
        {
            using var control = new ToolStrip();
            using var item = new SubToolStripItem
            {
                AllowDrop = childAllowDrop
            };
            control.Items.Add(item);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.AllowDrop = value;
            Assert.Equal(value, control.AllowDrop);
            Assert.Equal(childAllowDrop, item.AllowDrop);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.AllowDrop = value;
            Assert.Equal(value, control.AllowDrop);
            Assert.Equal(childAllowDrop, item.AllowDrop);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.AllowDrop = value;
            Assert.Equal(value, control.AllowDrop);
            Assert.Equal(childAllowDrop, item.AllowDrop);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ToolStrip_AllowDrop_SetAllowItemReorder_ThrowsArgumentException()
        {
            using var control = new ToolStrip
            {
                AllowItemReorder = true
            };
            Assert.Throws<ArgumentException>(null, () => control.AllowDrop = true);
            Assert.False(control.AllowDrop);

            control.AllowDrop = false;
            Assert.False(control.AllowDrop);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStrip_AllowItemReorder_Set_GetReturnsExpected(bool value)
        {
            using var control = new ToolStrip
            {
                AllowItemReorder = value
            };
            Assert.Equal(value, control.AllowItemReorder);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.AllowItemReorder = value;
            Assert.Equal(value, control.AllowItemReorder);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.AllowItemReorder = value;
            Assert.Equal(value, control.AllowItemReorder);
            Assert.False(control.IsHandleCreated);
        }

        [Fact]
        public void ToolStrip_AllowItemReorder_SetWithHandleNonSTAThread_ThrowsInvalidOperationException()
        {
            using var control = new ToolStrip();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Throws<InvalidOperationException>(() => control.AllowItemReorder = true);
            Assert.True(control.AllowItemReorder);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Can set to false.
            control.AllowItemReorder = false;
            Assert.False(control.AllowItemReorder);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStrip_AllowItemReorder_SetWithHandleSTA_GetReturnsExpected(bool value)
        {
            using var control = new ToolStrip();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.AllowItemReorder = value;
            Assert.Equal(value, control.AllowItemReorder);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.AllowItemReorder = value;
            Assert.Equal(value, control.AllowItemReorder);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.AllowItemReorder = value;
            Assert.Equal(value, control.AllowItemReorder);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ToolStrip_AllowItemReorder_SetAllowDrop_ThrowsArgumentException()
        {
            using var control = new ToolStrip
            {
                AllowDrop = true
            };
            Assert.Throws<ArgumentException>(null, () => control.AllowItemReorder = true);
            Assert.False(control.AllowItemReorder);

            control.AllowItemReorder = false;
            Assert.False(control.AllowItemReorder);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStrip_AllowMerge_Set_GetReturnsExpected(bool value)
        {
            using var control = new ToolStrip
            {
                AllowMerge = value
            };
            Assert.Equal(value, control.AllowMerge);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.AllowMerge = value;
            Assert.Equal(value, control.AllowMerge);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.AllowMerge = value;
            Assert.Equal(value, control.AllowMerge);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStrip_AllowMerge_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var control = new ToolStrip();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.AllowMerge = value;
            Assert.Equal(value, control.AllowMerge);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.AllowMerge = value;
            Assert.Equal(value, control.AllowMerge);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.AllowMerge = value;
            Assert.Equal(value, control.AllowMerge);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> Anchor_Set_TestData()
        {
            yield return new object[] { AnchorStyles.Top, AnchorStyles.Top, DockStyle.None };
            yield return new object[] { AnchorStyles.Top | AnchorStyles.Bottom, AnchorStyles.Top | AnchorStyles.Bottom, DockStyle.None };
            yield return new object[] { AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, DockStyle.None };
            yield return new object[] { AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right, DockStyle.None };
            yield return new object[] { AnchorStyles.Top | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Left, DockStyle.Top };
            yield return new object[] { AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, DockStyle.None };
            yield return new object[] { AnchorStyles.Top | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Right, DockStyle.None };
            yield return new object[] { AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, DockStyle.None };

            yield return new object[] { AnchorStyles.Bottom, AnchorStyles.Bottom, DockStyle.None };
            yield return new object[] { AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Bottom | AnchorStyles.Left, DockStyle.None };
            yield return new object[] { AnchorStyles.Bottom | AnchorStyles.Right, AnchorStyles.Bottom | AnchorStyles.Right, DockStyle.None };
            yield return new object[] { AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, DockStyle.None };

            yield return new object[] { AnchorStyles.Left, AnchorStyles.Left, DockStyle.None };
            yield return new object[] { AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Left | AnchorStyles.Right, DockStyle.None };

            yield return new object[] { AnchorStyles.Right, AnchorStyles.Right, DockStyle.None };

            yield return new object[] { AnchorStyles.None, AnchorStyles.None, DockStyle.None };
            yield return new object[] { (AnchorStyles)(-1), AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, DockStyle.None };
            yield return new object[] { (AnchorStyles)int.MaxValue, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, DockStyle.None };
        }

        [WinFormsTheory]
        [MemberData(nameof(Anchor_Set_TestData))]
        public void ToolStrip_Anchor_Set_GetReturnsExpected(AnchorStyles value, AnchorStyles expected, DockStyle expectedDock)
        {
            using var control = new ToolStrip();
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Anchor", e.AffectedProperty);
                layoutCallCount++;
            };

            control.Anchor = value;
            Assert.Equal(expected, control.Anchor);
            Assert.Equal(expectedDock, control.Dock);
            Assert.Equal(1, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Anchor = value;
            Assert.Equal(expected, control.Anchor);
            Assert.Equal(expectedDock, control.Dock);
            Assert.Equal(2, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStrip_AutoScroll_Set_ThrowsNotSupportedException(bool value)
        {
            using var control = new ToolStrip();
            Assert.Throws<NotSupportedException>(() => control.AutoScroll = value);
            Assert.False(control.AutoScroll);
        }

        public static IEnumerable<object[]> AutoScrollMargin_Set_TestData()
        {
            yield return new object[] { new Size(0, 0), 0 };
            yield return new object[] { new Size(1, 0), 0 };
            yield return new object[] { new Size(0, 1), 0 };
            yield return new object[] { new Size(1, 2), 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(AutoScrollMargin_Set_TestData))]
        public void ToolStrip_AutoScrollMargin_Set_GetReturnsExpected(Size value, int expectedLayoutCallCount)
        {
            using var control = new ToolStrip();
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Null(e.AffectedControl);
                Assert.Null(e.AffectedProperty);
                layoutCallCount++;
            };

            control.AutoScrollMargin = value;
            Assert.Equal(value, control.AutoScrollMargin);
            Assert.False(control.AutoScroll);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.AutoScrollMargin = value;
            Assert.Equal(value, control.AutoScrollMargin);
            Assert.False(control.AutoScroll);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStrip_AutoScrollMinSize_Set_ThrowsNotSupportedException()
        {
            using var control = new ToolStrip();
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;

            Assert.Throws<NotSupportedException>(() => control.AutoScrollMinSize = new Size(1, 2));
            Assert.Equal(new Size(1, 2), control.AutoScrollMinSize);
            Assert.False(control.AutoScroll);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.AutoScrollMinSize = new Size(1, 2);
            Assert.Equal(new Size(1, 2), control.AutoScrollMinSize);
            Assert.False(control.AutoScroll);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetPointTheoryData))]
        public void ToolStrip_AutoScrollPosition_Set_GetReturnsExpected(Point value)
        {
            using var control = new ToolStrip
            {
                AutoScrollPosition = value
            };
            Assert.Equal(Point.Empty, control.AutoScrollPosition);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.AutoScrollPosition = value;
            Assert.Equal(Point.Empty, control.AutoScrollPosition);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStrip_AutoSize_Set_GetReturnsExpected(bool value)
        {
            using var control = new ToolStrip
            {
                AutoSize = value
            };
            Assert.Equal(value, control.AutoSize);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.AutoSize = value;
            Assert.Equal(value, control.AutoSize);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.AutoSize = !value;
            Assert.Equal(!value, control.AutoSize);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStrip_AutoSize_SetInToolStripPanel_GetReturnsExpected(bool value)
        {
            using var parent = new ToolStripPanel();
            using var control = new ToolStrip
            {
                Parent = parent,
                AutoSize = value
            };
            Assert.Equal(value, control.AutoSize);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.AutoSize = value;
            Assert.Equal(value, control.AutoSize);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.AutoSize = !value;
            Assert.Equal(!value, control.AutoSize);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStrip_AutoSize_SetWithHandler_CallsAutoSizeChanged()
        {
            using var control = new ToolStrip
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

        public static IEnumerable<object[]> BackColor_Set_TestData()
        {
            yield return new object[] { Color.Empty, Control.DefaultBackColor };
            yield return new object[] { Color.Red, Color.Red };
        }

        [WinFormsTheory]
        [MemberData(nameof(BackColor_Set_TestData))]
        public void ToolStrip_BackColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new ToolStrip
            {
                BackColor = value
            };
            Assert.Equal(expected, control.BackColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
            Assert.False(control.IsHandleCreated);
        }
        [WinFormsFact]
        public void ToolStrip_BindingContext_GetWithParent_ReturnsExpected()
        {
            var bindingContext = new BindingContext();
            using var parent = new Control
            {
                BindingContext = bindingContext
            };
            using var control = new ToolStrip
            {
                Parent = parent
            };
            Assert.Same(bindingContext, control.BindingContext);
        }

        [WinFormsFact]
        public void ToolStrip_BindingContext_GetWithParentCantAccessProperties_ReturnsExpected()
        {
            var bindingContext = new BindingContext();
            using var parent = new SubAxHost("00000000-0000-0000-0000-000000000000")
            {
                BindingContext = bindingContext
            };
            using var control = new ToolStrip
            {
                Parent = parent
            };
            Assert.Null(control.BindingContext);
        }

        public static IEnumerable<object[]> BindingContext_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new BindingContext() };
        }

        [WinFormsTheory]
        [MemberData(nameof(BindingContext_Set_TestData))]
        public void ToolStrip_BindingContext_Set_GetReturnsExpected(BindingContext value)
        {
            using var control = new ToolStrip
            {
                BindingContext = value
            };
            Assert.Same(value, control.BindingContext);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.BindingContext = value;
            Assert.Same(value, control.BindingContext);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(BindingContext_Set_TestData))]
        public void ToolStrip_BindingContext_SetWithNonNullBindingContext_GetReturnsExpected(BindingContext value)
        {
            using var control = new ToolStrip
            {
                BindingContext = new BindingContext()
            };

            control.BindingContext = value;
            Assert.Same(value, control.BindingContext);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.BindingContext = value;
            Assert.Same(value, control.BindingContext);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStrip_BindingContext_SetWithHandler_CallsBindingContextChanged()
        {
            using var control = new ToolStrip();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.BindingContextChanged += handler;

            // Set different.
            var context1 = new BindingContext();
            control.BindingContext = context1;
            Assert.Same(context1, control.BindingContext);
            Assert.Equal(1, callCount);

            // Set same.
            control.BindingContext = context1;
            Assert.Same(context1, control.BindingContext);
            Assert.Equal(1, callCount);

            // Set different.
            var context2 = new BindingContext();
            control.BindingContext = context2;
            Assert.Same(context2, control.BindingContext);
            Assert.Equal(2, callCount);

            // Set null.
            control.BindingContext = null;
            Assert.Null(control.BindingContext);
            Assert.Equal(3, callCount);

            // Remove handler.
            control.BindingContextChanged -= handler;
            control.BindingContext = context1;
            Assert.Same(context1, control.BindingContext);
            Assert.Equal(3, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStrip_CanOverflow_Set_GetReturnsExpected(bool value)
        {
            using var control = new ToolStrip();
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
        [InlineData(true, 0)]
        [InlineData(false, 1)]
        public void ToolStrip_CanOverflow_SetWithHandle_GetReturnsExpected(bool value, int expectedLayoutCallCount)
        {
            using var control = new ToolStrip();
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

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStrip_CausesValidation_Set_GetReturnsExpected(bool value)
        {
            using var control = new ToolStrip
            {
                CausesValidation = value
            };
            Assert.Equal(value, control.CausesValidation);
            Assert.False(control.IsHandleCreated);

            // Set same
            control.CausesValidation = value;
            Assert.Equal(value, control.CausesValidation);
            Assert.False(control.IsHandleCreated);

            // Set different
            control.CausesValidation = !value;
            Assert.Equal(!value, control.CausesValidation);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStrip_CausesValidation_SetWithHandler_CallsCausesValidationChanged()
        {
            using var control = new ToolStrip
            {
                CausesValidation = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.CausesValidationChanged += handler;

            // Set different.
            control.CausesValidation = false;
            Assert.False(control.CausesValidation);
            Assert.Equal(1, callCount);

            // Set same.
            control.CausesValidation = false;
            Assert.False(control.CausesValidation);
            Assert.Equal(1, callCount);

            // Set different.
            control.CausesValidation = true;
            Assert.True(control.CausesValidation);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.CausesValidationChanged -= handler;
            control.CausesValidation = false;
            Assert.False(control.CausesValidation);
            Assert.Equal(2, callCount);
        }
        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetCursorTheoryData))]
        public void ToolStrip_Cursor_Set_GetReturnsExpected(Cursor value)
        {
            using var control = new ToolStrip
            {
                Cursor = value
            };
            Assert.Same(value ?? Cursors.Default, control.Cursor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Cursor = value;
            Assert.Same(value ?? Cursors.Default, control.Cursor);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStrip_Cursor_SetWithHandler_CallsCursorChanged()
        {
            using var control = new ToolStrip();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.CursorChanged += handler;

            // Set different.
            using var cursor1 = new Cursor((IntPtr)1);
            control.Cursor = cursor1;
            Assert.Same(cursor1, control.Cursor);
            Assert.Equal(1, callCount);

            // Set same.
            control.Cursor = cursor1;
            Assert.Same(cursor1, control.Cursor);
            Assert.Equal(1, callCount);

            // Set different.
            using var cursor2 = new Cursor((IntPtr)2);
            control.Cursor = cursor2;
            Assert.Same(cursor2, control.Cursor);
            Assert.Equal(2, callCount);

            // Set null.
            control.Cursor = null;
            Assert.Same(Cursors.Default, control.Cursor);
            Assert.Equal(3, callCount);

            // Remove handler.
            control.CursorChanged -= handler;
            control.Cursor = cursor1;
            Assert.Same(cursor1, control.Cursor);
            Assert.Equal(3, callCount);
        }

        [WinFormsFact]
        public void ToolStrip_Font_Get_ReturnsSame()
        {
            using var control = new ToolStrip();
            Assert.NotSame(Control.DefaultFont, control.Font);
            Assert.Same(control.Font, control.Font);
        }

        [WinFormsFact]
        public void ToolStrip_Font_GetWithParent_ReturnsExpected()
        {
            using var font1 = new Font("Arial", 8.25f);
            using var font2 = new Font("Arial", 8.5f);
            using var parent = new Control
            {
                Font = font1
            };
            using var control = new ToolStrip
            {
                Parent = parent
            };
            Assert.NotSame(font1, control.Font);
            Assert.Same(control.Font, control.Font);

            // Set custom.
            control.Font = font2;
            Assert.Same(font2, control.Font);
        }

        [WinFormsFact]
        public void ToolStrip_Font_GetWithParentCantAccessProperties_ReturnsExpected()
        {
            using var font1 = new Font("Arial", 8.25f);
            using var font2 = new Font("Arial", 8.5f);
            using var parent = new SubAxHost("00000000-0000-0000-0000-000000000000")
            {
                Font = font1
            };
            using var control = new ToolStrip
            {
                Parent = parent
            };
            Assert.NotSame(Control.DefaultFont, control.Font);
            Assert.Same(control.Font, control.Font);

            // Set custom.
            control.Font = font2;
            Assert.Same(font2, control.Font);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetFontTheoryData))]
        public void ToolStrip_Font_Set_GetReturnsExpected(Font value)
        {
            using var control = new SubToolStrip
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

        public static IEnumerable<object[]> DefaultDropDownDirection_Get_TestData()
        {
            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.None, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.None, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.None, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Left, RightToLeft.Yes, ToolStripDropDownDirection.Right };
            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Left, RightToLeft.No, ToolStripDropDownDirection.Right };
            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Left, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Right, RightToLeft.Yes, ToolStripDropDownDirection.Right };
            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Right, RightToLeft.No, ToolStripDropDownDirection.Left };
            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Right, RightToLeft.Inherit, ToolStripDropDownDirection.Left };
            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Top, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Top, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Top, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Bottom, RightToLeft.Yes, ToolStripDropDownDirection.AboveLeft };
            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Bottom, RightToLeft.No, ToolStripDropDownDirection.AboveRight };
            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Bottom, RightToLeft.Inherit, ToolStripDropDownDirection.AboveRight };

            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.None, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.None, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.None, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Right, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Right, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Right, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Left, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Left, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Left, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Top, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Top, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Top, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Bottom, RightToLeft.Yes, ToolStripDropDownDirection.AboveLeft };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Bottom, RightToLeft.No, ToolStripDropDownDirection.AboveRight };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Bottom, RightToLeft.Inherit, ToolStripDropDownDirection.AboveRight };

            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.None, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.None, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.None, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Right, RightToLeft.Yes, ToolStripDropDownDirection.Right };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Right, RightToLeft.No, ToolStripDropDownDirection.Left };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Right, RightToLeft.Inherit, ToolStripDropDownDirection.Left };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, RightToLeft.Yes, ToolStripDropDownDirection.Right };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, RightToLeft.No, ToolStripDropDownDirection.Right };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Bottom, RightToLeft.Yes, ToolStripDropDownDirection.AboveLeft };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Bottom, RightToLeft.No, ToolStripDropDownDirection.AboveRight };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Bottom, RightToLeft.Inherit, ToolStripDropDownDirection.AboveRight };

            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.None, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.None, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.None, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Right, RightToLeft.Yes, ToolStripDropDownDirection.Right };
            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Right, RightToLeft.No, ToolStripDropDownDirection.Left };
            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Right, RightToLeft.Inherit, ToolStripDropDownDirection.Left };
            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Left, RightToLeft.Yes, ToolStripDropDownDirection.Right };
            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Left, RightToLeft.No, ToolStripDropDownDirection.Right };
            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Left, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Top, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Top, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Top, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Bottom, RightToLeft.Yes, ToolStripDropDownDirection.AboveLeft };
            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Bottom, RightToLeft.No, ToolStripDropDownDirection.AboveRight };
            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Bottom, RightToLeft.Inherit, ToolStripDropDownDirection.AboveRight };

            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.None, RightToLeft.Yes, ToolStripDropDownDirection.Right };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.None, RightToLeft.No, ToolStripDropDownDirection.Right };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.None, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Right, RightToLeft.Yes, ToolStripDropDownDirection.Right };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Right, RightToLeft.No, ToolStripDropDownDirection.Left };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Right, RightToLeft.Inherit, ToolStripDropDownDirection.Left };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Left, RightToLeft.Yes, ToolStripDropDownDirection.Right };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Left, RightToLeft.No, ToolStripDropDownDirection.Right };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Left, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Top, RightToLeft.Yes, ToolStripDropDownDirection.Right };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Top, RightToLeft.No, ToolStripDropDownDirection.Right };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Top, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Bottom, RightToLeft.Yes, ToolStripDropDownDirection.Right };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Bottom, RightToLeft.No, ToolStripDropDownDirection.Right };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Bottom, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
        }

        [WinFormsTheory]
        [MemberData(nameof(DefaultDropDownDirection_Get_TestData))]
        public void ToolStrip_DefaultDropDownDirection_Get_ReturnsExpected(ToolStripLayoutStyle layoutStyle, DockStyle dock, RightToLeft rightToLeft, ToolStripDropDownDirection expected)
        {
            using var control = new ToolStrip
            {
                LayoutStyle = layoutStyle,
                Dock = dock,
                RightToLeft = rightToLeft
            };
            Assert.Equal(expected, control.DefaultDropDownDirection);
        }

        [WinFormsTheory]
        [MemberData(nameof(DefaultDropDownDirection_Get_TestData))]
        public void ToolStrip_DefaultDropDownDirection_GetDesignMode_ReturnsExpected(ToolStripLayoutStyle layoutStyle, DockStyle dock, RightToLeft rightToLeft, ToolStripDropDownDirection expected)
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(true);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.Name)
                .Returns("Name");
            using var control = new ToolStrip
            {
                Site = mockSite.Object,
                LayoutStyle = layoutStyle,
                Dock = dock,
                RightToLeft = rightToLeft
            };
            Assert.Equal(expected, control.DefaultDropDownDirection);
        }

        public static IEnumerable<object[]> DefaultDropDownDirection_GetWithParent_TestData()
        {
            foreach (DockStyle parentDock in new object[] { DockStyle.None, DockStyle.Left, DockStyle.Right, DockStyle.Top, DockStyle.Bottom })
            {
                yield return new object[] { parentDock, ToolStripLayoutStyle.Flow, DockStyle.None, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { parentDock, ToolStripLayoutStyle.Flow, DockStyle.None, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { parentDock, ToolStripLayoutStyle.Flow, DockStyle.None, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { parentDock, ToolStripLayoutStyle.Flow, DockStyle.Left, RightToLeft.Yes, ToolStripDropDownDirection.Right };
                yield return new object[] { parentDock, ToolStripLayoutStyle.Flow, DockStyle.Left, RightToLeft.No, ToolStripDropDownDirection.Right };
                yield return new object[] { parentDock, ToolStripLayoutStyle.Flow, DockStyle.Left, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
                yield return new object[] { parentDock, ToolStripLayoutStyle.Flow, DockStyle.Right, RightToLeft.Yes, ToolStripDropDownDirection.Right };
                yield return new object[] { parentDock, ToolStripLayoutStyle.Flow, DockStyle.Right, RightToLeft.No, ToolStripDropDownDirection.Left };
                yield return new object[] { parentDock, ToolStripLayoutStyle.Flow, DockStyle.Right, RightToLeft.Inherit, ToolStripDropDownDirection.Left };
                yield return new object[] { parentDock, ToolStripLayoutStyle.Flow, DockStyle.Top, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { parentDock, ToolStripLayoutStyle.Flow, DockStyle.Top, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { parentDock, ToolStripLayoutStyle.Flow, DockStyle.Top, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { parentDock, ToolStripLayoutStyle.Flow, DockStyle.Bottom, RightToLeft.Yes, ToolStripDropDownDirection.AboveLeft };
                yield return new object[] { parentDock, ToolStripLayoutStyle.Flow, DockStyle.Bottom, RightToLeft.No, ToolStripDropDownDirection.AboveRight };
                yield return new object[] { parentDock, ToolStripLayoutStyle.Flow, DockStyle.Bottom, RightToLeft.Inherit, ToolStripDropDownDirection.AboveRight };

                yield return new object[] { parentDock, ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.None, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { parentDock, ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.None, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { parentDock, ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.None, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { parentDock, ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Right, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { parentDock, ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Right, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { parentDock, ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Right, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { parentDock, ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Left, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { parentDock, ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Left, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { parentDock, ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Left, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { parentDock, ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Top, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { parentDock, ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Top, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { parentDock, ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Top, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { parentDock, ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Bottom, RightToLeft.Yes, ToolStripDropDownDirection.AboveLeft };
                yield return new object[] { parentDock, ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Bottom, RightToLeft.No, ToolStripDropDownDirection.AboveRight };
                yield return new object[] { parentDock, ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Bottom, RightToLeft.Inherit, ToolStripDropDownDirection.AboveRight };

                yield return new object[] { parentDock, ToolStripLayoutStyle.StackWithOverflow, DockStyle.None, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { parentDock, ToolStripLayoutStyle.StackWithOverflow, DockStyle.None, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { parentDock, ToolStripLayoutStyle.StackWithOverflow, DockStyle.None, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { parentDock, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Right, RightToLeft.Yes, ToolStripDropDownDirection.Right };
                yield return new object[] { parentDock, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Right, RightToLeft.No, ToolStripDropDownDirection.Left };
                yield return new object[] { parentDock, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Right, RightToLeft.Inherit, ToolStripDropDownDirection.Left };
                yield return new object[] { parentDock, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, RightToLeft.Yes, ToolStripDropDownDirection.Right };
                yield return new object[] { parentDock, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, RightToLeft.No, ToolStripDropDownDirection.Right };
                yield return new object[] { parentDock, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
                yield return new object[] { parentDock, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { parentDock, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { parentDock, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { parentDock, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Bottom, RightToLeft.Yes, ToolStripDropDownDirection.AboveLeft };
                yield return new object[] { parentDock, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Bottom, RightToLeft.No, ToolStripDropDownDirection.AboveRight };
                yield return new object[] { parentDock, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Bottom, RightToLeft.Inherit, ToolStripDropDownDirection.AboveRight };

                yield return new object[] { parentDock, ToolStripLayoutStyle.Table, DockStyle.None, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { parentDock, ToolStripLayoutStyle.Table, DockStyle.None, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { parentDock, ToolStripLayoutStyle.Table, DockStyle.None, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { parentDock, ToolStripLayoutStyle.Table, DockStyle.Right, RightToLeft.Yes, ToolStripDropDownDirection.Right };
                yield return new object[] { parentDock, ToolStripLayoutStyle.Table, DockStyle.Right, RightToLeft.No, ToolStripDropDownDirection.Left };
                yield return new object[] { parentDock, ToolStripLayoutStyle.Table, DockStyle.Right, RightToLeft.Inherit, ToolStripDropDownDirection.Left };
                yield return new object[] { parentDock, ToolStripLayoutStyle.Table, DockStyle.Left, RightToLeft.Yes, ToolStripDropDownDirection.Right };
                yield return new object[] { parentDock, ToolStripLayoutStyle.Table, DockStyle.Left, RightToLeft.No, ToolStripDropDownDirection.Right };
                yield return new object[] { parentDock, ToolStripLayoutStyle.Table, DockStyle.Left, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
                yield return new object[] { parentDock, ToolStripLayoutStyle.Table, DockStyle.Top, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { parentDock, ToolStripLayoutStyle.Table, DockStyle.Top, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { parentDock, ToolStripLayoutStyle.Table, DockStyle.Top, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { parentDock, ToolStripLayoutStyle.Table, DockStyle.Bottom, RightToLeft.Yes, ToolStripDropDownDirection.AboveLeft };
                yield return new object[] { parentDock, ToolStripLayoutStyle.Table, DockStyle.Bottom, RightToLeft.No, ToolStripDropDownDirection.AboveRight };
                yield return new object[] { parentDock, ToolStripLayoutStyle.Table, DockStyle.Bottom, RightToLeft.Inherit, ToolStripDropDownDirection.AboveRight };

                yield return new object[] { parentDock, ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.None, RightToLeft.Yes, ToolStripDropDownDirection.Right };
                yield return new object[] { parentDock, ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.None, RightToLeft.No, ToolStripDropDownDirection.Right };
                yield return new object[] { parentDock, ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.None, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
                yield return new object[] { parentDock, ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Right, RightToLeft.Yes, ToolStripDropDownDirection.Right };
                yield return new object[] { parentDock, ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Right, RightToLeft.No, ToolStripDropDownDirection.Left };
                yield return new object[] { parentDock, ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Right, RightToLeft.Inherit, ToolStripDropDownDirection.Left };
                yield return new object[] { parentDock, ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Left, RightToLeft.Yes, ToolStripDropDownDirection.Right };
                yield return new object[] { parentDock, ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Left, RightToLeft.No, ToolStripDropDownDirection.Right };
                yield return new object[] { parentDock, ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Left, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
                yield return new object[] { parentDock, ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Top, RightToLeft.Yes, ToolStripDropDownDirection.Right };
                yield return new object[] { parentDock, ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Top, RightToLeft.No, ToolStripDropDownDirection.Right };
                yield return new object[] { parentDock, ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Top, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
                yield return new object[] { parentDock, ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Bottom, RightToLeft.Yes, ToolStripDropDownDirection.Right };
                yield return new object[] { parentDock, ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Bottom, RightToLeft.No, ToolStripDropDownDirection.Right };
                yield return new object[] { parentDock, ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Bottom, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(DefaultDropDownDirection_GetWithParent_TestData))]
        public void ToolStrip_DefaultDropDownDirection_GetWithParent_ReturnsExpected(DockStyle parentDock, ToolStripLayoutStyle layoutStyle, DockStyle dock, RightToLeft rightToLeft, ToolStripDropDownDirection expected)
        {
            using var parent = new Control
            {
                Dock = parentDock
            };
            using var control = new ToolStrip
            {
                Parent = parent,
                LayoutStyle = layoutStyle,
                Dock = dock,
                RightToLeft = rightToLeft
            };
            Assert.Equal(expected, control.DefaultDropDownDirection);
        }

        [WinFormsTheory]
        [MemberData(nameof(DefaultDropDownDirection_GetWithParent_TestData))]
        public void ToolStrip_DefaultDropDownDirection_GetDesignModeWithParent_ReturnsExpected(DockStyle parentDock, ToolStripLayoutStyle layoutStyle, DockStyle dock, RightToLeft rightToLeft, ToolStripDropDownDirection expected)
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(true);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.Name)
                .Returns("Name");
            using var parent = new Control
            {
                Dock = parentDock
            };
            using var control = new ToolStrip
            {
                Parent = parent,
                Site = mockSite.Object,
                LayoutStyle = layoutStyle,
                Dock = dock,
                RightToLeft = rightToLeft
            };
            Assert.Equal(expected, control.DefaultDropDownDirection);
        }

        public static IEnumerable<object[]> DefaultDropDownDirection_GetWithToolStripPanelParent_TestData()
        {
            foreach (DockStyle childDock in new object[] { DockStyle.None, DockStyle.Left, DockStyle.Right, DockStyle.Top, DockStyle.Bottom })
            {
                yield return new object[] { DockStyle.None, ToolStripLayoutStyle.Flow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { DockStyle.None, ToolStripLayoutStyle.Flow, childDock, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.None, ToolStripLayoutStyle.Flow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.Flow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.Flow, childDock, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.Flow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.Flow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.Flow, childDock, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.Flow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.Flow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.Flow, childDock, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.Flow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.Flow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.AboveLeft };
                yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.Flow, childDock, RightToLeft.No, ToolStripDropDownDirection.AboveRight };
                yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.Flow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.AboveRight };

                yield return new object[] { DockStyle.None, ToolStripLayoutStyle.HorizontalStackWithOverflow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { DockStyle.None, ToolStripLayoutStyle.HorizontalStackWithOverflow, childDock, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.None, ToolStripLayoutStyle.HorizontalStackWithOverflow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.HorizontalStackWithOverflow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.HorizontalStackWithOverflow, childDock, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.HorizontalStackWithOverflow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.HorizontalStackWithOverflow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.HorizontalStackWithOverflow, childDock, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.HorizontalStackWithOverflow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.HorizontalStackWithOverflow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.HorizontalStackWithOverflow, childDock, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.HorizontalStackWithOverflow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.HorizontalStackWithOverflow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.AboveLeft };
                yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.HorizontalStackWithOverflow, childDock, RightToLeft.No, ToolStripDropDownDirection.AboveRight };
                yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.HorizontalStackWithOverflow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.AboveRight };

                yield return new object[] { DockStyle.None, ToolStripLayoutStyle.StackWithOverflow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { DockStyle.None, ToolStripLayoutStyle.StackWithOverflow, childDock, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.None, ToolStripLayoutStyle.StackWithOverflow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.StackWithOverflow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.Left };
                yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.StackWithOverflow, childDock, RightToLeft.No, ToolStripDropDownDirection.Left };
                yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.StackWithOverflow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.Left };
                yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.StackWithOverflow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.Right };
                yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.StackWithOverflow, childDock, RightToLeft.No, ToolStripDropDownDirection.Right };
                yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.StackWithOverflow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
                yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.StackWithOverflow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.StackWithOverflow, childDock, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.StackWithOverflow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.StackWithOverflow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.AboveLeft };
                yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.StackWithOverflow, childDock, RightToLeft.No, ToolStripDropDownDirection.AboveRight };
                yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.StackWithOverflow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.AboveRight };

                yield return new object[] { DockStyle.None, ToolStripLayoutStyle.Table, childDock, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { DockStyle.None, ToolStripLayoutStyle.Table, childDock, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.None, ToolStripLayoutStyle.Table, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.Table, childDock, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.Table, childDock, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.Table, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.Table, childDock, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.Table, childDock, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.Table, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.Table, childDock, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.Table, childDock, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.Table, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.Table, childDock, RightToLeft.Yes, ToolStripDropDownDirection.AboveLeft };
                yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.Table, childDock, RightToLeft.No, ToolStripDropDownDirection.AboveRight };
                yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.Table, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.AboveRight };

                yield return new object[] { DockStyle.None, ToolStripLayoutStyle.VerticalStackWithOverflow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.Right };
                yield return new object[] { DockStyle.None, ToolStripLayoutStyle.VerticalStackWithOverflow, childDock, RightToLeft.No, ToolStripDropDownDirection.Right };
                yield return new object[] { DockStyle.None, ToolStripLayoutStyle.VerticalStackWithOverflow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
                yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.VerticalStackWithOverflow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.Left };
                yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.VerticalStackWithOverflow, childDock, RightToLeft.No, ToolStripDropDownDirection.Left };
                yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.VerticalStackWithOverflow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.Left };
                yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.VerticalStackWithOverflow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.Right };
                yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.VerticalStackWithOverflow, childDock, RightToLeft.No, ToolStripDropDownDirection.Right };
                yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.VerticalStackWithOverflow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
                yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.VerticalStackWithOverflow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.Right };
                yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.VerticalStackWithOverflow, childDock, RightToLeft.No, ToolStripDropDownDirection.Right };
                yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.VerticalStackWithOverflow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
                yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.VerticalStackWithOverflow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.Right };
                yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.VerticalStackWithOverflow, childDock, RightToLeft.No, ToolStripDropDownDirection.Right };
                yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.VerticalStackWithOverflow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(DefaultDropDownDirection_GetWithToolStripPanelParent_TestData))]
        public void ToolStrip_DefaultDropDownDirection_GetWithToolStripPanelParent_ReturnsExpected(DockStyle parentDock, ToolStripLayoutStyle layoutStyle, DockStyle dock, RightToLeft rightToLeft, ToolStripDropDownDirection expected)
        {
            using var parent = new ToolStripPanel
            {
                Dock = parentDock
            };
            using var control = new ToolStrip
            {
                Parent = parent,
                LayoutStyle = layoutStyle,
                Dock = dock,
                RightToLeft = rightToLeft
            };
            Assert.Equal(expected, control.DefaultDropDownDirection);
        }

        public static IEnumerable<object[]> DefaultDropDownDirection_GetDesignModeWithToolStripPanelParent_TestData()
        {
            foreach (DockStyle childDock in new object[] { DockStyle.None, DockStyle.Left, DockStyle.Right, DockStyle.Top, DockStyle.Bottom })
            {
                yield return new object[] { DockStyle.None, ToolStripLayoutStyle.Flow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { DockStyle.None, ToolStripLayoutStyle.Flow, childDock, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.None, ToolStripLayoutStyle.Flow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.Flow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.Flow, childDock, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.Flow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.Flow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.Flow, childDock, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.Flow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.Flow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.Flow, childDock, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.Flow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.Flow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.AboveLeft };
                yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.Flow, childDock, RightToLeft.No, ToolStripDropDownDirection.AboveRight };
                yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.Flow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.AboveRight };

                yield return new object[] { DockStyle.None, ToolStripLayoutStyle.HorizontalStackWithOverflow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { DockStyle.None, ToolStripLayoutStyle.HorizontalStackWithOverflow, childDock, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.None, ToolStripLayoutStyle.HorizontalStackWithOverflow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.HorizontalStackWithOverflow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.HorizontalStackWithOverflow, childDock, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.HorizontalStackWithOverflow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.HorizontalStackWithOverflow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.HorizontalStackWithOverflow, childDock, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.HorizontalStackWithOverflow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.HorizontalStackWithOverflow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.HorizontalStackWithOverflow, childDock, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.HorizontalStackWithOverflow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.HorizontalStackWithOverflow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.AboveLeft };
                yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.HorizontalStackWithOverflow, childDock, RightToLeft.No, ToolStripDropDownDirection.AboveRight };
                yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.HorizontalStackWithOverflow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.AboveRight };

                yield return new object[] { DockStyle.None, ToolStripLayoutStyle.StackWithOverflow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { DockStyle.None, ToolStripLayoutStyle.StackWithOverflow, childDock, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.None, ToolStripLayoutStyle.StackWithOverflow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.StackWithOverflow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.Left };
                yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.StackWithOverflow, childDock, RightToLeft.No, ToolStripDropDownDirection.Left };
                yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.StackWithOverflow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.Left };
                yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.StackWithOverflow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.Right };
                yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.StackWithOverflow, childDock, RightToLeft.No, ToolStripDropDownDirection.Right };
                yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.StackWithOverflow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
                yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.StackWithOverflow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.StackWithOverflow, childDock, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.StackWithOverflow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.StackWithOverflow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.AboveLeft };
                yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.StackWithOverflow, childDock, RightToLeft.No, ToolStripDropDownDirection.AboveRight };
                yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.StackWithOverflow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.AboveRight };

                yield return new object[] { DockStyle.None, ToolStripLayoutStyle.Table, childDock, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { DockStyle.None, ToolStripLayoutStyle.Table, childDock, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.None, ToolStripLayoutStyle.Table, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.Table, childDock, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.Table, childDock, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.Table, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.Table, childDock, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.Table, childDock, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.Table, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.Table, childDock, RightToLeft.Yes, ToolStripDropDownDirection.BelowLeft };
                yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.Table, childDock, RightToLeft.No, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.Table, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.BelowRight };
                yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.Table, childDock, RightToLeft.Yes, ToolStripDropDownDirection.AboveLeft };
                yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.Table, childDock, RightToLeft.No, ToolStripDropDownDirection.AboveRight };
                yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.Table, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.AboveRight };

                yield return new object[] { DockStyle.None, ToolStripLayoutStyle.VerticalStackWithOverflow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.Right };
                yield return new object[] { DockStyle.None, ToolStripLayoutStyle.VerticalStackWithOverflow, childDock, RightToLeft.No, ToolStripDropDownDirection.Right };
                yield return new object[] { DockStyle.None, ToolStripLayoutStyle.VerticalStackWithOverflow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
                yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.VerticalStackWithOverflow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.Left };
                yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.VerticalStackWithOverflow, childDock, RightToLeft.No, ToolStripDropDownDirection.Left };
                yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.VerticalStackWithOverflow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.Left };
                yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.VerticalStackWithOverflow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.Right };
                yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.VerticalStackWithOverflow, childDock, RightToLeft.No, ToolStripDropDownDirection.Right };
                yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.VerticalStackWithOverflow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
                yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.VerticalStackWithOverflow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.Right };
                yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.VerticalStackWithOverflow, childDock, RightToLeft.No, ToolStripDropDownDirection.Right };
                yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.VerticalStackWithOverflow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
                yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.VerticalStackWithOverflow, childDock, RightToLeft.Yes, ToolStripDropDownDirection.Right };
                yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.VerticalStackWithOverflow, childDock, RightToLeft.No, ToolStripDropDownDirection.Right };
                yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.VerticalStackWithOverflow, childDock, RightToLeft.Inherit, ToolStripDropDownDirection.Right };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(DefaultDropDownDirection_GetDesignModeWithToolStripPanelParent_TestData))]
        public void ToolStrip_DefaultDropDownDirection_GetDesignModeWithToolStripPanelParent_ReturnsExpected(DockStyle parentDock, ToolStripLayoutStyle layoutStyle, DockStyle dock, RightToLeft rightToLeft, ToolStripDropDownDirection expected)
        {
            var mockSite = new Mock<ISite>(MockBehavior.Default);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(true);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.Name)
                .Returns("Name");
            using var parent = new ToolStripPanel
            {
                Site = mockSite.Object,
                Dock = parentDock
            };
            using var control = new SubToolStrip
            {
                Parent = parent,
                Site = mockSite.Object,
                LayoutStyle = layoutStyle,
                Dock = dock,
                RightToLeft = rightToLeft
            };
            Assert.True(control.DesignMode);
            Assert.Equal(expected, control.DefaultDropDownDirection);
        }

        public static IEnumerable<object[]> DefaultDropDownDirection_Set_TestData()
        {
            yield return new object[] { ToolStripDropDownDirection.AboveLeft, ToolStripDropDownDirection.AboveLeft };
            yield return new object[] { ToolStripDropDownDirection.AboveRight, ToolStripDropDownDirection.AboveRight };
            yield return new object[] { ToolStripDropDownDirection.BelowLeft, ToolStripDropDownDirection.BelowLeft };
            yield return new object[] { ToolStripDropDownDirection.BelowRight, ToolStripDropDownDirection.BelowRight };
            yield return new object[] { ToolStripDropDownDirection.Default, ToolStripDropDownDirection.BelowRight };
            yield return new object[] { ToolStripDropDownDirection.Left, ToolStripDropDownDirection.Left };
            yield return new object[] { ToolStripDropDownDirection.Right, ToolStripDropDownDirection.Right };
        }

        [WinFormsTheory]
        [MemberData(nameof(DefaultDropDownDirection_Set_TestData))]
        public void ToolStrip_DefaultDropDownDirection_Set_GetReturnsExpected(ToolStripDropDownDirection value, ToolStripDropDownDirection expected)
        {
            using var control = new ToolStrip
            {
                DefaultDropDownDirection = value
            };
            Assert.Equal(expected, control.DefaultDropDownDirection);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.DefaultDropDownDirection = value;
            Assert.Equal(expected, control.DefaultDropDownDirection);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(DefaultDropDownDirection_Set_TestData))]
        public void ToolStrip_DefaultDropDownDirection_SetWithHandle_GetReturnsExpected(ToolStripDropDownDirection value, ToolStripDropDownDirection expected)
        {
            using var control = new ToolStrip();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.DefaultDropDownDirection = value;
            Assert.Equal(expected, control.DefaultDropDownDirection);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.DefaultDropDownDirection = value;
            Assert.Equal(expected, control.DefaultDropDownDirection);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ToolStrip_DefaultDropDownDirection_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStrip))[nameof(ToolStrip.DefaultDropDownDirection)];
            using var control = new ToolStrip();
            Assert.False(property.CanResetValue(control));

            control.DefaultDropDownDirection = ToolStripDropDownDirection.Right;
            Assert.Equal(ToolStripDropDownDirection.Right, control.DefaultDropDownDirection);
            Assert.False(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(ToolStripDropDownDirection.Right, control.DefaultDropDownDirection);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void ToolStrip_DefaultDropDownDirection_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStrip))[nameof(ToolStrip.DefaultDropDownDirection)];
            using var control = new ToolStrip();
            Assert.False(property.ShouldSerializeValue(control));

            control.DefaultDropDownDirection = ToolStripDropDownDirection.Right;
            Assert.Equal(ToolStripDropDownDirection.Right, control.DefaultDropDownDirection);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(ToolStripDropDownDirection.Right, control.DefaultDropDownDirection);
            Assert.True(property.ShouldSerializeValue(control));

            control.DefaultDropDownDirection = ToolStripDropDownDirection.Default;
            Assert.Equal(ToolStripDropDownDirection.BelowRight, control.DefaultDropDownDirection);
            Assert.False(property.ShouldSerializeValue(control));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ToolStripDropDownDirection))]
        public void ToolStrip_DefaultDropDownDirection_SetInvalidValue_ThrowsInvalidEnumArgumentException(ToolStripDropDownDirection value)
        {
            using var control = new ToolStrip();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.DefaultDropDownDirection = value);
        }

        [WinFormsFact]
        public void ToolStrip_DefaultGripMargin_Get_ReturnsExpected()
        {
            using var control = new SubToolStrip();
            Assert.Equal(new Padding(2, 2, 2, 2), control.DefaultGripMargin);
        }

        [WinFormsFact]
        public void ToolStrip_DefaultGripMargin_GetWithGrip_ReturnsExpected()
        {
            using var control = new SubToolStrip();
            Assert.Equal(new Padding(2, 2, 2, 2), control.GripMargin);
            Assert.Equal(new Padding(2, 2, 2, 2), control.DefaultGripMargin);
        }

        public static IEnumerable<object[]> DisplayRectangle_TestData()
        {
            yield return new object[] { ToolStripLayoutStyle.Flow, ToolStripGripStyle.Visible, RightToLeft.Yes, new Rectangle(0, 0, 99, 25) };
            yield return new object[] { ToolStripLayoutStyle.Flow, ToolStripGripStyle.Visible, RightToLeft.No, new Rectangle(0, 0, 99, 25) };
            yield return new object[] { ToolStripLayoutStyle.Flow, ToolStripGripStyle.Visible, RightToLeft.Inherit, new Rectangle(0, 0, 99, 25) };
            yield return new object[] { ToolStripLayoutStyle.Flow, ToolStripGripStyle.Hidden, RightToLeft.Yes, new Rectangle(0, 0, 99, 25) };
            yield return new object[] { ToolStripLayoutStyle.Flow, ToolStripGripStyle.Hidden, RightToLeft.No, new Rectangle(0, 0, 99, 25) };
            yield return new object[] { ToolStripLayoutStyle.Flow, ToolStripGripStyle.Hidden, RightToLeft.Inherit, new Rectangle(0, 0, 99, 25) };

            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripGripStyle.Visible, RightToLeft.Yes, new Rectangle(0, 0, 92, 25) };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripGripStyle.Visible, RightToLeft.No, new Rectangle(7, 0, 92, 25) };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripGripStyle.Visible, RightToLeft.Inherit, new Rectangle(7, 0, 92, 25) };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripGripStyle.Hidden, RightToLeft.Yes, new Rectangle(0, 0, 99, 25) };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripGripStyle.Hidden, RightToLeft.No, new Rectangle(0, 0, 99, 25) };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripGripStyle.Hidden, RightToLeft.Inherit, new Rectangle(0, 0, 99, 25) };

            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, ToolStripGripStyle.Visible, RightToLeft.Yes, new Rectangle(0, 0, 92, 25) };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, ToolStripGripStyle.Visible, RightToLeft.No, new Rectangle(7, 0, 92, 25) };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, ToolStripGripStyle.Visible, RightToLeft.Inherit, new Rectangle(7, 0, 92, 25) };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, ToolStripGripStyle.Hidden, RightToLeft.Yes, new Rectangle(0, 0, 99, 25) };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, ToolStripGripStyle.Hidden, RightToLeft.No, new Rectangle(0, 0, 99, 25) };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, ToolStripGripStyle.Hidden, RightToLeft.Inherit, new Rectangle(0, 0, 99, 25) };

            yield return new object[] { ToolStripLayoutStyle.Table, ToolStripGripStyle.Visible, RightToLeft.Yes, new Rectangle(0, 0, 99, 25) };
            yield return new object[] { ToolStripLayoutStyle.Table, ToolStripGripStyle.Visible, RightToLeft.No, new Rectangle(0, 0, 99, 25) };
            yield return new object[] { ToolStripLayoutStyle.Table, ToolStripGripStyle.Visible, RightToLeft.Inherit, new Rectangle(0, 0, 99, 25) };
            yield return new object[] { ToolStripLayoutStyle.Table, ToolStripGripStyle.Hidden, RightToLeft.Yes, new Rectangle(0, 0, 99, 25) };
            yield return new object[] { ToolStripLayoutStyle.Table, ToolStripGripStyle.Hidden, RightToLeft.No, new Rectangle(0, 0, 99, 25) };
            yield return new object[] { ToolStripLayoutStyle.Table, ToolStripGripStyle.Hidden, RightToLeft.Inherit, new Rectangle(0, 0, 99, 25) };

            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripGripStyle.Visible, RightToLeft.Yes, new Rectangle(0, 7, 99, 18) };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripGripStyle.Visible, RightToLeft.No, new Rectangle(0, 7, 99, 18) };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripGripStyle.Visible, RightToLeft.Inherit, new Rectangle(0, 7, 99, 18) };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripGripStyle.Hidden, RightToLeft.Yes, new Rectangle(0, 0, 99, 25) };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripGripStyle.Hidden, RightToLeft.No, new Rectangle(0, 0, 99, 25) };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripGripStyle.Hidden, RightToLeft.Inherit, new Rectangle(0, 0, 99, 25) };
        }

        [WinFormsTheory]
        [MemberData(nameof(DisplayRectangle_TestData))]
        public void ToolStrip_DisplayRectangle_Get_ReturnsExpected(ToolStripLayoutStyle layoutStyle, ToolStripGripStyle gripStyle, RightToLeft rightToLeft, Rectangle expected)
        {
            using var control = new SubToolStrip
            {
                LayoutStyle = layoutStyle,
                GripStyle = gripStyle,
                RightToLeft = rightToLeft
            };
            Assert.Equal(expected, control.DisplayRectangle);
            Assert.Equal(expected.Size, control.MaxItemSize);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStrip_Dock_GetDefaultDock_ReturnsExpected()
        {
            using var control = new CustomDefaultDockToolStrip();
            Assert.Equal(DockStyle.Right, control.Dock);
        }

        private class CustomDefaultDockToolStrip : ToolStrip
        {
            protected override DockStyle DefaultDock => DockStyle.Right;
        }

        public static IEnumerable<object[]> Dock_Set_TestData()
        {
            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Bottom, 1, Orientation.Horizontal, 1 };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Bottom, 1, Orientation.Horizontal, 0 };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Bottom, 1, Orientation.Horizontal, 1 };
            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Bottom, 1, Orientation.Horizontal, 1 };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Bottom, 1, Orientation.Vertical, 0 };

            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Fill, 1, Orientation.Horizontal, 1 };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Fill, 1, Orientation.Horizontal, 0 };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Fill, 1, Orientation.Horizontal, 1 };
            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Fill, 1, Orientation.Horizontal, 1 };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Fill, 1, Orientation.Vertical, 0 };

            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Left, 1, Orientation.Vertical, 1 };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Left, 1, Orientation.Horizontal, 0 };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, 1, Orientation.Vertical, 1 };
            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Left, 1, Orientation.Vertical, 1 };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Left, 1, Orientation.Vertical, 0 };

            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.None, 1, Orientation.Horizontal, 1 };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.None, 1, Orientation.Horizontal, 0 };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.None, 1, Orientation.Horizontal, 1 };
            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.None, 1, Orientation.Horizontal, 1 };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.None, 1, Orientation.Vertical, 0 };

            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Right, 1, Orientation.Vertical, 1 };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Right, 1, Orientation.Horizontal, 0 };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Right, 1, Orientation.Vertical, 1 };
            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Right, 1, Orientation.Vertical, 1 };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Right, 1, Orientation.Vertical, 0 };

            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Top, 0, Orientation.Horizontal, 0 };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Top, 0, Orientation.Horizontal, 0 };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, 0, Orientation.Horizontal, 0 };
            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Top, 0, Orientation.Horizontal, 0 };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Top, 0, Orientation.Vertical, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Dock_Set_TestData))]
        public void ToolStrip_Dock_Set_GetReturnsExpected(ToolStripLayoutStyle layoutStyle, DockStyle value, int expectedLayoutCallCount, Orientation expectedOrientation, int expectedLayoutStyleChangedCallCount)
        {
            using var control = new ToolStrip
            {
                LayoutStyle = layoutStyle
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                if (e.AffectedProperty == "Dock")
                {
                    Assert.Same(control, sender);
                    Assert.Same(control, e.AffectedControl);
                    layoutCallCount++;
                }
            };
            int layoutStyleChangedCallCount = 0;
            control.LayoutStyleChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                layoutStyleChangedCallCount++;
            };

            control.Dock = value;
            Assert.Equal(value, control.Dock);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedOrientation, control.Orientation);
            Assert.Equal(expectedLayoutStyleChangedCallCount, layoutStyleChangedCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Dock = value;
            Assert.Equal(value, control.Dock);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedOrientation, control.Orientation);
            Assert.Equal(expectedLayoutStyleChangedCallCount, layoutStyleChangedCallCount);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Dock_SetWithParent_TestData()
        {
            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Bottom, Orientation.Horizontal, 0, 1, 1 };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Bottom, Orientation.Horizontal, 1, 0, 1 };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Bottom, Orientation.Horizontal, 0, 1, 1 };
            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Bottom, Orientation.Horizontal, 0, 1, 1 };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Bottom, Orientation.Vertical, 0, 0, 1 };

            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Fill, Orientation.Horizontal, 0, 1, 1 };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Fill, Orientation.Horizontal, 1, 0, 1 };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Fill, Orientation.Horizontal, 0, 1, 1 };
            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Fill, Orientation.Horizontal, 0, 1, 1 };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Fill, Orientation.Vertical, 0, 0, 1 };

            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Left, Orientation.Vertical, 0, 1, 1 };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Left, Orientation.Horizontal, 1, 0, 1 };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, Orientation.Vertical, 0, 1, 1 };
            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Left, Orientation.Vertical, 0, 1, 1 };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Left, Orientation.Vertical, 0, 0, 1 };

            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.None, Orientation.Horizontal, 0, 1, 1 };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.None, Orientation.Horizontal, 1, 0, 1 };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.None, Orientation.Horizontal, 0, 1, 1 };
            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.None, Orientation.Horizontal, 0, 1, 1 };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.None, Orientation.Vertical, 0, 0, 1 };

            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Right, Orientation.Vertical, 0, 1, 1 };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Right, Orientation.Horizontal, 1, 0, 1 };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Right, Orientation.Vertical, 0, 1, 1 };
            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Right, Orientation.Vertical, 0, 1, 1 };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Right, Orientation.Vertical, 0, 0, 1 };

            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Top, Orientation.Horizontal, 0, 0, 0 };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Top, Orientation.Horizontal, 0, 0, 0 };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, Orientation.Horizontal, 0, 0, 0 };
            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Top, Orientation.Horizontal, 0, 0, 0 };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Top, Orientation.Vertical, 0, 0, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Dock_SetWithParent_TestData))]
        public void ToolStrip_Dock_SetWithParent_GetReturnsExpected(ToolStripLayoutStyle layoutStyle, DockStyle value, Orientation expectedOrientation, int expectedLayoutCallCount, int expectedLayoutStyleChangedCallCount, int expectedParentLayoutCallCount)
        {
            using var parent = new Control();
            using var control = new ToolStrip
            {
                LayoutStyle = layoutStyle,
                Parent = parent
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                if (e.AffectedProperty == "Dock")
                {
                    Assert.Same(control, sender);
                    Assert.Same(control, e.AffectedControl);
                    layoutCallCount++;
                }
            };
            int layoutStyleChangedCallCount = 0;
            control.LayoutStyleChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                layoutStyleChangedCallCount++;
            };
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                if (e.AffectedProperty == "Dock" || e.AffectedProperty == "Orientation")
                {
                    Assert.Same(parent, sender);
                    Assert.Same(control, e.AffectedControl);
                    parentLayoutCallCount++;
                }
            }
            parent.Layout += parentHandler;

            try
            {
                control.Dock = value;
                Assert.Equal(value, control.Dock);
                Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
                Assert.Equal(expectedLayoutCallCount, layoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
                Assert.Equal(expectedLayoutStyleChangedCallCount, layoutStyleChangedCallCount);
                Assert.Equal(expectedOrientation, control.Orientation);
                Assert.False(control.IsHandleCreated);

                // Set same.
                control.Dock = value;
                Assert.Equal(value, control.Dock);
                Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
                Assert.Equal(expectedLayoutCallCount, layoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
                Assert.Equal(expectedLayoutStyleChangedCallCount, layoutStyleChangedCallCount);
                Assert.Equal(expectedOrientation, control.Orientation);
                Assert.False(control.IsHandleCreated);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        public static IEnumerable<object[]> Dock_SetWithToolStripPanelParent_TestData()
        {
            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Bottom, Orientation.Horizontal, 1 };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Bottom, Orientation.Horizontal, 1 };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Bottom, Orientation.Horizontal, 1 };
            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Bottom, Orientation.Horizontal, 1 };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Bottom, Orientation.Horizontal, 1 };

            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Fill, Orientation.Horizontal, 1 };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Fill, Orientation.Horizontal, 1 };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Fill, Orientation.Horizontal, 1 };
            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Fill, Orientation.Horizontal, 1 };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Fill, Orientation.Horizontal, 1 };

            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Left, Orientation.Horizontal, 1 };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Left, Orientation.Horizontal, 1 };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, Orientation.Horizontal, 1 };
            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Left, Orientation.Horizontal, 1 };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Left, Orientation.Horizontal, 1 };

            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.None, Orientation.Horizontal, 1 };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.None, Orientation.Horizontal, 1 };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.None, Orientation.Horizontal, 1 };
            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.None, Orientation.Horizontal, 1 };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.None, Orientation.Horizontal, 1 };

            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Right, Orientation.Horizontal, 1 };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Right, Orientation.Horizontal, 1 };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Right, Orientation.Horizontal, 1 };
            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Right, Orientation.Horizontal, 1 };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Right, Orientation.Horizontal, 1 };

            yield return new object[] { ToolStripLayoutStyle.Flow, DockStyle.Top, Orientation.Horizontal, 0 };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, DockStyle.Top, Orientation.Horizontal, 0 };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, Orientation.Horizontal, 0 };
            yield return new object[] { ToolStripLayoutStyle.Table, DockStyle.Top, Orientation.Horizontal, 0 };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, DockStyle.Top, Orientation.Horizontal, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Dock_SetWithToolStripPanelParent_TestData))]
        public void ToolStrip_Dock_SetWithToolStripPanelParent_GetReturnsExpected(ToolStripLayoutStyle layoutStyle, DockStyle value, Orientation expectedOrientation, int expectedLayoutCallCount)
        {
            using var parent = new ToolStripPanel();
            using var control = new ToolStrip
            {
                LayoutStyle = layoutStyle,
                Parent = parent
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                if (e.AffectedProperty == "Dock")
                {
                    Assert.Same(control, sender);
                    Assert.Same(control, e.AffectedControl);
                    layoutCallCount++;
                }
            };
            int layoutStyleChangedCallCount = 0;
            control.LayoutStyleChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                layoutStyleChangedCallCount++;
            };
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                if (e.AffectedProperty == "Dock" || e.AffectedProperty == "Orientation")
                {
                    Assert.Same(parent, sender);
                    Assert.Same(control, e.AffectedControl);
                    parentLayoutCallCount++;
                }
            }
            parent.Layout += parentHandler;

            try
            {
                control.Dock = value;
                Assert.Equal(value, control.Dock);
                Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
                Assert.Equal(expectedLayoutCallCount, layoutCallCount);
                Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
                Assert.Equal(0, layoutStyleChangedCallCount);
                Assert.Equal(expectedOrientation, control.Orientation);
                Assert.False(control.IsHandleCreated);

                // Set same.
                control.Dock = value;
                Assert.Equal(value, control.Dock);
                Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
                Assert.Equal(expectedLayoutCallCount, layoutCallCount);
                Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
                Assert.Equal(0, layoutStyleChangedCallCount);
                Assert.Equal(expectedOrientation, control.Orientation);
                Assert.False(control.IsHandleCreated);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        [WinFormsFact]
        public void ToolStrip_Dock_SetWithHandler_CallsDockChanged()
        {
            using var control = new ToolStrip
            {
                Dock = DockStyle.None
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.DockChanged += handler;

            // Set different.
            control.Dock = DockStyle.Top;
            Assert.Equal(DockStyle.Top, control.Dock);
            Assert.Equal(1, callCount);

            // Set same.
            control.Dock = DockStyle.Top;
            Assert.Equal(DockStyle.Top, control.Dock);
            Assert.Equal(1, callCount);

            // Set different.
            control.Dock = DockStyle.Left;
            Assert.Equal(DockStyle.Left, control.Dock);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.DockChanged -= handler;
            control.Dock = DockStyle.Top;
            Assert.Equal(DockStyle.Top, control.Dock);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(DockStyle))]
        public void ToolStrip_Dock_SetInvalid_ThrowsInvalidEnumArgumentException(DockStyle value)
        {
            using var control = new ToolStrip();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.Dock = value);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetForeColorTheoryData))]
        public void ToolStrip_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new ToolStrip
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

        [WinFormsFact]
        public void ToolStrip_ForeColor_SetWithHandler_CallsForeColorChanged()
        {
            using var control = new ToolStrip();
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

        [WinFormsTheory]
        [InlineData(ToolStripGripStyle.Hidden, 1)]
        [InlineData(ToolStripGripStyle.Visible, 0)]
        public void ToolStrip_GripStyle_Set_GetReturnsExpected(ToolStripGripStyle value, int expectedLayoutCallCount)
        {
            using var control = new ToolStrip();
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
        [InlineData(ToolStripGripStyle.Hidden, 1)]
        [InlineData(ToolStripGripStyle.Visible, 0)]
        public void ToolStrip_GripStyle_SetWithHandle_GetReturnsExpected(ToolStripGripStyle value, int expectedLayoutCallCount)
        {
            using var control = new ToolStrip();
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

        public static IEnumerable<object[]> LayoutStyle_Set_TestData()
        {
            foreach (DockStyle dock in new DockStyle[] { DockStyle.None, DockStyle.Top, DockStyle.Bottom })
            {
                yield return new object[] { dock, ToolStripLayoutStyle.Flow, ToolStripLayoutStyle.Flow, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.StackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.Table, ToolStripLayoutStyle.Table, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, Orientation.Vertical, ToolStripGripDisplayStyle.Horizontal, 1 };
            }

            foreach (DockStyle dock in new DockStyle[] { DockStyle.Right, DockStyle.Left })
            {
                yield return new object[] { dock, ToolStripLayoutStyle.Flow, ToolStripLayoutStyle.Flow, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 1 };
                yield return new object[] { dock, ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 1 };
                yield return new object[] { dock, ToolStripLayoutStyle.StackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, Orientation.Vertical, ToolStripGripDisplayStyle.Horizontal, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.Table, ToolStripLayoutStyle.Table, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 1 };
                yield return new object[] { dock, ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, Orientation.Vertical, ToolStripGripDisplayStyle.Horizontal, 0 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(LayoutStyle_Set_TestData))]
        public void ToolStrip_LayoutStyle_Set_GetReturnsExpected(DockStyle dock, ToolStripLayoutStyle value, ToolStripLayoutStyle expected, Orientation expectedOrientation, ToolStripGripDisplayStyle expectedGripDisplayStyle, int expectedLayoutCallCount)
        {
            using var control = new ToolStrip
            {
                Dock = dock
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;

            control.LayoutStyle = value;
            Assert.Equal(expected, control.LayoutStyle);
            Assert.Equal(expectedOrientation, control.Orientation);
            Assert.Equal(expectedGripDisplayStyle, control.GripDisplayStyle);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.LayoutStyle = value;
            Assert.Equal(expected, control.LayoutStyle);
            Assert.Equal(expectedOrientation, control.Orientation);
            Assert.Equal(expectedGripDisplayStyle, control.GripDisplayStyle);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(LayoutStyle_Set_TestData))]
        public void ToolStrip_LayoutStyle_SetWithParent_GetReturnsExpected(DockStyle dock, ToolStripLayoutStyle value, ToolStripLayoutStyle expected, Orientation expectedOrientation, ToolStripGripDisplayStyle expectedGripDisplayStyle, int expectedLayoutCallCount)
        {
            using var parent = new Control();
            using var control = new ToolStrip
            {
                Parent = parent,
                Dock = dock
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
            parent.Layout += parentHandler;

            try
            {
                control.LayoutStyle = value;
                Assert.Equal(expected, control.LayoutStyle);
                Assert.Equal(expectedOrientation, control.Orientation);
                Assert.Equal(expectedGripDisplayStyle, control.GripDisplayStyle);
                Assert.Equal(expectedLayoutCallCount * 2, layoutCallCount);
                Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);
                Assert.False(parent.IsHandleCreated);

                // Set same.
                control.LayoutStyle = value;
                Assert.Equal(expected, control.LayoutStyle);
                Assert.Equal(expectedOrientation, control.Orientation);
                Assert.Equal(expectedGripDisplayStyle, control.GripDisplayStyle);
                Assert.Equal(expectedLayoutCallCount * 2, layoutCallCount);
                Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);
                Assert.False(parent.IsHandleCreated);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        public static IEnumerable<object[]> LayoutStyle_SetWithToolStripPanelParent_TestData()
        {
            foreach (DockStyle dock in new DockStyle[] { DockStyle.None, DockStyle.Top, DockStyle.Bottom })
            {
                yield return new object[] { dock, ToolStripLayoutStyle.Flow, ToolStripLayoutStyle.Flow, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.StackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.Table, ToolStripLayoutStyle.Table, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, Orientation.Vertical, ToolStripGripDisplayStyle.Horizontal, 1 };
            }

            foreach (DockStyle dock in new DockStyle[] { DockStyle.Right, DockStyle.Left })
            {
                yield return new object[] { dock, ToolStripLayoutStyle.Flow, ToolStripLayoutStyle.Flow, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.StackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.Table, ToolStripLayoutStyle.Table, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, Orientation.Vertical, ToolStripGripDisplayStyle.Horizontal, 1 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(LayoutStyle_SetWithToolStripPanelParent_TestData))]
        public void ToolStrip_LayoutStyle_SetWithToolStripPanelParent_GetReturnsExpected(DockStyle dock, ToolStripLayoutStyle value, ToolStripLayoutStyle expected, Orientation expectedOrientation, ToolStripGripDisplayStyle expectedGripDisplayStyle, int expectedLayoutCallCount)
        {
            using var parent = new ToolStripPanel();
            using var control = new ToolStrip
            {
                Parent = parent,
                Dock = dock
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
            parent.Layout += parentHandler;

            try
            {
                control.LayoutStyle = value;
                Assert.Equal(expected, control.LayoutStyle);
                Assert.Equal(expectedOrientation, control.Orientation);
                Assert.Equal(expectedGripDisplayStyle, control.GripDisplayStyle);
                Assert.Equal(expectedLayoutCallCount * 2, layoutCallCount);
                Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);
                Assert.False(parent.IsHandleCreated);

                // Set same.
                control.LayoutStyle = value;
                Assert.Equal(expected, control.LayoutStyle);
                Assert.Equal(expectedOrientation, control.Orientation);
                Assert.Equal(expectedGripDisplayStyle, control.GripDisplayStyle);
                Assert.Equal(expectedLayoutCallCount * 2, layoutCallCount);
                Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);
                Assert.False(parent.IsHandleCreated);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        public static IEnumerable<object[]> LayoutStyle_SetWithCustomOldValue_TestData()
        {
            foreach (DockStyle dock in new DockStyle[] { DockStyle.None, DockStyle.Top, DockStyle.Bottom })
            {
                yield return new object[] { dock, ToolStripLayoutStyle.Flow, ToolStripLayoutStyle.Flow, ToolStripLayoutStyle.Flow, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.Flow, ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.Flow, ToolStripLayoutStyle.StackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 1 };
                yield return new object[] { dock, ToolStripLayoutStyle.Flow, ToolStripLayoutStyle.Table, ToolStripLayoutStyle.Table, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.Flow, ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, Orientation.Vertical, ToolStripGripDisplayStyle.Horizontal, 1 };
                yield return new object[] { dock, ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripLayoutStyle.Flow, ToolStripLayoutStyle.Flow, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripLayoutStyle.StackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 1 };
                yield return new object[] { dock, ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripLayoutStyle.Table, ToolStripLayoutStyle.Table, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, Orientation.Vertical, ToolStripGripDisplayStyle.Horizontal, 1 };
                yield return new object[] { dock, ToolStripLayoutStyle.StackWithOverflow, ToolStripLayoutStyle.Flow, ToolStripLayoutStyle.Flow, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.StackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.StackWithOverflow, ToolStripLayoutStyle.StackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.StackWithOverflow, ToolStripLayoutStyle.Table, ToolStripLayoutStyle.Table, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.StackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, Orientation.Vertical, ToolStripGripDisplayStyle.Horizontal, 1 };
                yield return new object[] { dock, ToolStripLayoutStyle.Table, ToolStripLayoutStyle.Flow, ToolStripLayoutStyle.Flow, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.Table, ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.Table, ToolStripLayoutStyle.StackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 1 };
                yield return new object[] { dock, ToolStripLayoutStyle.Table, ToolStripLayoutStyle.Table, ToolStripLayoutStyle.Table, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.Table, ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, Orientation.Vertical, ToolStripGripDisplayStyle.Horizontal, 1 };
                yield return new object[] { dock, ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripLayoutStyle.Flow, ToolStripLayoutStyle.Flow, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 1 };
                yield return new object[] { dock, ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 1 };
                yield return new object[] { dock, ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripLayoutStyle.StackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 1 };
                yield return new object[] { dock, ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripLayoutStyle.Table, ToolStripLayoutStyle.Table, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 1 };
                yield return new object[] { dock, ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, Orientation.Vertical, ToolStripGripDisplayStyle.Horizontal, 0 };
            }

            foreach (DockStyle dock in new DockStyle[] { DockStyle.Right }) //, DockStyle.Left })
            {
                yield return new object[] { dock, ToolStripLayoutStyle.Flow, ToolStripLayoutStyle.Flow, ToolStripLayoutStyle.Flow, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.Flow, ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.Flow, ToolStripLayoutStyle.StackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, Orientation.Vertical, ToolStripGripDisplayStyle.Horizontal, 1 };
                yield return new object[] { dock, ToolStripLayoutStyle.Flow, ToolStripLayoutStyle.Table, ToolStripLayoutStyle.Table, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.Flow, ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, Orientation.Vertical, ToolStripGripDisplayStyle.Horizontal, 1 };
                yield return new object[] { dock, ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripLayoutStyle.Flow, ToolStripLayoutStyle.Flow, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripLayoutStyle.StackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, Orientation.Vertical, ToolStripGripDisplayStyle.Horizontal, 1 };
                yield return new object[] { dock, ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripLayoutStyle.Table, ToolStripLayoutStyle.Table, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, Orientation.Vertical, ToolStripGripDisplayStyle.Horizontal, 1 };
                yield return new object[] { dock, ToolStripLayoutStyle.StackWithOverflow, ToolStripLayoutStyle.Flow, ToolStripLayoutStyle.Flow, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 1 };
                yield return new object[] { dock, ToolStripLayoutStyle.StackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 1 };
                yield return new object[] { dock, ToolStripLayoutStyle.StackWithOverflow, ToolStripLayoutStyle.StackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, Orientation.Vertical, ToolStripGripDisplayStyle.Horizontal, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.StackWithOverflow, ToolStripLayoutStyle.Table, ToolStripLayoutStyle.Table, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 1 };
                yield return new object[] { dock, ToolStripLayoutStyle.StackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, Orientation.Vertical, ToolStripGripDisplayStyle.Horizontal, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.Table, ToolStripLayoutStyle.Flow, ToolStripLayoutStyle.Flow, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.Table, ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.Table, ToolStripLayoutStyle.StackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, Orientation.Vertical, ToolStripGripDisplayStyle.Horizontal, 1 };
                yield return new object[] { dock, ToolStripLayoutStyle.Table, ToolStripLayoutStyle.Table, ToolStripLayoutStyle.Table, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 0 };
                yield return new object[] { dock, ToolStripLayoutStyle.Table, ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, Orientation.Vertical, ToolStripGripDisplayStyle.Horizontal, 1 };
                yield return new object[] { dock, ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripLayoutStyle.Flow, ToolStripLayoutStyle.Flow, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 1 };
                yield return new object[] { dock, ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 1 };
                yield return new object[] { dock, ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripLayoutStyle.StackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, Orientation.Vertical, ToolStripGripDisplayStyle.Horizontal, 1 };
                yield return new object[] { dock, ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripLayoutStyle.Table, ToolStripLayoutStyle.Table, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 1 };
                yield return new object[] { dock, ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, Orientation.Vertical, ToolStripGripDisplayStyle.Horizontal, 0 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(LayoutStyle_SetWithCustomOldValue_TestData))]
        public void ToolStrip_LayoutStyle_SetWithCustomOldValue_GetReturnsExpected(DockStyle dock, ToolStripLayoutStyle oldValue, ToolStripLayoutStyle value, ToolStripLayoutStyle expected, Orientation expectedOrientation, ToolStripGripDisplayStyle expectedGripDisplayStyle, int expectedLayoutCallCount)
        {
            using var control = new ToolStrip
            {
                Dock = dock,
                LayoutStyle = oldValue
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;

            control.LayoutStyle = value;
            Assert.Equal(expected, control.LayoutStyle);
            Assert.Equal(expectedOrientation, control.Orientation);
            Assert.Equal(expectedGripDisplayStyle, control.GripDisplayStyle);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.LayoutStyle = value;
            Assert.Equal(expected, control.LayoutStyle);
            Assert.Equal(expectedOrientation, control.Orientation);
            Assert.Equal(expectedGripDisplayStyle, control.GripDisplayStyle);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> LayoutStyle_SetWithHandle_TestData()
        {
            yield return new object[] { DockStyle.None, ToolStripLayoutStyle.Flow, ToolStripLayoutStyle.Flow, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 1 };
            yield return new object[] { DockStyle.None, ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 1 };
            yield return new object[] { DockStyle.None, ToolStripLayoutStyle.StackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 0 };
            yield return new object[] { DockStyle.None, ToolStripLayoutStyle.Table, ToolStripLayoutStyle.Table, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 1 };
            yield return new object[] { DockStyle.None, ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, Orientation.Vertical, ToolStripGripDisplayStyle.Horizontal, 2 };

            yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.Flow, ToolStripLayoutStyle.Flow, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 2 };
            yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 2 };
            yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.StackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, Orientation.Vertical, ToolStripGripDisplayStyle.Horizontal, 0 };
            yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.Table, ToolStripLayoutStyle.Table, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 2 };
            yield return new object[] { DockStyle.Left, ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, Orientation.Vertical, ToolStripGripDisplayStyle.Horizontal, 1 };

            yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.Flow, ToolStripLayoutStyle.Flow, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 2 };
            yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 2 };
            yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.StackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, Orientation.Vertical, ToolStripGripDisplayStyle.Horizontal, 0 };
            yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.Table, ToolStripLayoutStyle.Table, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 2 };
            yield return new object[] { DockStyle.Right, ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, Orientation.Vertical, ToolStripGripDisplayStyle.Horizontal, 1 };

            yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.Flow, ToolStripLayoutStyle.Flow, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 1 };
            yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 1 };
            yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.StackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 0 };
            yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.Table, ToolStripLayoutStyle.Table, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 1 };
            yield return new object[] { DockStyle.Top, ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, Orientation.Vertical, ToolStripGripDisplayStyle.Horizontal, 2 };

            yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.Flow, ToolStripLayoutStyle.Flow, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 1 };
            yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.HorizontalStackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 1 };
            yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.StackWithOverflow, ToolStripLayoutStyle.HorizontalStackWithOverflow, Orientation.Horizontal, ToolStripGripDisplayStyle.Vertical, 0 };
            yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.Table, ToolStripLayoutStyle.Table, Orientation.Horizontal, ToolStripGripDisplayStyle.Horizontal, 1 };
            yield return new object[] { DockStyle.Bottom, ToolStripLayoutStyle.VerticalStackWithOverflow, ToolStripLayoutStyle.VerticalStackWithOverflow, Orientation.Vertical, ToolStripGripDisplayStyle.Horizontal, 2 };
        }

        [WinFormsTheory]
        [MemberData(nameof(LayoutStyle_SetWithHandle_TestData))]
        public void ToolStrip_LayoutStyle_SetWithHandle_GetReturnsExpected(DockStyle dock, ToolStripLayoutStyle value, ToolStripLayoutStyle expected, Orientation expectedOrientation, ToolStripGripDisplayStyle expectedGripDisplayStyle, int expectedLayoutCallCount)
        {
            using var control = new ToolStrip
            {
                Dock = dock
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

            control.LayoutStyle = value;
            Assert.Equal(expected, control.LayoutStyle);
            Assert.Equal(expectedOrientation, control.Orientation);
            Assert.Equal(expectedGripDisplayStyle, control.GripDisplayStyle);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedLayoutCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.LayoutStyle = value;
            Assert.Equal(expected, control.LayoutStyle);
            Assert.Equal(expectedOrientation, control.Orientation);
            Assert.Equal(expectedGripDisplayStyle, control.GripDisplayStyle);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedLayoutCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ToolStrip_LayoutStyle_SetWithHandler_CallsLayoutStyleChanged()
        {
            using var control = new ToolStrip();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.LayoutStyleChanged += handler;

            // Set different.
            control.LayoutStyle = ToolStripLayoutStyle.Flow;
            Assert.Equal(ToolStripLayoutStyle.Flow, control.LayoutStyle);
            Assert.Equal(1, callCount);

            // Set same.
            control.LayoutStyle = ToolStripLayoutStyle.Flow;
            Assert.Equal(ToolStripLayoutStyle.Flow, control.LayoutStyle);
            Assert.Equal(1, callCount);

            // Set different.
            control.LayoutStyle = ToolStripLayoutStyle.Table;
            Assert.Equal(ToolStripLayoutStyle.Table, control.LayoutStyle);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.LayoutStyleChanged -= handler;
            control.LayoutStyle = ToolStripLayoutStyle.Flow;
            Assert.Equal(ToolStripLayoutStyle.Flow, control.LayoutStyle);
            Assert.Equal(2, callCount);
        }

        public static IEnumerable<object[]> Renderer_Set_TestData()
        {
            yield return new object[] { new SubToolStripRenderer() };
            yield return new object[] { new ToolStripSystemRenderer() };
            yield return new object[] { new ToolStripProfessionalRenderer() };
        }

        [WinFormsTheory]
        [MemberData(nameof(Renderer_Set_TestData))]
        public void ToolStrip_Renderer_Set_ReturnsExpected(ToolStripRenderer value)
        {
            using var control = new ToolStrip
            {
                Renderer = value
            };
            Assert.Same(value, control.Renderer);
            Assert.Equal(ToolStripRenderMode.Custom, control.RenderMode);

            // Set same.
            control.Renderer = value;
            Assert.Same(value, control.Renderer);
            Assert.Equal(ToolStripRenderMode.Custom, control.RenderMode);

            // Set null.
            control.Renderer = null;
            Assert.NotNull(control.Renderer);
            Assert.NotSame(value, control.Renderer);
            Assert.Same(control.Renderer, control.Renderer);
            Assert.Equal(ToolStripRenderMode.ManagerRenderMode, control.RenderMode);
        }

        [WinFormsFact]
        public void ToolStrip_Renderer_SetWithHandler_CallsRendererChanged()
        {
            using var control = new ToolStrip();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.RendererChanged += handler;

            // Set different.
            var renderer = new SubToolStripRenderer();
            control.Renderer = renderer;
            Assert.Same(renderer, control.Renderer);
            Assert.Equal(1, callCount);

            // Set same.
            control.Renderer = renderer;
            Assert.Same(renderer, control.Renderer);
            Assert.Equal(1, callCount);

            // Set different.
            control.Renderer = null;
            Assert.NotNull(control.Renderer);
            Assert.NotSame(renderer, control.Renderer);
            Assert.Same(control.Renderer, control.Renderer);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.RendererChanged -= handler;
            control.Renderer = renderer;
            Assert.Same(renderer, control.Renderer);
            Assert.Equal(2, callCount);
        }

        private class SubToolStripRenderer : ToolStripRenderer
        {
        }

        [WinFormsTheory]
        [InlineData(ToolStripRenderMode.Professional, typeof(ToolStripProfessionalRenderer))]
        [InlineData(ToolStripRenderMode.System, typeof(ToolStripSystemRenderer))]
        public void ToolStrip_RenderMode_Set_ReturnsExpected(ToolStripRenderMode value, Type expectedRendererType)
        {
            using var control = new ToolStrip();
            int rendererChangedCallCount = 0;
            control.RendererChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                rendererChangedCallCount++;
            };

            // Set same.
            control.RenderMode = ToolStripRenderMode.ManagerRenderMode;
            Assert.Equal(ToolStripRenderMode.ManagerRenderMode, control.RenderMode);
            Assert.NotNull(control.Renderer);
            Assert.Same(control.Renderer, control.Renderer);
            Assert.IsType<ToolStripProfessionalRenderer>(control.Renderer);
            Assert.Equal(0, rendererChangedCallCount);

            // Set different.
            control.RenderMode = value;
            Assert.Equal(value, control.RenderMode);
            Assert.IsType(expectedRendererType, control.Renderer);
            Assert.Equal(1, rendererChangedCallCount);

            // Set same.
            control.RenderMode = value;
            Assert.Equal(value, control.RenderMode);
            Assert.IsType(expectedRendererType, control.Renderer);
            Assert.Equal(2, rendererChangedCallCount);

            // Set ManagerRenderMode.
            control.RenderMode = ToolStripRenderMode.ManagerRenderMode;
            Assert.Equal(ToolStripRenderMode.ManagerRenderMode, control.RenderMode);
            Assert.NotNull(control.Renderer);
            Assert.Same(control.Renderer, control.Renderer);
            Assert.IsType<ToolStripProfessionalRenderer>(control.Renderer);
            Assert.Equal(3, rendererChangedCallCount);
        }

        [WinFormsTheory]
        [InlineData(ToolStripRenderMode.Professional, typeof(ToolStripProfessionalRenderer))]
        [InlineData(ToolStripRenderMode.System, typeof(ToolStripSystemRenderer))]
        public void ToolStrip_RenderMode_SetWithCustomRenderer_ReturnsExpected(ToolStripRenderMode value, Type expectedRendererType)
        {
            using var control = new ToolStrip
            {
                Renderer = new SubToolStripRenderer()
            };
            int rendererChangedCallCount = 0;
            control.RendererChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                rendererChangedCallCount++;
            };

            control.RenderMode = value;
            Assert.Equal(value, control.RenderMode);
            Assert.IsType(expectedRendererType, control.Renderer);
            Assert.Equal(1, rendererChangedCallCount);

            // Set same.
            control.RenderMode = value;
            Assert.Equal(value, control.RenderMode);
            Assert.IsType(expectedRendererType, control.Renderer);
            Assert.Equal(2, rendererChangedCallCount);

            // Set ManagerRenderMode.
            control.RenderMode = ToolStripRenderMode.ManagerRenderMode;
            Assert.Equal(ToolStripRenderMode.ManagerRenderMode, control.RenderMode);
            Assert.NotNull(control.Renderer);
            Assert.Same(control.Renderer, control.Renderer);
            Assert.IsType<ToolStripProfessionalRenderer>(control.Renderer);
            Assert.Equal(3, rendererChangedCallCount);
        }

        [WinFormsFact]
        public void ToolStrip_RenderMode_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStrip))[nameof(ToolStrip.RenderMode)];
            using var control = new ToolStrip();
            Assert.False(property.CanResetValue(control));

            control.RenderMode = ToolStripRenderMode.Professional;
            Assert.Equal(ToolStripRenderMode.Professional, control.RenderMode);
            Assert.True(property.CanResetValue(control));

            control.RenderMode = ToolStripRenderMode.System;
            Assert.Equal(ToolStripRenderMode.System, control.RenderMode);
            Assert.True(property.CanResetValue(control));

            control.Renderer = new SubToolStripRenderer();
            Assert.Equal(ToolStripRenderMode.Custom, control.RenderMode);
            Assert.False(property.CanResetValue(control));

            control.RenderMode = ToolStripRenderMode.ManagerRenderMode;
            Assert.Equal(ToolStripRenderMode.ManagerRenderMode, control.RenderMode);
            Assert.False(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(ToolStripRenderMode.ManagerRenderMode, control.RenderMode);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void ToolStrip_RenderMode_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStrip))[nameof(ToolStrip.RenderMode)];
            using var control = new ToolStrip();
            Assert.False(property.ShouldSerializeValue(control));

            control.RenderMode = ToolStripRenderMode.Professional;
            Assert.Equal(ToolStripRenderMode.Professional, control.RenderMode);
            Assert.True(property.ShouldSerializeValue(control));

            control.RenderMode = ToolStripRenderMode.System;
            Assert.Equal(ToolStripRenderMode.System, control.RenderMode);
            Assert.True(property.ShouldSerializeValue(control));

            control.Renderer = new SubToolStripRenderer();
            Assert.Equal(ToolStripRenderMode.Custom, control.RenderMode);
            Assert.False(property.ShouldSerializeValue(control));

            control.RenderMode = ToolStripRenderMode.ManagerRenderMode;
            Assert.Equal(ToolStripRenderMode.ManagerRenderMode, control.RenderMode);
            Assert.False(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(ToolStripRenderMode.ManagerRenderMode, control.RenderMode);
            Assert.False(property.ShouldSerializeValue(control));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ToolStripRenderMode))]
        public void ToolStrip_RenderMode_SetInvalidValue_ThrowsInvalidEnumArgumentException(ToolStripRenderMode value)
        {
            using var control = new ToolStrip();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.RenderMode = value);
        }

        [WinFormsFact]
        public void ToolStrip_RenderMode_SetCustomThrowsInvalidEnumArgumentException()
        {
            using var control = new ToolStrip();
            Assert.Throws<NotSupportedException>(() => control.RenderMode = ToolStripRenderMode.Custom);
        }

        [WinFormsFact]
        public void ToolStrip_ShowToolTipItems_GetDefaultShowItemToolTips_ReturnsExpected()
        {
            using var control = new CustomDefaultShowItemToolTipsToolStrip();
            Assert.False(control.ShowItemToolTips);
        }

        private class CustomDefaultShowItemToolTipsToolStrip : ToolStrip
        {
            protected override bool DefaultShowItemToolTips => false;
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStrip_ShowItemToolTips_Set_GetReturnsExpected(bool value)
        {
            using var control = new ToolStrip
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
        public void ToolStrip_ShowItemToolTips_SetWithItems_GetReturnsExpected(bool value)
        {
            using var control = new ToolStrip();
            using var item = new SubToolStripItem();
            control.Items.Add(item);

            control.ShowItemToolTips = value;
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
        public void ToolStrip_ShowItemToolTips_SetWithOverflowButton_GetReturnsExpected(bool value)
        {
            using var control = new ToolStrip();
            Assert.NotNull(control.OverflowButton);

            control.ShowItemToolTips = value;
            Assert.Equal(value, control.ShowItemToolTips);
            Assert.Equal(value, control.OverflowButton.DropDown.ShowItemToolTips);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.ShowItemToolTips = value;
            Assert.Equal(value, control.ShowItemToolTips);
            Assert.Equal(value, control.OverflowButton.DropDown.ShowItemToolTips);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.ShowItemToolTips = !value;
            Assert.Equal(!value, control.ShowItemToolTips);
            Assert.Equal(value, control.OverflowButton.DropDown.ShowItemToolTips);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStrip_Stretch_Set_GetReturnsExpected(bool value)
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
        public void ToolStrip_Stretch_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var control = new ToolStrip();
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

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStrip_TabStop_Set_GetReturnsExpected(bool value)
        {
            using var control = new ToolStrip
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
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStrip_TabStop_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var control = new ToolStrip();
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
        public void ToolStrip_TabStop_SetWithHandler_CallsTabStopChanged()
        {
            using var control = new ToolStrip
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
        [InlineData(ToolStripTextDirection.Inherit, ToolStripTextDirection.Horizontal, 1)]
        [InlineData(ToolStripTextDirection.Horizontal, ToolStripTextDirection.Horizontal, 1)]
        [InlineData(ToolStripTextDirection.Vertical90, ToolStripTextDirection.Vertical90, 1)]
        [InlineData(ToolStripTextDirection.Vertical270, ToolStripTextDirection.Vertical270, 1)]
        public void ToolStrip_TextDirection_Set_GetReturnsExpected(ToolStripTextDirection value, ToolStripTextDirection expected, int expectedLayoutCallCount)
        {
            using var control = new ToolStrip();
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("TextDirection", e.AffectedProperty);
                layoutCallCount++;
            };

            control.TextDirection = value;
            Assert.Equal(expected, control.TextDirection);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.TextDirection = value;
            Assert.Equal(expected, control.TextDirection);
            Assert.Equal(expectedLayoutCallCount + 1, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(ToolStripTextDirection.Inherit, ToolStripTextDirection.Horizontal, 1)]
        [InlineData(ToolStripTextDirection.Horizontal, ToolStripTextDirection.Horizontal, 1)]
        [InlineData(ToolStripTextDirection.Vertical90, ToolStripTextDirection.Vertical90, 1)]
        [InlineData(ToolStripTextDirection.Vertical270, ToolStripTextDirection.Vertical270, 1)]
        public void ToolStrip_TextDirection_SetWithHandle_GetReturnsExpected(ToolStripTextDirection value, ToolStripTextDirection expected, int expectedLayoutCallCount)
        {
            using var control = new ToolStrip();
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
                Assert.Equal("TextDirection", e.AffectedProperty);
                layoutCallCount++;
            };

            control.TextDirection = value;
            Assert.Equal(expected, control.TextDirection);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedLayoutCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.TextDirection = value;
            Assert.Equal(expected, control.TextDirection);
            Assert.Equal(expectedLayoutCallCount + 1, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedLayoutCallCount + 1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(ToolStripTextDirection.Inherit, ToolStripTextDirection.Horizontal, 1)]
        [InlineData(ToolStripTextDirection.Horizontal, ToolStripTextDirection.Horizontal, 1)]
        [InlineData(ToolStripTextDirection.Vertical90, ToolStripTextDirection.Vertical90, 1)]
        [InlineData(ToolStripTextDirection.Vertical270, ToolStripTextDirection.Vertical270, 1)]
        public void ToolStrip_TextDirection_SetWithItems_GetReturnsExpected(ToolStripTextDirection value, ToolStripTextDirection expected, int expectedLayoutCallCount)
        {
            using var control = new ToolStrip();
            using var item1 = new SubToolStripItem();
            using var item2 = new SubToolStripItem();
            control.Items.Add(item1);
            control.Items.Add(item2);
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("TextDirection", e.AffectedProperty);
                layoutCallCount++;
            };

            control.TextDirection = value;
            Assert.Equal(expected, control.TextDirection);
            Assert.Equal(expected, item1.TextDirection);
            Assert.Equal(expected, item2.TextDirection);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.TextDirection = value;
            Assert.Equal(expected, control.TextDirection);
            Assert.Equal(expected, item1.TextDirection);
            Assert.Equal(expected, item2.TextDirection);
            Assert.Equal(expectedLayoutCallCount + 1, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(ToolStripTextDirection.Inherit, ToolStripTextDirection.Horizontal, 1)]
        [InlineData(ToolStripTextDirection.Horizontal, ToolStripTextDirection.Horizontal, 1)]
        [InlineData(ToolStripTextDirection.Vertical90, ToolStripTextDirection.Vertical90, 1)]
        [InlineData(ToolStripTextDirection.Vertical270, ToolStripTextDirection.Vertical270, 1)]
        public void ToolStrip_TextDirection_SetWithItemsWithTextDirection_GetReturnsExpected(ToolStripTextDirection value, ToolStripTextDirection expected, int expectedLayoutCallCount)
        {
            using var control = new ToolStrip();
            using var item1 = new SubToolStripItem
            {
                TextDirection = ToolStripTextDirection.Vertical90
            };
            using var item2 = new SubToolStripItem
            {
                TextDirection = ToolStripTextDirection.Vertical270
            };
            control.Items.Add(item1);
            control.Items.Add(item2);
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("TextDirection", e.AffectedProperty);
                layoutCallCount++;
            };

            control.TextDirection = value;
            Assert.Equal(expected, control.TextDirection);
            Assert.Equal(ToolStripTextDirection.Vertical90, item1.TextDirection);
            Assert.Equal(ToolStripTextDirection.Vertical270, item2.TextDirection);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.TextDirection = value;
            Assert.Equal(expected, control.TextDirection);
            Assert.Equal(ToolStripTextDirection.Vertical90, item1.TextDirection);
            Assert.Equal(ToolStripTextDirection.Vertical270, item2.TextDirection);
            Assert.Equal(expectedLayoutCallCount + 1, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(ToolStripTextDirection.Inherit, ToolStripTextDirection.Horizontal, 1)]
        [InlineData(ToolStripTextDirection.Horizontal, ToolStripTextDirection.Horizontal, 1)]
        [InlineData(ToolStripTextDirection.Vertical90, ToolStripTextDirection.Vertical90, 1)]
        [InlineData(ToolStripTextDirection.Vertical270, ToolStripTextDirection.Vertical270, 1)]
        public void ToolStrip_TextDirection_SetWithItemsWithHandle_GetReturnsExpected(ToolStripTextDirection value, ToolStripTextDirection expected, int expectedLayoutCallCount)
        {
            using var control = new ToolStrip();
            using var item1 = new SubToolStripItem();
            using var item2 = new SubToolStripItem();
            control.Items.Add(item1);
            control.Items.Add(item2);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int layoutCallCount = 0;
            void layoutHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("TextDirection", e.AffectedProperty);
                layoutCallCount++;
            }
            control.Layout += layoutHandler;

            try
            {
                control.TextDirection = value;
                Assert.Equal(expected, control.TextDirection);
                Assert.Equal(expected, item1.TextDirection);
                Assert.Equal(expected, item2.TextDirection);
                Assert.Equal(expectedLayoutCallCount, layoutCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(expectedLayoutCallCount * 3, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);

                // Set same.
                control.TextDirection = value;
                Assert.Equal(expected, control.TextDirection);
                Assert.Equal(expected, item1.TextDirection);
                Assert.Equal(expected, item2.TextDirection);
                Assert.Equal(expectedLayoutCallCount + 1, layoutCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal((expectedLayoutCallCount + 1) * 3, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
            }
            finally
            {
                control.Layout -= layoutHandler;
            }
        }

        [WinFormsTheory]
        [InlineData(ToolStripTextDirection.Inherit, ToolStripTextDirection.Horizontal, 1)]
        [InlineData(ToolStripTextDirection.Horizontal, ToolStripTextDirection.Horizontal, 1)]
        [InlineData(ToolStripTextDirection.Vertical90, ToolStripTextDirection.Vertical90, 1)]
        [InlineData(ToolStripTextDirection.Vertical270, ToolStripTextDirection.Vertical270, 1)]
        public void ToolStrip_TextDirection_SetWithItemsWithTextDirectionWithHandle_GetReturnsExpected(ToolStripTextDirection value, ToolStripTextDirection expected, int expectedLayoutCallCount)
        {
            using var control = new ToolStrip();
            using var item1 = new SubToolStripItem
            {
                TextDirection = ToolStripTextDirection.Vertical90
            };
            using var item2 = new SubToolStripItem
            {
                TextDirection = ToolStripTextDirection.Vertical270
            };
            control.Items.Add(item1);
            control.Items.Add(item2);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int layoutCallCount = 0;
            void layoutHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("TextDirection", e.AffectedProperty);
                layoutCallCount++;
            }
            control.Layout += layoutHandler;

            try
            {
                control.TextDirection = value;
                Assert.Equal(expected, control.TextDirection);
                Assert.Equal(ToolStripTextDirection.Vertical90, item1.TextDirection);
                Assert.Equal(ToolStripTextDirection.Vertical270, item2.TextDirection);
                Assert.Equal(expectedLayoutCallCount, layoutCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(expectedLayoutCallCount, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);

                // Set same.
                control.TextDirection = value;
                Assert.Equal(expected, control.TextDirection);
                Assert.Equal(ToolStripTextDirection.Vertical90, item1.TextDirection);
                Assert.Equal(ToolStripTextDirection.Vertical270, item2.TextDirection);
                Assert.Equal(expectedLayoutCallCount + 1, layoutCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(expectedLayoutCallCount + 1, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
            }
            finally
            {
                control.Layout -= layoutHandler;
            }
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ToolStripTextDirection))]
        public void ToolStrip_TextDirection_SetInvalidValue_ThrowsInvalidEnumArgumentException(ToolStripTextDirection value)
        {
            using var control = new ToolStrip();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.TextDirection = value);
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
        public void ToolStrip_CreateDefaultItem_InvokeButton_Success(string text, Image image, EventHandler onClick)
        {
            using var control = new SubToolStrip();
            ToolStripButton button = Assert.IsType<ToolStripButton>(control.CreateDefaultItem(text, image, onClick));
            Assert.Equal(text, button.Text);
            Assert.Same(image, button.Image);
        }

        public static IEnumerable<object[]> CreateDefaultItem_Separator_TestData()
        {
            EventHandler onClick = (sender, e) => { };

            yield return new object[] { null, null };
            yield return new object[] { new Bitmap(10, 10), onClick };
        }

        [WinFormsTheory]
        [MemberData(nameof(CreateDefaultItem_Separator_TestData))]
        public void ToolStrip_CreateDefaultItem_InvokeSeparator_Success(Image image, EventHandler onClick)
        {
            using var control = new SubToolStrip();
            ToolStripSeparator separator = Assert.IsType<ToolStripSeparator>(control.CreateDefaultItem("-", image, onClick));
            Assert.Empty(separator.Text);
            Assert.Null(separator.Image);
        }

        [WinFormsTheory]
        [InlineData(null, 1)]
        [InlineData("", 1)]
        [InlineData("text", 1)]
        [InlineData("-", 0)]
        public void ToolStrip_CreateDefaultItem_PerformClick_Success(string text, int expectedCallCount)
        {
            int callCount = 0;
            EventHandler onClick = (sender, e) => callCount++;
            using var control = new SubToolStrip();
            ToolStripItem button = Assert.IsAssignableFrom<ToolStripItem>(control.CreateDefaultItem(text, null, onClick));
            button.PerformClick();
            Assert.Equal(expectedCallCount, callCount);
        }

        [WinFormsFact]
        public void ToolStrip_CreateLayoutSettings_InvokeFlow_ReturnsExpected()
        {
            using var toolStrip = new SubToolStrip();
            FlowLayoutSettings settings = Assert.IsType<FlowLayoutSettings>(toolStrip.CreateLayoutSettings(ToolStripLayoutStyle.Flow));
            Assert.Equal(FlowDirection.LeftToRight, settings.FlowDirection);
            Assert.NotNull(settings.LayoutEngine);
            Assert.Same(settings.LayoutEngine, settings.LayoutEngine);
            Assert.True(settings.WrapContents);
        }

        [WinFormsFact]
        public void ToolStrip_CreateLayoutSettings_InvokeTable_ReturnsExpected()
        {
            var toolStrip = new SubToolStrip();
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.CreateLayoutSettings(ToolStripLayoutStyle.Table));
            Assert.Equal(0, settings.ColumnCount);
            Assert.Empty(settings.ColumnStyles);
            Assert.Equal(TableLayoutPanelGrowStyle.AddRows, settings.GrowStyle);
            Assert.NotNull(settings.LayoutEngine);
            Assert.Same(settings.LayoutEngine, settings.LayoutEngine);
            Assert.Equal(0, settings.RowCount);
            Assert.Empty(settings.RowStyles);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ToolStripLayoutStyle))]
        [InlineData(ToolStripLayoutStyle.StackWithOverflow)]
        [InlineData(ToolStripLayoutStyle.HorizontalStackWithOverflow)]
        [InlineData(ToolStripLayoutStyle.VerticalStackWithOverflow)]
        public void ToolStrip_CreateLayoutSettings_InvalidLayoutStyle_ReturnsNull(ToolStripLayoutStyle layoutStyle)
        {
            var toolStrip = new SubToolStrip();
            Assert.Null(toolStrip.CreateLayoutSettings(layoutStyle));
        }
        [WinFormsFact]
        public void ToolStrip_Dispose_Invoke_Success()
        {
            using var control = new ToolStrip();
            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                Assert.Null(control.Parent);
                Assert.Empty(control.Controls);
                Assert.Empty(control.Items);
                Assert.Empty(control.DataBindings);
                Assert.False(control.IsHandleCreated);
                Assert.True(control.Disposing);
                Assert.Equal(callCount > 0, control.IsDisposed);
                callCount++;
            };
            control.Disposed += handler;

            try
            {
                control.Dispose();
                Assert.Null(control.Parent);
                Assert.Empty(control.Controls);
                Assert.Empty(control.Items);
                Assert.Empty(control.DataBindings);
                Assert.True(control.IsDisposed);
                Assert.False(control.Disposing);
                Assert.Equal(1, callCount);
                Assert.False(control.IsHandleCreated);

                // Dispose multiple times.
                control.Dispose();
                Assert.Null(control.Parent);
                Assert.Empty(control.Controls);
                Assert.Empty(control.Items);
                Assert.Empty(control.DataBindings);
                Assert.True(control.IsDisposed);
                Assert.False(control.Disposing);
                Assert.Equal(2, callCount);
                Assert.False(control.IsHandleCreated);
            }
            finally
            {
                control.Disposed -= handler;
            }
        }

        [WinFormsFact]
        public void ToolStrip_Dispose_InvokeWithItems_Success()
        {
            using var control = new ToolStrip();
            using var item1 = new SubToolStripItem();
            using var item2 = new SubToolStripItem();
            control.Items.Add(item1);
            control.Items.Add(item2);
            int itemRemovedCallCount = 0;
            control.ItemRemoved += (sender, e) => itemRemovedCallCount++;
            int controlRemovedCallCount = 0;
            control.ControlRemoved += (sender, e) => controlRemovedCallCount++;

            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                Assert.Null(control.Parent);
                Assert.Empty(control.Controls);
                Assert.Empty(control.Items);
                Assert.Empty(control.DataBindings);
                Assert.False(control.IsHandleCreated);
                Assert.True(control.Disposing);
                Assert.Equal(callCount > 0, control.IsDisposed);
                callCount++;
            };
            control.Disposed += handler;
            int item1CallCount = 0;
            item1.Disposed += (sender, e) => item1CallCount++;
            int item2CallCount = 0;
            item2.Disposed += (sender, e) => item2CallCount++;

            try
            {
                control.Dispose();
                Assert.Null(control.Parent);
                Assert.Empty(control.Controls);
                Assert.Empty(control.Items);
                Assert.Empty(control.DataBindings);
                Assert.Null(item1.Owner);
                Assert.Null(item2.Owner);
                Assert.Equal(0, controlRemovedCallCount);
                Assert.Equal(0, itemRemovedCallCount);
                Assert.True(control.IsDisposed);
                Assert.False(control.Disposing);
                Assert.True(item1.IsDisposed);
                Assert.True(item2.IsDisposed);
                Assert.Equal(1, callCount);
                Assert.Equal(1, item1CallCount);
                Assert.Equal(1, item2CallCount);
                Assert.False(control.IsHandleCreated);

                // Dispose multiple times.
                control.Dispose();
                Assert.Null(control.Parent);
                Assert.Empty(control.Controls);
                Assert.Empty(control.Items);
                Assert.Empty(control.DataBindings);
                Assert.Null(item1.Owner);
                Assert.Null(item2.Owner);
                Assert.Equal(0, controlRemovedCallCount);
                Assert.Equal(0, itemRemovedCallCount);
                Assert.True(control.IsDisposed);
                Assert.False(control.Disposing);
                Assert.True(item1.IsDisposed);
                Assert.True(item2.IsDisposed);
                Assert.Equal(2, callCount);
                Assert.Equal(1, item1CallCount);
                Assert.Equal(1, item2CallCount);
                Assert.False(control.IsHandleCreated);
            }
            finally
            {
                control.Disposed -= handler;
            }
        }

        [WinFormsFact]
        public void ToolStrip_Dispose_InvokeDisposing_Success()
        {
            using var control = new SubToolStrip();
            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                Assert.Null(control.Parent);
                Assert.Empty(control.Controls);
                Assert.Empty(control.Items);
                Assert.Empty(control.DataBindings);
                Assert.False(control.IsHandleCreated);
                Assert.True(control.Disposing);
                Assert.Equal(callCount > 0, control.IsDisposed);
                callCount++;
            };
            control.Disposed += handler;

            try
            {
                control.Dispose(true);
                Assert.Null(control.Parent);
                Assert.Empty(control.Controls);
                Assert.Empty(control.Items);
                Assert.Empty(control.DataBindings);
                Assert.True(control.IsDisposed);
                Assert.False(control.Disposing);
                Assert.Equal(1, callCount);
                Assert.False(control.IsHandleCreated);

                // Dispose multiple times.
                control.Dispose(true);
                Assert.Null(control.Parent);
                Assert.Empty(control.Controls);
                Assert.Empty(control.Items);
                Assert.Empty(control.DataBindings);
                Assert.True(control.IsDisposed);
                Assert.False(control.Disposing);
                Assert.Equal(2, callCount);
                Assert.False(control.IsHandleCreated);
            }
            finally
            {
                control.Disposed -= handler;
            }
        }

        [WinFormsFact]
        public void ToolStrip_Dispose_InvokeNotDisposing_Success()
        {
            using var control = new SubToolStrip();
            int callCount = 0;
            void handler(object sender, EventArgs e) => callCount++;
            control.Disposed += handler;

            try
            {
                control.Dispose(false);
                Assert.Null(control.Parent);
                Assert.Empty(control.Controls);
                Assert.Empty(control.Items);
                Assert.Empty(control.DataBindings);
                Assert.False(control.IsDisposed);
                Assert.False(control.Disposing);
                Assert.Equal(0, callCount);
                Assert.False(control.IsHandleCreated);

                // Dispose multiple times.
                control.Dispose(false);
                Assert.Null(control.Parent);
                Assert.Empty(control.Controls);
                Assert.Empty(control.Items);
                Assert.Empty(control.DataBindings);
                Assert.False(control.IsDisposed);
                Assert.False(control.Disposing);
                Assert.Equal(0, callCount);
                Assert.False(control.IsHandleCreated);
            }
            finally
            {
                control.Disposed -= handler;
            }
        }

        [WinFormsFact]
        public void ToolStrip_Dispose_InvokeDisposingWithItems_Success()
        {
            using var control = new SubToolStrip();
            using var item1 = new SubToolStripItem();
            using var item2 = new SubToolStripItem();
            control.Items.Add(item1);
            control.Items.Add(item2);
            int itemRemovedCallCount = 0;
            control.ItemRemoved += (sender, e) => itemRemovedCallCount++;
            int controlRemovedCallCount = 0;
            control.ControlRemoved += (sender, e) => controlRemovedCallCount++;

            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                Assert.Null(control.Parent);
                Assert.Empty(control.Controls);
                Assert.Empty(control.Items);
                Assert.Empty(control.DataBindings);
                Assert.False(control.IsHandleCreated);
                Assert.True(control.Disposing);
                Assert.Equal(callCount > 0, control.IsDisposed);
                callCount++;
            };
            control.Disposed += handler;
            int item1CallCount = 0;
            item1.Disposed += (sender, e) => item1CallCount++;
            int item2CallCount = 0;
            item2.Disposed += (sender, e) => item2CallCount++;

            try
            {
                control.Dispose(true);
                Assert.Null(control.Parent);
                Assert.Empty(control.Controls);
                Assert.Empty(control.Items);
                Assert.Empty(control.DataBindings);
                Assert.Null(item1.Owner);
                Assert.Null(item2.Owner);
                Assert.Equal(0, controlRemovedCallCount);
                Assert.Equal(0, itemRemovedCallCount);
                Assert.True(control.IsDisposed);
                Assert.False(control.Disposing);
                Assert.True(item1.IsDisposed);
                Assert.True(item2.IsDisposed);
                Assert.Equal(1, callCount);
                Assert.Equal(1, item1CallCount);
                Assert.Equal(1, item2CallCount);
                Assert.False(control.IsHandleCreated);

                // Dispose multiple times.
                control.Dispose(true);
                Assert.Null(control.Parent);
                Assert.Empty(control.Controls);
                Assert.Empty(control.Items);
                Assert.Empty(control.DataBindings);
                Assert.Null(item1.Owner);
                Assert.Null(item2.Owner);
                Assert.Equal(0, controlRemovedCallCount);
                Assert.Equal(0, itemRemovedCallCount);
                Assert.True(control.IsDisposed);
                Assert.False(control.Disposing);
                Assert.True(item1.IsDisposed);
                Assert.True(item2.IsDisposed);
                Assert.Equal(2, callCount);
                Assert.Equal(1, item1CallCount);
                Assert.Equal(1, item2CallCount);
                Assert.False(control.IsHandleCreated);
            }
            finally
            {
                control.Disposed -= handler;
            }
        }

        [WinFormsFact]
        public void ToolStrip_Dispose_InvokeNotDisposingWithItems_Success()
        {
            using var control = new SubToolStrip();
            using var item1 = new SubToolStripItem();
            using var item2 = new SubToolStripItem();
            control.Items.Add(item1);
            control.Items.Add(item2);
            int itemRemovedCallCount = 0;
            control.ItemRemoved += (sender, e) => itemRemovedCallCount++;
            int controlRemovedCallCount = 0;
            control.ControlRemoved += (sender, e) => controlRemovedCallCount++;

            int callCount = 0;
            void handler(object sender, EventArgs e) => callCount++;
            control.Disposed += handler;
            int item1CallCount = 0;
            item1.Disposed += (sender, e) => item1CallCount++;
            int item2CallCount = 0;
            item2.Disposed += (sender, e) => item2CallCount++;

            try
            {
                control.Dispose(false);
                Assert.Null(control.Parent);
                Assert.Empty(control.Controls);
                Assert.Equal(new ToolStripItem[] { item1, item2 }, control.Items.Cast<ToolStripItem>());
                Assert.Empty(control.DataBindings);
                Assert.Same(control, item1.Owner);
                Assert.Same(control, item2.Owner);
                Assert.Equal(0, controlRemovedCallCount);
                Assert.Equal(0, itemRemovedCallCount);
                Assert.False(control.IsDisposed);
                Assert.False(control.Disposing);
                Assert.False(item1.IsDisposed);
                Assert.False(item2.IsDisposed);
                Assert.Equal(0, callCount);
                Assert.Equal(0, item1CallCount);
                Assert.Equal(0, item2CallCount);
                Assert.False(control.IsHandleCreated);

                // Dispose multiple times.
                control.Dispose(false);
                Assert.Null(control.Parent);
                Assert.Empty(control.Controls);
                Assert.Equal(new ToolStripItem[] { item1, item2 }, control.Items.Cast<ToolStripItem>());
                Assert.Empty(control.DataBindings);
                Assert.Same(control, item1.Owner);
                Assert.Same(control, item2.Owner);
                Assert.Equal(0, controlRemovedCallCount);
                Assert.Equal(0, itemRemovedCallCount);
                Assert.False(control.IsDisposed);
                Assert.False(control.Disposing);
                Assert.False(item1.IsDisposed);
                Assert.False(item2.IsDisposed);
                Assert.Equal(0, callCount);
                Assert.Equal(0, item1CallCount);
                Assert.Equal(0, item2CallCount);
                Assert.False(control.IsHandleCreated);
            }
            finally
            {
                control.Disposed -= handler;
            }
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ArrowDirection))]
        public void ToolStrip_GetNextItem_NoItems_ReturnsNull(ArrowDirection direction)
        {
            var toolStrip = new ToolStrip();
            Assert.Null(toolStrip.GetNextItem(new SubToolStripItem(), direction));
            Assert.Null(toolStrip.GetNextItem(null, direction));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ArrowDirection))]
        public void ToolStrip_GetNextItem_InvalidDirection_ThrowsInvalidEnumArgumentException(ArrowDirection direction)
        {
            var toolStrip = new ToolStrip();
            Assert.Throws<InvalidEnumArgumentException>("direction", () => toolStrip.GetNextItem(new SubToolStripItem(), direction));
            Assert.Throws<InvalidEnumArgumentException>("direction", () => toolStrip.GetNextItem(null, direction));
        }

        [WinFormsFact]
        public void ToolStrip_GetAutoSizeMode_Invoke_ReturnsExpected()
        {
            using var control = new SubToolStrip();
            Assert.Equal(AutoSizeMode.GrowAndShrink, control.GetAutoSizeMode());
        }

        [WinFormsTheory]
        [InlineData(0, true)]
        [InlineData(SubToolStrip.ScrollStateAutoScrolling, false)]
        [InlineData(SubToolStrip.ScrollStateFullDrag, false)]
        [InlineData(SubToolStrip.ScrollStateHScrollVisible, false)]
        [InlineData(SubToolStrip.ScrollStateUserHasScrolled, false)]
        [InlineData(SubToolStrip.ScrollStateVScrollVisible, false)]
        [InlineData(int.MaxValue, false)]
        [InlineData((-1), false)]
        public void ToolStrip_GetScrollState_Invoke_ReturnsExpected(int bit, bool expected)
        {
            using var control = new SubToolStrip();
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
        public void ToolStrip_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
        {
            using var control = new SubToolStrip();
            Assert.Equal(expected, control.GetStyle(flag));

            // Call again to test caching.
            Assert.Equal(expected, control.GetStyle(flag));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStrip_OnBeginDrag_Invoke_CallsBeginDrag(EventArgs eventArgs)
        {
            using var control = new SubToolStrip();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.BeginDrag += handler;
            control.OnBeginDrag(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.BeginDrag -= handler;
            control.OnBeginDrag(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> ControlEventArgs_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ControlEventArgs(null) };
            yield return new object[] { new ControlEventArgs(new Control()) };
        }

        [WinFormsTheory]
        [MemberData(nameof(ControlEventArgs_TestData))]
        public void ToolStrip_OnControlAdded_Invoke_CallsControlAdded(ControlEventArgs eventArgs)
        {
            using var control = new SubToolStrip();
            int callCount = 0;
            ControlEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.ControlAdded += handler;
            control.OnControlAdded(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.ControlAdded -= handler;
            control.OnControlAdded(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ControlEventArgs_TestData))]
        public void ToolStrip_OnControlRemoved_Invoke_CallsControlRemoved(ControlEventArgs eventArgs)
        {
            using var control = new SubToolStrip();
            int callCount = 0;
            ControlEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.ControlRemoved += handler;
            control.OnControlRemoved(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.ControlRemoved -= handler;
            control.OnControlRemoved(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStrip_OnBeginDrag_InvokeWithHandle_CallsBeginDrag(EventArgs eventArgs)
        {
            using var control = new SubToolStrip();
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
            control.BeginDrag += handler;
            control.OnBeginDrag(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.BeginDrag -= handler;
            control.OnBeginDrag(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStrip_OnEndDrag_Invoke_CallsEndDrag(EventArgs eventArgs)
        {
            using var control = new SubToolStrip();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.EndDrag += handler;
            control.OnEndDrag(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.EndDrag -= handler;
            control.OnEndDrag(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStrip_OnEndDrag_InvokeWithHandle_CallsEndDrag(EventArgs eventArgs)
        {
            using var control = new SubToolStrip();
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
            control.EndDrag += handler;
            control.OnEndDrag(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.EndDrag -= handler;
            control.OnEndDrag(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStrip_OnLayoutCompleted_Invoke_CallsLayoutCompleted(EventArgs eventArgs)
        {
            using var control = new SubToolStrip();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.LayoutCompleted += handler;
            control.OnLayoutCompleted(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.LayoutCompleted -= handler;
            control.OnLayoutCompleted(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStrip_OnLayoutStyleChanged_Invoke_CallsLayoutStyleChanged(EventArgs eventArgs)
        {
            using var control = new SubToolStrip();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.LayoutStyleChanged += handler;
            control.OnLayoutStyleChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.LayoutStyleChanged -= handler;
            control.OnLayoutStyleChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStrip_OnLeave_Invoke_CallsLeave(EventArgs eventArgs)
        {
            using var control = new SubToolStrip();
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
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStrip_OnLostFocus_Invoke_CallsLostFocus(EventArgs eventArgs)
        {
            using var control = new SubToolStrip();
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

            // Remove handler.
            control.LostFocus -= handler;
            control.OnLostFocus(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void ToolStrip_OnPaintGrip_InvokeWithHandle_CallsPaintGrip()
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var eventArgs = new PaintEventArgs(graphics, Rectangle.Empty);

            using var control = new SubToolStrip();
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
            control.PaintGrip += handler;
            control.OnPaintGrip(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.PaintGrip -= handler;
            control.OnPaintGrip(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ToolStrip_OnPaintGrip_NullE_ThrowsNullReferenceException()
        {
            using var control = new SubToolStrip();
            Assert.Throws<NullReferenceException>(() => control.OnPaintGrip(null));
        }

        [WinFormsTheory]
        [InlineData((int)User32.WM.LBUTTONDOWN)]
        [InlineData((int)User32.WM.LBUTTONDBLCLK)]
        [InlineData((int)User32.WM.MBUTTONDOWN)]
        [InlineData((int)User32.WM.MBUTTONDBLCLK)]
        [InlineData((int)User32.WM.RBUTTONDOWN)]
        [InlineData((int)User32.WM.RBUTTONDBLCLK)]
        [InlineData((int)User32.WM.XBUTTONDOWN)]
        [InlineData((int)User32.WM.XBUTTONDBLCLK)]
        public void ToolStrip_WndProc_InvokeMouseDownWithHandle_Success(int msg)
        {
            using var control = new SubToolStrip();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int callCount = 0;
            control.MouseDown += (sender, e) => callCount++;
            var m = new Message
            {
                Msg = msg,
                LParam = IntPtr.Zero,
                WParam = PARAM.FromLowHigh(0, 1),
                Result = (IntPtr)250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(0, callCount);
            Assert.True(control.Capture);
            Assert.False(control.Focused);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        private class SubAxHost : AxHost
        {
            public SubAxHost(string clsid) : base(clsid)
            {
            }
        }

        private class SubToolStripItem : ToolStripItem
        {
        }

        private class SubToolStrip : ToolStrip
        {
            public new const int ScrollStateAutoScrolling = ToolStrip.ScrollStateAutoScrolling;

            public new const int ScrollStateHScrollVisible = ToolStrip.ScrollStateHScrollVisible;

            public new const int ScrollStateVScrollVisible = ToolStrip.ScrollStateVScrollVisible;

            public new const int ScrollStateUserHasScrolled = ToolStrip.ScrollStateUserHasScrolled;

            public new const int ScrollStateFullDrag = ToolStrip.ScrollStateFullDrag;

            public SubToolStrip() : base()
            {
            }

            public SubToolStrip(ToolStripItem[] items) : base(items)
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

            public new ToolStripItem CreateDefaultItem(string text, Image image, EventHandler onClick) => base.CreateDefaultItem(text, image, onClick);

            public new LayoutSettings CreateLayoutSettings(ToolStripLayoutStyle layoutStyle) => base.CreateLayoutSettings(layoutStyle);

            public new void Dispose(bool disposing) => base.Dispose(disposing);

            public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

            public new bool GetScrollState(int bit) => base.GetScrollState(bit);

            public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

            public new void OnBeginDrag(EventArgs e) => base.OnBeginDrag(e);

            public new void OnControlAdded(ControlEventArgs e) => base.OnControlAdded(e);

            public new void OnControlRemoved(ControlEventArgs e) => base.OnControlRemoved(e);

            public new void OnEndDrag(EventArgs e) => base.OnEndDrag(e);

            public new void OnLayoutCompleted(EventArgs e) => base.OnLayoutCompleted(e);

            public new void OnLayoutStyleChanged(EventArgs e) => base.OnLayoutStyleChanged(e);

            public new void OnLeave(EventArgs e) => base.OnLeave(e);

            public new void OnLostFocus(EventArgs e) => base.OnLostFocus(e);

            public new void OnPaintGrip(PaintEventArgs e) => base.OnPaintGrip(e);

            public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);

            public new void WndProc(ref Message m) => base.WndProc(ref m);
        }
    }
}
