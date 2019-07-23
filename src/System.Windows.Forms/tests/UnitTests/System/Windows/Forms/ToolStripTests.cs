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
        [Fact]
        public void ToolStrip_Ctor()
        {
            var toolStrip = new SubToolStrip();
            Assert.False(toolStrip.AllowItemReorder);
            Assert.True(toolStrip.AllowMerge);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, toolStrip.Anchor);
            Assert.False(toolStrip.AutoScroll);
            Assert.Equal(Size.Empty, toolStrip.AutoScrollMargin);
            Assert.Equal(Size.Empty, toolStrip.AutoScrollMinSize);
            Assert.Equal(Point.Empty, toolStrip.AutoScrollPosition);
            Assert.True(toolStrip.AutoSize);
            Assert.Equal(Control.DefaultBackColor, toolStrip.BackColor);
            Assert.Null(toolStrip.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, toolStrip.BackgroundImageLayout);
            Assert.Null(toolStrip.BindingContext);
            Assert.Equal(25, toolStrip.Bottom);
            Assert.Equal(new Rectangle(0, 0, 100, 25), toolStrip.Bounds);
            Assert.True(toolStrip.CanOverflow);
            Assert.True(toolStrip.CanRaiseEvents);
            Assert.False(toolStrip.CausesValidation);
            Assert.Equal(new Rectangle(0, 0, 100, 25), toolStrip.ClientRectangle);
            Assert.Equal(new Size(100, 25), toolStrip.ClientSize);
            Assert.False(toolStrip.Created);
            Assert.Null(toolStrip.Container);
            Assert.Empty(toolStrip.Controls);
            Assert.Same(toolStrip.Controls, toolStrip.Controls);
            Assert.Same(Cursors.Default, toolStrip.Cursor);
            Assert.Same(Cursors.Default, toolStrip.DefaultCursor);
            Assert.Equal(DockStyle.Top, toolStrip.DefaultDock);
            Assert.Equal(ToolStripDropDownDirection.BelowRight, toolStrip.DefaultDropDownDirection);
            Assert.Equal(ImeMode.Inherit, toolStrip.DefaultImeMode);
            Assert.Equal(new Padding(2, 2, 2, 2), toolStrip.DefaultGripMargin);
            Assert.Equal(Padding.Empty, toolStrip.DefaultMargin);
            Assert.Equal(Size.Empty, toolStrip.DefaultMaximumSize);
            Assert.Equal(Size.Empty, toolStrip.DefaultMinimumSize);
            Assert.Equal(new Padding(0, 0, 1, 0), toolStrip.DefaultPadding);
            Assert.Equal(new Size(100, 25), toolStrip.DefaultSize);
            Assert.True(toolStrip.DefaultShowItemToolTips);
            Assert.False(toolStrip.DesignMode);
            Assert.True(toolStrip.DisplayRectangle.X > 0);
            Assert.Equal(0, toolStrip.DisplayRectangle.Y);
            Assert.True(toolStrip.DisplayRectangle.Width > 0);
            Assert.Equal(25, toolStrip.DisplayRectangle.Height);
            Assert.Equal(DockStyle.Top, toolStrip.Dock);
            Assert.NotNull(toolStrip.DockPadding);
            Assert.Same(toolStrip.DockPadding, toolStrip.DockPadding);
            Assert.Equal(0, toolStrip.DockPadding.Top);
            Assert.Equal(0, toolStrip.DockPadding.Bottom);
            Assert.Equal(0, toolStrip.DockPadding.Left);
            Assert.Equal(1, toolStrip.DockPadding.Right);
            Assert.True(toolStrip.Enabled);
            Assert.NotNull(toolStrip.Events);
            Assert.Same(toolStrip.Events, toolStrip.Events);
            Assert.Equal(Control.DefaultFont, toolStrip.Font);
            Assert.Equal(Control.DefaultForeColor, toolStrip.ForeColor);
            Assert.Equal(ToolStripGripStyle.Visible, toolStrip.GripStyle);
            Assert.Equal(ToolStripGripDisplayStyle.Vertical, toolStrip.GripDisplayStyle);
            Assert.Equal(new Padding(2, 2, 2, 2), toolStrip.GripMargin);
            Assert.Equal(2, toolStrip.GripRectangle.X);
            Assert.Equal(0, toolStrip.GripRectangle.Y);
            Assert.True(toolStrip.GripRectangle.Width > 0);
            Assert.Equal(25, toolStrip.GripRectangle.Height);
            Assert.False(toolStrip.HasChildren);
            Assert.Equal(25, toolStrip.Height);
            Assert.NotNull(toolStrip.HorizontalScroll);
            Assert.Same(toolStrip.HorizontalScroll, toolStrip.HorizontalScroll);
            Assert.False(toolStrip.HScroll);
            Assert.Null(toolStrip.ImageList);
            Assert.Equal(new Size(16, 16), toolStrip.ImageScalingSize);
            Assert.Equal(ImeMode.NoControl, toolStrip.ImeMode);
            Assert.Equal(ImeMode.NoControl, toolStrip.ImeModeBase);
            Assert.False(toolStrip.IsCurrentlyDragging);
            Assert.False(toolStrip.IsDropDown);
            Assert.Empty(toolStrip.Items);
            Assert.Same(toolStrip.Items, toolStrip.Items);
            Assert.NotNull(toolStrip.LayoutEngine);
            Assert.Same(toolStrip.LayoutEngine, toolStrip.LayoutEngine);
            Assert.Null(toolStrip.LayoutSettings);
            Assert.Equal(ToolStripLayoutStyle.HorizontalStackWithOverflow, toolStrip.LayoutStyle);
            Assert.Equal(0, toolStrip.Left);
            Assert.Equal(Point.Empty, toolStrip.Location);
            Assert.Equal(Padding.Empty, toolStrip.Margin);
            Assert.True(toolStrip.MaxItemSize.Width > 0);
            Assert.Equal(25, toolStrip.MaxItemSize.Height);
            Assert.False(toolStrip.MenuAutoExpand);
            Assert.Equal(Orientation.Horizontal, toolStrip.Orientation);
            Assert.NotNull(toolStrip.OverflowButton);
            Assert.Same(toolStrip.OverflowButton, toolStrip.OverflowButton);
            Assert.Equal(new Padding(0, 0, 1, 0), toolStrip.Padding);
            Assert.NotNull(toolStrip.Renderer);
            Assert.Same(toolStrip.Renderer, toolStrip.Renderer);
            Assert.Equal(ToolStripRenderMode.ManagerRenderMode, toolStrip.RenderMode);
            Assert.Equal(100, toolStrip.Right);
            Assert.Equal(RightToLeft.No, toolStrip.RightToLeft);
            Assert.True(toolStrip.ShowItemToolTips);
            Assert.Null(toolStrip.Site);
            Assert.Equal(new Size(100, 25), toolStrip.Size);
            Assert.False(toolStrip.Stretch);
            Assert.Equal(0, toolStrip.TabIndex);
            Assert.False(toolStrip.TabStop);
            Assert.Empty(toolStrip.Text);
            Assert.Equal(ToolStripTextDirection.Horizontal, toolStrip.TextDirection);
            Assert.Equal(0, toolStrip.Top);
            Assert.NotNull(toolStrip.VerticalScroll);
            Assert.Same(toolStrip.VerticalScroll, toolStrip.VerticalScroll);
            Assert.True(toolStrip.Visible);
            Assert.False(toolStrip.VScroll);
            Assert.Equal(100, toolStrip.Width);
            Assert.True(ToolStripManager.ToolStrips.Contains(toolStrip));
        }

        public static IEnumerable<object[]> Ctor_ToolStripItemArray_TestData()
        {
            yield return new object[] { new ToolStripItem[0] };
            yield return new object[] { new ToolStripItem[] { new SubToolStripItem() } };
        }

        [Theory]
        [MemberData(nameof(Ctor_ToolStripItemArray_TestData))]
        public void ToolStrip_CtorToolStripItemArray(ToolStripItem[] items)
        {
            var toolStrip = new SubToolStrip(items);
            Assert.False(toolStrip.AllowItemReorder);
            Assert.True(toolStrip.AllowMerge);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, toolStrip.Anchor);
            Assert.False(toolStrip.AutoScroll);
            Assert.Equal(Size.Empty, toolStrip.AutoScrollMargin);
            Assert.Equal(Size.Empty, toolStrip.AutoScrollMinSize);
            Assert.Equal(Point.Empty, toolStrip.AutoScrollPosition);
            Assert.True(toolStrip.AutoSize);
            Assert.Equal(Control.DefaultBackColor, toolStrip.BackColor);
            Assert.Null(toolStrip.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, toolStrip.BackgroundImageLayout);
            Assert.Null(toolStrip.BindingContext);
            Assert.Equal(25, toolStrip.Bottom);
            Assert.Equal(new Rectangle(0, 0, 100, 25), toolStrip.Bounds);
            Assert.True(toolStrip.CanOverflow);
            Assert.True(toolStrip.CanRaiseEvents);
            Assert.False(toolStrip.CausesValidation);
            Assert.Equal(new Rectangle(0, 0, 100, 25), toolStrip.ClientRectangle);
            Assert.Equal(new Size(100, 25), toolStrip.ClientSize);
            Assert.False(toolStrip.Created);
            Assert.Null(toolStrip.Container);
            Assert.Empty(toolStrip.Controls);
            Assert.Same(toolStrip.Controls, toolStrip.Controls);
            Assert.Same(Cursors.Default, toolStrip.Cursor);
            Assert.Same(Cursors.Default, toolStrip.DefaultCursor);
            Assert.Equal(DockStyle.Top, toolStrip.DefaultDock);
            Assert.Equal(ToolStripDropDownDirection.BelowRight, toolStrip.DefaultDropDownDirection);
            Assert.Equal(ImeMode.Inherit, toolStrip.DefaultImeMode);
            Assert.Equal(new Padding(2, 2, 2, 2), toolStrip.DefaultGripMargin);
            Assert.Equal(Padding.Empty, toolStrip.DefaultMargin);
            Assert.Equal(Size.Empty, toolStrip.DefaultMaximumSize);
            Assert.Equal(Size.Empty, toolStrip.DefaultMinimumSize);
            Assert.Equal(new Padding(0, 0, 1, 0), toolStrip.DefaultPadding);
            Assert.Equal(new Size(100, 25), toolStrip.DefaultSize);
            Assert.True(toolStrip.DefaultShowItemToolTips);
            Assert.False(toolStrip.DesignMode);
            Assert.True(toolStrip.DisplayRectangle.X > 0);
            Assert.Equal(0, toolStrip.DisplayRectangle.Y);
            Assert.True(toolStrip.DisplayRectangle.Width > 0);
            Assert.Equal(25, toolStrip.DisplayRectangle.Height);
            Assert.Equal(DockStyle.Top, toolStrip.Dock);
            Assert.NotNull(toolStrip.DockPadding);
            Assert.Same(toolStrip.DockPadding, toolStrip.DockPadding);
            Assert.Equal(0, toolStrip.DockPadding.Top);
            Assert.Equal(0, toolStrip.DockPadding.Bottom);
            Assert.Equal(0, toolStrip.DockPadding.Left);
            Assert.Equal(1, toolStrip.DockPadding.Right);
            Assert.True(toolStrip.Enabled);
            Assert.NotNull(toolStrip.Events);
            Assert.Same(toolStrip.Events, toolStrip.Events);
            Assert.Equal(Control.DefaultFont, toolStrip.Font);
            Assert.Equal(Control.DefaultForeColor, toolStrip.ForeColor);
            Assert.Equal(ToolStripGripStyle.Visible, toolStrip.GripStyle);
            Assert.Equal(ToolStripGripDisplayStyle.Vertical, toolStrip.GripDisplayStyle);
            Assert.Equal(new Padding(2, 2, 2, 2), toolStrip.GripMargin);
            Assert.Equal(2, toolStrip.GripRectangle.X);
            Assert.Equal(0, toolStrip.GripRectangle.Y);
            Assert.True(toolStrip.GripRectangle.Width > 0);
            Assert.Equal(25, toolStrip.GripRectangle.Height);
            Assert.False(toolStrip.HasChildren);
            Assert.Equal(25, toolStrip.Height);
            Assert.NotNull(toolStrip.HorizontalScroll);
            Assert.Same(toolStrip.HorizontalScroll, toolStrip.HorizontalScroll);
            Assert.False(toolStrip.HScroll);
            Assert.Null(toolStrip.ImageList);
            Assert.Equal(new Size(16, 16), toolStrip.ImageScalingSize);
            Assert.Equal(ImeMode.NoControl, toolStrip.ImeMode);
            Assert.Equal(ImeMode.NoControl, toolStrip.ImeModeBase);
            Assert.False(toolStrip.IsCurrentlyDragging);
            Assert.False(toolStrip.IsDropDown);
            Assert.NotSame(items, toolStrip.Items);
            Assert.Same(toolStrip.Items, toolStrip.Items);
            Assert.Equal(items, toolStrip.Items.Cast<ToolStripItem>());
            Assert.NotNull(toolStrip.LayoutEngine);
            Assert.Same(toolStrip.LayoutEngine, toolStrip.LayoutEngine);
            Assert.Null(toolStrip.LayoutSettings);
            Assert.Equal(ToolStripLayoutStyle.HorizontalStackWithOverflow, toolStrip.LayoutStyle);
            Assert.Equal(0, toolStrip.Left);
            Assert.Equal(Point.Empty, toolStrip.Location);
            Assert.Equal(Padding.Empty, toolStrip.Margin);
            Assert.True(toolStrip.MaxItemSize.Width > 0);
            Assert.Equal(25, toolStrip.MaxItemSize.Height);
            Assert.False(toolStrip.MenuAutoExpand);
            Assert.Equal(Orientation.Horizontal, toolStrip.Orientation);
            Assert.NotNull(toolStrip.OverflowButton);
            Assert.Same(toolStrip.OverflowButton, toolStrip.OverflowButton);
            Assert.Equal(new Padding(0, 0, 1, 0), toolStrip.Padding);
            Assert.NotNull(toolStrip.Renderer);
            Assert.Same(toolStrip.Renderer, toolStrip.Renderer);
            Assert.Equal(ToolStripRenderMode.ManagerRenderMode, toolStrip.RenderMode);
            Assert.Equal(100, toolStrip.Right);
            Assert.Equal(RightToLeft.No, toolStrip.RightToLeft);
            Assert.True(toolStrip.ShowItemToolTips);
            Assert.Null(toolStrip.Site);
            Assert.Equal(new Size(100, 25), toolStrip.Size);
            Assert.False(toolStrip.Stretch);
            Assert.Equal(0, toolStrip.TabIndex);
            Assert.False(toolStrip.TabStop);
            Assert.Empty(toolStrip.Text);
            Assert.Equal(ToolStripTextDirection.Horizontal, toolStrip.TextDirection);
            Assert.Equal(0, toolStrip.Top);
            Assert.NotNull(toolStrip.VerticalScroll);
            Assert.Same(toolStrip.VerticalScroll, toolStrip.VerticalScroll);
            Assert.True(toolStrip.Visible);
            Assert.False(toolStrip.VScroll);
            Assert.Equal(100, toolStrip.Width);
            Assert.True(ToolStripManager.ToolStrips.Contains(toolStrip));
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

            public new EventHandlerList Events => base.Events;

            public new ImeMode ImeModeBase => base.ImeModeBase;

            public new bool HScroll
            {
                get => base.HScroll;
                set => base.HScroll = value;
            }

            public new Size MaxItemSize => base.MaxItemSize;

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
