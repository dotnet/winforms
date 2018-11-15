// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Drawing;

namespace System.Windows.Forms
{
    public partial class DataGridView
    {
        internal class DataGridViewDataConnection
        {
            DataGridView owner = null;
            CurrencyManager currencyManager = null;
            object dataSource = null;
            string dataMember = String.Empty;
            PropertyDescriptorCollection props = null;
            int lastListCount = -1;

            //
            // data connection state variables 
            //
            private BitVector32 dataConnectionState;
            private const int DATACONNECTIONSTATE_dataConnection_inSetDataConnection = 0x00000001;
            private const int DATACONNECTIONSTATE_processingMetaDataChanges = 0x00000002;

            // AddNew
            private const int DATACONNECTIONSTATE_finishedAddNew = 0x00000004;

            private const int DATACONNECTIONSTATE_doNotChangePositionInTheDataGridViewControl = 0x00000008;
            // DataGridView::SetCurrentCellAddressCore makes the current row unavailable during the OnRowEnter event.
            // we use the doNotChangePositionInTheCurrencyManager flag to go around this.
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
            // 4. The DGV bound to the Master table receives the ItemAdded event. At this point, no rows have been deleted from the DGV.
            // 5. The DGV bound to the Master table should not add a new DataGridViewRow to its Rows collection because it will be deleted later on.
            //    So the DGV marks _itemAddedInDeleteOperation to TRUE to know that the next event it expects is an ItemDeleted
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
                this.owner = owner;
                this.dataConnectionState = new BitVector32(DATACONNECTIONSTATE_finishedAddNew);
            }

            public bool AllowAdd
            {
                get
                {
                    if (this.currencyManager != null)
                    {
                        // we only allow to add new rows on an IBindingList
                        return (this.currencyManager.List is IBindingList) && this.currencyManager.AllowAdd && ((IBindingList)this.currencyManager.List).SupportsChangeNotification;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            public bool AllowEdit
            {
                get
                {
                    if (this.currencyManager != null)
                    {
                        return this.currencyManager.AllowEdit;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            public bool AllowRemove
            {
                get
                {
                    if (this.currencyManager != null)
                    {
                        // we only allow deletion on an IBindingList
                        return (this.currencyManager.List is IBindingList) && this.currencyManager.AllowRemove && ((IBindingList)this.currencyManager.List).SupportsChangeNotification;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            public bool CancellingRowEdit
            {
                get
                {
                    return this.dataConnectionState[DATACONNECTIONSTATE_cancellingRowEdit];
                }
            }

            public CurrencyManager CurrencyManager
            {
                get
                {
                    return this.currencyManager;
                }
            }

            public string DataMember
            {
                get
                {
                    return this.dataMember;
                }
            }

            public object DataSource
            {
                get
                {
                    return this.dataSource;
                }
            }

            public bool DoNotChangePositionInTheCurrencyManager
            {
                get
                {
                    return this.dataConnectionState[DATACONNECTIONSTATE_doNotChangePositionInTheCurrencyManager];
                }
                set
                {
                    this.dataConnectionState[DATACONNECTIONSTATE_doNotChangePositionInTheCurrencyManager] = value;
                }
            }

            public bool InterestedInRowEvents
            {
                get
                {
                    return this.dataConnectionState[DATACONNECTIONSTATE_interestedInRowEvents];
                }
            }

            public IList List
            {
                get
                {
                    if (this.currencyManager != null)
                    {
                        return this.currencyManager.List;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            public bool ListWasReset
            {
                get
                {
                    return this.dataConnectionState[DATACONNECTIONSTATE_listWasReset];
                }
            }

            public bool PositionChangingOutsideDataGridView
            {
                get
                {
                    // DATACONNECTIONSTATE_doNotChangePositionInTheDataGridViewControl means that the data grid view control
                    // manages the position change
                    // so if DATACONNECTIONSTATE_doNotChangePositionInTheDataGridViewControl is true then the data grid view knows about the position change
                    return !this.dataConnectionState[DATACONNECTIONSTATE_doNotChangePositionInTheDataGridViewControl] &&
                            this.dataConnectionState[DATACONNECTIONSTATE_positionChangingInCurrencyManager];
                }
            }

            public bool ProcessingListChangedEvent
            {
                get
                {
                    return this.dataConnectionState[DATACONNECTIONSTATE_processingListChangedEvent];
                }
            }

            public bool ProcessingMetaDataChanges
            {
                get
                {
                    return this.dataConnectionState[DATACONNECTIONSTATE_processingMetaDataChanges];
                }
            }

            public bool RestoreRow
            {
                get
                {
                    Debug.Assert(this.dataConnectionState[DATACONNECTIONSTATE_cancellingRowEdit]);
                    return this.dataConnectionState[DATACONNECTIONSTATE_restoreRow];
                }
            }

            public void AddNew()
            {
                if (this.currencyManager != null)
                {
                    // don't call AddNew on a suspended currency manager.
                    if (!this.currencyManager.ShouldBind)
                    {
                        return;
                    }

                    Debug.Assert(this.currencyManager.AllowAdd, "why did we call AddNew on the currency manager when the currency manager does not allow new rows?");
                    this.dataConnectionState[DATACONNECTIONSTATE_finishedAddNew] = false;

                    this.dataConnectionState[DATACONNECTIONSTATE_inEndCurrentEdit] = true;
                    try
                    {
                        this.currencyManager.EndCurrentEdit();
                    }
                    finally
                    {
                        this.dataConnectionState[DATACONNECTIONSTATE_inEndCurrentEdit] = false;
                    }

                    this.dataConnectionState[DATACONNECTIONSTATE_inAddNew] = true;

                    try
                    {
                        this.currencyManager.AddNew();
                    }
                    finally
                    {
                        this.dataConnectionState[DATACONNECTIONSTATE_inAddNew] = false;
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
                if (this.currencyManager == null)
                {
                    return;
                }

                PropertyDescriptor sortField = null;
                SortOrder sortOrder;
                GetSortingInformationFromBackend(out sortField, out sortOrder);

                // If we are not bound to a sorted IBindingList then set the SortGlyphDirection to SortOrder.None
                // on each dataBound DataGridViewColumn.
                // This will have the side effect of setting DataGridView::SortedColumn to null and setting DataGridView::SortOrder to null.
                if (sortField == null)
                {
                    for (int i = 0; i < this.owner.Columns.Count; i++)
                    {
                        if (this.owner.Columns[i].IsDataBound)
                        {
                            this.owner.Columns[i].HeaderCell.SortGlyphDirection = SortOrder.None;
                        }
                    }

                    this.owner.sortedColumn = null;
                    this.owner.sortOrder = SortOrder.None;

                    // now return;
                    return;
                }

                bool setSortedColumnYet = false;
                for (int i = 0; i < this.owner.Columns.Count; i++)
                {
                    DataGridViewColumn column = this.owner.Columns[i];
                    if (!column.IsDataBound)
                    {
                        continue;
                    }

                    if (column.SortMode == DataGridViewColumnSortMode.NotSortable)
                    {
                        continue;
                    }

                    if (String.Equals(column.DataPropertyName, sortField.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        // Set the sorted column on the dataGridView only if the sorted Field is set outside the dataGridView.
                        // If the sortedField is set inside the dataGridView ( either by user clicking on a ColumnHeader or by user calling DGV.Sort(...)
                        // then we don't want to tamper w/ it.
                        if (!setSortedColumnYet && !this.owner.InSortOperation)
                        {
                            this.owner.sortedColumn = column;
                            this.owner.sortOrder = sortOrder;
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
                Debug.Assert(this.props != null);
                return this.props[boundColumnIndex].Converter;
            }

            // given a data field name we get the bound index
            public int BoundColumnIndex(string dataPropertyName)
            {
                if (this.props == null)
                {
                    return -1;
                }

                int ret = -1;

                for (int i = 0; i < this.props.Count; i++)
                {
                    if (String.Compare(this.props[i].Name, dataPropertyName, true /*ignoreCase*/, CultureInfo.InvariantCulture) == 0)
                    {
                        ret = i;
                        break;
                    }
                }

                return ret;
            }

            public SortOrder BoundColumnSortOrder(int boundColumnIndex)
            {
                IBindingList ibl = this.currencyManager != null ? this.currencyManager.List as IBindingList : null;
                IBindingListView iblv = ibl != null ? ibl as IBindingListView : null;

                if (ibl == null || !ibl.SupportsSorting || !ibl.IsSorted)
                {
                    return SortOrder.None;
                }

                PropertyDescriptor sortProperty;
                SortOrder sortOrder;

                GetSortingInformationFromBackend(out sortProperty, out sortOrder);

                if (sortOrder == SortOrder.None)
                {
                    Debug.Assert(sortProperty == null);
                    return SortOrder.None;
                }

                if (String.Compare(this.props[boundColumnIndex].Name, sortProperty.Name, true /*ignoreCase*/, CultureInfo.InvariantCulture) == 0)
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
                Debug.Assert(this.props != null);
                return this.props[boundColumnIndex].PropertyType;
            }

#if DEBUG
            private void CheckRowCount(ListChangedEventArgs e)
            {
                if (e.ListChangedType != ListChangedType.Reset)
                {
                    return;
                }

                int dataGridViewRowsCount = this.owner.Rows.Count;

                Debug.Assert(DataBoundRowsCount() == this.currencyManager.List.Count || (this.owner.Columns.Count == 0 && dataGridViewRowsCount == 0),
                             "there should be the same number of rows in the dataGridView's Row Collection as in the back end list");
            }
#endif // DEBUG

            private void currencyManager_ListChanged(object sender, ListChangedEventArgs e)
            {
                Debug.Assert(sender == this.currencyManager, "did we forget to unregister our ListChanged event handler?");

                this.dataConnectionState[DATACONNECTIONSTATE_processingListChangedEvent] = true;
                try
                {
                    ProcessListChanged(e);
                }
                finally
                {
                    this.dataConnectionState[DATACONNECTIONSTATE_processingListChangedEvent] = false;
                }

                this.owner.OnDataBindingComplete(e.ListChangedType);

                this.lastListCount = this.currencyManager.Count;

#if DEBUG
                CheckRowCount(e);
#endif // DEBUG
            }

            private void ProcessListChanged(ListChangedEventArgs e)
            {
                if (e.ListChangedType == System.ComponentModel.ListChangedType.PropertyDescriptorAdded ||
                    e.ListChangedType == System.ComponentModel.ListChangedType.PropertyDescriptorDeleted ||
                    e.ListChangedType == System.ComponentModel.ListChangedType.PropertyDescriptorChanged)
                {
                    this.dataConnectionState[DATACONNECTIONSTATE_processingMetaDataChanges] = true;
                    try
                    {
                        DataSourceMetaDataChanged();
                    }
                    finally
                    {
                        this.dataConnectionState[DATACONNECTIONSTATE_processingMetaDataChanges] = false;
                    }
                    return;
                }

                Debug.Assert(!this.dataConnectionState[DATACONNECTIONSTATE_inAddNew] || !this.dataConnectionState[DATACONNECTIONSTATE_finishedAddNew],
                             "if inAddNew is true then finishedAddNew should be false");

                // The value of AllowUserToAddRowsInternal changed under the DataGridView.
                // Recreate the rows and return.
                if (this.dataConnectionState[DATACONNECTIONSTATE_cachedAllowUserToAddRowsInternal] != this.owner.AllowUserToAddRowsInternal)
                {
                    this.dataConnectionState[DATACONNECTIONSTATE_listWasReset] = true;
                    try
                    {
                        this.owner.RefreshRows(!this.owner.InSortOperation /*scrollIntoView*/);
                        this.owner.PushAllowUserToAddRows();
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
                if (!this.dataConnectionState[DATACONNECTIONSTATE_finishedAddNew] && this.owner.newRowIndex == e.NewIndex)
                {
                    Debug.Assert(this.owner.AllowUserToAddRowsInternal, "how did we start the add new transaction when the AllowUserToAddRowsInternal is false?");
                    if (e.ListChangedType == ListChangedType.ItemAdded)
                    {
                        if (this.dataConnectionState[DATACONNECTIONSTATE_inAddNew])
                        {
                            // still processing CurrencyManager::AddNew
                            // nothing to do
                            return;
                        }

                        if (this.dataConnectionState[DATACONNECTIONSTATE_rowValidatingInAddNew])
                        {
                            // DataGridView validation commited the AddNewRow to the back end
                            // DataGridView took care of newRowIndex, adding a new DataGridViewRow, etc
                            // we don't have to do anything
                            return;
                        }

                        // We got a ListChangedType.ItemAdded event outside row validation and outside CurrencyManager::AddNew
                        if (this.owner.Columns.Count > 0)
                        {
                            // add rows until the back end and the DGV have the same number of bound rows.
                            do
                            {
                                // the new row becomes a regular row and a "new" new row is appended
                                this.owner.newRowIndex = -1;
                                this.owner.AddNewRow(false /* createdByEditing */);
                            } while (DataBoundRowsCount() < this.currencyManager.Count);
                        }

                        this.dataConnectionState[DATACONNECTIONSTATE_finishedAddNew] = true;
                        MatchCurrencyManagerPosition(true /*scrollIntoView*/, true /*clearSelection*/);
                        return;
                    }
                    else if (e.ListChangedType == ListChangedType.ItemDeleted)
                    {
                        if (this.dataConnectionState[DATACONNECTIONSTATE_cancellingRowEdit])
                        {
                            // 'add new row' was discarded, bring back the new row default values.
                            this.owner.PopulateNewRowWithDefaultValues();
                        }
                        else if (this.dataConnectionState[DATACONNECTIONSTATE_inEndCurrentEdit] ||
                                 this.dataConnectionState[DATACONNECTIONSTATE_inAddNew])
                        {
                            // A row was deleted while the DataGridView control asked for a new row.
                            // Recreate the data grid view rows.
                            this.dataConnectionState[DATACONNECTIONSTATE_listWasReset] = true;
                            try
                            {
                                this.owner.RefreshRows(!this.owner.InSortOperation /*scrollIntoView*/);
                                this.owner.PushAllowUserToAddRows();
                            }
                            finally
                            {
                                this.dataConnectionState[DATACONNECTIONSTATE_listWasReset] = false;
                            }
                        }
                        else if (this.dataConnectionState[DATACONNECTIONSTATE_inDeleteOperation] && this.currencyManager.List.Count == 0)
                        {
                            // if System.Data.DataView was in AddNew transaction and we delete all the rows in the System.Data.DataView
                            // then System.Data.DataView will close the AddNew transaction under us
                            // start another AddNew transaction on the back end
                            this.AddNew();
                        }
                    }

                    return;
                }

                Debug.Assert(DataBoundRowsCount() != -1, "the data bound data grid view rows count should be at least 0");

                // we received an ListChangedType.ItemAdded and our list has exactly the same number of rows as the back-end.
                // return.
                if (e.ListChangedType == ListChangedType.ItemAdded &&
                    this.currencyManager.List.Count == (this.owner.AllowUserToAddRowsInternal ? this.owner.Rows.Count - 1 : this.owner.Rows.Count))
                {
                    if (this.dataConnectionState[DATACONNECTIONSTATE_inDeleteOperation] && this.dataConnectionState[DATACONNECTIONSTATE_didNotDeleteRowFromDataGridView])
                    {
                        // we received a ListChangedType.ItemAdded while we were deleting rows from the back end
                        // and we stil haven't removed a row from the data grid view
                        // System.Data.DataView started an AddNew transaction as a result of deleting rows
                        // mark the state as itemAddedInDeleteOperation
                        this.dataConnectionState[DATACONNECTIONSTATE_itemAddedInDeleteOperation] = true;

                        // The DGV gets in this situation when the user deletes the last row in a Master table.
                        // At this point, the Child table forces an AddNew on the Master Table.
                        // See comments where we declare _itemAddedInDeleteOperation");
                        //
                        Debug.Assert(this.currencyManager.List.Count == 1);

                        // if we were on an AddNew transaction then the MASTER table would have had more than 1 row.
                        // So the Child table should not have forcefully added a row on the MASTER table");
                        // 
                        Debug.Assert(this.dataConnectionState[DATACONNECTIONSTATE_finishedAddNew]);
                    }

                    return;
                }

                // this is the first ItemDeleted event we get after the ItemAdded event that we got while we were deleting rows from the data view
                // don't do anything - this is the equivalent of removing the row that was added before
                if (e.ListChangedType == ListChangedType.ItemDeleted)
                {
                    if (this.dataConnectionState[DATACONNECTIONSTATE_inDeleteOperation] &&
                        this.dataConnectionState[DATACONNECTIONSTATE_itemAddedInDeleteOperation] &&
                        this.dataConnectionState[DATACONNECTIONSTATE_didNotDeleteRowFromDataGridView])
                    {
                        // we removed the item that was added during the delete operation
                        this.dataConnectionState[DATACONNECTIONSTATE_itemAddedInDeleteOperation] = false;
                        Debug.Assert(this.currencyManager.List.Count == 0, "we deleted the row that the Child table forcefully added");
                    }
                    else if (!this.dataConnectionState[DATACONNECTIONSTATE_finishedAddNew] &&
                             this.dataConnectionState[DATACONNECTIONSTATE_inEndCurrentEdit])
                    {
                        // EndCurrentEdit caused an item to be deleted while in AddNew.
                        // Recreate the rows.
                        this.dataConnectionState[DATACONNECTIONSTATE_listWasReset] = true;
                        try
                        {
                            this.owner.RefreshRows(!this.owner.InSortOperation /*scrollIntoView*/);
                            this.owner.PushAllowUserToAddRows();
                        }
                        finally
                        {
                            this.dataConnectionState[DATACONNECTIONSTATE_listWasReset] = false;
                        }
                        return;
                    }
                    else if (this.currencyManager.List.Count == DataBoundRowsCount())
                    {
                        return;
                    }
                }

                // when we get the ListChanged notification the position in the currency manager already changed
                // so do not change the position when we get the RowEnter event 
                this.dataConnectionState[DATACONNECTIONSTATE_doNotChangePositionInTheCurrencyManager] = true;

                try
                {
                    switch (e.ListChangedType)
                    {
                        case ListChangedType.Reset:
                            this.dataConnectionState[DATACONNECTIONSTATE_listWasReset] = true;
                            bool startUpdateInternal = this.owner.Visible;
                            if (startUpdateInternal)
                            {
                                this.owner.BeginUpdateInternal();
                            }
                            try
                            {
                                this.owner.RefreshRows(!this.owner.InSortOperation /*scrollIntoView*/);
                                this.owner.PushAllowUserToAddRows();

                                // ListChangedType.Reset can signal that the list became sorted or that the list is not sorted anymore.
                                this.ApplySortingInformationFromBackEnd();
                            }
                            finally
                            {
                                // this will also set DATACONNECTIONSTATE_listWasReset to false
                                ResetDataConnectionState();
                                if (startUpdateInternal)
                                {
                                    this.owner.EndUpdateInternal(false);
                                    this.owner.Invalidate(true);
                                }
                            }
                            break;
                        case ListChangedType.ItemAdded:
                            if (this.owner.NewRowIndex == -1 || e.NewIndex != this.owner.Rows.Count)
                            {
                                this.owner.Rows.InsertInternal(e.NewIndex, this.owner.RowTemplateClone, true /*force*/);
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
                            this.owner.Rows.RemoveAtInternal(e.NewIndex, true /*force*/);
                            this.dataConnectionState[DATACONNECTIONSTATE_didNotDeleteRowFromDataGridView] = false;
                            break;
                        case ListChangedType.ItemMoved:
                            // an ItemMoved event means that all the rows shifted up or down by 1
                            // we have to invalidate all the rows in between
                            Debug.Assert(e.OldIndex > -1, "the currency manager should have taken care of this case");
                            Debug.Assert(e.NewIndex > -1, "how can we move an item outside of the list?");
                            int lo = Math.Min(e.OldIndex, e.NewIndex);
                            int hi = Math.Max(e.OldIndex, e.NewIndex);
                            this.owner.InvalidateRows(lo, hi);
                            break;
                        case ListChangedType.ItemChanged:
                            Debug.Assert(e.NewIndex != -1, "the item changed event does not cover changes to the entire list");
                            string dataPropertyName = null;
                            if (e.PropertyDescriptor != null)
                            {
                                dataPropertyName = ((System.ComponentModel.MemberDescriptor)(e.PropertyDescriptor)).Name;
                            }
                            for (int columnIndex = 0; columnIndex < this.owner.Columns.Count; columnIndex++)
                            {
                                DataGridViewColumn dataGridViewColumn = this.owner.Columns[columnIndex];
                                if (dataGridViewColumn.Visible && dataGridViewColumn.IsDataBound)
                                {
                                    if (!string.IsNullOrEmpty(dataPropertyName))
                                    {
                                        if (String.Compare(dataGridViewColumn.DataPropertyName, dataPropertyName, true /*ignoreCase*/, CultureInfo.InvariantCulture) == 0)
                                        {
                                            this.owner.OnCellCommonChange(columnIndex, e.NewIndex);
                                        }
                                    }
                                    else
                                    {
                                        this.owner.OnCellCommonChange(columnIndex, e.NewIndex);
                                    }
                                }
                            }
                            // Repaint the row header cell to show potential error icon
                            this.owner.InvalidateCell(-1, e.NewIndex);
                            // update the editing control value if the data changed in the row the user was editing
                            if (this.owner.CurrentCellAddress.Y == e.NewIndex && this.owner.IsCurrentCellInEditMode)
                            {
                                this.owner.RefreshEdit();
                            }
                            break;
                        default:
                            break;
                    }
                    // now put the position in the DataGridView control according to the position in the currency manager
                    if (this.owner.Rows.Count > 0 &&
                        !this.dataConnectionState[DATACONNECTIONSTATE_doNotChangePositionInTheDataGridViewControl] &&
                        !this.owner.InSortOperation)
                    {
                        MatchCurrencyManagerPosition(false /*scrollIntoView*/, e.ListChangedType == ListChangedType.Reset /*clearSelection*/);
                    }
                }
                finally
                {
                    this.dataConnectionState[DATACONNECTIONSTATE_doNotChangePositionInTheCurrencyManager] = false;
                }
            }

            private void currencyManager_PositionChanged(object sender, EventArgs e)
            {
                Debug.Assert(sender == this.currencyManager, "did we forget to unregister our events?");
                if (this.owner.Columns.Count == 0)
                {
                    Debug.Assert(this.owner.CurrentCellAddress.X == -1);
                    // No columns means we can't set the current cell.
                    // This happens when all columns where removed from the dataGridView, and all rows were cleared.
                    // Discuss this with Daniel/Mark.
                    // One solution: impose at least one visible column - all the time.
                    return;
                }

                if (this.owner.Rows.Count == (owner.AllowUserToAddRowsInternal ? 1 : 0))
                {
                    // the dataGridView control has not yet been notified that the list is not empty
                    // don't do anything
                    return;
                }

                if (this.dataConnectionState[DATACONNECTIONSTATE_doNotChangePositionInTheDataGridViewControl])
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
                if (this.owner.AllowUserToAddRowsInternal &&                                // condition 1.
                    this.dataConnectionState[DATACONNECTIONSTATE_finishedAddNew] &&         // condition 2.
                    !this.dataConnectionState[DATACONNECTIONSTATE_inAddNew] &&              // condition 2.
                    this.currencyManager.Position > -1 &&                                   // condition 3.
                    this.currencyManager.Position == this.owner.NewRowIndex &&              // condition 4.
                    this.owner.CurrentCellAddress.Y != this.owner.NewRowIndex &&            // condition 5.
                    this.currencyManager.Count == DataBoundRowsCount() + 1)                 // condition 6.
                {
                    return;
                }

                   

                this.dataConnectionState[DATACONNECTIONSTATE_positionChangingInCurrencyManager] = true;
                try
                {
                    if (!this.owner.InSortOperation)
                    {
                        bool scrollIntoView = true;
                        // When an item is repositioned in a sorted column, while its 
                        // row is being committed, don't scroll it into view.
                        if (this.dataConnectionState[DATACONNECTIONSTATE_rowValidatingInAddNew])
                        {
                            IBindingList ibl = this.currencyManager.List as IBindingList;
                            if (ibl != null && ibl.SupportsSorting && ibl.IsSorted)
                            {
                                scrollIntoView = false;
                            }
                        }

                        // If the user hit Escape while in AddNew then we clear the selection.
                        bool clearSelection = this.dataConnectionState[DATACONNECTIONSTATE_cancellingRowEdit] && !this.dataConnectionState[DATACONNECTIONSTATE_finishedAddNew];
                        // Otherwise we clear the selection if the last list count is still uninitialized
                        // or if it is the same as the current list count.
                        clearSelection |= this.lastListCount == -1 || this.lastListCount == this.currencyManager.Count;
                        MatchCurrencyManagerPosition(scrollIntoView, clearSelection);
                    }
                }
                finally
                {
                    this.dataConnectionState[DATACONNECTIONSTATE_positionChangingInCurrencyManager] = false;
                }
            }

            //
            // This function will return the number of rows inside the DataGridView which are data bound.
            // For instance, the AddNewRow inside the DataGridView is not data bound so it should not be counted.
            //
            private int DataBoundRowsCount()
            {
                int result = this.owner.Rows.Count;
                if (this.owner.AllowUserToAddRowsInternal && this.owner.Rows.Count > 0)
                {
                    Debug.Assert(this.owner.NewRowIndex != -1, "the NewRowIndex is -1 only when AllowUserToAddRows is false");

                    // We have to check if the AddNew row is data bound or not.
                    // The AddNew row is data bound if the user is positioned in the AddNew row and the AddNew row is not dirty
                    if (this.owner.CurrentCellAddress.Y != this.owner.NewRowIndex || this.owner.IsCurrentRowDirty)
                    {
                        // The AddNew row in the DataGridView row collection is not data bound.
                        // Substract it from the row count;
                        result--;
                    }
                }

                return result;
            }

            private void DataSource_Initialized(object sender, EventArgs e)
            {
                Debug.Assert(sender == this.dataSource);
                Debug.Assert(this.dataSource is ISupportInitializeNotification);
                Debug.Assert(this.dataConnectionState[DATACONNECTIONSTATE_dataSourceInitializedHookedUp]);

                ISupportInitializeNotification dsInit = this.dataSource as ISupportInitializeNotification;
                // Unhook the Initialized event.
                if (dsInit != null)
                {
                    dsInit.Initialized -= new EventHandler(DataSource_Initialized);
                }

                // The wait is over: DataSource is initialized.
                this.dataConnectionState[DATACONNECTIONSTATE_dataSourceInitializedHookedUp] = false;

                // Update the data manager
                SetDataConnection(this.dataSource, this.dataMember);
                Debug.Assert(this.currencyManager != null);
                this.owner.RefreshColumnsAndRows();
                this.owner.OnDataBindingComplete(ListChangedType.Reset);
            }

            private void DataSourceMetaDataChanged()
            {
                Debug.Assert(this.currencyManager != null);

                // get the new meta data
                this.props = this.currencyManager.GetItemProperties();

                // when AutoGenerate == true: RefreshColumnsAndRows will delete the previously dataBound columns and create new dataBounds columns
                //
                // AutoGenerate == false : RefreshColumnsAndRows will refresh the property descriptors for the dataBound Columns.
                // Some unBound columns may become dataBound, some dataBounds columns may become unBound
                //

                this.owner.RefreshColumnsAndRows();
            }

            public void DeleteRow(int rowIndex)
            {
                this.dataConnectionState[DATACONNECTIONSTATE_doNotChangePositionInTheDataGridViewControl] = true;
                try
                {
                    if (!this.dataConnectionState[DATACONNECTIONSTATE_finishedAddNew])
                    {
                        Debug.Assert(this.owner.AllowUserToAddRowsInternal, "how did we start an add new row transaction if the dataGridView control has AllowUserToAddRows == false?");
                        bool deleteAddNewRow = false;
                        if (this.owner.newRowIndex == this.currencyManager.List.Count)
                        {
                            // the user clicked on the 'add new row' and started typing
                            deleteAddNewRow = (rowIndex == this.owner.newRowIndex - 1);
                        }
                        else
                        {
                            // the user clicked on the 'add new row' but did not start typing
                            Debug.Assert(this.owner.newRowIndex == this.currencyManager.List.Count - 1);
                            deleteAddNewRow = (rowIndex == this.owner.newRowIndex);
                        }

                        if (deleteAddNewRow)
                        {
                            // we finished the add new transaction
                            CancelRowEdit(false /*restoreRow*/, true /*addNewFinished*/);
                        }
                        else
                        {
                            // start the Delete operation
                            this.dataConnectionState[DATACONNECTIONSTATE_inDeleteOperation] = true;
                            // we did not delete any rows from the data grid view yet
                            this.dataConnectionState[DATACONNECTIONSTATE_didNotDeleteRowFromDataGridView] = true;
                            try
                            {
                                this.currencyManager.RemoveAt(rowIndex);
                            }
                            finally
                            {
                                this.dataConnectionState[DATACONNECTIONSTATE_inDeleteOperation] = false;
                                this.dataConnectionState[DATACONNECTIONSTATE_didNotDeleteRowFromDataGridView] = false;
                            }
                        }
                    }
                    else
                    {
                        // start the Delete operation
                        this.dataConnectionState[DATACONNECTIONSTATE_inDeleteOperation] = true;
                        // we did not delete any rows from the data grid view yet
                        this.dataConnectionState[DATACONNECTIONSTATE_didNotDeleteRowFromDataGridView] = true;
                        try
                        {
                            this.currencyManager.RemoveAt(rowIndex);
                        }
                        finally
                        {
                            this.dataConnectionState[DATACONNECTIONSTATE_inDeleteOperation] = false;
                            this.dataConnectionState[DATACONNECTIONSTATE_didNotDeleteRowFromDataGridView] = false;
                        }
                    }
                }
                finally
                {
                    this.dataConnectionState[DATACONNECTIONSTATE_doNotChangePositionInTheDataGridViewControl] = false;
                }
            }

            public bool DataFieldIsReadOnly(int boundColumnIndex)
            {
                if (this.props == null)
                {
                    Debug.Fail("we only care about which data fields are read only when we are data bound");
                    return false;
                }

                return this.props[boundColumnIndex].IsReadOnly;
            }

            // All we do in dispose is to unwire the data source.
            public void Dispose()
            {
                UnWireEvents();

                // Set the currency manager to null so if someone would want to resurect this data grid view data connection
                // we would not unwire the events from the curency manager twice.
                // (NOTE: resurecting a disposed data grid view data connection is not allowed.)
                // 
                this.currencyManager = null;
            }

            private static DataGridViewColumn GetDataGridViewColumnFromType(Type type)
            {
                DataGridViewColumn dataGridViewColumn;
                TypeConverter imageTypeConverter = TypeDescriptor.GetConverter(typeof(Image));
                if (type.Equals(typeof(bool)) || type.Equals(typeof(CheckState)))
                {
                    dataGridViewColumn = new DataGridViewCheckBoxColumn(type.Equals(typeof(CheckState)));
                }
                else if (typeof(System.Drawing.Image).IsAssignableFrom(type) || imageTypeConverter.CanConvertFrom(type))
                {
                    dataGridViewColumn = new DataGridViewImageColumn();
                }
                else
                {
                    dataGridViewColumn = new DataGridViewTextBoxColumn();
                }
                return dataGridViewColumn;
            }

            public DataGridViewColumn[] GetCollectionOfBoundDataGridViewColumns()
            {
                if (this.props == null)
                {
                    return null;
                }
                ArrayList cols = new ArrayList();

                for (int i = 0; i < this.props.Count; i++)
                {
                    if (typeof(IList).IsAssignableFrom(this.props[i].PropertyType))
                    {
                        // we have an IList. It could be a byte[] in which case we want to generate an Image column
                        //
                        TypeConverter imageTypeConverter = TypeDescriptor.GetConverter(typeof(Image));
                        if (!imageTypeConverter.CanConvertFrom(this.props[i].PropertyType))
                        {
                            continue;
                        }
                    }

                    DataGridViewColumn dataGridViewColumn = GetDataGridViewColumnFromType(this.props[i].PropertyType);
                    dataGridViewColumn.IsDataBoundInternal = true;
                    dataGridViewColumn.BoundColumnIndex = i;
                    // we set the data property name
                    // if you plan on removing this, then you have to change the lookup into
                    // the GetCollectionOfBoundDataGridViewColumns
                    dataGridViewColumn.DataPropertyName = this.props[i].Name;
                    dataGridViewColumn.Name = this.props[i].Name;
                    dataGridViewColumn.BoundColumnConverter = this.props[i].Converter;
                    dataGridViewColumn.HeaderText = !String.IsNullOrEmpty(this.props[i].DisplayName) ? this.props[i].DisplayName : this.props[i].Name;
                    dataGridViewColumn.ValueType = this.props[i].PropertyType;

                    dataGridViewColumn.IsBrowsableInternal = this.props[i].IsBrowsable;

                    dataGridViewColumn.ReadOnly = props[i].IsReadOnly;

                    cols.Add(dataGridViewColumn);
                }

                DataGridViewColumn[] ret = new DataGridViewColumn[cols.Count];
                cols.CopyTo(ret);
                return ret;
            }

            private void GetSortingInformationFromBackend(out PropertyDescriptor sortProperty, out SortOrder sortOrder)
            {
                IBindingList ibl = this.currencyManager != null ? this.currencyManager.List as IBindingList : null;
                IBindingListView iblv = ibl != null ? ibl as IBindingListView : null;

                if (ibl == null || !ibl.SupportsSorting || !ibl.IsSorted)
                {
                    sortOrder = SortOrder.None;
                    sortProperty = null;
                    return;
                }

                if (ibl.SortProperty != null)
                {
                    sortProperty = ibl.SortProperty;
                    sortOrder = ibl.SortDirection == ListSortDirection.Ascending ? SortOrder.Ascending : SortOrder.Descending;
                }
                else if (iblv != null)
                {
                    // Maybe the data view is sorted on multiple columns.
                    // Go thru the IBindingListView which offers the entire list of sorted columns 
                    // and pick the first one as the SortedColumn.
                    ListSortDescriptionCollection sorts = iblv.SortDescriptions;
                    if (sorts != null && 
                        sorts.Count > 0 &&
                        sorts[0].PropertyDescriptor != null)
                    {
                        sortProperty = sorts[0].PropertyDescriptor;
                        sortOrder = sorts[0].SortDirection == ListSortDirection.Ascending ? SortOrder.Ascending : SortOrder.Descending;
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

            public void ResetCachedAllowUserToAddRowsInternal()
            {
                this.dataConnectionState[DATACONNECTIONSTATE_cachedAllowUserToAddRowsInternal] = this.owner.AllowUserToAddRowsInternal;
            }

            private void ResetDataConnectionState()
            {
                // Microsoft: I wish there would be a Reset method on BitVector32...
                this.dataConnectionState = new BitVector32(DATACONNECTIONSTATE_finishedAddNew);

                if (this.currencyManager != null)
                {
                    this.dataConnectionState[DATACONNECTIONSTATE_interestedInRowEvents] = true;
                }

                ResetCachedAllowUserToAddRowsInternal();
            }

            public void SetDataConnection(object dataSource, string dataMember)
            {
                if (this.dataConnectionState[DATACONNECTIONSTATE_dataConnection_inSetDataConnection])
                {
                    return;
                }

                ResetDataConnectionState();

                if (dataMember == null)
                {
                    dataMember = String.Empty;
                }

                ISupportInitializeNotification dsInit = this.dataSource as ISupportInitializeNotification;
                if (dsInit != null && this.dataConnectionState[DATACONNECTIONSTATE_dataSourceInitializedHookedUp])
                {
                    // If we previously hooked the data source's ISupportInitializeNotification
                    // Initialized event, then unhook it now (we don't always hook this event,
                    // only if we needed to because the data source was previously uninitialized)
                    dsInit.Initialized -= new EventHandler(DataSource_Initialized);
                    this.dataConnectionState[DATACONNECTIONSTATE_dataSourceInitializedHookedUp] = false;
                }

                this.dataSource = dataSource;
                this.dataMember = dataMember;

                if (this.owner.BindingContext == null)
                {
                    return;
                }

                this.dataConnectionState[DATACONNECTIONSTATE_dataConnection_inSetDataConnection] = true;
                try
                {
                    // unwire the events
                    UnWireEvents();

                    if (this.dataSource != null && this.owner.BindingContext != null && !(this.dataSource == Convert.DBNull))
                    {
                        dsInit = this.dataSource as ISupportInitializeNotification;
                        if (dsInit != null && !dsInit.IsInitialized)
                        {
                            if (!this.dataConnectionState[DATACONNECTIONSTATE_dataSourceInitializedHookedUp])
                            {
                                dsInit.Initialized += new EventHandler(DataSource_Initialized);
                                this.dataConnectionState[DATACONNECTIONSTATE_dataSourceInitializedHookedUp] = true;
                            }
                            this.currencyManager = null;
                        }
                        else
                        {
                            this.currencyManager = this.owner.BindingContext[this.dataSource, this.dataMember] as CurrencyManager;
                        }
                    }
                    else
                    {
                        this.currencyManager = null;
                    }

                    // wire the events
                    WireEvents();
                    if (this.currencyManager != null)
                    {
                        this.props = this.currencyManager.GetItemProperties();
                    }
                    else
                    {
                        this.props = null;
                    }
                }
                finally
                {
                    this.dataConnectionState[DATACONNECTIONSTATE_dataConnection_inSetDataConnection] = false;
                }

                ResetCachedAllowUserToAddRowsInternal();

                if (this.currencyManager != null)
                {
                    this.lastListCount = this.currencyManager.Count;
                }
                else
                {
                    this.lastListCount = -1;
                }
            }

            public string GetError(int rowIndex)
            {
                IDataErrorInfo errInfo = null;
                try 
                {
                    errInfo = this.currencyManager[rowIndex] as IDataErrorInfo;
                }
                catch (Exception exception)
                {
                    if (ClientUtils.IsCriticalException(exception) && !(exception is IndexOutOfRangeException))
                    {
                        throw;
                    }
                    DataGridViewDataErrorEventArgs dgvdee = new DataGridViewDataErrorEventArgs(exception,  -1 /*columnIndex*/, rowIndex,
                                                                                               DataGridViewDataErrorContexts.Display);
                    this.owner.OnDataErrorInternal(dgvdee);
                    if (dgvdee.ThrowException)
                    {
                        throw dgvdee.Exception;
                    }
                }

                if (errInfo != null)
                {
                    return errInfo.Error;
                }
                else
                {
                    return String.Empty;
                }
            }

            public string GetError(int boundColumnIndex, int columnIndex, int rowIndex)
            {
                Debug.Assert(rowIndex >= 0);

                IDataErrorInfo errInfo = null;
                try
                {
                    errInfo = this.currencyManager[rowIndex] as IDataErrorInfo;
                }
                catch (Exception exception)
                {
                    if (ClientUtils.IsCriticalException(exception) && !(exception is IndexOutOfRangeException))
                    {
                        throw;
                    }
                    DataGridViewDataErrorEventArgs dgvdee = new DataGridViewDataErrorEventArgs(exception, columnIndex, rowIndex,
                                                                                               DataGridViewDataErrorContexts.Display);
                    this.owner.OnDataErrorInternal(dgvdee);
                    if (dgvdee.ThrowException)
                    {
                        throw dgvdee.Exception;
                    }
                }

                if (errInfo != null)
                {
                    return errInfo[this.props[boundColumnIndex].Name];
                }
                else
                {
                    return String.Empty;
                }
            }

            public object GetValue(int boundColumnIndex, int columnIndex, int rowIndex)
            {
                Debug.Assert(rowIndex >= 0);
                object value = null;
                try
                {
                    value = this.props[boundColumnIndex].GetValue(this.currencyManager[rowIndex]);
                }
                catch (Exception exception)
                {
                    if (ClientUtils.IsCriticalException(exception) && !(exception is IndexOutOfRangeException))
                    {
                        throw;
                    }
                    DataGridViewDataErrorEventArgs dgvdee = new DataGridViewDataErrorEventArgs(exception, columnIndex, rowIndex,
                                                                                               DataGridViewDataErrorContexts.Display);
                    this.owner.OnDataErrorInternal(dgvdee);
                    if (dgvdee.ThrowException)
                    {
                        throw dgvdee.Exception;
                    }
                }
                return value;
            }

            public void MatchCurrencyManagerPosition(bool scrollIntoView, bool clearSelection)
            {
                if (this.owner.Columns.Count == 0)
                {
#if DEBUG
                    // all the properties in the currency manager should be either Browsable(false) or point to sub lists
                    if (this.props != null)
                    {
                        for (int i = 0; i < this.props.Count; i ++)
                        {
                            Debug.Assert(!props[i].IsBrowsable || typeof(IList).IsAssignableFrom(props[i].PropertyType), "if the DGV does not have any columns then the properties in the currency manager should be Browsable(false) or point to sub lists");
                        }
                    }
#endif // DEBUG

                    // nothing to do
                    return;
                }

                int columnIndex = this.owner.CurrentCellAddress.X == -1 ? this.owner.FirstDisplayedColumnIndex : this.owner.CurrentCellAddress.X;

                // Treat case where columnIndex == -1. We change the visibility of the first column.
                if (columnIndex == -1)
                {
                    DataGridViewColumn dataGridViewColumn = this.owner.Columns.GetFirstColumn(DataGridViewElementStates.None);
                    Debug.Assert(dataGridViewColumn != null);
                    dataGridViewColumn.Visible = true;
                    columnIndex = dataGridViewColumn.Index;
                }

                int rowIndex = this.currencyManager.Position;

                Debug.Assert(rowIndex >= -1);

                if (rowIndex == -1)
                {
                    // Occurs when calling SuspendBinding() on the currency manager?
                    if (!this.owner.SetCurrentCellAddressCore(-1, -1,
                                                              false, /*setAnchorCellAddress*/
                                                              false, /*validateCurrentCell*/
                                                              false  /*throughMouseClick*/))
                    {
                        throw new InvalidOperationException(string.Format(SR.DataGridView_CellChangeCannotBeCommittedOrAborted));
                    }
                }
                else if (rowIndex < this.owner.Rows.Count)
                {
                    // the currency manager sends the PositionChanged event before the ListChanged event.
                    // This means that it is possible for the data grid view to receive the position changed event
                    // before it had a chance to created its rows.
                    // So, if the position inside the currency manager is greater than the number of rows in the data grid view
                    // don't do anything.
                    // NOTE: because the currency manager will fire the list changed event after the position changed event
                    // the data grid view will actually get a second chance at matching the position inside the currency manager.

                    // Do not allow to set the current cell to an invisible cell
                    if ((this.owner.Rows.GetRowState(rowIndex) & DataGridViewElementStates.Visible) == 0)
                    {
                        // Make the target row visible.
                        this.owner.Rows[rowIndex].Visible = true;
                    }

                    if (rowIndex == this.owner.CurrentCellAddress.Y && columnIndex == this.owner.CurrentCellAddress.X)
                    {
                        return;
                    }

                    // Scroll target cell into view first.
                    if ((scrollIntoView && !this.owner.ScrollIntoView(columnIndex, rowIndex, true)) ||
                        (columnIndex < this.owner.Columns.Count && rowIndex < this.owner.Rows.Count && 
                         !this.owner.SetAndSelectCurrentCellAddress(columnIndex, rowIndex,
                                                                   true,  /*setAnchorCellAddress*/
                                                                   false, /*validateCurrentCell*/
                                                                   false,  /*throughMouseClick*/
                                                                   clearSelection,
                                                                   false /*forceCurrentCellSelection*/)))
                    {
                        throw new InvalidOperationException(string.Format(SR.DataGridView_CellChangeCannotBeCommittedOrAborted));
                    }
                }
            }

            public void CancelRowEdit(bool restoreRow, bool addNewFinished)
            {
                this.dataConnectionState[DATACONNECTIONSTATE_cancellingRowEdit] = true;
                this.dataConnectionState[DATACONNECTIONSTATE_restoreRow] = restoreRow;
                try
                {
                    object currentItem = null;
                    if (this.currencyManager.Position >= 0 && this.currencyManager.Position < this.currencyManager.List.Count )
                    {
                        currentItem = this.currencyManager.Current;
                    }

                    this.currencyManager.CancelCurrentEdit();

                    // CurrencyManager no longer starts a new transaction automatically
                    // when we call CurrencyManager::CancelCurrentEdit.
                    // So, if the current item inside the currency manager did not change, we have to start a new transaction.
                    // (If the current item inside the currency manager changed, then the currency manager would have already started a new transaction).
                    IEditableObject editableObject = null;
                    if (this.currencyManager.Position >= 0 && this.currencyManager.Position < this.currencyManager.List.Count )
                    {
                        editableObject = this.currencyManager.Current as IEditableObject;
                    }

                    if (editableObject != null && currentItem == editableObject)
                    {
                        editableObject.BeginEdit();
                    }

                }
                finally
                {
                    this.dataConnectionState[DATACONNECTIONSTATE_cancellingRowEdit] = false;
                }

                if (addNewFinished)
                {
                    this.dataConnectionState[DATACONNECTIONSTATE_finishedAddNew] = true;
                }
            }

            internal void OnNewRowNeeded()
            {
                this.dataConnectionState[DATACONNECTIONSTATE_doNotChangePositionInTheDataGridViewControl] = true;
                try
                {
                    AddNew();
                }
                finally
                {
                    this.dataConnectionState[DATACONNECTIONSTATE_doNotChangePositionInTheDataGridViewControl] = false;
                }
            }

            internal void OnRowEnter(DataGridViewCellEventArgs e)
            {
                // don't change position or start a transaction in the middle of a meta data change
                if (this.dataConnectionState[DATACONNECTIONSTATE_processingMetaDataChanges])
                {
                    return;
                }

                // don't start a transaction on a suspended currency manager.
                if (!this.currencyManager.ShouldBind)
                {
                    return;
                }

                this.dataConnectionState[DATACONNECTIONSTATE_doNotChangePositionInTheDataGridViewControl] = true;
                try
                {
                    if (e.RowIndex != this.owner.NewRowIndex &&
                        !this.dataConnectionState[DATACONNECTIONSTATE_doNotChangePositionInTheCurrencyManager] &&
                        this.currencyManager.Position != e.RowIndex)            // don't automatically force an EndCurrentEdit on the currency manager
                    {
                        try
                        {
                            this.currencyManager.Position = e.RowIndex;
                        }
                        catch (Exception exception)
                        {
                            if (ClientUtils.IsCriticalException(exception))
                            {
                                throw;
                            }
                            DataGridViewCellCancelEventArgs dgvce = new DataGridViewCellCancelEventArgs(e.ColumnIndex, e.RowIndex);
                            ProcessException(exception, dgvce, false /*beginEdit*/);
                        }

                        IEditableObject iEditObj = this.currencyManager.Current as IEditableObject;
                        if (iEditObj != null)
                        {
                            iEditObj.BeginEdit();
                        }
                    }
                }
                finally
                {
                    this.dataConnectionState[DATACONNECTIONSTATE_doNotChangePositionInTheDataGridViewControl] = false;
                }
            }

            internal void OnRowValidating(DataGridViewCellCancelEventArgs e)
            {
                // don't end the transaction on a suspended currency manager.
                if (!this.currencyManager.ShouldBind)
                {
                    return;
                }

                if (!this.dataConnectionState[DATACONNECTIONSTATE_finishedAddNew] && !this.owner.IsCurrentRowDirty)
                {
                    if (!this.dataConnectionState[DATACONNECTIONSTATE_cancellingRowEdit])
                    {
                        Debug.Assert(DataBoundRowsCount() == this.currencyManager.List.Count, "if the back end was changed while in AddNew the DGV should have updated its rows collection");
                        // Cancel the current AddNew transaction
                        // doNotChangePositionInTheDataGridViewControl because we will change position
                        // when we get notification from the back end that the cancel operation was completed
                        this.dataConnectionState[DATACONNECTIONSTATE_doNotChangePositionInTheDataGridViewControl] = true;
                        try
                        {
                            CancelRowEdit(false /*restoreRow*/, false /*addNewFinished*/);
                        }
                        finally
                        {
                            this.dataConnectionState[DATACONNECTIONSTATE_doNotChangePositionInTheDataGridViewControl] = false;
                        }
                    }
                }
                else if (this.owner.IsCurrentRowDirty)
                {
                    this.dataConnectionState[DATACONNECTIONSTATE_rowValidatingInAddNew] = true;

                    try
                    {
                        this.currencyManager.EndCurrentEdit();
                    }
                    catch (Exception exception)
                    {
                        if (ClientUtils.IsCriticalException(exception))
                        {
                            throw;
                        }
                        ProcessException(exception, e, true /*beginEdit*/);
                    }
                    finally
                    {
                        this.dataConnectionState[DATACONNECTIONSTATE_rowValidatingInAddNew] = false;
                    }
                }

                // we moved away from the 'add new row', so the 'add new row' has been committed in the back-end
                // or has been rejected from the back-end. In any case, the AddNew operation completed.
                this.dataConnectionState[DATACONNECTIONSTATE_finishedAddNew] = true;
            }

            public void ProcessException(Exception exception, DataGridViewCellCancelEventArgs e, bool beginEdit)
            {
                DataGridViewDataErrorEventArgs dgvdee = new DataGridViewDataErrorEventArgs(exception, e.ColumnIndex,
                    e.RowIndex,
                    // null,
                    // null,
                    DataGridViewDataErrorContexts.Commit);
                this.owner.OnDataErrorInternal(dgvdee);

                if (dgvdee.ThrowException)
                {
                    throw dgvdee.Exception;
                }
                else if (dgvdee.Cancel)
                {
                    e.Cancel = true;
                    if (beginEdit)
                    {
                        IEditableObject iEditObj = this.currencyManager.Current as IEditableObject;
                        if (iEditObj != null)
                        {
                            iEditObj.BeginEdit();
                        }
                    }
                }
                else
                {
                    CancelRowEdit(false /*restoreRow*/, false /*finishedAddNew*/);
                    // interrupt current operation
                }
            }

            public bool PushValue(int boundColumnIndex, int columnIndex, int rowIndex, object value)
            {
                try
                {
                    if (value != null)
                    {
                        Type valueType = value.GetType();
                        Type columnType = this.owner.Columns[columnIndex].ValueType;
                        if (!columnType.IsAssignableFrom(valueType))
                        {
                            // value needs to be converted before being fed to the back-end.
                            TypeConverter boundColumnConverter = BoundColumnConverter(boundColumnIndex);
                            if (boundColumnConverter != null && boundColumnConverter.CanConvertFrom(valueType))
                            {
                                value = boundColumnConverter.ConvertFrom(value);
                            }
                            else
                            {
                                TypeConverter valueConverter = this.owner.GetCachedTypeConverter(valueType);
                                if (valueConverter != null && valueConverter.CanConvertTo(columnType))
                                {
                                    value = valueConverter.ConvertTo(value, columnType);
                                }
                            }
                        }
                    }
                    this.props[boundColumnIndex].SetValue(this.currencyManager[rowIndex], value);
                }
                catch (Exception exception)
                {
                    if (ClientUtils.IsCriticalException(exception))
                    {
                        throw;
                    }
                    DataGridViewCellCancelEventArgs e = new DataGridViewCellCancelEventArgs(columnIndex, rowIndex);
                    ProcessException(exception, e, false);
                    return !e.Cancel;
                }
                return true;
            }

            public bool ShouldChangeDataMember(object newDataSource)
            {
                if (!this.owner.Created)
                {
                    // if the owner is not created yet then data member can be valid
                    return false;
                }

                if (this.owner.BindingContext == null)
                {
                    // if we don't have the BindingContext then the data member can still be valid
                    return false;
                }

                if (newDataSource == null)
                {
                    // we have the binding context and the new data source is null
                    // we should change the data member to ""
                    return true;
                }

                CurrencyManager cm = this.owner.BindingContext[newDataSource] as CurrencyManager;
                if (cm == null)
                {
                    // if we don't have a currency manager then the data member can be valid
                    return false;
                }

                PropertyDescriptorCollection props = cm.GetItemProperties();
                if (this.dataMember.Length != 0 && props[this.dataMember] != null)
                {
                    // the data member is valid. Don't change it
                    return false;
                }

                return true;
            }

            public void Sort(DataGridViewColumn dataGridViewColumn, ListSortDirection direction)
            {
                Debug.Assert(dataGridViewColumn.IsDataBound && dataGridViewColumn.BoundColumnIndex != -1, "we need a bound column index to perform the sort");
                Debug.Assert(this.List is IBindingList, "you should have checked by now that we are bound to an IBindingList");
                ((IBindingList)this.List).ApplySort(this.props[dataGridViewColumn.BoundColumnIndex], direction);
            }

            private void UnWireEvents()
            {
                if (this.currencyManager != null)
                {
                    this.currencyManager.PositionChanged -= new EventHandler(currencyManager_PositionChanged);
                    this.currencyManager.ListChanged -= new ListChangedEventHandler(currencyManager_ListChanged);
                    this.dataConnectionState[DATACONNECTIONSTATE_interestedInRowEvents] = false;
                }
            }

            private void WireEvents()
            {
                if (this.currencyManager != null)
                {
                    this.currencyManager.PositionChanged += new EventHandler(currencyManager_PositionChanged);
                    this.currencyManager.ListChanged += new ListChangedEventHandler(currencyManager_ListChanged);
                    this.dataConnectionState[DATACONNECTIONSTATE_interestedInRowEvents] = true;
                }
            }
        }
    }
}
