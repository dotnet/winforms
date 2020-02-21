// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ToolStripTests
    {
        [WinFormsFact]
        public void ToolStrip_Ctor()
        {
            using var control = new SubToolStrip();
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
            Assert.True(control.CanOverflow);
            Assert.True(control.CanRaiseEvents);
            Assert.False(control.CausesValidation);
            Assert.Equal(new Rectangle(0, 0, 100, 25), control.ClientRectangle);
            Assert.Equal(new Size(100, 25), control.ClientSize);
            Assert.Null(control.Container);
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
            Assert.True(control.DisplayRectangle.X > 0);
            Assert.Equal(0, control.DisplayRectangle.Y);
            Assert.True(control.DisplayRectangle.Width > 0);
            Assert.Equal(25, control.DisplayRectangle.Height);
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
            Assert.False(control.IsCurrentlyDragging);
            Assert.False(control.IsDropDown);
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
            Assert.True(control.MaxItemSize.Width > 0);
            Assert.Equal(25, control.MaxItemSize.Height);
            Assert.False(control.MenuAutoExpand);
            Assert.Equal(Size.Empty, control.MinimumSize);
            Assert.Equal(Orientation.Horizontal, control.Orientation);
            Assert.NotNull(control.OverflowButton);
            Assert.Same(control.OverflowButton, control.OverflowButton);
            Assert.Equal(new Padding(0, 0, 1, 0), control.Padding);
            Assert.Null(control.Parent);
            Assert.True(control.PreferredSize.Width > 0);
            Assert.True(control.PreferredSize.Height > 0);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.False(control.RecreatingHandle);
            Assert.Null(control.Region);
            Assert.NotNull(control.Renderer);
            Assert.Same(control.Renderer, control.Renderer);
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
            Assert.NotNull(control.VerticalScroll);
            Assert.Same(control.VerticalScroll, control.VerticalScroll);
            Assert.True(control.Visible);
            Assert.False(control.VScroll);
            Assert.Equal(100, control.Width);
            Assert.True(ToolStripManager.ToolStrips.Contains(control));

            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Ctor_ToolStripItemArray_TestData()
        {
            yield return new object[] { new ToolStripItem[0] };
            yield return new object[] { new ToolStripItem[] { new SubToolStripItem() } };
        }

        [WinFormsTheory]
        [MemberData(nameof(Ctor_ToolStripItemArray_TestData))]
        public void ToolStrip_CtorToolStripItemArray(ToolStripItem[] items)
        {
            using var control = new SubToolStrip(items);
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
            Assert.True(control.CanOverflow);
            Assert.True(control.CanRaiseEvents);
            Assert.False(control.CausesValidation);
            Assert.Equal(new Rectangle(0, 0, 100, 25), control.ClientRectangle);
            Assert.Equal(new Size(100, 25), control.ClientSize);
            Assert.False(control.Created);
            Assert.Null(control.Container);
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
            Assert.True(control.DisplayRectangle.X > 0);
            Assert.Equal(0, control.DisplayRectangle.Y);
            Assert.True(control.DisplayRectangle.Width > 0);
            Assert.Equal(25, control.DisplayRectangle.Height);
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
            Assert.False(control.IsCurrentlyDragging);
            Assert.False(control.IsDropDown);
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
            Assert.True(control.MaxItemSize.Width > 0);
            Assert.Equal(25, control.MaxItemSize.Height);
            Assert.False(control.MenuAutoExpand);
            Assert.Equal(Size.Empty, control.MinimumSize);
            Assert.Equal(Orientation.Horizontal, control.Orientation);
            Assert.NotNull(control.OverflowButton);
            Assert.Same(control.OverflowButton, control.OverflowButton);
            Assert.Equal(new Padding(0, 0, 1, 0), control.Padding);
            Assert.Null(control.Parent);
            Assert.True(control.PreferredSize.Width > 0);
            Assert.True(control.PreferredSize.Height > 0);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.False(control.RecreatingHandle);
            Assert.Null(control.Region);
            Assert.NotNull(control.Renderer);
            Assert.Same(control.Renderer, control.Renderer);
            Assert.Equal(ToolStripRenderMode.ManagerRenderMode, control.RenderMode);
            Assert.False(control.ResizeRedraw);
            Assert.Equal(100, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.True(control.ShowItemToolTips);
            Assert.Null(control.Site);
            Assert.Equal(new Size(100, 25), control.Size);
            Assert.False(control.Stretch);
            Assert.Equal(0, control.TabIndex);
            Assert.False(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(ToolStripTextDirection.Horizontal, control.TextDirection);
            Assert.Equal(0, control.Top);
            Assert.Null(control.TopLevelControl);
            Assert.NotNull(control.VerticalScroll);
            Assert.Same(control.VerticalScroll, control.VerticalScroll);
            Assert.True(control.Visible);
            Assert.False(control.VScroll);
            Assert.Equal(100, control.Width);
            Assert.True(ToolStripManager.ToolStrips.Contains(control));

            Assert.False(control.IsHandleCreated);
        }

        [Fact]
        public void ToolStrip_Ctor_NullItems_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("toolStripItems", () => new ToolStrip(null));
        }

        [Fact]
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ArrowDirection))]
        public void ToolStrip_GetNextItem_NoItems_ReturnsNull(ArrowDirection direction)
        {
            var toolStrip = new ToolStrip();
            Assert.Null(toolStrip.GetNextItem(new SubToolStripItem(), direction));
            Assert.Null(toolStrip.GetNextItem(null, direction));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ArrowDirection))]
        public void ToolStrip_GetNextItem_InvalidDirection_ThrowsInvalidEnumArgumentException(ArrowDirection direction)
        {
            var toolStrip = new ToolStrip();
            Assert.Throws<InvalidEnumArgumentException>("direction", () => toolStrip.GetNextItem(new SubToolStripItem(), direction));
            Assert.Throws<InvalidEnumArgumentException>("direction", () => toolStrip.GetNextItem(null, direction));
        }

        [Fact]
        public void ToolStrip_CreateLayoutSettings_InvokeFlow_ReturnsExpected()
        {
            var toolStrip = new SubToolStrip();
            FlowLayoutSettings settings = Assert.IsType<FlowLayoutSettings>(toolStrip.CreateLayoutSettings(ToolStripLayoutStyle.Flow));
            Assert.Equal(FlowDirection.LeftToRight, settings.FlowDirection);
            Assert.NotNull(settings.LayoutEngine);
            Assert.Same(settings.LayoutEngine, settings.LayoutEngine);
            Assert.Same(toolStrip, settings.Owner);
            Assert.True(settings.WrapContents);
        }

        [Fact]
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
            Assert.Same(toolStrip, settings.Owner);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ToolStripLayoutStyle))]
        [InlineData(ToolStripLayoutStyle.StackWithOverflow)]
        [InlineData(ToolStripLayoutStyle.HorizontalStackWithOverflow)]
        [InlineData(ToolStripLayoutStyle.VerticalStackWithOverflow)]
        public void ToolStrip_CreateLayoutSettings_InvalidLayoutStyle_ReturnsNull(ToolStripLayoutStyle layoutStyle)
        {
            var toolStrip = new SubToolStrip();
            Assert.Null(toolStrip.CreateLayoutSettings(layoutStyle));
        }

        private class SubToolStrip : ToolStrip
        {
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

            public new LayoutSettings CreateLayoutSettings(ToolStripLayoutStyle layoutStyle) => base.CreateLayoutSettings(layoutStyle);
        }

        private class SubToolStripItem : ToolStripItem
        {
        }
    }
}
