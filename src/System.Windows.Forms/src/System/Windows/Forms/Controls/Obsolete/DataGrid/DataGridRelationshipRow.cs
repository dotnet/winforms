// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

#nullable disable
[Obsolete(
    Obsoletions.DataGridRelationshipRowMessage,
    error: false,
    DiagnosticId = Obsoletions.DataGridRelationshipRowDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
internal class DataGridRelationshipRow : DataGridRow
{
    public DataGridRelationshipRow(DataGrid dataGrid, DataGridTableStyle dgTable, int rowNumber)
    : base(dataGrid, dgTable, rowNumber)
        => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual bool Expanded
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int Height
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    public override Rectangle GetCellBounds(int col)
        => throw new PlatformNotSupportedException();

    public override Rectangle GetNonScrollableArea()
        => throw new PlatformNotSupportedException();

    public override bool OnMouseDown(int x, int y, Rectangle rowHeaders, bool alignToRight)
        => throw new PlatformNotSupportedException();

    public override bool OnMouseMove(int x, int y, Rectangle rowHeaders, bool alignToRight)
        => throw new PlatformNotSupportedException();

    public override void OnMouseLeft(Rectangle rowHeaders, bool alignToRight)
        => throw new PlatformNotSupportedException();

    public override void OnMouseLeft()
        => throw new PlatformNotSupportedException();

    public override bool OnKeyPress(Keys keyData)
        => throw new PlatformNotSupportedException();

    public override int Paint(Graphics g,
        Rectangle bounds,
        Rectangle trueRowBounds,
        int firstVisibleColumn,
        int numVisibleColumns)
        => throw new PlatformNotSupportedException();

    public override int Paint(Graphics g,
        Rectangle bounds,
        Rectangle trueRowBounds,
        int firstVisibleColumn,
        int numVisibleColumns,
        bool alignToRight)
        => throw new PlatformNotSupportedException();

    protected override void PaintCellContents(Graphics g,
        Rectangle cellBounds,
        DataGridColumnStyle column,
        Brush backBr,
        Brush foreBrush,
        bool alignToRight)
    {
    }

    public override void PaintHeader(Graphics g, Rectangle bounds, bool alignToRight, bool isDirty)
        => throw new PlatformNotSupportedException();

    public void PaintHeaderInside(Graphics g, Rectangle bounds, Brush backBr, bool alignToRight, bool isDirty)
        => throw new PlatformNotSupportedException();

    [Obsolete(
        Obsoletions.DataGridRelationshipRowAccessibleObjectMessage,
        error: false,
        DiagnosticId = Obsoletions.DataGridRelationshipRowAccessibleObjectDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat)]
    internal class DataGridRelationshipRowAccessibleObject : DataGridRowAccessibleObject
    {
        public DataGridRelationshipRowAccessibleObject(DataGridRow owner) : base(owner)
            => throw new PlatformNotSupportedException();

        protected override void AddChildAccessibleObjects(IList children)
            => base.AddChildAccessibleObjects(children);

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string DefaultAction
        {
            get => throw new PlatformNotSupportedException();
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override AccessibleStates State
        {
            get => throw new PlatformNotSupportedException();
        }

        public override void DoDefaultAction()
            => throw new PlatformNotSupportedException();

        public override AccessibleObject GetFocused()
            => throw new PlatformNotSupportedException();
    }

    [Obsolete(
        Obsoletions.DataGridRelationshipAccessibleObjectMessage,
        error: false,
        DiagnosticId = Obsoletions.DataGridRelationshipAccessibleObjectDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat)]
    internal class DataGridRelationshipAccessibleObject : AccessibleObject
    {
        public DataGridRelationshipAccessibleObject(DataGridRelationshipRow owner, int relationship) : base()
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

        protected DataGridRelationshipRow Owner
        {
            get;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override AccessibleObject Parent
        {
            get => throw new PlatformNotSupportedException();
        }

        protected DataGrid DataGrid
        {
            get;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override AccessibleRole Role
        {
            get => throw new PlatformNotSupportedException();
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override AccessibleStates State
        {
            get => throw new PlatformNotSupportedException();
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string Value
        {
            get => throw new PlatformNotSupportedException();
            set => throw new PlatformNotSupportedException();
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string DefaultAction
        {
            get => throw new PlatformNotSupportedException();
        }

        public override void DoDefaultAction()
            => throw new PlatformNotSupportedException();

        public override AccessibleObject Navigate(AccessibleNavigation navdir)
            => throw new PlatformNotSupportedException();

        public override void Select(AccessibleSelection flags)
            => throw new PlatformNotSupportedException();
    }
}
