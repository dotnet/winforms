// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Moq;

namespace System.Windows.Forms.Layout.Tests;

public class TableLayoutSettingsTests
{
    [WinFormsFact]
    public void TableLayoutSettings_Properties_GetWithOwner_ReturnsExpected()
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        Assert.Equal(0, settings.ColumnCount);
        Assert.Empty(settings.ColumnStyles);
        Assert.Same(settings.ColumnStyles, settings.ColumnStyles);
        Assert.Equal(TableLayoutPanelGrowStyle.AddRows, settings.GrowStyle);
        Assert.NotNull(settings.LayoutEngine);
        Assert.Same(settings.LayoutEngine, settings.LayoutEngine);
        Assert.Equal(0, settings.RowCount);
        Assert.Empty(settings.RowStyles);
        Assert.Same(settings.RowStyles, settings.RowStyles);
    }

    [WinFormsFact]
    public void TableLayoutSettings_Properties_GetWithoutOwner_ReturnsExpected()
    {
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        Assert.Throws<NullReferenceException>(() => settings.GrowStyle);
        Assert.Throws<NullReferenceException>(() => settings.ColumnCount);
        Assert.Empty(settings.ColumnStyles);
        Assert.Same(settings.ColumnStyles, settings.ColumnStyles);
        Assert.NotNull(settings.LayoutEngine);
        Assert.Same(settings.LayoutEngine, settings.LayoutEngine);
        Assert.Throws<NullReferenceException>(() => settings.RowCount);
        Assert.Empty(settings.RowStyles);
        Assert.Same(settings.RowStyles, settings.RowStyles);
    }

    [WinFormsTheory]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    public void TableLayoutSettings_ColumnCount_Set_GetReturnsExpected(int value, int expectedLayoutCallCount)
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Columns", e.AffectedProperty);
            layoutCallCount++;
        };

        settings.ColumnCount = value;
        Assert.Equal(value, settings.ColumnCount);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        settings.ColumnCount = value;
        Assert.Equal(value, settings.ColumnCount);
        Assert.Equal(expectedLayoutCallCount + 1, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TableLayoutSettings_ColumnCount_SetNegative_ThrowsArgumentOutOfRangeException()
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        Assert.Throws<ArgumentOutOfRangeException>("value", () => settings.ColumnCount = -1);
    }

    [WinFormsTheory]
    [InlineData(TableLayoutPanelGrowStyle.FixedSize, 1)]
    [InlineData(TableLayoutPanelGrowStyle.AddRows, 0)]
    [InlineData(TableLayoutPanelGrowStyle.AddColumns, 1)]
    public void TableLayoutSettings_GrowStyle_Set_GetReturnsExpected(TableLayoutPanelGrowStyle value, int expectedLayoutCallCount)
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("GrowStyle", e.AffectedProperty);
            layoutCallCount++;
        };

        settings.GrowStyle = value;
        Assert.Equal(value, settings.GrowStyle);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        settings.GrowStyle = value;
        Assert.Equal(value, settings.GrowStyle);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InvalidEnumData<TableLayoutPanelGrowStyle>]
    public void TableLayoutSettings_GrowStyle_SetInvalid_ThrowsArgumentOutOfRangeException(TableLayoutPanelGrowStyle value)
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        Assert.Throws<InvalidEnumArgumentException>("value", () => settings.GrowStyle = value);
    }

    [WinFormsTheory]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    public void TableLayoutSettings_RowCount_Set_GetReturnsExpected(int value, int expectedLayoutCallCount)
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Rows", e.AffectedProperty);
            layoutCallCount++;
        };

        settings.RowCount = value;
        Assert.Equal(value, settings.RowCount);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        settings.RowCount = value;
        Assert.Equal(value, settings.RowCount);
        Assert.Equal(expectedLayoutCallCount + 1, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TableLayoutSettings_RowCount_SetNegative_ThrowsArgumentOutOfRangeException()
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        Assert.Throws<ArgumentOutOfRangeException>("value", () => settings.RowCount = -1);
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetCellPosition_NoSuchControl_ReturnsExpected()
    {
        using Control child = new();
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        Assert.Equal(new TableLayoutPanelCellPosition(-1, -1), settings.GetCellPosition(child));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetCellPosition_NoSuchControlStub_ReturnsExpected()
    {
        using Control child = new();
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        Assert.Equal(new TableLayoutPanelCellPosition(-1, -1), settings.GetCellPosition(child));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetCellPosition_NullControl_ThrowsArgumentNullException()
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        Assert.Throws<ArgumentNullException>("control", () => settings.GetCellPosition(null));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetCellPosition_NullControlStub_ThrowsArgumentNullException()
    {
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        Assert.Throws<ArgumentNullException>("control", () => settings.GetCellPosition(null));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetCellPosition_InvalidControl_ThrowsNotSupportedException()
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        Assert.Throws<NotSupportedException>(() => settings.GetCellPosition("control"));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetCellPosition_InvalidControlStub_ReturnsExpected()
    {
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        Assert.Equal(new TableLayoutPanelCellPosition(-1, -1), settings.GetCellPosition("control"));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetCellPosition_InvalidColumn_ThrowsArgumentOutOfRangeException()
    {
        using Control child = new();
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        settings.SetCellPosition(child, new TableLayoutPanelCellPosition { Column = -2 });
        Assert.Throws<ArgumentOutOfRangeException>("column", () => settings.GetCellPosition(child));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetCellPosition_InvalidColumnStub_ThrowsArgumentOutOfRangeException()
    {
        using Control child = new();
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        settings.SetCellPosition(child, new TableLayoutPanelCellPosition { Column = -2 });
        Assert.Throws<ArgumentOutOfRangeException>("column", () => settings.GetCellPosition(child));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetCellPosition_InvalidRow_ThrowsArgumentOutOfRangeException()
    {
        using Control child = new();
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        settings.SetCellPosition(child, new TableLayoutPanelCellPosition { Row = -2 });
        Assert.Throws<ArgumentOutOfRangeException>("row", () => settings.GetCellPosition(child));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetCellPosition_InvalidRowStub_ThrowsArgumentOutOfRangeException()
    {
        using Control child = new();
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        settings.SetCellPosition(child, new TableLayoutPanelCellPosition { Row = -2 });
        Assert.Throws<ArgumentOutOfRangeException>("row", () => settings.GetCellPosition(child));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetColumn_NoSuchControl_ReturnsExpected()
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        using Control child = new();
        Assert.Equal(-1, settings.GetColumn(child));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetColumn_NoSuchControlStub_ReturnsExpected()
    {
        using Control child = new();
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        Assert.Equal(-1, settings.GetColumn(child));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetColumn_NullControl_ThrowsArgumentNullException()
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        Assert.Throws<ArgumentNullException>("control", () => settings.GetColumn(null));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetColumn_NullControlStub_ThrowsArgumentNullException()
    {
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        Assert.Throws<ArgumentNullException>("control", () => settings.GetColumn(null));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetColumn_InvalidControl_ThrowsNotSupportedException()
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        Assert.Throws<NotSupportedException>(() => settings.GetColumn("control"));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetColumn_InvalidControlStub_ReturnsExpected()
    {
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        Assert.Equal(-1, settings.GetColumn("control"));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetColumnSpan_NoSuchControl_ReturnsExpected()
    {
        using Control child = new();
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        Assert.Equal(1, settings.GetColumnSpan(child));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetColumnSpan_NoSuchControlStub_ReturnsExpected()
    {
        using Control child = new();
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        Assert.Equal(1, settings.GetColumnSpan(child));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetColumnSpan_NullControl_ThrowsArgumentNullException()
    {
        using Control child = new();
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        Assert.Throws<ArgumentNullException>("control", () => settings.GetColumnSpan(null));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetColumnSpan_NullControlStub_ThrowsArgumentNullException()
    {
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        Assert.Throws<ArgumentNullException>("control", () => settings.GetColumnSpan(null));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetColumnSpan_InvalidControl_ThrowsNotSupportedException()
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        Assert.Throws<NotSupportedException>(() => settings.GetColumnSpan("control"));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetColumnSpan_InvalidControlStub_ReturnsExpected()
    {
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        Assert.Equal(1, settings.GetColumnSpan("control"));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetRow_NoSuchControl_ReturnsExpected()
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        using Control child = new();
        Assert.Equal(-1, settings.GetRow(child));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetRow_NoSuchControlStub_ReturnsExpected()
    {
        using Control child = new();
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        Assert.Equal(-1, settings.GetRow(child));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetRow_NullControl_ThrowsArgumentNullException()
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        Assert.Throws<ArgumentNullException>("control", () => settings.GetRow(null));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetRow_NullControlStub_ThrowsArgumentNullException()
    {
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        Assert.Throws<ArgumentNullException>("control", () => settings.GetRow(null));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetRow_InvalidControl_ThrowsNotSupportedException()
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        Assert.Throws<NotSupportedException>(() => settings.GetRow("control"));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetRow_InvalidControlStub_ReturnsExpected()
    {
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        Assert.Equal(-1, settings.GetRow("control"));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetRowSpan_NoSuchControl_ReturnsExpected()
    {
        using Control child = new();
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        Assert.Equal(1, settings.GetRowSpan(child));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetRowSpan_NoSuchControlStub_ReturnsExpected()
    {
        using Control child = new();
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        Assert.Equal(1, settings.GetRowSpan(child));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetRowSpan_NullControl_ThrowsArgumentNullException()
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        Assert.Throws<ArgumentNullException>("control", () => settings.GetRowSpan(null));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetRowSpan_NullControlStub_ReturnsExpected()
    {
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        Assert.Throws<ArgumentNullException>("control", () => settings.GetRowSpan(null));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetRowSpan_InvalidControl_ThrowsNotSupportedException()
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        Assert.Throws<NotSupportedException>(() => settings.GetRowSpan("control"));
    }

    [WinFormsFact]
    public void TableLayoutSettings_GetRowSpan_InvalidControlStub_ReturnsExpected()
    {
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        Assert.Equal(1, settings.GetRowSpan("control"));
    }

    public static IEnumerable<object[]> SetCellPosition_TestData()
    {
        yield return new object[] { new TableLayoutPanelCellPosition(-1, -1) };
        yield return new object[] { new TableLayoutPanelCellPosition(0, -1) };
        yield return new object[] { new TableLayoutPanelCellPosition(-1, 0) };
        yield return new object[] { new TableLayoutPanelCellPosition(0, 0) };
        yield return new object[] { new TableLayoutPanelCellPosition(1, 2) };
    }

    [WinFormsTheory]
    [MemberData(nameof(SetCellPosition_TestData))]
    public void TableLayoutPanel_SetCellPosition_Invoke_GetReturnsExpected(TableLayoutPanelCellPosition value)
    {
        using Control child = new();
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int childLayoutCallCount = 0;
        child.Layout += (sender, e) => childLayoutCallCount++;

        settings.SetCellPosition(child, value);
        Assert.Equal(value, settings.GetCellPosition(child));
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(0, childLayoutCallCount);
        Assert.False(control.IsHandleCreated);
        Assert.False(child.IsHandleCreated);

        // Set same.
        settings.SetCellPosition(child, value);
        Assert.Equal(value, settings.GetCellPosition(child));
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(0, childLayoutCallCount);
        Assert.False(control.IsHandleCreated);
        Assert.False(child.IsHandleCreated);
    }

    public static IEnumerable<object[]> SetCellPosition_ControlWithParent_TestData()
    {
        yield return new object[] { new TableLayoutPanelCellPosition(-1, -1), 1 };
        yield return new object[] { new TableLayoutPanelCellPosition(0, -1), 1 };
        yield return new object[] { new TableLayoutPanelCellPosition(-1, 0), 1 };
        yield return new object[] { new TableLayoutPanelCellPosition(0, 0), 1 };
        yield return new object[] { new TableLayoutPanelCellPosition(1, 2), 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(SetCellPosition_ControlWithParent_TestData))]
    public void TableLayoutPanel_SetCellPosition_InvokeControlWithParent_GetReturnsExpected(TableLayoutPanelCellPosition value, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using Control child = new()
        {
            Parent = parent
        };
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int childLayoutCallCount = 0;
        child.Layout += (sender, e) => childLayoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs eventArgs)
        {
            Assert.Same(parent, sender);
            Assert.Same(child, eventArgs.AffectedControl);
            Assert.Equal("TableIndex", eventArgs.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            settings.SetCellPosition(child, value);
            Assert.Equal(value, settings.GetCellPosition(child));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, childLayoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            settings.SetCellPosition(child, value);
            Assert.Equal(value, settings.GetCellPosition(child));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, childLayoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount * 2, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(SetCellPosition_TestData))]
    public void TableLayoutPanel_SetCellPosition_InvokeStub_GetReturnsExpected(TableLayoutPanelCellPosition value)
    {
        using Control child = new();
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        int childLayoutCallCount = 0;
        child.Layout += (sender, e) => childLayoutCallCount++;

        settings.SetCellPosition(child, value);
        Assert.Equal(value, settings.GetCellPosition(child));
        Assert.Equal(0, childLayoutCallCount);
        Assert.False(child.IsHandleCreated);

        // Set same.
        settings.SetCellPosition(child, value);
        Assert.Equal(value, settings.GetCellPosition(child));
        Assert.Equal(0, childLayoutCallCount);
        Assert.False(child.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetCellPosition_TestData))]
    public void TableLayoutPanel_SetCellPosition_InvokeStubWithParent_GetReturnsExpected(TableLayoutPanelCellPosition value)
    {
        using Control parent = new();
        using Control child = new()
        {
            Parent = parent
        };
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        int childLayoutCallCount = 0;
        child.Layout += (sender, e) => childLayoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs eventArgs)
        {
            Assert.Same(parent, sender);
            Assert.Same(child, eventArgs.AffectedControl);
            Assert.Equal("TableIndex", eventArgs.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            settings.SetCellPosition(child, value);
            Assert.Equal(value, settings.GetCellPosition(child));
            Assert.Equal(0, childLayoutCallCount);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(child.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            settings.SetCellPosition(child, value);
            Assert.Equal(value, settings.GetCellPosition(child));
            Assert.Equal(0, childLayoutCallCount);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(child.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void TableLayoutSettings_SetCellPosition_InvokeMultipleTimes_GetReturnsExpected()
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;

        using Control child = new();
        settings.SetCellPosition(child, new TableLayoutPanelCellPosition(1, 1));
        Assert.Equal(new TableLayoutPanelCellPosition(1, 1), settings.GetCellPosition(child));

        settings.SetCellPosition(child, new TableLayoutPanelCellPosition(2, 2));
        Assert.Equal(new TableLayoutPanelCellPosition(2, 2), settings.GetCellPosition(child));
    }

    [WinFormsFact]
    public void TableLayoutSettings_SetCellPosition_InvokeMultipleTimesStub_GetReturnsExpected()
    {
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));

        using Control child = new();
        settings.SetCellPosition(child, new TableLayoutPanelCellPosition(1, 1));
        Assert.Equal(new TableLayoutPanelCellPosition(1, 1), settings.GetCellPosition(child));

        settings.SetCellPosition(child, new TableLayoutPanelCellPosition(2, 2));
        Assert.Equal(new TableLayoutPanelCellPosition(2, 2), settings.GetCellPosition(child));
    }

    [WinFormsFact]
    public void TableLayoutSettings_SetCellPosition_NullControl_ThrowsArgumentNullException()
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        Assert.Throws<ArgumentNullException>("control", () => settings.SetCellPosition(null, default));
    }

    [WinFormsFact]
    public void TableLayoutSettings_SetCellPosition_NullControlStub_ThrowsArgumentNullException()
    {
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        Assert.Throws<ArgumentNullException>("control", () => settings.SetCellPosition(null, default));
    }

    [WinFormsFact]
    public void TableLayoutSettings_SetCellPosition_InvalidControl_ThrowsNotSupportedException()
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        Assert.Throws<NotSupportedException>(() => settings.SetCellPosition("control", default));
    }

    [WinFormsFact]
    public void TableLayoutSettings_SetCellPosition_InvalidControlStub_GetReturnsExpected()
    {
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        settings.SetCellPosition("control", default);
        Assert.Equal(default, settings.GetCellPosition("control"));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(int.MaxValue)]
    public void TableLayoutPanel_SetColumn_Invoke_GetReturnsExpected(int value)
    {
        using Control child = new();
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int childLayoutCallCount = 0;
        child.Layout += (sender, e) => childLayoutCallCount++;

        settings.SetColumn(child, value);
        Assert.Equal(value, settings.GetColumn(child));
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(0, childLayoutCallCount);
        Assert.False(control.IsHandleCreated);
        Assert.False(child.IsHandleCreated);

        // Set same.
        settings.SetColumn(child, value);
        Assert.Equal(value, settings.GetColumn(child));
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(0, childLayoutCallCount);
        Assert.False(control.IsHandleCreated);
        Assert.False(child.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1, 1)]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    [InlineData(int.MaxValue, 1)]
    public void TableLayoutPanel_SetColumn_InvokeControlWithParent_GetReturnsExpected(int value, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using Control child = new()
        {
            Parent = parent
        };
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int childLayoutCallCount = 0;
        child.Layout += (sender, e) => childLayoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs eventArgs)
        {
            Assert.Same(parent, sender);
            Assert.Same(child, eventArgs.AffectedControl);
            Assert.Equal("TableIndex", eventArgs.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            settings.SetColumn(child, value);
            Assert.Equal(value, settings.GetColumn(child));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, childLayoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            settings.SetColumn(child, value);
            Assert.Equal(value, settings.GetColumn(child));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, childLayoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount * 2, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(int.MaxValue)]
    public void TableLayoutPanel_SetColumn_InvokeStub_GetReturnsExpected(int value)
    {
        using Control child = new();
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        int childLayoutCallCount = 0;
        child.Layout += (sender, e) => childLayoutCallCount++;

        settings.SetColumn(child, value);
        Assert.Equal(value, settings.GetColumn(child));
        Assert.Equal(0, childLayoutCallCount);
        Assert.False(child.IsHandleCreated);

        // Set same.
        settings.SetColumn(child, value);
        Assert.Equal(value, settings.GetColumn(child));
        Assert.Equal(0, childLayoutCallCount);
        Assert.False(child.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(int.MaxValue)]
    public void TableLayoutPanel_SetColumn_InvokeStubWithParent_GetReturnsExpected(int value)
    {
        using Control parent = new();
        using Control child = new()
        {
            Parent = parent
        };
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        int childLayoutCallCount = 0;
        child.Layout += (sender, e) => childLayoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs eventArgs) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            settings.SetColumn(child, value);
            Assert.Equal(value, settings.GetColumn(child));
            Assert.Equal(0, childLayoutCallCount);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(child.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            settings.SetColumn(child, value);
            Assert.Equal(value, settings.GetColumn(child));
            Assert.Equal(0, childLayoutCallCount);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(child.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void TableLayoutSettings_SetColumn_InvokeMultipleTimes_GetReturnsExpected()
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;

        using Control child = new();
        settings.SetColumn(child, 1);
        Assert.Equal(1, settings.GetColumn(child));

        settings.SetColumn(child, 2);
        Assert.Equal(2, settings.GetColumn(child));
    }

    [WinFormsFact]
    public void TableLayoutSettings_SetColumn_InvokeMultipleTimesStub_GetReturnsExpected()
    {
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));

        using Control child = new();
        settings.SetColumn(child, 1);
        Assert.Equal(1, settings.GetColumn(child));

        settings.SetColumn(child, 2);
        Assert.Equal(2, settings.GetColumn(child));
    }

    [WinFormsTheory]
    [InlineData(-2)]
    [InlineData(-1)]
    public void TableLayoutSettings_SetColumn_NullControl_ThrowsArgumentNullException(int value)
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        Assert.Throws<ArgumentNullException>("control", () => settings.SetColumn(null, value));
    }

    [WinFormsTheory]
    [InlineData(-2)]
    [InlineData(-1)]
    public void TableLayoutSettings_SetColumn_NullControlStub_ThrowsArgumentNullException(int value)
    {
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        Assert.Throws<ArgumentNullException>("control", () => settings.SetColumn(null, value));
    }

    [WinFormsFact]
    public void TableLayoutSettings_SetColumn_InvalidControl_ThrowsNotSupportedException()
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        Assert.Throws<NotSupportedException>(() => settings.SetColumn("control", 1));
    }

    [WinFormsFact]
    public void TableLayoutSettings_SetColumn_InvalidControlStub_GetReturnsExpected()
    {
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        settings.SetColumn("control", 1);
        Assert.Equal(1, settings.GetColumn("control"));
    }

    [WinFormsFact]
    public void TableLayoutSettings_SetColumn_InvalidColumn_ThrowsArgumentOutOfRangeException()
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        Assert.Throws<ArgumentOutOfRangeException>("column", () => settings.SetColumn("control", -2));
    }

    [WinFormsFact]
    public void TableLayoutSettings_SetColumn_InvalidColumnStub_ThrowsArgumentOutOfRangeException()
    {
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        Assert.Throws<ArgumentOutOfRangeException>("column", () => settings.SetColumn("control", -2));
    }

    [WinFormsTheory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(int.MaxValue)]
    public void TableLayoutPanel_SetColumnSpan_Invoke_GetReturnsExpected(int value)
    {
        using Control child = new();
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int childLayoutCallCount = 0;
        child.Layout += (sender, e) => childLayoutCallCount++;

        settings.SetColumnSpan(child, value);
        Assert.Equal(value, settings.GetColumnSpan(child));
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(0, childLayoutCallCount);
        Assert.False(control.IsHandleCreated);
        Assert.False(child.IsHandleCreated);

        // Set same.
        settings.SetColumnSpan(child, value);
        Assert.Equal(value, settings.GetColumnSpan(child));
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(0, childLayoutCallCount);
        Assert.False(control.IsHandleCreated);
        Assert.False(child.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    public void TableLayoutPanel_SetColumnSpan_InvokeControlWithParent_GetReturnsExpected(int value, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using Control child = new()
        {
            Parent = parent
        };
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int childLayoutCallCount = 0;
        child.Layout += (sender, e) => childLayoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs eventArgs)
        {
            Assert.Same(parent, sender);
            Assert.Same(child, eventArgs.AffectedControl);
            Assert.Equal("ColumnSpan", eventArgs.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            settings.SetColumnSpan(child, value);
            Assert.Equal(value, settings.GetColumnSpan(child));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, childLayoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            settings.SetColumnSpan(child, value);
            Assert.Equal(value, settings.GetColumnSpan(child));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, childLayoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount + 1, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(int.MaxValue)]
    public void TableLayoutPanel_SetColumnSpan_InvokeStub_GetReturnsExpected(int value)
    {
        using Control child = new();
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        int childLayoutCallCount = 0;
        child.Layout += (sender, e) => childLayoutCallCount++;

        settings.SetColumnSpan(child, value);
        Assert.Equal(value, settings.GetColumnSpan(child));
        Assert.Equal(0, childLayoutCallCount);
        Assert.False(child.IsHandleCreated);

        // Set same.
        settings.SetColumnSpan(child, value);
        Assert.Equal(value, settings.GetColumnSpan(child));
        Assert.Equal(0, childLayoutCallCount);
        Assert.False(child.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(int.MaxValue)]
    public void TableLayoutPanel_SetColumnSpan_InvokeStubWithParent_GetReturnsExpected(int value)
    {
        using Control parent = new();
        using Control child = new()
        {
            Parent = parent
        };
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        int childLayoutCallCount = 0;
        child.Layout += (sender, e) => childLayoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs eventArgs) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            settings.SetColumnSpan(child, value);
            Assert.Equal(value, settings.GetColumnSpan(child));
            Assert.Equal(0, childLayoutCallCount);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(child.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            settings.SetColumnSpan(child, value);
            Assert.Equal(value, settings.GetColumnSpan(child));
            Assert.Equal(0, childLayoutCallCount);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(child.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void TableLayoutSettings_SetColumnSpan_InvokeMultipleTimes_GetReturnsExpected()
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;

        using Control child = new();
        settings.SetColumnSpan(child, 1);
        Assert.Equal(1, settings.GetColumnSpan(child));

        settings.SetColumnSpan(child, 2);
        Assert.Equal(2, settings.GetColumnSpan(child));
    }

    [WinFormsFact]
    public void TableLayoutSettings_SetColumnSpan_InvokeMultipleTimesStub_GetReturnsExpected()
    {
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));

        using Control child = new();
        settings.SetColumnSpan(child, 1);
        Assert.Equal(1, settings.GetColumnSpan(child));

        settings.SetColumnSpan(child, 2);
        Assert.Equal(2, settings.GetColumnSpan(child));
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    public void TableLayoutSettings_SetColumnSpan_NullControl_ThrowsArgumentNullException(int value)
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        Assert.Throws<ArgumentNullException>("control", () => settings.SetColumnSpan(null, value));
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    public void TableLayoutSettings_SetColumnSpan_NullControlStub_ThrowsArgumentNullException(int value)
    {
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        Assert.Throws<ArgumentNullException>("control", () => settings.SetColumnSpan(null, value));
        Assert.Throws<ArgumentNullException>("control", () => settings.GetColumnSpan(null));
    }

    [WinFormsFact]
    public void TableLayoutSettings_SetColumnSpan_InvalidControl_ThrowsNotSupportedException()
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        Assert.Throws<NotSupportedException>(() => settings.SetColumnSpan("control", 1));
    }

    [WinFormsFact]
    public void TableLayoutSettings_SetColumnSpan_InvalidControlStub_GetReturnsExpected()
    {
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        settings.SetColumnSpan("control", 1);
        Assert.Equal(1, settings.GetColumnSpan("control"));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    public void TableLayoutSettings_SetColumnSpan_InvalidValue_ThrowsArgumentOutOfRangeException(int value)
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        Assert.Throws<ArgumentOutOfRangeException>("value", () => settings.SetColumnSpan("control", value));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    public void TableLayoutSettings_SetColumnSpan_InvalidValueStub_ThrowsArgumentOutOfRangeException(int value)
    {
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        Assert.Throws<ArgumentOutOfRangeException>("value", () => settings.SetColumnSpan("control", value));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(int.MaxValue)]
    public void TableLayoutPanel_SetRow_Invoke_GetReturnsExpected(int value)
    {
        using Control child = new();
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int childLayoutCallCount = 0;
        child.Layout += (sender, e) => childLayoutCallCount++;

        settings.SetRow(child, value);
        Assert.Equal(value, settings.GetRow(child));
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(0, childLayoutCallCount);
        Assert.False(control.IsHandleCreated);
        Assert.False(child.IsHandleCreated);

        // Set same.
        settings.SetRow(child, value);
        Assert.Equal(value, settings.GetRow(child));
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(0, childLayoutCallCount);
        Assert.False(control.IsHandleCreated);
        Assert.False(child.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1, 1)]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    [InlineData(int.MaxValue, 1)]
    public void TableLayoutPanel_SetRow_InvokeControlWithParent_GetReturnsExpected(int value, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using Control child = new()
        {
            Parent = parent
        };
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int childLayoutCallCount = 0;
        child.Layout += (sender, e) => childLayoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs eventArgs)
        {
            Assert.Same(parent, sender);
            Assert.Same(child, eventArgs.AffectedControl);
            Assert.Equal("TableIndex", eventArgs.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            settings.SetRow(child, value);
            Assert.Equal(value, settings.GetRow(child));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, childLayoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            settings.SetRow(child, value);
            Assert.Equal(value, settings.GetRow(child));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, childLayoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount * 2, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(int.MaxValue)]
    public void TableLayoutPanel_SetRow_InvokeStub_GetReturnsExpected(int value)
    {
        using Control child = new();
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        int childLayoutCallCount = 0;
        child.Layout += (sender, e) => childLayoutCallCount++;

        settings.SetRow(child, value);
        Assert.Equal(value, settings.GetRow(child));
        Assert.Equal(0, childLayoutCallCount);
        Assert.False(child.IsHandleCreated);

        // Set same.
        settings.SetRow(child, value);
        Assert.Equal(value, settings.GetRow(child));
        Assert.Equal(0, childLayoutCallCount);
        Assert.False(child.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(int.MaxValue)]
    public void TableLayoutPanel_SetRow_InvokeStubWithParent_GetReturnsExpected(int value)
    {
        using Control parent = new();
        using Control child = new()
        {
            Parent = parent
        };
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        int childLayoutCallCount = 0;
        child.Layout += (sender, e) => childLayoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs eventArgs) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            settings.SetRow(child, value);
            Assert.Equal(value, settings.GetRow(child));
            Assert.Equal(0, childLayoutCallCount);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(child.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            settings.SetRow(child, value);
            Assert.Equal(value, settings.GetRow(child));
            Assert.Equal(0, childLayoutCallCount);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(child.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void TableLayoutSettings_SetRow_InvokeMultipleTimes_GetReturnsExpected()
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;

        using Control child = new();
        settings.SetRow(child, 1);
        Assert.Equal(1, settings.GetRow(child));

        settings.SetRow(child, 2);
        Assert.Equal(2, settings.GetRow(child));
    }

    [WinFormsFact]
    public void TableLayoutSettings_SetRow_InvokeMultipleTimesStub_GetReturnsExpected()
    {
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));

        using Control child = new();
        settings.SetRow(child, 1);
        Assert.Equal(1, settings.GetRow(child));

        settings.SetRow(child, 2);
        Assert.Equal(2, settings.GetRow(child));
    }

    [WinFormsTheory]
    [InlineData(-2)]
    [InlineData(-1)]
    public void TableLayoutSettings_SetRow_NullControl_ThrowsArgumentNullException(int value)
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        Assert.Throws<ArgumentNullException>("control", () => settings.SetRow(null, value));
    }

    [WinFormsTheory]
    [InlineData(-2)]
    [InlineData(-1)]
    public void TableLayoutSettings_SetRow_NullControlStub_ThrowsArgumentNullException(int value)
    {
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        Assert.Throws<ArgumentNullException>("control", () => settings.SetRow(null, value));
    }

    [WinFormsFact]
    public void TableLayoutSettings_SetRow_InvalidControl_ThrowsNotSupportedException()
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        Assert.Throws<NotSupportedException>(() => settings.SetRow("control", 1));
    }

    [WinFormsFact]
    public void TableLayoutSettings_SetRow_InvalidControlStub_GetReturnsExpected()
    {
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        settings.SetRow("control", 1);
        Assert.Equal(1, settings.GetRow("control"));
    }

    [WinFormsFact]
    public void TableLayoutSettings_SetRow_InvalidRow_ThrowsArgumentOutOfRangeException()
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        Assert.Throws<ArgumentOutOfRangeException>("row", () => settings.SetRow("control", -2));
    }

    [WinFormsFact]
    public void TableLayoutSettings_SetRow_InvalidRowStub_ThrowsArgumentOutOfRangeException()
    {
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        Assert.Throws<ArgumentOutOfRangeException>("row", () => settings.SetRow("control", -2));
    }

    [WinFormsTheory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(int.MaxValue)]
    public void TableLayoutPanel_SetRowSpan_Invoke_GetReturnsExpected(int value)
    {
        using Control child = new();
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int childLayoutCallCount = 0;
        child.Layout += (sender, e) => childLayoutCallCount++;

        settings.SetRowSpan(child, value);
        Assert.Equal(value, settings.GetRowSpan(child));
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(0, childLayoutCallCount);
        Assert.False(control.IsHandleCreated);
        Assert.False(child.IsHandleCreated);

        // Set same.
        settings.SetRowSpan(child, value);
        Assert.Equal(value, settings.GetRowSpan(child));
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(0, childLayoutCallCount);
        Assert.False(control.IsHandleCreated);
        Assert.False(child.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    public void TableLayoutPanel_SetRowSpan_InvokeControlWithParent_GetReturnsExpected(int value, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using Control child = new()
        {
            Parent = parent
        };
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int childLayoutCallCount = 0;
        child.Layout += (sender, e) => childLayoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs eventArgs)
        {
            Assert.Same(parent, sender);
            Assert.Same(child, eventArgs.AffectedControl);
            Assert.Equal("RowSpan", eventArgs.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            settings.SetRowSpan(child, value);
            Assert.Equal(value, settings.GetRowSpan(child));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, childLayoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            settings.SetRowSpan(child, value);
            Assert.Equal(value, settings.GetRowSpan(child));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, childLayoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount + 1, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(int.MaxValue)]
    public void TableLayoutPanel_SetRowSpan_InvokeStub_GetReturnsExpected(int value)
    {
        using Control child = new();
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        int childLayoutCallCount = 0;
        child.Layout += (sender, e) => childLayoutCallCount++;

        settings.SetRowSpan(child, value);
        Assert.Equal(value, settings.GetRowSpan(child));
        Assert.Equal(0, childLayoutCallCount);
        Assert.False(child.IsHandleCreated);

        // Set same.
        settings.SetRowSpan(child, value);
        Assert.Equal(value, settings.GetRowSpan(child));
        Assert.Equal(0, childLayoutCallCount);
        Assert.False(child.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(int.MaxValue)]
    public void TableLayoutPanel_SetRowSpan_InvokeStubWithParent_GetReturnsExpected(int value)
    {
        using Control parent = new();
        using Control child = new()
        {
            Parent = parent
        };
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        int childLayoutCallCount = 0;
        child.Layout += (sender, e) => childLayoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs eventArgs) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            settings.SetRowSpan(child, value);
            Assert.Equal(value, settings.GetRowSpan(child));
            Assert.Equal(0, childLayoutCallCount);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(child.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            settings.SetRowSpan(child, value);
            Assert.Equal(value, settings.GetRowSpan(child));
            Assert.Equal(0, childLayoutCallCount);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(child.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void TableLayoutSettings_SetRowSpan_InvokeMultipleTimes_GetReturnsExpected()
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;

        using Control child = new();
        settings.SetRowSpan(child, 1);
        Assert.Equal(1, settings.GetRowSpan(child));

        settings.SetRowSpan(child, 2);
        Assert.Equal(2, settings.GetRowSpan(child));
    }

    [WinFormsFact]
    public void TableLayoutSettings_SetRowSpan_InvokeMultipleTimesStub_GetReturnsExpected()
    {
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));

        using Control child = new();
        settings.SetRowSpan(child, 1);
        Assert.Equal(1, settings.GetRowSpan(child));

        settings.SetRowSpan(child, 2);
        Assert.Equal(2, settings.GetRowSpan(child));
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    public void TableLayoutSettings_SetRowSpan_NullControl_ThrowsArgumentNullException(int value)
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        Assert.Throws<ArgumentNullException>("control", () => settings.SetRowSpan(null, value));
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    public void TableLayoutSettings_SetRowSpan_NullControlStub_ThrowsArgumentNullException(int value)
    {
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        Assert.Throws<ArgumentNullException>("control", () => settings.SetRowSpan(null, value));
    }

    [WinFormsFact]
    public void TableLayoutSettings_SetRowSpan_InvalidControl_ThrowsNotSupportedException()
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        Assert.Throws<NotSupportedException>(() => settings.SetRowSpan("control", 1));
    }

    [WinFormsFact]
    public void TableLayoutSettings_SetRowSpan_InvalidControlStub_GetReturnsExpected()
    {
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        settings.SetRowSpan("control", 1);
        Assert.Equal(1, settings.GetRowSpan("control"));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    public void TableLayoutSettings_SetRowSpan_InvalidValue_ThrowsArgumentOutOfRangeException(int value)
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        Assert.Throws<ArgumentOutOfRangeException>("value", () => settings.SetRowSpan("control", value));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    public void TableLayoutSettings_SetRowSpan_InvalidValueStub_ThrowsArgumentOutOfRangeException(int value)
    {
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        Assert.Throws<ArgumentOutOfRangeException>("value", () => settings.SetRowSpan("control", value));
    }

    [WinFormsFact]
    public void TableLayoutSettings_ISerializableGetObjectData_InvokeSimple_Success()
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        ISerializable iSerializable = settings;
        SerializationInfo info = new(typeof(ListViewGroup), new FormatterConverter());
        StreamingContext context = default;

        iSerializable.GetObjectData(info, context);
        Assert.Equal(@"<?xml version=""1.0"" encoding=""utf-16""?><TableLayoutSettings><Controls /><Columns Styles="""" /><Rows Styles="""" /></TableLayoutSettings>", info.GetString("SerializedString"));
    }

    [WinFormsFact]
    public void TableLayoutSettings_ISerializableGetObjectData_InvokeSimpleStub_Success()
    {
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
        ISerializable iSerializable = settings;
        SerializationInfo info = new(typeof(ListViewGroup), new FormatterConverter());
        StreamingContext context = default;

        iSerializable.GetObjectData(info, context);
        Assert.Equal(@"<?xml version=""1.0"" encoding=""utf-16""?><TableLayoutSettings><Controls /><Columns Styles="""" /><Rows Styles="""" /></TableLayoutSettings>", info.GetString("SerializedString"));
    }

    [WinFormsFact]
    public void TableLayoutSettings_ISerializableGetObjectData_InvokeAdvanced_Success()
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;

        // Setup controls.
        using Control controlWithName = new()
        {
            Name = "name"
        };
        using Control controlWithDefaultName = new();
        settings.SetColumnSpan(controlWithName, 1);
        settings.SetRowSpan(controlWithName, 2);
        settings.SetColumn(controlWithName, 3);
        settings.SetRow(controlWithName, 4);

        // Setup styles.
        ColumnStyle columnStyle = new(SizeType.Percent, 1);
        RowStyle rowStyle = new(SizeType.Percent, 2);
        settings.ColumnStyles.Add(columnStyle);
        settings.RowStyles.Add(rowStyle);

        // Serialize.
        ISerializable iSerializable = settings;
        SerializationInfo info = new(typeof(ListViewGroup), new FormatterConverter());
        StreamingContext context = default;

        iSerializable.GetObjectData(info, context);
        Assert.Equal(@"<?xml version=""1.0"" encoding=""utf-16""?><TableLayoutSettings><Controls /><Columns Styles=""Percent,1"" /><Rows Styles=""Percent,2"" /></TableLayoutSettings>", info.GetString("SerializedString"));

        // Set parent.
        info = new SerializationInfo(typeof(ListViewGroup), new FormatterConverter());
        control.Controls.Add(controlWithName);
        control.Controls.Add(controlWithDefaultName);
        iSerializable.GetObjectData(info, context);
        Assert.Equal(@"<?xml version=""1.0"" encoding=""utf-16""?><TableLayoutSettings><Controls><Control Name=""name"" Row=""4"" RowSpan=""2"" Column=""3"" ColumnSpan=""1"" /><Control Name="""" Row=""-1"" RowSpan=""1"" Column=""-1"" ColumnSpan=""1"" /></Controls><Columns Styles=""Percent,1"" /><Rows Styles=""Percent,2"" /></TableLayoutSettings>", info.GetString("SerializedString"));
    }

    [WinFormsFact]
    public void TableLayoutSettings_ISerializableGetObjectData_InvokeAdvancedStub_Success()
    {
        TableLayoutSettingsTypeConverter converter = new();
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));

        // Setup controls.
        settings.SetColumnSpan("name", 1);
        settings.SetRowSpan("name", 2);
        settings.SetColumn("name", 3);
        settings.SetRow("name", 4);
        settings.SetRow("", 1);
        settings.SetColumn("", 1);

        // Setup styles.
        ColumnStyle columnStyle = new(SizeType.Percent, 1);
        RowStyle rowStyle = new(SizeType.Percent, 2);
        settings.ColumnStyles.Add(columnStyle);
        settings.RowStyles.Add(rowStyle);

        // Serialize.
        ISerializable iSerializable = settings;
        SerializationInfo info = new(typeof(ListViewGroup), new FormatterConverter());
        StreamingContext context = default;

        iSerializable.GetObjectData(info, context);
        Assert.Equal(@"<?xml version=""1.0"" encoding=""utf-16""?><TableLayoutSettings><Controls><Control Name=""name"" Row=""4"" RowSpan=""2"" Column=""3"" ColumnSpan=""1"" /><Control Name="""" Row=""1"" RowSpan=""1"" Column=""1"" ColumnSpan=""1"" /></Controls><Columns Styles=""Percent,1"" /><Rows Styles=""Percent,2"" /></TableLayoutSettings>", info.GetString("SerializedString"));
    }

    [WinFormsFact]
    public void TableLayoutSettings_ISerializableGetObjectData_InvokeToolStrip_Success()
    {
        using ToolStrip control = new()
        {
            LayoutStyle = ToolStripLayoutStyle.Table
        };
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(control.LayoutSettings);
        ISerializable iSerializable = settings;
        SerializationInfo info = new(typeof(ListViewGroup), new FormatterConverter());
        StreamingContext context = default;

        iSerializable.GetObjectData(info, context);
        Assert.Equal(@"<?xml version=""1.0"" encoding=""utf-16""?><TableLayoutSettings><Controls /><Columns Styles="""" /><Rows Styles="""" /></TableLayoutSettings>", info.GetString("SerializedString"));

        // Add children.
        using SubToolStripItem itemWithDefaultName = new();
        using ToolStripItemWithoutName itemWithoutName = new();
        using ToolStripItemWithNonStringName itemWithNonStringName = new();
        using SubToolStripItem itemWithName = new()
        {
            Name = "Name"
        };
        control.Items.Add(itemWithDefaultName);
        control.Items.Add(itemWithoutName);
        control.Items.Add(itemWithNonStringName);
        control.Items.Add(itemWithName);
        Assert.Equal(@"<?xml version=""1.0"" encoding=""utf-16""?><TableLayoutSettings><Controls /><Columns Styles="""" /><Rows Styles="""" /></TableLayoutSettings>", info.GetString("SerializedString"));
    }

    [WinFormsTheory]
    [InlineData(typeof(NullStringConverter))]
    [InlineData(typeof(EmptyStringConverter))]
    public void TableLayoutSettings_ISerializableGetObjectData_InvokeInvalidStringConverter_Success(Type type)
    {
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        TypeDescriptor.AddAttributes(settings, [new TypeConverterAttribute(type)]);

        ISerializable iSerializable = settings;
        SerializationInfo info = new(typeof(ListViewGroup), new FormatterConverter());
        StreamingContext context = default;

        iSerializable.GetObjectData(info, context);
        Assert.Throws<SerializationException>(() => info.GetString("SerializedString"));
    }

    [WinFormsFact]
    public void TableLayoutSettings_Serialize_Deserialize_Success()
    {
        using BinaryFormatterScope formatterScope = new(enable: true);
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        ColumnStyle columnStyle = new(SizeType.Percent, 1);
        RowStyle rowStyle = new(SizeType.Percent, 2);

        using Control controlWithName = new() { Name = "name" };
        settings.SetColumnSpan(controlWithName, 1);
        settings.SetRowSpan(controlWithName, 2);
        settings.SetColumn(controlWithName, 3);
        settings.SetRow(controlWithName, 4);
        settings.ColumnStyles.Add(columnStyle);
        settings.RowStyles.Add(rowStyle);

        using MemoryStream stream = new();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
        // cs/binary-formatter-without-binder
        BinaryFormatter formatter = new(); // CodeQL [SM04191] : This is a test. Safe use because the deserialization process is performed on trusted data and the types are controlled and validated.
        formatter.Serialize(stream, settings);
        stream.Seek(0, SeekOrigin.Begin);

        // cs/dangerous-binary-deserialization
        TableLayoutSettings result = Assert.IsType<TableLayoutSettings>(formatter.Deserialize(stream)); // CodeQL [SM03722] : Testing legacy feature. This is a safe use of BinaryFormatter because the data is trusted and the types are controlled and validated.
#pragma warning restore SYSLIB0011
        Assert.Equal(columnStyle.SizeType, ((ColumnStyle)Assert.Single(result.ColumnStyles)).SizeType);
        Assert.Equal(columnStyle.Width, ((ColumnStyle)Assert.Single(result.ColumnStyles)).Width);
        Assert.Equal(rowStyle.SizeType, ((RowStyle)Assert.Single(result.RowStyles)).SizeType);
        Assert.Equal(rowStyle.Height, ((RowStyle)Assert.Single(result.RowStyles)).Height);

        Assert.Equal(1, result.GetColumnSpan(controlWithName));
        Assert.Equal(1, result.GetRowSpan(controlWithName));
        Assert.Equal(-1, result.GetColumn(controlWithName));
        Assert.Equal(-1, result.GetRow(controlWithName));
    }

    [WinFormsTheory]
    [InlineData(typeof(NullStringConverter))]
    [InlineData(typeof(EmptyStringConverter))]
    public void TableLayoutSettings_Serialize_InvalidStringConverter_DeserializeThrowsSerializationException(Type type)
    {
        using BinaryFormatterScope formatterScope = new(enable: true);
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        TypeDescriptor.AddAttributes(settings, [new TypeConverterAttribute(type)]);
        using MemoryStream stream = new();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
        // cs/binary-formatter-without-binder
        BinaryFormatter formatter = new(); // CodeQL [SM04191] : This is a test. Safe use because the deserialization process is performed on trusted data and the types are controlled and validated.
        formatter.Serialize(stream, settings);
        stream.Seek(0, SeekOrigin.Begin);

        // cs/dangerous-binary-deserialization
        Assert.Throws<SerializationException>(() => formatter.Deserialize(stream)); // CodeQL [SM03722] : Testing legacy feature. This is a safe use of BinaryFormatter because the data is trusted and the types are controlled and validated.

#pragma warning restore SYSLIB0011
    }

    [WinFormsTheory]
    [InlineData(typeof(NullTableLayoutSettingsConverter))]
    [InlineData(typeof(NonTableLayoutSettingsConverter))]
    public void TableLayoutSettings_Deserialize_InvalidConverterResult_Success(Type type)
    {
        using BinaryFormatterScope formatterScope = new(enable: true);
        using TableLayoutPanel control = new();
        TableLayoutSettings settings = control.LayoutSettings;
        TypeDescriptor.AddAttributes(settings, [new TypeConverterAttribute(type)]);
        using MemoryStream stream = new();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
        // cs/binary-formatter-without-binder
        BinaryFormatter formatter = new(); // CodeQL [SM04191] : This is a test. Safe use because the deserialization process is performed on trusted data and the types are controlled and validated.
        formatter.Serialize(stream, settings);

        stream.Seek(0, SeekOrigin.Begin);

        // cs/dangerous-binary-deserialization
        TableLayoutSettings result = Assert.IsType<TableLayoutSettings>(formatter.Deserialize(stream)); // CodeQL [SM03722] : Testing legacy feature. This is a safe use of BinaryFormatter because the data is trusted and the types are controlled and validated.
#pragma warning restore SYSLIB0011
        Assert.NotNull(result.LayoutEngine);
        Assert.Same(result.LayoutEngine, result.LayoutEngine);
        Assert.Throws<NullReferenceException>(() => result.ColumnCount);
        Assert.Throws<NullReferenceException>(() => result.RowCount);
        Assert.Empty(result.ColumnStyles);
        Assert.Empty(result.RowStyles);
        Assert.Throws<NullReferenceException>(() => result.GrowStyle);
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

    private class CustomTypeDescriptionProvider : TypeDescriptionProvider
    {
        public CustomTypeDescriptionProvider()
        {
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            Mock<ICustomTypeDescriptor> mockDescriptor = new(MockBehavior.Strict);
            mockDescriptor
                .Setup(c => c.GetProperties())
                .Returns(new PropertyDescriptorCollection(Array.Empty<PropertyDescriptor>()));
            return mockDescriptor.Object;
        }
    }

    private class SubToolStripItem : ToolStripItem
    {
    }

    [TypeDescriptionProvider(typeof(CustomTypeDescriptionProvider))]
    private class ToolStripItemWithoutName : ToolStripItem
    {
    }

    private class ToolStripItemWithNonStringName : ToolStripItem
    {
        public new int Name { get; set; }
    }
}
