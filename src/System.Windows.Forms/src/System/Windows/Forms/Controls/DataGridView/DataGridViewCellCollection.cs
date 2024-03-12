// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
///  Represents a collection of <see cref="DataGridViewCell"/> objects in the <see cref="DataGridView"/>
///  control.
/// </summary>
[ListBindable(false)]
public class DataGridViewCellCollection : BaseCollection, IList
{
    private CollectionChangeEventHandler? _onCollectionChanged;
    private readonly List<DataGridViewCell> _items = [];
    private readonly DataGridViewRow _owner;

    int IList.Add(object? value) => Add((DataGridViewCell)value!);

    void IList.Clear() => Clear();

    bool IList.Contains(object? value) => ((IList)_items).Contains(value);

    int IList.IndexOf(object? value) => ((IList)_items).IndexOf(value);

    void IList.Insert(int index, object? value) => Insert(index, (DataGridViewCell)value!);

    void IList.Remove(object? value) => Remove((DataGridViewCell)value!);

    void IList.RemoveAt(int index) => RemoveAt(index);

    bool IList.IsFixedSize => ((IList)_items).IsFixedSize;

    bool IList.IsReadOnly => ((IList)_items).IsReadOnly;

    object? IList.this[int index]
    {
        get { return this[index]; }
        set { this[index] = (DataGridViewCell)value!; }
    }

    void ICollection.CopyTo(Array array, int index) => ((ICollection)_items).CopyTo(array, index);

    int ICollection.Count => _items.Count;

    bool ICollection.IsSynchronized => ((ICollection)_items).IsSynchronized;

    object ICollection.SyncRoot => ((ICollection)_items).SyncRoot;

    IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

    public DataGridViewCellCollection(DataGridViewRow dataGridViewRow)
    {
        Debug.Assert(dataGridViewRow is not null);
        _owner = dataGridViewRow;
    }

    protected override ArrayList List => ArrayList.Adapter(_items);

    /// <summary>
    ///  Retrieves the DataGridViewCell with the specified index.
    /// </summary>
    public DataGridViewCell this[int index]
    {
        get => _items[index];
        set
        {
            DataGridViewCell dataGridViewCell = value.OrThrowIfNull();

            if (dataGridViewCell.DataGridView is not null)
            {
                throw new InvalidOperationException(SR.DataGridViewCellCollection_CellAlreadyBelongsToDataGridView);
            }

            if (dataGridViewCell.OwningRow is not null)
            {
                throw new InvalidOperationException(SR.DataGridViewCellCollection_CellAlreadyBelongsToDataGridViewRow);
            }

            _owner.DataGridView?.OnReplacingCell(_owner, index);

            DataGridViewCell oldDataGridViewCell = _items[index];
            _items[index] = dataGridViewCell;
            dataGridViewCell.OwningRow = _owner;
            dataGridViewCell.State = oldDataGridViewCell.State;
            if (_owner.DataGridView is not null)
            {
                dataGridViewCell.DataGridView = _owner.DataGridView;
                dataGridViewCell.OwningColumn = _owner.DataGridView.Columns[index];
                _owner.DataGridView.OnReplacedCell(_owner, index);
            }

            oldDataGridViewCell.DataGridView = null;
            oldDataGridViewCell.OwningRow = null;
            oldDataGridViewCell.OwningColumn = null;
            if (oldDataGridViewCell.ReadOnly)
            {
                oldDataGridViewCell.ReadOnlyInternal = false;
            }

            if (oldDataGridViewCell.Selected)
            {
                oldDataGridViewCell.SelectedInternal = false;
            }
        }
    }

    /// <summary>
    ///  Retrieves the DataGridViewCell with the specified column name.
    /// </summary>
    public DataGridViewCell this[string columnName]
    {
        get
        {
            DataGridViewColumn? dataGridViewColumn = null;
            if (_owner.DataGridView is not null)
            {
                dataGridViewColumn = _owner.DataGridView.Columns[columnName];
            }

            if (dataGridViewColumn is null)
            {
                throw new ArgumentException(string.Format(SR.DataGridViewColumnCollection_ColumnNotFound, columnName), nameof(columnName));
            }

            return _items[dataGridViewColumn.Index];
        }
        set
        {
            DataGridViewColumn? dataGridViewColumn = null;
            if (_owner.DataGridView is not null)
            {
                dataGridViewColumn = _owner.DataGridView.Columns[columnName];
            }

            if (dataGridViewColumn is null)
            {
                throw new ArgumentException(string.Format(SR.DataGridViewColumnCollection_ColumnNotFound, columnName), nameof(columnName));
            }

            this[dataGridViewColumn.Index] = value;
        }
    }

    public event CollectionChangeEventHandler? CollectionChanged
    {
        add => _onCollectionChanged += value;
        remove => _onCollectionChanged -= value;
    }

    /// <summary>
    ///  Adds a <see cref="DataGridViewCell"/> to this collection.
    /// </summary>
    public virtual int Add(DataGridViewCell dataGridViewCell)
    {
        if (_owner.DataGridView is not null)
        {
            throw new InvalidOperationException(SR.DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView);
        }

        if (dataGridViewCell.OwningRow is not null)
        {
            throw new InvalidOperationException(SR.DataGridViewCellCollection_CellAlreadyBelongsToDataGridViewRow);
        }

        return AddInternal(dataGridViewCell);
    }

    internal int AddInternal(DataGridViewCell dataGridViewCell)
    {
        Debug.Assert(!dataGridViewCell.Selected);

        int index = ((IList)_items).Add(dataGridViewCell);
        dataGridViewCell.OwningRow = _owner;
        DataGridView? dataGridView = _owner.DataGridView;
        if (dataGridView is not null && dataGridView.Columns.Count > index)
        {
            dataGridViewCell.OwningColumn = dataGridView.Columns[index];
        }

        OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, dataGridViewCell));
        return index;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual void AddRange(params DataGridViewCell[] dataGridViewCells)
    {
        ArgumentNullException.ThrowIfNull(dataGridViewCells);

        if (_owner.DataGridView is not null)
        {
            throw new InvalidOperationException(SR.DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView);
        }

        foreach (DataGridViewCell dataGridViewCell in dataGridViewCells)
        {
            if (dataGridViewCell is null)
            {
                throw new InvalidOperationException(SR.DataGridViewCellCollection_AtLeastOneCellIsNull);
            }

            if (dataGridViewCell.OwningRow is not null)
            {
                throw new InvalidOperationException(SR.DataGridViewCellCollection_CellAlreadyBelongsToDataGridViewRow);
            }
        }

        // Make sure no two cells are identical
        int cellCount = dataGridViewCells.Length;
        for (int cell1 = 0; cell1 < cellCount - 1; cell1++)
        {
            for (int cell2 = cell1 + 1; cell2 < cellCount; cell2++)
            {
                if (dataGridViewCells[cell1] == dataGridViewCells[cell2])
                {
                    throw new InvalidOperationException(SR.DataGridViewCellCollection_CannotAddIdenticalCells);
                }
            }
        }

        _items.AddRange(dataGridViewCells);
        foreach (DataGridViewCell dataGridViewCell in dataGridViewCells)
        {
            dataGridViewCell.OwningRow = _owner;
            Debug.Assert(!dataGridViewCell.Selected);
        }

        OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
    }

    public virtual void Clear()
    {
        if (_owner.DataGridView is not null)
        {
            throw new InvalidOperationException(SR.DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView);
        }

        foreach (DataGridViewCell dataGridViewCell in _items)
        {
            dataGridViewCell.OwningRow = null;
        }

        _items.Clear();
        OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
    }

    public void CopyTo(DataGridViewCell[] array, int index)
    {
        _items.CopyTo(array, index);
    }

    /// <summary>
    ///  Checks to see if a DataGridViewCell is contained in this collection.
    /// </summary>
    public virtual bool Contains(DataGridViewCell dataGridViewCell)
    {
        int index = _items.IndexOf(dataGridViewCell);
        return index != -1;
    }

    public int IndexOf(DataGridViewCell dataGridViewCell) => _items.IndexOf(dataGridViewCell);

    public virtual void Insert(int index, DataGridViewCell dataGridViewCell)
    {
        if (_owner.DataGridView is not null)
        {
            throw new InvalidOperationException(SR.DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView);
        }

        if (dataGridViewCell.OwningRow is not null)
        {
            throw new InvalidOperationException(SR.DataGridViewCellCollection_CellAlreadyBelongsToDataGridViewRow);
        }

        Debug.Assert(!dataGridViewCell.ReadOnly);
        Debug.Assert(!dataGridViewCell.Selected);
        _items.Insert(index, dataGridViewCell);
        dataGridViewCell.OwningRow = _owner;
        OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, dataGridViewCell));
    }

    internal void InsertInternal(int index, DataGridViewCell dataGridViewCell)
    {
        Debug.Assert(!dataGridViewCell.Selected);
        _items.Insert(index, dataGridViewCell);
        dataGridViewCell.OwningRow = _owner;
        DataGridView? dataGridView = _owner.DataGridView;
        if (dataGridView is not null && dataGridView.Columns.Count > index)
        {
            dataGridViewCell.OwningColumn = dataGridView.Columns[index];
        }

        OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, dataGridViewCell));
    }

    protected void OnCollectionChanged(CollectionChangeEventArgs e)
    {
        _onCollectionChanged?.Invoke(this, e);
    }

    public virtual void Remove(DataGridViewCell cell)
    {
        if (_owner.DataGridView is not null)
        {
            throw new InvalidOperationException(SR.DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView);
        }

        int cellIndex = -1;
        int itemsCount = _items.Count;
        for (int i = 0; i < itemsCount; ++i)
        {
            if (_items[i] == cell)
            {
                cellIndex = i;
                break;
            }
        }

        if (cellIndex == -1)
        {
            throw new ArgumentException(SR.DataGridViewCellCollection_CellNotFound);
        }
        else
        {
            RemoveAt(cellIndex);
        }
    }

    public virtual void RemoveAt(int index)
    {
        if (_owner.DataGridView is not null)
        {
            throw new InvalidOperationException(SR.DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView);
        }

        RemoveAtInternal(index);
    }

    internal void RemoveAtInternal(int index)
    {
        DataGridViewCell dataGridViewCell = _items[index];
        _items.RemoveAt(index);
        dataGridViewCell.DataGridView = null;
        dataGridViewCell.OwningRow = null;
        if (dataGridViewCell.ReadOnly)
        {
            dataGridViewCell.ReadOnlyInternal = false;
        }

        if (dataGridViewCell.Selected)
        {
            dataGridViewCell.SelectedInternal = false;
        }

        OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, dataGridViewCell));
    }
}
