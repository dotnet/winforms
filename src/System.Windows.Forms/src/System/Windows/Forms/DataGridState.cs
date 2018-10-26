// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.Text;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;
    using System.Security.Permissions;

    /// <include file='doc\DataGridState.uex' path='docs/doc[@for="DataGridState"]/*' />
    /// <devdoc>
    ///      Encapsulates the state of a DataGrid that changes when the
    ///      user navigates back and forth through ADO.NET data relations.
    /// </devdoc>
    internal sealed class DataGridState : ICloneable {
        // fields
        //
        public object DataSource = null;
        public string DataMember = null;
        public CurrencyManager ListManager = null;
        public DataGridRow[] DataGridRows    = new DataGridRow[0];
        public DataGrid DataGrid;
        public int DataGridRowsLength = 0;
        public GridColumnStylesCollection GridColumnStyles = null;

        public int           FirstVisibleRow = 0;
        public int           FirstVisibleCol = 0;

        public int           CurrentRow      = 0;
        public int           CurrentCol      = 0;

        public DataGridRow   LinkingRow      = null;
        AccessibleObject         parentRowAccessibleObject;

        public DataGridState() {
        }

        public DataGridState(DataGrid dataGrid) {
            PushState(dataGrid);
        }
        
        internal AccessibleObject ParentRowAccessibleObject {
            get {
                if (parentRowAccessibleObject == null) {
                    parentRowAccessibleObject = new DataGridStateParentRowAccessibleObject(this);
                }
                return parentRowAccessibleObject;
            }
        }

        // methods
        //

        public object Clone() {
            DataGridState dgs = new DataGridState();
            dgs.DataGridRows = DataGridRows;
            dgs.DataSource = DataSource;
            dgs.DataMember = DataMember;
            dgs.FirstVisibleRow = FirstVisibleRow;
            dgs.FirstVisibleCol = FirstVisibleCol;
            dgs.CurrentRow = CurrentRow;
            dgs.CurrentCol = CurrentCol;
            dgs.GridColumnStyles = GridColumnStyles;
            dgs.ListManager = ListManager;
            dgs.DataGrid = DataGrid;
            return dgs;
        }

        /// <include file='doc\DataGridState.uex' path='docs/doc[@for="DataGridState.PushState"]/*' />
        /// <devdoc>
        ///      Called by a DataGrid when it wishes to preserve its
        ///      transient state in the current DataGridState object.
        /// </devdoc>
        public void PushState(DataGrid dataGrid) {
            this.DataSource = dataGrid.DataSource;
            this.DataMember = dataGrid.DataMember;
            this.DataGrid = dataGrid;
            this.DataGridRows = dataGrid.DataGridRows;
            this.DataGridRowsLength = dataGrid.DataGridRowsLength;
            this.FirstVisibleRow = dataGrid.firstVisibleRow;
            this.FirstVisibleCol = dataGrid.firstVisibleCol;
            this.CurrentRow = dataGrid.currentRow;
            this.GridColumnStyles = new GridColumnStylesCollection(dataGrid.myGridTable);
            
            this.GridColumnStyles.Clear();
            foreach(DataGridColumnStyle style in dataGrid.myGridTable.GridColumnStyles) {
                this.GridColumnStyles.Add(style);
            }
            
            this.ListManager = dataGrid.ListManager;
            this.ListManager.ItemChanged += new ItemChangedEventHandler(DataSource_Changed);
            this.ListManager.MetaDataChanged += new EventHandler(DataSource_MetaDataChanged);
            this.CurrentCol = dataGrid.currentCol;
        }

        // this is needed so that the parent rows will remove notification from the list
        // when the datagridstate is no longer needed;
        public void RemoveChangeNotification() {
            this.ListManager.ItemChanged -= new ItemChangedEventHandler(DataSource_Changed);
            this.ListManager.MetaDataChanged -= new EventHandler(DataSource_MetaDataChanged);
        }

        /// <include file='doc\DataGridState.uex' path='docs/doc[@for="DataGridState.PullState"]/*' />
        /// <devdoc>
        ///      Called by a grid when it wishes to match its transient
        ///      state with the current DataGridState object.
        /// </devdoc>
        public void PullState(DataGrid dataGrid, bool createColumn) {
            // dataGrid.DataSource = DataSource;
            // dataGrid.DataMember = DataMember;
            dataGrid.Set_ListManager(DataSource, DataMember, true, createColumn);   // true for forcing new listManager,

            /*
            if (DataSource.Table.ParentRelations.Count > 0)
                dataGrid.PopulateColumns();
            */

            dataGrid.firstVisibleRow = FirstVisibleRow;
            dataGrid.firstVisibleCol = FirstVisibleCol;
            dataGrid.currentRow = CurrentRow;
            dataGrid.currentCol = CurrentCol;
            dataGrid.SetDataGridRows(DataGridRows, DataGridRowsLength);
        }

        private void DataSource_Changed(object sender, ItemChangedEventArgs e) {
            if (this.DataGrid != null && this.ListManager.Position == e.Index) {
                DataGrid.InvalidateParentRows();
                return;
            }

            if (this.DataGrid != null)
                DataGrid.ParentRowsDataChanged();
        }

        private void DataSource_MetaDataChanged(object sender, EventArgs e) {
            if (this.DataGrid != null)
                DataGrid.ParentRowsDataChanged();
        }


        [ComVisible(true)]
        internal class DataGridStateParentRowAccessibleObject : AccessibleObject {
            DataGridState owner = null;

            public DataGridStateParentRowAccessibleObject(DataGridState owner) : base() {
                Debug.Assert(owner != null, "DataGridRowAccessibleObject must have a valid owner DataGridRow");
                this.owner = owner;
            }

            public override Rectangle Bounds {
                get {
                    DataGridParentRows dataGridParentRows = ((DataGridParentRows.DataGridParentRowsAccessibleObject)this.Parent).Owner;
                    DataGrid g = owner.LinkingRow.DataGrid;
                    Rectangle r = dataGridParentRows.GetBoundsForDataGridStateAccesibility(owner);
                    r.Y += g.ParentRowsBounds.Y;
                    return g.RectangleToScreen(r);
                }
            }

            public override string Name {
                get {
                    return SR.AccDGParentRow;
                }
            }

            public override AccessibleObject Parent {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get {
                    return owner.LinkingRow.DataGrid.ParentRowsAccessibleObject;
                }
            }

            public override AccessibleRole Role {
                get {
                    return AccessibleRole.ListItem;
                }
            }

            public override string Value {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get {
                    StringBuilder sb = new StringBuilder();

                    CurrencyManager source = (CurrencyManager)owner.LinkingRow.DataGrid.BindingContext[owner.DataSource, owner.DataMember];

                    sb.Append(owner.ListManager.GetListName());
                    sb.Append(": ");

                    bool needComma = false;
                    foreach (DataGridColumnStyle col in owner.GridColumnStyles) {
                        if (needComma) {
                            sb.Append(", ");
                        }

                        string colName = col.HeaderText;
                        string cellValue = col.PropertyDescriptor.Converter.ConvertToString(col.PropertyDescriptor.GetValue(source.Current));
                        sb.Append(colName);
                        sb.Append(": ");
                        sb.Append(cellValue);
                        needComma = true;
                    }

                    return sb.ToString();
                }
            }

            /// <include file='doc\DataGridState.uex' path='docs/doc[@for="DataGridState.DataGridStateParentRowAccessibleObject.Navigate"]/*' />
            /// <devdoc>
            ///      Navigate to the next or previous grid entry.
            /// </devdoc>
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override AccessibleObject Navigate(AccessibleNavigation navdir) {
                DataGridParentRows.DataGridParentRowsAccessibleObject parentAcc = (DataGridParentRows.DataGridParentRowsAccessibleObject)Parent;

                switch (navdir) {
                    case AccessibleNavigation.Down:
                    case AccessibleNavigation.Right:
                    case AccessibleNavigation.Next:
                        return parentAcc.GetNext(this);
                    case AccessibleNavigation.Up:
                    case AccessibleNavigation.Left:
                    case AccessibleNavigation.Previous:
                        return parentAcc.GetPrev(this);
                }

                return null;

            }
        }
    }
}
