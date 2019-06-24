// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Layout;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class TableLayoutPanelTests
    {
        [Fact]
        public void TableLayoutPanel_Ctor_Default()
        {
            var panel = new TableLayoutPanel();
            Assert.Equal(BorderStyle.None, panel.BorderStyle);
            Assert.Empty(panel.Controls);
            Assert.NotNull(panel.LayoutSettings);
            Assert.Equal(0, panel.ColumnCount);
            Assert.Equal(0, panel.RowCount);
            Assert.Empty(panel.RowStyles);
            Assert.Same(panel.LayoutSettings.RowStyles, panel.RowStyles);
            Assert.Empty(panel.ColumnStyles);
            Assert.Same(panel.LayoutSettings.ColumnStyles, panel.ColumnStyles);
            Assert.Equal(TableLayoutPanelGrowStyle.AddRows, panel.GrowStyle);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(BorderStyle))]
        public void TableLayoutPanel_BorderStyle_Set_GetReturnsExpected(BorderStyle value)
        {
            var panel = new TableLayoutPanel
            {
                BorderStyle = value
            };
            Assert.Equal(value, panel.BorderStyle);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(BorderStyle))]
        public void TableLayoutPanel_BorderStyle_SetInvalid_ThrowsInvalidEnumArgumentException(BorderStyle value)
        {
            var panel = new TableLayoutPanel();
            Assert.Throws<InvalidEnumArgumentException>("value", () => panel.BorderStyle = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(TableLayoutPanelCellBorderStyle))]
        public void TableLayoutPanel_CellBorderStyle_Set_GetReturnsExpected(TableLayoutPanelCellBorderStyle value)
        {
            var panel = new TableLayoutPanel
            {
                CellBorderStyle = value
            };
            Assert.Equal(value, panel.CellBorderStyle);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(TableLayoutPanelCellBorderStyle))]
        public void TableLayoutPanel_CellBorderStyle_SetInvalid_ThrowsArgumentOutOfRangeException(TableLayoutPanelCellBorderStyle value)
        {
            var panel = new TableLayoutPanel();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => panel.CellBorderStyle = value);
        }

        [Fact]
        public void TableLayoutPanel_LayoutSettings_SetEmptyStub_Success()
        {
            var converter = new TableLayoutSettingsTypeConverter();
            var panel = new TableLayoutPanel();

            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
            panel.LayoutSettings = settings;
            Assert.Empty(panel.LayoutSettings.ColumnStyles);
            Assert.Empty(panel.LayoutSettings.RowStyles);
        }

        [Fact]
        public void TableLayoutPanel_LayoutSettings_SetStubWithoutControls_Success()
        {
            var converter = new TableLayoutSettingsTypeConverter();
            var panel = new TableLayoutPanel();

            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
            var columnStyle = new ColumnStyle(SizeType.Percent, 1);
            var rowStyle = new RowStyle(SizeType.Percent, 1);

            settings.ColumnStyles.Add(columnStyle);
            settings.RowStyles.Add(rowStyle);

            panel.LayoutSettings = settings;
            Assert.Equal(columnStyle, Assert.Single(panel.LayoutSettings.ColumnStyles));
            Assert.Equal(rowStyle, Assert.Single(panel.LayoutSettings.RowStyles));
        }

        [Fact]
        public void TableLayoutPanel_LayoutSettings_SetStubWithControls_Success()
        {
            var converter = new TableLayoutSettingsTypeConverter();
            var panel = new TableLayoutPanel();

            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
            var columnStyle = new ColumnStyle(SizeType.Percent, 1);
            var rowStyle = new RowStyle(SizeType.Percent, 1);

            settings.SetColumnSpan("name", 1);
            settings.SetRowSpan("name", 2);
            settings.SetColumn("name", 3);
            settings.SetRow("name", 4);
            settings.SetColumnSpan("noSuchName", 5);
            settings.SetRowSpan("noSuchName", 6);
            settings.SetColumn("noSuchName", 7);
            settings.SetRow("noSuchName", 8);
            settings.ColumnStyles.Add(columnStyle);
            settings.RowStyles.Add(rowStyle);

            var controlWithName = new ScrollableControl { Name = "name" };
            var controlWithDefaultName = new ScrollableControl();
            var controlWithoutName = new ControlWithoutName();
            panel.Controls.Add(controlWithName);
            panel.Controls.Add(controlWithDefaultName);
            panel.Controls.Add(controlWithoutName);

            panel.LayoutSettings = settings;
            Assert.Equal(columnStyle, Assert.Single(panel.LayoutSettings.ColumnStyles));
            Assert.Equal(rowStyle, Assert.Single(panel.LayoutSettings.RowStyles));

            Assert.Equal(1, panel.LayoutSettings.GetColumnSpan(controlWithName));
            Assert.Equal(2, panel.LayoutSettings.GetRowSpan(controlWithName));
            Assert.Equal(3, panel.LayoutSettings.GetColumn(controlWithName));
            Assert.Equal(4, panel.LayoutSettings.GetRow(controlWithName));
        }

        [Fact]
        public void TableLayoutPanel_LayoutSettings_SetStub_ResetsOriginalSettings()
        {
            var converter = new TableLayoutSettingsTypeConverter();
            var panel = new TableLayoutPanel();

            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
            settings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1));
            settings.RowStyles.Add(new RowStyle(SizeType.Percent, 1));

            panel.LayoutSettings = settings;
            Assert.Empty(settings.ColumnStyles);
            Assert.Empty(settings.ColumnStyles);
        }

        [Fact]
        public void TableLayoutPanel_LayoutSettings_SetNull_ThrowsNNotSupportedException()
        {
            var panel = new TableLayoutPanel();
            Assert.Throws<NotSupportedException>(() => panel.LayoutSettings = null);
        }

        [Fact]
        public void TableLayoutPanel_LayoutSettings_SetNonStub_ThrowsNNotSupportedException()
        {
            var other = new TableLayoutPanel();

            var panel = new TableLayoutPanel();
            Assert.Throws<NotSupportedException>(() => panel.LayoutSettings = other.LayoutSettings);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void TableLayoutPanel_ColumnCount_Set_GetReturnsExpected(int value)
        {
            var panel = new TableLayoutPanel
            {
                ColumnCount = value
            };
            Assert.Equal(value, panel.ColumnCount);
        }

        [Fact]
        public void TableLayoutPanel_ColumnCount_SetNegative_ThrowsArgumentOutOfRangeException()
        {
            var panel = new TableLayoutPanel();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => panel.ColumnCount = -1);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void TableLayoutPanel_RowCount_Set_GetReturnsExpected(int value)
        {
            var panel = new TableLayoutPanel
            {
                RowCount = value
            };
            Assert.Equal(value, panel.RowCount);
        }

        [Fact]
        public void TableLayoutPanel_RowCount_SetNegative_ThrowsArgumentOutOfRangeException()
        {
            var panel = new TableLayoutPanel();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => panel.RowCount = -1);
        }

        [Theory]
        [InlineData(TableLayoutPanelGrowStyle.FixedSize)]
        [InlineData(TableLayoutPanelGrowStyle.AddRows)]
        [InlineData(TableLayoutPanelGrowStyle.AddColumns)]
        public void TableLayoutPanel_GrowStyle_Set_GetReturnsExpected(TableLayoutPanelGrowStyle value)
        {
            var panel = new TableLayoutPanel
            {
                GrowStyle = value
            };
            Assert.Equal(value, panel.GrowStyle);
        }

        [Theory]
        [InlineData((TableLayoutPanelGrowStyle)(TableLayoutPanelGrowStyle.FixedSize - 1))]
        [InlineData((TableLayoutPanelGrowStyle)(TableLayoutPanelGrowStyle.AddColumns + 1))]
        public void TableLayoutPanel_GrowStyle_SetNegative_ThrowsArgumentOutOfRangeException(TableLayoutPanelGrowStyle value)
        {
            var panel = new TableLayoutPanel();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => panel.GrowStyle = value);
        }

        [Fact]
        public void TableLayoutPanel_GetColumnSpan_NoSuchControl_ReturnsExpected()
        {
            var control = new ScrollableControl();
            var panel = new TableLayoutPanel();
            Assert.Equal(1, panel.GetColumnSpan(control));
        }

        [Fact]
        public void TableLayoutPanel_GetColumnSpan_NullControl_ThrowsArgumentNullException()
        {
            var panel = new TableLayoutPanel();
            Assert.Throws<ArgumentNullException>("control", () => panel.GetColumnSpan(null));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void TableLayoutPanel_SetColumnSpan_ValidControl_GetReturnsExpected(int value)
        {
            var control = new ScrollableControl();
            var panel = new TableLayoutPanel();
            panel.SetColumnSpan(control, value);
            Assert.Equal(value, panel.GetColumnSpan(control));
        }

        [Fact]
        public void TableLayoutPanel_SetColumnSpan_MultipleTimes_GetReturnsExpected()
        {
            var control = new ScrollableControl();
            var panel = new TableLayoutPanel();
            panel.SetColumnSpan(control, 1);
            Assert.Equal(1, panel.GetColumnSpan(control));

            panel.SetColumnSpan(control, 2);
            Assert.Equal(2, panel.GetColumnSpan(control));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void TableLayoutPanel_SetColumnSpan_NullControl_ThrowsArgumentNullException(int value)
        {
            var panel = new TableLayoutPanel();
            Assert.Throws<ArgumentNullException>("control", () => panel.SetColumnSpan(null, value));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void TableLayoutPanel_SetColumnSpan_InvalidValue_ThrowsArgumentOutOfRangeException(int value)
        {
            var panel = new TableLayoutPanel();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => panel.SetColumnSpan(new Control(), value));
        }

        [Fact]
        public void TableLayoutPanel_GetRowSpan_NoSuchControl_ReturnsExpected()
        {
            var control = new ScrollableControl();
            var panel = new TableLayoutPanel();
            Assert.Equal(1, panel.GetRowSpan(control));
        }

        [Fact]
        public void TableLayoutPanel_GetRowSpan_NullControl_ThrowsArgumentNullException()
        {
            var panel = new TableLayoutPanel();
            Assert.Throws<ArgumentNullException>("control", () => panel.GetRowSpan(null));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(int.MaxValue)]
        public void TableLayoutPanel_SetRowSpan_ValidControl_GetReturnsExpected(int value)
        {
            var control = new ScrollableControl();
            var panel = new TableLayoutPanel();
            panel.SetRowSpan(control, value);
            Assert.Equal(value, panel.GetRowSpan(control));
        }

        [Fact]
        public void TableLayoutPanel_SetRowSpan_MultipleTimes_GetReturnsExpected()
        {
            var control = new ScrollableControl();
            var panel = new TableLayoutPanel();
            panel.SetRowSpan(control, 1);
            Assert.Equal(1, panel.GetRowSpan(control));

            panel.SetRowSpan(control, 2);
            Assert.Equal(2, panel.GetRowSpan(control));
        }

        [Fact]
        public void TableLayoutPanel_SetRowSpan_NullControl_ThrowsArgumentNullException()
        {
            var panel = new TableLayoutPanel();
            Assert.Throws<ArgumentNullException>("control", () => panel.SetRowSpan(null, 1));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void TableLayoutPanel_SetRowSpan_InvalidValue_ThrowsArgumentOutOfRangeException(int value)
        {
            var panel = new TableLayoutPanel();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => panel.SetRowSpan(new Control(), value));
        }

        [Fact]
        public void TableLayoutPanel_GetRow_NoSuchControl_ReturnsExpected()
        {
            var control = new ScrollableControl();
            var panel = new TableLayoutPanel();
            Assert.Equal(-1, panel.GetRow(control));
        }

        [Fact]
        public void TableLayoutPanel_GetRow_NullControl_ThrowsArgumentNullException()
        {
            var panel = new TableLayoutPanel();
            Assert.Throws<ArgumentNullException>("control", () => panel.GetRow(null));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(int.MaxValue)]
        public void TableLayoutPanel_SetRow_ValidControl_GetReturnsExpected(int value)
        {
            var control = new ScrollableControl();
            var panel = new TableLayoutPanel();
            panel.SetRow(control, value);
            Assert.Equal(value, panel.GetRow(control));
        }

        [Fact]
        public void TableLayoutPanel_SetRow_MultipleTimes_GetReturnsExpected()
        {
            var control = new ScrollableControl();
            var panel = new TableLayoutPanel();
            panel.SetRow(control, 1);
            Assert.Equal(1, panel.GetRow(control));

            panel.SetRow(control, 2);
            Assert.Equal(2, panel.GetRow(control));
        }

        [Fact]
        public void TableLayoutPanel_SetRow_NullControl_ThrowsArgumentNullException()
        {
            var panel = new TableLayoutPanel();
            Assert.Throws<ArgumentNullException>("control", () => panel.SetRow(null, 1));
        }

        [Fact]
        public void TableLayoutPanel_SetRow_InvalidRow_ThrowsArgumentOutOfRangeException()
        {
            var panel = new TableLayoutPanel();
            Assert.Throws<ArgumentOutOfRangeException>("row", () => panel.SetRow(new Control(), -2));
        }

        [Fact]
        public void TableLayoutPanel_GetCellPosition_NoSuchControl_ReturnsExpected()
        {
            var control = new ScrollableControl();
            var panel = new TableLayoutPanel();
            Assert.Equal(new TableLayoutPanelCellPosition(-1, -1), panel.GetCellPosition(control));
        }

        [Fact]
        public void TableLayoutPanel_GetCellPosition_NullControl_ThrowsArgumentNullException()
        {
            var panel = new TableLayoutPanel();
            Assert.Throws<ArgumentNullException>("control", () => panel.GetCellPosition(null));
        }

        [Fact]
        public void TableLayoutPanel_GetCellPosition_InvalidColumn_ThrowsArgumentOutOfRangeException()
        {
            var control = new ScrollableControl();
            var panel = new TableLayoutPanel();
            panel.SetCellPosition(control, new TableLayoutPanelCellPosition { Column = -2 });
            Assert.Throws<ArgumentOutOfRangeException>("column", () => panel.GetCellPosition(control));
        }

        [Fact]
        public void TableLayoutPanel_GetCellPosition_InvalidRow_ThrowsArgumentOutOfRangeException()
        {
            var control = new ScrollableControl();
            var panel = new TableLayoutPanel();
            panel.SetCellPosition(control, new TableLayoutPanelCellPosition { Row = -2 });
            Assert.Throws<ArgumentOutOfRangeException>("row", () => panel.GetCellPosition(control));
        }

        public static IEnumerable<object[]> SetCellPosition_TestData()
        {
            yield return new object[] { new TableLayoutPanelCellPosition(-1, -1) };
            yield return new object[] { new TableLayoutPanelCellPosition(0, 0) };
            yield return new object[] { new TableLayoutPanelCellPosition(1, 2) };
        }

        [Theory]
        [MemberData(nameof(SetCellPosition_TestData))]
        public void TableLayoutPanel_SetCellPosition_ValidControl_GetReturnsExpected(TableLayoutPanelCellPosition value)
        {
            var control = new ScrollableControl();
            var panel = new TableLayoutPanel();
            panel.SetCellPosition(control, value);
            Assert.Equal(value, panel.GetCellPosition(control));
        }

        [Fact]
        public void TableLayoutPanel_SetCellPosition_MultipleTimes_GetReturnsExpected()
        {
            var control = new ScrollableControl();
            var panel = new TableLayoutPanel();
            panel.SetCellPosition(control, new TableLayoutPanelCellPosition(1, 1));
            Assert.Equal(new TableLayoutPanelCellPosition(1, 1), panel.GetCellPosition(control));

            panel.SetCellPosition(control, new TableLayoutPanelCellPosition(2, 2));
            Assert.Equal(new TableLayoutPanelCellPosition(2, 2), panel.GetCellPosition(control));
        }

        [Fact]
        public void TableLayoutPanel_SetCellPosition_NullControl_ThrowsArgumentNullException()
        {
            var panel = new TableLayoutPanel();
            Assert.Throws<ArgumentNullException>("control", () => panel.SetCellPosition(null, new TableLayoutPanelCellPosition()));
        }

        [Fact]
        public void TableLayoutPanel_GetColumn_NoSuchControl_ReturnsExpected()
        {
            var control = new ScrollableControl();
            var panel = new TableLayoutPanel();
            Assert.Equal(-1, panel.GetColumn(control));
        }

        [Fact]
        public void TableLayoutPanel_GetColumn_NullControl_ThrowsArgumentNullException()
        {
            var panel = new TableLayoutPanel();
            Assert.Throws<ArgumentNullException>("control", () => panel.GetColumn(null));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(int.MaxValue)]
        public void TableLayoutPanel_SetColumn_ValidControl_GetReturnsExpected(int value)
        {
            var control = new ScrollableControl();
            var panel = new TableLayoutPanel();
            panel.SetColumn(control, value);
            Assert.Equal(value, panel.GetColumn(control));
        }

        [Fact]
        public void TableLayoutPanel_SetColumn_MultipleTimes_GetReturnsExpected()
        {
            var control = new ScrollableControl();
            var panel = new TableLayoutPanel();
            panel.SetColumn(control, 1);
            Assert.Equal(1, panel.GetColumn(control));

            panel.SetColumn(control, 2);
            Assert.Equal(2, panel.GetColumn(control));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void TableLayoutPanel_SetColumn_NullControl_ThrowsArgumentNullException(int value)
        {
            var panel = new TableLayoutPanel();
            Assert.Throws<ArgumentNullException>("control", () => panel.SetColumn(null, value));
        }

        [Fact]
        public void TableLayoutPanel_SetColumn_InvalidColumn_ThrowsArgumentOutOfRangeException()
        {
            var panel = new TableLayoutPanel();
            Assert.Throws<ArgumentOutOfRangeException>("column", () => panel.SetColumn(new Control(), -2));
        }

        [Theory]
        [InlineData(1, 1, 10, 11)]
        [InlineData(2, 2, 11, 12)]
        [InlineData(4, 4, 10, 11)]
        public void TableLayoutPanel_GetControlFromPosition_ControlExistsAddedToContainer_ReturnsExpected(int columnSpan, int rowSpan, int column, int row)
        {
            var control = new ScrollableControl
            {
                Visible = true
            };
            var panel = new TableLayoutPanel();
            panel.Controls.Add(control);
            panel.SetColumnSpan(control, columnSpan);
            panel.SetRowSpan(control, rowSpan);
            panel.SetColumn(control, 10);
            panel.SetRow(control, 11);

            Assert.Equal(control, panel.GetControlFromPosition(column, row));
        }

        [Theory]
        [InlineData(1, 1, 9, 11)]
        [InlineData(1, 1, 10, 10)]
        [InlineData(1, 1, 11, 11)]
        [InlineData(1, 1, 10, 12)]
        [InlineData(2, 2, 8, 9)]
        [InlineData(2, 2, 9, 10)]
        [InlineData(4, 4, 9, 10)]
        public void TableLayoutPanel_GetControlFromPosition_OutOfRange_ReturnsNull(int columnSpan, int rowSpan, int column, int row)
        {
            var control = new ScrollableControl
            {
                Visible = true
            };
            var panel = new TableLayoutPanel();
            panel.Controls.Add(control);
            panel.SetColumnSpan(control, columnSpan);
            panel.SetRowSpan(control, rowSpan);
            panel.SetColumn(control, 10);
            panel.SetRow(control, 11);

            Assert.Null(panel.GetControlFromPosition(column, row));
        }

        [Fact]
        public void TableLayoutPanel_GetControlFromPosition_ControlExistsNotVisible_ReturnsNull()
        {
            var control = new ScrollableControl();
            var panel = new TableLayoutPanel();
            panel.Controls.Add(control);
            panel.SetColumn(control, 10);
            panel.SetRow(control, 11);

            Assert.Equal(control, panel.GetControlFromPosition(10, 11));
        }

        [Fact]
        public void TableLayoutPanel_GetControlFromPosition_ControlExistsButNotAddedToContainer_ReturnsNull()
        {
            var control = new ScrollableControl
            {
                Visible = true
            };
            var panel = new TableLayoutPanel();
            panel.SetColumn(control, 10);
            panel.SetRow(control, 11);

            Assert.Null(panel.GetControlFromPosition(10, 11));
        }

        [Fact]
        public void TableLayoutPanel_GetControlFromPosition_NoSuchControl_ReturnsExpected()
        {
            var panel = new TableLayoutPanel();
            Assert.Null(panel.GetControlFromPosition(0, 0));
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(-1)]
        public void TableLayoutPanel_GetControlFromPosition_NegativeColumn_ThrowsArgumentOutOfRangeException(int column)
        {
            var panel = new TableLayoutPanel();
            Assert.Throws<ArgumentOutOfRangeException>("column", () => panel.GetControlFromPosition(column, 0));
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(-1)]
        public void TableLayoutPanel_GetControlFromPosition_NegativeRow_ThrowsArgumentOutOfRangeException(int row)
        {
            var panel = new TableLayoutPanel();
            Assert.Throws<ArgumentOutOfRangeException>("row", () => panel.GetControlFromPosition(0, row));
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 3)]
        public void TableLayoutPanel_GetPositionFromControl_ControlExists_ReturnsExpected(int columnSpan, int rowSpan)
        {
            var control = new ScrollableControl
            {
                Visible = true
            };
            var panel = new TableLayoutPanel();
            panel.Controls.Add(control);
            panel.SetColumn(control, 1);
            panel.SetRow(control, 2);
            panel.SetColumnSpan(control, columnSpan);
            panel.SetRowSpan(control, rowSpan);

            Assert.Equal(new TableLayoutPanelCellPosition(1, 2), panel.GetPositionFromControl(control));
        }

        [Fact]
        public void TableLayoutPanel_GetPositionFromControl_NotVisibleSuchControl_ReturnsExpected()
        {
            var control = new ScrollableControl();
            var panel = new TableLayoutPanel();
            panel.Controls.Add(control);
            panel.SetColumn(control, 1);
            panel.SetRow(control, 2);

            Assert.Equal(new TableLayoutPanelCellPosition(1, 2), panel.GetPositionFromControl(control));
        }

        [Fact]
        public void TableLayoutPanel_GetPositionFromControl_NoSuchControl_ReturnsExpected()
        {
            var control = new ScrollableControl
            {
                Visible = true
            };
            var panel = new TableLayoutPanel();
            panel.SetColumn(control, 1);
            panel.SetRow(control, 2);

            Assert.Equal(new TableLayoutPanelCellPosition(-1, -1), panel.GetPositionFromControl(control));
        }

        [Fact]
        public void TableLayoutPanel_GetPositionFromControl_NullControl_ReturnsExpected()
        {
            var panel = new TableLayoutPanel();
            Assert.Equal(new TableLayoutPanelCellPosition(-1, -1), panel.GetPositionFromControl(null));
        }

        [Fact]
        public void TableLayoutPanel_GetColumnWidths_NoColumns_ReturnsExpectd()
        {
            var panel = new TableLayoutPanel();
            Assert.Empty(panel.GetColumnWidths());
        }

        [Theory]
        [InlineData(TableLayoutPanelGrowStyle.FixedSize)]
        [InlineData(TableLayoutPanelGrowStyle.AddRows)]
        [InlineData(TableLayoutPanelGrowStyle.AddColumns)]
        public void TableLayoutPanel_GetColumnWidths_InvokeNoChildren_ReturnsExpected(TableLayoutPanelGrowStyle growStyle)
        {
            var control = new Button
            {
                Visible = true
            };
            var panel = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 2,
                GrowStyle = growStyle
            };
            Assert.Equal(new int[] { 0, 0, 200 }, panel.GetColumnWidths());
        }

        [Fact]
        public void TableLayoutPanel_GetRowHeights_NoRows_ReturnsExpectd()
        {
            var panel = new TableLayoutPanel();
            Assert.Empty(panel.GetRowHeights());
        }

        [Theory]
        [InlineData(TableLayoutPanelGrowStyle.FixedSize)]
        [InlineData(TableLayoutPanelGrowStyle.AddRows)]
        [InlineData(TableLayoutPanelGrowStyle.AddColumns)]
        public void TableLayoutPanel_GetRowHeights_InvokeNoChildren_ReturnsExpected(TableLayoutPanelGrowStyle growStyle)
        {
            var panel = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 2,
                GrowStyle = growStyle
            };
            Assert.Equal(new int[] { 0, 100 }, panel.GetRowHeights());
        }

        [Theory]
        [InlineData(TableLayoutPanelGrowStyle.FixedSize)]
        [InlineData(TableLayoutPanelGrowStyle.AddRows)]
        [InlineData(TableLayoutPanelGrowStyle.AddColumns)]
        public void TableLayoutPanel_GetRowHeights_CustomAutoSizeStyles_ReturnsExpected(TableLayoutPanelGrowStyle growStyle)
        {
            var panel = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 2,
                GrowStyle = growStyle,
                ClientSize = new Size(70, 80)
            };
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize, 20));
            Assert.Equal(new int[] { 0, 80 }, panel.GetRowHeights());

            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize, 30));
            Assert.Equal(new int[] { 0, 80 }, panel.GetRowHeights());

            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize, 6));
            Assert.Equal(new int[] { 0, 80 }, panel.GetRowHeights());
        }

        [Theory]
        [InlineData(TableLayoutPanelGrowStyle.FixedSize)]
        [InlineData(TableLayoutPanelGrowStyle.AddRows)]
        [InlineData(TableLayoutPanelGrowStyle.AddColumns)]
        public void TableLayoutPanel_GetRowHeights_CustomAbsoluteStyles_ReturnsExpected(TableLayoutPanelGrowStyle growStyle)
        {
            var panel = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 2,
                GrowStyle = growStyle
            };
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));
            Assert.Equal(new int[] { 10, 90 }, panel.GetRowHeights());

            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));
            Assert.Equal(new int[] { 10, 90 }, panel.GetRowHeights());

            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 6));
            Assert.Equal(new int[] { 10, 90 }, panel.GetRowHeights());
        }

        [Theory]
        [InlineData(TableLayoutPanelGrowStyle.FixedSize)]
        [InlineData(TableLayoutPanelGrowStyle.AddRows)]
        [InlineData(TableLayoutPanelGrowStyle.AddColumns)]
        public void TableLayoutPanel_GetRowHeights_CustomPercentStyles_ReturnsExpected(TableLayoutPanelGrowStyle growStyle)
        {
            var panel = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 2,
                GrowStyle = growStyle,
                ClientSize = new Size(70, 80)
            };
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
            Assert.Equal(new int[] { 80, 0 }, panel.GetRowHeights());

            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 30));
            Assert.Equal(new int[] { 32, 48 }, panel.GetRowHeights());

            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 6));
            Assert.Equal(new int[] { 32, 48 }, panel.GetRowHeights());
        }

        [Theory]
        [InlineData(TableLayoutPanelGrowStyle.FixedSize)]
        [InlineData(TableLayoutPanelGrowStyle.AddRows)]
        [InlineData(TableLayoutPanelGrowStyle.AddColumns)]
        public void TableLayoutPanel_GetRowHeights_CustomStylesPercentOverflow_ReturnsExpected(TableLayoutPanelGrowStyle growStyle)
        {
            var panel = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 2,
                GrowStyle = growStyle,
                ClientSize = new Size(70, 80)
            };
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 75));
            Assert.Equal(new int[] { 80, 0 }, panel.GetRowHeights());

            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
            Assert.Equal(new int[] { -20, 100 }, panel.GetRowHeights());
        }

        [Theory]
        [InlineData(TableLayoutPanelGrowStyle.FixedSize)]
        [InlineData(TableLayoutPanelGrowStyle.AddRows)]
        [InlineData(TableLayoutPanelGrowStyle.AddColumns)]
        public void TableLayoutPanel_GetRowHeights_CustomStylesPercentAutoSize_ReturnsExpected(TableLayoutPanelGrowStyle growStyle)
        {
            var panel = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 2,
                GrowStyle = growStyle,
                ClientSize = new Size(70, 80)
            };
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 75));
            Assert.Equal(new int[] { 80, 0 }, panel.GetRowHeights());

            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize, 100));
            Assert.Equal(new int[] { 80, 0 }, panel.GetRowHeights());
        }

        [Theory]
        [InlineData(TableLayoutPanelGrowStyle.FixedSize)]
        [InlineData(TableLayoutPanelGrowStyle.AddRows)]
        [InlineData(TableLayoutPanelGrowStyle.AddColumns)]
        public void TableLayoutPanel_GetRowHeights_CustomStylesAbsoluteAutoSize_ReturnsExpected(TableLayoutPanelGrowStyle growStyle)
        {
            var panel = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 2,
                GrowStyle = growStyle,
                ClientSize = new Size(70, 80)
            };
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            Assert.Equal(new int[] { 50, 30 }, panel.GetRowHeights());

            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize, 100));
            Assert.Equal(new int[] { 50, 30 }, panel.GetRowHeights());
        }

        [Theory]
        [InlineData(TableLayoutPanelGrowStyle.FixedSize)]
        [InlineData(TableLayoutPanelGrowStyle.AddRows)]
        [InlineData(TableLayoutPanelGrowStyle.AddColumns)]
        public void TableLayoutPanel_GetRowHeights_CustomStylesAbsoluteOverflow_ReturnsExpected(TableLayoutPanelGrowStyle growStyle)
        {
            var panel = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 2,
                GrowStyle = growStyle,
                ClientSize = new Size(70, 80)
            };
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
            Assert.Equal(new int[] { 100, 0 }, panel.GetRowHeights());

            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
            Assert.Equal(new int[] { 100, 100 }, panel.GetRowHeights());
        }

        [Theory]
        [InlineData(TableLayoutPanelGrowStyle.FixedSize)]
        [InlineData(TableLayoutPanelGrowStyle.AddRows)]
        [InlineData(TableLayoutPanelGrowStyle.AddColumns)]
        public void TableLayoutPanel_GetRowHeights_CustomInvalidStyles_ReturnsExpected(TableLayoutPanelGrowStyle growStyle)
        {
            var panel = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 2,
                GrowStyle = growStyle,
                ClientSize = new Size(50, 50)
            };
            panel.RowStyles.Add(new RowStyle((SizeType)(SizeType.AutoSize - 1), 22));
            Assert.Equal(new int[] { 0, 50 }, panel.GetRowHeights());

            panel.RowStyles.Add(new RowStyle((SizeType)(SizeType.AutoSize - 1), 22));
            Assert.Equal(new int[] { 0, 50 }, panel.GetRowHeights());

            panel.RowStyles.Add(new RowStyle((SizeType)(SizeType.AutoSize - 1), 6));
            Assert.Equal(new int[] { 0, 50 }, panel.GetRowHeights());
        }

        [Fact]
        public void TableLayoutPanel_OnCellPaint_Invoke_Success()
        {
            var panel = new SubTableLayoutPanel();

            // No handler.
            panel.OnCellPaintEntry(null);

            // Handler.
            int callCount = 0;
            TableLayoutCellPaintEventHandler handler = (sender, e) =>
            {
                Assert.Equal(panel, sender);
                callCount++;
            };

            panel.CellPaint += handler;
            panel.OnCellPaintEntry(null);
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            panel.CellPaint -= handler;
            panel.OnCellPaintEntry(null);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void TableLayoutPanel_CanExtend_Control_ReturnsExpected()
        {
            var control = new ScrollableControl();
            var panel = new TableLayoutPanel();
            IExtenderProvider provider = panel;

            Assert.False(provider.CanExtend(control));
            panel.Controls.Add(control);
            Assert.True(provider.CanExtend(control));
        }

        [Theory]
        [InlineData("obj")]
        [InlineData(null)]
        public void TableLayoutPanel_CanExtend_NotControl_ReturnsFalse(object obj)
        {
            var panel = new TableLayoutPanel();
            IExtenderProvider provider = panel;
            Assert.False(provider.CanExtend(obj));
        }

        [TypeDescriptionProvider(typeof(CustomTypeDescriptionProvider))]
        private class ControlWithoutName : Control
        {
        }

        private class CustomTypeDescriptionProvider : TypeDescriptionProvider
        {
            public CustomTypeDescriptionProvider()
            {
            }

            public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
            {
                var mockDescriptor = new Mock<ICustomTypeDescriptor>(MockBehavior.Strict);
                mockDescriptor
                    .Setup(c => c.GetProperties())
                    .Returns(new PropertyDescriptorCollection(Array.Empty<PropertyDescriptor>()));
                return mockDescriptor.Object;
            }
        }

        private class SubTableLayoutPanel : TableLayoutPanel
        {
            public void OnCellPaintEntry(TableLayoutCellPaintEventArgs e) => OnCellPaint(e);
        }
    }
}
