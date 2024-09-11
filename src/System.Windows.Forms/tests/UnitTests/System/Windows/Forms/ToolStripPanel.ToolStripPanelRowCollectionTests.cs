// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using static System.Windows.Forms.ToolStripPanel;

namespace System.Windows.Forms.Tests;

public class ToolStripPanel_ToolStripPanelRowCollectionTests : IDisposable
{
    private readonly ToolStripPanel _toolStripPanel;
    private readonly ToolStripPanelRowCollection _toolStripPanelRowCollection;

    public ToolStripPanel_ToolStripPanelRowCollectionTests()
    {
        _toolStripPanel = new();
        _toolStripPanelRowCollection = new(_toolStripPanel);
    }

    public void Dispose() => _toolStripPanel.Dispose();

    [WinFormsFact]
    public void ToolStripPanelRowCollection_ConstructorWithOwnerAndValue_AddsRows()
    {
        var rows = AddRowsToCollection(_toolStripPanelRowCollection, _toolStripPanel, 2);

        _toolStripPanelRowCollection.Count.Should().Be(2);
        _toolStripPanelRowCollection[0].Should().Be(rows[0]);
        _toolStripPanelRowCollection[1].Should().Be(rows[1]);
    }

    [WinFormsFact]
    public void ToolStripPanelRowCollection_Add_AddsRow()
    {
        var row = AddRowToCollection(_toolStripPanelRowCollection, _toolStripPanel);

        _toolStripPanelRowCollection.Count.Should().Be(1);
        _toolStripPanelRowCollection[0].Should().Be(row);
    }

    [WinFormsFact]
    public void ToolStripPanelRowCollection_Add_NullValue_ThrowsArgumentNullException()
    {
        Action action = () => _toolStripPanelRowCollection.Add(null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [WinFormsFact]
    public void ToolStripPanelRowCollection_AddRange_AddsRows()
    {
        var rows = AddRowsToCollection(_toolStripPanelRowCollection, _toolStripPanel, 2);

        _toolStripPanelRowCollection.Count.Should().Be(2);
        _toolStripPanelRowCollection[0].Should().Be(rows[0]);
        _toolStripPanelRowCollection[1].Should().Be(rows[1]);
    }

    [WinFormsFact]
    public void ToolStripPanelRowCollection_AddRange_NullValue_ThrowsArgumentNullException()
    {
        Action action = () => _toolStripPanelRowCollection.AddRange((ToolStripPanelRow[])null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [WinFormsFact]
    public void ToolStripPanelRowCollection_AddRange_Collection_AddsRows()
    {
        var rows = AddRowsToCollection(_toolStripPanelRowCollection, _toolStripPanel, 2);

        _toolStripPanelRowCollection.AddRange(new ToolStripPanelRowCollection(_toolStripPanel) { rows[0], rows[1] });

        _toolStripPanelRowCollection.Count.Should().Be(4);
        _toolStripPanelRowCollection[2].Should().Be(rows[0]);
        _toolStripPanelRowCollection[3].Should().Be(rows[1]);
    }

    [WinFormsFact]
    public void ToolStripPanelRowCollection_AddRange_Collection_NullValue_ThrowsArgumentNullException()
    {
        Action action = () => _toolStripPanelRowCollection.AddRange((ToolStripPanelRowCollection)null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [WinFormsFact]
    public void ToolStripPanelRowCollection_Clear_RemovesAllRows()
    {
        AddRowsToCollection(_toolStripPanelRowCollection, _toolStripPanel, 2);

        _toolStripPanelRowCollection.Clear();

        _toolStripPanelRowCollection.Count.Should().Be(0);
    }

    [WinFormsFact]
    public void ToolStripPanelRowCollection_Clear_EmptyCollection_DoesNothing()
    {
        _toolStripPanelRowCollection.Clear();

        _toolStripPanelRowCollection.Count.Should().Be(0);
    }

    [WinFormsFact]
    public void ToolStripPanelRowCollection_Remove_RemovesRow()
    {
        var row = AddRowToCollection(_toolStripPanelRowCollection, _toolStripPanel);

        _toolStripPanelRowCollection.Remove(row);

        _toolStripPanelRowCollection.Count.Should().Be(0);
    }

    [WinFormsFact]
    public void ToolStripPanelRowCollection_Remove_RowNotInCollection_DoesNothing()
    {
        var row1 = AddRowToCollection(_toolStripPanelRowCollection, _toolStripPanel);
        ToolStripPanelRow row2 = new(_toolStripPanel);

        _toolStripPanelRowCollection.Remove(row2);

        _toolStripPanelRowCollection.Count.Should().Be(1);
        _toolStripPanelRowCollection[0].Should().Be(row1);
    }

    [WinFormsFact]
    public void ToolStripPanelRowCollection_RemoveAt_RemovesRowAtIndex()
    {
        AddRowToCollection(_toolStripPanelRowCollection, _toolStripPanel);
        var row2 = AddRowToCollection(_toolStripPanelRowCollection, _toolStripPanel);

        _toolStripPanelRowCollection.RemoveAt(0);

        _toolStripPanelRowCollection.Count.Should().Be(1);
        _toolStripPanelRowCollection[0].Should().Be(row2);
    }

    [WinFormsFact]
    public void ToolStripPanelRowCollection_RemoveAt_IndexOutOfRange_ThrowsArgumentOutOfRangeException()
    {
        var row = AddRowToCollection(_toolStripPanelRowCollection, _toolStripPanel);

        Action action = () => _toolStripPanelRowCollection.RemoveAt(1);

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [WinFormsFact]
    public void ToolStripPanelRowCollection_RemoveAt_EmptyCollection_ThrowsArgumentOutOfRangeException()
    {
        Action action = () => _toolStripPanelRowCollection.RemoveAt(0);

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [WinFormsFact]
    public void ToolStripPanelRowCollection_CopyTo_CopiesRowsToArray()
    {
        var rows = AddRowsToCollection(_toolStripPanelRowCollection, _toolStripPanel, 2);
        var array = new ToolStripPanelRow[2];

        _toolStripPanelRowCollection.CopyTo(array, 0);

        array[0].Should().Be(rows[0]);
        array[1].Should().Be(rows[1]);
    }

    [WinFormsFact]
    public void ToolStripPanelRowCollection_CopyTo_NullArray_ThrowsArgumentNullException()
    {
        Action action = () => _toolStripPanelRowCollection.CopyTo(null!, 0);

        action.Should().Throw<ArgumentNullException>();
    }

    [WinFormsFact]
    public void ToolStripPanelRowCollection_CopyTo_IndexOutOfRange_ThrowsArgumentException()
    {
        var row = AddRowToCollection(_toolStripPanelRowCollection, _toolStripPanel);
        var array = new ToolStripPanelRow[1];

        Action act = () => _toolStripPanelRowCollection.CopyTo(array, 1);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Destination array was not long enough. Check the destination index, length, and the array's lower bounds. (Parameter 'destinationArray')");
    }

    [WinFormsFact]
    public void ToolStripPanelRowCollection_CopyTo_ArrayTooSmall_ThrowsArgumentException()
    {
        var rows = AddRowsToCollection(_toolStripPanelRowCollection, _toolStripPanel, 2);
        var array = new ToolStripPanelRow[1];

        Action action = () => _toolStripPanelRowCollection.CopyTo(array, 0);

        action.Should().Throw<ArgumentException>();
    }

    private ToolStripPanelRow AddRowToCollection(ToolStripPanelRowCollection collection, ToolStripPanel panel)
    {
        ToolStripPanelRow row = new(panel);
        collection.Add(row);
        return row;
    }

    private ToolStripPanelRow[] AddRowsToCollection(ToolStripPanelRowCollection collection, ToolStripPanel panel, int count)
    {
        var rows = new ToolStripPanelRow[count];
        for (int i = 0; i < count; i++)
        {
            rows[i] = new ToolStripPanelRow(panel);
            collection.Add(rows[i]);
        }

        return rows;
    }
}
