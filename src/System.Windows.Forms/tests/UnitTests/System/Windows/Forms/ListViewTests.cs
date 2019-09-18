// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ListViewTests
    {
        [Fact]
        public void ListView_Ctor_Default()
        {
            var listView = new SubListView();
            Assert.Equal(ItemActivation.Standard, listView.Activation);
            Assert.Equal(ListViewAlignment.Top, listView.Alignment);
            Assert.False(listView.AllowColumnReorder);
            Assert.False(listView.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, listView.Anchor);
            Assert.True(listView.AutoArrange);
            Assert.False(listView.AutoSize);
            Assert.Equal(SystemColors.Window, listView.BackColor);
            Assert.Null(listView.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, listView.BackgroundImageLayout);
            Assert.False(listView.BackgroundImageTiled);
            Assert.Null(listView.BindingContext);
            Assert.Equal(BorderStyle.Fixed3D, listView.BorderStyle);
            Assert.Equal(97, listView.Bottom);
            Assert.Equal(new Rectangle(0, 0, 121, 97), listView.Bounds);
            Assert.True(listView.CanEnableIme);
            Assert.True(listView.CanRaiseEvents);
            Assert.True(listView.CausesValidation);
            Assert.False(listView.CheckBoxes);
            Assert.Empty(listView.CheckedIndices);
            Assert.Same(listView.CheckedIndices, listView.CheckedIndices);
            Assert.Empty(listView.CheckedItems);
            Assert.Same(listView.CheckedItems, listView.CheckedItems);
            Assert.Equal(new Size(117, 93), listView.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 117, 93), listView.ClientRectangle);
            Assert.Empty(listView.Columns);
            Assert.Same(listView.Columns, listView.Columns);
            Assert.Null(listView.Container);
            Assert.Null(listView.ContextMenu);
            Assert.Null(listView.ContextMenuStrip);
            Assert.Empty(listView.Controls);
            Assert.Same(listView.Controls, listView.Controls);
            Assert.False(listView.Created);
            Assert.Equal(Cursors.Default, listView.Cursor);
            Assert.Same(Cursors.Default, listView.DefaultCursor);
            Assert.Equal(ImeMode.Inherit, listView.DefaultImeMode);
            Assert.Equal(new Padding(3), listView.DefaultMargin);
            Assert.Equal(Size.Empty, listView.DefaultMaximumSize);
            Assert.Equal(Size.Empty, listView.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, listView.DefaultPadding);
            Assert.Equal(new Size(121, 97), listView.DefaultSize);
            Assert.False(listView.DesignMode);
            Assert.Equal(new Rectangle(0, 0, 117, 93), listView.DisplayRectangle);
            Assert.Equal(DockStyle.None, listView.Dock);
            Assert.False(listView.DoubleBuffered);
            Assert.True(listView.Enabled);
            Assert.NotNull(listView.Events);
            Assert.Same(listView.Events, listView.Events);
            Assert.Equal(Control.DefaultFont, listView.Font);
            Assert.Equal(listView.Font.Height, listView.FontHeight);
            Assert.Equal(SystemColors.WindowText, listView.ForeColor);
            Assert.False(listView.FullRowSelect);
            Assert.False(listView.GridLines);
            Assert.Empty(listView.Groups);
            Assert.Same(listView.Groups, listView.Groups);
            Assert.False(listView.HasChildren);
            Assert.Equal(ColumnHeaderStyle.Clickable, listView.HeaderStyle);
            Assert.Equal(97, listView.Height);
            Assert.False(listView.HideSelection);
            Assert.False(listView.HotTracking);
            Assert.False(listView.HoverSelection);
            Assert.Equal(ImeMode.NoControl, listView.ImeMode);
            Assert.Equal(ImeMode.NoControl, listView.ImeModeBase);
            Assert.NotNull(listView.InsertionMark);
            Assert.Same(listView.InsertionMark, listView.InsertionMark);
            Assert.Empty(listView.Items);
            Assert.Same(listView.Items, listView.Items);
            Assert.False(listView.LabelEdit);
            Assert.True(listView.LabelWrap);
            Assert.Null(listView.LargeImageList);
            Assert.Equal(0, listView.Left);
            Assert.Null(listView.ListViewItemSorter);
            Assert.Equal(Point.Empty, listView.Location);
            Assert.Equal(new Padding(3), listView.Margin);
            Assert.True(listView.MultiSelect);
            Assert.False(listView.OwnerDraw);
            Assert.Equal(Padding.Empty, listView.Padding);
            Assert.Null(listView.Parent);
            Assert.Equal("Microsoft\u00AE .NET", listView.ProductName);
            Assert.False(listView.RecreatingHandle);
            Assert.Null(listView.Region);
            Assert.False(listView.ResizeRedraw);
            Assert.Equal(121, listView.Right);
            Assert.Equal(RightToLeft.No, listView.RightToLeft);
            Assert.False(listView.RightToLeftLayout);
            Assert.True(listView.Scrollable);
            Assert.Empty(listView.SelectedIndices);
            Assert.Same(listView.SelectedIndices, listView.SelectedIndices);
            Assert.Empty(listView.SelectedItems);
            Assert.Same(listView.SelectedItems, listView.SelectedItems);
            Assert.True(listView.ShowGroups);
            Assert.False(listView.ShowItemToolTips);
            Assert.Null(listView.SmallImageList);
            Assert.Null(listView.Site);
            Assert.Equal(new Size(121, 97), listView.Size);
            Assert.Equal(SortOrder.None, listView.Sorting);
            Assert.Null(listView.StateImageList);
            Assert.Equal(0, listView.TabIndex);
            Assert.True(listView.TabStop);
            Assert.Empty(listView.Text);
            Assert.Equal(Size.Empty, listView.TileSize);
            Assert.Equal(0, listView.Top);
            Assert.Throws<InvalidOperationException>(() => listView.TopItem);
            Assert.True(listView.UseCompatibleStateImageBehavior);
            Assert.Equal(View.LargeIcon, listView.View);
            Assert.Equal(0, listView.VirtualListSize);
            Assert.False(listView.VirtualMode);
            Assert.True(listView.Visible);
            Assert.Equal(121, listView.Width);

            Assert.False(listView.IsHandleCreated);
        }

        [Fact]
        public void ListView_CreateParams_GetDefault_ReturnsExpected()
        {
            var control = new SubListView();
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("SysListView32", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x200, createParams.ExStyle);
            Assert.Equal(97, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x56010148, createParams.Style);
            Assert.Equal(121, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ItemActivation))]
        public void ListView_Activation_Set_GetReturnsExpected(ItemActivation value)
        {
            var listView = new ListView
            {
                Activation = value
            };
            Assert.Equal(value, listView.Activation);

            // Set same.
            listView.Activation = value;
            Assert.Equal(value, listView.Activation);
        }

        [Theory]
        [InlineData(ItemActivation.Standard, 0)]
        [InlineData(ItemActivation.OneClick, 1)]
        [InlineData(ItemActivation.TwoClick, 1)]
        public void ListView_Activation_SetWithHandle_GetReturnsExpected(ItemActivation value, int expectedInvalidatedCallCount)
        {
            var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;

            listView.Activation = value;
            Assert.Equal(value, listView.Activation);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);

            // Set same.
            listView.Activation = value;
            Assert.Equal(value, listView.Activation);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        }

        [Fact]
        public void ListView_Activation_SetHotTrackingOneClick_Nop()
        {
            var listView = new ListView
            {
                HotTracking = true,
                Activation = ItemActivation.OneClick
            };
            Assert.Equal(ItemActivation.OneClick, listView.Activation);

            // Set same.
            listView.Activation = ItemActivation.OneClick;
            Assert.Equal(ItemActivation.OneClick, listView.Activation);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ItemActivation))]
        public void ListView_Activation_SetInvalidValue_ThrowsInvalidEnumArgumentException(ItemActivation value)
        {
            var listView = new ListView();
            Assert.Throws<InvalidEnumArgumentException>("value", () => listView.Activation = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ItemActivation))]
        public void ListView_Activation_SetHotTrackingInvalidValue_ThrowsInvalidEnumArgumentException(ItemActivation value)
        {
            var listView = new ListView
            {
                HotTracking = true
            };
            Assert.Throws<InvalidEnumArgumentException>("value", () => listView.Activation = value);
        }

        [Theory]
        [InlineData(ItemActivation.Standard)]
        [InlineData(ItemActivation.TwoClick)]
        public void ListView_Activation_SetHotTrackingNotOneClick_ThrowsArgumentException(ItemActivation value)
        {
            var listView = new ListView
            {
                HotTracking = true
            };
            Assert.Throws<ArgumentException>("value", () => listView.Activation = value);
            Assert.Equal(ItemActivation.OneClick, listView.Activation);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_DoubleBuffered_Get_ReturnsExpected(bool value)
        {
            var control = new SubListView();
            control.SetStyle(ControlStyles.OptimizedDoubleBuffer, value);
            Assert.Equal(value, control.DoubleBuffered);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_DoubleBuffered_Set_GetReturnsExpected(bool value)
        {
            var control = new SubListView
            {
                DoubleBuffered = value
            };
            Assert.Equal(value, control.DoubleBuffered);
            Assert.Equal(value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));

            // Set same.
            control.DoubleBuffered = value;
            Assert.Equal(value, control.DoubleBuffered);
            Assert.Equal(value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));

            // Set different.
            control.DoubleBuffered = !value;
            Assert.Equal(!value, control.DoubleBuffered);
            Assert.Equal(!value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_HotTracking_Set_GetReturnsExpected(bool value)
        {
            var listView = new ListView
            {
                HotTracking = value
            };
            Assert.Equal(value, listView.HotTracking);

            // Set same.
            listView.HotTracking = value;
            Assert.Equal(value, listView.HotTracking);

            // Set different.
            listView.HotTracking = !value;
            Assert.Equal(!value, listView.HotTracking);
        }

        [Theory]
        [InlineData(true, 3, 4)]
        [InlineData(false, 0, 3)]
        public void ListView_HotTracking_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount1, int expectedInvalidatedCallCount2)
        {
            var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;

            listView.HotTracking = value;
            Assert.Equal(value, listView.HotTracking);
            Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);

            // Set same.
            listView.HotTracking = value;
            Assert.Equal(value, listView.HotTracking);
            Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);

            // Set different.
            listView.HotTracking = !value;
            Assert.Equal(!value, listView.HotTracking);
            Assert.Equal(expectedInvalidatedCallCount2, invalidatedCallCount);
        }

        [Fact]
        public void ListView_HotTracking_Set_SetsHoverSelectionAndActivationIfTrue()
        {
            var listView = new ListView
            {
                HotTracking = true
            };
            Assert.True(listView.HotTracking);
            Assert.True(listView.HoverSelection);
            Assert.Equal(ItemActivation.OneClick, listView.Activation);

            // Set same.
            listView.HotTracking = true;
            Assert.True(listView.HotTracking);
            Assert.True(listView.HoverSelection);
            Assert.Equal(ItemActivation.OneClick, listView.Activation);

            // Set different.
            listView.HotTracking = false;
            Assert.False(listView.HotTracking);
            Assert.True(listView.HoverSelection);
            Assert.Equal(ItemActivation.OneClick, listView.Activation);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_HoverSelection_Set_GetReturnsExpected(bool value)
        {
            var listView = new ListView
            {
                HoverSelection = value
            };
            Assert.Equal(value, listView.HoverSelection);

            // Set same.
            listView.HoverSelection = value;
            Assert.Equal(value, listView.HoverSelection);

            // Set different.
            listView.HoverSelection = !value;
            Assert.Equal(!value, listView.HoverSelection);
        }

        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void ListView_HoverSelection_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount)
        {
            var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;

            listView.HoverSelection = value;
            Assert.Equal(value, listView.HoverSelection);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);

            // Set same.
            listView.HoverSelection = value;
            Assert.Equal(value, listView.HoverSelection);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        }

        [Fact]
        public void ListView_HoverSelection_SetHotTrackingTrue_Nop()
        {
            var listView = new ListView
            {
                HotTracking = true,
                HoverSelection = true
            };
            Assert.True(listView.HoverSelection);

            // Set same.
            listView.HoverSelection = true;
            Assert.True(listView.HoverSelection);
        }

        [Fact]
        public void ListView_HoverSelection_SetHotTrackingFalse_ThrowsArgumentException()
        {
            var listView = new ListView
            {
                HotTracking = true
            };
            Assert.Throws<ArgumentException>("value", () => listView.HoverSelection = false);
            Assert.True(listView.HoverSelection);
        }

        private class SubListView : ListView
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

            public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

            public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);
        }
    }
}
