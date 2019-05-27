// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Xunit;

namespace System.Windows.Forms.Layout.Tests
{
    public class TableLayoutSettingsTests
    {
        [Fact]
        public void TableLayoutSettings_Properties_GetWithOwner_ReturnsExpected()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            Assert.NotNull(settings.LayoutEngine);
            // Make sure the layout engine is cached correctly.
            Assert.Same(settings.LayoutEngine, settings.LayoutEngine);
            Assert.Equal(0, settings.ColumnCount);
            Assert.Equal(0, settings.RowCount);
            Assert.Empty(settings.ColumnStyles);
            Assert.Empty(settings.RowStyles);
            Assert.Equal(TableLayoutPanelGrowStyle.AddRows, settings.GrowStyle);
        }

        [Fact]
        public void TableLayoutSettings_Properties_GetWithoutOwner_ReturnsExpected()
        {
            var converter = new TableLayoutSettingsTypeConverter();
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
            Assert.NotNull(settings.LayoutEngine);
            // Make sure the layout engine is cached correctly.
            Assert.Same(settings.LayoutEngine, settings.LayoutEngine);
            Assert.Throws<NullReferenceException>(() => settings.ColumnCount);
            Assert.Throws<NullReferenceException>(() => settings.RowCount);
            Assert.Empty(settings.ColumnStyles);
            Assert.Empty(settings.RowStyles);
            Assert.Throws<NullReferenceException>(() => settings.GrowStyle);
        }

        public static IEnumerable<TableLayoutSettings> EmptySettings()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            yield return (TableLayoutSettings)toolStrip.LayoutSettings;

            var converter = new TableLayoutSettingsTypeConverter();
            yield return Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        }

        public static IEnumerable<object[]> EmptySettings_TestData()
        {
            foreach (TableLayoutSettings settings in EmptySettings())
            {
                yield return new object[] { settings };
            }
        }

        [Theory]
        [MemberData(nameof(EmptySettings_TestData))]
        public void TableLayoutSettings_ColumnStyles_Get_ReturnsSameInstance(TableLayoutSettings settings)
        {
            // Make sure the column styles are cached correctly.
            Assert.Same(settings.ColumnStyles, settings.ColumnStyles);
        }

        [Theory]
        [MemberData(nameof(EmptySettings_TestData))]
        public void TableLayoutSettings_RowStyles_Get_ReturnsSameInstance(TableLayoutSettings settings)
        {
            // Make sure the row styles are cached correctly.
            Assert.Same(settings.RowStyles, settings.RowStyles);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void TableLayoutSettings_ColumnCount_Set_GetReturnsExpected(int value)
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            settings.ColumnCount = value;
            Assert.Equal(value, settings.ColumnCount);
        }

        [Fact]
        public void TableLayoutSettings_ColumnCount_SetNegative_ThrowsArgumentOutOfRangeException()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            Assert.Throws<ArgumentOutOfRangeException>("value", () => settings.ColumnCount = -1);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void TableLayoutSettings_RowCount_Set_GetReturnsExpected(int value)
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            settings.RowCount = value;
            Assert.Equal(value, settings.RowCount);
        }

        [Fact]
        public void TableLayoutSettings_RowCount_SetNegative_ThrowsArgumentOutOfRangeException()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            Assert.Throws<ArgumentOutOfRangeException>("value", () => settings.RowCount = -1);
        }

        [Theory]
        [InlineData(TableLayoutPanelGrowStyle.FixedSize)]
        [InlineData(TableLayoutPanelGrowStyle.AddRows)]
        [InlineData(TableLayoutPanelGrowStyle.AddColumns)]
        public void TableLayoutSettings_GrowStyle_Set_GetReturnsExpected(TableLayoutPanelGrowStyle value)
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            settings.GrowStyle = value;
            Assert.Equal(value, settings.GrowStyle);
        }

        [Theory]
        [InlineData((TableLayoutPanelGrowStyle)(TableLayoutPanelGrowStyle.FixedSize - 1))]
        [InlineData((TableLayoutPanelGrowStyle)(TableLayoutPanelGrowStyle.AddColumns + 1))]
        public void TableLayoutSettings_GrowStyle_SetNegative_ThrowsArgumentOutOfRangeException(TableLayoutPanelGrowStyle value)
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            Assert.Throws<ArgumentOutOfRangeException>("value", () => settings.GrowStyle = value);
        }

        [Theory]
        [MemberData(nameof(EmptySettings_TestData))]
        public void TableLayoutSettings_GetColumnSpan_NoSuchControl_ReturnsExpected(TableLayoutSettings settings)
        {
            var control = new ScrollableControl();
            Assert.Equal(1, settings.GetColumnSpan(control));
        }

        [Theory]
        [MemberData(nameof(EmptySettings_TestData))]
        public void TableLayoutSettings_GetColumnSpan_NullControl_ThrowsArgumentNullException(TableLayoutSettings settings)
        {
            Assert.Throws<ArgumentNullException>("control", () => settings.GetColumnSpan(null));
        }

        [Fact]
        public void TableLayoutSettings_GetColumnSpan_InvalidControl_ThrowsNotSupportedException()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            Assert.Throws<NotSupportedException>(() => settings.GetColumnSpan("control"));
        }

        [Fact]
        public void TableLayoutSettings_GetColumnSpan_InvalidControlStub_ReturnsExpected()
        {
            var converter = new TableLayoutSettingsTypeConverter();
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
            Assert.Equal(1, settings.GetColumnSpan("control"));
        }

        public static IEnumerable<object[]> SetColumnSpan_TestData()
        {
            foreach (TableLayoutSettings settings in EmptySettings())
            {
                yield return new object[] { settings, 1 };
                yield return new object[] { settings, 2 };
                yield return new object[] { settings, int.MaxValue };
            }
        }

        [Theory]
        [MemberData(nameof(SetColumnSpan_TestData))]
        public void TableLayoutSettings_SetColumnSpan_ValidControl_GetReturnsExpected(TableLayoutSettings settings, int value)
        {
            var control = new ScrollableControl();
            settings.SetColumnSpan(control, value);
            Assert.Equal(value, settings.GetColumnSpan(control));
        }

        [Theory]
        [MemberData(nameof(EmptySettings_TestData))]
        public void TableLayoutSettings_SetColumnSpan_MultipleTimes_GetReturnsExpected(TableLayoutSettings settings)
        {
            var control = new ScrollableControl();
            settings.SetColumnSpan(control, 1);
            Assert.Equal(1, settings.GetColumnSpan(control));

            settings.SetColumnSpan(control, 2);
            Assert.Equal(2, settings.GetColumnSpan(control));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void TableLayoutSettings_SetColumnSpan_NullControl_ThrowsArgumentNullException(int value)
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            Assert.Throws<ArgumentNullException>("control", () => settings.SetColumnSpan(null, value));
        }

        [Fact]
        public void TableLayoutSettings_SetColumnSpan_NullControlStub_ThrowsArgumentNullException()
        {
            var converter = new TableLayoutSettingsTypeConverter();
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
            Assert.Throws<ArgumentNullException>("control", () => settings.SetColumnSpan(null, 1));
            Assert.Throws<ArgumentNullException>("control", () => settings.GetColumnSpan(null));
        }

        [Fact]
        public void TableLayoutSettings_SetColumnSpan_InvalidControl_ThrowsNotSupportedException()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            Assert.Throws<NotSupportedException>(() => settings.SetColumnSpan("control", 1));
        }

        [Fact]
        public void TableLayoutSettings_SetColumnSpan_InvalidControlStub_GetReturnsExpected()
        {
            var converter = new TableLayoutSettingsTypeConverter();
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
            settings.SetColumnSpan("control", 1);
            Assert.Equal(1, settings.GetColumnSpan("control"));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void TableLayoutSettings_SetColumnSpan_InvalidValue_ThrowsArgumentOutOfRangeException(int value)
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            Assert.Throws<ArgumentOutOfRangeException>("value", () => settings.SetColumnSpan("control", value));
        }

        [Theory]
        [MemberData(nameof(EmptySettings_TestData))]
        public void TableLayoutSettings_GetRowSpan_NoSuchControl_ReturnsExpected(TableLayoutSettings settings)
        {
            var control = new ScrollableControl();
            Assert.Equal(1, settings.GetRowSpan(control));
        }

        [Fact]
        public void TableLayoutSettings_GetRowSpan_NullControl_ThrowsArgumentNullException()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            Assert.Throws<ArgumentNullException>("control", () => settings.GetRowSpan(null));
        }

        [Fact]
        public void TableLayoutSettings_GetRowSpan_NullControlStub_ReturnsExpected()
        {
            var converter = new TableLayoutSettingsTypeConverter();
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
            Assert.Equal(1, settings.GetRowSpan(-1));
        }

        [Fact]
        public void TableLayoutSettings_GetRowSpan_InvalidControl_ThrowsNotSupportedException()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            Assert.Throws<NotSupportedException>(() => settings.GetRowSpan("control"));
        }

        [Fact]
        public void TableLayoutSettings_GetRowSpan_InvalidControlStub_ReturnsExpected()
        {
            var converter = new TableLayoutSettingsTypeConverter();
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
            Assert.Equal(1, settings.GetRowSpan("control"));
        }

        public static IEnumerable<object[]> SetRowSpan_TestData()
        {
            foreach (TableLayoutSettings settings in EmptySettings())
            {
                yield return new object[] { settings, 1 };
                yield return new object[] { settings, 2 };
                yield return new object[] { settings, int.MaxValue };
            }
        }

        [Theory]
        [MemberData(nameof(SetRowSpan_TestData))]
        public void TableLayoutSettings_SetRowSpan_ValidControl_GetReturnsExpected(TableLayoutSettings settings, int value)
        {
            var control = new ScrollableControl();
            settings.SetRowSpan(control, value);
            Assert.Equal(value, settings.GetRowSpan(control));
        }

        [Theory]
        [MemberData(nameof(EmptySettings_TestData))]
        public void TableLayoutSettings_SetRowSpan_MultipleTimes_GetReturnsExpected(TableLayoutSettings settings)
        {
            var control = new ScrollableControl();
            settings.SetRowSpan(control, 1);
            Assert.Equal(1, settings.GetRowSpan(control));

            settings.SetRowSpan(control, 2);
            Assert.Equal(2, settings.GetRowSpan(control));
        }

        [Theory]
        [MemberData(nameof(EmptySettings_TestData))]
        public void TableLayoutSettings_SetRowSpan_NullControl_ThrowsArgumentNullException(TableLayoutSettings settings)
        {
            Assert.Throws<ArgumentNullException>("control", () => settings.SetRowSpan(null, 1));
            Assert.Throws<ArgumentNullException>("control", () => settings.SetRowSpan(null, 0));
        }

        [Fact]
        public void TableLayoutSettings_SetRowSpan_InvalidControl_ThrowsNotSupportedException()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            Assert.Throws<NotSupportedException>(() => settings.SetRowSpan("control", 1));
        }

        [Fact]
        public void TableLayoutSettings_SetRowSpan_InvalidControlStub_GetReturnsExpected()
        {
            var converter = new TableLayoutSettingsTypeConverter();
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
            settings.SetRowSpan("control", 1);
            Assert.Equal(1, settings.GetRowSpan("control"));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void TableLayoutSettings_SetRowSpan_InvalidValue_ThrowsArgumentOutOfRangeException(int value)
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            Assert.Throws<ArgumentOutOfRangeException>("value", () => settings.SetRowSpan("control", value));
        }

        [Theory]
        [MemberData(nameof(EmptySettings_TestData))]
        public void TableLayoutSettings_GetRow_NoSuchControl_ReturnsExpected(TableLayoutSettings settings)
        {
            var control = new ScrollableControl();
            Assert.Equal(-1, settings.GetRow(control));
        }

        [Theory]
        [MemberData(nameof(EmptySettings_TestData))]
        public void TableLayoutSettings_GetRow_NullControl_ThrowsArgumentNullException(TableLayoutSettings settings)
        {
            Assert.Throws<ArgumentNullException>("control", () => settings.GetRow(null));
        }

        [Fact]
        public void TableLayoutSettings_GetRow_InvalidControl_ThrowsNotSupportedException()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            Assert.Throws<NotSupportedException>(() => settings.GetRow("control"));
        }

        [Fact]
        public void TableLayoutSettings_GetRow_InvalidControlStub_ReturnsExpected()
        {
            var converter = new TableLayoutSettingsTypeConverter();
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
            Assert.Equal(-1, settings.GetRow("control"));
        }

        public static IEnumerable<object[]> SetRow_TestData()
        {
            foreach (TableLayoutSettings settings in EmptySettings())
            {
                yield return new object[] { settings, -1 };
                yield return new object[] { settings, 0 };
                yield return new object[] { settings, 1 };
                yield return new object[] { settings, 2 };
                yield return new object[] { settings, int.MaxValue };
            }
        }

        [Theory]
        [MemberData(nameof(SetRow_TestData))]
        public void TableLayoutSettings_SetRow_ValidControl_GetReturnsExpected(TableLayoutSettings settings, int value)
        {
            var control = new ScrollableControl();
            settings.SetRow(control, value);
            Assert.Equal(value, settings.GetRow(control));
        }

        [Theory]
        [MemberData(nameof(EmptySettings_TestData))]
        public void TableLayoutSettings_SetRow_MultipleTimes_GetReturnsExpected(TableLayoutSettings settings)
        {
            var control = new ScrollableControl();
            settings.SetRow(control, 1);
            Assert.Equal(1, settings.GetRow(control));

            settings.SetRow(control, 2);
            Assert.Equal(2, settings.GetRow(control));
        }

        [Theory]
        [MemberData(nameof(EmptySettings_TestData))]
        public void TableLayoutSettings_SetRow_NullControl_ThrowsArgumentNullException(TableLayoutSettings settings)
        {
            Assert.Throws<ArgumentNullException>("control", () => settings.SetRow(null, 1));
        }

        [Fact]
        public void TableLayoutSettings_SetRow_InvalidControl_ThrowsNotSupportedException()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            Assert.Throws<NotSupportedException>(() => settings.SetRow("control", 1));
        }

        [Fact]
        public void TableLayoutSettings_SetRow_InvalidControlStub_GetReturnsExpected()
        {
            var converter = new TableLayoutSettingsTypeConverter();
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
            settings.SetRow("control", 1);
            Assert.Equal(1, settings.GetRow("control"));
        }

        [Theory]
        [MemberData(nameof(EmptySettings_TestData))]
        public void TableLayoutSettings_SetRow_InvalidRow_ThrowsArgumentOutOfRangeException(TableLayoutSettings settings)
        {
            Assert.Throws<ArgumentOutOfRangeException>("row", () => settings.SetRow("control", -2));
        }

        [Theory]
        [MemberData(nameof(EmptySettings_TestData))]
        public void TableLayoutSettings_GetCellPosition_NoSuchControl_ReturnsExpected(TableLayoutSettings settings)
        {
            var control = new ScrollableControl();
            Assert.Equal(new TableLayoutPanelCellPosition(-1, -1), settings.GetCellPosition(control));
        }

        [Theory]
        [MemberData(nameof(EmptySettings_TestData))]
        public void TableLayoutSettings_GetCellPosition_NullControl_ThrowsArgumentNullException(TableLayoutSettings settings)
        {
            Assert.Throws<ArgumentNullException>("control", () => settings.GetCellPosition(null));
        }

        [Fact]
        public void TableLayoutSettings_GetCellPosition_InvalidControl_ThrowsNotSupportedException()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            Assert.Throws<NotSupportedException>(() => settings.GetCellPosition("control"));
        }

        [Fact]
        public void TableLayoutSettings_GetCellPosition_InvalidControlStub_ReturnsExpected()
        {
            var converter = new TableLayoutSettingsTypeConverter();
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
            Assert.Equal(new TableLayoutPanelCellPosition(-1, -1), settings.GetCellPosition("control"));
        }

        [Theory]
        [MemberData(nameof(EmptySettings_TestData))]
        public void TableLayoutSettings_GetCellPosition_InvalidColumn_ThrowsArgumentOutOfRangeException(TableLayoutSettings settings)
        {
            var control = new ScrollableControl();
            settings.SetCellPosition(control, new TableLayoutPanelCellPosition { Column = -2 });
            Assert.Throws<ArgumentOutOfRangeException>("column", () => settings.GetCellPosition(control));
        }

        [Theory]
        [MemberData(nameof(EmptySettings_TestData))]
        public void TableLayoutSettings_GetCellPosition_InvalidRow_ThrowsArgumentOutOfRangeException(TableLayoutSettings settings)
        {
            var control = new ScrollableControl();
            settings.SetCellPosition(control, new TableLayoutPanelCellPosition { Row = -2 });
            Assert.Throws<ArgumentOutOfRangeException>("row", () => settings.GetCellPosition(control));
        }

        public static IEnumerable<object[]> SetCellPosition_TestData()
        {
            foreach (TableLayoutSettings settings in EmptySettings())
            {
                yield return new object[] { settings, new TableLayoutPanelCellPosition(-1, -1) };
                yield return new object[] { settings, new TableLayoutPanelCellPosition(0, 0) };
                yield return new object[] { settings, new TableLayoutPanelCellPosition(1, 2) };
            }
        }

        [Theory]
        [MemberData(nameof(SetCellPosition_TestData))]
        public void TableLayoutSettings_SetCellPosition_ValidControl_GetReturnsExpected(TableLayoutSettings settings, TableLayoutPanelCellPosition value)
        {
            var control = new ScrollableControl();
            settings.SetCellPosition(control, value);
            Assert.Equal(value, settings.GetCellPosition(control));
        }

        [Theory]
        [MemberData(nameof(EmptySettings_TestData))]
        public void TableLayoutSettings_SetCellPosition_MultipleTimes_GetReturnsExpected(TableLayoutSettings settings)
        {
            var control = new ScrollableControl();
            settings.SetCellPosition(control, new TableLayoutPanelCellPosition(1, 1));
            Assert.Equal(new TableLayoutPanelCellPosition(1, 1), settings.GetCellPosition(control));

            settings.SetCellPosition(control, new TableLayoutPanelCellPosition(2, 2));
            Assert.Equal(new TableLayoutPanelCellPosition(2, 2), settings.GetCellPosition(control));
        }

        [Theory]
        [MemberData(nameof(EmptySettings_TestData))]
        public void TableLayoutSettings_SetCellPosition_NullControl_ThrowsArgumentNullException(TableLayoutSettings settings)
        {
            Assert.Throws<ArgumentNullException>("control", () => settings.SetCellPosition(null, new TableLayoutPanelCellPosition()));
        }

        [Fact]
        public void TableLayoutSettings_SetCellPosition_InvalidControl_ThrowsNotSupportedException()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            Assert.Throws<NotSupportedException>(() => settings.SetCellPosition("control", new TableLayoutPanelCellPosition()));
        }

        [Fact]
        public void TableLayoutSettings_SetCellPosition_InvalidControlStub_GetReturnsExpected()
        {
            var converter = new TableLayoutSettingsTypeConverter();
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
            settings.SetCellPosition("control", new TableLayoutPanelCellPosition());
            Assert.Equal(new TableLayoutPanelCellPosition(), settings.GetCellPosition("control"));
        }

        [Theory]
        [MemberData(nameof(EmptySettings_TestData))]
        public void TableLayoutSettings_GetColumn_NoSuchControl_ReturnsExpected(TableLayoutSettings settings)
        {
            var control = new ScrollableControl();
            Assert.Equal(-1, settings.GetColumn(control));
        }

        [Theory]
        [MemberData(nameof(EmptySettings_TestData))]
        public void TableLayoutSettings_GetColumn_NullControl_ThrowsArgumentNullException(TableLayoutSettings settings)
        {
            Assert.Throws<ArgumentNullException>("control", () => settings.GetColumn(null));
        }

        [Fact]
        public void TableLayoutSettings_GetColumn_InvalidControl_ThrowsNotSupportedException()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            Assert.Throws<NotSupportedException>(() => settings.GetColumn("control"));
        }

        [Fact]
        public void TableLayoutSettings_GetColumn_InvalidControlStub_ReturnsExpected()
        {
            var converter = new TableLayoutSettingsTypeConverter();
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
            Assert.Equal(-1, settings.GetColumn("control"));
        }

        public static IEnumerable<object[]> SetColumn_TestData()
        {
            foreach (TableLayoutSettings settings in EmptySettings())
            {
                yield return new object[] { settings, -1 };
                yield return new object[] { settings, 0 };
                yield return new object[] { settings, 1 };
                yield return new object[] { settings, 2 };
                yield return new object[] { settings, int.MaxValue };
            }
        }

        [Theory]
        [MemberData(nameof(SetColumn_TestData))]
        public void TableLayoutSettings_SetColumn_ValidControl_GetReturnsExpected(TableLayoutSettings settings, int value)
        {
            var control = new ScrollableControl();
            settings.SetColumn(control, value);
            Assert.Equal(value, settings.GetColumn(control));
        }

        [Theory]
        [MemberData(nameof(EmptySettings_TestData))]
        public void TableLayoutSettings_SetColumn_MultipleTimes_GetReturnsExpected(TableLayoutSettings settings)
        {
            var control = new ScrollableControl();
            settings.SetColumn(control, 1);
            Assert.Equal(1, settings.GetColumn(control));

            settings.SetColumn(control, 2);
            Assert.Equal(2, settings.GetColumn(control));
        }

        [Fact]
        public void TableLayoutSettings_SetColumn_NullControl_ThrowsArgumentNullException()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            Assert.Throws<ArgumentNullException>("control", () => settings.SetColumn(null, 1));
        }

        [Fact]
        public void TableLayoutSettings_SetColumn_NullControlStub_ThrowsArgumentNullException()
        {
            var converter = new TableLayoutSettingsTypeConverter();
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
            Assert.Throws<ArgumentNullException>("control", () => settings.SetColumn(null, 1));
        }

        [Fact]
        public void TableLayoutSettings_SetColumn_InvalidControl_ThrowsNotSupportedException()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            Assert.Throws<NotSupportedException>(() => settings.SetColumn("control", 1));
        }

        [Fact]
        public void TableLayoutSettings_SetColumn_InvalidControlStub_ThrowsNotSupportedException()
        {
            var converter = new TableLayoutSettingsTypeConverter();
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
            settings.SetColumn("control", 1);
            Assert.Equal(1, settings.GetColumn("control"));
        }

        [Theory]
        [MemberData(nameof(EmptySettings_TestData))]
        public void TableLayoutSettings_SetColumn_InvalidColumn_ThrowsArgumentOutOfRangeException(TableLayoutSettings settings)
        {
            Assert.Throws<ArgumentOutOfRangeException>("column", () => settings.SetColumn("control", -2));
        }

        [Fact]
        public void TableLayoutSettings_Serialize_Deserialize_Success()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            var columnStyle = new ColumnStyle(SizeType.Percent, 1);
            var rowStyle = new RowStyle(SizeType.Percent, 1);

            var controlWithName = new ScrollableControl { Name = "name" };
            settings.SetColumnSpan(controlWithName, 1);
            settings.SetRowSpan(controlWithName, 2);
            settings.SetColumn(controlWithName, 3);
            settings.SetRow(controlWithName, 4);
            settings.ColumnStyles.Add(columnStyle);
            settings.RowStyles.Add(rowStyle);

            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, settings);
                stream.Seek(0, SeekOrigin.Begin);

                TableLayoutSettings result = Assert.IsType<TableLayoutSettings>(formatter.Deserialize(stream));
                Assert.Equal(columnStyle.SizeType, ((ColumnStyle)Assert.Single(result.ColumnStyles)).SizeType);
                Assert.Equal(columnStyle.Width, ((ColumnStyle)Assert.Single(result.ColumnStyles)).Width);
                Assert.Equal(rowStyle.SizeType, ((RowStyle)Assert.Single(result.RowStyles)).SizeType);
                Assert.Equal(rowStyle.Height, ((RowStyle)Assert.Single(result.RowStyles)).Height);

                Assert.Equal(1, result.GetColumnSpan(controlWithName));
                Assert.Equal(1, result.GetRowSpan(controlWithName));
                Assert.Equal(-1, result.GetColumn(controlWithName));
                Assert.Equal(-1, result.GetRow(controlWithName));
            }
        }

        [Theory]
        [InlineData(typeof(NullStringConverter))]
        [InlineData(typeof(EmptyStringConverter))]
        public void TableLayoutSettings_Serialize_InvalidStringConverter_DeserializeThrowsTargetInvocationException(Type type)
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            TypeDescriptor.AddAttributes(settings, new Attribute[] { new TypeConverterAttribute(type) });
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, settings);
                stream.Seek(0, SeekOrigin.Begin);

                Assert.Throws<TargetInvocationException>(() => formatter.Deserialize(stream));
            }
        }

        [Theory]
        [InlineData(typeof(NullTableLayoutSettingsConverter))]
        [InlineData(typeof(NonTableLayoutSettingsConverter))]
        public void TableLayoutSettings_Deserialize_InvalidConverterResult_Success(Type type)
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            TypeDescriptor.AddAttributes(settings, new Attribute[] { new TypeConverterAttribute(type) });
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, settings);
                stream.Seek(0, SeekOrigin.Begin);

                TableLayoutSettings result = Assert.IsType<TableLayoutSettings>(formatter.Deserialize(stream));
                Assert.NotNull(result.LayoutEngine);
                Assert.Same(result.LayoutEngine, result.LayoutEngine);
                Assert.Throws<NullReferenceException>(() => result.ColumnCount);
                Assert.Throws<NullReferenceException>(() => result.RowCount);
                Assert.Empty(result.ColumnStyles);
                Assert.Empty(result.RowStyles);
                Assert.Throws<NullReferenceException>(() => result.GrowStyle);
            }
        }

        private class NullStringConverter : TypeConverter
        {
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                return null;
            }
        }

        private class EmptyStringConverter : TypeConverter
        {
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                return "";
            }
        }

        private class NullTableLayoutSettingsConverter : TableLayoutSettingsTypeConverter
        {
            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                return null;
            }
        }

        private class NonTableLayoutSettingsConverter : TableLayoutSettingsTypeConverter
        {
            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                return "";
            }
        }
    }
}
