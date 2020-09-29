// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Microsoft.DotNet.RemoteExecutor;
using WinForms.Common.Tests;
using Xunit;
using static System.Windows.Forms.ListViewItem;
using static Interop;
using static Interop.ComCtl32;

namespace System.Windows.Forms.Tests
{
    using Point = System.Drawing.Point;
    using Size = System.Drawing.Size;

    public class ListViewTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ListView_Ctor_Default()
        {
            using var control = new SubListView();
            Assert.Null(control.AccessibleDefaultActionDescription);
            Assert.Null(control.AccessibleDescription);
            Assert.Null(control.AccessibleName);
            Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
            Assert.Equal(ItemActivation.Standard, control.Activation);
            Assert.Equal(ListViewAlignment.Top, control.Alignment);
            Assert.False(control.AllowColumnReorder);
            Assert.False(control.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.True(control.AutoArrange);
            Assert.False(control.AutoSize);
            Assert.Equal(SystemColors.Window, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.False(control.BackgroundImageTiled);
            Assert.Null(control.BindingContext);
            Assert.Equal(BorderStyle.Fixed3D, control.BorderStyle);
            Assert.Equal(97, control.Bottom);
            Assert.Equal(new Rectangle(0, 0, 121, 97), control.Bounds);
            Assert.True(control.CanEnableIme);
            Assert.False(control.CanFocus);
            Assert.True(control.CanRaiseEvents);
            Assert.True(control.CanSelect);
            Assert.False(control.Capture);
            Assert.True(control.CausesValidation);
            Assert.False(control.CheckBoxes);
            Assert.Empty(control.CheckedIndices);
            Assert.Same(control.CheckedIndices, control.CheckedIndices);
            Assert.Empty(control.CheckedItems);
            Assert.Same(control.CheckedItems, control.CheckedItems);
            Assert.Equal(new Size(117, 93), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 117, 93), control.ClientRectangle);
            Assert.Empty(control.Columns);
            Assert.Same(control.Columns, control.Columns);
            Assert.Null(control.Container);
            Assert.False(control.ContainsFocus);
            Assert.Null(control.ContextMenuStrip);
            Assert.Empty(control.Controls);
            Assert.Same(control.Controls, control.Controls);
            Assert.False(control.Created);
            Assert.Equal(Cursors.Default, control.Cursor);
            Assert.Same(Cursors.Default, control.DefaultCursor);
            Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
            Assert.Equal(new Padding(3), control.DefaultMargin);
            Assert.Equal(Size.Empty, control.DefaultMaximumSize);
            Assert.Equal(Size.Empty, control.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, control.DefaultPadding);
            Assert.Equal(new Size(121, 97), control.DefaultSize);
            Assert.False(control.DesignMode);
            Assert.Equal(new Rectangle(0, 0, 117, 93), control.DisplayRectangle);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.False(control.DoubleBuffered);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.False(control.Focused);
            Assert.Null(control.FocusedItem);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(SystemColors.WindowText, control.ForeColor);
            Assert.False(control.FullRowSelect);
            Assert.False(control.GridLines);
            Assert.Empty(control.Groups);
            Assert.Same(control.Groups, control.Groups);
            Assert.Null(control.GroupImageList);
            Assert.False(control.HasChildren);
            Assert.Equal(ColumnHeaderStyle.Clickable, control.HeaderStyle);
            Assert.Equal(97, control.Height);
            Assert.False(control.HideSelection);
            Assert.False(control.HotTracking);
            Assert.False(control.HoverSelection);
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.NotNull(control.InsertionMark);
            Assert.Same(control.InsertionMark, control.InsertionMark);
            Assert.False(control.IsAccessible);
            Assert.False(control.IsMirrored);
            Assert.Empty(control.Items);
            Assert.Same(control.Items, control.Items);
            Assert.False(control.LabelEdit);
            Assert.True(control.LabelWrap);
            Assert.Null(control.LargeImageList);
            Assert.NotNull(control.LayoutEngine);
            Assert.Same(control.LayoutEngine, control.LayoutEngine);
            Assert.Equal(0, control.Left);
            Assert.Null(control.ListViewItemSorter);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Equal(new Padding(3), control.Margin);
            Assert.Equal(Size.Empty, control.MaximumSize);
            Assert.Equal(Size.Empty, control.MinimumSize);
            Assert.True(control.MultiSelect);
            Assert.False(control.OwnerDraw);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Null(control.Parent);
            Assert.Equal(new Size(121, 97), control.PreferredSize);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.False(control.RecreatingHandle);
            Assert.Null(control.Region);
            Assert.False(control.ResizeRedraw);
            Assert.Equal(121, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.False(control.RightToLeftLayout);
            Assert.True(control.Scrollable);
            Assert.Empty(control.SelectedIndices);
            Assert.Same(control.SelectedIndices, control.SelectedIndices);
            Assert.Empty(control.SelectedItems);
            Assert.Same(control.SelectedItems, control.SelectedItems);
            Assert.True(control.ShowFocusCues);
            Assert.True(control.ShowGroups);
            Assert.False(control.ShowItemToolTips);
            Assert.True(control.ShowKeyboardCues);
            Assert.Null(control.SmallImageList);
            Assert.Null(control.Site);
            Assert.Equal(new Size(121, 97), control.Size);
            Assert.Equal(SortOrder.None, control.Sorting);
            Assert.Null(control.StateImageList);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(Size.Empty, control.TileSize);
            Assert.Equal(0, control.Top);
            Assert.Throws<InvalidOperationException>(() => control.TopItem);
            Assert.Null(control.TopLevelControl);
            Assert.True(control.UseCompatibleStateImageBehavior);
            Assert.False(control.UseWaitCursor);
            Assert.Equal(View.LargeIcon, control.View);
            Assert.Equal(0, control.VirtualListSize);
            Assert.False(control.VirtualMode);
            Assert.True(control.Visible);
            Assert.Equal(121, control.Width);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListView_CreateParams_GetDefault_ReturnsExpected()
        {
            using var control = new SubListView();
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("SysListView32", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x200, createParams.ExStyle);
            Assert.Equal(97, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            // LVS.SHAREIMAGELISTS is temporarily removed from style until ownership management is fixed
            // https://github.com/dotnet/winforms/issues/3531
            Assert.Equal(0x56010108, createParams.Style);
            Assert.Equal(121, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ItemActivation))]
        public void ListView_Activation_Set_GetReturnsExpected(ItemActivation value)
        {
            using var listView = new ListView
            {
                Activation = value
            };
            Assert.Equal(value, listView.Activation);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.Activation = value;
            Assert.Equal(value, listView.Activation);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(ItemActivation.Standard, 0)]
        [InlineData(ItemActivation.OneClick, 1)]
        [InlineData(ItemActivation.TwoClick, 1)]
        public void ListView_Activation_SetWithHandle_GetReturnsExpected(ItemActivation value, int expectedInvalidatedCallCount)
        {
            using var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.Activation = value;
            Assert.Equal(value, listView.Activation);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            listView.Activation = value;
            Assert.Equal(value, listView.Activation);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ListView_Activation_SetHotTrackingOneClick_Nop()
        {
            using var listView = new ListView
            {
                HotTracking = true,
                Activation = ItemActivation.OneClick
            };
            Assert.Equal(ItemActivation.OneClick, listView.Activation);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.Activation = ItemActivation.OneClick;
            Assert.Equal(ItemActivation.OneClick, listView.Activation);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ItemActivation))]
        public void ListView_Activation_SetInvalidValue_ThrowsInvalidEnumArgumentException(ItemActivation value)
        {
            using var listView = new ListView();
            Assert.Throws<InvalidEnumArgumentException>("value", () => listView.Activation = value);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ItemActivation))]
        public void ListView_Activation_SetHotTrackingInvalidValue_ThrowsInvalidEnumArgumentException(ItemActivation value)
        {
            using var listView = new ListView
            {
                HotTracking = true
            };
            Assert.Throws<InvalidEnumArgumentException>("value", () => listView.Activation = value);
        }

        [WinFormsTheory]
        [InlineData(ItemActivation.Standard)]
        [InlineData(ItemActivation.TwoClick)]
        public void ListView_Activation_SetHotTrackingNotOneClick_ThrowsArgumentException(ItemActivation value)
        {
            using var listView = new ListView
            {
                HotTracking = true
            };
            Assert.Throws<ArgumentException>("value", () => listView.Activation = value);
            Assert.Equal(ItemActivation.OneClick, listView.Activation);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ListViewAlignment))]
        public void ListView_Alignment_Set_GetReturnsExpected(ListViewAlignment value)
        {
            using var listView = new ListView
            {
                Alignment = value
            };
            Assert.Equal(value, listView.Alignment);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.Alignment = value;
            Assert.Equal(value, listView.Alignment);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(ListViewAlignment.Default, 2, 1)]
        [InlineData(ListViewAlignment.Top, 0, 0)]
        [InlineData(ListViewAlignment.Left, 2, 1)]
        [InlineData(ListViewAlignment.SnapToGrid, 2, 1)]
        public void ListView_Alignment_SetWithHandle_GetReturnsExpected(ListViewAlignment value, int expectedInvalidatedCallCount, int expectedCreatedCallCount)
        {
            using var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.Alignment = value;
            Assert.Equal(value, listView.Alignment);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set same.
            listView.Alignment = value;
            Assert.Equal(value, listView.Alignment);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ListViewAlignment))]
        public void ListView_Alignment_SetInvalidValue_ThrowsInvalidEnumArgumentException(ListViewAlignment value)
        {
            using var listView = new ListView();
            Assert.Throws<InvalidEnumArgumentException>("value", () => listView.Alignment = value);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_AllowColumnReorder_Set_GetReturnsExpected(bool value)
        {
            using var listView = new ListView
            {
                AllowColumnReorder = value
            };
            Assert.Equal(value, listView.AllowColumnReorder);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.AllowColumnReorder = value;
            Assert.Equal(value, listView.AllowColumnReorder);
            Assert.False(listView.IsHandleCreated);

            // Set different.
            listView.AllowColumnReorder = !value;
            Assert.Equal(!value, listView.AllowColumnReorder);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void ListView_AllowColumnReorder_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount)
        {
            using var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.AllowColumnReorder = value;
            Assert.Equal(value, listView.AllowColumnReorder);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            listView.AllowColumnReorder = value;
            Assert.Equal(value, listView.AllowColumnReorder);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            listView.AllowColumnReorder = !value;
            Assert.Equal(!value, listView.AllowColumnReorder);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_AutoArrange_Set_GetReturnsExpected(bool value)
        {
            using var listView = new ListView
            {
                AutoArrange = value
            };
            Assert.Equal(value, listView.AutoArrange);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.AutoArrange = value;
            Assert.Equal(value, listView.AutoArrange);
            Assert.False(listView.IsHandleCreated);

            // Set different.
            listView.AutoArrange = !value;
            Assert.Equal(!value, listView.AutoArrange);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 0)]
        [InlineData(false, 1)]
        public void ListView_AutoArrange_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount)
        {
            using var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.AutoArrange = value;
            Assert.Equal(value, listView.AutoArrange);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            listView.AutoArrange = value;
            Assert.Equal(value, listView.AutoArrange);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            listView.AutoArrange = !value;
            Assert.Equal(!value, listView.AutoArrange);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
            Assert.Equal(expectedInvalidatedCallCount + 1, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> BackColor_Set_TestData()
        {
            yield return new object[] { Color.Empty, SystemColors.Window };
            yield return new object[] { Color.Red, Color.Red };
        }

        [WinFormsTheory]
        [MemberData(nameof(BackColor_Set_TestData))]
        public void ListView_BackColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new ListView
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

        public static IEnumerable<object[]> BackColor_SetWithHandle_TestData()
        {
            yield return new object[] { Color.Empty, SystemColors.Window, 0 };
            yield return new object[] { Color.Red, Color.Red, 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(BackColor_SetWithHandle_TestData))]
        public void ListView_BackColor_SetWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
        {
            using var control = new ListView();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ListView_BackColor_GetBkColor_Success()
        {
            using var control = new ListView();

            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.BackColor = Color.FromArgb(0xFF, 0x12, 0x34, 0x56);
            Assert.Equal((IntPtr)0x563412, User32.SendMessageW(control.Handle, (User32.WM)LVM.GETBKCOLOR));
        }

        [WinFormsFact]
        public void ListView_BackColor_SetWithHandler_CallsBackColorChanged()
        {
            using var control = new ListView();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.BackColorChanged += handler;

            // Set different.
            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Equal(1, callCount);

            // Set same.
            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Equal(1, callCount);

            // Set different.
            control.BackColor = Color.Empty;
            Assert.Equal(SystemColors.Window, control.BackColor);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.BackColorChanged -= handler;
            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ImageLayout))]
        public void ListView_BackgroundImageLayout_Set_GetReturnsExpected(ImageLayout value)
        {
            using var control = new SubListView
            {
                BackgroundImageLayout = value
            };
            Assert.Equal(value, control.BackgroundImageLayout);
            Assert.False(control.DoubleBuffered);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.BackgroundImageLayout = value;
            Assert.Equal(value, control.BackgroundImageLayout);
            Assert.False(control.DoubleBuffered);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListView_BackgroundImageLayout_SetWithHandler_CallsBackgroundImageLayoutChanged()
        {
            using var control = new ListView();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.BackgroundImageLayoutChanged += handler;

            // Set different.
            control.BackgroundImageLayout = ImageLayout.Center;
            Assert.Equal(ImageLayout.Center, control.BackgroundImageLayout);
            Assert.Equal(1, callCount);

            // Set same.
            control.BackgroundImageLayout = ImageLayout.Center;
            Assert.Equal(ImageLayout.Center, control.BackgroundImageLayout);
            Assert.Equal(1, callCount);

            // Set different.
            control.BackgroundImageLayout = ImageLayout.Stretch;
            Assert.Equal(ImageLayout.Stretch, control.BackgroundImageLayout);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.BackgroundImageLayoutChanged -= handler;
            control.BackgroundImageLayout = ImageLayout.Center;
            Assert.Equal(ImageLayout.Center, control.BackgroundImageLayout);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ImageLayout))]
        public void ListView_BackgroundImageLayout_SetInvalid_ThrowsInvalidEnumArgumentException(ImageLayout value)
        {
            using var control = new ListView();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.BackgroundImageLayout = value);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_BackgroundImageTiled_Set_GetReturnsExpected(bool value)
        {
            using var listView = new ListView
            {
                BackgroundImageTiled = value
            };
            Assert.Equal(value, listView.BackgroundImageTiled);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.BackgroundImageTiled = value;
            Assert.Equal(value, listView.BackgroundImageTiled);
            Assert.False(listView.IsHandleCreated);

            // Set different.
            listView.BackgroundImageTiled = !value;
            Assert.Equal(!value, listView.BackgroundImageTiled);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_BackgroundImageTiled_SetWithBackgroundImage_GetReturnsExpected(bool value)
        {
            using var listView = new ListView
            {
                BackgroundImage = new Bitmap(10, 10),
                BackgroundImageTiled = value
            };
            Assert.Equal(value, listView.BackgroundImageTiled);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.BackgroundImageTiled = value;
            Assert.Equal(value, listView.BackgroundImageTiled);
            Assert.False(listView.IsHandleCreated);

            // Set different.
            listView.BackgroundImageTiled = !value;
            Assert.Equal(!value, listView.BackgroundImageTiled);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_BackgroundImageTiled_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.BackgroundImageTiled = value;
            Assert.Equal(value, listView.BackgroundImageTiled);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            listView.BackgroundImageTiled = value;
            Assert.Equal(value, listView.BackgroundImageTiled);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            listView.BackgroundImageTiled = !value;
            Assert.Equal(!value, listView.BackgroundImageTiled);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_BackgroundImageTiled_SetWithBackgroundImageWithHandle_GetReturnsExpected(bool value)
        {
            using var listView = new ListView
            {
                BackgroundImage = new Bitmap(10, 10)
            };
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.BackgroundImageTiled = value;
            Assert.Equal(value, listView.BackgroundImageTiled);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            listView.BackgroundImageTiled = value;
            Assert.Equal(value, listView.BackgroundImageTiled);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            listView.BackgroundImageTiled = !value;
            Assert.Equal(!value, listView.BackgroundImageTiled);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(BorderStyle))]
        public void ListView_BorderStyle_Set_GetReturnsExpected(BorderStyle value)
        {
            using var listView = new ListView
            {
                BorderStyle = value
            };
            Assert.Equal(value, listView.BorderStyle);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.BorderStyle = value;
            Assert.Equal(value, listView.BorderStyle);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(BorderStyle.Fixed3D, 0)]
        [InlineData(BorderStyle.FixedSingle, 1)]
        [InlineData(BorderStyle.None, 1)]
        public void ListView_BorderStyle_SetWithHandle_GetReturnsExpected(BorderStyle value, int expectedInvalidatedCallCount)
        {
            using var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.BorderStyle = value;
            Assert.Equal(value, listView.BorderStyle);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            listView.BorderStyle = value;
            Assert.Equal(value, listView.BorderStyle);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(BorderStyle))]
        public void ListView_BorderStyle_SetInvalid_ThrowsInvalidEnumArgumentException(BorderStyle value)
        {
            using var listView = new ListView();
            Assert.Throws<InvalidEnumArgumentException>("value", () => listView.BorderStyle = value);
        }

        public static IEnumerable<object[]> CheckBoxes_Set_TestData()
        {
            foreach (bool useCompatibleStateImageBehavior in new bool[] { true, false })
            {
                foreach (ListViewAlignment alignment in Enum.GetValues(typeof(ListViewAlignment)))
                {
                    foreach (Func<ImageList> imageListFactory in new Func<ImageList>[] { () => new ImageList(), () => null })
                    {
                        foreach (bool value in new bool[] { true, false })
                        {
                            yield return new object[] { useCompatibleStateImageBehavior, View.Details, alignment, imageListFactory(), value };
                            yield return new object[] { useCompatibleStateImageBehavior, View.LargeIcon, alignment, imageListFactory(), value };
                            yield return new object[] { useCompatibleStateImageBehavior, View.List, alignment, imageListFactory(), value };
                            yield return new object[] { useCompatibleStateImageBehavior, View.SmallIcon, alignment, imageListFactory(), value };
                        }
                    }
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(CheckBoxes_Set_TestData))]
        public void ListView_CheckBoxes_Set_GetReturnsExpected(bool useCompatibleStateImageBehavior, View view, ListViewAlignment alignment, ImageList stateImageList, bool value)
        {
            using var listView = new ListView
            {
                UseCompatibleStateImageBehavior = useCompatibleStateImageBehavior,
                View = view,
                Alignment = alignment,
                StateImageList = stateImageList,
                CheckBoxes = value
            };
            Assert.Equal(value, listView.CheckBoxes);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.CheckBoxes = value;
            Assert.Equal(value, listView.CheckBoxes);
            Assert.False(listView.IsHandleCreated);

            // Set different.
            listView.CheckBoxes = !value;
            Assert.Equal(!value, listView.CheckBoxes);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(CheckBoxes_Set_TestData))]
        public void ListView_CheckBoxes_SetAutoArrange_GetReturnsExpected(bool useCompatibleStateImageBehavior, View view, ListViewAlignment alignment, ImageList stateImageList, bool value)
        {
            using var listView = new ListView
            {
                AutoArrange = true,
                UseCompatibleStateImageBehavior = useCompatibleStateImageBehavior,
                View = view,
                Alignment = alignment,
                StateImageList = stateImageList,
                CheckBoxes = value
            };
            Assert.Equal(value, listView.CheckBoxes);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.CheckBoxes = value;
            Assert.Equal(value, listView.CheckBoxes);
            Assert.False(listView.IsHandleCreated);

            // Set different.
            listView.CheckBoxes = !value;
            Assert.Equal(!value, listView.CheckBoxes);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_CheckBoxes_SetWithCheckedItems_Success(bool value)
        {
            var item1 = new ListViewItem
            {
                Checked = true
            };
            var item2 = new ListViewItem();
            using var listView = new ListView();
            listView.Items.Add(item1);
            listView.Items.Add(item2);
            Assert.Equal(new ListViewItem[] { item1, item2 }, listView.Items.Cast<ListViewItem>());

            listView.CheckBoxes = value;
            Assert.Equal(value, listView.CheckBoxes);
            Assert.True(item1.Checked);
            Assert.False(item2.Checked);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.CheckBoxes = value;
            Assert.Equal(value, listView.CheckBoxes);
            Assert.True(item1.Checked);
            Assert.False(item2.Checked);
            Assert.False(listView.IsHandleCreated);

            // Set different.
            listView.CheckBoxes = !value;
            Assert.Equal(!value, listView.CheckBoxes);
            Assert.True(item1.Checked);
            Assert.False(item2.Checked);
            Assert.False(listView.IsHandleCreated);
        }

        public static IEnumerable<object[]> CheckBoxes_SetWithHandle_TestData()
        {
            foreach (ListViewAlignment alignment in Enum.GetValues(typeof(ListViewAlignment)))
            {
                foreach (Func<ImageList> imageListFactory in new Func<ImageList>[] { () => new ImageList(), () => null })
                {
                    yield return new object[] { true, View.Details, alignment, imageListFactory(), true, 1, 0, 2, 0 };
                    yield return new object[] { true, View.LargeIcon, alignment, imageListFactory(), true, 1, 0, 2, 0 };
                    yield return new object[] { true, View.List, alignment, imageListFactory(), true, 1, 0, 2, 0 };
                    yield return new object[] { true, View.SmallIcon, alignment, imageListFactory(), true, 1, 0, 2, 0 };
                    yield return new object[] { true, View.Details, alignment, imageListFactory(), false, 0, 0, 1, 0 };
                    yield return new object[] { true, View.LargeIcon, alignment, imageListFactory(), false, 0, 0, 1, 0 };
                    yield return new object[] { true, View.List, alignment, imageListFactory(), false, 0, 0, 1, 0 };
                    yield return new object[] { true, View.SmallIcon, alignment, imageListFactory(), false, 0, 0, 1, 0 };
                }

                if (alignment != ListViewAlignment.Left)
                {
                    yield return new object[] { false, View.Details, alignment, null, true, 1, 0, 2, 0 };
                    yield return new object[] { false, View.LargeIcon, alignment, null, true, 2, 1, 3, 1 };
                    yield return new object[] { false, View.List, alignment, null, true, 1, 1, 2, 1 };
                    yield return new object[] { false, View.SmallIcon, alignment, null, true, 2, 1, 3, 1 };
                    yield return new object[] { false, View.Details, alignment, null, false, 0, 0, 1, 0 };
                    yield return new object[] { false, View.LargeIcon, alignment, null, false, 0, 0, 2, 1 };
                    yield return new object[] { false, View.List, alignment, null, false, 0, 0, 1, 1 };
                    yield return new object[] { false, View.SmallIcon, alignment, null, false, 0, 0, 2, 1 };

                    yield return new object[] { false, View.Details, alignment, new ImageList(), true, 1, 0, 2, 1 };
                    yield return new object[] { false, View.LargeIcon, alignment, new ImageList(), true, 2, 1, 4, 2 };
                    yield return new object[] { false, View.List, alignment, new ImageList(), true, 1, 1, 2, 2 };
                    yield return new object[] { false, View.SmallIcon, alignment, new ImageList(), true, 2, 1, 4, 2 };
                    yield return new object[] { false, View.Details, alignment, new ImageList(), false, 0, 0, 1, 0 };
                    yield return new object[] { false, View.LargeIcon, alignment, new ImageList(), false, 0, 0, 2, 1 };
                    yield return new object[] { false, View.List, alignment, new ImageList(), false, 0, 0, 1, 1 };
                    yield return new object[] { false, View.SmallIcon, alignment, new ImageList(), false, 0, 0, 2, 1 };
                }
            }

            foreach (Func<ImageList> imageListFactory in new Func<ImageList>[] { () => new ImageList(), () => null })
            {
                yield return new object[] { false, View.Details, ListViewAlignment.Left, imageListFactory(), true, 1, 0, 2, 1 };
                yield return new object[] { false, View.LargeIcon, ListViewAlignment.Left, imageListFactory(), true, 2, 1, 4, 2 };
                yield return new object[] { false, View.List, ListViewAlignment.Left, imageListFactory(), true, 1, 1, 2, 2 };
                yield return new object[] { false, View.SmallIcon, ListViewAlignment.Left, imageListFactory(), true, 2, 1, 4, 2 };
                yield return new object[] { false, View.Details, ListViewAlignment.Left, imageListFactory(), false, 0, 0, 1, 0 };
                yield return new object[] { false, View.LargeIcon, ListViewAlignment.Left, imageListFactory(), false, 0, 0, 2, 1 };
                yield return new object[] { false, View.List, ListViewAlignment.Left, imageListFactory(), false, 0, 0, 1, 1 };
                yield return new object[] { false, View.SmallIcon, ListViewAlignment.Left, imageListFactory(), false, 0, 0, 2, 1 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(CheckBoxes_SetWithHandle_TestData))]
        public void ListView_CheckBoxes_SetWithHandle_GetReturnsExpected(bool useCompatibleStateImageBehavior, View view, ListViewAlignment alignment, ImageList stateImageList, bool value, int expectedInvalidatedCallCount1, int expectedCreatedCallCount1, int expectedInvalidatedCallCount2, int expectedCreatedCallCount2)
        {
            using var listView = new ListView
            {
                UseCompatibleStateImageBehavior = useCompatibleStateImageBehavior,
                View = view,
                Alignment = alignment,
                StateImageList = stateImageList
            };
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.CheckBoxes = value;
            Assert.Equal(value, listView.CheckBoxes);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount1, createdCallCount);

            // Set same.
            listView.CheckBoxes = value;
            Assert.Equal(value, listView.CheckBoxes);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount1, createdCallCount);

            // Set different.
            listView.CheckBoxes = !value;
            Assert.Equal(!value, listView.CheckBoxes);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount2, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(CheckBoxes_SetWithHandle_TestData))]
        public void ListView_CheckBoxes_SetAutoArrangeWithHandle_GetReturnsExpected(bool useCompatibleStateImageBehavior, View view, ListViewAlignment alignment, ImageList stateImageList, bool value, int expectedInvalidatedCallCount1, int expectedCreatedCallCount1, int expectedInvalidatedCallCount2, int expectedCreatedCallCount2)
        {
            using var listView = new ListView
            {
                AutoArrange = true,
                UseCompatibleStateImageBehavior = useCompatibleStateImageBehavior,
                View = view,
                Alignment = alignment,
                StateImageList = stateImageList
            };
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.CheckBoxes = value;
            Assert.Equal(value, listView.CheckBoxes);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount1, createdCallCount);

            // Set same.
            listView.CheckBoxes = value;
            Assert.Equal(value, listView.CheckBoxes);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount1, createdCallCount);

            // Set different.
            listView.CheckBoxes = !value;
            Assert.Equal(!value, listView.CheckBoxes);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount2, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_CheckBoxes_SetWithCheckedItemsWithHandle_Success(bool value)
        {
            var item1 = new ListViewItem
            {
                Checked = true
            };
            var item2 = new ListViewItem();
            using var listView = new ListView();
            listView.Items.Add(item1);
            listView.Items.Add(item2);
            Assert.Equal(new ListViewItem[] { item1, item2 }, listView.Items.Cast<ListViewItem>());
            Assert.NotEqual(IntPtr.Zero, listView.Handle);

            listView.CheckBoxes = value;
            Assert.Equal(value, listView.CheckBoxes);
            Assert.True(item1.Checked);
            Assert.False(item2.Checked);
            Assert.True(listView.IsHandleCreated);

            // Set same.
            listView.CheckBoxes = value;
            Assert.Equal(value, listView.CheckBoxes);
            Assert.True(item1.Checked);
            Assert.False(item2.Checked);
            Assert.True(listView.IsHandleCreated);

            // Set different.
            listView.CheckBoxes = !value;
            Assert.Equal(!value, listView.CheckBoxes);
            Assert.True(item1.Checked);
            Assert.False(item2.Checked);
            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_CheckBoxes_SetTile_ThrowsNotSupportedException(bool useCompatibleStateImageBehavior)
        {
            using var listView = new ListView
            {
                UseCompatibleStateImageBehavior = useCompatibleStateImageBehavior,
                View = View.Tile
            };
            Assert.Throws<NotSupportedException>(() => listView.CheckBoxes = true);
            Assert.False(listView.CheckBoxes);

            listView.CheckBoxes = false;
            Assert.False(listView.CheckBoxes);
        }

        [WinFormsFact]
        public void ListView_DisposeWithReferencedImageListDoesNotLeak()
        {
            // must be separate function because GC of local variables is not precise
            static WeakReference CreateAndDisposeListViewWithImageListReference(ImageList imageList)
            {
                // short lived test code, whatever you need to trigger the leak
                using var listView = new ListView();
                listView.LargeImageList = imageList;

                // return a weak reference to whatever you want to track GC of
                // creating a long weak reference to make sure finalizer does not resurrect the ListView
                return new WeakReference(listView, true);
            }

            // simulate a long-living ImageList by keeping it alive for the test
            using var imageList = new ImageList();

            // simulate a short-living ListView by disposing it (returning a WeakReference to track finalization)
            var listViewRef = CreateAndDisposeListViewWithImageListReference(imageList);

            GC.Collect(); // mark for finalization (also would clear normal weak references)
            GC.WaitForPendingFinalizers(); // wait until finalizer is executed
            GC.Collect(); // wait for long weak reference to be cleared

            // at this point the WeakReference is cleared if -and only if- the finalizer was called and did not resurrect the object
            // (if the test ever fails you can set a breakpoint here, debug the test, and make heap snapshot in VS;
            // then search for the ListView in the heap snapshot UI and look who is referencing it, usually you
            // can derive from types referencing the ListView who is to blame)
            Assert.False(listViewRef.IsAlive);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_DoubleBuffered_Get_ReturnsExpected(bool value)
        {
            using var control = new SubListView();
            control.SetStyle(ControlStyles.OptimizedDoubleBuffer, value);
            Assert.Equal(value, control.DoubleBuffered);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_DoubleBuffered_Set_GetReturnsExpected(bool value)
        {
            using var control = new SubListView
            {
                DoubleBuffered = value
            };
            Assert.Equal(value, control.DoubleBuffered);
            Assert.Equal(value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.DoubleBuffered = value;
            Assert.Equal(value, control.DoubleBuffered);
            Assert.Equal(value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.DoubleBuffered = !value;
            Assert.Equal(!value, control.DoubleBuffered);
            Assert.Equal(!value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void ListView_DoubleBuffered_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount)
        {
            using var control = new SubListView();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.DoubleBuffered = value;
            Assert.Equal(value, control.DoubleBuffered);
            Assert.Equal(value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.DoubleBuffered = value;
            Assert.Equal(value, control.DoubleBuffered);
            Assert.Equal(value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.DoubleBuffered = !value;
            Assert.Equal(!value, control.DoubleBuffered);
            Assert.Equal(!value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> FocusedItem_Set_TestData()
        {
            yield return new object[] { new ListViewItem(), false };
            yield return new object[] { null, null };
        }

        [WinFormsTheory]
        [MemberData(nameof(FocusedItem_Set_TestData))]
        public void ListView_FocusedItem_Set_GetReturnsExpected(ListViewItem value, bool? expectedFocused)
        {
            using var control = new SubListView
            {
                FocusedItem = value
            };
            Assert.Null(control.FocusedItem);
            Assert.False(control.IsHandleCreated);
            Assert.Equal(expectedFocused, value?.Focused);

            // Set same.
            control.FocusedItem = value;
            Assert.Null(control.FocusedItem);
            Assert.False(control.IsHandleCreated);
            Assert.Equal(expectedFocused, value?.Focused);
        }

        [WinFormsFact]
        public void ListView_FocusedItem_SetChild_GetReturnsExpected()
        {
            var value = new ListViewItem();
            using var control = new SubListView();
            control.Items.Add(value);

            control.FocusedItem = value;
            Assert.Null(control.FocusedItem);
            Assert.False(control.Focused);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.FocusedItem = value;
            Assert.Null(control.FocusedItem);
            Assert.False(control.Focused);
            Assert.False(control.IsHandleCreated);

            // Set null.
            control.FocusedItem = null;
            Assert.Null(control.FocusedItem);
            Assert.False(control.Focused);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(FocusedItem_Set_TestData))]
        public void ListView_FocusedItem_SetWithHandle_GetReturnsExpected(ListViewItem value, bool? expectedFocused)
        {
            using var control = new SubListView();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.FocusedItem = value;
            Assert.Null(control.FocusedItem);
            Assert.Equal(expectedFocused, value?.Focused);

            // Set same.
            control.FocusedItem = value;
            Assert.Null(control.FocusedItem);
            Assert.Equal(expectedFocused, value?.Focused);
        }

        [WinFormsFact]
        public void ListView_FocusedItem_SetChildWithHandle_GetReturnsExpected()
        {
            var value = new ListViewItem();
            using var control = new SubListView();
            control.Items.Add(value);
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.FocusedItem = value;
            Assert.Same(value, control.FocusedItem);
            Assert.True(value.Focused);

            // Set same.
            control.FocusedItem = value;
            Assert.Same(value, control.FocusedItem);
            Assert.True(value.Focused);

            // Set null.
            control.FocusedItem = null;
            Assert.Same(value, control.FocusedItem);
            Assert.True(value.Focused);
        }

        public static IEnumerable<object[]> ForeColor_Set_TestData()
        {
            yield return new object[] { Color.Empty, SystemColors.WindowText };
            yield return new object[] { Color.FromArgb(254, 1, 2, 3), Color.FromArgb(254, 1, 2, 3) };
            yield return new object[] { Color.White, Color.White };
            yield return new object[] { Color.Black, Color.Black };
            yield return new object[] { Color.Red, Color.Red };
        }

        [WinFormsTheory]
        [MemberData(nameof(ForeColor_Set_TestData))]
        public void ListView_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new ListView
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

        public static IEnumerable<object[]> ForeColor_SetWithHandle_TestData()
        {
            yield return new object[] { Color.Empty, SystemColors.WindowText, 0 };
            yield return new object[] { Color.FromArgb(254, 1, 2, 3), Color.FromArgb(254, 1, 2, 3), 1 };
            yield return new object[] { Color.White, Color.White, 1 };
            yield return new object[] { Color.Black, Color.Black, 1 };
            yield return new object[] { Color.Red, Color.Red, 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(ForeColor_SetWithHandle_TestData))]
        public void ListView_ForeColor_SetWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
        {
            using var control = new ListView();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ListView_ForeColor_GetTxtColor_Success()
        {
            using var control = new ListView();

            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.ForeColor = Color.FromArgb(0x12, 0x34, 0x56, 0x78);
            Assert.Equal((IntPtr)0x785634, User32.SendMessageW(control.Handle, (User32.WM)LVM.GETTEXTCOLOR));
        }

        [WinFormsFact]
        public void ListView_ForeColor_SetWithHandler_CallsForeColorChanged()
        {
            using var control = new ListView();
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
            Assert.Equal(SystemColors.WindowText, control.ForeColor);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.ForeColorChanged -= handler;
            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_FullRowSelect_Set_GetReturnsExpected(bool value)
        {
            using var listView = new ListView
            {
                FullRowSelect = value
            };
            Assert.Equal(value, listView.FullRowSelect);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.FullRowSelect = value;
            Assert.Equal(value, listView.FullRowSelect);
            Assert.False(listView.IsHandleCreated);

            // Set different.
            listView.FullRowSelect = !value;
            Assert.Equal(!value, listView.FullRowSelect);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void ListView_FullRowSelect_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount)
        {
            using var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.FullRowSelect = value;
            Assert.Equal(value, listView.FullRowSelect);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            listView.FullRowSelect = value;
            Assert.Equal(value, listView.FullRowSelect);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            listView.FullRowSelect = !value;
            Assert.Equal(!value, listView.FullRowSelect);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_GridLines_Set_GetReturnsExpected(bool value)
        {
            using var listView = new ListView
            {
                GridLines = value
            };
            Assert.Equal(value, listView.GridLines);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.GridLines = value;
            Assert.Equal(value, listView.GridLines);
            Assert.False(listView.IsHandleCreated);

            // Set different.
            listView.GridLines = !value;
            Assert.Equal(!value, listView.GridLines);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void ListView_GridLines_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount)
        {
            using var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.GridLines = value;
            Assert.Equal(value, listView.GridLines);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            listView.GridLines = value;
            Assert.Equal(value, listView.GridLines);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            listView.GridLines = !value;
            Assert.Equal(!value, listView.GridLines);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> GroupImageList_Set_GetReturnsExpected()
        {
            foreach (bool autoArrange in new bool[] { true, false })
            {
                foreach (bool virtualMode in new bool[] { true, false })
                {
                    foreach (View view in new View[] { View.Details, View.LargeIcon, View.List, View.SmallIcon })
                    {
                        yield return new object[] { autoArrange, virtualMode, view, null };
                        yield return new object[] { autoArrange, virtualMode, view, new ImageList() };
                        yield return new object[] { autoArrange, virtualMode, view, CreateNonEmpty() };
                    }
                }

                yield return new object[] { autoArrange, false, View.Tile, null };
                yield return new object[] { autoArrange, false, View.Tile, new ImageList() };
                yield return new object[] { autoArrange, false, View.Tile, CreateNonEmpty() };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(GroupImageList_Set_GetReturnsExpected))]
        public void ListView_GroupImageList_Set_GetReturnsExpected(bool autoArrange, bool virtualMode, View view, ImageList value)
        {
            using var listView = new ListView
            {
                AutoArrange = autoArrange,
                VirtualMode = virtualMode,
                View = view,
                GroupImageList = value
            };

            Assert.Same(value, listView.GroupImageList);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.GroupImageList = value;
            Assert.Same(value, listView.GroupImageList);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(GroupImageList_Set_GetReturnsExpected))]
        public void ListView_GroupImageList_SetWithNonNullOldValue_GetReturnsExpected(bool autoArrange, bool virtualMode, View view, ImageList value)
        {
            using var imageList = new ImageList();
            using var listView = new ListView
            {
                AutoArrange = autoArrange,
                VirtualMode = virtualMode,
                View = view,
                GroupImageList = imageList
            };

            listView.GroupImageList = value;
            Assert.Same(value, listView.GroupImageList);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.GroupImageList = value;
            Assert.Same(value, listView.GroupImageList);
            Assert.False(listView.IsHandleCreated);
        }

        public static IEnumerable<object[]> GroupImageList_SetWithHandle_GetReturnsExpected()
        {
            yield return new object[] { true, false, View.Details, null };
            yield return new object[] { true, false, View.Details, new ImageList() };
            yield return new object[] { true, false, View.Details, CreateNonEmpty() };
            yield return new object[] { true, false, View.LargeIcon, null };
            yield return new object[] { true, false, View.LargeIcon, new ImageList() };
            yield return new object[] { true, false, View.LargeIcon, CreateNonEmpty() };
            yield return new object[] { true, false, View.List, null };
            yield return new object[] { true, false, View.List, new ImageList() };
            yield return new object[] { true, false, View.List, CreateNonEmpty() };
            yield return new object[] { true, false, View.SmallIcon, null };
            yield return new object[] { true, false, View.SmallIcon, new ImageList() };
            yield return new object[] { true, false, View.SmallIcon, CreateNonEmpty() };
            yield return new object[] { true, false, View.Tile, null };
            yield return new object[] { true, false, View.Tile, new ImageList() };
            yield return new object[] { true, false, View.Tile, CreateNonEmpty() };

            foreach (bool autoArrange in new bool[] { true, false })
            {
                yield return new object[] { autoArrange, true, View.Details, null };
                yield return new object[] { autoArrange, true, View.Details, new ImageList() };
                yield return new object[] { autoArrange, true, View.Details, CreateNonEmpty() };
                yield return new object[] { autoArrange, true, View.LargeIcon, null };
                yield return new object[] { autoArrange, true, View.LargeIcon, new ImageList() };
                yield return new object[] { autoArrange, true, View.LargeIcon, CreateNonEmpty() };
                yield return new object[] { autoArrange, true, View.List, null };
                yield return new object[] { autoArrange, true, View.List, new ImageList() };
                yield return new object[] { autoArrange, true, View.List, CreateNonEmpty() };
                yield return new object[] { autoArrange, true, View.SmallIcon, null };
                yield return new object[] { autoArrange, true, View.SmallIcon, new ImageList() };
                yield return new object[] { autoArrange, true, View.SmallIcon, CreateNonEmpty() };
            }

            yield return new object[] { false, false, View.Details, null };
            yield return new object[] { false, false, View.Details, new ImageList() };
            yield return new object[] { false, false, View.Details, CreateNonEmpty() };
            yield return new object[] { false, false, View.LargeIcon, null };
            yield return new object[] { false, false, View.LargeIcon, new ImageList() };
            yield return new object[] { false, false, View.LargeIcon, CreateNonEmpty() };
            yield return new object[] { false, false, View.List, null };
            yield return new object[] { false, false, View.List, new ImageList() };
            yield return new object[] { false, false, View.List, CreateNonEmpty() };
            yield return new object[] { false, false, View.SmallIcon, null };
            yield return new object[] { false, false, View.SmallIcon, new ImageList() };
            yield return new object[] { false, false, View.SmallIcon, CreateNonEmpty() };
            yield return new object[] { false, false, View.Tile, null };
            yield return new object[] { false, false, View.Tile, new ImageList() };
            yield return new object[] { false, false, View.Tile, CreateNonEmpty() };
        }

        [WinFormsTheory]
        [MemberData(nameof(GroupImageList_SetWithHandle_GetReturnsExpected))]
        public void ListView_GroupImageList_SetWithHandle_GetReturnsExpected(bool autoArrange, bool virtualMode, View view, ImageList value)
        {
            using var listView = new ListView
            {
                AutoArrange = autoArrange,
                VirtualMode = virtualMode,
                View = view
            };

            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.GroupImageList = value;
            Assert.Same(value, listView.GroupImageList);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            listView.GroupImageList = value;
            Assert.Same(value, listView.GroupImageList);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> GroupImageList_SetWithHandleWithNonNullOldValue_GetReturnsExpected()
        {
            yield return new object[] { true, false, View.Details, null };
            yield return new object[] { true, false, View.Details, new ImageList() };
            yield return new object[] { true, false, View.Details, CreateNonEmpty() };
            yield return new object[] { true, false, View.LargeIcon, null };
            yield return new object[] { true, false, View.LargeIcon, new ImageList() };
            yield return new object[] { true, false, View.LargeIcon, CreateNonEmpty() };
            yield return new object[] { true, false, View.List, null };
            yield return new object[] { true, false, View.List, new ImageList() };
            yield return new object[] { true, false, View.List, CreateNonEmpty() };
            yield return new object[] { true, false, View.SmallIcon, null };
            yield return new object[] { true, false, View.SmallIcon, new ImageList() };
            yield return new object[] { true, false, View.SmallIcon, CreateNonEmpty() };
            yield return new object[] { true, false, View.Tile, null };
            yield return new object[] { true, false, View.Tile, new ImageList() };
            yield return new object[] { true, false, View.Tile, CreateNonEmpty() };

            foreach (bool autoArrange in new bool[] { true, false })
            {
                yield return new object[] { autoArrange, true, View.Details, null };
                yield return new object[] { autoArrange, true, View.Details, new ImageList() };
                yield return new object[] { autoArrange, true, View.Details, CreateNonEmpty() };
                yield return new object[] { autoArrange, true, View.LargeIcon, null };
                yield return new object[] { autoArrange, true, View.LargeIcon, new ImageList() };
                yield return new object[] { autoArrange, true, View.LargeIcon, CreateNonEmpty() };
                yield return new object[] { autoArrange, true, View.List, null };
                yield return new object[] { autoArrange, true, View.List, new ImageList() };
                yield return new object[] { autoArrange, true, View.List, CreateNonEmpty() };
                yield return new object[] { autoArrange, true, View.SmallIcon, null };
                yield return new object[] { autoArrange, true, View.SmallIcon, new ImageList() };
                yield return new object[] { autoArrange, true, View.SmallIcon, CreateNonEmpty() };
            }

            yield return new object[] { false, false, View.Details, null };
            yield return new object[] { false, false, View.Details, new ImageList() };
            yield return new object[] { false, false, View.Details, CreateNonEmpty() };
            yield return new object[] { false, false, View.LargeIcon, null };
            yield return new object[] { false, false, View.LargeIcon, new ImageList() };
            yield return new object[] { false, false, View.LargeIcon, CreateNonEmpty() };
            yield return new object[] { false, false, View.List, null };
            yield return new object[] { false, false, View.List, new ImageList() };
            yield return new object[] { false, false, View.List, CreateNonEmpty() };
            yield return new object[] { false, false, View.SmallIcon, null };
            yield return new object[] { false, false, View.SmallIcon, new ImageList() };
            yield return new object[] { false, false, View.SmallIcon, CreateNonEmpty() };
            yield return new object[] { false, false, View.Tile, null };
            yield return new object[] { false, false, View.Tile, new ImageList() };
            yield return new object[] { false, false, View.Tile, CreateNonEmpty() };
        }

        [WinFormsTheory]
        [MemberData(nameof(GroupImageList_SetWithHandleWithNonNullOldValue_GetReturnsExpected))]
        public void ListView_GroupImageList_SetWithHandleWithNonNullOldValue_GetReturnsExpected(bool autoArrange, bool virtualMode, View view, ImageList value)
        {
            using var listView = new ListView
            {
                AutoArrange = autoArrange,
                VirtualMode = virtualMode,
                View = view,
            };

            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.GroupImageList = value;
            Assert.Same(value, listView.GroupImageList);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            listView.GroupImageList = value;
            Assert.Same(value, listView.GroupImageList);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_GroupImageList_Dispose_DetachesFromListView(bool autoArrange)
        {
            using var imageList1 = new ImageList();
            using var imageList2 = new ImageList();
            using var listView = new ListView
            {
                AutoArrange = autoArrange,
                GroupImageList = imageList1
            };

            Assert.Same(imageList1, listView.GroupImageList);

            imageList1.Dispose();
            Assert.Null(listView.GroupImageList);
            Assert.False(listView.IsHandleCreated);

            // Make sure we detached the setter.
            listView.GroupImageList = imageList2;
            imageList1.Dispose();
            Assert.Same(imageList2, listView.GroupImageList);
        }

        [WinFormsTheory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void ListView_GroupImageList_DisposeWithHandle_DetachesFromListView(bool autoArrange, int expectedInvalidatedCallCount)
        {
            using var imageList1 = new ImageList();
            using var imageList2 = new ImageList();
            using var listView = new ListView
            {
                AutoArrange = autoArrange
            };

            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.GroupImageList = imageList1;
            Assert.Same(imageList1, listView.GroupImageList);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            imageList1.Dispose();
            Assert.Null(listView.GroupImageList);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Make sure we detached the setter.
            listView.GroupImageList = imageList2;
            imageList1.Dispose();
            Assert.Same(imageList2, listView.GroupImageList);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ListView_Handle_GetWithBackColor_Success()
        {
            using var control = new ListView
            {
                BackColor = Color.FromArgb(0xFF, 0x12, 0x34, 0x56)
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Equal((IntPtr)0x563412, User32.SendMessageW(control.Handle, (User32.WM)LVM.GETBKCOLOR));
        }

        [WinFormsFact]
        public void ListView_Handle_GetWithForeColor_Success()
        {
            using var control = new ListView
            {
                ForeColor = Color.FromArgb(0x12, 0x34, 0x56, 0x78)
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Equal((IntPtr)0x785634, User32.SendMessageW(control.Handle, (User32.WM)LVM.GETTEXTCOLOR));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_Handle_GetWithoutGroups_Success(bool showGroups)
        {
            using var listView = new ListView
            {
                ShowGroups = showGroups
            };
            Assert.Equal((IntPtr)0, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETGROUPCOUNT, IntPtr.Zero, IntPtr.Zero));
        }

        public static IEnumerable<object[]> Handle_GetWithGroups_TestData()
        {
            foreach (bool showGroups in new bool[] { true, false })
            {
                yield return new object[] { showGroups, null, HorizontalAlignment.Left, null, HorizontalAlignment.Right, string.Empty, string.Empty, 0x00000021 };
                yield return new object[] { showGroups, null, HorizontalAlignment.Center, null, HorizontalAlignment.Center, string.Empty, string.Empty, 0x00000012 };
                yield return new object[] { showGroups, null, HorizontalAlignment.Right, null, HorizontalAlignment.Left, string.Empty, string.Empty, 0x0000000C };

                yield return new object[] { showGroups, string.Empty, HorizontalAlignment.Left, string.Empty, HorizontalAlignment.Right, string.Empty, string.Empty, 0x00000021 };
                yield return new object[] { showGroups, string.Empty, HorizontalAlignment.Center, string.Empty, HorizontalAlignment.Center, string.Empty, string.Empty, 0x00000012 };
                yield return new object[] { showGroups, string.Empty, HorizontalAlignment.Right, string.Empty, HorizontalAlignment.Left, string.Empty, string.Empty, 0x0000000C };

                yield return new object[] { showGroups, "header", HorizontalAlignment.Left, "footer", HorizontalAlignment.Right, "header", "footer", 0x00000021 };
                yield return new object[] { showGroups, "header", HorizontalAlignment.Center, "footer", HorizontalAlignment.Center, "header", "footer", 0x00000012 };
                yield return new object[] { showGroups, "header", HorizontalAlignment.Right, "footer", HorizontalAlignment.Left, "header", "footer", 0x0000000C };

                yield return new object[] { showGroups, "he\0der", HorizontalAlignment.Left, "fo\0oter", HorizontalAlignment.Right, "he", "fo", 0x00000021 };
                yield return new object[] { showGroups, "he\0der", HorizontalAlignment.Center, "fo\0oter", HorizontalAlignment.Center, "he", "fo", 0x00000012 };
                yield return new object[] { showGroups, "he\0der", HorizontalAlignment.Right, "fo\0oter", HorizontalAlignment.Left, "he", "fo", 0x0000000C };
            }
        }

        [WinFormsFact(Skip = "Crash with AbandonedMutexException. See: https://github.com/dotnet/arcade/issues/5325")]
        public unsafe void ListView_Handle_GetWithGroups_Success()
        {
            // Run this from another thread as we call Application.EnableVisualStyles.
            using RemoteInvokeHandle invokerHandle = RemoteExecutor.Invoke(() =>
            {
                foreach (object[] data in Handle_GetWithGroups_TestData())
                {
                    bool showGroups = (bool)data[0];
                    string header = (string)data[1];
                    HorizontalAlignment headerAlignment = (HorizontalAlignment)data[2];
                    string footer = (string)data[3];
                    HorizontalAlignment footerAlignment = (HorizontalAlignment)data[4];
                    string expectedHeaderText = (string)data[5];
                    string expectedFooterText = (string)data[6];
                    int expectedAlign = (int)data[7];

                    Application.EnableVisualStyles();

                    using var listView = new ListView
                    {
                        ShowGroups = showGroups
                    };
                    var group1 = new ListViewGroup();
                    var group2 = new ListViewGroup
                    {
                        Header = header,
                        HeaderAlignment = headerAlignment,
                        Footer = footer,
                        FooterAlignment = footerAlignment
                    };
                    listView.Groups.Add(group1);
                    listView.Groups.Add(group2);

                    Assert.Equal((IntPtr)2, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETGROUPCOUNT, IntPtr.Zero, IntPtr.Zero));
                    char* headerBuffer = stackalloc char[256];
                    char* footerBuffer = stackalloc char[256];
                    var lvgroup1 = new LVGROUPW
                    {
                        cbSize = (uint)sizeof(LVGROUPW),
                        mask = LVGF.HEADER | LVGF.FOOTER | LVGF.GROUPID | LVGF.ALIGN,
                        pszHeader = headerBuffer,
                        cchHeader = 256,
                        pszFooter = footerBuffer,
                        cchFooter = 256,
                    };
                    Assert.Equal((IntPtr)1, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETGROUPINFOBYINDEX, (IntPtr)0, ref lvgroup1));
                    Assert.Equal("ListViewGroup", new string(lvgroup1.pszHeader));
                    Assert.Empty(new string(lvgroup1.pszFooter));
                    Assert.True(lvgroup1.iGroupId >= 0);
                    Assert.Equal(0x00000009, (int)lvgroup1.uAlign);

                    var lvgroup2 = new LVGROUPW
                    {
                        cbSize = (uint)sizeof(LVGROUPW),
                        mask = LVGF.HEADER | LVGF.FOOTER | LVGF.GROUPID | LVGF.ALIGN,
                        pszHeader = headerBuffer,
                        cchHeader = 256,
                        pszFooter = footerBuffer,
                        cchFooter = 256,
                    };
                    Assert.Equal((IntPtr)1, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETGROUPINFOBYINDEX, (IntPtr)1, ref lvgroup2));
                    Assert.Equal(expectedHeaderText, new string(lvgroup2.pszHeader));
                    Assert.Equal(expectedFooterText, new string(lvgroup2.pszFooter));
                    Assert.True(lvgroup2.iGroupId > 0);
                    Assert.Equal(expectedAlign, (int)lvgroup2.uAlign);
                    Assert.True(lvgroup2.iGroupId > lvgroup1.iGroupId);
                }
            });

            // verify the remote process succeeded
            Assert.Equal(0, invokerHandle.ExitCode);
        }

        [WinFormsFact]
        public void ListView_Handle_GetTextBackColor_Success()
        {
            using var control = new ListView();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            IntPtr expected = IntPtr.Size == 8 ? (IntPtr)0xFFFFFFFF : (IntPtr)(-1);
            Assert.Equal(expected, User32.SendMessageW(control.Handle, (User32.WM)LVM.GETTEXTBKCOLOR));
        }

        [WinFormsFact]
        public void ListView_Handle_GetVersion_ReturnsExpected()
        {
            using var control = new ListView();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Equal((IntPtr)5, User32.SendMessageW(control.Handle, (User32.WM)CCM.GETVERSION));
        }

        public static IEnumerable<object[]> Handle_CustomGetVersion_TestData()
        {
            yield return new object[] { IntPtr.Zero, 1 };
            yield return new object[] { (IntPtr)4, 1 };
            yield return new object[] { (IntPtr)5, 0 };
            yield return new object[] { (IntPtr)6, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Handle_CustomGetVersion_TestData))]
        public void ListView_Handle_CustomGetVersion_Success(IntPtr getVersionResult, int expectedSetVersionCallCount)
        {
            using var control = new CustomGetVersionListView
            {
                GetVersionResult = getVersionResult
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Equal(expectedSetVersionCallCount, control.SetVersionCallCount);
        }

        private class CustomGetVersionListView : ListView
        {
            public IntPtr GetVersionResult { get; set; }
            public int SetVersionCallCount { get; set; }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == (int)CCM.GETVERSION)
                {
                    Assert.Equal(IntPtr.Zero, m.WParam);
                    Assert.Equal(IntPtr.Zero, m.LParam);
                    m.Result = GetVersionResult;
                    return;
                }
                else if (m.Msg == (int)CCM.SETVERSION)
                {
                    Assert.Equal((IntPtr)5, m.WParam);
                    Assert.Equal(IntPtr.Zero, m.LParam);
                    SetVersionCallCount++;
                    return;
                }

                base.WndProc(ref m);
            }
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ColumnHeaderStyle))]
        public void ListView_HeaderStyle_Set_GetReturnsExpected(ColumnHeaderStyle value)
        {
            using var listView = new ListView
            {
                HeaderStyle = value
            };
            Assert.Equal(value, listView.HeaderStyle);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.HeaderStyle = value;
            Assert.Equal(value, listView.HeaderStyle);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(ColumnHeaderStyle.Clickable, 0, 0, 0)]
        [InlineData(ColumnHeaderStyle.Nonclickable, 2, 0, 1)]
        [InlineData(ColumnHeaderStyle.None, 1, 1, 0)]
        public void ListView_HeaderStyle_SetClickableWithHandle_GetReturnsExpected(ColumnHeaderStyle value, int expectedInvalidatedCallCount, int expectedStyleChangedCallCount, int expectedCreatedCallCount)
        {
            using var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.HeaderStyle = value;
            Assert.Equal(value, listView.HeaderStyle);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set same.
            listView.HeaderStyle = value;
            Assert.Equal(value, listView.HeaderStyle);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(ColumnHeaderStyle.Clickable, 2, 0, 1)]
        [InlineData(ColumnHeaderStyle.Nonclickable, 0, 0, 0)]
        [InlineData(ColumnHeaderStyle.None, 1, 1, 0)]
        public void ListView_HeaderStyle_SetNonClickableWithHandle_GetReturnsExpected(ColumnHeaderStyle value, int expectedInvalidatedCallCount, int expectedStyleChangedCallCount, int expectedCreatedCallCount)
        {
            using var listView = new ListView
            {
                HeaderStyle = ColumnHeaderStyle.Nonclickable
            };
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.HeaderStyle = value;
            Assert.Equal(value, listView.HeaderStyle);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set same.
            listView.HeaderStyle = value;
            Assert.Equal(value, listView.HeaderStyle);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ColumnHeaderStyle))]
        public void ListView_HeaderStyle_SetInvalidValue_ThrowsInvalidEnumArgumentException(ColumnHeaderStyle value)
        {
            using var listView = new ListView();
            Assert.Throws<InvalidEnumArgumentException>("value", () => listView.HeaderStyle = value);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_HideSelection_Set_GetReturnsExpected(bool value)
        {
            using var listView = new ListView
            {
                HideSelection = value
            };
            Assert.Equal(value, listView.HideSelection);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.HideSelection = value;
            Assert.Equal(value, listView.HideSelection);
            Assert.False(listView.IsHandleCreated);

            // Set different.
            listView.HideSelection = !value;
            Assert.Equal(!value, listView.HideSelection);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void ListView_HideSelection_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount)
        {
            using var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.HideSelection = value;
            Assert.Equal(value, listView.HideSelection);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            listView.HideSelection = value;
            Assert.Equal(value, listView.HideSelection);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            listView.HideSelection = !value;
            Assert.Equal(!value, listView.HideSelection);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
            Assert.Equal(expectedInvalidatedCallCount + 1, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_HotTracking_Set_GetReturnsExpected(bool value)
        {
            using var listView = new ListView
            {
                HotTracking = value
            };
            Assert.Equal(value, listView.HotTracking);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.HotTracking = value;
            Assert.Equal(value, listView.HotTracking);
            Assert.False(listView.IsHandleCreated);

            // Set different.
            listView.HotTracking = !value;
            Assert.Equal(!value, listView.HotTracking);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 3, 4)]
        [InlineData(false, 0, 3)]
        public void ListView_HotTracking_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount1, int expectedInvalidatedCallCount2)
        {
            using var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.HotTracking = value;
            Assert.Equal(value, listView.HotTracking);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            listView.HotTracking = value;
            Assert.Equal(value, listView.HotTracking);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            listView.HotTracking = !value;
            Assert.Equal(!value, listView.HotTracking);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ListView_HotTracking_Set_SetsHoverSelectionAndActivationIfTrue()
        {
            using var listView = new ListView
            {
                HotTracking = true
            };
            Assert.True(listView.HotTracking);
            Assert.True(listView.HoverSelection);
            Assert.Equal(ItemActivation.OneClick, listView.Activation);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.HotTracking = true;
            Assert.True(listView.HotTracking);
            Assert.True(listView.HoverSelection);
            Assert.Equal(ItemActivation.OneClick, listView.Activation);
            Assert.False(listView.IsHandleCreated);

            // Set different.
            listView.HotTracking = false;
            Assert.False(listView.HotTracking);
            Assert.True(listView.HoverSelection);
            Assert.Equal(ItemActivation.OneClick, listView.Activation);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_HoverSelection_Set_GetReturnsExpected(bool value)
        {
            using var listView = new ListView
            {
                HoverSelection = value
            };
            Assert.Equal(value, listView.HoverSelection);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.HoverSelection = value;
            Assert.Equal(value, listView.HoverSelection);
            Assert.False(listView.IsHandleCreated);

            // Set different.
            listView.HoverSelection = !value;
            Assert.Equal(!value, listView.HoverSelection);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void ListView_HoverSelection_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount)
        {
            using var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.HoverSelection = value;
            Assert.Equal(value, listView.HoverSelection);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            listView.HoverSelection = value;
            Assert.Equal(value, listView.HoverSelection);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            listView.HoverSelection = !value;
            Assert.Equal(!value, listView.HoverSelection);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ListView_HoverSelection_SetHotTrackingTrue_Nop()
        {
            using var listView = new ListView
            {
                HotTracking = true,
                HoverSelection = true
            };
            Assert.True(listView.HoverSelection);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.HoverSelection = true;
            Assert.True(listView.HoverSelection);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListView_HoverSelection_SetHotTrackingFalse_ThrowsArgumentException()
        {
            using var listView = new ListView
            {
                HotTracking = true
            };
            Assert.Throws<ArgumentException>("value", () => listView.HoverSelection = false);
            Assert.True(listView.HoverSelection);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_LabelEdit_Set_GetReturnsExpected(bool value)
        {
            using var listView = new ListView
            {
                LabelEdit = value
            };
            Assert.Equal(value, listView.LabelEdit);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.LabelEdit = value;
            Assert.Equal(value, listView.LabelEdit);
            Assert.False(listView.IsHandleCreated);

            // Set different.
            listView.LabelEdit = !value;
            Assert.Equal(!value, listView.LabelEdit);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void ListView_LabelEdit_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount)
        {
            using var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.LabelEdit = value;
            Assert.Equal(value, listView.LabelEdit);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            listView.LabelEdit = value;
            Assert.Equal(value, listView.LabelEdit);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            listView.LabelEdit = !value;
            Assert.Equal(!value, listView.LabelEdit);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
            Assert.Equal(expectedInvalidatedCallCount + 1, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_LabelWrap_Set_GetReturnsExpected(bool value)
        {
            using var listView = new ListView
            {
                LabelWrap = value
            };
            Assert.Equal(value, listView.LabelWrap);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.LabelWrap = value;
            Assert.Equal(value, listView.LabelWrap);
            Assert.False(listView.IsHandleCreated);

            // Set different.
            listView.LabelWrap = !value;
            Assert.Equal(!value, listView.LabelWrap);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 0)]
        [InlineData(false, 1)]
        public void ListView_LabelWrap_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount)
        {
            using var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.LabelWrap = value;
            Assert.Equal(value, listView.LabelWrap);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            listView.LabelWrap = value;
            Assert.Equal(value, listView.LabelWrap);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            listView.LabelWrap = !value;
            Assert.Equal(!value, listView.LabelWrap);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
            Assert.Equal(expectedInvalidatedCallCount + 1, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> LargeImageList_Set_GetReturnsExpected()
        {
            foreach (bool autoArrange in new bool[] { true, false })
            {
                foreach (bool virtualMode in new bool[] { true, false })
                {
                    foreach (View view in new View[] { View.Details, View.LargeIcon, View.List, View.SmallIcon })
                    {
                        yield return new object[] { autoArrange, virtualMode, view, null };
                        yield return new object[] { autoArrange, virtualMode, view, new ImageList() };
                        yield return new object[] { autoArrange, virtualMode, view, CreateNonEmpty() };
                    }
                }

                yield return new object[] { autoArrange, false, View.Tile, null };
                yield return new object[] { autoArrange, false, View.Tile, new ImageList() };
                yield return new object[] { autoArrange, false, View.Tile, CreateNonEmpty() };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(LargeImageList_Set_GetReturnsExpected))]
        public void ListView_LargeImageList_Set_GetReturnsExpected(bool autoArrange, bool virtualMode, View view, ImageList value)
        {
            using var listView = new ListView
            {
                AutoArrange = autoArrange,
                VirtualMode = virtualMode,
                View = view,
                LargeImageList = value
            };
            Assert.Same(value, listView.LargeImageList);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.LargeImageList = value;
            Assert.Same(value, listView.LargeImageList);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(LargeImageList_Set_GetReturnsExpected))]
        public void ListView_LargeImageList_SetWithNonNullOldValue_GetReturnsExpected(bool autoArrange, bool virtualMode, View view, ImageList value)
        {
            using var imageList = new ImageList();
            using var listView = new ListView
            {
                AutoArrange = autoArrange,
                VirtualMode = virtualMode,
                View = view,
                LargeImageList = imageList
            };

            listView.LargeImageList = value;
            Assert.Same(value, listView.LargeImageList);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.LargeImageList = value;
            Assert.Same(value, listView.LargeImageList);
            Assert.False(listView.IsHandleCreated);
        }

        public static IEnumerable<object[]> LargeImageList_SetWithHandle_GetReturnsExpected()
        {
            yield return new object[] { true, false, View.Details, null, 0 };
            yield return new object[] { true, false, View.Details, new ImageList(), 0 };
            yield return new object[] { true, false, View.Details, CreateNonEmpty(), 0 };
            yield return new object[] { true, false, View.LargeIcon, null, 0 };
            yield return new object[] { true, false, View.LargeIcon, new ImageList(), 1 };
            yield return new object[] { true, false, View.LargeIcon, CreateNonEmpty(), 1 };
            yield return new object[] { true, false, View.List, null, 0 };
            yield return new object[] { true, false, View.List, new ImageList(), 0 };
            yield return new object[] { true, false, View.List, CreateNonEmpty(), 0 };
            yield return new object[] { true, false, View.SmallIcon, null, 0 };
            yield return new object[] { true, false, View.SmallIcon, new ImageList(), 1 };
            yield return new object[] { true, false, View.SmallIcon, CreateNonEmpty(), 1 };
            yield return new object[] { true, false, View.Tile, null, 0 };
            yield return new object[] { true, false, View.Tile, new ImageList(), 0 };
            yield return new object[] { true, false, View.Tile, CreateNonEmpty(), 0 };

            foreach (bool autoArrange in new bool[] { true, false })
            {
                yield return new object[] { autoArrange, true, View.Details, null, 0 };
                yield return new object[] { autoArrange, true, View.Details, new ImageList(), 0 };
                yield return new object[] { autoArrange, true, View.Details, CreateNonEmpty(), 0 };
                yield return new object[] { autoArrange, true, View.LargeIcon, null, 0 };
                yield return new object[] { autoArrange, true, View.LargeIcon, new ImageList(), 0 };
                yield return new object[] { autoArrange, true, View.LargeIcon, CreateNonEmpty(), 0 };
                yield return new object[] { autoArrange, true, View.List, null, 0 };
                yield return new object[] { autoArrange, true, View.List, new ImageList(), 0 };
                yield return new object[] { autoArrange, true, View.List, CreateNonEmpty(), 0 };
                yield return new object[] { autoArrange, true, View.SmallIcon, null, 0 };
                yield return new object[] { autoArrange, true, View.SmallIcon, new ImageList(), 0 };
                yield return new object[] { autoArrange, true, View.SmallIcon, CreateNonEmpty(), 0 };
            }

            yield return new object[] { false, false, View.Details, null, 0 };
            yield return new object[] { false, false, View.Details, new ImageList(), 0 };
            yield return new object[] { false, false, View.Details, CreateNonEmpty(), 0 };
            yield return new object[] { false, false, View.LargeIcon, null, 0 };
            yield return new object[] { false, false, View.LargeIcon, new ImageList(), 0 };
            yield return new object[] { false, false, View.LargeIcon, CreateNonEmpty(), 0 };
            yield return new object[] { false, false, View.List, null, 0 };
            yield return new object[] { false, false, View.List, new ImageList(), 0 };
            yield return new object[] { false, false, View.List, CreateNonEmpty(), 0 };
            yield return new object[] { false, false, View.SmallIcon, null, 0 };
            yield return new object[] { false, false, View.SmallIcon, new ImageList(), 0 };
            yield return new object[] { false, false, View.SmallIcon, CreateNonEmpty(), 0 };
            yield return new object[] { false, false, View.Tile, null, 0 };
            yield return new object[] { false, false, View.Tile, new ImageList(), 0 };
            yield return new object[] { false, false, View.Tile, CreateNonEmpty(), 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(LargeImageList_SetWithHandle_GetReturnsExpected))]
        public void ListView_LargeImageList_SetWithHandle_GetReturnsExpected(bool autoArrange, bool virtualMode, View view, ImageList value, int expectedInvalidatedCallCount)
        {
            using var listView = new ListView
            {
                AutoArrange = autoArrange,
                VirtualMode = virtualMode,
                View = view
            };
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.LargeImageList = value;
            Assert.Same(value, listView.LargeImageList);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            listView.LargeImageList = value;
            Assert.Same(value, listView.LargeImageList);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> LargeImageList_SetWithHandleWithNonNullOldValue_GetReturnsExpected()
        {
            yield return new object[] { true, false, View.Details, null, 0 };
            yield return new object[] { true, false, View.Details, new ImageList(), 0 };
            yield return new object[] { true, false, View.Details, CreateNonEmpty(), 0 };
            yield return new object[] { true, false, View.LargeIcon, null, 1 };
            yield return new object[] { true, false, View.LargeIcon, new ImageList(), 1 };
            yield return new object[] { true, false, View.LargeIcon, CreateNonEmpty(), 1 };
            yield return new object[] { true, false, View.List, null, 0 };
            yield return new object[] { true, false, View.List, new ImageList(), 0 };
            yield return new object[] { true, false, View.List, CreateNonEmpty(), 0 };
            yield return new object[] { true, false, View.SmallIcon, null, 1 };
            yield return new object[] { true, false, View.SmallIcon, new ImageList(), 1 };
            yield return new object[] { true, false, View.SmallIcon, CreateNonEmpty(), 1 };
            yield return new object[] { true, false, View.Tile, null, 0 };
            yield return new object[] { true, false, View.Tile, new ImageList(), 0 };
            yield return new object[] { true, false, View.Tile, CreateNonEmpty(), 0 };

            foreach (bool autoArrange in new bool[] { true, false })
            {
                yield return new object[] { autoArrange, true, View.Details, null, 0 };
                yield return new object[] { autoArrange, true, View.Details, new ImageList(), 0 };
                yield return new object[] { autoArrange, true, View.Details, CreateNonEmpty(), 0 };
                yield return new object[] { autoArrange, true, View.LargeIcon, null, 0 };
                yield return new object[] { autoArrange, true, View.LargeIcon, new ImageList(), 0 };
                yield return new object[] { autoArrange, true, View.LargeIcon, CreateNonEmpty(), 0 };
                yield return new object[] { autoArrange, true, View.List, null, 0 };
                yield return new object[] { autoArrange, true, View.List, new ImageList(), 0 };
                yield return new object[] { autoArrange, true, View.List, CreateNonEmpty(), 0 };
                yield return new object[] { autoArrange, true, View.SmallIcon, null, 0 };
                yield return new object[] { autoArrange, true, View.SmallIcon, new ImageList(), 0 };
                yield return new object[] { autoArrange, true, View.SmallIcon, CreateNonEmpty(), 0 };
            }

            yield return new object[] { false, false, View.Details, null, 0 };
            yield return new object[] { false, false, View.Details, new ImageList(), 0 };
            yield return new object[] { false, false, View.Details, CreateNonEmpty(), 0 };
            yield return new object[] { false, false, View.LargeIcon, null, 0 };
            yield return new object[] { false, false, View.LargeIcon, new ImageList(), 0 };
            yield return new object[] { false, false, View.LargeIcon, CreateNonEmpty(), 0 };
            yield return new object[] { false, false, View.List, null, 0 };
            yield return new object[] { false, false, View.List, new ImageList(), 0 };
            yield return new object[] { false, false, View.List, CreateNonEmpty(), 0 };
            yield return new object[] { false, false, View.SmallIcon, null, 0 };
            yield return new object[] { false, false, View.SmallIcon, new ImageList(), 0 };
            yield return new object[] { false, false, View.SmallIcon, CreateNonEmpty(), 0 };
            yield return new object[] { false, false, View.Tile, null, 0 };
            yield return new object[] { false, false, View.Tile, new ImageList(), 0 };
            yield return new object[] { false, false, View.Tile, CreateNonEmpty(), 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(LargeImageList_SetWithHandleWithNonNullOldValue_GetReturnsExpected))]
        public void ListView_LargeImageList_SetWithHandleWithNonNullOldValue_GetReturnsExpected(bool autoArrange, bool virtualMode, View view, ImageList value, int expectedInvalidatedCallCount)
        {
            using var imageList = new ImageList();
            using var listView = new ListView
            {
                AutoArrange = autoArrange,
                VirtualMode = virtualMode,
                View = view,
                LargeImageList = imageList
            };
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.LargeImageList = value;
            Assert.Same(value, listView.LargeImageList);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            listView.LargeImageList = value;
            Assert.Same(value, listView.LargeImageList);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_LargeImageList_Dispose_DetachesFromListView(bool autoArrange)
        {
            using var imageList1 = new ImageList();
            using var imageList2 = new ImageList();
            using var listView = new ListView
            {
                AutoArrange = autoArrange,
                LargeImageList = imageList1
            };
            Assert.Same(imageList1, listView.LargeImageList);

            imageList1.Dispose();
            Assert.Null(listView.LargeImageList);
            Assert.False(listView.IsHandleCreated);

            // Make sure we detached the setter.
            listView.LargeImageList = imageList2;
            imageList1.Dispose();
            Assert.Same(imageList2, listView.LargeImageList);
        }

        [WinFormsTheory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void ListView_LargeImageList_DisposeWithHandle_DetachesFromListView(bool autoArrange, int expectedInvalidatedCallCount)
        {
            using var imageList1 = new ImageList();
            using var imageList2 = new ImageList();
            using var listView = new ListView
            {
                AutoArrange = autoArrange
            };
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.LargeImageList = imageList1;
            Assert.Same(imageList1, listView.LargeImageList);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            imageList1.Dispose();
            Assert.Null(listView.LargeImageList);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Make sure we detached the setter.
            listView.LargeImageList = imageList2;
            imageList1.Dispose();
            Assert.Same(imageList2, listView.LargeImageList);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount * 3, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_MultiSelect_Set_GetReturnsExpected(bool value)
        {
            using var listView = new ListView
            {
                MultiSelect = value
            };
            Assert.Equal(value, listView.MultiSelect);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.MultiSelect = value;
            Assert.Equal(value, listView.MultiSelect);
            Assert.False(listView.IsHandleCreated);

            // Set different.
            listView.MultiSelect = !value;
            Assert.Equal(!value, listView.MultiSelect);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 0)]
        [InlineData(false, 1)]
        public void ListView_MultiSelect_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount)
        {
            using var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.MultiSelect = value;
            Assert.Equal(value, listView.MultiSelect);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            listView.MultiSelect = value;
            Assert.Equal(value, listView.MultiSelect);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            listView.MultiSelect = !value;
            Assert.Equal(!value, listView.MultiSelect);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
            Assert.Equal(expectedInvalidatedCallCount + 1, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_OwnerDraw_Set_GetReturnsExpected(bool value)
        {
            using var listView = new ListView
            {
                OwnerDraw = value
            };
            Assert.Equal(value, listView.OwnerDraw);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.OwnerDraw = value;
            Assert.Equal(value, listView.OwnerDraw);
            Assert.False(listView.IsHandleCreated);

            // Set different.
            listView.OwnerDraw = !value;
            Assert.Equal(!value, listView.OwnerDraw);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void ListView_OwnerDraw_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount)
        {
            using var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.OwnerDraw = value;
            Assert.Equal(value, listView.OwnerDraw);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            listView.OwnerDraw = value;
            Assert.Equal(value, listView.OwnerDraw);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            listView.OwnerDraw = !value;
            Assert.Equal(!value, listView.OwnerDraw);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_Scrollable_Set_GetReturnsExpected(bool value)
        {
            using var listView = new ListView
            {
                Scrollable = value
            };
            Assert.Equal(value, listView.Scrollable);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.Scrollable = value;
            Assert.Equal(value, listView.Scrollable);
            Assert.False(listView.IsHandleCreated);

            // Set different.
            listView.Scrollable = !value;
            Assert.Equal(!value, listView.Scrollable);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 0, 0)]
        [InlineData(false, 2, 1)]
        public void ListView_Scrollable_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount, int expectedCreatedCallCount)
        {
            using var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.Scrollable = value;
            Assert.Equal(value, listView.Scrollable);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set same.
            listView.Scrollable = value;
            Assert.Equal(value, listView.Scrollable);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set different.
            listView.Scrollable = !value;
            Assert.Equal(!value, listView.Scrollable);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount + 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount + 1, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_ShowGroups_Set_GetReturnsExpected(bool value)
        {
            using var listView = new ListView
            {
                ShowGroups = value
            };
            Assert.Equal(value, listView.ShowGroups);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.ShowGroups = value;
            Assert.Equal(value, listView.ShowGroups);
            Assert.False(listView.IsHandleCreated);

            // Set different.
            listView.ShowGroups = !value;
            Assert.Equal(!value, listView.ShowGroups);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_ShowGroups_VirtualMode_Set_GetReturnsExpected(bool value)
        {
            using var listView = new ListView
            {
                ShowGroups = value,
                VirtualMode = true,
            };
            Assert.Equal(value, listView.ShowGroups);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.ShowGroups = value;
            Assert.Equal(value, listView.ShowGroups);
            Assert.False(listView.IsHandleCreated);

            // Set different.
            listView.ShowGroups = !value;
            Assert.Equal(!value, listView.ShowGroups);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_ShowGroups_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.ShowGroups = value;
            Assert.Equal(value, listView.ShowGroups);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            listView.ShowGroups = value;
            Assert.Equal(value, listView.ShowGroups);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            listView.ShowGroups = !value;
            Assert.Equal(!value, listView.ShowGroups);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_ShowGroups_VirtualMode_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var listView = new ListView
            {
                VirtualMode = true,
            };

            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.ShowGroups = value;
            Assert.Equal(value, listView.ShowGroups);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            listView.ShowGroups = value;
            Assert.Equal(value, listView.ShowGroups);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            listView.ShowGroups = !value;
            Assert.Equal(!value, listView.ShowGroups);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_ShowItemToolTips_Set_GetReturnsExpected(bool value)
        {
            using var listView = new ListView
            {
                ShowItemToolTips = value
            };
            Assert.Equal(value, listView.ShowItemToolTips);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.ShowItemToolTips = value;
            Assert.Equal(value, listView.ShowItemToolTips);
            Assert.False(listView.IsHandleCreated);

            // Set different.
            listView.ShowItemToolTips = !value;
            Assert.Equal(!value, listView.ShowItemToolTips);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 2, 1)]
        [InlineData(false, 0, 0)]
        public void ListView_ShowItemToolTips_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount, int expectedCreatedCallCount)
        {
            using var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.ShowItemToolTips = value;
            Assert.Equal(value, listView.ShowItemToolTips);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set same.
            listView.ShowItemToolTips = value;
            Assert.Equal(value, listView.ShowItemToolTips);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set different.
            listView.ShowItemToolTips = !value;
            Assert.Equal(!value, listView.ShowItemToolTips);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount + 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount + 1, createdCallCount);
        }

        public static IEnumerable<object[]> SmallImageList_Set_GetReturnsExpected()
        {
            foreach (bool autoArrange in new bool[] { true, false })
            {
                foreach (bool virtualMode in new bool[] { true, false })
                {
                    foreach (View view in new View[] { View.Details, View.LargeIcon, View.List, View.SmallIcon })
                    {
                        yield return new object[] { autoArrange, virtualMode, view, null };
                        yield return new object[] { autoArrange, virtualMode, view, new ImageList() };
                        yield return new object[] { autoArrange, virtualMode, view, CreateNonEmpty() };
                    }
                }

                yield return new object[] { autoArrange, false, View.Tile, null };
                yield return new object[] { autoArrange, false, View.Tile, new ImageList() };
                yield return new object[] { autoArrange, false, View.Tile, CreateNonEmpty() };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(SmallImageList_Set_GetReturnsExpected))]
        public void ListView_SmallImageList_Set_GetReturnsExpected(bool autoArrange, bool virtualMode, View view, ImageList value)
        {
            using var listView = new ListView
            {
                AutoArrange = autoArrange,
                VirtualMode = virtualMode,
                View = view,
                SmallImageList = value
            };
            Assert.Same(value, listView.SmallImageList);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.SmallImageList = value;
            Assert.Same(value, listView.SmallImageList);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(SmallImageList_Set_GetReturnsExpected))]
        public void ListView_SmallImageList_SetWithNonNullOldValue_GetReturnsExpected(bool autoArrange, bool virtualMode, View view, ImageList value)
        {
            using var imageList = new ImageList();
            using var listView = new ListView
            {
                AutoArrange = autoArrange,
                VirtualMode = virtualMode,
                View = view,
                SmallImageList = imageList
            };

            listView.SmallImageList = value;
            Assert.Same(value, listView.SmallImageList);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.SmallImageList = value;
            Assert.Same(value, listView.SmallImageList);
            Assert.False(listView.IsHandleCreated);
        }

        public static IEnumerable<object[]> SmallImageList_SetWithHandle_GetReturnsExpected()
        {
            yield return new object[] { true, false, View.Details, null, 0, 0 };
            yield return new object[] { true, false, View.Details, new ImageList(), 1, 0 };
            yield return new object[] { true, false, View.Details, CreateNonEmpty(), 1, 0 };
            yield return new object[] { true, false, View.LargeIcon, null, 0, 0 };
            yield return new object[] { true, false, View.LargeIcon, new ImageList(), 1, 0 };
            yield return new object[] { true, false, View.LargeIcon, CreateNonEmpty(), 1, 0 };
            yield return new object[] { true, false, View.List, null, 0, 0 };
            yield return new object[] { true, false, View.List, new ImageList(), 0, 0 };
            yield return new object[] { true, false, View.List, CreateNonEmpty(), 0, 0 };
            yield return new object[] { true, false, View.SmallIcon, null, 0, 0 };
            yield return new object[] { true, false, View.SmallIcon, new ImageList(), 4, 2 };
            yield return new object[] { true, false, View.SmallIcon, CreateNonEmpty(), 4, 2 };
            yield return new object[] { true, false, View.Tile, null, 0, 0 };
            yield return new object[] { true, false, View.Tile, new ImageList(), 0, 0 };
            yield return new object[] { true, false, View.Tile, CreateNonEmpty(), 0, 0 };

            foreach (bool autoArrange in new bool[] { true, false })
            {
                yield return new object[] { autoArrange, true, View.Details, null, 0, 0 };
                yield return new object[] { autoArrange, true, View.Details, new ImageList(), 1, 0 };
                yield return new object[] { autoArrange, true, View.Details, CreateNonEmpty(), 1, 0 };
                yield return new object[] { autoArrange, true, View.LargeIcon, null, 0, 0 };
                yield return new object[] { autoArrange, true, View.LargeIcon, new ImageList(), 0, 0 };
                yield return new object[] { autoArrange, true, View.LargeIcon, CreateNonEmpty(), 0, 0 };
                yield return new object[] { autoArrange, true, View.List, null, 0, 0 };
                yield return new object[] { autoArrange, true, View.List, new ImageList(), 0, 0 };
                yield return new object[] { autoArrange, true, View.List, CreateNonEmpty(), 0, 0 };
                yield return new object[] { autoArrange, true, View.SmallIcon, null, 0, 0 };
                yield return new object[] { autoArrange, true, View.SmallIcon, new ImageList(), 2, 2 };
                yield return new object[] { autoArrange, true, View.SmallIcon, CreateNonEmpty(), 2, 2 };
            }

            yield return new object[] { false, false, View.Details, null, 0, 0 };
            yield return new object[] { false, false, View.Details, new ImageList(), 1, 0 };
            yield return new object[] { false, false, View.Details, CreateNonEmpty(), 1, 0 };
            yield return new object[] { false, false, View.LargeIcon, null, 0, 0 };
            yield return new object[] { false, false, View.LargeIcon, new ImageList(), 0, 0 };
            yield return new object[] { false, false, View.LargeIcon, CreateNonEmpty(), 0, 0 };
            yield return new object[] { false, false, View.List, null, 0, 0 };
            yield return new object[] { false, false, View.List, new ImageList(), 0, 0 };
            yield return new object[] { false, false, View.List, CreateNonEmpty(), 0, 0 };
            yield return new object[] { false, false, View.SmallIcon, null, 0, 0 };
            yield return new object[] { false, false, View.SmallIcon, new ImageList(), 2, 2 };
            yield return new object[] { false, false, View.SmallIcon, CreateNonEmpty(), 2, 2 };
            yield return new object[] { false, false, View.Tile, null, 0, 0 };
            yield return new object[] { false, false, View.Tile, new ImageList(), 0, 0 };
            yield return new object[] { false, false, View.Tile, CreateNonEmpty(), 0, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(SmallImageList_SetWithHandle_GetReturnsExpected))]
        public void ListView_SmallImageList_SetWithHandle_GetReturnsExpected(bool autoArrange, bool virtualMode, View view, ImageList value, int expectedInvalidatedCallCount, int expectedStyleChangedCallCount)
        {
            using var listView = new ListView
            {
                AutoArrange = autoArrange,
                VirtualMode = virtualMode,
                View = view
            };
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.SmallImageList = value;
            Assert.Same(value, listView.SmallImageList);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            listView.SmallImageList = value;
            Assert.Same(value, listView.SmallImageList);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> SmallImageList_SetWithHandleWithNonNullOldValue_GetReturnsExpected()
        {
            yield return new object[] { true, false, View.Details, null, 1, 0 };
            yield return new object[] { true, false, View.Details, new ImageList(), 1, 0 };
            yield return new object[] { true, false, View.Details, CreateNonEmpty(), 1, 0 };
            yield return new object[] { true, false, View.LargeIcon, null, 1, 0 };
            yield return new object[] { true, false, View.LargeIcon, new ImageList(), 1, 0 };
            yield return new object[] { true, false, View.LargeIcon, CreateNonEmpty(), 1, 0 };
            yield return new object[] { true, false, View.List, null, 0, 0 };
            yield return new object[] { true, false, View.List, new ImageList(), 0, 0 };
            yield return new object[] { true, false, View.List, CreateNonEmpty(), 0, 0 };
            yield return new object[] { true, false, View.SmallIcon, null, 4, 2 };
            yield return new object[] { true, false, View.SmallIcon, new ImageList(), 4, 2 };
            yield return new object[] { true, false, View.SmallIcon, CreateNonEmpty(), 4, 2 };
            yield return new object[] { true, false, View.Tile, null, 0, 0 };
            yield return new object[] { true, false, View.Tile, new ImageList(), 0, 0 };
            yield return new object[] { true, false, View.Tile, CreateNonEmpty(), 0, 0 };

            foreach (bool autoArrange in new bool[] { true, false })
            {
                yield return new object[] { autoArrange, true, View.Details, null, 1, 0 };
                yield return new object[] { autoArrange, true, View.Details, new ImageList(), 1, 0 };
                yield return new object[] { autoArrange, true, View.Details, CreateNonEmpty(), 1, 0 };
                yield return new object[] { autoArrange, true, View.LargeIcon, null, 0, 0 };
                yield return new object[] { autoArrange, true, View.LargeIcon, new ImageList(), 0, 0 };
                yield return new object[] { autoArrange, true, View.LargeIcon, CreateNonEmpty(), 0, 0 };
                yield return new object[] { autoArrange, true, View.List, null, 0, 0 };
                yield return new object[] { autoArrange, true, View.List, new ImageList(), 0, 0 };
                yield return new object[] { autoArrange, true, View.List, CreateNonEmpty(), 0, 0 };
                yield return new object[] { autoArrange, true, View.SmallIcon, null, 2, 2 };
                yield return new object[] { autoArrange, true, View.SmallIcon, new ImageList(), 2, 2 };
                yield return new object[] { autoArrange, true, View.SmallIcon, CreateNonEmpty(), 2, 2 };
            }

            yield return new object[] { false, false, View.Details, null, 1, 0 };
            yield return new object[] { false, false, View.Details, new ImageList(), 1, 0 };
            yield return new object[] { false, false, View.Details, CreateNonEmpty(), 1, 0 };
            yield return new object[] { false, false, View.LargeIcon, null, 0, 0 };
            yield return new object[] { false, false, View.LargeIcon, new ImageList(), 0, 0 };
            yield return new object[] { false, false, View.LargeIcon, CreateNonEmpty(), 0, 0 };
            yield return new object[] { false, false, View.List, null, 0, 0 };
            yield return new object[] { false, false, View.List, new ImageList(), 0, 0 };
            yield return new object[] { false, false, View.List, CreateNonEmpty(), 0, 0 };
            yield return new object[] { false, false, View.SmallIcon, null, 2, 2 };
            yield return new object[] { false, false, View.SmallIcon, new ImageList(), 2, 2 };
            yield return new object[] { false, false, View.SmallIcon, CreateNonEmpty(), 2, 2 };
            yield return new object[] { false, false, View.Tile, null, 0, 0 };
            yield return new object[] { false, false, View.Tile, new ImageList(), 0, 0 };
            yield return new object[] { false, false, View.Tile, CreateNonEmpty(), 0, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(SmallImageList_SetWithHandleWithNonNullOldValue_GetReturnsExpected))]
        public void ListView_SmallImageList_SetWithHandleWithNonNullOldValue_GetReturnsExpected(bool autoArrange, bool virtualMode, View view, ImageList value, int expectedInvalidatedCallCount, int expectedStyleChangedCallCount)
        {
            using var imageList = new ImageList();
            using var listView = new ListView
            {
                AutoArrange = autoArrange,
                VirtualMode = virtualMode,
                View = view,
                SmallImageList = imageList
            };
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.SmallImageList = value;
            Assert.Same(value, listView.SmallImageList);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            listView.SmallImageList = value;
            Assert.Same(value, listView.SmallImageList);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_SmallImageList_Dispose_DetachesFromListView(bool autoArrange)
        {
            using var imageList1 = new ImageList();
            using var imageList2 = new ImageList();
            using var listView = new ListView
            {
                AutoArrange = autoArrange,
                SmallImageList = imageList1
            };
            Assert.Same(imageList1, listView.SmallImageList);

            imageList1.Dispose();
            Assert.Null(listView.SmallImageList);
            Assert.False(listView.IsHandleCreated);

            // Make sure we detached the setter.
            listView.SmallImageList = imageList2;
            imageList1.Dispose();
            Assert.Same(imageList2, listView.SmallImageList);
        }

        [WinFormsTheory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void ListView_SmallImageList_DisposeWithHandle_DetachesFromListView(bool autoArrange, int expectedInvalidatedCallCount)
        {
            using var imageList1 = new ImageList();
            using var imageList2 = new ImageList();
            using var listView = new ListView
            {
                AutoArrange = autoArrange
            };
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.SmallImageList = imageList1;
            Assert.Same(imageList1, listView.SmallImageList);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            imageList1.Dispose();
            Assert.Null(listView.SmallImageList);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Make sure we detached the setter.
            listView.SmallImageList = imageList2;
            imageList1.Dispose();
            Assert.Same(imageList2, listView.SmallImageList);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount * 3, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> StateImageList_Set_GetReturnsExpected()
        {
            foreach (bool useCompatibleStateImageBehavior in new bool[] { true, false })
            {
                foreach (bool checkBoxes in new bool[] { true, false })
                {
                    foreach (bool autoArrange in new bool[] { true, false })
                    {
                        foreach (bool virtualMode in new bool[] { true, false })
                        {
                            foreach (View view in new View[] { View.Details, View.LargeIcon, View.List, View.SmallIcon })
                            {
                                yield return new object[] { useCompatibleStateImageBehavior, checkBoxes, autoArrange, virtualMode, view, null };
                                yield return new object[] { useCompatibleStateImageBehavior, checkBoxes, autoArrange, virtualMode, view, new ImageList() };
                                yield return new object[] { useCompatibleStateImageBehavior, checkBoxes, autoArrange, virtualMode, view, CreateNonEmpty() };
                            }
                        }
                    }
                }

                yield return new object[] { useCompatibleStateImageBehavior, false, true, false, View.Tile, null };
                yield return new object[] { useCompatibleStateImageBehavior, false, true, false, View.Tile, new ImageList() };
                yield return new object[] { useCompatibleStateImageBehavior, false, true, false, View.Tile, CreateNonEmpty() };

                yield return new object[] { useCompatibleStateImageBehavior, false, false, false, View.Tile, null };
                yield return new object[] { useCompatibleStateImageBehavior, false, false, false, View.Tile, new ImageList() };
                yield return new object[] { useCompatibleStateImageBehavior, false, false, false, View.Tile, CreateNonEmpty() };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(StateImageList_Set_GetReturnsExpected))]
        public void ListView_StateImageList_Set_GetReturnsExpected(bool useCompatibleStateImageBehavior, bool checkBoxes, bool autoArrange, bool virtualMode, View view, ImageList value)
        {
            using var listView = new ListView
            {
                UseCompatibleStateImageBehavior = useCompatibleStateImageBehavior,
                CheckBoxes = checkBoxes,
                AutoArrange = autoArrange,
                VirtualMode = virtualMode,
                View = view,
                StateImageList = value
            };
            Assert.Same(value, listView.StateImageList);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.StateImageList = value;
            Assert.Same(value, listView.StateImageList);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(StateImageList_Set_GetReturnsExpected))]
        public void ListView_StateImageList_SetWithNonNullOldValue_GetReturnsExpected(bool useCompatibleStateImageBehavior, bool checkBoxes, bool autoArrange, bool virtualMode, View view, ImageList value)
        {
            using var imageList = new ImageList();
            using var listView = new ListView
            {
                UseCompatibleStateImageBehavior = useCompatibleStateImageBehavior,
                CheckBoxes = checkBoxes,
                AutoArrange = autoArrange,
                VirtualMode = virtualMode,
                View = view,
                StateImageList = imageList
            };

            listView.StateImageList = value;
            Assert.Same(value, listView.StateImageList);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.StateImageList = value;
            Assert.Same(value, listView.StateImageList);
            Assert.False(listView.IsHandleCreated);
        }

        public static IEnumerable<object[]> StateImageList_SetWithHandle_GetReturnsExpected()
        {
            // UseCompatibleStateImageBehavior true
            foreach (bool checkBoxes in new bool[] { true, false })
            {
                yield return new object[] { true, checkBoxes, true, false, View.Details, null, 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.Details, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.Details, CreateNonEmpty(), 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.LargeIcon, null, 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.LargeIcon, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.LargeIcon, CreateNonEmpty(), 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.List, null, 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.List, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.List, CreateNonEmpty(), 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.SmallIcon, null, 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.SmallIcon, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.SmallIcon, CreateNonEmpty(), 0, 0 };

                foreach (bool autoArrange in new bool[] { true, false })
                {
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.Details, null, 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.Details, new ImageList(), 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.Details, CreateNonEmpty(), 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.LargeIcon, null, 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.LargeIcon, new ImageList(), 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.LargeIcon, CreateNonEmpty(), 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.List, null, 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.List, new ImageList(), 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.List, CreateNonEmpty(), 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.SmallIcon, null, 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.SmallIcon, new ImageList(), 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.SmallIcon, CreateNonEmpty(), 0, 0 };
                }

                yield return new object[] { true, checkBoxes, false, false, View.Details, null, 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.Details, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.Details, CreateNonEmpty(), 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.LargeIcon, null, 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.LargeIcon, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.LargeIcon, CreateNonEmpty(), 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.List, null, 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.List, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.List, CreateNonEmpty(), 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.SmallIcon, null, 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.SmallIcon, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.SmallIcon, CreateNonEmpty(), 0, 0 };
            }

            yield return new object[] { true, false, true, false, View.Tile, null, 0, 0 };
            yield return new object[] { true, false, true, false, View.Tile, new ImageList(), 0, 0 };
            yield return new object[] { true, false, true, false, View.Tile, CreateNonEmpty(), 0, 0 };

            yield return new object[] { true, false, false, false, View.Tile, null, 0, 0 };
            yield return new object[] { true, false, false, false, View.Tile, new ImageList(), 0, 0 };
            yield return new object[] { true, false, false, false, View.Tile, CreateNonEmpty(), 0, 0 };

            // UseCompatibleStateImageBehavior false, CheckBoxes true
            yield return new object[] { false, true, true, false, View.Details, null, 0, 0 };
            yield return new object[] { false, true, true, false, View.Details, new ImageList(), 1, 1 };
            yield return new object[] { false, true, true, false, View.Details, CreateNonEmpty(), 1, 1 };
            yield return new object[] { false, true, true, false, View.LargeIcon, null, 0, 0 };
            yield return new object[] { false, true, true, false, View.LargeIcon, new ImageList(), 3, 1 };
            yield return new object[] { false, true, true, false, View.LargeIcon, CreateNonEmpty(), 3, 1 };
            yield return new object[] { false, true, true, false, View.List, null, 0, 0 };
            yield return new object[] { false, true, true, false, View.List, new ImageList(), 1, 1 };
            yield return new object[] { false, true, true, false, View.List, CreateNonEmpty(), 1, 1 };
            yield return new object[] { false, true, true, false, View.SmallIcon, null, 0, 0 };
            yield return new object[] { false, true, true, false, View.SmallIcon, new ImageList(), 3, 1 };
            yield return new object[] { false, true, true, false, View.SmallIcon, CreateNonEmpty(), 3, 1 };

            foreach (bool autoArrange in new bool[] { true, false })
            {
                yield return new object[] { false, true, autoArrange, true, View.Details, null, 0, 0 };
                yield return new object[] { false, true, autoArrange, true, View.Details, new ImageList(), 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.Details, CreateNonEmpty(), 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.LargeIcon, null, 0, 0 };
                yield return new object[] { false, true, autoArrange, true, View.LargeIcon, new ImageList(), 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.LargeIcon, CreateNonEmpty(), 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.List, null, 0, 0 };
                yield return new object[] { false, true, autoArrange, true, View.List, new ImageList(), 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.List, CreateNonEmpty(), 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.SmallIcon, null, 0, 0 };
                yield return new object[] { false, true, autoArrange, true, View.SmallIcon, new ImageList(), 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.SmallIcon, CreateNonEmpty(), 1, 1 };
            }

            yield return new object[] { false, true, false, false, View.Details, null, 0, 0 };
            yield return new object[] { false, true, false, false, View.Details, new ImageList(), 1, 1 };
            yield return new object[] { false, true, false, false, View.Details, CreateNonEmpty(), 1, 1 };
            yield return new object[] { false, true, false, false, View.LargeIcon, null, 0, 0 };
            yield return new object[] { false, true, false, false, View.LargeIcon, new ImageList(), 1, 1 };
            yield return new object[] { false, true, false, false, View.LargeIcon, CreateNonEmpty(), 1, 1 };
            yield return new object[] { false, true, false, false, View.List, null, 0, 0 };
            yield return new object[] { false, true, false, false, View.List, new ImageList(), 1, 1 };
            yield return new object[] { false, true, false, false, View.List, CreateNonEmpty(), 1, 1 };
            yield return new object[] { false, true, false, false, View.SmallIcon, null, 0, 0 };
            yield return new object[] { false, true, false, false, View.SmallIcon, new ImageList(), 1, 1 };
            yield return new object[] { false, true, false, false, View.SmallIcon, CreateNonEmpty(), 1, 1 };

            // UseCompatibleStateImageBehavior false, CheckBoxes false
            yield return new object[] { false, false, true, false, View.Details, null, 0, 0 };
            yield return new object[] { false, false, true, false, View.Details, new ImageList(), 0, 0 };
            yield return new object[] { false, false, true, false, View.Details, CreateNonEmpty(), 0, 0 };
            yield return new object[] { false, false, true, false, View.LargeIcon, null, 0, 0 };
            yield return new object[] { false, false, true, false, View.LargeIcon, new ImageList(), 1, 0 };
            yield return new object[] { false, false, true, false, View.LargeIcon, CreateNonEmpty(), 1, 0 };
            yield return new object[] { false, false, true, false, View.List, null, 0, 0 };
            yield return new object[] { false, false, true, false, View.List, new ImageList(), 0, 0 };
            yield return new object[] { false, false, true, false, View.List, CreateNonEmpty(), 0, 0 };
            yield return new object[] { false, false, true, false, View.SmallIcon, null, 0, 0 };
            yield return new object[] { false, false, true, false, View.SmallIcon, new ImageList(), 1, 0 };
            yield return new object[] { false, false, true, false, View.SmallIcon, CreateNonEmpty(), 1, 0 };
            yield return new object[] { false, false, true, false, View.Tile, null, 0, 0 };
            yield return new object[] { false, false, true, false, View.Tile, new ImageList(), 0, 0 };
            yield return new object[] { false, false, true, false, View.Tile, CreateNonEmpty(), 0, 0 };

            foreach (bool autoArrange in new bool[] { true, false })
            {
                yield return new object[] { false, false, autoArrange, true, View.Details, null, 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.Details, new ImageList(), 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.Details, CreateNonEmpty(), 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.LargeIcon, null, 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.LargeIcon, new ImageList(), 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.LargeIcon, CreateNonEmpty(), 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.List, null, 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.List, new ImageList(), 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.List, CreateNonEmpty(), 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.SmallIcon, null, 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.SmallIcon, new ImageList(), 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.SmallIcon, CreateNonEmpty(), 0, 0 };
            }

            yield return new object[] { false, false, false, false, View.Details, null, 0, 0 };
            yield return new object[] { false, false, false, false, View.Details, new ImageList(), 0, 0 };
            yield return new object[] { false, false, false, false, View.Details, CreateNonEmpty(), 0, 0 };
            yield return new object[] { false, false, false, false, View.LargeIcon, null, 0, 0 };
            yield return new object[] { false, false, false, false, View.LargeIcon, new ImageList(), 0, 0 };
            yield return new object[] { false, false, false, false, View.LargeIcon, CreateNonEmpty(), 0, 0 };
            yield return new object[] { false, false, false, false, View.List, null, 0, 0 };
            yield return new object[] { false, false, false, false, View.List, new ImageList(), 0, 0 };
            yield return new object[] { false, false, false, false, View.List, CreateNonEmpty(), 0, 0 };
            yield return new object[] { false, false, false, false, View.SmallIcon, null, 0, 0 };
            yield return new object[] { false, false, false, false, View.SmallIcon, new ImageList(), 0, 0 };
            yield return new object[] { false, false, false, false, View.SmallIcon, CreateNonEmpty(), 0, 0 };
            yield return new object[] { false, false, false, false, View.Tile, null, 0, 0 };
            yield return new object[] { false, false, false, false, View.Tile, new ImageList(), 0, 0 };
            yield return new object[] { false, false, false, false, View.Tile, CreateNonEmpty(), 0, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(StateImageList_SetWithHandle_GetReturnsExpected))]
        public void ListView_StateImageList_SetWithHandle_GetReturnsExpected(bool useCompatibleStateImageBehavior, bool checkBoxes, bool autoArrange, bool virtualMode, View view, ImageList value, int expectedInvalidatedCallCount, int expectedCreatedCallCount)
        {
            using var listView = new ListView
            {
                UseCompatibleStateImageBehavior = useCompatibleStateImageBehavior,
                CheckBoxes = checkBoxes,
                AutoArrange = autoArrange,
                VirtualMode = virtualMode,
                View = view
            };
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.StateImageList = value;
            Assert.Same(value, listView.StateImageList);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set same.
            listView.StateImageList = value;
            Assert.Same(value, listView.StateImageList);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);
        }

        public static IEnumerable<object[]> StateImageList_SetWithHandleWithNonNullOldValue_GetReturnsExpected()
        {
            // UseCompatibleStateImageBehavior true
            foreach (bool checkBoxes in new bool[] { true, false })
            {
                yield return new object[] { true, checkBoxes, true, false, View.Details, null, 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.Details, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.Details, CreateNonEmpty(), 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.LargeIcon, null, 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.LargeIcon, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.LargeIcon, CreateNonEmpty(), 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.List, null, 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.List, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.List, CreateNonEmpty(), 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.SmallIcon, null, 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.SmallIcon, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.SmallIcon, CreateNonEmpty(), 0, 0 };

                foreach (bool autoArrange in new bool[] { true, false })
                {
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.Details, null, 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.Details, new ImageList(), 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.Details, CreateNonEmpty(), 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.LargeIcon, null, 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.LargeIcon, new ImageList(), 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.LargeIcon, CreateNonEmpty(), 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.List, null, 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.List, new ImageList(), 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.List, CreateNonEmpty(), 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.SmallIcon, null, 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.SmallIcon, new ImageList(), 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.SmallIcon, CreateNonEmpty(), 0, 0 };
                }

                yield return new object[] { true, checkBoxes, false, false, View.Details, null, 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.Details, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.Details, CreateNonEmpty(), 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.LargeIcon, null, 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.LargeIcon, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.LargeIcon, CreateNonEmpty(), 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.List, null, 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.List, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.List, CreateNonEmpty(), 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.SmallIcon, null, 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.SmallIcon, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.SmallIcon, CreateNonEmpty(), 0, 0 };
            }

            yield return new object[] { true, false, true, false, View.Tile, null, 0, 0 };
            yield return new object[] { true, false, true, false, View.Tile, new ImageList(), 0, 0 };
            yield return new object[] { true, false, true, false, View.Tile, CreateNonEmpty(), 0, 0 };

            yield return new object[] { true, false, false, false, View.Tile, null, 0, 0 };
            yield return new object[] { true, false, false, false, View.Tile, new ImageList(), 0, 0 };
            yield return new object[] { true, false, false, false, View.Tile, CreateNonEmpty(), 0, 0 };

            // UseCompatibleStateImageBehavior false, CheckBoxes true
            yield return new object[] { false, true, true, false, View.Details, null, 1, 1 };
            yield return new object[] { false, true, true, false, View.Details, new ImageList(), 1, 1 };
            yield return new object[] { false, true, true, false, View.Details, CreateNonEmpty(), 1, 1 };
            yield return new object[] { false, true, true, false, View.LargeIcon, null, 3, 1 };
            yield return new object[] { false, true, true, false, View.LargeIcon, new ImageList(), 3, 1 };
            yield return new object[] { false, true, true, false, View.LargeIcon, CreateNonEmpty(), 3, 1 };
            yield return new object[] { false, true, true, false, View.List, null, 1, 1 };
            yield return new object[] { false, true, true, false, View.List, new ImageList(), 1, 1 };
            yield return new object[] { false, true, true, false, View.List, CreateNonEmpty(), 1, 1 };
            yield return new object[] { false, true, true, false, View.SmallIcon, null, 3, 1 };
            yield return new object[] { false, true, true, false, View.SmallIcon, new ImageList(), 3, 1 };
            yield return new object[] { false, true, true, false, View.SmallIcon, CreateNonEmpty(), 3, 1 };

            foreach (bool autoArrange in new bool[] { true, false })
            {
                yield return new object[] { false, true, autoArrange, true, View.Details, null, 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.Details, new ImageList(), 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.Details, CreateNonEmpty(), 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.LargeIcon, null, 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.LargeIcon, new ImageList(), 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.LargeIcon, CreateNonEmpty(), 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.List, null, 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.List, new ImageList(), 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.List, CreateNonEmpty(), 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.SmallIcon, null, 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.SmallIcon, new ImageList(), 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.SmallIcon, CreateNonEmpty(), 1, 1 };
            }

            yield return new object[] { false, true, false, false, View.Details, null, 1, 1 };
            yield return new object[] { false, true, false, false, View.Details, new ImageList(), 1, 1 };
            yield return new object[] { false, true, false, false, View.Details, CreateNonEmpty(), 1, 1 };
            yield return new object[] { false, true, false, false, View.LargeIcon, null, 1, 1 };
            yield return new object[] { false, true, false, false, View.LargeIcon, new ImageList(), 1, 1 };
            yield return new object[] { false, true, false, false, View.LargeIcon, CreateNonEmpty(), 1, 1 };
            yield return new object[] { false, true, false, false, View.List, null, 1, 1 };
            yield return new object[] { false, true, false, false, View.List, new ImageList(), 1, 1 };
            yield return new object[] { false, true, false, false, View.List, CreateNonEmpty(), 1, 1 };
            yield return new object[] { false, true, false, false, View.SmallIcon, null, 1, 1 };
            yield return new object[] { false, true, false, false, View.SmallIcon, new ImageList(), 1, 1 };
            yield return new object[] { false, true, false, false, View.SmallIcon, CreateNonEmpty(), 1, 1 };

            // UseCompatibleStateImageBehavior false, CheckBoxes false
            yield return new object[] { false, false, true, false, View.Details, null, 0, 0 };
            yield return new object[] { false, false, true, false, View.Details, new ImageList(), 0, 0 };
            yield return new object[] { false, false, true, false, View.Details, CreateNonEmpty(), 0, 0 };
            yield return new object[] { false, false, true, false, View.LargeIcon, null, 1, 0 };
            yield return new object[] { false, false, true, false, View.LargeIcon, new ImageList(), 1, 0 };
            yield return new object[] { false, false, true, false, View.LargeIcon, CreateNonEmpty(), 1, 0 };
            yield return new object[] { false, false, true, false, View.List, null, 0, 0 };
            yield return new object[] { false, false, true, false, View.List, new ImageList(), 0, 0 };
            yield return new object[] { false, false, true, false, View.List, CreateNonEmpty(), 0, 0 };
            yield return new object[] { false, false, true, false, View.SmallIcon, null, 1, 0 };
            yield return new object[] { false, false, true, false, View.SmallIcon, new ImageList(), 1, 0 };
            yield return new object[] { false, false, true, false, View.SmallIcon, CreateNonEmpty(), 1, 0 };
            yield return new object[] { false, false, true, false, View.Tile, null, 0, 0 };
            yield return new object[] { false, false, true, false, View.Tile, new ImageList(), 0, 0 };
            yield return new object[] { false, false, true, false, View.Tile, CreateNonEmpty(), 0, 0 };

            foreach (bool autoArrange in new bool[] { true, false })
            {
                yield return new object[] { false, false, autoArrange, true, View.Details, null, 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.Details, new ImageList(), 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.Details, CreateNonEmpty(), 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.LargeIcon, null, 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.LargeIcon, new ImageList(), 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.LargeIcon, CreateNonEmpty(), 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.List, null, 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.List, new ImageList(), 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.List, CreateNonEmpty(), 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.SmallIcon, null, 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.SmallIcon, new ImageList(), 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.SmallIcon, CreateNonEmpty(), 0, 0 };
            }

            yield return new object[] { false, false, false, false, View.Details, null, 0, 0 };
            yield return new object[] { false, false, false, false, View.Details, new ImageList(), 0, 0 };
            yield return new object[] { false, false, false, false, View.Details, CreateNonEmpty(), 0, 0 };
            yield return new object[] { false, false, false, false, View.LargeIcon, null, 0, 0 };
            yield return new object[] { false, false, false, false, View.LargeIcon, new ImageList(), 0, 0 };
            yield return new object[] { false, false, false, false, View.LargeIcon, CreateNonEmpty(), 0, 0 };
            yield return new object[] { false, false, false, false, View.List, null, 0, 0 };
            yield return new object[] { false, false, false, false, View.List, new ImageList(), 0, 0 };
            yield return new object[] { false, false, false, false, View.List, CreateNonEmpty(), 0, 0 };
            yield return new object[] { false, false, false, false, View.SmallIcon, null, 0, 0 };
            yield return new object[] { false, false, false, false, View.SmallIcon, new ImageList(), 0, 0 };
            yield return new object[] { false, false, false, false, View.SmallIcon, CreateNonEmpty(), 0, 0 };
            yield return new object[] { false, false, false, false, View.Tile, null, 0, 0 };
            yield return new object[] { false, false, false, false, View.Tile, new ImageList(), 0, 0 };
            yield return new object[] { false, false, false, false, View.Tile, CreateNonEmpty(), 0, 0 };
        }

        [WinFormsTheory(Skip = "Leads to random AccessViolationException. See: https://github.com/dotnet/winforms/issues/3358")]
        [ActiveIssue("https://github.com/dotnet/winforms/issues/3358")]
        [MemberData(nameof(StateImageList_SetWithHandleWithNonNullOldValue_GetReturnsExpected))]
        public void ListView_StateImageList_SetWithHandleWithNonNullOldValue_GetReturnsExpected(bool useCompatibleStateImageBehavior, bool checkBoxes, bool autoArrange, bool virtualMode, View view, ImageList value, int expectedInvalidatedCallCount, int expectedCreatedCallCount)
        {
            using var imageList = new ImageList();
            using var listView = new ListView
            {
                UseCompatibleStateImageBehavior = useCompatibleStateImageBehavior,
                CheckBoxes = checkBoxes,
                AutoArrange = autoArrange,
                VirtualMode = virtualMode,
                View = view,
            };

            listView.StateImageList = imageList;

            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.StateImageList = value;
            Assert.Same(value, listView.StateImageList);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set same.
            listView.StateImageList = value;
            Assert.Same(value, listView.StateImageList);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        public void ListView_StateImageList_Dispose_DetachesFromListView(bool useCompatibleStateImageBehavior, bool autoArrange)
        {
            using var imageList1 = new ImageList();
            using var imageList2 = new ImageList();
            using var listView = new ListView
            {
                UseCompatibleStateImageBehavior = useCompatibleStateImageBehavior,
                AutoArrange = autoArrange,
                StateImageList = imageList1
            };
            Assert.Same(imageList1, listView.StateImageList);

            imageList1.Dispose();
            Assert.Null(listView.StateImageList);
            Assert.False(listView.IsHandleCreated);

            // Make sure we detached the setter.
            listView.StateImageList = imageList2;
            imageList1.Dispose();
            Assert.Same(imageList2, listView.StateImageList);
        }

        [WinFormsTheory]
        [InlineData(true, true, 0, 1, 1)]
        [InlineData(false, true, 1, 2, 3)]
        [InlineData(true, false, 0, 0, 0)]
        [InlineData(false, false, 0, 0, 0)]
        public void ListView_StateImageList_DisposeWithHandle_DetachesFromListView(bool useCompatibleStateImageBehavior, bool autoArrange, int expectedInvalidatedCallCount1, int expectedInvalidatedCallCount2, int expectedInvalidatedCallCount3)
        {
            using var imageList1 = new ImageList();
            using var imageList2 = new ImageList();
            using var listView = new ListView
            {
                UseCompatibleStateImageBehavior = useCompatibleStateImageBehavior,
                AutoArrange = autoArrange
            };
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.StateImageList = imageList1;
            Assert.Same(imageList1, listView.StateImageList);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            imageList1.Dispose();
            Assert.Null(listView.StateImageList);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Make sure we detached the setter.
            listView.StateImageList = imageList2;
            imageList1.Dispose();
            Assert.Same(imageList2, listView.StateImageList);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount3, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_UseCompatibleStateImageBehavior_Set_GetReturnsExpected(bool value)
        {
            using var listView = new ListView
            {
                UseCompatibleStateImageBehavior = value
            };
            Assert.Equal(value, listView.UseCompatibleStateImageBehavior);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.UseCompatibleStateImageBehavior = value;
            Assert.Equal(value, listView.UseCompatibleStateImageBehavior);
            Assert.False(listView.IsHandleCreated);

            // Set different.
            listView.UseCompatibleStateImageBehavior = !value;
            Assert.Equal(!value, listView.UseCompatibleStateImageBehavior);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_UseCompatibleStateImageBehavior_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.UseCompatibleStateImageBehavior = value;
            Assert.Equal(value, listView.UseCompatibleStateImageBehavior);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            listView.UseCompatibleStateImageBehavior = value;
            Assert.Equal(value, listView.UseCompatibleStateImageBehavior);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ListView_GetAutoSizeMode_Invoke_ReturnsExpected()
        {
            using var control = new SubListView();
            Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
        }

        [WinFormsFact]
        public void ListView_GetItemRect_InvokeWithoutHandle_ReturnsExpectedAndCreatedHandle()
        {
            using var control = new ListView();
            var item1 = new ListViewItem();
            var item2 = new ListViewItem();
            control.Items.Add(item1);
            control.Items.Add(item2);

            Rectangle rect1 = control.GetItemRect(0);
            Assert.True(rect1.X >= 0);
            Assert.True(rect1.Y >= 0);
            Assert.True(rect1.Width > 0);
            Assert.True(rect1.Height > 0);
            Assert.Equal(rect1, control.GetItemRect(0));
            Assert.True(control.IsHandleCreated);

            Rectangle rect2 = control.GetItemRect(1);
            Assert.True((rect2.X >= rect1.Right && rect2.Y == rect1.Y) || (rect2.X == rect1.X && rect2.Y >= rect1.Bottom));
            Assert.True(rect2.Width > 0);
            Assert.True(rect2.Height > 0);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListView_GetItemRect_InvokeWithHandle_ReturnsExpected()
        {
            using var control = new ListView();
            var item1 = new ListViewItem();
            var item2 = new ListViewItem();
            control.Items.Add(item1);
            control.Items.Add(item2);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Rectangle rect1 = control.GetItemRect(0);
            Assert.True(rect1.X >= 0);
            Assert.True(rect1.Y >= 0);
            Assert.True(rect1.Width > 0);
            Assert.True(rect1.Height > 0);
            Assert.Equal(rect1, control.GetItemRect(0));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            Rectangle rect2 = control.GetItemRect(1);
            Assert.True((rect2.X >= rect1.Right && rect2.Y == rect1.Y) || (rect2.X == rect1.X && rect2.Y >= rect1.Bottom));
            Assert.True(rect2.Width > 0);
            Assert.True(rect2.Height > 0);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> GetItemRect_InvokeCustomGetItemRect_TestData()
        {
            yield return new object[] { new RECT(), Rectangle.Empty };
            yield return new object[] { new RECT(1, 2, 3, 4), new Rectangle(1, 2, 2, 2) };
        }

        [WinFormsTheory]
        [MemberData(nameof(GetItemRect_InvokeCustomGetItemRect_TestData))]
        public void ListView_GetItemRect_InvokeCustomGetItemRect_ReturnsExpected(object getItemRectResult, Rectangle expected)
        {
            using var control = new CustomGetItemRectListView
            {
                GetItemRectResult = (RECT)getItemRectResult
            };
            var item = new ListViewItem();
            control.Items.Add(item);
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            Assert.Equal(expected, control.GetItemRect(0));
        }

        private class CustomGetItemRectListView : ListView
        {
            public RECT GetItemRectResult { get; set; }

            protected unsafe override void WndProc(ref Message m)
            {
                if (m.Msg == (int)LVM.GETITEMRECT)
                {
                    RECT* pRect = (RECT*)m.LParam;
                    *pRect = GetItemRectResult;
                    m.Result = (IntPtr)1;
                    return;
                }

                base.WndProc(ref m);
            }
        }

        [WinFormsFact]
        public void ListView_GetItemRect_InvokeInvalidGetItemRect_ThrowsArgumentOutOfRangeException()
        {
            using var control = new InvalidGetItemRectListView();
            var item = new ListViewItem();
            control.Items.Add(item);
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.MakeInvalid = true;
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRect(0));
        }

        private class InvalidGetItemRectListView : ListView
        {
            public bool MakeInvalid { get; set; }

            protected unsafe override void WndProc(ref Message m)
            {
                if (MakeInvalid && m.Msg == (int)LVM.GETITEMRECT)
                {
                    RECT* pRect = (RECT*)m.LParam;
                    *pRect = new RECT(1, 2, 3, 4);
                    m.Result = IntPtr.Zero;
                    return;
                }

                base.WndProc(ref m);
            }
        }

        [WinFormsFact]
        public void ListView_GetItemRect_InvalidIndexEmpty_ThrowsArgumentOutOfRangeException()
        {
            using var control = new ListView();
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRect(-1));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRect(0));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRect(1));
        }

        [WinFormsFact]
        public void ListView_GetItemRect_InvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException()
        {
            using var control = new ListView();
            var item1 = new ListViewItem();
            control.Items.Add(item1);

            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRect(-1));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRect(1));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRect(2));
        }

        [WinFormsFact]
        public void ListView_GetItemRect_InvalidIndexWithHandleEmpty_ThrowsArgumentOutOfRangeException()
        {
            using var control = new ListView();
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRect(-1));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRect(0));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRect(1));
        }

        [WinFormsFact]
        public void ListView_GetItemRect_InvalidIndexWithHandleNotEmpty_ThrowsArgumentOutOfRangeException()
        {
            using var control = new ListView();
            var item1 = new ListViewItem();
            control.Items.Add(item1);
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRect(-1));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRect(1));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRect(2));
        }

        [WinFormsTheory]
        [InlineData(ControlStyles.ContainerControl, false)]
        [InlineData(ControlStyles.UserPaint, false)]
        [InlineData(ControlStyles.Opaque, false)]
        [InlineData(ControlStyles.ResizeRedraw, false)]
        [InlineData(ControlStyles.FixedWidth, false)]
        [InlineData(ControlStyles.FixedHeight, false)]
        [InlineData(ControlStyles.StandardClick, false)]
        [InlineData(ControlStyles.Selectable, true)]
        [InlineData(ControlStyles.UserMouse, false)]
        [InlineData(ControlStyles.SupportsTransparentBackColor, false)]
        [InlineData(ControlStyles.StandardDoubleClick, true)]
        [InlineData(ControlStyles.AllPaintingInWmPaint, true)]
        [InlineData(ControlStyles.CacheText, false)]
        [InlineData(ControlStyles.EnableNotifyMessage, false)]
        [InlineData(ControlStyles.DoubleBuffer, false)]
        [InlineData(ControlStyles.OptimizedDoubleBuffer, false)]
        [InlineData(ControlStyles.UseTextForAccessibility, false)]
        [InlineData((ControlStyles)0, true)]
        [InlineData((ControlStyles)int.MaxValue, false)]
        [InlineData((ControlStyles)(-1), false)]
        public void ListView_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
        {
            using var control = new SubListView();
            Assert.Equal(expected, control.GetStyle(flag));

            // Call again to test caching.
            Assert.Equal(expected, control.GetStyle(flag));
        }

        [WinFormsFact]
        public void ListView_GetTopLevel_Invoke_ReturnsExpected()
        {
            using var control = new SubListView();
            Assert.False(control.GetTopLevel());
        }

        private static ImageList CreateNonEmpty()
        {
            var nonEmptyImageList = new ImageList();
            nonEmptyImageList.Images.Add(new Bitmap(10, 10));
            return nonEmptyImageList;
        }

        public static IEnumerable<object[]> ListView_InvokeOnSelectedIndexChanged_TestData()
        {
            foreach (View view in Enum.GetValues(typeof(View)))
            {
                foreach (bool showGrops in new[] { true, false })
                {
                    foreach (bool focused in new[] { true, false })
                    {
                        foreach (bool selected in new[] { true, false })
                        {
                            // Updating Focused property of ListViewItem always calls RaiseAutomatiomEvent.
                            // If ListViewItem is focused and selected then RaiseAutomatiomEvent is also called.
                            int expectedCallCount = focused && selected ? 2 : 1;
                            yield return new object[] { view, showGrops, focused, selected, expectedCallCount };
                        }
                    }
                };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ListView_InvokeOnSelectedIndexChanged_TestData))]
        public void ListView_OnSelectedIndexChanged_Invoke(View view, bool showGroups, bool focused, bool selected, int expectedCallCount)
        {
            using var listView = new SubListView
            {
                View = view,
                VirtualMode = false,
                ShowGroups = showGroups
            };

            listView.CreateControl();

            SubListViewItem testItem = new SubListViewItem("Test 1");

            listView.Items.Add(testItem);

            SubListViewItemAccessibleObject customAccessibleObject = new SubListViewItemAccessibleObject(testItem);
            testItem.CustomAccessibleObject = customAccessibleObject;

            listView.Items[0].Focused = focused;
            listView.Items[0].Selected = selected;

            Assert.Equal(expectedCallCount, customAccessibleObject?.RaiseAutomationEventCalls);
        }

        public static IEnumerable<object[]> ListView_InvokeOnSelectedIndexChanged_VirtualMode_TestData()
        {
            foreach (View view in Enum.GetValues(typeof(View)))
            {
                // View.Tile is not supported by ListView in virtual mode
                if (view == View.Tile)
                {
                    continue;
                }

                foreach (bool showGrops in new[] { true, false })
                {
                    foreach (bool focused in new[] { true, false })
                    {
                        foreach (bool selected in new[] { true, false })
                        {
                            // Updating Focused property of ListViewItem always calls RaiseAutomatiomEvent.
                            // If ListViewItem is focused and selected then RaiseAutomatiomEvent is also called.
                            int expectedCallCount = focused && selected ? 2 : 1;
                            yield return new object[] { view, showGrops, focused, selected, expectedCallCount };
                        }
                    }
                };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ListView_InvokeOnSelectedIndexChanged_VirtualMode_TestData))]
        public void ListView_OnSelectedIndexChanged_VirtualMode_Invoke(View view, bool showGroups, bool focused, bool selected, int expectedCallCount)
        {
            SubListViewItem listItem1 = new SubListViewItem("Test 1");

            using ListView listView = new ListView
            {
                View = view,
                VirtualMode = true,
                ShowGroups = showGroups,
                VirtualListSize = 1
            };

            listView.RetrieveVirtualItem += (s, e) =>
            {
                e.Item = e.ItemIndex switch
                {
                    0 => listItem1,
                    _ => throw new NotImplementedException()
                };
            };

            listView.CreateControl();
            listItem1.SetItemIndex(listView, 0);

            SubListViewItemAccessibleObject customAccessibleObject = new SubListViewItemAccessibleObject(listItem1);
            listItem1.CustomAccessibleObject = customAccessibleObject;

            listView.Items[0].Focused = focused;
            listView.Items[0].Selected = selected;

            Assert.Equal(expectedCallCount, customAccessibleObject?.RaiseAutomationEventCalls);
        }

        public static IEnumerable<object[]> ListView_Checkboxes_VirtualMode_Disabling_TestData()
        {
            foreach (View view in Enum.GetValues(typeof(View)))
            {
                // View.Tile is not supported by ListView in virtual mode
                if (view == View.Tile)
                {
                    continue;
                }

                foreach (bool showGroups in new[] { true, false })
                {
                    yield return new object[] { view, showGroups };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ListView_Checkboxes_VirtualMode_Disabling_TestData))]
        public void ListView_Checkboxes_VirtualMode_Disabling_ThrowException(View view, bool showGroups)
        {
            using var listView = new SubListView
            {
                View = view,
                VirtualMode = true,
                ShowGroups = showGroups
            };

            listView.CheckBoxes = true;
            Assert.Throws<InvalidOperationException>(() => listView.CheckBoxes = false);
        }

        private class SubListViewItem : ListViewItem
        {
            public AccessibleObject CustomAccessibleObject { get; set; }

            public SubListViewItem(string text) : base(text)
            {
            }

            internal override AccessibleObject AccessibilityObject => CustomAccessibleObject;
        }

        private class SubListViewItemAccessibleObject : ListViewItemAccessibleObject
        {
            public int RaiseAutomationEventCalls;

            public SubListViewItemAccessibleObject(ListViewItem owningItem) : base(owningItem, null)
            {
            }

            internal override bool RaiseAutomationEvent(UiaCore.UIA eventId)
            {
                RaiseAutomationEventCalls++;
                return base.RaiseAutomationEvent(eventId);
            }
        }

        [WinFormsFact]
        public void ListView_WmReflectNotify_LVN_KEYDOWN_WithoutGroups_and_CheckBoxes_DoesntHaveSelectedItems()
        {
            using var control = new ListView();
            control.Items.Add(new ListViewItem());
            control.Items.Add(new ListViewItem());
            control.CreateControl();
            User32.SendMessageW(control, User32.WM.KEYDOWN);
            Assert.Equal(0, control.SelectedItems.Count);
        }

        [WinFormsTheory]
        [InlineData(true, true, true)]
        [InlineData(true, false, true)]
        [InlineData(false, true, true)]
        [InlineData(false, false, true)]
        [InlineData(true, true, false)]
        [InlineData(true, false, false)]
        [InlineData(false, true, false)]
        [InlineData(false, false, false)]
        public unsafe void ListView_WmReflectNotify_LVN_KEYDOWN_SpaceKey_HasCheckBoxes_WithoutGroups_CheckedExpected(bool focusItem, bool checkItem, bool selectItems)
        {
            using var control = new ListView();
            control.CheckBoxes = true;
            ListViewItem item1 = new ListViewItem();
            item1.Text = "First";
            ListViewItem item2 = new ListViewItem();
            item2.Text = "Second";

            control.Items.Add(item1);
            control.Items.Add(item2);
            control.CreateControl();
            control.VirtualMode = false;

            item1.Focused = focusItem;
            item1.Checked = checkItem;
            item1.Selected = selectItems;
            item2.Selected = selectItems;

            // https://docs.microsoft.com/en-us/windows/win32/inputdev/wm-keydown?redirectedfrom=MSDN
            // The MSDN page tells us what bits of lParam to use for each of the parameters.
            // All we need to do is some bit shifting to assemble lParam
            // lParam = repeatCount | (scanCode << 16)
            // The source: https://stackoverflow.com/questions/21994276/setting-wm-keydown-lparam-parameters
            uint keyCode = (uint)Keys.Space;
            uint lParam = (0x00000001 | keyCode << 16);

            User32.SendMessageW(control, User32.WM.KEYDOWN, (IntPtr)keyCode, (IntPtr)lParam);
            Assert.Equal(selectItems ? 2 : 0, control.SelectedItems.Count);
            Assert.Equal(!checkItem && selectItems && focusItem, item2.Checked);
        }

        [WinFormsTheory]
        [InlineData(Keys.Down)]
        [InlineData(Keys.Up)]
        public unsafe void ListView_WmReflectNotify_LVN_KEYDOWN_WithGroups_WithoutSelection_DoesntFocusGroup(Keys key)
        {
            using var control = new ListView();
            ListViewItem item1 = new ListViewItem();
            item1.Text = "First";
            ListViewItem item2 = new ListViewItem();
            item2.Text = "Second";

            ListViewGroup group = new ListViewGroup("Test group");
            group.Items.Add(item1);
            group.Items.Add(item2);

            control.VirtualMode = false;
            control.Groups.Add(group);
            control.CreateControl();

            // https://docs.microsoft.com/en-us/windows/win32/inputdev/wm-keydown?redirectedfrom=MSDN
            // The MSDN page tells us what bits of lParam to use for each of the parameters.
            // All we need to do is some bit shifting to assemble lParam
            // lParam = repeatCount | (scanCode << 16)
            // The source: https://stackoverflow.com/questions/21994276/setting-wm-keydown-lparam-parameters
            uint keyCode = (uint)key;
            uint lParam = (0x00000001 | keyCode << 16);

            // If control doesn't have selected items noone will be focused.
            User32.SendMessageW(control, User32.WM.KEYDOWN, (IntPtr)keyCode, (IntPtr)lParam);
            Assert.Empty(control.SelectedIndices);
            Assert.Null(control.FocusedItem);
            Assert.Null(control.FocusedGroup);
        }

        [WinFormsTheory(Skip = "Crash with unexpected invokerHandle ExitCode")]
        [InlineData("Keys.Down", "2")]
        [InlineData("Keys.Up", "1")]
        public unsafe void ListView_WmReflectNotify_LVN_KEYDOWN_WithGroups_and_SelectedItems_FocusedGroupIsExpected(string keyString, string expectedGroupIndexString)
        {
            // Run this from another thread as we call Application.EnableVisualStyles.
            using RemoteInvokeHandle invokerHandle = RemoteExecutor.Invoke((key_s, expectedGroupIndex_s) =>
            {
                Application.EnableVisualStyles();

                using var control = new ListView();
                ListViewGroup group1 = new ListViewGroup("Test group1");
                ListViewGroup group2 = new ListViewGroup("Test group2");
                ListViewGroup group3 = new ListViewGroup("Test group3");
                ListViewItem item1 = new ListViewItem(group1);
                item1.Text = "First";
                ListViewItem item2 = new ListViewItem(group2);
                item2.Text = "Second";
                ListViewItem item3 = new ListViewItem(group3);
                item3.Text = "Third";
                control.Items.Add(item1);
                control.Items.Add(item2);
                control.Items.Add(item3);
                control.Groups.Add(group1);
                control.Groups.Add(group2);
                control.Groups.Add(group3);
                control.VirtualMode = false;
                control.CreateControl();

                item2.Selected = true;

                // https://docs.microsoft.com/en-us/windows/win32/inputdev/wm-keydown?redirectedfrom=MSDN
                // The MSDN page tells us what bits of lParam to use for each of the parameters.
                // All we need to do is some bit shifting to assemble lParam
                // lParam = repeatCount | (scanCode << 16)
                // The source: https://stackoverflow.com/questions/21994276/setting-wm-keydown-lparam-parameters
                uint keyCode = (uint)(key_s == "Keys.Down" ? Keys.Down : Keys.Up);
                uint lParam = (0x00000001 | keyCode << 16);

                User32.SendMessageW(control, User32.WM.KEYDOWN, (IntPtr)keyCode, (IntPtr)lParam);
                Assert.False(control.GroupsEnabled);
                Assert.True(control.Items.Count > 0);
                int expectedGroupIndex = int.Parse(expectedGroupIndex_s);
                Assert.Equal(control.Groups[expectedGroupIndex], control.FocusedGroup);
            }, keyString, expectedGroupIndexString);

            // verify the remote process succeeded
            Assert.Equal(0, invokerHandle.ExitCode);
        }

        [WinFormsTheory]
        [InlineData(Keys.Down)]
        [InlineData(Keys.Up)]
        public unsafe void ListView_VirtualMode_WmReflectNotify_LVN_KEYDOWN_WithGroups_DoenstFocusGroups(Keys key)
        {
            using ListView control = new ListView
            {
                ShowGroups = true,
                CheckBoxes = false,
                VirtualMode = true,
                VirtualListSize = 2 // we can't add items, just indicate how many we have
            };

            ListViewGroup group = new ListViewGroup("Test group");
            control.Groups.Add(group);

            control.RetrieveVirtualItem += (s, e) =>
            {
                e.Item = e.ItemIndex switch
                {
                    0 => new ListViewItem(group) { Selected = true },
                    _ => new ListViewItem(group),
                };
            };

            control.CreateControl();

            // https://docs.microsoft.com/en-us/windows/win32/inputdev/wm-keydown?redirectedfrom=MSDN
            // The MSDN page tells us what bits of lParam to use for each of the parameters.
            // All we need to do is some bit shifting to assemble lParam
            // lParam = repeatCount | (scanCode << 16)
            // The source: https://stackoverflow.com/questions/21994276/setting-wm-keydown-lparam-parameters
            uint keyCode = (uint)key;
            uint lParam = (0x00000001 | keyCode << 16);

            // Actually ListView in VirtualMode can't have Groups
            User32.SendMessageW(control, User32.WM.KEYDOWN, (IntPtr)keyCode, (IntPtr)lParam);
            Assert.Null(control.FocusedGroup);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public unsafe void ListView_VirtualMode_WmReflectNotify_LVN_KEYDOWN_EnabledCheckBoxes_WithoutGroups_DoenstCheckItems(bool checkedItem)
        {
            using ListView control = new ListView
            {
                ShowGroups = true,
                CheckBoxes = true,
                VirtualMode = true,
                VirtualListSize = 2 // we can't add items, just indicate how many we have
            };

            ListViewItem item1 = new ListViewItem();
            ListViewItem item2 = new ListViewItem();

            control.RetrieveVirtualItem += (s, e) =>
            {
                e.Item = e.ItemIndex switch
                {
                    0 => item1,
                    _ => item2,
                };
            };

            control.CreateControl();
            item1.Checked = checkedItem;
            item2.Checked = false;
            control.FocusedItem = item1;

            // https://docs.microsoft.com/en-us/windows/win32/inputdev/wm-keydown?redirectedfrom=MSDN
            // The MSDN page tells us what bits of lParam to use for each of the parameters.
            // All we need to do is some bit shifting to assemble lParam
            // lParam = repeatCount | (scanCode << 16)
            // The source: https://stackoverflow.com/questions/21994276/setting-wm-keydown-lparam-parameters
            uint keyCode = (uint)Keys.Space;
            uint lParam = (0x00000001 | keyCode << 16);

            // Actually ListView in VirtualMode doesn't check items here
            User32.SendMessageW(control, User32.WM.KEYDOWN, (IntPtr)keyCode, (IntPtr)lParam);
            Assert.False(item2.Checked);
        }

        public static IEnumerable<object[]> ListView_SelectedIndexies_Contains_Invoke_TestData()
        {
            foreach (bool virtualMode in new[] { true, false })
            {
                foreach (View view in Enum.GetValues(typeof(View)))
                {
                    // View.Tile is not supported by ListView in virtual mode
                    if (virtualMode == true && View.Tile == view)
                    {
                        continue;
                    }

                    foreach (bool showGroups in new[] { true, false })
                    {
                        foreach (bool createHandle in new[] { true, false })
                        {
                            yield return new object[] { view, showGroups, createHandle, virtualMode };
                        }
                    }
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ListView_SelectedIndexies_Contains_Invoke_TestData))]
        public void ListView_SelectedIndexies_Contains_Invoke_ReturnExpected(View view, bool showGroups, bool createHandle, bool virtualMode)
        {
            using ListView listView = new ListView
            {
                ShowGroups = showGroups,
                VirtualMode = virtualMode,
                View = view,
                VirtualListSize = 1
            };

            var listItem = new ListViewItem();

            if (virtualMode)
            {
                listView.RetrieveVirtualItem += (s, e) =>
                {
                    e.Item = e.ItemIndex switch
                    {
                        0 => listItem,
                        _ => throw new NotImplementedException()
                    };
                };
            }
            else
            {
                listView.Items.Add(listItem);
            }

            if (createHandle)
            {
                Assert.NotEqual(IntPtr.Zero, listView.Handle);
            }

            listView.Items[0].Selected = true;

            Assert.False(listView.SelectedIndices.Contains(-1));
            Assert.False(listView.SelectedIndices.Contains(1));
            Assert.True(listView.SelectedIndices.Contains(0));
            Assert.Equal(createHandle, listView.IsHandleCreated);
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

            public new bool ShowFocusCues => base.ShowFocusCues;

            public new bool ShowKeyboardCues => base.ShowKeyboardCues;

            public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

            public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

            public new bool GetTopLevel() => base.GetTopLevel();

            public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);
        }
    }
}
