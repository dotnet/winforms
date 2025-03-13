// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
///  A collection of listview groups.
/// </summary>
[ListBindable(false)]
public class ListViewGroupCollection : IList
{
    private readonly ListView _listView;

    private List<ListViewGroup>? _list;

    internal ListViewGroupCollection(ListView listView)
    {
        _listView = listView;
    }

    public int Count => List.Count;

    object ICollection.SyncRoot => this;

    bool ICollection.IsSynchronized => true;

    bool IList.IsFixedSize => false;

    bool IList.IsReadOnly => false;

    private List<ListViewGroup> List => _list ??= [];

    public ListViewGroup this[int index]
    {
        get => List[index];
        set
        {
            ArgumentNullException.ThrowIfNull(value);

            if (List.Contains(value))
            {
                return;
            }

            CheckListViewItems(value);
            value.ListView = _listView;
            List[index] = value;
        }
    }

    public ListViewGroup? this[string key]
    {
        get
        {
            if (_list is null)
            {
                return null;
            }

            for (int i = 0; i < _list.Count; i++)
            {
                if (string.Equals(key, this[i].Name, StringComparison.CurrentCulture))
                {
                    return this[i];
                }
            }

            return null;
        }
        set
        {
            ArgumentNullException.ThrowIfNull(value);

            if (_list is null)
            {
                // nothing to do
                return;
            }

            int index = -1;
            for (int i = 0; i < _list.Count; i++)
            {
                if (string.Equals(key, this[i].Name, StringComparison.CurrentCulture))
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                _list[index] = value;
            }
        }
    }

    object? IList.this[int index]
    {
        get => this[index];
        set
        {
            if (value is ListViewGroup group)
            {
                this[index] = group;
            }
        }
    }

    public int Add(ListViewGroup group)
    {
        ArgumentNullException.ThrowIfNull(group);
        ThrowInvalidOperationExceptionIfVirtualMode();

        if (Contains(group))
        {
            return -1;
        }

        CheckListViewItems(group);
        group.ListView = _listView;
        int index = ((IList)List).Add(group);
        if (_listView.IsHandleCreated)
        {
            _listView.InsertGroupInListView(List.Count, group);
            MoveGroupItems(group);
        }

        return index;
    }

    public ListViewGroup Add(string? key, string? headerText)
    {
        ListViewGroup group = new(key, headerText);
        Add(group);
        return group;
    }

    int IList.Add(object? value)
    {
        if (value is not ListViewGroup group)
        {
            throw new ArgumentException(SR.ListViewGroupCollectionBadListViewGroup, nameof(value));
        }

        return Add(group);
    }

    public void AddRange(params ListViewGroup[] groups)
    {
        ArgumentNullException.ThrowIfNull(groups);
        ThrowInvalidOperationExceptionIfVirtualMode();

        for (int i = 0; i < groups.Length; i++)
        {
            Add(groups[i]);
        }
    }

    public void AddRange(ListViewGroupCollection groups)
    {
        ArgumentNullException.ThrowIfNull(groups);
        ThrowInvalidOperationExceptionIfVirtualMode();

        for (int i = 0; i < groups.Count; i++)
        {
            Add(groups[i]);
        }
    }

    private void CheckListViewItems(ListViewGroup group)
    {
        for (int i = 0; i < group.Items.Count; i++)
        {
            ListViewItem item = group.Items[i];
            if (item.ListView is not null && item.ListView != _listView)
            {
                throw new ArgumentException(string.Format(SR.OnlyOneControl, item.Text));
            }
        }
    }

    public void Clear()
    {
        if (_listView.IsHandleCreated)
        {
            for (int i = 0; i < Count; i++)
            {
                _listView.RemoveGroupFromListView(this[i]);
            }
        }

        // Dissociate groups from the ListView
        for (int i = 0; i < Count; i++)
        {
            this[i].ListView = null;
            this[i].ReleaseUiaProvider();
        }

        List.Clear();

        // we have to tell the listView that there are no more groups
        // so the list view knows to remove items from the default group
        _listView.UpdateGroupView();
    }

    public bool Contains(ListViewGroup value) => List.Contains(value);

    bool IList.Contains(object? value)
    {
        if (value is not ListViewGroup group)
        {
            return false;
        }

        return Contains(group);
    }

    public void CopyTo(Array array, int index) => ((ICollection)List).CopyTo(array, index);

    public IEnumerator GetEnumerator() => List.GetEnumerator();

    public int IndexOf(ListViewGroup value) => List.IndexOf(value);

    int IList.IndexOf(object? value)
    {
        if (value is not ListViewGroup group)
        {
            return -1;
        }

        return IndexOf(group);
    }

    public void Insert(int index, ListViewGroup group)
    {
        ArgumentNullException.ThrowIfNull(group);
        ThrowInvalidOperationExceptionIfVirtualMode();

        if (Contains(group))
        {
            return;
        }

        CheckListViewItems(group);
        group.ListView = _listView;
        List.Insert(index, group);
        if (_listView.IsHandleCreated)
        {
            _listView.InsertGroupInListView(index, group);
            MoveGroupItems(group);
        }
    }

    void IList.Insert(int index, object? value)
    {
        if (value is ListViewGroup group)
        {
            Insert(index, group);
        }
    }

    private void MoveGroupItems(ListViewGroup group)
    {
        Debug.Assert(_listView.IsHandleCreated, "MoveGroupItems pre-condition: listView handle must be created");

        foreach (ListViewItem item in group.Items)
        {
            if (item.ListView == _listView)
            {
                item.UpdateStateToListView(item.Index);
            }
        }
    }

    public void Remove(ListViewGroup group)
    {
        if (!List.Remove(group))
        {
            return;
        }

        group.ListView = null;
        group.ReleaseUiaProvider();

        if (_listView.IsHandleCreated)
        {
            _listView.RemoveGroupFromListView(group);
        }
    }

    void IList.Remove(object? value)
    {
        if (value is ListViewGroup group)
        {
            Remove(group);
        }
    }

    public void RemoveAt(int index) => Remove(this[index]);

    private void ThrowInvalidOperationExceptionIfVirtualMode()
    {
        if (_listView.VirtualMode)
        {
            throw new InvalidOperationException(SR.ListViewCannotAddGroupsToVirtualListView);
        }
    }
}
