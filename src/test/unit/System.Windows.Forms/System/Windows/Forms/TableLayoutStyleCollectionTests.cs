// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms.Tests;

public class TableLayoutStyleCollectionTests
{
    [WinFormsFact]
    public void TableLayoutStyleCollection_Properties_GetDefault_ReturnsExpected()
    {
        using ToolStrip toolStrip = new() { LayoutStyle = ToolStripLayoutStyle.Table };
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
        IList collection = settings.RowStyles;
        Assert.Empty(collection);
        Assert.False(collection.IsFixedSize);
        Assert.False(collection.IsReadOnly);
        Assert.False(collection.IsSynchronized);
        Assert.NotNull(collection.SyncRoot);
    }

    [WinFormsFact]
    public void TableLayoutStyleCollection_Add_TableLayoutStyle_Success()
    {
        using ToolStrip toolStrip = new() { LayoutStyle = ToolStripLayoutStyle.Table };
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
        TableLayoutStyleCollection collection = settings.RowStyles;

        RowStyle style = new();
        collection.Add(style);
        Assert.Equal(style, Assert.Single(collection));
    }

    [WinFormsFact]
    public void TableLayoutStyleCollection_Add_Object_Success()
    {
        using ToolStrip toolStrip = new() { LayoutStyle = ToolStripLayoutStyle.Table };
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
        IList collection = settings.RowStyles;

        RowStyle style = new();
        collection.Add(style);
        Assert.Equal(style, Assert.Single(collection));
    }

    [WinFormsFact]
    public void TableLayoutStyleCollection_Add_Null_ThrowsArgumentNullException()
    {
        using ToolStrip toolStrip = new() { LayoutStyle = ToolStripLayoutStyle.Table };
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
        TableLayoutStyleCollection collection = settings.RowStyles;
        Assert.Throws<ArgumentNullException>("style", () => collection.Add(null));
        Assert.Throws<ArgumentNullException>("style", () => ((IList)collection).Add(null));
    }

    [WinFormsFact]
    public void TableLayoutStyleCollection_Add_ObjectNotTableLayoutStyle_ThrowsInvalidCastException()
    {
        using ToolStrip toolStrip = new() { LayoutStyle = ToolStripLayoutStyle.Table };
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
        IList collection = settings.RowStyles;
        Assert.Throws<InvalidCastException>(() => collection.Add(new object()));
    }

    [WinFormsFact]
    public void TableLayoutStyleCollection_Add_StyleAlreadyAdded_ThrowsArgumentException()
    {
        using ToolStrip toolStrip = new() { LayoutStyle = ToolStripLayoutStyle.Table };
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
        TableLayoutStyleCollection collection = settings.RowStyles;
        RowStyle style = new();
        collection.Add(style);
        Assert.Throws<ArgumentException>("style", () => collection.Add(style));
        Assert.Throws<ArgumentException>("style", () => ((IList)collection).Add(style));
    }

    [WinFormsFact]
    public void TableLayoutStyleCollection_Insert_Object_Success()
    {
        using ToolStrip toolStrip = new() { LayoutStyle = ToolStripLayoutStyle.Table };
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
        IList collection = settings.RowStyles;

        RowStyle style = new();
        collection.Insert(0, style);
        Assert.Equal(style, Assert.Single(collection));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    public void TableLayoutStyleCollection_Insert_Null_ThrowsArgumentNullException(int index)
    {
        using ToolStrip toolStrip = new() { LayoutStyle = ToolStripLayoutStyle.Table };
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
        TableLayoutStyleCollection collection = settings.RowStyles;
        Assert.Throws<ArgumentNullException>("style", () => ((IList)collection).Insert(index, null));
    }

    [WinFormsFact]
    public void TableLayoutStyleCollection_Insert_ObjectNotTableLayoutStyle_ArgumentException()
    {
        using ToolStrip toolStrip = new() { LayoutStyle = ToolStripLayoutStyle.Table };
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
        IList collection = settings.RowStyles;
        Assert.Throws<ArgumentException>(() => collection.Insert(0, new object()));
    }

    [WinFormsFact]
    public void TableLayoutStyleCollection_Insert_StyleAlreadyAdded_ThrowsArgumentException()
    {
        using ToolStrip toolStrip = new() { LayoutStyle = ToolStripLayoutStyle.Table };
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
        TableLayoutStyleCollection collection = settings.RowStyles;
        RowStyle style = new();
        collection.Add(style);
        Assert.Throws<ArgumentException>("style", () => ((IList)collection).Insert(0, style));
    }

    [WinFormsFact]
    public void TableLayoutStyleCollection_Item_SetTableLayoutStyle_GetReturnsExpected()
    {
        using ToolStrip toolStrip = new() { LayoutStyle = ToolStripLayoutStyle.Table };
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
        TableLayoutStyleCollection collection = settings.RowStyles;
        collection.Add(new RowStyle());

        RowStyle style = new();
        collection[0] = style;
        Assert.Single(collection);
        Assert.Equal(style, collection[0]);
    }

    [WinFormsFact]
    public void TableLayoutStyleCollection_Item_SetObject_GetReturnsExpected()
    {
        using ToolStrip toolStrip = new() { LayoutStyle = ToolStripLayoutStyle.Table };
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
        IList collection = settings.RowStyles;
        collection.Add(new RowStyle());

        RowStyle style = new();
        collection[0] = style;
        Assert.Single(collection);
        Assert.Equal(style, collection[0]);
    }

    [WinFormsTheory]
    [BoolData]
    public void TableLayoutStyleCollection_Item_Replace_ReleasesPreviousStyle(bool columns)
    {
        using TableLayoutPanel first = new();
        using TableLayoutPanel second = new();
        TableLayoutStyleCollection collection = columns ? first.ColumnStyles : first.RowStyles;
        TableLayoutStyleCollection otherCollection = columns ? second.ColumnStyles : second.RowStyles;
        TableLayoutStyle previous = columns ? new ColumnStyle() : new RowStyle();
        TableLayoutStyle replacement = columns ? new ColumnStyle() : new RowStyle();
        collection.Add(previous);

        collection[0] = replacement;

        int firstLayoutCount = 0;
        int secondLayoutCount = 0;
        first.Layout += (sender, e) => firstLayoutCount++;
        previous.SizeType = SizeType.Absolute;
        Assert.Equal(0, firstLayoutCount);
        otherCollection.Add(previous);
        second.Layout += (sender, e) => secondLayoutCount++;

        replacement.SizeType = SizeType.Absolute;
        previous.SizeType = SizeType.Percent;

        Assert.Equal(1, firstLayoutCount);
        Assert.Equal(1, secondLayoutCount);
        Assert.Same(replacement, Assert.Single(collection));
        Assert.Same(previous, Assert.Single(otherCollection));
        Assert.False(first.IsHandleCreated);
        Assert.False(second.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void TableLayoutStyleCollection_Item_ReplaceDuplicate_ReleasesStyleAfterLastReplacement(bool columns)
    {
        TypeConverter converter = TypeDescriptor.GetConverter(typeof(TableLayoutSettings));
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFromInvariantString("<TableLayoutSettings />"));
        TableLayoutStyleCollection styles = columns ? settings.ColumnStyles : settings.RowStyles;
        TableLayoutStyle shared = columns ? new ColumnStyle() : new RowStyle();
        styles.Add(shared);
        styles.Add(shared);

        using TableLayoutPanel owner = new() { LayoutSettings = settings };
        using TableLayoutPanel other = new();
        TableLayoutStyleCollection collection = columns ? owner.ColumnStyles : owner.RowStyles;
        TableLayoutStyleCollection otherCollection = columns ? other.ColumnStyles : other.RowStyles;

        collection[0] = columns ? new ColumnStyle() : new RowStyle();

        Assert.Same(shared, collection[1]);
        int layoutCount = 0;
        owner.Layout += (sender, e) => layoutCount++;
        shared.SizeType = SizeType.Absolute;
        Assert.Equal(1, layoutCount);
        Assert.Throws<ArgumentException>("style", () => otherCollection.Add(shared));
        Assert.Empty(otherCollection);

        collection[1] = columns ? new ColumnStyle() : new RowStyle();

        layoutCount = 0;
        shared.SizeType = SizeType.Percent;
        Assert.Equal(0, layoutCount);
        otherCollection.Add(shared);
        Assert.Same(shared, Assert.Single(otherCollection));
    }

    [WinFormsTheory]
    [BoolData]
    public void TableLayoutStyleCollection_Item_ReplaceInUnownedCollection_PreservesOtherOwner(bool columns)
    {
        TypeConverter converter = TypeDescriptor.GetConverter(typeof(TableLayoutSettings));
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFromInvariantString("<TableLayoutSettings />"));
        using TableLayoutPanel owner = new();
        using TableLayoutPanel other = new();
        TableLayoutStyleCollection unownedCollection = columns ? settings.ColumnStyles : settings.RowStyles;
        TableLayoutStyleCollection ownedCollection = columns ? owner.ColumnStyles : owner.RowStyles;
        TableLayoutStyleCollection otherCollection = columns ? other.ColumnStyles : other.RowStyles;
        TableLayoutStyle previous = columns ? new ColumnStyle() : new RowStyle();
        TableLayoutStyle replacement = columns ? new ColumnStyle() : new RowStyle();
        unownedCollection.Add(previous);
        ownedCollection.Add(previous);

        unownedCollection[0] = replacement;

        int layoutCount = 0;
        owner.Layout += (sender, e) => layoutCount++;
        previous.SizeType = SizeType.Absolute;

        Assert.Equal(1, layoutCount);
        Assert.Same(replacement, Assert.Single(unownedCollection));
        Assert.Same(previous, Assert.Single(ownedCollection));
        Assert.Throws<ArgumentException>("style", () => otherCollection.Add(previous));
        Assert.Empty(otherCollection);
        Assert.False(owner.IsHandleCreated);
        Assert.False(other.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(false, -1)]
    [InlineData(false, 1)]
    [InlineData(false, int.MaxValue)]
    [InlineData(true, -1)]
    [InlineData(true, 1)]
    [InlineData(true, int.MaxValue)]
    public void TableLayoutStyleCollection_Insert_InvalidIndex_DoesNotTakeOwnership(bool columns, int index)
    {
        using TableLayoutPanel first = new();
        using TableLayoutPanel second = new();
        TableLayoutStyleCollection collection = columns ? first.ColumnStyles : first.RowStyles;
        TableLayoutStyleCollection otherCollection = columns ? second.ColumnStyles : second.RowStyles;
        TableLayoutStyle style = columns ? new ColumnStyle() : new RowStyle();

        Assert.Throws<ArgumentOutOfRangeException>("index", () => ((IList)collection).Insert(index, style));

        int layoutCount = 0;
        first.Layout += (sender, e) => layoutCount++;
        style.SizeType = SizeType.Absolute;
        Assert.Equal(0, layoutCount);
        otherCollection.Add(style);

        Assert.Empty(collection);
        Assert.Same(style, Assert.Single(otherCollection));
        Assert.False(first.IsHandleCreated);
        Assert.False(second.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(false, -1)]
    [InlineData(false, 0)]
    [InlineData(false, int.MaxValue)]
    [InlineData(true, -1)]
    [InlineData(true, 0)]
    [InlineData(true, int.MaxValue)]
    public void TableLayoutStyleCollection_Item_InvalidIndex_DoesNotTakeOwnership(bool columns, int index)
    {
        using TableLayoutPanel first = new();
        using TableLayoutPanel second = new();
        TableLayoutStyleCollection collection = columns ? first.ColumnStyles : first.RowStyles;
        TableLayoutStyleCollection otherCollection = columns ? second.ColumnStyles : second.RowStyles;
        TableLayoutStyle style = columns ? new ColumnStyle() : new RowStyle();

        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index] = style);

        int layoutCount = 0;
        first.Layout += (sender, e) => layoutCount++;
        style.SizeType = SizeType.Absolute;
        Assert.Equal(0, layoutCount);
        otherCollection.Add(style);

        Assert.Empty(collection);
        Assert.Same(style, Assert.Single(otherCollection));
        Assert.False(first.IsHandleCreated);
        Assert.False(second.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    public void TableLayoutStyleCollection_Item_SetNull_ThrowsArgumentNullException(int index)
    {
        using ToolStrip toolStrip = new() { LayoutStyle = ToolStripLayoutStyle.Table };
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
        TableLayoutStyleCollection collection = settings.RowStyles;
        collection.Add(new RowStyle());
        Assert.Throws<ArgumentNullException>("value", () => collection[index] = null);
        Assert.Throws<ArgumentNullException>("value", () => ((IList)collection)[index] = null);
    }

    [WinFormsFact]
    public void TableLayoutStyleCollection_Item_ObjectNotTableLayoutStyle_ThrowsArgumentException()
    {
        using ToolStrip toolStrip = new() { LayoutStyle = ToolStripLayoutStyle.Table };
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
        IList collection = settings.RowStyles;
        collection.Add(new RowStyle());
        Assert.Throws<ArgumentException>(() => collection[0] = new object());
    }

    [WinFormsFact]
    public void TableLayoutStyleCollection_Item_StyleAlreadyAdded_ThrowsArgumentException()
    {
        using ToolStrip toolStrip = new() { LayoutStyle = ToolStripLayoutStyle.Table };
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
        TableLayoutStyleCollection collection = settings.RowStyles;
        RowStyle style = new();
        collection.Add(style);
        Assert.Throws<ArgumentException>("style", () => collection[0] = style);
        Assert.Throws<ArgumentException>("style", () => ((IList)collection)[0] = style);
    }

    [WinFormsFact]
    public void TableLayoutStyleCollection_Remove_Object_Success()
    {
        using ToolStrip toolStrip = new() { LayoutStyle = ToolStripLayoutStyle.Table };
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
        IList collection = settings.RowStyles;
        RowStyle style = new();
        collection.Add(style);
        collection.Remove(style);
        Assert.Empty(collection);

        collection.Add(style);
        Assert.Equal(style, Assert.Single(collection));
    }

    [WinFormsFact]
    public void TableLayoutStyleCollection_Remove_Null_Nop()
    {
        using ToolStrip toolStrip = new() { LayoutStyle = ToolStripLayoutStyle.Table };
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
        IList collection = settings.RowStyles;
        RowStyle style = new();
        collection.Add(style);
        collection.Remove(null);
        Assert.Same(style, Assert.Single(collection));
    }

    [WinFormsFact]
    public void TableLayoutStyleCollection_Remove_ObjectNotTableLayoutStyle_ThrowsArgumentException()
    {
        using ToolStrip toolStrip = new() { LayoutStyle = ToolStripLayoutStyle.Table };
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
        IList collection = settings.RowStyles;
        Assert.Throws<ArgumentException>(() => collection.Remove(new object()));
    }

    [WinFormsFact]
    public void TableLayoutStyleCollection_RemoveAt_Invoke_Success()
    {
        using ToolStrip toolStrip = new() { LayoutStyle = ToolStripLayoutStyle.Table };
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
        TableLayoutStyleCollection collection = settings.RowStyles;
        RowStyle style = new();
        collection.Add(style);
        collection.RemoveAt(0);
        Assert.Empty(collection);

        collection.Add(style);
        Assert.Equal(style, Assert.Single(collection));
    }

    [WinFormsFact]
    public void TableLayoutStyleCollection_Clear_Invoke_Success()
    {
        using ToolStrip toolStrip = new() { LayoutStyle = ToolStripLayoutStyle.Table };
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
        TableLayoutStyleCollection collection = settings.RowStyles;
        RowStyle style = new();
        collection.Add(style);
        collection.Clear();
        Assert.Empty(collection);

        collection.Clear();
        Assert.Empty(collection);

        collection.Add(style);
        Assert.Equal(style, Assert.Single(collection));
    }

    [WinFormsFact]
    public void TableLayoutStyleCollection_Contains_Object_ReturnsExpected()
    {
        using ToolStrip toolStrip = new() { LayoutStyle = ToolStripLayoutStyle.Table };
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
        IList collection = settings.RowStyles;
        RowStyle style = new();
        collection.Add(style);
        Assert.True(collection.Contains(style));
        Assert.False(collection.Contains(new ColumnStyle()));
        Assert.False(collection.Contains(new RowStyle()));
        Assert.False(collection.Contains(null));
    }

    [WinFormsFact]
    public void TableLayoutStyleCollection_IndexOf_Object_ReturnsExpected()
    {
        using ToolStrip toolStrip = new() { LayoutStyle = ToolStripLayoutStyle.Table };
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
        IList collection = settings.RowStyles;
        RowStyle style = new();
        collection.Add(style);
        Assert.Equal(0, collection.IndexOf(style));
        Assert.Equal(-1, collection.IndexOf(new ColumnStyle()));
        Assert.Equal(-1, collection.IndexOf(new RowStyle()));
        Assert.Equal(-1, collection.IndexOf(null));
    }

    [WinFormsFact]
    public void TableLayoutStyleCollection_CopyTo_Invoke_Success()
    {
        using ToolStrip toolStrip = new() { LayoutStyle = ToolStripLayoutStyle.Table };
        TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
        IList collection = settings.RowStyles;
        RowStyle style = new();
        collection.Add(style);

        object[] array = [1, 2, 3];
        collection.CopyTo(array, 1);
        Assert.Equal([1, style, 3], array);
    }
}
