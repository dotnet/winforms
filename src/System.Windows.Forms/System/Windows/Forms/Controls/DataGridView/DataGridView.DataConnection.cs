// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace System.Windows.Forms;

public partial class DataGridView
{
    internal class DataGridViewDataConnection
    {
        private readonly DataGridView _owner;
        private PropertyDescriptorCollection? _props;
        private int _lastListCount = -1;

        // Data connection state variables
        private BitVector32 _dataConnectionState;

        private const int DATACONNECTIONSTATE_dataConnection_inSetDataConnection = 0x00000001;
        private const int DATACONNECTIONSTATE_processingMetaDataChanges = 0x00000002;

        // AddNew
        private const int DATACONNECTIONSTATE_finishedAddNew = 0x00000004;

        private const int DATACONNECTIONSTATE_doNotChangePositionInTheDataGridViewControl = 0x00000008;

        // DataGridView::SetCurrentCellAddressCore makes the current row unavailable during the OnRowEnter event.
        // We use the doNotChangePositionInTheCurrencyManager flag to go around this.
        private const int DATACONNECTIONSTATE_doNotChangePositionInTheCurrencyManager = 0x00000010;

        private const int DATACONNECTIONSTATE_interestedInRowEvents = 0x00000020;
        private const int DATACONNECTIONSTATE_cancellingRowEdit = 0x00000040;
        private const int DATACONNECTIONSTATE_restoreRow = 0x00000080;
        private const int DATACONNECTIONSTATE_rowValidatingInAddNew = 0x00000100;
        private const int DATACONNECTIONSTATE_inAddNew = 0x00000200;
        private const int DATACONNECTIONSTATE_listWasReset = 0x00000400;
        private const int DATACONNECTIONSTATE_positionChangingInCurrencyManager = 0x00000800;

        //
        // The following three constants deal w/ the following situation:
        // This is Master-Details schema.
        // One DGV is bound to Master, another DGV is bound to Details.
        // Master has 1 row.
        // The user deletes the one and only row from Master
        //
        // Then the following sequence of Events happen:
        // 1. DGV deletes the row from Master
        // 2. The Child currency manager finds out that there are no rows in the Master table
        // 3. The Child currency manager adds a row in the Master table - which tracks removal of this feature was POSTPONED.
        // 4. The DGV bound to the Master table receives the ItemAdded event. At this point, no rows have been deleted
        //    from the DGV.
        // 5. The DGV bound to the Master table should not add a new DataGridViewRow to its Rows collection because
        //    it will be deleted later on. So the DGV marks _itemAddedInDeleteOperation to TRUE to know that the next
        //    event it expects is an ItemDeleted
        // 6. The DGV bound to the Master table receives the ItemDeleted event.
        //    It goes ahead and deletes the item and resets _itemAddedInDeleteOperation
        //
        private const int DATACONNECTIONSTATE_inDeleteOperation = 0x00001000;
        private const int DATACONNECTIONSTATE_didNotDeleteRowFromDataGridView = 0x00002000;
        private const int DATACONNECTIONSTATE_itemAddedInDeleteOperation = 0x00004000;

        // This constant is used to know if EndCurentEdit caused an item to be deleted from the back end
        private const int DATACONNECTIONSTATE_inEndCurrentEdit = 0x00008000;

        // We need to cache the value of AllowUserToAddRowsInternal because it may change outside the DataGridView.
        // When the DataGridView catches this change it will refresh its rows collection, no questions asked.
        private const int DATACONNECTIONSTATE_cachedAllowUserToAddRowsInternal = 0x00010000;

        private const int DATACONNECTIONSTATE_processingListChangedEvent = 0x00020000;

        private const int DATACONNECTIONSTATE_dataSourceInitializedHookedUp = 0x00040000;

        public DataGridViewDataConnection(DataGridView owner)
        {
            _owner = owner;
            _dataConnectionState = new BitVector32(DATACONNECTIONSTATE_finishedAddNew);
        }

        public bool AllowAdd
        {
            get
            {
                // We only allow to add new rows on an IBindingList.
                return CurrencyManager is { List: IBindingList { SupportsChangeNotification: true }, AllowAdd: true };
            }
        }

        public bool AllowEdit => CurrencyManager is not null && CurrencyManager.AllowEdit;

        public bool AllowRemove
        {
            get
            {
                // We only allow deletion on an IBindingList.
                return CurrencyManager is { AllowRemove: true, List: IBindingList { SupportsChangeNotification: true } };
            }
        }

        public bool CancellingRowEdit => _dataConnectionState[DATACONNECTIONSTATE_cancellingRowEdit];

        public CurrencyManager? CurrencyManager { get; private set; }

        public string DataMember { get; private set; } = string.Empty;

        public object? DataSource { get; private set; }

        public bool DoNotChangePositionInTheCurrencyManager
        {
            get => _dataConnectionState[DATACONNECTIONSTATE_doNotChangePositionInTheCurrencyManager];
            set => _dataConnectionState[DATACONNECTIONSTATE_doNotChangePositionInTheCurrencyManager] = value;
        }

        public bool InterestedInRowEvents =>
            _dataConnectionState[DATACONNECTIONSTATE_interestedInRowEvents];

        public IList? List => CurrencyManager?.List;

        public bool ListWasReset =>
            _dataConnectionState[DATACONNECTIONSTATE_listWasReset];

        public bool PositionChangingOutsideDataGridView
        {
            get
            {
                // DATACONNECTIONSTATE_doNotChangePositionInTheDataGridViewControl means that the data grid view control
                // manages the position change
                // so if DATACONNECTIONSTATE_doNotChangePositionInTheDataGridViewControl is true then the
                // data grid view knows about the position change
                return !_dataConnectionState[DATACONNECTIONSTATE_doNotChangePositionInTheDataGridViewControl]
                    && _dataConnectionState[DATACONNECTIONSTATE_positionChangingInCurrencyManager];
            }
        }

        public bool ProcessingListChangedEvent =>
            _dataConnectionState[DATACONNECTIONSTATE_processingListChangedEvent];

        public bool ProcessingMetaDataChanges =>
            _dataConnectionState[DATACONNECTIONSTATE_processingMetaDataChanges];

        public bool RestoreRow
        {
            get
            {
                Debug.Assert(_dataConnectionState[DATACONNECTIONSTATE_cancellingRowEdit]);
                return _dataConnectionState[DATACONNECTIONSTATE_restoreRow];
            }
        }

        public void AddNew()
        {
            if (CurrencyManager is not null)
            {
                // don't call AddNew on a suspended currency manager.
                if (!CurrencyManager.ShouldBind)
                {
                    return;
                }

                Debug.Assert(CurrencyManager.AllowAdd, "why did we call AddNew on the currency manager when the currency manager does not allow new rows?");
                _dataConnectionState[DATACONNECTIONSTATE_finishedAddNew] = false;

                _dataConnectionState[DATACONNECTIONSTATE_inEndCurrentEdit] = true;
                try
                {
                    CurrencyManager.EndCurrentEdit();
                }
                finally
                {
                    _dataConnectionState[DATACONNECTIONSTATE_inEndCurrentEdit] = false;
                }

                _dataConnectionState[DATACONNECTIONSTATE_inAddNew] = true;

                try
                {
                    CurrencyManager.AddNew();
                }
                finally
                {
                    _dataConnectionState[DATACONNECTIONSTATE_inAddNew] = false;
                }
            }
        }

        //
        // This method pulls the information about which dataField is sorted on the IBindingList
        // and applies it to the DataGridView.
        //
        // Here is how it does that:
        //      1. Updating the DataGridView::SortedColumn property:
        //          When multiple columns are bound to a sorted column
        //          in the backend then the DataGridView::SortedColumn property should return the
        //          first column in index order that is sorted. For example, if the datasource is sorted on CustomerID and two
        //          CustomerID columns are in the grid at index 0 and 5, then SortedColumn should return the DGVColumn at index 0.
        //      2. Changes to DataGridView::SortGlyphDirection.
        //          Go thru all the data bound columns on the back end and if they map to the sorted dataField
        //          set their SortGlyphDirection to the sort direction on the back end.
        //
        // Note: on IBindingList there is only one column that can be sorted.
        // So if the back end is an IBindingView ( which supports sorting on multiple columns ) this code will not take into
        // account the case that multiple columns are sorted.
        //
        public void ApplySortingInformationFromBackEnd()
        {
            if (CurrencyManager is null)
            {
                return;
            }

            GetSortingInformationFromBackend(out PropertyDescriptor? sortField, out SortOrder sortOrder);

            // If we are not bound to a sorted IBindingList then set the SortGlyphDirection to SortOrder.None
            // on each dataBound DataGridViewColumn.
            // This will have the side effect of setting DataGridView::SortedColumn to null and setting DataGridView::SortOrder to null.
            if (sortField is null)
            {
                for (int i = 0; i < _owner.Columns.Count; i++)
                {
                    if (_owner.Columns[i].IsDataBound)
                    {
                        _owner.Columns[i].HeaderCell.SortGlyphDirection = SortOrder.None;
                    }
                }

                _owner.SortedColumn = null;
                _owner.SortOrder = SortOrder.None;

                // now return;
                return;
            }

            bool setSortedColumnYet = false;
            for (int i = 0; i < _owner.Columns.Count; i++)
            {
                DataGridViewColumn column = _owner.Columns[i];
                if (!column.IsDataBound)
                {
                    continue;
                }

                if (column.SortMode == DataGridViewColumnSortMode.NotSortable)
                {
                    continue;
                }

                if (string.Equals(column.DataPropertyName, sortField.Name, StringComparison.OrdinalIgnoreCase))
                {
                    // Set the sorted column on the dataGridView only if the sorted Field is set outside the dataGridView.
                    // If the sortedField is set inside the dataGridView ( either by user clicking on a ColumnHeader or by user calling DGV.Sort(...)
                    // then we don't want to tamper w/ it.
                    if (!setSortedColumnYet && !_owner.InSortOperation)
                    {
                        _owner.SortedColumn = column;
                        _owner.SortOrder = sortOrder;
                        setSortedColumnYet = true;
                    }

                    // set the SortGlyphDirection on the data bound DataGridViewColumn
                    column.HeaderCell.SortGlyphDirection = sortOrder;
                }
                else
                {
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                }
            }
        }

        public TypeConverter BoundColumnConverter(int boundColumnIndex)
        {
            Debug.Assert(_props is not null);
            return _props[boundColumnIndex].Converter;
        }

        // given a data field name we get the bound index
        public int BoundColumnIndex(string dataPropertyName)
        {
            if (_props is null)
            {
                return -1;
            }

            int ret = -1;

            for (int i = 0; i < _props.Count; i++)
            {
                if (string.Compare(_props[i].Name, dataPropertyName, ignoreCase: true, CultureInfo.InvariantCulture) == 0)
                {
                    ret = i;
                    break;
                }
            }

            return ret;
        }

        public SortOrder BoundColumnSortOrder(int boundColumnIndex)
        {
            if (CurrencyManager?.List is not IBindingList { SupportsSorting: true, IsSorted: true })
            {
                return SortOrder.None;
            }

            GetSortingInformationFromBackend(out PropertyDescriptor? sortProperty, out SortOrder sortOrder);

            if (sortOrder == SortOrder.None)
            {
                Debug.Assert(sortProperty is null);
                return SortOrder.None;
            }

            if (string.Compare(_props![boundColumnIndex].Name, sortProperty!.Name, ignoreCase: true, CultureInfo.InvariantCulture) == 0)
            {
                return sortOrder;
            }
            else
            {
                return SortOrder.None;
            }
        }

        public Type BoundColumnValueType(int boundColumnIndex)
        {
            Debug.Assert(_props is not null);
            return _props[boundColumnIndex].PropertyType;
        }

#if DEBUG
        private void CheckRowCount(ListChangedEventArgs e)
        {
            if (e.ListChangedType != ListChangedType.Reset)
            {
                return;
            }

            int dataGridViewRowsCount = _owner.Rows.Count;

            Debug.Assert(DataBoundRowsCount() == CurrencyManager!.List!.Count || (_owner.Columns.Count == 0 && dataGridViewRowsCount == 0),
                         "there should be the same number of rows in the dataGridView's Row Collection as in the back end list");
        }
#endif // DEBUG

        private void currencyManager_ListChanged(object? sender, ListChangedEventArgs e)
        {
            Debug.Assert(sender == CurrencyManager, "did we forget to unregister our ListChanged event handler?");

            _dataConnectionState[DATACONNECTIONSTATE_processingListChangedEvent] = true;
            try
            {
                ProcessListChanged(e);
            }
            finally
            {
                _dataConnectionState[DATACONNECTIONSTATE_processingListChangedEvent] = false;
            }

            _owner.OnDataBindingComplete(e.ListChangedType);

            _lastListCount = CurrencyManager!.Count;

#if DEBUG
            CheckRowCount(e);
#endif // DEBUG
        }

        private void ProcessListChanged(ListChangedEventArgs e)
        {
            if (e.ListChangedType is ListChangedType.PropertyDescriptorAdded
                or ListChangedType.PropertyDescriptorDeleted
                or ListChangedType.PropertyDescriptorChanged)
            {
                _dataConnectionState[DATACONNECTIONSTATE_processingMetaDataChanges] = true;
                try
                {
                    DataSourceMetaDataChanged();
                }
                finally
                {
                    _dataConnectionState[DATACONNECTIONSTATE_processingMetaDataChanges] = false;
                }

                return;
            }

            Debug.Assert(!_dataConnectionState[DATACONNECTIONSTATE_inAddNew] || !_dataConnectionState[DATACONNECTIONSTATE_finishedAddNew],
                         "if inAddNew is true then finishedAddNew should be false");

            // The value of AllowUserToAddRowsInternal changed under the DataGridView.
            // Recreate the rows and return.
            if (_dataConnectionState[DATACONNECTIONSTATE_cachedAllowUserToAddRowsInternal] != _owner.AllowUserToAddRowsInternal)
            {
                _dataConnectionState[DATACONNECTIONSTATE_listWasReset] = true;
                try
                {
                    _owner.RefreshRows(scrollIntoView: !_owner.InSortOperation);
                    _owner.PushAllowUserToAddRows();
                }
                finally
                {
                    // this will also set DATACONNECTIONSTATE_listWasReset to false
                    ResetDataConnectionState();
                }

                return;
            }

            // if the list changed the AddNew and we did not finish the AddNew operation then
            // finish it now and return
            if (!_dataConnectionState[DATACONNECTIONSTATE_finishedAddNew] && _owner.NewRowIndex == e.NewIndex)
            {
                Debug.Assert(_owner.AllowUserToAddRowsInternal, "how did we start the add new transaction when the AllowUserToAddRowsInternal is false?");
                if (e.ListChangedType == ListChangedType.ItemAdded)
                {
                    if (_dataConnectionState[DATACONNECTIONSTATE_inAddNew])
                    {
                        // still processing CurrencyManager::AddNew
                        // nothing to do
                        return;
                    }

                    if (_dataConnectionState[DATACONNECTIONSTATE_rowValidatingInAddNew])
                    {
                        // DataGridView validation commited the AddNewRow to the back end
                        // DataGridView took care of newRowIndex, adding a new DataGridViewRow, etc
                        // we don't have to do anything
                        return;
                    }

                    // We got a ListChangedType.ItemAdded event outside row validation and outside CurrencyManager::AddNew
                    if (_owner.Columns.Count > 0)
                    {
                        // add rows until the back end and the DGV have the same number of bound rows.
                        do
                        {
                            // the new row becomes a regular row and a "new" new row is appended
                            _owner.NewRowIndex = -1;
                            _owner.AddNewRow(createdByEditing: false);
                        }
                        while (DataBoundRowsCount() < CurrencyManager!.Count);
                    }

                    _dataConnectionState[DATACONNECTIONSTATE_finishedAddNew] = true;
                    MatchCurrencyManagerPosition(scrollIntoView: true, clearSelection: true);
                    return;
                }
                else if (e.ListChangedType == ListChangedType.ItemDeleted)
                {
                    if (_dataConnectionState[DATACONNECTIONSTATE_cancellingRowEdit])
                    {
                        // 'add new row' was discarded, bring back the new row default values.
                        _owner.PopulateNewRowWithDefaultValues();
                    }
                    else if (_dataConnectionState[DATACONNECTIONSTATE_inEndCurrentEdit] ||
                             _dataConnectionState[DATACONNECTIONSTATE_inAddNew])
                    {
                        // A row was deleted while the DataGridView control asked for a new row.
                        // Recreate the data grid view rows.
                        _dataConnectionState[DATACONNECTIONSTATE_listWasReset] = true;
                        try
                        {
                            _owner.RefreshRows(scrollIntoView: !_owner.InSortOperation);
                            _owner.PushAllowUserToAddRows();
                        }
                        finally
                        {
                            _dataConnectionState[DATACONNECTIONSTATE_listWasReset] = false;
                        }
                    }
                    else
                    {
                        Debug.Assert(CurrencyManager?.List is not null);
                        if (_dataConnectionState[DATACONNECTIONSTATE_inDeleteOperation] && CurrencyManager.List.Count == 0)
                        {
                            // if System.Data.DataView was in AddNew transaction and we delete all the rows in the System.Data.DataView
                            // then System.Data.DataView will close the AddNew transaction under us
                            // start another AddNew transaction on the back end
                            AddNew();
                        }
                    }
                }

                return;
            }

            Debug.Assert(DataBoundRowsCount() != -1, "the data bound data grid view rows count should be at least 0");

            // we received an ListChangedType.ItemAdded and our list has exactly the same number of rows as the back-end.
            // return.
            Debug.Assert(CurrencyManager?.List is not null);
            if (e.ListChangedType == ListChangedType.ItemAdded
                && CurrencyManager.List.Count == (_owner.AllowUserToAddRowsInternal ? _owner.Rows.Count - 1 : _owner.Rows.Count))
            {
                if (_dataConnectionState[DATACONNECTIONSTATE_inDeleteOperation] && _dataConnectionState[DATACONNECTIONSTATE_didNotDeleteRowFromDataGridView])
                {
                    // we received a ListChangedType.ItemAdded while we were deleting rows from the back end
                    // and we still haven't removed a row from the data grid view
                    // System.Data.DataView started an AddNew transaction as a result of deleting rows
                    // mark the state as itemAddedInDeleteOperation
                    _dataConnectionState[DATACONNECTIONSTATE_itemAddedInDeleteOperation] = true;

                    // The DGV gets in this situation when the user deletes the last row in a Master table.
                    // At this point, the Child table forces an AddNew on the Master Table.
                    // See comments where we declare _itemAddedInDeleteOperation");
                    //
                    Debug.Assert(CurrencyManager.List.Count == 1);

                    // if we were on an AddNew transaction then the MASTER table would have had more than 1 row.
                    // So the Child table should not have forcefully added a row on the MASTER table");
                    //
                    Debug.Assert(_dataConnectionState[DATACONNECTIONSTATE_finishedAddNew]);
                }

                return;
            }

            // this is the first ItemDeleted event we get after the ItemAdded event that we got while we were deleting rows from the data view
            // don't do anything - this is the equivalent of removing the row that was added before
            if (e.ListChangedType == ListChangedType.ItemDeleted)
            {
                if (_dataConnectionState[DATACONNECTIONSTATE_inDeleteOperation]
                    && _dataConnectionState[DATACONNECTIONSTATE_itemAddedInDeleteOperation]
                    && _dataConnectionState[DATACONNECTIONSTATE_didNotDeleteRowFromDataGridView])
                {
                    // we removed the item that was added during the delete operation
                    _dataConnectionState[DATACONNECTIONSTATE_itemAddedInDeleteOperation] = false;
                    Debug.Assert(CurrencyManager!.List!.Count == 0, "we deleted the row that the Child table forcefully added");
                }
                else if (!_dataConnectionState[DATACONNECTIONSTATE_finishedAddNew]
                    && _dataConnectionState[DATACONNECTIONSTATE_inEndCurrentEdit])
                {
                    // EndCurrentEdit caused an item to be deleted while in AddNew.
                    // Recreate the rows.
                    _dataConnectionState[DATACONNECTIONSTATE_listWasReset] = true;
                    try
                    {
                        _owner.RefreshRows(scrollIntoView: !_owner.InSortOperation);
                        _owner.PushAllowUserToAddRows();
                    }
                    finally
                    {
                        _dataConnectionState[DATACONNECTIONSTATE_listWasReset] = false;
                    }

                    return;
                }
                else
                {
                    Debug.Assert(CurrencyManager?.List is not null);
                    if (CurrencyManager.List.Count == DataBoundRowsCount())
                    {
                        return;
                    }
                }
            }

            // when we get the ListChanged notification the position in the currency manager already changed
            // so do not change the position when we get the RowEnter event
            _dataConnectionState[DATACONNECTIONSTATE_doNotChangePositionInTheCurrencyManager] = true;

            try
            {
                switch (e.ListChangedType)
                {
                    case ListChangedType.Reset:
                        _dataConnectionState[DATACONNECTIONSTATE_listWasReset] = true;
                        bool startUpdateInternal = _owner.Visible;
                        if (startUpdateInternal)
                        {
                            _owner.BeginUpdateInternal();
                        }

                        try
                        {
                            _owner.RefreshRows(scrollIntoView: !_owner.InSortOperation);
                            _owner.PushAllowUserToAddRows();

                            // ListChangedType.Reset can signal that the list became sorted or that the list is not sorted anymore.
                            ApplySortingInformationFromBackEnd();
                        }
                        finally
                        {
                            // this will also set DATACONNECTIONSTATE_listWasReset to false
                            ResetDataConnectionState();
                            if (startUpdateInternal)
                            {
                                _owner.EndUpdateInternal(false);
                                _owner.Invalidate(true);
                            }
                        }

                        break;
                    case ListChangedType.ItemAdded:
                        if (_owner.NewRowIndex == -1 || e.NewIndex != _owner.Rows.Count)
                        {
                            _owner.Rows.InsertInternal(e.NewIndex, _owner.RowTemplateClone, force: true);
                        }
                        else
                        {
#if DEBUG
                            Debug.Fail("fail in debug builds so we can catch this situation in the check in suites");
#endif // DEBUG
                            throw new InvalidOperationException();
                        }

                        break;
                    case ListChangedType.ItemDeleted:
                        _owner.Rows.RemoveAtInternal(e.NewIndex, force: true);
                        _dataConnectionState[DATACONNECTIONSTATE_didNotDeleteRowFromDataGridView] = false;
                        break;
                    case ListChangedType.ItemMoved:
                        // an ItemMoved event means that all the rows shifted up or down by 1
                        // we have to invalidate all the rows in between
                        Debug.Assert(e.OldIndex > -1, "the currency manager should have taken care of this case");
                        Debug.Assert(e.NewIndex > -1, "how can we move an item outside of the list?");
                        int lo = Math.Min(e.OldIndex, e.NewIndex);
                        int hi = Math.Max(e.OldIndex, e.NewIndex);
                        _owner.InvalidateRows(lo, hi);
                        break;
                    case ListChangedType.ItemChanged:
                        Debug.Assert(e.NewIndex != -1, "the item changed event does not cover changes to the entire list");
                        string? dataPropertyName = null;
                        if (e.PropertyDescriptor is not null)
                        {
                            dataPropertyName = e.PropertyDescriptor.Name;
                        }

                        for (int columnIndex = 0; columnIndex < _owner.Columns.Count; columnIndex++)
                        {
                            DataGridViewColumn dataGridViewColumn = _owner.Columns[columnIndex];
                            if (dataGridViewColumn.Visible && dataGridViewColumn.IsDataBound)
                            {
                                if (!string.IsNullOrEmpty(dataPropertyName))
                                {
                                    if (string.Compare(dataGridViewColumn.DataPropertyName, dataPropertyName, ignoreCase: true, CultureInfo.InvariantCulture) == 0)
                                    {
                                        _owner.OnCellCommonChange(columnIndex, e.NewIndex);
                                    }
                                }
                                else
                                {
                                    _owner.OnCellCommonChange(columnIndex, e.NewIndex);
                                }
                            }
                        }

                        // Repaint the row header cell to show potential error icon
                        _owner.InvalidateCell(-1, e.NewIndex);
                        // update the editing control value if the data changed in the row the user was editing
                        if (_owner.CurrentCellAddress.Y == e.NewIndex && _owner.IsCurrentCellInEditMode)
                        {
                            _owner.RefreshEdit();
                        }

                        break;
                    default:
                        break;
                }

                // now put the position in the DataGridView control according to the position in the currency manager
                if (_owner.Rows.Count > 0
                    && !_dataConnectionState[DATACONNECTIONSTATE_doNotChangePositionInTheDataGridViewControl]
                    && !_owner.InSortOperation)
                {
                    MatchCurrencyManagerPosition(scrollIntoView: false, clearSelection: e.ListChangedType == ListChangedType.Reset);
                }
            }
            finally
            {
                _dataConnectionState[DATACONNECTIONSTATE_doNotChangePositionInTheCurrencyManager] = false;
            }
        }

        private void currencyManager_PositionChanged(object? sender, EventArgs e)
        {
            Debug.Assert(sender == CurrencyManager, "did we forget to unregister our events?");
            if (_owner.Columns.Count == 0)
            {
                Debug.Assert(_owner.CurrentCellAddress.X == -1);
                // No columns means we can't set the current cell.
                // This happens when all columns where removed from the dataGridView, and all rows were cleared.
                // Discuss this with Daniel/Mark.
                // One solution: impose at least one visible column - all the time.
                return;
            }

            if (_owner.Rows.Count == (_owner.AllowUserToAddRowsInternal ? 1 : 0))
            {
                // the dataGridView control has not yet been notified that the list is not empty
                // don't do anything
                return;
            }

            if (_dataConnectionState[DATACONNECTIONSTATE_doNotChangePositionInTheDataGridViewControl])
            {
                return;
            }

            // when the back end is still inside an AddNew we get a PositionChanged event before
            // we get the list changed event. So, we get the position changed event before we have a chance to refresh our
            // row collection.
            // It may be the case that the new position in the currency manager corresponds to the DataGridView::AddNew row position.
            // And then DataGridView will enter its AddNew row and as a result of that will start another AddNew transaction - inside
            // the current AddNew transaction.
            // The solution is to not change the current cell if:
            // 1. DataGridView::AllowUserToAddRowsInternal == true, and
            // 2. DataGridView is not inside DataGridView::AddNew transaction, and
            // 3. the new position inside the currency manager is not -1.
            // 4. the new position corresponds to the DataGridView::NewRow position, and
            // 5. the position inside the DataGridView is not on the new row index.
            // 6. the count on the back end list is 1 more than the number of data bound data grid view rows.
            // The DataGridView will change its current cell once the currency manager fires ListChanged event.
            if (_owner.AllowUserToAddRowsInternal &&                                // condition 1.
                _dataConnectionState[DATACONNECTIONSTATE_finishedAddNew] &&         // condition 2.
                !_dataConnectionState[DATACONNECTIONSTATE_inAddNew] &&              // condition 2.
                CurrencyManager!.Position > -1 &&                                   // condition 3.
                CurrencyManager.Position == _owner.NewRowIndex &&              // condition 4.
                _owner.CurrentCellAddress.Y != _owner.NewRowIndex &&            // condition 5.
                CurrencyManager.Count == DataBoundRowsCount() + 1)                 // condition 6.
            {
                return;
            }

            _dataConnectionState[DATACONNECTIONSTATE_positionChangingInCurrencyManager] = true;
            try
            {
                if (!_owner.InSortOperation)
                {
                    bool scrollIntoView = true;
                    // When an item is repositioned in a sorted column, while its
                    // row is being committed, don't scroll it into view.
                    if (_dataConnectionState[DATACONNECTIONSTATE_rowValidatingInAddNew])
                    {
                        Debug.Assert(CurrencyManager?.List is not null);
                        if (CurrencyManager.List is IBindingList ibl && ibl.SupportsSorting && ibl.IsSorted)
                        {
                            scrollIntoView = false;
                        }
                    }

                    // If the user hit Escape while in AddNew then we clear the selection.
                    bool clearSelection = _dataConnectionState[DATACONNECTIONSTATE_cancellingRowEdit] && !_dataConnectionState[DATACONNECTIONSTATE_finishedAddNew];
                    // Otherwise we clear the selection if the last list count is still uninitialized
                    // or if it is the same as the current list count.
                    clearSelection |= _lastListCount == -1 || _lastListCount == CurrencyManager!.Count;
                    MatchCurrencyManagerPosition(scrollIntoView, clearSelection);
                }
            }
            finally
            {
                _dataConnectionState[DATACONNECTIONSTATE_positionChangingInCurrencyManager] = false;
            }
        }

        //
        // This function will return the number of rows inside the DataGridView which are data bound.
        // For instance, the AddNewRow inside the DataGridView is not data bound so it should not be counted.
        //
        private int DataBoundRowsCount()
        {
            int result = _owner.Rows.Count;
            if (_owner.AllowUserToAddRowsInternal && _owner.Rows.Count > 0)
            {
                Debug.Assert(_owner.NewRowIndex != -1, "the NewRowIndex is -1 only when AllowUserToAddRows is false");

                // We have to check if the AddNew row is data bound or not.
                // The AddNew row is data bound if the user is positioned in the AddNew row and the AddNew row is not dirty
                if (_owner.CurrentCellAddress.Y != _owner.NewRowIndex || _owner.IsCurrentRowDirty)
                {
                    // The AddNew row in the DataGridView row collection is not data bound.
                    // Substract it from the row count;
                    result--;
                }
            }

            return result;
        }

        private void DataSource_Initialized(object? sender, EventArgs e)
        {
            Debug.Assert(sender == DataSource);
            Debug.Assert(DataSource is ISupportInitializeNotification);
            Debug.Assert(_dataConnectionState[DATACONNECTIONSTATE_dataSourceInitializedHookedUp]);

            // Unhook the Initialized event.
            if (DataSource is ISupportInitializeNotification dsInit)
            {
                dsInit.Initialized -= DataSource_Initialized;
            }

            // The wait is over: DataSource is initialized.
            _dataConnectionState[DATACONNECTIONSTATE_dataSourceInitializedHookedUp] = false;

            // Update the data manager
            SetDataConnection(DataSource, DataMember);
            Debug.Assert(CurrencyManager is not null);
            _owner.RefreshColumnsAndRows();
            _owner.OnDataBindingComplete(ListChangedType.Reset);
        }

        private void DataSourceMetaDataChanged()
        {
            Debug.Assert(CurrencyManager is not null);

            // get the new meta data
            _props = CurrencyManager.GetItemProperties();

            // when AutoGenerate == true: RefreshColumnsAndRows will delete the previously dataBound columns and create new dataBounds columns
            //
            // AutoGenerate == false : RefreshColumnsAndRows will refresh the property descriptors for the dataBound Columns.
            // Some unBound columns may become dataBound, some dataBounds columns may become unBound
            //

            _owner.RefreshColumnsAndRows();
        }

        public void DeleteRow(int rowIndex)
        {
            _dataConnectionState[DATACONNECTIONSTATE_doNotChangePositionInTheDataGridViewControl] = true;
            try
            {
                if (!_dataConnectionState[DATACONNECTIONSTATE_finishedAddNew])
                {
                    Debug.Assert(_owner.AllowUserToAddRowsInternal, "how did we start an add new row transaction if the dataGridView control has AllowUserToAddRows == false?");
                    bool deleteAddNewRow = false;
                    Debug.Assert(CurrencyManager?.List is not null);
                    if (_owner.NewRowIndex == CurrencyManager.List.Count)
                    {
                        // the user clicked on the 'add new row' and started typing
                        deleteAddNewRow = (rowIndex == _owner.NewRowIndex - 1);
                    }
                    else
                    {
                        // the user clicked on the 'add new row' but did not start typing
                        Debug.Assert(_owner.NewRowIndex == CurrencyManager.List.Count - 1);
                        deleteAddNewRow = (rowIndex == _owner.NewRowIndex);
                    }

                    if (deleteAddNewRow)
                    {
                        // we finished the add new transaction
                        CancelRowEdit(restoreRow: false, addNewFinished: true);
                    }
                    else
                    {
                        // start the Delete operation
                        _dataConnectionState[DATACONNECTIONSTATE_inDeleteOperation] = true;
                        // we did not delete any rows from the data grid view yet
                        _dataConnectionState[DATACONNECTIONSTATE_didNotDeleteRowFromDataGridView] = true;
                        try
                        {
                            CurrencyManager.RemoveAt(rowIndex);
                        }
                        finally
                        {
                            _dataConnectionState[DATACONNECTIONSTATE_inDeleteOperation] = false;
                            _dataConnectionState[DATACONNECTIONSTATE_didNotDeleteRowFromDataGridView] = false;
                        }
                    }
                }
                else
                {
                    // start the Delete operation
                    _dataConnectionState[DATACONNECTIONSTATE_inDeleteOperation] = true;
                    // we did not delete any rows from the data grid view yet
                    _dataConnectionState[DATACONNECTIONSTATE_didNotDeleteRowFromDataGridView] = true;
                    try
                    {
                        CurrencyManager!.RemoveAt(rowIndex);
                    }
                    finally
                    {
                        _dataConnectionState[DATACONNECTIONSTATE_inDeleteOperation] = false;
                        _dataConnectionState[DATACONNECTIONSTATE_didNotDeleteRowFromDataGridView] = false;
                    }
                }
            }
            finally
            {
                _dataConnectionState[DATACONNECTIONSTATE_doNotChangePositionInTheDataGridViewControl] = false;
            }
        }

        public bool DataFieldIsReadOnly(int boundColumnIndex)
        {
            if (_props is null)
            {
                Debug.Fail("we only care about which data fields are read only when we are data bound");
                return false;
            }

            return _props[boundColumnIndex].IsReadOnly;
        }

        // All we do in dispose is to unwire the data source.
        public void Dispose()
        {
            UnWireEvents();

            // Set the currency manager to null so if someone would want to resurrect this data grid view data connection
            // we would not unwire the events from the currency manager twice.
            // (NOTE: resurrecting a disposed data grid view data connection is not allowed.)
            //
            CurrencyManager = null;
        }

        private static DataGridViewColumn GetDataGridViewColumnFromType(Type type)
        {
            DataGridViewColumn dataGridViewColumn;
            TypeConverter imageTypeConverter = TypeDescriptor.GetConverter(typeof(Image));
            if (type.Equals(typeof(bool)) || type.Equals(typeof(CheckState)))
            {
                dataGridViewColumn = new DataGridViewCheckBoxColumn(type.Equals(typeof(CheckState)));
            }
            else if (typeof(Image).IsAssignableFrom(type) || imageTypeConverter.CanConvertFrom(type))
            {
                dataGridViewColumn = new DataGridViewImageColumn();
            }
            else
            {
                dataGridViewColumn = new DataGridViewTextBoxColumn();
            }

            return dataGridViewColumn;
        }

        public List<DataGridViewColumn>? GetCollectionOfBoundDataGridViewColumns()
        {
            if (_props is null)
            {
                return null;
            }

            List<DataGridViewColumn> cols = new(_props.Count);

            for (int i = 0; i < _props.Count; i++)
            {
                if (typeof(IList).IsAssignableFrom(_props[i].PropertyType))
                {
                    // We have an IList. It could be a byte[] in which case we want to generate an Image column.
                    TypeConverter imageTypeConverter = TypeDescriptor.GetConverter(typeof(Image));
                    if (!imageTypeConverter.CanConvertFrom(_props[i].PropertyType))
                    {
                        continue;
                    }
                }

                DataGridViewColumn dataGridViewColumn = GetDataGridViewColumnFromType(_props[i].PropertyType);
                dataGridViewColumn.IsDataBoundInternal = true;
                dataGridViewColumn.BoundColumnIndex = i;
                // we set the data property name
                // if you plan on removing this, then you have to change the lookup into
                // the GetCollectionOfBoundDataGridViewColumns
                dataGridViewColumn.DataPropertyName = _props[i].Name;
                dataGridViewColumn.Name = _props[i].Name;
                dataGridViewColumn.BoundColumnConverter = _props[i].Converter;
                dataGridViewColumn.HeaderText = !string.IsNullOrEmpty(_props[i].DisplayName) ? _props[i].DisplayName : _props[i].Name;
                dataGridViewColumn.ValueType = _props[i].PropertyType;

                dataGridViewColumn.IsBrowsableInternal = _props[i].IsBrowsable;

                dataGridViewColumn.ReadOnly = _props[i].IsReadOnly;

                cols.Add(dataGridViewColumn);
            }

            return cols;
        }

        private void GetSortingInformationFromBackend(out PropertyDescriptor? sortProperty, out SortOrder sortOrder)
        {
            if (CurrencyManager?.List is not IBindingList { SupportsSorting: true, IsSorted: true } ibl)
            {
                sortOrder = SortOrder.None;
                sortProperty = null;
                return;
            }

            if (ibl.SortProperty is not null)
            {
                sortProperty = ibl.SortProperty;
                sortOrder = ibl.SortDirection == ListSortDirection.Ascending ? SortOrder.Ascending : SortOrder.Descending;
            }
            else if (ibl is IBindingListView iblv)
            {
                // Maybe the data view is sorted on multiple columns.
                // Go thru the IBindingListView which offers the entire list of sorted columns
                // and pick the first one as the SortedColumn.
                ListSortDescriptionCollection sorts = iblv.SortDescriptions;
                if (sorts is not null
                    && sorts.Count > 0
                    && sorts[0]!.PropertyDescriptor is not null)
                {
                    sortProperty = sorts[0]!.PropertyDescriptor;
                    sortOrder = sorts[0]!.SortDirection == ListSortDirection.Ascending ? SortOrder.Ascending : SortOrder.Descending;
                }
                else
                {
                    // The IBindingListView did not have any sorting information.
                    sortProperty = null;
                    sortOrder = SortOrder.None;
                }
            }
            else
            {
                // We could not get the sort order either from IBindingList nor from IBindingListView.
                sortProperty = null;
                sortOrder = SortOrder.None;
            }
        }

        public void ResetCachedAllowUserToAddRowsInternal() =>
            _dataConnectionState[DATACONNECTIONSTATE_cachedAllowUserToAddRowsInternal] = _owner.AllowUserToAddRowsInternal;

        private void ResetDataConnectionState()
        {
            // Microsoft: I wish there would be a Reset method on BitVector32...
            _dataConnectionState = new BitVector32(DATACONNECTIONSTATE_finishedAddNew);

            if (CurrencyManager is not null)
            {
                _dataConnectionState[DATACONNECTIONSTATE_interestedInRowEvents] = true;
            }

            ResetCachedAllowUserToAddRowsInternal();
        }

        public void SetDataConnection(object? dataSource, string dataMember)
        {
            if (_dataConnectionState[DATACONNECTIONSTATE_dataConnection_inSetDataConnection])
            {
                return;
            }

            ResetDataConnectionState();

            dataMember ??= string.Empty;

            if (DataSource is ISupportInitializeNotification dsInit && _dataConnectionState[DATACONNECTIONSTATE_dataSourceInitializedHookedUp])
            {
                // If we previously hooked the data source's ISupportInitializeNotification
                // Initialized event, then unhook it now (we don't always hook this event,
                // only if we needed to because the data source was previously uninitialized)
                dsInit.Initialized -= DataSource_Initialized;
                _dataConnectionState[DATACONNECTIONSTATE_dataSourceInitializedHookedUp] = false;
            }

            DataSource = dataSource;
            DataMember = dataMember;

            if (_owner.BindingContext is null)
            {
                return;
            }

            _dataConnectionState[DATACONNECTIONSTATE_dataConnection_inSetDataConnection] = true;
            try
            {
                // unwire the events
                UnWireEvents();

                if (DataSource is not null && _owner.BindingContext is not null && DataSource != Convert.DBNull)
                {
                    dsInit = (DataSource as ISupportInitializeNotification)!;
                    if (dsInit is not null && !dsInit.IsInitialized)
                    {
                        if (!_dataConnectionState[DATACONNECTIONSTATE_dataSourceInitializedHookedUp])
                        {
                            dsInit.Initialized += DataSource_Initialized;
                            _dataConnectionState[DATACONNECTIONSTATE_dataSourceInitializedHookedUp] = true;
                        }

                        CurrencyManager = null;
                    }
                    else
                    {
                        CurrencyManager = _owner.BindingContext[DataSource, DataMember] as CurrencyManager;
                    }
                }
                else
                {
                    CurrencyManager = null;
                }

                // wire the events
                WireEvents();
                if (CurrencyManager is not null)
                {
                    _props = CurrencyManager.GetItemProperties();
                }
                else
                {
                    _props = null;
                }
            }
            finally
            {
                _dataConnectionState[DATACONNECTIONSTATE_dataConnection_inSetDataConnection] = false;
            }

            ResetCachedAllowUserToAddRowsInternal();

            if (CurrencyManager is not null)
            {
                _lastListCount = CurrencyManager.Count;
            }
            else
            {
                _lastListCount = -1;
            }
        }

        public string GetError(int rowIndex)
        {
            IDataErrorInfo? errInfo = null;
            try
            {
                errInfo = CurrencyManager![rowIndex] as IDataErrorInfo;
            }
            catch (Exception exception) when (!exception.IsCriticalException() || exception is IndexOutOfRangeException)
            {
                DataGridViewDataErrorEventArgs dgvdee = new(
                    exception,
                    columnIndex: -1,
                    rowIndex,
                    DataGridViewDataErrorContexts.Display);
                _owner.OnDataErrorInternal(dgvdee);
                if (dgvdee.ThrowException)
                {
                    throw dgvdee.Exception;
                }
            }

            return errInfo is not null ? errInfo.Error : string.Empty;
        }

        public string GetError(int boundColumnIndex, int columnIndex, int rowIndex)
        {
            Debug.Assert(rowIndex >= 0);

            IDataErrorInfo? errInfo = null;
            try
            {
                errInfo = CurrencyManager![rowIndex] as IDataErrorInfo;
            }
            catch (Exception exception) when (!exception.IsCriticalException() || exception is IndexOutOfRangeException)
            {
                DataGridViewDataErrorEventArgs dgvdee = new(
                    exception,
                    columnIndex,
                    rowIndex,
                    DataGridViewDataErrorContexts.Display);
                _owner.OnDataErrorInternal(dgvdee);
                if (dgvdee.ThrowException)
                {
                    throw dgvdee.Exception;
                }
            }

            if (errInfo is not null)
            {
                return errInfo[_props![boundColumnIndex].Name];
            }
            else
            {
                return string.Empty;
            }
        }

        public object? GetValue(int boundColumnIndex, int columnIndex, int rowIndex)
        {
            Debug.Assert(rowIndex >= 0);
            object? value = null;
            try
            {
                value = _props![boundColumnIndex].GetValue(CurrencyManager![rowIndex]);
            }
            catch (Exception exception) when (!exception.IsCriticalException() || exception is IndexOutOfRangeException)
            {
                DataGridViewDataErrorEventArgs dgvdee = new(
                    exception,
                    columnIndex,
                    rowIndex,
                    DataGridViewDataErrorContexts.Display);
                _owner.OnDataErrorInternal(dgvdee);
                if (dgvdee.ThrowException)
                {
                    throw dgvdee.Exception;
                }
            }

            return value;
        }

        public void MatchCurrencyManagerPosition(bool scrollIntoView, bool clearSelection)
        {
            if (_owner.Columns.Count == 0)
            {
#if DEBUG
                // all the properties in the currency manager should be either Browsable(false) or point to sub lists
                if (_props is not null)
                {
                    for (int i = 0; i < _props.Count; i++)
                    {
                        Debug.Assert(!_props[i].IsBrowsable || typeof(IList).IsAssignableFrom(_props[i].PropertyType), "if the DGV does not have any columns then the properties in the currency manager should be Browsable(false) or point to sub lists");
                    }
                }
#endif // DEBUG

                // nothing to do
                return;
            }

            int columnIndex = _owner.CurrentCellAddress.X == -1 ? _owner.FirstDisplayedColumnIndex : _owner.CurrentCellAddress.X;

            // Treat case where columnIndex == -1. We change the visibility of the first column.
            if (columnIndex == -1)
            {
                DataGridViewColumn dataGridViewColumn = _owner.Columns.GetFirstColumn(DataGridViewElementStates.None)!;
                Debug.Assert(dataGridViewColumn is not null);
                dataGridViewColumn.Visible = true;
                columnIndex = dataGridViewColumn.Index;
            }

            int rowIndex = CurrencyManager!.Position;

            Debug.Assert(rowIndex >= -1);

            if (rowIndex == -1)
            {
                // Occurs when calling SuspendBinding() on the currency manager?
                if (!_owner.SetCurrentCellAddressCore(
                    columnIndex: -1,
                    rowIndex: -1,
                    setAnchorCellAddress: false,
                    validateCurrentCell: false,
                    throughMouseClick: false))
                {
                    throw new InvalidOperationException(SR.DataGridView_CellChangeCannotBeCommittedOrAborted);
                }
            }
            else if (rowIndex < _owner.Rows.Count)
            {
                // the currency manager sends the PositionChanged event before the ListChanged event.
                // This means that it is possible for the data grid view to receive the position changed event
                // before it had a chance to created its rows.
                // So, if the position inside the currency manager is greater than the number of rows in the data grid view
                // don't do anything.
                // NOTE: because the currency manager will fire the list changed event after the position changed event
                // the data grid view will actually get a second chance at matching the position inside the currency manager.

                // Do not allow to set the current cell to an invisible cell
                if ((_owner.Rows.GetRowState(rowIndex) & DataGridViewElementStates.Visible) == 0)
                {
                    // Make the target row visible.
                    _owner.Rows[rowIndex].Visible = true;
                }

                if (rowIndex == _owner.CurrentCellAddress.Y && columnIndex == _owner.CurrentCellAddress.X)
                {
                    return;
                }

                // Scroll target cell into view first.
                if ((scrollIntoView && !_owner.ScrollIntoView(columnIndex, rowIndex, forCurrentCellChange: true))
                    || (columnIndex < _owner.Columns.Count
                        && rowIndex < _owner.Rows.Count
                        && !_owner.SetAndSelectCurrentCellAddress(
                            columnIndex,
                            rowIndex,
                            setAnchorCellAddress: true,
                            validateCurrentCell: false,
                            throughMouseClick: false,
                            clearSelection,
                            forceCurrentCellSelection: false)))
                {
                    throw new InvalidOperationException(SR.DataGridView_CellChangeCannotBeCommittedOrAborted);
                }
            }
        }

        public void CancelRowEdit(bool restoreRow, bool addNewFinished)
        {
            _dataConnectionState[DATACONNECTIONSTATE_cancellingRowEdit] = true;
            _dataConnectionState[DATACONNECTIONSTATE_restoreRow] = restoreRow;
            try
            {
                object? currentItem = null;
                if (CurrencyManager!.Position >= 0 && CurrencyManager.Position < CurrencyManager.List!.Count)
                {
                    currentItem = CurrencyManager.Current;
                }

                CurrencyManager.CancelCurrentEdit();

                // CurrencyManager no longer starts a new transaction automatically
                // when we call CurrencyManager::CancelCurrentEdit.
                // So, if the current item inside the currency manager did not change, we have to start a new transaction.
                // (If the current item inside the currency manager changed, then the currency manager would have
                // already started a new transaction).
                IEditableObject? editableObject = null;
                if (CurrencyManager.Position >= 0 && CurrencyManager.Position < CurrencyManager.List!.Count)
                {
                    editableObject = CurrencyManager.Current as IEditableObject;
                }

                if (editableObject is not null && currentItem == editableObject)
                {
                    editableObject.BeginEdit();
                }
            }
            finally
            {
                _dataConnectionState[DATACONNECTIONSTATE_cancellingRowEdit] = false;
            }

            if (addNewFinished)
            {
                _dataConnectionState[DATACONNECTIONSTATE_finishedAddNew] = true;
            }
        }

        internal void OnNewRowNeeded()
        {
            _dataConnectionState[DATACONNECTIONSTATE_doNotChangePositionInTheDataGridViewControl] = true;
            try
            {
                AddNew();
            }
            finally
            {
                _dataConnectionState[DATACONNECTIONSTATE_doNotChangePositionInTheDataGridViewControl] = false;
            }
        }

        internal void OnRowEnter(DataGridViewCellEventArgs e)
        {
            // don't change position or start a transaction in the middle of a meta data change
            if (_dataConnectionState[DATACONNECTIONSTATE_processingMetaDataChanges])
            {
                return;
            }

            // don't start a transaction on a suspended currency manager.
            if (!CurrencyManager!.ShouldBind)
            {
                return;
            }

            _dataConnectionState[DATACONNECTIONSTATE_doNotChangePositionInTheDataGridViewControl] = true;
            try
            {
                // Don't automatically force an EndCurrentEdit on the currency manager
                if (e.RowIndex != _owner.NewRowIndex
                    && !_dataConnectionState[DATACONNECTIONSTATE_doNotChangePositionInTheCurrencyManager]
                    && CurrencyManager.Position != e.RowIndex)
                {
                    try
                    {
                        CurrencyManager.Position = e.RowIndex;
                    }
                    catch (Exception exception) when (!exception.IsCriticalException())
                    {
                        DataGridViewCellCancelEventArgs dgvce = new(e.ColumnIndex, e.RowIndex);
                        ProcessException(exception, dgvce, beginEdit: false);
                    }

                    if (CurrencyManager.Current is IEditableObject iEditObj)
                    {
                        iEditObj.BeginEdit();
                    }
                }
            }
            finally
            {
                _dataConnectionState[DATACONNECTIONSTATE_doNotChangePositionInTheDataGridViewControl] = false;
            }
        }

        internal void OnRowValidating(DataGridViewCellCancelEventArgs e)
        {
            // don't end the transaction on a suspended currency manager.
            if (!CurrencyManager!.ShouldBind)
            {
                return;
            }

            if (!_dataConnectionState[DATACONNECTIONSTATE_finishedAddNew] && !_owner.IsCurrentRowDirty)
            {
                if (!_dataConnectionState[DATACONNECTIONSTATE_cancellingRowEdit])
                {
                    Debug.Assert(DataBoundRowsCount() == CurrencyManager.List!.Count, "if the back end was changed while in AddNew the DGV should have updated its rows collection");
                    // Cancel the current AddNew transaction
                    // doNotChangePositionInTheDataGridViewControl because we will change position
                    // when we get notification from the back end that the cancel operation was completed
                    _dataConnectionState[DATACONNECTIONSTATE_doNotChangePositionInTheDataGridViewControl] = true;
                    try
                    {
                        CancelRowEdit(restoreRow: false, addNewFinished: false);
                    }
                    finally
                    {
                        _dataConnectionState[DATACONNECTIONSTATE_doNotChangePositionInTheDataGridViewControl] = false;
                    }
                }
            }
            else if (_owner.IsCurrentRowDirty)
            {
                _dataConnectionState[DATACONNECTIONSTATE_rowValidatingInAddNew] = true;

                try
                {
                    CurrencyManager.EndCurrentEdit();
                }
                catch (Exception exception) when (!exception.IsCriticalException())
                {
                    ProcessException(exception, e, beginEdit: true);
                }
                finally
                {
                    _dataConnectionState[DATACONNECTIONSTATE_rowValidatingInAddNew] = false;
                }
            }

            // we moved away from the 'add new row', so the 'add new row' has been committed in the back-end
            // or has been rejected from the back-end. In any case, the AddNew operation completed.
            _dataConnectionState[DATACONNECTIONSTATE_finishedAddNew] = true;
        }

        public void ProcessException(Exception exception, DataGridViewCellCancelEventArgs e, bool beginEdit)
        {
            DataGridViewDataErrorEventArgs dgvdee = new(
                exception,
                e.ColumnIndex,
                e.RowIndex,
                DataGridViewDataErrorContexts.Commit);
            _owner.OnDataErrorInternal(dgvdee);

            if (dgvdee.ThrowException)
            {
                throw dgvdee.Exception;
            }
            else if (dgvdee.Cancel)
            {
                e.Cancel = true;
                if (beginEdit)
                {
                    if (CurrencyManager!.Current is IEditableObject iEditObj)
                    {
                        iEditObj.BeginEdit();
                    }
                }
            }
            else
            {
                CancelRowEdit(restoreRow: false, addNewFinished: false);
                // interrupt current operation
            }
        }

        public bool PushValue(int boundColumnIndex, int columnIndex, int rowIndex, object? value)
        {
            try
            {
                if (value is not null)
                {
                    Type valueType = value.GetType();
                    Type columnType = _owner.Columns[columnIndex].ValueType!;
                    if (!columnType.IsAssignableFrom(valueType))
                    {
                        // value needs to be converted before being fed to the back-end.
                        TypeConverter boundColumnConverter = BoundColumnConverter(boundColumnIndex);
                        if (boundColumnConverter is not null && boundColumnConverter.CanConvertFrom(valueType))
                        {
                            value = boundColumnConverter.ConvertFrom(value);
                        }
                        else
                        {
                            TypeConverter valueConverter = _owner.GetCachedTypeConverter(valueType);
                            if (valueConverter is not null && valueConverter.CanConvertTo(columnType))
                            {
                                value = valueConverter.ConvertTo(value, columnType);
                            }
                        }
                    }
                }

                _props![boundColumnIndex].SetValue(CurrencyManager![rowIndex], value);
            }
            catch (Exception exception) when (!exception.IsCriticalException())
            {
                DataGridViewCellCancelEventArgs e = new(columnIndex, rowIndex);
                ProcessException(exception, e, false);
                return !e.Cancel;
            }

            return true;
        }

        public bool ShouldChangeDataMember(object? newDataSource)
        {
            if (!_owner.Created)
            {
                // if the owner is not created yet then data member can be valid
                return false;
            }

            if (_owner.BindingContext is null)
            {
                // if we don't have the BindingContext then the data member can still be valid
                return false;
            }

            if (newDataSource is null)
            {
                // we have the binding context and the new data source is null
                // we should change the data member to ""
                return true;
            }

            if (_owner.BindingContext[newDataSource] is not CurrencyManager cm)
            {
                // if we don't have a currency manager then the data member can be valid
                return false;
            }

            PropertyDescriptorCollection props = cm.GetItemProperties();
            if (DataMember.Length != 0 && props[DataMember] is not null)
            {
                // the data member is valid. Don't change it
                return false;
            }

            return true;
        }

        public void Sort(DataGridViewColumn dataGridViewColumn, ListSortDirection direction)
        {
            Debug.Assert(dataGridViewColumn.IsDataBound && dataGridViewColumn.BoundColumnIndex != -1, "we need a bound column index to perform the sort");
            Debug.Assert(List is IBindingList, "you should have checked by now that we are bound to an IBindingList");
            ((IBindingList)List).ApplySort(_props![dataGridViewColumn.BoundColumnIndex], direction);
        }

        private void UnWireEvents()
        {
            if (CurrencyManager is not null)
            {
                CurrencyManager.PositionChanged -= currencyManager_PositionChanged;
                CurrencyManager.ListChanged -= currencyManager_ListChanged;
                _dataConnectionState[DATACONNECTIONSTATE_interestedInRowEvents] = false;
            }
        }

        private void WireEvents()
        {
            if (CurrencyManager is not null)
            {
                CurrencyManager.PositionChanged += currencyManager_PositionChanged;
                CurrencyManager.ListChanged += currencyManager_ListChanged;
                _dataConnectionState[DATACONNECTIONSTATE_interestedInRowEvents] = true;
            }
        }
    }
}
