// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

[Editor($"System.Windows.Forms.Design.StyleCollectionEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
public abstract class TableLayoutStyleCollection : IList
{
    private IArrangedElement? _owner;
    private readonly List<TableLayoutStyle> _innerList = [];

    internal TableLayoutStyleCollection(IArrangedElement? owner)
    {
        _owner = owner;
    }

    internal IArrangedElement? Owner => _owner;

    internal virtual string? PropertyName => null;

    int IList.Add(object? style) => Add((TableLayoutStyle)style!);

    public int Add(TableLayoutStyle style)
    {
        ArgumentNullException.ThrowIfNull(style);
        if (style is not TableLayoutStyle tableLayoutStyle)
        {
            throw new ArgumentException(string.Format(SR.InvalidArgumentType, nameof(style), typeof(TableLayoutStyle)), nameof(style));
        }

        EnsureNotOwned(tableLayoutStyle);
        tableLayoutStyle.Owner = Owner;
        int index = ((IList)_innerList).Add(tableLayoutStyle);
        PerformLayoutIfOwned();
        return index;
    }

    void IList.Insert(int index, object? style)
    {
        ArgumentNullException.ThrowIfNull(style);
        if (style is not TableLayoutStyle tableLayoutStyle)
        {
            throw new ArgumentException(string.Format(SR.InvalidArgumentType, nameof(style), typeof(TableLayoutStyle)), nameof(style));
        }

        EnsureNotOwned(tableLayoutStyle);
        tableLayoutStyle.Owner = Owner;
        _innerList.Insert(index, tableLayoutStyle);
        PerformLayoutIfOwned();
    }

    object? IList.this[int index]
    {
        get => _innerList[index];
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value is not TableLayoutStyle tableLayoutStyle)
            {
                throw new ArgumentException(string.Format(SR.InvalidArgumentType, nameof(value), typeof(TableLayoutStyle)), nameof(value));
            }

            EnsureNotOwned(tableLayoutStyle);
            tableLayoutStyle.Owner = Owner;
            _innerList[index] = tableLayoutStyle;
            PerformLayoutIfOwned();
        }
    }

    public TableLayoutStyle this[int index]
    {
        get => (TableLayoutStyle)((IList)this)[index]!;
        set => ((IList)this)[index] = value;
    }

    void IList.Remove(object? style)
    {
        if (style is null)
        {
            return;
        }

        if (style is not TableLayoutStyle tableLayoutStyle)
        {
            throw new ArgumentException(string.Format(SR.InvalidArgumentType, nameof(style), typeof(TableLayoutStyle)), nameof(style));
        }

        if (!_innerList.Remove(tableLayoutStyle))
        {
            return;
        }

        tableLayoutStyle.Owner = null;
        PerformLayoutIfOwned();
    }

    public void Clear()
    {
        foreach (TableLayoutStyle style in _innerList)
        {
            style.Owner = null;
        }

        _innerList.Clear();
        PerformLayoutIfOwned();
    }

    public void RemoveAt(int index)
    {
        TableLayoutStyle style = _innerList[index];
        style.Owner = null;
        _innerList.RemoveAt(index);
        PerformLayoutIfOwned();
    }

    bool IList.Contains(object? style) => ((IList)_innerList).Contains(style);

    int IList.IndexOf(object? style) => ((IList)_innerList).IndexOf(style);

    bool IList.IsFixedSize => ((IList)_innerList).IsFixedSize;

    bool IList.IsReadOnly => ((IList)_innerList).IsReadOnly;

    void ICollection.CopyTo(Array array, int startIndex) => ((ICollection)_innerList).CopyTo(array, startIndex);

    public int Count => _innerList.Count;

    bool ICollection.IsSynchronized => ((ICollection)_innerList).IsSynchronized;

    object ICollection.SyncRoot => ((ICollection)_innerList).SyncRoot;

    IEnumerator IEnumerable.GetEnumerator() => _innerList.GetEnumerator();

    private static void EnsureNotOwned(TableLayoutStyle style)
    {
        if (style.Owner is not null)
        {
            throw new ArgumentException(string.Format(SR.OnlyOneControl, style.GetType().Name), nameof(style));
        }
    }

    internal void EnsureOwnership(IArrangedElement owner)
    {
        _owner = owner;
        for (int i = 0; i < Count; i++)
        {
            this[i].Owner = owner;
        }
    }

    private void PerformLayoutIfOwned()
    {
        if (Owner is not null)
        {
            LayoutTransaction.DoLayout(Owner, Owner, PropertyName);
        }
    }
}
