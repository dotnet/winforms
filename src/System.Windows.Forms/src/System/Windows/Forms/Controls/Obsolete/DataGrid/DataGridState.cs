// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

#nullable disable
[Obsolete(
    Obsoletions.DataGridStateMessage,
    error: false,
    DiagnosticId = Obsoletions.DataGridStateDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
internal sealed class DataGridState : ICloneable
{
    public object DataSource;
    public string DataMember;
    public CurrencyManager ListManager;
    public DataGridRow[] DataGridRows = [];
    public DataGrid DataGrid;
    public int DataGridRowsLength;
    public GridColumnStylesCollection GridColumnStyles;
    public int FirstVisibleRow;
    public int FirstVisibleCol;
    public int CurrentRow;
    public int CurrentCol;
    public DataGridRow LinkingRow;

    public DataGridState()
        => throw new PlatformNotSupportedException();

    public DataGridState(DataGrid dataGrid)
        => throw new PlatformNotSupportedException();

    public object Clone()
        => throw new PlatformNotSupportedException();

    public void PushState(DataGrid dataGrid)
        => throw new PlatformNotSupportedException();

    public void RemoveChangeNotification()
        => throw new PlatformNotSupportedException();

    public void PullState(DataGrid dataGrid, bool createColumn)
        => throw new PlatformNotSupportedException();

    [ComVisible(true)]
    [Obsolete(
    Obsoletions.DataGridStateParentRowAccessibleObjectMessage,
    error: false,
    DiagnosticId = Obsoletions.DataGridStateParentRowAccessibleObjectDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
    internal class DataGridStateParentRowAccessibleObject : AccessibleObject
    {
        public DataGridStateParentRowAccessibleObject(DataGridState owner) : base()
            => throw new PlatformNotSupportedException();

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Rectangle Bounds
        {
            get => throw new PlatformNotSupportedException();
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string Name
        {
            get => throw new PlatformNotSupportedException();
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override AccessibleObject Parent
        {
            get => throw new PlatformNotSupportedException();
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override AccessibleRole Role
        {
            get => throw new PlatformNotSupportedException();
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string Value
        {
            get => throw new PlatformNotSupportedException();
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override AccessibleObject Navigate(AccessibleNavigation navdir)
            => throw new PlatformNotSupportedException();
    }
}
