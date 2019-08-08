// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Manages the position and bindings of a list.
    /// </summary>
    public class CurrencyManager : BindingManagerBase
    {
        private object dataSource;
        private IList list;

        private bool bound = false;
        private bool shouldBind = true;

        protected int listposition = -1;

        private int lastGoodKnownRow = -1;
        private bool pullingData = false;

        private bool inChangeRecordState = false;
        private bool suspendPushDataInCurrentChanged = false;
        // private bool onItemChangedCalled = false;
        // private EventHandler onCurrentChanged;
        // private CurrentChangingEventHandler onCurrentChanging;
        private ItemChangedEventHandler onItemChanged;
        private ListChangedEventHandler onListChanged;
        private readonly ItemChangedEventArgs resetEvent = new ItemChangedEventArgs(-1);
        private EventHandler onMetaDataChangedHandler;

        /// <summary>
        ///  Gets the type of the list.
        /// </summary>
        protected Type finalType;

        /// <summary>
        ///  Occurs when the
        ///  current item has been
        ///  altered.
        /// </summary>
        [SRCategory(nameof(SR.CatData))]
        public event ItemChangedEventHandler ItemChanged
        {
            add => onItemChanged += value;
            remove => onItemChanged -= value;
        }

        public event ListChangedEventHandler ListChanged
        {
            add => onListChanged += value;
            remove => onListChanged -= value;
        }

        internal CurrencyManager(object dataSource)
        {
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
                if (list is IBindingList)
                {
                    return ((IBindingList)list).AllowNew;
                }
                if (list == null)
                {
                    return false;
                }

                return !list.IsReadOnly && !list.IsFixedSize;
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
                if (list is IBindingList)
                {
                    return ((IBindingList)list).AllowEdit;
                }
                if (list == null)
                {
                    return false;
                }

                return !list.IsReadOnly;
            }
        }

        /// <summary>
        ///  Gets a value indicating whether items can be removed from the list.
        /// </summary>
        internal bool AllowRemove
        {
            get
            {
                if (list is IBindingList)
                {
                    return ((IBindingList)list).AllowRemove;
                }
                if (list == null)
                {
                    return false;
                }

                return !list.IsReadOnly && !list.IsFixedSize;
            }
        }

        /// <summary>
        ///  Gets the number of items in the list.
        /// </summary>
        public override int Count
        {
            get
            {
                if (list == null)
                {
                    return 0;
                }
                else
                {
                    return list.Count;
                }
            }
        }

        /// <summary>
        ///  Gets the current item in the list.
        /// </summary>
        public override object Current
        {
            get
            {
                return this[Position];
            }
        }

        internal override Type BindType
        {
            get
            {
                return ListBindingHelper.GetListItemType(List);
            }
        }

        /// <summary>
        ///  Gets the data source of the list.
        /// </summary>
        internal override object DataSource
        {
            get
            {
                return dataSource;
            }
        }

        private protected override void SetDataSource(object dataSource)
        {
            if (this.dataSource != dataSource)
            {
                Release();
                this.dataSource = dataSource;
                list = null;
                finalType = null;

                object tempList = dataSource;
                if (tempList is Array)
                {
                    finalType = tempList.GetType();
                    tempList = (Array)tempList;
                }

                if (tempList is IListSource)
                {
                    tempList = ((IListSource)tempList).GetList();
                }

                if (tempList is IList)
                {
                    if (finalType == null)
                    {
                        finalType = tempList.GetType();
                    }
                    list = (IList)tempList;
                    WireEvents(list);
                    if (list.Count > 0)
                    {
                        listposition = 0;
                    }
                    else
                    {
                        listposition = -1;
                    }

                    OnItemChanged(resetEvent);
                    OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
                    UpdateIsBinding();
                }
                else
                {
                    if (tempList == null)
                    {
                        throw new ArgumentNullException(nameof(dataSource));
                    }
                    throw new ArgumentException(string.Format(SR.ListManagerSetDataSource, tempList.GetType().FullName), "dataSource");
                }

            }
        }

        /// <summary>
        ///  Gets a value indicating whether the list is bound to a data source.
        /// </summary>
        internal override bool IsBinding
        {
            get
            {
                return bound;
            }
        }

        // The DataGridView needs this.
        internal bool ShouldBind
        {
            get
            {
                return shouldBind;
            }
        }

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
                //
                return list;
            }
        }

        /// <summary>
        ///  Gets or sets the position you are at within the list.
        /// </summary>
        public override int Position
        {
            get
            {
                return listposition;
            }
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

                int count = list.Count;
                if (value >= count)
                {
                    value = count - 1;
                }

                ChangeRecordState(value, listposition != value, true, true, false);       // true for endCurrentEdit
                                                                                          // true for firingPositionChange notification
                                                                                          // data will be pulled from controls anyway.
            }
        }

        /// <summary>
        ///  Gets or sets the object at the specified index.
        /// </summary>
        internal object this[int index]
        {
            get
            {
                if (index < 0 || index >= list.Count)
                {
                    throw new IndexOutOfRangeException(string.Format(SR.ListManagerNoValue, index.ToString(CultureInfo.CurrentCulture)));
                }
                return list[index];
            }
            set
            {
                if (index < 0 || index >= list.Count)
                {
                    throw new IndexOutOfRangeException(string.Format(SR.ListManagerNoValue, index.ToString(CultureInfo.CurrentCulture)));
                }
                list[index] = value;
            }
        }

        public override void AddNew()
        {
            if (list is IBindingList ibl)
            {
                ibl.AddNew();
            }
            else
            {
                // If the list is not IBindingList, then throw an exception:
                throw new NotSupportedException(SR.CurrencyManagerCantAddNew);
            }

            ChangeRecordState(list.Count - 1, (Position != list.Count - 1), (Position != list.Count - 1), true, true);  // true for firingPositionChangeNotification
                                                                                                                        // true for pulling data from the controls
        }

        /// <summary>
        ///  Cancels the current edit operation.
        /// </summary>
        public override void CancelCurrentEdit()
        {
            if (Count > 0)
            {
                object item = (Position >= 0 && Position < list.Count) ? list[Position] : null;

                // onItemChangedCalled = false;

                if (item is IEditableObject iEditableItem)
                {
                    iEditableItem.CancelEdit();
                }

                if (list is ICancelAddNew iListWithCancelAddNewSupport)
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

        private void ChangeRecordState(int newPosition, bool validating, bool endCurrentEdit, bool firePositionChange, bool pullData)
        {
            if (newPosition == -1 && list.Count == 0)
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
                inChangeRecordState = true;
                try
                {
                    EndCurrentEdit();
                }
                finally
                {
                    inChangeRecordState = false;
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
            if (dataSource == null || list == null || list.Count == 0)
            {
                throw new InvalidOperationException(SR.ListManagerEmptyList);
            }
        }

        // will return true if this function changes the position in the list
        private bool CurrencyManager_PushData()
        {
            if (pullingData)
            {
                return false;
            }

            int initialPosition = listposition;
            if (lastGoodKnownRow == -1)
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
                lastGoodKnownRow = listposition;
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

                    listposition = lastGoodKnownRow;
                    PushData();
                }
                lastGoodKnownRow = listposition;
            }

            return initialPosition != listposition;
        }

        private bool CurrencyManager_PullData()
        {
            bool success = true;
            pullingData = true;

            try
            {
                PullData(out success);
            }
            finally
            {
                pullingData = false;
            }

            return success;
        }

        public override void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }

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
                    object item = (Position >= 0 && Position < list.Count) ? list[Position] : null;

                    if (item is IEditableObject iEditableItem)
                    {
                        iEditableItem.EndEdit();
                    }

                    if (list is ICancelAddNew iListWithCancelAddNewSupport)
                    {
                        iListWithCancelAddNewSupport.EndNew(Position);
                    }
                }
            }
        }

        private void FindGoodRow()
        {
            int rowCount = list.Count;
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
            if (list is IBindingList && ((IBindingList)list).SupportsSorting)
            {
                ((IBindingList)list).ApplySort(property, sortDirection);
            }
        }

        /// <summary>
        ///  Gets a <see cref='PropertyDescriptor'/> for a CurrencyManager.
        /// </summary>
        internal PropertyDescriptor GetSortProperty()
        {
            if ((list is IBindingList) && ((IBindingList)list).SupportsSorting)
            {
                return ((IBindingList)list).SortProperty;
            }
            return null;
        }

        /// <summary>
        ///  Gets the sort direction of a list.
        /// </summary>
        internal ListSortDirection GetSortDirection()
        {
            if ((list is IBindingList) && ((IBindingList)list).SupportsSorting)
            {
                return ((IBindingList)list).SortDirection;
            }
            return ListSortDirection.Ascending;
        }

        /// <summary>
        ///  Find the position of a desired list item.
        /// </summary>
        internal int Find(PropertyDescriptor property, object key, bool keepIndex)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (property != null && (list is IBindingList) && ((IBindingList)list).SupportsSearching)
            {
                return ((IBindingList)list).Find(property, key);
            }

            if (property != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    object value = property.GetValue(list[i]);
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
        internal override string GetListName()
        {
            if (list is ITypedList)
            {
                return ((ITypedList)list).GetListName(null);
            }
            else
            {
                return finalType.Name;
            }
        }

        /// <summary>
        ///  Gets the name of the specified list.
        /// </summary>
        protected internal override string GetListName(ArrayList listAccessors)
        {
            if (list is ITypedList)
            {
                PropertyDescriptor[] properties = new PropertyDescriptor[listAccessors.Count];
                listAccessors.CopyTo(properties, 0);
                return ((ITypedList)list).GetListName(properties);
            }
            return "";
        }

        internal override PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            return ListBindingHelper.GetListItemProperties(list, listAccessors);
        }

        /// <summary>
        ///  Gets the <see cref='T:System.ComponentModel.PropertyDescriptorCollection'/> for
        ///  the list.
        /// </summary>
        public override PropertyDescriptorCollection GetItemProperties()
        {
            return GetItemProperties(null);
        }

        /// <summary>
        ///  Gets the <see cref='T:System.ComponentModel.PropertyDescriptorCollection'/> for the specified list.
        /// </summary>
        private void List_ListChanged(object sender, ListChangedEventArgs e)
        {
            // If you change the assert below, better change the
            // code in the OnCurrentChanged that deals w/ firing the OnCurrentChanged event
            Debug.Assert(lastGoodKnownRow == -1 || lastGoodKnownRow == listposition, "if we have a valid lastGoodKnownRow, then it should equal the position in the list");

            //

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

            if (list.Count == 0)
            {
                listposition = -1;

                if (oldposition != -1)
                {
                    // if we used to have a current row, but not any more, then report current as changed
                    OnPositionChanged(EventArgs.Empty);
                    OnCurrentChanged(EventArgs.Empty);
                }

                if (dbe.ListChangedType == System.ComponentModel.ListChangedType.Reset && e.NewIndex == -1)
                {
                    // if the list is reset, then let our users know about it.
                    OnItemChanged(resetEvent);
                }

                if (dbe.ListChangedType == System.ComponentModel.ListChangedType.ItemDeleted)
                {
                    // if the list is reset, then let our users know about it.
                    OnItemChanged(resetEvent);
                }

                // we should still fire meta data change notification even when the list is empty
                if (e.ListChangedType == System.ComponentModel.ListChangedType.PropertyDescriptorAdded ||
                    e.ListChangedType == System.ComponentModel.ListChangedType.PropertyDescriptorDeleted ||
                    e.ListChangedType == System.ComponentModel.ListChangedType.PropertyDescriptorChanged)
                {
                    OnMetaDataChanged(EventArgs.Empty);
                }

                //

                OnListChanged(dbe);
                return;
            }

            suspendPushDataInCurrentChanged = true;
            try
            {
                switch (dbe.ListChangedType)
                {
                    case System.ComponentModel.ListChangedType.Reset:
                        Debug.WriteLineIf(CompModSwitches.DataCursor.TraceVerbose, "System.ComponentModel.ListChangedType.Reset Position: " + Position + " Count: " + list.Count);
                        if (listposition == -1 && list.Count > 0)
                        {
                            ChangeRecordState(0, true, false, true, false);     // last false: we don't pull the data from the control when DM changes
                        }
                        else
                        {
                            ChangeRecordState(Math.Min(listposition, list.Count - 1), true, false, true, false);
                        }

                        UpdateIsBinding(/*raiseItemChangedEvent:*/ false);
                        OnItemChanged(resetEvent);
                        break;
                    case System.ComponentModel.ListChangedType.ItemAdded:
                        Debug.WriteLineIf(CompModSwitches.DataCursor.TraceVerbose, "System.ComponentModel.ListChangedType.ItemAdded " + dbe.NewIndex.ToString(CultureInfo.InvariantCulture));
                        if (dbe.NewIndex <= listposition && listposition < list.Count - 1)
                        {
                            // this means the current row just moved down by one.
                            // the position changes, so end the current edit
                            ChangeRecordState(listposition + 1, true, true, listposition != list.Count - 2, false);
                            UpdateIsBinding();
                            // refresh the list after we got the item added event
                            OnItemChanged(resetEvent);
                            // when we get the itemAdded, and the position was at the end
                            // of the list, do the right thing and notify the positionChanged after refreshing the list
                            if (listposition == list.Count - 1)
                            {
                                OnPositionChanged(EventArgs.Empty);
                            }

                            break;
                        }
                        else if (dbe.NewIndex == listposition && listposition == list.Count - 1 && listposition != -1)
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
                        OnItemChanged(resetEvent);
                        break;
                    case System.ComponentModel.ListChangedType.ItemDeleted:
                        Debug.WriteLineIf(CompModSwitches.DataCursor.TraceVerbose, "System.ComponentModel.ListChangedType.ItemDeleted " + dbe.NewIndex.ToString(CultureInfo.InvariantCulture));
                        if (dbe.NewIndex == listposition)
                        {
                            // this means that the current row got deleted.
                            // cannot end an edit on a row that does not exist anymore
                            ChangeRecordState(Math.Min(listposition, Count - 1), true, false, true, false);
                            // put the call to OnItemChanged after setting the position
                            // in the currencyManager, so controls will use the actual position
                            OnItemChanged(resetEvent);
                            break;

                        }
                        if (dbe.NewIndex < listposition)
                        {
                            // this means the current row just moved up by one.
                            // cannot end an edit on a row that does not exist anymore
                            ChangeRecordState(listposition - 1, true, false, true, false);
                            // put the call to OnItemChanged after setting the position
                            // in the currencyManager, so controls will use the actual position
                            OnItemChanged(resetEvent);
                            break;
                        }
                        OnItemChanged(resetEvent);
                        break;
                    case System.ComponentModel.ListChangedType.ItemChanged:
                        Debug.WriteLineIf(CompModSwitches.DataCursor.TraceVerbose, "System.ComponentModel.ListChangedType.ItemChanged " + dbe.NewIndex.ToString(CultureInfo.InvariantCulture));
                        // the current item changed
                        if (dbe.NewIndex == listposition)
                        {
                            OnCurrentItemChanged(EventArgs.Empty);
                        }

                        OnItemChanged(new ItemChangedEventArgs(dbe.NewIndex));
                        break;
                    case System.ComponentModel.ListChangedType.ItemMoved:
                        Debug.WriteLineIf(CompModSwitches.DataCursor.TraceVerbose, "System.ComponentModel.ListChangedType.ItemMoved " + dbe.NewIndex.ToString(CultureInfo.InvariantCulture));
                        if (dbe.OldIndex == listposition)
                        { // current got moved.
                            // the position changes, so end the current edit. Make sure there is something that we can end edit...
                            ChangeRecordState(dbe.NewIndex, true, Position > -1 && Position < list.Count, true, false);
                        }
                        else if (dbe.NewIndex == listposition)
                        { // current was moved
                            // the position changes, so end the current edit. Make sure there is something that we can end edit
                            ChangeRecordState(dbe.OldIndex, true, Position > -1 && Position < list.Count, true, false);
                        }
                        OnItemChanged(resetEvent);
                        break;
                    case System.ComponentModel.ListChangedType.PropertyDescriptorAdded:
                    case System.ComponentModel.ListChangedType.PropertyDescriptorDeleted:
                    case System.ComponentModel.ListChangedType.PropertyDescriptorChanged:
                        // reset lastGoodKnownRow because it was computed against property descriptors which changed
                        lastGoodKnownRow = -1;

                        // In Everett, metadata changes did not alter current list position. In Whidbey, this behavior
                        // preserved - except that we will now force the position to stay in valid range if necessary.
                        if (listposition == -1 && list.Count > 0)
                        {
                            ChangeRecordState(0, true, false, true, false);
                        }
                        else if (listposition > list.Count - 1)
                        {
                            ChangeRecordState(list.Count - 1, true, false, true, false);
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
                suspendPushDataInCurrentChanged = false;
            }
            Debug.Assert(lastGoodKnownRow == -1 || listposition == lastGoodKnownRow, "how did they get out of sync?");
        }

        [SRCategory(nameof(SR.CatData))]
        public event EventHandler MetaDataChanged
        {
            add => onMetaDataChangedHandler += value;
            remove => onMetaDataChangedHandler -= value;
        }

        /// <summary>
        ///  Causes the CurrentChanged event to occur.
        /// </summary>
        internal protected override void OnCurrentChanged(EventArgs e)
        {
            if (!inChangeRecordState)
            {
                Debug.WriteLineIf(CompModSwitches.DataView.TraceVerbose, "OnCurrentChanged() " + e.ToString());
                int curLastGoodKnownRow = lastGoodKnownRow;
                bool positionChanged = false;
                if (!suspendPushDataInCurrentChanged)
                {
                    positionChanged = CurrencyManager_PushData();
                }

                if (Count > 0)
                {
                    object item = list[Position];
                    if (item is IEditableObject)
                    {
                        ((IEditableObject)item).BeginEdit();
                    }
                }
                try
                {
                    // if currencyManager changed position then we have two cases:
                    // 1. the previous lastGoodKnownRow was valid: in that case we fell back so do not fire onCurrentChanged
                    // 2. the previous lastGoodKnownRow was invalid: we have two cases:
                    //      a. FindGoodRow actually found a good row, so it can't be the one before the user changed the position: fire the onCurrentChanged
                    //      b. FindGoodRow did not find a good row: we should have gotten an exception so we should not even execute this code
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
            if ((e.Index == listposition || (e.Index == -1 && Position < Count)) && !inChangeRecordState)
            {
                positionChanged = CurrencyManager_PushData();
            }

            Debug.WriteLineIf(CompModSwitches.DataView.TraceVerbose, "OnItemChanged(" + e.Index.ToString(CultureInfo.InvariantCulture) + ") " + e.ToString());
            try
            {
                onItemChanged?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                OnDataError(ex);
            }

            if (positionChanged)
            {
                OnPositionChanged(EventArgs.Empty);
            }
            // onItemChangedCalled = true;
        }

        private void OnListChanged(ListChangedEventArgs e)
        {
            onListChanged?.Invoke(this, e);
        }

//Exists in Everett
        internal protected void OnMetaDataChanged(EventArgs e)
        {
            onMetaDataChangedHandler?.Invoke(this, e);
        }

        protected virtual void OnPositionChanged(EventArgs e)
        {
            // if (!inChangeRecordState) {
            Debug.WriteLineIf(CompModSwitches.DataView.TraceVerbose, "OnPositionChanged(" + listposition.ToString(CultureInfo.InvariantCulture) + ") " + e.ToString());
            try
            {
                onPositionChangedHandler?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                OnDataError(ex);
            }
            // }
        }

        /// <summary>
        ///  Forces a repopulation of the CurrencyManager
        /// </summary>
        public void Refresh()
        {
            if (list.Count > 0)
            {
                if (listposition >= list.Count)
                {
                    lastGoodKnownRow = -1;
                    listposition = 0;
                }
            }
            else
            {
                listposition = -1;
            }
            List_ListChanged(list, new ListChangedEventArgs(System.ComponentModel.ListChangedType.Reset, -1));
        }

        internal void Release()
        {
            UnwireEvents(list);
        }

        /// <summary>
        ///  Resumes binding of component properties to list items.
        /// </summary>
        public override void ResumeBinding()
        {
            lastGoodKnownRow = -1;
            try
            {
                if (!shouldBind)
                {
                    shouldBind = true;
                    // we need to put the listPosition at the beginning of the list if the list is not empty
                    listposition = (list != null && list.Count != 0) ? 0 : -1;
                    UpdateIsBinding();
                }
            }
            catch
            {
                shouldBind = false;
                UpdateIsBinding();
                throw;
            }
        }

        /// <summary>
        ///  Suspends binding.
        /// </summary>
        public override void SuspendBinding()
        {
            lastGoodKnownRow = -1;
            if (shouldBind)
            {
                shouldBind = false;
                UpdateIsBinding();
            }
        }

        internal void UnwireEvents(IList list)
        {
            if ((list is IBindingList) && ((IBindingList)list).SupportsChangeNotification)
            {
                ((IBindingList)list).ListChanged -= new ListChangedEventHandler(List_ListChanged);
                /*
                ILiveList liveList = (ILiveList) list;
                liveList.TableChanged -= new TableChangedEventHandler(List_TableChanged);
                */
            }
        }

        protected override void UpdateIsBinding()
        {
            UpdateIsBinding(true);
        }

        private void UpdateIsBinding(bool raiseItemChangedEvent)
        {
            bool newBound = list != null && list.Count > 0 && shouldBind && listposition != -1;
            if (list != null)
            {
                if (bound != newBound)
                {
                    // we will call end edit when moving from bound state to unbounded state
                    //
                    //bool endCurrentEdit = bound && !newBound;
                    bound = newBound;
                    int newPos = newBound ? 0 : -1;
                    ChangeRecordState(newPos, bound, (Position != newPos), true, false);
                    int numLinks = Bindings.Count;
                    for (int i = 0; i < numLinks; i++)
                    {
                        Bindings[i].UpdateIsBinding();
                    }

                    if (raiseItemChangedEvent)
                    {
                        OnItemChanged(resetEvent);
                    }
                }
            }
        }

        private void UpdateLastGoodKnownRow(ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case System.ComponentModel.ListChangedType.ItemDeleted:
                    if (e.NewIndex == lastGoodKnownRow)
                    {
                        lastGoodKnownRow = -1;
                    }

                    break;
                case System.ComponentModel.ListChangedType.Reset:
                    lastGoodKnownRow = -1;
                    break;
                case System.ComponentModel.ListChangedType.ItemAdded:
                    if (e.NewIndex <= lastGoodKnownRow && lastGoodKnownRow < List.Count - 1)
                    {
                        lastGoodKnownRow++;
                    }

                    break;
                case System.ComponentModel.ListChangedType.ItemMoved:
                    if (e.OldIndex == lastGoodKnownRow)
                    {
                        lastGoodKnownRow = e.NewIndex;
                    }

                    break;
                case System.ComponentModel.ListChangedType.ItemChanged:
                    if (e.NewIndex == lastGoodKnownRow)
                    {
                        lastGoodKnownRow = -1;
                    }

                    break;
            }
        }

        internal void WireEvents(IList list)
        {
            if ((list is IBindingList) && ((IBindingList)list).SupportsChangeNotification)
            {
                ((IBindingList)list).ListChanged += new ListChangedEventHandler(List_ListChanged);
                /*
                ILiveList liveList = (ILiveList) list;
                liveList.TableChanged += new TableChangedEventHandler(List_TableChanged);
                */
            }
        }
    }
}

