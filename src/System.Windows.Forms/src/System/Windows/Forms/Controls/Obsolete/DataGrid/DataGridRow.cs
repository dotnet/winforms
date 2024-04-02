// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace System.Windows.Forms;

#nullable disable
[Obsolete(
    Obsoletions.DataGridRowMessage,
    error: false,
    DiagnosticId = Obsoletions.DataGridRowDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
internal abstract class DataGridRow : MarshalByRefObject
{
    protected DataGridTableStyle dgTable;
    protected const int XOffset = 3;
    protected const int YOffset = 2;

    public DataGridRow(DataGrid dataGrid, DataGridTableStyle dgTable, int rowNumber)
        => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public AccessibleObject AccessibleObject
    {
        get => throw new PlatformNotSupportedException();
    }

    protected virtual AccessibleObject CreateAccessibleObject()
        => new DataGridRowAccessibleObject(this);

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DataGrid DataGrid
    {
        get => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual int Height
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int RowNumber
    {
        get => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual bool Selected
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [ResourceExposure(ResourceScope.Machine)]
    [ResourceConsumption(ResourceScope.Machine)]
    protected Bitmap GetBitmap(string bitmapName)
        => throw new PlatformNotSupportedException();

    public virtual Rectangle GetCellBounds(int col)
        => throw new PlatformNotSupportedException();

    public virtual Rectangle GetNonScrollableArea()
        => throw new PlatformNotSupportedException();

    [ResourceExposure(ResourceScope.Machine)]
    [ResourceConsumption(ResourceScope.Machine)]
    protected Bitmap GetStarBitmap()
        => throw new PlatformNotSupportedException();

    [ResourceExposure(ResourceScope.Machine)]
    [ResourceConsumption(ResourceScope.Machine)]
    protected Bitmap GetPencilBitmap()
        => throw new PlatformNotSupportedException();

    [ResourceExposure(ResourceScope.Machine)]
    [ResourceConsumption(ResourceScope.Machine)]
    protected Bitmap GetErrorBitmap()
        => throw new PlatformNotSupportedException();

    [ResourceExposure(ResourceScope.Machine)]
    [ResourceConsumption(ResourceScope.Machine)]
    protected Bitmap GetLeftArrowBitmap()
        => throw new PlatformNotSupportedException();

    [ResourceExposure(ResourceScope.Machine)]
    [ResourceConsumption(ResourceScope.Machine)]
    protected Bitmap GetRightArrowBitmap()
        => throw new PlatformNotSupportedException();

    public virtual void InvalidateRow()
        => throw new PlatformNotSupportedException();

    public virtual void InvalidateRowRect(Rectangle r)
        => throw new PlatformNotSupportedException();

    public virtual void OnEdit()
        => throw new PlatformNotSupportedException();

    public virtual bool OnKeyPress(Keys keyData)
        => throw new PlatformNotSupportedException();

    public virtual bool OnMouseDown(int x, int y, Rectangle rowHeaders)
        => throw new PlatformNotSupportedException();

    public virtual bool OnMouseDown(int x, int y, Rectangle rowHeaders, bool alignToRight)
        => throw new PlatformNotSupportedException();

    public virtual bool OnMouseMove(int x, int y, Rectangle rowHeaders)
        => throw new PlatformNotSupportedException();

    public virtual bool OnMouseMove(int x, int y, Rectangle rowHeaders, bool alignToRight)
        => throw new PlatformNotSupportedException();

    public virtual void OnMouseLeft(Rectangle rowHeaders, bool alignToRight)
        => throw new PlatformNotSupportedException();

    public virtual void OnMouseLeft()
        => throw new PlatformNotSupportedException();

    public virtual void OnRowEnter()
        => throw new PlatformNotSupportedException();

    public virtual void OnRowLeave()
        => throw new PlatformNotSupportedException();

    public abstract int Paint(Graphics g,
        Rectangle dataBounds,
        Rectangle rowBounds,
        int firstVisibleColumn,
        int numVisibleColumns);

    public abstract int Paint(Graphics g,
        Rectangle dataBounds,
        Rectangle rowBounds,
        int firstVisibleColumn,
        int numVisibleColumns,
        bool alignToRight);

    protected virtual void PaintBottomBorder(Graphics g, Rectangle bounds, int dataWidth)
        => throw new PlatformNotSupportedException();

    protected virtual void PaintBottomBorder(Graphics g, Rectangle bounds, int dataWidth, int borderWidth, bool alignToRight)
    {
    }

    public virtual int PaintData(Graphics g,
        Rectangle bounds,
        int firstVisibleColumn,
        int columnCount)
        => throw new PlatformNotSupportedException();

    public virtual int PaintData(Graphics g,
        Rectangle bounds,
        int firstVisibleColumn,
        int columnCount,
        bool alignToRight)
        => throw new PlatformNotSupportedException();

    protected virtual void PaintCellContents(Graphics g,
        Rectangle cellBounds,
        DataGridColumnStyle column,
        Brush backBr,
        Brush foreBrush)
        => throw new PlatformNotSupportedException();

    protected virtual void PaintCellContents(Graphics g,
        Rectangle cellBounds,
        DataGridColumnStyle column,
        Brush backBr,
        Brush foreBrush,
        bool alignToRight)
        => throw new PlatformNotSupportedException();

    protected Rectangle PaintIcon(Graphics g,
        Rectangle visualBounds,
        bool paintIcon,
        bool alignToRight,
        Bitmap bmp)
        => throw new PlatformNotSupportedException();

    protected Rectangle PaintIcon(Graphics g,
        Rectangle visualBounds,
        bool paintIcon,
        bool alignToRight,
        Bitmap bmp,
        Brush backBrush)
        => throw new PlatformNotSupportedException();

    public virtual void PaintHeader(Graphics g, Rectangle visualBounds)
        => throw new PlatformNotSupportedException();

    public virtual void PaintHeader(Graphics g, Rectangle visualBounds, bool alignToRight)
        => throw new PlatformNotSupportedException();

    public virtual void PaintHeader(Graphics g, Rectangle visualBounds, bool alignToRight, bool rowIsDirty)
        => throw new PlatformNotSupportedException();

    protected Brush GetBackBrush()
        => throw new PlatformNotSupportedException();

    protected Brush BackBrushForDataPaint(ref DataGridCell current, DataGridColumnStyle gridColumn, int column)
        => throw new PlatformNotSupportedException();

    protected Brush ForeBrushForDataPaint(ref DataGridCell current, DataGridColumnStyle gridColumn, int column)
        => throw new PlatformNotSupportedException();

    [ComVisible(true)]
    [Obsolete(
    Obsoletions.DataGridRowAccessibleObjectMessage,
    error: false,
    DiagnosticId = Obsoletions.DataGridRowAccessibleObjectDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
    internal class DataGridRowAccessibleObject : AccessibleObject
    {
        public DataGridRowAccessibleObject(DataGridRow owner) : base()
            => throw new PlatformNotSupportedException();

        protected virtual void AddChildAccessibleObjects(IList children)
            => throw new PlatformNotSupportedException();

        protected virtual AccessibleObject CreateCellAccessibleObject(int column)
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

        protected DataGridRow Owner
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
        public override AccessibleStates State
        {
            get => throw new PlatformNotSupportedException();
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string Value
        {
            get => throw new PlatformNotSupportedException();
        }

        public override AccessibleObject GetChild(int index)
            => throw new PlatformNotSupportedException();

        public override int GetChildCount()
            => throw new PlatformNotSupportedException();

        public override AccessibleObject GetFocused()
            => throw new PlatformNotSupportedException();

        public override AccessibleObject Navigate(AccessibleNavigation navdir)
            => throw new PlatformNotSupportedException();

        public override void Select(AccessibleSelection flags)
            => throw new PlatformNotSupportedException();
    }

    [ComVisible(true)]
    [Obsolete(
    Obsoletions.DataGridCellAccessibleObjectMessage,
    error: false,
    DiagnosticId = Obsoletions.DataGridCellAccessibleObjectDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
    internal class DataGridCellAccessibleObject : AccessibleObject
    {
        public DataGridCellAccessibleObject(DataGridRow owner, int column) : base()
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

        protected DataGrid DataGrid
        {
            get => throw new PlatformNotSupportedException();
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string DefaultAction
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

        public override void DoDefaultAction()
            => throw new PlatformNotSupportedException();

        public override AccessibleObject GetFocused()
            => throw new PlatformNotSupportedException();

        public override AccessibleObject Navigate(AccessibleNavigation navdir)
            => throw new PlatformNotSupportedException();

        public override void Select(AccessibleSelection flags)
            => throw new PlatformNotSupportedException();
    }
}
