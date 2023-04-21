﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
///  Represents a collection of strings.
/// </summary>
public class AutoCompleteStringCollection : IList
{
    private CollectionChangeEventHandler onCollectionChanged;
    private readonly List<string> data = new();

    public AutoCompleteStringCollection()
    {
    }

    /// <summary>
    ///  Represents the entry at the specified index of the <see cref="AutoCompleteStringCollection"/>.
    /// </summary>
    public string this[int index]
    {
        get
        {
            return data[index];
        }
        set
        {
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, data[index]));
            data[index] = value;
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
        }
    }

    /// <summary>
    ///  Gets the number of strings in the
    ///  <see cref="AutoCompleteStringCollection"/> .
    /// </summary>
    public int Count => data.Count;

    bool IList.IsReadOnly => ((IList)data).IsReadOnly;

    bool IList.IsFixedSize => ((IList)data).IsFixedSize;

    public event CollectionChangeEventHandler CollectionChanged
    {
        add => onCollectionChanged += value;
        remove => onCollectionChanged -= value;
    }

    protected void OnCollectionChanged(CollectionChangeEventArgs e) => onCollectionChanged?.Invoke(this, e);

    /// <summary>
    ///  Adds a string with the specified value to the
    ///  <see cref="AutoCompleteStringCollection"/> .
    /// </summary>
    public int Add(string value)
    {
        int index = ((IList)data).Add(value);
        OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
        return index;
    }

    /// <summary>
    ///  Copies the elements of a string array to the end of the <see cref="AutoCompleteStringCollection"/>.
    /// </summary>
    public void AddRange(string[] value)
    {
        ArgumentNullException.ThrowIfNull(value);

        data.AddRange(value);
        OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
    }

    /// <summary>
    ///  Removes all the strings from the
    ///  <see cref="AutoCompleteStringCollection"/> .
    /// </summary>
    public void Clear()
    {
        data.Clear();
        OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
    }

    /// <summary>
    ///  Gets a value indicating whether the
    ///  <see cref="AutoCompleteStringCollection"/> contains a string with the specified
    ///  value.
    /// </summary>
    public bool Contains(string value) => data.Contains(value);

    /// <summary>
    ///  Copies the <see cref="AutoCompleteStringCollection"/> values to a one-dimensional <see cref="Array"/> instance at the
    ///  specified index.
    /// </summary>
    public void CopyTo(string[] array, int index) => data.CopyTo(array, index);

    /// <summary>
    ///  Returns the index of the first occurrence of a string in
    ///  the <see cref="AutoCompleteStringCollection"/> .
    /// </summary>
    public int IndexOf(string value) => data.IndexOf(value);

    /// <summary>
    ///  Inserts a string into the <see cref="AutoCompleteStringCollection"/> at the specified
    ///  index.
    /// </summary>
    public void Insert(int index, string value)
    {
        data.Insert(index, value);
        OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
    }

    /// <summary>
    ///  Gets a value indicating whether the <see cref="AutoCompleteStringCollection"/> is read-only.
    /// </summary>
    public bool IsReadOnly => ((IList)data).IsReadOnly;

    /// <summary>
    ///  Gets a value indicating whether access to the <see cref="AutoCompleteStringCollection"/>
    ///  is synchronized (thread-safe).
    /// </summary>
    public bool IsSynchronized => ((IList)data).IsSynchronized;

    /// <summary>
    ///  Removes a specific string from the <see cref="AutoCompleteStringCollection"/> .
    /// </summary>
    public void Remove(string value)
    {
        data.Remove(value);
        OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, value));
    }

    /// <summary>
    ///  Removes the string at the specified index of the <see cref="AutoCompleteStringCollection"/>.
    /// </summary>
    public void RemoveAt(int index)
    {
        string value = data[index];
        data.RemoveAt(index);
        OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, value));
    }

    /// <summary>
    ///  Gets an object that can be used to synchronize access to the <see cref="AutoCompleteStringCollection"/>.
    /// </summary>
    public object SyncRoot => ((IList)data).SyncRoot;

    object IList.this[int index]
    {
        get => this[index];
        set => this[index] = (string)value;
    }

    int IList.Add(object value) => Add((string)value);

    bool IList.Contains(object value) => Contains((string)value);

    int IList.IndexOf(object value) => IndexOf((string)value);

    void IList.Insert(int index, object value) => Insert(index, (string)value);

    void IList.Remove(object value) => Remove((string)value);

    void ICollection.CopyTo(Array array, int index) => ((ICollection)data).CopyTo(array, index);

    public IEnumerator GetEnumerator() => data.GetEnumerator();
}
