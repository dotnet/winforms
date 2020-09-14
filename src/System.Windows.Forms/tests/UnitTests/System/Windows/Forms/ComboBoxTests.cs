// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Moq;
using WinForms.Common.Tests;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    using Point = System.Drawing.Point;
    using Size = System.Drawing.Size;

    public class ComboBoxTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ComboBox_Ctor_Default()
        {
            using var control = new SubComboBox();
            Assert.Null(control.AccessibleDefaultActionDescription);
            Assert.Null(control.AccessibleDescription);
            Assert.Null(control.AccessibleName);
            Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
            Assert.False(control.AllowDrop);
            Assert.True(control.AllowSelection);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.Empty(control.AutoCompleteCustomSource);
            Assert.Same(control.AutoCompleteCustomSource, control.AutoCompleteCustomSource);
            Assert.Equal(AutoCompleteMode.None, control.AutoCompleteMode);
            Assert.Equal(AutoCompleteSource.None, control.AutoCompleteSource);
            Assert.False(control.AutoSize);
            Assert.Equal(SystemColors.Window, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.Null(control.BindingContext);
            Assert.Equal(control.PreferredHeight, control.Bottom);
            Assert.Equal(new Rectangle(0, 0, 121, control.PreferredHeight), control.Bounds);
            Assert.True(control.CanEnableIme);
            Assert.False(control.CanFocus);
            Assert.True(control.CanRaiseEvents);
            Assert.True(control.CanSelect);
            Assert.False(control.Capture);
            Assert.True(control.CausesValidation);
            Assert.Equal(new Size(117, control.PreferredHeight - 4), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 117, control.PreferredHeight - 4), control.ClientRectangle);
            Assert.Null(control.Container);
            Assert.False(control.ContainsFocus);
            Assert.Null(control.ContextMenuStrip);
            Assert.Empty(control.Controls);
            Assert.Same(control.Controls, control.Controls);
            Assert.False(control.Created);
            Assert.Same(Cursors.Default, control.Cursor);
            Assert.Null(control.DataManager);
            Assert.Null(control.DataSource);
            Assert.Same(Cursors.Default, control.DefaultCursor);
            Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
            Assert.Equal(new Padding(3), control.DefaultMargin);
            Assert.Equal(Size.Empty, control.DefaultMaximumSize);
            Assert.Equal(Size.Empty, control.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, control.DefaultPadding);
            Assert.Equal(new Size(121, control.PreferredHeight), control.DefaultSize);
            Assert.False(control.DesignMode);
            Assert.Empty(control.DisplayMember);
            Assert.Equal(new Rectangle(0, 0, 117, control.PreferredHeight - 4), control.DisplayRectangle);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.False(control.DoubleBuffered);
            Assert.Equal(DrawMode.Normal, control.DrawMode);
            Assert.Equal(106, control.DropDownHeight);
            Assert.Equal(ComboBoxStyle.DropDown, control.DropDownStyle);
            Assert.Equal(121, control.DropDownWidth);
            Assert.False(control.DroppedDown);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.Equal(FlatStyle.Standard, control.FlatStyle);
            Assert.False(control.Focused);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(SystemColors.WindowText, control.ForeColor);
            Assert.Null(control.FormatInfo);
            Assert.Empty(control.FormatString);
            Assert.False(control.FormattingEnabled);
            Assert.False(control.HasChildren);
            Assert.Equal(control.PreferredHeight, control.Height);
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.True(control.IntegralHeight);
            Assert.Equal(Control.DefaultFont.Height + 2, control.ItemHeight);
            Assert.False(control.IsAccessible);
            Assert.False(control.IsMirrored);
            Assert.Empty(control.Items);
            Assert.Same(control.Items, control.Items);
            Assert.NotNull(control.LayoutEngine);
            Assert.Same(control.LayoutEngine, control.LayoutEngine);
            Assert.Equal(0, control.Left);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Equal(new Padding(3), control.Margin);
            Assert.Equal(8, control.MaxDropDownItems);
            Assert.Equal(0, control.MaxLength);
            Assert.Equal(Size.Empty, control.MaximumSize);
            Assert.Equal(Size.Empty, control.MinimumSize);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Null(control.Parent);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.True(control.PreferredHeight > 0);
            Assert.Equal(new Size(121, control.PreferredHeight), control.PreferredSize);
            Assert.False(control.RecreatingHandle);
            Assert.Null(control.Region);
            Assert.False(control.ResizeRedraw);
            Assert.Equal(121, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.Null(control.SelectedValue);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.True(control.ShowFocusCues);
            Assert.True(control.ShowKeyboardCues);
            Assert.Null(control.Site);
            Assert.Equal(new Size(121, control.PreferredHeight), control.Size);
            Assert.False(control.Sorted);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(0, control.Top);
            Assert.Null(control.TopLevelControl);
            Assert.False(control.UseWaitCursor);
            Assert.Empty(control.ValueMember);
            Assert.True(control.Visible);
            Assert.Equal(121, control.Width);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ComboBox_CreateParams_GetDefault_ReturnsExpected()
        {
            using var control = new SubComboBox();
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("ComboBox", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x200, createParams.ExStyle);
            Assert.Equal(control.PreferredHeight, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x56210242, createParams.Style);
            Assert.Equal(121, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(AutoCompleteMode))]
        public void ComboBox_AutoCompleteMode_SetInvalidValue_ThrowsInvalidEnumArgumentException(AutoCompleteMode value)
        {
            using var control = new ComboBox();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.AutoCompleteMode = value);
        }

        public static IEnumerable<object[]> BackColor_Set_TestData()
        {
            yield return new object[] { Color.Empty, SystemColors.Window };
            yield return new object[] { Color.Red, Color.Red };
        }

        [WinFormsTheory]
        [MemberData(nameof(BackColor_Set_TestData))]
        public void ComboBox_BackColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new ComboBox
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
        public void ComboBox_BackColor_SetWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
        {
            using var control = new ComboBox();
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
        public void ComboBox_BackColor_SetWithHandler_CallsBackColorChanged()
        {
            using var control = new ComboBox();
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
        public void ComboBox_BackColor_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ComboBox))[nameof(ComboBox.BackColor)];
            using var control = new ComboBox();
            Assert.False(property.CanResetValue(control));

            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.Window, control.BackColor);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void ComboBox_BackColor_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ComboBox))[nameof(ComboBox.BackColor)];
            using var control = new ComboBox();
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
        public void ComboBox_BackgroundImage_Set_GetReturnsExpected(Image value)
        {
            using var control = new ComboBox
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
        public void ComboBox_BackgroundImage_SetWithHandler_CallsBackgroundImageChanged()
        {
            using var control = new ComboBox();
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
        public void ComboBox_BackgroundImageLayout_Set_GetReturnsExpected(ImageLayout value)
        {
            using var control = new ComboBox
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
        public void ComboBox_BackgroundImageLayout_SetWithHandler_CallsBackgroundImageLayoutChanged()
        {
            using var control = new ComboBox();
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
        public void ComboBox_BackgroundImageLayout_SetInvalid_ThrowsInvalidEnumArgumentException(ImageLayout value)
        {
            using var control = new ComboBox();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.BackgroundImageLayout = value);
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
        public void ComboBox_DataSource_Set_GetReturnsExpected(object value)
        {
            using var control = new SubComboBox
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
        public void ComboBox_DataSource_SetWithHandler_CallsDataSourceChanged()
        {
            using var control = new ComboBox();
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
            Assert.Equal(0, dataSourceCallCount);
            Assert.Equal(0, displayMemberCallCount);

            // Set same.
            control.DataSource = dataSource1;
            Assert.Same(dataSource1, control.DataSource);
            Assert.Equal(0, dataSourceCallCount);
            Assert.Equal(0, displayMemberCallCount);

            // Set different.
            var dataSource2 = new List<int>();
            control.DataSource = dataSource2;
            Assert.Same(dataSource2, control.DataSource);
            Assert.Equal(0, dataSourceCallCount);
            Assert.Equal(0, displayMemberCallCount);

            // Set null.
            control.DataSource = null;
            Assert.Null(control.DataSource);
            Assert.Equal(0, dataSourceCallCount);
            Assert.Equal(0, displayMemberCallCount);

            // Remove handler.
            control.DataSourceChanged -= dataSourceHandler;
            control.DisplayMemberChanged -= displayMemberHandler;
            control.DataSource = dataSource1;
            Assert.Same(dataSource1, control.DataSource);
            Assert.Equal(0, dataSourceCallCount);
            Assert.Equal(0, displayMemberCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ComboBoxStyle))]
        public void ComboBox_DropDownStyle_Set_GetReturnsExpected(ComboBoxStyle value)
        {
            using var control = new ComboBox
            {
                DropDownStyle = value
            };
            Assert.Equal(value, control.DropDownStyle);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.DropDownStyle = value;
            Assert.Equal(value, control.DropDownStyle);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> DropDownStyle_Set_TestData()
        {
            foreach (AutoCompleteSource source in Enum.GetValues(typeof(AutoCompleteSource)))
            {
                foreach (AutoCompleteMode mode in Enum.GetValues(typeof(AutoCompleteMode)))
                {
                    yield return new object[] { source, mode, ComboBoxStyle.Simple, mode };
                    yield return new object[] { source, mode, ComboBoxStyle.DropDown, mode };
                    yield return new object[] { source, mode, ComboBoxStyle.DropDownList, source != AutoCompleteSource.ListItems ? AutoCompleteMode.None : mode };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(DropDownStyle_Set_TestData))]
        public void ComboBox_DropDownStyle_SetWithSourceAndMode_GetReturnsExpected(AutoCompleteSource source, AutoCompleteMode mode, ComboBoxStyle value, AutoCompleteMode expectedMode)
        {
            using var control = new ComboBox
            {
                AutoCompleteSource = source,
                AutoCompleteMode = mode,
                DropDownStyle = value
            };
            Assert.Equal(value, control.DropDownStyle);
            Assert.Equal(source, control.AutoCompleteSource);
            Assert.Equal(expectedMode, control.AutoCompleteMode);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.DropDownStyle = value;
            Assert.Equal(value, control.DropDownStyle);
            Assert.Equal(source, control.AutoCompleteSource);
            Assert.Equal(expectedMode, control.AutoCompleteMode);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ComboBox_DropDownStyle_SetWithPreferredHeight_ResetsPreferredHeight()
        {
            using var control = new ComboBox
            {
                FormattingEnabled = true
            };
            int height1 = control.PreferredHeight;

            control.DropDownStyle = ComboBoxStyle.DropDownList;
            Assert.Equal(height1, control.PreferredHeight);

            control.DropDownStyle = ComboBoxStyle.Simple;
            int height2 = control.PreferredHeight;
            Assert.True(height2 > height1);

            control.DropDownStyle = ComboBoxStyle.DropDownList;
            Assert.Equal(height1, control.PreferredHeight);
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.Simple, 1)]
        [InlineData(ComboBoxStyle.DropDown, 0)]
        [InlineData(ComboBoxStyle.DropDownList, 1)]
        public void ComboBox_DropDownStyle_SetWithHandle_GetReturnsExpected(ComboBoxStyle value, int expectedCreatedCallCount)
        {
            using var control = new ComboBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.DropDownStyle = value;
            Assert.Equal(value, control.DropDownStyle);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set same.
            control.DropDownStyle = value;
            Assert.Equal(value, control.DropDownStyle);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);
        }

        [WinFormsFact]
        public void ComboBox_DropDownStyle_SetWithHandler_CallsDropDownStyleChanged()
        {
            using var control = new ComboBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.DropDownStyleChanged += handler;

            // Set different.
            control.DropDownStyle = ComboBoxStyle.DropDownList;
            Assert.Equal(ComboBoxStyle.DropDownList, control.DropDownStyle);
            Assert.Equal(1, callCount);

            // Set same.
            control.DropDownStyle = ComboBoxStyle.DropDownList;
            Assert.Equal(ComboBoxStyle.DropDownList, control.DropDownStyle);
            Assert.Equal(1, callCount);

            // Set different.
            control.DropDownStyle = ComboBoxStyle.Simple;
            Assert.Equal(ComboBoxStyle.Simple, control.DropDownStyle);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.DropDownStyleChanged -= handler;
            control.DropDownStyle = ComboBoxStyle.DropDownList;
            Assert.Equal(ComboBoxStyle.DropDownList, control.DropDownStyle);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ComboBoxStyle))]
        public void ComboBox_DropDownStyle_SetInvalidValue_ThrowsInvalidEnumArgumentException(ComboBoxStyle value)
        {
            using var control = new ComboBox();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.DropDownStyle = value);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetFontTheoryData))]
        public void ComboBox_Font_Set_GetReturnsExpected(Font value)
        {
            using var control = new SubComboBox
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

        [WinFormsFact]
        public void ComboBox_Font_SetWithHandler_CallsFontChanged()
        {
            using var control = new ComboBox();
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
        public void ComboBox_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new ComboBox
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
        public void ComboBox_ForeColor_SetWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
        {
            using var control = new ComboBox();
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
        public void ComboBox_ForeColor_SetWithHandler_CallsForeColorChanged()
        {
            using var control = new ComboBox();
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
        public void ComboBox_ForeColor_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ComboBox))[nameof(ComboBox.ForeColor)];
            using var control = new ComboBox();
            Assert.False(property.CanResetValue(control));

            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.WindowText, control.ForeColor);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void ComboBox_ForeColor_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ComboBox))[nameof(ComboBox.ForeColor)];
            using var control = new ComboBox();
            Assert.False(property.ShouldSerializeValue(control));

            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.WindowText, control.ForeColor);
            Assert.False(property.ShouldSerializeValue(control));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetPaddingNormalizedTheoryData))]
        public void ComboBox_Padding_Set_GetReturnsExpected(Padding value, Padding expected)
        {
            using var control = new ComboBox
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
        public void ComboBox_Padding_SetWithHandle_GetReturnsExpected(Padding value, Padding expected)
        {
            using var control = new ComboBox();
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
        public void ComboBox_Padding_SetWithHandler_CallsPaddingChanged()
        {
            using var control = new ComboBox();
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

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetRightToLeftTheoryData))]
        public void ComboBox_RightToLeft_Set_GetReturnsExpected(RightToLeft value, RightToLeft expected)
        {
            using var control = new ComboBox
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
        public void ComboBox_RightToLeft_SetWithHandler_CallsRightToLeftChanged()
        {
            using var control = new ComboBox();
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
        public void ComboBox_RightToLeft_SetInvalid_ThrowsInvalidEnumArgumentException(RightToLeft value)
        {
            using var control = new ComboBox();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.RightToLeft = value);
        }

        [WinFormsTheory]
        [InlineData(-1, "")]
        [InlineData(0, "System.Windows.Forms.Tests.ComboBoxTests+DataClass")]
        [InlineData(1, "System.Windows.Forms.Tests.ComboBoxTests+DataClass")]
        public void ComboBox_SelectedIndex_SetWithoutDisplayMember_GetReturnsExpected(int value, string expectedText)
        {
            using var control = new ComboBox();
            control.Items.Add(new DataClass { Value = "Value1" });
            control.Items.Add(new DataClass { Value = "Value2" });

            control.SelectedIndex = value;
            Assert.Equal(value, control.SelectedIndex);
            Assert.Equal(value == -1 ? null : control.Items[control.SelectedIndex], control.SelectedItem);
            Assert.Equal(expectedText, control.Text);

            // Set same.
            control.SelectedIndex = value;
            Assert.Equal(value, control.SelectedIndex);
            Assert.Equal(value == -1 ? null : control.Items[control.SelectedIndex], control.SelectedItem);
            Assert.Equal(expectedText, control.Text);
        }

        [WinFormsTheory]
        [InlineData(-1, "")]
        [InlineData(0, "Value1")]
        [InlineData(1, "Value2")]
        public void ComboBox_SelectedIndex_SetWithDisplayMember_GetReturnsExpected(int value, string expectedText)
        {
            using var control = new ComboBox
            {
                DisplayMember = "Value"
            };
            control.Items.Add(new DataClass { Value = "Value1" });
            control.Items.Add(new DataClass { Value = "Value2" });

            control.SelectedIndex = value;
            Assert.Equal(value, control.SelectedIndex);
            Assert.Equal(value == -1 ? null : control.Items[control.SelectedIndex], control.SelectedItem);
            Assert.Equal(expectedText, control.Text);

            // Set same.
            control.SelectedIndex = value;
            Assert.Equal(value, control.SelectedIndex);
            Assert.Equal(value == -1 ? null : control.Items[control.SelectedIndex], control.SelectedItem);
            Assert.Equal(expectedText, control.Text);
        }

        [WinFormsFact]
        public void ComboBox_SelectedText_GetWithoutHandle_ReturnsExpected()
        {
            using var control = new ComboBox();
            Assert.Empty(control.SelectedText);
            Assert.True(control.IsHandleCreated);

            // Get again.
            Assert.Empty(control.SelectedText);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ComboBox_SelectedText_GetWithHandle_ReturnsExpected()
        {
            using var control = new ComboBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            Assert.Empty(control.SelectedText);
            Assert.True(control.IsHandleCreated);

            // Get again.
            Assert.Empty(control.SelectedText);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ComboBox_SelectionLength_GetWithoutHandle_ReturnsExpected()
        {
            using var control = new ComboBox();
            Assert.Equal(0, control.SelectionLength);
            Assert.True(control.IsHandleCreated);

            // Get again.
            Assert.Equal(0, control.SelectionLength);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ComboBox_SelectionLength_GetWithHandle_ReturnsExpected()
        {
            using var control = new ComboBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            Assert.Equal(0, control.SelectionLength);
            Assert.True(control.IsHandleCreated);

            // Get again.
            Assert.Equal(0, control.SelectionLength);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ComboBox_SelectionStart_GetWithoutHandle_ReturnsExpected()
        {
            using var control = new ComboBox();
            Assert.False(control.IsHandleCreated);
            Assert.Equal(0, control.SelectionStart);
            Assert.True(control.IsHandleCreated); // SelectionStart forces Handle creating

            // Get again.
            Assert.Equal(0, control.SelectionStart);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ComboBox_SelectionStart_GetWithHandle_ReturnsExpected()
        {
            using var control = new ComboBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            Assert.Equal(0, control.SelectionStart);
            Assert.True(control.IsHandleCreated);

            // Get again.
            Assert.Equal(0, control.SelectionStart);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ComboBox_GetAutoSizeMode_Invoke_ReturnsExpected()
        {
            using var control = new SubComboBox();
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
        public void ComboBox_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
        {
            using var control = new SubComboBox();
            Assert.Equal(expected, control.GetStyle(flag));

            // Call again to test caching.
            Assert.Equal(expected, control.GetStyle(flag));
        }

        [WinFormsFact]
        public void ComboBox_GetTopLevel_Invoke_ReturnsExpected()
        {
            using var control = new SubComboBox();
            Assert.False(control.GetTopLevel());
        }

        public static IEnumerable<object[]> FindString_TestData()
        {
            foreach (int startIndex in new int[] { -2, -1, 0, 1 })
            {
                yield return new object[] { new ComboBox(), null, startIndex, -1 };
                yield return new object[] { new ComboBox(), string.Empty, startIndex, -1 };
                yield return new object[] { new ComboBox(), "s", startIndex, -1 };

                using var controlWithNoItems = new ComboBox();
                Assert.Empty(controlWithNoItems.Items);
                yield return new object[] { new ComboBox(), null, startIndex, -1 };
                yield return new object[] { new ComboBox(), string.Empty, startIndex, -1 };
                yield return new object[] { new ComboBox(), "s", startIndex, -1 };
            }

            using var controlWithItems = new ComboBox
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
        public void ComboBox_FindString_Invoke_ReturnsExpected(ComboBox control, string s, int startIndex, int expected)
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
        public void ComboBox_FindString_InvalidStartIndex_ThrowsArgumentOutOfRangeException(int startIndex)
        {
            using var control = new ComboBox();
            control.Items.Add("item");
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => control.FindString("s", startIndex));
        }

        public static IEnumerable<object[]> FindStringExact_TestData()
        {
            foreach (int startIndex in new int[] { -2, -1, 0, 1 })
            {
                foreach (bool ignoreCase in new bool[] { true, false })
                {
                    yield return new object[] { new ComboBox(), null, startIndex, ignoreCase, -1 };
                    yield return new object[] { new ComboBox(), string.Empty, startIndex, ignoreCase, -1 };
                    yield return new object[] { new ComboBox(), "s", startIndex, ignoreCase, -1 };

                    using var controlWithNoItems = new ComboBox();
                    Assert.Empty(controlWithNoItems.Items);
                    yield return new object[] { new ComboBox(), null, startIndex, ignoreCase, -1 };
                    yield return new object[] { new ComboBox(), string.Empty, startIndex, ignoreCase, -1 };
                    yield return new object[] { new ComboBox(), "s", startIndex, ignoreCase, -1 };
                }
            }

            using var controlWithItems = new ComboBox
            {
                DisplayMember = "Value"
            };
            controlWithItems.Items.Add(new DataClass { Value = "abc" });
            controlWithItems.Items.Add(new DataClass { Value = "abc" });
            controlWithItems.Items.Add(new DataClass { Value = "ABC" });
            controlWithItems.Items.Add(new DataClass { Value = "def" });
            controlWithItems.Items.Add(new DataClass { Value = "" });
            controlWithItems.Items.Add(new DataClass { Value = null });

            foreach (bool ignoreCase in new bool[] { true, false })
            {
                yield return new object[] { controlWithItems, "abc", -1, ignoreCase, 0 };
                yield return new object[] { controlWithItems, "abc", 0, ignoreCase, 1 };
                yield return new object[] { controlWithItems, "abc", 1, ignoreCase, ignoreCase ? 2 : 0 };
                yield return new object[] { controlWithItems, "abc", 2, ignoreCase, 0 };
                yield return new object[] { controlWithItems, "abc", 5, ignoreCase, 0 };
            }

            yield return new object[] { controlWithItems, "ABC", -1, false, 2 };
            yield return new object[] { controlWithItems, "ABC", 0, false, 2 };
            yield return new object[] { controlWithItems, "ABC", 1, false, 2 };
            yield return new object[] { controlWithItems, "ABC", 2, false, 2 };
            yield return new object[] { controlWithItems, "ABC", 5, false, 2 };

            yield return new object[] { controlWithItems, "ABC", -1, true, 0 };
            yield return new object[] { controlWithItems, "ABC", 0, true, 1 };
            yield return new object[] { controlWithItems, "ABC", 1, true, 2 };
            yield return new object[] { controlWithItems, "ABC", 2, true, 0 };
            yield return new object[] { controlWithItems, "ABC", 5, true, 0 };

            foreach (bool ignoreCase in new bool[] { true, false })
            {
                yield return new object[] { controlWithItems, "a", -1, ignoreCase, -1 };
                yield return new object[] { controlWithItems, "a", 0, ignoreCase, -1 };
                yield return new object[] { controlWithItems, "a", 1, ignoreCase, -1 };
                yield return new object[] { controlWithItems, "a", 2, ignoreCase, -1 };
                yield return new object[] { controlWithItems, "a", 5, ignoreCase, -1 };

                yield return new object[] { controlWithItems, "A", -1, ignoreCase, -1 };
                yield return new object[] { controlWithItems, "A", 0, ignoreCase, -1 };
                yield return new object[] { controlWithItems, "A", 1, ignoreCase, -1 };
                yield return new object[] { controlWithItems, "A", 2, ignoreCase, -1 };
                yield return new object[] { controlWithItems, "A", 5, ignoreCase, -1 };

                yield return new object[] { controlWithItems, "abcd", -1, ignoreCase, -1 };
                yield return new object[] { controlWithItems, "abcd", 0, ignoreCase, -1 };
                yield return new object[] { controlWithItems, "abcd", 1, ignoreCase, -1 };
                yield return new object[] { controlWithItems, "abcd", 2, ignoreCase, -1 };
                yield return new object[] { controlWithItems, "abcd", 5, ignoreCase, -1 };

                yield return new object[] { controlWithItems, "def", -1, ignoreCase, 3 };
                yield return new object[] { controlWithItems, "def", 0, ignoreCase, 3 };
                yield return new object[] { controlWithItems, "def", 1, ignoreCase, 3 };
                yield return new object[] { controlWithItems, "def", 2, ignoreCase, 3 };
                yield return new object[] { controlWithItems, "def", 5, ignoreCase, 3 };

                yield return new object[] { controlWithItems, null, -1, ignoreCase, -1 };
                yield return new object[] { controlWithItems, null, 0, ignoreCase, -1 };
                yield return new object[] { controlWithItems, null, 1, ignoreCase, -1 };
                yield return new object[] { controlWithItems, null, 2, ignoreCase, -1 };
                yield return new object[] { controlWithItems, null, 5, ignoreCase, -1 };

                yield return new object[] { controlWithItems, string.Empty, -1, ignoreCase, 4 };
                yield return new object[] { controlWithItems, string.Empty, 0, ignoreCase, 4 };
                yield return new object[] { controlWithItems, string.Empty, 1, ignoreCase, 4 };
                yield return new object[] { controlWithItems, string.Empty, 2, ignoreCase, 4 };
                yield return new object[] { controlWithItems, string.Empty, 5, ignoreCase, 4 };

                yield return new object[] { controlWithItems, "NoSuchItem", -1, ignoreCase, -1 };
                yield return new object[] { controlWithItems, "NoSuchItem", 0, ignoreCase, -1 };
                yield return new object[] { controlWithItems, "NoSuchItem", 1, ignoreCase, -1 };
                yield return new object[] { controlWithItems, "NoSuchItem", 2, ignoreCase, -1 };
                yield return new object[] { controlWithItems, "NoSuchItem", 5, ignoreCase, -1 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(FindStringExact_TestData))]
        public void ComboBox_FindStringExact_Invoke_ReturnsExpected(ComboBox control, string s, int startIndex, bool ignoreCase, int expected)
        {
            if (ignoreCase)
            {
                if (startIndex == -1)
                {
                    Assert.Equal(expected, control.FindStringExact(s));
                }

                Assert.Equal(expected, control.FindStringExact(s, startIndex));
            }

            Assert.Equal(expected, control.FindStringExact(s, startIndex, ignoreCase));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(1)]
        [InlineData(2)]
        public void ComboBox_FindStringExact_InvalidStartIndex_ThrowsArgumentOutOfRangeException(int startIndex)
        {
            using var control = new ComboBox();
            control.Items.Add("item");
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => control.FindStringExact("s", startIndex));
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => control.FindStringExact("s", startIndex, ignoreCase: true));
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => control.FindStringExact("s", startIndex, ignoreCase: false));
        }

        private void SendCtrlBackspace(SubComboBox tb)
        {
            var message = new Message();
            tb.ProcessCmdKey(ref message, Keys.Control | Keys.Back);
        }

        [WinFormsFact]
        public void CtrlBackspaceTextRemainsEmpty()
        {
            using SubComboBox control = new SubComboBox();
            control.ConfigureForCtrlBackspace();
            SendCtrlBackspace(control);
            Assert.Equal("", control.Text);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetCtrlBackspaceData))]
        public void CtrlBackspaceTextChanged(string value, string expected, int cursorRelativeToEnd)
        {
            using SubComboBox control = new SubComboBox(value);
            control.ConfigureForCtrlBackspace(cursorRelativeToEnd);
            SendCtrlBackspace(control);
            Assert.Equal(expected, control.Text);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetCtrlBackspaceRepeatedData))]
        public void CtrlBackspaceRepeatedTextChanged(string value, string expected, int repeats)
        {
            using SubComboBox control = new SubComboBox(value);
            control.ConfigureForCtrlBackspace();
            for (int i = 0; i < repeats; i++)
            {
                SendCtrlBackspace(control);
            }
            Assert.Equal(expected, control.Text);
        }

        [WinFormsFact]
        public void CtrlBackspaceDeletesSelection()
        {
            using SubComboBox control = new SubComboBox("123-5-7-9");
            control.ConfigureForCtrlBackspace();
            control.SelectionStart = 2;
            control.SelectionLength = 5;
            SendCtrlBackspace(control);
            Assert.Equal("12-9", control.Text);
        }

        public static IEnumerable<object[]> WndProc_PaintWithoutWParam_TestData()
        {
            foreach (bool allPaintingInWmPaint in new bool[] { true, false })
            {
                yield return new object[] { FlatStyle.Flat, false, true, allPaintingInWmPaint, true, 0 };
                yield return new object[] { FlatStyle.Popup, false, true, allPaintingInWmPaint, true, 0 };
                yield return new object[] { FlatStyle.Standard, false, true, allPaintingInWmPaint, false, 0 };
                yield return new object[] { FlatStyle.System, false, true, allPaintingInWmPaint, false, 0 };
                yield return new object[] { FlatStyle.Flat, false, false, allPaintingInWmPaint, true, 0 };
                yield return new object[] { FlatStyle.Popup, false, false, allPaintingInWmPaint, true, 0 };
                yield return new object[] { FlatStyle.Standard, false, false, allPaintingInWmPaint, false, 0 };
                yield return new object[] { FlatStyle.System, false, false, allPaintingInWmPaint, false, 0 };

                yield return new object[] { FlatStyle.Flat, true, true, allPaintingInWmPaint, true, 0 };
                yield return new object[] { FlatStyle.Popup, true, true, allPaintingInWmPaint, true, 0 };
                yield return new object[] { FlatStyle.Standard, true, true, allPaintingInWmPaint, true, 0 };
                yield return new object[] { FlatStyle.System, true, true, allPaintingInWmPaint, true, 0 };
                yield return new object[] { FlatStyle.Flat, true, false, allPaintingInWmPaint, true, 1 };
                yield return new object[] { FlatStyle.Popup, true, false, allPaintingInWmPaint, true, 1 };
                yield return new object[] { FlatStyle.Standard, true, false, allPaintingInWmPaint, true, 1 };
                yield return new object[] { FlatStyle.System, true, false, allPaintingInWmPaint, true, 1 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(WndProc_PaintWithoutWParam_TestData))]
        public void ComboBox_WndProc_InvokePaintWithoutWParam_Success(FlatStyle flatStyle, bool userPaint, bool doubleBuffered, bool allPaintingInWmPaint, bool expectedIsHandleCreated, int expectedPaintCallCount)
        {
            using (new NoAssertContext())
            {
                using var control = new SubComboBox
                {
                    FlatStyle = flatStyle
                };
                control.SetStyle(ControlStyles.UserPaint, userPaint);
                control.SetStyle(ControlStyles.OptimizedDoubleBuffer, doubleBuffered);
                control.SetStyle(ControlStyles.AllPaintingInWmPaint, allPaintingInWmPaint);
                int paintCallCount = 0;
                control.Paint += (sender, e) => paintCallCount++;

                var m = new Message
                {
                    Msg = (int)User32.WM.PAINT
                };
                control.WndProc(ref m);
                Assert.Equal(expectedIsHandleCreated, control.IsHandleCreated);
                Assert.Equal(expectedPaintCallCount, paintCallCount);
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(WndProc_PaintWithoutWParam_TestData))]
        public void ComboBox_WndProc_InvokePaintWithoutWParamWithBounds_Success(FlatStyle flatStyle, bool userPaint, bool doubleBuffered, bool allPaintingInWmPaint, bool expectedIsHandleCreated, int expectedPaintCallCount)
        {
            using (new NoAssertContext())
            {
                using var control = new SubComboBox
                {
                    FlatStyle = flatStyle,
                    Bounds = new Rectangle(1, 2, 30, 40)
                };
                control.SetStyle(ControlStyles.UserPaint, userPaint);
                control.SetStyle(ControlStyles.OptimizedDoubleBuffer, doubleBuffered);
                control.SetStyle(ControlStyles.AllPaintingInWmPaint, allPaintingInWmPaint);
                int paintCallCount = 0;
                control.Paint += (sender, e) => paintCallCount++;

                var m = new Message
                {
                    Msg = (int)User32.WM.PAINT
                };
                control.WndProc(ref m);
                Assert.Equal(expectedIsHandleCreated, control.IsHandleCreated);
                Assert.Equal(expectedPaintCallCount, paintCallCount);
            }
        }

        public static IEnumerable<object[]> WndProc_PaintWithoutWParamWithHandle_TestData()
        {
            foreach (bool allPaintingInWmPaint in new bool[] { true, false })
            {
                yield return new object[] { FlatStyle.Flat, false, true, allPaintingInWmPaint, 0 };
                yield return new object[] { FlatStyle.Popup, false, true, allPaintingInWmPaint, 0 };
                yield return new object[] { FlatStyle.Standard, false, true, allPaintingInWmPaint, 0 };
                yield return new object[] { FlatStyle.System, false, true, allPaintingInWmPaint, 0 };
                yield return new object[] { FlatStyle.Flat, false, false, allPaintingInWmPaint, 0 };
                yield return new object[] { FlatStyle.Popup, false, false, allPaintingInWmPaint, 0 };
                yield return new object[] { FlatStyle.Standard, false, false, allPaintingInWmPaint, 0 };
                yield return new object[] { FlatStyle.System, false, false, allPaintingInWmPaint, 0 };

                yield return new object[] { FlatStyle.Flat, true, true, allPaintingInWmPaint, 0 };
                yield return new object[] { FlatStyle.Popup, true, true, allPaintingInWmPaint, 0 };
                yield return new object[] { FlatStyle.Standard, true, true, allPaintingInWmPaint, 0 };
                yield return new object[] { FlatStyle.System, true, true, allPaintingInWmPaint, 0 };
                yield return new object[] { FlatStyle.Flat, true, false, allPaintingInWmPaint, 1 };
                yield return new object[] { FlatStyle.Popup, true, false, allPaintingInWmPaint, 1 };
                yield return new object[] { FlatStyle.Standard, true, false, allPaintingInWmPaint, 1 };
                yield return new object[] { FlatStyle.System, true, false, allPaintingInWmPaint, 1 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(WndProc_PaintWithoutWParamWithHandle_TestData))]
        public void ComboBox_WndProc_InvokePaintWithoutWParamWithHandle_Success(FlatStyle flatStyle, bool userPaint, bool doubleBuffered, bool allPaintingInWmPaint, int expectedPaintCallCount)
        {
            using var control = new SubComboBox
            {
                FlatStyle = flatStyle
            };
            control.SetStyle(ControlStyles.UserPaint, userPaint);
            control.SetStyle(ControlStyles.OptimizedDoubleBuffer, doubleBuffered);
            control.SetStyle(ControlStyles.AllPaintingInWmPaint, allPaintingInWmPaint);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int paintCallCount = 0;
            control.Paint += (sender, e) => paintCallCount++;

            var m = new Message
            {
                Msg = (int)User32.WM.PAINT
            };
            control.WndProc(ref m);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.Equal(expectedPaintCallCount, paintCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(WndProc_PaintWithoutWParamWithHandle_TestData))]
        public void ComboBox_WndProc_InvokePaintWithoutWParamWithBoundsWithHandle_Success(FlatStyle flatStyle, bool userPaint, bool doubleBuffered, bool allPaintingInWmPaint, int expectedPaintCallCount)
        {
            using var control = new SubComboBox
            {
                FlatStyle = flatStyle,
                Bounds = new Rectangle(1, 2, 30, 40)
            };
            control.SetStyle(ControlStyles.UserPaint, userPaint);
            control.SetStyle(ControlStyles.OptimizedDoubleBuffer, doubleBuffered);
            control.SetStyle(ControlStyles.AllPaintingInWmPaint, allPaintingInWmPaint);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int paintCallCount = 0;
            control.Paint += (sender, e) => paintCallCount++;

            var m = new Message
            {
                Msg = (int)User32.WM.PAINT
            };
            control.WndProc(ref m);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.Equal(expectedPaintCallCount, paintCallCount);
        }

        public static IEnumerable<object[]> WndProc_PaintWithWParam_TestData()
        {
            foreach (bool allPaintingInWmPaint in new bool[] { true, false })
            {
                yield return new object[] { FlatStyle.Flat, false, true, allPaintingInWmPaint, true, 0 };
                yield return new object[] { FlatStyle.Popup, false, true, allPaintingInWmPaint, true, 0 };
                yield return new object[] { FlatStyle.Standard, false, true, allPaintingInWmPaint, false, 0 };
                yield return new object[] { FlatStyle.System, false, true, allPaintingInWmPaint, false, 0 };
                yield return new object[] { FlatStyle.Flat, false, false, allPaintingInWmPaint, true, 0 };
                yield return new object[] { FlatStyle.Popup, false, false, allPaintingInWmPaint, true, 0 };
                yield return new object[] { FlatStyle.Standard, false, false, allPaintingInWmPaint, false, 0 };
                yield return new object[] { FlatStyle.System, false, false, allPaintingInWmPaint, false, 0 };

                yield return new object[] { FlatStyle.Flat, true, true, allPaintingInWmPaint, false, 1 };
                yield return new object[] { FlatStyle.Popup, true, true, allPaintingInWmPaint, false, 1 };
                yield return new object[] { FlatStyle.Standard, true, true, allPaintingInWmPaint, false, 1 };
                yield return new object[] { FlatStyle.System, true, true, allPaintingInWmPaint, false, 1 };
                yield return new object[] { FlatStyle.Flat, true, false, allPaintingInWmPaint, false, 1 };
                yield return new object[] { FlatStyle.Popup, true, false, allPaintingInWmPaint, false, 1 };
                yield return new object[] { FlatStyle.Standard, true, false, allPaintingInWmPaint, false, 1 };
                yield return new object[] { FlatStyle.System, true, false, allPaintingInWmPaint, false, 1 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(WndProc_PaintWithWParam_TestData))]
        public void ComboBox_WndProc_InvokePaintWithWParam_Success(FlatStyle flatStyle, bool userPaint, bool doubleBuffered, bool allPaintingInWmPaint, bool expectedIsHandleCreated, int expectedPaintCallCount)
        {
            using (new NoAssertContext())
            {
                using var image = new Bitmap(10, 10);
                using Graphics graphics = Graphics.FromImage(image);
                IntPtr hdc = graphics.GetHdc();
                try
                {
                    using var control = new SubComboBox
                    {
                        FlatStyle = flatStyle
                    };
                    control.SetStyle(ControlStyles.UserPaint, userPaint);
                    control.SetStyle(ControlStyles.OptimizedDoubleBuffer, doubleBuffered);
                    control.SetStyle(ControlStyles.AllPaintingInWmPaint, allPaintingInWmPaint);
                    int paintCallCount = 0;
                    control.Paint += (sender, e) => paintCallCount++;

                    var m = new Message
                    {
                        Msg = (int)User32.WM.PAINT,
                        WParam = hdc
                    };
                    control.WndProc(ref m);
                    Assert.Equal(expectedIsHandleCreated, control.IsHandleCreated);
                    Assert.Equal(expectedPaintCallCount, paintCallCount);
                }
                finally
                {
                    graphics.ReleaseHdc();
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(WndProc_PaintWithWParam_TestData))]
        public void ComboBox_WndProc_InvokePaintWithWParamWithBounds_Success(FlatStyle flatStyle, bool userPaint, bool doubleBuffered, bool allPaintingInWmPaint, bool expectedIsHandleCreated, int expectedPaintCallCount)
        {
            using (new NoAssertContext())
            {
                using var image = new Bitmap(10, 10);
                using Graphics graphics = Graphics.FromImage(image);
                IntPtr hdc = graphics.GetHdc();
                try
                {
                    using var control = new SubComboBox
                    {
                        FlatStyle = flatStyle,
                        Bounds = new Rectangle(1, 2, 30, 40)
                    };
                    control.SetStyle(ControlStyles.UserPaint, userPaint);
                    control.SetStyle(ControlStyles.OptimizedDoubleBuffer, doubleBuffered);
                    control.SetStyle(ControlStyles.AllPaintingInWmPaint, allPaintingInWmPaint);
                    int paintCallCount = 0;
                    control.Paint += (sender, e) => paintCallCount++;

                    var m = new Message
                    {
                        Msg = (int)User32.WM.PAINT,
                        WParam = hdc
                    };
                    control.WndProc(ref m);
                    Assert.Equal(expectedIsHandleCreated, control.IsHandleCreated);
                    Assert.Equal(expectedPaintCallCount, paintCallCount);
                }
                finally
                {
                    graphics.ReleaseHdc();
                }
            }
        }

        public static IEnumerable<object[]> WndProc_PaintWithWParamWithHandle_TestData()
        {
            foreach (bool allPaintingInWmPaint in new bool[] { true, false })
            {
                yield return new object[] { FlatStyle.Flat, false, true, allPaintingInWmPaint, 0 };
                yield return new object[] { FlatStyle.Popup, false, true, allPaintingInWmPaint, 0 };
                yield return new object[] { FlatStyle.Standard, false, true, allPaintingInWmPaint, 0 };
                yield return new object[] { FlatStyle.System, false, true, allPaintingInWmPaint, 0 };
                yield return new object[] { FlatStyle.Flat, false, false, allPaintingInWmPaint, 0 };
                yield return new object[] { FlatStyle.Popup, false, false, allPaintingInWmPaint, 0 };
                yield return new object[] { FlatStyle.Standard, false, false, allPaintingInWmPaint, 0 };
                yield return new object[] { FlatStyle.System, false, false, allPaintingInWmPaint, 0 };

                yield return new object[] { FlatStyle.Flat, true, true, allPaintingInWmPaint, 1 };
                yield return new object[] { FlatStyle.Popup, true, true, allPaintingInWmPaint, 1 };
                yield return new object[] { FlatStyle.Standard, true, true, allPaintingInWmPaint, 1 };
                yield return new object[] { FlatStyle.System, true, true, allPaintingInWmPaint, 1 };
                yield return new object[] { FlatStyle.Flat, true, false, allPaintingInWmPaint, 1 };
                yield return new object[] { FlatStyle.Popup, true, false, allPaintingInWmPaint, 1 };
                yield return new object[] { FlatStyle.Standard, true, false, allPaintingInWmPaint, 1 };
                yield return new object[] { FlatStyle.System, true, false, allPaintingInWmPaint, 1 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(WndProc_PaintWithWParamWithHandle_TestData))]
        public void ComboBox_WndProc_InvokePaintWithWParamWithHandle_Success(FlatStyle flatStyle, bool userPaint, bool doubleBuffered, bool allPaintingInWmPaint, int expectedPaintCallCount)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            IntPtr hdc = graphics.GetHdc();
            try
            {
                using var control = new SubComboBox
                {
                    FlatStyle = flatStyle
                };
                control.SetStyle(ControlStyles.UserPaint, userPaint);
                control.SetStyle(ControlStyles.OptimizedDoubleBuffer, doubleBuffered);
                control.SetStyle(ControlStyles.AllPaintingInWmPaint, allPaintingInWmPaint);
                Assert.NotEqual(IntPtr.Zero, control.Handle);
                int invalidatedCallCount = 0;
                control.Invalidated += (sender, e) => invalidatedCallCount++;
                int styleChangedCallCount = 0;
                control.StyleChanged += (sender, e) => styleChangedCallCount++;
                int createdCallCount = 0;
                control.HandleCreated += (sender, e) => createdCallCount++;
                int paintCallCount = 0;
                control.Paint += (sender, e) => paintCallCount++;

                var m = new Message
                {
                    Msg = (int)User32.WM.PAINT,
                    WParam = hdc
                };
                control.WndProc(ref m);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.Equal(expectedPaintCallCount, paintCallCount);
            }
            finally
            {
                graphics.ReleaseHdc();
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(WndProc_PaintWithWParamWithHandle_TestData))]
        public void ComboBox_WndProc_InvokePaintWithWParamWithBoundsWithHandle_Success(FlatStyle flatStyle, bool userPaint, bool doubleBuffered, bool allPaintingInWmPaint, int expectedPaintCallCount)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            IntPtr hdc = graphics.GetHdc();
            try
            {
                using var control = new SubComboBox
                {
                    FlatStyle = flatStyle,
                    Bounds = new Rectangle(1, 2, 30, 40)
                };
                control.SetStyle(ControlStyles.UserPaint, userPaint);
                control.SetStyle(ControlStyles.OptimizedDoubleBuffer, doubleBuffered);
                control.SetStyle(ControlStyles.AllPaintingInWmPaint, allPaintingInWmPaint);
                Assert.NotEqual(IntPtr.Zero, control.Handle);
                int invalidatedCallCount = 0;
                control.Invalidated += (sender, e) => invalidatedCallCount++;
                int styleChangedCallCount = 0;
                control.StyleChanged += (sender, e) => styleChangedCallCount++;
                int createdCallCount = 0;
                control.HandleCreated += (sender, e) => createdCallCount++;
                int paintCallCount = 0;
                control.Paint += (sender, e) => paintCallCount++;

                var m = new Message
                {
                    Msg = (int)User32.WM.PAINT,
                    WParam = hdc
                };
                control.WndProc(ref m);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.Equal(expectedPaintCallCount, paintCallCount);
            }
            finally
            {
                graphics.ReleaseHdc();
            }
        }

        private class SubComboBox : ComboBox
        {
            public SubComboBox()
            { }

            public SubComboBox(string text)
            {
                Text = text;
            }

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

            public new void CreateHandle() => base.CreateHandle();

            public void ConfigureForCtrlBackspace(int cursorRelativeToEnd = 0)
            {
                Focus();
                SelectionStart = this.Text.Length + cursorRelativeToEnd;
                SelectionLength = 0;
            }

            public new void Dispose(bool disposing) => base.Dispose(disposing);

            public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

            public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

            public new bool GetTopLevel() => base.GetTopLevel();

            public new bool IsInputKey(Keys keyData) => base.IsInputKey(keyData);

            public new void OnDrawItem(DrawItemEventArgs e) => base.OnDrawItem(e);

            public new void OnDropDown(EventArgs e) => base.OnDropDown(e);

            public new void OnDropDownStyleChanged(EventArgs e) => base.OnDropDownStyleChanged(e);

            public new void OnFontChanged(EventArgs e) => base.OnFontChanged(e);

            public new void OnHandleCreated(EventArgs e) => base.OnHandleCreated(e);

            public new void OnHandleDestroyed(EventArgs e) => base.OnHandleDestroyed(e);

            public new void OnKeyDown(KeyEventArgs e) => base.OnKeyDown(e);

            public new void OnKeyPress(KeyPressEventArgs e) => base.OnKeyPress(e);

            public new void OnMeasureItem(MeasureItemEventArgs e) => base.OnMeasureItem(e);

            public new void OnMouseEnter(EventArgs e) => base.OnMouseEnter(e);

            public new void OnMouseLeave(EventArgs e) => base.OnMouseLeave(e);

            public new void OnParentBackColorChanged(EventArgs e) => base.OnParentBackColorChanged(e);

            public new void OnSelectedIndexChanged(EventArgs e) => base.OnSelectedIndexChanged(e);

            public new void OnSelectedItemChanged(EventArgs e) => base.OnSelectedItemChanged(e);

            public new void OnSelectedValueChanged(EventArgs e) => base.OnSelectedValueChanged(e);

            public new void OnSelectionChangeCommitted(EventArgs e) => base.OnSelectionChangeCommitted(e);

            public new bool ProcessCmdKey(ref Message msg, Keys keyData) => base.ProcessCmdKey(ref msg, keyData);

            public new void ScaleControl(SizeF factor, BoundsSpecified specified) => base.ScaleControl(factor, specified);

            public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);

            public new void WndProc(ref Message m) => base.WndProc(ref m);
        }

        private class DataClass
        {
            public string Value { get; set; }
        }
    }
}
