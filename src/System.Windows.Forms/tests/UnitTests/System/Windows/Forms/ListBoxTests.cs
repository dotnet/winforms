// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using Moq;
using WinForms.Common.Tests;
using Xunit;
using static Interop;
using static Interop.User32;

namespace System.Windows.Forms.Tests
{
    using Point = System.Drawing.Point;
    using Size = System.Drawing.Size;

    public class ListBoxTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ListBox_Ctor_Default()
        {
            using var control = new SubListBox();
            Assert.Null(control.AccessibleDefaultActionDescription);
            Assert.Null(control.AccessibleDescription);
            Assert.Null(control.AccessibleName);
            Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
            Assert.False(control.AllowDrop);
            Assert.True(control.AllowSelection);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.False(control.AutoSize);
            Assert.Equal(SystemColors.Window, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.Null(control.BindingContext);
            Assert.Equal(BorderStyle.Fixed3D, control.BorderStyle);
            Assert.Equal(96, control.Bottom);
            Assert.Equal(new Rectangle(0, 0, 120, 96), control.Bounds);
            Assert.True(control.CanEnableIme);
            Assert.False(control.CanFocus);
            Assert.True(control.CanRaiseEvents);
            Assert.True(control.CanSelect);
            Assert.False(control.Capture);
            Assert.True(control.CausesValidation);
            Assert.Equal(new Size(116, 92), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 116, 92), control.ClientRectangle);
            Assert.Equal(0, control.ColumnWidth);
            Assert.Null(control.Container);
            Assert.False(control.ContainsFocus);
            Assert.Null(control.ContextMenuStrip);
            Assert.Empty(control.Controls);
            Assert.Same(control.Controls, control.Controls);
            Assert.False(control.Created);
            Assert.Same(Cursors.Default, control.Cursor);
            Assert.Empty(control.CustomTabOffsets);
            Assert.Same(control.CustomTabOffsets, control.CustomTabOffsets);
            Assert.Null(control.DataManager);
            Assert.Null(control.DataSource);
            Assert.Same(Cursors.Default, control.DefaultCursor);
            Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
            Assert.Equal(new Padding(3), control.DefaultMargin);
            Assert.Equal(Size.Empty, control.DefaultMaximumSize);
            Assert.Equal(Size.Empty, control.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, control.DefaultPadding);
            Assert.Equal(new Size(120, 96), control.DefaultSize);
            Assert.False(control.DesignMode);
            Assert.Empty(control.DisplayMember);
            Assert.Equal(new Rectangle(0, 0, 116, 92), control.DisplayRectangle);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.False(control.DoubleBuffered);
            Assert.Equal(DrawMode.Normal, control.DrawMode);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.False(control.Focused);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(SystemColors.WindowText, control.ForeColor);
            Assert.Null(control.FormatInfo);
            Assert.Empty(control.FormatString);
            Assert.False(control.FormattingEnabled);
            Assert.False(control.HasChildren);
            Assert.Equal(96, control.Height);
            Assert.Equal(0, control.HorizontalExtent);
            Assert.False(control.HorizontalScrollbar);
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.True(control.IntegralHeight);
            Assert.False(control.IsAccessible);
            Assert.False(control.IsMirrored);
            Assert.Equal(13, control.ItemHeight);
            Assert.Empty(control.Items);
            Assert.Same(control.Items, control.Items);
            Assert.NotNull(control.LayoutEngine);
            Assert.Same(control.LayoutEngine, control.LayoutEngine);
            Assert.Equal(0, control.Left);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Equal(new Padding(3), control.Margin);
            Assert.Equal(Size.Empty, control.MaximumSize);
            Assert.Equal(Size.Empty, control.MinimumSize);
            Assert.False(control.MultiColumn);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Null(control.Parent);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.Equal(13 + SystemInformation.BorderSize.Height * 4 + 3, control.PreferredHeight);
            Assert.Equal(new Size(120, 96), control.PreferredSize);
            Assert.False(control.RecreatingHandle);
            Assert.Null(control.Region);
            Assert.False(control.ResizeRedraw);
            Assert.Equal(120, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.False(control.ScrollAlwaysVisible);
            Assert.Null(control.SelectedValue);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.SelectedIndices);
            Assert.Same(control.SelectedIndices, control.SelectedIndices);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.SelectedItems);
            Assert.Same(control.SelectedItems, control.SelectedItems);
            Assert.Equal(SelectionMode.One, control.SelectionMode);
            Assert.True(control.ShowFocusCues);
            Assert.True(control.ShowKeyboardCues);
            Assert.Null(control.Site);
            Assert.Equal(new Size(120, 96), control.Size);
            Assert.False(control.Sorted);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(0, control.Top);
            Assert.Equal(0, control.TopIndex);
            Assert.Null(control.TopLevelControl);
            Assert.False(control.UseCustomTabOffsets);
            Assert.True(control.UseTabStops);
            Assert.False(control.UseWaitCursor);
            Assert.Empty(control.ValueMember);
            Assert.True(control.Visible);
            Assert.Equal(120, control.Width);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBox_CreateParams_GetDefault_ReturnsExpected()
        {
            using var control = new SubListBox();
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("ListBox", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x200, createParams.ExStyle);
            Assert.Equal(96, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x562100C1, createParams.Style);
            Assert.Equal(120, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 0x562110C1)]
        [InlineData(false, 0x562100C1)]
        public void ListBox_CreateParams_GetScrolAlwaysVisible_ReturnsExpected(bool scrollAlwaysVisible, int expectedStyle)
        {
            using var control = new SubListBox
            {
                ScrollAlwaysVisible = scrollAlwaysVisible
            };

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("ListBox", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x200, createParams.ExStyle);
            Assert.Equal(96, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(120, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 0x562100C1)]
        [InlineData(false, 0x562101C1)]
        public void ListBox_CreateParams_GetIntegralHeight_ReturnsExpected(bool integralHeight, int expectedStyle)
        {
            using var control = new SubListBox
            {
                IntegralHeight = integralHeight
            };

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("ListBox", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x200, createParams.ExStyle);
            Assert.Equal(96, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(120, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 0x562100C1)]
        [InlineData(false, 0x56210041)]
        public void ListBox_CreateParams_GetUseTabStops_ReturnsExpected(bool useTabStops, int expectedStyle)
        {
            using var control = new SubListBox
            {
                UseTabStops = useTabStops
            };

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("ListBox", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x200, createParams.ExStyle);
            Assert.Equal(96, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(120, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(BorderStyle.None, 0x562100C1, 0)]
        [InlineData(BorderStyle.Fixed3D, 0x562100C1, 0x200)]
        [InlineData(BorderStyle.FixedSingle, 0x56A100C1, 0)]
        public void ListBox_CreateParams_GetBorderStyle_ReturnsExpected(BorderStyle borderStyle, int expectedStyle, int expectedExStyle)
        {
            using var control = new SubListBox
            {
                BorderStyle = borderStyle
            };

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("ListBox", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(expectedExStyle, createParams.ExStyle);
            Assert.Equal(96, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(120, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true, 0x563102C1)]
        [InlineData(true, false, 0x563102C1)]
        [InlineData(false, true, 0x563100C1)]
        [InlineData(false, false, 0x562100C1)]
        public void ListBox_CreateParams_GetMultiColumn_ReturnsExpected(bool multiColumn, bool horizontalScrollBar, int expectedStyle)
        {
            using var control = new SubListBox
            {
                MultiColumn = multiColumn,
                HorizontalScrollbar = horizontalScrollBar
            };

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("ListBox", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x200, createParams.ExStyle);
            Assert.Equal(96, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(120, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(SelectionMode.MultiExtended, 0x562108C1)]
        [InlineData(SelectionMode.MultiSimple, 0x562100C9)]
        [InlineData(SelectionMode.None, 0x562140C1)]
        [InlineData(SelectionMode.One, 0x562100C1)]
        public void ListBox_CreateParams_GetSelectionMode_ReturnsExpected(SelectionMode selectionMode, int expectedStyle)
        {
            using var control = new SubListBox
            {
                SelectionMode = selectionMode
            };

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("ListBox", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x200, createParams.ExStyle);
            Assert.Equal(96, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(120, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(DrawMode.Normal, 0x562100C1)]
        [InlineData(DrawMode.OwnerDrawFixed, 0x562100D1)]
        [InlineData(DrawMode.OwnerDrawVariable, 0x562100E1)]
        public void ListBox_CreateParams_GetDrawMode_ReturnsExpected(DrawMode drawMode, int expectedStyle)
        {
            using var control = new SubListBox
            {
                DrawMode = drawMode
            };

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("ListBox", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x200, createParams.ExStyle);
            Assert.Equal(96, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(120, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> BackColor_Set_TestData()
        {
            yield return new object[] { Color.Empty, SystemColors.Window };
            yield return new object[] { Color.Red, Color.Red };
        }

        [WinFormsTheory]
        [MemberData(nameof(BackColor_Set_TestData))]
        public void ListBox_BackColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new ListBox
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
        public void ListBox_BackColor_SetWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
        {
            using var control = new ListBox();
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
        public void ListBox_BackColor_SetWithHandler_CallsBackColorChanged()
        {
            using var control = new ListBox();
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

        [WinFormsFact]
        public void ListBox_BackColor_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ListBox))[nameof(ListBox.BackColor)];
            using var control = new ListBox();
            Assert.False(property.CanResetValue(control));

            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.Window, control.BackColor);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void ListBox_BackColor_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ListBox))[nameof(ListBox.BackColor)];
            using var control = new ListBox();
            Assert.False(property.ShouldSerializeValue(control));

            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.Window, control.BackColor);
            Assert.False(property.ShouldSerializeValue(control));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetImageTheoryData))]
        public void ListBox_BackgroundImage_Set_GetReturnsExpected(Image value)
        {
            using var control = new ListBox
            {
                BackgroundImage = value
            };
            Assert.Same(value, control.BackgroundImage);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.BackgroundImage = value;
            Assert.Same(value, control.BackgroundImage);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBox_BackgroundImage_SetWithHandler_CallsBackgroundImageChanged()
        {
            using var control = new ListBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.BackgroundImageChanged += handler;

            // Set different.
            using var image1 = new Bitmap(10, 10);
            control.BackgroundImage = image1;
            Assert.Same(image1, control.BackgroundImage);
            Assert.Equal(1, callCount);

            // Set same.
            control.BackgroundImage = image1;
            Assert.Same(image1, control.BackgroundImage);
            Assert.Equal(1, callCount);

            // Set different.
            using var image2 = new Bitmap(10, 10);
            control.BackgroundImage = image2;
            Assert.Same(image2, control.BackgroundImage);
            Assert.Equal(2, callCount);

            // Set null.
            control.BackgroundImage = null;
            Assert.Null(control.BackgroundImage);
            Assert.Equal(3, callCount);

            // Remove handler.
            control.BackgroundImageChanged -= handler;
            control.BackgroundImage = image1;
            Assert.Same(image1, control.BackgroundImage);
            Assert.Equal(3, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ImageLayout))]
        public void ListBox_BackgroundImageLayout_Set_GetReturnsExpected(ImageLayout value)
        {
            using var control = new ListBox
            {
                BackgroundImageLayout = value
            };
            Assert.Equal(value, control.BackgroundImageLayout);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.BackgroundImageLayout = value;
            Assert.Equal(value, control.BackgroundImageLayout);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBox_BackgroundImageLayout_SetWithHandler_CallsBackgroundImageLayoutChanged()
        {
            using var control = new ListBox();
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
        public void ListBox_BackgroundImageLayout_SetInvalid_ThrowsInvalidEnumArgumentException(ImageLayout value)
        {
            using var control = new ListBox();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.BackgroundImageLayout = value);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(BorderStyle))]
        public void ListBox_BorderStyle_Set_GetReturnsExpected(BorderStyle value)
        {
            using var control = new ListBox()
            {
                BorderStyle = value
            };
            Assert.Equal(value, control.BorderStyle);
            Assert.Equal(96, control.Height);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.BorderStyle = value;
            Assert.Equal(value, control.BorderStyle);
            Assert.Equal(96, control.Height);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(BorderStyle.Fixed3D, 0)]
        [InlineData(BorderStyle.FixedSingle, 1)]
        [InlineData(BorderStyle.None, 1)]
        public void ListBox_BorderStyle_SetWithHandle_GetReturnsExpected(BorderStyle value, int expectedCreatedCallCount)
        {
            using var control = new ListBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.BorderStyle = value;
            Assert.Equal(value, control.BorderStyle);
            Assert.True(control.Height > 0);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set same.
            control.BorderStyle = value;
            Assert.Equal(value, control.BorderStyle);
            Assert.True(control.Height > 0);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(BorderStyle))]
        public void ListBox_BorderStyle_SetInvalid_ThrowsInvalidEnumArgumentException(BorderStyle value)
        {
            using var control = new ListBox();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.BorderStyle = value);
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(60)]
        [InlineData(int.MaxValue)]
        public void ListBox_ColumnWidth_Set_GetReturnsExpected(int value)
        {
            using var control = new ListBox
            {
                ColumnWidth = value
            };
            Assert.Equal(value, control.ColumnWidth);
            Assert.False(control.IsHandleCreated);

            control.ColumnWidth = value;
            Assert.Equal(value, control.ColumnWidth);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(60)]
        [InlineData(int.MaxValue)]
        public void ListBox_ColumnWidth_SetWithCustomOldValue_GetReturnsExpected(int value)
        {
            using var control = new ListBox
            {
                ColumnWidth = 10
            };

            control.ColumnWidth = value;
            Assert.Equal(value, control.ColumnWidth);
            Assert.False(control.IsHandleCreated);

            control.ColumnWidth = value;
            Assert.Equal(value, control.ColumnWidth);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(60)]
        [InlineData(int.MaxValue)]
        public void ListBox_ColumnWidth_SetWithHandle_GetReturnsExpected(int value)
        {
            using var control = new ListBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.ColumnWidth = value;
            Assert.Equal(value, control.ColumnWidth);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            control.ColumnWidth = value;
            Assert.Equal(value, control.ColumnWidth);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        [InlineData(60, 0)]
        [InlineData(int.MaxValue, 0)]
        public void ListBox_ColumnWidth_SetWithCustomOldValueWithHandle_GetReturnsExpected(int value, int expectedCreatedCallCount)
        {
            using var control = new ListBox
            {
                ColumnWidth = 10
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.ColumnWidth = value;
            Assert.Equal(value, control.ColumnWidth);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            control.ColumnWidth = value;
            Assert.Equal(value, control.ColumnWidth);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);
        }

        [WinFormsFact]
        public void ListBox_ColumnWidth_GetItemRect_ReturnsExpected()
        {
            using var control = new ListBox
            {
                MultiColumn = true
            };
            control.Items.Add("Value");

            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.ColumnWidth = 123;

            RECT rc = default;
            Assert.Equal((IntPtr)1, SendMessageW(control.Handle, (WM)LB.GETITEMRECT, (IntPtr)0, ref rc));
            Assert.Equal(123, ((Rectangle)rc).Width);
        }

        [WinFormsFact]
        public void ListBox_ColumnWidth_SetNegative_ThrowsArgumentOutOfRangeException()
        {
            using var control = new ListBox();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => control.ColumnWidth = -1);
        }

        public static IEnumerable<object[]> DataSource_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new List<int>() };
            yield return new object[] { Array.Empty<int>() };

            var mockSource = new Mock<IListSource>(MockBehavior.Strict);
            mockSource
                .Setup(s => s.GetList())
                .Returns(new int[] { 1 });
            yield return new object[] { mockSource.Object };
        }

        [WinFormsTheory]
        [MemberData(nameof(DataSource_Set_TestData))]
        public void ListBox_DataSource_Set_GetReturnsExpected(object value)
        {
            using var control = new SubListBox
            {
                DataSource = value
            };
            Assert.Same(value, control.DataSource);
            Assert.Empty(control.DisplayMember);
            Assert.Null(control.DataManager);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.DataSource = value;
            Assert.Same(value, control.DataSource);
            Assert.Empty(control.DisplayMember);
            Assert.Null(control.DataManager);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBox_DataSource_SetWithHandler_CallsDataSourceChanged()
        {
            using var control = new ListBox();
            int dataSourceCallCount = 0;
            int displayMemberCallCount = 0;
            EventHandler dataSourceHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                dataSourceCallCount++;
            };
            EventHandler displayMemberHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                displayMemberCallCount++;
            };
            control.DataSourceChanged += dataSourceHandler;
            control.DisplayMemberChanged += displayMemberHandler;

            // Set different.
            var dataSource1 = new List<int>();
            control.DataSource = dataSource1;
            Assert.Same(dataSource1, control.DataSource);
            Assert.Equal(1, dataSourceCallCount);
            Assert.Equal(0, displayMemberCallCount);

            // Set same.
            control.DataSource = dataSource1;
            Assert.Same(dataSource1, control.DataSource);
            Assert.Equal(1, dataSourceCallCount);
            Assert.Equal(0, displayMemberCallCount);

            // Set different.
            var dataSource2 = new List<int>();
            control.DataSource = dataSource2;
            Assert.Same(dataSource2, control.DataSource);
            Assert.Equal(2, dataSourceCallCount);
            Assert.Equal(0, displayMemberCallCount);

            // Set null.
            control.DataSource = null;
            Assert.Null(control.DataSource);
            Assert.Equal(3, dataSourceCallCount);
            Assert.Equal(0, displayMemberCallCount);

            // Remove handler.
            control.DataSourceChanged -= dataSourceHandler;
            control.DisplayMemberChanged -= displayMemberHandler;
            control.DataSource = dataSource1;
            Assert.Same(dataSource1, control.DataSource);
            Assert.Equal(3, dataSourceCallCount);
            Assert.Equal(0, displayMemberCallCount);
        }

        public static IEnumerable<object[]> DrawMode_Set_TestData()
        {
            foreach (bool autoSize in new bool[] { true, false })
            {
                yield return new object[] { autoSize, true, DrawMode.Normal };
                yield return new object[] { autoSize, false, DrawMode.Normal };

                yield return new object[] { autoSize, true, DrawMode.OwnerDrawFixed };
                yield return new object[] { autoSize, false, DrawMode.OwnerDrawFixed };

                yield return new object[] { autoSize, false, DrawMode.OwnerDrawVariable };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(DrawMode_Set_TestData))]
        public void ListBox_DrawMode_Set_GetReturnsExpected(bool autoSize, bool multiColumn, DrawMode value)
        {
            using var control = new ListBox
            {
                AutoSize = autoSize,
                MultiColumn = multiColumn
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;

            control.DrawMode = value;
            Assert.Equal(value, control.DrawMode);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.DrawMode = value;
            Assert.Equal(value, control.DrawMode);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> DrawMode_SetWithParent_TestData()
        {
            yield return new object[] { true, true, DrawMode.Normal, 0 };
            yield return new object[] { true, false, DrawMode.Normal, 0 };
            yield return new object[] { false, true, DrawMode.Normal, 0 };
            yield return new object[] { false, false, DrawMode.Normal, 0 };

            yield return new object[] { true, true, DrawMode.OwnerDrawFixed, 0 };
            yield return new object[] { true, false, DrawMode.OwnerDrawFixed, 0 };
            yield return new object[] { false, true, DrawMode.OwnerDrawFixed, 0 };
            yield return new object[] { false, false, DrawMode.OwnerDrawFixed, 0 };

            yield return new object[] { true, false, DrawMode.OwnerDrawVariable, 1 };
            yield return new object[] { false, false, DrawMode.OwnerDrawVariable, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(DrawMode_SetWithParent_TestData))]
        public void ListBox_DrawMode_SetWithParent_GetReturnsExpected(bool autoSize, bool multiColumn, DrawMode value, int expectedParentLayoutCallCount)
        {
            using var parent = new Control();
            using var control = new ListBox
            {
                AutoSize = autoSize,
                MultiColumn = multiColumn,
                Parent = parent
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("DrawMode", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;

            try
            {
                control.DrawMode = value;
                Assert.Equal(value, control.DrawMode);
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);
                Assert.False(parent.IsHandleCreated);

                // Set same.
                control.DrawMode = value;
                Assert.Equal(value, control.DrawMode);
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);
                Assert.False(parent.IsHandleCreated);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        public static IEnumerable<object[]> DrawMode_SetWithHandle_TestData()
        {
            foreach (bool autoSize in new bool[] { true, false })
            {
                yield return new object[] { autoSize, true, DrawMode.Normal, 0 };
                yield return new object[] { autoSize, false, DrawMode.Normal, 0 };

                yield return new object[] { autoSize, true, DrawMode.OwnerDrawFixed, 1 };
                yield return new object[] { autoSize, false, DrawMode.OwnerDrawFixed, 1 };

                yield return new object[] { autoSize, false, DrawMode.OwnerDrawVariable, 1 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(DrawMode_SetWithHandle_TestData))]
        public void ListBox_DrawMode_SetWithHandle_GetReturnsExpected(bool autoSize, bool multiColumn, DrawMode value, int expectedCreatedCallCount)
        {
            using var control = new ListBox
            {
                AutoSize = autoSize,
                MultiColumn = multiColumn
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

            control.DrawMode = value;
            Assert.Equal(value, control.DrawMode);
            Assert.Equal(expectedCreatedCallCount * 2, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set same.
            control.DrawMode = value;
            Assert.Equal(value, control.DrawMode);
            Assert.Equal(expectedCreatedCallCount * 2, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(DrawMode))]
        public void ListBox_DrawMode_SetInvalidValue_ThrowsInvalidEnumArgumentException(DrawMode value)
        {
            using var control = new ListBox();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.DrawMode = value);
        }

        [WinFormsFact]
        public void ListBox_DrawMode_SetMultiColumnOwnerDrawVariable_ThrowsArgumentException()
        {
            using var control = new ListBox
            {
                MultiColumn = true
            };
            Assert.Throws<ArgumentException>("value", () => control.DrawMode = DrawMode.OwnerDrawVariable);
        }

        public static IEnumerable<object[]> Font_Set_TestData()
        {
            foreach (bool integralHeight in new bool[] { true, false })
            {
                yield return new object[] { integralHeight, null };
                yield return new object[] { integralHeight, new Font("Arial", 8.25f) };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Font_Set_TestData))]
        public void ListBox_Font_Set_GetReturnsExpected(bool integralHeight, Font value)
        {
            using var control = new SubListBox
            {
                IntegralHeight = integralHeight,
                Font = value
            };
            Assert.Equal(value ?? Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(96, control.Height);
            Assert.Empty(control.Items);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Font = value;
            Assert.Equal(value ?? Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(96, control.Height);
            Assert.Empty(control.Items);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Font_Set_TestData))]
        public void ListBox_Font_SetWithItems_GetReturnsExpected(bool integralHeight, Font value)
        {
            using var control = new SubListBox
            {
                IntegralHeight = integralHeight
            };
            control.Items.Add("item1");
            control.Items.Add("item2");
            control.Items.Add("item1");

            control.Font = value;
            Assert.Equal(value ?? Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(96, control.Height);
            Assert.Equal(new string[] { "item1", "item2", "item1" }, control.Items.Cast<object>());
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Font = value;
            Assert.Equal(value ?? Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(96, control.Height);
            Assert.Equal(new string[] { "item1", "item2", "item1" }, control.Items.Cast<object>());
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Font_SetWithHandle_TestData()
        {
            yield return new object[] { true, null, 0, 0 };
            yield return new object[] { false, null, 0, 1 };
            yield return new object[] { true, new Font("Arial", 8.25f), 1, 1 };
            yield return new object[] { false, new Font("Arial", 8.25f), 1, 2 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Font_SetWithHandle_TestData))]
        public void ListBox_Font_SetWithHandle_GetReturnsExpected(bool integralHeight, Font value, int expectedInvalidatedCallCount1, int expectedInvalidatedCallCount2)
        {
            using var control = new SubListBox
            {
                IntegralHeight = integralHeight
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Font = value;
            Assert.Equal(value ?? Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.True(control.Height > 0);
            Assert.Empty(control.Items);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Font = value;
            Assert.Equal(value ?? Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.True(control.Height > 0);
            Assert.Empty(control.Items);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> Font_SetHandleWithItems_TestData()
        {
            yield return new object[] { true, null, 0, 0 };
            yield return new object[] { false, null, 1, 2 };
            yield return new object[] { true, new Font("Arial", 8.25f), 1, 1 };
            yield return new object[] { false, new Font("Arial", 8.25f), 2, 3 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Font_SetHandleWithItems_TestData))]
        public void ListBox_Font_SetWithItemsWithHandle_GetReturnsExpected(bool integralHeight, Font value, int expectedInvalidatedCallCount1, int expectedInvalidatedCallCount2)
        {
            using var control = new SubListBox
            {
                IntegralHeight = integralHeight
            };
            control.Items.Add("item1");
            control.Items.Add("item2");
            control.Items.Add("item1");
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Font = value;
            Assert.Equal(value ?? Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.True(control.Height > 0);
            Assert.Equal(new string[] { "item1", "item2", "item1" }, control.Items.Cast<object>());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Font = value;
            Assert.Equal(value ?? Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.True(control.Height > 0);
            Assert.Equal(new string[] { "item1", "item2", "item1" }, control.Items.Cast<object>());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ListBox_Font_SetWithHandler_CallsFontChanged()
        {
            using var control = new ListBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.FontChanged += handler;

            // Set different.
            using var font1 = new Font("Arial", 8.25f);
            control.Font = font1;
            Assert.Same(font1, control.Font);
            Assert.Equal(1, callCount);

            // Set same.
            control.Font = font1;
            Assert.Same(font1, control.Font);
            Assert.Equal(1, callCount);

            // Set different.
            using var font2 = SystemFonts.DialogFont;
            control.Font = font2;
            Assert.Same(font2, control.Font);
            Assert.Equal(2, callCount);

            // Set null.
            control.Font = null;
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(3, callCount);

            // Remove handler.
            control.FontChanged -= handler;
            control.Font = font1;
            Assert.Same(font1, control.Font);
            Assert.Equal(3, callCount);
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
        public void ListBox_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new ListBox
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
        public void ListBox_ForeColor_SetWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
        {
            using var control = new ListBox();
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
        public void ListBox_ForeColor_SetWithHandler_CallsForeColorChanged()
        {
            using var control = new ListBox();
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

        [WinFormsFact]
        public void ListBox_ForeColor_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ListBox))[nameof(ListBox.ForeColor)];
            using var control = new ListBox();
            Assert.False(property.CanResetValue(control));

            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.WindowText, control.ForeColor);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void ListBox_ForeColor_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ListBox))[nameof(ListBox.ForeColor)];
            using var control = new ListBox();
            Assert.False(property.ShouldSerializeValue(control));

            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.WindowText, control.ForeColor);
            Assert.False(property.ShouldSerializeValue(control));
        }

        public static IEnumerable<object[]> HorizontalExtent_Set_TestData()
        {
            foreach (bool multiColumn in new bool[] { true, false })
            {
                foreach (bool horizontalScrollbar in new bool[] { true, false })
                {
                    yield return new object[] { multiColumn, horizontalScrollbar, -1 };
                    yield return new object[] { multiColumn, horizontalScrollbar, 0 };
                    yield return new object[] { multiColumn, horizontalScrollbar, 120 };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(HorizontalExtent_Set_TestData))]
        public void ListBox_HorizontalExtent_Set_GetReturnsExpected(bool multiColumn, bool horizontalScrollBar, int value)
        {
            using var control = new ListBox
            {
                MultiColumn = multiColumn,
                HorizontalScrollbar = horizontalScrollBar,
                HorizontalExtent = value
            };
            Assert.Equal(value, control.HorizontalExtent);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.HorizontalExtent = value;
            Assert.Equal(value, control.HorizontalExtent);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> HorizontalExtent_SetWithHandle_TestData()
        {
            foreach (bool multiColumn in new bool[] { true, false })
            {
                foreach (bool horizontalScrollbar in new bool[] { true, false })
                {
                    yield return new object[] { multiColumn, horizontalScrollbar, -1, 0 };
                    yield return new object[] { multiColumn, horizontalScrollbar, 0, 0 };
                    yield return new object[] { multiColumn, horizontalScrollbar, 120, !multiColumn && horizontalScrollbar ? 1 : 0 };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(HorizontalExtent_SetWithHandle_TestData))]
        public void ListBox_HorizontalExtent_SetWithHandle_GetReturnsExpected(bool multiColumn, bool horizontalScrollBar, int value, int expectedInvalidatedCallCount)
        {
            using var control = new ListBox
            {
                MultiColumn = multiColumn,
                HorizontalScrollbar = horizontalScrollBar
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.HorizontalExtent = value;
            Assert.Equal(value, control.HorizontalExtent);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.HorizontalExtent = value;
            Assert.Equal(value, control.HorizontalExtent);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(true, true, 0)]
        [InlineData(true, false, 0)]
        [InlineData(false, true, 10)]
        [InlineData(false, false, 0)]
        public void ListBox_HorizontalExtent_GetHorizontalExtent_Success(bool multiColumn, bool horizontalScrollBar, int expected)
        {
            using var control = new ListBox
            {
                MultiColumn = multiColumn,
                HorizontalScrollbar = horizontalScrollBar
            };

            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Equal(0, control.HorizontalExtent);
            Assert.Equal((IntPtr)0, SendMessageW(control.Handle, (WM)LB.GETHORIZONTALEXTENT));

            control.HorizontalExtent = 10;
            Assert.Equal((IntPtr)expected, SendMessageW(control.Handle, (WM)LB.GETHORIZONTALEXTENT));
        }

        public static IEnumerable<object[]> HorizontalScrollbar_Set_TestData()
        {
            foreach (bool multiColumn in new bool[] { true, false })
            {
                yield return new object[] { multiColumn, true };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(HorizontalScrollbar_Set_TestData))]
        public void ListBox_HorizontalScrollbar_Set_GetReturnsExpected(bool multiColumn, bool value)
        {
            using var control = new ListBox
            {
                MultiColumn = multiColumn,
                HorizontalScrollbar = value
            };
            Assert.Equal(value, control.HorizontalScrollbar);
            Assert.Empty(control.Items);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.HorizontalScrollbar = value;
            Assert.Equal(value, control.HorizontalScrollbar);
            Assert.Empty(control.Items);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.HorizontalScrollbar = !value;
            Assert.Equal(!value, control.HorizontalScrollbar);
            Assert.Empty(control.Items);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(HorizontalScrollbar_Set_TestData))]
        public void ListBox_HorizontalScrollbar_SetWithItems_GetReturnsExpected(bool multiColumn, bool value)
        {
            using var control = new ListBox
            {
                MultiColumn = multiColumn
            };
            control.Items.Add("item1");
            control.Items.Add("item2");
            control.Items.Add("item1");

            control.HorizontalScrollbar = value;
            Assert.Equal(value, control.HorizontalScrollbar);
            Assert.Equal(new string[] { "item1", "item2", "item1" }, control.Items.Cast<string>());
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.HorizontalScrollbar = value;
            Assert.Equal(value, control.HorizontalScrollbar);
            Assert.Equal(new string[] { "item1", "item2", "item1" }, control.Items.Cast<string>());
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.HorizontalScrollbar = !value;
            Assert.Equal(!value, control.HorizontalScrollbar);
            Assert.Equal(new string[] { "item1", "item2", "item1" }, control.Items.Cast<string>());
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> HorizontalScrollbar_SetWithHandle_TestData()
        {
            yield return new object[] { true, true, 0, 0, 1, 0 };
            yield return new object[] { true, false, 0, 0, 1, 0 };
            yield return new object[] { false, true, 2, 1, 3, 2 };
            yield return new object[] { false, false, 0, 0, 3, 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(HorizontalScrollbar_SetWithHandle_TestData))]
        public void ListBox_HorizontalScrollbar_SetWithHandle_GetReturnsExpected(bool multiColumn, bool value, int expectedInvalidatedCallCount1, int expectedCreatedCallCount1, int expectedInvalidatedCallCount2, int expectedCreatedCallCount2)
        {
            using var control = new ListBox
            {
                MultiColumn = multiColumn
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.HorizontalScrollbar = value;
            Assert.Equal(value, control.HorizontalScrollbar);
            Assert.Empty(control.Items);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount1, createdCallCount);

            // Set same.
            control.HorizontalScrollbar = value;
            Assert.Equal(value, control.HorizontalScrollbar);
            Assert.Empty(control.Items);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount1, createdCallCount);

            // Set different.
            control.HorizontalScrollbar = !value;
            Assert.Equal(!value, control.HorizontalScrollbar);
            Assert.Empty(control.Items);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount2, createdCallCount);
        }

        public static IEnumerable<object[]> HorizontalScrollbar_SetWithItemsWithHandle_TestData()
        {
            yield return new object[] { true, true, 1, 0, 2, 0 };
            yield return new object[] { true, false, 0, 0, 1, 0 };
            yield return new object[] { false, true, 3, 1, 4, 2 };
            yield return new object[] { false, false, 0, 0, 3, 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(HorizontalScrollbar_SetWithItemsWithHandle_TestData))]
        public void ListBox_HorizontalScrollbar_SetWithItemsWithHandle_GetReturnsExpected(bool multiColumn, bool value, int expectedInvalidatedCallCount1, int expectedCreatedCallCount1, int expectedInvalidatedCallCount2, int expectedCreatedCallCount2)
        {
            using var control = new ListBox
            {
                MultiColumn = multiColumn
            };
            control.Items.Add("item1");
            control.Items.Add("item2");
            control.Items.Add("item1");
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.HorizontalScrollbar = value;
            Assert.Equal(value, control.HorizontalScrollbar);
            Assert.Equal(new string[] { "item1", "item2", "item1" }, control.Items.Cast<string>());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount1, createdCallCount);

            // Set same.
            control.HorizontalScrollbar = value;
            Assert.Equal(value, control.HorizontalScrollbar);
            Assert.Equal(new string[] { "item1", "item2", "item1" }, control.Items.Cast<string>());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount1, createdCallCount);

            // Set different.
            control.HorizontalScrollbar = !value;
            Assert.Equal(!value, control.HorizontalScrollbar);
            Assert.Equal(new string[] { "item1", "item2", "item1" }, control.Items.Cast<string>());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount2, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListBox_IntegralHeight_Set_GetReturnsExpected(bool value)
        {
            using var control = new ListBox
            {
                IntegralHeight = value
            };
            Assert.Equal(value, control.IntegralHeight);
            Assert.Equal(96, control.Height);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.IntegralHeight = value;
            Assert.Equal(value, control.IntegralHeight);
            Assert.Equal(96, control.Height);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.IntegralHeight = !value;
            Assert.Equal(!value, control.IntegralHeight);
            Assert.Equal(96, control.Height);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 0)]
        [InlineData(false, 1)]
        public void ListBox_IntegralHeight_SetWithHandle_GetReturnsExpected(bool value, int expectedCreatedCallCount)
        {
            using var control = new ListBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.IntegralHeight = value;
            Assert.Equal(value, control.IntegralHeight);
            Assert.True(control.Height > 0);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set same.
            control.IntegralHeight = value;
            Assert.Equal(value, control.IntegralHeight);
            Assert.True(control.Height > 0);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set different.
            control.IntegralHeight = !value;
            Assert.Equal(!value, control.IntegralHeight);
            Assert.True(control.Height > 0);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount + 1, createdCallCount);
        }

        public static IEnumerable<object[]> ItemHeight_Set_TestData()
        {
            foreach (Enum drawMode in Enum.GetValues(typeof(DrawMode)))
            {
                foreach (bool integralHeight in new bool[] { true, false })
                {
                    yield return new object[] { drawMode, integralHeight, 1 };
                    yield return new object[] { drawMode, integralHeight, 13 };
                    yield return new object[] { drawMode, integralHeight, 255 };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ItemHeight_Set_TestData))]
        public void ListBox_ItemHeight_Set_GetReturnsExpected(DrawMode drawMode, bool integralHeight, int value)
        {
            using var control = new ListBox
            {
                DrawMode = drawMode,
                IntegralHeight = integralHeight,
                ItemHeight = value
            };
            Assert.Equal(value, control.ItemHeight);
            Assert.Equal(96, control.Height);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.ItemHeight = value;
            Assert.Equal(value, control.ItemHeight);
            Assert.Equal(96, control.Height);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> ItemHeight_SetWithHandle_TestData()
        {
            foreach (bool integralHeight in new bool[] { true, false })
            {
                yield return new object[] { DrawMode.Normal, integralHeight, 1, 0 };
                yield return new object[] { DrawMode.Normal, integralHeight, 13, 0 };
                yield return new object[] { DrawMode.Normal, integralHeight, 255, 0 };
                yield return new object[] { DrawMode.OwnerDrawFixed, integralHeight, 1, 1 };
                yield return new object[] { DrawMode.OwnerDrawFixed, integralHeight, 13, 0 };
                yield return new object[] { DrawMode.OwnerDrawFixed, integralHeight, 255, 1 };
                yield return new object[] { DrawMode.OwnerDrawVariable, integralHeight, 1, 0 };
                yield return new object[] { DrawMode.OwnerDrawVariable, integralHeight, 13, 0 };
                yield return new object[] { DrawMode.OwnerDrawVariable, integralHeight, 255, 0 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ItemHeight_SetWithHandle_TestData))]
        public void ListBox_ItemHeight_SetWithHandle_GetReturnsExpected(DrawMode drawMode, bool integralHeight, int value, int expectedInvalidatedCallCount)
        {
            using var control = new ListBox
            {
                DrawMode = drawMode,
                IntegralHeight = integralHeight
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.ItemHeight = value;
            Assert.True(control.ItemHeight > 0);
            Assert.True(control.Height > 0);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.ItemHeight = value;
            Assert.True(control.ItemHeight > 0);
            Assert.True(control.Height > 0);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(DrawMode.Normal, false)]
        [InlineData(DrawMode.OwnerDrawFixed, true)]
        [InlineData(DrawMode.OwnerDrawVariable, false)]
        public void ListBox_ItemHeight_Set_GetItemHeight_ReturnsExpected(DrawMode drawMode, bool expected)
        {
            using var control = new ListBox
            {
                DrawMode = drawMode,
                ItemHeight = 25
            };

            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Equal(expected, SendMessageW(control.Handle, (WM)LB.GETITEMHEIGHT) == (IntPtr)25);
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(256)]
        public void ListBox_ItemHeight_SetInvalidValue_ThrowsArgumentOutOfRangeException(int value)
        {
            using var control = new ListBox();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => control.ItemHeight = value);
        }

        [WinFormsFact]
        public void ListBox_ItemHeight_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ListBox))[nameof(ListBox.ItemHeight)];
            using var control = new ListBox();
            Assert.False(property.CanResetValue(control));

            control.ItemHeight = 15;
            Assert.Equal(15, control.ItemHeight);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(13, control.ItemHeight);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void ListBox_ItemHeight_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ListBox))[nameof(ListBox.ItemHeight)];
            using var control = new ListBox();
            Assert.False(property.ShouldSerializeValue(control));

            control.ItemHeight = 15;
            Assert.Equal(15, control.ItemHeight);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(13, control.ItemHeight);
            Assert.False(property.ShouldSerializeValue(control));
        }

        public static IEnumerable<object[]> Items_CustomCreateItemCollection_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ListBox.ObjectCollection(new ListBox()) };
        }

        [WinFormsTheory]
        [MemberData(nameof(Items_CustomCreateItemCollection_TestData))]
        public void ListBox_Items_GetCustomCreateItemCollection_ReturnsExpected(ListBox.ObjectCollection result)
        {
            using var control = new CustomCreateItemCollectionListBox
            {
                CreateListBoxResult = result
            };
            Assert.Same(result, control.Items);
            Assert.Same(control.Items, control.Items);
            Assert.False(control.IsHandleCreated);
        }

        private class CustomCreateItemCollectionListBox : ListBox
        {
            public ListBox.ObjectCollection CreateListBoxResult { get; set; }

            protected override ListBox.ObjectCollection CreateItemCollection() => CreateListBoxResult;
        }

        public static IEnumerable<object[]> MultiColumn_Set_TestData()
        {
            yield return new object[] { DrawMode.Normal, true };
            yield return new object[] { DrawMode.Normal, false };
            yield return new object[] { DrawMode.OwnerDrawFixed, true };
            yield return new object[] { DrawMode.OwnerDrawFixed, false };
        }

        [WinFormsTheory]
        [MemberData(nameof(MultiColumn_Set_TestData))]
        public void ListBox_MultiColumn_Set_GetReturnsExpected(DrawMode drawMode, bool value)
        {
            using var control = new ListBox
            {
                DrawMode = drawMode,
                MultiColumn = value
            };
            Assert.Equal(value, control.MultiColumn);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.MultiColumn = value;
            Assert.Equal(value, control.MultiColumn);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.MultiColumn = !value;
            Assert.Equal(!value, control.MultiColumn);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> MultiColumn_SetWithHandle_TestData()
        {
            yield return new object[] { DrawMode.Normal, true, 1 };
            yield return new object[] { DrawMode.Normal, false, 0 };
            yield return new object[] { DrawMode.OwnerDrawFixed, true, 1 };
            yield return new object[] { DrawMode.OwnerDrawFixed, false, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(MultiColumn_SetWithHandle_TestData))]
        public void ListBox_MultiColumn_SetWithHandle_GetReturnsExpected(DrawMode drawMode, bool value, int expectedCreatedCallCount)
        {
            using var control = new ListBox
            {
                DrawMode = drawMode
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.MultiColumn = value;
            Assert.Equal(value, control.MultiColumn);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set same.
            control.MultiColumn = value;
            Assert.Equal(value, control.MultiColumn);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set different.
            control.MultiColumn = !value;
            Assert.Equal(!value, control.MultiColumn);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount + 1, createdCallCount);
        }

        [WinFormsFact]
        public void ListBox_MultiColumn_SetOwnerDrawVariable_ThrowsArgumentException()
        {
            using var control = new ListBox
            {
                DrawMode = DrawMode.OwnerDrawVariable
            };
            control.MultiColumn = false;
            Assert.False(control.MultiColumn);

            Assert.Throws<ArgumentException>("value", () => control.MultiColumn = true);
            Assert.False(control.MultiColumn);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetPaddingNormalizedTheoryData))]
        public void ListBox_Padding_Set_GetReturnsExpected(Padding value, Padding expected)
        {
            using var control = new ListBox
            {
                Padding = value
            };
            Assert.Equal(expected, control.Padding);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Padding = value;
            Assert.Equal(expected, control.Padding);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetPaddingNormalizedTheoryData))]
        public void ListBox_Padding_SetWithHandle_GetReturnsExpected(Padding value, Padding expected)
        {
            using var control = new ListBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Padding = value;
            Assert.Equal(expected, control.Padding);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Padding = value;
            Assert.Equal(expected, control.Padding);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ListBox_Padding_SetWithHandler_CallsPaddingChanged()
        {
            using var control = new ListBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(control, sender);
                Assert.Equal(EventArgs.Empty, e);
                callCount++;
            };
            control.PaddingChanged += handler;

            // Set different.
            var padding1 = new Padding(1);
            control.Padding = padding1;
            Assert.Equal(padding1, control.Padding);
            Assert.Equal(1, callCount);

            // Set same.
            control.Padding = padding1;
            Assert.Equal(padding1, control.Padding);
            Assert.Equal(1, callCount);

            // Set different.
            var padding2 = new Padding(2);
            control.Padding = padding2;
            Assert.Equal(padding2, control.Padding);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.PaddingChanged -= handler;
            control.Padding = padding1;
            Assert.Equal(padding1, control.Padding);
            Assert.Equal(2, callCount);
        }

        public static IEnumerable<object[]> PreferredHeight_GetEmpty_TestData()
        {
            int extra = SystemInformation.BorderSize.Height * 4 + 3;
            yield return new object[] { DrawMode.Normal, BorderStyle.Fixed3D, 13 + extra };
            yield return new object[] { DrawMode.Normal, BorderStyle.FixedSingle, 13 + extra };
            yield return new object[] { DrawMode.Normal, BorderStyle.None, 13 };

            yield return new object[] { DrawMode.OwnerDrawFixed, BorderStyle.Fixed3D, 13 + extra };
            yield return new object[] { DrawMode.OwnerDrawFixed, BorderStyle.FixedSingle, 13 + extra };
            yield return new object[] { DrawMode.OwnerDrawFixed, BorderStyle.None, 13 };

            yield return new object[] { DrawMode.OwnerDrawVariable, BorderStyle.Fixed3D, extra };
            yield return new object[] { DrawMode.OwnerDrawVariable, BorderStyle.FixedSingle, extra };
            yield return new object[] { DrawMode.OwnerDrawVariable, BorderStyle.None, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(PreferredHeight_GetEmpty_TestData))]
        public void ListBox_PreferredHeight_GetEmpty_ReturnsExpected(DrawMode drawMode, BorderStyle borderStyle, int expected)
        {
            using var control = new ListBox
            {
                DrawMode = drawMode,
                BorderStyle = borderStyle
            };
            Assert.Equal(expected, control.PreferredHeight);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> PreferredHeight_GetNotEmpty_TestData()
        {
            int extra = SystemInformation.BorderSize.Height * 4 + 3;
            yield return new object[] { DrawMode.Normal, BorderStyle.Fixed3D, 26 + extra };
            yield return new object[] { DrawMode.Normal, BorderStyle.FixedSingle, 26 + extra };
            yield return new object[] { DrawMode.Normal, BorderStyle.None, 26 };

            yield return new object[] { DrawMode.OwnerDrawFixed, BorderStyle.Fixed3D, 26 + extra };
            yield return new object[] { DrawMode.OwnerDrawFixed, BorderStyle.FixedSingle, 26 + extra };
            yield return new object[] { DrawMode.OwnerDrawFixed, BorderStyle.None, 26 };

            yield return new object[] { DrawMode.OwnerDrawVariable, BorderStyle.Fixed3D, 26 + extra };
            yield return new object[] { DrawMode.OwnerDrawVariable, BorderStyle.FixedSingle, 26 + extra };
            yield return new object[] { DrawMode.OwnerDrawVariable, BorderStyle.None, 26 };
        }

        [WinFormsTheory]
        [MemberData(nameof(PreferredHeight_GetNotEmpty_TestData))]
        public void ListBox_PreferredHeight_GetNotEmpty_ReturnsExpected(DrawMode drawMode, BorderStyle borderStyle, int expected)
        {
            using var control = new ListBox
            {
                DrawMode = drawMode,
                BorderStyle = borderStyle
            };
            control.Items.Add("Item1");
            control.Items.Add("Item2");
            Assert.Equal(expected, control.PreferredHeight);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> PreferredHeight_GetWithHandle_TestData()
        {
            yield return new object[] { DrawMode.Normal, BorderStyle.Fixed3D };
            yield return new object[] { DrawMode.Normal, BorderStyle.FixedSingle };
            yield return new object[] { DrawMode.Normal, BorderStyle.None };

            yield return new object[] { DrawMode.OwnerDrawFixed, BorderStyle.Fixed3D };
            yield return new object[] { DrawMode.OwnerDrawFixed, BorderStyle.FixedSingle };
            yield return new object[] { DrawMode.OwnerDrawFixed, BorderStyle.None };

            yield return new object[] { DrawMode.OwnerDrawVariable, BorderStyle.Fixed3D };
            yield return new object[] { DrawMode.OwnerDrawVariable, BorderStyle.FixedSingle };
            yield return new object[] { DrawMode.OwnerDrawVariable, BorderStyle.None };
        }

        [WinFormsTheory]
        [MemberData(nameof(PreferredHeight_GetWithHandle_TestData))]
        public void ListBox_PreferredHeight_GetEmptyWithHandle_ReturnsExpected(DrawMode drawMode, BorderStyle borderStyle)
        {
            using var control = new ListBox
            {
                DrawMode = drawMode,
                BorderStyle = borderStyle
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.True(control.PreferredHeight >= 0);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(PreferredHeight_GetWithHandle_TestData))]
        public void ListBox_PreferredHeight_GetNotEmptyWithHandle_ReturnsExpected(DrawMode drawMode, BorderStyle borderStyle)
        {
            using var control = new ListBox
            {
                DrawMode = drawMode,
                BorderStyle = borderStyle
            };
            control.Items.Add("Item1");
            control.Items.Add("Item2");
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.True(control.PreferredHeight > 0);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetRightToLeftTheoryData))]
        public void ListBox_RightToLeft_Set_GetReturnsExpected(RightToLeft value, RightToLeft expected)
        {
            using var control = new ListBox
            {
                RightToLeft = value
            };
            Assert.Equal(expected, control.RightToLeft);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.RightToLeft = value;
            Assert.Equal(expected, control.RightToLeft);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBox_RightToLeft_SetWithHandler_CallsRightToLeftChanged()
        {
            using var control = new ListBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.RightToLeftChanged += handler;

            // Set different.
            control.RightToLeft = RightToLeft.Yes;
            Assert.Equal(RightToLeft.Yes, control.RightToLeft);
            Assert.Equal(1, callCount);

            // Set same.
            control.RightToLeft = RightToLeft.Yes;
            Assert.Equal(RightToLeft.Yes, control.RightToLeft);
            Assert.Equal(1, callCount);

            // Set different.
            control.RightToLeft = RightToLeft.Inherit;
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.RightToLeftChanged -= handler;
            control.RightToLeft = RightToLeft.Yes;
            Assert.Equal(RightToLeft.Yes, control.RightToLeft);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(RightToLeft))]
        public void ListBox_RightToLeft_SetInvalid_ThrowsInvalidEnumArgumentException(RightToLeft value)
        {
            using var control = new ListBox();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.RightToLeft = value);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListBox_ScrollAlwaysVisible_Set_GetReturnsExpected(bool value)
        {
            using var control = new ListBox
            {
                ScrollAlwaysVisible = value
            };
            Assert.Equal(value, control.ScrollAlwaysVisible);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.ScrollAlwaysVisible = value;
            Assert.Equal(value, control.ScrollAlwaysVisible);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.ScrollAlwaysVisible = !value;
            Assert.Equal(!value, control.ScrollAlwaysVisible);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void ListBox_ScrollAlwaysVisible_SetWithHandle_GetReturnsExpected(bool value, int expectedCreatedCallCount)
        {
            using var control = new ListBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.ScrollAlwaysVisible = value;
            Assert.Equal(value, control.ScrollAlwaysVisible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set same.
            control.ScrollAlwaysVisible = value;
            Assert.Equal(value, control.ScrollAlwaysVisible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set different.
            control.ScrollAlwaysVisible = !value;
            Assert.Equal(!value, control.ScrollAlwaysVisible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount + 1, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(SelectionMode))]
        public void ListBox_SelectedIndex_GetEmptyWithHandle_ReturnsMinusOne(SelectionMode selectionMode)
        {
            using var control = new ListBox
            {
                SelectionMode = selectionMode
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            Assert.Equal(-1, control.SelectedIndex);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(SelectionMode))]
        public void ListBox_SelectedIndex_GetNotEmptyWithHandle_ReturnsMinusOne(SelectionMode selectionMode)
        {
            using var control = new ListBox
            {
                SelectionMode = selectionMode
            };
            control.Items.Add("Item");
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            Assert.Equal(-1, control.SelectedIndex);
        }

        [WinFormsTheory]
        [InlineData(SelectionMode.MultiExtended)]
        [InlineData(SelectionMode.MultiSimple)]
        [InlineData(SelectionMode.One)]
        public void ListBox_SelectedIndex_SetEmpty_GetReturnsExpected(SelectionMode selectionMode)
        {
            using var control = new ListBox
            {
                SelectionMode = selectionMode,
                SelectedIndex = -1
            };
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.SelectedIndices);
            Assert.Empty(control.SelectedItems);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.SelectedIndex = -1;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.SelectedIndices);
            Assert.Empty(control.SelectedItems);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBox_SelectedIndex_SetSelectionModeOne_GetReturnsExpected()
        {
            using var control = new ListBox
            {
                SelectionMode = SelectionMode.One
            };
            control.Items.Add("item1");
            control.Items.Add("item2");
            control.Items.Add("item1");

            // Select end.
            control.SelectedIndex = 1;
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.False(control.IsHandleCreated);

            // Select same.
            control.SelectedIndex = 1;
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.False(control.IsHandleCreated);

            // Select first.
            control.SelectedIndex = 0;
            Assert.Equal(0, control.SelectedIndex);
            Assert.Equal(0, Assert.Single(control.SelectedIndices));
            Assert.Equal("item1", Assert.Single(control.SelectedItems));
            Assert.False(control.IsHandleCreated);

            // Clear selection.
            control.SelectedIndex = -1;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.SelectedIndices);
            Assert.Empty(control.SelectedItems);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(SelectionMode.MultiExtended)]
        [InlineData(SelectionMode.MultiSimple)]
        public void ListBox_SelectedIndex_SetSelectionModeMultiple_GetReturnsExpected(SelectionMode selectionMode)
        {
            using var control = new ListBox
            {
                SelectionMode = selectionMode
            };
            control.Items.Add("item1");
            control.Items.Add("item2");
            control.Items.Add("item1");

            // Select end.
            control.SelectedIndex = 1;
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.False(control.IsHandleCreated);

            // Select same.
            control.SelectedIndex = 1;
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.False(control.IsHandleCreated);

            // Select first.
            control.SelectedIndex = 0;
            Assert.Equal(0, control.SelectedIndex);
            Assert.Equal("item1", control.SelectedItem);
            Assert.Equal(new int[] { 0, 1 }, control.SelectedIndices.Cast<int>());
            Assert.Equal(new string[] { "item1", "item2" }, control.SelectedItems.Cast<string>());
            Assert.False(control.IsHandleCreated);

            // Clear selection.
            control.SelectedIndex = -1;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.SelectedIndices);
            Assert.Empty(control.SelectedItems);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 0)]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        [InlineData(false, 1)]
        public void ListBox_SelectedIndex_SetWithDataManager_SetsDataManagerPosition(bool formattingEnabled, int position)
        {
            var bindingContext = new BindingContext();
            var dataSource = new List<string> { "item1", "item2", "item3" };
            using var control = new SubListBox
            {
                BindingContext = bindingContext,
                DataSource = dataSource,
                FormattingEnabled = formattingEnabled
            };
            control.DataManager.Position = position;

            // Select end.
            control.SelectedIndex = 1;
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.Equal(1, control.DataManager.Position);
            Assert.False(control.IsHandleCreated);

            // Select same.
            control.SelectedIndex = 1;
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.Equal(1, control.DataManager.Position);
            Assert.False(control.IsHandleCreated);

            // Select first.
            control.SelectedIndex = 0;
            Assert.Equal(0, control.SelectedIndex);
            Assert.Equal("item1", control.SelectedItem);
            Assert.Equal(0, Assert.Single(control.SelectedIndices));
            Assert.Equal("item1", Assert.Single(control.SelectedItems));
            Assert.Equal(0, control.DataManager.Position);
            Assert.False(control.IsHandleCreated);

            // Clear selection.
            control.SelectedIndex = -1;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.SelectedIndices);
            Assert.Empty(control.SelectedItems);
            Assert.Equal(0, control.DataManager.Position);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBox_SelectedIndex_SetSelectionModeOneWithHandle_GetReturnsExpected()
        {
            using var control = new ListBox
            {
                SelectionMode = SelectionMode.One
            };
            control.Items.Add("item1");
            control.Items.Add("item2");
            control.Items.Add("item1");
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            // Select end.
            control.SelectedIndex = 1;
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Select same.
            control.SelectedIndex = 1;
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Select first.
            control.SelectedIndex = 0;
            Assert.Equal(0, control.SelectedIndex);
            Assert.Equal(0, Assert.Single(control.SelectedIndices));
            Assert.Equal("item1", Assert.Single(control.SelectedItems));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Clear selection.
            control.SelectedIndex = -1;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.SelectedIndices);
            Assert.Empty(control.SelectedItems);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(SelectionMode.MultiExtended)]
        [InlineData(SelectionMode.MultiSimple)]
        public void ListBox_SelectedIndex_SetSelectionModeMultipleWithHandle_GetReturnsExpected(SelectionMode selectionMode)
        {
            using var control = new ListBox
            {
                SelectionMode = selectionMode
            };
            control.Items.Add("item1");
            control.Items.Add("item2");
            control.Items.Add("item1");
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            // Select end.
            control.SelectedIndex = 1;
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Select same.
            control.SelectedIndex = 1;
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Select first.
            control.SelectedIndex = 0;
            Assert.Equal(0, control.SelectedIndex);
            Assert.Equal("item1", control.SelectedItem);
            Assert.Equal(new int[] { 0, 1 }, control.SelectedIndices.Cast<int>());
            Assert.Equal(new string[] { "item1", "item2" }, control.SelectedItems.Cast<string>());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Clear selection.
            control.SelectedIndex = -1;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.SelectedIndices);
            Assert.Empty(control.SelectedItems);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ListBox_SelectedIndex_GetCurSelOne_Success()
        {
            using var control = new ListBox
            {
                SelectionMode = SelectionMode.One
            };
            control.Items.Add("item1");
            control.Items.Add("item2");
            control.Items.Add("item1");

            // Select last.
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.SelectedIndex = 1;
            Assert.Equal((IntPtr)1, SendMessageW(control.Handle, (WM)LB.GETCURSEL));

            // Select first.
            control.SelectedIndex = 0;
            Assert.Equal((IntPtr)0, SendMessageW(control.Handle, (WM)LB.GETCURSEL));

            // Clear selection.
            control.SelectedIndex = -1;
            Assert.Equal((IntPtr)(-1), SendMessageW(control.Handle, (WM)LB.GETCURSEL));
        }

        [WinFormsTheory]
        [InlineData(SelectionMode.MultiExtended)]
        [InlineData(SelectionMode.MultiSimple)]
        public void ListBox_SelectedIndex_GetCurSelMultiple_Success(SelectionMode selectionMode)
        {
            using var control = new ListBox
            {
                SelectionMode = selectionMode
            };
            control.Items.Add("item1");
            control.Items.Add("item2");
            control.Items.Add("item1");

            // Select last.
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.SelectedIndex = 1;
            Assert.Equal((IntPtr)1, SendMessageW(control.Handle, (WM)LB.GETCURSEL));
            Span<int> buffer = stackalloc int[5];
            Assert.Equal((IntPtr)1, SendMessageW(control.Handle, (WM)LB.GETSELITEMS, (IntPtr)buffer.Length, ref buffer[0]));
            Assert.Equal(new int[] { 1, 0, 0, 0, 0 }, buffer.ToArray());

            // Select first.
            control.SelectedIndex = 0;
            Assert.Equal((IntPtr)0, SendMessageW(control.Handle, (WM)LB.GETCURSEL));
            Assert.Equal((IntPtr)2, SendMessageW(control.Handle, (WM)LB.GETSELITEMS, (IntPtr)buffer.Length, ref buffer[0]));
            Assert.Equal(new int[] { 0, 1, 0, 0, 0 }, buffer.ToArray());

            // Clear selection.
            control.SelectedIndex = -1;
            Assert.Equal((IntPtr)0, SendMessageW(control.Handle, (WM)LB.GETCURSEL));
            Assert.Equal((IntPtr)0, SendMessageW(control.Handle, (WM)LB.GETSELITEMS, (IntPtr)buffer.Length, ref buffer[0]));
            Assert.Equal(new int[] { 0, 1, 0, 0, 0 }, buffer.ToArray());
        }

        [WinFormsTheory]
        [InlineData(SelectionMode.One)]
        [InlineData(SelectionMode.MultiExtended)]
        [InlineData(SelectionMode.MultiSimple)]
        public void ListBox_SelectedIndex_SetWithHandler_CallsSelectedIndexChanged(SelectionMode selectionMode)
        {
            using var control = new ListBox
            {
                SelectionMode = selectionMode
            };
            control.Items.Add("item1");
            control.Items.Add("item2");
            control.Items.Add("item1");
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.SelectedIndexChanged += handler;

            // Select last.
            control.SelectedIndex = 1;
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, callCount);

            // Select same.
            control.SelectedIndex = 1;
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, callCount);

            // Select first.
            control.SelectedIndex = 0;
            Assert.Equal(0, control.SelectedIndex);
            Assert.Equal(2, callCount);

            // Clear selection.
            control.SelectedIndex = -1;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(3, callCount);

            // Remove handler.
            control.SelectedIndexChanged -= handler;
            control.SelectedIndex = 1;
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal(3, callCount);
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(0)]
        [InlineData(1)]
        public void ListBox_SelectedIndex_SetInvalidValueEmpty_ThrowsArgumentOutOfRangeException(int value)
        {
            using var control = new ListBox();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => control.SelectedIndex = value);
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(1)]
        public void ListBox_SelectedIndex_SetInvalidValueNotEmpty_ThrowsArgumentOutOfRangeException(int value)
        {
            using var control = new ListBox();
            control.Items.Add("Item");
            Assert.Throws<ArgumentOutOfRangeException>("value", () => control.SelectedIndex = value);
        }

        [WinFormsFact]
        public void ListBox_SelectedIndex_SetNoSelection_ThrowsArgumentException()
        {
            using var control = new ListBox
            {
                SelectionMode = SelectionMode.None
            };
            Assert.Throws<ArgumentException>("value", () => control.SelectedIndex = -1);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(SelectionMode))]
        public void ListBox_SelectedItem_GetEmptyWithHandle_ReturnsNull(SelectionMode selectionMode)
        {
            using var control = new ListBox
            {
                SelectionMode = selectionMode
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            Assert.Null(control.SelectedItem);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(SelectionMode))]
        public void ListBox_SelectedItem_GetNotEmptyWithHandle_ReturnsNull(SelectionMode selectionMode)
        {
            using var control = new ListBox
            {
                SelectionMode = selectionMode
            };
            control.Items.Add("Item");
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            Assert.Null(control.SelectedItem);
        }

        [WinFormsTheory]
        [InlineData(SelectionMode.MultiExtended, null)]
        [InlineData(SelectionMode.MultiSimple, null)]
        [InlineData(SelectionMode.One, null)]
        [InlineData(SelectionMode.MultiExtended, "item")]
        [InlineData(SelectionMode.MultiSimple, "item")]
        [InlineData(SelectionMode.One, "item")]
        public void ListBox_SelectedItem_SetEmpty_GetReturnsExpected(SelectionMode selectionMode, string value)
        {
            using var control = new ListBox
            {
                SelectionMode = selectionMode,
                SelectedItem = value
            };
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.SelectedIndices);
            Assert.Empty(control.SelectedItems);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.SelectedItem = value;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.SelectedIndices);
            Assert.Empty(control.SelectedItems);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBox_SelectedItem_SetSelectionModeOne_GetReturnsExpected()
        {
            using var control = new ListBox
            {
                SelectionMode = SelectionMode.One
            };
            control.Items.Add("item1");
            control.Items.Add("item2");
            control.Items.Add("item1");

            // Select end.
            control.SelectedItem = "item2";
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.False(control.IsHandleCreated);

            // Select same.
            control.SelectedItem = "item2";
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.False(control.IsHandleCreated);

            // Select invalid.
            control.SelectedItem = "NoSuchItem";
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.False(control.IsHandleCreated);

            // Select first.
            control.SelectedItem = "item1";
            Assert.Equal(0, control.SelectedIndex);
            Assert.Equal(0, Assert.Single(control.SelectedIndices));
            Assert.Equal("item1", Assert.Single(control.SelectedItems));
            Assert.False(control.IsHandleCreated);

            // Clear selection.
            control.SelectedItem = null;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.SelectedIndices);
            Assert.Empty(control.SelectedItems);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(SelectionMode.MultiExtended)]
        [InlineData(SelectionMode.MultiSimple)]
        public void ListBox_SelectedItem_SetSelectionModeMultiple_GetReturnsExpected(SelectionMode selectionMode)
        {
            using var control = new ListBox
            {
                SelectionMode = selectionMode
            };
            control.Items.Add("item1");
            control.Items.Add("item2");
            control.Items.Add("item1");

            // Select end.
            control.SelectedItem = "item2";
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.False(control.IsHandleCreated);

            // Select same.
            control.SelectedItem = "item2";
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.False(control.IsHandleCreated);

            // Select invalid.
            control.SelectedItem = "NoSuchItem";
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.False(control.IsHandleCreated);

            // Select first.
            control.SelectedItem = "item1";
            Assert.Equal(0, control.SelectedIndex);
            Assert.Equal("item1", control.SelectedItem);
            Assert.Equal(new int[] { 0, 1 }, control.SelectedIndices.Cast<int>());
            Assert.Equal(new string[] { "item1", "item2" }, control.SelectedItems.Cast<string>());
            Assert.False(control.IsHandleCreated);

            // Clear selection.
            control.SelectedItem = null;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.SelectedIndices);
            Assert.Empty(control.SelectedItems);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 0)]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        [InlineData(false, 1)]
        public void ListBox_SelectedItem_SetWithDataManager_SetsDataManagerPosition(bool formattingEnabled, int position)
        {
            var bindingContext = new BindingContext();
            var dataSource = new List<string> { "item1", "item2", "item3" };
            using var control = new SubListBox
            {
                BindingContext = bindingContext,
                DataSource = dataSource,
                FormattingEnabled = formattingEnabled
            };
            control.DataManager.Position = position;

            // Select end.
            control.SelectedItem = "item2";
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.Equal(1, control.DataManager.Position);
            Assert.False(control.IsHandleCreated);

            // Select same.
            control.SelectedItem = "item2";
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.Equal(1, control.DataManager.Position);
            Assert.False(control.IsHandleCreated);

            // Select invalid.
            control.SelectedItem = "NoSuchItem";
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.Equal(1, control.DataManager.Position);
            Assert.False(control.IsHandleCreated);

            // Select first.
            control.SelectedItem = "item1";
            Assert.Equal(0, control.SelectedIndex);
            Assert.Equal("item1", control.SelectedItem);
            Assert.Equal(0, Assert.Single(control.SelectedIndices));
            Assert.Equal("item1", Assert.Single(control.SelectedItems));
            Assert.Equal(0, control.DataManager.Position);
            Assert.False(control.IsHandleCreated);

            // Clear selection.
            control.SelectedItem = null;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.SelectedIndices);
            Assert.Empty(control.SelectedItems);
            Assert.Equal(0, control.DataManager.Position);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBox_SelectedItem_SetSelectionModeOneWithHandle_GetReturnsExpected()
        {
            using var control = new ListBox
            {
                SelectionMode = SelectionMode.One
            };
            control.Items.Add("item1");
            control.Items.Add("item2");
            control.Items.Add("item1");
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            // Select end.
            control.SelectedItem = "item2";
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Select same.
            control.SelectedItem = "item2";
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Select invalid.
            control.SelectedItem = "NoSuchItem";
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Select first.
            control.SelectedItem = "item1";
            Assert.Equal(0, control.SelectedIndex);
            Assert.Equal(0, Assert.Single(control.SelectedIndices));
            Assert.Equal("item1", Assert.Single(control.SelectedItems));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Clear selection.
            control.SelectedItem = null;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.SelectedIndices);
            Assert.Empty(control.SelectedItems);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(SelectionMode.MultiExtended)]
        [InlineData(SelectionMode.MultiSimple)]
        public void ListBox_SelectedItem_SetSelectionModeMultipleWithHandle_GetReturnsExpected(SelectionMode selectionMode)
        {
            using var control = new ListBox
            {
                SelectionMode = selectionMode
            };
            control.Items.Add("item1");
            control.Items.Add("item2");
            control.Items.Add("item1");
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            // Select end.
            control.SelectedItem = "item2";
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Select same.
            control.SelectedItem = "item2";
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Select invalid.
            control.SelectedItem = "NoSuchItem";
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Select first.
            control.SelectedItem = "item1";
            Assert.Equal(0, control.SelectedIndex);
            Assert.Equal("item1", control.SelectedItem);
            Assert.Equal(new int[] { 0, 1 }, control.SelectedIndices.Cast<int>());
            Assert.Equal(new string[] { "item1", "item2" }, control.SelectedItems.Cast<string>());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Clear selection.
            control.SelectedItem = null;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.SelectedIndices);
            Assert.Empty(control.SelectedItems);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ListBox_SelectedItem_GetCurSelOne_Success()
        {
            using var control = new ListBox
            {
                SelectionMode = SelectionMode.One
            };
            control.Items.Add("item1");
            control.Items.Add("item2");
            control.Items.Add("item1");

            // Select last.
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.SelectedItem = "item2";
            Assert.Equal((IntPtr)1, SendMessageW(control.Handle, (WM)LB.GETCURSEL));

            // Select invalid.
            control.SelectedItem = "NoSuchItem";
            Assert.Equal((IntPtr)1, SendMessageW(control.Handle, (WM)LB.GETCURSEL));

            // Select first.
            control.SelectedItem = "item1";
            Assert.Equal((IntPtr)0, SendMessageW(control.Handle, (WM)LB.GETCURSEL));

            // Clear selection.
            control.SelectedItem = null;
            Assert.Equal((IntPtr)(-1), SendMessageW(control.Handle, (WM)LB.GETCURSEL));
        }

        [WinFormsTheory]
        [InlineData(SelectionMode.MultiExtended)]
        [InlineData(SelectionMode.MultiSimple)]
        public void ListBox_SelectedItem_GetCurSelMultiple_Success(SelectionMode selectionMode)
        {
            using var control = new ListBox
            {
                SelectionMode = selectionMode
            };
            control.Items.Add("item1");
            control.Items.Add("item2");
            control.Items.Add("item1");

            // Select last.
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.SelectedItem = "item2";
            Assert.Equal((IntPtr)1, SendMessageW(control.Handle, (WM)LB.GETCURSEL));
            Span<int> buffer = stackalloc int[5];
            Assert.Equal((IntPtr)1, SendMessageW(control.Handle, (WM)LB.GETSELITEMS, (IntPtr)buffer.Length, ref buffer[0]));
            Assert.Equal(new int[] { 1, 0, 0, 0, 0 }, buffer.ToArray());

            // Select invalid.
            control.SelectedItem = "NoSuchItem";
            Assert.Equal((IntPtr)1, SendMessageW(control.Handle, (WM)LB.GETCURSEL));
            buffer = stackalloc int[5];
            Assert.Equal((IntPtr)1, SendMessageW(control.Handle, (WM)LB.GETSELITEMS, (IntPtr)buffer.Length, ref buffer[0]));
            Assert.Equal(new int[] { 1, 0, 0, 0, 0 }, buffer.ToArray());

            // Select first.
            control.SelectedItem = "item1";
            Assert.Equal((IntPtr)0, SendMessageW(control.Handle, (WM)LB.GETCURSEL));
            Assert.Equal((IntPtr)2, SendMessageW(control.Handle, (WM)LB.GETSELITEMS, (IntPtr)buffer.Length, ref buffer[0]));
            Assert.Equal(new int[] { 0, 1, 0, 0, 0 }, buffer.ToArray());

            // Clear selection.
            control.SelectedItem = null;
            Assert.Equal((IntPtr)0, SendMessageW(control.Handle, (WM)LB.GETCURSEL));
            Assert.Equal((IntPtr)0, SendMessageW(control.Handle, (WM)LB.GETSELITEMS, (IntPtr)buffer.Length, ref buffer[0]));
            Assert.Equal(new int[] { 0, 1, 0, 0, 0 }, buffer.ToArray());
        }

        [WinFormsTheory]
        [InlineData(SelectionMode.One)]
        [InlineData(SelectionMode.MultiExtended)]
        [InlineData(SelectionMode.MultiSimple)]
        public void ListBox_SelectedItem_SetWithHandler_CallsSelectedIndexChanged(SelectionMode selectionMode)
        {
            using var control = new ListBox
            {
                SelectionMode = selectionMode
            };
            control.Items.Add("item1");
            control.Items.Add("item2");
            control.Items.Add("item1");
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.SelectedIndexChanged += handler;

            // Select last.
            control.SelectedItem = "item2";
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, callCount);

            // Select same.
            control.SelectedItem = "item2";
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, callCount);

            // Select invalid.
            control.SelectedItem = "NoSuchItem";
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, callCount);

            // Select first.
            control.SelectedItem = "item1";
            Assert.Equal(0, control.SelectedIndex);
            Assert.Equal(2, callCount);

            // Clear selection.
            control.SelectedItem = null;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(3, callCount);

            // Remove handler.
            control.SelectedIndexChanged -= handler;
            control.SelectedItem = "item2";
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal(3, callCount);
        }

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData("NoSuchItem")]
        public void ListBox_SelectedItem_SetNoSelectionEmpty_Nop(object value)
        {
            using var control = new ListBox
            {
                SelectionMode = SelectionMode.None
            };
            control.SelectedItem = value;
            Assert.Null(control.SelectedItem);
        }

        [WinFormsFact]
        public void ListBox_SelectedItem_SetNoSelectionNotEmpty_ThrowsArgumentException()
        {
            using var control = new ListBox
            {
                SelectionMode = SelectionMode.None
            };
            control.Items.Add("item");
            AssertExtensions.Throws<ArgumentException>("value", () => control.SelectedItem = "item");
            Assert.Null(control.SelectedItem);

            control.SelectedItem = "NoSuchItem";
            Assert.Null(control.SelectedItem);

            AssertExtensions.Throws<ArgumentException>("value", () => control.SelectedItem = null);
            Assert.Null(control.SelectedItem);
        }

        [WinFormsFact]
        public void ListBox_SelectedItems_GetDirtyCustom_ReturnsExpected()
        {
            using var control = new CustomListBox
            {
                SelectionMode = SelectionMode.MultiSimple
            };
            control.Items.Add("Item0");
            control.Items.Add("Item1");
            control.Items.Add("Item2");
            control.Items.Add("Item3");
            control.Items.Add("Item4");
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            // Set MakeCustom after the Handle is created to allow for default behaviour.
            control.MakeCustom = true;

            // Verify equal lengths.
            control.GetSelCountResult = (IntPtr)1;
            control.GetSelResult = new int[] { 2 };
            Dirty();
            Assert.Equal(new int[] { 2 }, control.SelectedIndices.Cast<int>());

            // Verify truncated
            control.GetSelCountResult = (IntPtr)2;
            control.GetSelResult = new int[] { 2 };
            Dirty();
            Assert.Equal(new int[] { 0, 2 }, control.SelectedIndices.Cast<int>());

            void Dirty()
            {
                // Simulate a selection change notification.
                SendMessageW(control.Handle, WM.REFLECT | WM.COMMAND, PARAM.FromLowHigh(0, (int)LBN.SELCHANGE));
            }
        }

        private class CustomListBox : ListBox
        {
            public bool MakeCustom { get; set; }

            public IntPtr GetSelCountResult { get; set; }
            public int[] GetSelResult { get; set; }

            protected override void WndProc(ref Message m)
            {
                if (MakeCustom && m.Msg == (int)LB.GETSELCOUNT)
                {
                    m.Result = GetSelCountResult;
                    return;
                }
                else if (MakeCustom && m.Msg == (int)LB.GETSELITEMS)
                {
                    Assert.Equal(GetSelCountResult, m.WParam);
                    Marshal.Copy(GetSelResult, 0, m.LParam, GetSelResult.Length);
                    m.Result = (IntPtr)GetSelResult.Length;
                    return;
                }

                base.WndProc(ref m);
            }
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(SelectionMode))]
        public void ListBox_SelectionMode_SetEmpty_GetReturnsExpected(SelectionMode value)
        {
            using var control = new ListBox
            {
                SelectionMode = value
            };
            Assert.Equal(value, control.SelectionMode);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.SelectedIndices);
            Assert.Empty(control.SelectedItems);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.SelectionMode = value;
            Assert.Equal(value, control.SelectionMode);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.SelectedIndices);
            Assert.Empty(control.SelectedItems);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> SelectionMode_SetWithCustomOldValue_TestData()
        {
            foreach (SelectionMode selectionMode in Enum.GetValues(typeof(SelectionMode)))
            {
                foreach (SelectionMode value in Enum.GetValues(typeof(SelectionMode)))
                {
                    yield return new object[] { selectionMode, value };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(SelectionMode_SetWithCustomOldValue_TestData))]
        public void ListBox_SelectionMode_SetEmptyWithCustomOldValue_GetReturnsExpected(SelectionMode selectionMode, SelectionMode value)
        {
            using var control = new ListBox
            {
                SelectionMode = selectionMode
            };

            control.SelectionMode = value;
            Assert.Equal(value, control.SelectionMode);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.SelectedIndices);
            Assert.Empty(control.SelectedItems);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.SelectionMode = value;
            Assert.Equal(value, control.SelectionMode);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.SelectedIndices);
            Assert.Empty(control.SelectedItems);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(SelectionMode.MultiSimple)]
        [InlineData(SelectionMode.MultiExtended)]
        public void ListBox_SelectionMode_SetWithItemsOneSelectedToMulti_GetReturnsExpected(SelectionMode value)
        {
            using var control = new ListBox();
            control.Items.Add("item1");
            control.Items.Add("item2");
            control.Items.Add("item1");
            control.SelectedIndex = 1;

            control.SelectionMode = value;
            Assert.Equal(value, control.SelectionMode);
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.SelectionMode = value;
            Assert.Equal(value, control.SelectionMode);
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.False(control.IsHandleCreated);

            // Set back to one.
            control.SelectionMode = SelectionMode.One;
            Assert.Equal(SelectionMode.One, control.SelectionMode);
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBox_SelectionMode_SetWithItemsOneSelectedToNone_GetReturnsExpected()
        {
            using var control = new ListBox();
            control.Items.Add("item1");
            control.Items.Add("item2");
            control.Items.Add("item1");
            control.SelectedIndex = 1;

            control.SelectionMode = SelectionMode.None;
            Assert.Equal(SelectionMode.None, control.SelectionMode);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.SelectionMode = SelectionMode.None;
            Assert.Equal(SelectionMode.None, control.SelectionMode);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.False(control.IsHandleCreated);

            // Set back to one.
            control.SelectionMode = SelectionMode.One;
            Assert.Equal(SelectionMode.One, control.SelectionMode);
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(SelectionMode.MultiExtended)]
        [InlineData(SelectionMode.MultiSimple)]
        public void ListBox_SelectionMode_SetWithItemsMultiSelectedToOne_GetReturnsExpected(SelectionMode selectionMode)
        {
            using var control = new ListBox
            {
                SelectionMode = selectionMode
            };
            control.Items.Add("item1");
            control.Items.Add("item2");
            control.Items.Add("item1");
            control.SelectedIndex = 0;
            control.SelectedIndex = 1;

            control.SelectionMode = SelectionMode.One;
            Assert.Equal(SelectionMode.One, control.SelectionMode);
            Assert.Equal(0, control.SelectedIndex);
            Assert.Equal("item1", control.SelectedItem);
            Assert.Equal(new int[] { 0, 1 }, control.SelectedIndices.Cast<int>());
            Assert.Equal(new string[] { "item1", "item2" }, control.SelectedItems.Cast<object>());
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.SelectionMode = SelectionMode.One;
            Assert.Equal(SelectionMode.One, control.SelectionMode);
            Assert.Equal(0, control.SelectedIndex);
            Assert.Equal("item1", control.SelectedItem);
            Assert.Equal(new int[] { 0, 1 }, control.SelectedIndices.Cast<int>());
            Assert.Equal(new string[] { "item1", "item2" }, control.SelectedItems.Cast<object>());
            Assert.False(control.IsHandleCreated);

            // Set back to multi.
            control.SelectionMode = selectionMode;
            Assert.Equal(selectionMode, control.SelectionMode);
            Assert.Equal(0, control.SelectedIndex);
            Assert.Equal("item1", control.SelectedItem);
            Assert.Equal(new int[] { 0, 1 }, control.SelectedIndices.Cast<int>());
            Assert.Equal(new string[] { "item1", "item2" }, control.SelectedItems.Cast<object>());
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(SelectionMode.MultiExtended)]
        [InlineData(SelectionMode.MultiSimple)]
        public void ListBox_SelectionMode_SetWithItemsMultiSelectedToNone_GetReturnsExpected(SelectionMode selectionMode)
        {
            using var control = new ListBox
            {
                SelectionMode = selectionMode
            };
            control.Items.Add("item1");
            control.Items.Add("item2");
            control.Items.Add("item1");
            control.SelectedIndex = 0;
            control.SelectedIndex = 1;

            control.SelectionMode = SelectionMode.None;
            Assert.Equal(SelectionMode.None, control.SelectionMode);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal("item1", control.SelectedItem);
            Assert.Equal(new int[] { 0, 1 }, control.SelectedIndices.Cast<int>());
            Assert.Equal(new string[] { "item1", "item2" }, control.SelectedItems.Cast<object>());
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.SelectionMode = SelectionMode.None;
            Assert.Equal(SelectionMode.None, control.SelectionMode);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal("item1", control.SelectedItem);
            Assert.Equal(new int[] { 0, 1 }, control.SelectedIndices.Cast<int>());
            Assert.Equal(new string[] { "item1", "item2" }, control.SelectedItems.Cast<object>());
            Assert.False(control.IsHandleCreated);

            // Set back to multi.
            control.SelectionMode = selectionMode;
            Assert.Equal(selectionMode, control.SelectionMode);
            Assert.Equal(0, control.SelectedIndex);
            Assert.Equal("item1", control.SelectedItem);
            Assert.Equal(new int[] { 0, 1 }, control.SelectedIndices.Cast<int>());
            Assert.Equal(new string[] { "item1", "item2" }, control.SelectedItems.Cast<object>());
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(SelectionMode.None, 1)]
        [InlineData(SelectionMode.MultiExtended, 1)]
        [InlineData(SelectionMode.MultiSimple, 1)]
        [InlineData(SelectionMode.One, 0)]
        public void ListBox_SelectionMode_SetEmptyWithHandle_GetReturnsExpected(SelectionMode value, int expectedCreatedCallCount)
        {
            using var control = new ListBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.SelectionMode = value;
            Assert.Equal(value, control.SelectionMode);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.SelectedIndices);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set same.
            control.SelectionMode = value;
            Assert.Equal(value, control.SelectionMode);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.SelectedIndices);
            Assert.Empty(control.SelectedItems);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);
        }

        public static IEnumerable<object[]> SelectionMode_SetWithCustomOldValueWithHandle_TestData()
        {
            yield return new object[] { SelectionMode.None, SelectionMode.None, 0 };
            yield return new object[] { SelectionMode.None, SelectionMode.MultiExtended, 1 };
            yield return new object[] { SelectionMode.None, SelectionMode.MultiSimple, 1 };
            yield return new object[] { SelectionMode.None, SelectionMode.One, 1 };

            yield return new object[] { SelectionMode.MultiExtended, SelectionMode.None, 1 };
            yield return new object[] { SelectionMode.MultiExtended, SelectionMode.MultiExtended, 0 };
            yield return new object[] { SelectionMode.MultiExtended, SelectionMode.MultiSimple, 1 };
            yield return new object[] { SelectionMode.MultiExtended, SelectionMode.One, 1 };

            yield return new object[] { SelectionMode.MultiSimple, SelectionMode.None, 1 };
            yield return new object[] { SelectionMode.MultiSimple, SelectionMode.MultiExtended, 1 };
            yield return new object[] { SelectionMode.MultiSimple, SelectionMode.MultiSimple, 0 };
            yield return new object[] { SelectionMode.MultiSimple, SelectionMode.One, 1 };

            yield return new object[] { SelectionMode.One, SelectionMode.None, 1 };
            yield return new object[] { SelectionMode.One, SelectionMode.MultiExtended, 1 };
            yield return new object[] { SelectionMode.One, SelectionMode.MultiSimple, 1 };
            yield return new object[] { SelectionMode.One, SelectionMode.One, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(SelectionMode_SetWithCustomOldValueWithHandle_TestData))]
        public void ListBox_SelectionMode_SetEmptyWithCustomOldValueWithHandle_GetReturnsExpected(SelectionMode selectionMode, SelectionMode value, int expectedCreatedCallCount)
        {
            using var control = new ListBox
            {
                SelectionMode = selectionMode
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.SelectionMode = value;
            Assert.Equal(value, control.SelectionMode);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.SelectedIndices);
            Assert.Empty(control.SelectedItems);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set same.
            control.SelectionMode = value;
            Assert.Equal(value, control.SelectionMode);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.SelectedIndices);
            Assert.Empty(control.SelectedItems);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(SelectionMode.MultiSimple)]
        [InlineData(SelectionMode.MultiExtended)]
        public void ListBox_SelectionMode_SetWithItemsOneSelectedToMultiWithHandle_GetReturnsExpected(SelectionMode value)
        {
            using var control = new ListBox();
            control.Items.Add("item1");
            control.Items.Add("item2");
            control.Items.Add("item1");
            control.SelectedIndex = 1;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.SelectionMode = value;
            Assert.Equal(value, control.SelectionMode);
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(1, createdCallCount);

            // Set same.
            control.SelectionMode = value;
            Assert.Equal(value, control.SelectionMode);
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(1, createdCallCount);

            // Set back to one.
            control.SelectionMode = SelectionMode.One;
            Assert.Equal(SelectionMode.One, control.SelectionMode);
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(2, createdCallCount);
        }

        [WinFormsFact]
        public void ListBox_SelectionMode_SetWithItemsOneSelectedToNoneWithHandle_GetReturnsExpected()
        {
            using var control = new ListBox();
            control.Items.Add("item1");
            control.Items.Add("item2");
            control.Items.Add("item1");
            control.SelectedIndex = 1;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.SelectionMode = SelectionMode.None;
            Assert.Equal(SelectionMode.None, control.SelectionMode);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.SelectedIndices);
            Assert.Empty(control.SelectedItems);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(1, createdCallCount);

            // Set same.
            control.SelectionMode = SelectionMode.None;
            Assert.Equal(SelectionMode.None, control.SelectionMode);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.SelectedIndices);
            Assert.Empty(control.SelectedItems);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(1, createdCallCount);

            // Set back to one.
            control.SelectionMode = SelectionMode.One;
            Assert.Equal(SelectionMode.One, control.SelectionMode);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.SelectedIndices);
            Assert.Empty(control.SelectedItems);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(2, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(SelectionMode.MultiExtended)]
        [InlineData(SelectionMode.MultiSimple)]
        public void ListBox_SelectionMode_SetWithItemsMultiSelectedToOneWithHandle_GetReturnsExpected(SelectionMode selectionMode)
        {
            using var control = new ListBox
            {
                SelectionMode = selectionMode
            };
            control.Items.Add("item1");
            control.Items.Add("item2");
            control.Items.Add("item1");
            control.SelectedIndex = 0;
            control.SelectedIndex = 1;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.SelectionMode = SelectionMode.One;
            Assert.Equal(SelectionMode.One, control.SelectionMode);
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(1, createdCallCount);

            // Set same.
            control.SelectionMode = SelectionMode.One;
            Assert.Equal(SelectionMode.One, control.SelectionMode);
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(1, createdCallCount);

            // Set back to multi.
            control.SelectionMode = selectionMode;
            Assert.Equal(selectionMode, control.SelectionMode);
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal("item2", control.SelectedItem);
            Assert.Equal(1, Assert.Single(control.SelectedIndices));
            Assert.Equal("item2", Assert.Single(control.SelectedItems));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(2, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(SelectionMode.MultiExtended)]
        [InlineData(SelectionMode.MultiSimple)]
        public void ListBox_SelectionMode_SetWithItemsMultiSelectedToNoneWithHandle_GetReturnsExpected(SelectionMode selectionMode)
        {
            using var control = new ListBox
            {
                SelectionMode = selectionMode
            };
            control.Items.Add("item1");
            control.Items.Add("item2");
            control.Items.Add("item1");
            control.SelectedIndex = 0;
            control.SelectedIndex = 1;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.SelectionMode = SelectionMode.None;
            Assert.Equal(SelectionMode.None, control.SelectionMode);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.SelectedIndices);
            Assert.Empty(control.SelectedItems);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(1, createdCallCount);

            // Set same.
            control.SelectionMode = SelectionMode.None;
            Assert.Equal(SelectionMode.None, control.SelectionMode);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.SelectedIndices);
            Assert.Empty(control.SelectedItems);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(1, createdCallCount);

            // Set back to multi.
            control.SelectionMode = selectionMode;
            Assert.Equal(selectionMode, control.SelectionMode);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.SelectedIndices);
            Assert.Empty(control.SelectedItems);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(2, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(SelectionMode))]
        public void ListBox_SelectionMode_SetInvalidValue_ThrowsInvalidEnumArgumentException(SelectionMode value)
        {
            using var control = new ListBox();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.SelectionMode = value);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListBox_Sorted_SetWithoutItems_GetReturnsExpected(bool value)
        {
            using var control = new ListBox
            {
                Sorted = value
            };
            Assert.Equal(value, control.Sorted);
            Assert.Empty(control.Items);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Sorted = value;
            Assert.Equal(value, control.Sorted);
            Assert.Empty(control.Items);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.Sorted = !value;
            Assert.Equal(!value, control.Sorted);
            Assert.Empty(control.Items);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListBox_Sorted_SetWithEmptyItems_GetReturnsExpected(bool value)
        {
            using var control = new ListBox();
            Assert.Empty(control.Items);

            control.Sorted = value;
            Assert.Equal(value, control.Sorted);
            Assert.Empty(control.Items);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Sorted = value;
            Assert.Equal(value, control.Sorted);
            Assert.Empty(control.Items);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.Sorted = !value;
            Assert.Equal(!value, control.Sorted);
            Assert.Empty(control.Items);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Sorted_SetWithItems_TestData()
        {
            yield return new object[] { true, new string[] { "item1", "item2" } };
            yield return new object[] { false, new string[] { "item2", "item1" } };
        }

        [WinFormsTheory]
        [MemberData(nameof(Sorted_SetWithItems_TestData))]
        public void ListBox_Sorted_SetWithItems_GetReturnsExpected(bool value, string[] expected)
        {
            using var control = new ListBox();
            control.Items.Add("item2");
            control.Items.Add("item1");

            control.Sorted = value;
            Assert.Equal(value, control.Sorted);
            Assert.Equal(expected, control.Items.Cast<string>());
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Sorted = value;
            Assert.Equal(value, control.Sorted);
            Assert.Equal(expected, control.Items.Cast<string>());
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.Sorted = !value;
            Assert.Equal(!value, control.Sorted);
            Assert.Equal(new string[] { "item1", "item2" }, control.Items.Cast<string>());
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListBox_Sorted_SetWithoutItemsWithHandle_GetReturnsExpected(bool value)
        {
            using var control = new ListBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Sorted = value;
            Assert.Equal(value, control.Sorted);
            Assert.Empty(control.Items);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Sorted = value;
            Assert.Equal(value, control.Sorted);
            Assert.Empty(control.Items);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.Sorted = !value;
            Assert.Equal(!value, control.Sorted);
            Assert.Empty(control.Items);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListBox_Sorted_SetWithEmptyItemsWithHandle_GetReturnsExpected(bool value)
        {
            using var control = new ListBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            Assert.Empty(control.Items);

            control.Sorted = value;
            Assert.Equal(value, control.Sorted);
            Assert.Empty(control.Items);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Sorted = value;
            Assert.Equal(value, control.Sorted);
            Assert.Empty(control.Items);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.Sorted = !value;
            Assert.Equal(!value, control.Sorted);
            Assert.Empty(control.Items);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(Sorted_SetWithItems_TestData))]
        public void ListBox_Sorted_SetWithItemsWithHandle_GetReturnsExpected(bool value, string[] expected)
        {
            using var control = new ListBox();
            control.Items.Add("item2");
            control.Items.Add("item1");
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Sorted = value;
            Assert.Equal(value, control.Sorted);
            Assert.Equal(expected, control.Items.Cast<string>());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Sorted = value;
            Assert.Equal(value, control.Sorted);
            Assert.Equal(expected, control.Items.Cast<string>());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.Sorted = !value;
            Assert.Equal(!value, control.Sorted);
            Assert.Equal(new string[] { "item1", "item2" }, control.Items.Cast<string>());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ListBox_Text_Set_GetReturnsExpected(string value, string expected)
        {
            using var control = new ListBox
            {
                Text = value
            };
            Assert.Equal(expected, control.Text);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Text_SetWithItems_TestData()
        {
            foreach (bool formattingEnabled in new bool[] { true, false })
            {
                yield return new object[] { formattingEnabled, SelectionMode.None, null, string.Empty, -1 };
                yield return new object[] { formattingEnabled, SelectionMode.None, string.Empty, string.Empty, -1 };
                yield return new object[] { formattingEnabled, SelectionMode.None, "NoSuchItem", "NoSuchItem", -1 };
                yield return new object[] { formattingEnabled, SelectionMode.None, "item1", "item1", -1 };
                yield return new object[] { formattingEnabled, SelectionMode.None, "ITEM1", "ITEM1", -1 };
                yield return new object[] { formattingEnabled, SelectionMode.None, "item2", "item2", -1 };

                foreach (SelectionMode selectionMode in new SelectionMode[] { SelectionMode.MultiExtended, SelectionMode.MultiSimple, SelectionMode.One })
                {
                    yield return new object[] { formattingEnabled, selectionMode, null, string.Empty, -1 };
                    yield return new object[] { formattingEnabled, selectionMode, string.Empty, string.Empty, -1 };
                    yield return new object[] { formattingEnabled, selectionMode, "NoSuchItem", "NoSuchItem", -1 };
                    yield return new object[] { formattingEnabled, selectionMode, "item1", "item1", 0 };
                    yield return new object[] { formattingEnabled, selectionMode, "ITEM1", "item1", 0 };
                    yield return new object[] { formattingEnabled, selectionMode, "item2", "item2", 1 };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Text_SetWithItems_TestData))]
        public void ListBox_Text_SetWithItems_GetReturnsExpected(bool formattingEnabled, SelectionMode selectionMode, string value, string expected, int expectedSelectedIndex)
        {
            using var control = new ListBox
            {
                FormattingEnabled = formattingEnabled,
                SelectionMode = selectionMode
            };
            control.Items.Add("item1");
            control.Items.Add("item2");
            control.Items.Add("item1");

            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.Equal(expectedSelectedIndex, control.SelectedIndex);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.Equal(expectedSelectedIndex, control.SelectedIndex);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Text_SetWithItemsWithSelection_TestData()
        {
            foreach (bool formattingEnabled in new bool[] { true, false })
            {
                yield return new object[] { formattingEnabled, SelectionMode.None, null, string.Empty, -1 };
                yield return new object[] { formattingEnabled, SelectionMode.None, string.Empty, string.Empty, -1 };
                yield return new object[] { formattingEnabled, SelectionMode.None, "NoSuchItem", "NoSuchItem", -1 };
                yield return new object[] { formattingEnabled, SelectionMode.None, "item1", "item1", -1 };
                yield return new object[] { formattingEnabled, SelectionMode.None, "ITEM1", "ITEM1", -1 };
                yield return new object[] { formattingEnabled, SelectionMode.None, "item2", "item2", -1 };

                foreach (SelectionMode selectionMode in new SelectionMode[] { SelectionMode.MultiExtended, SelectionMode.MultiSimple, SelectionMode.One })
                {
                    yield return new object[] { formattingEnabled, selectionMode, null, "item1", 0 };
                    yield return new object[] { formattingEnabled, selectionMode, string.Empty, "item1", 0 };
                    yield return new object[] { formattingEnabled, selectionMode, "NoSuchItem", "item1", 0 };
                    yield return new object[] { formattingEnabled, selectionMode, "item1", "item1", 0 };
                    yield return new object[] { formattingEnabled, selectionMode, "ITEM1", "item1", 0 };
                }

                yield return new object[] { formattingEnabled, SelectionMode.MultiExtended, "item2", "item1", 0 };
                yield return new object[] { formattingEnabled, SelectionMode.MultiSimple, "item2", "item1", 0 };
                yield return new object[] { formattingEnabled, SelectionMode.One, "item2", "item2", 1 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Text_SetWithItemsWithSelection_TestData))]
        public void ListBox_Text_SetWithItemsWithSelection_GetReturnsExpected(bool formattingEnabled, SelectionMode selectionMode, string value, string expected, int expectedSelectedIndex)
        {
            using var control = new ListBox
            {
                FormattingEnabled = formattingEnabled
            };
            control.Items.Add("item1");
            control.Items.Add("item2");
            control.Items.Add("item1");
            control.SelectedIndex = 0;
            control.SelectionMode = selectionMode;

            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.Equal(expectedSelectedIndex, control.SelectedIndex);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.Equal(expectedSelectedIndex, control.SelectedIndex);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ListBox_Text_SetWithHandle_GetReturnsExpected(string value, string expected)
        {
            using var control = new ListBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ListBox_Text_SetWithHandler_CallsTextChanged()
        {
            using var control = new ListBox();
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
        public void ListBox_TopIndex_GetWithHandle_ReturnsExpected()
        {
            using var control = new ListBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            Assert.Equal(0, control.TopIndex);
        }

        [WinFormsTheory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        public void ListBox_TopIndex_SetEmpty_GetReturnsExpected(int value)
        {
            using var control = new ListBox
            {
                TopIndex = value
            };
            Assert.Equal(value, control.TopIndex);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.TopIndex = value;
            Assert.Equal(value, control.TopIndex);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        public void ListBox_TopIndex_SetNotEmpty_GetReturnsExpected(int value)
        {
            using var control = new ListBox();
            control.Items.Add("item1");
            control.Items.Add("item2");
            control.Items.Add("item1");

            control.TopIndex = value;
            Assert.Equal(value, control.TopIndex);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.TopIndex = value;
            Assert.Equal(value, control.TopIndex);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        public void ListBox_TopIndex_SetWithHandleEmpty_GetReturnsExpected(int value)
        {
            using var control = new ListBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.TopIndex = value;
            Assert.Equal(0, control.TopIndex);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.TopIndex = value;
            Assert.Equal(0, control.TopIndex);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(int.MaxValue)]
        public void ListBox_TopIndex_SetWithHandleNotEmpty_GetReturnsExpected(int value)
        {
            using var control = new ListBox();
            control.Items.Add("item1");
            control.Items.Add("item2");
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.TopIndex = value;
            Assert.Equal(0, control.TopIndex);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.TopIndex = value;
            Assert.Equal(0, control.TopIndex);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ListBox_TopIndex_GetTopIndex_ReturnsExpected()
        {
            using var control = new ListBox();
            control.Items.Add("item1");
            control.Items.Add("item2");
            control.Items.Add("item1");

            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.TopIndex = 1;
            Assert.Equal((IntPtr)0, SendMessageW(control.Handle, (WM)LB.GETTOPINDEX));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListBox_UseCustomTabOffsets_Set_GetReturnsExpected(bool value)
        {
            using var control = new ListBox
            {
                UseCustomTabOffsets = value
            };
            Assert.Equal(value, control.UseCustomTabOffsets);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.UseCustomTabOffsets = value;
            Assert.Equal(value, control.UseCustomTabOffsets);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.UseCustomTabOffsets = !value;
            Assert.Equal(!value, control.UseCustomTabOffsets);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void ListBox_UseCustomTabOffsets_SetWithHandle_GetReturnsExpected(bool value, int expectedCreatedCallCount)
        {
            using var control = new ListBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.UseCustomTabOffsets = value;
            Assert.Equal(value, control.UseCustomTabOffsets);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set same.
            control.UseCustomTabOffsets = value;
            Assert.Equal(value, control.UseCustomTabOffsets);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set different.
            control.UseCustomTabOffsets = !value;
            Assert.Equal(!value, control.UseCustomTabOffsets);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount + 1, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListBox_UseTabStops_Set_GetReturnsExpected(bool value)
        {
            using var control = new ListBox
            {
                UseTabStops = value
            };
            Assert.Equal(value, control.UseTabStops);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.UseTabStops = value;
            Assert.Equal(value, control.UseTabStops);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.UseTabStops = !value;
            Assert.Equal(!value, control.UseTabStops);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 0)]
        [InlineData(false, 1)]
        public void ListBox_UseTabStops_SetWithHandle_GetReturnsExpected(bool value, int expectedCreatedCallCount)
        {
            using var control = new ListBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.UseTabStops = value;
            Assert.Equal(value, control.UseTabStops);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set same.
            control.UseTabStops = value;
            Assert.Equal(value, control.UseTabStops);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set different.
            control.UseTabStops = !value;
            Assert.Equal(!value, control.UseTabStops);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount + 1, createdCallCount);
        }

        [WinFormsFact]
        public void ListBox_AddItemsCore_Invoke_Success()
        {
            using var control = new SubListBox();

            // Add multiple.
            control.AddItemsCore(new object[] { "item1", "item2" });
            Assert.Equal(new string[] { "item1", "item2" }, control.Items.Cast<object>());
            Assert.False(control.IsHandleCreated);

            // Add another.
            control.AddItemsCore(new object[] { "item3" });
            Assert.Equal(new string[] { "item1", "item2", "item3" }, control.Items.Cast<object>());
            Assert.False(control.IsHandleCreated);

            // Add empty.
            control.AddItemsCore(Array.Empty<object>());
            Assert.Equal(new string[] { "item1", "item2", "item3" }, control.Items.Cast<object>());
            Assert.False(control.IsHandleCreated);

            // Add null.
            control.AddItemsCore(null);
            Assert.Equal(new string[] { "item1", "item2", "item3" }, control.Items.Cast<object>());
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBox_AddItemsCore_InvokeWithHandle_Success()
        {
            using var control = new SubListBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            // Add multiple.
            control.AddItemsCore(new object[] { "item1", "item2" });
            Assert.Equal(new string[] { "item1", "item2" }, control.Items.Cast<object>());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Add another.
            control.AddItemsCore(new object[] { "item3" });
            Assert.Equal(new string[] { "item1", "item2", "item3" }, control.Items.Cast<object>());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Add empty.
            control.AddItemsCore(Array.Empty<object>());
            Assert.Equal(new string[] { "item1", "item2", "item3" }, control.Items.Cast<object>());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Add null.
            control.AddItemsCore(null);
            Assert.Equal(new string[] { "item1", "item2", "item3" }, control.Items.Cast<object>());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ListBox_BeginUpdate_InvokeWithoutHandle_Nop()
        {
            using var control = new ListBox();
            control.BeginUpdate();
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.BeginUpdate();
            Assert.False(control.IsHandleCreated);

            // End once.
            control.EndUpdate();
            Assert.False(control.IsHandleCreated);

            // End twice.
            control.EndUpdate();
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBox_BeginUpdate_InvokeWithHandle_Nop()
        {
            using var control = new ListBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.BeginUpdate();
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            control.BeginUpdate();
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // End once.
            control.EndUpdate();
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // End twice.
            control.EndUpdate();
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ListBox_EndUpdate_InvokeWithoutHandle_Success()
        {
            using var control = new ListBox();

            // End without beginning.
            control.EndUpdate();
            Assert.False(control.IsHandleCreated);

            // Begin.
            control.BeginUpdate();
            Assert.False(control.IsHandleCreated);

            // End.
            control.EndUpdate();
            Assert.False(control.IsHandleCreated);

            // End again.
            control.EndUpdate();
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBox_EndUpdate_InvokeWithHandle_Success()
        {
            using var control = new ListBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            // End without beginning.
            control.EndUpdate();
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Begin.
            control.BeginUpdate();
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // End.
            control.EndUpdate();
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // End again.
            control.EndUpdate();
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.List)]
        [InlineData(false, AccessibleRole.None)]
        public void ListBox_CreateAccessibilityInstance_Invoke_ReturnsExpected(bool createControl, AccessibleRole accessibleRole)
        {
            using var control = new SubListBox();

            if (createControl)
            {
                control.CreateControl();
            }

            Control.ControlAccessibleObject instance = Assert.IsAssignableFrom<Control.ControlAccessibleObject>(control.CreateAccessibilityInstance());
            Assert.NotNull(instance);
            Assert.Same(control, instance.Owner);
            Assert.Equal(accessibleRole, instance.Role);
            Assert.NotSame(control.CreateAccessibilityInstance(), instance);
            Assert.NotSame(control.AccessibilityObject, instance);
            Assert.Equal(createControl, control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBox_CreateControlsInstance_Invoke_ReturnsExpected()
        {
            using var control = new SubListBox();
            ListBox.ObjectCollection items = Assert.IsType<ListBox.ObjectCollection>(control.CreateItemCollection());
            Assert.Empty(items);
            Assert.False(items.IsReadOnly);
            Assert.NotSame(items, control.CreateItemCollection());
        }

        [WinFormsFact]
        public void ListBox_GetAutoSizeMode_Invoke_ReturnsExpected()
        {
            using var control = new SubListBox();
            Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
        }

        public static IEnumerable<object[]> GetPreferredSize_TestData()
        {
            foreach (BorderStyle borderStyle in Enum.GetValues(typeof(BorderStyle)))
            {
                yield return new object[] { borderStyle, Size.Empty };
                yield return new object[] { borderStyle, new Size(-1, -2) };
                yield return new object[] { borderStyle, new Size(10, 20) };
                yield return new object[] { borderStyle, new Size(30, 40) };
                yield return new object[] { borderStyle, new Size(int.MaxValue, int.MaxValue) };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(GetPreferredSize_TestData))]
        public void ListBox_GetPreferredSize_Invoke_ReturnsExpected(BorderStyle borderStyle, Size proposedSize)
        {
            using var control = new ListBox
            {
                BorderStyle = borderStyle
            };
            Assert.Equal(new Size(120, 96), control.GetPreferredSize(proposedSize));
            Assert.False(control.IsHandleCreated);

            // Call again.
            Assert.Equal(new Size(120, 96), control.GetPreferredSize(proposedSize));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(GetPreferredSize_TestData))]
        public void ListBox_GetPreferredSize_InvokeWithPadding_ReturnsExpected(BorderStyle borderStyle, Size proposedSize)
        {
            using var control = new ListBox
            {
                BorderStyle = borderStyle,
                Padding = new Padding(1, 2, 3, 4)
            };
            Assert.Equal(new Size(120, 96), control.GetPreferredSize(proposedSize));
            Assert.False(control.IsHandleCreated);

            // Call again.
            Assert.Equal(new Size(120, 96), control.GetPreferredSize(proposedSize));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(GetPreferredSize_TestData))]
        public void ListBox_GetPreferredSize_InvokeWithHandle_ReturnsExpected(BorderStyle borderStyle, Size proposedSize)
        {
            using var control = new ListBox
            {
                BorderStyle = borderStyle
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Size result = control.GetPreferredSize(proposedSize);
            Assert.True(result.Width > 0 && result.Width < 120);
            Assert.Equal(control.PreferredHeight, result.Height);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            Assert.Equal(result, control.GetPreferredSize(proposedSize));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(GetPreferredSize_TestData))]
        public void ListBox_GetPreferredSize_InvokeWithHandleWithPadding_ReturnsExpected(BorderStyle borderStyle, Size proposedSize)
        {
            using var control = new ListBox
            {
                BorderStyle = borderStyle,
                Padding = new Padding(1, 2, 3, 4)
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Size result = control.GetPreferredSize(proposedSize);
            Assert.True(result.Width > 0 && result.Width < 120);
            Assert.Equal(control.PreferredHeight + 6, result.Height);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            Assert.Equal(result, control.GetPreferredSize(proposedSize));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
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
        public void ListBox_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
        {
            using var control = new SubListBox();
            Assert.Equal(expected, control.GetStyle(flag));

            // Call again to test caching.
            Assert.Equal(expected, control.GetStyle(flag));
        }

        [WinFormsFact]
        public void ListBox_GetTopLevel_Invoke_ReturnsExpected()
        {
            using var control = new SubListBox();
            Assert.False(control.GetTopLevel());
        }

        public static IEnumerable<object[]> FindString_TestData()
        {
            foreach (int startIndex in new int[] { -2, -1, 0, 1 })
            {
                yield return new object[] { new ListBox(), null, startIndex, -1 };
                yield return new object[] { new ListBox(), string.Empty, startIndex, -1 };
                yield return new object[] { new ListBox(), "s", startIndex, -1 };

                using var controlWithNoItems = new ListBox();
                Assert.Empty(controlWithNoItems.Items);
                yield return new object[] { new ListBox(), null, startIndex, -1 };
                yield return new object[] { new ListBox(), string.Empty, startIndex, -1 };
                yield return new object[] { new ListBox(), "s", startIndex, -1 };
            }

            using var controlWithItems = new ListBox
            {
                DisplayMember = "Value"
            };
            controlWithItems.Items.Add(new DataClass { Value = "abc" });
            controlWithItems.Items.Add(new DataClass { Value = "abc" });
            controlWithItems.Items.Add(new DataClass { Value = "ABC" });
            controlWithItems.Items.Add(new DataClass { Value = "def" });
            controlWithItems.Items.Add(new DataClass { Value = "" });
            controlWithItems.Items.Add(new DataClass { Value = null });

            yield return new object[] { controlWithItems, "abc", -1, 0 };
            yield return new object[] { controlWithItems, "abc", 0, 1 };
            yield return new object[] { controlWithItems, "abc", 1, 2 };
            yield return new object[] { controlWithItems, "abc", 2, 0 };
            yield return new object[] { controlWithItems, "abc", 5, 0 };

            yield return new object[] { controlWithItems, "ABC", -1, 0 };
            yield return new object[] { controlWithItems, "ABC", 0, 1 };
            yield return new object[] { controlWithItems, "ABC", 1, 2 };
            yield return new object[] { controlWithItems, "ABC", 2, 0 };
            yield return new object[] { controlWithItems, "ABC", 5, 0 };

            yield return new object[] { controlWithItems, "a", -1, 0 };
            yield return new object[] { controlWithItems, "a", 0, 1 };
            yield return new object[] { controlWithItems, "a", 1, 2 };
            yield return new object[] { controlWithItems, "a", 2, 0 };
            yield return new object[] { controlWithItems, "a", 5, 0 };

            yield return new object[] { controlWithItems, "A", -1, 0 };
            yield return new object[] { controlWithItems, "A", 0, 1 };
            yield return new object[] { controlWithItems, "A", 1, 2 };
            yield return new object[] { controlWithItems, "A", 2, 0 };
            yield return new object[] { controlWithItems, "A", 5, 0 };

            yield return new object[] { controlWithItems, "abcd", -1, -1 };
            yield return new object[] { controlWithItems, "abcd", 0, -1 };
            yield return new object[] { controlWithItems, "abcd", 1, -1 };
            yield return new object[] { controlWithItems, "abcd", 2, -1 };
            yield return new object[] { controlWithItems, "abcd", 5, -1 };

            yield return new object[] { controlWithItems, "def", -1, 3 };
            yield return new object[] { controlWithItems, "def", 0, 3 };
            yield return new object[] { controlWithItems, "def", 1, 3 };
            yield return new object[] { controlWithItems, "def", 2, 3 };
            yield return new object[] { controlWithItems, "def", 5, 3 };

            yield return new object[] { controlWithItems, null, -1, -1 };
            yield return new object[] { controlWithItems, null, 0, -1 };
            yield return new object[] { controlWithItems, null, 1, -1 };
            yield return new object[] { controlWithItems, null, 2, -1 };
            yield return new object[] { controlWithItems, null, 5, -1 };

            yield return new object[] { controlWithItems, string.Empty, -1, 0 };
            yield return new object[] { controlWithItems, string.Empty, 0, 1 };
            yield return new object[] { controlWithItems, string.Empty, 1, 2 };
            yield return new object[] { controlWithItems, string.Empty, 2, 3 };
            yield return new object[] { controlWithItems, string.Empty, 5, 0 };

            yield return new object[] { controlWithItems, "NoSuchItem", -1, -1 };
            yield return new object[] { controlWithItems, "NoSuchItem", 0, -1 };
            yield return new object[] { controlWithItems, "NoSuchItem", 1, -1 };
            yield return new object[] { controlWithItems, "NoSuchItem", 2, -1 };
            yield return new object[] { controlWithItems, "NoSuchItem", 5, -1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(FindString_TestData))]
        public void ListBox_FindString_Invoke_ReturnsExpected(ListBox control, string s, int startIndex, int expected)
        {
            if (startIndex == -1)
            {
                Assert.Equal(expected, control.FindString(s));
            }

            Assert.Equal(expected, control.FindString(s, startIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(1)]
        [InlineData(2)]
        public void ListBox_FindString_InvalidStartIndex_ThrowsArgumentOutOfRangeException(int startIndex)
        {
            using var control = new ListBox();
            control.Items.Add("item");
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => control.FindString("s", startIndex));
        }

        public static IEnumerable<object[]> FindStringExact_TestData()
        {
            foreach (int startIndex in new int[] { -2, -1, 0, 1 })
            {
                yield return new object[] { new ListBox(), null, startIndex, -1 };
                yield return new object[] { new ListBox(), string.Empty, startIndex, -1 };
                yield return new object[] { new ListBox(), "s", startIndex, -1 };

                using var controlWithNoItems = new ListBox();
                Assert.Empty(controlWithNoItems.Items);
                yield return new object[] { new ListBox(), null, startIndex, -1 };
                yield return new object[] { new ListBox(), string.Empty, startIndex, -1 };
                yield return new object[] { new ListBox(), "s", startIndex, -1 };
            }

            using var controlWithItems = new ListBox
            {
                DisplayMember = "Value"
            };
            controlWithItems.Items.Add(new DataClass { Value = "abc" });
            controlWithItems.Items.Add(new DataClass { Value = "abc" });
            controlWithItems.Items.Add(new DataClass { Value = "ABC" });
            controlWithItems.Items.Add(new DataClass { Value = "def" });
            controlWithItems.Items.Add(new DataClass { Value = "" });
            controlWithItems.Items.Add(new DataClass { Value = null });

            yield return new object[] { controlWithItems, "abc", -1, 0 };
            yield return new object[] { controlWithItems, "abc", 0, 1 };
            yield return new object[] { controlWithItems, "abc", 1, 2 };
            yield return new object[] { controlWithItems, "abc", 2, 0 };
            yield return new object[] { controlWithItems, "abc", 5, 0 };

            yield return new object[] { controlWithItems, "ABC", -1, 0 };
            yield return new object[] { controlWithItems, "ABC", 0, 1 };
            yield return new object[] { controlWithItems, "ABC", 1, 2 };
            yield return new object[] { controlWithItems, "ABC", 2, 0 };
            yield return new object[] { controlWithItems, "ABC", 5, 0 };

            yield return new object[] { controlWithItems, "a", -1, -1 };
            yield return new object[] { controlWithItems, "a", 0, -1 };
            yield return new object[] { controlWithItems, "a", 1, -1 };
            yield return new object[] { controlWithItems, "a", 2, -1 };
            yield return new object[] { controlWithItems, "a", 5, -1 };

            yield return new object[] { controlWithItems, "A", -1, -1 };
            yield return new object[] { controlWithItems, "A", 0, -1 };
            yield return new object[] { controlWithItems, "A", 1, -1 };
            yield return new object[] { controlWithItems, "A", 2, -1 };
            yield return new object[] { controlWithItems, "A", 5, -1 };

            yield return new object[] { controlWithItems, "abcd", -1, -1 };
            yield return new object[] { controlWithItems, "abcd", 0, -1 };
            yield return new object[] { controlWithItems, "abcd", 1, -1 };
            yield return new object[] { controlWithItems, "abcd", 2, -1 };
            yield return new object[] { controlWithItems, "abcd", 5, -1 };

            yield return new object[] { controlWithItems, "def", -1, 3 };
            yield return new object[] { controlWithItems, "def", 0, 3 };
            yield return new object[] { controlWithItems, "def", 1, 3 };
            yield return new object[] { controlWithItems, "def", 2, 3 };
            yield return new object[] { controlWithItems, "def", 5, 3 };

            yield return new object[] { controlWithItems, null, -1, -1 };
            yield return new object[] { controlWithItems, null, 0, -1 };
            yield return new object[] { controlWithItems, null, 1, -1 };
            yield return new object[] { controlWithItems, null, 2, -1 };
            yield return new object[] { controlWithItems, null, 5, -1 };

            yield return new object[] { controlWithItems, string.Empty, -1, 4 };
            yield return new object[] { controlWithItems, string.Empty, 0, 4 };
            yield return new object[] { controlWithItems, string.Empty, 1, 4 };
            yield return new object[] { controlWithItems, string.Empty, 2, 4 };
            yield return new object[] { controlWithItems, string.Empty, 5, 4 };

            yield return new object[] { controlWithItems, "NoSuchItem", -1, -1 };
            yield return new object[] { controlWithItems, "NoSuchItem", 0, -1 };
            yield return new object[] { controlWithItems, "NoSuchItem", 1, -1 };
            yield return new object[] { controlWithItems, "NoSuchItem", 2, -1 };
            yield return new object[] { controlWithItems, "NoSuchItem", 5, -1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(FindStringExact_TestData))]
        public void ListBox_FindStringExact_Invoke_ReturnsExpected(ListBox control, string s, int startIndex, int expected)
        {
            if (startIndex == -1)
            {
                Assert.Equal(expected, control.FindStringExact(s));
            }

            Assert.Equal(expected, control.FindStringExact(s, startIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(1)]
        [InlineData(2)]
        public void ListBox_FindStringExact_InvalidStartIndex_ThrowsArgumentOutOfRangeException(int startIndex)
        {
            using var control = new ListBox();
            control.Items.Add("item");
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => control.FindStringExact("s", startIndex));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DrawMode))]
        public void ListBox_GetItemHeight_InvokeEmptyWithoutHandle_ReturnsExpected(DrawMode drawMode)
        {
            using var control = new ListBox
            {
                DrawMode = drawMode
            };
            Assert.Equal(13, control.GetItemHeight(0));
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> GetItemHeight_NotEmpty_TestData()
        {
            foreach (DrawMode drawMode in Enum.GetValues(typeof(DrawMode)))
            {
                yield return new object[] { drawMode, 0 };
                yield return new object[] { drawMode, 1 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(GetItemHeight_NotEmpty_TestData))]
        public void ListBox_GetItemHeight_InvokeNotEmptyWithoutHandle_ReturnsExpected(DrawMode drawMode, int index)
        {
            using var control = new ListBox
            {
                DrawMode = drawMode
            };
            control.Items.Add("Item1");
            control.Items.Add("Item2");
            Assert.Equal(13, control.GetItemHeight(index));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(DrawMode.Normal)]
        [InlineData(DrawMode.OwnerDrawFixed)]
        [InlineData(DrawMode.OwnerDrawVariable)]
        public void ListBox_GetItemHeight_InvokeEmptyWithHandle_ReturnsExpected(DrawMode drawMode)
        {
            using var control = new ListBox
            {
                DrawMode = drawMode
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.True(control.GetItemHeight(0) > 0);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(GetItemHeight_NotEmpty_TestData))]
        public void ListBox_GetItemHeight_InvokeNotEmptyWithHandle_ReturnsExpected(DrawMode drawMode, int index)
        {
            using var control = new ListBox
            {
                DrawMode = drawMode
            };
            control.Items.Add("Item1");
            control.Items.Add("Item2");
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.True(control.GetItemHeight(index) > 0);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> GetItemHeight_CustomGetItemHeight_TestData()
        {
            yield return new object[] { DrawMode.Normal, 0, 0, 0, 0 };
            yield return new object[] { DrawMode.Normal, 1, 0, -2, -2 };
            yield return new object[] { DrawMode.Normal, 0, 0, 10, 10 };
            yield return new object[] { DrawMode.OwnerDrawFixed, 0, 0, 0, 0 };
            yield return new object[] { DrawMode.OwnerDrawFixed, 1, 0, -2, -2 };
            yield return new object[] { DrawMode.OwnerDrawFixed, 0, 0, 10, 10 };
            yield return new object[] { DrawMode.OwnerDrawVariable, 0, 0, 0, 0 };
            yield return new object[] { DrawMode.OwnerDrawVariable, 1, 1, -2, -2 };
            yield return new object[] { DrawMode.OwnerDrawVariable, 0, 0, 10, 10 };
        }

        [WinFormsTheory]
        [MemberData(nameof(GetItemHeight_CustomGetItemHeight_TestData))]
        public void ListBox_GetItemHeight_InvokeCustomGetItemHeight_ReturnsExpected(DrawMode drawMode, int index, int expectedIndex, int getItemRectResult, int expected)
        {
            using var control = new CustomGetItemHeightListBox
            {
                DrawMode = drawMode,
                ExpectedIndex = expectedIndex,
                GetItemHeightResult = (IntPtr)getItemRectResult
            };
            control.Items.Add("Item1");
            control.Items.Add("Item2");
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.MakeCustom = true;
            Assert.Equal(expected, control.GetItemHeight(index));
        }

        private class CustomGetItemHeightListBox : ListBox
        {
            public bool MakeCustom { get; set; }
            public int ExpectedIndex { get; set; }
            public IntPtr GetItemHeightResult { get; set; }

            protected unsafe override void WndProc(ref Message m)
            {
                if (MakeCustom && m.Msg == (int)LB.GETITEMHEIGHT)
                {
                    Assert.Equal(ExpectedIndex, (int)m.WParam);
                    m.Result = GetItemHeightResult;
                    return;
                }

                base.WndProc(ref m);
            }
        }

        [WinFormsFact]
        public void ListBox_GetItemHeight_InvokeInvalidGetItemHeight_ThrowsWin32Exception()
        {
            using var control = new InvalidGetItemHeightListBox();
            control.Items.Add("Item");
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.MakeInvalid = true;
            Assert.Throws<Win32Exception>(() => control.GetItemHeight(0));
        }

        private class InvalidGetItemHeightListBox : ListBox
        {
            public bool MakeInvalid { get; set; }

            protected unsafe override void WndProc(ref Message m)
            {
                if (MakeInvalid && m.Msg == (int)LB.GETITEMHEIGHT)
                {
                    m.Result = (IntPtr)(-1);
                    return;
                }

                base.WndProc(ref m);
            }
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(1)]
        public void ListBox_GetItemHeight_InvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
        {
            using var control = new ListBox();
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemHeight(index));
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(1)]
        [InlineData(2)]
        public void ListBox_GetItemHeight_InvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
        {
            using var control = new ListBox();
            control.Items.Add("Item");
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemHeight(index));
        }

        [WinFormsFact]
        public void ListBox_GetItemRectangle_InvokeWithoutHandle_ReturnsExpectedAndCreatedHandle()
        {
            using var control = new ListBox();
            control.Items.Add("Item1");
            control.Items.Add("Item1");

            Rectangle rect1 = control.GetItemRectangle(0);
            Assert.True(rect1.X >= 0);
            Assert.True(rect1.Y >= 0);
            Assert.True(rect1.Width > 0);
            Assert.True(rect1.Height > 0);
            Assert.Equal(rect1, control.GetItemRectangle(0));
            Assert.True(control.IsHandleCreated);

            Rectangle rect2 = control.GetItemRectangle(1);
            Assert.Equal(rect2.X, rect1.X);
            Assert.True(rect2.Y >= rect1.Y + rect1.Height);
            Assert.True(rect2.Width > 0);
            Assert.True(rect2.Height > 0);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBox_GetItemRectangle_InvokeWithHandle_ReturnsExpected()
        {
            using var control = new ListBox();
            control.Items.Add("Item1");
            control.Items.Add("Item1");
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Rectangle rect1 = control.GetItemRectangle(0);
            Assert.True(rect1.X >= 0);
            Assert.True(rect1.Y >= 0);
            Assert.True(rect1.Width > 0);
            Assert.True(rect1.Height > 0);
            Assert.Equal(rect1, control.GetItemRectangle(0));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            Rectangle rect2 = control.GetItemRectangle(1);
            Assert.Equal(rect2.X, rect1.X);
            Assert.True(rect2.Y >= rect1.Y + rect1.Height);
            Assert.True(rect2.Width > 0);
            Assert.True(rect2.Height > 0);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> GetItemRectangle_CustomGetItemRect_TestData()
        {
            yield return new object[] { new RECT(), Rectangle.Empty };
            yield return new object[] { new RECT(1, 2, 3, 4), new Rectangle(1, 2, 2, 2) };
        }

        [WinFormsTheory]
        [MemberData(nameof(GetItemRectangle_CustomGetItemRect_TestData))]
        public void ListBox_GetItemRectangle_InvokeCustomGetItemRect_ReturnsExpected(object getItemRectResult, Rectangle expected)
        {
            using var control = new CustomGetItemRectListBox
            {
                GetItemRectResult = (RECT)getItemRectResult
            };
            control.Items.Add("Item");
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            Assert.Equal(expected, control.GetItemRectangle(0));
        }

        private class CustomGetItemRectListBox : ListBox
        {
            public RECT GetItemRectResult { get; set; }

            protected unsafe override void WndProc(ref Message m)
            {
                if (m.Msg == (int)LB.GETITEMRECT)
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
        public void ListBox_GetItemRectangle_InvokeInvalidGetItemRect_ReturnsExpected()
        {
            using var control = new InvalidGetItemRectListBox();
            control.Items.Add("Item");
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.MakeInvalid = true;
            Assert.Equal(Rectangle.Empty, control.GetItemRectangle(0));
        }

        private class InvalidGetItemRectListBox : ListBox
        {
            public bool MakeInvalid { get; set; }

            protected unsafe override void WndProc(ref Message m)
            {
                if (MakeInvalid && m.Msg == (int)LB.GETITEMRECT)
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
        public void ListBox_GetItemRectangle_InvalidIndexEmpty_ThrowsArgumentOutOfRangeException()
        {
            using var control = new ListBox();
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRectangle(-1));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRectangle(0));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRectangle(1));
        }

        [WinFormsFact]
        public void ListBox_GetItemRectangle_InvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException()
        {
            using var control = new ListBox();
            control.Items.Add("Item");

            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRectangle(-1));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRectangle(1));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRectangle(2));
        }

        [WinFormsFact]
        public void ListBox_GetItemRectangle_InvalidIndexWithHandleEmpty_ThrowsArgumentOutOfRangeException()
        {
            using var control = new ListBox();
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRectangle(-1));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRectangle(0));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRectangle(1));
        }

        [WinFormsFact]
        public void ListBox_GetItemRectangle_InvalidIndexWithHandleNotEmpty_ThrowsArgumentOutOfRangeException()
        {
            using var control = new ListBox();
            control.Items.Add("Item");
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRectangle(-1));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRectangle(1));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRectangle(2));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ListBox_OnClick_Invoke_CallsClick(EventArgs eventArgs)
        {
            using var control = new SubListBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Click += handler;
            control.OnClick(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.Click -= handler;
            control.OnClick(eventArgs);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> OnDrawItem_TestData()
        {
            using var bitmap = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(bitmap);
            yield return new object[] { null };
            yield return new object[] { new DrawItemEventArgs(graphics, null, new Rectangle(1, 2, 3, 4), 0, DrawItemState.Checked) };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnDrawItem_TestData))]
        public void ListBox_OnDrawItem_Invoke_CallsDrawItem(DrawItemEventArgs eventArgs)
        {
            using var control = new SubListBox();
            int callCount = 0;
            void handler(object sender, DrawItemEventArgs e)
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            }

            // Call with handler.
            control.DrawItem += handler;
            control.OnDrawItem(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.DrawItem -= handler;
            control.OnDrawItem(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ListBox_OnFontChanged_Invoke_CallsFontChanged(EventArgs eventArgs)
        {
            using var control = new SubListBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.FontChanged += handler;
            control.OnFontChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(96, control.Height);

            // Remove handler.
            control.FontChanged -= handler;
            control.OnFontChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(96, control.Height);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ListBox_OnGotFocus_Invoke_CallsGotFocus(EventArgs eventArgs)
        {
            using var control = new SubListBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.GotFocus += handler;
            control.OnGotFocus(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.GotFocus -= handler;
            control.OnGotFocus(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ListBox_OnGotFocus_InvokeWithHandle_CallsGotFocus(EventArgs eventArgs)
        {
            using var control = new SubListBox();
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
            control.GotFocus += handler;
            control.OnGotFocus(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.GotFocus -= handler;
            control.OnGotFocus(eventArgs);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> OnMeasureItem_TestData()
        {
            using var bitmap = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(bitmap);
            yield return new object[] { null };
            yield return new object[] { new MeasureItemEventArgs(graphics, 0, 0) };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnMeasureItem_TestData))]
        public void ListBox_OnMeasureItem_Invoke_CallsMeasureItem(MeasureItemEventArgs eventArgs)
        {
            using var control = new SubListBox();
            int callCount = 0;
            void handler(object sender, MeasureItemEventArgs e)
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            }

            // Call with handler.
            control.MeasureItem += handler;
            control.OnMeasureItem(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.MeasureItem -= handler;
            control.OnMeasureItem(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetMouseEventArgsTheoryData))]
        public void ListBox_OnMouseClick_Invoke_CallsMouseClick(MouseEventArgs eventArgs)
        {
            using var control = new SubListBox();
            int callCount = 0;
            MouseEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.MouseClick += handler;
            control.OnMouseClick(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.MouseClick -= handler;
            control.OnMouseClick(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetPaintEventArgsTheoryData))]
        public void ListBox_OnPaint_Invoke_CallsPaint(PaintEventArgs eventArgs)
        {
            using var control = new SubListBox();
            int callCount = 0;
            PaintEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Paint += handler;
            control.OnPaint(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.Paint -= handler;
            control.OnPaint(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ListBox_OnSelectedIndexChanged_Invoke_CallsSelectedIndexChanged(EventArgs eventArgs)
        {
            using var control = new SubListBox();
            int selectedValueChangedCallCount = 0;
            control.SelectedValueChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                selectedValueChangedCallCount++;
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                Assert.True(callCount < selectedValueChangedCallCount);
                callCount++;
            };

            // Call with handler.
            control.SelectedIndexChanged += handler;
            control.OnSelectedIndexChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, selectedValueChangedCallCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.SelectedIndexChanged -= handler;
            control.OnSelectedIndexChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(2, selectedValueChangedCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ListBox_OnSelectedIndexChanged_InvokeWithHandle_CallsSelectedIndexChanged(EventArgs eventArgs)
        {
            using var control = new SubListBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int selectedValueChangedCallCount = 0;
            control.SelectedValueChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                selectedValueChangedCallCount++;
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                Assert.True(callCount < selectedValueChangedCallCount);
                callCount++;
            };

            // Call with handler.
            control.SelectedIndexChanged += handler;
            control.OnSelectedIndexChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, selectedValueChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.SelectedIndexChanged -= handler;
            control.OnSelectedIndexChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(2, selectedValueChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> OnSelectedIndexChanged_WithDataManager_TestData()
        {
            foreach (bool formattingEnabled in new bool[] { true, false })
            {
                foreach (int position in new int[] { 0, 1 })
                {
                    yield return new object[] { formattingEnabled, position, null };
                    yield return new object[] { formattingEnabled, position, new EventArgs() };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnSelectedIndexChanged_WithDataManager_TestData))]
        public void ListBox_OnSelectedIndexChanged_InvokeWithDataManager_CallsSelectedIndexChanged(bool formattingEnabled, int position, EventArgs eventArgs)
        {
            var bindingContext = new BindingContext();
            var dataSource = new List<string> { "item1", "item2", "item3" };
            using var control = new SubListBox
            {
                BindingContext = bindingContext,
                DataSource = dataSource,
                FormattingEnabled = formattingEnabled
            };
            control.DataManager.Position = position;
            int selectedValueChangedCallCount = 0;
            control.SelectedValueChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                selectedValueChangedCallCount++;
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                Assert.True(callCount < selectedValueChangedCallCount);
                callCount++;
            };

            // Call with handler.
            control.SelectedIndexChanged += handler;
            control.OnSelectedIndexChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, selectedValueChangedCallCount);
            Assert.Equal(position, control.DataManager.Position);

            // Remove handler.
            control.SelectedIndexChanged -= handler;
            control.OnSelectedIndexChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(2, selectedValueChangedCallCount);
            Assert.Equal(position, control.DataManager.Position);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ListBox_OnSelectedValueChanged_Invoke_CallsSelectedValueChanged(EventArgs eventArgs)
        {
            using var control = new SubListBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.SelectedValueChanged += handler;
            control.OnSelectedValueChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.SelectedValueChanged -= handler;
            control.OnSelectedValueChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void ListBox_RefreshItems_InvokeEmpty_Success()
        {
            using var control = new SubListBox();
            control.RefreshItems();
            Assert.Empty(control.Items);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.RefreshItems();
            Assert.Empty(control.Items);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBox_RefreshItems_InvokeNotEmpty_Success()
        {
            using var control = new SubListBox();
            control.Items.Add("item1");
            control.Items.Add("item2");

            control.RefreshItems();
            Assert.Equal(new object[] { "item1", "item2" }, control.Items.Cast<object>());
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.RefreshItems();
            Assert.Equal(new object[] { "item1", "item2" }, control.Items.Cast<object>());
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBox_RefreshItems_InvokeEmptyWithHandle_Success()
        {
            using var control = new SubListBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.RefreshItems();
            Assert.Empty(control.Items);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            control.RefreshItems();
            Assert.Empty(control.Items);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ListBox_RefreshItems_InvokeNotEmptyWithHandle_Success()
        {
            using var control = new SubListBox();
            control.Items.Add("item1");
            control.Items.Add("item2");
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.RefreshItems();
            Assert.Equal(new object[] { "item1", "item2" }, control.Items.Cast<object>());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            control.RefreshItems();
            Assert.Equal(new object[] { "item1", "item2" }, control.Items.Cast<object>());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ListBox_ResetBackColor_Invoke_Success()
        {
            using var control = new ListBox();

            // Reset without value.
            control.ResetBackColor();
            Assert.Equal(SystemColors.Window, control.BackColor);

            // Reset with value.
            control.BackColor = Color.Black;
            control.ResetBackColor();
            Assert.Equal(SystemColors.Window, control.BackColor);

            // Reset again.
            control.ResetBackColor();
            Assert.Equal(SystemColors.Window, control.BackColor);
        }

        [WinFormsFact]
        public void ListBox_ResetForeColor_Invoke_Success()
        {
            using var control = new ListBox();

            // Reset without value.
            control.ResetForeColor();
            Assert.Equal(SystemColors.WindowText, control.ForeColor);

            // Reset with value.
            control.ForeColor = Color.Black;
            control.ResetForeColor();
            Assert.Equal(SystemColors.WindowText, control.ForeColor);

            // Reset again.
            control.ResetForeColor();
            Assert.Equal(SystemColors.WindowText, control.ForeColor);
        }

        public static IEnumerable<object[]> SetItemsCore_TestData()
        {
            yield return new object[] { new object[] { "item1", "item2", "item3" } };
            yield return new object[] { Array.Empty<object>() };
        }

        [WinFormsTheory]
        [MemberData(nameof(SetItemsCore_TestData))]
        public void ListBox_SetItemsCore_Invoke_Success(object[] value)
        {
            using var control = new SubListBox();
            control.SetItemsCore(value);
            Assert.Equal(value, control.Items.Cast<object>());
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.SetItemsCore(value);
            Assert.Equal(value, control.Items.Cast<object>());
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(SetItemsCore_TestData))]
        public void ListBox_SetItemsCore_InvokeWithHandle_Success(object[] value)
        {
            using var control = new SubListBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.SetItemsCore(value);
            Assert.Equal(value, control.Items.Cast<object>());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            control.SetItemsCore(value);
            Assert.Equal(value, control.Items.Cast<object>());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ListBox_SetItemsCore_NullValueEmpty_ThrowsArgumentNullException()
        {
            using var control = new SubListBox();
            Assert.Throws<ArgumentNullException>("value", () => control.SetItemsCore(null));
            Assert.Empty(control.Items);
        }

        [WinFormsFact]
        public void ListBox_SetItemsCore_NullValueNotEmpty_ThrowsArgumentNullException()
        {
            using var control = new SubListBox();
            control.Items.Add("item1");
            control.Items.Add("item2");
            Assert.Throws<ArgumentNullException>("value", () => control.SetItemsCore(null));
            Assert.Equal(new object[] { "item1", "item2" }, control.Items.Cast<object>());
        }

        [WinFormsFact]
        public void ListBox_ToString_InvokeWithoutItems_ReturnsExpected()
        {
            using var control = new ListBox();
            Assert.Equal("System.Windows.Forms.ListBox", control.ToString());
        }

        [WinFormsFact]
        public void ListBox_ToString_InvokeWithEmptyItems_ReturnsExpected()
        {
            using var control = new ListBox();
            Assert.Empty(control.Items);
            Assert.Equal("System.Windows.Forms.ListBox, Items.Count: 0", control.ToString());
        }

        public static IEnumerable<object[]> ToString_WithItems_TestData()
        {
            yield return new object[] { string.Empty, "System.Windows.Forms.ListBox, Items.Count: 2, Items[0]: " };
            yield return new object[] { "abc", "System.Windows.Forms.ListBox, Items.Count: 2, Items[0]: abc" };
            yield return new object[] { new string('a', 41), "System.Windows.Forms.ListBox, Items.Count: 2, Items[0]: aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" };
        }

        [WinFormsTheory]
        [MemberData(nameof(ToString_WithItems_TestData))]
        public void ListBox_ToString_InvokeWithItems_ReturnsExpected(string item1, string expected)
        {
            using var control = new ListBox();
            control.Items.Add(item1);
            control.Items.Add("item2");
            Assert.Equal(expected, control.ToString());
        }

        private class SubListBox : ListBox
        {
            public new bool AllowSelection => base.AllowSelection;

            public new bool CanEnableIme => base.CanEnableIme;

            public new bool CanRaiseEvents => base.CanRaiseEvents;

            public new CreateParams CreateParams => base.CreateParams;

            public new CurrencyManager DataManager => base.DataManager;

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

#pragma warning disable 0618
            public new void AddItemsCore(object[] value) => base.AddItemsCore(value);
#pragma warning restore 0618

            public new AccessibleObject CreateAccessibilityInstance() => base.CreateAccessibilityInstance();

            public new ObjectCollection CreateItemCollection() => base.CreateItemCollection();

            public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

            public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

            public new bool GetTopLevel() => base.GetTopLevel();

            public new void OnChangeUICues(UICuesEventArgs e) => base.OnChangeUICues(e);

            public new void OnClick(EventArgs e) => base.OnClick(e);

            public new void OnDataSourceChanged(EventArgs e) => base.OnDataSourceChanged(e);

            public new void OnDisplayMemberChanged(EventArgs e) => base.OnDisplayMemberChanged(e);

            public new void OnDrawItem(DrawItemEventArgs e) => base.OnDrawItem(e);

            public new void OnFontChanged(EventArgs e) => base.OnFontChanged(e);

            public new void OnGotFocus(EventArgs e) => base.OnGotFocus(e);

            public new void OnHandleCreated(EventArgs e) => base.OnHandleCreated(e);

            public new void OnHandleDestroyed(EventArgs e) => base.OnHandleDestroyed(e);

            public new void OnMeasureItem(MeasureItemEventArgs e) => base.OnMeasureItem(e);

            public new void OnMouseClick(MouseEventArgs e) => base.OnMouseClick(e);

            public new void OnPaint(PaintEventArgs e) => base.OnPaint(e);

            public new void OnParentChanged(EventArgs e) => base.OnParentChanged(e);

            public new void OnResize(EventArgs e) => base.OnResize(e);

            public new void OnSelectedIndexChanged(EventArgs e) => base.OnSelectedIndexChanged(e);

            public new void OnSelectedValueChanged(EventArgs e) => base.OnSelectedValueChanged(e);

            public new void RefreshItem(int index) => base.RefreshItem(index);

            public new void RefreshItems() => base.RefreshItems();

            public new void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew) => base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);

            public new void ScaleControl(SizeF factor, BoundsSpecified specified) => base.ScaleControl(factor, specified);

            public new void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) => base.SetBoundsCore(x, y, width, height, specified);

            public new void SetItemCore(int index, object value) => base.SetItemCore(index, value);

            public new void SetItemsCore(IList value) => base.SetItemsCore(value);

            public new void Sort() => base.Sort();

            public new void WmReflectCommand(ref Message m) => base.WmReflectCommand(ref m);

            public new void WndProc(ref Message m) => base.WndProc(ref m);
        }

        private class DataClass
        {
            public string Value { get; set; }
        }
    }
}
