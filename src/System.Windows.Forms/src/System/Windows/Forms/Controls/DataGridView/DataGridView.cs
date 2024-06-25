// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

[Designer($"System.Windows.Forms.Design.DataGridViewDesigner, {AssemblyRef.SystemDesign}")]
[DefaultEvent(nameof(CellContentClick))]
[ComplexBindingProperties(nameof(DataSource), nameof(DataMember))]
[Docking(DockingBehavior.Ask)]
[Editor($"System.Windows.Forms.Design.DataGridViewComponentEditor, {AssemblyRef.SystemDesign}", typeof(ComponentEditor))]
[SRDescription(nameof(SR.DescriptionDataGridView))]
public partial class DataGridView : Control, ISupportInitialize
{
    private static readonly object s_allowUserToAddRowsChangedEvent = new();
    private static readonly object s_allowUserToDeleteRowsChangedEvent = new();
    private static readonly object s_allowUserToOrderColumnsChangedEvent = new();
    private static readonly object s_allowUserToResizeColumnsChangedEvent = new();
    private static readonly object s_allowUserToResizeRowsChangedEvent = new();
    private static readonly object s_alternatingRowsDefaultCellStyleChangedEvent = new();
    private static readonly object s_autoGenerateColumnsChangedEvent = new();
    private static readonly object s_autosizeColumnModeChangedEvent = new();
    private static readonly object s_autosizeColumnsModeChangedEvent = new();
    private static readonly object s_autosizeRowsModeChangedEvent = new();
    private static readonly object s_backgroundColorChangedEvent = new();
    private static readonly object s_borderStyleChangedEvent = new();
    private static readonly object s_cancelRowEditEvent = new();
    private static readonly object s_cellBeginEditEvent = new();
    private static readonly object s_cellBorderStyleChangedEvent = new();
    private static readonly object s_cellClickEvent = new();
    private static readonly object s_cellContentClickEvent = new();
    private static readonly object s_cellContentDoubleClickEvent = new();
    private static readonly object s_cellContextMenuStripChangedEvent = new();
    private static readonly object s_cellContextMenuStripNeededEvent = new();
    private static readonly object s_cellDoubleClickEvent = new();
    private static readonly object s_cellEndEditEvent = new();
    private static readonly object s_cellEnterEvent = new();
    private static readonly object s_cellErrorTextChangedEvent = new();
    private static readonly object s_cellErrorTextNeededEvent = new();
    private static readonly object s_cellFormattingEvent = new();
    private static readonly object s_cellLeaveEvent = new();
    private static readonly object s_cellMouseClickEvent = new();
    private static readonly object s_cellMouseDoubleClickEvent = new();
    private static readonly object s_cellMouseDownEvent = new();
    private static readonly object s_cellMouseEnterEvent = new();
    private static readonly object s_cellMouseLeaveEvent = new();
    private static readonly object s_cellMouseMoveEvent = new();
    private static readonly object s_cellMouseUpEvent = new();
    private static readonly object s_cellPaintingEvent = new();
    private static readonly object s_cellParsingEvent = new();
    private static readonly object s_cellStateChangedEvent = new();
    private static readonly object s_cellStyleChangedEvent = new();
    private static readonly object s_cellStyleContentChangedEvent = new();
    private static readonly object s_cellTooltipTextChangedEvent = new();
    private static readonly object s_cellTooltipTextNeededEvent = new();
    private static readonly object s_cellValidatingEvent = new();
    private static readonly object s_cellValidatedEvent = new();
    private static readonly object s_cellValueChangedEvent = new();
    private static readonly object s_cellValueNeededEvent = new();
    private static readonly object s_cellValuePushedEvent = new();
    private static readonly object s_columnAddedEvent = new();
    private static readonly object s_columnContextMenuStripChangedEvent = new();
    private static readonly object s_columnDataPropertyNameChangedEvent = new();
    private static readonly object s_columnDefaultCellStyleChangedEvent = new();
    private static readonly object s_columnDisplayIndexChangedEvent = new();
    private static readonly object s_columnDividerWidthChangedEvent = new();
    private static readonly object s_columnHeaderCellChangedEvent = new();
    private static readonly object s_columnDividerDoubleClickEvent = new();
    private static readonly object s_columnHeaderMouseClickEvent = new();
    private static readonly object s_columnHeaderMouseDoubleClickEvent = new();
    private static readonly object s_columnHeadersBorderStyleChangedEvent = new();
    private static readonly object s_columnHeadersDefaultCellStyleChangedEvent = new();
    private static readonly object s_columnHeadersHeightChangedEvent = new();
    private static readonly object s_columnHeadersHeightSizeModeChangedEvent = new();
    private static readonly object s_columnMinimumWidthChangedEvent = new();
    private static readonly object s_columnNameChangedEvent = new();
    private static readonly object s_columnRemovedEvent = new();
    private static readonly object s_columnSortModeChangedEvent = new();
    private static readonly object s_columnStateChangedEvent = new();
    private static readonly object s_columnTooltipTextChangedEvent = new();
    private static readonly object s_columnWidthChangedEvent = new();
    private static readonly object s_currentCellChangedEvent = new();
    private static readonly object s_currentCellDirtyStateChangedEvent = new();
    private static readonly object s_dataBindingCompleteEvent = new();
    private static readonly object s_dataErrorEvent = new();
    private static readonly object s_dataMemberChangedEvent = new();
    private static readonly object s_dataSourceChangedEvent = new();
    private static readonly object s_defaultCellStyleChangedEvent = new();
    private static readonly object s_defaultValuesNeededEvent = new();
    private static readonly object s_editingControlShowingEvent = new();
    private static readonly object s_editModeChangedEvent = new();
    private static readonly object s_gridColorChangedEvent = new();
    private static readonly object s_multiselectChangedEvent = new();
    private static readonly object s_newRowNeededEvent = new();
    private static readonly object s_readOnlyChangedEvent = new();
    private static readonly object s_rowContextMenuStripChangedEvent = new();
    private static readonly object s_rowContextMenuStripNeededEvent = new();
    private static readonly object s_rowDefaultCellStyleChangedEvent = new();
    private static readonly object s_rowDirtyStateNeededEvent = new();
    private static readonly object s_rowDividerHeightChangedEvent = new();
    private static readonly object s_rowEnterEvent = new();
    private static readonly object s_rowErrorTextChangedEvent = new();
    private static readonly object s_rowErrorTextNeededEvent = new();
    private static readonly object s_rowHeaderCellChangedEvent = new();
    private static readonly object s_rowDividerDoubleClickEvent = new();
    private static readonly object s_rowHeaderMouseClickEvent = new();
    private static readonly object s_rowHeaderMouseDoubleClickEvent = new();
    private static readonly object s_rowHeadersBorderStyleChangedEvent = new();
    private static readonly object s_rowHeadersDefaultCellStyleChangedEvent = new();
    private static readonly object s_rowHeadersWidthChangedEvent = new();
    private static readonly object s_rowHeadersWidthSizeModeChangedEvent = new();
    private static readonly object s_rowHeightChangedEvent = new();
    private static readonly object s_rowHeightInfoNeededEvent = new();
    private static readonly object s_rowHeightInfoPushedEvent = new();
    private static readonly object s_rowLeaveEvent = new();
    private static readonly object s_rowMinimumHeightChangeEvent = new();
    private static readonly object s_rowPostPaintEvent = new();
    private static readonly object s_rowPrePaintEvent = new();
    private static readonly object s_rowsAddedEvent = new();
    private static readonly object s_rowsDefaultCellStyleChangedEvent = new();
    private static readonly object s_rowsRemovedEvent = new();
    private static readonly object s_rowStateChangedEvent = new();
    private static readonly object s_rowUnsharedEvent = new();
    private static readonly object s_rowValidatedEvent = new();
    private static readonly object s_rowValidatingEvent = new();
    private static readonly object s_scrollEvent = new();
    private static readonly object s_selectionChangedEvent = new();
    private static readonly object s_sortCompareEvent = new();
    private static readonly object s_sortedEvent = new();
    private static readonly object s_userAddedRowEvent = new();
    private static readonly object s_userDeletedRowEvent = new();
    private static readonly object s_userDeletingRowEvent = new();

    private const int State1_AllowUserToAddRows = 0x00000001;
    private const int State1_AllowUserToDeleteRows = 0x00000002;
    private const int State1_AllowUserToOrderColumns = 0x00000004;
    private const int State1_ColumnHeadersVisible = 0x00000008;
    private const int State1_RowHeadersVisible = 0x00000010;
    private const int State1_ForwardCharMessage = 0x00000020;
    private const int State1_LeavingWithTabKey = 0x00000040;
    private const int State1_MultiSelect = 0x00000080;
    private const int State1_IgnoringEditingChanges = 0x00000200;
    private const int State1_AmbientForeColor = 0x00000400;
    private const int State1_ScrolledSinceMouseDown = 0x00000800;
    private const int State1_EditingControlHidden = 0x00001000;
    private const int State1_StandardTab = 0x00002000;
    private const int State1_EditingControlChanging = 0x00004000;
    private const int State1_CurrentCellInEditMode = 0x00008000;
    private const int State1_VirtualMode = 0x00010000;
    private const int State1_EditedCellChanged = 0x00020000;
    private const int State1_EditedRowChanged = 0x00040000;
    private const int State1_NewRowEdited = 0x00080000;
    private const int State1_ReadOnly = 0x00100000;
    private const int State1_NewRowCreatedByEditing = 0x00200000;
    private const int State1_TemporarilyResetCurrentCell = 0x00400000;
    private const int State1_AutoGenerateColumns = 0x00800000;
    private const int State1_CustomCursorSet = 0x01000000;
    private const int State1_AmbientFont = 0x02000000;
    private const int State1_AmbientColumnHeadersFont = 0x04000000;
    private const int State1_AmbientRowHeadersFont = 0x08000000;
    private const int State1_IsAutoSized = 0x40000000;

    // State2_
    private const int State2_ShowEditingIcon = 0x00000001;
    private const int State2_AllowUserToResizeColumns = 0x00000002;
    private const int State2_AllowUserToResizeRows = 0x00000004;
    private const int State2_MouseOverRemovedEditingCtrl = 0x00000008;
    private const int State2_MouseOverRemovedEditingPanel = 0x00000010;
    private const int State2_MouseEnterExpected = 0x00000020;
    private const int State2_EnableHeadersVisualStyles = 0x00000040;
    private const int State2_ShowCellErrors = 0x00000080;
    private const int State2_ShowCellToolTips = 0x00000100;
    private const int State2_ShowRowErrors = 0x00000200;
    private const int State2_ShowColumnRelocationInsertion = 0x00000400;
    private const int State2_RightToLeftMode = 0x00000800;
    private const int State2_RightToLeftValid = 0x00001000;
    private const int State2_CurrentCellWantsInputKey = 0x00002000;
    private const int State2_StopRaisingVerticalScroll = 0x00004000;
    private const int State2_StopRaisingHorizontalScroll = 0x00008000;
    private const int State2_ReplacedCellSelected = 0x00010000;
    private const int State2_ReplacedCellReadOnly = 0x00020000;
    private const int State2_RaiseSelectionChanged = 0x00040000;
    private const int State2_Initializing = 0x00080000;
    private const int State2_AutoSizedWithoutHandle = 0x00100000;
    private const int State2_IgnoreCursorChange = 0x00200000;
    private const int State2_RowsCollectionClearedInSetCell = 0x00400000;
    private const int State2_NextMouseUpIsDouble = 0x00800000;
    private const int State2_InBindingContextChanged = 0x01000000;
    private const int State2_AllowHorizontalScrollbar = 0x02000000;
    private const int State2_UsedFillWeightsDirty = 0x04000000;
    private const int State2_MessageFromEditingCtrls = 0x08000000;
    private const int State2_CellMouseDownInContentBounds = 0x10000000;
    private const int State2_DiscardEditingControl = 0x20000000;

    // Operation
    private const int OperationTrackColResize = 0x00000001;
    private const int OperationTrackRowResize = 0x00000002;
    private const int OperationTrackColSelect = 0x00000004;
    private const int OperationTrackRowSelect = 0x00000008;
    private const int OperationTrackCellSelect = 0x00000010;
    private const int OperationTrackColRelocation = 0x00000020;
    private const int OperationInSort = 0x00000040;
    private const int OperationTrackColHeadersResize = 0x00000080;
    private const int OperationTrackRowHeadersResize = 0x00000100;
    private const int OperationTrackMouseMoves = 0x00000200;
    private const int OperationInRefreshColumns = 0x00000400;
    private const int OperationInDisplayIndexAdjustments = 0x00000800;
    private const int OperationLastEditCtrlClickDoubled = 0x00001000;
    private const int OperationInMouseDown = 0x00002000;
    private const int OperationInReadOnlyChange = 0x00004000;
    private const int OperationInCellValidating = 0x00008000;
    private const int OperationInBorderStyleChange = 0x00010000;
    private const int OperationInCurrentCellChange = 0x00020000;
    private const int OperationInAdjustFillingColumns = 0x00040000;
    private const int OperationInAdjustFillingColumn = 0x00080000;
    private const int OperationInDispose = 0x00100000;
    private const int OperationInBeginEdit = 0x00200000;
    private const int OperationInEndEdit = 0x00400000;
    private const int OperationResizingOperationAboutToStart = 0x00800000;
    private const int OperationTrackKeyboardColResize = 0x01000000;
    private const int OperationMouseOperationMask = OperationTrackColResize | OperationTrackRowResize |
        OperationTrackColRelocation | OperationTrackColHeadersResize | OperationTrackRowHeadersResize;
    private const int OperationKeyboardOperationMask = OperationTrackKeyboardColResize;

    private static readonly Size s_dragSize = SystemInformation.DragSize;

    private const byte ColumnSizingHotZone = 6;
    private const byte RowSizingHotZone = 5;
    private const byte InsertionBarWidth = 3;
    private const byte BulkPaintThreshold = 8;

    private const int FocusRectOffset = 2;

    private BitVector32 _dataGridViewState1;  // see State1_ consts above
    private BitVector32 _dataGridViewState2;  // see State2_ consts above
    private BitVector32 _dataGridViewOper;    // see Operation consts above

    private const BorderStyle DefaultBorderStyle = BorderStyle.FixedSingle;
    private const DataGridViewAdvancedCellBorderStyle DefaultAdvancedCellBorderStyle
        = DataGridViewAdvancedCellBorderStyle.Single;
    private const DataGridViewAdvancedCellBorderStyle DefaultAdvancedRowHeadersBorderStyle
        = DataGridViewAdvancedCellBorderStyle.OutsetPartial;
    private const DataGridViewAdvancedCellBorderStyle DefaultAdvancedColumnHeadersBorderStyle
        = DataGridViewAdvancedCellBorderStyle.OutsetPartial;

    private const DataGridViewSelectionMode DefaultSelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
    private const DataGridViewEditMode DefaultEditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;

    private const DataGridViewAutoSizeRowCriteriaInternal InvalidDataGridViewAutoSizeRowCriteriaInternalMask
        = ~(DataGridViewAutoSizeRowCriteriaInternal.Header | DataGridViewAutoSizeRowCriteriaInternal.AllColumns);
    private Cursor? _oldCursor;

    private HScrollBar _horizScrollBar = new();
    private VScrollBar _vertScrollBar = new();
    private DataGridViewHeaderCell? _topLeftHeaderCell;

    private DataGridViewRow? _rowTemplate;
    private DataGridViewRowCollection? _dataGridViewRows;
    private DataGridViewColumnCollection? _dataGridViewColumns;

    private DataGridViewCellStyle? _placeholderCellStyle;
    private StringFormat? _placeholderStringFormat;
    private object? _uneditedFormattedValue;
    private Control? _latestEditingControl;
    private Control? _cachedEditingControl;
    private DataGridViewEditingPanel? _editingPanel;
    private DataGridViewEditingPanelAccessibleObject? _editingPanelAccessibleObject;
    private Point _ptCurrentCell;
    private Point _ptCurrentCellCache = Point.Empty;
    private Point _ptAnchorCell;
    private Point _ptMouseDownCell;
    private Point _ptMouseEnteredCell;
    private Point _ptToolTipCell;
    private Point _ptMouseDownGridCoord;

    private DataGridViewSelectionMode _selectionMode;
    private DataGridViewEditMode _editMode;

    // Note that a cell can only be in one bag but not both at the same time.
    private readonly DataGridViewCellLinkedList _individualSelectedCells;
    private readonly DataGridViewCellLinkedList _individualReadOnlyCells;
    private readonly DataGridViewIntLinkedList _selectedBandIndexes;
    private DataGridViewIntLinkedList? _selectedBandSnapshotIndexes;

    private DataGridViewCellStyle? _defaultCellStyle;
    private DataGridViewCellStyle? _columnHeadersDefaultCellStyle;
    private DataGridViewCellStyle? _rowHeadersDefaultCellStyle;
    private DataGridViewCellStyle? _rowsDefaultCellStyle;
    private DataGridViewCellStyle? _alternatingRowsDefaultCellStyle;
    private ScrollBars _scrollBars;
    private LayoutData _layout;
    private Rectangle _normalClientRectangle;
    private readonly List<DataGridViewRow> _lstRows;
    private int _availableWidthForFillColumns;

    private BorderStyle _borderStyle;
    private DataGridViewClipboardCopyMode _clipboardCopyMode;

    private const int MinimumRowHeadersWidth = 4;
    private const int MinimumColumnHeadersHeight = 4;
    private const int DefaultRowHeadersWidth = 41;
    private const int MaxHeadersThickness = 32768;
    private const int UpperSize = 0x007FFFFF;
    private int _cachedRowHeadersWidth;
    private int _rowHeaderWidth;
    private const int DefaultColumnHeadersHeight = 23;
    private int _columnHeadersHeight = DefaultColumnHeadersHeight;
    private int _cachedColumnHeadersHeight;
    private DataGridViewAutoSizeRowsMode _autoSizeRowsMode;
    private DataGridViewAutoSizeColumnsMode _autoSizeColumnsMode;
    private DataGridViewColumnHeadersHeightSizeMode _columnHeadersHeightSizeMode;
    private DataGridViewRowHeadersWidthSizeMode _rowHeadersWidthSizeMode;

    private DataGridViewCellStyleChangedEventArgs? _dgvcsce;
    private DataGridViewCellPaintingEventArgs? _dgvcpe;
    private DataGridViewCellValueEventArgs? _dgvcve;
    private DataGridViewRowHeightInfoNeededEventArgs? _dgvrhine;
    private DataGridViewRowPostPaintEventArgs? _dgvrpope;
    private DataGridViewRowPrePaintEventArgs? _dgvrprpe;

    // The sum of the widths in pixels of the scrolling columns preceding the first visible scrolling column.
    private int _horizontalOffset;

    // Residual fraction of WHEEL_DELTA (120) for wheel scrolling
    private int _cumulativeVerticalWheelDelta;
    private int _cumulativeHorizontalWheelDelta;

    private int _trackColAnchor;
    private int _trackColumn = -1;
    private int _trackColumnEdge = -1;
    private int _trackRowAnchor;
    private int _trackRow = -1;
    private int _trackRowEdge = -1;
    private int _lastHeaderShadow = -1;
    private int _currentColSplitBar = -1, _lastColSplitBar = -1;
    private int _currentRowSplitBar = -1, _lastRowSplitBar = -1;
    private int _mouseBarOffset;
    private int _noDimensionChangeCount;
    private int _noSelectionChangeCount;
    private int _noAutoSizeCount;
    private int _inBulkPaintCount;
    private int _inBulkLayoutCount;
    private int _inPerformLayoutCount;

    private int _keyboardResizeStep;
    private Rectangle _resizeClipRectangle;

    private Timer? _vertScrollTimer;
    private Timer? _horizScrollTimer;

    private readonly Dictionary<Type, TypeConverter> _converters;
    private static readonly Color s_defaultBackColor = Application.ApplicationColors.Window;
    private static readonly Color s_defaultBackgroundColor = Application.ApplicationColors.ControlDark;
    private Color _backgroundColor = s_defaultBackgroundColor;

    private RECT[]? _cachedScrollableRegion;

    // ToolTip
    private readonly DataGridViewToolTip _toolTipControl;
    private static readonly int s_propToolTip = PropertyStore.CreateKey();

    // Last Mouse Click Info
    private MouseClickInfo _lastMouseClickInfo;

    private ToolTipBuffer _toolTipBuffer;

#if DEBUG
    // set to false when the grid is not in sync with the underlying data store
    // in virtual mode, and OnCellValueNeeded cannot be called.
    // disable csharp compiler warning #0414: field assigned unused value
#pragma warning disable 0414
    internal bool _dataStoreAccessAllowed = true;
#pragma warning restore 0414
#endif
    /// <summary>
    ///  Initializes a new instance of the <see cref="DataGridView"/> class.
    /// </summary>
    public DataGridView()
    {
        SetStyle(ControlStyles.UserPaint |
                 ControlStyles.Opaque |
                 ControlStyles.UserMouse, true);

        SetStyle(ControlStyles.SupportsTransparentBackColor, false);

        // this class overrides GetPreferredSizeCore, let Control automatically cache the result
        SetExtendedState(ExtendedStates.UserPreferredSizeCache, true);

        _dataGridViewState1 = new BitVector32(0x00000000);
        _dataGridViewState2 = new BitVector32(0x00000000);
        _dataGridViewOper = new BitVector32(0x00000000);

        _dataGridViewState1[State1_ColumnHeadersVisible
                                | State1_RowHeadersVisible
                                | State1_AutoGenerateColumns
                                | State1_AllowUserToAddRows
                                | State1_AllowUserToDeleteRows] = true;

        _dataGridViewState2[State2_ShowEditingIcon
                                | State2_EnableHeadersVisualStyles
                                | State2_MouseEnterExpected
                                | State2_AllowUserToResizeColumns
                                | State2_AllowUserToResizeRows
                                | State2_ShowCellToolTips
                                | State2_ShowCellErrors
                                | State2_ShowRowErrors
                                | State2_AllowHorizontalScrollbar
                                | State2_UsedFillWeightsDirty] = true;

        DisplayedBandsInfo = new DisplayedBandsData();
        _lstRows = [];

        _converters = new(8);
        GridPenColor = DefaultGridColor;

        _selectedBandIndexes = [];
        _individualSelectedCells = [];
        _individualReadOnlyCells = [];

        AdvancedCellBorderStyle = new DataGridViewAdvancedBorderStyle(this,
            DataGridViewAdvancedCellBorderStyle.OutsetDouble,
            DataGridViewAdvancedCellBorderStyle.OutsetPartial,
            DataGridViewAdvancedCellBorderStyle.InsetDouble);
        AdvancedRowHeadersBorderStyle = new DataGridViewAdvancedBorderStyle(this);
        AdvancedColumnHeadersBorderStyle = new DataGridViewAdvancedBorderStyle(this);
        AdvancedCellBorderStyle.All = DefaultAdvancedCellBorderStyle;
        AdvancedRowHeadersBorderStyle.All = DefaultAdvancedRowHeadersBorderStyle;
        AdvancedColumnHeadersBorderStyle.All = DefaultAdvancedColumnHeadersBorderStyle;
        _borderStyle = DefaultBorderStyle;
        _dataGridViewState1[State1_MultiSelect] = true;
        _selectionMode = DefaultSelectionMode;
        _editMode = DefaultEditMode;
        _autoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        _autoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        _columnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
        _rowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;

        _clipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithAutoHeaderText;

        _layout = new LayoutData
        {
            TopLeftHeader = Rectangle.Empty,
            ColumnHeaders = Rectangle.Empty,
            RowHeaders = Rectangle.Empty,
            ColumnHeadersVisible = true,
            RowHeadersVisible = true,
            ClientRectangle = ClientRectangle
        };

        _scrollBars = ScrollBars.Both;

        _horizScrollBar.RightToLeft = RightToLeft.Inherit;
        _horizScrollBar.AccessibleName = SR.DataGridView_AccHorizontalScrollBarAccName;
        _horizScrollBar.Top = ClientRectangle.Height - _horizScrollBar.Height;
        _horizScrollBar.Left = 0;
        _horizScrollBar.Visible = false;
        _horizScrollBar.Scroll += new ScrollEventHandler(DataGridViewHScrolled);
        Controls.Add(_horizScrollBar);

        _vertScrollBar.Top = 0;
        _vertScrollBar.AccessibleName = SR.DataGridView_AccVerticalScrollBarAccName;
        _vertScrollBar.Left = ClientRectangle.Width - _vertScrollBar.Width;
        _vertScrollBar.Visible = false;
        _vertScrollBar.Scroll += new ScrollEventHandler(DataGridViewVScrolled);
        Controls.Add(_vertScrollBar);

        _ptCurrentCell = new Point(-1, -1);
        _ptAnchorCell = new Point(-1, -1);
        _ptMouseDownCell = new Point(-2, -2);
        _ptMouseEnteredCell = new Point(-2, -2);
        _ptToolTipCell = new Point(-1, -1);
        _ptMouseDownGridCoord = new Point(-1, -1);

        SortOrder = SortOrder.None;

        _lastMouseClickInfo.TimeStamp = 0;

        WireScrollBarsEvents();
        PerformLayout();

        _toolTipControl = new DataGridViewToolTip(this);
        RowHeadersWidth = LogicalToDeviceUnits(DefaultRowHeadersWidth);
        _columnHeadersHeight = LogicalToDeviceUnits(DefaultColumnHeadersHeight);
        Invalidate();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual DataGridViewAdvancedBorderStyle AdjustedTopLeftHeaderBorderStyle
    {
        get
        {
            DataGridViewAdvancedBorderStyle dgvabs;
            if (ApplyVisualStylesToHeaderCells)
            {
                switch (AdvancedColumnHeadersBorderStyle.All)
                {
                    case DataGridViewAdvancedCellBorderStyle.OutsetDouble:
                    case DataGridViewAdvancedCellBorderStyle.OutsetPartial:
                        dgvabs = new DataGridViewAdvancedBorderStyle();
                        if (RightToLeftInternal)
                        {
                            dgvabs.LeftInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                        }
                        else
                        {
                            dgvabs.LeftInternal = DataGridViewAdvancedCellBorderStyle.OutsetDouble;
                        }

                        dgvabs.RightInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                        dgvabs.TopInternal = DataGridViewAdvancedCellBorderStyle.OutsetDouble;
                        dgvabs.BottomInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                        break;

                    case DataGridViewAdvancedCellBorderStyle.InsetDouble:
                        dgvabs = new DataGridViewAdvancedBorderStyle();
                        if (RightToLeftInternal)
                        {
                            dgvabs.LeftInternal = DataGridViewAdvancedCellBorderStyle.Inset;
                        }
                        else
                        {
                            dgvabs.LeftInternal = DataGridViewAdvancedCellBorderStyle.InsetDouble;
                        }

                        dgvabs.RightInternal = DataGridViewAdvancedCellBorderStyle.Inset;
                        dgvabs.TopInternal = DataGridViewAdvancedCellBorderStyle.InsetDouble;
                        dgvabs.BottomInternal = DataGridViewAdvancedCellBorderStyle.Inset;
                        break;

                    case DataGridViewAdvancedCellBorderStyle.NotSet:
                        // Since the row headers are visible, we should make sure
                        // that there is a left/right border for the TopLeftHeaderCell no matter what.
                        if ((!RightToLeftInternal && AdvancedColumnHeadersBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.None)
                            || (RightToLeftInternal && AdvancedColumnHeadersBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.None))
                        {
                            dgvabs = new DataGridViewAdvancedBorderStyle();
                            if (RightToLeftInternal)
                            {
                                dgvabs.LeftInternal = AdvancedColumnHeadersBorderStyle.Left;
                                dgvabs.RightInternal = AdvancedRowHeadersBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.OutsetDouble ?
                                    DataGridViewAdvancedCellBorderStyle.Outset : AdvancedRowHeadersBorderStyle.Right;
                            }
                            else
                            {
                                dgvabs.LeftInternal = AdvancedRowHeadersBorderStyle.Left;
                                dgvabs.RightInternal = AdvancedColumnHeadersBorderStyle.Right;
                            }

                            dgvabs.TopInternal = AdvancedColumnHeadersBorderStyle.Top;
                            dgvabs.BottomInternal = AdvancedColumnHeadersBorderStyle.Bottom;
                        }
                        else
                        {
                            dgvabs = AdvancedColumnHeadersBorderStyle;
                        }

                        break;

                    default:
                        dgvabs = AdvancedColumnHeadersBorderStyle;
                        break;
                }
            }
            else
            {
                switch (AdvancedColumnHeadersBorderStyle.All)
                {
                    case DataGridViewAdvancedCellBorderStyle.OutsetDouble:
                    case DataGridViewAdvancedCellBorderStyle.OutsetPartial:
                        dgvabs = new DataGridViewAdvancedBorderStyle
                        {
                            LeftInternal = RightToLeftInternal ? DataGridViewAdvancedCellBorderStyle.Outset : DataGridViewAdvancedCellBorderStyle.OutsetDouble,
                            RightInternal = RightToLeftInternal ? DataGridViewAdvancedCellBorderStyle.OutsetDouble : DataGridViewAdvancedCellBorderStyle.Outset,
                            TopInternal = DataGridViewAdvancedCellBorderStyle.OutsetDouble,
                            BottomInternal = DataGridViewAdvancedCellBorderStyle.Outset
                        };
                        break;

                    case DataGridViewAdvancedCellBorderStyle.InsetDouble:
                        dgvabs = new DataGridViewAdvancedBorderStyle
                        {
                            LeftInternal = RightToLeftInternal ? DataGridViewAdvancedCellBorderStyle.Inset : DataGridViewAdvancedCellBorderStyle.InsetDouble,
                            RightInternal = RightToLeftInternal ? DataGridViewAdvancedCellBorderStyle.InsetDouble : DataGridViewAdvancedCellBorderStyle.Inset,
                            TopInternal = DataGridViewAdvancedCellBorderStyle.InsetDouble,
                            BottomInternal = DataGridViewAdvancedCellBorderStyle.Inset
                        };
                        break;

                    case DataGridViewAdvancedCellBorderStyle.NotSet:
                        // Since the row headers are visible, we should make sure
                        // that there is a left/right border for the TopLeftHeaderCell no matter what.
                        if ((!RightToLeftInternal && AdvancedColumnHeadersBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.None)
                            || (RightToLeftInternal && AdvancedColumnHeadersBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.None))
                        {
                            dgvabs = new DataGridViewAdvancedBorderStyle();
                            if (RightToLeftInternal)
                            {
                                dgvabs.LeftInternal = AdvancedColumnHeadersBorderStyle.Left;
                                dgvabs.RightInternal = AdvancedRowHeadersBorderStyle.Right;
                            }
                            else
                            {
                                dgvabs.LeftInternal = AdvancedRowHeadersBorderStyle.Left;
                                dgvabs.RightInternal = AdvancedColumnHeadersBorderStyle.Right;
                            }

                            dgvabs.TopInternal = AdvancedColumnHeadersBorderStyle.Top;
                            dgvabs.BottomInternal = AdvancedColumnHeadersBorderStyle.Bottom;
                        }
                        else
                        {
                            dgvabs = AdvancedColumnHeadersBorderStyle;
                        }

                        break;

                    default:
                        dgvabs = AdvancedColumnHeadersBorderStyle;
                        break;
                }
            }

            return dgvabs;
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public DataGridViewAdvancedBorderStyle AdvancedCellBorderStyle { get; }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public DataGridViewAdvancedBorderStyle AdvancedColumnHeadersBorderStyle { get; }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public DataGridViewAdvancedBorderStyle AdvancedRowHeadersBorderStyle { get; }

    [DefaultValue(true)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DataGridView_AllowUserToAddRowsDescr))]
    public bool AllowUserToAddRows
    {
        get => _dataGridViewState1[State1_AllowUserToAddRows];
        set
        {
            if (AllowUserToAddRows != value)
            {
                _dataGridViewState1[State1_AllowUserToAddRows] = value;
                if (DataSource is not null)
                {
                    DataConnection!.ResetCachedAllowUserToAddRowsInternal();
                }

                OnAllowUserToAddRowsChanged(EventArgs.Empty);
            }
        }
    }

    internal bool AllowUserToAddRowsInternal => DataSource is null
        ? AllowUserToAddRows
        : AllowUserToAddRows && DataConnection!.AllowAdd;

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridViewOnAllowUserToAddRowsChangedDescr))]
    public event EventHandler? AllowUserToAddRowsChanged
    {
        add => Events.AddHandler(s_allowUserToAddRowsChangedEvent, value);
        remove => Events.RemoveHandler(s_allowUserToAddRowsChangedEvent, value);
    }

    [DefaultValue(true)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DataGridView_AllowUserToDeleteRowsDescr))]
    public bool AllowUserToDeleteRows
    {
        get => _dataGridViewState1[State1_AllowUserToDeleteRows];
        set
        {
            if (AllowUserToDeleteRows != value)
            {
                _dataGridViewState1[State1_AllowUserToDeleteRows] = value;
                OnAllowUserToDeleteRowsChanged(EventArgs.Empty);
            }
        }
    }

    internal bool AllowUserToDeleteRowsInternal => DataSource is null
        ? AllowUserToDeleteRows
        : AllowUserToDeleteRows && DataConnection!.AllowRemove;

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridViewOnAllowUserToDeleteRowsChangedDescr))]
    public event EventHandler? AllowUserToDeleteRowsChanged
    {
        add => Events.AddHandler(s_allowUserToDeleteRowsChangedEvent, value);
        remove => Events.RemoveHandler(s_allowUserToDeleteRowsChangedEvent, value);
    }

    [DefaultValue(false)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DataGridView_AllowUserToOrderColumnsDescr))]
    public bool AllowUserToOrderColumns
    {
        get
        {
            return _dataGridViewState1[State1_AllowUserToOrderColumns];
        }
        set
        {
            if (AllowUserToOrderColumns != value)
            {
                _dataGridViewState1[State1_AllowUserToOrderColumns] = value;
                OnAllowUserToOrderColumnsChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridViewOnAllowUserToOrderColumnsChangedDescr))]
    public event EventHandler? AllowUserToOrderColumnsChanged
    {
        add => Events.AddHandler(s_allowUserToOrderColumnsChangedEvent, value);
        remove => Events.RemoveHandler(s_allowUserToOrderColumnsChangedEvent, value);
    }

    /// <summary>
    ///  Gets or sets a global value indicating if the dataGridView's columns are resizable with the mouse.
    ///  The resizable aspect of a column can be overridden by DataGridViewColumn.Resizable.
    /// </summary>
    [DefaultValue(true)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DataGridView_AllowUserToResizeColumnsDescr))]
    public bool AllowUserToResizeColumns
    {
        get => _dataGridViewState2[State2_AllowUserToResizeColumns];
        set
        {
            if (AllowUserToResizeColumns != value)
            {
                _dataGridViewState2[State2_AllowUserToResizeColumns] = value;
                OnAllowUserToResizeColumnsChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridViewOnAllowUserToResizeColumnsChangedDescr))]
    public event EventHandler? AllowUserToResizeColumnsChanged
    {
        add => Events.AddHandler(s_allowUserToResizeColumnsChangedEvent, value);
        remove => Events.RemoveHandler(s_allowUserToResizeColumnsChangedEvent, value);
    }

    /// <summary>
    ///  Gets or sets a global value indicating if the dataGridView's rows are resizable with the mouse.
    ///  The resizable aspect of a row can be overridden by DataGridViewRow.Resizable.
    /// </summary>
    [DefaultValue(true)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DataGridView_AllowUserToResizeRowsDescr))]
    public bool AllowUserToResizeRows
    {
        get => _dataGridViewState2[State2_AllowUserToResizeRows];
        set
        {
            if (AllowUserToResizeRows != value)
            {
                _dataGridViewState2[State2_AllowUserToResizeRows] = value;
                OnAllowUserToResizeRowsChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridViewOnAllowUserToResizeRowsChangedDescr))]
    public event EventHandler? AllowUserToResizeRowsChanged
    {
        add => Events.AddHandler(s_allowUserToResizeRowsChangedEvent, value);
        remove => Events.RemoveHandler(s_allowUserToResizeRowsChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_AlternatingRowsDefaultCellStyleDescr))]
    [AllowNull]
    public DataGridViewCellStyle AlternatingRowsDefaultCellStyle
    {
        get
        {
            if (_alternatingRowsDefaultCellStyle is null)
            {
                _alternatingRowsDefaultCellStyle = new DataGridViewCellStyle();
                _alternatingRowsDefaultCellStyle.AddScope(this, DataGridViewCellStyleScopes.AlternatingRows);
            }

            return _alternatingRowsDefaultCellStyle;
        }
        set
        {
            DataGridViewCellStyle cs = AlternatingRowsDefaultCellStyle;
            cs.RemoveScope(DataGridViewCellStyleScopes.AlternatingRows);
            _alternatingRowsDefaultCellStyle = value;
            _alternatingRowsDefaultCellStyle?.AddScope(this, DataGridViewCellStyleScopes.AlternatingRows);

            DataGridViewCellStyleDifferences dgvcsc = cs.GetDifferencesFrom(AlternatingRowsDefaultCellStyle);
            if (dgvcsc != DataGridViewCellStyleDifferences.None)
            {
                CellStyleChangedEventArgs.ChangeAffectsPreferredSize = (dgvcsc == DataGridViewCellStyleDifferences.AffectPreferredSize);
                OnAlternatingRowsDefaultCellStyleChanged(CellStyleChangedEventArgs);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridViewAlternatingRowsDefaultCellStyleChangedDescr))]
    public event EventHandler? AlternatingRowsDefaultCellStyleChanged
    {
        add => Events.AddHandler(s_alternatingRowsDefaultCellStyleChangedEvent, value);
        remove => Events.RemoveHandler(s_alternatingRowsDefaultCellStyleChangedEvent, value);
    }

    internal static bool ApplyVisualStylesToInnerCells => Application.RenderWithVisualStyles;

    internal bool ApplyVisualStylesToHeaderCells => Application.RenderWithVisualStyles && EnableHeadersVisualStyles;

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DefaultValue(true)]
    public bool AutoGenerateColumns
    {
        get => _dataGridViewState1[State1_AutoGenerateColumns];
        set
        {
            if (_dataGridViewState1[State1_AutoGenerateColumns] != value)
            {
                _dataGridViewState1[State1_AutoGenerateColumns] = value;
                OnAutoGenerateColumnsChanged(EventArgs.Empty);
            }
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public event EventHandler? AutoGenerateColumnsChanged
    {
        add => Events.AddHandler(s_autoGenerateColumnsChangedEvent, value);
        remove => Events.RemoveHandler(s_autoGenerateColumnsChangedEvent, value);
    }

    /// <summary>
    ///  Overriding base implementation for perf gains.
    /// </summary>
    public override bool AutoSize
    {
        get => _dataGridViewState1[State1_IsAutoSized];
        set
        {
            base.AutoSize = value;
            _dataGridViewState1[State1_IsAutoSized] = value;
        }
    }

    /// <summary>
    ///  Gets or sets the columns' autosizing mode. Standard inheritance model is used:
    ///  Columns with AutoSizeMode property set to NotSet will use this auto size mode.
    /// </summary>
    [DefaultValue(DataGridViewAutoSizeColumnsMode.None)]
    [SRCategory(nameof(SR.CatLayout))]
    [SRDescription(nameof(SR.DataGridView_AutoSizeColumnsModeDescr))]
    public DataGridViewAutoSizeColumnsMode AutoSizeColumnsMode
    {
        get => _autoSizeColumnsMode;
        set
        {
            switch (value)
            {
                case DataGridViewAutoSizeColumnsMode.None:
                case DataGridViewAutoSizeColumnsMode.ColumnHeader:
                case DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader:
                case DataGridViewAutoSizeColumnsMode.AllCells:
                case DataGridViewAutoSizeColumnsMode.DisplayedCellsExceptHeader:
                case DataGridViewAutoSizeColumnsMode.DisplayedCells:
                case DataGridViewAutoSizeColumnsMode.Fill:
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridViewAutoSizeColumnsMode));
            }

            if (_autoSizeColumnsMode != value)
            {
                foreach (DataGridViewColumn dataGridViewColumn in Columns)
                {
                    if (dataGridViewColumn.AutoSizeMode == DataGridViewAutoSizeColumnMode.NotSet && dataGridViewColumn.Visible)
                    {
                        // Make sure there is no visible column which would have an inherited autosize mode based on the header only.
                        if (value == DataGridViewAutoSizeColumnsMode.ColumnHeader && !ColumnHeadersVisible)
                        {
                            throw new InvalidOperationException(SR.DataGridView_CannotAutoSizeColumnsInvisibleColumnHeaders);
                        }

                        // Make sure there is no visible frozen column which would have a Fill inherited autosize mode.
                        if (value == DataGridViewAutoSizeColumnsMode.Fill && dataGridViewColumn.Frozen)
                        {
                            throw new InvalidOperationException(SR.DataGridView_CannotAutoFillFrozenColumns);
                        }
                    }
                }

                DataGridViewAutoSizeColumnMode[] previousModes = new DataGridViewAutoSizeColumnMode[Columns.Count];
                foreach (DataGridViewColumn dataGridViewColumn in Columns)
                {
                    previousModes[dataGridViewColumn.Index] = dataGridViewColumn.InheritedAutoSizeMode;
                }

                DataGridViewAutoSizeColumnsModeEventArgs dgvcasme = new(previousModes);
                _autoSizeColumnsMode = value;
                OnAutoSizeColumnsModeChanged(dgvcasme);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridViewAutoSizeColumnsModeChangedDescr))]
    public event DataGridViewAutoSizeColumnsModeEventHandler? AutoSizeColumnsModeChanged
    {
        add => Events.AddHandler(s_autosizeColumnsModeChangedEvent, value);
        remove => Events.RemoveHandler(s_autosizeColumnsModeChangedEvent, value);
    }

    /// <summary>
    ///  Gets or sets the rows' autosizing mode.
    /// </summary>
    [DefaultValue(DataGridViewAutoSizeRowsMode.None)]
    [SRCategory(nameof(SR.CatLayout))]
    [SRDescription(nameof(SR.DataGridView_AutoSizeRowsModeDescr))]
    public DataGridViewAutoSizeRowsMode AutoSizeRowsMode
    {
        get => _autoSizeRowsMode;
        set
        {
            switch (value)
            {
                case DataGridViewAutoSizeRowsMode.None:
                case DataGridViewAutoSizeRowsMode.AllHeaders:
                case DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders:
                case DataGridViewAutoSizeRowsMode.AllCells:
                case DataGridViewAutoSizeRowsMode.DisplayedHeaders:
                case DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders:
                case DataGridViewAutoSizeRowsMode.DisplayedCells:
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridViewAutoSizeRowsMode));
            }

            if ((value == DataGridViewAutoSizeRowsMode.AllHeaders || value == DataGridViewAutoSizeRowsMode.DisplayedHeaders)
                && !RowHeadersVisible)
            {
                throw new InvalidOperationException(SR.DataGridView_CannotAutoSizeRowsInvisibleRowHeader);
            }

            if (_autoSizeRowsMode != value)
            {
                DataGridViewAutoSizeModeEventArgs dgvasme = new(_autoSizeRowsMode != DataGridViewAutoSizeRowsMode.None);
                _autoSizeRowsMode = value;
                OnAutoSizeRowsModeChanged(dgvasme);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridViewAutoSizeRowsModeChangedDescr))]
    public event DataGridViewAutoSizeModeEventHandler? AutoSizeRowsModeChanged
    {
        add => Events.AddHandler(s_autosizeRowsModeChangedEvent, value);
        remove => Events.RemoveHandler(s_autosizeRowsModeChangedEvent, value);
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override Color BackColor
    {
        get => base.BackColor;
        set => base.BackColor = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? BackColorChanged
    {
        add => base.BackColorChanged += value;
        remove => base.BackColorChanged -= value;
    }

    /// <summary>
    ///  Gets or sets the background color of the dataGridView.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridViewBackgroundColorDescr))]
    public Color BackgroundColor
    {
        get => _backgroundColor;
        set
        {
            if (value.IsEmpty)
                throw new ArgumentException(string.Format(SR.DataGridView_EmptyColor, "BackgroundColor"));
            if (value.A < 255)
                throw new ArgumentException(string.Format(SR.DataGridView_TransparentColor, "BackgroundColor"));

            if (!value.Equals(_backgroundColor))
            {
                _backgroundColor = value;
                OnBackgroundColorChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridViewBackgroundColorChangedDescr))]
    public event EventHandler? BackgroundColorChanged
    {
        add => Events.AddHandler(s_backgroundColorChangedEvent, value);
        remove => Events.RemoveHandler(s_backgroundColorChangedEvent, value);
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Image? BackgroundImage
    {
        get => base.BackgroundImage;
        set => base.BackgroundImage = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override ImageLayout BackgroundImageLayout
    {
        get => base.BackgroundImageLayout;
        set => base.BackgroundImageLayout = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? BackgroundImageChanged
    {
        add => base.BackgroundImageChanged += value;
        remove => base.BackgroundImageChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? BackgroundImageLayoutChanged
    {
        add => base.BackgroundImageLayoutChanged += value;
        remove => base.BackgroundImageLayoutChanged -= value;
    }

    private bool ShouldSerializeBackgroundColor() => !BackgroundColor.Equals(s_defaultBackgroundColor);

    [DefaultValue(BorderStyle.FixedSingle)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_BorderStyleDescr))]
    public BorderStyle BorderStyle
    {
        get => _borderStyle;
        set
        {
            // Sequential enum.  Valid values are 0x0 to 0x2
            SourceGenerated.EnumValidator.Validate(value);
            if (_borderStyle != value)
            {
                using (LayoutTransaction.CreateTransactionIf(AutoSize, ParentInternal, this, PropertyNames.BorderStyle))
                {
                    _borderStyle = value;
                    if (!AutoSize)
                    {
                        PerformLayoutPrivate(
                            useRowShortcut: false,
                            computeVisibleRows: false,
                            invalidInAdjustFillingColumns: true,
                            repositionEditingControl: true);
                    }

                    Invalidate();
                    OnBorderStyleChanged(EventArgs.Empty);
                }
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridViewBorderStyleChangedDescr))]
    public event EventHandler? BorderStyleChanged
    {
        add => Events.AddHandler(s_borderStyleChangedEvent, value);
        remove => Events.RemoveHandler(s_borderStyleChangedEvent, value);
    }

    internal int BorderWidth
    {
        get
        {
            if (BorderStyle == BorderStyle.Fixed3D)
            {
                return Application.RenderWithVisualStyles ? 1 : SystemInformation.Border3DSize.Width;
            }
            else if (BorderStyle == BorderStyle.FixedSingle)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }

    // Ime can be shown when there is a read-write current cell.
    protected override bool CanEnableIme
    {
        get
        {
            bool canEnable = false;

            if (_ptCurrentCell.X != -1 && ColumnEditable(_ptCurrentCell.X))
            {
                DataGridViewCell dataGridViewCell = CurrentCellInternal;
                Debug.Assert(dataGridViewCell is not null);

                if (!IsSharedCellReadOnly(dataGridViewCell, _ptCurrentCell.Y))
                {
                    canEnable = base.CanEnableIme;
                }
            }

            return canEnable;
        }
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_CellBorderStyleDescr))]
    [Browsable(true)]
    [DefaultValue(DataGridViewCellBorderStyle.Single)]
    public DataGridViewCellBorderStyle CellBorderStyle
    {
        get
        {
            switch (AdvancedCellBorderStyle.All)
            {
                case DataGridViewAdvancedCellBorderStyle.NotSet:
                    if (AdvancedCellBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.None
                        && AdvancedCellBorderStyle.Bottom == DataGridViewAdvancedCellBorderStyle.None)
                    {
                        if (RightToLeftInternal)
                        {
                            if (AdvancedCellBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.None
                                && AdvancedCellBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.Single)
                            {
                                return DataGridViewCellBorderStyle.SingleVertical;
                            }
                        }
                        else
                        {
                            if (AdvancedCellBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.None
                                && AdvancedCellBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.Single)
                            {
                                return DataGridViewCellBorderStyle.SingleVertical;
                            }
                        }

                        if (AdvancedCellBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.Outset
                            && AdvancedCellBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.Outset)
                        {
                            return DataGridViewCellBorderStyle.RaisedVertical;
                        }

                        if (AdvancedCellBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.Inset
                            && AdvancedCellBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.Inset)
                        {
                            return DataGridViewCellBorderStyle.SunkenVertical;
                        }
                    }

                    if (AdvancedCellBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.None
                        && AdvancedCellBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.None)
                    {
                        if (AdvancedCellBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.None
                            && AdvancedCellBorderStyle.Bottom == DataGridViewAdvancedCellBorderStyle.Single)
                        {
                            return DataGridViewCellBorderStyle.SingleHorizontal;
                        }

                        if (AdvancedCellBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.Outset
                            && AdvancedCellBorderStyle.Bottom == DataGridViewAdvancedCellBorderStyle.Outset)
                        {
                            return DataGridViewCellBorderStyle.RaisedHorizontal;
                        }

                        if (AdvancedCellBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.Inset
                            && AdvancedCellBorderStyle.Bottom == DataGridViewAdvancedCellBorderStyle.Inset)
                        {
                            return DataGridViewCellBorderStyle.SunkenHorizontal;
                        }
                    }

                    return DataGridViewCellBorderStyle.Custom;

                case DataGridViewAdvancedCellBorderStyle.None:
                    return DataGridViewCellBorderStyle.None;

                case DataGridViewAdvancedCellBorderStyle.Single:
                    return DataGridViewCellBorderStyle.Single;

                case DataGridViewAdvancedCellBorderStyle.Inset:
                    return DataGridViewCellBorderStyle.Sunken;

                case DataGridViewAdvancedCellBorderStyle.Outset:
                    return DataGridViewCellBorderStyle.Raised;

                default:
                    Debug.Fail("Unexpected this.advancedCellBorderStyle.All value in CellBorderStyle.get");
                    return DataGridViewCellBorderStyle.Custom;
            }
        }
        set
        {
            // Sequential enum.  Valid values are 0x0 to 0xa
            SourceGenerated.EnumValidator.Validate(value);

            if (value != CellBorderStyle)
            {
                if (value == DataGridViewCellBorderStyle.Custom)
                {
                    throw new ArgumentException(string.Format(SR.DataGridView_CustomCellBorderStyleInvalid, "CellBorderStyle"));
                }

                _dataGridViewOper[OperationInBorderStyleChange] = true;
                try
                {
                    switch (value)
                    {
                        case DataGridViewCellBorderStyle.Single:
                            AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Single;
                            break;

                        case DataGridViewCellBorderStyle.Raised:
                            AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Outset;
                            break;

                        case DataGridViewCellBorderStyle.Sunken:
                            AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Inset;
                            break;

                        case DataGridViewCellBorderStyle.None:
                            AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                            break;

                        case DataGridViewCellBorderStyle.SingleVertical:
                            AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                            if (RightToLeftInternal)
                            {
                                AdvancedCellBorderStyle.LeftInternal = DataGridViewAdvancedCellBorderStyle.Single;
                            }
                            else
                            {
                                AdvancedCellBorderStyle.RightInternal = DataGridViewAdvancedCellBorderStyle.Single;
                            }

                            break;

                        case DataGridViewCellBorderStyle.RaisedVertical:
                            AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                            AdvancedCellBorderStyle.RightInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                            AdvancedCellBorderStyle.LeftInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                            break;

                        case DataGridViewCellBorderStyle.SunkenVertical:
                            AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                            AdvancedCellBorderStyle.RightInternal = DataGridViewAdvancedCellBorderStyle.Inset;
                            AdvancedCellBorderStyle.LeftInternal = DataGridViewAdvancedCellBorderStyle.Inset;
                            break;

                        case DataGridViewCellBorderStyle.SingleHorizontal:
                            AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                            AdvancedCellBorderStyle.BottomInternal = DataGridViewAdvancedCellBorderStyle.Single;
                            break;

                        case DataGridViewCellBorderStyle.RaisedHorizontal:
                            AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                            AdvancedCellBorderStyle.TopInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                            AdvancedCellBorderStyle.BottomInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                            break;

                        case DataGridViewCellBorderStyle.SunkenHorizontal:
                            AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                            AdvancedCellBorderStyle.TopInternal = DataGridViewAdvancedCellBorderStyle.Inset;
                            AdvancedCellBorderStyle.BottomInternal = DataGridViewAdvancedCellBorderStyle.Inset;
                            break;
                    }
                }
                finally
                {
                    _dataGridViewOper[OperationInBorderStyleChange] = false;
                }

                OnCellBorderStyleChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridView_CellBorderStyleChangedDescr))]
    public event EventHandler? CellBorderStyleChanged
    {
        add => Events.AddHandler(s_cellBorderStyleChangedEvent, value);
        remove => Events.RemoveHandler(s_cellBorderStyleChangedEvent, value);
    }

    internal bool CellMouseDownInContentBounds
    {
        get => _dataGridViewState2[State2_CellMouseDownInContentBounds];
        set
        {
            _dataGridViewState2[State2_CellMouseDownInContentBounds] = value;
        }
    }

    internal DataGridViewCellPaintingEventArgs CellPaintingEventArgs
    {
        get
        {
            _dgvcpe ??= new DataGridViewCellPaintingEventArgs(this);

            return _dgvcpe;
        }
    }

    private DataGridViewCellStyleChangedEventArgs CellStyleChangedEventArgs
    {
        get
        {
            _dgvcsce ??= new DataGridViewCellStyleChangedEventArgs();

            return _dgvcsce;
        }
    }

    internal DataGridViewCellValueEventArgs CellValueEventArgs
    {
        get
        {
            _dgvcve ??= new DataGridViewCellValueEventArgs();

            return _dgvcve;
        }
    }

    [Browsable(true)]
    [DefaultValue(DataGridViewClipboardCopyMode.EnableWithAutoHeaderText)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DataGridView_ClipboardCopyModeDescr))]
    public DataGridViewClipboardCopyMode ClipboardCopyMode
    {
        get => _clipboardCopyMode;
        set
        {
            // Sequential enum.  Valid values are 0x0 to 0x3
            SourceGenerated.EnumValidator.Validate(value);
            _clipboardCopyMode = value;
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [DefaultValue(0)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public int ColumnCount
    {
        get => Columns.Count;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);

            if (DataSource is not null)
            {
                throw new InvalidOperationException(SR.DataGridView_CannotSetColumnCountOnDataBoundDataGridView);
            }

            if (value != Columns.Count)
            {
                if (value == 0)
                {
                    // Total removal of the columns. This also clears the rows.
                    Columns.Clear();
                }
                else if (value < Columns.Count)
                {
                    // Some columns need to be removed, from the tail of the columns collection
                    while (value < Columns.Count)
                    {
                        int currentColumnCount = Columns.Count;
                        Columns.RemoveAt(currentColumnCount - 1);
                        if (Columns.Count >= currentColumnCount)
                        {
                            // Column removal failed. We stop the loop.
                            break;
                        }
                    }
                }
                else
                {
                    // Some DataGridViewTextBoxColumn columns need to be appended.
                    while (value > Columns.Count)
                    {
                        int currentColumnCount = Columns.Count;
                        Columns.Add(columnName: null, headerText: null);
                        if (Columns.Count <= currentColumnCount)
                        {
                            // Column addition failed. We stop the loop.
                            break;
                        }
                    }
                }
            }
        }
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_ColumnHeadersBorderStyleDescr))]
    [Browsable(true)]
    [DefaultValue(DataGridViewHeaderBorderStyle.Raised)]
    public DataGridViewHeaderBorderStyle ColumnHeadersBorderStyle
    {
        get => AdvancedColumnHeadersBorderStyle.All switch
        {
            DataGridViewAdvancedCellBorderStyle.NotSet => DataGridViewHeaderBorderStyle.Custom,
            DataGridViewAdvancedCellBorderStyle.None => DataGridViewHeaderBorderStyle.None,
            DataGridViewAdvancedCellBorderStyle.Single => DataGridViewHeaderBorderStyle.Single,
            DataGridViewAdvancedCellBorderStyle.InsetDouble => DataGridViewHeaderBorderStyle.Sunken,
            DataGridViewAdvancedCellBorderStyle.OutsetPartial => DataGridViewHeaderBorderStyle.Raised,
            _ => DataGridViewHeaderBorderStyle.Custom,
        };
        set
        {
            // Sequential enum.  Valid values are 0x0 to 0x4
            SourceGenerated.EnumValidator.Validate(value);
            if (value != ColumnHeadersBorderStyle)
            {
                if (value == DataGridViewHeaderBorderStyle.Custom)
                {
                    throw new ArgumentException(string.Format(SR.DataGridView_CustomCellBorderStyleInvalid, "ColumnHeadersBorderStyle"));
                }

                _dataGridViewOper[OperationInBorderStyleChange] = true;
                try
                {
                    switch (value)
                    {
                        case DataGridViewHeaderBorderStyle.Single:
                            AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Single;
                            break;

                        case DataGridViewHeaderBorderStyle.Raised:
                            AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.OutsetPartial;
                            break;

                        case DataGridViewHeaderBorderStyle.Sunken:
                            AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.InsetDouble;
                            break;

                        case DataGridViewHeaderBorderStyle.None:
                            AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                            break;
                    }
                }
                finally
                {
                    _dataGridViewOper[OperationInBorderStyleChange] = false;
                }

                OnColumnHeadersBorderStyleChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridView_ColumnHeadersBorderStyleChangedDescr))]
    public event EventHandler? ColumnHeadersBorderStyleChanged
    {
        add => Events.AddHandler(s_columnHeadersBorderStyleChangedEvent, value);
        remove => Events.RemoveHandler(s_columnHeadersBorderStyleChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_ColumnHeadersDefaultCellStyleDescr))]
    [AmbientValue(null)]
    [AllowNull]
    public DataGridViewCellStyle ColumnHeadersDefaultCellStyle
    {
        get
        {
            _columnHeadersDefaultCellStyle ??= DefaultColumnHeadersDefaultCellStyle;

            return _columnHeadersDefaultCellStyle;
        }
        set
        {
            DataGridViewCellStyle cs = ColumnHeadersDefaultCellStyle;
            cs.RemoveScope(DataGridViewCellStyleScopes.ColumnHeaders);
            _columnHeadersDefaultCellStyle = value;
            _columnHeadersDefaultCellStyle?.AddScope(this, DataGridViewCellStyleScopes.ColumnHeaders);

            // Update ambient font flag depending on cell style font
            _dataGridViewState1[State1_AmbientColumnHeadersFont] = value?.Font == base.Font;

            DataGridViewCellStyleDifferences dgvcsc = cs.GetDifferencesFrom(ColumnHeadersDefaultCellStyle);
            if (dgvcsc != DataGridViewCellStyleDifferences.None)
            {
                CellStyleChangedEventArgs.ChangeAffectsPreferredSize = (dgvcsc == DataGridViewCellStyleDifferences.AffectPreferredSize);
                OnColumnHeadersDefaultCellStyleChanged(CellStyleChangedEventArgs);
            }
        }
    }

    private DataGridViewCellStyle DefaultColumnHeadersDefaultCellStyle
    {
        get
        {
            DataGridViewCellStyle defaultStyle = new DataGridViewCellStyle
            {
                BackColor = DefaultHeadersBackBrush.Color,
                ForeColor = DefaultForeBrush.Color,
                SelectionBackColor = DefaultSelectionBackBrush.Color,
                SelectionForeColor = DefaultSelectionForeBrush.Color,
                Font = base.Font,
                AlignmentInternal = DataGridViewContentAlignment.MiddleLeft,
                WrapModeInternal = DataGridViewTriState.True
            };
            defaultStyle.AddScope(this, DataGridViewCellStyleScopes.ColumnHeaders);

            _dataGridViewState1[State1_AmbientColumnHeadersFont] = true;

            return defaultStyle;
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridViewColumnHeadersDefaultCellStyleChangedDescr))]
    public event EventHandler? ColumnHeadersDefaultCellStyleChanged
    {
        add => Events.AddHandler(s_columnHeadersDefaultCellStyleChangedEvent, value);
        remove => Events.RemoveHandler(s_columnHeadersDefaultCellStyleChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [Localizable(true)]
    [SRDescription(nameof(SR.DataGridView_ColumnHeadersHeightDescr))]
    public int ColumnHeadersHeight
    {
        get => _columnHeadersHeight;
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, MinimumColumnHeadersHeight);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, MaxHeadersThickness);

            if (ColumnHeadersHeightSizeMode == DataGridViewColumnHeadersHeightSizeMode.AutoSize)
            {
                _cachedColumnHeadersHeight = value;
            }
            else if (_columnHeadersHeight != value)
            {
                SetColumnHeadersHeightInternal(value, invalidInAdjustFillingColumns: true);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridViewColumnHeadersHeightChangedDescr))]
    public event EventHandler? ColumnHeadersHeightChanged
    {
        add => Events.AddHandler(s_columnHeadersHeightChangedEvent, value);
        remove => Events.RemoveHandler(s_columnHeadersHeightChangedEvent, value);
    }

    private bool ShouldSerializeColumnHeadersHeight() =>
        ColumnHeadersHeightSizeMode != DataGridViewColumnHeadersHeightSizeMode.AutoSize
        && ColumnHeadersHeight != DefaultColumnHeadersHeight;

    /// <summary>
    ///  Gets or sets a value that determines the behavior for adjusting the column headers height.
    /// </summary>
    [DefaultValue(DataGridViewColumnHeadersHeightSizeMode.EnableResizing)]
    [RefreshProperties(RefreshProperties.All)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DataGridView_ColumnHeadersHeightSizeModeDescr))]
    public DataGridViewColumnHeadersHeightSizeMode ColumnHeadersHeightSizeMode
    {
        get => _columnHeadersHeightSizeMode;
        set
        {
            // Sequential enum.  Valid values are 0x0 to 0x2
            SourceGenerated.EnumValidator.Validate(value);
            if (_columnHeadersHeightSizeMode != value)
            {
                DataGridViewAutoSizeModeEventArgs dgvasme = new(_columnHeadersHeightSizeMode == DataGridViewColumnHeadersHeightSizeMode.AutoSize);
                _columnHeadersHeightSizeMode = value;
                OnColumnHeadersHeightSizeModeChanged(dgvasme);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridView_ColumnHeadersHeightSizeModeChangedDescr))]
    public event DataGridViewAutoSizeModeEventHandler? ColumnHeadersHeightSizeModeChanged
    {
        add => Events.AddHandler(s_columnHeadersHeightSizeModeChangedEvent, value);
        remove => Events.RemoveHandler(s_columnHeadersHeightSizeModeChangedEvent, value);
    }

    /// <summary>
    ///  Gets
    ///  or sets a value indicating if the dataGridView's column headers are visible.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.DataGridViewColumnHeadersVisibleDescr))]
    public bool ColumnHeadersVisible
    {
        get => _dataGridViewState1[State1_ColumnHeadersVisible];
        set
        {
            if (ColumnHeadersVisible != value)
            {
                if (!value)
                {
                    // Make sure that there is no visible column that only counts on the column headers to autosize
                    DataGridViewColumn? dataGridViewColumn = Columns.GetFirstColumn(DataGridViewElementStates.Visible);
                    while (dataGridViewColumn is not null)
                    {
                        if (dataGridViewColumn.InheritedAutoSizeMode == DataGridViewAutoSizeColumnMode.ColumnHeader)
                        {
                            throw new InvalidOperationException(SR.DataGridView_ColumnHeadersCannotBeInvisible);
                        }

                        dataGridViewColumn = Columns.GetNextColumn(dataGridViewColumn,
                            DataGridViewElementStates.Visible,
                            DataGridViewElementStates.None);
                    }
                }

                using (LayoutTransaction.CreateTransactionIf(AutoSize, ParentInternal, this, PropertyNames.ColumnHeadersVisible))
                {
                    _dataGridViewState1[State1_ColumnHeadersVisible] = value;
                    _layout.ColumnHeadersVisible = value;
                    DisplayedBandsInfo.EnsureDirtyState();
                    if (!AutoSize)
                    {
                        PerformLayoutPrivate(
                            useRowShortcut: false,
                            computeVisibleRows: false,
                            invalidInAdjustFillingColumns: true,
                            repositionEditingControl: true);
                    }

                    InvalidateInside();
                    OnColumnHeadersGlobalAutoSize();
                }
            }
        }
    }

    [Editor($"System.Windows.Forms.Design.DataGridViewColumnCollectionEditor, {AssemblyRef.SystemDesign}", typeof(Drawing.Design.UITypeEditor))]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [MergableProperty(false)]
    public DataGridViewColumnCollection Columns
    {
        get
        {
            _dataGridViewColumns ??= CreateColumnsInstance();

            return _dataGridViewColumns;
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DataGridViewCell? CurrentCell
    {
        get
        {
            if (_ptCurrentCell.X == -1 && _ptCurrentCell.Y == -1)
            {
                return null;
            }

            Debug.Assert(_ptCurrentCell.X >= 0 && _ptCurrentCell.Y >= 0);
            Debug.Assert(_ptCurrentCell.X < Columns.Count);
            Debug.Assert(_ptCurrentCell.Y < Rows.Count);
            DataGridViewRow dataGridViewRow = Rows[_ptCurrentCell.Y]; // unsharing row
            return dataGridViewRow.Cells[_ptCurrentCell.X];
        }
        set
        {
            if ((value is not null && (value.RowIndex != _ptCurrentCell.Y || value.ColumnIndex != _ptCurrentCell.X))
                || (value is null && _ptCurrentCell.X != -1))
            {
                if (value is null)
                {
                    ClearSelection();
                    if (!SetCurrentCellAddressCore(
                        columnIndex: -1,
                        rowIndex: -1,
                        setAnchorCellAddress: true,
                        validateCurrentCell: true,
                        throughMouseClick: false))
                    {
                        // Edited value couldn't be committed or aborted
                        throw new InvalidOperationException(SR.DataGridView_CellChangeCannotBeCommittedOrAborted);
                    }
                }
                else
                {
                    if (value.DataGridView != this)
                    {
                        throw new ArgumentException(SR.DataGridView_CellDoesNotBelongToDataGridView);
                    }

                    if (!Columns[value.ColumnIndex].Visible || (Rows.GetRowState(value.RowIndex) & DataGridViewElementStates.Visible) == 0)
                    {
                        throw new InvalidOperationException(SR.DataGridView_CurrentCellCannotBeInvisible);
                    }

                    if (!ScrollIntoView(value.ColumnIndex, value.RowIndex, true))
                    {
                        throw new InvalidOperationException(SR.DataGridView_CellChangeCannotBeCommittedOrAborted);
                    }

                    if (IsInnerCellOutOfBounds(value.ColumnIndex, value.RowIndex))
                    {
                        return;
                    }

                    ClearSelection(value.ColumnIndex, value.RowIndex, selectExceptionElement: true);
                    if (!SetCurrentCellAddressCore(
                        value.ColumnIndex,
                        value.RowIndex,
                        setAnchorCellAddress: true,
                        validateCurrentCell: false,
                        throughMouseClick: false))
                    {
                        throw new InvalidOperationException(SR.DataGridView_CellChangeCannotBeCommittedOrAborted);
                    }
                }
            }
        }
    }

    [Browsable(false)]
    public Point CurrentCellAddress => _ptCurrentCell;

    private DataGridViewCell CurrentCellInternal
    {
        get
        {
            Debug.Assert(_ptCurrentCell.X >= 0 && _ptCurrentCell.X < Columns.Count);
            Debug.Assert(_ptCurrentCell.Y >= 0 && _ptCurrentCell.Y < Rows.Count);
            DataGridViewRow dataGridViewRow = Rows.SharedRow(_ptCurrentCell.Y);
            Debug.Assert(dataGridViewRow is not null);
            DataGridViewCell dataGridViewCell = dataGridViewRow.Cells[_ptCurrentCell.X];
            Debug.Assert(IsSharedCellVisible(dataGridViewCell, _ptCurrentCell.Y));
            return dataGridViewCell;
        }
    }

    private bool CurrentCellIsFirstVisibleCell
    {
        get
        {
            if (_ptCurrentCell.X == -1)
            {
                return false;
            }

            Debug.Assert(_ptCurrentCell.Y != -1);

            bool previousVisibleColumnExists = (Columns.GetPreviousColumn(Columns[_ptCurrentCell.X], DataGridViewElementStates.Visible, DataGridViewElementStates.None) is not null);
            bool previousVisibleRowExists = (Rows.GetPreviousRow(_ptCurrentCell.Y, DataGridViewElementStates.Visible) != -1);

            return !previousVisibleColumnExists && !previousVisibleRowExists;
        }
    }

    private bool CurrentCellIsLastVisibleCell
    {
        get
        {
            if (_ptCurrentCell.X == -1)
            {
                return false;
            }

            Debug.Assert(_ptCurrentCell.Y != -1);

            bool nextVisibleColumnExists = (Columns.GetNextColumn(Columns[_ptCurrentCell.X], DataGridViewElementStates.Visible, DataGridViewElementStates.None) is not null);
            bool nextVisibleRowExists = (Rows.GetNextRow(_ptCurrentCell.Y, DataGridViewElementStates.Visible) != -1);

            return !nextVisibleColumnExists && !nextVisibleRowExists;
        }
    }

    private bool CurrentCellIsEditedAndOnlySelectedCell
    {
        get
        {
            if (_ptCurrentCell.X == -1)
            {
                return false;
            }

            Debug.Assert(_ptCurrentCell.Y != -1);

            return EditingControl is not null
                && GetCellCount(DataGridViewElementStates.Selected) == 1
                && CurrentCellInternal.Selected;
        }
    }

    [Browsable(false)]
    public DataGridViewRow? CurrentRow
    {
        get
        {
            if (_ptCurrentCell.X == -1)
            {
                return null;
            }

            Debug.Assert(_ptCurrentCell.Y >= 0);
            Debug.Assert(_ptCurrentCell.Y < Rows.Count);

            return Rows[_ptCurrentCell.Y];
        }
    }

    [AllowNull]
    internal Cursor CursorInternal
    {
        set
        {
            _dataGridViewState2[State2_IgnoreCursorChange] = true;
            try
            {
                Cursor = value;
            }
            finally
            {
                _dataGridViewState2[State2_IgnoreCursorChange] = false;
            }
        }
    }

    internal DataGridViewDataConnection? DataConnection { get; private set; }

    [DefaultValue("")]
    [SRCategory(nameof(SR.CatData))]
    [Editor($"System.Windows.Forms.Design.DataMemberListEditor, {AssemblyRef.SystemDesign}", typeof(Drawing.Design.UITypeEditor))]
    [SRDescription(nameof(SR.DataGridViewDataMemberDescr))]
    public string DataMember
    {
        get => DataConnection is null ? string.Empty : DataConnection.DataMember;
        set
        {
            if (value != DataMember)
            {
                CurrentCell = null;
                DataConnection ??= new DataGridViewDataConnection(this);

                DataConnection.SetDataConnection(DataSource, value);
                OnDataMemberChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridViewDataMemberChangedDescr))]
    public event EventHandler? DataMemberChanged
    {
        add => Events.AddHandler(s_dataMemberChangedEvent, value);
        remove => Events.RemoveHandler(s_dataMemberChangedEvent, value);
    }

    [DefaultValue(null)]
    [SRCategory(nameof(SR.CatData))]
    [RefreshProperties(RefreshProperties.Repaint)]
    [AttributeProvider(typeof(IListSource))]
    [SRDescription(nameof(SR.DataGridViewDataSourceDescr))]
    public object? DataSource
    {
        get => DataConnection?.DataSource;
        set
        {
            if (value != DataSource)
            {
                if (DataSource is Component oldDataSource)
                {
                    oldDataSource.Disposed -= OnDataSourceDisposed;
                }

                if (value is Component newDataSource)
                {
                    newDataSource.Disposed += OnDataSourceDisposed;
                }

                CurrentCell = null;
                if (DataConnection is null)
                {
                    DataConnection = new DataGridViewDataConnection(this);
                    DataConnection.SetDataConnection(value, DataMember);
                }
                else
                {
                    if (DataConnection.ShouldChangeDataMember(value))
                    {
                        // we fire DataMemberChanged event
                        DataMember = string.Empty;
                    }

                    DataConnection.SetDataConnection(value, DataMember);
                    if (value is null)
                    {
                        DataConnection = null;
                    }
                }

                OnDataSourceChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridViewDataSourceChangedDescr))]
    public event EventHandler? DataSourceChanged
    {
        add => Events.AddHandler(s_dataSourceChangedEvent, value);
        remove => Events.RemoveHandler(s_dataSourceChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_DefaultCellStyleDescr))]
    [AmbientValue(null)]
    [AllowNull]
    public DataGridViewCellStyle DefaultCellStyle
    {
        get
        {
            if (_defaultCellStyle is null)
            {
                _defaultCellStyle = DefaultDefaultCellStyle;
                return _defaultCellStyle;
            }
            else if (_defaultCellStyle.BackColor == Color.Empty
                || _defaultCellStyle.ForeColor == Color.Empty
                || _defaultCellStyle.SelectionBackColor == Color.Empty
                || _defaultCellStyle.SelectionForeColor == Color.Empty
                || _defaultCellStyle.Font is null
                || _defaultCellStyle.Alignment == DataGridViewContentAlignment.NotSet
                || _defaultCellStyle.WrapMode == DataGridViewTriState.NotSet)
            {
                DataGridViewCellStyle defaultCellStyleTmp = new(_defaultCellStyle)
                {
                    Scope = DataGridViewCellStyleScopes.None
                };
                if (_defaultCellStyle.BackColor == Color.Empty)
                {
                    defaultCellStyleTmp.BackColor = s_defaultBackColor;
                }

                if (_defaultCellStyle.ForeColor == Color.Empty)
                {
                    defaultCellStyleTmp.ForeColor = SystemInformation.HighContrast ? DefaultForeBrush.Color : base.ForeColor;
                    _dataGridViewState1[State1_AmbientForeColor] = true;
                }

                if (_defaultCellStyle.SelectionBackColor == Color.Empty)
                {
                    defaultCellStyleTmp.SelectionBackColor = DefaultSelectionBackBrush.Color;
                }

                if (_defaultCellStyle.SelectionForeColor == Color.Empty)
                {
                    defaultCellStyleTmp.SelectionForeColor = DefaultSelectionForeBrush.Color;
                }

                if (_defaultCellStyle.Font is null)
                {
                    defaultCellStyleTmp.Font = base.Font;
                    _dataGridViewState1[State1_AmbientFont] = true;
                }

                if (_defaultCellStyle.Alignment == DataGridViewContentAlignment.NotSet)
                {
                    defaultCellStyleTmp.AlignmentInternal = DataGridViewContentAlignment.MiddleLeft;
                }

                if (_defaultCellStyle.WrapMode == DataGridViewTriState.NotSet)
                {
                    defaultCellStyleTmp.WrapModeInternal = DataGridViewTriState.False;
                }

                defaultCellStyleTmp.AddScope(this, DataGridViewCellStyleScopes.DataGridView);
                return defaultCellStyleTmp;
            }
            else
            {
                return _defaultCellStyle;
            }
        }
        set
        {
            DataGridViewCellStyle cs = DefaultCellStyle;
            cs.RemoveScope(DataGridViewCellStyleScopes.DataGridView);
            _defaultCellStyle = value;
            _defaultCellStyle?.AddScope(this, DataGridViewCellStyleScopes.DataGridView);

            // Update ambient font flag depending on cell style font
            _dataGridViewState1[State1_AmbientFont] = value?.Font == base.Font;

            DataGridViewCellStyleDifferences dgvcsc = cs.GetDifferencesFrom(DefaultCellStyle);
            if (dgvcsc != DataGridViewCellStyleDifferences.None)
            {
                CellStyleChangedEventArgs.ChangeAffectsPreferredSize = (dgvcsc == DataGridViewCellStyleDifferences.AffectPreferredSize);
                OnDefaultCellStyleChanged(CellStyleChangedEventArgs);
            }
        }
    }

    private DataGridViewCellStyle DefaultDefaultCellStyle
    {
        get
        {
            DataGridViewCellStyle defaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = s_defaultBackColor,
                ForeColor = SystemInformation.HighContrast ? DefaultForeBrush.Color : base.ForeColor,
                SelectionBackColor = DefaultSelectionBackBrush.Color,
                SelectionForeColor = DefaultSelectionForeBrush.Color,
                Font = base.Font,
                AlignmentInternal = DataGridViewContentAlignment.MiddleLeft,
                WrapModeInternal = DataGridViewTriState.False
            };
            defaultCellStyle.AddScope(this, DataGridViewCellStyleScopes.DataGridView);

            _dataGridViewState1[State1_AmbientFont] = true;
            _dataGridViewState1[State1_AmbientForeColor] = true;

            return defaultCellStyle;
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridViewDefaultCellStyleChangedDescr))]
    public event EventHandler? DefaultCellStyleChanged
    {
        add => Events.AddHandler(s_defaultCellStyleChangedEvent, value);
        remove => Events.RemoveHandler(s_defaultCellStyleChangedEvent, value);
    }

    private static SolidBrush DefaultForeBrush => (SolidBrush)SystemBrushes.WindowText;

    private static Color DefaultGridColor => Application.ApplicationColors.WindowFrame;

    private static SolidBrush DefaultHeadersBackBrush => (SolidBrush)SystemBrushes.Control;

    private DataGridViewCellStyle DefaultRowHeadersDefaultCellStyle
    {
        get
        {
            DataGridViewCellStyle defaultStyle = new DataGridViewCellStyle
            {
                BackColor = DefaultHeadersBackBrush.Color,
                ForeColor = DefaultForeBrush.Color,
                SelectionBackColor = DefaultSelectionBackBrush.Color,
                SelectionForeColor = DefaultSelectionForeBrush.Color,
                Font = base.Font,
                AlignmentInternal = DataGridViewContentAlignment.MiddleLeft,
                WrapModeInternal = DataGridViewTriState.True
            };
            defaultStyle.AddScope(this, DataGridViewCellStyleScopes.RowHeaders);

            _dataGridViewState1[State1_AmbientRowHeadersFont] = true;

            return defaultStyle;
        }
    }

    private static SolidBrush DefaultSelectionBackBrush => (SolidBrush)SystemBrushes.Highlight;

    private static SolidBrush DefaultSelectionForeBrush => (SolidBrush)SystemBrushes.HighlightText;

    protected override Size DefaultSize => new(240, 150);

    internal DisplayedBandsData DisplayedBandsInfo { get; }

    /// <summary>
    ///  Returns the client rect of the display area of the control.
    ///  The DataGridView control return its client rectangle minus the potential scrollbars.
    /// </summary>
    public override Rectangle DisplayRectangle
    {
        get
        {
            Rectangle rectDisplay = ClientRectangle;
            if (_horizScrollBar is not null && _horizScrollBar.Visible)
            {
                rectDisplay.Height -= _horizScrollBar.Height;
            }

            if (_vertScrollBar is not null && _vertScrollBar.Visible)
            {
                rectDisplay.Width -= _vertScrollBar.Width;
                if (RightToLeftInternal)
                {
                    rectDisplay.X = _vertScrollBar.Width;
                }
            }

            return rectDisplay;
        }
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(DataGridViewEditMode.EditOnKeystrokeOrF2)]
    [SRDescription(nameof(SR.DataGridView_EditModeDescr))]
    public DataGridViewEditMode EditMode
    {
        get => _editMode;
        set
        {
            // Sequential enum.  Valid values are 0x0 to 0x4
            SourceGenerated.EnumValidator.Validate(value);

            if (_editMode != value)
            {
                _editMode = value;
                OnEditModeChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridView_EditModeChangedDescr))]
    public event EventHandler? EditModeChanged
    {
        add => Events.AddHandler(s_editModeChangedEvent, value);
        remove => Events.RemoveHandler(s_editModeChangedEvent, value);
    }

    internal Point MouseEnteredCellAddress => _ptMouseEnteredCell;

    private bool MouseOverEditingControl
    {
        get
        {
            if (EditingControl is not null)
            {
                Point ptMouse = PointToClient(MousePosition);
                return EditingControl.Bounds.Contains(ptMouse);
            }

            return false;
        }
    }

    private bool MouseOverEditingPanel
    {
        get
        {
            if (_editingPanel is not null)
            {
                Point ptMouse = PointToClient(MousePosition);
                return _editingPanel.Bounds.Contains(ptMouse);
            }

            return false;
        }
    }

    private bool MouseOverScrollBar
    {
        get
        {
            Point ptMouse = PointToClient(MousePosition);
            if (_vertScrollBar is not null && _vertScrollBar.Visible)
            {
                if (_vertScrollBar.Bounds.Contains(ptMouse))
                {
                    return true;
                }
            }

            if (_horizScrollBar is not null && _horizScrollBar.Visible)
            {
                return _horizScrollBar.Bounds.Contains(ptMouse);
            }

            return false;
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public Control? EditingControl { get; private set; }

    internal AccessibleObject? EditingControlAccessibleObject => EditingControl?.AccessibilityObject;

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public Panel EditingPanel
    {
        get
        {
            _editingPanel ??= new DataGridViewEditingPanel(this)
            {
                AccessibleName = SR.DataGridView_AccEditingPanelAccName
            };

            return _editingPanel;
        }
    }

    internal DataGridViewEditingPanelAccessibleObject EditingPanelAccessibleObject
    {
        get
        {
            _editingPanelAccessibleObject ??= new DataGridViewEditingPanelAccessibleObject(this, EditingPanel);

            return _editingPanelAccessibleObject;
        }
    }

    /// <summary>
    ///  Determines whether the DataGridView's header cells render using XP theming visual styles or not
    ///  when visual styles are enabled in the application.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.DataGridView_EnableHeadersVisualStylesDescr))]
    public bool EnableHeadersVisualStyles
    {
        get => _dataGridViewState2[State2_EnableHeadersVisualStyles];
        set
        {
            if (_dataGridViewState2[State2_EnableHeadersVisualStyles] != value)
            {
                _dataGridViewState2[State2_EnableHeadersVisualStyles] = value;
                // Some autosizing may have to be applied since the margins are potentially changed.
                OnGlobalAutoSize(); // Put this into OnEnableHeadersVisualStylesChanged if created.
            }
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DataGridViewCell? FirstDisplayedCell
    {
        get
        {
            Point firstDisplayedCellAddress = FirstDisplayedCellAddress;
            if (firstDisplayedCellAddress.X >= 0)
            {
                return Rows[firstDisplayedCellAddress.Y].Cells[firstDisplayedCellAddress.X]; // unshares the row of first displayed cell
            }

            return null;
        }
        set
        {
            if (value is not null)
            {
                DataGridViewCell firstDisplayedCell = value;
                if (firstDisplayedCell.DataGridView != this)
                {
                    throw new ArgumentException(SR.DataGridView_CellDoesNotBelongToDataGridView);
                }

                if (firstDisplayedCell.RowIndex == -1 || firstDisplayedCell.ColumnIndex == -1)
                {
                    throw new InvalidOperationException(SR.DataGridView_FirstDisplayedCellCannotBeAHeaderOrSharedCell);
                }

                Debug.Assert(firstDisplayedCell.RowIndex >= 0
                    && firstDisplayedCell.RowIndex < Rows.Count
                    && firstDisplayedCell.ColumnIndex >= 0
                    && firstDisplayedCell.ColumnIndex < Columns.Count);

                if (!firstDisplayedCell.Visible)
                {
                    throw new InvalidOperationException(SR.DataGridView_FirstDisplayedCellCannotBeInvisible);
                }

                if (!firstDisplayedCell.Frozen)
                {
                    if (!Rows[firstDisplayedCell.RowIndex].Frozen)
                    {
                        FirstDisplayedScrollingRowIndex = firstDisplayedCell.RowIndex;
                    }

                    if (!Columns[firstDisplayedCell.ColumnIndex].Frozen)
                    {
                        FirstDisplayedScrollingColumnIndex = firstDisplayedCell.ColumnIndex;
                    }
                }
            }
        }
    }

    private Point FirstDisplayedCellAddress
    {
        get
        {
            Point ptFirstDisplayedCellAddress = new(-1, -1)
            {
                Y = Rows.GetFirstRow(DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen)
            };
            if (ptFirstDisplayedCellAddress.Y == -1)
            {
                Debug.Assert(DisplayedBandsInfo.NumTotallyDisplayedFrozenRows == 0);
                if (DisplayedBandsInfo.FirstDisplayedScrollingRow >= 0)
                {
                    ptFirstDisplayedCellAddress.Y = DisplayedBandsInfo.FirstDisplayedScrollingRow;
                }
#if DEBUG
                else
                {
                    Debug.Assert(DisplayedBandsInfo.FirstDisplayedScrollingRow == -1);
                    Debug.Assert(DisplayedBandsInfo.NumDisplayedScrollingRows == 0);
                    Debug.Assert(DisplayedBandsInfo.NumTotallyDisplayedScrollingRows == 0);
                }
#endif
            }

            if (ptFirstDisplayedCellAddress.Y >= 0)
            {
                ptFirstDisplayedCellAddress.X = FirstDisplayedColumnIndex;
            }

            return ptFirstDisplayedCellAddress;
        }
    }

    internal int FirstDisplayedColumnIndex
    {
        get
        {
            if (!IsHandleCreated)
            {
                return -1;
            }

            int firstDisplayedColumnIndex = -1;
            DataGridViewColumn? dataGridViewColumn = Columns.GetFirstColumn(DataGridViewElementStates.Visible);
            if (dataGridViewColumn is not null)
            {
                if (dataGridViewColumn.Frozen)
                {
                    firstDisplayedColumnIndex = dataGridViewColumn.Index;
                }
                else if (DisplayedBandsInfo.FirstDisplayedScrollingCol >= 0)
                {
                    firstDisplayedColumnIndex = DisplayedBandsInfo.FirstDisplayedScrollingCol;
                }
            }
#if DEBUG
            DataGridViewColumn? dataGridViewColumnDbg1 = Columns.GetFirstColumn(DataGridViewElementStates.Displayed);
            int firstDisplayedColumnIndexDbg1 = (dataGridViewColumnDbg1 is null) ? -1 : dataGridViewColumnDbg1.Index;

            int firstDisplayedColumnIndexDbg2 = -1;
            DataGridViewColumn? dataGridViewColumnDbg = Columns.GetFirstColumn(DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen);
            if (dataGridViewColumnDbg is not null)
            {
                firstDisplayedColumnIndexDbg2 = dataGridViewColumnDbg.Index;
            }
            else if (DisplayedBandsInfo.FirstDisplayedScrollingCol >= 0)
            {
                firstDisplayedColumnIndexDbg2 = DisplayedBandsInfo.FirstDisplayedScrollingCol;
            }
            else
            {
                Debug.Assert(DisplayedBandsInfo.LastTotallyDisplayedScrollingCol == -1);
            }

            Debug.Assert(firstDisplayedColumnIndex == firstDisplayedColumnIndexDbg1 || !Visible || DisplayedBandsInfo.Dirty);
            Debug.Assert(firstDisplayedColumnIndex == firstDisplayedColumnIndexDbg2 || DisplayedBandsInfo.Dirty);
#endif
            return firstDisplayedColumnIndex;
        }
    }

    internal int FirstDisplayedRowIndex
    {
        get
        {
            if (!IsHandleCreated)
            {
                return -1;
            }

            int firstDisplayedRowIndex = Rows.GetFirstRow(DataGridViewElementStates.Visible);
            if (firstDisplayedRowIndex != -1)
            {
                if ((Rows.GetRowState(firstDisplayedRowIndex) & DataGridViewElementStates.Frozen) == 0
                    && DisplayedBandsInfo.FirstDisplayedScrollingRow >= 0)
                {
                    firstDisplayedRowIndex = DisplayedBandsInfo.FirstDisplayedScrollingRow;
                }
            }

            return firstDisplayedRowIndex;
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int FirstDisplayedScrollingColumnHiddenWidth { get; private set; }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int FirstDisplayedScrollingColumnIndex
    {
        get => DisplayedBandsInfo.FirstDisplayedScrollingCol;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value, Columns.Count);

            if (!Columns[value].Visible)
            {
                throw new InvalidOperationException(SR.DataGridView_FirstDisplayedScrollingColumnCannotBeInvisible);
            }

            if (Columns[value].Frozen)
            {
                throw new InvalidOperationException(SR.DataGridView_FirstDisplayedScrollingColumnCannotBeFrozen);
            }

            if (!IsHandleCreated)
            {
                CreateHandle();
            }

            int displayWidth = _layout.Data.Width;
            if (displayWidth <= 0)
            {
                throw new InvalidOperationException(SR.DataGridView_NoRoomForDisplayedColumns);
            }

            int totalVisibleFrozenWidth = Columns.GetColumnsWidth(DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen);
            if (totalVisibleFrozenWidth >= displayWidth)
            {
                Debug.Assert(totalVisibleFrozenWidth > 0);
                throw new InvalidOperationException(SR.DataGridView_FrozenColumnsPreventFirstDisplayedScrollingColumn);
            }

            if (value == DisplayedBandsInfo.FirstDisplayedScrollingCol)
            {
                return;
            }

            if (_ptCurrentCell.X >= 0
                && !CommitEdit(
                    DataGridViewDataErrorContexts.Parsing | DataGridViewDataErrorContexts.Commit | DataGridViewDataErrorContexts.Scroll,
                    forCurrentCellChange: false,
                    forCurrentRowChange: false))
            {
                // Could not commit edited cell value - return silently
                // Microsoft: should we throw an error here?
                return;
            }

            if (IsColumnOutOfBounds(value))
            {
                return;
            }

            bool success = ScrollColumnIntoView(
                columnIndex: value,
                rowIndex: -1,
                committed: true,
                forCurrentCellChange: false);
            Debug.Assert(success);

            Debug.Assert(DisplayedBandsInfo.FirstDisplayedScrollingCol >= 0);
            Debug.Assert(DisplayedBandsInfo.FirstDisplayedScrollingCol == value
                || Columns.DisplayInOrder(DisplayedBandsInfo.FirstDisplayedScrollingCol, value));

            int maxHorizontalOffset = Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - displayWidth;

            while (DisplayedBandsInfo.FirstDisplayedScrollingCol != value
                && HorizontalOffset < maxHorizontalOffset)
            {
                ScrollColumns(1);
            }
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int FirstDisplayedScrollingRowIndex
    {
        get => DisplayedBandsInfo.FirstDisplayedScrollingRow;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value, Rows.Count);

            if ((Rows.GetRowState(value) & DataGridViewElementStates.Visible) == 0)
            {
                throw new InvalidOperationException(SR.DataGridView_FirstDisplayedScrollingRowCannotBeInvisible);
            }

            if ((Rows.GetRowState(value) & DataGridViewElementStates.Frozen) != 0)
            {
                throw new InvalidOperationException(SR.DataGridView_FirstDisplayedScrollingRowCannotBeFrozen);
            }

            if (!IsHandleCreated)
            {
                CreateHandle();
            }

            int displayHeight = _layout.Data.Height;
            if (displayHeight <= 0)
            {
                throw new InvalidOperationException(SR.DataGridView_NoRoomForDisplayedRows);
            }

            int totalVisibleFrozenHeight = Rows.GetRowsHeight(DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen);
            if (totalVisibleFrozenHeight >= displayHeight)
            {
                Debug.Assert(totalVisibleFrozenHeight > 0);
                throw new InvalidOperationException(SR.DataGridView_FrozenRowsPreventFirstDisplayedScrollingRow);
            }

            if (value == DisplayedBandsInfo.FirstDisplayedScrollingRow)
            {
                return;
            }

            if (_ptCurrentCell.X >= 0
                && !CommitEdit(
                    DataGridViewDataErrorContexts.Parsing | DataGridViewDataErrorContexts.Commit | DataGridViewDataErrorContexts.Scroll,
                    forCurrentCellChange: false,
                    forCurrentRowChange: false))
            {
                // Could not commit edited cell value - return silently
                // Microsoft: should we throw an error here?
                return;
            }

            if (IsRowOutOfBounds(value))
            {
                return;
            }

            Debug.Assert(DisplayedBandsInfo.FirstDisplayedScrollingRow >= 0);

            if (value > DisplayedBandsInfo.FirstDisplayedScrollingRow)
            {
                int rowsToScroll = Rows.GetRowCount(DataGridViewElementStates.Visible, DisplayedBandsInfo.FirstDisplayedScrollingRow, value);
                Debug.Assert(rowsToScroll != 0);
                ScrollRowsByCount(rowsToScroll, rowsToScroll > 1 ? ScrollEventType.LargeIncrement : ScrollEventType.SmallIncrement);
            }
            else
            {
                bool success = ScrollRowIntoView(
                    columnIndex: -1,
                    rowIndex: value,
                    committed: true,
                    forCurrentCellChange: false);
                Debug.Assert(success);
            }
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override Color ForeColor
    {
        get => base.ForeColor;
        set => base.ForeColor = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new event EventHandler? ForeColorChanged
    {
        add => base.ForeColorChanged += value;
        remove => base.ForeColorChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [AllowNull]
    public override Font Font
    {
        get => base.Font;
        set => base.Font = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new event EventHandler? FontChanged
    {
        add => base.FontChanged += value;
        remove => base.FontChanged -= value;
    }

    /// <summary>
    ///  Gets or sets the grid color of the dataGridView (when Single mode is used).
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridViewGridColorDescr))]
    public Color GridColor
    {
        get => GridPenColor;
        set
        {
            if (value.IsEmpty)
                throw new ArgumentException(string.Format(SR.DataGridView_EmptyColor, nameof(GridColor)));
            if (value.A < 255)
                throw new ArgumentException(string.Format(SR.DataGridView_TransparentColor, nameof(GridColor)));

            if (!value.Equals(GridPenColor))
            {
                GridPenColor = value;
                OnGridColorChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridViewOnGridColorChangedDescr))]
    public event EventHandler? GridColorChanged
    {
        add => Events.AddHandler(s_gridColorChangedEvent, value);
        remove => Events.RemoveHandler(s_gridColorChangedEvent, value);
    }

    private bool ShouldSerializeGridColor() => !GridPenColor.Equals(DefaultGridColor);

    internal Color GridPenColor { get; private set; }

    internal int HorizontalOffset
    {
        get => _horizontalOffset;
        set
        {
            if (value < 0)
            {
                value = 0;
            }

            int widthNotVisible = Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - _layout.Data.Width;
            if (value > widthNotVisible && widthNotVisible > 0)
            {
                value = widthNotVisible;
            }

            if (value == _horizontalOffset)
            {
                return;
            }

            ScrollEventType scrollEventType;
            int oldFirstVisibleScrollingCol = DisplayedBandsInfo.FirstDisplayedScrollingCol;
            int change = _horizontalOffset - value;
            if (_horizScrollBar.Enabled)
            {
                _horizScrollBar.Value = value;
            }

            _horizontalOffset = value;

            int totalVisibleFrozenWidth = Columns.GetColumnsWidth(DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen);

            Rectangle rectTmp = _layout.Data;
            if (_layout.ColumnHeadersVisible)
            {
                // Column headers must scroll as well
                rectTmp = Rectangle.Union(rectTmp, _layout.ColumnHeaders);
            }
            else if (SingleVerticalBorderAdded)
            {
                if (!RightToLeftInternal)
                {
                    rectTmp.X--;
                }

                rectTmp.Width++;
            }

            if (SingleVerticalBorderAdded && totalVisibleFrozenWidth > 0)
            {
                if (!RightToLeftInternal)
                {
                    rectTmp.X++;
                }

                rectTmp.Width--;
            }

            if (!RightToLeftInternal)
            {
                rectTmp.X += totalVisibleFrozenWidth;
            }

            rectTmp.Width -= totalVisibleFrozenWidth;

            DisplayedBandsInfo.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();

            // Update the lastTotallyDisplayedScrollingCol
            ComputeVisibleColumns();

            if (EditingControl is not null
                && !Columns[_ptCurrentCell.X].Frozen
                && DisplayedBandsInfo.FirstDisplayedScrollingCol > -1)
            {
                PositionEditingControl(setLocation: true, setSize: false, setFocus: false);
            }

            // The mouse probably is not over the same cell after the scroll.
            UpdateMouseEnteredCell(hti: null, e: null);

            if (oldFirstVisibleScrollingCol == DisplayedBandsInfo.FirstDisplayedScrollingCol)
            {
                scrollEventType = change > 0 ? ScrollEventType.SmallIncrement : ScrollEventType.SmallDecrement;
            }
            else if (Columns.DisplayInOrder(oldFirstVisibleScrollingCol, DisplayedBandsInfo.FirstDisplayedScrollingCol))
            {
                scrollEventType = Columns.GetColumnCount(DataGridViewElementStates.Visible, oldFirstVisibleScrollingCol, DisplayedBandsInfo.FirstDisplayedScrollingCol) > 1 ? ScrollEventType.LargeIncrement : ScrollEventType.SmallIncrement;
            }
            else
            {
                Debug.Assert(Columns.DisplayInOrder(DisplayedBandsInfo.FirstDisplayedScrollingCol, oldFirstVisibleScrollingCol));
                scrollEventType = Columns.GetColumnCount(DataGridViewElementStates.Visible, DisplayedBandsInfo.FirstDisplayedScrollingCol, oldFirstVisibleScrollingCol) > 1 ? ScrollEventType.LargeDecrement : ScrollEventType.SmallDecrement;
            }

            RECT[]? rects = CreateScrollableRegion(rectTmp);
            if (RightToLeftInternal)
            {
                change = -change;
            }

            ScrollRectangles(rects, change);
            if (!_dataGridViewState2[State2_StopRaisingHorizontalScroll])
            {
                OnScroll(scrollEventType, _horizontalOffset + change, _horizontalOffset, ScrollOrientation.HorizontalScroll);
            }

            FlushDisplayedChanged();
        }
    }

    protected ScrollBar HorizontalScrollBar => _horizScrollBar;

    internal int HorizontalScrollBarHeight => _horizScrollBar.Height;

    internal bool HorizontalScrollBarVisible => _horizScrollBar.Visible;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int HorizontalScrollingOffset
    {
        get => _horizontalOffset;
        set
        {
            // int widthNotVisible = this.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - this.layout.Data.Width;
            ArgumentOutOfRangeException.ThrowIfNegative(value);

            // Intentionally ignoring the out of range situation.
            // if (value > widthNotVisible && widthNotVisible > 0)
            // {
            //     throw new ArgumentOutOfRangeException(string.Format(SR.DataGridView_PropertyTooLarge, "HorizontalScrollingOffset", (widthNotVisible).ToString()));
            // }
            if (value > 0 && (Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - _layout.Data.Width) <= 0)
            {
                // Intentionally ignoring the case where dev tries to set value while there is no horizontal scrolling possible.
                // throw new ArgumentOutOfRangeException(nameof(HorizontalScrollingOffset), SR.DataGridView_PropertyMustBeZero);
                Debug.Assert(_horizontalOffset == 0);
                return;
            }

            if (value == _horizontalOffset)
            {
                return;
            }

            HorizontalOffset = value;
        }
    }

    private Timer HorizScrollTimer
    {
        get
        {
            if (_horizScrollTimer is null)
            {
                _horizScrollTimer = new Timer();
                _horizScrollTimer.Tick += new EventHandler(HorizScrollTimer_Tick);
            }

            return _horizScrollTimer;
        }
    }

    private bool InAdjustFillingColumns
        => _dataGridViewOper[OperationInAdjustFillingColumn] || _dataGridViewOper[OperationInAdjustFillingColumns];

    internal bool InBeginEdit => _dataGridViewOper[OperationInBeginEdit];

    internal bool InDisplayIndexAdjustments
    {
        get => _dataGridViewOper[OperationInDisplayIndexAdjustments];
        set => _dataGridViewOper[OperationInDisplayIndexAdjustments] = value;
    }

    internal bool InEndEdit => _dataGridViewOper[OperationInEndEdit];

    private DataGridViewCellStyle? InheritedEditingCellStyle
        => _ptCurrentCell.X == -1 ? null : CurrentCellInternal.GetInheritedStyleInternal(_ptCurrentCell.Y);

    internal bool InInitialization => _dataGridViewState2[State2_Initializing];

    internal bool InSortOperation => _dataGridViewOper[OperationInSort];

    [Browsable(false)]
    public bool IsCurrentCellDirty => _dataGridViewState1[State1_EditedCellChanged];

    private bool IsCurrentCellDirtyInternal
    {
        set
        {
            if (value != _dataGridViewState1[State1_EditedCellChanged])
            {
                _dataGridViewState1[State1_EditedCellChanged] = value;
                OnCurrentCellDirtyStateChanged(EventArgs.Empty);
            }
        }
    }

    [Browsable(false)]
    public bool IsCurrentCellInEditMode
        => EditingControl is not null || _dataGridViewState1[State1_CurrentCellInEditMode];

    // Only used in bound scenarios, when binding to a IEditableObject
    [Browsable(false)]
    public bool IsCurrentRowDirty
    {
        get
        {
            if (!VirtualMode)
            {
                return _dataGridViewState1[State1_EditedRowChanged] || IsCurrentCellDirty;
            }
            else
            {
                QuestionEventArgs qe = new(_dataGridViewState1[State1_EditedRowChanged] || IsCurrentCellDirty);
                OnRowDirtyStateNeeded(qe);
                return qe.Response;
            }
        }
    }

    internal bool IsCurrentRowDirtyInternal
    {
        set
        {
            if (value != _dataGridViewState1[State1_EditedRowChanged])
            {
                _dataGridViewState1[State1_EditedRowChanged] = value;
                if (RowHeadersVisible && ShowEditingIcon && _ptCurrentCell.Y >= 0)
                {
                    // Force the pencil to appear in the row header
                    InvalidateCellPrivate(-1, _ptCurrentCell.Y);
                }
            }
        }
    }

    private bool IsEscapeKeyEffective => _dataGridViewOper[OperationTrackColResize]
        || _dataGridViewOper[OperationTrackRowResize]
        || _dataGridViewOper[OperationTrackColHeadersResize]
        || _dataGridViewOper[OperationTrackRowHeadersResize]
        || _dataGridViewOper[OperationTrackColRelocation]
        || IsCurrentCellDirty
        || ((VirtualMode || DataSource is not null) && IsCurrentRowDirty)
        || (EditMode != DataGridViewEditMode.EditOnEnter && EditingControl is not null)
        || _dataGridViewState1[State1_NewRowEdited];

    private bool IsMinimized
        => TopLevelControlInternal is Form parentForm && parentForm.WindowState == FormWindowState.Minimized;

    private bool IsSharedCellReadOnly(DataGridViewCell dataGridViewCell, int rowIndex)
    {
        Debug.Assert(dataGridViewCell is not null);
        Debug.Assert(rowIndex >= 0);
        DataGridViewElementStates rowState = Rows.GetRowState(rowIndex);
        return ReadOnly
            || (rowState & DataGridViewElementStates.ReadOnly) != 0
            || (dataGridViewCell.OwningColumn is not null && dataGridViewCell.OwningColumn.ReadOnly)
            || dataGridViewCell.StateIncludes(DataGridViewElementStates.ReadOnly);
    }

    internal bool IsSharedCellSelected(DataGridViewCell dataGridViewCell, int rowIndex)
    {
        Debug.Assert(dataGridViewCell is not null);
        Debug.Assert(rowIndex >= 0);
        DataGridViewElementStates rowState = Rows.GetRowState(rowIndex);
        return (rowState & DataGridViewElementStates.Selected) != 0
            || (dataGridViewCell.OwningColumn is not null && dataGridViewCell.OwningColumn.Selected)
            || dataGridViewCell.StateIncludes(DataGridViewElementStates.Selected);
    }

    internal bool IsSharedCellVisible(DataGridViewCell dataGridViewCell, int rowIndex)
    {
        Debug.Assert(dataGridViewCell is not null);
        Debug.Assert(rowIndex >= 0);
        DataGridViewElementStates rowState = Rows.GetRowState(rowIndex);
        return (rowState & DataGridViewElementStates.Visible) != 0
            && dataGridViewCell.OwningColumn is not null
            && dataGridViewCell.OwningColumn.Visible;
    }

    internal ToolTip KeyboardToolTip
    {
        get
        {
            if (Properties.TryGetObject(s_propToolTip, out ToolTip? toolTip))
            {
                return toolTip!;
            }

            toolTip = new ToolTip
            {
                ReshowDelay = 500,
                InitialDelay = 500
            };

            Properties.SetObject(s_propToolTip, toolTip);

            return toolTip;
        }
    }

    internal LayoutData LayoutInfo
    {
        get
        {
            if (_layout._dirty && IsHandleCreated)
            {
                PerformLayoutPrivate(
                    useRowShortcut: false,
                    computeVisibleRows: true,
                    invalidInAdjustFillingColumns: false,
                    repositionEditingControl: false);
            }

            return _layout;
        }
    }

    internal Point MouseDownCellAddress => _ptMouseDownCell;

    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.DataGridView_MultiSelectDescr))]
    public bool MultiSelect
    {
        get => _dataGridViewState1[State1_MultiSelect];
        set
        {
            if (MultiSelect != value)
            {
                ClearSelection();
                _dataGridViewState1[State1_MultiSelect] = value;
                OnMultiSelectChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridViewOnMultiSelectChangedDescr))]
    public event EventHandler? MultiSelectChanged
    {
        add => Events.AddHandler(s_multiselectChangedEvent, value);
        remove => Events.RemoveHandler(s_multiselectChangedEvent, value);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int NewRowIndex { get; private set; } = -1;

    internal bool NoDimensionChangeAllowed => _noDimensionChangeCount > 0;

    private int NoSelectionChangeCount
    {
        get => _noSelectionChangeCount;
        set
        {
            Debug.Assert(value >= 0);
            _noSelectionChangeCount = value;
            if (value == 0)
            {
                FlushSelectionChanged();
            }
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new Padding Padding
    {
        get => base.Padding;
        set => base.Padding = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new event EventHandler? PaddingChanged
    {
        add => base.PaddingChanged += value;
        remove => base.PaddingChanged -= value;
    }

    internal DataGridViewCellStyle PlaceholderCellStyle => _placeholderCellStyle ??= new DataGridViewCellStyle();

    [Browsable(true)]
    [DefaultValue(false)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DataGridView_ReadOnlyDescr))]
    public bool ReadOnly
    {
        get => _dataGridViewState1[State1_ReadOnly];
        set
        {
            if (value == _dataGridViewState1[State1_ReadOnly])
            {
                return;
            }

            if (value && _ptCurrentCell.X != -1 && IsCurrentCellInEditMode)
            {
                // Current cell becomes read-only. Exit editing mode.
                if (!EndEdit(
                    DataGridViewDataErrorContexts.Parsing | DataGridViewDataErrorContexts.Commit,
                    DataGridViewValidateCellInternal.Always,
                    fireCellLeave: false,
                    fireCellEnter: false,
                    fireRowLeave: false,
                    fireRowEnter: false,
                    fireLeave: false,
                    keepFocus: true,
                    resetCurrentCell: false,
                    resetAnchorCell: false))
                {
                    throw new InvalidOperationException(SR.DataGridView_CommitFailedCannotCompleteOperation);
                }
            }

            _dataGridViewState1[State1_ReadOnly] = value;

            if (value)
            {
                try
                {
                    _dataGridViewOper[OperationInReadOnlyChange] = true;
                    for (int columnIndex = 0; columnIndex < Columns.Count; columnIndex++)
                    {
                        SetReadOnlyColumnCore(columnIndex, false);
                    }

                    int rowCount = Rows.Count;
                    for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                    {
                        SetReadOnlyRowCore(rowIndex, false);
                    }
                }
                finally
                {
                    _dataGridViewOper[OperationInReadOnlyChange] = false;
                }
            }
#if DEBUG
            else
            {
                Debug.Assert(_individualReadOnlyCells.Count == 0);
                for (int columnIndex = 0; columnIndex < Columns.Count; columnIndex++)
                {
                    Debug.Assert(!Columns[columnIndex].ReadOnly);
                }

                int rowCount = Rows.Count;
                for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                {
                    Debug.Assert((Rows.GetRowState(rowIndex) & DataGridViewElementStates.ReadOnly) == 0);
                }
            }
#endif
            OnReadOnlyChanged(EventArgs.Empty);
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridViewOnReadOnlyChangedDescr))]
    public event EventHandler? ReadOnlyChanged
    {
        add => Events.AddHandler(s_readOnlyChangedEvent, value);
        remove => Events.RemoveHandler(s_readOnlyChangedEvent, value);
    }

    private void ResetCurrentCell()
    {
        if (_ptCurrentCell.X != -1 && !SetCurrentCellAddressCore(
            -1,
            -1,
            setAnchorCellAddress: true,
            validateCurrentCell: true,
            throughMouseClick: false))
        {
            // Edited value couldn't be committed or aborted
            throw new InvalidOperationException(SR.DataGridView_CellChangeCannotBeCommittedOrAborted);
        }
    }

    internal bool ResizingOperationAboutToStart => _dataGridViewOper[OperationResizingOperationAboutToStart];

    internal bool RightToLeftInternal
    {
        get
        {
            if (_dataGridViewState2[State2_RightToLeftValid])
            {
                return _dataGridViewState2[State2_RightToLeftMode];
            }

            _dataGridViewState2[State2_RightToLeftMode] = (RightToLeft == RightToLeft.Yes);
            _dataGridViewState2[State2_RightToLeftValid] = true;
            return _dataGridViewState2[State2_RightToLeftMode];
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DefaultValue(0)]
    public int RowCount
    {
        get => Rows.Count;
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, AllowUserToAddRowsInternal ? 1 : 0);

            if (DataSource is not null)
            {
                throw new InvalidOperationException(SR.DataGridView_CannotSetRowCountOnDataBoundDataGridView);
            }

            if (value == Rows.Count)
            {
                return;
            }

            if (value == 0)
            {
                // Total removal of the rows.
                Rows.Clear();
            }
            else if (value < Rows.Count)
            {
                // Some rows need to be removed, from the tail of the rows collection
                while (value < Rows.Count)
                {
                    int currentRowCount = Rows.Count;
                    Rows.RemoveAt(currentRowCount - (AllowUserToAddRowsInternal ? 2 : 1));
                    if (Rows.Count >= currentRowCount)
                    {
                        // Row removal failed. We stop the loop.
                        break;
                    }
                }
            }
            else
            {
                // Some rows need to be appended.
                if (Columns.Count == 0)
                {
                    // There are no columns yet, we simply create a single DataGridViewTextBoxColumn.
                    DataGridViewTextBoxColumn dataGridViewTextBoxColumn = new();
                    Columns.Add(dataGridViewTextBoxColumn);
                }

                int rowsToAdd = value - Rows.Count;
                if (rowsToAdd > 0)
                {
                    Rows.Add(rowsToAdd);
                }
            }
        }
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_RowHeadersBorderStyleDescr))]
    [Browsable(true)]
    [DefaultValue(DataGridViewHeaderBorderStyle.Raised)]
    public DataGridViewHeaderBorderStyle RowHeadersBorderStyle
    {
        get => AdvancedRowHeadersBorderStyle.All switch
        {
            DataGridViewAdvancedCellBorderStyle.NotSet => DataGridViewHeaderBorderStyle.Custom,
            DataGridViewAdvancedCellBorderStyle.None => DataGridViewHeaderBorderStyle.None,
            DataGridViewAdvancedCellBorderStyle.Single => DataGridViewHeaderBorderStyle.Single,
            DataGridViewAdvancedCellBorderStyle.InsetDouble => DataGridViewHeaderBorderStyle.Sunken,
            DataGridViewAdvancedCellBorderStyle.OutsetPartial => DataGridViewHeaderBorderStyle.Raised,
            _ => DataGridViewHeaderBorderStyle.Custom,
        };
        set
        {
            // Sequential enum.  Valid values are 0x0 to 0x4
            SourceGenerated.EnumValidator.Validate(value);

            if (value != RowHeadersBorderStyle)
            {
                if (value == DataGridViewHeaderBorderStyle.Custom)
                {
                    throw new ArgumentException(string.Format(SR.DataGridView_CustomCellBorderStyleInvalid, "RowHeadersBorderStyle"));
                }

                _dataGridViewOper[OperationInBorderStyleChange] = true;
                try
                {
                    switch (value)
                    {
                        case DataGridViewHeaderBorderStyle.Single:
                            AdvancedRowHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Single;
                            break;

                        case DataGridViewHeaderBorderStyle.Raised:
                            AdvancedRowHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.OutsetPartial;
                            break;

                        case DataGridViewHeaderBorderStyle.Sunken:
                            AdvancedRowHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.InsetDouble;
                            break;

                        case DataGridViewHeaderBorderStyle.None:
                            AdvancedRowHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                            break;
                    }
                }
                finally
                {
                    _dataGridViewOper[OperationInBorderStyleChange] = false;
                }

                OnRowHeadersBorderStyleChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridView_RowHeadersBorderStyleChangedDescr))]
    public event EventHandler? RowHeadersBorderStyleChanged
    {
        add => Events.AddHandler(s_rowHeadersBorderStyleChangedEvent, value);
        remove => Events.RemoveHandler(s_rowHeadersBorderStyleChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_RowHeadersDefaultCellStyleDescr))]
    [AmbientValue(null)]
    [AllowNull]
    public DataGridViewCellStyle RowHeadersDefaultCellStyle
    {
        get
        {
            _rowHeadersDefaultCellStyle ??= DefaultRowHeadersDefaultCellStyle;

            return _rowHeadersDefaultCellStyle;
        }
        set
        {
            DataGridViewCellStyle cs = RowHeadersDefaultCellStyle;
            cs.RemoveScope(DataGridViewCellStyleScopes.RowHeaders);
            _rowHeadersDefaultCellStyle = value;
            _rowHeadersDefaultCellStyle?.AddScope(this, DataGridViewCellStyleScopes.RowHeaders);

            // Update ambient font flag depending on cell style font
            _dataGridViewState1[State1_AmbientRowHeadersFont] = value?.Font == base.Font;

            DataGridViewCellStyleDifferences dgvcsc = cs.GetDifferencesFrom(RowHeadersDefaultCellStyle);
            if (dgvcsc != DataGridViewCellStyleDifferences.None)
            {
                CellStyleChangedEventArgs.ChangeAffectsPreferredSize = (dgvcsc == DataGridViewCellStyleDifferences.AffectPreferredSize);
                OnRowHeadersDefaultCellStyleChanged(CellStyleChangedEventArgs);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridViewRowHeadersDefaultCellStyleChangedDescr))]
    public event EventHandler? RowHeadersDefaultCellStyleChanged
    {
        add => Events.AddHandler(s_rowHeadersDefaultCellStyleChangedEvent, value);
        remove => Events.RemoveHandler(s_rowHeadersDefaultCellStyleChangedEvent, value);
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the dataGridView's row headers are
    ///  visible.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.DataGridViewRowHeadersVisibleDescr))]
    public bool RowHeadersVisible
    {
        get => _dataGridViewState1[State1_RowHeadersVisible];
        set
        {
            if (RowHeadersVisible != value)
            {
                if (!value
                    && (_autoSizeRowsMode == DataGridViewAutoSizeRowsMode.AllHeaders || _autoSizeRowsMode == DataGridViewAutoSizeRowsMode.DisplayedHeaders))
                {
                    throw new InvalidOperationException(SR.DataGridView_RowHeadersCannotBeInvisible);
                }

                using (LayoutTransaction.CreateTransactionIf(AutoSize, ParentInternal, this, PropertyNames.RowHeadersVisible))
                {
                    _dataGridViewState1[State1_RowHeadersVisible] = value;
                    _layout.RowHeadersVisible = value;
                    DisplayedBandsInfo.EnsureDirtyState();
                    if (!AutoSize)
                    {
                        PerformLayoutPrivate(
                            useRowShortcut: false,
                            computeVisibleRows: false,
                            invalidInAdjustFillingColumns: true,
                            repositionEditingControl: true);
                    }

                    InvalidateInside();
                    OnRowHeadersGlobalAutoSize(expandingRows: value);
                }
            }
        }
    }

    [SRCategory(nameof(SR.CatLayout))]
    [Localizable(true)]
    [SRDescription(nameof(SR.DataGridView_RowHeadersWidthDescr))]
    public int RowHeadersWidth
    {
        get => _rowHeaderWidth;
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, MinimumRowHeadersWidth);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, MaxHeadersThickness);

            if (RowHeadersWidthSizeMode is not DataGridViewRowHeadersWidthSizeMode.EnableResizing
                and not DataGridViewRowHeadersWidthSizeMode.DisableResizing)
            {
                _cachedRowHeadersWidth = value;
            }
            else if (_rowHeaderWidth != value)
            {
                RowHeadersWidthInternal = value;
            }
        }
    }

    private int RowHeadersWidthInternal
    {
        set
        {
            using (LayoutTransaction.CreateTransactionIf(AutoSize, ParentInternal, this, PropertyNames.RowHeadersWidth))
            {
                Debug.Assert(_rowHeaderWidth != value);
                Debug.Assert(value >= MinimumRowHeadersWidth);
                _rowHeaderWidth = value;
                if (AutoSize)
                {
                    InvalidateInside();
                }
                else
                {
                    if (_layout.RowHeadersVisible)
                    {
                        PerformLayoutPrivate(
                            useRowShortcut: false,
                            computeVisibleRows: false,
                            invalidInAdjustFillingColumns: true,
                            repositionEditingControl: true);
                        InvalidateInside();
                    }
                }

                OnRowHeadersWidthChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridViewRowHeadersWidthChangedDescr))]
    public event EventHandler? RowHeadersWidthChanged
    {
        add => Events.AddHandler(s_rowHeadersWidthChangedEvent, value);
        remove => Events.RemoveHandler(s_rowHeadersWidthChangedEvent, value);
    }

    private bool ShouldSerializeRowHeadersWidth() =>
        _rowHeadersWidthSizeMode is DataGridViewRowHeadersWidthSizeMode.EnableResizing or DataGridViewRowHeadersWidthSizeMode.DisableResizing
        && RowHeadersWidth != DefaultRowHeadersWidth;

    /// <summary>
    ///  Gets or sets a value that determines the behavior for adjusting the row headers width.
    /// </summary>
    [DefaultValue(DataGridViewRowHeadersWidthSizeMode.EnableResizing)]
    [RefreshProperties(RefreshProperties.All)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DataGridView_RowHeadersWidthSizeModeDescr))]
    public DataGridViewRowHeadersWidthSizeMode RowHeadersWidthSizeMode
    {
        get
        {
            return _rowHeadersWidthSizeMode;
        }
        set
        {
            // Sequential enum.  Valid values are 0x0 to 0x4
            SourceGenerated.EnumValidator.Validate(value);
            if (_rowHeadersWidthSizeMode != value)
            {
                DataGridViewAutoSizeModeEventArgs dgvasme = new(
                    previousModeAutoSized: _rowHeadersWidthSizeMode is not DataGridViewRowHeadersWidthSizeMode.EnableResizing
                        and not DataGridViewRowHeadersWidthSizeMode.DisableResizing);
                _rowHeadersWidthSizeMode = value;
                OnRowHeadersWidthSizeModeChanged(dgvasme);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridView_RowHeadersWidthSizeModeChangedDescr))]
    public event DataGridViewAutoSizeModeEventHandler? RowHeadersWidthSizeModeChanged
    {
        add => Events.AddHandler(s_rowHeadersWidthSizeModeChangedEvent, value);
        remove => Events.RemoveHandler(s_rowHeadersWidthSizeModeChangedEvent, value);
    }

    [Browsable(false)]
    public DataGridViewRowCollection Rows
    {
        get
        {
            _dataGridViewRows ??= CreateRowsInstance();

            return _dataGridViewRows;
        }
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_RowsDefaultCellStyleDescr))]
    [AllowNull]
    public DataGridViewCellStyle RowsDefaultCellStyle
    {
        get
        {
            if (_rowsDefaultCellStyle is null)
            {
                _rowsDefaultCellStyle = new DataGridViewCellStyle();
                _rowsDefaultCellStyle.AddScope(this, DataGridViewCellStyleScopes.Rows);
            }

            return _rowsDefaultCellStyle;
        }
        set
        {
            DataGridViewCellStyle cs = RowsDefaultCellStyle;
            cs.RemoveScope(DataGridViewCellStyleScopes.Rows);
            _rowsDefaultCellStyle = value;
            _rowsDefaultCellStyle?.AddScope(this, DataGridViewCellStyleScopes.Rows);

            DataGridViewCellStyleDifferences dgvcsc = cs.GetDifferencesFrom(RowsDefaultCellStyle);
            if (dgvcsc != DataGridViewCellStyleDifferences.None)
            {
                CellStyleChangedEventArgs.ChangeAffectsPreferredSize = (dgvcsc == DataGridViewCellStyleDifferences.AffectPreferredSize);
                OnRowsDefaultCellStyleChanged(CellStyleChangedEventArgs);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridViewRowsDefaultCellStyleChangedDescr))]
    public event EventHandler? RowsDefaultCellStyleChanged
    {
        add => Events.AddHandler(s_rowsDefaultCellStyleChangedEvent, value);
        remove => Events.RemoveHandler(s_rowsDefaultCellStyleChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [Browsable(true)]
    [SRDescription(nameof(SR.DataGridView_RowTemplateDescr))]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [AllowNull]
    public DataGridViewRow RowTemplate
    {
        get
        {
            _rowTemplate ??= new DataGridViewRow();

            return _rowTemplate;
        }
        set
        {
            DataGridViewRow? dataGridViewRow = value;
            if (dataGridViewRow is not null)
            {
                if (dataGridViewRow.DataGridView is not null)
                {
                    throw new InvalidOperationException(SR.DataGridView_RowAlreadyBelongsToDataGridView);
                }

                // if (dataGridViewRow.Selected)
                // {
                //    throw new InvalidOperationException(SR.DataGridView_RowTemplateCannotBeSelected);
                // }
            }

            _rowTemplate = dataGridViewRow;
        }
    }

    private bool ShouldSerializeRowTemplate() => _rowTemplate is not null;

    internal DataGridViewRow RowTemplateClone
    {
        get
        {
            DataGridViewRow rowTemplateClone = (DataGridViewRow)RowTemplate.Clone();
            CompleteCellsCollection(rowTemplateClone);
            return rowTemplateClone;
        }
    }

    /// <summary>
    ///  Possible return values are given by the ScrollBars enumeration.
    /// </summary>
    [DefaultValue(ScrollBars.Both)]
    [Localizable(true)]
    [SRCategory(nameof(SR.CatLayout))]
    [SRDescription(nameof(SR.DataGridView_ScrollBarsDescr))]
    public ScrollBars ScrollBars
    {
        get => _scrollBars;
        set
        {
            // Sequential enum.  Valid values are 0x0 to 0x3
            SourceGenerated.EnumValidator.Validate(value);

            if (_scrollBars != value)
            {
                using (LayoutTransaction.CreateTransactionIf(AutoSize, ParentInternal, this, PropertyNames.ScrollBars))
                {
                    // Before changing the value of this.scrollBars, we scroll to the top-left cell to
                    // avoid inconsistent state of scrollbars.
                    DataGridViewColumn? dataGridViewColumn = Columns.GetFirstColumn(DataGridViewElementStates.Visible);
                    int firstVisibleRowIndex = Rows.GetFirstRow(DataGridViewElementStates.Visible);

                    if (dataGridViewColumn is not null && firstVisibleRowIndex != -1)
                    {
                        if (!ScrollIntoView(dataGridViewColumn.Index, firstVisibleRowIndex, false))
                        {
                            throw new InvalidOperationException(SR.DataGridView_CellChangeCannotBeCommittedOrAborted);
                        }
                    }

                    Debug.Assert(HorizontalOffset == 0);
                    Debug.Assert(VerticalOffset == 0);

                    _scrollBars = value;

                    if (!AutoSize)
                    {
                        PerformLayoutPrivate(
                            useRowShortcut: false,
                            computeVisibleRows: false,
                            invalidInAdjustFillingColumns: true,
                            repositionEditingControl: true);
                    }

                    Invalidate();
                }
            }
        }
    }

    [Browsable(false)]
    public DataGridViewSelectedCellCollection SelectedCells
    {
        get
        {
            DataGridViewSelectedCellCollection stcc = [];
            switch (SelectionMode)
            {
                case DataGridViewSelectionMode.CellSelect:
                    {
                        // Note: If we change the design and decide that SelectAll() should use band selection,
                        // we need to add those to the selected cells.
                        stcc.AddCellLinkedList(_individualSelectedCells);
                        break;
                    }

                case DataGridViewSelectionMode.FullColumnSelect:
                case DataGridViewSelectionMode.ColumnHeaderSelect:
                    {
                        foreach (int columnIndex in _selectedBandIndexes)
                        {
                            foreach (DataGridViewRow dataGridViewRow in Rows)   // unshares all rows!
                            {
                                stcc.Add(dataGridViewRow.Cells[columnIndex]);
                            }
                        }

                        if (SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect)
                        {
                            stcc.AddCellLinkedList(_individualSelectedCells);
                        }

                        break;
                    }

                case DataGridViewSelectionMode.FullRowSelect:
                case DataGridViewSelectionMode.RowHeaderSelect:
                    {
                        foreach (int rowIndex in _selectedBandIndexes)
                        {
                            DataGridViewRow dataGridViewRow = Rows[rowIndex]; // unshares the selected row
                            foreach (DataGridViewCell dataGridViewCell in dataGridViewRow.Cells)
                            {
                                stcc.Add(dataGridViewCell);
                            }
                        }

                        if (SelectionMode == DataGridViewSelectionMode.RowHeaderSelect)
                        {
                            stcc.AddCellLinkedList(_individualSelectedCells);
                        }

                        break;
                    }
            }

            return stcc;
        }
    }

    [Browsable(false)]
    public DataGridViewSelectedColumnCollection SelectedColumns
    {
        get
        {
            DataGridViewSelectedColumnCollection strc = [];
            switch (SelectionMode)
            {
                case DataGridViewSelectionMode.CellSelect:
                case DataGridViewSelectionMode.FullRowSelect:
                case DataGridViewSelectionMode.RowHeaderSelect:
                    break;
                case DataGridViewSelectionMode.FullColumnSelect:
                case DataGridViewSelectionMode.ColumnHeaderSelect:
                    foreach (int columnIndex in _selectedBandIndexes)
                    {
                        strc.Add(Columns[columnIndex]);
                    }

                    break;
            }

            return strc;
        }
    }

    [Browsable(false)]
    public DataGridViewSelectedRowCollection SelectedRows
    {
        get
        {
            DataGridViewSelectedRowCollection strc = [];
            switch (SelectionMode)
            {
                case DataGridViewSelectionMode.CellSelect:
                case DataGridViewSelectionMode.FullColumnSelect:
                case DataGridViewSelectionMode.ColumnHeaderSelect:
                    break;
                case DataGridViewSelectionMode.FullRowSelect:
                case DataGridViewSelectionMode.RowHeaderSelect:
                    foreach (int rowIndex in _selectedBandIndexes)
                    {
                        strc.Add(Rows[rowIndex]); // unshares the selected row
                    }

                    break;
            }

            return strc;
        }
    }

    [Browsable(true)]
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(DataGridViewSelectionMode.RowHeaderSelect)]
    [SRDescription(nameof(SR.DataGridView_SelectionModeDescr))]
    public DataGridViewSelectionMode SelectionMode
    {
        get => _selectionMode;
        set
        {
            // Sequential enum.  Valid values are 0x0 to 0x4
            SourceGenerated.EnumValidator.Validate(value);

            if (SelectionMode != value)
            {
                if (!_dataGridViewState2[State2_Initializing]
                    && (value == DataGridViewSelectionMode.FullColumnSelect || value == DataGridViewSelectionMode.ColumnHeaderSelect))
                {
                    foreach (DataGridViewColumn dataGridViewColumn in Columns)
                    {
                        if (dataGridViewColumn.SortMode == DataGridViewColumnSortMode.Automatic)
                        {
                            throw new InvalidOperationException(string.Format(SR.DataGridView_SelectionModeAndSortModeClash, (value).ToString()));
                        }
                    }
                }

                ClearSelection();
                _selectionMode = value;
            }
        }
    }

    [DefaultValue(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_ShowCellErrorsDescr))]
    public bool ShowCellErrors
    {
        get => _dataGridViewState2[State2_ShowCellErrors];
        set
        {
            if (_dataGridViewState2[State2_ShowCellErrors] != value)
            {
                _dataGridViewState2[State2_ShowCellErrors] = value;

                // Put this into OnShowCellErrorsChanged if created.
                if (IsHandleCreated && !DesignMode)
                {
                    if (!ShowRowErrors && !ShowCellToolTips)
                    {
                        if (value)
                        {
                            // The tool tip hasn't yet been activated
                            // activate it now
                            _toolTipControl.Activate(!string.IsNullOrEmpty(ToolTipPrivate));
                        }
                        else
                        {
                            // There is no reason to keep the tool tip activated
                            // deactivate it
                            ToolTipPrivate = string.Empty;
                            _toolTipControl.Activate(activate: false);
                        }
                    }
                    else
                    {
                        if (!value)
                        {
                            // Reset the tool tip
                            _toolTipControl.Activate(!string.IsNullOrEmpty(ToolTipPrivate));
                        }
                    }

                    // Some autosizing may have to be applied since the potential presence of error icons influences the preferred sizes.
                    OnGlobalAutoSize();
                }

                if (!_layout._dirty && !DesignMode)
                {
                    Invalidate(Rectangle.Union(_layout.Data, _layout.ColumnHeaders));
                    Invalidate(_layout.TopLeftHeader);
                }
            }
        }
    }

    [DefaultValue(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_ShowCellToolTipsDescr))]
    public bool ShowCellToolTips
    {
        get => _dataGridViewState2[State2_ShowCellToolTips];
        set
        {
            if (_dataGridViewState2[State2_ShowCellToolTips] != value)
            {
                _dataGridViewState2[State2_ShowCellToolTips] = value;

                if (IsHandleCreated && !DesignMode)
                {
                    if (!ShowRowErrors && !ShowCellErrors)
                    {
                        if (value)
                        {
                            // The tool tip hasn't yet been activated
                            // activate it now
                            _toolTipControl.Activate(activate: !string.IsNullOrEmpty(ToolTipPrivate));
                        }
                        else
                        {
                            // There is no reason to keep the tool tip activated
                            // deactivate it
                            ToolTipPrivate = string.Empty;
                            _toolTipControl.Activate(activate: false);
                        }
                    }
                    else
                    {
                        if (!value)
                        {
                            bool activate = !string.IsNullOrEmpty(ToolTipPrivate);
                            Point mouseCoord = MousePosition;
                            activate &= ClientRectangle.Contains(PointToClient(mouseCoord));

                            // Reset the tool tip
                            _toolTipControl.Activate(activate);
                        }
                    }
                }

                if (!_layout._dirty && !DesignMode)
                {
                    Invalidate(_layout.Data);
                }
            }
        }
    }

    [DefaultValue(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_ShowEditingIconDescr))]
    public bool ShowEditingIcon
    {
        get => _dataGridViewState2[State2_ShowEditingIcon];
        set
        {
            if (ShowEditingIcon != value)
            {
                _dataGridViewState2[State2_ShowEditingIcon] = value;

                // invalidate the row header to pick up the new ShowEditingIcon value
                if (RowHeadersVisible)
                {
                    if (VirtualMode || DataSource is not null)
                    {
                        if (IsCurrentRowDirty)
                        {
                            Debug.Assert(_ptCurrentCell.Y >= 0);
                            InvalidateCellPrivate(-1, _ptCurrentCell.Y);
                        }
                    }
                    else
                    {
                        if (IsCurrentCellDirty)
                        {
                            Debug.Assert(_ptCurrentCell.Y >= 0);
                            InvalidateCellPrivate(-1, _ptCurrentCell.Y);
                        }
                    }
                }
            }
        }
    }

    [DefaultValue(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_ShowRowErrorsDescr))]
    public bool ShowRowErrors
    {
        get => _dataGridViewState2[State2_ShowRowErrors];
        set
        {
            if (ShowRowErrors != value)
            {
                _dataGridViewState2[State2_ShowRowErrors] = value;

                if (IsHandleCreated && !DesignMode)
                {
                    if (value && !ShowCellErrors && !ShowCellToolTips)
                    {
                        // the tool tip hasn't yet been activated
                        // activate it now
                        _toolTipControl.Activate(!string.IsNullOrEmpty(ToolTipPrivate));
                    }

                    if (!value && !ShowCellErrors && !ShowCellToolTips)
                    {
                        // there is no reason to keep the tool tip activated
                        // deactivate it
                        ToolTipPrivate = string.Empty;
                        _toolTipControl.Activate(activate: false);
                    }

                    if (!value && (ShowCellErrors || ShowCellToolTips))
                    {
                        // reset the tool tip
                        _toolTipControl.Activate(activate: !string.IsNullOrEmpty(ToolTipPrivate));
                    }
                }

                if (!_layout._dirty && !DesignMode)
                {
                    Invalidate(_layout.RowHeaders);
                }
            }
        }
    }

    internal bool SingleHorizontalBorderAdded =>
        !_layout.ColumnHeadersVisible
        && (AdvancedCellBorderStyle.All == DataGridViewAdvancedCellBorderStyle.Single
            || CellBorderStyle == DataGridViewCellBorderStyle.SingleHorizontal);

    internal bool SingleVerticalBorderAdded =>
        !_layout.RowHeadersVisible
        && (AdvancedCellBorderStyle.All == DataGridViewAdvancedCellBorderStyle.Single
            || CellBorderStyle == DataGridViewCellBorderStyle.SingleVertical);

    [Browsable(false)]
    public DataGridViewColumn? SortedColumn { get; private set; }

    [Browsable(false)]
    public SortOrder SortOrder { get; private set; }

    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SRDescription(nameof(SR.DataGridView_StandardTabDescr))]
    public bool StandardTab
    {
        get => _dataGridViewState1[State1_StandardTab];
        set
        {
            if (_dataGridViewState1[State1_StandardTab] != value)
            {
                _dataGridViewState1[State1_StandardTab] = value;
            }
        }
    }

    internal override bool SupportsUiaProviders => true;

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Bindable(false)]
    [AllowNull]
    public override string Text
    {
        get => base.Text;
        set => base.Text = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? TextChanged
    {
        add => base.TextChanged += value;
        remove => base.TextChanged -= value;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DataGridViewCell this[int columnIndex, int rowIndex]
    {
        get
        {
            DataGridViewRow row = Rows[rowIndex];
            return row.Cells[columnIndex];
        }
        set
        {
            DataGridViewRow row = Rows[rowIndex];
            row.Cells[columnIndex] = value;
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DataGridViewCell this[string columnName, int rowIndex]
    {
        get
        {
            DataGridViewRow row = Rows[rowIndex];
            return row.Cells[columnName];
        }
        set
        {
            DataGridViewRow row = Rows[rowIndex];
            row.Cells[columnName] = value;
        }
    }

    private string ToolTipPrivate { get; set; } = string.Empty;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [AllowNull]
    public DataGridViewHeaderCell TopLeftHeaderCell
    {
        get
        {
            if (_topLeftHeaderCell is null)
            {
                TopLeftHeaderCell = new DataGridViewTopLeftHeaderCell();
            }

            return _topLeftHeaderCell!;
        }
        set
        {
            if (_topLeftHeaderCell != value)
            {
                if (_topLeftHeaderCell is not null)
                {
                    // Detach existing header cell
                    _topLeftHeaderCell.DataGridView = null;
                }

                _topLeftHeaderCell = value;
                if (_topLeftHeaderCell is not null)
                {
                    _topLeftHeaderCell.DataGridView = this;
                }

                if (ColumnHeadersVisible && RowHeadersVisible)
                {
                    // If headers (rows or columns) are autosized, then this.RowHeadersWidth or this.ColumnHeadersHeight
                    // must be updated based on new cell preferred size
                    OnColumnHeadersGlobalAutoSize();
                    // In all cases, the top left cell needs to repaint
                    Invalidate(new Rectangle(_layout.Inside.X, _layout.Inside.Y, RowHeadersWidth, ColumnHeadersHeight));
                }
            }
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public Cursor? UserSetCursor => _dataGridViewState1[State1_CustomCursorSet]
        ? _oldCursor
        : Cursor;

    internal int VerticalOffset
    {
        get => VerticalScrollingOffset;
        set
        {
            if (value < 0)
            {
                value = 0;
            }

            int totalVisibleFrozenHeight = Rows.GetRowsHeight(DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen);
            int fittingTrailingScrollingRowsHeight = ComputeHeightOfFittingTrailingScrollingRows(totalVisibleFrozenHeight);
            if (value > _vertScrollBar.Maximum - fittingTrailingScrollingRowsHeight)
            {
                value = _vertScrollBar.Maximum - fittingTrailingScrollingRowsHeight;
            }

            if (value == VerticalScrollingOffset)
            {
                return;
            }

            int change = value - VerticalScrollingOffset;
            if (_vertScrollBar.Enabled)
            {
                _vertScrollBar.Value = value;
            }

            ScrollRowsByHeight(change); // calculate how many rows need to be scrolled based on 'change'
        }
    }

    protected ScrollBar VerticalScrollBar => _vertScrollBar;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int VerticalScrollingOffset { get; private set; }

    private Timer VertScrollTimer
    {
        get
        {
            if (_vertScrollTimer is null)
            {
                _vertScrollTimer = new Timer();
                _vertScrollTimer.Tick += new EventHandler(VertScrollTimer_Tick);
            }

            return _vertScrollTimer;
        }
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SRDescription(nameof(SR.DataGridViewVirtualModeDescr))]
    public bool VirtualMode
    {
        get => _dataGridViewState1[State1_VirtualMode];
        set
        {
            if (_dataGridViewState1[State1_VirtualMode] != value)
            {
                _dataGridViewState1[State1_VirtualMode] = value;
                InvalidateRowHeights();
            }
        }
    }

    private bool VisibleCellExists =>
        Columns.GetFirstColumn(DataGridViewElementStates.Visible) is not null
        && Rows.GetFirstRow(DataGridViewElementStates.Visible) != -1;

    // Events start here

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridViewAutoSizeColumnModeChangedDescr))]
    public event DataGridViewAutoSizeColumnModeEventHandler? AutoSizeColumnModeChanged
    {
        add => Events.AddHandler(s_autosizeColumnModeChangedEvent, value);
        remove => Events.RemoveHandler(s_autosizeColumnModeChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.DataGridView_CancelRowEditDescr))]
    public event QuestionEventHandler? CancelRowEdit
    {
        add => Events.AddHandler(s_cancelRowEditEvent, value);
        remove => Events.RemoveHandler(s_cancelRowEditEvent, value);
    }

    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.DataGridView_CellBeginEditDescr))]
    public event DataGridViewCellCancelEventHandler? CellBeginEdit
    {
        add => Events.AddHandler(s_cellBeginEditEvent, value);
        remove => Events.RemoveHandler(s_cellBeginEditEvent, value);
    }

    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.DataGridView_CellClickDescr))]
    public event DataGridViewCellEventHandler? CellClick
    {
        add => Events.AddHandler(s_cellClickEvent, value);
        remove => Events.RemoveHandler(s_cellClickEvent, value);
    }

    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.DataGridView_CellContentClick))]
    public event DataGridViewCellEventHandler? CellContentClick
    {
        add => Events.AddHandler(s_cellContentClickEvent, value);
        remove => Events.RemoveHandler(s_cellContentClickEvent, value);
    }

    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.DataGridView_CellContentDoubleClick))]
    public event DataGridViewCellEventHandler? CellContentDoubleClick
    {
        add => Events.AddHandler(s_cellContentDoubleClickEvent, value);
        remove => Events.RemoveHandler(s_cellContentDoubleClickEvent, value);
    }

    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.DataGridView_CellContextMenuStripChanged))]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public event DataGridViewCellEventHandler? CellContextMenuStripChanged
    {
        add => Events.AddHandler(s_cellContextMenuStripChangedEvent, value);
        remove => Events.RemoveHandler(s_cellContextMenuStripChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DataGridView_CellContextMenuStripNeeded))]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public event DataGridViewCellContextMenuStripNeededEventHandler? CellContextMenuStripNeeded
    {
        add => Events.AddHandler(s_cellContextMenuStripNeededEvent, value);
        remove => Events.RemoveHandler(s_cellContextMenuStripNeededEvent, value);
    }

    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.DataGridView_CellDoubleClickDescr))]
    public event DataGridViewCellEventHandler? CellDoubleClick
    {
        add => Events.AddHandler(s_cellDoubleClickEvent, value);
        remove => Events.RemoveHandler(s_cellDoubleClickEvent, value);
    }

    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.DataGridView_CellEndEditDescr))]
    public event DataGridViewCellEventHandler? CellEndEdit
    {
        add => Events.AddHandler(s_cellEndEditEvent, value);
        remove => Events.RemoveHandler(s_cellEndEditEvent, value);
    }

    [SRCategory(nameof(SR.CatFocus))]
    [SRDescription(nameof(SR.DataGridView_CellEnterDescr))]
    public event DataGridViewCellEventHandler? CellEnter
    {
        add => Events.AddHandler(s_cellEnterEvent, value);
        remove => Events.RemoveHandler(s_cellEnterEvent, value);
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridView_CellErrorTextChangedDescr))]
    public event DataGridViewCellEventHandler? CellErrorTextChanged
    {
        add => Events.AddHandler(s_cellErrorTextChangedEvent, value);
        remove => Events.RemoveHandler(s_cellErrorTextChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatData))]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SRDescription(nameof(SR.DataGridView_CellErrorTextNeededDescr))]
    public event DataGridViewCellErrorTextNeededEventHandler? CellErrorTextNeeded
    {
        add => Events.AddHandler(s_cellErrorTextNeededEvent, value);
        remove => Events.RemoveHandler(s_cellErrorTextNeededEvent, value);
    }

    [SRCategory(nameof(SR.CatDisplay))]
    [SRDescription(nameof(SR.DataGridView_CellFormattingDescr))]
    public event DataGridViewCellFormattingEventHandler? CellFormatting
    {
        add => Events.AddHandler(s_cellFormattingEvent, value);
        remove => Events.RemoveHandler(s_cellFormattingEvent, value);
    }

    [SRCategory(nameof(SR.CatFocus))]
    [SRDescription(nameof(SR.DataGridView_CellLeaveDescr))]
    public event DataGridViewCellEventHandler? CellLeave
    {
        add => Events.AddHandler(s_cellLeaveEvent, value);
        remove => Events.RemoveHandler(s_cellLeaveEvent, value);
    }

    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.DataGridView_CellMouseClickDescr))]
    public event DataGridViewCellMouseEventHandler? CellMouseClick
    {
        add => Events.AddHandler(s_cellMouseClickEvent, value);
        remove => Events.RemoveHandler(s_cellMouseClickEvent, value);
    }

    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.DataGridView_CellMouseDoubleClickDescr))]
    public event DataGridViewCellMouseEventHandler? CellMouseDoubleClick
    {
        add => Events.AddHandler(s_cellMouseDoubleClickEvent, value);
        remove => Events.RemoveHandler(s_cellMouseDoubleClickEvent, value);
    }

    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.DataGridView_CellMouseDownDescr))]
    public event DataGridViewCellMouseEventHandler? CellMouseDown
    {
        add => Events.AddHandler(s_cellMouseDownEvent, value);
        remove => Events.RemoveHandler(s_cellMouseDownEvent, value);
    }

    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.DataGridView_CellMouseEnterDescr))]
    public event DataGridViewCellEventHandler? CellMouseEnter
    {
        add => Events.AddHandler(s_cellMouseEnterEvent, value);
        remove => Events.RemoveHandler(s_cellMouseEnterEvent, value);
    }

    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.DataGridView_CellMouseLeaveDescr))]
    public event DataGridViewCellEventHandler? CellMouseLeave
    {
        add => Events.AddHandler(s_cellMouseLeaveEvent, value);
        remove => Events.RemoveHandler(s_cellMouseLeaveEvent, value);
    }

    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.DataGridView_CellMouseMoveDescr))]
    public event DataGridViewCellMouseEventHandler? CellMouseMove
    {
        add => Events.AddHandler(s_cellMouseMoveEvent, value);
        remove => Events.RemoveHandler(s_cellMouseMoveEvent, value);
    }

    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.DataGridView_CellMouseUpDescr))]
    public event DataGridViewCellMouseEventHandler? CellMouseUp
    {
        add => Events.AddHandler(s_cellMouseUpEvent, value);
        remove => Events.RemoveHandler(s_cellMouseUpEvent, value);
    }

    [SRCategory(nameof(SR.CatDisplay))]
    [SRDescription(nameof(SR.DataGridView_CellPaintingDescr))]
    public event DataGridViewCellPaintingEventHandler? CellPainting
    {
        add => Events.AddHandler(s_cellPaintingEvent, value);
        remove => Events.RemoveHandler(s_cellPaintingEvent, value);
    }

    [SRCategory(nameof(SR.CatDisplay))]
    [SRDescription(nameof(SR.DataGridView_CellParsingDescr))]
    public event DataGridViewCellParsingEventHandler? CellParsing
    {
        add => Events.AddHandler(s_cellParsingEvent, value);
        remove => Events.RemoveHandler(s_cellParsingEvent, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DataGridView_CellStateChangedDescr))]
    public event DataGridViewCellStateChangedEventHandler? CellStateChanged
    {
        add => Events.AddHandler(s_cellStateChangedEvent, value);
        remove => Events.RemoveHandler(s_cellStateChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridView_CellStyleChangedDescr))]
    public event DataGridViewCellEventHandler? CellStyleChanged
    {
        add => Events.AddHandler(s_cellStyleChangedEvent, value);
        remove => Events.RemoveHandler(s_cellStyleChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridView_CellStyleContentChangedDescr))]
    public event DataGridViewCellStyleContentChangedEventHandler? CellStyleContentChanged
    {
        add => Events.AddHandler(s_cellStyleContentChangedEvent, value);
        remove => Events.RemoveHandler(s_cellStyleContentChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridView_CellToolTipTextChangedDescr))]
    public event DataGridViewCellEventHandler? CellToolTipTextChanged
    {
        add => Events.AddHandler(s_cellTooltipTextChangedEvent, value);
        remove => Events.RemoveHandler(s_cellTooltipTextChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DataGridView_CellToolTipTextNeededDescr))]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public event DataGridViewCellToolTipTextNeededEventHandler? CellToolTipTextNeeded
    {
        add => Events.AddHandler(s_cellTooltipTextNeededEvent, value);
        remove => Events.RemoveHandler(s_cellTooltipTextNeededEvent, value);
    }

    [SRCategory(nameof(SR.CatFocus))]
    [SRDescription(nameof(SR.DataGridView_CellValidatedDescr))]
    public event DataGridViewCellEventHandler? CellValidated
    {
        add => Events.AddHandler(s_cellValidatedEvent, value);
        remove => Events.RemoveHandler(s_cellValidatedEvent, value);
    }

    [SRCategory(nameof(SR.CatFocus))]
    [SRDescription(nameof(SR.DataGridView_CellValidatingDescr))]
    public event DataGridViewCellValidatingEventHandler? CellValidating
    {
        add => Events.AddHandler(s_cellValidatingEvent, value);
        remove => Events.RemoveHandler(s_cellValidatingEvent, value);
    }

    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.DataGridView_CellValueChangedDescr))]
    public event DataGridViewCellEventHandler? CellValueChanged
    {
        add => Events.AddHandler(s_cellValueChangedEvent, value);
        remove => Events.RemoveHandler(s_cellValueChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatData))]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SRDescription(nameof(SR.DataGridView_CellValueNeededDescr))]
    public event DataGridViewCellValueEventHandler? CellValueNeeded
    {
        add => Events.AddHandler(s_cellValueNeededEvent, value);
        remove => Events.RemoveHandler(s_cellValueNeededEvent, value);
    }

    [SRCategory(nameof(SR.CatData))]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SRDescription(nameof(SR.DataGridView_CellValuePushedDescr))]
    public event DataGridViewCellValueEventHandler? CellValuePushed
    {
        add => Events.AddHandler(s_cellValuePushedEvent, value);
        remove => Events.RemoveHandler(s_cellValuePushedEvent, value);
    }

    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.DataGridView_ColumnAddedDescr))]
    public event DataGridViewColumnEventHandler? ColumnAdded
    {
        add => Events.AddHandler(s_columnAddedEvent, value);
        remove => Events.RemoveHandler(s_columnAddedEvent, value);
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridView_ColumnContextMenuStripChangedDescr))]
    public event DataGridViewColumnEventHandler? ColumnContextMenuStripChanged
    {
        add => Events.AddHandler(s_columnContextMenuStripChangedEvent, value);
        remove => Events.RemoveHandler(s_columnContextMenuStripChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridView_ColumnDataPropertyNameChangedDescr))]
    public event DataGridViewColumnEventHandler? ColumnDataPropertyNameChanged
    {
        add => Events.AddHandler(s_columnDataPropertyNameChangedEvent, value);
        remove => Events.RemoveHandler(s_columnDataPropertyNameChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridView_ColumnDefaultCellStyleChangedDescr))]
    public event DataGridViewColumnEventHandler? ColumnDefaultCellStyleChanged
    {
        add => Events.AddHandler(s_columnDefaultCellStyleChangedEvent, value);
        remove => Events.RemoveHandler(s_columnDefaultCellStyleChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridView_ColumnDisplayIndexChangedDescr))]
    public event DataGridViewColumnEventHandler? ColumnDisplayIndexChanged
    {
        add => Events.AddHandler(s_columnDisplayIndexChangedEvent, value);
        remove => Events.RemoveHandler(s_columnDisplayIndexChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.DataGridView_ColumnDividerDoubleClickDescr))]
    public event DataGridViewColumnDividerDoubleClickEventHandler? ColumnDividerDoubleClick
    {
        add => Events.AddHandler(s_columnDividerDoubleClickEvent, value);
        remove => Events.RemoveHandler(s_columnDividerDoubleClickEvent, value);
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridView_ColumnDividerWidthChangedDescr))]
    public event DataGridViewColumnEventHandler? ColumnDividerWidthChanged
    {
        add => Events.AddHandler(s_columnDividerWidthChangedEvent, value);
        remove => Events.RemoveHandler(s_columnDividerWidthChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.DataGridView_ColumnHeaderMouseClickDescr))]
    public event DataGridViewCellMouseEventHandler? ColumnHeaderMouseClick
    {
        add => Events.AddHandler(s_columnHeaderMouseClickEvent, value);
        remove => Events.RemoveHandler(s_columnHeaderMouseClickEvent, value);
    }

    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.DataGridView_ColumnHeaderMouseDoubleClickDescr))]
    public event DataGridViewCellMouseEventHandler? ColumnHeaderMouseDoubleClick
    {
        add => Events.AddHandler(s_columnHeaderMouseDoubleClickEvent, value);
        remove => Events.RemoveHandler(s_columnHeaderMouseDoubleClickEvent, value);
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridView_ColumnHeaderCellChangedDescr))]
    public event DataGridViewColumnEventHandler? ColumnHeaderCellChanged
    {
        add => Events.AddHandler(s_columnHeaderCellChangedEvent, value);
        remove => Events.RemoveHandler(s_columnHeaderCellChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridView_ColumnMinimumWidthChangedDescr))]
    public event DataGridViewColumnEventHandler? ColumnMinimumWidthChanged
    {
        add => Events.AddHandler(s_columnMinimumWidthChangedEvent, value);
        remove => Events.RemoveHandler(s_columnMinimumWidthChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridView_ColumnNameChangedDescr))]
    public event DataGridViewColumnEventHandler? ColumnNameChanged
    {
        add => Events.AddHandler(s_columnNameChangedEvent, value);
        remove => Events.RemoveHandler(s_columnNameChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.DataGridView_ColumnRemovedDescr))]
    public event DataGridViewColumnEventHandler? ColumnRemoved
    {
        add => Events.AddHandler(s_columnRemovedEvent, value);
        remove => Events.RemoveHandler(s_columnRemovedEvent, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DataGridViewColumnSortModeChangedDescr))]
    public event DataGridViewColumnEventHandler? ColumnSortModeChanged
    {
        add => Events.AddHandler(s_columnSortModeChangedEvent, value);
        remove => Events.RemoveHandler(s_columnSortModeChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DataGridView_ColumnStateChangedDescr))]
    public event DataGridViewColumnStateChangedEventHandler? ColumnStateChanged
    {
        add => Events.AddHandler(s_columnStateChangedEvent, value);
        remove => Events.RemoveHandler(s_columnStateChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridView_ColumnToolTipTextChangedDescr))]
    public event DataGridViewColumnEventHandler? ColumnToolTipTextChanged
    {
        add => Events.AddHandler(s_columnTooltipTextChangedEvent, value);
        remove => Events.RemoveHandler(s_columnTooltipTextChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.DataGridView_ColumnWidthChangedDescr))]
    public event DataGridViewColumnEventHandler? ColumnWidthChanged
    {
        add => Events.AddHandler(s_columnWidthChangedEvent, value);
        remove => Events.RemoveHandler(s_columnWidthChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.DataGridView_CurrentCellChangedDescr))]
    public event EventHandler? CurrentCellChanged
    {
        add => Events.AddHandler(s_currentCellChangedEvent, value);
        remove => Events.RemoveHandler(s_currentCellChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SRDescription(nameof(SR.DataGridView_CurrentCellDirtyStateChangedDescr))]
    public event EventHandler? CurrentCellDirtyStateChanged
    {
        add => Events.AddHandler(s_currentCellDirtyStateChangedEvent, value);
        remove => Events.RemoveHandler(s_currentCellDirtyStateChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.DataGridView_DataBindingCompleteDescr))]
    public event DataGridViewBindingCompleteEventHandler? DataBindingComplete
    {
        add => Events.AddHandler(s_dataBindingCompleteEvent, value);
        remove => Events.RemoveHandler(s_dataBindingCompleteEvent, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DataGridView_DataErrorDescr))]
    public event DataGridViewDataErrorEventHandler? DataError
    {
        add => Events.AddHandler(s_dataErrorEvent, value);
        remove => Events.RemoveHandler(s_dataErrorEvent, value);
    }

    [SRCategory(nameof(SR.CatData))]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SRDescription(nameof(SR.DataGridView_DefaultValuesNeededDescr))]
    public event DataGridViewRowEventHandler? DefaultValuesNeeded
    {
        add => Events.AddHandler(s_defaultValuesNeededEvent, value);
        remove => Events.RemoveHandler(s_defaultValuesNeededEvent, value);
    }

    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.DataGridView_EditingControlShowingDescr))]
    public event DataGridViewEditingControlShowingEventHandler? EditingControlShowing
    {
        add => Events.AddHandler(s_editingControlShowingEvent, value);
        remove => Events.RemoveHandler(s_editingControlShowingEvent, value);
    }

    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.DataGridView_NewRowNeededDescr))]
    public event DataGridViewRowEventHandler? NewRowNeeded
    {
        add => Events.AddHandler(s_newRowNeededEvent, value);
        remove => Events.RemoveHandler(s_newRowNeededEvent, value);
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridView_RowContextMenuStripChangedDescr))]
    public event DataGridViewRowEventHandler? RowContextMenuStripChanged
    {
        add => Events.AddHandler(s_rowContextMenuStripChangedEvent, value);
        remove => Events.RemoveHandler(s_rowContextMenuStripChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatData))]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SRDescription(nameof(SR.DataGridView_RowContextMenuStripNeededDescr))]
    public event DataGridViewRowContextMenuStripNeededEventHandler? RowContextMenuStripNeeded
    {
        add => Events.AddHandler(s_rowContextMenuStripNeededEvent, value);
        remove => Events.RemoveHandler(s_rowContextMenuStripNeededEvent, value);
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridView_RowDefaultCellStyleChangedDescr))]
    public event DataGridViewRowEventHandler? RowDefaultCellStyleChanged
    {
        add => Events.AddHandler(s_rowDefaultCellStyleChangedEvent, value);
        remove => Events.RemoveHandler(s_rowDefaultCellStyleChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatData))]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SRDescription(nameof(SR.DataGridView_RowDirtyStateNeededDescr))]
    public event QuestionEventHandler? RowDirtyStateNeeded
    {
        add => Events.AddHandler(s_rowDirtyStateNeededEvent, value);
        remove => Events.RemoveHandler(s_rowDirtyStateNeededEvent, value);
    }

    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.DataGridView_RowDividerDoubleClickDescr))]
    public event DataGridViewRowDividerDoubleClickEventHandler? RowDividerDoubleClick
    {
        add => Events.AddHandler(s_rowDividerDoubleClickEvent, value);
        remove => Events.RemoveHandler(s_rowDividerDoubleClickEvent, value);
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_RowDividerHeightChangedDescr))]
    public event DataGridViewRowEventHandler? RowDividerHeightChanged
    {
        add => Events.AddHandler(s_rowDividerHeightChangedEvent, value);
        remove => Events.RemoveHandler(s_rowDividerHeightChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatFocus))]
    [SRDescription(nameof(SR.DataGridView_RowEnterDescr))]
    public event DataGridViewCellEventHandler? RowEnter
    {
        add => Events.AddHandler(s_rowEnterEvent, value);
        remove => Events.RemoveHandler(s_rowEnterEvent, value);
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridView_RowErrorTextChangedDescr))]
    public event DataGridViewRowEventHandler? RowErrorTextChanged
    {
        add => Events.AddHandler(s_rowErrorTextChangedEvent, value);
        remove => Events.RemoveHandler(s_rowErrorTextChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatData))]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SRDescription(nameof(SR.DataGridView_RowErrorTextNeededDescr))]
    public event DataGridViewRowErrorTextNeededEventHandler? RowErrorTextNeeded
    {
        add => Events.AddHandler(s_rowErrorTextNeededEvent, value);
        remove => Events.RemoveHandler(s_rowErrorTextNeededEvent, value);
    }

    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.DataGridView_RowHeaderMouseClickDescr))]
    public event DataGridViewCellMouseEventHandler? RowHeaderMouseClick
    {
        add => Events.AddHandler(s_rowHeaderMouseClickEvent, value);
        remove => Events.RemoveHandler(s_rowHeaderMouseClickEvent, value);
    }

    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.DataGridView_RowHeaderMouseDoubleClickDescr))]
    public event DataGridViewCellMouseEventHandler? RowHeaderMouseDoubleClick
    {
        add => Events.AddHandler(s_rowHeaderMouseDoubleClickEvent, value);
        remove => Events.RemoveHandler(s_rowHeaderMouseDoubleClickEvent, value);
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridView_RowHeaderCellChangedDescr))]
    public event DataGridViewRowEventHandler? RowHeaderCellChanged
    {
        add => Events.AddHandler(s_rowHeaderCellChangedEvent, value);
        remove => Events.RemoveHandler(s_rowHeaderCellChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridView_RowHeightChangedDescr))]
    public event DataGridViewRowEventHandler? RowHeightChanged
    {
        add => Events.AddHandler(s_rowHeightChangedEvent, value);
        remove => Events.RemoveHandler(s_rowHeightChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatData))]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SRDescription(nameof(SR.DataGridView_RowHeightInfoNeededDescr))]
    public event DataGridViewRowHeightInfoNeededEventHandler? RowHeightInfoNeeded
    {
        add => Events.AddHandler(s_rowHeightInfoNeededEvent, value);
        remove => Events.RemoveHandler(s_rowHeightInfoNeededEvent, value);
    }

    internal DataGridViewRowHeightInfoNeededEventArgs RowHeightInfoNeededEventArgs
    {
        get
        {
            _dgvrhine ??= new DataGridViewRowHeightInfoNeededEventArgs();

            return _dgvrhine;
        }
    }

    [SRCategory(nameof(SR.CatData))]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SRDescription(nameof(SR.DataGridView_RowHeightInfoPushedDescr))]
    public event DataGridViewRowHeightInfoPushedEventHandler? RowHeightInfoPushed
    {
        add => Events.AddHandler(s_rowHeightInfoPushedEvent, value);
        remove => Events.RemoveHandler(s_rowHeightInfoPushedEvent, value);
    }

    [SRCategory(nameof(SR.CatFocus))]
    [SRDescription(nameof(SR.DataGridView_RowLeaveDescr))]
    public event DataGridViewCellEventHandler? RowLeave
    {
        add => Events.AddHandler(s_rowLeaveEvent, value);
        remove => Events.RemoveHandler(s_rowLeaveEvent, value);
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DataGridView_RowMinimumHeightChangedDescr))]
    public event DataGridViewRowEventHandler? RowMinimumHeightChanged
    {
        add => Events.AddHandler(s_rowMinimumHeightChangeEvent, value);
        remove => Events.RemoveHandler(s_rowMinimumHeightChangeEvent, value);
    }

    [SRCategory(nameof(SR.CatDisplay))]
    [SRDescription(nameof(SR.DataGridView_RowPostPaintDescr))]
    public event DataGridViewRowPostPaintEventHandler? RowPostPaint
    {
        add => Events.AddHandler(s_rowPostPaintEvent, value);
        remove => Events.RemoveHandler(s_rowPostPaintEvent, value);
    }

    internal DataGridViewRowPostPaintEventArgs RowPostPaintEventArgs
    {
        get
        {
            _dgvrpope ??= new DataGridViewRowPostPaintEventArgs(this);

            return _dgvrpope;
        }
    }

    [SRCategory(nameof(SR.CatDisplay))]
    [SRDescription(nameof(SR.DataGridView_RowPrePaintDescr))]
    public event DataGridViewRowPrePaintEventHandler? RowPrePaint
    {
        add => Events.AddHandler(s_rowPrePaintEvent, value);
        remove => Events.RemoveHandler(s_rowPrePaintEvent, value);
    }

    internal DataGridViewRowPrePaintEventArgs RowPrePaintEventArgs
    {
        get
        {
            _dgvrprpe ??= new DataGridViewRowPrePaintEventArgs(this);

            return _dgvrprpe;
        }
    }

    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.DataGridView_RowsAddedDescr))]
    public event DataGridViewRowsAddedEventHandler? RowsAdded
    {
        add => Events.AddHandler(s_rowsAddedEvent, value);
        remove => Events.RemoveHandler(s_rowsAddedEvent, value);
    }

    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.DataGridView_RowsRemovedDescr))]
    public event DataGridViewRowsRemovedEventHandler? RowsRemoved
    {
        add => Events.AddHandler(s_rowsRemovedEvent, value);
        remove => Events.RemoveHandler(s_rowsRemovedEvent, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DataGridView_RowStateChangedDescr))]
    public event DataGridViewRowStateChangedEventHandler? RowStateChanged
    {
        add => Events.AddHandler(s_rowStateChangedEvent, value);
        remove => Events.RemoveHandler(s_rowStateChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SRDescription(nameof(SR.DataGridView_RowUnsharedDescr))]
    public event DataGridViewRowEventHandler? RowUnshared
    {
        add => Events.AddHandler(s_rowUnsharedEvent, value);
        remove => Events.RemoveHandler(s_rowUnsharedEvent, value);
    }

    [SRCategory(nameof(SR.CatFocus))]
    [SRDescription(nameof(SR.DataGridView_RowValidatedDescr))]
    public event DataGridViewCellEventHandler? RowValidated
    {
        add => Events.AddHandler(s_rowValidatedEvent, value);
        remove => Events.RemoveHandler(s_rowValidatedEvent, value);
    }

    [SRCategory(nameof(SR.CatFocus))]
    [SRDescription(nameof(SR.DataGridView_RowValidatingDescr))]
    public event DataGridViewCellCancelEventHandler? RowValidating
    {
        add => Events.AddHandler(s_rowValidatingEvent, value);
        remove => Events.RemoveHandler(s_rowValidatingEvent, value);
    }

    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.DataGridView_ScrollDescr))]
    public event ScrollEventHandler? Scroll
    {
        add => Events.AddHandler(s_scrollEvent, value);
        remove => Events.RemoveHandler(s_scrollEvent, value);
    }

    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.DataGridView_SelectionChangedDescr))]
    public event EventHandler? SelectionChanged
    {
        add => Events.AddHandler(s_selectionChangedEvent, value);
        remove => Events.RemoveHandler(s_selectionChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatData))]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SRDescription(nameof(SR.DataGridView_SortCompareDescr))]
    public event DataGridViewSortCompareEventHandler? SortCompare
    {
        add => Events.AddHandler(s_sortCompareEvent, value);
        remove => Events.RemoveHandler(s_sortCompareEvent, value);
    }

    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.DataGridView_SortedDescr))]
    public event EventHandler? Sorted
    {
        add => Events.AddHandler(s_sortedEvent, value);
        remove => Events.RemoveHandler(s_sortedEvent, value);
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? StyleChanged
    {
        add => base.StyleChanged += value;
        remove => base.StyleChanged -= value;
    }

    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.DataGridView_UserAddedRowDescr))]
    public event DataGridViewRowEventHandler? UserAddedRow
    {
        add => Events.AddHandler(s_userAddedRowEvent, value);
        remove => Events.RemoveHandler(s_userAddedRowEvent, value);
    }

    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.DataGridView_UserDeletedRowDescr))]
    public event DataGridViewRowEventHandler? UserDeletedRow
    {
        add => Events.AddHandler(s_userDeletedRowEvent, value);
        remove => Events.RemoveHandler(s_userDeletedRowEvent, value);
    }

    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.DataGridView_UserDeletingRowDescr))]
    public event DataGridViewRowCancelEventHandler? UserDeletingRow
    {
        add => Events.AddHandler(s_userDeletingRowEvent, value);
        remove => Events.RemoveHandler(s_userDeletingRowEvent, value);
    }

    ////////////////////////
    //                    //
    // ISupportInitialize //
    //                    //
    ////////////////////////
    void ISupportInitialize.BeginInit()
    {
        if (_dataGridViewState2[State2_Initializing])
        {
            throw new InvalidOperationException(SR.DataGridViewBeginInit);
        }

        _dataGridViewState2[State2_Initializing] = true;
    }

    void ISupportInitialize.EndInit()
    {
        _dataGridViewState2[State2_Initializing] = false;

        foreach (DataGridViewColumn dataGridViewColumn in Columns)
        {
            if (dataGridViewColumn.Frozen
                && dataGridViewColumn.Visible
                && dataGridViewColumn.InheritedAutoSizeMode == DataGridViewAutoSizeColumnMode.Fill)
            {
                dataGridViewColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
        }

        DataGridViewSelectionMode selectionMode = SelectionMode;
        if (selectionMode is DataGridViewSelectionMode.FullColumnSelect or DataGridViewSelectionMode.ColumnHeaderSelect)
        {
            foreach (DataGridViewColumn dataGridViewColumn in Columns)
            {
                if (dataGridViewColumn.SortMode == DataGridViewColumnSortMode.Automatic)
                {
                    // Resetting SelectionMode to its acceptable default value. We don't want the control to ever end up in an invalid state.
                    SelectionMode = DefaultSelectionMode; // DataGridViewSelectionMode.RowHeaderSelect
                    throw new InvalidOperationException(
                        string.Format(
                            SR.DataGridView_SelectionModeReset,
                            string.Format(
                                SR.DataGridView_SelectionModeAndSortModeClash,
                                (selectionMode).ToString()),
                                (DefaultSelectionMode).ToString()));
                }
            }
        }
    }
}
