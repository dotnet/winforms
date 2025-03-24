// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms;

/// <summary>
///  Manages the position and bindings of a list.
/// </summary>
public partial class CurrencyManager : BindingManagerBase
{
    private object? _dataSource;
    private IList _list;

    // flags enum field to hold private bool fields
    private CurrencyManagerStates _state;

    protected int listposition = -1;

    private int _lastGoodKnownRow = -1;
    private ItemChangedEventHandler? _onItemChanged;
    private ListChangedEventHandler? _onListChanged;
    private readonly ItemChangedEventArgs _resetEvent = new(-1);
    private EventHandler? _onMetaDataChangedHandler;

    /// <summary>
    ///  Gets the type of the list.
    /// </summary>
    protected Type? finalType;

    /// <summary>
    ///  Occurs when the
    ///  current item has been
    ///  altered.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    public event ItemChangedEventHandler? ItemChanged
    {
        add => _onItemChanged += value;
        remove => _onItemChanged -= value;
    }

    public event ListChangedEventHandler? ListChanged
    {
        add => _onListChanged += value;
        remove => _onListChanged -= value;
    }

    internal CurrencyManager(object? dataSource)
    {
        _state.ChangeFlags(CurrencyManagerStates.ShouldBind, true);
        _list = null!;
        SetDataSource(dataSource);
    }

    /// <summary>
    ///  Gets a value indicating
    ///  whether items can be added to the list.
    /// </summary>
    internal bool AllowAdd
    {
        get
        {
            if (_list is IBindingList bindingList)
            {
                return bindingList.AllowNew;
            }

            if (_list is null)
            {
                return false;
            }

            return !_list.IsReadOnly && !_list.IsFixedSize;
        }
    }

    /// <summary>
    ///  Gets a value
    ///  indicating whether edits to the list are allowed.
    /// </summary>
    internal bool AllowEdit
    {
        get
        {
            if (_list is IBindingList bindingList)
            {
                return bindingList.AllowEdit;
            }

            if (_list is null)
            {
                return false;
            }

            return !_list.IsReadOnly;
        }
    }

    /// <summary>
    ///  Gets a value indicating whether items can be removed from the list.
    /// </summary>
    internal bool AllowRemove
    {
        get
        {
            if (_list is IBindingList bindingList)
            {
                return bindingList.AllowRemove;
            }

            if (_list is null)
            {
                return false;
            }

            return !_list.IsReadOnly && !_list.IsFixedSize;
        }
    }

    /// <summary>
    ///  Gets the number of items in the list.
    /// </summary>
    public override int Count => _list is null ? 0 : _list.Count;

    /// <summary>
    ///  Gets the current item in the list.
    /// </summary>
    public override object? Current => this[Position];

    internal override Type BindType => ListBindingHelper.GetListItemType(List);

    /// <summary>
    ///  Gets the data source of the list.
    /// </summary>
    internal override object? DataSource => _dataSource;

    private protected override void SetDataSource(object? dataSource)
    {
        if (_dataSource == dataSource)
        {
            return;
        }

        Release();
        _dataSource = dataSource;
        _list = null!;
        finalType = null;

        object? tempList = dataSource;
        if (tempList is Array array)
        {
            finalType = tempList.GetType();
            tempList = array;
        }

        if (tempList is IListSource listSource)
        {
            tempList = listSource.GetList();
        }

        if (tempList is IList list)
        {
            finalType ??= tempList.GetType();

            _list = list;
            WireEvents(_list);
            if (_list.Count > 0)
            {
                listposition = 0;
            }
            else
            {
                listposition = -1;
            }

            OnItemChanged(_resetEvent);
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
            UpdateIsBinding();
        }
        else
        {
            ArgumentNullException.ThrowIfNull(tempList, nameof(dataSource));

            throw new ArgumentException(string.Format(SR.ListManagerSetDataSource, tempList.GetType().FullName), nameof(dataSource));
        }
    }

    /// <summary>
    ///  Gets a value indicating whether the list is bound to a data source.
    /// </summary>
    internal override bool IsBinding => _state.HasFlag(CurrencyManagerStates.Bound);

    // The DataGridView needs this.
    internal bool ShouldBind => _state.HasFlag(CurrencyManagerStates.ShouldBind);

    /// <summary>
    ///  Gets the list as an object.
    /// </summary>
    public IList List
    {
        get
        {
            // NOTE: do not change this to throw an exception if the list is not IBindingList.
            // doing this will cause a major performance hit when wiring the
            // dataGrid to listen for MetaDataChanged events from the IBindingList
            // (basically we would have to wrap all calls to CurrencyManager::List with
            // a try/catch block.)
            return _list;
        }
    }

    /// <summary>
    ///  Gets or sets the position you are at within the list.
    /// </summary>
    public override int Position
    {
        get => listposition;
        set
        {
            if (listposition == -1)
            {
                return;
            }

            if (value < 0)
            {
                value = 0;
            }

            int count = _list.Count;
            if (value >= count)
            {
                value = count - 1;
            }

            ChangeRecordState(
                value,
                validating: listposition != value,
                endCurrentEdit: true,
                firePositionChange: true,
                pullData: false);
        }
    }

    /// <summary>
    ///  Gets or sets the object at the specified index.
    /// </summary>
    internal object? this[int index]
    {
        get
        {
            if (index < 0 || index >= _list.Count)
            {
                throw new IndexOutOfRangeException(string.Format(SR.ListManagerNoValue, index.ToString(CultureInfo.CurrentCulture)));
            }

            return _list[index];
        }
        set
        {
            if (index < 0 || index >= _list.Count)
            {
                throw new IndexOutOfRangeException(string.Format(SR.ListManagerNoValue, index.ToString(CultureInfo.CurrentCulture)));
            }

            _list[index] = value;
        }
    }

    public override void AddNew()
    {
        if (_list is IBindingList ibl)
        {
            ibl.AddNew();
        }
        else
        {
            // If the list is not IBindingList, then throw an exception:
            throw new NotSupportedException(SR.CurrencyManagerCantAddNew);
        }

        ChangeRecordState(
            _list.Count - 1,
            validating: (Position != _list.Count - 1),
            endCurrentEdit: (Position != _list.Count - 1),
            firePositionChange: true,
            pullData: true);
    }

    /// <summary>
    ///  Cancels the current edit operation.
    /// </summary>
    public override void CancelCurrentEdit()
    {
        if (Count > 0)
        {
            object? item = (Position >= 0 && Position < _list.Count) ? _list[Position] : null;

            if (item is IEditableObject iEditableItem)
            {
                iEditableItem.CancelEdit();
            }

            if (_list is ICancelAddNew iListWithCancelAddNewSupport)
            {
                iListWithCancelAddNewSupport.CancelNew(Position);
            }

            OnItemChanged(new ItemChangedEventArgs(Position));
            if (Position != -1)
            {
                OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, Position));
            }
        }
    }

    private void ChangeRecordState(
        int newPosition,
        bool validating,
        bool endCurrentEdit,
        bool firePositionChange,
        bool pullData)
    {
        if (newPosition == -1 && _list.Count == 0)
        {
            if (listposition != -1)
            {
                listposition = -1;
                OnPositionChanged(EventArgs.Empty);
            }

            return;
        }

        if ((newPosition < 0 || newPosition >= Count) && IsBinding)
        {
            throw new IndexOutOfRangeException(SR.ListManagerBadPosition);
        }

        // if PushData fails in the OnCurrentChanged and there was a lastGoodKnownRow
        // then the position does not change, so we should not fire the OnPositionChanged
        // event;
        // this is why we have to cache the old position and compare that w/ the position that
        // the user will want to navigate to
        int oldPosition = listposition;
        if (endCurrentEdit)
        {
            // Do not PushData when pro.
            _state.ChangeFlags(CurrencyManagerStates.InChangeRecordState, true);
            try
            {
                EndCurrentEdit();
            }
            finally
            {
                _state.ChangeFlags(CurrencyManagerStates.InChangeRecordState, false);
            }
        }

        // we pull the data from the controls only when the ListManager changes the list. when the backEnd changes the list we do not
        // pull the data from the controls
        if (validating && pullData)
        {
            CurrencyManager_PullData();
        }

        // EndCurrentEdit or PullData can cause the list managed by the CurrencyManager to shrink.
        listposition = Math.Min(newPosition, Count - 1);

        if (validating)
        {
            OnCurrentChanged(EventArgs.Empty);
        }

        bool positionChanging = (oldPosition != listposition);
        if (positionChanging && firePositionChange)
        {
            OnPositionChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    ///  Throws an exception if there is no list.
    /// </summary>
    protected void CheckEmpty()
    {
        if (_dataSource is null || _list is null || _list.Count == 0)
        {
            throw new InvalidOperationException(SR.ListManagerEmptyList);
        }
    }

    // will return true if this function changes the position in the list
    private bool CurrencyManager_PushData()
    {
        if (_state.HasFlag(CurrencyManagerStates.PullingData))
        {
            return false;
        }

        int initialPosition = listposition;
        if (_lastGoodKnownRow == -1)
        {
            try
            {
                PushData();
            }
            catch (Exception ex)
            {
                OnDataError(ex);

                // get the first item in the list that is good to push data
                // for now, we assume that there is a row in the backEnd
                // that is good for all the bindings.
                FindGoodRow();
            }

            _lastGoodKnownRow = listposition;
        }
        else
        {
            try
            {
                PushData();
            }
            catch (Exception ex)
            {
                OnDataError(ex);

                listposition = _lastGoodKnownRow;
                PushData();
            }

            _lastGoodKnownRow = listposition;
        }

        return initialPosition != listposition;
    }

    private bool CurrencyManager_PullData()
    {
        bool success = true;
        _state.ChangeFlags(CurrencyManagerStates.PullingData, true);

        try
        {
            PullData(out success);
        }
        finally
        {
            _state.ChangeFlags(CurrencyManagerStates.PullingData, false);
        }

        return success;
    }

    public override void RemoveAt(int index) => _list.RemoveAt(index);

    /// <summary>
    ///  Ends the current edit operation.
    /// </summary>
    public override void EndCurrentEdit()
    {
        if (Count > 0)
        {
            bool success = CurrencyManager_PullData();

            if (success)
            {
                object? item = (Position >= 0 && Position < _list.Count) ? _list[Position] : null;

                if (item is IEditableObject iEditableItem)
                {
                    iEditableItem.EndEdit();
                }

                if (_list is ICancelAddNew iListWithCancelAddNewSupport)
                {
                    iListWithCancelAddNewSupport.EndNew(Position);
                }
            }
        }
    }

    private void FindGoodRow()
    {
        int rowCount = _list.Count;
        for (int i = 0; i < rowCount; i++)
        {
            listposition = i;
            try
            {
                PushData();
            }
            catch (Exception ex)
            {
                OnDataError(ex);
                continue;
            }

            listposition = i;
            return;
        }

        // if we got here, the list did not contain any rows suitable for the bindings
        // suspend binding and throw an exception
        SuspendBinding();
        throw new InvalidOperationException(SR.DataBindingPushDataException);
    }

    /// <summary>
    ///  Sets the column to sort by, and the direction of the sort.
    /// </summary>
    internal void SetSort(PropertyDescriptor property, ListSortDirection sortDirection)
    {
        if (_list is IBindingList { SupportsSorting: true } bindingList)
        {
            bindingList.ApplySort(property, sortDirection);
        }
    }

    /// <summary>
    ///  Gets a <see cref="PropertyDescriptor"/> for a CurrencyManager.
    /// </summary>
    internal PropertyDescriptor? GetSortProperty()
    {
        if (_list is IBindingList { SupportsSorting: true } bindingList)
        {
            return bindingList.SortProperty;
        }

        return null;
    }

    /// <summary>
    ///  Gets the sort direction of a list.
    /// </summary>
    internal ListSortDirection GetSortDirection()
    {
        if (_list is IBindingList { SupportsSorting: true } bindingList)
        {
            return bindingList.SortDirection;
        }

        return ListSortDirection.Ascending;
    }

    /// <summary>
    ///  Find the position of a desired list item.
    /// </summary>
    internal int Find(PropertyDescriptor? property, object key)
    {
        ArgumentNullException.ThrowIfNull(key);

        if (property is not null && _list is IBindingList { SupportsSearching: true } bindingList)
        {
            return bindingList.Find(property, key);
        }

        if (property is not null)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                object? value = property.GetValue(_list[i]);
                if (key.Equals(value))
                {
                    return i;
                }
            }
        }

        return -1;
    }

    /// <summary>
    ///  Gets the name of the list.
    /// </summary>
    internal override string GetListName() =>
        _list is ITypedList typedList
            ? typedList.GetListName(null)
            : finalType!.Name;

    /// <summary>
    ///  Gets the name of the specified list.
    /// </summary>
    protected internal override string GetListName(ArrayList? listAccessors)
    {
        if (listAccessors is null)
        {
            return string.Empty;
        }

        if (_list is ITypedList typedList)
        {
            PropertyDescriptor[] properties = new PropertyDescriptor[listAccessors.Count];
            listAccessors.CopyTo(properties, 0);
            return typedList.GetListName(properties);
        }

        return string.Empty;
    }

    internal override PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[]? listAccessors) =>
        ListBindingHelper.GetListItemProperties(_list, listAccessors);

    /// <summary>
    ///  Gets the <see cref="PropertyDescriptorCollection"/> for the list.
    /// </summary>
    public override PropertyDescriptorCollection GetItemProperties() => GetItemProperties(null);

    /// <summary>
    ///  Gets the <see cref="PropertyDescriptorCollection"/> for the specified list.
    /// </summary>
    private void List_ListChanged(object? sender, ListChangedEventArgs e)
    {
        // If you change the assert below, better change the
        // code in the OnCurrentChanged that deals w/ firing the OnCurrentChanged event
        Debug.Assert(_lastGoodKnownRow == -1 || _lastGoodKnownRow == listposition, "if we have a valid lastGoodKnownRow, then it should equal the position in the list");

        ListChangedEventArgs dbe;
        if (e.ListChangedType == ListChangedType.ItemMoved && e.OldIndex < 0)
        {
            dbe = new ListChangedEventArgs(ListChangedType.ItemAdded, e.NewIndex, e.OldIndex);
        }
        else if (e.ListChangedType == ListChangedType.ItemMoved && e.NewIndex < 0)
        {
            dbe = new ListChangedEventArgs(ListChangedType.ItemDeleted, e.OldIndex, e.NewIndex);
        }
        else
        {
            dbe = e;
        }

        int oldposition = listposition;

        UpdateLastGoodKnownRow(dbe);
        UpdateIsBinding();

        if (_list.Count == 0)
        {
            listposition = -1;

            if (oldposition != -1)
            {
                // if we used to have a current row, but not any more, then report current as changed
                OnPositionChanged(EventArgs.Empty);
                OnCurrentChanged(EventArgs.Empty);
            }

            if (dbe.ListChangedType == ListChangedType.Reset && e.NewIndex == -1)
            {
                // if the list is reset, then let our users know about it.
                OnItemChanged(_resetEvent);
            }

            if (dbe.ListChangedType == ListChangedType.ItemDeleted)
            {
                // if the list is reset, then let our users know about it.
                OnItemChanged(_resetEvent);
            }

            // we should still fire meta data change notification even when the list is empty
            if (e.ListChangedType is ListChangedType.PropertyDescriptorAdded or
                ListChangedType.PropertyDescriptorDeleted or
                ListChangedType.PropertyDescriptorChanged)
            {
                OnMetaDataChanged(EventArgs.Empty);
            }

            OnListChanged(dbe);
            return;
        }

        _state.ChangeFlags(CurrencyManagerStates.SuspendPushDataInCurrentChanged, true);
        try
        {
            switch (dbe.ListChangedType)
            {
                case ListChangedType.Reset:
                    if (listposition == -1 && _list.Count > 0)
                    {
                        ChangeRecordState(0, true, false, true, false);     // last false: we don't pull the data from the control when DM changes
                    }
                    else
                    {
                        ChangeRecordState(Math.Min(listposition, _list.Count - 1), true, false, true, false);
                    }

                    UpdateIsBinding(raiseItemChangedEvent: false);
                    OnItemChanged(_resetEvent);
                    break;
                case ListChangedType.ItemAdded:
                    if (dbe.NewIndex <= listposition && listposition < _list.Count - 1)
                    {
                        // this means the current row just moved down by one.
                        // the position changes, so end the current edit
                        ChangeRecordState(listposition + 1, true, true, listposition != _list.Count - 2, false);
                        UpdateIsBinding();
                        // refresh the list after we got the item added event
                        OnItemChanged(_resetEvent);
                        // when we get the itemAdded, and the position was at the end
                        // of the list, do the right thing and notify the positionChanged after refreshing the list
                        if (listposition == _list.Count - 1)
                        {
                            OnPositionChanged(EventArgs.Empty);
                        }

                        break;
                    }
                    else if (dbe.NewIndex == listposition && listposition == _list.Count - 1 && listposition != -1)
                    {
                        // The CurrencyManager has a non-empty list.
                        // The position inside the currency manager is at the end of the list and the list still fired an ItemAdded event.
                        // This could be the second ItemAdded event that the DataView fires to signal that the AddNew operation was commited.
                        // We need to fire CurrentItemChanged event so that relatedCurrencyManagers update their lists.
                        OnCurrentItemChanged(EventArgs.Empty);
                    }

                    if (listposition == -1)
                    {
                        // do not call EndEdit on a row that was not there ( position == -1)
                        ChangeRecordState(0, false, false, true, false);
                    }

                    UpdateIsBinding();
                    // put the call to OnItemChanged after setting the position, so the
                    // controls would use the actual position.
                    // if we have a control bound to a dataView, and then we add a row to a the dataView,
                    // then the control will use the old listposition to get the data. and this is bad.
                    //
                    OnItemChanged(_resetEvent);
                    break;
                case ListChangedType.ItemDeleted:
                    if (dbe.NewIndex == listposition)
                    {
                        // this means that the current row got deleted.
                        // cannot end an edit on a row that does not exist anymore
                        ChangeRecordState(Math.Min(listposition, Count - 1), true, false, true, false);
                        // put the call to OnItemChanged after setting the position
                        // in the currencyManager, so controls will use the actual position
                        OnItemChanged(_resetEvent);
                        break;
                    }

                    if (dbe.NewIndex < listposition)
                    {
                        // this means the current row just moved up by one.
                        // cannot end an edit on a row that does not exist anymore
                        ChangeRecordState(listposition - 1, true, false, true, false);
                        // put the call to OnItemChanged after setting the position
                        // in the currencyManager, so controls will use the actual position
                        OnItemChanged(_resetEvent);
                        break;
                    }

                    OnItemChanged(_resetEvent);
                    break;
                case ListChangedType.ItemChanged:
                    // the current item changed
                    if (dbe.NewIndex == listposition)
                    {
                        OnCurrentItemChanged(EventArgs.Empty);
                    }

                    OnItemChanged(new ItemChangedEventArgs(dbe.NewIndex));
                    break;
                case ListChangedType.ItemMoved:
                    if (dbe.OldIndex == listposition)
                    {
                        // current got moved.
                        // the position changes, so end the current edit. Make sure there is something that we can end edit...
                        ChangeRecordState(dbe.NewIndex, true, Position > -1 && Position < _list.Count, true, false);
                    }
                    else if (dbe.NewIndex == listposition)
                    {
                        // current was moved
                        // the position changes, so end the current edit. Make sure there is something that we can end edit
                        ChangeRecordState(dbe.OldIndex, true, Position > -1 && Position < _list.Count, true, false);
                    }

                    OnItemChanged(_resetEvent);
                    break;
                case ListChangedType.PropertyDescriptorAdded:
                case ListChangedType.PropertyDescriptorDeleted:
                case ListChangedType.PropertyDescriptorChanged:
                    // reset lastGoodKnownRow because it was computed against property descriptors which changed
                    _lastGoodKnownRow = -1;

                    // In Everett, metadata changes did not alter current list position. In Whidbey, this behavior
                    // preserved - except that we will now force the position to stay in valid range if necessary.
                    if (listposition == -1 && _list.Count > 0)
                    {
                        ChangeRecordState(0, true, false, true, false);
                    }
                    else if (listposition > _list.Count - 1)
                    {
                        ChangeRecordState(_list.Count - 1, true, false, true, false);
                    }

                    // fire the MetaDataChanged event
                    OnMetaDataChanged(EventArgs.Empty);
                    break;
            }

            // send the ListChanged notification after the position changed in the list
            //

            OnListChanged(dbe);
        }
        finally
        {
            _state.ChangeFlags(CurrencyManagerStates.SuspendPushDataInCurrentChanged, false);
        }

        Debug.Assert(_lastGoodKnownRow == -1 || listposition == _lastGoodKnownRow, "how did they get out of sync?");
    }

    [SRCategory(nameof(SR.CatData))]
    public event EventHandler? MetaDataChanged
    {
        add => _onMetaDataChangedHandler += value;
        remove => _onMetaDataChangedHandler -= value;
    }

    /// <summary>
    ///  Causes the CurrentChanged event to occur.
    /// </summary>
    protected internal override void OnCurrentChanged(EventArgs e)
    {
        if (!_state.HasFlag(CurrencyManagerStates.InChangeRecordState))
        {
            int curLastGoodKnownRow = _lastGoodKnownRow;
            bool positionChanged = false;
            if (!_state.HasFlag(CurrencyManagerStates.SuspendPushDataInCurrentChanged))
            {
                positionChanged = CurrencyManager_PushData();
            }

            if (Count > 0)
            {
                object? item = _list[Position];
                if (item is IEditableObject editableObject)
                {
                    editableObject.BeginEdit();
                }
            }

            try
            {
                // if currencyManager changed position then we have two cases:
                // 1. the previous lastGoodKnownRow was valid: in that case we fell back so do not fire onCurrentChanged
                // 2. the previous lastGoodKnownRow was invalid: we have two cases:
                //      a. FindGoodRow actually found a good row, so it can't be the one before the user changed
                //         the position: fire the onCurrentChanged
                //      b. FindGoodRow did not find a good row: we should have gotten an exception so we should
                //         not even execute this code
                if (!positionChanged || (positionChanged && curLastGoodKnownRow != -1))
                {
                    onCurrentChangedHandler?.Invoke(this, e);

                    // we fire OnCurrentItemChanged event every time we fire the CurrentChanged + when a property of the Current item changed
                    _onCurrentItemChangedHandler?.Invoke(this, e);
                }
            }
            catch (Exception ex)
            {
                OnDataError(ex);
            }
        }
    }

    // this method should only be called when the currency manager receives the ListChangedType.ItemChanged event
    // and when the index of the ListChangedEventArgs == the position in the currency manager
    protected internal override void OnCurrentItemChanged(EventArgs e)
    {
        _onCurrentItemChangedHandler?.Invoke(this, e);
    }

    protected virtual void OnItemChanged(ItemChangedEventArgs e)
    {
        // It is possible that CurrencyManager_PushData will change the position
        // in the list. in that case we have to fire OnPositionChanged event
        bool positionChanged = false;

        // We should not push the data when we suspend the changeEvents.
        // but we should still fire the OnItemChanged event that we get when processing the EndCurrentEdit method.
        if ((e.Index == listposition || (e.Index == -1 && Position < Count)) && !_state.HasFlag(CurrencyManagerStates.InChangeRecordState))
        {
            positionChanged = CurrencyManager_PushData();
        }

        try
        {
            _onItemChanged?.Invoke(this, e);
        }
        catch (Exception ex)
        {
            OnDataError(ex);
        }

        if (positionChanged)
        {
            OnPositionChanged(EventArgs.Empty);
        }
    }

    private void OnListChanged(ListChangedEventArgs e)
    {
        _onListChanged?.Invoke(this, e);
    }

    // Exists in Everett
    protected internal void OnMetaDataChanged(EventArgs e)
    {
        _onMetaDataChangedHandler?.Invoke(this, e);
    }

    protected virtual void OnPositionChanged(EventArgs e)
    {
        try
        {
            onPositionChangedHandler?.Invoke(this, e);
        }
        catch (Exception ex)
        {
            OnDataError(ex);
        }
    }

    /// <summary>
    ///  Forces a repopulation of the CurrencyManager
    /// </summary>
    public void Refresh()
    {
        if (_list.Count > 0)
        {
            if (listposition >= _list.Count)
            {
                _lastGoodKnownRow = -1;
                listposition = 0;
            }
        }
        else
        {
            listposition = -1;
        }

        List_ListChanged(_list, new ListChangedEventArgs(ListChangedType.Reset, -1));
    }

    internal void Release()
    {
        UnwireEvents(_list);
    }

    /// <summary>
    ///  Resumes binding of component properties to list items.
    /// </summary>
    public override void ResumeBinding()
    {
        _lastGoodKnownRow = -1;
        try
        {
            if (!ShouldBind)
            {
                _state.ChangeFlags(CurrencyManagerStates.ShouldBind, true);
                // we need to put the listPosition at the beginning of the list if the list is not empty
                listposition = (_list is not null && _list.Count != 0) ? 0 : -1;
                UpdateIsBinding();
            }
        }
        catch
        {
            _state.ChangeFlags(CurrencyManagerStates.ShouldBind, false);
            UpdateIsBinding();
            throw;
        }
    }

    /// <summary>
    ///  Suspends binding.
    /// </summary>
    public override void SuspendBinding()
    {
        _lastGoodKnownRow = -1;
        if (ShouldBind)
        {
            _state.ChangeFlags(CurrencyManagerStates.ShouldBind, false);
            UpdateIsBinding();
        }
    }

    internal void UnwireEvents(IList list)
    {
        if (list is IBindingList bindingList && bindingList.SupportsChangeNotification)
        {
            bindingList.ListChanged -= List_ListChanged;
        }
    }

    protected override void UpdateIsBinding()
    {
        UpdateIsBinding(true);
    }

    private void UpdateIsBinding(bool raiseItemChangedEvent)
    {
        bool newBound = _list is not null && _list.Count > 0 && ShouldBind && listposition != -1;
        if (_list is not null)
        {
            if (_state.HasFlag(CurrencyManagerStates.Bound) != newBound)
            {
                // we will call end edit when moving from bound state to unbounded state
                //
                // bool endCurrentEdit = bound && !newBound;
                _state.ChangeFlags(CurrencyManagerStates.Bound, newBound);
                int newPos = newBound ? 0 : -1;
                ChangeRecordState(newPos, _state.HasFlag(CurrencyManagerStates.Bound), (Position != newPos), true, false);
                int numLinks = Bindings.Count;
                for (int i = 0; i < numLinks; i++)
                {
                    Bindings[i].UpdateIsBinding();
                }

                if (raiseItemChangedEvent)
                {
                    OnItemChanged(_resetEvent);
                }
            }
        }
    }

    private void UpdateLastGoodKnownRow(ListChangedEventArgs e)
    {
        switch (e.ListChangedType)
        {
            case ListChangedType.ItemDeleted:
                if (e.NewIndex == _lastGoodKnownRow)
                {
                    _lastGoodKnownRow = -1;
                }

                break;
            case ListChangedType.Reset:
                _lastGoodKnownRow = -1;
                break;
            case ListChangedType.ItemAdded:
                if (e.NewIndex <= _lastGoodKnownRow && _lastGoodKnownRow < List.Count - 1)
                {
                    _lastGoodKnownRow++;
                }

                break;
            case ListChangedType.ItemMoved:
                if (e.OldIndex == _lastGoodKnownRow)
                {
                    _lastGoodKnownRow = e.NewIndex;
                }

                break;
            case ListChangedType.ItemChanged:
                if (e.NewIndex == _lastGoodKnownRow)
                {
                    _lastGoodKnownRow = -1;
                }

                break;
        }
    }

    internal void WireEvents(IList list)
    {
        if (list is IBindingList bindingList && bindingList.SupportsChangeNotification)
        {
            bindingList.ListChanged += List_ListChanged;
        }
    }
}
