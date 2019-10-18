// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.Layout;
using static Interop;

namespace System.Windows.Forms
{
    [
        ComVisible(true),
        ClassInterface(ClassInterfaceType.AutoDispatch),
        Designer("System.Windows.Forms.Design.DataGridViewDesigner, " + AssemblyRef.SystemDesign),
        //DefaultProperty("DataSource"),
        DefaultEvent(nameof(CellContentClick)),
        ComplexBindingProperties(nameof(DataSource), nameof(DataMember)),
        Docking(DockingBehavior.Ask),
        Editor("System.Windows.Forms.Design.DataGridViewComponentEditor, " + AssemblyRef.SystemDesign, typeof(ComponentEditor)),
        SRDescription(nameof(SR.DescriptionDataGridView))
    ]
    public partial class DataGridView : Control, ISupportInitialize
    {
        private static readonly object EVENT_DATAGRIDVIEWALLOWUSERTOADDROWSCHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWALLOWUSERTODELETEROWSCHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWALLOWUSERTOORDERCOLUMNSCHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWALLOWUSERTORESIZECOLUMNSCHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWALLOWUSERTORESIZEROWSCHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWALTERNATINGROWSDEFAULTCELLSTYLECHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWAUTOGENERATECOLUMNSCHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWAUTOSIZECOLUMNMODECHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWAUTOSIZECOLUMNSMODECHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWAUTOSIZEROWSMODECHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWBACKGROUNDCOLORCHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWBORDERSTYLECHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCANCELROWEDIT = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLBEGINEDIT = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLBORDERSTYLECHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLCLICK = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLCONTENTCLICK = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLCONTENTDOUBLECLICK = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLCONTEXTMENUSTRIPCHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLCONTEXTMENUSTRIPNEEDED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLDOUBLECLICK = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLENDEDIT = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLENTER = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLERRORTEXTCHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLERRORTEXTNEEDED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLFORMATTING = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLLEAVE = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLMOUSECLICK = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLMOUSEDOUBLECLICK = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLMOUSEDOWN = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLMOUSEENTER = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLMOUSELEAVE = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLMOUSEMOVE = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLMOUSEUP = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLPAINTING = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLPARSING = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLSTATECHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLSTYLECHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLSTYLECONTENTCHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLTOOLTIPTEXTCHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLTOOLTIPTEXTNEEDED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLVALIDATING = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLVALIDATED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLVALUECHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLVALUENEEDED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCELLVALUEPUSHED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCOLUMNADDED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCOLUMNCONTEXTMENUSTRIPCHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCOLUMNDATAPROPERTYNAMECHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCOLUMNDEFAULTCELLSTYLECHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCOLUMNDISPLAYINDEXCHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCOLUMNDIVIDERWIDTHCHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCOLUMNHEADERCELLCHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCOLUMNDIVIDERDOUBLECLICK = new object();
        private static readonly object EVENT_DATAGRIDVIEWCOLUMNHEADERMOUSECLICK = new object();
        private static readonly object EVENT_DATAGRIDVIEWCOLUMNHEADERMOUSEDOUBLECLICK = new object();
        private static readonly object EVENT_DATAGRIDVIEWCOLUMNHEADERSBORDERSTYLECHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCOLUMNHEADERSDEFAULTCELLSTYLECHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCOLUMNHEADERSHEIGHTCHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCOLUMNHEADERSHEIGHTSIZEMODECHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCOLUMNMINIMUMWIDTHCHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCOLUMNNAMECHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCOLUMNREMOVED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCOLUMNSORTMODECHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCOLUMNSTATECHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCOLUMNTOOLTIPTEXTCHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCOLUMNWIDTHCHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCURRENTCELLCHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWCURRENTCELLDIRTYSTATECHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWDATABINDINGCOMPLETE = new object();
        private static readonly object EVENT_DATAGRIDVIEWDATAERROR = new object();
        private static readonly object EVENT_DATAGRIDVIEWDATAMEMBERCHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWDATASOURCECHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWDEFAULTCELLSTYLECHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWDEFAULTVALUESNEEDED = new object();
        private static readonly object EVENT_DATAGRIDVIEWEDITINGCONTROLSHOWING = new object();
        private static readonly object EVENT_DATAGRIDVIEWEDITMODECHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWGRIDCOLORCHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWMULTISELECTCHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWNEWROWNEEDED = new object();
        private static readonly object EVENT_DATAGRIDVIEWREADONLYCHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWROWCONTEXTMENUSTRIPCHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWROWCONTEXTMENUSTRIPNEEDED = new object();
        private static readonly object EVENT_DATAGRIDVIEWROWDEFAULTCELLSTYLECHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWROWDIRTYSTATENEEDED = new object();
        private static readonly object EVENT_DATAGRIDVIEWROWDIVIDERHEIGHTCHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWROWENTER = new object();
        private static readonly object EVENT_DATAGRIDVIEWROWERRORTEXTCHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWROWERRORTEXTNEEDED = new object();
        private static readonly object EVENT_DATAGRIDVIEWROWHEADERCELLCHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWROWDIVIDERDOUBLECLICK = new object();
        private static readonly object EVENT_DATAGRIDVIEWROWHEADERMOUSECLICK = new object();
        private static readonly object EVENT_DATAGRIDVIEWROWHEADERMOUSEDOUBLECLICK = new object();
        private static readonly object EVENT_DATAGRIDVIEWROWHEADERSBORDERSTYLECHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWROWHEADERSDEFAULTCELLSTYLECHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWROWHEADERSWIDTHCHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWROWHEADERSWIDTHSIZEMODECHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWROWHEIGHTCHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWROWHEIGHTINFONEEDED = new object();
        private static readonly object EVENT_DATAGRIDVIEWROWHEIGHTINFOPUSHED = new object();
        private static readonly object EVENT_DATAGRIDVIEWROWLEAVE = new object();
        private static readonly object EVENT_DATAGRIDVIEWROWMINIMUMHEIGHTCHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWROWPOSTPAINT = new object();
        private static readonly object EVENT_DATAGRIDVIEWROWPREPAINT = new object();
        private static readonly object EVENT_DATAGRIDVIEWROWSADDED = new object();
        private static readonly object EVENT_DATAGRIDVIEWROWSDEFAULTCELLSTYLECHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWROWSREMOVED = new object();
        private static readonly object EVENT_DATAGRIDVIEWROWSTATECHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWROWUNSHARED = new object();
        private static readonly object EVENT_DATAGRIDVIEWROWVALIDATED = new object();
        private static readonly object EVENT_DATAGRIDVIEWROWVALIDATING = new object();
        private static readonly object EVENT_DATAGRIDVIEWSCROLL = new object();
        private static readonly object EVENT_DATAGRIDVIEWSELECTIONCHANGED = new object();
        private static readonly object EVENT_DATAGRIDVIEWSORTCOMPARE = new object();
        private static readonly object EVENT_DATAGRIDVIEWSORTED = new object();
        private static readonly object EVENT_DATAGRIDVIEWUSERADDEDROW = new object();
        private static readonly object EVENT_DATAGRIDVIEWUSERDELETEDROW = new object();
        private static readonly object EVENT_DATAGRIDVIEWUSERDELETINGROW = new object();

        private const int DATAGRIDVIEWSTATE1_allowUserToAddRows = 0x00000001;
        private const int DATAGRIDVIEWSTATE1_allowUserToDeleteRows = 0x00000002;
        private const int DATAGRIDVIEWSTATE1_allowUserToOrderColumns = 0x00000004;
        private const int DATAGRIDVIEWSTATE1_columnHeadersVisible = 0x00000008;
        private const int DATAGRIDVIEWSTATE1_rowHeadersVisible = 0x00000010;
        private const int DATAGRIDVIEWSTATE1_forwardCharMessage = 0x00000020;
        private const int DATAGRIDVIEWSTATE1_leavingWithTabKey = 0x00000040;
        private const int DATAGRIDVIEWSTATE1_multiSelect = 0x00000080;
        private const int DATAGRIDVIEWSTATE1_ignoringEditingChanges = 0x00000200;
        private const int DATAGRIDVIEWSTATE1_ambientForeColor = 0x00000400;
        private const int DATAGRIDVIEWSTATE1_scrolledSinceMouseDown = 0x00000800;
        private const int DATAGRIDVIEWSTATE1_editingControlHidden = 0x00001000;
        private const int DATAGRIDVIEWSTATE1_standardTab = 0x00002000;
        private const int DATAGRIDVIEWSTATE1_editingControlChanging = 0x00004000;
        private const int DATAGRIDVIEWSTATE1_currentCellInEditMode = 0x00008000;
        private const int DATAGRIDVIEWSTATE1_virtualMode = 0x00010000;
        private const int DATAGRIDVIEWSTATE1_editedCellChanged = 0x00020000;
        private const int DATAGRIDVIEWSTATE1_editedRowChanged = 0x00040000;
        private const int DATAGRIDVIEWSTATE1_newRowEdited = 0x00080000;
        private const int DATAGRIDVIEWSTATE1_readOnly = 0x00100000;
        private const int DATAGRIDVIEWSTATE1_newRowCreatedByEditing = 0x00200000;
        private const int DATAGRIDVIEWSTATE1_temporarilyResetCurrentCell = 0x00400000;
        private const int DATAGRIDVIEWSTATE1_autoGenerateColumns = 0x00800000;
        private const int DATAGRIDVIEWSTATE1_customCursorSet = 0x01000000;
        private const int DATAGRIDVIEWSTATE1_ambientFont = 0x02000000;
        private const int DATAGRIDVIEWSTATE1_ambientColumnHeadersFont = 0x04000000;
        private const int DATAGRIDVIEWSTATE1_ambientRowHeadersFont = 0x08000000;
        private const int DATAGRIDVIEWSTATE1_isAutoSized = 0x40000000;

        // DATAGRIDVIEWSTATE2_
        private const int DATAGRIDVIEWSTATE2_showEditingIcon = 0x00000001;
        private const int DATAGRIDVIEWSTATE2_allowUserToResizeColumns = 0x00000002;
        private const int DATAGRIDVIEWSTATE2_allowUserToResizeRows = 0x00000004;
        private const int DATAGRIDVIEWSTATE2_mouseOverRemovedEditingCtrl = 0x00000008;
        private const int DATAGRIDVIEWSTATE2_mouseOverRemovedEditingPanel = 0x00000010;
        private const int DATAGRIDVIEWSTATE2_mouseEnterExpected = 0x00000020;
        private const int DATAGRIDVIEWSTATE2_enableHeadersVisualStyles = 0x00000040;
        private const int DATAGRIDVIEWSTATE2_showCellErrors = 0x00000080;
        private const int DATAGRIDVIEWSTATE2_showCellToolTips = 0x00000100;
        private const int DATAGRIDVIEWSTATE2_showRowErrors = 0x00000200;
        private const int DATAGRIDVIEWSTATE2_showColumnRelocationInsertion = 0x00000400;
        private const int DATAGRIDVIEWSTATE2_rightToLeftMode = 0x00000800;
        private const int DATAGRIDVIEWSTATE2_rightToLeftValid = 0x00001000;
        private const int DATAGRIDVIEWSTATE2_currentCellWantsInputKey = 0x00002000;
        private const int DATAGRIDVIEWSTATE2_stopRaisingVerticalScroll = 0x00004000;
        private const int DATAGRIDVIEWSTATE2_stopRaisingHorizontalScroll = 0x00008000;
        private const int DATAGRIDVIEWSTATE2_replacedCellSelected = 0x00010000;
        private const int DATAGRIDVIEWSTATE2_replacedCellReadOnly = 0x00020000;
        private const int DATAGRIDVIEWSTATE2_raiseSelectionChanged = 0x00040000;
        private const int DATAGRIDVIEWSTATE2_initializing = 0x00080000;
        private const int DATAGRIDVIEWSTATE2_autoSizedWithoutHandle = 0x00100000;
        private const int DATAGRIDVIEWSTATE2_ignoreCursorChange = 0x00200000;
        private const int DATAGRIDVIEWSTATE2_rowsCollectionClearedInSetCell = 0x00400000;
        private const int DATAGRIDVIEWSTATE2_nextMouseUpIsDouble = 0x00800000;
        private const int DATAGRIDVIEWSTATE2_inBindingContextChanged = 0x01000000;
        private const int DATAGRIDVIEWSTATE2_allowHorizontalScrollbar = 0x02000000;
        private const int DATAGRIDVIEWSTATE2_usedFillWeightsDirty = 0x04000000;
        private const int DATAGRIDVIEWSTATE2_messageFromEditingCtrls = 0x08000000;
        private const int DATAGRIDVIEWSTATE2_cellMouseDownInContentBounds = 0x10000000;
        private const int DATAGRIDVIEWSTATE2_discardEditingControl = 0x20000000;

        // DATAGRIDVIEWOPER_
        private const int DATAGRIDVIEWOPER_trackColResize = 0x00000001;
        private const int DATAGRIDVIEWOPER_trackRowResize = 0x00000002;
        private const int DATAGRIDVIEWOPER_trackColSelect = 0x00000004;
        private const int DATAGRIDVIEWOPER_trackRowSelect = 0x00000008;
        private const int DATAGRIDVIEWOPER_trackCellSelect = 0x00000010;
        private const int DATAGRIDVIEWOPER_trackColRelocation = 0x00000020;
        private const int DATAGRIDVIEWOPER_inSort = 0x00000040;
        private const int DATAGRIDVIEWOPER_trackColHeadersResize = 0x00000080;
        private const int DATAGRIDVIEWOPER_trackRowHeadersResize = 0x00000100;
        private const int DATAGRIDVIEWOPER_trackMouseMoves = 0x00000200;
        private const int DATAGRIDVIEWOPER_inRefreshColumns = 0x00000400;
        private const int DATAGRIDVIEWOPER_inDisplayIndexAdjustments = 0x00000800;
        private const int DATAGRIDVIEWOPER_lastEditCtrlClickDoubled = 0x00001000;
        private const int DATAGRIDVIEWOPER_inMouseDown = 0x00002000;
        private const int DATAGRIDVIEWOPER_inReadOnlyChange = 0x00004000;
        private const int DATAGRIDVIEWOPER_inCellValidating = 0x00008000;
        private const int DATAGRIDVIEWOPER_inBorderStyleChange = 0x00010000;
        private const int DATAGRIDVIEWOPER_inCurrentCellChange = 0x00020000;
        private const int DATAGRIDVIEWOPER_inAdjustFillingColumns = 0x00040000;
        private const int DATAGRIDVIEWOPER_inAdjustFillingColumn = 0x00080000;
        private const int DATAGRIDVIEWOPER_inDispose = 0x00100000;
        private const int DATAGRIDVIEWOPER_inBeginEdit = 0x00200000;
        private const int DATAGRIDVIEWOPER_inEndEdit = 0x00400000;
        private const int DATAGRIDVIEWOPER_resizingOperationAboutToStart = 0x00800000;
        private const int DATAGRIDVIEWOPER_trackKeyboardColResize = 0x01000000;
        private const int DATAGRIDVIEWOPER_mouseOperationMask = DATAGRIDVIEWOPER_trackColResize | DATAGRIDVIEWOPER_trackRowResize |
            DATAGRIDVIEWOPER_trackColRelocation | DATAGRIDVIEWOPER_trackColHeadersResize | DATAGRIDVIEWOPER_trackRowHeadersResize;
        private const int DATAGRIDVIEWOPER_keyboardOperationMask = DATAGRIDVIEWOPER_trackKeyboardColResize;

        private static Size DragSize = SystemInformation.DragSize;

        private const byte DATAGRIDVIEW_columnSizingHotZone = 6;
        private const byte DATAGRIDVIEW_rowSizingHotZone = 5;
        private const byte DATAGRIDVIEW_insertionBarWidth = 3;
        private const byte DATAGRIDVIEW_bulkPaintThreshold = 8;

        private const string DATAGRIDVIEW_htmlPrefix = "Version:1.0\r\nStartHTML:00000097\r\nEndHTML:{0}\r\nStartFragment:00000133\r\nEndFragment:{1}\r\n";
        private const string DATAGRIDVIEW_htmlStartFragment = "<HTML>\r\n<BODY>\r\n<!--StartFragment-->";
        private const string DATAGRIDVIEW_htmlEndFragment = "\r\n<!--EndFragment-->\r\n</BODY>\r\n</HTML>";

        private const int FOCUS_RECT_OFFSET = 2;

        private Collections.Specialized.BitVector32 dataGridViewState1;  // see DATAGRIDVIEWSTATE1_ consts above
        private Collections.Specialized.BitVector32 dataGridViewState2;  // see DATAGRIDVIEWSTATE2_ consts above
        private Collections.Specialized.BitVector32 dataGridViewOper;   // see DATAGRIDVIEWOPER_ consts above

        private const BorderStyle defaultBorderStyle = BorderStyle.FixedSingle;
        private const DataGridViewAdvancedCellBorderStyle defaultAdvancedCellBorderStyle = DataGridViewAdvancedCellBorderStyle.Single;
        private const DataGridViewAdvancedCellBorderStyle defaultAdvancedRowHeadersBorderStyle = DataGridViewAdvancedCellBorderStyle.OutsetPartial;
        private const DataGridViewAdvancedCellBorderStyle defaultAdvancedColumnHeadersBorderStyle = DataGridViewAdvancedCellBorderStyle.OutsetPartial;

        private const DataGridViewSelectionMode defaultSelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
        private const DataGridViewEditMode defaultEditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;

        private const DataGridViewAutoSizeRowCriteriaInternal invalidDataGridViewAutoSizeRowCriteriaInternalMask = ~(DataGridViewAutoSizeRowCriteriaInternal.Header | DataGridViewAutoSizeRowCriteriaInternal.AllColumns);

        private SolidBrush backgroundBrush = DefaultBackgroundBrush;
        private Pen gridPen;
        private Cursor oldCursor;

        private HScrollBar horizScrollBar = new HScrollBar();
        private VScrollBar vertScrollBar = new VScrollBar();
        private DataGridViewHeaderCell topLeftHeaderCell;

        private DataGridViewRow rowTemplate;
        private DataGridViewRowCollection dataGridViewRows;
        private DataGridViewColumnCollection dataGridViewColumns;

        private DataGridViewCellStyle placeholderCellStyle;
        private StringFormat placeholderStringFormat;

        private DataGridViewColumn sortedColumn;
        private SortOrder sortOrder;

        private object uneditedFormattedValue;
        private Control editingControl, latestEditingControl, cachedEditingControl;
        private Panel editingPanel;
        private DataGridViewEditingPanelAccessibleObject editingPanelAccessibleObject;
        private Point ptCurrentCell, ptCurrentCellCache = Point.Empty, ptAnchorCell, ptMouseDownCell, ptMouseEnteredCell, ptToolTipCell, ptMouseDownGridCoord;

        private DataGridViewSelectionMode selectionMode;
        private DataGridViewEditMode editMode;

        // Note that a cell can only be in one bag but not both at the same time.
        private readonly DataGridViewCellLinkedList individualSelectedCells;
        private readonly DataGridViewCellLinkedList individualReadOnlyCells;
        private readonly DataGridViewIntLinkedList selectedBandIndexes;
        private DataGridViewIntLinkedList selectedBandSnapshotIndexes;

        private DataGridViewCellStyle defaultCellStyle, columnHeadersDefaultCellStyle, rowHeadersDefaultCellStyle;
        private DataGridViewCellStyle rowsDefaultCellStyle, alternatingRowsDefaultCellStyle;
        private ScrollBars scrollBars;
        private LayoutData layout;
        private readonly DisplayedBandsData displayedBandsInfo;
        private Rectangle normalClientRectangle;
        private readonly ArrayList lstRows;
        private int availableWidthForFillColumns;

        private BorderStyle borderStyle;
        private readonly DataGridViewAdvancedBorderStyle advancedCellBorderStyle;
        private readonly DataGridViewAdvancedBorderStyle advancedRowHeadersBorderStyle;
        private readonly DataGridViewAdvancedBorderStyle advancedColumnHeadersBorderStyle;

        private DataGridViewClipboardCopyMode clipboardCopyMode;

        private const int minimumRowHeadersWidth = 4;
        private const int minimumColumnHeadersHeight = 4;
        private const int defaultRowHeadersWidth = 41;
        private const int maxHeadersThickness = 32768;
        private const int upperSize = 0x007FFFFF;
        private int rowHeadersWidth = defaultRowHeadersWidth;
        private int cachedRowHeadersWidth;
        private const int defaultColumnHeadersHeight = 23;
        private int columnHeadersHeight = defaultColumnHeadersHeight;
        private int cachedColumnHeadersHeight;
        private DataGridViewAutoSizeRowsMode autoSizeRowsMode;
        private DataGridViewAutoSizeColumnsMode autoSizeColumnsMode;
        private DataGridViewColumnHeadersHeightSizeMode columnHeadersHeightSizeMode;
        private DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode;

        private DataGridViewCellStyleChangedEventArgs dgvcsce;
        private DataGridViewCellPaintingEventArgs dgvcpe;
        private DataGridViewCellValueEventArgs dgvcve;
        private DataGridViewRowHeightInfoNeededEventArgs dgvrhine;
        private DataGridViewRowPostPaintEventArgs dgvrpope;
        private DataGridViewRowPrePaintEventArgs dgvrprpe;

        // the sum of the widths in pixels of the scrolling columns preceding
        // the first visible scrolling column
        private int horizontalOffset;

        // the sum of the heights in pixels of the scrolling rows preceding
        // the first visible scrolling row
        private int verticalOffset;

        // the number of pixels of the firstDisplayedScrollingCol which are not visible
        private int negOffset;

        // the index of the potential 'new' row. -1 if there is no 'new' row.
        private int newRowIndex = -1;

        // residual fraction of WHEEL_DELTA (120) for wheel scrolling
        private int cumulativeVerticalWheelDelta;
        private int cumulativeHorizontalWheelDelta;

        private int trackColAnchor;
        private int trackColumn = -1;
        private int trackColumnEdge = -1;
        private int trackRowAnchor;
        private int trackRow = -1;
        private int trackRowEdge = -1;
        private int lastHeaderShadow = -1;
        private int currentColSplitBar = -1, lastColSplitBar = -1;
        private int currentRowSplitBar = -1, lastRowSplitBar = -1;
        private int mouseBarOffset;
        private int noDimensionChangeCount;
        private int noSelectionChangeCount;
        private int noAutoSizeCount;
        private int inBulkPaintCount;
        private int inBulkLayoutCount;
        private int inPerformLayoutCount;

        private int keyboardResizeStep;
        private Rectangle resizeClipRectangle;

        private Timer vertScrollTimer, horizScrollTimer;

        private readonly Hashtable converters;
        private Hashtable pens;
        private Hashtable brushes;

        private RECT[] cachedScrollableRegion;

        // DataBinding
        private DataGridViewDataConnection dataConnection;

        // ToolTip
        private readonly DataGridViewToolTip toolTipControl;
        private static readonly int PropToolTip = PropertyStore.CreateKey();
        // the tool tip string we get from cells
        private string toolTipCaption = string.Empty;

        private const int maxTTDISPINFOBufferLength = 80;

        // Last Mouse Click Info
        private MouseClickInfo lastMouseClickInfo;

#if DEBUG
        // set to false when the grid is not in sync with the underlying data store
        // in virtual mode, and OnCellValueNeeded cannot be called.
        // disable csharp compiler warning #0414: field assigned unused value
#pragma warning disable 0414
        internal bool dataStoreAccessAllowed = true;
#pragma warning restore 0414
#endif
        /// <summary>
        ///  Initializes a new instance of the <see cref='DataGridView'/> class.
        /// </summary>
        public DataGridView()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.Opaque |
                     ControlStyles.UserMouse, true);

            SetStyle(ControlStyles.SupportsTransparentBackColor, false);

            // this class overrides GetPreferredSizeCore, let Control automatically cache the result
            SetState2(STATE2_USEPREFERREDSIZECACHE, true);

            dataGridViewState1 = new Collections.Specialized.BitVector32(0x00000000);
            dataGridViewState2 = new Collections.Specialized.BitVector32(0x00000000);
            dataGridViewOper = new Collections.Specialized.BitVector32(0x00000000);

            dataGridViewState1[DATAGRIDVIEWSTATE1_columnHeadersVisible
                                    | DATAGRIDVIEWSTATE1_rowHeadersVisible
                                    | DATAGRIDVIEWSTATE1_autoGenerateColumns
                                    | DATAGRIDVIEWSTATE1_allowUserToAddRows
                                    | DATAGRIDVIEWSTATE1_allowUserToDeleteRows] = true;

            dataGridViewState2[DATAGRIDVIEWSTATE2_showEditingIcon
                                    | DATAGRIDVIEWSTATE2_enableHeadersVisualStyles
                                    | DATAGRIDVIEWSTATE2_mouseEnterExpected
                                    | DATAGRIDVIEWSTATE2_allowUserToResizeColumns
                                    | DATAGRIDVIEWSTATE2_allowUserToResizeRows
                                    | DATAGRIDVIEWSTATE2_showCellToolTips
                                    | DATAGRIDVIEWSTATE2_showCellErrors
                                    | DATAGRIDVIEWSTATE2_showRowErrors
                                    | DATAGRIDVIEWSTATE2_allowHorizontalScrollbar
                                    | DATAGRIDVIEWSTATE2_usedFillWeightsDirty] = true;

            displayedBandsInfo = new DisplayedBandsData();
            lstRows = new ArrayList();

            converters = new Hashtable(8);
            pens = new Hashtable(8);
            brushes = new Hashtable(10);
            gridPen = new Pen(DefaultGridColor);

            selectedBandIndexes = new DataGridViewIntLinkedList();
            individualSelectedCells = new DataGridViewCellLinkedList();
            individualReadOnlyCells = new DataGridViewCellLinkedList();

            advancedCellBorderStyle = new DataGridViewAdvancedBorderStyle(this,
                DataGridViewAdvancedCellBorderStyle.OutsetDouble,
                DataGridViewAdvancedCellBorderStyle.OutsetPartial,
                DataGridViewAdvancedCellBorderStyle.InsetDouble);
            advancedRowHeadersBorderStyle = new DataGridViewAdvancedBorderStyle(this);
            advancedColumnHeadersBorderStyle = new DataGridViewAdvancedBorderStyle(this);
            advancedCellBorderStyle.All = defaultAdvancedCellBorderStyle;
            advancedRowHeadersBorderStyle.All = defaultAdvancedRowHeadersBorderStyle;
            advancedColumnHeadersBorderStyle.All = defaultAdvancedColumnHeadersBorderStyle;
            borderStyle = defaultBorderStyle;
            dataGridViewState1[DATAGRIDVIEWSTATE1_multiSelect] = true;
            selectionMode = defaultSelectionMode;
            editMode = defaultEditMode;
            autoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            autoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            columnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            rowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;

            clipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithAutoHeaderText;

            layout = new LayoutData
            {
                TopLeftHeader = Rectangle.Empty,
                ColumnHeaders = Rectangle.Empty,
                RowHeaders = Rectangle.Empty,
                ColumnHeadersVisible = true,
                RowHeadersVisible = true,
                ClientRectangle = ClientRectangle
            };

            scrollBars = ScrollBars.Both;

            horizScrollBar.RightToLeft = RightToLeft.Inherit;
            horizScrollBar.AccessibleName = SR.DataGridView_AccHorizontalScrollBarAccName;
            horizScrollBar.Top = ClientRectangle.Height - horizScrollBar.Height;
            horizScrollBar.Left = 0;
            horizScrollBar.Visible = false;
            horizScrollBar.Scroll += new ScrollEventHandler(DataGridViewHScrolled);
            Controls.Add(horizScrollBar);

            vertScrollBar.Top = 0;
            vertScrollBar.AccessibleName = SR.DataGridView_AccVerticalScrollBarAccName;
            vertScrollBar.Left = ClientRectangle.Width - vertScrollBar.Width;
            vertScrollBar.Visible = false;
            vertScrollBar.Scroll += new ScrollEventHandler(DataGridViewVScrolled);
            Controls.Add(vertScrollBar);

            ptCurrentCell = new Point(-1, -1);
            ptAnchorCell = new Point(-1, -1);
            ptMouseDownCell = new Point(-2, -2);
            ptMouseEnteredCell = new Point(-2, -2);
            ptToolTipCell = new Point(-1, -1);
            ptMouseDownGridCoord = new Point(-1, -1);

            sortOrder = SortOrder.None;

            lastMouseClickInfo.timeStamp = 0;

            WireScrollBarsEvents();
            PerformLayout();

            toolTipControl = new DataGridViewToolTip(this);
            rowHeadersWidth = ScaleToCurrentDpi(defaultRowHeadersWidth);
            columnHeadersHeight = ScaleToCurrentDpi(defaultColumnHeadersHeight);
            Invalidate();
        }

        /// <summary>
        ///  Scaling row header width and column header height.
        /// </summary>
        private int ScaleToCurrentDpi(int value)
        {
            return DpiHelper.IsScalingRequirementMet ? LogicalToDeviceUnits(value) : value;
        }

        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
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
                            if ((!RightToLeftInternal && AdvancedColumnHeadersBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.None) ||
                                (RightToLeftInternal && AdvancedColumnHeadersBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.None))
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
                            if ((!RightToLeftInternal && AdvancedColumnHeadersBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.None) ||
                                (RightToLeftInternal && AdvancedColumnHeadersBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.None))
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

        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public DataGridViewAdvancedBorderStyle AdvancedCellBorderStyle
        {
            get
            {
                return advancedCellBorderStyle;
            }
        }

        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public DataGridViewAdvancedBorderStyle AdvancedColumnHeadersBorderStyle
        {
            get
            {
                return advancedColumnHeadersBorderStyle;
            }
        }

        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public DataGridViewAdvancedBorderStyle AdvancedRowHeadersBorderStyle
        {
            get
            {
                return advancedRowHeadersBorderStyle;
            }
        }

        [
            DefaultValue(true),
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_AllowUserToAddRowsDescr))
        ]
        public bool AllowUserToAddRows
        {
            get
            {
                return dataGridViewState1[DATAGRIDVIEWSTATE1_allowUserToAddRows];
            }
            set
            {
                if (AllowUserToAddRows != value)
                {
                    dataGridViewState1[DATAGRIDVIEWSTATE1_allowUserToAddRows] = value;
                    if (DataSource != null)
                    {
                        dataConnection.ResetCachedAllowUserToAddRowsInternal();
                    }
                    OnAllowUserToAddRowsChanged(EventArgs.Empty);
                }
            }
        }

        internal bool AllowUserToAddRowsInternal
        {
            get
            {
                if (DataSource == null)
                {
                    return AllowUserToAddRows;
                }
                else
                {
                    return AllowUserToAddRows && dataConnection.AllowAdd;
                }
            }
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridViewOnAllowUserToAddRowsChangedDescr))
        ]
        public event EventHandler AllowUserToAddRowsChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWALLOWUSERTOADDROWSCHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWALLOWUSERTOADDROWSCHANGED, value);
        }

        [
            DefaultValue(true),
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_AllowUserToDeleteRowsDescr))
        ]
        public bool AllowUserToDeleteRows
        {
            get
            {
                return dataGridViewState1[DATAGRIDVIEWSTATE1_allowUserToDeleteRows];
            }
            set
            {
                if (AllowUserToDeleteRows != value)
                {
                    dataGridViewState1[DATAGRIDVIEWSTATE1_allowUserToDeleteRows] = value;
                    OnAllowUserToDeleteRowsChanged(EventArgs.Empty);
                }
            }
        }

        internal bool AllowUserToDeleteRowsInternal
        {
            get
            {
                if (DataSource == null)
                {
                    return AllowUserToDeleteRows;
                }
                else
                {
                    return AllowUserToDeleteRows && dataConnection.AllowRemove;
                }
            }
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridViewOnAllowUserToDeleteRowsChangedDescr))
        ]
        public event EventHandler AllowUserToDeleteRowsChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWALLOWUSERTODELETEROWSCHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWALLOWUSERTODELETEROWSCHANGED, value);
        }

        [
            DefaultValue(false),
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_AllowUserToOrderColumnsDescr))
        ]
        public bool AllowUserToOrderColumns
        {
            get
            {
                return dataGridViewState1[DATAGRIDVIEWSTATE1_allowUserToOrderColumns];
            }
            set
            {
                if (AllowUserToOrderColumns != value)
                {
                    dataGridViewState1[DATAGRIDVIEWSTATE1_allowUserToOrderColumns] = value;
                    OnAllowUserToOrderColumnsChanged(EventArgs.Empty);
                }
            }
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridViewOnAllowUserToOrderColumnsChangedDescr))
        ]
        public event EventHandler AllowUserToOrderColumnsChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWALLOWUSERTOORDERCOLUMNSCHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWALLOWUSERTOORDERCOLUMNSCHANGED, value);
        }

        /// <summary>
        ///  Gets or sets a global value indicating if the dataGridView's columns are resizable with the mouse.
        ///  The resizable aspect of a column can be overridden by DataGridViewColumn.Resizable.
        /// </summary>
        [
            DefaultValue(true),
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_AllowUserToResizeColumnsDescr))
        ]
        public bool AllowUserToResizeColumns
        {
            get
            {
                return dataGridViewState2[DATAGRIDVIEWSTATE2_allowUserToResizeColumns];
            }
            set
            {
                if (AllowUserToResizeColumns != value)
                {
                    dataGridViewState2[DATAGRIDVIEWSTATE2_allowUserToResizeColumns] = value;
                    OnAllowUserToResizeColumnsChanged(EventArgs.Empty);
                }
            }
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridViewOnAllowUserToResizeColumnsChangedDescr))
        ]
        public event EventHandler AllowUserToResizeColumnsChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWALLOWUSERTORESIZECOLUMNSCHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWALLOWUSERTORESIZECOLUMNSCHANGED, value);
        }

        /// <summary>
        ///  Gets or sets a global value indicating if the dataGridView's rows are resizable with the mouse.
        ///  The resizable aspect of a row can be overridden by DataGridViewRow.Resizable.
        /// </summary>
        [
            DefaultValue(true),
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_AllowUserToResizeRowsDescr))
        ]
        public bool AllowUserToResizeRows
        {
            get
            {
                return dataGridViewState2[DATAGRIDVIEWSTATE2_allowUserToResizeRows];
            }
            set
            {
                if (AllowUserToResizeRows != value)
                {
                    dataGridViewState2[DATAGRIDVIEWSTATE2_allowUserToResizeRows] = value;
                    OnAllowUserToResizeRowsChanged(EventArgs.Empty);
                }
            }
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridViewOnAllowUserToResizeRowsChangedDescr))
        ]
        public event EventHandler AllowUserToResizeRowsChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWALLOWUSERTORESIZEROWSCHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWALLOWUSERTORESIZEROWSCHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_AlternatingRowsDefaultCellStyleDescr))
        ]
        public DataGridViewCellStyle AlternatingRowsDefaultCellStyle
        {
            get
            {
                if (alternatingRowsDefaultCellStyle == null)
                {
                    alternatingRowsDefaultCellStyle = new DataGridViewCellStyle();
                    alternatingRowsDefaultCellStyle.AddScope(this, DataGridViewCellStyleScopes.AlternatingRows);
                }
                return alternatingRowsDefaultCellStyle;
            }
            set
            {
                DataGridViewCellStyle cs = AlternatingRowsDefaultCellStyle;
                cs.RemoveScope(DataGridViewCellStyleScopes.AlternatingRows);
                alternatingRowsDefaultCellStyle = value;
                if (value != null)
                {
                    alternatingRowsDefaultCellStyle.AddScope(this, DataGridViewCellStyleScopes.AlternatingRows);
                }
                DataGridViewCellStyleDifferences dgvcsc = cs.GetDifferencesFrom(AlternatingRowsDefaultCellStyle);
                if (dgvcsc != DataGridViewCellStyleDifferences.None)
                {
                    CellStyleChangedEventArgs.ChangeAffectsPreferredSize = (dgvcsc == DataGridViewCellStyleDifferences.AffectPreferredSize);
                    OnAlternatingRowsDefaultCellStyleChanged(CellStyleChangedEventArgs);
                }
            }
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridViewAlternatingRowsDefaultCellStyleChangedDescr))
        ]
        public event EventHandler AlternatingRowsDefaultCellStyleChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWALTERNATINGROWSDEFAULTCELLSTYLECHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWALTERNATINGROWSDEFAULTCELLSTYLECHANGED, value);
        }

        internal bool ApplyVisualStylesToInnerCells
        {
            get
            {
                return Application.RenderWithVisualStyles;
            }
        }

        internal bool ApplyVisualStylesToHeaderCells
        {
            get
            {
                return Application.RenderWithVisualStyles && EnableHeadersVisualStyles;
            }
        }

        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced),
            DefaultValue(true)
        ]
        public bool AutoGenerateColumns
        {
            get
            {
                return dataGridViewState1[DATAGRIDVIEWSTATE1_autoGenerateColumns];
            }
            set
            {
                if (dataGridViewState1[DATAGRIDVIEWSTATE1_autoGenerateColumns] != value)
                {
                    dataGridViewState1[DATAGRIDVIEWSTATE1_autoGenerateColumns] = value;
                    OnAutoGenerateColumnsChanged(EventArgs.Empty);
                }
            }
        }

        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public event EventHandler AutoGenerateColumnsChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWAUTOGENERATECOLUMNSCHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWAUTOGENERATECOLUMNSCHANGED, value);
        }

        /// <summary>
        ///  Overriding base implementation for perf gains.
        /// </summary>
        public override bool AutoSize
        {
            get
            {
                return dataGridViewState1[DATAGRIDVIEWSTATE1_isAutoSized];
            }
            set
            {
                base.AutoSize = value;
                dataGridViewState1[DATAGRIDVIEWSTATE1_isAutoSized] = value;
            }
        }

        /// <summary>
        ///  Gets or sets the columns' autosizing mode. Standard inheritance model is used:
        ///  Columns with AutoSizeMode property set to NotSet will use this auto size mode.
        /// </summary>
        [
            DefaultValue(DataGridViewAutoSizeColumnsMode.None),
            SRCategory(nameof(SR.CatLayout)),
            SRDescription(nameof(SR.DataGridView_AutoSizeColumnsModeDescr))
        ]
        public DataGridViewAutoSizeColumnsMode AutoSizeColumnsMode
        {
            get
            {
                return autoSizeColumnsMode;
            }

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

                if (autoSizeColumnsMode != value)
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
                        /*DataGridViewAutoSizeColumnMode previousInheritedMode = dataGridViewColumn.InheritedAutoSizeMode;
                        bool previousInheritedModeAutoSized = previousInheritedMode != DataGridViewAutoSizeColumnMode.Fill &&
                                                              previousInheritedMode != DataGridViewAutoSizeColumnMode.None &&
                                                              previousInheritedMode != DataGridViewAutoSizeColumnMode.NotSet;*/
                        previousModes[dataGridViewColumn.Index] = dataGridViewColumn.InheritedAutoSizeMode;
                    }
                    DataGridViewAutoSizeColumnsModeEventArgs dgvcasme = new DataGridViewAutoSizeColumnsModeEventArgs(previousModes);
                    autoSizeColumnsMode = value;
                    OnAutoSizeColumnsModeChanged(dgvcasme);
                }
            }
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridViewAutoSizeColumnsModeChangedDescr))
        ]
        public event DataGridViewAutoSizeColumnsModeEventHandler AutoSizeColumnsModeChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWAUTOSIZECOLUMNSMODECHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWAUTOSIZECOLUMNSMODECHANGED, value);
        }

        /// <summary>
        ///  Gets or sets the rows' autosizing mode.
        /// </summary>
        [
            DefaultValue(DataGridViewAutoSizeRowsMode.None),
            SRCategory(nameof(SR.CatLayout)),
            SRDescription(nameof(SR.DataGridView_AutoSizeRowsModeDescr))
        ]
        public DataGridViewAutoSizeRowsMode AutoSizeRowsMode
        {
            get
            {
                return autoSizeRowsMode;
            }
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
                if ((value == DataGridViewAutoSizeRowsMode.AllHeaders || value == DataGridViewAutoSizeRowsMode.DisplayedHeaders) &&
                    !RowHeadersVisible)
                {
                    throw new InvalidOperationException(SR.DataGridView_CannotAutoSizeRowsInvisibleRowHeader);
                }
                if (autoSizeRowsMode != value)
                {
                    DataGridViewAutoSizeModeEventArgs dgvasme = new DataGridViewAutoSizeModeEventArgs(autoSizeRowsMode != DataGridViewAutoSizeRowsMode.None);
                    autoSizeRowsMode = value;
                    OnAutoSizeRowsModeChanged(dgvasme);
                }
            }
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridViewAutoSizeRowsModeChangedDescr))
        ]
        public event DataGridViewAutoSizeModeEventHandler AutoSizeRowsModeChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWAUTOSIZEROWSMODECHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWAUTOSIZEROWSMODECHANGED, value);
        }

        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Never),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }

        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Never)
        ]
        new public event EventHandler BackColorChanged
        {
            add => base.BackColorChanged += value;
            remove => base.BackColorChanged -= value;
        }

        internal SolidBrush BackgroundBrush
        {
            get
            {
                return backgroundBrush;
            }
        }

        /// <summary>
        ///  Gets or sets the background color of the dataGridView.
        /// </summary>
        [
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridViewBackgroundColorDescr))
        ]
        public Color BackgroundColor
        {
            get
            {
                return backgroundBrush.Color;
            }
            set
            {
                if (value.IsEmpty)
                {
                    throw new ArgumentException(string.Format(SR.DataGridView_EmptyColor, "BackgroundColor"));
                }
                if (value.A < 255)
                {
                    throw new ArgumentException(string.Format(SR.DataGridView_TransparentColor, "BackgroundColor"));
                }
                if (!value.Equals(backgroundBrush.Color))
                {
                    backgroundBrush = new SolidBrush(value);
                    OnBackgroundColorChanged(EventArgs.Empty);
                }
            }
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridViewBackgroundColorChangedDescr))
        ]
        public event EventHandler BackgroundColorChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWBACKGROUNDCOLORCHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWBACKGROUNDCOLORCHANGED, value);
        }

        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Never)
        ]
        public override Image BackgroundImage
        {
            get
            {
                return base.BackgroundImage;
            }
            set
            {
                base.BackgroundImage = value;
            }
        }

        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Never)
        ]
        public override ImageLayout BackgroundImageLayout
        {
            get
            {
                return base.BackgroundImageLayout;
            }
            set
            {
                base.BackgroundImageLayout = value;
            }
        }

        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Never)
        ]
        new public event EventHandler BackgroundImageChanged
        {
            add => base.BackgroundImageChanged += value;
            remove => base.BackgroundImageChanged -= value;
        }

        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Never)
        ]
        new public event EventHandler BackgroundImageLayoutChanged
        {
            add => base.BackgroundImageLayoutChanged += value;
            remove => base.BackgroundImageLayoutChanged -= value;
        }

        private bool ShouldSerializeBackgroundColor()
        {
            return !BackgroundColor.Equals(DefaultBackgroundBrush.Color);
        }

        [
            DefaultValue(BorderStyle.FixedSingle),
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_BorderStyleDescr))
        ]
        public BorderStyle BorderStyle
        {
            get
            {
                return borderStyle;
            }
            set
            {
                // Sequential enum.  Valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)BorderStyle.None, (int)BorderStyle.Fixed3D))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(BorderStyle));
                }
                if (borderStyle != value)
                {
                    using (LayoutTransaction.CreateTransactionIf(AutoSize, ParentInternal, this, PropertyNames.BorderStyle))
                    {
                        borderStyle = value;
                        if (!AutoSize)
                        {
                            PerformLayoutPrivate(false /*useRowShortcut*/, false /*computeVisibleRows*/, true /*invalidInAdjustFillingColumns*/, true /*repositionEditingControl*/);
                        }
                        Invalidate();
                        OnBorderStyleChanged(EventArgs.Empty);
                    }
                }
            }
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridViewBorderStyleChangedDescr))
        ]
        public event EventHandler BorderStyleChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWBORDERSTYLECHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWBORDERSTYLECHANGED, value);
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

                Debug.WriteLineIf(CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside get_CanEnableIme(), this = " + this);
                Debug.Indent();

                if (ptCurrentCell.X != -1 /*&& !this.IsCurrentCellInEditMode*/ && ColumnEditable(ptCurrentCell.X))
                {
                    DataGridViewCell dataGridViewCell = CurrentCellInternal;
                    Debug.Assert(dataGridViewCell != null);

                    if (!IsSharedCellReadOnly(dataGridViewCell, ptCurrentCell.Y))
                    {
                        canEnable = base.CanEnableIme;
                    }
                }

                Debug.WriteLineIf(CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Value = " + canEnable);
                Debug.Unindent();

                return canEnable;
            }
        }

        [
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_CellBorderStyleDescr)),
            Browsable(true),
            DefaultValue(DataGridViewCellBorderStyle.Single)
        ]
        public DataGridViewCellBorderStyle CellBorderStyle
        {
            get
            {
                switch (advancedCellBorderStyle.All)
                {
                    case DataGridViewAdvancedCellBorderStyle.NotSet:
                        if (advancedCellBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.None &&
                            advancedCellBorderStyle.Bottom == DataGridViewAdvancedCellBorderStyle.None)
                        {
                            if (RightToLeftInternal)
                            {
                                if (advancedCellBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.None &&
                                    advancedCellBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.Single)
                                {
                                    return DataGridViewCellBorderStyle.SingleVertical;
                                }
                            }
                            else
                            {
                                if (advancedCellBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.None &&
                                    advancedCellBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.Single)
                                {
                                    return DataGridViewCellBorderStyle.SingleVertical;
                                }
                            }
                            if (advancedCellBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.Outset &&
                                advancedCellBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.Outset)
                            {
                                return DataGridViewCellBorderStyle.RaisedVertical;
                            }
                            if (advancedCellBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.Inset &&
                                advancedCellBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.Inset)
                            {
                                return DataGridViewCellBorderStyle.SunkenVertical;
                            }
                        }
                        if (advancedCellBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.None &&
                            advancedCellBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.None)
                        {
                            if (advancedCellBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.None &&
                                advancedCellBorderStyle.Bottom == DataGridViewAdvancedCellBorderStyle.Single)
                            {
                                return DataGridViewCellBorderStyle.SingleHorizontal;
                            }
                            if (advancedCellBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.Outset &&
                                advancedCellBorderStyle.Bottom == DataGridViewAdvancedCellBorderStyle.Outset)
                            {
                                return DataGridViewCellBorderStyle.RaisedHorizontal;
                            }
                            if (advancedCellBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.Inset &&
                                advancedCellBorderStyle.Bottom == DataGridViewAdvancedCellBorderStyle.Inset)
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
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DataGridViewCellBorderStyle.Custom, (int)DataGridViewCellBorderStyle.SunkenHorizontal))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridViewCellBorderStyle));
                }

                if (value != CellBorderStyle)
                {
                    if (value == DataGridViewCellBorderStyle.Custom)
                    {
                        throw new ArgumentException(string.Format(SR.DataGridView_CustomCellBorderStyleInvalid, "CellBorderStyle"));
                    }
                    dataGridViewOper[DATAGRIDVIEWOPER_inBorderStyleChange] = true;
                    try
                    {
                        switch (value)
                        {
                            case DataGridViewCellBorderStyle.Single:
                                advancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Single;
                                break;

                            case DataGridViewCellBorderStyle.Raised:
                                advancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Outset;
                                break;

                            case DataGridViewCellBorderStyle.Sunken:
                                advancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Inset;
                                break;

                            case DataGridViewCellBorderStyle.None:
                                advancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                                break;

                            case DataGridViewCellBorderStyle.SingleVertical:
                                advancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                                if (RightToLeftInternal)
                                {
                                    advancedCellBorderStyle.LeftInternal = DataGridViewAdvancedCellBorderStyle.Single;
                                }
                                else
                                {
                                    advancedCellBorderStyle.RightInternal = DataGridViewAdvancedCellBorderStyle.Single;
                                }
                                break;

                            case DataGridViewCellBorderStyle.RaisedVertical:
                                advancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                                advancedCellBorderStyle.RightInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                                advancedCellBorderStyle.LeftInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                                break;

                            case DataGridViewCellBorderStyle.SunkenVertical:
                                advancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                                advancedCellBorderStyle.RightInternal = DataGridViewAdvancedCellBorderStyle.Inset;
                                advancedCellBorderStyle.LeftInternal = DataGridViewAdvancedCellBorderStyle.Inset;
                                break;

                            case DataGridViewCellBorderStyle.SingleHorizontal:
                                advancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                                advancedCellBorderStyle.BottomInternal = DataGridViewAdvancedCellBorderStyle.Single;
                                break;

                            case DataGridViewCellBorderStyle.RaisedHorizontal:
                                advancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                                advancedCellBorderStyle.TopInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                                advancedCellBorderStyle.BottomInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                                break;

                            case DataGridViewCellBorderStyle.SunkenHorizontal:
                                advancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                                advancedCellBorderStyle.TopInternal = DataGridViewAdvancedCellBorderStyle.Inset;
                                advancedCellBorderStyle.BottomInternal = DataGridViewAdvancedCellBorderStyle.Inset;
                                break;
                        }
                    }
                    finally
                    {
                        dataGridViewOper[DATAGRIDVIEWOPER_inBorderStyleChange] = false;
                    }
                    OnCellBorderStyleChanged(EventArgs.Empty);
                }
            }
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_CellBorderStyleChangedDescr))
        ]
        public event EventHandler CellBorderStyleChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLBORDERSTYLECHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLBORDERSTYLECHANGED, value);
        }

        internal bool CellMouseDownInContentBounds
        {
            get
            {
                return dataGridViewState2[DATAGRIDVIEWSTATE2_cellMouseDownInContentBounds];
            }
            set
            {
                dataGridViewState2[DATAGRIDVIEWSTATE2_cellMouseDownInContentBounds] = value;
            }
        }

        internal DataGridViewCellPaintingEventArgs CellPaintingEventArgs
        {
            get
            {
                if (dgvcpe == null)
                {
                    dgvcpe = new DataGridViewCellPaintingEventArgs(this);
                }
                return dgvcpe;
            }
        }

        private DataGridViewCellStyleChangedEventArgs CellStyleChangedEventArgs
        {
            get
            {
                if (dgvcsce == null)
                {
                    dgvcsce = new DataGridViewCellStyleChangedEventArgs();
                }
                return dgvcsce;
            }
        }

        internal DataGridViewCellValueEventArgs CellValueEventArgs
        {
            get
            {
                if (dgvcve == null)
                {
                    dgvcve = new DataGridViewCellValueEventArgs();
                }
                return dgvcve;
            }
        }

        [
            Browsable(true),
            DefaultValue(DataGridViewClipboardCopyMode.EnableWithAutoHeaderText),
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_ClipboardCopyModeDescr))
        ]
        public DataGridViewClipboardCopyMode ClipboardCopyMode
        {
            get
            {
                return clipboardCopyMode;
            }
            set
            {
                // Sequential enum.  Valid values are 0x0 to 0x3
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DataGridViewClipboardCopyMode.Disable, (int)DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridViewClipboardCopyMode));
                }
                clipboardCopyMode = value;
            }
        }

        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            DefaultValue(0),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public int ColumnCount
        {
            get
            {
                return Columns.Count;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(ColumnCount), value, 0));
                }
                if (DataSource != null)
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
                        // Some DataGridViewTextBoxColumn columns need to be appened.
                        while (value > Columns.Count)
                        {
                            int currentColumnCount = Columns.Count;
                            Columns.Add(null /*columnName*/, null /*headerText*/);
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

        [
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_ColumnHeadersBorderStyleDescr)),
            Browsable(true),
            DefaultValue(DataGridViewHeaderBorderStyle.Raised)
        ]
        public DataGridViewHeaderBorderStyle ColumnHeadersBorderStyle
        {
            get
            {
                switch (advancedColumnHeadersBorderStyle.All)
                {
                    case DataGridViewAdvancedCellBorderStyle.NotSet:
                        return DataGridViewHeaderBorderStyle.Custom;

                    case DataGridViewAdvancedCellBorderStyle.None:
                        return DataGridViewHeaderBorderStyle.None;

                    case DataGridViewAdvancedCellBorderStyle.Single:
                        return DataGridViewHeaderBorderStyle.Single;

                    case DataGridViewAdvancedCellBorderStyle.InsetDouble:
                        return DataGridViewHeaderBorderStyle.Sunken;

                    case DataGridViewAdvancedCellBorderStyle.OutsetPartial:
                        return DataGridViewHeaderBorderStyle.Raised;

                    default:
                        return DataGridViewHeaderBorderStyle.Custom;
                }
            }
            set
            {
                // Sequential enum.  Valid values are 0x0 to 0x4
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DataGridViewHeaderBorderStyle.Custom, (int)DataGridViewHeaderBorderStyle.None))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridViewHeaderBorderStyle));
                }
                if (value != ColumnHeadersBorderStyle)
                {
                    if (value == DataGridViewHeaderBorderStyle.Custom)
                    {
                        throw new ArgumentException(string.Format(SR.DataGridView_CustomCellBorderStyleInvalid, "ColumnHeadersBorderStyle"));
                    }
                    dataGridViewOper[DATAGRIDVIEWOPER_inBorderStyleChange] = true;
                    try
                    {
                        switch (value)
                        {
                            case DataGridViewHeaderBorderStyle.Single:
                                advancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Single;
                                break;

                            case DataGridViewHeaderBorderStyle.Raised:
                                advancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.OutsetPartial;
                                break;

                            case DataGridViewHeaderBorderStyle.Sunken:
                                advancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.InsetDouble;
                                break;

                            case DataGridViewHeaderBorderStyle.None:
                                advancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                                break;
                        }
                    }
                    finally
                    {
                        dataGridViewOper[DATAGRIDVIEWOPER_inBorderStyleChange] = false;
                    }
                    OnColumnHeadersBorderStyleChanged(EventArgs.Empty);
                }
            }
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_ColumnHeadersBorderStyleChangedDescr))
        ]
        public event EventHandler ColumnHeadersBorderStyleChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNHEADERSBORDERSTYLECHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNHEADERSBORDERSTYLECHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_ColumnHeadersDefaultCellStyleDescr)),
            AmbientValue(null)
        ]
        public DataGridViewCellStyle ColumnHeadersDefaultCellStyle
        {
            get
            {
                if (columnHeadersDefaultCellStyle == null)
                {
                    columnHeadersDefaultCellStyle = DefaultColumnHeadersDefaultCellStyle;
                }
                return columnHeadersDefaultCellStyle;
            }
            set
            {
                DataGridViewCellStyle cs = ColumnHeadersDefaultCellStyle;
                cs.RemoveScope(DataGridViewCellStyleScopes.ColumnHeaders);
                columnHeadersDefaultCellStyle = value;
                if (value != null)
                {
                    columnHeadersDefaultCellStyle.AddScope(this, DataGridViewCellStyleScopes.ColumnHeaders);
                }
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

                dataGridViewState1[DATAGRIDVIEWSTATE1_ambientColumnHeadersFont] = true;

                return defaultStyle;
            }
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridViewColumnHeadersDefaultCellStyleChangedDescr))
        ]
        public event EventHandler ColumnHeadersDefaultCellStyleChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNHEADERSDEFAULTCELLSTYLECHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNHEADERSDEFAULTCELLSTYLECHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatAppearance)),
            Localizable(true),
            SRDescription(nameof(SR.DataGridView_ColumnHeadersHeightDescr))
        ]
        public int ColumnHeadersHeight
        {
            get
            {
                return columnHeadersHeight;
            }
            set
            {
                if (value < minimumColumnHeadersHeight)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), string.Format(SR.InvalidLowBoundArgumentEx, nameof(ColumnHeadersHeight), value, minimumColumnHeadersHeight));
                }
                if (value > maxHeadersThickness)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), string.Format(SR.InvalidHighBoundArgumentEx, nameof(ColumnHeadersHeight), value, maxHeadersThickness));
                }
                if (ColumnHeadersHeightSizeMode == DataGridViewColumnHeadersHeightSizeMode.AutoSize)
                {
                    cachedColumnHeadersHeight = value;
                }
                else if (columnHeadersHeight != value)
                {
                    SetColumnHeadersHeightInternal(value, true /*invalidInAdjustFillingColumns*/);
                }
            }
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridViewColumnHeadersHeightChangedDescr))
        ]
        public event EventHandler ColumnHeadersHeightChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNHEADERSHEIGHTCHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNHEADERSHEIGHTCHANGED, value);
        }

        private bool ShouldSerializeColumnHeadersHeight()
        {
            return ColumnHeadersHeightSizeMode != DataGridViewColumnHeadersHeightSizeMode.AutoSize && defaultColumnHeadersHeight != ColumnHeadersHeight;
        }

        /// <summary>
        ///  Gets or sets a value that determines the behavior for adjusting the column headers height.
        /// </summary>
        [
            DefaultValue(DataGridViewColumnHeadersHeightSizeMode.EnableResizing),
            RefreshProperties(RefreshProperties.All),
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_ColumnHeadersHeightSizeModeDescr))
        ]
        public DataGridViewColumnHeadersHeightSizeMode ColumnHeadersHeightSizeMode
        {
            get
            {
                return columnHeadersHeightSizeMode;
            }
            set
            {
                // Sequential enum.  Valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DataGridViewColumnHeadersHeightSizeMode.EnableResizing, (int)DataGridViewColumnHeadersHeightSizeMode.AutoSize))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridViewColumnHeadersHeightSizeMode));
                }
                if (columnHeadersHeightSizeMode != value)
                {
                    /*if (value == DataGridViewColumnHeadersHeightSizeMode.AutoSize && !this.ColumnHeadersVisible)
                    {
                        We intentionally don't throw an error because of designer code spit order.
                    }*/
                    DataGridViewAutoSizeModeEventArgs dgvasme = new DataGridViewAutoSizeModeEventArgs(columnHeadersHeightSizeMode == DataGridViewColumnHeadersHeightSizeMode.AutoSize);
                    columnHeadersHeightSizeMode = value;
                    OnColumnHeadersHeightSizeModeChanged(dgvasme);
                }
            }
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_ColumnHeadersHeightSizeModeChangedDescr))
        ]
        public event DataGridViewAutoSizeModeEventHandler ColumnHeadersHeightSizeModeChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNHEADERSHEIGHTSIZEMODECHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNHEADERSHEIGHTSIZEMODECHANGED, value);
        }

        /// <summary>
        ///  Indicates whether the ComboBox editing control was just detached. (focused out to another cell)
        /// </summary>
        internal bool ComboBoxControlWasDetached { get; set; }

        /// <summary>
        ///  Indicates whether the TextBox editing control was just detached. (focused out to another cell)
        /// </summary>
        internal bool TextBoxControlWasDetached { get; set; }

        /// <summary>
        ///  Gets
        ///  or sets a value indicating if the dataGridView's column headers are visible.
        /// </summary>
        [
            SRCategory(nameof(SR.CatAppearance)),
            DefaultValue(true),
            SRDescription(nameof(SR.DataGridViewColumnHeadersVisibleDescr))
        ]
        public bool ColumnHeadersVisible
        {
            get
            {
                return dataGridViewState1[DATAGRIDVIEWSTATE1_columnHeadersVisible];
            }
            set
            {
                if (ColumnHeadersVisible != value)
                {
                    if (!value)
                    {
                        // Make sure that there is no visible column that only counts on the column headers to autosize
                        DataGridViewColumn dataGridViewColumn = Columns.GetFirstColumn(DataGridViewElementStates.Visible);
                        while (dataGridViewColumn != null)
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
                        dataGridViewState1[DATAGRIDVIEWSTATE1_columnHeadersVisible] = value;
                        layout.ColumnHeadersVisible = value;
                        displayedBandsInfo.EnsureDirtyState();
                        if (!AutoSize)
                        {
                            PerformLayoutPrivate(false /*useRowShortcut*/, false /*computeVisibleRows*/, true /*invalidInAdjustFillingColumns*/, true /*repositionEditingControl*/);
                        }
                        InvalidateInside();
                        OnColumnHeadersGlobalAutoSize();
                    }
                }
            }
        }

        [
            Editor("System.Windows.Forms.Design.DataGridViewColumnCollectionEditor, " + AssemblyRef.SystemDesign, typeof(Drawing.Design.UITypeEditor)),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
            MergableProperty(false)
        ]
        public DataGridViewColumnCollection Columns
        {
            get
            {
                if (dataGridViewColumns == null)
                {
                    dataGridViewColumns = CreateColumnsInstance();
                }
                return dataGridViewColumns;
            }
        }

        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public DataGridViewCell CurrentCell
        {
            get
            {
                if (ptCurrentCell.X == -1 && ptCurrentCell.Y == -1)
                {
                    return null;
                }
                Debug.Assert(ptCurrentCell.X >= 0 && ptCurrentCell.Y >= 0);
                Debug.Assert(ptCurrentCell.X < Columns.Count);
                Debug.Assert(ptCurrentCell.Y < Rows.Count);
                DataGridViewRow dataGridViewRow = (DataGridViewRow)Rows[ptCurrentCell.Y]; // unsharing row
                return dataGridViewRow.Cells[ptCurrentCell.X];
            }
            set
            {
                if ((value != null && (value.RowIndex != ptCurrentCell.Y || value.ColumnIndex != ptCurrentCell.X)) ||
                    (value == null && ptCurrentCell.X != -1))
                {
                    if (value == null)
                    {
                        ClearSelection();
                        if (!SetCurrentCellAddressCore(-1, -1, true /*setAnchorCellAddress*/, true /*validateCurrentCell*/, false /*throughMouseClick*/))
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
                        if (!Columns[value.ColumnIndex].Visible ||
                            (Rows.GetRowState(value.RowIndex) & DataGridViewElementStates.Visible) == 0)
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
                        ClearSelection(value.ColumnIndex, value.RowIndex, true /*selectExceptionElement*/);
                        if (!SetCurrentCellAddressCore(value.ColumnIndex, value.RowIndex, true, false, false))
                        {
                            throw new InvalidOperationException(SR.DataGridView_CellChangeCannotBeCommittedOrAborted);
                        }
                    }
                }
            }
        }

        [
            Browsable(false)
        ]
        public Point CurrentCellAddress
        {
            get
            {
                return ptCurrentCell;
            }
        }

        private DataGridViewCell CurrentCellInternal
        {
            get
            {
                Debug.Assert(ptCurrentCell.X >= 0 && ptCurrentCell.X < Columns.Count);
                Debug.Assert(ptCurrentCell.Y >= 0 && ptCurrentCell.Y < Rows.Count);
                DataGridViewRow dataGridViewRow = Rows.SharedRow(ptCurrentCell.Y);
                Debug.Assert(dataGridViewRow != null);
                DataGridViewCell dataGridViewCell = dataGridViewRow.Cells[ptCurrentCell.X];
                Debug.Assert(IsSharedCellVisible(dataGridViewCell, ptCurrentCell.Y));
                return dataGridViewCell;
            }
        }

        private bool CurrentCellIsFirstVisibleCell
        {
            get
            {
                if (ptCurrentCell.X == -1)
                {
                    return false;
                }
                Debug.Assert(ptCurrentCell.Y != -1);

                bool previousVisibleColumnExists = (null != Columns.GetPreviousColumn(Columns[ptCurrentCell.X], DataGridViewElementStates.Visible, DataGridViewElementStates.None));
                bool previousVisibleRowExists = (-1 != Rows.GetPreviousRow(ptCurrentCell.Y, DataGridViewElementStates.Visible));

                return !previousVisibleColumnExists && !previousVisibleRowExists;
            }
        }

        private bool CurrentCellIsLastVisibleCell
        {
            get
            {
                if (ptCurrentCell.X == -1)
                {
                    return false;
                }

                Debug.Assert(ptCurrentCell.Y != -1);

                bool nextVisibleColumnExists = (null != Columns.GetNextColumn(Columns[ptCurrentCell.X], DataGridViewElementStates.Visible, DataGridViewElementStates.None));
                bool nextVisibleRowExists = (-1 != Rows.GetNextRow(ptCurrentCell.Y, DataGridViewElementStates.Visible));

                return !nextVisibleColumnExists && !nextVisibleRowExists;
            }
        }

        private bool CurrentCellIsEditedAndOnlySelectedCell
        {
            get
            {
                if (ptCurrentCell.X == -1)
                {
                    return false;
                }

                Debug.Assert(ptCurrentCell.Y != -1);

                return editingControl != null &&
                       GetCellCount(DataGridViewElementStates.Selected) == 1 &&
                       CurrentCellInternal.Selected;
            }
        }

        [
            Browsable(false)
        ]
        public DataGridViewRow CurrentRow
        {
            get
            {
                if (ptCurrentCell.X == -1)
                {
                    return null;
                }

                Debug.Assert(ptCurrentCell.Y >= 0);
                Debug.Assert(ptCurrentCell.Y < Rows.Count);

                return Rows[ptCurrentCell.Y];
            }
        }

        internal Cursor CursorInternal
        {
            set
            {
                dataGridViewState2[DATAGRIDVIEWSTATE2_ignoreCursorChange] = true;
                try
                {
                    Cursor = value;
                }
                finally
                {
                    dataGridViewState2[DATAGRIDVIEWSTATE2_ignoreCursorChange] = false;
                }
            }
        }

        internal DataGridViewDataConnection DataConnection
        {
            get
            {
                return dataConnection;
            }
        }

        [
         DefaultValue(""),
         SRCategory(nameof(SR.CatData)),
         Editor("System.Windows.Forms.Design.DataMemberListEditor, " + AssemblyRef.SystemDesign, typeof(Drawing.Design.UITypeEditor)),
         SRDescription(nameof(SR.DataGridViewDataMemberDescr))
        ]
        public string DataMember
        {
            get
            {
                if (dataConnection == null)
                {
                    return string.Empty;
                }
                else
                {
                    return dataConnection.DataMember;
                }
            }
            set
            {
                if (value != DataMember)
                {
                    CurrentCell = null;
                    if (dataConnection == null)
                    {
                        dataConnection = new DataGridViewDataConnection(this);
                    }
                    dataConnection.SetDataConnection(DataSource, value);
                    OnDataMemberChanged(EventArgs.Empty);
                }
            }
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridViewDataMemberChangedDescr))
        ]
        public event EventHandler DataMemberChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWDATAMEMBERCHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWDATAMEMBERCHANGED, value);
        }

        [
         DefaultValue(null),
         SRCategory(nameof(SR.CatData)),
         RefreshProperties(RefreshProperties.Repaint),
         AttributeProvider(typeof(IListSource)),
         SRDescription(nameof(SR.DataGridViewDataSourceDescr))
        ]
        public object DataSource
        {
            get
            {
                if (dataConnection == null)
                {
                    return null;
                }
                else
                {
                    return dataConnection.DataSource;
                }
            }
            set
            {
                if (value != DataSource)
                {
                    CurrentCell = null;
                    if (dataConnection == null)
                    {
                        dataConnection = new DataGridViewDataConnection(this);
                        dataConnection.SetDataConnection(value, DataMember);
                    }
                    else
                    {
                        if (dataConnection.ShouldChangeDataMember(value))
                        {
                            // we fire DataMemberChanged event
                            DataMember = string.Empty;
                        }
                        dataConnection.SetDataConnection(value, DataMember);
                        if (value == null)
                        {
                            dataConnection = null;
                        }
                    }
                    OnDataSourceChanged(EventArgs.Empty);
                }
            }
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridViewDataSourceChangedDescr))
        ]
        public event EventHandler DataSourceChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWDATASOURCECHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWDATASOURCECHANGED, value);
        }

        private static SolidBrush DefaultBackBrush
        {
            get
            {
                return (SolidBrush)SystemBrushes.Window;
            }
        }

        private static SolidBrush DefaultBackgroundBrush
        {
            get
            {
                return (SolidBrush)SystemBrushes.ControlDark;
            }
        }

        [
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_DefaultCellStyleDescr)),
            AmbientValue(null)
        ]
        public DataGridViewCellStyle DefaultCellStyle
        {
            get
            {
                if (defaultCellStyle == null)
                {
                    defaultCellStyle = DefaultDefaultCellStyle;
                    return defaultCellStyle;
                }
                else if (defaultCellStyle.BackColor == Color.Empty ||
                    defaultCellStyle.ForeColor == Color.Empty ||
                    defaultCellStyle.SelectionBackColor == Color.Empty ||
                    defaultCellStyle.SelectionForeColor == Color.Empty ||
                    defaultCellStyle.Font == null ||
                    defaultCellStyle.Alignment == DataGridViewContentAlignment.NotSet ||
                    defaultCellStyle.WrapMode == DataGridViewTriState.NotSet)
                {
                    DataGridViewCellStyle defaultCellStyleTmp = new DataGridViewCellStyle(defaultCellStyle)
                    {
                        Scope = DataGridViewCellStyleScopes.None
                    };
                    if (defaultCellStyle.BackColor == Color.Empty)
                    {
                        defaultCellStyleTmp.BackColor = DefaultBackBrush.Color;
                    }
                    if (defaultCellStyle.ForeColor == Color.Empty)
                    {
                        defaultCellStyleTmp.ForeColor = base.ForeColor;
                        dataGridViewState1[DATAGRIDVIEWSTATE1_ambientForeColor] = true;
                    }
                    if (defaultCellStyle.SelectionBackColor == Color.Empty)
                    {
                        defaultCellStyleTmp.SelectionBackColor = DefaultSelectionBackBrush.Color;
                    }
                    if (defaultCellStyle.SelectionForeColor == Color.Empty)
                    {
                        defaultCellStyleTmp.SelectionForeColor = DefaultSelectionForeBrush.Color;
                    }
                    if (defaultCellStyle.Font == null)
                    {
                        defaultCellStyleTmp.Font = base.Font;
                        dataGridViewState1[DATAGRIDVIEWSTATE1_ambientFont] = true;
                    }
                    if (defaultCellStyle.Alignment == DataGridViewContentAlignment.NotSet)
                    {
                        defaultCellStyleTmp.AlignmentInternal = DataGridViewContentAlignment.MiddleLeft;
                    }
                    if (defaultCellStyle.WrapMode == DataGridViewTriState.NotSet)
                    {
                        defaultCellStyleTmp.WrapModeInternal = DataGridViewTriState.False;
                    }
                    defaultCellStyleTmp.AddScope(this, DataGridViewCellStyleScopes.DataGridView);
                    return defaultCellStyleTmp;
                }
                else
                {
                    return defaultCellStyle;
                }
            }
            set
            {
                DataGridViewCellStyle cs = DefaultCellStyle;
                cs.RemoveScope(DataGridViewCellStyleScopes.DataGridView);
                defaultCellStyle = value;
                if (value != null)
                {
                    defaultCellStyle.AddScope(this, DataGridViewCellStyleScopes.DataGridView);
                }
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
                    BackColor = DefaultBackBrush.Color,
                    ForeColor = base.ForeColor,
                    SelectionBackColor = DefaultSelectionBackBrush.Color,
                    SelectionForeColor = DefaultSelectionForeBrush.Color,
                    Font = base.Font,
                    AlignmentInternal = DataGridViewContentAlignment.MiddleLeft,
                    WrapModeInternal = DataGridViewTriState.False
                };
                defaultCellStyle.AddScope(this, DataGridViewCellStyleScopes.DataGridView);

                dataGridViewState1[DATAGRIDVIEWSTATE1_ambientFont] = true;
                dataGridViewState1[DATAGRIDVIEWSTATE1_ambientForeColor] = true;

                return defaultCellStyle;
            }
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridViewDefaultCellStyleChangedDescr))
        ]
        public event EventHandler DefaultCellStyleChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWDEFAULTCELLSTYLECHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWDEFAULTCELLSTYLECHANGED, value);
        }

        private static SolidBrush DefaultForeBrush
        {
            get
            {
                return (SolidBrush)SystemBrushes.WindowText;
            }
        }

        private static Color DefaultGridColor
        {
            get
            {
                return SystemColors.ControlDark;
            }
        }

        private static SolidBrush DefaultHeadersBackBrush
        {
            get
            {
                return (SolidBrush)SystemBrushes.Control;
            }
        }

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

                dataGridViewState1[DATAGRIDVIEWSTATE1_ambientRowHeadersFont] = true;

                return defaultStyle;
            }
        }

        private static SolidBrush DefaultSelectionBackBrush
        {
            get
            {
                return (SolidBrush)SystemBrushes.Highlight;
            }
        }

        private static SolidBrush DefaultSelectionForeBrush
        {
            get
            {
                return (SolidBrush)SystemBrushes.HighlightText;
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(240, 150);
            }
        }

        internal DisplayedBandsData DisplayedBandsInfo
        {
            get
            {
                return displayedBandsInfo;
            }
        }

        /// <summary>
        ///  Returns the client rect of the display area of the control.
        ///  The DataGridView control return its client rectangle minus the potential scrollbars.
        /// </summary>
        public override Rectangle DisplayRectangle
        {
            get
            {
                Rectangle rectDisplay = ClientRectangle;
                if (horizScrollBar != null && horizScrollBar.Visible)
                {
                    rectDisplay.Height -= horizScrollBar.Height;
                }
                if (vertScrollBar != null && vertScrollBar.Visible)
                {
                    rectDisplay.Width -= vertScrollBar.Width;
                    if (RightToLeftInternal)
                    {
                        rectDisplay.X = vertScrollBar.Width;
                    }
                }
                return rectDisplay;
            }
        }

        [
            SRCategory(nameof(SR.CatBehavior)),
            DefaultValue(DataGridViewEditMode.EditOnKeystrokeOrF2),
            SRDescription(nameof(SR.DataGridView_EditModeDescr))
        ]
        public DataGridViewEditMode EditMode
        {
            get
            {
                return editMode;
            }
            set
            {
                // Sequential enum.  Valid values are 0x0 to 0x4
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DataGridViewEditMode.EditOnEnter, (int)DataGridViewEditMode.EditProgrammatically))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridViewEditMode));
                }
                if (editMode != value)
                {
                    editMode = value;
                    OnEditModeChanged(EventArgs.Empty);
                }
            }
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_EditModeChangedDescr))
        ]
        public event EventHandler EditModeChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWEDITMODECHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWEDITMODECHANGED, value);
        }

        internal Point MouseEnteredCellAddress
        {
            get
            {
                return ptMouseEnteredCell;
            }
        }

        private bool MouseOverEditingControl
        {
            get
            {
                if (editingControl != null)
                {
                    Point ptMouse = PointToClient(Control.MousePosition);
                    return editingControl.Bounds.Contains(ptMouse);
                }
                return false;
            }
        }

        private bool MouseOverEditingPanel
        {
            get
            {
                if (editingPanel != null)
                {
                    Point ptMouse = PointToClient(Control.MousePosition);
                    return editingPanel.Bounds.Contains(ptMouse);
                }
                return false;
            }
        }

        private bool MouseOverScrollBar
        {
            get
            {
                Point ptMouse = PointToClient(Control.MousePosition);
                if (vertScrollBar != null && vertScrollBar.Visible)
                {
                    if (vertScrollBar.Bounds.Contains(ptMouse))
                    {
                        return true;
                    }
                }
                if (horizScrollBar != null && horizScrollBar.Visible)
                {
                    return horizScrollBar.Bounds.Contains(ptMouse);
                }
                return false;
            }
        }

        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public Control EditingControl
        {
            get
            {
                return editingControl;
            }
        }

        internal AccessibleObject EditingControlAccessibleObject
        {
            get
            {
                return EditingControl.AccessibilityObject;
            }
        }

        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public Panel EditingPanel
        {
            get
            {
                if (editingPanel == null)
                {
                    editingPanel = new DataGridViewEditingPanel(this)
                    {
                        AccessibleName = SR.DataGridView_AccEditingPanelAccName
                    };
                }
                return editingPanel;
            }
        }

        internal DataGridViewEditingPanelAccessibleObject EditingPanelAccessibleObject
        {
            get
            {
                if (editingPanelAccessibleObject == null)
                {
                    editingPanelAccessibleObject = new DataGridViewEditingPanelAccessibleObject(this, EditingPanel);
                }

                return editingPanelAccessibleObject;
            }
        }

        /// <summary>
        ///  Determines whether the DataGridView's header cells render using XP theming visual styles or not
        ///  when visual styles are enabled in the application.
        /// </summary>
        [
            SRCategory(nameof(SR.CatAppearance)),
            DefaultValue(true),
            SRDescription(nameof(SR.DataGridView_EnableHeadersVisualStylesDescr))
        ]
        public bool EnableHeadersVisualStyles
        {
            get
            {
                return dataGridViewState2[DATAGRIDVIEWSTATE2_enableHeadersVisualStyles];
            }
            set
            {
                if (dataGridViewState2[DATAGRIDVIEWSTATE2_enableHeadersVisualStyles] != value)
                {
                    dataGridViewState2[DATAGRIDVIEWSTATE2_enableHeadersVisualStyles] = value;
                    //OnEnableHeadersVisualStylesChanged(EventArgs.Empty);
                    // Some autosizing may have to be applied since the margins are potentially changed.
                    OnGlobalAutoSize(); // Put this into OnEnableHeadersVisualStylesChanged if created.
                }
            }
        }

        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public DataGridViewCell FirstDisplayedCell
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
                if (value != null)
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

                    Debug.Assert(firstDisplayedCell.RowIndex >= 0 &&
                        firstDisplayedCell.RowIndex < Rows.Count &&
                        firstDisplayedCell.ColumnIndex >= 0 &&
                        firstDisplayedCell.ColumnIndex < Columns.Count);

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
                Point ptFirstDisplayedCellAddress = new Point(-1, -1)
                {
                    Y = Rows.GetFirstRow(DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen)
                };
                if (ptFirstDisplayedCellAddress.Y == -1)
                {
                    Debug.Assert(displayedBandsInfo.NumTotallyDisplayedFrozenRows == 0);
                    if (displayedBandsInfo.FirstDisplayedScrollingRow >= 0)
                    {
                        ptFirstDisplayedCellAddress.Y = displayedBandsInfo.FirstDisplayedScrollingRow;
                    }
#if DEBUG
                    else
                    {
                        Debug.Assert(displayedBandsInfo.FirstDisplayedScrollingRow == -1);
                        Debug.Assert(displayedBandsInfo.NumDisplayedScrollingRows == 0);
                        Debug.Assert(displayedBandsInfo.NumTotallyDisplayedScrollingRows == 0);
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
                DataGridViewColumn dataGridViewColumn = Columns.GetFirstColumn(DataGridViewElementStates.Visible);
                if (dataGridViewColumn != null)
                {
                    if (dataGridViewColumn.Frozen)
                    {
                        firstDisplayedColumnIndex = dataGridViewColumn.Index;
                    }
                    else if (displayedBandsInfo.FirstDisplayedScrollingCol >= 0)
                    {
                        firstDisplayedColumnIndex = displayedBandsInfo.FirstDisplayedScrollingCol;
                    }
                }
#if DEBUG
                DataGridViewColumn dataGridViewColumnDbg1 = Columns.GetFirstColumn(DataGridViewElementStates.Displayed);
                int firstDisplayedColumnIndexDbg1 = (dataGridViewColumnDbg1 == null) ? -1 : dataGridViewColumnDbg1.Index;

                int firstDisplayedColumnIndexDbg2 = -1;
                DataGridViewColumn dataGridViewColumnDbg = Columns.GetFirstColumn(DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen);
                if (dataGridViewColumnDbg != null)
                {
                    firstDisplayedColumnIndexDbg2 = dataGridViewColumnDbg.Index;
                }
                else if (displayedBandsInfo.FirstDisplayedScrollingCol >= 0)
                {
                    firstDisplayedColumnIndexDbg2 = displayedBandsInfo.FirstDisplayedScrollingCol;
                }
                else
                {
                    Debug.Assert(displayedBandsInfo.LastTotallyDisplayedScrollingCol == -1);
                }
                Debug.Assert(firstDisplayedColumnIndex == firstDisplayedColumnIndexDbg1 || !Visible || displayedBandsInfo.Dirty);
                Debug.Assert(firstDisplayedColumnIndex == firstDisplayedColumnIndexDbg2 || displayedBandsInfo.Dirty);
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
                    if ((Rows.GetRowState(firstDisplayedRowIndex) & DataGridViewElementStates.Frozen) == 0 &&
                        displayedBandsInfo.FirstDisplayedScrollingRow >= 0)
                    {
                        firstDisplayedRowIndex = displayedBandsInfo.FirstDisplayedScrollingRow;
                    }
                }
#if FALSE //DEBUG
                int firstDisplayedRowIndexDbg1 = this.Rows.GetFirstRow(DataGridViewElementStates.Displayed);

                int firstDisplayedRowIndexDbg2 = this.Rows.GetFirstRow(DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen);
                if (firstDisplayedRowIndexDbg2 == -1)
                {
                    if (this.displayedBandsInfo.FirstDisplayedScrollingRow >= 0)
                    {
                        firstDisplayedRowIndexDbg2 = this.displayedBandsInfo.FirstDisplayedScrollingRow;
                    }
                }

                Debug.Assert(firstDisplayedRowIndex == firstDisplayedRowIndexDbg1 || !this.Visible || this.displayedBandsInfo.Dirty, "firstDisplayedRowIndex =" + firstDisplayedRowIndex.ToString() + ", firstDisplayedRowIndexDbg1=" + firstDisplayedRowIndexDbg1.ToString());
                Debug.Assert(firstDisplayedRowIndex == firstDisplayedRowIndexDbg2 || this.displayedBandsInfo.Dirty, "firstDisplayedRowIndex =" + firstDisplayedRowIndex.ToString() + ", firstDisplayedRowIndexDbg2=" + firstDisplayedRowIndexDbg2.ToString());
#endif
                return firstDisplayedRowIndex;
            }
        }

        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public int FirstDisplayedScrollingColumnHiddenWidth
        {
            get
            {
                return negOffset;
            }
        }

        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public int FirstDisplayedScrollingColumnIndex
        {
            get
            {
                return displayedBandsInfo.FirstDisplayedScrollingCol;
            }
            set
            {
                if (value < 0 || value >= Columns.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
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

                int displayWidth = layout.Data.Width;
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

                if (value == displayedBandsInfo.FirstDisplayedScrollingCol)
                {
                    return;
                }

                if (ptCurrentCell.X >= 0 &&
                    !CommitEdit(DataGridViewDataErrorContexts.Parsing | DataGridViewDataErrorContexts.Commit | DataGridViewDataErrorContexts.Scroll,
                                false /*forCurrentCellChange*/, false /*forCurrentRowChange*/))
                {
                    // Could not commit edited cell value - return silently
                    // Microsoft: should we throw an error here?
                    return;
                }
                if (IsColumnOutOfBounds(value))
                {
                    return;
                }
                bool success = ScrollColumnIntoView(value, -1, /*committed*/ true, false /*forCurrentCellChange*/);
                Debug.Assert(success);

                Debug.Assert(displayedBandsInfo.FirstDisplayedScrollingCol >= 0);
                Debug.Assert(displayedBandsInfo.FirstDisplayedScrollingCol == value ||
                             Columns.DisplayInOrder(displayedBandsInfo.FirstDisplayedScrollingCol, value));
                int maxHorizontalOffset = Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - displayWidth;
                while (displayedBandsInfo.FirstDisplayedScrollingCol != value &&
                        HorizontalOffset < maxHorizontalOffset)
                {
                    ScrollColumns(1);
                }
            }
        }

        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public int FirstDisplayedScrollingRowIndex
        {
            get
            {
                return displayedBandsInfo.FirstDisplayedScrollingRow;
            }
            set
            {
                if (value < 0 || value >= Rows.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
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

                int displayHeight = layout.Data.Height;
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

                if (value == displayedBandsInfo.FirstDisplayedScrollingRow)
                {
                    return;
                }

                if (ptCurrentCell.X >= 0 &&
                    !CommitEdit(DataGridViewDataErrorContexts.Parsing | DataGridViewDataErrorContexts.Commit | DataGridViewDataErrorContexts.Scroll,
                                false /*forCurrentCellChange*/, false /*forCurrentRowChange*/))
                {
                    // Could not commit edited cell value - return silently
                    // Microsoft: should we throw an error here?
                    return;
                }
                if (IsRowOutOfBounds(value))
                {
                    return;
                }

                Debug.Assert(displayedBandsInfo.FirstDisplayedScrollingRow >= 0);

                if (value > displayedBandsInfo.FirstDisplayedScrollingRow)
                {
                    int rowsToScroll = Rows.GetRowCount(DataGridViewElementStates.Visible, displayedBandsInfo.FirstDisplayedScrollingRow, value);
                    Debug.Assert(rowsToScroll != 0);
                    ScrollRowsByCount(rowsToScroll, rowsToScroll > 1 ? ScrollEventType.LargeIncrement : ScrollEventType.SmallIncrement);
                }
                else
                {
                    bool success = ScrollRowIntoView(-1, value, /*committed*/ true, false /*forCurrentCellChange*/);
                    Debug.Assert(success);
                }
            }
        }

        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
            }
        }

        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        new public event EventHandler ForeColorChanged
        {
            add => base.ForeColorChanged += value;
            remove => base.ForeColorChanged -= value;
        }

        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
            }
        }

        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        new public event EventHandler FontChanged
        {
            add => base.FontChanged += value;
            remove => base.FontChanged -= value;
        }

        /// <summary>
        ///  Gets or sets the grid color of the dataGridView (when Single mode is used).
        /// </summary>
        [
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridViewGridColorDescr))
        ]
        public Color GridColor
        {
            get
            {
                return gridPen.Color;
            }
            set
            {
                if (value.IsEmpty)
                {
                    throw new ArgumentException(string.Format(SR.DataGridView_EmptyColor, "GridColor"));
                }
                if (value.A < 255)
                {
                    throw new ArgumentException(string.Format(SR.DataGridView_TransparentColor, "GridColor"));
                }
                if (!value.Equals(gridPen.Color))
                {
                    if (gridPen != null)
                    {
                        gridPen.Dispose();
                    }

                    gridPen = new Pen(value);
                    OnGridColorChanged(EventArgs.Empty);
                }
            }
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridViewOnGridColorChangedDescr))
        ]
        public event EventHandler GridColorChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWGRIDCOLORCHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWGRIDCOLORCHANGED, value);
        }

        private bool ShouldSerializeGridColor()
        {
            return !GridPen.Color.Equals(DefaultGridColor);
        }

        internal Pen GridPen
        {
            get
            {
                return gridPen;
            }
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
                {
                    value = 0;
                }
                int widthNotVisible = Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - layout.Data.Width;
                if (value > widthNotVisible && widthNotVisible > 0)
                {
                    value = widthNotVisible;
                }
                if (value == horizontalOffset)
                {
                    return;
                }

                ScrollEventType scrollEventType;
                int oldFirstVisibleScrollingCol = displayedBandsInfo.FirstDisplayedScrollingCol;
                int change = horizontalOffset - value;
                if (horizScrollBar.Enabled)
                {
                    horizScrollBar.Value = value;
                }
                horizontalOffset = value;

                int totalVisibleFrozenWidth = Columns.GetColumnsWidth(DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen);

                Rectangle rectTmp = layout.Data;
                if (layout.ColumnHeadersVisible)
                {
                    // column headers must scroll as well
                    rectTmp = Rectangle.Union(rectTmp, layout.ColumnHeaders);
                }
                else if (SingleVerticalBorderAdded)
                {
                    if (!RightToLeftInternal)
                    {
                        rectTmp.X--;
                    }
                    rectTmp.Width++;
                }

                if (SingleVerticalBorderAdded &&
                    totalVisibleFrozenWidth > 0)
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

                displayedBandsInfo.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();
                // update the lastTotallyDisplayedScrollingCol
                ComputeVisibleColumns();

                if (editingControl != null &&
                    !Columns[ptCurrentCell.X].Frozen &&
                    displayedBandsInfo.FirstDisplayedScrollingCol > -1)
                {
                    PositionEditingControl(true /*setLocation*/, false /*setSize*/, false /*setFocus*/);
                }

                // The mouse probably is not over the same cell after the scroll.
                UpdateMouseEnteredCell(null /*HitTestInfo*/, null /*MouseEventArgs*/);

                if (oldFirstVisibleScrollingCol == displayedBandsInfo.FirstDisplayedScrollingCol)
                {
                    scrollEventType = change > 0 ? ScrollEventType.SmallIncrement : ScrollEventType.SmallDecrement;
                }
                else if (Columns.DisplayInOrder(oldFirstVisibleScrollingCol, displayedBandsInfo.FirstDisplayedScrollingCol))
                {
                    scrollEventType = Columns.GetColumnCount(DataGridViewElementStates.Visible, oldFirstVisibleScrollingCol, displayedBandsInfo.FirstDisplayedScrollingCol) > 1 ? ScrollEventType.LargeIncrement : ScrollEventType.SmallIncrement;
                }
                else
                {
                    Debug.Assert(Columns.DisplayInOrder(displayedBandsInfo.FirstDisplayedScrollingCol, oldFirstVisibleScrollingCol));
                    scrollEventType = Columns.GetColumnCount(DataGridViewElementStates.Visible, displayedBandsInfo.FirstDisplayedScrollingCol, oldFirstVisibleScrollingCol) > 1 ? ScrollEventType.LargeDecrement : ScrollEventType.SmallDecrement;
                }

                RECT[] rects = CreateScrollableRegion(rectTmp);
                if (RightToLeftInternal)
                {
                    change = -change;
                }
                ScrollRectangles(rects, change);
                if (!dataGridViewState2[DATAGRIDVIEWSTATE2_stopRaisingHorizontalScroll])
                {
                    OnScroll(scrollEventType, horizontalOffset + change, horizontalOffset, ScrollOrientation.HorizontalScroll);
                }
                FlushDisplayedChanged();
            }
        }

        protected ScrollBar HorizontalScrollBar
        {
            get
            {
                return horizScrollBar;
            }
        }

        internal int HorizontalScrollBarHeight
        {
            get
            {
                return horizScrollBar.Height;
            }
        }

        internal bool HorizontalScrollBarVisible
        {
            get
            {
                return horizScrollBar.Visible;
            }
        }

        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public int HorizontalScrollingOffset
        {
            get
            {
                return horizontalOffset;
            }
            set
            {
                // int widthNotVisible = this.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - this.layout.Data.Width;
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(HorizontalScrollingOffset), value, 0));
                }
                // Intentionally ignoring the out of range situation.
                // else if (value > widthNotVisible && widthNotVisible > 0)
                //{
                //    throw new ArgumentOutOfRangeException(string.Format(SR.DataGridView_PropertyTooLarge, "HorizontalScrollingOffset", (widthNotVisible).ToString()));
                //}
                else if (value > 0 && (Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - layout.Data.Width) <= 0)
                {
                    // Intentionally ignoring the case where dev tries to set value while there is no horizontal scrolling possible.
                    // throw new ArgumentOutOfRangeException(nameof(HorizontalScrollingOffset), SR.DataGridView_PropertyMustBeZero);
                    Debug.Assert(horizontalOffset == 0);
                    return;
                }
                if (value == horizontalOffset)
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
                if (horizScrollTimer == null)
                {
                    horizScrollTimer = new Timer();
                    horizScrollTimer.Tick += new EventHandler(HorizScrollTimer_Tick);
                }
                return horizScrollTimer;
            }
        }

        private bool InAdjustFillingColumns
        {
            get
            {
                return dataGridViewOper[DATAGRIDVIEWOPER_inAdjustFillingColumn] || dataGridViewOper[DATAGRIDVIEWOPER_inAdjustFillingColumns];
            }
        }

        internal bool InBeginEdit
        {
            get
            {
                return dataGridViewOper[DATAGRIDVIEWOPER_inBeginEdit];
            }
        }

        internal bool InDisplayIndexAdjustments
        {
            get
            {
                return dataGridViewOper[DATAGRIDVIEWOPER_inDisplayIndexAdjustments];
            }
            set
            {
                dataGridViewOper[DATAGRIDVIEWOPER_inDisplayIndexAdjustments] = value;
            }
        }

        internal bool InEndEdit
        {
            get
            {
                return dataGridViewOper[DATAGRIDVIEWOPER_inEndEdit];
            }
        }

        private DataGridViewCellStyle InheritedEditingCellStyle
        {
            get
            {
                if (ptCurrentCell.X == -1)
                {
                    return null;
                }

                return CurrentCellInternal.GetInheritedStyleInternal(ptCurrentCell.Y);
            }
        }

        internal bool InInitialization
        {
            get
            {
                return dataGridViewState2[DATAGRIDVIEWSTATE2_initializing];
            }
        }

        internal bool InSortOperation
        {
            get
            {
                return dataGridViewOper[DATAGRIDVIEWOPER_inSort];
            }
        }

        [Browsable(false)]
        public bool IsCurrentCellDirty
        {
            get
            {
                return dataGridViewState1[DATAGRIDVIEWSTATE1_editedCellChanged];
            }
        }

        private bool IsCurrentCellDirtyInternal
        {
            set
            {
                if (value != dataGridViewState1[DATAGRIDVIEWSTATE1_editedCellChanged])
                {
                    dataGridViewState1[DATAGRIDVIEWSTATE1_editedCellChanged] = value;
                    OnCurrentCellDirtyStateChanged(EventArgs.Empty);
                }
            }
        }

        [Browsable(false)]
        public bool IsCurrentCellInEditMode
        {
            get
            {
                return editingControl != null || dataGridViewState1[DATAGRIDVIEWSTATE1_currentCellInEditMode];
            }
        }

        // Only used in bound scenarios, when binding to a IEditableObject
        [Browsable(false)]
        public bool IsCurrentRowDirty
        {
            get
            {
                if (!VirtualMode)
                {
                    return dataGridViewState1[DATAGRIDVIEWSTATE1_editedRowChanged] || IsCurrentCellDirty;
                }
                else
                {
                    QuestionEventArgs qe = new QuestionEventArgs(dataGridViewState1[DATAGRIDVIEWSTATE1_editedRowChanged] || IsCurrentCellDirty);
                    OnRowDirtyStateNeeded(qe);
                    return qe.Response;
                }
            }
        }

        internal bool IsCurrentRowDirtyInternal
        {
            set
            {
                if (value != dataGridViewState1[DATAGRIDVIEWSTATE1_editedRowChanged])
                {
                    dataGridViewState1[DATAGRIDVIEWSTATE1_editedRowChanged] = value;
                    if (RowHeadersVisible && ShowEditingIcon && ptCurrentCell.Y >= 0)
                    {
                        // Force the pencil to appear in the row header
                        InvalidateCellPrivate(-1, ptCurrentCell.Y);
                    }
                }
            }
        }

        private bool IsEscapeKeyEffective
        {
            get
            {
                return dataGridViewOper[DATAGRIDVIEWOPER_trackColResize] ||
                       dataGridViewOper[DATAGRIDVIEWOPER_trackRowResize] ||
                       dataGridViewOper[DATAGRIDVIEWOPER_trackColHeadersResize] ||
                       dataGridViewOper[DATAGRIDVIEWOPER_trackRowHeadersResize] ||
                       dataGridViewOper[DATAGRIDVIEWOPER_trackColRelocation] ||
                       IsCurrentCellDirty ||
                       ((VirtualMode || DataSource != null) && IsCurrentRowDirty) ||
                       (EditMode != DataGridViewEditMode.EditOnEnter && editingControl != null ||
                       dataGridViewState1[DATAGRIDVIEWSTATE1_newRowEdited]);
            }
        }

        private bool IsMinimized
        {
            get
            {
                return TopLevelControlInternal is Form parentForm && parentForm.WindowState == FormWindowState.Minimized;
            }
        }

        private bool IsSharedCellReadOnly(DataGridViewCell dataGridViewCell, int rowIndex)
        {
            Debug.Assert(dataGridViewCell != null);
            Debug.Assert(rowIndex >= 0);
            DataGridViewElementStates rowState = Rows.GetRowState(rowIndex);
            return ReadOnly ||
                   (rowState & DataGridViewElementStates.ReadOnly) != 0 ||
                   (dataGridViewCell.OwningColumn != null && dataGridViewCell.OwningColumn.ReadOnly) ||
                   dataGridViewCell.StateIncludes(DataGridViewElementStates.ReadOnly);
        }

        internal bool IsSharedCellSelected(DataGridViewCell dataGridViewCell, int rowIndex)
        {
            Debug.Assert(dataGridViewCell != null);
            Debug.Assert(rowIndex >= 0);
            DataGridViewElementStates rowState = Rows.GetRowState(rowIndex);
            return (rowState & DataGridViewElementStates.Selected) != 0 ||
                   (dataGridViewCell.OwningColumn != null && dataGridViewCell.OwningColumn.Selected) ||
                   dataGridViewCell.StateIncludes(DataGridViewElementStates.Selected);
        }

        internal bool IsSharedCellVisible(DataGridViewCell dataGridViewCell, int rowIndex)
        {
            Debug.Assert(dataGridViewCell != null);
            Debug.Assert(rowIndex >= 0);
            DataGridViewElementStates rowState = Rows.GetRowState(rowIndex);
            return (rowState & DataGridViewElementStates.Visible) != 0 &&
                   (dataGridViewCell.OwningColumn != null && dataGridViewCell.OwningColumn.Visible);
        }

        internal ToolTip KeyboardToolTip
        {
            get
            {
                ToolTip toolTip;
                if (!Properties.ContainsObject(PropToolTip))
                {
                    toolTip = new ToolTip();
                    toolTip.ReshowDelay = 500;
                    toolTip.InitialDelay = 500;
                    Properties.SetObject(PropToolTip, toolTip);
                }
                else
                {
                    toolTip = (ToolTip)Properties.GetObject(PropToolTip);
                }
                return toolTip;
            }
        }

        internal LayoutData LayoutInfo
        {
            get
            {
                if (layout.dirty && IsHandleCreated)
                {
                    PerformLayoutPrivate(false /*useRowShortcut*/, true /*computeVisibleRows*/, false /*invalidInAdjustFillingColumns*/, false /*repositionEditingControl*/);
                }
                return layout;
            }
        }

        internal Point MouseDownCellAddress
        {
            get
            {
                return ptMouseDownCell;
            }
        }

        [
            SRCategory(nameof(SR.CatBehavior)),
            DefaultValue(true),
            SRDescription(nameof(SR.DataGridView_MultiSelectDescr))
        ]
        public bool MultiSelect
        {
            get
            {
                return dataGridViewState1[DATAGRIDVIEWSTATE1_multiSelect];
            }
            set
            {
                if (MultiSelect != value)
                {
                    ClearSelection();
                    dataGridViewState1[DATAGRIDVIEWSTATE1_multiSelect] = value;
                    OnMultiSelectChanged(EventArgs.Empty);
                }
            }
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridViewOnMultiSelectChangedDescr))
        ]
        public event EventHandler MultiSelectChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWMULTISELECTCHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWMULTISELECTCHANGED, value);
        }

        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public int NewRowIndex
        {
            get
            {
                return newRowIndex;
            }
        }

        internal bool NoDimensionChangeAllowed
        {
            get
            {
                return noDimensionChangeCount > 0;
            }
        }

        private int NoSelectionChangeCount
        {
            get
            {
                return noSelectionChangeCount;
            }
            set
            {
                Debug.Assert(value >= 0);
                noSelectionChangeCount = value;
                if (value == 0)
                {
                    FlushSelectionChanged();
                }
            }
        }

        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Never),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new Padding Padding
        {
            get
            {
                return base.Padding;
            }
            set
            {
                base.Padding = value;
            }
        }

        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Never),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new event EventHandler PaddingChanged
        {
            add => base.PaddingChanged += value;
            remove => base.PaddingChanged -= value;
        }

        internal DataGridViewCellStyle PlaceholderCellStyle
        {
            get
            {
                if (placeholderCellStyle == null)
                {
                    placeholderCellStyle = new DataGridViewCellStyle();
                }
                return placeholderCellStyle;
            }
        }

        [
            Browsable(true),
            DefaultValue(false),
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_ReadOnlyDescr))
        ]
        public bool ReadOnly
        {
            get
            {
                return dataGridViewState1[DATAGRIDVIEWSTATE1_readOnly];
            }
            set
            {
                if (value != dataGridViewState1[DATAGRIDVIEWSTATE1_readOnly])
                {
                    if (value &&
                        ptCurrentCell.X != -1 &&
                        IsCurrentCellInEditMode)
                    {
                        // Current cell becomes read-only. Exit editing mode.
                        if (!EndEdit(DataGridViewDataErrorContexts.Parsing | DataGridViewDataErrorContexts.Commit,
                                     DataGridViewValidateCellInternal.Always /*validateCell*/,
                                     false /*fireCellLeave*/,
                                     false /*fireCellEnter*/,
                                     false /*fireRowLeave*/,
                                     false /*fireRowEnter*/,
                                     false /*fireLeave*/,
                                     true /*keepFocus*/,
                                     false /*resetCurrentCell*/,
                                     false /*resetAnchorCell*/))
                        {
                            throw new InvalidOperationException(SR.DataGridView_CommitFailedCannotCompleteOperation);
                        }
                    }

                    dataGridViewState1[DATAGRIDVIEWSTATE1_readOnly] = value;

                    if (value)
                    {
                        try
                        {
                            dataGridViewOper[DATAGRIDVIEWOPER_inReadOnlyChange] = true;
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
                            dataGridViewOper[DATAGRIDVIEWOPER_inReadOnlyChange] = false;
                        }
                    }
#if DEBUG
                    else
                    {
                        Debug.Assert(individualReadOnlyCells.Count == 0);
                        for (int columnIndex = 0; columnIndex < Columns.Count; columnIndex++)
                        {
                            Debug.Assert(Columns[columnIndex].ReadOnly == false);
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
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridViewOnReadOnlyChangedDescr))
        ]
        public event EventHandler ReadOnlyChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWREADONLYCHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWREADONLYCHANGED, value);
        }

        private void ResetCurrentCell()
        {
            if (ptCurrentCell.X != -1 &&
                !SetCurrentCellAddressCore(-1, -1, true /*setAnchorCellAddress*/, true /*validateCurrentCell*/, false /*throughMouseClick*/))
            {
                // Edited value couldn't be committed or aborted
                throw new InvalidOperationException(SR.DataGridView_CellChangeCannotBeCommittedOrAborted);
            }
        }

        internal bool ResizingOperationAboutToStart
        {
            get
            {
                return dataGridViewOper[DATAGRIDVIEWOPER_resizingOperationAboutToStart];
            }
        }

        internal bool RightToLeftInternal
        {
            get
            {
                if (dataGridViewState2[DATAGRIDVIEWSTATE2_rightToLeftValid])
                {
                    return dataGridViewState2[DATAGRIDVIEWSTATE2_rightToLeftMode];
                }
                dataGridViewState2[DATAGRIDVIEWSTATE2_rightToLeftMode] = (RightToLeft == RightToLeft.Yes);
                dataGridViewState2[DATAGRIDVIEWSTATE2_rightToLeftValid] = true;
                return dataGridViewState2[DATAGRIDVIEWSTATE2_rightToLeftMode];
            }
        }

        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            EditorBrowsable(EditorBrowsableState.Advanced),
            DefaultValue(0)
        ]
        public int RowCount
        {
            get
            {
                return Rows.Count;
            }
            set
            {
                if (AllowUserToAddRowsInternal)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(RowCount), value, 1));
                    }
                }
                else
                {
                    if (value < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(RowCount), value, 0));
                    }
                }
                if (DataSource != null)
                {
                    throw new InvalidOperationException(SR.DataGridView_CannotSetRowCountOnDataBoundDataGridView);
                }
                if (value != Rows.Count)
                {
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
                        // Some rows need to be appened.
                        if (Columns.Count == 0)
                        {
                            // There are no columns yet, we simply create a single DataGridViewTextBoxColumn.
                            DataGridViewTextBoxColumn dataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
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
        }

        [
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_RowHeadersBorderStyleDescr)),
            Browsable(true),
            DefaultValue(DataGridViewHeaderBorderStyle.Raised)
        ]
        public DataGridViewHeaderBorderStyle RowHeadersBorderStyle
        {
            get
            {
                switch (advancedRowHeadersBorderStyle.All)
                {
                    case DataGridViewAdvancedCellBorderStyle.NotSet:
                        return DataGridViewHeaderBorderStyle.Custom;

                    case DataGridViewAdvancedCellBorderStyle.None:
                        return DataGridViewHeaderBorderStyle.None;

                    case DataGridViewAdvancedCellBorderStyle.Single:
                        return DataGridViewHeaderBorderStyle.Single;

                    case DataGridViewAdvancedCellBorderStyle.InsetDouble:
                        return DataGridViewHeaderBorderStyle.Sunken;

                    case DataGridViewAdvancedCellBorderStyle.OutsetPartial:
                        return DataGridViewHeaderBorderStyle.Raised;

                    default:
                        return DataGridViewHeaderBorderStyle.Custom;
                }
            }
            set
            {
                // Sequential enum.  Valid values are 0x0 to 0x4
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DataGridViewHeaderBorderStyle.Custom, (int)DataGridViewHeaderBorderStyle.None))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridViewHeaderBorderStyle));
                }

                if (value != RowHeadersBorderStyle)
                {
                    if (value == DataGridViewHeaderBorderStyle.Custom)
                    {
                        throw new ArgumentException(string.Format(SR.DataGridView_CustomCellBorderStyleInvalid, "RowHeadersBorderStyle"));
                    }
                    dataGridViewOper[DATAGRIDVIEWOPER_inBorderStyleChange] = true;
                    try
                    {
                        switch (value)
                        {
                            case DataGridViewHeaderBorderStyle.Single:
                                advancedRowHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Single;
                                break;

                            case DataGridViewHeaderBorderStyle.Raised:
                                advancedRowHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.OutsetPartial;
                                break;

                            case DataGridViewHeaderBorderStyle.Sunken:
                                advancedRowHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.InsetDouble;
                                break;

                            case DataGridViewHeaderBorderStyle.None:
                                advancedRowHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                                break;
                        }
                    }
                    finally
                    {
                        dataGridViewOper[DATAGRIDVIEWOPER_inBorderStyleChange] = false;
                    }
                    OnRowHeadersBorderStyleChanged(EventArgs.Empty);
                }
            }
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_RowHeadersBorderStyleChangedDescr))
        ]
        public event EventHandler RowHeadersBorderStyleChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWROWHEADERSBORDERSTYLECHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWROWHEADERSBORDERSTYLECHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_RowHeadersDefaultCellStyleDescr)),
            AmbientValue(null)
        ]
        public DataGridViewCellStyle RowHeadersDefaultCellStyle
        {
            get
            {
                if (rowHeadersDefaultCellStyle == null)
                {
                    rowHeadersDefaultCellStyle = DefaultRowHeadersDefaultCellStyle;
                }
                return rowHeadersDefaultCellStyle;
            }
            set
            {
                DataGridViewCellStyle cs = RowHeadersDefaultCellStyle;
                cs.RemoveScope(DataGridViewCellStyleScopes.RowHeaders);
                rowHeadersDefaultCellStyle = value;
                if (value != null)
                {
                    rowHeadersDefaultCellStyle.AddScope(this, DataGridViewCellStyleScopes.RowHeaders);
                }
                DataGridViewCellStyleDifferences dgvcsc = cs.GetDifferencesFrom(RowHeadersDefaultCellStyle);
                if (dgvcsc != DataGridViewCellStyleDifferences.None)
                {
                    CellStyleChangedEventArgs.ChangeAffectsPreferredSize = (dgvcsc == DataGridViewCellStyleDifferences.AffectPreferredSize);
                    OnRowHeadersDefaultCellStyleChanged(CellStyleChangedEventArgs);
                }
            }
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridViewRowHeadersDefaultCellStyleChangedDescr))
        ]
        public event EventHandler RowHeadersDefaultCellStyleChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWROWHEADERSDEFAULTCELLSTYLECHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWROWHEADERSDEFAULTCELLSTYLECHANGED, value);
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the dataGridView's row headers are
        ///  visible.
        /// </summary>
        [
            SRCategory(nameof(SR.CatAppearance)),
            DefaultValue(true),
            SRDescription(nameof(SR.DataGridViewRowHeadersVisibleDescr))
        ]
        public bool RowHeadersVisible
        {
            get
            {
                return dataGridViewState1[DATAGRIDVIEWSTATE1_rowHeadersVisible];
            }
            set
            {
                if (RowHeadersVisible != value)
                {
                    if (!value &&
                        (autoSizeRowsMode == DataGridViewAutoSizeRowsMode.AllHeaders || autoSizeRowsMode == DataGridViewAutoSizeRowsMode.DisplayedHeaders))
                    {
                        throw new InvalidOperationException(SR.DataGridView_RowHeadersCannotBeInvisible);
                    }
                    using (LayoutTransaction.CreateTransactionIf(AutoSize, ParentInternal, this, PropertyNames.RowHeadersVisible))
                    {
                        dataGridViewState1[DATAGRIDVIEWSTATE1_rowHeadersVisible] = value;
                        layout.RowHeadersVisible = value;
                        displayedBandsInfo.EnsureDirtyState();
                        if (!AutoSize)
                        {
                            PerformLayoutPrivate(false /*useRowShortcut*/, false /*computeVisibleRows*/, true /*invalidInAdjustFillingColumns*/, true /*repositionEditingControl*/);
                        }
                        InvalidateInside();
                        OnRowHeadersGlobalAutoSize(value /*expandingRows*/);
                    }
                }
            }
        }

        [
            SRCategory(nameof(SR.CatLayout)),
            Localizable(true),
            SRDescription(nameof(SR.DataGridView_RowHeadersWidthDescr))
        ]
        public int RowHeadersWidth
        {
            get
            {
                return rowHeadersWidth;
            }
            set
            {
                if (value < minimumRowHeadersWidth)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(RowHeadersWidth), value, minimumRowHeadersWidth));
                }
                if (value > maxHeadersThickness)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidHighBoundArgumentEx, nameof(RowHeadersWidth), value, maxHeadersThickness));
                }

                if (RowHeadersWidthSizeMode != DataGridViewRowHeadersWidthSizeMode.EnableResizing &&
                    RowHeadersWidthSizeMode != DataGridViewRowHeadersWidthSizeMode.DisableResizing)
                {
                    cachedRowHeadersWidth = value;
                }
                else if (rowHeadersWidth != value)
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
                    Debug.Assert(rowHeadersWidth != value);
                    Debug.Assert(value >= minimumRowHeadersWidth);
                    rowHeadersWidth = value;
                    if (AutoSize)
                    {
                        InvalidateInside();
                    }
                    else
                    {
                        if (layout.RowHeadersVisible)
                        {
                            PerformLayoutPrivate(false /*useRowShortcut*/, false /*computeVisibleRows*/, true /*invalidInAdjustFillingColumns*/, true /*repositionEditingControl*/);
                            InvalidateInside();
                        }
                    }
                    OnRowHeadersWidthChanged(EventArgs.Empty);
                }
            }
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridViewRowHeadersWidthChangedDescr))
        ]
        public event EventHandler RowHeadersWidthChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWROWHEADERSWIDTHCHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWROWHEADERSWIDTHCHANGED, value);
        }

        private bool ShouldSerializeRowHeadersWidth()
        {
            return (rowHeadersWidthSizeMode == DataGridViewRowHeadersWidthSizeMode.EnableResizing || rowHeadersWidthSizeMode == DataGridViewRowHeadersWidthSizeMode.DisableResizing) &&
                   defaultRowHeadersWidth != RowHeadersWidth;
        }

        /// <summary>
        ///  Gets or sets a value that determines the behavior for adjusting the row headers width.
        /// </summary>
        [
            DefaultValue(DataGridViewRowHeadersWidthSizeMode.EnableResizing),
            RefreshProperties(RefreshProperties.All),
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_RowHeadersWidthSizeModeDescr))
        ]
        public DataGridViewRowHeadersWidthSizeMode RowHeadersWidthSizeMode
        {
            get
            {
                return rowHeadersWidthSizeMode;
            }
            set
            {
                // Sequential enum.  Valid values are 0x0 to 0x4
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DataGridViewRowHeadersWidthSizeMode.EnableResizing, (int)DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridViewRowHeadersWidthSizeMode));
                }
                if (rowHeadersWidthSizeMode != value)
                {
                    /*if (value != DataGridViewRowHeadersWidthSizeMode.EnableResizing &&
                     *    value != DataGridViewRowHeadersWidthSizeMode.DisableResizing &&
                     *    !this.RowHeadersVisible)
                    {
                        We intentionally don't throw an error because of designer code spit order.
                    }*/
                    DataGridViewAutoSizeModeEventArgs dgvasme = new DataGridViewAutoSizeModeEventArgs(rowHeadersWidthSizeMode != DataGridViewRowHeadersWidthSizeMode.EnableResizing &&
                                                                                                      rowHeadersWidthSizeMode != DataGridViewRowHeadersWidthSizeMode.DisableResizing);
                    rowHeadersWidthSizeMode = value;
                    OnRowHeadersWidthSizeModeChanged(dgvasme);
                }
            }
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_RowHeadersWidthSizeModeChangedDescr))
        ]
        public event DataGridViewAutoSizeModeEventHandler RowHeadersWidthSizeModeChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWROWHEADERSWIDTHSIZEMODECHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWROWHEADERSWIDTHSIZEMODECHANGED, value);
        }

        [
            Browsable(false)
        ]
        public DataGridViewRowCollection Rows
        {
            get
            {
                if (dataGridViewRows == null)
                {
                    dataGridViewRows = CreateRowsInstance();
                }
                return dataGridViewRows;
            }
        }

        [
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_RowsDefaultCellStyleDescr))
        ]
        public DataGridViewCellStyle RowsDefaultCellStyle
        {
            get
            {
                if (rowsDefaultCellStyle == null)
                {
                    rowsDefaultCellStyle = new DataGridViewCellStyle();
                    rowsDefaultCellStyle.AddScope(this, DataGridViewCellStyleScopes.Rows);
                }
                return rowsDefaultCellStyle;
            }
            set
            {
                DataGridViewCellStyle cs = RowsDefaultCellStyle;
                cs.RemoveScope(DataGridViewCellStyleScopes.Rows);
                rowsDefaultCellStyle = value;
                if (value != null)
                {
                    rowsDefaultCellStyle.AddScope(this, DataGridViewCellStyleScopes.Rows);
                }
                DataGridViewCellStyleDifferences dgvcsc = cs.GetDifferencesFrom(RowsDefaultCellStyle);
                if (dgvcsc != DataGridViewCellStyleDifferences.None)
                {
                    CellStyleChangedEventArgs.ChangeAffectsPreferredSize = (dgvcsc == DataGridViewCellStyleDifferences.AffectPreferredSize);
                    OnRowsDefaultCellStyleChanged(CellStyleChangedEventArgs);
                }
            }
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridViewRowsDefaultCellStyleChangedDescr))
        ]
        public event EventHandler RowsDefaultCellStyleChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWROWSDEFAULTCELLSTYLECHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWROWSDEFAULTCELLSTYLECHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatAppearance)),
            Browsable(true),
            SRDescription(nameof(SR.DataGridView_RowTemplateDescr)),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
        ]
        public DataGridViewRow RowTemplate
        {
            get
            {
                if (rowTemplate == null)
                {
                    rowTemplate = new DataGridViewRow();
                }
                return rowTemplate;
            }
            set
            {
                DataGridViewRow dataGridViewRow = value;
                if (dataGridViewRow != null)
                {
                    if (dataGridViewRow.DataGridView != null)
                    {
                        throw new InvalidOperationException(SR.DataGridView_RowAlreadyBelongsToDataGridView);
                    }
                    //if (dataGridViewRow.Selected)
                    //{
                    //    throw new InvalidOperationException(SR.DataGridView_RowTemplateCannotBeSelected);
                    //}
                }
                rowTemplate = dataGridViewRow;
            }
        }

        private bool ShouldSerializeRowTemplate()
        {
            return rowTemplate != null;
        }

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
        [
            DefaultValue(ScrollBars.Both),
            Localizable(true),
            SRCategory(nameof(SR.CatLayout)),
            SRDescription(nameof(SR.DataGridView_ScrollBarsDescr))
        ]
        public ScrollBars ScrollBars
        {
            get
            {
                return scrollBars;
            }
            set
            {
                // Sequential enum.  Valid values are 0x0 to 0x3
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)ScrollBars.None, (int)ScrollBars.Both))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ScrollBars));
                }

                if (scrollBars != value)
                {
                    using (LayoutTransaction.CreateTransactionIf(AutoSize, ParentInternal, this, PropertyNames.ScrollBars))
                    {
                        // Before changing the value of this.scrollBars, we scroll to the top-left cell to
                        // avoid inconsitent state of scrollbars.
                        DataGridViewColumn dataGridViewColumn = Columns.GetFirstColumn(DataGridViewElementStates.Visible);
                        int firstVisibleRowIndex = Rows.GetFirstRow(DataGridViewElementStates.Visible);

                        if (dataGridViewColumn != null && firstVisibleRowIndex != -1)
                        {
                            if (!ScrollIntoView(dataGridViewColumn.Index, firstVisibleRowIndex, false))
                            {
                                throw new InvalidOperationException(SR.DataGridView_CellChangeCannotBeCommittedOrAborted);
                            }
                        }
                        Debug.Assert(HorizontalOffset == 0);
                        Debug.Assert(VerticalOffset == 0);

                        scrollBars = value;

                        if (!AutoSize)
                        {
                            PerformLayoutPrivate(false /*useRowShortcut*/, false /*computeVisibleRows*/, true /*invalidInAdjustFillingColumns*/, true /*repositionEditingControl*/);
                        }
                        Invalidate();
                    }
                }
            }
        }

        [
            Browsable(false)
        ]
        public DataGridViewSelectedCellCollection SelectedCells
        {
            get
            {
                DataGridViewSelectedCellCollection stcc = new DataGridViewSelectedCellCollection();
                switch (SelectionMode)
                {
                    case DataGridViewSelectionMode.CellSelect:
                        {
                            // Note: If we change the design and decide that SelectAll() should use band selection,
                            // we need to add those to the selected cells.
                            stcc.AddCellLinkedList(individualSelectedCells);
                            break;
                        }
                    case DataGridViewSelectionMode.FullColumnSelect:
                    case DataGridViewSelectionMode.ColumnHeaderSelect:
                        {
                            foreach (int columnIndex in selectedBandIndexes)
                            {
                                foreach (DataGridViewRow dataGridViewRow in Rows)   // unshares all rows!
                                {
                                    stcc.Add(dataGridViewRow.Cells[columnIndex]);
                                }
                            }
                            if (SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect)
                            {
                                stcc.AddCellLinkedList(individualSelectedCells);
                            }
                            break;
                        }
                    case DataGridViewSelectionMode.FullRowSelect:
                    case DataGridViewSelectionMode.RowHeaderSelect:
                        {
                            foreach (int rowIndex in selectedBandIndexes)
                            {
                                DataGridViewRow dataGridViewRow = (DataGridViewRow)Rows[rowIndex]; // unshares the selected row
                                foreach (DataGridViewCell dataGridViewCell in dataGridViewRow.Cells)
                                {
                                    stcc.Add(dataGridViewCell);
                                }
                            }
                            if (SelectionMode == DataGridViewSelectionMode.RowHeaderSelect)
                            {
                                stcc.AddCellLinkedList(individualSelectedCells);
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
                DataGridViewSelectedColumnCollection strc = new DataGridViewSelectedColumnCollection();
                switch (SelectionMode)
                {
                    case DataGridViewSelectionMode.CellSelect:
                    case DataGridViewSelectionMode.FullRowSelect:
                    case DataGridViewSelectionMode.RowHeaderSelect:
                        break;
                    case DataGridViewSelectionMode.FullColumnSelect:
                    case DataGridViewSelectionMode.ColumnHeaderSelect:
                        foreach (int columnIndex in selectedBandIndexes)
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
                DataGridViewSelectedRowCollection strc = new DataGridViewSelectedRowCollection();
                switch (SelectionMode)
                {
                    case DataGridViewSelectionMode.CellSelect:
                    case DataGridViewSelectionMode.FullColumnSelect:
                    case DataGridViewSelectionMode.ColumnHeaderSelect:
                        break;
                    case DataGridViewSelectionMode.FullRowSelect:
                    case DataGridViewSelectionMode.RowHeaderSelect:
                        foreach (int rowIndex in selectedBandIndexes)
                        {
                            strc.Add((DataGridViewRow)Rows[rowIndex]); // unshares the selected row
                        }
                        break;
                }
                return strc;
            }
        }

        [
            Browsable(true),
            SRCategory(nameof(SR.CatBehavior)),
            DefaultValue(DataGridViewSelectionMode.RowHeaderSelect),
            SRDescription(nameof(SR.DataGridView_SelectionModeDescr))
        ]
        public DataGridViewSelectionMode SelectionMode
        {
            get
            {
                return selectionMode;
            }
            set
            {
                // Sequential enum.  Valid values are 0x0 to 0x4
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DataGridViewSelectionMode.CellSelect, (int)DataGridViewSelectionMode.ColumnHeaderSelect))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridViewSelectionMode));
                }

                if (SelectionMode != value)
                {
                    if (!dataGridViewState2[DATAGRIDVIEWSTATE2_initializing] &&
                        (value == DataGridViewSelectionMode.FullColumnSelect || value == DataGridViewSelectionMode.ColumnHeaderSelect))
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
                    selectionMode = value;
                }
            }
        }

        [
            DefaultValue(true),
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_ShowCellErrorsDescr))
        ]
        public bool ShowCellErrors
        {
            get
            {
                return dataGridViewState2[DATAGRIDVIEWSTATE2_showCellErrors];
            }
            set
            {
                if (ShowCellErrors != value)
                {
                    dataGridViewState2[DATAGRIDVIEWSTATE2_showCellErrors] = value;

                    // Put this into OnShowCellErrorsChanged if created.
                    if (IsHandleCreated && !DesignMode)
                    {
                        if (value && !ShowRowErrors && !ShowCellToolTips)
                        {
                            // the tool tip hasn't yet been activated
                            // activate it now
                            toolTipControl.Activate(!string.IsNullOrEmpty(toolTipCaption));
                        }

                        if (!value && !ShowRowErrors && !ShowCellToolTips)
                        {
                            // there is no reason to keep the tool tip activated
                            // deactivate it
                            toolTipCaption = string.Empty;
                            toolTipControl.Activate(false /*activate*/);
                        }

                        if (!value && (ShowRowErrors || ShowCellToolTips))
                        {
                            // reset the tool tip
                            toolTipControl.Activate(!string.IsNullOrEmpty(toolTipCaption));
                        }

                        // Some autosizing may have to be applied since the potential presence of error icons influences the preferred sizes.
                        OnGlobalAutoSize();
                    }

                    if (!layout.dirty && !DesignMode)
                    {
                        Invalidate(Rectangle.Union(layout.Data, layout.ColumnHeaders));
                        Invalidate(layout.TopLeftHeader);
                    }
                }
            }
        }

        [
            DefaultValue(true),
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_ShowCellToolTipsDescr))
        ]
        public bool ShowCellToolTips
        {
            get
            {
                return dataGridViewState2[DATAGRIDVIEWSTATE2_showCellToolTips];
            }
            set
            {
                if (ShowCellToolTips != value)
                {
                    dataGridViewState2[DATAGRIDVIEWSTATE2_showCellToolTips] = value;

                    if (IsHandleCreated && !DesignMode)
                    {
                        if (value && !ShowRowErrors && !ShowCellErrors)
                        {
                            // the tool tip hasn't yet been activated
                            // activate it now
                            toolTipControl.Activate(!string.IsNullOrEmpty(toolTipCaption) /*activate*/);
                        }

                        if (!value && !ShowRowErrors && !ShowCellErrors)
                        {
                            // there is no reason to keep the tool tip activated
                            // deactivate it
                            toolTipCaption = string.Empty;
                            toolTipControl.Activate(false /*activate*/);
                        }

                        if (!value && (ShowRowErrors || ShowCellErrors))
                        {
                            bool activate = !string.IsNullOrEmpty(toolTipCaption);
                            Point mouseCoord = System.Windows.Forms.Control.MousePosition;
                            activate &= ClientRectangle.Contains(PointToClient(mouseCoord));

                            // reset the tool tip
                            toolTipControl.Activate(activate);
                        }
                    }

                    if (!layout.dirty && !DesignMode)
                    {
                        Invalidate(layout.Data);
                    }
                }
            }
        }

        [
            DefaultValue(true),
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_ShowEditingIconDescr))
        ]
        public bool ShowEditingIcon
        {
            get
            {
                return dataGridViewState2[DATAGRIDVIEWSTATE2_showEditingIcon];
            }
            set
            {
                if (ShowEditingIcon != value)
                {
                    dataGridViewState2[DATAGRIDVIEWSTATE2_showEditingIcon] = value;

                    // invalidate the row header to pick up the new ShowEditingIcon value
                    if (RowHeadersVisible)
                    {
                        if (VirtualMode || DataSource != null)
                        {
                            if (IsCurrentRowDirty)
                            {
                                Debug.Assert(ptCurrentCell.Y >= 0);
                                InvalidateCellPrivate(-1, ptCurrentCell.Y);
                            }
                        }
                        else
                        {
                            if (IsCurrentCellDirty)
                            {
                                Debug.Assert(ptCurrentCell.Y >= 0);
                                InvalidateCellPrivate(-1, ptCurrentCell.Y);
                            }
                        }
                    }
                }
            }
        }

        [
            DefaultValue(true),
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_ShowRowErrorsDescr))
        ]
        public bool ShowRowErrors
        {
            get
            {
                return dataGridViewState2[DATAGRIDVIEWSTATE2_showRowErrors];
            }
            set
            {
                if (ShowRowErrors != value)
                {
                    dataGridViewState2[DATAGRIDVIEWSTATE2_showRowErrors] = value;

                    if (IsHandleCreated && !DesignMode)
                    {
                        if (value && !ShowCellErrors && !ShowCellToolTips)
                        {
                            // the tool tip hasn't yet been activated
                            // activate it now
                            toolTipControl.Activate(!string.IsNullOrEmpty(toolTipCaption));
                        }

                        if (!value && !ShowCellErrors && !ShowCellToolTips)
                        {
                            // there is no reason to keep the tool tip activated
                            // deactivate it
                            toolTipCaption = string.Empty;
                            toolTipControl.Activate(false /*activate*/);
                        }

                        if (!value && (ShowCellErrors || ShowCellToolTips))
                        {
                            // reset the tool tip
                            toolTipControl.Activate(!string.IsNullOrEmpty(toolTipCaption));
                        }
                    }

                    if (!layout.dirty && !DesignMode)
                    {
                        Invalidate(layout.RowHeaders);
                    }
                }
            }
        }

        internal bool SingleHorizontalBorderAdded
        {
            get
            {
                return !layout.ColumnHeadersVisible &&
                    (AdvancedCellBorderStyle.All == DataGridViewAdvancedCellBorderStyle.Single ||
                     CellBorderStyle == DataGridViewCellBorderStyle.SingleHorizontal);
            }
        }

        internal bool SingleVerticalBorderAdded
        {
            get
            {
                return !layout.RowHeadersVisible &&
                    (AdvancedCellBorderStyle.All == DataGridViewAdvancedCellBorderStyle.Single ||
                     CellBorderStyle == DataGridViewCellBorderStyle.SingleVertical);
            }
        }

        [
            Browsable(false)
        ]
        public DataGridViewColumn SortedColumn
        {
            get
            {
                return sortedColumn;
            }
        }

        [
            Browsable(false)
        ]
        public SortOrder SortOrder
        {
            get
            {
                return sortOrder;
            }
        }

        [
            SRCategory(nameof(SR.CatBehavior)),
            DefaultValue(false),
            EditorBrowsable(EditorBrowsableState.Advanced),
            SRDescription(nameof(SR.DataGridView_StandardTabDescr))
        ]
        public bool StandardTab
        {
            get
            {
                return dataGridViewState1[DATAGRIDVIEWSTATE1_standardTab];
            }
            set
            {
                if (dataGridViewState1[DATAGRIDVIEWSTATE1_standardTab] != value)
                {
                    dataGridViewState1[DATAGRIDVIEWSTATE1_standardTab] = value;
                    //OnStandardTabChanged(EventArgs.Empty);
                }
            }
        }

        internal override bool SupportsUiaProviders => true;

        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Never),
            Bindable(false)
        ]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Never)
        ]
        new public event EventHandler TextChanged
        {
            add => base.TextChanged += value;
            remove => base.TextChanged -= value;
        }

        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        ]
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

        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
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

        private string ToolTipPrivate
        {
            get
            {
                return toolTipCaption;
            }
        }

        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public DataGridViewHeaderCell TopLeftHeaderCell
        {
            get
            {
                if (topLeftHeaderCell == null)
                {
                    TopLeftHeaderCell = new DataGridViewTopLeftHeaderCell();
                }
                return topLeftHeaderCell;
            }
            set
            {
                if (topLeftHeaderCell != value)
                {
                    if (topLeftHeaderCell != null)
                    {
                        // Detach existing header cell
                        topLeftHeaderCell.DataGridView = null;
                    }
                    topLeftHeaderCell = value;
                    if (value != null)
                    {
                        topLeftHeaderCell.DataGridView = this;
                    }
                    if (ColumnHeadersVisible && RowHeadersVisible)
                    {
                        // If headers (rows or columns) are autosized, then this.RowHeadersWidth or this.ColumnHeadersHeight
                        // must be updated based on new cell preferred size
                        OnColumnHeadersGlobalAutoSize();
                        // In all cases, the top left cell needs to repaint
                        Invalidate(new Rectangle(layout.Inside.X, layout.Inside.Y, RowHeadersWidth, ColumnHeadersHeight));
                    }
                }
            }
        }

        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public Cursor UserSetCursor
        {
            get
            {
                if (dataGridViewState1[DATAGRIDVIEWSTATE1_customCursorSet])
                {
                    return oldCursor;
                }
                else
                {
                    return Cursor;
                }
            }
        }

        internal int VerticalOffset
        {
            get
            {
                return verticalOffset;
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                int totalVisibleFrozenHeight = Rows.GetRowsHeight(DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen);
                int fittingTrailingScrollingRowsHeight = ComputeHeightOfFittingTrailingScrollingRows(totalVisibleFrozenHeight);
                if (value > vertScrollBar.Maximum - fittingTrailingScrollingRowsHeight)
                {
                    value = vertScrollBar.Maximum - fittingTrailingScrollingRowsHeight;
                }
                if (value == verticalOffset)
                {
                    return;
                }

                int change = value - verticalOffset;
                if (vertScrollBar.Enabled)
                {
                    vertScrollBar.Value = value;
                }
                ScrollRowsByHeight(change); // calculate how many rows need to be scrolled based on 'change'
            }
        }

        protected ScrollBar VerticalScrollBar
        {
            get
            {
                return vertScrollBar;
            }
        }

        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public int VerticalScrollingOffset
        {
            get
            {
                return verticalOffset;
            }
        }

        private Timer VertScrollTimer
        {
            get
            {
                if (vertScrollTimer == null)
                {
                    vertScrollTimer = new Timer();
                    vertScrollTimer.Tick += new EventHandler(VertScrollTimer_Tick);
                }
                return vertScrollTimer;
            }
        }

        [
            SRCategory(nameof(SR.CatBehavior)),
            DefaultValue(false),
            EditorBrowsable(EditorBrowsableState.Advanced),
            SRDescription(nameof(SR.DataGridViewVirtualModeDescr))
        ]
        public bool VirtualMode
        {
            get
            {
                return dataGridViewState1[DATAGRIDVIEWSTATE1_virtualMode];
            }
            set
            {
                if (dataGridViewState1[DATAGRIDVIEWSTATE1_virtualMode] != value)
                {
                    dataGridViewState1[DATAGRIDVIEWSTATE1_virtualMode] = value;
                    InvalidateRowHeights();
                    //OnVirtualModeChanged(EventArgs.Empty);
                }
            }
        }

        private bool VisibleCellExists
        {
            get
            {
                if (null == Columns.GetFirstColumn(DataGridViewElementStates.Visible))
                {
                    return false;
                }
                return -1 != Rows.GetFirstRow(DataGridViewElementStates.Visible);
            }
        }

        // Events start here

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridViewAutoSizeColumnModeChangedDescr))
        ]
        public event DataGridViewAutoSizeColumnModeEventHandler AutoSizeColumnModeChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWAUTOSIZECOLUMNMODECHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWAUTOSIZECOLUMNMODECHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatAction)),
            SRDescription(nameof(SR.DataGridView_CancelRowEditDescr))
        ]
        public event QuestionEventHandler CancelRowEdit
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCANCELROWEDIT, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCANCELROWEDIT, value);
        }

        [
            SRCategory(nameof(SR.CatData)),
            SRDescription(nameof(SR.DataGridView_CellBeginEditDescr))
        ]
        public event DataGridViewCellCancelEventHandler CellBeginEdit
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLBEGINEDIT, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLBEGINEDIT, value);
        }

        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_CellClickDescr))
        ]
        public event DataGridViewCellEventHandler CellClick
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLCLICK, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLCLICK, value);
        }

        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_CellContentClick))
        ]
        public event DataGridViewCellEventHandler CellContentClick
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLCONTENTCLICK, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLCONTENTCLICK, value);
        }

        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_CellContentDoubleClick))
        ]
        public event DataGridViewCellEventHandler CellContentDoubleClick
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLCONTENTDOUBLECLICK, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLCONTENTDOUBLECLICK, value);
        }

        [
            SRCategory(nameof(SR.CatAction)),
            SRDescription(nameof(SR.DataGridView_CellContextMenuStripChanged)),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public event DataGridViewCellEventHandler CellContextMenuStripChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLCONTEXTMENUSTRIPCHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLCONTEXTMENUSTRIPCHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_CellContextMenuStripNeeded)),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public event DataGridViewCellContextMenuStripNeededEventHandler CellContextMenuStripNeeded
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLCONTEXTMENUSTRIPNEEDED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLCONTEXTMENUSTRIPNEEDED, value);
        }

        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_CellDoubleClickDescr))
        ]
        public event DataGridViewCellEventHandler CellDoubleClick
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLDOUBLECLICK, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLDOUBLECLICK, value);
        }

        [
            SRCategory(nameof(SR.CatData)),
            SRDescription(nameof(SR.DataGridView_CellEndEditDescr))
        ]
        public event DataGridViewCellEventHandler CellEndEdit
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLENDEDIT, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLENDEDIT, value);
        }

        [
            SRCategory(nameof(SR.CatFocus)),
            SRDescription(nameof(SR.DataGridView_CellEnterDescr))
        ]
        public event DataGridViewCellEventHandler CellEnter
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLENTER, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLENTER, value);
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_CellErrorTextChangedDescr))
        ]
        public event DataGridViewCellEventHandler CellErrorTextChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLERRORTEXTCHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLERRORTEXTCHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatData)),
            EditorBrowsable(EditorBrowsableState.Advanced),
            SRDescription(nameof(SR.DataGridView_CellErrorTextNeededDescr))
        ]
        public event DataGridViewCellErrorTextNeededEventHandler CellErrorTextNeeded
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLERRORTEXTNEEDED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLERRORTEXTNEEDED, value);
        }

        [
            SRCategory(nameof(SR.CatDisplay)),
            SRDescription(nameof(SR.DataGridView_CellFormattingDescr))
        ]
        public event DataGridViewCellFormattingEventHandler CellFormatting
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLFORMATTING, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLFORMATTING, value);
        }

        [
            SRCategory(nameof(SR.CatFocus)),
            SRDescription(nameof(SR.DataGridView_CellLeaveDescr))
        ]
        public event DataGridViewCellEventHandler CellLeave
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLLEAVE, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLLEAVE, value);
        }

        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_CellMouseClickDescr))
        ]
        public event DataGridViewCellMouseEventHandler CellMouseClick
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLMOUSECLICK, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLMOUSECLICK, value);
        }

        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_CellMouseDoubleClickDescr))
        ]
        public event DataGridViewCellMouseEventHandler CellMouseDoubleClick
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLMOUSEDOUBLECLICK, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLMOUSEDOUBLECLICK, value);
        }

        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_CellMouseDownDescr))
        ]
        public event DataGridViewCellMouseEventHandler CellMouseDown
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLMOUSEDOWN, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLMOUSEDOWN, value);
        }

        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_CellMouseEnterDescr))
        ]
        public event DataGridViewCellEventHandler CellMouseEnter
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLMOUSEENTER, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLMOUSEENTER, value);
        }

        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_CellMouseLeaveDescr))
        ]
        public event DataGridViewCellEventHandler CellMouseLeave
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLMOUSELEAVE, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLMOUSELEAVE, value);
        }

        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_CellMouseMoveDescr))
        ]
        public event DataGridViewCellMouseEventHandler CellMouseMove
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLMOUSEMOVE, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLMOUSEMOVE, value);
        }

        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_CellMouseUpDescr))
        ]
        public event DataGridViewCellMouseEventHandler CellMouseUp
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLMOUSEUP, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLMOUSEUP, value);
        }

        [
            SRCategory(nameof(SR.CatDisplay)),
            SRDescription(nameof(SR.DataGridView_CellPaintingDescr))
        ]
        public event DataGridViewCellPaintingEventHandler CellPainting
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLPAINTING, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLPAINTING, value);
        }

        [
            SRCategory(nameof(SR.CatDisplay)),
            SRDescription(nameof(SR.DataGridView_CellParsingDescr))
        ]
        public event DataGridViewCellParsingEventHandler CellParsing
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLPARSING, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLPARSING, value);
        }

        [
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_CellStateChangedDescr))
        ]
        public event DataGridViewCellStateChangedEventHandler CellStateChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLSTATECHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLSTATECHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_CellStyleChangedDescr))
        ]
        public event DataGridViewCellEventHandler CellStyleChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLSTYLECHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLSTYLECHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_CellStyleContentChangedDescr))
        ]
        public event DataGridViewCellStyleContentChangedEventHandler CellStyleContentChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLSTYLECONTENTCHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLSTYLECONTENTCHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_CellToolTipTextChangedDescr))
        ]
        public event DataGridViewCellEventHandler CellToolTipTextChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLTOOLTIPTEXTCHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLTOOLTIPTEXTCHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_CellToolTipTextNeededDescr)),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public event DataGridViewCellToolTipTextNeededEventHandler CellToolTipTextNeeded
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLTOOLTIPTEXTNEEDED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLTOOLTIPTEXTNEEDED, value);
        }

        [
            SRCategory(nameof(SR.CatFocus)),
            SRDescription(nameof(SR.DataGridView_CellValidatedDescr))
        ]
        public event DataGridViewCellEventHandler CellValidated
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLVALIDATED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLVALIDATED, value);
        }

        [
            SRCategory(nameof(SR.CatFocus)),
            SRDescription(nameof(SR.DataGridView_CellValidatingDescr))
        ]
        public event DataGridViewCellValidatingEventHandler CellValidating
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLVALIDATING, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLVALIDATING, value);
        }

        [
            SRCategory(nameof(SR.CatAction)),
            SRDescription(nameof(SR.DataGridView_CellValueChangedDescr))
        ]
        public event DataGridViewCellEventHandler CellValueChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLVALUECHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLVALUECHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatData)),
            EditorBrowsable(EditorBrowsableState.Advanced),
            SRDescription(nameof(SR.DataGridView_CellValueNeededDescr))
        ]
        public event DataGridViewCellValueEventHandler CellValueNeeded
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLVALUENEEDED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLVALUENEEDED, value);
        }

        [
            SRCategory(nameof(SR.CatData)),
            EditorBrowsable(EditorBrowsableState.Advanced),
            SRDescription(nameof(SR.DataGridView_CellValuePushedDescr))
        ]
        public event DataGridViewCellValueEventHandler CellValuePushed
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCELLVALUEPUSHED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLVALUEPUSHED, value);
        }

        [
            SRCategory(nameof(SR.CatAction)),
            SRDescription(nameof(SR.DataGridView_ColumnAddedDescr))
        ]
        public event DataGridViewColumnEventHandler ColumnAdded
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNADDED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNADDED, value);
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_ColumnContextMenuStripChangedDescr))
        ]
        public event DataGridViewColumnEventHandler ColumnContextMenuStripChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNCONTEXTMENUSTRIPCHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNCONTEXTMENUSTRIPCHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_ColumnDataPropertyNameChangedDescr))
        ]
        public event DataGridViewColumnEventHandler ColumnDataPropertyNameChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNDATAPROPERTYNAMECHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNDATAPROPERTYNAMECHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_ColumnDefaultCellStyleChangedDescr))
        ]
        public event DataGridViewColumnEventHandler ColumnDefaultCellStyleChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNDEFAULTCELLSTYLECHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNDEFAULTCELLSTYLECHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_ColumnDisplayIndexChangedDescr))
        ]
        public event DataGridViewColumnEventHandler ColumnDisplayIndexChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNDISPLAYINDEXCHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNDISPLAYINDEXCHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_ColumnDividerDoubleClickDescr))
        ]
        public event DataGridViewColumnDividerDoubleClickEventHandler ColumnDividerDoubleClick
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNDIVIDERDOUBLECLICK, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNDIVIDERDOUBLECLICK, value);
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_ColumnDividerWidthChangedDescr))
        ]
        public event DataGridViewColumnEventHandler ColumnDividerWidthChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNDIVIDERWIDTHCHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNDIVIDERWIDTHCHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_ColumnHeaderMouseClickDescr))
        ]
        public event DataGridViewCellMouseEventHandler ColumnHeaderMouseClick
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNHEADERMOUSECLICK, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNHEADERMOUSECLICK, value);
        }

        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_ColumnHeaderMouseDoubleClickDescr))
        ]
        public event DataGridViewCellMouseEventHandler ColumnHeaderMouseDoubleClick
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNHEADERMOUSEDOUBLECLICK, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNHEADERMOUSEDOUBLECLICK, value);
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_ColumnHeaderCellChangedDescr))
        ]
        public event DataGridViewColumnEventHandler ColumnHeaderCellChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNHEADERCELLCHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNHEADERCELLCHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_ColumnMinimumWidthChangedDescr))
        ]
        public event DataGridViewColumnEventHandler ColumnMinimumWidthChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNMINIMUMWIDTHCHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNMINIMUMWIDTHCHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_ColumnNameChangedDescr))
        ]
        public event DataGridViewColumnEventHandler ColumnNameChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNNAMECHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNNAMECHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatAction)),
            SRDescription(nameof(SR.DataGridView_ColumnRemovedDescr))
        ]
        public event DataGridViewColumnEventHandler ColumnRemoved
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNREMOVED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNREMOVED, value);
        }

        /*
        public event EventHandler ColumnsDefaultCellStyleChanged
        {
            add => this.Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNSDEFAULTCELLSTYLECHANGED, value);
            remove => this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNSDEFAULTCELLSTYLECHANGED, value);
        }*/

        [
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridViewColumnSortModeChangedDescr))
        ]
        public event DataGridViewColumnEventHandler ColumnSortModeChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNSORTMODECHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNSORTMODECHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_ColumnStateChangedDescr))
        ]
        public event DataGridViewColumnStateChangedEventHandler ColumnStateChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNSTATECHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNSTATECHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_ColumnToolTipTextChangedDescr))
        ]
        public event DataGridViewColumnEventHandler ColumnToolTipTextChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNTOOLTIPTEXTCHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNTOOLTIPTEXTCHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatAction)),
            SRDescription(nameof(SR.DataGridView_ColumnWidthChangedDescr))
        ]
        public event DataGridViewColumnEventHandler ColumnWidthChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNWIDTHCHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNWIDTHCHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatAction)),
            SRDescription(nameof(SR.DataGridView_CurrentCellChangedDescr))
        ]
        public event EventHandler CurrentCellChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCURRENTCELLCHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCURRENTCELLCHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatBehavior)),
            EditorBrowsable(EditorBrowsableState.Advanced),
            SRDescription(nameof(SR.DataGridView_CurrentCellDirtyStateChangedDescr))
        ]
        public event EventHandler CurrentCellDirtyStateChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWCURRENTCELLDIRTYSTATECHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWCURRENTCELLDIRTYSTATECHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatData)),
            SRDescription(nameof(SR.DataGridView_DataBindingCompleteDescr))
        ]
        public event DataGridViewBindingCompleteEventHandler DataBindingComplete
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWDATABINDINGCOMPLETE, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWDATABINDINGCOMPLETE, value);
        }

        [
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_DataErrorDescr))
        ]
        public event DataGridViewDataErrorEventHandler DataError
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWDATAERROR, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWDATAERROR, value);
        }

        [
            SRCategory(nameof(SR.CatData)),
            EditorBrowsable(EditorBrowsableState.Advanced),
            SRDescription(nameof(SR.DataGridView_DefaultValuesNeededDescr))
        ]
        public event DataGridViewRowEventHandler DefaultValuesNeeded
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWDEFAULTVALUESNEEDED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWDEFAULTVALUESNEEDED, value);
        }

        [
            SRCategory(nameof(SR.CatAction)),
            SRDescription(nameof(SR.DataGridView_EditingControlShowingDescr))
        ]
        public event DataGridViewEditingControlShowingEventHandler EditingControlShowing
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWEDITINGCONTROLSHOWING, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWEDITINGCONTROLSHOWING, value);
        }

        /*
        public event QuestionEventHandler KeepNewRow
        {
            add => this.Events.AddHandler(EVENT_DATAGRIDVIEWKEEPNEWROW, value);
            remove => this.Events.RemoveHandler(EVENT_DATAGRIDVIEWKEEPNEWROW, value);
        }*/

        /*
        public event EventHandler NewRowDiscarded
        {
            add => this.Events.AddHandler(EVENT_DATAGRIDVIEWNEWROWDISCARDED, value);
            remove => this.Events.RemoveHandler(EVENT_DATAGRIDVIEWNEWROWDISCARDED, value);
        }*/

        [
            SRCategory(nameof(SR.CatData)),
            SRDescription(nameof(SR.DataGridView_NewRowNeededDescr))
        ]
        public event DataGridViewRowEventHandler NewRowNeeded
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWNEWROWNEEDED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWNEWROWNEEDED, value);
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_RowContextMenuStripChangedDescr))
        ]
        public event DataGridViewRowEventHandler RowContextMenuStripChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWROWCONTEXTMENUSTRIPCHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWROWCONTEXTMENUSTRIPCHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatData)),
            EditorBrowsable(EditorBrowsableState.Advanced),
            SRDescription(nameof(SR.DataGridView_RowContextMenuStripNeededDescr))
        ]
        public event DataGridViewRowContextMenuStripNeededEventHandler RowContextMenuStripNeeded
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWROWCONTEXTMENUSTRIPNEEDED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWROWCONTEXTMENUSTRIPNEEDED, value);
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_RowDefaultCellStyleChangedDescr))
        ]
        public event DataGridViewRowEventHandler RowDefaultCellStyleChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWROWDEFAULTCELLSTYLECHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWROWDEFAULTCELLSTYLECHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatData)),
            EditorBrowsable(EditorBrowsableState.Advanced),
            SRDescription(nameof(SR.DataGridView_RowDirtyStateNeededDescr))
        ]
        public event QuestionEventHandler RowDirtyStateNeeded
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWROWDIRTYSTATENEEDED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWROWDIRTYSTATENEEDED, value);
        }

        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_RowDividerDoubleClickDescr))
        ]
        public event DataGridViewRowDividerDoubleClickEventHandler RowDividerDoubleClick
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWROWDIVIDERDOUBLECLICK, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWROWDIVIDERDOUBLECLICK, value);
        }

        [
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_RowDividerHeightChangedDescr))
        ]
        public event DataGridViewRowEventHandler RowDividerHeightChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWROWDIVIDERHEIGHTCHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWROWDIVIDERHEIGHTCHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatFocus)),
            SRDescription(nameof(SR.DataGridView_RowEnterDescr))
        ]
        public event DataGridViewCellEventHandler RowEnter
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWROWENTER, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWROWENTER, value);
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_RowErrorTextChangedDescr))
        ]
        public event DataGridViewRowEventHandler RowErrorTextChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWROWERRORTEXTCHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWROWERRORTEXTCHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatData)),
            EditorBrowsable(EditorBrowsableState.Advanced),
            SRDescription(nameof(SR.DataGridView_RowErrorTextNeededDescr))
        ]
        public event DataGridViewRowErrorTextNeededEventHandler RowErrorTextNeeded
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWROWERRORTEXTNEEDED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWROWERRORTEXTNEEDED, value);
        }

        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_RowHeaderMouseClickDescr))
        ]
        public event DataGridViewCellMouseEventHandler RowHeaderMouseClick
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWROWHEADERMOUSECLICK, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWROWHEADERMOUSECLICK, value);
        }

        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_RowHeaderMouseDoubleClickDescr))
        ]
        public event DataGridViewCellMouseEventHandler RowHeaderMouseDoubleClick
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWROWHEADERMOUSEDOUBLECLICK, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWROWHEADERMOUSEDOUBLECLICK, value);
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_RowHeaderCellChangedDescr))
        ]
        public event DataGridViewRowEventHandler RowHeaderCellChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWROWHEADERCELLCHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWROWHEADERCELLCHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_RowHeightChangedDescr))
        ]
        public event DataGridViewRowEventHandler RowHeightChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWROWHEIGHTCHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWROWHEIGHTCHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatData)),
            EditorBrowsable(EditorBrowsableState.Advanced),
            SRDescription(nameof(SR.DataGridView_RowHeightInfoNeededDescr))
        ]
        public event DataGridViewRowHeightInfoNeededEventHandler RowHeightInfoNeeded
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWROWHEIGHTINFONEEDED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWROWHEIGHTINFONEEDED, value);
        }

        internal DataGridViewRowHeightInfoNeededEventArgs RowHeightInfoNeededEventArgs
        {
            get
            {
                if (dgvrhine == null)
                {
                    dgvrhine = new DataGridViewRowHeightInfoNeededEventArgs();
                }
                return dgvrhine;
            }
        }

        [
            SRCategory(nameof(SR.CatData)),
            EditorBrowsable(EditorBrowsableState.Advanced),
            SRDescription(nameof(SR.DataGridView_RowHeightInfoPushedDescr))
        ]
        public event DataGridViewRowHeightInfoPushedEventHandler RowHeightInfoPushed
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWROWHEIGHTINFOPUSHED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWROWHEIGHTINFOPUSHED, value);
        }

        [
            SRCategory(nameof(SR.CatFocus)),
            SRDescription(nameof(SR.DataGridView_RowLeaveDescr))
        ]
        public event DataGridViewCellEventHandler RowLeave
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWROWLEAVE, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWROWLEAVE, value);
        }

        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_RowMinimumHeightChangedDescr))
        ]
        public event DataGridViewRowEventHandler RowMinimumHeightChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWROWMINIMUMHEIGHTCHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWROWMINIMUMHEIGHTCHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatDisplay)),
            SRDescription(nameof(SR.DataGridView_RowPostPaintDescr))
        ]
        public event DataGridViewRowPostPaintEventHandler RowPostPaint
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWROWPOSTPAINT, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWROWPOSTPAINT, value);
        }

        internal DataGridViewRowPostPaintEventArgs RowPostPaintEventArgs
        {
            get
            {
                if (dgvrpope == null)
                {
                    dgvrpope = new DataGridViewRowPostPaintEventArgs(this);
                }
                return dgvrpope;
            }
        }

        [
            SRCategory(nameof(SR.CatDisplay)),
            SRDescription(nameof(SR.DataGridView_RowPrePaintDescr))
        ]
        public event DataGridViewRowPrePaintEventHandler RowPrePaint
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWROWPREPAINT, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWROWPREPAINT, value);
        }

        internal DataGridViewRowPrePaintEventArgs RowPrePaintEventArgs
        {
            get
            {
                if (dgvrprpe == null)
                {
                    dgvrprpe = new DataGridViewRowPrePaintEventArgs(this);
                }
                return dgvrprpe;
            }
        }

        [
            SRCategory(nameof(SR.CatAction)),
            SRDescription(nameof(SR.DataGridView_RowsAddedDescr))
        ]
        public event DataGridViewRowsAddedEventHandler RowsAdded
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWROWSADDED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWROWSADDED, value);
        }

        [
            SRCategory(nameof(SR.CatAction)),
            SRDescription(nameof(SR.DataGridView_RowsRemovedDescr))
        ]
        public event DataGridViewRowsRemovedEventHandler RowsRemoved
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWROWSREMOVED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWROWSREMOVED, value);
        }

        [
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_RowStateChangedDescr))
        ]
        public event DataGridViewRowStateChangedEventHandler RowStateChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWROWSTATECHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWROWSTATECHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatBehavior)),
            EditorBrowsable(EditorBrowsableState.Advanced),
            SRDescription(nameof(SR.DataGridView_RowUnsharedDescr))
        ]
        public event DataGridViewRowEventHandler RowUnshared
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWROWUNSHARED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWROWUNSHARED, value);
        }

        [
            SRCategory(nameof(SR.CatFocus)),
            SRDescription(nameof(SR.DataGridView_RowValidatedDescr))
        ]
        public event DataGridViewCellEventHandler RowValidated
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWROWVALIDATED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWROWVALIDATED, value);
        }

        [
            SRCategory(nameof(SR.CatFocus)),
            SRDescription(nameof(SR.DataGridView_RowValidatingDescr))
        ]
        public event DataGridViewCellCancelEventHandler RowValidating
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWROWVALIDATING, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWROWVALIDATING, value);
        }

        [
            SRCategory(nameof(SR.CatAction)),
            SRDescription(nameof(SR.DataGridView_ScrollDescr))
        ]
        public event ScrollEventHandler Scroll
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWSCROLL, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWSCROLL, value);
        }

        [
            SRCategory(nameof(SR.CatAction)),
            SRDescription(nameof(SR.DataGridView_SelectionChangedDescr))
        ]
        public event EventHandler SelectionChanged
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWSELECTIONCHANGED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWSELECTIONCHANGED, value);
        }

        [
            SRCategory(nameof(SR.CatData)),
            EditorBrowsable(EditorBrowsableState.Advanced),
            SRDescription(nameof(SR.DataGridView_SortCompareDescr))
        ]
        public event DataGridViewSortCompareEventHandler SortCompare
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWSORTCOMPARE, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWSORTCOMPARE, value);
        }

        [
            SRCategory(nameof(SR.CatData)),
            SRDescription(nameof(SR.DataGridView_SortedDescr))
        ]
        public event EventHandler Sorted
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWSORTED, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWSORTED, value);
        }

        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Never)
        ]
        new public event EventHandler StyleChanged
        {
            add => base.StyleChanged += value;
            remove => base.StyleChanged -= value;
        }

        [
            SRCategory(nameof(SR.CatAction)),
            SRDescription(nameof(SR.DataGridView_UserAddedRowDescr))
        ]
        public event DataGridViewRowEventHandler UserAddedRow
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWUSERADDEDROW, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWUSERADDEDROW, value);
        }

        [
            SRCategory(nameof(SR.CatAction)),
            SRDescription(nameof(SR.DataGridView_UserDeletedRowDescr))
        ]
        public event DataGridViewRowEventHandler UserDeletedRow
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWUSERDELETEDROW, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWUSERDELETEDROW, value);
        }

        [
            SRCategory(nameof(SR.CatAction)),
            SRDescription(nameof(SR.DataGridView_UserDeletingRowDescr))
        ]
        public event DataGridViewRowCancelEventHandler UserDeletingRow
        {
            add => Events.AddHandler(EVENT_DATAGRIDVIEWUSERDELETINGROW, value);
            remove => Events.RemoveHandler(EVENT_DATAGRIDVIEWUSERDELETINGROW, value);
        }

        ////////////////////////
        //                    //
        // ISupportInitialize //
        //                    //
        ////////////////////////
        void ISupportInitialize.BeginInit()
        {
            if (dataGridViewState2[DATAGRIDVIEWSTATE2_initializing])
            {
                throw new InvalidOperationException(SR.DataGridViewBeginInit);
            }

            dataGridViewState2[DATAGRIDVIEWSTATE2_initializing] = true;
        }

        void ISupportInitialize.EndInit()
        {
            dataGridViewState2[DATAGRIDVIEWSTATE2_initializing] = false;

            foreach (DataGridViewColumn dataGridViewColumn in Columns)
            {
                if (dataGridViewColumn.Frozen &&
                    dataGridViewColumn.Visible &&
                    dataGridViewColumn.InheritedAutoSizeMode == DataGridViewAutoSizeColumnMode.Fill)
                {
                    dataGridViewColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                }
            }

            DataGridViewSelectionMode selectionMode = SelectionMode;
            if (selectionMode == DataGridViewSelectionMode.FullColumnSelect || selectionMode == DataGridViewSelectionMode.ColumnHeaderSelect)
            {
                foreach (DataGridViewColumn dataGridViewColumn in Columns)
                {
                    if (dataGridViewColumn.SortMode == DataGridViewColumnSortMode.Automatic)
                    {
                        // Resetting SelectionMode to its acceptable default value. We don't want the control to ever end up in an invalid state.
                        SelectionMode = defaultSelectionMode; // DataGridViewSelectionMode.RowHeaderSelect
                        throw new InvalidOperationException(string.Format(SR.DataGridView_SelectionModeReset,
                                                                         string.Format(SR.DataGridView_SelectionModeAndSortModeClash, (selectionMode).ToString()),
                                                                         (defaultSelectionMode).ToString()));
                    }
                }
            }
        }

        /* INTERNAL ENUMERATIONS */

        internal enum DataGridViewHitTestTypeInternal
        {
            None,
            Cell,
            ColumnHeader,
            RowHeader,
            ColumnResizeLeft,
            ColumnResizeRight,
            RowResizeTop,
            RowResizeBottom,
            FirstColumnHeaderLeft,
            TopLeftHeader,
            TopLeftHeaderResizeLeft,
            TopLeftHeaderResizeRight,
            TopLeftHeaderResizeTop,
            TopLeftHeaderResizeBottom,
            ColumnHeadersResizeBottom,
            ColumnHeadersResizeTop,
            RowHeadersResizeRight,
            RowHeadersResizeLeft,
            ColumnHeaderLeft,
            ColumnHeaderRight
        }

        internal enum DataGridViewValidateCellInternal
        {
            Never,
            Always,
            WhenChanged
        }

        private enum DataGridViewMouseEvent
        {
            Click,
            DoubleClick,
            MouseClick,
            MouseDoubleClick,
            MouseDown,
            MouseUp,
            MouseMove
        }

        private struct MouseClickInfo
        {
            public MouseButtons button;
            public long timeStamp;
            public int x;
            public int y;
            public int col;
            public int row;
        }

        internal class DataGridViewEditingPanel : Panel
        {
            private readonly DataGridView owningDataGridView;

            public DataGridViewEditingPanel(DataGridView owningDataGridView)
            {
                this.owningDataGridView = owningDataGridView;
            }

            internal override bool SupportsUiaProviders => true;

            protected override AccessibleObject CreateAccessibilityInstance()
            {
                return new DataGridViewEditingPanelAccessibleObject(owningDataGridView, this);
            }
        }
    }
}
