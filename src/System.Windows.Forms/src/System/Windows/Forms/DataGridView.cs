// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System.Text;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.ComponentModel;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Collections;
    using System.Windows.Forms;
    using System.Windows.Forms.Design;
    using System.ComponentModel.Design;
    using System.Drawing;
    using System.Windows.Forms.ComponentModel;
    using System.Windows.Forms.Layout;
    using System.Globalization;
    using System.Diagnostics;
    using System.Windows.Forms.VisualStyles;
    using Microsoft.Win32;
    using System.Collections.Specialized;


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

        private const int DATAGRIDVIEWSTATE1_allowUserToAddRows           = 0x00000001;
        private const int DATAGRIDVIEWSTATE1_allowUserToDeleteRows        = 0x00000002;
        private const int DATAGRIDVIEWSTATE1_allowUserToOrderColumns      = 0x00000004;
        private const int DATAGRIDVIEWSTATE1_columnHeadersVisible         = 0x00000008;
        private const int DATAGRIDVIEWSTATE1_rowHeadersVisible            = 0x00000010;
        private const int DATAGRIDVIEWSTATE1_forwardCharMessage           = 0x00000020;
        private const int DATAGRIDVIEWSTATE1_leavingWithTabKey            = 0x00000040;
        private const int DATAGRIDVIEWSTATE1_multiSelect                  = 0x00000080;
        private const int DATAGRIDVIEWSTATE1_ignoringEditingChanges       = 0x00000200;
        private const int DATAGRIDVIEWSTATE1_ambientForeColor             = 0x00000400;
        private const int DATAGRIDVIEWSTATE1_scrolledSinceMouseDown       = 0x00000800;
        private const int DATAGRIDVIEWSTATE1_editingControlHidden         = 0x00001000;
        private const int DATAGRIDVIEWSTATE1_standardTab                  = 0x00002000;
        private const int DATAGRIDVIEWSTATE1_editingControlChanging       = 0x00004000;
        private const int DATAGRIDVIEWSTATE1_currentCellInEditMode        = 0x00008000;
        private const int DATAGRIDVIEWSTATE1_virtualMode                  = 0x00010000;
        private const int DATAGRIDVIEWSTATE1_editedCellChanged            = 0x00020000;
        private const int DATAGRIDVIEWSTATE1_editedRowChanged             = 0x00040000;
        private const int DATAGRIDVIEWSTATE1_newRowEdited                 = 0x00080000;
        private const int DATAGRIDVIEWSTATE1_readOnly                     = 0x00100000;
        private const int DATAGRIDVIEWSTATE1_newRowCreatedByEditing       = 0x00200000;
        private const int DATAGRIDVIEWSTATE1_temporarilyResetCurrentCell  = 0x00400000;
        private const int DATAGRIDVIEWSTATE1_autoGenerateColumns          = 0x00800000;
        private const int DATAGRIDVIEWSTATE1_customCursorSet              = 0x01000000;
        private const int DATAGRIDVIEWSTATE1_ambientFont                  = 0x02000000;
        private const int DATAGRIDVIEWSTATE1_ambientColumnHeadersFont     = 0x04000000;
        private const int DATAGRIDVIEWSTATE1_ambientRowHeadersFont        = 0x08000000;
        private const int DATAGRIDVIEWSTATE1_isRestrictedChecked          = 0x10000000;
        private const int DATAGRIDVIEWSTATE1_isRestricted                 = 0x20000000;
        private const int DATAGRIDVIEWSTATE1_isAutoSized                  = 0x40000000;

        // DATAGRIDVIEWSTATE2_
        private const int DATAGRIDVIEWSTATE2_showEditingIcon               = 0x00000001;
        private const int DATAGRIDVIEWSTATE2_allowUserToResizeColumns      = 0x00000002;
        private const int DATAGRIDVIEWSTATE2_allowUserToResizeRows         = 0x00000004;
        private const int DATAGRIDVIEWSTATE2_mouseOverRemovedEditingCtrl   = 0x00000008;
        private const int DATAGRIDVIEWSTATE2_mouseOverRemovedEditingPanel  = 0x00000010;
        private const int DATAGRIDVIEWSTATE2_mouseEnterExpected            = 0x00000020;
        private const int DATAGRIDVIEWSTATE2_enableHeadersVisualStyles     = 0x00000040;
        private const int DATAGRIDVIEWSTATE2_showCellErrors                = 0x00000080;
        private const int DATAGRIDVIEWSTATE2_showCellToolTips              = 0x00000100;
        private const int DATAGRIDVIEWSTATE2_showRowErrors                 = 0x00000200;
        private const int DATAGRIDVIEWSTATE2_showColumnRelocationInsertion = 0x00000400;
        private const int DATAGRIDVIEWSTATE2_rightToLeftMode               = 0x00000800;
        private const int DATAGRIDVIEWSTATE2_rightToLeftValid              = 0x00001000;
        private const int DATAGRIDVIEWSTATE2_currentCellWantsInputKey      = 0x00002000;
        private const int DATAGRIDVIEWSTATE2_stopRaisingVerticalScroll     = 0x00004000;
        private const int DATAGRIDVIEWSTATE2_stopRaisingHorizontalScroll   = 0x00008000;
        private const int DATAGRIDVIEWSTATE2_replacedCellSelected          = 0x00010000;
        private const int DATAGRIDVIEWSTATE2_replacedCellReadOnly          = 0x00020000;
        private const int DATAGRIDVIEWSTATE2_raiseSelectionChanged         = 0x00040000;
        private const int DATAGRIDVIEWSTATE2_initializing                  = 0x00080000;
        private const int DATAGRIDVIEWSTATE2_autoSizedWithoutHandle        = 0x00100000;
        private const int DATAGRIDVIEWSTATE2_ignoreCursorChange            = 0x00200000;
        private const int DATAGRIDVIEWSTATE2_rowsCollectionClearedInSetCell= 0x00400000;
        private const int DATAGRIDVIEWSTATE2_nextMouseUpIsDouble           = 0x00800000;
        private const int DATAGRIDVIEWSTATE2_inBindingContextChanged       = 0x01000000;
        private const int DATAGRIDVIEWSTATE2_allowHorizontalScrollbar      = 0x02000000;
        private const int DATAGRIDVIEWSTATE2_usedFillWeightsDirty          = 0x04000000;
        private const int DATAGRIDVIEWSTATE2_messageFromEditingCtrls       = 0x08000000;
        private const int DATAGRIDVIEWSTATE2_cellMouseDownInContentBounds  = 0x10000000;
        private const int DATAGRIDVIEWSTATE2_discardEditingControl         = 0x20000000;

        // DATAGRIDVIEWOPER_
        private const int DATAGRIDVIEWOPER_trackColResize                = 0x00000001;
        private const int DATAGRIDVIEWOPER_trackRowResize                = 0x00000002;
        private const int DATAGRIDVIEWOPER_trackColSelect                = 0x00000004;
        private const int DATAGRIDVIEWOPER_trackRowSelect                = 0x00000008;
        private const int DATAGRIDVIEWOPER_trackCellSelect               = 0x00000010;
        private const int DATAGRIDVIEWOPER_trackColRelocation            = 0x00000020;
        private const int DATAGRIDVIEWOPER_inSort                        = 0x00000040;
        private const int DATAGRIDVIEWOPER_trackColHeadersResize         = 0x00000080;
        private const int DATAGRIDVIEWOPER_trackRowHeadersResize         = 0x00000100;
        private const int DATAGRIDVIEWOPER_trackMouseMoves               = 0x00000200;
        private const int DATAGRIDVIEWOPER_inRefreshColumns              = 0x00000400;
        private const int DATAGRIDVIEWOPER_inDisplayIndexAdjustments     = 0x00000800;
        private const int DATAGRIDVIEWOPER_lastEditCtrlClickDoubled      = 0x00001000;
        private const int DATAGRIDVIEWOPER_inMouseDown                   = 0x00002000;
        private const int DATAGRIDVIEWOPER_inReadOnlyChange              = 0x00004000;
        private const int DATAGRIDVIEWOPER_inCellValidating              = 0x00008000;
        private const int DATAGRIDVIEWOPER_inBorderStyleChange           = 0x00010000;
        private const int DATAGRIDVIEWOPER_inCurrentCellChange           = 0x00020000;
        private const int DATAGRIDVIEWOPER_inAdjustFillingColumns        = 0x00040000;
        private const int DATAGRIDVIEWOPER_inAdjustFillingColumn         = 0x00080000;
        private const int DATAGRIDVIEWOPER_inDispose                     = 0x00100000;
        private const int DATAGRIDVIEWOPER_inBeginEdit                   = 0x00200000;
        private const int DATAGRIDVIEWOPER_inEndEdit                     = 0x00400000;
        private const int DATAGRIDVIEWOPER_resizingOperationAboutToStart = 0x00800000;
        private const int DATAGRIDVIEWOPER_trackKeyboardColResize        = 0x01000000;
        private const int DATAGRIDVIEWOPER_mouseOperationMask            = DATAGRIDVIEWOPER_trackColResize | DATAGRIDVIEWOPER_trackRowResize | 
            DATAGRIDVIEWOPER_trackColRelocation | DATAGRIDVIEWOPER_trackColHeadersResize | DATAGRIDVIEWOPER_trackRowHeadersResize;
        private const int DATAGRIDVIEWOPER_keyboardOperationMask         = DATAGRIDVIEWOPER_trackKeyboardColResize;

        private static Size DragSize = SystemInformation.DragSize;

        private const byte DATAGRIDVIEW_columnSizingHotZone = 6;
        private const byte DATAGRIDVIEW_rowSizingHotZone = 5;
        private const byte DATAGRIDVIEW_insertionBarWidth = 3;
        private const byte DATAGRIDVIEW_bulkPaintThreshold = 8;

        private const string DATAGRIDVIEW_htmlPrefix = "Version:1.0\r\nStartHTML:00000097\r\nEndHTML:{0}\r\nStartFragment:00000133\r\nEndFragment:{1}\r\n";
        private const string DATAGRIDVIEW_htmlStartFragment = "<HTML>\r\n<BODY>\r\n<!--StartFragment-->";
        private const string DATAGRIDVIEW_htmlEndFragment = "\r\n<!--EndFragment-->\r\n</BODY>\r\n</HTML>";

        private const int FOCUS_RECT_OFFSET = 2;

        private System.Collections.Specialized.BitVector32 dataGridViewState1;  // see DATAGRIDVIEWSTATE1_ consts above
        private System.Collections.Specialized.BitVector32 dataGridViewState2;  // see DATAGRIDVIEWSTATE2_ consts above
        private System.Collections.Specialized.BitVector32 dataGridViewOper;   // see DATAGRIDVIEWOPER_ consts above

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
        private DataGridViewCellLinkedList individualSelectedCells;
        private DataGridViewCellLinkedList individualReadOnlyCells;
        private DataGridViewIntLinkedList selectedBandIndexes;
        private DataGridViewIntLinkedList selectedBandSnapshotIndexes;

        private DataGridViewCellStyle defaultCellStyle, columnHeadersDefaultCellStyle, rowHeadersDefaultCellStyle;
        private DataGridViewCellStyle rowsDefaultCellStyle, alternatingRowsDefaultCellStyle;
        private ScrollBars scrollBars;
        private LayoutData layout;
        private DisplayedBandsData displayedBandsInfo;
        private Rectangle normalClientRectangle;
        private ArrayList lstRows;
        private int availableWidthForFillColumns;

        private BorderStyle borderStyle;
        private DataGridViewAdvancedBorderStyle advancedCellBorderStyle;
        private DataGridViewAdvancedBorderStyle advancedRowHeadersBorderStyle;
        private DataGridViewAdvancedBorderStyle advancedColumnHeadersBorderStyle;

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

        private System.Windows.Forms.Timer vertScrollTimer, horizScrollTimer;

        private Hashtable converters;
        private Hashtable pens;
        private Hashtable brushes;

        private NativeMethods.RECT[] cachedScrollableRegion;

        // DataBinding
        private DataGridViewDataConnection dataConnection;

        // ToolTip
        private DataGridViewToolTip toolTipControl;
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

        /// <devdoc>
        /// <para>Initializes a new instance of the <see cref='System.Windows.Forms.DataGridView'/> class.</para>
        /// </devdoc>
        public DataGridView()
        {
            SetStyle(ControlStyles.UserPaint | 
                     ControlStyles.Opaque | 
                     ControlStyles.UserMouse, true);
            
            SetStyle(ControlStyles.SupportsTransparentBackColor, false);

            // this class overrides GetPreferredSizeCore, let Control automatically cache the result
            SetState2(STATE2_USEPREFERREDSIZECACHE, true);  

            this.dataGridViewState1 = new System.Collections.Specialized.BitVector32(0x00000000);
            this.dataGridViewState2 = new System.Collections.Specialized.BitVector32(0x00000000);
            this.dataGridViewOper   = new System.Collections.Specialized.BitVector32(0x00000000);

            this.dataGridViewState1[  DATAGRIDVIEWSTATE1_columnHeadersVisible 
                                    | DATAGRIDVIEWSTATE1_rowHeadersVisible 
                                    | DATAGRIDVIEWSTATE1_autoGenerateColumns
                                    | DATAGRIDVIEWSTATE1_allowUserToAddRows
                                    | DATAGRIDVIEWSTATE1_allowUserToDeleteRows ] = true;



            this.dataGridViewState2[  DATAGRIDVIEWSTATE2_showEditingIcon
                                    | DATAGRIDVIEWSTATE2_enableHeadersVisualStyles
                                    | DATAGRIDVIEWSTATE2_mouseEnterExpected
                                    | DATAGRIDVIEWSTATE2_allowUserToResizeColumns
                                    | DATAGRIDVIEWSTATE2_allowUserToResizeRows
                                    | DATAGRIDVIEWSTATE2_showCellToolTips
                                    | DATAGRIDVIEWSTATE2_showCellErrors
                                    | DATAGRIDVIEWSTATE2_showRowErrors
                                    | DATAGRIDVIEWSTATE2_allowHorizontalScrollbar
                                    | DATAGRIDVIEWSTATE2_usedFillWeightsDirty ] = true;


            this.displayedBandsInfo = new DisplayedBandsData();
            this.lstRows = new ArrayList();

            this.converters = new Hashtable(8);
            this.pens = new Hashtable(8);
            this.brushes = new Hashtable(10);
            this.gridPen = new Pen(DefaultGridColor);

            this.selectedBandIndexes = new DataGridViewIntLinkedList();
            this.individualSelectedCells = new DataGridViewCellLinkedList();
            this.individualReadOnlyCells = new DataGridViewCellLinkedList();

            this.advancedCellBorderStyle = new DataGridViewAdvancedBorderStyle(this, 
                DataGridViewAdvancedCellBorderStyle.OutsetDouble, 
                DataGridViewAdvancedCellBorderStyle.OutsetPartial, 
                DataGridViewAdvancedCellBorderStyle.InsetDouble);
            this.advancedRowHeadersBorderStyle = new DataGridViewAdvancedBorderStyle(this);
            this.advancedColumnHeadersBorderStyle = new DataGridViewAdvancedBorderStyle(this);
            this.advancedCellBorderStyle.All = defaultAdvancedCellBorderStyle;
            this.advancedRowHeadersBorderStyle.All = defaultAdvancedRowHeadersBorderStyle;
            this.advancedColumnHeadersBorderStyle.All = defaultAdvancedColumnHeadersBorderStyle;
            this.borderStyle = defaultBorderStyle;
            this.dataGridViewState1[DATAGRIDVIEWSTATE1_multiSelect] = true;
            this.selectionMode = defaultSelectionMode;
            this.editMode = defaultEditMode;
            this.autoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            this.autoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            this.columnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.rowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;

            this.clipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithAutoHeaderText;

            this.layout = new LayoutData();
            this.layout.TopLeftHeader        = Rectangle.Empty;
            this.layout.ColumnHeaders        = Rectangle.Empty;
            this.layout.RowHeaders           = Rectangle.Empty;
            this.layout.ColumnHeadersVisible = true;
            this.layout.RowHeadersVisible    = true;
            this.layout.ClientRectangle      = this.ClientRectangle;

            this.scrollBars = ScrollBars.Both;

            this.horizScrollBar.RightToLeft = RightToLeft.Inherit;
            this.horizScrollBar.AccessibleName = string.Format(SR.DataGridView_AccHorizontalScrollBarAccName);
            this.horizScrollBar.Top = this.ClientRectangle.Height - horizScrollBar.Height;
            this.horizScrollBar.Left = 0;
            this.horizScrollBar.Visible = false;
            this.horizScrollBar.Scroll += new ScrollEventHandler(DataGridViewHScrolled);
            this.Controls.Add(this.horizScrollBar);

            this.vertScrollBar.Top = 0;
            this.vertScrollBar.AccessibleName = string.Format(SR.DataGridView_AccVerticalScrollBarAccName);
            this.vertScrollBar.Left = this.ClientRectangle.Width - vertScrollBar.Width;
            this.vertScrollBar.Visible = false;
            this.vertScrollBar.Scroll += new ScrollEventHandler(DataGridViewVScrolled);
            this.Controls.Add(this.vertScrollBar);

            this.ptCurrentCell = new Point(-1, -1);
            this.ptAnchorCell = new Point(-1, -1);
            this.ptMouseDownCell = new Point(-2, -2);
            this.ptMouseEnteredCell = new Point(-2, -2);
            this.ptToolTipCell = new Point(-1, -1);
            this.ptMouseDownGridCoord = new Point(-1, -1);

            this.sortOrder = SortOrder.None;

            this.lastMouseClickInfo.timeStamp = 0;

            WireScrollBarsEvents();
            PerformLayout();

            this.toolTipControl = new DataGridViewToolTip(this);
            this.rowHeadersWidth = ScaleToCurrentDpi(defaultRowHeadersWidth);
            this.columnHeadersHeight = ScaleToCurrentDpi(defaultColumnHeadersHeight);
            Invalidate();
        }

        /// <summary>
        /// Scaling row header width and column header height.
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
                if (this.ApplyVisualStylesToHeaderCells)
                {
                    switch (this.AdvancedColumnHeadersBorderStyle.All)
                    {
                        case DataGridViewAdvancedCellBorderStyle.OutsetDouble:
                        case DataGridViewAdvancedCellBorderStyle.OutsetPartial:
                            dgvabs = new DataGridViewAdvancedBorderStyle();
                            if (this.RightToLeftInternal)
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
                            if (this.RightToLeftInternal)
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
                            if ((!this.RightToLeftInternal && this.AdvancedColumnHeadersBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.None) ||
                                (this.RightToLeftInternal && this.AdvancedColumnHeadersBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.None))
                            {
                                dgvabs = new DataGridViewAdvancedBorderStyle();
                                if (this.RightToLeftInternal)
                                {
                                    dgvabs.LeftInternal = this.AdvancedColumnHeadersBorderStyle.Left;
                                    dgvabs.RightInternal = this.AdvancedRowHeadersBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.OutsetDouble ?
                                        DataGridViewAdvancedCellBorderStyle.Outset : this.AdvancedRowHeadersBorderStyle.Right;
                                }
                                else
                                {
                                    dgvabs.LeftInternal = this.AdvancedRowHeadersBorderStyle.Left;
                                    dgvabs.RightInternal = this.AdvancedColumnHeadersBorderStyle.Right;
                                }
                                dgvabs.TopInternal = this.AdvancedColumnHeadersBorderStyle.Top;
                                dgvabs.BottomInternal = this.AdvancedColumnHeadersBorderStyle.Bottom;
                            }
                            else
                            {
                                dgvabs = this.AdvancedColumnHeadersBorderStyle;
                            }
                            break;

                        default:
                            dgvabs = this.AdvancedColumnHeadersBorderStyle;
                            break;
                    }
                }
                else
                {
                    switch (this.AdvancedColumnHeadersBorderStyle.All)
                    {
                        case DataGridViewAdvancedCellBorderStyle.OutsetDouble:
                        case DataGridViewAdvancedCellBorderStyle.OutsetPartial:
                            dgvabs = new DataGridViewAdvancedBorderStyle();
                            dgvabs.LeftInternal = this.RightToLeftInternal ? DataGridViewAdvancedCellBorderStyle.Outset : DataGridViewAdvancedCellBorderStyle.OutsetDouble;
                            dgvabs.RightInternal = this.RightToLeftInternal ? DataGridViewAdvancedCellBorderStyle.OutsetDouble : DataGridViewAdvancedCellBorderStyle.Outset;
                            dgvabs.TopInternal = DataGridViewAdvancedCellBorderStyle.OutsetDouble;
                            dgvabs.BottomInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                            break;

                        case DataGridViewAdvancedCellBorderStyle.InsetDouble:
                            dgvabs = new DataGridViewAdvancedBorderStyle();
                            dgvabs.LeftInternal = this.RightToLeftInternal ? DataGridViewAdvancedCellBorderStyle.Inset : DataGridViewAdvancedCellBorderStyle.InsetDouble;
                            dgvabs.RightInternal = this.RightToLeftInternal ? DataGridViewAdvancedCellBorderStyle.InsetDouble : DataGridViewAdvancedCellBorderStyle.Inset;
                            dgvabs.TopInternal = DataGridViewAdvancedCellBorderStyle.InsetDouble;
                            dgvabs.BottomInternal = DataGridViewAdvancedCellBorderStyle.Inset;
                            break;

                        case DataGridViewAdvancedCellBorderStyle.NotSet:
                            // Since the row headers are visible, we should make sure
                            // that there is a left/right border for the TopLeftHeaderCell no matter what.
                            if ((!this.RightToLeftInternal && this.AdvancedColumnHeadersBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.None) ||
                                (this.RightToLeftInternal && this.AdvancedColumnHeadersBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.None))
                            {
                                dgvabs = new DataGridViewAdvancedBorderStyle();
                                if (this.RightToLeftInternal)
                                {
                                    dgvabs.LeftInternal = this.AdvancedColumnHeadersBorderStyle.Left;
                                    dgvabs.RightInternal = this.AdvancedRowHeadersBorderStyle.Right;
                                }
                                else
                                {
                                    dgvabs.LeftInternal = this.AdvancedRowHeadersBorderStyle.Left;
                                    dgvabs.RightInternal = this.AdvancedColumnHeadersBorderStyle.Right;
                                }
                                dgvabs.TopInternal = this.AdvancedColumnHeadersBorderStyle.Top;
                                dgvabs.BottomInternal = this.AdvancedColumnHeadersBorderStyle.Bottom;
                            }
                            else
                            {
                                dgvabs = this.AdvancedColumnHeadersBorderStyle;
                            }
                            break;

                        default:
                            dgvabs = this.AdvancedColumnHeadersBorderStyle;
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
                return this.advancedCellBorderStyle;
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
                return this.advancedColumnHeadersBorderStyle;
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
                return this.advancedRowHeadersBorderStyle;
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
                return this.dataGridViewState1[DATAGRIDVIEWSTATE1_allowUserToAddRows];
            }
            set
            {
                if (this.AllowUserToAddRows != value)
                {
                    this.dataGridViewState1[DATAGRIDVIEWSTATE1_allowUserToAddRows] = value;
                    if (this.DataSource != null)
                    {
                        this.dataConnection.ResetCachedAllowUserToAddRowsInternal();
                    }
                    OnAllowUserToAddRowsChanged(EventArgs.Empty);
                }
            }
        }

        internal bool AllowUserToAddRowsInternal
        {
            get
            {
                if (this.DataSource == null)
                {
                    return this.AllowUserToAddRows;
                }
                else
                {
                    return this.AllowUserToAddRows && this.dataConnection.AllowAdd;
                }
            }
        }


        [
            SRCategory(nameof(SR.CatPropertyChanged)), 
            SRDescription(nameof(SR.DataGridViewOnAllowUserToAddRowsChangedDescr))
        ]
        public event EventHandler AllowUserToAddRowsChanged
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWALLOWUSERTOADDROWSCHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWALLOWUSERTOADDROWSCHANGED, value);
            }
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
                return this.dataGridViewState1[DATAGRIDVIEWSTATE1_allowUserToDeleteRows];
            }
            set
            {
                if (this.AllowUserToDeleteRows != value)
                {
                    this.dataGridViewState1[DATAGRIDVIEWSTATE1_allowUserToDeleteRows] = value;
                    OnAllowUserToDeleteRowsChanged(EventArgs.Empty);
                }
            }
        }

        internal bool AllowUserToDeleteRowsInternal
        {
            get
            {
                if (this.DataSource == null)
                {
                    return this.AllowUserToDeleteRows;
                }
                else
                {
                    return this.AllowUserToDeleteRows && this.dataConnection.AllowRemove;
                }
            }
        }


        [
            SRCategory(nameof(SR.CatPropertyChanged)), 
            SRDescription(nameof(SR.DataGridViewOnAllowUserToDeleteRowsChangedDescr))
        ]
        public event EventHandler AllowUserToDeleteRowsChanged
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWALLOWUSERTODELETEROWSCHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWALLOWUSERTODELETEROWSCHANGED, value);
            }
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
                return this.dataGridViewState1[DATAGRIDVIEWSTATE1_allowUserToOrderColumns];
            }
            set
            {
                if (this.AllowUserToOrderColumns != value)
                {
                    this.dataGridViewState1[DATAGRIDVIEWSTATE1_allowUserToOrderColumns] = value;
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
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWALLOWUSERTOORDERCOLUMNSCHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWALLOWUSERTOORDERCOLUMNSCHANGED, value);
            }
        }


        /// <devdoc>
        ///    <para>
        ///       Gets or sets a global value indicating if the dataGridView's columns are resizable with the mouse.
        ///       The resizable aspect of a column can be overridden by DataGridViewColumn.Resizable.
        ///    </para>
        /// </devdoc>
        [
            DefaultValue(true),
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_AllowUserToResizeColumnsDescr))
        ]
        public bool AllowUserToResizeColumns
        {
            get
            {
                return this.dataGridViewState2[DATAGRIDVIEWSTATE2_allowUserToResizeColumns];
            }
            set
            {
                if (this.AllowUserToResizeColumns != value)
                {
                    this.dataGridViewState2[DATAGRIDVIEWSTATE2_allowUserToResizeColumns] = value;
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
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWALLOWUSERTORESIZECOLUMNSCHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWALLOWUSERTORESIZECOLUMNSCHANGED, value);
            }
        }


        /// <devdoc>
        ///    <para>
        ///       Gets or sets a global value indicating if the dataGridView's rows are resizable with the mouse.
        ///       The resizable aspect of a row can be overridden by DataGridViewRow.Resizable.
        ///    </para>
        /// </devdoc>
        [
            DefaultValue(true),
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_AllowUserToResizeRowsDescr))
        ]
        public bool AllowUserToResizeRows
        {
            get
            {
                return this.dataGridViewState2[DATAGRIDVIEWSTATE2_allowUserToResizeRows];
            }
            set
            {
                if (this.AllowUserToResizeRows != value)
                {
                    this.dataGridViewState2[DATAGRIDVIEWSTATE2_allowUserToResizeRows] = value;
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
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWALLOWUSERTORESIZEROWSCHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWALLOWUSERTORESIZEROWSCHANGED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_AlternatingRowsDefaultCellStyleDescr))
        ]
        public DataGridViewCellStyle AlternatingRowsDefaultCellStyle
        {
            get
            {
                if (this.alternatingRowsDefaultCellStyle == null)
                {
                    this.alternatingRowsDefaultCellStyle = new DataGridViewCellStyle();
                    this.alternatingRowsDefaultCellStyle.AddScope(this, DataGridViewCellStyleScopes.AlternatingRows);
                }
                return this.alternatingRowsDefaultCellStyle;
            }
            set
            {
                DataGridViewCellStyle cs = this.AlternatingRowsDefaultCellStyle;
                cs.RemoveScope(DataGridViewCellStyleScopes.AlternatingRows);
                this.alternatingRowsDefaultCellStyle = value;
                if (value != null)
                {
                    this.alternatingRowsDefaultCellStyle.AddScope(this, DataGridViewCellStyleScopes.AlternatingRows);
                }
                DataGridViewCellStyleDifferences dgvcsc = cs.GetDifferencesFrom(this.AlternatingRowsDefaultCellStyle);
                if (dgvcsc != DataGridViewCellStyleDifferences.None)
                {
                    this.CellStyleChangedEventArgs.ChangeAffectsPreferredSize = (dgvcsc == DataGridViewCellStyleDifferences.AffectPreferredSize);
                    OnAlternatingRowsDefaultCellStyleChanged(this.CellStyleChangedEventArgs);
                }
            }
        }


        [
            SRCategory(nameof(SR.CatPropertyChanged)), 
            SRDescription(nameof(SR.DataGridViewAlternatingRowsDefaultCellStyleChangedDescr))
        ]
        public event EventHandler AlternatingRowsDefaultCellStyleChanged
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWALTERNATINGROWSDEFAULTCELLSTYLECHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWALTERNATINGROWSDEFAULTCELLSTYLECHANGED, value);
            }
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
                return Application.RenderWithVisualStyles && this.EnableHeadersVisualStyles;
            }
        }


        /// <devdoc>
        ///    <para>
        ///    </para>
        /// </devdoc>
        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced),
            DefaultValue(true)
        ]
        public bool AutoGenerateColumns
        {
            get
            {
                return this.dataGridViewState1[DATAGRIDVIEWSTATE1_autoGenerateColumns];
            }
            set{
                if (this.dataGridViewState1[DATAGRIDVIEWSTATE1_autoGenerateColumns] != value)
                {
                    this.dataGridViewState1[DATAGRIDVIEWSTATE1_autoGenerateColumns] = value;
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
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWAUTOGENERATECOLUMNSCHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWAUTOGENERATECOLUMNSCHANGED, value);
            }
        }


        /// <devdoc>
        ///    <para> Overriding base implementation for perf gains. </para>
        /// </devdoc>
        public override bool AutoSize
        {
            get
            {
                return this.dataGridViewState1[DATAGRIDVIEWSTATE1_isAutoSized];
            }
            set
            {
                base.AutoSize = value;
                this.dataGridViewState1[DATAGRIDVIEWSTATE1_isAutoSized] = value;
            }
        }


        /// <devdoc>
        ///    <para> Gets or sets the columns' autosizing mode. Standard inheritance model is used:
        ///           Columns with AutoSizeMode property set to NotSet will use this auto size mode.
        ///    </para>
        /// </devdoc>
        [
            DefaultValue(DataGridViewAutoSizeColumnsMode.None),
            SRCategory(nameof(SR.CatLayout)),
            SRDescription(nameof(SR.DataGridView_AutoSizeColumnsModeDescr))
        ]
        public DataGridViewAutoSizeColumnsMode AutoSizeColumnsMode
        {
            get
            {
                return this.autoSizeColumnsMode;
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


                if (this.autoSizeColumnsMode != value)
                {
                    foreach (DataGridViewColumn dataGridViewColumn in this.Columns)
                    {
                        if (dataGridViewColumn.AutoSizeMode == DataGridViewAutoSizeColumnMode.NotSet && dataGridViewColumn.Visible)
                        {
                            // Make sure there is no visible column which would have an inherited autosize mode based on the header only.
                            if (value == DataGridViewAutoSizeColumnsMode.ColumnHeader && !this.ColumnHeadersVisible)
                            {
                                throw new InvalidOperationException(string.Format(SR.DataGridView_CannotAutoSizeColumnsInvisibleColumnHeaders));
                            }
                            // Make sure there is no visible frozen column which would have a Fill inherited autosize mode.
                            if (value == DataGridViewAutoSizeColumnsMode.Fill && dataGridViewColumn.Frozen)
                            {
                                throw new InvalidOperationException(string.Format(SR.DataGridView_CannotAutoFillFrozenColumns));
                            }
                        }
                    }
                    DataGridViewAutoSizeColumnMode[] previousModes = new DataGridViewAutoSizeColumnMode[this.Columns.Count];
                    foreach (DataGridViewColumn dataGridViewColumn in this.Columns)
                    {
                        /*DataGridViewAutoSizeColumnMode previousInheritedMode = dataGridViewColumn.InheritedAutoSizeMode;
                        bool previousInheritedModeAutoSized = previousInheritedMode != DataGridViewAutoSizeColumnMode.Fill &&
                                                              previousInheritedMode != DataGridViewAutoSizeColumnMode.None &&
                                                              previousInheritedMode != DataGridViewAutoSizeColumnMode.NotSet;*/
                        previousModes[dataGridViewColumn.Index] = dataGridViewColumn.InheritedAutoSizeMode;
                    }
                    DataGridViewAutoSizeColumnsModeEventArgs dgvcasme = new DataGridViewAutoSizeColumnsModeEventArgs(previousModes);
                    this.autoSizeColumnsMode = value;
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
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWAUTOSIZECOLUMNSMODECHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWAUTOSIZECOLUMNSMODECHANGED, value);
            }
        }


        /// <devdoc>
        ///    <para> Gets or sets the rows' autosizing mode. </para>
        /// </devdoc>
        [
            DefaultValue(DataGridViewAutoSizeRowsMode.None),
            SRCategory(nameof(SR.CatLayout)),
            SRDescription(nameof(SR.DataGridView_AutoSizeRowsModeDescr))
        ]
        public DataGridViewAutoSizeRowsMode AutoSizeRowsMode
        {
            get
            {
                return this.autoSizeRowsMode;
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
                    !this.RowHeadersVisible)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_CannotAutoSizeRowsInvisibleRowHeader));
                }
                if (this.autoSizeRowsMode != value)
                {
                    DataGridViewAutoSizeModeEventArgs dgvasme = new DataGridViewAutoSizeModeEventArgs(this.autoSizeRowsMode != DataGridViewAutoSizeRowsMode.None);
                    this.autoSizeRowsMode = value;
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
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWAUTOSIZEROWSMODECHANGED, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWAUTOSIZEROWSMODECHANGED, value);
            }
        }
        

        /// <internalonly/>
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


        /// <internalonly/>
        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Never)
        ]
        new public event EventHandler BackColorChanged
        {
            add
            {
                base.BackColorChanged += value;
            }
            remove
            {
                base.BackColorChanged -= value;
            }
        }

        internal SolidBrush BackgroundBrush 
        {
            get 
            {
                return this.backgroundBrush;
            }
        }


        /// <devdoc>
        ///    <para>Gets or sets the background color of the dataGridView.</para>
        /// </devdoc>
        [
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridViewBackgroundColorDescr))
        ]
        public Color BackgroundColor
        {
            get
            {
                return this.backgroundBrush.Color;
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
                if (!value.Equals(this.backgroundBrush.Color)) 
                {
                    this.backgroundBrush = new SolidBrush(value);
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
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWBACKGROUNDCOLORCHANGED, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWBACKGROUNDCOLORCHANGED, value);
            }
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
            add
            {
                base.BackgroundImageChanged += value;
            }
            remove
            {
                base.BackgroundImageChanged -= value;
            }
        }


        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Never)
        ]
        new public event EventHandler BackgroundImageLayoutChanged
        {
            add
            {
                base.BackgroundImageLayoutChanged += value;
            }
            remove
            {
                base.BackgroundImageLayoutChanged -= value;
            }
        }


        private bool ShouldSerializeBackgroundColor()
        {
            return !this.BackgroundColor.Equals(DefaultBackgroundBrush.Color);
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
                return this.borderStyle;
            }
            set 
            {
                // Sequential enum.  Valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)BorderStyle.None, (int)BorderStyle.Fixed3D)){
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(BorderStyle)); 
                }
                if (this.borderStyle != value) 
                {
                    using (LayoutTransaction.CreateTransactionIf(this.AutoSize, this.ParentInternal, this, PropertyNames.BorderStyle))
                    {
                        this.borderStyle = value;
                        if (!this.AutoSize)
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
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWBORDERSTYLECHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWBORDERSTYLECHANGED, value);
            }
        }

        private int BorderWidth 
        {
            get 
            {
                if (this.BorderStyle == BorderStyle.Fixed3D) 
                {
                    return Application.RenderWithVisualStyles ? 1 : SystemInformation.Border3DSize.Width;
                }
                else if (this.BorderStyle == BorderStyle.FixedSingle) 
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

                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside get_CanEnableIme(), this = " + this );
                Debug.Indent();

                if (this.ptCurrentCell.X != -1 /*&& !this.IsCurrentCellInEditMode*/ && ColumnEditable(this.ptCurrentCell.X))
                {
                    DataGridViewCell dataGridViewCell = this.CurrentCellInternal;
                    Debug.Assert(dataGridViewCell != null);

                    if (!IsSharedCellReadOnly(dataGridViewCell, this.ptCurrentCell.Y))
                    {
                        canEnable = base.CanEnableIme;
                    }
                }

                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Value = " + canEnable );
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
                switch (this.advancedCellBorderStyle.All)
                {
                    case DataGridViewAdvancedCellBorderStyle.NotSet:
                        if (this.advancedCellBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.None &&
                            this.advancedCellBorderStyle.Bottom == DataGridViewAdvancedCellBorderStyle.None)
                        {
                            if (this.RightToLeftInternal)
                            {
                                if (this.advancedCellBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.None &&
                                    this.advancedCellBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.Single)
                                {
                                    return DataGridViewCellBorderStyle.SingleVertical;
                                }
                            }
                            else
                            {
                                if (this.advancedCellBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.None &&
                                    this.advancedCellBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.Single)
                                {
                                    return DataGridViewCellBorderStyle.SingleVertical;
                                }
                            }
                            if (this.advancedCellBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.Outset && 
                                this.advancedCellBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.Outset)
                            {
                                return DataGridViewCellBorderStyle.RaisedVertical;
                            }
                            if (this.advancedCellBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.Inset && 
                                this.advancedCellBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.Inset)
                            {
                                return DataGridViewCellBorderStyle.SunkenVertical;
                            }
                        }
                        if (this.advancedCellBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.None &&
                            this.advancedCellBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.None)
                        {
                            if (this.advancedCellBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.None &&
                                this.advancedCellBorderStyle.Bottom == DataGridViewAdvancedCellBorderStyle.Single)
                            {
                                return DataGridViewCellBorderStyle.SingleHorizontal;
                            }
                            if (this.advancedCellBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.Outset && 
                                this.advancedCellBorderStyle.Bottom == DataGridViewAdvancedCellBorderStyle.Outset)
                            {
                                return DataGridViewCellBorderStyle.RaisedHorizontal;
                            }
                            if (this.advancedCellBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.Inset && 
                                this.advancedCellBorderStyle.Bottom == DataGridViewAdvancedCellBorderStyle.Inset)
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

                if (value != this.CellBorderStyle)
                {
                    if (value == DataGridViewCellBorderStyle.Custom)
                    {
                        throw new ArgumentException(string.Format(SR.DataGridView_CustomCellBorderStyleInvalid, "CellBorderStyle"));
                    }
                    this.dataGridViewOper[DATAGRIDVIEWOPER_inBorderStyleChange] = true;
                    try
                    {
                        switch (value)
                        {
                            case DataGridViewCellBorderStyle.Single:
                                this.advancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Single;
                                break;

                            case DataGridViewCellBorderStyle.Raised:
                                this.advancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Outset;
                                break;

                            case DataGridViewCellBorderStyle.Sunken:
                                this.advancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Inset;
                                break;

                            case DataGridViewCellBorderStyle.None:
                                this.advancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                                break;

                            case DataGridViewCellBorderStyle.SingleVertical:
                                this.advancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                                if (this.RightToLeftInternal)
                                {
                                    this.advancedCellBorderStyle.LeftInternal = DataGridViewAdvancedCellBorderStyle.Single;
                                }
                                else
                                {
                                    this.advancedCellBorderStyle.RightInternal = DataGridViewAdvancedCellBorderStyle.Single;
                                }
                                break;

                            case DataGridViewCellBorderStyle.RaisedVertical:
                                this.advancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                                this.advancedCellBorderStyle.RightInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                                this.advancedCellBorderStyle.LeftInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                                break;

                            case DataGridViewCellBorderStyle.SunkenVertical:
                                this.advancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                                this.advancedCellBorderStyle.RightInternal = DataGridViewAdvancedCellBorderStyle.Inset;
                                this.advancedCellBorderStyle.LeftInternal = DataGridViewAdvancedCellBorderStyle.Inset;
                                break;

                            case DataGridViewCellBorderStyle.SingleHorizontal:
                                this.advancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                                this.advancedCellBorderStyle.BottomInternal = DataGridViewAdvancedCellBorderStyle.Single;
                                break;

                            case DataGridViewCellBorderStyle.RaisedHorizontal:
                                this.advancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                                this.advancedCellBorderStyle.TopInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                                this.advancedCellBorderStyle.BottomInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                                break;

                            case DataGridViewCellBorderStyle.SunkenHorizontal:
                                this.advancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                                this.advancedCellBorderStyle.TopInternal = DataGridViewAdvancedCellBorderStyle.Inset;
                                this.advancedCellBorderStyle.BottomInternal = DataGridViewAdvancedCellBorderStyle.Inset;
                                break;
                        }
                    }
                    finally
                    {
                        this.dataGridViewOper[DATAGRIDVIEWOPER_inBorderStyleChange] = false;
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
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLBORDERSTYLECHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLBORDERSTYLECHANGED, value);
            }
        }

        internal bool CellMouseDownInContentBounds
        {
            get
            {
                return this.dataGridViewState2[DATAGRIDVIEWSTATE2_cellMouseDownInContentBounds];
            }
            set
            {
                this.dataGridViewState2[DATAGRIDVIEWSTATE2_cellMouseDownInContentBounds] = value;
            }
        }

        internal DataGridViewCellPaintingEventArgs CellPaintingEventArgs
        {
            get
            {
                if (this.dgvcpe == null)
                {
                    this.dgvcpe = new DataGridViewCellPaintingEventArgs(this);
                }
                return this.dgvcpe;
            }
        }

        private DataGridViewCellStyleChangedEventArgs CellStyleChangedEventArgs
        {
            get
            {
                if (this.dgvcsce == null)
                {
                    this.dgvcsce = new DataGridViewCellStyleChangedEventArgs();
                }
                return this.dgvcsce;
            }
        }        

        internal DataGridViewCellValueEventArgs CellValueEventArgs
        {
            get
            {
                if (this.dgvcve == null)
                {
                    this.dgvcve = new DataGridViewCellValueEventArgs();
                }
                return this.dgvcve;
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
                return this.clipboardCopyMode;
            }
            set
            {
                // Sequential enum.  Valid values are 0x0 to 0x3
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DataGridViewClipboardCopyMode.Disable, (int)DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText))
                {
                   throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridViewClipboardCopyMode)); 
                }
                this.clipboardCopyMode = value;
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
                return this.Columns.Count;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(ColumnCount), string.Format(SR.InvalidLowBoundArgumentEx, "ColumnCount", value.ToString(CultureInfo.CurrentCulture), (0).ToString(CultureInfo.CurrentCulture)));
                }
                if (this.DataSource != null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_CannotSetColumnCountOnDataBoundDataGridView));
                }
                if (value != this.Columns.Count)
                {
                    if (value == 0)
                    {
                        // Total removal of the columns. This also clears the rows.
                        this.Columns.Clear();
                    }
                    else if (value < this.Columns.Count)
                    {
                        // Some columns need to be removed, from the tail of the columns collection
                        while (value < this.Columns.Count)
                        {
                            int currentColumnCount = this.Columns.Count;
                            this.Columns.RemoveAt(currentColumnCount - 1);
                            if (this.Columns.Count >= currentColumnCount)
                            {
                                // Column removal failed. We stop the loop.
                                break;
                            }
                        }
                    }
                    else
                    {
                        // Some DataGridViewTextBoxColumn columns need to be appened.
                        while (value > this.Columns.Count)
                        {
                            int currentColumnCount = this.Columns.Count;
                            this.Columns.Add(null /*columnName*/, null /*headerText*/);
                            if (this.Columns.Count <= currentColumnCount)
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
                switch (this.advancedColumnHeadersBorderStyle.All)
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
                if (value != this.ColumnHeadersBorderStyle)
                {
                    if (value == DataGridViewHeaderBorderStyle.Custom)
                    {
                        throw new ArgumentException(string.Format(SR.DataGridView_CustomCellBorderStyleInvalid, "ColumnHeadersBorderStyle"));
                    }
                    this.dataGridViewOper[DATAGRIDVIEWOPER_inBorderStyleChange] = true;
                    try
                    {
                        switch (value)
                        {
                            case DataGridViewHeaderBorderStyle.Single:
                                this.advancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Single;
                                break;

                            case DataGridViewHeaderBorderStyle.Raised:
                                this.advancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.OutsetPartial;
                                break;

                            case DataGridViewHeaderBorderStyle.Sunken:
                                this.advancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.InsetDouble;
                                break;

                            case DataGridViewHeaderBorderStyle.None:
                                this.advancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                                break;
                        }
                    }
                    finally
                    {
                        this.dataGridViewOper[DATAGRIDVIEWOPER_inBorderStyleChange] = false;
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
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNHEADERSBORDERSTYLECHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNHEADERSBORDERSTYLECHANGED, value);
            }
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
                if (this.columnHeadersDefaultCellStyle == null)
                {
                    this.columnHeadersDefaultCellStyle = this.DefaultColumnHeadersDefaultCellStyle;
                }
                return this.columnHeadersDefaultCellStyle;
            }
            set 
            {
                DataGridViewCellStyle cs = this.ColumnHeadersDefaultCellStyle;
                cs.RemoveScope(DataGridViewCellStyleScopes.ColumnHeaders);
                this.columnHeadersDefaultCellStyle = value;
                if (value != null)
                {
                    this.columnHeadersDefaultCellStyle.AddScope(this, DataGridViewCellStyleScopes.ColumnHeaders);
                }
                DataGridViewCellStyleDifferences dgvcsc = cs.GetDifferencesFrom(this.ColumnHeadersDefaultCellStyle);
                if (dgvcsc != DataGridViewCellStyleDifferences.None)
                {
                    this.CellStyleChangedEventArgs.ChangeAffectsPreferredSize = (dgvcsc == DataGridViewCellStyleDifferences.AffectPreferredSize);
                    OnColumnHeadersDefaultCellStyleChanged(this.CellStyleChangedEventArgs);
                }
            }
        }

        private DataGridViewCellStyle DefaultColumnHeadersDefaultCellStyle {
            get
            {
                DataGridViewCellStyle defaultStyle = new DataGridViewCellStyle();
                defaultStyle.BackColor = DefaultHeadersBackBrush.Color;
                defaultStyle.ForeColor = DefaultForeBrush.Color;
                defaultStyle.SelectionBackColor = DefaultSelectionBackBrush.Color;
                defaultStyle.SelectionForeColor = DefaultSelectionForeBrush.Color;
                defaultStyle.Font = base.Font;
                defaultStyle.AlignmentInternal = DataGridViewContentAlignment.MiddleLeft;
                defaultStyle.WrapModeInternal = DataGridViewTriState.True;
                defaultStyle.AddScope(this, DataGridViewCellStyleScopes.ColumnHeaders);

                this.dataGridViewState1[DATAGRIDVIEWSTATE1_ambientColumnHeadersFont] = true;

                return defaultStyle;
            }
        }


        [
            SRCategory(nameof(SR.CatPropertyChanged)), 
            SRDescription(nameof(SR.DataGridViewColumnHeadersDefaultCellStyleChangedDescr))
        ]
        public event EventHandler ColumnHeadersDefaultCellStyleChanged
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNHEADERSDEFAULTCELLSTYLECHANGED, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNHEADERSDEFAULTCELLSTYLECHANGED, value);
            }
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
                return this.columnHeadersHeight;
            }
            set 
            {
                if (value < minimumColumnHeadersHeight)
                {
                    throw new ArgumentOutOfRangeException(nameof(ColumnHeadersHeight), string.Format(SR.InvalidLowBoundArgumentEx, "ColumnHeadersHeight", (value).ToString(CultureInfo.CurrentCulture), (minimumColumnHeadersHeight).ToString(CultureInfo.CurrentCulture)));
                }
                if (value > maxHeadersThickness)
                {
                    throw new ArgumentOutOfRangeException(nameof(ColumnHeadersHeight), string.Format(SR.InvalidHighBoundArgumentEx, "ColumnHeadersHeight", (value).ToString(CultureInfo.CurrentCulture), (maxHeadersThickness).ToString(CultureInfo.CurrentCulture)));
                }
                if (this.ColumnHeadersHeightSizeMode == DataGridViewColumnHeadersHeightSizeMode.AutoSize)
                {
                    this.cachedColumnHeadersHeight = value;
                }
                else if (this.columnHeadersHeight != value)
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
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNHEADERSHEIGHTCHANGED, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNHEADERSHEIGHTCHANGED, value);
            }
        }

        private bool ShouldSerializeColumnHeadersHeight()
        {
            return this.ColumnHeadersHeightSizeMode != DataGridViewColumnHeadersHeightSizeMode.AutoSize && defaultColumnHeadersHeight != this.ColumnHeadersHeight;
        }


        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value that determines the behavior for adjusting the column headers height.
        ///    </para>
        /// </devdoc>
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
                return this.columnHeadersHeightSizeMode;
            }
            set
            {
               // Sequential enum.  Valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DataGridViewColumnHeadersHeightSizeMode.EnableResizing, (int)DataGridViewColumnHeadersHeightSizeMode.AutoSize))
                {
                     throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridViewColumnHeadersHeightSizeMode)); 
                }
                if (this.columnHeadersHeightSizeMode != value)
                {
                    /*if (value == DataGridViewColumnHeadersHeightSizeMode.AutoSize && !this.ColumnHeadersVisible)
                    {
                        We intentionally don't throw an error because of designer code spit order.
                    }*/
                    DataGridViewAutoSizeModeEventArgs dgvasme = new DataGridViewAutoSizeModeEventArgs(this.columnHeadersHeightSizeMode == DataGridViewColumnHeadersHeightSizeMode.AutoSize);
                    this.columnHeadersHeightSizeMode = value;
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
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNHEADERSHEIGHTSIZEMODECHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNHEADERSHEIGHTSIZEMODECHANGED, value);
            }
        }

        /// <summary>
        /// Indicates whether the ComboBox editing control was just detached. (focused out to another cell)
        /// </summary>
        internal bool ComboBoxControlWasDetached { get; set; }

        /// <summary>
        /// Indicates whether the TextBox editing control was just detached. (focused out to another cell)
        /// </summary>
        internal bool TextBoxControlWasDetached { get; set; }


        /// <devdoc>
        ///    <para>
        ///       Gets
        ///       or sets a value indicating if the dataGridView's column headers are visible.
        ///    </para>
        /// </devdoc>
        [
            SRCategory(nameof(SR.CatAppearance)),
            DefaultValue(true),
            SRDescription(nameof(SR.DataGridViewColumnHeadersVisibleDescr))
        ]
        public bool ColumnHeadersVisible 
        {
            get 
            {
                return this.dataGridViewState1[DATAGRIDVIEWSTATE1_columnHeadersVisible];
            }
            set 
            {
                if (this.ColumnHeadersVisible != value)
                {
                    if (!value)
                    {
                        // Make sure that there is no visible column that only counts on the column headers to autosize
                        DataGridViewColumn dataGridViewColumn = this.Columns.GetFirstColumn(DataGridViewElementStates.Visible);
                        while (dataGridViewColumn != null)
                        {
                            if (dataGridViewColumn.InheritedAutoSizeMode == DataGridViewAutoSizeColumnMode.ColumnHeader)
                            {
                                throw new InvalidOperationException(string.Format(SR.DataGridView_ColumnHeadersCannotBeInvisible));
                            }
                            dataGridViewColumn = this.Columns.GetNextColumn(dataGridViewColumn,
                                DataGridViewElementStates.Visible,
                                DataGridViewElementStates.None);
                        }
                    }
                    using (LayoutTransaction.CreateTransactionIf(this.AutoSize, this.ParentInternal, this, PropertyNames.ColumnHeadersVisible))
                    {
                        this.dataGridViewState1[DATAGRIDVIEWSTATE1_columnHeadersVisible] = value;
                        this.layout.ColumnHeadersVisible = value;
                        this.displayedBandsInfo.EnsureDirtyState();
                        if (!this.AutoSize)
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
            Editor("System.Windows.Forms.Design.DataGridViewColumnCollectionEditor, " + AssemblyRef.SystemDesign, typeof(System.Drawing.Design.UITypeEditor)),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
            MergableProperty(false)
        ]
        public DataGridViewColumnCollection Columns
        {
            get
            {
                if (this.dataGridViewColumns == null)
                {
                    this.dataGridViewColumns = CreateColumnsInstance();
                }
                return this.dataGridViewColumns;
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
                if (this.ptCurrentCell.X == -1 && this.ptCurrentCell.Y == -1)
                {
                    return null;
                }
                Debug.Assert(this.ptCurrentCell.X >= 0 && ptCurrentCell.Y >= 0);
                Debug.Assert(this.ptCurrentCell.X < this.Columns.Count);
                Debug.Assert(this.ptCurrentCell.Y < this.Rows.Count);
                DataGridViewRow dataGridViewRow = (DataGridViewRow) this.Rows[this.ptCurrentCell.Y]; // unsharing row
                return dataGridViewRow.Cells[this.ptCurrentCell.X];
            }
            set
            {
                if ((value != null && (value.RowIndex != this.ptCurrentCell.Y || value.ColumnIndex != this.ptCurrentCell.X)) ||
                    (value == null && this.ptCurrentCell.X != -1))
                {
                    if (value == null)
                    {
                        ClearSelection();
                        if (!SetCurrentCellAddressCore(-1, -1, true /*setAnchorCellAddress*/, true /*validateCurrentCell*/, false /*throughMouseClick*/))
                        {
                            // Edited value couldn't be committed or aborted
                            throw new InvalidOperationException(string.Format(SR.DataGridView_CellChangeCannotBeCommittedOrAborted));
                        }
                    }
                    else
                    {
                        if (value.DataGridView != this)
                        {
                            throw new ArgumentException(string.Format(SR.DataGridView_CellDoesNotBelongToDataGridView));
                        }
                        if (!this.Columns[value.ColumnIndex].Visible ||
                            (this.Rows.GetRowState(value.RowIndex) & DataGridViewElementStates.Visible) == 0)
                        {
                            throw new InvalidOperationException(string.Format(SR.DataGridView_CurrentCellCannotBeInvisible));
                        }
                        if (!ScrollIntoView(value.ColumnIndex, value.RowIndex, true))
                        {
                            throw new InvalidOperationException(string.Format(SR.DataGridView_CellChangeCannotBeCommittedOrAborted));
                        }
                        if (IsInnerCellOutOfBounds(value.ColumnIndex, value.RowIndex))
                        {
                            return;
                        }
                        ClearSelection(value.ColumnIndex, value.RowIndex, true /*selectExceptionElement*/);
                        if (!SetCurrentCellAddressCore(value.ColumnIndex, value.RowIndex, true, false, false))
                        {
                            throw new InvalidOperationException(string.Format(SR.DataGridView_CellChangeCannotBeCommittedOrAborted));
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
                return this.ptCurrentCell;
            }
        }

        private DataGridViewCell CurrentCellInternal
        {
            get
            {
                Debug.Assert(this.ptCurrentCell.X >= 0 && this.ptCurrentCell.X < this.Columns.Count);
                Debug.Assert(this.ptCurrentCell.Y >= 0 && this.ptCurrentCell.Y < this.Rows.Count);
                DataGridViewRow dataGridViewRow = this.Rows.SharedRow(this.ptCurrentCell.Y);
                Debug.Assert(dataGridViewRow != null);
                DataGridViewCell dataGridViewCell = dataGridViewRow.Cells[this.ptCurrentCell.X];
                Debug.Assert(this.IsSharedCellVisible(dataGridViewCell, this.ptCurrentCell.Y));
                return dataGridViewCell;
            }
        }

        private bool CurrentCellIsFirstVisibleCell
        {
            get
            {
                if (this.ptCurrentCell.X == -1)
                {
                    return false;
                }
                Debug.Assert(this.ptCurrentCell.Y != -1);

                bool previousVisibleColumnExists = (null != this.Columns.GetPreviousColumn(this.Columns[this.ptCurrentCell.X], DataGridViewElementStates.Visible, DataGridViewElementStates.None));
                bool previousVisibleRowExists = (-1 != this.Rows.GetPreviousRow(this.ptCurrentCell.Y, DataGridViewElementStates.Visible));

                return !previousVisibleColumnExists && !previousVisibleRowExists;
            }
        }

        private bool CurrentCellIsLastVisibleCell
        {
            get
            {
                if (this.ptCurrentCell.X == -1)
                {
                    return false;
                }

                Debug.Assert(this.ptCurrentCell.Y != -1);

                bool nextVisibleColumnExists = (null != this.Columns.GetNextColumn(this.Columns[this.ptCurrentCell.X], DataGridViewElementStates.Visible, DataGridViewElementStates.None));
                bool nextVisibleRowExists = (-1 != this.Rows.GetNextRow(this.ptCurrentCell.Y, DataGridViewElementStates.Visible));

                return !nextVisibleColumnExists && !nextVisibleRowExists;
            }
        }

        private bool CurrentCellIsEditedAndOnlySelectedCell
        {
            get
            {
                if (this.ptCurrentCell.X == -1)
                {
                    return false;
                }

                Debug.Assert(this.ptCurrentCell.Y != -1);

                return this.editingControl != null &&
                       GetCellCount(DataGridViewElementStates.Selected) == 1 &&
                       this.CurrentCellInternal.Selected;
            }
        }


        [
            Browsable(false)
        ]
        public DataGridViewRow CurrentRow 
        {
            get
            {
                if (this.ptCurrentCell.X == -1)
                {
                    return null;
                }

                Debug.Assert(this.ptCurrentCell.Y >= 0);
                Debug.Assert(this.ptCurrentCell.Y < this.Rows.Count);

                return this.Rows[this.ptCurrentCell.Y];
            }
        }

        internal Cursor CursorInternal
        {
            set
            {
                this.dataGridViewState2[DATAGRIDVIEWSTATE2_ignoreCursorChange] = true;
                try
                {
                    this.Cursor = value;
                }
                finally
                {
                    this.dataGridViewState2[DATAGRIDVIEWSTATE2_ignoreCursorChange] = false;
                }
            }
        }

        internal DataGridViewDataConnection DataConnection
        {
            get
            {
                return this.dataConnection;
            }
        }


        [
         DefaultValue(""),
         SRCategory(nameof(SR.CatData)),
         Editor("System.Windows.Forms.Design.DataMemberListEditor, " + AssemblyRef.SystemDesign, typeof(System.Drawing.Design.UITypeEditor)),
         SRDescription(nameof(SR.DataGridViewDataMemberDescr))
        ]
        public string DataMember
        {
            get
            {
                if (this.dataConnection == null)
                {
                    return string.Empty;
                }
                else
                {
                    return this.dataConnection.DataMember;
                }
            }
            set
            {
                if (value != this.DataMember)
                {
                    this.CurrentCell = null;
                    if (this.dataConnection == null)
                    {
                        this.dataConnection = new DataGridViewDataConnection(this);
                    }
                    this.dataConnection.SetDataConnection(this.DataSource, value);
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
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWDATAMEMBERCHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWDATAMEMBERCHANGED, value);
            }
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
                if (this.dataConnection == null)
                {
                    return null;
                }
                else
                {
                    return this.dataConnection.DataSource;
                }
            }
            set
            {
                if (value != this.DataSource)
                {
                    this.CurrentCell = null;
                    if (this.dataConnection == null)
                    {
                        this.dataConnection = new DataGridViewDataConnection(this);
                        this.dataConnection.SetDataConnection(value, this.DataMember);
                    }
                    else
                    {
                        if (this.dataConnection.ShouldChangeDataMember(value))
                        {
                            // we fire DataMemberChanged event
                            this.DataMember = "";
                        }
                        this.dataConnection.SetDataConnection(value, this.DataMember);
                        if (value == null)
                        {
                            this.dataConnection = null;
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
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWDATASOURCECHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWDATASOURCECHANGED, value);
            }
        }

        private static SolidBrush DefaultBackBrush 
        {
            get 
            {
                return (SolidBrush) SystemBrushes.Window;
            }
        }

        private static SolidBrush DefaultBackgroundBrush 
        {
            get 
            {
                return (SolidBrush) SystemBrushes.AppWorkspace;
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
                if (this.defaultCellStyle == null)
                {
                    this.defaultCellStyle = this.DefaultDefaultCellStyle;                    
                    return this.defaultCellStyle;
                }
                else if (this.defaultCellStyle.BackColor == Color.Empty || 
                    this.defaultCellStyle.ForeColor == Color.Empty || 
                    this.defaultCellStyle.SelectionBackColor == Color.Empty || 
                    this.defaultCellStyle.SelectionForeColor == Color.Empty || 
                    this.defaultCellStyle.Font == null || 
                    this.defaultCellStyle.Alignment == DataGridViewContentAlignment.NotSet || 
                    this.defaultCellStyle.WrapMode == DataGridViewTriState.NotSet)
                {
                    DataGridViewCellStyle defaultCellStyleTmp = new DataGridViewCellStyle(this.defaultCellStyle);
                    defaultCellStyleTmp.Scope = DataGridViewCellStyleScopes.None;
                    if (this.defaultCellStyle.BackColor == Color.Empty)
                    {
                        defaultCellStyleTmp.BackColor = DefaultBackBrush.Color;
                    }
                    if (this.defaultCellStyle.ForeColor == Color.Empty)
                    {
                        defaultCellStyleTmp.ForeColor = base.ForeColor;
                        this.dataGridViewState1[DATAGRIDVIEWSTATE1_ambientForeColor] = true;
                    }
                    if (this.defaultCellStyle.SelectionBackColor == Color.Empty)
                    {
                        defaultCellStyleTmp.SelectionBackColor = DefaultSelectionBackBrush.Color;
                    }
                    if (this.defaultCellStyle.SelectionForeColor == Color.Empty)
                    {
                        defaultCellStyleTmp.SelectionForeColor = DefaultSelectionForeBrush.Color;
                    }
                    if (this.defaultCellStyle.Font == null)
                    {
                        defaultCellStyleTmp.Font = base.Font;
                        this.dataGridViewState1[DATAGRIDVIEWSTATE1_ambientFont] = true;
                    }
                    if (this.defaultCellStyle.Alignment == DataGridViewContentAlignment.NotSet)
                    {
                        defaultCellStyleTmp.AlignmentInternal = DataGridViewContentAlignment.MiddleLeft;
                    }
                    if (this.defaultCellStyle.WrapMode == DataGridViewTriState.NotSet)
                    {
                        defaultCellStyleTmp.WrapModeInternal = DataGridViewTriState.False;
                    }
                    defaultCellStyleTmp.AddScope(this, DataGridViewCellStyleScopes.DataGridView);
                    return defaultCellStyleTmp;
                }
                else
                {
                    return this.defaultCellStyle;
                }
            }
            set 
            {
                DataGridViewCellStyle cs = this.DefaultCellStyle;
                cs.RemoveScope(DataGridViewCellStyleScopes.DataGridView);
                this.defaultCellStyle = value;
                if (value != null)
                {
                    this.defaultCellStyle.AddScope(this, DataGridViewCellStyleScopes.DataGridView);
                }
                DataGridViewCellStyleDifferences dgvcsc = cs.GetDifferencesFrom(this.DefaultCellStyle);
                if (dgvcsc != DataGridViewCellStyleDifferences.None)
                {
                    this.CellStyleChangedEventArgs.ChangeAffectsPreferredSize = (dgvcsc == DataGridViewCellStyleDifferences.AffectPreferredSize);
                    OnDefaultCellStyleChanged(this.CellStyleChangedEventArgs);
                }
            }
        }

        private DataGridViewCellStyle DefaultDefaultCellStyle 
        {
            get 
            {
                DataGridViewCellStyle defaultCellStyle = new DataGridViewCellStyle();
                defaultCellStyle.BackColor = DefaultBackBrush.Color;
                defaultCellStyle.ForeColor = base.ForeColor;
                defaultCellStyle.SelectionBackColor = DefaultSelectionBackBrush.Color;
                defaultCellStyle.SelectionForeColor = DefaultSelectionForeBrush.Color;
                defaultCellStyle.Font = base.Font;
                defaultCellStyle.AlignmentInternal = DataGridViewContentAlignment.MiddleLeft;
                defaultCellStyle.WrapModeInternal = DataGridViewTriState.False;
                defaultCellStyle.AddScope(this, DataGridViewCellStyleScopes.DataGridView);

                this.dataGridViewState1[DATAGRIDVIEWSTATE1_ambientFont] = true;
                this.dataGridViewState1[DATAGRIDVIEWSTATE1_ambientForeColor] = true;
                
                return defaultCellStyle;
            }
        }


        [
            SRCategory(nameof(SR.CatPropertyChanged)), 
            SRDescription(nameof(SR.DataGridViewDefaultCellStyleChangedDescr))
        ]
        public event EventHandler DefaultCellStyleChanged
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWDEFAULTCELLSTYLECHANGED, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWDEFAULTCELLSTYLECHANGED, value);
            }
        }

        private static SolidBrush DefaultForeBrush 
        {
            get 
            {
                return (SolidBrush) SystemBrushes.WindowText;
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
                return (SolidBrush) SystemBrushes.Control;
            }
        }

        private DataGridViewCellStyle DefaultRowHeadersDefaultCellStyle
        {
            get 
            {
                DataGridViewCellStyle defaultStyle = new DataGridViewCellStyle();
                defaultStyle.BackColor = DefaultHeadersBackBrush.Color;
                defaultStyle.ForeColor = DefaultForeBrush.Color;
                defaultStyle.SelectionBackColor = DefaultSelectionBackBrush.Color;
                defaultStyle.SelectionForeColor = DefaultSelectionForeBrush.Color;
                defaultStyle.Font = base.Font;
                defaultStyle.AlignmentInternal = DataGridViewContentAlignment.MiddleLeft;
                defaultStyle.WrapModeInternal = DataGridViewTriState.True;
                defaultStyle.AddScope(this, DataGridViewCellStyleScopes.RowHeaders);

                this.dataGridViewState1[DATAGRIDVIEWSTATE1_ambientRowHeadersFont] = true;

                return defaultStyle;
            }
        }

        private static SolidBrush DefaultSelectionBackBrush 
        {
            get 
            {
                return (SolidBrush) SystemBrushes.Highlight;
            }
        }
        
        private static SolidBrush DefaultSelectionForeBrush 
        {
            get 
            {
                return (SolidBrush) SystemBrushes.HighlightText;
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
                return this.displayedBandsInfo;
            }
        }


        /// <devdoc>
        ///     Returns the client rect of the display area of the control.
        ///     The DataGridView control return its client rectangle minus the potential scrollbars.
        /// </devdoc>
        public override Rectangle DisplayRectangle {
            get {
                Rectangle rectDisplay = this.ClientRectangle;
                if (this.horizScrollBar != null && this.horizScrollBar.Visible)
                {
                    rectDisplay.Height -= this.horizScrollBar.Height;
                }
                if (this.vertScrollBar != null && this.vertScrollBar.Visible)
                {
                    rectDisplay.Width -= this.vertScrollBar.Width;
                    if (this.RightToLeftInternal)
                    {
                        rectDisplay.X = this.vertScrollBar.Width;
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
                return this.editMode;
            }
            set
            {
                // Sequential enum.  Valid values are 0x0 to 0x4
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DataGridViewEditMode.EditOnEnter, (int)DataGridViewEditMode.EditProgrammatically))
                {
                     throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridViewEditMode)); 
                }               
                if (this.editMode != value)
                {
                    this.editMode = value;
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
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWEDITMODECHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWEDITMODECHANGED, value);
            }
        }

        internal Point MouseEnteredCellAddress
        {
            get
            {
                return this.ptMouseEnteredCell;
            }
        }

        private bool MouseOverEditingControl
        {
            get
            {
                if (this.editingControl != null)
                {
                    Point ptMouse = PointToClient(Control.MousePosition);
                    return this.editingControl.Bounds.Contains(ptMouse);
                }
                return false;
            }
        }

        private bool MouseOverEditingPanel
        {
            get
            {
                if (this.editingPanel != null)
                {
                    Point ptMouse = PointToClient(Control.MousePosition);
                    return this.editingPanel.Bounds.Contains(ptMouse);
                }
                return false;
            }
        }

        private bool MouseOverScrollBar
        {
            get
            {
                Point ptMouse = PointToClient(Control.MousePosition);
                if (this.vertScrollBar != null && this.vertScrollBar.Visible)
                {
                    if (this.vertScrollBar.Bounds.Contains(ptMouse))
                    {
                        return true;
                    }
                }
                if (this.horizScrollBar != null && this.horizScrollBar.Visible)
                {
                    return this.horizScrollBar.Bounds.Contains(ptMouse);
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
                return this.editingControl;
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
                if (this.editingPanel == null)
                {
                    this.editingPanel = AccessibilityImprovements.Level3 ? new DataGridViewEditingPanel(this) : new Panel();
                    this.editingPanel.AccessibleName = string.Format(SR.DataGridView_AccEditingPanelAccName);
                }
                return this.editingPanel;
            }
        }

        internal DataGridViewEditingPanelAccessibleObject EditingPanelAccessibleObject
        {
            get
            {
                if (this.editingPanelAccessibleObject == null)
                {
                    editingPanelAccessibleObject = new DataGridViewEditingPanelAccessibleObject(this, this.EditingPanel);
                }

                return editingPanelAccessibleObject;
            }
        }


        /// <devdoc>
        ///    <para>
        ///     Determines whether the DataGridView's header cells render using XP theming visual styles or not
        ///     when visual styles are enabled in the application.
        ///    </para>
        /// </devdoc>
        [
            SRCategory(nameof(SR.CatAppearance)),
            DefaultValue(true),
            SRDescription(nameof(SR.DataGridView_EnableHeadersVisualStylesDescr))
        ]
        public bool EnableHeadersVisualStyles
        {
            get
            {
                return this.dataGridViewState2[DATAGRIDVIEWSTATE2_enableHeadersVisualStyles];
            }
            set
            {
                if (this.dataGridViewState2[DATAGRIDVIEWSTATE2_enableHeadersVisualStyles] != value)
                {
                    this.dataGridViewState2[DATAGRIDVIEWSTATE2_enableHeadersVisualStyles] = value;
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
                Point firstDisplayedCellAddress = this.FirstDisplayedCellAddress;
                if (firstDisplayedCellAddress.X >= 0)
                {
                    return this.Rows[firstDisplayedCellAddress.Y].Cells[firstDisplayedCellAddress.X]; // unshares the row of first displayed cell
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
                        throw new ArgumentException(string.Format(SR.DataGridView_CellDoesNotBelongToDataGridView));
                    }
                    if (firstDisplayedCell.RowIndex == -1 || firstDisplayedCell.ColumnIndex == -1)
                    {
                        throw new InvalidOperationException(string.Format(SR.DataGridView_FirstDisplayedCellCannotBeAHeaderOrSharedCell));
                    }

                    Debug.Assert(firstDisplayedCell.RowIndex >= 0 &&
                        firstDisplayedCell.RowIndex < this.Rows.Count &&
                        firstDisplayedCell.ColumnIndex >= 0 &&
                        firstDisplayedCell.ColumnIndex < this.Columns.Count);

                    if (!firstDisplayedCell.Visible)
                    {
                        throw new InvalidOperationException(string.Format(SR.DataGridView_FirstDisplayedCellCannotBeInvisible));
                    }

                    if (!firstDisplayedCell.Frozen)
                    {
                        if (!this.Rows[firstDisplayedCell.RowIndex].Frozen)
                        {
                            this.FirstDisplayedScrollingRowIndex = firstDisplayedCell.RowIndex;
                        }

                        if (!this.Columns[firstDisplayedCell.ColumnIndex].Frozen)
                        {
                            this.FirstDisplayedScrollingColumnIndex = firstDisplayedCell.ColumnIndex;
                        }
                    }
                }
            }
        }

        private Point FirstDisplayedCellAddress
        {
            get
            {
                Point ptFirstDisplayedCellAddress = new Point(-1, -1);
                ptFirstDisplayedCellAddress.Y = this.Rows.GetFirstRow(DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen);
                if (ptFirstDisplayedCellAddress.Y == -1)
                {
                    Debug.Assert(this.displayedBandsInfo.NumTotallyDisplayedFrozenRows == 0);
                    if (this.displayedBandsInfo.FirstDisplayedScrollingRow >= 0)
                    {
                        ptFirstDisplayedCellAddress.Y = this.displayedBandsInfo.FirstDisplayedScrollingRow;
                    }
#if DEBUG
                    else
                    {
                        Debug.Assert(this.displayedBandsInfo.FirstDisplayedScrollingRow == -1);
                        Debug.Assert(this.displayedBandsInfo.NumDisplayedScrollingRows == 0);
                        Debug.Assert(this.displayedBandsInfo.NumTotallyDisplayedScrollingRows == 0);
                    }
#endif
                }
                if (ptFirstDisplayedCellAddress.Y >= 0)
                {
                    ptFirstDisplayedCellAddress.X = this.FirstDisplayedColumnIndex;
                }
                return ptFirstDisplayedCellAddress;
            }
        }

        internal int FirstDisplayedColumnIndex
        {
            get
            {
                if (!this.IsHandleCreated)
                {
                    return -1;
                }
                
                int firstDisplayedColumnIndex = -1;
                DataGridViewColumn dataGridViewColumn = this.Columns.GetFirstColumn(DataGridViewElementStates.Visible);
                if (dataGridViewColumn != null)
                {
                    if (dataGridViewColumn.Frozen)
                    {
                        firstDisplayedColumnIndex = dataGridViewColumn.Index;
                    }
                    else if (this.displayedBandsInfo.FirstDisplayedScrollingCol >= 0)
                    {
                        firstDisplayedColumnIndex = this.displayedBandsInfo.FirstDisplayedScrollingCol;
                    }
                }
#if DEBUG
                DataGridViewColumn dataGridViewColumnDbg1 = this.Columns.GetFirstColumn(DataGridViewElementStates.Displayed);
                int firstDisplayedColumnIndexDbg1 = (dataGridViewColumnDbg1 == null) ? -1 : dataGridViewColumnDbg1.Index;

                int firstDisplayedColumnIndexDbg2 = -1;
                DataGridViewColumn dataGridViewColumnDbg = this.Columns.GetFirstColumn(DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen);
                if (dataGridViewColumnDbg != null)
                {
                    firstDisplayedColumnIndexDbg2 = dataGridViewColumnDbg.Index;
                }
                else if (this.displayedBandsInfo.FirstDisplayedScrollingCol >= 0)
                {
                    firstDisplayedColumnIndexDbg2 = this.displayedBandsInfo.FirstDisplayedScrollingCol;
                }
                else
                {
                    Debug.Assert(this.displayedBandsInfo.LastTotallyDisplayedScrollingCol == -1);
                }
                Debug.Assert(firstDisplayedColumnIndex == firstDisplayedColumnIndexDbg1 || !this.Visible || this.displayedBandsInfo.Dirty);
                Debug.Assert(firstDisplayedColumnIndex == firstDisplayedColumnIndexDbg2 || this.displayedBandsInfo.Dirty);
#endif
                return firstDisplayedColumnIndex;
            }
        }

        internal int FirstDisplayedRowIndex
        {
            get
            {
                if (!this.IsHandleCreated)
                {
                    return -1;
                }

                int firstDisplayedRowIndex = this.Rows.GetFirstRow(DataGridViewElementStates.Visible);
                if (firstDisplayedRowIndex != -1)
                {
                    if ((this.Rows.GetRowState(firstDisplayedRowIndex) & DataGridViewElementStates.Frozen) == 0 &&
                        this.displayedBandsInfo.FirstDisplayedScrollingRow >= 0)
                    {
                        firstDisplayedRowIndex = this.displayedBandsInfo.FirstDisplayedScrollingRow;
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
                return this.negOffset;
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
                return this.displayedBandsInfo.FirstDisplayedScrollingCol;
            }
            set
            {
                if (value < 0 || value >= this.Columns.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                if (!this.Columns[value].Visible)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_FirstDisplayedScrollingColumnCannotBeInvisible));
                }
                if (this.Columns[value].Frozen)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_FirstDisplayedScrollingColumnCannotBeFrozen));
                }

                if (!this.IsHandleCreated)
                {
                    CreateHandle();
                }

                int displayWidth = this.layout.Data.Width;
                if (displayWidth <= 0)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_NoRoomForDisplayedColumns));
                }

                int totalVisibleFrozenWidth = this.Columns.GetColumnsWidth(DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen);
                if (totalVisibleFrozenWidth >= displayWidth)
                {
                    Debug.Assert(totalVisibleFrozenWidth > 0);
                    throw new InvalidOperationException(string.Format(SR.DataGridView_FrozenColumnsPreventFirstDisplayedScrollingColumn));
                }

                if (value == this.displayedBandsInfo.FirstDisplayedScrollingCol)
                {
                    return;
                }

                if (this.ptCurrentCell.X >= 0 && 
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

                Debug.Assert(this.displayedBandsInfo.FirstDisplayedScrollingCol >= 0);
                Debug.Assert(this.displayedBandsInfo.FirstDisplayedScrollingCol == value ||
                             this.Columns.DisplayInOrder(this.displayedBandsInfo.FirstDisplayedScrollingCol, value));
                int maxHorizontalOffset = this.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - displayWidth;
                while (this.displayedBandsInfo.FirstDisplayedScrollingCol != value &&
                        this.HorizontalOffset < maxHorizontalOffset)
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
                return this.displayedBandsInfo.FirstDisplayedScrollingRow;
            }
            set
            {
                if (value < 0 || value >= this.Rows.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                if ((this.Rows.GetRowState(value) & DataGridViewElementStates.Visible) == 0)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_FirstDisplayedScrollingRowCannotBeInvisible));
                }
                if ((this.Rows.GetRowState(value) & DataGridViewElementStates.Frozen) != 0)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_FirstDisplayedScrollingRowCannotBeFrozen));
                }

                if (!this.IsHandleCreated)
                {
                    CreateHandle();
                }

                int displayHeight = this.layout.Data.Height;
                if (displayHeight <= 0)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_NoRoomForDisplayedRows));
                }

                int totalVisibleFrozenHeight = this.Rows.GetRowsHeight(DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen);
                if (totalVisibleFrozenHeight >= displayHeight)
                {
                    Debug.Assert(totalVisibleFrozenHeight > 0);
                    throw new InvalidOperationException(string.Format(SR.DataGridView_FrozenRowsPreventFirstDisplayedScrollingRow));
                }

                if (value == this.displayedBandsInfo.FirstDisplayedScrollingRow)
                {
                    return;
                }

                if (this.ptCurrentCell.X >= 0 &&
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

                Debug.Assert(this.displayedBandsInfo.FirstDisplayedScrollingRow >= 0);

                if (value > this.displayedBandsInfo.FirstDisplayedScrollingRow)
                {
                    int rowsToScroll = this.Rows.GetRowCount(DataGridViewElementStates.Visible, this.displayedBandsInfo.FirstDisplayedScrollingRow, value);
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


        /// <internalonly/>
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


        /// <internalonly/>
        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        new public event EventHandler ForeColorChanged
        {
            add
            {
                base.ForeColorChanged += value;
            }
            remove
            {
                base.ForeColorChanged -= value;
            }
        }


        /// <internalonly/>
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


        /// <internalonly/>
        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        new public event EventHandler FontChanged
        {
            add
            {
                base.FontChanged += value;
            }
            remove
            {
                base.FontChanged -= value;
            }
        }


        /// <devdoc>
        ///    <para>Gets or sets the grid color of the dataGridView (when Single mode is used).</para>
        /// </devdoc>
        [
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridViewGridColorDescr))
        ]
        public Color GridColor
        {
            get
            {
                return this.gridPen.Color;
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
                if (!value.Equals(this.gridPen.Color)) 
                {
                    if (this.gridPen != null)
                    {
                        this.gridPen.Dispose();
                    }

                    this.gridPen = new Pen(value);
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
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWGRIDCOLORCHANGED, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWGRIDCOLORCHANGED, value);
            }
        }


        private bool ShouldSerializeGridColor()
        {
            return !this.GridPen.Color.Equals(DefaultGridColor);
        }

        internal Pen GridPen
        {
            get
            {
                return this.gridPen;
            }
        }

        internal int HorizontalOffset 
        {
            get 
            {
                return this.horizontalOffset;
            }
            set 
            {
                if (value < 0)
                {
                    value = 0;
                }
                int widthNotVisible = this.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - this.layout.Data.Width;
                if (value > widthNotVisible && widthNotVisible > 0)
                {
                    value = widthNotVisible;
                }
                if (value == this.horizontalOffset)
                {
                    return;
                }

                ScrollEventType scrollEventType;
                int oldFirstVisibleScrollingCol = this.displayedBandsInfo.FirstDisplayedScrollingCol;
                int change = this.horizontalOffset - value;
                if (this.horizScrollBar.Enabled)
                {
                    this.horizScrollBar.Value = value;
                }
                this.horizontalOffset = value;

                int totalVisibleFrozenWidth = this.Columns.GetColumnsWidth(DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen);

                Rectangle rectTmp = this.layout.Data;
                if (this.layout.ColumnHeadersVisible)
                {
                    // column headers must scroll as well
                    rectTmp = Rectangle.Union(rectTmp, this.layout.ColumnHeaders);
                }
                else if (this.SingleVerticalBorderAdded)
                {
                    if (!this.RightToLeftInternal)
                    {
                        rectTmp.X--;
                    }
                    rectTmp.Width++;
                }

                if (this.SingleVerticalBorderAdded &&
                    totalVisibleFrozenWidth > 0)
                {
                    if (!this.RightToLeftInternal)
                    {
                        rectTmp.X++;
                    }
                    rectTmp.Width--;
                }

                if (!this.RightToLeftInternal)
                {
                    rectTmp.X += totalVisibleFrozenWidth;
                }
                rectTmp.Width -= totalVisibleFrozenWidth;

                this.displayedBandsInfo.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();
                // update the lastTotallyDisplayedScrollingCol
                ComputeVisibleColumns();

                if (this.editingControl != null && 
                    !this.Columns[this.ptCurrentCell.X].Frozen &&
                    this.displayedBandsInfo.FirstDisplayedScrollingCol > -1)
                {
                    PositionEditingControl(true /*setLocation*/, false /*setSize*/, false /*setFocus*/);
                }

                // The mouse probably is not over the same cell after the scroll.
                UpdateMouseEnteredCell(null /*HitTestInfo*/, null /*MouseEventArgs*/);

                if (oldFirstVisibleScrollingCol == this.displayedBandsInfo.FirstDisplayedScrollingCol)
                {
                    scrollEventType = change > 0 ? ScrollEventType.SmallIncrement : ScrollEventType.SmallDecrement;
                }
                else if (this.Columns.DisplayInOrder(oldFirstVisibleScrollingCol, this.displayedBandsInfo.FirstDisplayedScrollingCol))
                {
                    scrollEventType = this.Columns.GetColumnCount(DataGridViewElementStates.Visible, oldFirstVisibleScrollingCol, this.displayedBandsInfo.FirstDisplayedScrollingCol) > 1 ? ScrollEventType.LargeIncrement : ScrollEventType.SmallIncrement;
                }
                else
                {
                    Debug.Assert(this.Columns.DisplayInOrder(this.displayedBandsInfo.FirstDisplayedScrollingCol, oldFirstVisibleScrollingCol));
                    scrollEventType = this.Columns.GetColumnCount(DataGridViewElementStates.Visible, this.displayedBandsInfo.FirstDisplayedScrollingCol, oldFirstVisibleScrollingCol) > 1 ? ScrollEventType.LargeDecrement : ScrollEventType.SmallDecrement;
                }

                NativeMethods.RECT[] rects = CreateScrollableRegion(rectTmp);
                if (this.RightToLeftInternal)
                {
                    change = -change;
                }
                ScrollRectangles(rects, change);
                if (!this.dataGridViewState2[DATAGRIDVIEWSTATE2_stopRaisingHorizontalScroll])
                {
                    OnScroll(scrollEventType, this.horizontalOffset + change, this.horizontalOffset, ScrollOrientation.HorizontalScroll);
                }
                FlushDisplayedChanged();
            }
        }


        protected ScrollBar HorizontalScrollBar
        {
            get
            {
                return this.horizScrollBar;
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
                return this.horizontalOffset;
            }
            set
            {
                // int widthNotVisible = this.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - this.layout.Data.Width;
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(HorizontalScrollingOffset), string.Format(SR.InvalidLowBoundArgumentEx, "HorizontalScrollingOffset", (value).ToString(CultureInfo.CurrentCulture), (0).ToString(CultureInfo.CurrentCulture)));
                }
                // Intentionally ignoring the out of range situation. 
                // else if (value > widthNotVisible && widthNotVisible > 0)
                //{
                //    throw new ArgumentOutOfRangeException(string.Format(SR.DataGridView_PropertyTooLarge, "HorizontalScrollingOffset", (widthNotVisible).ToString()));
                //}
                else if (value > 0 && (this.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - this.layout.Data.Width) <= 0)
                {
                    // Intentionally ignoring the case where dev tries to set value while there is no horizontal scrolling possible.
                    // throw new ArgumentOutOfRangeException(nameof(HorizontalScrollingOffset), string.Format(SR.DataGridView_PropertyMustBeZero));
                    Debug.Assert(this.horizontalOffset == 0);
                    return;
                }
                if (value == this.horizontalOffset)
                {
                    return;
                }
                this.HorizontalOffset = value;
            }
        }

        private System.Windows.Forms.Timer HorizScrollTimer
        {
            get
            {
                if (this.horizScrollTimer == null)
                {
                    this.horizScrollTimer = new System.Windows.Forms.Timer();
                    this.horizScrollTimer.Tick += new System.EventHandler(HorizScrollTimer_Tick);
                }
                return this.horizScrollTimer;
            }
        }

        private bool InAdjustFillingColumns
        {
            get
            {
                return this.dataGridViewOper[DATAGRIDVIEWOPER_inAdjustFillingColumn] || this.dataGridViewOper[DATAGRIDVIEWOPER_inAdjustFillingColumns];
            }
        }

        internal bool InBeginEdit
        {
            get
            {
                return this.dataGridViewOper[DATAGRIDVIEWOPER_inBeginEdit];
            }
        }

        internal bool InDisplayIndexAdjustments
        {
            get
            {
                return this.dataGridViewOper[DATAGRIDVIEWOPER_inDisplayIndexAdjustments];
            }
            set
            {
                this.dataGridViewOper[DATAGRIDVIEWOPER_inDisplayIndexAdjustments] = value;
            }
        }

        internal bool InEndEdit
        {
            get
            {
                return this.dataGridViewOper[DATAGRIDVIEWOPER_inEndEdit];
            }
        }

        private DataGridViewCellStyle InheritedEditingCellStyle
        {
            get
            {
                if (this.ptCurrentCell.X == -1)
                {
                    return null;
                }

                return this.CurrentCellInternal.GetInheritedStyleInternal(this.ptCurrentCell.Y);
            }
        }

        internal bool InInitialization
        {
            get
            {
                return this.dataGridViewState2[DATAGRIDVIEWSTATE2_initializing];
            }
        }

        internal bool InSortOperation
        {
            get
            {
                return this.dataGridViewOper[DATAGRIDVIEWOPER_inSort];
            }
        }


        [Browsable(false)]
        public bool IsCurrentCellDirty
        {
            get
            {
                return this.dataGridViewState1[DATAGRIDVIEWSTATE1_editedCellChanged];
            }
        }

        private bool IsCurrentCellDirtyInternal
        {
            set
            {
                if (value != this.dataGridViewState1[DATAGRIDVIEWSTATE1_editedCellChanged])
                {
                    this.dataGridViewState1[DATAGRIDVIEWSTATE1_editedCellChanged] = value;
                    OnCurrentCellDirtyStateChanged(EventArgs.Empty);
                }
            }
        }


        [Browsable(false)]
        public bool IsCurrentCellInEditMode
        {
            get
            {
                return this.editingControl != null || this.dataGridViewState1[DATAGRIDVIEWSTATE1_currentCellInEditMode];
            }
        }


        // Only used in bound scenarios, when binding to a IEditableObject
        [Browsable(false)]
        public bool IsCurrentRowDirty
        {
            get
            {
                if (!this.VirtualMode)
                {
                    return this.dataGridViewState1[DATAGRIDVIEWSTATE1_editedRowChanged] || this.IsCurrentCellDirty;
                }
                else
                {
                    QuestionEventArgs qe = new QuestionEventArgs(this.dataGridViewState1[DATAGRIDVIEWSTATE1_editedRowChanged] || this.IsCurrentCellDirty);
                    OnRowDirtyStateNeeded(qe);
                    return qe.Response;
                }
            }
        }

        internal bool IsCurrentRowDirtyInternal
        {
            set
            {
                if (value != this.dataGridViewState1[DATAGRIDVIEWSTATE1_editedRowChanged])
                {
                    this.dataGridViewState1[DATAGRIDVIEWSTATE1_editedRowChanged] = value;
                    if (this.RowHeadersVisible && this.ShowEditingIcon && this.ptCurrentCell.Y >= 0)
                    {
                        // Force the pencil to appear in the row header
                        InvalidateCellPrivate(-1, this.ptCurrentCell.Y);
                    }
                }
            }
        }

        private bool IsEscapeKeyEffective
        {
            get
            {
                return this.dataGridViewOper[DATAGRIDVIEWOPER_trackColResize] ||
                       this.dataGridViewOper[DATAGRIDVIEWOPER_trackRowResize] ||
                       this.dataGridViewOper[DATAGRIDVIEWOPER_trackColHeadersResize] ||
                       this.dataGridViewOper[DATAGRIDVIEWOPER_trackRowHeadersResize] ||
                       this.dataGridViewOper[DATAGRIDVIEWOPER_trackColRelocation] ||
                       this.IsCurrentCellDirty ||
                       ((this.VirtualMode || this.DataSource != null) && this.IsCurrentRowDirty) ||
                       (this.EditMode != DataGridViewEditMode.EditOnEnter && this.editingControl != null ||
                       this.dataGridViewState1[DATAGRIDVIEWSTATE1_newRowEdited]);
            }
        }

        private bool IsMinimized
        {
            get
            {
                Form parentForm = this.TopLevelControlInternal as Form;
                return parentForm != null && parentForm.WindowState == FormWindowState.Minimized;
            }
        }

        internal bool IsRestricted
        {
            get
            {
                if (!this.dataGridViewState1[DATAGRIDVIEWSTATE1_isRestrictedChecked])
                {
                    this.dataGridViewState1[DATAGRIDVIEWSTATE1_isRestricted] = false;
                    this.dataGridViewState1[DATAGRIDVIEWSTATE1_isRestrictedChecked] = true;
                }
                return this.dataGridViewState1[DATAGRIDVIEWSTATE1_isRestricted];
            }
        }

        private bool IsSharedCellReadOnly(DataGridViewCell dataGridViewCell, int rowIndex)
        {
            Debug.Assert(dataGridViewCell != null);
            Debug.Assert(rowIndex >= 0);
            DataGridViewElementStates rowState = this.Rows.GetRowState(rowIndex); 
            return this.ReadOnly ||
                   (rowState & DataGridViewElementStates.ReadOnly) != 0 ||
                   (dataGridViewCell.OwningColumn != null && dataGridViewCell.OwningColumn.ReadOnly) ||
                   dataGridViewCell.StateIncludes(DataGridViewElementStates.ReadOnly);
        }

        internal bool IsSharedCellSelected(DataGridViewCell dataGridViewCell, int rowIndex)
        {
            Debug.Assert(dataGridViewCell != null);
            Debug.Assert(rowIndex >= 0);
            DataGridViewElementStates rowState = this.Rows.GetRowState(rowIndex);
            return (rowState & DataGridViewElementStates.Selected) != 0 ||
                   (dataGridViewCell.OwningColumn != null && dataGridViewCell.OwningColumn.Selected) ||
                   dataGridViewCell.StateIncludes(DataGridViewElementStates.Selected);
        }

        internal bool IsSharedCellVisible(DataGridViewCell dataGridViewCell, int rowIndex)
        {
            Debug.Assert(dataGridViewCell != null);
            Debug.Assert(rowIndex >= 0);
            DataGridViewElementStates rowState = this.Rows.GetRowState(rowIndex);
            return (rowState & DataGridViewElementStates.Visible) != 0 &&
                   (dataGridViewCell.OwningColumn != null && dataGridViewCell.OwningColumn.Visible);
        }

        internal LayoutData LayoutInfo
        {
            get
            {
                if (this.layout.dirty && this.IsHandleCreated)
                {
                    PerformLayoutPrivate(false /*useRowShortcut*/, true /*computeVisibleRows*/, false /*invalidInAdjustFillingColumns*/, false /*repositionEditingControl*/);
                }
                return this.layout;
            }
        }

        internal Point MouseDownCellAddress
        {
            get
            {
                return this.ptMouseDownCell;
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
                return this.dataGridViewState1[DATAGRIDVIEWSTATE1_multiSelect];
            }
            set
            {
                if (this.MultiSelect != value)
                {
                    ClearSelection();
                    this.dataGridViewState1[DATAGRIDVIEWSTATE1_multiSelect] = value;
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
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWMULTISELECTCHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWMULTISELECTCHANGED, value);
            }
        }


        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public int NewRowIndex
        {
            get
            {
                return this.newRowIndex;
            }
        }

        internal bool NoDimensionChangeAllowed
        {
            get
            {
                return this.noDimensionChangeCount > 0;
            }
        }

        private int NoSelectionChangeCount
        {
            get
            {
                return this.noSelectionChangeCount;
            }
            set
            {
                Debug.Assert(value >= 0);
                this.noSelectionChangeCount = value;
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
            add
            {
                base.PaddingChanged += value;
            }
            remove
            {
                base.PaddingChanged -= value;
            }
        }

        internal DataGridViewCellStyle PlaceholderCellStyle
        {
            get
            {
                if (this.placeholderCellStyle == null)
                {
                    this.placeholderCellStyle = new DataGridViewCellStyle();
                }
                return this.placeholderCellStyle;
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
                return this.dataGridViewState1[DATAGRIDVIEWSTATE1_readOnly];
            }
            set
            {
                if (value != this.dataGridViewState1[DATAGRIDVIEWSTATE1_readOnly])
                {
                    if (value &&
                        this.ptCurrentCell.X != -1 &&
                        this.IsCurrentCellInEditMode)
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
                            throw new InvalidOperationException(string.Format(SR.DataGridView_CommitFailedCannotCompleteOperation));
                        }
                    }

                    this.dataGridViewState1[DATAGRIDVIEWSTATE1_readOnly] = value;

                    if (value)
                    {
                        try
                        {
                            this.dataGridViewOper[DATAGRIDVIEWOPER_inReadOnlyChange] = true;
                            for (int columnIndex = 0; columnIndex < this.Columns.Count; columnIndex++)
                            {
                                SetReadOnlyColumnCore(columnIndex, false);
                            }
                            int rowCount = this.Rows.Count;
                            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                            {
                                SetReadOnlyRowCore(rowIndex, false);
                            }
                        }
                        finally
                        {
                            this.dataGridViewOper[DATAGRIDVIEWOPER_inReadOnlyChange] = false;
                        }
                    }
#if DEBUG
                    else
                    {
                        Debug.Assert(this.individualReadOnlyCells.Count == 0);
                        for (int columnIndex = 0; columnIndex < this.Columns.Count; columnIndex++)
                        {
                            Debug.Assert(this.Columns[columnIndex].ReadOnly == false);
                        }
                        int rowCount = this.Rows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            Debug.Assert((this.Rows.GetRowState(rowIndex) & DataGridViewElementStates.ReadOnly) == 0);
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
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWREADONLYCHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWREADONLYCHANGED, value);
            }
        }

        private void ResetCurrentCell()
        {
            if (this.ptCurrentCell.X != -1 &&
                !SetCurrentCellAddressCore(-1, -1, true /*setAnchorCellAddress*/, true /*validateCurrentCell*/, false /*throughMouseClick*/))
            {
                // Edited value couldn't be committed or aborted
                throw new InvalidOperationException(string.Format(SR.DataGridView_CellChangeCannotBeCommittedOrAborted));
            }
        }

        internal bool ResizingOperationAboutToStart
        {
            get
            {
                return this.dataGridViewOper[DATAGRIDVIEWOPER_resizingOperationAboutToStart];
            }
        }

        internal bool RightToLeftInternal
        {
            get
            {
                if (this.dataGridViewState2[DATAGRIDVIEWSTATE2_rightToLeftValid])
                {
                    return this.dataGridViewState2[DATAGRIDVIEWSTATE2_rightToLeftMode];
                }
                this.dataGridViewState2[DATAGRIDVIEWSTATE2_rightToLeftMode] = (this.RightToLeft == RightToLeft.Yes);
                this.dataGridViewState2[DATAGRIDVIEWSTATE2_rightToLeftValid] = true;
                return this.dataGridViewState2[DATAGRIDVIEWSTATE2_rightToLeftMode];
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
                return this.Rows.Count;
            }
            set
            {
                if (this.AllowUserToAddRowsInternal)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(RowCount), string.Format(SR.InvalidLowBoundArgumentEx, "RowCount", value.ToString(CultureInfo.CurrentCulture), (1).ToString(CultureInfo.CurrentCulture)));
                    }
                }
                else
                {
                    if (value < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(RowCount), string.Format(SR.InvalidLowBoundArgumentEx, "RowCount", value.ToString(CultureInfo.CurrentCulture), (0).ToString(CultureInfo.CurrentCulture)));
                    }
                }
                if (this.DataSource != null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_CannotSetRowCountOnDataBoundDataGridView));
                }
                if (value != this.Rows.Count)
                {
                    if (value == 0)
                    {
                        // Total removal of the rows.
                        this.Rows.Clear();
                    }
                    else if (value < this.Rows.Count)
                    {
                        // Some rows need to be removed, from the tail of the rows collection
                        while (value < this.Rows.Count)
                        {
                            int currentRowCount = this.Rows.Count;
                            this.Rows.RemoveAt(currentRowCount - (this.AllowUserToAddRowsInternal ? 2 :  1));
                            if (this.Rows.Count >= currentRowCount)
                            {
                                // Row removal failed. We stop the loop.
                                break;
                            }
                        }
                    }
                    else
                    {
                        // Some rows need to be appened.
                        if (this.Columns.Count == 0)
                        {
                            // There are no columns yet, we simply create a single DataGridViewTextBoxColumn.
                            DataGridViewTextBoxColumn dataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
                            this.Columns.Add(dataGridViewTextBoxColumn);
                        }
                        int rowsToAdd = value - this.Rows.Count;
                        if (rowsToAdd > 0)
                        {
                            this.Rows.Add(rowsToAdd);
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
                switch (this.advancedRowHeadersBorderStyle.All)
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

                if (value != this.RowHeadersBorderStyle)
                {
                    if (value == DataGridViewHeaderBorderStyle.Custom)
                    {
                        throw new ArgumentException(string.Format(SR.DataGridView_CustomCellBorderStyleInvalid, "RowHeadersBorderStyle"));
                    }
                    this.dataGridViewOper[DATAGRIDVIEWOPER_inBorderStyleChange] = true;
                    try
                    {
                        switch (value)
                        {
                            case DataGridViewHeaderBorderStyle.Single:
                                this.advancedRowHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Single;
                                break;

                            case DataGridViewHeaderBorderStyle.Raised:
                                this.advancedRowHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.OutsetPartial;
                                break;

                            case DataGridViewHeaderBorderStyle.Sunken:
                                this.advancedRowHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.InsetDouble;
                                break;

                            case DataGridViewHeaderBorderStyle.None:
                                this.advancedRowHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                                break;
                        }
                    }
                    finally
                    {
                        this.dataGridViewOper[DATAGRIDVIEWOPER_inBorderStyleChange] = false;
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
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWROWHEADERSBORDERSTYLECHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWROWHEADERSBORDERSTYLECHANGED, value);
            }
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
                if (this.rowHeadersDefaultCellStyle == null)
                {
                    this.rowHeadersDefaultCellStyle = this.DefaultRowHeadersDefaultCellStyle;
                }
                return this.rowHeadersDefaultCellStyle;
            }
            set 
            {
                DataGridViewCellStyle cs = this.RowHeadersDefaultCellStyle;
                cs.RemoveScope(DataGridViewCellStyleScopes.RowHeaders);
                this.rowHeadersDefaultCellStyle = value;
                if (value != null)
                {
                    this.rowHeadersDefaultCellStyle.AddScope(this, DataGridViewCellStyleScopes.RowHeaders);
                }
                DataGridViewCellStyleDifferences dgvcsc = cs.GetDifferencesFrom(this.RowHeadersDefaultCellStyle);
                if (dgvcsc != DataGridViewCellStyleDifferences.None)
                {
                    this.CellStyleChangedEventArgs.ChangeAffectsPreferredSize = (dgvcsc == DataGridViewCellStyleDifferences.AffectPreferredSize);
                    OnRowHeadersDefaultCellStyleChanged(this.CellStyleChangedEventArgs);
                }
            }
        }


        [
            SRCategory(nameof(SR.CatPropertyChanged)), 
            SRDescription(nameof(SR.DataGridViewRowHeadersDefaultCellStyleChangedDescr))
        ]
        public event EventHandler RowHeadersDefaultCellStyleChanged
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWROWHEADERSDEFAULTCELLSTYLECHANGED, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWROWHEADERSDEFAULTCELLSTYLECHANGED, value);
            }
        }
        

        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether the dataGridView's row headers are
        ///       visible.
        ///    </para>
        /// </devdoc>
        [
            SRCategory(nameof(SR.CatAppearance)),
            DefaultValue(true),
            SRDescription(nameof(SR.DataGridViewRowHeadersVisibleDescr))
        ]
        public bool RowHeadersVisible
        {
            get 
            {
                return this.dataGridViewState1[DATAGRIDVIEWSTATE1_rowHeadersVisible];
            }
            set 
            {
                if (this.RowHeadersVisible != value)
                {
                    if (!value &&
                        (this.autoSizeRowsMode == DataGridViewAutoSizeRowsMode.AllHeaders || this.autoSizeRowsMode == DataGridViewAutoSizeRowsMode.DisplayedHeaders))
                    {
                        throw new InvalidOperationException(string.Format(SR.DataGridView_RowHeadersCannotBeInvisible));
                    }
                    using (LayoutTransaction.CreateTransactionIf(this.AutoSize, this.ParentInternal, this, PropertyNames.RowHeadersVisible))
                    {
                        this.dataGridViewState1[DATAGRIDVIEWSTATE1_rowHeadersVisible] = value;
                        this.layout.RowHeadersVisible = value;
                        this.displayedBandsInfo.EnsureDirtyState();
                        if (!this.AutoSize)
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
                return this.rowHeadersWidth;
            }
            set 
            {
                if (value < minimumRowHeadersWidth)
                {
                    throw new ArgumentOutOfRangeException(nameof(RowHeadersWidth), string.Format(SR.InvalidLowBoundArgumentEx, "RowHeadersWidth", (value).ToString(CultureInfo.CurrentCulture), (minimumRowHeadersWidth).ToString(CultureInfo.CurrentCulture)));
                }
                if (value > maxHeadersThickness)
                {
                    throw new ArgumentOutOfRangeException(nameof(RowHeadersWidth), string.Format(SR.InvalidHighBoundArgumentEx, "RowHeadersWidth", (value).ToString(CultureInfo.CurrentCulture), (maxHeadersThickness).ToString(CultureInfo.CurrentCulture)));
                }
                if (this.RowHeadersWidthSizeMode != DataGridViewRowHeadersWidthSizeMode.EnableResizing &&
                    this.RowHeadersWidthSizeMode != DataGridViewRowHeadersWidthSizeMode.DisableResizing)
                {
                    this.cachedRowHeadersWidth = value;
                }
                else if (this.rowHeadersWidth != value)
                {
                    this.RowHeadersWidthInternal = value;
                }
            }
        }

        private int RowHeadersWidthInternal
        {
            set
            {
                using (LayoutTransaction.CreateTransactionIf(this.AutoSize, this.ParentInternal, this, PropertyNames.RowHeadersWidth))
                {
                    Debug.Assert(this.rowHeadersWidth != value);
                    Debug.Assert(value >= minimumRowHeadersWidth);
                    this.rowHeadersWidth = value;
                    if (this.AutoSize)
                    {
                        InvalidateInside();
                    }
                    else
                    {
                        if (this.layout.RowHeadersVisible)
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
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWROWHEADERSWIDTHCHANGED, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWROWHEADERSWIDTHCHANGED, value);
            }
        }

        private bool ShouldSerializeRowHeadersWidth()
        {
            return (this.rowHeadersWidthSizeMode == DataGridViewRowHeadersWidthSizeMode.EnableResizing || this.rowHeadersWidthSizeMode == DataGridViewRowHeadersWidthSizeMode.DisableResizing) && 
                   defaultRowHeadersWidth != this.RowHeadersWidth;
        }


        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value that determines the behavior for adjusting the row headers width.
        ///    </para>
        /// </devdoc>
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
                return this.rowHeadersWidthSizeMode;
            }
            set
            {
                // Sequential enum.  Valid values are 0x0 to 0x4
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DataGridViewRowHeadersWidthSizeMode.EnableResizing, (int)DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridViewRowHeadersWidthSizeMode)); 
                }
                if (this.rowHeadersWidthSizeMode != value)
                {
                    /*if (value != DataGridViewRowHeadersWidthSizeMode.EnableResizing && 
                     *    value != DataGridViewRowHeadersWidthSizeMode.DisableResizing &&
                     *    !this.RowHeadersVisible)
                    {
                        We intentionally don't throw an error because of designer code spit order.
                    }*/
                    DataGridViewAutoSizeModeEventArgs dgvasme = new DataGridViewAutoSizeModeEventArgs(this.rowHeadersWidthSizeMode != DataGridViewRowHeadersWidthSizeMode.EnableResizing &&
                                                                                                      this.rowHeadersWidthSizeMode != DataGridViewRowHeadersWidthSizeMode.DisableResizing);
                    this.rowHeadersWidthSizeMode = value;
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
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWROWHEADERSWIDTHSIZEMODECHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWROWHEADERSWIDTHSIZEMODECHANGED, value);
            }
        }


        [
            Browsable(false)
        ]
        public DataGridViewRowCollection Rows
        {
            get
            {
                if (this.dataGridViewRows == null)
                {
                    this.dataGridViewRows = CreateRowsInstance();
                }
                return this.dataGridViewRows;
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
                if (this.rowsDefaultCellStyle == null)
                {
                    this.rowsDefaultCellStyle = new DataGridViewCellStyle();
                    this.rowsDefaultCellStyle.AddScope(this, DataGridViewCellStyleScopes.Rows);
                }
                return this.rowsDefaultCellStyle;
            }
            set
            {
                DataGridViewCellStyle cs = this.RowsDefaultCellStyle;
                cs.RemoveScope(DataGridViewCellStyleScopes.Rows);
                this.rowsDefaultCellStyle = value;
                if (value != null)
                {
                    this.rowsDefaultCellStyle.AddScope(this, DataGridViewCellStyleScopes.Rows);
                }
                DataGridViewCellStyleDifferences dgvcsc = cs.GetDifferencesFrom(this.RowsDefaultCellStyle);
                if (dgvcsc != DataGridViewCellStyleDifferences.None)
                {
                    this.CellStyleChangedEventArgs.ChangeAffectsPreferredSize = (dgvcsc == DataGridViewCellStyleDifferences.AffectPreferredSize);
                    OnRowsDefaultCellStyleChanged(this.CellStyleChangedEventArgs);
                }
            }
        }


        [
            SRCategory(nameof(SR.CatPropertyChanged)), 
            SRDescription(nameof(SR.DataGridViewRowsDefaultCellStyleChangedDescr))
        ]
        public event EventHandler RowsDefaultCellStyleChanged
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWROWSDEFAULTCELLSTYLECHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWROWSDEFAULTCELLSTYLECHANGED, value);
            }
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
                if (this.rowTemplate == null)
                {
                    this.rowTemplate = new DataGridViewRow();
                }
                return this.rowTemplate;
            }
            set
            {
                DataGridViewRow dataGridViewRow = value;
                if (dataGridViewRow != null)
                {
                    if (dataGridViewRow.DataGridView != null)
                    {
                        throw new InvalidOperationException(string.Format(SR.DataGridView_RowAlreadyBelongsToDataGridView));
                    }
                    //if (dataGridViewRow.Selected)
                    //{
                    //    throw new InvalidOperationException(string.Format(SR.DataGridView_RowTemplateCannotBeSelected));
                    //}
                }
                this.rowTemplate = dataGridViewRow;
            }
        }

        private bool ShouldSerializeRowTemplate()
        {
            return this.rowTemplate != null;
        }

        internal DataGridViewRow RowTemplateClone
        {
            get
            {
                DataGridViewRow rowTemplateClone = (DataGridViewRow) this.RowTemplate.Clone();
                CompleteCellsCollection(rowTemplateClone);
                return rowTemplateClone;
            }
        }


        /// <devdoc>
        ///     Possible return values are given by the ScrollBars enumeration.
        /// </devdoc>
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
                return this.scrollBars;
            }
            set
            {
                // Sequential enum.  Valid values are 0x0 to 0x3
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)ScrollBars.None, (int)ScrollBars.Both))
                {
                     throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ScrollBars)); 
                }

                if (this.scrollBars != value)
                {
                    using (LayoutTransaction.CreateTransactionIf(this.AutoSize, this.ParentInternal, this, PropertyNames.ScrollBars))
                    {
                        // Before changing the value of this.scrollBars, we scroll to the top-left cell to
                        // avoid inconsitent state of scrollbars.
                        DataGridViewColumn dataGridViewColumn = this.Columns.GetFirstColumn(DataGridViewElementStates.Visible);
                        int firstVisibleRowIndex = this.Rows.GetFirstRow(DataGridViewElementStates.Visible);

                        if (dataGridViewColumn != null && firstVisibleRowIndex != -1)
                        {
                            if (!ScrollIntoView(dataGridViewColumn.Index, firstVisibleRowIndex, false))
                            {
                                throw new InvalidOperationException(string.Format(SR.DataGridView_CellChangeCannotBeCommittedOrAborted));
                            }
                        }
                        Debug.Assert(this.HorizontalOffset == 0);
                        Debug.Assert(this.VerticalOffset == 0);

                        this.scrollBars = value;

                        if (!this.AutoSize)
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
            [
                SuppressMessage("Microsoft.Performance", "CA1817:DoNotCallPropertiesThatCloneValuesInLoops"), // not legitimate
                SuppressMessage("Microsoft.Performance", "CA1808:AvoidCallsThatBoxValueTypes") // consider using generics instead of DataGridViewIntLinkedList
            ]
            get
            {
                DataGridViewSelectedCellCollection stcc = new DataGridViewSelectedCellCollection();
                switch (this.SelectionMode)
                {
                    case DataGridViewSelectionMode.CellSelect:
                    {
                        // Note: If we change the design and decide that SelectAll() should use band selection, 
                        // we need to add those to the selected cells.
                        stcc.AddCellLinkedList(this.individualSelectedCells);
                        break;
                    }
                    case DataGridViewSelectionMode.FullColumnSelect:
                    case DataGridViewSelectionMode.ColumnHeaderSelect:
                    {
                        foreach (int columnIndex in this.selectedBandIndexes)
                        {
                            foreach (DataGridViewRow dataGridViewRow in this.Rows)   // unshares all rows!
                            {
                                stcc.Add(dataGridViewRow.Cells[columnIndex]);
                            }
                        }
                        if (this.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect)
                        {
                            stcc.AddCellLinkedList(this.individualSelectedCells);
                        }
                        break;
                    }
                    case DataGridViewSelectionMode.FullRowSelect:
                    case DataGridViewSelectionMode.RowHeaderSelect:
                    {
                        foreach (int rowIndex in this.selectedBandIndexes)
                        {
                            DataGridViewRow dataGridViewRow = (DataGridViewRow) this.Rows[rowIndex]; // unshares the selected row
                            foreach (DataGridViewCell dataGridViewCell in dataGridViewRow.Cells)
                            {
                                stcc.Add(dataGridViewCell);
                            }
                        }
                        if (this.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect)
                        {
                            stcc.AddCellLinkedList(this.individualSelectedCells);
                        }
                        break;
                    }
                }
                return stcc;
            }
        }


        [
            Browsable(false)
        ]
        public DataGridViewSelectedColumnCollection SelectedColumns
        {
            [
                SuppressMessage("Microsoft.Performance", "CA1808:AvoidCallsThatBoxValueTypes") // consider using generics instead of DataGridViewIntLinkedList
            ]
            get
            {
                DataGridViewSelectedColumnCollection strc = new DataGridViewSelectedColumnCollection();
                switch (this.SelectionMode)
                {
                    case DataGridViewSelectionMode.CellSelect:
                    case DataGridViewSelectionMode.FullRowSelect:
                    case DataGridViewSelectionMode.RowHeaderSelect:
                        break;
                    case DataGridViewSelectionMode.FullColumnSelect:
                    case DataGridViewSelectionMode.ColumnHeaderSelect:
                        foreach (int columnIndex in this.selectedBandIndexes)
                        {
                            strc.Add(this.Columns[columnIndex]);
                        }
                        break;
                }
                return strc;
            }
        }
        

        [
            Browsable(false),
        ]
        public DataGridViewSelectedRowCollection SelectedRows
        {
            [
                SuppressMessage("Microsoft.Performance", "CA1808:AvoidCallsThatBoxValueTypes"), // using specialized DataGridViewIntLinkedList class instead of generics
                SuppressMessage("Microsoft.Performance", "CA1817:DoNotCallPropertiesThatCloneValuesInLoops") // not legitimate
            ]
            get
            {
                DataGridViewSelectedRowCollection strc = new DataGridViewSelectedRowCollection();
                switch (this.SelectionMode)
                {
                    case DataGridViewSelectionMode.CellSelect:
                    case DataGridViewSelectionMode.FullColumnSelect:
                    case DataGridViewSelectionMode.ColumnHeaderSelect:
                        break;
                    case DataGridViewSelectionMode.FullRowSelect:
                    case DataGridViewSelectionMode.RowHeaderSelect:
                        foreach (int rowIndex in this.selectedBandIndexes)
                        {
                            strc.Add((DataGridViewRow) this.Rows[rowIndex]); // unshares the selected row
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
                return this.selectionMode;
            }
            set
            {
                // Sequential enum.  Valid values are 0x0 to 0x4
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DataGridViewSelectionMode.CellSelect, (int)DataGridViewSelectionMode.ColumnHeaderSelect))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridViewSelectionMode)); 
                }

                if (this.SelectionMode != value)
                {
                    if (!this.dataGridViewState2[DATAGRIDVIEWSTATE2_initializing] &&
                        (value == DataGridViewSelectionMode.FullColumnSelect || value == DataGridViewSelectionMode.ColumnHeaderSelect))
                    {
                        foreach (DataGridViewColumn dataGridViewColumn in this.Columns)
                        {
                            if (dataGridViewColumn.SortMode == DataGridViewColumnSortMode.Automatic)
                            {
                                throw new InvalidOperationException(string.Format(SR.DataGridView_SelectionModeAndSortModeClash, (value).ToString()));
                            }
                        }
                    }
                    ClearSelection();
                    this.selectionMode = value;
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
                return this.dataGridViewState2[DATAGRIDVIEWSTATE2_showCellErrors];
            }
            set
            {
                if (this.ShowCellErrors != value)
                {
                    this.dataGridViewState2[DATAGRIDVIEWSTATE2_showCellErrors] = value;

                    // Put this into OnShowCellErrorsChanged if created.
                    if (this.IsHandleCreated && !this.DesignMode)
                    {
                        if (value && !this.ShowRowErrors && !this.ShowCellToolTips)
                        {
                            // the tool tip hasn't yet been activated
                            // activate it now
                            this.toolTipControl.Activate(!string.IsNullOrEmpty(this.toolTipCaption));
                        }

                        if (!value && !this.ShowRowErrors && !this.ShowCellToolTips)
                        {
                            // there is no reason to keep the tool tip activated
                            // deactivate it
                            this.toolTipCaption = string.Empty;
                            this.toolTipControl.Activate(false /*activate*/);
                        }

                        if (!value && (this.ShowRowErrors || this.ShowCellToolTips))
                        {
                            // reset the tool tip
                            this.toolTipControl.Activate(!string.IsNullOrEmpty(this.toolTipCaption));
                        }

                        // Some autosizing may have to be applied since the potential presence of error icons influences the preferred sizes.
                        OnGlobalAutoSize();
                    }

                    if (!this.layout.dirty && !this.DesignMode)
                    {
                        this.Invalidate(Rectangle.Union(this.layout.Data, this.layout.ColumnHeaders));
                        this.Invalidate(this.layout.TopLeftHeader);
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
                return this.dataGridViewState2[DATAGRIDVIEWSTATE2_showCellToolTips];
            }
            set
            {
                if (this.ShowCellToolTips != value)
                {
                    this.dataGridViewState2[DATAGRIDVIEWSTATE2_showCellToolTips] = value;

                    if (this.IsHandleCreated && !this.DesignMode)
                    {
                        if (value && !this.ShowRowErrors && !this.ShowCellErrors)
                        {
                            // the tool tip hasn't yet been activated
                            // activate it now
                            this.toolTipControl.Activate(!string.IsNullOrEmpty(this.toolTipCaption) /*activate*/);
                        }

                        if (!value && !this.ShowRowErrors && !this.ShowCellErrors)
                        {
                            // there is no reason to keep the tool tip activated
                            // deactivate it
                            this.toolTipCaption = string.Empty;
                            this.toolTipControl.Activate(false /*activate*/);
                        }

                        if (!value && (this.ShowRowErrors || this.ShowCellErrors))
                        {
                            bool activate = !string.IsNullOrEmpty(this.toolTipCaption);
                            Point mouseCoord = System.Windows.Forms.Control.MousePosition;
                            activate &= this.ClientRectangle.Contains(PointToClient(mouseCoord));

                            // reset the tool tip
                            this.toolTipControl.Activate(activate);
                        }
                    }

                    if (!this.layout.dirty && !this.DesignMode)
                    {
                        Invalidate(this.layout.Data);
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
                return this.dataGridViewState2[DATAGRIDVIEWSTATE2_showEditingIcon];
            }
            set
            {
                if (this.ShowEditingIcon != value)
                {
                    this.dataGridViewState2[DATAGRIDVIEWSTATE2_showEditingIcon] = value;

                    // invalidate the row header to pick up the new ShowEditingIcon value 
                    if (this.RowHeadersVisible)
                    {
                        if (this.VirtualMode || this.DataSource != null)
                        {
                            if (this.IsCurrentRowDirty)
                            {
                                Debug.Assert(this.ptCurrentCell.Y >= 0);
                                InvalidateCellPrivate(-1, this.ptCurrentCell.Y);
                            }
                        }
                        else
                        {
                            if (this.IsCurrentCellDirty) {
                                Debug.Assert(this.ptCurrentCell.Y >= 0);
                                InvalidateCellPrivate(-1, this.ptCurrentCell.Y);
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
                return this.dataGridViewState2[DATAGRIDVIEWSTATE2_showRowErrors];
            }
            set 
            {
                if (this.ShowRowErrors != value)
                {
                    this.dataGridViewState2[DATAGRIDVIEWSTATE2_showRowErrors] = value;

                    if (this.IsHandleCreated && !this.DesignMode)
                    {
                        if (value && !this.ShowCellErrors && !this.ShowCellToolTips)
                        {
                            // the tool tip hasn't yet been activated
                            // activate it now
                            this.toolTipControl.Activate(!string.IsNullOrEmpty(this.toolTipCaption));
                        }

                        if (!value && !this.ShowCellErrors && !this.ShowCellToolTips)
                        {
                            // there is no reason to keep the tool tip activated
                            // deactivate it
                            this.toolTipCaption = string.Empty;
                            this.toolTipControl.Activate(false /*activate*/);
                        }

                        if (!value && (this.ShowCellErrors || this.ShowCellToolTips))
                        {
                            // reset the tool tip
                            this.toolTipControl.Activate(!string.IsNullOrEmpty(this.toolTipCaption));
                        }
                    }

                    if (!this.layout.dirty && !this.DesignMode)
                    {
                        Invalidate(this.layout.RowHeaders);
                    }
                }
            }
        }

        internal bool SingleHorizontalBorderAdded
        {
            get
            {
                return !this.layout.ColumnHeadersVisible && 
                    (this.AdvancedCellBorderStyle.All == DataGridViewAdvancedCellBorderStyle.Single ||
                     this.CellBorderStyle == DataGridViewCellBorderStyle.SingleHorizontal);
            }
        }

        internal bool SingleVerticalBorderAdded
        {
            get
            {
                return !this.layout.RowHeadersVisible && 
                    (this.AdvancedCellBorderStyle.All == DataGridViewAdvancedCellBorderStyle.Single ||
                     this.CellBorderStyle == DataGridViewCellBorderStyle.SingleVertical);
            }
        }


        [
            Browsable(false)
        ]
        public DataGridViewColumn SortedColumn
        {
            get
            {
                return this.sortedColumn;
            }
        }


        [
            Browsable(false)
        ]
        public SortOrder SortOrder
        {
            get
            {
                return this.sortOrder;
            }
        }


        /// <devdoc>
        ///    <para>
        ///    </para>
        /// </devdoc>
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
                return this.dataGridViewState1[DATAGRIDVIEWSTATE1_standardTab];
            }
            set
            {
                if (this.dataGridViewState1[DATAGRIDVIEWSTATE1_standardTab] != value)
                {
                    this.dataGridViewState1[DATAGRIDVIEWSTATE1_standardTab] = value;
                    //OnStandardTabChanged(EventArgs.Empty);
                }
            }
        }

        internal override bool SupportsUiaProviders
        {
            get
            {
                return AccessibilityImprovements.Level3;
            }
        }


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
            add
            {
                base.TextChanged += value;
            }
            remove
            {
                base.TextChanged -= value;
            }
        }


        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            SuppressMessage("Microsoft.Design", "CA1023:IndexersShouldNotBeMultidimensional")
        ]
        public DataGridViewCell this[int columnIndex, int rowIndex]
        {
            get
            {
                DataGridViewRow row = this.Rows[rowIndex];
                return row.Cells[columnIndex];
            }
            set
            {
                DataGridViewRow row = this.Rows[rowIndex];
                row.Cells[columnIndex] = value;
            }
        }


        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            SuppressMessage("Microsoft.Design", "CA1023:IndexersShouldNotBeMultidimensional")
        ]
        public DataGridViewCell this[string columnName, int rowIndex]
        {
            get
            {
                DataGridViewRow row = this.Rows[rowIndex];
                return row.Cells[columnName];
            }
            set
            {
                DataGridViewRow row = this.Rows[rowIndex];
                row.Cells[columnName] = value;
            }
        }

        private string ToolTipPrivate
        {
            get
            {
                return this.toolTipCaption;
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
                if (this.topLeftHeaderCell == null)
                {
                    this.TopLeftHeaderCell = new DataGridViewTopLeftHeaderCell();
                }
                return this.topLeftHeaderCell;
            }
            set
            {
                if (this.topLeftHeaderCell != value)
                {
                    if (this.topLeftHeaderCell != null)
                    {
                        // Detach existing header cell
                        this.topLeftHeaderCell.DataGridViewInternal = null;
                    }
                    this.topLeftHeaderCell = value;
                    if (value != null)
                    {
                        this.topLeftHeaderCell.DataGridViewInternal = this;
                    }
                    if (this.ColumnHeadersVisible && this.RowHeadersVisible)
                    {
                        // If headers (rows or columns) are autosized, then this.RowHeadersWidth or this.ColumnHeadersHeight
                        // must be updated based on new cell preferred size
                        OnColumnHeadersGlobalAutoSize();
                        // In all cases, the top left cell needs to repaint
                        Invalidate(new Rectangle(this.layout.Inside.X, this.layout.Inside.Y, this.RowHeadersWidth, this.ColumnHeadersHeight));
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
                if (this.dataGridViewState1[DATAGRIDVIEWSTATE1_customCursorSet])
                {
                    return this.oldCursor;
                }
                else
                {
                    return this.Cursor;
                }
            }
        }

        internal int VerticalOffset
        {
            get
            {
                return this.verticalOffset;
            }
            set 
            {
                if (value < 0)
                {
                    value = 0;
                }
                int totalVisibleFrozenHeight = this.Rows.GetRowsHeight(DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen);
                int fittingTrailingScrollingRowsHeight = ComputeHeightOfFittingTrailingScrollingRows(totalVisibleFrozenHeight);
                if (value > this.vertScrollBar.Maximum - fittingTrailingScrollingRowsHeight)
                {
                    value = this.vertScrollBar.Maximum - fittingTrailingScrollingRowsHeight;
                }
                if (value == this.verticalOffset)
                {
                    return;
                }

                int change = value - this.verticalOffset;
                if (this.vertScrollBar.Enabled)
                {
                    this.vertScrollBar.Value = value;
                }
                ScrollRowsByHeight(change); // calculate how many rows need to be scrolled based on 'change'
            }
        }


        protected ScrollBar VerticalScrollBar
        {
            get
            {
                return this.vertScrollBar;
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
                return this.verticalOffset;
            }
        }

        private System.Windows.Forms.Timer VertScrollTimer
        {
            get
            {
                if (this.vertScrollTimer == null)
                {
                    this.vertScrollTimer = new System.Windows.Forms.Timer();
                    this.vertScrollTimer.Tick += new System.EventHandler(VertScrollTimer_Tick);
                }
                return this.vertScrollTimer;
            }
        }


        /// <devdoc>
        ///    <para>
        ///    </para>
        /// </devdoc>
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
                return this.dataGridViewState1[DATAGRIDVIEWSTATE1_virtualMode];
            }
            set
            {
                if (this.dataGridViewState1[DATAGRIDVIEWSTATE1_virtualMode] != value)
                {
                    this.dataGridViewState1[DATAGRIDVIEWSTATE1_virtualMode] = value;
                    InvalidateRowHeights();
                    //OnVirtualModeChanged(EventArgs.Empty);
                }
            }
        }

        private bool VisibleCellExists
        {
            get
            {
                if (null == this.Columns.GetFirstColumn(DataGridViewElementStates.Visible))
                {
                    return false;
                }
                return -1 != this.Rows.GetFirstRow(DataGridViewElementStates.Visible);
            }
        }

        // Events start here


        [
            SRCategory(nameof(SR.CatPropertyChanged)), 
            SRDescription(nameof(SR.DataGridViewAutoSizeColumnModeChangedDescr))
        ]
        public event DataGridViewAutoSizeColumnModeEventHandler AutoSizeColumnModeChanged
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWAUTOSIZECOLUMNMODECHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWAUTOSIZECOLUMNMODECHANGED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatAction)),
            SRDescription(nameof(SR.DataGridView_CancelRowEditDescr))
        ]
        public event QuestionEventHandler CancelRowEdit
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCANCELROWEDIT, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCANCELROWEDIT, value);
            }
        }


        [
            SRCategory(nameof(SR.CatData)),
            SRDescription(nameof(SR.DataGridView_CellBeginEditDescr))
        ]
        public event DataGridViewCellCancelEventHandler CellBeginEdit 
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLBEGINEDIT, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLBEGINEDIT, value);
            }
        }


        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_CellClickDescr))
        ]
        public event DataGridViewCellEventHandler CellClick
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLCLICK, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLCLICK, value);
            }
        }


        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_CellContentClick))
        ]
        public event DataGridViewCellEventHandler CellContentClick
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLCONTENTCLICK, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLCONTENTCLICK, value);
            }
        }


        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_CellContentDoubleClick))
        ]
        public event DataGridViewCellEventHandler CellContentDoubleClick
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLCONTENTDOUBLECLICK, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLCONTENTDOUBLECLICK, value);
            }
        }


        [
            SRCategory(nameof(SR.CatAction)),
            SRDescription(nameof(SR.DataGridView_CellContextMenuStripChanged)),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public event DataGridViewCellEventHandler CellContextMenuStripChanged
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLCONTEXTMENUSTRIPCHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLCONTEXTMENUSTRIPCHANGED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_CellContextMenuStripNeeded)),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public event DataGridViewCellContextMenuStripNeededEventHandler CellContextMenuStripNeeded
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLCONTEXTMENUSTRIPNEEDED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLCONTEXTMENUSTRIPNEEDED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_CellDoubleClickDescr))
        ]
        public event DataGridViewCellEventHandler CellDoubleClick
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLDOUBLECLICK, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLDOUBLECLICK, value);
            }
        }


        [
            SRCategory(nameof(SR.CatData)),
            SRDescription(nameof(SR.DataGridView_CellEndEditDescr))
        ]
        public event DataGridViewCellEventHandler CellEndEdit 
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLENDEDIT, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLENDEDIT, value);
            }
        }


        [
            SRCategory(nameof(SR.CatFocus)),
            SRDescription(nameof(SR.DataGridView_CellEnterDescr))
        ]
        public event DataGridViewCellEventHandler CellEnter
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLENTER, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLENTER, value);
            }
        }


        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_CellErrorTextChangedDescr))
        ]
        public event DataGridViewCellEventHandler CellErrorTextChanged
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLERRORTEXTCHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLERRORTEXTCHANGED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatData)),
            EditorBrowsable(EditorBrowsableState.Advanced),
            SRDescription(nameof(SR.DataGridView_CellErrorTextNeededDescr))
        ]
        public event DataGridViewCellErrorTextNeededEventHandler CellErrorTextNeeded
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLERRORTEXTNEEDED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLERRORTEXTNEEDED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatDisplay)),
            SRDescription(nameof(SR.DataGridView_CellFormattingDescr))
        ]
        public event DataGridViewCellFormattingEventHandler CellFormatting 
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLFORMATTING, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLFORMATTING, value);
            }
        }


        [
            SRCategory(nameof(SR.CatFocus)),
            SRDescription(nameof(SR.DataGridView_CellLeaveDescr))
        ]
        public event DataGridViewCellEventHandler CellLeave
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLLEAVE, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLLEAVE, value);
            }
        }


        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_CellMouseClickDescr))
        ]
        public event DataGridViewCellMouseEventHandler CellMouseClick
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLMOUSECLICK, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLMOUSECLICK, value);
            }
        }


        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_CellMouseDoubleClickDescr))
        ]
        public event DataGridViewCellMouseEventHandler CellMouseDoubleClick
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLMOUSEDOUBLECLICK, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLMOUSEDOUBLECLICK, value);
            }
        }


        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_CellMouseDownDescr))
        ]
        public event DataGridViewCellMouseEventHandler CellMouseDown
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLMOUSEDOWN, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLMOUSEDOWN, value);
            }
        }


        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_CellMouseEnterDescr))
        ]
        public event DataGridViewCellEventHandler CellMouseEnter
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLMOUSEENTER, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLMOUSEENTER, value);
            }
        }


        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_CellMouseLeaveDescr))
        ]
        public event DataGridViewCellEventHandler CellMouseLeave
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLMOUSELEAVE, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLMOUSELEAVE, value);
            }
        }


        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_CellMouseMoveDescr))
        ]
        public event DataGridViewCellMouseEventHandler CellMouseMove
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLMOUSEMOVE, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLMOUSEMOVE, value);
            }
        }


        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_CellMouseUpDescr))
        ]
        public event DataGridViewCellMouseEventHandler CellMouseUp 
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLMOUSEUP, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLMOUSEUP, value);
            }
        }


        [
            SRCategory(nameof(SR.CatDisplay)),
            SRDescription(nameof(SR.DataGridView_CellPaintingDescr))
        ]
        public event DataGridViewCellPaintingEventHandler CellPainting 
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLPAINTING, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLPAINTING, value);
            }
        }


        [
            SRCategory(nameof(SR.CatDisplay)),
            SRDescription(nameof(SR.DataGridView_CellParsingDescr))
        ]
        public event DataGridViewCellParsingEventHandler CellParsing
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLPARSING, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLPARSING, value);
            }
        }


        [
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_CellStateChangedDescr))
        ]
        public event DataGridViewCellStateChangedEventHandler CellStateChanged 
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLSTATECHANGED, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLSTATECHANGED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_CellStyleChangedDescr))
        ]
        public event DataGridViewCellEventHandler CellStyleChanged 
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLSTYLECHANGED, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLSTYLECHANGED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_CellStyleContentChangedDescr))
        ]
        public event DataGridViewCellStyleContentChangedEventHandler CellStyleContentChanged
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLSTYLECONTENTCHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLSTYLECONTENTCHANGED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_CellToolTipTextChangedDescr))
        ]
        public event DataGridViewCellEventHandler CellToolTipTextChanged
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLTOOLTIPTEXTCHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLTOOLTIPTEXTCHANGED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_CellToolTipTextNeededDescr)),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public event DataGridViewCellToolTipTextNeededEventHandler CellToolTipTextNeeded
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLTOOLTIPTEXTNEEDED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLTOOLTIPTEXTNEEDED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatFocus)),
            SRDescription(nameof(SR.DataGridView_CellValidatedDescr))
        ]
        public event DataGridViewCellEventHandler CellValidated
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLVALIDATED, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLVALIDATED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatFocus)),
            SRDescription(nameof(SR.DataGridView_CellValidatingDescr))
        ]
        public event DataGridViewCellValidatingEventHandler CellValidating
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLVALIDATING, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLVALIDATING, value);
            }
        }


        [
            SRCategory(nameof(SR.CatAction)),
            SRDescription(nameof(SR.DataGridView_CellValueChangedDescr))
        ]
        public event DataGridViewCellEventHandler CellValueChanged 
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLVALUECHANGED, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLVALUECHANGED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatData)),
            EditorBrowsable(EditorBrowsableState.Advanced),
            SRDescription(nameof(SR.DataGridView_CellValueNeededDescr))
        ]
        public event DataGridViewCellValueEventHandler CellValueNeeded 
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLVALUENEEDED, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLVALUENEEDED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatData)),
            EditorBrowsable(EditorBrowsableState.Advanced),
            SRDescription(nameof(SR.DataGridView_CellValuePushedDescr))
        ]
        public event DataGridViewCellValueEventHandler CellValuePushed 
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCELLVALUEPUSHED, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCELLVALUEPUSHED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatAction)),
            SRDescription(nameof(SR.DataGridView_ColumnAddedDescr))
        ]
        public event DataGridViewColumnEventHandler ColumnAdded
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNADDED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNADDED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_ColumnContextMenuStripChangedDescr))
        ]
        public event DataGridViewColumnEventHandler ColumnContextMenuStripChanged
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNCONTEXTMENUSTRIPCHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNCONTEXTMENUSTRIPCHANGED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_ColumnDataPropertyNameChangedDescr))
        ]
        public event DataGridViewColumnEventHandler ColumnDataPropertyNameChanged
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNDATAPROPERTYNAMECHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNDATAPROPERTYNAMECHANGED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_ColumnDefaultCellStyleChangedDescr))
        ]
        public event DataGridViewColumnEventHandler ColumnDefaultCellStyleChanged
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNDEFAULTCELLSTYLECHANGED, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNDEFAULTCELLSTYLECHANGED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_ColumnDisplayIndexChangedDescr))
        ]
        public event DataGridViewColumnEventHandler ColumnDisplayIndexChanged 
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNDISPLAYINDEXCHANGED, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNDISPLAYINDEXCHANGED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_ColumnDividerDoubleClickDescr))
        ]
        public event DataGridViewColumnDividerDoubleClickEventHandler ColumnDividerDoubleClick
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNDIVIDERDOUBLECLICK, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNDIVIDERDOUBLECLICK, value);
            }
        }


        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_ColumnDividerWidthChangedDescr))
        ]
        public event DataGridViewColumnEventHandler ColumnDividerWidthChanged
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNDIVIDERWIDTHCHANGED, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNDIVIDERWIDTHCHANGED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_ColumnHeaderMouseClickDescr))
        ]
        public event DataGridViewCellMouseEventHandler ColumnHeaderMouseClick 
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNHEADERMOUSECLICK, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNHEADERMOUSECLICK, value);
            }
        }


        [
            SRCategory(nameof(SR.CatMouse)), 
            SRDescription(nameof(SR.DataGridView_ColumnHeaderMouseDoubleClickDescr))
        ]
        public event DataGridViewCellMouseEventHandler ColumnHeaderMouseDoubleClick
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNHEADERMOUSEDOUBLECLICK, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNHEADERMOUSEDOUBLECLICK, value);
            }
        }


        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_ColumnHeaderCellChangedDescr))
        ]
        public event DataGridViewColumnEventHandler ColumnHeaderCellChanged 
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNHEADERCELLCHANGED, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNHEADERCELLCHANGED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_ColumnMinimumWidthChangedDescr))
        ]
        public event DataGridViewColumnEventHandler ColumnMinimumWidthChanged 
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNMINIMUMWIDTHCHANGED, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNMINIMUMWIDTHCHANGED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_ColumnNameChangedDescr))
        ]
        public event DataGridViewColumnEventHandler ColumnNameChanged
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNNAMECHANGED, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNNAMECHANGED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatAction)),
            SRDescription(nameof(SR.DataGridView_ColumnRemovedDescr))
        ]
        public event DataGridViewColumnEventHandler ColumnRemoved
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNREMOVED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNREMOVED, value);
            }
        }



        public event EventHandler ColumnsDefaultCellStyleChanged
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNSDEFAULTCELLSTYLECHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNSDEFAULTCELLSTYLECHANGED, value);
            }
        }*/


        [
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridViewColumnSortModeChangedDescr))
        ]
        public event DataGridViewColumnEventHandler ColumnSortModeChanged
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNSORTMODECHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNSORTMODECHANGED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_ColumnStateChangedDescr))
        ]
        public event DataGridViewColumnStateChangedEventHandler ColumnStateChanged 
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNSTATECHANGED, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNSTATECHANGED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_ColumnToolTipTextChangedDescr))
        ]
        public event DataGridViewColumnEventHandler ColumnToolTipTextChanged
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNTOOLTIPTEXTCHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNTOOLTIPTEXTCHANGED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatAction)),
            SRDescription(nameof(SR.DataGridView_ColumnWidthChangedDescr))
        ]
        public event DataGridViewColumnEventHandler ColumnWidthChanged 
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCOLUMNWIDTHCHANGED, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCOLUMNWIDTHCHANGED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatAction)), 
            SRDescription(nameof(SR.DataGridView_CurrentCellChangedDescr))
        ]
        public event EventHandler CurrentCellChanged
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCURRENTCELLCHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCURRENTCELLCHANGED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatBehavior)), 
            EditorBrowsable(EditorBrowsableState.Advanced),
            SRDescription(nameof(SR.DataGridView_CurrentCellDirtyStateChangedDescr))
        ]
        public event EventHandler CurrentCellDirtyStateChanged
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWCURRENTCELLDIRTYSTATECHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWCURRENTCELLDIRTYSTATECHANGED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatData)), 
            SRDescription(nameof(SR.DataGridView_DataBindingCompleteDescr))
        ]
        public event DataGridViewBindingCompleteEventHandler DataBindingComplete
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWDATABINDINGCOMPLETE, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWDATABINDINGCOMPLETE, value);
            }
        }


        [
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_DataErrorDescr))
        ]
        public event DataGridViewDataErrorEventHandler DataError
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWDATAERROR, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWDATAERROR, value);
            }
        }


        [
            SRCategory(nameof(SR.CatData)), 
            EditorBrowsable(EditorBrowsableState.Advanced),
            SRDescription(nameof(SR.DataGridView_DefaultValuesNeededDescr))
        ]
        public event DataGridViewRowEventHandler DefaultValuesNeeded
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWDEFAULTVALUESNEEDED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWDEFAULTVALUESNEEDED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatAction)),
            SRDescription(nameof(SR.DataGridView_EditingControlShowingDescr))
        ]
        public event DataGridViewEditingControlShowingEventHandler EditingControlShowing
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWEDITINGCONTROLSHOWING, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWEDITINGCONTROLSHOWING, value);
            }
        }

        /*

        public event QuestionEventHandler KeepNewRow
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWKEEPNEWROW, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWKEEPNEWROW, value);
            }
        }*/



        public event EventHandler NewRowDiscarded
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWNEWROWDISCARDED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWNEWROWDISCARDED, value);
            }
        }*/


        [
            SRCategory(nameof(SR.CatData)),
            SRDescription(nameof(SR.DataGridView_NewRowNeededDescr))
        ]
        public event DataGridViewRowEventHandler NewRowNeeded
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWNEWROWNEEDED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWNEWROWNEEDED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_RowContextMenuStripChangedDescr))
        ]
        public event DataGridViewRowEventHandler RowContextMenuStripChanged
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWROWCONTEXTMENUSTRIPCHANGED, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWROWCONTEXTMENUSTRIPCHANGED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatData)),
            EditorBrowsable(EditorBrowsableState.Advanced),
            SRDescription(nameof(SR.DataGridView_RowContextMenuStripNeededDescr))
        ]
        public event DataGridViewRowContextMenuStripNeededEventHandler RowContextMenuStripNeeded
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWROWCONTEXTMENUSTRIPNEEDED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWROWCONTEXTMENUSTRIPNEEDED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_RowDefaultCellStyleChangedDescr))
        ]
        public event DataGridViewRowEventHandler RowDefaultCellStyleChanged 
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWROWDEFAULTCELLSTYLECHANGED, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWROWDEFAULTCELLSTYLECHANGED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatData)),
            EditorBrowsable(EditorBrowsableState.Advanced),
            SRDescription(nameof(SR.DataGridView_RowDirtyStateNeededDescr))
        ]
        public event QuestionEventHandler RowDirtyStateNeeded
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWROWDIRTYSTATENEEDED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWROWDIRTYSTATENEEDED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_RowDividerDoubleClickDescr))
        ]
        public event DataGridViewRowDividerDoubleClickEventHandler RowDividerDoubleClick
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWROWDIVIDERDOUBLECLICK, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWROWDIVIDERDOUBLECLICK, value);
            }
        }


        [
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_RowDividerHeightChangedDescr))
        ]
        public event DataGridViewRowEventHandler RowDividerHeightChanged
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWROWDIVIDERHEIGHTCHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWROWDIVIDERHEIGHTCHANGED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatFocus)),
            SRDescription(nameof(SR.DataGridView_RowEnterDescr))
        ]
        public event DataGridViewCellEventHandler RowEnter
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWROWENTER, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWROWENTER, value);
            }
        }


        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_RowErrorTextChangedDescr))
        ]
        public event DataGridViewRowEventHandler RowErrorTextChanged
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWROWERRORTEXTCHANGED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWROWERRORTEXTCHANGED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatData)),
            EditorBrowsable(EditorBrowsableState.Advanced),
            SRDescription(nameof(SR.DataGridView_RowErrorTextNeededDescr))
        ]
        public event DataGridViewRowErrorTextNeededEventHandler RowErrorTextNeeded
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWROWERRORTEXTNEEDED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWROWERRORTEXTNEEDED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_RowHeaderMouseClickDescr))
        ]
        public event DataGridViewCellMouseEventHandler RowHeaderMouseClick 
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWROWHEADERMOUSECLICK, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWROWHEADERMOUSECLICK, value);
            }
        }


        [
            SRCategory(nameof(SR.CatMouse)),
            SRDescription(nameof(SR.DataGridView_RowHeaderMouseDoubleClickDescr))
        ]
        public event DataGridViewCellMouseEventHandler RowHeaderMouseDoubleClick 
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWROWHEADERMOUSEDOUBLECLICK, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWROWHEADERMOUSEDOUBLECLICK, value);
            }
        }


        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_RowHeaderCellChangedDescr))
        ]
        public event DataGridViewRowEventHandler RowHeaderCellChanged 
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWROWHEADERCELLCHANGED, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWROWHEADERCELLCHANGED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_RowHeightChangedDescr))
        ]
        public event DataGridViewRowEventHandler RowHeightChanged 
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWROWHEIGHTCHANGED, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWROWHEIGHTCHANGED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatData)),
            EditorBrowsable(EditorBrowsableState.Advanced),
            SRDescription(nameof(SR.DataGridView_RowHeightInfoNeededDescr))
        ]
        public event DataGridViewRowHeightInfoNeededEventHandler RowHeightInfoNeeded
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWROWHEIGHTINFONEEDED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWROWHEIGHTINFONEEDED, value);
            }
        }

        internal DataGridViewRowHeightInfoNeededEventArgs RowHeightInfoNeededEventArgs
        {
            get
            {
                if (this.dgvrhine == null)
                {
                    this.dgvrhine = new DataGridViewRowHeightInfoNeededEventArgs();
                }
                return this.dgvrhine;
            }
        }


        [
            SRCategory(nameof(SR.CatData)),
            EditorBrowsable(EditorBrowsableState.Advanced),
            SRDescription(nameof(SR.DataGridView_RowHeightInfoPushedDescr))
        ]
        public event DataGridViewRowHeightInfoPushedEventHandler RowHeightInfoPushed
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWROWHEIGHTINFOPUSHED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWROWHEIGHTINFOPUSHED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatFocus)),
            SRDescription(nameof(SR.DataGridView_RowLeaveDescr))
        ]
        public event DataGridViewCellEventHandler RowLeave
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWROWLEAVE, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWROWLEAVE, value);
            }
        }


        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.DataGridView_RowMinimumHeightChangedDescr))
        ]
        public event DataGridViewRowEventHandler RowMinimumHeightChanged 
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWROWMINIMUMHEIGHTCHANGED, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWROWMINIMUMHEIGHTCHANGED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatDisplay)), 
            SRDescription(nameof(SR.DataGridView_RowPostPaintDescr))
        ]
        public event DataGridViewRowPostPaintEventHandler RowPostPaint
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWROWPOSTPAINT, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWROWPOSTPAINT, value);
            }
        }

        internal DataGridViewRowPostPaintEventArgs RowPostPaintEventArgs
        {
            get
            {
                if (this.dgvrpope == null)
                {
                    this.dgvrpope = new DataGridViewRowPostPaintEventArgs(this);
                }
                return this.dgvrpope;
            }
        }


        [
            SRCategory(nameof(SR.CatDisplay)), 
            SRDescription(nameof(SR.DataGridView_RowPrePaintDescr))
        ]
        public event DataGridViewRowPrePaintEventHandler RowPrePaint
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWROWPREPAINT, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWROWPREPAINT, value);
            }
        }

        internal DataGridViewRowPrePaintEventArgs RowPrePaintEventArgs
        {
            get
            {
                if (this.dgvrprpe == null)
                {
                    this.dgvrprpe = new DataGridViewRowPrePaintEventArgs(this);
                }
                return this.dgvrprpe;
            }
        }


        [
            SRCategory(nameof(SR.CatAction)), 
            SRDescription(nameof(SR.DataGridView_RowsAddedDescr))
        ]
        public event DataGridViewRowsAddedEventHandler RowsAdded
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWROWSADDED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWROWSADDED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatAction)),
            SRDescription(nameof(SR.DataGridView_RowsRemovedDescr))
        ]
        public event DataGridViewRowsRemovedEventHandler RowsRemoved
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWROWSREMOVED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWROWSREMOVED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_RowStateChangedDescr))
        ]
        public event DataGridViewRowStateChangedEventHandler RowStateChanged 
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWROWSTATECHANGED, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWROWSTATECHANGED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatBehavior)),
            EditorBrowsable(EditorBrowsableState.Advanced),
            SRDescription(nameof(SR.DataGridView_RowUnsharedDescr))
        ] 
        public event DataGridViewRowEventHandler RowUnshared 
        {
            add 
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWROWUNSHARED, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWROWUNSHARED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatFocus)),
            SRDescription(nameof(SR.DataGridView_RowValidatedDescr))
        ]
        public event DataGridViewCellEventHandler RowValidated
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWROWVALIDATED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWROWVALIDATED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatFocus)),
            SRDescription(nameof(SR.DataGridView_RowValidatingDescr))
        ]
        public event DataGridViewCellCancelEventHandler RowValidating
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWROWVALIDATING, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWROWVALIDATING, value);
            }
        }


        [
            SRCategory(nameof(SR.CatAction)), 
            SRDescription(nameof(SR.DataGridView_ScrollDescr))
        ]
        public event ScrollEventHandler Scroll
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWSCROLL, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWSCROLL, value);
            }
        }


        [
            SRCategory(nameof(SR.CatAction)),
            SRDescription(nameof(SR.DataGridView_SelectionChangedDescr))
        ]
        public event EventHandler SelectionChanged
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWSELECTIONCHANGED, value);
            }
            remove 
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWSELECTIONCHANGED, value);
            }
        }


        [
            SRCategory(nameof(SR.CatData)),
            EditorBrowsable(EditorBrowsableState.Advanced),
            SRDescription(nameof(SR.DataGridView_SortCompareDescr))
        ]
        public event DataGridViewSortCompareEventHandler SortCompare
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWSORTCOMPARE, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWSORTCOMPARE, value);
            }
        }


        [
            SRCategory(nameof(SR.CatData)),
            SRDescription(nameof(SR.DataGridView_SortedDescr))
        ]
        public event EventHandler Sorted
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWSORTED, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWSORTED, value);
            }
        }


        /// <internalonly/>
        [
            Browsable(false), 
            EditorBrowsable(EditorBrowsableState.Never)
        ]
        new public event EventHandler StyleChanged
        {
            add
            {
                base.StyleChanged += value;
            }
            remove
            {
                base.StyleChanged -= value;
            }
        }


        [
            SRCategory(nameof(SR.CatAction)),
            SRDescription(nameof(SR.DataGridView_UserAddedRowDescr))
        ]
        public event DataGridViewRowEventHandler UserAddedRow
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWUSERADDEDROW, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWUSERADDEDROW, value);
            }
        }



        public event DataGridViewRowCancelEventHandler UserAddingRow
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWUSERADDINGROW, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWUSERADDINGROW, value);
            }
        }*/


        [
            SRCategory(nameof(SR.CatAction)),
            SRDescription(nameof(SR.DataGridView_UserDeletedRowDescr))
        ]
        public event DataGridViewRowEventHandler UserDeletedRow
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWUSERDELETEDROW, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWUSERDELETEDROW, value);
            }
        }


        [
            SRCategory(nameof(SR.CatAction)),
            SRDescription(nameof(SR.DataGridView_UserDeletingRowDescr))
        ]
        public event DataGridViewRowCancelEventHandler UserDeletingRow
        {
            add
            {
                this.Events.AddHandler(EVENT_DATAGRIDVIEWUSERDELETINGROW, value);
            }
            remove
            {
                this.Events.RemoveHandler(EVENT_DATAGRIDVIEWUSERDELETINGROW, value);
            }
        }

        ////////////////////////
        //                    //
        // ISupportInitialize //
        //                    //
        ////////////////////////
        [
            SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")
        ]
        void ISupportInitialize.BeginInit()
        {
            if (this.dataGridViewState2[DATAGRIDVIEWSTATE2_initializing])
            {
                throw new InvalidOperationException(SR.DataGridViewBeginInit);
            }

            this.dataGridViewState2[DATAGRIDVIEWSTATE2_initializing] = true;
        }

        [
            SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")
        ]
        void ISupportInitialize.EndInit()
        {
            this.dataGridViewState2[DATAGRIDVIEWSTATE2_initializing] = false;

            foreach (DataGridViewColumn dataGridViewColumn in this.Columns)
            {
                if (dataGridViewColumn.Frozen &&
                    dataGridViewColumn.Visible &&
                    dataGridViewColumn.InheritedAutoSizeMode == DataGridViewAutoSizeColumnMode.Fill)
                {
                    dataGridViewColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                }
            }

            DataGridViewSelectionMode selectionMode = this.SelectionMode;
            if (selectionMode == DataGridViewSelectionMode.FullColumnSelect || selectionMode == DataGridViewSelectionMode.ColumnHeaderSelect)
            {
                foreach (DataGridViewColumn dataGridViewColumn in this.Columns)
                {
                    if (dataGridViewColumn.SortMode == DataGridViewColumnSortMode.Automatic)
                    {
                        // Resetting SelectionMode to its acceptable default value. We don't want the control to ever end up in an invalid state.
                        this.SelectionMode = defaultSelectionMode; // DataGridViewSelectionMode.RowHeaderSelect
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
            private DataGridView owningDataGridView;

            public DataGridViewEditingPanel(DataGridView owningDataGridView)
            {
                this.owningDataGridView = owningDataGridView;
            }

            internal override bool SupportsUiaProviders
            {
                get
                {
                    return AccessibilityImprovements.Level3;
                }
            }

            protected override AccessibleObject CreateAccessibilityInstance()
            {
                if (AccessibilityImprovements.Level3)
                {
                    return new DataGridViewEditingPanelAccessibleObject(owningDataGridView, this);
                }

                return base.CreateAccessibilityInstance();
            }
        }
    }
}
