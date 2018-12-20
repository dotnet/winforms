// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System;
    using Microsoft.Win32;
    using System.Diagnostics;    
    using System.Diagnostics.CodeAnalysis;
    using System.ComponentModel;
    using System.Collections;
    using System.Reflection;
    using System.Globalization;

    /// <include file='doc\ListManager.uex' path='docs/doc[@for="CurrencyManager"]/*' />
    /// <devdoc>
    ///    <para>Manages the position and bindings of a
    ///       list.</para>
    /// </devdoc>
    public class CurrencyManager : BindingManagerBase {

        private Object dataSource;
        private IList list;
        
        private bool bound = false;
        private bool shouldBind = true;
        
        /// <include file='doc\ListManager.uex' path='docs/doc[@for="CurrencyManager.listposition"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        [
            SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields") // We can't make CurrencyManager.listposition internal
                                                                                            // because it would be a breaking change.
        ]
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
        private ItemChangedEventArgs resetEvent = new ItemChangedEventArgs(-1);
        private EventHandler onMetaDataChangedHandler;

        /// <include file='doc\ListManager.uex' path='docs/doc[@for="CurrencyManager.finalType"]/*' />
        /// <devdoc>
        ///    <para>Gets the type of the list.</para>
        /// </devdoc>
        protected Type finalType;

        /// <include file='doc\ListManager.uex' path='docs/doc[@for="CurrencyManager.ItemChanged"]/*' />
        /// <devdoc>
        ///    <para>Occurs when the
        ///       current item has been
        ///       altered.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatData))]
        public event ItemChangedEventHandler ItemChanged {
            add {
                onItemChanged += value;
            }
            remove {
                onItemChanged -= value;
            }
        }

        /// <include file='doc\ListManager.uex' path='docs/doc[@for="CurrencyManager.ListChanged"]/*' />
        public event ListChangedEventHandler ListChanged {
            add {
                onListChanged += value;
            }
            remove {
                onListChanged -= value;
            }
        }

        /// <include file='doc\ListManager.uex' path='docs/doc[@for="CurrencyManager.CurrencyManager"]/*' />
        /// <devdoc>
        /// </devdoc>
        [
            SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")  // If the constructor does not set the dataSource
                                                                                                    // it would be a breaking change.
        ]
        internal CurrencyManager(Object dataSource) {
            SetDataSource(dataSource);
        }

        /// <devdoc>
        ///    <para>Gets a value indicating
        ///       whether items can be added to the list.</para>
        /// </devdoc>
        internal bool AllowAdd {
            get {
                if (list is IBindingList) {
                    return ((IBindingList)list).AllowNew;
                }
                if (list == null)
                    return false;
                return !list.IsReadOnly && !list.IsFixedSize;
            }
        }
        
        /// <devdoc>
        ///    <para>Gets a value
        ///       indicating whether edits to the list are allowed.</para>
        /// </devdoc>
        internal bool AllowEdit {
            get {
                if (list is IBindingList) {
                    return ((IBindingList)list).AllowEdit;
                }
                if (list == null)
                    return false;
                return !list.IsReadOnly;
            }
        }
        
        /// <devdoc>
        ///    <para>Gets a value indicating whether items can be removed from the list.</para>
        /// </devdoc>
        internal bool AllowRemove {
            get {
                if (list is IBindingList) {
                    return ((IBindingList)list).AllowRemove;
                }
                if (list == null)
                    return false;
                return !list.IsReadOnly && !list.IsFixedSize;
            }
        }
        
        /// <include file='doc\ListManager.uex' path='docs/doc[@for="CurrencyManager.Count"]/*' />
        /// <devdoc>
        ///    <para>Gets the number of items in the list.</para>
        /// </devdoc>
        public override int Count {
            get {
                if (list == null)
                    return 0;
                else
                    return list.Count;
            }
        }
        
        /// <include file='doc\ListManager.uex' path='docs/doc[@for="CurrencyManager.Current"]/*' />
        /// <devdoc>
        ///    <para>Gets the current item in the list.</para>
        /// </devdoc>
        public override Object Current {
            get {
                return this[Position];
            }
        }

        internal override Type BindType {
            get {
                return ListBindingHelper.GetListItemType(this.List);
            }
        }
        
        /// <devdoc>
        ///    <para>Gets the data source of the list.</para>
        /// </devdoc>
        internal override Object DataSource {
            get {
                return dataSource;
            }
        }

        internal override void SetDataSource(Object dataSource) {
            if (this.dataSource != dataSource) {
                Release();
                this.dataSource = dataSource;
                this.list = null;
                this.finalType = null;
                
                Object tempList = dataSource;
                if (tempList is Array) {
                    finalType = tempList.GetType();
                    tempList = (Array)tempList;
                }
                
                if (tempList is IListSource) {
                    tempList = ((IListSource)tempList).GetList();                    
                }
            
                if (tempList is IList) {
                    if (finalType == null) {
                        finalType = tempList.GetType();
                    }
                    this.list = (IList)tempList;
                    WireEvents(list);
                    if (list.Count > 0 )
                        listposition = 0;
                    else
                        listposition = -1;
                    OnItemChanged(resetEvent);
                    OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
                    UpdateIsBinding();
                }
                else {
                    if (tempList == null) {
                        throw new ArgumentNullException(nameof(dataSource));
                    }
                    throw new ArgumentException(string.Format(SR.ListManagerSetDataSource, tempList.GetType().FullName), "dataSource");
                }

            }
        }

        /// <include file='doc\ListManager.uex' path='docs/doc[@for="CurrencyManager.IsBinding"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>Gets a value indicating whether the list is bound to a data source.</para>
        /// </devdoc>
        internal override bool IsBinding {
            get {
                return bound;
            }
        }

        // The DataGridView needs this.
        internal bool ShouldBind {
            get {
                return shouldBind;
            }
        }

        /// <include file='doc\ListManager.uex' path='docs/doc[@for="CurrencyManager.List"]/*' />
        /// <devdoc>
        ///    <para>Gets the list as an object.</para>
        /// </devdoc>
        public IList List {
            get {
                // NOTE: do not change this to throw an exception if the list is not IBindingList.
                // doing this will cause a major performance hit when wiring the 
                // dataGrid to listen for MetaDataChanged events from the IBindingList
                // (basically we would have to wrap all calls to CurrencyManager::List with
                // a try/catch block.)
                //
                return list;
            }
        }

        /// <include file='doc\ListManager.uex' path='docs/doc[@for="CurrencyManager.Position"]/*' />
        /// <devdoc>
        ///    <para> Gets or sets the position you are at within the list.</para>
        /// </devdoc>
        public override int Position {
            get {
                return listposition;
            }
            set {
                if (listposition == -1)
                    return;

                if (value < 0)
                    value = 0;
                int count = list.Count;
                if (value >= count)
                    value = count - 1;

                ChangeRecordState(value, listposition != value, true, true, false);       // true for endCurrentEdit
                                                                                          // true for firingPositionChange notification
                                                                                          // data will be pulled from controls anyway.
            }
        }

        /// <devdoc>
        ///    <para>Gets or sets the object at the specified index.</para>
        /// </devdoc>
        internal Object this[int index] {
            get {
                if (index < 0 || index >= list.Count) {
                    throw new IndexOutOfRangeException(string.Format(SR.ListManagerNoValue, index.ToString(CultureInfo.CurrentCulture)));
                }
                return list[index];
            }
            set {
                if (index < 0 || index >= list.Count) {
                    throw new IndexOutOfRangeException(string.Format(SR.ListManagerNoValue, index.ToString(CultureInfo.CurrentCulture)));
                }
                list[index] = value;
            }
        }
        
        /// <include file='doc\ListManager.uex' path='docs/doc[@for="CurrencyManager.AddNew"]/*' />
        public override void AddNew() {
            IBindingList ibl = list as IBindingList;
            if (ibl != null) {
                ibl.AddNew();
            }
            else {
                // If the list is not IBindingList, then throw an exception:
                throw new NotSupportedException(SR.CurrencyManagerCantAddNew);
            }

            ChangeRecordState(list.Count - 1, (Position != list.Count - 1), (Position != list.Count - 1), true, true);  // true for firingPositionChangeNotification
                                                                                                                    // true for pulling data from the controls
        }

        /// <include file='doc\ListManager.uex' path='docs/doc[@for="CurrencyManager.CancelCurrentEdit"]/*' />
        /// <devdoc>
        ///    <para>Cancels the current edit operation.</para>
        /// </devdoc>
        public override void CancelCurrentEdit() {
            if (Count > 0) {
                Object item = (Position >= 0 && Position < list.Count) ? list[Position] : null;

                // onItemChangedCalled = false;

                IEditableObject iEditableItem = item as IEditableObject;
                if (iEditableItem != null) {
                    iEditableItem.CancelEdit();
                }

                ICancelAddNew iListWithCancelAddNewSupport = list as ICancelAddNew;
                if (iListWithCancelAddNewSupport != null) {
                    iListWithCancelAddNewSupport.CancelNew(this.Position);
                }

                OnItemChanged(new ItemChangedEventArgs(Position));
                if (this.Position != -1) {
                    OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, this.Position));
                }
            }
        }

        private void ChangeRecordState(int newPosition, bool validating, bool endCurrentEdit, bool firePositionChange, bool pullData) {
            if (newPosition == -1 && list.Count == 0) {
                if (listposition != -1) {
                    this.listposition = -1;
                    OnPositionChanged(EventArgs.Empty);
                }
                return;
            }
            
            if ((newPosition < 0 || newPosition >= Count) && this.IsBinding) {
                throw new IndexOutOfRangeException(SR.ListManagerBadPosition);
            }

            // if PushData fails in the OnCurrentChanged and there was a lastGoodKnownRow
            // then the position does not change, so we should not fire the OnPositionChanged
            // event;
            // this is why we have to cache the old position and compare that w/ the position that
            // the user will want to navigate to
            int oldPosition = listposition;
            if (endCurrentEdit) {
                // Do not PushData when pro.
                inChangeRecordState = true;
                try {
                    EndCurrentEdit();
                } finally {
                    inChangeRecordState = false;
                }
            }

            // we pull the data from the controls only when the ListManager changes the list. when the backEnd changes the list we do not 
            // pull the data from the controls
            if (validating && pullData) {
                CurrencyManager_PullData();
            }

            // EndCurrentEdit or PullData can cause the list managed by the CurrencyManager to shrink.
            this.listposition = Math.Min(newPosition, Count - 1);

            if (validating) {
                OnCurrentChanged(EventArgs.Empty);
            }
                
            bool positionChanging = (oldPosition != listposition);
            if (positionChanging && firePositionChange) {
                OnPositionChanged(EventArgs.Empty);
            }                
        }

        /// <include file='doc\ListManager.uex' path='docs/doc[@for="CurrencyManager.CheckEmpty"]/*' />
        /// <devdoc>
        ///    <para>Throws an exception if there is no list.</para>
        /// </devdoc>
        protected void CheckEmpty() {
            if (dataSource == null || list == null || list.Count == 0) {
                throw new InvalidOperationException(SR.ListManagerEmptyList);
            }
        }

        // will return true if this function changes the position in the list
        private bool CurrencyManager_PushData() {
            if (pullingData)
                return false;

            int initialPosition = listposition;
            if (lastGoodKnownRow == -1) {
                try {
                    PushData();
                }
                catch (Exception ex) {
                    OnDataError(ex);

                    // get the first item in the list that is good to push data
                    // for now, we assume that there is a row in the backEnd
                    // that is good for all the bindings.
                    FindGoodRow();
                }
                lastGoodKnownRow = listposition;
            } else {
                try {
                    PushData();
                }
                catch (Exception ex) {
                    OnDataError(ex);

                    listposition = lastGoodKnownRow;
                    PushData();
                }
                lastGoodKnownRow = listposition;
            }

            return initialPosition != listposition;
        }

        private bool CurrencyManager_PullData() {
            bool success = true;
            pullingData = true;

            try {
                PullData(out success);
            } finally {
                pullingData = false;
            }

            return success;
        }

        /// <include file='doc\ListManager.uex' path='docs/doc[@for="CurrencyManager.RemoveAt"]/*' />
        public override void RemoveAt(int index) {
            list.RemoveAt(index);
        }

        /// <include file='doc\ListManager.uex' path='docs/doc[@for="CurrencyManager.EndCurrentEdit"]/*' />
        /// <devdoc>
        ///    <para>Ends the current edit operation.</para>
        /// </devdoc>
        public override void EndCurrentEdit() {
            if (Count > 0) {
                bool success = CurrencyManager_PullData();

                if (success) {
                    Object item = (Position >= 0 && Position < list.Count) ? list[Position] : null;

                    IEditableObject iEditableItem = item as IEditableObject;
                    if (iEditableItem != null) {
                        iEditableItem.EndEdit();
                    }

                    ICancelAddNew iListWithCancelAddNewSupport = list as ICancelAddNew;
                    if (iListWithCancelAddNewSupport != null) {
                        iListWithCancelAddNewSupport.EndNew(this.Position);
                    }
                }
            }
        }

        private void FindGoodRow() {
            int rowCount = this.list.Count;
            for (int i = 0; i < rowCount; i++) {
                listposition = i;
                try {
                    PushData();
                }
                catch (Exception ex) {
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

        /// <devdoc>
        ///    <para>Sets the column to sort by, and the direction of the sort.</para>
        /// </devdoc>
        internal void SetSort(PropertyDescriptor property, ListSortDirection sortDirection) {
            if (list is IBindingList && ((IBindingList)list).SupportsSorting) {
                ((IBindingList)list).ApplySort(property, sortDirection);
            }
        }
        
        /// <devdoc>
        /// <para>Gets a <see cref='System.ComponentModel.PropertyDescriptor'/> for a CurrencyManager.</para>
        /// </devdoc>
        internal PropertyDescriptor GetSortProperty() {
            if ((list is IBindingList) && ((IBindingList)list).SupportsSorting) {
                return ((IBindingList)list).SortProperty;
            }
            return null;
        }

        /// <devdoc>
        ///    <para>Gets the sort direction of a list.</para>
        /// </devdoc>
        internal ListSortDirection GetSortDirection() {
            if ((list is IBindingList) && ((IBindingList)list).SupportsSorting) {
                return ((IBindingList)list).SortDirection;
            }
            return ListSortDirection.Ascending;
        }
                
        /// <devdoc>
        ///    <para>Find the position of a desired list item.</para>
        /// </devdoc>
        internal int Find(PropertyDescriptor property, Object key, bool keepIndex) {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (property != null && (list is IBindingList) && ((IBindingList)list).SupportsSearching) {
                return ((IBindingList)list).Find(property, key);
            }

            if (property != null) {
                for (int i = 0; i < list.Count; i++) {
                    object value = property.GetValue(list[i]);
                    if (key.Equals(value)) {
                        return i;
                    }
                }
            }

            return -1;
        }

        /// <devdoc>
        ///    <para>Gets the name of the list.</para>
        /// </devdoc>
        internal override string GetListName() {
            if (list is ITypedList) {
                return ((ITypedList)list).GetListName(null);
            }
            else {
                return finalType.Name;
            }
        }

        /// <include file='doc\ListManager.uex' path='docs/doc[@for="CurrencyManager.GetListName1"]/*' />
        /// <devdoc>
        ///    <para>Gets the name of the specified list.</para>
        /// </devdoc>
        protected internal override string GetListName(ArrayList listAccessors) {
            if (list is ITypedList) {
                PropertyDescriptor[] properties = new PropertyDescriptor[listAccessors.Count];
                listAccessors.CopyTo(properties, 0);
                return ((ITypedList)list).GetListName(properties);
            }
            return "";            
        }
        
        /// <devdoc>
        /// </devdoc>
        internal override PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
            return ListBindingHelper.GetListItemProperties(this.list, listAccessors);
        }

        /// <include file='doc\ListManager.uex' path='docs/doc[@for="CurrencyManager.GetItemProperties"]/*' />
        /// <devdoc>
        /// <para>Gets the <see cref='T:System.ComponentModel.PropertyDescriptorCollection'/> for
        ///    the list.</para>
        /// </devdoc>
        public override PropertyDescriptorCollection GetItemProperties() {
            return GetItemProperties(null);
        }

        /// <devdoc>
        /// <para>Gets the <see cref='T:System.ComponentModel.PropertyDescriptorCollection'/> for the specified list.</para>
        /// </devdoc>
        private void List_ListChanged(Object sender, System.ComponentModel.ListChangedEventArgs e) {
            // If you change the assert below, better change the 
            // code in the OnCurrentChanged that deals w/ firing the OnCurrentChanged event
            Debug.Assert(lastGoodKnownRow == -1 || lastGoodKnownRow == listposition, "if we have a valid lastGoodKnownRow, then it should equal the position in the list");

            // 





            ListChangedEventArgs dbe;

            if (e.ListChangedType == ListChangedType.ItemMoved && e.OldIndex < 0) {
                dbe = new ListChangedEventArgs(ListChangedType.ItemAdded, e.NewIndex, e.OldIndex);
            } else if (e.ListChangedType == ListChangedType.ItemMoved && e.NewIndex < 0) {
                dbe = new ListChangedEventArgs(ListChangedType.ItemDeleted, e.OldIndex, e.NewIndex);
            } else {
                dbe = e;
            }

            int oldposition = listposition;

            UpdateLastGoodKnownRow(dbe);
            UpdateIsBinding();

            if (list.Count == 0) {
                listposition = -1;

                if (oldposition != -1) {
                    // if we used to have a current row, but not any more, then report current as changed
                    OnPositionChanged(EventArgs.Empty);
                    OnCurrentChanged(EventArgs.Empty);
                }

                if (dbe.ListChangedType == System.ComponentModel.ListChangedType.Reset && e.NewIndex == -1) {
                    // if the list is reset, then let our users know about it.
                    OnItemChanged(resetEvent);
                }

                if (dbe.ListChangedType == System.ComponentModel.ListChangedType.ItemDeleted) {
                    // if the list is reset, then let our users know about it.
                    OnItemChanged(resetEvent);
                }

                // we should still fire meta data change notification even when the list is empty
                if (e.ListChangedType == System.ComponentModel.ListChangedType.PropertyDescriptorAdded ||
                    e.ListChangedType == System.ComponentModel.ListChangedType.PropertyDescriptorDeleted ||
                    e.ListChangedType == System.ComponentModel.ListChangedType.PropertyDescriptorChanged)
                    OnMetaDataChanged(EventArgs.Empty);

                // 


                OnListChanged(dbe);
                return;
            }
            
            suspendPushDataInCurrentChanged = true;
            try {
                switch (dbe.ListChangedType) {
                    case System.ComponentModel.ListChangedType.Reset:
                        Debug.WriteLineIf(CompModSwitches.DataCursor.TraceVerbose, "System.ComponentModel.ListChangedType.Reset Position: " + Position + " Count: " + list.Count);
                        if (listposition == -1 && list.Count > 0)
                            ChangeRecordState(0, true, false, true, false);     // last false: we don't pull the data from the control when DM changes
                        else 
                            ChangeRecordState(Math.Min(listposition,list.Count - 1), true, false, true, false);
                        UpdateIsBinding(/*raiseItemChangedEvent:*/ false);
                        OnItemChanged(resetEvent);
                        break;
                    case System.ComponentModel.ListChangedType.ItemAdded:
                        Debug.WriteLineIf(CompModSwitches.DataCursor.TraceVerbose, "System.ComponentModel.ListChangedType.ItemAdded " + dbe.NewIndex.ToString(CultureInfo.InvariantCulture));
                        if (dbe.NewIndex <= listposition && listposition < list.Count - 1) {
                            // this means the current row just moved down by one.
                            // the position changes, so end the current edit
                            ChangeRecordState(listposition + 1, true, true, listposition != list.Count - 2, false);
                            UpdateIsBinding();
                            // refresh the list after we got the item added event
                            OnItemChanged(resetEvent);
                            // when we get the itemAdded, and the position was at the end
                            // of the list, do the right thing and notify the positionChanged after refreshing the list
                            if (listposition == list.Count - 1)
                                OnPositionChanged(EventArgs.Empty);
                            break;
                        } else if (dbe.NewIndex == this.listposition && this.listposition == list.Count - 1 && this.listposition != -1) {
                            // The CurrencyManager has a non-empty list.
                            // The position inside the currency manager is at the end of the list and the list still fired an ItemAdded event.
                            // This could be the second ItemAdded event that the DataView fires to signal that the AddNew operation was commited.
                            // We need to fire CurrentItemChanged event so that relatedCurrencyManagers update their lists.
                            OnCurrentItemChanged(System.EventArgs.Empty);
                        }

                        if (listposition == -1) {
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
                        if (dbe.NewIndex == listposition) {
                            // this means that the current row got deleted.
                            // cannot end an edit on a row that does not exist anymore
                            ChangeRecordState(Math.Min(listposition, Count - 1), true, false, true, false);
                            // put the call to OnItemChanged after setting the position
                            // in the currencyManager, so controls will use the actual position
                            OnItemChanged(resetEvent);
                            break;
                           
                        }
                        if (dbe.NewIndex < listposition) {
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
                        if (dbe.NewIndex == this.listposition) {
                            OnCurrentItemChanged(EventArgs.Empty);
                        }

                        OnItemChanged(new ItemChangedEventArgs(dbe.NewIndex));
                        break;
                    case System.ComponentModel.ListChangedType.ItemMoved:
                        Debug.WriteLineIf(CompModSwitches.DataCursor.TraceVerbose, "System.ComponentModel.ListChangedType.ItemMoved " + dbe.NewIndex.ToString(CultureInfo.InvariantCulture));
                        if (dbe.OldIndex == listposition) { // current got moved.
                            // the position changes, so end the current edit. Make sure there is something that we can end edit...
                            ChangeRecordState(dbe.NewIndex, true, this.Position > -1 && this.Position < list.Count, true, false);
                        }
                        else if (dbe.NewIndex == listposition) { // current was moved
                            // the position changes, so end the current edit. Make sure there is something that we can end edit
                            ChangeRecordState(dbe.OldIndex, true, this.Position > -1 && this.Position < list.Count, true, false);
                        }
                        OnItemChanged(resetEvent);
                        break;
                    case System.ComponentModel.ListChangedType.PropertyDescriptorAdded:
                    case System.ComponentModel.ListChangedType.PropertyDescriptorDeleted:
                    case System.ComponentModel.ListChangedType.PropertyDescriptorChanged:
                        // reset lastGoodKnownRow because it was computed against property descriptors which changed
                        this.lastGoodKnownRow = -1;

                        // In Everett, metadata changes did not alter current list position. In Whidbey, this behavior
                        // preserved - except that we will now force the position to stay in valid range if necessary.
                        if (listposition == -1 && list.Count > 0)
                            ChangeRecordState(0, true, false, true, false);
                        else if (listposition > list.Count - 1)
                            ChangeRecordState(list.Count - 1, true, false, true, false);

                        // fire the MetaDataChanged event
                        OnMetaDataChanged(EventArgs.Empty);
                        break;
                }
                // send the ListChanged notification after the position changed in the list
                // 


                OnListChanged(dbe);
            } finally {
                suspendPushDataInCurrentChanged = false;
            }
            Debug.Assert(lastGoodKnownRow == -1 || listposition == lastGoodKnownRow, "how did they get out of sync?");
        }

        /// <include file='doc\ListManager.uex' path='docs/doc[@for="CurrencyManager.MetaDataChanged"]/*' />
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly")] //Exists in Everett
        [SRCategory(nameof(SR.CatData))]
        public event EventHandler MetaDataChanged {
            add {
                onMetaDataChangedHandler += value;
            }
            remove {
                onMetaDataChangedHandler -= value;
            }
        }

        /// <include file='doc\ListManager.uex' path='docs/doc[@for="CurrencyManager.OnCurrentChanged"]/*' />
        /// <devdoc>
        /// <para>Causes the CurrentChanged event to occur. </para>
        /// </devdoc>
        internal protected override void OnCurrentChanged(EventArgs e) {
            if (!inChangeRecordState) {
                Debug.WriteLineIf(CompModSwitches.DataView.TraceVerbose, "OnCurrentChanged() " + e.ToString());
                int curLastGoodKnownRow = lastGoodKnownRow;
                bool positionChanged = false;
                if (!suspendPushDataInCurrentChanged)
                    positionChanged = CurrencyManager_PushData();
                if (Count > 0) {
                    Object item = list[Position];
                    if (item is IEditableObject) {
                        ((IEditableObject)item).BeginEdit();
                    }
                }
                try {
                    // if currencyManager changed position then we have two cases:
                    // 1. the previous lastGoodKnownRow was valid: in that case we fell back so do not fire onCurrentChanged
                    // 2. the previous lastGoodKnownRow was invalid: we have two cases:
                    //      a. FindGoodRow actually found a good row, so it can't be the one before the user changed the position: fire the onCurrentChanged
                    //      b. FindGoodRow did not find a good row: we should have gotten an exception so we should not even execute this code
                    if (!positionChanged ||(positionChanged && curLastGoodKnownRow != -1)) {
                        if (onCurrentChangedHandler != null) {
                            onCurrentChangedHandler(this, e);
                        }

                        // we fire OnCurrentItemChanged event every time we fire the CurrentChanged + when a property of the Current item changed
                        if (onCurrentItemChangedHandler != null) {
                            onCurrentItemChangedHandler(this, e);
                        }
                    }
                }
                catch (Exception ex) {
                    OnDataError(ex);
                }                    
            }
        }

        // this method should only be called when the currency manager receives the ListChangedType.ItemChanged event
        // and when the index of the ListChangedEventArgs == the position in the currency manager
        /// <include file='doc\ListManager.uex' path='docs/doc[@for="CurrencyManager.OnCurrentItemChanged"]/*' />
        internal protected override void OnCurrentItemChanged(EventArgs e) {
            if (onCurrentItemChangedHandler != null) {
                onCurrentItemChangedHandler(this, e);
            }
        }
        
        /// <include file='doc\ListManager.uex' path='docs/doc[@for="CurrencyManager.OnItemChanged"]/*' />
        /// <devdoc>
        /// </devdoc>
        protected virtual void OnItemChanged(ItemChangedEventArgs e) {
            // It is possible that CurrencyManager_PushData will change the position
            // in the list. in that case we have to fire OnPositionChanged event
            bool positionChanged = false;

            // We should not push the data when we suspend the changeEvents.
            // but we should still fire the OnItemChanged event that we get when processing the EndCurrentEdit method.
            if ((e.Index == listposition || (e.Index == -1 && Position < Count)) && !inChangeRecordState)
                positionChanged = CurrencyManager_PushData();
            Debug.WriteLineIf(CompModSwitches.DataView.TraceVerbose, "OnItemChanged(" + e.Index.ToString(CultureInfo.InvariantCulture) + ") " + e.ToString());
            try {
                if (onItemChanged != null)
                    onItemChanged(this, e);
            }
            catch (Exception ex) {
                OnDataError(ex);
            }

            if (positionChanged)
                OnPositionChanged(EventArgs.Empty);
            // onItemChangedCalled = true;
        }

        private void OnListChanged(ListChangedEventArgs e) {
            if (onListChanged != null)
                onListChanged(this, e);
        }

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly")] //Exists in Everett
        internal protected void OnMetaDataChanged(EventArgs e)
        {
            if (onMetaDataChangedHandler != null)
                onMetaDataChangedHandler(this,e);
        }

        /// <include file='doc\ListManager.uex' path='docs/doc[@for="CurrencyManager.OnPositionChanged"]/*' />
        /// <devdoc>
        /// </devdoc>
        protected virtual void OnPositionChanged(EventArgs e) {
            // if (!inChangeRecordState) {
                Debug.WriteLineIf(CompModSwitches.DataView.TraceVerbose, "OnPositionChanged(" + listposition.ToString(CultureInfo.InvariantCulture) + ") " + e.ToString());
                try {
                    if (onPositionChangedHandler != null)
                        onPositionChangedHandler(this, e);
                }
                catch (Exception ex) {
                    OnDataError(ex);
                }
            // }
        }

        /// <include file='doc\ListManager.uex' path='docs/doc[@for="CurrencyManager.Refresh"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Forces a repopulation of the CurrencyManager
        ///    </para>
        /// </devdoc>
        public void Refresh() { 
            if (list.Count > 0 ) {
                if (listposition >= list.Count) {
                    lastGoodKnownRow = -1;
                    listposition = 0;
                }
            } else {
                listposition = -1;
            }
            List_ListChanged(list, new System.ComponentModel.ListChangedEventArgs(System.ComponentModel.ListChangedType.Reset, -1));
        }

        internal void Release() {
            UnwireEvents(list);
        }

        /// <include file='doc\ListManager.uex' path='docs/doc[@for="CurrencyManager.ResumeBinding"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>Resumes binding of component properties to list items.</para>
        /// </devdoc>
        public override void ResumeBinding() {
            lastGoodKnownRow = -1;
            try {
                if (!shouldBind) {
                    shouldBind = true;
                    // we need to put the listPosition at the beginning of the list if the list is not empty
                    this.listposition = (this.list != null && this.list.Count != 0) ? 0:-1;
                    UpdateIsBinding();
                }
            }
            catch {
                shouldBind = false;
                UpdateIsBinding();
                throw;
            }
        }

        /// <include file='doc\ListManager.uex' path='docs/doc[@for="CurrencyManager.SuspendBinding"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>Suspends binding.</para>
        /// </devdoc>
        public override void SuspendBinding() {
            lastGoodKnownRow = -1;
            if (shouldBind) {
                shouldBind = false;
                UpdateIsBinding();
            }
        }

        internal void UnwireEvents(IList list) {
            if ((list is IBindingList) && ((IBindingList)list).SupportsChangeNotification) {
                ((IBindingList)list).ListChanged -= new System.ComponentModel.ListChangedEventHandler(List_ListChanged);
                /*
                ILiveList liveList = (ILiveList) list;
                liveList.TableChanged -= new TableChangedEventHandler(List_TableChanged);
                */
            }
        }
        
        /// <include file='doc\ListManager.uex' path='docs/doc[@for="CurrencyManager.UpdateIsBinding"]/*' />
        protected override void UpdateIsBinding() {
            UpdateIsBinding(true);
        }

        private void UpdateIsBinding(bool raiseItemChangedEvent) {
            bool newBound = list != null && list.Count > 0 && shouldBind && listposition != -1;
            if (list != null)
            if (bound != newBound) {
                // we will call end edit when moving from bound state to unbounded state
                //
                //bool endCurrentEdit = bound && !newBound;
                bound = newBound;
                int newPos = newBound ? 0 : -1;
                ChangeRecordState(newPos, bound, (Position != newPos), true, false);
                int numLinks = Bindings.Count;
                for (int i = 0; i < numLinks; i++) {
                    Bindings[i].UpdateIsBinding();
                }

                if (raiseItemChangedEvent) {
                    OnItemChanged(resetEvent);
                }
            }
        }

        private void UpdateLastGoodKnownRow(System.ComponentModel.ListChangedEventArgs e) {
            switch (e.ListChangedType) {
                case System.ComponentModel.ListChangedType.ItemDeleted:
                    if (e.NewIndex == lastGoodKnownRow)
                        lastGoodKnownRow = -1;
                    break;
                case System.ComponentModel.ListChangedType.Reset:
                    lastGoodKnownRow = -1;
                    break;
                case System.ComponentModel.ListChangedType.ItemAdded:
                    if (e.NewIndex <= lastGoodKnownRow && lastGoodKnownRow < this.List.Count - 1)
                        lastGoodKnownRow ++;
                    break;
                case System.ComponentModel.ListChangedType.ItemMoved:
                    if (e.OldIndex == lastGoodKnownRow)
                        lastGoodKnownRow = e.NewIndex;
                    break;
                case System.ComponentModel.ListChangedType.ItemChanged:
                    if (e.NewIndex == lastGoodKnownRow)
                        lastGoodKnownRow = -1;
                    break;
            }
        }

        internal void WireEvents(IList list) {
            if ((list is IBindingList) && ((IBindingList)list).SupportsChangeNotification) {
                ((IBindingList)list).ListChanged += new System.ComponentModel.ListChangedEventHandler(List_ListChanged);
                /*
                ILiveList liveList = (ILiveList) list;
                liveList.TableChanged += new TableChangedEventHandler(List_TableChanged);
                */
            }
        }
    }
}

