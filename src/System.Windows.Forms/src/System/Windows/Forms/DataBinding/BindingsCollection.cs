// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
///  Represents a collection of data bindings on a control.
/// </summary>
[DefaultEvent(nameof(CollectionChanged))]
public class BindingsCollection : BaseCollection
{
    private readonly List<Binding> _list = [];
    private CollectionChangeEventHandler? _onCollectionChanging;
    private CollectionChangeEventHandler? _onCollectionChanged;

    internal BindingsCollection()
    {
    }

    public override int Count => _list.Count;

    /// <summary>
    ///  Gets the bindings in the collection as an object.
    /// </summary>
    protected override ArrayList List => ArrayList.Adapter(_list);

    /// <summary>
    ///  Gets the <see cref="Binding"/> at the specified index.
    /// </summary>
    public Binding this[int index] => _list[index]!;

    protected internal void Add(Binding binding)
    {
        CollectionChangeEventArgs eventArgs = new(CollectionChangeAction.Add, binding);
        OnCollectionChanging(eventArgs);
        AddCore(binding);
        OnCollectionChanged(eventArgs);
    }

    /// <summary>
    ///  Adds a <see cref="Binding"/> to the collection.
    /// </summary>
    protected virtual void AddCore(Binding dataBinding)
    {
        ArgumentNullException.ThrowIfNull(dataBinding);
        _list.Add(dataBinding);
    }

    /// <summary>
    ///  Occurs when the collection is about to change.
    /// </summary>
    [SRDescription(nameof(SR.collectionChangingEventDescr))]
    public event CollectionChangeEventHandler? CollectionChanging
    {
        add => _onCollectionChanging += value;
        remove => _onCollectionChanging -= value;
    }

    /// <summary>
    ///  Occurs when the collection is changed.
    /// </summary>
    [SRDescription(nameof(SR.collectionChangedEventDescr))]
    public event CollectionChangeEventHandler? CollectionChanged
    {
        add => _onCollectionChanged += value;
        remove => _onCollectionChanged -= value;
    }

    protected internal void Clear()
    {
        CollectionChangeEventArgs eventArgs = new(CollectionChangeAction.Refresh, null);
        OnCollectionChanging(eventArgs);
        ClearCore();
        OnCollectionChanged(eventArgs);
    }

    /// <summary>
    ///  Clears the collection of any members.
    /// </summary>
    protected virtual void ClearCore() => _list.Clear();

    /// <summary>
    ///  Raises the <see cref="CollectionChanging"/> event.
    /// </summary>
    protected virtual void OnCollectionChanging(CollectionChangeEventArgs e)
    {
        _onCollectionChanging?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the <see cref="CollectionChanged"/> event.
    /// </summary>
    protected virtual void OnCollectionChanged(CollectionChangeEventArgs ccevent)
    {
        _onCollectionChanged?.Invoke(this, ccevent);
    }

    protected internal void Remove(Binding binding)
    {
        CollectionChangeEventArgs eventArgs = new(CollectionChangeAction.Remove, binding);
        OnCollectionChanging(eventArgs);
        RemoveCore(binding);
        OnCollectionChanged(eventArgs);
    }

    protected internal void RemoveAt(int index) => Remove(this[index]);

    /// <summary>
    ///  Removes the specified <see cref="Binding"/> from the collection.
    /// </summary>
    protected virtual void RemoveCore(Binding dataBinding) => _list.Remove(dataBinding);

    protected internal bool ShouldSerializeMyAll() => Count > 0;
}
