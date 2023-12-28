// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class TableLayoutColumnStyleCollectionTests
{
    [WinFormsFact]
    public void TableLayoutColumnStyleCollection_Add_ColumnStyle_Success()
    {
        using ToolStrip toolStrip = new() { LayoutStyle = ToolStripLayoutStyle.Table };
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
        TableLayoutColumnStyleCollection collection = settings.ColumnStyles;

        ColumnStyle style = new();
        collection.Add(style);
        Assert.Equal(style, Assert.Single(collection));
    }

    [WinFormsFact]
    public void TableLayoutColumnStyleCollection_Add_RowStyle_ThrowsInvalidCastException()
    {
        using ToolStrip toolStrip = new() { LayoutStyle = ToolStripLayoutStyle.Table };
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
        TableLayoutColumnStyleCollection collection = settings.ColumnStyles;

        RowStyle style = new();
        Assert.Throws<InvalidCastException>(() => collection.Add(style));
        Assert.Equal(style, Assert.Single(collection));
    }

    [WinFormsFact]
    public void TableLayoutColumnStyleCollection_Insert_ColumnStyle_Success()
    {
        using ToolStrip toolStrip = new() { LayoutStyle = ToolStripLayoutStyle.Table };
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
        TableLayoutColumnStyleCollection collection = settings.ColumnStyles;

        ColumnStyle style = new();
        collection.Insert(0, style);
        Assert.Equal(style, Assert.Single(collection));
    }

    [WinFormsFact]
    public void TableLayoutColumnStyleCollection_Item_SetColumnStyle_GetReturnsExpected()
    {
        using ToolStrip toolStrip = new() { LayoutStyle = ToolStripLayoutStyle.Table };
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
        TableLayoutColumnStyleCollection collection = settings.ColumnStyles;
        collection.Add(new ColumnStyle());

        ColumnStyle style = new();
        collection[0] = style;
        Assert.Single(collection);
        Assert.Equal(style, collection[0]);
    }

    [WinFormsFact]
    public void TableLayoutColumnStyleCollection_Item_GetNotColumnStyle_ThrowsInvalidCastException()
    {
        using ToolStrip toolStrip = new() { LayoutStyle = ToolStripLayoutStyle.Table };
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
        TableLayoutColumnStyleCollection collection = settings.ColumnStyles;
        Assert.Throws<InvalidCastException>(() => collection.Add(new RowStyle()));
        Assert.Throws<InvalidCastException>(() => collection[0]);
    }

    [WinFormsFact]
    public void TableLayoutColumnStyleCollection_Remove_ColumnStyle_Success()
    {
        using ToolStrip toolStrip = new() { LayoutStyle = ToolStripLayoutStyle.Table };
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
        TableLayoutColumnStyleCollection collection = settings.ColumnStyles;
        ColumnStyle style = new();
        collection.Add(style);
        collection.Remove(style);
        Assert.Empty(collection);

        collection.Add(style);
        Assert.Equal(style, Assert.Single(collection));
    }

    [WinFormsFact]
    public void TableLayoutColumnStyleCollection_Contains_ColumnStyle_ReturnsExpected()
    {
        using ToolStrip toolStrip = new() { LayoutStyle = ToolStripLayoutStyle.Table };
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
        TableLayoutColumnStyleCollection collection = settings.ColumnStyles;
        ColumnStyle style = new();
        collection.Add(style);
        Assert.True(collection.Contains(style));
        Assert.False(collection.Contains(new ColumnStyle()));
        Assert.False(collection.Contains(null));
    }

    [WinFormsFact]
    public void TableLayoutColumnStyleCollection_IndexOf_Invoke_ReturnsExpected()
    {
        using ToolStrip toolStrip = new() { LayoutStyle = ToolStripLayoutStyle.Table };
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
        TableLayoutColumnStyleCollection collection = settings.ColumnStyles;
        ColumnStyle style = new();
        collection.Add(style);
        Assert.Equal(0, collection.IndexOf(style));
        Assert.Equal(-1, collection.IndexOf(new ColumnStyle()));
        Assert.Equal(-1, collection.IndexOf(null));
    }
}
