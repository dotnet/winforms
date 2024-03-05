// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Security.Permissions;

namespace System.Windows.Forms;

#pragma warning disable RS0016
#nullable disable
[Obsolete("DataGrid has been deprecated. Use DataGridView instead.")]
public class DataGrid : Control, ISupportInitialize, IDataGridEditingService
{
#if DEBUG
    internal TraceSwitch DataGridAcc = new TraceSwitch("DataGridAcc", "Trace Windows Forms DataGrid Accessibility");
#else
            internal TraceSwitch DataGridAcc = null;
#endif

    private const int GRIDSTATE_trackColResize = 0x00000008;
    private const int GRIDSTATE_trackRowResize = 0x00000010;
    private const int GRIDSTATE_isLedgerStyle = 0x00000020;
    private const int GRIDSTATE_listHasErrors = 0x00000080;
    private const int GRIDSTATE_dragging = 0x00000100;
    private const int GRIDSTATE_inListAddNew = 0x00000200;
    private const int GRIDSTATE_inDeleteRow = 0x00000400;
    private const int GRIDSTATE_canFocus = 0x00000800;
    private const int GRIDSTATE_isNavigating = 0x00004000;
    private const int GRIDSTATE_isEditing = 0x00008000;
    private const int GRIDSTATE_editControlChanging = 0x00010000;
    private const int GRIDSTATE_isScrolling = 0x00020000;
    private const int GRIDSTATE_overCaption = 0x00040000;
    private const int GRIDSTATE_childLinkFocused = 0x00080000;
    private const int GRIDSTATE_inAddNewRow = 0x00100000;
    private const int GRIDSTATE_inSetListManager = 0x00200000;
    private const int GRIDSTATE_metaDataChanged = 0x00400000;
    private const int GRIDSTATE_exceptionInPaint = 0x00800000;
    private const int GRIDSTATE_layoutSuspended = 0x01000000;

    // PERF: take all the bools and put them into a state variable
    private Collections.Specialized.BitVector32 gridState;                  // see GRIDSTATE_ consts above

    private const int errorRowBitmapWidth = 15;

    private const DataGridParentRowsLabelStyle defaultParentRowsLabelStyle = DataGridParentRowsLabelStyle.Both;
    private const bool defaultCaptionVisible = true;

    private const bool defaultParentRowsVisible = true;

    private DataGridTableStyle defaultTableStyle = new DataGridTableStyle(true);

    // private bool allowSorting = true;

    private SolidBrush alternatingBackBrush = DefaultAlternatingBackBrush;

    // private bool columnHeadersVisible = true;

    private SolidBrush gridLineBrush = DefaultGridLineBrush;
    private SolidBrush headerBackBrush = DefaultHeaderBackBrush;

    private Font headerFont; // this is ambient property to Font value

    private SolidBrush headerForeBrush = DefaultHeaderForeBrush;
    private Pen headerForePen = DefaultHeaderForePen;

    private SolidBrush linkBrush = DefaultLinkBrush;
    private static int defaultFontHeight = DefaultFont.Height;
    private int prefferedRowHeight = defaultFontHeight + 3;
    private int minRowHeaderWidth;

    private SolidBrush selectionBackBrush = DefaultSelectionBackBrush;
    private SolidBrush selectionForeBrush = DefaultSelectionForeBrush;

    // parent rows
    private DataGridParentRows parentRows;
    // Set_ListManager uses the originalState to determine
    // if the grid should disconnect from all the MetaDataChangedEvents
    // keep "originalState is not null" when navigating back and forth in the grid
    // and use Add/RemoveMetaDataChanged methods.
    private DataGridState originalState;

    // ui state
    // Don't use dataGridRows, use the accessor!!!
    private DataGridRow[] dataGridRows = [];
    private int dataGridRowsLength;

    // for toolTip
    private int toolTipId;
    private DataGridToolTip toolTipProvider;

    private DataGridAddNewRow addNewRow;
    private LayoutData layout = new LayoutData();
    private RECT[] cachedScrollableRegion;

    // header namespace goo

    // these are actually get/set by ColumnBehavior
    internal bool allowColumnResize = true;

    internal bool allowRowResize = true;

    internal DataGridParentRowsLabelStyle parentRowsLabels = defaultParentRowsLabelStyle;

    // information for col/row resizing
    // private bool       trackColResize         = false;
    private int trackColAnchor;
    private int trackColumn;
    // private bool       trackRowResize         = false;
    private int trackRowAnchor;
    private int trackRow;
    private PropertyDescriptor trackColumnHeader;
    private MouseEventArgs lastSplitBar;

    // private bool isLedgerStyle = true;
    // private bool isFlatMode    = false;
    private Font linkFont;

    private SolidBrush backBrush = DefaultBackBrush;
    private SolidBrush foreBrush = DefaultForeBrush;
    private SolidBrush backgroundBrush = DefaultBackgroundBrush;

    // font cacheing info
    private int fontHeight = -1;
    private int linkFontHeight = -1;
    private int captionFontHeight = -1;
    private int headerFontHeight = -1;

    // the preffered height of the row.

    // if the list has items with errors

    // private bool listHasErrors = false;

    // caption
    private DataGridCaption caption;

    // data binding
    //
    private object dataSource;
    private string dataMember = "";
    private CurrencyManager listManager;

    // currently focused control
    // we want to unparent it either when rebinding the grid or when the grid is disposed
    private Control toBeDisposedEditingControl;

    // persistent data state
    //
    internal GridTableStylesCollection dataGridTables;
    // SET myGridTable in SetDataGridTable ONLY
    internal DataGridTableStyle myGridTable;
    internal bool checkHierarchy = true;
    internal bool inInit;

    // Selection
    internal int currentRow;
    internal int currentCol;
    private int numSelectedRows;
    private int lastRowSelected = -1;

    // Policy
    // private bool   readOnlyMode = false;
    private Policy policy = new Policy();
    private DataGridColumnStyle editColumn;
    private DataGridRow editRow;

    // scrolling
    private ScrollBar horizScrollBar = new HScrollBar();
    private ScrollBar vertScrollBar = new VScrollBar();

    // the sum of the widths of the columns preceding the firstVisibleColumn
    private int horizontalOffset;

    // the number of pixels of the firstVisibleColumn which are not visible
    //
    private int negOffset;

    private int wheelDelta;
    // private bool      isScrolling = false;

    // Visibility
    //
    internal int firstVisibleRow;
    internal int firstVisibleCol;
    private int numVisibleRows;
    // the number of columns which are visible
    private int numVisibleCols;
    private int numTotallyVisibleRows;
    // lastTotallyVisibleCol == -1 means that the data grid does not show any column in its entirety
    private int lastTotallyVisibleCol;

    // mouse move hot-tracking
    //
    private int oldRow = -1;
    private static readonly object EVENT_CURRENTCELLCHANGED = new object();
    // private static readonly object EVENT_COLUMNRESIZE = new object();
    // private static readonly object EVENT_LINKCLICKED = new object();
    private static readonly object EVENT_NODECLICKED = new object();
    // private static readonly object EVENT_ROWRESIZE = new object();
    private static readonly object EVENT_SCROLL = new object();
    private static readonly object EVENT_BACKBUTTONCLICK = new object();
    private static readonly object EVENT_DOWNBUTTONCLICK = new object();

    private EventHandler onRowHeaderClick;

    public DataGrid() : base()
    {
        parentRows = new DataGridParentRows(this);
        caption = new DataGridCaption(this);
        throw new PlatformNotSupportedException();
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public bool AllowSorting
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Color AlternatingBackColor
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    public void ResetAlternatingBackColor()
    {
        throw new PlatformNotSupportedException();
    }

    protected virtual bool ShouldSerializeAlternatingBackColor()
    {
        return !AlternatingBackBrush.Equals(DefaultAlternatingBackBrush);
    }

    internal Brush AlternatingBackBrush
    {
        get
        {
            return alternatingBackBrush;
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override Color BackColor
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    public override void ResetBackColor()
    {
        throw new PlatformNotSupportedException();
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override Color ForeColor
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    public override void ResetForeColor()
    {
        throw new PlatformNotSupportedException();
    }

    internal SolidBrush BackBrush
    {
        get
        {
            return backBrush;
        }
    }

    internal SolidBrush ForeBrush
    {
        get
        {
            return foreBrush;
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public BorderStyle BorderStyle
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    private static readonly object EVENT_BORDERSTYLECHANGED = new object();

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler BorderStyleChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    private int BorderWidth
    {
        get
        {
            if (BorderStyle == BorderStyle.Fixed3D)
            {
                return SystemInformation.Border3DSize.Width;
            }
            else if (BorderStyle == BorderStyle.FixedSingle)
            {
                return 2;
            }
            else
            {
                return 0;
            }
        }
    }

    protected override Size DefaultSize
    {
        get
        {
            return new Size(130, 80);
        }
    }

    private static SolidBrush DefaultSelectionBackBrush
    {
        get
        {
            return (SolidBrush)SystemBrushes.ActiveCaption;
        }
    }

    private static SolidBrush DefaultSelectionForeBrush
    {
        get
        {
            return (SolidBrush)SystemBrushes.ActiveCaptionText;
        }
    }

    internal static SolidBrush DefaultBackBrush
    {
        get
        {
            return (SolidBrush)SystemBrushes.Window;
        }
    }

    internal static SolidBrush DefaultForeBrush
    {
        get
        {
            return (SolidBrush)SystemBrushes.WindowText;
        }
    }

    private static SolidBrush DefaultBackgroundBrush
    {
        get
        {
            return (SolidBrush)SystemBrushes.AppWorkspace;
        }
    }

    internal static SolidBrush DefaultParentRowsForeBrush
    {
        get
        {
            return (SolidBrush)SystemBrushes.WindowText;
        }
    }

    internal static SolidBrush DefaultParentRowsBackBrush
    {
        get
        {
            return (SolidBrush)SystemBrushes.Control;
        }
    }

    internal static SolidBrush DefaultAlternatingBackBrush
    {
        get
        {
            return (SolidBrush)SystemBrushes.Window;
        }
    }

    private static SolidBrush DefaultGridLineBrush
    {
        get
        {
            return (SolidBrush)SystemBrushes.Control;
        }
    }

    private static SolidBrush DefaultHeaderBackBrush
    {
        get
        {
            return (SolidBrush)SystemBrushes.Control;
        }
    }

    private static SolidBrush DefaultHeaderForeBrush
    {
        get
        {
            return (SolidBrush)SystemBrushes.ControlText;
        }
    }

    private static Pen DefaultHeaderForePen
    {
        get
        {
            return new Pen(SystemColors.ControlText);
        }
    }

    private static SolidBrush DefaultLinkBrush
    {
        get
        {
            return (SolidBrush)SystemBrushes.HotTrack;
        }
    }

    private bool ListHasErrors
    {
        get
        {
            return gridState[GRIDSTATE_listHasErrors];
        }
        set
        {
            if (ListHasErrors != value)
            {
                gridState[GRIDSTATE_listHasErrors] = value;
                ComputeMinimumRowHeaderWidth();
                if (!layout.RowHeadersVisible)
                    return;
                if (value)
                {
                    if (myGridTable.IsDefault)
                        RowHeaderWidth += errorRowBitmapWidth;
                    else
                        myGridTable.RowHeaderWidth += errorRowBitmapWidth;
                }
                else
                {
                    if (myGridTable.IsDefault)
                        RowHeaderWidth -= errorRowBitmapWidth;
                    else
                        myGridTable.RowHeaderWidth -= errorRowBitmapWidth;
                }
            }
        }
    }

    private bool Bound
    {
        get
        {
            return !(listManager is null || myGridTable is null);
        }
    }

    internal DataGridCaption Caption
    {
        get
        {
            return caption;
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Color CaptionBackColor
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    private void ResetCaptionBackColor()
    {
        Caption.ResetBackColor();
    }

    protected virtual bool ShouldSerializeCaptionBackColor()
    {
        return Caption.ShouldSerializeBackColor();
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Color CaptionForeColor
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    private void ResetCaptionForeColor()
    {
        Caption.ResetForeColor();
    }

    protected virtual bool ShouldSerializeCaptionForeColor()
    {
        return Caption.ShouldSerializeForeColor();
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Font CaptionFont
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    private bool ShouldSerializeCaptionFont()
    {
        return Caption.ShouldSerializeFont();
    }

    private void ResetCaptionFont()
    {
        Caption.ResetFont();
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public string CaptionText
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public bool CaptionVisible
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    private static readonly object EVENT_CAPTIONVISIBLECHANGED = new object();

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler CaptionVisibleChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public DataGridCell CurrentCell
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    internal int CurrentCellAccIndex
    {
        get
        {
            int currentCellAccIndex = 0;
            currentCellAccIndex++;                                                 // ParentRowsAccessibleObject
            currentCellAccIndex += myGridTable.GridColumnStyles.Count;         // ColumnHeaderAccessibleObject
            currentCellAccIndex += DataGridRows.Length;                        // DataGridRowAccessibleObject
            if (horizScrollBar.Visible)                                        // Horizontal Scroll Bar Accessible Object
                currentCellAccIndex++;
            if (vertScrollBar.Visible)                                         // Vertical Scroll Bar Accessible Object
                currentCellAccIndex++;
            currentCellAccIndex += (currentRow * myGridTable.GridColumnStyles.Count) + currentCol;
            return currentCellAccIndex;
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler CurrentCellChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    private int CurrentColumn
    {
        get
        {
            return CurrentCell.ColumnNumber;
        }
        set
        {
            CurrentCell = new DataGridCell(currentRow, value);
        }
    }

    private int CurrentRow
    {
        get
        {
            return CurrentCell.RowNumber;
        }
        set
        {
            CurrentCell = new DataGridCell(value, currentCol);
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Color SelectionBackColor
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    internal SolidBrush SelectionBackBrush
    {
        get
        {
            return selectionBackBrush;
        }
    }

    internal SolidBrush SelectionForeBrush
    {
        get
        {
            return selectionForeBrush;
        }
    }

    protected bool ShouldSerializeSelectionBackColor()
    {
        return !DefaultSelectionBackBrush.Equals(selectionBackBrush);
    }

    public void ResetSelectionBackColor()
    {
        throw new PlatformNotSupportedException();
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Color SelectionForeColor
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    protected virtual bool ShouldSerializeSelectionForeColor()
    {
        return !SelectionForeBrush.Equals(DefaultSelectionForeBrush);
    }

    public void ResetSelectionForeColor()
    {
        throw new PlatformNotSupportedException();
    }

    internal override bool ShouldSerializeForeColor()
    {
        return !DefaultForeBrush.Color.Equals(this.ForeColor);
    }

    internal override bool ShouldSerializeBackColor()
    {
        return !DefaultBackBrush.Color.Equals(this.BackColor);
    }

    // Don't use dataGridRows, use the accessor!!!
    internal DataGridRow[] DataGridRows
    {
        get
        {
            if (dataGridRows is null)
                CreateDataGridRows();
            return dataGridRows;
        }
    }

    // ToolTipping
    internal DataGridToolTip ToolTipProvider
    {
        get
        {
            return toolTipProvider;
        }
    }

    internal int ToolTipId
    {
        get
        {
            return toolTipId;
        }
        set
        {
            toolTipId = value;
        }
    }

    private void ResetToolTip()
    {
        // remove all the tool tips which are stored
        for (int i = 0; i < ToolTipId; i++)
        {
            DataGridToolTip.RemoveToolTip(new IntPtr(i));
        }

        if (!parentRows.IsEmpty())
        {
            bool alignRight = isRightToLeft();
            int detailsButtonWidth = DataGridCaption.GetDetailsButtonWidth();
            Rectangle backButton = DataGridCaption.GetBackButtonRect(layout.Caption, alignRight, detailsButtonWidth);
            Rectangle detailsButton = DataGridCaption.GetDetailsButtonRect(layout.Caption, alignRight);

            // mirror the buttons wrt RTL property
            backButton.X = MirrorRectangle(backButton, layout.Inside, isRightToLeft());
            detailsButton.X = MirrorRectangle(detailsButton, layout.Inside, isRightToLeft());

            DataGridToolTip.AddToolTip("SR.GetString(SR.DataGridCaptionBackButtonToolTip)", new IntPtr(0), backButton);
            DataGridToolTip.AddToolTip("SR.GetString(SR.DataGridCaptionDetailsButtonToolTip)", new IntPtr(1), detailsButton);
            ToolTipId = 2;
        }
        else
        {
            ToolTipId = 0;
        }
    }

    private void CreateDataGridRows()
    {
        CurrencyManager listManager = ListManager;
        DataGridTableStyle dgt = myGridTable;
        InitializeColumnWidths();

        if (listManager is null)
        {
            SetDataGridRows(Array.Empty<DataGridRow>(), 0);
            return;
        }

        int nDataGridRows = listManager.Count;
        if (policy.AllowAdd)
            nDataGridRows++;

        DataGridRow[] rows = new DataGridRow[nDataGridRows];
        for (int r = 0; r < listManager.Count; r++)
        {
            rows[r] = new DataGridRelationshipRow(this, dgt, r);
        }

        if (policy.AllowAdd)
        {
            addNewRow = new DataGridAddNewRow(this, dgt, nDataGridRows - 1);
            rows[nDataGridRows - 1] = addNewRow;
        }
        else
        {
            addNewRow = null;
        }

        SetDataGridRows(rows, nDataGridRows);
    }

    private void RecreateDataGridRows()
    {
        int nDataGridRows = 0;
        CurrencyManager listManager = ListManager;

        if (listManager is not null)
        {
            nDataGridRows = listManager.Count;
            if (policy.AllowAdd)
            {
                nDataGridRows++;
            }
        }

        SetDataGridRows(null, nDataGridRows);
    }

    /// <devdoc>
    ///      Sets the array of DataGridRow objects used for
    ///      all row-related logic in the DataGrid.
    /// </devdoc>
    internal void SetDataGridRows(DataGridRow[] newRows, int newRowsLength)
    {
        dataGridRows = newRows;
        dataGridRowsLength = newRowsLength;

        // update the vertical scroll bar
        vertScrollBar.Maximum = Math.Max(0, DataGridRowsLength - 1);
        if (firstVisibleRow > newRowsLength)
        {
            vertScrollBar.Value = 0;
            firstVisibleRow = 0;
        }

        ResetUIState();
#if DEBUG
        // sanity check: all the rows should have the same
        // dataGridTable
        if (newRows is not null && newRowsLength > 0)
        {
            DataGridTableStyle dgTable = newRows[0].DataGridTableStyle;
            for (int i = 0; i < newRowsLength; i++)
            {
                Debug.Assert(dgTable == newRows[i].DataGridTableStyle, "how can two rows have different tableStyles?");
            }
        }
#endif // DEBUG
        Debug.WriteLineIf(CompModSwitches.DataGridCursor.TraceVerbose, "DataGridCursor: There are now " + DataGridRowsLength.ToString(CultureInfo.InvariantCulture) + " rows.");
    }

    internal int DataGridRowsLength
    {
        get
        {
            return dataGridRowsLength;
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public object DataSource
    {
        get
        {
            throw new PlatformNotSupportedException();
        }

        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    private static readonly object EVENT_DATASOURCECHANGED = new object();

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler DataSourceChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public string DataMember
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    public void SetDataBinding(object dataSource, string dataMember)
    {
        throw new PlatformNotSupportedException();
    }

    internal protected CurrencyManager ListManager
    {
        get
        {
            if (listManager is null && BindingContext is not null && DataSource is not null)
                return (CurrencyManager)BindingContext[DataSource, DataMember];
            else
                return listManager;
        }
        set
        {
            throw new NotSupportedException("SR.GetString(SR.DataGridSetListManager)");
        }
    }

    internal void Set_ListManager(object newDataSource, string newDataMember, bool force)
    {
        Set_ListManager(newDataSource, newDataMember, force, true);        // true for forcing column creation
    }

    //
    // prerequisite: the dataMember and the dataSource should be set to the new values
    //
    // will do the following:
    // call EndEdit on the current listManager, will unWire the listManager events, will set the listManager to the new
    // reality, will wire the new listManager, will update the policy, will set the dataGridTable, will reset the ui state.
    //
    internal void Set_ListManager(object newDataSource, string newDataMember, bool force, bool forceColumnCreation)
    {
        bool dataSourceChanged = DataSource != newDataSource;
        bool dataMemberChanged = DataMember != newDataMember;

        // if nothing happened, then why do any work?
        if (!force && !dataSourceChanged && !dataMemberChanged && gridState[GRIDSTATE_inSetListManager])
            return;

        gridState[GRIDSTATE_inSetListManager] = true;
        if (toBeDisposedEditingControl is not null)
        {
            Debug.Assert(Controls.Contains(toBeDisposedEditingControl));
            Controls.Remove(toBeDisposedEditingControl);
            toBeDisposedEditingControl = null;
        }

        bool beginUpdateInternal = true;
        try
        {
            // will endEdit on the current listManager
            UpdateListManager();

            // unwire the events:
            if (listManager is not null)
                UnWireDataSource();

            CurrencyManager oldListManager = listManager;
            bool listManagerChanged = false;
            // set up the new listManager
            // CAUTION: we need to set up the listManager in the grid before setting the dataSource/dataMember props
            // in the grid. the reason is that if the BindingContext was not yet requested, and it is created in the BindingContext prop
            // then the grid will call Set_ListManager again, and eventually that means that the dataGrid::listManager will
            // be hooked up twice to all the events (PositionChanged, ItemChanged, CurrentChanged)
            if (newDataSource is not null && BindingContext is not null && !(newDataSource == Convert.DBNull))
                listManager = (CurrencyManager)BindingContext[newDataSource, newDataMember];
            else
                listManager = null;

            // update the dataSource and the dateMember
            dataSource = newDataSource;
            dataMember = newDataMember is null ? "" : newDataMember;

            listManagerChanged = (listManager != oldListManager);

            // wire the events
            if (listManager is not null)
            {
                WireDataSource();
                // update the policy
                policy.UpdatePolicy(listManager, ReadOnly);
            }

            if (!Initializing)
            {
                if (listManager is null)
                {
                    if (ContainsFocus && ParentInternal is null)
                    {
                        Debug.Assert(toBeDisposedEditingControl is null, "we should have removed the toBeDisposedEditingControl already");
                        // if we unparent the active control then the form won't close
                        for (int i = 0; i < Controls.Count; i++)
                        {
                            if (Controls[i].Focused)
                            {
                                toBeDisposedEditingControl = Controls[i];
                                break;
                            }
                        }

                        if (toBeDisposedEditingControl == horizScrollBar || toBeDisposedEditingControl == vertScrollBar)
                        {
                            toBeDisposedEditingControl = null;
                        }

#if DEBUG
                        else
                        {
                            Debug.Assert(toBeDisposedEditingControl is not null, "if the grid contains the focus, then the active control should be in the children of data grid control");
                            Debug.Assert(editColumn is not null, "if we have an editing control should be a control in the data grid column");
                            if (editColumn is DataGridTextBoxColumn)
                            {
                                Debug.Assert(((DataGridTextBoxColumn)editColumn).TextBox == toBeDisposedEditingControl, "if we have an editing control should be a control in the data grid column");
                            }
                        }
#endif // debug;

                    }

                    SetDataGridRows(null, 0);
                    defaultTableStyle.GridColumnStyles.Clear();
                    SetDataGridTable(defaultTableStyle, forceColumnCreation);

                    if (toBeDisposedEditingControl is not null)
                    {
                        Controls.Add(toBeDisposedEditingControl);
                    }
                }
            }

            // PERF: if the listManager did not change, then do not:
            //      1. create new rows
            //      2. create new columns
            //      3. compute the errors in the list
            //
            // when the metaDataChanges, we need to recreate
            // the rows and the columns
            //
            if (listManagerChanged || gridState[GRIDSTATE_metaDataChanged])
            {
                if (Visible)
                    BeginUpdateInternal();

                if (listManager is not null)
                {
                    // get rid of the old gridColumns
                    // we need to clear the old column collection even when navigating to
                    // a list that has a table style associated w/ it. Why? because the
                    // old column collection will be used by the parent rows to paint
                    defaultTableStyle.GridColumnStyles.ResetDefaultColumnCollection();

                    DataGridTableStyle newGridTable = dataGridTables[listManager.GetListName()];
                    if (newGridTable is null)
                    {
                        SetDataGridTable(defaultTableStyle, forceColumnCreation);
                    }
                    else
                    {
                        SetDataGridTable(newGridTable, forceColumnCreation);
                    }

                    // set the currentRow in ssync w/ the position in the listManager
                    currentRow = listManager.Position == -1 ? 0 : listManager.Position;
                }

                // when we create the rows we need to use the current dataGridTable
                //
                RecreateDataGridRows();
                if (Visible)
                    EndUpdateInternal();
                beginUpdateInternal = false;

                ComputeMinimumRowHeaderWidth();
                if (myGridTable.IsDefault)
                    RowHeaderWidth = Math.Max(minRowHeaderWidth, RowHeaderWidth);
                else
                    myGridTable.RowHeaderWidth = Math.Max(minRowHeaderWidth, RowHeaderWidth);

                ListHasErrors = DataGridSourceHasErrors();

                // build the list of columns and relationships
                // wipe out the now invalid states
                // ResetMouseState();

                ResetUIState();

                // layout.CaptionVisible = dataCursor is null ? false : true;

                OnDataSourceChanged(EventArgs.Empty);
            }
        }
        finally
        {
            gridState[GRIDSTATE_inSetListManager] = false;
            // start painting again
            if (beginUpdateInternal && Visible)
                EndUpdateInternal();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public int CurrentRowIndex
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public GridTableStylesCollection TableStyles
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
    }

    internal new int FontHeight
    {
        get
        {
            return fontHeight;
        }
    }

    internal AccessibleObject ParentRowsAccessibleObject
    {
        get
        {
            return parentRows.AccessibleObject;
        }
    }

    internal Rectangle ParentRowsBounds
    {
        get
        {
            return layout.ParentRows;
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Color GridLineColor
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    protected virtual bool ShouldSerializeGridLineColor()
    {
        return !GridLineBrush.Equals(DefaultGridLineBrush);
    }

    public void ResetGridLineColor()
    {
        throw new PlatformNotSupportedException();
    }

    internal SolidBrush GridLineBrush
    {
        get
        {
            return gridLineBrush;
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public DataGridLineStyle GridLineStyle
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    internal int GridLineWidth
    {
        get
        {
            Debug.Assert(GridLineStyle == DataGridLineStyle.Solid || GridLineStyle == DataGridLineStyle.None, "are there any other styles?");
            return GridLineStyle == DataGridLineStyle.Solid ? 1 : 0;
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public DataGridParentRowsLabelStyle ParentRowsLabelStyle
    {
        get
        {
            throw new PlatformNotSupportedException();
        }

        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    private static readonly object EVENT_PARENTROWSLABELSTYLECHANGED = new object();

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler ParentRowsLabelStyleChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    internal bool Initializing
    {
        get
        {
            return inInit;
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public int FirstVisibleColumn
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public bool FlatMode
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    private static readonly object EVENT_FLATMODECHANGED = new object();

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler FlatModeChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Color HeaderBackColor
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    internal SolidBrush HeaderBackBrush
    {
        get
        {
            return headerBackBrush;
        }
    }

    protected virtual bool ShouldSerializeHeaderBackColor()
    {
        return !HeaderBackBrush.Equals(DefaultHeaderBackBrush);
    }

    public void ResetHeaderBackColor()
    {
        throw new PlatformNotSupportedException();
    }

    internal SolidBrush BackgroundBrush
    {
        get
        {
            return backgroundBrush;
        }
    }

    private void ResetBackgroundColor()
    {
        if (backgroundBrush is not null && BackgroundBrush != DefaultBackgroundBrush)
        {
            backgroundBrush.Dispose();
            backgroundBrush = null;
        }

        backgroundBrush = DefaultBackgroundBrush;
    }

    protected virtual bool ShouldSerializeBackgroundColor()
    {
        return !BackgroundBrush.Equals(DefaultBackgroundBrush);
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Color BackgroundColor
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    private static readonly object EVENT_BACKGROUNDCOLORCHANGED = new object();

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler BackgroundColorChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Font HeaderFont
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            headerFont = value;
            throw new PlatformNotSupportedException();
        }
    }

    protected bool ShouldSerializeHeaderFont()
    {
        return (headerFont is not null);
    }

    public void ResetHeaderFont()
    {
        throw new PlatformNotSupportedException();
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Color HeaderForeColor
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    protected virtual bool ShouldSerializeHeaderForeColor()
    {
        return !HeaderForePen.Equals(DefaultHeaderForePen);
    }

    public void ResetHeaderForeColor()
    {
        throw new PlatformNotSupportedException();
    }

    internal SolidBrush HeaderForeBrush
    {
        get
        {
            return headerForeBrush;
        }
    }

    internal Pen HeaderForePen
    {
        get
        {
            return headerForePen;
        }
    }

    private void ResetHorizontalOffset()
    {
        horizontalOffset = 0;
        negOffset = 0;
        firstVisibleCol = 0;
        numVisibleCols = 0;
        lastTotallyVisibleCol = -1;
    }

    internal int HorizontalOffset
    {
        get
        {
            return horizontalOffset;
        }
        set
        {
            if (value < 0)
                value = 0;

            int totalWidth = GetColumnWidthSum();
            int widthNotVisible = totalWidth - layout.Data.Width;
            if (value > widthNotVisible && widthNotVisible > 0)
                value = widthNotVisible;

            if (value == horizontalOffset)
                return;

            int change = horizontalOffset - value;
            horizScrollBar.Value = value;
            Rectangle scroll = layout.Data;
            if (layout.ColumnHeadersVisible)
                scroll = Rectangle.Union(scroll, layout.ColumnHeaders);
            horizontalOffset = value;

            firstVisibleCol = ComputeFirstVisibleColumn();
            // update the lastTotallyVisibleCol
            ComputeVisibleColumns();

            if (gridState[GRIDSTATE_isScrolling])
            {
                // if the user did not click on the grid yet, then do not put the edit
                // control when scrolling
                if (currentCol >= firstVisibleCol && currentCol < firstVisibleCol + numVisibleCols - 1 && (gridState[GRIDSTATE_isEditing] || gridState[GRIDSTATE_isNavigating]))
                    Edit();
                else
                    EndEdit();

                // isScrolling is set to TRUE when the user scrolls.
                // once we move the edit box, we finished processing the scroll event, so set isScrolling to FALSE
                // to set isScrolling to TRUE, we need another scroll event.
                gridState[GRIDSTATE_isScrolling] = false;
            }
            else
            {
                EndEdit();
            }

            RECT[] rects = CreateScrollableRegion(scroll);
            ScrollRectangles(rects, change);
            OnScroll(EventArgs.Empty);
        }
    }

    private void ScrollRectangles(RECT[] rects, int change)
    {
        if (rects is not null)
        {
            RECT scroll;
            if (isRightToLeft())
                change = -change;
            for (int r = 0; r < rects.Length; r++)
            {
                scroll = rects[r];
            }
        }
    }

    protected ScrollBar HorizScrollBar
    {
        get
        {
            return horizScrollBar;
        }
    }

    /// <devdoc>
    ///    <para>
    ///       Retrieves a value indicating whether odd and even
    ///       rows are painted using a different background color.
    ///    </para>
    /// </devdoc>
    // CUT by 53973 - Cleanup eventually to be static.
    internal bool LedgerStyle
    {
        get
        {
            return gridState[GRIDSTATE_isLedgerStyle];
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Color LinkColor
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    internal virtual bool ShouldSerializeLinkColor()
    {
        return !LinkBrush.Equals(DefaultLinkBrush);
    }

    public void ResetLinkColor()
    {
        throw new PlatformNotSupportedException();
    }

    internal Brush LinkBrush
    {
        get
        {
            return linkBrush;
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Color LinkHoverColor
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
        }
    }

    protected virtual bool ShouldSerializeLinkHoverColor()
    {
        return false;
    }

    public void ResetLinkHoverColor()
    {
        throw new PlatformNotSupportedException();
    }

    internal Font LinkFont
    {
        get
        {
            return linkFont;
        }
    }

    internal int LinkFontHeight
    {
        get
        {
            return linkFontHeight;
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public bool AllowNavigation
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    private static readonly object EVENT_ALLOWNAVIGATIONCHANGED = new object();

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler AllowNavigationChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override Cursor Cursor
    {
        get
        {
            throw new PlatformNotSupportedException();
        }

        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    new public event EventHandler CursorChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override Image BackgroundImage
    {
        // get the BackgroundImage out of the propertyGrid.
        get
        {
            throw new PlatformNotSupportedException();
        }

        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override ImageLayout BackgroundImageLayout
    {
        // get the BackgroundImage out of the propertyGrid.
        get
        {
            throw new PlatformNotSupportedException();
        }

        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    new public event EventHandler BackgroundImageChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    new public event EventHandler BackgroundImageLayoutChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Color ParentRowsBackColor
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    internal SolidBrush ParentRowsBackBrush
    {
        get
        {
            return parentRows.BackBrush;
        }
    }

    protected virtual bool ShouldSerializeParentRowsBackColor()
    {
        return !ParentRowsBackBrush.Equals(DefaultParentRowsBackBrush);
    }

    private void ResetParentRowsBackColor()
    {
        if (ShouldSerializeParentRowsBackColor())
            parentRows.BackBrush = DefaultParentRowsBackBrush;
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Color ParentRowsForeColor
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    internal SolidBrush ParentRowsForeBrush
    {
        get
        {
            return parentRows.ForeBrush;
        }
    }

    protected virtual bool ShouldSerializeParentRowsForeColor()
    {
        return !ParentRowsForeBrush.Equals(DefaultParentRowsForeBrush);
    }

    private void ResetParentRowsForeColor()
    {
        if (ShouldSerializeParentRowsForeColor())
            parentRows.ForeBrush = DefaultParentRowsForeBrush;
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public int PreferredColumnWidth
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public int PreferredRowHeight
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    private void ResetPreferredRowHeight()
    {
        prefferedRowHeight = defaultFontHeight + 3;
    }

    protected bool ShouldSerializePreferredRowHeight()
    {
        return prefferedRowHeight != defaultFontHeight + 3;
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public bool ReadOnly
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    private static readonly object EVENT_READONLYCHANGED = new object();

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler ReadOnlyChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public bool ColumnHeadersVisible
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public bool ParentRowsVisible
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    private static readonly object EVENT_PARENTROWSVISIBLECHANGED = new object();

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler ParentRowsVisibleChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    internal bool ParentRowsIsEmpty()
    {
        return parentRows.IsEmpty();
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public bool RowHeadersVisible
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public int RowHeaderWidth
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override string Text
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    new public event EventHandler TextChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    protected ScrollBar VertScrollBar
    {
        get
        {
            return vertScrollBar;
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public int VisibleColumnCount
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public int VisibleRowCount
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public object this[int rowIndex, int columnIndex]
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public object this[DataGridCell cell]
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    private void WireTableStylePropChanged(DataGridTableStyle gridTable)
    {
        gridTable.GridLineColorChanged += new EventHandler(GridLineColorChanged);
        gridTable.GridLineStyleChanged += new EventHandler(GridLineStyleChanged);
        gridTable.HeaderBackColorChanged += new EventHandler(HeaderBackColorChanged);
        gridTable.HeaderFontChanged += new EventHandler(HeaderFontChanged);
        gridTable.HeaderForeColorChanged += new EventHandler(HeaderForeColorChanged);
        gridTable.LinkColorChanged += new EventHandler(LinkColorChanged);
        gridTable.LinkHoverColorChanged += new EventHandler(LinkHoverColorChanged);
        gridTable.PreferredColumnWidthChanged += new EventHandler(PreferredColumnWidthChanged);
        gridTable.RowHeadersVisibleChanged += new EventHandler(RowHeadersVisibleChanged);
        gridTable.ColumnHeadersVisibleChanged += new EventHandler(ColumnHeadersVisibleChanged);
        gridTable.RowHeaderWidthChanged += new EventHandler(RowHeaderWidthChanged);
        gridTable.AllowSortingChanged += new EventHandler(AllowSortingChanged);
    }

    private void UnWireTableStylePropChanged(DataGridTableStyle gridTable)
    {
        gridTable.GridLineColorChanged -= new EventHandler(GridLineColorChanged);
        gridTable.GridLineStyleChanged -= new EventHandler(GridLineStyleChanged);
        gridTable.HeaderBackColorChanged -= new EventHandler(HeaderBackColorChanged);
        gridTable.HeaderFontChanged -= new EventHandler(HeaderFontChanged);
        gridTable.HeaderForeColorChanged -= new EventHandler(HeaderForeColorChanged);
        gridTable.LinkColorChanged -= new EventHandler(LinkColorChanged);
        gridTable.LinkHoverColorChanged -= new EventHandler(LinkHoverColorChanged);
        gridTable.PreferredColumnWidthChanged -= new EventHandler(PreferredColumnWidthChanged);
        gridTable.RowHeadersVisibleChanged -= new EventHandler(RowHeadersVisibleChanged);
        gridTable.ColumnHeadersVisibleChanged -= new EventHandler(ColumnHeadersVisibleChanged);
        gridTable.RowHeaderWidthChanged -= new EventHandler(RowHeaderWidthChanged);
        gridTable.AllowSortingChanged -= new EventHandler(AllowSortingChanged);
    }

    /// <devdoc>
    ///      DataSource events are handled
    /// </devdoc>
    private void WireDataSource()
    {
        Debug.WriteLineIf(CompModSwitches.DataGridCursor.TraceVerbose, "DataGridCursor: WireDataSource");
        Debug.Assert(listManager is not null, "Can't wire up to a null DataSource");
    }

    private void UnWireDataSource()
    {
        Debug.WriteLineIf(CompModSwitches.DataGridCursor.TraceVerbose, "DataGridCursor: UnWireDataSource");
        Debug.Assert(listManager is not null, "Can't un wire from a null DataSource");
    }

    // This is called after a row has been added.  And I think whenever
    // a row gets deleted, etc.
    // We recreate our datagrid rows at this point.
    private void DataSource_Changed(object sender, EventArgs ea)
    {
        Debug.WriteLineIf(CompModSwitches.DataGridCursor.TraceVerbose, "DataGridCursor: DataSource_Changed");

        // the grid will receive the dataSource_Changed event when
        // allowAdd changes on the dataView.
        policy.UpdatePolicy(ListManager, ReadOnly);
        if (gridState[GRIDSTATE_inListAddNew])
        {
            // we are adding a new row
            // keep the old rows, w/ their height, expanded/collapsed information
            //
            Debug.Assert(policy.AllowAdd, "how can we add a new row if the policy does not allow this?");
            Debug.Assert(DataGridRowsLength == DataGridRows.Length, "how can this fail?");

            DataGridRow[] gridRows = DataGridRows;
            int currentRowCount = DataGridRowsLength;
            // put the added row:
            //
            gridRows[currentRowCount - 1] = new DataGridRelationshipRow(this, myGridTable, currentRowCount - 1);
            SetDataGridRows(gridRows, currentRowCount);
        }
        else if (gridState[GRIDSTATE_inAddNewRow] && !gridState[GRIDSTATE_inDeleteRow])
        {
            // when the backEnd adds a row and we are still inAddNewRow
            listManager.CancelCurrentEdit();
            gridState[GRIDSTATE_inAddNewRow] = false;
            RecreateDataGridRows();
        }
        else if (!gridState[GRIDSTATE_inDeleteRow])
        {
            RecreateDataGridRows();
            currentRow = Math.Min(currentRow, listManager.Count);
        }

        bool oldListHasErrors = ListHasErrors;
        ListHasErrors = DataGridSourceHasErrors();
        // if we changed the ListHasErrors, then the grid is already invalidated
        if (oldListHasErrors == ListHasErrors)
            InvalidateInside();
    }

    private void GridLineColorChanged(object sender, EventArgs e)
    {
        Invalidate(layout.Data);
    }

    private void GridLineStyleChanged(object sender, EventArgs e)
    {
        this.myGridTable.ResetRelationsUI();
        Invalidate(layout.Data);
    }

    private void HeaderBackColorChanged(object sender, EventArgs e)
    {
        if (layout.RowHeadersVisible)
            Invalidate(layout.RowHeaders);
        if (layout.ColumnHeadersVisible)
            Invalidate(layout.ColumnHeaders);
        Invalidate(layout.TopLeftHeader);
    }

    private void HeaderFontChanged(object sender, EventArgs e)
    {
        RecalculateFonts();
        PerformLayout();
        Invalidate(layout.Inside);
    }

    private void HeaderForeColorChanged(object sender, EventArgs e)
    {
        if (layout.RowHeadersVisible)
            Invalidate(layout.RowHeaders);
        if (layout.ColumnHeadersVisible)
            Invalidate(layout.ColumnHeaders);
        Invalidate(layout.TopLeftHeader);
    }

    private void LinkColorChanged(object sender, EventArgs e)
    {
        Invalidate(layout.Data);
    }

    private void LinkHoverColorChanged(object sender, EventArgs e)
    {
        Invalidate(layout.Data);
    }

    private void PreferredColumnWidthChanged(object sender, EventArgs e)
    {
        // reset the dataGridRows
        SetDataGridRows(null, this.DataGridRowsLength);
        // layout the horizontal scroll bar
        PerformLayout();
        // invalidate everything
        Invalidate();
    }

    private void RowHeadersVisibleChanged(object sender, EventArgs e)
    {
        layout.RowHeadersVisible = myGridTable is null ? false : myGridTable.RowHeadersVisible;
        PerformLayout();
        InvalidateInside();
    }

    private void ColumnHeadersVisibleChanged(object sender, EventArgs e)
    {
        layout.ColumnHeadersVisible = myGridTable is null ? false : myGridTable.ColumnHeadersVisible;
        PerformLayout();
        InvalidateInside();
    }

    private void RowHeaderWidthChanged(object sender, EventArgs e)
    {
        if (layout.RowHeadersVisible)
        {
            PerformLayout();
            InvalidateInside();
        }
    }

    private void AllowSortingChanged(object sender, EventArgs e)
    {
        if (!myGridTable.AllowSorting && listManager is not null)
        {
            IList list = listManager.List;
            if (list is IBindingList)
                ((IBindingList)list).RemoveSort();
        }
    }

    private void DataSource_RowChanged(object sender, EventArgs ea)
    {
        Debug.WriteLineIf(CompModSwitches.DataGridCursor.TraceVerbose, "DataGridCursor: DataSource_RowChanged");
        // it may be the case that our cache was not updated
        // to the latest changes in the list : CurrentChanged is fired before
        // ListChanged.
        // So invalidate the row if there is something to invalidate
        DataGridRow[] rows = DataGridRows;
        if (currentRow < DataGridRowsLength)
        {
            InvalidateRow(currentRow);
        }
    }

    /// <devdoc>
    ///    <para>
    ///       Fired by the DataSource when row position moves.
    ///    </para>
    /// </devdoc>
    private void DataSource_PositionChanged(object sender, EventArgs ea)
    {
        Debug.WriteLineIf(CompModSwitches.DataGridCursor.TraceVerbose, "DataGridCursor: DataSource_PositionChanged to " + listManager.Position.ToString(CultureInfo.InvariantCulture));
        // the grid will get the PositionChanged event
        // before the OnItemChanged event when a row will be deleted in the backEnd;
        // we still want to keep the old rows when the user deletes the rows using the grid
        // and we do not want to do the same work twice when the user adds a row via the grid
        if (DataGridRowsLength > listManager.Count + (policy.AllowAdd ? 1 : 0) && !gridState[GRIDSTATE_inDeleteRow])
        {
            Debug.Assert(!gridState[GRIDSTATE_inAddNewRow] && !gridState[GRIDSTATE_inListAddNew], "how can the list decrease when we are adding a row?");
            RecreateDataGridRows();
        }

        if (ListManager.Position != currentRow)
        {
            CurrentCell = new DataGridCell(listManager.Position, currentCol);
        }
    }

    internal void DataSource_MetaDataChanged(object sender, EventArgs e)
    {
        MetaDataChanged();
    }

    private bool DataGridSourceHasErrors()
    {
        if (listManager is null)
            return false;
        for (int i = 0; i < listManager.Count; i++)
        {
            object errObj = listManager[i];
            if (errObj is IDataErrorInfo)
            {
                string errString = ((IDataErrorInfo)errObj).Error;
                if (errString is not null && errString.Length != 0)
                    return true;
            }
        }

        return false;
    }

    private void TableStylesCollectionChanged(object sender, CollectionChangeEventArgs ccea)
    {
        // if the users changed the collection of tableStyles
        if (sender != dataGridTables)
            return;
        if (listManager is null)
            return;

        if (ccea.Action == CollectionChangeAction.Add)
        {
            DataGridTableStyle tableStyle = (DataGridTableStyle)ccea.Element;
            if (listManager.GetListName().Equals(tableStyle.MappingName))
            {
                Debug.Assert(myGridTable.IsDefault, "if the table is not default, then it had a name. how can one add another table to the collection w/ the same name and not throw an exception");
                SetDataGridTable(tableStyle, true);                // true for forcing column creation
                SetDataGridRows(null, 0);
            }
        }
        else if (ccea.Action == CollectionChangeAction.Remove)
        {
            DataGridTableStyle tableStyle = (DataGridTableStyle)ccea.Element;
            if (myGridTable.MappingName.Equals(tableStyle.MappingName))
            {
                Debug.Assert(myGridTable.IsDefault, "if the table is not default, then it had a name. how can one add another table to the collection w/ the same name and not throw an exception");
                defaultTableStyle.GridColumnStyles.ResetDefaultColumnCollection();
                SetDataGridTable(defaultTableStyle, true);    // true for forcing column creation
                SetDataGridRows(null, 0);
            }
        }
        else
        {
            Debug.Assert(ccea.Action == CollectionChangeAction.Refresh, "what else is possible?");
            // we have to search to see if the collection of table styles contains one
            // w/ the same name as the list in the dataGrid

            DataGridTableStyle newGridTable = dataGridTables[listManager.GetListName()];
            if (newGridTable is null)
            {
                if (!myGridTable.IsDefault)
                {
                    // get rid of the old gridColumns
                    defaultTableStyle.GridColumnStyles.ResetDefaultColumnCollection();
                    SetDataGridTable(defaultTableStyle, true);    // true for forcing column creation
                    SetDataGridRows(null, 0);
                }
            }
            else
            {
                SetDataGridTable(newGridTable, true);              // true for forcing column creation
                SetDataGridRows(null, 0);
            }
        }
    }

    private void DataSource_ItemChanged(object sender, ItemChangedEventArgs ea)
    {
        Debug.WriteLineIf(CompModSwitches.DataGridCursor.TraceVerbose, "DataGridCursor: DataSource_ItemChanged at index " + ea.Index.ToString(CultureInfo.InvariantCulture));

        // if ea.Index == -1, then we invalidate all rows.
        if (ea.Index == -1)
        {
            DataSource_Changed(sender, EventArgs.Empty);
        }
        else
        {
            // let's see how we are doing w/ the errors
            object errObj = this.listManager[ea.Index];
            bool oldListHasErrors = ListHasErrors;
            if (errObj is IDataErrorInfo)
            {
                if (((IDataErrorInfo)errObj).Error.Length != 0)
                    ListHasErrors = true;
                else if (ListHasErrors)
                {
                    // maybe there was an error that now is fixed
                    ListHasErrors = DataGridSourceHasErrors();
                }
            }

            // Invalidate the row only if we did not change the ListHasErrors
            if (oldListHasErrors == ListHasErrors)
                InvalidateRow(ea.Index);

            // we need to update the edit box:
            // we update the text in the edit box only when the currentRow
            // equals the ea.Index
            if (editColumn is not null && ea.Index == currentRow)
                editColumn.UpdateUI(ListManager, ea.Index, null);
        }
    }

    protected virtual void OnBorderStyleChanged(EventArgs e)
    {
        EventHandler eh = Events[EVENT_BORDERSTYLECHANGED] as EventHandler;
        if (eh is not null)
        {
            eh(this, e);
        }
    }

    protected virtual void OnCaptionVisibleChanged(EventArgs e)
    {
        EventHandler eh = Events[EVENT_CAPTIONVISIBLECHANGED] as EventHandler;
        if (eh is not null)
        {
            eh(this, e);
        }
    }

    protected virtual void OnCurrentCellChanged(EventArgs e)
    {
        EventHandler eh = Events[EVENT_CURRENTCELLCHANGED] as EventHandler;
        if (eh is not null)
        {
            eh(this, e);
        }
    }

    protected virtual void OnFlatModeChanged(EventArgs e)
    {
        EventHandler eh = Events[EVENT_FLATMODECHANGED] as EventHandler;
        if (eh is not null)
        {
            eh(this, e);
        }
    }

    protected virtual void OnBackgroundColorChanged(EventArgs e)
    {
        EventHandler eh = Events[EVENT_BACKGROUNDCOLORCHANGED] as EventHandler;
        if (eh is not null)
        {
            eh(this, e);
        }
    }

    protected virtual void OnAllowNavigationChanged(EventArgs e)
    {
        EventHandler eh = Events[EVENT_ALLOWNAVIGATIONCHANGED] as EventHandler;
        if (eh is not null)
        {
            eh(this, e);
        }
    }

    protected virtual void OnParentRowsVisibleChanged(EventArgs e)
    {
        EventHandler eh = Events[EVENT_PARENTROWSVISIBLECHANGED] as EventHandler;
        if (eh is not null)
        {
            eh(this, e);
        }
    }

    protected virtual void OnParentRowsLabelStyleChanged(EventArgs e)
    {
        EventHandler eh = Events[EVENT_PARENTROWSLABELSTYLECHANGED] as EventHandler;
        if (eh is not null)
        {
            eh(this, e);
        }
    }

    protected virtual void OnReadOnlyChanged(EventArgs e)
    {
        EventHandler eh = Events[EVENT_READONLYCHANGED] as EventHandler;
        if (eh is not null)
        {
            eh(this, e);
        }
    }

    protected void OnNavigate(NavigateEventArgs e)
    {
    }

    internal void OnNodeClick(EventArgs e)
    {
        PerformLayout();

        GridColumnStylesCollection columns = myGridTable.GridColumnStyles;
        if (firstVisibleCol > -1 && firstVisibleCol < columns.Count && columns[firstVisibleCol] == editColumn)
            Edit();

        // Raise the event for the event listeners
        EventHandler handler = (EventHandler)Events[EVENT_NODECLICKED];
        if (handler is not null)
        {
            handler(this, e);
        }
    }

    protected void OnRowHeaderClick(EventArgs e)
    {
        if (onRowHeaderClick is not null)
            onRowHeaderClick(this, e);
    }

    protected void OnScroll(EventArgs e)
    {
        // reset the toolTip information
        if (ToolTipProvider is not null)
            ResetToolTip();

        EventHandler handler = (EventHandler)Events[EVENT_SCROLL];
        if (handler is not null)
        {
            handler(this, e);
        }
    }

    protected virtual void GridHScrolled(object sender, ScrollEventArgs se)
    {
        if (!Enabled)
            return;
        if (DataSource is null)
        {
            Debug.Fail("Horizontal Scrollbar should be disabled without a DataSource.");
            return;
        }

        gridState[GRIDSTATE_isScrolling] = true;

#if DEBUG

        Debug.WriteLineIf(CompModSwitches.DataGridScrolling.TraceVerbose, "DataGridScrolling: in GridHScrolled: the scroll event type:");
        switch (se.Type)
        {
            case ScrollEventType.SmallIncrement:
                Debug.WriteLineIf(CompModSwitches.DataGridScrolling.TraceVerbose, "small increment");
                break;
            case ScrollEventType.SmallDecrement:
                Debug.WriteLineIf(CompModSwitches.DataGridScrolling.TraceVerbose, "small decrement");
                break;
            case ScrollEventType.LargeIncrement:
                Debug.WriteLineIf(CompModSwitches.DataGridScrolling.TraceVerbose, "Large decrement");
                break;
            case ScrollEventType.LargeDecrement:
                Debug.WriteLineIf(CompModSwitches.DataGridScrolling.TraceVerbose, "small decrement");
                break;
            case ScrollEventType.ThumbPosition:
                Debug.WriteLineIf(CompModSwitches.DataGridScrolling.TraceVerbose, "Thumb Position");
                break;
            case ScrollEventType.ThumbTrack:
                Debug.WriteLineIf(CompModSwitches.DataGridScrolling.TraceVerbose, "Thumb Track");
                break;
            case ScrollEventType.First:
                Debug.WriteLineIf(CompModSwitches.DataGridScrolling.TraceVerbose, "First");
                break;
            case ScrollEventType.Last:
                Debug.WriteLineIf(CompModSwitches.DataGridScrolling.TraceVerbose, "Last");
                break;
            case ScrollEventType.EndScroll:
                Debug.WriteLineIf(CompModSwitches.DataGridScrolling.TraceVerbose, "EndScroll");
                break;
        }

#endif // DEBUG

        if (se.Type == ScrollEventType.SmallIncrement ||
            se.Type == ScrollEventType.SmallDecrement)
        {
            int dCols = (se.Type == ScrollEventType.SmallIncrement) ? 1 : -1;
            if (se.Type == ScrollEventType.SmallDecrement && negOffset == 0)
            {
                GridColumnStylesCollection cols = myGridTable.GridColumnStyles;
                // if the column before the first visible column has width == 0 then skip it
                for (int i = firstVisibleCol - 1; i >= 0 && cols[i].Width == 0; i--)
                {
                    dCols--;
                }
            }

            if (se.Type == ScrollEventType.SmallIncrement && negOffset == 0)
            {
                GridColumnStylesCollection cols = myGridTable.GridColumnStyles;
                for (int i = firstVisibleCol; i > -1 && i < cols.Count && cols[i].Width == 0; i++)
                {
                    dCols++;
                }
            }

            ScrollRight(dCols);
            se.NewValue = HorizontalOffset;
        }
        else if (se.Type != ScrollEventType.EndScroll)
        {
            HorizontalOffset = se.NewValue;
        }

        gridState[GRIDSTATE_isScrolling] = false;
    }

    protected virtual void GridVScrolled(object sender, ScrollEventArgs se)
    {
        if (!Enabled)
            return;
        if (DataSource is null)
        {
            Debug.Fail("Vertical Scrollbar should be disabled without a DataSource.");
            return;
        }

        gridState[GRIDSTATE_isScrolling] = true;

        try
        {
            se.NewValue = Math.Min(se.NewValue, DataGridRowsLength - numTotallyVisibleRows);
            int dRows = se.NewValue - firstVisibleRow;
            ScrollDown(dRows);
        }
        finally
        {
            gridState[GRIDSTATE_isScrolling] = false;
        }
    }

    private void HandleEndCurrentEdit()
    {
        int currentRowSaved = currentRow;
        int currentColSaved = currentCol;

        string errorMessage = null;

        try
        {
            listManager.EndCurrentEdit();
        }
        catch (Exception e)
        {
            errorMessage = e.Message;
        }

        if (errorMessage is not null)
        {
            DialogResult result = RTLAwareMessageBox.Show(null,
                "SR.GetString(SR.DataGridPushedIncorrectValueIntoColumn, errorMessage)",
                "SR.GetString(SR.DataGridErrorMessageBoxCaption)",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, 0);

            if (result == DialogResult.Yes)
            {
                currentRow = currentRowSaved;
                currentCol = currentColSaved;
                Debug.Assert(currentRow == ListManager.Position || listManager.Position == -1, "the position in the list manager (" + ListManager.Position + ") is out of sync with the currentRow (" + currentRow + ")" + " and the exception is '" + errorMessage + "'");
                // also, make sure that we get the row selector on the currentrow, too
                InvalidateRowHeader(currentRow);
                Edit();
            }
            else
            {
                Debug.Assert(result == DialogResult.No, "we only put cancel and ok on the error message box");
                listManager.CancelCurrentEdit();
                listManager.Position = currentRow;
            }
        }
    }

    protected void OnBackButtonClicked(object sender, EventArgs e)
    {
        NavigateBack();

        EventHandler handler = (EventHandler)Events[EVENT_BACKBUTTONCLICK];
        if (handler is not null)
            handler(this, e);
    }

    protected override void OnBackColorChanged(EventArgs e)
    {
        backBrush = new SolidBrush(BackColor);
        Invalidate();

        base.OnBackColorChanged(e);
    }

    protected override void OnBindingContextChanged(EventArgs e)
    {
        if (DataSource is not null && !gridState[GRIDSTATE_inSetListManager])
            try
            {
                Set_ListManager(DataSource, DataMember, true, false);
            }
            catch
            {
                // at runtime we will rethrow the exception
                if (Site is null || !Site.DesignMode)
                    throw;

                RTLAwareMessageBox.Show(null, "SR.GetString(SR.DataGridExceptionInPaint)", null,
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, 0);

                if (Visible)
                    BeginUpdateInternal();

                ResetParentRows();

                Set_ListManager(null, string.Empty, true);
                if (Visible)
                    EndUpdateInternal();
            }

        base.OnBindingContextChanged(e);
    }

    protected virtual void OnDataSourceChanged(EventArgs e)
    {
        EventHandler eh = Events[EVENT_DATASOURCECHANGED] as EventHandler;
        if (eh is not null)
        {
            eh(this, e);
        }
    }

    protected void OnShowParentDetailsButtonClicked(object sender, EventArgs e)
    {
        ParentRowsVisible = !caption.ToggleDownButtonDirection();

        EventHandler handler = (EventHandler)Events[EVENT_DOWNBUTTONCLICK];
        if (handler is not null)
            handler(this, e);
    }

    protected override void OnForeColorChanged(EventArgs e)
    {
        foreBrush = new SolidBrush(ForeColor);
        Invalidate();

        base.OnForeColorChanged(e);
    }

    protected override void OnFontChanged(EventArgs e)
    {
        // let the caption know about the event changed
        //
        Caption.OnGridFontChanged();
        RecalculateFonts();
        RecreateDataGridRows();
        // get all the rows in the parentRows stack, and modify their height
        if (originalState is not null)
        {
            Debug.Assert(!parentRows.IsEmpty(), "if the originalState is not null, then parentRows contains at least one row");
            Stack parentStack = new Stack();
            while (!parentRows.IsEmpty())
            {
                DataGridState dgs = parentRows.PopTop();
                int rowCount = dgs.DataGridRowsLength;

                for (int i = 0; i < rowCount; i++)
                {
                    // performance hit: this will cause to invalidate a bunch of
                    // stuff

                    dgs.DataGridRows[i].Height = dgs.DataGridRows[i].MinimumRowHeight(dgs.GridColumnStyles);
                }

                parentStack.Push(dgs);
            }

            while (parentStack.Count != 0)
            {
                parentRows.AddParent((DataGridState)parentStack.Pop());
            }
        }

        base.OnFontChanged(e);
    }

    protected override void OnPaintBackground(PaintEventArgs ebe)
    {
    }

    protected override void OnLayout(LayoutEventArgs levent)
    {
        if (gridState[GRIDSTATE_editControlChanging])
            return;

        Debug.WriteLineIf(CompModSwitches.DataGridLayout.TraceVerbose, "DataGridLayout: OnLayout");
        base.OnLayout(levent);

        if (gridState[GRIDSTATE_layoutSuspended])
            return;

        gridState[GRIDSTATE_canFocus] = false;
        try
        {
            if (IsHandleCreated)
            {
                if (layout.ParentRowsVisible)
                    parentRows.OnLayout();

                // reset the toolTip information
                if (ToolTipProvider is not null)
                    ResetToolTip();

                ComputeLayout();
            }
        }
        finally
        {
            gridState[GRIDSTATE_canFocus] = true;
        }
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        toolTipProvider = new DataGridToolTip(this);
        DataGridToolTip.CreateToolTipHandle();
        toolTipId = 0;

        PerformLayout();
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        base.OnHandleDestroyed(e);

        // toolTipping
        if (toolTipProvider is not null)
        {
            DataGridToolTip.Destroy();
            toolTipProvider = null;
        }

        toolTipId = 0;
    }

    protected override void OnEnter(EventArgs e)
    {
        if (gridState[GRIDSTATE_canFocus] && !gridState[GRIDSTATE_editControlChanging])
        {
            if (Bound)
            {
                Edit();
            }

            base.OnEnter(e);
        }
    }

    protected override void OnLeave(EventArgs e)
    {
        OnLeave_Grid();
        base.OnLeave(e);
    }

    private void OnLeave_Grid()
    {
        gridState[GRIDSTATE_canFocus] = false;
        try
        {
            EndEdit();
            if (listManager is not null && !gridState[GRIDSTATE_editControlChanging])
            {
                if (gridState[GRIDSTATE_inAddNewRow])
                {
                    // if the user did not type anything
                    // in the addNewRow, then cancel the currentedit
                    listManager.CancelCurrentEdit();
                    // set the addNewRow back
                    DataGridRow[] localGridRows = DataGridRows;
                    localGridRows[DataGridRowsLength - 1] = new DataGridAddNewRow(this, myGridTable, DataGridRowsLength - 1);
                    SetDataGridRows(localGridRows, DataGridRowsLength);
                }
                else
                {
                    // this.listManager.EndCurrentEdit();
                    HandleEndCurrentEdit();
                }
            }
        }
        finally
        {
            gridState[GRIDSTATE_canFocus] = true;
            // inAddNewRow should be set to false if the control was
            // not changing
            if (!gridState[GRIDSTATE_editControlChanging])
                gridState[GRIDSTATE_inAddNewRow] = false;
        }
    }

    protected override void OnKeyDown(KeyEventArgs ke)
    {
        Debug.WriteLineIf(CompModSwitches.DataGridKeys.TraceVerbose, "DataGridKeys: OnKeyDown ");
        base.OnKeyDown(ke);
        ProcessGridKey(ke);
    }

    protected override void OnKeyPress(KeyPressEventArgs kpe)
    {
        Debug.WriteLineIf(CompModSwitches.DataGridKeys.TraceVerbose, "DataGridKeys: OnKeyPress " + TypeDescriptor.GetConverter(typeof(Keys)).ConvertToString(kpe.KeyChar));

        base.OnKeyPress(kpe);
        GridColumnStylesCollection coll = myGridTable.GridColumnStyles;
        if (coll is not null && currentCol > 0 && currentCol < coll.Count)
        {
            if (!coll[currentCol].ReadOnly)
                if (kpe.KeyChar > 32)
                {
                    Edit(new string(new char[] { kpe.KeyChar }));
                }
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        gridState[GRIDSTATE_childLinkFocused] = false;
        gridState[GRIDSTATE_dragging] = false;
        if (listManager is null)
            return;

        HitTestInfo location = HitTest(e.X, e.Y);
        Keys nModifier = ModifierKeys;
        bool isControlDown = (nModifier & Keys.Control) == Keys.Control && (nModifier & Keys.Alt) == 0;
        bool isShiftDown = (nModifier & Keys.Shift) == Keys.Shift;

        // Only left clicks for now
        if (e.Button != MouseButtons.Left)
            return;

        // Check column resize
        if (location.type == HitTestType.ColumnResize)
        {
            if (e.Clicks > 1)
            {
                ColAutoResize(location.col);
            }
            else
                ColResizeBegin(e, location.col);
            return;
        }

        // Check row resize
        if (location.type == HitTestType.RowResize)
        {
            if (e.Clicks > 1)
            {
                RowAutoResize(location.row);
            }
            else
            {
                RowResizeBegin(e, location.row);
            }

            return;
        }

        // Check column headers
        if (location.type == HitTestType.ColumnHeader)
        {
            trackColumnHeader = myGridTable.GridColumnStyles[location.col].PropertyDescriptor;
            return;
        }

        if (location.type == HitTestType.Caption)
        {
            Rectangle captionRect = layout.Caption;
            caption.MouseDown(e.X - captionRect.X, e.Y - captionRect.Y);
            return;
        }

        if (layout.Data.Contains(e.X, e.Y) || layout.RowHeaders.Contains(e.X, e.Y))
        {
            // Check with the row underneath the mouse
            int row = GetRowFromY(e.Y);
            if (row > -1)
            {
                Point p = NormalizeToRow(e.X, e.Y, row);
                DataGridRow[] localGridRows = DataGridRows;
                if (localGridRows[row].OnMouseDown(p.X, p.Y,
                                                   layout.RowHeaders,
                                                   isRightToLeft()))
                {
                    CommitEdit();

                    localGridRows = DataGridRows;
                    if (row < DataGridRowsLength && (localGridRows[row] is DataGridRelationshipRow) && ((DataGridRelationshipRow)localGridRows[row]).Expanded)
                        EnsureVisible(row, 0);

                    Edit();
                    return;
                }
            }
        }

        if (location.type == HitTestType.RowHeader)
        {
            EndEdit();
            if (!(DataGridRows[location.row] is DataGridAddNewRow))
            {
                int savedCurrentRow = currentRow;
                CurrentCell = new DataGridCell(location.row, currentCol);
                if (location.row != savedCurrentRow &&
                    currentRow != location.row &&
                    currentRow == savedCurrentRow)
                {
                    // The data grid was not able to move away from its previous current row.
                    // Be defensive and don't select the row.
                    return;
                }
            }

            if (isControlDown)
            {
                if (IsSelected(location.row))
                    UnSelect(location.row);
                else
                    Select(location.row);
            }
            else
            {
                if (lastRowSelected == -1 || !isShiftDown)
                {
                    ResetSelection();
                    Select(location.row);
                }
                else
                {
                    int lowerRow = Math.Min(lastRowSelected, location.row);
                    int upperRow = Math.Max(lastRowSelected, location.row);

                    // we need to reset the old SelectedRows.
                    // ResetSelection() will also reset lastRowSelected, so we
                    // need to save it
                    int saveLastRowSelected = lastRowSelected;
                    ResetSelection();
                    lastRowSelected = saveLastRowSelected;

                    DataGridRow[] rows = DataGridRows;
                    for (int i = lowerRow; i <= upperRow; i++)
                    {
                        rows[i].Selected = true;
                        numSelectedRows++;
                    }

                    EndEdit();
                    return;
                }
            }

            lastRowSelected = location.row;
            // OnRowHeaderClick(EventArgs.Empty);
            return;
        }

        // Check parentRows
        if (location.type == HitTestType.ParentRows)
        {
            EndEdit();
            parentRows.OnMouseDown(e.X, e.Y, isRightToLeft());
        }

        // Check cell clicks
        if (location.type == HitTestType.Cell)
        {
            if (myGridTable.GridColumnStyles[location.col].MouseDown(location.row, e.X, e.Y))
                return;
            DataGridCell target = new DataGridCell(location.row, location.col);
            if (policy.AllowEdit && CurrentCell.Equals(target))
            {
                ResetSelection();
                // what if only a part of the current cell is visible?
                EnsureVisible(currentRow, currentCol);
                Edit();
            }
            else
            {
                ResetSelection();
                CurrentCell = target;
            }
        }
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        if (oldRow != -1)
        {
            DataGridRow[] localGridRows = DataGridRows;
            localGridRows[oldRow].OnMouseLeft(layout.RowHeaders, isRightToLeft());
        }

        if (gridState[GRIDSTATE_overCaption])
        {
            caption.MouseLeft();
        }

        Cursor = null;
    }

    internal void TextBoxOnMouseWheel(MouseEventArgs e)
    {
        OnMouseWheel(e);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (listManager is null)
            return;

        HitTestInfo location = HitTest(e.X, e.Y);

        bool alignToRight = isRightToLeft();

        // We need to give UI feedback when the user is resizing a column
        if (gridState[GRIDSTATE_trackColResize])
        {
            ColResizeMove(e);
        }

        if (gridState[GRIDSTATE_trackRowResize])
        {
            RowResizeMove(e);
        }

        if (gridState[GRIDSTATE_trackColResize] || location.type == HitTestType.ColumnResize)
        {
            Cursor = Cursors.SizeWE;
            return;
        }
        else if (gridState[GRIDSTATE_trackRowResize] || location.type == HitTestType.RowResize)
        {
            Cursor = Cursors.SizeNS;
            return;
        }
        else
        {
            Cursor = null;
        }

        if ((layout.Data.Contains(e.X, e.Y)
            || (layout.RowHeadersVisible && layout.RowHeaders.Contains(e.X, e.Y))))
        {
            // && (isNavigating || isEditing)) {
            DataGridRow[] localGridRows = DataGridRows;
            // If we are over a row, let it know about the mouse move.
            int rowOver = GetRowFromY(e.Y);
            // set the dragging bit:
            if (lastRowSelected != -1 && !gridState[GRIDSTATE_dragging])
            {
                int topRow = GetRowTop(lastRowSelected);
                int bottomRow = topRow + localGridRows[lastRowSelected].Height;
                int dragHeight = SystemInformation.DragSize.Height;
                gridState[GRIDSTATE_dragging] = ((e.Y - topRow < dragHeight && topRow - e.Y < dragHeight) || (e.Y - bottomRow < dragHeight && bottomRow - e.Y < dragHeight));
            }

            if (rowOver > -1)
            {
                Point p = NormalizeToRow(e.X, e.Y, rowOver);
                if (!localGridRows[rowOver].OnMouseMove(p.X, p.Y, layout.RowHeaders, alignToRight) && gridState[GRIDSTATE_dragging])
                {
                    // if the row did not use this, see if selection can use it
                    MouseButtons mouse = MouseButtons;
                    if (lastRowSelected != -1 && (mouse & MouseButtons.Left) == MouseButtons.Left
                        && !(((Control.ModifierKeys & Keys.Control) == Keys.Control) && (Control.ModifierKeys & Keys.Alt) == 0))
                    {
                        // ResetSelection() will reset the lastRowSelected too
                        //
                        int saveLastRowSelected = lastRowSelected;
                        ResetSelection();
                        lastRowSelected = saveLastRowSelected;

                        int lowerRow = Math.Min(lastRowSelected, rowOver);
                        int upperRow = Math.Max(lastRowSelected, rowOver);

                        DataGridRow[] rows = DataGridRows;
                        for (int i = lowerRow; i <= upperRow; i++)
                        {
                            rows[i].Selected = true;
                            numSelectedRows++;
                        }
                    }
                }
            }

            if (oldRow != rowOver && oldRow != -1)
            {
                localGridRows[oldRow].OnMouseLeft(layout.RowHeaders, alignToRight);
            }

            oldRow = rowOver;
        }

        // check parentRows
        if (location.type == HitTestType.ParentRows)
        {
            if (parentRows is not null)
            {
                parentRows.OnMouseMove(e.X, e.Y);
            }
        }

        if (location.type == HitTestType.Caption)
        {
            gridState[GRIDSTATE_overCaption] = true;
            Rectangle captionRect = layout.Caption;
            caption.MouseOver(e.X - captionRect.X, e.Y - captionRect.Y);
            return;
        }
        else
        {
            if (gridState[GRIDSTATE_overCaption])
            {
                gridState[GRIDSTATE_overCaption] = false;
                caption.MouseLeft();
            }
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        gridState[GRIDSTATE_dragging] = false;
        if (listManager is null || myGridTable is null)
            return;
        if (gridState[GRIDSTATE_trackColResize])
        {
            ColResizeEnd(e);
        }

        if (gridState[GRIDSTATE_trackRowResize])
        {
            RowResizeEnd(e);
        }

        gridState[GRIDSTATE_trackColResize] = false;
        gridState[GRIDSTATE_trackRowResize] = false;

        HitTestInfo ci = HitTest(e.X, e.Y);
        if ((ci.type & HitTestType.Caption) == HitTestType.Caption)
        {
            caption.MouseUp(e.X, e.Y);
        }

        // Check column headers
        if (ci.type == HitTestType.ColumnHeader)
        {
            PropertyDescriptor prop = myGridTable.GridColumnStyles[ci.col].PropertyDescriptor;
            if (prop == trackColumnHeader)
            {
                ColumnHeaderClicked(trackColumnHeader);
            }
        }

        trackColumnHeader = null;
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);

        if (e is HandledMouseEventArgs)
        {
            if (((HandledMouseEventArgs)e).Handled)
            {
                // The application event handler handled the scrolling - don't do anything more.
                return;
            }

            ((HandledMouseEventArgs)e).Handled = true;
        }

        bool wheelingDown = true;
        if ((ModifierKeys & Keys.Control) != 0)
            wheelingDown = false;

        if (listManager is null || myGridTable is null)
            return;
        ScrollBar sb = wheelingDown ? vertScrollBar : horizScrollBar;
        if (!sb.Visible)
            return;

        // so we scroll. we have to know this, cause otherwise we will call EndEdit
        // and that would be wrong.
        gridState[GRIDSTATE_isScrolling] = true;
        wheelDelta += e.Delta;
        float movePerc = (float)wheelDelta / PInvoke.WHEEL_DELTA;
        int move = (int)(SystemInformation.MouseWheelScrollLines * movePerc);
        if (move != 0)
        {
            wheelDelta = 0;
            Debug.WriteLineIf(CompModSwitches.DataGridLayout.TraceVerbose, "DataGridLayout: OnMouseWheel move=" + move.ToString(CultureInfo.InvariantCulture));
            if (wheelingDown)
            {
                int newRow = firstVisibleRow - move;
                newRow = Math.Max(0, Math.Min(newRow, DataGridRowsLength - numTotallyVisibleRows));
                ScrollDown(newRow - firstVisibleRow);
            }
            else
            {
                int newValue = horizScrollBar.Value + (move < 0 ? 1 : -1) * horizScrollBar.LargeChange;
                HorizontalOffset = newValue;
            }
        }

        gridState[GRIDSTATE_isScrolling] = false;
    }

    protected override void OnPaint(PaintEventArgs pe)
    {
        try
        {
            CheckHierarchyState();

            if (layout.dirty)
                ComputeLayout();

            Graphics g = pe.Graphics;

            Region clipRegion = g.Clip;
            if (layout.CaptionVisible)
                caption.Paint(g, layout.Caption, isRightToLeft());

            if (layout.ParentRowsVisible)
            {
                Debug.WriteLineIf(CompModSwitches.DataGridParents.TraceVerbose, "DataGridParents: Painting ParentRows " + layout.ParentRows.ToString());
                g.FillRectangle(SystemBrushes.AppWorkspace, layout.ParentRows);
                parentRows.Paint(g, layout.ParentRows, isRightToLeft());
            }

            Rectangle gridRect = layout.Data;
            if (layout.RowHeadersVisible)
                gridRect = Rectangle.Union(gridRect, layout.RowHeaders);
            if (layout.ColumnHeadersVisible)
                gridRect = Rectangle.Union(gridRect, layout.ColumnHeaders);

            g.SetClip(gridRect);
            PaintGrid(g, gridRect);
            g.Clip = clipRegion;
            clipRegion.Dispose();
            PaintBorder(g, layout.ClientRectangle);

            g.FillRectangle(DefaultHeaderBackBrush, layout.ResizeBoxRect);

            base.OnPaint(pe); // raise paint event
        }
        catch
        {
            // at runtime we will rethrow the exception
            if (Site is null || !Site.DesignMode)
                throw;
            gridState[GRIDSTATE_exceptionInPaint] = true;
            try
            {
                RTLAwareMessageBox.Show(null, "SR.GetString(SR.DataGridExceptionInPaint)", null, MessageBoxButtons.OK,
                    MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, 0);

                if (Visible)
                    BeginUpdateInternal();

                ResetParentRows();

                Set_ListManager(null, string.Empty, true);
            }
            finally
            {
                gridState[GRIDSTATE_exceptionInPaint] = false;
                if (Visible)
                    EndUpdateInternal();
            }
        }
    }

    protected override void OnResize(EventArgs e)
    {
        Debug.WriteLineIf(CompModSwitches.DataGridLayout.TraceVerbose, "DataGridLayout: OnResize");

        if (layout.CaptionVisible)
            Invalidate(layout.Caption);
        if (layout.ParentRowsVisible)
            parentRows.OnResize(layout.ParentRows);

        int borderWidth = BorderWidth;
        Rectangle right;
        Rectangle bottom;
        Rectangle oldClientRectangle = layout.ClientRectangle;

        right = new Rectangle(oldClientRectangle.X + oldClientRectangle.Width - borderWidth,
                              oldClientRectangle.Y,
                              borderWidth,
                              oldClientRectangle.Height);
        bottom = new Rectangle(oldClientRectangle.X,
                               oldClientRectangle.Y + oldClientRectangle.Height - borderWidth,
                               oldClientRectangle.Width,
                               borderWidth);

        Rectangle newClientRectangle = ClientRectangle;
        if (newClientRectangle.Width != oldClientRectangle.Width)
        {
            Invalidate(right);
            right = new Rectangle(newClientRectangle.X + newClientRectangle.Width - borderWidth,
                                  newClientRectangle.Y,
                                  borderWidth,
                                  newClientRectangle.Height);
            Invalidate(right);
        }

        if (newClientRectangle.Height != oldClientRectangle.Height)
        {
            Invalidate(bottom);
            bottom = new Rectangle(newClientRectangle.X,
                                   newClientRectangle.Y + newClientRectangle.Height - borderWidth,
                                   newClientRectangle.Width,
                                   borderWidth);
            Invalidate(bottom);
        }

        // also, invalidate the ResizeBoxRect
        if (!layout.ResizeBoxRect.IsEmpty)
            Invalidate(layout.ResizeBoxRect);

        layout.ClientRectangle = newClientRectangle;

        int oldFirstVisibleRow = firstVisibleRow;
        base.OnResize(e);
        if (isRightToLeft() || oldFirstVisibleRow != firstVisibleRow)
            Invalidate();
    }

    internal void OnRowHeightChanged(DataGridRow row)
    {
        ClearRegionCache();
        int cy = GetRowTop(row.RowNumber);
        if (cy > 0)
        {
            Rectangle refresh = new Rectangle
            {
                Y = cy,
                X = layout.Inside.X,
                Width = layout.Inside.Width,
                Height = layout.Inside.Bottom - cy
            };
            Invalidate(refresh);
        }
    }

    internal void ParentRowsDataChanged()
    {
        Debug.Assert(originalState is not null, "how can we get a list changed event from another listmanager/list while not navigating");

        // do the reset work that is done in SetDataBindings, set_DataSource, set_DataMember;
        parentRows.Clear();
        caption.BackButtonActive = caption.DownButtonActive = caption.BackButtonVisible = false;
        caption.SetDownButtonDirection(!layout.ParentRowsVisible);
        object dSource = originalState.DataSource;
        string dMember = originalState.DataMember;
        // we don't need to set the GRIDSTATE_metaDataChanged bit, cause
        // the listManager from the originalState should be different from the current listManager
        // set the originalState to null so that Set_ListManager knows that
        // it has to unhook the MetaDataChanged events
        originalState = null;
        Set_ListManager(dSource, dMember, true);
    }

    private void AbortEdit()
    {
        Debug.WriteLineIf(CompModSwitches.DataGridEditing.TraceVerbose, "DataGridEditing: \t! AbortEdit");
        Debug.Assert(gridState[GRIDSTATE_isEditing], "Can't abort an edit that is not happening!");

        // the same rules from editColumn.OnEdit
        // while changing the editControl's visibility, do not
        // PerformLayout on the entire DataGrid
        gridState[GRIDSTATE_editControlChanging] = true;

        editColumn.Abort(editRow.RowNumber);

        // reset the editControl flag:
        gridState[GRIDSTATE_editControlChanging] = false;

        gridState[GRIDSTATE_isEditing] = false;
        editRow = null;
        editColumn = null;
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event NavigateEventHandler Navigate
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    protected event EventHandler RowHeaderClick
    {
        add
        {
            onRowHeaderClick += value;
        }
        remove
        {
            onRowHeaderClick -= value;
        }
    }

    internal event EventHandler NodeClick
    {
        add
        {
            Events.AddHandler(EVENT_NODECLICKED, value);
        }
        remove
        {
            Events.RemoveHandler(EVENT_NODECLICKED, value);
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler Scroll
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override ISite Site
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    internal void AddNewRow()
    {
        EnsureBound();
        ResetSelection();
        // EndEdit();
        UpdateListManager();
        gridState[GRIDSTATE_inListAddNew] = true;
        gridState[GRIDSTATE_inAddNewRow] = true;
        try
        {
            ListManager.AddNew();
        }
        catch
        {
            gridState[GRIDSTATE_inListAddNew] = false;
            gridState[GRIDSTATE_inAddNewRow] = false;
            PerformLayout();
            InvalidateInside();
            throw;
        }

        gridState[GRIDSTATE_inListAddNew] = false;
    }

    public bool BeginEdit(DataGridColumnStyle gridColumn, int rowNumber)
    {
        throw new PlatformNotSupportedException();
    }

    public void BeginInit()
    {
        throw new PlatformNotSupportedException();
    }

    private Rectangle CalcRowResizeFeedbackRect(MouseEventArgs e)
    {
        Rectangle inside = layout.Data;
        Rectangle r = new Rectangle(inside.X, e.Y, inside.Width, 3);
        r.Y = Math.Min(inside.Bottom - 3, r.Y);
        r.Y = Math.Max(r.Y, 0);
        return r;
    }

    private Rectangle CalcColResizeFeedbackRect(MouseEventArgs e)
    {
        Rectangle inside = layout.Data;
        Rectangle r = new Rectangle(e.X, inside.Y, 3, inside.Height);
        r.X = Math.Min(inside.Right - 3, r.X);
        r.X = Math.Max(r.X, 0);
        return r;
    }

    private void CancelCursorUpdate()
    {
        Debug.WriteLineIf(CompModSwitches.DataGridCursor.TraceVerbose, "DataGridCursor: Requesting CancelEdit()");
        if (listManager is not null)
        {
            EndEdit();
            listManager.CancelCurrentEdit();
        }
    }

    private void CheckHierarchyState()
    {
        if (checkHierarchy && listManager is not null && myGridTable is not null)
        {
            if (myGridTable is null)
                return;

            for (int j = 0; j < myGridTable.GridColumnStyles.Count; j++)
            {
                DataGridColumnStyle gridColumn = myGridTable.GridColumnStyles[j];
            }

            checkHierarchy = false;
        }
    }

    /// <devdoc>
    ///      The DataGrid caches an array of rectangular areas
    ///      which represent the area which scrolls left to right.
    ///      This method is invoked whenever the DataGrid's
    ///      scrollable regions change in such a way as to require
    ///      a re-recalculation.
    /// </devdoc>
    private void ClearRegionCache()
    {
        cachedScrollableRegion = null;
    }

    /// <devdoc>
    ///      Determines the best fit size for the given column.
    /// </devdoc>
    private void ColAutoResize(int col)
    {
        Debug.WriteLineIf(CompModSwitches.DataGridLayout.TraceVerbose, "DataGridLayout: ColAutoResize");
        EndEdit();
        CurrencyManager listManager = this.listManager;
        if (listManager is null)
            return;

        int size;
        Graphics g = CreateGraphicsInternal();
        try
        {
            DataGridColumnStyle column = myGridTable.GridColumnStyles[col];
            string columnName = column.HeaderText;

            Font headerFont;
            if (myGridTable.IsDefault)
                headerFont = HeaderFont;
            else
                headerFont = myGridTable.HeaderFont;
            size = (int)g.MeasureString(columnName, headerFont).Width + layout.ColumnHeaders.Height + 1; // This is not a bug, the sort triangle's width is equal to it's height.
            int rowCount = listManager.Count;
            for (int row = 0; row < rowCount; ++row)
            {
                object value = column.GetColumnValueAtRow(listManager, row);
                int width = 0;
                if (width > size)
                    size = width;
            }

            if (column.Width != size)
            {
                column.width = size;

                ComputeVisibleColumns();

                bool lastColumnIsLastTotallyVisibleCol = true;
                if (lastTotallyVisibleCol != -1)
                {
                    for (int i = lastTotallyVisibleCol + 1; i < myGridTable.GridColumnStyles.Count; i++)
                    {
                        if (myGridTable.GridColumnStyles[i].PropertyDescriptor is not null)
                        {
                            lastColumnIsLastTotallyVisibleCol = false;
                            break;
                        }
                    }
                }
                else
                {
                    lastColumnIsLastTotallyVisibleCol = false;
                }

                // if the column shrank and the last totally visible column was the last column
                // then we need to recompute the horizontalOffset, firstVisibleCol, negOffset.
                // lastTotallyVisibleCol remains the last column
                if (lastColumnIsLastTotallyVisibleCol &&
                    (negOffset != 0 || horizontalOffset != 0))
                {
                    // update the column width
                    column.width = size;

                    int cx = 0;
                    int colCount = myGridTable.GridColumnStyles.Count;
                    int visibleWidth = layout.Data.Width;
                    GridColumnStylesCollection cols = myGridTable.GridColumnStyles;

                    // assume everything fits
                    negOffset = 0;
                    horizontalOffset = 0;
                    firstVisibleCol = 0;

                    for (int i = colCount - 1; i >= 0; i--)
                    {
                        if (cols[i].PropertyDescriptor is null)
                        {
                            continue;
                        }

                        cx += cols[i].Width;
                        if (cx > visibleWidth)
                        {
                            if (negOffset == 0)
                            {
                                firstVisibleCol = i;
                                negOffset = cx - visibleWidth;
                                horizontalOffset = negOffset;
                                numVisibleCols++;
                            }
                            else
                            {
                                horizontalOffset += cols[i].Width;
                            }
                        }
                        else
                        {
                            numVisibleCols++;
                        }
                    }

                    // refresh the horizontal scrollbar
                    PerformLayout();

                    // we need to invalidate the layout.Data and layout.ColumnHeaders
                    Invalidate(Rectangle.Union(layout.Data, layout.ColumnHeaders));
                }
                else
                {
                    // need to refresh the scroll bar
                    PerformLayout();

                    Rectangle rightArea = layout.Data;
                    if (layout.ColumnHeadersVisible)
                        rightArea = Rectangle.Union(rightArea, layout.ColumnHeaders);

                    int left = GetColBeg(col);

                    if (!isRightToLeft())
                    {
                        rightArea.Width -= left - rightArea.X;
                        rightArea.X = left;
                    }
                    else
                    {
                        rightArea.Width = rightArea.Width - left;
                    }

                    Invalidate(rightArea);
                }
            }
        }
        finally
        {
            g.Dispose();
        }

        if (horizScrollBar.Visible)
        {
            horizScrollBar.Value = HorizontalOffset;
        }
    }

    public void Collapse(int row)
    {
        throw new PlatformNotSupportedException();
    }

    private void ColResizeBegin(MouseEventArgs e, int col)
    {
        Debug.WriteLineIf(CompModSwitches.DataGridLayout.TraceVerbose, "DataGridLayout: ColResizeBegin");
        Debug.Assert(myGridTable is not null, "Column resizing operations can't be called when myGridTable is null.");

        int x = e.X;
        EndEdit();
        Rectangle clip = Rectangle.Union(layout.ColumnHeaders, layout.Data);
        if (isRightToLeft())
        {
            clip.Width = GetColBeg(col) - layout.Data.X - 2;
        }
        else
        {
            int leftEdge = GetColBeg(col);
            clip.X = leftEdge + 3;
            clip.Width = layout.Data.X + layout.Data.Width - leftEdge - 2;
        }

        gridState[GRIDSTATE_trackColResize] = true;
        trackColAnchor = x;
        trackColumn = col;

        DrawColSplitBar(e);
        lastSplitBar = e;
    }

    private void ColResizeMove(MouseEventArgs e)
    {
        if (lastSplitBar is not null)
        {
            DrawColSplitBar(lastSplitBar);
            lastSplitBar = e;
        }

        DrawColSplitBar(e);
    }

    private void ColResizeEnd(MouseEventArgs e)
    {
        Debug.WriteLineIf(CompModSwitches.DataGridLayout.TraceVerbose, "DataGridLayout: ColResizeEnd");
        Debug.Assert(myGridTable is not null, "Column resizing operations can't be called when myGridTable is null.");

        gridState[GRIDSTATE_layoutSuspended] = true;
        try
        {
            if (lastSplitBar is not null)
            {
                DrawColSplitBar(lastSplitBar);
                lastSplitBar = null;
            }

            bool rightToLeft = isRightToLeft();

            int x = rightToLeft ? Math.Max(e.X, layout.Data.X) : Math.Min(e.X, layout.Data.Right + 1);
            int delta = x - GetColEnd(trackColumn);
            if (rightToLeft)
                delta = -delta;

            if (trackColAnchor != x && delta != 0)
            {
                DataGridColumnStyle column = myGridTable.GridColumnStyles[trackColumn];
                int proposed = column.Width + delta;
                proposed = Math.Max(proposed, 3);
                column.Width = proposed;

                // refresh scrolling data: horizontalOffset, negOffset, firstVisibleCol, numVisibleCols, lastTotallyVisibleCol
                ComputeVisibleColumns();

                bool lastColumnIsLastTotallyVisibleCol = true;
                for (int i = lastTotallyVisibleCol + 1; i < myGridTable.GridColumnStyles.Count; i++)
                {
                    if (myGridTable.GridColumnStyles[i].PropertyDescriptor is not null)
                    {
                        lastColumnIsLastTotallyVisibleCol = false;
                        break;
                    }
                }

                if (lastColumnIsLastTotallyVisibleCol &&
                    (negOffset != 0 || horizontalOffset != 0))
                {
                    int cx = 0;
                    int colCount = myGridTable.GridColumnStyles.Count;
                    int visibleWidth = layout.Data.Width;
                    GridColumnStylesCollection cols = myGridTable.GridColumnStyles;

                    // assume everything fits
                    negOffset = 0;
                    horizontalOffset = 0;
                    firstVisibleCol = 0;

                    for (int i = colCount - 1; i > -1; i--)
                    {
                        if (cols[i].PropertyDescriptor is null)
                        {
                            continue;
                        }

                        cx += cols[i].Width;

                        if (cx > visibleWidth)
                        {
                            if (negOffset == 0)
                            {
                                negOffset = cx - visibleWidth;
                                firstVisibleCol = i;
                                horizontalOffset = negOffset;
                                numVisibleCols++;
                            }
                            else
                            {
                                horizontalOffset += cols[i].Width;
                            }
                        }
                        else
                        {
                            numVisibleCols++;
                        }
                    }

                    // and invalidate pretty much everything
                    Invalidate(Rectangle.Union(layout.Data, layout.ColumnHeaders));
                }
                else
                {
                    Rectangle rightArea = Rectangle.Union(layout.ColumnHeaders, layout.Data);
                    int left = GetColBeg(trackColumn);
                    rightArea.Width -= rightToLeft ? rightArea.Right - left : left - rightArea.X;
                    rightArea.X = rightToLeft ? layout.Data.X : left;
                    Invalidate(rightArea);
                }
            }
        }
        finally
        {
            gridState[GRIDSTATE_layoutSuspended] = false;
        }

        PerformLayout();

        if (horizScrollBar.Visible)
        {
            horizScrollBar.Value = HorizontalOffset;
        }
    }

    private void MetaDataChanged()
    {
        // when we reset the Binding in the grid, we need to clear the parent rows.
        // the same goes for all the caption UI: reset it when the datasource changes.
        parentRows.Clear();
        caption.BackButtonActive = caption.DownButtonActive = caption.BackButtonVisible = false;
        caption.SetDownButtonDirection(!layout.ParentRowsVisible);

        gridState[GRIDSTATE_metaDataChanged] = true;
        try
        {
            if (originalState is not null)
            {
                // set the originalState to null so that Set_ListManager knows that
                // it has to unhook the MetaDataChanged events
                Set_ListManager(originalState.DataSource, originalState.DataMember, true);
                originalState = null;
            }
            else
            {
                Set_ListManager(DataSource, DataMember, true);
            }
        }
        finally
        {
            gridState[GRIDSTATE_metaDataChanged] = false;
        }
    }

    private void RowAutoResize(int row)
    {
        Debug.WriteLineIf(CompModSwitches.DataGridLayout.TraceVerbose, "DataGridLayout: RowAutoResize");

        EndEdit();
        CurrencyManager listManager = ListManager;
        if (listManager is null)
            return;

        Graphics g = CreateGraphicsInternal();
        try
        {
            GridColumnStylesCollection columns = myGridTable.GridColumnStyles;
            DataGridRow resizeRow = DataGridRows[row];
            int rowCount = listManager.Count;
            int resizeHeight = 0;

            // compute the height that we should resize to:
            int columnsCount = columns.Count;
            for (int col = 0; col < columnsCount; col++)
            {
                object value = columns[col].GetColumnValueAtRow(listManager, row);
                resizeHeight = 0;
            }

            if (resizeRow.Height != resizeHeight)
            {
                resizeRow.Height = resizeHeight;

                // needed to refresh scrollbar properties
                PerformLayout();

                Rectangle rightArea = layout.Data;
                if (layout.RowHeadersVisible)
                    rightArea = Rectangle.Union(rightArea, layout.RowHeaders);
                int top = GetRowTop(row);
                rightArea.Height -= rightArea.Y - top;
                rightArea.Y = top;
                Invalidate(rightArea);
            }
        }
        finally
        {
            g.Dispose();
        }

        return;
    }

    private void RowResizeBegin(MouseEventArgs e, int row)
    {
        Debug.WriteLineIf(CompModSwitches.DataGridLayout.TraceVerbose, "DataGridLayout: RowResizeBegin");
        Debug.Assert(myGridTable is not null, "Row resizing operations can't be called when myGridTable is null.");

        int y = e.Y;
        EndEdit();
        Rectangle clip = Rectangle.Union(layout.RowHeaders, layout.Data);
        int topEdge = GetRowTop(row);
        clip.Y = topEdge + 3;
        clip.Height = layout.Data.Y + layout.Data.Height - topEdge - 2;

        gridState[GRIDSTATE_trackRowResize] = true;
        trackRowAnchor = y;
        trackRow = row;

        DrawRowSplitBar(e);
        lastSplitBar = e;
    }

    private void RowResizeMove(MouseEventArgs e)
    {
        if (lastSplitBar is not null)
        {
            DrawRowSplitBar(lastSplitBar);
            lastSplitBar = e;
        }

        DrawRowSplitBar(e);
    }

    private void RowResizeEnd(MouseEventArgs e)
    {
        Debug.WriteLineIf(CompModSwitches.DataGridLayout.TraceVerbose, "DataGridLayout: RowResizeEnd");
        Debug.Assert(myGridTable is not null, "Row resizing operations can't be called when myGridTable is null.");

        try
        {
            if (lastSplitBar is not null)
            {
                DrawRowSplitBar(lastSplitBar);
                lastSplitBar = null;
            }

            int y = Math.Min(e.Y, layout.Data.Y + layout.Data.Height + 1);
            int delta = y - GetRowBottom(trackRow);
            if (trackRowAnchor != y && delta != 0)
            {
                DataGridRow row = DataGridRows[trackRow];
                int proposed = row.Height + delta;
                proposed = Math.Max(proposed, 3);
                row.Height = proposed;

                // needed to refresh scrollbar properties
                PerformLayout();

                Rectangle rightArea = Rectangle.Union(layout.RowHeaders, layout.Data);
                int top = GetRowTop(trackRow);
                rightArea.Height -= rightArea.Y - top;
                rightArea.Y = top;
                Invalidate(rightArea);
            }
        }
        finally
        {
        }
    }

    /// <devdoc>
    ///      Fires the ColumnHeaderClicked event and handles column
    ///      sorting.
    /// </devdoc>
    private void ColumnHeaderClicked(PropertyDescriptor prop)
    {
        if (!CommitEdit())
            return;

        // OnColumnHeaderClick(EventArgs.Empty);
        bool allowSorting;
        if (myGridTable.IsDefault)
            allowSorting = AllowSorting;
        else
            allowSorting = myGridTable.AllowSorting;

        if (!allowSorting)
            return;

        // if (CompModSwitches.DataGridCursor.OutputVerbose) Debug.WriteLine("DataGridCursor: We are about to sort column " + col.ToString());
        ListSortDirection direction = ListManager.GetSortDirection();
        PropertyDescriptor sortColumn = ListManager.GetSortProperty();
        if (sortColumn is not null && sortColumn.Equals(prop))
            direction = (direction == ListSortDirection.Ascending) ? ListSortDirection.Descending : ListSortDirection.Ascending;
        else
            // defaultSortDirection : ascending
            direction = ListSortDirection.Ascending;

        if (listManager.Count == 0)
            return;

        ListManager.SetSort(prop, direction);
        ResetSelection();

        InvalidateInside();
    }

    /// <devdoc>
    ///      Attempts to commit editing if a cell is being edited.
    ///      Return true if successfully commited editing.
    ///      Return false if editing can not be completed and the gird must
    ///      remain in our current Edit state.
    /// </devdoc>
    private bool CommitEdit()
    {
        Debug.WriteLineIf(CompModSwitches.DataGridEditing.TraceVerbose, "DataGridEditing: \t  CommitEdit " + (editRow is null ? "" : editRow.RowNumber.ToString(CultureInfo.InvariantCulture)));
        // we want to commit the editing if
        // 1. the user was editing or navigating around the data grid and
        // 2. this is not the result of moving focus inside the data grid and
        // 3. if the user was scrolling
        if ((!gridState[GRIDSTATE_isEditing] && !gridState[GRIDSTATE_isNavigating]) || (gridState[GRIDSTATE_editControlChanging] && !gridState[GRIDSTATE_isScrolling]))
            return true;

        // the same rules from editColumn.OnEdit
        // flag that we are editing the Edit control, so if we get a OnLayout on the
        // datagrid side of things while the edit control changes its visibility and bounds
        // the datagrid does not perform a layout
        gridState[GRIDSTATE_editControlChanging] = true;

        if (editColumn.ReadOnly || gridState[GRIDSTATE_inAddNewRow])
        {
            bool focusTheGrid = false;
            if (ContainsFocus)
            {
                focusTheGrid = true;
            }

            if (focusTheGrid && gridState[GRIDSTATE_canFocus])
                FocusInternal();
            editColumn.ConcedeFocus();

            // set the focus back to the grid
            if (focusTheGrid && gridState[GRIDSTATE_canFocus] && CanFocus && !Focused)
                FocusInternal();

            // reset the editControl flag
            gridState[GRIDSTATE_editControlChanging] = false;
            return true;
        }

        bool retVal = editColumn.Commit(ListManager, currentRow);

        // reset the editControl flag
        gridState[GRIDSTATE_editControlChanging] = false;

        if (retVal)
            gridState[GRIDSTATE_isEditing] = false;

        return retVal;
    }

    /// <devdoc>
    ///      Figure out how many rows we need to scroll down
    ///      to move targetRow into visibility.
    /// </devdoc>
    private int ComputeDeltaRows(int targetRow)
    {
        if (firstVisibleRow == targetRow)
            return 0;

        int dRows = 0;
        int firstVisibleRowLogicalTop = -1;
        int targetRowLogicalTop = -1;
        int nRows = DataGridRowsLength;
        int cy = 0;
        DataGridRow[] localGridRows = DataGridRows;

        for (int row = 0; row < nRows; ++row)
        {
            if (row == firstVisibleRow)
                firstVisibleRowLogicalTop = cy;
            if (row == targetRow)
                targetRowLogicalTop = cy;
            if (targetRowLogicalTop != -1 && firstVisibleRowLogicalTop != -1)
                break;
            cy += localGridRows[row].Height;
        }

        int targetRowLogicalBottom = targetRowLogicalTop + localGridRows[targetRow].Height;
        int dataLogicalBottom = layout.Data.Height + firstVisibleRowLogicalTop;
        if (targetRowLogicalBottom > dataLogicalBottom)
        {
            // we need to move down.
            int downDelta = targetRowLogicalBottom - dataLogicalBottom;
            firstVisibleRowLogicalTop += downDelta;
        }
        else if (firstVisibleRowLogicalTop < targetRowLogicalTop)
        {
            // we don't need to move
            return 0;
        }
        else
        {
            // we need to move up.
            int upDelta = firstVisibleRowLogicalTop - targetRowLogicalTop;
            firstVisibleRowLogicalTop -= upDelta;
        }

        int newFirstRow = ComputeFirstVisibleRow(firstVisibleRowLogicalTop);
        dRows = (newFirstRow - firstVisibleRow);
        return dRows;
    }

    /// <devdoc>
    ///      Given the a logical vertical offset, figure out
    ///      which row number should be the first fully visible
    ///      row on or after the offset.
    /// </devdoc>
    private int ComputeFirstVisibleRow(int firstVisibleRowLogicalTop)
    {
        int first;
        int nRows = DataGridRowsLength;
        int cy = 0;
        DataGridRow[] localGridRows = DataGridRows;
        for (first = 0; first < nRows; ++first)
        {
            if (cy >= firstVisibleRowLogicalTop)
                break;
            cy += localGridRows[first].Height;
        }

        return first;
    }

    /// <devdoc>
    ///      Constructs an updated Layout object.
    /// </devdoc>
    private void ComputeLayout()
    {
        Debug.WriteLineIf(CompModSwitches.DataGridLayout.TraceVerbose, "DataGridLayout: ComputeLayout");

        bool alignLeft = !isRightToLeft();
        Rectangle oldResizeRect = layout.ResizeBoxRect;

        // hide the EditBox
        EndEdit();

        ClearRegionCache();

        // NOTE : Since Rectangles are structs, then assignment is a
        //      : copy. Therefore, after saying "Rectangle inside = newLayout.Inside",
        //      : we must always assign back to newLayout.Inside.
        //

        // Important since all of the visibility flags will move
        // to the new layout here.
        LayoutData newLayout = new LayoutData(layout);

        // Inside
        newLayout.Inside = ClientRectangle;
        Rectangle inside = newLayout.Inside;
        int borderWidth = BorderWidth;
        inside.Inflate(-borderWidth, -borderWidth);

        Rectangle insideLeft = inside;

        // Caption
        if (layout.CaptionVisible)
        {
            int captionHeight = captionFontHeight + 6;
            Rectangle cap = newLayout.Caption;
            cap = insideLeft;
            cap.Height = captionHeight;
            insideLeft.Y += captionHeight;
            insideLeft.Height -= captionHeight;

            newLayout.Caption = cap;
        }
        else
        {
            newLayout.Caption = Rectangle.Empty;
        }

        // Parent Rows
        if (layout.ParentRowsVisible)
        {
            Rectangle parents = newLayout.ParentRows;
            int parentHeight = parentRows.Height;
            parents = insideLeft;
            parents.Height = parentHeight;
            insideLeft.Y += parentHeight;
            insideLeft.Height -= parentHeight;

            newLayout.ParentRows = parents;
        }
        else
        {
            newLayout.ParentRows = Rectangle.Empty;
        }

        // Headers
        //
        int columnHeaderHeight = headerFontHeight + 6;
        if (layout.ColumnHeadersVisible)
        {
            Rectangle colHeaders = newLayout.ColumnHeaders;
            colHeaders = insideLeft;
            colHeaders.Height = columnHeaderHeight;
            insideLeft.Y += columnHeaderHeight;
            insideLeft.Height -= columnHeaderHeight;

            newLayout.ColumnHeaders = colHeaders;
        }
        else
        {
            newLayout.ColumnHeaders = Rectangle.Empty;
        }

        bool newRowHeadersVisible = myGridTable.IsDefault ? RowHeadersVisible : myGridTable.RowHeadersVisible;
        int newRowHeaderWidth = myGridTable.IsDefault ? RowHeaderWidth : myGridTable.RowHeaderWidth;
        newLayout.RowHeadersVisible = newRowHeadersVisible;
        if (myGridTable is not null && newRowHeadersVisible)
        {
            Rectangle rowHeaders = newLayout.RowHeaders;
            if (alignLeft)
            {
                rowHeaders = insideLeft;
                rowHeaders.Width = newRowHeaderWidth;
                insideLeft.X += newRowHeaderWidth;
                insideLeft.Width -= newRowHeaderWidth;
            }
            else
            {
                rowHeaders = insideLeft;
                rowHeaders.Width = newRowHeaderWidth;
                rowHeaders.X = insideLeft.Right - newRowHeaderWidth;
                insideLeft.Width -= newRowHeaderWidth;
            }

            newLayout.RowHeaders = rowHeaders;

            if (layout.ColumnHeadersVisible)
            {
                Rectangle topLeft = newLayout.TopLeftHeader;
                Rectangle colHeaders = newLayout.ColumnHeaders;
                if (alignLeft)
                {
                    topLeft = colHeaders;
                    topLeft.Width = newRowHeaderWidth;
                    colHeaders.Width -= newRowHeaderWidth;
                    colHeaders.X += newRowHeaderWidth;
                }
                else
                {
                    topLeft = colHeaders;
                    topLeft.Width = newRowHeaderWidth;
                    topLeft.X = colHeaders.Right - newRowHeaderWidth;
                    colHeaders.Width -= newRowHeaderWidth;
                }

                newLayout.TopLeftHeader = topLeft;
                newLayout.ColumnHeaders = colHeaders;
            }
            else
            {
                newLayout.TopLeftHeader = Rectangle.Empty;
            }
        }
        else
        {
            newLayout.RowHeaders = Rectangle.Empty;
            newLayout.TopLeftHeader = Rectangle.Empty;
        }

        // The Data region
        newLayout.Data = insideLeft;
        newLayout.Inside = inside;

        layout = newLayout;

        LayoutScrollBars();

        // if the user shrank the grid client area, then OnResize invalidated the old
        // resize area. however, we need to invalidate the left upper corner in the new ResizeArea
        // note that we can't take the Invalidate call from the OnResize method, because if the
        // user enlarges the form then the old area will not be invalidated.
        if (!oldResizeRect.Equals(layout.ResizeBoxRect) && !layout.ResizeBoxRect.IsEmpty)
            Invalidate(layout.ResizeBoxRect);

        layout.dirty = false;
        Debug.WriteLineIf(CompModSwitches.DataGridLayout.TraceVerbose, "DataGridLayout: " + layout.ToString());
    }

    /// <devdoc>
    ///      Computes the number of pixels to scroll to scroll from one
    ///      row to another.
    /// </devdoc>
    private int ComputeRowDelta(int from, int to)
    {
        int first = from;
        int last = to;
        int sign = -1;
        if (first > last)
        {
            first = to;
            last = from;
            sign = 1;
        }

        DataGridRow[] localGridRows = DataGridRows;
        int delta = 0;
        for (int row = first; row < last; ++row)
        {
            delta += localGridRows[row].Height;
        }

        return sign * delta;
    }

    internal int MinimumRowHeaderWidth()
    {
        return minRowHeaderWidth;
    }

    internal void ComputeMinimumRowHeaderWidth()
    {
        minRowHeaderWidth = errorRowBitmapWidth; // the size of the pencil, star and row selector images are the same as the image for the error bitmap
        if (ListHasErrors)
            minRowHeaderWidth += errorRowBitmapWidth;
        if (myGridTable is not null && myGridTable.RelationsList.Count != 0)
            minRowHeaderWidth += 15; // the size of the plus/minus glyph and spacing around it
    }

    /// <internalonly/>
    /// <devdoc>
    ///      Updates the internal variables with the number of columns visible
    ///      inside the Grid's client rectangle.
    /// </devdoc>
    private void ComputeVisibleColumns()
    {
        Debug.WriteLineIf(CompModSwitches.DataGridLayout.TraceVerbose, "DataGridLayout: ComputeVisibleColumns");
        EnsureBound();

        GridColumnStylesCollection columns = myGridTable.GridColumnStyles;

        int nGridCols = columns.Count;
        int cx = -negOffset;
        int visibleColumns = 0;
        int visibleWidth = layout.Data.Width;
        int curCol = firstVisibleCol;

        // the same problem with negative numbers:
        // if the width passed in is negative, then return 0
        //
        // added the check for the columns.Count == 0 ( danielhe, November 14, 2000)
        //
        if (visibleWidth < 0 || columns.Count == 0)
        {
            numVisibleCols = firstVisibleCol = 0;
            lastTotallyVisibleCol = -1;
            return;
        }

        while (cx < visibleWidth && curCol < nGridCols)
        {
            // if (columns.Visible && columns.PropertyDescriptor is not null)
            if (columns[curCol].PropertyDescriptor is not null)
                cx += columns[curCol].Width;
            curCol++;
            visibleColumns++;
        }

        numVisibleCols = visibleColumns;

        // if we inflate the data area
        // then we paint columns to the left of firstVisibleColumn
        if (cx < visibleWidth)
        {
            for (int i = firstVisibleCol - 1; i > 0; i--)
            {
                if (cx + columns[i].Width > visibleWidth)
                    break;
                // if (columns.Visible && columns.PropertyDescriptor is not null)
                if (columns[i].PropertyDescriptor is not null)
                    cx += columns[i].Width;
                visibleColumns++;
                firstVisibleCol--;
            }

            if (numVisibleCols != visibleColumns)
            {
                Debug.Assert(numVisibleCols < visibleColumns, "the number of visible columns can only grow");
                // is there space for more columns than were visible?
                // if so, then we need to repaint Data and ColumnHeaders
                Invalidate(layout.Data);
                Invalidate(layout.ColumnHeaders);

                // update the number of visible columns to the new reality
                numVisibleCols = visibleColumns;
            }
        }

        lastTotallyVisibleCol = firstVisibleCol + numVisibleCols - 1;
        if (cx > visibleWidth)
        {
            if (numVisibleCols <= 1 || (numVisibleCols == 2 && negOffset != 0))
            {
                // no column is entirely visible
                lastTotallyVisibleCol = -1;
            }
            else
            {
                lastTotallyVisibleCol--;
            }
        }
    }

    /// <devdoc>
    ///      Determines which column is the first visible given
    ///      the object's horizontalOffset.
    /// </devdoc>
    private int ComputeFirstVisibleColumn()
    {
        int first = 0;
        if (horizontalOffset == 0)
        {
            negOffset = 0;
            return 0;
        }

        // we will check to see if myGridTables.GridColumns.Count != 0
        // because when we reset the dataGridTable, we don't have any columns, and we still
        // call HorizontalOffset = 0, and that will call ComputeFirstVisibleColumn with an empty collection of columns.
        if (myGridTable is not null && myGridTable.GridColumnStyles is not null && myGridTable.GridColumnStyles.Count != 0)
        {
            GridColumnStylesCollection columns = myGridTable.GridColumnStyles;
            int cx = 0;
            int nGridCols = columns.Count;

            if (columns[0].Width == -1)
            {
                // the columns are not initialized yet
                //
#if DEBUG
                for (int i = 0; i < nGridCols; i++)
                {
                    Debug.Assert(columns[i].Width == -1, "the columns' widths should not be initialized");
                }
#endif // DEBUG
                negOffset = 0;
                return 0;
            }

            for (first = 0; first < nGridCols; first++)
            {
                // if (columns[first].Visible && columns[first].PropertyDescriptor is not null);
                if (columns[first].PropertyDescriptor is not null)
                    cx += columns[first].Width;
                if (cx > horizontalOffset)
                    break;
            }

            // first may actually be the number of columns
            // in that case all the columns fit in the layout data
            if (first == nGridCols)
            {
                Debug.Assert(cx <= horizontalOffset, "look at the for loop before: we only exit that loop early if the cx is over the horizontal offset");
                negOffset = 0;
                return 0;
            }

            negOffset = columns[first].Width - (cx - horizontalOffset);
            // Debug.WriteLineIf(CompModSwitches.DataGridScrolling.TraceVerbose, "DataGridScrolling: ComputeFirstVisibleColumn, ret = " + first.ToString() + ", negOffset = " + negOffset.ToString());
        }

        return first;
    }

    /// <devdoc>
    ///      Updates the internal variables with the number of rows visible
    ///      in a given DataGrid Layout.
    /// </devdoc>
    private void ComputeVisibleRows()
    {
        Debug.WriteLineIf(CompModSwitches.DataGridLayout.TraceVerbose, "DataGridLayout: ComputeVisibleRows");
        EnsureBound();

        Rectangle data = layout.Data;
        int visibleHeight = data.Height;
        int cy = 0;
        int visibleRows = 0;
        DataGridRow[] localGridRows = DataGridRows;
        int numRows = DataGridRowsLength;

        // when minimizing the dataGrid window, we will get negative values for the
        // layout.Data.Width and layout.Data.Height ( is this a bug or not? if layout.Data.Height == 0 in that case,
        // the old code would have worked )
        //
        // if this is the case, then set numVisibleRows = numTotallyVisibleRows = 0;
        //
        if (visibleHeight < 0)
        {
            numVisibleRows = numTotallyVisibleRows = 0;
            return;
        }

        for (int i = firstVisibleRow; i < numRows; ++i)
        {
            if (cy > visibleHeight)
                break;
            cy += localGridRows[i].Height;
            visibleRows++;
        }

        if (cy < visibleHeight)
        {
            for (int i = firstVisibleRow - 1; i >= 0; i--)
            {
                int height = localGridRows[i].Height;
                if (cy + height > visibleHeight)
                    break;
                cy += height;
                firstVisibleRow--;
                visibleRows++;
            }
        }

        numVisibleRows = numTotallyVisibleRows = visibleRows;
        if (cy > visibleHeight)
            numTotallyVisibleRows--;

        Debug.Assert(numVisibleRows >= 0, "the number of visible rows can't be negative");
        Debug.Assert(numTotallyVisibleRows >= 0, "the number of totally visible rows can't be negative");
    }

    protected override AccessibleObject CreateAccessibilityInstance()
    {
        return new DataGridAccessibleObject(this);
    }

    private DataGridState CreateChildState(string relationName, DataGridRow source)
    {
        DataGridState dgs = new DataGridState();

        string newDataMember;
        if (string.IsNullOrEmpty(DataMember))
        {
            newDataMember = relationName;
        }
        else
        {
            newDataMember = DataMember + "." + relationName;
        }

        CurrencyManager childLM = (CurrencyManager)BindingContext[DataSource, newDataMember];

        dgs.DataSource = DataSource;
        dgs.DataMember = newDataMember;
        dgs.ListManager = childLM;

        dgs.DataGridRows = null;
        dgs.DataGridRowsLength = childLM.Count + (policy.AllowAdd ? 1 : 0);

        return dgs;
    }

    /// <devdoc>
    ///      Constructs a Layout object containing the state
    ///      for a newly constructed DataGrid.
    /// </devdoc>
    private LayoutData CreateInitialLayoutState()
    {
        Debug.WriteLineIf(CompModSwitches.DataGridLayout.TraceVerbose, "DataGridLayout: CreateInitialLayoutState");
        LayoutData newLayout = new LayoutData();
        newLayout.Inside = default;
        newLayout.TopLeftHeader = default;
        newLayout.ColumnHeaders = default;
        newLayout.RowHeaders = default;
        newLayout.Data = default;
        newLayout.Caption = default;
        newLayout.ParentRows = default;
        newLayout.ResizeBoxRect = default;
        newLayout.ColumnHeadersVisible = true;
        newLayout.RowHeadersVisible = true;
        newLayout.CaptionVisible = defaultCaptionVisible;
        newLayout.ParentRowsVisible = defaultParentRowsVisible;
        newLayout.ClientRectangle = ClientRectangle;
        return newLayout;
    }

    /// <devdoc>
    ///      The DataGrid caches an array of rectangular areas
    ///      which represent the area which scrolls left to right.
    ///      This method is invoked whenever the DataGrid needs
    ///      this scrollable region.
    /// </devdoc>
    private RECT[] CreateScrollableRegion(Rectangle scroll)
    {
        if (cachedScrollableRegion is not null)
        {
            return cachedScrollableRegion;
        }

        bool alignToRight = isRightToLeft();

        using (Region region = new Region(scroll))
        {
            int nRows = numVisibleRows;
            int cy = layout.Data.Y;
            int cx = layout.Data.X;
            DataGridRow[] localGridRows = DataGridRows;
            for (int r = firstVisibleRow; r < nRows; r++)
            {
                int rowHeight = localGridRows[r].Height;
                Rectangle rowExclude = localGridRows[r].GetNonScrollableArea();
                rowExclude.X += cx;
                rowExclude.X = MirrorRectangle(rowExclude, layout.Data, alignToRight);
                if (!rowExclude.IsEmpty)
                {
                    region.Exclude(new Rectangle(rowExclude.X,
                                                 rowExclude.Y + cy,
                                                 rowExclude.Width,
                                                 rowExclude.Height));
                }

                cy += rowHeight;
            }
        }

        return cachedScrollableRegion;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (vertScrollBar is not null)
                vertScrollBar.Dispose();
            if (horizScrollBar is not null)
                horizScrollBar.Dispose();

            if (toBeDisposedEditingControl is not null)
            {
                toBeDisposedEditingControl.Dispose();
                toBeDisposedEditingControl = null;
            }

            GridTableStylesCollection tables = TableStyles;
            if (tables is not null)
            {
#if DEBUG
                Debug.Assert(myGridTable is null || myGridTable.IsDefault || tables.Contains(myGridTable), "how come that the currentTable is not in the list of tables?");
#endif // DEBUG
                for (int i = 0; i < tables.Count; i++)
                    tables[i].Dispose();
            }
        }

        base.Dispose(disposing);
    }

    /// <devdoc>
    ///      Draws an XOR region to give UI feedback for Column Resizing.
    ///      This looks just like the Splitter control's UI when resizing.
    /// </devdoc>
    private void DrawColSplitBar(MouseEventArgs e)
    {
        Rectangle r = CalcColResizeFeedbackRect(e);
        DrawSplitBar(r);
    }

    /// <devdoc>
    ///      Draws an XOR region to give UI feedback for Row Resizing.
    ///      This looks just like the Splitter control's UI when resizing.
    /// </devdoc>
    private void DrawRowSplitBar(MouseEventArgs e)
    {
        Rectangle r = CalcRowResizeFeedbackRect(e);
        DrawSplitBar(r);
    }

    /// <devdoc>
    ///      Draws an XOR region to give UI feedback for Column/Row Resizing.
    ///      This looks just like the Splitter control's UI when resizing.
    /// </devdoc>
    private void DrawSplitBar(Rectangle r)
    {
        IntPtr parentHandle = Handle;
    }

    /// <devdoc>
    ///      Begin in-place editing of a cell.  Any editing is commited
    ///      before the new edit takes place.
    ///
    ///      This will always edit the currentCell
    ///      If you want to edit another cell than the current one, just reset CurrentCell
    /// </devdoc>
    private void Edit()
    {
        Edit(null);
    }

    private void Edit(string displayText)
    {
        EnsureBound();

        // whoever needs to call ResetSelection should not rely on
        // Edit() to call it;
        //
        // ResetSelection();

        EndEdit();

        Debug.WriteLineIf(CompModSwitches.DataGridEditing.TraceVerbose, "DataGridEditing: Edit, currentRow = " + currentRow.ToString(CultureInfo.InvariantCulture) +
                                                                       ", currentCol = " + currentCol.ToString(CultureInfo.InvariantCulture) + (displayText is not null ? ", displayText= " + displayText : ""));

        /* allow navigation even if the policy does not allow editing
        if (!policy.AllowEdit)
            return;
        */

        DataGridRow[] localGridRows = DataGridRows;

        // what do you want to edit when there are no rows?
        if (DataGridRowsLength == 0)
            return;

        localGridRows[currentRow].OnEdit();
        editRow = localGridRows[currentRow];

        // if the list has no columns, then what good is an edit?
        if (myGridTable.GridColumnStyles.Count == 0)
            return;

        // what if the currentCol does not have a propDesc?
        editColumn = myGridTable.GridColumnStyles[currentCol];
        if (editColumn.PropertyDescriptor is null)
            return;

        Rectangle cellBounds = Rectangle.Empty;
        if (currentRow < firstVisibleRow || currentRow > firstVisibleRow + numVisibleRows ||
            currentCol < firstVisibleCol || currentCol > firstVisibleCol + numVisibleCols - 1 ||
            (currentCol == firstVisibleCol && negOffset != 0))
        {
        }
        else
        {
            cellBounds = GetCellBounds(currentRow, currentCol);
        }

        gridState[GRIDSTATE_isNavigating] = true;
        gridState[GRIDSTATE_isEditing] = false;

        // once we call editColumn.Edit on a DataGridTextBoxColumn
        // the edit control will become visible, and its bounds will get set.
        // both actions cause Edit.Parent.OnLayout
        // so we flag this change, cause we don't want to PerformLayout on the entire DataGrid
        // everytime the edit column gets edited
        gridState[GRIDSTATE_editControlChanging] = true;

        // reset the editControlChanging to false
        gridState[GRIDSTATE_editControlChanging] = false;
    }

    public bool EndEdit(DataGridColumnStyle gridColumn, int rowNumber, bool shouldAbort)
    {
        throw new PlatformNotSupportedException();
    }

    /// <devdoc>
    ///      Ends any editing in progress by attempting to commit and then
    ///      aborting if not possible.
    /// </devdoc>
    private void EndEdit()
    {
        Debug.WriteLineIf(CompModSwitches.DataGridEditing.TraceVerbose, "DataGridEditing: EndEdit");

        if (!gridState[GRIDSTATE_isEditing] && !gridState[GRIDSTATE_isNavigating])
            return;

        if (!CommitEdit())
        {
            AbortEdit();
        }
    }

    // PERF: we attempt to create a ListManager for the DataSource/DateMember combination
    // we do this in order to check for a valid DataMember
    // if the check succeeds, then it means that we actully put the listManager in the BindingContext's
    // list of BindingManagers. this is fine, cause if the check succeds, then Set_ListManager
    // will be called, and this will get the listManager from the bindingManagerBase hashTable kept in the BindingContext
    //
    // this will work if the dataMember does not contain any dots ('.')
    // if the dataMember contains dots, then it will be more complicated: maybe part of the binding path
    // is valid w/ the new dataSource
    // but we can leave w/ this, cause in the designer the dataMember will be only a column name. and the DataSource/DataMember
    // properties are for use w/ the designer.
    //
    private void EnforceValidDataMember(object value)
    {
        Debug.Assert(value is not null, "we should not have a null dataSource when we want to check for a valid dataMember");
        if (DataMember is null || DataMember.Length == 0)
            return;
        if (BindingContext is null)
            return;
        //
        try
        {
            BindingManagerBase bm = BindingContext[value, dataMember];
        }
        catch
        {
            dataMember = "";
        }
    }

    internal protected virtual void ColumnStartedEditing(Rectangle bounds)
    {
        Debug.Assert(currentRow >= firstVisibleRow && currentRow <= firstVisibleRow + numVisibleRows, "how can one edit a row which is invisible?");
        DataGridRow[] localGridRows = DataGridRows;

        if (bounds.IsEmpty && editColumn is DataGridTextBoxColumn && currentRow != -1 && currentCol != -1)
        {
            // set the bounds on the control
            // this will only work w/ our DataGridTexBox control
            DataGridTextBoxColumn col = editColumn as DataGridTextBoxColumn;
            Rectangle editBounds = GetCellBounds(currentRow, currentCol);

            gridState[GRIDSTATE_editControlChanging] = true;
            try
            {
                col.TextBox.Bounds = editBounds;
            }
            finally
            {
                gridState[GRIDSTATE_editControlChanging] = false;
            }
        }

        if (gridState[GRIDSTATE_inAddNewRow])
        {
            int currentRowCount = DataGridRowsLength;
            DataGridRow[] newDataGridRows = new DataGridRow[currentRowCount + 1];
            for (int i = 0; i < currentRowCount; i++)
            {
                newDataGridRows[i] = localGridRows[i];
            }

            // put the AddNewRow
            newDataGridRows[currentRowCount] = new DataGridAddNewRow(this, myGridTable, currentRowCount);
            SetDataGridRows(newDataGridRows, currentRowCount + 1);

            Edit();
            // put this after the call to edit so that
            // CommitEdit knows that the inAddNewRow is true;
            gridState[GRIDSTATE_inAddNewRow] = false;
            gridState[GRIDSTATE_isEditing] = true;
            gridState[GRIDSTATE_isNavigating] = false;
            return;
        }

        gridState[GRIDSTATE_isEditing] = true;
        gridState[GRIDSTATE_isNavigating] = false;
        InvalidateRowHeader(currentRow);

        // tell the current row to lose the childFocuse
        localGridRows[currentRow].LoseChildFocus(layout.RowHeaders, isRightToLeft());
    }

    internal protected virtual void ColumnStartedEditing(Control editingControl)
    {
        ColumnStartedEditing(editingControl.Bounds);
    }

    public void Expand(int row)
    {
        throw new PlatformNotSupportedException();
    }

    protected virtual DataGridColumnStyle CreateGridColumn(PropertyDescriptor prop, bool isDefault)
    {
        return myGridTable is null ? null : myGridTable.CreateGridColumn(prop, isDefault);
    }

    protected virtual DataGridColumnStyle CreateGridColumn(PropertyDescriptor prop)
    {
        return myGridTable is null ? null : myGridTable.CreateGridColumn(prop);
    }

#if PARENT_LINKS
    
            private ListManager ListManagerForChildColumn(ListManager childListManager, PropertyDescriptor prop)
            {
                /*
                DataKey key;
                RelationsCollection relCollection = dataColumn.Table.ParentRelations;
                */
    
                // this will give us the list of properties of the child
                PropertyDescriptorCollection propCollection = childListManager.GetItemProperties();
    
                int relCount = propCollection.Count;
                for (int i=0;i<relCount;i++)
                {
                    PropertyDescriptor currRelation = propCollection[i];
                    if (typeof(IList).IsAssignableFrom(currRelation.PropertyType))
                    {
                        object childRelation = currRelation.GetValue(childListManager.Current);
                        ListManager childTable = this.BindingContexgt is null ? null : this.BindingManager[childRelation, ""];
                        if (childTable is null)
                            continue;
    
                        // now loop thru all properties in the childTable...
                        PropertyDescriptorCollection childProps = childTable.GetItemProperties();
                        int colCount = childProps.Count;
                        for (int j = 0; j < colCount; j++)
                        {
                            PropertyDescriptor currCol = childProps[j];
                            if (currCol.Name is not null && currCol.Name.Equals(prop.Name))
                                // there is a column in the parent with the same name
                                return childTable;
                        }
                    }
    
                    /*
                    DataColumn[] childCols = relCollection[i].ChildColumns;
                    DataColumn[] parentCols = relCollection[i].ParentColumns;
    
                    // if the relationship involves more than one column in the child
                    // table, then look at the next relationship
                    if (childCols.Count != 1)
                        continue;
    
                    // if the relationship involves more than one column in the parent 
                    // table, then look at the next relationship
                    if (parentCols.Count != 1)
                        continue;
    
                    if (childCols[0] == dataColumn)
                    {
                        dataColumn = parentCols[0];
                        return new DataSource(relCollection[i].ParentTable);
                    }
                    */
                }
    
                return null;
            }
    
#endif

    public void EndInit()
    {
        throw new PlatformNotSupportedException();
    }

    /// <devdoc>
    ///      Given a x coordinate, returns the column it is over.
    /// </devdoc>
    private int GetColFromX(int x)
    {
        if (myGridTable is null)
            return -1;

        Rectangle inside = layout.Data;
        Debug.Assert(x >= inside.X && x < inside.Right, "x must be inside the horizontal bounds of layout.Data");

        x = MirrorPoint(x, inside, isRightToLeft());

        GridColumnStylesCollection columns = myGridTable.GridColumnStyles;
        int columnCount = columns.Count;

        int cx = inside.X - negOffset;
        int col = firstVisibleCol;
        while (cx < inside.Width + inside.X && col < columnCount)
        {
            // if (columns[col].Visible && columns[col].PropertyDescriptor is not null)
            if (columns[col].PropertyDescriptor is not null)
                cx += columns[col].Width;
            if (cx > x)
                return col;
            ++col;
        }

        return -1;
    }

    /// <devdoc>
    ///      Returns the coordinate of the left edge of the given column
    ///      Bi-Di: if the grid has the RightToLeft property set to RightToLeft.Yes, this will
    ///      return what appears as the right edge of the column
    /// </devdoc>
    internal int GetColBeg(int col)
    {
        Debug.Assert(myGridTable is not null, "GetColBeg can't be called when myGridTable is null.");

        int offset = layout.Data.X - negOffset;
        GridColumnStylesCollection columns = myGridTable.GridColumnStyles;

        int lastCol = Math.Min(col, columns.Count);
        for (int i = firstVisibleCol; i < lastCol; ++i)
        {
            // if (columns[i].Visible && columns[i].PropertyDescriptor is not null)
            if (columns[i].PropertyDescriptor is not null)
                offset += columns[i].Width;
        }

        return MirrorPoint(offset, layout.Data, isRightToLeft());
    }

    /// <devdoc>
    ///      Returns the coordinate of the right edge of the given column
    ///      Bi-Di: if the grid has the RightToLeft property set to RightToLeft.Yes, this will
    ///      return what appears as the left edge of the column
    /// </devdoc>
    internal int GetColEnd(int col)
    {
        int colBeg = GetColBeg(col);
        Debug.Assert(myGridTable.GridColumnStyles[col].PropertyDescriptor is not null, "why would we need the coordinate of a column that is not visible?");
        int width = myGridTable.GridColumnStyles[col].Width;
        return isRightToLeft() ? colBeg - width : colBeg + width;
    }

    private int GetColumnWidthSum()
    {
        int sum = 0;
        if (myGridTable is not null && myGridTable.GridColumnStyles is not null)
        {
            GridColumnStylesCollection columns = myGridTable.GridColumnStyles;

            int columnsCount = columns.Count;
            for (int i = 0; i < columnsCount; i++)
                if (columns[i].PropertyDescriptor is not null)
                    sum += columns[i].Width;
        }

        return sum;
    }

    /// <devdoc>
    ///      Not all rows in the DataGrid are expandable,
    ///      this computes which ones are and returns an array
    ///      of references to them.
    /// </devdoc>
    private DataGridRelationshipRow[] GetExpandableRows()
    {
        int nExpandableRows = DataGridRowsLength;
        DataGridRow[] localGridRows = DataGridRows;
        if (policy.AllowAdd)
            nExpandableRows = Math.Max(nExpandableRows - 1, 0);
        DataGridRelationshipRow[] expandableRows = new DataGridRelationshipRow[nExpandableRows];
        for (int i = 0; i < nExpandableRows; i++)
            expandableRows[i] = (DataGridRelationshipRow)localGridRows[i];
        return expandableRows;
    }

    /// <devdoc>
    ///      Returns the row number underneath the given y coordinate.
    /// </devdoc>
    /// <internalonly/>
    private int GetRowFromY(int y)
    {
        Rectangle inside = layout.Data;
        Debug.Assert(y >= inside.Y && y < inside.Bottom, "y must be inside the vertical bounds of the data");

        int cy = inside.Y;
        int row = firstVisibleRow;
        int rowCount = DataGridRowsLength;
        DataGridRow[] localGridRows = DataGridRows;
        int bottom = inside.Bottom;
        while (cy < bottom && row < rowCount)
        {
            cy += localGridRows[row].Height;
            if (cy > y)
            {
                return row;
            }

            ++row;
        }

        return -1;
    }

    internal Rectangle GetRowHeaderRect()
    {
        return layout.RowHeaders;
    }

    internal Rectangle GetColumnHeadersRect()
    {
        return layout.ColumnHeaders;
    }

    /// <devdoc>
    ///      Determines where on the control's ClientRectangle a given row is
    ///      painting to.
    /// </devdoc>
    private Rectangle GetRowRect(int rowNumber)
    {
        Rectangle inside = layout.Data;
        int cy = inside.Y;
        DataGridRow[] localGridRows = DataGridRows;
        for (int row = firstVisibleRow; row <= rowNumber; ++row)
        {
            if (cy > inside.Bottom)
            {
                break;
            }

            if (row == rowNumber)
            {
                Rectangle rowRect = new Rectangle(inside.X,
                                                  cy,
                                                  inside.Width,
                                                  localGridRows[row].Height);
                if (layout.RowHeadersVisible)
                {
                    rowRect.Width += layout.RowHeaders.Width;
                    rowRect.X -= isRightToLeft() ? 0 : layout.RowHeaders.Width;
                }

                return rowRect;
            }

            cy += localGridRows[row].Height;
        }

        return Rectangle.Empty;
    }

    /// <devdoc>
    ///      Returns the coordinate of the top edge of the given row
    /// </devdoc>
    private int GetRowTop(int row)
    {
        DataGridRow[] localGridRows = DataGridRows;
        int offset = layout.Data.Y;
        int lastRow = Math.Min(row, DataGridRowsLength);
        for (int i = firstVisibleRow; i < lastRow; ++i)
        {
            offset += localGridRows[i].Height;
        }

        for (int i = firstVisibleRow; i > lastRow; i--)
        {
            offset -= localGridRows[i].Height;
        }

        return offset;
    }

    /// <devdoc>
    ///      Returns the coordinate of the bottom edge of the given row
    /// </devdoc>
    private int GetRowBottom(int row)
    {
        DataGridRow[] localGridRows = DataGridRows;

        return GetRowTop(row) + localGridRows[row].Height;
    }

    /// <devdoc>
    ///      This method is called on methods that need the grid
    ///      to be bound to a DataTable to work.
    /// </devdoc>
    private void EnsureBound()
    {
        if (!Bound)
        {
            throw new InvalidOperationException("SR.GetString(SR.DataGridUnbound)");
        }
    }

    private void EnsureVisible(int row, int col)
    {
        if (row < firstVisibleRow
            || row >= firstVisibleRow + numTotallyVisibleRows)
        {
            int dRows = ComputeDeltaRows(row);
            ScrollDown(dRows);
        }

        if (firstVisibleCol == 0 && numVisibleCols == 0 && lastTotallyVisibleCol == -1)
        {
            // no columns are displayed whatsoever
            // some sanity checks
            Debug.Assert(negOffset == 0, " no columns are displayed so the negative offset should be 0");
            return;
        }

        int previousFirstVisibleCol = firstVisibleCol;
        int previousNegOffset = negOffset;
        int previousLastTotallyVisibleCol = lastTotallyVisibleCol;

        while (col < firstVisibleCol
            || (col == firstVisibleCol && negOffset != 0)
            || (lastTotallyVisibleCol == -1 && col > firstVisibleCol)
            || (lastTotallyVisibleCol > -1 && col > lastTotallyVisibleCol))
        {
            ScrollToColumn(col);

            if (previousFirstVisibleCol == firstVisibleCol &&
                previousNegOffset == negOffset &&
                previousLastTotallyVisibleCol == lastTotallyVisibleCol)
            {
                // nothing changed since the last iteration
                // don't get into an infinite loop
                break;
            }

            previousFirstVisibleCol = firstVisibleCol;
            previousNegOffset = negOffset;
            previousLastTotallyVisibleCol = lastTotallyVisibleCol;

            // continue to scroll to the right until the scrollTo column is the totally last visible column or it is the first visible column
        }
    }

    public Rectangle GetCurrentCellBounds()
    {
        throw new PlatformNotSupportedException();
    }

    public Rectangle GetCellBounds(int row, int col)
    {
        throw new PlatformNotSupportedException();
    }

    public Rectangle GetCellBounds(DataGridCell dgc)
    {
        throw new PlatformNotSupportedException();
    }

    // UNDONE : ChrisAn, 10/27/00 - using internal hack to expose data for
    //        : Accessibility, is there a cleaner way to do this?
    internal Rectangle GetRowBounds(DataGridRow row)
    {
        Rectangle rowBounds = default;
        rowBounds.Y = GetRowTop(row.RowNumber);
        rowBounds.X = layout.Data.X;
        rowBounds.Height = row.Height;
        rowBounds.Width = layout.Data.Width;
        return rowBounds;
    }

    public HitTestInfo HitTest(int x, int y)
    {
        throw new PlatformNotSupportedException();
    }

    public HitTestInfo HitTest(Point position)
    {
        throw new PlatformNotSupportedException();
    }

    /// <internalonly/>
    /// <devdoc>
    ///      Initializes the values for column widths in the table.
    /// </devdoc>
    private void InitializeColumnWidths()
    {
        if (myGridTable is null)
            return;

        GridColumnStylesCollection columns = myGridTable.GridColumnStyles;
        int numCols = columns.Count;

        // Resize the columns to a approximation of a best fit.
        // We find the best fit width of NumRowsForAutoResize rows
        // and use it for each column.
        int preferredColumnWidth = this.myGridTable.IsDefault ? this.PreferredColumnWidth : this.myGridTable.PreferredColumnWidth;
        // if we set the PreferredColumnWidth to something else than AutoColumnSize
        // then use that value
        //
        for (int col = 0; col < numCols; col++)
        {
            // if the column width is not -1, then this column was initialized already
            if (columns[col].width != -1)
                continue;

            columns[col].width = preferredColumnWidth;
        }
    }

    /// <devdoc>
    ///      Invalidates the scrollable area of the DataGrid.
    /// </devdoc>
    internal void InvalidateInside()
    {
        Invalidate(layout.Inside);
    }

    /// <devdoc>
    ///      Invalidates the caption area of the DataGrid.
    /// </devdoc>
    internal void InvalidateCaption()
    {
        if (layout.CaptionVisible)
            Invalidate(layout.Caption);
    }

    /// <devdoc>
    ///      Invalidates a rectangle normalized to the caption's
    ///      visual bounds.
    /// </devdoc>
    internal void InvalidateCaptionRect(Rectangle r)
    {
        if (layout.CaptionVisible)
        {
            Invalidate(r);
        }
    }

    /// <devdoc>
    ///      Invalidates the display region of a given DataGridColumn.
    /// </devdoc>
    internal void InvalidateColumn(int column)
    {
        GridColumnStylesCollection gridColumns = myGridTable.GridColumnStyles;
        if (column < 0 || gridColumns is null || gridColumns.Count <= column)
            return;

        Debug.Assert(gridColumns[column].PropertyDescriptor is not null, "how can we invalidate a column that is invisible?");
        // bail if the column is not visible.
        if (column < firstVisibleCol || column > firstVisibleCol + numVisibleCols - 1)
            return;

        Rectangle columnArea = default;
        columnArea.Height = layout.Data.Height;
        columnArea.Width = gridColumns[column].Width;
        columnArea.Y = layout.Data.Y;

        int x = layout.Data.X - negOffset;
        int gridColumnsCount = gridColumns.Count;
        for (int i = firstVisibleCol; i < gridColumnsCount; ++i)
        {
            if (i == column)
                break;
            x += gridColumns[i].Width;
        }

        columnArea.X = x;
        columnArea.X = MirrorRectangle(columnArea, layout.Data, isRightToLeft());
        Invalidate(columnArea);
    }

    /// <devdoc>
    ///      Invalidates the parent rows area of the DataGrid
    /// </devdoc>
    internal void InvalidateParentRows()
    {
        if (layout.ParentRowsVisible)
            Invalidate(layout.ParentRows);
    }

    /// <devdoc>
    ///      Invalidates a rectangle normalized to the parent
    ///      rows area's visual bounds.
    /// </devdoc>
    internal void InvalidateParentRowsRect(Rectangle r)
    {
        Rectangle parentRowsRect = layout.ParentRows;
        Invalidate(r);
        if (!parentRowsRect.IsEmpty)
        {
            // Invalidate(new Rectangle(parentRowsRect.X + r.X, parentRowsRect.Y + r.Y,
            // r.Width, r.Height));
        }
    }

    /// <devdoc>
    ///      Invalidate the painting region for the row specified.
    /// </devdoc>
    internal void InvalidateRow(int rowNumber)
    {
        Rectangle rowRect = GetRowRect(rowNumber);
        if (!rowRect.IsEmpty)
        {
            Debug.WriteLineIf(CompModSwitches.DataGridPainting.TraceVerbose, "DataGridPainting: Invalidating row " + rowNumber.ToString(CultureInfo.InvariantCulture));
            Invalidate(rowRect);
        }
    }

    private void InvalidateRowHeader(int rowNumber)
    {
        if (rowNumber >= firstVisibleRow && rowNumber < firstVisibleRow + numVisibleRows)
        {
            if (!layout.RowHeadersVisible)
                return;

            Rectangle invalid = default;
            invalid.Y = GetRowTop(rowNumber);
            invalid.X = layout.RowHeaders.X;
            invalid.Width = layout.RowHeaders.Width;
            invalid.Height = this.DataGridRows[rowNumber].Height;
            Invalidate(invalid);
        }
    }

    // NOTE:
    // because of Rtl, we assume that the only place that calls InvalidateRowRect is
    // the DataGridRelationshipRow
    internal void InvalidateRowRect(int rowNumber, Rectangle r)
    {
        Rectangle rowRect = GetRowRect(rowNumber);
        if (!rowRect.IsEmpty)
        {
            Debug.WriteLineIf(CompModSwitches.DataGridPainting.TraceVerbose, "DataGridPainting: Invalidating a rect in row " + rowNumber.ToString(CultureInfo.InvariantCulture));
            Rectangle inner = new Rectangle(rowRect.X + r.X, rowRect.Y + r.Y, r.Width, r.Height);
            if (vertScrollBar.Visible && isRightToLeft())
                inner.X -= vertScrollBar.Width;
            Invalidate(inner);
        }
    }

    public bool IsExpanded(int rowNumber)
    {
        throw new PlatformNotSupportedException();
    }

    public bool IsSelected(int row)
    {
        throw new PlatformNotSupportedException();
    }

    internal static bool IsTransparentColor(Color color)
    {
        return color.A < 255;
    }

    /// <devdoc>
    ///      Determines if Scrollbars should be visible,
    ///      updates their bounds and the bounds of all
    ///      other regions in the DataGrid's Layout.
    /// </devdoc>
    private void LayoutScrollBars()
    {
        // if we set the dataSource to null, then take away the scrollbars.
        if (listManager is null || myGridTable is null)
        {
            horizScrollBar.Visible = false;
            vertScrollBar.Visible = false;
            return;
        }

        // Scrollbars are a tricky issue.
        // We need to see if we can cram our columns and rows
        // in without scrollbars and if they don't fit, we make
        // scrollbars visible and then fixup our regions for the
        // data and headers.
        bool needHorizScrollbar = false;
        bool needVertScrollbar = false;
        bool recountRows = false;
        bool alignToRight = isRightToLeft();

        int nGridCols = myGridTable.GridColumnStyles.Count;

        // if we call LayoutScrollBars before CreateDataGridRows
        // then the columns will have their default width ( 100 )
        // CreateDataGridRows will possibly change the columns' width
        // and anyway, ComputeVisibleRows will call the DataGridRows accessor
        DataGridRow[] gridRows = this.DataGridRows;

        // at this stage, the data grid columns may have their width set to -1 ( ie, their width is uninitialized )
        // make sure that the totalWidth is at least 0
        int totalWidth = Math.Max(0, GetColumnWidthSum());

        if (totalWidth > layout.Data.Width && !needHorizScrollbar)
        {
            int horizHeight = horizScrollBar.Height;
            layout.Data.Height -= horizHeight;
            if (layout.RowHeadersVisible)
                layout.RowHeaders.Height -= horizHeight;
            needHorizScrollbar = true;
        }

        int oldFirstVisibleRow = firstVisibleRow;

        ComputeVisibleRows();
        if (numTotallyVisibleRows != DataGridRowsLength && !needVertScrollbar)
        {
            int vertWidth = vertScrollBar.Width;
            layout.Data.Width -= vertWidth;
            if (layout.ColumnHeadersVisible)
            {
                if (alignToRight)
                    layout.ColumnHeaders.X += vertWidth;

                layout.ColumnHeaders.Width -= vertWidth;
            }

            needVertScrollbar = true;
        }

        firstVisibleCol = ComputeFirstVisibleColumn();
        // we compute the number of visible columns only after we set up the vertical scroll bar.
        ComputeVisibleColumns();

        if (needVertScrollbar && totalWidth > layout.Data.Width && !needHorizScrollbar)
        {
            firstVisibleRow = oldFirstVisibleRow;
            int horizHeight = horizScrollBar.Height;
            layout.Data.Height -= horizHeight;
            if (layout.RowHeadersVisible)
                layout.RowHeaders.Height -= horizHeight;
            needHorizScrollbar = true;
            recountRows = true;
        }

        if (recountRows)
        {
            ComputeVisibleRows();
            if (numTotallyVisibleRows != DataGridRowsLength && !needVertScrollbar)
            {
                int vertWidth = vertScrollBar.Width;
                layout.Data.Width -= vertWidth;
                if (layout.ColumnHeadersVisible)
                {
                    if (alignToRight)
                        layout.ColumnHeaders.X += vertWidth;

                    layout.ColumnHeaders.Width -= vertWidth;
                }

                needVertScrollbar = true;
            }
        }

        layout.ResizeBoxRect = default;
        if (needVertScrollbar && needHorizScrollbar)
        {
            Rectangle data = layout.Data;
            layout.ResizeBoxRect = new Rectangle(alignToRight ? data.X : data.Right,
                                           data.Bottom,
                                           vertScrollBar.Width,
                                           horizScrollBar.Height);
        }

        if (needHorizScrollbar && nGridCols > 0)
        {
            // Debug.WriteLineIf(CompModSwitches.DataGridScrolling.TraceVerbose, "DataGridScrolling: foo");

            int widthNotVisible = totalWidth - layout.Data.Width;

            horizScrollBar.Minimum = 0;
            horizScrollBar.Maximum = totalWidth;
            horizScrollBar.SmallChange = 1;
            horizScrollBar.LargeChange = Math.Max(totalWidth - widthNotVisible, 0);
            horizScrollBar.Enabled = Enabled;
            horizScrollBar.RightToLeft = RightToLeft;
            horizScrollBar.Bounds = new Rectangle(alignToRight ? layout.Inside.X + layout.ResizeBoxRect.Width : layout.Inside.X,
                                                        layout.Data.Bottom,
                                                        layout.Inside.Width - layout.ResizeBoxRect.Width,
                                                        horizScrollBar.Height);
            horizScrollBar.Visible = true;
        }
        else
        {
            HorizontalOffset = 0;
            horizScrollBar.Visible = false;
        }

        if (needVertScrollbar)
        {
            int vertScrollBarTop = layout.Data.Y;
            if (layout.ColumnHeadersVisible)
                vertScrollBarTop = layout.ColumnHeaders.Y;
            // if numTotallyVisibleRows == 0 ( the height of the row is bigger than the height of
            // the grid ) then scroll in increments of 1.
            vertScrollBar.LargeChange = numTotallyVisibleRows != 0 ? numTotallyVisibleRows : 1;
            vertScrollBar.Bounds = new Rectangle(alignToRight ? layout.Data.X : layout.Data.Right,
                                                 vertScrollBarTop,
                                                 vertScrollBar.Width,
                                                 layout.Data.Height + layout.ColumnHeaders.Height);
            vertScrollBar.Enabled = Enabled;
            vertScrollBar.Visible = true;
            if (alignToRight)
                layout.Data.X += vertScrollBar.Width;
        }
        else
        {
            vertScrollBar.Visible = false;
        }
    }

    public void NavigateBack()
    {
        throw new PlatformNotSupportedException();
    }

    public void NavigateTo(int rowNumber, string relationName)
    {
        throw new PlatformNotSupportedException();
    }

    internal void NavigateTo(string relationName, DataGridRow source, bool fromRow)
    {
        // do not navigate if AllowNavigation is set to false
        if (!AllowNavigation)
            return;
        // Commit the edit if possible
        if (!CommitEdit())
            return;

        DataGridState childState;
        try
        {
            childState = CreateChildState(relationName, source);
        }
        catch
        {
            // if we get an error when creating the RelatedCurrencyManager
            // then navigateBack and ignore the exception.
            NavigateBack();
            return;
        }

        // call EndCurrentEdit before navigating.
        // if we get an exception, we do not navigate.
        try
        {
            listManager.EndCurrentEdit();
        }
        catch
        {
            return;
        }

        // Preserve our current state
        // we need to do this after the EndCurrentEdit, otherwise the
        // DataGridState will get the listChanged event from the EndCurrentEdit
        DataGridState dgs = new DataGridState(this);
        dgs.LinkingRow = source;

        // we need to update the Position in the ListManager
        // ( the RelatedListManager uses only the position in the parentManager
        //   to create the childRows
        // before the code was calling CurrentCell = this and such
        // we should only call EndCurrentEdit ( which the code was doing anyway )
        // and then set the position in the listManager to the new row.
        if (source.RowNumber != CurrentRow)
            listManager.Position = source.RowNumber;

        // We save our state if the parent rows stack is empty.
        if (parentRows.GetTopParent() is null)
        {
            originalState = dgs;
        }

        parentRows.AddParent(dgs);

        NavigateTo(childState);

        if (fromRow)
        {
            // OnLinkClick(EventArgs.Empty);
        }
    }

    private void NavigateTo(DataGridState childState)
    {
        // we are navigating... better stop editing.
        EndEdit();

        // also, we are no longer in editOrNavigate mode either
        gridState[GRIDSTATE_isNavigating] = false;

        // reset hot tracking
        ResetMouseState();

        // Retrieve the child state
        childState.PullState(this, true);               // true for creating columns when we navigate to child rows

        if (listManager.Position != currentRow)
        {
            currentRow = listManager.Position == -1 ? 0 : listManager.Position;
        }

        if (parentRows.GetTopParent() is not null)
        {
            caption.BackButtonActive = AllowNavigation;
            caption.BackButtonVisible = caption.BackButtonActive;
            caption.DownButtonActive = true;
        }

        HorizontalOffset = 0;
        PerformLayout();
        Invalidate();
    }

    /// <devdoc>
    ///      Given a coordinate in the control this method returns
    ///      the equivalent point for a row.
    /// </devdoc>
    private Point NormalizeToRow(int x, int y, int row)
    {
        Debug.Assert(row >= firstVisibleRow && row < firstVisibleRow + numVisibleRows,
                     "Row " + row.ToString(CultureInfo.InvariantCulture) + "is not visible! firstVisibleRow = " +
                     firstVisibleRow.ToString(CultureInfo.InvariantCulture) + ", numVisibleRows = " +
                     numVisibleRows.ToString(CultureInfo.InvariantCulture));
        Point origin = new Point(0, layout.Data.Y);

        DataGridRow[] localGridRows = DataGridRows;
        for (int r = firstVisibleRow; r < row; ++r)
        {
            origin.Y += localGridRows[r].Height;
        }

        // when hittesting for the PlusMinus, the code in the DataGridRelationshipRow
        // will use real X coordinate ( the one from layout.RowHeaders ) to paint the glyph
        return new Point(x, y - origin.Y);
    }

    internal void OnColumnCollectionChanged(object sender, CollectionChangeEventArgs e)
    {
        DataGridTableStyle table = (DataGridTableStyle)sender;
        if (table.Equals(myGridTable))
        {
            // if we changed the column collection, then we need to set the property
            // descriptors in the column collection.
            // unless the user set the propertyDescriptor in the columnCollection
            if (!myGridTable.IsDefault)
            {
                // if the element in the collectionChangeEventArgs is not null
                // and the action is refresh, then it means that the user
                // set the propDesc. we do not want to override this.
                if (e.Action != CollectionChangeAction.Refresh || e.Element is null)
                    PairTableStylesAndGridColumns(listManager, myGridTable, false);
            }

            Invalidate();
            PerformLayout();
        }
    }

    /// <devdoc>
    ///      Paints column headers.
    /// </devdoc>
    private void PaintColumnHeaders(Graphics g)
    {
        bool alignToLeft = isRightToLeft();
        Rectangle boundingRect = layout.ColumnHeaders;
        if (!alignToLeft)
            boundingRect.X -= negOffset;
        boundingRect.Width += negOffset;

        int columnHeaderWidth = PaintColumnHeaderText(g, boundingRect);

        if (alignToLeft)
            boundingRect.X = boundingRect.Right - columnHeaderWidth;

        boundingRect.Width = columnHeaderWidth;
        if (!FlatMode)
        {
            ControlPaint.DrawBorder3D(g, boundingRect, Border3DStyle.RaisedInner);
            boundingRect.Inflate(-1, -1);
            // g.SetPen(OldSystemPens.Control);
            // g.OldBrush = (OldSystemBrushes.Hollow);
            boundingRect.Width--;
            boundingRect.Height--;
            g.DrawRectangle(SystemPens.Control, boundingRect);
        }
    }

    private int PaintColumnHeaderText(Graphics g, Rectangle boundingRect)
    {
        int cx = 0;
        Rectangle textBounds = boundingRect;
        GridColumnStylesCollection gridColumns = myGridTable.GridColumnStyles;
        bool alignRight = isRightToLeft();

        int nGridCols = gridColumns.Count;
        // for sorting
        PropertyDescriptor sortProperty = null;
        sortProperty = ListManager.GetSortProperty();

        // Now paint the column header text!
        for (int col = firstVisibleCol; col < nGridCols; ++col)
        {
            if (gridColumns[col].PropertyDescriptor is null)
                continue;

            if (cx > boundingRect.Width)
                break;

            bool columnSorted = sortProperty is not null && sortProperty.Equals(gridColumns[col].PropertyDescriptor);

            if (alignRight)
            {
                textBounds.Width = gridColumns[col].Width -
                                   (columnSorted ? textBounds.Height : 0);
                textBounds.X = boundingRect.Right - cx - textBounds.Width;
            }
            else
            {
                textBounds.X = boundingRect.X + cx;
                textBounds.Width = gridColumns[col].Width -
                                   (columnSorted ? textBounds.Height : 0);
            }

            // at the moment we paint some pixels twice.
            // we should not call FilLRectangle, once the real GDI+ is there, we will have no need to do that

            // if the user set the HeaderBackBrush property on the
            // dataGrid, then use that property
            Brush headerBrush;
            if (myGridTable.IsDefault)
                headerBrush = HeaderBackBrush;
            else
                headerBrush = myGridTable.HeaderBackBrush;

            g.FillRectangle(headerBrush, textBounds);
            // granted, the code would be a lot cleaner if we were using a "new Rectangle"
            // but like this will be faster
            if (alignRight)
            {
                textBounds.X -= 2;
                textBounds.Y += 2;
            }
            else
            {
                textBounds.X += 2;
                textBounds.Y += 2;
            }

            StringFormat format = new StringFormat();

            // the columnHeaderText alignment should be the same as
            // the alignment in the column
            //
            HorizontalAlignment colAlignment = gridColumns[col].Alignment;
            format.Alignment = colAlignment == HorizontalAlignment.Right ? StringAlignment.Far :
                               colAlignment == HorizontalAlignment.Center ? StringAlignment.Center :
                                                                             StringAlignment.Near;

            // part 1, section 1: the column headers should not wrap
            format.FormatFlags |= StringFormatFlags.NoWrap;

            if (alignRight)
            {
                format.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
                format.Alignment = StringAlignment.Near;
            }

            g.DrawString(gridColumns[col].HeaderText,
                         myGridTable.IsDefault ? HeaderFont : myGridTable.HeaderFont,
                         myGridTable.IsDefault ? HeaderForeBrush : myGridTable.HeaderForeBrush,
                         textBounds,
                         format);
            format.Dispose();

            if (alignRight)
            {
                textBounds.X += 2;
                textBounds.Y -= 2;
            }
            else
            {
                textBounds.X -= 2;
                textBounds.Y -= 2;
            }

            if (columnSorted)
            {
                // CONSIDER: This triangle painting is pretty unclean
                //           perhaps this should be a bitmap?
                Rectangle triBounds = new Rectangle(alignRight ? textBounds.X - textBounds.Height : textBounds.Right,
                                                    textBounds.Y,
                                                    textBounds.Height,
                                                    textBounds.Height);

                g.FillRectangle(headerBrush, triBounds);
                int deflateValue = Math.Max(0, (textBounds.Height - 5) / 2);
                triBounds.Inflate(-deflateValue, -deflateValue);

                Pen pen1 = new Pen(BackgroundBrush);
                Pen pen2 = new Pen(myGridTable.BackBrush);
                pen1.Dispose();
                pen2.Dispose();
            }

            int paintedWidth = textBounds.Width + (columnSorted ? textBounds.Height : 0);

            if (!FlatMode)
            {
                if (alignRight && columnSorted)
                    textBounds.X -= textBounds.Height;
                textBounds.Width = paintedWidth;

                ControlPaint.DrawBorder3D(g, textBounds, Border3DStyle.RaisedInner);
            }

            cx += paintedWidth;
        }

        // paint the possible exposed portion to the right ( or left, as the case may be)
        if (cx < boundingRect.Width)
        {
            textBounds = boundingRect;

            if (!alignRight)
                textBounds.X += cx;

            textBounds.Width -= cx;
            g.FillRectangle(backgroundBrush, textBounds);
        }

        return cx;
    }

    /// <devdoc>
    ///      Paints a border around the bouding rectangle given
    /// </devdoc>
    private void PaintBorder(Graphics g, Rectangle bounds)
    {
        if (BorderStyle == BorderStyle.None)
            return;
        if (BorderStyle == BorderStyle.Fixed3D)
        {
            Border3DStyle style = Border3DStyle.Sunken;
            ControlPaint.DrawBorder3D(g, bounds, style);
        }
        else if (BorderStyle == BorderStyle.FixedSingle)
        {
            Brush br;

            if (myGridTable.IsDefault)
                br = HeaderForeBrush;
            else
                br = myGridTable.HeaderForeBrush;
            g.FillRectangle(br, bounds.X, bounds.Y, bounds.Width + 2, 2);
            g.FillRectangle(br, bounds.Right - 2, bounds.Y, 2, bounds.Height + 2);
            g.FillRectangle(br, bounds.X, bounds.Bottom - 2, bounds.Width + 2, 2);
            g.FillRectangle(br, bounds.X, bounds.Y, 2, bounds.Height + 2);
        }
        else
        {
            Pen pen = SystemPens.WindowFrame;
            bounds.Width--;
            bounds.Height--;
            g.DrawRectangle(pen, bounds);
        }
    }

    /// <devdoc>
    ///      Paints the grid in the bounding rectangle given.
    ///      This includes the column headers and each visible row.
    /// </devdoc>
    private void PaintGrid(Graphics g, Rectangle gridBounds)
    {
        Debug.WriteLineIf(CompModSwitches.DataGridPainting.TraceVerbose, "DataGridPainting: PaintGrid on " + gridBounds.ToString());

        Rectangle rc = gridBounds;

        if (listManager is not null)
        {
            if (layout.ColumnHeadersVisible)
            {
                Region r = g.Clip;
                g.SetClip(layout.ColumnHeaders);
                PaintColumnHeaders(g);
                g.Clip = r;
                r.Dispose();
                int columnHeaderHeight = layout.ColumnHeaders.Height;
                rc.Y += columnHeaderHeight;
                rc.Height -= columnHeaderHeight;
            }

            if (layout.TopLeftHeader.Width > 0)
            {
                if (myGridTable.IsDefault)
                    g.FillRectangle(HeaderBackBrush, layout.TopLeftHeader);
                else
                    g.FillRectangle(myGridTable.HeaderBackBrush, layout.TopLeftHeader);

                if (!FlatMode)
                {
                    ControlPaint.DrawBorder3D(g, layout.TopLeftHeader, Border3DStyle.RaisedInner);
                }
            }

            PaintRows(g, ref rc);
        }

        // paint the possible exposed portion below
        if (rc.Height > 0)
        {
            g.FillRectangle(backgroundBrush, rc);
        }
    }

    private void DeleteDataGridRows(int deletedRows)
    {
        if (deletedRows == 0)
            return;

        int currentRowCount = DataGridRowsLength;
        int newDataGridRowsLength = currentRowCount - deletedRows + (gridState[GRIDSTATE_inAddNewRow] ? 1 : 0);
        DataGridRow[] newDataGridRows = new DataGridRow[newDataGridRowsLength];
        DataGridRow[] gridRows = DataGridRows;

        // the number of selected entries so far in the array
        int selectedEntries = 0;

        for (int i = 0; i < currentRowCount; i++)
        {
            if (gridRows[i].Selected)
            {
                selectedEntries++;
            }
            else
            {
                newDataGridRows[i - selectedEntries] = gridRows[i];
                newDataGridRows[i - selectedEntries].number = i - selectedEntries;
            }
        }

        if (gridState[GRIDSTATE_inAddNewRow])
        {
            newDataGridRows[currentRowCount - selectedEntries] = new DataGridAddNewRow(this, myGridTable, currentRowCount - selectedEntries);
            gridState[GRIDSTATE_inAddNewRow] = false;
        }

        Debug.Assert(selectedEntries == deletedRows, "all the rows that would have been deleted should have been selected: selectedGridEntries " + selectedEntries.ToString(CultureInfo.InvariantCulture) + " deletedRows " + deletedRows.ToString(CultureInfo.InvariantCulture));

        SetDataGridRows(newDataGridRows, newDataGridRowsLength);
    }

    /// <devdoc>
    ///      Paints the visible rows on the grid.
    /// </devdoc>
    private void PaintRows(Graphics g, ref Rectangle boundingRect)
    {
        int cy = 0;
        bool alignRight = isRightToLeft();
        Rectangle rowBounds = boundingRect;
        Rectangle dataBounds = Rectangle.Empty;
        bool paintRowHeaders = layout.RowHeadersVisible;
        Rectangle headerBounds = Rectangle.Empty;

        int numRows = DataGridRowsLength;
        DataGridRow[] localGridRows = DataGridRows;
        int numCols = myGridTable.GridColumnStyles.Count - firstVisibleCol;

        for (int row = firstVisibleRow; row < numRows; row++)
        {
            if (cy > boundingRect.Height)
                break;

            rowBounds = boundingRect;
            rowBounds.Height = localGridRows[row].Height;
            rowBounds.Y = boundingRect.Y + cy;

            // will add some errors
#if false
                    if (forDebug == 0 || forDebug == 1)
                    {
                        object dRowView = listManager[row];
                        DataRow dRow= ((DataRowView) dRowView).Row;
                        // dRow.RowError = "Error " + forDebug.ToString();
                        dRow.SetColumnError(forDebug, "another error " + forDebug.ToString());
    
                        /*
                        if (localGridRows[row].DataRow is not null)
                        {
                            localGridRows[row].DataRow.RowError = "error " + forDebug.ToString();
                            localGridRows[row].DataRow.SetColumnError(forDebug, "another error " + forDebug.ToString());
                        }
                        */
                        forDebug ++;
                    }
#endif // false
            if (paintRowHeaders)
            {
                headerBounds = rowBounds;
                headerBounds.Width = layout.RowHeaders.Width;

                if (alignRight)
                {
                    headerBounds.X = rowBounds.Right - headerBounds.Width;
                }

                if (g.IsVisible(headerBounds))
                {
                    localGridRows[row].PaintHeader(g, headerBounds, alignRight, gridState[GRIDSTATE_isEditing]);
                    g.ExcludeClip(headerBounds);
                }

                if (!alignRight)
                    rowBounds.X += headerBounds.Width;
                rowBounds.Width -= headerBounds.Width;
            }

            if (g.IsVisible(rowBounds))
            {
                dataBounds = rowBounds;
                if (!alignRight)
                    dataBounds.X -= negOffset;
                dataBounds.Width += negOffset;

                localGridRows[row].Paint(g, dataBounds, rowBounds, firstVisibleCol, numCols, alignRight);
            }

            cy += rowBounds.Height;
        }

        boundingRect.Y += cy;
        boundingRect.Height -= cy;
    }

    protected override bool ProcessDialogKey(Keys keyData)
    {
        Debug.WriteLineIf(CompModSwitches.DataGridKeys.TraceVerbose, "DataGridKeys: ProcessDialogKey " + TypeDescriptor.GetConverter(typeof(Keys)).ConvertToString(keyData));
        DataGridRow[] localGridRows = DataGridRows;
        if (listManager is not null && DataGridRowsLength > 0 && localGridRows[currentRow].OnKeyPress(keyData))
        {
            Debug.WriteLineIf(CompModSwitches.DataGridKeys.TraceVerbose, "DataGridKeys: Current Row ate the keystroke");
            return true;
        }

        switch (keyData & Keys.KeyCode)
        {
            case Keys.Tab:
            case Keys.Up:
            case Keys.Down:
            case Keys.Left:
            case Keys.Right:
            case Keys.Next:
            case Keys.Prior:
            case Keys.Enter:
            case Keys.Escape:
            case Keys.Oemplus:
            case Keys.Add:
            case Keys.OemMinus:
            case Keys.Subtract:
            case Keys.Space:
            case Keys.Delete:
            case Keys.A:
                KeyEventArgs ke = new KeyEventArgs(keyData);
                if (ProcessGridKey(ke))
                    return true;
                break;

            case Keys.C:
                if ((keyData & Keys.Control) != 0 && (keyData & Keys.Alt) == 0)
                {
                    // the user pressed Ctrl-C
                    if (!Bound)
                        break;

                    // need to distinguish between selecting a set of rows, and
                    // selecting just one column.
                    if (numSelectedRows == 0)
                    {
                        // copy the data from one column only
                        if (currentRow < ListManager.Count)
                        {
                            GridColumnStylesCollection columns = myGridTable.GridColumnStyles;
                            if (currentCol >= 0 && currentCol < columns.Count)
                            {
                                DataGridColumnStyle column = columns[currentCol];
                                string text = column.GetDisplayText(column.GetColumnValueAtRow(ListManager, currentRow));

                                // copy the data to the clipboard
                                Clipboard.SetDataObject(text);
                                return true;
                            }
                        }
                    }
                    else
                    {
                        // the user selected a set of rows to copy the data from

                        int numRowsOutputted = 0;           // the number of rows written to "text"
                        string text = "";

                        for (int i = 0; i < DataGridRowsLength; ++i)
                        {
                            if (localGridRows[i].Selected)
                            {
                                GridColumnStylesCollection columns = myGridTable.GridColumnStyles;
                                int numCols = columns.Count;
                                for (int j = 0; j < numCols; j++)
                                {
                                    DataGridColumnStyle column = columns[j];
                                    text += column.GetDisplayText(column.GetColumnValueAtRow(ListManager, i));

                                    // do not put the delimiter at the end of the last column
                                    if (j < numCols - 1)
                                    {
                                        text += GetOutputTextDelimiter();
                                    }
                                }

                                // put the hard enter "\r\n" only if this is not the last selected row
                                if (numRowsOutputted < numSelectedRows - 1)
                                {
                                    text += "\r\n";
                                }

                                numRowsOutputted++;
                            }
                        }

                        // copy the data to the clipboard
                        Clipboard.SetDataObject(text);
                        return true;
                    }
                }

                break;
        }

        return base.ProcessDialogKey(keyData);
    }

    private void DeleteRows(DataGridRow[] localGridRows)
    {
        int rowsDeleted = 0;

        int currentRowsCount = listManager is null ? 0 : listManager.Count;

        if (Visible)
            BeginUpdateInternal();
        try
        {
            if (ListManager is not null)
            {
                for (int i = 0; i < DataGridRowsLength; i++)
                {
                    if (localGridRows[i].Selected)
                    {
                        if (localGridRows[i] is DataGridAddNewRow)
                        {
                            Debug.Assert(i == DataGridRowsLength - 1, "the location of addNewRow is " + i.ToString(CultureInfo.InvariantCulture) + " and there are " + DataGridRowsLength.ToString(CultureInfo.InvariantCulture) + " rows ");
                            localGridRows[i].Selected = false;
                        }
                        else
                        {
                            ListManager.RemoveAt(i - rowsDeleted);
                            rowsDeleted++;
                        }
                    }
                }
            }
        }
        catch
        {
            // if we got an exception from the back end
            // when deleting the rows then we should reset
            // our rows and re-throw the exception
            RecreateDataGridRows();
            gridState[GRIDSTATE_inDeleteRow] = false;
            if (Visible)
                EndUpdateInternal();
            throw;
        }

        // keep the copy of the old rows in place
        // it may be the case that deleting one row could cause multiple rows to be deleted in the same list
        if (listManager is not null && currentRowsCount == listManager.Count + rowsDeleted)
        {
            DeleteDataGridRows(rowsDeleted);
        }
        else
        {
            RecreateDataGridRows();
        }

        gridState[GRIDSTATE_inDeleteRow] = false;
        if (Visible)
            EndUpdateInternal();

        if (listManager is not null && currentRowsCount != listManager.Count + rowsDeleted)
        {
            Invalidate();
        }
    }

    // convention:
    // if we return -1 it means that the user was going left and there were no visible columns to the left of the current one
    // if we return cols.Count + 1 it means that the user was going right and there were no visible columns to the right of the currrent
    private static int MoveLeftRight(GridColumnStylesCollection cols, int startCol, bool goRight)
    {
        int i;
        if (goRight)
        {
            for (i = startCol + 1; i < cols.Count; i++)
            {
                // if (cols[i].Visible && cols[i].PropertyDescriptor is not null)
                if (cols[i].PropertyDescriptor is not null)
                    return i;
            }

            return i;
        }
        else
        {
            for (i = startCol - 1; i >= 0; i--)
            {
                // if (cols[i].Visible && cols[i].PropertyDescriptor is not null)
                if (cols[i].PropertyDescriptor is not null)
                    return i;
            }

            return i;
        }
    }

    protected bool ProcessGridKey(KeyEventArgs ke)
    {
        Debug.WriteLineIf(CompModSwitches.DataGridKeys.TraceVerbose, "DataGridKeys: ProcessGridKey " + TypeDescriptor.GetConverter(typeof(Keys)).ConvertToString(ke.KeyCode));
        if (listManager is null || myGridTable is null)
            return false;

        DataGridRow[] localGridRows = DataGridRows;
        KeyEventArgs biDiKe = ke;
        // check for Bi-Di
        if (isRightToLeft())
        {
            switch (ke.KeyCode)
            {
                case Keys.Left:
                    biDiKe = new KeyEventArgs((Keys.Right | ke.Modifiers));
                    break;
                case Keys.Right:
                    biDiKe = new KeyEventArgs((Keys.Left | ke.Modifiers));
                    break;
                default:
                    break;
            }
        }

        GridColumnStylesCollection cols = myGridTable.GridColumnStyles;
        int firstColumnMarkedVisible = 0;
        int lastColumnMarkedVisible = cols.Count;
        for (int i = 0; i < cols.Count; i++)
        {
            if (cols[i].PropertyDescriptor is not null)
            {
                firstColumnMarkedVisible = i;
                break;
            }
        }

        for (int i = cols.Count - 1; i >= 0; i--)
        {
            if (cols[i].PropertyDescriptor is not null)
            {
                lastColumnMarkedVisible = i;
                break;
            }
        }

        switch (biDiKe.KeyCode)
        {
            case Keys.Tab:
                return ProcessTabKey(biDiKe.KeyData);
            case Keys.Up:
                gridState[GRIDSTATE_childLinkFocused] = false;
                if (dataGridRowsLength == 0)
                {
                    return true;
                }

                if (biDiKe.Control && !biDiKe.Alt)
                {
                    if (biDiKe.Shift)
                    {
                        DataGridRow[] gridRows = DataGridRows;

                        int savedCurrentRow = currentRow;
                        CurrentRow = 0;

                        ResetSelection();

                        for (int i = 0; i <= savedCurrentRow; i++)
                            gridRows[i].Selected = true;
                        numSelectedRows = savedCurrentRow + 1;
                        // hide the edit box
                        EndEdit();
                        return true;
                    }

                    // do not make the parentRowsVisible = false;
                    // ParentRowsVisible = false;
                    ResetSelection();
                    CurrentRow = 0;
                    Debug.Assert(ListManager.Position == CurrentCell.RowNumber || listManager.Count == 0, "current row out of ssync with DataSource");
                    return true;
                }
                else if (biDiKe.Shift)
                {
                    DataGridRow[] gridRows = DataGridRows;
                    // keep a continous selected region
                    if (gridRows[currentRow].Selected)
                    {
                        if (currentRow >= 1)
                        {
                            if (gridRows[currentRow - 1].Selected)
                            {
                                if (currentRow >= DataGridRowsLength - 1 || !gridRows[currentRow + 1].Selected)
                                {
                                    numSelectedRows--;
                                    gridRows[currentRow].Selected = false;
                                }
                            }
                            else
                            {
                                numSelectedRows += gridRows[currentRow - 1].Selected ? 0 : 1;
                                gridRows[currentRow - 1].Selected = true;
                            }

                            CurrentRow--;
                        }
                    }
                    else
                    {
                        numSelectedRows++;
                        gridRows[currentRow].Selected = true;
                        if (currentRow >= 1)
                        {
                            numSelectedRows += gridRows[currentRow - 1].Selected ? 0 : 1;
                            gridRows[currentRow - 1].Selected = true;
                            CurrentRow--;
                        }
                    }

                    // hide the edit box:
                    EndEdit();
                    Debug.Assert(ListManager.Position == CurrentCell.RowNumber || listManager.Count == 0, "current row out of ssync with DataSource");
                    return true;
                }
                else if (biDiKe.Alt)
                {
                    // will need to collapse all child table links
                    // -1 is for all rows, and false is for collapsing the rows
                    SetRowExpansionState(-1, false);
                    Debug.Assert(ListManager.Position == CurrentCell.RowNumber || listManager.Count == 0, "current row out of ssync with DataSource");
                    return true;
                }

                ResetSelection();
                CurrentRow = CurrentRow - 1;
                Edit();
                Debug.Assert(ListManager.Position == CurrentCell.RowNumber || listManager.Count == 0, "current row out of ssync with DataSource");
                break;
            case Keys.Down:
                gridState[GRIDSTATE_childLinkFocused] = false;
                if (dataGridRowsLength == 0)
                {
                    return true;
                }

                if (biDiKe.Control && !biDiKe.Alt)
                {
                    if (biDiKe.Shift)
                    {
                        int savedCurrentRow = currentRow;
                        CurrentRow = Math.Max(0, DataGridRowsLength - (policy.AllowAdd ? 2 : 1));
                        DataGridRow[] gridRows = DataGridRows;

                        ResetSelection();

                        for (int i = savedCurrentRow; i <= currentRow; i++)
                            gridRows[i].Selected = true;

                        numSelectedRows = currentRow - savedCurrentRow + 1;
                        // hide the edit box
                        EndEdit();
                        return true;
                    }

                    // do not make the parentRowsVisible = true;
                    // ParentRowsVisible = true;
                    ResetSelection();
                    CurrentRow = Math.Max(0, DataGridRowsLength - (policy.AllowAdd ? 2 : 1));
                    Debug.Assert(ListManager.Position == CurrentCell.RowNumber || listManager.Count == 0, "current row out of ssync with DataSource");
                    return true;
                }
                else if (biDiKe.Shift)
                {
                    DataGridRow[] gridRows = DataGridRows;

                    // keep a continous selected region
                    if (gridRows[currentRow].Selected)
                    {
                        // -1 because we index from 0
                        if (currentRow < DataGridRowsLength - (policy.AllowAdd ? 1 : 0) - 1)
                        {
                            if (gridRows[currentRow + 1].Selected)
                            {
                                if (currentRow == 0 || !gridRows[currentRow - 1].Selected)
                                {
                                    numSelectedRows--;
                                    gridRows[currentRow].Selected = false;
                                }
                            }
                            else
                            {
                                numSelectedRows += gridRows[currentRow + 1].Selected ? 0 : 1;
                                gridRows[currentRow + 1].Selected = true;
                            }

                            CurrentRow++;
                        }
                    }
                    else
                    {
                        numSelectedRows++;
                        gridRows[currentRow].Selected = true;
                        // -1 because we index from 0, and -1 so this is not the last row
                        // so it adds to -2
                        if (currentRow < DataGridRowsLength - (policy.AllowAdd ? 1 : 0) - 1)
                        {
                            CurrentRow++;
                            numSelectedRows += gridRows[currentRow].Selected ? 0 : 1;
                            gridRows[currentRow].Selected = true;
                        }
                    }

                    // hide the edit box:
                    //
                    EndEdit();
                    Debug.Assert(ListManager.Position == CurrentCell.RowNumber || listManager.Count == 0, "current row out of ssync with DataSource");
                    return true;
                }
                else if (biDiKe.Alt)
                {
                    // will need to expande all child table links
                    // -1 is for all rows, and true is for expanding the rows
                    SetRowExpansionState(-1, true);
                    return true;
                }

                ResetSelection();
                Edit();
                CurrentRow = CurrentRow + 1;
                Debug.Assert(ListManager.Position == CurrentCell.RowNumber || listManager.Count == 0, "current row out of ssync with DataSource");
                break;
            case Keys.OemMinus:
            case Keys.Subtract:
                gridState[GRIDSTATE_childLinkFocused] = false;
                if (biDiKe.Control && !biDiKe.Alt)
                {
                    SetRowExpansionState(-1, false);
                    return true;
                }

                return false;
            case Keys.Oemplus:
            case Keys.Add:
                gridState[GRIDSTATE_childLinkFocused] = false;
                if (biDiKe.Control)
                {
                    SetRowExpansionState(-1, true);
                    // hide the edit box
                    EndEdit();
                    return true;
                }

                return false;
            case Keys.Space:
                gridState[GRIDSTATE_childLinkFocused] = false;
                if (dataGridRowsLength == 0)
                {
                    return true;
                }

                if (biDiKe.Shift)
                {
                    ResetSelection();
                    EndEdit();
                    DataGridRow[] gridRows = DataGridRows;
                    gridRows[currentRow].Selected = true;
                    numSelectedRows = 1;

                    return true;
                }

                return false;
            case Keys.Next:
                gridState[GRIDSTATE_childLinkFocused] = false;
                if (dataGridRowsLength == 0)
                {
                    return true;
                }

                if (biDiKe.Shift)
                {
                    int savedCurrentRow = currentRow;
                    CurrentRow = Math.Min(DataGridRowsLength - (policy.AllowAdd ? 2 : 1), currentRow + numTotallyVisibleRows);

                    DataGridRow[] gridRows = DataGridRows;
                    for (int i = savedCurrentRow; i <= currentRow; i++)
                    {
                        if (!gridRows[i].Selected)
                        {
                            gridRows[i].Selected = true;
                            numSelectedRows++;
                        }
                    }

                    EndEdit();
                }
                else if (biDiKe.Control && !biDiKe.Alt)
                {
                    // map ctrl-pageDown to show the parentRows
                    ParentRowsVisible = true;
                }
                else
                {
                    ResetSelection();
                    CurrentRow = Math.Min(DataGridRowsLength - (policy.AllowAdd ? 2 : 1),
                                          CurrentRow + numTotallyVisibleRows);
                }

                break;
            case Keys.Prior:
                if (dataGridRowsLength == 0)
                {
                    return true;
                }

                gridState[GRIDSTATE_childLinkFocused] = false;
                if (biDiKe.Shift)
                {
                    int savedCurrentRow = currentRow;
                    CurrentRow = Math.Max(0, CurrentRow - numTotallyVisibleRows);

                    DataGridRow[] gridRows = DataGridRows;
                    for (int i = savedCurrentRow; i >= currentRow; i--)
                    {
                        if (!gridRows[i].Selected)
                        {
                            gridRows[i].Selected = true;
                            numSelectedRows++;
                        }
                    }

                    EndEdit();
                }
                else if (biDiKe.Control && !biDiKe.Alt)
                {
                    // map ctrl-pageUp to hide the parentRows
                    ParentRowsVisible = false;
                }
                else
                {
                    ResetSelection();
                    CurrentRow = Math.Max(0,
                                          CurrentRow - numTotallyVisibleRows);
                }

                break;
            case Keys.Left:
                gridState[GRIDSTATE_childLinkFocused] = false;
                ResetSelection();
                if ((biDiKe.Modifiers & Keys.Modifiers) == Keys.Alt)
                {
                    if (Caption.BackButtonVisible)
                        NavigateBack();
                    return true;
                }

                if ((biDiKe.Modifiers & Keys.Control) == Keys.Control)
                {
                    // we should navigate to the first visible column
                    CurrentColumn = firstColumnMarkedVisible;
                    break;
                }

                if (currentCol == firstColumnMarkedVisible && currentRow != 0)
                {
                    CurrentRow = CurrentRow - 1;
                    int newCol = MoveLeftRight(myGridTable.GridColumnStyles, myGridTable.GridColumnStyles.Count, false);
                    Debug.Assert(newCol != -1, "there should be at least a visible column, right?");
                    CurrentColumn = newCol;
                }
                else
                {
                    int newCol = MoveLeftRight(myGridTable.GridColumnStyles, currentCol, false);
                    if (newCol == -1)
                    {
                        if (currentRow == 0)
                            return true;
                        else
                        {
                            // go to the previous row:
                            CurrentRow = CurrentRow - 1;
                            CurrentColumn = lastColumnMarkedVisible;
                        }
                    }
                    else
                    {
                        CurrentColumn = newCol;
                    }
                }

                break;
            case Keys.Right:
                gridState[GRIDSTATE_childLinkFocused] = false;
                ResetSelection();
                if ((biDiKe.Modifiers & Keys.Control) == Keys.Control && !biDiKe.Alt)
                {
                    // we should navigate to the last column that is marked as Visible
                    CurrentColumn = lastColumnMarkedVisible;
                    break;
                }

                if (currentCol == lastColumnMarkedVisible && currentRow != DataGridRowsLength - 1)
                {
                    CurrentRow = CurrentRow + 1;
                    // navigate to the first visible column
                    CurrentColumn = firstColumnMarkedVisible;
                }
                else
                {
                    int newCol = MoveLeftRight(myGridTable.GridColumnStyles, currentCol, true);
                    if (newCol == cols.Count + 1)
                    {
                        // navigate to the first visible column
                        // and the next row
                        CurrentColumn = firstColumnMarkedVisible;
                        CurrentRow++;
                    }
                    else
                        CurrentColumn = newCol;
                }

                break;
            case Keys.F2:
                gridState[GRIDSTATE_childLinkFocused] = false;
                ResetSelection();
                Edit();
                break;
#if DEBUG
            case Keys.F12:
                gridState[GRIDSTATE_childLinkFocused] = false;
                AddNewRow();
                break;
#endif
            case Keys.Home:
                gridState[GRIDSTATE_childLinkFocused] = false;
                if (dataGridRowsLength == 0)
                {
                    return true;
                }

                ResetSelection();
                CurrentColumn = 0;
                if (biDiKe.Control && !biDiKe.Alt)
                {
                    int currentRowSaved = currentRow;
                    CurrentRow = 0;

                    if (biDiKe.Shift)
                    {
                        // Ctrl-Shift-Home will select all the rows up to the first one
                        DataGridRow[] gridRows = DataGridRows;
                        for (int i = 0; i <= currentRowSaved; i++)
                        {
                            gridRows[i].Selected = true;
                            numSelectedRows++;
                        }

                        EndEdit();
                    }

                    Debug.Assert(ListManager.Position == CurrentCell.RowNumber || listManager.Count == 0, "current row out of ssync with DataSource");
                    return true;
                }

                Debug.Assert(ListManager.Position == CurrentCell.RowNumber || listManager.Count == 0, "current row out of ssync with DataSource");
                break;
            case Keys.Delete:
                gridState[GRIDSTATE_childLinkFocused] = false;
                if (policy.AllowRemove && numSelectedRows > 0)
                {
#if DEBUG
                    // when the list is empty, then the position
                    // in the listManager is -1, and the currentPosition in the grid is 0
                    if (ListManager is not null && ListManager.Count > 0)
                    {
                        Debug.Assert(ListManager.Position == currentRow,
                                        "Current row out of sync with DataSource",
                                        "The DataSource's Position property should be mirrored by the CurrentCell.RowNumber of the DataGrid.");
                    }
#endif // DEBUG

                    gridState[GRIDSTATE_inDeleteRow] = true;
                    DeleteRows(localGridRows);
                    // set the currentRow to the position in the list
                    currentRow = listManager.Count == 0 ? 0 : listManager.Position;
                    numSelectedRows = 0;
                }
                else
                {
                    // if we did not use the the Delete key, let the dataGridTextBox use it
                    return false;
                }

                break;
            case Keys.End:
                gridState[GRIDSTATE_childLinkFocused] = false;
                if (dataGridRowsLength == 0)
                {
                    return true;
                }

                ResetSelection();
                // go the the last visible column
                CurrentColumn = lastColumnMarkedVisible;

                if (biDiKe.Control && !biDiKe.Alt)
                {
                    int savedCurrentRow = currentRow;
                    CurrentRow = Math.Max(0, DataGridRowsLength - (policy.AllowAdd ? 2 : 1));

                    if (biDiKe.Shift)
                    {
                        // Ctrl-Shift-Home will select all the rows up to the first one
                        DataGridRow[] gridRows = DataGridRows;
                        for (int i = savedCurrentRow; i <= currentRow; i++)
                        {
                            gridRows[i].Selected = true;
                        }

                        numSelectedRows = currentRow - savedCurrentRow + 1;
                        // hide the edit box
                        //
                        EndEdit();
                    }

                    Debug.Assert(ListManager.Position == CurrentCell.RowNumber || listManager.Count == 0, "current row out of ssync with DataSource");
                    return true;
                }

                Debug.Assert(ListManager.Position == CurrentCell.RowNumber || listManager.Count == 0, "current row out of ssync with DataSource");
                break;
            case Keys.Enter:
                gridState[GRIDSTATE_childLinkFocused] = false;
                ResetSelection();

                // yield the return key if there is no editing
                if (!gridState[GRIDSTATE_isEditing])
                    return false;

                // Ctrl-Enter will call EndCurrentEdit
                if ((biDiKe.Modifiers & Keys.Control) != 0 && !biDiKe.Alt)
                {
                    EndEdit();
                    HandleEndCurrentEdit();
                    Edit();                 // put the edit box on the screen
                }
                else
                {
                    // Do not commit the edit, cause reseting the
                    // current cell will do that
                    // CommitEdit();

                    CurrentRow = currentRow + 1;
                }

                break;
            case Keys.A:
                gridState[GRIDSTATE_childLinkFocused] = false;
                if (biDiKe.Control && !biDiKe.Alt)
                {
                    DataGridRow[] gridRows = DataGridRows;
                    for (int i = 0; i < DataGridRowsLength; i++)
                        if (gridRows[i] is DataGridRelationshipRow)
                            gridRows[i].Selected = true;

                    numSelectedRows = DataGridRowsLength - (policy.AllowAdd ? 1 : 0);
                    // hide the edit box
                    EndEdit();
                    return true;
                }

                return false;
            case Keys.Escape:
                gridState[GRIDSTATE_childLinkFocused] = false;
                ResetSelection();
                if (gridState[GRIDSTATE_isEditing])
                {
                    // rollback
                    AbortEdit();

                    // we have to invalidate the row header ( make it display the row selector instead of the pencil )
                    if (layout.RowHeadersVisible && currentRow > -1)
                    {
                        Rectangle rowHdrRect = GetRowRect(currentRow);
                        rowHdrRect.Width = layout.RowHeaders.Width;
                        Invalidate(rowHdrRect);
                    }

                    // now put the edit column back on the screen
                    Edit();
                }
                else
                {
                    // add this protected virtual method for the XML designer team
                    CancelEditing();
                    Edit();
                    return false;
                }

                break;
        }

        return true;
    }

    protected override bool ProcessKeyPreview(ref Message m)
    {
        if (m.Msg == PInvoke.WM_KEYDOWN)
        {
            KeyEventArgs ke = new KeyEventArgs((Keys)(unchecked((int)(long)m.WParam)) | ModifierKeys);
            switch (ke.KeyCode)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Prior:
                case Keys.Next:
                case Keys.Right:
                case Keys.Left:
                case Keys.Tab:
                case Keys.Escape:
                case Keys.Enter:
                case Keys.OemMinus:
                case Keys.Subtract:
                case Keys.Oemplus:
                case Keys.Add:
                case Keys.Space:
                case Keys.Home:
                case Keys.End:
                case Keys.F2:
                case Keys.Delete:
                case Keys.A:
                    return ProcessGridKey(ke);
            }
        }
        else if (m.Msg == PInvoke.WM_KEYUP)
        {
            KeyEventArgs ke = new KeyEventArgs((Keys)(unchecked((int)(long)m.WParam)) | ModifierKeys);
            if (ke.KeyCode == Keys.Tab)
                return ProcessGridKey(ke);
        }

        return base.ProcessKeyPreview(ref m);
    }

    protected bool ProcessTabKey(Keys keyData)
    {
        if (listManager is null || myGridTable is null)
            return false;
        bool wasEditing = false;
        int columnCount = myGridTable.GridColumnStyles.Count;
        bool biDi = isRightToLeft();
        ResetSelection();

        // Try to commit changes to cell if we were editing
        if (gridState[GRIDSTATE_isEditing])
        {
            wasEditing = true;
            if (!CommitEdit())
            {
                Edit();         // if we can't commit the value put the edit box so that the user sees where the focus is
                return true;
            }
        }

        if ((keyData & Keys.Control) == Keys.Control)
        {
            // when the user hits ctrl-alt-tab just ignore it.
            if ((keyData & Keys.Alt) == Keys.Alt)
                return true;

            // navigate to the next control in the form
            Keys ke = keyData & ~(Keys.Control);
            EndEdit();

            gridState[GRIDSTATE_editControlChanging] = true;
            try
            {
                FocusInternal();
            }
            finally
            {
                gridState[GRIDSTATE_editControlChanging] = false;
            }

            bool ret = false;

            return ret;
        }

        // see if the child relationships can use this TAB key
        DataGridRow[] localRows = DataGridRows;
        GridColumnStylesCollection cols = myGridTable.GridColumnStyles;

        int lastColumnMarkedVisible = 0;
        int firstColumnMarkedVisible = cols.Count - 1;
        // bug 70492: if we do not have any rows, then tab should move focus to the next control
        if (localRows.Length == 0)
        {
            EndEdit();

            bool ret = false;
            ret = base.ProcessDialogKey(keyData);

            return ret;
        }

        for (int i = 0; i < cols.Count; i++)
        {
            // if (cols[i].Visible && cols[i].PropertyDescriptor is not null) {
            if (cols[i].PropertyDescriptor is not null)
            {
                firstColumnMarkedVisible = i;
                break;
            }
        }

        for (int i = cols.Count - 1; i >= 0; i--)
        {
            // if (cols[i].Visible && cols[i].PropertyDescriptor is not null) {
            if (cols[i].PropertyDescriptor is not null)
            {
                lastColumnMarkedVisible = i;
                break;
            }
        }

        if (CurrentColumn == lastColumnMarkedVisible)
        {
            if (gridState[GRIDSTATE_childLinkFocused] || (!gridState[GRIDSTATE_childLinkFocused] && (keyData & Keys.Shift) != Keys.Shift))
            {
                if (localRows[CurrentRow].ProcessTabKey(keyData, layout.RowHeaders, isRightToLeft()))
                {
                    if (cols.Count > 0)
                        cols[CurrentColumn].ConcedeFocus();
                    gridState[GRIDSTATE_childLinkFocused] = true;
                    // let the grid regain focus
                    // introduced because of that BeginInvoke thing in the OnLeave method....
                    if (gridState[GRIDSTATE_canFocus] && CanFocus && !Focused)
                        FocusInternal();
                    return true;
                }
            }

            // actually, it turns out that we should leave the
            // control if we are in the last row
            if ((currentRow == DataGridRowsLength - 1) && ((keyData & Keys.Shift) == 0))
            {
                EndEdit();
                bool ret = false;
                ret = base.ProcessDialogKey(keyData);

                return ret;
            }
        }

        if (CurrentColumn == firstColumnMarkedVisible)
        {
            // if the childLink is focused, then navigate within the relations
            // in the row, otherwise expand the relations list for the row above
            if (!gridState[GRIDSTATE_childLinkFocused])
            {
                if (CurrentRow != 0 && (keyData & Keys.Shift) == Keys.Shift)
                {
                    if (localRows[CurrentRow - 1].ProcessTabKey(keyData, layout.RowHeaders, isRightToLeft()))
                    {
                        CurrentRow--;
                        if (cols.Count > 0)
                            cols[CurrentColumn].ConcedeFocus();
                        gridState[GRIDSTATE_childLinkFocused] = true;
                        // let the grid regain focus
                        // introduced because of that BeginInvoke thing in the OnLeave method....
                        if (gridState[GRIDSTATE_canFocus] && CanFocus && !Focused)
                            FocusInternal();
                        return true;
                    }
                }
            }
            else
            {
                if (localRows[CurrentRow].ProcessTabKey(keyData, layout.RowHeaders, isRightToLeft()))
                {
                    return true;
                }
                else
                {
                    // we were on the firstColumn, previously the link was focused
                    // we have to navigate to the last column
                    gridState[GRIDSTATE_childLinkFocused] = false;
                    CurrentColumn = lastColumnMarkedVisible;
                    return true;
                }
            }

            // if we are on the first cell ( not on the addNewRow )
            // then shift - tab should move to the next control on the form
            if (currentRow == 0 && ((keyData & Keys.Shift) == Keys.Shift))
            {
                EndEdit();
                bool ret = false;
                ret = base.ProcessDialogKey(keyData);

                return ret;
            }
        }

        // move
        if ((keyData & Keys.Shift) != Keys.Shift)
        {
            // forward
            if (CurrentColumn == lastColumnMarkedVisible)
            {
                if (CurrentRow != DataGridRowsLength - 1)
                    CurrentColumn = firstColumnMarkedVisible;
                CurrentRow = CurrentRow + 1;
            }
            else
            {
                int nextCol = MoveLeftRight(cols, currentCol, true);        // true for going right;
                Debug.Assert(nextCol < cols.Count, "we already checked that we are not at the lastColumnMarkedVisible");
                CurrentColumn = nextCol;
            }
        }
        else
        {
            // backward
            if (CurrentColumn == firstColumnMarkedVisible)
            {
                if (CurrentRow != 0)
                {
                    CurrentColumn = lastColumnMarkedVisible;
                }

                if (!gridState[GRIDSTATE_childLinkFocused])             // bug 86803
                    CurrentRow--;
            }
            else if (gridState[GRIDSTATE_childLinkFocused] && CurrentColumn == lastColumnMarkedVisible)
            {
                // part deux: when we hilite the childLink and then press shift-tab, we
                // don't want to navigate at the second to last column
                InvalidateRow(currentRow);
                Edit();
            }
            else
            {
                int prevCol = MoveLeftRight(cols, currentCol, false);       // false for going left
                Debug.Assert(prevCol != -1, "we already checked that we are not at the first columnMarked visible");
                CurrentColumn = prevCol;
            }
        }

        // if we got here, then invalidate childLinkFocused
        gridState[GRIDSTATE_childLinkFocused] = false;

        // Begin another edit if we were editing before
        if (wasEditing)
        {
            ResetSelection();
            Edit();
        }

        return true;
    }

    virtual protected void CancelEditing()
    {
        CancelCursorUpdate();
        // yield the escape key if there is no editing
        // make the last row a DataGridAddNewRow
        if (gridState[GRIDSTATE_inAddNewRow])
        {
            gridState[GRIDSTATE_inAddNewRow] = false;
            DataGridRow[] localGridRows = DataGridRows;

            localGridRows[DataGridRowsLength - 1] = new DataGridAddNewRow(this, myGridTable, DataGridRowsLength - 1);
            SetDataGridRows(localGridRows, DataGridRowsLength);
        }
    }

    internal void RecalculateFonts()
    {
        try
        {
            linkFont = new Font(Font, FontStyle.Underline);
        }
        catch
        {
        }

        fontHeight = Font.Height;
        linkFontHeight = LinkFont.Height;
        captionFontHeight = CaptionFont.Height;

        if (myGridTable is null || myGridTable.IsDefault)
            headerFontHeight = HeaderFont.Height;
        else
            headerFontHeight = myGridTable.HeaderFont.Height;
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler BackButtonClick
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler ShowParentDetailsButtonClick
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    private void ResetMouseState()
    {
        oldRow = -1;
        gridState[GRIDSTATE_overCaption] = true;
    }

    protected void ResetSelection()
    {
        if (numSelectedRows > 0)
        {
            DataGridRow[] localGridRows = DataGridRows;
            for (int i = 0; i < DataGridRowsLength; ++i)
                if (localGridRows[i].Selected)
                    localGridRows[i].Selected = false;
        }

        numSelectedRows = 0;
        lastRowSelected = -1;
    }

    private void ResetParentRows()
    {
        parentRows.Clear();
        originalState = null;
        caption.BackButtonActive = caption.DownButtonActive = caption.BackButtonVisible = false;
        caption.SetDownButtonDirection(!layout.ParentRowsVisible);
    }

    /// <devdoc>
    ///      Re-initializes all UI related state.
    /// </devdoc>
    private void ResetUIState()
    {
        gridState[GRIDSTATE_childLinkFocused] = false;
        ResetSelection();
        ResetMouseState();
        PerformLayout();
        Invalidate();               // we want to invalidate after we set up the scrollbars

        // invalidate the horizontalscrollbar and the vertical scrollbar
        //
        if (horizScrollBar.Visible)
            horizScrollBar.Invalidate();
        if (vertScrollBar.Visible)
            vertScrollBar.Invalidate();
    }

    /// <devdoc>
    ///      Scrolls the datagrid down an arbritrary number of rows.
    /// </devdoc>
    private void ScrollDown(int rows)
    {
        // Debug.WriteLineIf(CompModSwitches.DataGridScrolling.TraceVerbose, "DataGridScrolling: ScrollDown, rows = " + rows.ToString());
        if (rows != 0)
        {
            ClearRegionCache();

            // we should put "dataGridRowsLength -1"
            int newFirstRow = Math.Max(0, Math.Min(firstVisibleRow + rows, this.DataGridRowsLength - 1));
            int oldFirstRow = firstVisibleRow;
            firstVisibleRow = newFirstRow;
            vertScrollBar.Value = newFirstRow;
            bool wasEditing = this.gridState[GRIDSTATE_isEditing];
            ComputeVisibleRows();

            if (gridState[GRIDSTATE_isScrolling])
            {
                Edit();
                // isScrolling is set to TRUE when the user scrolls.
                // once we move the edit box, we finished processing the scroll event, so set isScrolling to FALSE
                // to set isScrolling to TRUE, we need another scroll event.
                gridState[GRIDSTATE_isScrolling] = false;
            }
            else
            {
                EndEdit();
            }

            int deltaY = ComputeRowDelta(oldFirstRow, newFirstRow);
            Rectangle rowsRect = layout.Data;
            if (layout.RowHeadersVisible)
                rowsRect = Rectangle.Union(rowsRect, layout.RowHeaders);
            RECT scrollArea = RECT.FromXYWH(rowsRect.X, rowsRect.Y, rowsRect.Width, rowsRect.Height);
            // SafeNativeMethods.ScrollWindow(new HandleRef(this, Handle), 0, deltaY, ref scrollArea, ref scrollArea);
            OnScroll(EventArgs.Empty);

            if (wasEditing)
            {
                // invalidate the rowHeader for the
                InvalidateRowHeader(currentRow);
            }
        }
    }

    /// <devdoc>
    ///      Scrolls the datagrid right an arbritrary number of columns.
    /// </devdoc>
    private void ScrollRight(int columns)
    {
        Debug.WriteLineIf(CompModSwitches.DataGridScrolling.TraceVerbose, "DataGridScrolling: ScrollRight, columns = " + columns.ToString(CultureInfo.InvariantCulture));
        int newCol = firstVisibleCol + columns;

        GridColumnStylesCollection gridColumns = myGridTable.GridColumnStyles;
        int newColOffset = 0;
        int nGridCols = gridColumns.Count;
        int nVisibleCols = 0;

        // if we try to scroll past the last totally visible column,
        // then the toolTips will dissapear
        if (this.myGridTable.IsDefault)
            nVisibleCols = nGridCols;
        else
            for (int i = 0; i < nGridCols; i++)
                if (gridColumns[i].PropertyDescriptor is not null)
                    nVisibleCols++;

        if ((lastTotallyVisibleCol == nVisibleCols - 1 && columns > 0) ||
            (firstVisibleCol == 0 && columns < 0 && negOffset == 0))
            return;

        newCol = Math.Min(newCol, nGridCols - 1);

        for (int i = 0; i < newCol; i++)
            // if (gridColumns[i].Visible && gridColumns[i].PropertyDescriptor is not null)
            if (gridColumns[i].PropertyDescriptor is not null)
                newColOffset += gridColumns[i].Width;

        HorizontalOffset = newColOffset;
    }

    /// <devdoc>
    ///      Scrolls a given column into visibility.
    /// </devdoc>
    private void ScrollToColumn(int targetCol)
    {
        // do not flush the columns to the left
        // so, scroll only as many columns as is necessary.
        // CONSIDER: after doing a sort, maybe the user would like to have
        // selected column flushed to the left
        int dCols = targetCol - firstVisibleCol;

        if (targetCol > lastTotallyVisibleCol && lastTotallyVisibleCol != -1)
            dCols = targetCol - lastTotallyVisibleCol;

        // if only part of the currentCol is visible
        // then we should still scroll
        if (dCols != 0 || negOffset != 0)
            ScrollRight(dCols);
    }

    public void Select(int row)
    {
        throw new PlatformNotSupportedException();
    }

    // this function will pair the listManager w/ a table from the TableStylesCollection.
    // and for each column in the TableStylesCollection will pair them w/ a propertyDescriptor
    // from the listManager
    // prerequisite: the current table is either the default table, or has the same name as the
    // list in the listManager.
    private void PairTableStylesAndGridColumns(CurrencyManager lm, DataGridTableStyle gridTable, bool forceColumnCreation)
    {
        PropertyDescriptorCollection props = lm.GetItemProperties();
        GridColumnStylesCollection gridCols = gridTable.GridColumnStyles;

        // ]it is possible to have a dataTable w/ an empty string for a name.
        if (!gridTable.IsDefault && string.Compare(lm.GetListName(), gridTable.MappingName, true, CultureInfo.InvariantCulture) == 0)
        {
            // we will force column creation only at runtime
            if (gridTable.GridColumnStyles.Count == 0 && !DesignMode)
            {
                // we have to create some default columns for each of the propertyDescriptors
                //
                if (forceColumnCreation)
                    gridTable.SetGridColumnStylesCollection(lm);
                else
                    gridTable.SetRelationsList(lm);
            }
            else
            {
                // it may the case that the user will have two lists w/ the same name.
                // When switching binding between those different lists, we need to invalidate
                // the propertyDescriptors from the current gridColumns
                for (int i = 0; i < gridCols.Count; i++)
                    gridCols[i].PropertyDescriptor = null;

                // pair the propertyDescriptor from each column to the actual property descriptor
                // from the listManager
                for (int i = 0; i < props.Count; i++)
                {
                    DataGridColumnStyle col = gridCols.MapColumnStyleToPropertyName(props[i].Name);
                    if (col is not null)
                    {
                        col.PropertyDescriptor = props[i];
                    }
                }

                // TableStyle::SetGridColumnStylesCollection will also set the
                // relations list in the tableStyle.
                gridTable.SetRelationsList(lm);
            }
        }
        else
        {
            // we should put an assert, that this is the default Table Style
#if DEBUG
            Debug.Assert(gridTable.IsDefault, "if we don't have a match, then the dataGRid should have the default table");
#endif // DEBUG
            gridTable.SetGridColumnStylesCollection(lm);
            if (gridTable.GridColumnStyles.Count > 0 && gridTable.GridColumnStyles[0].Width == -1)
            {
#if DEBUG
                GridColumnStylesCollection cols = gridTable.GridColumnStyles;
                for (int i = 0; i < cols.Count; i++)
                {
                    Debug.Assert(cols[i].Width == -1, "if one column's width is not initialized, the same should be happening for the rest of the columns");
                }
#endif // DEBUG
                InitializeColumnWidths();
            }
        }
    }

    /// <devdoc>
    ///      Sets the current GridTable for the DataGrid.
    ///      This GridTable is the table which is currently
    ///      being displayed on the grid.
    /// </devdoc>
    internal void SetDataGridTable(DataGridTableStyle newTable, bool forceColumnCreation)
    {
        // we have to listen to the dataGridTable for the propertyChangedEvent
        if (myGridTable is not null)
        {
            // unwire the propertyChanged event
            UnWireTableStylePropChanged(myGridTable);

            if (myGridTable.IsDefault)
            {
                // reset the propertyDescriptors on the default table.
                myGridTable.GridColumnStyles.ResetPropertyDescriptors();

                // reset the relationship list from the default table
                myGridTable.ResetRelationsList();
            }
        }

        myGridTable = newTable;

        WireTableStylePropChanged(myGridTable);

        layout.RowHeadersVisible = newTable.IsDefault ? RowHeadersVisible : newTable.RowHeadersVisible;

        // we need to force the grid into the dataGridTableStyle
        // this way the controls in the columns will be parented
        // consider this scenario: when the user finished InitializeComponent, it added
        // a bunch of tables. all of those tables will have the DataGrid property set to this
        // grid. however, in InitializeComponent the tables will not have parented the
        // edit controls w/ the grid.
        //
        // the code in DataGridTextBoxColumn already checks to see if the edits are parented
        // before parenting them.
        //
        if (newTable is not null)
            newTable.DataGrid = this;

        // pair the tableStyles and GridColumns
        //
        if (listManager is not null)
            PairTableStylesAndGridColumns(listManager, myGridTable, forceColumnCreation);

        // reset the relations UI on the newTable
        if (newTable is not null)
            newTable.ResetRelationsUI();

        // set the isNavigating to false
        gridState[GRIDSTATE_isNavigating] = false;

        horizScrollBar.Value = 0;
        firstVisibleRow = 0;
        currentCol = 0;
        // if we add a tableStyle that mapps to the
        // current listName, then we should set the currentRow to the
        // position in the listManager
        if (listManager is null)
            currentRow = 0;
        else
            currentRow = listManager.Position == -1 ? 0 : listManager.Position;
        ResetHorizontalOffset();
        negOffset = 0;
        ResetUIState();

        // check the hierarchy
        checkHierarchy = true;
    }

    /// <devdoc>
    ///      Scrolls the data area down to make room for the parent rows
    ///      and lays out the different regions of the DataGrid.
    /// </devdoc>
    internal void SetParentRowsVisibility(bool visible)
    {
        Rectangle parentRowsRect = layout.ParentRows;
        Rectangle underParentRows = layout.Data;

        if (layout.RowHeadersVisible)
        {
            underParentRows.X -= isRightToLeft() ? 0 : layout.RowHeaders.Width;
            underParentRows.Width += layout.RowHeaders.Width;
        }

        if (layout.ColumnHeadersVisible)
        {
            underParentRows.Y -= layout.ColumnHeaders.Height;
            underParentRows.Height += layout.ColumnHeaders.Height;
        }

        // hide the Edit Box
        EndEdit();

        if (visible)
        {
            layout.ParentRowsVisible = true;

            PerformLayout();

            Invalidate();
        }
        else
        {
            RECT scrollRECT = RECT.FromXYWH(underParentRows.X, underParentRows.Y - layout.ParentRows.Height, underParentRows.Width, underParentRows.Height + layout.ParentRows.Height);

            if (vertScrollBar.Visible)
            {
                Rectangle fixupRect = vertScrollBar.Bounds;
                fixupRect.Y -= parentRowsRect.Height;
                fixupRect.Height += parentRowsRect.Height;
                Invalidate(fixupRect);
            }

            Debug.WriteLineIf(CompModSwitches.DataGridParents.TraceVerbose, "DataGridParents: Making parent rows invisible.");
            layout.ParentRowsVisible = false;
            PerformLayout();
        }
    }

    /// <devdoc>
    ///      Sets whether a row is expanded or not.
    /// </devdoc>
    private void SetRowExpansionState(int row, bool expanded)
    {
        if (row < -1 || row > DataGridRowsLength - (policy.AllowAdd ? 2 : 1))
        {
            throw new ArgumentOutOfRangeException();
        }

        DataGridRow[] localGridRows = DataGridRows;
        if (row == -1)
        {
            DataGridRelationshipRow[] expandableRows = GetExpandableRows();
            bool repositionEditControl = false;

            for (int r = 0; r < expandableRows.Length; ++r)
            {
                if (expandableRows[r].Expanded != expanded)
                {
                    expandableRows[r].Expanded = expanded;
                    repositionEditControl = true;
                }
            }

            if (repositionEditControl)
            {
                // we need to reposition the edit control
                if (gridState[GRIDSTATE_isNavigating] || gridState[GRIDSTATE_isEditing])
                {
                    ResetSelection();
                    Edit();
                }
            }
        }
        else if (localGridRows[row] is DataGridRelationshipRow)
        {
            DataGridRelationshipRow expandableRow = (DataGridRelationshipRow)localGridRows[row];
            if (expandableRow.Expanded != expanded)
            {
                // we need to reposition the edit control
                if (gridState[GRIDSTATE_isNavigating] || gridState[GRIDSTATE_isEditing])
                {
                    ResetSelection();
                    Edit();
                }

                expandableRow.Expanded = expanded;
            }
        }
    }

    private void ObjectSiteChange(IContainer container, IComponent component, bool site)
    {
        if (site)
        {
            if (component.Site is null)
            {
                container.Add(component);
            }
        }
        else
        {
            if (component.Site is not null && component.Site.Container == container)
            {
                container.Remove(component);
            }
        }
    }

    public void SubObjectsSiteChange(bool site)
    {
        throw new PlatformNotSupportedException();
    }

    public void UnSelect(int row)
    {
        throw new PlatformNotSupportedException();
    }

    /// <devdoc>
    ///      Asks the cursor to update.
    /// </devdoc>
    private void UpdateListManager()
    {
        Debug.WriteLineIf(CompModSwitches.DataGridCursor.TraceVerbose, "DataGridCursor: Requesting EndEdit()");
        try
        {
            if (listManager is not null)
            {
                EndEdit();
                listManager.EndCurrentEdit();
            }
        }
        catch
        {
        }
    }

    protected virtual string GetOutputTextDelimiter()
    {
        return "\t";
    }

    /// <devdoc>
    ///     The accessible object class for a DataGrid. The child accessible objects
    ///     are accessible objects corresponding to the propertygrid entries.
    /// </devdoc>
    [Runtime.InteropServices.ComVisible(true)]
    [Obsolete("DataGridAccessibleObject has been deprecated.")]
    internal class DataGridAccessibleObject : ControlAccessibleObject
    {
        /// <devdoc>
        ///     Construct a PropertyGridViewAccessibleObject
        /// </devdoc>
        public DataGridAccessibleObject(DataGrid owner) : base(owner)
        {
            throw new PlatformNotSupportedException();
        }

        internal DataGrid DataGrid
        {
            get
            {
                return (DataGrid)Owner;
            }
        }

        private int ColumnCountPrivate
        {
            get
            {
                return ((DataGrid)Owner).myGridTable.GridColumnStyles.Count;
            }
        }

        private int RowCountPrivate
        {
            get
            {
                return ((DataGrid)Owner).dataGridRows.Length;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override string Name
        {
            get
            {
                throw new PlatformNotSupportedException();
            }

            set
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

        public override AccessibleObject GetChild(int index)
        {
            throw new PlatformNotSupportedException();
        }

        public override int GetChildCount()
        {
            throw new PlatformNotSupportedException();
        }

        public override AccessibleObject GetFocused()
        {
            throw new PlatformNotSupportedException();
        }

        public override AccessibleObject GetSelected()
        {
            throw new PlatformNotSupportedException();
        }

        public override AccessibleObject HitTest(int x, int y)
        {
            throw new PlatformNotSupportedException();
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public override AccessibleObject Navigate(AccessibleNavigation navdir)
        {
            throw new PlatformNotSupportedException();
        }
    }

    // <summary>
    //      This simple data structure holds all of the layout information
    //      for the DataGrid.
    // </summary>
    [Obsolete("LayoutData has been deprecated.")]
    internal class LayoutData
    {
        internal bool dirty = true;
        // region inside the Control's borders.
        public Rectangle Inside = Rectangle.Empty;

        public Rectangle RowHeaders = Rectangle.Empty;

        public Rectangle TopLeftHeader = Rectangle.Empty;
        public Rectangle ColumnHeaders = Rectangle.Empty;
        public Rectangle Data = Rectangle.Empty;

        public Rectangle Caption = Rectangle.Empty;
        public Rectangle ParentRows = Rectangle.Empty;

        public Rectangle ResizeBoxRect = Rectangle.Empty;

        public bool ColumnHeadersVisible;
        public bool RowHeadersVisible;
        public bool CaptionVisible;
        public bool ParentRowsVisible;

        // used for resizing.
        public Rectangle ClientRectangle = Rectangle.Empty;

        public LayoutData()
        {
            throw new PlatformNotSupportedException();
        }

        public LayoutData(LayoutData src)
        {
            throw new PlatformNotSupportedException();
        }

        private void GrabLayout(LayoutData src)
        {
            Inside = src.Inside;
            TopLeftHeader = src.TopLeftHeader;
            ColumnHeaders = src.ColumnHeaders;
            RowHeaders = src.RowHeaders;
            Data = src.Data;
            Caption = src.Caption;
            ParentRows = src.ParentRows;
            ResizeBoxRect = src.ResizeBoxRect;
            ColumnHeadersVisible = src.ColumnHeadersVisible;
            RowHeadersVisible = src.RowHeadersVisible;
            CaptionVisible = src.CaptionVisible;
            ParentRowsVisible = src.ParentRowsVisible;
            ClientRectangle = src.ClientRectangle;
        }

        public override string ToString()
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Obsolete("HitTestInfo has been deprecated.")]
    public sealed class HitTestInfo
    {
        internal HitTestType type = HitTestType.None;

        internal int row;
        internal int col;

        public static readonly HitTestInfo Nowhere = new HitTestInfo();

        internal HitTestInfo()
        {
            type = (HitTestType)0;
            row = col = -1;
        }

        internal HitTestInfo(HitTestType type)
        {
            this.type = type;
            row = col = -1;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int Column
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int Row
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public HitTestType Type
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        public override bool Equals(object value)
        {
            throw new PlatformNotSupportedException();
        }

        public override int GetHashCode()
        {
            throw new PlatformNotSupportedException();
        }

        public override string ToString()
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Flags]
    [Obsolete("HitTestType has been deprecated.")]
    public enum HitTestType
    {
        None = 0x00000000,

        Cell = 0x00000001,

        ColumnHeader = 0x00000002,

        RowHeader = 0x00000004,

        ColumnResize = 0x00000008,

        RowResize = 0x00000010,

        Caption = 0x00000020,

        ParentRows = 0x00000040
    }

    /// <devdoc>
    ///      Holds policy information for what the grid can and cannot do.
    /// </devdoc>
    private class Policy
    {
        private bool allowAdd = true;
        private bool allowEdit = true;
        private bool allowRemove = true;

        public Policy()
        {
        }

        public bool AllowAdd
        {
            get
            {
                return allowAdd;
            }
            set
            {
                if (allowAdd != value)
                {
                    allowAdd = value;
                }
            }
        }

        public bool AllowEdit
        {
            get
            {
                return allowEdit;
            }
            set
            {
                if (allowEdit != value)
                {
                    allowEdit = value;
                }
            }
        }

        public bool AllowRemove
        {
            get
            {
                return allowRemove;
            }
            set
            {
                if (allowRemove != value)
                {
                    allowRemove = value;
                }
            }
        }

        // returns true if the UI needs to be updated (here because addnew has changed)
        public bool UpdatePolicy(CurrencyManager listManager, bool gridReadOnly)
        {
            bool change = false;
            // only IBindingList can have an AddNewRow
            IBindingList bl = listManager is null ? null : listManager.List as IBindingList;
            if (listManager is null)
            {
                if (!allowAdd)
                    change = true;
                allowAdd = allowEdit = allowRemove = true;
            }
            else
            {
                if (AllowAdd != listManager.AllowAdd && !gridReadOnly)
                    change = true;
                AllowAdd = listManager.AllowAdd && !gridReadOnly && bl is not null && bl.SupportsChangeNotification;
                AllowEdit = listManager.AllowEdit && !gridReadOnly;
                AllowRemove = listManager.AllowRemove && !gridReadOnly && bl is not null && bl.SupportsChangeNotification;     // bug 86061
            }

            return change;
        }
    }

    private int MirrorRectangle(Rectangle R1, Rectangle rect, bool rightToLeft)
    {
        if (rightToLeft)
            return rect.Right + rect.X - R1.Right;
        else
            return R1.X;
    }

    private int MirrorPoint(int x, Rectangle rect, bool rightToLeft)
    {
        if (rightToLeft)
            return rect.Right + rect.X - x;
        else
            return x;
    }

    private bool isRightToLeft()
    {
        return (RightToLeft == RightToLeft.Yes);
    }
}
