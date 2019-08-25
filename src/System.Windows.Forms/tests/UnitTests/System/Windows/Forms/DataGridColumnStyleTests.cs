// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridColumnStyleTests
    {
        [Fact]
        public void DataGridColumnStyle_Ctor_Default()
        {
            var style = new SubDataGridColumnStyle();
            Assert.Equal(HorizontalAlignment.Left, style.Alignment);
            Assert.True(style.CanRaiseEvents);
            Assert.Null(style.Container);
            Assert.Null(style.DataGridTableStyle);
            Assert.False(style.DesignMode);
            Assert.NotNull(style.Events);
            Assert.Same(style.Events, style.Events);
            Assert.Equal(Control.DefaultFont.Height, style.FontHeight);
            Assert.NotNull(style.HeaderAccessibleObject);
            Assert.Same(style.HeaderAccessibleObject, style.HeaderAccessibleObject);
            Assert.Empty(style.HeaderText);
            Assert.Empty(style.MappingName);
            Assert.Equal("(null)", style.NullText);
            Assert.Null(style.PropertyDescriptor);
            Assert.False(style.ReadOnly);
            Assert.Null(style.Site);
            Assert.Equal(-1, style.Width);
        }

        public static IEnumerable<object[]> Ctor_PropertyDescriptor_TestData()
        {
            yield return new object[] { null, false };
            yield return new object[] { TypeDescriptor.GetProperties(typeof(DataClass))[0], false };
            yield return new object[] { TypeDescriptor.GetProperties(typeof(ReadOnlyDataClass))[0], true };
        }

        [Theory]
        [MemberData(nameof(Ctor_PropertyDescriptor_TestData))]
        public void Ctor_PropertyDescriptor(PropertyDescriptor descriptor, bool expectedReadOnly)
        {
            var style = new SubDataGridColumnStyle(descriptor);
            Assert.Equal(HorizontalAlignment.Left, style.Alignment);
            Assert.True(style.CanRaiseEvents);
            Assert.Null(style.Container);
            Assert.Null(style.DataGridTableStyle);
            Assert.False(style.DesignMode);
            Assert.NotNull(style.Events);
            Assert.Same(style.Events, style.Events);
            Assert.Equal(Control.DefaultFont.Height, style.FontHeight);
            Assert.NotNull(style.HeaderAccessibleObject);
            Assert.Same(style.HeaderAccessibleObject, style.HeaderAccessibleObject);
            Assert.Empty(style.HeaderText);
            Assert.Empty(style.MappingName);
            Assert.Equal("(null)", style.NullText);
            Assert.Same(descriptor, style.PropertyDescriptor);
            Assert.Equal(expectedReadOnly, style.ReadOnly);
            Assert.Null(style.Site);
            Assert.Equal(-1, style.Width);
        }

        public static IEnumerable<object[]> Ctor_PropertyDescriptor_Bool_TestData()
        {
            yield return new object[] { null, true, string.Empty, false };
            yield return new object[] { null, false, string.Empty, false };

            yield return new object[] { TypeDescriptor.GetProperties(typeof(DataClass))[0], true, "Value1", false };
            yield return new object[] { TypeDescriptor.GetProperties(typeof(DataClass))[0], false, string.Empty, false };

            yield return new object[] { TypeDescriptor.GetProperties(typeof(ReadOnlyDataClass))[0], true, "Value1", true };
            yield return new object[] { TypeDescriptor.GetProperties(typeof(ReadOnlyDataClass))[0], false, string.Empty, true };
        }

        [Theory]
        [MemberData(nameof(Ctor_PropertyDescriptor_Bool_TestData))]
        public void Ctor_PropertyDescriptor_Bool(PropertyDescriptor descriptor, bool isDefault, string expectedMappingName, bool expectedReadOnly)
        {
            var style = new SubDataGridColumnStyle(descriptor, isDefault);
            Assert.Equal(HorizontalAlignment.Left, style.Alignment);
            Assert.True(style.CanRaiseEvents);
            Assert.Null(style.Container);
            Assert.Null(style.DataGridTableStyle);
            Assert.False(style.DesignMode);
            Assert.NotNull(style.Events);
            Assert.Same(style.Events, style.Events);
            Assert.Equal(Control.DefaultFont.Height, style.FontHeight);
            Assert.NotNull(style.HeaderAccessibleObject);
            Assert.Same(style.HeaderAccessibleObject, style.HeaderAccessibleObject);
            Assert.Equal(expectedMappingName, style.HeaderText);
            Assert.Equal(expectedMappingName, style.MappingName);
            Assert.Equal("(null)", style.NullText);
            Assert.Same(descriptor, style.PropertyDescriptor);
            Assert.Equal(expectedReadOnly, style.ReadOnly);
            Assert.Null(style.Site);
            Assert.Equal(-1, style.Width);
        }

        [Fact]
        public void DataGridColumnStyle_DefaultProperty_Get_ReturnsExpected()
        {
            PropertyDescriptor property = TypeDescriptor.GetDefaultProperty(typeof(DataGridColumnStyle));
            Assert.Equal("HeaderText", property.Name);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(HorizontalAlignment))]
        public void Alignment_Set_GetReturnsExpected(HorizontalAlignment value)
        {
            var style = new SubDataGridColumnStyle
            {
                Alignment = value
            };
            Assert.Equal(value, style.Alignment);

            // Set same.
            style.Alignment = value;
            Assert.Equal(value, style.Alignment);
        }

        [Fact]
        public void DataGridColumnStyle_Alignment_SetWithHandler_CallsAlignmentChanged()
        {
            var style = new SubDataGridColumnStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.AlignmentChanged += handler;

            // Set different.
            style.Alignment = HorizontalAlignment.Center;
            Assert.Equal(HorizontalAlignment.Center, style.Alignment);
            Assert.Equal(1, callCount);

            // Set same.
            style.Alignment = HorizontalAlignment.Center;
            Assert.Equal(HorizontalAlignment.Center, style.Alignment);
            Assert.Equal(1, callCount);

            // Set different.
            style.Alignment = HorizontalAlignment.Left;
            Assert.Equal(HorizontalAlignment.Left, style.Alignment);
            Assert.Equal(2, callCount);

            // Remove handler.
            style.AlignmentChanged -= handler;
            style.Alignment = HorizontalAlignment.Center;
            Assert.Equal(HorizontalAlignment.Center, style.Alignment);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(HorizontalAlignment))]
        public void Alignment_SetInvalid_ThrowsInvalidEnumArgumentException(HorizontalAlignment value)
        {
            var style = new SubDataGridColumnStyle();
            Assert.Throws<InvalidEnumArgumentException>("value", () => style.Alignment = value);
        }

        [Fact]
        public void DataGridColumnStyle_FontHeight_GetWithParentWithDataGrid_ReturnsExpected()
        {
            Font font = new Font(Control.DefaultFont.FontFamily, 15);
            var dataGrid = new DataGrid
            {
                Font = font
            };
            var tableStyle = new DataGridTableStyle();
            dataGrid.TableStyles.Add(tableStyle);
            var style = new SubDataGridColumnStyle();
            tableStyle.GridColumnStyles.Add(style);
            Assert.Equal(font.Height, style.FontHeight);
        }

        [Fact]
        public void DataGridColumnStyle_FontHeight_GetWithParentWithoutDataGrid_ReturnsExpected()
        {
            var tableStyle = new DataGridTableStyle();
            var style = new SubDataGridColumnStyle();
            tableStyle.GridColumnStyles.Add(style);
            Assert.Equal(Control.DefaultFont.Height, style.FontHeight);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void HeaderText_Set_GetReturnsExpected(string value, string expected)
        {
            var style = new SubDataGridColumnStyle
            {
                HeaderText = value
            };
            Assert.Equal(expected, style.HeaderText);

            // Set same.
            style.HeaderText = value;
            Assert.Equal(expected, style.HeaderText);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void HeaderText_SetWithPropertyDescriptor_GetReturnsExpected(string value, string expected)
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(DataClass))[0];
            var style = new SubDataGridColumnStyle(property)
            {
                HeaderText = value
            };
            Assert.Equal(expected, style.HeaderText);

            // Set same.
            style.HeaderText = value;
            Assert.Equal(expected, style.HeaderText);
        }

        [Fact]
        public void DataGridColumnStyle_HeaderText_SetWithHandler_CallsHeaderTextChanged()
        {
            var style = new SubDataGridColumnStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.HeaderTextChanged += handler;

            // Set different.
            style.HeaderText = "HeaderText";
            Assert.Equal("HeaderText", style.HeaderText);
            Assert.Equal(1, callCount);

            // Set same.
            style.HeaderText = "HeaderText";
            Assert.Equal("HeaderText", style.HeaderText);
            Assert.Equal(1, callCount);

            // Set different.
            style.HeaderText = "OtherHeaderText";
            Assert.Equal("OtherHeaderText", style.HeaderText);
            Assert.Equal(2, callCount);

            // Set null.
            style.HeaderText = null;
            Assert.Empty(style.HeaderText);
            Assert.Equal(3, callCount);

            // Set null again.
            style.HeaderText = null;
            Assert.Empty(style.HeaderText);
            Assert.Equal(3, callCount);

            // Set empty.
            style.HeaderText = string.Empty;
            Assert.Empty(style.HeaderText);
            Assert.Equal(3, callCount);

            // Remove handler.
            style.HeaderTextChanged -= handler;
            style.HeaderText = "HeaderText";
            Assert.Equal("HeaderText", style.HeaderText);
            Assert.Equal(3, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void MappingName_Set_GetReturnsExpected(string value, string expected)
        {
            var style = new SubDataGridColumnStyle
            {
                MappingName = value
            };
            Assert.Equal(expected, style.MappingName);

            style.MappingName = value;
            Assert.Equal(expected, style.MappingName);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void MappingName_SetWithDataGridView_GetReturnsExpected(string value, string expected)
        {
            var dataGrid = new DataGrid();
            var tableStyle = new DataGridTableStyle();
            dataGrid.TableStyles.Add(tableStyle);
            var style = new SubDataGridColumnStyle();
            tableStyle.GridColumnStyles.Add(style);

            style.MappingName = value;
            Assert.Equal(expected, style.MappingName);

            style.MappingName = value;
            Assert.Equal(expected, style.MappingName);
        }

        [Fact]
        public void DataGridColumnStyle_MappingName_SetWithHandler_CallsMappingNameChanged()
        {
            var style = new SubDataGridColumnStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.MappingNameChanged += handler;

            // Set different.
            style.MappingName = "MappingName";
            Assert.Equal("MappingName", style.MappingName);
            Assert.Equal(1, callCount);

            // Set same.
            style.MappingName = "MappingName";
            Assert.Equal("MappingName", style.MappingName);
            Assert.Equal(1, callCount);

            // Set different.
            style.MappingName = "OtherMappingName";
            Assert.Equal("OtherMappingName", style.MappingName);
            Assert.Equal(2, callCount);

            // Set null.
            style.MappingName = null;
            Assert.Empty(style.MappingName);
            Assert.Equal(3, callCount);

            // Set null again.
            style.MappingName = null;
            Assert.Empty(style.MappingName);
            Assert.Equal(3, callCount);

            // Set empty.
            style.MappingName = string.Empty;
            Assert.Empty(style.MappingName);
            Assert.Equal(3, callCount);

            // Remove handler.
            style.MappingNameChanged -= handler;
            style.MappingName = "MappingName";
            Assert.Equal("MappingName", style.MappingName);
            Assert.Equal(3, callCount);
        }

        [Fact]
        public void DataGridColumnStyle_MappingName_SetInvalid_ThrowsArgumentException()
        {
            var dataGrid = new DataGrid();
            var tableStyle = new DataGridTableStyle();
            dataGrid.TableStyles.Add(tableStyle);

            var style1 = new SubDataGridColumnStyle
            {
                MappingName = "MappingName1"
            };
            var style2 = new SubDataGridColumnStyle
            {
                MappingName = "MappingName2"
            };
            tableStyle.GridColumnStyles.Add(style1);
            tableStyle.GridColumnStyles.Add(style2);

            Assert.Throws<ArgumentException>("column", () => style2.MappingName = "MappingName1");
            Assert.Equal("MappingName2", style2.MappingName);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void NullText_Set_GetReturnsExpected(string value)
        {
            var style = new SubDataGridColumnStyle
            {
                NullText = value
            };
            Assert.Equal(value, style.NullText);

            // Set same.
            style.NullText = value;
            Assert.Equal(value, style.NullText);
        }

        [Fact]
        public void DataGridColumnStyle_NullText_SetWithHandler_CallsMappingNameChanged()
        {
            var style = new SubDataGridColumnStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.NullTextChanged += handler;

            // Set different.
            style.NullText = "NullText";
            Assert.Equal("NullText", style.NullText);
            Assert.Equal(1, callCount);

            // Set same.
            style.NullText = "NullText";
            Assert.Equal("NullText", style.NullText);
            Assert.Equal(1, callCount);

            // Set different.
            style.NullText = "OtherMappingName";
            Assert.Equal("OtherMappingName", style.NullText);
            Assert.Equal(2, callCount);

            // Set null.
            style.NullText = null;
            Assert.Null(style.NullText);
            Assert.Equal(3, callCount);

            // Set null again.
            style.NullText = null;
            Assert.Null(style.NullText);
            Assert.Equal(3, callCount);

            // Remove handler.
            style.NullTextChanged -= handler;
            style.NullText = "NullText";
            Assert.Equal("NullText", style.NullText);
            Assert.Equal(3, callCount);
        }

        public static IEnumerable<object[]> PropertyDescriptor_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { TypeDescriptor.GetProperties(typeof(DataClass))[0] };
        }

        [Theory]
        [MemberData(nameof(PropertyDescriptor_Set_TestData))]
        public void PropertyDescriptor_Set_GetReturnsExpected(PropertyDescriptor value)
        {
            var style = new SubDataGridColumnStyle
            {
                PropertyDescriptor = value
            };
            Assert.Same(value, style.PropertyDescriptor);

            // Set same.
            style.PropertyDescriptor = value;
            Assert.Same(value, style.PropertyDescriptor);
        }

        [Fact]
        public void DataGridColumnStyle_PropertyDescriptor_SetWithHandler_CallsPropertyDescriptorChanged()
        {
            var style = new SubDataGridColumnStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.PropertyDescriptorChanged += handler;

            // Set different.
            PropertyDescriptor descriptor1 = TypeDescriptor.GetProperties(typeof(DataClass))[0];
            style.PropertyDescriptor = descriptor1;
            Assert.Equal(descriptor1, style.PropertyDescriptor);
            Assert.Equal(1, callCount);

            // Set same.
            style.PropertyDescriptor = descriptor1;
            Assert.Equal(descriptor1, style.PropertyDescriptor);
            Assert.Equal(1, callCount);

            // Set different.
            PropertyDescriptor descriptor2 = TypeDescriptor.GetProperties(typeof(DataClass))[1];
            style.PropertyDescriptor = descriptor2;
            Assert.Equal(descriptor2, style.PropertyDescriptor);
            Assert.Equal(2, callCount);

            // Set null.
            style.PropertyDescriptor = null;
            Assert.Null(style.PropertyDescriptor);
            Assert.Equal(3, callCount);

            // Set null again.
            style.PropertyDescriptor = null;
            Assert.Null(style.PropertyDescriptor);
            Assert.Equal(3, callCount);

            // Remove handler.
            style.PropertyDescriptorChanged -= handler;
            style.PropertyDescriptor = descriptor1;
            Assert.Equal(descriptor1, style.PropertyDescriptor);
            Assert.Equal(3, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ReadOnly_Set_GetReturnsExpected(bool value)
        {
            var style = new SubDataGridColumnStyle
            {
                ReadOnly = value
            };
            Assert.Equal(value, style.ReadOnly);

            // Set same.
            style.ReadOnly = value;
            Assert.Equal(value, style.ReadOnly);

            // Set different
            style.ReadOnly = !value;
            Assert.Equal(!value, style.ReadOnly);
        }

        [Fact]
        public void DataGridColumnStyle_ReadOnly_SetWithHandler_CallsMappingNameChanged()
        {
            var style = new SubDataGridColumnStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.ReadOnlyChanged += handler;

            // Set different.
            style.ReadOnly = true;
            Assert.True(style.ReadOnly);
            Assert.Equal(1, callCount);

            // Set same.
            style.ReadOnly = true;
            Assert.True(style.ReadOnly);
            Assert.Equal(1, callCount);

            // Set different.
            style.ReadOnly = false;
            Assert.False(style.ReadOnly);
            Assert.Equal(2, callCount);

            // Remove handler.
            style.ReadOnlyChanged -= handler;
            style.ReadOnly = true;
            Assert.True(style.ReadOnly);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void Width_Set_GetReturnsExpected(int value)
        {
            var style = new SubDataGridColumnStyle
            {
                Width = value
            };
            Assert.Equal(value, style.Width);

            // Set same.
            style.Width = value;
            Assert.Equal(value, style.Width);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void Width_SetWithTableStyleWithDataGrid_GetReturnsExpected(int value)
        {
            var dataGrid = new DataGrid();
            int callCount = 0;
            dataGrid.Layout += (sender, e) => callCount++;
            var tableStyle = new DataGridTableStyle();
            dataGrid.TableStyles.Add(tableStyle);
            var style = new SubDataGridColumnStyle();
            tableStyle.GridColumnStyles.Add(style);

            style.Width = value;
            Assert.Equal(value, style.Width);
            Assert.Equal(1, callCount);

            // Set same.
            style.Width = value;
            Assert.Equal(value, style.Width);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void Width_SetWithTableStyleWithoutDataGrid_GetReturnsExpected(int value)
        {
            var tableStyle = new DataGridTableStyle();
            var style = new SubDataGridColumnStyle();
            tableStyle.GridColumnStyles.Add(style);

            style.Width = value;
            Assert.Equal(value, style.Width);

            // Set same.
            style.Width = value;
            Assert.Equal(value, style.Width);
        }

        [Fact]
        public void DataGridColumnStyle_Width_SetWithHandler_CallsWidthChanged()
        {
            var style = new SubDataGridColumnStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.WidthChanged += handler;

            // Set different.
            style.Width = 2;
            Assert.Equal(2, style.Width);
            Assert.Equal(1, callCount);

            // Set same.
            style.Width = 2;
            Assert.Equal(2, style.Width);
            Assert.Equal(1, callCount);

            // Set different.
            style.Width = 3;
            Assert.Equal(3, style.Width);
            Assert.Equal(2, callCount);

            // Remove handler.
            style.WidthChanged -= handler;
            style.Width = 2;
            Assert.Equal(2, style.Width);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void DataGridColumnStyle_FontChanged_Add_Remove_Success()
        {
            var style = new SubDataGridColumnStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) => callCount++;

            style.FontChanged += handler;
            style.FontChanged -= handler;
            Assert.Equal(0, callCount);
        }

        [Fact]
        public void DataGridColumnStyle_BeginUpdate_EndUpdate_DoesNotCallInvalidate()
        {
            var style = new InvalidateDataGridColumnStyle();
            style.BeginUpdate();
            style.EndUpdate();
            Assert.Equal(0, style.InvalidateCallCount);
        }

        [Fact]
        public void DataGridColumnStyle_BeginUpdate_SeveralTimes_DoesNotCallInvalidate()
        {
            var style = new InvalidateDataGridColumnStyle();
            style.BeginUpdate();
            style.BeginUpdate();
            Assert.Equal(0, style.InvalidateCallCount);
        }

        [Fact]
        public void DataGridColumnStyle_CheckValidDataSource_HasPropertyDescriptor_Nop()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(DataClass))[0];
            var style = new SubDataGridColumnStyle(property);
            var context = new BindingContext();
            var dataSource = new List<int> { 1, 2, 3 };
            CurrencyManager value = Assert.IsType<CurrencyManager>(context[dataSource]);
            style.CheckValidDataSource(value);
        }

        [Fact]
        public void DataGridColumnStyle_CheckValidDataSource_NullValue_ThrowsArgumentNullException()
        {
            var style = new SubDataGridColumnStyle();
            Assert.Throws<ArgumentNullException>("value", () => style.CheckValidDataSource(null));
        }

        [Fact]
        public void DataGridColumnStyle_CheckValidDataSource_NoPropertyDescriptor_ThrowsInvalidOperationException()
        {
            var style = new SubDataGridColumnStyle();
            var context = new BindingContext();
            var dataSource = new List<int> { 1, 2, 3 };
            CurrencyManager value = Assert.IsType<CurrencyManager>(context[dataSource]);
            Assert.Throws<InvalidOperationException>(() => style.CheckValidDataSource(value));
        }

        public static IEnumerable<object[]> ColumnStartedEditing_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new Control() };
        }

        [Theory]
        [MemberData(nameof(ColumnStartedEditing_TestData))]
        public void ColumnStartedEditing_WithTableStyleWithDataGridWithoutRows_Nop(Control editingControl)
        {
            var dataGrid = new DataGrid();
            var tableStyle = new DataGridTableStyle();
            dataGrid.TableStyles.Add(tableStyle);
            var style = new SubDataGridColumnStyle();
            tableStyle.GridColumnStyles.Add(style);
            style.ColumnStartedEditing(editingControl);
        }

        [Theory]
        [MemberData(nameof(ColumnStartedEditing_TestData))]
        public void ColumnStartedEditing_WithTableStyle_Nop(Control editingControl)
        {
            var tableStyle = new DataGridTableStyle();
            var style = new SubDataGridColumnStyle();
            tableStyle.GridColumnStyles.Add(style);
            style.ColumnStartedEditing(editingControl);
        }

        [Theory]
        [MemberData(nameof(ColumnStartedEditing_TestData))]
        public void ColumnStartedEditing_NoTableStyle_Nop(Control editingControl)
        {
            var style = new SubDataGridColumnStyle();
            style.ColumnStartedEditing(editingControl);
        }

        [Fact]
        public void DataGridColumnStyle_ConcedeFocus_InvokeSeveralTimes_Nop()
        {
            var style = new SubDataGridColumnStyle();
            style.ConcedeFocus();
            style.ConcedeFocus();
        }

        [Fact]
        public void DataGridColumnStyle_CreateHeaderAccessibleObject_Invoke_ReturnsExpected()
        {
            var style = new SubDataGridColumnStyle();
            AccessibleObject accessibleObject = style.CreateHeaderAccessibleObject();
            Assert.NotNull(accessibleObject);
            Assert.NotSame(accessibleObject, style.CreateHeaderAccessibleObject());
        }

        public static IEnumerable<object[]> Edit_CurrencyManagerIntRectangleBool_TestData()
        {
            yield return new object[] { null, -2, Rectangle.Empty, false };

            var context = new BindingContext();
            var dataSource = new List<int> { 1, 2, 3 };
            yield return new object[] { context[dataSource], -1, new Rectangle(1, 2, 3, 4), true };
            yield return new object[] { context[dataSource], 1, new Rectangle(-1, -2, -3, -4), true };
        }

        [Theory]
        [MemberData(nameof(Edit_CurrencyManagerIntRectangleBool_TestData))]
        public void Edit_InvokeCurrencyManagerIntRectangleBool_Success(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly)
        {
            var style = new SubDataGridColumnStyle();
            int callCount = 0;
            style.EditAction = (actualSource, actualRowNum, actualBounds, actualReadOnly, actualDisplayText, actualCellIsVisible) =>
            {
                Assert.Same(source, actualSource);
                Assert.Equal(rowNum, actualRowNum);
                Assert.Equal(bounds, actualBounds);
                Assert.Equal(readOnly, actualReadOnly);
                Assert.Null(actualDisplayText);
                Assert.True(actualCellIsVisible);
                callCount++;
            };
            style.Edit(source, rowNum, bounds, readOnly);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> Edit_CurrencyManagerIntRectangleBoolString_TestData()
        {
            yield return new object[] { null, -2, Rectangle.Empty, false, null };

            var context = new BindingContext();
            var dataSource = new List<int> { 1, 2, 3 };
            yield return new object[] { context[dataSource], -1, new Rectangle(1, 2, 3, 4), true, string.Empty };
            yield return new object[] { context[dataSource], 1, new Rectangle(-1, -2, -3, -4), true, "displayText" };
        }

        [Theory]
        [MemberData(nameof(Edit_CurrencyManagerIntRectangleBoolString_TestData))]
        public void Edit_InvokeCurrencyManagerIntRectangleBoolString_Success(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly, string displayText)
        {
            var style = new SubDataGridColumnStyle();
            int callCount = 0;
            style.EditAction = (actualSource, actualRowNum, actualBounds, actualReadOnly, actualDisplayText, actualCellIsVisible) =>
            {
                Assert.Same(source, actualSource);
                Assert.Equal(rowNum, actualRowNum);
                Assert.Equal(bounds, actualBounds);
                Assert.Equal(readOnly, actualReadOnly);
                Assert.Same(displayText, actualDisplayText);
                Assert.True(actualCellIsVisible);
                callCount++;
            };
            style.Edit(source, rowNum, bounds, readOnly, displayText);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void DataGridColumnStyle_EndUpdate_BeganUpdateInvalidated_DoesNotCallInvalidate()
        {
            var style = new InvalidateDataGridColumnStyle();
            style.BeginUpdate();
            Assert.Equal(0, style.InvalidateCallCount);

            style.CallInvalidate();
            Assert.Equal(1, style.InvalidateCallCount);

            style.EndUpdate();
            Assert.Equal(2, style.InvalidateCallCount);
        }

        [Fact]
        public void DataGridColumnStyle_EndUpdate_NotCalledBeganUpdate_DoesNotCallInvalidate()
        {
            var style = new InvalidateDataGridColumnStyle();
            style.EndUpdate();
            Assert.Equal(0, style.InvalidateCallCount);
        }

        [Fact]
        public void DataGridColumnStyle_EndUpdate_SeveralTimes_DoesNotCallInvalidate()
        {
            var style = new InvalidateDataGridColumnStyle();
            style.BeginUpdate();
            style.EndUpdate();
            style.EndUpdate();
            Assert.Equal(0, style.InvalidateCallCount);
        }

        [Fact]
        public void DataGridColumnStyle_EnterNullValue_InvokeSeveralTimes_Nop()
        {
            var style = new SubDataGridColumnStyle();
            style.EnterNullValue();
            style.EnterNullValue();
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 2)]
        [InlineData(2, 3)]
        public void GetColumnValueAtRow_PropertyDescriptor_ReturnsExpected(int rowNum, int expected)
        {
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(typeof(DataClass)).Find(nameof(DataClass.Value1), ignoreCase: false);
            var style = new SubDataGridColumnStyle(descriptor);
            var context = new BindingContext();
            var dataSource = new List<DataClass> { new DataClass { Value1 = 1 }, new DataClass { Value1 = 2 }, new DataClass { Value1 = 3 } };
            CurrencyManager source = Assert.IsType<CurrencyManager>(context[dataSource]);
            Assert.Equal(expected, style.GetColumnValueAtRow(source, rowNum));
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 2)]
        [InlineData(2, 3)]
        public void GetColumnValueAtRow_PropertyDescriptorCalledTwice_ReturnsExpected(int rowNum, int expected)
        {
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(typeof(DataClass)).Find(nameof(DataClass.Value1), ignoreCase: false);
            var style = new NullPropertyDescriptorDataGridColumnStyle(descriptor, 2);
            var context = new BindingContext();
            var dataSource = new List<DataClass> { new DataClass { Value1 = 1 }, new DataClass { Value1 = 2 }, new DataClass { Value1 = 3 } };
            CurrencyManager source = Assert.IsType<CurrencyManager>(context[dataSource]);
            Assert.Equal(expected, style.GetColumnValueAtRow(source, rowNum));
        }

        [Fact]
        public void DataGridColumnStyle_GetColumnValueAtRow_NullSource_ThrowsArgumentNullException()
        {
            var style = new SubDataGridColumnStyle();
            Assert.Throws<ArgumentNullException>("value", () => style.GetColumnValueAtRow(null, 0));
        }

        [Fact]
        public void DataGridColumnStyle_GetColumnValueAtRow_NoPropertyDescriptor_ThrowsInvalidOperationException()
        {
            var style = new SubDataGridColumnStyle();
            var context = new BindingContext();
            var dataSource = new List<int> { 1, 2, 3 };
            CurrencyManager source = Assert.IsType<CurrencyManager>(context[dataSource]);
            Assert.Throws<InvalidOperationException>(() => style.GetColumnValueAtRow(source, 0));
        }

        [Fact]
        public void DataGridColumnStyle_GetColumnValueAtRow_NoPropertyDescriptorNoValidation_ThrowsInvalidOperationException()
        {
            var style = new NullPropertyDescriptorDataGridColumnStyle(1);
            var context = new BindingContext();
            var dataSource = new List<int> { 1, 2, 3 };
            CurrencyManager source = Assert.IsType<CurrencyManager>(context[dataSource]);
            Assert.Throws<InvalidOperationException>(() => style.GetColumnValueAtRow(source, 0));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(3)]
        public void GetColumnValueAtRow_InvalidIndex_ThrowsIndexOutOfRangeException(int rowNum)
        {
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(typeof(DataClass)).Find(nameof(DataClass.Value1), ignoreCase: false);
            var style = new SubDataGridColumnStyle(descriptor);
            var context = new BindingContext();
            var dataSource = new List<DataClass> { new DataClass { Value1 = 1 }, new DataClass { Value1 = 2 }, new DataClass { Value1 = 3 } };
            CurrencyManager source = Assert.IsType<CurrencyManager>(context[dataSource]);
            Assert.Throws<IndexOutOfRangeException>((() => style.GetColumnValueAtRow(source, rowNum)));
        }

        [Fact]
        public void DataGridColumnStyle_Invalidate_InvokeSeveralTimesWithTableStyleWithDataGrid_Nop()
        {
            var dataGrid = new DataGrid();
            var tableStyle = new DataGridTableStyle();
            dataGrid.TableStyles.Add(tableStyle);
            var style = new SubDataGridColumnStyle();
            tableStyle.GridColumnStyles.Add(style);
            style.Invalidate();
            style.Invalidate();
        }

        [Fact]
        public void DataGridColumnStyle_Invalidate_InvokeSeveralTimesWithTableStyle_Nop()
        {
            var tableStyle = new DataGridTableStyle();
            var style = new SubDataGridColumnStyle();
            tableStyle.GridColumnStyles.Add(style);
            style.Invalidate();
            style.Invalidate();
        }

        [Fact]
        public void DataGridColumnStyle_Invalidate_InvokeSeveralTimes_Nop()
        {
            var style = new SubDataGridColumnStyle();
            style.Invalidate();
            style.Invalidate();
        }

        public static IEnumerable<object[]> Paint_TestData()
        {
            yield return new object[] { null, Rectangle.Empty, null, -2, null, null, false };

            var image = new Bitmap(10, 10);
            var style = new SubDataGridColumnStyle();
            var context = new BindingContext();
            var dataSource = new List<DataClass> { new DataClass { Value1 = 1 }, new DataClass { Value1 = 2 }, new DataClass { Value1 = 3 } };
            CurrencyManager source = Assert.IsType<CurrencyManager>(context[dataSource]);
            yield return new object[] { Graphics.FromImage(image), new Rectangle(1, 2, 3, 4), source, -1, new SolidBrush(Color.Red), new SolidBrush(Color.Blue), true };
            yield return new object[] { Graphics.FromImage(image), new Rectangle(-1, -2, -3, -4), source, 1, new SolidBrush(Color.Red), new SolidBrush(Color.Blue), false };
        }

        [Theory]
        [MemberData(nameof(Paint_TestData))]
        public void Paint_Invoke_Success(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, Brush backBrush, Brush foreBrush, bool alignToRight)
        {
            var style = new SubDataGridColumnStyle();
            int callCount = 0;
            style.PaintAction = (actualG, actualBounds, actualSource, actualRowNum, actualAlignToRight) =>
            {
                Assert.Same(g, actualG);
                Assert.Same(source, actualSource);
                Assert.Equal(bounds, actualBounds);
                Assert.Equal(rowNum, actualRowNum);
                Assert.Equal(alignToRight, actualAlignToRight);
                callCount++;
            };
            style.Paint(g, bounds, source, rowNum, backBrush, foreBrush, alignToRight);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void DataGridColumnStyle_ReleaseHostedControl_InvokeSeveralTimes_Nop()
        {
            var style = new SubDataGridColumnStyle();
            style.ReleaseHostedControl();
            style.ReleaseHostedControl();
        }

        [Fact]
        public void DataGridColumnStyle_ResetHeaderText_Invoke_Success()
        {
            var style = new SubDataGridColumnStyle
            {
                HeaderText = "HeaderText"
            };
            style.ResetHeaderText();
            Assert.Empty(style.HeaderText);

            // Reset again.
            style.ResetHeaderText();
            Assert.Empty(style.HeaderText);
        }

        [Theory]
        [InlineData(0, 2)]
        [InlineData(1, 3)]
        [InlineData(2, 4)]
        public void SetColumnValueAtRow_PropertyDescriptor_ReturnsExpected(int rowNum, int value)
        {
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(typeof(DataClass)).Find(nameof(DataClass.Value1), ignoreCase: false);
            var style = new SubDataGridColumnStyle(descriptor);
            var context = new BindingContext();
            var dataSource = new List<DataClass> { new DataClass { Value1 = 1 }, new DataClass { Value1 = 2 }, new DataClass { Value1 = 3 } };
            CurrencyManager source = Assert.IsType<CurrencyManager>(context[dataSource]);
            source.Position = rowNum;

            style.SetColumnValueAtRow(source, rowNum, value);
            Assert.Equal(value, style.GetColumnValueAtRow(source, rowNum));
            Assert.Equal(value, dataSource[rowNum].Value1);
        }

        [Theory]
        [InlineData(0, 2)]
        [InlineData(1, 3)]
        [InlineData(2, 4)]
        public void SetColumnValueAtRow_IEditablePropertyDescriptor_ReturnsExpected(int rowNum, int value)
        {
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(typeof(EditableDataClass)).Find(nameof(EditableDataClass.Value), ignoreCase: false);
            var style = new SubDataGridColumnStyle(descriptor);
            var context = new BindingContext();
            var dataSource = new List<EditableDataClass> { new EditableDataClass { Value = 1 }, new EditableDataClass { Value = 2 }, new EditableDataClass { Value = 3 } };
            CurrencyManager source = Assert.IsType<CurrencyManager>(context[dataSource]);
            source.Position = rowNum;
            Assert.Equal(1, dataSource[rowNum].BeginEditCallCount);

            style.SetColumnValueAtRow(source, rowNum, value);
            Assert.Equal(value, style.GetColumnValueAtRow(source, rowNum));
            Assert.Equal(value, dataSource[rowNum].Value);
            Assert.Equal(2, dataSource[rowNum].BeginEditCallCount);
        }

        [Theory]
        [InlineData(0, 2)]
        [InlineData(1, 3)]
        [InlineData(2, 4)]
        public void SetColumnValueAtRow_PropertyDescriptorCalledTwice_ReturnsExpected(int rowNum, int value)
        {
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(typeof(DataClass)).Find(nameof(DataClass.Value1), ignoreCase: false);
            var style = new NullPropertyDescriptorDataGridColumnStyle(descriptor, 2);
            var context = new BindingContext();
            var dataSource = new List<DataClass> { new DataClass { Value1 = 1 }, new DataClass { Value1 = 2 }, new DataClass { Value1 = 3 } };
            CurrencyManager source = Assert.IsType<CurrencyManager>(context[dataSource]);
            source.Position = rowNum;

            style.SetColumnValueAtRow(source, rowNum, value);
            style.RequiredCallCount = int.MaxValue;
            Assert.Equal(value, style.GetColumnValueAtRow(source, rowNum));
            Assert.Equal(value, dataSource[rowNum].Value1);
        }

        [Fact]
        public void DataGridColumnStyle_SetColumnValueAtRow_NullSource_ThrowsArgumentNullException()
        {
            var style = new SubDataGridColumnStyle();
            Assert.Throws<ArgumentNullException>("value", () => style.SetColumnValueAtRow(null, 0, 1));
        }

        [Fact]
        public void DataGridColumnStyle_SetColumnValueAtRow_NoPropertyDescriptor_ThrowsInvalidOperationException()
        {
            var style = new SubDataGridColumnStyle();
            var context = new BindingContext();
            var dataSource = new List<int> { 1, 2, 3 };
            CurrencyManager source = Assert.IsType<CurrencyManager>(context[dataSource]);
            Assert.Throws<InvalidOperationException>(() => style.SetColumnValueAtRow(source, 0, 1));
        }

        [Fact]
        public void DataGridColumnStyle_SetColumnValueAtRow_NoPropertyDescriptorNoValidation_ThrowsInvalidOperationException()
        {
            var style = new NullPropertyDescriptorDataGridColumnStyle(1);
            var context = new BindingContext();
            var dataSource = new List<int> { 1, 2, 3 };
            CurrencyManager source = Assert.IsType<CurrencyManager>(context[dataSource]);
            Assert.Throws<InvalidOperationException>(() => style.SetColumnValueAtRow(source, 0, new object()));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void SetColumnValueAtRow_InvalidIndex_ThrowsArgumentException(int rowNum)
        {
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(typeof(DataClass)).Find(nameof(DataClass.Value1), ignoreCase: false);
            var style = new SubDataGridColumnStyle(descriptor);
            var context = new BindingContext();
            var dataSource = new List<DataClass> { new DataClass { Value1 = 1 }, new DataClass { Value1 = 2 }, new DataClass { Value1 = 3 } };
            CurrencyManager source = Assert.IsType<CurrencyManager>(context[dataSource]);
            Assert.Throws<ArgumentException>("rowNum", () => style.SetColumnValueAtRow(source, rowNum, 1));
        }

        public static IEnumerable<object[]> DataGrid_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new DataGrid() };
        }

        [Theory]
        [MemberData(nameof(DataGrid_TestData))]
        public void SetDataGrid_Invoke_CallsSetDataGridInColumn(DataGrid value)
        {
            var style = new SetDataGridInColumnDataGridColumnStyle();
            int callCount = 0;
            style.SetDataGridInColumnAction = actualValue =>
            {
                Assert.Same(value, actualValue);
                callCount++;
            };
            style.SetDataGrid(value);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> SetDataGridInColumn_TestData()
        {
            yield return new object[] { null, null, null };

            var dataGridWithoutListManager = new DataGrid();
            Assert.Null(dataGridWithoutListManager.ListManager);
            yield return new object[] { null, dataGridWithoutListManager, null };

            var dataGrid = new DataGrid
            {
                BindingContext = new BindingContext(),
                DataSource = new List<DataClass> { new DataClass { Value1 = 1 } }
            };
            Assert.NotNull(dataGrid.ListManager);
            yield return new object[] { null, dataGrid, null };
            yield return new object[] { string.Empty, dataGrid, null };
            yield return new object[] { "NoSuchProperty", dataGrid, null };
            yield return new object[] { nameof(DataClass.ListProperty), dataGrid, null };
            yield return new object[] { nameof(DataClass.Value1).ToLower(), dataGrid, null };
            yield return new object[] { nameof(DataClass.Value1), dataGrid, TypeDescriptor.GetProperties(typeof(DataClass)).Find(nameof(DataClass.Value1), ignoreCase: false) };
        }

        [Theory]
        [MemberData(nameof(SetDataGridInColumn_TestData))]
        public void SetDataGridInColumn_DataGridWithoutListManager_Nop(string headerText, DataGrid value, object expectedPropertyDescriptor)
        {
            var style = new SubDataGridColumnStyle
            {
                HeaderText = headerText
            };
            style.SetDataGrid(value);
            Assert.Equal(expectedPropertyDescriptor, style.PropertyDescriptor);
        }

        [Fact]
        public void DataGridColumnStyle_SetDataGridInColumn_HasPropertyDescriptor_Nop()
        {
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(typeof(DataClass)).Find(nameof(DataClass.Value1), ignoreCase: false);
            var style = new SubDataGridColumnStyle(descriptor)
            {
                HeaderText = nameof(DataClass.Value2)
            };
            var dataGrid = new DataGrid
            {
                BindingContext = new BindingContext(),
                DataSource = new List<DataClass> { new DataClass { Value1 = 1 } }
            };
            Assert.NotNull(dataGrid.ListManager);
            style.SetDataGridInColumn(dataGrid);
            Assert.Same(descriptor, style.PropertyDescriptor);
        }

        public static IEnumerable<object[]> UpdateUI_TestData()
        {
            yield return new object[] { null, -2, null };

            var style = new SubDataGridColumnStyle();
            var context = new BindingContext();
            var dataSource = new List<DataClass> { new DataClass { Value1 = 1 }, new DataClass { Value1 = 2 }, new DataClass { Value1 = 3 } };
            CurrencyManager source = Assert.IsType<CurrencyManager>(context[dataSource]);
            yield return new object[] { source, -1, string.Empty };
            yield return new object[] { source, 1, "displayText" };
        }

        [Theory]
        [MemberData(nameof(UpdateUI_TestData))]
        public void UpdateUI_Invoke_Nop(CurrencyManager source, int rowNum, string displayText)
        {
            var style = new SubDataGridColumnStyle();
            style.UpdateUI(source, rowNum, displayText);
        }

        private class DataClass
        {
            public int Value1 { get; set; }
            public int Value2 { get; set; }

            public IList ListProperty { get; set; }
        }

        private class EditableDataClass : IEditableObject
        {
            public int BeginEditCallCount { get; set; }

            public void BeginEdit() => BeginEditCallCount++;

            public void CancelEdit() => throw new NotImplementedException();

            public void EndEdit() { }

            public int Value { get; set; }
        }

        private class ReadOnlyDataClass
        {
            public int Value1 { get; }
        }

        private abstract class CustomDataGridColumnStyle : DataGridColumnStyle
        {
            public CustomDataGridColumnStyle()
            {
            }

            public CustomDataGridColumnStyle(PropertyDescriptor prop) : base(prop)
            {
            }

            public CustomDataGridColumnStyle(PropertyDescriptor prop, bool isDefault) : base(prop, isDefault)
            {
            }

            protected internal override void Abort(int rowNum)
            {
                throw new NotImplementedException();
            }

            protected internal override bool Commit(CurrencyManager dataSource, int rowNum)
            {
                throw new NotImplementedException();
            }

            protected internal override void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly, string displayText, bool cellIsVisible)
            {
                throw new NotImplementedException();
            }

            protected internal override Size GetPreferredSize(Graphics g, object value)
            {
                throw new NotImplementedException();
            }

            protected internal override int GetMinimumHeight()
            {
                throw new NotImplementedException();
            }

            protected internal override int GetPreferredHeight(Graphics g, object value)
            {
                throw new NotImplementedException();
            }

            protected internal override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum)
            {
                throw new NotImplementedException();
            }

            protected internal override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, bool alignToRight)
            {
                throw new NotImplementedException();
            }
        }

        private class SubDataGridColumnStyle : CustomDataGridColumnStyle
        {
            public SubDataGridColumnStyle()
            {
            }

            public SubDataGridColumnStyle(PropertyDescriptor prop) : base(prop)
            {
            }

            public SubDataGridColumnStyle(PropertyDescriptor prop, bool isDefault) : base(prop, isDefault)
            {
            }

            public new bool CanRaiseEvents => base.CanRaiseEvents;

            public new bool DesignMode => base.DesignMode;

            public new EventHandlerList Events => base.Events;

            public new int FontHeight => base.FontHeight;

            public new void BeginUpdate() => base.BeginUpdate();

            public new void CheckValidDataSource(CurrencyManager value) => base.CheckValidDataSource(value);

            public new AccessibleObject CreateHeaderAccessibleObject() => base.CreateHeaderAccessibleObject();

            public new void EndUpdate() => base.EndUpdate();

            public new void Invalidate() => base.Invalidate();

            public new void SetDataGrid(DataGrid value) => base.SetDataGrid(value);

            public new void SetDataGridInColumn(DataGrid value) => base.SetDataGridInColumn(value);

            public Action<CurrencyManager, int, Rectangle, bool, string, bool> EditAction { get; set; }

            protected internal override void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly, string displayText, bool cellIsVisible)
            {
                EditAction(source, rowNum, bounds, readOnly, displayText, cellIsVisible);
            }

            public Action<Graphics, Rectangle, CurrencyManager, int, bool> PaintAction { get; set; }

            protected internal override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, bool alignToRight)
            {
                PaintAction(g, bounds, source, rowNum, alignToRight);
            }
        }

        private class NullPropertyDescriptorDataGridColumnStyle : CustomDataGridColumnStyle
        {
            public NullPropertyDescriptorDataGridColumnStyle(int requiredCallCount)
            {
                RequiredCallCount = requiredCallCount;
            }

            public NullPropertyDescriptorDataGridColumnStyle(PropertyDescriptor prop, int requiredCallCount) : base(prop)
            {
                RequiredCallCount = requiredCallCount;
            }

            public int RequiredCallCount { get; set; }

            private int _callCount = 0;

            public override PropertyDescriptor PropertyDescriptor
            {
                get
                {
                    if (_callCount < RequiredCallCount)
                    {
                        _callCount++;
                        return TypeDescriptor.GetProperties(typeof(DataClass)).Find(nameof(DataClass.Value1), ignoreCase: false);
                    }

                    return null;
                }
                set { }
            }
        }

        private class InvalidateDataGridColumnStyle : CustomDataGridColumnStyle
        {
            public new void BeginUpdate() => base.BeginUpdate();

            public new void EndUpdate() => base.EndUpdate();

            public void CallInvalidate() => Invalidate();

            public int InvalidateCallCount { get; set; }

            protected override void Invalidate()
            {
                InvalidateCallCount++;
                base.Invalidate();
            }
        }

        private class SetDataGridInColumnDataGridColumnStyle : CustomDataGridColumnStyle
        {
            public new void SetDataGrid(DataGrid value) => base.SetDataGrid(value);

            public Action<DataGrid> SetDataGridInColumnAction { get; set; }

            protected override void SetDataGridInColumn(DataGrid value)
            {
                SetDataGridInColumnAction(value);
            }
        }
    }
}
