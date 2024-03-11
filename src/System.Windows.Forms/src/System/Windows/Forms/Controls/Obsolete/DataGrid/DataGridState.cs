// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

#nullable disable
[Obsolete("DataGridState is obsolete. Use the DataGrid control and the System.Windows.Forms.DataGridView control instead.")]
internal sealed class DataGridState : ICloneable
{
    public object DataSource;
    public string DataMember;
    public CurrencyManager ListManager;
    public DataGridRow[] DataGridRows = Array.Empty<DataGridRow>();
    public DataGrid DataGrid;
    public int DataGridRowsLength;
    public GridColumnStylesCollection GridColumnStyles;

    public int FirstVisibleRow;
    public int FirstVisibleCol;

    public int CurrentRow;
    public int CurrentCol;

    public DataGridRow LinkingRow;
    private AccessibleObject parentRowAccessibleObject;

    public DataGridState()
    {
        throw new PlatformNotSupportedException();
    }

    public DataGridState(DataGrid dataGrid)
    {
        throw new PlatformNotSupportedException();
    }

    internal AccessibleObject ParentRowAccessibleObject
    {
        get
        {
            if (parentRowAccessibleObject is null)
            {
                parentRowAccessibleObject = new DataGridStateParentRowAccessibleObject(this);
            }

            return parentRowAccessibleObject;
        }
    }

    public object Clone()
    {
        throw new PlatformNotSupportedException();
    }

    public void PushState(DataGrid dataGrid)
    {
        throw new PlatformNotSupportedException();
    }

    public void RemoveChangeNotification()
    {
        throw new PlatformNotSupportedException();
    }

    public void PullState(DataGrid dataGrid, bool createColumn)
    {
        throw new PlatformNotSupportedException();
    }

    private void DataSource_Changed(object sender, ItemChangedEventArgs e)
    {
        if (DataGrid is not null && ListManager.Position == e.Index)
        {
            DataGrid.InvalidateParentRows();
            return;
        }

        if (DataGrid is not null)
            DataGrid.ParentRowsDataChanged();
    }

    private void DataSource_MetaDataChanged(object sender, EventArgs e)
    {
        if (DataGrid is not null)
        {
            DataGrid.ParentRowsDataChanged();
        }
    }

    [ComVisible(true)]
    [Obsolete("DataGridStateParentRowAccessibleObject has been deprecated.")]
    internal class DataGridStateParentRowAccessibleObject : AccessibleObject
    {
        public DataGridStateParentRowAccessibleObject(DataGridState owner) : base()
        {
            throw new PlatformNotSupportedException();
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Rectangle Bounds
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override string Name
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override AccessibleObject Parent
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override AccessibleRole Role
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override string Value
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override AccessibleObject Navigate(AccessibleNavigation navdir)
        {
            throw new PlatformNotSupportedException();
        }
    }
}
