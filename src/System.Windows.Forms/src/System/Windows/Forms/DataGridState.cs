// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Encapsulates the state of a DataGrid that changes when the
    ///  user navigates back and forth through ADO.NET data relations.
    /// </summary>
    internal sealed class DataGridState : ICloneable
    {
        // fields
        //
        public object DataSource = null;
        public string DataMember = null;
        public CurrencyManager ListManager = null;
        public DataGridRow[] DataGridRows = Array.Empty<DataGridRow>();
        public DataGrid DataGrid;
        public int DataGridRowsLength = 0;
        public GridColumnStylesCollection GridColumnStyles = null;

        public int FirstVisibleRow = 0;
        public int FirstVisibleCol = 0;

        public int CurrentRow = 0;
        public int CurrentCol = 0;

        public DataGridRow LinkingRow = null;
        AccessibleObject parentRowAccessibleObject;

        public DataGridState()
        {
        }

        public DataGridState(DataGrid dataGrid)
        {
            PushState(dataGrid);
        }

        internal AccessibleObject ParentRowAccessibleObject
        {
            get
            {
                if (parentRowAccessibleObject == null)
                {
                    parentRowAccessibleObject = new DataGridStateParentRowAccessibleObject(this);
                }
                return parentRowAccessibleObject;
            }
        }

        // methods
        //

        public object Clone()
        {
            DataGridState dgs = new DataGridState
            {
                DataGridRows = DataGridRows,
                DataSource = DataSource,
                DataMember = DataMember,
                FirstVisibleRow = FirstVisibleRow,
                FirstVisibleCol = FirstVisibleCol,
                CurrentRow = CurrentRow,
                CurrentCol = CurrentCol,
                GridColumnStyles = GridColumnStyles,
                ListManager = ListManager,
                DataGrid = DataGrid
            };
            return dgs;
        }

        /// <summary>
        ///  Called by a DataGrid when it wishes to preserve its
        ///  transient state in the current DataGridState object.
        /// </summary>
        public void PushState(DataGrid dataGrid)
        {
            DataSource = dataGrid.DataSource;
            DataMember = dataGrid.DataMember;
            DataGrid = dataGrid;
            DataGridRows = dataGrid.DataGridRows;
            DataGridRowsLength = dataGrid.DataGridRowsLength;
            FirstVisibleRow = dataGrid.firstVisibleRow;
            FirstVisibleCol = dataGrid.firstVisibleCol;
            CurrentRow = dataGrid.currentRow;
            GridColumnStyles = new GridColumnStylesCollection(dataGrid.myGridTable);

            GridColumnStyles.Clear();
            foreach (DataGridColumnStyle style in dataGrid.myGridTable.GridColumnStyles)
            {
                GridColumnStyles.Add(style);
            }

            ListManager = dataGrid.ListManager;
            ListManager.ItemChanged += new ItemChangedEventHandler(DataSource_Changed);
            ListManager.MetaDataChanged += new EventHandler(DataSource_MetaDataChanged);
            CurrentCol = dataGrid.currentCol;
        }

        // this is needed so that the parent rows will remove notification from the list
        // when the datagridstate is no longer needed;
        public void RemoveChangeNotification()
        {
            ListManager.ItemChanged -= new ItemChangedEventHandler(DataSource_Changed);
            ListManager.MetaDataChanged -= new EventHandler(DataSource_MetaDataChanged);
        }

        /// <summary>
        ///  Called by a grid when it wishes to match its transient
        ///  state with the current DataGridState object.
        /// </summary>
        public void PullState(DataGrid dataGrid, bool createColumn)
        {
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

        private void DataSource_Changed(object sender, ItemChangedEventArgs e)
        {
            if (DataGrid != null && ListManager.Position == e.Index)
            {
                DataGrid.InvalidateParentRows();
                return;
            }

            if (DataGrid != null)
            {
                DataGrid.ParentRowsDataChanged();
            }
        }

        private void DataSource_MetaDataChanged(object sender, EventArgs e)
        {
            if (DataGrid != null)
            {
                DataGrid.ParentRowsDataChanged();
            }
        }

        [ComVisible(true)]
        internal class DataGridStateParentRowAccessibleObject : AccessibleObject
        {
            readonly DataGridState owner = null;

            public DataGridStateParentRowAccessibleObject(DataGridState owner) : base()
            {
                Debug.Assert(owner != null, "DataGridRowAccessibleObject must have a valid owner DataGridRow");
                this.owner = owner;
            }

            public override Rectangle Bounds
            {
                get
                {
                    DataGridParentRows dataGridParentRows = ((DataGridParentRows.DataGridParentRowsAccessibleObject)Parent).Owner;
                    DataGrid g = owner.LinkingRow.DataGrid;
                    Rectangle r = dataGridParentRows.GetBoundsForDataGridStateAccesibility(owner);
                    r.Y += g.ParentRowsBounds.Y;
                    return g.RectangleToScreen(r);
                }
            }

            public override string Name
            {
                get
                {
                    return SR.AccDGParentRow;
                }
            }

            public override AccessibleObject Parent
            {
                get
                {
                    return owner.LinkingRow.DataGrid.ParentRowsAccessibleObject;
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    return AccessibleRole.ListItem;
                }
            }

            public override string Value
            {
                get
                {
                    StringBuilder sb = new StringBuilder();

                    CurrencyManager source = (CurrencyManager)owner.LinkingRow.DataGrid.BindingContext[owner.DataSource, owner.DataMember];

                    sb.Append(owner.ListManager.GetListName());
                    sb.Append(": ");

                    bool needComma = false;
                    foreach (DataGridColumnStyle col in owner.GridColumnStyles)
                    {
                        if (needComma)
                        {
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

            /// <summary>
            ///  Navigate to the next or previous grid entry.
            /// </summary>
            public override AccessibleObject Navigate(AccessibleNavigation navdir)
            {
                DataGridParentRows.DataGridParentRowsAccessibleObject parentAcc = (DataGridParentRows.DataGridParentRowsAccessibleObject)Parent;

                switch (navdir)
                {
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
