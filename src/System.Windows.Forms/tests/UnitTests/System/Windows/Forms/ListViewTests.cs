// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using WinForms.Common.Tests;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    using Point = System.Drawing.Point;
    using Size = System.Drawing.Size;

    public class ListViewTests
    {
        public ListViewTests()
        {
            Application.ThreadException += (sender, e) => throw new Exception(e.Exception.StackTrace.ToString());
        }

        [WinFormsFact]
        public void ListView_Ctor_Default()
        {
            using var control = new SubListView();
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
            Assert.True(control.CanRaiseEvents);
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
            Assert.Null(control.FocusedItem);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(SystemColors.WindowText, control.ForeColor);
            Assert.False(control.FullRowSelect);
            Assert.False(control.GridLines);
            Assert.Empty(control.Groups);
            Assert.Same(control.Groups, control.Groups);
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
            Assert.Equal(0x56010148, createParams.Style);
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
            var control = new ListView
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
            var control = new ListView();
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
        public void ListView_BackColor_SetWithHandler_CallsBackColorChanged()
        {
            var control = new ListView();
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
            var control = new SubListView
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
            var control = new ListView();
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
            var control = new ListView();
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
                    foreach (ImageList imageList in new ImageList[] { new ImageList(), null })
                    {
                        foreach (bool value in new bool[] { true, false })
                        {
                            yield return new object[] { useCompatibleStateImageBehavior, View.Details, alignment, imageList, value };
                            yield return new object[] { useCompatibleStateImageBehavior, View.LargeIcon, alignment, imageList, value };
                            yield return new object[] { useCompatibleStateImageBehavior, View.List, alignment, imageList, value };
                            yield return new object[] { useCompatibleStateImageBehavior, View.SmallIcon, alignment, imageList, value };
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
                foreach (ImageList imageList in new ImageList[] { new ImageList(), null })
                {
                    yield return new object[] { true, View.Details, alignment, imageList, true, 1, 0, 2, 0 };
                    yield return new object[] { true, View.LargeIcon, alignment, imageList, true, 1, 0, 2, 0 };
                    yield return new object[] { true, View.List, alignment, imageList, true, 1, 0, 2, 0 };
                    yield return new object[] { true, View.SmallIcon, alignment, imageList, true, 1, 0, 2, 0 };
                    yield return new object[] { true, View.Details, alignment, imageList, false, 0, 0, 1, 0 };
                    yield return new object[] { true, View.LargeIcon, alignment, imageList, false, 0, 0, 1, 0 };
                    yield return new object[] { true, View.List, alignment, imageList, false, 0, 0, 1, 0 };
                    yield return new object[] { true, View.SmallIcon, alignment, imageList, false, 0, 0, 1, 0 };
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

            foreach (ImageList imageList in new ImageList[] { new ImageList(), null })
            {
                yield return new object[] { false, View.Details, ListViewAlignment.Left, imageList, true, 1, 0, 2, 1 };
                yield return new object[] { false, View.LargeIcon, ListViewAlignment.Left, imageList, true, 2, 1, 4, 2 };
                yield return new object[] { false, View.List, ListViewAlignment.Left, imageList, true, 1, 1, 2, 2 };
                yield return new object[] { false, View.SmallIcon, ListViewAlignment.Left, imageList, true, 2, 1, 4, 2 };
                yield return new object[] { false, View.Details, ListViewAlignment.Left, imageList, false, 0, 0, 1, 0 };
                yield return new object[] { false, View.LargeIcon, ListViewAlignment.Left, imageList, false, 0, 0, 2, 1 };
                yield return new object[] { false, View.List, ListViewAlignment.Left, imageList, false, 0, 0, 1, 1 };
                yield return new object[] { false, View.SmallIcon, ListViewAlignment.Left, imageList, false, 0, 0, 2, 1 };
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

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_DoubleBuffered_Get_ReturnsExpected(bool value)
        {
            var control = new SubListView();
            control.SetStyle(ControlStyles.OptimizedDoubleBuffer, value);
            Assert.Equal(value, control.DoubleBuffered);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListView_DoubleBuffered_Set_GetReturnsExpected(bool value)
        {
            var control = new SubListView
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
            var control = new SubListView();
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
            var control = new SubListView
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
            var control = new SubListView();
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
            var control = new SubListView();
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
            var control = new SubListView();
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
            var control = new ListView
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
            var control = new ListView();
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
        public void ListView_ForeColor_SetWithHandler_CallsForeColorChanged()
        {
            var control = new ListView();
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
            var nonEmptyImageList = new ImageList();
            nonEmptyImageList.Images.Add(new Bitmap(10, 10));

            foreach (bool autoArrange in new bool[] { true, false })
            {
                foreach (bool virtualMode in new bool[] { true, false })
                {
                    foreach (View view in new View[] { View.Details, View.LargeIcon, View.List, View.SmallIcon })
                    {
                        yield return new object[] { autoArrange, virtualMode, view, null };
                        yield return new object[] { autoArrange, virtualMode, view, new ImageList() };
                        yield return new object[] { autoArrange, virtualMode, view, nonEmptyImageList };
                    }
                }

                yield return new object[] { autoArrange, false, View.Tile, null };
                yield return new object[] { autoArrange, false, View.Tile, new ImageList() };
                yield return new object[] { autoArrange, false, View.Tile, nonEmptyImageList };
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
            using var listView = new ListView
            {
                AutoArrange = autoArrange,
                VirtualMode = virtualMode,
                View = view,
                LargeImageList = new ImageList()
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
            var nonEmptyImageList = new ImageList();
            nonEmptyImageList.Images.Add(new Bitmap(10, 10));

            yield return new object[] { true, false, View.Details, null, 0 };
            yield return new object[] { true, false, View.Details, new ImageList(), 0 };
            yield return new object[] { true, false, View.Details, nonEmptyImageList, 0 };
            yield return new object[] { true, false, View.LargeIcon, null, 0 };
            yield return new object[] { true, false, View.LargeIcon, new ImageList(), 1 };
            yield return new object[] { true, false, View.LargeIcon, nonEmptyImageList, 1 };
            yield return new object[] { true, false, View.List, null, 0 };
            yield return new object[] { true, false, View.List, new ImageList(), 0 };
            yield return new object[] { true, false, View.List, nonEmptyImageList, 0 };
            yield return new object[] { true, false, View.SmallIcon, null, 0 };
            yield return new object[] { true, false, View.SmallIcon, new ImageList(), 1 };
            yield return new object[] { true, false, View.SmallIcon, nonEmptyImageList, 1 };
            yield return new object[] { true, false, View.Tile, null, 0 };
            yield return new object[] { true, false, View.Tile, new ImageList(), 0 };
            yield return new object[] { true, false, View.Tile, nonEmptyImageList, 0 };
            
            foreach (bool autoArrange in new bool[] { true, false })
            {
                yield return new object[] { autoArrange, true, View.Details, null, 0 };
                yield return new object[] { autoArrange, true, View.Details, new ImageList(), 0 };
                yield return new object[] { autoArrange, true, View.Details, nonEmptyImageList, 0 };
                yield return new object[] { autoArrange, true, View.LargeIcon, null, 0 };
                yield return new object[] { autoArrange, true, View.LargeIcon, new ImageList(), 0 };
                yield return new object[] { autoArrange, true, View.LargeIcon, nonEmptyImageList, 0 };
                yield return new object[] { autoArrange, true, View.List, null, 0 };
                yield return new object[] { autoArrange, true, View.List, new ImageList(), 0 };
                yield return new object[] { autoArrange, true, View.List, nonEmptyImageList, 0 };
                yield return new object[] { autoArrange, true, View.SmallIcon, null, 0 };
                yield return new object[] { autoArrange, true, View.SmallIcon, new ImageList(), 0 };
                yield return new object[] { autoArrange, true, View.SmallIcon, nonEmptyImageList, 0 };
            }
            
            yield return new object[] { false, false, View.Details, null, 0 };
            yield return new object[] { false, false, View.Details, new ImageList(), 0 };
            yield return new object[] { false, false, View.Details, nonEmptyImageList, 0 };
            yield return new object[] { false, false, View.LargeIcon, null, 0 };
            yield return new object[] { false, false, View.LargeIcon, new ImageList(), 0 };
            yield return new object[] { false, false, View.LargeIcon, nonEmptyImageList, 0 };
            yield return new object[] { false, false, View.List, null, 0 };
            yield return new object[] { false, false, View.List, new ImageList(), 0 };
            yield return new object[] { false, false, View.List, nonEmptyImageList, 0 };
            yield return new object[] { false, false, View.SmallIcon, null, 0 };
            yield return new object[] { false, false, View.SmallIcon, new ImageList(), 0 };
            yield return new object[] { false, false, View.SmallIcon, nonEmptyImageList, 0 };
            yield return new object[] { false, false, View.Tile, null, 0 };
            yield return new object[] { false, false, View.Tile, new ImageList(), 0 };
            yield return new object[] { false, false, View.Tile, nonEmptyImageList, 0 };
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
            var nonEmptyImageList = new ImageList();
            nonEmptyImageList.Images.Add(new Bitmap(10, 10));

            yield return new object[] { true, false, View.Details, null, 0 };
            yield return new object[] { true, false, View.Details, new ImageList(), 0 };
            yield return new object[] { true, false, View.Details, nonEmptyImageList, 0 };
            yield return new object[] { true, false, View.LargeIcon, null, 1 };
            yield return new object[] { true, false, View.LargeIcon, new ImageList(), 1 };
            yield return new object[] { true, false, View.LargeIcon, nonEmptyImageList, 1 };
            yield return new object[] { true, false, View.List, null, 0 };
            yield return new object[] { true, false, View.List, new ImageList(), 0 };
            yield return new object[] { true, false, View.List, nonEmptyImageList, 0 };
            yield return new object[] { true, false, View.SmallIcon, null, 1 };
            yield return new object[] { true, false, View.SmallIcon, new ImageList(), 1 };
            yield return new object[] { true, false, View.SmallIcon, nonEmptyImageList, 1 };
            yield return new object[] { true, false, View.Tile, null, 0 };
            yield return new object[] { true, false, View.Tile, new ImageList(), 0 };
            yield return new object[] { true, false, View.Tile, nonEmptyImageList, 0 };
            
            foreach (bool autoArrange in new bool[] { true, false })
            {
                yield return new object[] { autoArrange, true, View.Details, null, 0 };
                yield return new object[] { autoArrange, true, View.Details, new ImageList(), 0 };
                yield return new object[] { autoArrange, true, View.Details, nonEmptyImageList, 0 };
                yield return new object[] { autoArrange, true, View.LargeIcon, null, 0 };
                yield return new object[] { autoArrange, true, View.LargeIcon, new ImageList(), 0 };
                yield return new object[] { autoArrange, true, View.LargeIcon, nonEmptyImageList, 0 };
                yield return new object[] { autoArrange, true, View.List, null, 0 };
                yield return new object[] { autoArrange, true, View.List, new ImageList(), 0 };
                yield return new object[] { autoArrange, true, View.List, nonEmptyImageList, 0 };
                yield return new object[] { autoArrange, true, View.SmallIcon, null, 0 };
                yield return new object[] { autoArrange, true, View.SmallIcon, new ImageList(), 0 };
                yield return new object[] { autoArrange, true, View.SmallIcon, nonEmptyImageList, 0 };
            }
            
            yield return new object[] { false, false, View.Details, null, 0 };
            yield return new object[] { false, false, View.Details, new ImageList(), 0 };
            yield return new object[] { false, false, View.Details, nonEmptyImageList, 0 };
            yield return new object[] { false, false, View.LargeIcon, null, 0 };
            yield return new object[] { false, false, View.LargeIcon, new ImageList(), 0 };
            yield return new object[] { false, false, View.LargeIcon, nonEmptyImageList, 0 };
            yield return new object[] { false, false, View.List, null, 0 };
            yield return new object[] { false, false, View.List, new ImageList(), 0 };
            yield return new object[] { false, false, View.List, nonEmptyImageList, 0 };
            yield return new object[] { false, false, View.SmallIcon, null, 0 };
            yield return new object[] { false, false, View.SmallIcon, new ImageList(), 0 };
            yield return new object[] { false, false, View.SmallIcon, nonEmptyImageList, 0 };
            yield return new object[] { false, false, View.Tile, null, 0 };
            yield return new object[] { false, false, View.Tile, new ImageList(), 0 };
            yield return new object[] { false, false, View.Tile, nonEmptyImageList, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(LargeImageList_SetWithHandleWithNonNullOldValue_GetReturnsExpected))]
        public void ListView_LargeImageList_SetWithHandleWithNonNullOldValue_GetReturnsExpected(bool autoArrange, bool virtualMode, View view, ImageList value, int expectedInvalidatedCallCount)
        {
            using var listView = new ListView
            {
                AutoArrange = autoArrange,
                VirtualMode = virtualMode,
                View = view,
                LargeImageList = new ImageList()
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
            var nonEmptyImageList = new ImageList();
            nonEmptyImageList.Images.Add(new Bitmap(10, 10));

            foreach (bool autoArrange in new bool[] { true, false })
            {
                foreach (bool virtualMode in new bool[] { true, false })
                {
                    foreach (View view in new View[] { View.Details, View.LargeIcon, View.List, View.SmallIcon })
                    {
                        yield return new object[] { autoArrange, virtualMode, view, null };
                        yield return new object[] { autoArrange, virtualMode, view, new ImageList() };
                        yield return new object[] { autoArrange, virtualMode, view, nonEmptyImageList };
                    }
                }

                yield return new object[] { autoArrange, false, View.Tile, null };
                yield return new object[] { autoArrange, false, View.Tile, new ImageList() };
                yield return new object[] { autoArrange, false, View.Tile, nonEmptyImageList };
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
            using var listView = new ListView
            {
                AutoArrange = autoArrange,
                VirtualMode = virtualMode,
                View = view,
                SmallImageList = new ImageList()
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
            var nonEmptyImageList = new ImageList();
            nonEmptyImageList.Images.Add(new Bitmap(10, 10));

            yield return new object[] { true, false, View.Details, null, 0, 0 };
            yield return new object[] { true, false, View.Details, new ImageList(), 1, 0 };
            yield return new object[] { true, false, View.Details, nonEmptyImageList, 1, 0 };
            yield return new object[] { true, false, View.LargeIcon, null, 0, 0 };
            yield return new object[] { true, false, View.LargeIcon, new ImageList(), 1, 0 };
            yield return new object[] { true, false, View.LargeIcon, nonEmptyImageList, 1, 0 };
            yield return new object[] { true, false, View.List, null, 0, 0 };
            yield return new object[] { true, false, View.List, new ImageList(), 0, 0 };
            yield return new object[] { true, false, View.List, nonEmptyImageList, 0, 0 };
            yield return new object[] { true, false, View.SmallIcon, null, 0, 0 };
            yield return new object[] { true, false, View.SmallIcon, new ImageList(), 4, 2 };
            yield return new object[] { true, false, View.SmallIcon, nonEmptyImageList, 4, 2 };
            yield return new object[] { true, false, View.Tile, null, 0, 0 };
            yield return new object[] { true, false, View.Tile, new ImageList(), 0, 0 };
            yield return new object[] { true, false, View.Tile, nonEmptyImageList, 0, 0 };
            
            foreach (bool autoArrange in new bool[] { true, false })
            {
                yield return new object[] { autoArrange, true, View.Details, null, 0, 0 };
                yield return new object[] { autoArrange, true, View.Details, new ImageList(), 1, 0 };
                yield return new object[] { autoArrange, true, View.Details, nonEmptyImageList, 1, 0 };
                yield return new object[] { autoArrange, true, View.LargeIcon, null, 0, 0 };
                yield return new object[] { autoArrange, true, View.LargeIcon, new ImageList(), 0, 0 };
                yield return new object[] { autoArrange, true, View.LargeIcon, nonEmptyImageList, 0, 0 };
                yield return new object[] { autoArrange, true, View.List, null, 0, 0 };
                yield return new object[] { autoArrange, true, View.List, new ImageList(), 0, 0 };
                yield return new object[] { autoArrange, true, View.List, nonEmptyImageList, 0, 0 };
                yield return new object[] { autoArrange, true, View.SmallIcon, null, 0, 0 };
                yield return new object[] { autoArrange, true, View.SmallIcon, new ImageList(), 2, 2 };
                yield return new object[] { autoArrange, true, View.SmallIcon, nonEmptyImageList, 2, 2 };
            }
            
            yield return new object[] { false, false, View.Details, null, 0, 0 };
            yield return new object[] { false, false, View.Details, new ImageList(), 1, 0 };
            yield return new object[] { false, false, View.Details, nonEmptyImageList, 1, 0 };
            yield return new object[] { false, false, View.LargeIcon, null, 0, 0 };
            yield return new object[] { false, false, View.LargeIcon, new ImageList(), 0, 0 };
            yield return new object[] { false, false, View.LargeIcon, nonEmptyImageList, 0, 0 };
            yield return new object[] { false, false, View.List, null, 0, 0 };
            yield return new object[] { false, false, View.List, new ImageList(), 0, 0 };
            yield return new object[] { false, false, View.List, nonEmptyImageList, 0, 0 };
            yield return new object[] { false, false, View.SmallIcon, null, 0, 0 };
            yield return new object[] { false, false, View.SmallIcon, new ImageList(), 2, 2 };
            yield return new object[] { false, false, View.SmallIcon, nonEmptyImageList, 2, 2 };
            yield return new object[] { false, false, View.Tile, null, 0, 0 };
            yield return new object[] { false, false, View.Tile, new ImageList(), 0, 0 };
            yield return new object[] { false, false, View.Tile, nonEmptyImageList, 0, 0 };
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
            var nonEmptyImageList = new ImageList();
            nonEmptyImageList.Images.Add(new Bitmap(10, 10));

            yield return new object[] { true, false, View.Details, null, 1, 0 };
            yield return new object[] { true, false, View.Details, new ImageList(), 1, 0 };
            yield return new object[] { true, false, View.Details, nonEmptyImageList, 1, 0 };
            yield return new object[] { true, false, View.LargeIcon, null, 1, 0 };
            yield return new object[] { true, false, View.LargeIcon, new ImageList(), 1, 0 };
            yield return new object[] { true, false, View.LargeIcon, nonEmptyImageList, 1, 0 };
            yield return new object[] { true, false, View.List, null, 0, 0 };
            yield return new object[] { true, false, View.List, new ImageList(), 0, 0 };
            yield return new object[] { true, false, View.List, nonEmptyImageList, 0, 0 };
            yield return new object[] { true, false, View.SmallIcon, null, 4, 2 };
            yield return new object[] { true, false, View.SmallIcon, new ImageList(), 4, 2 };
            yield return new object[] { true, false, View.SmallIcon, nonEmptyImageList, 4, 2 };
            yield return new object[] { true, false, View.Tile, null, 0, 0 };
            yield return new object[] { true, false, View.Tile, new ImageList(), 0, 0 };
            yield return new object[] { true, false, View.Tile, nonEmptyImageList, 0, 0 };
            
            foreach (bool autoArrange in new bool[] { true, false })
            {
                yield return new object[] { autoArrange, true, View.Details, null, 1, 0 };
                yield return new object[] { autoArrange, true, View.Details, new ImageList(), 1, 0 };
                yield return new object[] { autoArrange, true, View.Details, nonEmptyImageList, 1, 0 };
                yield return new object[] { autoArrange, true, View.LargeIcon, null, 0, 0 };
                yield return new object[] { autoArrange, true, View.LargeIcon, new ImageList(), 0, 0 };
                yield return new object[] { autoArrange, true, View.LargeIcon, nonEmptyImageList, 0, 0 };
                yield return new object[] { autoArrange, true, View.List, null, 0, 0 };
                yield return new object[] { autoArrange, true, View.List, new ImageList(), 0, 0 };
                yield return new object[] { autoArrange, true, View.List, nonEmptyImageList, 0, 0 };
                yield return new object[] { autoArrange, true, View.SmallIcon, null, 2, 2 };
                yield return new object[] { autoArrange, true, View.SmallIcon, new ImageList(), 2, 2 };
                yield return new object[] { autoArrange, true, View.SmallIcon, nonEmptyImageList, 2, 2 };
            }
            
            yield return new object[] { false, false, View.Details, null, 1, 0 };
            yield return new object[] { false, false, View.Details, new ImageList(), 1, 0 };
            yield return new object[] { false, false, View.Details, nonEmptyImageList, 1, 0 };
            yield return new object[] { false, false, View.LargeIcon, null, 0, 0 };
            yield return new object[] { false, false, View.LargeIcon, new ImageList(), 0, 0 };
            yield return new object[] { false, false, View.LargeIcon, nonEmptyImageList, 0, 0 };
            yield return new object[] { false, false, View.List, null, 0, 0 };
            yield return new object[] { false, false, View.List, new ImageList(), 0, 0 };
            yield return new object[] { false, false, View.List, nonEmptyImageList, 0, 0 };
            yield return new object[] { false, false, View.SmallIcon, null, 2, 2 };
            yield return new object[] { false, false, View.SmallIcon, new ImageList(), 2, 2 };
            yield return new object[] { false, false, View.SmallIcon, nonEmptyImageList, 2, 2 };
            yield return new object[] { false, false, View.Tile, null, 0, 0 };
            yield return new object[] { false, false, View.Tile, new ImageList(), 0, 0 };
            yield return new object[] { false, false, View.Tile, nonEmptyImageList, 0, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(SmallImageList_SetWithHandleWithNonNullOldValue_GetReturnsExpected))]
        public void ListView_SmallImageList_SetWithHandleWithNonNullOldValue_GetReturnsExpected(bool autoArrange, bool virtualMode, View view, ImageList value, int expectedInvalidatedCallCount, int expectedStyleChangedCallCount)
        {
            using var listView = new ListView
            {
                AutoArrange = autoArrange,
                VirtualMode = virtualMode,
                View = view,
                SmallImageList = new ImageList()
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
            var nonEmptyImageList = new ImageList();
            nonEmptyImageList.Images.Add(new Bitmap(10, 10));

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
                                yield return new object[] { useCompatibleStateImageBehavior, checkBoxes, autoArrange, virtualMode, view, nonEmptyImageList };
                            }
                        }
                    }
                }

                yield return new object[] { useCompatibleStateImageBehavior, false, true, false, View.Tile, null };
                yield return new object[] { useCompatibleStateImageBehavior, false, true, false, View.Tile, new ImageList() };
                yield return new object[] { useCompatibleStateImageBehavior, false, true, false, View.Tile, nonEmptyImageList };

                yield return new object[] { useCompatibleStateImageBehavior, false, false, false, View.Tile, null };
                yield return new object[] { useCompatibleStateImageBehavior, false, false, false, View.Tile, new ImageList() };
                yield return new object[] { useCompatibleStateImageBehavior, false, false, false, View.Tile, nonEmptyImageList };
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
            using var listView = new ListView
            {
                UseCompatibleStateImageBehavior = useCompatibleStateImageBehavior,
                CheckBoxes = checkBoxes,
                AutoArrange = autoArrange,
                VirtualMode = virtualMode,
                View = view,
                StateImageList = new ImageList()
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
            var nonEmptyImageList = new ImageList();
            nonEmptyImageList.Images.Add(new Bitmap(10, 10));

            // UseCompatibleStateImageBehavior true
            foreach (bool checkBoxes in new bool[] { true, false })
            {
                yield return new object[] { true, checkBoxes, true, false, View.Details, null, 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.Details, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.Details, nonEmptyImageList, 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.LargeIcon, null, 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.LargeIcon, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.LargeIcon, nonEmptyImageList, 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.List, null, 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.List, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.List, nonEmptyImageList, 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.SmallIcon, null, 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.SmallIcon, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.SmallIcon, nonEmptyImageList, 0, 0 };

                foreach (bool autoArrange in new bool[] { true, false })
                {
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.Details, null, 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.Details, new ImageList(), 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.Details, nonEmptyImageList, 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.LargeIcon, null, 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.LargeIcon, new ImageList(), 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.LargeIcon, nonEmptyImageList, 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.List, null, 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.List, new ImageList(), 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.List, nonEmptyImageList, 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.SmallIcon, null, 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.SmallIcon, new ImageList(), 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.SmallIcon, nonEmptyImageList, 0, 0 };
                }
                
                yield return new object[] { true, checkBoxes, false, false, View.Details, null, 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.Details, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.Details, nonEmptyImageList, 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.LargeIcon, null, 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.LargeIcon, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.LargeIcon, nonEmptyImageList, 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.List, null, 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.List, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.List, nonEmptyImageList, 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.SmallIcon, null, 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.SmallIcon, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.SmallIcon, nonEmptyImageList, 0, 0 };
            }

            yield return new object[] { true, false, true, false, View.Tile, null, 0, 0 };
            yield return new object[] { true, false, true, false, View.Tile, new ImageList(), 0, 0 };
            yield return new object[] { true, false, true, false, View.Tile, nonEmptyImageList, 0, 0 };
            
            yield return new object[] { true, false, false, false, View.Tile, null, 0, 0 };
            yield return new object[] { true, false, false, false, View.Tile, new ImageList(), 0, 0 };
            yield return new object[] { true, false, false, false, View.Tile, nonEmptyImageList, 0, 0 };

            // UseCompatibleStateImageBehavior false, CheckBoxes true
            yield return new object[] { false, true, true, false, View.Details, null, 0, 0 };
            yield return new object[] { false, true, true, false, View.Details, new ImageList(), 1, 1 };
            yield return new object[] { false, true, true, false, View.Details, nonEmptyImageList, 1, 1 };
            yield return new object[] { false, true, true, false, View.LargeIcon, null, 0, 0 };
            yield return new object[] { false, true, true, false, View.LargeIcon, new ImageList(), 3, 1 };
            yield return new object[] { false, true, true, false, View.LargeIcon, nonEmptyImageList, 3, 1 };
            yield return new object[] { false, true, true, false, View.List, null, 0, 0 };
            yield return new object[] { false, true, true, false, View.List, new ImageList(), 1, 1 };
            yield return new object[] { false, true, true, false, View.List, nonEmptyImageList, 1, 1 };
            yield return new object[] { false, true, true, false, View.SmallIcon, null, 0, 0 };
            yield return new object[] { false, true, true, false, View.SmallIcon, new ImageList(), 3, 1 };
            yield return new object[] { false, true, true, false, View.SmallIcon, nonEmptyImageList, 3, 1 };

            foreach (bool autoArrange in new bool[] { true, false })
            {
                yield return new object[] { false, true, autoArrange, true, View.Details, null, 0, 0 };
                yield return new object[] { false, true, autoArrange, true, View.Details, new ImageList(), 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.Details, nonEmptyImageList, 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.LargeIcon, null, 0, 0 };
                yield return new object[] { false, true, autoArrange, true, View.LargeIcon, new ImageList(), 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.LargeIcon, nonEmptyImageList, 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.List, null, 0, 0 };
                yield return new object[] { false, true, autoArrange, true, View.List, new ImageList(), 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.List, nonEmptyImageList, 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.SmallIcon, null, 0, 0 };
                yield return new object[] { false, true, autoArrange, true, View.SmallIcon, new ImageList(), 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.SmallIcon, nonEmptyImageList, 1, 1 };
            }
            
            yield return new object[] { false, true, false, false, View.Details, null, 0, 0 };
            yield return new object[] { false, true, false, false, View.Details, new ImageList(), 1, 1 };
            yield return new object[] { false, true, false, false, View.Details, nonEmptyImageList, 1, 1 };
            yield return new object[] { false, true, false, false, View.LargeIcon, null, 0, 0 };
            yield return new object[] { false, true, false, false, View.LargeIcon, new ImageList(), 1, 1 };
            yield return new object[] { false, true, false, false, View.LargeIcon, nonEmptyImageList, 1, 1 };
            yield return new object[] { false, true, false, false, View.List, null, 0, 0 };
            yield return new object[] { false, true, false, false, View.List, new ImageList(), 1, 1 };
            yield return new object[] { false, true, false, false, View.List, nonEmptyImageList, 1, 1 };
            yield return new object[] { false, true, false, false, View.SmallIcon, null, 0, 0 };
            yield return new object[] { false, true, false, false, View.SmallIcon, new ImageList(), 1, 1 };
            yield return new object[] { false, true, false, false, View.SmallIcon, nonEmptyImageList, 1, 1 };

            // UseCompatibleStateImageBehavior false, CheckBoxes false
            yield return new object[] { false, false, true, false, View.Details, null, 0, 0 };
            yield return new object[] { false, false, true, false, View.Details, new ImageList(), 0, 0 };
            yield return new object[] { false, false, true, false, View.Details, nonEmptyImageList, 0, 0 };
            yield return new object[] { false, false, true, false, View.LargeIcon, null, 0, 0 };
            yield return new object[] { false, false, true, false, View.LargeIcon, new ImageList(), 1, 0 };
            yield return new object[] { false, false, true, false, View.LargeIcon, nonEmptyImageList, 1, 0 };
            yield return new object[] { false, false, true, false, View.List, null, 0, 0 };
            yield return new object[] { false, false, true, false, View.List, new ImageList(), 0, 0 };
            yield return new object[] { false, false, true, false, View.List, nonEmptyImageList, 0, 0 };
            yield return new object[] { false, false, true, false, View.SmallIcon, null, 0, 0 };
            yield return new object[] { false, false, true, false, View.SmallIcon, new ImageList(), 1, 0 };
            yield return new object[] { false, false, true, false, View.SmallIcon, nonEmptyImageList, 1, 0 };
            yield return new object[] { false, false, true, false, View.Tile, null, 0, 0 };
            yield return new object[] { false, false, true, false, View.Tile, new ImageList(), 0, 0 };
            yield return new object[] { false, false, true, false, View.Tile, nonEmptyImageList, 0, 0 };
            
            foreach (bool autoArrange in new bool[] { true, false })
            {
                yield return new object[] { false, false, autoArrange, true, View.Details, null, 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.Details, new ImageList(), 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.Details, nonEmptyImageList, 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.LargeIcon, null, 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.LargeIcon, new ImageList(), 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.LargeIcon, nonEmptyImageList, 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.List, null, 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.List, new ImageList(), 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.List, nonEmptyImageList, 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.SmallIcon, null, 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.SmallIcon, new ImageList(), 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.SmallIcon, nonEmptyImageList, 0, 0 };
            }
            
            yield return new object[] { false, false, false, false, View.Details, null, 0, 0 };
            yield return new object[] { false, false, false, false, View.Details, new ImageList(), 0, 0 };
            yield return new object[] { false, false, false, false, View.Details, nonEmptyImageList, 0, 0 };
            yield return new object[] { false, false, false, false, View.LargeIcon, null, 0, 0 };
            yield return new object[] { false, false, false, false, View.LargeIcon, new ImageList(), 0, 0 };
            yield return new object[] { false, false, false, false, View.LargeIcon, nonEmptyImageList, 0, 0 };
            yield return new object[] { false, false, false, false, View.List, null, 0, 0 };
            yield return new object[] { false, false, false, false, View.List, new ImageList(), 0, 0 };
            yield return new object[] { false, false, false, false, View.List, nonEmptyImageList, 0, 0 };
            yield return new object[] { false, false, false, false, View.SmallIcon, null, 0, 0 };
            yield return new object[] { false, false, false, false, View.SmallIcon, new ImageList(), 0, 0 };
            yield return new object[] { false, false, false, false, View.SmallIcon, nonEmptyImageList, 0, 0 };
            yield return new object[] { false, false, false, false, View.Tile, null, 0, 0 };
            yield return new object[] { false, false, false, false, View.Tile, new ImageList(), 0, 0 };
            yield return new object[] { false, false, false, false, View.Tile, nonEmptyImageList, 0, 0 };
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
            var nonEmptyImageList = new ImageList();
            nonEmptyImageList.Images.Add(new Bitmap(10, 10));

            // UseCompatibleStateImageBehavior true
            foreach (bool checkBoxes in new bool[] { true, false })
            {
                yield return new object[] { true, checkBoxes, true, false, View.Details, null, 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.Details, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.Details, nonEmptyImageList, 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.LargeIcon, null, 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.LargeIcon, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.LargeIcon, nonEmptyImageList, 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.List, null, 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.List, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.List, nonEmptyImageList, 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.SmallIcon, null, 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.SmallIcon, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, true, false, View.SmallIcon, nonEmptyImageList, 0, 0 };

                foreach (bool autoArrange in new bool[] { true, false })
                {
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.Details, null, 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.Details, new ImageList(), 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.Details, nonEmptyImageList, 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.LargeIcon, null, 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.LargeIcon, new ImageList(), 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.LargeIcon, nonEmptyImageList, 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.List, null, 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.List, new ImageList(), 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.List, nonEmptyImageList, 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.SmallIcon, null, 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.SmallIcon, new ImageList(), 0, 0 };
                    yield return new object[] { true, checkBoxes, autoArrange, true, View.SmallIcon, nonEmptyImageList, 0, 0 };
                }
                
                yield return new object[] { true, checkBoxes, false, false, View.Details, null, 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.Details, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.Details, nonEmptyImageList, 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.LargeIcon, null, 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.LargeIcon, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.LargeIcon, nonEmptyImageList, 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.List, null, 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.List, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.List, nonEmptyImageList, 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.SmallIcon, null, 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.SmallIcon, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, false, false, View.SmallIcon, nonEmptyImageList, 0, 0 };
            }

            yield return new object[] { true, false, true, false, View.Tile, null, 0, 0 };
            yield return new object[] { true, false, true, false, View.Tile, new ImageList(), 0, 0 };
            yield return new object[] { true, false, true, false, View.Tile, nonEmptyImageList, 0, 0 };
            
            yield return new object[] { true, false, false, false, View.Tile, null, 0, 0 };
            yield return new object[] { true, false, false, false, View.Tile, new ImageList(), 0, 0 };
            yield return new object[] { true, false, false, false, View.Tile, nonEmptyImageList, 0, 0 };

            // UseCompatibleStateImageBehavior false, CheckBoxes true
            yield return new object[] { false, true, true, false, View.Details, null, 1, 1 };
            yield return new object[] { false, true, true, false, View.Details, new ImageList(), 1, 1 };
            yield return new object[] { false, true, true, false, View.Details, nonEmptyImageList, 1, 1 };
            yield return new object[] { false, true, true, false, View.LargeIcon, null, 3, 1 };
            yield return new object[] { false, true, true, false, View.LargeIcon, new ImageList(), 3, 1 };
            yield return new object[] { false, true, true, false, View.LargeIcon, nonEmptyImageList, 3, 1 };
            yield return new object[] { false, true, true, false, View.List, null, 1, 1 };
            yield return new object[] { false, true, true, false, View.List, new ImageList(), 1, 1 };
            yield return new object[] { false, true, true, false, View.List, nonEmptyImageList, 1, 1 };
            yield return new object[] { false, true, true, false, View.SmallIcon, null, 3, 1 };
            yield return new object[] { false, true, true, false, View.SmallIcon, new ImageList(), 3, 1 };
            yield return new object[] { false, true, true, false, View.SmallIcon, nonEmptyImageList, 3, 1 };

            foreach (bool autoArrange in new bool[] { true, false })
            {
                yield return new object[] { false, true, autoArrange, true, View.Details, null, 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.Details, new ImageList(), 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.Details, nonEmptyImageList, 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.LargeIcon, null, 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.LargeIcon, new ImageList(), 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.LargeIcon, nonEmptyImageList, 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.List, null, 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.List, new ImageList(), 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.List, nonEmptyImageList, 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.SmallIcon, null, 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.SmallIcon, new ImageList(), 1, 1 };
                yield return new object[] { false, true, autoArrange, true, View.SmallIcon, nonEmptyImageList, 1, 1 };
            }
            
            yield return new object[] { false, true, false, false, View.Details, null, 1, 1 };
            yield return new object[] { false, true, false, false, View.Details, new ImageList(), 1, 1 };
            yield return new object[] { false, true, false, false, View.Details, nonEmptyImageList, 1, 1 };
            yield return new object[] { false, true, false, false, View.LargeIcon, null, 1, 1 };
            yield return new object[] { false, true, false, false, View.LargeIcon, new ImageList(), 1, 1 };
            yield return new object[] { false, true, false, false, View.LargeIcon, nonEmptyImageList, 1, 1 };
            yield return new object[] { false, true, false, false, View.List, null, 1, 1 };
            yield return new object[] { false, true, false, false, View.List, new ImageList(), 1, 1 };
            yield return new object[] { false, true, false, false, View.List, nonEmptyImageList, 1, 1 };
            yield return new object[] { false, true, false, false, View.SmallIcon, null, 1, 1 };
            yield return new object[] { false, true, false, false, View.SmallIcon, new ImageList(), 1, 1 };
            yield return new object[] { false, true, false, false, View.SmallIcon, nonEmptyImageList, 1, 1 };

            // UseCompatibleStateImageBehavior false, CheckBoxes false
            yield return new object[] { false, false, true, false, View.Details, null, 0, 0 };
            yield return new object[] { false, false, true, false, View.Details, new ImageList(), 0, 0 };
            yield return new object[] { false, false, true, false, View.Details, nonEmptyImageList, 0, 0 };
            yield return new object[] { false, false, true, false, View.LargeIcon, null, 1, 0 };
            yield return new object[] { false, false, true, false, View.LargeIcon, new ImageList(), 1, 0 };
            yield return new object[] { false, false, true, false, View.LargeIcon, nonEmptyImageList, 1, 0 };
            yield return new object[] { false, false, true, false, View.List, null, 0, 0 };
            yield return new object[] { false, false, true, false, View.List, new ImageList(), 0, 0 };
            yield return new object[] { false, false, true, false, View.List, nonEmptyImageList, 0, 0 };
            yield return new object[] { false, false, true, false, View.SmallIcon, null, 1, 0 };
            yield return new object[] { false, false, true, false, View.SmallIcon, new ImageList(), 1, 0 };
            yield return new object[] { false, false, true, false, View.SmallIcon, nonEmptyImageList, 1, 0 };
            yield return new object[] { false, false, true, false, View.Tile, null, 0, 0 };
            yield return new object[] { false, false, true, false, View.Tile, new ImageList(), 0, 0 };
            yield return new object[] { false, false, true, false, View.Tile, nonEmptyImageList, 0, 0 };
            
            foreach (bool autoArrange in new bool[] { true, false })
            {
                yield return new object[] { false, false, autoArrange, true, View.Details, null, 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.Details, new ImageList(), 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.Details, nonEmptyImageList, 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.LargeIcon, null, 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.LargeIcon, new ImageList(), 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.LargeIcon, nonEmptyImageList, 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.List, null, 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.List, new ImageList(), 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.List, nonEmptyImageList, 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.SmallIcon, null, 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.SmallIcon, new ImageList(), 0, 0 };
                yield return new object[] { false, false, autoArrange, true, View.SmallIcon, nonEmptyImageList, 0, 0 };
            }
            
            yield return new object[] { false, false, false, false, View.Details, null, 0, 0 };
            yield return new object[] { false, false, false, false, View.Details, new ImageList(), 0, 0 };
            yield return new object[] { false, false, false, false, View.Details, nonEmptyImageList, 0, 0 };
            yield return new object[] { false, false, false, false, View.LargeIcon, null, 0, 0 };
            yield return new object[] { false, false, false, false, View.LargeIcon, new ImageList(), 0, 0 };
            yield return new object[] { false, false, false, false, View.LargeIcon, nonEmptyImageList, 0, 0 };
            yield return new object[] { false, false, false, false, View.List, null, 0, 0 };
            yield return new object[] { false, false, false, false, View.List, new ImageList(), 0, 0 };
            yield return new object[] { false, false, false, false, View.List, nonEmptyImageList, 0, 0 };
            yield return new object[] { false, false, false, false, View.SmallIcon, null, 0, 0 };
            yield return new object[] { false, false, false, false, View.SmallIcon, new ImageList(), 0, 0 };
            yield return new object[] { false, false, false, false, View.SmallIcon, nonEmptyImageList, 0, 0 };
            yield return new object[] { false, false, false, false, View.Tile, null, 0, 0 };
            yield return new object[] { false, false, false, false, View.Tile, new ImageList(), 0, 0 };
            yield return new object[] { false, false, false, false, View.Tile, nonEmptyImageList, 0, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(StateImageList_SetWithHandleWithNonNullOldValue_GetReturnsExpected))]
        public void ListView_StateImageList_SetWithHandleWithNonNullOldValue_GetReturnsExpected(bool useCompatibleStateImageBehavior, bool checkBoxes, bool autoArrange, bool virtualMode, View view, ImageList value, int expectedInvalidatedCallCount, int expectedCreatedCallCount)
        {
            using var listView = new ListView
            {
                UseCompatibleStateImageBehavior = useCompatibleStateImageBehavior,
                CheckBoxes = checkBoxes,
                AutoArrange = autoArrange,
                VirtualMode = virtualMode,
                View = view,
                StateImageList = new ImageList()
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

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("bla")]
        public void ListView_FindItemWithText_1_should_return_null_if_no_items(string text)
        {
            var listView = new ListView();

            Assert.Null(listView.FindItemWithText(text));
        }

        [WinFormsFact]
        public void ListView_FindItemWithText_should_throw_ArgumentOutOfRangeException_if_startIndex_invalid()
        {
            var listView = new ListView();

            // no items
            Assert.Throws<ArgumentOutOfRangeException>(() => listView.FindItemWithText("", /* immaterial */false, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => listView.FindItemWithText("", /* immaterial */false, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => listView.FindItemWithText("", /* immaterial */false, 1));

            // add 2 items
            listView.Items.Add("item1");
            listView.Items.Add("item2");
            Assert.Throws<ArgumentOutOfRangeException>(() => listView.FindItemWithText("", /* immaterial */false, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => listView.FindItemWithText("", /* immaterial */false, listView.Items.Count));
        }

        [WinFormsFact]
        public void ListView_FindItemWithText_FindItem_non_VirtualMode_should_find_existing_item()
        {
            var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);

            for (int i = 0; i < 100; i++)
            {
                listView.Items.Add($"item{i:00}");
            }

            for (int i = 0; i < 100; i++)
            {
                // search for an item
                var text = $"item{i:00}";
                ListViewItem item = listView.FindItemWithText(text);
                Assert.NotNull(item);
                Assert.Equal(text, item.Text);
            }
        }

        [WinFormsFact]
        public void ListView_GetDisplayIndex_non_VirtualMode_should_find_existing_item_index()
        {
            var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);

            var items = new List<ListViewItem>(100);
            for (int i = 0; i < 100; i++)
            {
                items.Add(listView.Items.Add($"item{i:00}"));
            }

            for (int i = 0; i < items.Count; i++)
            {
                var itemIndex = listView.GetDisplayIndex(items[i], 0);
                Assert.Same(items[i], listView.Items[itemIndex]);

                itemIndex = listView.GetDisplayIndex(items[i], -1);
                Assert.Same(items[i], listView.Items[itemIndex]);
            }
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

            public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);
        }
    }
}
